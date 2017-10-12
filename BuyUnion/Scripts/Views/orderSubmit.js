

var colAddress = function (option) {
    if (option == undefined) {
        option = {};
    }
    if (option.selected == undefined) {
        option.selected = function () { };
    }
    var $target = $("#districtSelector");
    var $tabProv = $target.find("#tabProvince");
    var $tabCity = $target.find("#tabCity");
    var $tabDist = $target.find("#tabDistrict");
    var $lstCity = $target.find("#lstCity");
    var $lstDist = $target.find("#lstDistrict");
    $tabCity.click(function () {
        $tabCity.addClass(".active").removeClass("hidden");
        $lstDist.removeClass(".active").addClass("hidden");
    });

    $tabDist.click(function () {
        $tabCity.removeClass(".active").addClass("hidden");
        $lstDist.addClass(".active").removeClass("hidden");
    });

    function _show() {
        $target.removeClass("hidden");
    }

    function _hide() {
        $target.removeClass("hidden");
    }

    function _seleced(prov, city, dist) {
        option.selected(prov, city, dist);
    }

    $tabCity.children().click(function () {
        var city = $(this).text();
        $tabCity.text(city);
        
        $.ajax({
            type: "POST",
            url: comm.action("GetDistrict", "AddressSelector"),
            data: { name: city },
            dataType: "json",
            success: function (data) {
                if (data.State == "Success") {
                    $lstDist.children().remove();
                    $.each(data.Result, function (i, n) {
                        var $li = $("<li>").text(n.Name);
                        $lstDist.append($li);
                    });

                    $lstDist.children().click(function () {
                        _hide();
                        var dist=$(this).text();
                        _seleced("广东", city, dist);
                    });
                }
            }
        });
    });


}


var $colAddress = new colAddress();