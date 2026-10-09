using Anemette.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Services;

/// <summary>Kontaktoplysninger m.m., som kan rettes under Administration → Indstillinger. Hentes én gang pr. forespørgsel.</summary>
public class Indstillinger(AppDbContext db)
{
    private Dictionary<string, string>? _vaerdier;

    public async Task<string> HentAsync(string noegle)
    {
        _vaerdier ??= await db.Indstillinger.AsNoTracking().ToDictionaryAsync(i => i.Noegle, i => i.Vaerdi);
        return _vaerdier.GetValueOrDefault(noegle, "");
    }

    public async Task<Kontaktinfo> KontaktAsync() => new(
        await HentAsync("Navn"),
        await HentAsync("Forlag"),
        await HentAsync("Adresse"),
        await HentAsync("Postnummer og by"),
        await HentAsync("Telefon"),
        await HentAsync("Email"));
}

public record Kontaktinfo(string Navn, string Forlag, string Adresse, string PostnrBy, string Telefon, string Email)
{
    /// <summary>Telefonnummer til tel:-links, uden mellemrum.</summary>
    public string TelefonLink => "+45" + new string(Telefon.Where(char.IsDigit).ToArray());
}
