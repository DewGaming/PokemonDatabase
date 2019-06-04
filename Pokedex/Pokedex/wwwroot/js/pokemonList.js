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

$('#search').on('input', function() {
    var val = this.value;
    if($('#pokemonList option').filter(function() {
        return this.value.toUpperCase() === val.toUpperCase();        
    }).length) {
        $('.search-button').trigger("click");
    }
});