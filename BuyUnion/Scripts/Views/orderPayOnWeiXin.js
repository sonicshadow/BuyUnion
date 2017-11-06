var wxJsApiParam = $("#wxJsApiParam").data("param");
var code = $("#Code").val();

function jsApiCall() {
    WeixinJSBridge.invoke(
    'getBrandWCPayRequest',
    wxJsApiParam,
    function (res) {
        WeixinJSBridge.log(res.err_msg);
        if (res.err_msg == "get_brand_wcpay_request:ok") {
            //location = comm.action("Result", "Order", { code: code, type: 0 });
            $("#orderPayState").fadeIn();
        } else {
            //alert(res.err_code + res.err_desc + res.err_msg);
        }
    }
    );
}

function callpay() {

    if (typeof WeixinJSBridge == "undefined") {
        if (document.addEventListener) {
            document.addEventListener('WeixinJSBridgeReady', jsApiCall, false);
        }
        else if (document.attachEvent) {
            document.attachEvent('WeixinJSBridgeReady', jsApiCall);
            document.attachEvent('onWeixinJSBridgeReady', jsApiCall);
        }
    }
    else {
        jsApiCall();
    }
}

$("#btnSubmit").click(function () {
    $(this).attr("disabled", 'true');
    $(this).val("支付中...")
    if (new check().isWeiXin()) {
        callpay();
    }
    else {
        location = encodeURI(wxJsApiParam.mweb_url);
    }
})

$("#closed").click(function () {
    comm.action("Submit", "Orders", { code: $("#Code").val() });
});

PsySuccess();
function PsySuccess() {
    var paySuccess = setInterval(clock, 1500);
    function clock() {
        $.ajax({
            type: "GET",
            url: comm.action("Get", "Order"),
            data: { code: $("#Code").val() },
            dataType: "json",
            success: function (data) {
                if (data.State == "Success") {
                    if (data.Result.PayCode != null) {
                        clearInterval(paySuccess);
                        $("#orderPay").addClass("hidden");
                        $("#orderPayState").removeClass("hidden");
                    }
                }
            }
        });
    }


}

