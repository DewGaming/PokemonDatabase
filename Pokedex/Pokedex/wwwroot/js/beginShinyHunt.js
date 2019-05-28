$('.generationDropDown').on('change', function() {
    $('.generationDropDown option[value=""]').remove();
    var generationId = this.value;
    var selection = $('.pokemonDropDown').val();
    var gen5Date = new Date(2012, 10, 7);
    
    $.ajax({
        url: '/update_pokemon_list/' + generationId,
        method: "POST"
    })
    .done(function(data) {
        var latestGenDate = new Date(data[data.length - 1].generation.releaseDate);

        $('.pokemonDropDown').empty();

        $(data).each(function(index, pokemon) {
            $('.pokemonDropDown').append($("<option></option>").val(pokemon.id).text(pokemon.name.replace('_', ' ')));
        });

        if (data.length >= selection && selection != null)
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