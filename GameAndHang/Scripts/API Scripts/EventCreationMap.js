
// This Google Maps API script will initialize a map object centered on the user's location, and will drop a pin on the 
// Event's address when entered. It was cobbled together based on several different tutorials
// found at https://developers.google.com/maps/documentation/javascript/tutorial

// IMPORTANT: In the .cshtml file that calls this script, after the <script> element that references this file you MUST
// include the following second <script> element in order for the script to actually send the request to the API:

// <script async defer
//     src="https://maps.googleapis.com/maps/api/js?key=YOUR_API_KEY&callback=initMap">
// </script>

// Also remember to switch out the YOUR_API_KEY part in the URL for the actual key.

// This map initialization requires that you consent to location sharing when
// prompted by your browser. If you see the error "The Geolocation service
// failed.", it probably means you did not give permission for the browser to
// locate you.

var geocoder;
var map, infoWindow;

function initMap() {
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 44.943, lng: 123.035 },
        zoom: 6
    });
    infoWindow = new google.maps.InfoWindow;

    // Try HTML5 geolocation.
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };

            infoWindow.setPosition(pos);
            infoWindow.setContent('Location found.');
            infoWindow.open(map);
            map.setCenter(pos);
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

function codeAddress() {
    var address = document.getElementById('address').value;
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status == 'OK') {
            map.setCenter(results[0].geometry.location);
            var marker = new google.maps.Marker({
                map: map,
                position: results[0].geometry.location
            });
        } else {
            alert('Geocode was not successful for the following reason: ' + status);
        }
    });
}


/* Here's the syntax for using this script in the .cshtml file:
 * 
 * <body onload="initMap()">
 *  <div id="map" style="width: 320px; height: 480px;"></div>
 *  <div>
 *      <input id="address" type="textbox" value="Salem, OR">
 *      <input type="button" value="Encode" onclick="codeAddress()">
 *  </div>
 * </body>
 * 
 */