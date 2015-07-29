using System.Web.Optimization;

namespace ScampWebFront
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new StyleBundle("~/Styles/GlobalStyle")
                .Include(
                    "~/Content/css/bootstrap.css",
                    "~/Content/css/bootstrap-theme.css",
                    "~/Content/css/index.css",
                    "~/Content/css/toaster.css",
                    "~/Content/css/loading-bar.min.css",
                    "~/Content/css/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/angular")
                .Include(
                    "~/Scripts/angular.min.js",
                    "~/Scripts/angular-resource.min.js",
                    "~/Scripts/angular-animate.min.js",
                    "~/Scripts/angular-route.min.js",
                    "~/Scripts/toaster.js",
                    "~/Scripts/angular-ui/ui-bootstrap.min.js",
                    "~/Scripts/angular-ui/ui-bootstrap-tpls.min.js",
                    "~/Content/app/services/courseService.js",
                    "~/Content/app/services/app.js")
                .IncludeDirectory("~/Content/app/controllers", "*.js")
                );
        }
    }
}
