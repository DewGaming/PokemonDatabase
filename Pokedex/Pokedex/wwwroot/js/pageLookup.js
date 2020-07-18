var currentGeneration = 0

function lookupPage(pageName) {
  $('.active').each(function () {
    $(this).removeClass('active');
  })

  $('.' + pageName).addClass('active');
  $('#' + pageName).addClass('active');
}

function lookupPokemon(pageName, iconLink, pokemonName) {
  $('.active').each(function () {
    $(this).removeClass('active');
  })

  pokemonName = pokemonName.replace("&apos;", "\'");

  $('.generations#' + pageName.replace('%', '\\%')).addClass('active');
  $('.pokemon#' + pageName.replace('%', '\\%')).addClass('active');

  $('.tabIcon').attr("href", iconLink);
  $('.pageTitle').text(pokemonName + " | Pokémon Database");
}

function lookupIncompletePokemon() {
  $('button').each(function () {
    $(this).removeClass('active');
  });

  $('.pokemonList').removeClass('active');
  $('.pokemonList > .grid-container').empty();

  $('.grid-container').load('/get-incomplete-pokemon-admin/', function () {
    $.each($('.grid-container .pokemonName'), function (index, item) {
      if ($(item).text().includes('_')) {
        $(item).text($(item).text().replace('_', ' '));
      }
    });

    $('.pokemonList').addClass('active');
  });

  $('.pageButtons').hide();
  $('.incompletePokemon').hide();
  $('.allPokemon').show();
};

function lookupAllPokemon() {
  $('.pokemonList > .grid-container').empty();
  $('.pokemonList').removeClass('active');

  $('.pageButtons').show();
  $('.incompletePokemon').show();
  $('.allPokemon').hide();
};

function lookupGeneration(generationId) {
  if (currentGeneration != generationId) {
    currentGeneration = generationId;
  }

  $('button').each(function () {
    $(this).removeClass('active');
  });

  $('.pokemonList').removeClass('active');
  $('.pokemonList > .grid-container').empty();
  $('button#Generation' + generationId).addClass('active');

  $('.grid-container').load('/get-pokemon-by-generation/' + generationId, function () {
    $.each($('.grid-container .pokemonName'), function (index, item) {
      if ($(item).text().includes('_')) {
        $(item).text($(item).text().replace('_', ' '));
      }
    });

    $('.pokemonList').addClass('active');
  });
}

function lookupAvailableGame(gameId) {
  if (!$('.active').is($('#Game' + gameId))) {
    $('button').each(function () {
      $(this).removeClass('active');
    });

    $('.pokemonCount').remove();
    $('.updateButton').remove();

    $('.pokemonList').removeClass('active');
    $('.pokemonList > .grid-container').empty();
    $('button#Game' + gameId).addClass('active');

    $('.grid-container').load('/get-available-pokemon-by-game/' + gameId, function () {
      $('.totalPokemon').prepend($('<h5>').addClass('pokemonCount').text('Total Available Pokémon: ' + $('.grid-container').children().length));
      $('.updater').append($('<a>').attr('href','admin/edit_game_availability/' + gameId).addClass('updateButton btn btn-primary').text('Update Game Availability'));

      $('.pokemonList').addClass('active');
    });
  }
}

function lookupAdminGeneration(generationId) {
  if (!$('.active').is($('#Generation' + generationId))) {
    $('button').each(function () {
      $(this).removeClass('active');
    });

    $('.pokemonList').removeClass('active');
    $('.pokemonList > .grid-container').empty();
    $('button#Generation' + generationId).addClass('active');

    $('.grid-container').load('/get-pokemon-by-generation-admin/' + generationId, function () {
      $.each($('.grid-container .pokemonName'), function (index, item) {
        if ($(item).text().includes('_')) {
          $(item).text($(item).text().replace('_', ' '));
        }
      });

      $('.pokemonList').addClass('active');
    });
  }
}

function lookupMoveTypes(typeId) {
  if (!$('.active').is($('#Type' + typeId))) {
    $('button').each(function () {
      $(this).removeClass('active');
    });

    $('.page').each(function () {
      $(this).removeClass('active');
    });

    $('button#Type' + typeId).addClass('active');

    $('.type' + typeId).addClass('active');
  }
}