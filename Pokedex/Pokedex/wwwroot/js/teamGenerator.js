var pokemonList, fillGeneratedTable = function() {
    $('.teamGeneratorTable tbody').remove();
    $('.teamGeneratorTable').append($('<tbody>'));

    if($(window).width() >= 1000)
    {
        for (var i = 0; i < 2; i++) {
            $('.teamGenerator tbody').append($('<tr>'));
        }

        for (var i = 0; i < 3; i++) {
            $('.teamGenerator tr:nth-child(1)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 1)
            }));
            $('.teamGenerator tr:nth-child(2)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 4)
            }));
        }
    }
    else if($(window).width() >= 768)
    {
        for (var i = 0; i < 3; i++) {
            $('.teamGenerator tbody').append($('<tr>'));
        }

        for (var i = 0; i < 2; i++) {
            $('.teamGenerator tr:nth-child(1)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 1)
            }));
            $('.teamGenerator tr:nth-child(2)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 3)
            }));
            $('.teamGenerator tr:nth-child(3)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 5)
            }));
        }
    }
    else
    {
        for (var i = 0; i < 6; i++) {
            $('.teamGenerator tbody').append($('<tr>'));
        }

        for (var i = 0; i < 6; i++) {
            $('.teamGenerator tr:nth-child(' + (i + 1) + ')').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 1)
            }));
        }
    }

    for (var i = 0; i < 6; i++)
    {
        $('.pokemon' + (i + 1)).append('<a href="/' + originalNames[i].name.replace(": ", "_").replace(' ', '_').toLowerCase() + '/" target="_blank"><img title="' + pokemonList[i].name.replace('_', ' ') + ' (Click to learn more)" src="https://www.pokemondatabase.net/images/pokemon/' + pokemonList[i].id + '.png" /></a>');
    }
}, checkLegendaryChecks = function() {
    var boxChecked = false;
    $('.legendaryCheckbox input').each(function() {
        if ($(this).is(':checked'))
        {
            boxChecked = true;
            return false;
        }
    });

    if(!boxChecked && $('.legendaryBoolCheckbox').is(':visible'))
    {
        $(".legendaryBoolCheckbox").hide();
        $("#legendaryBool").prop('checked', false);
    }
    else
    {
        $(".legendaryBoolCheckbox").show();
    }
}, checkAltFormChecks = function() {
    var boxChecked = false;
    $('.alternateFormCheckbox input').each(function() {
        if ($(this).is(':checked'))
        {
            boxChecked = true;
            return false;
        }
    });

    if(!boxChecked && $('.altFormBoolCheckbox').is(':visible'))
    {
        $(".altFormBoolCheckbox").hide();
        $("#altFormBool").prop('checked', false);
    }
    else
    {
        $(".altFormBoolCheckbox").show();
    }
};

$(function() {
    checkAltFormChecks();
    checkLegendaryChecks();
});

$('.alternateFormCheckbox').on('click', function() {
    checkAltFormChecks();
});

$('.legendaryCheckbox').on('click', function() {
    checkLegendaryChecks();
});

$(window).on('resize', function() {
    if(
        ($('.teamGenerator tr').length == 2 && $(window).width() < 1000) ||
        ($('.teamGenerator tr').length == 3 && $(window).width() >= 1000) ||
        ($('.teamGenerator tr').length == 3 && $(window).width() < 751) ||
        ($('.teamGenerator tr').length == 6 && $(window).width() >= 751)
    )
    {
        fillGeneratedTable();
    }
});

$('.generatorButton').on('click', function() {
    var selectedGens = [], selectedLegendaries = [], selectedForms = [], selectedEvolutions;
    $('.generationCheckbox input').each(function() {
        if($(this).is(':checked')){
            selectedGens.push(this.value);
        }
    });

    $('.alternateFormCheckbox input').each(function() {
        if($(this).is(':checked')){
            selectedForms.push(this.value);
        }
    });

    $('.legendaryCheckbox input').each(function() {
        if($(this).is(':checked')){
            selectedLegendaries.push(this.value);
        }
    });

    $('.evolutionRadio input').each(function() {
        if($(this).is(':checked')){
            selectedEvolutions = this.value;
        }
    });

    $.ajax({
        url: '/get-pokemon-team/',
        method: 'POST',
        data: { 'selectedGens': selectedGens, 'selectedLegendaries': selectedLegendaries, 'selectedForms': selectedForms, 'selectedEvolutions': selectedEvolutions, 'onlyLegendaries': $("#legendaryBool").is(":checked"), 'onlyAltForms':  $("#altFormBool").is(":checked") }
    })
    .done(function(data) {
        pokemonList = data.allPokemonChangedNames;
        originalNames = data.allPokemonOriginalNames;
        fillGeneratedTable();
    })
    .fail( function() {
        alert("Failed To Get Team!");
    });
});