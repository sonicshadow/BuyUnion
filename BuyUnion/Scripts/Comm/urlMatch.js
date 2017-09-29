var urlMatch = function (option) {
    var $target = $(option.target);
    var $URlText= $(".urlMatch").children(":text");
    var $urlMatch = $(".urlMatch_content")
    var $close = $urlMatch.find(".close");
    var $type = $urlMatch.find("#type");
    var $keyWord = $urlMatch.find(".keyWord");
    var $submit = $urlMatch.find(".submit");
    var $order = $urlMatch.find(".order");
    var $searchResult = $urlMatch.find(".searchResult");
    var $fixedList = $urlMatch.find(".fixedList");
    if (option == undefined) {
        option = {
            selected: function () { }
        };
        $target = $URlText;
        $URlText.removeClass("hidden");
    } else {
        if (option.selected == undefined) {
            option.selected = function () { };
        }
        if (option.target == undefined) {
            $target = $URlText;
            $URlText.removeClass("hidden");
        }
    }
    $target.focus(function (e) {
        $urlMatch.removeClass("hidden");
    });
    $close.click(function (e) {
        $urlMatch.addClass("hidden");
    });
    $type.change(function (e) {
        ajax();
    });
    $submit.click(function (e) {
        ajax();
    });
    $order.click(function (e) {
        $searchResult.empty();
        $fixedList.removeClass("hidden");
    });
    $fixedList.find(".item").click(function (e) {
        $urlMatch.addClass("hidden");
        $target.val($(this).find(".url").text());
    });
    function ajax() {
        var data = {};
        data.keyWord = $keyWord.val();
        data.type = $type.val();
        $.ajax({
            type: "GET",
            url: comm.action("Index", "UrlMatch"),
            data: data,
            dataType: "html",
            success: function (data) {
                $fixedList.addClass("hidden");
                $searchResult.empty().append(data);
                $searchResult.find(".item").click(function (e) {
                    var data = JSON.parse($(this).find(".info").val());
                    $target.attr("value", data.URL);
                    option.selected(data);
                    $urlMatch.addClass("hidden");
                    //getChild();
                });
            }
        });
    }

    this.show = function () {
        $urlMatch.removeClass("hidden");
    };
    this.hide = function () {
        $urlMatch.addClass("hidden");
    }
}

//var select = new searchContent("#text", {
//    selected: function (data) {
//        $("#text").val(data.URL);
//    }
//})
