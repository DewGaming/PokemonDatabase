var checkTypings = function () {
    $('.secondaryList option').each(function () {
        if (!$(this).is(':visible')) {
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

        $('.effectivenessChart').load('/get-typing-evaluator-chart/', { 'primaryTypeID': $('.primaryList > select').val(), 'secondaryTypeID': $('.secondaryList > select').val(), 'gameID': $('.gameList > select').val() }, function () {
            if ($('.typing-table-strong').children().length > 0) {
                $(".StrongAgainst").css("display", "block");
            }
            else {
                $(".StrongAgainst").css("display", "none");
            }

            if ($('.typing-table-weak').children().length > 0) {
                $(".WeakAgainst").css("display", "block");
            }
            else {
                $(".WeakAgainst").css("display", "none");
            }

            if ($('.typing-table-immune').children().length > 0) {
                $(".ImmuneTo").css("display", "block");
            }
            else {
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
    $(".overlay").fadeIn(300);
    if ($('.primaryList > select').val() != '') {
        $('.pokemonWithTyping').css('display', 'none');
        $('.effectivenessChart').css('display', 'none');
        $('.pokemonList').empty();
        $('.pokemonList').load('/get-pokemon-by-typing/', { 'primaryTypeID': $('.primaryList > select').val(), 'secondaryTypeID': $('.secondaryList > select').val(), 'gameID': $('.gameList > select').val() }, function () {
            if ($('.pokemonList').children().length > 0) {
                $('.pokemonWithTyping').css('display', 'block');
            }
            else {
                $('.pokemonWithTyping').css('display', 'none');
            }

            if ($('.secondaryList > select').val() == '') {
                $('.effectivenessChart').css('display', 'flex');
            }

            setTimeout(function () {
                $(".overlay").fadeOut(300);
            });
        });
    }
    else {
        $('.pokemonWithTyping').css('display', 'none');

        setTimeout(function () {
            $(".overlay").fadeOut(300);
        });
    }
}, grabTypes = function (gameID) {
    var primaryTypeID = $('.primaryList > select').val(), secondaryTypeID = $('.secondaryList > select').val();
    $('.typeLists').load('/get-types-by-game/', { 'gameID': gameID }, function () {
        if ($('.primaryList option[value=' + primaryTypeID + ']').length != 0) {
            $('.primaryList > select').val(primaryTypeID);
        }
        else {
            $('.primaryList > select').val(0);
        }

        if ($('.secondaryList option[value=' + secondaryTypeID + ']').length != 0) {
            $('.secondaryList > select').val(secondaryTypeID);
        }
        else {
            $('.secondaryList > select').val(0);
        }

        $('.typingSelectList').off();

        $('.typingSelectList').on('change', function () {
            grabPokemon();
        });

        grabPokemon();
    });
};

$(function () {
    grabTypes($('.gameList > select').val());
});

$(".gameSelectList").on('change', function () {
    grabTypes($('.gameList > select').val());
});