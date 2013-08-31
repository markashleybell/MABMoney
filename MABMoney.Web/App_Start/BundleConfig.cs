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
            bundles.Add(new ScriptBundle("~/Content/Js/base").Include(
                "~/Content/Js/bootstrap.js",
                "~/Content/Js/bootstrap-typeahead.js",
                "~/Content/Js/bootstrap-datepicker.js",
                "~/Content/Js/main.js")
            );

            bundles.Add(new StyleBundle("~/Content/Css/base").Include(
                "~/Content/Css/bootstrap.css",
                "~/Content/Css/datepicker.css",
                "~/Content/Css/styles.css")
            );
        }
    }
}