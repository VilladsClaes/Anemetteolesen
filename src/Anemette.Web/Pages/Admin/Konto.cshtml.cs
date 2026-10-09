using System.Security.Claims;
using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Admin;

public class KontoModel(AppDbContext db, AdminLogin login) : PageModel
{
    public List<Administrator> Administratorer { get; private set; } = [];
    public int MitId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    public string? Fejl { get; private set; }

    public async Task OnGetAsync() => await HentAsync();

    private async Task HentAsync() =>
        Administratorer = await db.Administratorer.AsNoTracking().OrderBy(a => a.Brugernavn).ToListAsync();

    public async Task<IActionResult> OnPostSkiftAdgangskodeAsync(string nuvaerende, string ny, string gentag)
    {
        await HentAsync();
        var mig = await db.Administratorer.FindAsync(MitId);
        if (mig is null) return RedirectToPage("/Admin/Login");
        if (await login.KontrollerAsync(mig.Brugernavn, nuvaerende ?? "") is null)
            Fejl = "Den nuværende adgangskode er forkert.";
        else if ((ny ?? "").Length < AdminLogin.MindsteLaengde)
            Fejl = $"Den nye adgangskode skal være mindst {AdminLogin.MindsteLaengde} tegn.";
        else if (ny != gentag)
            Fejl = "De to nye adgangskoder er ikke ens.";
        if (Fejl != null) return Page();

        mig = await db.Administratorer.FindAsync(MitId);
        AdminLogin.SaetAdgangskode(mig!, ny!);
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = "Din adgangskode er skiftet.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostOpretAsync(string brugernavn, string? email, string adgangskode)
    {
        await HentAsync();
        brugernavn = (brugernavn ?? "").Trim();
        if (brugernavn.Length < 2)
            Fejl = "Skriv et brugernavn.";
        else if ((adgangskode ?? "").Length < AdminLogin.MindsteLaengde)
            Fejl = $"Adgangskoden skal være mindst {AdminLogin.MindsteLaengde} tegn.";
        else if (await db.Administratorer.AnyAsync(a => a.Brugernavn == brugernavn))
            Fejl = "Brugernavnet findes allerede.";
        if (Fejl != null) return Page();

        var admin = new Administrator { Brugernavn = brugernavn, Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim() };
        AdminLogin.SaetAdgangskode(admin, adgangskode!);
        db.Administratorer.Add(admin);
        await db.SaveChangesAsync();
        TempData["AdminBesked"] = $"Administratoren «{brugernavn}» er oprettet.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSletAsync(int id)
    {
        if (id != MitId)
        {
            await db.Administratorer.Where(a => a.Id == id).ExecuteDeleteAsync();
            TempData["AdminBesked"] = "Administratoren er slettet.";
        }
        return RedirectToPage();
    }
}
