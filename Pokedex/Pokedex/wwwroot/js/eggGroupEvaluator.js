var primaryEggGroupID, secondaryEggGroupID, updateIDs = function(){
    primaryEggGroupID = $(".primaryList > select").val();
    secondaryEggGroupID = $(".secondaryList > select").val();
}, checkEggGroups = function(){
    if(primaryEggGroupID != $(".primaryList > select").val() || secondaryEggGroupID != $(".secondaryList > select").val())
    {
        updateIDs();
        if(primaryEggGroupID == "0" && secondaryEggGroupID != "0")
        {
            $(".primaryList > select").val(secondaryEggGroupID);
            $(".secondaryList > select").val("0");
            updateIDs();
        }
        else if(primaryEggGroupID == secondaryEggGroupID)
        {
            $(".secondaryList > select").val("0");
            updateIDs();
        }
    }
}, fillList = function(data){
    var eggGroups = data.allPokemonWithEggGroups, pokemon = data.allPokemon;
    for(i = 0; i < eggGroups.length; i += 1){
        var pokemonDiv = $('<div>'), pokemonLink = $('<a>'), pokemonImage = $('<img>'), pokemonNameDiv = $('<div>'), pokemonName = $('<span>');
        $(pokemonLink).attr('href', data.appConfig.webUrl + '/pokemon/' + eggGroups[i].pokemon.name.replace(': ', '_').replace(' ', '_').toLowerCase());
        $(pokemonImage).addClass('pokemonListPicture').attr('title', eggGroups[i].pokemon.name.replace('_', ' ')).attr('src', data.appConfig.webUrl + '/images/pokemon/' + eggGroups[i].pokemon.id + '.png');
        $(pokemonLink).append(pokemonImage);
        $(pokemonName).addClass('pokemonName').text(pokemon[i].name.replace('_', ' '));
        $(pokemonNameDiv).append(pokemonName);
        $(pokemonDiv).addClass(eggGroups[i].pokemon.name).append(pokemonLink).append(pokemonNameDiv);
        $('.pokemonList').append(pokemonDiv);
    };
}, grabPokemon = function(){
    if(primaryEggGroupID != "")
    {
        $.ajax({
            url: '/get-pokemon-by-egg-group/',
            method: 'POST',
            data: { 'primaryEggGroupID': primaryEggGroupID, 'secondaryEggGroupID': secondaryEggGroupID }
        })
        .done(function(data) {
            pokemonList = data.allPokemonWithEggGroups;
            if(pokemonList.length != 0)
            {
                $(".pokemonList").empty();

                fillList(data);

                $(".pokemonWithEggGroup").css("display", "block");
            }
            else
            {
                $(".pokemonWithEggGroup").css("display", "none");
            }
        })
        .fail( function() {
            alert("Failed To Get Egg Group Chart!");
        });
    }
    else
    {
        $(".pokemonWithEggGroups").css("display", "none");
    }
}

$(function() {
    checkEggGroups();
    grabPokemon();
});

$(".eggGroupSelectList").on('change', function(){
    checkEggGroups();
    grabPokemon();
});