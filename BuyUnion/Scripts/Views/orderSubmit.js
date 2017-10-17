
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
        $tabCity.addClass("active");
        $lstCity.removeClass("hidden");

        $tabDist.addClass("hidden");
        $tabDist.text("选择");
        $lstDist.addClass("hidden");
    });

    $tabDist.click(function () {
        $tabCity.removeClass(".active").addClass("hidden");
        $lstDist.addClass(".active").removeClass("hidden");
    });

    function _show() {

        $target.removeClass("hidden");
        comm.mask2.show();
    }
    this.show = function () {
        _show();
    }

    function _hide() {
        $target.addClass("hidden");
        comm.mask2.hide();
    }
    this.hide = function () {
        _hide();
    }

    function _seleced(prov, city, dist) {
        option.selected(prov, city, dist);
    }

    $lstCity.children().click(function () {
        var city = $(this).text();
        $tabCity.text(city);
        $lstCity.children().removeClass("active");
        $(this).addClass("active");

        $.ajax({
            type: "GET",
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
                    $lstCity.addClass("hidden");

                    $tabDist.removeClass("hidden").addClass("active");
                    $lstDist.removeClass("hidden");
                    $lstDist.children().click(function () {
                        var dist = $(this).text();
                        $(this).addClass("active");
                        $tabDist.text(dist);
                        _seleced("广东", city, dist);
                        _hide();
                    });
                }
            }
        });
    });

}

var $colAddress = new colAddress({
    selected: function (prov, city, dist) {
        $("#selectedAddress").text(prov + city + dist)
    }
});

//判断配送方式
//if ($("#typeExpress").is(":checked")) {
//    $(".consigneeDisplay").removeClass("hidden");
//} else {
//    $(".consigneeDisplay").addClass("hidden");
//}
//改变配送方式
//$("[name=Type]").change(function (e) {
//    if ($("#typeExpress").is(":checked")) {
//        $(".consigneeDisplay").removeClass("hidden");
//    } else {
//        $.ajax({
//            type: "POST",
//            url: comm.action("EditOrder", "Order"),
//            data: { ID: $("#ID").val(), Type: "Offine" },
//            dataType: "json",
//            success: function (data) {
//                if (data.State == "Success") {
//                    $(".consigneeDisplay").addClass("hidden");
//                    changePrice(data.Result.Amount, data.Result.Free)
//                }
//            }
//        });
//    }
//});

//编辑地址
$("#glyphicon-edit").click(function (e) {
    $(".consigneeInfo").removeClass("hidden");
});
//编辑地址返回
$("#editBack").click(function (e) {
    $(".consigneeInfo").addClass("hidden");
});

//选择地区
$("#selectedAddress").click(function (e) {
    $colAddress.show();
});

//点击其他隐藏选择地区
$(".mask").click(function (e) {
    $colAddress.hide();
});

//确认收货地址
$("#addressSure").click(function (e) {
    if ($("#Address").val().trim() == "") {
        comm.promptBox("填写地址");
        return false;
    }
    var ajaxdata = {
        ID: $("#ID").val(),
        Address: $("#selectedAddress").text().trim() + $("#Address").val().trim(),
        PhoneNumber: $("#PhoneNumber").val(),
        Consignee: $("#Consignee").val(),
        Type: "Express",
    };
    $.ajax({
        type: "POST",
        url: comm.action("EditOrder", "Order"),
        data: ajaxdata,
        dataType: "json",
        success: function (data) {
            if (data.State == "Success") {
                $(".consigneeInfo").addClass("hidden");
                $(".consignee").text(ajaxdata.Consignee);
                $(".phoneNumber").text(ajaxdata.PhoneNumber);
                $(".address").text(ajaxdata.Address);
                changePrice(data.Result.Amount, data.Result.Free)
            } else {
                comm.promptBox(data.Message);
            }
        }
    });
});

//收货地址验证
$(".consigneeInfo input").keyup(function (e) {
    var data = {
        Address: $("#Address").val(),
        PhoneNumber: $("#PhoneNumber").val(),
        Consignee: $("#Consignee").val(),
    }
    if (data.Address != "" && data.PhoneNumber != "" && data.Consignee != "") {
        $("#addressSure").prop("disabled", false);
    } else {
        $("#addressSure").prop("disabled", true);
    }
});

//编辑详情
var editDetails = function (target) {
    var $target = $(target);
    var $item = $target.parents(".orderDetail-item");
    var $minus = $item.find(".minus");
    var $plus = $item.find(".plus");
    var $count = $item.find("[name=Count]");

    $minus.click(function (e) {
        $count.val(Number($count.val()) - 1);
        detailsEdit();
        disable();
    });
    disable();
    function disable() {
        if (Number($count.val()) <= 1) {
            $minus.attr("disabled", "true");
        } else {
            $minus.removeAttr("disabled");
        }
    }
    $plus.click(function (e) {
        $count.val(Number($count.val()) + 1);
        detailsEdit();
        disable();
    });

    $count.keyup(function (e) {
        if (Number($count.val()) <= 0) {
            $count.val(1);
        }
        if (Number($count.val()) == NaN) {
            $count.val(1);
        }
    });

    function detailsEdit() {
        var itemInfo = $item.data("item");
        var data = {
            ID: itemInfo.ID,
            count: $count.val()
        };
        $.ajax({
            type: "POST",
            url: comm.action("EditDetails", "Order"),
            data: data,
            dataType: "json",
            success: function (data) {
                if (data.State == "Success") {
                    changePrice(data.Result.Amount, data.Result.Free);
                }
            },
            beforeSend: function () {
                //开始加载
            },
            complete: function () {
                //完成加载
            }
        });
    }
}
new editDetails($(".plus"));

//改变价格
function changePrice(amount, free) {
    var Amount = Number(amount);
    var Free = Number(free);
    $("#Amount").text(Amount.toFixed(2));
    $("#Free").text(Free.toFixed(2));
    $("#Sum").text((Amount + Free).toFixed(2));
}

//返回
$("#btnBack").goback(comm.action("Index", "Home"));
