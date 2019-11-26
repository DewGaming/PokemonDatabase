$(this).ready(function() {
    updateCounterWidth();
});

var updateCounterWidth = function() {
    $(".attemptCount").css({
        'width': (($(".attemptCount").val().length + 1) * 8) + 'px'
    });
}

var updateCounter = function(data) {
    $(".attemptCount").val(data);
    updateCounterWidth();
}

var subtractCounter = function() {
    var shinyHuntId = $('#shinyHuntId').val();

    $.ajax({
        url: '/subtract-hunt-attempt/' + shinyHuntId,
        method: "POST"
    })
    .done(function(data) {
        updateCounter(data);
    })
    .fail( function() {
        alert("Subtraction Failed!");
    });
}

var addCounter = function() {
    var shinyHuntId = $('#shinyHuntId').val();

    $.ajax({
        url: '/add-hunt-attempt/' + shinyHuntId,
        method: "POST"
    })
    .done(function(data) {
        updateCounter(data);
    })
    .fail( function() {
        alert("Addition Failed!");
    });
}

$('.subtractAttemptCount').on('click', (function() {
    subtractCounter();
}));

$('.addAttemptCount').on('click', function() {
    addCounter();
});

$('.attemptCount').on('focusout', function() {
    var shinyHuntId = $('#shinyHuntId').val();
    var attemptCount = $('.attemptCount').val();

    $.ajax({
        url: '/update-hunt-attempt/' + shinyHuntId + '/' + attemptCount,
        method: "POST"
    })
    .done(function(data) {
        updateCounter(data);
    })
    .fail( function() {
        alert("Update Failed!");
    });
});

$('.attemptCount').on('input', function() {
    updateCounterWidth();
});

$(this).on('keydown', function(e) {
    var addArray = [32,38,39,61,107];
    var subtractArray = [37,40,109,173];
    if (jQuery.inArray(e.keyCode, subtractArray) != -1)
    {
        subtractCounter();
    }
    else if (jQuery.inArray(e.keyCode, addArray) != -1)
    {
        addCounter();
    }
});