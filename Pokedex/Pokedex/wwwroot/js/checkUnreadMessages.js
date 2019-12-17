$(document).ready(function () {
    $.ajax({
        url: '/check-unread-messages/',
        method: 'POST'
    })
        .done(function (data) {
            if(data != 0)
            {
                $('.dropdown-item.messages').html($('.dropdown-item.messages').text() + ' <b>New!</b>');
                $('#userDropdown').html($('#userDropdown').text() + ' <span class="newMessageCount">' + data + '</span>')
            }
        })
        .fail(function (jqXHR) {
            if (jqXHR.statusText != "error") {
                alert(jqXHR.statusText);
            }
        });
})