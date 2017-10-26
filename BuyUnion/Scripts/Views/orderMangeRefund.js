$("#btnRefund").click(function (e) {
    var $this = $(this);
    $this.prop("disable", true).val("退款");
    $.ajax({
        type: "POST",
        url: comm.action("Refund", "OrderManage"),
        data: { id: $this.data("id") },
        dataType: "json",
        success: function (data) {
            if (data.State == "Success") {
                comm.alter(1, data.Message);
                setTimeout(function () {
                    location = comm.action("Index", "OrderManage", { state: "Paid" });
                }, 1000);
            } else {
                comm.alter(0, data.Message);
                $this.prop("disable", false).val("退款");
            }

        }
    });
});