var ticket_app = new Vue({
    el: '#ticket_app',
    data: {
        selected: null,
        search: '',
        ticketList: []
    },
    mounted() {
        try {
            var error;
            axios({
                method: 'get',
                url: '/concerts/getTickets'
            })
                .then((response) => this.ticketList = response.data)
                .catch(function (error) {
                    console.log(error);
                });
        } catch (ex) {
            console.log(ex);
        };
    }
})