$(document).ready(function () {
    $('.pokemonTeamButton').on("click", function () {
        $.ajax({
            url: '/grab-user-pokemon-team/',
            method: 'POST',
            async: false,
            data: { 'teamId': $(this).attr('id') }
        })
            .done(function (data) {
                var pokemonTeam = data;
                if (typeof (pokemonTeam) !== "undefined") {
                    var exportString = pokemonTeam[0].exportString.replace(':', '\:').replace('(', '\(').replace(')', '\)');

                    console.clear();

                    if (exportString.indexOf("No Ability") >= 0) { 
                        alert("It appears that one of your pokemon does not have an ability, even though it should. The pokemon without an ability will have a \"No Ability\" in it's section");
                    } 

                    var temp = $("<textarea>");
                    $("body").append(temp);
                    $(temp).text(exportString);
                    $(temp).select();
                    document.execCommand("copy");
                    $(temp).remove();

                    console.log(exportString);

                    alert("Team has been copied to your clipboard!");
                }
            })
            .fail(function (jqXHR) {
                if (jqXHR.statusText != "error") {
                    alert(jqXHR.statusText);
                }
            });
    });
})