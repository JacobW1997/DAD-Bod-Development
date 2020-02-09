﻿using System;
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
    public class ResultsController : Controller
    {
        private SwimTrackerContext db = new SwimTrackerContext();

        // GET: Results
        public ActionResult Index()
        {
            var results = db.Results.Include(r => r.Athlete).Include(r => r.Event).Include(r => r.Meet);
            return View(results.ToList());
        }

        // GET: Results/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Result result = db.Results.Find(id);
            if (result == null)
            {
                return HttpNotFound();
            }
            return View(result);
        }

        // GET: Results/Create
        public ActionResult Create()
        {
            ViewBag.ATHLETEID = new SelectList(db.Athletes, "ID", "NAME");
            ViewBag.EVENTID = new SelectList(db.Events, "ID", "STROKE");
            ViewBag.MEETID = new SelectList(db.Meets, "ID", "LOCATION");
            return View();
        }

        // POST: Results/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EVENTID,ATHLETEID,MEETID,TIME")] Result result)
        {
            if (ModelState.IsValid)
            {
                db.Results.Add(result);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ATHLETEID = new SelectList(db.Athletes, "ID", "NAME", result.ATHLETEID);
            ViewBag.EVENTID = new SelectList(db.Events, "ID", "STROKE", result.EVENTID);
            ViewBag.MEETID = new SelectList(db.Meets, "ID", "LOCATION", result.MEETID);
            return View(result);
        }

        // GET: Results/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Result result = db.Results.Find(id);
            if (result == null)
            {
                return HttpNotFound();
            }
            ViewBag.ATHLETEID = new SelectList(db.Athletes, "ID", "NAME", result.ATHLETEID);
            ViewBag.EVENTID = new SelectList(db.Events, "ID", "STROKE", result.EVENTID);
            ViewBag.MEETID = new SelectList(db.Meets, "ID", "LOCATION", result.MEETID);
            return View(result);
        }

        // POST: Results/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EVENTID,ATHLETEID,MEETID,TIME")] Result result)
        {
            if (ModelState.IsValid)
            {
                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ATHLETEID = new SelectList(db.Athletes, "ID", "NAME", result.ATHLETEID);
            ViewBag.EVENTID = new SelectList(db.Events, "ID", "STROKE", result.EVENTID);
            ViewBag.MEETID = new SelectList(db.Meets, "ID", "LOCATION", result.MEETID);
            return View(result);
        }

        // GET: Results/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Result result = db.Results.Find(id);
            if (result == null)
            {
                return HttpNotFound();
            }
            return View(result);
        }

        // POST: Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Result result = db.Results.Find(id);
            db.Results.Remove(result);
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