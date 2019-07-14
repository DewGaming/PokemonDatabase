var primaryTypeID, secondaryTypeID, updateIDs = function(){
    primaryTypeID = $(".primaryList > select").val();
    secondaryTypeID = $(".secondaryList > select").val();
}, fillTables = function(data)
{
   fillStrong(data);
   fillWeak(data);
   fillImmune(data); 
}, fillStrong = function(data)
{
    if(data.strongAgainst.length == 0)
    {
        $(".StrongAgainst").css("display", "none");
    }
    else
    {
        $.each(data.strongAgainst, function(input, type){
            var rowTag = $("<tr>"), dataTag = $("<td>"), iconTag = $("<div>"), quadTag = $("<div>");
            $(quadTag).addClass("pokemon-type quad-icon");

            $(iconTag).addClass("pokemon-type type-icon type-" + type.toLowerCase());
            $(iconTag).text(type);

            if(~type.indexOf("Quad"))
            {
                $(iconTag).addClass("pokemon-type type-icon type-" + type.toLowerCase().substr(0, type.indexOf(' ')));
                $(iconTag).text(type.substr(0, type.indexOf(' ')));
                $(quadTag).addClass("quad-resist");
                $(quadTag).text("Quad Resist");
            }

            $(dataTag).append(iconTag);
            if(~type.indexOf("Quad"))
            {
                $(dataTag).append(quadTag);
            }

            $(rowTag).append(dataTag);

            $(".typing-table-strong").append(rowTag);
        });
        
        $(".StrongAgainst").css("display", "block");
    }
}, fillWeak = function(data)
{
    $.each(data.weakAgainst, function(input, type){
        var rowTag = $("<tr>"), dataTag = $("<td>"), iconTag = $("<div>"), quadTag = $("<div>");
        $(quadTag).addClass("pokemon-type quad-icon");

        $(iconTag).addClass("pokemon-type type-icon type-" + type.toLowerCase());
        $(iconTag).text(type);

        if(~type.indexOf("Quad"))
        {
            $(iconTag).addClass("pokemon-type type-icon type-" + type.toLowerCase().substr(0, type.indexOf(' ')));
            $(iconTag).text(type.substr(0, type.indexOf(' ')));
            $(quadTag).addClass("quad-weak");
            $(quadTag).text("Quad Weak");
        }

        $(dataTag).append(iconTag);
        if(~type.indexOf("Quad"))
        {
            $(dataTag).append(quadTag);
        }

        $(rowTag).append(dataTag);

        $(".typing-table-weak").append(rowTag);
    });
}, fillImmune = function(data)
{
    if(data.immuneTo.length == 0)
    {
        $(".ImmuneTo").css("display", "none");
    }
    else
    {
        $.each(data.immuneTo, function(input, type){
            var rowTag = $("<tr>"), dataTag = $("<td>"), iconTag = $("<div>"), quadTag = $("<div>");
            $(quadTag).addClass("pokemon-type quad-icon");
    
            $(iconTag).addClass("pokemon-type type-icon type-" + type.toLowerCase());
            $(iconTag).text(type);    
            $(dataTag).append(iconTag);
    
            $(rowTag).append(dataTag);
    
            $(".typing-table-immune").append(rowTag);
        });
        
        $(".ImmuneTo").css("display", "block");
    }
}, checkTypings = function(){
    if(primaryTypeID != $(".primaryList > select").val() || secondaryTypeID != $(".secondaryList > select").val())
    {
        updateIDs();
        if(primaryTypeID == "" && secondaryTypeID != "")
        {
            $(".primaryList > select").val(secondaryTypeID);
            $(".secondaryList > select").val("");
            updateIDs();
        }
        else if(primaryTypeID == secondaryTypeID)
        {
            $(".secondaryList > select").val("");
            updateIDs();
        }

        if(primaryTypeID != "")
        {
            $.ajax({
                url: '/get-typing-effectiveness/',
                method: 'POST',
                data: { 'primaryTypeID': primaryTypeID, 'secondaryTypeID': secondaryTypeID }
            })
            .done(function(data) {
                typingList = data;
                
                $(".typing-table").each(function(index) {
                    $(this).children().remove()
                });

                fillTables(data);

                $(".effectivenessChart").css("display", "flex");
            })
            .fail( function() {
                alert("Failed To Get Effectiveness Chart!");
            });
        }
        else
        {
            $(".effectivenessChart").css("display", "none");
        }
    }
}, fillList = function(data){
    $.each(data, function(input, pokemon){
        var pokemonDiv = $('<div>'), pokemonLink = $('<a>'), pokemonImage = $('<img>'), pokemonNameDiv = $('<div>'), pokemonName = $('<span>');
        $(pokemonLink).attr('href', 'https://pokedex.dewgaming.a2hosted.com/' + pokemon.pokemon.name.replace(': ', '_').replace(' ', '_').toLowerCase());
        $(pokemonImage).addClass('pokemonListPicture').attr('title', pokemon.pokemon.name.replace('_', ' ')).attr('src', 'https://pokedex.dewgaming.a2hosted.com/images/pokemon/' + pokemon.pokemon.id + '.png');
        $(pokemonLink).append(pokemonImage);
        $(pokemonName).addClass('pokemonName').text('#' + pokemon.pokemon.id + ' ' + pokemon.pokemon.name.replace('_', ' '));
        $(pokemonNameDiv).append(pokemonName);
        $(pokemonDiv).addClass(pokemon.pokemon.name).append(pokemonLink).append(pokemonNameDiv);
        $('.pokemonList').append(pokemonDiv);
    });
}, grabPokemon = function(){
    if(primaryTypeID != "")
    {
        $.ajax({
            url: '/get-pokemon-by-typing/',
            method: 'POST',
            data: { 'primaryTypeID': primaryTypeID, 'secondaryTypeID': secondaryTypeID }
        })
        .done(function(data) {
            if(data.length != 0)
            {
                pokemonList = data;
                
                $(".pokemonList").empty();

                fillList(data);

                $(".pokemonWithTyping").css("display", "block");
            }
            else
            {
                $(".pokemonWithTyping").css("display", "none");
            }
        })
        .fail( function() {
            alert("Failed To Get Effectiveness Chart!");
        });
    }
    else
    {
        $(".pokemonWithTyping").css("display", "none");
    }
}

$(function() {
    checkTypings();
    grabPokemon();
});

$(".typingSelectList").on('change', function(){
    checkTypings();
    grabPokemon();
});