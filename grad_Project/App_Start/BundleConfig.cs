using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace front_end {

    public class BundleConfig {

        public static void RegisterBundles(BundleCollection bundles) {

            var scriptBundle = new ScriptBundle("~/Scripts/bundle");
            var styleBundle = new StyleBundle("~/Content/bundle");

            // jQuery
            scriptBundle
                .Include("~/Scripts/jquery-3.3.1.js");

            // Bootstrap-min
            scriptBundle
                .Include("~/Scripts/bootstrap.min.js");

            // Bootstrap-min
            styleBundle
                .Include("~/Content/bootstrap.min.css");

            // Custom site styles
            styleBundle
                .Include("~/Content/Site.css");

            bundles.Add(scriptBundle);
            bundles.Add(styleBundle);

#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}