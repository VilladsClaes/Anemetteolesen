using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace Anemette.Web.Services;

/// <summary>Adgang til Microsoft 365 via en app-registrering (se README.md → Mail).</summary>
public class GraphIndstillinger
{
    public string TenantId { get; set; } = "";
    public string KlientId { get; set; } = "";
    public string Hemmelighed { get; set; } = "";
    /// <summary>Postkassen, der sendes fra. Kundens adresse sættes som svar-adresse.</summary>
    public string Afsender { get; set; } = "anemette@anemetteolesen.dk";

    public bool ErSatOp =>
        !string.IsNullOrWhiteSpace(TenantId) && !string.IsNullOrWhiteSpace(KlientId) &&
        !string.IsNullOrWhiteSpace(Hemmelighed) && !string.IsNullOrWhiteSpace(Afsender);
}

/// <summary>
/// Sender mails fra Anemettes Microsoft 365-postkasse via Microsoft Graph.
/// Domænets SPF tillader kun Microsoft at sende fra @anemetteolesen.dk, så almindelig SMTP hos Simply kan ikke bruges.
/// Mangler opsætningen, gemmes ordrer, tilmeldinger og beskeder stadig i databasen.
/// </summary>
public class Mail(HttpClient http, IOptions<GraphIndstillinger> options, ILogger<Mail> log)
{
    private readonly GraphIndstillinger _graph = options.Value;

    // Adgangsnøglen fra Microsoft gælder ca. en time og genbruges imens
    private static readonly SemaphoreSlim Laas = new(1, 1);
    private static (string Noegle, string Token, DateTimeOffset Udloeber)? _cache;

    public bool ErSatOp => _graph.ErSatOp;
    public string Afsender => _graph.Afsender;

    public async Task<bool> SendAsync(string til, string emne, string tekst, string? svarTil = null)
    {
        if (!_graph.ErSatOp)
        {
            log.LogWarning("Mail til {Til} ({Emne}) blev ikke sendt, fordi mail via Microsoft 365 ikke er sat op", til, emne);
            return false;
        }
        try
        {
            var token = await HentTokenAsync();
            var besked = new SendMail(new Besked(
                emne,
                new Indhold("Text", tekst),
                [new Modtager(new Adresse(til))],
                string.IsNullOrWhiteSpace(svarTil) ? [] : [new Modtager(new Adresse(svarTil))]));

            using var request = new HttpRequestMessage(HttpMethod.Post,
                $"https://graph.microsoft.com/v1.0/users/{Uri.EscapeDataString(_graph.Afsender)}/sendMail")
            {
                Content = JsonContent.Create(besked),
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var svar = await http.SendAsync(request);
            if (svar.IsSuccessStatusCode) return true;

            log.LogError("Microsoft 365 afviste mail til {Til} ({Emne}): {Status} {Svar}",
                til, emne, (int)svar.StatusCode, await svar.Content.ReadAsStringAsync());
            return false;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Kunne ikke sende mail til {Til} ({Emne})", til, emne);
            return false;
        }
    }

    private async Task<string> HentTokenAsync()
    {
        var noegle = $"{_graph.TenantId}|{_graph.KlientId}";
        await Laas.WaitAsync();
        try
        {
            if (_cache is { } c && c.Noegle == noegle && c.Udloeber > DateTimeOffset.UtcNow.AddMinutes(5))
                return c.Token;

            using var svar = await http.PostAsync(
                $"https://login.microsoftonline.com/{Uri.EscapeDataString(_graph.TenantId)}/oauth2/v2.0/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = _graph.KlientId,
                    ["client_secret"] = _graph.Hemmelighed,
                    ["scope"] = "https://graph.microsoft.com/.default",
                    ["grant_type"] = "client_credentials",
                }));
            if (!svar.IsSuccessStatusCode)
                throw new InvalidOperationException($"Login til Microsoft 365 mislykkedes ({(int)svar.StatusCode}): {await svar.Content.ReadAsStringAsync()}");
            var token = await svar.Content.ReadFromJsonAsync<TokenSvar>()
                ?? throw new InvalidOperationException("Tomt svar fra Microsoft 365.");
            _cache = (noegle, token.AccessToken, DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn));
            return token.AccessToken;
        }
        finally
        {
            Laas.Release();
        }
    }

    // Microsoft Graph sendMail: https://learn.microsoft.com/graph/api/user-sendmail
    private record SendMail([property: JsonPropertyName("message")] Besked Message,
                            [property: JsonPropertyName("saveToSentItems")] bool SaveToSentItems = true);
    private record Besked([property: JsonPropertyName("subject")] string Subject,
                          [property: JsonPropertyName("body")] Indhold Body,
                          [property: JsonPropertyName("toRecipients")] Modtager[] ToRecipients,
                          [property: JsonPropertyName("replyTo")] Modtager[] ReplyTo);
    private record Indhold([property: JsonPropertyName("contentType")] string ContentType,
                           [property: JsonPropertyName("content")] string Content);
    private record Modtager([property: JsonPropertyName("emailAddress")] Adresse EmailAddress);
    private record Adresse([property: JsonPropertyName("address")] string Address);
    private record TokenSvar([property: JsonPropertyName("access_token")] string AccessToken,
                             [property: JsonPropertyName("expires_in")] int ExpiresIn);
}
