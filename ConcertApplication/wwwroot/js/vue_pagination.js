﻿Vue.component('paginated-list', {
    data: function () {
        return {
            pageNumber: 0
        }
    },
    props: {
        listData: {
            type: Array,
            required: true
        },
        size: {
            type: Number,
            required: false,
            default: 5
        }
    },
    methods: {
        nextPage() {
            this.pageNumber++;
        },
        prevPage() {
            this.pageNumber--;
        }
    },
    computed: {
        pageCount() {
            let l = this.listData.length,
                s = this.size;
            return Math.ceil(l / s);
        },
        paginatedData() {
            const start = this.pageNumber * this.size,
                end = start + this.size;
            return this.listData
                .slice(start, end);
        }
    },
    template: `<div>
                <div class="wrapper">
                    <div v-if="paginatedData.length == 0">
                        <div class="card">
                            <header>No such concerts.</header>
                        </div>
                    </div>
                   <div class="card" v-for="concert in paginatedData">
                        <card_body>
                            <header>{{ concert.name }}</header>
                            <small>Performer: {{ concert.performer }}</small>
                            <small>Type: {{ concert.type }}</small>
                            <small>Date: {{ concert.date }}</small>
                            <small>Place: {{ concert.place }}</small>
                            <small>Price: {{ concert.price }}</small>
                            <small>TicketsAmount: {{ concert.ticketsAmount }}</small>
                            <small>TicketsLeft: {{ concert.ticketsLeft }}</small>
                        </card_body>
                        <div class="inline-div">
                            <a class="btn form-control" v-bind:href="'Edit/'+ concert.id">Edit</a>
                            <a class="btn form-control" v-bind:href="'Details/'+ concert.id">Details</a>
                            <a class="btn form-control" v-bind:href="'Delete/'+ concert.id">Delete</a>
                        </div>
                    </div>
             </div>    
                <button class="btn"
                  :disabled="pageNumber === 0" 
                  @click="prevPage">
                  Previous
              </button>
              <button class="btn"
                  :disabled="pageNumber >= pageCount -1" 
                  @click="nextPage">
                  Next
              </button>  
        </div>
  `
});
var page_app = new Vue({
    el: '#page_app',
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