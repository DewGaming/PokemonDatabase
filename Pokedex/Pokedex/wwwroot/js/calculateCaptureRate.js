$(function() {
    var catchRate = $('.catchRate').attr('id');
    var x = Math.min(255, catchRate / 3);
    var y;
    var round = function (x) {
        return Math.round(x * 4096) / 4096;
    };

    if (x == 0) {
        y = 0;
    }
    else {
        y = Math.floor(round(65536 / round(Math.pow(round(255 / x), 3 / 16))));
    }

    var chance = Math.pow(y / 65536, 4);

    chance = Number(Math.round((chance * 100) + 'e' + 1) + 'e-' + 1);

    var chanceText = " (" + chance + "% With A Pokeball At Full HP)";

    $('.catchRate').text(catchRate + chanceText);
});