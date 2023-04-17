function updateGameAvailability(huntingMethodId) {
    var games = [];
    $('.gameAvailability input').each(function () {
        if ($(this).is(':checked')) {
            games.push(this.value);
        }
    });

    $.ajax({
        url: '/update-hunting-method-game-availability/',
        method: "POST",
        data: { "huntingMethodId": huntingMethodId, "games": games }
    })
        .done(function (data) {
            window.location = data;
        })
        .fail(function () {
            alert("Update Failed!");
        });
}