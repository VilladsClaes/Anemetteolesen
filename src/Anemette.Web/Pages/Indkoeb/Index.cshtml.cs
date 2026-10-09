using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Anemette.Web.Pages.Indkoeb;

public class IndexModel(Kurv kurv, Bestilling bestilling, AppDbContext db) : PageModel
{
    public List<KurvLinje> Linjer { get; private set; } = [];
    public decimal Total => Linjer.Sum(l => l.Beloeb);

    [TempData] public string? Besked { get; set; }

    public async Task OnGetAsync() => Linjer = await bestilling.KurvLinjerAsync();

    public async Task<IActionResult> OnPostTilfoejAsync(int bogId, int antal = 1)
    {
        var bog = await db.Boeger.FindAsync(bogId);
        if (bog is null || !bog.KanBestilles) return RedirectToPage();
        kurv.Tilfoej(bogId, Math.Clamp(antal, 1, Kurv.MaksAntal));
        Besked = $"«{bog.Titel}» er lagt i kurven.";
        return RedirectToPage();
    }

    public IActionResult OnPostOpdater(Dictionary<int, int> antal)
    {
        foreach (var (bogId, nytAntal) in antal)
            kurv.Saet(bogId, nytAntal);
        Besked = "Kurven er opdateret.";
        return RedirectToPage();
    }

    public IActionResult OnPostFjern(int bogId)
    {
        kurv.Saet(bogId, 0);
        Besked = "Bogen er fjernet fra kurven.";
        return RedirectToPage();
    }
}
