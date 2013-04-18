
function Runner(trap, name) {
    var self = this;
    self.Trap = ko.observable(trap);
    self.Name = ko.observable(name);
    self.TrapLabel = ko.computed(function () { //using this to provide a title for each input for the individual race section
        return "Trap " + self.Trap();
    });
}

function GetTableElement(dataElement) {
    var td = document.createElement("td");
    $(td).html(dataElement);
    return td;
}

//display the dummy scores in the element 'target'
function DisplayData(data, target) {
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
            target.append(tableRow);
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
    self.runners = ko.observableArray([ //used for table initialisation only, the collection that goes to server is runnersFull
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

    //pushes a single race worth of greyhound names
    self.upload = function () {
        $.ajax({
            url: '/Home/GetPredictions',
            type: 'POST',
            dataType: 'json',
            data: ko.toJSON(self.runnersFull()),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                DisplayData(data, $("#result-table"));
            }
        });
    };

    //custom binding for the ko_autocomplete html element (the name input box) that makes the input a jquery autocomplete and handles
    //getting the autocomplete list, pushing the selcted item into the runners collection and removing a deleted runner from the runners collection.
    //params().trapValue() takes its value from this: {trapValue: Trap} (in the input's databind field), 'Trap' is an attribute of Runner and is also 
    //part of the 'value: ' binding.
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
                select: function (event, ui) { //autocomplete item selected so push it to the runners collection
                    self.runnersFull.push(new Runner(params().trapValue(), ui.item.value));
                },
                change: function (event, ui) {
                    if (ui.item === null) { //name deleted so need to remove that runner
                        for (var i in self.runnersFull()) {
                            if (self.runnersFull()[i].Trap() === params().trapValue()) {
                                self.runnersFull.remove(self.runnersFull()[i]);
                            }
                        }
                    }
                }

            });
        }
    };

    
}

//pushes a whole meetings worth of greyhound names to the server
$("#load-meeting").click(function () {
    var runners = $("#race-meeting-textarea").val();
    var data = JSON.stringify({ multipleRace_Runners: $("#race-meeting-textarea").val() });
    $.ajax({
        url: '/Home/GetPredictions',
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify({ multipleRace_Runners: $("#race-meeting-textarea").val() }),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            DisplayData(data, $("#batch-result-table"));
        }
    });
});

$(document).ready(function () {
    ko.applyBindings(new RunnerInputViewModel());
});