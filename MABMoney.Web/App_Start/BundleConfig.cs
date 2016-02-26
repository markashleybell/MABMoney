using System.Web.Optimization;

namespace MABMoney.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            var jqueryCdnUrl = "//ajax.googleapis.com/ajax/libs/jquery/2.2.0/jquery.min.js";
            var bootstrapJsCdnUrl = "//maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js";
            var bootstrapCssCdnUrl = "//maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css";

            bundles.Add(new ScriptBundle("~/Scripts/jquery", jqueryCdnUrl).Include(
                "~/Scripts/jquery-{version}.js")
            );

            bundles.Add(new ScriptBundle("~/Scripts/bootstrap", bootstrapJsCdnUrl).Include(
                "~/Scripts/bootstrap.js")
            );

            bundles.Add(new ScriptBundle("~/Scripts/base").Include(
                "~/Scripts/bootstrap3-typeahead.js",
                "~/Scripts/bootstrap-datepicker.js",
                "~/Scripts/jquery.cookie.js",
                "~/Scripts/mustache.js",
                "~/Scripts/main.js")
            );

            bundles.Add(new StyleBundle("~/Content/bootstrap", bootstrapCssCdnUrl).Include(
                "~/Content/bootstrap.css")
            );

            bundles.Add(new StyleBundle("~/Content/base").Include(
                "~/Content/bootstrap-datepicker.css",
                "~/Content/styles.css")
            );
        }
    }
}