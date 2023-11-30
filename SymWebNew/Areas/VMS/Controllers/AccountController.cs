//using JQueryDataTables.Models;
using SymOrdinary;
using SymphonySofttech.Reports.Report;
//using SymRepository.Common;
using SymRepository.VMS;
using SymVATWebUI.Areas.VMS.Models;
//using SymViewModel.Common;
using VATViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymVATWebUI.Areas.vms.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /vms/Branch/
        //SalahUddin

        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        [HttpGet]
        public ActionResult Create()
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/vms/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/vms/Home");
            }
            VendorGroupVM vm = new VendorGroupVM();
            vm.Operation = "add";
            vm.ActiveStatus = "Y";
            return PartialView(vm);
        }
    }
}
