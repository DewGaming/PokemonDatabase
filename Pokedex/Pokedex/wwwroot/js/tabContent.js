function openPage(evt, pageName, parentName, iconLink) {
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
  if (iconLink != null)
  {
    $('.tabIcon').attr("href", iconLink);
  }
}