using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Admin;

public record BogSalg(string Titel, int Antal, decimal Beloeb);

public class IndexModel(AppDbContext db, Mail mail) : PageModel
{
    public bool MailErSatOp => mail.ErSatOp;
    public int NyeOrdrer { get; private set; }
    public int UlaesteBeskeder { get; private set; }
    public int SolgtIAar { get; private set; }
    public decimal OmsaetningIAar { get; private set; }
    public int SolgtIAlt { get; private set; }
    public int BoegerPaaLager { get; private set; }
    public List<BogSalg> MestSolgte { get; private set; } = [];
    public List<(string Maaned, int Antal, decimal Beloeb)> Maaneder { get; private set; } = [];
    public List<Bog> LavtLager { get; private set; } = [];
    public List<(Arrangement Arrangement, int Tilmeldte)> Kommende { get; private set; } = [];

    public async Task OnGetAsync()
    {
        NyeOrdrer = await db.Ordrer.CountAsync(o => o.Status == OrdreStatus.Ny);
        UlaesteBeskeder = await db.Kontaktbeskeder.CountAsync(k => !k.Laest);

        // Salgstal: alle ordrelinjer på ordrer, der ikke er annulleret
        var salg = await db.Ordrelinjer.AsNoTracking()
            .Where(l => l.Ordre!.Status != OrdreStatus.Annulleret)
            .Select(l => new { l.Titel, l.Antal, l.Stykpris, l.Ordre!.Oprettet })
            .ToListAsync();
        var aarStart = new DateTime(Tekst.DanskTidNu.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        SolgtIAar = salg.Where(s => s.Oprettet >= aarStart).Sum(s => s.Antal);
        OmsaetningIAar = salg.Where(s => s.Oprettet >= aarStart).Sum(s => s.Antal * s.Stykpris);
        SolgtIAlt = salg.Sum(s => s.Antal);
        MestSolgte = salg.GroupBy(s => s.Titel)
            .Select(g => new BogSalg(g.Key, g.Sum(s => s.Antal), g.Sum(s => s.Antal * s.Stykpris)))
            .OrderByDescending(b => b.Antal).Take(10).ToList();
        var nu = Tekst.DanskTidNu;
        Maaneder = Enumerable.Range(0, 12).Select(i => new DateTime(nu.Year, nu.Month, 1).AddMonths(-i))
            .Select(m => (
                m.ToString("MMMM yyyy", Tekst.Dansk),
                salg.Where(s => Tekst.DanskTid(s.Oprettet).Year == m.Year && Tekst.DanskTid(s.Oprettet).Month == m.Month).Sum(s => s.Antal),
                salg.Where(s => Tekst.DanskTid(s.Oprettet).Year == m.Year && Tekst.DanskTid(s.Oprettet).Month == m.Month).Sum(s => s.Antal * s.Stykpris)))
            .ToList();

        BoegerPaaLager = await db.Boeger.Where(b => b.Status == BogStatus.TilSalg).SumAsync(b => b.Lager);
        LavtLager = (await db.Boeger.AsNoTracking().Where(b => b.Status == BogStatus.TilSalg && b.Lager <= 2).ToListAsync())
            .OrderBy(b => b.Lager).ThenBy(b => b.Titel, Tekst.DanskSortering).ToList();

        var kommende = await db.Arrangementer.AsNoTracking().Where(a => a.Start >= nu).OrderBy(a => a.Start).Take(5)
            .Select(a => new { Arrangement = a, Tilmeldte = a.Tilmeldinger.Sum(t => (int?)t.Antal) ?? 0 })
            .ToListAsync();
        Kommende = kommende.Select(k => (k.Arrangement, k.Tilmeldte)).ToList();
    }
}
