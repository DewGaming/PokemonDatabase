var grabPokemon = function () {
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

            var shinyLink = $('.webUrl').prop('name') + $('.shinyUrl').prop('name') + $('#PokemonId').val() + '.png';
            $('.pokemonShinyImage').prop('src', shinyLink);

            $('.hide').each(function () {
                $(this).removeClass('hide');
            })
            
            $(".overlay").fadeOut(300);
        })
        .fail(function () {
            alert("Failed to grab pokemon!");
            
            $(".overlay").fadeOut(300);
        });
}, grabHuntingMethod = function () {
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
        })
        .fail(function () {
            alert("Failed to grab hunting methods!");
        });
}

$('#GameId').on('change', function () {
    grabPokemon();
    grabHuntingMethod();
});

$('#PokemonId').on('change', function () {
    var shinyLink = $('.webUrl').prop('name') + $('.shinyUrl').prop('name') + $('#PokemonId').val() + '.png';
    $('.pokemonShinyImage').prop('src', shinyLink);
});