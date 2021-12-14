var abilityId = 0, gender = "Empty", battleItem = "Empty", grabAbilities = function () {
    $.ajax({
        url: '/get-pokemon-abilities/',
        method: "POST",
        data: { 'pokemonId': $('#PokemonTeamDetail_PokemonId').val() }
    })
        .done(function (data) {
            $('#PokemonTeamDetail_AbilityId').empty();
            $.each(data, function (index, item) {
                $('#PokemonTeamDetail_AbilityId').append($('<option>').val(item.id).text(item.name));
                if (item.id == abilityId)
                {
                    $('#PokemonTeamDetail_AbilityId option:contains("' + item.name + '")').attr("selected", "selected");
                }
            });
        })
        .fail(function () {
            alert("Failed to grab abilities!");
        });
}, refreshGenders = function () {
    $.ajax({
        url: '/get-pokemon-genders/',
        method: "POST",
        data: { 'pokemonId': $('#PokemonTeamDetail_PokemonId').val() }
    })
        .done(function (data) {
            $('#PokemonTeamDetail_Gender').empty();
            $.each(data, function (index, item) {
                $('#PokemonTeamDetail_Gender').append($('<option>').text(item));
                if (item == gender)
                {
                    $('#PokemonTeamDetail_Gender option:contains("' + item + '")').attr("selected", "selected");
                }
            });
            if ($.inArray("None", data) !== -1 && data.length == 1) {
                $('#PokemonTeamDetail_Gender').parent().hide();
            }
            else {
                $('#PokemonTeamDetail_Gender').parent().show();
            }
        })
        .fail(function () {
            alert("Failed to grab genders!");
        });
}, refreshHeldItems = function () {
    $.ajax({
        url: '/get-pokemon-battle-items/',
        method: "POST",
        data: { 'pokemonId': $('#PokemonTeamDetail_PokemonId').val(), 'generationId': $('#GenerationId').val() }
    })
        .done(function (data) {
            if (data.length != 0) {
                $('#PokemonTeamDetail_BattleItemId').empty();
                $('#PokemonTeamDetail_BattleItemId').append($('<option>'));
                $.each(data, function (index, item) {
                    $('#PokemonTeamDetail_BattleItemId').append($('<option>').val(item.id).text(item.name));
                    if (item.name == battleItem)
                    {
                        $('#PokemonTeamDetail_BattleItemId option:contains("' + item.name + '")').attr("selected", "selected");
                    }
                });
                $('#PokemonTeamDetail_BattleItemId').parent().show();
            }
            else {
                $('#PokemonTeamDetail_BattleItemId').parent().hide();
            }
        })
        .fail(function () {
            alert("Failed to grab held items!");
        });
}

$(document).ready(function () {
    abilityId = $('#PokemonTeamDetail_AbilityId').val();
    gender = $('#PokemonTeamDetail_Gender').val();
    if ($('#PokemonTeamDetail_BattleItemId').val().length != 0) {
        battleItem = $('#PokemonTeamDetail_BattleItemId option[value=' + $('#PokemonTeamDetail_BattleItemId').val() + ']').text();
    }
    refreshGenders();
    grabAbilities();
    refreshHeldItems();
});

$('#PokemonTeamDetail_PokemonId').on('change', function () {
    abilityId = $('#PokemonTeamDetail_AbilityId').val();
    gender = $('#PokemonTeamDetail_Gender').val();
    if ($('#PokemonTeamDetail_BattleItemId').val().length != 0) {
        battleItem = $('#PokemonTeamDetail_BattleItemId option[value=' + $('#PokemonTeamDetail_BattleItemId').val() + ']').text();
    }
    refreshGenders();
    grabAbilities();
});

$('#PokemonTeamDetail_Gender').on('change', function () {
    if ($('#PokemonTeamDetail_PokemonId').val() == "678") {
        abilityId = 0;
        gender = "";
        grabAbilities();
    }
});