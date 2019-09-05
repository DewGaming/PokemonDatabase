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

    $.ajax({
      url: '/get-pokemon-by-generation/',
      method: 'POST',
      data: { 'generationId': generationId }
    })
    .done(function(data) {
      pokemonList = data;
      for(var i = 0; i < data[0].length; i++)
      {
        var pokemonName = data[0][i].pokemon.name.replace('_', ' ');

        pokemonDiv = $('<div>');
        pokemonDiv.addClass(data[0][i].pokemon.name);

        pokemonLink = $('<a>');
        pokemonLink.attr('href', data[1][i])

        pokemonImage = $('<img>');
        pokemonImage.addClass('pokemonlistPicture')
                       .attr('title', pokemonName)
                       .attr('src', data[2][i]);

        pokemonLink.append(pokemonImage);
        pokemonDiv.append(pokemonLink);

        pokemonDescription = $('<div>');
        pokemonDescription.addClass('description');

        pokemonNameSpan = $('<span>');
        pokemonNameSpan.addClass('pokemonName')
                       .text('#' + data[0][i].pokemon.id.slice(-3) + ' ' + pokemonName);

        pokemonTyping = $('<div>');
        pokemonTyping.addClass('typing');

        pokemonPrimaryType = $('<div>');
        pokemonPrimaryType.addClass('pokemon-type type-icon type-' + data[0][i].primaryType.name.toLowerCase())
                          .text(data[0][i].primaryType.name)

        pokemonTyping.append(pokemonPrimaryType);

        if(data[0][i].secondaryType != null)
        {
          pokemonSeconaryType = $('<div>');
          pokemonSeconaryType.addClass('pokemon-type type-icon type-' + data[0][i].secondaryType.name.toLowerCase())
                            .text(data[0][i].secondaryType.name)

          pokemonTyping.append(pokemonSeconaryType);
        }

        pokemonDescription.append(pokemonNameSpan);
        pokemonDescription.append(pokemonTyping);

        pokemonDiv.append(pokemonDescription);

        $('.pokemonList > .grid-container').append(pokemonDiv);
      }
   
      $('.pokemonList').addClass('active');
    })
    .fail( function() {
      alert("Failed To Get Pokemon Table!");
    });
  }
}