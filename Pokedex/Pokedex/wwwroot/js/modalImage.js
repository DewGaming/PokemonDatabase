$('.siteFeatures img').on('click', function(){
  $('.modal').css('display', 'block');
  $('.modalImage').prop('src', $(this).attr('src').replace('/small', ''));
  $('.modalLink').attr('href', $(this).attr('data-modalUrl'));
  $('.modalCaption').text($(this).attr('alt'));
  $('.modalDescription').text($(this).attr('title'));
});

$('.close').on('click', function() {
  $('.modal').css('display', 'none');
  $('.modalImage').prop('src', '');
  $('.modalLink').attr('href', '');
  $('.modalCaption').text('');
  $('.modalDescription').text('');
});