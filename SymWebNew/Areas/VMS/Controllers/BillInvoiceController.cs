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
using SymphonySofttech.Reports;
using CrystalDecisions.Shared;
using System.Configuration;
using VATServer.Ordinary;
using System.IO;
using Newtonsoft.Json;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.vms.Controllers
{
    [ShampanAuthorize]
    public class BillInvoiceController : Controller
    {
        //
        // GET: /vms/Branch/
        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        BillInvoiceRepo _repo = null;
        ShampanIdentity identity = null;

        public BillInvoiceController()
        {
            try
            {

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new BillInvoiceRepo(identity);
            }
            catch
            {

            }
            ////identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            ////_repo = new BillInvoiceRepo(identity);
           
        }

        [Authorize(Roles = "Admin")]
        public ActionResult HomeIndex()
        {
            return View();
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Index(BillInvoiceMasterVM paramVM)
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
        public ActionResult _index(JQueryDataTableParamModel param, BillInvoiceMasterVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BillInvoiceRepo(identity, Session);
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
            //string searchedBranchId = "0";
            string dtFrom = DateTime.Now.ToString("yyyyMMdd");
            string dtTo = DateTime.Now.AddDays(1).ToString("yyyyMMdd");

            if (!string.IsNullOrWhiteSpace(paramVM.BillDateTimeFrom))
            {
                dtFrom = Convert.ToDateTime(paramVM.BillDateTimeFrom).ToString("yyyyMMdd");
            }
            if (!string.IsNullOrWhiteSpace(paramVM.BillDateTimeTo))
            {
                dtTo = Convert.ToDateTime(paramVM.BillDateTimeTo).AddDays(1).ToString("yyyyMMdd");
            }
           
            if (paramVM.BranchId == 0)
            {
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            }

            if (paramVM.BranchId == -1)
            {
                paramVM.BranchId = 0;
            }

            #endregion Search and Filter Data
            List<BillInvoiceMasterVM> getAllData = new List<BillInvoiceMasterVM>();
            if (!identity.IsAdmin)
            {
                string[] conditionFields = { "b.BillNo like", "b.ChallanNo like", "b.BillDate>=", "b.BillDate<", "b.Post", "b.TransactionType"};
                string[] conditionValues = { paramVM.BillNo, paramVM.ChallanNo, dtFrom, dtTo, paramVM.Post, paramVM.TransactionType};
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            else
            {
                string[] conditionFields = { "b.BillNo like", "b.ChallanNo like", "b.BillDate>=", "b.BillDate<", "b.Post", "b.TransactionType" };
                string[] conditionValues = { paramVM.BillNo, paramVM.ChallanNo, dtFrom, dtTo, paramVM.Post, paramVM.TransactionType };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            //var getAllData = _repo.SelectAll();
            IEnumerable<BillInvoiceMasterVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                //var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);



                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.BillNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.BillDate.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.PONo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.ChallanNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    //|| isSearchable5 && c.DepositPerson.ToString().ToLower().Contains(param.sSearch.ToLower())

                    );
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<BillInvoiceMasterVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.BillNo :
                sortColumnIndex == 2 && isSortable_2 ? c.BillDate :
                sortColumnIndex == 3 && isSortable_3 ? c.PONo.ToString() :
                sortColumnIndex == 3 && isSortable_4 ? c.ChallanNo.ToString() :
                //sortColumnIndex == 3 && isSortable_5 ? c.DepositPerson.ToString() :
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
                ,c.BillNo
                ,c.BillDate
                ,c.PONo
                ,c.ChallanNo.ToString()
                //,c.DepositPerson
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
        [HttpGet]
        public ActionResult Create(BillInvoiceMasterVM vm)
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

            if (string.IsNullOrWhiteSpace(vm.TransactionType))
            {
                vm.TransactionType = "Treasury";
            }
            vm.Details = new List<BillInvoiceDetailVM>();
            vm.Operation = "add";       
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(BillInvoiceMasterVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BillInvoiceRepo(identity, Session);

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
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.LastModifiedBy = identity.Name;
                    ////vm.TransactionType = "VDS";
                    vm.Post = "N";
                    //vm.DepositId = "0";
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                    result = _repo.BillInvoiceInsert(vm,vm.Details);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0] == "Success")
                    {
                        return RedirectToAction("Edit", new { id = result[4], TransactionType = vm.TransactionType });
                    }
                    else
                    {
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                    AdjustmentHistoryVM adjHistory = new AdjustmentHistoryVM();
                    //result = _repo.DepositUpdate(vm, vm.Details, adjHistory);
             
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0] == "Success")
                    {
                        return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.TransactionType });
                    }
                    else
                    {
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
                //Session["result"] = "Fail~Data not Successfully";
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id, string TransactionType)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new BillInvoiceRepo(identity, Session);
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

            if (TransactionType == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string[] conditionFields = new string[] { "b.TransactionType" };
            string[] conditionValues = new string[] { TransactionType };

            BillInvoiceMasterVM vm = new BillInvoiceMasterVM();
            vm = _repo.SelectAll(Convert.ToInt32(id),conditionFields,conditionValues).FirstOrDefault();
            //vm.TreasuryN = vm.TreasuryNo;
            if (vm == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //var vds = new VDSRepo(identity, Session);
            List<BillInvoiceDetailVM> BillDetailVMs = new List<BillInvoiceDetailVM>();
            BillDetailVMs = _repo.SelectAllDetails(Convert.ToInt32(vm.Id), null, null);

            vm.Details = BillDetailVMs;
            vm.Operation = "update";
            return View("Create", vm);
        }


        [Authorize(Roles = "Admin")]
        public JsonResult GetBankInformation(string bankId)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            var _repo = new BankInformationRepo(identity, Session);

            var bank = _repo.SelectAll(Convert.ToInt32(bankId)).FirstOrDefault();
            var branchName = bank.BranchName;
            var accountNo = bank.AccountNumber;
            var district = bank.City;
            var depositPersonContactNo = bank.ContactPersonTelephone;
            var depositPersonAddress = bank.Address1;

            string result = branchName + "~" + accountNo + "~" + district + "~" + depositPersonContactNo + "~" + depositPersonAddress;

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        public JsonResult SelectPurchase(string PurchaseNo)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            var _repo = new PurchaseRepo(identity, Session);
            string[] conditionalFields = new string[] { "pih.PurchaseInvoiceNo" };
            string[] conditionalValues = new string[] { PurchaseNo };

            PurchaseMasterVM vm = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();
            ////vm.TotalVDSAmount
            ////var billAmount = purchaseVM.TotalAmount;
            ////var vendorId = purchaseVM.VendorID;
            ////var vendorName = purchaseVM.VendorName;
            ////var purchaseDate = purchaseVM.InvoiceDate;

            ////string result = billAmount + "~" + vendorId + "~" + vendorName + "~" + purchaseDate;

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult SelectSale(string SaleNo)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            var _repo = new SaleInvoiceRepo(identity, Session);
            string[] conditionalFields = new string[] { "sih.SalesInvoiceNo" };
            string[] conditionalValues = new string[] { SaleNo };

            SaleMasterVM vm = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();

            ////var billAmount = vm.TotalAmount;
            ////var vendorId = vm.CustomerID;
            ////var vendorName = vm.CustomerName;
            ////var purchaseDate = vm.InvoiceDateTime;

            ////string result = billAmount + "~" + vendorId + "~" + vendorName + "~" + purchaseDate;

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        //[Authorize(Roles = "Admin")]
        //public ActionResult BlankItem(VDSMasterVM vm)
        //{
        //    string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
        //    if (project.ToLower() == "vms")
        //    {
        //        if (!identity.IsAdmin)
        //        {

        //        }
        //    }
        //    else
        //    {
        //        Session["rollPermission"] = "deny";
        //        return Redirect("/Vms/Home");
        //    }

        //    if (string.IsNullOrWhiteSpace(vm.PurchaseNumber))
        //    {
        //        vm.PurchaseNumber = "NA";
        //    }

        //    vm.VDSPercent = vm.BillDeductedAmount * 100 / vm.BillAmount;

        //    List<VDSMasterVM> vms = new List<VDSMasterVM>();
        //    vms.Add(vm);

        //    return PartialView("_detail", vms);
        //}

        //public ActionResult BlankItems(string[] ids, string IsPercent, string isPurchase)
        //{
        //    identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        //    string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
        //    if (project.ToLower() == "vms")
        //    {
        //        if (!identity.IsAdmin)
        //        {

        //        }
        //    }
        //    else
        //    {
        //        Session["rollPermission"] = "deny";
        //        return Redirect("/Vms/Home");
        //    }

        //    PurchaseRepo _repo = new PurchaseRepo(identity, Session);
        //    List<VDSMasterVM> vms = new List<VDSMasterVM>();

        //    foreach (string PurchaseNo in ids)
        //    {
        //        VDSMasterVM VDSVM = new VDSMasterVM();

        //        string[] conditionalFields = new string[] { "pih.PurchaseInvoiceNo" };
        //        string[] conditionalValues = new string[] { PurchaseNo };

        //        PurchaseMasterVM vm = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();

        //        VDSVM.BillNo = vm.BENumber;
        //        VDSVM.VendorId = vm.VendorID;
        //        VDSVM.VendorName = vm.VendorName;
        //        VDSVM.BillAmount = vm.TotalAmount;
        //        VDSVM.BillDate = vm.InvoiceDate;
        //        VDSVM.BillDeductedAmount = vm.TotalVDSAmount;
        //        VDSVM.IssueDate = DateTime.Now.ToString("dd-MMM-yyyy");
        //        VDSVM.PurchaseNumber = vm.PurchaseInvoiceNo;
        //        VDSVM.IsPercent = IsPercent;
        //        VDSVM.IsPurchase = isPurchase;
        //        VDSVM.Remarks = "-";

        //        if (string.IsNullOrWhiteSpace(VDSVM.PurchaseNumber))
        //        {
        //            VDSVM.PurchaseNumber = "NA";
        //        }

        //        VDSVM.VDSPercent = VDSVM.BillDeductedAmount * 100 / VDSVM.BillAmount;

        //        vms.Add(VDSVM);

        //    }

        //    return PartialView("_detail", vms);
        //}


        public ActionResult Navigate(string id, string btn, string ttype)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetIdForTtype("Deposits", "Id", id, btn, ttype);
            return RedirectToAction("Edit", new { id = targetId, TransactionType = ttype });
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetItemPopUpBillInvoice(string targetId, string TransactionType)
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
            return PartialView("_salePopUpBillInvoice", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItems(string saleNo, string InvoiceDate, bool SearchPreviousForCNDN = false)
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                SaleInvoiceRepo _repo = null;
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

                List<BillInvoiceDetailVM> VMSs = new List<BillInvoiceDetailVM>();
                foreach (var item in vms)
                {
                    BillInvoiceDetailVM vm = new BillInvoiceDetailVM();
                    vm.Quantity = item.Quantity;
                    vm.SubTotal = item.SubTotal;
                    vm.VATAmount = item.VATAmount;
                    vm.VATRate = item.VATRate;
                    vm.Post = item.Post;
                    vm.ItemNo = item.ItemNo;
                    vm.NBRPrice = item.NBRPrice;

                    VMSs.Add(vm);                   
                }


                ////return PartialView("_detailMultiple", VMSs);
                return PartialView("_detail", VMSs);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);

                return RedirectToAction("Index");

            }
        }

        [Authorize]
        //[HttpPost]
        public ActionResult PreviewReport_BillInvoice(Vat16ViewModel paramVM)
        {
            ReportDocument reportDocument = new ReportDocument();

            ResultVM rVM = new ResultVM();
            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                //StaticValueReAssign(identity);

                //connVM = Ordinary.StaticValueReAssign(identity, Session);

                if (string.IsNullOrWhiteSpace(paramVM.BillNo))
                {
                    if (paramVM != null && paramVM.IDs != null && paramVM.IDs.Count > 0)
                    {
                        paramVM.IDs = paramVM.IDs.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

                        if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                        {
                            rVM.Message = "No Data to Preview!";
                            Session["result"] = rVM.Status + "~" + rVM.Message;
                            return Redirect("/VMS/Home");
                        }
                    }
                    else
                    {
                        rVM.Message = "No Data to Preview!";
                        Session["result"] = rVM.Status + "~" + rVM.Message;
                        return Redirect("/VMS/Home");

                        ////return RedirectToAction("Index");
                    }
                }

                string IDs = "";
                //IDs = string.Join("','", paramVM.IDs);

                if (!string.IsNullOrWhiteSpace(paramVM.BillNo))
                {
                    IDs = paramVM.BillNo;
                }
                else
                {
                    IDs = string.Join("','", paramVM.IDs);
                }

                //IDs = "'" + IDs + "'";

                SaleReport _reportClass = new SaleReport();
                //bool PreviewOnly = true;
                reportDocument = _reportClass.BillInvoiceReportNew(IDs);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);


                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception ex)
            {
                FileLogger.Log("Sale", "SaveSale", ex.Message + "\n");

                return RedirectToAction("Index");
            }
            finally
            {
                if (reportDocument != null)
                {
                    reportDocument.Close();
                    reportDocument.Dispose();
                }
            }

        }



    }
}
