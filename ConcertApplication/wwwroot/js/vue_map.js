Vue.component('concert-map', {
    data: function () {
        return {
            mapName: 'concert-map',
            markerCoordinates: [{
                latitude: 40.7,
                longitude: -73.9
            }],
            map: null,
            bounds: null,
            markers: [],
            addresses: [],
            searchAddressInput: '',
            currentLocation: {
                latitude: 53.9,
                longitude: 27.5667
            },
            addresses: []
        }
    },
    mounted() {
        
        var get_data_template = (response, resolve) => {
            this.addresses = response.data;
            resolve("Success!");
        };
        myPromise = new Promise((resolve, reject) => {
            try {
                var error;
                axios({
                    method: 'get',
                    url: '/concerts/getPlaces'
                })
                    .then((data) => get_data_template(data, resolve))
                    .catch(function (error) {
                        console.log(error);
                    });
            } catch (ex) {
                console.log(ex);
            }
        });
        var promises = [];

        myPromise.then((successMessage) => {
            var that = this;           
            var geocoder = new google.maps.Geocoder();

            for (var i = 0; i < that.addresses.length; i++) {
                if (!this.addresses[i]) {
                    break;
                }

                promises.push(new Promise((resolve, reject) => {
                    geocoder.geocode({ 'address': this.addresses[i] }, function (results, status) {
                        if (status == google.maps.GeocoderStatus.OK) {
                            let currentLocation = {
                                latitude: results[0].geometry.location.lat(),
                                longitude: results[0].geometry.location.lng()
                            }
                            resolve(currentLocation);
                        }
                        else {
                            resolve(null);
                        }
                    });
                }))
            }

            var that = this;
            Promise.all(promises).then((locations) => {

                for (var i = 0; i < locations.length; i++) {
                    var coord = locations[i];
                    if (coord != null) {
                        var position = new google.maps.LatLng(coord.latitude, coord.longitude);
                        var marker = new google.maps.Marker({
                            position,
                            map: this.map,
                            title: this.addresses[i]
                        });
                        var infowindow = new google.maps.InfoWindow({
                            content: that.addresses[i]
                        });
                        marker.addListener('click', function () {
                            infowindow.open(this.map, marker);
                        });
                        this.markers.push(marker);
                        this.map.fitBounds(this.bounds.extend(position));
                    }
                }
            });
        });
        this.bounds = new google.maps.LatLngBounds();
        const element = document.getElementById('concert-map')
        const mapCentre = this.markerCoordinates[0]
        const options = {
            zoom: 10,
            center: new google.maps.LatLng(mapCentre.latitude, mapCentre.longitude)
        }
        this.map = new google.maps.Map(element, options);


        if (navigator.geolocation) {
            var that = this;
            navigator.geolocation.getCurrentPosition(function (position) {
                initialLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                that.map.setCenter(initialLocation);
            });
        }

    },
    template: '#concert_map_template'
});

var concert_map_app = new Vue({
    el: "#concert_map_app"
});