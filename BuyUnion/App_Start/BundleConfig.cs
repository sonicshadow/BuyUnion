using System;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace BuyUnion
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/bundles/datetimepicker/css").Include(
                     "~/Scripts/datetimepicker/css/bootstrap-datetimepicker.css"));
            bundles.Add(new ScriptBundle("~/bundles/datetimepicker/js").Include(
                     "~/Scripts/datetimepicker/js/bootstrap-datetimepicker.js",
                     "~/Scripts/datetimepicker/js/locales/bootstrap-datetimepicker.zh-CN.js"));

            bundles.Add(new ScriptBundle("~/bundles/comm").Include(
                      "~/Scripts/Comm/canvas-to-blob.min.js",
                      "~/Scripts/Comm/jsEx.js",
                      "~/Scripts/Comm/jQueryEx.js",
                      "~/Scripts/Comm/comm.js",
                      "~/Scripts/Comm/check.js",
                      "~/Scripts/Comm/uploadfile.js",
                      "~/Scripts/Comm/imageResizeUpload.js",
                      "~/Scripts/jquery.lazyload.min.js",
                      "~/Scripts/Comm/imageModule.js",
                      "~/Scripts/Comm/accessLog.js",
                      "~/Scripts/Comm/urlMatch.js",
                      "~/Scripts/Comm/getMessage.js"
                  ));
            bundles.Add(new ScriptBundle($"~/bundles/cloud").Include(
              "~/Scripts/Comm/cloud.js"));
            bundles.Add(new ScriptBundle($"~/bundles/searchUser").Include(
              "~/Scripts/Comm/searchUser.js"));

            bundles.Add(new StyleBundle("~/bundles/swiper/css").Include(
                    "~/Scripts/Swiper/css/swiper.min.css"));
            bundles.Add(new ScriptBundle("~/bundles/swiper/js").Include(
                "~/Scripts/Swiper/js/swiper.jquery.min.js",
                "~/Scripts/Swiper/js/swiper.min.js"));

            //view js
            Action<string, string[]> addViewScripts = (name, js) =>
            {
                js = js.Select(s => s.Contains("~") ? s : $"~/Scripts/Views/{s}").ToArray();
                bundles.Add(new ScriptBundle($"~/bundles/{name}").Include(js));
            };

            addViewScripts("roleGroup", new string[] { "roleGroup.js" });
            addViewScripts("productManageCreate", new string[] { "productManageCreate.js" });
            addViewScripts("productManageIndex", new string[] { "productManageIndex.js" });
            addViewScripts("orderSubmit", new string[] { "orderSubmit.js" });
            addViewScripts("homeIndex", new string[] { "homeIndex.js" });
            addViewScripts("orderPayOnWeiXin", new string[] { "orderPayOnWeiXin.js" });
            addViewScripts("productDetail", new string[] { "productDetail.js" });
        }
    }
}
