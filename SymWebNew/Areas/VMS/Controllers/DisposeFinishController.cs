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
using System.Configuration;
using VATServer.Ordinary;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class DisposeFinishController : Controller
    {
        //
        // GET: /Vms/FinancialTransaction/

        DisposeFinishRepo _repo = null;
        ShampanIdentity identity = null;

        public DisposeFinishController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new DisposeFinishRepo(identity);
            }
            catch
            {

            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult HomeIndex()
        {
            return View();
        }

        #region Index and _index
        [Authorize(Roles = "Admin")]
        public ActionResult Index(DisposeFinishMasterVM paramVM, string transactionType)
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


            if (string.IsNullOrWhiteSpace(paramVM.TransactionType))
            {
                paramVM.TransactionType = "Other";
            }


            return View(paramVM);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamVM param, DisposeFinishMasterVM paramVM)
        {
            _repo = new DisposeFinishRepo(identity, Session);

            List<DisposeFinishMasterVM> getAllData = new List<DisposeFinishMasterVM>();
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

            string dtFrom = DateTime.Now.ToString("yyyyMMdd");
            string dtTo = DateTime.Now.AddDays(1).ToString("yyyyMMdd");

            if (!string.IsNullOrWhiteSpace(paramVM.DisposeDateTimeFrom))
            {
                dtFrom = Convert.ToDateTime(paramVM.DisposeDateTimeFrom).ToString("yyyyMMdd");
            }
            if (!string.IsNullOrWhiteSpace(paramVM.DisposeDateTimeTo))
            {

                dtTo = Convert.ToDateTime(paramVM.DisposeDateTimeTo).AddDays(1).ToString("yyyyMMdd");
            }

            string branchId = "";
            if (paramVM.BranchId == 0)
            {
                ////paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                branchId = "";
            }

            #endregion SeachParameters


            if (!identity.IsAdmin)
            {
                string[] conditionFields = { "df.TransactionDateTime>=", "df.TransactionDateTime<=", "df.Post", "df.BranchId", "df.TransactionType" };
                string[] conditionValues = { dtFrom, dtTo, paramVM.Post, branchId, paramVM.TransactionType };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            else
            {
                string[] conditionFields = { "df.TransactionDateTime>=", "df.TransactionDateTime<=", "df.Post", "df.BranchId", "df.TransactionType" };
                string[] conditionValues = { dtFrom, dtTo, paramVM.Post, branchId, paramVM.TransactionType };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }


            #endregion
            #region Search and Filter Data
            IEnumerable<DisposeFinishMasterVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Id
                //IssueNo
                //IssueDateTime
                //TotalVATAmount
                //TotalAmount
                //SerialNo
                //Post

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                //var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.DisposeNo.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.TransactionDateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.FinishItemNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Quantity.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                    //|| isSearchable6 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            //var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<DisposeFinishMasterVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.DisposeNo :
                //sortColumnIndex == 2 && isSortable_2 ? Ordinary.DateToString(c.TransactionDateTime) :
                                                           sortColumnIndex == 2 && isSortable_2 ? (c.TransactionDateTime) :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.FinishItemNo.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Quantity.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Post.ToString() :
                //sortColumnIndex == 6 && isSortable_6 ? c.Post.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id+"~"+ c.Post+"~"+ c.BranchId
                , c.DisposeNo
                , Convert.ToDateTime(c.TransactionDateTime).ToString("yyyy-MMM-dd")
                , c.FinishItemNo.ToString()
                , c.Quantity.ToString()                          
                , c.Post=="Y" ? "Posted" : "Not Posted"            
                ,c.TransactionType              
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
        public ActionResult BlankItem(DisposeFinishDetailVM vm)
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

            ProductRepo PRepo = new ProductRepo(identity, Session);

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
            DisposeFinishMasterVM vm = new DisposeFinishMasterVM();

            CommonRepo commonRepo = new CommonRepo(identity, Session);

            string DecimalPlace = commonRepo.settings("DecimalPlace", "FormNumeric");

            List<DisposeFinishDetailVM> IssueDetailVMs = new List<DisposeFinishDetailVM>();
            vm.Details = IssueDetailVMs;
            vm.Operation = "add";
            vm.TransactionType = tType;
            vm.DecimalPlace = Convert.ToInt32(DecimalPlace);

            if (vm.TransactionType == "DisposeTrading")
            {
                vm.ProductType = "Trading";
            }
            else
            {
                vm.ProductType = "Raw";
            }

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(DisposeFinishMasterVM vm)
        {
            _repo = new DisposeFinishRepo(identity, Session);
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
            vm.TransactionDateTime = Convert.ToDateTime(vm.TransactionDateTime).ToString("yyyy-MMM-dd HH:mm:ss");

            ResultVM rVM = new ResultVM();

            try
            {
                foreach (DisposeFinishDetailVM vmD in vm.Details)
                {
                    int i = 1;
                    vmD.DisposeLineNo = i.ToString();
                    vmD.TransactionDateTime = vm.TransactionDateTime;
                    vmD.TransactionType = vm.TransactionType;
                    i++;
                }

                if (vm.Operation.ToLower() == "add")
                {
                    if (vm.IsSaleableChecked == true)
                    {
                        vm.IsSaleable = "Y";
                    }
                    else
                    {
                        vm.IsSaleable = "N";
                    }
                    vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.CreatedBy = identity.Name;
                    vm.Post = "N";
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);


                    //foreach (DisposeFinishDetailVM vmD in vm.Details)
                    //{
                    //    int i = 1;
                    //    vmD.DisposeLineNo = i.ToString();
                    //    vmD.TransactionDateTime = vm.TransactionDateTime;
                    //    vmD.TransactionType = vm.TransactionType;

                    //    i++;
                    //}

                    rVM = _repo.DisposeFinishInsert(vm);

                    //rVM.Status = result[0];
                    //rVM.Message = result[1];

                    if (rVM.Status == "Success")
                    {
                        Session["result"] = rVM.Status + "~" + rVM.Message;
                        //return RedirectToAction("Edit", new { id = result[4], TransactionType = vm.transactionType });
                        return RedirectToAction("Edit", new { DisposeNo = rVM.InvoiceNo });
                    }
                    else
                    {
                        Session["result"] = rVM.Status + "~" + rVM.Message;
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    if (vm.IsSaleableChecked == true)
                    {
                        vm.IsSaleable = "Y";
                    }
                    else
                    {
                        vm.IsSaleable = "N";
                    }

                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                    var check = vm.Id;

                    //foreach (DisposeFinishDetailVM vmD in vm.Details)
                    //{
                    //    int i = 1;
                    //    vmD.DisposeLineNo = i.ToString();
                    //    vmD.TransactionDateTime = vm.TransactionDateTime;
                    //    vmD.TransactionType = vm.TransactionType;
                    //    i++;
                    //}

                    rVM = _repo.DisposeFinishUpdate(vm);
                    if (rVM.Status == "Success")
                    {
                        Session["result"] = rVM.Status + "~" + rVM.Message;
                        //return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.transactionType });
                        return RedirectToAction("Edit", new { id = vm.Id });
                    }
                    else
                    {
                        Session["result"] = rVM.Status + "~" + rVM.Message;
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
        public ActionResult Edit(string id, string TransactionType, string DisposeNo)
        {
            _repo = new DisposeFinishRepo(identity, Session);
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



            //if (TransactionType == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            DisposeFinishMasterVM vm = new DisposeFinishMasterVM();
            //vm.TransactionDateTime = Convert.ToDateTime(vm.TransactionDateTime).ToString("yyyy-MMM-dd HH:mm:ss");

            string[] conditionFields = new string[] { "df.DisposeNo", "df.Id" };
            string[] conditionValues = new string[] { DisposeNo, id };

            vm = _repo.SelectAll(Convert.ToInt32(id), conditionFields, conditionValues).FirstOrDefault();
            //////vm = _repo.SelectAllWeb(Convert.ToInt32(id), conditionFields, conditionValues).FirstOrDefault();

            string[] cFields = { "Pr.ItemNo" };
            string[] cValues = { vm.FinishItemNo };
            ProductVM varProductVM = new ProductVM();
            varProductVM = new ProductRepo(identity, Session).SelectAll("0", cFields, cValues).FirstOrDefault();
            vm.FinishItemName = varProductVM.ProductName;


            if (vm == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<DisposeFinishDetailVM> DisposeFinishDetailVMs = new List<DisposeFinishDetailVM>();

            DisposeFinishDetailVMs = _repo.SelectDetail(new[] { "dfd.DisposeNo" }, new[] { vm.DisposeNo });
            vm.TransactionDateTime = Convert.ToDateTime(vm.TransactionDateTime).ToString("yyyy-MMM-dd");

            vm.Details = DisposeFinishDetailVMs;
            //vm.TransactionType = "Other";
            vm.Operation = "update";
            vm.ProductType = "Raw";

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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult VoucherReportView(string id)
        {
            try
            {
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                IssueDetailVM vm = new IssueDetailVM();
                //IssueDetailRepo _detailRepo = new IssueDetailRepo();

                string[] ConditionFields = { };
                string[] ConditionValues = { };
                //dt = _detailRepo.VoucherReport(vm, ConditionFields, ConditionValues);

                ds.Tables.Add(dt);
                ds.Tables[0].TableName = "Issue";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Vms\IssueVoucher.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);

                //string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\" + Ordinary.ReportHeaderLogo;
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'My company'";
                //doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            _repo = new DisposeFinishRepo(identity, Session);
            string[] a = ids.Split('~');
            var id = a[0];
            ResultVM rVM = new ResultVM();
            DisposeFinishMasterVM vm = new DisposeFinishMasterVM();
            vm = _repo.SelectAllWeb(Convert.ToInt32(id)).FirstOrDefault();
            //List<DisposeFinishDetailVM> DisposeDetailVMs = new List<DisposeFinishDetailVM>();
            //DisposeDetailVMs = _repo.SelectDetail(vm.DisposeNo);
            //vm.Details = DisposeDetailVMs;
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";
            ParameterVM pVm = new ParameterVM();
            pVm.InvoiceNo = vm.DisposeNo;

            rVM = _repo.DisposeFinishPost(pVm);

            result[1] = rVM.Message;

            return Json(result[1], JsonRequestBehavior.AllowGet);
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

            ////////var code = product.ProductCode;
            ////////var uom = product.UOM;
            ////////var hscode = product.HSCodeNo;
            ////////var costPrice = "";
            ////////var stock = "";
            ////////var name = product.ProductName;
            ////////var itemNo = product.ItemNo;
            if (product == null)
            {
                product = _repo.SelectAll(productCode).FirstOrDefault();
            }

            string UserId = identity.UserId;

            //#region businessLogic
            //OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);
            //var issueDatetime = DateTime.Parse(IssueDate).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            //DataTable priceData = _repo.AvgPriceNew(product.ItemNo, issueDatetime, null, null, true, true, true, true, UserId);
            //decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
            //decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

            //if (quan > 0)
            //{
            //    product.CostPrice = (amount / quan);
            //}
            //else
            //{
            //    product.CostPrice = 0;
            //}
            //#endregion businessLogic
            //product.Stock = quan;

            #region BOM Set

            //DataTable dt = new DataTable();

            //dt = _repo.SelectBOMRaw(product.ProductCode, Convert.ToDateTime(issueDatetime).ToString("yyyy-MMM-dd"));

            //int BOMId = 0;



            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    DataRow dr = dt.Rows[0];
            //    string tempBOMId = dr["BOMId"].ToString();
            //    if (!string.IsNullOrWhiteSpace(tempBOMId))
            //    {
            //        BOMId = Convert.ToInt32(tempBOMId);
            //    }
            //}

            //txtBOMId.Text = BOMId.ToString();
            #endregion

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

        public ActionResult Navigate(string id, string btn, string ttype)
        {
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetIdForTtype("IssueHeaders", "Id", id, btn, ttype);
            return RedirectToAction("Edit", new { id = targetId, TransactionType = ttype });
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetProductPopUp(PopUpViewModel vm)
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
            vm.ProductCode = null;
            vm.ProductType = "Finish";
            vm.TargetId = null;
            return PartialView("_products", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetBomPopUp(string BOMName)
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
            BOMNBRVM vm = new BOMNBRVM();
            //vm.CustomerName = CustomerName;
            return PartialView("_boms", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredBoms(BOMNBRVM vm)
        {
            _repo = new DisposeFinishRepo(identity, Session);

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
            var activeSatus = "";
            if (string.IsNullOrEmpty(vm.ActiveStatus))
            {
                activeSatus = "Y";
            }
            else
            {
                activeSatus = vm.ActiveStatus == "true" ? "Y" : "N";

            }
            string[] conditionFields;
            string[] conditionValues;
            conditionFields = new string[] { "bm.FinishItemNo" };
            conditionValues = new string[] { vm.FinishItemNo };
            //if (vm.SearchField != null)
            //{
            //    conditionalFields = new string[] { "Pr." + vm.SearchField + " like", "Pr.CategoryID", "Pc.IsRaw", "Pr.ActiveStatus" };
            //    conditionalValues = new string[] { vm.SearchValue, vm.CategoryID, vm.ProductType, activeSatus };
            //}
            //else
            //{
            //    conditionalFields = new string[] { "Pr.CategoryID", "Pc.IsRaw", "Pr.ActiveStatus" };
            //    conditionalValues = new string[] { vm.CategoryID, vm.ProductType, activeSatus };
            //}
            //var list = _repo.SelectAll("0", conditionalFields, conditionalValues);
            var list = _repo.SelectAllBOM(0, conditionFields, conditionValues);

            return PartialView("_filteredBoms", list);
        }

        public ActionResult PopUpPurchaseDetails(DisposeFinishDetailVM vm)
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

            return PartialView("_purchaseDetails", vm);
        }

        public ActionResult PopUpPreviousPurchase(DisposeFinishDetailVM vm)
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

            return PartialView("_PopUpPreviousPurchase", vm);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredPurchase(PurchaseDetailVM vm)
        {
            _repo = new DisposeFinishRepo(identity, Session);

            PurchaseRepo _Repo = null;
            _Repo = new PurchaseRepo(identity, Session);

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
            //var activeSatus = "";
            //if (string.IsNullOrEmpty(vm.ActiveStatus))
            //{
            //    activeSatus = "Y";
            //}
            //else
            //{
            //    activeSatus = vm.ActiveStatus == "true" ? "Y" : "N";

            //}
            string[] conditionFields;
            string[] conditionValues;

            conditionFields = new string[] { "pd.ItemNo" };
            conditionValues = new string[] { vm.ItemNo };

            //if (vm.SearchField != null)
            //{
            //    conditionalFields = new string[] { "Pr." + vm.SearchField + " like", "Pr.CategoryID", "Pc.IsRaw", "Pr.ActiveStatus" };
            //    conditionalValues = new string[] { vm.SearchValue, vm.CategoryID, vm.ProductType, activeSatus };
            //}
            //else
            //{
            //    conditionalFields = new string[] { "Pr.CategoryID", "Pc.IsRaw", "Pr.ActiveStatus" };
            //    conditionalValues = new string[] { vm.CategoryID, vm.ProductType, activeSatus };
            //}
            //var list = _repo.SelectAll("0", conditionalFields, conditionalValues);
            var list = _Repo.SelectPurchaseDetail(null, conditionFields, conditionValues);

            return PartialView("_filteredPurchase", list);
        }
    }
}
