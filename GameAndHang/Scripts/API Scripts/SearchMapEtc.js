

// Pre-declare objects
    var map, infoWindow, markers;
    var geocoder;

// Stores user Lat/Long data in hidden fields in the view.
function getUserLoc() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };
            document.getElementById("userlat").value = pos.lat;
            document.getElementById("userlong").value = pos.lng;


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


// Generates a map centered on the user's current location as in EventCreationMap.js 
// and puts a marker on it for each search result.
    function initMap(markerNames, markerLats, markerLongs) {
        geocoder = new google.maps.Geocoder();
        map = new google.maps.Map(document.getElementById('map'), {
            center: {lat: 44.943, lng: -123.035 },
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

        // Generate objects to hold marker data
        var markerlabels = markerNames;
        var markerlats = markerLats;
        var markerlongs = markerLongs;
        var markerlocs;

        for (var i = 0; i < markerdata.Length; i++) {
            markerlocs[i] = new google.maps.LatLng(markerlats[i], markerlongs[i]);
        }

        // Generate markers from data
        markers = markerlocs.map(function(location, i) {
            return new google.maps.Marker({
                position: location,
                label: markerlabels[i]
            });
        });

// THIS WAS FOR TESTING
        //var start = new google.maps.Marker({
        //    map: map,
        //    animation: google.maps.Animation.DROP,
        //    position: { lat: 44.943, lng: -123.035 },
        //    title: "test"
        //})
        // Try HTML5 geolocation.
    }