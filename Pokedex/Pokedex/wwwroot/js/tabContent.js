function openPage(evt, pageName, username) {
  $('.tabcontent').each(function() {
    $(this).removeClass('active');
  })

  $('.tab').each(function() {
    $(this).removeClass('active');
  })

  $('#' + pageName).addClass('active');
  $(evt.currentTarget).addClass('active');
  if ('#' + username != null)
  {
    $('#' + username).addClass('active');
  }
}