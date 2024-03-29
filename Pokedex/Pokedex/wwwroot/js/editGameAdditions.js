function updateMarkGames(gameId) {
    var mark = [];
    $('.markGameAvailability input').each(function () {
        if ($(this).is(':checked')) {
            mark.push(this.value);
        }
    });

    $(".overlay").fadeIn(300);

    $.ajax({
        url: '/update-mark-games/',
        method: "POST",
        data: { "gameId": gameId, "markList": mark }
    })
        .done(function (data) {
            window.location = data;
        })
        .fail(function () {
            alert("Update Failed!");

            $(".overlay").fadeOut(300);
        });
}

function updatePokeballGames(gameId) {
    var pokeball = [];
    $('.pokeballGameAvailability input').each(function () {
        if ($(this).is(':checked')) {
            pokeball.push(this.value);
        }
    });

    $(".overlay").fadeIn(300);

    $.ajax({
        url: '/update-pokeball-games/',
        method: "POST",
        data: { "gameId": gameId, "pokeballList": pokeball }
    })
        .done(function (data) {
            window.location = data;
        })
        .fail(function () {
            alert("Update Failed!");

            $(".overlay").fadeOut(300);
        });
}