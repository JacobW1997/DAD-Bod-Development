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
using reCAPTCHA.MVC;
using GameAndHang.UtilityFunctions;
using System.Configuration;

namespace GameAndHang.Controllers
{

    public class EventsController : Controller
    {
        private GnHContext db = new GnHContext();
        //Checks to see if an event is in the future or if it has passed
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

        // GET: Events
        public  ActionResult Index()
        {
            var events = db.Events.Include(r => r.User);
            var filtered = events.Where(NeededEvent);
            return View(filtered);
        }

        //Search the Events in the database based off hame name
        public ActionResult Search(string search, double? UserLat, double? UserLong)
        {
            ViewBag.ApiUrl = "https://maps.googleapis.com/maps/api/js?key=" + System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleAPIKey"].ToString() + "&callback=initMap";
            ViewBag.Games = new SelectList(db.Games.Select(x => x.Name), "Name");

            var searchResults = db.Events.Where(x => x.EventName.Contains(search));
            var markerNames = new string[searchResults.Count()];
            var markerLats = new double[searchResults.Count()];
            var markerLongs = new double[searchResults.Count()];
            var temp = searchResults.ToList();

            if (UserLat != null && UserLong != null)
            {
                searchResults = proximityFilter(searchResults, UserLat, UserLong).AsQueryable();
            }

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

        //Searches database for events based off unsupported games 
        public ActionResult SearchBox(string games, double? UserLat, double? UserLong)
        {
            ViewBag.ApiUrl = "https://maps.googleapis.com/maps/api/js?key=" + System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleAPIKey"].ToString() + "&callback=initMap";
            ViewBag.Games = new SelectList(db.Games.Select(x => x.Name), "Name");
            var EGSearchResults = db.APIEventGames.Where(x => x.GameName.Contains(games));
            var searchResults = db.Events.Where(s => s.APIEventGames.Intersect(EGSearchResults).Count() > 0);
            if (UserLat != null && UserLong != null)
            {
                searchResults = proximityFilter(searchResults, UserLat, UserLong).AsQueryable();
            }
            return View(searchResults);
        }

        //Filters events based off proximity of 30 miles
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

        //Determine event distance from user
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
        
        //Haversine formula to calculate great-circle distance: https://en.wikipedia.org/wiki/Haversine_formula
        private double haversine(double theta)
        {
            return (1 - Math.Cos(theta)) / 2;
        }

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
            //Check to see if users are already friends or not, this is so only friends can join private events
            ViewBag.ConfirmedFriends = "1";
            ViewBag.ConfirmedFriends = CheckHostRelationships(@event.HostID);
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            ViewBag.HostID = User.Identity.GetUserId();
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
            var locServ = new GoogleLocationService(apikey: System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleAPIKey"].ToString());
            Console.WriteLine("Converting to point");
            var point = locServ.GetLatLongFromAddress(address);
            Console.WriteLine(point.ToString());
            if (point != null)
            {
                currentEvent.EventLat = (float)point.Latitude;
                currentEvent.EventLong = (float)point.Longitude;
            }
            //ModelState.AddModelError("Event Address", "The address you have entered is invalid!");
        }

        //Creates a new, random ID for an event
        public void CreateNewID(Event currentEvent)
        {
            Guid g = Guid.NewGuid();
            string gIDString = Convert.ToBase64String(g.ToByteArray());
            gIDString = gIDString.Replace("=", "");
            gIDString = gIDString.Replace("+", "");
            gIDString = gIDString.Replace("/", "");
            currentEvent.ID = gIDString;
        }

        public readonly string CapKey = System.Web.Configuration.WebConfigurationManager.AppSettings["ReCaptchaPrivateKey"].ToString();
       
        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CaptchaValidator(
            ErrorMessage = "Captcha Invalid!",
            RequiredMessage = "Captcha input is required!"
            )]
        public async Task<ActionResult> Create([Bind(Include = "EventName,IsPublic,Date,EventDescription,EventLocation,PlayerSlotsMin,PlayerSlotsMax,PlayersCount,UnsupGames")] Event @event)
        {
            
            var currentID = User.Identity.GetUserId();
            AspNetUser currentUser = db.AspNetUsers.Find(currentID);
            User TheUser = db.Users.Find(currentID);
            @event.HostID = currentID; //Set the host
            @event.PlayersCount = 1; //Default value for players count

            //CheckLoginStatus(currentID, currentUser); //Check if user is logged in and verfied
            ConvertAddressToCoords(@event); //Convert provided address to coords
            CreateNewID(@event); //Create a new database ID for the event 

            if (TheUser != null)
            {
                TheUser.HostXP += 20;
                db.Entry(TheUser).State = EntityState.Modified; //Add xp to host account for creating new event and save
                db.SaveChanges();
            }

            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                await db.SaveChangesAsync();
                return RedirectToAction("EventCreatedNotice");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "EventName,IsPublic,Date,EventDescription,EventLocation,PlayerSlotsMin,PlayerSlotsMax,UnsupGames")] Event @event)
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

        //GET APIGame - Adds a game to an event from the game datails page
        public ActionResult APIGame(string id)
        {
            GetGame(id);
            if (User.Identity.GetUserId() == null)
            {
                return RedirectToAction("Register", "Account");
            }
            var getuserID = User.Identity.GetUserId();

            //Get the current user Object from the database
            User currentUser = db.Users
                .Where(x => x.ID == getuserID)
                .Select(x => x)
                .Single();
            //Get all events for the user
            IQueryable<Event> gameList = db.Events
                .Where(x => x.HostID == currentUser.ID)
                .Select(x => x);

            ViewBag.EventID = new SelectList(gameList, "ID", "EventName");
            ViewBag.GameID = id;
            return View();
        }
        //Gets a specific game from the Atlas API
        public void GetGame(string id) 
        {
            string URL = "https://www.boardgameatlas.com/api/search?ids=" + id + "&client_id=" + System.Web.Configuration.WebConfigurationManager.AppSettings["AtlasKey"];
            Debug.WriteLine(URL);
            string allData = SendRequest(URL);
            JObject rootObj = JObject.Parse(allData);
            Debug.WriteLine(allData);

            //Lists to add game infor to
            List<string> outputNames = new List<string>();
            List<string> outputThumbUrls = new List<string>();
            List<string> min_playersList = new List<string>();
            List<string> max_playersList = new List<string>();
            List<string> description_previewList = new List<string>();
            List<string> reddit_week_countList = new List<string>();
            List<string> categoriesList = new List<string>();
            List<string> avgUsrRatingList = new List<string>();

            //Parse data
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
                name = outputNames,
                min_players = min_playersList,
                max_players = max_playersList,
                description_preview = description_previewList,
                average_user_rating = avgUsrRatingList,
                reddit_all_time_count = reddit_week_countList,
                categories = categoriesList,
                thumb_url = outputThumbUrls,
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
        //Send request to the Atlas API
        private string SendRequest(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Accept = "application/json";
            string jsonString = null;
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

        //Chack host relationships, friends will be able to join private events
        public List<User> CheckHostRelationships(string id)
        {
            List<User> ConfirmedFriends = new List<User>();
            List<string> confirmedFriendsIDs = (from b in db.Relationships where b.UserFirstID == id | b.UserSecondID == id && b.Type == 1 select b.UserFirstID).ToList(); //User could have sent friend request
            List<string> moreConfirmedFriendIDs = (from b in db.Relationships where b.UserFirstID == id | b.UserSecondID == id && b.Type == 1 select b.UserSecondID).ToList(); //User could have been requested
            
            //Each loops add each list to a master list in order
            foreach (string item in confirmedFriendsIDs)
            {
                ConfirmedFriends.Add(db.Users.Find(item));
            }
            foreach (string item in moreConfirmedFriendIDs)
            {
                ConfirmedFriends.Add(db.Users.Find(item));
            }
            //If the current users ID is present then remove it
            foreach (var item in ConfirmedFriends.ToList())
            {
                if (item.ID == id)
                {
                    ConfirmedFriends.Remove(item);
                }

            }
            return (ConfirmedFriends);
        }
        //Redirect to a page after event has been created
        public ActionResult EventCreatedNotice()
        {
            return View();
        }

    }
}
