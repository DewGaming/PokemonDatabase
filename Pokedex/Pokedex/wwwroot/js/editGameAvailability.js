function updateGameAvailability(pokemonId) {
    var games = [];
    $('.gameAvailability input').each(function () {
        if ($(this).is(':checked')) {
            games.push(this.value);
        }
    });

    $.ajax({
        url: '/update-game-availability/',
        method: "POST",
        data: { "pokemonId": pokemonId, "games": games }
    })
        .done(function (data) {
            window.location = data;
        })
        .fail(function () {
            alert("Update Failed!");
        });
}