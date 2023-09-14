var officialRender = function() {
    $('.current').removeClass('current');
    $('.official').addClass('current');
    $('.pokemonImageButtons>.hidden').removeClass('hidden');
    $('.pokemonImageButtons .officialButton').addClass('hidden');
}, homeRender = function() {
    $('.current').removeClass('current');
    $('.home').addClass('current');
    $('.pokemonImageButtons>.hidden').removeClass('hidden');
    $('.pokemonImageButtons .homeButton').addClass('hidden');
}, shinyRender = function() {
    $('.current').removeClass('current');
    $('.shiny').addClass('current');
    $('.pokemonImageButtons>.hidden').removeClass('hidden');
    $('.pokemonImageButtons .shinyButton').addClass('hidden');
}, grabTypingChart = function (primaryType, generationId) {
    $('.page.active .effectivenessChart').load('/get-typing-evaluator-chart/', { 'primaryTypeID': primaryType, 'secondaryTypeID': '0', 'generationID': generationId }, function () {
        if ($('.typing-table-strong').children().length > 0) {
            $(".StrongAgainst").css("display", "block");
        }
        else {
            $(".StrongAgainst").css("display", "none");
        }

        if ($('.typing-table-weak').children().length > 0) {
            $(".WeakAgainst").css("display", "block");
        }
        else {
            $(".WeakAgainst").css("display", "none");
        }

        if ($('.typing-table-immune').children().length > 0) {
            $(".ImmuneTo").css("display", "block");
        }
        else {
            $(".ImmuneTo").css("display", "none");
        }
    });
}, grabTypingChartByPokemon = function (pokemonId, generationId) {
    $('.page.active .effectivenessChart').load('/get-typing-evaluator-chart-by-pokemon/', { 'pokemonId': pokemonId, 'generationID': generationId }, function () {
        if ($('.typing-table-strong').children().length > 0) {
            $(".StrongAgainst").css("display", "block");
        }
        else {
            $(".StrongAgainst").css("display", "none");
        }

        if ($('.typing-table-weak').children().length > 0) {
            $(".WeakAgainst").css("display", "block");
        }
        else {
            $(".WeakAgainst").css("display", "none");
        }

        if ($('.typing-table-immune').children().length > 0) {
            $(".ImmuneTo").css("display", "block");
        }
        else {
            $(".ImmuneTo").css("display", "none");
        }
    });
}

$('span[title]').on('click', function() {
    alert($(this).attr('title'))
})

$('.teraTypeSelectList').on('change', function () {
    var generationId = $('#' + $('.page.active').attr('id') + '.page.active').attr('class').split(' ')[1].split('generation')[1];
    var teraType = $(this).val();
    var currentImage = $('.page.active .pokemonImage .current').attr("class").split(' ')[0];
    var pokemonId = $('.page.active .pokemonImage .current img').attr("id");
    if (teraType != 0) {
        grabTypingChart(teraType, generationId);
    } else {
        grabTypingChartByPokemon(pokemonId, generationId);
    }

    if (pokemonId == '741' || pokemonId == '1301') {
        $('.pokemonPicture .pokemonImage').load('/get-pokemon-images-with-type/', { 'pokemonId': pokemonId, 'typeId': teraType, 'currentImage': currentImage });
    }
});