using System.Web.Mvc;

namespace SymVATWebUI.Areas.Common
{
    public class CommonAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Common";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Common_default",
                "Common/{controller}/{action}/{id}",
              new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "SymVATWebUI.Areas.Common.Controllers" }
            );
        }
    }
}
