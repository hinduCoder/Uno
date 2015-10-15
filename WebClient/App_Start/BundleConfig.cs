using System.Web.Optimization;

namespace WebClient
{
    public class BundleConfig
    {
        private static string Script(string path)
        {
            return $"~/Scripts/lib/{path}";
        }
        private static string Style(string path)
        {
            return $"~/Content/{path}";
        }
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js/basic").Include(
                Script("jquery-2.1.4.min.js"),
                Script("bootstrap.min.js"),
                Script("mustache.min.js")));
            bundles.Add(new ScriptBundle("~/js/signalr").Include(
                Script("jquery.signalR-2.2.0.min.js"),
                "~/signalr/hubs"));
            bundles.Add(new ScriptBundle("~/js/angular")
                .Include(Script("angular/angular.min.js"))
                .Include(Script("angular/angular-sanitize.min.js"))
                .Include(Script("angular/angular-animate.min.js"))
                .Include(Script("angular-ui/ui-bootstrap-tpls.min.js"))
                .IncludeDirectory(Script("angular"), "*.min.js", false)
                .IncludeDirectory(Script("angular-ui"), "*.min.js", false));

            bundles.Add(new StyleBundle("~/css/bootstrap").Include(
                Style("bootstrap.min.css"),
                Style("bootstrap-theme.min.css"),
                Style("ui-bootstrap-csp.css")));
        }
    }
}