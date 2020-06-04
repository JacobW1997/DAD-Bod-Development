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

        //Method for creating a review and saving to DB. Parameters are passed from the view
        //Param review: string of text that the user inputs as comments
        //Param ID: The ID of the user that is being reviewed
        //Param answer: is the number associated with a rating from 1-5
        public async Task<ActionResult> ReviewCreate(string review, string ID, int answer)
        {
            Review newReview = new Review();   //create new review
            newReview.ReviewString = review; //set new reviews string 
            newReview.Reviewer_ID = User.Identity.GetUserId(); //the person who is currently logged in is the reviewer

            //code to generate the new reviews ID
            Guid g = Guid.NewGuid();
            string gIDString = Convert.ToBase64String(g.ToByteArray());
            gIDString = gIDString.Replace("=", "");
            gIDString = gIDString.Replace("+", "");
            gIDString = gIDString.Replace("/", "");


            newReview.ID = gIDString; //Set newly generated ID to be the new reviews ID
            newReview.Host_ID = ID; //assign our parameter ID to the person we are leaving the review for
            newReview.Rating = answer; //set new reviews rating

            if (ModelState.IsValid) //if the model is valid
            {
                db.Reviews.Add(newReview); //add new review to DB
                await db.SaveChangesAsync(); //save DB changes
                return RedirectToAction("Success", "Reviews"); //direct user to success page
            }

            return View(); //return to view if input was bad
        }

        public ActionResult Success()
        {
            return View();
        }

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
