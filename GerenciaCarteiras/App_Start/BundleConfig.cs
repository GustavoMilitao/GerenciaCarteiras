using System.Web;
using System.Web.Optimization;

namespace GerenciaCarteiras
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                        .Include("~/Scripts/jquery-{version}.js")
                        .Include("~/Scripts/jquery.validate.js"));


            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                        "~/Scripts/GerenciaCarteiras/login.js"));

            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                        "~/Scripts/GerenciaCarteiras/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/menu").Include(
                        "~/Scripts/GerenciaCarteiras/menu.js"));

            bundles.Add(new ScriptBundle("~/bundles/paginacao").Include(
                        "~/Scripts/GerenciaCarteiras/paginacao.js"));

            bundles.Add(new ScriptBundle("~/bundles/log4js").Include(
                        "~/Scripts/GerenciaCarteiras/log4js.js"));

            bundles.Add(new ScriptBundle("~/bundles/default").Include(
                        "~/Scripts/GerenciaCarteiras/default.js"));

            bundles.Add(new ScriptBundle("~/bundles/logs2indexeddb").Include(
                        "~/Scripts/GerenciaCarteiras/logs2indexeddb.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
