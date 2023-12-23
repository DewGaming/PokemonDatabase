var grabPokemonDropdown = function (gameId) {
    $('.eggGroupSelectList').empty();
    $('.pokemonWithEggGroup').empty();
    if (gameId != "" && gameId != 0) {
        $(".overlay").fadeIn(300);
        if (gameId != "") {
            $('.eggGroupSelectList').load('/get-pokemon-by-egg-group-dropdown/', { 'gameId': gameId }, function () {
                setTimeout(function () {
                    $(".overlay").fadeOut(300);

                    $(".eggGroupSelectList").select2();
                });
            });
        }
    }
}, grabPokemonResults = function (pokemonId, gameId) {
    $('.pokemonWithEggGroup').empty();
    if (pokemonId != "" && pokemonId != 0) {
        $(".overlay").fadeIn(300);
        $('.pokemonWithEggGroup').empty();
        if (pokemonId != "") {
            $('.pokemonWithEggGroup').load('/get-pokemon-by-egg-group/', { 'pokemonId': pokemonId, 'gameId': gameId }, function () {
                setTimeout(function () {
                    $(".overlay").fadeOut(300);
                });
            });
        }
    }
}

$(function () {
    grabPokemonDropdown($('.gameSelectList option:selected').val());
    $('.gameSelectList').select2();
});

$(".gameSelectList").on('change', function () {
    grabPokemonDropdown($('.gameSelectList option:selected').val());
});

$(".eggGroupSelectList").on('change', function () {
    grabPokemonResults($('.eggGroupSelectList option:selected').val(), $('.gameSelectList option:selected').val());
});