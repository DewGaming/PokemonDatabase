function lookupHuntsInGame(element, gameId) {
    if (!$('.active').is($('#Game' + gameId))) {
        $('button').each(function () {
            $(this).removeClass('active');
        });

        $('.pokemonList').removeClass('active');

        $('.shadowed.hide').each(function () {
            $(this).removeClass('hide');
        });

        if (!$(element).hasClass('incompleteAllGames') && !$(element).hasClass('completeAllGames')) {
            $('div.shadowed').not('.HuntGame' + gameId).each(function () {
                $(this).addClass('hide');
            });
        }

        if (!$(element).hasClass('incompleteAllGames') && !$(element).hasClass('pinnedHunts') && !$(element).hasClass('completeAllGames')) {
            $('.gameHuntedIn').each(function () {
                $(this).addClass('hide');
            });
        } else {
            $('.gameHuntedIn').each(function () {
                $(this).removeClass('hide');
            });
        }

        if (gameId != '0') {
            $('.shiniesFoundCount').html($('.completedHunts .HuntGame' + gameId).length)
        } else {
            $('.shiniesFoundCount').html($('.completedHunts .grid-container').children().length)
        }

        $('button#Game' + gameId).addClass('active');
        $('.pokemonList').addClass('active');
        $('.active.hide').removeClass('active');
    }
}

function incrementEncounter(shinyHuntId) {
    var currentEncounters = parseInt($('.Hunt' + shinyHuntId + ' .encounters').html());
    var incrementAmount = parseInt($('.Hunt' + shinyHuntId + ' .increments').html());
    $('.Hunt' + shinyHuntId + ' .encounters').html(currentEncounters + incrementAmount);
    $.ajax({
        url: '/set-shiny-hunt-encounters/',
        method: "POST",
        data: { "shinyHuntId": shinyHuntId, "encounters": currentEncounters + incrementAmount }
    })
        .fail(function () {
            alert("Update Failed!");
        });
}

function incrementPhase(shinyHuntId) {
    var currentPhases = parseInt($('.Hunt' + shinyHuntId + ' .phases').html());
    $('.Hunt' + shinyHuntId + ' .phases').html(currentPhases + 1);
    $('.Hunt' + shinyHuntId + ' .encounterP b').html('Current Phase Encounters: ')
    $('.Hunt' + shinyHuntId + ' .encounters').html(0);
    $('.Hunt' + shinyHuntId + ' .phaseCounter').removeClass('hide');
    $.ajax({
        url: '/increment-shiny-hunt-phases/',
        method: "POST",
        data: { "shinyHuntId": shinyHuntId, "phases": currentPhases + 1 }
    })
        .fail(function () {
            alert("Update Failed!");
        });
}

function adjustEncountersManually(shinyHuntId) {
    var currentEncounters = $('.Hunt' + shinyHuntId + ' .encounters').html();
    var encounters = prompt("Total Number of Encounters", currentEncounters);
    if ($.isNumeric(encounters)) {
        $('.Hunt' + shinyHuntId + ' .encounters').html(encounters);
        $.ajax({
            url: '/set-shiny-hunt-encounters/',
            method: "POST",
            data: { "shinyHuntId": shinyHuntId, "encounters": encounters }
        })
            .fail(function () {
                alert("Update Failed!");
            });
    } else if (encounters != null) {
        alert("Entered Encounters Need to be a Number")
    }
}

function adjustPhasesManually(shinyHuntId) {
    var currentPhases = $('.Hunt' + shinyHuntId + ' .phases').html();
    var phases = prompt("Total Number of Phases", currentPhases);
    if ($.isNumeric(phases)) {
        $('.Hunt' + shinyHuntId + ' .phases').html(phases);
        $.ajax({
            url: '/set-shiny-hunt-phases/',
            method: "POST",
            data: { "shinyHuntId": shinyHuntId, "phases": phases }
        })
            .fail(function () {
                alert("Update Failed!");
            });
    } else if (phases != null) {
        alert("Entered Phases Need to be a Number")
    }
}

function adjustIncrements(shinyHuntId) {
    var currentIncrements = $('.Hunt' + shinyHuntId + ' .increments').html();
    var increments = prompt("Increment Amount", currentIncrements);
    if ($.isNumeric(increments)) {
        $('.Hunt' + shinyHuntId + ' .increments').html(increments);
        $.ajax({
            url: '/set-shiny-hunt-increments/',
            method: "POST",
            data: { "shinyHuntId": shinyHuntId, "increments": increments }
        })
            .fail(function () {
                alert("Update Failed!");
            });
    } else if (increments != null) {
        alert("Entered Phases Need to be a Number")
    }
}

function hideIncomplete() {
    $('.incompletedHunts').addClass('hide');
    $('.completedHunts').removeClass('hide');
    if ($('.incompletedHunts').hasClass('active')) {
        $('.incompletedHunts').removeClass('active');
        $('.completedHunts').addClass('active');
    }
    $('.completedHuntsButton').addClass('hide');
    $('.currentlyHuntingButton').removeClass('hide');
    $('.uncapturedGamesList').addClass('hide');
    $('.capturedGamesList').removeClass('hide');
    if ($('.shiniesFoundCount').html() == "") {
        $('.shiniesFoundCount').html($('.completedHunts .grid-container').children().length)
    }
}

function hideComplete() {
    $('.completedHunts').addClass('hide');
    $('.incompletedHunts').removeClass('hide');
    if ($('.completedHunts').hasClass('active')) {
        $('.completedHunts').removeClass('active');
        $('.incompletedHunts').addClass('active');
    }
    $('.currentlyHuntingButton').addClass('hide');
    $('.completedHuntsButton').removeClass('hide');
    $('.capturedGamesList').addClass('hide');
    $('.uncapturedGamesList').removeClass('hide');
}

function hideAltForms() {
    $('.altForm').each(function () {
        $(this).addClass('hide');
    });

    $.ajax({
        url: '/toggle-shiny-alt-forms/',
        method: "POST",
        data: { 'altFormToggle': 'hide' }
    })
        .done(function () {
            $('.hideAltFormsButton').each(function () {
                $(this).addClass('hide');
            });
        
            $('.showAltFormsButton').each(function () {
                $(this).removeClass('hide');
            });
        
            $('.shiniesFoundPercentWithAlts').each(function () {
                $(this).addClass('hide');
            });

            $('.shiniesFoundPercentNoAlts').each(function () {
                $(this).removeClass('hide');
            });
        })
        .fail(function () {
            alert("Failed to toggle shiny alt forms!");
        });
}

function showAltForms() {
    $('.altForm.uncaptured').each(function () {
        $(this).removeClass('hide');
    });

    if ($('.showCapturedButton').hasClass('hide')) {
        $('.altForm:not(.uncaptured)').each(function () {
            $(this).removeClass('hide');
        });
    }

    $.ajax({
        url: '/toggle-shiny-alt-forms/',
        method: "POST",
        data: { 'altFormToggle': 'show' }
    })
        .done(function () {
            $('.showAltFormsButton').each(function () {
                $(this).addClass('hide');
            });
        
            $('.hideAltFormsButton').each(function () {
                $(this).removeClass('hide');
            });
        
            $('.shiniesFoundPercentNoAlts').each(function () {
                $(this).addClass('hide');
            });

            $('.shiniesFoundPercentWithAlts').each(function () {
                $(this).removeClass('hide');
            });
        })
        .fail(function () {
            alert("Failed to toggle shiny alt forms!");
        });
}

function hideCaptured() {
    $('.shadowed:not(.uncaptured)').each(function () {
        $(this).addClass('hide');
    });

    $.ajax({
        url: '/toggle-captured-shinies/',
        method: "POST",
        data: { 'capturedShiniesToggle': 'hide' }
    })
        .done(function () {
            $('.hideCapturedButton').each(function () {
                $(this).addClass('hide');
            });
        
            $('.showCapturedButton').each(function () {
                $(this).removeClass('hide');
            });
        })
        .fail(function () {
            alert("Failed to toggle captured shinies!");
        });
}

function showCaptured() {
    $('.shadowed:not(.uncaptured):not(.altForm)').each(function () {
        $(this).removeClass('hide');
    });

    if ($('.showAltFormsButton').hasClass('hide')) {
        $('.shadowed.altForm:not(.uncaptured)').each(function () {
            $(this).removeClass('hide');
        });
    }

    $.ajax({
        url: '/toggle-captured-shinies/',
        method: "POST",
        data: { 'capturedShiniesToggle': 'show' }
    })
        .done(function () {
            $('.showCapturedButton').each(function () {
                $(this).addClass('hide');
            });

            $('.hideCapturedButton').each(function () {
                $(this).removeClass('hide');
            });
        })
        .fail(function () {
            alert("Failed to toggle captured shinies!");
        });
}

function lookupGeneration(generationId) {
    $('.active').each(function () {
        $(this).removeClass('active');
    });

    $('.generationPercentage:not(.hide)').each(function () {
        $(this).addClass('hide');
    });

    $('button#Generation' + generationId).addClass('active');

    $('.generationPercentage.generation' + generationId).each(function () {
        $(this).removeClass('hide');
    });

    if (generationId != 0) {
        $('.page.generation' + generationId).each(function () {
            $(this).addClass('active');
        });

        $('.generationHeaders').each(function () {
            $(this).hide();
        });
    } else {
        $('.page').each(function () {
            $(this).addClass('active');
        });

        $('.generationHeaders').each(function () {
            $(this).show();
        });
    }
}

function togglePin(shinyHuntId) {
    $.ajax({
        url: '/toggle-hunt-pin/',
        method: "POST",
        data: { "shinyHuntId": shinyHuntId }
    })
        .done(function (data) {
            if (data) {
                $('.Hunt' + shinyHuntId).addClass('HuntGamePin');
                $('.Hunt' + shinyHuntId + ' .pin').addClass('pinned');
                $('.Hunt' + shinyHuntId + ' .pinned').removeClass('pin');
                if ($('.pinnedHunts').hasClass('hide')) {
                    $('.pinnedHunts').removeClass('hide');
                }
            } else {
                $('.Hunt' + shinyHuntId).removeClass('HuntGamePin');
                $('.Hunt' + shinyHuntId + ' .pinned').addClass('pin');
                $('.Hunt' + shinyHuntId + ' .pin').removeClass('pinned');
                if ($('.pinnedHunts').hasClass('active')) {
                    $('.Hunt' + shinyHuntId).addClass('hide');
                }
            }
        })
        .fail(function () {
            alert("Update Failed!");
        });
}

function abandonHunt(shinyHuntId, pokemonName) {
    var typedName = prompt("Abandoning a Hunt is not reverisble. To confirm, please type the pokemon's name:\r\n\r\n" + pokemonName);
    if (typedName.toLowerCase() == "flabebe") {
        typedName = "Flabébé";
    }

    if (typedName.toLowerCase() == pokemonName.toLowerCase()) {
        $.ajax({
            url: '/abandon-shiny-hunt/',
            data: { "shinyHuntId": shinyHuntId }
        })
            .done(function () {
                $('.Hunt' + shinyHuntId).remove();
            })
            .fail(function () {
                alert("Update Failed!");
            });
    }
}