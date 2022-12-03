var grabPokemon = function (gameId, health, attack, defense, specialAttack, specialDefense, speed) {
    $('.evYields').empty();
    if (gameId != "") {
        $('.evYields').load('/get-pokemon-by-ev-yields/', { 'gameId': gameId, 'health': health, 'attack': attack, 'defense': defense, 'specialAttack': specialAttack, 'specialDefense': specialDefense, 'speed': speed });
        
        setTimeout(function () {
            $(".overlay").fadeOut(300);
        }, 750);
    }
}, checkTotalEVs = function (slider, label) {
    var totalEVs = Number($('.healthSlider').val()) + Number($('.attackSlider').val()) + Number($('.defenseSlider').val()) + Number($('.specialAttackSlider').val()) + Number($('.specialDefenseSlider').val()) + Number($('.speedSlider').val());

    if (totalEVs > 3) {
        var otherEVTotal = 0;
        if (!$(slider).hasClass('healthSlider')) {
            otherEVTotal += Number($('.healthSlider').val());
        }

        if (!$(slider).hasClass('attackSlider')) {
            otherEVTotal += Number($('.attackSlider').val());
        }

        if (!$(slider).hasClass('defenseSlider')) {
            otherEVTotal += Number($('.defenseSlider').val());
        }

        if (!$(slider).hasClass('specialAttackSlider')) {
            otherEVTotal += Number($('.specialAttackSlider').val());
        }
        if (!$(slider).hasClass('specialDefenseSlider')) {
            otherEVTotal += Number($('.specialDefenseSlider').val());
        }

        if (!$(slider).hasClass('speedSlider')) {
            otherEVTotal += Number($('.speedSlider').val());
        }

        var remainingEVs = Number(3) - Number(otherEVTotal);
        $(slider).val(remainingEVs);
        $(label).text($(slider).val());
    }
}, setupLabels = function (health, attack, defense, specialAttack, specialDefense, speed) {
    $('#healthVal').text(health);
    $('#attackVal').text(attack);
    $('#defenseVal').text(defense);
    $('#specialAttackVal').text(specialAttack);
    $('#specialDefenseVal').text(specialDefense);
    $('#speedVal').text(speed);
    $('.healthSlider').val(health);
    $('.attackSlider').val(attack);
    $('.defenseSlider').val(defense);
    $('.specialAttackSlider').val(specialAttack);
    $('.specialDefenseSlider').val(specialDefense);
    $('.speedSlider').val(speed);
};

$('.healthSlider').on('input', function () {
    $('#healthVal').text($(this).val());
    checkTotalEVs($(this), $('#healthVal'));
});

$('.attackSlider').on('input', function () {
    $('#attackVal').text($(this).val());
    checkTotalEVs($(this), $('#attackVal'));
});

$('.defenseSlider').on('input', function () {
    $('#defenseVal').text($(this).val());
    checkTotalEVs($(this), $('#defenseVal'));
});

$('.specialAttackSlider').on('input', function () {
    $('#specialAttackVal').text($(this).val());
    checkTotalEVs($(this), $('#specialAttackVal'));
});

$('.specialDefenseSlider').on('input', function () {
    $('#specialDefenseVal').text($(this).val());
    checkTotalEVs($(this), $('#specialDefenseVal'));
});

$('.speedSlider').on('input', function () {
    $('#speedVal').text($(this).val());
    checkTotalEVs($(this), $('#speedVal'));
});

jQuery(function ($) {
    $(document).ajaxSend(function () {
        $(".overlay").fadeIn(300);
    })
});

$(function () {
    $(setupLabels(0, 0, 0, 0, 0, 0));
    $('.gameSelectList').val($('.gameSelectList option:last').val());
});

$(".gameSelectList").on('change', function () {
    grabPokemon($('.gameList > select').val(), $('.healthSlider').val(), $('.attackSlider').val(), $('.defenseSlider').val(), $('.specialAttackSlider').val(), $('.specialDefenseSlider').val(), $('.speedSlider').val());
});

$(".slider").on('change', function () {
    grabPokemon($('.gameList > select').val(), $('.healthSlider').val(), $('.attackSlider').val(), $('.defenseSlider').val(), $('.specialAttackSlider').val(), $('.specialDefenseSlider').val(), $('.speedSlider').val());
});