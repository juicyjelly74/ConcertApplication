﻿Vue.component('concert-map', {
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
            infos: [],
            addresses: [],
            concerts: []
        }
    },
    mounted() {
        
        var get_data_template = (response, resolve) => {
            this.concerts = response.data;
            resolve("Success!");
        };
        myPromise = new Promise((resolve, reject) => {
            try {
                var error;
                axios({
                    method: 'get',
                    url: '/concerts/getConcerts'
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

            for (var i = 0; i < that.concerts.length; i++) {
                if (!this.concerts[i]) {
                    break;
                }

                promises.push(new Promise((resolve, reject) => {
                    geocoder.geocode({ 'address': this.concerts[i].place }, function (results, status) {
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
                            title: this.concerts[i].place
                        });
                        marker.info = new google.maps.InfoWindow({
                            content: "<h5><b>" + that.concerts[i].name + "</b></h5>" + 
                                "<p>performer: " + that.concerts[i].performer +
                                "</p><p>date: " + that.concerts[i].date +
                                "</p><p>price: " + that.concerts[i].price +
                                "</p><p>tickets left: " + that.concerts[i].ticketsLeft
                        });
                        that.infos.push(marker.info);
                        google.maps.event.addListener(marker, 'click', function () {
                            var marker_map = this.getMap();
                            this.info.open(marker_map, this);
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
            zoom: 4,
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