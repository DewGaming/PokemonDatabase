var grabPokemon = function (pokemonId) {
    $('.pokemonWithEggGroup').empty();
    if (pokemonId != "") {
        $('.pokemonWithEggGroup').load('/get-pokemon-by-egg-group/', { 'pokemonId': pokemonId });
        
        setTimeout(function () {
            $(".overlay").fadeOut(300);
        }, 500);
    }
}

jQuery(function ($) {
    $(document).ajaxSend(function () {
        $(".overlay").fadeIn(300);
    })
});

$(function () {
    grabPokemon($('.eggGroupSelectList option:selected').val());
});

$(".eggGroupSelectList").on('change', function () {
    grabPokemon($('.eggGroupSelectList option:selected').val());
});