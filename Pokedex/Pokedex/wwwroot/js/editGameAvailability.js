function updateGameAvailability(gameId) {
    var pokemon = [];
    $('.gameAvailability input').each(function () {
        if ($(this).is(':checked')) {
            pokemon.push(this.value);
        }
    });

    $(".overlay").fadeIn(300);

    $.ajax({
        url: '/update-game-availability/',
        method: "POST",
        data: { "gameId": gameId, "pokemonList": pokemon }
    })
        .done(function (data) {
            window.location = data;
        })
        .fail(function () { 
            setTimeout(function () {
                $(".overlay").fadeOut(300);
            }, 500);

            alert("Update Failed!");
        });

    setTimeout(function () {
        $(".overlay").fadeOut(300);
    }, 500);
}