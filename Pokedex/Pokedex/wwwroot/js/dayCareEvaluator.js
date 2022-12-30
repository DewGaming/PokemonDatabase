var grabPokemon = function (pokemonId) {
    if (pokemonId != "" && pokemonId != 0) {
        $(".overlay").fadeIn(300);
        $('.pokemonWithEggGroup').empty();
        if (pokemonId != "") {
            $('.pokemonWithEggGroup').load('/get-pokemon-by-egg-group/', { 'pokemonId': pokemonId }, function () {
                setTimeout(function () {
                    $(".overlay").fadeOut(300);
                });
            });
        }
    }
}

$(function () {
    grabPokemon($('.eggGroupSelectList option:selected').val());
});

$(".eggGroupSelectList").on('change', function () {
    grabPokemon($('.eggGroupSelectList option:selected').val());
});