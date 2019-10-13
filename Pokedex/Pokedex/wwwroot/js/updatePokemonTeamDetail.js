var abilityId = 0, grabAbilities = function() {
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
        if(abilityId != 0)
        {
            $('#PokemonTeamDetail_AbilityId').val(abilityId);
        }
    })
    .fail(function() {
        alert("Failed to grab abilities!");
    });
}

$(document).ready(function() {
    abilityId = $('#PokemonTeamDetail_AbilityId').val();
    grabAbilities();
});

$('#PokemonTeamDetail_PokemonId').on('change', function(){
    abilityId = 0;
    grabAbilities();
});