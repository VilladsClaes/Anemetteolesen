using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Anemette.Models;

namespace Anemette.Controllers
{
    public class PersonController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();

        // GET: Person
        public ActionResult Index()
        {
            var tblPersons = db.tblPersons.Include(t => t.tblBillet).Include(t => t.tblEventRegion).Include(t => t.tblSted).Include(t => t.tblTjan);
            return View(tblPersons.ToList());
        }

        // GET: Person/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPerson tblPerson = db.tblPersons.Find(id);
            if (tblPerson == null)
            {
                return HttpNotFound();
            }
            return View(tblPerson);
        }

        // GET: Person/Create
        public ActionResult Create()
        {
            ViewBag.FK_BilletType = new SelectList(db.tblBillets, "ID", "BilletType");
            ViewBag.FK_Region_Hjemstavn = new SelectList(db.tblEventRegions, "ID", "Region");
            ViewBag.FK_HvorKenderViPersonenFra = new SelectList(db.tblSteds, "ID", "HvorKenderViPersonenFra");
            ViewBag.FK_TjansUnderEvent = new SelectList(db.tblTjans, "ID", "TjansUnderEvent");
            return View();
        }

        // POST: Person/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NavnTilPerson,TelefonnummerTilPerson,EmailTilPerson,BilledeAfPerson,RSVP,SamtykkeTilDenneListe,FK_HvorKenderViPersonenFra,MedlemsskabAfForetagsomheden,BidragTilAuktionen,BidragTilSneglebingo,BidragTilForberedelse,FK_TjansUnderEvent,Salt,HashetLink,FK_BilletType,AlderAfPerson,LoebeDistanceForPerson,FK_Region_Hjemstavn")] tblPerson tblPerson)
        {
            if (ModelState.IsValid)
            {
                db.tblPersons.Add(tblPerson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FK_BilletType = new SelectList(db.tblBillets, "ID", "BilletType", tblPerson.FK_BilletType);
            ViewBag.FK_Region_Hjemstavn = new SelectList(db.tblEventRegions, "ID", "Region", tblPerson.FK_Region_Hjemstavn);
            ViewBag.FK_HvorKenderViPersonenFra = new SelectList(db.tblSteds, "ID", "HvorKenderViPersonenFra", tblPerson.FK_HvorKenderViPersonenFra);
            ViewBag.FK_TjansUnderEvent = new SelectList(db.tblTjans, "ID", "TjansUnderEvent", tblPerson.FK_TjansUnderEvent);
            return View(tblPerson);
        }

        // GET: Person/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPerson tblPerson = db.tblPersons.Find(id);
            if (tblPerson == null)
            {
                return HttpNotFound();
            }
            ViewBag.FK_BilletType = new SelectList(db.tblBillets, "ID", "BilletType", tblPerson.FK_BilletType);
            ViewBag.FK_Region_Hjemstavn = new SelectList(db.tblEventRegions, "ID", "Region", tblPerson.FK_Region_Hjemstavn);
            ViewBag.FK_HvorKenderViPersonenFra = new SelectList(db.tblSteds, "ID", "HvorKenderViPersonenFra", tblPerson.FK_HvorKenderViPersonenFra);
            ViewBag.FK_TjansUnderEvent = new SelectList(db.tblTjans, "ID", "TjansUnderEvent", tblPerson.FK_TjansUnderEvent);
            return View(tblPerson);
        }

        // POST: Person/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NavnTilPerson,TelefonnummerTilPerson,EmailTilPerson,BilledeAfPerson,RSVP,SamtykkeTilDenneListe,FK_HvorKenderViPersonenFra,MedlemsskabAfForetagsomheden,BidragTilAuktionen,BidragTilSneglebingo,BidragTilForberedelse,FK_TjansUnderEvent,Salt,HashetLink,FK_BilletType,AlderAfPerson,LoebeDistanceForPerson,FK_Region_Hjemstavn")] tblPerson tblPerson)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblPerson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FK_BilletType = new SelectList(db.tblBillets, "ID", "BilletType", tblPerson.FK_BilletType);
            ViewBag.FK_Region_Hjemstavn = new SelectList(db.tblEventRegions, "ID", "Region", tblPerson.FK_Region_Hjemstavn);
            ViewBag.FK_HvorKenderViPersonenFra = new SelectList(db.tblSteds, "ID", "HvorKenderViPersonenFra", tblPerson.FK_HvorKenderViPersonenFra);
            ViewBag.FK_TjansUnderEvent = new SelectList(db.tblTjans, "ID", "TjansUnderEvent", tblPerson.FK_TjansUnderEvent);
            return View(tblPerson);
        }

        // GET: Person/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPerson tblPerson = db.tblPersons.Find(id);
            if (tblPerson == null)
            {
                return HttpNotFound();
            }
            return View(tblPerson);
        }

        // POST: Person/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblPerson tblPerson = db.tblPersons.Find(id);
            db.tblPersons.Remove(tblPerson);
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
