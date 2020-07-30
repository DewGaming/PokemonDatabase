function updateTypeChart(typeId, genId) {
    var resistances = [], weaknesses = [], immunities = [];
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

    $('.immunity input').each(function () {
        if ($(this).is(':checked')) {
            immunities.push(this.value);
        }
    });

    console.log(genId)

    $.ajax({
        url: '/update-type-chart/',
        method: "POST",
        data: { "typeId": typeId, "genId": genId, "resistances": resistances, "weaknesses": weaknesses, "immunities": immunities }
    })
        .done(function (data) {
            window.location = data;
        })
        .fail(function () {
            alert("Update Failed!");
        });
}