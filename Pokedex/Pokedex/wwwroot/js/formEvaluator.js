var grabPokemon = function (formId) {
    if (formId != "" && formId != 0) {
        $(".overlay").fadeIn(300);
        $('.pokemonWithAlternateForm').empty();
        if (formId != "") {
            $('.pokemonWithAlternateForm').load('/get-pokemon-by-alternate-form/', { 'formId': formId }, function () {
                setTimeout(function () {
                    $(".overlay").fadeOut(300);
                });
            });
        }
    }
}

$(function () {
    grabPokemon($('.formSelectList option:selected').val());
});

$(".formSelectList").on('change', function () {
    grabPokemon($('.formSelectList option:selected').val());
});