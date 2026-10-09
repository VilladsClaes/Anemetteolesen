using Anemette.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Nyt;

public class ArtikelModel(AppDbContext db) : PageModel
{
    public Artikel Artikel { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        var a = await db.Artikler.AsNoTracking().FirstOrDefaultAsync(x => x.Slug == slug && x.Synlig);
        if (a is null) return NotFound();
        Artikel = a;
        return Page();
    }
}
