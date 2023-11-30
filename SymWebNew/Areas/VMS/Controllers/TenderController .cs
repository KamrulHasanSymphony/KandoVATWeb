using CrystalDecisions.CrystalReports.Engine;
//using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.VMS;
//using SymRepository.Common;
using VATViewModel.DTOs;
//using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading.Tasks;
using System.Net.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class TenderController : Controller
    {
        //
        // GET: /Vms/FinancialTransaction/

        ShampanIdentity identity = null;

        TenderRepo _repo = null;

        public TenderController()
        {
            try
            {

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new TenderRepo(identity);

            }
            catch
            {

            }
        }

        //TenderRepo _repo = new TenderRepo();

        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        [Authorize(Roles = "Admin")]
        public ActionResult Menu()
        {
            return View();
        }

        #region Index and _index
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
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


            TenderMasterVM vm = new TenderMasterVM();
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamVM param)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TenderRepo(identity, Session);

            List<TenderMasterVM> getAllData = new List<TenderMasterVM>();
            #region Access Controll
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

            getAllData = _repo.SelectAll();
            #endregion
            #region Search and Filter Data
            IEnumerable<TenderMasterVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //TenderId
                //RefNo
                //GroupName
                //CustomerName
                //TenderDate
                //DeleveryDate

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.RefNo.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.GroupName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.CustomerName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.TenderDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.DeleveryDate.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<TenderMasterVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.RefNo :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.GroupName :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.CustomerName :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.TenderDate.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.DeleveryDate.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.TenderId+"~"+ c.Post
                , c.RefNo
                , c.GroupName
                , c.CustomerName
                , c.TenderDate.ToString()             
                , c.DeleveryDate.ToString()               
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


        #endregion

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(TenderDetailVM vm)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {

            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }
            return PartialView("_detail", vm);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create(string tType)
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
            TenderMasterVM vm = new TenderMasterVM();

            List<TenderDetailVM> IssueDetailVMs = new List<TenderDetailVM>();
            vm.Details = IssueDetailVMs;
            vm.Operation = "add";
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(TenderMasterVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TenderRepo(identity, Session);

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
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.CreatedBy = identity.Name;

                    result = _repo.TenderInsert(vm, vm.Details);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    result = _repo.TenderUpdate(vm, vm.Details);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return View("Create", vm);
                    }
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
                //  Session["result"] = "Fail~Data not Successfully";
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TenderRepo(identity, Session);
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
            TenderMasterVM vm = new TenderMasterVM();
            vm = _repo.SelectAll(id).FirstOrDefault();

            List<TenderDetailVM> TenderDetailVMs = new List<TenderDetailVM>();

            TenderDetailVMs = _repo.SelectAllDetails(vm.TenderId);

            vm.Details = TenderDetailVMs;
            vm.Operation = "update";
            return View("Create", vm);
        }

        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }

        public JsonResult SelectProductDetails(string productCode, string IssueDate)
        {
            var _repo = new ProductRepo(identity, Session);
            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };
            var product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
            return Json(product, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUomOption(string uomFrom)
        {
            var _repo = new UOMRepo(identity, Session);
            string[] conditionalFields = new string[] { "UOMFrom" };
            string[] conditionalValues = new string[] { uomFrom };
            var uoms = _repo.SelectAll(0, conditionalFields, conditionalValues);
            var html = "";
            html += "<option value=" + uomFrom + ">" + uomFrom + "</option>";
            foreach (var item in uoms)
            {
                html += "<option value=" + item.UOMTo + ">" + item.UOMTo + "</option>";
            }
            return Json(html, JsonRequestBehavior.AllowGet);
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

        public ActionResult Navigate(string id, string btn)
        {
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetId("IssueHeaders", "Id", id, btn);
            return RedirectToAction("Edit", new { id = targetId });
        }

    }
}
