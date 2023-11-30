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
using SymphonySofttech.Reports.Report;
using SymVATWebUI.Areas.VMS.Models;
using System.Configuration;
using Newtonsoft.Json;
using VATServer.Ordinary;
using VATServer.Library;
using SymphonySofttech.Utilities;
using SymVATWebUI.Filters;


namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class PurchaseController : Controller
    {
        //PurchaseRepo _repo = new PurchaseRepo();

        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        ShampanIdentity identity = null;

        PurchaseRepo _repo = null;

        public PurchaseController()
        {
            try
            {

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new PurchaseRepo(identity);
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
        [Authorize(Roles = "Admin")]
        public ActionResult Index(PurchaseMasterVM paramVM)
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

            CommonRepo _cRepo = new CommonRepo(identity, Session);

            if (string.IsNullOrWhiteSpace(paramVM.TransactionType))
            {
                paramVM.TransactionType = "Other";
            }

            //PurchaseMasterVM vm = new PurchaseMasterVM();
            //vm.TransactionType = tType;
            paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            paramVM.RebateWithGRN = _cRepo.settingValue("Purchase", "RebateWithGRN");

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
        public ActionResult _index(JQueryDataTableParamVM param, PurchaseMasterVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new PurchaseRepo(identity, Session);
            List<PurchaseMasterVM> getAllData = new List<PurchaseMasterVM>();
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
            //string dtFrom = DateTime.Now.ToString("yyyyMMdd");
            //string dtTo = DateTime.Now.ToString("yyyyMMdd");
            //if (!string.IsNullOrWhiteSpace(Session["Branch"] as string))
            //{
            //    searchedBranchId = Session["Branch"].ToString();
            //}
            //if (!string.IsNullOrWhiteSpace(Session["dtFrom"] as string))
            //{
            //    dtFrom = Convert.ToDateTime(Session["dtFrom"]).ToString("yyyyMMdd");
            //}
            //if (!string.IsNullOrWhiteSpace(Session["dtTo"] as string))
            //{
            //    dtTo = Convert.ToDateTime(Session["dtTo"]).ToString("yyyyMMdd");
            //}
            //string BranchId = "";
            //if (searchedBranchId == "-1")
            //{
            //    BranchId = "";
            //}
            //else if (BranchId != searchedBranchId && searchedBranchId != "0")
            //{
            //    BranchId = searchedBranchId;
            //}
            //else
            //{
            //    BranchId = identity.BranchId.ToString();
            //}

            string dtFrom = null;
            string dtTo = null;

            if (string.IsNullOrWhiteSpace(paramVM.SearchField))
            {
                //    string dtFrom = DateTime.Now.ToString("yyyyMMdd");

                //    string dtTo = DateTime.Now.AddDays(1).ToString("yyyyMMdd");

                if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeFrom))
                {
                    dtFrom = Convert.ToDateTime(paramVM.InvoiceDateTimeFrom).ToString("yyyy-MM-dd");
                }
                if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeTo))
                {

                    dtTo = Convert.ToDateTime(paramVM.InvoiceDateTimeTo).AddDays(1).ToString("yyyy-MM-dd");
                }
            }


            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "All";
            }


            if (paramVM.BranchId == 0)
            {
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            }

            if (paramVM.BranchId == -1)
            {
                paramVM.BranchId = 0;
            }

            #endregion SeachParameters
            string[] conditionFields;
            string[] conditionValues;
            if (string.IsNullOrWhiteSpace(paramVM.SearchField))
            {
                ////conditionFields = new string[] { "pih.InvoiceDateTime>=", "pih.InvoiceDateTime<=", "pih.TransactionType", "v.VendorGroupID", "pih.Post", "pih.WithVDS", "pih.BranchId" };
                conditionFields = new string[] { "pih.ReceiveDate>=", "pih.ReceiveDate<", "pih.TransactionType", "v.VendorGroupID", "pih.Post", "pih.WithVDS", "pih.BranchId", "SelectTop", "pih.IsRebate" };
                conditionValues = new string[] { dtFrom, dtTo, paramVM.TransactionType, paramVM.VendorGroup, paramVM.Post, paramVM.WithVDS, paramVM.BranchId.ToString(), paramVM.SelectTop, paramVM.IsRebate };
            }

            else
            {
                if (string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeFrom))
                {
                    dtFrom = "";
                }
                if (string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeTo))
                {

                    dtTo = "";
                }

                if (paramVM.SearchField == "VendorName")
                {
                    paramVM.SearchField = "v.VendorName like";
                }
                else if (paramVM.SearchField == "VendorCode")
                {
                    paramVM.SearchField = "v.VendorCode like";
                }

                else if (paramVM.SearchField == "ImportID")
                {
                    paramVM.SearchField = "pih.ImportIDExcel like";
                }

                else
                {
                    paramVM.SearchField = "pih." + paramVM.SearchField + " like";
                }
                ////conditionFields = new string[] { "pih.InvoiceDateTime>=", "pih.InvoiceDateTime<=", "pih.TransactionType", "v.VendorGroupID", "pih.Post", "pih.WithVDS", paramVM.SearchField, "pih.BranchId" };
                conditionFields = new string[] { "pih.ReceiveDate>=", "pih.ReceiveDate<=", "pih.TransactionType", "v.VendorGroupID", "pih.Post", "pih.WithVDS", paramVM.SearchField, "pih.BranchId", "SelectTop", "pih.IsRebate" };
                conditionValues = new string[] { dtFrom, dtTo, paramVM.TransactionType, paramVM.VendorGroup, paramVM.Post, paramVM.WithVDS, paramVM.SearchValue, paramVM.BranchId.ToString(), paramVM.SelectTop, paramVM.IsRebate };

            }
            getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            //if (!identity.IsAdmin)
            //{
            //    string[] conditionFields = { "pih.InvoiceDateTime>=", "pih.InvoiceDateTime<=", "pih.TransactionType", "pih.PurchaseInvoiceNo like", "v.VendorName like", "v.VendorGroupID", "pih.BENumber like", "pih.SerialNo like", "pih.Post", "pih.WithVDS" };
            //    string[] conditionValues = { dtFrom, dtTo, paramVM.TransactionType, paramVM.PurchaseInvoiceNo, paramVM.VendorName, paramVM.VendorGroup, paramVM.BENumber, paramVM.SerialNo, paramVM.Post, paramVM.WithVDS };
            //    getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            //}
            //else
            //{
            //    string[] conditionFields = { "pih.InvoiceDateTime>=", "pih.InvoiceDateTime<=", "pih.TransactionType", "pih.PurchaseInvoiceNo like", "v.VendorName like", "v.VendorGroupID", "pih.BENumber like", "pih.SerialNo like", "pih.Post", "pih.WithVDS" };
            //    string[] conditionValues = { dtFrom, dtTo, paramVM.TransactionType, paramVM.PurchaseInvoiceNo, paramVM.VendorName, paramVM.VendorGroup, paramVM.BENumber, paramVM.SerialNo, paramVM.Post, paramVM.WithVDS };
            //    getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            //}
            #endregion
            #region Search and Filter Data
            IEnumerable<PurchaseMasterVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Id
                //PurchaseInvoiceNo
                //Vendor
                //InvoiceDate
                //TotalAmount
                //TotalVATAmount
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

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.PurchaseInvoiceNo.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.VendorName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.ReceiveDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.TotalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.TotalVATAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.BENumber.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable8 && c.ImportID.ToString().ToLower().Contains(param.sSearch.ToLower())

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
            Func<PurchaseMasterVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.PurchaseInvoiceNo :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.VendorName.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.ReceiveDate.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.TotalAmount.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.TotalVATAmount.ToString() :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Post.ToString() :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.BENumber.ToString() :
                                                           sortColumnIndex == 8 && isSortable_8 ? c.ImportID.ToString() :

                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id+"~"+ c.Post+"~"+c.BranchId
                , c.PurchaseInvoiceNo
                , c.VendorName.ToString()
                , Convert.ToDateTime(c.ReceiveDate).ToString("dd-MMM-yyyy")
                , c.TotalAmount.ToString()     
                , c.TotalVATAmount.ToString()
                , c.Post=="Y" ? "Posted" : "Not Posted"
                ,c.BENumber
                ,c.ImportID
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

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(PurchaseDetailVM vm)
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

            CommonRepo _cRepo = new CommonRepo(identity, Session);
            string SettingValue = _cRepo.settingValue("Purchase", "TotalPriceIncludingVAT");

            if (SettingValue.ToLower() == "y")
            {
                decimal totalValueWithVAT = 0;
                decimal VATAmount = 0;
                decimal VATRate = 0;
                decimal subtotal = 0;
                decimal quty = 0;

                totalValueWithVAT = vm.AssessableValue;
                VATRate = vm.VATRate;
                quty = vm.Quantity;

                VATAmount = (totalValueWithVAT * VATRate) / (100 + VATRate);

                subtotal = (totalValueWithVAT * 100) / (100 + VATRate);

                vm.VATAmount = decimal.Round(VATAmount, 4, MidpointRounding.AwayFromZero);
                vm.UnitPrice = subtotal / quty;

                vm.UnitPrice = decimal.Round(vm.UnitPrice, 4, MidpointRounding.AwayFromZero);

                vm.UOMPrice = decimal.Round(vm.UnitPrice, 4, MidpointRounding.AwayFromZero);
                vm.FixedVATAmount = vm.VATAmount;
                vm.SubTotal = decimal.Round(subtotal, 4, MidpointRounding.AwayFromZero);


            }

            if (vm.Rowtype == "New")
            {
                vm.ExpireDate = "2100-01-01";
                vm.CPCName = "-";
                vm.BEItemNo = "-";
            }

            //if(Convert.ToBoolean(vm.Section21)==true)
            //{
            //    vm.Section21 = "Y";
            //}
            //else
            //{
            //    vm.Section21 = "N";
            //}

            #region Total cal

            decimal Total = 0;
            decimal vImpTotalPrice = 0;

            decimal vImpInsurance1 = vm.InsuranceAmount;
            decimal vImpCnF1 = vm.CnFAmount;
            decimal vImpAV1 = vm.SubTotal;
            decimal vImpCD1 = vm.CDAmount;
            decimal vImpRD1 = vm.RDAmount;
            decimal vImpTVB1 = vm.TVBAmount;
            decimal vImpSD1 = vm.SDAmount;
            decimal vImpVAT1 = vm.VATAmount;
            decimal vImpATV1 = vm.ATVAmount;
            decimal vImpTVA1 = vm.TVAAmount;
            decimal vImpOthers1 = vm.OthersAmount;
            decimal vImpAIT1 = vm.AITAmount;

            vImpTotalPrice = (vImpAV1 + vImpCD1 + vImpRD1 + vImpTVB1 + vImpSD1 + vImpVAT1 + vImpATV1 +
                                      vImpOthers1 + vImpOthers1 + vImpTVA1 + vImpInsurance1 + vImpCnF1);

            Total = vImpTotalPrice;

            vm.Total = Total;

            #endregion



            if (vm.TransactionType == "PurchaseReturn")
            {
                decimal Subtotal = 0;
                decimal SDAmount = 0;
                decimal VATAmount = 0;

                ////vm.ReasonOfReturn = "-";
                vm.PreviousSD = vm.PreviousSD == 0 ? vm.SD : vm.PreviousSD;
                vm.PreviousNBRPrice = vm.PreviousNBRPrice == 0 ? vm.UOMPrice : vm.PreviousNBRPrice;
                vm.PreviousUOM = vm.UOM;
                vm.PreviousVATRate = vm.VATRate;


                Subtotal = Convert.ToDecimal(vm.PreviousQuantity) * Convert.ToDecimal(vm.PreviousNBRPrice);
                SDAmount = (Subtotal * Convert.ToDecimal(vm.PreviousSD) / 100);
                VATAmount = ((Subtotal + SDAmount) * Convert.ToDecimal(vm.PreviousVATRate) / 100);
                vm.PreviousSubTotal = Subtotal;
                vm.PreviousVATAmount = VATAmount;
                vm.PreviousSDAmount = SDAmount;


                return PartialView("_detailAdjustment", vm);
            }
            else
            {
                return PartialView("_detail", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItems(string purchaseNo)
        {
            _repo = new PurchaseRepo(identity, Session);
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
            List<PurchaseDetailVM> vms = _repo.SelectPurchaseDetail(purchaseNo);
            foreach (PurchaseDetailVM vmD in vms)
            {
                vmD.PurchaseReturnId = vmD.PurchaseInvoiceNo;
                vmD.PreviousInvoiceDateTime = vmD.ReceiveDate;
                vmD.PreviousNBRPrice = vmD.UOMPrice;
                vmD.PreviousQuantity = vmD.Quantity;
                vmD.PreviousSD = vmD.SD;
                vmD.PreviousSDAmount = vmD.SDAmount;
                vmD.PreviousSubTotal = vmD.SubTotal;
                vmD.PreviousUOM = vmD.UOM;
                vmD.PreviousVATAmount = vmD.VATAmount;
                vmD.PreviousVATRate = vmD.VATRate;
                vmD.ReasonOfReturn = "-";
                vmD.TransactionType = "PurchaseReturn";

            }
            return PartialView("_detailMultiple", vms);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItemTrakings(TrackingVM vm)
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
            //string ItemNo = vm.ItemNo.Trim();
            //vm.ItemNo = ItemNo;
            //vm.Quantity = 1;
            vm.IsPurchase = "Y";

            CommonRepo _cRepo = new CommonRepo(identity, Session);
            List<TrackingVM> TrackingsVMS = new List<TrackingVM>();

            //string SettingValue = _cRepo.settingValue("Purchase", "TotalPriceIncludingVAT");

            #region Auto check

            if (vm.chkHeading1 == false && vm.chkHeading2 == false)
            {
                //string ItemNo = vm.ItemNo.Trim();
                //vm.ItemNo = ItemNo;
                //vm.Quantity = 1;
                vm.IsPurchase = "Y";

                TrackingsVMS.Add(vm);

            }
            else
            {

                for (int i = 0; i < vm.Quantity; i++)
                {
                    TrackingVM model = new TrackingVM();


                    if (vm.chkHeading1 == true && vm.chkHeading2 == true)
                    {
                        string[] partsOfID = new string[2];
                        partsOfID = GenerateTrackingIDs(vm.Heading1.Trim());
                        string noPart = partsOfID[1];
                        decimal regenratePart = Convert.ToDecimal(noPart) + i;
                        string newNoPart = regenratePart.ToString();
                        if (noPart.Length > regenratePart.ToString().Length)
                        {
                            decimal oCount = (noPart.Length) - (regenratePart.ToString().Length);

                            for (int count = 0; count < oCount; count++)
                            {
                                newNoPart = "0" + newNoPart;
                            }
                        }


                        string[] partsOfID2 = new string[2];
                        partsOfID2 = GenerateTrackingIDs(vm.Heading2.Trim());
                        string noPart2 = partsOfID2[1];
                        decimal regenratePart2 = Convert.ToDecimal(noPart2) + i;
                        string newNoPart2 = regenratePart2.ToString();
                        if (noPart2.Length > regenratePart2.ToString().Length)
                        {
                            decimal oCount = (noPart2.Length) - (regenratePart2.ToString().Length);

                            for (int count = 0; count < oCount; count++)
                            {
                                newNoPart2 = "0" + newNoPart2;
                            }
                        }

                        var heading2 = partsOfID2[0] + newNoPart2;

                        var heading1 = partsOfID[0] + newNoPart;

                        model.Heading1 = heading1;
                        model.Heading2 = heading2;
                        model.ItemNo = vm.ItemNo;
                        model.IsPurchase = "Y";
                        TrackingsVMS.Add(model);

                    }
                    else if (vm.chkHeading1 == true && vm.chkHeading2 == false)
                    {

                        string[] partsOfID = new string[2];
                        partsOfID = GenerateTrackingIDs(vm.Heading1.Trim());
                        string noPart = partsOfID[1];
                        decimal regenratePart = Convert.ToDecimal(noPart) + i;
                        string newNoPart = regenratePart.ToString();
                        if (noPart.Length > regenratePart.ToString().Length)
                        {
                            decimal oCount = (noPart.Length) - (regenratePart.ToString().Length);

                            for (int count = 0; count < oCount; count++)
                            {
                                newNoPart = "0" + newNoPart;
                            }
                        }


                        //var heading2 = partsOfID2[0] + newNoPart2;
                        var heading1 = partsOfID[0] + newNoPart;
                        model.Heading1 = heading1;
                        model.Heading2 = vm.Heading2;
                        model.ItemNo = vm.ItemNo;
                        model.IsPurchase = "Y";
                        TrackingsVMS.Add(model);


                    }
                    else if (vm.chkHeading1 == false && vm.chkHeading2 == true)
                    {
                        string[] partsOfID2 = new string[2];
                        partsOfID2 = GenerateTrackingIDs(vm.Heading2.Trim());
                        string noPart2 = partsOfID2[1];
                        decimal regenratePart2 = Convert.ToDecimal(noPart2) + i;
                        string newNoPart2 = regenratePart2.ToString();
                        if (noPart2.Length > regenratePart2.ToString().Length)
                        {
                            decimal oCount = (noPart2.Length) - (regenratePart2.ToString().Length);

                            for (int count = 0; count < oCount; count++)
                            {
                                newNoPart2 = "0" + newNoPart2;
                            }
                        }

                        var heading2 = partsOfID2[0] + newNoPart2;
                        //var heading1 = partsOfID[0] + newNoPart;
                        model.Heading1 = vm.Heading1;
                        model.Heading2 = heading2;
                        model.ItemNo = vm.ItemNo;
                        model.IsPurchase = "Y";
                        TrackingsVMS.Add(model);

                    }
                }


            }

            #endregion


            return PartialView("_detailTrakings", TrackingsVMS);
        }

        private string[] GenerateTrackingIDs(string text)
        {
            string[] partsOfID = new string[2];
            try
            {
                int pre = 0;

                for (int i = text.Length; i <= text.Length; i--)
                {
                    if (i > 0)
                    {


                        var a = text.Substring(i - 1, 1);
                        char b = Convert.ToChar(a);

                        if (Char.IsNumber(b))
                        {
                            pre = pre + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;

                    }
                }
                string part1 = text.Substring(0, text.Length - pre);
                partsOfID[0] = part1;

                string part2 = text.Substring(text.Length - pre);

                //if (Program.IsNumeric(part2))
                if (OrdinaryVATDesktop.IsNumeric(part2))
                {
                    partsOfID[1] = part2;
                }


            }
            catch (Exception)
            {

                throw;
            }

            return partsOfID;

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create(string tType)
        {
            try
            {

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new PurchaseRepo(identity, Session);
                CommonRepo _cRepo = new CommonRepo(identity, Session);

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
                PurchaseMasterVM vm = new PurchaseMasterVM();

                List<PurchaseDetailVM> PurchaseDetailVMs = new List<PurchaseDetailVM>();
                List<PurchaseDutiesVM> PurchaseDutiesVMs = new List<PurchaseDutiesVM>();
                List<TrackingVM> purchaseTrackings = new List<TrackingVM>();
                vm.Details = PurchaseDetailVMs;
                vm.Duties = PurchaseDutiesVMs;
                vm.Trackings = purchaseTrackings;
                vm.Operation = "add";

                if (string.IsNullOrWhiteSpace(tType))
                {
                    tType = "Other";
                }
                vm.TransactionType = tType;

                #region FormMaker

                FormMaker(vm);

                #endregion

                vm.ReceiveDate = Session["SessionDate"].ToString();
                vm.InvoiceDate = Session["SessionDate"].ToString();
                vm.LCDate = Session["SessionDate"].ToString();

                if (tType == "PurchaseCN" || tType == "PurchaseDN" || tType == "PurchaseReturn")
                {
                    vm.IsTotalPrice = _cRepo.settingValue("Purchase", "TotalPrice");
                    vm.TransactionType = tType;
                    return View("CreateAdjustment", vm);
                }
                if (tType == "Import" || tType == "TradingImport" || tType == "InputServiceImport")
                {
                    vm.IsImport = true;
                }
                vm.RebateDate = DateTime.Now.ToString("MMMM-yyyy");
                vm.IsRebates = true;
                //vm.IsTDS = "Y";
                vm.IsTDS = _cRepo.settingValue("Purchase", "IsTDS");

                vm.WithVDS = "N";
                vm.USDInvoiceValue = 1;

                vm.IsTotalPrice = _cRepo.settingValue("Purchase", "TotalPrice");
                // vTrackingTrace = commonDal.settingsDesktop("TrackingTrace", "Tracking");
                vm.TrackingTrace = _cRepo.settingValue("TrackingTrace", "Tracking");
                vm.MultipleItemInsert = _cRepo.settingValue("Purchase", "MultipleItemInsert");

                vm.VATTypeVATAutoChange = _cRepo.settingValue("VATTypeVAT", "AutoChange");
                if (vm.TransactionType == "TollReceive" || vm.TransactionType == "ClientFGReceiveWOBOM")
                {
                    vm.IsTotalPrice = "N";
                }


                return View(vm);
            }
            catch (Exception e)
            {
                ////SymOrdinary.FileLogger.Log("SaleInvoiceController", "CreateEdit", "-----" +"\n"+e.StackTrace);

                return RedirectToAction("Index");

            }
        }

        private void FormMaker(PurchaseMasterVM vm)
        {
            #region  Defoalt Settings Controll In From
            CommonRepo commonRepo = new CommonRepo(identity, Session);

            string DefaultProductGroup = commonRepo.settings("Purchase", "DefaultProductGroup");
            string DefaultProductType = commonRepo.settings("Purchase", "DefaultProductType");
            string DefaultVATType = commonRepo.settings("DefaultVATType", "Purchase");
            string ExpireDateTracking = commonRepo.settings("Purchase", "ExpireDateTracking");

            vm.ProductType = DefaultProductType.ToLower().ToUpperFirstLetter() == "" ? "Raw" : DefaultProductType.ToLower().ToUpperFirstLetter();
            vm.ProductGroup = DefaultProductGroup.ToLower().ToUpperFirstLetter() == "" ? "Raw" : DefaultProductGroup.ToLower().ToUpperFirstLetter();

            var product = new ProductCategoryRepo(identity, Session).SelectAll(0, new string[] { "CategoryName" }, new string[] { vm.ProductGroup }).FirstOrDefault();
            if (product == null)
            {
                vm.ProductCategoryId = 0;
            }
            else
            {
                vm.ProductCategoryId = Convert.ToInt32(product.CategoryID);
            }

            vm.Type = DefaultVATType;
            vm.IsExpireDate = ExpireDateTracking;

            if (vm.Type.ToUpper() == "NONVAT")
            {
                vm.Type = "NonVAT";
            }
            if (vm.Type.ToUpper() == "FIXEDVAT")
            {
                vm.Type = "FixedVAT";
            }
            if (vm.Type.ToUpper() == "OTHERRATE")
            {
                vm.Type = "OtherRate";
            }
            if (vm.Type.ToUpper() == "EXEMPTED")
            {
                vm.Type = "Exempted";
            }
            if (vm.Type.ToUpper() == "TRUNCATED")
            {
                vm.Type = "Truncated";
            }
            if (vm.Type.ToUpper() == "TRUNOVER")
            {
                vm.Type = "Trunover";
            }
            if (vm.Type.ToUpper() == "NONREBATE")
            {
                vm.Type = "NonRebate";
            }
            if (vm.Type.ToUpper() == "UNREGISTER")
            {
                vm.Type = "UnRegister";
            }
            #endregion
            //vm.Type = "VAT";
            //vm.ProductType = "Raw";


            switch (vm.TransactionType)
            {
                case "Other": break;
                case "Trading": { vm.ProductType = "Trading"; } break;
                case "TradingImport": { vm.ProductType = "Trading"; } break;
                case "Import": { } break;
                case "InputService": { vm.ProductType = "Overhead"; } break;
                case "PurchaseReturn": { } break;
                case "Service": { vm.ProductType = "Service"; } break;
                case "ServiceNS": { vm.ProductType = "Service"; } break;
                case "ClientRawReceive": { vm.ProductType = "Raw"; } break;
                case "ClientFGReceiveWOBOM": { vm.ProductType = "Finish"; } break;

                case "TollReceive": { vm.ProductType = "Overhead"; } break;
                case "PurchaseTollcharge": { vm.ProductType = "Overhead"; } break;

                case "TollReceiveRaw": { } break;
                case "PurchaseCN": { } break;
                case "PurchaseDN": { } break;

                default: break;
            }

        }

        [HttpPost]
        //[Authorize]
        public ActionResult CreateEdit(PurchaseMasterVM vm)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new PurchaseRepo(identity, Session);
            CommonRepo _cRepo = new CommonRepo(identity, Session);

            #region variable
            FiscalYearVM varFiscalYearVM = new FiscalYearVM();

            int periodId = 0;

            string vDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

            #endregion

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
            List<TrackingVM> Trackings = new List<TrackingVM>();

            try
            {

                #region Find Rebate Period Id

                string RebatePeriod = vm.RebateDate;
                string[] conditionValues = { RebatePeriod };
                string[] conditionFields = { "PeriodName" };
                varFiscalYearVM = new FiscalYearRepo(identity, Session).SelectAll(0, conditionFields, conditionValues).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(RebatePeriod + ": This Fiscal Period is not Exist!");
                }

                periodId = Convert.ToInt32(varFiscalYearVM.PeriodID);

                vm.RebatePeriodId = varFiscalYearVM.PeriodID;

                #endregion

                if (vm.TransactionType.ToLower() == "trading" && vm.IsImport == true)
                {
                    vm.TransactionType = "TradingImport";
                }
                vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                #region Details

                string vlogString = "";

                int count = 1;
                foreach (var item in vm.Details)
                {
                    item.LineNo = count.ToString();
                    item.ProductType = vm.ProductType;
                    item.BENumber = vm.BENumber;

                    if (string.IsNullOrWhiteSpace(item.Comments))
                    {
                        item.Comments = "NA";
                    }

                    count = count + 1;

                    string logString = " ItemNo : " + item.ItemNo + "\n" + " ProductCode : " + item.ProductCode + "\n" + " UOM : " + item.UOM + "\n" + " UOMn : " + item.UOMn + "\n" + " UOMc : " + item.UOMc + "\n"
                        + " UOMQty : " + item.UOMQty + "\n" + " UOMPrice : " + item.UOMPrice;

                    vlogString += "\n" + logString;

                }
                #endregion

                #region DateTime Convert

                vm.InvoiceDate = Convert.ToDateTime(vm.InvoiceDate).ToString("yyyy-MMM-dd HH:mm:ss");//// + DateTime.Now.ToString(" hh:mm:ss");
                vm.ReceiveDate = Convert.ToDateTime(vm.ReceiveDate).ToString("yyyy-MMM-dd HH:mm:ss");//// +DateTime.Now.ToString(" hh:mm:ss"); ;
                vm.RebateDate = Convert.ToDateTime(vm.RebateDate).ToString("yyyy-MMM-dd HH:mm:ss");
                #endregion

                #region Check Point

                vm.IsRebate = vm.IsRebates ? "Y" : "N";

                if (string.IsNullOrWhiteSpace(vm.AppVersion))
                {
                    vm.AppVersion = "Web";
                }
                if (string.IsNullOrWhiteSpace(vm.LCNumber))
                {
                    vm.LCNumber = "-";
                }
                if (string.IsNullOrWhiteSpace(vm.SerialNo))
                {
                    vm.SerialNo = "-";
                }
                if (string.IsNullOrWhiteSpace(vm.Comments))
                {
                    vm.Comments = "-";
                }
                if (string.IsNullOrWhiteSpace(vm.WithVDS))
                {
                    vm.WithVDS = "N";
                }
                if (string.IsNullOrWhiteSpace(vm.CustomHouse))
                {
                    vm.CustomHouse = "";
                }
                if (string.IsNullOrWhiteSpace(vm.BankGuarantee))
                {
                    vm.BankGuarantee = "";
                }

                if (vm.LCNumber != "-")
                {
                    vm.LCDate = Convert.ToDateTime(vm.LCDate).ToString("yyyy-MMM-dd") + Convert.ToDateTime(vDateTime).ToString(" HH:mm:ss");
                }
                #endregion

                List<PurchaseDutiesVM> Duties = new List<PurchaseDutiesVM>();
                if (vm.Operation.ToLower() == "add")
                {
                    if (vm.TransactionType == "InputService" && vm.IsImport == true)
                    {
                        vm.TransactionType = "InputServiceImport";
                    }

                    if (vm.TransactionType == "PurchaseReturn")
                    {
                        vm.RebateDate = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                }
                #region duties

                if (vm.TransactionType == "Import" || vm.TransactionType == "InputServiceImport")
                {
                    Duties = new List<PurchaseDutiesVM>();
                    foreach (var Details in vm.Details)
                    {
                        PurchaseDutiesVM purchaseDuty = new PurchaseDutiesVM();

                        purchaseDuty.PIDutyID = "";
                        purchaseDuty.PurchaseInvoiceNo = vm.PurchaseInvoiceNo;
                        purchaseDuty.ItemNo = Details.ItemNo;//dgvDuty.Rows[i].Cells["ItemNoDuty"].Value.ToString();
                        purchaseDuty.Quantity = Convert.ToDecimal(Details.Quantity);//Convert.ToDecimal(dgvPurchase.Rows[i].Cells["Quantity"].Value.ToString());

                        purchaseDuty.CnFInp = Convert.ToDecimal(Details.CnFAmount);////Convert.ToDecimal(dgvDuty.Rows[i].Cells["CnFInp"].Value.ToString());
                        purchaseDuty.CnFRate = 0;//Convert.ToDecimal(dgvDuty.Rows[i].Cells["CnFRate"].Value.ToString());
                        purchaseDuty.CnFAmount = Convert.ToDecimal(Details.CnFAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["CnFAmount"].Value.ToString());

                        purchaseDuty.InsuranceInp = Convert.ToDecimal(Details.InsuranceAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["InsuranceInp"].Value.ToString());
                        purchaseDuty.InsuranceRate = 0;//Convert.ToDecimal(dgvDuty.Rows[i].Cells["InsuranceRate"].Value.ToString());
                        purchaseDuty.InsuranceAmount = Convert.ToDecimal(Details.InsuranceAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["InsuranceAmount"].Value.ToString());

                        purchaseDuty.AssessableInp = Convert.ToDecimal(Details.AssessableValue);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["AssessableInp"].Value.ToString());
                        purchaseDuty.AssessableValue = Convert.ToDecimal(Details.AssessableValue);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["AssessableValue"].Value.ToString());

                        purchaseDuty.CDInp = Convert.ToDecimal(Details.CDAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["CDInp"].Value.ToString());
                        purchaseDuty.CDRate = 0;//Convert.ToDecimal(dgvDuty.Rows[i].Cells["CDRate"].Value.ToString());
                        purchaseDuty.CDAmount = Convert.ToDecimal(Details.CDAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["CDAmount"].Value.ToString());

                        purchaseDuty.RDInp = Convert.ToDecimal(Details.RDAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["RDInp"].Value.ToString());
                        purchaseDuty.RDRate = 0;//Convert.ToDecimal(dgvDuty.Rows[i].Cells["RDRate"].Value.ToString());
                        purchaseDuty.RDAmount = Convert.ToDecimal(Details.RDAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["RDAmount"].Value.ToString());

                        purchaseDuty.TVBInp = Convert.ToDecimal(Details.TVBAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVBInp"].Value.ToString());
                        purchaseDuty.TVBRate = 0;//Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVBRate"].Value.ToString());
                        purchaseDuty.TVBAmount = Convert.ToDecimal(Details.TVBAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVBAmount"].Value.ToString());

                        purchaseDuty.SDInp = Convert.ToDecimal(Details.SDAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["SDInp"].Value.ToString());
                        purchaseDuty.SD = 0;//Convert.ToDecimal(dgvDuty.Rows[i].Cells["SDuty"].Value.ToString());
                        purchaseDuty.SDAmount = Convert.ToDecimal(Details.SDAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["SDutyAmount"].Value.ToString());

                        purchaseDuty.VATInp = Convert.ToDecimal(Details.VATAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["VATInp"].Value.ToString());
                        purchaseDuty.VATRate = Convert.ToDecimal(Details.VATRate);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["VATRateDuty"].Value.ToString());
                        purchaseDuty.VATAmount = Convert.ToDecimal(Details.VATAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["VATAmountDuty"].Value.ToString());

                        purchaseDuty.TVAInp = Convert.ToDecimal(Details.TVAAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVAInp"].Value.ToString());
                        purchaseDuty.TVARate = Convert.ToDecimal(0);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVARate"].Value.ToString());
                        purchaseDuty.TVAAmount = Convert.ToDecimal(Details.TVAAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["TVAAmount"].Value.ToString());

                        purchaseDuty.ATVInp = Convert.ToDecimal(Details.ATVAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["ATVInp"].Value.ToString());
                        purchaseDuty.ATVRate = 0;//Convert.ToDecimal(dgvDuty.Rows[i].Cells["ATVRate"].Value.ToString());
                        purchaseDuty.ATVAmount = Convert.ToDecimal(Details.ATVAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["ATVAmount"].Value.ToString());

                        purchaseDuty.OthersInp = Convert.ToDecimal(Details.OthersAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["OthersInp"].Value.ToString());
                        purchaseDuty.OthersRate = 0;//Convert.ToDecimal(dgvDuty.Rows[i].Cells["OthersRate"].Value.ToString());
                        purchaseDuty.OthersAmount = Convert.ToDecimal(Details.OthersAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["OthersAmount"].Value.ToString());

                        purchaseDuty.AITInp = Convert.ToDecimal(Details.AITAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["AITInp"].Value.ToString());
                        purchaseDuty.AITAmount = Convert.ToDecimal(Details.AITAmount);//Convert.ToDecimal(dgvDuty.Rows[i].Cells["AITAmount"].Value.ToString());

                        purchaseDuty.Remarks = Details.DutyRemarks;//dgvDuty.Rows[i].Cells["Remarks"].Value.ToString();
                        purchaseDuty.SetCost();
                        purchaseDuty.BranchId = vm.BranchId;

                        Duties.Add(purchaseDuty);
                    }
                }

                #endregion duties

                #region Trakings

                if (vm.TransactionType == "Import" && vm.Trackings != null)
                {
                    //List<TrackingVM> = new List<TrackingVM>();
                    int trcount = 1;
                    foreach (var tr in vm.Trackings)
                    {
                        TrackingVM PurchaseTrakingsVm = new TrackingVM();

                        PurchaseTrakingsVm.TrackingLineNo = trcount.ToString();

                        PurchaseTrakingsVm.ItemNo = tr.ItemNo;
                        PurchaseTrakingsVm.Heading1 = tr.Heading1;
                        PurchaseTrakingsVm.Heading2 = tr.Heading2;
                        PurchaseTrakingsVm.IsPurchase = "Y";
                        PurchaseTrakingsVm.IsIssue = "N";
                        PurchaseTrakingsVm.IsReceive = "N";
                        PurchaseTrakingsVm.IsSale = "N";

                        var prodByCode = from prd in vm.Details.ToList()
                                         where prd.ItemNo.ToLower() == PurchaseTrakingsVm.ItemNo.ToLower()
                                         select prd;

                        var products = prodByCode.First();

                        PurchaseTrakingsVm.Quantity = products.Quantity;
                        PurchaseTrakingsVm.UnitPrice = Convert.ToDecimal(products.UnitPrice);

                        PurchaseTrakingsVm.BranchId = vm.BranchId;

                        trcount = trcount + 1;

                        Trackings.Add(PurchaseTrakingsVm);

                    }

                }

                #endregion Trakings

                #region 01-Feb-2021

                TDSCalcVM tdsVm = new TDSCalcVM();

                tdsVm.VendorId = vm.VendorID;
                tdsVm.ReceiveDate = Convert.ToDateTime(vm.ReceiveDate).ToString("dd-MMM-yyyy");
                tdsVm.TotalSubTotal = vm.TotalSubTotal;
                tdsVm.TotalVatAmount = vm.TotalVATAmount;
                tdsVm.TotalVDSAmount = vm.TotalVDSAmount;

                //txtTDSAmount.Text = Program.FormatingNumeric(tdsVm.TDSAmount.ToString(), 3);
                //txtNetBill.Text = tdsVm.NetBill.ToString();
                vm.IsTDS = _cRepo.settingValue("Purchase", "IsTDS");
                decimal tdsAmount = 0;

                #endregion

                #region Add Data

                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.CreatedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.LastModifiedBy = identity.Name;
                    //////vm.TransactionType = "Other";
                    vm.Post = "N";
                    //List<TrackingVM> Trackings = new List<TrackingVM>();

                    string userId = identity.UserId;

                    result = _repo.PurchaseInsert(vm, vm.Details, Duties, Trackings);

                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1].Replace("\r", "").Replace("\n", "");
                        tdsAmount = vm.TDSAmount;

                        if (vm.IsTDS != "N")
                        {
                            if (!string.IsNullOrWhiteSpace(vm.VendorID))
                            {
                                tdsVm.InvoiceNo = result[2];
                                tdsVm = _repo.TDSCalculation(tdsVm);
                                tdsAmount = tdsVm.TDSAmount;
                            }

                        }

                        //return RedirectToAction("Edit", new { id = result[4], TransactionType = vm.TransactionType, NetBill = tdsVm.NetBill, TDSAmount = tdsVm.TDSAmount });
                        return RedirectToAction("Edit", new { id = result[4], TransactionType = vm.TransactionType, NetBill = tdsVm.NetBill, tdsAmount });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1].Replace("\r", "").Replace("\n", "");
                        return View("Create", vm);
                    }
                }
                #endregion

                #region Update Data

                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    //////vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                    //List<PurchaseDutiesVM> Duties = new List<PurchaseDutiesVM>();
                    //List<TrackingVM> Trackings = new List<TrackingVM>();
                    string userId = identity.UserId;

                    result = _repo.PurchaseUpdate(vm, vm.Details, Duties, Trackings, userId);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1].Replace("\r", " ").Replace("\n", "");
                        tdsAmount = vm.TDSAmount;

                        if (vm.IsTDS != "N")
                        {
                            if (!string.IsNullOrWhiteSpace(vm.VendorID))
                            {
                                tdsVm.InvoiceNo = result[2];
                                tdsVm = _repo.TDSCalculation(tdsVm);
                                tdsAmount = tdsVm.TDSAmount;
                            }

                        }

                        //return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.TransactionType, NetBill = tdsVm.NetBill, TDSAmount = tdsVm.TDSAmount });
                        return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.TransactionType, NetBill = tdsVm.NetBill, tdsAmount });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1].Replace("\r", " ").Replace("\n", "");
                        //return View("Create", vm);
                        return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.TransactionType, NetBill = tdsVm.NetBill, TDSAmount = tdsVm.TDSAmount });

                    }
                }
                #endregion

                else
                {
                    return View("Create", vm);
                }
            }
            catch (Exception ex)
            {
                string msg = "Fail~" + ex.Message.Replace("\r", "").Replace("\n", "");
                Session["result"] = msg;
                // Session["result"] = "Fail~Data not Successfully";
                SymOrdinary.FileLogger.Log("PurchaseController", "CreateEdit", "-----" + "\n" + ex.StackTrace);

                if (vm.TransactionType == "PurchaseCN" || vm.TransactionType == "PurchaseDN" || vm.TransactionType == "PurchaseReturn")
                {
                    return View("CreateAdjustment", vm);
                }
                else
                {
                    return View("Create", vm);
                }
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id, string TransactionType, decimal NetBill = 0, decimal TDSAmount = 0)
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new PurchaseRepo(identity, Session);
                CommonRepo _cRepo = new CommonRepo(identity, Session);

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

                if (TransactionType == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                PurchaseMasterVM vm = new PurchaseMasterVM();

                string[] conditionFields = new string[] { "pih.TransactionType" };
                string[] conditionValues = new string[] { TransactionType };


                vm = _repo.SelectAll(Convert.ToInt32(id), conditionFields, conditionValues).FirstOrDefault();


                if (vm == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                vm.IsRebates = vm.IsRebate == "Y";
                vm.RebateDate = Convert.ToDateTime(vm.RebateDate).ToString("MMMM-yyyy");
                vm.TDSAmount = TDSAmount;
                vm.NetBill = NetBill;

                List<PurchaseDetailVM> PurchaseDetailVMs = new List<PurchaseDetailVM>();

                PurchaseDetailVMs = _repo.SelectPurchaseDetail(vm.PurchaseInvoiceNo);

                vm.Details = PurchaseDetailVMs;


                List<TrackingVM> PurchaseTrakingVMs = new List<TrackingVM>();

                PurchaseTrakingVMs = _repo.SelectTrakingsDetail(PurchaseDetailVMs, vm.PurchaseInvoiceNo, null);
                vm.Trackings = PurchaseTrakingVMs;



                vm.Operation = "update";
                if (vm.TransactionType == "PurchaseCN" || vm.TransactionType == "PurchaseDN" || vm.TransactionType == "PurchaseReturn")
                {
                    vm.IsTotalPrice = _cRepo.settingValue("Purchase", "TotalPrice");

                    return View("CreateAdjustment", vm);
                }
                if (vm.TransactionType.ToLower() == "tradingimport" || vm.TransactionType.ToLower() == "import")
                {
                    vm.IsImport = true;
                }
                #region FormMaker
                FormMaker(vm);
                #endregion


                vm.IsTotalPrice = _cRepo.settingValue("Purchase", "TotalPrice");
                vm.TrackingTrace = _cRepo.settingValue("TrackingTrace", "Tracking");
                vm.IsTDS = _cRepo.settingValue("Purchase", "IsTDS");

                return View("Create", vm);
            }
            catch (Exception e)
            {
                ////SymOrdinary.FileLogger.Log("SaleInvoiceController", "CreateEdit", "-----" + "\n" + e.StackTrace);

                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult UnileverIntegrationEdit(string id)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new PurchaseRepo(identity, Session);
            CommonRepo _cRepo = new CommonRepo(identity, Session);

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



            PurchaseMasterVM vm = new PurchaseMasterVM();

            string[] conditionFields = new string[] { "pih.ImportIDExcel" };
            string[] conditionValues = new string[] { id };


            vm = _repo.SelectAll(0, conditionFields, conditionValues).FirstOrDefault();


            if (vm == null)
            {
                return RedirectToAction("Index", "Home");
            }

            vm.IsRebates = vm.IsRebate == "Y";
            vm.RebateDate = Convert.ToDateTime(vm.RebateDate).ToString("MMMM-yyyy");


            List<PurchaseDetailVM> PurchaseDetailVMs = new List<PurchaseDetailVM>();

            PurchaseDetailVMs = _repo.SelectPurchaseDetail(vm.PurchaseInvoiceNo);

            vm.Details = PurchaseDetailVMs;


            List<TrackingVM> PurchaseTrakingVMs = new List<TrackingVM>();

            PurchaseTrakingVMs = _repo.SelectTrakingsDetail(PurchaseDetailVMs, vm.PurchaseInvoiceNo, null);
            vm.Trackings = PurchaseTrakingVMs;



            vm.Operation = "update";
            if (vm.TransactionType == "PurchaseCN" || vm.TransactionType == "PurchaseDN" || vm.TransactionType == "PurchaseReturn")
            {
                return View("CreateAdjustment", vm);
            }
            if (vm.TransactionType.ToLower() == "tradingimport" || vm.TransactionType.ToLower() == "import")
            {
                vm.IsImport = true;
            }
            #region FormMaker
            FormMaker(vm);
            #endregion


            vm.IsTotalPrice = _cRepo.settingValue("Purchase", "TotalPrice");
            vm.TrackingTrace = _cRepo.settingValue("TrackingTrace", "Tracking");


            return View("Create", vm);
        }
        #region Report Actions

        [Authorize]
        [HttpPost]
        public ActionResult ReportVAT16(VAT6_1ParamVM vm)//Vat16ViewModel
        {
            try
            {

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                string post1;
                string post2;

                if (vm.PreviewOnly == true)
                {
                    post1 = "y";
                    post2 = "N";
                }
                else
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                int branchId = Convert.ToInt32(Session["BranchId"]);
                vm.BranchId = branchId;
                vm.UserId = identity.UserId;
                //////var ReportResult = repo.VAT16New(vm.ItemNo, identity.FullName, vm.StartDate, vm.EndDate, post1, post2, "", branchId);
                var ReportResult = repo.VAT16New(vm);

                var vat16s = repo.VAT16List(ReportResult);
                ////// RptVAT16_New objrpt = new RptVAT16_New();
                ReportClass objrpt = new ReportClass();

                objrpt.SetDataSource(vat16s);

                if (vm.PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                string[] conditionFields = { "Pr.ItemNo" };
                string[] conditionValues = { vm.ItemNo };
                var _repo = new ProductRepo(identity, Session);
                var product = _repo.SelectAll("0", conditionFields, conditionValues).FirstOrDefault();
                objrpt.DataDefinition.FormulaFields["HSCode"].Text = "'" + product.HSCodeNo + "'";
                objrpt.DataDefinition.FormulaFields["ProductName"].Text = "'" + product.ProductName + "'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'Address2 '";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'Address3 '";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'VatRegistrationNo'";

                var gr = new GenericReport<ReportClass>();
                var rpt = gr.RenderReportAsPDF(objrpt);
                objrpt.Close();
                return rpt;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult PrintVAT16(string itemNo, string invoiceDate)
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
            Vat16ViewModel vm = new Vat16ViewModel();
            vm.ItemNo = itemNo;
            vm.StartDate = invoiceDate;
            vm.EndDate = invoiceDate;
            return PartialView("_printVAT16", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportVAT18(Vat16ViewModel vm)
        {
            try
            {
                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                string post1;
                string post2;

                if (vm.PreviewOnly == true)
                {
                    post1 = "y";
                    post2 = "N";
                }
                else
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                var ReportResult = repo.VAT18New(identity.FullName, vm.StartDate, vm.EndDate, post1, post2);
                ReportResult.Tables[0].TableName = "DsVAT18";
                RptVAT18_New objrpt = new RptVAT18_New();
                if (vm.PreviewOnly == true)
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'Address2'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'Address3'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'VatRegistrationNo'";

                objrpt.SetDataSource(ReportResult);


                var gr = new GenericReport<RptVAT18_New>();
                var rpt = gr.RenderReportAsPDF(objrpt);
                objrpt.Close();
                return rpt;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult PrintVAT18()
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
            Vat16ViewModel vm = new Vat16ViewModel();
            return PartialView("_printVAT18", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult PrintMIS(string purNo)
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
            MISViewModel vm = new MISViewModel();
            vm.ProductName = purNo;
            return PartialView("_printMIS", vm);
        }


        [Authorize]
        [HttpPost]
        public ActionResult ReportMIS(MISViewModel vm)
        {
            try
            {
                vm.ReceiveDateFrom = "";
                vm.ReceiveDateTo = "";
                vm.VendorName = "";
                vm.ProductName = "";
                vm.ProductGroup = "";
                vm.Post = "";
                vm.VendorGroup = "";
                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                var ReportResult = repo.PurchaseNew(vm.PurchaseNo, vm.ReceiveDateFrom, vm.ReceiveDateTo,
                    vm.VendorName, vm.ProductName, vm.ProductGroup, "",
                    "Other", vm.Post, "", vm.VendorGroup, "N", "-", "-", 0, 0, 0, false, null);

                ReportResult.Tables[0].TableName = "DsPurchase";
                RptPurchaseSummery objrpt = new RptPurchaseSummery();
                var reportName = "All";

                if (vm.ReportType.ToLower() == "Summery")
                {
                    reportName += "(Summery)";
                }
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + reportName + "'";
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'CurrentUser'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName '";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
                objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'[All]'";
                objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'[All]'";
                objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";

                //if (vm.ProductName == "")
                //{ objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + vm.ProductName + "'  "; }

                //if (vm.VendorName == "")
                //{ objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'" + vm.VendorName + "'  "; }

                //if (txtInvoiceNo.Text == "")
                //{ objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'" + txtInvoiceNo.Text.Trim() + "'  "; }

                //if (dtpPurchaseFromDate.Checked == false)
                //{ objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + dtpPurchaseFromDate.Value.ToString("dd/MM/yyyy") + "'  "; }

                //if (dtpPurchaseToDate.Checked == false)
                //{ objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + dtpPurchaseToDate.Value.ToString("dd/MM/yyyy") + "'  "; }

                objrpt.SetDataSource(ReportResult);


                var gr = new GenericReport<RptPurchaseSummery>();
                var rpt = gr.RenderReportAsPDF(objrpt);
                objrpt.Close();
                return rpt;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region MISC Actions
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
            //if (string.IsNullOrWhiteSpace(code))
            //{
            //    vm = _repo.SelectAll();
            //}
            //else {
            //    string[] conditionalFields = new string[] { "ProductCode" };
            //    string[] conditionalValues = new string[] { code };
            //    vm=_repo.SelectAll("0", conditionalFields, conditionalValues).ToList();
            //}

            return PartialView("_product", vm);
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

            return PartialView("_create", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult SelectProductDetails(string productCode, string IssueDate, string vatName)
        {
            ProductVM product = new ProductVM();

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                var _repo = new ProductRepo(identity, Session);
                string[] conditionalFields = new string[] { "Pr.ProductCode" };
                string[] conditionalValues = new string[] { productCode };

                product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
                if (product == null)
                {

                    product = _repo.SelectAll("0", new string[] { "Pr.ItemNo" }, new string[] { productCode }).FirstOrDefault();

                }
                #region Business Logic
                string UserId = identity.UserId;
                var issueDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (!string.IsNullOrWhiteSpace(IssueDate))
                {
                    issueDatetime = DateTime.Parse(IssueDate).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                }
                if (vatName == "VAT 1 (Wastage)")
                {
                    product.Stock = 0;
                }
                else
                {
                    DataTable priceData = _repo.AvgPriceNew(product.ItemNo, issueDatetime, null, null, true, true, true, false, UserId);
                    ////DataTable priceData = _repo.AvgPriceNew(product.ProductCode, issueDatetime, null, null, false);
                    product.Stock = Convert.ToDecimal(priceData.Rows[0]["Quantity"]);
                }

                product.SalesPrice = _repo.GetLastNBRPriceFromBOM(product.ItemNo, vatName, issueDatetime, null, null);

                #endregion

            }
            catch (Exception e)
            {
                //SymOrdinary.FileLogger.Log("PurchaseController", "SelectProductDetails", e.Message + "\n" + e.StackTrace);
            }
            return Json(product, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUomOption(string uomFrom)
        {
            try
            {
                var _repo = new UOMRepo(identity, Session);
                string[] conditionalFields = new string[] { "UOMFrom" };
                string[] conditionalValues = new string[] { uomFrom };
                var uoms = _repo.SelectAll(0, conditionalFields, conditionalValues);
                var html = "";

                #region Old Code

                ////string uomF = OrdinaryVATDesktop.StringReplacingForHTML(uomFrom);

                ////html += "<option value='" + uomF + "'>" + uomF + "</option>";

                #region Comments

                //if (uoms == null || uoms.Count == 0)
                //{
                //    //string uomF = OrdinaryVATDesktop.StringReplacingForHTML(uomFrom);

                //    html += "<option value='" + uomF + "'>" + uomF + "</option>";
                //}

                #endregion Comments

                //////if (uoms != null || uoms.Count != 0)
                //////{
                //////    foreach (var item in uoms)
                //////    {
                //////        html += "<option value='" + item.UOMTo + "'>" + item.UOMTo + "</option>";

                //////        ////html += "<option value=" + item.UOMTo + ">" + item.UOMTo + "</option>";
                //////    }
                //////}

                //////html += "<option value=" + uomFrom + ">" + uomFrom + "</option>";
                //////foreach (var item in uoms)
                //////{
                //////    html += "<option value=" + item.UOMTo + ">" + item.UOMTo + "</option>";
                //////}

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

        [Authorize(Roles = "Admin")]
        public ActionResult ShowPurchaseInvoice(string PurchaseNo)
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
            var _repo = new PurchaseRepo(identity, Session);

            var vm = new PurchaseMasterVM();
            string[] conditionalFields = new string[] { "pih.PurchaseInvoiceNo" };
            string[] conditionalValues = new string[] { PurchaseNo };
            vm = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();
            vm.Operation = "detail";

            return PartialView("Create", vm);
        }

        public ActionResult Navigate(string id, string btn, string ttype)
        {
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetIdForTtype("PurchaseInvoiceHeaders", "Id", id, btn, ttype);
            return RedirectToAction("Edit", new { id = targetId, TransactionType = ttype });
        }

        public JsonResult GetRateCheck(List<PurchaseDetailVM> VMs)
        {
            var repo = new PurchaseRepo(identity, Session);
            try
            {
                VMs = _repo.RateCheck(VMs);
                return Json(VMs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                foreach (var vm in VMs)
                {
                    SymOrdinary.FileLogger.Log("Purchase", "SavePurchase", vm.ItemNo);
                }

                SymOrdinary.FileLogger.Log("Purchase", "SavePurchase", e.ToString());

                return Json(new List<PurchaseDetailVM>(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetConvFactor(string uomFrom, string UomTo)
        {
            var _repo = new UOMRepo(identity, Session);
            string[] conditionalFields = new string[] { "UOMFrom", "UOMTo" };
            string[] conditionalValues = new string[] { uomFrom, UomTo };
            var uom = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();
            decimal uomFactor = 1;//// = uom.Convertion;
            if (uom != null)
            {
                uomFactor = uom.Convertion;
            }
            return Json(uomFactor, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredItems(PurchaseMasterVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new PurchaseRepo(identity, Session);
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
            string[] conditionalFields;
            string[] conditionalValues;

            string vendor = null;
            if (vm.VendorID != "" && vm.VendorID != "0" && vm.VendorID != null)
            {
                vendor = vm.VendorID.ToString();
            }


            string IsVDSCompleted = "";

            if (vm.TransactionType == "VDS")
            {
                IsVDSCompleted = "N";
            }

            if (vm.SearchField != null)
            {
                if (vm.SearchField == "VendorName")
                {
                    vm.SearchField = "v.VendorName like";
                }
                else
                {
                    vm.SearchField = "pih." + vm.SearchField + " like";
                }
                conditionalFields = new string[] { "pih.InvoiceDateTime>", "pih.InvoiceDateTime<", "pih.Post", "pih.VendorID", "v.VendorGroupID", "pih.WithVDS", "pih.IsVDSCompleted isnull", vm.SearchField };
                conditionalValues = new string[] { vm.InvoiceDateTimeFrom, vm.InvoiceDateTimeTo, vm.Post, vendor, vm.VendorGroup, vm.WithVDS, IsVDSCompleted, vm.SearchValue };
            }
            else
            {
                if (vm.TransactionType == "PurchaseReturn")
                {

                    conditionalFields = new string[] { "pih.InvoiceDateTime>", "pih.InvoiceDateTime<", "pih.Post", "pih.VendorID", "v.VendorGroupID", "pih.WithVDS", "pih.IsVDSCompleted isnull", "pih.TransactionType" };
                    conditionalValues = new string[] { vm.InvoiceDateTimeFrom, vm.InvoiceDateTimeTo, "Y", vendor, vm.VendorGroup, vm.WithVDS, IsVDSCompleted, "Other" };
                }
                else
                {
                    conditionalFields = new string[] { "pih.InvoiceDateTime>", "pih.InvoiceDateTime<", "pih.Post", "pih.VendorID", "v.VendorGroupID", "pih.WithVDS", "pih.IsVDSCompleted isnull" };
                    conditionalValues = new string[] { vm.InvoiceDateTimeFrom, vm.InvoiceDateTimeTo, vm.Post, vendor, vm.VendorGroup, vm.WithVDS, IsVDSCompleted };
                }

            }

            var list = _repo.SelectAll(0, conditionalFields, conditionalValues);

            return PartialView("_filteredPurchases", list);
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
            vm.TargetId = targetId;
            vm.TransactionType = TransactionType;
            return PartialView("_purchasePopUp", vm);
        }

        public JsonResult GetVendor(string purchaseNo)
        {
            _repo = new PurchaseRepo(identity, Session);

            string[] conditionalFields = new string[] { "pih.PurchaseInvoiceNo" };
            string[] conditionalValues = new string[] { purchaseNo };

            var purchase = _repo.SelectAll(0, conditionalFields, conditionalValues).SingleOrDefault();


            string result = purchase.VendorID + "~" + purchase.VendorName;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new PurchaseRepo(identity, Session);
            string[] a = ids.Split('~');
            var id = a[0];
            PurchaseMasterVM vm = new PurchaseMasterVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            List<PurchaseDetailVM> PurchaseDetailVMS = new List<PurchaseDetailVM>();
            List<TrackingVM> TrackingVM = new List<TrackingVM>();
            PurchaseDetailVMS = _repo.SelectPurchaseDetail(vm.PurchaseInvoiceNo);
            TrackingVM = _repo.SelectTrakingsDetail(PurchaseDetailVMS, vm.PurchaseInvoiceNo, null);

            vm.Details = PurchaseDetailVMS;
            vm.Trackings = TrackingVM;
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";
            //result = _repo.PurchasePost(vm, vm.Details, new List<PurchaseDutiesVM>(), new List<TrackingVM>());
            result = _repo.PurchasePost(vm, vm.Details, new List<PurchaseDutiesVM>(), vm.Trackings);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult ImportExcel(PurchaseMasterVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new PurchaseRepo(identity, Session);
            string[] result = new string[6];
            try
            {
                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                vm.BranchCode = Session["BranchCode"].ToString();
                vm.CurrentUser = identity.UserId;
                result = _repo.ImportExcelFile(vm);
                Session["result"] = result[0] + "~" + result[1];
                //return View("Index", vm);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message.Replace("\r", "").Replace("\n", "");
                //SymOrdinary.FileLogger.Log("Purchase", "ImportExcel", ex.Message + "\n" + ex.StackTrace + "\n");
                // FileLogger.Log("Sale", "SaveSale", ex.Message + "\n" + ex.StackTrace + "\n");
                //return View("Index", vm);

                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin")]
        public JsonResult getVendorDetails(string VendorId)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new PurchaseRepo(identity, Session);
            var repo = new VendorRepo(identity, Session);
            var id = 0;
            try
            {
                id = Convert.ToInt32(VendorId);
            }
            catch (Exception)
            {
                throw;
            }
            var vehicle = repo.SelectAll(id).FirstOrDefault();
            return Json(vehicle, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult MultiplePost(PurchaseMasterVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new PurchaseRepo(identity, Session);
            #region Access Control

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


            try
            {

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

                string[] ids = paramVM.IDs.ToArray();

                result = _repo.MultiplePost(ids);

                rVM.Status = result[0];
                rVM.Message = result[1];


            }
            catch (Exception)
            {


            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult ExportExcell(PurchaseMasterVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new PurchaseRepo(identity, Session);
            #region Access Control

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


            try
            {
                string dtFrom = null;
                string dtTo = null;
                dtFrom = "2019-07-01 00:00:00";
                dtTo = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");

                if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeFrom))
                {
                    dtFrom = Convert.ToDateTime(paramVM.InvoiceDateTimeFrom).ToString("yyyyMMdd");
                }
                if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeTo))
                {

                    dtTo = Convert.ToDateTime(paramVM.InvoiceDateTimeTo).AddDays(1).ToString("yyyyMMdd");
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
                    paramVM.SelectTop = "All";
                }


                paramVM.CurrentUser = identity.UserId;

                if (paramVM.ExportAll)
                {
                    string[] conditionFields = new string[] { "pih.ReceiveDate>=", "pih.ReceiveDate<=", "pih.BranchId", "pih.TransactionType", "SelectTop" };
                    string[] conditionValues = new string[] { dtFrom, dtTo, paramVM.BranchId.ToString(), paramVM.TransactionType.ToString(), paramVM.SelectTop };

                    PurchaseRepo repo = new PurchaseRepo(identity, Session);

                    var list = repo.SelectAll(0, conditionFields, conditionValues);

                    paramVM.IDs = list.Select(x => x.Id).ToList();

                }


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
                    //rVM.Message = "No Data to Export";
                    //return Json(rVM, JsonRequestBehavior.AllowGet);

                    Session["result"] = "Fail" + "~" + "No Data to Export Please Select DateRange First";
                    return RedirectToAction("Index");

                }

                //string[] ids = paramVM.IDs.ToArray();


                DataTable dt = _repo.GetExcelDataWeb(paramVM.IDs);

                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }
                //// OrdinaryVATDesktop.SaveExcel(dt, "Purchase", "PurchaseM");
                //MessageBox.Show("Successfully Exported data in Excel files of root directory");
                var vm = OrdinaryVATDesktop.DownloadExcel(dt, "Purchase", "PurchaseM");
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + vm.FileName + ".xlsx");
                    vm.varExcelPackage.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                //rVM.Status = "Success";
                //rVM.Message = "Your requested information successfully Exported";

            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message.Replace("\r", "").Replace("\n", "");
                return RedirectToAction("Index");
            }

            finally { }
            return RedirectToAction("Index");

            // return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult SyncAll()
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new PurchaseRepo(identity, Session);

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
                return Redirect("/Vms/Purchase/Index?TransactionType=other");
            }
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

        public ActionResult PopUpCPCNameUpdatePurchase(PurchaseDetailVM vm)
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
            return PartialView("_PopUpCPCNameupdatePurchase", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult CPCNameUpdate(PurchaseDetailVM vm)
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
            if (vm.CPCName == null || vm.CPCName == "")
            {
                vm.CPCName = "-";
            }

            return PartialView("_detail", vm);
        }

        [Authorize]
        public ActionResult MultipleRebate(PurchaseMasterVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new PurchaseRepo(identity, Session);
            #region Access Control

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

            try
            {

                paramVM.CurrentUser = identity.UserId;

                if (paramVM != null && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    paramVM.IDs = paramVM.IDs.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

                    if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                    {
                        rVM.Message = "No Data to Rebate";
                        return Json(rVM, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    rVM.Message = "No Data to Rebate";
                    return Json(rVM, JsonRequestBehavior.AllowGet);
                }

                ////string[] ids = paramVM.IDs.ToArray();

                result = _repo.MultipleRebate(paramVM.IDs, paramVM.RebateDate);

                rVM.Status = result[0];
                rVM.Message = result[1];

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message.ToString();

            }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult MultiplePeriodDateTime(PurchaseMasterVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new PurchaseRepo(identity, Session);
            #region Access Control

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

            try
            {

                paramVM.CurrentUser = identity.UserId;

                if (paramVM != null && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    paramVM.IDs = paramVM.IDs.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

                    if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                    {
                        rVM.Message = "No Data to PeriodDateTime";
                        return Json(rVM, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    rVM.Message = "No Data to PeriodDateTime";
                    return Json(rVM, JsonRequestBehavior.AllowGet);
                }

                ////string[] ids = paramVM.IDs.ToArray();

                result = _repo.MultipleRebate(paramVM.IDs, paramVM.PeriodDateTime);

                rVM.Status = result[0];
                rVM.Message = result[1];

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message.ToString();

            }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PopUpPreviousPurchase(PurchaseDetailVM vm)
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


    }
}
