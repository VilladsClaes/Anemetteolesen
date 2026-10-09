using System.ComponentModel.DataAnnotations;
using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Anemette.Web.Pages;

public class KontaktModel(AppDbContext db, Mail mail, Indstillinger indstillinger) : PageModel
{
    public Kontaktinfo Kontakt { get; private set; } = null!;

    [TempData] public string? Besked { get; set; }
    [BindProperty] public Formular Input { get; set; } = new();

    /// <summary>Usynligt felt, som kun robotter udfylder.</summary>
    [BindProperty] public string? Hjemmeside { get; set; }

    public class Formular
    {
        [Required(ErrorMessage = "Skriv dit navn."), MaxLength(200), Display(Name = "Navn")]
        public string Navn { get; set; } = "";

        [Required(ErrorMessage = "Skriv din e-mail, så jeg kan svare dig."), EmailAddress(ErrorMessage = "E-mailadressen ser ikke rigtig ud."), MaxLength(200), Display(Name = "E-mail")]
        public string Email { get; set; } = "";

        [MaxLength(30), Display(Name = "Telefon (valgfri)")]
        public string? Telefon { get; set; }

        [Required(ErrorMessage = "Skriv hvad det drejer sig om."), MaxLength(200), Display(Name = "Emne")]
        public string Emne { get; set; } = "";

        [Required(ErrorMessage = "Skriv din besked."), MaxLength(5000), Display(Name = "Besked")]
        public string Besked { get; set; } = "";

        [Range(typeof(bool), "true", "true", ErrorMessage = "Sæt kryds for at bekræfte, at du har læst, hvordan dine oplysninger bruges.")]
        public bool Samtykke { get; set; }
    }

    public async Task OnGetAsync(string? emne)
    {
        Kontakt = await indstillinger.KontaktAsync();
        if (!string.IsNullOrWhiteSpace(emne)) Input.Emne = emne;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Kontakt = await indstillinger.KontaktAsync();
        if (!string.IsNullOrEmpty(Hjemmeside)) return RedirectToPage(); // spam
        if (!ModelState.IsValid) return Page();

        db.Kontaktbeskeder.Add(new Kontaktbesked
        {
            Navn = Input.Navn.Trim(),
            Email = Input.Email.Trim(),
            Telefon = string.IsNullOrWhiteSpace(Input.Telefon) ? null : Input.Telefon.Trim(),
            Emne = Input.Emne.Trim(),
            Besked = Input.Besked.Trim(),
        });
        await db.SaveChangesAsync();

        await mail.SendAsync(Kontakt.Email, $"Besked fra hjemmesiden: {Input.Emne}", $"""
            {Input.Navn} har skrevet via kontaktformularen.

            Emne: {Input.Emne}
            E-mail: {Input.Email}
            {(string.IsNullOrWhiteSpace(Input.Telefon) ? "" : "Telefon: " + Input.Telefon)}

            {Input.Besked}

            (Svar direkte på denne mail for at svare {Input.Navn}.)
            """, svarTil: Input.Email, afsenderNavn: "Hjemmesiden");

        Besked = "Tak for din besked. Jeg svarer så hurtigt, jeg kan.";
        return RedirectToPage();
    }
}
