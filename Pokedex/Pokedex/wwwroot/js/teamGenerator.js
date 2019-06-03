$('.generatorButton').on('click', function() {
    $.ajax({
        url: '/get-pokemon-team/',
        method: "POST"
    })
    .done(function(data) {
        $('.teamGeneratorTable tbody').remove();
        $('.teamGeneratorTable').append($('<tbody>'));

        for (var i = 0; i < 2; i++) {
            $('.Table3x2 tbody').append($('<tr>'));
        }

        for (var i = 0; i < 3; i++) {
            $('.Table2x3 tbody').append($('<tr>'));
        }

        for (var i = 0; i < 6; i++) {
            $('.Table1x6 tbody').append($('<tr>'));
        }

        for (var i = 0; i < 3; i++) {
            $('.Table3x2 tr:nth-child(1)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 1)
            }));
            $('.Table3x2 tr:nth-child(2)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 4)
            }));
        };

        for (var i = 0; i < 2; i++) {
            $('.Table2x3 tr:nth-child(1)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 1)
            }));
            $('.Table2x3 tr:nth-child(2)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 3)
            }));
            $('.Table2x3 tr:nth-child(3)').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 5)
            }));
        };

        for (var i = 0; i < 6; i++) {
            $('.Table1x6 tr:nth-child(' + (i + 1) + ')').append($('<td>', {
                class: "generatorPicture shadowed pokemon" + (i + 1)
            }));
        };

        for (var i = 0; i < 6; i++)
        {
            $('.pokemon' + (i + 1)).append('<a href="/' + data[i].name.replace(": ", "_").replace(' ', '_').toLowerCase() + '/" target="_blank"><img title="' + data[i].name.replace('_', ' ') + ' (Click to learn more)" src="/images/pokemon/' + data[i].id + '.png")" /></a>');
        }
    })
    .fail( function() {
        alert("Failed To Get Team!");
    });
});