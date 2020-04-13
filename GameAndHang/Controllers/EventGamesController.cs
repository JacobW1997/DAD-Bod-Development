using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GameAndHang.DAL;
using GameAndHang.Models;
using Microsoft.AspNet.Identity;

namespace GameAndHang.Controllers
{
    public class EventGamesController : Controller
    {
        private GnHContext db = new GnHContext();

        // GET: EventGames
        public async Task<ActionResult> Index()
        {
            var eventGames = db.EventGames.Include(e => e.Event).Include(e => e.Game);
            return View(await eventGames.ToListAsync());
        }

        // GET: EventGames/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventGame eventGame = await db.EventGames.FindAsync(id);
            if (eventGame == null)
            {
                return HttpNotFound();
            }
            return View(eventGame);
        }

        // GET: EventGames/Create
        public ActionResult Create()
        {
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
                .Select(x => x)
                ;

            if (!gameList.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.EventID= new SelectList(gameList, "ID", "EventName");
            ViewBag.GameID = new SelectList(db.Games, "ID", "Name");
            return View();
        }

        // POST: EventGames/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,EventID,GameID")] EventGame eventGame)
        {
            //var gameID = db.Events
            //.Where(x => x.EventName == eventGame.EventList)
            //.Select(x => x.ID)
            //.Single();

            //eventGame.EventID = gameID;
            var findEventID = db.Events
                .Where(x => x.EventName == eventGame.Event.EventName)
                .Select(x => x.ID);

            //eventGame.EventID = findEventID.ToString();


            if (ModelState.IsValid)
            {
                db.EventGames.Add(eventGame);
                await db.SaveChangesAsync();
                return RedirectToAction("Create");
            }

            ViewBag.EventID = new SelectList(db.Events, "ID", "EventName", eventGame.EventID);
            ViewBag.GameID = new SelectList(db.Games, "ID", "Name", eventGame.GameID);
            return View(eventGame);
        }

        // GET: EventGames/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventGame eventGame = await db.EventGames.FindAsync(id);
            if (eventGame == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventID = new SelectList(db.Events, "ID", "EventName", eventGame.EventID);
            ViewBag.GameID = new SelectList(db.Games, "ID", "Name", eventGame.GameID);
            return View(eventGame);
        }

        // POST: EventGames/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,EventID,GameID")] EventGame eventGame)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventGame).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EventID = new SelectList(db.Events, "ID", "EventName", eventGame.EventID);
            ViewBag.GameID = new SelectList(db.Games, "ID", "Name", eventGame.GameID);
            return View(eventGame);
        }

        // GET: EventGames/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventGame eventGame = await db.EventGames.FindAsync(id);
            if (eventGame == null)
            {
                return HttpNotFound();
            }
            return View(eventGame);
        }

        // POST: EventGames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            EventGame eventGame = await db.EventGames.FindAsync(id);
            db.EventGames.Remove(eventGame);
            await db.SaveChangesAsync();
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
