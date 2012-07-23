jQuery(document).ready(function ($) {

  /* index page */
  $('#banners').orbit({ fluid: '833x100' });

  $('div.slideshow').orbit();
  $(this).tooltips();

  $('div.inloggen').on('click', 'a.inactive', function (event) {
    event.preventDefault();
    event.stopPropagation();

    $(this).addClass("active").removeClass("inactive");

    $('div.inloggen').css("width", "230px");
    $('div.inloggen').animate({
      "margin-top": -20
    }, 500, function() {
      $('div.inloggen input').first().focus();
    });
  });

  $('div.inloggen').on('click', 'a.active', function (event) {
    event.preventDefault();
    event.stopPropagation();

    $(this).addClass("inactive").removeClass("active");

    $('div.inloggen').animate({
      "margin-top": -500
    }, 500, function () {
      $('div.inloggen').css("width", "100px");
    });
  });

  $('div.gebruiker').on('click', 'a.inactive', function (event) {
    event.preventDefault();
    event.stopPropagation();

    $(this).addClass("active").removeClass("inactive");

    $('div.gebruiker').css("width", "270px");
    $('div.gebruiker').animate({
      "margin-top": -20
    }, 500, function () {
      
    });
  });

  $('div.gebruiker').on('click', 'a.active', function (event) {
    event.preventDefault();
    event.stopPropagation();

    $(this).addClass("inactive").removeClass("active");

    $('div.gebruiker').animate({
      "margin-top": -400
    }, 500, function () {
      $('div.gebruiker').css("width", "170px");
    });
  });

  /* photo albums */

  $('div.photo').click(function () {

    revertAlbums();

    var $this = $(this);
    var id = $this.attr("id");
    var name = $this.attr("name");

    $this.css("background-image", "");
    $this.parent().parent().removeClass("three").addClass("six");

    $.get("/fotos/" + name + "/" + id, function (data) {
      for (var i = 0; i < data.length; i++) {
        $this.append("<img src='" + data[i].Url + "' />");
      }
      $this.orbit();
    });

  });

  function revertAlbums() {
    $('div.photo.orbit').empty().each(function (i, e) {
      $(e).css("background-image", "url('" + $(e).data("thumbnail") + "')");
      $(e).prependTo($(e).parent().parent());
      $(e).parent().parent().removeClass("six").addClass("three");
      $(e).css("width", "").css("height", "").removeClass("orbit");
      $('div.orbit-wrapper').remove();
    });
  }

  /* calendar */

  $('div.twelve.agenda p.titel').click(function () {
    $("div.agendacontainer div.six.active").removeClass("active").addClass("inactive")
    $(this).parent().parent().removeClass("inactive").addClass("active");
    $(this).parent().parent().prependTo($(this).parent().parent().parent());

    var inactive = $("div.agendacontainer div.six.inactive").toArray();
    inactive.sort(function (ae, be) {
      var a = $(ae).data("sortkey");
      var b = $(be).data("sortkey");
      return (a < b ? -1 : (a > b ? 1 : 0));
    });
    for (var i = 0; i < inactive.length; i++) {
      $(inactive[i]).appendTo($("div.agendacontainer"));
    }
  });

  /* audio */
  var canPlayMp3 = $('#player').get(0).canPlayType('audio/mpeg') != '';

  if (canPlayMp3) {
    var player = $('#player').get(0);

    var onLoadedData = function () { 
      player.addEventListener('timeupdate', onTimeUpdate, false);
    };

    var onTimeUpdate = function () {

      var currSec = parseInt(player.currentTime % 60);
      var currMin = parseInt((player.currentTime / 60) % 60);
      var totalSec = parseInt(player.duration % 60);
      var totalMin = parseInt((player.duration / 60) % 60);

      // "onEnded" should be triggered from an onended event, but it's not always reliable.playPause
      if (player.ended) {
        onEnded();
      } else if (player.seeking) {
      } else {
        var status = [currMin >= 10 ? currMin : '0' + currMin, ':',
                 currSec >= 10 ? currSec : '0' + currSec,
                 ' / ',
                 totalMin >= 10 ? totalMin : '0' + totalMin, ':',
                 totalSec >= 10 ? totalSec : '0' + totalSec, ].join('');
        $('div.progress div.info').text(status);
        $('div.progress div.bar').css('width', (player.currentTime / player.duration) * 100 + '%');
      }
    };

    var onEnded_ = function () {
      player.removeEventListener("timeupdate", onTimeUpdate, false);
    };

    player.addEventListener("loadeddata", onLoadedData, false);

    $("div.recording a").click(function (event) {
      event.preventDefault();
      event.stopPropagation();

      $this = $(this);

      $('.audioplayer')
        .appendTo($this.parent().parent().parent().parent())
        .show()
        .find("a").attr("href", $this.attr("href")).end()
      ;

      $('div.recordingpointer').hide();
      $('div.recording.active').removeClass("active");
      $this.parent().parent().parent().addClass("active");
      $this.parent().parent().parent().parent().find(".recordingpointer").show();

      reset();
    });

    var reset = function () {
      $('div.pause').removeClass("pause").addClass("play");
      $('div.audioplayer div.stop').hide();
      player.src = $('div.recording.active a').attr("href");
      player.removeEventListener('timeupdate', onTimeUpdate, false);
      player.load();
      $('div.progress div.info').text('');
      $('div.progress div.bar').css("width", "0%");
      // http://studio.html5rocks.com/samples/audio-podcast/podcast.js
    };

    $('div.audioplayer').on('click', 'div.play', function () {
      $(this).removeClass("play").addClass("pause");
      $('div.audioplayer div.stop').show();
      player.play();
    });
    $('div.audioplayer').on('click', 'div.pause', function () {
      $(this).removeClass("pause").addClass("play");
      $('div.audioplayer div.stop').show();
      player.pause();
    });
    $('div.audioplayer').on('click', 'div.stop', function () {
      reset();
    });
  }

  $('span.check').click(function () {
    var $this = $(this);
    var categorie = $this.data("cat");
    if ($this.hasClass("checked")) {
      $this.removeClass("checked");
      $('div[data-cat="' + categorie + '"]').hide();
    } else {
      $this.addClass("checked");
      $('div[data-cat="' + categorie + '"]').show();
    }

    $('div.recordingheader').show();
    $('div.recordingheader').each(function (index, elem) {
      var $this = $(elem);
      if ($this.next("div.recordinggroup").find("div.four:visible").length == 0) {
        $this.hide();
      }
    });
  });

  /* ALERT BOXES ------------ */
  //$(".alert-box").delegate("a.close", "click", function(event) {
  //  event.preventDefault();
  //  $(this).closest(".alert-box").fadeOut(function(event){
  //    $(this).remove();
  //  });
  //});

  /* PLACEHOLDER FOR FORMS ------------- */
  /* Remove this and jquery.placeholder.min.js if you don't need :) */
  //$('input, textarea').placeholder();

  /* TOOLTIPS ------------ */


  /* UNCOMMENT THE LINE YOU WANT BELOW IF YOU WANT IE6/7/8 SUPPORT AND ARE USING .block-grids */
  //  $('.block-grid.two-up>li:nth-child(2n+1)').css({clear: 'left'});
  //  $('.block-grid.three-up>li:nth-child(3n+1)').css({clear: 'left'});
  //  $('.block-grid.four-up>li:nth-child(4n+1)').css({clear: 'left'});
  //  $('.block-grid.five-up>li:nth-child(5n+1)').css({clear: 'left'});

  /* DISABLED BUTTONS ------------- */
  /* Gives elements with a class of 'disabled' a return: false; */

  /* SPLIT BUTTONS/DROPDOWNS */
  //$('.button.dropdown > ul').addClass('no-hover');

  //$('.button.dropdown').on('click.fndtn touchstart.fndtn', function (e) {
  //  e.stopPropagation();
  //});
  //$('.button.dropdown.split span').on('click.fndtn touchstart.fndtn', function (e) {
  //  e.preventDefault();
  //  $('.button.dropdown').not($(this).parent()).children('ul').removeClass('show-dropdown');
  //  $(this).siblings('ul').toggleClass('show-dropdown');
  //});
  //$('.button.dropdown').not('.split').on('click.fndtn touchstart.fndtn', function (e) {
  //  e.preventDefault();
  //  $('.button.dropdown').not(this).children('ul').removeClass('show-dropdown');
  //  $(this).children('ul').toggleClass('show-dropdown');
  //});
  //$('body, html').on('click.fndtn touchstart.fndtn', function () {
  //  $('.button.dropdown ul').removeClass('show-dropdown');
  //});

  //// Positioning the Flyout List
  //var normalButtonHeight  = $('.button.dropdown:not(.large):not(.small):not(.tiny)').outerHeight() - 1,
  //    largeButtonHeight   = $('.button.large.dropdown').outerHeight() - 1,
  //    smallButtonHeight   = $('.button.small.dropdown').outerHeight() - 1,
  //    tinyButtonHeight    = $('.button.tiny.dropdown').outerHeight() - 1;

  //$('.button.dropdown:not(.large):not(.small):not(.tiny) > ul').css('top', normalButtonHeight);
  //$('.button.dropdown.large > ul').css('top', largeButtonHeight);
  //$('.button.dropdown.small > ul').css('top', smallButtonHeight);
  //$('.button.dropdown.tiny > ul').css('top', tinyButtonHeight);

});