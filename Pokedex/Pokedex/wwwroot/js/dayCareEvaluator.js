var grabPokemon = function (pokemonId) {
    $('.pokemonWithEggGroup').empty();
    if (pokemonId != "") {
        $('.pokemonWithEggGroup').load('/get-pokemon-by-egg-group/', { 'pokemonId': pokemonId });
    }
}

$(function () {
    grabPokemon($('.eggGroupSelectList option:selected').val());
});

$(".eggGroupSelectList").on('change', function () {
    grabPokemon($('.eggGroupSelectList option:selected').val());
});