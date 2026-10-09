using Anemette.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Admin.Arrangementer;

public class RedigerModel(AppDbContext db) : PageModel
{
    [BindProperty] public Arrangement Arrangement { get; set; } = new();
    public List<Tilmelding> Tilmeldinger { get; private set; } = [];
    public bool ErNy => Arrangement.Id == 0;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return Page();
        var a = await db.Arrangementer.FindAsync(id);
        if (a is null) return NotFound();
        Arrangement = a;
        Tilmeldinger = await db.Tilmeldinger.AsNoTracking().Where(t => t.ArrangementId == id).OrderBy(t => t.Oprettet).ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (!ModelState.IsValid)
        {
            if (id != null)
                Tilmeldinger = await db.Tilmeldinger.AsNoTracking().Where(t => t.ArrangementId == id).ToListAsync();
            return Page();
        }
        var a = id is null ? new Arrangement() : await db.Arrangementer.FindAsync(id);
        if (a is null) return NotFound();
        a.Type = Arrangement.Type;
        a.Titel = Arrangement.Titel.Trim();
        a.Beskrivelse = Arrangement.Beskrivelse?.Trim();
        a.Start = Arrangement.Start;
        a.Sted = Arrangement.Sted?.Trim();
        a.Pris = Arrangement.Pris;
        a.Pladser = Arrangement.Pladser;
        a.Synlig = Arrangement.Synlig;
        if (id is null) db.Arrangementer.Add(a);
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = $"«{a.Titel}» er gemt.";
        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostSletAsync(int id)
    {
        var a = await db.Arrangementer.FindAsync(id);
        if (a is null) return NotFound();
        db.Arrangementer.Remove(a);
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = $"«{a.Titel}» og tilmeldingerne til det er slettet.";
        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostSletTilmeldingAsync(int id, int tilmeldingId)
    {
        var t = await db.Tilmeldinger.FirstOrDefaultAsync(x => x.Id == tilmeldingId && x.ArrangementId == id);
        if (t != null)
        {
            db.Tilmeldinger.Remove(t);
            await db.SaveChangesAsync();
            TempData["AdminBesked"] = "Tilmeldingen er slettet.";
        }
        return RedirectToPage(new { id });
    }
}
