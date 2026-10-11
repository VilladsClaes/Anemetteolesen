using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Admin;

public class BeskederModel(AppDbContext db) : PageModel
{
    public List<Kontaktbesked> Beskeder { get; private set; } = [];

    public async Task OnGetAsync() =>
        Beskeder = await db.Kontaktbeskeder.AsNoTracking().OrderByDescending(b => b.Oprettet).Take(200).ToListAsync();

    public async Task<IActionResult> OnPostLaestAsync(int id)
    {
        await db.Kontaktbeskeder.Where(b => b.Id == id).ExecuteUpdateAsync(s => s.SetProperty(b => b.Laest, true));
        return RedirectToPage();
    }

    /// <summary>Sletter de markerede beskeder.</summary>
    public async Task<IActionResult> OnPostSletValgteAsync(int[] valgte)
    {
        var antal = valgte.Length == 0 ? 0 : await db.Kontaktbeskeder.Where(b => valgte.Contains(b.Id)).ExecuteDeleteAsync();
        TempData["AdminBesked"] = antal == 0 ? "Der var ingen beskeder markeret." : $"{antal} beskeder er slettet.";
        return RedirectToPage();
    }

    /// <summary>Sletter beskeder, hvis indhold ligner spam (samme tjek som på kontaktformularen).</summary>
    public async Task<IActionResult> OnPostRydSpamAsync()
    {
        var alle = await db.Kontaktbeskeder.ToListAsync();
        var spam = alle.Where(b => SpamVaern.LignerSpam(b.Navn, b.Emne, b.Besked, b.Email)).ToList();
        db.Kontaktbeskeder.RemoveRange(spam);
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = spam.Count == 0
            ? "Der blev ikke fundet beskeder, der ligner spam. Markér dem, du vil slette, og brug «Slet markerede»."
            : $"{spam.Count} beskeder, der lignede spam, er slettet.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSletAsync(int id)
    {
        await db.Kontaktbeskeder.Where(b => b.Id == id).ExecuteDeleteAsync();
        TempData["AdminBesked"] = "Beskeden er slettet.";
        return RedirectToPage();
    }
}
