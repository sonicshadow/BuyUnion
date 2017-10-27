//移动端js
var swiper = new Swiper('.homeIndexM-swiper .swiper-container', {
    direction: 'vertical'
});

var uat = navigator.userAgent;
var isWX = (uat.toLowerCase().match(/MicroMessenger/i) == 'micromessenger');
var isiOS = !!uat.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/); //ios终端
var isIosWebkit = !!window.webkit && !isWX;
var isWebkit = !!window.webkit;
var isAndroid = !!window.jsCallNative;
var isApp = isAndroid || isIosWebkit;

$("#btns").click(function () {
    if (isWX || !isiOS) {
        $(".linkToMobile-tip").removeClass("hidden");
    }
});

$("#closed").click(function () {
    $(".linkToMobile-tip").addClass("hidden");
})