using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DBD_Swim_Tracker_0._2.DAL;
using DBD_Swim_Tracker_0._2.Models;

namespace DBD_Swim_Tracker_0._2.Views
{
    public class TeamRostersController : Controller
    {
        private SwimTrackerContext db = new SwimTrackerContext();

        // GET: TeamRosters
        public ActionResult Index()
        {
            var teamRosters = db.TeamRosters.Include(t => t.Athlete).Include(t => t.Team);
            return View(teamRosters.ToList());
        }

        // GET: TeamRosters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamRoster teamRoster = db.TeamRosters.Find(id);
            if (teamRoster == null)
            {
                return HttpNotFound();
            }
            return View(teamRoster);
        }

        // GET: TeamRosters/Create
        public ActionResult Create()
        {
            ViewBag.ATHLETEID = new SelectList(db.Athletes, "ID", "NAME");
            ViewBag.TEAMID = new SelectList(db.Teams, "ID", "NAME");
            return View();
        }

        // POST: TeamRosters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TEAMID,ATHLETEID")] TeamRoster teamRoster)
        {
            if (ModelState.IsValid)
            {
                db.TeamRosters.Add(teamRoster);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ATHLETEID = new SelectList(db.Athletes, "ID", "NAME", teamRoster.ATHLETEID);
            ViewBag.TEAMID = new SelectList(db.Teams, "ID", "NAME", teamRoster.TEAMID);
            return View(teamRoster);
        }

        // GET: TeamRosters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamRoster teamRoster = db.TeamRosters.Find(id);
            if (teamRoster == null)
            {
                return HttpNotFound();
            }
            ViewBag.ATHLETEID = new SelectList(db.Athletes, "ID", "NAME", teamRoster.ATHLETEID);
            ViewBag.TEAMID = new SelectList(db.Teams, "ID", "NAME", teamRoster.TEAMID);
            return View(teamRoster);
        }

        // POST: TeamRosters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TEAMID,ATHLETEID")] TeamRoster teamRoster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teamRoster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ATHLETEID = new SelectList(db.Athletes, "ID", "NAME", teamRoster.ATHLETEID);
            ViewBag.TEAMID = new SelectList(db.Teams, "ID", "NAME", teamRoster.TEAMID);
            return View(teamRoster);
        }

        // GET: TeamRosters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamRoster teamRoster = db.TeamRosters.Find(id);
            if (teamRoster == null)
            {
                return HttpNotFound();
            }
            return View(teamRoster);
        }

        // POST: TeamRosters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TeamRoster teamRoster = db.TeamRosters.Find(id);
            db.TeamRosters.Remove(teamRoster);
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
