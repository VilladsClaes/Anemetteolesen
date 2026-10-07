using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//Fra følgende tutor:
//https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/database-first-development/enhancing-data-validation#add-metadata-classes
namespace Anemette.Models
{


    public class tblProduktMetadata
    {
        [Display(Name = "Produktnavn")]
        [Required(ErrorMessage = "Produktet skal som minimum have et navn")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Skriv et længee navn")]
        //[RegularExpression(@"^[a-zA-Z0-9'-'\s]*$", ErrorMessage = "Kun almindelige bogstaver osv")]
        public string Navn;

        [Display(Name = "Produktbeskrivelse")]
        //[Required(ErrorMessage = "The product description cannot be blank")]
        //[StringLength(200, MinimumLength = 10, ErrorMessage = "Please enter a product description between 10 and 200 characters in length")]
        //[RegularExpression(@"^[,;a-zA-Z0-9'-'\s]*$", ErrorMessage = "Please enter a product description made up of letters and numbers only")]
        //[DataType(DataType.MultilineText)]
        public string Beskrivelse;


        //[Required(ErrorMessage = "Ting skal have en pris")]
        //[Range(0.10, 10000, ErrorMessage = "Prisen skal være mellem {0} og {1}")]
        //[DataType(DataType.Currency)]
        ////[DisplayFormat(DataFormatString = “{0:c}”)] specifies that the price property should be displayed in currency format, i.e., £1,234.56 (with the currency set by the server locale). Generally either of these attributes should work and display the price formatted as currency.We have included them both here for completeness.
        //[DisplayFormat(DataFormatString = "{0:c}")]
        public string Pris;

        public string Antal;

    }

    public class tblBilledeMetadata
    {

        [Display(Name = "Billedefil")]
        //[StringLength(100)]
        //[Index(IsUnique = true)]
        public string BilledeFil;

        [Display(Name = "Hvad forestiller billedet")]
        public string BilledeAlternativTekst;

        [Display(Name = "Billedets titel")]
        public string BilledeOverskrift;
    }

    public class tblProduktKategoriMetadata
    {
        [Display(Name = "Kategorinavn")]
        [Required(ErrorMessage = "Der skal være et navn til kategorien")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kategoriens navn skal være mellem 3 og 50 tegn langt")]
        // this expression says the first character must be an uppercase letter followed by a repetition of letter and spaces. 
        //https://regex101.com/
        [RegularExpression(@"^[A-ZÆØÅa-zæøå_0-9]+[A-ZÆØÅa-zæøå_0-9''-'\s]*$", ErrorMessage = "Nogle af de tegn du har skrevet er ulovlige som kategorinavn")]
        public string Kategori;


    }


    public class tblStedMetadata
    {
        [Display(Name = "Hvor kender vi dig fra?")]
        //[Required(ErrorMessage = "Der skal stå hvilket netværk personen er fra")]       
        public string HvorKenderViPersonenFra;

    }


    public class tblPersonMetadata
    {
        [Display(Name = "Navn")]
        [MaxLength(50), MinLength(2)]
        public string NavnTilPerson;

        [Phone]
        [Display(Name = "Telefonnummer")]
        [MaxLength(20), MinLength(8)]
        public string TelefonnummerTilPerson;

        [Display(Name = "Email")]
        //[Required(ErrorMessage = "Du mangler din emailadresse")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailTilPerson;

        [Display(Name = "Profilbillede")]
        public string BilledeAfPerson;

        [Display(Name = "Adresse")]
        public string AdresseTilPerson;

        [Display(Name = "Deltager du?")]
        public string RSVP;

        [Display(Name = "Samtykke")]
        public string SamtykkeTilDenneListe;

        [Display(Name = "Hvor er personen fra")]
        public Nullable<int> FK_HvorKenderViPersonenFra;

        [Display(Name = "Hvilken landsdel er du fra?")]
        public Nullable<int> FK_Region_Hjemstavn;


        [Display(Name = "Medlem af Foretagsomheden?")]
        public string MedlemsskabAfForetagsomheden;

        [Display(Name = "Bidrag til Auktionen")]
        public string BidragTilAuktionen;

        [Display(Name = "Bidrag til Sneglebingo")]
        public string BidragTilSneglebingo;

        [Display(Name = "Bidrag til forberedelsesdagene")]
        public string BidragTilForberedelse;

        [Display(Name = "Tjans under event")]
        public Nullable<int> FK_TjansUnderEvent;

        [Display(Name = "Billettype")]
        public Nullable<int> FK_BilletType;

        [Display(Name = "Din fødselsdag")]
        [DataType(DataType.DateTime, ErrorMessage = "Der er noget galt med formatet")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> AlderAfPerson;

        [Display(Name = "Hvor langt kan du løbe")]
        [Range(500.0, Double.MaxValue, ErrorMessage = "Angiv distancen i meter (dvs. 1500 istedet for 1.5)")]
        public Nullable<int> LoebeDistanceForPerson;

        


    }

    public class tblTjanMetadata
    {
        [Display(Name = "Tjans på event")]
        //[Required(ErrorMessage = "Du skal vælge en tjans")]        
        public string TjansUnderEvent;


    }

    public class tblBilletMetadata
    {
        [Display(Name = "Billettype")]
        public string BilletType;
        [Display(Name = "Beskrivelse")]
        public string BilletBeskrivelse;
        [Display(Name = "Antal glas")]
        public Nullable<int> BilletAntalGlas;
        [Display(Name = "Pris")]
        public Nullable<int> BilletPris;
        [Display(Name = "Link")]
        public string BilletLink;


    }


    public class tblAdminMetadata
    {


        public string Brugernavn;


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Adgangskode")]
        public string Password;

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }


        public string Salt;
        
        
    }

    public class tblBestyrelseMetadata
    {

        [Display(Name = "Billede")]
        public string MedlemsBillede;
        
        [Display(Name = "Navn")]
        [MaxLength(50), MinLength(10)]
        public string MedlemsNavn;
        [Display(Name = "Titel")]
        public string MedlemsTitel;
        [Display(Name = "Beskrivelse")]
        public string MedlemsBeskrivelse;
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string MedlemsMail;
    }

    public class tblEventMetadata
    {

        [Display(Name = "Navn")]
        [Required(ErrorMessage = "Eventet skal have en overskrift")]
        [MaxLength(50, ErrorMessage = "Titlen må ikke være længere end 50 tegn")]
        public string EventOverskrift;

        [Display(Name = "Dato")]
        [Required(ErrorMessage = "Du skal vælge en dato for eventet!")]
        [DataType(DataType.Date, ErrorMessage = "Der er noget galt med formatet")]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        //Chrome skal have datoen præsenteret med årstal først
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> EventDato;


        [Display(Name = "Beskrivelse")]
        [Required(ErrorMessage = "Der bør være en beskrivelse af eventet")]
        [MinLength(10, ErrorMessage = "Beskrivelsen skal være længere end 10 tegn")]
        public string EventBeskrivelse;
        [Display(Name = "Hvor i landet")]
        public string FK_Region;
        [Display(Name = "Distance")]
        [Range(500.0, Double.MaxValue, ErrorMessage = "Angiv distancen i meter (dvs. 1500 istedet for 1.5)")]
        public string EventDistance;
        [Display(Name = "Pris")]
        public Nullable<int> EventPris;
        [Display(Name = "Pladser")]
        public Nullable<int> EventPladser;      

        
    }

    public class tblKontaktsideMetadata
    {
        public string Overskrift;
        public string Tekst;
    }

    public class tblNyhedsbrevMetadata
    {
        [Required]
        //[DataType(DataType.EmailAddress)]
        //[EmailAddress]
        public string Emailadresse; 
    }

    public class tblOmsideMetadata
    {
        public string Overskrift;
        public string Tekst;
        public string Billede; 
    }

    public class tblSponsorMetadata
    {
        [Display(Name = "Sponsorens navn")]
        //[Required(ErrorMessage = "Skriv et navn")]
        public string SponsorNavn;
        [Display(Name = "Sponsorens logo")]
        //[Required(ErrorMessage = "Vælg et billede")]
        public string SponsorLogo;
    
    }

    public class tblSponsorTypeMetadata
    {
        //[Display(Name = "Sponsorkategori")]
        //[Required(ErrorMessage = "Vælg en sponsorkategori")]
        public string SponsorType;
    }
    public class tblTilmeldingMetadata
    {
        [Display(Name = "Din email")]
        [Required(ErrorMessage = "Du mangler din emailadresse")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string TilmeldteEmail;
       

    }

    public class tblEventRegionMetadata
    {
        [Display(Name = "Landsdel")]
        public string Region;


    }



}