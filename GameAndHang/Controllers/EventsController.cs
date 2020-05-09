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
using GoogleMaps.LocationServices;
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

        public ActionResult Search(string search, double? UserLat, double? UserLong)
        {
            ViewBag.ApiUrl = "https://maps.googleapis.com/maps/api/js?key=AIzaSyDuwWq60IrpVvV1uNd-1IvOmlAZ2tAGAM8&callback=initMap";
            ViewBag.Games = new SelectList(db.Games.Select(x => x.Name), "Name");
            var searchResults = db.Events.Where(x => x.EventName.Contains(search));
            if (UserLat != null && UserLong != null)
            {
                searchResults = proximityFilter(searchResults, UserLat, UserLong).AsQueryable();
            }
            return View(searchResults);
        }

        public ActionResult SearchBox(string Games, double? UserLat, double? UserLong)
        {
            //string gameName = data.Name;
            ViewBag.ApiUrl = "https://maps.googleapis.com/maps/api/js?key=AIzaSyDuwWq60IrpVvV1uNd-1IvOmlAZ2tAGAM8&callback=initMap";
            ViewBag.Games = new SelectList(db.Games.Select(x => x.Name), "Name");
            var searchResults = db.EventGames.Where(x => x.Game.Name.Contains(Games));
            if (UserLat != null && UserLong != null)
            {
                //searchResults = proximityFilter(searchResults, UserLat, UserLong).AsQueryable();
            }
            return View(searchResults);
        }

        private IEnumerable<Event> proximityFilter(IQueryable<Event> events, double? userLat, double? userLong)
        {
            IEnumerable<Event> filteredEvents = new List<Event>().AsEnumerable();
            IEnumerable<Event> eventsWithNoLocData = new List<Event>().AsEnumerable();
            foreach (Event @event in events)
            {
                double dist = distanceToUser(@event, userLat, userLong);

                if(dist < 0)
                {
                    eventsWithNoLocData = eventsWithNoLocData.Append(@event);     //save events with NULL for their lat/long
                }

                else if(dist < 50)
                {
                    filteredEvents = filteredEvents.Append(@event);      //Filter out events farther away than 50km
                }
                
            }

            return filteredEvents.Concat(eventsWithNoLocData);   //return neaby events followed by null-data events
        }

        private double distanceToUser(Event @event, double? userLat, double? userLong)
        {
            if (@event.EventLat == null || @event.EventLong == null)    //double check that the event actually has location data
            {
                return -1;
            }

            int radius = 6371; // Radius of the earth in km

            double eradLat = (double)@event.EventLat * (Math.PI/180) ;  // Convert to radians
            double eradLong = (double)@event.EventLong * (Math.PI / 180);
            double uradLat = (double)userLat * (Math.PI / 180);
            double uradLong = (double)userLong * (Math.PI / 180);

            //Haversine formula to calculate great-circle distance: https://en.wikipedia.org/wiki/Haversine_formula
            double sqrtContents = haversine(uradLat - eradLat) + Math.Cos(eradLat) * Math.Cos(uradLat) * haversine(uradLong - eradLong);

            double dist = 2 * radius * Math.Asin(Math.Sqrt(sqrtContents));
            return dist;
        }

        private double haversine(double theta)
        {
            return (1 - Math.Cos(theta)) / 2;
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

            
            //  DEPLOYED --->   ViewBag.ApiUrl = "https://maps.googleapis.com/maps/api/js?key=" + System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleAPIKey"].ToString() + "&callback=initMap";
            //  LOCAL --->      ViewBag.ApiUrl = "https://maps.googleapis.com/maps/api/js?key=AIzaSyDuwWq60IrpVvV1uNd-1IvOmlAZ2tAGAM8&callback=initMap";
            ViewBag.ApiUrl = "https://maps.googleapis.com/maps/api/js?key=AIzaSyDuwWq60IrpVvV1uNd-1IvOmlAZ2tAGAM8&callback=initMap";
            

            return View();
        }

        public void CheckLoginStatus(string currentID, AspNetUser currentUser)
        {
            if (currentID == null || currentUser.EmailConfirmed == false)
            {
                ModelState.AddModelError("HostID", "Plese login before creating an event!");
            }
            else if (currentUser.EmailConfirmed == false)
            {
                ModelState.AddModelError("HostID", "Plese confirm your email before creating an event!");
            }
        }

        public void ConvertAddressToCoords(Event currentEvent)
        {
            //Convert to Coords
            var address = currentEvent.EventLocation;

            
            //  DEPLOYED --->   apikey: System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleAPIKey"].ToString() 
            //  LOCAL --->      apikey: "AIzaSyDuwWq60IrpVvV1uNd-1IvOmlAZ2tAGAM8"
            var locServ = new GoogleLocationService(apikey: "AIzaSyDuwWq60IrpVvV1uNd-1IvOmlAZ2tAGAM8");
            
            
            Console.WriteLine("Converting to point");
            var point = locServ.GetLatLongFromAddress(address);
            Console.WriteLine(point.ToString());
            if(point != null)
            {
                currentEvent.EventLat = (float)point.Latitude;
                currentEvent.EventLong = (float)point.Longitude;
            }
            //ModelState.AddModelError("Event Address", "The address you have netered is invalid!");
        }

        public void CreateNewID(Event currentEvent)
        {
            Guid g = Guid.NewGuid();
            string gIDString = Convert.ToBase64String(g.ToByteArray());
            gIDString = gIDString.Replace("=", "");
            gIDString = gIDString.Replace("+", "");
            gIDString = gIDString.Replace("/", "");
            currentEvent.ID = gIDString;
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,EventName,IsPublic,Date,EventDescription,EventLocation,PlayerSlotsMin,PlayerSlotsMax,PlayersCount,UnsupGames,HostID")] Event @event)
        {
            var currentID = User.Identity.GetUserId();
            AspNetUser currentUser = db.AspNetUsers.Find(currentID);
            User TheUser = db.Users.Find(currentID);
           
            //Check If user is logged in, Convert address to lat log, Create a new ID and attach to event
            //CheckLoginStatus(currentID, currentUser);
            ConvertAddressToCoords(@event);
            CreateNewID(@event);

            @event.HostID = currentID;
            @event.PlayersCount = 1;

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
