
function Runner(trap, name) {
    var self = this;
    self.Trap = ko.observable(trap);
    self.Name = ko.observable(name);
    self.TrapLabel = ko.computed(function () {
        return "Trap " + self.Trap();
    });
}

//for (var i in data) {
//    var image = document.createElement("img");
//    ///Home/GetProductDetails?productImageID=1
//    var link = document.createElement("a");
//    var URL = "/Home/GetProductDetails?productImageID=" + data[i].ImageID;
//    link.setAttribute("href", URL);
//    image.setAttribute("src", data[i].ImageURL);
//    image.setAttribute("alt", "ECS image");
//    image.setAttribute("width", 100);
//    image.setAttribute("height", 100);
//    image.setAttribute("id", data[i].ImageID);
//    $(link).append(image);
//    $("#imagesContainer").append(link);
//}

//@* public string Name { get; set; }
//public bool NoData { get; set; }
//public bool scoreField1 { get; set; }
//public int Score { get; set; }
//public string Comments { get; set; }
//public string scoreField2 { get; set; }
//public int Trap { get; set; }
//public string scoreField3 { get; set; }
//public string scoreField4 { get; set; }
//public string scoreField5 { get; set; }
//public string scoreField6 { get; set; }
//public string scoreField7 { get; set; }
//public string GradesUsed { get; set; }*@

function GetTableElement(dataElement) {
    var td = document.createElement("td");
    $(td).html(dataElement);
    return td;
}

function DisplayData(data) {
    for (var i = 0; i < data.length; i++) {
        for (var j = 0; j < data[i].Scores.length; j++) {
            var tableRow = document.createElement("tr");
            $(tableRow).append(GetTableElement(data[i].Scores[j].Name));
            $(tableRow).append(GetTableElement(data[i].Scores[j].NoData));
            $(tableRow).append(GetTableElement(data[i].Scores[j].scoreField1));
            $(tableRow).append(GetTableElement(data[i].Scores[j].Score));
            $(tableRow).append(GetTableElement(data[i].Scores[j].Comments));
            $(tableRow).append(GetTableElement(data[i].Scores[j].scoreField2));
            $(tableRow).append(GetTableElement(data[i].Scores[j].Trap));
            $(tableRow).append(GetTableElement(data[i].Scores[j].scoreField3));
            $(tableRow).append(GetTableElement(data[i].Scores[j].scoreField4));
            $(tableRow).append(GetTableElement(data[i].Scores[j].scoreField5));
            $(tableRow).append(GetTableElement(data[i].Scores[j].scoreField6));
            $(tableRow).append(GetTableElement(data[i].Scores[j].scoreField7));
            $(tableRow).append(GetTableElement(data[i].Scores[j].GradesUsed));
            $("#result-table").append(tableRow);
        }
        
    }
}

function RunnerInputViewModel() {
    var self = this;

    //Lances Gallahad
    //Shakalakaboom
    //Vegas Gold
    //Burn Brae
    //Excel Twotone
    //Opening Piper
    self.runners = ko.observableArray([
        new Runner(1, ""),
        new Runner(2, ""),
        new Runner(3, ""),
        new Runner(4, ""),
        new Runner(5, ""),
        new Runner(6, ""),
    ]);

    self.runnersFull = ko.observableArray([]);

    self.AddRunner = function () {
        self.runners.push(new Runner(0, ""));
    };

    self.upload = function () {
        $.ajax({
            url: '/Home/GetPredictions',
            type: 'POST',
            dataType: 'json',
            data: ko.toJSON(self.runnersFull()),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                var text = "Scores: ";
                //data[0].Scores[0].scoreField6
               
                for (var i = 0; i < data.length; i++) {
                    for (var j = 0; j < data[i].Scores.length; j++) {
                        text += data[i].Scores[j].Name;
                    }
                }
                DisplayData(data);
            }
        });
    };

    ko.bindingHandlers.ko_autocomplete = {
        init: function (element, params) {
            $(element).autocomplete();
        },
        update: function (element, params) {
            $(element).autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: '/Home/GetAutocompleteNames',
                        type: 'POST',
                        dataType: 'json',
                        data: JSON.stringify({ Content: request.term }),
                        contentType: 'application/json; charset=utf-8',
                        success: function (data) {
                            response(data);
                        }
                    });
                },
                select: function (event, ui) {
                    self.runnersFull.push(new Runner(params().trapValue(), ui.item.value));
                }

            });
        }
    };

}

$(document).ready(function () {
    ko.applyBindings(new RunnerInputViewModel());
});