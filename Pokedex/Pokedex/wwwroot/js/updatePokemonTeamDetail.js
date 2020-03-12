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
            });
            if (abilityId != 0) {
                $('#PokemonTeamDetail_AbilityId').val(abilityId);
            }
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
            });
            if (gender != "Empty") {
                $('#PokemonTeamDetail_Gender').val(gender);
            }
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
                });
                $('#PokemonTeamDetail_BattleItemId').parent().show();
                if (battleItem != "Empty") {
                    $('#PokemonTeamDetail_BattleItemId').val($("#PokemonTeamDetail_BattleItemId option:contains(" + battleItem + ")").val());
                }
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
    abilityId = 0;
    battleItem = "Empty";
    gender = "Empty";
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