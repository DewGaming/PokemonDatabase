var checkTotalEVs = function (slider, label) {
    var totalEVs = Number($('.healthSlider').val()) + Number($('.attackSlider').val()) + Number($('.defenseSlider').val()) + Number($('.specialAttackSlider').val()) + Number($('.specialDefenseSlider').val()) + Number($('.speedSlider').val());

    if (totalEVs > 510) {
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

        var remainingEVs = Number(510) - Number(otherEVTotal);
        $(slider).val(remainingEVs);
        $(label).text($(slider).val());
        $('.evsRemaining').text("Total EVs Remaining: 0");
    } else {
        var remainingEVs = Number(510) - Number($('.healthSlider').val()) - Number($('.attackSlider').val()) - Number($('.defenseSlider').val()) - Number($('.specialAttackSlider').val()) - Number($('.specialDefenseSlider').val()) - Number($('.speedSlider').val());
        $('.evsRemaining').text("Total EVs Remaining: " + remainingEVs);
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