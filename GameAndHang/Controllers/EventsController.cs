using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GameAndHang.DAL;
using GameAndHang.Models;
using Microsoft.AspNet.Identity;

namespace GameAndHang.Controllers
{

    public class EventsController : Controller
    {
        [NonAction]
        public bool NeededEvent(Event event1)
        {
            DateTime timeNow = DateTime.Now;

            DateTime timeofEvent = event1.Date;

            if (timeofEvent < timeNow)
                return false;
            else
                return true;
        }

        private GnHContext db = new GnHContext();

        // GET: Events
        public async Task<ActionResult> Index()
        {
            var events = db.Events.Include(r => r.User);

            var filtered = events.Where(NeededEvent);

           // return View(await events.ToListAsync());
            return View(filtered);
        }

        public ActionResult Search(string search)
        {
            ViewBag.Games = new SelectList(db.Games.Select(x => x.Name), "Name");
            return View(db.Events.Where(x => x.UnsupGames.Contains(search)));
        }

        public ActionResult SearchBox(string Games)
        {
            //string gameName = data.Name;
            return View(db.EventGames.Where(x => x.Game.Name.Contains(Games)));
        }

        //public JsonResult GetData(string data)
        //{
        //    List<Event> eventlist = new List<Event>();
        //    db.Configuration.ProxyCreationEnabled = false;
        //    eventlist = db.Events.Where(x => x.UnsupGames.Contains(data)).ToList();
        //    return Json(eventlist, JsonRequestBehavior.AllowGet);
        //}

        // GET: Events/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            //ViewBag.HostID = new SelectList(db.Users, "ID", "CredentialsID");
            //ViewBag.Games = new SelectList(db.Games, "ID", "Name");
            //+ System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleAPIKey"].ToString() + "&callback=initMap";
            ViewBag.HostID = User.Identity.GetUserId();
            //ViewBag.ApiUrl = "https://maps.googleapis.com/maps/api/js?key=" + System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleAPIKey"].ToString() + "&callback=initMap";
            ViewBag.ApiUrl = "https://maps.googleapis.com/maps/api/js?key=AIzaSyDuwWq60IrpVvV1uNd-1IvOmlAZ2tAGAM8&callback=initMap";
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,EventName,IsPublic,Date,EventDescription,EventLocation,EventLat,EventLong,PlayerSlotsMin,PlayerSlotsMax,PlayersCount,UnsupGames,HostID")] Event @event)
        {
            var currentID = User.Identity.GetUserId();
            AspNetUser currentUser = db.AspNetUsers.Find(currentID);
            //if (currentID == null || currentUser.EmailConfirmed == false)
            //{
            //    ModelState.AddModelError("HostID", "Plese login before creating an event!");
            //    return View(@event);
            //}
            //else if (currentUser.EmailConfirmed == false)
            //{
            //    ModelState.AddModelError("HostID", "Plese confirm your email before creating an event!");
            //    return View(@event);
            //}
           
            
                @event.HostID = currentID;
                Guid g = Guid.NewGuid();
                string gIDString = Convert.ToBase64String(g.ToByteArray());
                gIDString = gIDString.Replace("=", "");
                gIDString = gIDString.Replace("+", "");
                gIDString = gIDString.Replace("/", "");

                @event.ID = gIDString;

                @event.PlayersCount = 1;

                User TheUser = db.Users.Find(currentID);

                if (TheUser != null)
                {
                    TheUser.HostXP += 20;
                    db.Entry(TheUser).State = EntityState.Modified;
                    db.SaveChanges();
                }


                if (ModelState.IsValid)
                {
                    db.Events.Add(@event);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Create", "EventGames");
                }

                ViewBag.HostID = new SelectList(db.Users, "ID", "ID", @event.HostID);
                return View(@event);
            
        }

        // GET: Events/Edit/5
        public async  Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            ViewBag.HostID = new SelectList(db.Users, "ID", "FirstName", @event.HostID);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,EventName,IsPublic,Date,EventDescription,EventLocation,PlayerSlotsMin,PlayerSlotsMax,UnsupGames,HostID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.HostID = new SelectList(db.Users, "ID", "FirstName", @event.HostID);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Event @event = await db.Events.FindAsync(id);
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
