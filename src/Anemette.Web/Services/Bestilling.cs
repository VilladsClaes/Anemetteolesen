using System.Text;
using Anemette.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Services;

public record KurvLinje(Bog Bog, int Antal)
{
    public decimal Beloeb => Bog.Pris * Antal;
}

public record Kundeoplysninger(string Navn, string Adresse, string Postnummer, string By, string Email, string Telefon, string? Besked);

/// <summary>Opretter bestillinger, trækker bøgerne fra lageret og giver besked til Anemette og kunden.</summary>
public class Bestilling(AppDbContext db, Kurv kurv, Mail mail, Indstillinger indstillinger, ILogger<Bestilling> log)
{
    public async Task<List<KurvLinje>> KurvLinjerAsync()
    {
        var indhold = kurv.Indhold();
        if (indhold.Count == 0) return [];
        var boeger = await db.Boeger.AsNoTracking().Where(b => indhold.Keys.Contains(b.Id)).ToListAsync();
        return boeger
            .OrderBy(b => b.Titel, Tekst.DanskSortering)
            .Select(b => new KurvLinje(b, indhold[b.Id]))
            .ToList();
    }

    /// <summary>Opretter ordren. Returnerer fejlbeskeder, hvis en bog ikke længere kan leveres i det ønskede antal.</summary>
    public async Task<(Ordre? Ordre, List<string> Fejl)> OpretAsync(Kundeoplysninger kunde)
    {
        var fejl = new List<string>();
        var indhold = kurv.Indhold();
        if (indhold.Count == 0)
        {
            fejl.Add("Kurven er tom.");
            return (null, fejl);
        }

        await using var transaktion = await db.Database.BeginTransactionAsync();
        var boeger = await db.Boeger.Where(b => indhold.Keys.Contains(b.Id)).ToListAsync();
        var ordre = new Ordre
        {
            Navn = kunde.Navn.Trim(),
            Adresse = kunde.Adresse.Trim(),
            Postnummer = kunde.Postnummer.Trim(),
            By = kunde.By.Trim(),
            Email = kunde.Email.Trim(),
            Telefon = kunde.Telefon.Trim(),
            Besked = string.IsNullOrWhiteSpace(kunde.Besked) ? null : kunde.Besked.Trim(),
        };
        foreach (var bog in boeger.OrderBy(b => b.Titel, Tekst.DanskSortering))
        {
            var antal = indhold[bog.Id];
            if (!bog.KanBestilles)
            {
                fejl.Add($"«{bog.Titel}» kan desværre ikke bestilles i øjeblikket.");
                continue;
            }
            if (bog.Lager < antal)
            {
                fejl.Add($"Der er kun {bog.Lager} stk. af «{bog.Titel}» på lager.");
                continue;
            }
            bog.Lager -= antal;
            ordre.Linjer.Add(new Ordrelinje { BogId = bog.Id, Titel = bog.Titel, Antal = antal, Stykpris = bog.Pris });
        }
        if (fejl.Count > 0 || ordre.Linjer.Count == 0)
        {
            if (fejl.Count == 0) fejl.Add("Ingen af bøgerne i kurven kan bestilles.");
            return (null, fejl);
        }

        db.Ordrer.Add(ordre);
        await db.SaveChangesAsync();
        await transaktion.CommitAsync();
        kurv.Toem();
        log.LogInformation("Ny ordre {Ordre} med {Antal} bøger", ordre.Id, ordre.Linjer.Sum(l => l.Antal));

        await SendMailsAsync(ordre);
        return (ordre, fejl);
    }

    /// <summary>Annullerer en ordre og lægger bøgerne tilbage på lageret.</summary>
    public async Task AnnullerAsync(Ordre ordre)
    {
        if (ordre.Status == OrdreStatus.Annulleret) return;
        foreach (var linje in ordre.Linjer.Where(l => l.BogId != null))
        {
            var bog = await db.Boeger.FindAsync(linje.BogId);
            if (bog != null) bog.Lager += linje.Antal;
        }
        ordre.Status = OrdreStatus.Annulleret;
        await db.SaveChangesAsync();
    }

    private async Task SendMailsAsync(Ordre ordre)
    {
        var linjer = new StringBuilder();
        foreach (var l in ordre.Linjer)
            linjer.AppendLine($"  {l.Antal} × {l.Titel} à {Tekst.Kr(l.Stykpris)} = {Tekst.Kr(l.Antal * l.Stykpris)}");
        linjer.AppendLine($"  I alt: {Tekst.Kr(ordre.Total)} + porto");

        var kunde = $"""
            {ordre.Navn}
            {ordre.Adresse}
            {ordre.Postnummer} {ordre.By}
            Telefon: {ordre.Telefon}
            E-mail: {ordre.Email}
            """;

        var tilAnemette = $"""
            Der er kommet en ny bestilling (nr. {ordre.Id}) fra hjemmesiden.

            {linjer}
            Sendes til:
            {kunde}
            {(ordre.Besked is null ? "" : $"\nBesked fra kunden:\n{ordre.Besked}\n")}
            Se og markér ordren som sendt under Administration → Ordrer.
            """;
        var modtager = await indstillinger.HentAsync("Ordrer sendes til");
        if (!string.IsNullOrWhiteSpace(modtager))
            await mail.SendAsync(modtager, $"Ny bestilling nr. {ordre.Id} fra {ordre.Navn}", tilAnemette, svarTil: ordre.Email, afsenderNavn: "Hjemmesiden");

        var k = await indstillinger.KontaktAsync();
        var tilKunden = $"""
            Kære {ordre.Navn}

            Tak for din bestilling. Jeg har modtaget den og sender bøgerne hurtigst muligt sammen med en faktura. Porto lægges til.

            Du har bestilt:
            {linjer}
            Bøgerne sendes til:
            {kunde}

            Har du spørgsmål, er du velkommen til at svare på denne mail eller ringe på {k.Telefon}.

            Venlig hilsen
            {k.Navn}
            {k.Forlag}
            {k.Adresse}, {k.PostnrBy}
            """;
        await mail.SendAsync(ordre.Email, $"Tak for din bestilling hos {k.Forlag}", tilKunden, svarTil: k.Email, afsenderNavn: k.Forlag);
    }
}
