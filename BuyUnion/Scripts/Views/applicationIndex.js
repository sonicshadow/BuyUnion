$(".btn-check").click(function (e) {
    $.ajax({
        type: "POST",
        url: comm.action("Check", "Withdraw"),
        data: { id: $(this).data("id"), result: $(this).data("result") },
        dataType: "json",
        success: function (data) {
            if (data.State == "Success") {
                comm.alter(1, data.Message);
            } else {
                comm.alter(0, data.Message);
            }
        }
    });
});