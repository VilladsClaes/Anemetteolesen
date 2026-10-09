using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages;

public class IndexModel(AppDbContext db) : PageModel
{
    public Side? Velkomst { get; private set; }
    public List<Bog> Fremhaevede { get; private set; } = [];
    public List<Bog> KommerSnart { get; private set; } = [];
    public List<Arrangement> Kommende { get; private set; } = [];
    public List<Artikel> Nyheder { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Velkomst = await db.Sider.FindAsync("forside");
        Fremhaevede = await db.Boeger.AsNoTracking()
            .Where(b => b.Fremhaevet && b.Status != BogStatus.Skjult && b.Status != BogStatus.KommerSnart)
            .OrderBy(b => b.Titel).Take(4).ToListAsync();
        KommerSnart = await db.Boeger.AsNoTracking()
            .Where(b => b.Status == BogStatus.KommerSnart).OrderBy(b => b.Titel).ToListAsync();
        var nu = Tekst.DanskTidNu;
        Kommende = await db.Arrangementer.AsNoTracking()
            .Where(a => a.Synlig && a.Start >= nu).OrderBy(a => a.Start).Take(3).ToListAsync();
        Nyheder = await db.Artikler.AsNoTracking()
            .Where(a => a.Synlig).OrderByDescending(a => a.Udgivet).Take(3).ToListAsync();
    }
}
