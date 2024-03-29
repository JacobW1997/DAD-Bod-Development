﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GameAndHang.DAL;
using GameAndHang.Models;
using Microsoft.AspNet.Identity;

namespace GameAndHang.Controllers
{
    public class EventPlayersController : Controller
    {
        private GnHContext db = new GnHContext();

        // GET: EventPlayers
        public ActionResult Index()
        {
            var eventPlayers = db.EventPlayers.Include(e => e.Event).Include(e => e.User);
            return View(eventPlayers.ToList());
        }

        // GET: EventPlayers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventPlayer eventPlayer = db.EventPlayers.Find(id);
            if (eventPlayer == null)
            {
                return HttpNotFound();
            }
            return View(eventPlayer);
        }

        //A method used for creating a new entry in the event players table
        //this table was used to assign players to events
        public ActionResult Create(Event event1)
        {
            var getuserID = User.Identity.GetUserId();

            User currentUser = db.Users
                .Where(x => x.ID == getuserID)
                .Select(x => x)
                .Single();

            var hostID = event1.HostID;
            User host = db.Users.Find(hostID);

            host.HostXP += 1;
            db.Entry(host).State = EntityState.Modified;
            db.SaveChanges();

            EventPlayer eventPlayer = new EventPlayer
            {
                EventID = event1.ID,
                PlayerID = User.Identity.GetUserId(),
            };


            int? numPlayers = event1.PlayersCount;
        

            var eventID = event1.ID;
            var eventItem = db.Events.FirstOrDefault(x => x.ID == eventID);
            if(eventItem != null)
            {
                eventItem.PlayersCount = numPlayers + 1;
                db.Entry(eventItem).State = EntityState.Modified;
                db.SaveChanges();
            }


            if (User.Identity.GetUserId() != null)
            {
                db.EventPlayers.Add(eventPlayer);
                db.SaveChanges();
                return RedirectToAction("Success");
            }

            ViewBag.EventID = new SelectList(db.Events, "ID", "EventName", eventPlayer.EventID);
            ViewBag.PlayerID = new SelectList(db.Users, "ID", "FirstName", eventPlayer.PlayerID);
            return View(eventPlayer);
        }

        public ActionResult Success()
        {
            return View();
        }


        // GET: EventPlayers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventPlayer eventPlayer = db.EventPlayers.Find(id);
            if (eventPlayer == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventID = new SelectList(db.Events, "ID", "EventName", eventPlayer.EventID);
            ViewBag.PlayerID = new SelectList(db.Users, "ID", "FirstName", eventPlayer.PlayerID);
            return View(eventPlayer);
        }

        // POST: EventPlayers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,PlayerID,EventID")] EventPlayer eventPlayer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventPlayer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventID = new SelectList(db.Events, "ID", "EventName", eventPlayer.EventID);
            ViewBag.PlayerID = new SelectList(db.Users, "ID", "FirstName", eventPlayer.PlayerID);
            return View(eventPlayer);
        }

        // GET: EventPlayers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventPlayer eventPlayer = db.EventPlayers.Find(id);
            if (eventPlayer == null)
            {
                return HttpNotFound();
            }
            return View(eventPlayer);
        }

        // POST: EventPlayers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventPlayer eventPlayer = db.EventPlayers.Find(id);
            db.EventPlayers.Remove(eventPlayer);
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
