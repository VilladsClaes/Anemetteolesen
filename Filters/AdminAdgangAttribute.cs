using System.Web.Mvc;

namespace Anemette.Filters
{
    //Tillad kun adgang hvis administratoren er logget ind. Tjekkes FØR handlingen udføres,
    //så sletning/oprettelse aldrig kører for en bruger der ikke er logget ind.
    public class AdminAdgangAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["LoginBruger"] == null)
            {
                filterContext.Result = new RedirectResult("/Home/Login");
            }
        }
    }
}
