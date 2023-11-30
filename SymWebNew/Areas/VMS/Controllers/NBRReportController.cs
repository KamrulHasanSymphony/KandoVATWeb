using CrystalDecisions.CrystalReports.Engine;
//using JQueryDataTables.Models;
using SymOrdinary;
using SymphonySofttech.Reports.Report;
using SymRepository.VMS;
using SymVATWebUI.Areas.VMS.Models;
using VATViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using CrystalDecisions.Shared;
using Newtonsoft.Json;
using SymphonySofttech.Reports;
////using VATServer.Library;
using VATServer.Ordinary;
using SymRepository;
using SymphonySofttech.Utilities;
using VATServer.Library;
using FileLogger = SymOrdinary.FileLogger;
using System.Configuration;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class NBRReportController : Controller
    {
        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //SaleInvoiceRepo _SaleInvoiceRepo = new SaleInvoiceRepo(identity);
        ShampanIdentity identity = null;

        SaleInvoiceRepo _SaleInvoiceRepo = null;
        NBRReportRepo _NBRReportRepo = null;
        CommonRepo _CommonRepo = null;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public NBRReportController()
        {
            ////HttpSessionStateBase session = Session;
            try
            {

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _SaleInvoiceRepo = new SaleInvoiceRepo(identity);
                _NBRReportRepo = new NBRReportRepo(identity);
                _CommonRepo = new CommonRepo(identity);

                connVM.SysDatabaseName = identity.InitialCatalog;
                connVM.SysUserName = SysDBInfoVM.SysUserName;
                connVM.SysPassword = SysDBInfoVM.SysPassword;
                connVM.SysdataSource = SysDBInfoVM.SysdataSource;

                //connVM = Ordinary.StaticValueReAssign(identity, session);
            }
            catch
            {

            }

        }

        public NBRReportController(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, Session);

        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
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
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexCEL()
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
            return View();
        }

        #region VAT 9.1

        [Authorize]
        [HttpGet]
        public ActionResult ViewVAT9_1()
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
            VATReturnVM vm = new VATReturnVM();

            vm.PeriodName = DateTime.Now.AddMonths(-1).ToString("MMMM-yyyy");
            vm.BranchId = Convert.ToInt32(Session["BranchId"].ToString());

            vm.BranchList = getBranchList();

            return View("ViewVAT9_1", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult VAT9_1Load(VATReturnVM vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            settingVM.SettingsDTUser = null;
            StaticValueReAssign(identity);

            _NBRReportRepo = new NBRReportRepo(identity, Session);


            DataSet dsVAT9_1 = new DataSet();

            DataTable dt = new DataTable();

            if (vm.BranchId == 0)
            {
                vm.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            }

            if (vm.BranchId == -1)
            {
                vm.BranchId = 0;
            }


            if (vm.Operation == "Save")
            {
                UserInformationRepo _UserInformationRepo = new UserInformationRepo(identity, Session);
                UserInformationVM varUserInformationVM = new UserInformationVM();
                varUserInformationVM = _UserInformationRepo.SelectAll(Convert.ToInt32(identity.UserId)).FirstOrDefault();
                vm.varVATReturnHeaderVM.SignatoryName = varUserInformationVM.FullName;
                vm.varVATReturnHeaderVM.SignatoryDesig = varUserInformationVM.Designation;
                vm.varVATReturnHeaderVM.Mobile = varUserInformationVM.Mobile;
                vm.varVATReturnHeaderVM.Email = varUserInformationVM.Email;
                dsVAT9_1 = _NBRReportRepo.VAT9_1_CompleteSave(vm);
            }

            dsVAT9_1 = _NBRReportRepo.VAT9_1_CompleteLoad(vm);

            if (dsVAT9_1 != null && dsVAT9_1.Tables.Count > 0)
            {
                dt = dsVAT9_1.Tables[0];
            }

            return PartialView("_vat9_1Load", dt);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportVAT9_1(VATReturnVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                NBRReports _reportClass = new NBRReports();
                bool PreviewOnly = true;

                vm.PeriodName = Convert.ToDateTime(vm.PeriodName).ToString("MMMM-yyyy");

                if (vm.BranchId == 0)
                {
                    vm.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (vm.BranchId == -1)
                {
                    vm.BranchId = 0;
                }

                OrdinaryVATDesktop.CurrentUserID = identity.UserId;

                reportDocument = _reportClass.VAT9_1Print(vm, null);

                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "No data available!";
                    return RedirectToAction("Index");
                }

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");
            }
            catch (Exception ex)
            {
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


        [Authorize]
        [HttpGet]
        public ActionResult PrintVAT9_1SubForm(VATReturnSubFormVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                NBRReports _reportClass = new NBRReports();
                vm.FontSize = "7";
                vm.PeriodName = Convert.ToDateTime(vm.PeriodName).ToString("MMMM-yyyy");
                vm.IsSummary = true;

                vm.post1 = "y";
                vm.post2 = "n";
                if (!string.IsNullOrWhiteSpace(vm.Post))
                {
                    if (vm.Post.ToLower() == "y")
                    {
                        vm.post1 = "y";
                        vm.post2 = "y";
                    }
                    else if (vm.Post.ToLower() == "n")
                    {
                        vm.post1 = "n";
                        vm.post2 = "n";
                    }
                }
                if (vm.BranchId == 0)
                {
                    vm.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }
                else if (vm.BranchId == -1)
                {
                    vm.BranchId = 0;
                }


                reportDocument = _reportClass.VAT9_1_SubForm(vm, connVM);


                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "No data available!";
                    return RedirectToAction("Index");
                }


                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");
            }
            catch (Exception ex)
            {
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


        [Authorize]
        [HttpGet]
        public ActionResult DownloadVAT9_1SubForm(VATReturnSubFormVM vm)
        {
            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                NBRReports _reportClass = new NBRReports();

                vm.PeriodName = Convert.ToDateTime(vm.PeriodName).ToString("MMMM-yyyy");
                //  vm.IsSummary = true;
                vm.post1 = "y";
                vm.post2 = "n";
                if (!string.IsNullOrWhiteSpace(vm.Post))
                {
                    if (vm.Post.ToLower() == "y")
                    {
                        vm.post1 = "y";
                        vm.post2 = "y";
                    }
                    else if (vm.Post.ToLower() == "n")
                    {
                        vm.post1 = "n";
                        vm.post2 = "n";
                    }
                }
                if (vm.BranchId == 0)
                {
                    vm.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }
                else if (vm.BranchId == -1)
                {
                    vm.BranchId = 0;
                }

                vm = _reportClass.VAT9_1_SubForm_Download(vm);

                if (vm.varExcelPackage == null)
                {
                    Session["result"] = "Fail" + "~" + "No data available!";
                    return RedirectToAction("Index");
                }


                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + vm.FileName + ".xlsx");
                    vm.varExcelPackage.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        public JsonResult Get_VATReturnHeader(VATReturnVM vm)
        {
            VATReturnHeaderVM varHeaderVM = new VATReturnHeaderVM();
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                _NBRReportRepo = new NBRReportRepo(identity, Session);
                varHeaderVM = _NBRReportRepo.SelectAll_VATReturnHeader_Model(vm);


            }
            catch (Exception)
            {

                throw;
            }
            finally { }

            return Json(varHeaderVM, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FiscalPeriodCheck(string PeriodName)
        {
            ResultVM rVM = new ResultVM();
            rVM.Status = "";

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                StaticValueReAssign(identity);

                List<FiscalYearVM> fiscalYearVms = new List<FiscalYearVM>();
                FiscalYearVM fiscalYearVM = new FiscalYearVM();
                FiscalYearRepo _repo = null;
                _repo = new FiscalYearRepo(identity);

                string[] cFields = { "PeriodName" };
                string[] cValues = { PeriodName };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();

                varFiscalYearVM = _repo.SelectAll(0, cFields, cValues, null, null).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    rVM.Status = "Fail";
                    rVM.Message = PeriodName + ": This Fiscal Period is not Exist!";
                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    rVM.Status = "Fail";
                    rVM.Message = PeriodName + ": VAT Return (9.1) already submitted for this month!";
                }

            }
            catch (Exception e)
            {
                rVM.Status = "Fail";
                rVM.Message = e.Message;
            }

            return Json(rVM, JsonRequestBehavior.AllowGet);

        }

        #region Unused


        [Authorize(Roles = "Admin")]
        public ActionResult LoadVAT9_1(JQueryDataTableParamVM param, VAT19header paramVM)
        {
            ////ReportDSDAL _ReportDSDAL = new ReportDSDAL();
            bool ckbVAT9_1 = true;
            int BranchId = Convert.ToInt32(Session["BranchId"]);

            List<VAT19header> getAllData = new List<VAT19header>();
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

            if (!string.IsNullOrWhiteSpace(paramVM.MonthYear))
            {
                dtFrom = paramVM.MonthYear;
            }

            DateTime PeriodStart = Convert.ToDateTime(paramVM.MonthYear);

            DateTime HardDecember2019 = Convert.ToDateTime("December-2019");

            if (PeriodStart < HardDecember2019)
            {
                ckbVAT9_1 = false;
            }


            #endregion SeachParameters

            DataSet dsVAT9_1 = new DataSet();

            if (ckbVAT9_1)
            {
                //////Version 2
                ////dsVAT9_1 = _ReportDSDAL.VAT9_1_V2Load(Convert.ToDateTime(paramVM.MonthYear).ToString("MMMM-yyyy"), BranchId, Convert.ToDateTime(paramVM.MonthYear).ToString("dd-MMM-yyyy"));

            }
            else
            {
                ////dsVAT9_1 = _ReportDSDAL.VAT9_1(Convert.ToDateTime(paramVM.MonthYear).ToString("MMMM-yyyy"), BranchId, Convert.ToDateTime(paramVM.MonthYear).ToString("dd-MMM-yyyy"));

            }



            #endregion

            #region Search and Filter Data
            IEnumerable<VAT19header> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.MonthYear.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data


            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<VAT19header, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.MonthYear :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.MonthYear
             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }


        private DataSet getVAT19Result(string monthYear)
        {
            ReportDSRepo repo = new ReportDSRepo(identity, Session);
            return repo.VAT19NewNewformat(monthYear, "Y");
        }

        private Vat19ViewModel getVAT19(string monthYear)
        {
            var VAT19Result = getVAT19Result(monthYear);
            Vat19ViewModel vm = new Vat19ViewModel();
            VAT19Result.Tables[0].TableName = "DsVAT19";
            DataRow dr;
            dr = VAT19Result.Tables[0].Rows[0];
            vm.Line10PurchasePrice = Convert.ToDecimal(dr[1]);
            vm.Line1SalePrice = Convert.ToDecimal(dr[1]);
            vm.Line1Duty = Convert.ToDecimal(dr[2]);
            vm.Line1VatPrice = Convert.ToDecimal(dr[3]);
            vm.Line2SalePrice = Convert.ToDecimal(dr[4]);
            vm.Line2Duty = Convert.ToDecimal(dr[5]);
            vm.Line2VatPrice = Convert.ToDecimal(dr[6]);
            vm.Line3SalePrice = Convert.ToDecimal(dr[7]);
            vm.Line4Amount = Convert.ToDecimal(dr[8]);
            vm.Line5Amount = Convert.ToDecimal(dr[9]);
            vm.Line6Amount = Convert.ToDecimal(dr[10]);
            vm.Line7PurchasePrice = Convert.ToDecimal(dr[11]);
            vm.Line7VatPrice = Convert.ToDecimal(dr[12]);
            vm.Line8PurchasePrice = Convert.ToDecimal(dr[13]);
            vm.Line8VatPrice = Convert.ToDecimal(dr[14]);
            vm.Line9PurchasePrice = Convert.ToDecimal(dr[15]);
            vm.Line9VatPrice = Convert.ToDecimal(dr[16]);
            vm.Line10PurchasePrice = Convert.ToDecimal(dr[17]);
            vm.Line11Amount = Convert.ToDecimal(dr[18]);
            vm.Line12Amount = Convert.ToDecimal(dr[19]);
            vm.Line13Amount = Convert.ToDecimal(dr[20]);
            vm.Line14Amount = Convert.ToDecimal(dr[21]);
            vm.Line15Amount = Convert.ToDecimal(dr[22]);
            vm.Line16Amount = Convert.ToDecimal(dr[23]);
            vm.Line17Amount = Convert.ToDecimal(dr[24]);
            vm.Line18Amount = Convert.ToDecimal(dr[25]);
            vm.Line19Amount = Convert.ToDecimal(dr[26]);
            return vm;
        }

        #endregion

        #region Old Version

        [Authorize(Roles = "Admin")]
        public ActionResult VAT19Load(string VAT19Date)
        {
            var monthYear = Convert.ToDateTime(VAT19Date).ToString("MMMM-yyyy");
            var vm = getVAT19(monthYear);
            return PartialView("_vat19load", vm);
        }

        #endregion

        #endregion

        #region VAT 6.3

        [Authorize]
        //[HttpPost]
        public ActionResult Report_VAT6_3(ReportParamVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {


                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);


                SaleReport _reportClass = new SaleReport();

                if (string.IsNullOrWhiteSpace(vm.SalesInvoiceNo))
                {
                    vm.SalesInvoiceNo = vm.InvoiceNo;
                }

                bool IsBlank = false;

                string Blank = _CommonRepo.settings("Sale", "IsBlank");

                IsBlank = Blank == "Y";

                if (vm.TransactionType == "TollIssue")
                {
                    reportDocument = _reportClass.Report_VAT6_3_Completed("'" + vm.SalesInvoiceNo + "'",
                      null, false, false, false, false, true, false,
                       vm.PreviewOnly, vm.PrintCopy, 0, false, false, false, false, false, false, connVM);

                }
                else if (vm.TransactionType == "Credit")
                {
                    reportDocument = _reportClass.Report_VAT6_3_Completed(vm.SalesInvoiceNo, vm.TransactionType
                    , true, false, false, false, false, false, vm.PreviewOnly, 0, 0, false, false, false, false, false, false, connVM);

                }
                else if (vm.TransactionType == "Debit")
                {
                    reportDocument = _reportClass.Report_VAT6_3_Completed(vm.SalesInvoiceNo, vm.TransactionType
                   , false, true, false, false, false, false, vm.PreviewOnly, 0, 0, false, false, false, false, false, false, connVM);

                }
                else
                {
                    if (OrdinaryVATDesktop.IsUnileverCompany(Convert.ToString(Session["CompanyCode"])))
                    {
                        string SKUCount = "";
                        DataTable dtresult = new DataTable();
                        SaleInvoiceRepo _SaleInvoice = new SaleInvoiceRepo(identity, Session);
                        dtresult = _SaleInvoice.GetSource_SaleData_dis_Details(new IntegrationParam { RefNo = vm.SalesInvoiceNo });

                        if (dtresult != null && dtresult.Rows.Count > 0)
                        {
                            SKUCount = dtresult.Rows[0]["SKUCount"].ToString();
                        }
                        reportDocument = _reportClass.Report_VAT6_3_Completed("'" + vm.SalesInvoiceNo + "'",
                     vm.TransactionType, false, false, false, false, false, false,
                      vm.PreviewOnly, vm.PrintCopy, 0, IsBlank, false, false, false, false, false, connVM, SKUCount);
                    }
                    else
                    {
                        reportDocument = _reportClass.Report_VAT6_3_Completed("'" + vm.SalesInvoiceNo + "'",
                  vm.TransactionType, false, false, false, false, false, false,
                   vm.PreviewOnly, vm.PrintCopy, 0, IsBlank, false, false, false, false, false, connVM);
                    }


                }

                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }



                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);


                return new FileStreamResult(stream, "application/pdf");



            }
            catch (Exception ex)
            {
                FileLogger.Log("Sale", "Report_VAT6_3", ex.ToString());

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


        [Authorize]
        //[HttpPost]
        public ActionResult MegnaReport_VAT6_3(ReportParamVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {


                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);


                SaleReport _reportClass = new SaleReport();

                //if (string.IsNullOrWhiteSpace(vm.SalesInvoiceNo))
                //{
                //    vm.SalesInvoiceNo = vm.InvoiceNo;
                //}

                bool IsBlank = false;

                string Blank = _CommonRepo.settings("Sale", "IsBlank");

                IsBlank = Blank == "Y";

                reportDocument = _reportClass.MegnaReport_VAT6_3_Completed("'" + vm.Id + "'",
          vm.TransactionType, false, false, false, false, false, false,
           vm.PreviewOnly, vm.PrintCopy, 0, IsBlank, false, false, false, false, false, connVM);




                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }



                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);


                return new FileStreamResult(stream, "application/pdf");



            }
            catch (Exception ex)
            {
                FileLogger.Log("Sale", "Report_VAT6_3", ex.ToString());

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

        [Authorize]
        [HttpPost]
        public ActionResult MultiplePreviewReport_VAT6_3(Vat16ViewModel paramVM)
        {
            ReportDocument reportDocument = new ReportDocument();

            ResultVM rVM = new ResultVM();
            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);

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

                //IDs = "'" + IDs + "'";

                SaleReport _reportClass = new SaleReport();
                bool PreviewOnly = true;
                reportDocument = _reportClass.Report_VAT6_3_Completed("'" + IDs + "'", null, false, false, false, false, false
                    , false, true, 0, 0, false, false, false, false, false, false, connVM);

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


        [Authorize]
        [HttpPost]
        public ActionResult MultiplePrintReport_VAT6_3(Vat16ViewModel paramVM)
        {
            ResultVM rVM = new ResultVM();
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);

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

                //IDs = "'" + IDs + "'";

                SaleReport _reportClass = new SaleReport();
                bool PreviewOnly = true;
                reportDocument = _reportClass.Report_VAT6_3_Completed("'" + IDs + "'", null, false, false, false, false, false
                   , false, false, 1, 1, false, false, false, false, false, false, connVM);

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


        [Authorize]
        [HttpGet]
        public ActionResult PreviewVAT6_3(Vat16ViewModel vm)
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

            vm.FontSize = 8;
            vm.BranchId = Convert.ToInt32(Session["BranchId"]);

            return PartialView("_VAT6_3Preview", vm);
        }


        [Authorize]
        [HttpGet]
        public ActionResult PreviewVAT6_6(Vat16ViewModel vm)
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

            vm.FontSize = 8;
            vm.BranchId = Convert.ToInt32(Session["BranchId"]);

            return PartialView("_VAT6_6Preview", vm);
        }


        public ActionResult GetVAT6_3PopUp(PopUpViewModel vm)
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

            vm.BranchId = Convert.ToInt32(Session["BranchId"]);

            return PartialView("_VAT6_3", vm);

        }


        public ActionResult GetVAT6_6PopUp(PopUpViewModel vm)
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

            vm.BranchId = Convert.ToInt32(Session["BranchId"]);

            return PartialView("_VAT6_6", vm);

        }

        [Authorize]
        //[HttpPost]
        public ActionResult Report_VAT6_6(ReportParamVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {


                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);

                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();

                //SaleReport _reportClass = new SaleReport();

                //if (string.IsNullOrWhiteSpace(vm.DepositId))
                //{
                //    vm.DepositId = vm.InvoiceNo;
                //}

                //bool IsBlank = false;

                //string Blank = _CommonRepo.settings("Sale", "IsBlank");

                //IsBlank = Blank == "Y";

                NBRReports _report = new NBRReports();
                reportDocument = _report.VDS12KhaNew_Multiple(vm.DepositId, true, connVM);



                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }



                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);


                return new FileStreamResult(stream, "application/pdf");



            }
            catch (Exception ex)
            {
                FileLogger.Log("Deposit", "Report_VAT6_6", ex.ToString());

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


        public ActionResult GetFilteredVAT6_3(SaleMasterVM vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            settingVM.SettingsDTUser = null;
            StaticValueReAssign(identity);

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

            //////conditionalFields = new string[] { "SalesInvoiceNo like" };
            //////conditionalValues = new string[] { vm.SearchValue };

            conditionalFields = new string[] { "SalesInvoiceNo like", "sih.TransactionType", "sih.BranchId" };
            conditionalValues = new string[] { vm.SearchValue, vm.TransactionType, vm.BranchId.ToString() };

            SaleInvoiceRepo _repo = new SaleInvoiceRepo(identity, Session);
            var list = _repo.SelectAll(0, conditionalFields, conditionalValues);
            return PartialView("_filteredVAT6_3", list);


        }

        public ActionResult GetFilteredVAT6_6(DepositMasterVM vm)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            settingVM.SettingsDTUser = null;
            StaticValueReAssign(identity);

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


            //string transactionOpening = transactionType1 + "-Opening";

            string[] conditionalFields = { "d.DepositId like", "d.DepositId like", "d.TreasuryNo like", "d.DepositDateTime>", "d.DepositDateTime<", "d.DepositType", "d.ChequeNo like", "d.ChequeDate>", "d.ChequeDate<", "b.BankName like", "b.BranchName like", "b.AccountNumber like", "d.TransactionType", "d.Post", "d.BranchId" };
            string[] conditionalValues = { vm.SearchValue,vm.DepositId,vm.TreasuryNo,vm.IssueDateTimeFrom,vm.IssueDateTimeTo, vm.DepositType,vm.ChequeNo,vm.ChequeDate,vm.CheckDateTo,vm.BankName,vm.BranchName,vm.AccountNumber
                                       , vm.TransactionType,vm.Post, vm.BranchId.ToString()};
            DepositRepo _repo = new DepositRepo(identity, Session);
            var list = _repo.SelectAll(0, conditionalFields, conditionalValues);

            //string[] columnNames = { "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn" };

            //DepositResult = OrdinaryVATDesktop.DtDeleteColumns(DepositResult, columnNames);


            return PartialView("_filteredVAT6_6", list);


        }


        [Authorize]
        [HttpGet]
        public ActionResult PrintVAT11(string SalesInvoiceNo, string Ids)
        {
            #region Access Control
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            settingVM.SettingsDTUser = null;
            StaticValueReAssign(identity);

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

            #region Data Call

            string[] conditionFields;
            string[] conditionValues;
            conditionFields = new string[] { "sih.SalesInvoiceNo" };
            conditionValues = new string[] { SalesInvoiceNo };
            SaleInvoiceRepo _repo = new SaleInvoiceRepo(identity, Session);
            _CommonRepo = new CommonRepo(identity, Session);

            SaleMasterVM SaleInvoice = _repo.SelectAll(0, conditionFields, conditionValues, null, null, null).FirstOrDefault();
            string AlReadyPrint = SaleInvoice.AlReadyPrint;

            #endregion

            #region Settings Value

            string vPrintCopy = _CommonRepo.settings("Sale", "ReportNumberOfCopiesPrint");
            string vDefaultPrinter = _CommonRepo.settings("Printer", "DefaultPrinter");
            string Is3Plyer = _CommonRepo.settings("Sale", "Page3Plyer");

            #endregion

            #region Value Assign

            ReportParamVM vm = new ReportParamVM();

            vm.AlreadyPrintCopy = Convert.ToInt32(AlReadyPrint ?? "0");
            vm.InvoiceNo = SalesInvoiceNo;
            vm.PrintCopy = Convert.ToInt32(vPrintCopy ?? "0");
            vm.TransactionType = SaleInvoice.TransactionType;
            ////vm.Id = Convert.ToInt32(Ids);

            #endregion

            return PartialView("_printVAT11", vm);
        }



        [Authorize]
        [HttpPost]
        public ActionResult ReportVAT11(Vat16ViewModel vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);

                SaleInvoiceRepo _repo = new SaleInvoiceRepo(identity, Session);

                string[] sqlResults;
                int PrintCopy = 1;
                vm.TransactionTypes = "Other";
                sqlResults = _SaleInvoiceRepo.UpdatePrintNew(vm.SalesInvoiceNo, PrintCopy);// Change 04
                int NewPrintCopy = Convert.ToInt32(sqlResults[3]) - 1;

                var reports = new SaleReport();

                reportDocument = reports.Report_VAT6_3_Completed(vm.SalesInvoiceNo, vm.TransactionTypes, false, false, false, false, false, false, false,
                    PrintCopy, NewPrintCopy, false, false, false, false, false, false, connVM);

                reportDocument.PrintOptions.PrinterName = vm.PrinterName;
                reportDocument.PrintToPrinter(1, false, 0, 0);


                return RedirectToAction("Edit", "SaleInvoice", new { id = vm.Id });

            }

            catch (Exception ex)
            {
                throw ex;
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






        #endregion

        #region VAT 6.7

        public ActionResult Report_VAT6_7(string SalesInvoiceNo)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);

                SaleReport _reportClass = new SaleReport();

                SaleInvoiceRepo _repo = new SaleInvoiceRepo(identity, Session);
                SaleMasterVM vm = new SaleMasterVM();
                vm = _repo.SelectAll(0, new string[] { "sih.SalesInvoiceNo" }, new string[] { SalesInvoiceNo }, null, null, null).FirstOrDefault();
                bool PreviewOnly = true;

                if (vm.Post == "Y")
                {
                    PreviewOnly = false;
                }

                reportDocument = _reportClass.Report_VAT6_3_Completed(SalesInvoiceNo, "Credit"
                    , true, false, false, false, false, false, PreviewOnly, 0, 0, false, false, false, false, false, false, connVM);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception ex)
            {
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

        [Authorize]
        [HttpGet]
        public ActionResult PrintVAT6_7(string SalesInvoiceNo, string Ids)
        {

            //string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            //if (project.ToLower() == "vms")
            //{
            //    if (!identity.IsAdmin)
            //    {

            //    }
            //}
            //else
            //{
            //    Session["rollPermission"] = "deny";
            //    return Redirect("/Vms/Home");
            //}

            ////string[] conditionFields;
            ////string[] conditionValues;
            ////conditionFields = new string[] { "sih.SalesInvoiceNo" };
            ////conditionValues = new string[] { SalesInvoiceNo };
            ////SaleInvoiceRepo _repo = new SaleInvoiceRepo();

            ////var SaleInvoice = _repo.SelectAll(0, conditionFields, conditionValues, null, null, null).FirstOrDefault();
            ////var AlReadyPrint = SaleInvoice.AlReadyPrint;

            //Vat16ViewModel vm = new Vat16ViewModel();
            //vm.SalesInvoiceNo = SalesInvoiceNo;
            ////CommonDAL commonDal = new CommonDAL();

            ////string vPrintCopy = commonDal.settings("Sale", "ReportNumberOfCopiesPrint");
            ////string vDefaultPrinter = commonDal.settings("Printer", "DefaultPrinter");
            ////string Is3Plyer = commonDal.settings("Sale", "Page3Plyer");


            ////vm.copyNo = AlReadyPrint;
            ////vm.SalesInvoiceNo = SalesInvoiceNo;
            //vm.pageNo = "1";
            //vm.Id = Convert.ToInt32(Ids);
            #region Access Control
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            settingVM.SettingsDTUser = null;
            StaticValueReAssign(identity);

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

            #region Data Call

            string[] conditionFields;
            string[] conditionValues;
            conditionFields = new string[] { "sih.SalesInvoiceNo" };
            conditionValues = new string[] { SalesInvoiceNo };
            SaleInvoiceRepo _repo = new SaleInvoiceRepo(identity, Session);
            _CommonRepo = new CommonRepo(identity, Session);

            SaleMasterVM SaleInvoice = _repo.SelectAll(0, conditionFields, conditionValues, null, null, null).FirstOrDefault();
            string AlReadyPrint = SaleInvoice.AlReadyPrint;

            #endregion

            #region Settings Value

            string vPrintCopy = _CommonRepo.settings("Sale", "ReportNumberOfCopiesPrint");
            string vDefaultPrinter = _CommonRepo.settings("Printer", "DefaultPrinter");
            string Is3Plyer = _CommonRepo.settings("Sale", "Page3Plyer");

            #endregion

            #region Value Assign

            ReportParamVM vm = new ReportParamVM();

            vm.AlreadyPrintCopy = Convert.ToInt32(AlReadyPrint ?? "0");
            vm.InvoiceNo = SalesInvoiceNo;
            vm.PrintCopy = Convert.ToInt32(vPrintCopy ?? "0");
            ////vm.Id = Convert.ToInt32(Ids);

            #endregion

            return PartialView("_printVAT6_7", vm);
        }


        [Authorize]
        [HttpPost]
        public ActionResult PrintReportVAT6_7(ReportParamVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {

                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                //DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                //settingVM.SettingsDTUser = null;
                //StaticValueReAssign(identity);

                //SaleReport _reportClass = new SaleReport();

                //if (string.IsNullOrWhiteSpace(vm.SalesInvoiceNo))
                //{
                //    vm.SalesInvoiceNo = vm.InvoiceNo;
                //}

                //ReportDocument reportDocument = _reportClass.Report_VAT6_3_Completed("'" + vm.SalesInvoiceNo + "'",
                //    null, false, false, false, false, false, false,
                //     vm.PreviewOnly, vm.PrintCopy, 0, false, false, false);

                //var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);


                //reportDocument.Dispose();

                //return new FileStreamResult(stream, "application/pdf");

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);

                string[] sqlResults;
                int PrintCopy = 1;
                if (string.IsNullOrWhiteSpace(vm.SalesInvoiceNo))
                {
                    vm.SalesInvoiceNo = vm.InvoiceNo;
                }


                var reports = new SaleReport();

                reportDocument = reports.Report_VAT6_3_Completed(vm.SalesInvoiceNo, "Credit", true,
                      false, false, false, false, false, false, vm.PrintCopy, 0, false, false, false, false, false, false, connVM);

                //reportDocument.PrintOptions.PrinterName = vm.PrinterName;
                //reportDocument.PrintToPrinter(1, false, 0, 0);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);


                reportDocument.Dispose();

                return new FileStreamResult(stream, "application/pdf");
                //return RedirectToAction("Edit", "SaleInvoice", new { id = vm.Id });

            }

            catch (Exception ex)
            {
                throw ex;
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

        #endregion

        #region VAT 6.8

        public ActionResult Report_VAT6_8(string SalesInvoiceNo)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);

                SaleReport _reportClass = new SaleReport();

                SaleInvoiceRepo _repo = new SaleInvoiceRepo(identity, Session);
                SaleMasterVM vm = new SaleMasterVM();
                vm = _repo.SelectAll(0, new string[] { "sih.SalesInvoiceNo" }, new string[] { SalesInvoiceNo }, null, null, null).FirstOrDefault();
                bool PreviewOnly = true;

                if (vm.Post == "Y")
                {
                    PreviewOnly = false;
                }

                reportDocument = _reportClass.Report_VAT6_3_Completed(SalesInvoiceNo, "Debit"
                   , false, true, false, false, false, false, PreviewOnly, 0, 0, false, false, false, false, false, false, connVM);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception ex)
            {
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

        public ActionResult Report_PurchaseReturn(string SalesInvoiceNo)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);

                SaleReport _reportClass = new SaleReport();

                PurchaseRepo _repo = new PurchaseRepo(identity, Session);
                PurchaseMasterVM vm = new PurchaseMasterVM();
                vm = _repo.SelectAll(0, new string[] { "pih.PurchaseInvoiceNo" }, new string[] { SalesInvoiceNo }, null, null, null, null, false).FirstOrDefault();
                bool PreviewOnly = true;

                if (vm.Post == "Y")
                {
                    PreviewOnly = false;
                }

                reportDocument = _reportClass.Report_VAT6_3_Completed(SalesInvoiceNo, "PurchaseReturn"
                   , false, false, false, false, false, false, PreviewOnly, 0, 0, false, false, false, false, false, false, connVM, "", false, true);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception ex)
            {
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

        [Authorize]
        [HttpGet]
        public ActionResult PrintVAT6_8(string SalesInvoiceNo, string Ids)
        {

            //string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            //if (project.ToLower() == "vms")
            //{
            //    if (!identity.IsAdmin)
            //    {

            //    }
            //}
            //else
            //{
            //    Session["rollPermission"] = "deny";
            //    return Redirect("/Vms/Home");
            //}

            ////string[] conditionFields;
            ////string[] conditionValues;
            ////conditionFields = new string[] { "sih.SalesInvoiceNo" };
            ////conditionValues = new string[] { SalesInvoiceNo };
            ////SaleInvoiceRepo _repo = new SaleInvoiceRepo();

            ////var SaleInvoice = _repo.SelectAll(0, conditionFields, conditionValues, null, null, null).FirstOrDefault();
            ////var AlReadyPrint = SaleInvoice.AlReadyPrint;

            //Vat16ViewModel vm = new Vat16ViewModel();
            //vm.SalesInvoiceNo = SalesInvoiceNo;
            ////CommonDAL commonDal = new CommonDAL();

            ////string vPrintCopy = commonDal.settings("Sale", "ReportNumberOfCopiesPrint");
            ////string vDefaultPrinter = commonDal.settings("Printer", "DefaultPrinter");
            ////string Is3Plyer = commonDal.settings("Sale", "Page3Plyer");


            ////vm.copyNo = AlReadyPrint;
            ////vm.SalesInvoiceNo = SalesInvoiceNo;
            //vm.pageNo = "1";
            //vm.Id = Convert.ToInt32(Ids);
            #region Access Control
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
            settingVM.SettingsDTUser = null;
            StaticValueReAssign(identity);

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

            #region Data Call

            string[] conditionFields;
            string[] conditionValues;
            conditionFields = new string[] { "sih.SalesInvoiceNo" };
            conditionValues = new string[] { SalesInvoiceNo };
            SaleInvoiceRepo _repo = new SaleInvoiceRepo(identity, Session);
            _CommonRepo = new CommonRepo(identity, Session);

            SaleMasterVM SaleInvoice = _repo.SelectAll(0, conditionFields, conditionValues, null, null, null).FirstOrDefault();
            string AlReadyPrint = SaleInvoice.AlReadyPrint;

            #endregion

            #region Settings Value

            string vPrintCopy = _CommonRepo.settings("Sale", "ReportNumberOfCopiesPrint");
            string vDefaultPrinter = _CommonRepo.settings("Printer", "DefaultPrinter");
            string Is3Plyer = _CommonRepo.settings("Sale", "Page3Plyer");

            #endregion

            #region Value Assign

            ReportParamVM vm = new ReportParamVM();

            vm.AlreadyPrintCopy = Convert.ToInt32(AlReadyPrint ?? "0");
            vm.InvoiceNo = SalesInvoiceNo;
            vm.PrintCopy = Convert.ToInt32(vPrintCopy ?? "0");
            ////vm.Id = Convert.ToInt32(Ids);

            #endregion

            return PartialView("_printVAT6_8", vm);
        }


        [Authorize]
        [HttpPost]
        public ActionResult PrintReportVAT6_8(ReportParamVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {

                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                //DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                //settingVM.SettingsDTUser = null;
                //StaticValueReAssign(identity);

                //SaleReport _reportClass = new SaleReport();

                //if (string.IsNullOrWhiteSpace(vm.SalesInvoiceNo))
                //{
                //    vm.SalesInvoiceNo = vm.InvoiceNo;
                //}

                //ReportDocument reportDocument = _reportClass.Report_VAT6_3_Completed("'" + vm.SalesInvoiceNo + "'",
                //    null, false, false, false, false, false, false,
                //     vm.PreviewOnly, vm.PrintCopy, 0, false, false, false);

                //var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);


                //reportDocument.Dispose();

                //return new FileStreamResult(stream, "application/pdf");





                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);

                string[] sqlResults;
                int PrintCopy = 1;
                if (string.IsNullOrWhiteSpace(vm.SalesInvoiceNo))
                {
                    vm.SalesInvoiceNo = vm.InvoiceNo;
                }


                var reports = new SaleReport();

                reportDocument = reports.Report_VAT6_3_Completed(vm.SalesInvoiceNo, "Debit", false,
                   true, false, false, false, false, false, vm.PrintCopy, 0, false, false, false, false, false, false, connVM);

                //reportDocument.PrintOptions.PrinterName = vm.PrinterName;
                //reportDocument.PrintToPrinter(1, false, 0, 0);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);


                reportDocument.Dispose();

                return new FileStreamResult(stream, "application/pdf");
                //return RedirectToAction("Edit", "SaleInvoice", new { id = vm.Id });

            }

            catch (Exception ex)
            {
                throw ex;
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

        #endregion

        [Authorize]
        //[HttpPost]
        public ActionResult Report_Client6_3(ReportParamVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {


                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);


                NBRReports _reportClass = new NBRReports();

                if (string.IsNullOrWhiteSpace(vm.SalesInvoiceNo))
                {
                    vm.SalesInvoiceNo = vm.InvoiceNo;
                }
                int BranchId = Convert.ToInt32(Session["BranchId"]);
                reportDocument = _reportClass.Client6_3(vm.SalesInvoiceNo, BranchId, vm.PreviewOnly, connVM);

                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }



                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);


                return new FileStreamResult(stream, "application/pdf");



            }
            catch (Exception ex)
            {
                FileLogger.Log("Sale", "Report_VAT6_3", ex.ToString());

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

        [Authorize]
        //[HttpPost]
        public ActionResult VAT6_3Toll(ReportParamVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {


                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);


                NBRReports _reportClass = new NBRReports();

                if (string.IsNullOrWhiteSpace(vm.TollNo))
                {
                    vm.TollNo = vm.InvoiceNo;
                }
                int BranchId = Convert.ToInt32(Session["BranchId"]);
                reportDocument = _reportClass.VAT6_3Toll(vm.TollNo, "", "", connVM);

                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }



                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);


                return new FileStreamResult(stream, "application/pdf");



            }
            catch (Exception ex)
            {
                FileLogger.Log("Sale", "Report_VAT6_3", ex.ToString());

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
        public ActionResult chakkha()
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
            ChakKaModel vm = new ChakKaModel();

            vm.Branch = Convert.ToInt32(Session["BranchId"]);
            vm.FontSize = 8;

            return PartialView("_ChakKha", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult chakka()
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
            ChakKaModel vm = new ChakKaModel();
            vm.FontSize = 8;
            vm.Branch = Convert.ToInt32(Session["BranchId"]);


            return PartialView("_ChakKa", vm);
        }
        [Authorize]
        [HttpGet]
        public ActionResult transferIsuue(ReportCommonVM vm)
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
            // = new ReportCommonVM();
            vm.FontSize = 8;
            vm.Branch = Convert.ToInt32(Session["BranchId"]);


            return PartialView("_transferIsuue", vm);
        }


        [Authorize]
        [HttpGet]
        public ActionResult MegnatransferIsuue(ReportCommonVM vm)
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
            // = new ReportCommonVM();
            vm.FontSize = 8;
            vm.Branch = Convert.ToInt32(Session["BranchId"]);


            return PartialView("_megnatransferIsuue", vm);
        }

        /*      
              [Authorize]
              [HttpGet]
              public ActionResult Report_VAT6_5(ReportCommonVM vm)
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
                  // = new ReportCommonVM();
                  vm.FontSize = 8;
                  vm.Branch = Convert.ToInt32(Session["BranchId"]);


                  return PartialView("_transferIsuue", vm);
              }
 
      */

        public ActionResult VAT6_10Report()
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
            ReportCommonVM vm = new ReportCommonVM();
            vm.FontSize = 8;
            vm.Branch = Convert.ToInt32(Session["BranchId"]);


            return PartialView("_VAT6_10", vm);
        }

        #region Old Reports


        [Authorize]
        [HttpGet]
        public ActionResult PrintVAT18Breakdown()
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
            return PartialView("_printVAT18Breakdown", vm);
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

        #endregion

        [Authorize]
        [HttpPost]
        public ActionResult ReportChakKa(ChakKaModel vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                var reports = new NBRReports();

                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();
                vm.Branch = vm.Branch == -1 ? 0 : vm.Branch;
                reportDocument = reports.Chak_kaReport(vm.StartDate, vm.EndDate, vm.Branch, vm.TransferBranch, "Y");

                //var gr = new GenericReport<RptChakKa>();
                //var rpt = gr.RenderReportAsPDF(reportDocument);
                //reportDocument.Close();
                //return rpt;
                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }
                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception ex)
            {
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

        [Authorize]
        [HttpPost]
        public ActionResult Reportchakkha(ChakKaModel vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                var reports = new NBRReports();

                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();
                vm.Branch = vm.Branch == -1 ? 0 : vm.Branch;
                reportDocument = reports.Chak_khaReport(vm.StartDate, vm.EndDate, vm.Branch, "Y");
                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }
                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");
            }

            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message;
                return Redirect("/Vms/NBRReport");
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

        [Authorize]
        //[HttpPost]
        public ActionResult ReporttransferIsuue(ReportCommonVM paramVM)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                TransferIssueVM vm = new TransferIssueVM();
                string[] cFields = { "ti.TransferIssueNo" };
                string[] cValues = { paramVM.IssueNo };

                vm = new TransferIssueRepo(identity, Session).SelectAll(0, cFields, cValues, null, null).FirstOrDefault();

                /*
                if (vm.Post == "Y")
                {
                    paramVM.PreviewOnly = false;
                }
                else
                {
                    paramVM.PreviewOnly = true;
                }
                */

                var reports = new NBRReports();

                OrdinaryVATDesktop.FontSize = "9";
                bool PreviewOnly = paramVM.PreviewOnly;

                reportDocument = reports.TransferIssueNew(paramVM.IssueNo, "", "", "", "", "", "", "", PreviewOnly);
                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }
                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                reportDocument.Dispose();

                return new FileStreamResult(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message;
                return Redirect("/Vms/NBRReport");
                ////throw ex;
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

        [Authorize]
        //[HttpPost]
        public ActionResult MegnaReporttransferIsuue(ReportCommonVM paramVM)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                TransferMPLIssueVM vm = new TransferMPLIssueVM();
                string[] cFields = { "TI.TransferIssueNo" };
                string[] cValues = { paramVM.IssueNo };

                vm = new TransferIssueMPLRepo(identity, Session).SelectAll(0, cFields, cValues, null, null).FirstOrDefault();


                var reports = new NBRReports();

                OrdinaryVATDesktop.FontSize = "9";
                bool PreviewOnly = paramVM.PreviewOnly;

                reportDocument = reports.MegnaTransferIssueNew(vm.TransferIssueNo, "", "", "", "", "", "", "", PreviewOnly);
                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }
                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                reportDocument.Dispose();

                return new FileStreamResult(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message;
                return Redirect("/Vms/NBRReport");
                ////throw ex;
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

        public ActionResult ReportVAT6_10(ReportCommonVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                string TotalAmount = _CommonRepo.settings("VAT6_10", "ChallanRange");

                string post1;
                string post2;
                if (vm.PreviewOnly == true)
                {
                    if (string.IsNullOrWhiteSpace(vm.Post))
                    {
                        post1 = "y";
                        post2 = "N";
                    }
                    else if (vm.Post.ToLower() == "y")
                    {
                        post1 = "Y";
                        post2 = "Y";
                    }
                    else if (vm.Post.ToLower() == "n")
                    {
                        post1 = "N";
                        post2 = "N";
                    }
                    else
                    {
                        post1 = "y";
                        post2 = "N";
                    }

                    //post1 = "Y";
                    //post2 = "N";
                }
                else
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                var reports = new NBRReports();

                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();

                if (vm.Branch == -1)
                {
                    vm.Branch = 0;
                }

                OrdinaryVATDesktop.CurrentUserID = identity.UserId;

                reportDocument = reports.VAT6_10Report(TotalAmount, vm.StartDate, vm.EndDate, post1, post2, vm.Branch, "Y");
                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }
                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message;
                return Redirect("/Vms/NBRReport");
                ////throw ex;
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

        [Authorize]
        [HttpPost]
        public ActionResult ReportVAT17(Vat16ViewModel vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                string post1;
                string post2;

                #region Conditional Values

                if (vm.PreviewOnly == true)
                {
                    post1 = "Y";
                    post2 = "N";
                }
                else
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                if (vm.ItemNo == null)
                {
                    vm.ItemNo = "";
                }
                if (vm.StartDate == null)
                {
                    vm.StartDate = "";
                }
                if (vm.EndDate == null)
                {
                    vm.EndDate = "";
                }

                #endregion

                //int branchId = Convert.ToInt32(Session["BranchId"]);
                int branchId = vm.Branch == 0 ? vm.BranchId : vm.Branch;

                NBRReports _reportClass = new NBRReports();

                #region Parmeter Assign

                VAT6_2ParamVM varVAT6_2ParamVM = new VAT6_2ParamVM();

                varVAT6_2ParamVM.ItemNo = vm.ItemNo;
                varVAT6_2ParamVM.StartDate = vm.StartDate;
                varVAT6_2ParamVM.EndDate = vm.EndDate;
                varVAT6_2ParamVM.Post1 = post1;
                varVAT6_2ParamVM.Post2 = post2;
                varVAT6_2ParamVM.BranchId = branchId;
                varVAT6_2ParamVM.PreviewOnly = vm.PreviewOnly;
                varVAT6_2ParamVM.InEnglish = vm.InEnglish;
                varVAT6_2ParamVM.UserId = identity.UserId;
                varVAT6_2ParamVM.IsMonthly = vm.IsMonthly;
                varVAT6_2ParamVM.FontSize = Convert.ToString(vm.FontSize);
                varVAT6_2ParamVM.BranchWise = vm.BranchWise && branchId != -1;

                ProductRepo productRepo = new ProductRepo(identity, Session);
                ProductVM product = productRepo.SelectAll("0", new[] { "pr.ItemNo" }, new[] { vm.ItemNo }).FirstOrDefault();

                if (product == null)
                {
                    product = productRepo.SelectAll("0", new[] { "pr.ProductCode" }, new[] { vm.ItemNo }).FirstOrDefault();

                    if (product != null)
                    {
                        varVAT6_2ParamVM.ItemNo = product.ItemNo;
                    }
                    else
                    {
                        throw new Exception("Product Not Valid");
                    }
                }

                #endregion

                #region process flag check
                CommonDAL commonDal = new CommonDAL();
                string dayend = commonDal.settings("DayEnd", "DayEndProcess", null, null, connVM);

                if (!string.IsNullOrEmpty(dayend) && dayend == "Y")
                {

                    try
                    {
                        //CommonDAL commonDal = new CommonDAL();
                        CommonRepo commonRepo = new CommonRepo();
                        commonRepo.CheckProcessFlag(varVAT6_2ParamVM.ItemNo, varVAT6_2ParamVM.ProdutCategoryId, varVAT6_2ParamVM.ProdutType);

                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message.Split('\r').FirstOrDefault();

                        Session["result"] = "Fail~" + msg;

                        return View("DailyActivities");

                    }
                }
                #endregion

                reportDocument = _reportClass.VAT6_2(varVAT6_2ParamVM, connVM);

                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }

            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message.Split('\n').FirstOrDefault();

                return Redirect("/Vms/NBRReport");
                ////throw ex;
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
            if (itemNo != null)
            {
                vm.ItemNo = itemNo;
                vm.StartDate = invoiceDate;
                vm.EndDate = invoiceDate;
                vm.InEnglish = "Y";
            }
            vm.FontSize = 8;

            vm.BranchList = getBranchList();

            return PartialView("_printVAT16", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportVAT16(VAT6_1ParamVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();
            CommonRepo commonRepo = new CommonRepo();
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                NBRReports _reportClass = new NBRReports();

                #region Condition Fields

                if (vm.PreviewOnly == true)
                {
                    vm.Post1 = "y";
                    vm.Post2 = "N";
                }
                else
                {
                    vm.Post1 = "y";
                    vm.Post2 = "Y";
                }

                if (vm.ItemNo == null)
                {
                    vm.ItemNo = "";
                }
                if (vm.StartDate == null)
                {
                    vm.StartDate = "";
                }
                if (vm.EndDate == null)
                {
                    vm.EndDate = "";
                }

                //int branchId = Convert.ToInt32(Session["BranchId"]);

                //vm.BranchId = branchId;

                #endregion

                vm.UserName = identity.UserId;
                vm.UserId = identity.UserId;
                //vm.InEnglish ="Y";

                CommonDAL commonDAl = new CommonDAL();
                string dayend = commonDAl.settings("DayEnd", "DayEndProcess", null, null, connVM);

                #region process flag check

                if (!string.IsNullOrEmpty(dayend) && dayend == "Y")
                {
                    try
                    {
                        CommonDAL commonDal = new CommonDAL();
                        //commonDal.CheckProcessFlag(varVAT6_1ParamVM.ItemNo, varVAT6_1ParamVM.ProdutCategoryId, varVAT6_1ParamVM.ProdutType);
                        commonRepo.CheckProcessFlag(vm.ItemNo, vm.ProdutCategoryId, vm.ProdutType);

                    }
                    catch (Exception ex)
                    {

                        string msg = ex.Message.Split('\r').FirstOrDefault();

                        Session["result"] = "Fail~" + msg;

                        return View("DailyActivities", vm);
                        //MessageBox.Show(ex.Message);

                        //FormRegularProcess formRegular = new FormRegularProcess();
                        //formRegular.ShowDialog();

                        //return;
                    }
                }
                #endregion

                reportDocument = _reportClass.VAT6_1_WithConn(vm, connVM);

                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }
                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }

            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message;
                return Redirect("/Vms/NBRReport");
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

        [Authorize]
        [HttpPost]
        public ActionResult ReportIssuePreview(string issueNo)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                // NBRReports _reportClass = new NBRReports();

                #region Condition Fields


                int branchId = Convert.ToInt32(Session["BranchId"]);

                #endregion

                MISReport _report = new MISReport();
                //reportDocument = _report.IssueInformation(txtIssueNo.Text.Trim(), Program.BranchId);


                reportDocument = _report.IssueInformation(issueNo, branchId);  //_reportClass.VAT6_1_WithConn(vm);
                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }
                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");


            }

            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message;
                return Redirect("/Vms/NBRReport");
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

        [Authorize]
        [HttpGet]
        public ActionResult PrintVAT6_2_1(string itemNo, string invoiceDate)
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
            if (itemNo != null)
            {
                vm.ItemNo = itemNo;
                vm.StartDate = invoiceDate;
                vm.EndDate = invoiceDate;
            }
            vm.FontSize = 8;
            vm.BranchId = Convert.ToInt32(Session["BranchId"]);

            vm.BranchList = getBranchList();

            return PartialView("_printVAT6_2_1", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportVAT6_2_1(Vat16ViewModel vm)
        {
            ReportDocument reportDocument = new ReportDocument();
            CommonRepo commonRepo = new CommonRepo();

            try
            {
                vm.BranchId = vm.BranchId == -1 ? 0 : vm.BranchId;
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ProductRepo _repo = new ProductRepo(identity, Session);

                string post1;
                string post2;

                #region Conditional Values

                if (vm.PreviewOnly == true)
                {
                    post1 = "Y";
                    post2 = "N";
                }
                else
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                if (vm.ItemNo == null)
                {
                    vm.ItemNo = "";
                }
                if (vm.StartDate == null)
                {
                    vm.StartDate = "";
                }
                if (vm.EndDate == null)
                {
                    vm.EndDate = "";
                }

                #endregion
                #region Trading Product Check
                ProductVM vProductVM = new ProductVM();

                vProductVM = _repo.SelectAll("0", new[] { "Pr.ProductCode" }, new[] { vm.ItemNo }, null, null, null).FirstOrDefault();
                if (vProductVM == null)
                {
                    vProductVM = _repo.SelectAll(vm.ItemNo).FirstOrDefault();

                }
                if (vProductVM.ProductType != "Trading")
                {
                    Session["result"] = "Fail" + "~" + vProductVM.ProductName + " is not a Trading Product!";
                    return Redirect("/Vms/NBRReport");
                }
                #endregion

                //int branchId = Convert.ToInt32(Session["BranchId"]);

                NBRReports _reportClass = new NBRReports();

                #region Parmeter Assign
                string ItemNo = vProductVM.ItemNo;
                string StartDate = Convert.ToDateTime(vm.StartDate).ToString("yyyy-MMM-dd");
                string EndDate = Convert.ToDateTime(vm.EndDate).ToString("yyyy-MMM-dd");
                string Post1 = post1;
                string Post2 = post2;
                bool PreviewOnly = vm.PreviewOnly;
                string InEnglish = "Y";


                //VAT6_2ParamVM varVAT6_2ParamVM = new VAT6_2ParamVM();

                //varVAT6_2ParamVM.ItemNo = vm.ItemNo;
                //varVAT6_2ParamVM.StartDate = vm.StartDate;
                //varVAT6_2ParamVM.EndDate = vm.EndDate;
                //varVAT6_2ParamVM.Post1 = post1;
                //varVAT6_2ParamVM.Post2 = post2;
                //varVAT6_2ParamVM.BranchId = branchId;
                //varVAT6_2ParamVM.PreviewOnly = vm.PreviewOnly;
                //varVAT6_2ParamVM.InEnglish = "Y";

                #endregion

                VAT6_1ParamVM pVM = new VAT6_1ParamVM();

                #region process flag check

                CommonDAL commonDal = new CommonDAL();

                string dayend = commonDal.settings("DayEnd", "DayEndProcess", null, null, connVM);

                if (!string.IsNullOrEmpty(dayend) && dayend == "Y")
                {

                    try
                    {
                        //CommonDAL commonDal = new CommonDAL();
                        commonRepo.CheckProcessFlag(ItemNo, null, null);

                    }
                    catch (Exception ex)
                    {

                        string msg = ex.Message.Split('\r').FirstOrDefault();

                        Session["result"] = "Fail~" + msg;

                        return View("DailyActivities", pVM);

                    }
                }

                #endregion

                reportDocument = _reportClass.StockMovement6_2_1(ItemNo, StartDate, EndDate, vm.BranchId, null, null, post1, post2, "", "", connVM, PreviewOnly, InEnglish, identity.UserId);
                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }
                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }

            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message;
                return Redirect("/Vms/NBRReport");
                ////throw ex;
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
        [Authorize]
        [HttpPost]
        public ActionResult ReportVAT1(Vat16ViewModel vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                var reports = new NBRReports();

                /// vm.Branch = Convert.ToInt32(Session["BranchId"]);

                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();

                reportDocument = reports.BOMNew(vm.BOMId, vm.VatName, "Y", vm.Branch, vm.InEnglish);
                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }
                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");
            }

            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message;
                return Redirect("/Vms/NBRReport");
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

        [Authorize]
        [HttpGet]
        public ActionResult PrintVAT17(string itemNo, string invoiceDate)
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
            if (itemNo != null)
            {
                vm.ItemNo = itemNo;
                vm.StartDate = invoiceDate;
                vm.EndDate = invoiceDate;
            }
            vm.IsMonthly = false;
            vm.FontSize = 8;
            vm.BranchList = getBranchList();

            return PartialView("_printVAT17", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult PrintVAT1(string itemNo, string invoiceDate, string BOMId, string vatName, string ProductName)
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
            vm.Branch = Convert.ToInt32(Session["BranchId"]);

            if (itemNo != null)
            {
                vm.ItemNo = itemNo;
                vm.EndDate = invoiceDate;
                vm.BOMId = BOMId;
                vm.VatName = vatName;
                vm.ProductName = ProductName;
                vm.InEnglish = "Y";
            }
            vm.FontSize = 8;

            vm.BranchList = getBranchList();

            return PartialView("_printVAT1", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult PrintVATSD(string invoiceDate)
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
            vm.FontSize = 8;
            return PartialView("_printVATSD", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult PrintVAT18Summery()
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
            return PartialView("_printVAT18Summery", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportVATSD(Vat16ViewModel vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
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


                NBRReports _report = new NBRReports();
                reportDocument = _report.SD_Data(identity.FullName, vm.StartDate, vm.EndDate, post1, post2, "Y");
                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }
                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

                #region Old

                //var ReportResult = repo.SD_Data(identity.FullName, vm.StartDate, vm.EndDate, post1, post2);
                //ReportResult.Tables[0].TableName = "DsVAT18";
                //RptSD objrpt = new RptSD();
                //if (vm.PreviewOnly == true)
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                //}
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'Address2'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'Address3'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'VatRegistrationNo'";
                //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'Trial'";

                //objrpt.SetDataSource(ReportResult);

                //var gr = new GenericReport<RptSD>();
                //var rpt = gr.RenderReportAsPDF(objrpt);
                //objrpt.Close();
                //return rpt;
                #endregion


            }

            catch (Exception ex)
            {
                throw ex;
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

        [Authorize]
        [HttpPost]
        public ActionResult ReportVAT18Summery(Vat16ViewModel vm)
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
                var startPeriod = Convert.ToDateTime(vm.StartDate).ToString("yyyy-MM-dd");
                var endPeriod = Convert.ToDateTime(vm.EndDate).ToString("yyyy-MM-dd");

                var ReportResult = repo.Current_AC_VAT18(startPeriod, endPeriod, post1, post2);
                ReportResult.Tables[0].TableName = "DsCurrentVAT18";
                RptCurrent_AC_VAT18 objrpt = new RptCurrent_AC_VAT18();
                if (vm.PreviewOnly == true)
                {
                    //objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                }
                else
                {
                    //objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                }
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1 '";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'Address2'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'Address3'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'VatRegistrationNo'";
                objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + Convert.ToDateTime(vm.StartDate).ToString("yy") + "'  ";
                objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + Convert.ToDateTime(vm.EndDate).ToString("yy") + "'  ";
                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'Trial'";

                objrpt.SetDataSource(ReportResult);


                var gr = new GenericReport<RptCurrent_AC_VAT18>();
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
        [HttpPost]
        public ActionResult ReportVAT18Breakdown(Vat16ViewModel vm)
        {
            try
            {
                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();

                var VAT18Result = repo.VAT18Breakdown(Convert.ToDateTime(vm.StartDate).ToString("MMM-yyyy"), "BDT");
                VAT18Result.Tables[0].TableName = "DsVAT18Breakdown";
                objrpt = new RptVAT18Breakdown();
                objrpt.SetDataSource(VAT18Result);
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'Address2'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'Address3'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo '";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'VatRegistrationNo'";
                objrpt.DataDefinition.FormulaFields["Currency"].Text = "'BDT'";


                var rpt = gr.RenderReportAsPDF(objrpt);
                objrpt.Close();
                return rpt;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        public JsonResult GetSartEndPeriod(string year)
        {
            var vm = new FiscalYearRepo(identity, Session).StartEndPeriod(year);
            string startPeriod = Ordinary.DateTimeToDate(vm.PeriodStart);
            string endPeriod = Ordinary.DateTimeToDate(vm.PeriodEnd);
            string result = startPeriod + "~" + endPeriod;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void StaticValueReAssign(ShampanIdentity identity)
        {
            try
            {
                #region Get Company Information
                CompanyProfileRepo _CompanyProfileRepo = new CompanyProfileRepo(identity); // need change
                CompanyProfileVM varCompanyProfileVM = new CompanyProfileVM();

                //FileLogger.Log("NBRReportController", "StaticValueReAssign", " CompanyId: " + identity.CompanyId );

                ////string CompanyId = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, identity.CompanyId);
                string CompanyId = Session[identity.InitialCatalog].ToString();

                //FileLogger.Log("NBRReportController", "StaticValueReAssign", "DESDecrypt CompanyId: " + CompanyId);


                varCompanyProfileVM = _CompanyProfileRepo.SelectAll(CompanyId).FirstOrDefault();

                if (varCompanyProfileVM == null)
                {
                    throw new Exception("Company Not found " + CompanyId);
                }

                OrdinaryVATDesktop.CompanyName = StaticValueNullCheck(varCompanyProfileVM.CompanyLegalName, "CompanyLegalName");
                OrdinaryVATDesktop.Address1 = StaticValueNullCheck(varCompanyProfileVM.Address1, "Address1");

                //OrdinaryVATDesktop.Address1 = varCompanyProfileVM.Address1;

                //OrdinaryVATDesktop.Address2 = varCompanyProfileVM.Address2;
                //OrdinaryVATDesktop.Address3 = varCompanyProfileVM.Address3;
                //OrdinaryVATDesktop.TelephoneNo = varCompanyProfileVM.TelephoneNo;
                //OrdinaryVATDesktop.FaxNo = varCompanyProfileVM.FaxNo;
                //OrdinaryVATDesktop.VatRegistrationNo = varCompanyProfileVM.VatRegistrationNo;
                //OrdinaryVATDesktop.Section = varCompanyProfileVM.Section;

                OrdinaryVATDesktop.CompanyName = StaticValueNullCheck(varCompanyProfileVM.CompanyLegalName, "CompanyName");
                OrdinaryVATDesktop.Address1 = StaticValueNullCheck(varCompanyProfileVM.Address1, "Address1");
                OrdinaryVATDesktop.Address2 = StaticValueNullCheck(varCompanyProfileVM.Address2, "Address2");
                OrdinaryVATDesktop.Address3 = StaticValueNullCheck(varCompanyProfileVM.Address3, "Address3");
                OrdinaryVATDesktop.TelephoneNo = StaticValueNullCheck(varCompanyProfileVM.TelephoneNo, "TelephoneNo");
                OrdinaryVATDesktop.FaxNo = StaticValueNullCheck(varCompanyProfileVM.FaxNo, "FaxNo");
                OrdinaryVATDesktop.VatRegistrationNo = StaticValueNullCheck(varCompanyProfileVM.VatRegistrationNo, "VatRegistrationNo");
                OrdinaryVATDesktop.Section = StaticValueNullCheck(varCompanyProfileVM.Section, "Section");

                #endregion

                #region Get Branch Information
                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity); // need to change Session
                BranchProfileVM varBranchProfileVM = new BranchProfileVM();
                varBranchProfileVM = branchProfileRepo.SelectAll(Convert.ToString(Session["BranchId"])).FirstOrDefault();
                OrdinaryVATDesktop.IsWCF = "N";
                //OrdinaryVATDesktop.IsWCF = StaticValueNullCheck(varBranchProfileVM.IsWCF, "IsWCF");
                #endregion
                OrdinaryVATDesktop.CurrentUser = Convert.ToString(Session["LogInUserName"]);
            }
            catch (Exception ex)
            {
                ////FileLogger.Log("NBRReportController", "StaticValueReAssign", ex.ToString());

                throw ex;
            }


        }

        private string StaticValueNullCheck(string value, string valueSource)
        {
            string val = value;

            if (string.IsNullOrWhiteSpace(val))
            {
                //FileLogger.Log("NBRReportController", "StaticValueNullCheck", valueSource + " : " + val);

                val = "-";
            }

            return val;
        }

        [Authorize]
        [HttpGet]
        public ActionResult DataProcess()
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
            // Vat16ViewModel vm = new Vat16ViewModel();

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportProcess(AVGPriceVm vm)
        {
            try
            {
                IssueHeaderRepo _repo = null;
                _repo = new IssueHeaderRepo(identity);

                ResultVM rVM = new ResultVM();
                // AVGPriceVm priceVm = new AVGPriceVm();

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


                vm.AvgDateTime = Convert.ToDateTime(vm.AvgDateTime).ToString("yyyy-MMM-dd 00:00:00");

                if (vm.flag.ToLower() == "process")
                {
                    rVM = _repo.UpdateAvgPrice_New(vm);
                }
                else
                {
                    rVM = _repo.UpdateAvgPrice_New_Refresh(vm);
                }


                // Vat16ViewModel vm = new Vat16ViewModel();
                //Session["result"] = result[0] + "~" + result[1];
                Session["result"] = rVM.Status + "~" + rVM.Message;
                return View("DataProcess", vm);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult Process6_1Permanent(VAT6_1ParamVM vm)
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
            string todate = DateTime.Now.ToString("MMMM-yyyy");
            vm.PeriodMonth = todate;

            // Vat16ViewModel vm = new Vat16ViewModel();

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult _Process6_1Permanent(VAT6_1ParamVM vm)
        {

            try
            {
                IssueHeaderRepo _repo = null;
                _repo = new IssueHeaderRepo(identity);

                ProductRepo repo = null;
                repo = new ProductRepo(identity);

                FiscalYearVM fiscalYearVM = new FiscalYearVM();

                ResultVM rVM = new ResultVM();
                // AVGPriceVm priceVm = new AVGPriceVm();

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
                if (vm.Flag.ToLower() == "process")
                {
                    CommonDAL commonDal = new CommonDAL();
                    string vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", null);

                    ////VAT6_1ParamVM varVAT6_1ParamVM = new VAT6_1ParamVM();
                    if (vm.IsChecked != true)
                    {
                        // PeriodDetails(FromPeriodName.Value.ToString("MMyyyy"));



                        string[] cFields = { "PeriodId" };
                        string[] cValues = { Convert.ToDateTime(vm.PeriodMonth).ToString("MMyyyy") };

                        fiscalYearVM = new FiscalYearRepo(identity, Session).SelectAll(0, cFields, cValues).FirstOrDefault();

                        vm.StartDate = Convert.ToDateTime(fiscalYearVM.PeriodStart).ToString("dd-MMM-yyyy");
                        vm.EndDate = Convert.ToDateTime(fiscalYearVM.PeriodEnd).ToString("dd-MMM-yyyy");
                    }

                    ////vm.StartDate = vm.StartDate;
                    ////vm.EndDate = vm.EndDate;

                    vm.Post1 = "Y";
                    vm.Post2 = "Y";
                    vm.BranchId = 0;
                    vm.PreviewOnly = false;
                    vm.InEnglish = "N";
                    vm.UOMConversion = 1;
                    vm.UOM = "";
                    vm.UOMTo = "";
                    vm.UserName = Convert.ToString(Session["LogInUserName"]);
                    vm.ReportName = "";
                    vm.Opening = false;
                    vm.OpeningFromProduct = false;

                    vm.IsMonthly = false;
                    vm.IsTopSheet = false;
                    vm.IsGroupTopSheet = false;
                    vm.Is6_1Permanent = true;

                    vm.UserId = identity.UserId;

                    string[] results = _repo.SaveVAT6_1_Permanent(vm, null, null, connVM);
                    results = _repo.SaveVAT6_1_Permanent_Branch(vm, null, null, connVM);
                    //rVM = _repo.SaveVAT6_1_Permanent(vm);
                    //rVM = _repo.SaveVAT6_1_Permanent_Branch(vm);
                    rVM.Message = results[1].ToString();
                    rVM.Status = results[0].ToString();

                    Session["result"] = "Success" + "~" + "Data Successfully Saved Permanently(6.1)!";
                    return View("Process6_1Permanent", vm);

                }
                else
                {
                    string itemNo = vm.ItemNo;
                    rVM = repo.Delete6_1Permanent(itemNo);
                    rVM = repo.Delete6_1Permanent_Branch(itemNo);
                    Session["result"] = "Success" + "~" + "Data has been deleted in VAT 6.1 permanent table!";
                    return View("Process6_1Permanent", vm);

                }



                // Vat16ViewModel vm = new Vat16ViewModel();
                //Session["result"] = result[0] + "~" + result[1];
                Session["result"] = rVM.Status + "~" + rVM.Message;
                return View("Process6_1Permanent", vm);
            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;

                return View("Process6_1Permanent", vm);
            }

        }

        [Authorize]
        public ActionResult ExportNegaiveData()
        {
            #region Access Control
            ProductRepo _repo = null;
            _repo = new ProductRepo(identity, Session);

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

            List<SaleMasterVM> getAllData = new List<SaleMasterVM>();

            try
            {

                DataSet ds = _repo.SelectNegInventoryData("6_1");

                //var dataSet = new DataSet();
                //dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);
                //dataSet.Tables.Add(dt);

                //if (dt.Rows.Count == 0)
                //{
                //    dt.Rows.Add(dt.NewRow());
                //}

                //var vm = OrdinaryVATDesktop.DownloadExcel(dt, "VAT_6_1_Negetive", "VAT_6_1_Negetive");

                DataTable dt = new DataTable();
                DataTable Branchdt = new DataTable();


                var dataSet = new DataSet();
                dt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[0].Copy());
                dataSet.Tables.Add(dt);
                Branchdt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[1].Copy());
                dataSet.Tables.Add(Branchdt);

                var sheetNames = new[] { "VAT_6_1_Negetive", "VAT_6_1_BranchWiseNegetive" };

                var vm = OrdinaryVATDesktop.DownloadExcelMultiple(dataSet, "VAT_6_1_Negetive", sheetNames);


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
            catch (Exception)
            {


            }

            finally { }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Process6_2Permanent(VAT6_2ParamVM vm)
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
            string todate = DateTime.Now.ToString("MMMM-yyyy");
            vm.PeriodMonth = todate;

            // Vat16ViewModel vm = new Vat16ViewModel();

            return View(vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Process6_2_1Permanent(VAT6_2ParamVM vm)
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
            string todate = DateTime.Now.ToString("MMMM-yyyy");
            vm.PeriodMonth = todate;
            vm.VAT6_2_1 = true;
            // Vat16ViewModel vm = new Vat16ViewModel();

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult _Process6_2Permanent(VAT6_2ParamVM vm)
        {

            IssueHeaderRepo _repo = null;
            _repo = new IssueHeaderRepo(identity);

            ProductRepo repo = null;
            repo = new ProductRepo(identity);

            FiscalYearVM fiscalYearVM = new FiscalYearVM();

            ResultVM rVM = new ResultVM();

            CommonRepo commonRepo = new CommonRepo(identity, Session);
            string code = commonRepo.settings("CompanyCode", "Code");
            string[] result = new string[6];
            // AVGPriceVm priceVm = new AVGPriceVm();

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

            try
            {

                if (vm.Flag.ToLower() == "process")
                {
                    CommonDAL commonDal = new CommonDAL();
                    string vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", null, connVM);


                    ////VAT6_1ParamVM varVAT6_1ParamVM = new VAT6_1ParamVM();
                    if (vm.IsChecked != true)
                    {
                        // PeriodDetails(FromPeriodName.Value.ToString("MMyyyy"));

                        string[] cFields = { "PeriodId" };
                        string[] cValues = { Convert.ToDateTime(vm.PeriodMonth).ToString("MMyyyy") };

                        fiscalYearVM = new FiscalYearRepo(identity, Session).SelectAll(0, cFields, cValues).FirstOrDefault();

                        vm.StartDate = Convert.ToDateTime(fiscalYearVM.PeriodStart).ToString();
                        vm.EndDate = Convert.ToDateTime(fiscalYearVM.PeriodEnd).ToString();
                    }

                    vm.StartDate = Convert.ToDateTime(vm.StartDate).ToString("dd-MMM-yyyy");
                    vm.EndDate = Convert.ToDateTime(vm.EndDate).ToString("dd-MMM-yyyy");

                    vm.Post1 = "Y";
                    vm.Post2 = "Y";
                    vm.BranchId = 0;
                    vm.rbtnService = false;
                    vm.rbtnWIP = false;
                    vm.UOMTo = "";

                    vm.IsBureau = false;
                    if (code == "246")
                    {
                        vm.IsBureau = true;
                    }

                    vm.AutoAdjustment = vAutoAdjustment == "Y";
                    vm.PreviewOnly = false;
                    vm.InEnglish = "N";

                    vm.UOM = "";
                    vm.IsMonthly = false;
                    vm.IsTopSheet = false;
                    vm.UserId = identity.UserId;

                    //string[] results = _repo.SaveVAT6_2_Permanent(vm);
                    //results = _repo.SaveVAT6_2_Permanent_Branch(vm);
                    string[] results = _repo.SaveVAT6_2_Permanent(vm, null, null, connVM);
                    results = _repo.SaveVAT6_2_Permanent_Branch(vm, null, null, connVM);
                    rVM.Status = results[0].ToString();
                    rVM.Message = results[1].ToString();

                    Session["result"] = "Success" + "~" + "Data Successfully Saved Permanently(6.2)!";
                    return View("Process6_2Permanent", vm);


                }
                else
                {
                    string itemNo = vm.ItemNo;

                    rVM = repo.Delete6_2Permanent(itemNo);
                    rVM = repo.Delete6_2Permanent_Branch(itemNo);
                    Session["result"] = "Success" + "~" + "Data has been deleted in VAT 6.2 permanent table!";
                    return View("Process6_2Permanent", vm);

                }

            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;

                return View("Process6_2Permanent", vm);
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult _Process6_2_1Permanent(VAT6_2ParamVM vm)
        {

            IssueHeaderRepo _repo = null;
            _repo = new IssueHeaderRepo(identity);

            ProductRepo repo = null;
            repo = new ProductRepo(identity);

            FiscalYearVM fiscalYearVM = new FiscalYearVM();

            ResultVM rVM = new ResultVM();

            CommonRepo commonRepo = new CommonRepo(identity, Session);
            string code = commonRepo.settings("CompanyCode", "Code");
            string[] result = new string[6];
            // AVGPriceVm priceVm = new AVGPriceVm();

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

            try
            {
                VAT6_2_1Process(vm);

                Session["result"] = "Success" + "~" + "Data Successfully Saved Permanently(6.2.1)!";
                return View("Process6_2_1Permanent", vm);

            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;

                return View("Process6_2_1Permanent", vm);
            }

        }

        [Authorize]
        public ActionResult ExportNegaiveData6_2()
        {
            #region Access Control

            ProductRepo _repo = null;
            _repo = new ProductRepo(identity, Session);

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

            List<SaleMasterVM> getAllData = new List<SaleMasterVM>();

            try
            {

                //DataTable dt = _repo.SelectNegInventoryData("6_2");

                //var dataSet = new DataSet();
                //dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);
                //dataSet.Tables.Add(dt);

                //if (dt.Rows.Count == 0)
                //{
                //    dt.Rows.Add(dt.NewRow());
                //}

                //var vm = OrdinaryVATDesktop.DownloadExcel(dt, "VAT_6_2_Negetive", "VAT_6_2_Negetive");

                DataSet ds = _repo.SelectNegInventoryData("6_2");

                DataTable dt = new DataTable();
                DataTable Branchdt = new DataTable();


                var dataSet = new DataSet();
                dt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[0].Copy());
                dataSet.Tables.Add(dt);
                Branchdt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[1].Copy());
                dataSet.Tables.Add(Branchdt);

                var sheetNames = new[] { "VAT_6_2_Negetive", "VAT_6_2_BranchWiseNegetive" };

                var vm = OrdinaryVATDesktop.DownloadExcelMultiple(dataSet, "VAT_6_2_Negetive", sheetNames);

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
            catch (Exception)
            {


            }

            finally { }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Process6_1_6_2Permanent(VAT6_1ParamVM vm)
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
            string todate = DateTime.Now.ToString("MMMM-yyyy");
            vm.PeriodMonth = todate;

            // Vat16ViewModel vm = new Vat16ViewModel();

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult _Process6_1_6_2Permanent(VAT6_1ParamVM vm, VAT6_2ParamVM vms)
        {

            try
            {

                List<FiscalYearVM> fiscalYearVms = new List<FiscalYearVM>();
                string[] results = new string[5];
                IssueHeaderRepo _repo = null;
                _repo = new IssueHeaderRepo(identity);
                DataSet ds = new DataSet();

                ProductRepo repo = null;
                repo = new ProductRepo(identity);

                FiscalYearVM fiscalYearVM = new FiscalYearVM();

                ResultVM rVM = new ResultVM();

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

                if (vm.ProcesType.ToLower() == "6_1process")
                {
                    VAT6_1Process(vm);

                    //Session["result"] = rVM.Status + "~" + "Data Successfully Saved Permanently(6.1)  !";
                    Session["result"] = "Success" + "~" + "Data Successfully Saved Permanently(6.1)  !";
                    return View("Process6_1_6_2Permanent", vm);

                }
                else if (vm.ProcesType.ToLower() == "6_2process")
                {
                    VAT6_2Process(vms);
                    Session["result"] = "Success" + "~" + "Data Successfully Saved Permanently(6.2)  !";
                    return View("Process6_1_6_2Permanent", vm);
                }

                else if (vm.ProcesType.ToLower() == "6_2_1process")
                {
                    VAT6_2_1Process(vms);
                    Session["result"] = "Success" + "~" + "Data Successfully Saved Permanently(6.2.1)  !";
                    return View("Process6_1_6_2Permanent", vm);
                }

                else if (vm.ProcesType.ToLower() == "bothprocess")
                {
                    BothSaveProcess(vm);
                    Session["result"] = "Success" + "~" + "Data Successfully Saved Permanently(6.1 & 6.2)  !";
                    return View("Process6_1_6_2Permanent", vm);
                }
                else if (vm.ProcesType.ToLower() == "6.1negativedownload")
                {
                    ds = new DataSet();


                    var sheetNames = new[] { "VAT_6_1_Negetive", "VAT_6_1_BranchWiseNegetive" };

                    ds = DataLoad("6_1");//, "VAT_6_1_Negetive", sheetNames


                    DataTable dt = new DataTable();
                    if (dt.Rows.Count == 0)
                    {
                        dt.Rows.Add(dt.NewRow());
                    }
                    DataTable Branchdt = new DataTable();

                    if (Branchdt.Rows.Count == 0)
                    {
                        Branchdt.Rows.Add(Branchdt.NewRow());
                    }
                    var dataSet = new DataSet();
                    dt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[0].Copy());
                    dataSet.Tables.Add(dt);

                    Branchdt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[1].Copy());
                    dataSet.Tables.Add(Branchdt);




                    var excelvm = OrdinaryVATDesktop.DownloadExcelMultiple(dataSet, "VAT_6_1_Negetive", sheetNames);

                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=" + excelvm.FileName + ".xlsx");
                        excelvm.varExcelPackage.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
                else if (vm.ProcesType.ToLower() == "6.2negativedownload")
                {
                    //var sheetNames = new[] { "VAT_6_2_Negetive" };
                    var sheetNames = new[] { "VAT_6_2_Negetive", "VAT_6_2_BranchWiseNegetive" };

                    ds = DataLoad("6_2");

                    DataTable dt = new DataTable();
                    if (dt.Rows.Count == 0)
                    {
                        dt.Rows.Add(dt.NewRow());
                    }
                    DataTable Branchdt = new DataTable();

                    if (Branchdt.Rows.Count == 0)
                    {
                        Branchdt.Rows.Add(Branchdt.NewRow());
                    }
                    var dataSet = new DataSet();
                    dt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[0].Copy());
                    dataSet.Tables.Add(dt);

                    Branchdt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[1].Copy());
                    dataSet.Tables.Add(Branchdt);


                    var excelvm = OrdinaryVATDesktop.DownloadExcelMultiple(dataSet, "VAT_6_2_Negetive", sheetNames);

                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=" + excelvm.FileName + ".xlsx");
                        excelvm.varExcelPackage.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }


                Session["result"] = rVM.Status + "~" + rVM.Message;
                return View("Process6_1_6_2Permanent", vm);
                ////return RedirectToAction("/VMS/NBRReport/Process6_1_6_2Permanent");
            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;

                return View("Process6_1_6_2Permanent", vm);
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult _Process6_1_6_2Delete(VAT6_1ParamVM vm, VAT6_2ParamVM vms)
        {

            try
            {

                List<FiscalYearVM> fiscalYearVms = new List<FiscalYearVM>();
                string[] results = new string[5];
                IssueHeaderRepo _repo = null;
                _repo = new IssueHeaderRepo(identity);

                ProductRepo repo = null;
                repo = new ProductRepo(identity);

                FiscalYearVM fiscalYearVM = new FiscalYearVM();

                ResultVM rVM = new ResultVM();

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

                if (vm.ProcesType.ToLower() == "6_1process")
                {
                    VAT6_1Delete(vm);
                    Session["result"] = rVM.Status + "~" + "Data has been deleted in VAT 6.1 permanent table!";
                    return View("Process6_1_6_2Permanent", vm);

                }
                else if (vm.ProcesType.ToLower() == "6_2process")
                {
                    //VAT6_2Process(vms);
                    VAT6_2Delete(vm);
                    Session["result"] = rVM.Status + "~" + "Data has been deleted in VAT 6.2 permanent table!";
                    return View("Process6_1_6_2Permanent", vm);
                }

                else if (vm.ProcesType.ToLower() == "6_2_1process")
                {
                    VAT6_2_1Delete(vm);
                    //Session["result"] = rVM.Status + "~" + "Data has been deleted in VAT 6.2.1 permanent table!";
                    Session["result"] = "Success" + "~" + "Data has been deleted in VAT 6.2.1 permanent table!";
                    return View("Process6_1_6_2Permanent", vm);
                }

                else if (vm.ProcesType.ToLower() == "bothprocess")
                {
                    BothDeleteProcess(vm);
                    Session["result"] = rVM.Status + "~" + "Data has been deleted in VAT 6.1 & 6.2 permanent table!";
                    return View("Process6_1_6_2Permanent", vm);
                }


                Session["result"] = rVM.Status + "~" + rVM.Message;
                return View("Process6_1_6_2Permanent", vm);
            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;

                return View("Process6_1_6_2Permanent", vm);
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult DailyActivities(VAT6_1ParamVM vm)
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
            //string todate = DateTime.Now.ToString("MMMM-yyyy");
            //vm.PeriodMonth = todate;

            // Vat16ViewModel vm = new Vat16ViewModel();

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult _DailyActivities(VAT6_1ParamVM vm)
        {

            try
            {

                List<FiscalYearVM> fiscalYearVms = new List<FiscalYearVM>();
                string[] results = new string[5];
                IssueHeaderRepo _repo = null;
                _repo = new IssueHeaderRepo(identity);
                ProductRepo repo = null;
                repo = new ProductRepo(identity);
                FiscalYearVM fiscalYearVM = new FiscalYearVM();
                ResultVM rVM = new ResultVM();
                CommonRepo commonRepo = new CommonRepo(identity, Session);

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

                try
                {

                    ProductDAL productdal = new ProductDAL();

                    IntegrationParam processParam = new IntegrationParam
                    {
                        CurrentUserId = identity.UserId,
                        CurrentUserName = identity.Name,
                        IsBureau = false,
                        SetLabel = s => { }
                    };

                    ResultVM resultVM = productdal.DayEndProcess(processParam, connVM);

                    if (resultVM.IsSuccess)
                    {
                        Session["result"] = "Success" + "~" + "Data Successfully Processed !";
                    }
                    else
                    {
                        Session["result"] = "Fail" + "~" + resultVM.Message;

                    }

                    return View("DailyActivities", vm);

                }
                catch (Exception ex)
                {
                    string msg = ex.Message.Split('\r').FirstOrDefault();

                    Session["result"] = "Fail~" + msg;

                    return View("DailyActivities", vm);
                }

                //Session["result"] = rVM.Status + "~" + rVM.Message;
                //return View("DailyActivities", vm);
            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;

                return View("DailyActivities", vm);
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult FreshStockProcess(ParameterVM vm)
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
            //string todate = DateTime.Now.ToString("MMMM-yyyy");
            //vm.PeriodMonth = todate;

            // Vat16ViewModel vm = new Vat16ViewModel();

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult _FreshStockProcess(ParameterVM vm)
        {
            ProductRepo _repo = new ProductRepo(identity);
            string[] results = new string[5];
            try
            {
                //vm.ItemNo = ItemNo;
                vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                vm.CurrentUserID = identity.UserId;
                vm.CurrentUser = Session["LogInUserName"].ToString();

                ResultVM rVM = _repo.ProcessFreshStock(vm);

                results[0] = rVM.Status;
                results[1] = rVM.Message;

                //Session["result"] = rVM.Status + "~" + "Data Successfully Processed !";         
                Session["result"] = "Success" + "~" + "Data Successfully Processed!";
                return View("FreshStockProcess", vm);
            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;

                return View("FreshStockProcess", vm);
            }

        }

        private void VAT6_1Process(VAT6_1ParamVM vm)
        {
            List<FiscalYearVM> fiscalYearVms = new List<FiscalYearVM>();
            FiscalYearVM fiscalYearVM = new FiscalYearVM();
            FiscalYearRepo _repo = null;
            _repo = new FiscalYearRepo(identity);

            try
            {

                VAT6_1ParamVM varVAT6_1ParamVM = new VAT6_1ParamVM();

                varVAT6_1ParamVM.Post1 = "Y";
                varVAT6_1ParamVM.Post2 = "Y";
                varVAT6_1ParamVM.BranchId = 0;
                varVAT6_1ParamVM.PreviewOnly = false;
                varVAT6_1ParamVM.InEnglish = "N";
                varVAT6_1ParamVM.UOMConversion = 1;
                varVAT6_1ParamVM.UOM = "";
                varVAT6_1ParamVM.UOMTo = "";
                varVAT6_1ParamVM.UserName = Convert.ToString(Session["LogInUserName"]);
                varVAT6_1ParamVM.ReportName = "";
                varVAT6_1ParamVM.Opening = false;
                varVAT6_1ParamVM.OpeningFromProduct = false;

                varVAT6_1ParamVM.IsMonthly = false;
                varVAT6_1ParamVM.IsTopSheet = false;
                varVAT6_1ParamVM.IsGroupTopSheet = false;
                varVAT6_1ParamVM.Is6_1Permanent = true;

                varVAT6_1ParamVM.UserId = identity.UserId;

                varVAT6_1ParamVM.FromPeriodName = vm.FromPeriodName;
                varVAT6_1ParamVM.ToPeriodName = vm.ToPeriodName;

                if (vm.IsChecked == true)
                {
                    varVAT6_1ParamVM.ItemNo = vm.ItemNo;
                }

                FiscalYearDAL fiscalYearDAL = new FiscalYearDAL();

                fiscalYearVms = _repo.SelectAll();
                fiscalYearVM = fiscalYearVms.FirstOrDefault();

                FiscalYearVM fromVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == varVAT6_1ParamVM.FromPeriodName);
                FiscalYearVM toVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == varVAT6_1ParamVM.ToPeriodName);

                string[] results = VAT6_1RangeProcess(fromVm, toVm, varVAT6_1ParamVM, fiscalYearVms);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void VAT6_2Process(VAT6_2ParamVM vms)
        {
            List<FiscalYearVM> fiscalYearVms = new List<FiscalYearVM>();
            FiscalYearVM fiscalYearVM = new FiscalYearVM();
            FiscalYearRepo _repo = null;
            _repo = new FiscalYearRepo(identity);
            try
            {

                //  progressBar1.Visible = true;
                CommonDAL commonDal = new CommonDAL();
                string vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", null, connVM);

                VAT6_2ParamVM varVAT6_2ParamVM = new VAT6_2ParamVM();

                varVAT6_2ParamVM.Post1 = "Y";
                varVAT6_2ParamVM.Post2 = "Y";
                varVAT6_2ParamVM.BranchId = 0;
                varVAT6_2ParamVM.rbtnService = false;
                varVAT6_2ParamVM.rbtnWIP = false;
                varVAT6_2ParamVM.UOMTo = "";

                //varVAT6_2ParamVM.IsBureau = Program.IsBureau;
                varVAT6_2ParamVM.AutoAdjustment = vAutoAdjustment == "Y";
                varVAT6_2ParamVM.PreviewOnly = false;
                varVAT6_2ParamVM.InEnglish = "N";

                varVAT6_2ParamVM.UOM = "";
                varVAT6_2ParamVM.IsMonthly = false;
                varVAT6_2ParamVM.IsTopSheet = false;

                varVAT6_2ParamVM.UserId = identity.UserId;

                varVAT6_2ParamVM.FromPeriodName = vms.FromPeriodName;
                varVAT6_2ParamVM.ToPeriodName = vms.ToPeriodName;

                if (vms.IsChecked == true)
                {
                    varVAT6_2ParamVM.ItemNo = vms.ItemNo;
                }

                FiscalYearDAL fiscalYearDAL = new FiscalYearDAL();

                fiscalYearVms = _repo.SelectAll();
                fiscalYearVM = fiscalYearVms.FirstOrDefault();

                FiscalYearVM fromVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == varVAT6_2ParamVM.FromPeriodName);
                FiscalYearVM toVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == varVAT6_2ParamVM.ToPeriodName);

                string[] results = VAT6_2RangeProcess(fromVm, toVm, varVAT6_2ParamVM, fiscalYearVms);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void VAT6_2_1Process(VAT6_2ParamVM vms)
        {
            try
            {
                List<FiscalYearVM> fiscalYearVms = new List<FiscalYearVM>();
                FiscalYearVM fiscalYearVM = new FiscalYearVM();
                FiscalYearRepo _repo = null;
                _repo = new FiscalYearRepo(identity);

                //string message = "Do you want to Process 6.2.1 Permanently?";
                //string title = "Process 6.2.1";
                //MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                //DialogResult result = MessageBox.Show(message, title, buttons);
                //if (result == DialogResult.No)
                //{
                //    return;
                //}

                //progressBar1.Visible = true;
                CommonDAL commonDal = new CommonDAL();
                string vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", null, connVM);

                VAT6_2ParamVM paramVm = new VAT6_2ParamVM();

                paramVm.Post1 = "Y";
                paramVm.Post2 = "Y";
                paramVm.BranchId = 0;
                paramVm.rbtnService = false;
                paramVm.rbtnWIP = false;
                paramVm.UOMTo = "";

                //paramVm.IsBureau = Program.IsBureau;
                paramVm.AutoAdjustment = vAutoAdjustment == "Y";
                paramVm.PreviewOnly = false;
                paramVm.InEnglish = "N";

                paramVm.UOM = "";
                paramVm.IsMonthly = false;
                paramVm.IsTopSheet = false;

                //paramVm.UserId = Program.CurrentUserID;
                paramVm.UserId = identity.UserId;

                //paramVm.FromPeriodName = cmbFromMonth.Text;
                //paramVm.ToPeriodName = cmbToMonth.Text;

                paramVm.FromPeriodName = vms.FromPeriodName;
                paramVm.ToPeriodName = vms.ToPeriodName;

                //if (!string.IsNullOrEmpty(ItemNo))
                //{
                //    paramVm.ItemNo = ItemNo;
                //}
                fiscalYearVms = _repo.SelectAll();

                if (vms.IsChecked != true)
                {
                    // PeriodDetails(FromPeriodName.Value.ToString("MMyyyy"));

                    FiscalYearVM fiscalYear = fiscalYearVms.FirstOrDefault(x => x.PeriodID == Convert.ToDateTime(vms.PeriodMonth).ToString("MMyyyy"));
                    paramVm.FromPeriodName = fiscalYear.PeriodName;
                    paramVm.ToPeriodName = fiscalYear.PeriodName;
                }
                else
                {
                    paramVm.ItemNo = vms.ItemNo;

                    vms.StartDate = Convert.ToDateTime(vms.StartDate).ToString("dd-MMM-yyyy");
                    vms.EndDate = Convert.ToDateTime(vms.EndDate).ToString("dd-MMM-yyyy");
                    string FromPeriodId = Convert.ToDateTime(vms.StartDate).ToString("MMyyyy");
                    string ToPeriodId = Convert.ToDateTime(vms.EndDate).ToString("MMyyyy");
                    FiscalYearVM from = fiscalYearVms.FirstOrDefault(x => x.PeriodID == FromPeriodId);
                    FiscalYearVM to = fiscalYearVms.FirstOrDefault(x => x.PeriodID == ToPeriodId);

                    paramVm.FromPeriodName = from.PeriodName;
                    paramVm.ToPeriodName = to.PeriodName;
                }

                //VAT6_2ParamVM argument = (VAT6_2ParamVM)e.Argument;
                //fiscalYearVM = fiscalYearVms.FirstOrDefault();

                FiscalYearVM fromVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == paramVm.FromPeriodName);
                FiscalYearVM toVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == paramVm.ToPeriodName);

                string[] results = VAT6_2_1RangeProcess(fromVm, toVm, paramVm, fiscalYearVms);

                //e.Result = results;
                //  bgwVAT6_2_1Process.RunWorkerAsync(paramVm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BothSaveProcess(VAT6_1ParamVM vm)
        {
            List<FiscalYearVM> fiscalYearVms = new List<FiscalYearVM>();
            FiscalYearVM fiscalYearVM = new FiscalYearVM();
            FiscalYearRepo _repo = null;
            _repo = new FiscalYearRepo(identity);

            try
            {
                CommonDAL commonDal = new CommonDAL();
                string vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", null, connVM);

                VAT6_2ParamVM varVAT6_2ParamVM = new VAT6_2ParamVM();



                varVAT6_2ParamVM.Post1 = "Y";
                varVAT6_2ParamVM.Post2 = "Y";
                varVAT6_2ParamVM.BranchId = 0;
                varVAT6_2ParamVM.rbtnService = false;
                varVAT6_2ParamVM.rbtnWIP = false;
                varVAT6_2ParamVM.UOMTo = "";


                //varVAT6_2ParamVM.IsBureau = Program.IsBureau;
                varVAT6_2ParamVM.AutoAdjustment = vAutoAdjustment == "Y";
                varVAT6_2ParamVM.PreviewOnly = false;
                varVAT6_2ParamVM.InEnglish = "N";

                varVAT6_2ParamVM.UOM = "";
                varVAT6_2ParamVM.IsMonthly = false;
                varVAT6_2ParamVM.IsTopSheet = false;

                varVAT6_2ParamVM.UserId = identity.UserId;

                varVAT6_2ParamVM.FromPeriodName = vm.FromPeriodName;
                varVAT6_2ParamVM.ToPeriodName = vm.ToPeriodName;

                if (vm.IsChecked == true)
                {
                    varVAT6_2ParamVM.ItemNo = vm.ItemNo;
                }


                VAT6_1ParamVM varVAT6_1ParamVM = new VAT6_1ParamVM();

                varVAT6_1ParamVM.Post1 = "Y";
                varVAT6_1ParamVM.Post2 = "Y";
                varVAT6_1ParamVM.BranchId = 0;
                varVAT6_1ParamVM.PreviewOnly = false;
                varVAT6_1ParamVM.InEnglish = "N";
                varVAT6_1ParamVM.UOMConversion = 1;
                varVAT6_1ParamVM.UOM = "";
                varVAT6_1ParamVM.UOMTo = "";
                varVAT6_1ParamVM.UserName = Convert.ToString(Session["LogInUserName"]);
                varVAT6_1ParamVM.ReportName = "";
                varVAT6_1ParamVM.Opening = false;
                varVAT6_1ParamVM.OpeningFromProduct = false;

                varVAT6_1ParamVM.IsMonthly = false;
                varVAT6_1ParamVM.IsTopSheet = false;
                varVAT6_1ParamVM.IsGroupTopSheet = false;
                varVAT6_1ParamVM.Is6_1Permanent = true;

                varVAT6_1ParamVM.UserId = identity.UserId;

                varVAT6_1ParamVM.FromPeriodName = vm.FromPeriodName;
                varVAT6_1ParamVM.ToPeriodName = vm.ToPeriodName;

                if (vm.IsChecked == true)
                {
                    varVAT6_1ParamVM.ItemNo = vm.ItemNo;
                }

                List<Object> arguments = new List<object> { varVAT6_1ParamVM, varVAT6_2ParamVM };

                FiscalYearDAL fiscalYearDAL = new FiscalYearDAL();

                fiscalYearVms = _repo.SelectAll();
                fiscalYearVM = fiscalYearVms.FirstOrDefault();

                FiscalYearVM fromVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == varVAT6_2ParamVM.FromPeriodName);
                FiscalYearVM toVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == varVAT6_2ParamVM.ToPeriodName);

                string[] res1 = VAT6_1RangeProcess(fromVm, toVm, varVAT6_1ParamVM, fiscalYearVms);
                string[] res2 = VAT6_2RangeProcess(fromVm, toVm, varVAT6_2ParamVM, fiscalYearVms);

                //progressBar1.Visible = true;
                // bgwBothProcess.RunWorkerAsync(arguments);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void VAT6_1Delete(VAT6_1ParamVM vm)
        {
            ProductRepo repo = null;
            repo = new ProductRepo(identity);
            ResultVM rVM = new ResultVM();

            string itemNo = vm.ItemNo;
            rVM = repo.Delete6_1Permanent(itemNo);
            rVM = repo.Delete6_1Permanent_Branch(itemNo);
        }

        private void VAT6_2Delete(VAT6_1ParamVM vm)
        {
            ProductRepo repo = null;
            repo = new ProductRepo(identity);
            ResultVM rVM = new ResultVM();

            string itemNo = vm.ItemNo;
            rVM = repo.Delete6_2Permanent(itemNo);
            rVM = repo.Delete6_2Permanent_Branch(itemNo);
        }

        private void VAT6_2_1Delete(VAT6_1ParamVM vm)
        {
            ProductRepo repo = null;
            repo = new ProductRepo(identity);
            ResultVM rVM = new ResultVM();
            string itemNo = vm.ItemNo;

            rVM = repo.Delete6_2_1Permanent(itemNo);
            rVM = repo.Delete6_2_1Permanent_Branch(itemNo);
        }

        private void BothDeleteProcess(VAT6_1ParamVM vm)
        {
            ProductRepo repo = null;
            repo = new ProductRepo(identity);
            ResultVM rVM = new ResultVM();

            string itemNo = vm.ItemNo;

            rVM = repo.Delete6_2Permanent(itemNo);
            rVM = repo.Delete6_2Permanent_Branch(itemNo);

            rVM = repo.Delete6_1Permanent(itemNo);
            rVM = repo.Delete6_1Permanent_Branch(itemNo);


        }

        private string[] VAT6_1RangeProcess(FiscalYearVM fromVm, FiscalYearVM toVm, VAT6_1ParamVM varVAT6_1ParamVM, List<FiscalYearVM> fiscalYearVms)
        {

            IssueDAL issueDal = new IssueDAL();

            IssueHeaderRepo _repo = null;
            _repo = new IssueHeaderRepo(identity);
            string[] results = new string[5];

            try
            {
                if (fromVm == null || toVm == null)
                {
                    throw new Exception("Select Fiscal Range Not Found");
                }

                List<FiscalYearVM> filteredYears = fiscalYearVms.Where(x =>
                    Convert.ToDateTime(x.PeriodStart) >= Convert.ToDateTime(fromVm.PeriodStart) &&
                    Convert.ToDateTime(x.PeriodStart) <= Convert.ToDateTime(toVm.PeriodStart)).ToList();

                ValidateFiscalPeriod(fromVm.PeriodStart, toVm.PeriodStart);


                foreach (FiscalYearVM filteredYear in filteredYears)
                {
                    varVAT6_1ParamVM.StartDate = filteredYear.PeriodStart.ToDateString();
                    varVAT6_1ParamVM.EndDate = filteredYear.PeriodEnd.ToDateString();

                    //results = issueDal.SaveVAT6_1_Permanent(varVAT6_1ParamVM, null, null, connVM);
                    //results = issueDal.SaveVAT6_1_Permanent_Branch(varVAT6_1ParamVM, null, null, connVM);

                    results = _repo.SaveVAT6_1_Permanent(varVAT6_1ParamVM, null, null, connVM);
                    results = _repo.SaveVAT6_1_Permanent_Branch(varVAT6_1ParamVM, null, null, connVM);

                }

                return results;

            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string[] VAT6_2RangeProcess(FiscalYearVM fromVm, FiscalYearVM toVm, VAT6_2ParamVM varVAT6_2ParamVM, List<FiscalYearVM> fiscalYearVms)
        {
            IssueDAL issueDal = new IssueDAL();
            IssueHeaderRepo _repo = null;
            _repo = new IssueHeaderRepo(identity);

            if (fromVm == null || toVm == null)
            {
                throw new Exception("Select Fiscal Range Not Found");
            }

            List<FiscalYearVM> filteredYears = fiscalYearVms.Where(x =>
                Convert.ToDateTime(x.PeriodStart) >= Convert.ToDateTime(fromVm.PeriodStart) &&
                Convert.ToDateTime(x.PeriodStart) <= Convert.ToDateTime(toVm.PeriodStart)).ToList();

            string[] results = new string[5];

            ValidateFiscalPeriod(fromVm.PeriodStart, toVm.PeriodStart);

            foreach (FiscalYearVM filteredYear in filteredYears)
            {
                varVAT6_2ParamVM.StartDate = filteredYear.PeriodStart.ToDateString();
                varVAT6_2ParamVM.EndDate = filteredYear.PeriodEnd.ToDateString();

                //results = issueDal.SaveVAT6_2_Permanent(varVAT6_2ParamVM, null, null, connVM);
                //results = issueDal.SaveVAT6_2_Permanent_Branch(varVAT6_2ParamVM, null, null, connVM);
                results = _repo.SaveVAT6_2_Permanent(varVAT6_2ParamVM, null, null, connVM);
                results = _repo.SaveVAT6_2_Permanent_Branch(varVAT6_2ParamVM, null, null, connVM);
            }

            return results;
        }

        private string[] VAT6_2_1RangeProcess(FiscalYearVM fromVm, FiscalYearVM toVm, VAT6_2ParamVM varVAT6_2ParamVM, List<FiscalYearVM> fiscalYearVms)
        {
            IssueDAL issueDal = new IssueDAL();
            IssueHeaderRepo _repo = null;
            _repo = new IssueHeaderRepo(identity);

            if (fromVm == null || toVm == null)
            {
                throw new Exception("Select Fiscal Range Not Found");
            }

            List<FiscalYearVM> filteredYears = fiscalYearVms.Where(x =>
                Convert.ToDateTime(x.PeriodStart) >= Convert.ToDateTime(fromVm.PeriodStart) &&
                Convert.ToDateTime(x.PeriodStart) <= Convert.ToDateTime(toVm.PeriodStart)).ToList();

            string[] results = new string[5];

            ValidateFiscalPeriod(fromVm.PeriodStart, toVm.PeriodStart);

            foreach (FiscalYearVM filteredYear in filteredYears)
            {
                varVAT6_2ParamVM.StartDate = filteredYear.PeriodStart.ToDateString();
                varVAT6_2ParamVM.EndDate = filteredYear.PeriodEnd.ToDateString();

                results = _repo.SaveVAT6_2_1_Permanent(varVAT6_2ParamVM, null, null, connVM);
                //results = issueDal.SaveVAT6_2_1_Permanent_Branch(varVAT6_2ParamVM, null, null, connVM);

            }

            foreach (FiscalYearVM filteredYear in filteredYears)
            {
                varVAT6_2ParamVM.StartDate = filteredYear.PeriodStart.ToDateString();
                varVAT6_2ParamVM.EndDate = filteredYear.PeriodEnd.ToDateString();
                results = _repo.SaveVAT6_2_1_Permanent_Branch(varVAT6_2ParamVM);

            }

            return results;
        }

        private void ValidateFiscalPeriod(string periodStart, string periodEnd)
        {
            try
            {
                CommonDAL commonDal = new CommonDAL();
                DataTable dtResult = commonDal.FiscalYearLock(periodStart, periodEnd);

                if (dtResult.Rows.Count > 0)
                {
                    List<ErrorMessage> errorMessages = dtResult.AsEnumerable().Select(row => new ErrorMessage() { ColumnName = row.Field<string>("PeriodName"), Message = "Period is Locked" }).ToList();

                    //FormErrorMessage.ShowDetails(errorMessages);

                    throw new Exception("Please Unlock the Periods");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataSet DataLoad(string VATType)
        {
            ResultVM rvm = new ResultVM();
            DataSet dataSet = new DataSet();
            DataSet ds = null;

            ProductRepo _repo = null;
            _repo = new ProductRepo(identity, Session);
            ProductDAL dal = new ProductDAL();
            try
            {

                //DataTable dt = _repo.SelectNegInventoryData(VATType);
                ds = _repo.SelectNegInventoryData(VATType);


            }
            catch (Exception)
            {


            }

            finally { }

            return ds;
        }

        #region Toll 6.1 and Toll 6.2

        [Authorize]
        [HttpGet]
        public ActionResult PrintToll6_1(string itemNo, string invoiceDate)
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
            if (itemNo != null)
            {
                vm.ItemNo = itemNo;
                vm.StartDate = invoiceDate;
                vm.EndDate = invoiceDate;
                vm.InEnglish = "Y";
            }
            vm.FontSize = 8;
            return PartialView("_printToll6_1", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportToll6_1(VAT6_1ParamVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();
            CommonRepo commonRepo = new CommonRepo();
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                NBRReports _reportClass = new NBRReports();

                #region Condition Fields

                if (vm.PreviewOnly == true)
                {
                    vm.Post1 = "y";
                    vm.Post2 = "N";
                }
                else
                {
                    vm.Post1 = "Y";
                    vm.Post2 = "Y";
                }

                if (vm.ItemNo == null)
                {
                    vm.ItemNo = "";
                }
                if (vm.StartDate == null)
                {
                    vm.StartDate = "";
                }
                else
                {
                    vm.StartDate = Convert.ToDateTime(vm.StartDate).ToString("yyyy-MMM-dd");
                }
                if (vm.EndDate == null)
                {
                    vm.EndDate = "";
                }
                else
                {
                    vm.EndDate = Convert.ToDateTime(vm.EndDate).ToString("yyyy-MMM-dd");
                }

                #endregion

                vm.UserName = Session["LogInUserName"].ToString();
                vm.UserId = identity.UserId;

                reportDocument = _reportClass.ReportToll6_1(vm, connVM);

                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }
                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }

            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message;
                return Redirect("/Vms/NBRReport");
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


        [Authorize]
        [HttpGet]
        public ActionResult PrintToll6_2(string itemNo, string invoiceDate)
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
            if (itemNo != null)
            {
                vm.ItemNo = itemNo;
                vm.StartDate = invoiceDate;
                vm.EndDate = invoiceDate;
                vm.InEnglish = "Y";
            }
            vm.FontSize = 8;
            return PartialView("_printToll6_2", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportToll6_2(VAT6_1ParamVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();
            CommonRepo commonRepo = new CommonRepo();
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                NBRReports _reportClass = new NBRReports();

                #region Condition Fields

                if (vm.PreviewOnly == true)
                {
                    vm.Post1 = "y";
                    vm.Post2 = "N";
                }
                else
                {
                    vm.Post1 = "Y";
                    vm.Post2 = "Y";
                }

                if (vm.ItemNo == null)
                {
                    vm.ItemNo = "";
                }
                if (vm.StartDate == null)
                {
                    vm.StartDate = "";
                }
                else
                {
                    vm.StartDate = Convert.ToDateTime(vm.StartDate).ToString("yyyy-MMM-dd");
                }
                if (vm.EndDate == null)
                {
                    vm.EndDate = "";
                }
                else
                {
                    vm.EndDate = Convert.ToDateTime(vm.EndDate).ToString("yyyy-MMM-dd");
                }

                #endregion

                vm.UserName = Session["LogInUserName"].ToString();
                vm.UserId = identity.UserId;

                reportDocument = _reportClass.ReportToll6_2(vm, false, connVM);
                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }
                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }

            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message;
                return Redirect("/Vms/NBRReport");
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


        #endregion

        [Authorize]
        public ActionResult Post(string PeriodName)
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

            ResultVM rVM = new ResultVM();
            string[] result = new string[6];
            try
            {

                VATReturnHeaderVM varVATReturnHeaderVM = new VATReturnHeaderVM();
                UserInformationRepo _userinforepo = new UserInformationRepo(identity);
                var userinfo = _userinforepo.SelectAll(Convert.ToInt32(identity.UserId)).FirstOrDefault();

                varVATReturnHeaderVM.SignatoryName = userinfo.FullName;
                varVATReturnHeaderVM.SignatoryDesig = userinfo.Designation;
                varVATReturnHeaderVM.Email = userinfo.Email;
                varVATReturnHeaderVM.Mobile = userinfo.Mobile;
                varVATReturnHeaderVM.NationalID = userinfo.NationalId;

                VATReturnVM vm = new VATReturnVM();
                vm.PeriodName = PeriodName;
                vm.varVATReturnHeaderVM = varVATReturnHeaderVM;
                rVM = _NBRReportRepo.Post(vm);

            }
            catch (Exception)
            {


            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        //[HttpPost]
        public ActionResult Report_VAT6_3DeliveryChallan(ReportParamVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {


                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);


                SaleReport _reportClass = new SaleReport();

                if (string.IsNullOrWhiteSpace(vm.SalesInvoiceNo))
                {
                    vm.SalesInvoiceNo = vm.InvoiceNo;
                }

                bool IsBlank = false;

                string Blank = _CommonRepo.settings("Sale", "IsBlank");

                //IsBlank = Blank == "Y";


                //reportDocument = _reportClass.SaleDeliveryChallanReportNew("'" + vm.SalesInvoiceNo + "'", connVM);
                reportDocument = _reportClass.SaleDeliveryChallanReportNew(vm.SalesInvoiceNo, connVM);



                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/NBRReport");
                }



                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);


                return new FileStreamResult(stream, "application/pdf");



            }
            catch (Exception ex)
            {
                FileLogger.Log("Sale", "Report_VAT6_3", ex.ToString());

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

        private List<BranchProfileVM> getBranchList()
        {

            #region BranchList

            int userId = Convert.ToInt32(identity.UserId);
            var list = new SymRepository.VMS.BranchRepo(identity).UserDropDownBranchProfile(userId);

            var listBranch = new SymRepository.VMS.BranchRepo(identity).SelectAll();

            if (list.Count() == listBranch.Count())
            {
                list.Add(new BranchProfileVM() { BranchID = -1, BranchName = "All" });
            }

            return list;

            #endregion

        }

        [Authorize]
        [HttpGet]
        public ActionResult Process6_1BigDataProcess(VAT6_1ParamVM vm)
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
            string todate = DateTime.Now.ToString("MMMM-yyyy");
            vm.PeriodMonth = todate;

            // Vat16ViewModel vm = new Vat16ViewModel();

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult _Process6_1BigDataProcess(VAT6_1ParamVM vm)
        {

            try
            {
                IssueHeaderRepo _repo = null;
                _repo = new IssueHeaderRepo(identity);

                ProductRepo repo = null;
                repo = new ProductRepo(identity);

                CommonRepo _Crepo = null;
                _Crepo = new CommonRepo(identity);

                FiscalYearVM fiscalYearVM = new FiscalYearVM();

                ResultVM rVM = new ResultVM();
                // AVGPriceVm priceVm = new AVGPriceVm();

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
                if (vm.Flag.ToLower() == "process" || vm.Flag.ToLower() == "branchprocess" || vm.Flag.ToLower() == "processday" || vm.Flag.ToLower() == "branchdayprocess")
                {
                    CommonDAL commonDal = new CommonDAL();
                    string vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", null);

                    ////VAT6_1ParamVM varVAT6_1ParamVM = new VAT6_1ParamVM();
                    if (vm.IsChecked != true)
                    {
                        // PeriodDetails(FromPeriodName.Value.ToString("MMyyyy"));



                        string[] cFields = { "PeriodId" };
                        string[] cValues = { Convert.ToDateTime(vm.PeriodMonth).ToString("MMyyyy") };

                        fiscalYearVM = new FiscalYearRepo(identity, Session).SelectAll(0, cFields, cValues).FirstOrDefault();

                        vm.StartDate = Convert.ToDateTime(fiscalYearVM.PeriodStart).ToString("dd-MMM-yyyy");
                        vm.EndDate = Convert.ToDateTime(fiscalYearVM.PeriodEnd).ToString("dd-MMM-yyyy");
                    }

                    ////vm.StartDate = vm.StartDate;
                    ////vm.EndDate = vm.EndDate;

                    vm.Post1 = "Y";
                    vm.Post2 = "Y";
                    vm.BranchId = 0;
                    vm.PreviewOnly = false;
                    vm.InEnglish = "N";
                    vm.UOMConversion = 1;
                    vm.UOM = "";
                    vm.UOMTo = "";
                    vm.UserName = Convert.ToString(Session["LogInUserName"]);
                    vm.ReportName = "";
                    vm.Opening = false;
                    vm.OpeningFromProduct = false;

                    vm.IsMonthly = false;
                    vm.IsTopSheet = false;
                    vm.IsGroupTopSheet = false;
                    vm.Is6_1Permanent = true;

                    vm.UserId = identity.UserId;

                    string[] results = new string[5];

                    if (vm.Flag.ToLower() == "process")
                    {
                        results = _repo.SaveVAT6_1_Permanent_Stored(vm, null, null, connVM);
                    }

                    if (vm.Flag.ToLower() == "branchprocess")
                    {
                        results = _repo.SaveVAT6_1_Permanent_Stored_Branch(vm, null, null, connVM);

                    }

                    if (vm.Flag.ToLower() == "processday")
                    {
                        results = _repo.SaveVAT6_1_Permanent_DayWise(vm, null, null, connVM);
                    }

                    if (vm.Flag.ToLower() == "branchdayprocess")
                    {
                        results = _repo.SaveVAT6_1_Permanent_DayWise_Branch(vm, null, null, connVM);
                    }

                    //string[] results = _repo.SaveVAT6_1_Permanent(vm, null, null, connVM);
                    //results = _repo.SaveVAT6_1_Permanent_Branch(vm, null, null, connVM);
                    ////rVM = _repo.SaveVAT6_1_Permanent(vm);
                    //rVM = _repo.SaveVAT6_1_Permanent_Branch(vm);

                    _Crepo.settingsUpdateMaster("DayEnd", "DayEndProcess", "Y");

                    rVM.Message = results[1].ToString();
                    rVM.Status = results[0].ToString();

                    Session["result"] = "Success" + "~" + "Data Successfully Saved Permanently(6.1)!";
                    return View("Process6_1BigDataProcess", vm);

                }
                else
                {
                    string itemNo = vm.ItemNo;
                    rVM = repo.Delete6_1Permanent(itemNo);
                    rVM = repo.Delete6_1Permanent_Branch(itemNo);
                    Session["result"] = "Success" + "~" + "Data has been deleted in VAT 6.1 permanent table!";
                    return View("Process6_1BigDataProcess", vm);

                }



                // Vat16ViewModel vm = new Vat16ViewModel();
                //Session["result"] = result[0] + "~" + result[1];
                Session["result"] = rVM.Status + "~" + rVM.Message;
                return View("Process6_1BigDataProcess", vm);
            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;

                return View("Process6_1BigDataProcess", vm);
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult Process6_2BigDataProcess(VAT6_2ParamVM vm)
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
            string todate = DateTime.Now.ToString("MMMM-yyyy");
            vm.PeriodMonth = todate;

            // Vat16ViewModel vm = new Vat16ViewModel();

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult _Process6_2BigDataProcess(VAT6_2ParamVM vm)
        {

            IssueHeaderRepo _repo = null;
            _repo = new IssueHeaderRepo(identity);

            ProductRepo repo = null;
            repo = new ProductRepo(identity);

            FiscalYearVM fiscalYearVM = new FiscalYearVM();

            ResultVM rVM = new ResultVM();

            CommonRepo commonRepo = new CommonRepo(identity, Session);
            string code = commonRepo.settings("CompanyCode", "Code");
            string[] result = new string[6];
            // AVGPriceVm priceVm = new AVGPriceVm();

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

            try
            {

                if (vm.Flag.ToLower() == "process" || vm.Flag.ToLower() == "branchprocess" || vm.Flag.ToLower() == "dayprocess" || vm.Flag.ToLower() == "branchdayprocess")
                {
                    CommonDAL commonDal = new CommonDAL();
                    string vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", null);


                    ////VAT6_1ParamVM varVAT6_1ParamVM = new VAT6_1ParamVM();
                    if (vm.IsChecked != true)
                    {
                        // PeriodDetails(FromPeriodName.Value.ToString("MMyyyy"));

                        string[] cFields = { "PeriodId" };
                        string[] cValues = { Convert.ToDateTime(vm.PeriodMonth).ToString("MMyyyy") };

                        fiscalYearVM = new FiscalYearRepo(identity, Session).SelectAll(0, cFields, cValues).FirstOrDefault();

                        vm.StartDate = Convert.ToDateTime(fiscalYearVM.PeriodStart).ToString();
                        vm.EndDate = Convert.ToDateTime(fiscalYearVM.PeriodEnd).ToString();
                    }

                    vm.StartDate = Convert.ToDateTime(vm.StartDate).ToString("dd-MMM-yyyy");
                    vm.EndDate = Convert.ToDateTime(vm.EndDate).ToString("dd-MMM-yyyy");

                    vm.Post1 = "Y";
                    vm.Post2 = "Y";
                    vm.BranchId = 0;
                    vm.rbtnService = false;
                    vm.rbtnWIP = false;
                    vm.UOMTo = "";

                    vm.IsBureau = false;
                    if (code == "246")
                    {
                        vm.IsBureau = true;
                    }

                    vm.AutoAdjustment = vAutoAdjustment == "Y";
                    vm.PreviewOnly = false;
                    vm.InEnglish = "N";

                    vm.UOM = "";
                    vm.IsMonthly = false;
                    vm.IsTopSheet = false;
                    vm.UserId = identity.UserId;

                    string[] results = new string[5];

                    if (vm.Flag.ToLower() == "process")
                    {
                        results = _repo.SaveVAT6_2_Permanent_Stored(vm, null, null, connVM);

                    }

                    if (vm.Flag.ToLower() == "branchprocess")
                    {
                        results = _repo.SaveVAT6_2_Permanent_Stored_Branch(vm, null, null, connVM);

                    }

                    if (vm.Flag.ToLower() == "dayprocess")
                    {
                        results = _repo.SaveVAT6_2_Permanent_DayWise(vm, null, null, connVM);
                    }

                    if (vm.Flag.ToLower() == "branchdayprocess")
                    {
                        results = _repo.SaveVAT6_2_Permanent_DayWise_Branch(vm, null, null, connVM);
                    }

                    //string[] results = _repo.SaveVAT6_2_Permanent(vm);
                    //results = _repo.SaveVAT6_2_Permanent_Branch(vm);
                    //string[] results = _repo.SaveVAT6_2_Permanent(vm, null, null, connVM);
                    //results = _repo.SaveVAT6_2_Permanent_Branch(vm, null, null, connVM);
                    rVM.Status = results[0].ToString();
                    rVM.Message = results[1].ToString();

                    Session["result"] = "Success" + "~" + "Data Successfully Saved Permanently(6.2)!";
                    return View("Process6_2BigDataProcess", vm);


                }
                else
                {
                    string itemNo = vm.ItemNo;

                    rVM = repo.Delete6_2Permanent(itemNo);
                    rVM = repo.Delete6_2Permanent_Branch(itemNo);
                    Session["result"] = "Success" + "~" + "Data has been deleted in VAT 6.2 permanent table!";
                    return View("Process6_2BigDataProcess", vm);

                }

            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;

                return View("Process6_2BigDataProcess", vm);
            }

        }


    }
}
