using DBD_Swim_Tracker_0._2.Models;
using DBD_Swim_Tracker_0._2.Models.ViewModels;
using DBD_Swim_Tracker_0._2.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace DBD_Swim_Tracker_0._2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        private SwimTrackerContext db = new SwimTrackerContext();
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //Search by Athlete name using POST
        [HttpPost]
        public ActionResult Index(string searchString)
        {
            //Double check that a search string was entered
            if (String.IsNullOrEmpty(searchString))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Filter stock items by name according to search string
            IQueryable<Athlete> athletes = db.Athletes;
            athletes = athletes.Where(p => p.NAME.Contains(searchString));

            //Return filtered results
            return View(athletes.ToList());
        }

        //GET: Athlete Details by ID
        public ActionResult AthleteDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Athlete athlete = db.Athletes.Find(id);
            if (athlete == null) { return HttpNotFound(); }

            AthleteDetailsViewModel viewModel = new AthleteDetailsViewModel(athlete);
            return View(viewModel);
        }
    }
}