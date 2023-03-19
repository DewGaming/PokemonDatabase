$(document).ready(function () {
    $(".overlay").fadeIn(300);

    var image = new Image();
    image.src = $('.webUrl').prop('name') + $('.shinyUrl').prop('name') + $('#Pokemon_Id').val() + '.png';
    image.onload = function () {
        if (this.width > 0) {
            $('.pokemonShinyImage').prop('src', $('.webUrl').prop('name') + $('.shinyUrl').prop('name') + $('#Pokemon_Id').val() + '.png');
        }
    }
    image.onerror = function () {
        $('.pokemonShinyImage').prop('src', $('.webUrl').prop('name') + $('.officialUrl').prop('name') + $('#Pokemon_Id').val() + '.png');
    }
    
    $(".overlay").fadeOut(300);
});