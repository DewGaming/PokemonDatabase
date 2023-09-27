var pokemonTeams, exportString, deleteTeams = function () {
    var teamIds = [];
    $('.pokemonTeam input').each(function () {
        if ($(this).is(':checked')) {
            teamIds.push(this.value);
        }
    });
    $.ajax({
        xhr: function () {
            var xhr = new window.XMLHttpRequest();

            // Upload progress
            xhr.upload.addEventListener("progress", function (evt) {
                if (evt.lengthComputable) {
                    var percentComplete = evt.loaded / evt.total;
                    //Do something with upload progress
                    console.log(percentComplete);
                }
            }, false);

            // Download progress
            xhr.addEventListener("progress", function (evt) {
                if (evt.lengthComputable) {
                    var percentComplete = evt.loaded / evt.total;
                    // Do something with download progress
                    console.log(percentComplete);
                }
            }, false);

            return xhr;
        },
        url: '/delete-pokemon-teams/',
        method: 'POST',
        async: false,
        data: { 'teamIds': teamIds }
    })
        .done(function (data) {
            window.location = data;
        })
        .fail(function () {
            alert("Update Failed!");
        });
}, lookupTeamsInGame = function(gameId) {
    if (!$('.active').is($('#Game' + gameId))) {
        $('button').each(function () {
            $(this).removeClass('active');
        });

        $('.pokemonTeams').removeClass('active');

        $('.pokemonTeam.hide').each(function () {
            $(this).removeClass('hide');
        });

        if (gameId != 0) {
            $('div.pokemonTeam').not('.TeamGame' + gameId).each(function () {
                $(this).addClass('hide');
            });
            $('.gameTeamIn').each(function () {
                $(this).addClass('hide');
            });
        } else {
            $('.gameTeamIn').each(function () {
                $(this).removeClass('hide');
            });
        }

        $('button#Game' + gameId).addClass('active');
        $('.pokemonTeams').addClass('active');
        $('.active.hide').removeClass('active');
    }
}

$(document).ready(function () {
    $('.pokemonTeamButton').on("click", function () {
        $.ajax({
            url: '/grab-user-pokemon-team/',
            method: 'POST',
            async: false,
            data: { 'teamId': $(this).attr('id') }
        })
            .done(function (data) {
                if (typeof (data) !== "undefined") {
                    if (navigator.clipboard) {
                        navigator.clipboard.writeText(data.exportString)
                            .then(() => {
                                alert("Team has been copied to your clipboard!");
                            })
                            .catch((error) => {
                                alert("Team was unable to be copied to your clipboard!");
                            });

                        console.log(data.exportString);
                    } else {
                        alert("Team was unable to be copied to your clipboard!");
                    }
                }
            })
            .fail(function (jqXHR) {
                if (jqXHR.statusText != "error") {
                    alert(jqXHR.statusText);
                }
            });
    });

    $.ajax({
        url: '/grab-all-user-pokemon-teams/',
        method: 'POST'
    })
        .done(function (data) {
            pokemonTeams = data;
            if (pokemonTeams.length != 0) {
                $('.pokemonTeamsButton').removeClass('hide');
                $('.pokemonTeamButton').unbind("click");

                $('.pokemonTeamButton').on("click", function () {
                    var buttonId = $(this).attr('id')
                    if (typeof (pokemonTeams) !== "undefined") {
                        exportString = pokemonTeams.find(x => x.teamId == buttonId).exportString;
    
                        if (navigator.clipboard) {
                            navigator.clipboard.writeText(exportString)
                                .then(() => {
                                    alert("Team has been copied to your clipboard!");
                                })
                                .catch((error) => {
                                    alert("Team was unable to be copied to your clipboard!");
                                });
    
                            console.log(exportString);
                        } else {
                            alert("Team was unable to be copied to your clipboard!");
                        }
                    }
                });

                $('.pokemonTeamsButton').on("click", function () {
                    if (typeof (pokemonTeams) !== "undefined") {
                        exportString = pokemonTeams.map(x => x.exportString).join("\n\n");
    
                        if (navigator.clipboard) {
                            navigator.clipboard.writeText(exportString)
                                .then(() => {
                                    alert("Team has been copied to your clipboard!");
                                })
                                .catch((error) => {
                                    alert("Team was unable to be copied to your clipboard!");
                                });
    
                            console.log(exportString);
                        } else {
                            alert("Team was unable to be copied to your clipboard!");
                        }
                    }
                });
            }
        })
        .fail(function (jqXHR) {
            if (jqXHR.statusText != "error") {
                alert(jqXHR.statusText);
            }
        });
})