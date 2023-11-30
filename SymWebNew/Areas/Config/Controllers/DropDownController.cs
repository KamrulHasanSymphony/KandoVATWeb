
//using SymRepository.Common;
using SymRepository.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SymOrdinary;
using System.Threading;

using System.Web.Configuration;
using System.Configuration;
using SymRepository.VMS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using VATViewModel.DTOs;


namespace SymVATWebUI.Areas.Config.Controllers
{
    public class DropDownController : Controller
    {

        #region

        public JsonResult VATReturnPartName()
        {
            return Json(new SelectList(new EnumRepo().VATReturnPartName(), "Value", "Key"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult VATReturnValue(string PartName)
        {
            return Json(new SelectList(new EnumRepo().VATReturnValue(PartName), "Value", "Key"), JsonRequestBehavior.AllowGet);
        }


        #endregion

        public JsonResult GetSettingValue(string SettingGroup, string SettingName)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            CommonRepo _cRepo = new CommonRepo(identity, Session);
            string SettingValue = _cRepo.settingValue(SettingGroup, SettingName);

            return Json(SettingValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TransferReceiveColumnSearch()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new TransferReceiveRepo(identity).GetTransferReceiveColumn(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult TransferIssueColumnSearch()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new TransferIssueRepo(identity).GetTransferIssueColumn(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DepositType()
        {
            return Json(new SelectList(new EnumRepo().GetDepositTypeList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult TreasuryDepositType()
        {
            return Json(new SelectList(new EnumRepo().GetTreasuryDepositTypeList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SDDepositType()
        {
            return Json(new SelectList(new EnumRepo().GetSDDepositTypeList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DropDownBOMReferenceNo(string itemNo, string VatName, string effectDate, string CustomerID)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new ProductRepo(identity).DropDownBOMReferenceNo(itemNo, VatName, effectDate, CustomerID), "ReferenceNo", "ReferenceNo"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PurchaseVATType()
        {
            return Json(new SelectList(new EnumRepo().GetPurchaseVATTypeList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PurchaseVATTypeWeb()
        {
            return Json(new SelectList(new EnumRepo().GetPurchaseVATTypeListWeb(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnumVAT()
        {
            return Json(new SelectList(new EnumRepo().GetEnumVATList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecordSelectList()
        {
            return Json(new SelectList(new EnumRepo().RecordSelectList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnumVATExport()
        {
            return Json(new SelectList(new EnumRepo().GetEnumVATExportList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DecisionList()
        {
            return Json(new SelectList(new EnumRepo().DecisionList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DefaultCurrencyList()
        {
            List<SelectListItem> currencies = new List<SelectListItem>();
            currencies.Add(new SelectListItem() { Text = "BDT", Value = "260" });
            currencies.Add(new SelectListItem() { Text = "USD", Value = "249" });
            return Json(new SelectList(currencies, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AdjTypeList()
        {
            List<SelectListItem> AdjType = new List<SelectListItem>();
            AdjType.Add(new SelectListItem() { Text = "DecreasingAdjustment", Value = "DecreasingAdjustment" });
            AdjType.Add(new SelectListItem() { Text = "IncreasingAdjustment", Value = "IncreasingAdjustment" });
            return Json(new SelectList(AdjType, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SalesColumnSearch()
        {
            return Json(new SelectList(new SaleInvoiceRepo().GetSalesColumn(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReceiveColumnSearch()
        {
            return Json(new SelectList(new ReceiveRepo().GetReceiveColumn(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllHSCode()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new HSCodeRepo(identity).SelectAll(), "Id", "Code"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTrasferColumn()
        {
            return Json(new SelectList(new TransferRepo().GetTrasferColumn(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PurchaseColumnSearch()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new PurchaseRepo(identity).GetPurchaseColumn(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CustomerColumnSearch()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new CustomerRepo(identity).GetCustomerColumn(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult MPLSalesColumnSearch()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SaleInvoiceRepo(identity).MPLSalesColumnSearch(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CRInfoColumnSearch()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SaleMPLInvoiceRepo(identity).GetCRInfoColumn(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult VendorColumnSearch()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new VendorRepo(identity).GetVendorColumn(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductColumnSearch()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new ProductRepo(identity).GetProductColumn(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult HSCodeColumnSearch()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new HSCodeRepo(identity).GetHSCodeColumn(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Shift()
        {
            return Json(new SelectList(new ShiftRepo().DropDown(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CPCName()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new CPCDetailsRepo(identity).DropDown("Sale"), "Code", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CPCNamePurchase()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new CPCDetailsRepo(identity).DropDown("Purchase"), "Code", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CPCNameType()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

           // return Json(new SelectList(new CPCDetailsRepo(identity).DropDownType(), "Code", "Type"), JsonRequestBehavior.AllowGet);
            List<SelectListItem> Type = new List<SelectListItem>();
            Type.Add(new SelectListItem() { Text = "Sale", Value = "Sale" });
            Type.Add(new SelectListItem() { Text = "Purchase", Value = "Purchase" });
            return Json(new SelectList(Type, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PriceDeclaration()
        {
            return Json(new SelectList(new EnumRepo().GetPriceDeclarationList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult MasterTableList()
        {
            return Json(new SelectList(new EnumRepo().MasterTableList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Types()
        {
            return Json(new SelectList(new EnumRepo().GetTypeList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult TransferIsuueTypes()
        {
            return Json(new SelectList(new EnumRepo().GetTransferIsuueTypes(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult CompanyName()
        {
            string CompanyList = "All";
            try
            {
                CompanyList = new AppSettingsReader().GetValue("CompanyList", typeof(string)).ToString();
            }
            catch (Exception ex)
            {
                CompanyList = "All";
            }

            return Json(new SelectList(new SymRepository.VMS.SymphonyVATSysCompanyInformationRepo().DropDown(CompanyList), "Id", "Name"), JsonRequestBehavior.AllowGet);

        }

        public JsonResult BranchProfile()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            var list = new SymRepository.VMS.BranchRepo(identity).DropDownBranchProfile();

            list.Add(new BranchProfileVM(){BranchID=-1,BranchName="All"});

            return Json(new SelectList(list, "BranchID", "BranchName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UserBranchProfile()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            int userId = Convert.ToInt32(identity.UserId);
            var list = new SymRepository.VMS.BranchRepo(identity).UserDropDownBranchProfile(userId);

            var listBranch = new SymRepository.VMS.BranchRepo(identity).SelectAll();

            if(list.Count()==listBranch.Count())
            {
                list.Add(new BranchProfileVM() { BranchID = -1, BranchName = "All" });

            }

            return Json(new SelectList(list, "BranchID", "BranchName"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult OtherBranch()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var branchs = new SymRepository.VMS.BranchRepo(identity).DropDownBranchProfile();

            var currentBranch = Convert.ToInt32(Session["BranchId"]);
            branchs = branchs.Where(x => x.BranchID != currentBranch).ToList();

            return Json(new SelectList(branchs, "BranchID", "BranchName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult TanksList(string ItemNo)
        {
            var currentBranch = Convert.ToInt32(Session["BranchId"]);
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var tanks = new SymRepository.VMS.TanksMPLRepo(identity).DropDown(currentBranch, ItemNo);

            return Json(new SelectList(tanks, "Id", "TankCode"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AdjustmentName()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new AdjustmentRepo(identity).DropDown(), "AdjId", "AdjName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Purchase()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new SymRepository.VMS.PurchaseRepo(identity).DropDown(), "PurchaseInvoiceNo", "PurchaseInvoiceNo"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult BankInformations()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.BankInformationRepo(identity).DropDown(), "BankID", "BankName"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MPLBDBankInformations()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.MPLBDBankInformationRepo(identity).DropDown(), "BankID", "BankName"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MPLSelfBankInformations()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.MPLSelfBankInformationRepo(identity).DropDown(), "BankID", "BankName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalesInvoiceNo()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.MPLBankDepositSlipRepo(identity).DropDown(), "Id", "SalesInvoiceNo"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ZoneProfile()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.MPLZoneProfileRepo(identity).DropDown(), "ZoneID", "ZoneName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Customer()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new SymRepository.VMS.CustomerRepo(identity).DropDown(), "CustomerID", "CustomerName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DropDownByCustomerID(int CustomerID = 0)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new SymRepository.VMS.CustomerRepo(identity).DropDownByCustomerID(CustomerID), "CustomerID", "CustomerName"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult DropDownCustomerByGroup(string groupId)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new SymRepository.VMS.CustomerRepo(identity).DropDownByGroup(groupId), "CustomerID", "CustomerName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CustomerGroup()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new SymRepository.VMS.CustomerGroupRepo(identity).DropDown(), "CustomerGroupID", "CustomerGroupName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult BranchDropdown()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.BranchRepo(identity).DropDown(), "DBName", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Users()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new SymRepository.VMS.UserInformationRepo(identity).DropDown(), "UserName", "UserName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductCategory(string IsRaw)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new SymRepository.VMS.ProductCategoryRepo(identity).DropDown(IsRaw), "CategoryID", "CategoryName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductTypes()
        {
            return Json(new SelectList(new EnumRepo().productTypeList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductTransferTypes()
        {
            return Json(new SelectList(new EnumRepo().productTransferTypeList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SettingGroup()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.SettingRepo(identity).DropDownAll(), "SettingGroup", "SettingGroup"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SettingGroupMaster()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.SettingMasterRepo(identity).DropDownSettingMaster(), "SettingGroup", "SettingGroup"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrefixGroup()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.PrefixRepo(identity).DropDownAll(), "CodeGroup", "CodeGroup"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult FiscalYear()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.FiscalYearRepo(identity).DropDownAll(), "CurrentYear", "CurrentYear"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult FiscalPeriod()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new FiscalYearRepo(identity).DropDown(), "PeriodName", "PeriodName"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Product(string CategoryID = "0", string IsRaw = "")
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.ProductRepo(identity).DropDown(Convert.ToInt32(CategoryID), IsRaw), "ItemNo", "ProductName"), JsonRequestBehavior.AllowGet);
        }

        public JToken SelectProducts(string CategoryID = "0", string IsRaw = "")
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            try 
            {
                var selectList = new SelectList(new SymRepository.VMS.ProductRepo(identity).DropDown(Convert.ToInt32(CategoryID), IsRaw), "ItemNo", "ProductName");
                Response.StatusCode = 200;
                var json = JsonConvert.SerializeObject(selectList);
                return JArray.Parse(json);
            }
            catch (Exception e)
            {
                Response.StatusCode = 400;
                return JObject.Parse("{}");
            }
        }

        public JsonResult DropDownProductByCategory(string id)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.ProductRepo(identity).DropDownProductByCategory(id), "ProductCode", "ProductName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductName()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.ProductRepo(identity).DropDown(), "ProductCode", "ProductName"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EnumModeOfPayment(string type)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new SymRepository.VMS.SaleMPLInvoiceRepo(identity).DropDown(type), "Code", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnumVehicleType()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            return Json(new SelectList(new SymRepository.VMS.SaleMPLInvoiceRepo(identity).DropDownEnumVehicleType(), "Code", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Font()
        {
            var fontSize = new List<object>();


            for (int i = 5; i <= 15; i++)
            {
                fontSize.Add(new { value = i, text = i });
            }

            return Json(new SelectList(fontSize, "value", "text"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReportDecimal()
        {
            var Decimal = new List<object>();


            for (int i = 0; i <= 10; i++)
            {
                Decimal.Add(new { value = i, text = i });
            }

            return Json(new SelectList(Decimal, "value", "text"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult StockReportType()
        {
            List<SelectListItem> ReportType = new List<SelectListItem>();
            ReportType.Add(new SelectListItem() { Text = "Report_1", Value = "Report_1" });
            ReportType.Add(new SelectListItem() { Text = "Report_2", Value = "Report_2" });
            ReportType.Add(new SelectListItem() { Text = "Report_3", Value = "Report_3" });
            ReportType.Add(new SelectListItem() { Text = "Report_4", Value = "Report_4" });

            return Json(new SelectList(ReportType, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Language()
        {
            var language = new List<object>();

            language.Add(new { value = "Y", text = "Y" });
            language.Add(new { value = "N", text = "N" });

            /**
             for (int i = 5; i <= 15; i++)
             {
                 fontSize.Add(new { value = i, text = i });
             }
            */
            return Json(new SelectList(language, "value", "text"), JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetTDSCode()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new TDSRepo(identity).SelectAll(), "Id", "Code"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ShiftName()
        {
            return Json(new SelectList(new SymRepository.VMS.ShiftRepo().DropDown(), "Id", "ShiftName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportType()
        {
            List<SelectListItem> Export = new List<SelectListItem>();
            Export.Add(new SelectListItem() { Text = "Export", Value = "Export" });
            Export.Add(new SelectListItem() { Text = "DeemExport", Value = "DeemExport" });
            return Json(new SelectList(Export, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult BOMRef()
        {
            List<SelectListItem> BOMRef = new List<SelectListItem>();
            BOMRef.Add(new SelectListItem() { Text = "NA", Value = "1" });
            return Json(new SelectList(BOMRef, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult adjType()
        {

            List<SelectListItem> Type = new List<SelectListItem>();
            Type.Add(new SelectListItem() { Text = "DevelopmentSurcharge", Value = "DevelopmentSurcharge" });
            Type.Add(new SelectListItem() { Text = "EnvironmentProtectionSurcharge", Value = "EnvironmentProtectionSurcharge" });
            Type.Add(new SelectListItem() { Text = "ExciseDuty", Value = "ExciseDuty" });
            Type.Add(new SelectListItem() { Text = "FineOrPenalty", Value = "FineOrPenalty" });
            Type.Add(new SelectListItem() { Text = "FinePenaltyForNonSubmissionOfReturn", Value = "FinePenaltyForNonSubmissionOfReturn" });
            Type.Add(new SelectListItem() { Text = "HelthCareSurcharge", Value = "HelthCareSurcharge" });
            Type.Add(new SelectListItem() { Text = "ICTDevelopmentSurcharge", Value = "ICTDevelopmentSurcharge" });
            Type.Add(new SelectListItem() { Text = "InterestOnOveredSD", Value = "InterestOnOveredSD" });
            Type.Add(new SelectListItem() { Text = "InterestOnOveredVAT", Value = "InterestOnOveredVAT" });
            Type.Add(new SelectListItem() { Text = "WithoutBankPay", Value = "WithoutBankPay" });

            return Json(new SelectList(Type, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult adjid()
        {
            List<SelectListItem> adjid = new List<SelectListItem>();
            adjid.Add(new SelectListItem() { Text = "DDB", Value = "1" });
            return Json(new SelectList(adjid, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LineNo()
        {
            List<SelectListItem> line = new List<SelectListItem>();
            line.Add(new SelectListItem() { Text = "LineA", Value = "LineA" });
            line.Add(new SelectListItem() { Text = "LineB", Value = "LineB" });
            line.Add(new SelectListItem() { Text = "LineC", Value = "LineC" });
            return Json(new SelectList(line, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }


        #region Archive


        public JsonResult IdNameDropdown(string tableName, string Id, string Name, string AllData, string code)
        {
            return Json(new SelectList(new SymRepository.VMS.CommonRepo().IdNameDropdown(tableName, Id, Name, AllData, code), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult IdNameTtype(string tableName, string Id, string Name, string type, string code)
        {
            return Json(new SelectList(new SymRepository.VMS.CommonRepo().IdNameTtype(tableName, Id, Name, type, code), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult IdNameDropdownOverhead(string Id, string Name, string AllData, string code)
        {
            return Json(new SelectList(new SymRepository.VMS.CommonRepo().IdNameDropdownOverhead(Id, Name, AllData, code), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductByType(string type)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.ProductRepo(identity).GetProductByType(type), "ProductCode", "ProductName"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductCodeByType(string type)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.ProductRepo(identity).GetProductByType(type), "ProductCode", "ProductCode"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Currency()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.CurrencyRepo(identity).DropDown(), "CurrencyId", "CurrencyCode"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult UOM()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.UOMNameRepo(identity).DropDown(), "UOMCode", "UOMCode"), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult Table()
        //{
        //    return Json(new SelectList(new SymRepository.VMS.DataUpdateRepo().TableName(), "tableName", "tableName"), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult Vehicle()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.VehicleRepo(identity).DropDown(), "VehicleID", "VehicleNo"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Vendor()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.VendorRepo(identity).DropDown(), "VendorID", "VendorName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult VendorName()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new SelectList(new SymRepository.VMS.VendorRepo(identity).DropDown(), "VendorName", "VendorName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult VendorGroup()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var res = Json(new SelectList(new SymRepository.VMS.VendorGroupRepo(identity).DropDown(), "VendorGroupID", "VendorGroupName"), JsonRequestBehavior.AllowGet);
            return res;
        }
        public JsonResult StructureType()
        {
            return Json(new SelectList(new EnumRepo().GetStructureTypeList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostStatus()
        {
            return Json(new SelectList(new EnumRepo().GetPostStatusList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult DropDowDaysOfWeek()
        {
            return Json(new SelectList(new EnumRepo().DaysOfWeekList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult VatTypesList()
        {
            return Json(new SelectList(new EnumRepo().VatTypesList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaleTypesList()
        {
            return Json(new SelectList(new EnumRepo().SaleTypesList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AdjTypesList()
        {
            return Json(new SelectList(new EnumRepo().AdjTypesList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PrinterList()
        {
            return Json(new SelectList(new EnumRepo().PrinterList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductTypeList()
        {
            return Json(new SelectList(new EnumRepo().productTypeList(), "Name", "Name"), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult DropDownReport(string ReportType)
        //{
        //    return Json(new SelectList(new EnumReportRepo().DropDown(ReportType), "ReportId", "Name"), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult ColumnList(string tableName = "")
        //{
        //    return Json(new SelectList(new EnumColumnListRepo().DropDown(tableName), "Name", "Name"), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult AbsentDeductFromList()
        {
            return Json(new SelectList(new EnumRepo().AbsentDeductFromList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DaysCountList()
        {
            return Json(new SelectList(new EnumRepo().DaysCountList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AttnStatusName()
        {
            return Json(new SelectList(new EnumRepo().GetAttnStatusNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult HoliDayTypeName()
        {
            return Json(new SelectList(new EnumRepo().GetHoliDayTypeNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SalarySheetName()
        {
            return Json(new SelectList(new EnumRepo().GetSalarySheetNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoanInterestPolicyList()
        {
            return Json(new SelectList(new EnumRepo().GetLoanInterestPolicyList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SalaryProcessName()
        {
            return Json(new SelectList(new EnumRepo().GetSalaryProcessNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EarningRptParamName()
        {
            return Json(new SelectList(new EnumRepo().GetEarningRptParamNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeductionRptParamName()
        {
            return Json(new SelectList(new EnumRepo().GetDeductionRptParamNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PFTaxRptParamName()
        {
            return Json(new SelectList(new EnumRepo().GetPFTaxRptParamNameList(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ReportType() 
        {
            List<SelectListItem> ReportType = new List<SelectListItem>();


            //dicReportType.Add("Select", "Select");
            ReportType.Add(new SelectListItem() { Text = "VAT6_1_And_6_2", Value = "VAT6_1_And_6_2" });
            ReportType.Add(new SelectListItem() { Text = "VAT6_1", Value = "VAT6_1" });
            ReportType.Add(new SelectListItem() { Text = "VAT6_2", Value = "VAT6_2" });
            ReportType.Add(new SelectListItem() { Text = "VAT6_2_1", Value = "VAT6_2_1" });

            return Json(new SelectList(ReportType, "Value", "Text"), JsonRequestBehavior.AllowGet);
          
        }

        #endregion


    }
}
