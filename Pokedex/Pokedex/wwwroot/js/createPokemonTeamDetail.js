var grabAbilities = function() {
    $.ajax({
        url: '/get-pokemon-abilities/',
        method: "POST",
        data: { 'pokemonId': $('#PokemonId').val(), "gender": $('#Gender').val() }
    })
    .done(function(data) {
        $('#AbilityId').empty();
        $.each(data, function(index, item) {
            $('#AbilityId').append($('<option>').val(item.id).text(item.name));
        });
    })
    .fail(function() {
        alert("Failed to grab abilities!");
    });
}, refreshGenders = function() {
    $.ajax({
        url: '/get-pokemon-genders/',
        method: "POST",
        data: { 'pokemonId': $('#PokemonId').val() }
    })
    .done(function(data) {
        $('#Gender').empty();
        $.each(data, function(index, item) {
            $('#Gender').append($('<option>').text(item));
        });
        if($.inArray("None", data) !== -1 && data.length == 1)
        {
            $('#Gender').parent().hide();
        }
        else
        {
            $('#Gender').parent().show();
        }
    })
    .fail(function() {
        alert("Failed to grab genders!");
    });
}, refreshHeldItems = function() {
    $.ajax({
        url: '/get-pokemon-battle-items/',
        method: "POST",
        data: { 'pokemonId': $('#PokemonId').val(), 'generationId': $('#GenerationId').val() }
    })
    .done(function(data) {
        if(data.length != 0)
        {
            $('#BattleItemId').empty();
            $('#BattleItemId').append($('<option>'));
            $.each(data, function(index, item) {
                $('#BattleItemId').append($('<option>').val(item.id).text(item.name));
            });
            $('#BattleItemId').parent().show();
        }
        else
        {
            $('#BattleItemId').parent().hide();
        }
    })
    .fail(function() {
        alert("Failed to grab held items!");
    });
}

$(document).ready(function() {
    $('#BattleItemId').parent().hide();
    $('#Gender').parent().hide();
});

$('#PokemonId').on('change', function(){
    refreshGenders();
    grabAbilities();
    refreshHeldItems();
});

$('#Gender').on('change', function(){
    if($('#PokemonId').val() == "678")
    {
        grabAbilities();
    }
});