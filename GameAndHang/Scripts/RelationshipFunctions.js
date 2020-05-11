$(document).on('click', '#RelationshipBtn', function () {
    var data = $('#modelID');
    var source = '/RelationshipController/SendFriendRequest/' + data;
    $.ajax({
        url: source,
        success: ChangeFirendBtn,
        error: errorOnAjax
    });
})

function HelpSendFriendRequest(source) {
    if (confirm("Are you sure?")) {
        $.ajax({
            url: source,
            
        }).done(function () {
            alert("Friends Request Sent!")
            ChangeFirendBtn;
        });
        return false;
    }
    else {
        return false;
    }
}
function NavigetToFriend(source) {
    $.ajax({
        url: source,
    })
}
function HelpConfirmFriendRequest(source) {
    if (confirm("Are you sure?")) {
        $.ajax({
            url: source,

        }).done(function () {
            alert("Conformation sent!")
            ChangeFirendBtn;
        });
        return false;
    }
    else {
        return false;
    }
}

function errorOnAjax() {
    console.log('Error on AJAX return');
}

function ChangeFirendBtn() {
    $("#relationshipBtn").style.background = "Red";
}

function displayFriends(FriendsList) {
    console.log(FriendsList);
}
function CheckUnconfirmedRelationships(UnconfirmedList) {

}




//        //Confirms a friendship
//        public void ConfirmRelationship(string PrimaryID, string SecondaryID)
//{
//    Relationship existingRelationship = GetRelationship(PrimaryID, SecondaryID);
//    existingRelationship.Type = 1;
//    SaveChanges(existingRelationship);
//}
//        //Gets a list of users friendships
//        public List < Relationship > GetRelationshipsList(string id)
//{
//    List < Relationship > friendsList = new List<Relationship>();
//    friendsList.Append(db.Relationships.Find(id)).Where(item => item.UserFirstID == id || item.UserSecondID == id);
//    return (friendsList);
//}
//        //Gets a specific relationship
//        public Relationship GetRelationship(string primaryID, string secondaryID)
//{
//    Relationship existingRelationship = db.Relationships.Find(primaryID, secondaryID);
//    return existingRelationship;
//}
//        //Removes friend
//        public void RemoveRelationship(string primaryID, string secondaryID)
//{
//    Relationship existingRelationship = GetRelationship(primaryID, secondaryID);
//    db.Relationships.Remove(existingRelationship);
//    db.SaveChanges();
//}
//        //Blocks a user from being a friend
//        public void BlockUser(string primaryID, string secondaryID)
//{
//    Relationship relationship = GetRelationship(primaryID, secondaryID);
//    relationship.Type = 4;
//    SaveChanges(relationship);
//}