using System.Web;
using System.Web.Optimization;

namespace FinancialPlanner
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

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

            bundles.Add(new StyleBundle("~/bundles/templatestyles").Include(
                      "~/css/bootstrap.min.css",
                      "~/css/sb-admin.css",
                      "~/css/plugins/morris.css",
                      "~/font-awesome/css/font-awesome.min.css",
                      "~/css/animate.css",
                      "~/css/highcharts.css",
                      "~/Content/DataTables/css/jquery.dataTables.min.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/templatescripts").Include(
                      "~/js/jquery.js",
                      "~/js/bootstrap.min.js",
                      "~/js/plugins/morris/raphael.min.js",
                      "~/js/plugins/morris/morris.min.js",
                      "~/js/date.js",
                      "~/js/highcharts.js",
                      "~/js/custom.js",
                      "~/Scripts/DataTables/jquery.dataTables.min.js"
                ));
        }
    }
}
