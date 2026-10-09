using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Admin.Artikler;

public class RedigerModel(AppDbContext db, UploadMappe uploads) : PageModel
{
    [BindProperty] public Artikel Artikel { get; set; } = new();
    [BindProperty] public IFormFile? NytBillede { get; set; }
    public bool ErNy => Artikel.Id == 0;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return Page();
        var a = await db.Artikler.FindAsync(id);
        if (a is null) return NotFound();
        Artikel = a;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        ModelState.Remove("Artikel.Slug");
        if (!ModelState.IsValid) return Page();
        var a = id is null ? new Artikel() : await db.Artikler.FindAsync(id);
        if (a is null) return NotFound();

        if (NytBillede is { Length: > 0 })
        {
            var (filnavn, fejl) = await uploads.GemBilledeAsync(NytBillede, Artikel.Titel);
            if (fejl != null)
            {
                ModelState.AddModelError(nameof(NytBillede), fejl);
                Artikel.Billede = a.Billede;
                return Page();
            }
            a.Billede = filnavn;
        }
        a.Titel = Artikel.Titel.Trim();
        a.Indledning = Artikel.Indledning?.Trim();
        a.Indhold = Artikel.Indhold?.Trim() ?? "";
        a.Udgivet = Artikel.Udgivet;
        a.Synlig = Artikel.Synlig;
        if (id is null)
        {
            var grund = Tekst.Slug(a.Titel);
            a.Slug = grund;
            for (var i = 2; await db.Artikler.AnyAsync(x => x.Slug == a.Slug); i++) a.Slug = $"{grund}-{i}";
            db.Artikler.Add(a);
        }
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = $"«{a.Titel}» er gemt.";
        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostSletAsync(int id)
    {
        var a = await db.Artikler.FindAsync(id);
        if (a is null) return NotFound();
        db.Artikler.Remove(a);
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = $"«{a.Titel}» er slettet.";
        return RedirectToPage("Index");
    }
}
