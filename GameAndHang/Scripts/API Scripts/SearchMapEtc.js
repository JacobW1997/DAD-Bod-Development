

var geocoder;
var map, infoWindow, markers;
var eventNames, eventLats, eventLongs;


function initMap() {
    geocoder = new google.maps.Geocoder();
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 44.943, lng: -123.035 },
        zoom: 10
    });
    infoWindow = new google.maps.InfoWindow;


    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };

            infoWindow.setPosition(pos);
            infoWindow.setContent('You are here.');
            infoWindow.open(map);
            map.setCenter(pos);
        }, function () {
            handleLocationError(true, infoWindow, map.getCenter());
        });
    } else {
        // Browser doesn't support Geolocation
        handleLocationError(false, infoWindow, map.getCenter());
    }

    var namedivs = document.querySelectorAll(".name");
    var latdivs = document.querySelectorAll(".lat");
    var longdivs = document.querySelectorAll(".long");

    var len = namedivs.length;

    for (i = 0; i < len; i++) {
        eventNames[i] = namedivs[i].textContent;
        eventLats[i] = latdivs[i].textContent;
        eventLongs[i] = longdivs[i].textContent;
    }

    for (i = 0; i < len; i++) {
        if (eventLats[i] != null && eventLongs[i] != null) {
            markers[i] = new google.maps.Marker({
                map: map,
                animation: google.maps.animation.DROP,
                position: { lat: eventLats[i], lng: eventLongs[i] },
                title: eventNames[i]
            })
        }
    }

    // THIS WAS FOR TESTING
    //var start = new google.maps.Marker({
    //    map: map,
    //    animation: google.maps.Animation.DROP,
    //    position: { lat: 44.943, lng: -123.035 },
    //    title: "test"
    //})
    // Try HTML5 geolocation.
}

function getUserLoc() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };
            document.getElementById("userlat").value = pos.lat;
            document.getElementById("userlong").value = pos.lng;
            //document.getElementById("userlochere").insertAdjacentHTML('beforebegin', '@Html.Hidden("UserLat", "' + pos.lat + '")');
            //document.getElementById("userlochere").insertAdjacentHTML('beforebegin', '@Html.Hidden("UserLong", "' + pos.lng + '")');
            
        }, function () {
            handleLocationError(true, infoWindow, map.getCenter());
        });
    } else {
        // Browser doesn't support Geolocation
        handleLocationError(false, infoWindow, map.getCenter());
    }
}


function handleLocationError(browserHasGeolocation, infoWindow, pos) {
    infoWindow.setPosition(pos);
    infoWindow.setContent(browserHasGeolocation ?
        'Error: The Geolocation service failed.' :
        'Error: Your browser doesn\'t support geolocation.');
    infoWindow.open(map);
}