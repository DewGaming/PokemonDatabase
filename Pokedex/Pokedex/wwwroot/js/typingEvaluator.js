var primaryTypeID, secondaryTypeID, generationId, updateIDs = function () {
    primaryTypeID = $('.primaryList > select').val();
    secondaryTypeID = $('.secondaryList > select').val();
    generationID = $('.generationList > select').val();
}, checkTypings = function () {
    if (primaryTypeID != $('.primaryList > select').val() || secondaryTypeID != $('.secondaryList > select').val() || generationID != $('.generationList > select').val()) {
        updateIDs();
        $('.secondaryList option').each(function() {
            if(!$(this).is(':visible'))
            {
                $(this).css('display', 'block');
            }
        });

        $('.secondaryList option[value="' + primaryTypeID + '"]').css('display', 'none');

        if (primaryTypeID == '0' && secondaryTypeID != '0') {
            if ($('.secondaryList > select').val() == '100') {
                $('.primaryList > select').val('0');
            }
            else {
                $('.primaryList > select').val(secondaryTypeID);
            }

            $('.secondaryList > select').val('0');
            updateIDs();
        }
        else if (primaryTypeID == secondaryTypeID) {
            $('.secondaryList > select').val('0');
            updateIDs();
        }

        if (primaryTypeID != '0' && secondaryTypeID != '100') {
            $('.effectivenessChart').empty();

            $('.effectivenessChart').load('/get-typing-evaluator-chart/', { 'primaryTypeID': primaryTypeID, 'secondaryTypeID': secondaryTypeID }, function() {
                if($('.typing-table-strong').children().length > 0)
                {
                    $(".StrongAgainst").css("display", "block");
                }
                else
                {
                    $(".StrongAgainst").css("display", "none");
                }

                if($('.typing-table-weak').children().length > 0)
                {
                    $(".WeakAgainst").css("display", "block");
                }
                else
                {
                    $(".WeakAgainst").css("display", "none");
                }

                if($('.typing-table-immune').children().length > 0)
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
    }
}, grabPokemon = function () {
    if (primaryTypeID != '') {
        $('.pokemonWithTyping').css('display', 'none');
        $('.effectivenessChart').css('display', 'none');
        $('.pokemonList').empty();
        $('.pokemonList').load('/get-pokemon-by-typing/', { 'primaryTypeID': primaryTypeID, 'secondaryTypeID': secondaryTypeID, 'generationId': generationID }, function () {
            if ($('.pokemonList').children().length > 0) {
                $('.pokemonWithTyping').css('display', 'block');
            }
            else {
                $('.pokemonWithTyping').css('display', 'none');
            }

            if($('.secondaryList > select').val() == '')
            {
                $('.effectivenessChart').css('display', 'flex');
            }
        });
    }
    else {
        $('.pokemonWithTyping').css('display', 'none');
    }
}

$(function () {
    checkTypings();
    grabPokemon();
});

$('.typingSelectList').on('change', function () {
    checkTypings();
    grabPokemon();
});