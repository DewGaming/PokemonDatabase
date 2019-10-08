var pokemonTeams, exportString;
$(document).ready(function() {
    $.ajax({
        url: '/grab-all-user-pokemon-teams/',
        method: 'POST'
    })
    .done(function(data) {
        pokemonTeams = data;
    })
    .fail(function(jqXHR) {
        alert(jqXHR.statusText);
    });
})

$('.pokemonTeamButton').on("click", function() {
    var buttonId = $(this).attr('id')
    $.each(pokemonTeams, function(index, item) {
        if(item.teamId == buttonId)
        {
            exportString = item.exportString.replace(':', '\:').replace('(', '\(').replace(')', '\)');
        }
    });

    var temp = $("<textarea>");
    $("body").append(temp);
    $(temp).text(exportString);
    $(temp).select();
    document.execCommand("copy");
    $(temp).remove();

    alert("Teams have been copied to your clipboard!");
});

$('.pokemonTeamsButton').on("click", function() {
    exportString = "";
    $.each(pokemonTeams, function(index, item) {
        if(index != 0)
        {
            exportString += "\n\n";
        }
        exportString += item.exportString.replace(':', '\:').replace('(', '\(').replace(')', '\)');
    });

    var temp = $("<textarea>");
    $("body").append(temp);
    $(temp).text(exportString);
    $(temp).select();
    document.execCommand("copy");
    $(temp).remove();

    alert("Teams have been copied to your clipboard!");
});