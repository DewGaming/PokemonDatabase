$('.subtractAttemptCount').click(function() {
    var shinyHuntId = $('#shinyHuntId').val(); // maybe .value()

    $.post('/subtract-attempt-count/' + shinyHuntId, function(returnData) {
        // This is the callback...
        alert('Attempt Count Subtracted');
        $(".attemptCount").text(returnData);
    });
});

$('.addAttemptCount').click(function() {
    var shinyHuntId = $('#shinyHuntId').val(); // maybe .value()

    $.post('/add-attempt-count/' + shinyHuntId, function(returnData) {
        // This is the callback...
        alert('Attempt Count Increased');
        $(".attemptCount").text(returnData);
    });
});