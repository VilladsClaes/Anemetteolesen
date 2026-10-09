using Anemette.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Nyt;

public class IndexModel(AppDbContext db) : PageModel
{
    public List<Artikel> Artikler { get; private set; } = [];

    public async Task OnGetAsync() =>
        Artikler = await db.Artikler.AsNoTracking().Where(a => a.Synlig).OrderByDescending(a => a.Udgivet).ToListAsync();
}
