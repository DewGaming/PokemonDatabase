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
    $.ajax({
        url: '/increment-shiny-hunt-encounters/',
        method: "POST",
        data: { "shinyHuntId": shinyHuntId, "incrementAmount": $('.Hunt' + shinyHuntId + ' .increments').html() }
    })
        .done(function (data) {
            $('.Hunt' + shinyHuntId + ' .encounters').html(data);
        })
        .fail(function () {
            alert("Update Failed!");
        });
}

function incrementPhase(shinyHuntId) {
    $.ajax({
        url: '/increment-shiny-hunt-phases/',
        method: "POST",
        data: { "shinyHuntId": shinyHuntId }
    })
        .done(function (data) {
            $('.Hunt' + shinyHuntId + ' .phases').html(data);
            $('.Hunt' + shinyHuntId + ' .encounterP b').html('Current Phase Encounters: ')
            $('.Hunt' + shinyHuntId + ' .encounters').html(0);
            $('.Hunt' + shinyHuntId + ' .phaseCounter').removeClass('hide');
        })
        .fail(function () {
            alert("Update Failed!");
        });
}

function adjustEncountersManually(shinyHuntId) {
    var currentEncounters = $('.Hunt' + shinyHuntId + ' .encounters').html();
    var encounters = prompt("Total Number of Encounters", currentEncounters);
    if ($.isNumeric(encounters)) {
        $.ajax({
            url: '/set-shiny-hunt-encounters/',
            method: "POST",
            data: { "shinyHuntId": shinyHuntId, "encounters": encounters }
        })
            .done(function (data) {
                $('.Hunt' + shinyHuntId + ' .encounters').html(data);
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
        $.ajax({
            url: '/set-shiny-hunt-phases/',
            method: "POST",
            data: { "shinyHuntId": shinyHuntId, "phases": phases }
        })
            .done(function (data) {
                $('.Hunt' + shinyHuntId + ' .phases').html(data);
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
        $.ajax({
            url: '/set-shiny-hunt-increments/',
            method: "POST",
            data: { "shinyHuntId": shinyHuntId, "increments": increments }
        })
            .done(function (data) {
                $('.Hunt' + shinyHuntId + ' .increments').html(data);
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

    $('.hideAltFormsButton').each(function () {
        $(this).removeClass('hide');
    });

    $('.showAltFormsButton').each(function () {
        $(this).addClass('hide');
    });

    $('.shiniesFoundPercentWithAlts').each(function () {
        $(this).removeClass('hide');
    });

    $('.shiniesFoundPercentNoAlts').each(function () {
        $(this).addClass('hide');
    });
}

function hideCaptured() {
    $('.shadowed:not(.uncaptured)').each(function () {
        $(this).addClass('hide');
    });

    $('.hideCapturedButton').each(function () {
        $(this).addClass('hide');
    });

    $('.showCapturedButton').each(function () {
        $(this).removeClass('hide');
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

    $('.hideCapturedButton').each(function () {
        $(this).removeClass('hide');
    });

    $('.showCapturedButton').each(function () {
        $(this).addClass('hide');
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