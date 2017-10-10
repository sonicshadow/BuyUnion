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

            //公共js
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

            //view js
            Action<string, string[]> addViewScripts = (name, js) =>
            {
                js = js.Select(s => s.Contains("~") ? s : $"~/Scripts/View/{s}").ToArray();
                bundles.Add(new ScriptBundle($"~/bundles/{name}").Include(js));
            };

            addViewScripts("roleGroup", new string[] { "roleGroup.js" });
        }
    }
}
