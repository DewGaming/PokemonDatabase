$(this).ready(function() {
    $(".attemptCount").css({
        'width': (($(".attemptCount").val().length + 1) * 8) + 'px'
    });
});

var updateCounter = function(data) {
    $(".attemptCount").val(data);
    $(".attemptCount").css({
        'width': (($(".attemptCount").val().length + 1) * 8) + 'px'
    });
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

$('.subtractAttemptCount').click(function() {
    subtractCounter();
});

$('.addAttemptCount').click(function() {
    addCounter();
});

$('.attemptCount').focusout(function() {
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

$(this).keypress(function(e) {
    if (e.keyCode == 45)
    {
        subtractCounter();
    }
    else if (e.which == 43 || e.which == 61)
    {
        addCounter();
    }
});