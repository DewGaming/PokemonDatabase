var grabPokemon = function (pokemonId) {
    $(".overlay").fadeIn(300);

    $.ajax({
        url: '/get-pokemon-by-game/',
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

            var image = new Image();
            image.src = $('.webUrl').prop('name') + $('.shinyUrl').prop('name') + $('#PokemonId').val() + '.png';
            image.onload = function () {
                if (this.width > 0) {
                    $('.pokemonShinyImage').prop('src', $('.webUrl').prop('name') + $('.shinyUrl').prop('name') + $('#PokemonId').val() + '.png');
                }
            }
            image.onerror = function () {
                $('.pokemonShinyImage').prop('src', $('.webUrl').prop('name') + $('.officialUrl').prop('name') + $('#PokemonId').val() + '.png');
            }

            $('.hide').not('.gameSpecific').each(function () {
                $(this).removeClass('hide');
            })

            $(".overlay").fadeOut(300);
        })
        .fail(function () {
            alert("Failed to grab pokemon!");

            $(".overlay").fadeOut(300);
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
                if ($('.shinyCharm').hasClass('hide')) {
                    $('.shinyCharm').removeClass('hide');
                }
            } else {
                if (!$('.shinyCharm').hasClass('hide')) {
                    $('.shinyCharm').addClass('hide');
                    $('#HasShinyCharm').prop('checked', false)
                }
            }
        })
        .fail(function () {
            alert("Failed to check shiny charm!");
        });
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
}

$(document).ready(function () {
    var pokemonId = $('#PokemonId').val(), huntingMethodId = $('#HuntingMethodId').val();
    if ($('#GameId').val() !== "") {
        grabPokemon(pokemonId);
        grabHuntingMethod(huntingMethodId);
        checkShinyCharm();
        checkSparklingPower();
    }
});

$('#GameId').on('change', function () {
    var pokemonId = $('#PokemonId').val(), huntingMethodId = $('#HuntingMethodId').val();
    grabPokemon(pokemonId);
    grabHuntingMethod(huntingMethodId);
    checkShinyCharm();
    checkSparklingPower();
});

$('#HuntingMethodId').on('change', function () {
    checkSparklingPower();
    checkShinyCharm();
});

$('#PokemonId').on('change', function () {
    var image = new Image();
    image.src = $('.webUrl').prop('name') + $('.shinyUrl').prop('name') + $('#PokemonId').val() + '.png';
    image.onload = function () {
        if (this.width > 0) {
            $('.pokemonShinyImage').prop('src', $('.webUrl').prop('name') + $('.shinyUrl').prop('name') + $('#PokemonId').val() + '.png');
        }
    }
    image.onerror = function () {
        $('.pokemonShinyImage').prop('src', $('.webUrl').prop('name') + $('.officialUrl').prop('name') + $('#PokemonId').val() + '.png');
    }
});