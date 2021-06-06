var checkTypings = function () {
    $('.secondaryList option').each(function() {
        if (!$(this).is(':visible'))
        {
            $(this).css('display', 'block');
        }
    });

    $('.secondaryList option[value="' + $('.primaryList > select').val() + '"]').css('display', 'none');

    if ($('.primaryList > select').val() == '0' && $('.secondaryList > select').val() != '0') {
        if ($('.secondaryList > select').val() == '100') {
            $('.primaryList > select').val('0');
        }
        else {
            $('.primaryList > select').val($('.secondaryList > select').val());
        }

        $('.secondaryList > select').val('0');
    }
    else if ($('.primaryList > select').val() == $('.secondaryList > select').val()) {
        $('.secondaryList > select').val('0');
    }

    if ($('.primaryList > select').val() != '0' && $('.secondaryList > select').val() != '100') {
        $('.effectivenessChart').empty();

        $('.effectivenessChart').load('/get-typing-evaluator-chart/', { 'primaryTypeID': $('.primaryList > select').val(), 'secondaryTypeID': $('.secondaryList > select').val(), 'generationID': $('.generationList > select').val() }, function() {
            if ($('.typing-table-strong').children().length > 0)
            {
                $(".StrongAgainst").css("display", "block");
            }
            else
            {
                $(".StrongAgainst").css("display", "none");
            }

            if ($('.typing-table-weak').children().length > 0)
            {
                $(".WeakAgainst").css("display", "block");
            }
            else
            {
                $(".WeakAgainst").css("display", "none");
            }

            if ($('.typing-table-immune').children().length > 0)
            {
                $(".ImmuneTo").css("display", "block");
            }
            else
            {
                $(".ImmuneTo").css("display", "none");
            }

            $('.effectivenessChart').css('display', 'flex');
        });
    }
    else {
        $('.effectivenessChart').css('display', 'none');
    }
}, grabPokemon = function () {
    checkTypings();
    if ($('.abilityList > select').val() != '') {
        $('.pokemonWithAbility').css('display', 'none');
        $('.pokemonWithAbility').empty();
        $('.pokemonWithAbility').load('/get-pokemon-by-ability/', { 'abilityId': $('.abilityList > select').val(), 'generationID': $('.generationList > select').val() }, function () {
            if ($('.pokemonList').children().length > 0) {
                $('.pokemonWithAbility').css('display', 'block');
            }
            else {
                $('.pokemonWithAbility').css('display', 'none');
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