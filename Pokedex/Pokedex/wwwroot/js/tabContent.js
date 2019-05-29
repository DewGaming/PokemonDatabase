function openPage(evt, pageName, parentName) {
  $('.tabcontent').each(function() {
    $(this).removeClass('active');
  })

  $('.tab').each(function() {
    $(this).removeClass('active');
  })

  $('#' + pageName).addClass('active');
  $(evt.currentTarget).addClass('active');
  if ('#' + parentName != null)
  {
    $('#' + parentName).addClass('active');
  }
}