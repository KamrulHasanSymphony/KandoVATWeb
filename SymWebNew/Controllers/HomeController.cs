using SymOrdinary;
using SymRepository;
//using SymRepository.Common;
//using SymViewModel.Common;
using SymVATWebUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VATViewModel.DTOs;


namespace SymVATWebUI.Controllers
{


    [OutputCache(NoStore = true, Duration = 150)]
    [Authorize]
    public class HomeController : Controller
    {

        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            #region Ticket Clear

            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null && authCookie.Value != "")
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                authCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Response.Cookies.Add(authCookie);
            }

            #endregion  

            #region Session Clear
            
            UserLogsVM vm = new UserLogsVM();
            Session["User"] = "";
            Session["FullName"] = "";
            Session["UserType"] = "";
            Session["EmployeeId"] = "";
            Session["SessionDate"] = "";
            Session["SessionYear"] = "";

            Session["LogInBranch"] = null;
            Session["LogInLoginTime"] = null;
            Session["LogInUserName"] = null;
            Session["BranchId"] = null;
            Session["LogInUserId"] = null;
            //Session["AllRoles"] = null;
            //Session["UserRoles"] = null;
            Session["result"] = null;
            Session["CompanyName"] = null;
            Session["CompanyCode"] = null;
            Session["SessionDate"] = null;
            Session["SessionYear"] = null;
            Session["BranchCode"] = null;
            Session["BranchId"] = null;

            Session.Abandon();

            #endregion


            ViewBag.ReturnUrl = returnUrl;
            vm.ReturnUrl = returnUrl;
            vm.SessionDate = DateTime.Now.ToString("dd-MMM-yyyy");

            return RedirectToAction("Login", "Home", new { area = "VMS" });
        }
        [AllowAnonymous]
        public ActionResult Client(string returnUrl)
        {

            return View();
        }
        [AllowAnonymous]
        public ActionResult ContactUs(string returnUrl)
        {
            EmailSettings ems = new EmailSettings();

            return View(ems);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ContactUs(EmailSettings ems)
        {
            //var result=   EmpEmailProcess(ems);

            return View();
        }
        [AllowAnonymous]
        public ActionResult AboutUs(string returnUrl)
        {

            return View();
        }
        
        public ActionResult LogOut()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null && authCookie.Value != "")
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                authCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Response.Cookies.Add(authCookie);
            }
            Session["User"] = "";
            Session["FullName"] = "";
            Session["UserType"] = "";
            Session["EmployeeId"] = "";
            Session["SessionDate"] = "";
            Session["SessionYear"] = "";
            Session["mgs"] = "";
            return RedirectToAction("Index");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("/hrm/employeeinfo");
            }
        }

        public ActionResult Test()
        {

            return View();
        }

    }
}
