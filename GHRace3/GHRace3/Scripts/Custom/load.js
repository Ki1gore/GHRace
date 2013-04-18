
//display the skipped / added url's in the format {Date} {link to url}
function DisplayData(data) {
    var container = $("#load-status");
    var headerSKipped = document.createElement("h2");
    $(headerSKipped).html("Skipped:");
    container.append(headerSKipped);
    for (var i in data.Skipped) {
        var pSkipped = document.createElement("p");
        $(pSkipped).html(data.Skipped[i].Date + "    " + '<a href="' + data.Skipped[i].url + '">' + data.Skipped[i].TrackName + '</a>');
        container.append(pSkipped);
    }
    var headerAdded = document.createElement("h2");
    $(headerAdded).html("Added:");
    container.append(headerAdded);
    for (var j in data.Added) {
        var pAdded = document.createElement("p");
        $(pAdded).html(data.Added[j].Date + "    " + '<a href="' + data.Added[j].url + '">' + data.Added[j].TrackName + '</a>');
        container.append(pAdded);
    }
    $("#url-form").slideUp(300, function () { });
}

function LoadClick() {
    $("#progress-bar").animate({
        width: '100%'
    }, 10000, function () {
        //complete
    });
    //"load-url-text-area"
    $.ajax({
        url: '/Home/Load',
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify({ Url: $("#load-url-text-area").val() }),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            DisplayData(data);
        }
    });
}

$("#minimize").click(function () {
    $("#url-form").slideUp(300, function () { });
});

$("#maximize").click(function () {
    $("#url-form").slideDown(300, function () { });
});

$(document).ready(function () {
    $("#load-button").click(LoadClick);
});
