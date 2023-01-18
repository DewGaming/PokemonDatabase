function updateRegionalDex(gameId, preEvos) {
    var pokemon = [];
    $('.regionalDex input').each(function () {
        if ($(this).is(':checked')) {
            pokemon.push(this.value);
        }
    });

    $(".overlay").fadeIn(300);

    $.ajax({
        url: '/update-regional-dex/',
        method: "POST",
        data: { "gameId": gameId, "pokemonList": pokemon, "addPreEvos": preEvos }
    })
        .done(function (data) {
            window.location = data;
        })
        .fail(function () { 
            setTimeout(function () {
                $(".overlay").fadeOut(300);
            });

            alert("Update Failed!");
        });
}

function updateDLCRegionalDex(gameId, preEvos) {
    var pokemon = [];
    $('.dlcRegionalDex input').each(function () {
        if ($(this).is(':checked')) {
            pokemon.push(this.value);
        }
    });

    $(".overlay").fadeIn(300);

    $.ajax({
        url: '/update-dlc-regional-dex/',
        method: "POST",
        data: { "gameId": gameId, "pokemonList": pokemon, "addPreEvos": preEvos }
    })
        .done(function (data) {
            window.location = data;
        })
        .fail(function () { 
            setTimeout(function () {
                $(".overlay").fadeOut(300);
            });

            alert("Update Failed!");
        });
}