var primaryTypeID, secondaryTypeID, updateIDs = function () {
    primaryTypeID = $(".primaryList > select").val();
    secondaryTypeID = $(".secondaryList > select").val();
}, fillTables = function (data) {
    fillStrong(data);
    fillWeak(data);
    fillImmune(data);
}, fillStrong = function (data) {
    if (data.strongAgainst.length == 0) {
        $(".StrongAgainst").css("display", "none");
    }
    else {
        $.each(data.strongAgainst, function (input, type) {
            var rowTag = $("<tr>"), dataTag = $("<td>"), iconTag = $("<div>"), quadTag = $("<div>");
            $(quadTag).addClass("pokemon-type quad-icon");

            $(iconTag).addClass("pokemon-type type-icon type-" + type.toLowerCase());
            $(iconTag).text(type);

            if (~type.indexOf("Quad")) {
                $(iconTag).addClass("pokemon-type type-icon type-" + type.toLowerCase().substr(0, type.indexOf(' ')));
                $(iconTag).text(type.substr(0, type.indexOf(' ')));
                $(quadTag).addClass("quad-resist");
                $(quadTag).text("Quad");
            }

            $(dataTag).append(iconTag);
            if (~type.indexOf("Quad")) {
                $(dataTag).append(quadTag);
            }

            $(rowTag).append(dataTag);

            $(".typing-table-strong").append(rowTag);
        });

        $(".StrongAgainst").css("display", "block");
    }
}, fillWeak = function (data) {
    $.each(data.weakAgainst, function (input, type) {
        var rowTag = $("<tr>"), dataTag = $("<td>"), iconTag = $("<div>"), quadTag = $("<div>");
        $(quadTag).addClass("pokemon-type quad-icon");

        $(iconTag).addClass("pokemon-type type-icon type-" + type.toLowerCase());
        $(iconTag).text(type);

        if (~type.indexOf("Quad")) {
            $(iconTag).addClass("pokemon-type type-icon type-" + type.toLowerCase().substr(0, type.indexOf(' ')));
            $(iconTag).text(type.substr(0, type.indexOf(' ')));
            $(quadTag).addClass("quad-weak");
            $(quadTag).text("Quad");
        }

        $(dataTag).append(iconTag);
        if (~type.indexOf("Quad")) {
            $(dataTag).append(quadTag);
        }

        $(rowTag).append(dataTag);

        $(".typing-table-weak").append(rowTag);
    });
}, fillImmune = function (data) {
    if (data.immuneTo.length == 0) {
        $(".ImmuneTo").css("display", "none");
    }
    else {
        $.each(data.immuneTo, function (input, type) {
            var rowTag = $("<tr>"), dataTag = $("<td>"), iconTag = $("<div>"), quadTag = $("<div>");
            $(quadTag).addClass("pokemon-type quad-icon");

            $(iconTag).addClass("pokemon-type type-icon type-" + type.toLowerCase());
            $(iconTag).text(type);
            $(dataTag).append(iconTag);

            $(rowTag).append(dataTag);

            $(".typing-table-immune").append(rowTag);
        });

        $(".ImmuneTo").css("display", "block");
    }
}, checkTypings = function () {
    if (primaryTypeID != $(".primaryList > select").val() || secondaryTypeID != $(".secondaryList > select").val()) {
        updateIDs();
        $(".secondaryList option").each(function() {
            if(!$(this).is(":visible"))
            {
                $(this).css('display', 'block');
            }
        });

        $('.secondaryList option[value="' + primaryTypeID + '"]').css('display', 'none');

        if (primaryTypeID == "0" && secondaryTypeID != "0") {
            if ($(".secondaryList > select").val() == "100") {
                $(".primaryList > select").val("0");
            }
            else {
                $(".primaryList > select").val(secondaryTypeID);
            }

            $(".secondaryList > select").val("0");
            updateIDs();
        }
        else if (primaryTypeID == secondaryTypeID) {
            $(".secondaryList > select").val("0");
            updateIDs();
        }

        if (primaryTypeID != "0" && secondaryTypeID != "100") {
            $.ajax({
                url: '/get-typing-effectiveness/',
                method: 'POST',
                data: { 'primaryTypeID': primaryTypeID, 'secondaryTypeID': secondaryTypeID }
            })
                .done(function (data) {
                    typingList = data;

                    $(".typing-table").each(function (index) {
                        $(this).children().remove()
                    });

                    fillTables(data);

                    $(".effectivenessChart").css("display", "flex");
                })
                .fail(function () {
                    alert("Failed To Get Effectiveness Chart!");
                });
        }
        else {
            $(".effectivenessChart").css("display", "none");
        }
    }
}, grabPokemon = function () {
    if (primaryTypeID != "") {
        $(".pokemonWithTyping").css("display", "none");
        $('.effectivenessChart').css("display", "none");
        $(".pokemonList").empty();
        $('.pokemonList').load('/get-pokemon-by-typing/', { 'primaryTypeID': primaryTypeID, 'secondaryTypeID': secondaryTypeID }, function () {
            if ($('.pokemonList').children().length > 0) {
                $(".pokemonWithTyping").css("display", "block");
            }
            else {
                $(".pokemonWithTyping").css("display", "none");
            }

            if($(".secondaryList > select").val() == "")
            {
                $('.effectivenessChart').css("display", "flex");
            }
        });
    }
    else {
        $(".pokemonWithTyping").css("display", "none");
    }
}

$(function () {
    checkTypings();
    grabPokemon();
});

$(".typingSelectList").on('change', function () {
    checkTypings();
    grabPokemon();
});