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

            bundles.Add(new ScriptBundle("~/Content/Js/jquery", jqueryCdnUrl).Include(
                "~/Content/Js/jquery.js")
            );

            bundles.Add(new ScriptBundle("~/Content/Js/bootstrap", bootstrapJsCdnUrl).Include(
                "~/Content/Js/bootstrap.js")
            );

            bundles.Add(new ScriptBundle("~/Content/Js/base").Include(
                "~/Content/Js/bootstrap-typeahead.js",
                "~/Content/Js/bootstrap-datepicker.js",
                "~/Content/Js/jquery.cookie.js",
                "~/Content/Js/main.js")
            );

            bundles.Add(new StyleBundle("~/Content/Css/bootstrap", bootstrapCssCdnUrl).Include(
                "~/Content/Css/bootstrap.css")
            );

            bundles.Add(new StyleBundle("~/Content/Css/base").Include(
                "~/Content/Css/datepicker.css",
                "~/Content/Css/styles.css")
            );
        }
    }
}