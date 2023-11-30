using CrystalDecisions.CrystalReports.Engine;
//using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.VMS;
//using SymRepository.Common;
using VATViewModel.DTOs;
//using SymViewModel.Common;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading.Tasks;
using System.Net.Http;
using Excel.Log.Logger;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymVATWebUI.Filters;
using VATServer.Library;
using VATServer.Ordinary;
using SymphonySofttech.Utilities;
using System.Text;


namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class SaleInvoiceController : Controller
    {
        ShampanIdentity identity = null;

        SaleInvoiceRepo _repo = null;

        public SaleInvoiceController()
        {
            try
            {

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new SaleInvoiceRepo(identity);
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

        [Authorize(Roles = "Admin")]
        public ActionResult HomeIndex()
        {
            return View();
        }

        //, RoleFilter(FormId = "140110110")
        [UserFilter]
        public ActionResult Index(SaleMasterVM paramVM)
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

            #region Comments

            //string formId = "0";
            //if (paramVM.TransactionType == "other")
            //{
            //    formId = "140110110";

            //}
            //else if (paramVM.TransactionType == "Export")
            //{
            //    formId = "140110120";
            //}


            //if (!identity.IsPermitted(formId))
            //{
            //    return RedirectToAction("Login", "Home");
            //}

            //Session["dtFrom"] = dtFrom;
            //Session["dtTo"] = dtTo;

            //SaleMasterVM vm = new SaleMasterVM();
            //vm.TransactionType = tType;

            #endregion

            ////paramVM.SelectTop = "100";

            ViewBag.TransactionType = paramVM.TransactionType;

            paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            paramVM.DuplicateInvoiceSave = new CommonRepo(identity, Session).settings("Integration", "DuplicateInvoiceSave");
            paramVM.DuplicateInvoiceSave = paramVM.DuplicateInvoiceSave == "" ? "N" : paramVM.DuplicateInvoiceSave.ToUpper();

            #region BranchList

            int userId = Convert.ToInt32(identity.UserId);
            var list = new SymRepository.VMS.BranchRepo(identity).UserDropDownBranchProfile(userId);

            var listBranch = new SymRepository.VMS.BranchRepo(identity).SelectAll();

            if (list.Count() == listBranch.Count())
            {
                list.Add(new BranchProfileVM() { BranchID = -1, BranchName = "All" });
            }

            paramVM.BranchList = list;

            #endregion

            return View(paramVM);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamVM param, SaleMasterVM paramVM)
        {
            _repo = new SaleInvoiceRepo(identity, Session);

            List<SaleMasterVM> getAllData = new List<SaleMasterVM>();

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

            #endregion

            #region SeachParameters

            string dtFrom = null;
            string dtTo = null;

            if (string.IsNullOrWhiteSpace(paramVM.SearchField))
            {
                //dtFrom = DateTime.Now.ToString("yyyyMMdd");
                //dtTo = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
                if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeFrom))
                {
                    dtFrom = Convert.ToDateTime(paramVM.InvoiceDateTimeFrom).ToString("yyyyMMdd");
                }
                if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeTo))
                {
                    dtTo = Convert.ToDateTime(paramVM.InvoiceDateTimeTo).AddDays(1).ToString("yyyyMMdd");
                }
            }

            if (paramVM.BranchId == 0)
            {
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            }

            if (paramVM.BranchId == -1)
            {
                paramVM.BranchId = 0;
            }
            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "100";
            }

            #endregion SeachParameters

            #region Data Call

            string[] conditionFields;
            string[] conditionValues;
            if (string.IsNullOrWhiteSpace(paramVM.SearchField))
            {
                conditionFields = new string[] { "sih.InvoiceDateTime>=", "sih.InvoiceDateTime<=", "sih.SaleType", "sih.CustomerID", "sih.IsPrint", "sih.Post", "sih.BranchId", "sih.IsInstitution", "SelectTop" };
                conditionValues = new string[] { dtFrom, dtTo, paramVM.SaleType, paramVM.CustomerID, paramVM.IsPrint, paramVM.Post, paramVM.BranchId.ToString(), paramVM.IsInstitution, paramVM.SelectTop };

            }
            else
            {
                if (paramVM.SearchField == "VehicleNo")
                {
                    paramVM.SearchField = "v.VehicleNo like";
                }

                else if (paramVM.SearchField == "CustomerName")
                {
                    paramVM.SearchField = "c." + paramVM.SearchField + " like";

                }
                else if (paramVM.SearchField == "CustomerCode")
                {
                    paramVM.SearchField = "c.CustomerCode like";
                }
                else if (paramVM.SearchField == "ImportID")
                {
                    paramVM.SearchField = "sih.ImportIDExcel like";
                }
                else
                {
                    paramVM.SearchField = "sih." + paramVM.SearchField + " like";
                }

                conditionFields = new string[] { "sih.InvoiceDateTime>=", "sih.InvoiceDateTime<=", "sih.SaleType", "sih.CustomerID", "sih.IsPrint", "sih.Post", paramVM.SearchField, "sih.BranchId", "sih.IsInstitution", "SelectTop" };
                conditionValues = new string[] { dtFrom, dtTo, paramVM.SaleType, paramVM.CustomerID, paramVM.IsPrint, paramVM.Post, paramVM.SearchValue, paramVM.BranchId.ToString(), paramVM.IsInstitution, paramVM.SelectTop };


            }
            getAllData = _repo.SelectAll(0, conditionFields, conditionValues, null, null, null, paramVM.TransactionType, "Y");


            #endregion

            #region Search and Filter Data
            IEnumerable<SaleMasterVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Id
                //SalesInvoiceNo
                //CustomerName
                //DeliveryAddress1
                //DeliveryDate
                //TotalAmount
                //Post

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                var isSearchable9 = Convert.ToBoolean(Request["bSearchable_9"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.SalesInvoiceNo.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.CustomerName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.DeliveryAddress1.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.InvoiceDateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.TotalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.TotalVATAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable8 && c.IsInstitution.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable9 && c.ImportIDExcel.ToString().ToLower().Contains(param.sSearch.ToLower())

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
            var isSortable_9 = Convert.ToBoolean(Request["bSortable_9"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SaleMasterVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.SalesInvoiceNo :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.CustomerName.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.DeliveryAddress1.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.InvoiceDateTime.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.TotalAmount.ToString() :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.TotalVATAmount.ToString() :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.Post.ToString() :
                                                           sortColumnIndex == 8 && isSortable_8 ? c.IsInstitution.ToString() :
                                                           sortColumnIndex == 9 && isSortable_9 ? c.ImportIDExcel.ToString() :

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
                , c.SalesInvoiceNo
                , c.CustomerName.ToString()
                , c.DeliveryAddress1.ToString()
                , c.InvoiceDateTime.ToString()             
                , c.TotalAmount.ToString()               
                , c.TotalVATAmount.ToString()               
                , c.Post=="Y" ? "Posted" : "Not Posted"
                , c.ImportIDExcel
                , c.TransactionType
                , c.IsInstitution=="Y" ? "Institution" : "Not Institution"

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
        public ActionResult BlankItem(SaleDetailVm vm)
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
            #region precalculation
            //////if (vm.SalesPrice > 0)
            //////{
            //////    vm.NBRPrice = vm.SalesPrice;
            //////}
            #endregion

            CommonRepo commonRepo = new CommonRepo(identity, Session);

            string code = commonRepo.settings("CompanyCode", "Code");

            bool IsEONCompany = code.ToLower() == "eon" || code.ToLower() == "purofood" || code.ToLower() == "eahpl" || code.ToLower() == "eail"
                                || code.ToLower() == "eeufl" || code.ToLower() == "exfl";

            #region Calculations



            if (vm.CurrencyRateFromBDT == 0) vm.CurrencyRateFromBDT = 1;

            vm.SalesPrice = vm.SalesPrice * vm.UOMc;
            vm.Quantity = vm.SaleQuantity + vm.PromotionalQuantity;
            vm.SubTotal = vm.NBRPrice * vm.Quantity;

            if (IsEONCompany)
            {
                if (string.IsNullOrWhiteSpace(vm.Option1) || vm.Option1 == "-")
                {
                    vm.Option1 = "0";
                }

                vm.SubTotal = vm.NBRPrice * vm.SaleQuantity;

                vm.SubTotal = vm.SubTotal - Convert.ToDecimal(vm.Option1);
            }

            vm.SDAmount = (vm.SubTotal * vm.SD) / 100;

            vm.VATAmount = ((vm.SubTotal + vm.SDAmount) * vm.VATRate) / 100;
            vm.TotalValue = vm.SubTotal + vm.VATAmount + vm.SDAmount;
            vm.Total = vm.TotalValue;

            vm.UOMQty = vm.UOMc * vm.Quantity;
            //////vm.UOMPrice = vm.UOMc * vm.NBRPrice;
            vm.UOMPrice = vm.NBRPrice - vm.DiscountAmount;

            vm.CurrencyValue = vm.CurrencyRateFromBDT * vm.SubTotal;
            //  vm.BDTValue = vm.Total;
            // vm.BDTValue = vm.CurrencyRateFromBDT * vm.SubTotal;

            vm.DollerValue = vm.CurrencyValue / vm.CurrencyRateFromBDT;

            vm.ValueOnly = "N";
            vm.HPSAmount = vm.SubTotal * vm.HPSRate / 100;

            if (vm.Rowtype == "New")
            {
                vm.CPCName = "-";
                vm.BEItemNo = "-";
            }

            CommercialImporterCalculation(vm);

            if (string.IsNullOrWhiteSpace(vm.BillingPeriodFrom))
            {
                vm.BillingPeriodFrom = vm.InvoiceDateTime;
            }
            if (string.IsNullOrWhiteSpace(vm.BillingPeriodTo))
            {
                vm.BillingPeriodTo = vm.InvoiceDateTime;
            }

            if (string.IsNullOrWhiteSpace(vm.BENumber))
            {
                vm.BENumber = "-";
            }

            #region VDS Amount

            decimal VDSAmountD = 0;
            var vVATRateForVDSRatio = new SymRepository.VMS.CommonRepo(identity, Session).settings("Sale", "VATRateForVDSRatio");
            var vVDSRatio = new SymRepository.VMS.CommonRepo(identity, Session).settings("Sale", "VDSRatio");
            VDSAmountD = vm.VATAmount;
            if (vm.VATRate == Convert.ToDecimal(vVATRateForVDSRatio) && Convert.ToDecimal(vVDSRatio) > 0)
            {
                VDSAmountD = vm.VATAmount / Convert.ToDecimal(vVDSRatio);
            }
            vm.VDSAmountD = VDSAmountD;

            vm.DiscountedNBRPrice = vm.NBRPrice;

            #endregion

            #endregion

            return PartialView("_detail", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult CPCNameUpdate(SaleDetailVm vm)
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

            if (vm.CPCName == "" || vm.CPCName == null)
            {
                vm.CPCName = "-";
            }

            return PartialView("_detail", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItemAdjustment(SaleDetailVm vm)
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
            #region precalculation
            if (vm.SalesPrice > 0)
            {
                vm.NBRPrice = vm.SalesPrice;
            }


            decimal Subtotal = 0;
            decimal SDAmount = 0;
            decimal VATAmount = 0;
            Subtotal = Convert.ToDecimal(vm.PreviousQuantity) * Convert.ToDecimal(vm.PreviousNBRPrice);
            SDAmount = (Subtotal * Convert.ToDecimal(vm.PreviousSD) / 100);
            VATAmount = ((Subtotal + SDAmount) * Convert.ToDecimal(vm.PreviousVATRate) / 100);
            vm.PreviousSubTotal = Subtotal;
            vm.PreviousVATAmount = VATAmount;
            vm.PreviousSDAmount = SDAmount;

            #endregion

            string[] cFields = { "Pr.ItemNo" };
            string[] cValues = { vm.ItemNo };
            ProductVM varProductVM = new ProductVM();
            varProductVM = new ProductRepo(identity, Session).SelectAll("0", cFields, cValues).FirstOrDefault();
            vm.ProductDescription = varProductVM.ProductDescription;

            if (string.IsNullOrWhiteSpace(varProductVM.ProductDescription)
                            || varProductVM.ProductDescription == "-"
                            || varProductVM.ProductDescription == "NA"
                            || varProductVM.ProductDescription == "N/A"
                            )
            {
                vm.ProductDescription = varProductVM.ProductName;
            }

            #region Calculations

            vm.SalesPrice = vm.SalesPrice * vm.UOMc;
            vm.SubTotal = vm.NBRPrice * vm.Quantity;
            ////vm.VATAmount = (vm.SubTotal * vm.VATRate) / 100;
            vm.SDAmount = vm.SubTotal * vm.SD / 100;
            vm.VATAmount = (vm.SubTotal + vm.SDAmount) * vm.VATRate / 100;

            vm.Total = vm.SubTotal + vm.VATAmount + vm.SDAmount;
            vm.UOMQty = vm.UOMc * vm.Quantity;
            vm.BDTValue = vm.Total;
            vm.CurrencyValue = vm.SubTotal;
            ////vm.ValueOnly = "N";

            CommercialImporterCalculation(vm);

            #endregion

            return PartialView("_detailAdjustment", vm);
        }

        private void CommercialImporterCalculation(SaleDetailVm vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            decimal cTotalValue = 0;
            decimal cQuantity = 0;
            decimal cATVablePrice = 0;
            decimal cATVAmount = 0;
            decimal cWareHouseRent = 0;
            decimal cWareHouseVAT = 0;
            decimal cVATRate = 0;
            decimal cVATablePrice = 0;
            decimal cVATAmount = 0;
            decimal cTradeVATRate = Convert.ToDecimal(133.34);
            decimal cWareHouseRentPerQuantity = Convert.ToDecimal(new SymRepository.VMS.CommonRepo(identity, Session).settings("Sale", "WareHouseRentPerQuantity"));
            decimal cATVRate = Convert.ToDecimal(new SymRepository.VMS.CommonRepo(identity, Session).settings("Sale", "ATVRate"));
            var CommercialImporter = new SymRepository.VMS.CommonRepo(identity, Session).settings("Sale", "CommercialImporter");

            if (CommercialImporter == "Y")
            {
                cTotalValue = Convert.ToDecimal(vm.TotalValue);
                cVATRate = Convert.ToDecimal(vm.VATRate);
                cQuantity = Convert.ToDecimal(vm.Quantity);
                cWareHouseRent = cWareHouseRentPerQuantity * cQuantity;
                cWareHouseVAT = cWareHouseRent * cVATRate / 100;
                cATVablePrice = (cTotalValue - (cWareHouseRent + cWareHouseVAT)) * 100 / (cATVRate + 100);
                cATVAmount = cATVablePrice * cATVRate / 100;
                cVATablePrice = (cATVablePrice * 100) / (cTradeVATRate);
                cVATAmount = cVATablePrice * cVATRate / 100;
            }
            vm.TotalValue = cTotalValue;
            vm.WareHouseRent = cWareHouseRent;
            vm.WareHouseVAT = cWareHouseVAT;
            vm.ATVRate = cATVRate;
            vm.ATVablePrice = cATVablePrice;
            vm.ATVAmount = cATVAmount;
            vm.IsCommercialImporter = CommercialImporter == "Y" ? "Y" : "N";
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItems(string saleNo, string InvoiceDate, bool SearchPreviousForCNDN = false)
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new SaleInvoiceRepo(identity, Session);


                string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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
                //List<SaleDetailVM> vms = _repo.SelectSaleDetail(saleNo);
                List<SaleDetailVm> vms = _repo.SearchSaleDetailDTNewList(saleNo, InvoiceDate, SearchPreviousForCNDN);
                return PartialView("_detailMultiple", vms);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);

                return RedirectToAction("Index");

            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create(string tType)
        {

            CommonRepo commonRepo = new CommonRepo(identity, Session);
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            string FormNumeric = commonRepo.settings("DecimalPlace", "FormNumeric");
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
            SaleMasterVM vm = new SaleMasterVM();

            List<SaleDetailVm> SaleDetailVMs = new List<SaleDetailVm>();
            vm.Details = SaleDetailVMs;
            vm.Operation = "add";
            vm.TransactionType = tType;

            vm.CurrencyID = "260";
            vm.CurrencyRateFromBDT = 1;

            #region FormMaker
            FormMaker(vm);
            #endregion

            vm.InvoiceDateTime = Session["SessionDate"].ToString();
            vm.ConversionDate = Session["SessionDate"].ToString();

            if (tType.ToLower() == "debit" || tType.ToLower() == "credit")
            {
                return View("CreateAdjustment", vm);
            }
            if (tType.ToLower() == "tollissue")
            {
                return View("CreateTollsale", vm);
            }

            CommonRepo _cRepo = new CommonRepo(identity, Session);
            vm.TrackingTrace = _cRepo.settingValue("TrackingTrace", "Tracking");
            vm.RefRequired = _cRepo.settingValue("Sale", "RefRequired");
            vm.MultipleItemInsert = _cRepo.settingValue("Sale", "MultipleItemInsert");
            vm.SaleDeliveryChallanTracking = _cRepo.settingValue("Sale", "SaleDeliveryChallanTracking");



            //vm.ConversionDate = DateTime.Now.ToString("dd-MMM-yyyy");
            vm.ShiftId = "1";
            vm.FormNumeric = FormNumeric;

            return View(vm);
        }

        private void FormMaker(SaleMasterVM vm)
        {
            #region Defoalt Settings Controll In From
            CommonRepo commonRepo = new CommonRepo(identity, Session);
            string ChassisTracking = commonRepo.settings("Sale", "ChassisTracking");
            string DefaultProductGroup = commonRepo.settings("Sale", "DefaultProductGroup");
            string DefaultProductType = commonRepo.settings("Sale", "DefaultProductType");
            string DefaultVATType = commonRepo.settings("DefaultVATType", "LocalSale");
            string ExpireDateTracking = commonRepo.settings("Purchase", "ExpireDateTracking");
            vm.ProductType = DefaultProductType.ToLower().ToUpperFirstLetter() == "" ? "Finish" : DefaultProductType.ToLower().ToUpperFirstLetter();
            vm.ProductGroup = DefaultProductGroup.ToLower().ToUpperFirstLetter() == "" ? "Finish" : DefaultProductGroup.ToLower().ToUpperFirstLetter();
            vm.ChassisTracking = ChassisTracking == "" ? "N" : ChassisTracking.ToUpper();
            var product = new ProductCategoryRepo(identity, Session).SelectAll(0, new string[] { "CategoryName" }, new string[] { vm.ProductGroup }).FirstOrDefault();
            if (product == null)
            {
                vm.ProductCategoryId = 0;
            }
            else
            {
                vm.ProductCategoryId = Convert.ToInt32(product.CategoryID);
            }
            vm.VatName = "VAT 4.3";

            vm.Type = DefaultVATType;
            vm.IsExpireDate = ExpireDateTracking;

            if (vm.Type == "NONVAT")
            {
                vm.Type = "NonVAT";
            }
            if (vm.Type == "FIXEDVAT")
            {
                vm.Type = "FixedVAT";
            }
            if (vm.Type == "OTHERRATE")
            {
                vm.Type = "OtherRate";
            }
            if (vm.Type == "RETAIL")
            {
                vm.Type = "Retail";
            }
            #endregion
            //vm.Type = "VAT";
            //vm.ProductType = "Finish";
            //vm.VatName = "VAT 4.3";


            switch (vm.TransactionType)
            {
                case "Other": break;
                case "Trading":
                    {
                        vm.ProductType = "Trading";
                        //vm.VatName = "VAT 1 Kha (Trading)";
                    }
                    break;
                case "Export":
                    {
                        vm.Type = "Export";
                        vm.ProductType = "Export";
                        //vm.VatName = "VAT 1 Ga (Export)";
                    }
                    break;
                case "Tender": break;
                case "TradingTender": break;
                case "Service":
                    {
                        vm.ProductType = "Service";
                    }
                    break;
                case "ServiceNS":
                    {
                        vm.ProductType = "Service";
                    }
                    break;
                case "RawSale":
                    {
                        vm.ProductType = "Raw";
                    }
                    break;

                case "SaleWastage":
                    {
                        vm.ProductType = "Raw";
                        vm.VatName = "VAT 4.3 (Wastage)";
                    }
                    break;

                case "InternalIssue":////Transfer
                    {
                        vm.ProductType = "Raw";
                        //vm.VatName = "VAT 1 (Internal Issue)";
                    }
                    break;
                case "PackageSale":////Package
                    {
                        //vm.VatName = "VAT 1 (Package)";
                    }
                    break;
                case "TollIssue":////Toll
                    {
                        vm.ProductType = "Raw";
                        vm.Type = "NonVAT";
                    }
                    break;
                case "VAT11GaGa":////Toll
                    {
                        vm.ProductType = "Raw";
                    }
                    break;
                case "TollFinishIssue":////Toll
                    {
                        vm.ProductType = "OverHead";
                        //vm.VatName = "VAT 1 (Toll Issue)";
                    }
                    break;

                case "Credit"://CN
                    {
                        vm.ProductType = "Finish";
                        vm.ProductGroup = "Finish";
                        vm.ProductCategoryId = 1;

                    }
                    break;

                default: break;
            }

        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateEdit(SaleMasterVM vm)
        {
            _repo = new SaleInvoiceRepo(identity, Session);

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

            vm.InvoiceDateTime = Convert.ToDateTime(vm.InvoiceDateTime).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            vm.DeliveryDate = Convert.ToDateTime(vm.DeliveryDate).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");

            CommonRepo commonRepo = new CommonRepo(identity, Session);

            string code = commonRepo.settings("CompanyCode", "Code");

            bool IsEONCompany = code.ToLower() == "eon" || code.ToLower() == "purofood" || code.ToLower() == "eahpl" || code.ToLower() == "eail"
                                || code.ToLower() == "eeufl" || code.ToLower() == "exfl";


            #region Check and Set

            if (vm.DeliveryAddress2 == null)
            {
                vm.DeliveryAddress2 = "-";
            }

            if (vm.DeliveryAddress3 == null)
            {
                vm.DeliveryAddress3 = "-";
            }
            if (vm.SerialNo == null)
            {
                vm.SerialNo = "-";
            }
            if (vm.Comments == null)
            {
                vm.Comments = "-";
            }


            if (vm.Trading == null)
            {
                vm.Trading = "N";
            }

            if (vm.IsPrint == null)
            {
                vm.IsPrint = "N";
            }

            if (vm.TenderId == null)
            {
                vm.TenderId = "0";
            }
            if (vm.LCNumber == null)
            {
                vm.LCNumber = "0";
            }

            if (vm.ReturnId == null)
            {
                vm.ReturnId = "80";
            }
            if (vm.LCBank == null)
            {
                vm.LCBank = " ";
            }
            if (vm.PreviousSalesInvoiceNo == null)
            {
                vm.PreviousSalesInvoiceNo = "0.00";
            }
            if (vm.PreviousSalesInvoiceNo == null)
            {
                vm.PreviousSalesInvoiceNo = "0.00";
            }
            if (vm.PONo == null)
            {
                vm.PONo = "-";
            }
            if (vm.SaleType == null)
            {
                vm.SaleType = "New";

            }

            if(vm.TransactionType!="Export")
            {
                vm.CurrencyID = "260";
                vm.CurrencyRateFromBDT = decimal.Parse("1.00");
             
            }

            #endregion

            UserInformationRepo _UserInformationRepo = new UserInformationRepo(identity, Session);
            UserInformationVM varUserInformationVM = new UserInformationVM();
            //varUserInformationVM = _UserInformationRepo.SelectAll(Convert.ToInt32(identity.UserId)).FirstOrDefault();

            ////DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + identity.UserId + "'");
            ////vm.SignatoryName = userInfo[0]["FullName"].ToString();
            ////vm.SignatoryDesig = userInfo[0]["Designation"].ToString();

            //vm.SignatoryName = varUserInformationVM.FullName;
            //vm.SignatoryDesig = varUserInformationVM.Designation;

            vm.SignatoryName = identity.FullName.Split('~')[0];
            vm.SignatoryDesig = identity.FullName.Split('~')[1];




            string SaleTypeD = "New";
            if (vm.TransactionType.ToLower() == "credit")
            {
                SaleTypeD = "Credit";
            }
            vm.SaleType = SaleTypeD;

            try
            {

                string UserId = identity.UserId;

                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.CreatedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.LastModifiedBy = identity.Name;
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                    ////vm.TransactionType = "Other";
                    vm.Post = "N";

                    string vlogString = "";

                    int i = 1;

                    foreach (SaleDetailVm vmD in vm.Details)
                    {
                        #region Check and Set

                        if (vm.PreviousSalesInvoiceNo != "0.00")
                        {
                            vmD.PreviousSalesInvoiceNoD = vm.PreviousSalesInvoiceNo;
                        }

                        if (vmD.CommentsD == null || vmD.CommentsD == "")
                        {
                            vmD.CommentsD = "NA";
                        }
                        if (vmD.PreviousSalesInvoiceNoD == null)
                        {
                            vmD.PreviousSalesInvoiceNoD = "0.00";
                        }
                        //if (vmD.TradingD == null)
                        //{
                        //    vmD.TradingD = "N";
                        //}
                        vmD.TradingD = "N";
                        if (vmD.NonStockD == null)
                        {
                            vmD.NonStockD = "N";
                        }

                        vmD.SaleTypeD = SaleTypeD;

                        if (string.IsNullOrWhiteSpace(vmD.BillingPeriodFrom))
                        {
                            vmD.BillingPeriodFrom = vm.InvoiceDateTime;
                        }

                        if (string.IsNullOrWhiteSpace(vmD.BillingPeriodTo))
                        {
                            vmD.BillingPeriodTo = vm.InvoiceDateTime;
                        }


                        #endregion

                        #region Adding Line No

                        vmD.InvoiceLineNo = i.ToString();
                        i++;

                        string logString = "ItemNo : " + vmD.ItemNo + "\n" + "ProductCode : " + vmD.ProductCode + "\n" + "UOM : " + vmD.UOM + "\n" + "UOMn : " + vmD.UOMn + "\n" + "UOMc : " + vmD.UOMc + "\n"
                       + "UOMQty : " + vmD.UOMQty + "\n" + "UOMPrice : " + vmD.UOMPrice;

                        vlogString += logString + "\n";

                        #endregion
                    }



                    List<SaleExportVM> ExportDetails = new List<SaleExportVM>();
                    List<TrackingVM> Trackings = new List<TrackingVM>();

                    result = _repo.SalesInsert(vm, vm.Details, ExportDetails, Trackings, null, null, "", vm.DeliveryTrackingDetails);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[4], TransactionType = vm.TransactionType });
                    }
                    else
                    {
                        string msg = result[1].Split('\r').FirstOrDefault();

                        Session["result"] = result[0] + "~" + msg; //result[1];

                        if (vm.TransactionType.ToLower() == "credit" || vm.TransactionType.ToLower() == "debit")
                        {
                            return View("CreateAdjustment", vm);
                        }

                        else
                        {
                            return View("Create", vm);//CreateAdjustment
                        }


                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                    string vlogString = "";

                    int j = 1;

                    foreach (SaleDetailVm vmD in vm.Details)
                    {
                        #region Check and Set
                        if (vm.PreviousSalesInvoiceNo != "0.00")
                        {
                            vmD.PreviousSalesInvoiceNoD = vm.PreviousSalesInvoiceNo;
                        }
                        ////vmD.PreviousSalesInvoiceNoD = vm.PreviousSalesInvoiceNo;

                        if (vmD.CommentsD == null || vmD.CommentsD == "")
                        {
                            vmD.CommentsD = "NA";
                        }
                        if (vmD.PreviousSalesInvoiceNoD == null)
                        {
                            vmD.PreviousSalesInvoiceNoD = "0.00";
                        }
                        if (vmD.TradingD == null)
                        {
                            vmD.TradingD = "N";
                        }
                        //if (vmD.NonStockD == null)
                        //{
                        //    vmD.NonStockD = "N";
                        //}
                        vmD.NonStockD = "N";
                        if (vm.DeliveryChallanNo == null)
                        {
                            vm.DeliveryChallanNo = "-";
                        }

                        if (string.IsNullOrWhiteSpace(vmD.BillingPeriodFrom))
                        {
                            vmD.BillingPeriodFrom = vm.InvoiceDateTime;
                        }

                        if (string.IsNullOrWhiteSpace(vmD.BillingPeriodTo))
                        {
                            vmD.BillingPeriodTo = vm.InvoiceDateTime;
                        }



                        vmD.SaleTypeD = SaleTypeD;

                        vmD.SubTotal = vmD.NBRPrice * vmD.Quantity;

                        if (IsEONCompany)
                        {
                            if (string.IsNullOrWhiteSpace(vmD.Option1) || vmD.Option1 == "-")
                            {
                                vmD.Option1 = "0";
                            }

                            vmD.SubTotal = vmD.NBRPrice * vmD.SaleQuantity;

                            vmD.SubTotal = vmD.SubTotal - Convert.ToDecimal(vmD.Option1);
                        }

                        vmD.CurrencyValue = vm.CurrencyRateFromBDT * vmD.SubTotal;
                        vmD.DollerValue = vmD.CurrencyValue / vm.CurrencyRateFromBDT;

                        #endregion

                        #region Adding Line No

                        vmD.InvoiceLineNo = j.ToString();
                        j++;

                        #endregion

                        string logString = " ItemNo : " + vmD.ItemNo + "\n" + " ProductCode : " + vmD.ProductCode + "\n" + " UOM : " + vmD.UOM + "\n" + " UOMn : " + vmD.UOMn + "\n" + " UOMc : " + vmD.UOMc + "\n"
                       + "UOMQty : " + vmD.UOMQty + "\n" + "UOMPrice : " + vmD.UOMPrice;

                        vlogString += logString + "\n";

                    }


                    List<SaleExportVM> ExportDetails = new List<SaleExportVM>();
                    List<TrackingVM> Trackings = new List<TrackingVM>();

                    result = _repo.SalesUpdate(vm, vm.Details, ExportDetails, Trackings, UserId, vm.DeliveryTrackingDetails);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.TransactionType });
                    }
                    else
                    {
                        string msg = result[1].Split('\r').FirstOrDefault();
                        Session["result"] = result[0] + "~" + msg; //result[1];

                        if (vm.TransactionType.ToLower() == "credit" || vm.TransactionType.ToLower() == "debit")
                        {
                            return View("CreateAdjustment", vm);
                        }
                        else
                        {
                            return View("Create", vm);
                        }

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

                ////string msg = "Fail~" + ex.Message;
                Session["result"] = "Fail~" + msg;

                ////Session["result"] = "Fail~Data not Successfully";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.TransactionType });
            }
        }

        [Authorize(Roles = "Admin")]
        public JsonResult getCABalance(string startDate)
        {
            DateTime startDateTime = DateTime.Now;
            try
            {
                startDateTime = DateTime.Parse(startDate);
            }
            catch (Exception)
            {
            }
            var repo = new ReportDSRepo(identity, Session);
            //var statusResult = repo.ResultVATStatus(startDateTime, "");
            //var caBalance = Convert.ToDecimal(statusResult.Tables[0].Rows[0][0]).ToString();//"0.00")
            var statusResult = repo.VAT18New(identity.Name, startDateTime.AddDays(1).ToString("yyyy-MM-dd"), startDateTime.AddDays(1).ToString("yyyy-MM-dd"), "Y", "Y");
            var caBalance = Convert.ToDecimal(statusResult.Tables[0].Rows[0]["StartingVAT"]).ToString(); //"0.00")
            return Json(caBalance, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        public JsonResult getVehicleDetails(string vehicleId)
        {
            var repo = new VehicleRepo(identity, Session);
            var id = 0;
            string[] conditionalFields;
            string[] conditionalValues;

            try
            {
                conditionalFields = new string[] { "VehicleNo" };
                conditionalValues = new string[] { vehicleId };

                //id = Convert.ToInt32(vehicleId);
            }
            catch (Exception)
            {
                throw;
            }
            //var vehicle = repo.SelectAll(id).FirstOrDefault();
            var vehicle = repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();
            return Json(vehicle, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        public JsonResult getCutomerDetails(string CustomerName)
        {
            var repo = new CustomerRepo(identity, Session);
            var id = 0;
            string[] conditionalFields;
            string[] conditionalValues;

            try
            {
                conditionalFields = new string[] { "c.CustomerName" };
                conditionalValues = new string[] { CustomerName };
            }
            catch (Exception)
            {
                throw;
            }
            var customer = repo.SelectAll(null, conditionalFields, conditionalValues).FirstOrDefault();
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Admin")]
        public JsonResult SelectProductDetails(string productCode, string IssueDate, string vatName, string TransactionType)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var _repo = new ProductRepo(identity, Session);
            CommonRepo commonRepo = new CommonRepo(identity, Session);
            string FixBOMReferenceName = commonRepo.settings("Sale", "FixBOMReferenceName");
            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };

            var product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();

            if (product == null)
            {
                conditionalFields = new string[] { "Pr.ItemNo" };
                conditionalValues = new string[] { productCode };
                product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
            }
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);
            Decimal salesPrice = 0;
            Decimal stock = 0;
            Decimal amount = 0;
            Decimal quantity = 0;
            //var name = product.ProductName;
            #region businessLogic

            string UserId = identity.UserId;

            var issueDatetime = DateTime.Parse(IssueDate).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            if (vatName == "VAT 1 (Wastage)")
            {
                stock = 0;
            }
            else
            {
                DataTable priceData = _repo.AvgPriceNew(product.ItemNo, issueDatetime, null, null, true, true, true, false, UserId);
                stock = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
            }
            if (!string.IsNullOrEmpty(FixBOMReferenceName) && FixBOMReferenceName != "-")
            {
                //Cavinkare BOMReference 
                salesPrice = _repo.CavinkareGetLastNBRPriceFromBOM(product.ItemNo, vatName, issueDatetime, FixBOMReferenceName, null, null);
            }
            else
            {
                salesPrice = _repo.GetLastNBRPriceFromBOM(product.ItemNo, vatName, issueDatetime, null, null);
            }
            salesPrice = Convert.ToDecimal(ParseDecimalObject(salesPrice));
            if (TransactionType == "TollIssue")
            {
                DataTable priceData = _repo.AvgPriceNew(product.ItemNo, issueDatetime, null, null, true, true, true, false, UserId);
                quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                stock = quantity;
                if (quantity > 0)
                {
                    salesPrice = (amount / quantity);
                }

            }
            #endregion businessLogic
            product.Stock = stock;
            product.SalesPrice = salesPrice;
            return Json(product, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult SelectCurrencyDetails(string ConversionDate)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var _repo = new CurrencyConversionRepo(identity, Session);

            string[] cValue = new string[] { "Y", Convert.ToDateTime(ConversionDate).ToString("yyyy-MMM-dd HH:mm:ss") };
            string[] cField = new string[] { "cc.ActiveStatus like", "cc.ConversionDate<=" };
            CurrencyConversionVM CurrencyConversionResult = _repo.SelectAll(0, cField, cValue, null, null).FirstOrDefault();

            return Json(CurrencyConversionResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUomOption(string uomFrom)
        {
            try
            {
                var _repo = new UOMRepo(identity, Session);
                string[] conditionalFields = new string[] { "UOMFrom" };
                string[] conditionalValues = new string[] { uomFrom };
                var uoms = _repo.SelectAll(0, conditionalFields, conditionalValues);

                #region Old Code

                ////var html = "";

                ////string uomF = OrdinaryVATDesktop.StringReplacingForHTML(uomFrom);

                ////html += "<option value='" + uomF + "'>" + uomF + "</option>";

                //////////if (uoms == null || uoms.Count == 0)
                //////////{
                //////////    string uomF = OrdinaryVATDesktop.StringReplacingForHTML(uomFrom);

                //////////    html += "<option value='" + uomF + "'>" + uomF + "</option>";
                //////////}
                
                //if (uoms != null || uoms.Count != 0)
                //{
                //    foreach (var item in uoms)
                //    {
                //        html += "<option value='" + item.UOMTo + "'>" + item.UOMTo + "</option>";

                //        ////html += "<option value=" + item.UOMTo + ">" + item.UOMTo + "</option>";
                //    }
                //}

                //////////html += "<option value='" + uomFrom + "'>" + uomFrom + "</option>";
                //////////foreach (var item in uoms)
                //////////{
                //////////    html += "<option value=" + item.UOMTo + ">" + item.UOMTo + "</option>";
                //////////}
                
                #endregion Old Code

                return Json(uoms, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                List<UOMConversionVM> VMs = new List<UOMConversionVM>();
                UOMConversionVM vm = new UOMConversionVM();
                vm.UOMFrom = uomFrom.ToString();
                vm.UOMTo = uomFrom.ToString();
                vm.Convertion = 1;
                VMs.Add(vm);

                return Json(VMs, JsonRequestBehavior.AllowGet);

            }
        }

        [ShampanAuthorize]
        [HttpGet]
        public ActionResult Edit(string id, string TransactionType)
        {
            SaleMasterVM vm = new SaleMasterVM();

            try
            {
                _repo = new SaleInvoiceRepo(identity, Session);

                CommonRepo commonRepo = new CommonRepo(identity, Session);
                CommonRepo _cRepo = new CommonRepo(identity, Session);

                string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
                string FormNumeric = commonRepo.settings("DecimalPlace", "FormNumeric");
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

                if (TransactionType == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                vm = _repo.SelectAll(Convert.ToInt32(id), null, null, null, null, null, TransactionType).FirstOrDefault();

                if (vm == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //DataRow row = GetRole(vm.TransactionType);

                //if (row["Access"].ToString() == "1")
                //{
                //    return RedirectToAction("Index");
                //}

                vm.ConversionDate = DateTime.Now.ToString("dd-MMM-yyyy");

                List<SaleDetailVm> SaleDetailVMs = new List<SaleDetailVm>();
                if (vm.TransactionType.ToLower() == "credit" || vm.TransactionType.ToLower() == "debit")
                {
                    vm.ProductType = "Finish";
                    vm.ProductGroup = "Finish";
                    vm.ProductCategoryId = 1;
                    SaleDetailVMs = _repo.SearchSaleDetailDTNewList(vm.SalesInvoiceNo, vm.InvoiceDateTime, false);
                }
                else
                {
                    SaleDetailVMs = _repo.SelectSaleDetail(vm.SalesInvoiceNo);
                }

                vm.Details = SaleDetailVMs;
                vm.Operation = "update";

                vm.ConversionDate = DateTime.Now.ToString("dd-MMM-yyyy");

                #region FormMaker
                FormMaker(vm);
                #endregion

                vm.FormNumeric = FormNumeric;

                if (vm.TransactionType.ToLower() == "credit" || vm.TransactionType.ToLower() == "debit")
                {
                    return View("CreateAdjustment", vm);
                }
                if (vm.TransactionType.ToLower() == "tollissue")
                {
                    return View("CreateTollsale", vm);
                }

                vm.TrackingTrace = _cRepo.settingValue("TrackingTrace", "Tracking");
                vm.RefRequired = _cRepo.settingValue("Sale", "RefRequired");
                vm.SaleDeliveryChallanTracking = _cRepo.settingValue("Sale", "SaleDeliveryChallanTracking");

                List<SaleDeliveryTrakingVM> SaleDeliveryVMs = new List<SaleDeliveryTrakingVM>();
                if (vm.SaleDeliveryChallanTracking == "Y")
                {
                    SaleDeliveryVMs = _repo.SelectSaleDeliveryTrakings(vm.SalesInvoiceNo);
                }
                vm.DeliveryTrackingDetails = SaleDeliveryVMs;


                return View("Create", vm);
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);

                return RedirectToAction("Index");
            }
        }

        [HttpGet, Authorize]
        public ActionResult SelectSale(string refNo)
        {
            try
            {
                _repo = new SaleInvoiceRepo(identity, Session);
                SaleDAL saleDal = new SaleDAL();

                SaleMasterVM vm = _repo.SelectAll(0, new[] { "sih.SerialNo" }, new[] { refNo.Replace(" ", "") })
                    .FirstOrDefault();
                Session["saleIntegration"] = null;

                return RedirectToAction("Edit", new { id = vm.Id });
            }
            catch (Exception e)
            {

                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult GetSaleInvoices()
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
            var _repo = new SaleInvoiceRepo(identity, Session);

            var vm = new List<SaleMasterVM>();

            vm = _repo.SelectAll(0);

            return PartialView("_saleInvoice", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ShowSaleInvoice(string SaleNo)
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
            var _repo = new SaleInvoiceRepo(identity, Session);

            var vm = new SaleMasterVM();
            string[] conditionalFields = new string[] { "SalesInvoiceNo" };
            string[] conditionalValues = new string[] { SaleNo };
            vm = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();
            vm.Operation = "detail";

            return PartialView("Create", vm);
        }

        public ActionResult Navigate(string id, string btn, string ttype)
        {
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetIdForTtype("SalesInvoiceHeaders", "Id", id, btn, ttype);
            return RedirectToAction("Edit", new { id = targetId, TransactionType = ttype });
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
        [HttpGet]
        public ActionResult GetItemPopUp(string targetId, string TransactionType)
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
            vm.TransactionType = TransactionType;
            vm.TargetId = targetId;
            return PartialView("_salePopUp", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredItems(SaleMasterVM vm)
        {
            _repo = new SaleInvoiceRepo(identity, Session);

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

            string BranchId = Session["BranchId"].ToString();

            string[] conditionalFields;
            string[] conditionalValues;
            if (vm.SearchField != null)
            {
                string seachField = "sih." + vm.SearchField;
                if (vm.SearchField == "VehicleNo")
                {
                    seachField = "v.VehicleNo";
                }
                conditionalFields = new string[] { seachField, "sih.InvoiceDateTime>", "sih.InvoiceDateTime<", "sih.Post", "sih.IsPrint", "sih.CustomerID", "sih.SaleType", "sih.BranchId", "sih.IsBillCompleted" };
                conditionalValues = new string[] { vm.SearchValue, vm.InvoiceDateTimeFrom, vm.InvoiceDateTimeTo, vm.Post, vm.IsPrint, vm.CustomerID, vm.SaleType, BranchId, vm.IsBillCompleted };
            }
            else
            {
                conditionalFields = new string[] { "sih.InvoiceDateTime>", "sih.InvoiceDateTime<", "sih.Post", "sih.IsPrint", "sih.CustomerID", "sih.SaleType", "sih.BranchId", "sih.IsBillCompleted" };
                conditionalValues = new string[] { vm.InvoiceDateTimeFrom, vm.InvoiceDateTimeTo, vm.Post, vm.IsPrint, vm.CustomerID, vm.SaleType, BranchId, vm.IsBillCompleted };
            }
            var list = _repo.SelectAll(0, conditionalFields, conditionalValues);

            return PartialView("_filteredSales", list);
        }

        #region Post

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            _repo = new SaleInvoiceRepo(identity, Session);
            UserInformationRepo _UserInformationRepo = new UserInformationRepo(identity, Session);
            UserInformationVM varUserInformationVM = new UserInformationVM();
            string[] a = ids.Split('~');
            var id = a[0];
            SaleMasterVM vm = new SaleMasterVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            List<SaleDetailVm> SaleDetailVMS = new List<SaleDetailVm>();
            List<TrackingVM> TrackingVM = new List<TrackingVM>();

            SaleDetailVMS = _repo.SelectSaleDetail(vm.SalesInvoiceNo);
            TrackingVM = _repo.SelectTrakingsDetail(SaleDetailVMS, vm.SalesInvoiceNo, null);

            vm.Details = SaleDetailVMS;
            vm.Trackings = TrackingVM;


            //varUserInformationVM = _UserInformationRepo.SelectAll(Convert.ToInt32(identity.UserId)).FirstOrDefault();
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);

            //vm.SignatoryName = varUserInformationVM.FullName;
            //vm.SignatoryDesig = varUserInformationVM.Designation;
            vm.SignatoryName = identity.FullName.Split('~')[0];
            vm.SignatoryDesig = identity.FullName.Split('~')[1];
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";
            //result = _repo.SalesPost(vm, vm.Details, new List<TrackingVM>());
            result = _repo.SalesPost(vm, vm.Details, vm.Trackings);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult MultipleSalePost(SaleMasterVM paramVM)
        {
            #region Access Control
            _repo = new SaleInvoiceRepo(identity, Session);
            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            #endregion

            ResultVM rVM = new ResultVM();

            try
            {
                OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);

                paramVM.CurrentUser = identity.UserId;

                if (paramVM != null && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    paramVM.IDs = paramVM.IDs.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

                    if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                    {
                        rVM.Message = "No Data to Post";
                        return Json(rVM, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    rVM.Message = "No Data to Post";
                    return Json(rVM, JsonRequestBehavior.AllowGet);
                }

                rVM = _repo.Multiple_SalePost(paramVM);

            }
            catch (Exception)
            {


            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [Authorize]
        public ActionResult GetEngine_ACI(SaleEngineChassisVM vm)
        {
            _repo = new SaleInvoiceRepo(identity, Session);

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

            string BranchId = Session["BranchId"].ToString();

            string[] conditionalFields;
            string[] conditionalValues;

            conditionalFields = new string[] { "SE.ItemNo", "SE.SalesInvoiceNo" };
            conditionalValues = new string[] { vm.ItemNo, vm.SalesInvoiceNo };

            var list = _repo.SelectAllEngineList(0, conditionalFields, conditionalValues);

            return PartialView("_filteredEngine", list);
        }

        [Authorize]
        public ActionResult GetEngineCredit_ACI(SaleEngineChassisVM vm)
        {
            _repo = new SaleInvoiceRepo(identity, Session);

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

            string BranchId = Session["BranchId"].ToString();

            string[] conditionalFields;
            string[] conditionalValues;

            conditionalFields = new string[] { "SE.ItemNo", "SE.SalesInvoiceNo" };
            conditionalValues = new string[] { vm.ItemNo, vm.SalesInvoiceNo };

            var list = _repo.SelectAllEngineList(0, conditionalFields, conditionalValues);

            if (list == null || list.Count == 0)
            {
                conditionalFields = new string[] { "SE.ItemNo", "SE.SalesInvoiceNo" };
                conditionalValues = new string[] { vm.ItemNo, vm.PSalesInvoiceNo };

                list = _repo.SelectAllEngineList(0, conditionalFields, conditionalValues);

            }


            return PartialView("_filteredEngine", list);
        }

        // put it in another vm
        [Authorize]
        [HttpPost]
        public ActionResult MultipleEngineRemove(SaleEngineChassisVM vm)
        {
            #region Access Control
            _repo = new SaleInvoiceRepo(identity, Session);
            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            #endregion

            ResultVM rVM = new ResultVM();

            try
            {
                rVM = _repo.Multiple_EngineRemove(vm);

            }
            catch (Exception)
            {


            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EngineChange_Credit(SaleEngineChassisVM vm)
        {
            #region Access Control
            _repo = new SaleInvoiceRepo(identity, Session);
            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            #endregion

            ResultVM rVM = new ResultVM();

            try
            {
                vm.BranchCode = Session["BranchCode"].ToString();
                rVM = _repo.EngineChange_Credit(vm);

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
            }

            return Json(rVM, JsonRequestBehavior.AllowGet);

        }


        [Authorize]
        [HttpPost]
        public ActionResult EngineAdd(SaleEngineChassisVM vm)
        {
            #region Access Control
            _repo = new SaleInvoiceRepo(identity, Session);

            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            #endregion

            ResultVM rVM = new ResultVM();

            try
            {
                rVM = _repo.InsertEngine(vm);

            }
            catch (Exception)
            {


            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult ExportExcell(SaleMasterVM paramVM)
        {
            #region Access Control
            _repo = new SaleInvoiceRepo(identity, Session);

            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            #endregion

            ResultVM rVM = new ResultVM();
            DataTable dt = new DataTable();

            List<SaleMasterVM> getAllData = new List<SaleMasterVM>();

            try
            {

                #region SeachParameters

                string dtFrom = null;
                string dtTo = null;

                if (string.IsNullOrWhiteSpace(paramVM.SearchField))
                {
                    dtFrom = DateTime.Now.ToString("yyyyMMdd");
                    dtTo = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
                    if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeFrom))
                    {
                        dtFrom = Convert.ToDateTime(paramVM.InvoiceDateTimeFrom).ToString("yyyyMMdd");
                    }
                    if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeTo))
                    {
                        dtTo = Convert.ToDateTime(paramVM.InvoiceDateTimeTo).AddDays(1).ToString("yyyyMMdd");
                    }
                }

                if (paramVM.BranchId == 0)
                {
                    paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (paramVM.BranchId == -1)
                {
                    paramVM.BranchId = 0;
                }

                ////if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
                ////{
                ////    paramVM.SelectTop = "All";
                ////}

                paramVM.SelectTop = "All";

                #endregion SeachParameters

                #region Data Call

                string[] conditionFields;
                string[] conditionValues;
                if (string.IsNullOrWhiteSpace(paramVM.SearchField))
                {
                    conditionFields = new string[] { "sih.InvoiceDateTime>=", "sih.InvoiceDateTime<=", "sih.SaleType", "sih.CustomerID", "sih.IsPrint", "sih.Post", "sih.BranchId", "sih.IsInstitution", "SelectTop", "sih.TransactionType" };
                    conditionValues = new string[] { dtFrom, dtTo, paramVM.SaleType, paramVM.CustomerID, paramVM.IsPrint, paramVM.Post, paramVM.BranchId.ToString(), paramVM.IsInstitution, paramVM.SelectTop, paramVM.TransactionType };

                }
                else
                {
                    if (paramVM.SearchField == "VehicleNo")
                    {
                        paramVM.SearchField = "v.VehicleNo like";
                    }
                    else
                    {
                        paramVM.SearchField = "sih." + paramVM.SearchField + " like";
                    }

                    conditionFields = new string[] { "sih.InvoiceDateTime>=", "sih.InvoiceDateTime<=", "sih.SaleType", "sih.CustomerID", "sih.IsPrint", "sih.Post", paramVM.SearchField, "sih.BranchId", "sih.IsInstitution", "SelectTop", "sih.TransactionType" };
                    conditionValues = new string[] { dtFrom, dtTo, paramVM.SaleType, paramVM.CustomerID, paramVM.IsPrint, paramVM.Post, paramVM.SearchValue, paramVM.BranchId.ToString(), paramVM.IsInstitution, paramVM.SelectTop, paramVM.TransactionType };


                }

                if (paramVM.ExportAll)
                {
                    conditionFields = new string[] { "sih.InvoiceDateTime>=", "sih.InvoiceDateTime<=", "sih.SaleType", "sih.CustomerID", "sih.IsPrint", "sih.Post", "sih.BranchId", "sih.IsInstitution", "sih.TransactionType", "SelectTop" };
                    conditionValues = new string[] { dtFrom, dtTo, paramVM.SaleType, paramVM.CustomerID, paramVM.IsPrint, paramVM.Post, paramVM.BranchId.ToString(), paramVM.IsInstitution, paramVM.TransactionType, paramVM.SelectTop };
                    
                    //getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
                    //paramVM.IDs = getAllData.Select(x => x.SalesInvoiceNo).ToList();

                    paramVM.IDs = new List<string>(0);
                    dt = _repo.GetSaleExcelDataWeb(paramVM.IDs, conditionFields, conditionValues);
                }
              

                #endregion

                paramVM.CurrentUser = identity.UserId;

                if (paramVM != null && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    paramVM.IDs = paramVM.IDs.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

                    if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                    {
                        rVM.Message = "No Data to Export";
                        return Json(rVM, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        dt = _repo.GetSaleExcelDataWeb(paramVM.IDs, conditionFields, conditionValues);
                    }
                }
                else if (dt.Rows.Count == 0)
                {
                    rVM.Message = "No Data to Export";
                    return Json(rVM, JsonRequestBehavior.AllowGet);
                }

                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                 ////OrdinaryVATDesktop.SaveExcel(dt, "Sale", "SaleM");
                //var vm = OrdinaryVATDesktop.DownloadExcel(dt, "Sale", "SaleM");

                if (dt.Rows.Count > 0)
                {
                    // Create a new Excel package
                    using (var package = new ExcelPackage())
                    {
                        // Add a new worksheet to the package
                        var worksheet = package.Workbook.Worksheets.Add("Sale");

                        // Load data to the worksheet
                        worksheet.Cells["A1"].LoadFromDataTable(dt, true);

                        // Set the content type and headers for the response
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment; filename=Sale" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx");

                        // Stream the file to the response
                        Response.BinaryWrite(package.GetAsByteArray());
                        Response.Flush();
                        Response.End();
                    }
                }

                //int totalRow = 0;
                //DataTable firstdt = new DataTable();
                //DataTable skipdt = new DataTable();
                //DataTable skipdt2 = new DataTable();

                //if (dt.Rows.Count > 300000)
                //{
                //    totalRow = dt.Rows.Count / 3;

                //    var takeRows = dt.AsEnumerable().Take(totalRow);
                //    // Create a new DataTable with the remaining rows
                //    firstdt = takeRows.CopyToDataTable();

                //    var skipRows = dt.AsEnumerable().Skip(totalRow);
                //    // Create a new DataTable with the next set of rows after skipping
                //    skipdt = skipRows.Take(totalRow).CopyToDataTable();

                //    var skip2Rows = dt.AsEnumerable().Skip(totalRow * 2);
                //    // Create a new DataTable with the remaining rows
                //    skipdt2 = skip2Rows.CopyToDataTable();
                //}

                //string fileName = "Sale-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
                
                //if (dt.Rows.Count > 0)
                //{
                //    using (var package = new ExcelPackage())
                //    {
                //        var worksheet = package.Workbook.Worksheets.Add("SaleM");

                //        if (dt.Rows.Count > 300000 && firstdt.Rows.Count > 0)
                //        {
                //            worksheet.Cells["A1"].LoadFromDataTable(firstdt, true);

                //            // Save the first part
                //            package.SaveAs(new FileInfo("C:\\Users\\User\\Downloads\\" + fileName));
                //        }
                //        else
                //        {
                //            worksheet.Cells["A1"].LoadFromDataTable(dt, true);

                //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //            Response.AddHeader("content-disposition", "attachment; filename=" + fileName + "");

                //            // Stream the file to the response
                //            Response.BinaryWrite(package.GetAsByteArray());
                //            Response.Flush();
                //            Response.End();
                //        }
                //    }
                //}

                //if (skipdt.Rows.Count > 0)
                //{
                //    using (var existingPackage = new ExcelPackage(new FileInfo("C:\\Users\\User\\Downloads\\" + fileName)))
                //    {
                //        var worksheet = existingPackage.Workbook.Worksheets[0];

                //        // Load data to the worksheet
                //        worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].LoadFromDataTable(skipdt, true);

                //        // Save the modifications back to the same file
                //        existingPackage.Save();
                //    }
                //}

                //if (skipdt2.Rows.Count > 0)
                //{
                //    using (var existingPackage = new ExcelPackage(new FileInfo("C:\\Users\\User\\Downloads\\" + fileName)))
                //    {
                //        var worksheet = existingPackage.Workbook.Worksheets[0];

                //        // Load data to the worksheet
                //        worksheet.Cells[worksheet.Dimension.End.Row + 1, 1].LoadFromDataTable(skipdt2, true);

                //        // Save the modifications back to the same file
                //        existingPackage.Save();
                //    }
                //}

                //if (dt.Rows.Count > 0)
                //{
                //    rVM.Message = "Please find the file within download folder.";
                //    return Json(rVM, JsonRequestBehavior.AllowGet);
                //}


                // previous Code
                //using (var memoryStream = new MemoryStream())
                //{
                //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //    Response.AddHeader("content-disposition", "attachment;  filename=" + vm.FileName + ".xlsx");
                //    vm.varExcelPackage.SaveAs(memoryStream);
                //    memoryStream.WriteTo(Response.OutputStream);
                //    Response.Flush();
                //    Response.End();
                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }

            finally { }

            return RedirectToAction("Index");
        }

        static string GetRowsSubsetText(DataTable sourceTable, int startRow, int endRow)
        {
            // Generate a text representation of the subset of rows
            StringBuilder textBuilder = new StringBuilder();

            for (int i = startRow; i < endRow; i++)
            {
                DataRow row = sourceTable.Rows[i];
                for (int j = 0; j < sourceTable.Columns.Count; j++)
                {
                    if (j > 0)
                    {
                        textBuilder.Append("\t");
                    }
                    textBuilder.Append(row[j]);
                }
                textBuilder.AppendLine();
            }

            return textBuilder.ToString();
        }


        public JsonResult GetSaleMaster(string saleNo)
        {
            _repo = new SaleInvoiceRepo(identity, Session);

            string[] conditionalFields = new string[] { "sih.SalesInvoiceNo" };
            string[] conditionalValues = new string[] { saleNo };

            SaleMasterVM vm = _repo.SelectAll(0, conditionalFields, conditionalValues).SingleOrDefault();


            ////string result = vm.VehicleID + "~" + vm.VehicleNo + "~" + vm.CustomerID + "~" + vm.CustomerName;

            return Json(vm, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ImportExcel(SaleMasterVM vm)
        {
            string[] result = new string[6];
            try
            {
                _repo = new SaleInvoiceRepo(identity, Session);

                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                vm.BranchCode = Session["BranchCode"].ToString();
                result = _repo.ImportExcelIntegrationFile(vm);
                Session["result"] = result[0] + "~" + result[1];
                return Json(JsonConvert.SerializeObject(new { message = "Saved Successfully", action = result[0] }),
                    JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                Session["result"] = result[0] + "~" + result[1];
                //Session["result"] = "Fail" + "~" + e.Message.Replace("\r", "").Replace("\n", "");

                Elmah.ErrorSignal.FromCurrentContext().Raise(e);

                return Json(JsonConvert.SerializeObject(new { message = e.Message, action = "Fail" }),
                    JsonRequestBehavior.AllowGet);



            }

        }


        [Authorize]
        [HttpPost]
        public ActionResult ProcessExcel(SaleMasterVM vm)
        {
            string[] result = new string[6];

            try
            {
                _repo = new SaleInvoiceRepo(identity, Session);

                if (vm.File == null)
                    return Json(JsonConvert.SerializeObject(new { message = "Please select a file", action = "fail" }),
                        JsonRequestBehavior.AllowGet);


                string token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + Session["LogInUserId"];
                Session["currentExcelToken"] = token;

                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                vm.BranchCode = Session["BranchCode"].ToString();

                vm.Token = token;
                result = _repo.ImportExcelFile(vm);

                return Json(JsonConvert.SerializeObject(new { message = "Saved to temp", action = result[0] }),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;

                Elmah.ErrorSignal.FromCurrentContext().Raise(e);

                //Session["result"] = "fail" + "~" + "Something gone wrong";
                return Json(JsonConvert.SerializeObject(new { message = "Please Check your session", action = "fail" }),
                    JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveTransactions(SaleMasterVM vm)
        {
            string[] result = new string[6];

            try
            {

                _repo = new SaleInvoiceRepo(identity, Session);
                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                vm.Token = Session["currentExcelToken"].ToString();
                result = _repo.SaveTransactions(vm);

                return Json(JsonConvert.SerializeObject(new { message = "Transaction Saved", action = result[0] }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                //Session["result"] = "fail" + "~" + "Something gone wrong";
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Json(JsonConvert.SerializeObject(new { message = "Please Check your session", action = "fail" }),
                    JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult ProductDscripition()
        {

            return View();

        }

        public ActionResult GetEXPFormNoPopUp()
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
            return PartialView("_ExpNoPopUp");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Searchtrip(string tripNo)
        {
            _repo = new SaleInvoiceRepo(identity, Session);

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

            DataTable dt = new DataTable();

            CommonDAL commonDal = new CommonDAL();

            //String saleFromProduction = commonDal.settingsDesktop("Sale", "SaleFromProduction");
            String saleFromProduction = commonDal.settings("Sale", "SaleFromProduction");
            try
            {

                if (saleFromProduction.ToLower() == "y")
                {
                    _repo = new SaleInvoiceRepo(identity, Session);


                    dt = _repo.SearchByReferenceNo(tripNo);

                    if (dt.Rows.Count <= 0)
                    {
                        return Json(new { productName = "", code = 0, quantity = 0, message = "Trip Not Found" }, JsonRequestBehavior.AllowGet);

                    }

                    string IsTripComplete = dt.Rows[0]["IsTripComplete"].ToString();

                    if (IsTripComplete.ToLower() == "y")
                    {
                        return Json(new { productName = "", code = 0, quantity = 0, message = "This Ref/Trip # Already Used." }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new
                    {
                        productName = dt.Rows[0]["ProductName"].ToString(),
                        code = dt.Rows[0]["ProductCode"].ToString(),
                        quantity = dt.Rows[0]["Quantity"].ToString()
                    }, JsonRequestBehavior.AllowGet);


                }

                return Json(new { productName = "", code = 0, quantity = 0 }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                return Json(new { productName = "", code = 0, quantity = 0 }, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult PopUpGetEXP()
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
            return PartialView("PopUpGetEXP");
        }

        public ActionResult GetFilteredExp(SaleMasterVM vm)
        {
            _repo = new SaleInvoiceRepo(identity, Session);

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




            var list = _repo.SalesInvoiceExpsLoad();

            List<SaleMasterVM> ExpsVMS = new List<SaleMasterVM>();


            foreach (SalesInvoiceExpVM Expsvm in list)
            {
                SaleMasterVM mVM = new SaleMasterVM();

                DateTime EXPFormDate = Convert.ToDateTime(Expsvm.EXPDate);
                DateTime LCDate = Convert.ToDateTime(Expsvm.LCDate);
                DateTime PIDate = Convert.ToDateTime(Expsvm.PIDate);

                mVM.EXPFormDate = EXPFormDate.ToString("dd-MMM-yyyy");
                mVM.LCNumber = Expsvm.LCNumber;
                mVM.LCBank = Expsvm.LCBank;
                mVM.LCDate = LCDate.ToString("dd-MMM-yyyy");
                mVM.PINo = Expsvm.PINo;
                mVM.PIDate = PIDate.ToString("dd-MMM-yyyy");
                mVM.SerialNo = Expsvm.EXPNo;

                ExpsVMS.Add(mVM);

            }

            return PartialView("_filteredExp", ExpsVMS);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetSaleIntegration(PopUpViewModel vm)
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


            return PartialView("_SaleIntegration", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetSaleData(SaleIntegrationViewModel vm)
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

            CommonRepo commonRepo = new CommonRepo(identity, Session);

            string code = commonRepo.settings("CompanyCode", "Code");

            DataTable dtTableResult = null;

            BCLIntegrationRepo integration = new BCLIntegrationRepo(identity, Session);

            IntegrationParam integrationParam = new IntegrationParam()
            {
                RefNo = vm.RefNo,
                FromDate = string.IsNullOrEmpty(vm.FromDate) ? "" : Convert.ToDateTime(vm.FromDate).ToString("yyyy-MMM-dd"),
                ToDate = string.IsNullOrEmpty(vm.ToDate) ? "" : Convert.ToDateTime(vm.ToDate).ToString("yyyy-MMM-dd"),
                DefaultBranchCode = Session["BranchCode"].ToString(),
                Top = "1000"
            };

            Session["saleIntegration"] = integrationParam;//JsonConvert.SerializeObject(integrationParam)

            dtTableResult = integration.LoadData(integrationParam);

            //string salesJson = JsonConvert.SerializeObject(dtTableResult);

            ////Session["saleIntegration"] = salesJson;

            ////var vms = JsonConvert.DeserializeObject<List<SaleIntegrationViewModel>>(salesJson);

            //JsonResult result = Json(salesJson, JsonRequestBehavior.AllowGet);
            //result.MaxJsonLength = Int32.MaxValue;
            //return result;

            return PartialView("_integrationBody", dtTableResult);
        }


        [Authorize]
        public ActionResult SaveSale(string transactoinType, SaleIntegrationViewModel vm)
        {
            #region Auth

            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            #endregion

            CommonRepo commonRepo = new CommonRepo(identity, Session);

            DataTable dtTableResult = null;
            string[] result = { "fail" };

            IntegrationParam integrationParam = new IntegrationParam()
            {
                RefNo = vm.RefNo,
                FromDate = string.IsNullOrEmpty(vm.FromDate) ? "" : Convert.ToDateTime(vm.FromDate).ToString("yyyy-MMM-dd"),
                ToDate = string.IsNullOrEmpty(vm.ToDate) ? "" : Convert.ToDateTime(vm.ToDate).ToString("yyyy-MMM-dd"),
                DefaultBranchCode = Session["BranchCode"].ToString(),
                Top = "0"
            };

            BCLIntegrationRepo integration = new BCLIntegrationRepo(identity, Session);
            dtTableResult = integration.LoadData(integrationParam);



            string value = commonRepo.settings("SaleWeb", "Setps");


            if (value == "N")
            {
                result = integration.SaveSale(dtTableResult, new IntegrationParam());

                return Json(new { Message = "Successfully Saved", action = result[0], type = "n" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + Session["LogInUserId"];
                Session["currentExcelToken"] = token;

                result = integration.SaveToTemp(dtTableResult, integrationParam.DefaultBranchId,
                    integrationParam.TransactionType, token);

                return Json(new { Message = "Saved to temp", action = result[0], type = "Y" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize, HttpPost]
        public ActionResult SetSession(string key, string value = null)
        {
            Session[key] = value;

            return Json(Session[key], JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult GetSettings(string group, string name)
        {
            try
            {
                CommonRepo commonRepo = new CommonRepo(identity, Session);

                if (!string.IsNullOrEmpty(group) && !string.IsNullOrEmpty(name))
                {
                    string value = commonRepo.settings(group, name);

                    return Json(value.ToLower(), JsonRequestBehavior.AllowGet);
                }

                return Json("N", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("N", JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize(Roles = "Admin")]
        public JsonResult getCustomerAddress(string CustomerId)
        {
            var repo = new CustomerRepo(identity, Session);
            try
            {
                var CustomerAddress = repo.SelectAll(CustomerId).FirstOrDefault();
                return Json(CustomerAddress, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);


                throw;
            }

        }

        [Authorize(Roles = "Admin")]
        public JsonResult getNBRPriceByBOMReferenceNo(string BOMReferenceNo, string ItemNo, string IssueDate, string vatName)
        {
            var repo = new ProductRepo(identity, Session);
            try
            {
                CommonRepo commonRepo = new CommonRepo(identity, Session);

                string FormNumeric = commonRepo.settings("DecimalPlace", "FormNumeric");
                string NBRPrice = "";
                string BOMId = "";
                var issueDatetime = DateTime.Parse(IssueDate).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                DataTable dt = repo.GetBOMReferenceNo(ItemNo, vatName, issueDatetime);

                DataView dv = new DataView(dt);
                dv.RowFilter = "ReferenceNo = '" + BOMReferenceNo + "'";

                DataTable dtBOM = new DataTable();
                dtBOM = dv.ToTable();
                if (dtBOM != null && dtBOM.Rows.Count > 0)
                {
                    DataRow dr = dtBOM.Rows[0];
                    BOMId = dr["BOMId"].ToString();
                    ////NBRPrice = Convert.ToDecimal(dr["NBRPrice"]).ToString("0.0000");
                    NBRPrice = Convert.ToDecimal(dr["NBRPrice"]).ToString();

                }
                NBRPrice = OrdinaryVATDesktop.FormatingNumeric(NBRPrice, FormNumeric);

                return Json(new { NBRPrice, BOMId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }

        }


        public ActionResult PopUpPreviousSales(SaleDetailVm vm)
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
            return PartialView("_PopUpPreviousSales", vm);
        }

        public ActionResult PopUpCPCNameUpdateSales(SaleDetailVm vm)
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
            return PartialView("_PopUpCPCNameupdateSales", vm);
        }

        [Authorize]
        public ActionResult SyncAll()
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            // _repo = new ProductRepo(identity);
            _repo = new SaleInvoiceRepo(identity, Session);

            try
            {
                string[] results = new string[4];

                ImportRepo importrepo = new ImportRepo(identity, Session);
                CommonRepo commonrepo = new CommonRepo(identity, Session);
                results[0] = "fail";

                IntegrationParam Param = new IntegrationParam();
                Param.BranchId = Session["BranchId"].ToString();
                ResultVM pVM = new ResultVM();

                #region Product
                ProductRepo pRepo = new ProductRepo(identity, Session);
                pVM = pRepo.IntegrationSyncProducts(Param);
                #endregion

                #region Customer
                CustomerRepo cRepo = new CustomerRepo(identity, Session);
                pVM = cRepo.IntegrationSyncCustomers(Param);
                #endregion

                #region vendor
                VendorRepo vRepo = new VendorRepo(identity, Session);
                pVM = vRepo.IntegrationSyncVendors(Param);
                #endregion
                #region old Product customer vendoer

                #region Old Products
                //List<ProductVM> products = new List<ProductVM>();
                //int rowsCount = productDt.Rows.Count;
                //List<string> ids = new List<string>();

                //string defaultGroup = commonrepo.settings("AutoSave", "DefaultProductCategory");

                //for (int i = 0; i < rowsCount; i++)
                //{
                //    ProductVM product = new ProductVM();

                //    product.ProductName = Ordinary.RemoveStringExpresion(productDt.Rows[i]["ProductName"].ToString());
                //    product.ProductDescription = productDt.Rows[i]["Description"].ToString();
                //    product.CategoryName = productDt.Rows[i]["ProductGroup"].ToString();

                //    if (product.CategoryName == "-")
                //    {
                //        product.CategoryName = defaultGroup;
                //    }

                //    product.UOM = productDt.Rows[i]["UOM"].ToString();

                //    product.NBRPrice = Convert.ToDecimal(productDt.Rows[i]["UnitPrice"].ToString());

                //    product.SerialNo = "-";
                //    product.HSCodeNo = productDt.Rows[i]["HSCode"].ToString();
                //    product.VATRate = Convert.ToDecimal(productDt.Rows[i]["VATRate"].ToString());
                //    product.Comments = "-";
                //    product.ActiveStatus = "Y";
                //    product.SD = Convert.ToDecimal(productDt.Rows[i]["SDRate"].ToString());
                //    product.Packetprice = 0;
                //    product.Trading = "N";
                //    product.TradingMarkUp = 0;
                //    product.NonStock = "N"; ;
                //    product.OpeningDate = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                //    product.CreatedBy = OrdinaryVATDesktop.CurrentUser;
                //    product.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                //    product.ProductCode = Ordinary.RemoveStringExpresion(productDt.Rows[i]["ProductCode"].ToString());
                //    product.TollCharge = 0;
                //    product.BranchId = OrdinaryVATDesktop.BranchId;
                //    product.IsConfirmed = "N";

                //    if (code.ToLower() == "cepl")
                //    {
                //        product.UOM = "pcs";
                //        product.VATRate = 15;
                //    }

                //    products.Add(product);

                //    ids.Add(productDt.Rows[i]["SL"].ToString());
                //}

                //results = importrepo.ImportProductSync(products, new List<TrackingVM>());

                //if (results[0].ToLower() == "success")
                //{
                //    results = importrepo.UpdateACIMaster(ids, settingVM.BranchInfoDT, "Products");

                //}
                #endregion

                #region Old Customers
                //DataTable customerDt = importrepo.GetCustomerACIDbData(settingVM.BranchInfoDT);
                //List<CustomerVM> customers = new List<CustomerVM>();

                //int rowsCount1 = customerDt.Rows.Count;
                //List<string>  ids = new List<string>();

                //string defaultCustomerGroup = commonrepo.settings("AutoSave", "DefaultCustomerGroup");

                //for (int i = 0; i < rowsCount1; i++)
                //{
                //    CustomerVM customer = new CustomerVM();

                //    customer.CustomerName =
                //        Ordinary.RemoveStringExpresion(customerDt.Rows[i]["CustomerName"].ToString());
                //    customer.CustomerCode =
                //        Ordinary.RemoveStringExpresion(customerDt.Rows[i]["CustomerCode"].ToString());
                //    customer.CustomerGroup = customerDt.Rows[i]["CustomerGroup"].ToString();
                //    customer.Address1 = customerDt.Rows[i]["Address"].ToString();

                //    ////if (customer.CustomerGroup == "-")
                //    ////{
                //    ////    customer.CustomerGroup = defaultGroup1;
                //    ////}
                //    if (customer.CustomerGroup == "-")
                //    {
                //        if (defaultCustomerGroup == "-")
                //        {
                //            throw new Exception("Default Customer Group Not Found. Please set Default Customer Group in Setting .");
                //        }
                //        customer.CustomerGroup =defaultCustomerGroup;
                //    }

                //    customer.City = "-";
                //    customer.TelephoneNo = "-";
                //    customer.FaxNo = "-";
                //    customer.Email = "-";
                //    customer.StartDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                //    customer.ContactPerson = "-";
                //    customer.ContactPersonDesignation = "-";
                //    customer.ContactPersonTelephone = "-";
                //    customer.ContactPersonEmail = "-";
                //    ;
                //    customer.TINNo = "-";
                //    ;
                //    customer.VATRegistrationNo = customerDt.Rows[i]["BIN_No"].ToString();
                //    customer.Comments = "-";
                //    customer.ActiveStatus = "Y";
                //    customer.CreatedBy = OrdinaryVATDesktop.CurrentUser; // need to change
                //    customer.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                //    customer.Country = "-";
                //    customer.IsVDSWithHolder = "N";
                //    customer.BranchId = OrdinaryVATDesktop.BranchId; // need to change
                //    customer.IsInstitution = "N";
                //    customers.Add(customer);

                //    ids.Add(customerDt.Rows[i]["SL"].ToString());
                //}



                //results = importrepo.ImportCustomer(customers);

                //if (results[0].ToLower() == "success")
                //{
                //    results = importrepo.UpdateACIMaster(ids, settingVM.BranchInfoDT, "Customers");

                //}
                #endregion

                #region Old Vendors
                //DataTable vendorsDt = importrepo.GetVendorACIDbData(settingVM.BranchInfoDT);

                //List<VendorVM> vendors = new List<VendorVM>();


                //int rowsCount = vendorsDt.Rows.Count;
                //List<string> ids = new List<string>();

                //string defaultVendorGroup = commonrepo.settings("AutoSave", "DefaultVendorGroup");

                ////if(defaultGroup == "-")
                ////{
                ////    MessageBox.Show("Default Vendor Group Not Found");
                ////}

                //for (int i = 0; i < rowsCount; i++)
                //{
                //    VendorVM vendor = new VendorVM();
                //    vendor.VendorCode = Ordinary.RemoveStringExpresion(vendorsDt.Rows[i]["VendorCode"].ToString());
                //    vendor.VendorName = Ordinary.RemoveStringExpresion(vendorsDt.Rows[i]["VendorName"].ToString());

                //    vendor.VendorGroup = vendorsDt.Rows[i]["VendorGroup"].ToString();

                //    if (vendor.VendorGroup == "-")
                //    {
                //        if (defaultVendorGroup == "-")
                //        {
                //            throw new Exception("Default Vendor Group Not Found.\nPlease set Default Vendor Group in Setting .");
                //        }
                //        vendor.VendorGroup = defaultVendorGroup;
                //    }

                //    vendor.Address1 = vendorsDt.Rows[i]["Address"].ToString();
                //    vendor.Address2 = "-";
                //    vendor.Address3 = "-";

                //    vendor.City = "-";
                //    vendor.TelephoneNo = vendorsDt.Rows[i]["TelephoneNo"].ToString();
                //    vendor.FaxNo = "-";
                //    vendor.Email = "-";

                //    vendor.StartDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                //    vendor.ContactPerson = "-";
                //    vendor.ContactPersonDesignation = "-";
                //    vendor.ContactPersonTelephone = "-";
                //    vendor.ContactPersonEmail = "-";
                //    vendor.VATRegistrationNo = vendorsDt.Rows[i]["BIN_No"].ToString();
                //    vendor.TINNo = "-";
                //    vendor.Comments = "-";
                //    vendor.ActiveStatus = "Y";
                //    vendor.CreatedBy = OrdinaryVATDesktop.CurrentUser;
                //    vendor.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                //    vendor.Country = "-";
                //    vendor.BranchId = OrdinaryVATDesktop.BranchId;
                //    vendors.Add(vendor);

                //    ids.Add(vendorsDt.Rows[i]["SL"].ToString());
                //}


                //results = importrepo.ImportVendor(vendors);

                //if (results[0].ToLower() == "success")
                //{
                //    results = importrepo.UpdateACIMaster(ids, settingVM.BranchInfoDT);

                //}
                #endregion
                #endregion

                if (pVM.Status.ToLower() == "Success")
                {
                    Session["result"] = "Success~Successfully Synchronized";
                    return Redirect("/Vms/SaleInvoice/Index?TransactionType=other");
                }
                else
                {
                    Session["result"] = "Fail~Nothing to Syncronized";
                    return Redirect("/Vms/SaleInvoice/Index?TransactionType=other");
                }
            }
            catch (Exception ex)
            {
                // Session["result"] = "Fail~"+ ex.Message;
                string msg = "Fail~" + ex.Message;
                Session["result"] = msg;
                //Session["result"] = "Fail~Fail";
                return Redirect("/Vms/SaleInvoice/Index?TransactionType=other");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddEngine(SaleEngineChassisVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new SaleInvoiceRepo(identity, Session);
            CommonRepo commonRepo = new CommonRepo(identity, Session);

            string code = commonRepo.settings("CompanyCode", "Code");

            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();
            vm.CompanyCode = code;
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

            return PartialView("EngineCreate", vm);
        }

        private void StaticValueReAssign(ShampanIdentity identity)
        {
            try
            {
                #region Get Company Information
                CompanyProfileRepo _CompanyProfileRepo = new CompanyProfileRepo(identity);
                CompanyProfileVM varCompanyProfileVM = new CompanyProfileVM();
                string CompanyId = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, identity.CompanyId);
                varCompanyProfileVM = _CompanyProfileRepo.SelectAll(CompanyId).FirstOrDefault();
                OrdinaryVATDesktop.CompanyName = varCompanyProfileVM.CompanyLegalName;
                OrdinaryVATDesktop.Address1 = varCompanyProfileVM.Address1;
                OrdinaryVATDesktop.Address2 = varCompanyProfileVM.Address2;
                OrdinaryVATDesktop.Address3 = varCompanyProfileVM.Address3;
                OrdinaryVATDesktop.TelephoneNo = varCompanyProfileVM.TelephoneNo;
                OrdinaryVATDesktop.FaxNo = varCompanyProfileVM.FaxNo;
                OrdinaryVATDesktop.VatRegistrationNo = varCompanyProfileVM.VatRegistrationNo;
                OrdinaryVATDesktop.Section = varCompanyProfileVM.Section;
                #endregion

                #region Get Branch Information
                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity);
                BranchProfileVM varBranchProfileVM = new BranchProfileVM();
                varBranchProfileVM = branchProfileRepo.SelectAll(Convert.ToString(Session["BranchId"])).FirstOrDefault();
                OrdinaryVATDesktop.IsWCF = varBranchProfileVM.IsWCF;
                OrdinaryVATDesktop.BranchId = varBranchProfileVM.BranchID;
                #endregion
                OrdinaryVATDesktop.CurrentUser = Convert.ToString(Session["LogInUserName"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        public ActionResult PopUpBillingPeriod(SaleDetailVm vm)
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
            return PartialView("_PopUpBillingPeriod", vm);
        }

        public JsonResult GetTotalDays(string FromDate, string ToDate)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            int tDays = 0;

            DateTime BillingPeriodFrom = Convert.ToDateTime(FromDate);
            DateTime BillingPeriodTo = Convert.ToDateTime(ToDate);

            TimeSpan Totaldays = (BillingPeriodTo) - (BillingPeriodFrom.AddDays(-1));
            double NrOfDays = Totaldays.TotalDays;
            tDays = Convert.ToInt32(NrOfDays);

            return Json(tDays, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetTrakingsNo(string ProductCode, string cDate, string SaleInvoiceNo, string tType)
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
            TrackingVM vm = new TrackingVM();
            List<TrackingVM> vms = new List<TrackingVM>();

            var repo = new ProductRepo(identity, Session);
            var _repo = new SaleInvoiceRepo(identity, Session);

            if (tType.ToLower() == "other")
            {
                tType = "Sale";

            }

            ////string[] conditionalFields = new string[] { "Pr.ProductCode" };
            ////string[] conditionalValues = new string[] { ProductCode };
            ////var product = repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();

            DataTable TrackingRawsDt = _repo.FindTrackingItems(ProductCode, "VAT 4.3", cDate);

            for (int i = 0; i < TrackingRawsDt.Rows.Count; i++)
            {
                string rawItemNo = TrackingRawsDt.Rows[i]["RawItemNo"].ToString();
                vms = _repo.SelectTrakingsDetail(rawItemNo, "N", SaleInvoiceNo, tType);
            }

            //if(vms.Count<=0 || vms= null )
            //{
            //}


            return PartialView("_detailTrakings", vms);
        }

        [Authorize]
        [HttpPost]
        public ActionResult TrakingsUpdate(TrackingVM vm)
        {
            #region Access Control
            _repo = new SaleInvoiceRepo(identity, Session);
            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            #endregion

            ResultVM rVM = new ResultVM();
            List<TrackingVM> vms = new List<TrackingVM>();

            try
            {
                string sInvoice = "";
                decimal qty = 0;
                decimal engQty = 0;
                string itmno = "";

                foreach (var item in vm.IDs)
                {
                    TrackingVM Trakingsvm = new TrackingVM();
                    if (item.transactionType.ToLower() == "other")
                    {
                        Trakingsvm.transactionType = "Sale";

                    }

                    Trakingsvm.ItemNo = item.ItemNo;
                    Trakingsvm.Heading1 = item.Heading1;
                    Trakingsvm.IsSale = item.IsSale;
                    Trakingsvm.SaleInvoiceNo = item.SaleInvoiceNo;
                    Trakingsvm.FinishItemNo = item.FinishItemNo;

                    sInvoice = Trakingsvm.SaleInvoiceNo;
                    itmno = Trakingsvm.FinishItemNo;

                    engQty += 1;

                    vms.Add(Trakingsvm);


                }

                var SaleDetailsvm = new SaleDetailVm();
                string[] conditionalFields = new string[] { "sd.SalesInvoiceNo", "sd.ItemNo" };
                string[] conditionalValues = new string[] { sInvoice, itmno };
                SaleDetailsvm = _repo.SelectSaleDetail(null, conditionalFields, conditionalValues).FirstOrDefault();
                qty = SaleDetailsvm.Quantity;

                if (engQty != qty)
                {

                    rVM.Message = "Fail" + "~" + "Please select " + qty + " iteams for finish products";

                    return Json(rVM, JsonRequestBehavior.AllowGet);

                }

                //vm.BranchCode = Session["BranchCode"].ToString();
                string result = _repo.TrackingUpdate(vms, null, null, null);
                rVM.Message = result;

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
            }

            return Json(rVM, JsonRequestBehavior.AllowGet);

        }

        public string ParseDecimalObject(object numb)
        {

            String val = "0";
            string a = "str123";
            string numbField = numb.ToString();



            try
            {


                if (string.IsNullOrWhiteSpace(numbField.ToString()))
                {
                    numbField = "0";
                }
                else
                {
                    numbField = numbField.ToString().Replace(",", "");
                }
                CommonRepo commonRepo = new CommonRepo(identity, Session);

                string FormNumeric = commonRepo.settings("DecimalPlace", "FormNumeric");

                string Pre = "";
                Pre = Pre.PadRight(Convert.ToInt32(FormNumeric), '#');
                val = decimal.Parse(numbField.ToString(), System.Globalization.NumberStyles.Float).ToString("#,###0." + Pre);

            }
            catch { }
            return val;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItemDekiveryTraking(SaleDeliveryTrakingVM vm)
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
            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { vm.ProductCode };

            var product = PRepo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
            vm.ItemNo = product.ItemNo;
            vm.SubTotal = vm.Quantity * vm.UnitPrice;
            ////List<SaleDeliveryTrakingVM> vms = new List<SaleDeliveryTrakingVM>();
            ////vms.Add(vm);

            return PartialView("_deliveryTrakingsdetail", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaleDeliveryAdd(SaleDeliveryTrakingVM vm)
        {
            #region Access Control
            _repo = new SaleInvoiceRepo(identity, Session);
            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            #endregion

            ResultVM rVM = new ResultVM();
            string[] result = new string[6];
            //List<TrackingVM> vms = new List<TrackingVM>();

            try
            {
                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                //vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                result = _repo.DeliveryChallanInsert(vm, null, null);

                rVM.Status = result[0];
                rVM.Message = result[1];

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
            }

            return Json(rVM, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        [HttpPost]
        public ActionResult SaleDeliveryUpdate(SaleDeliveryTrakingVM vm)
        {
            #region Access Control
            _repo = new SaleInvoiceRepo(identity, Session);
            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            #endregion

            ResultVM rVM = new ResultVM();
            string[] result = new string[6];
            //List<TrackingVM> vms = new List<TrackingVM>();

            try
            {
                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                //vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                result = _repo.DeliveryChallanUpdate(vm, null, null);

                rVM.Status = result[0];
                rVM.Message = result[1];


            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
            }

            return Json(rVM, JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "Admin")]
        public ActionResult PostDeleveryChallan(string ids)
        {
            _repo = new SaleInvoiceRepo(identity, Session);
            ResultVM rVM = new ResultVM();
            string[] a = ids.Split('~');
            var id = a[0];
            SaleDeliveryTrakingVM vm = new SaleDeliveryTrakingVM();
            vm.DeliveryChallanNo = id;
            //vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            List<SaleDeliveryTrakingVM> SaleDeliveryDetailVMs = new List<SaleDeliveryTrakingVM>();
            SaleDeliveryDetailVMs = _repo.SelectSaleDeliveryTrakings(vm.DeliveryChallanNo);
            vm.IDs = SaleDeliveryDetailVMs;
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";
            ParameterVM pVm = new ParameterVM();
            pVm.InvoiceNo = vm.DeliveryChallanNo;
            rVM = _repo.PostDeliveryChallan(pVm);

            result[1] = rVM.Message;
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }



    }
}
