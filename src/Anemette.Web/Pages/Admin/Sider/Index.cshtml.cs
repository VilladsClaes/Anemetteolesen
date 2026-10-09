using Anemette.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Admin.Sider;

public class IndexModel(AppDbContext db) : PageModel
{
    /// <summary>Hvor teksterne vises – så det er tydeligt, hvad man retter.</summary>
    public static readonly Dictionary<string, (string Navn, string Url)> Placering = new()
    {
        ["forside"] = ("Forsiden – velkomst", "/"),
        ["om"] = ("Om Anemette", "/om"),
        ["forlaget"] = ("Om forlaget (nederst på «Om Anemette»)", "/om"),
        ["foredrag"] = ("Foredrag", "/foredrag"),
        ["naturvandringer"] = ("Naturvandringer og kurser", "/naturvandringer"),
        ["bestilling"] = ("Sådan bestiller du (øverst på «Bøger»)", "/boeger"),
        ["privatliv"] = ("Privatliv og cookies", "/privatliv"),
    };

    public List<Side> Sider { get; private set; } = [];
    [BindProperty] public Side? Valgt { get; set; }

    public async Task OnGetAsync(string? noegle)
    {
        Sider = await db.Sider.AsNoTracking().ToListAsync();
        Sider = Sider.OrderBy(s => Placering.Keys.ToList().IndexOf(s.Noegle)).ToList();
        if (noegle != null) Valgt = Sider.FirstOrDefault(s => s.Noegle == noegle);
    }

    public async Task<IActionResult> OnPostAsync(string noegle)
    {
        var side = await db.Sider.FindAsync(noegle);
        if (side is null || Valgt is null) return NotFound();
        side.Titel = Valgt.Titel.Trim();
        side.Indhold = Valgt.Indhold?.Trim() ?? "";
        side.Opdateret = DateTime.UtcNow;
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = $"Teksten «{side.Titel}» er gemt.";
        return RedirectToPage(new { noegle = (string?)null });
    }
}
