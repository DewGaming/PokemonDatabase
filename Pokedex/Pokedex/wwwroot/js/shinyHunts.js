"use strict";

var adjustIncrements = function (shinyHuntId) {
    var currentIncrements = $('.Hunt' + shinyHuntId + ' .increments').html();
    var increments = prompt("Increment Amount", currentIncrements);
    if ($.isNumeric(increments)) {
        if (increments < 1) {
            increments = 1;
        }
        $('.Hunt' + shinyHuntId + ' .increments').html(increments);
        $.ajax({
            url: '/set-shiny-hunt-increments/',
            method: "POST",
            data: { "shinyHuntId": shinyHuntId, "increments": increments }
        })
            .done(function () {
                connection.invoke("UpdateHuntAttributes", parseInt(shinyHuntId), parseInt(-1), parseInt(-1), parseInt(increments)).catch(function (err) {
                    return console.error(err.toString());
                });
            })
            .fail(function () {
                if (isLocalHost) {
                    alert("Update Failed!");
                }
            });
    } else if (increments != null) {
        alert("Entered Phases Need to be a Number")
    }
}, adjustEncountersManually = function (shinyHuntId) {
    var currentEncounters = $('.Hunt' + shinyHuntId + ' .encounters').html();
    var encounters = prompt("Total Number of Encounters", currentEncounters);
    if ($.isNumeric(encounters)) {
        if (encounters < 0) {
            encounters = 0;
        }
        $('.Hunt' + shinyHuntId + ' .encounters').html(encounters);
        $.ajax({
            url: '/set-shiny-hunt-encounters/',
            method: "POST",
            data: { "shinyHuntId": shinyHuntId, "encounters": encounters }
        })
            .done(function () {
                connection.invoke("UpdateHuntAttributes", parseInt(shinyHuntId), parseInt(encounters), parseInt(-1), parseInt(-1)).catch(function (err) {
                    return console.error(err.toString());
                });
            })
            .fail(function () {
                if (isLocalhost) {
                    alert("Update Failed!");
                }
            });
    } else if (encounters != null) {
        alert("Entered Encounters Need to be a Number")
    }
}, updatePinStatus = function (shinyHuntId, pinned) {
    if (pinned) {
        $('.Hunt' + shinyHuntId).addClass('HuntGamePin');
        $('.Hunt' + shinyHuntId).removeClass('hide');
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

    if ($('.HuntGamePin').length <= 0) {
        $('.pinnedHunts').addClass('hide');
        $('#Game0').trigger('click');
    }
}

$('.encounterIncrement.pointer').on('click', function () {
    adjustIncrements($(this).prop('id'));
});

$('.currentEncounters.pointer').on('click', function () {
    adjustEncountersManually($(this).prop('id'));
});

//#region SignalR Connection
const connection = new signalR.HubConnectionBuilder().withUrl("/hub/shinyHunts").withAutomaticReconnect().build();
connection.start().catch(function (err) {
    return console.error(err.toString());
});

connection.on("SendHuntAttributes", function (shinyHuntId, encounters, phases, increments) {
    if (encounters != -1) {
        $('.Hunt' + shinyHuntId + ' .encounters').html(encounters);
    }

    if (phases != -1) {
        $('.Hunt' + shinyHuntId + ' .phases').html(phases);
        if (phases == 1) {
            $('.Hunt' + shinyHuntId + ' .currentEncounters b').html('Encounters: ')
            $('.Hunt' + shinyHuntId + ' .phaseCounter').addClass('hide');
        } else {
            $('.Hunt' + shinyHuntId + ' .currentEncounters b').html('Current Phase Encounters: ')
            $('.Hunt' + shinyHuntId + ' .phaseCounter').removeClass('hide');
        }
    }

    if (increments != -1) {
        $('.Hunt' + shinyHuntId + ' .increments').html(increments);
    }
});

connection.on("SendPinStatus", function (shinyHuntId, isPinned) {
    updatePinStatus(shinyHuntId, isPinned);
});

connection.on("RemoveShinyHunt", function (shinyHuntId) {
    $('.Hunt' + shinyHuntId).remove();
});

connection.on("FinishShinyHunt", function (shinyHuntId) {
    $.ajax({
        url: '/add-completed-shiny-hunt/',
        method: "POST",
        data: { "shinyHuntId": shinyHuntId }
    })
        .done(function (view) {
            $('.incompletedHunts .grid-container .Hunt' + shinyHuntId).remove();
            $('.completedHunts .grid-container').prepend(view);
        })
        .fail(function () {
            if (isLocalhost) {
                alert("Update Failed!");
            }
        });
});
//#endregion SignalR Connections

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
        .done(function () {
            connection.invoke("UpdateHuntAttributes", parseInt(shinyHuntId), parseInt(currentEncounters + incrementAmount), parseInt(-1), parseInt(-1)).catch(function (err) {
                return console.error(err.toString());
            });
        })
        .fail(function () {
            if (isLocalhost) {
                alert("Update Failed!");
            }
        });
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
            connection.invoke("UpdatePinStatus", parseInt(shinyHuntId), Boolean(data)).catch(function (err) {
                return console.error(err.toString());
            });
        })
        .fail(function () {
            if (isLocalhost) {
                alert("Update Failed!");
            }
        });
}

function abandonHunt(shinyHuntId, pokemonName) {
    var typedName = prompt("Abandoning a Hunt is not reverisble. To confirm, please type the pokemon's name:\r\n\r\n" + pokemonName);
    typedName = typedName.toLowerCase();
    if (typedName == "flabebe") {
        typedName = typedName.replace("flabebe", "flabébé");
    }

    if (typedName == pokemonName.toLowerCase()) {
        $.ajax({
            url: '/abandon-shiny-hunt/',
            data: { "shinyHuntId": shinyHuntId }
        })
            .done(function () {
                connection.invoke("DeleteShinyHunt", parseInt(shinyHuntId)).catch(function (err) {
                    return console.error(err.toString());
                });
                $('.Hunt' + shinyHuntId).remove();
            })
            .fail(function () {
                if (isLocalhost) {
                    alert("Update Failed!");
                }
            });
    }
}

function giveSharableLink(username) {
    var url = window.location.href + '/' + username.toLowerCase();
    if (navigator.clipboard) {
        navigator.clipboard.writeText(url)
            .then(() => {
                if (window.confirm('Sharable link has been copied to your clipboard. If you would like to see this page for yourself, press OK. Otherwise, press Cancel.')) {
                    window.open(url, '_blank');
                };
            })

        console.log(url);
    }
}