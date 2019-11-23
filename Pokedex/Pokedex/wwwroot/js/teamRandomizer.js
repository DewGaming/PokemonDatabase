var altCheck, legendCheck, megaCheck, pokemonList, pokemonURLs, abilityList, exportString
, fillGeneratedTable = function() {
    removeEventButtons();
    $('.teamRandomizerTable tbody').remove();
    $('.teamRandomizerTable').append($('<tbody>'));

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
        if ($(randomAbilityBool).is(':checked'))
        {
            $('.pokemon' + (i + 1)).append('<div title="Description: ' + abilityList[i].description + '" class="pokemonAbility">Ability: ' + abilityList[i].name + '</div>')
        }
    }

    $('<button class="btn btn-primary exportTeamButton">Export Team</button>').insertAfter('.generatorButton');
    $('<button class="btn btn-primary saveTeamButton">Save Team</button>').insertAfter('.exportTeamButton');
    refreshEvents();
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
}, removeEventButtons = function() {
    $('.exportTeamButton').remove();
    $('.saveTeamButton').remove();
}, refreshEvents = function() {
    refreshExportEvent();
    refreshSaveEvent();
}, refreshExportEvent = function() {
    $('.exportTeamButton').off();

    $('.exportTeamButton').on('click', function() {
        console.clear();

        var temp = $("<textarea>");
        $("body").append(temp);
        $(temp).text(exportString);
        $(temp).select();
        document.execCommand("copy");
        $(temp).remove();

        console.log(exportString);

        alert("Team has been copied to your clipboard!");
    });
}, refreshSaveEvent = function() {
    $('.saveTeamButton').off();

    $('.saveTeamButton').on('click', function() {
        var pokemonStringList = [], abilityIdList = [];
        var teamName = prompt("Please Enter Team Name");
        pokemonList.forEach(function(item) {
            pokemonStringList.push(item.id);
        });

        if(randomAbilityBool)
        {
            abilityList.forEach(function(item) {
                abilityIdList.push(item.id);
            });
        }

        $('.gameRadio input').each(function() {
            if($(this).is(':checked')){
                selectedGame = this.value;
            }
        });

        $.ajax({
            url: '/save-pokemon-team/',
            method: 'POST',
            data: { 'pokemonTeamName': teamName, 'selectedGame': selectedGame, 'pokemonIdList': pokemonStringList, 'abilityIdList': abilityIdList, 'exportAbilities':  $("#randomAbilityBool").is(":checked") }
        })
        .done(function(data) {
            alert(data);
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

$('.gameRadio input').on('click', function() {
    $.ajax({
        url: '/get-generations/',
        method: 'POST',
        data: { 'selectedGame': $(this).val() }
    })
    .done(function(data) {
        $(".generationCheckbox").remove();
        var generationIds = [];

        $.each(data, function() {
            var dropdownItem = $("<li>").addClass("dropdown-item generationOption generationCheckbox gen" + this.id + "Checkbox");
            var dropdownInput = $("<input>").attr("id", "gen" + this.id).attr("type", "checkbox").val(this.id).attr("checked", "checked");
            var dropdownLabel = $("<label>").attr("for", "gen" + this.id).addClass("generatorOptionTitle").text("Generation " + this.id);
            $(dropdownItem).append(dropdownInput).append(dropdownLabel);
            $("#generations").append($(dropdownItem));
            generationIds.push(this.id);
        });
        
        if($.inArray('6', generationIds) != -1 || $.inArray('6-1', generationIds) != -1)
        {
            if (!$('.multipleMegaBoolCheckbox').is(':visible'))
            {
                $(".multipleMegaBoolCheckbox").show();
            }

            if (!$('.megaCheckbox').is(':visible'))
            {
                $(".megaCheckbox").show();
            }
        }
        else
        {
            $(".multipleMegaBoolCheckbox").hide();
            $("#multipleMegaBool").prop('checked', false);
            $(".megaCheckbox").hide();
            $("#Mega").prop('checked', false);
        }
        
        if($.inArray('7', generationIds) != -1 || $.inArray('7-1', generationIds) != -1)
        {
            if (!$('.ultraBeastCheckbox').is(':visible'))
            {
                $(".ultraBeastCheckbox").show();
            }

            if (!$('.alolanFormCheckbox').is(':visible'))
            {
                $(".alolanFormCheckbox").show();
            }

            $('.gen1Checkbox').on('click', function() {
                checkAlolanForms();
                altCheck = checkAltFormChecks();
            });
            
            $('.gen7Checkbox').on('click', function() {
                checkUltraBeasts();
                legendCheck = checkLegendaryChecks();
            });
        }
        else
        {
            $(".ultraBeastCheckbox").hide();
            $("#UltraBeast").prop('checked', false);
            $(".alolanFormCheckbox").hide();
            $("#Alolan").prop('checked', false);
        }
        
        if($.inArray('8', generationIds) != -1)
        {
            $(".multipleMegaBoolCheckbox").hide();
            $("#multipleMegaBool").prop('checked', false);
            $(".megaCheckbox").hide();
            $("#Mega").prop('checked', false);
            if (!$('.galarianFormCheckbox').is(':visible'))
            {
                $(".galarianFormCheckbox").show();
            }
        }
        else
        {
            $(".galarianFormCheckbox").hide();
            $("#Galarian").prop('checked', false);
        }
        
        megaCheck = checkMegaCheck();
        altCheck = checkAltFormChecks();
        legendCheck = checkLegendaryChecks();
    })
    .fail( function() {
        alert("Failed To Get Team!");
    });
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
    var selectedGens = [], selectedLegendaries = [], selectedForms = [], selectedEvolutions, selectedGame;
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

    $('.gameRadio input').each(function() {
        if($(this).is(':checked')){
            selectedGame = this.value;
        }
    });

    $.ajax({
        url: '/get-pokemon-team/',
        method: 'POST',
        data: { 'selectedGens': selectedGens, 'selectedGame': selectedGame, 'selectedLegendaries': selectedLegendaries, 'selectedForms': selectedForms, 'selectedEvolutions': selectedEvolutions, 'onlyLegendaries': $("#legendaryBool").is(":checked"), 'onlyAltForms':  $("#altFormBool").is(":checked"), 'multipleMegas':  $("#multipleMegaBool").is(":checked"), 'onePokemonForm':  $("#onePokemonFormBool").is(":checked"), 'randomAbility':  $("#randomAbilityBool").is(":checked") }
    })
    .done(function(data) {
        pokemonList = data.allPokemonChangedNames;
        originalNames = data.allPokemonOriginalNames;
        pokemonURLs = data.pokemonURLs;
        abilityList = data.pokemonAbilities;
        exportString = data.exportString;
        fillGeneratedTable();
    })
    .fail( function() {
        alert("Failed To Get Team!");
    });
});