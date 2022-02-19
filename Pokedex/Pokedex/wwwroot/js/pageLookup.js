var currentGeneration = 0

function lookupPage(pageName) {
  $('.active').each(function () {
    $(this).removeClass('active');
  })

  $('.' + pageName).addClass('active');
  $('#' + pageName).addClass('active');
}

function lookupTypeChart(generation) {
  $('.active').each(function () {
    $(this).removeClass('active');
  })

  $('button#Generation' + generation + ', .grid-row#Generation' + generation).addClass('active');
}

function lookupPokemon(pageName, iconLink, pokemonName, generation) {
  $('.active').each(function () {
    $(this).removeClass('active');
  })

  pokemonName = pokemonName.replace("&apos;", "\'");

  while ($('.generation' + generation + '#' + pageName).length == 0)
  {
    generation -= 1
    if (generation == 0)
    {
      generation = Number($('#LatestGenerationId').val())
    }
  }

  $('button#Generation' + generation + ', .generations#' + pageName.replace('%', '\\%') + ', .generation' + generation + '#' + pageName.replace('%', '\\%')).addClass('active');

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
      $('.updater').append($('<a>').attr('href', 'admin/edit_game_availability/' + gameId).addClass('updateButton btn btn-primary').text('Update Game Availability'));

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

function lookupGames(gameId) {
  if (!$('.active').is($('#Game' + gameId))) {
    $('button').each(function () {
      $(this).removeClass('active');
    });

    $('.page').each(function () {
      $(this).removeClass('active');
    });

    $('button#Game' + gameId).addClass('active');

    $('.game' + gameId).addClass('active');
  }
}

function lookupYear(year) {
  $('.year button').each(function () {
    $(this).removeClass('active');
  });

  $('.year.pageStatList').removeClass('active');
  $('.year.pageStatList').empty();
  $('button#Year' + year).addClass('active');

  $('.year.pageStatList').load('/get-month-by-year/' + year, function () {
    $('.year.pageStatList').addClass('active');
  });
}

function lookupMonth(month) {
  $('.month button').each(function () {
    $(this).removeClass('active');
  });

  $('.month.pageStatList').removeClass('active');
  $('.month.pageStatList').empty();
  $('button#Month' + month).addClass('active');

  $('.month.pageStatList').load('/get-day-by-month/' + month + '/' + $('.pageButtons.year .active').attr('id').replace('Year', ''), function () {
    $('.month.pageStatList').addClass('active');
  });
}

function lookupDay(day) {
  $('.day button').each(function () {
    $(this).removeClass('active');
  });

  $('.day.pageStatList').removeClass('active');
  $('.day.pageStatList').empty();
  $('button#Day' + day).addClass('active');

  $('.day.pageStatList').load('/get-stats-by-date/' + day + '/' + $('.pageButtons.month .active').attr('id').replace('Month', '') + '/' + $('.pageButtons.year .active').attr('id').replace('Year', ''), function () {
    $('.day.pageStatList').addClass('active');
  });
}