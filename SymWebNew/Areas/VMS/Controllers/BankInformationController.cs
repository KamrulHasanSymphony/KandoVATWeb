//using JQueryDataTables.Models;
using SymOrdinary;
using SymphonySofttech.Reports.Report;
////////using SymRepository.Common;
using SymRepository.VMS;
using SymVATWebUI.Areas.VMS.Models;
using SymVATWebUI.Filters;
//using SymViewModel.Common;
//using VATViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using VATViewModel.DTOs;

namespace SymVATWebUI.Areas.vms.Controllers
{
    [ShampanAuthorize]
    public class BankInformationController : Controller
    {
        ShampanIdentity identity = null;

        BankInformationRepo _repo = null;

        public BankInformationController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new BankInformationRepo(identity);
            }
            catch
            {

            }
        }
        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //BankInformationRepo _repo = new BankInformationRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
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
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BankInformationRepo(identity, Session);

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
            //00     //BankID 
            //01     //BankName
            //02     //BranchName
            //03     //City
            //04     //TelephoneNo
            //05     //Email


            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<BankInformationVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.BankName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.BranchName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.City.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.TelephoneNo.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Email.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<BankInformationVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.BankName :
                sortColumnIndex == 2 && isSortable_2 ? c.BranchName :
                sortColumnIndex == 3 && isSortable_3 ? c.City :
                sortColumnIndex == 4 && isSortable_4 ? c.TelephoneNo :
                sortColumnIndex == 5 && isSortable_5 ? c.Email :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                  c.BankID
                , c.BankName
                , c.BranchName
                , c.City
                , c.TelephoneNo
                , c.Email
            };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Admin")]
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
            BankInformationVM vm = new BankInformationVM();
            vm.Operation = "add";
            vm.ActiveStatus = "Y";
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(BankInformationVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BankInformationRepo(identity, Session);

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
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedOn = DateTime.Now.ToString();
                    vm.CreatedBy = identity.Name;
                    //vm.ActiveStatus = "Y";
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    result = _repo.InsertToBankInformation(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    vm.LastModifiedBy = identity.Name;
                    result = _repo.UpdateBankInformation(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = result[2] });
                }
                else
                {
                    return View("Create", vm);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                // FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("BankInformationController", "CreateEdit", ex.ToString());
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
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
            BankInformationVM vm = new BankInformationVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BankInformationRepo(identity, Session);

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
            BankInformationVM vm = new BankInformationVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.LastModifiedBy = identity.Name;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Print()
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {

                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }
            return PartialView("_printInformation");
        }

        [Authorize]
        public ActionResult ReportView(string ItemId)
        {
            try
            {
                if (ItemId == null)
                {
                    ItemId = "";
                }
                var ReportResult = new DataSet();
                ReportDSRepo reportDsdal = new ReportDSRepo(identity, Session);
                ReportResult = reportDsdal.BankNew(ItemId);
                if (ReportResult.Tables.Count <= 0)
                {
                    //some codes here
                }
                ReportResult.Tables[0].TableName = "DsBank";
                RptBankList objrpt = new RptBankList();
                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + identity.UserId + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Vendor Group Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + identity.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Program.Address1'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'Program.Address2'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'Program.Address3'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                var gr = new GenericReport<RptBankList>();
                var rpt = gr.RenderReportAsPDF(objrpt);
                objrpt.Close();
                return rpt;
            }
            catch (Exception e)
            {

                // string msg = e.Message.Split('\r').FirstOrDefault();
                //  Session["result"] = "Fail~" + msg;
                throw e;
            }

        }

        public ActionResult GetItemInfo(string ItemId)
        {
            var vm = new BankInformationRepo(identity, Session).SelectAll(Convert.ToInt32(ItemId)).FirstOrDefault();
            return Json(vm.BranchName, JsonRequestBehavior.AllowGet);
        }
    }
}
