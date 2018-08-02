var add_concert_app = new Vue({
    el: '#add_concert_app',
    data: {
        selected: null
    }
});

Vue.component('concert-map', {
    data: function () {
        return {
            mapName: 'concert-map',
            markerCoordinates: [{
                latitude: 53.9,
                longitude: 27.5667
            },
            {
                latitude: 51.501527,
                longitude: -0.1921837
            },

            {
                latitude: 51.505874,
                longitude: -0.1838486
            },
            {
                latitude: 51.4998973,
                longitude: -0.202432
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

        var fff = (response, resolve) => {
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
                    .then((data) => fff(data, resolve))
                    .catch(function (error) {
                        console.log(error);
                    });
            } catch (ex) {
                console.log(ex);
            }
        });

        myPromise.then((successMessage) => {
            this.getMarkerCoordinates(); 
        })
            .catch(function (error) {
                console.log(error);
            });

        myPromise.then((successMessage) => {
            this.markerCoordinates.forEach((coord) => {
                const position = new google.maps.LatLng(coord.latitude, coord.longitude);
                const marker = new google.maps.Marker({
                    position,
                    map: this.map
                });
                this.markers.push(marker);
                this.map.fitBounds(this.bounds.extend(position));
            });
        })
            .catch(function (error) {
                console.log(error);
            });

        this.bounds = new google.maps.LatLngBounds();
        const element = document.getElementById('concert-map')
        const mapCentre = this.markerCoordinates[0]
        const options = {
            center: new google.maps.LatLng(mapCentre.latitude, mapCentre.longitude)
        }
        this.map = new google.maps.Map(element, options);

    },
    methods: {
        getMarkerCoordinates: function () {
            var that = this;
            var geocoder = new google.maps.Geocoder();
            this.addresses.forEach((address) => {
                geocoder.geocode({ 'address': address }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        that.currentLocation.latitude = results[0].geometry.location.lat();
                        that.currentLocation.longitude = results[0].geometry.location.lng();
                        that.markerCoordinates.push(that.currentLocation);
                    }
                });
            });
        },
        getAddresses: function () {
            
        }
    },
    template: '#temp'
});

var aaa = new Vue({
    el: "#aaa"
});