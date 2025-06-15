//$("#settings-container").load("settings.html");
$.get("settings.html", function (data) {
    $("body").append(data);
    //showAccountSettings()
    $('#s-upload').on('submit', function (e) {
      e.preventDefault();

      const description = $('#s-u-desc').val();
      const fileInput = $('#s-u-file')[0];

      if (!token || currentChat==0 || !description || fileInput.files.length === 0) {
          alert("Fill form and put file.");
          return;
      }

      const formData = new FormData();
      formData.append("token", token);
      formData.append("chat_id", currentChat);
      formData.append("desc", description);
      formData.append("file", fileInput.files[0]);

      $.ajax({
          url: '/api/images/upload',
          method: 'POST',
          data: formData,
          processData: false, // обязательно для FormData
          contentType: false, // обязательно для FormData
          success: function (response) {
              //alert("Succes.");
          },
          error: function (xhr) {
              alertPanel('Server Error', xhr.responseText);
          }
      });
      $("#s-u-desc").val("");
      $("#s-u-file").val("");
      hideUploadSettings();
  });
});

$(document).ready(function () {
  $(document).on("submit", ".setting-form", function (e) {
    e.preventDefault();

    const $form = $(this);
    const formElement = $form[0];
    const formData = new FormData(formElement);

    formData.append("token", token);

    $.ajax({
      url: $form.attr("action"),
      method: "POST",
      data: formData,
      processData: false,
      contentType: false,
      success: function (response) {
        alertPanel("Settings Saved", "Reload page to apply changes.");
      },
      error: function (xhr) {
        alertPanel('Server Error', xhr.responseText);
      }
    });
  });
});

$(document).ready(function () {
  $(document).on("submit", ".setting-form-s", function (e) {
    e.preventDefault();

    const $form = $(this);
    const formElement = $form[0];
    const formData = new FormData(formElement);

    formData.append("token", token);
    formData.append("id", currentServer);

    $.ajax({
      url: $form.attr("action"),
      method: "POST",
      data: formData,
      processData: false,
      contentType: false,
      success: function (response) {
        alertPanel("Settings Saved", "Reload page to apply changes.");
      },
      error: function (xhr) {
        alertPanel('Server Error', xhr.responseText);
      }
    });
  });
});




function showServerSettings() {
  $("#server-settings")
    .css({ display: "flex", opacity: 0 })
    .animate({ opacity: 1 }, 300);
  $("#s-sv-name").html(servers[currentServer]);
  $("#s-sv-id").html(currentServer);
  document.querySelector('sl-tab-group#s-server').show('t-s-profile')
}

function hideServerSettings() {
  $("#server-settings")
    .animate({ opacity: 0 }, 300, function() {
      $(this).css("display", "none");
    });
}

function showAccountSettings() {
  $("#account-settings")
    .css({ display: "flex", opacity: 0 })
    .animate({ opacity: 1 }, 300);
  document.querySelector('sl-tab-group#s-account').show('t-profile')
}

function hideAccountSettings() {
  $("#account-settings")
    .animate({ opacity: 0 }, 300, function() {
      $(this).css("display", "none");
    });
}

function showJoinSettings() {
  $("#join-settings")
    .css({ display: "flex", opacity: 0 })
    .animate({ opacity: 1 }, 300);
}

function hideJoinSettings() {
  $("#join-settings")
    .animate({ opacity: 0 }, 300, function() {
      $(this).css("display", "none");
    });
}

function showUploadSettings() {
  $("#upload-settings")
    .css({ display: "flex", opacity: 0 })
    .animate({ opacity: 1 }, 300);
}

function hideUploadSettings() {
  $("#upload-settings")
    .animate({ opacity: 0 }, 300, function() {
      $(this).css("display", "none");
    });
}

