using System.Web;
using System.Web.Optimization;

namespace Anke.SHManage.Web
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            //bundles 需要小心不要添加 mini.js 文件会自动过滤
            bundles.Add(new ScriptBundle("~/AnkeJS").Include(
                "~/Scripts/jquery.AnkeExtend.js",
                 "~/Scripts/jquery.cookie.js",
                 "~/Scripts/changeEasyuiTheme.js"
                ));
            bundles.Add(new ScriptBundle("~/JqueryJS").Include(
             "~/Scripts/jquery-{version}.js",
              "~/Scripts/jquery.unobtrusive-ajax.js",
              "~/Scripts/jquery.validate.js",
              "~/Scripts/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/EasyUIJS").Include(
                 "~/Content/EasyUI/jquery.js",
                 "~/Content/EasyUI/jquery.easyui.js",
                 "~/Scripts/datagrid-detailview.js",
                 "~/Content/EasyUI/locale/easyui-lang-zh_CN.js"));

            bundles.Add(new StyleBundle("~/EasyUICSS").Include(
                "~/Content/EasyUI/themes/metro/easyui.css",
                "~/Content/EasyUI/themes/icon.css"));
        }
    }
}
