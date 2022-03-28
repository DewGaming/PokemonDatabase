var grabPokemon = function (formId) {
    $('.pokemonWithAlternateForm').empty();
    if (formId != "") {
        $('.pokemonWithAlternateForm').load('/get-pokemon-by-alternate-form/', { 'formId': formId });
    }
}

$(function () {
    grabPokemon($('.formSelectList option:selected').val());
});

$(".formSelectList").on('change', function () {
    grabPokemon($('.formSelectList option:selected').val());
});