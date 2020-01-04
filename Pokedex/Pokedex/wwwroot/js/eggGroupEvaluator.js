var fillList = function (data) {
    var eggGroups = data.allPokemonWithEggGroups, pokemon = data.allPokemon;
    for (i = 0; i < eggGroups.length; i += 1) {
        var pokemonDiv = $('<div>'), pokemonLink = $('<a>'), pokemonImage = $('<img>'), pokemonNameDiv = $('<div>'), pokemonName = $('<span>');
        $(pokemonLink).attr('href', data.appConfig.webUrl + '/pokemon/' + eggGroups[i].pokemon.name.replace(': ', '_').replace(' ', '_').toLowerCase());
        $(pokemonImage).addClass('pokemonListPicture').attr('title', eggGroups[i].pokemon.name.replace('_', ' ')).attr('src', data.appConfig.webUrl + '/images/pokemon/' + eggGroups[i].pokemon.id + '.png');
        $(pokemonLink).append(pokemonImage);
        $(pokemonName).addClass('pokemonName').text(pokemon[i].name.replace('_', ' '));
        $(pokemonNameDiv).append(pokemonName);
        $(pokemonDiv).addClass(eggGroups[i].pokemon.name).append(pokemonLink).append(pokemonNameDiv);
        $('.pokemonList').append(pokemonDiv);
    };
}, grabPokemon = function (pokemonId) {
    $('.pokemonWithEggGroup').empty();
    if (pokemonId != "") {
        $('.pokemonWithEggGroup').load('/get-pokemon-by-egg-group/', { 'pokemonId': pokemonId });
    }
}

$(function () {
    grabPokemon($('.eggGroupSelectList option:selected').val());
});

$(".eggGroupSelectList").on('change', function () {
    grabPokemon($('.eggGroupSelectList option:selected').val());
});