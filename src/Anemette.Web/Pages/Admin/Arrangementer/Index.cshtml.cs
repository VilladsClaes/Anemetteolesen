using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Admin.Arrangementer;

public record ArrangementLinje(Arrangement Arrangement, int Tilmeldte);

public class IndexModel(AppDbContext db) : PageModel
{
    public List<ArrangementLinje> Kommende { get; private set; } = [];
    public List<ArrangementLinje> Afholdte { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var alle = await db.Arrangementer.AsNoTracking()
            .Select(a => new { Arrangement = a, Tilmeldte = a.Tilmeldinger.Sum(t => (int?)t.Antal) ?? 0 })
            .ToListAsync();
        var nu = Tekst.DanskTidNu;
        Kommende = alle.Where(a => a.Arrangement.Start >= nu).OrderBy(a => a.Arrangement.Start)
            .Select(a => new ArrangementLinje(a.Arrangement, a.Tilmeldte)).ToList();
        Afholdte = alle.Where(a => a.Arrangement.Start < nu).OrderByDescending(a => a.Arrangement.Start).Take(30)
            .Select(a => new ArrangementLinje(a.Arrangement, a.Tilmeldte)).ToList();
    }
}
