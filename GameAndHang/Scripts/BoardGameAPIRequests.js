$(document).ready(function () {
    var source = '/Home/GetGames';
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: source,
        success: displayGames,
        error: errorOnAjax
    });

});

function errorOnAjax() {
    console.log('Error on AJAX return');
}


function displayGames(games) {
    console.log(games);

    for (j = 0; j < 1; j++) {
        $('#GameOutput').append('<div class="row" id="games' + j + '">');

        for (i = 0; i < 40; i++) {
            $('#games' + j).append('<div class="col-md-3"> <div class= "card"> <div class="card-img" id="outputCard'+i+'"></div></div>');
            $('#outputCard' + i).append('<img src="' + games.thumb_url[i] + '" id="imgProfile" style="width: 235px; height: 150px" class="img-thumbnail" /></div>');
            $('#outputCard' + i).append('<p>' + games.name[i] + '</p>');
            $('#outputCard' + i).append('<p> Mentions on Reddit: ' + games.reddit_all_time_count[i] + '<p>');
            $('#outputCard' + i).append('<p> Average Rating: ' + games.average_user_rating[i] + '<p>');
            $('#outputCard' + i).append('<p>Min Players: ' + games.min_players[i] + '<p>');
            $('#outputCard' + i).append('<p>Max Players: ' + games.max_players[i] + '<p>');
            $('#outputCard' + i).append('<a onclick = getGames('+games.id[i]+'); href=/APIEventGame/APIGame/' + games.id[i] + '>Details</a>');
        }
        $('#GameOutput').append('</div>');
    }
}

function searchByGame() {
    var source = '/Home/GameSearch';
    var searchstring = document.getElementById('searchstring').value;
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: source,
        data: { 'searchString': searchstring },
        success: displaySearchResults,
        error: errorOnAjax
    });
}

function displaySearchResults(games) {
    console.log(games);

    $('#SearchResults').html('');

    for (j = 0; j < 1; j++) {
        $('#SearchResults').append('<div class="row" id="gamesearchresults' + j + '">');

        for (i = 0; i < 40; i++) {
            $('#gamesearchresults' + j).append('<div class="col-md-3"> <div class= "card"> <div class="card-img" id="searchOutputCard' + i + '"></div></div>');
            $('#searchOutputCard' + i).append('<img src="' + games.thumb_url[i] + '" id="imgProfile" style="width: 235px; height: 150px" class="img-thumbnail" /></div>');
            $('#searchOutputCard' + i).append('<p>' + games.name[i] + '</p>');
            $('#searchOutputCard' + i).append('<p> Mentions on Reddit: ' + games.reddit_all_time_count[i] + '<p>');
            $('#searchOutputCard' + i).append('<p> Average Rating: ' + games.average_user_rating[i] + '<p>');
            $('#searchOutputCard' + i).append('<p>Min Players: ' + games.min_players[i] + '<p>');
            $('#searchOutputCard' + i).append('<p>Max Players: ' + games.max_players[i] + '<p>');
            $('#searchOutputCard' + i).append('<a onclick = getGames(' + games.id[i] + '); href=/APIEventGame/APIGame/' + games.id[i] + '>Details</a>');
        }
        $('#SearchResults').append('</div>');
    }
}