using System.Web;
using System.Web.Optimization;

namespace Amirhome
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/custom_scripts").Include(
                      "~/Scripts/shared_script.js",
                      "~/Scripts/jalaali.js",
                      "~/Plugins/select2/select2.js",
                      "~/Plugins/select2/select2_locale_fa.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-rtl.css",
                      "~/Content/site.css",
                      "~/Content/LinkEffect.css",
                      "~/Content/font-awesome.css",
                      "~/Plugins/select2/select2.css",
                      "~/Plugins/select2/select2-bootstrap.css"));
        }
    }
}
