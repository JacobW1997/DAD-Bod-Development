using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GameAndHang.DAL;
using GameAndHang.Models;
using Microsoft.AspNet.Identity;

namespace GameAndHang.Controllers
{
    public class EventsController : Controller
    {
        private GnHContext db = new GnHContext();

        // GET: Events
        public ActionResult Index()
        {
            var events = db.Events.Include(r => r.User);
            return View(events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            ViewBag.HostID = new SelectList(db.Users, "ID", "CredentialsID");
            ViewBag.Games = new SelectList(db.Games, "ID", "Name");
            ViewBag.ApiUrl = "https://maps.googleapis.com/maps/api/js?key=" + System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleAPIKey"].ToString() + "&callback=initMap";
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EventName,IsPublic,Date,EventDescription,EventLocation,PlayerSlotsMin,PlayerSlotsMax,UnsupGames,HostID")] Event @event)
        {
            var getuserID = User.Identity.GetUserId();
            int currentUserID = db.Users
                .Where(x => x.CredentialsID == getuserID)
                .Select(x => x.ID)
                .Single();
            @event.HostID = currentUserID;
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Create", "EventGames");
            }

            ViewBag.HostID = new SelectList(db.Users, "ID", "CredentialsID", @event.HostID);
            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            ViewBag.HostID = new SelectList(db.Users, "ID", "CredentialsID", @event.HostID);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EventName,IsPublic,Date,EventDescription,EventLocation,PlayerSlotsMin,PlayerSlotsMax,UnsupGames,HostID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HostID = new SelectList(db.Users, "ID", "CredentialsID", @event.HostID);
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
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
