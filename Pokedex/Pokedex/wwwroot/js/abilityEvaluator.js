var grabPokemon = function () {
    if ($('.abilityList > select').val() != '') {
        $('.pokemonWithAbility').css('display', 'none');
        $('.pokemonWithAbility').empty();
        $('.pokemonWithAbility').load('/get-pokemon-by-ability/', { 'abilityId': $('.abilityList > select').val(), 'generationID': $('.generationList > select').val() }, function () {
            $('.pokemonWithAbility').css('display', 'block');
            if ($('.grid-container').children().length > 0) {
                $('.pokemonList').css('display', 'block');
                $('.noPokemon').css('display', 'none');
            }
            else {
                $('.pokemonList').css('display', 'none');
                $('.noPokemon').css('display', 'block');
            }
        });
    }
    else {
        $('.pokemonWithAbility').css('display', 'none');
    }
}, grabAbilities = function (generationID) {
    var abilityId = $('.abilityList > select').val();
    $('.abilityList').load('/get-abilities-by-generation/', { 'generationID': generationID }, function () {
        if ($('.abilityList option[value=' + abilityId + ']').length != 0)
        {
            $('.abilityList > select').val(abilityId);
        }
        else
        {
            $('.abilityList > select').val(0);
        }

        $('.typingSelectList').off();

        $('.typingSelectList').on('change', function () {
            grabPokemon();
        });
    });
};

$(function () {
    $('.generationList > select').val($('.generationList option:last-child').val());
    grabAbilities($('.generationList > select').val());
});

$(".generationSelectList").on('change', function () {
    grabAbilities($('.generationList > select').val());
    grabPokemon();
});