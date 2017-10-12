
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
