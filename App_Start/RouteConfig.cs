using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Anemette
{
    //One of the most important things about routing is that routes have to be added in the order of most specific first, with more general routes further down the list. The routing system searches down the routes until it finds anything that matches the current URL and then it stops. If there is a general route that matches a URL and a more specific route that also matches, but it is defined below the more general route then the more specific route will never be used.
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            //Rækkefølgen af ruter er afgørende. De mest specifikke skal stå øverst
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Denne rute har ikke et ID med sig, og derfor skal den have sin egen rute, hvorimod Edit og Delete har et id og derfor skal følge en anden rute
            routes.MapRoute(
                 //The name parameter represents the name of the route and can be left blank; however, we'll be using them in this book to differentiate between routes.
                 name: "OpretProdukt",
                 //The url parameter contains a rule for matching the route to a URL format. This can contain several formats and arguments, as follows:
                 url: "Produkt/Opret",
                 defaults: new { controller = "Produkt", action = "OpretProdukt" }
             );

            routes.MapRoute(
               name: "ProdukterInddeltEfterKategoriInddeltPaaSider",
               url: "Produkter/{category}/Side{side}",
               defaults: new { controller = "Produkt", action = "Index" }
           );


            routes.MapRoute(
                name: "ProdukterInddeltPaaSider",
                url: "Produkter/Side{side}",
                defaults: new { controller = "Produkt", action = "Index" }
            );

            routes.MapRoute(
                name: "ProdukterInddeltEfterKategori",
                url: "Produkter/{category}",
                //Each segment can also be specified as being optional or having a default value if the corresponding element of the URL is blank. A good example of this is the default route specified when the project was created. This route uses the following code to specify default values for the controller and action method to be used. It also defines the id element as being optional:
                defaults: new { controller = "Produkt", action = "Index" }
            );

            routes.MapRoute(
                name: "ProduktOversigt",
                url: "Produkter",
                defaults: new { controller = "Produkt", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                //ASP.NET MVC also automatically matches any parameters in the HTTP request to a method parameter if they have the same name. The matching is case insensitive. For example, entering the URL http://localhost:58735/Home/About?id=7 will return the same result as previously, because the id parameter in the query string is automatically mapped to the id parameter in the About method
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
