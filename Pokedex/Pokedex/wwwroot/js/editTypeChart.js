function updateTypeChart(typeId) {
    var resistances = [], weaknesses = [];
    $('.resistance input').each(function () {
        if ($(this).is(':checked')) {
            resistances.push(this.value);
        }
    });

    $('.weakness input').each(function () {
        if ($(this).is(':checked')) {
            weaknesses.push(this.value);
        }
    });

    $.ajax({
        url: '/update-type-chart/',
        method: "POST",
        data: { "typeId": typeId, "resistances": resistances, "weaknesses": weaknesses }
    })
        .done(function (data) {
            window.location = data;
        })
        .fail(function () {
            alert("Update Failed!");
        });
}