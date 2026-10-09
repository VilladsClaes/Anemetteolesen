using Anemette.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Boeger;

public class BogModel(AppDbContext db) : PageModel
{
    public Bog Bog { get; private set; } = null!;
    public List<Bog> SammeEmne { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        var bog = await db.Boeger.AsNoTracking().FirstOrDefaultAsync(b => b.Slug == slug && b.Status != BogStatus.Skjult);
        if (bog is null) return NotFound();
        Bog = bog;
        if (bog.Emne != null)
        {
            SammeEmne = await db.Boeger.AsNoTracking()
                .Where(b => b.Emne == bog.Emne && b.Id != bog.Id && b.Status == BogStatus.TilSalg && b.Lager > 0)
                .OrderBy(b => b.Id).Take(4).ToListAsync();
        }
        return Page();
    }
}
