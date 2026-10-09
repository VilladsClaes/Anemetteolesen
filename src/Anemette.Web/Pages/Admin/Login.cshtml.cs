using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages.Admin;

[EnableRateLimiting("login")]
public class LoginModel(AppDbContext db, AdminLogin login, ILogger<LoginModel> log) : PageModel
{
    public bool IngenAdministratorer { get; private set; }
    public string? Fejl { get; private set; }

    [BindProperty, Required(ErrorMessage = "Skriv dit brugernavn."), Display(Name = "Brugernavn")]
    public string Brugernavn { get; set; } = "";

    [BindProperty, Required(ErrorMessage = "Skriv din adgangskode."), DataType(DataType.Password), Display(Name = "Adgangskode")]
    public string Adgangskode { get; set; } = "";

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated == true) return Redirect("/admin");
        IngenAdministratorer = !await db.Administratorer.AnyAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl)
    {
        IngenAdministratorer = !await db.Administratorer.AnyAsync();
        if (!ModelState.IsValid) return Page();

        Administrator? admin;
        if (IngenAdministratorer)
        {
            // Første gang: den første administrator oprettes direkte her
            if (Adgangskode.Length < AdminLogin.MindsteLaengde)
            {
                Fejl = $"Adgangskoden skal være mindst {AdminLogin.MindsteLaengde} tegn.";
                return Page();
            }
            admin = new Administrator { Brugernavn = Brugernavn.Trim() };
            AdminLogin.SaetAdgangskode(admin, Adgangskode);
            db.Administratorer.Add(admin);
            await db.SaveChangesAsync();
            log.LogInformation("Første administrator {Bruger} oprettet", admin.Brugernavn);
        }
        else
        {
            admin = await login.KontrollerAsync(Brugernavn, Adgangskode);
            if (admin is null)
            {
                log.LogWarning("Mislykket login for {Bruger}", Brugernavn);
                Fejl = "Forkert brugernavn eller adgangskode.";
                return Page();
            }
        }

        var identitet = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, admin.Brugernavn), new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString())],
            CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(new ClaimsPrincipal(identitet));
        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/admin");
    }
}
