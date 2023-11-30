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
    public class DepositController : Controller
    {
        //
        // GET: /vms/Branch/
        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        DepositRepo _repo = null;
        ShampanIdentity identity = null;

        public DepositController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new DepositRepo(identity);

            }
            catch
            {

            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult HomeIndex()
        {
            return View();
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Index(DepositMasterVM paramVM)
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
                paramVM.TransactionType = "Treasury";
            }


            return View(paramVM);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param, DepositMasterVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new DepositRepo(identity, Session);
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
            //00     //Id
            //01     //DepositId
            //02     //TreasuryNo
            //03     //DepositDateTime
            //04     //DepositAmount
            //05     //DepositPerson

            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "100";
            }

            #region Search and Filter Data
            //string searchedBranchId = "0";
            string dtFrom = DateTime.Now.ToString("yyyyMMdd");
            string dtTo = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
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
            if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTimeFrom))
            {
                dtFrom = Convert.ToDateTime(paramVM.IssueDateTimeFrom).ToString("yyyyMMdd");
            }
            if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTimeTo))
            {
                dtTo = Convert.ToDateTime(paramVM.IssueDateTimeTo).AddDays(1).ToString("yyyyMMdd");
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
            List<DepositMasterVM> getAllData = new List<DepositMasterVM>();

            string transactionOpening = paramVM.TransactionType + "-Opening";

            if (!identity.IsAdmin)
            {
                string[] conditionFields = { "d.DepositId like", "d.TreasuryNo like", "d.DepositDateTime>=", "d.DepositDateTime<", "d.DepositType", "d.ChequeNo like", "d.ChequeDate>=", "d.ChequeDate<=", "b.BankName like", "b.AccountNumber like", "d.Post", "d.TransactionType", "SelectTop" };
                string[] conditionValues = { paramVM.DepositId, paramVM.TreasuryNo, dtFrom, dtTo, paramVM.DepositType, paramVM.ChequeNo, paramVM.CheckDateFrom, paramVM.CheckDateTo, paramVM.BankName, paramVM.AccountNumber, paramVM.Post, paramVM.TransactionType, paramVM.SelectTop };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues, transactionOpening);
            }
            else
            {
                string[] conditionFields = { "d.DepositId like", "d.TreasuryNo like", "d.DepositDateTime>=", "d.DepositDateTime<", "d.DepositType", "d.ChequeNo like", "d.ChequeDate>=", "d.ChequeDate<=", "b.BankName like", "b.AccountNumber like", "d.Post", "d.TransactionType", "SelectTop", "d.BranchId" };
                string[] conditionValues = { paramVM.DepositId, paramVM.TreasuryNo, dtFrom, dtTo, paramVM.DepositType, paramVM.ChequeNo, paramVM.CheckDateFrom, paramVM.CheckDateTo, paramVM.BankName, paramVM.AccountNumber, paramVM.Post, paramVM.TransactionType, paramVM.SelectTop, paramVM.BranchId.ToString() };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues, transactionOpening);
            }
            //var getAllData = _repo.SelectAll();
            IEnumerable<DepositMasterVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);



                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.DepositId.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.TreasuryNo.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.DepositDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.DepositAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.DepositPerson.ToString().ToLower().Contains(param.sSearch.ToLower())

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
            Func<DepositMasterVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.DepositId :
                sortColumnIndex == 2 && isSortable_2 ? c.TreasuryNo :
                sortColumnIndex == 3 && isSortable_3 ? c.DepositDate.ToString() :
                sortColumnIndex == 3 && isSortable_4 ? c.DepositAmount.ToString() :
                sortColumnIndex == 3 && isSortable_5 ? c.DepositPerson.ToString() :
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
                ,c.DepositId
                ,c.TreasuryNo
                ,c.DepositDate
                ,c.DepositAmount.ToString()
                ,c.DepositPerson
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
        public ActionResult Create(DepositMasterVM vm)
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
            vm.Details = new List<VDSMasterVM>();
            vm.Operation = "add";
            vm.SingleTR6 = true;
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(DepositMasterVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new DepositRepo(identity, Session);

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
                    vm.DepositId = "0";
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                    AdjustmentHistoryVM adjHistory = new AdjustmentHistoryVM();
                    result = _repo.DepositInsert(vm, vm.Details, adjHistory);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0] == "Success")
                    {                       
                        _repo.UpdateVdsComplete("Y", result[2],null,vm.TransactionType);

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
                    result = _repo.DepositUpdate(vm, vm.Details, adjHistory);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0] == "Success")
                    {
                        _repo.UpdateVdsComplete("Y", result[2], null, vm.TransactionType);

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

            _repo = new DepositRepo(identity, Session);
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

            string[] conditionFields = new string[] { "d.TransactionType" };
            string[] conditionValues = new string[] { TransactionType };

            DepositMasterVM vm = new DepositMasterVM();
            vm = _repo.SelectAll(Convert.ToInt32(id), conditionFields, conditionValues).FirstOrDefault();
            vm.TreasuryN = vm.TreasuryNo;
            if (vm == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var vds = new VDSRepo(identity, Session);
            vm.Details = vds.SelectVDSDetail(vm.DepositId, null, null, null, null, TransactionType);
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

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(VDSMasterVM vm)
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

            if (string.IsNullOrWhiteSpace(vm.PurchaseNumber))
            {
                vm.PurchaseNumber = "NA";
            }

            vm.VDSPercent = vm.BillDeductedAmount * 100 / vm.BillAmount;

            List<VDSMasterVM> vms = new List<VDSMasterVM>();
            vms.Add(vm);

            return PartialView("_detail", vms);
        }

        public ActionResult BlankItems(string[] ids, string IsPercent, string isPurchase)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

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

            PurchaseRepo _repo = new PurchaseRepo(identity, Session);
            List<VDSMasterVM> vms = new List<VDSMasterVM>();

            foreach (string PurchaseNo in ids)
            {
                VDSMasterVM VDSVM = new VDSMasterVM();

                string[] conditionalFields = new string[] { "pih.PurchaseInvoiceNo" };
                string[] conditionalValues = new string[] { PurchaseNo };

                PurchaseMasterVM vm = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();

                VDSVM.BillNo = vm.BENumber;
                VDSVM.VendorId = vm.VendorID;
                VDSVM.VendorName = vm.VendorName;
                VDSVM.BillAmount = vm.TotalAmount;
                VDSVM.BillDate = vm.InvoiceDate;
                VDSVM.BillDeductedAmount = vm.TotalVDSAmount;
                VDSVM.IssueDate = DateTime.Now.ToString("dd-MMM-yyyy");
                VDSVM.PurchaseNumber = vm.PurchaseInvoiceNo;
                VDSVM.IsPercent = IsPercent;
                VDSVM.IsPurchase = isPurchase;
                VDSVM.Remarks = "-";

                if (string.IsNullOrWhiteSpace(VDSVM.PurchaseNumber))
                {
                    VDSVM.PurchaseNumber = "NA";
                }

                VDSVM.VDSPercent = VDSVM.BillDeductedAmount * 100 / VDSVM.BillAmount;

                vms.Add(VDSVM);

            }

            return PartialView("_detail", vms);
        }


        public ActionResult Navigate(string id, string btn, string ttype)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetIdForTtype("Deposits", "Id", id, btn, ttype);
            return RedirectToAction("Edit", new { id = targetId, TransactionType = ttype });
        }

        public ActionResult GetDepositPopUp(string targetId)
        {
            PopUpViewModel vm = new PopUpViewModel();
            vm.TargetId = targetId;
            return PartialView("_deposits", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredDeposits(DepositMasterVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

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
            string[] conditionalFields = new string[] { "d.DepositDateTime<", "d.DepositDateTime>", "d.Post" };
            string[] conditionalValues = new string[] { vm.IssueDateTimeFrom, vm.IssueDateTimeTo, vm.Post };
            var list = _repo.SelectAll(0, conditionalFields, conditionalValues);

            return PartialView("_filteredDeposits", list);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new DepositRepo(identity, Session);


            string[] a = ids.Split('~');
            var id = a[0];
            DepositMasterVM vm = new DepositMasterVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            List<VDSMasterVM> DepositDetailVMS = new List<VDSMasterVM>();
            VDSRepo vdsRepo = new VDSRepo(identity, Session);
            DepositDetailVMS = vdsRepo.SelectVDSDetail(vm.DepositId);
            vm.Details = DepositDetailVMS;
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";
            result = _repo.DepositPost(vm, vm.Details, new AdjustmentHistoryVM());
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult MultiplePost(DepositMasterVM paramVM)
        {
            #region Access Control
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new DepositRepo(identity, Session);

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

                rVM = _repo.MultiplePost(paramVM);

            }
            catch (Exception)
            {


            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportTR(DepositMasterVM vm)
        {

            try
            {
                ReportClass objrpt;

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                DataSet ReportResult;
                DataSet ReportSubResult = new DataSet();
                GenericReport<ReportClass> gr = new GenericReport<ReportClass>();
                CompanyProfileVM varCompanyProfileVM = new CompanyProfileVM();
                CompanyProfileRepo _CompanyProfileRepo = new CompanyProfileRepo(identity); // need change

                if (vm.DepositId == null)
                {
                    vm.DepositId = "";
                }
                if (vm.TransactionType.ToLower() == "vds")
                {
                    if (vm.SingleTR6)
                    {
                        ReportResult = repo.TrasurryDepositeNew(vm.DepositId);
                        ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                        objrpt = new RptTR();
                        //objrpt.Load(Program.ReportAppPath + @"\RptTR.rpt");

                        objrpt.SetDataSource(ReportResult);
                        objrpt.DataDefinition.FormulaFields["SingleTR6"].Text = "'Y'";
                    }
                    else
                    {
                        //ReportResult = repo.TrasurryDepositeNew(vm.DepositId);
                        //ReportSubResult = repo.VDSDepositNew(vm.DepositId);
                        ReportResult = repo.VDSDepositNew(vm.DepositId);

                        //ReportSubResult.Tables[0].TableName = "DsVDSDeposit";
                        ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";

                        objrpt = new RptTRVDS();
                        objrpt.SetDataSource(ReportResult);
                        //objrpt.Subreports[0].SetDataSource(ReportSubResult);
                        //objrpt.Subreports[1].SetDataSource(ReportSubResult);

                    }
                }
                else
                {
                    ReportResult = repo.TrasurryDepositeNew(vm.DepositId);
                    ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                    objrpt = new RptTR();
                    objrpt.SetDataSource(ReportResult);
                    objrpt.DataDefinition.FormulaFields["SingleTR6"].Text = "'N'";
                }

                //if (vm.TransactionType.ToLower() != "vds")
                //{
                //    ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                //    objrpt = new RptTR();
                //    objrpt.SetDataSource(ReportResult);
                //    objrpt.DataDefinition.FormulaFields["SingleTR6"].Text = "'N'";
                //}

                //else
                //{
                //    if (vm.SingleTR6)
                //    {
                //        ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";
                //        objrpt = new RptTR();
                //        //objrpt.Load(Program.ReportAppPath + @"\RptTR.rpt");

                //        objrpt.SetDataSource(ReportResult);
                //        objrpt.DataDefinition.FormulaFields["SingleTR6"].Text = "'Y'";

                //    }
                //    else
                //    {
                //        ReportSubResult.Tables[0].TableName = "DsVDSDeposit";
                //        ReportResult.Tables[0].TableName = "DsTrasurryDepositeNew";

                //        objrpt = new RptTRVDS();
                //        objrpt.SetDataSource(ReportResult);
                //        objrpt.Subreports[0].SetDataSource(ReportSubResult);
                //        objrpt.Subreports[1].SetDataSource(ReportSubResult);
                //    }
                //}
                /*
                 objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                 objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                 objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                 objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";
                 objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'VatRegistrationNo'";
                 objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'ZipCode'";
                 objrpt.DataDefinition.FormulaFields["VATCircle"].Text = "'Comments'";
                 */
                string CompanyId = Session[identity.InitialCatalog].ToString();

                if (varCompanyProfileVM == null)
                {
                    throw new Exception("Company Not found " + CompanyId);
                }


                varCompanyProfileVM = _CompanyProfileRepo.SelectAll(CompanyId).FirstOrDefault();
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + varCompanyProfileVM.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + varCompanyProfileVM.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + varCompanyProfileVM.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + varCompanyProfileVM.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + varCompanyProfileVM.VatRegistrationNo + "'";
                objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + varCompanyProfileVM.ZipCode + "'";
                objrpt.DataDefinition.FormulaFields["VATCircle"].Text = "";
                objrpt.DataDefinition.FormulaFields["InEnglish"].Text = "'N'";

                string copiesNo = "";
                int cpno = 0;
                #region CopyName

                for (int i = 1; i <= 3; i++)
                {
                    cpno = i;
                    if (cpno == 1)
                    {
                        copiesNo = cpno + " st copy";
                    }
                    else if (cpno == 2)
                    {
                        copiesNo = cpno + " nd copy";
                    }
                    else if (cpno == 3)
                    {
                        copiesNo = cpno + " rd copy";
                    }
                    else
                    {
                        copiesNo = cpno + " copy";
                    }

                }

                #endregion CopyName
                //objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "'" + copiesNo + "'";

                //objrpt.SetDataSource(ReportResult);

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
        public ActionResult PrintVAT6_6(string DepositNo, string TransactionType)
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
            DepositMISViewModel vm = new DepositMISViewModel();
            vm.TransactionType = TransactionType;
            return PartialView("_printVAT6_6", vm);
        }

        public ActionResult GetVAT6_6PopUp(string targetId)
        {
            PopUpViewModel vm = new PopUpViewModel();
            vm.TargetId = targetId;
            return PartialView("_VAT6_6Search", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredVAT6_6(DepositMasterVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new DepositRepo(identity, Session);

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
            string[] conditionalFields = new string[] { "d.DepositId like", "d.DepositDateTime<", "d.DepositDateTime>", "d.Post" };
            string[] conditionalValues = new string[] { vm.DepositId, vm.IssueDateTimeFrom, vm.IssueDateTimeTo, vm.Post };
            var list = _repo.SelectAll(0, conditionalFields, conditionalValues);

            return PartialView("_filteredVAT6_6", list);
        }

        [Authorize]
        //[HttpPost]
        public ActionResult Report_VAT6_6(DepositMISViewModel vm)
        {
            try
            {
                var post = vm.Post ? "Y" : "N";
                #region Date Format

                string DepositDateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string DepositDateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.DepositDateFrom))
                {
                    DepositDateFrom = Convert.ToDateTime(vm.DepositDateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.DepositDateTo))
                {
                    DepositDateTo = Convert.ToDateTime(vm.DepositDateTo).ToString("yyyy-MM-dd");
                }

                #endregion
                NBRReports _reportClass = new NBRReports();
                bool PreviewOnly = true;
                ReportDocument reportDocument = new ReportDocument();

                bool IsPurchase = true;
                if (vm.TransactionType != "VDS")
                {
                    IsPurchase = false;
                }


                reportDocument = _reportClass.VDS12KhaNew("", vm.DepositNo, DepositDateFrom, DepositDateTo, "", "", "", "", "", IsPurchase, null);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

        }


        [Authorize]
        [HttpGet]
        public ActionResult GetUserInformationPopUp(PopUpViewModel vm)
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
            return PartialView("_UserInformation", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredUserInformation(UserInformationVM vm)
        {
            UserInformationRepo _repo = new UserInformationRepo(identity, Session);

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
            var activeSatus = vm.ActiveStatus == true ? "Y" : "N";
            string[] conditionalFields;
            string[] conditionalValues;

            conditionalFields = new string[] { "ui.ActiveStatus" };
            conditionalValues = new string[] { activeSatus };

            var list = _repo.SelectAll(0, conditionalFields, conditionalValues);

            return PartialView("_filteredUserInformation", list);
        }

        [Authorize]
        [HttpGet]
        public ActionResult ExportExcell(DepositMasterVM paramVM)
        {
            #region Access Control
            // _repo = new UserInformationRepo(identity, Session);
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new DepositRepo(identity, Session);

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

            List<DepositMasterVM> getAllData = new List<DepositMasterVM>();

            try
            {

                #region SeachParameters

                string dtFrom = null;
                string dtTo = null;


                dtFrom = DateTime.Now.ToString("yyyyMMdd");
                dtTo = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
                if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTimeFrom))
                {
                    dtFrom = Convert.ToDateTime(paramVM.IssueDateTimeFrom).ToString("yyyyMMdd");
                }
                if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTimeTo))
                {
                    dtTo = Convert.ToDateTime(paramVM.IssueDateTimeTo).AddDays(1).ToString("yyyyMMdd");
                }


                if (paramVM.BranchId == 0)
                {
                    paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
                {
                    paramVM.SelectTop = "All";
                }

                #endregion SeachParameters

                #region Data Call

                string[] conditionFields;
                string[] conditionValues;

                ////conditionFields = new string[] { "dp.InvoiceDateTime>=", "dp.InvoiceDateTime<=", "dp.DepositId", "dp.TreasuryNo", "dp.DepositType", "dp.Post", "dp.BranchId", "SelectTop" };
                ////conditionValues = new string[] { dtFrom, dtTo, paramVM.DepositId, paramVM.TreasuryNo, paramVM.DepositType, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SelectTop };



                //conditionFields = new string[] { "sih.InvoiceDateTime>=", "sih.InvoiceDateTime<=", "sih.SaleType", "sih.CustomerID", "sih.IsPrint", "sih.Post", paramVM.SearchField, "sih.BranchId", "sih.IsInstitution", "SelectTop" };
                //conditionValues = new string[] { dtFrom, dtTo, paramVM.SaleType, paramVM.CustomerID, paramVM.IsPrint, paramVM.Post, paramVM.SearchValue, paramVM.BranchId.ToString(), paramVM.IsInstitution, paramVM.SelectTop };

                if (paramVM.ExportAll)
                {
                    //conditionFields = new string[] { "dp.InvoiceDateTime>=", "dp.InvoiceDateTime<=", "dp.DepositId", "dp.TreasuryNo", "dp.DepositType", "dp.Post", "dp.BranchId", "SelectTop" };
                    //conditionValues = new string[] { dtFrom, dtTo, paramVM.DepositId, paramVM.TreasuryNo, paramVM.DepositType, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SelectTop };

                    conditionFields = new string[] { "d.DepositId like", "d.TreasuryNo like", "d.DepositDateTime>=", "d.DepositDateTime<", "d.DepositType", "d.ChequeNo like", "d.ChequeDate>=", "d.ChequeDate<=", "b.BankName like", "b.AccountNumber like", "d.Post", "d.TransactionType", "SelectTop" };
                    conditionValues = new string[] { paramVM.DepositId, paramVM.TreasuryNo, dtFrom, dtTo, paramVM.DepositType, paramVM.ChequeNo, paramVM.CheckDateFrom, paramVM.CheckDateTo, paramVM.BankName, paramVM.AccountNumber, paramVM.Post, paramVM.TransactionType, paramVM.SelectTop };



                    getAllData = _repo.SelectAll(0, conditionFields, conditionValues);

                    paramVM.IDs = getAllData.Select(x => x.DepositId).ToList();

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
                }
                else
                {
                    rVM.Message = "No Data to Export";
                    return Json(rVM, JsonRequestBehavior.AllowGet);
                }

                DataTable dt = new DataTable();

                // DataTable dt = _repo.GetVDSExcelDataWeb(paramVM.IDs);

                if (paramVM.TransactionType != "SaleVDS")
                {
                    dt = _repo.GetVDSExcelDataWeb(paramVM.IDs);

                }
                else
                {
                    dt = _repo.GetVDSExcelDataWithcustomerWeb(paramVM.IDs);

                }
                dt.Columns["Deposit_Date"].ColumnName = "Effect_Date";


                if (paramVM.TransactionType == "SaleVDS")
                {
                    dt.Columns["Treasury_No"].ColumnName = "VDS_Certificate_No";
                    dt.Columns["BankDepositDate"].ColumnName = "VDS_Certificate_Date";
                    dt.Columns["Cheque_No"].ColumnName = "Tax_Deposit_Account_Code";
                    dt.Columns["Cheque_Date"].ColumnName = "Tax_Deposit_Date";
                    dt.Columns["Cheque_Bank"].ColumnName = "Tax_Deposit_Serial_No";

                }



                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                var vm = OrdinaryVATDesktop.DownloadExcel(dt, "VDS", "VDSM");
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

        [HttpPost]
        public ActionResult ImportExcel(DepositMasterVM vm)
        {
            string[] result = new string[6];
            try
            {

                _repo = new DepositRepo(identity, Session);

                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                vm.BranchCode = Session["BranchCode"].ToString();
                vm.CurrentUser = identity.UserId;
                result = _repo.ImportExcelFile(vm);
                Session["result"] = result[0] + "~" + result[1];
                //return Json(JsonConvert.SerializeObject(new { message = "Saved Successfully", action = result[0] }),
                //    JsonRequestBehavior.AllowGet);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                //Session["result"] = result[0] + "~" + result[1];
                Session["result"] = "Fail" + "~" + e.Message.Replace("\r", "").Replace("\n", "");
                //return Json(JsonConvert.SerializeObject(new { message = e.Message, action = "Fail" }),
                //    JsonRequestBehavior.AllowGet);
                return RedirectToAction("Index");

            }
        }

    }
}
