$('.generationDropDown').on('change', function() {
    $('.generationDropDown option[value=""]').remove();
    var generationId = this.value;
    var selection = $('.pokemonDropDown').val();
    var gen5Date = new Date('October 7, 2012');
    
    $.ajax({
        url: '/update-pokemon-list/' + generationId,
        method: "POST"
    })
    .done(function(data) {
        var latestGenDate = new Date(Date.parse(data.generation.releaseDate));

        $('.pokemonDropDown').empty();

        $(data.pokemonList).each(function(index, pokemon) {
            $('.pokemonDropDown').append($("<option></option>").val(pokemon.id).text(pokemon.name.replace('_', ' ')));
        });

        if (data.pokemonList.length >= selection && selection != null)
        {
            $('.pokemonDropDown').val(selection);
        }
        else
        {
            $('.pokemonDropDown').val('1');
        }

        if (latestGenDate >= gen5Date)
        {
            $('.shinyCharmCheckBox').removeClass("hidden");
        }
        else
        {
            $('.shinyCharmCheckBox').addClass("hidden");
        }
    })
    .fail( function() {
        alert("Update Failed!");
    });
});