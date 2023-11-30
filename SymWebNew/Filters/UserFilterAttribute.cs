using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SymVATWebUI.Filters
{
    public class UserFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

            //HttpCookie authCookie = filterContext.HttpContext.Request.Cookies[CompanyName];

            //if (authCookie == null)
            //{
            //    filterContext.Result = new RedirectToRouteResult(
            //        new RouteValueDictionary {{ "Controller", "Home" },
            //            { "Action", "Login" } });
            //}

            var type = filterContext.HttpContext.Session.Contents["LogInBranch"];

            if (type == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {{ "Controller", "Home" },
                        { "Action", "Login" } });
            }

            //if (type != null && type.ToString() == "Client")
            //{
            //    filterContext.Result = new RedirectToRouteResult(
            //        new RouteValueDictionary {{ "Controller", "Home" },
            //            { "Action", "Login" } });
            //}

            base.OnActionExecuting(filterContext);
        }
    }


    public class ShampanAuthorizeAttribute : AuthorizeAttribute
    {
       
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

            HttpCookie authCookie = httpContext.Request.Cookies[CompanyName];

            return authCookie != null;
        }
    }
}