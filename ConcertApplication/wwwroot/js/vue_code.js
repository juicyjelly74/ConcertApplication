var concert_details = new Vue({
    el: '#concert_details',
    data: {
        item: null
    },
    mounted() {
        axios
            .get('/concerts/details/1')
            .then(response => (this.item = response.data));
    },
    methods: {
    }
});

var creation_concert = new Vue({
    el: '#creation_concert',
    data: {     
        selected: null

        /*name: null,
        concert_performer: null,
        tickets_amount: null,
        concert_date: null,
        place: null,
        price: null,
        concert_type: null,
        vocal_type: null,
        classical_concert_name: null,
        composer: null,
        drive_way: null,
        headliner: null,
        age_qualification: null*/

    },
    methods: {
        //create_concert: function () {
        //    try {
        //        var data = {
        //            "performer": this.concert_performer,
        //            "ticketsAmount": this.tickets_amount,
        //            "concertDate": this.concert_date,
        //            "place": this.place,
        //            "price": this.price,
        //            "type": this.concert_type,
        //            "vocalType": this.vocal_type,
        //            "classicalConcertName": this.classical_concert_name,
        //            "composer": this.composer,
        //            "driveWay": this.drive_way,
        //            "headliner": this.headliner,
        //            "ageQualification": this.age_qualification
        //        };
        //        var error;
        //        axios({
        //            method: 'post',
        //            url: '/concerts/add',
        //            data
        //        })
        //            .then(function (response) {
        //                console.log("successfully added!");
        //            })
        //            .catch(function (error) {
        //                console.log(error);
        //            });
        //    } catch (ex) {
        //        console.log(ex);
        //    }
        //    return false;
        //}


        sortBy: function () {
            console.log(this.sorting);
        },
    }
});