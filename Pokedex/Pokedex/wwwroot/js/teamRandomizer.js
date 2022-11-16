var pokemonList, pokemonURLs, abilityList, typeList, exportString
    , fillGeneratedTable = function (appConfig) {
        for (var i = 0; i < 6; i++) {
            $('.teamRandomizerResults').append($('<div>', {
                class: "generatorPicture shadowed pokemon" + (i + 1)
            }));
        }

        for (var i = 0; i < originalNames.length; i++) {
            var imageLocation = appConfig.officialPokemonImageUrl;
            if (Math.floor((Math.random() * 4096) + 1) == 4096 && originalNames[i].hasShinyImage) {
                console.log("Shiny Pokemon!");
                imageLocation = appConfig.shinyPokemonImageUrl;
                $.ajax({
                    url: '/shiny-pokemon-found/',
                    method: 'POST'
                })
            }
            $('.pokemon' + (i + 1)).append('<a href="' + pokemonURLs[i] + '" target="_blank"><img title="' + pokemonList[i].name.replace('_', ' ') + ' (Click to learn more)" src="' + appConfig.webUrl + imageLocation + pokemonList[i].id + '.png" /></a>');
            if ($(randomAbilityBool).prop('checked')) {
                $('.pokemon' + (i + 1)).append('<div title="Description: ' + abilityList[i].description + '" class="pokemonAbility">Ability: ' + abilityList[i].name + '</div>')
            }
        }

        if ($(window).width() >= 406) {
            if (!abilityList.find((o) => { return o["name"] === "Unknown" })) {
                $('<button class="btn btn-primary saveTeamButton">Save Team</button>').insertAfter('.typeSelector');
            }
            $('<button class="btn btn-primary exportTeamButton">Export Team</button>').insertAfter('.generatorButton');
        }
        else {
            $('<br class="mobileBreak" />').insertAfter('.generatorButton');
            $('<button class="btn btn-primary exportTeamButton">Export Team</button>').insertAfter('.mobileBreak');
            if (!abilityList.find((o) => { return o["name"] === "Unknown" })) {
                $('<button class="btn btn-primary saveTeamButton">Save Team</button>').insertAfter('.mobileBreak');
            }
        }

        refreshEvents();
    }, checkLegendaryChecks = function () {
        var boxChecked = false;
        $('.legendaryCheckbox input').each(function () {
            if ($(this).prop('checked')) {
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
            if ($(this).prop('checked')) {
                boxChecked = true;
                return false;
            }
        });

        if (!boxChecked) {
            if ($('.altFormBoolCheckbox').is(':visible')) {
                $(".altFormBoolCheckbox").hide();
                $("#altFormBool").prop('checked', false);
            }

            if ($('.onePokemonFormBoolCheckbox').is(':visible')) {
                $(".onePokemonFormBoolCheckbox").hide();
                $("#onePokemonFormBool").prop('checked', false);
            }
        }
        else {
            $(".altFormBoolCheckbox").show();
            $(".onePokemonFormBoolCheckbox").show();
        }
    }, checkMegaCheck = function () {
        var boxChecked = false;
        if ($('#MegaEvolution').prop('checked')) {
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
        if ($('#Gigantamax').prop('checked')) {
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
        if ($(window).width() < 405) {
            $('.mobileBreak').remove();
        }
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

            $('.gameRadioOption input').each(function () {
                if ($(this).prop('checked')) {
                    selectedGame = this.value;
                }
            });

            $.ajax({
                url: '/save-pokemon-team/',
                method: 'POST',
                data: { 'selectedGame': selectedGame, 'pokemonIdList': pokemonStringList, 'abilityIdList': abilityIdList, 'exportAbilities': $("#randomAbilityBool").is(":checked"), 'pokemonTeamName': teamName }
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
                var gensChecked = []
                $('.generationCheckbox').each(function () {
                    if ($(this).find('input').prop('checked')) {
                        gensChecked.push($(this).attr('class').split(' ').pop());
                    }
                })

                $(".generationCheckbox").remove();

                $.each(data.allGenerations, function () {
                    var dropdownItem = $("<li>").addClass("dropdown-item generationOption generationCheckbox gen" + this.id + "Checkbox");
                    var dropdownInput = $("<input>").attr("id", "gen" + this.id).attr("type", "checkbox").val(this.id);
                    var dropdownLabel = $("<label>").attr("for", "gen" + this.id).addClass("generatorOptionTitle").text(this.generationName);
                    $(dropdownItem).append(dropdownInput).append(dropdownLabel);
                    $("#generations").append($(dropdownItem));
                });

                $.each(gensChecked, function (index, gen) {
                    $('.' + gen + ' input').prop('checked', true);
                })

                if ($.inArray(selectedGame, ['0']) != -1) {
                    $(".starterBoolCheckbox").hide();
                    $("#starterBool").prop('checked', false);
                }
                else {
                    $(".starterBoolCheckbox").show();
                }

                if ($.inArray(selectedGame, ['1', '2', '20', '23', '37']) != -1) {
                    $(".randomAbilityCheckbox").hide();
                    $("#randomAbilityBool").prop('checked', false);
                }
                else {
                    $(".randomAbilityCheckbox").show();
                }

                if ($.inArray(selectedGame, ['1', '2', '20', '23']) != -1) {
                    $("#alternateForms").hide();
                    $(".otherFormCheckbox").hide();
                    $("#Other").prop('checked', false);
                    $(".onePokemonFormBoolCheckbox").hide();
                    $("#onePokemonFormBool").prop('checked', false);
                }
                else {
                    $("#alternateForms").show();
                    $(".otherFormCheckbox").show();
                    $(".onePokemonFormBoolCheckbox").show();
                }

                if ($.inArray(selectedGame, ['0', '12', '13', '14', '15', '16']) != -1) {
                    if (!$('.megaevolutionFormCheckbox').is(':visible')) {
                        $(".megaevolutionFormCheckbox").show();
                    }

                    if ($('.megaevolutionFormCheckbox').prop('checked') && !$('.multipleMegaBoolCheckbox').is(':visible')) {
                        $(".multipleMegaBoolCheckbox").show();
                    }
                }
                else {
                    $(".multipleMegaBoolCheckbox").hide();
                    $("#multipleMegaBool").prop('checked', false);
                    $(".megaevolutionFormCheckbox").hide();
                    $("#MegaEvolution").prop('checked', false);
                }

                if (selectedGame == 16) {
                    $(".randomAbilityCheckbox").hide();
                    $("#randomAbilityBool").prop('checked', false);
                }

                if ($.inArray(selectedGame, ['0', '14', '15', '16', '17', '37']) != -1) {
                    if (!$('.alolanFormCheckbox').is(':visible')) {
                        $(".alolanFormCheckbox").show();
                        $('.gen1Checkbox').on('click', function () {
                            altCheck = checkAltFormChecks();
                        });
                    }
                }
                else {
                    $(".alolanFormCheckbox").hide();
                    $("#Alolan").prop('checked', false);
                }

                if ($.inArray(selectedGame, ['0', '17']) != -1) {
                    if (!$('.galarianFormCheckbox').is(':visible')) {
                        $(".galarianFormCheckbox").show();
                    }

                    if (!$('.gigantamaxFormCheckbox').is(':visible')) {
                        $(".gigantamaxFormCheckbox").show();
                    }

                    if ($('.gigantamaxFormCheckbox').prop('checked') && !$('.multipleGMaxBoolCheckbox').is(':visible')) {
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

                if ($.inArray(selectedGame, ['0', '37', '41', '42']) != -1) {
                    if (!$('.hisuianFormCheckbox').is(':visible')) {
                        $(".hisuianFormCheckbox").show();
                    }
                }
                else {
                    $(".hisuianFormCheckbox").hide();
                    $("#Hisuian").prop('checked', false);
                }

                if ($.inArray(selectedGame, ['0', '41', '42']) != -1) {
                    if (!$('.paldeanFormCheckbox').is(':visible')) {
                        $(".paldeanFormCheckbox").show();
                    }
                }
                else {
                    $(".paldeanFormCheckbox").hide();
                    $("#Paldean").prop('checked', false);
                }

                data.pokemonTypes;

                var typeId = $('input[name=typeSelection]:checked').val();

                $('.typeRadioOption').remove();

                var dropdownItem = $("<li>").addClass("dropdown-item generationOption typeRadioOption");
                var dropdownInput = $("<input>").attr("id", "type0").attr("name", "typeSelection").attr("type", "radio").val(0).attr("checked", "checked");;
                var dropdownLabel = $("<label>").attr("for", "type0").addClass("generatorOptionTitle").text("Any Type");
                $(dropdownItem).append(dropdownInput).append(dropdownLabel);
                $("#types").append($(dropdownItem));

                let len = Math.ceil(data.allTypes.length / 2);

                for (let i = 0; i < len; i++) {
                    var dropdownItem = $("<li>").addClass("dropdown-item generationOption typeRadioOption");
                    var dropdownInput = $("<input>").attr("id", "type" + data.allTypes[i].id).attr("name", "typeSelection").attr("type", "radio").val(data.allTypes[i].id);
                    var dropdownLabel = $("<label>").attr("for", "type" + data.allTypes[i].id).addClass("generatorOptionTitle").text(data.allTypes[i].name);
                    $(dropdownItem).append(dropdownInput).append(dropdownLabel);
                    $(".typeSelector section:first-of-type").append($(dropdownItem));
                }

                for (let i = len; i < data.allTypes.length; i++) {
                    var dropdownItem = $("<li>").addClass("dropdown-item generationOption typeRadioOption");
                    var dropdownInput = $("<input>").attr("id", "type" + data.allTypes[i].id).attr("name", "typeSelection").attr("type", "radio").val(data.allTypes[i].id);
                    var dropdownLabel = $("<label>").attr("for", "type" + data.allTypes[i].id).addClass("generatorOptionTitle").text(data.allTypes[i].name);
                    $(dropdownItem).append(dropdownInput).append(dropdownLabel);
                    $(".typeSelector section:last-of-type").append($(dropdownItem));
                }

                $("input[name=typeSelection][value=" + typeId + "]").prop('checked', true);

                $('.typeRadioOption input').off()

                $('.typeRadioOption input').on('click', function () {
                    checkTyping();
                    checkOtherOptions();
                });

                megaCheck = checkMegaCheck();
                altCheck = checkAltFormChecks();
                legendCheck = checkLegendaryChecks();
                checkOtherOptions();
            })
            .fail(function () {
                alert("Failed To Get Generations!");
            });
    }, checkTyping = function () {
        var boxChecked = false;
        if ($('.typeRadioOption input:checked').val() == "0") {
            boxChecked = true;
        }

        if (!boxChecked) {
            if ($('.noRepeatTypeBoolCheckbox').is(':visible')) {
                $(".noRepeatTypeBoolCheckbox").hide();
                $("#noRepeatTypeBool").prop('checked', false);
            }
        }
        else {
            $(".noRepeatTypeBoolCheckbox").show();
        }
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
        refreshGenerationsByGame();
        checkAltFormChecks();
        checkLegendaryChecks();
        checkMegaCheck();
        checkGigantamaxCheck();
        checkTyping();
        checkOtherOptions();
        generatorMenuCheck();
    };

$(function () {
    updateDropdown();
});

$('.otherSelector button').on('mouseover', function () {
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

$('.megaevolutionFormCheckbox').on('click', function () {
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

$('.typeRadioOption input').on('click', function () {
    checkTyping();
    checkOtherOptions();
});

$('.generatorButton').on('click', function () {
    var selectedGens = [], selectedLegendaries = [], selectedForms = [], selectedEvolutions = [], selectedGame, selectedType;
    $('.generationCheckbox input').each(function () {
        if ($(this).prop('checked')) {
            selectedGens.push(this.value);
        }
    });

    $('.alternateFormCheckbox input').each(function () {
        if ($(this).prop('checked')) {
            selectedForms.push(this.value);
        }
    });

    $('.legendaryCheckbox input').each(function () {
        if ($(this).prop('checked')) {
            selectedLegendaries.push(this.value);
        }
    });

    $('.evolutionCheckbox input').each(function () {
        if ($(this).prop('checked')) {
            selectedEvolutions.push(this.value);
        }
    });

    $('.gameRadioOption input').each(function () {
        if ($(this).prop('checked')) {
            selectedGame = this.value;
        }
    });

    $('.typeRadioOption input').each(function () {
        if ($(this).prop('checked')) {
            selectedType = this.value;
        }
    });
    
    $(".overlay").fadeIn(300);
    
    $('.teamRandomizerResults div').each(function () { $(this).remove() });
    
    removeEventButtons();

    $.ajax({
        url: '/get-pokemon-team/',
        method: 'POST',
        data: { 'pokemonCount': $('input[name=pokemonCount]').val(), 'selectedGens': selectedGens, 'selectedGameId': selectedGame, 'selectedType': selectedType, 'selectedLegendaries': selectedLegendaries, 'selectedForms': selectedForms, 'selectedEvolutions': selectedEvolutions, 'needsStarter': $("#starterBool").is(":checked"), 'onlyLegendaries': $("#legendaryBool").is(":checked"), 'onlyAltForms': $("#altFormBool").is(":checked"), 'multipleMegas': $("#multipleMegaBool").is(":checked"), 'multipleGMax': $("#multipleGMaxBool").is(":checked"), 'onePokemonForm': $("#onePokemonFormBool").is(":checked"), 'randomAbility': $("#randomAbilityBool").is(":checked"), 'noRepeatType': $("#noRepeatTypeBool").is(":checked") }
    })
        .done(function (data) {
            pokemonList = data.allPokemonChangedNames;
            originalNames = data.allPokemonOriginalNames;
            pokemonURLs = data.pokemonURLs;
            abilityList = data.pokemonAbilities;
            exportString = data.exportString;
            setTimeout(function(){
                $(".overlay").fadeOut(300);
            }, 500);

            fillGeneratedTable(data.appConfig);
        })
        .fail(function () {
            setTimeout(function(){
                $(".overlay").fadeOut(300);
            }, 500);
            
            alert("Failed To Get Team!");
        });
});