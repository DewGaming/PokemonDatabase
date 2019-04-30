$('.subtractAttemptCount').click(function() {
    var shinyHuntId = $('#shinyHuntId').val(); // maybe .value()

    $.post('/subtract-attempt-count/' + shinyHuntId, function(returnData) {
        // This is the callback...
        alert('It is done!');
        $(".attemptCount").text(returnData);
    });
});