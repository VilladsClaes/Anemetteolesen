using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Admin.Boeger;

public class RedigerModel(AppDbContext db, UploadMappe uploads) : PageModel
{
    [BindProperty] public Bog Bog { get; set; } = new();
    [BindProperty] public IFormFile? NytBillede { get; set; }
    public List<string> Emner { get; private set; } = [];
    public bool ErNy => Bog.Id == 0;

    private async Task HentEmnerAsync() =>
        Emner = (await db.Boeger.Where(b => b.Emne != null).Select(b => b.Emne!).Distinct().ToListAsync())
            .Order(Tekst.DanskSortering).ToList();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        await HentEmnerAsync();
        if (id is null) return Page();
        var bog = await db.Boeger.FindAsync(id);
        if (bog is null) return NotFound();
        Bog = bog;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        await HentEmnerAsync();
        ModelState.Remove("Bog.Slug");
        if (!ModelState.IsValid) return Page();

        var bog = id is null ? new Bog() : await db.Boeger.FindAsync(id);
        if (bog is null) return NotFound();

        if (NytBillede is { Length: > 0 })
        {
            var (filnavn, fejl) = await uploads.GemBilledeAsync(NytBillede, Bog.Titel);
            if (fejl != null)
            {
                ModelState.AddModelError(nameof(NytBillede), fejl);
                Bog.Id = bog.Id;
                Bog.Forside = bog.Forside;
                return Page();
            }
            bog.Forside = filnavn;
        }

        bog.Titel = Bog.Titel.Trim();
        bog.Undertitel = Bog.Undertitel?.Trim();
        bog.Beskrivelse = Bog.Beskrivelse?.Trim();
        bog.Pris = Bog.Pris;
        bog.Lager = Bog.Lager;
        bog.Status = Bog.Status;
        bog.Emne = string.IsNullOrWhiteSpace(Bog.Emne) ? null : Bog.Emne.Trim();
        bog.Udgivelsesaar = Bog.Udgivelsesaar;
        bog.Sider = Bog.Sider;
        bog.Isbn = Bog.Isbn?.Trim();
        bog.Fremhaevet = Bog.Fremhaevet;
        if (string.IsNullOrEmpty(bog.Slug) || id is null)
            bog.Slug = await UnikSlugAsync(bog.Titel, bog.Id);

        if (id is null) db.Boeger.Add(bog);
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = $"«{bog.Titel}» er gemt.";
        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostFjernBilledeAsync(int id)
    {
        var bog = await db.Boeger.FindAsync(id);
        if (bog is null) return NotFound();
        bog.Forside = null;
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = "Billedet er fjernet fra bogen.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostSletAsync(int id)
    {
        var bog = await db.Boeger.FindAsync(id);
        if (bog is null) return NotFound();
        db.Boeger.Remove(bog);
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = $"«{bog.Titel}» er slettet. Salgstallene for bogen er bevaret.";
        return RedirectToPage("Index");
    }

    private async Task<string> UnikSlugAsync(string titel, int id)
    {
        var grund = Tekst.Slug(titel);
        var slug = grund;
        for (var i = 2; await db.Boeger.AnyAsync(b => b.Slug == slug && b.Id != id); i++) slug = $"{grund}-{i}";
        return slug;
    }
}
