using System.Net;
using System.Text.Json;
using Anemette.Web.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Anemette.Tests;

public class MailTests
{
    /// <summary>Spiller Microsoft: svarer på login og sendMail og husker forespørgslerne.</summary>
    private sealed class FalskMicrosoft(HttpStatusCode sendStatus = HttpStatusCode.Accepted) : HttpMessageHandler
    {
        public List<(HttpRequestMessage Request, string Body)> Kald { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Kald.Add((request, request.Content is null ? "" : await request.Content.ReadAsStringAsync(ct)));
            if (request.RequestUri!.Host == "login.microsoftonline.com")
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("""{"access_token":"tok123","expires_in":3600}""") };
            return new HttpResponseMessage(sendStatus);
        }
    }

    private static Mail NyMail(FalskMicrosoft handler, string tenant) => new(
        new HttpClient(handler),
        Options.Create(new GraphIndstillinger { TenantId = tenant, KlientId = "klient", Hemmelighed = "hemmelig", Afsender = "anemette@anemetteolesen.dk" }),
        NullLogger<Mail>.Instance);

    [Fact]
    public async Task Sender_via_Graph_fra_Anemettes_postkasse_med_svar_adresse()
    {
        var ms = new FalskMicrosoft();
        var ok = await NyMail(ms, Guid.NewGuid().ToString()).SendAsync("kunde@example.com", "Tak for din bestilling", "Hej Grethe", svarTil: "anemette@anemetteolesen.dk");

        Assert.True(ok);
        Assert.Equal(2, ms.Kald.Count);
        Assert.Contains("grant_type=client_credentials", ms.Kald[0].Body);
        Assert.Contains("scope=https%3A%2F%2Fgraph.microsoft.com%2F.default", ms.Kald[0].Body);

        var (send, body) = ms.Kald[1];
        Assert.Equal("https://graph.microsoft.com/v1.0/users/anemette%40anemetteolesen.dk/sendMail", send.RequestUri!.ToString());
        Assert.Equal("Bearer tok123", send.Headers.Authorization!.ToString());
        var besked = JsonDocument.Parse(body).RootElement.GetProperty("message");
        Assert.Equal("Tak for din bestilling", besked.GetProperty("subject").GetString());
        Assert.Equal("Hej Grethe", besked.GetProperty("body").GetProperty("content").GetString());
        Assert.Equal("kunde@example.com", besked.GetProperty("toRecipients")[0].GetProperty("emailAddress").GetProperty("address").GetString());
        Assert.Equal("anemette@anemetteolesen.dk", besked.GetProperty("replyTo")[0].GetProperty("emailAddress").GetProperty("address").GetString());
    }

    [Fact]
    public async Task Adgangsnoeglen_genbruges_til_flere_mails()
    {
        var ms = new FalskMicrosoft();
        var mail = NyMail(ms, Guid.NewGuid().ToString());
        await mail.SendAsync("a@example.com", "1", "x");
        await mail.SendAsync("b@example.com", "2", "x");
        Assert.Single(ms.Kald, k => k.Request.RequestUri!.Host == "login.microsoftonline.com");
    }

    [Fact]
    public async Task Afvisning_fra_Microsoft_giver_false()
    {
        var ms = new FalskMicrosoft(HttpStatusCode.Forbidden);
        Assert.False(await NyMail(ms, Guid.NewGuid().ToString()).SendAsync("a@example.com", "x", "x"));
    }

    [Fact]
    public async Task Uden_opsaetning_sendes_intet()
    {
        var ms = new FalskMicrosoft();
        var mail = new Mail(new HttpClient(ms), Options.Create(new GraphIndstillinger()), NullLogger<Mail>.Instance);
        Assert.False(mail.ErSatOp);
        Assert.False(await mail.SendAsync("a@example.com", "x", "x"));
        Assert.Empty(ms.Kald);
    }
}
