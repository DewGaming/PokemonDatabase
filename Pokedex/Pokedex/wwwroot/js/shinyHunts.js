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

function hideComplete() {
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
}

function hideIncomplete() {
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