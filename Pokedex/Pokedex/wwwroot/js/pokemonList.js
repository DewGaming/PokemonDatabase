$(document).ready(function() {
    $.ajax({
        url: '/get-pokemon-list/',
        method: "POST"
    })
    .done(function(data) {
        $(data).each(function() {
            $('datalist#pokemonList').append($('<option>', {
                value: this.name
            }));
        });
    });
});