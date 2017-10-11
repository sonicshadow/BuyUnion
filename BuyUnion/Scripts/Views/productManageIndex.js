$("[data-state]").click(function (e) {

    var $this = $(this);
    var id = $this.data("id");
    var state = $this.data("state");
    var name = $this.text();
    $.ajax({
        type: "POST",
        url: comm.action("ChangeState", "ProductManage"),
        data: { id: id, state: state },
        dataType: "json",
        success: function (data) {
            if (data.State == "Success") {
                comm.alter(1, data.Message);
                setTimeout(function () {
                    location = location;
                }, 1000);
            } else {
                comm.alter(0, data.Message);
            }
        }
    });
    return false;
});