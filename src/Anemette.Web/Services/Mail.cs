using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Anemette.Web.Services;

public class SmtpIndstillinger
{
    public string Server { get; set; } = "websmtp.simply.com";
    public int Port { get; set; } = 587;
    public string Bruger { get; set; } = "";
    public string Kodeord { get; set; } = "";

    public bool ErSatOp => !string.IsNullOrWhiteSpace(Bruger) && !string.IsNullOrWhiteSpace(Kodeord);
}

/// <summary>Sender mails via Simply.com's mailserver. Mangler opsætningen, gemmes ordrer m.m. stadig i databasen.</summary>
public class Mail(Microsoft.Extensions.Options.IOptions<SmtpIndstillinger> options, ILogger<Mail> log)
{
    private readonly SmtpIndstillinger _smtp = options.Value;

    public bool ErSatOp => _smtp.ErSatOp;

    public async Task<bool> SendAsync(string til, string emne, string tekst, string? svarTil = null, string? afsenderNavn = null)
    {
        if (!_smtp.ErSatOp)
        {
            log.LogWarning("Mail til {Til} ({Emne}) blev ikke sendt, fordi SMTP ikke er sat op", til, emne);
            return false;
        }
        try
        {
            var besked = new MimeMessage();
            // Simply tillader kun at sende fra egen mailkonto; kundens adresse sættes som svar-adresse
            besked.From.Add(new MailboxAddress(afsenderNavn ?? "Skarresøhus Forlag", _smtp.Bruger));
            besked.To.Add(MailboxAddress.Parse(til));
            if (!string.IsNullOrWhiteSpace(svarTil))
                besked.ReplyTo.Add(MailboxAddress.Parse(svarTil));
            besked.Subject = emne;
            besked.Body = new TextPart("plain") { Text = tekst };

            using var klient = new SmtpClient();
            await klient.ConnectAsync(_smtp.Server, _smtp.Port, SecureSocketOptions.StartTls);
            await klient.AuthenticateAsync(_smtp.Bruger, _smtp.Kodeord);
            await klient.SendAsync(besked);
            await klient.DisconnectAsync(true);
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Kunne ikke sende mail til {Til} ({Emne})", til, emne);
            return false;
        }
    }
}
