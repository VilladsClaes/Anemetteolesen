using System.Web;
using System.Web.Optimization;

namespace Anemette
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            

            //Jeg har tilføjet de tre nederste. De var ikke med da solution blev bygget
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery.validate.unobtrusive.js"));


            
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));


            //Problemer med dropdownmenu på Kontakt-siden?
            //Bootstrap skal installeres med SASS
            //Webcompiler skal kompillere scss til css
            //css-filen skal flyttes via webcompiler-json til content-mappen
            //Popper skal hentes fra umd-mappen istedet for script-mappen
            //rækkefølgen for script-rendering er jquery, popper, bootstrap
            bundles.Add(new ScriptBundle("~/bundles/popper").Include(
                    "~/Scripts/umd/popper.js"));

            bundles.Add(new ScriptBundle("~/bundles/EgneScripts").Include(
                        "~/Scripts/postnummeropslag.js"
                ));

            //Og den skal også rendere mine egne stil-tilføjelser
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/PagedList.css",
                      "~/Content/Site.css"));







        }
    }
}
