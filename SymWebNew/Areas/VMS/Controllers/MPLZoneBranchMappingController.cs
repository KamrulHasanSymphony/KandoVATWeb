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
    public class MPLZoneBranchMappingController : Controller
    {
        ShampanIdentity identity = null;

        MPLZoneBranchMappingRepo _repo = null;

        public MPLZoneBranchMappingController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new MPLZoneBranchMappingRepo(identity);
            }
            catch
            {

            }
        }
        

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
            _repo = new MPLZoneBranchMappingRepo(identity, Session);

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
          


            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<MPLZoneBranchMappingVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.ZoneName.ToLower().Contains(param.sSearch.Trim().ToLower())
                    || isSearchable2 && c.BranchName.ToLower().Contains(param.sSearch.Trim().ToLower())
                    
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<MPLZoneBranchMappingVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.ZoneName :
                sortColumnIndex == 2 && isSortable_2 ? c.BranchName :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id.ToString()
                  , c.ZoneName
                , c.BranchName
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
        public ActionResult GetZoneCodeWise(JQueryDataTableParamModel param,int ZoneID)
        {
            #region Search and Filter Data

            var getAllData = _repo.GetZoneCodeWise(ZoneID, null, null, null, null);

            IEnumerable<MPLZoneBranchMappingVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.BranchName.ToLower().Contains(param.sSearch.Trim().ToLower())
                    || isSearchable2 && c.ZoneName.ToLower().Contains(param.sSearch.Trim().ToLower())

                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<MPLZoneBranchMappingVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.BranchName :
                sortColumnIndex == 2 && isSortable_2 ? c.ZoneName :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id.ToString()
                , c.BranchName
                , c.BranchAddress
                ,c.Id.ToString() + "~" + c.MappingZoneId.ToString() + "~" + c.MappingBranchId.ToString()
                , c.ZoneName
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
            MPLZoneBranchMappingVM vm = new MPLZoneBranchMappingVM();
            vm.Operation = "add";
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(MPLZoneBranchMappingVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new MPLZoneBranchMappingRepo(identity, Session);

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

            int currentBranch = Convert.ToInt32(Session["BranchId"]);
            vm.ZoneId = vm.MappingZoneId;
            vm.BranchId = vm.MappingBranchId;
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    result = _repo.InsertToMPLZoneBranchMapping(vm);
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
                    result = _repo.UpdateMPLZoneBranchMapping(vm);
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
                FileLogger.Log("MPLBDBankInformationController", "CreateEdit", ex.ToString());
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
            MPLZoneBranchMappingVM vm = new MPLZoneBranchMappingVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new MPLZoneBranchMappingRepo(identity, Session);

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
            MPLZoneBranchMappingVM vm = new MPLZoneBranchMappingVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
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
            var vm = new MPLZoneBranchMappingRepo(identity, Session).SelectAll(Convert.ToInt32(ItemId)).FirstOrDefault();
            return Json(vm.ZoneName, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetZonePopUp(string operation, int MappingZoneId, int MappingBranchId, int Id)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    //
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }
            MPLZoneBranchMappingVM vm = new MPLZoneBranchMappingVM();
            vm.ModalOperation = operation.Trim();
            vm.MappingZoneId = MappingZoneId;
            vm.MappingBranchId = MappingBranchId;
            vm.Id = Id;
            return PartialView("_zoneMapping", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult SaveUpdate(MPLZoneBranchMappingVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new MPLZoneBranchMappingRepo(identity, Session);

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

            int currentBranch = Convert.ToInt32(Session["BranchId"]);

            string[] result = new string[6];
            try
            {
                if (vm.ModalOperation.ToLower() == "add")
                {
                    result = _repo.InsertToMPLZoneBranchMapping(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return Json(result);
                }
                else if (vm.ModalOperation.ToLower() == "update")
                {
                    result = _repo.UpdateMPLZoneBranchMapping(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return Json(result);
                }
                else
                {
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                FileLogger.Log("MPLBDBankInformationController", "SaveUpdate", ex.ToString());
                return Json(ex.Message.ToString());
            }
        }
    }
}
