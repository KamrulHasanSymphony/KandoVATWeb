using CrystalDecisions.CrystalReports.Engine;
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

namespace SymVATWebUI.Areas.vms.Controllers
{
    [ShampanAuthorize]
    public class VAT7Controller : Controller
    {
        ShampanIdentity identity = null;

        VAT7Repo _repo = null;

        public VAT7Controller()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new VAT7Repo(identity);

            }
            catch
            {

            }
        }
        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //VAT7Repo _repo = new VAT7Repo();
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
            _repo = new VAT7Repo(identity, Session);

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
            //Id
            //00     //VAT7No 
            //01     //FinishItemNo
            //02    //FinishItemName
            //03     //FinishUOM
            //04     //Vat7DateTime
            //05     //Post
            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<VAT7VM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.VAT7No.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.FinishItemName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.FinishUOM.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Vat7DateTime.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.Post.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.Post.ToLower().Contains(param.sSearch.ToLower())
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
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);


            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<VAT7VM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.VAT7No :
                sortColumnIndex == 2 && isSortable_2 ? c.FinishItemNo :
                sortColumnIndex == 3 && isSortable_3 ? c.FinishItemName :
                sortColumnIndex == 4 && isSortable_4 ? c.FinishUOM :
                sortColumnIndex == 5 && isSortable_5 ? c.Vat7DateTime :
                sortColumnIndex == 6 && isSortable_6 ? c.Post :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id
                , c.VAT7No
                , c.FinishItemNo
                , c.FinishItemName
                , c.FinishUOM
                , c.Vat7DateTime
                , c.Post
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
            VAT7VM vm = new VAT7VM();
            vm.Operation = "add";
            vm.Post = "N";
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(VAT7VM vm)
        {
            _repo = new VAT7Repo(identity, Session);
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
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    vm.VAT7No = "New";
                    var details = new List<VAT7VM>();
                    details.Add(vm);
                    result = _repo.Vat7Insert(vm, details);

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
                    var details = new List<VAT7VM>();
                    details.Add(vm);
                    result = _repo.Vat7update(vm, details);
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
                // Session["result"] = "Fail~Data Not Succeessfully!";
                // FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("VAT7Controller", "CreateEdit", ex.ToString());
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            _repo = new VAT7Repo(identity, Session);
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
            VAT7VM vm = new VAT7VM();
            vm = _repo.SelectAll(id).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }

        //public ActionResult Navigate(string id, string btn)
        //{
        //    var _repo = new SymRepository.VMS.CommonRepo();
        //    var targetId = _repo.GetTargetId("Currencies", "CurrencyId", id, btn);
        //    return RedirectToAction("Edit", new { id = targetId });
        //}

        public JsonResult GetUomOption(string uomFrom)
        {
            var _repo = new UOMRepo(identity, Session);
            string[] conditionalFields = new string[] { "UOMFrom" };
            string[] conditionalValues = new string[] { uomFrom };
            var uoms = _repo.SelectAll(0, conditionalFields, conditionalValues);
            var html = "";
            foreach (var item in uoms)
            {
                html += "<option value=" + item.UOMTo + ">" + item.UOMTo + "</option>";
            }
            return Json(html, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(VAT7VM vm)
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
            return PartialView("_detail", vm);
        }

        public JsonResult GetConvFactor(string uomFrom, string UomTo)
        {
            var _repo = new UOMRepo(identity, Session);
            string[] conditionalFields = new string[] { "UOMFrom", "UOMTo" };
            string[] conditionalValues = new string[] { uomFrom, UomTo };
            var uom = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();
            var uomFactor = uom.Convertion;
            return Json(uomFactor, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult ReportVAT7(string VAT7No)
        {
            try
            {
                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                DataSet ReportResult = new DataSet();

                objrpt = new RptVAT7Main();
                ReportResult = new ReportDSRepo(identity, Session).RptVAT7Report(VAT7No);
                ReportResult.Tables[0].TableName = "DsVAT7";
                objrpt.SetDataSource(ReportResult);

                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";

                var rpt = gr.RenderReportAsPDF(objrpt);
                objrpt.Close();
                return rpt;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
