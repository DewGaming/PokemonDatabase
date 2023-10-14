function updateRegionalDexEntries(regionalDexId) {
    var pokemon = [];
    $('.regionalDexEntryList span').each(function () {
        pokemon.push(this.id);
    });

    $(".overlay").fadeIn(300);

    $.ajax({
        url: '/update-regional-dex-entries/',
        method: "POST",
        data: { "regionalDexId": regionalDexId, "pokemonList": pokemon }
    })
        .done(function (data) {
            window.location = data;
        })
        .fail(function () { 
            setTimeout(function () {
                $(".overlay").fadeOut(300);
            });

            alert("Update Failed!");
        });
}

$(document).ready(function () {
    $('.regionalDexEntryList').sortable();
});

$('.regionalDexEntryOption').on('change', function () {
    if ($(this).prop('checked')) {
        var li = $('<li>');
        var span = $('<span>').prop('id', $(this).prop('id')).html($(this).prop('id'));
        li.append(span);
        $('.regionalDexEntryList').append(li);
    } else {
        $('.regionalDexEntryList #' + $(this).prop('id')).parent().remove();
    }
});