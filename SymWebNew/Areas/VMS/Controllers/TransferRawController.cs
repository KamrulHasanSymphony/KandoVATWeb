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
    public class TransferRawController : Controller
    {
        //
        // GET: /Vms/FinancialTransaction/

        TransferRawRepo _repo = null;
        ShampanIdentity identity = null;

        public TransferRawController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new TransferRawRepo(identity);

            }
            catch
            {

            }
        }




        #region Index and _index
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string tType, string dtFrom = "", string dtTo = "")
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


            if (string.IsNullOrWhiteSpace(tType))
            {
                tType = "Other";
            }

            Session["dtFrom"] = dtFrom;
            Session["dtTo"] = dtTo;

            TransferRawMasterVM vm = new TransferRawMasterVM();
            vm.Details = new List<TransferRawDetailVM>();
            vm.TransactionType = tType;

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamVM param, string tType)
        {
            _repo = new TransferRawRepo(identity, Session);
            List<TransferRawMasterVM> getAllData = new List<TransferRawMasterVM>();
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
            #region SeachParameters
            string searchedBranchId = "0";
            string dtFrom = DateTime.Now.ToString("yyyyMMdd");
            string dtTo = DateTime.Now.ToString("yyyyMMdd");
            if (!string.IsNullOrWhiteSpace(Session["Branch"] as string))
            {
                searchedBranchId = Session["Branch"].ToString();
            }
            if (!string.IsNullOrWhiteSpace(Session["dtFrom"] as string))
            {
                dtFrom = Convert.ToDateTime(Session["dtFrom"]).ToString("yyyyMMdd");
            }
            if (!string.IsNullOrWhiteSpace(Session["dtTo"] as string))
            {
                dtTo = Convert.ToDateTime(Session["dtTo"]).ToString("yyyyMMdd");
            }
            string BranchId = "";
            if (searchedBranchId == "-1")
            {
                BranchId = "";
            }
            else if (BranchId != searchedBranchId && searchedBranchId != "0")
            {
                BranchId = searchedBranchId;
            }
            else
            {
                BranchId = identity.BranchId.ToString();
            }

            #endregion SeachParameters


            if (!identity.IsAdmin)
            {
                string[] conditionFields = { "tr.TransferDateTime>", "tr.TransferDateTime<", "tr.TransactionType" };
                string[] conditionValues = { dtFrom, dtTo, tType };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            else
            {
                string[] conditionFields = { "tr.TransferDateTime>", "tr.TransferDateTime<", "tr.TransactionType" };
                string[] conditionValues = { dtFrom, dtTo, tType };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            #endregion
            #region Search and Filter Data
            IEnumerable<TransferRawMasterVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Id
                //TransferId
                //RawCode
                //RawName
                //TransferDateTime
                //TransferedQty
                //TransferedAmt
                //UOM

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.TransferId.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.RawCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.RawName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.TransferDateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.TransferedQty.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.TransferedAmt.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.UOM.ToString().ToLower().Contains(param.sSearch.ToLower())

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
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<TransferRawMasterVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.TransferId :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.RawCode :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.RawName :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.TransferDateTime.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.TransferedQty.ToString() :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.TransferedAmt.ToString() :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.UOM.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id+"~"+ c.Post
                , c.TransferId
                , c.RawCode
                , c.RawName
                , c.TransferDateTime.ToString()             
                , c.TransferedQty.ToString()               
                , c.TransferedAmt.ToString()
                , c.UOM
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
        public ActionResult BlankItem(TransferRawDetailVM vm)
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
            TransferRawMasterVM vm = new TransferRawMasterVM();

            List<TransferRawDetailVM> trDetails = new List<TransferRawDetailVM>();
            vm.Details = trDetails;
            vm.Operation = "add";
            vm.TransactionType = tType;
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(TransferRawMasterVM vm)
        {
            _repo = new TransferRawRepo(identity, Session);
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
                    result = _repo.TransferRawInsert(vm, vm.Details);
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
                    var check = vm.Id;
                    result = _repo.TransferRawUpdate(vm, vm.Details);
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
                // Session["result"] = "Fail~Data not Successfully";
                return View("Create", vm);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            _repo = new TransferRawRepo(identity, Session);
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
            TransferRawMasterVM vm = new TransferRawMasterVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

            List<TransferRawDetailVM> DetailVMs = new List<TransferRawDetailVM>();

            DetailVMs = _repo.SelectTransferDetail(vm.TransferId);

            vm.Details = DetailVMs;
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Posted(string id)
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
            IssueMasterVM vm = new IssueMasterVM();
            //vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            List<IssueDetailVM> IssueDetailVMs = new List<IssueDetailVM>();

            //IssueDetailVMs = _detailRepo.SelectByMasterId(Convert.ToInt32(id));

            vm.Details = IssueDetailVMs;

            vm.Operation = "posted";
            return View("Posted", vm);
        }

        public JsonResult SelectProductDetails(string productCode, string IssueDate)
        {
            var _repo = new ProductRepo(identity, Session);
            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };

            //DataTable dt = _repo.SearchProductDT("", productName, "", "", "", "", "", "", "", "", "", "", "", "");
            var product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();

            var code = product.ProductCode;
            var uom = product.UOM;
            var hscode = product.HSCodeNo;
            var costPrice = "";
            var stock = "";
            var name = product.ProductName;
            var itemNo = product.ItemNo;

            #region businessLogic

            string UserId = identity.UserId;

            var issueDatetime = DateTime.Parse(IssueDate).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            DataTable priceData = _repo.AvgPriceNew(code, issueDatetime, null, null, false, true, true, true, UserId);
            decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
            decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

            if (quan > 0)
            {
                costPrice = (amount / quan).ToString();
            }
            else
            {
                costPrice = "0";
            }
            #endregion businessLogic
            stock = quan.ToString();
            string result = code + "~" + uom + "~" + hscode + "~" + costPrice + "~" + stock + "~" + name + "~" + itemNo;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

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
            var targetId = _repo.GetTargetId("TransferRawHeaders", "Id", id, btn);
            return RedirectToAction("Edit", new { id = targetId });
        }
    }
}
