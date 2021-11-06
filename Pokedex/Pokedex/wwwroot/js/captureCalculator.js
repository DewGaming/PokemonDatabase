$(function () {
    $('#healthVal').text($('.healthSlider').val() + '%');
    $('.healthSlider').val($('.healthSlider').val());
    $('#encounterLevelVal').text($('.encounterLevelSlider').val());
    $('.encounterLevelSlider').val($('.encounterLevelSlider').val());
    $('#userLevelVal').text($('.userLevelSlider').val());
    $('.userLevelSlider').val($('.userLevelSlider').val());
    turnText = ''
    if (Number($('.turnSlider').val()) == 11)
    {
        turnText = '11+'
    }
    else
    {
        turnText = $('.turnSlider').val()
    }
    $('#turnVal').text(turnText);
    $('.turnSlider').val($('.turnSlider').val());
    $('#Generation').val($('#Generation option').last().val())
    updateIDs();
    checkPokemon();
    checkPokeball();
});

$('.healthSlider').on('input', function () {
    $('#healthVal').text($(this).val() + '%');
});

$('.encounterLevelSlider').on('input', function () {
    $('#encounterLevelVal').text($(this).val());
});

$('.userLevelSlider').on('input', function () {
    $('#userLevelVal').text($(this).val());
});

$('.turnSlider').on('input', function () {
    turnText = ''
    if (Number($(this).val()) == 11)
    {
        turnText = '11+'
    }
    else
    {
        turnText = $(this).val()
    }
    $('#turnVal').text(turnText);
});

$('#Pokeball').on('input', function () {
    checkSpecialPokeball();
});

$('.calculatorButton').on('click', function() {
    $.ajax({
        url: '/get-capture-chance/',
        method: 'POST',
        data: { 'pokemonId': $('#Pokemon').val(), 'generationId': $('#Generation').val(), 'healthPercentage': $('.healthSlider').val() / 100, 'pokeballId': $('#Pokeball').val(), 'statusId': $('#Status').val(), 'turnCount': $('.turnSlider').val(), 'encounterLevel': $('.encounterLevelSlider').val(), 'userLevel': $('.userLevelSlider').val(), 'surfing': $('#Surfing').is(':checked'), 'fishing': $('#Fishing').is(':checked'), 'previouslyCaught': $('#PreviouslyCaught').is(':checked'), 'caveOrNight': $('#CaveOrNight').is(':checked'), 'sameGender': $("input[name='Gender']:checked").val() }
    })
        .done(function (data) {
            $('.calculatedChance').text(data);
        })
        .fail(function () {
            alert("Failed to grab technique!");
        });
});

$('.regionList').on('change', function() {
    updateIDs();
    checkPokemon();
    checkPokeball();
})

var generationId, pokemonId, pokeballId, updateIDs = function () {
    generationId = $('.regionList > select').val();
    pokemonId = $('.pokemonList > select').val();
    pokeballId = $('.pokeballList > select').val();
}, checkPokemon = function () {
    $('.pokemonList option').each(function() {
        if (Number($(this).attr('id')) <= Number(generationId))
        {
            $(this).css('display', 'block');
        }
        else
        {
            $(this).css('display', 'none');
        }
    });

    if (!$('.pokemonList option[value="' + pokemonId + '"]').is(':visible'))
    {
        $('.pokemonList #Pokemon').val(1);
    }
}, checkPokeball = function() {
    $('.pokeballList option').each(function() {
        if (Number($(this).attr('id')) <= Number(generationId))
        {
            $(this).css('display', 'block');
        }
        else
        {
            $(this).css('display', 'none');
        }
    });

    if (!$('.pokeballList option[value="' + pokeballId + '"]').is(':visible'))
    {
        $('.pokeballList #Pokeball').val(1);
    }
    checkSpecialPokeball(); 
}, checkSpecialPokeball = function() {
    switch(Number($('#Pokeball').val())) {
        case 7:
            $('.Surfing').css('display', 'block');
            $('.Fishing').css('display', 'none');
            $('#Fishing').prop('checked', false);
            $('.PreviouslyCaught').css('display', 'none');
            $('#PreviouslyCaught').prop('checked', false);
            $('.CaveOrNight').css('display', 'none');
            $('#CaveOrNight').prop('checked', false);
            $('.SameGender').css('display', 'none');
            $('#SameGender').prop('checked', false);
            $('.encounterLevelBar').css('display', 'none');
            $('.encounterLevelBar').val(100);
            $('.userLevelBar').css('display', 'none');
            $('.userLevelBar').val(100);
            $('.turnBar').css('display', 'none');
            $('.turnBar').val(1);
            break;
        case 8:
            $('.Surfing').css('display', 'none');
            $('#Surfing').prop('checked', false);
            $('.Fishing').css('display', 'none');
            $('#Fishing').prop('checked', false);
            $('.PreviouslyCaught').css('display', 'none');
            $('#PreviouslyCaught').prop('checked', false);
            $('.CaveOrNight').css('display', 'none');
            $('#CaveOrNight').prop('checked', false);
            $('.SameGender').css('display', 'none');
            $('#SameGender').prop('checked', false);
            $('.encounterLevelBar').css('display', 'block');
            $('.encounterLevelBar').val(100);
            $('.userLevelBar').css('display', 'none');
            $('.userLevelBar').val(100);
            $('.turnBar').css('display', 'none');
            $('.turnBar').val(1);
            break;
        case 9:
            $('.Surfing').css('display', 'none');
            $('#Surfing').prop('checked', false);
            $('.Fishing').css('display', 'none');
            $('#Fishing').prop('checked', false);
            $('.PreviouslyCaught').css('display', 'block');
            $('.CaveOrNight').css('display', 'none');
            $('#CaveOrNight').prop('checked', false);
            $('.SameGender').css('display', 'none');
            $('#SameGender').prop('checked', false);
            $('.encounterLevelBar').css('display', 'none');
            $('.encounterLevelBar').val(100);
            $('.userLevelBar').css('display', 'none');
            $('.userLevelBar').val(100);
            $('.turnBar').css('display', 'none');
            $('.turnBar').val(1);
            break;
        case 12:
            $('.Surfing').css('display', 'none');
            $('#Surfing').prop('checked', false);
            $('.Fishing').css('display', 'none');
            $('#Fishing').prop('checked', false);
            $('.PreviouslyCaught').css('display', 'none');
            $('#PreviouslyCaught').prop('checked', false);
            $('.CaveOrNight').css('display', 'block');
            $('.SameGender').css('display', 'none');
            $('#SameGender').prop('checked', false);
            $('.encounterLevelBar').css('display', 'none');
            $('.encounterLevelBar').val(100);
            $('.userLevelBar').css('display', 'none');
            $('.userLevelBar').val(100);
            $('.turnBar').css('display', 'none');
            $('.turnBar').val(1);
            break;
        case 14:
            $('.Surfing').css('display', 'none');
            $('#Surfing').prop('checked', false);
            $('.Fishing').css('display', 'none');
            $('#Fishing').prop('checked', false);
            $('.PreviouslyCaught').css('display', 'none');
            $('#PreviouslyCaught').prop('checked', false);
            $('.CaveOrNight').css('display', 'none');
            $('#CaveOrNight').prop('checked', false);
            $('.SameGender').css('display', 'none');
            $('#SameGender').prop('checked', false);
            $('.encounterLevelBar').css('display', 'none');
            $('.encounterLevelBar').val(100);
            $('.userLevelBar').css('display', 'none');
            $('.userLevelBar').val(100);
            $('.turnBar').css('display', 'block');
            $('.turnBar').val(1);
            break;
        case 21:
            $('.Surfing').css('display', 'none');
            $('#Surfing').prop('checked', false);
            $('.Fishing').css('display', 'none');
            $('#Fishing').prop('checked', false);
            $('.PreviouslyCaught').css('display', 'none');
            $('#PreviouslyCaught').prop('checked', false);
            $('.CaveOrNight').css('display', 'none');
            $('#CaveOrNight').prop('checked', false);
            $('.SameGender').css('display', 'none');
            $('#SameGender').prop('checked', false);
            $('.encounterLevelBar').css('display', 'block');
            $('.encounterLevelBar').val(100);
            $('.userLevelBar').css('display', 'block');
            $('.userLevelBar').val(100);
            $('.turnBar').css('display', 'none');
            $('.turnBar').val(1);
            break;
        case 22:
            $('.Surfing').css('display', 'none');
            $('#Surfing').prop('checked', false);
            $('.Fishing').css('display', 'none');
            $('#Fishing').prop('checked', false);
            $('.PreviouslyCaught').css('display', 'none');
            $('#PreviouslyCaught').prop('checked', false);
            $('.CaveOrNight').css('display', 'none');
            $('#CaveOrNight').prop('checked', false);
            $('.SameGender').css('display', 'block');
            $('input#SameGender').prop('checked', true);
            $('.encounterLevelBar').css('display', 'none');
            $('.encounterLevelBar').val(100);
            $('.userLevelBar').css('display', 'none');
            $('.userLevelBar').val(100);
            $('.turnBar').css('display', 'none');
            $('.turnBar').val(1);
            break;
        case 23:
            $('.Surfing').css('display', 'none');
            $('#Surfing').prop('checked', false);
            $('.Fishing').css('display', 'block');
            $('.PreviouslyCaught').css('display', 'none');
            $('#PreviouslyCaught').prop('checked', false);
            $('.CaveOrNight').css('display', 'none');
            $('#CaveOrNight').prop('checked', false);
            $('.SameGender').css('display', 'none');
            $('#SameGender').prop('checked', false);
            $('.encounterLevelBar').css('display', 'none');
            $('.encounterLevelBar').val(100);
            $('.userLevelBar').css('display', 'none');
            $('.userLevelBar').val(100);
            $('.turnBar').css('display', 'none');
            $('.turnBar').val(1);
            break;
        case 29:
            $('.Surfing').css('display', 'none');
            $('#Surfing').prop('checked', false);
            $('.Fishing').css('display', 'none');
            $('#Fishing').prop('checked', false);
            $('.PreviouslyCaught').css('display', 'none');
            $('#PreviouslyCaught').prop('checked', false);
            $('.CaveOrNight').css('display', 'none');
            $('#CaveOrNight').prop('checked', false);
            $('.SameGender').css('display', 'none');
            $('#SameGender').prop('checked', false);
            $('.encounterLevelBar').css('display', 'none');
            $('.encounterLevelBar').val(100);
            $('.userLevelBar').css('display', 'none');
            $('.userLevelBar').val(100);
            $('.turnBar').css('display', 'block');
            $('.turnBar').val(1);
            break;
        default:
            $('.Surfing').css('display', 'none');
            $('#Surfing').prop('checked', false);
            $('.Fishing').css('display', 'none');
            $('#Fishing').prop('checked', false);
            $('.PreviouslyCaught').css('display', 'none');
            $('#PreviouslyCaught').prop('checked', false);
            $('.CaveOrNight').css('display', 'none');
            $('#CaveOrNight').prop('checked', false);
            $('.SameGender').css('display', 'none');
            $('#SameGender').prop('checked', false);
            $('.encounterLevelBar').css('display', 'none');
            $('.encounterLevelBar').val(100);
            $('.userLevelBar').css('display', 'none');
            $('.userLevelBar').val(100);
            $('.turnBar').css('display', 'none');
            $('.turnBar').val(1);
            break;
    }
}