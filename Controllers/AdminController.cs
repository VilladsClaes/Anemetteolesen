using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Anemette.Filters;
using Anemette.Models;

namespace Anemette.Controllers
{
    //Tillad kun indgang hvis administratoren er logget ind
    [AdminAdgang]
    public class AdminController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();




        public ActionResult _Navbar()
        {

            return PartialView("_NavbarAdmin");
        }

        // GET: Admin
        public ActionResult Index()
        {

            
          
            //Viser en side med overblik over lidt tal
            ViewModel Oversigt = new ViewModel();

            //Alt hvad der ligger i databasen om tilmeldte til nyhedsbreve
            Oversigt.Nyhedsbrevstilmeldte = db.tblNyhedsbrevs.ToList();

            //Alt hvad der ligger i databasen om Events
            Oversigt.Events = db.tblEvents.ToList();

            //Alle der er tilmeldte events
            Oversigt.DeltagerTilmeldings = db.tblTilmeldings.ToList();

            //Sig hej til admin ved at vise hvad der er i session[brugerlogin]
            //Oversigt.Adminstrator.Brugernavn = Session["LoginBruger"].ToString();


            return View(Oversigt);
        }

        public ActionResult _AdministratorListe()
        {
            return PartialView(db.tblAdmins.ToList());
        }




        //Den allerførste administrator må oprettes uden login (når databasen er tom). Derefter kræves login.
        private bool MaaOpretteAdministrator()
        {
            return Session["LoginBruger"] != null || !db.tblAdmins.Any();
        }

        //Sidevisning til Opret ny bruger
        [OverrideActionFilters]
        public ActionResult OpretAdministrator()
        {
            if (!MaaOpretteAdministrator())
            {
                return Redirect("/Home/Login");
            }
            tblAdmin NyBruger = new tblAdmin();

            return View(NyBruger);
        }

        //Oprettelse med formular(POST) til ny bruger
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OverrideActionFilters]
        public ActionResult OpretAdministrator([Bind(Include = "Brugernavn,Password,BrugerEmail")] tblAdmin nybruger)
        {
            if (!MaaOpretteAdministrator())
            {
                return Redirect("/Home/Login");
            }

            if (ModelState.IsValid)
            {
                var isEmailAlreadyExists = db.tblAdmins.Any(x => x.BrugerEmail == nybruger.BrugerEmail || x.Brugernavn == nybruger.Brugernavn);
                if (isEmailAlreadyExists)
                {
                    ViewBag.Besked = "Der er allerede en bruger med dette brugernavn eller denne email";
                    return View(nybruger);
                }
                string NewSalt = HashSalt.GetRandomSalt();
                nybruger.Salt = NewSalt;
                nybruger.Password = HashSalt.HashPassword(nybruger.Password, NewSalt);

                db.tblAdmins.Add(nybruger);
                db.SaveChanges();
                ViewBag.Besked = "Du har nu oprettet din profil";
                //Den første administrator er ikke logget ind endnu og sendes til login
                if (Session["LoginBruger"] == null)
                {
                    return Redirect("/Home/Login");
                }
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Besked = "Noget gik galt med din oprettelse";

           
            return View(nybruger);
        }

        
        public ActionResult DetaljeAdministrator(int? id)
        {
            tblAdmin editbruger = new tblAdmin();
            editbruger = db.tblAdmins.Find(id);
            if (editbruger == null)
            {
                return HttpNotFound();
            }

            return View(editbruger);
        }

        //Formular POST til Editering af profil   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DetaljeAdministrator(tblAdmin editbruger)
        {
            if (ModelState.IsValid)
            {
                //Gør brug af den class-fil som vi har gennemgået i undervisning
                string NytSalt = HashSalt.GetRandomSalt();

                //Lægger salt'en ind i databasen for brugeren.
                editbruger.Salt = NytSalt;
                //Lægger passwordet ind i databasen, men først konkatineres det med salt og hashes med SHA256 
                editbruger.Password = HashSalt.HashPassword(editbruger.Password, NytSalt); //tager metode fra cs-filen med to argumenter
                //Sæt en Session-fil med ID fra brugeren
                editbruger.ID = (int)Session["BrugerID"];
                //Ændr på database-record
                db.Entry(editbruger).State = EntityState.Modified;
                // kan også skrives db.Brugers.AddOrUpdate(editbruger);

                ViewBag.Besked = "Du har nu ændret din profil";
                db.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Besked = "Noget gik galt med din ændring";
            return View(editbruger);
        }


        // GET: Produkt/Delete/5

        public ActionResult SletAdministrator(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAdmin sletAdmin = db.tblAdmins.Find(id);
            if (sletAdmin == null)
            {
                return HttpNotFound();
            }
            return View(sletAdmin);
        }

        // POST: Logins/Delete/5
        [HttpPost, ActionName("SletAdministrator")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed()
        {
            tblAdmin sletbruger = new tblAdmin();
            //Find bruger baseret på Session-fil indeholdende ID fra databasen
            sletbruger = db.tblAdmins.Find((int)Session["BrugerID"]);
            //Slet denne record fra tabellen
            db.tblAdmins.Remove(sletbruger);
            //Gem databasen
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //En logud-metode ved tryk på knap "Log ud"
        public ActionResult Logud()
        {
            //Fjern alle sessioner
            Session.RemoveAll();
            //Går tilbage til det view vi kalder Index (over dette actionresult) i den controller der hedder Admin (den her controller)
            return RedirectToAction("Index", "Home");
        }



























    }
}
