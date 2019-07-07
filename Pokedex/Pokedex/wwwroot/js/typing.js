var primaryTypeID, secondaryTypeID, updateIDs = function(){
    primaryTypeID = $(".primaryList > select").val();
    secondaryTypeID = $(".secondaryList > select").val();
}, checkTypings = function(){
    if(primaryTypeID != $(".primaryList > select").val() || secondaryTypeID != $(".secondaryList > select").val())
    {
        updateIDs();
        if(primaryTypeID == "" && secondaryTypeID != "")
        {
            $(".primaryList > select").val(secondaryTypeID);
            $(".secondaryList > select").val("");
            updateIDs();
        }
        else if(primaryTypeID == secondaryTypeID)
        {
            $(".secondaryList > select").val("");
            updateIDs();
        }

        if(primaryTypeID != "")
        {
            $.ajax({
                url: '/get-typing-effectiveness/',
                method: 'POST',
                data: { 'primaryTypeID': primaryTypeID, 'secondaryTypeID': secondaryTypeID }
            })
            .done(function(data) {
                typingList = data;

                $(".type-fx-cell").empty();
                $(".type-fx-0").each(function(index) {
                    $(this).removeClass("type-fx-0")
                });
                $(".type-fx-25").each(function(index) {
                    $(this).removeClass("type-fx-25")
                });
                $(".type-fx-50").each(function(index) {
                    $(this).removeClass("type-fx-50")
                });
                $(".type-fx-100").each(function(index) {
                    $(this).removeClass("type-fx-100")
                });
                $(".type-fx-200").each(function(index) {
                    $(this).removeClass("type-fx-200")
                });
                $(".type-fx-400").each(function(index) {
                    $(this).removeClass("type-fx-400")
                });

                $(data).each(function(input, typeChart)
                {
                    if(typeChart.effectiveness == 0)
                    {
                        $("#" + typeChart.typeId).text("0");
                        $("#" + typeChart.typeId).addClass("type-fx-0");
                    }
                    else if(typeChart.effectiveness == 0.25)
                    {
                        $("#" + typeChart.typeId).text("¼");
                        $("#" + typeChart.typeId).addClass("type-fx-25");
                    }
                    else if(typeChart.effectiveness == 0.5)
                    {
                        $("#" + typeChart.typeId).text("½");
                        $("#" + typeChart.typeId).addClass("type-fx-50");
                    }
                    else if(typeChart.effectiveness == 1)
                    {
                        $("#" + typeChart.typeId).text("1");
                        $("#" + typeChart.typeId).addClass("type-fx-100");
                    }
                    else if(typeChart.effectiveness == 2)
                    {
                        $("#" + typeChart.typeId).text("2");
                        $("#" + typeChart.typeId).addClass("type-fx-200");
                    }
                    else if(typeChart.effectiveness == 4)
                    {
                        $("#" + typeChart.typeId).text("4");
                        $("#" + typeChart.typeId).addClass("type-fx-400");
                    }
                });

                $(".toggleTypingTable").css("display", "block");
            })
            .fail( function() {
                alert("Failed To Get Effectiveness Chart!");
            });
        }
        else
        {
            $(".toggleTypingTable").css("display", "none");
        }
    }
}

$(function() {
    checkTypings();
});

$(".typingSelectList").on('change', function(){
    checkTypings();
});