using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GameAndHang.Models;
using static GameAndHang.Models.API;
using System.Threading.Tasks;
using GameAndHang.DAL;

namespace GameAndHang.Controllers
{
    public class HomeController : Controller
    {
    private GnHContext db = new GnHContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Codex()
        {
            GetPopularSiteGames();
            return View();
        }
        public ActionResult GetGames()
        {           
            string cred = System.Web.Configuration.WebConfigurationManager.AppSettings["AtlasKey"];
            string URL = "https://www.boardgameatlas.com/api/search?order_by=popularity&limit=40&client_id=" + System.Web.Configuration.WebConfigurationManager.AppSettings["AtlasKey"];
            Debug.WriteLine(URL);
            var allData = SendRequest(URL);
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



            for (int i = 0; i< 40; i++) {
                var getIDs = (string)rootObj.SelectToken("games[" + i + "].id");
                outputIDs.Add(getIDs);
                var getNames = (string)rootObj.SelectToken("games["+i+"].name");
                outputNames.Add(getNames);
                var getThumbUrls = (string)rootObj.SelectToken("games["+i+"].thumb_url");
                outputThumbUrls.Add(getThumbUrls);
                var getDescriptionPrev = (string)rootObj.SelectToken("games[" + i + "].description_preview");
                description_previewList.Add(getDescriptionPrev);
                var getCount = (string)rootObj.SelectToken("games[" + i + "].reddit_all_time_count");
                reddit_week_countList.Add(getCount);
                var getCategories = rootObj.SelectToken("games[" + i + "].categories");
                categoriesList.Add(getCategories.ToString());
                var getMinPlayers = (string)rootObj.SelectToken("games[" + i + "].min_players");
                min_playersList.Add(getMinPlayers);
                var getMaxPlayers = (string)rootObj.SelectToken("games[" + i + "].max_players");
                max_playersList.Add(getMaxPlayers);
                var getAvgRating = (string)rootObj.SelectToken("games[" + i + "].average_user_rating");
                avgUsrRatingList.Add(getAvgRating.Substring(0,3));
            }

            
            var JsonData = new
            {
                id = outputIDs,
                name = outputNames,
                //year_published = getYearPublished,
                min_players = min_playersList,
                max_players = max_playersList,
                //min_playtime = getMinPlayTime,
                //max_playtime = getMaxPlayTime,
                //description = getDescription,
                description_preview =description_previewList ,
                average_user_rating= avgUsrRatingList,
                //age = getAge,
                reddit_all_time_count = reddit_week_countList,
                categories = categoriesList,
                thumb_url = outputThumbUrls,
                //image_url = getGameImage,
                //price = getPrice,
                //url = getGameUrl,
            };
            Debug.WriteLine(JsonData.ToString());
            return Json(JsonData, JsonRequestBehavior.AllowGet);
        }

        public class customGame
        {
            public string id { get; set; }
            public string name { get; set; }

            public string min_players { get; set; }
            public string max_players { get; set; }

            public string description_preview { get; set; }
            public string average_user_rating { get; set; }

            public string reddit_all_time_count { get; set; }

            public string thumb_url { get; set; }

            public double score { get; set; }
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
        public class custom
        {
            public double Score { get; set; }
            public string gameID { get; set; }
        }
        public ActionResult GetPopularSiteGames()
        {
            //Gets all GAME IDs from APIGAMES and counts their occurances this is the base score of the game 
            List<customGame> allAPIGameIDs = (from b in db.APIEventGames
                                        group b.GameID by b.GameID into g
                                        orderby g.Count() descending
                                        select new customGame { id=g.Key, score= (g.Count() + 1) *1.2 }).ToList();

            //Join Events with APIEventGames so we can count the amount of players associated with the game
            var JoinEventsAPIEvents = (from b in db.Events
                         join product in db.APIEventGames on b.ID equals product.EventID
                         select product);

            //Using our joined table, count the players for each event, if the event have more then 1 EventPlayer then select it and output it
            //Contains a list of EventPlayers count, GameID and EventID
            List<custom> countPlayers = (from b in JoinEventsAPIEvents
                        where b.Event.EventPlayers.Count > 1
                        select new custom { Score = b.Event.EventPlayers.Count, gameID = b.GameID}).ToList();

            //Add the score from allAPIGameIDs to each associated game in our data list
            //Adds one the the score of each game for each associated EventPlayer and multiplies the overall value by 1.8
            for(int i = 0; i != countPlayers.Count(); i++)
            {
                customGame current = allAPIGameIDs.Find(d => d.id == countPlayers.ElementAt(i).gameID);
                current.score += (countPlayers.ElementAt(i).Score + 1) * 1.4;    
            }
            //Gets the rest of the information required to display the game and details 
            GetGamesFromAPI(allAPIGameIDs);

            //Sort all elements by their score inplace
            allAPIGameIDs.Sort((x, y) => y.score.CompareTo(x.score));

            ViewBag.Games = allAPIGameIDs;

            return View();
        }

        public void GetGamesFromAPI(List<customGame> GameIDsWithScore)
        {
            foreach(customGame scoredgame in GameIDsWithScore)
            {
                GetGame(scoredgame);
            }
        }
        

        public customGame GetGame(customGame currentGame)
        {
            string cred = System.Web.Configuration.WebConfigurationManager.AppSettings["AtlasKey"];
            string URL = "https://www.boardgameatlas.com/api/search?ids=" + currentGame.id + "&client_id=" + System.Web.Configuration.WebConfigurationManager.AppSettings["AtlasKey"];
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
                var getIDs = (string)rootObj.SelectToken("games[" + i + "].id");
                outputIDs.Add(getIDs);

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

                currentGame.id = outputIDs.ElementAt(i);
                currentGame.name = outputNames.ElementAt(i);
                currentGame.min_players = min_playersList.ElementAt(i);
                currentGame.max_players = max_playersList.ElementAt(i);
                currentGame.description_preview = description_previewList.ElementAt(i);
                currentGame.average_user_rating = avgUsrRatingList.ElementAt(i);
                currentGame.reddit_all_time_count = reddit_week_countList.ElementAt(i);
                currentGame.thumb_url = outputThumbUrls.ElementAt(i);

            }

            return currentGame;
        }

        public ActionResult GameSearch(string searchString)
        {
            string cred = System.Web.Configuration.WebConfigurationManager.AppSettings["AtlasKey"];
            string URL = "https://www.boardgameatlas.com/api/search?name=" + searchString + "&fuzzy_match=true&order_by=popularity&limit=40&client_id=" + System.Web.Configuration.WebConfigurationManager.AppSettings["AtlasKey"];
            Debug.WriteLine(URL);
            var allData = SendRequest(URL);
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



            for (int i = 0; i < 40; i++)
            {
                var getIDs = (string)rootObj.SelectToken("games[" + i + "].id");
                outputIDs.Add(getIDs);
                var getNames = (string)rootObj.SelectToken("games[" + i + "].name");
                outputNames.Add(getNames);
                var getThumbUrls = (string)rootObj.SelectToken("games[" + i + "].thumb_url");
                outputThumbUrls.Add(getThumbUrls);
                var getDescriptionPrev = (string)rootObj.SelectToken("games[" + i + "].description_preview");
                description_previewList.Add(getDescriptionPrev);
                var getCount = (string)rootObj.SelectToken("games[" + i + "].reddit_all_time_count");
                reddit_week_countList.Add(getCount);
                var getCategories = rootObj.SelectToken("games[" + i + "].categories");
                if (getCategories != null) { 
                    categoriesList.Add(getCategories.ToString());
                }
                var getMinPlayers = (string)rootObj.SelectToken("games[" + i + "].min_players");
                min_playersList.Add(getMinPlayers);
                var getMaxPlayers = (string)rootObj.SelectToken("games[" + i + "].max_players");
                max_playersList.Add(getMaxPlayers);
                var getAvgRating = (string)rootObj.SelectToken("games[" + i + "].average_user_rating");
                avgUsrRatingList.Add(getAvgRating/*.Substring(0, 3)*/);
            }


            var JsonData = new
            {
                id = outputIDs,
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
            Debug.WriteLine(JsonData.ToString());
            return Json(JsonData, JsonRequestBehavior.AllowGet);
        }

    }
}