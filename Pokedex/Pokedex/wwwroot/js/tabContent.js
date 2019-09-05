function openPage(evt, pageName, parentName, iconLink, pokemonName) {
  $('.tabcontent').each(function() {
    $(this).removeClass('active');
  })

  $('.tab').each(function() {
    $(this).removeClass('active');
  })

  $('#' + pageName).addClass('active');
  $(evt.currentTarget).addClass('active');
  if ('#' + parentName != null)
  {
    $('#' + parentName).addClass('active');
  }
  if (iconLink != null)
  {
    $('.tabIcon').attr("href", iconLink);
    $('.pageTitle').text(pokemonName + " | Pokemon Database");
  }
}

function openGeneration(generationId) {
  if(!$('.active').is($('#Generation' + generationId)))
  {
    $('.tab').each(function() {
      $(this).removeClass('active');
    });

    $('.pokemonList').removeClass('active');
    $('.pokemonList > .grid-container').empty();
    $('.tab#Generation' + generationId).addClass('active');

    $('.grid-container').load('get-pokemon-by-generation/' + generationId, function (){
      $('.pokemonList').addClass('active');
    });
  }
}



      $('.pokemonList').addClass('active');
    });
  }
}