

$(document).ready(function () {
    var forslag = $("#forslagsliste"); //<datalist id="forslagsliste">
    var postnummerfelt = $("#inpPostnr")

    var wsurl = "https://dawa.aws.dk/adresser/autocomplete?q="

    
    function KaldWebservice() {

        $.ajax({
            type: "GET",
            dataType: "json",
            url: wsurl,
            success: function (data) {
                console.log(data)
                udskrivData(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("FEJL: " + jqXHR + textStatus + errorThrown)

            }
        })
    }
    
    function udskrivData(data) {


        forslag.html(""); //for hver gang der udskrives skal html-indholdet være tomt

        for (var x in data) {

            forslag.append("<option>" + data[x].tekst + "</option>") //dette lægges ind i  <datalist id="forslagsliste"> 

        };
       
        console.log(wsurl)

    };


    postnummerfelt.on('input', function () {

        wsurl = "https://dawa.aws.dk/postnumre/autocomplete?q=" + postnummerfelt.val()

        KaldWebservice();

    });


    postnummerfelt.on("select", function () {

        alert("du har valgt ")

    });



});



