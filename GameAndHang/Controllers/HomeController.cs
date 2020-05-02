using Newtonsoft.Json.Linq;
using System;
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

namespace GameAndHang.Controllers
{
    public class HomeController : Controller
    {
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
            return View();
        }
        public ActionResult GetGames()
        {           
            //string cred = System.Web.Configuration.WebConfigurationManager.AppSettings["AtlasKey"];
            string URL = "https://www.boardgameatlas.com/api/search?order_by=popularity&limit=20&client_id=xv4UwTJIGG";
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



            for (int i = 0; i< 20; i++) { 

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
                //id = getID,
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
    }
}