var grabAbilities = function() {
    $.ajax({
        url: '/get-pokemon-abilities/',
        method: "POST",
        data: { 'pokemonId': $('#PokemonTeamDetail_PokemonId').val() }
    })
    .done(function(data) {
        $('#PokemonTeamDetail_AbilityId').empty();
        $.each(data, function(index, item) {
            $('#PokemonTeamDetail_AbilityId').append($('<option>').val(item.id).text(item.name));
        });
    })
    .fail(function() {
        alert("Failed to grab abilities!");
    });
}

$(document).ready(function() {
    grabAbilities();
});

$('#PokemonTeamDetail_PokemonId').on('change', function(){
    grabAbilities();
});