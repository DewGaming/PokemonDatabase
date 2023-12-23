var grabPokemon = function () {
    if ($('.abilityList > select').val() != '0') {
        $(".overlay").fadeIn(300);
        $('.pokemonWithAbility').css('display', 'none');
        $('.pokemonWithAbility').empty();
        $('.pokemonWithAbility').load('/get-pokemon-by-ability/', { 'abilityId': $('.abilityList > select').val(), 'gameId': $('.gameList > select').val() }, function () {
            $('.pokemonWithAbility').css('display', 'block');
            if ($('.grid-container').children().length > 0) {
                $('.pokemonList').css('display', 'block');
                $('.noPokemon').css('display', 'none');
            }
            else {
                $('.pokemonList').css('display', 'none');
                $('.noPokemon').css('display', 'block');
            }
      
            setTimeout(function () {
              $(".overlay").fadeOut(300);
            });
        });
    }
    else {
        $('.pokemonWithAbility').css('display', 'none');
    }
}, grabAbilities = function (gameId) {
    var abilityId = $('.abilityList > select').val();
    $('.abilityList > select').val(0);
    $('.abilityList').load('/get-abilities-by-game/', { 'gameId': gameId }, function () {
        if ($('.abilityList option[value=' + abilityId + ']').length != 0) {
            $('.abilityList > select').val(abilityId);
        }

        $('.abilitySelectList').off();

        $('.abilitySelectList').on('change', function () {
            grabPokemon();
        });

        $(".abilitySelectList").select2();

        grabPokemon();
    });
};

$(function () {
    grabAbilities($('.gameList > select').val());
    $('.gameSelectList').select2();
});

$(".gameSelectList").on('change', function () {
    grabAbilities($('.gameList > select').val());
    grabPokemon();
});