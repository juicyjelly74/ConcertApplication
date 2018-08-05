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
            marker: null,
            addresses: [],
            concert: null,
            id: null
        }
    },
    mounted() {
        try {
            this.id = document.getElementById('#idDiv').getAttribute('data-value');
        } catch (ex) {
            console.log(ex);
        };
        var get_data_template = (response, resolve) => {
            this.concert = response.data;
            resolve("Success!");
        };
        getDataPromise = new Promise((resolve, reject) => {
            try {
                var error;
                axios({
                    method: 'get',
                    url: '/concerts/getConcertById',
                    params: {
                        id: this.id
                    }
                })
                    .then((data) => get_data_template(data, resolve))
                    .catch(function (error) {
                        console.log(error);
                    });
            } catch (ex) {
                console.log(ex);
            }
        });
        
        getDataPromise.then((successMessage) => {
            var that = this;
            var geocoder = new google.maps.Geocoder();
            var locatePromise;


            if (this.concert) {
                locatePromise = new Promise((resolve, reject) => {
                    geocoder.geocode({ 'address': this.concert.place }, function (results, status) {
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
                })
            }

            var that = this;
            locatePromise.then((location) => {
                var coord = location;
                if (coord != null) {
                    var position = new google.maps.LatLng(coord.latitude, coord.longitude);
                    var marker = new google.maps.Marker({
                        position,
                        map: this.map,
                        title: this.concert.place
                    });
                    marker.info = new google.maps.InfoWindow({
                        content: "<h5><b>" + that.concert.name + "</b></h5>" +
                            "<p>performer: " + that.concert.performer +
                            "</p><p>date: " + that.concert.date +
                            "</p><p>price: " + that.concert.price +
                            "</p><p>tickets left: " + that.concert.ticketsLeft
                    });
                    google.maps.event.addListener(marker, 'click', function () {
                        var marker_map = this.getMap();
                        this.info.open(marker_map, this);
                    });
                    this.marker = marker;
                    this.map.fitBounds(this.bounds.extend(position));
                }
            });
        });
        this.bounds = new google.maps.LatLngBounds();
        const element = document.getElementById('concert-map')
        const mapCentre = this.markerCoordinates[0]
        const options = {
            zoom: 4,
            center: new google.maps.LatLng(mapCentre.latitude, mapCentre.longitude)
        }
        this.map = new google.maps.Map(element, options);
        google.maps.event.addListener(this.map, "click", function (event) {
            this.marker.info.close();
        });

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