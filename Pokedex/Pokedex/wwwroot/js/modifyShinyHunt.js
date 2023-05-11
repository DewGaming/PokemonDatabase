var grabPokemon = function (pokemonId) {
    $(".overlay").fadeIn(300);

    $.ajax({
        url: '/get-shiny-huntable-pokemon-by-game/',
        method: "POST",
        data: { 'gameId': $('#GameId').val() }
    })
        .done(function (data) {
            $('#PokemonId').empty();
            $.each(data, function (index, item) {
                $('#PokemonId').append($('<option>').val(item.id).text(item.name));
            });

            if (pokemonId !== "") {
                $('#PokemonId option').each(function () {
                    if (this.value == pokemonId) {
                        $('#PokemonId').val(pokemonId);
                    }
                });
            }

            $('.pokemonShinyImage').prop('src', $('.webUrl').prop('name') + $('.shinyUrl').prop('name') + $('#PokemonId').val() + '.png');

            $('.hide').not('.gameSpecific').each(function () {
                $(this).removeClass('hide');
            })

            refreshGenders();

            $(".overlay").fadeOut(300);
        })
        .fail(function () {
            alert("Failed to grab pokemon!");

            $(".overlay").fadeOut(300);
        });
}, refreshGenders = function () {
    $.ajax({
        url: '/get-pokemon-genders/',
        method: "POST",
        data: { 'pokemonId': $('#PokemonId').val() }
    })
        .done(function (data) {
            if (data.length == 3) {
                data.shift();
            }
            $('#Gender').empty();
            $.each(data, function (index, item) {
                if (item == "None") {
                    item = "Gender Unknown"
                }
                $('#Gender').append($('<option>').text(item));
            });
        })
        .fail(function () {
            alert("Failed to grab genders!");
        });
}, grabHuntingMethod = function (huntingMethodId) {
    $.ajax({
        url: '/get-hunting-methods/',
        method: "POST",
        data: { 'gameId': $('#GameId').val() }
    })
        .done(function (data) {
            $('#HuntingMethodId').empty();
            $.each(data, function (index, item) {
                $('#HuntingMethodId').append($('<option>').val(item.id).text(item.name));
            });

            if (huntingMethodId !== "") {
                $('#HuntingMethodId option').each(function () {
                    if (this.value == huntingMethodId) {
                        $('#HuntingMethodId').val(huntingMethodId);
                    }
                });
            }

            checkFunctions();
        })
        .fail(function () {
            alert("Failed to grab hunting methods!");
        });
}, checkShinyCharm = function () {
    $.ajax({
        url: '/check-shiny-charm/',
        method: "POST",
        data: { 'gameId': $('#GameId').val(), 'huntingMethodId': $('#HuntingMethodId').val() }
    })
        .done(function (data) {
            if (data) {
                if ($('.shinyCharmCheckbox').hasClass('hide')) {
                    $('.shinyCharmCheckbox').removeClass('hide');
                }
            } else {
                if (!$('.shinyCharmCheckbox').hasClass('hide')) {
                    $('.shinyCharmCheckbox').addClass('hide');
                    $('#HasShinyCharm').prop('checked', false)
                }
            }
        })
        .fail(function () {
            alert("Failed to check shiny charm!");
        });
}, checkAlpha = function () {
    var gameId = $('#GameId').val()
    if (gameId == 37) {
        if ($('.alphaCheckbox').hasClass('hide')) {
            $('.alphaCheckbox').removeClass('hide');
        }
    } else {
        if (!$('.alphaCheckbox').hasClass('hide')) {
            $('.alphaCheckbox').addClass('hide');
            $('#IsAlpha').prop('checked', false)
        }
    }
}, checkCommunityDay = function () {
    var gameId = $('#GameId').val(), huntingMethodId = $('#HuntingMethodId').val()
    if (gameId == 43 && huntingMethodId == 1) {
        if ($('.communityDayCheckbox').hasClass('hide')) {
            $('.communityDayCheckbox').removeClass('hide');
        }
    } else {
        if (!$('.communityDayCheckbox').hasClass('hide')) {
            $('.communityDayCheckbox').addClass('hide');
            $('#DuringCommunityDay').prop('checked', false)
        }
    }
}, checkSparklingPower = function () {
    $.ajax({
        url: '/check-sparkling-power/',
        method: "POST",
        data: { 'gameId': $('#GameId').val(), 'huntingMethodId': $('#HuntingMethodId').val() }
    })
        .done(function (data) {
            if (data) {
                if ($('.sparklingPower').hasClass('hide')) {
                    $('.sparklingPower').removeClass('hide');
                }
            } else {
                if (!$('.sparklingPower').hasClass('hide')) {
                    $('.sparklingPower').addClass('hide');
                    $('#SparklingPowerLevel').val(0)
                }
            }
        })
        .fail(function () {
            alert("Failed to check sparkling power!");
        });
}, checkLure = function () {
    var gameId = $('#GameId').val(), huntingMethodId = $('#HuntingMethodId').val()
    if ((gameId == 16 || gameId == 28) && (huntingMethodId == 1 || huntingMethodId == 16)) {
        if ($('.lureCheckbox').hasClass('hide')) {
            $('.lureCheckbox').removeClass('hide');
        }
    } else {
        if (!$('.lureCheckbox').hasClass('hide')) {
            $('.lureCheckbox').addClass('hide');
            $('#UsingLures').prop('checked', false)
        }
    }
}, checkFunctions = function () {
    checkLure();
    checkAlpha();
    checkShinyCharm();
    checkSparklingPower();
}

$(document).ready(function () {
    var pokemonId = $('#PokemonId').val(), huntingMethodId = $('#HuntingMethodId').val();
    if ($('#GameId').val() !== "") {
        grabPokemon(pokemonId);
        grabHuntingMethod(huntingMethodId);
        checkFunctions();
    }
    
    $("#PokemonId").select2();
});

$('#GameId').on('change', function () {
    var pokemonId = $('#PokemonId').val(), huntingMethodId = $('#HuntingMethodId').val();
    grabPokemon(pokemonId);
    grabHuntingMethod(huntingMethodId);
});

$('#HuntingMethodId').on('change', function () {
    checkFunctions();
});

$('#PokemonId').on('change', function () {
    $('.pokemonShinyImage').prop('src', $('.webUrl').prop('name') + $('.shinyUrl').prop('name') + $('#PokemonId').val() + '.png');
    refreshGenders();
});