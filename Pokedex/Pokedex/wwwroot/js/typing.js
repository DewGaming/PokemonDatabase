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
                
                $(".type-icon").each(function(index) {
                    $(this).remove()
                });

                $(data).each(function(input, typeChart)
                {
                    var appendTag = $("<td>");
                    appendTag.addClass("type-icon type-" + typeChart.typeName.toLowerCase() + " type-cell");
                    appendTag.text(typeChart.typeName);

                    if(typeChart.effectiveness == 0)
                    {
                        $(".typing-table-immune").parent().parent().children('td').children('table').children('tbody').children('tr').append(appendTag);
                    }
                    else if(typeChart.effectiveness == 0.25)
                    {
                        $(".typing-table-very-weak").parent().parent().children('td').children('table').children('tbody').children('tr').append(appendTag);
                    }
                    else if(typeChart.effectiveness == 0.5)
                    {
                        $(".typing-table-weak").parent().parent().children('td').children('table').children('tbody').children('tr').append(appendTag);
                    }
                    else if(typeChart.effectiveness == 1)
                    {
                        $(".typing-table-neutral").parent().parent().children('td').children('table').children('tbody').children('tr').append(appendTag);
                    }
                    else if(typeChart.effectiveness == 2)
                    {
                        $(".typing-table-strong").parent().parent().children('td').children('table').children('tbody').children('tr').append(appendTag);
                    }
                    else if(typeChart.effectiveness == 4)
                    {
                        $(".typing-table-very-strong").parent().parent().children('td').children('table').children('tbody').children('tr').append(appendTag);
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