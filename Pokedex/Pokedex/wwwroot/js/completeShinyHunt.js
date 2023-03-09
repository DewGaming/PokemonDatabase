$(document).ready(function () {
    var image = new Image();
    image.src = $('.webUrl').prop('name') + $('.shinyUrl').prop('name') + $('#PokemonId').val() + '.png';
    image.onload = function () {
        if (this.width > 0) {
            $('.pokemonShinyImage').prop('src', $('.webUrl').prop('name') + $('.shinyUrl').prop('name') + $('#PokemonId').val() + '.png');
        }
    }
    image.onerror = function () {
        $('.pokemonShinyImage').prop('src', $('.webUrl').prop('name') + $('.officialUrl').prop('name') + $('#PokemonId').val() + '.png');
    }
});