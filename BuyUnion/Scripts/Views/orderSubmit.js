

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
    this.show = function () {
        _show();
    }

    function _hide() {
        $target.removeClass("hidden");
    }
    this.hide = function () {
        _hide();
    }

    function _seleced(prov, city, dist) {
        option.selected(prov, city, dist);
    }

    $lstCity.children().click(function () {
        var city = $(this).text();
        $lstCity.text(city);
        console.log(city);
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
                    $tabCity.removeClass("active");

                    $tabDist.removeClass("hidden");
                    $tabDist.addClass("active");
                    $lstDist.children().click(function () {
                        _hide();
                        var dist = $(this).text();
                        $lstDist.text(dist);
                        _seleced("广东", city, dist);
                    });
                }
            }
        });
    });


}


var $colAddress = new colAddress();

$("#selectedAddress").click(function (e) {
    $colAddress.show();
});

$("[name=Type]").change(function (e) {
    if ($("#typeExpress").is(":checked")) {
        $(".consigneeInfo").removeClass("hidden");
    } else {
        $(".consigneeInfo").addClass("hidden");
    }
});