//using JQueryDataTables.Models;
using SymOrdinary;
using SymphonySofttech.Reports.Report;
//using SymRepository.Common;
using SymVATWebUI.Areas.VMS.Models;
//using SymViewModel.Common;
using VATViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymRepository.VMS;
using SymVATWebUI.Filters;
using VATServer.Ordinary;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class ProductController : Controller
    {
        //
        // GET: /VMS/Branch/

        // 
        ShampanIdentity identity = null;

        ProductRepo _repo = null;

        public ProductController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new ProductRepo(identity);

            }
            catch
            {

            }
        }
        // ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        // ProductRepo _repo = new ProductRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index(ProductVM paramVM, string TransactionType)
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

            paramVM.TransactionType = TransactionType;

            if (string.IsNullOrWhiteSpace(paramVM.TransactionType))
            {
                paramVM.TransactionType = "other";
            }

            return View(paramVM);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param, ProductVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ProductRepo(identity, Session);

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
            //00     //ItemNo 
            //01     //ProductName
            //02     //CostPrice
            //03     //SalesPrice  
            //04     //VATRate
            //05     //OpeningDate
            //06    //CategoryName

            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "100";
            }

            #region Search and Filter Data
            string[] cFields;
            string[] cValues;

            cFields = new string[] { "Pr.CategoryID", "Pr.ActiveStatus", "SelectTop" };
            cValues = new string[] { paramVM.CategoryID, paramVM.isActive, paramVM.SelectTop };

            if (!string.IsNullOrWhiteSpace(paramVM.SearchField))
            {
                //////cFields.ToList().Add("Pr." + paramVM.SearchField + " like");
                //////cValues.ToList().Add(paramVM.SearchValue);

                var tempFields = cFields.ToList();
                tempFields.Add("Pr." + paramVM.SearchField + " like");
                cFields = tempFields.ToArray();

                var tempValues = cValues.ToList();
                tempValues.Add(paramVM.SearchValue);
                cValues = tempValues.ToArray();

            }

            if (!string.IsNullOrWhiteSpace(paramVM.ProductType))
            {
                var tempFields = cFields.ToList();
                tempFields.Add("Pc.IsRaw");
                cFields = tempFields.ToArray();

                var tempValues = cValues.ToList();
                tempValues.Add(paramVM.ProductType);
                cValues = tempValues.ToArray();
            }
            else
            {
                if (paramVM.TransactionType.ToLower() == "other")
                {
                    var tempFields = cFields.ToList();
                    tempFields.Add("Pc.IsRaw !");
                    cFields = tempFields.ToArray();

                    var tempValues = cValues.ToList();
                    tempValues.Add("Overhead");
                    cValues = tempValues.ToArray();

                }
                else
                {

                    var tempFields = cFields.ToList();
                    tempFields.Add("Pc.IsRaw");
                    cFields = tempFields.ToArray();

                    var tempValues = cValues.ToList();
                    tempValues.Add("Overhead");
                    cValues = tempValues.ToArray();

                }
            }


            var getAllData = _repo.SelectAll("0", cFields, cValues);

            IEnumerable<ProductVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);



                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.ProductName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.CostPrice.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.SalesPrice.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.NBRPrice.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.VATRate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.OpeningDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.CategoryName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable8 && c.ProductCode.ToString().ToLower().Contains(param.sSearch.ToLower())

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
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);



            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ProductVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.ProductName :
                sortColumnIndex == 2 && isSortable_2 ? c.CostPrice.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.SalesPrice.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.NBRPrice.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.VATRate.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.OpeningDate.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.CategoryName :
                sortColumnIndex == 8 && isSortable_8 ? c.ProductCode :

                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                c.ItemNo.ToString()
                , c.ProductCode
                , c.ProductName
                , c.CostPrice.ToString()
                , c.SalesPrice.ToString()
                , c.NBRPrice.ToString()
                , c.VATRate.ToString()+"%"
                , Ordinary.DateTimeToDate(c.OpeningDate.ToString())
                , c.CategoryName
                , c.IsSample
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
        public ActionResult Create(string TransactionType)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ProductRepo(identity, Session);
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
            ProductVM vm = new ProductVM();

            List<ProductNameVM> ProductDetailVMs = new List<ProductNameVM>();
            vm.Details = ProductDetailVMs;
            vm.TransactionType = TransactionType;
            vm.Operation = "add";
            vm.ActiveStatus = "Y";
            vm.TollProduct = "N";
            vm.IsVDS = "N";
            vm.IsConfirmed = "N";
            vm.IsTransport = "N";
            vm.Banderol = "N";
            vm.IsFixedVATRebate = "N";
            vm.IsSample = "N";

            if (vm.TransactionType == "Other")
            {
                vm.Type = "Finish";
            }
            if (vm.TransactionType == "Overhead")
            {
                vm.Type = "Overhead";
            }

            CommonRepo commonRepo = new CommonRepo(identity);

            string ExpireDateTracking = commonRepo.settings("Purchase", "ExpireDateTracking");
            vm.IsExpireDate = ExpireDateTracking;


            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(ProductVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ProductRepo(identity, Session);
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
            string[] result = new string[6];
            string[] stockResult = new string[6];
            string[] resultName = new string[6];

            #region populating
            //vm.CostPrice = 0;
            //vm.SalesPrice = 0;
            //vm.ReceivePrice = 0;
            //vm.TenderPrice = 0;
            //vm.IssuePrice = 0;
            //vm.ExportPrice = 0;
            //vm.InternalIssuePrice = 0;
            //vm.TollCharge = 0;
            //vm.PacketPrice = 0;
            //vm.TVBRate = 0;
            //vm.CnFRate = 0;
            //vm.InsuranceRate = 0;
            //vm.CDRate = 0;
            //vm.RDRate = 0;
            //vm.AITRate = 0;
            //vm.ATVRate = 0;
            //vm.TVARate = 0;
            //vm.RebatePercent = 0;

            #endregion populating
            try
            {

                if (vm.Operation.ToLower() == "add")
                {
                    if (string.IsNullOrWhiteSpace(vm.OpeningDate))
                    {
                        var OpeningDate = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                        vm.OpeningDate = OpeningDate.ToString();
                    }

                    vm.ItemNo = vm.TransactionType;
                    vm.CreatedOn = DateTime.Now.ToString();
                    vm.CreatedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString();

                    vm.IsFixedSD = vm.IsFixedSDChecked ? "Y" : "N";
                    vm.IsFixedCD = vm.IsFixedCDChecked ? "Y" : "N";
                    vm.IsFixedRD = vm.IsFixedRDChecked ? "Y" : "N";
                    vm.IsFixedAIT = vm.IsFixedAITChecked ? "Y" : "N";
                    vm.IsFixedVAT1 = vm.IsFixedVAT1Checked ? "Y" : "N";
                    vm.IsFixedAT = vm.IsFixedATChecked ? "Y" : "N";
                    
                    //result = _repo.InsertToProduct(vm,new List<TrackingVM>(),vm.Type);
                    result = _repo.InsertToProduct(vm, new List<TrackingVM>(), vm.Type);


                    Session["result"] = result[0] + "~" + result[1];

                    vm.ItemNo = result[3];
                    //string itemno=result[3];
                    if (vm.Details != null && vm.Details.Count > 0)
                    {
                        foreach (var detailVM in vm.Details)
                        {
                            ProductNameVM pVM = new ProductNameVM();
                            pVM.ItemNo = vm.ItemNo;
                            pVM.ProductName = detailVM.ProductName;
                            resultName = _repo.InsertToProductName(pVM);
                        }
                    }

                    if (result[0].ToLower() == "success")
                    {
                        var _Stockrepo = new ProductRepo(identity, Session);

                        ProductStockVM ProductStockVM = new ProductStockVM();

                        ProductStockVM.StockQuantity = 0;
                        ProductStockVM.StockValue = 0;
                        ProductStockVM.BranchId = Convert.ToInt32((Session["BranchId"]).ToString());
                        ProductStockVM.ItemNo = result[2];
                        if (string.IsNullOrWhiteSpace(ProductStockVM.Comments))
                        {
                            ProductStockVM.Comments = "NA";
                        }

                        string UserId = identity.UserId;
                        stockResult = _Stockrepo.InserToProductStock(ProductStockVM, null, null, null, UserId);

                    }


                    //var vmName = new List<ProductNameVM>();
                    // foreach (ProductNameVM vmN in vmName)
                    // {
                    //     vmN.ItemNo = vm.ItemNo;

                    // }

                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2], vm.TransactionType });
                    }
                    else
                    {
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    if (string.IsNullOrWhiteSpace(vm.OpeningDate))
                    {
                        var OpeningDate = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                        vm.OpeningDate = OpeningDate.ToString();
                    }

                    vm.LastModifiedOn = DateTime.Now.ToString();
                    vm.LastModifiedBy = identity.Name;

                    vm.IsFixedSD = vm.IsFixedSDChecked ? "Y" : "N";
                    vm.IsFixedCD = vm.IsFixedCDChecked ? "Y" : "N";
                    vm.IsFixedRD = vm.IsFixedRDChecked ? "Y" : "N";
                    vm.IsFixedAIT = vm.IsFixedAITChecked ? "Y" : "N";
                    vm.IsFixedVAT1 = vm.IsFixedVAT1Checked ? "Y" : "N";
                    vm.IsFixedAT = vm.IsFixedATChecked ? "Y" : "N";

                    result = _repo.UpdateProduct(vm, new List<TrackingVM>(), vm.Type);

                    #region `Comments

                    //string[] DeleteName = new string[6];

                    ////vm.Details = new List<ProductNameVM>();

                    //if (vm.Details != null && vm.Details.Count > 0)
                    //{
                    //    foreach (var detailVM in vm.Details)
                    //    {
                    //        ProductNameVM pVM = new ProductNameVM();
                    //        string itemNo = detailVM.ItemNo;
                    //        string id = detailVM.Id.ToString();                         
                    //        pVM.ProductName = detailVM.ProductName;
                    //        DeleteName = _repo.DeleteProductName(itemNo, id);
                    //    }
                    //}


                    //if (DeleteName[0] == "Success")
                    //{
                    //    if (vm.Details != null && vm.Details.Count > 0)
                    //    {
                    //        foreach (var detailVM in vm.Details)
                    //        {
                    //            ProductNameVM pVM = new ProductNameVM();
                    //            pVM.ItemNo = vm.ItemNo;
                    //            pVM.ProductName = detailVM.ProductName;
                    //            resultName = _repo.InsertToProductName(pVM);
                    //        }
                    //    }

                    //}

                    #endregion

                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = result[2], vm.TransactionType });
                }
                else
                {
                    return View("Create", vm);
                }
            }
            catch (Exception ex)
            {

                Session["result"] = "Fail~!" + ex.Message.Replace("\n", "").Replace("\r", "");
                // FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string Id, string TransactionType)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ProductRepo(identity, Session);
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

            try
            {



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
                DataTable dt = new DataTable();
                ProductNameVM VMName = new ProductNameVM();

                CommonRepo commonRepo = new CommonRepo(identity);

                string ExpireDateTracking = commonRepo.settings("Purchase", "ExpireDateTracking");

                ProductVM vm = new ProductVM();
                vm = _repo.SelectAll(Id).FirstOrDefault();
                vm.Type = vm.IsRaw;
                vm.IsExpireDate = ExpireDateTracking;

                string Item = vm.ItemNo;
                dt = _repo.SelectProductName(Item);
                vm.Details = new List<ProductNameVM>();

                foreach (DataRow row in dt.Rows)
                {
                    var detail = new ProductNameVM();

                    detail.ProductName = row["ProductName"].ToString();
                    detail.ItemNo = row["ItemNo"].ToString();
                    detail.Id = Convert.ToInt32(row["Id"]);

                    vm.Details.Add(detail);
                }



                DataTable dtStock = new DataTable();

                List<UserBranchDetailVM> vms = _repo.SelectAllLst(identity.UserId);

                dtStock = _repo.SearchProductStock(vm.ItemNo, "", vms);

                vm.ProductStocks = new List<ProductStockVM>();

                foreach (DataRow row in dtStock.Rows)
                {
                    ProductStockVM ProductStockVM = new ProductStockVM();

                    ProductStockVM.StockQuantity = Convert.ToDecimal(row["StockQuantity"].ToString());
                    ProductStockVM.StockValue = Convert.ToDecimal(row["StockValue"].ToString());
                    ProductStockVM.StockId = Convert.ToInt32(row["Id"].ToString());
                    ProductStockVM.BranchId = Convert.ToInt32(row["BranchId"].ToString());
                    ProductStockVM.ItemNo = row["ItemNo"].ToString();
                    ProductStockVM.Comments = row["Comments"].ToString();
                    ProductStockVM.BranchName = row["BranchName"].ToString();

                    vm.ProductStocks.Add(ProductStockVM);
                }

                vm.TransactionType = TransactionType;
                vm.IsFixedSDChecked = vm.IsFixedSD == "Y";
                vm.IsFixedCDChecked = vm.IsFixedCD == "Y";
                vm.IsFixedRDChecked = vm.IsFixedRD == "Y";
                vm.IsFixedAITChecked = vm.IsFixedAIT == "Y";
                vm.IsFixedVAT1Checked = vm.IsFixedVAT1 == "Y";
                vm.IsFixedATChecked = vm.IsFixedAT == "Y";
                vm.Operation = "update";
                vm.OpeningDate = Ordinary.DateTimeToDate(vm.OpeningDate);
                return View("Create", vm);
            }
            catch (Exception ex)
            {

                Session["result"] = "Fail~!" + ex.Message.Replace("\n", "").Replace("\r", "");
                FileLogger.Log("ProductController", "Edit", ex.ToString());
                return View("Index");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ProductRepo(identity, Session);
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
            ProductVM vm = new ProductVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.LastModifiedBy = identity.Name;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult GetHsCode(string id)
        {
            var repo = new HSCodeRepo(identity, Session);

            var hsCode = repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();


            return Json(hsCode, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult GetProducts(string code)
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
            var _repo = new ProductRepo(identity, Session);

            var vm = new List<ProductVM>();
            vm = _repo.SelectAll();

            return PartialView("_product", vm);
        }

        [Authorize]
        public ActionResult PrintProducts()
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
            var vm = new ProductReportViewModel();
            return PartialView("_printProduct", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ShowProduct(string code)
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
            var _repo = new ProductRepo(identity, Session);

            var vm = new ProductVM();
            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { code };
            vm = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
            vm.Operation = "detail";
            if (vm.TransactionType == null)
            {
                vm.TransactionType = "";
            }
            if (vm.IsExpireDate == null)
            {
                vm.IsExpireDate = "n";
            }

            return PartialView("Create", vm);
        }

        public ActionResult Navigate(string id, string btn)
        {
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetId("Products", "Id", id, btn);
            return RedirectToAction("Edit", new { id = targetId });
        }

        [Authorize]
        public ActionResult GetProductGroupId(string productCode)
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
            var _repo = new ProductRepo(identity, Session);

            var vm = new ProductVM();
            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };
            vm = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();

            var groupRepo = new ProductCategoryRepo(identity, Session);
            var group = groupRepo.SelectAll(Convert.ToInt32(vm.CategoryID)).SingleOrDefault();

            var data = vm.CategoryID + "~" + group.IsRaw;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult GetProductType(string groupId)
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

            var groupRepo = new ProductCategoryRepo(identity, Session);
            var group = groupRepo.SelectAll(Convert.ToInt32(groupId)).SingleOrDefault();

            return Json(group.IsRaw, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult ReportView(ProductReportViewModel vm)
        {
            try
            {
                if (vm.ProductName == null)
                {
                    vm.ProductName = "";
                }
                if (vm.ProductGroup == null)
                {
                    vm.ProductGroup = "";
                }
                if (vm.ProductType == null)
                {
                    vm.ProductType = "";
                }
                var ReportResult = new DataSet();
                ReportDSRepo reportDsdal = new ReportDSRepo(identity, Session);
                ReportResult = reportDsdal.ProductNew(vm.ProductName, vm.ProductGroup, vm.ProductType);
                if (ReportResult.Tables.Count <= 0)
                {
                    //some codes here
                }
                ReportResult.Tables[0].TableName = "DsProduct";
                RptProductListing objrpt = new RptProductListing();
                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + identity.FullName + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Product Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + identity.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Program.Address1'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'Program.Address2'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'Program.Address3'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                var gr = new GenericReport<RptProductListing>();
                var rpt = gr.RenderReportAsPDF(objrpt);
                objrpt.Close();
                return rpt;
            }
            catch (Exception e)
            {
                throw e;
            }

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
            return PartialView("_products", vm);
        }

        public ActionResult GetVehicleNoPopUp(PopUpViewModel vm)
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
            return PartialView("_VehicleNo", vm);
        }


        public ActionResult GetProductNamesPopUp(PopUpViewModel vm)
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
            return PartialView("_productsNames", vm);
        }
        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredProducts(ProductVM vm)
        {
            var _repo = new ProductRepo(identity, Session);

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
            string[] conditionalFields;
            string[] conditionalValues;
            if (vm.SearchField != null)
            {
                conditionalFields = new string[] { "Pr." + vm.SearchField + " like", "Pr.CategoryID", "Pc.IsRaw", "Pr.ActiveStatus" };
                conditionalValues = new string[] { vm.SearchValue, vm.CategoryID, vm.ProductType, activeSatus };
            }
            else
            {
                conditionalFields = new string[] { "Pr.CategoryID", "Pc.IsRaw", "Pr.ActiveStatus" };
                conditionalValues = new string[] { vm.CategoryID, vm.ProductType, activeSatus };
            }
            var list = _repo.SelectAll("0", conditionalFields, conditionalValues);

            return PartialView("_filteredProducts", list);
        }
        public ActionResult GetFilteredProductNames(ProductVM vm)
        {
            var _repo = new ProductRepo(identity, Session);

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
            var activeSatus = vm.ActiveStatus;
            string[] conditionalFields;
            string[] conditionalValues;
            if (vm.SearchField != null)
            {
                conditionalFields = new string[] { "Pr." + vm.SearchField, "Pr.CategoryID", "Pc.IsRaw", "Pr.ActiveStatus" };
                conditionalValues = new string[] { vm.SearchValue, vm.CategoryID, vm.ProductType, activeSatus };
            }
            else
            {
                conditionalFields = new string[] { "Pr.CategoryID", "Pc.IsRaw", "Pr.ActiveStatus" };
                conditionalValues = new string[] { vm.CategoryID, vm.ProductType, activeSatus };
            }
            var VM = new ProductVM();
            VM = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
            //VM = _repo.SelectAll("0", conditionalFields, conditionalValues);
            vm.ItemNo = VM.ItemNo;
            conditionalFields = new string[] { "Pr.ItemNo" };
            conditionalValues = new string[] { vm.ItemNo };
            var list = _repo.SelectProductName("0", conditionalFields, conditionalValues);
            return PartialView("_filteredProductsNames", list);
        }

        public ActionResult GetFilteredVehicleNo(VehicleVM vm)
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
            //var activeSatus = vm.ActiveStatus;
            string[] conditionalFields;
            string[] conditionalValues;
            //if (vm.SearchField != null)
            //{
            //    conditionalFields = new string[] { "Pr." + vm.SearchValue, "Pr.CategoryID", "Pc.IsRaw", "Pr.ActiveStatus" };
            //    conditionalValues = new string[] { vm.SearchValue};
            //}
            //else
            //{
            //    conditionalFields = new string[] { "Pr.CategoryID", "Pc.IsRaw", "Pr.ActiveStatus" };
            //    conditionalValues = new string[] { vm.CategoryID, vm.ProductType, activeSatus };
            //}
            //var VM = new ProductVM();
            //VM = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
            ////VM = _repo.SelectAll("0", conditionalFields, conditionalValues);
            //vm.ItemNo = VM.ItemNo;
            conditionalFields = new string[] { "VehicleNo like" };
            conditionalValues = new string[] { vm.SearchValue };
            var list = new VehicleRepo(identity, Session).SelectAll(0, conditionalFields, conditionalValues);
            return PartialView("_filteredVehicleNo", list);
        }



        public JsonResult GetProductByCode(string code)
        {
            var _repo = new ProductRepo(identity, Session);

            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { code };
            var vm = new ProductVM();
            var product = _repo.SelectAll("0", conditionalFields, conditionalValues, null, null, vm).SingleOrDefault();

            return Json(product, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetProductByItemNo(string ItemNo)
        {
            var _repo = new ProductRepo(identity, Session);

            string[] conditionalFields = new string[] { "Pr.ItemNo" };
            string[] conditionalValues = new string[] { ItemNo };
            var vm = new ProductVM();
            var product = _repo.SelectAll("0", conditionalFields, conditionalValues, null, null, vm).SingleOrDefault();

            return Json(product, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(ProductNameVM vm)
        {
            var _repo = new ProductRepo(identity, Session);
            string[] sqlResults = new string[4];

            sqlResults = _repo.InsertToProductName(vm);

            return PartialView("_detail", vm);

        }


        [Authorize(Roles = "Admin")]
        public ActionResult BlankItems(ProductStockVM vm)
        {
            var _repo = new ProductRepo(identity, Session);
            var _bRepo = new BranchRepo(identity, Session);

            ProductStockVM ProductStockVM = new ProductStockVM();

            string[] stockResult = new string[4];

            if (string.IsNullOrWhiteSpace(vm.Comments))
            {
                vm.Comments = "NA";
            }

            string UserId = identity.UserId;
            ////stockResult = _repo.InserToProductStock(vm, null, null, null, UserId);
            stockResult = _repo.UpdateToProductStockWeb(vm, null, null, null, UserId);

            if (stockResult[0] == "Fail")
            {
                Session["result"] = stockResult[0] + "~" + stockResult[1];

                return PartialView("_detailStock", ProductStockVM);
            }

            DataTable dt = new DataTable();

            List<UserBranchDetailVM> vms = _repo.SelectAllLst(identity.UserId);

            BranchProfileVM bVM = _bRepo.SelectAll(vm.BranchId.ToString()).FirstOrDefault();

            ////dt = _repo.SearchProductStock(vm.ItemNo, "", vms);

            ////foreach (DataRow row in dt.Rows)
            ////{

            ////    ProductStockVM.StockQuantity = Convert.ToDecimal(row["StockQuantity"].ToString());
            ////    ProductStockVM.StockValue = Convert.ToDecimal(row["StockValue"].ToString());
            ////    ProductStockVM.StockId = Convert.ToInt32(row["Id"].ToString());
            ////    ProductStockVM.BranchId = Convert.ToInt32(row["BranchId"].ToString());
            ////    ProductStockVM.BranchName = row["BranchName"].ToString();
            ////    ProductStockVM.ItemNo = row["ItemNo"].ToString();
            ////    ProductStockVM.Comments = row["Comments"].ToString();
            ////}

            ProductStockVM.StockQuantity = vm.StockQuantity;
            ProductStockVM.StockValue = vm.StockValue;
            ProductStockVM.StockId = vm.StockId;
            ProductStockVM.BranchId = vm.BranchId;
            ProductStockVM.BranchName = bVM.BranchName;
            ProductStockVM.ItemNo = vm.ItemNo;
            ProductStockVM.Comments = vm.Comments;

            return PartialView("_detailStock", ProductStockVM);
        }




        [Authorize(Roles = "Admin")]
        public ActionResult SyncProduct()
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ProductRepo(identity, Session);
            DataTable productDt = new DataTable();
            try
            {
                string[] results = new string[4];

                ImportRepo importrepo = new ImportRepo(identity, Session);
                CommonRepo commonrepo = new CommonRepo(identity, Session);
                results[0] = "fail";
                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                var BranchInfoDT = branchProfileRepo.SelectAl(Convert.ToString(Session["BranchId"]));
                string code = commonrepo.settings("CompanyCode", "Code");
                settingVM.BranchInfoDT = BranchInfoDT;
                if (OrdinaryVATDesktop.IsACICompany(Convert.ToString(Session["CompanyCode"])))
                {
                    productDt = importrepo.GetProductACIDbData(settingVM.BranchInfoDT);
                }
                else if (OrdinaryVATDesktop.IsUnileverCompany(Convert.ToString(Session["CompanyCode"])))
                {
                    productDt = importrepo.GetProductUnileverDbData(settingVM.BranchInfoDT);
                }
                List<ProductVM> products = new List<ProductVM>();

                int rowsCount = productDt.Rows.Count;
                List<string> ids = new List<string>();

                string defaultGroup = commonrepo.settings("AutoSave", "DefaultProductCategory");

                for (int i = 0; i < rowsCount; i++)
                {
                    ProductVM product = new ProductVM();

                    product.ProductName = Ordinary.RemoveStringExpresion(productDt.Rows[i]["ProductName"].ToString());
                    product.ProductDescription = productDt.Rows[i]["Description"].ToString();
                    product.CategoryName = productDt.Rows[i]["ProductGroup"].ToString();

                    if (product.CategoryName == "-" || string.IsNullOrWhiteSpace(product.CategoryName))
                    {
                        product.CategoryName = defaultGroup;
                    }

                    if (OrdinaryVATDesktop.IsUnileverCompany(Convert.ToString(Session["CompanyCode"])))
                    {
                        product.TradingSaleVATRate = Convert.ToDecimal(productDt.Rows[i]["VATRate"].ToString());
                        product.TradingSaleSD = Convert.ToDecimal(productDt.Rows[i]["SDRate"].ToString());

                        product.VATRate = Convert.ToDecimal(productDt.Rows[i]["VATRate"].ToString());
                        product.SD = Convert.ToDecimal(productDt.Rows[i]["SDRate"].ToString());
                        product.ShortName = productDt.Rows[i]["ProductNameBangla"].ToString();
                        product.Packetprice = Convert.ToDecimal(productDt.Rows[i]["Packetprice"].ToString());
                        product.BranchId = 1;

                    }
                    else
                    {
                        product.VATRate = Convert.ToDecimal(productDt.Rows[i]["VATRate"].ToString());
                        product.SD = Convert.ToDecimal(productDt.Rows[i]["SDRate"].ToString());
                        product.Packetprice = 0;
                        product.BranchId = OrdinaryVATDesktop.BranchId;

                    }
                    product.UOM = productDt.Rows[i]["UOM"].ToString();

                    product.NBRPrice = Convert.ToDecimal(productDt.Rows[i]["UnitPrice"].ToString());

                    product.SerialNo = "-";
                    product.HSCodeNo = productDt.Rows[i]["HSCode"].ToString();
                    //product.VATRate = Convert.ToDecimal(productDt.Rows[i]["VATRate"].ToString());
                    product.Comments = "-";
                    product.ActiveStatus = "Y";
                    //product.SD = Convert.ToDecimal(productDt.Rows[i]["SDRate"].ToString());
                    //product.Packetprice = 0;
                    product.Trading = "N";
                    product.TradingMarkUp = 0;
                    product.NonStock = "N"; ;
                    product.OpeningDate = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    product.CreatedBy = OrdinaryVATDesktop.CurrentUser;
                    product.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    product.ProductCode = Ordinary.RemoveStringExpresion(productDt.Rows[i]["ProductCode"].ToString());
                    product.TollCharge = 0;
                    product.IsConfirmed = "N";

                    if (code.ToLower() == "cepl")
                    {
                        product.UOM = "pcs";
                        product.VATRate = 15;
                    }

                    products.Add(product);

                    ids.Add(productDt.Rows[i]["SL"].ToString());
                }

                results = importrepo.ImportProductSync(products, new List<TrackingVM>());

                if (results[0].ToLower() == "success")
                {
                    if (OrdinaryVATDesktop.IsACICompany(Convert.ToString(Session["CompanyCode"])))
                    {
                        results = importrepo.UpdateACIMaster(ids, settingVM.BranchInfoDT, "Products");
                    }
                    //else if (OrdinaryVATDesktop.IsUnileverCompany(Convert.ToString(Session["CompanyCode"])))
                    //{
                    //    results = importrepo.UpdateUnileverMaster(ids, settingVM.BranchInfoDT, "Products");
                    //}

                }
                if (results[0].ToLower() == "success")
                {
                    Session["result"] = "Success~Successfully Synchronized";
                    return Redirect("/Vms/Product/Index?TransactionType=other");
                }
                else
                {
                    Session["result"] = "Fail~Nothing to syncronize";
                    return Redirect("/Vms/Product/Index?TransactionType=other");
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;
                return Redirect("/Vms/Product/Index?TransactionType=other");
            }
        }

        public JsonResult SelectBOMRaw(string ProductCode, string effectDate)
        {

            DataTable dt = new DataTable();
            int BOMId = 0;
            dt = _repo.SelectBOMRaw(ProductCode, effectDate);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                string tempBOMId = dr["BOMId"].ToString();
                if (!string.IsNullOrWhiteSpace(tempBOMId))
                {
                    BOMId = Convert.ToInt32(tempBOMId);
                }
            }


            return Json(BOMId, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult ExportExcell(ProductVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ProductRepo(identity, Session);

            ResultVM rVM = new ResultVM();

            List<ProductVM> getAllData = new List<ProductVM>();

            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "All";
            }

            try
            {

                if (paramVM.ExportAll)
                {
                    string[] conditionFields = new string[] { "Pr.CategoryID", "Pc.IsRaw", "Pr.ActiveStatus", "SelectTop" };
                    string[] conditionValues = new string[] { paramVM.CategoryID, paramVM.ProductType, paramVM.ActiveStatus, paramVM.SelectTop };

                    List<ProductVM> products = _repo.SelectAll("0", conditionFields, conditionValues);

                    paramVM.ProductIDs = products.Select(x => x.ProductCode).ToList();

                }

                DataTable dt = _repo.GetExcelData(paramVM.ProductIDs);
                var details = _repo.GetExcelProductDetails(paramVM.ProductIDs);

                var dataSet = new DataSet();
                dataSet.Tables.Add(dt);

                var sheetNames = new[] { "Products" };

                if (details.Rows.Count > 0)
                {
                    dataSet.Tables.Add(details);
                    sheetNames = new[] { "Products", "Details" };
                }

                var vm = OrdinaryVATDesktop.DownloadExcelMultiple(dataSet, "Products", sheetNames);
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
            catch (Exception ex)
            {


            }
            finally { }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteData(ProductStockVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ProductRepo(identity, Session);
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
            //ProductVM vm = new ProductVM();
            //string[] a = ids.Split('~');
            string[] result = new string[6];
            //vm.LastModifiedOn = DateTime.Now.ToString();
            //vm.LastModifiedBy = identity.Name;
            result = _repo.DeleteToProductStock(paramVM);


            Session["result"] = result[1];

            return Json(result[1], JsonRequestBehavior.AllowGet);
        }



    }
}
