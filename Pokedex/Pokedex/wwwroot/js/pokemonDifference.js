var grabGames = function (gameList) {
    $(".overlay").fadeIn(300);
    var gameId = $('#' + gameList).val(), pokemonId;

    if (gameList == 'FirstGame') {
        pokemonId = $('#FirstPokemon').val();
    } else if (gameList == 'SecondGame') {
        pokemonId = $('#SecondPokemon').val();
    }

    $.ajax({
        url: '/get-games-by-pokemon/',
        method: "POST",
        data: { 'pokemonId': pokemonId }
    })
        .done(function (data) {
            $('#' + gameList).empty();
            $.each(data, function (index, item) {
                $('#' + gameList).append($('<option>').val(item.id).text(item.name));
            });

            if (gameId) {
                $('#' + gameList + ' option').each(function () {
                    if (this.value == gameId) {
                        $('#' + gameList).val(gameId);
                    }
                });
            }

            if (gameList == 'FirstGame') {
                $('.firstPokemon .gameList').removeClass('hide');
            } else if (gameList == 'SecondGame') {
                $('.secondPokemon .gameList').removeClass('hide');
            }

            if ($('#FirstPokemon').val() && $('#SecondPokemon').val()) {
                $('.compareButton.hide').removeClass('hide');
            }

            $('#' + gameList).select2();
            $(".overlay").fadeOut(300);
        })
        .fail(function () {
            if (isLocalhost) {
                alert("Failed to grab games!");
            }

            $(".overlay").fadeOut(300);
        });
}, comparePokemon = function () {
    if ($('#FirstPokemon').val() && $('#SecondPokemon').val()) {
        $(".overlay").fadeIn(300);
        $('.pokemonDifferences').removeClass('active');
        $('.pokemonDifferences').empty();
        $('.pokemonDifferences').load('/get-pokemon-differences/', { 'firstPokemonId': $('#FirstPokemon').val(), 'firstGameId': $('#FirstGame').val(), 'secondPokemonId': $('#SecondPokemon').val(), 'secondGameId': $('#SecondGame').val() }, function () {
            $('.pokemonDifferences').addClass('active');
            $('.overlay').fadeOut(300);
        });
    } else {
        $('.compareButton').addClass('hide');
    }
}

$(function () {
    $(".firstPokemonSelectList").select2();
    $(".secondPokemonSelectList").select2();
    if ($('#FirstPokemon').val()) {
        grabGames('FirstGame');
    }
    if ($('#SecondPokemon').val()) {
        grabGames('SecondGame');
    }
});

$('#FirstPokemon').on('change', function () {
    grabGames('FirstGame');
});

$('#SecondPokemon').on('change', function () {
    grabGames('SecondGame');
});