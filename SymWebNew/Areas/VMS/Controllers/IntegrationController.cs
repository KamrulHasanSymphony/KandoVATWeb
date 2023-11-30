using SymOrdinary;
using SymRepository.VMS;
using VATViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Newtonsoft.Json;
using SymVATWebUI.Filters;
using SymphonySofttech.Reports;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using SymVATWebUI.Areas.VMS.Models;
using SymphonySofttech.Utilities;
using VATServer.Ordinary;
using SymVATWebUI.Filters;


namespace SymVATWebUI.Areas.VMS.Controllers
{
    [ShampanAuthorize]
    public class IntegrationController : Controller
    {
        //
        // GET: /VMS/Integration/

        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        ShampanIdentity identity = null;
        IntegrationRepo _IntegrationRepo = null;
        CommonRepo _CommonRepo = null;
        public IntegrationController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                _IntegrationRepo = new IntegrationRepo(identity);
                _CommonRepo = new CommonRepo(identity);

            }
            catch
            {

            }

        }
        public ActionResult Index()
        {
            return View();
        }


        #region BCL - Beximco Communication Ltd.

        #region Sale Actions

        [Authorize]
        [HttpGet]
        public ActionResult ViewSaleForm_BCL(PopUpViewModel vm)
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

            return PartialView("_SaleHeader_BCL", vm);
        }

        [Authorize]
        ////[HttpGet]
        public ActionResult GetSaleData_BCL(IntegrationParam vm)
        {
            #region Access Control

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

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

            DataTable dtSale = new DataTable();

            vm.BranchCode = Session["BranchCode"].ToString();
            dtSale = _IntegrationRepo.GetSource_SaleData_Master_BCL_Trading(vm);

            return PartialView("_SaleBody_BCL", dtSale);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetSaleData_Detail_BCL(IntegrationParam vm)
        {
            #region Access Control

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

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


            DataTable dtSale = new DataTable();
            vm.BranchCode = Session["BranchCode"].ToString();

            dtSale = _IntegrationRepo.GetSale_DBData_BCL_Trading(vm);

            if (vm.TransactionType == "Credit")
            {
                return PartialView("_SaleBody_Detail_BCL", dtSale);
            }
            else
            {
                return PartialView("_CreditBody_Detail_BCL", dtSale);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveSale_BCL(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

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


            string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
            Session["currentExcelToken"] = Token;
            vm.CurrentUser = identity.UserId;
            vm.Token = Token;

            vm.BranchCode = Session["BranchCode"].ToString();

            if (vm.TransactionType == "Credit")
            {
                rVM = _IntegrationRepo.SaveCredit_BCL_Trading(vm);
            }
            else
            {
                rVM = _IntegrationRepo.SaveSale_BCL_Trading_Pre(vm);

            }


            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveSale_BCL_Step2(SaleMasterVM vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            ResultVM rVM = new ResultVM();

            try
            {
                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                vm.Token = Session["currentExcelToken"].ToString();
                vm.BranchCode = Session["BranchCode"].ToString();

                rVM = _IntegrationRepo.SaveSale_BCL_Trading_Setp2(vm);

                return Json(rVM, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                //  Session["result"] = "Fail" + "~" + "Something gone wrong";
                SymOrdinary.FileLogger.Log("SaveSale_BCL_Step2", this.GetType().Name, e.Message);
                rVM = new ResultVM();
                return Json(rVM, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        [HttpPost]
        public ActionResult Report_VAT6_3_Preview(IntegrationParam paramVM)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            settingVM.SettingsDTUser = null;
            StaticValueReAssign(identity);

            SysDBInfoVMTemp connVM = Ordinary.StaticValueReAssign(identity, Session);

            paramVM.IsDuplicateInvoiceSave = new CommonRepo(identity, Session).settings("Integration", "DuplicateInvoiceSave");
            paramVM.IsDuplicateInvoiceSave = paramVM.IsDuplicateInvoiceSave == "" ? "N" : paramVM.IsDuplicateInvoiceSave.ToUpper();
            ResultVM rVM = new ResultVM();
            try
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
                if (paramVM.IsDuplicateInvoiceSave == "Y")
                {
                    List<string> values = new List<string>();

                    for (int i = 0; i < paramVM.IDs.Count; i++)
                    {
                        string value = paramVM.IDs[i];
                        values.AddRange(value.Split(',').ToList());
                    }
                    paramVM.IDs = values;

                }
                string IDs = "";
                IDs = string.Join("','", paramVM.IDs);


                SaleReport _reportClass = new SaleReport();
                ////bool PreviewOnly = true;
                ReportDocument reportDocument = new ReportDocument();
                string MultipleSalesInvoiceRows = "";


                if (OrdinaryVATDesktop.IsUnileverCompany(Convert.ToString(Session["CompanyCode"])))
                {
                    for (int i = 0; i < paramVM.IDs.Count; i++)
                    {
                        string InvoiceRow = paramVM.SKUCounts[i];
                        MultipleSalesInvoiceRows = MultipleSalesInvoiceRows + "~" + InvoiceRow;
                    }
                    MultipleSalesInvoiceRows = MultipleSalesInvoiceRows.Substring(1, MultipleSalesInvoiceRows.Length - 1);

                    reportDocument = _reportClass.Report_VAT6_3_Completed("'" + IDs + "'", null, false,
                   false, false, false, false
                   , false, paramVM.PreviewOnly, 1, 0, false, false, false, false, false, false, connVM, MultipleSalesInvoiceRows);
                }
                else
                {
                    reportDocument = _reportClass.Report_VAT6_3_Completed("'" + IDs + "'", null, false,
                                      false, false, false, false
                                      , false, paramVM.PreviewOnly, 1, 0, false, false, false, false, false, false, connVM);
                }


                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                reportDocument.Close();
                reportDocument.Dispose();

                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception ex)
            {
                FileLogger.Log("Sale", "SaveSale", ex.Message + "\n" + ex.StackTrace + "\n");

                Session["result"] = rVM.Status + "~" + ex.Message;
                return Redirect("/VMS/Home");
            }

        }

        [Authorize]
        public ActionResult SalePost(IntegrationParam paramVM)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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


                rVM = _IntegrationRepo.Multiple_SalePost(paramVM);

            }
            catch (Exception)
            {


            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpGet]
        public ActionResult Form_VAT_6_3()
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            settingVM.SettingsDTUser = null;
            StaticValueReAssign(identity);

            #region Access Control

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

            var BranchVM = new BranchProfileRepo(identity, Session).SelectAll(Session["BranchId"].ToString()).FirstOrDefault();
            SysDBInfoVM.SysdataSource = BranchVM.IP;
            DatabaseInfoVM.DatabaseName = BranchVM.DbName;
            SysDBInfoVM.SysPassword = BranchVM.Pass;
            SysDBInfoVM.SysUserName = BranchVM.Id;
            SysDBInfoVM.IsWindowsAuthentication = BranchVM.IsWCF == "Y" ? true : false;

            #region LoginVM

            LoginInfo varLoginVM = new LoginInfo();

            varLoginVM.BranchCode = Session["BranchCode"].ToString();
            varLoginVM.BranchId = Convert.ToInt32(Session["BranchId"]);
            varLoginVM.CurrentUser = Session["LogInUserName"].ToString();
            varLoginVM.CurrentUserId = Session["LogInUserId"].ToString();
            varLoginVM.SysdataSource = SysDBInfoVM.SysdataSource;
            varLoginVM.DatabaseName = DatabaseInfoVM.DatabaseName;
            varLoginVM.SysPassword = SysDBInfoVM.SysPassword;
            varLoginVM.SysUserName = SysDBInfoVM.SysUserName;
            varLoginVM.IsWindowsAuthentication = SysDBInfoVM.IsWindowsAuthentication;

            #endregion

            string loginInfo = "";
            loginInfo = JsonConvert.SerializeObject(varLoginVM);

            ReportCommonVM vm = new ReportCommonVM();
            vm.Json = loginInfo;

            return PartialView("_Form_VAT_6_3", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Report_VAT6_7_Preview(IntegrationParam paramVM)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            settingVM.SettingsDTUser = null;
            StaticValueReAssign(identity);

            SysDBInfoVMTemp connVM = Ordinary.StaticValueReAssign(identity, Session);

            ResultVM rVM = new ResultVM();
            try
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
                }

                string IDs = "";
                IDs = string.Join("','", paramVM.IDs);

                SaleReport _reportClass = new SaleReport();
                ReportDocument reportDocument = _reportClass.Report_VAT6_3_Completed("'" + IDs + "'", "Credit"
                    , true, false, false, false, false, false, paramVM.PreviewOnly, 0, 0, false, false, false, false, false, false, connVM);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception ex)
            {
                Session["result"] = rVM.Status + "~" + ex.Message;
                return Redirect("/VMS/Home");
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult Report_VAT6_8_Preview(IntegrationParam paramVM)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            settingVM.SettingsDTUser = null;
            StaticValueReAssign(identity);

            SysDBInfoVMTemp connVM = Ordinary.StaticValueReAssign(identity, Session);

            ResultVM rVM = new ResultVM();
            try
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
                }

                string IDs = "";
                IDs = string.Join("','", paramVM.IDs);

                SaleReport _reportClass = new SaleReport();
                ReportDocument reportDocument = _reportClass.Report_VAT6_3_Completed("'" + IDs + "'", "Debit"
                    , true, false, false, false, false, false, paramVM.PreviewOnly, 0, 0, false, false, false, false, false, false, connVM);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception ex)
            {
                Session["result"] = rVM.Status + "~" + ex.Message;
                return Redirect("/VMS/Home");
            }

        }


        #endregion

        #region Backup

        ////#region Credit Note Actions


        ////[Authorize]
        ////[HttpGet]
        ////public ActionResult ViewCreditForm_BCL(PopUpViewModel vm)
        ////{
        ////    #region Access Control

        ////    string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
        ////    if (project.ToLower() == "vms")
        ////    {
        ////        if (!identity.IsAdmin)
        ////        {

        ////        }
        ////    }
        ////    else
        ////    {
        ////        Session["rollPermission"] = "deny";
        ////        return Redirect("/Vms/Home");
        ////    }
        ////    #endregion

        ////    return PartialView("_CreditHeader_BCL", vm);
        ////}



        ////[Authorize]
        ////[HttpGet]
        ////public ActionResult GetCreditData_BCL(IntegrationParam vm)
        ////{
        ////    #region Access Control

        ////    string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
        ////    if (project.ToLower() == "vms")
        ////    {
        ////        if (!identity.IsAdmin)
        ////        {

        ////        }
        ////    }
        ////    else
        ////    {
        ////        Session["rollPermission"] = "deny";
        ////        return Redirect("/Vms/Home");
        ////    }
        ////    #endregion

        ////    DataTable dtCredit = new DataTable();
        ////    vm.BranchCode = Session["BranchCode"].ToString();
        ////    ////dtCredit = _IntegrationRepo.GetCredit_DBData_BCL_Trading(vm);
        ////    dtCredit = _IntegrationRepo.GetSource_CreditData_Master_BCL_Trading(vm);

        ////    return PartialView("_CreditBody_BCL", dtCredit);
        ////}


        ////[Authorize]
        ////[HttpGet]
        ////public ActionResult GetCreditData_Detail_BCL(IntegrationParam vm)
        ////{
        ////    #region Access Control

        ////    string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
        ////    if (project.ToLower() == "vms")
        ////    {
        ////        if (!identity.IsAdmin)
        ////        {

        ////        }
        ////    }
        ////    else
        ////    {
        ////        Session["rollPermission"] = "deny";
        ////        return Redirect("/Vms/Home");
        ////    }

        ////    #endregion

        ////    DataTable dtCredit = new DataTable();
        ////    vm.BranchCode = Session["BranchCode"].ToString();

        ////    dtCredit = _IntegrationRepo.GetSource_CreditData_Detail_BCL_Trading(vm);

        ////    return PartialView("_CreditBody_Detail_BCL", dtCredit);
        ////}

        ////[Authorize]
        ////[HttpPost]
        ////public ActionResult SaveCredit_BCL(IntegrationParam vm)
        ////{
        ////    #region Access Control

        ////    string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
        ////    if (project.ToLower() == "vms")
        ////    {
        ////        if (!identity.IsAdmin)
        ////        {
        ////        }
        ////    }
        ////    else
        ////    {
        ////        Session["rollPermission"] = "deny";
        ////        return Redirect("/Vms/Home");
        ////    }

        ////    #endregion

        ////    ResultVM rVM = new ResultVM();


        ////    vm.CurrentUser = identity.UserId;
        ////    vm.BranchCode = Session["BranchCode"].ToString();

        ////    rVM = _IntegrationRepo.SaveCredit_BCL_Trading(vm);


        ////    return Json(rVM, JsonRequestBehavior.AllowGet);
        ////}

        ////#endregion

        #endregion


        #region Transfer Action

        [Authorize]
        [HttpGet]
        public ActionResult ViewTransferForm_BCL(PopUpViewModel vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            return PartialView("_TransferHeader_BCL", vm);
        }

        [Authorize]
        ////[HttpGet]
        public ActionResult GetTransferData_BCL(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtTransfer = new DataTable();
            vm.BranchCode = Session["BranchCode"].ToString();

            dtTransfer = _IntegrationRepo.GetSource_TransferData_Master_BCL_Trading(vm);

            return PartialView("_TransferBody_BCL", dtTransfer);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetTransferData_Detail_BCL(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtTransfer = new DataTable();
            vm.BranchCode = Session["BranchCode"].ToString();

            dtTransfer = _IntegrationRepo.GetSource_TransferData_Detail_BCL_Trading(vm);

            return PartialView("_TransferBody_Detail_BCL", dtTransfer);
        }


        [Authorize]
        public ActionResult SaveTransfer_BCL(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            ////DataTable dtTransfer = new DataTable();

            //////dtTransfer = _IntegrationRepo.GetSource_TransferData_Detail_BCL_Trading(vm);

            //////if (dtTransfer == null || dtTransfer.Rows.Count == 0)
            //////{
            //////    rVM.Message = "This Transaction Already Integrated or Not Exist in Source!";
            //////    return Json(rVM, JsonRequestBehavior.AllowGet);
            //////}

            //////IntegrationParam paramVM = new IntegrationParam();

            vm.BranchCode = Session["BranchCode"].ToString();
            vm.CurrentUser = identity.UserId;
            vm.BranchCode = Session["BranchCode"].ToString();

            rVM = _IntegrationRepo.SaveTransfer_BCL_Trading(vm);

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult TransferPost(IntegrationParam paramVM)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

                rVM = _IntegrationRepo.Multiple_TransferPost(paramVM);

            }
            catch (Exception)
            {


            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpPost]
        public ActionResult Report_VAT6_5_Preview(IntegrationParam paramVM)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            settingVM.SettingsDTUser = null;
            StaticValueReAssign(identity);

            ResultVM rVM = new ResultVM();
            try
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

                string IDs = "";
                IDs = string.Join("','", paramVM.IDs);


                NBRReports _reportClass = new NBRReports();
                ReportDocument reportDocument = _reportClass.TransferIssueNew("'" + IDs + "'", "", "", "", "", "", "", "", paramVM.PreviewOnly);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                reportDocument.Dispose();

                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception ex)
            {
                Session["result"] = rVM.Status + "~" + ex.Message;
                return Redirect("/VMS/Home");
            }

        }


        #endregion

        #endregion


        #region ACI Integration

        #region Purchase Actions

        [Authorize]
        ////[HttpGet]
        public ActionResult GetPurchaseData_ACI(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtPurchase = new DataTable();

            vm.BranchCode = Session["BranchCode"].ToString();
            String CompanyCode = Convert.ToString(Session["CompanyCode"]);
            if (vm.IsTrading && (CompanyCode.ToUpper() == "ACI CB ELECTRICAL" || CompanyCode.ToUpper() == "YAMAHAFACTORY"))
            {
                dtPurchase = _IntegrationRepo.GetSource_PurchaseData_Master_ACI_CB_ELECTRICAL(vm);

            }
            else
            {
                dtPurchase = _IntegrationRepo.GetSource_PurchaseData_Master_ACI(vm);

            }

            return PartialView("_PurchaseBody_ACI ", dtPurchase);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetPurchaseData_Detail_ACI(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtPurchases = new DataTable();
                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
                String CompanyCode = Convert.ToString(Session["CompanyCode"]);

                if (CompanyCode.ToUpper() == "ACI CB ELECTRICAL" && vm.IsTrading)
                {
                    dtPurchases = _IntegrationRepo.GetACI_CB_ElecticalPurchaseDataWeb(vm);

                }
                else
                {
                    dtPurchases = _IntegrationRepo.GetPurchase_DBData_ACI(vm);

                }


                return PartialView("_Purchase_Detail_ACI", dtPurchases);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }


        [Authorize]
        [HttpPost]
        public ActionResult SavePurchase_ACI(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            String CompanyCode = Convert.ToString(Session["CompanyCode"]);

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


            string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
            Session["currentExcelToken"] = Token;
            vm.CurrentUser = identity.UserId;
            vm.Token = Token;

            vm.BranchCode = Session["BranchCode"].ToString();

            //if (vm.TransactionType == "Credit")
            //{
            //    rVM = _IntegrationRepo.SaveCredit_BCL_Trading(vm);
            //}
            //else
            //{

            //}

            vm.IsEntryDate = true;
            vm.InvoiceDateTime = DateTime.Now.ToString("yyyy-MM-dd");

            vm.RefNo = "";
            vm.CompanyCode = CompanyCode;

            rVM = _IntegrationRepo.SavePurchase_ACI(vm);

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Sale Actions
        [Authorize]
        ////[HttpGet]
        public ActionResult GetSaleData_ACI(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtSale = new DataTable();

            vm.BranchCode = Session["BranchCode"].ToString();

            String CompanyCode = Convert.ToString(Session["CompanyCode"]);

            if (CompanyCode == "ACI CB HYGINE" && vm.BranchCode == "AT")
            {
                dtSale = _IntegrationRepo.GetSource_SaleData_Master_ACI_CBHygine(vm);
            }
            else
            {
                dtSale = _IntegrationRepo.GetSource_SaleData_Master_ACI(vm);
            }

            return PartialView("_SaleBody_ACI", dtSale);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetSaleData_Detail_ACI(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtSale = new DataTable();
                vm.BranchCode = Session["BranchCode"].ToString();

                String CompanyCode = Convert.ToString(Session["CompanyCode"]);

                if (CompanyCode == "ACI CB HYGINE" && vm.BranchCode == "AT")
                {
                    dtSale = _IntegrationRepo.GetACISaleData_Web_CBHygine(vm);

                }
                else
                {
                    dtSale = _IntegrationRepo.GetSale_DBData_ACI(vm);
                }

                return PartialView("_SaleBody_Detail_ACI", dtSale);
            }
            catch (Exception e)
            {
                FileLogger.Log("IntegrationController", "GetSaleData_Detail_ACI", e.ToString());

                return RedirectToAction("Index");
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult GetSaleEngineChassisData_ACI(string ID, string ItemCode)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

            try
            {
                DataTable dtSale = new DataTable();
                string BranchCode = Session["BranchCode"].ToString();

                String CompanyCode = Convert.ToString(Session["CompanyCode"]);

                dtSale = _IntegrationRepo.GetACISaleEngineChassisData(ID, ItemCode, BranchCode);

                return PartialView("_SaleBody_EngineChassis_ACI", dtSale);
            }
            catch (Exception e)
            {
                FileLogger.Log("IntegrationController", "GetSaleEngineChassisData_ACI", e.ToString());

                return RedirectToAction("Index");
            }

        }


        [Authorize]
        [HttpPost]
        public ActionResult SaveSale_ACI(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            #region Access Control
            vm.BranchCode = Session["BranchCode"].ToString();

            DataTable dtIssue = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
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

                vm.IsDuplicateInvoiceSave = new CommonRepo(identity, Session).settings("Integration", "DuplicateInvoiceSave");
                vm.IsDuplicateInvoiceSave = vm.IsDuplicateInvoiceSave == "" ? "N" : vm.IsDuplicateInvoiceSave.ToUpper();

                string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
                Session["currentExcelToken"] = Token;
                vm.CurrentUser = identity.Name;
                vm.Token = Token;

                vm.IsEntryDate = true;
                vm.InvoiceDateTime = DateTime.Now.ToString("yyyy-MM-dd");

                if (vm.IsMultiple == true && !string.IsNullOrWhiteSpace(vm.CustomerCode))
                {
                    string ID = vm.IDs[0].ToString();
                    vm.IDs = ID.Split(',').ToList();
                }
                if (vm.IDs == null || vm.IDs.Count > 0)
                {
                    vm.FromDate = "";
                    vm.ToDate = "";
                }


                rVM = _IntegrationRepo.SaveSale_ACI(vm);

                return Json(rVM, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                rVM.Status = "Fail";
                rVM.Message = ex.Message;

                FileLogger.Log("IntegrationController", "SaveSale_ACI", ex.ToString());

                return Json(rVM, JsonRequestBehavior.AllowGet);

            }
        }


        [Authorize]
        public ActionResult SalePost_ACI(IntegrationParam paramVM)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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
            paramVM.IsDuplicateInvoiceSave = new CommonRepo(identity, Session).settings("Integration", "DuplicateInvoiceSave");
            paramVM.IsDuplicateInvoiceSave = paramVM.IsDuplicateInvoiceSave == "" ? "N" : paramVM.IsDuplicateInvoiceSave.ToUpper();

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
                paramVM.DuplicateInvoiceIDs = paramVM.IDs;
                if (paramVM.IsDuplicateInvoiceSave == "Y")
                {
                    List<string> values = new List<string>();

                    for (int i = 0; i < paramVM.IDs.Count; i++)
                    {
                        string value = paramVM.IDs[i];
                        values.AddRange(value.Split(',').ToList());
                    }
                    paramVM.IDs = values;

                }
                rVM = _IntegrationRepo.Multiple_SalePost_ACI(paramVM);

            }
            catch (Exception)
            {


            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region issuePost
        [Authorize]
        public ActionResult IssuePost_ACI(IntegrationParam paramVM)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            #region Access Control
            paramVM.BranchCode = Session["BranchCode"].ToString();

            DataTable dtIssue = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            paramVM.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { paramVM.BranchCode });
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

                rVM = _IntegrationRepo.Multiple_IssuePost_ACI(paramVM);

            }
            catch (Exception)
            {


            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Receive Actions
        [Authorize]
        ////[HttpGet]
        public ActionResult GetReceiveData_ACI(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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
            vm.BranchCode = Session["BranchCode"].ToString();

            DataTable dtReceive = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
            dtReceive = _IntegrationRepo.GetSource_ReceiveData_Master_ACI(vm);

            return PartialView("_ReceiveBody_ACI", dtReceive);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetReceiveData_Detail_ACI(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtReceives = new DataTable();

                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

                dtReceives = _IntegrationRepo.GetReceive_DBData_ACI(vm);

                return PartialView("_Receive_Detail_ACI", dtReceives);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }


        [Authorize]
        [HttpPost]
        public ActionResult SaveReceive_ACI(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
            Session["currentExcelToken"] = Token;
            vm.CurrentUser = identity.UserId;
            vm.Token = Token;

            vm.BranchCode = Session["BranchCode"].ToString();
            vm.DefaultBranchId = Convert.ToInt32(Session["BranchId"]);

            //if (vm.TransactionType == "Credit")
            //{
            //    rVM = _IntegrationRepo.SaveCredit_BCL_Trading(vm);
            //}
            //else
            //{

            //}

            vm.IsEntryDate = true;
            vm.InvoiceDateTime = DateTime.Now.ToString("yyyy-MM-dd");

            vm.RefNo = "";

            rVM = _IntegrationRepo.SaveReceive_ACI(vm);


            return Json(rVM, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Issue Actions
        [Authorize]
        ////[HttpGet]
        public ActionResult GetIssueData_ACI(IntegrationParam vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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
            vm.BranchCode = Session["BranchCode"].ToString();

            DataTable dtIssue = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
            dtIssue = _IntegrationRepo.GetSource_IssueData_Master_ACI(vm);

            return PartialView("_IssueBody_ACI", dtIssue);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetIssueData_Detail_ACI(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtIssue = new DataTable();

                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

                dtIssue = _IntegrationRepo.GetIssue_DBData_ACI(vm);

                return PartialView("_Issue_Detail_ACI", dtIssue);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }


        [Authorize]
        [HttpPost]
        public ActionResult SaveIssue_ACI(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

            try
            {


                string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
                Session["currentExcelToken"] = Token;
                vm.CurrentUser = identity.UserId;
                vm.Token = Token;

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.DefaultBranchId = Convert.ToInt32(Session["BranchId"]);

                //if (vm.TransactionType == "Credit")
                //{
                //    rVM = _IntegrationRepo.SaveCredit_BCL_Trading(vm);
                //}
                //else
                //{

                //}

                vm.IsEntryDate = true;
                vm.InvoiceDateTime = DateTime.Now.ToString("yyyy-MM-dd");

                vm.RefNo = "";

                rVM = _IntegrationRepo.SaveIssue_ACI(vm);


                return Json(rVM, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
                //Fail
                return Json(rVM, JsonRequestBehavior.AllowGet);

            }
        }
        #endregion

        #region Transfer Actions

        [Authorize]
        ////[HttpGet]
        public ActionResult GetTransferData_ACI(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

            DataTable dtTransfer = new DataTable();
            vm.BranchCode = Session["BranchCode"].ToString();

            dtTransfer = _IntegrationRepo.GetSource_TransferData_Master_ACI(vm);

            return PartialView("_TransferBody_ACI", dtTransfer);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetTransferData_Detail_ACI(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            #endregion

            DataTable dtTransfer = new DataTable();
            vm.BranchCode = Session["BranchCode"].ToString();

            dtTransfer = _IntegrationRepo.GetSource_TransferData_Detail_ACI(vm);

            return PartialView("_TransferBody_Detail_ACI", dtTransfer);
        }

        [Authorize]
        public ActionResult SaveTransfer_ACI(IntegrationParam vm)
        {
            ResultVM rVM = new ResultVM();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _IntegrationRepo = new IntegrationRepo(identity, Session);

                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.CurrentUser = identity.UserId;
                vm.BranchCode = Session["BranchCode"].ToString();

                if (vm.IsMultiple == true && !string.IsNullOrWhiteSpace(vm.ToBranchCode))
                {
                    vm.InvoiceDateTime = DateTime.Now.ToString("yyyy-MM-dd");
                    string ID = vm.IDs[0].ToString();
                    vm.IDs = ID.Split(',').ToList();
                }

                if (vm.IDs == null || vm.IDs.Count > 0)
                {
                    vm.FromDate = "";
                    vm.ToDate = "";
                }

                rVM = _IntegrationRepo.SaveTransfer_ACI(vm);
            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;
                FileLogger.Log("ImportDAL", "UpdateSource_TransferData", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult TransferPost_ACI(IntegrationParam paramVM)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                rVM = _IntegrationRepo.Multiple_TransferPost_ACI(paramVM);

            }
            catch (Exception)
            {


            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #endregion

        #region Unilever Integration
        #region Sale Actions
        [Authorize]
        ////[HttpGet]
        public ActionResult GetSaleData_Unilever(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            #region Access Control
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl();
            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (vm.Processed == "ALL")
            {
                vm.Processed = null;
            }
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

            DataTable dtSale = new DataTable();
            String CompanyCode = Convert.ToString(Session["CompanyCode"]);
            if (vm.TransactionType.ToLower() == "other")
            {
                dtSale = _IntegrationRepo.GetSource_SaleData_Unilever(vm);
            }
            else if (vm.TransactionType.ToLower() == "credit")
            {
                dtSale = _IntegrationRepo.GetSource_SaleCNData_Unilever(vm);
            }
            else
            {
                dtSale = _IntegrationRepo.GetSource_SaleDNData_Unilever(vm);
            }
            return PartialView("_SaleBody_Unilever", dtSale);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveSale_Unilever(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            #region Access Control

            DataTable dtIssue = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl();
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
                string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
                Session["currentExcelToken"] = Token;
                vm.CurrentUser = identity.Name;
                vm.Token = Token;
                vm.RefNo = vm.IDs[0].ToString();
                rVM = _IntegrationRepo.SaveSale_Unilever(vm);

                return Json(rVM, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                rVM.Status = "Fail";
                rVM.Message = ex.Message;

                FileLogger.Log("IntegrationController", "SaveSale_Unilever", ex.ToString());

                return Json(rVM, JsonRequestBehavior.AllowGet);

            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetSaleData_Detail_Unilever(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtSale = new DataTable();

                vm.RefNo = vm.IDs[0].ToString();
                if (vm.TransactionType.ToLower() == "other")
                {
                    dtSale = _IntegrationRepo.GetUnileverSaleDataDetails_Web(vm);
                }
                else
                {
                    dtSale = _IntegrationRepo.GetUnileverSaleDataReturnDetails_Web(vm);
                }
                ViewBag.TransactionType = vm.TransactionType;
                return PartialView("_SaleBody_Detail_Unilever", dtSale);
            }
            catch (Exception e)
            {
                FileLogger.Log("IntegrationController", "GetSaleData_Detail_Unilever", e.ToString());

                return RedirectToAction("Index");
            }

        }
        #endregion

        #region Purchase Action
        [Authorize]
        ////[HttpGet]
        public ActionResult GetPurchaseData_Unilever(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            #region Access Control
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl();
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

            DataTable dtPurchase = new DataTable();

            String CompanyCode = Convert.ToString(Session["CompanyCode"]);
            if (vm.TransactionType.ToLower() == "other")
            {
                dtPurchase = _IntegrationRepo.GetSource_DistinctPurchaseData_Master(vm);
            }
            else
            {
                dtPurchase = _IntegrationRepo.GetSource_DistinctPurchaseReturnData_Master(vm);

            }

            return PartialView("_PurchaseBody_Unilever", dtPurchase);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetPurchaseData_Detail_Unilever(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtPurchases = new DataTable();
                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl();
                String CompanyCode = Convert.ToString(Session["CompanyCode"]);
                if (vm.TransactionType.ToLower() == "other")
                {
                    dtPurchases = _IntegrationRepo.GetSource_PurchaseDetailsData_Master(vm);

                }
                else
                {
                    dtPurchases = _IntegrationRepo.GetSource_PurchaseReturnData_Master(vm);
                }


                return PartialView("_Purchase_Detail_Unilever", dtPurchases);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }


        [Authorize]
        [HttpPost]
        public ActionResult SavePurchase_Unilever(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            String CompanyCode = Convert.ToString(Session["CompanyCode"]);

            #region Access Control
            DataTable dtIssue = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl();
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


            string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
            Session["currentExcelToken"] = Token;
            vm.CurrentUser = identity.UserId;
            vm.Token = Token;

            vm.CompanyCode = CompanyCode;

            rVM = _IntegrationRepo.SavePurchase_Unilever(vm);

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        private void StaticValueReAssign(ShampanIdentity identity)
        {
            try
            {
                #region Get Company Information
                CompanyProfileRepo _CompanyProfileRepo = new CompanyProfileRepo(identity, Session);
                CompanyProfileVM varCompanyProfileVM = new CompanyProfileVM();
                string CompanyId = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, identity.CompanyId);
                varCompanyProfileVM = _CompanyProfileRepo.SelectAll(CompanyId).FirstOrDefault();

                OrdinaryVATDesktop.CompanyName = varCompanyProfileVM.CompanyLegalName;
                OrdinaryVATDesktop.Address1 = varCompanyProfileVM.Address1;
                //OrdinaryVATDesktop.Address2 = varCompanyProfileVM.Address2;
                //OrdinaryVATDesktop.Address3 = varCompanyProfileVM.Address3;
                OrdinaryVATDesktop.TelephoneNo = varCompanyProfileVM.TelephoneNo;
                OrdinaryVATDesktop.FaxNo = varCompanyProfileVM.FaxNo;
                OrdinaryVATDesktop.VatRegistrationNo = varCompanyProfileVM.VatRegistrationNo;
                OrdinaryVATDesktop.Section = varCompanyProfileVM.Section;
                #endregion

                #region Get Branch Information
                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                BranchProfileVM varBranchProfileVM = new BranchProfileVM();
                varBranchProfileVM = branchProfileRepo.SelectAll(Convert.ToString(Session["BranchId"])).FirstOrDefault();
                OrdinaryVATDesktop.IsWCF = varBranchProfileVM.IsWCF;
                #endregion

                OrdinaryVATDesktop.CurrentUser = Convert.ToString(Session["LogInUserName"]);

                #region Get IsMainSetting
                UserInformationVM varUserInformationVM = new UserInformationVM();

                UserInformationRepo _UserInformationRepo = new UserInformationRepo(identity, Session);

                varUserInformationVM = _UserInformationRepo.SelectAll(0, new[] { "UserName" }, new[] { OrdinaryVATDesktop.CurrentUser }).FirstOrDefault();
                UserInfoVM.IsMainSetting = varUserInformationVM.IsMainSettings == "Y" ? true : false;
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        #region KCCL Intrigation

        [Authorize]
        ////[HttpGet]
        public ActionResult GetSaleData_KCCL(IntegrationParam vm)
        {
            #region Access Control

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

            //CommonRepo _cDal = new CommonRepo();
            //var importDal = new ImportRepo();
            //var saleDal = new SaleInvoiceRepo();

            ////string value = new CommonRepo().settings("Import", "SaleExistContinue");

            ////if (value == "N")
            ////{
            ////    List<SaleMasterVM> sales = saleDal.SelectAllTop1(0, new[] { "sih.ImportIDExcel" }, new[] { invoiceNo }, null, null, null, null);

            ////    if (sales != null && sales.Count > 0)
            ////    {
            ////        var sale = sales.FirstOrDefault();

            ////        if (sale != null)
            ////            MessageBox.Show(@"This Transaction No is already in system with invoice no - " +
            ////                            sale.SalesInvoiceNo);

            ////        return;
            ////    }
            ////}

            DataTable dtSale = new DataTable();

            string BranchId = Session["BranchId"].ToString();

            BranchProfileRepo _bRepo = new BranchProfileRepo(identity, Session);
            DataTable dt = _bRepo.SelectAl(BranchId, null, null, true);

            dtSale = _IntegrationRepo.GetSaleKohinoorDbData(vm.RefNo, dt);

            return PartialView("_SaleBody_KCCL", dtSale);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveSale_KCCL(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            #region Access Control
            vm.BranchCode = Session["BranchCode"].ToString();

            DataTable dtIssue = new DataTable();
            BranchProfileRepo _bRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _bRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
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

            string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
            Session["currentExcelToken"] = Token;
            vm.CurrentUserName = identity.Name;
            vm.Token = Token;
            vm.BranchId = Session["BranchId"].ToString();
            vm.CurrentUser = identity.UserId;
            ////if (vm.IsMultiple == true && !string.IsNullOrWhiteSpace(vm.CustomerCode))
            ////{
            ////    string ID = vm.IDs[0].ToString();
            ////    vm.IDs = ID.Split(',').ToList();
            ////}

            rVM = _IntegrationRepo.SaveSale_KCCL(vm);

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        ////[HttpGet]
        public ActionResult GetTransferData_KCCL(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

            DataTable dtTransfer = new DataTable();
            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();

            dtTransfer = _IntegrationRepo.GetSource_TransferData_Master_KCCL(vm);

            return PartialView("_TransferBody_KCCL", dtTransfer);
        }

        [Authorize]
        public ActionResult SaveTransfer_KCCL(IntegrationParam vm)
        {
            ResultVM rVM = new ResultVM();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                _IntegrationRepo = new IntegrationRepo(identity, Session);

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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.CurrentUser = identity.UserId;
                vm.CurrentUserName = identity.Name;
                vm.BranchId = Session["BranchId"].ToString();

                rVM = _IntegrationRepo.SaveTransfer_KCCL(vm);
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;
                FileLogger.Log("IntegrationController", "SaveTransfer_KCCL", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region SMC Integration

        #region Purchase Actions

        [Authorize]
        ////[HttpGet]
        public ActionResult GetPurchaseData_SMC(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtPurchase = new DataTable();

            vm.BranchCode = Session["BranchCode"].ToString();

            BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
            vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

            //////if (vm.TransactionType != "Other")
            //////{
            //////    vm.DataSourceType = "Normal";
            //////}

            dtPurchase = _IntegrationRepo.GetSource_PurchaseData_Master_SMC(vm);

            return PartialView("_PurchaseBody_SMC", dtPurchase);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetPurchaseData_Detail_SMC(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtPurchases = new DataTable();
                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

                //////if (vm.TransactionType != "Other")
                //////{
                //////    vm.DataSourceType = "Normal";
                //////}

                dtPurchases = _IntegrationRepo.GetSMCPurchaseDetailDataWeb(vm);

                return PartialView("_Purchase_Detail_SMC", dtPurchases);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult SavePurchase_SMC(IntegrationParam vm)
        {
            ResultVM rVM = new ResultVM();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _IntegrationRepo = new IntegrationRepo(identity, Session);

                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
                Session["currentExcelToken"] = Token;
                vm.CurrentUser = identity.UserId;
                vm.Token = Token;

                vm.BranchCode = Session["BranchCode"].ToString();

                if (vm.TransactionType != "Other")
                {
                    vm.DataSourceType = "Normal";
                }

                ////vm.IsEntryDate = true;
                //vm.InvoiceDateTime = DateTime.Now.ToString("yyyy-MM-dd");

                vm.RefNo = "";

                rVM = _IntegrationRepo.SavePurchase_SMC(vm);

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
            }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Issue Actions

        [Authorize]
        ////[HttpGet]
        public ActionResult GetIssueData_SMC(IntegrationParam vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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
            vm.BranchCode = Session["BranchCode"].ToString();

            DataTable dtIssue = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

            dtIssue = _IntegrationRepo.GetSource_IssueData_Master_SMC(vm);

            return PartialView("_IssueBody_SMC", dtIssue);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetIssueData_Detail_SMC(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtIssue = new DataTable();

                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

                dtIssue = _IntegrationRepo.GetIssue_DBData_SMC(vm);

                return PartialView("_Issue_Detail_SMC", dtIssue);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }


        [Authorize]
        [HttpPost]
        public ActionResult SaveIssue_SMC(IntegrationParam vm)
        {
            ResultVM rVM = new ResultVM();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _IntegrationRepo = new IntegrationRepo(identity, Session);

                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
                Session["currentExcelToken"] = Token;
                vm.CurrentUser = identity.UserId;
                vm.Token = Token;

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.DefaultBranchId = Convert.ToInt32(Session["BranchId"]);

                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

                vm.RefNo = "";

                rVM = _IntegrationRepo.SaveIssue_SMC(vm);
            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
            }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Receive Actions

        [Authorize]
        public ActionResult GetReceiveData_SMC(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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
            vm.BranchCode = Session["BranchCode"].ToString();

            DataTable dtReceive = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
            dtReceive = _IntegrationRepo.GetSource_ReceiveData_Master_SMC(vm);

            return PartialView("_ReceiveBody_SMC", dtReceive);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetReceiveData_Detail_SMC(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtReceives = new DataTable();

                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

                dtReceives = _IntegrationRepo.GetReceive_DBData_SMC(vm);

                return PartialView("_Receive_Detail_SMC", dtReceives);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }


        [Authorize]
        [HttpPost]
        public ActionResult SaveReceive_SMC(IntegrationParam vm)
        {
            ResultVM rVM = new ResultVM();

            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _IntegrationRepo = new IntegrationRepo(identity, Session);

                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
                Session["currentExcelToken"] = Token;
                vm.CurrentUser = identity.UserId;
                vm.Token = Token;

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.DefaultBranchId = Convert.ToInt32(Session["BranchId"]);

                vm.RefNo = "";

                rVM = _IntegrationRepo.SaveReceive_SMC(vm);
            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
            }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Transfer

        [Authorize]
        [HttpPost]
        public ActionResult SaveSale_SMC(IntegrationParam vm)
        {
            ResultVM rVM = new ResultVM();

            try
            {


                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _IntegrationRepo = new IntegrationRepo(identity, Session);

                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

                #region Access Control
                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
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

                rVM = _IntegrationRepo.SaveSale_SMC(vm);

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
        public ActionResult SaveTransfer_SMC(IntegrationParam vm)
        {
            ResultVM rVM = new ResultVM();

            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _IntegrationRepo = new IntegrationRepo(identity, Session);

                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

                #region Access Control
                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
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

                rVM = _IntegrationRepo.SaveTransfer_SMC(vm);
            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
            }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Toll Receive

        [Authorize]
        ////[HttpGet]
        public ActionResult GetTollReceiveData_SMC(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

            DataTable dtPurchase = new DataTable();

            vm.BranchCode = Session["BranchCode"].ToString();

            BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
            vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

            dtPurchase = _IntegrationRepo.GetTollReceiveDataMaster_SMC(vm);

            return PartialView("_TollReceiveBody_SMC", dtPurchase);

        }

        [Authorize]
        [HttpGet]
        public ActionResult GetTollReceiveData_Detail_SMC(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

            try
            {
                DataTable dtPurchases = new DataTable();
                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

                dtPurchases = _IntegrationRepo.GetTollReceiveDataDetails_SMC(vm);

                return PartialView("_TollReceive_Detail_SMC", dtPurchases);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveTollReceive_SMC(IntegrationParam vm)
        {
            ResultVM rVM = new ResultVM();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _IntegrationRepo = new IntegrationRepo(identity, Session);

                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
                Session["currentExcelToken"] = Token;
                vm.CurrentUser = identity.UserId;
                vm.Token = Token;

                vm.BranchCode = Session["BranchCode"].ToString();

                vm.RefNo = "";

                rVM = _IntegrationRepo.SaveSMCTollReceive_Web(vm);

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
            }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region BombaySweets Integration

        [UserFilter]
        public ActionResult GetSaleMwWareData_BS(BombaySaleDetailsVM vm)
        {
            #region Access Control

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

            return View(vm);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult _GetSaleMwWareData_BS(JQueryDataTableParamVM param, BombaySaleDetailsVM paramVM)
        {
            //_repo = new SaleInvoiceRepo(identity, Session);

            List<BombaySaleDetailsVM> getAllData = new List<BombaySaleDetailsVM>();

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

            //if (paramVM.BranchId == 0)
            //{
            //    paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            //}

            //if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            //{
            //    paramVM.SelectTop = "100";
            //}

            #endregion SeachParameters

            #region Data Call

            string[] conditionFields;
            string[] conditionValues;
            if (string.IsNullOrWhiteSpace(paramVM.SearchField))
            {
                conditionFields = new string[] { "Invoice_Date>=", "Invoice_Date<=", "IsProcessed" };
                conditionValues = new string[] { dtFrom, dtTo, paramVM.IsProcessed };



            }
            else
            {

                paramVM.SearchField = paramVM.SearchField + " like";


                conditionFields = new string[] { "Invoice_Date>=", "Invoice_Date<=", "IsProcessed", paramVM.SearchField, };
                conditionValues = new string[] { dtFrom, dtTo, paramVM.IsProcessed, paramVM.SearchValue, };

            }

            getAllData = _IntegrationRepo.SelectAllBSMwareData(0, conditionFields, conditionValues, null, null, null);

            // getAllData = _IntegrationRepo.SelectAllBSMwareData(0, conditionFields, conditionValues,null,"Y",null);


            #endregion

            #region Search and Filter Data
            IEnumerable<BombaySaleDetailsVM> filteredData;
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
                   .Where(c => isSearchable1 && c.ID.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.Branch_Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.CustomerCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Reference_No.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Invoice_Date.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Invoice_Time.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.IsProcessed.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable8 && c.TransactionType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable9 && c.VehicleType.ToString().ToLower().Contains(param.sSearch.ToLower())

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
            Func<BombaySaleDetailsVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.ID :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Branch_Code.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.CustomerCode.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Reference_No.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Invoice_Date.ToString() :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Invoice_Time.ToString() :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.IsProcessed.ToString() :
                                                           sortColumnIndex == 8 && isSortable_8 ? c.TransactionType.ToString() :
                                                           sortColumnIndex == 9 && isSortable_9 ? c.VehicleType.ToString() :

                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                   c.ID+"~" //+ c.Branch_Code+"~"+ c.CustomerCode
                  , c.ID
                  ,c.Branch_Code
                  ,c.CustomerCode
                  , c.Reference_No
                  , c.Invoice_Date.ToString()
                  , c.Invoice_Time.ToString()
                  , c.IsProcessed.ToString()             
                  , c.TransactionType.ToString()               
                 // , c.VehicleType.ToString()               
                //, c.Post=="Y" ? "Posted" : "Not Posted"
                //, c.ImportIDExcel
                //, c.TransactionType
                //, c.IsInstitution=="Y" ? "Institution" : "Not Institution"

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

        public ActionResult GetSaleMwWareData_BS_Details(BombaySaleDetailsVM vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            IntegrationRepo _IntegrationRepo = new IntegrationRepo(identity);
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
            //CustomerVM vm = new CustomerVM();
            //vm.CustomerName = CustomerName;
            DataTable dtMWDataDetails = new DataTable();
            //vm.BranchCode = Session["BranchCode"].ToString();
            //vm.Processed = "N";

            dtMWDataDetails = _IntegrationRepo.SelectAllBSMWDeatailData(vm);

            //  var TransactionType = new DataColumn("TransactionType") { DefaultValue = vm.TransactionType };

            // dtSalesMaster.Columns.Add(TransactionType);

            return PartialView("_MiddleWareData_BS", dtMWDataDetails);
        }

        #endregion

        #region EON Integration
        [Authorize]
        ////[HttpGet]
        public ActionResult GetSaleData_EON(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            BranchProfileRepo _repo = new BranchProfileRepo();

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtSale = new DataTable();


            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();

            DataTable dt = _repo.SelectAl(vm.BranchId);
            vm.dtConnectionInfo = dt;
            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");

            //var dal = new BranchProfileDAL();

            //DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);


            dtSale = _IntegrationRepo.GetSaleData_EON(vm);

            return PartialView("_SaleBody_EON", dtSale);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveSale_EON(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();
            try
            {
                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();


                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });


                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                vm.CurrentUserId = Session["LogInUserId"].ToString();

                rVM = _IntegrationRepo.SaveSale_EON(vm);

            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;
                //FileLogger.Log("IntegrationController", "SaveSale_EON", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        ////[HttpGet]
        public ActionResult GetPurchaseData_EON(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            BranchProfileRepo _repo = new BranchProfileRepo();

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtPurchase = new DataTable();

            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();

            DataTable dt = _repo.SelectAl(vm.BranchId);
            vm.dtConnectionInfo = dt;
            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");


            dtPurchase = _IntegrationRepo.GetPurchaseData_EON(vm);

            return PartialView("_PurchaseBody_EON", dtPurchase);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SavePurchase_EON(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            try
            {
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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();

                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                vm.CurrentUserId = Session["LogInUserId"].ToString();

                rVM = _IntegrationRepo.SavePurchase_EON(vm);
            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;
                //FileLogger.Log("IntegrationController", "SaveSale_EON", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);


        }

        [Authorize]
        ////[HttpGet]
        public ActionResult GetIssueData_EON(IntegrationParam vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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
            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();

            DataTable dtIssue = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");

            dtIssue = _IntegrationRepo.GetPIssueData_EON(vm);

            return PartialView("_IssueBody_EON", dtIssue);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveIssue_EON(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            ResultVM rVM = new ResultVM();

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            try
            {

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



                //string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
                //Session["currentExcelToken"] = Token;
                //vm.CurrentUser = identity.UserId;
                //vm.Token = Token;

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();
                vm.DefaultBranchId = Convert.ToInt32(Session["BranchId"]);

                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                vm.CurrentUserId = Session["LogInUserId"].ToString();

                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });


                //vm.RefNo = "";

                rVM = _IntegrationRepo.SavePIssue_EON(vm);

            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;

            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);


        }

        [Authorize]
        public ActionResult GetReceiveData_EON(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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
            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();
            DataTable dtReceive = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");


            dtReceive = _IntegrationRepo.GetPReceiveData_EON(vm);

            return PartialView("_ReceiveBody_EON", dtReceive);
        }


        [Authorize]
        [HttpPost]
        public ActionResult SaveReceive_EON(IntegrationParam vm)
        {


            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

            try
            {
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


                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();
                vm.DefaultBranchId = Convert.ToInt32(Session["BranchId"]);

                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                vm.CurrentUserId = Session["LogInUserId"].ToString();

                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

                rVM = _IntegrationRepo.SavePReceive_EON(vm);
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;

            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        ////[HttpGet]
        public ActionResult GetTransferData_EON(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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


            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();


            DataTable dtTransfer = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");


            dtTransfer = _IntegrationRepo.GetTransferData__EON(vm);

            return PartialView("_TransferBody_EON", dtTransfer);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveTransferData_EON(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();
            try
            {
                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();

                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });


                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                vm.CurrentUserId = Session["LogInUserId"].ToString();

                rVM = _IntegrationRepo.SaveTransferData_EON(vm);

            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;
                //FileLogger.Log("IntegrationController", "SaveSale_EON", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region Berger Integration

        [Authorize]
        [HttpPost]
        public ActionResult SaveSale_Berger(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();
            try
            {
                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                vm.BranchCode = Session["BranchCode"].ToString();

                //BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                //vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });


                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                vm.CurrentUserId = Session["LogInUserId"].ToString();

                string[] result = _IntegrationRepo.SaveSale_BERGER(vm);

                rVM.Status = result[0];
                rVM.Message = result[1];

            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;
                //FileLogger.Log("IntegrationController", "SaveSale_EON", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult GetReceiveData_Berger(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();
            DataTable dtReceive = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

            if (string.IsNullOrWhiteSpace(vm.FromDate))
            {
                vm.FromDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrWhiteSpace(vm.ToDate))
            {
                vm.ToDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
            }

            dtReceive = _IntegrationRepo.GetPReceiveData_Berger(vm);

            return PartialView("_ReceiveBody_Berger", dtReceive);
        }


        [Authorize]
        [HttpPost]
        public ActionResult SaveReceive_Berger(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

            try
            {
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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();
                vm.DefaultBranchId = Convert.ToInt32(Session["BranchId"]);

                ////////vm.FromDate = DateTime.Now.ToString("yyyy-MM-01");
                ////////vm.ToDate = (Convert.ToDateTime(vm.FromDate).AddMonths(1)).AddDays(-1).ToString("yyyy-MM-dd");

                ////////vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                ////////vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");

                if (string.IsNullOrWhiteSpace(vm.FromDate))
                {
                    vm.FromDate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                }
                if (string.IsNullOrWhiteSpace(vm.ToDate))
                {
                    vm.ToDate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                }

                vm.CurrentUserId = Session["LogInUserId"].ToString();

                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

                rVM = _IntegrationRepo.SavePReceive_Berger(vm);

                rVM.Message = rVM.Message.Split('\r').FirstOrDefault();

            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;

            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        ////[HttpGet]
        public ActionResult GetTransferData_Berger(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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


            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();


            DataTable dtTransfer = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");


            dtTransfer = _IntegrationRepo.GetTransferData_Berger(vm);

            return PartialView("_TransferBody_Berger", dtTransfer);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveTransferData_Berger(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();
            try
            {
                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();

                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });


                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                vm.CurrentUserId = Session["LogInUserId"].ToString();

                rVM = _IntegrationRepo.SaveTransferData_Berger(vm);

            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;
                //FileLogger.Log("IntegrationController", "SaveSale_EON", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }


        #endregion.

        #region Decathlon Integration

        [Authorize]
        ////[HttpGet]
        public ActionResult GetSaleData_Decathlon(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            BranchProfileRepo _repo = new BranchProfileRepo();

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtSale = new DataTable();


            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();

            DataTable dt = _repo.SelectAl(vm.BranchId);
            vm.dtConnectionInfo = dt;
            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");

            //var dal = new BranchProfileDAL();

            //DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);


            dtSale = _IntegrationRepo.GetSaleData_Decathlon(vm);

            return PartialView("_SaleBody_Decathlon", dtSale);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveSale_Decathlon(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();
            try
            {
                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();


                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });


                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                vm.CurrentUserId = Session["LogInUserId"].ToString();

                rVM = _IntegrationRepo.SaveSale_Decathlon(vm);

            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;
                //FileLogger.Log("IntegrationController", "SaveSale_EON", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        ////[HttpGet]
        public ActionResult GetPurchaseData_Decathlon(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            BranchProfileRepo _repo = new BranchProfileRepo();

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtPurchase = new DataTable();

            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();

            DataTable dt = _repo.SelectAl(vm.BranchId);
            vm.dtConnectionInfo = dt;
            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");


            dtPurchase = _IntegrationRepo.GetPurchaseData_Decathlon(vm);

            return PartialView("_PurchaseBody_Decathlon", dtPurchase);
        }


        [Authorize]
        [HttpPost]
        public ActionResult SavePurchase_Decathlon(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            try
            {
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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();

                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                vm.CurrentUserId = Session["LogInUserId"].ToString();

                rVM = _IntegrationRepo.SavePurchase_Decathlon(vm);
            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;
                //FileLogger.Log("IntegrationController", "SaveSale_EON", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);


        }

        [Authorize]
        ////[HttpGet]
        public ActionResult GetTransferData_Decathlon(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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


            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();


            DataTable dtTransfer = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");


            dtTransfer = _IntegrationRepo.GetTransferData__Decathlon(vm);

            return PartialView("_TransferBody_Decathlon", dtTransfer);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveTransferData_Decathlon(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();
            try
            {
                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();

                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });


                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                vm.CurrentUserId = Session["LogInUserId"].ToString();

                rVM = _IntegrationRepo.SaveTransferData_Decathlon(vm);

            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;
                //FileLogger.Log("IntegrationController", "SaveSale_EON", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }






        #endregion

        #region ShumisHotCake Integration

        [Authorize]
        ////[HttpGet]
        public ActionResult GetSaleData_ShumiHotCake(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            BranchProfileRepo _repo = new BranchProfileRepo();

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtSale = new DataTable();


            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();

            DataTable dt = _repo.SelectAl(vm.BranchId);
            vm.dtConnectionInfo = dt;
            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");

            //var dal = new BranchProfileDAL();

            //DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);


            dtSale = _IntegrationRepo.GetSaleData_ShumiHotCake(vm);

            return PartialView("_SaleBody_ShumiHotCake", dtSale);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveSale_ShumiHotCake(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();
            try
            {
                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();


                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });


                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                vm.CurrentUserId = Session["LogInUserId"].ToString();

                rVM = _IntegrationRepo.SaveSale_ShumiHotCake(vm);

            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;
                //FileLogger.Log("IntegrationController", "SaveSale_EON", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ShumisHotCakeCTG Integration

        [Authorize]
        ////[HttpGet]
        public ActionResult GetSaleData_ShumiHotCakeCtg(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            BranchProfileRepo _repo = new BranchProfileRepo();

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtSale = new DataTable();


            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();

            DataTable dt = _repo.SelectAl(vm.BranchId);
            vm.dtConnectionInfo = dt;
            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");

            //var dal = new BranchProfileDAL();

            //DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);


            dtSale = _IntegrationRepo.GetSaleData_ShumiHotCakeCtg(vm);

            return PartialView("_SaleBody_ShumiHotCakeCtg", dtSale);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveSale_ShumiHotCakeCtg(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();
            try
            {
                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();


                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });


                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                vm.CurrentUserId = Session["LogInUserId"].ToString();

                rVM = _IntegrationRepo.SaveSale_ShumiHotCakeCtg(vm);

            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;
                //FileLogger.Log("IntegrationController", "SaveSale_EON", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region JAPFA Integration

        #region Purchase
        [Authorize]
        ////[HttpGet]
        public ActionResult GetPurchaseData_JAPFA(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtPurchase = new DataTable();

            //vm.BranchCode = Session["BranchCode"].ToString();
            //String CompanyCode = Convert.ToString(Session["CompanyCode"]);

            vm.BranchCode = Session["BranchCode"].ToString();

            BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
            vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
            String CompanyCode = Convert.ToString(Session["CompanyCode"]);


            dtPurchase = _IntegrationRepo.GetSource_PurchaseData_Master_JAPFA(vm);



            return PartialView("_PurchaseBody_JAPFA", dtPurchase);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetPurchaseData_Detail_JAPFA(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtPurchases = new DataTable();
                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
                String CompanyCode = Convert.ToString(Session["CompanyCode"]);

                dtPurchases = _IntegrationRepo.GetPurchase_DBData_JAPFA(vm);

                return PartialView("_Purchase_Detail_JAPFA", dtPurchases);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult SavePurchase_JAPFA(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            String CompanyCode = Convert.ToString(Session["CompanyCode"]);

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


            string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
            Session["currentExcelToken"] = Token;
            vm.CurrentUser = identity.UserId;
            vm.Token = Token;

            vm.BranchCode = Session["BranchCode"].ToString();



            //vm.IsEntryDate = true;
            vm.InvoiceDateTime = DateTime.Now.ToString("yyyy-MM-dd");

            vm.RefNo = "";
            vm.CompanyCode = CompanyCode;
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);

            rVM = _IntegrationRepo.SavePurchase_JAPFA(vm);

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Sale

        [Authorize]
        ////[HttpGet]
        public ActionResult GetSaleData_JAPFA(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtSale = new DataTable();

            vm.BranchCode = Session["BranchCode"].ToString();

            BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
            vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
            String CompanyCode = Convert.ToString(Session["CompanyCode"]);


            dtSale = _IntegrationRepo.GetSource_SaleData_Master_JAPFA(vm);


            return PartialView("_SaleBody_JAPFA", dtSale);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetSaleData_Detail_JAPFA(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtSale = new DataTable();
                vm.BranchCode = Session["BranchCode"].ToString();
                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
                String CompanyCode = Convert.ToString(Session["CompanyCode"]);



                dtSale = _IntegrationRepo.GetSale_DBData_JAPFA(vm);


                return PartialView("_SaleBody_Detail_JAPFA", dtSale);
            }
            catch (Exception e)
            {
                FileLogger.Log("IntegrationController", "GetSaleData_Detail_JAPFA", e.ToString());

                return RedirectToAction("Index");
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveSale_JAPFA(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            #region Access Control
            vm.BranchCode = Session["BranchCode"].ToString();

            DataTable dtIssue = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
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

                //vm.IsDuplicateInvoiceSave = new CommonRepo(identity, Session).settings("Integration", "DuplicateInvoiceSave");
                //vm.IsDuplicateInvoiceSave = vm.IsDuplicateInvoiceSave == "" ? "N" : vm.IsDuplicateInvoiceSave.ToUpper();

                //string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
                //Session["currentExcelToken"] = Token;
                //vm.CurrentUser = identity.Name;
                //vm.Token = Token;

                OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);
                vm.InvoiceDateTime = DateTime.Now.ToString("yyyy-MM-dd");


                if (vm.IDs == null || vm.IDs.Count > 0)
                {
                    vm.FromDate = "";
                    vm.ToDate = "";
                }


                rVM = _IntegrationRepo.SaveSale_JAPFA(vm);

                return Json(rVM, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                rVM.Status = "Fail";
                rVM.Message = ex.Message;

                FileLogger.Log("IntegrationController", "SaveSale_ACI", ex.ToString());

                return Json(rVM, JsonRequestBehavior.AllowGet);

            }
        }



        #endregion

        #region Issue

        [Authorize]
        ////[HttpGet]
        public ActionResult GetIssueData_JAPFA(IntegrationParam vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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
            vm.BranchCode = Session["BranchCode"].ToString();

            DataTable dtIssue = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
            dtIssue = _IntegrationRepo.GetSource_IssueData_Master_JAPFA(vm);

            return PartialView("_IssueBody_JAPFA", dtIssue);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetIssueData_Detail_JAPFA(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtIssue = new DataTable();

                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

                dtIssue = _IntegrationRepo.GetIssue_DBData_JAPFA(vm);

                return PartialView("_Issue_Detail_JAPFA", dtIssue);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveIssue_JAPFA(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

            try
            {


                string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
                Session["currentExcelToken"] = Token;
                vm.CurrentUser = identity.UserId;
                vm.Token = Token;

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.DefaultBranchId = Convert.ToInt32(Session["BranchId"]);
                OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);
                //if (vm.TransactionType == "Credit")
                //{
                //    rVM = _IntegrationRepo.SaveCredit_BCL_Trading(vm);
                //}
                //else
                //{

                //}

                vm.IsEntryDate = true;
                vm.InvoiceDateTime = DateTime.Now.ToString("yyyy-MM-dd");

                vm.RefNo = "";

                rVM = _IntegrationRepo.SaveIssue_JAPFA(vm);


                return Json(rVM, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
                //Fail
                return Json(rVM, JsonRequestBehavior.AllowGet);

            }
        }


        #endregion

        #region Receive

        [Authorize]
        ////[HttpGet]
        public ActionResult GetReceiveData_JAPFA(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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
            vm.BranchCode = Session["BranchCode"].ToString();

            DataTable dtReceive = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
            dtReceive = _IntegrationRepo.GetSource_ReceiveData_Master_JAPFA(vm);

            return PartialView("_ReceiveBody_JAPFA", dtReceive);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetReceiveData_Detail_JAPFA(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtReceives = new DataTable();

                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

                dtReceives = _IntegrationRepo.GetReceive_DBData_JAPFA(vm);

                return PartialView("_Receive_Detail_JAPFA", dtReceives);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveReceive_JAPFA(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
            Session["currentExcelToken"] = Token;
            vm.CurrentUser = identity.UserId;
            vm.Token = Token;

            vm.BranchCode = Session["BranchCode"].ToString();
            vm.DefaultBranchId = Convert.ToInt32(Session["BranchId"]);
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);

            //if (vm.TransactionType == "Credit")
            //{
            //    rVM = _IntegrationRepo.SaveCredit_BCL_Trading(vm);
            //}
            //else
            //{

            //}

            vm.IsEntryDate = true;
            vm.InvoiceDateTime = DateTime.Now.ToString("yyyy-MM-dd");

            vm.RefNo = "";

            rVM = _IntegrationRepo.SaveReceive_JAPFA(vm);


            return Json(rVM, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Transfer

        [Authorize]
        ////[HttpGet]
        public ActionResult GetTransferData_JAPFA(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

            DataTable dtTransfer = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);
            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

            vm.BranchCode = Session["BranchCode"].ToString();

            dtTransfer = _IntegrationRepo.GetSource_TransferData_Master_JAFPA(vm);

            return PartialView("_TransferBody_JAPFA", dtTransfer);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetTransferData_Detail_JAPFA(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            #endregion

            DataTable dtTransfer = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);
            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
            vm.BranchCode = Session["BranchCode"].ToString();

            dtTransfer = _IntegrationRepo.GetSource_TransferData_Detail_JAPFA(vm);

            return PartialView("_TransferBody_Detail_JAPFA", dtTransfer);
        }

        [Authorize]
        public ActionResult SaveTransfer_JAPFA(IntegrationParam vm)
        {
            ResultVM rVM = new ResultVM();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _IntegrationRepo = new IntegrationRepo(identity, Session);

                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                vm.BranchCode = Session["BranchCode"].ToString();
                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
                vm.CurrentUser = identity.UserId;
                vm.BranchCode = Session["BranchCode"].ToString();
                OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);
                //if (vm.IsMultiple == true && !string.IsNullOrWhiteSpace(vm.ToBranchCode))
                //{
                //    vm.InvoiceDateTime = DateTime.Now.ToString("yyyy-MM-dd");
                //    string ID = vm.IDs[0].ToString();
                //    vm.IDs = ID.Split(',').ToList();
                //}

                if (vm.IDs == null || vm.IDs.Count > 0)
                {
                    vm.FromDate = "";
                    vm.ToDate = "";
                }

                rVM = _IntegrationRepo.SaveTransfer_JAPFA(vm);
            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;
                FileLogger.Log("ImportDAL", "UpdateSource_TransferData", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region Nourish Integration

        [Authorize]
        ////[HttpGet]
        public ActionResult GetPurchaseData_Nourish(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            BranchProfileRepo _repo = new BranchProfileRepo();

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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

            DataTable dtPurchase = new DataTable();

            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();

            DataTable dt = _repo.SelectAl(vm.BranchId);
            vm.dtConnectionInfo = dt;
            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("dd-MMM-yyyy");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("dd-MMM-yyyy");

            //CommonRepo commonRepo = new CommonRepo(identity, Session);
            //string value = commonRepo.settings("CompanyCode", "Code");
            String CompanyCode = Convert.ToString(Session["CompanyCode"]);
            vm.CompanyCode = CompanyCode;

            //FileLogger.Log("IntegrationController", "GetPurchaseData_Nourish", vm.FromDate + vm.ToDate);
            dtPurchase = _IntegrationRepo.GetPurchaseData_Nourish(vm);

            //DataColumn column6 = new DataColumn("Selected", typeof(int));
            //DataColumn column1 = new DataColumn("ID", typeof(int));
            //DataColumn column2 = new DataColumn("BranchCode", typeof(string));
            //DataColumn column3 = new DataColumn("vendor_name", typeof(string));
            //DataColumn column4 = new DataColumn("vendor_code", typeof(string));
            //DataColumn column5 = new DataColumn("Receive_Date", typeof(string));


            //dtPurchase.Columns.Add(column1);
            //dtPurchase.Columns.Add(column2);
            //dtPurchase.Columns.Add(column3);
            //dtPurchase.Columns.Add(column4);
            //dtPurchase.Columns.Add(column5);
            //dtPurchase.Columns.Add(column6);

            //DataRow row1 = dtPurchase.NewRow();
            //row1["Selected"] = 0;
            //row1["ID"] = 1;
            //row1["BranchCode"] = "001";
            //row1["vendor_name"] = "abc";
            //row1["vendor_code"] = "222";
            //row1["Receive_Date"] = "22-jan-2";

            //dtPurchase.Rows.Add(row1);

            //FileLogger.Log("IntegrationController", "GetPurchaseData_Nourish", JsonConvert.SerializeObject(dtPurchase));
            return PartialView("_PurchaseBody_Nourish", dtPurchase);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetPurchaseData_Detail_Nourish(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtPurchases = new DataTable();
                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
                String CompanyCode = Convert.ToString(Session["CompanyCode"]);
                vm.CompanyCode = CompanyCode;
                dtPurchases = _IntegrationRepo.GetPurchase_DBData_Nourish(vm);

                return PartialView("_Purchase_Detail_Nourish", dtPurchases);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult SavePurchase_Nourish(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            try
            {
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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();
                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("dd-MMM-yyyy");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("dd-MMM-yyyy");

                String CompanyCode = Convert.ToString(Session["CompanyCode"]);
                vm.CompanyCode = CompanyCode;

                vm.CurrentUserId = Session["LogInUserId"].ToString();
                //FileLogger.Log("IntegrationController", "SavePurchase_Nourish", string.Join("','", vm.IDs));
                rVM = _IntegrationRepo.SavePurchase_Nourish(vm);
            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;
                FileLogger.Log("IntegrationController", "SavePurchase_Nourish", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);


        }

        [Authorize]
        ////[HttpGet]
        public ActionResult GetIssueData_Nourish(IntegrationParam vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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
            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();

            DataTable dtIssue = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("dd-MMM-yyyy");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("dd-MMM-yyyy");
            String CompanyCode = Convert.ToString(Session["CompanyCode"]);
            vm.CompanyCode = CompanyCode;

            dtIssue = _IntegrationRepo.GetPIssueData_Nourish(vm);

            return PartialView("_IssueBody_Nourish", dtIssue);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetIssueData_Detail_Nourish(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtIssue = new DataTable();

                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("dd-MMM-yyyy");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("dd-MMM-yyyy");

                String CompanyCode = Convert.ToString(Session["CompanyCode"]);
                vm.CompanyCode = CompanyCode;
                dtIssue = _IntegrationRepo.GetIssue_DBData_Nourish(vm);

                return PartialView("_Issue_Detail_Nourish", dtIssue);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveIssue_Nourish(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

            try
            {


                string Token = DateTime.Now.ToString("yyyyMMddmmss") + "~" + identity.UserId;
                Session["currentExcelToken"] = Token;
                vm.CurrentUser = identity.UserId;
                vm.Token = Token;

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.DefaultBranchId = Convert.ToInt32(Session["BranchId"]);


                vm.InvoiceDateTime = DateTime.Now.ToString("yyyy-MM-dd");

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("dd-MMM-yyyy");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("dd-MMM-yyyy");
                String CompanyCode = Convert.ToString(Session["CompanyCode"]);
                vm.CompanyCode = CompanyCode;


                rVM = _IntegrationRepo.SaveIssue_Nourish(vm);


                return Json(rVM, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
                //Fail
                return Json(rVM, JsonRequestBehavior.AllowGet);

            }
        }

        [Authorize]
        public ActionResult GetReceiveData_Nourish(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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
            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();
            DataTable dtReceive = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

            vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("dd-MMM-yyyy");
            vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("dd-MMM-yyyy");
            String CompanyCode = Convert.ToString(Session["CompanyCode"]);
            vm.CompanyCode = CompanyCode;

            dtReceive = _IntegrationRepo.GetPReceiveData_Nourish(vm);

            return PartialView("_ReceiveBody_Nourish", dtReceive);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetReceiveData_Detail_Nourish(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
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


            try
            {
                DataTable dtReceives = new DataTable();

                vm.BranchCode = Session["BranchCode"].ToString();

                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                vm.dtConnectionInfo = branchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });
                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("dd-MMM-yyyy");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("dd-MMM-yyyy");
                String CompanyCode = Convert.ToString(Session["CompanyCode"]);
                vm.CompanyCode = CompanyCode;

                dtReceives = _IntegrationRepo.GetReceive_DBData_Nourish(vm);

                return PartialView("_Receive_Detail_Nourish", dtReceives);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveReceive_Nourish(IntegrationParam vm)
        {


            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

            try
            {
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


                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();
                vm.DefaultBranchId = Convert.ToInt32(Session["BranchId"]);

                vm.CurrentUserId = Session["LogInUserId"].ToString();

                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

                vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("dd-MMM-yyyy");
                vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("dd-MMM-yyyy");
                String CompanyCode = Convert.ToString(Session["CompanyCode"]);
                vm.CompanyCode = CompanyCode;


                rVM = _IntegrationRepo.SaveReceive_Nourish(vm);
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;

            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        ////[HttpGet]
        public ActionResult GetTransferData_Nourish(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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


            vm.BranchCode = Session["BranchCode"].ToString();
            vm.BranchId = Session["BranchId"].ToString();


            DataTable dtTransfer = new DataTable();
            BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

            vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });

            //vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
            //vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");


            dtTransfer = _IntegrationRepo.GetTransferData__Nourish(vm);

            return PartialView("_TransferBody_Nourish", dtTransfer);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetTransferData_Detail_Nourish(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);

            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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
            SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            #endregion

            DataTable dtTransfer = new DataTable();
            vm.BranchCode = Session["BranchCode"].ToString();

            dtTransfer = _IntegrationRepo.GetSource_TransferData_Detail_Nourish(vm);

            return PartialView("_TransferBody_Detail_Nourish", dtTransfer);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveTransferData_Nourish(IntegrationParam vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _IntegrationRepo = new IntegrationRepo(identity, Session);
            ResultVM rVM = new ResultVM();
            try
            {
                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;

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

                vm.BranchCode = Session["BranchCode"].ToString();
                vm.BranchId = Session["BranchId"].ToString();

                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl(null, new[] { "BranchCode" }, new[] { vm.BranchCode });


                //vm.FromDate = Convert.ToDateTime(vm.FromDate).ToString("yyyy-MM-dd");
                //vm.ToDate = Convert.ToDateTime(vm.ToDate).ToString("yyyy-MM-dd");
                vm.CurrentUserId = Session["LogInUserId"].ToString();

                rVM = _IntegrationRepo.SaveTransferData_Nourish(vm);

            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                rVM.Status = "Fail";
                rVM.Message = msg;
                //FileLogger.Log("IntegrationController", "SaveSale_EON", ex.Message + "\n" + ex.StackTrace + "\n");
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
