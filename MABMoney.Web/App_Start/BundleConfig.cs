using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace MABMoney.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            var jqueryCdnUrl = "//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js";
            var bootstrapCssCdnUrl = "//netdna.bootstrapcdn.com/bootstrap/3.1.0/css/bootstrap.min.css";
            var bootstrapJsCdnUrl = "//netdna.bootstrapcdn.com/bootstrap/3.1.0/js/bootstrap.min.js";

            bundles.Add(new ScriptBundle("~/Scripts/jquery").Include(
                "~/Scripts/jquery-{version}.js")
            );

            bundles.Add(new ScriptBundle("~/Scripts/bootstrap").Include(
                "~/Scripts/bootstrap.js")
            );

            bundles.Add(new ScriptBundle("~/Scripts/base").Include(
                "~/Scripts/typeahead-bundle.js",
                "~/Scripts/bootstrap-datepicker.js",
                "~/Scripts/jquery.cookie.js",
                "~/Scripts/mustache.js",
                "~/Scripts/main.js")
            );

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                "~/Content/bootstrap.css")
            );

            bundles.Add(new StyleBundle("~/Content/base").Include(
                "~/Content/bootstrap-datepicker.css",
                "~/Content/styles.css")
            );
        }
    }
}