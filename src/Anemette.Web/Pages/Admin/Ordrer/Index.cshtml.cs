using Anemette.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Admin.Ordrer;

public class IndexModel(AppDbContext db) : PageModel
{
    [BindProperty(SupportsGet = true)] public OrdreStatus? Status { get; set; }
    public List<Ordre> Ordrer { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var q = db.Ordrer.AsNoTracking().Include(o => o.Linjer).AsQueryable();
        if (Status != null) q = q.Where(o => o.Status == Status);
        Ordrer = await q.OrderByDescending(o => o.Oprettet).Take(200).ToListAsync();
    }
}
