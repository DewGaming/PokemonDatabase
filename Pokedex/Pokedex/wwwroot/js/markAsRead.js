$(document).ready(function () {
    $.ajax({
        url: '/mark-as-read/',
        method: 'POST'
    })
        .done(function (data) {
            
        })
        .fail(function (jqXHR) {
            if (jqXHR.statusText != "error") {
                alert(jqXHR.statusText);
            }
        });
})