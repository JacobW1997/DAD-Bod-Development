using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GameAndHang.DAL;
using GameAndHang.Models;
using Microsoft.AspNet.Identity;

namespace GameAndHang.Controllers
{
    public class ReviewsController : Controller
    {
        private GnHContext db = new GnHContext();

        // GET: Reviews
        public ActionResult Index()
        {
            return View(db.Reviews.ToList());
        }

        // GET: Reviews/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        
        public async Task<ActionResult> ReviewCreate(string review, string ID)
        {
            Review newReview = new Review();
            newReview.ReviewString = review;
            newReview.Reviewer_ID = User.Identity.GetUserId();

            Guid g = Guid.NewGuid();
            string gIDString = Convert.ToBase64String(g.ToByteArray());
            gIDString = gIDString.Replace("=", "");
            gIDString = gIDString.Replace("+", "");
            gIDString = gIDString.Replace("/", "");

            newReview.ID = gIDString;
            newReview.Host_ID = ID;

            if (ModelState.IsValid)
            {
                db.Reviews.Add(newReview);
                await db.SaveChangesAsync();
                return RedirectToAction("Success", "Reviews");
            }

            return View();
        }

        public ActionResult Success()
        {
            return View();
        }

        //public ActionResult Create(User user1)
        //{

        //}

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,ReviewString,Reviewer_ID,Host_ID")] Review review)
        //{
         


        //    if (ModelState.IsValid)
        //    {
        //        db.Reviews.Add(review);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(review);
        //}

        // GET: Reviews/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ReviewString,Reviewer_ID,Host_ID")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
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
