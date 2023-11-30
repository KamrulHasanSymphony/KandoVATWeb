using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace SymVATWebUI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            var path = HttpContext.Current.Request.Path;
            if (path.StartsWith("/bundles/jqueryval"))
            {
                return;
            }

            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            HttpCookie authCookie = Request.Cookies[CompanyName];
            if (authCookie != null && authCookie.Value != "")
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);


                ShampanIdentity identity = new ShampanIdentity(ticket.UserData);
                var basicTicket = Application["BasicTicket" + identity.Name + identity.InitialCatalog];
                var roleTicket = Application["RoleTicket" + identity.Name + identity.InitialCatalog];
                bool isHigher = false;
                if (basicTicket != null && roleTicket != null && basicTicket.ToString() == ticket.UserData && !isHigher)
                {
                    identity.SetRoles(roleTicket.ToString());
                    ShampanPrincipal principal = new ShampanPrincipal(identity);
                    HttpContext.Current.User = principal;
                    Thread.CurrentPrincipal = principal;
                    int timeOut = Convert.ToInt32(new AppSettingsReader().GetValue("COOKIE_TIMEOUT", typeof(string)));
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, FormsAuthentication.FormsCookieName, DateTime.Now, DateTime.Now.AddMinutes(timeOut), ticket.IsPersistent, ticket.UserData);
                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                    //////try
                    //////{
                    //////    var sec = Session["UserType"].ToString();
                    //////}
                    //////catch (Exception)
                    //////{

                    //////    authCookie.Expires = DateTime.Now.AddDays(-1);
                    //////    HttpContext.Current.Response.Cookies.Add(authCookie);
                    //////    Application["BasicTicket" + identity.Name] = null;
                    //////    Application["RoleTicket" + identity.Name] = null;
                    //////    HttpContext.Current.Response.Redirect("/");
                    //////}
                    return;
                }
                else
                {
                    authCookie.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(authCookie);
                    Application["BasicTicket" + identity.Name + identity.InitialCatalog] = null;
                    Application["RoleTicket" + identity.Name + identity.InitialCatalog] = null;
                    HttpContext.Current.Response.Redirect("/");
                }
            }
            else
            {
                //HttpContext.Current.Response.Redirect("/Home/Login");
            }

        }

    }
}