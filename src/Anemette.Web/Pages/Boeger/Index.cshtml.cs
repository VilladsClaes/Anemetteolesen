using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Boeger;

public class IndexModel(AppDbContext db) : PageModel
{
    [BindProperty(SupportsGet = true)] public string? Emne { get; set; }
    [BindProperty(SupportsGet = true)] public string? Soeg { get; set; }

    public List<string> Emner { get; private set; } = [];
    public List<Bog> Boeger { get; private set; } = [];
    public Side? Bestillingstekst { get; private set; }

    public async Task OnGetAsync()
    {
        var synlige = db.Boeger.AsNoTracking().Where(b => b.Status != BogStatus.Skjult);
        Emner = (await synlige.Where(b => b.Emne != null).Select(b => b.Emne!).Distinct().ToListAsync())
            .Order(Tekst.DanskSortering).ToList();

        var forespoergsel = synlige;
        if (!string.IsNullOrWhiteSpace(Emne))
            forespoergsel = forespoergsel.Where(b => b.Emne == Emne);
        if (!string.IsNullOrWhiteSpace(Soeg))
        {
            var s = Soeg.Trim();
            forespoergsel = forespoergsel.Where(b => b.Titel.Contains(s) || (b.Beskrivelse != null && b.Beskrivelse.Contains(s)));
        }
        Boeger = (await forespoergsel.ToListAsync())
            // Bøger der kan bestilles først, derefter alfabetisk på dansk (Æ, Ø, Å til sidst)
            .OrderBy(b => b.KanBestilles ? 0 : 1)
            .ThenBy(b => b.Titel, Tekst.DanskSortering)
            .ToList();
        Bestillingstekst = await db.Sider.FindAsync("bestilling");
    }
}
