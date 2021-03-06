﻿using System.Web;
using System.Web.Optimization;

namespace XF
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/doTjs").Include(
            "~/Scripts/doT.min.js"));


            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                "~/Scripts/kendo.all.min.js",
                "~/Scripts/kendo.timezones.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/fontawesome-all.min",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/boostrap").Include(
                      "~/Content/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/fontawesome").Include(
                "~/Content/font-awesome.min.css"));


            bundles.Add(new StyleBundle("~/Content/kendo").Include(
                      "~/Content/kendo.common-bootstrap.min.css",
                      "~/Content/kendo.bootstrap.min.css",
                      "~/Content/kendo.dataviz.min.css",
                      "~/Content/kendo.dataviz.bootstrap.min.css"
                      ));
      
        }
    }
}
