var pokemonTeams, exportString, refreshClickEvents = function() {
    $('.pokemonTeamButton').on("click", function() {
        var buttonId = $(this).attr('id')
        if(typeof(pokemonTeams) !== "undefined")
        {
            $.each(pokemonTeams, function(index, item) {
                if(item.teamId == buttonId)
                {
                    exportString = item.exportString.replace(':', '\:').replace('(', '\(').replace(')', '\)');
                }
            });

            console.clear();

            var temp = $("<textarea>");
            $("body").append(temp);
            $(temp).text(exportString);
            $(temp).select();
            document.execCommand("copy");
            $(temp).remove();

            console.log(exportString);

            alert("Teams have been copied to your clipboard!");
        }
    });

    $('.pokemonTeamsButton').on("click", function() {
        if(typeof(pokemonTeams) !== "undefined")
        {
            exportString = "";
            $.each(pokemonTeams, function(index, item) {
                if(index != 0)
                {
                    exportString += "\n\n";
                }
                exportString += item.exportString.replace(':', '\:').replace('(', '\(').replace(')', '\)');
            });

            console.clear();

            var temp = $("<textarea>");
            $("body").append(temp);
            $(temp).text(exportString);
            $(temp).select();
            document.execCommand("copy");
            $(temp).remove();

            console.log(exportString);

            alert("Teams have been copied to your clipboard!");
        }
    });
};

$(document).ready(function() {
    $.ajax({
        url: '/grab-all-user-pokemon-teams/',
        method: 'POST'
    })
    .done(function(data) {
        pokemonTeams = data;
        $('.hide').each(function() {
            $(this).removeClass('hide');
        });
        refreshClickEvents();
    })
    .fail(function(jqXHR) {
        if(jqXHR.statusText != "error")
        {
            alert(jqXHR.statusText);
        }
    });
})