using GameAndHang.DAL;
using GameAndHang.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static GameAndHang.Models.API;

namespace GameAndHang.Controllers
{
    public class APIEventGameController : Controller
    {
        private GnHContext db = new GnHContext();
        // GET: APIEventGame
        public ActionResult Index()
        {
            return View();
        }

        //Gets the details for a specific game in the Codex
        public ActionResult APIGame(string id)
        {
            GetGame(id);
            ViewBag.checkGames = 1; //Bool to check later on
            if (User.Identity.GetUserId() == null)
            {
                return RedirectToAction("Register", "Account");
            }
            var getuserID = User.Identity.GetUserId();

            User currentUser = db.Users //Get current user object
                .Where(x => x.ID == getuserID)
                .Select(x => x)
                .Single();

            IQueryable<Event> gameList = db.Events //Get all of the hosts events to display in a drop down
                .Where(x => x.HostID == currentUser.ID)
                .Select(x => x)
                ;

            if (!gameList.Any())
            {
                ViewBag.checkGames = 0; // Dont display anything on the view
            }

            ViewBag.EventID = new SelectList(gameList, "ID", "EventName");
            ViewBag.GameID = id;
            
            return View();
        }
        //Gets a specific game from the Atlas API
        public void GetGame(string id)
        {
            string URL = "https://www.boardgameatlas.com/api/search?ids=" + id + "&client_id=" + "xv4UwTJIGG";
            Debug.WriteLine(URL);
            string allData = SendRequest(URL);

            JObject rootObj = JObject.Parse(allData);
            Debug.WriteLine(allData);

            List<string> outputNames = new List<string>();
            List<string> outputThumbUrls = new List<string>();
            List<string> min_playersList = new List<string>();
            List<string> max_playersList = new List<string>();
            List<string> description_previewList = new List<string>();
            List<string> reddit_week_countList = new List<string>();
            List<string> categoriesList = new List<string>();
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
                if (getCategories != null){
                categoriesList.Add(getCategories.ToString());
                }
                
                var getMinPlayers = (string)rootObj.SelectToken("games[" + i + "].min_players");
                min_playersList.Add(getMinPlayers);
                var getMaxPlayers = (string)rootObj.SelectToken("games[" + i + "].max_players");
                max_playersList.Add(getMaxPlayers);
                var getAvgRating = (string)rootObj.SelectToken("games[" + i + "].average_user_rating");
                
                if(getAvgRating != null)
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
        //Send request to Atlas API
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

        //Create a new APIEventGame
        public ActionResult Create(string id)
        {
            if (User.Identity.GetUserId() == null)
            {
                return RedirectToAction("Register", "Account");
            }
            var getuserID = User.Identity.GetUserId();
            User currentUser = db.Users
                .Where(x => x.ID == getuserID)
                .Select(x => x)
                .Single();

            IQueryable<Event> gameList = db.Events
                .Where(x => x.HostID == currentUser.ID)
                .Select(x => x)
                ;

            if (!gameList.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            string cred = System.Web.Configuration.WebConfigurationManager.AppSettings["AtlasKey"];
            string URL = "https://www.boardgameatlas.com/api/search?ids=" +id+ "%26client_id=" + cred;
            Debug.WriteLine(URL);
            string SelectedGame = SendRequest(URL);
            Debug.WriteLine(SelectedGame.ToString());
            ViewBag.EventID = gameList;
            return View();
        }

        // POST: APIEventGames/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult APIGame([Bind(Include = "ID,EventID,GameID,GameName")] APIEventGame aPIEventGame)
        {
            aPIEventGame.Event = db.Events.Find(aPIEventGame.EventID);
            aPIEventGame.GameID.ToString();
            db.APIEventGames.Add(aPIEventGame);
            db.SaveChanges();
            return RedirectToAction("Details", "Events", new { id= aPIEventGame.EventID});
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            APIEventGame aPIEventGame = db.APIEventGames.Find(id);
            if (aPIEventGame == null)
            {
                return HttpNotFound();
            }
            return View(aPIEventGame);
        }

        // POST: APIEventGames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            APIEventGame aPIEventGame = db.APIEventGames.Find(id);
            db.APIEventGames.Remove(aPIEventGame);
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