using System.ComponentModel.DataAnnotations;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Anemette.Web.Pages.Indkoeb;

public class BestilModel(Bestilling bestilling) : PageModel
{
    public List<KurvLinje> Linjer { get; private set; } = [];
    public decimal Total => Linjer.Sum(l => l.Beloeb);
    public List<string> Fejl { get; private set; } = [];

    [BindProperty] public Formular Input { get; set; } = new();

    public class Formular
    {
        [Required(ErrorMessage = "Skriv dit navn."), MaxLength(200), Display(Name = "Navn")]
        public string Navn { get; set; } = "";

        [Required(ErrorMessage = "Skriv din adresse."), MaxLength(300), Display(Name = "Adresse")]
        public string Adresse { get; set; } = "";

        [Required(ErrorMessage = "Skriv dit postnummer."), RegularExpression(@"^\s*\d{4}\s*$", ErrorMessage = "Postnummeret skal være fire cifre."), Display(Name = "Postnr.")]
        public string Postnummer { get; set; } = "";

        [Required(ErrorMessage = "Skriv din by."), MaxLength(100), Display(Name = "By")]
        public string By { get; set; } = "";

        [Required(ErrorMessage = "Skriv din e-mail, så du kan få en bekræftelse."), EmailAddress(ErrorMessage = "E-mailadressen ser ikke rigtig ud."), MaxLength(200), Display(Name = "E-mail")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Skriv dit telefonnummer."), RegularExpression(@"^[\d\s+()-]{8,20}$", ErrorMessage = "Telefonnummeret ser ikke rigtigt ud."), Display(Name = "Telefon")]
        public string Telefon { get; set; } = "";

        [MaxLength(2000), Display(Name = "Besked (valgfri)")]
        public string? Besked { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Sæt kryds for at bekræfte, at du har læst, hvordan dine oplysninger bruges.")]
        public bool Samtykke { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Linjer = await bestilling.KurvLinjerAsync();
        return Linjer.Count == 0 ? RedirectToPage("Index") : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Linjer = await bestilling.KurvLinjerAsync();
        if (Linjer.Count == 0) return RedirectToPage("Index");
        if (!ModelState.IsValid) return Page();

        var (ordre, fejl) = await bestilling.OpretAsync(new Kundeoplysninger(
            Input.Navn, Input.Adresse, Input.Postnummer, Input.By, Input.Email, Input.Telefon, Input.Besked));
        if (ordre is null)
        {
            Fejl = fejl;
            return Page();
        }
        TempData["OrdreNummer"] = ordre.Id;
        return RedirectToPage("Tak");
    }
}
