var edit_concert_app = new Vue({
    el: '#edit_concert_app',
    data: {
        selected: '',
        type: null
    },
    mounted() {
        try {
            this.type = document.getElementById('#typeDiv').getAttribute('data-value');
        } catch (ex) {
            console.log(ex);
        };        
    }
});