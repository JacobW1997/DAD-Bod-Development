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

namespace GameAndHang.Controllers
{
    public class UsersController : Controller
    {

        private GnHContext db = new GnHContext();

        // GET: Users 
        public async Task<ActionResult> Index()
        {
            var users = db.Users.Include(u => u.AspNetUser);
            return View(await users.ToListAsync());
        }
        
        public async Task<ActionResult> IndexUser()
        {
            var userId = User.Identity.GetUserId();
            User findUser = await db.Users.FindAsync(userId);
            List<Event> UpcomingEvents = new List<Event>();
            var userID = findUser.ID;
            int xp = findUser.HostXP;
            int newLevel = HostLevel(xp);
            int countReviews = 0;

            if (findUser != null)
            {
                findUser.HostLevel = newLevel;
                db.Entry(findUser).State = EntityState.Modified;
                db.SaveChanges();
            }
            countReviews += (from b in db.Reviews where b.Host_ID == userID select b.ReviewString).Count();
            if (countReviews > 0)
            {
                var userreviews = (from b in db.Reviews where b.Host_ID == userID select b.ReviewString).ToList();
                ViewBag.Reviews = userreviews;
            }
            else
            {
                ViewBag.Reviews = "No reviews yet!";
            }
            GetFriendsData();
            foreach (var hostedEvent in findUser.Events)
            {
                if (hostedEvent.Date <= DateTime.Now.AddDays(30) && hostedEvent.Date > DateTime.Now.AddDays(-1))
                {
                    UpcomingEvents.Add(hostedEvent);
                }
            }
            ViewBag.upcomingEvents = new List<Event>(UpcomingEvents);
            return View(findUser);
        }

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
        public List<string> CheckPendingFriendships()
        {
            string secondaryID = User.Identity.GetUserId();
            List<Relationship> friendshipIDs = new List<Relationship>();
            var friendships = (from b in db.Relationships where b.UserFirstID == secondaryID && b.Type == 2 select b.UserSecondID).ToList();
            return friendships;
        }
        public List<string> CheckPendingFriendships(string id)
        {
            string secondaryID = id;
            List<Relationship> friendshipIDs = new List<Relationship>();
            var friendships = (from b in db.Relationships where b.UserFirstID == secondaryID && b.Type == 2 select b.UserSecondID).ToList();
            return friendships;
        }
        public List<string> CheckUnConfirmedFriendships()
        {
            string secondaryID = User.Identity.GetUserId();
            List<Relationship> friendshipIDs = new List<Relationship>();
            var friendships = (from b in db.Relationships where b.UserSecondID == secondaryID && b.Type == 2 select b.UserFirstID).ToList();
            return friendships;
        }

        public List<string> CheckRelationships(int type)
        {
            string secondaryID = User.Identity.GetUserId();
            List<string> ConfirmedFriends = new List<string>();
            List<string> getFriendships = (from b in db.Relationships where b.UserFirstID == secondaryID | b.UserSecondID == secondaryID && b.Type == type select b.UserFirstID).ToList();
            List<string> getMoreFriendships = (from b in db.Relationships where b.UserFirstID == secondaryID | b.UserSecondID == secondaryID && b.Type == type select b.UserSecondID).ToList();

            foreach(var item in getFriendships)
            {
                ConfirmedFriends.Add(item);
            }
            foreach(var item in getMoreFriendships)
            {
                ConfirmedFriends.Add(item);
            }

            return ConfirmedFriends;
        }

        public void CheckHostRelationships(string id)
        {
            List<User> ConfirmedFriends = new List<User>();
            List <string> confirmedFriendsIDs = (from b in db.Relationships where b.UserFirstID == id | b.UserSecondID == id && b.Type == 1 select b.UserFirstID ).ToList();
            List <string> moreConfirmedFriendIDs = (from b in db.Relationships where b.UserFirstID == id | b.UserSecondID == id && b.Type == 1 select b.UserSecondID).ToList();


            foreach (string item in confirmedFriendsIDs)
            {
                ConfirmedFriends.Add(db.Users.Find(item));
            }
            foreach(string item in moreConfirmedFriendIDs)
            {
                ConfirmedFriends.Add(db.Users.Find(item));
            }
            foreach (var item in ConfirmedFriends.ToList())
            if (item.ID == id)
            {
                    ConfirmedFriends.Remove(item);
            }
            


            ViewBag.ConfirmedFriends = ConfirmedFriends;
        }

        public async Task<ActionResult> HostProfile(string host)
        {
            User FindUsr = await db.Users.FindAsync(host);
            var userID = FindUsr.ID;
            double? numRatings = 0;
            double? sumRatings = 0;
            int xp = 0;

            xp += FindUsr.HostXP;



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

        public string GetUserName(string ID)
        {
            User UserName = db.Users.Find(ID);
            return (UserName.DisplayName);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.ID = User.Identity.GetUserId();
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,FirstName,LastName,DOB,DisplayName,Bio,ProfilePic")] User user)
        {
            //var userID = User.Identity.GetUserId();
            user.ID = User.Identity.GetUserId();
            user.HostXP = 0;
            user.HostLevel = 0;
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction("Index","Users" );
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
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.AspNetUsers, "Id", "Email", user.ID);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
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
        [HttpPost]
        public ActionResult IndexPhoto(HttpPostedFileBase file)
        {
            UploadPhotoToDB(file);
            return RedirectToAction("IndexUser", "Users"); ;
        }
        public ActionResult FriendProfile(string id)
        {
            User requestedUser = db.Users.Find(id);
            return RedirectToAction("HostProfile/", new { host = requestedUser });
        }

    }
}
