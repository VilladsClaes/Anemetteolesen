using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Admin.Boeger;

public class IndexModel(AppDbContext db) : PageModel
{
    public List<Bog> Boeger { get; private set; } = [];
    public Dictionary<int, int> Solgt { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Boeger = (await db.Boeger.AsNoTracking().ToListAsync()).OrderBy(b => b.Titel, Tekst.DanskSortering).ToList();
        Solgt = await db.Ordrelinjer
            .Where(l => l.BogId != null && l.Ordre!.Status != OrdreStatus.Annulleret)
            .GroupBy(l => l.BogId!.Value)
            .Select(g => new { g.Key, Antal = g.Sum(l => l.Antal) })
            .ToDictionaryAsync(x => x.Key, x => x.Antal);
    }

    /// <summary>Hurtig rettelse af lagertal direkte i listen.</summary>
    public async Task<IActionResult> OnPostLagerAsync(Dictionary<int, int> lager)
    {
        var boeger = await db.Boeger.Where(b => lager.Keys.Contains(b.Id)).ToListAsync();
        foreach (var bog in boeger)
            bog.Lager = Math.Max(0, lager[bog.Id]);
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = "Lagertallene er gemt.";
        return RedirectToPage();
    }
}
