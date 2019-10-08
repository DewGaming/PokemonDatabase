$('.pokemonTeamButton').on("click", function() {
    var pokemonTeamId = $(this).attr('id')
    $.ajax({
        url: '/export-user-pokemon-team/',
        method: 'POST',
        data: { 'pokemonTeamId': pokemonTeamId }
    })
    .done(function(data) {
        alert("Copy This For Pokemon Showdown: \n\n" + data);
    })
    .fail(function(jqXHR) {
        alert(jqXHR.statusText);
    });
});