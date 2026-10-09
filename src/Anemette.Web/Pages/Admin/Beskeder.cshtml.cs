using Anemette.Web.Data;
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

    public async Task<IActionResult> OnPostSletAsync(int id)
    {
        await db.Kontaktbeskeder.Where(b => b.Id == id).ExecuteDeleteAsync();
        TempData["AdminBesked"] = "Beskeden er slettet.";
        return RedirectToPage();
    }
}
