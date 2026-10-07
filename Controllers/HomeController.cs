using Anemette.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Anemette.Controllers
{
    public class HomeController : Controller
    {

        //The code private StoreContext db = new StoreContext(); instantiates a new context object for use by the controller. This is then used throughout the lifetime of the controller and disposed of by the Dispose method at the end of the controller code.
        private DatabaseEntities db = new DatabaseEntities();

        // GET: Home
        public ActionResult _Navbar()
        {

            return PartialView("_Navbar");
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Skarresøhus Forlag";
            return View();


        }


        //Sidevisning med loginformular        
        public ActionResult Login()
        {

            tblAdmin bruger = new tblAdmin();

            //Besked til besøgende
            ViewBag.Besked = "Værsgo at logge ind";

            //Fjern Session-fil når denne side tilgås
            if (Session["LoginBruger"] != null)
            {
                Session.Remove("LoginBruger");
                ViewBag.Besked = "Du er blevet logget ud";
            }

            return View(bruger);
        }

        //Loginformular POST (ved tryk på log in)
        [HttpPost]
        public ActionResult Login(tblAdmin bruger, FormCollection MinformCollection)
        {
            //Bliv på siden hvis model-validering ikke opnås
            if (!ModelState.IsValid)
            {
                ViewBag.Besked = "Noget er galt med modellen";
                return View();
            }

            tblAdmin BrugerMatch = new tblAdmin();
            //Tjek om det der skrives i viewet svarer til et brugernavn i databasen
            BrugerMatch = db.tblAdmins.Where(b => b.Brugernavn == bruger.Brugernavn).FirstOrDefault();

            //Hvis Username i databasen ikke svarer til brugernavn i formularen
            if (BrugerMatch == null)
            {
                Session.Remove("LoginBruger");
                //Giv besked til viewet
                ViewBag.Besked = "Der er ikke match på brugernavn";
                return View();
            }
            //Tag det der blev skrevet som password. Kør det igennem en hashing krydret med salt. 
            string MyPassword = HashSalt.HashPassword(MinformCollection["Password"], BrugerMatch.Salt);
            //Find om der er et navn i databasen som svarer til det der blev tastet i html-formularen (skrevet som lampda-udtryk). Hvis der er et match i databasen (et password hashet og saltet på samme måde) så tag den første
            BrugerMatch = (from U in db.tblAdmins where U.Brugernavn == bruger.Brugernavn && U.Password == MyPassword select U).FirstOrDefault();

            if (BrugerMatch != null)
            {
                //Opret en session-fil med den indloggede bruger. Dette skal bruges til at tjekke adgang på alle Admin-sider
                Session["LoginBruger"] = bruger.Brugernavn;
                //Opret en anden sessionsfil. 
                Session["BrugerID"] = bruger.ID;
                ViewBag.Besked = "Du er nu logget ind";

                //Response.Redirect("");
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                //Giv besked til viewet
                ViewBag.Besked = "Det er der ingen der hedder";
            }

            return View();

        }



        //Indkøbskurv
        public ActionResult TilfojKurv(tblProdukt tingderskalkoebes)
        {
            //Hvis kurven er null laves en liste med produkter hvor man tilføjer det man har trykket på fra viewet og tilføjer det til session   
            if (Session["cart"] == null)
            {
                List<tblProdukt> li = new List<tblProdukt>();

                li.Add(tingderskalkoebes);
                Session["cart"] = li;
                ViewBag.cart = li.Count();
                //Opret en session der holder antallet af varer
                Session["count"] = 1;

            }
            else
            {
                //Hvis kurven ikke er null så skal der oprettes en session som indeholder listen og der skal lægges 1 til antallet af produkter
                List<tblProdukt> li = (List<tblProdukt>)Session["cart"];
                li.Add(tingderskalkoebes);
                Session["cart"] = li;
                ViewBag.cart = li.Count();
                Session["count"] = Convert.ToInt32(Session["count"]) + 1;

            }
            //Efter man har trykker på knappen føres man tilbage til listen med varer
            return Redirect(Request.UrlReferrer.ToString());




        }

        //Indkøbskurv i View
        public ActionResult MinOrdre()
        {

            return View((List<tblProdukt>)Session["cart"]);

        }

        public ActionResult FjernFraKurv(tblProdukt fjernetprodukt)
        {
            List<tblProdukt> li = (List<tblProdukt>)Session["cart"];
            li.RemoveAll(x => x.ID == fjernetprodukt.ID);
            Session["cart"] = li;
            Session["count"] = Convert.ToInt32(Session["count"]) - 1;
            return RedirectToAction("MinOrdre", "Home");

        }









        [HttpGet]
        public ActionResult PlacerOrdre()
        {
            Anemette.Models.ViewModel vm = new Models.ViewModel();
            vm.Personer = db.tblPersons.ToList();
            vm.Produkter = (List<tblProdukt>)Session["cart"];
            return View(vm);
        }

      
        [HttpPost]
        public ActionResult PlacerOrdre(ViewModel Bestilling)      
        {

           
            if (ModelState.IsValid)
            {
                
                //Hvis emailen findes i forvejen skal databasen opdatere den eksisterende fortegnelse
                if (db.tblPersons.Where(k => k.EmailTilPerson == Bestilling.Person.EmailTilPerson).FirstOrDefault() != null)
                {                    
                    var kendtFraTidligere = db.tblPersons.Where(personer => personer.EmailTilPerson == Bestilling.Person.EmailTilPerson).FirstOrDefault();
                    Bestilling.Person.ID = kendtFraTidligere.ID;
                    Session["PersonID"] = Bestilling.Person.ID;
                    kendtFraTidligere.NavnTilPerson = Bestilling.Person.NavnTilPerson;
                    kendtFraTidligere.AntalGangePersonenHarBesogtSiden = Bestilling.Person.AntalGangePersonenHarBesogtSiden + 1;
                    kendtFraTidligere.AdresseTilPerson = Bestilling.Person.AdresseTilPerson;
                    db.Entry(kendtFraTidligere).State = EntityState.Modified;


                    db.SaveChanges();
                    
                }
                //Hvis emailen ikke findes skal der oprettes en ny fortegnelse
                else
                {
                    db.tblPersons.Add(Bestilling.Person);                    
                    db.SaveChanges();
                    Session["PersonID"] = Bestilling.Person.ID;
                }

                //Uanset om vi kender dem eller ej, skal varerne trækkes fra lagerbeholdningen
                var produkterIKurven = (List<tblProdukt>)Session["cart"];
                Bestilling.Produkter = produkterIKurven;

                foreach (var produkt in produkterIKurven)
                {
                    produkt.Antal -= 1;
                }

                db.SaveChanges();

                //Opret et brev
                MailViewModel Brev = new MailViewModel();
                Brev.MailAdressFrom = Bestilling.Person.EmailTilPerson;
                Brev.MailAdressTo = "anemette@anemetteolesen.dk"; //Her skal Anemette@anemetteolesen.dk stå
                Brev.MailSubject = "Bestilling af " + Session["count"].ToString() + " varer";
                Brev.MailTelefon = Bestilling.Person.TelefonnummerTilPerson;
                Brev.MailName = Bestilling.Person.NavnTilPerson;

                var kurven = Session["cart"] as List<Anemette.Models.tblProdukt>;
                StringBuilder kurvenSomTekst = new StringBuilder();
                kurvenSomTekst.Append("De følgende bøger bestilles: <br/><br/>Produktnavn::");
                foreach (var produkt in kurven)
                {
                    kurvenSomTekst.AppendFormat("<br/>{0}", produkt.Navn.ToString());
                }

                Brev.MailBody = kurvenSomTekst.ToString();
                Brev.MailBody =
                        @"Kundens navn: <b>" + Brev.MailName + "</b>" +
                        "<br/> " +
                        "Adresse:  <b>" + Bestilling.Person.AdresseTilPerson + "</b>" +
                        "<br/>" +
                        "Telefonnummer:  <b>" + Brev.MailTelefon + "</b>" +
                        "<br/>" +
                        "Har bestilt:  <b>" + Brev.MailBody.Replace(Environment.NewLine, "</b><br/>");
                Brev.Message = "Bestillingen er sendt";
                SendMail(Brev);

                
            }

            Bestilling.Besked = "Bestillingen er sendt";
            return View(Bestilling);
        }





















        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OpretBruger(tblAdmin User)
        {
           

            tblAdmin newUser = db.tblAdmins.Where(c => c.BrugerEmail == User.BrugerEmail).FirstOrDefault();
            if (newUser != null)
            {
                ViewBag.Besked = "Der er allerede oprette en bruger med denne email - gå til login istedet";
                return View();
            }

            string NewSalt = HashSalt.GetRandomSalt();
            User.Salt = NewSalt;
            User.Password = HashSalt.HashPassword(User.Password, NewSalt);

            db.tblAdmins.Add(User);
            db.SaveChanges();
            return RedirectToAction("Index", "Login");


        }




















        //GET: Partialview ved navn _Footer
        [ChildActionOnly]
        public ActionResult _Footer()
        {
            
            return PartialView( "_Footer");
        }

        [ChildActionOnly]
        public ActionResult _OmAnemette()
        {
            Models.tblArtikel model = db.tblArtikels.Where(specifik => specifik.ArtikelOverskrift == "Om-Anemette").FirstOrDefault(); 
            
            return PartialView("_OmAnemette", model);
        }

        


        //------------------------------------Kontaktformular------------------------------------------------------
        //Send en besked via en formular


        public ActionResult ContactForm()
        {
            Anemette.Models.MailViewModel vm = new Models.MailViewModel();
            return View(vm);
        }


        //The [HttpPost] attribute tells the controller that when it receives a POST request for the Create action, it should use this method rather than the other create method.
        [HttpPost]
        public ActionResult ContactForm(Anemette.Models.MailViewModel skrevneMail)
        {
            if (ModelState.IsValid)
            {

                skrevneMail.MailAdressFrom = skrevneMail.MailAdressFrom;
                skrevneMail.MailAdressTo = "villadsclaes@villadsclaes.dk";
                skrevneMail.MailSubject = "Emne: " + skrevneMail.MailSubject;
                skrevneMail.MailBody = @"Kundens navn: <b>" + skrevneMail.MailName + "<br/> "
                    + "</b>" +
                    "Emne:  <b>" + skrevneMail.MailSubject + "</b> <br/>" +
                    "Besked:  <b>" + skrevneMail.MailBody.Replace(Environment.NewLine, "</b><br/>");
                skrevneMail.MailTelefon = skrevneMail.MailTelefon;

                //Tilføj personen til tblPerson

                Anemette.Models.tblPerson GemAfsenderen = new tblPerson();


                tblPerson TjekEksisterendeBruger = db.tblPersons.Where(c => c.EmailTilPerson == skrevneMail.MailAdressFrom).FirstOrDefault();
                if (TjekEksisterendeBruger != null)
                {
                    skrevneMail.Message = "Dig kender vi i forvejen, og beskeden er sendt";
                    return View(skrevneMail);
                }
                else
                {
                    skrevneMail.Message = "Vi gemmer dig i vores kontaktbog, og vi har modtaget din besked";
                    GemAfsenderen.NavnTilPerson = skrevneMail.MailName;
                    GemAfsenderen.EmailTilPerson = skrevneMail.MailAdressFrom;
                    GemAfsenderen.TelefonnummerTilPerson = skrevneMail.MailTelefon;
                    db.tblPersons.Add(GemAfsenderen);
                    db.SaveChanges();
                }
                skrevneMail.Message = "Beskeden er sendt";
                SendMail(skrevneMail);
            }
            else
            {
                skrevneMail.Message = "Du har ikke udfyldt Kontakt formularen korrekt, prøv igen.";
            }



            return View(skrevneMail);
        }




        //Istedet for at lave en ekstern klassefil til at sende mails via:
        public void SendMail(Anemette.Models.MailViewModel myMail)
        {
            
            // starter på en ny mail (laver en ny instans af mailmessage).. 
            //.net biblioteket Mail med klassen Mailmessage 
            MailMessage mail = new MailMessage();

            // From er = mailens afsender. Det er ikke mailens afsender når vi bruger gmails smtp, CHeck ved webhost.
            mail.From = new MailAddress(myMail.MailAdressFrom);

            //Dette er den mail som man besvare til (tilbage til kd efter kontakt med hjemmeside staf(input MailFrom i Contact form))
            mail.ReplyToList.Add(myMail.MailAdressFrom);

            // Emnefelt på mail. VIGTIG, men ikke et ultimativt krav.
            mail.Subject = myMail.MailSubject;

            //Er den besked der skrives i textarea. (Da vi har valgt at sende HTML kan vi erstate NewLine(Enviroment) med <br/>)
            mail.Body = myMail.MailBody;
            mail.IsBodyHtml = true;

            //er den mail add der modtager mail fra form
            //statisk i Contact form, dynamisk i newlettter = mail from.
            mail.To.Add(myMail.MailAdressTo);

            //"smtp" er en instans af smtpClient,  Smtp = simpel mail transport protocol
            SmtpClient smtp = new SmtpClient();

            //host er udbyderen udgående mailserver.
            //her er der gmails smtp og port.
            //tjek med webhost........!
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;

            //ssl er nøjvendigt  for gmail - tjek med udbyder..
            smtp.EnableSsl = true;

            //Her slår vi standart longi-oplysningerne fra.. 
            smtp.UseDefaultCredentials = false;

            //her skriver vi vores login oplysninger.
            //Tilføj tilladelse på https://myaccount.google.com/u/1/lesssecureapps
            smtp.Credentials = new System.Net.NetworkCredential("skarresoehusx@gmail.com", "ballevej28");

            //Her pakker vi hele instansen(alt data tastet ovenfor) "mail" ned som parameter til metoden send.
            smtp.Send(mail);

        }
    }
}