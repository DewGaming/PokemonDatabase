var abilityId = 0, gender = "", grabAbilities = function() {
    $.ajax({
        url: '/get-pokemon-abilities/',
        method: "POST",
        data: { 'pokemonId': $('#PokemonTeamDetail_PokemonId').val(), "gender": $('#PokemonTeamDetail_Gender').val() }
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
}, refreshGenders = function() {
    $.ajax({
        url: '/get-pokemon-genders/',
        method: "POST",
        data: { 'pokemonId': $('#PokemonTeamDetail_PokemonId').val() }
    })
    .done(function(data) {
        $('#PokemonTeamDetail_Gender').empty();
        $.each(data, function(index, item) {
            $('#PokemonTeamDetail_Gender').append($('<option>').text(item));
        });
        if(gender != "")
        {
            $('#PokemonTeamDetail_Gender').val(gender);
        }
        if($.inArray("", data) && data.length == 1)
        {
            $('#PokemonTeamDetail_Gender').parent().hide();
        }
        else
        {
            $('#PokemonTeamDetail_Gender').parent().show();
        }
    })
    .fail(function() {
        alert("Failed to grab genders!");
    });
}

$(document).ready(function() {
    abilityId = $('#PokemonTeamDetail_AbilityId').val();
    gender = $('#PokemonTeamDetail_Gender').val();
    refreshGenders();
    grabAbilities();
});

$('#PokemonTeamDetail_PokemonId').on('change', function(){
    abilityId = 0;
    gender = "";
    refreshGenders();
    grabAbilities();
});

$('#PokemonTeamDetail_Gender').on('change', function(){
    if($('#PokemonTeamDetail_PokemonId').val() == "678")
    {
        abilityId = 0;
        gender = "";
        grabAbilities();
    }
});