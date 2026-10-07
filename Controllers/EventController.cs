using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Anemette.Models;

namespace Anemette.Controllers
{
    public class EventController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();

        // GET: Event
        public ActionResult Index()
        {
            //It tells Entity Framework to perform a single query and retrieve all the products and also all the related categories. Eager loading typically results in an SQL join query that retrieves all the required data at once. You could omit the Include method and Entity Framework would use lazy loading, which would involve multiple queries rather than a single join query.
            var tblEvents = db.tblEvents.Include(t => t.tblEventRegion).Include(t => t.tblSponsor).Include(t => t.tblType);
            return View(tblEvents.ToList());
        }

        // GET: Event/Details/5
        public ActionResult EnkeltKursus(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblEvent detteEvent = db.tblEvents.Find(id);
            if (detteEvent == null)
            {
                return HttpNotFound();
            }
            
            return View(detteEvent);
        }

        // GET: Event/Create
        public ActionResult OpretKursus()
        {
            ViewBag.FK_Region = new SelectList(db.tblEventRegions, "ID", "Region");
            ViewBag.FK_Sponsor = new SelectList(db.tblSponsors, "ID", "SponsorNavn");
            ViewBag.FK_Type = new SelectList(db.tblTypes, "ID", "Type");


            return View();
        }


       


        // POST: Produkt/Create
        [HttpPost]
        //[ValidateAntiForgeryToken] ensures that the token passed by the HTML form, thus validating the request. The purpose of this is to ensure that the request actually came from the form it is expected to come from in order to prevent cross-site request forgeries. In simple terms, a cross-site request forgery is a request from a form on another web site to your web site with malicious intentions.
        [ValidateAntiForgeryToken]
        //The parameters ([Bind(Include = “ID,Name”)] Category category) tell the method to include only the ID and the Name properties when adding a new category. The Bind attribute is used to protect against overposting attacks by creating a list of safe properties to update; however, as we will discuss later, it does not work as expected and so it is safer to use a different method for editing or creating where some values may be blank. As an example of overposting, consider a scenario where the price is submitted as part of the request when a user submits an order for a product. An overposting attack would attempt to alter this price data by changing the submitted request data in an attempt to buy the product cheaper.
        public ActionResult OpretKursus(IEnumerable<HttpPostedFileBase> SamlingAfBilleder, tblEvent NytEvent)
        {
            if (ModelState.IsValid)
            {

                //Denne linje udputter SQL til Output-vinduet her i VS
                db.Database.Log = sql => Trace.WriteLine(sql);
                #region FotoUpload

                if (SamlingAfBilleder != null)
                {
                    //For alle billeder: Gem filen i filmappen, hvis den er valideret
                    foreach (var BilledeAfEvent in SamlingAfBilleder)
                    {


                        if (BilledeAfEvent != null)
                        {
                            var BilledetsFilnavn = "";

                            //Tjek billedet for filformat og størrelse
                            if (IOTools.ValidateFile(BilledeAfEvent))
                            {
                               
                                //Forsøg at gemme filen
                                try
                                {
                                    BilledetsFilnavn = IOTools.GivUniktNavn(BilledeAfEvent);
                                    IOTools.SaveFileToDisk(BilledeAfEvent, BilledetsFilnavn);
                                }
                                //Hvis det ikke lykkes
                                catch (Exception)
                                {
                                    ModelState.AddModelError("BilledeFil", "Der skete sgu en fejl med det billede");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("BilledeFil", "Det skal være et rigtigt billede og det må ikke fylde særligt meget");
                            }

                            //Tildel modellen samme filnavn
                            tblBillede NytBillede = new tblBillede();
                            NytBillede.BilledeFil = BilledetsFilnavn;
                            NytBillede.FK_Event = NytEvent.ID;
                            db.tblBilledes.Add(NytBillede);
                        }
                       


                    }
                }
                //Hvis der ikke er valgt et billede
                else
                {
                    ModelState.AddModelError("BilledeFil", "Vælg en fil");
                }


                #endregion

                //Tilføj produktet til databasen
                db.tblEvents.Add(NytEvent);
                //Gem databsen
                db.SaveChanges();



                return RedirectToAction("Index");
            }

            ViewBag.FK_Region = new SelectList(db.tblEventRegions, "ID", "Region", NytEvent.FK_Region);
            ViewBag.FK_Sponsor = new SelectList(db.tblSponsors, "ID", "SponsorNavn", NytEvent.FK_Sponsor);
            ViewBag.FK_Type = new SelectList(db.tblTypes, "ID", "Type", NytEvent.FK_Type);
            return View(NytEvent);

        }

        // GET: Event/Edit/5
        public ActionResult EditKursus( int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblEvent detteEvent = db.tblEvents.Find(id);
            if (detteEvent == null)
            {
                return HttpNotFound();
            }
            detteEvent.EventDato = db.tblEvents.Find(id).EventDato;



            ViewBag.FK_Region = new SelectList(db.tblEventRegions, "ID", "Region", detteEvent.FK_Region);
            ViewBag.FK_Sponsor = new SelectList(db.tblSponsors, "ID", "SponsorNavn", detteEvent.FK_Sponsor);
            ViewBag.FK_Type = new SelectList(db.tblTypes, "ID", "Type", detteEvent.FK_Type);
            return View(detteEvent);
        }

        // POST: Event/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditKursus([Bind(Include = "ID,EventOverskrift,EventDato,EventBeskrivelse,EventPris,EventPladser,EventDistance,FK_Region,FK_Sponsor,FK_Type")] tblEvent tblEvent, IEnumerable<HttpPostedFileBase> EkstraSamlingAfBilleder)
        {
            if (ModelState.IsValid)
            {

                if (EkstraSamlingAfBilleder != null)
                {
                    //For alle billeder: Gem filen i filmappen, hvis den er valideret
                    foreach (var EkstraBilledeAfEvent in EkstraSamlingAfBilleder)
                    {


                        if (EkstraBilledeAfEvent != null)
                        {
                            var BilledetsFilnavn = "";

                            //Tjek billedet for filformat og størrelse
                            if (IOTools.ValidateFile(EkstraBilledeAfEvent))
                            {

                                //Forsøg at gemme filen
                                try
                                {
                                    BilledetsFilnavn = IOTools.GivUniktNavn(EkstraBilledeAfEvent);
                                    IOTools.SaveFileToDisk(EkstraBilledeAfEvent, BilledetsFilnavn);
                                }
                                //Hvis det ikke lykkes
                                catch (Exception)
                                {
                                    ModelState.AddModelError("BilledeFil", "Der skete sgu en fejl med det billede");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("BilledeFil", "Det skal være et rigtigt billede og det må ikke fylde særligt meget");
                            }

                            //Tildel modellen samme filnavn
                            tblBillede NytBillede = new tblBillede();
                            NytBillede.BilledeFil = BilledetsFilnavn;
                            NytBillede.FK_Event = tblEvent.ID;
                            db.tblBilledes.Add(NytBillede);
                        }



                    }
                }
                //Hvis der ikke er valgt et billede
                else
                {
                    ModelState.AddModelError("BilledeFil", "Vælg en fil");
                }


                db.Entry(tblEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FK_Region = new SelectList(db.tblEventRegions, "ID", "Region", tblEvent.FK_Region);
            ViewBag.FK_Sponsor = new SelectList(db.tblSponsors, "ID", "SponsorNavn", tblEvent.FK_Sponsor);
            ViewBag.FK_Type = new SelectList(db.tblTypes, "ID", "Type", tblEvent.FK_Type);

            return View(tblEvent);
        }

        // GET: Event/Delete/5
        public ActionResult SletKursus(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblEvent tblEvent = db.tblEvents.Find(id);
            if (tblEvent == null)
            {
                return HttpNotFound();
            }
            return View(tblEvent);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("SletKursus")]        
        public ActionResult DeleteConfirmed(int id)
        {
            tblEvent tblEvent = db.tblEvents.Find(id);
            //Fjern billede-relationen til eventet FK_tblBillede_tblEvent
            //https://stackoverflow.com/a/25228623/6826776
            db.tblEvents.Remove(tblEvent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

           

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

   


    }
}
