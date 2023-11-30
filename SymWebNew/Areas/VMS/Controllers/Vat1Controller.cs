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
using VATServer.Ordinary;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class Vat1Controller : Controller
    {

        ShampanIdentity identity = null;

        BOMRepo _repo = null;

        public Vat1Controller()
        {
            try
            {

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new BOMRepo(identity);
            }
            catch
            {

            }

        }
        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //BOMRepo _repo = new BOMRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index(BOMNBRVM paramVM)
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
            //Session["dtFrom"] = dtFrom;
            //Session["dtTo"] = dtTo;

            //BOMNBRVM vm = new BOMNBRVM();
            //vm.TransactionType = tType;

            return View(paramVM);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamVM param, BOMNBRVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BOMRepo(identity, Session);

            List<BOMNBRVM> getAllData = new List<BOMNBRVM>();
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
            //string searchedBranchId = "0";
            string dtFrom = DateTime.Now.ToString("yyyyMMdd");
            string dtTo = DateTime.Now.ToString("yyyyMMdd");
            ////if (!string.IsNullOrWhiteSpace(Session["Branch"] as string))
            ////{
            ////    searchedBranchId = Session["Branch"].ToString();
            ////}

            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "All";
            }


            if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeFrom))
            {
                dtFrom = paramVM.InvoiceDateTimeFrom;
            }

            if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeTo))
            {
                dtTo = paramVM.InvoiceDateTimeTo;
            }


            //if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeTo))
            //{
            //    dtTo = paramVM.InvoiceDateTimeTo;
            //}
            ////string BranchId = "";
            ////if (searchedBranchId == "-1")
            ////{
            ////    BranchId = "";
            ////}
            ////else if (BranchId != searchedBranchId && searchedBranchId != "0")
            ////{
            ////    BranchId = searchedBranchId;
            ////}
            ////else
            ////{
            ////    BranchId = identity.BranchId.ToString();
            ////}

            #endregion SeachParameters

            string searchedBranchId = Session["BranchId"].ToString();

            if (!identity.IsAdmin)
            {
                ////string[] conditionFields = { "p.ProductName like", "p.ProductCode like", "bm.EffectDate>", "bm.EffectDate<", "bm.VATName", "bm.Post", "bm.CustomerID" };
                string[] conditionFields = { "p.ProductName like", "p.ProductCode like", "bm.EffectDate>=", "bm.EffectDate<=", "bm.VATName", "bm.Post", "bm.CustomerID", "bm.BranchId", "SelectTop" };
                string[] conditionValues = { paramVM.FinishItemName, paramVM.FinishItemCode, dtFrom, dtTo, paramVM.VATName, paramVM.Post, paramVM.CustomerID, searchedBranchId, paramVM.SelectTop };
                getAllData = _repo.SelectAll(null, conditionFields, conditionValues);
            }
            else
            {
                ////////string[] conditionFields = { "p.ProductName like", "p.ProductCode like", "bm.EffectDate>", "bm.EffectDate<", "bm.VATName", "bm.Post", "bm.CustomerID"};
                string[] conditionFields = { "p.ProductName like", "p.ProductCode like", "bm.EffectDate>=", "bm.EffectDate<=", "bm.VATName", "bm.Post", "bm.CustomerID", "bm.BranchId", "SelectTop" };
                string[] conditionValues = { paramVM.FinishItemName, paramVM.FinishItemCode, dtFrom, dtTo, paramVM.VATName, paramVM.Post, paramVM.CustomerID, searchedBranchId, paramVM.SelectTop };
                getAllData = _repo.SelectAll(null, conditionFields, conditionValues);
            }
            #endregion
            #region Search and Filter Data
            IEnumerable<BOMNBRVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //BomId
                //CustomerName
                //ProductName
                //VatName
                //SalePrice
                //Uom
                //Post

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.CustomerName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.FinishItemName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.VATName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.WholeSalePrice.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.UOM.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.EffectDate.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<BOMNBRVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.CustomerName :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.FinishItemName.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.VATName.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.WholeSalePrice.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.UOM.ToString() :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Post.ToString() :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.EffectDate.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.BOMId+"~"+ c.Post.ToString() 
                , c.CustomerName.ToString() 
                , c.FinishItemName.ToString()
                , c.VATName.ToString()
                , c.WholeSalePrice.ToString()             
                , c.UOM.ToString()               
                , c.Post.ToString() =="Y" ? "Posted" : "Not Posted"
                , c.EffectDate.ToString()               

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

                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }
            BOMNBRVM vm = new BOMNBRVM();

            List<BOMItemVM> items = new List<BOMItemVM>();
            List<BOMOHVM> overheads = new List<BOMOHVM>();
            vm.Items = items;
            vm.Overheads = overheads;
            vm.VATName = "VAT 4.3";
            vm.Operation = "add";
            return View(vm);
        }

        #region Get Data

        [Authorize(Roles = "Admin")]
        public JsonResult GetProductDetails(string productCode, string purchaseNo, string effectDate)
        {

            var _repo = new ProductRepo(identity, Session);
            if (string.IsNullOrWhiteSpace(purchaseNo))
            {
                purchaseNo = "";
            }
            if (string.IsNullOrWhiteSpace(effectDate))
            {
                effectDate = DateTime.Now.ToString();
            }
            effectDate = Convert.ToDateTime(effectDate).ToString("yyyy-MM-dd");
            ProductVM product = _repo.GetProductWithCostPrice(productCode, purchaseNo, effectDate);
            return Json(product, JsonRequestBehavior.AllowGet);
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

        [Authorize]
        [HttpGet]
        public ActionResult GetProductPurchasePopUp(string ProductCode)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                { }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }

            string[] cfields = { "p.ProductCode" };
            string[] cVals = { ProductCode };
            PurchaseRepo purRepo = new PurchaseRepo(identity, Session);
            var detailVms = purRepo.SelectPurchaseDetail(null, cfields, cVals);
            return PartialView("_productPurchase", detailVms);
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

        [Authorize(Roles = "Admin")]
        public JsonResult GetOverheadDetails(string productCode)
        {
            var _repo = new ProductRepo(identity, Session);
            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };

            var product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
            var code = product.ProductCode;
            var rebatePercent = product.RebatePercent;
            var cost = product.CostPrice;
            var itemNo = product.ItemNo;
            var name = product.ProductName;
            var uom = product.UOM;

            string result = code + "~" + rebatePercent + "~" + cost + "~" + itemNo + "~" + name + "~" + uom;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(BOMItemVM vm)
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
            if (vm.RawItemType == "" || vm.RawItemType == null)
            {
                var _repo = new ProductCategoryRepo(identity, Session);
                var pCategory = _repo.SelectAll(Convert.ToInt32(vm.CategoryId)).FirstOrDefault();
                vm.RawItemType = pCategory.IsRaw;
            }
            var repo = new ProductRepo(identity, Session);
            DataTable dt = repo.GetLIFOPurchaseInformation(vm.RawItemNo, vm.EffectDate, vm.PInvoiceNo);
            if (dt.Rows.Count > 0)
            {
                var invoiceNo = dt.Rows[0]["PurchaseInvoiceNo"].ToString();
                vm.PInvoiceNo = invoiceNo;
                vm.TransactionType = new ProductRepo(identity, Session).GetTransactionType(vm.RawItemNo, vm.EffectDate);
            }
            else
            {
                vm.PInvoiceNo = "0";
                vm.TransactionType = "0";
            }
            vm.PBOMId = "0";

            return PartialView("_items", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlankOverhead(BOMOHVM vm)
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

            return PartialView("~/Areas/VMS/Views/Vat1/_overheads.cshtml", vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(BOMNBRVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BOMRepo(identity, Session);

            ModelState.Clear();

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
            vm.BranchId = Convert.ToInt32(Session["BranchId"]);
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    vm.CreatedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    vm.LastModifiedBy = identity.Name;
                    result = _repo.BOMPreInsert(vm);
                    //////result = _repo.BOMInsert(vm.Items, vm.Overheads, vm);
                    Session["result"] = result[0] + "~" + result[1];

                    if (result[0] == "Success")
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
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    //List<BOMItemVM> Items = new List<BOMItemVM>();
                    //List<BOMOHVM> Trackings = new List<BOMOHVM>();

                    result = _repo.BOMPreInsert(vm);
                    //result = _repo.PurchaseUpdate(vm, vm.Details, Duties, Trackings);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1].Split('\r').FirstOrDefault();
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
            vm = _repo.SelectAll(id).FirstOrDefault();
            vm.EffectDate = Convert.ToDateTime(vm.EffectDate).ToString("yyyy-MMM-dd");
            vm.FirstSupplyDate = Convert.ToDateTime(vm.EffectDate).ToString("yyyy-MMM-dd");

            List<BOMItemVM> Items = new List<BOMItemVM>();
            List<BOMOHVM> Overheads = new List<BOMOHVM>();

            Items = _repo.SelectAllItems(id);
            Overheads = _repo.SelectAllOverheads(id);
            foreach (var head in Overheads)
            {
                if (head.HeadName == "Margin")
                {
                    vm.Margin = head.HeadAmount;
                    Overheads.Remove(head);
                    break;
                }
            }
            vm.Items = Items;
            vm.Overheads = Overheads;
            vm.Operation = "update";
            return View("Create", vm);
        }

        public ActionResult FixedTable()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredBoms(BOMNBRVM vm)
        {
            _repo = new BOMRepo(identity, Session);
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
            vm.Post = vm.Post ?? "";
            vm.EffectDate = vm.EffectDate ?? "";
            string[] conditionalFields = new string[] { "bm.VATName", "bm.EffectDate", "bm.Post", "p.ProductName like" };
            string[] conditionalValues = new string[] { vm.VATName, vm.EffectDate, vm.Post, vm.FinishItemName };
            var list = _repo.SelectAll("0", conditionalFields, conditionalValues, null, null, vm);

            return PartialView("_filteredBoms", list);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetBOMPopUp(string targetId)
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
            PopUpViewModel vm = new PopUpViewModel();
            vm.TargetId = targetId;
            return PartialView("_boms", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            _repo = new BOMRepo(identity, Session);
            string[] a = ids.Split('~');
            var id = a[0];
            BOMNBRVM vm = new BOMNBRVM();
            vm = _repo.SelectAll(id).FirstOrDefault();
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";
            result = _repo.BOMPost(vm);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BOMRepo(identity, Session);

            string[] a = ids.Split('~');
            var id = a[0];
            BOMNBRVM vm = new BOMNBRVM();
            vm = _repo.SelectAll(id).FirstOrDefault();
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";
            result = _repo.DeleteBOM(vm.ItemNo, vm.VATName, vm.EffectDate, null, null, vm.CustomerID);
            var retresult = result[0] + "~" + result[1];
            return Json(retresult, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult ExportExcell(BOMNBRVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BOMRepo(identity, Session);

            ResultVM rVM = new ResultVM();

            List<BOMNBRVM> getAllData = new List<BOMNBRVM>();
            var BOMIdList = new List<string>();

            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "All";
            }

            try
            {

                if (paramVM != null && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    paramVM.IDs = paramVM.IDs.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

                    if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                    {
                        rVM.Message = "No Data to Export";
                        return Json(rVM, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    rVM.Message = "No Data to Export";
                    return Json(rVM, JsonRequestBehavior.AllowGet);
                }


                DataTable dt = _repo.GetBOMExcelData(paramVM.IDs);

                dt.Columns.Remove("Id");
                dt.Columns.Remove("BOMId");
                dt.Columns.Remove("RItemNo");
                dt.Columns.Remove("Type");

                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }


                var dataSet = new DataSet();
                dataSet.Tables.Add(dt);

                var sheetNames = new[] { "Product" };

                //if (details.Rows.Count > 0)
                //{
                //    dataSet.Tables.Add(details);
                //    sheetNames = new[] { "Products", "Details" };
                //}

                var vm = OrdinaryVATDesktop.DownloadExcelMultiple(dataSet, "Product", sheetNames);
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + vm.FileName + ".xlsx");
                    vm.varExcelPackage.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }
            catch (Exception)
            {


            }
            finally { }
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult CompareBOMExcel(BOMNBRVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BOMRepo(identity, Session);

            ResultVM rVM = new ResultVM();

            List<BOMNBRVM> bomList = new List<BOMNBRVM>();



            try
            {

                if (paramVM != null && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    paramVM.IDs = paramVM.IDs.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

                    if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                    {
                        rVM.Message = "No Data to Export";
                        return Json(rVM, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    rVM.Message = "No Data to Export";
                    return Json(rVM, JsonRequestBehavior.AllowGet);
                }
                BOMNBRVM vm = _repo.SelectAll(paramVM.IDs[0]).FirstOrDefault();

                bomList.Add(new BOMNBRVM()
                {
                    BOMId = vm.BOMId,
                    FinishItemCode = vm.FinishItemCode,
                    EffectDate = vm.EffectDate
                });

                vm = _repo.SelectPreviousBOM(vm).FirstOrDefault();

                if (vm == null)
                {
                    rVM.Message = "Can't find Previous BOM for Compare!";
                    return Json(rVM, JsonRequestBehavior.AllowGet);
                }

                bomList.Add(new BOMNBRVM()
                {
                    BOMId = vm.BOMId,
                    FinishItemCode = vm.FinishItemCode,
                    EffectDate = vm.EffectDate
                });
                DataTable dtResult = _repo.GetCompareData(bomList.Select(x => x.BOMId).ToList(), paramVM.ExportOverhead);
                ReportDSRepo report = new ReportDSRepo();
                DataSet dsResult = report.ComapnyProfile("");
                string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                string fileDirectory = pathRoot + "//Excel Files";
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                fileDirectory += "\\" + "BOM_Compare" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
                //FileStream fileStream = File.Create(fileDirectory);

                ExcelPackage excel = new ExcelPackage();

                ExcelWorksheet ws = excel.Workbook.Worksheets.Add("BOMs");

                decimal PreviousCostTotala = 0;
                decimal CurrentCostTotala = 0;
                decimal value = 0;
                decimal Result = 0;

                string finishCode = dtResult.Rows[0]["FinishCode"].ToString();
                string finishName = dtResult.Rows[0]["FinishName"].ToString();
                string finishUOM = dtResult.Rows[0]["FinishUOM"].ToString();

                string PreviousEffectDate = dtResult.Rows[0]["FirstEffectDate"].ToString();
                string CurrentEffectDate = dtResult.Rows[0]["SecondEffectDate"].ToString();
                string InctiveDate = Convert.ToDateTime(dtResult.Rows[0]["SecondEffectDate"]).AddDays(-1).ToString();

                dtResult.Columns.Remove("FinishCode");
                dtResult.Columns.Remove("FinishName");
                dtResult.Columns.Remove("FirstEffectDate");
                dtResult.Columns.Remove("SecondEffectDate");
                dtResult.Columns.Remove("FinishUOM");


                ws.Cells[1, 1, 1, dtResult.Columns.Count].Merge = true;
                ws.Cells[1, 1, 1, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, 1, 1, dtResult.Columns.Count].Style.Font.Size = 14;
                //ws.Cells[1, 1, 1, dtResult.Columns.Count].Style.Font.Bold = true;

                ws.Cells[2, 1, 2, dtResult.Columns.Count].Merge = true;
                ws.Cells[2, 1, 2, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[2, 1, 2, dtResult.Columns.Count].Style.Font.Size = 14;
                //ws.Cells[2, 1, 2, dtResult.Columns.Count].Style.Font.Bold = true;

                ws.Cells[3, 1, 3, dtResult.Columns.Count].Merge = true;
                ws.Cells[3, 1, 3, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[3, 1, 3, dtResult.Columns.Count].Style.Font.Size = 14;
                //ws.Cells[3, 1, 3, dtResult.Columns.Count].Style.Font.Bold = true;

                ws.Cells[4, 1, 4, dtResult.Columns.Count].Merge = true;
                ws.Cells[4, 1, 4, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[4, 1, 4, dtResult.Columns.Count].Style.Font.Size = 14;
                //ws.Cells[4, 1, 4, dtResult.Columns.Count].Style.Font.Bold = true;

                ws.Cells[5, 1, 5, dtResult.Columns.Count].Merge = true;
                ws.Cells[5, 1, 5, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[5, 1, 5, dtResult.Columns.Count].Style.Font.Size = 14;
                //ws.Cells[5, 1, 5, dtResult.Columns.Count].Style.Font.Bold = true;

                ws.Cells[7, 3, 7, 6].Merge = true;
                ws.Cells[7, 3, 7, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[7, 3, 7, 6].Style.Font.Size = 14;

                ws.Cells[7, 7, 7, 10].Merge = true;
                ws.Cells[7, 7, 7, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[7, 7, 7, 10].Style.Font.Size = 14;

                ws.Cells[7, 11, 7, 14].Merge = true;
                ws.Cells[7, 11, 7, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[7, 11, 7, 14].Style.Font.Size = 14;


                ws.Cells[1, 1].LoadFromText("Company Name: " + dsResult.Tables[0].Rows[0]["CompanyLegalName"]);
                ws.Cells[2, 1].LoadFromText("Address: " + dsResult.Tables[0].Rows[0]["Address1"]);

                ws.Cells[3, 1].LoadFromText("Product Name: " + finishName + " (" + finishCode + ")");
                ws.Cells[4, 1].LoadFromText("Previous Effect Date: " + ToDateString(PreviousEffectDate, "dd-MMM-yyyy") + "      Inctive Date: " + ToDateString(InctiveDate, "dd-MMM-yyyy"));
                ws.Cells[5, 1].LoadFromText("Current Effect Date: " + ToDateString(CurrentEffectDate, "dd-MMM-yyyy"));

                ws.Cells[7, 3].LoadFromText("Previous_Effect Date: " + ToDateString(PreviousEffectDate, "dd-MMM-yyyy"));
                ws.Cells[7, 7].LoadFromText("Current_Effect Date: " + ToDateString(CurrentEffectDate, "dd-MMM-yyyy"));

                ws.Cells[7, 11].LoadFromText("Diff_Percentage");


                ws.Cells["A8"].LoadFromDataTable(dtResult, true);

                for (var index = 1; index <= dtResult.Columns.Count; index++)
                {
                    ws.Cells[8, index].Value = ws.Cells[8, index].Value.ToString().Replace("_Diff_Percentage", "")
                        .Replace("First_", "").Replace("Second_", "");
                }


                ws.Cells["A8:B" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["A8:B" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                ws.Cells["C8:F" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["C8:F" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);

                ws.Cells["G8:J" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["G8:J" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                ws.Cells["K8:N" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["K8:N" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

                //ws.Cells["P1:S" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //ws.Cells["P1:S" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.YellowGreen);
                ws.Cells[8 + dtResult.Rows.Count + 1, 6].Formula = "=Sum(" + ws.Cells[9, 6].Address + ":" + ws.Cells[8 + dtResult.Rows.Count, 6].Address + ")";
                ws.Cells[8 + dtResult.Rows.Count + 1, 10].Formula = "=Sum(" + ws.Cells[9, 10].Address + ":" + ws.Cells[8 + dtResult.Rows.Count, 10].Address + ")";

                if (dtResult.Rows.Count > 0)
                {
                    PreviousCostTotala = Convert.ToDecimal(dtResult.Compute("SUM(First_Cost)", string.Empty));
                    CurrentCostTotala = Convert.ToDecimal(dtResult.Compute("SUM(Second_Cost)", string.Empty));
                    value = (100 / PreviousCostTotala) * CurrentCostTotala;
                    Result = (value - 100);
                }

                ws.Cells[8 + dtResult.Rows.Count + 1, 14].Value = Convert.ToDecimal(Result.ToString("0.##"));
                decimal rate = 7.50M;
                if (Result >= rate)
                {
                    ws.Cells[8 + dtResult.Rows.Count + 1, 14].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    ws.Cells[8 + dtResult.Rows.Count + 1, 14].Style.Numberformat.Format = "#,##0.00\\%;[Red](#,##0.00\\%)";

                }
                ws.Cells[8 + dtResult.Rows.Count + 1, 6].Style.Font.Bold = true;
                ws.Cells[8 + dtResult.Rows.Count + 1, 10].Style.Font.Bold = true;
                ws.Cells[8 + dtResult.Rows.Count + 1, 14].Style.Font.Bold = true;


                ws.Column(3).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(4).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(5).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(6).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(7).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(8).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(9).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(10).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(11).Style.Numberformat.Format = "#,##0.00\\%;[Red](#,##0.00\\%)";
                ws.Column(12).Style.Numberformat.Format = "#,##0.00\\%;[Red](#,##0.00\\%)";
                ws.Column(13).Style.Numberformat.Format = "#,##0.00\\%;[Red](#,##0.00\\%)";
                ws.Column(14).Style.Numberformat.Format = "#,##0.00\\%;[Red](#,##0.00\\%)";
                ws.Column(15).Style.Numberformat.Format = "#,##0.00\\%;[Red](#,##0.00\\%)";




                string filename = "BOM_Compare" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }
            catch (Exception)
            {


            }
            finally { }
            return RedirectToAction("Index");
        }

        public string ToDateString(string value, string format = "yyyy-MM-dd")
        {
            return Convert.ToDateTime(value).ToString(format);
        }


        [HttpPost]

        public ActionResult ApprovedBOMImport(BOMNBRVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BOMRepo(identity, Session);
            string[] result = new string[6];
            try
            {
                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                vm.ServerPath = Server.MapPath("~/Files/Approved_VAT4_3");

                result = _repo.ImportFile(vm);
                Session["result"] = result[0] + "~" + result[1];
                //return View("Index", vm);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message.Replace("\r", "").Replace("\n", "");

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult SubmittedBOMImport(BOMNBRVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            //_repo = new AuditRepo(identity, Session);
            string[] result = new string[6];
            try
            {
                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                vm.ServerPath = Server.MapPath("~/Files/Submitted_VAT4_3");

                result = _repo.ImportFile(vm);
                Session["result"] = result[0] + "~" + result[1];
                //return View("Index", vm);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message.Replace("\r", "").Replace("\n", "");

                return RedirectToAction("Index");
            }
        }

        public ActionResult Submitted_VAT4_3_Download(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                _repo = new BOMRepo(identity, Session);
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
                BOMNBRVM vm = new BOMNBRVM();
                var path = Server.MapPath("~/Files/Submitted_VAT4_3");

                vm = _repo.SelectAll(id).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(vm.SubmittedFilePath) && !string.IsNullOrWhiteSpace(vm.SubmittedFileName))
                {
                    path = Path.Combine(path, vm.SubmittedFilePath); // Use Path.Combine for better path handling

                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        var memory = new MemoryStream();
                        stream.CopyTo(memory);
                        memory.Position = 0;
                        return File(memory, "application/pdf", Path.GetFileName(path));
                    }
                }
                else
                {
                    Session["result"] = "Fail" + "~" + "There was no Attachment file";
                }
                return RedirectToAction("Index");
                
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message.Replace("\r", "").Replace("\n", "");

                return RedirectToAction("Index");
            }
        }

        public ActionResult Approved_VAT4_3_Download(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                _repo = new BOMRepo(identity, Session);
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
                BOMNBRVM vm = new BOMNBRVM();
                var path = Server.MapPath("~/Files/Approved_VAT4_3");

                vm = _repo.SelectAll(id).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(vm.ApprovedFilePath) && !string.IsNullOrWhiteSpace(vm.ApprovedFileName))
                {
                    path = Path.Combine(path, vm.ApprovedFilePath); // Use Path.Combine for better path handling

                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        var memory = new MemoryStream();
                        stream.CopyTo(memory);
                        memory.Position = 0;
                        return File(memory, "application/pdf", Path.GetFileName(path));
                    }
                }
                else
                {
                    Session["result"] = "Fail" + "~" + "There was no Attachment file";
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message.Replace("\r", "").Replace("\n", "");

                return RedirectToAction("Index");
            }
        }



    }
}
