using System.ComponentModel.DataAnnotations;

namespace Anemette.Web.Data;

public enum BogStatus
{
    [Display(Name = "Til salg")] TilSalg = 0,
    [Display(Name = "Udsolgt")] Udsolgt = 1,
    [Display(Name = "Kommer snart")] KommerSnart = 2,
    [Display(Name = "Skjult")] Skjult = 3,
}

public class Bog
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Bogen skal have en titel."), MaxLength(200), Display(Name = "Titel")]
    public string Titel { get; set; } = "";

    [MaxLength(220)]
    public string Slug { get; set; } = "";

    [MaxLength(200), Display(Name = "Undertitel")]
    public string? Undertitel { get; set; }

    [MaxLength(4000), Display(Name = "Beskrivelse")]
    public string? Beskrivelse { get; set; }

    [Range(0, 100000, ErrorMessage = "Prisen skal være et positivt beløb."), Display(Name = "Pris i kr.")]
    public decimal Pris { get; set; }

    [Range(0, 100000, ErrorMessage = "Lageret kan ikke være negativt."), Display(Name = "Antal på lager")]
    public int Lager { get; set; }

    [Display(Name = "Status")]
    public BogStatus Status { get; set; } = BogStatus.TilSalg;

    [MaxLength(100), Display(Name = "Emne")]
    public string? Emne { get; set; }

    [Display(Name = "Udgivelsesår")]
    public int? Udgivelsesaar { get; set; }

    [Display(Name = "Antal sider")]
    public int? Sider { get; set; }

    [MaxLength(20), Display(Name = "ISBN")]
    public string? Isbn { get; set; }

    [MaxLength(300), Display(Name = "Forsidebillede")]
    public string? Forside { get; set; }

    [Display(Name = "Vis på forsiden")]
    public bool Fremhaevet { get; set; }

    public DateTime Oprettet { get; set; } = DateTime.UtcNow;

    public bool KanBestilles => Status == BogStatus.TilSalg && Lager > 0;
}

public enum OrdreStatus
{
    [Display(Name = "Ny")] Ny = 0,
    [Display(Name = "Sendt")] Sendt = 1,
    [Display(Name = "Annulleret")] Annulleret = 2,
}

/// <summary>En bestilling fra bestillingsformularen. Kundens oplysninger gemmes krypteret.</summary>
public class Ordre
{
    public int Id { get; set; }
    public DateTime Oprettet { get; set; } = DateTime.UtcNow;
    public OrdreStatus Status { get; set; } = OrdreStatus.Ny;
    public DateTime? Sendt { get; set; }

    /// <summary>Kundens oplysninger er fjernet (efter bogføringslovens 5 år). Ordrelinjerne bevares til salgstal.</summary>
    public bool Anonymiseret { get; set; }

    [Krypteret] public string Navn { get; set; } = "";
    [Krypteret] public string Adresse { get; set; } = "";
    [Krypteret] public string Postnummer { get; set; } = "";
    [Krypteret] public string By { get; set; } = "";
    [Krypteret] public string Email { get; set; } = "";
    [Krypteret] public string Telefon { get; set; } = "";
    [Krypteret] public string? Besked { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    public List<Ordrelinje> Linjer { get; set; } = new();

    public decimal Total => Linjer.Sum(l => l.Stykpris * l.Antal);
}

public class Ordrelinje
{
    public int Id { get; set; }
    public int OrdreId { get; set; }
    public Ordre? Ordre { get; set; }
    public int? BogId { get; set; }
    public Bog? Bog { get; set; }

    [MaxLength(200)]
    public string Titel { get; set; } = "";
    public int Antal { get; set; }
    public decimal Stykpris { get; set; }
}

public enum ArrangementType
{
    [Display(Name = "Foredrag")] Foredrag = 0,
    [Display(Name = "Naturvandring")] Naturvandring = 1,
    [Display(Name = "Kursus")] Kursus = 2,
}

public class Arrangement
{
    public int Id { get; set; }

    [Display(Name = "Type")]
    public ArrangementType Type { get; set; }

    [Required(ErrorMessage = "Skriv en titel."), MaxLength(200), Display(Name = "Titel")]
    public string Titel { get; set; } = "";

    [MaxLength(4000), Display(Name = "Beskrivelse")]
    public string? Beskrivelse { get; set; }

    [Display(Name = "Dato og tidspunkt")]
    public DateTime Start { get; set; } = DateTime.Today.AddDays(14).AddHours(19);

    [MaxLength(300), Display(Name = "Sted")]
    public string? Sted { get; set; }

    [Display(Name = "Pris i kr. (tom = gratis)")]
    public decimal? Pris { get; set; }

    [Display(Name = "Antal pladser (tom = ingen tilmelding)")]
    public int? Pladser { get; set; }

    [Display(Name = "Vis på hjemmesiden")]
    public bool Synlig { get; set; } = true;

    public List<Tilmelding> Tilmeldinger { get; set; } = new();
}

public class Tilmelding
{
    public int Id { get; set; }
    public int ArrangementId { get; set; }
    public Arrangement? Arrangement { get; set; }
    public DateTime Oprettet { get; set; } = DateTime.UtcNow;
    public int Antal { get; set; } = 1;

    [Krypteret] public string Navn { get; set; } = "";
    [Krypteret] public string Email { get; set; } = "";
    [Krypteret] public string Telefon { get; set; } = "";
    [Krypteret] public string? Besked { get; set; }
}

public class Artikel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Skriv en overskrift."), MaxLength(200), Display(Name = "Overskrift")]
    public string Titel { get; set; } = "";

    [MaxLength(220)]
    public string Slug { get; set; } = "";

    [MaxLength(600), Display(Name = "Kort indledning")]
    public string? Indledning { get; set; }

    [Display(Name = "Tekst")]
    public string Indhold { get; set; } = "";

    [MaxLength(300), Display(Name = "Billede")]
    public string? Billede { get; set; }

    [Display(Name = "Dato")]
    public DateTime Udgivet { get; set; } = DateTime.Today;

    [Display(Name = "Vis på hjemmesiden")]
    public bool Synlig { get; set; } = true;
}

/// <summary>Redigerbare tekster på faste sider (forside, om Anemette, foredrag, ...).</summary>
public class Side
{
    [Key, MaxLength(60)]
    public string Noegle { get; set; } = "";

    [MaxLength(200), Display(Name = "Overskrift")]
    public string Titel { get; set; } = "";

    [Display(Name = "Tekst")]
    public string Indhold { get; set; } = "";

    public DateTime Opdateret { get; set; } = DateTime.UtcNow;
}

public class Kontaktbesked
{
    public int Id { get; set; }
    public DateTime Oprettet { get; set; } = DateTime.UtcNow;
    public bool Laest { get; set; }

    [Krypteret] public string Navn { get; set; } = "";
    [Krypteret] public string Email { get; set; } = "";
    [Krypteret] public string? Telefon { get; set; }
    [Krypteret] public string Emne { get; set; } = "";
    [Krypteret] public string Besked { get; set; } = "";
}

public class Indstilling
{
    [Key, MaxLength(60)]
    public string Noegle { get; set; } = "";

    [MaxLength(1000)]
    public string Vaerdi { get; set; } = "";
}

public class Administrator
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Brugernavn { get; set; } = "";

    [MaxLength(200)]
    public string? Email { get; set; }

    /// <summary>ASP.NET Core PasswordHasher (PBKDF2). Tom hvis brugeren stadig har en gammel hash.</summary>
    [MaxLength(500)]
    public string? AdgangskodeHash { get; set; }

    /// <summary>Hash og salt fra den gamle side (SHA-256). Opgraderes ved første login.</summary>
    [MaxLength(200)]
    public string? GammelHash { get; set; }

    [MaxLength(200)]
    public string? GammelSalt { get; set; }

    public DateTime Oprettet { get; set; } = DateTime.UtcNow;
    public DateTime? SidstLoggetInd { get; set; }
}

/// <summary>Markerer en tekstegenskab, der skal gemmes krypteret i databasen.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class KrypteretAttribute : Attribute;
