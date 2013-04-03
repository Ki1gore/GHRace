
function UrlInputClick() {
    var data = JSON.stringify({ Url: $("#urls").val() });
    $.ajax({
        url: '/Home/LoadUrls',
        type: 'POST',
        dataType: 'json',
        data: data,
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data != null) {
                //alert(data[0].Greyhounds[0].Races[0].Comments);
            }
        }
    });
}

$(document).ready(function () {
    $("#url-input").click(UrlInputClick);
});