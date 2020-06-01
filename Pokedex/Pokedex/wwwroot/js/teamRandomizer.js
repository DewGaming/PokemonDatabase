var pokemonList, pokemonURLs, abilityList, typeList, exportString
    , fillGeneratedTable = function (appConfig) {
        removeEventButtons();
        $('.teamRandomizerTable tbody').remove();
        $('.teamRandomizerTable').append($('<tbody>'));

        if ($(window).width() >= 1000) {
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
        else if ($(window).width() >= 768) {
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
        else {
            for (var i = 0; i < 6; i++) {
                $('.teamRandomizer tbody').append($('<tr>'));
            }

            for (var i = 0; i < 6; i++) {
                $('.teamRandomizer tr:nth-child(' + (i + 1) + ')').append($('<td>', {
                    class: "generatorPicture shadowed pokemon" + (i + 1)
                }));
            }
        }

        for (var i = 0; i < originalNames.length; i++) {
            $('.pokemon' + (i + 1)).append('<a href="' + pokemonURLs[i] + '" target="_blank"><img title="' + pokemonList[i].name.replace('_', ' ') + ' (Click to learn more)" src="' + appConfig.webUrl + appConfig.pokemonImageUrl + pokemonList[i].id + '.png" /></a>');
            if ($(randomAbilityBool).is(':checked')) {
                $('.pokemon' + (i + 1)).append('<div title="Description: ' + abilityList[i].description + '" class="pokemonAbility">Ability: ' + abilityList[i].name + '</div>')
            }
        }

        $('<button class="btn btn-primary exportTeamButton">Export Team</button>').insertAfter('.generatorButton');
        $('<button class="btn btn-primary saveTeamButton">Save Team</button>').insertAfter('.exportTeamButton');
        refreshEvents();
    }, checkLegendaryChecks = function () {
        var boxChecked = false;
        $('.legendaryCheckbox input').each(function () {
            if ($(this).is(':checked')) {
                boxChecked = true;
                return false;
            }
        });

        if (!boxChecked) {
            if ($('.legendaryBoolCheckbox').is(':visible')) {
                $(".legendaryBoolCheckbox").hide();
                $("#legendaryBool").prop('checked', false);
            }
        }
        else {
            $(".legendaryBoolCheckbox").show();
        }
    }, checkAltFormChecks = function () {
        var boxChecked = false;
        $('.alternateFormCheckbox input').each(function () {
            if ($(this).is(':checked')) {
                boxChecked = true;
                return false;
            }
        });

        if (!boxChecked) {
            if ($('.altFormBoolCheckbox').is(':visible')) {
                $(".altFormBoolCheckbox").hide();
                $(".oneAltFormBoolCheckbox").hide();
                $("#oneAltFormBool").prop('checked', false);
                $("#altFormBool").prop('checked', false);
            }
        }
        else {
            $(".altFormBoolCheckbox").show();
            $(".oneAltFormBoolCheckbox").show();
        }
    }, checkMegaCheck = function () {
        var boxChecked = false;
        if ($('#Mega').is(':checked')) {
            boxChecked = true;
        }

        if (!boxChecked) {
            if ($('.multipleMegaBoolCheckbox').is(':visible')) {
                $(".multipleMegaBoolCheckbox").hide();
                $("#multipleMegaBool").prop('checked', false);
            }
        }
        else {
            $(".multipleMegaBoolCheckbox").show();
        }
    }, checkGigantamaxCheck = function () {
        var boxChecked = false;
        if ($('#Gigantamax').is(':checked')) {
            boxChecked = true;
        }

        if (!boxChecked) {
            if ($('.multipleGMaxBoolCheckbox').is(':visible')) {
                $(".multipleGMaxBoolCheckbox").hide();
                $("#multipleGMaxBool").prop('checked', false);
            }
        }
        else {
            $(".multipleGMaxBoolCheckbox").show();
        }
    }, checkUltraBeasts = function () {
        var boxChecked = false;
        if ($('#gen7').is(':checked')) {
            boxChecked = true;
        }

        if (!boxChecked) {
            if ($('.ultraBeastCheckbox').is(':visible')) {
                $(".ultraBeastCheckbox").hide();
                $("#UltraBeast").prop('checked', false);
            }
        }
        else {
            $(".ultraBeastCheckbox").show();
        }
    }, generatorMenuCheck = function () {
        if ($(window).width() < 768) {
            $('.generatorDropdownMenu').css('flex-wrap', 'wrap');
        }
        else {
            $('.generatorDropdownMenu').css('flex-wrap', 'nowrap');
        }
    }, removeEventButtons = function () {
        $('.exportTeamButton').remove();
        $('.saveTeamButton').remove();
    }, refreshEvents = function () {
        refreshExportEvent();
        refreshSaveEvent();
    }, refreshExportEvent = function () {
        $('.exportTeamButton').off();

        $('.exportTeamButton').on('click', function () {
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
    }, refreshSaveEvent = function () {
        $('.saveTeamButton').off();

        $('.saveTeamButton').on('click', function () {
            var pokemonStringList = [], abilityIdList = [];
            var teamName = prompt("Please Enter Team Name");
            pokemonList.forEach(function (item) {
                pokemonStringList.push(item.id);
            });

            if (randomAbilityBool) {
                abilityList.forEach(function (item) {
                    abilityIdList.push(item.id);
                });
            }

            $('.gameRadio input').each(function () {
                if ($(this).is(':checked')) {
                    selectedGame = this.value;
                }
            });

            $.ajax({
                url: '/save-pokemon-team/',
                method: 'POST',
                data: { 'pokemonTeamName': teamName, 'selectedGame': selectedGame, 'pokemonIdList': pokemonStringList, 'abilityIdList': abilityIdList, 'exportAbilities': $("#randomAbilityBool").is(":checked") }
            })
                .done(function (data) {
                    alert(data);
                })
                .fail(function (jqXHR) {
                    alert(jqXHR.statusText);
                });
        });
    }, refreshGenerationsByGame = function () {
        var selectedGame = $('.gameRadioOption input:checked').val();
        $.ajax({
            url: '/get-generations/',
            method: 'POST',
            data: { 'selectedGame': selectedGame }
        })
            .done(function (data) {
                $(".generationCheckbox").remove();

                $.each(data.allGenerations, function () {
                    var dropdownItem = $("<li>").addClass("dropdown-item generationOption generationCheckbox gen" + this.id + "Checkbox");
                    var dropdownInput = $("<input>").attr("id", "gen" + this.id).attr("type", "checkbox").val(this.id).attr("checked", "checked");
                    var dropdownLabel = $("<label>").attr("for", "gen" + this.id).addClass("generatorOptionTitle").text("Generation " + this.id);
                    $(dropdownItem).append(dropdownInput).append(dropdownLabel);
                    $("#generations").append($(dropdownItem));
                });

                if ($.inArray(selectedGame, ['1', '2']) != -1) {
                    $("#alternateForms").hide();
                    $(".randomAbilityCheckbox").hide();
                    $("#randomAbilityBool").prop('checked', false);
                    $(".otherFormCheckbox").hide();
                    $("#Other").prop('checked', false);
                    $(".onePokemonFormBoolCheckbox").hide();
                    $("#onePokemonFormBool").prop('checked', false);
                }
                else {
                    $("#alternateForms").show();
                    $(".randomAbilityCheckbox").show();
                    $("#randomAbilityBool").prop('checked', true);
                    $(".otherFormCheckbox").show();
                    $("#Other").prop('checked', true);
                    $(".onePokemonFormBoolCheckbox").show();
                    $("#onePokemonFormBool").prop('checked', true);
                }

                if ($.inArray(selectedGame, ['0', '12', '13', '14', '15', '16']) != -1) {
                    if (!$('.multipleMegaBoolCheckbox').is(':visible')) {
                        $(".multipleMegaBoolCheckbox").show();
                    }

                    if (!$('.megaCheckbox').is(':visible')) {
                        $(".megaCheckbox").show();
                        $("#Mega").prop('checked', true);
                    }
                }
                else {
                    $(".multipleMegaBoolCheckbox").hide();
                    $("#multipleMegaBool").prop('checked', false);
                    $(".megaCheckbox").hide();
                    $("#Mega").prop('checked', false);
                }

                if ($.inArray(selectedGame, ['0', '14', '15']) != -1) {
                    if (!$('.ultraBeastCheckbox').is(':visible')) {
                        $(".ultraBeastCheckbox").show();
                        $("#UltraBeast").prop('checked', true);
                    }

                    $('.gen7Checkbox').on('click', function () {
                        checkUltraBeasts();
                        legendCheck = checkLegendaryChecks();
                    });
                }
                else {
                    $(".ultraBeastCheckbox").hide();
                    $("#UltraBeast").prop('checked', false);
                }

                if (selectedGame == 16) {
                    $(".randomAbilityCheckbox").hide();
                    $("#randomAbilityBool").prop('checked', false);
                }

                if ($.inArray(selectedGame, ['0', '14', '15', '16', '17']) != -1) {
                    if (!$('.alolanFormCheckbox').is(':visible')) {
                        $(".alolanFormCheckbox").show();
                        $("#Alolan").prop('checked', true);
                    }

                    $('.gen1Checkbox').on('click', function () {
                        altCheck = checkAltFormChecks();
                    });
                }
                else {
                    $(".alolanFormCheckbox").hide();
                    $("#Alolan").prop('checked', false);
                }

                if ($.inArray(selectedGame, ['0', '17']) != -1) {
                    if (!$('.galarianFormCheckbox').is(':visible')) {
                        $(".galarianFormCheckbox").show();
                        $("#Galarian").prop('checked', true);
                    }
                    
                    if (!$('.gigantamaxFormCheckbox').is(':visible')) {
                        $(".gigantamaxFormCheckbox").show();
                        $("#Gigantamax").prop('checked', true);
                    }

                    if (!$('.multipleGMaxBoolCheckbox').is(':visible')) {
                        $(".multipleGMaxBoolCheckbox").show();
                    }
                }
                else {
                    $(".galarianFormCheckbox").hide();
                    $("#Galarian").prop('checked', false);
                    $(".gigantamaxFormCheckbox").hide();
                    $("#Gigantamax").prop('checked', false);
                    $(".multipleGMaxBoolCheckbox").hide();
                    $("#multipleGMaxBool").prop('checked', false);
                }

                data.pokemonTypes;

                var typeId = $('input[name=typeSelection]:checked').val();

                $('.typeRadioOption').remove();
        
                var dropdownItem = $("<li>").addClass("dropdown-item generationOption typeRadioOption");
                var dropdownInput = $("<input>").attr("id", "type0").attr("name", "typeSelection").attr("type", "radio").val(0).attr("checked", "checked");;
                var dropdownLabel = $("<label>").attr("for", "type0").addClass("generatorOptionTitle").text("Any Type");
                $(dropdownItem).append(dropdownInput).append(dropdownLabel);
                $("#types").append($(dropdownItem));

                $.each(data.allTypes, function() {
                    var dropdownItem = $("<li>").addClass("dropdown-item generationOption typeRadioOption");
                    var dropdownInput = $("<input>").attr("id", "type" + this.id).attr("name", "typeSelection").attr("type", "radio").val(this.id);
                    var dropdownLabel = $("<label>").attr("for", "type" + this.id).addClass("generatorOptionTitle").text(this.name);
                    $(dropdownItem).append(dropdownInput).append(dropdownLabel);
                    $("#types").append($(dropdownItem));
                });

                $("input[name=typeSelection][value=" + typeId + "]").prop('checked', true);

                megaCheck = checkMegaCheck();
                altCheck = checkAltFormChecks();
                legendCheck = checkLegendaryChecks();
                checkOtherOptions();
            })
            .fail(function () {
                alert("Failed To Get Generations!");
            });
    }, checkOtherOptions = function () {
        var isVisible = 0;

        $('.otherOption').each(function () {
            if ($(this).css('display') != 'none') {
                isVisible++;
            }
        });

        if (isVisible == 0) {
            $('#otherOptions').hide();
        }
        else {
            $('#otherOptions').show();
        }
    }, updateDropdown = function () {
        checkAltFormChecks();
        checkLegendaryChecks();
        checkMegaCheck();
        checkGigantamaxCheck();
        checkUltraBeasts();
        checkOtherOptions();
        refreshGenerationsByGame();
        generatorMenuCheck();
    };

$(function () {
    updateDropdown();
});

$('.generatorDropdown').on('mouseover', function () {
    updateDropdown();
});

$('.alternateFormCheckbox').on('click', function () {
    checkAltFormChecks();
    checkOtherOptions();
});

$('.legendaryCheckbox').on('click', function () {
    checkLegendaryChecks();
    checkOtherOptions();
});

$('.megaCheckbox').on('click', function () {
    checkMegaCheck();
    checkOtherOptions();
});

$('.gigantamaxFormCheckbox').on('click', function () {
    checkGigantamaxCheck();
    checkOtherOptions();
});

$('.gameRadioOption input').on('click', function () {
    refreshGenerationsByGame();
});

$(window).on('resize', function () {
    generatorMenuCheck();

    if (
        ($('.teamRandomizer tr').length == 2 && $(window).width() < 1000) ||
        ($('.teamRandomizer tr').length == 3 && $(window).width() >= 1000) ||
        ($('.teamRandomizer tr').length == 3 && $(window).width() < 768) ||
        ($('.teamRandomizer tr').length == 6 && $(window).width() >= 768)
    ) {
        fillGeneratedTable();
    }
});

$('.generatorButton').on('click', function () {
    var selectedGens = [], selectedLegendaries = [], selectedForms = [], selectedEvolutions, selectedGame, selectedType;
    $('.generationCheckbox input').each(function () {
        if ($(this).is(':checked')) {
            selectedGens.push(this.value);
        }
    });

    $('.alternateFormCheckbox input').each(function () {
        if ($(this).is(':checked')) {
            selectedForms.push(this.value);
        }
    });

    $('.legendaryCheckbox input').each(function () {
        if ($(this).is(':checked')) {
            selectedLegendaries.push(this.value);
        }
    });

    $('.evolutionRadio input').each(function () {
        if ($(this).is(':checked')) {
            selectedEvolutions = this.value;
        }
    });

    $('.gameRadioOption input').each(function () {
        if ($(this).is(':checked')) {
            selectedGame = this.value;
        }
    });

    $('.typeRadioOption input').each(function () {
        if ($(this).is(':checked')) {
            selectedType = this.value;
        }
    });

    $.ajax({
        url: '/get-pokemon-team/',
        method: 'POST',
        data: { 'selectedGens': selectedGens, 'selectedGame': selectedGame, 'selectedType': selectedType, 'selectedLegendaries': selectedLegendaries, 'selectedForms': selectedForms, 'selectedEvolutions': selectedEvolutions, 'onlyLegendaries': $("#legendaryBool").is(":checked"), 'onlyAltForms': $("#altFormBool").is(":checked"), 'multipleMegas': $("#multipleMegaBool").is(":checked"), 'multipleGMax': $("#multipleGMaxBool").is(":checked"), 'onePokemonForm': $("#onePokemonFormBool").is(":checked"), 'randomAbility': $("#randomAbilityBool").is(":checked") }
    })
        .done(function (data) {
            pokemonList = data.allPokemonChangedNames;
            originalNames = data.allPokemonOriginalNames;
            pokemonURLs = data.pokemonURLs;
            abilityList = data.pokemonAbilities;
            exportString = data.exportString;
            fillGeneratedTable(data.appConfig);
        })
        .fail(function () {
            alert("Failed To Get Team!");
        });
});