using System.Data.Common;
using Anemette.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Data;

/// <summary>Køres ved opstart: opdaterer databasen, lægger standardtekster ind og flytter data fra den gamle side.</summary>
public static class Startdata
{
    /// <summary>Bøger fra den gamle side uden kendt lagertal får dette antal. Ret det i Administration → Bøger.</summary>
    private const int UkendtLager = 5;

    private static readonly string[] FremhaevedeTitler = ["Spis dit ukrudt", "Mit Grønne Apotek", "Gamle Lægeurter", "Naturens kryddersnaps"];

    public static async Task KoerAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var log = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startdata");

        await db.Database.MigrateAsync();
        await StandardteksterAsync(db);
        await FlytGammeltDataAsync(db, log);
        await SletGamlePersondataAsync(db, log);
    }

    private static async Task StandardteksterAsync(AppDbContext db)
    {
        var sider = await db.Sider.Select(s => s.Noegle).ToListAsync();
        foreach (var (noegle, titel, indhold) in Standardtekster.Sider.Where(s => !sider.Contains(s.Noegle)))
            db.Sider.Add(new Side { Noegle = noegle, Titel = titel, Indhold = Ryk(indhold) });

        var indstillinger = await db.Indstillinger.Select(i => i.Noegle).ToListAsync();
        foreach (var (noegle, vaerdi) in Standardtekster.Indstillinger.Where(i => !indstillinger.Contains(i.Noegle)))
            db.Indstillinger.Add(new Indstilling { Noegle = noegle, Vaerdi = vaerdi });

        await db.SaveChangesAsync();
    }

    /// <summary>Fjerner den fælles indrykning fra tekster skrevet i C#-koden.</summary>
    private static string Ryk(string tekst) =>
        string.Join('\n', tekst.Replace("\r", "").Split('\n').Select(l => l.Trim())).Trim();

    /// <summary>Henter bøger og administratorer fra den gamle sides tabeller (tblProdukt, tblBillede, tblAdmin) én gang.</summary>
    private static async Task FlytGammeltDataAsync(AppDbContext db, ILogger log)
    {
        var forbindelse = db.Database.GetDbConnection();
        await db.Database.OpenConnectionAsync();
        try
        {
            if (!await db.Boeger.AnyAsync() && await TabelFindesAsync(forbindelse, "tblProdukt"))
            {
                var boeger = new List<Bog>();
                await using (var cmd = forbindelse.CreateCommand())
                {
                    cmd.CommandText = """
                        SELECT p.Navn, p.Beskrivelse, p.Pris, p.Antal,
                               (SELECT b.BilledeFil FROM tblBillede b WHERE b.FK_Produkt = p.ID ORDER BY b.ID LIMIT 1)
                        FROM tblProdukt p ORDER BY p.ID
                        """;
                    await using var r = await cmd.ExecuteReaderAsync();
                    while (await r.ReadAsync())
                    {
                        var titel = (r.IsDBNull(0) ? "" : r.GetString(0)).Trim();
                        if (titel.Length == 0) continue;
                        int? antal = r.IsDBNull(3) ? null : Convert.ToInt32(r.GetValue(3));
                        boeger.Add(new Bog
                        {
                            Titel = titel,
                            Beskrivelse = r.IsDBNull(1) ? null : r.GetString(1),
                            Pris = r.IsDBNull(2) ? 0 : Convert.ToDecimal(r.GetValue(2)),
                            Lager = antal ?? UkendtLager,
                            Status = antal == 0 ? BogStatus.Udsolgt : BogStatus.TilSalg,
                            Forside = r.IsDBNull(4) ? null : r.GetString(4),
                            Emne = Standardtekster.Emner.GetValueOrDefault(titel, "Andre bøger"),
                            Fremhaevet = FremhaevedeTitler.Contains(titel, StringComparer.OrdinalIgnoreCase),
                        });
                    }
                }
                var brugteSlugs = new HashSet<string>();
                foreach (var bog in boeger)
                {
                    var slug = Tekst.Slug(bog.Titel);
                    for (var i = 2; !brugteSlugs.Add(slug); i++) slug = $"{Tekst.Slug(bog.Titel)}-{i}";
                    bog.Slug = slug;
                }
                db.Boeger.AddRange(boeger);
                await db.SaveChangesAsync();
                log.LogInformation("Flyttede {Antal} bøger fra den gamle side", boeger.Count);
            }

            if (!await db.Administratorer.AnyAsync() && await TabelFindesAsync(forbindelse, "tblAdmin"))
            {
                await using var cmd = forbindelse.CreateCommand();
                cmd.CommandText = "SELECT Brugernavn, Password, Salt, BrugerEmail FROM tblAdmin WHERE Brugernavn IS NOT NULL AND Password IS NOT NULL";
                await using var r = await cmd.ExecuteReaderAsync();
                var admins = new List<Administrator>();
                while (await r.ReadAsync())
                {
                    admins.Add(new Administrator
                    {
                        Brugernavn = r.GetString(0).Trim(),
                        GammelHash = r.GetString(1),
                        GammelSalt = r.IsDBNull(2) ? "" : r.GetString(2),
                        Email = r.IsDBNull(3) ? null : r.GetString(3),
                    });
                }
                await r.CloseAsync();
                db.Administratorer.AddRange(admins.DistinctBy(a => a.Brugernavn, StringComparer.OrdinalIgnoreCase));
                await db.SaveChangesAsync();
                log.LogInformation("Flyttede {Antal} administratorer fra den gamle side", admins.Count);
            }
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
    }

    private static async Task<bool> TabelFindesAsync(DbConnection forbindelse, string tabel)
    {
        await using var cmd = forbindelse.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = @t";
        var p = cmd.CreateParameter();
        p.ParameterName = "@t";
        p.Value = tabel;
        cmd.Parameters.Add(p);
        return Convert.ToInt64(await cmd.ExecuteScalarAsync()) > 0;
    }

    /// <summary>Sletter persondata efter de frister, der står i privatlivsteksten.</summary>
    internal static async Task SletGamlePersondataAsync(AppDbContext db, ILogger log)
    {
        var nu = DateTime.UtcNow;
        // Bogføringsloven: 5 år efter regnskabsårets udløb
        var ordreGraense = new DateTime(nu.Year - 5, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var gamleOrdrer = await db.Ordrer.Where(o => o.Oprettet < ordreGraense && !o.Anonymiseret).ToListAsync();
        foreach (var o in gamleOrdrer)
        {
            o.Navn = o.Adresse = o.Postnummer = o.By = o.Email = o.Telefon = "";
            o.Besked = null;
            o.Anonymiseret = true;
        }
        await db.SaveChangesAsync();
        var ordrer = gamleOrdrer.Count;
        // MySQL tillader ikke at slette med en join mod samme forespørgsel, så arrangementerne findes først
        var gamleArrangementer = await db.Arrangementer.Where(a => a.Start < nu.AddYears(-1)).Select(a => a.Id).ToListAsync();
        var tilmeldinger = gamleArrangementer.Count == 0 ? 0
            : await db.Tilmeldinger.Where(t => gamleArrangementer.Contains(t.ArrangementId)).ExecuteDeleteAsync();
        var beskeder = await db.Kontaktbeskeder.Where(k => k.Oprettet < nu.AddYears(-2)).ExecuteDeleteAsync();
        if (ordrer + tilmeldinger + beskeder > 0)
            log.LogInformation("Slettede gamle persondata: {Ordrer} ordrer, {Tilmeldinger} tilmeldinger, {Beskeder} beskeder", ordrer, tilmeldinger, beskeder);
    }
}
