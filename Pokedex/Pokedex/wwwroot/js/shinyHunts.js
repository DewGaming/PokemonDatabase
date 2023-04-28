function incrementEncounter(shinyHuntId, increment) {
    $.ajax({
        url: '/increment-shiny-hunt-encounters/',
        method: "POST",
        data: { "shinyHuntId": shinyHuntId, "increment": increment }
    })
        .done(function (data) {
            $('.Hunt' + shinyHuntId + ' .encounters').html(data);
        })
        .fail(function () {
            alert("Update Failed!");
        });
}

function adjustEncountersManually(shinyHuntId) {
    var encounters = prompt("Total Number of Encounters");
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

    $('.page.generation' + generationId).each(function () {
        $(this).addClass('active');
    });

    $('.generationPercentage.generation' + generationId).each(function () {
        $(this).removeClass('hide');
    });
}