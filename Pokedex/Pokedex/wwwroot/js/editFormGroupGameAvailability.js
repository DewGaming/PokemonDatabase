function updateGameAvailability(formGroupId) {
    var games = [];
    $('.gameAvailability input').each(function () {
        if ($(this).is(':checked')) {
            games.push(this.value);
        }
    });

    $.ajax({
        url: '/update-form-group-game-availability/',
        method: "POST",
        data: { "formGroupId": formGroupId, "games": games }
    })
        .done(function (data) {
            window.location = data;
        })
        .fail(function () {
            alert("Update Failed!");
        });
}