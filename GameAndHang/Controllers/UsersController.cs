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
using static GameAndHang.Controllers.ManageController;
using System.IO;
using System.Text.RegularExpressions;

namespace GameAndHang.Controllers
{
    public class UsersController : Controller
    {

        private GnHContext db = new GnHContext();

        // GET: Users 
        public async Task<ActionResult> Index()
        {
            if (User.Identity.GetUserId() == null)
            {
                return RedirectToAction("Index", "Home"); 
            }
            var users = db.Users.Include(u => u.AspNetUser);
            return View(await users.ToListAsync());
        }


        //Method used to generate information to pass to the view of a users profile 
        //when accessing their own profile
        public async Task<ActionResult> IndexUser()
        {
            var userId = User.Identity.GetUserId(); //get the ID of the user who is logged in

            //query the user from the DB that matches the ID of the person logged in
            //create a new User and assign the logged in user to it
            User findUser = await db.Users.FindAsync(userId); 


            List<Event> UpcomingEvents = new List<Event>(); //creat new list of events that will be used for upcoming events

            var userID = findUser.ID; 
            int xp = findUser.HostXP;  //grab the users current XP 
            int newLevel = HostLevel(xp); //take current users xp and pass it into the HostLevel function to calcualate the users current level
            int countReviews = 0; //initalize a variable for counting the total number of reviews the user has

            if (findUser != null) //if we successfully grabbed the user
            {
                findUser.HostLevel = newLevel; //assign the users new level 
                db.Entry(findUser).State = EntityState.Modified; //Modify the entry in the DB
                db.SaveChanges(); //Save DB changes
            }
            countReviews += (from b in db.Reviews where b.Host_ID == userID select b.ReviewString).Count(); //query the total reviews and count them
            if (countReviews > 0)
            {
                var userreviews = (from b in db.Reviews where b.Host_ID == userID select b.ReviewString).ToList(); //if there are more than 0 reviews, generate a list
                ViewBag.Reviews = userreviews; 
            }
            else
            {
                ViewBag.Reviews = ""; //if there are zero reviews, pass nothing to the viewbag
            }

            GetFriendsData(); //call method to get the users friends

            foreach (var hostedEvent in findUser.Events) //for each event the user is hosting
            {
                if (hostedEvent.Date <= DateTime.Now.AddDays(30) && hostedEvent.Date > DateTime.Now.AddDays(-1))
                {
                    UpcomingEvents.Add(hostedEvent);
                }
            }
            ViewBag.upcomingEvents = new List<Event>(UpcomingEvents);
            return View(findUser);
        }

        //Method to generate the informatio to display when someone is visiting a hosts profile.
        //passing in the ID of the host we are viewing
        public ActionResult HostProfile(string host)
        {
            User FindUsr = db.Users.Find(host); //find the user that matches the hostID
            var userID = FindUsr.ID;
            double? numRatings = 0; 
            double? sumRatings = 0;
            int xp = 0;
            ViewBag.status = 0;
            string secondaryID = User.Identity.GetUserId();
            xp += FindUsr.HostXP; //set xp to be the hosts current xp
            foreach(var relationship in db.Relationships)
            {
                if(db.Relationships.Find(secondaryID, host) != null | db.Relationships.Find(host, secondaryID) != null)
                {
                ViewBag.status = 1;
                }
            }
            if(numRatings != 0)
            {
                 sumRatings = (from b in db.Reviews
                                where b.Host_ID == userID
                                 select b.Rating).Sum();
            }
            CheckHostRelationships(FindUsr.ID);
            int newLevel = HostLevel(xp);
            ViewBag.ImagePath = @"~/Content/Images/Level1.png";
            numRatings += (from b in db.Reviews where b.Host_ID == userID select b).Count();
            if(numRatings > 0) {
            sumRatings += (from b in db.Reviews where b.Host_ID == userID select b.Rating).Sum();
            }
            if (FindUsr != null)
            {
                FindUsr.HostLevel = newLevel;
                db.Entry(FindUsr).State = EntityState.Modified;
                db.SaveChanges();
            }

            var userreviews = (from b in db.Reviews
                               where b.Host_ID == userID
                               select b.ReviewString).ToList();
            var format = "test";
            if(numRatings != 0 && sumRatings != 0)
            {
                format = String.Format("{0:0.#}", sumRatings / numRatings);
            }
            else if(numRatings > 0 && sumRatings == 0)
            {
                format = "0";
            }
            else
            {
                format = "This host has no ratings";
            }
            if(userreviews != null)
            {
            ViewBag.Reviews = userreviews;
            }
            else
            {
                ViewBag.Reviews = "No Reviews Yet";
            }
            ViewBag.Rating = format;
            ViewBag.NumRatings = numRatings;
            return View(FindUsr);
        }

        //Calculate host xp
        public int HostLevel(int xp)
        {
            if (xp < 10) return 1;
            if (xp >= 20 && xp < 50) return 2;
            if (xp >= 50 && xp < 100) return 3;
            if (xp >= 100 && xp < 160) return 4;
            if (xp >= 160 && xp < 220) return 5;
            if (xp >= 220 && xp < 300) return 6;
            if (xp >= 300) return 7;
            return -1;
        }

        //Functions for profile picture upload
        public byte[] ConvertToByte(HttpPostedFileBase image)
        {
            byte[] tempByte = null;
            BinaryReader ImgReader = new BinaryReader(image.InputStream);
            tempByte = ImgReader.ReadBytes((int)image.ContentLength);
            return tempByte;
        }

        public void UploadPhotoToDB(HttpPostedFileBase file)
        {
            String myuserID = User.Identity.GetUserId();
            User CurrentUser = db.Users.Find(myuserID);
            CurrentUser.ProfilePic = ConvertToByte(file);
            db.SaveChanges();
        }

        public ActionResult IndexPhoto()
        {
            return View();
        }
        //Gets profile pic from db
        [HttpPost]
        public ActionResult IndexPhoto(HttpPostedFileBase file)
        {
            UploadPhotoToDB(file);
            return RedirectToAction("IndexUser", "Users"); ;
        }

        //Find a specific profile
        public ActionResult FriendProfile(string id)
        {
            User requestedUser = db.Users.Find(id);
            return RedirectToAction("HostProfile/", new { host = requestedUser });
        }

        //Checks for malicious input
        public void CheckInput(User input)
        {
            if(input.FirstName.Length <= 1 || input.LastName.Length <= 2)
            {
                ModelState.AddModelError("Error", "First or Last name Must be Greater than 1 character");
            }
            if(input.DisplayName.Length <= 2)
            {
                ModelState.AddModelError("Error", "User Name Must be Greater than 2 characters");
            }
            var compRegEx = new Regex(@"[^a-zA-Z0-9\s]");
            if (compRegEx.IsMatch(input.DisplayName) || compRegEx.IsMatch(input.FirstName) || compRegEx.IsMatch(input.LastName))
            {
                ModelState.AddModelError("Error", "Names cannot contain special characters");
            }
        }

        //Friendship Functions that are user specific
        public void GetFriendsData()
        {
            var currentID = User.Identity.GetUserId();
            List<User> Pendingfriends = new List<User>();
            List<User> UnconfirmedFirends = new List<User>();
            List<User> ConfirmedFriends = new List<User>();
            List<string> pendingFriendIds = CheckPendingFriendships();
            List<string> unconfirmedFriendIDs = CheckUnConfirmedFriendships();
            List<string> confirmedFriendsIDs = CheckRelationships(1);
            foreach (string item in pendingFriendIds)
            {
                if (item != currentID)
                {
                    Pendingfriends.Add(db.Users.Find(item));
                }
            }
            foreach (string item in unconfirmedFriendIDs)
            {
                if (item != currentID)
                {
                    UnconfirmedFirends.Add(db.Users.Find(item));
                }
            }
            foreach (string item in confirmedFriendsIDs)
            {
                if (item != currentID)
                {
                    ConfirmedFriends.Add(db.Users.Find(item));
                }
            }
            ViewBag.PendingFriends = Pendingfriends;
            ViewBag.UnconfirmedFriends = UnconfirmedFirends;
            ViewBag.ConfirmedFriends = ConfirmedFriends;
        }
        //Checks the user profile for pending friends requests
        public List<string> CheckPendingFriendships()
        {
            string secondaryID = User.Identity.GetUserId();
            List<Relationship> friendshipIDs = new List<Relationship>();
            var friendships = (from b in db.Relationships where b.UserFirstID == secondaryID && b.Type == 2 select b.UserSecondID).ToList();
            return friendships;
        }
        //Checks to see if the user has any inconfirmed friends
        public List<string> CheckUnConfirmedFriendships()
        {
            string secondaryID = User.Identity.GetUserId();
            List<Relationship> friendshipIDs = new List<Relationship>();
            var friendships = (from b in db.Relationships where b.UserSecondID == secondaryID && b.Type == 2 select b.UserFirstID).ToList();
            return friendships;
        }
        //Checks to see if the user has any relationships
        public List<string> CheckRelationships(int type)
        {
            string secondaryID = User.Identity.GetUserId();
            List<string> ConfirmedFriends = new List<string>();
            List<string> getFriendships = (from b in db.Relationships where b.UserFirstID == secondaryID | b.UserSecondID == secondaryID && b.Type == type select b.UserFirstID).ToList();
            List<string> getMoreFriendships = (from b in db.Relationships where b.UserFirstID == secondaryID | b.UserSecondID == secondaryID && b.Type == type select b.UserSecondID).ToList();

            foreach (var item in getFriendships)
            {
                ConfirmedFriends.Add(item);
            }
            foreach (var item in getMoreFriendships)
            {
                ConfirmedFriends.Add(item);
            }
            return ConfirmedFriends;
        }
        //Gets all of the confirmed frinds for a host
        public void CheckHostRelationships(string id)
        {
            List<User> ConfirmedFriends = new List<User>();
            List<string> confirmedFriendsIDs = (from b in db.Relationships where b.UserFirstID == id | b.UserSecondID == id && b.Type == 1 select b.UserFirstID).ToList();
            List<string> moreConfirmedFriendIDs = (from b in db.Relationships where b.UserFirstID == id | b.UserSecondID == id && b.Type == 1 select b.UserSecondID).ToList();

            foreach (string item in confirmedFriendsIDs)
            {
                ConfirmedFriends.Add(db.Users.Find(item));
            }
            foreach (string item in moreConfirmedFriendIDs)
            {
                ConfirmedFriends.Add(db.Users.Find(item));
            }
            foreach (var item in ConfirmedFriends.ToList())
            {
                if (item.ID == id)
                {
                    ConfirmedFriends.Remove(item);
                }
            }
            ViewBag.ConfirmedFriends = ConfirmedFriends;
        }



        public ActionResult Create()
        {
            ViewBag.ID = User.Identity.GetUserId();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FirstName,LastName,DOB,DisplayName,Bio,ProfilePic")] User user)
        {
            user.ID = User.Identity.GetUserId();
            user.HostXP = 0;
            user.HostLevel = 0;
            CheckInput(user);
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction("IndexUser");
            }
            ViewBag.ID = new SelectList(db.AspNetUsers, "Id", "Email", user.ID);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user.ID != User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Home");
            }
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.AspNetUsers, "Id", "Email", user.ID);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,FirstName,LastName,DOB")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.AspNetUsers, "Id", "Email", user.ID);
            return View(user);
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user.ID != User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Home");
            }
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            User user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
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
