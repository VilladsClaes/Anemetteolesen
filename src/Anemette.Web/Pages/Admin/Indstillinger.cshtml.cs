using Anemette.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Admin;

public class IndstillingerModel(AppDbContext db) : PageModel
{
    public List<Indstilling> Liste { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var raekkefoelge = Standardtekster.Indstillinger.Select(i => i.Noegle).ToList();
        Liste = (await db.Indstillinger.AsNoTracking().ToListAsync()).OrderBy(i => raekkefoelge.IndexOf(i.Noegle)).ToList();
    }

    public async Task<IActionResult> OnPostAsync(Dictionary<string, string> vaerdi)
    {
        var alle = await db.Indstillinger.ToListAsync();
        foreach (var i in alle.Where(i => vaerdi.ContainsKey(i.Noegle)))
            i.Vaerdi = (vaerdi[i.Noegle] ?? "").Trim();
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = "Indstillingerne er gemt.";
        return RedirectToPage();
    }
}
