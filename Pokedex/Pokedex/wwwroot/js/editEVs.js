var setupLabels = function(health, attack, defense, specialAttack, specialDefense, speed) {
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

$('.healthSlider').on('input', function() {
    $('#healthVal').text($(this).val());
});

$('.attackSlider').on('input', function() {
    $('#attackVal').text($(this).val());
});

$('.defenseSlider').on('input', function() {
    $('#defenseVal').text($(this).val());
});

$('.specialAttackSlider').on('input', function() {
    $('#specialAttackVal').text($(this).val());
});

$('.specialDefenseSlider').on('input', function() {
    $('#specialDefenseVal').text($(this).val());
});

$('.speedSlider').on('input', function() {
    $('#speedVal').text($(this).val());
});