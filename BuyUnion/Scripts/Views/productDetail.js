
var buyUrl = "http://localhost:62209/";
function GetParent() {
    $.ajax({
        type: "GET",
        url: buyUrl + "User/GetParentUserID",
        data: { userId: $("#userId").val() },
        dataType: "json",
        success: function (data) {
            if (data.State == "Success")
            {
                $("#proxyID").val(data.Result.ProxyID);
                $("#childProxyID").val(data.Result.ChildProxyID);
            }
        }
    });
}
GetParent();

//swiper轮播
var swiper = new Swiper('.productDetails-swiper .swiper-container', {
    pagination: '.productDetails-swiper .swiper-pagination',
    paginationClickable: true
});

//图文详情
$("[name='slide'] .title").click(function () {
    $(this).parent().find(".detail").slideToggle();
    $(this).toggleClass("active");
});

//规格
$("#speclist li").click(function () {
    $("#speclist li").removeClass("active");
    $(this).addClass("hidden");
});

//数量
var plus = $("#plus");
var minus = $("#minus");
var sumBox = $("input[name='buynum']");
var sum = sumBox.val();
plus.click(function () {
    sum++;
    minus.removeClass("minus-disable");
    sumBox.val(sum);
});

minus.click(function () {
    if (sum == "1") {
        minus.addClass("minus-disable");
    } else {
        sum--;

        if (sum == "1") {
            minus.addClass("minus-disable");
        }
    }
    
    sumBox.val(sum);
});

//返回
$("#btnBack").goback(comm.action("Index", "Home"));