using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Admin.Ordrer;

public class VisModel(AppDbContext db, Bestilling bestilling) : PageModel
{
    public Ordre Ordre { get; private set; } = null!;
    [BindProperty] public string? Note { get; set; }

    private async Task<Ordre?> HentAsync(int id) =>
        await db.Ordrer.Include(o => o.Linjer).FirstOrDefaultAsync(o => o.Id == id);

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var ordre = await HentAsync(id);
        if (ordre is null) return NotFound();
        Ordre = ordre;
        Note = ordre.Note;
        return Page();
    }

    public async Task<IActionResult> OnPostSendtAsync(int id)
    {
        var ordre = await HentAsync(id);
        if (ordre is null) return NotFound();
        if (ordre.Status == OrdreStatus.Ny)
        {
            ordre.Status = OrdreStatus.Sendt;
            ordre.Sendt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            TempData["AdminBesked"] = $"Ordre {id} er markeret som sendt.";
        }
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostAnnullerAsync(int id)
    {
        var ordre = await HentAsync(id);
        if (ordre is null) return NotFound();
        await bestilling.AnnullerAsync(ordre);
        TempData["AdminBesked"] = $"Ordre {id} er annulleret, og bøgerne er lagt tilbage på lageret.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostNoteAsync(int id)
    {
        var ordre = await HentAsync(id);
        if (ordre is null) return NotFound();
        ordre.Note = string.IsNullOrWhiteSpace(Note) ? null : Note.Trim();
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = "Noten er gemt.";
        return RedirectToPage(new { id });
    }
}
