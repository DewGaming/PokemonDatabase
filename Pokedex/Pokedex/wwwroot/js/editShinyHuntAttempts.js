$('.subtractAttemptCount').click(function() {
    var shinyHuntId = $('#shinyHuntId').val();

    $.ajax({
        url: '/subtract-hunt-attempt/' + shinyHuntId,
        method: "POST"
    })
    .done(function(data) {
        $(".attemptCount").text(data);
    })
    .fail( function() {
        alert("Subtraction Failed!");
    });
});

$('.addAttemptCount').click(function() {
    var shinyHuntId = $('#shinyHuntId').val();

    $.ajax({
        url: '/add-hunt-attempt/' + shinyHuntId,
        method: "POST"
    })
    .done(function(data) {
        $(".attemptCount").text(data);
    })
    .fail( function() {
        alert("Addition Failed!");
    });
});