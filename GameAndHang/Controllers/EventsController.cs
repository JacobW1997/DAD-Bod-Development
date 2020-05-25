using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
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
using Newtonsoft.Json.Linq;

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
            ViewBag.ApiUrl = "https://maps.googleapis.com/maps/api/js?key=" + System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleAPIKey"].ToString() + "&callback=initMap";
            ViewBag.Games = new SelectList(db.Games.Select(x => x.Name), "Name");

            var searchResults = db.Events.Where(x => x.EventName.Contains(search));
            var markerNames = new string[searchResults.Count()];
            var markerLats = new double[searchResults.Count()];
            var markerLongs = new double[searchResults.Count()];

            if (UserLat != null && UserLong != null)
            {
                searchResults = proximityFilter(searchResults, UserLat, UserLong).AsQueryable();
            }

            var temp = searchResults.ToList();

            for (int i = 0; i < searchResults.Count(); i++)
            {
                if (temp[i].EventLat.HasValue && temp[i].EventLong.HasValue)
                {
                    markerNames[i] = temp[i].EventName;
                    markerLats[i] = (double)temp[i].EventLat;
                    markerLongs[i] = (double)temp[i].EventLong; 
                }
            }

            ViewBag.MarkerNames = markerNames;
            ViewBag.MarkerLats = markerLats;
            ViewBag.MarkerLongs = markerLongs;

            return View(searchResults);
        }

        public ActionResult SearchBox(string Games, double? UserLat, double? UserLong)
        {
            //string gameName = data.Name;
            ViewBag.ApiUrl = "https://maps.googleapis.com/maps/api/js?key=" + System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleAPIKey"].ToString() + "&callback=initMap";
            ViewBag.Games = new SelectList(db.Games.Select(x => x.Name), "Name");
            var EGSearchResults = db.EventGames.Where(x => x.Game.Name.Contains(Games));
            var searchResults = db.Events.Where(s => s.EventGames.Intersect(EGSearchResults).Count() > 0);
            if (UserLat != null && UserLong != null)
            {
                searchResults = proximityFilter(searchResults, UserLat, UserLong).AsQueryable();
                
            }
            return View(searchResults);
        }

        private IEnumerable<Event> proximityFilter(IQueryable<Event> events, double? userLat, double? userLong)
        {

            if ( userLat == null || userLong == null ){
                return events;
            }

            IEnumerable<Event> filteredEvents = new List<Event>().AsEnumerable();
            IEnumerable<Event> eventsWithNoLocData = new List<Event>().AsEnumerable();
            foreach (Event @event in events)
            {
                double dist = distanceToUser(@event, userLat, userLong);

                if(dist < 0)
                {
                    //eventsWithNoLocData = eventsWithNoLocData.Append(@event);     //save events with NULL for their lat/long
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
            ViewBag.ConfirmedFriends = "1";
            ViewBag.ConfirmedFriends = CheckHostRelationships(@event.HostID);
            
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

            ViewBag.ApiUrl = "https://maps.googleapis.com/maps/api/js?key=" + System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleAPIKey"].ToString() + "&callback=initMap";

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
            //  LOCAL --->      apikey: ""
            var locServ = new GoogleLocationService(apikey: System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleAPIKey"].ToString());
            
            

            Console.WriteLine("Converting to point");
            var point = locServ.GetLatLongFromAddress(address);
            Console.WriteLine(point.ToString());
            if (point != null)
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

            var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                await db.SaveChangesAsync();
                return RedirectToAction("Codex", "Home");
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
            if(@event.HostID != User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Home");
            }
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
            if (@event.HostID != User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Home");
            }
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

        public ActionResult APIGame(string id)
        {
            GetGame(id);

            if (User.Identity.GetUserId() == null)
            {
                return RedirectToAction("Register", "Account");
            }
            var getuserID = User.Identity.GetUserId();
            string currentUserID = db.Users
                .Where(x => x.ID == getuserID)
                .Select(x => x.ID)
                .Single();

            User currentUser = db.Users
                .Where(x => x.ID == currentUserID)
                .Select(x => x)
                .Single();

            IQueryable<Event> gameList = db.Events
                .Where(x => x.HostID == currentUser.ID)
                .Select(x => x);

            ViewBag.EventID = new SelectList(gameList, "ID", "EventName");
            ViewBag.GameID = id;

            return View();
        }
        public void GetGame(string id)
        {
            string cred = System.Web.Configuration.WebConfigurationManager.AppSettings["AtlasKey"];
            string URL = "https://www.boardgameatlas.com/api/search?ids=" + id + "&client_id=" + System.Web.Configuration.WebConfigurationManager.AppSettings["AtlasKey"];
            Debug.WriteLine(URL);
            string allData = SendRequest(URL);

            JObject rootObj = JObject.Parse(allData);
            Debug.WriteLine(allData);

            List<string> outputIDs = new List<string>();
            List<string> outputNames = new List<string>();
            List<string> outputThumbUrls = new List<string>();
            List<string> YearPublishedList = new List<string>();
            List<string> min_playersList = new List<string>();
            List<string> max_playersList = new List<string>();
            List<string> min_playtimeList = new List<string>();
            List<string> max_playtimeList = new List<string>();
            List<string> descriptionList = new List<string>();
            List<string> description_previewList = new List<string>();
            List<string> ageList = new List<string>();
            List<string> reddit_week_countList = new List<string>();
            List<string> categoriesList = new List<string>();
            List<string> image_urlList = new List<string>();
            List<string> priceList = new List<string>();
            List<string> urlList = new List<string>();
            List<string> avgUsrRatingList = new List<string>();



            for (int i = 0; i < 1; i++)
            {

                var getNames = (string)rootObj.SelectToken("games[" + i + "].name");
                outputNames.Add(getNames);

                var getThumbUrls = (string)rootObj.SelectToken("games[" + i + "].thumb_url");
                outputThumbUrls.Add(getThumbUrls);

                var getDescriptionPrev = (string)rootObj.SelectToken("games[" + i + "].description_preview");
                description_previewList.Add(getDescriptionPrev);

                var getCount = (string)rootObj.SelectToken("games[" + i + "].reddit_all_time_count");
                reddit_week_countList.Add(getCount);

                var getCategories = rootObj.SelectToken("games[" + i + "].categories");
                if (getCategories != null)
                {
                    categoriesList.Add(getCategories.ToString());
                }

                var getMinPlayers = (string)rootObj.SelectToken("games[" + i + "].min_players");
                min_playersList.Add(getMinPlayers);

                var getMaxPlayers = (string)rootObj.SelectToken("games[" + i + "].max_players");
                max_playersList.Add(getMaxPlayers);

                var getAvgRating = (string)rootObj.SelectToken("games[" + i + "].average_user_rating");
                if (getAvgRating != null)
                {
                    avgUsrRatingList.Add(getAvgRating.Substring(0, 3));
                }

            }

            var JsonData = new
            {
                //id = getID,
                name = outputNames,
                //year_published = getYearPublished,
                min_players = min_playersList,
                max_players = max_playersList,
                //min_playtime = getMinPlayTime,
                //max_playtime = getMaxPlayTime,
                //description = getDescription,
                description_preview = description_previewList,
                average_user_rating = avgUsrRatingList,
                //age = getAge,
                reddit_all_time_count = reddit_week_countList,
                categories = categoriesList,
                thumb_url = outputThumbUrls,
                //image_url = getGameImage,
                //price = getPrice,
                //url = getGameUrl,
            };
            ViewBag.name = JsonData.name[0].ToString();
            ViewBag.min_players = JsonData.min_players[0].ToString();
            ViewBag.max_players = JsonData.max_players[0].ToString();
            ViewBag.description = JsonData.description_preview[0].ToString();
            ViewBag.average_rating = JsonData.average_user_rating[0].ToString();
            ViewBag.reddit_all_time = JsonData.reddit_all_time_count[0].ToString();
            ViewBag.categories = JsonData.categories[0].ToString();
            ViewBag.thumb = JsonData.thumb_url[0].ToString();

        }
        private string SendRequest(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Accept = "application/json";

            string jsonString = null;
            // TODO: You should handle exceptions here
            using (WebResponse response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                jsonString = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }

            return jsonString;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public List<User> CheckHostRelationships(string id)
        {

            List<User> ConfirmedFriends = new List<User>();
            List<string> confirmedFriendsIDs = (from b in db.Relationships where b.UserFirstID == id | b.UserSecondID == id && b.Type == 1 select b.UserFirstID).ToList();
            List<string> moreConfirmedFriendIDs = (from b in db.Relationships where b.UserFirstID == id | b.UserSecondID == id && b.Type == 1 select b.UserSecondID).ToList();


            foreach (string item in confirmedFriendsIDs)
            {
                ConfirmedFriends.Add(db.Users.Find(item));

            }
            foreach (string item in moreConfirmedFriendIDs)
            {
                ConfirmedFriends.Add(db.Users.Find(item));
            }
            foreach (var item in ConfirmedFriends.ToList())
            {
                if (item.ID == id)
                {
                    ConfirmedFriends.Remove(item);
                }

            }
            return (ConfirmedFriends);
        }

    }
}
