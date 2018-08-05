var search_app = new Vue({
    el: '#search_app',
    data: {
        selected: null,
        search: '',
        concertList: [],
        typeFilter: ''
    },
    mounted() {
        try {
            var error;
            axios({
                method: 'get',
                url: '/concerts/getConcerts'
            })
                .then((response) => this.concertList = response.data)
                .catch(function (error) {
                    console.log(error);
                });
        } catch (ex) {
            console.log(ex);
        };
    },
    computed: {
        filteredList() {
            return this.concertList.filter(concert => {
                return ((concert.name.toLowerCase().includes(this.search.toLowerCase())
                    || concert.performer.toLowerCase().includes(this.search.toLowerCase())
                    || concert.place.toLowerCase().includes(this.search.toLowerCase()))
                    && concert.type.toLowerCase().includes(this.typeFilter.toLowerCase()))
            })
        }
    }
})