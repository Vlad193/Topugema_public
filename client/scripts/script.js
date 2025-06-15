$('body').append("<div id=alert-container></div>");
function alertPanel(type,message) {
    console.log("PN: " + type + "\n ER:" + message);
    const newPanel = $(`<div class="alert-panel">
        <h2>${type}</h2>
        <p>${message}</p>
        </div>`).appendTo('#alert-container');

    setTimeout(function() {
    $(newPanel).fadeOut(1000, function() {
        $(this).remove();
    });
    }, 3000);
}