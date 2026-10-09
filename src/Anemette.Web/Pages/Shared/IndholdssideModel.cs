using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Shared;

/// <summary>Fælles grundlag for sider, hvis tekst kan redigeres under Administration → Sidetekster.</summary>
public abstract class IndholdssideModel(AppDbContext db) : PageModel
{
    protected AppDbContext Db { get; } = db;
    public Side Side { get; private set; } = new();
    public List<Arrangement> Arrangementer { get; private set; } = [];

    protected async Task HentSideAsync(string noegle) =>
        Side = await Db.Sider.AsNoTracking().FirstOrDefaultAsync(s => s.Noegle == noegle) ?? new Side { Noegle = noegle };

    protected async Task HentArrangementerAsync(params ArrangementType[] typer)
    {
        var nu = Tekst.DanskTidNu;
        Arrangementer = await Db.Arrangementer.AsNoTracking()
            .Where(a => a.Synlig && a.Start >= nu && typer.Contains(a.Type))
            .OrderBy(a => a.Start).ToListAsync();
    }
}
