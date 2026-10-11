using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.DataProtection;

namespace Anemette.Web.Services;

public enum SpamVurdering
{
    /// <summary>Ser ud til at komme fra et menneske.</summary>
    Ok,
    /// <summary>Fælde-feltet er udfyldt, tidsstemplet mangler, eller formularen blev sendt for hurtigt.</summary>
    Robot,
    /// <summary>Siden har været åben i meget lang tid – et menneske bør bare sende igen.</summary>
    ForGammel,
    /// <summary>Indholdet ligner spam (links, spamord, fremmed alfabet).</summary>
    Indhold,
}

/// <summary>
/// Beskytter de offentlige formularer mod spam uden CAPTCHA (som er besværlig for ældre brugere og sender data til tredjepart):
/// et skjult fælde-felt, et krypteret tidsstempel og en vurdering af indholdet.
/// Grænsen for antal indsendelser pr. afsender sættes i Program.cs (rate limiting).
/// </summary>
public partial class SpamVaern(IDataProtectionProvider dataProtection, ILogger<SpamVaern> log)
{
    public const string FeltTid = "formularTid";
    public const string FeltFaelde = "hjemmeside";

    /// <summary>Et menneske bruger mere end 3 sekunder på at udfylde en formular.</summary>
    internal static readonly TimeSpan MindsteTid = TimeSpan.FromSeconds(3);
    internal static readonly TimeSpan LaengsteTid = TimeSpan.FromHours(12);

    private readonly IDataProtector _beskytter = dataProtection.CreateProtector("Anemette.Formularstempel");

    public string NytStempel() => _beskytter.Protect(DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture));

    /// <param name="form">Den indsendte formular.</param>
    /// <param name="tekster">Fritekstfelter der skal vurderes (fx navn, emne og besked). Navnet skal stå først.</param>
    public SpamVurdering Vurder(IFormCollection form, params string?[] tekster)
    {
        var resultat = VurderInternt(form, tekster);
        if (resultat is SpamVurdering.Robot or SpamVurdering.Indhold)
            log.LogInformation("Formular afvist som spam ({Grund})", resultat);
        return resultat;
    }

    private SpamVurdering VurderInternt(IFormCollection form, string?[] tekster)
    {
        if (!string.IsNullOrWhiteSpace(form[FeltFaelde]))
            return SpamVurdering.Robot;

        try
        {
            var ticks = long.Parse(_beskytter.Unprotect(form[FeltTid].ToString()), CultureInfo.InvariantCulture);
            var alder = DateTime.UtcNow - new DateTime(ticks, DateTimeKind.Utc);
            if (alder < MindsteTid) return SpamVurdering.Robot;
            if (alder > LaengsteTid) return SpamVurdering.ForGammel;
        }
        catch (Exception ex) when (ex is System.Security.Cryptography.CryptographicException or FormatException or ArgumentException)
        {
            return SpamVurdering.Robot; // stemplet mangler eller er forfalsket
        }

        return LignerSpam(tekster) ? SpamVurdering.Indhold : SpamVurdering.Ok;
    }

    /// <summary>Vurderer fritekst. Det første felt er afsenderens navn, hvor links aldrig hører hjemme.</summary>
    internal static bool LignerSpam(params string?[] tekster)
    {
        var navn = tekster.FirstOrDefault() ?? "";
        var alt = string.Join("\n", tekster.Where(t => !string.IsNullOrEmpty(t)));
        if (alt.Length == 0) return false;

        if (LinkRegex().IsMatch(navn)) return true;
        if (LinkRegex().Matches(alt).Count >= 2) return true;
        if (SpamordRegex().IsMatch(alt)) return true;

        // Mest tekst i fremmede alfabeter (kyrillisk, kinesisk, arabisk ...) – Anemettes kunder skriver på dansk
        var bogstaver = alt.Where(char.IsLetter).ToList();
        if (bogstaver.Count >= 10 && bogstaver.Count(c => c > 'ɏ') > bogstaver.Count * 0.3) return true;

        return false;
    }

    [GeneratedRegex(@"https?://|www\.|\[url|<a\s|\.(ru|xyz|top|click|link|site|online)\b", RegexOptions.IgnoreCase)]
    private static partial Regex LinkRegex();

    [GeneratedRegex(@"\b(seo|backlinks?|casino|viagra|cialis|crypto|bitcoin|forex|porn|escort|payday|guest\s?post|link\s?building|web\s?traffic|website\s?traffic|ranking on google|first page of google|lead generation|whatsapp\s?me|telegram\s?me|unsubscribe)\b", RegexOptions.IgnoreCase)]
    private static partial Regex SpamordRegex();
}
