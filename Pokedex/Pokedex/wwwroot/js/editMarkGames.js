function updateMarkGames(gameId) {
    var mark = [];
    $('.markGameAvailability input').each(function () {
        if ($(this).is(':checked')) {
            mark.push(this.value);
        }
    });

    $(".overlay").fadeIn(300);

    $.ajax({
        url: '/update-mark-games/',
        method: "POST",
        data: { "gameId": gameId, "markList": mark }
    })
        .done(function (data) {
            window.location = data;
        })
        .fail(function () { 
            setTimeout(function () {
                $(".overlay").fadeOut(300);
            });

            alert("Update Failed!");
        });
}