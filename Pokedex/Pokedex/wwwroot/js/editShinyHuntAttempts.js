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

$(this).on('keypress', function(e) {
    if (e.keyCode == 45)
    {
        subtractCounter();
    }
    else if (e.which == 43 || e.which == 61)
    {
        addCounter();
    }
});