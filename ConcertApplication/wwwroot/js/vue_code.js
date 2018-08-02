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
                latitude: 51.501527,
                longitude: -0.1921837
            }, {
                latitude: 51.505874,
                longitude: -0.1838486
            }, {
                latitude: 51.4998973,
                longitude: -0.202432
            }],
            map: null,
            bounds: null,
            markers: [],
            addresses: [],
            searchAddressInput: '',
            currentAddress: '',
            currentLocation: '',
            addresses: []
        }
    },
    mounted() {

        this.getMarkerCoordinates();

        this.bounds = new google.maps.LatLngBounds();
        const element = document.getElementById('concert-map')
        const mapCentre = this.markerCoordinates[0]
        const options = {
            center: new google.maps.LatLng(mapCentre.latitude, mapCentre.longitude)
        }
        this.map = new google.maps.Map(element, options);
        this.markerCoordinates.forEach((coord) => {
            const position = new google.maps.LatLng(coord.latitude, coord.longitude);
            const marker = new google.maps.Marker({
                position,
                map: this.map
            });
            this.markers.push(marker);
            this.map.fitBounds(this.bounds.extend(position));
        });
    },
    methods: {
        searchLocation: function () {
            var geocoder = new google.maps.Geocoder();
            geocoder.geocode({ 'address': this.searchAddressInput }, (results, status) => {
                if (status === 'OK') {
                    this.currentLocation.lat = results[0].geometry.location.lat();
                    this.currentLocation.lng = results[0].geometry.location.lng();
                }
            });
        },
        getMarkerCoordinates: function () {
            this.getAddresses();
            var geocoder = new google.maps.Geocoder();
            this.addresses.forEach((address) => {
                geocoder.geocode({ 'address': this.address }, (results, status) => {
                    if (status === 'OK') { 
                        this.currentAddress.lat = results[0].geometry.location.lat();
                        this.currentAddress.lng = results[0].geometry.location.lng();
                        this.markerCoordinates.push(currentAddress);
                    }
                });
            });
        },
        getAddresses: function () {
            axios
                .get('/concerts/getPlaces')
                .then(response => (this.addresses = response.data));
        }
    },
    template: '#temp'
});

var aaa = new Vue({
    el: "#aaa"
});