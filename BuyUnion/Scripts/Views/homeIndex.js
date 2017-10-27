//移动端js
var swiper = new Swiper('.homeIndexM-swiper .swiper-container', {
    direction: 'vertical'
});

if (new check().isWeiXin() || location.getUrlParam("test") == "1") {

    $(".homeIndexM").removeClass("hidden");
}
else {
    $(".homeIndexM-pc").removeClass("hidden");
}