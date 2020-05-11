using GameAndHang.DAL;
using GameAndHang.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameAndHang.Controllers
{
    public class RelationshipController : Controller
    {
        private GnHContext db = new GnHContext();

        public void SendFriendRequest(string recieverID)
        {
            string senderID = User.Identity.GetUserId();
            Relationship newRelationship = new Relationship();
            newRelationship.UserFirstID = recieverID;
            newRelationship.UserSecondID = senderID;
            newRelationship.Type = 2;
            if(newRelationship != null) { 
            db.Relationships.Add(newRelationship);
            db.SaveChanges();
            }
        }

        //Saves the changes to the relationship in the DB
        public void SaveChanges(Relationship relationship)
        {
            db.Entry(relationship).State = EntityState.Modified;
            db.SaveChanges();
        }
        //Might not need
        public List<Relationship> CheckUnconfirmedRelationships(string ID)
        {
            List<Relationship> friendshipIDs = new List<Relationship>();
            friendshipIDs.Append(db.Relationships.Find(ID)).Where(item => item.UserFirstID == ID || item.UserSecondID == ID && item.Type == 1);
            return friendshipIDs;
        }
        //Confirms a friendship
        public void ConfirmRelationship(string PrimaryID, int type)
        {
            string SecondaryID = User.Identity.GetUserId();
            Relationship existingRelationship = GetRelationship(SecondaryID,PrimaryID);
            existingRelationship.Type = type;
            SaveChanges(existingRelationship);
        }
        //Gets a list of users friendships
        public List<Relationship> GetRelationshipsList(string id)
        {
            List<Relationship> friendsList = new List<Relationship>();
            friendsList.Append(db.Relationships.Find(id)).Where(item => item.UserFirstID == id || item.UserSecondID == id);
            return (friendsList);
        }
        //Gets a specific relationship
        public Relationship GetRelationship(string primaryID, string secondaryID)
        {
            Relationship existingRelationship = db.Relationships.Find(primaryID, secondaryID);
            return existingRelationship;
        }
        //Removes friend
        public void RemoveRelationship(string primaryID)
        {
            string secondaryID = User.Identity.GetUserId();
            Relationship existingRelationship = GetRelationship(primaryID, secondaryID);
            db.Relationships.Remove(existingRelationship);
            db.SaveChanges();
        }
        //Blocks a user from being a friend
        public void BlockUser(string primaryID, string secondaryID)
        {
            Relationship relationship = GetRelationship(primaryID, secondaryID);
            relationship.Type = 4;
            SaveChanges(relationship);
        }

    }
}
