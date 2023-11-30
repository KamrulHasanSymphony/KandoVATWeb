using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;

namespace SymVATWebUI.Filters
{
    //public class RoleFilterAttribute : ActionFilterAttribute
    //{
    //    public string FormId { get; set; }

    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        var allRoles = filterContext.HttpContext.Session.Contents["AllRoles"];
    //        var userRoles = filterContext.HttpContext.Session.Contents["UserRoles"]; ;


    //        if (allRoles == null)
    //        {
    //            filterContext.Result = new RedirectToRouteResult(
    //                new RouteValueDictionary {{ "Controller", "Home" },
    //                    { "Action", "Login" } });
    //        }

    //        if (userRoles == null)
    //        {
    //            filterContext.Result = new RedirectToRouteResult(
    //                new RouteValueDictionary {{ "Controller", "Home" },
    //                    { "Action", "Login" } });
    //        }


    //        #region Check Company Role

    //        DataTable table = JsonConvert.DeserializeObject<DataTable>(allRoles.ToString());

    //        DataRow[] rows = table.Select("FormID = '" + FormId + "'");

    //        if (rows.Length == 0)
    //        {
    //            filterContext.Result = new RedirectToRouteResult(
    //                new RouteValueDictionary {{ "Controller", "Home" },
    //                    { "Action", "Login" } });
    //        }


    //        if (rows.Length != 0 && rows[0]["Access"].ToString() != "1")
    //        {
    //            filterContext.Result = new RedirectToRouteResult(
    //                new RouteValueDictionary {{ "Controller", "Home" },
    //                    { "Action", "Login" } });
    //        }

    //        #endregion


    //        table = JsonConvert.DeserializeObject<DataTable>(userRoles.ToString());

    //        rows = table.Select("FormID = '" + FormId + "'");

    //        if (rows.Length == 0)
    //        {
    //            filterContext.Result = new RedirectToRouteResult(
    //                new RouteValueDictionary {{ "Controller", "Home" },
    //                    { "Action", "Login" } });
    //        }


    //        if (rows.Length != 0 && rows[0]["Access"].ToString() != "1")
    //        {
    //            filterContext.Result = new RedirectToRouteResult(
    //                new RouteValueDictionary {{ "Controller", "Home" },
    //                    { "Action", "Login" } });
    //        }


    //        base.OnActionExecuting(filterContext);
    //    }
    //}
}