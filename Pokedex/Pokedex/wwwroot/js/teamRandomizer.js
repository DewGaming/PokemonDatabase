var altCheck, legendCheck, megaCheck, pokemonList, pokemonURLs, abilityList, randomAbilityBool, randomZygarde = '718' + ((Math.floor(Math.random() * 2 + 1) == 1) ? '' : '-1')
, randomNecrozma = '800-' + Math.floor(Math.random() * 2 + 1)
, fillGeneratedTable = function() {
    $('.teamRandomizerTable tbody').remove();
    $('.teamRandomizerTable').append($('<tbody>'));
    randomAbilityBool = $("#randomAbilityBool").is(":checked");

    if($(window).width() >= 1000)
    {
        for (var i = 0; i < 2; i++) {
            $('.teamRandomizer tbody').append($('<tr>'));
        }

        for (var i = 0; i < 3; i++) {
            $('.teamRandomizer tr:nth-child(1)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 1)
            }));
            $('.teamRandomizer tr:nth-child(2)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 4)
            }));
        }
    }
    else if($(window).width() >= 768)
    {
        for (var i = 0; i < 3; i++) {
            $('.teamRandomizer tbody').append($('<tr>'));
        }

        for (var i = 0; i < 2; i++) {
            $('.teamRandomizer tr:nth-child(1)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 1)
            }));
            $('.teamRandomizer tr:nth-child(2)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 3)
            }));
            $('.teamRandomizer tr:nth-child(3)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 5)
            }));
        }
    }
    else
    {
        for (var i = 0; i < 6; i++) {
            $('.teamRandomizer tbody').append($('<tr>'));
        }

        for (var i = 0; i < 6; i++) {
            $('.teamRandomizer tr:nth-child(' + (i + 1) + ')').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 1)
            }));
        }
    }

    for (var i = 0; i < originalNames.length; i++)
    {
        $('.pokemon' + (i + 1)).append('<a href="' + pokemonURLs[i] + '" target="_blank"><img title="' + pokemonList[i].name.replace('_', ' ') + ' (Click to learn more)" src="https://www.pokemondatabase.net/images/pokemon/' + pokemonList[i].id + '.png" /></a>');
        if (randomAbilityBool)
        {
            $('.pokemon' + (i + 1)).append('<div title="Description: ' + abilityList[i].description + '" class="pokemonAbility">Ability: ' + abilityList[i].name + '</div>')
        }
    }

    $('<button class="btn btn-primary exportTeamButton">Export Team</button>').insertAfter('.generatorButton');
    refreshExportEvent();
}, checkLegendaryChecks = function() {
    var boxChecked = false;
    $('.legendaryCheckbox input').each(function() {
        if ($(this).is(':checked'))
        {
            boxChecked = true;
            return false;
        }
    });

    if(!boxChecked)
    {
        if($('.legendaryBoolCheckbox').is(':visible'))
        {
            $(".legendaryBoolCheckbox").hide();
            $("#legendaryBool").prop('checked', false);
        }
    }
    else
    {
        $(".legendaryBoolCheckbox").show();
    }

    return boxChecked;
}, checkAltFormChecks = function() {
    var boxChecked = false;
    $('.alternateFormCheckbox input').each(function() {
        if ($(this).is(':checked'))
        {
            boxChecked = true;
            return false;
        }
    });

    if(!boxChecked)
    {
        if ($('.altFormBoolCheckbox').is(':visible'))
        {
            $(".altFormBoolCheckbox").hide();
            $(".oneAltFormBoolCheckbox").hide();
            $("#oneAltFormBool").prop('checked', false);
            $("#altFormBool").prop('checked', false);
        }
    }
    else
    {
        $(".altFormBoolCheckbox").show();
        $(".oneAltFormBoolCheckbox").show();
    }

    return boxChecked;
}, checkMegaCheck = function() {
    var boxChecked = false;
    if ($('#Mega').is(':checked'))
    {
        boxChecked = true;
    }

    if(!boxChecked)
    {
        if ($('.multipleMegaBoolCheckbox').is(':visible'))
        {
            $(".multipleMegaBoolCheckbox").hide();
            $("#multipleMegaBool").prop('checked', false);
        }
    }
    else
    {
        $(".multipleMegaBoolCheckbox").show();
    }

    return boxChecked;
}, checkUltraBeasts = function() {
    var boxChecked = false;
    if ($('#gen7').is(':checked'))
    {
        boxChecked = true;
    }

    if(!boxChecked)
    {
        if ($('.ultraBeastCheckbox').is(':visible'))
        {
            $(".ultraBeastCheckbox").hide();
            $("#UltraBeast").prop('checked', false);
        }
    }
    else
    {
        $(".ultraBeastCheckbox").show();
    }

    return boxChecked;
}, checkAlolanForms = function() {
    var boxChecked = false;
    if ($('#gen1').is(':checked'))
    {
        boxChecked = true;
    }

    if(!boxChecked)
    {
        if ($('.alolanFormCheckbox').is(':visible'))
        {
            $(".alolanFormCheckbox").hide();
            $("#Alolan").prop('checked', false);
        }
    }
    else
    {
        $(".alolanFormCheckbox").show();
    }

    return boxChecked;
}, generatorMenuCheck = function() {
    if($(window).width() < 768)
    {
        $('.generatorDropdownMenu').css('flex-wrap', 'wrap');
    }
    else
    {
        $('.generatorDropdownMenu').css('flex-wrap', 'nowrap');
    }
}, refreshExportEvent = function() {
    $('.exportTeamButton').off();

    var necrozmaOriginalId, zygardeOriginalId;

    $('.exportTeamButton').on('click', function() {
        var pokemonStringList = [], abilityStringList = [];
        pokemonList.forEach(function(item) {
            pokemonStringList.push(item.id);
        });

        if(pokemonStringList.indexOf("800-1") > -1 && pokemonStringList.indexOf("800-2") == -1)
        {
            necrozmaOriginalId = "800-2";
        }
        else if(pokemonStringList.indexOf("800-1") == -1 && pokemonStringList.indexOf("800-2") > -1)
        {
            necrozmaOriginalId = "800-1";
        }
        else
        {
            necrozmaOriginalId = randomNecrozma;
        }

        if(pokemonStringList.indexOf("718") > -1 && pokemonStringList.indexOf("718-1") == -1)
        {
            zygardeOriginalId = "718-1";
        }
        else if(pokemonStringList.indexOf("718") == -1 && pokemonStringList.indexOf("718-1") > -1)
        {
            zygardeOriginalId = "718";
        }
        else
        {
            zygardeOriginalId = randomZygarde;
        }

        if(randomAbilityBool)
        {
            abilityList.forEach(function(item) {
                abilityStringList.push(item.name);
            });
        }

        $.ajax({
            url: '/export-pokemon-team/',
            method: 'POST',
            data: { 'pokemonIdList': pokemonStringList, 'abilityList': abilityStringList, 'exportAbilities':  randomAbilityBool, 'necrozmaOriginalId': necrozmaOriginalId, 'zygardeOriginalId': zygardeOriginalId }
        })
        .done(function(data) {
            alert("Copy This For Pokemon Showdown: \n\n=== Export from pokemondatabase.net ===\n\n" + data);
        })
        .fail(function(jqXHR) {
            alert(jqXHR.statusText);
        });
    });
};

$(function() {
    generatorMenuCheck();
    altCheck = checkAltFormChecks();
    legendCheck = checkLegendaryChecks();
    megaCheck = checkMegaCheck();
    checkAlolanForms();
    checkUltraBeasts();
});

$('.generatorDropdown').on('mouseover', function() {
    altCheck = checkAltFormChecks();
    legendCheck = checkLegendaryChecks();
    megaCheck = checkMegaCheck();
    checkAlolanForms();
    checkUltraBeasts();
});

$('.alternateFormCheckbox').on('click', function() {
    altCheck = checkAltFormChecks();
});

$('.legendaryCheckbox').on('click', function() {
    legendCheck = checkLegendaryChecks();
});

$('.megaCheckbox').on('click', function() {
    megaCheck = checkMegaCheck();
});

$('.gen1Checkbox').on('click', function() {
    checkAlolanForms();
    altCheck = checkAltFormChecks();
});

$('.gen7Checkbox').on('click', function() {
    checkUltraBeasts();
    legendCheck = checkLegendaryChecks();
});

$(window).on('resize', function() {
    generatorMenuCheck();

    if(
        ($('.teamRandomizer tr').length == 2 && $(window).width() < 1000) ||
        ($('.teamRandomizer tr').length == 3 && $(window).width() >= 1000) ||
        ($('.teamRandomizer tr').length == 3 && $(window).width() < 768) ||
        ($('.teamRandomizer tr').length == 6 && $(window).width() >= 768)
    )
    {
        fillGeneratedTable();
    }
});

$('.generatorButton').on('click', function() {
    var selectedGens = [], selectedLegendaries = [], selectedForms = [], selectedEvolutions;
    $('.exportTeamButton').remove();
    $('.generationCheckbox input').each(function() {
        if($(this).is(':checked')){
            selectedGens.push(this.value);
        }
    });

    $('.alternateFormCheckbox input').each(function() {
        if($(this).is(':checked')){
            selectedForms.push(this.value);
        }
    });

    $('.legendaryCheckbox input').each(function() {
        if($(this).is(':checked')){
            selectedLegendaries.push(this.value);
        }
    });

    $('.evolutionRadio input').each(function() {
        if($(this).is(':checked')){
            selectedEvolutions = this.value;
        }
    });

    $.ajax({
        url: '/get-pokemon-team/',
        method: 'POST',
        data: { 'selectedGens': selectedGens, 'selectedLegendaries': selectedLegendaries, 'selectedForms': selectedForms, 'selectedEvolutions': selectedEvolutions, 'onlyLegendaries': $("#legendaryBool").is(":checked"), 'onlyAltForms':  $("#altFormBool").is(":checked"), 'multipleMegas':  $("#multipleMegaBool").is(":checked"), 'onePokemonForm':  $("#onePokemonFormBool").is(":checked"), 'randomAbility':  randomAbilityBool }
    })
    .done(function(data) {
        pokemonList = data.allPokemonChangedNames;
        originalNames = data.allPokemonOriginalNames;
        pokemonURLs = data.pokemonURLs;
        abilityList = data.pokemonAbilities
        fillGeneratedTable();
    })
    .fail( function() {
        alert("Failed To Get Team!");
    });
});