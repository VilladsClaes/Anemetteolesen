using System.ComponentModel.DataAnnotations;
using Anemette.Web.Data;
using Anemette.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages;

public class ArrangementModel(AppDbContext db, Mail mail, Indstillinger indstillinger) : PageModel
{
    public Arrangement Arrangement { get; private set; } = null!;
    public int LedigePladser { get; private set; }
    public bool ErOvre => Arrangement.Start < Tekst.DanskTidNu;
    public bool KanTilmelde => Arrangement.Pladser is > 0 && !ErOvre && LedigePladser > 0;

    [TempData] public string? Besked { get; set; }
    [BindProperty] public Formular Input { get; set; } = new();

    public class Formular
    {
        [Required(ErrorMessage = "Skriv dit navn."), MaxLength(200), Display(Name = "Navn")]
        public string Navn { get; set; } = "";

        [Required(ErrorMessage = "Skriv din e-mail."), EmailAddress(ErrorMessage = "E-mailadressen ser ikke rigtig ud."), MaxLength(200), Display(Name = "E-mail")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Skriv dit telefonnummer."), RegularExpression(@"^[\d\s+()-]{8,20}$", ErrorMessage = "Telefonnummeret ser ikke rigtigt ud."), Display(Name = "Telefon")]
        public string Telefon { get; set; } = "";

        [Range(1, 20, ErrorMessage = "Vælg mellem 1 og 20 personer."), Display(Name = "Antal personer")]
        public int Antal { get; set; } = 1;

        [MaxLength(1000), Display(Name = "Besked (valgfri)")]
        public string? Besked { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Sæt kryds for at bekræfte, at du har læst, hvordan dine oplysninger bruges.")]
        public bool Samtykke { get; set; }
    }

    private async Task<bool> HentAsync(int id)
    {
        var a = await db.Arrangementer.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.Synlig);
        if (a is null) return false;
        Arrangement = a;
        var tilmeldte = await db.Tilmeldinger.Where(t => t.ArrangementId == id).SumAsync(t => (int?)t.Antal) ?? 0;
        LedigePladser = Math.Max(0, (a.Pladser ?? 0) - tilmeldte);
        return true;
    }

    public async Task<IActionResult> OnGetAsync(int id) => await HentAsync(id) ? Page() : NotFound();

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!await HentAsync(id)) return NotFound();
        if (!KanTilmelde)
        {
            ModelState.AddModelError("", "Der er desværre ikke flere ledige pladser.");
            return Page();
        }
        if (Input.Antal > LedigePladser)
            ModelState.AddModelError("Input.Antal", $"Der er kun {LedigePladser} ledige pladser.");
        if (!ModelState.IsValid) return Page();

        db.Tilmeldinger.Add(new Tilmelding
        {
            ArrangementId = id,
            Navn = Input.Navn.Trim(),
            Email = Input.Email.Trim(),
            Telefon = Input.Telefon.Trim(),
            Antal = Input.Antal,
            Besked = string.IsNullOrWhiteSpace(Input.Besked) ? null : Input.Besked.Trim(),
        });
        await db.SaveChangesAsync();

        var tidspunkt = $"{Tekst.Dato(Arrangement.Start)} kl. {Arrangement.Start:HH.mm}";
        var modtager = await indstillinger.HentAsync("Ordrer sendes til");
        if (!string.IsNullOrWhiteSpace(modtager))
        {
            await mail.SendAsync(modtager, $"Ny tilmelding: {Arrangement.Titel}", $"""
                {Input.Navn} har tilmeldt {Input.Antal} {(Input.Antal == 1 ? "person" : "personer")} til «{Arrangement.Titel}» {tidspunkt}.

                Telefon: {Input.Telefon}
                E-mail: {Input.Email}
                {(string.IsNullOrWhiteSpace(Input.Besked) ? "" : "Besked: " + Input.Besked)}

                Se alle tilmeldinger under Administration → Arrangementer.
                """, svarTil: Input.Email, afsenderNavn: "Hjemmesiden");
        }
        var k = await indstillinger.KontaktAsync();
        await mail.SendAsync(Input.Email, $"Din tilmelding: {Arrangement.Titel}", $"""
            Kære {Input.Navn}

            Tak for din tilmelding til «{Arrangement.Titel}» {tidspunkt}{(string.IsNullOrEmpty(Arrangement.Sted) ? "" : ", " + Arrangement.Sted)}.
            Du har tilmeldt {Input.Antal} {(Input.Antal == 1 ? "person" : "personer")}.

            Kan du alligevel ikke komme, så giv mig besked på {k.Telefon} eller ved at svare på denne mail.

            Venlig hilsen
            {k.Navn}
            """, svarTil: k.Email, afsenderNavn: k.Forlag);

        Besked = $"Tak, {Input.Navn}. Du er nu tilmeldt.";
        return RedirectToPage(new { id });
    }
}
