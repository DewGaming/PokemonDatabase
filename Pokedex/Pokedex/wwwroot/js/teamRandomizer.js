var pokemonList, pokemonURLs, typeList, exportString, fillGeneratedTable = function (appConfig) {
    for (var i = 0; i < 6; i++) {
        $('.teamRandomizerResults').append($('<div>', {
            class: "generatorPicture shadowed pokemon" + (i + 1)
        }));
    }

    for (var i = 0; i < originalNames.length; i++) {
        var imageLocation = appConfig.officialPokemonImageUrl;
        if (Math.floor((Math.random() * 4096) + 1) == 4096 && !originalNames[i].isShinyLocked) {
            console.log("Shiny Pokemon!");
            $('.pokemon' + (i + 1)).addClass('shiny');
            imageLocation = appConfig.shinyPokemonImageUrl;
            $.ajax({
                url: '/shiny-pokemon-found/',
                method: 'POST'
            })
        }
        $('.pokemon' + (i + 1)).append('<a href="' + pokemonURLs[i] + '" target="_blank"><img loading="lazy" title="' + pokemonList[i].name.replace('_', ' ') + ' (Click to learn more)" src="' + appConfig.webUrl + imageLocation + pokemonList[i].id + '.png" /></a>');
    }

    if ($(window).width() >= 406) {
        $('<button class="btn btn-primary saveTeamButton">Save Team</button>').insertAfter('.typeSelector');
        $('<button class="btn btn-primary exportTeamButton">Export Team</button>').insertAfter('.generatorButton');
    }
    else {
        $('<br class="mobileBreak" />').insertAfter('.generatorButton');
        $('<button class="btn btn-primary exportTeamButton">Export Team</button>').insertAfter('.mobileBreak');
        $('<button class="btn btn-primary saveTeamButton">Save Team</button>').insertAfter('.mobileBreak');
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
        if (navigator.clipboard) {
            navigator.clipboard.writeText(exportString)
                .then(() => {
                    alert("Team has been copied to your clipboard!");
                })
                .catch((error) => {
                    alert("Team was unable to be copied to your clipboard!");
                });

            console.log(exportString);
        } else {
            alert("Team was unable to be copied to your clipboard!");
        }
    });
}, refreshSaveEvent = function () {
    $('.saveTeamButton').off();

    $('.saveTeamButton').on('click', function () {
        var pokemonStringList = [], shinyPokemonList = [];
        var teamName = prompt("Please Enter Team Name");
        pokemonList.forEach(function (item) {
            pokemonStringList.push(item.id);
        });

        $('.generatorPicture').each(function () {
            if ($(this).hasClass('shiny')) {
                shinyPokemonList.push(true);
            } else {
                shinyPokemonList.push(false);
            }
        });

        $('.gameRadioOption input').each(function () {
            if ($(this).prop('checked')) {
                selectedGame = this.value;
            }
        });

        $.ajax({
            url: '/save-pokemon-team/',
            method: 'POST',
            data: { 'selectedGame': selectedGame, 'pokemonIdList': pokemonStringList, 'shinyPokemonList': shinyPokemonList, 'pokemonTeamName': teamName }
        })
            .done(function (data) {
                alert(data);
            })
            .fail(function () {
                alert('Failed to save pokemon team. Website owner has been notified.');
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
            var gensChecked = [], formsAvailable = $.grep(data.allFormGroupGameDetails, function (n) { return n.gameId == selectedGame; });
            if (selectedGame == 0) {
                formsAvailable = data.allFormGroupGameDetails;
            }

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

            $.each(data.allFormGroups, function (index, formGroup) {
                var checkboxClass = formGroup.name.toLowerCase().concat("FormCheckbox").replace(/\s/g, '');
                var checkboxTag = formGroup.name.replace(/\s/g, '');
                if (formsAvailable.find(x => x.formGroup.name == formGroup.name)) {
                    if (!$('.' + checkboxClass).is(':visible')) {
                        $('.' + checkboxClass).show();
                    }
                }
                else {
                    $('.' + checkboxClass).hide();
                    $('#' + checkboxTag).prop('checked', false);
                }
            })

            var types = [];
            $('input[name=typeSelection]:checked').each(function () {
                types.push($(this).val());
            });

            $('.typeCheckboxOption').remove();

            let len = Math.ceil(data.allTypes.length / 2);

            for (let i = 0; i < len; i++) {
                var dropdownItem = $("<li>").addClass("dropdown-item generationOption typeCheckboxOption");
                var dropdownInput = $("<input>").attr("id", "type" + data.allTypes[i].id).attr("name", "typeSelection").attr("type", "checkbox").val(data.allTypes[i].id);
                var dropdownLabel = $("<label>").attr("for", "type" + data.allTypes[i].id).addClass("generatorOptionTitle").text(data.allTypes[i].name);
                $(dropdownItem).append(dropdownInput).append(dropdownLabel);
                $(".typeSelector section:first-of-type").append($(dropdownItem));
            }

            for (let i = len; i < data.allTypes.length; i++) {
                var dropdownItem = $("<li>").addClass("dropdown-item generationOption typeCheckboxOption");
                var dropdownInput = $("<input>").attr("id", "type" + data.allTypes[i].id).attr("name", "typeSelection").attr("type", "checkbox").val(data.allTypes[i].id);
                var dropdownLabel = $("<label>").attr("for", "type" + data.allTypes[i].id).addClass("generatorOptionTitle").text(data.allTypes[i].name);
                $(dropdownItem).append(dropdownInput).append(dropdownLabel);
                $(".typeSelector section:last-of-type").append($(dropdownItem));
            }

            $.each(types, function () {
                $("input[name=typeSelection][value=" + this + "]").prop('checked', true);
            });

            $('.typeCheckboxOption input').off()

            $('.typeCheckboxOption input').on('click', function () {
                checkOtherOptions();
            });

            megaCheck = checkMegaCheck();
            altCheck = checkAltFormChecks();
            legendCheck = checkLegendaryChecks();
            checkOtherOptions();
        })
        .fail(function () {
            if (isLocalhost) {
                alert("Failed To Get Generations!");
            }
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
    refreshGenerationsByGame();
    checkAltFormChecks();
    checkLegendaryChecks();
    checkMegaCheck();
    checkGigantamaxCheck();
    checkOtherOptions();
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

$('.typeCheckboxOption input').on('click', function () {
    checkOtherOptions();
});

$('.generatorButton').on('click', function () {
    var selectedGens = [], selectedLegendaries = [], selectedForms = [], selectedEvolutions = [], selectedTypes = [], selectedGameId;
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

    $('.typeCheckboxOption input').each(function () {
        if ($(this).prop('checked')) {
            selectedTypes.push(this.value);
        }

    $('.gameRadioOption input').each(function () {
        if ($(this).prop('checked')) {
            selectedGameId = this.value;
        }
    });
    });

    $(".overlay").fadeIn(300);

    $('.teamRandomizerResults div').each(function () { $(this).remove() });

    removeEventButtons();

    $.ajax({
        url: '/get-pokemon-team/',
        method: 'POST',
        data: { 'pokemonCount': $('input[name=pokemonCount]').val(), 'selectedGens': selectedGens, 'selectedTypes': selectedTypes, 'selectedGameId': selectedGameId, 'selectedLegendaries': selectedLegendaries, 'selectedForms': selectedForms, 'selectedEvolutions': selectedEvolutions, 'onlyLegendaries': $("#legendaryBool").is(":checked"), 'onlyAltForms': $("#altFormBool").is(":checked"), 'multipleMegas': $("#multipleMegaBool").is(":checked"), 'multipleGMax': $("#multipleGMaxBool").is(":checked"), 'onePokemonForm': $("#onePokemonFormBool").is(":checked"), 'monotypeOnly': $("#monotypeBool").is(":checked"), 'noRepeatType': $("#noRepeatTypeBool").is(":checked"), 'allowIncomplete': $("#allowIncompleteBool").is(":checked"), 'onlyUseRegionalDexes': $("#onlyUseRegionalDexesBool").is(":checked") }
    })
        .done(function (data) {
            pokemonList = data.allPokemonChangedNames;
            originalNames = data.allPokemonOriginalNames;
            pokemonURLs = data.pokemonURLs;
            exportString = data.exportString;
            setTimeout(function () {
                $(".overlay").fadeOut(300);
            });

            fillGeneratedTable(data.appConfig);
        })
        .fail(function () {
            alert("Failed To Get Team! Please Try Again.");
            
            $(".overlay").fadeOut(300);
        });
});