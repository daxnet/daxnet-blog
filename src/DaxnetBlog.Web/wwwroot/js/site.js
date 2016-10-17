// Write your Javascript code.
$(document).ready(function () {
    $("[data-hide]").on("click", function () {
        $("." + $(this).attr("data-hide")).hide();
    });
});


// Displays the message box on the page.
// type:  The type of the message box, can be info, warning, danger
// title: The title of the message to be displayed
// body:  The body of the message
function showMessage(type, title, body) {
    $('#message-alert').removeClass(function (index, css) {
        return (css.match(/(^|\s)alert-\S+/g) || []).join(' ');
    });

    $('#message-alert').addClass('alert-' + type);
    
    $('#message-alert-title').text(title + '：');
    $('#message-alert-body').text(body);

    $('#message-alert').show();
}

function hideMessage() {
    $('#message-alert').hide();
}
