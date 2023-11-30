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
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class ImportController : Controller
    {

        ShampanIdentity identity = null;

        ImportRepo _repo = null;

        public ImportController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new ImportRepo(identity);

            }
            catch
            {

            }
        }


        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //CustomerRepo _repo = new CustomerRepo(identity);
        [Authorize(Roles = "Admin")]
        public ActionResult Index(ImportVM paramVM)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
            }
            ////ImportVM paramVM = new ImportVM();
            return View(paramVM);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult ImportExcel(ImportVM paramVM, DataTable dt = null, SysDBInfoVMTemp connVM = null)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ImportRepo(identity, Session);
            string[] result = new string[6];
            List<ErrorMessage> errormessage = new List<ErrorMessage>();
            //ErrorMessage errormessagevm;

            try
            {
                paramVM.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                paramVM.CreatedBy = identity.Name;
                paramVM.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                paramVM.LastModifiedBy = identity.Name;
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"]);

                ImportRepo iRepo = new ImportRepo(identity, Session);
                result = iRepo.ImportExcelFile(paramVM, null, connVM);

                //errormessage = iRepo.ImportExcelFile_Web(paramVM);

                //if (errormessage.Count > 0)
                //{
                //    return PartialView("_PopUpErrorMessage", errormessage);

                //}

                string Message = result[1].Split('\r').FirstOrDefault();

                Session["result"] = result[0] + "~" + Message;
                return View("Index", paramVM);
                ////return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                // Session["result"] = result[0] + "~" + result[1];
                // FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("ImportController", "ImportExcel", ex.ToString());
                return View("Index", paramVM);
            }
        }

    }
}
