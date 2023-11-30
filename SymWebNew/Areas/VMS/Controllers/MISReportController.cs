using CrystalDecisions.CrystalReports.Engine;
//using JQueryDataTables.Models;
using SymOrdinary;
using SymphonySofttech.Reports.Report;
using SymRepository.VMS;
using SymVATWebUI.Areas.VMS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymphonySofttech.Reports.List;
using SymphonySofttech.Reports;
using CrystalDecisions.Shared;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using SymphonySofttech.Utilities;
using System.Data.SqlClient;
using SymVATWebUI.Areas.VMS.Controllers;
using VATViewModel;
using System.Configuration;
using SymVATWebUI.Filters;
using VATServer.Library;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class MISReportController : Controller
    {
        ShampanIdentity identity = null;
        CommonRepo _CommonRepo = null;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public MISReportController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _CommonRepo = new CommonRepo(identity);

                connVM.SysDatabaseName = identity.InitialCatalog;
                connVM.SysUserName = SysDBInfoVM.SysUserName;
                connVM.SysPassword = SysDBInfoVM.SysPassword;
                connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            }
            catch
            {

            }
        }
        ReportDocument reportDocument = new ReportDocument();
        MISReport _reportClass = new MISReport();


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

        [Authorize]
        [HttpGet]
        public ActionResult PrintIssue(string itemNo)
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
            ProductionMISViewModel vm = new ProductionMISViewModel();
            vm.FontSize = 8;

            #region Product Call

            //CommonRepo _repo = new CommonRepo(identity, Session);
            //ParameterVM paramVM = new ParameterVM();
            //paramVM.selectFields = new[] { "ItemNo", "ProductCode", "ProductName" };
            //paramVM.TableName = "Products";
            //paramVM.conditionFields = new[] { "ItemNo" };
            //paramVM.conditionValues = new[] { itemNo };

            ////DataTable dt = new DataTable();

            ////dt = _repo.Select(paramVM);

            ////vm.ItemNo = itemNo;
            ////if (dt != null && dt.Rows.Count > 0)
            ////{
            ////    vm.ProductName = dt.Rows[0]["ProductName"].ToString();

            ////}

            #endregion

            return PartialView("_printIssue", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult PrintReceive()
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
            ProductionMISViewModel vm = new ProductionMISViewModel();
            vm.FontSize = 8;
            return PartialView("_printReceive", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult PrintPurchase(string itemNo)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
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

            PurchaseMISViewModel vm = new PurchaseMISViewModel();
            vm.FontSize = 8;

            #region Product Call

            CommonRepo _repo = new CommonRepo(identity, Session);
            ParameterVM paramVM = new ParameterVM();
            paramVM.selectFields = new[] { "ItemNo", "ProductCode", "ProductName" };
            paramVM.TableName = "Products";
            paramVM.conditionFields = new[] { "ItemNo" };
            paramVM.conditionValues = new[] { itemNo };

            DataTable dt = new DataTable();
            if (itemNo != null)
            {
                dt = _repo.Select(paramVM);

                vm.ItemNo = itemNo;

                if (dt != null && dt.Rows.Count > 0)
                {
                    vm.ProductName = dt.Rows[0]["ProductName"].ToString();

                }
            }


            #endregion

            return PartialView("_printPurchase", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult PrintSale()
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
            SaleMISViewModel vm = new SaleMISViewModel();
            vm.FontSize = 8;
            vm.Post = true;
            return PartialView("_printSale", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportIssue(ProductionMISViewModel vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                connVM = Ordinary.StaticValueReAssign(identity, Session);

                #region Date Format

                string IssueDateFrom = "1990-01-01";
                string IssueDateTo = DateTime.Now.ToString("yyyy-MM-dd");
                //int BranchId = Convert.ToInt32(Session["BranchId"]);
                int BranchId = vm.BranchId;

                if (vm.BranchId == 0)
                {
                    BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (vm.BranchId == -1)
                {
                    BranchId = 0;
                }


                if (!string.IsNullOrWhiteSpace(vm.IssueDateFrom))
                {
                    IssueDateFrom = Convert.ToDateTime(vm.IssueDateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.IssueDateTo))
                {
                    IssueDateTo = Convert.ToDateTime(vm.IssueDateTo).ToString("yyyy-MM-dd");
                }

                #endregion
                MISReport _reportClass = new MISReport();

                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();
                if (vm.ExcelRpt && vm.ReportType == "Detail")
                {
                    //var post = vm.Post ? "Y" : "N";
                    //var waste = vm.Wastage ? "Y" : "N";

                    var post = vm.IsPost;
                    var waste = vm.IsWastage;

                    if (vm.IssueNo == null)
                    {
                        vm.IssueNo = "";
                    }
                    if (vm.IssueDateFrom == null)
                    {
                        //vm.IssueDateFrom = "1753-Jan-01 00:00:00";
                        vm.IssueDateFrom = "";
                    }
                    if (vm.IssueDateTo == null)
                    {
                        //vm.IssueDateTo = "9998-Dec-31 00:00:00";
                        vm.IssueDateTo = "";
                    }
                    if (vm.ItemNo == null)
                    {
                        vm.ItemNo = "";
                    }
                    if (vm.ProductGroup == null)
                    {
                        vm.ProductGroup = "";
                    }
                    if (vm.ProductType == null)
                    {
                        vm.ProductType = "";
                    }
                    if (vm.ProductName == null)
                    {
                        vm.ProductName = "";
                    }
                    if (vm.ReportType == null)
                    {
                        vm.ReportType = "";
                    }
                    vm = _reportClass.IssueNew(vm, connVM);

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
                }
                else
                {
                    #region Detail
                    if (vm.ReportType == "Detail" || vm.ReportType == "Summary")
                    {


                        var post = vm.IsPost;
                        var waste = vm.IsWastage;

                        if (vm.IsPost == null)
                        {
                            vm.IsPost = "";
                        }
                        if (vm.IsWastage == null)
                        {
                            vm.IsWastage = "";
                        }
                        if (vm.IssueNo == null)
                        {
                            vm.IssueNo = "";
                        }
                        if (vm.IssueDateFrom == null)
                        {
                            IssueDateFrom = "1753-Jan-01 00:00:00";
                        }
                        if (vm.IssueDateTo == null)
                        {
                            IssueDateTo = "9998-Dec-31 00:00:00";
                        }
                        if (vm.ItemNo == null)
                        {
                            vm.ItemNo = "";
                        }
                        if (vm.ProductGroup == null)
                        {
                            vm.ProductGroup = "";
                        }
                        if (vm.ProductType == null)
                        {
                            vm.ProductType = "";
                        }
                        if (vm.ProductName == null)
                        {
                            vm.ProductName = "";
                        }
                        if (vm.ReportType == null)
                        {
                            vm.ReportType = "";
                        }

                        reportDocument = _reportClass.IssueNew(vm.IssueNo, IssueDateFrom, IssueDateTo, vm.ItemNo, vm.ProductGroup,
                            vm.ProductType, "", post, waste, false, "", BranchId, vm.ProductName, vm.ReportType);

                        #region Old

                        //var ReportResult = repo.IssueNew(vm.IssueNo, IssueDateFrom, IssueDateTo, vm.ItemNo, vm.ProductGroup, vm.ProductType, "", post, waste, false);
                        //ReportResult.Tables[0].TableName = "DsIssue";

                        //RptIssue objrpt = new RptIssue();

                        //objrpt.SetDataSource(ReportResult);
                        //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + identity.Name + "'";
                        //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Raw product issue information'";
                        //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                        //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                        //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'Address2'";
                        //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'Address3'";
                        //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                        //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                        //objrpt.DataDefinition.FormulaFields["PQty"].Text = "'" + waste + "'";

                        //if (vm.ProductName == "" || vm.ProductName == null)
                        //{
                        //    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
                        //}
                        //else
                        //{
                        //    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + vm.ProductName + "'  ";
                        //}


                        //if (vm.IssueNo == "" || vm.IssueNo == null)
                        //{
                        //    objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'[All]'";
                        //}
                        //else
                        //{
                        //    objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'" + vm.IssueNo + "'  ";
                        //}

                        //if (vm.IssueDateFrom == "" || vm.IssueDateFrom == null)
                        //{
                        //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                        //}
                        //else
                        //{
                        //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + vm.IssueDateFrom + "'  ";
                        //}

                        //if (vm.IssueDateTo == "" || vm.IssueDateTo == null)
                        //{
                        //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                        //}
                        //else
                        //{
                        //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + vm.IssueDateTo + "'  ";
                        //}
                        //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'Trial'";
                        //var gr = new GenericReport<RptIssue>();
                        //var rpt = gr.RenderReportAsPDF(objrpt);
                        //objrpt.Close();
                        //return rpt;

                        #endregion

                    }
                    #endregion detail

                    #region Summary
                    //else if (vm.ReportType == "Summary")
                    //{
                    //    var post = vm.Post ? "Y" : "N";
                    //    var waste = vm.Wastage ? "Y" : "N";
                    //    var ReportResult = repo.IssueNew(vm.IssueNo, IssueDateFrom, IssueDateTo, vm.ItemNo, vm.ProductGroup, vm.ProductType, "", post, waste, false);
                    //    ReportResult.Tables[0].TableName = "DsIssue";

                    //    RptIssueSummery objrpt = new RptIssueSummery();

                    //    objrpt.SetDataSource(ReportResult);
                    //    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + identity.Name + "'";
                    //    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Raw product issue summery Information'";
                    //    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                    //    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                    //    objrpt.DataDefinition.FormulaFields["Address2"].Text = "'Address2'";
                    //    objrpt.DataDefinition.FormulaFields["Address3"].Text = "'Address3'";
                    //    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                    //    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                    //    objrpt.DataDefinition.FormulaFields["PQty"].Text = "'" + waste + "'";

                    //    if (vm.ProductName == "" || vm.ProductName == null)
                    //    {
                    //        objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
                    //    }
                    //    else
                    //    {
                    //        objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + vm.ProductName + "'  ";
                    //    }


                    //    if (vm.IssueNo == "" || vm.IssueNo == null)
                    //    {
                    //        objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'[All]'";
                    //    }
                    //    else
                    //    {
                    //        objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'" + vm.IssueNo + "'  ";
                    //    }

                    //    if (vm.IssueDateFrom == "" || vm.IssueDateFrom == null)
                    //    {
                    //        objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                    //    }
                    //    else
                    //    {
                    //        objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + vm.IssueDateFrom + "'  ";
                    //    }

                    //    if (vm.IssueDateTo == "" || vm.IssueDateTo == null)
                    //    {
                    //        objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                    //    }
                    //    else
                    //    {
                    //        objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + vm.IssueDateTo + "'  ";
                    //    }
                    //    objrpt.DataDefinition.FormulaFields["Trial"].Text = "'Trial'";
                    //    var gr = new GenericReport<RptIssueSummery>();
                    //    var rpt = gr.RenderReportAsPDF(objrpt);
                    //    objrpt.Close();
                    //    return rpt;
                    //}
                    #endregion summary

                    #region single
                    if (vm.ReportType == "Single")
                    {

                        reportDocument = _reportClass.IssueMis(vm.IssueNo, BranchId);


                        //var ReportResult = repo.IssueMis(vm.IssueNo);
                        //ReportResult.Tables[0].TableName = "DsIssueHeaders";
                        //ReportResult.Tables[1].TableName = "DsIssueDetails";

                        //RptMISIssue objrpt = new RptMISIssue();
                        //objrpt.SetDataSource(ReportResult);
                        //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'Trial'";

                        //var gr = new GenericReport<RptMISIssue>();
                        //var rpt = gr.RenderReportAsPDF(objrpt);
                        //objrpt.Close();
                        //return rpt;
                    }
                    #endregion

                    var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                    return new FileStreamResult(stream, "application/pdf");
                }


                return View();
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
        public ActionResult ReportReceive(ProductionMISViewModel vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                MISReport _reportClass = new MISReport();

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();

                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();

                #region Date Format

                string IssueDateFrom = "1990-01-01 HH:mm:ss";
                string IssueDateTo = DateTime.Now.ToString("yyyy-MM-dd 23:59:59.000");

                if (!string.IsNullOrWhiteSpace(vm.IssueDateFrom))
                {
                    IssueDateFrom = Convert.ToDateTime(vm.IssueDateFrom).ToString("yyyy-MM-dd HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(vm.IssueDateTo))
                {
                    IssueDateTo = Convert.ToDateTime(vm.IssueDateTo).ToString("yyyy-MM-dd 23:59:59.000");
                }

                #endregion

                //int BranchId = Convert.ToInt32(Session["BranchId"]);
                int BranchId = vm.BranchId;

                if (vm.BranchId == 0)
                {
                    BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (vm.BranchId == -1)
                {
                    BranchId = 0;
                }

                #region Detail
                if (vm.ReportType == "Detail" || vm.ReportType == "Summary")
                {
                    var post = vm.Post ? "Y" : "N";
                    if (vm.ReportType == "Detail")
                    {
                        vm.ReportType = "Detail";
                    }
                    else if (vm.ReportType == "Summary")
                    {
                        vm.ReportType = "Summary";
                    }
                    if (vm.IsPost == null)
                    {
                        vm.IsPost = "";
                    }
                    if (vm.IsWastage == null)
                    {
                        vm.IsWastage = "";
                    }
                    if (vm.IssueNo == null)
                    {
                        vm.IssueNo = "";
                    }
                    if (vm.IssueDateFrom == null)
                    {
                        IssueDateFrom = "";
                    }
                    if (vm.IssueDateTo == null)
                    {
                        IssueDateTo = "";
                    }
                    if (vm.ItemNo == null)
                    {
                        vm.ItemNo = "";
                    }
                    if (vm.ProductGroup == null)
                    {
                        vm.ProductGroup = "";
                    }
                    if (vm.ProductType == null)
                    {
                        vm.ProductType = "";
                    }
                    if (vm.ProductName == null)
                    {
                        vm.ProductName = "";
                    }



                    reportDocument = _reportClass.ReceiveNew(vm.IssueNo, IssueDateFrom, IssueDateTo, vm.ItemNo, vm.ProductGroup,
                        vm.ProductType, "", post, "0", BranchId, vm.ProductName, vm.ReportType);

                    #region Old

                    //var ReportResult = repo.ReceiveNew(vm.IssueNo, IssueDateFrom, IssueDateTo, vm.ItemNo, vm.ProductGroup, vm.ProductType, "", post);

                    //if (vm.ReportType == "Detail")
                    //{
                    //    ReportResult.Tables[0].TableName = "DsReceive";
                    //    objrpt = new RptReceivingReport();
                    //}
                    //else
                    //{
                    //    ReportResult.Tables[0].TableName = "DsReceive";
                    //    objrpt = new RptReceivingSummery();
                    //}

                    //objrpt.SetDataSource(ReportResult);
                    //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'CurrentUse'";
                    //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Receive Information'";
                    //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                    ////objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                    //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                    //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'Address2 '";
                    //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'Address3'";
                    //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                    //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";
                    ////objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

                    //if (string.IsNullOrWhiteSpace(vm.ProductName))
                    //{
                    //    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
                    //}
                    //else
                    //{
                    //    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + vm.ProductName + "'  ";
                    //}

                    //if (string.IsNullOrWhiteSpace(vm.IssueNo))
                    //{
                    //    objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'[All]'";
                    //}
                    //else
                    //{
                    //    objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'" + vm.IssueNo + "'  ";
                    //}

                    //if (string.IsNullOrWhiteSpace(vm.IssueDateFrom))
                    //{
                    //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                    //}
                    //else
                    //{
                    //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + vm.IssueDateFrom + "'  ";
                    //}

                    //if (string.IsNullOrWhiteSpace(vm.IssueDateTo))
                    //{
                    //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                    //}
                    //else
                    //{
                    //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + vm.IssueDateTo +
                    //                                                          "'  ";
                    //}
                    //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'Trial'";
                    ////var rpt = gr.RenderReportAsPDF(objrpt);
                    ////objrpt.Close();
                    ////return rpt;

                    #endregion

                }
                #endregion detail

                #region single
                else if (vm.ReportType == "Single")
                {
                    BranchRepo _branchrepo = new BranchRepo(identity);

                    string BranchName = "All";
                    int branchId = BranchId;
                    if (branchId != 0)
                    {
                        DataTable dtBranch = _branchrepo.SelectAllBranch(branchId.ToString(), null, null, null, null);
                        BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
                    }

                    if (vm.IssueNo == null)
                    {
                        vm.IssueNo = "";
                    }
                    var ReportMIS = repo.ReceiveMis(vm.IssueNo);
                    ReportMIS.Tables[0].TableName = "DsReceiveHeader";
                    ReportMIS.Tables[1].TableName = "DsReceiveDetails";
                    objrpt = new RptMISReceive();
                    objrpt.SetDataSource(ReportMIS);
                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'.CompanyName'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1 '";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";
                    objrpt.DataDefinition.FormulaFields["Trial"].Text = "'Trial'";
                    objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

                    var rpt = gr.RenderReportAsPDF(objrpt);
                    objrpt.Close();
                    return rpt;
                }

                #endregion

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

                //var rpt = gr.RenderReportAsPDF(objrpt);
                //objrpt.Close();
                //return rpt;
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
        public ActionResult ReportPurchase(PurchaseMISViewModel vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                MISReport _reportClass = new MISReport();


                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();

                #region Date Format

                string ReceiveDateFrom = "";
                string ReceiveDateTo = "";
                var post = vm.Post ? "Y" : "N";
                if (!string.IsNullOrWhiteSpace(vm.ReceiveDateFrom))
                {
                    ReceiveDateFrom = Convert.ToDateTime(vm.ReceiveDateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.ReceiveDateTo))
                {
                    //ReceiveDateTo = Convert.ToDateTime(vm.ReceiveDateTo).AddDays(1).ToString("yyyy-MM-dd");
                    ReceiveDateTo = Convert.ToDateTime(vm.ReceiveDateTo).ToString("yyyy-MM-dd");
                }

                #endregion

                if (!string.IsNullOrEmpty(vm.VendorId))
                {
                    int id = Convert.ToInt32(vm.VendorId);
                    var _repo = new VendorRepo(identity, Session);
                    var VendorName = _repo.SelectAll(id).FirstOrDefault();
                    vm.VendorName = VendorName.VendorName;

                }
                if (!string.IsNullOrEmpty(vm.ProductGroupId))
                {
                    int id = Convert.ToInt32(vm.ProductGroupId);
                    var _repo = new ProductCategoryRepo(identity, Session);
                    var ProductGroupName = _repo.SelectAll(id).FirstOrDefault();
                    vm.ProductGroupName = ProductGroupName.CategoryName;

                }
                //int BranchId = Convert.ToInt32(Session["BranchId"]);
                int BranchId = vm.BranchId;

                if (vm.BranchId == 0)
                {
                    BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (vm.BranchId == -1)
                {
                    BranchId = 0;
                }

                #region

                if (vm.PurchaseNo == null)
                {
                    vm.PurchaseNo = "";
                }
                if (vm.VATType == null)
                {
                    vm.VATType = "";
                }
                if (vm.VendorGroup == null)
                {
                    vm.VendorGroup = "";
                }
                if (vm.VendorId == null)
                {
                    vm.VendorId = "";
                }
                if (vm.VendorName == null)
                {
                    vm.VendorName = "";
                }
                if (vm.ProductType == null)
                {
                    vm.ProductType = "";
                }
                if (vm.ProductName == null)
                {
                    vm.ProductName = "";
                }
                if (vm.ProductGroupName == null)
                {
                    vm.ProductGroupName = "";
                }
                if (vm.ProductGroupId == null)
                {
                    vm.ProductGroupId = "";
                }
                if (vm.ReportType == "Summary By Product")
                {
                    vm.ReportType = "SummaryByProduct";
                }

                #endregion


                #region Signle
                if (vm.ReportType == "Single")
                {
                    reportDocument = _reportClass.PurchaseMis(vm.PurchaseNo, BranchId, vm.VATType, "");

                    //var ReportMIS = repo.PurchaseMis(vm.PurchaseNo);

                    //ReportMIS.Tables[0].TableName = "DsPurchaseHeader";
                    //ReportMIS.Tables[1].TableName = "DsPurchaseDetails";

                    //objrpt = new RptMISPurchase1();
                    //objrpt.SetDataSource(ReportMIS);

                    ////objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName '";
                    ////objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1 '";
                    ////objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                    ////objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                    //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + identity.CompanyName + "'";
                    //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + identity.Address1 + " '";
                    //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + identity.TelephoneNo + "'";
                    //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                    //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + vm.ReportType + "'";
                    //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'Trial'";
                }
                #endregion



                #region Detail, Summary, Summary By Product
                else if (vm.ReportType == "Detail" || vm.ReportType == "Summary" || vm.ReportType == "SummaryByProduct" || vm.ReportType == "AT")
                {
                    reportDocument = _reportClass.PurchaseNew(vm.PurchaseNo, ReceiveDateFrom, ReceiveDateTo, vm.VendorId, vm.ItemNo,
                        vm.ProductGroupId, vm.ProductType, "", post, "", vm.VendorGroup, "N", "-", "-", 0, 0, 0, false,
                     vm.ProductGroupName, BranchId, vm.VATType, vm.ReportType, "", vm.ProductName, vm.VendorName, vm.Duty);

                    #region Old

                    //var ReportResult = repo.PurchaseNew(vm.PurchaseNo, ReceiveDateFrom, ReceiveDateTo, vm.VendorName, vm.ItemNo,
                    //vm.ProductGroup, vm.ProductType, "", post, "", vm.VendorGroup, "N", "-", "-", 0, 0, 0, false, null);

                    ////var ReportResult = repo.PurchaseNew(vm.PurchaseNo, ReceiveDateFrom, ReceiveDateTo, "", "",
                    ////"", "", "", "", "", "", "N", "-", "-", 0, 0, 0, false, null);

                    //ReportResult.Tables[0].TableName = "DsPurchase";
                    //if (vm.ReportType == "Summary")
                    //{
                    //    objrpt = new RptPurchaseSummery();
                    //    objrpt.SetDataSource(ReportResult);
                    //    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + vm.ReportType + "'";
                    //}
                    //else if (vm.ReportType == "SummaryByProduct")
                    //{
                    //    objrpt = new RptPurchaseSummeryByProduct();
                    //    objrpt.SetDataSource(ReportResult);
                    //    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + vm.ReportType + "'";
                    //}
                    //else
                    //{
                    //    ReportResult.Tables[0].TableName = "DsPurchase";

                    //    if (vm.Duty == false)
                    //    {
                    //        objrpt = new RptPurchaseTransaction();
                    //    }
                    //    else if (vm.Duty == true)
                    //    {
                    //        objrpt = new RptPurchaseTransaction_Duty();
                    //    }
                    //    objrpt.SetDataSource(ReportResult);
                    //    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'" + vm.ReportType + "'";
                    //}


                    //#region Formulla
                    //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + identity.CompanyName + "'";
                    //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + identity.Address1 + " '";
                    //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + identity.TelephoneNo + "'";
                    //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                    //if (vm.ProductName == null)
                    //{ objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'"; }
                    //else
                    //{ objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + vm.ProductName + "'  "; }

                    //if (vm.VendorName == null)
                    //{ objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'[All]'"; }
                    //else
                    //{ objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'" + vm.VendorName + "'  "; }

                    //if (vm.PurchaseNo == null)
                    //{ objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'[All]'"; }
                    //else
                    //{ objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'" + vm.PurchaseNo + "'  "; }

                    //if (vm.ReceiveDateFrom == null)
                    //{ 
                    //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'"; 
                    //}
                    //else
                    //{
                    //    DateTime dtfrom = Convert.ToDateTime(vm.ReceiveDateFrom);
                    //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + dtfrom.ToString("dd/MMM/yyyy") + "'  "; 
                    //}

                    //if (vm.ReceiveDateTo == null)
                    //{ objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'"; }
                    //else
                    //{
                    //    DateTime dtTo = Convert.ToDateTime(vm.ReceiveDateTo);
                    //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + dtTo.ToString("dd/MMM/yyyy") + "'  "; 
                    //}

                    //#endregion Formulla

                    //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'CurrentUser'";
                    //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                    //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                    //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                    //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                    #endregion


                }
                #endregion

                #region Monthly
                else if (vm.ReportType == "Monthly")
                {
                    var ReportDataTable = repo.MonthlyPurchases(vm.PurchaseNo, ReceiveDateFrom, ReceiveDateTo, vm.VendorName, vm.ItemNo,
                    vm.ProductGroup, vm.ProductType, "", post, "", vm.VendorGroup, "N", "-", "-", 0, 0, 0);
                    DataView dtview = new DataView(ReportDataTable);
                    dtview.Sort = "YearSerial ASC, MonthSerial ASC";
                    DataTable dtsorted = dtview.ToTable();
                }

                #endregion

                //var rpt = gr.RenderReportAsPDF(objrpt);
                //objrpt.Close();
                //return rpt;

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");





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
        public ActionResult ReportSale(SaleMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                DataSet ReportMIS;
                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                var post = vm.Post ? "Y" : "N";
                var discount = vm.Discount ? "Y" : "N";
                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();



                //int BranchId = Convert.ToInt32(Session["BranchId"]);
                int BranchId = vm.BranchId;

                if (vm.BranchId == 0)
                {
                    BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (vm.BranchId == -1)
                {
                    BranchId = 0;
                }
                //formula
                BranchRepo _branchrepo = new BranchRepo(identity);

                string BranchName = "All";
                int branchId = BranchId;
                if (branchId != 0)
                {
                    DataTable dtBranch = _branchrepo.SelectAllBranch(branchId.ToString(), null, null, null, null);
                    //DataTable dtBranch = _branchrepo.SelectAllBranch(branchId.ToString(), null, null, null, null);
                    BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
                }
                //end formula
                if (string.IsNullOrWhiteSpace(vm.InvoiceNo))
                {
                    vm.InvoiceNo = "";
                }
                if (string.IsNullOrWhiteSpace(vm.CustomerName))
                {
                    vm.CustomerName = "";
                }
                if (string.IsNullOrWhiteSpace(vm.ItemNo))
                {
                    vm.ItemNo = "";
                }
                if (string.IsNullOrWhiteSpace(vm.ProductType))
                {
                    vm.ProductType = "";
                }
                if (string.IsNullOrWhiteSpace(vm.CustomerGroup))
                {
                    vm.CustomerGroup = "";
                }

                if (vm.VatType == null || vm.VatType.ToLower() == "select")
                {
                    vm.VatType = "";
                }

                if (string.IsNullOrWhiteSpace(vm.CustomerId))
                {
                    vm.CustomerId = "";
                }

                #region Date Format

                string DateFrom = "";
                string DateTo = "";

                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    DateFrom = Convert.ToDateTime(vm.DateFrom).ToString("yyyy-MM-dd HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    //DateTo = Convert.ToDateTime(vm.DateTo).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                    DateTo = Convert.ToDateTime(vm.DateTo).ToString("yyyy-MM-dd 23:59:59");
                }

                #endregion
                #region Signle
                if (vm.ReportType == "Single")
                {

                    var IsBureau = false;
                    if (IsBureau)
                    {
                        ReportMIS = repo.BureauSaleMis(vm.InvoiceNo);
                    }
                    else
                    {
                        ReportMIS = repo.SaleMis(vm.InvoiceNo);
                    }
                    ReportMIS.Tables[0].TableName = "DsSaleHeader";
                    ReportMIS.Tables[1].TableName = "DsSalesDetails";
                    ReportMIS.Tables[2].TableName = "DsSalesHeadersExport";
                    objrpt = new RptMISSales1();
                    objrpt.SetDataSource(ReportMIS);

                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + identity.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + identity.Address1 + " '";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + identity.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";
                    objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";
                    ////objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName '";
                    ////objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1 '";
                    ////objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                    ////objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";
                    //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'Trial'";
                }
                #endregion

                #region CreditNoteDetils,DebitNoteDetails

                else if (vm.ReportType.ToLower() == "creditnotedetails")
                {
                    string vTransactionType = "Credit";

                    ReportMIS = repo.VATCreditNoteMis(vm.InvoiceNo, DateFrom, DateTo, vm.CustomerId, vm.ItemNo, "", vm.ProductType, vTransactionType, post, discount, false, vm.CustomerGroup, false,
                                         vm.CustomerGroup, "0", BranchId, vm.VatType, vm.OrderBy);

                    ReportMIS.Tables[0].TableName = "DsVAT11";
                    objrpt = new RptVATCreditNoteInformation();

                    objrpt.SetDataSource(ReportMIS);
                    ////string FormNumeric = _cDal.settingsDesktop("DecimalPlace", "Quantity6_3", settingsDt, connVM);

                    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";
                    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Credit Note Information'";
                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyLegalName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                    ////objrpt.DataDefinition.FormulaFields["Quantity6_3"].Text = "'" + FormNumeric + "'";
                    ////objrpt.DataDefinition.FormulaFields["Amount6_3"].Text = "'" + FormNumeric + "'";
                    objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

                }
                else if (vm.ReportType.ToLower() == "debitnotedetails")
                {
                    string vTransactionType = "Debit";
                    ReportMIS = repo.VATCreditNoteMis(vm.InvoiceNo, DateFrom, DateTo, vm.CustomerId, vm.ItemNo, "", vm.ProductType, vTransactionType, post, discount, false, vm.CustomerGroup, false,
                                       vm.CustomerGroup, "0", BranchId, vm.VatType, vm.OrderBy);

                    ReportMIS.Tables[0].TableName = "DsVAT11";
                    objrpt = new RptVATDebitNoteInformation();

                    objrpt.SetDataSource(ReportMIS);

                    objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + OrdinaryVATDesktop.CurrentUser + "'";

                    objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Debit Note Information'";

                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyLegalName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                    //objrpt.DataDefinition.FormulaFields["Quantity6_3"].Text = "'" + FormNumeric + "'";
                    //objrpt.DataDefinition.FormulaFields["Amount6_3"].Text = "'" + FormNumeric + "'";
                    objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";



                }



                #endregion
                #region detail, summary, summary by product
                else
                {
                    //DataSet ReportMIS;

                    ReportMIS = repo.SaleNew(vm.InvoiceNo, DateFrom, DateTo, vm.CustomerId, vm.ItemNo, "", vm.ProductType, "", post, discount, false, vm.CustomerGroup,
                        false, "", "0", BranchId, vm.VatType, vm.OrderBy, vm.ReportType);

                    if (vm.ReportType.ToLower() == "detail")
                    {
                        ReportMIS.Tables[0].TableName = "DsSale";
                        //objrpt = new RptMISSales1();
                        objrpt = new RptSalesTransaction();

                    }

                    else if (vm.ReportType.ToLower() == "summary") //summary
                    {
                        ReportMIS.Tables[0].TableName = "DsSale";

                        objrpt = new RptSalesSummery();


                    }
                    else if (vm.ReportType.ToLower() == "summaryqtyonly") //summary
                    {

                        ReportMIS.Tables[0].TableName = "DsSale";
                        ReportMIS.Tables[1].TableName = "DsSalePCategory";
                        ReportMIS.Tables[2].TableName = "DsSaleCustomerPCategory";

                        //objrpt.Load(Program.ReportAppPath + @"\RptSalessummaryQuantityOnly.rpt");
                        objrpt = new RptSalesSummeryQuantityOnly();

                    }
                    else if (vm.ReportType.ToLower() == "summarybyproduct") //Summery
                    {

                        ReportMIS.Tables[0].TableName = "DsSale";

                        objrpt = new RptSalesSummeryByProduct();

                    }

                    objrpt.SetDataSource(ReportMIS);

                    //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + identity.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                    objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + " '";
                    objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                    objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";
                    //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'Trial'";
                    objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

                    if (string.IsNullOrWhiteSpace(vm.InvoiceNo))
                    {
                        objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["PInvoiceNo"].Text = "'" + vm.InvoiceNo + "'  ";
                    }

                    if (string.IsNullOrWhiteSpace(DateFrom))
                    {
                        objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                    }
                    else
                    {
                        DateTime dtfrom = Convert.ToDateTime(DateFrom);
                        objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + dtfrom.ToString("dd/MMM/yyyy HH:mm") + "'  ";
                    }

                    if (string.IsNullOrWhiteSpace(DateTo))
                    {
                        objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                    }
                    else
                    {
                        DateTime dtTo = Convert.ToDateTime(DateTo);

                        objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + dtTo.ToString("dd/MMM/yyyy HH:mm") + "'  ";
                    }

                    if (string.IsNullOrWhiteSpace(vm.CustomerName))
                    {
                        objrpt.DataDefinition.FormulaFields["PCustomer"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["PCustomer"].Text = "'" + vm.CustomerName + "'  ";
                    }

                    if (string.IsNullOrWhiteSpace(vm.ProductType))
                    {
                        objrpt.DataDefinition.FormulaFields["PType"].Text = "'[All]'";
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["PType"].Text = "'" + vm.ProductType + "'  ";
                    }

                }


                #endregion


                //if (vm.ReportType.ToLower() == "monthly")
                //{
                //    DataTable ReportDataTable = new DataTable();
                //    var IsBureau = false;
                //    if (IsBureau)
                //    {
                //        ReportDataTable = repo.BureauMonthlySales(null, vm.DateFrom, vm.DateTo, vm.CustomerName, vm.ItemNo, "", vm.ProductType, "", post, discount,
                //            false, vm.CustomerGroup,"1",0,null);
                //    }
                //    else
                //    {
                //        ReportDataTable = repo.MonthlySales(null, vm.DateFrom, vm.DateTo, vm.CustomerName, vm.ItemNo, "", vm.ProductType, "", post, discount,
                //            false, vm.CustomerGroup, "1", 0,"", null);
                //    }
                //}

                #region detail, summary, summary by product
                //else
                //{
                //    var ReportResult = repo.SaleNew(vm.InvoiceNo, SalesFromDate, SalesToDate, txtCustomerId.Text.Trim(),
                //       txtItemNo.Text.Trim(), txtPGroupId.Text.Trim(), cmbTypeText, vTransactionType, cmbPostText,
                //       DiscountOnly, bPromotional, txtCustomerGroupID.Text.Trim(), chkCategoryLike.Checked, txtPGroup.Text.Trim());
                //}
                #endregion
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
        public ActionResult PrintStock()
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
            StockMISViewModel vm = new StockMISViewModel();
            vm.FontSize = 8;
            vm.ReportDecimal = 4;
            vm.ReportType = "Report_1";
            vm.DateFrom = DateTime.Now.ToString("dd-MMM-yyyy");
            vm.DateTo = DateTime.Now.ToString("dd-MMM-yyyy");
            vm.BranchId = Convert.ToInt32(Session["BranchId"]);

            return PartialView("_printStock", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportStock(StockMISViewModel vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();
                string post1;
                string post2;
                if (vm.Post)
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                else
                {
                    post1 = "Y";
                    post2 = "N";
                }
                DataSet ReportResult;
                vm.ItemNo = vm.ItemNo ?? "";
                vm.ProductGroup = vm.ProductGroup ?? "";
                vm.ProductType = vm.ProductType ?? "";
                // int BranchId = Convert.ToInt32(Session["BranchId"]);
                int BranchId = vm.BranchId;

                if (vm.BranchId == 0)
                {
                    BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (vm.BranchId == -1)
                {
                    BranchId = 0;
                }


                #region Date Format

                string DateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string DateTo = DateTime.Now.ToString("yyyy-MM-dd");

                bool isStartDate = false;
                bool isEndDate = false;

                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    isStartDate = true;
                    DateFrom = Convert.ToDateTime(vm.DateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    isEndDate = true;

                    DateTo = Convert.ToDateTime(vm.DateTo).ToString("yyyy-MM-dd");
                }


                #endregion


                reportDocument = _reportClass.StockNew(vm.ItemNo, vm.ProductGroup, vm.ProductType, DateFrom,
                   DateTo, post1, post2, vm.WithOutZero, false, "", BranchId, "", true, vm.QuantityOnly
                   , vm.Summary, false, null, vm.ReportDecimal.ToString(), identity.UserId, "N", vm.ReportType
                   , Convert.ToString(Session["LogInUserName"]), isStartDate, isEndDate);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

                #region Old
                //ReportResult = repo.StockNew(vm.ItemNo, vm.ProductGroup, vm.ProductType, DateFrom, DateTo, post1, post2, false, false, null);

                //DataTable dtStock = ReportResult.Tables[0].Select("ItemType <> 'Overhead'").CopyToDataTable();
                //objrpt = new RptStockAll();
                //objrpt.SetDataSource(dtStock);
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Stock Information'";
                //objrpt.DataDefinition.FormulaFields["FSummery"].Text = "'Y'";
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + identity.UserId + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                //if (vm.ProductName == "")
                //{
                //    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PProduct"].Text = "'" + vm.ProductName + "'  ";
                //}


                //if (vm.ProductGroup == "")
                //{
                //    objrpt.DataDefinition.FormulaFields["PCategory"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PCategory"].Text = "'" + vm.ProductGroup +
                //                                                            "'  ";
                //}

                //if (vm.ProductType == "")
                //{
                //    objrpt.DataDefinition.FormulaFields["PType"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PType"].Text = "'" + vm.ProductType + "'  ";
                //}

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

        public ActionResult DownloadMISStock(StockMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                MISReportRepo _repo = new MISReportRepo(identity, Session);
                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();
                MISReport _reportClass = new MISReport();

                connVM = Ordinary.StaticValueReAssign(identity, Session);

                string post1;
                string post2;
                if (vm.Post)
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                else
                {
                    post1 = "Y";
                    post2 = "N";
                }

                DataSet ReportResult;
                vm.ItemNo = vm.ItemNo ?? "";
                vm.ProductGroup = vm.ProductGroup ?? "";
                vm.ProductType = vm.ProductType ?? "";
                vm.FileName = DateTime.Now.ToString("dd_MMM_yyyy_HH_mm_ss") + "_Stock_Report";
                ////int BranchId = Convert.ToInt32(Session["BranchId"]);
                int BranchId = vm.BranchId;

                if (vm.BranchId == 0)
                {
                    BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (vm.BranchId == -1)
                {
                    BranchId = 0;
                }

                #region Date Format

                string DateFrom = "1753-01-01";
                string DateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    DateFrom = Convert.ToDateTime(vm.DateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    DateTo = Convert.ToDateTime(vm.DateTo).ToString("yyyy-MM-dd");
                }


                #endregion

                StockMovementVM sVM = new StockMovementVM();
                sVM.ItemNo = vm.ItemNo;
                sVM.StartDate = DateFrom;
                sVM.ToDate = DateTo;
                sVM.BranchId = BranchId;
                sVM.Post1 = post1;
                sVM.Post2 = post2;
                sVM.ProductType = vm.ProductType;
                sVM.CategoryLike = false;
                sVM.FormNumeric = "";
                sVM.CurrentUserID = identity.UserId;
                sVM.Branchwise = BranchId == 0 ? false : true;
                sVM.OrderBy = vm.OrderBy;



                DataSet dS = _repo.StockMovement(sVM, null, null);

                DataTable dt = dS.Tables[1];
                //vm.d
                vm.dt = dt;
                //DataTable dt = _repo.GetSaleExcelDataWeb(paramVM.IDs);

                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                vm.ReportHeaderName = "Stock Summery";

                vm = _reportClass.MISStock_Download(vm, connVM);

                if (vm.varExcelPackage == null)
                {
                    Session["result"] = "Fail" + "~" + "No data available!";
                    return RedirectToAction("Index");
                }

                //var vm = OrdinaryVATDesktop.DownloadExcel(dt, "Sale", "SaleM");
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
                throw ex;
            }

        }

        public ActionResult DownloadMISStockDetail(StockMISViewModel vm)
        {
            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                MISReportRepo _repo = new MISReportRepo(identity, Session);
                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();
                MISReport _reportClass = new MISReport();
                connVM = Ordinary.StaticValueReAssign(identity, Session);

                string post1;
                string post2;
                if (vm.Post)
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                else
                {
                    post1 = "Y";
                    post2 = "N";
                }

                DataSet ReportResult;
                vm.ItemNo = vm.ItemNo ?? "";
                vm.ProductGroup = vm.ProductGroup ?? "";
                vm.ProductType = vm.ProductType ?? "";
                vm.FileName = DateTime.Now.ToString("dd_MMM_yyyy_HH_mm_ss") + "_Stock_Report";
                ////int BranchId = Convert.ToInt32(Session["BranchId"]);
                int BranchId = vm.BranchId;

                if (vm.BranchId == 0)
                {
                    BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (vm.BranchId == -1)
                {
                    BranchId = 0;
                }

                #region Date Format

                string DateFrom = "1753-Jan-01";
                string DateTo = DateTime.Now.ToString("yyyy-MMM-dd");

                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    DateFrom = Convert.ToDateTime(vm.DateFrom).ToString("yyyy-MMM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    DateTo = Convert.ToDateTime(vm.DateTo).ToString("yyyy-MMM-dd");
                }


                #endregion

                StockMovementVM sVM = new StockMovementVM();
                sVM.ItemNo = vm.ItemNo;
                sVM.StartDate = DateFrom;
                sVM.ToDate = DateTo;
                sVM.BranchId = BranchId;
                sVM.Post1 = post1;
                sVM.Post2 = post2;
                sVM.ProductType = vm.ProductType;
                sVM.CategoryLike = false;
                sVM.FormNumeric = "";
                sVM.CurrentUserID = identity.UserId;
                sVM.Branchwise = BranchId == 0 ? false : true;
                sVM.OrderBy = vm.OrderBy;



                DataSet dS = _repo.StockMovement(sVM, null, null);

                DataTable dt = dS.Tables[0];
                //vm.d
                vm.dt = dt;
                //DataTable dt = _repo.GetSaleExcelDataWeb(paramVM.IDs);

                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                vm.ReportHeaderName = "Stock Details Movement";

                vm = _reportClass.MISStock_Download(vm, connVM);

                if (vm.varExcelPackage == null)
                {
                    Session["result"] = "Fail" + "~" + "No data available!";
                    return RedirectToAction("Index");
                }

                //var vm = OrdinaryVATDesktop.DownloadExcel(dt, "Sale", "SaleM");
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
                throw ex;
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult PrintReceiveStock()
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
            StockMISViewModel vm = new StockMISViewModel();
            vm.FontSize = 8;

            return PartialView("_printReceiveStock", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportReceiveStock(StockMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                var post = vm.Post ? "Y" : "N";
                DataSet ReportResult;
                vm.DateFrom = vm.DateFrom ?? "";
                vm.DateTo = vm.DateTo ?? "";
                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();

                #region Date Format

                string DateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string DateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    DateFrom = Convert.ToDateTime(vm.DateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    DateTo = Convert.ToDateTime(vm.DateTo).ToString("yyyy-MM-dd");
                }

                if (string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    DateFrom = "1900-01-01";
                }
                if (string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    DateTo = "2900-12-31";
                }


                #endregion

                reportDocument = _reportClass.SaleReceiveMIS(DateFrom, DateTo, vm.ShiftId, post);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");



                //ReportResult = repo.SaleReceiveMIS(DateFrom, DateTo, "1", post);

                //ReportResult.Tables[0].TableName = "dtSaleReceive";
                //objrpt = new RptSaleReceive();
                //objrpt.SetDataSource(ReportResult);

                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";

                //objrpt.DataDefinition.FormulaFields["shift"].Text = "'A'";

                //if (vm.DateFrom == "")
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + vm.DateFrom + "'  ";
                //}

                //if (vm.DateTo == "")
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + vm.DateTo + "'  ";
                //}

                //var rpt = gr.RenderReportAsPDF(objrpt);
                //objrpt.Close();
                //return rpt;




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
        [HttpGet]
        public ActionResult PrintDeposit()
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
            vm.FontSize = 8;
            return PartialView("_printDeposit", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult PrintVDS()
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
            vm.FontSize = 8;
            return PartialView("_printVDS", vm);
        }


        [Authorize]
        [HttpPost]
        public ActionResult ReportDeposit(DepositMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                var post = vm.Post ? "Y" : "N";
                DataSet ReportResult;
                vm.BankId = vm.BankId ?? "";
                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();

                #region Date Format

                string DepositDateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string DepositDateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.DepositDateFrom))
                {
                    DepositDateFrom = Convert.ToDateTime(vm.DepositDateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.DepositDateTo))
                {
                    DepositDateTo = Convert.ToDateTime(vm.DepositDateTo).AddDays(1).ToString("yyyy-MM-dd");
                }

                #endregion

                if (vm.DepositNo == null)
                {
                    vm.DepositNo = "";
                }
                if (vm.DepositDateFrom == null)
                {
                    DepositDateFrom = "";
                }
                if (vm.DepositDateTo == null)
                {
                    DepositDateTo = "";
                }
                if (vm.BankId == null)
                {
                    vm.BankId = "";
                }
                if (post == null)
                {
                    post = "";
                }
                if (vm.VendorId == null)
                {
                    vm.VendorId = "";
                }


                MISReport _reportClass = new MISReport();
                bool PreviewOnly = true;

                if (vm.ReportType == "VDS" || vm.ReportType == "VDS-TR6")
                {
                    reportDocument = _reportClass.VDSReport(vm.DepositNo, DepositDateFrom, DepositDateTo, vm.BankId, post, "", vm.VendorId, vm.ReportType);

                }
                else
                {
                    reportDocument = _reportClass.DepositNew(vm.DepositNo, DepositDateFrom, DepositDateTo, vm.BankId, post, "");

                }

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

                #region Old

                //ReportResult = repo.DepositNew(vm.DepositNo, DepositDateFrom, DepositDateTo, vm.BankId, post, "");
                //ReportResult.Tables[0].TableName = "DsDeposit";
                //objrpt = new RptTreasuryDepositTransaction();
                //objrpt.SetDataSource(ReportResult);

                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + identity.UserId + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Deposit Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'Address2'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'Address3'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";


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
        public ActionResult DownloadVDSSummary(DepositMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();

                #region Date Format

                string DepositDateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string DepositDateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.DepositDateFrom))
                {
                    vm.DepositDateFrom = Convert.ToDateTime(vm.DepositDateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.DepositDateTo))
                {
                    vm.DepositDateTo = Convert.ToDateTime(vm.DepositDateTo).AddDays(1).ToString("yyyy-MM-dd");
                }

                #endregion

                if (vm.DepositNo == null)
                {
                    vm.DepositNo = "";
                }
                if (vm.DepositDateFrom == null)
                {
                    vm.DepositDateFrom = "";
                }
                if (vm.DepositDateTo == null)
                {
                    vm.DepositDateTo = "";
                }
                if (vm.BankId == null)
                {
                    vm.BankId = "";
                }

                if (vm.VendorId == null)
                {
                    vm.VendorId = "";
                }


                MISReport _reportClass = new MISReport();

                vm.ReportHeaderName = "VDS Summary";

                vm = _reportClass.MISSummaryVDS_Download(vm);



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
                throw ex;
            }
            finally
            {

            }
        }
        [Authorize]
        [HttpGet]
        public ActionResult PrintAdjustment()
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
            AdjustmentMISViewModel vm = new AdjustmentMISViewModel();
            vm.FontSize = 8;

            return PartialView("_printAdjustment", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportAdjustment(AdjustmentMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                var post = vm.Post ? "Y" : "N";
                DataSet ReportResult;
                vm.Name = vm.Name ?? "";
                vm.AdjType = vm.AdjType ?? "";
                vm.AdjDateFrom = vm.AdjDateFrom ?? "";
                vm.AdjDateTo = vm.AdjDateTo ?? "";
                ////int BranchId = Convert.ToInt32(Session["BranchId"]);
                int BranchId = vm.BranchId;
                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();
                #region Date Format

                string AdjDateFrom = "1753-01-01";
                //string AdjDateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");1753-01-01
                //string AdjDateTo = DateTime.Now.ToString("yyyy-MM-dd");
                //string AdjDateTo = DateTime.MaxValue.ToString("yyyy-MM-dd");9999-12-31
                string AdjDateTo = "9998-12-31";

                if (!string.IsNullOrWhiteSpace(vm.AdjDateFrom))
                {
                    AdjDateFrom = Convert.ToDateTime(vm.AdjDateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.AdjDateTo))
                {
                    AdjDateTo = Convert.ToDateTime(vm.AdjDateTo).AddDays(1).ToString("yyyy-MM-dd");
                }



                #endregion



                //ReportResult = repo.Adjustment(vm.Name, vm.AdjType, AdjDateFrom, AdjDateTo, post);

                MISReport _reportClass = new MISReport();
                bool PreviewOnly = true;
                reportDocument = _reportClass.Adjustment(vm.Name, vm.AdjType, AdjDateFrom, AdjDateTo, post, BranchId);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");



                //ReportResult.Tables[0].TableName = "DtAdjustment";
                //objrpt = new RptAdjustment();
                //objrpt.SetDataSource(ReportResult);

                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + identity.UserId + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Adjustment Information'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                //var rpt = gr.RenderReportAsPDF(objrpt);
                //objrpt.Close();
                //return rpt;
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
        [HttpGet]
        public ActionResult PrintCoEfficient()
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
            CoEfficientMISViewModel vm = new CoEfficientMISViewModel();
            vm.FontSize = 8;

            return PartialView("_printCoEfficient", vm);
        }
        public ActionResult TransferIssue(ReportCommonVM vm)
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

            vm.Branch = Convert.ToInt32(Session["BranchId"]);
            vm.FontSize = 8;
            return PartialView("_TransferIssuePopUp", vm);
        }

        public ActionResult DownloadMIS_TransferIssueReport(ReportCommonVM vm)
        {
            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                TransferIssueRepo _transferIssueRepo = new TransferIssueRepo(identity, Session);
                //vm.branchId = Convert.ToInt32(Session["BranchId"]);

                #region null caheck

                if (vm.StartDate == null)
                {
                    vm.StartDate = "";
                }
                if (vm.EndDate == null)
                {
                    vm.EndDate = "";
                }

                if (vm.IssueNo == null)
                {
                    vm.IssueNo = "";
                }
                if (vm.Branch == null)
                {
                    vm.Branch = 0;
                }
                if (vm.TransferBranch == null)
                {
                    vm.TransferBranch = 0;
                }

                #endregion

                #region Date Format

                if (!string.IsNullOrWhiteSpace(vm.StartDate))
                {
                    vm.StartDate = Convert.ToDateTime(vm.StartDate).ToString("yyyy-MM-dd 00:00:00");
                }

                if (!string.IsNullOrWhiteSpace(vm.EndDate))
                {
                    vm.EndDate = Convert.ToDateTime(vm.EndDate).ToString("yyyy-MM-dd 23:59:59");
                }

                #endregion

                //vm.FileName = "SaleInformation";
                MISExcelVM ExcelVar = _transferIssueRepo.DownloadMIS_TransferIssueReport(vm.IssueNo, vm.StartDate, vm.EndDate, vm.TransactionType, vm.Branch, vm.TransferBranch);

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + ExcelVar.FileName + ".xlsx");
                    ExcelVar.varExcelPackage.SaveAs(memoryStream);
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



        public ActionResult TransferReceive(ReportCommonVM vm)
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

            vm.Branch = Convert.ToInt32(Session["BranchId"]);
            vm.FontSize = 8;
            return PartialView("_TransferReceivePopUP", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportCoEfficient(CoEfficientMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                string post1, post2;
                if (!vm.Post)
                {
                    post1 = "Y";
                    post2 = "N";
                }
                else
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                DataSet ReportResult;
                vm.ItemNo = vm.ItemNo ?? "";
                vm.DateFrom = vm.DateFrom ?? "";
                vm.DateTo = vm.DateTo ?? "";
                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();

                #region Date Format

                string DateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string DateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    DateFrom = Convert.ToDateTime(vm.DateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    DateTo = Convert.ToDateTime(vm.DateTo).ToString("yyyy-MM-dd");
                }

                #endregion


                MISReport _reportClass = new MISReport();
                _reportClass.DateFrom = Convert.ToDateTime(DateFrom).ToString("dd/MMM/yyyy");
                _reportClass.DateTo = Convert.ToDateTime(DateTo).ToString("dd/MMM/yyyy");
                bool PreviewOnly = true;
                reportDocument = _reportClass.InputOutputCoEfficient(vm.ItemNo, DateFrom, DateTo, post1, post2);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

                #region Old

                //ReportResult = repo.InputOutputCoEfficient(vm.ItemNo, DateFrom, DateTo, post1, post2);
                //ReportResult.Tables[0].TableName = "DtInputOutputCoEfficientT1";
                //ReportResult.Tables[1].TableName = "DtInputOutputCoEfficientT2";
                //objrpt = new RptInputOutputCoEfficient();
                //objrpt.SetDataSource(ReportResult);

                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + identity.UserId + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 16 MIS Report'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";
                //objrpt.DataDefinition.FormulaFields["DateFrom"].Text = "'" + vm.DateFrom + "'  ";
                //objrpt.DataDefinition.FormulaFields["DateTo"].Text = "'" + vm.DateTo + "'  ";
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
        [HttpGet]
        public ActionResult PrintWastage()
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
            WastageMISViewModel vm = new WastageMISViewModel();
            vm.FontSize = 8;

            return PartialView("_printWastage", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportWastage(WastageMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();
                #region Value Assign

                string post1;
                string post2;
                if (vm.Post)
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                else
                {
                    post1 = "Y";
                    post2 = "N";
                }
                DataSet ReportResult;
                if (vm.DateFrom == null)
                {
                    vm.DateFrom = "";
                }
                if (vm.DateTo == null)
                {
                    vm.DateTo = "";
                }
                if (vm.ItemNo == null)
                {
                    vm.ItemNo = "";
                }
                if (vm.ProductGroup == null)
                {
                    vm.ProductGroup = "";
                }
                if (vm.ProductType == null)
                {
                    vm.ProductType = "";
                }
                #endregion

                #region Date Format

                string DateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string DateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    DateFrom = Convert.ToDateTime(vm.DateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    DateTo = Convert.ToDateTime(vm.DateTo).ToString("yyyy-MM-dd");
                }

                #endregion

                MISReport _reportClass = new MISReport();
                bool PreviewOnly = true;
                reportDocument = _reportClass.Wastage(vm.ItemNo, vm.ProductGroup, vm.ProductType, post1, post2, DateFrom, DateTo);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

                #region


                //ReportResult = repo.StockWastage(vm.ItemNo, vm.ProductGroup, vm.ProductType, DateFrom, DateTo, post1, post2, vm.WithOutZero);
                //DataTable dtStock = ReportResult.Tables[0].Select("ItemType <> 'Overhead'").CopyToDataTable();
                //objrpt = new RptWastage();
                //objrpt.SetDataSource(dtStock);

                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Wastage Information'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                //if (vm.DateFrom == "")
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateFrom"].Text = "'" + vm.DateFrom + "'  ";
                //}

                //if (vm.DateTo == "")
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'[All]'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["PDateTo"].Text = "'" + vm.DateTo + "'  ";
                //}

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
        [HttpGet]
        public ActionResult PrintTollRegister()
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
            TollRegisterMISViewModel vm = new TollRegisterMISViewModel();
            return PartialView("_printTollRegister", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportTollRegister(TollRegisterMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                string post1;
                string post2;

                if (vm.Preview == true)
                {
                    post1 = "Y";
                    post2 = "N";
                }
                else
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                vm.ItemNo = vm.ItemNo ?? "";
                vm.DateFrom = vm.DateFrom ?? "";
                vm.DateTo = vm.DateTo ?? "";

                #region Date Format

                string DateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string DateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    DateFrom = Convert.ToDateTime(vm.DateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    DateTo = Convert.ToDateTime(vm.DateTo).ToString("yyyy-MM-dd");
                }

                #endregion



                var ReportResult = repo.TollRegister(vm.ItemNo, "Admin", DateFrom, DateTo, post1, post2);
                var vat16s = repo.VAT16List(ReportResult);
                //RptVAT16_New objrpt = new RptVAT16_New();
                ReportClass objrpt = new ReportClass();
                objrpt.SetDataSource(vat16s);

                if (vm.Preview == true)
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

                // var gr = new GenericReport<RptVAT16_New>();
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
        public ActionResult PrintSerialStock()
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
            StockMISViewModel vm = new StockMISViewModel();
            vm.FontSize = 8;

            return PartialView("_printSerialStock", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportSerialStock(StockMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();
                string post1;
                string post2;
                if (vm.Post)
                {
                    post1 = "Y";
                    post2 = "Y";
                }
                else
                {
                    post1 = "Y";
                    post2 = "N";
                }
                DataSet ReportResult;
                vm.ItemNo = vm.ItemNo ?? "";
                vm.ProductGroup = vm.ProductGroup ?? "";
                vm.ProductType = vm.ProductType ?? "";

                #region Date Format

                string DateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string DateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    DateFrom = Convert.ToDateTime(vm.DateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    DateTo = Convert.ToDateTime(vm.DateTo).ToString("yyyy-MM-dd");
                }

                #endregion



                ReportResult = repo.SerialStockStatus(vm.ItemNo, vm.ProductGroup, vm.ProductType, DateFrom, DateTo, post1, post2);
                ReportResult.Tables[0].TableName = "DsSerialStock";
                objrpt = new RptSerialStockStatus();
                objrpt.SetDataSource(ReportResult);

                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                objrpt.DataDefinition.FormulaFields["Heading1"].Text = "'Heading1'";
                objrpt.DataDefinition.FormulaFields["Heading2"].Text = "'Heading2'";

                if (vm.DateFrom == "")
                {
                    objrpt.DataDefinition.FormulaFields["FromDate"].Text = "'[All]'";
                    objrpt.DataDefinition.FormulaFields["FromDate"].Text = "'-'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["FromDate"].Text = "'" + vm.DateFrom + "'  ";
                }

                if (vm.DateTo == "")
                {
                    objrpt.DataDefinition.FormulaFields["ToDate"].Text = "'-'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["ToDate"].Text = "'" + vm.DateTo + "'  ";
                }

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
        public ActionResult PrintPurchaseLC()
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
            PurchaseMISViewModel vm = new PurchaseMISViewModel();
            vm.FontSize = 8;
            return PartialView("_printPurchaseLC", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportPurchaseLC(PurchaseMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();

                var post = vm.Post ? "Y" : "N";
                vm.PurchaseNo = vm.PurchaseNo ?? "";
                vm.ReceiveDateFrom = vm.ReceiveDateFrom ?? "";
                vm.ReceiveDateTo = vm.ReceiveDateTo ?? "";
                vm.VendorName = vm.VendorName ?? "";
                vm.VendorGroup = vm.VendorGroup ?? "";
                vm.LC = vm.LC ?? "";


                #region Date Format

                string ReceiveDateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string ReceiveDateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.ReceiveDateFrom))
                {
                    ReceiveDateFrom = Convert.ToDateTime(vm.ReceiveDateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.ReceiveDateTo))
                {
                    ReceiveDateTo = Convert.ToDateTime(vm.ReceiveDateTo).ToString("yyyy-MM-dd");
                }

                #endregion

                MISReport _reportClass = new MISReport();
                bool PreviewOnly = true;
                reportDocument = _reportClass.PurchaseWithLCInfo(vm.PurchaseNo, ReceiveDateFrom, ReceiveDateTo, vm.VendorName, "", vm.VendorGroup, vm.LC, post);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");


                //DataSet ReportResult = repo.PurchaseWithLCInfo(vm.PurchaseNo, ReceiveDateFrom, ReceiveDateTo, vm.VendorName, "", vm.VendorGroup, vm.LC, post);
                //ReportResult.Tables[0].TableName = "DsPurchaseLC";
                //objrpt = new RptPurchase_LCInfo();
                //objrpt.SetDataSource(ReportResult);

                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + identity.UserId + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                //if (vm.LC == "")
                //{ objrpt.DataDefinition.FormulaFields["LCNo"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["LCNo"].Text = "'" + vm.LC + "'  "; }

                //if (vm.VendorName == "")
                //{ objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PVendor"].Text = "'" + vm.VendorName + "'  "; }

                //if (vm.PurchaseNo == "")
                //{ objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["PInvoice"].Text = "'" + vm.PurchaseNo + "'  "; }

                //if (vm.ReceiveDateFrom == "")
                //{ objrpt.DataDefinition.FormulaFields["LCDateFrom"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["LCDateFrom"].Text = "'" + vm.ReceiveDateFrom + "'  "; }

                //if (vm.ReceiveDateTo == "")
                //{ objrpt.DataDefinition.FormulaFields["LCDateTo"].Text = "'[All]'"; }
                //else
                //{ objrpt.DataDefinition.FormulaFields["LCDateTo"].Text = "'" + vm.ReceiveDateTo + "'  "; }


                //var rpt = gr.RenderReportAsPDF(objrpt);
                //objrpt.Close();
                //return rpt;
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
        [HttpGet]
        public ActionResult PrintCStatement()
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
            CStatementMISViewModel vm = new CStatementMISViewModel();
            vm.FontSize = 8;
            return PartialView("_printCStatement", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportCStatement(CStatementMISViewModel vm)
        {
            try
            {
                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();
                #region Statement
                List<MIS19VM> mis19s = new List<MIS19VM>();
                MIS19VM mis19A = new MIS19VM();
                MIS19VM mis19B = new MIS19VM();

                DataTable ReportResult1 = new DataTable();
                DataTable ReportResult2 = new DataTable();

                #region Date Format

                string FirstDateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string FirstDateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.FirstDateFrom))
                {
                    FirstDateFrom = Convert.ToDateTime(vm.FirstDateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.FirstDateTo))
                {
                    FirstDateTo = Convert.ToDateTime(vm.FirstDateTo).ToString("yyyy-MM-dd");
                }


                string SecondDateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string SecondDateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.SecondDateFrom))
                {
                    SecondDateFrom = Convert.ToDateTime(vm.SecondDateFrom).ToString("yyyy-MM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.SecondDateTo))
                {
                    SecondDateTo = Convert.ToDateTime(vm.SecondDateTo).ToString("yyyy-MM-dd");
                }

                #endregion


                ReportResult1 = repo.MIS19(FirstDateFrom, FirstDateTo);
                ReportResult2 = repo.MIS19(SecondDateFrom, SecondDateTo);
                string MonthA = Convert.ToDateTime(vm.FirstDateFrom).ToString("MMM/yy") + " To " + Convert.ToDateTime(vm.FirstDateTo).ToString("MMM/yy");
                string MonthB = Convert.ToDateTime(vm.SecondDateFrom).ToString("MMM/yy") + " To " + Convert.ToDateTime(vm.SecondDateTo).ToString("MMM/yy");
                #endregion

                #region 1
                foreach (DataRow item in ReportResult1.Rows)
                {
                    mis19A = new MIS19VM();
                    mis19A.One = item["One"].ToString();
                    mis19A.Two = MonthA;
                    mis19A.Three = Convert.ToDecimal(item["Three"].ToString());
                    mis19A.FourA = Convert.ToDecimal(item["FourA"].ToString());
                    mis19A.FourB = Convert.ToDecimal(item["FourB"].ToString());
                    mis19A.FourC = Convert.ToDecimal(item["FourC"].ToString());
                    mis19A.FourD = Convert.ToDecimal(item["FourD"].ToString());
                    mis19A.Five = Convert.ToDecimal(item["Five"].ToString());
                    mis19A.Six = Convert.ToDecimal(item["Six"].ToString());
                    mis19A.SevenA = Convert.ToDecimal(item["SevenA"].ToString());
                    mis19A.SevenB = Convert.ToDecimal(item["SevenB"].ToString());
                    mis19A.SevenC = Convert.ToDecimal(item["SevenC"].ToString());
                    mis19A.SevenD = Convert.ToDecimal(item["SevenD"].ToString());
                    mis19A.EightA = Convert.ToDecimal(item["EightA"].ToString());
                    mis19A.EightB = Convert.ToDecimal(item["EightB"].ToString());
                    mis19A.EightC = Convert.ToDecimal(item["EightC"].ToString());
                    mis19A.EightD = Convert.ToDecimal(item["EightD"].ToString());
                    mis19A.Nine = Convert.ToDecimal(item["Nine"].ToString());
                    mis19A.Ten = Convert.ToDecimal(item["Ten"].ToString());
                    mis19A.Eleven = Convert.ToDecimal(item["Eleven"].ToString());
                    mis19A.Twelve = Convert.ToDecimal(item["Twelve"].ToString());
                    mis19A.Thirteen = Convert.ToDecimal(item["Thirteen"].ToString());
                    mis19A.Fourteen = Convert.ToDecimal(item["Fourteen"].ToString());
                    mis19A.Fifteen = Convert.ToDecimal(item["Fifteen"].ToString());
                    mis19A.Sixteen = Convert.ToDecimal(item["Sixteen"].ToString());
                    mis19A.Seventeen = Convert.ToDecimal(item["Seventeen"].ToString());
                    mis19A.Eighteen = "";//  item["Eighteen"].ToString( );
                    mis19s.Add(mis19A);

                }
                #endregion 1
                #region 2
                foreach (DataRow item in ReportResult2.Rows)
                {
                    mis19B = new MIS19VM();
                    mis19B.One = item["One"].ToString();
                    mis19B.Two = MonthB;// item["Two"].ToString();
                    mis19B.Three = Convert.ToDecimal(item["Three"].ToString());
                    mis19B.FourA = Convert.ToDecimal(item["FourA"].ToString());
                    mis19B.FourB = Convert.ToDecimal(item["FourB"].ToString());
                    mis19B.FourC = Convert.ToDecimal(item["FourC"].ToString());
                    mis19B.FourD = Convert.ToDecimal(item["FourD"].ToString());
                    mis19B.Five = Convert.ToDecimal(item["Five"].ToString());
                    mis19B.Six = Convert.ToDecimal(item["Six"].ToString());
                    mis19B.SevenA = Convert.ToDecimal(item["SevenA"].ToString());
                    mis19B.SevenB = Convert.ToDecimal(item["SevenB"].ToString());
                    mis19B.SevenC = Convert.ToDecimal(item["SevenC"].ToString());
                    mis19B.SevenD = Convert.ToDecimal(item["SevenD"].ToString());
                    mis19B.EightA = Convert.ToDecimal(item["EightA"].ToString());
                    mis19B.EightB = Convert.ToDecimal(item["EightB"].ToString());
                    mis19B.EightC = Convert.ToDecimal(item["EightC"].ToString());
                    mis19B.EightD = Convert.ToDecimal(item["EightD"].ToString());
                    mis19B.Nine = Convert.ToDecimal(item["Nine"].ToString());
                    mis19B.Ten = Convert.ToDecimal(item["Ten"].ToString());
                    mis19B.Eleven = Convert.ToDecimal(item["Eleven"].ToString());
                    mis19B.Twelve = Convert.ToDecimal(item["Twelve"].ToString());
                    mis19B.Thirteen = Convert.ToDecimal(item["Thirteen"].ToString());
                    mis19B.Fourteen = Convert.ToDecimal(item["Fourteen"].ToString());
                    mis19B.Fifteen = Convert.ToDecimal(item["Fifteen"].ToString());
                    mis19B.Sixteen = Convert.ToDecimal(item["Sixteen"].ToString());
                    mis19B.Seventeen = Convert.ToDecimal(item["Seventeen"].ToString());
                    mis19B.Eighteen = "";// item["Eighteen"].ToString();
                    mis19s.Add(mis19B);

                }

                #endregion 2

                #region Info
                int TMonthA = 0;
                int TMonthB = 0;
                string Info1 = "";
                string Info2 = "";
                string Info3 = "";
                string Info4 = "";
                string Info5 = "";
                string Info6 = "";
                string Info7 = "";
                string Info8 = "";
                string Info9 = "";
                string Info10 = "";
                string Info11 = "";
                string Info12 = "";
                decimal Result = 0;

                TMonthA = (Convert.ToInt32(Convert.ToDateTime(vm.FirstDateTo).ToString("yyyy")) - Convert.ToInt32(Convert.ToDateTime(vm.FirstDateFrom).ToString("yyyy"))) * 12 + Convert.ToInt32(Convert.ToDateTime(vm.FirstDateTo).ToString("MM")) - Convert.ToInt32(Convert.ToDateTime(vm.FirstDateFrom).ToString("MM")) + 1;
                TMonthB = (Convert.ToInt32(Convert.ToDateTime(vm.SecondDateTo).ToString("yyyy")) - Convert.ToInt32(Convert.ToDateTime(vm.SecondDateFrom).ToString("yyyy"))) * 12 + Convert.ToInt32(Convert.ToDateTime(vm.SecondDateTo).ToString("MM")) - Convert.ToInt32(Convert.ToDateTime(vm.SecondDateFrom).ToString("MM")) + 1;
                Result = 0;
                if (mis19A.SevenD > 0)

                    Result = (mis19B.SevenD - mis19A.SevenD) / mis19A.SevenD * 100;

                Info1 = Result.ToString("#.##") + " %";

                Result = 0;
                if (mis19A.Ten > 0)
                    Result = (mis19B.Ten - mis19A.Ten) / mis19A.Ten * 100;
                Info2 = Result.ToString("#.##") + " %";

                Result = 0;
                if (mis19A.Twelve > 0)
                    Result = (mis19B.Twelve - mis19A.Twelve) / mis19A.Twelve * 100;
                Info3 = Result.ToString("#.##") + " %";

                Result = 0;
                if ((mis19A.Eleven - mis19A.Fifteen) > 0)
                    Result = ((mis19B.Eleven - mis19B.Fifteen) - (mis19A.Eleven - mis19A.Fifteen)) / (mis19A.Eleven - mis19A.Fifteen) * 100;
                Info4 = Result.ToString("#.##") + " %";

                Result = 0;
                if (mis19A.Fourteen > 0)
                    Result = (mis19B.Fourteen - mis19A.Fourteen) / mis19A.Fourteen * 100;
                Info5 = Result.ToString("#.##") + " %";

                Result = 0;
                if (mis19A.SevenD > 0)
                    Result = mis19A.Twelve / mis19A.SevenD * 100;
                Info6 = Result.ToString("#.##") + " %";

                Result = 0;
                if (mis19B.SevenD > 0)
                    Result = mis19B.Twelve / mis19B.SevenD * 100;
                Info7 = Result.ToString("#.##") + " %";

                Result = 0;
                if (mis19B.SevenD > 0)
                    Info8 = Result.ToString("#.##");

                Result = 0;
                Result = (mis19B.SevenD - mis19B.Twelve);
                Info9 = Result.ToString("#.##");

                Result = 0;
                if (TMonthB > 0)
                    Result = (mis19B.SevenD - mis19B.Twelve) / TMonthB;
                Info10 = Result.ToString("#.##");

                Result = 0;
                if (TMonthB > 0)
                    Result = mis19B.FourD / TMonthB;
                Info11 = Result.ToString("#.##");

                Result = 0;
                if (mis19A.Seventeen > 0)
                    Result = (mis19B.Seventeen - mis19A.Seventeen) / mis19A.Seventeen * 100;
                Info12 = Result.ToString("#.##") + " %";

                #endregion Info

                //ReportResult.Tables[0].TableName = "DsPurchaseLC";
                objrpt = new MIS19();
                objrpt.SetDataSource(mis19s);

                #region folmula
                objrpt.DataDefinition.FormulaFields["Info1"].Text = "'" + Info1 + "'";
                objrpt.DataDefinition.FormulaFields["Info2"].Text = "'" + Info2 + "'";
                objrpt.DataDefinition.FormulaFields["Info3"].Text = "'" + Info3 + "'";
                objrpt.DataDefinition.FormulaFields["Info4"].Text = "'" + Info4 + "'";
                objrpt.DataDefinition.FormulaFields["Info5"].Text = "'" + Info5 + "'";
                objrpt.DataDefinition.FormulaFields["Info6"].Text = "'" + Info6 + "'";
                objrpt.DataDefinition.FormulaFields["Info7"].Text = "'" + Info7 + "'";
                objrpt.DataDefinition.FormulaFields["Info8"].Text = "'" + Info8 + "'";
                objrpt.DataDefinition.FormulaFields["Info9"].Text = "'" + Info9 + "'";
                objrpt.DataDefinition.FormulaFields["Info10"].Text = "'" + Info10 + "'";
                objrpt.DataDefinition.FormulaFields["Info11"].Text = "'" + Info11 + "'";
                objrpt.DataDefinition.FormulaFields["Info12"].Text = "'" + Info12 + "'";
                objrpt.DataDefinition.FormulaFields["MonthA"].Text = "'" + MonthA + "'";
                objrpt.DataDefinition.FormulaFields["MonthB"].Text = "'" + MonthB + "'";
                objrpt.DataDefinition.FormulaFields["TMonthA"].Text = "'= " + TMonthA.ToString() + " Month(s)" + "'";
                objrpt.DataDefinition.FormulaFields["TMonthB"].Text = "'= " + TMonthB.ToString() + " Month(s)" + "'";

                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'CompanyName'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Address1'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                #endregion

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
        public ActionResult PrintCurrentAccount()
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
            vm.FontSize = 8;

            return PartialView("_printCurrentAccount", vm);
        }



        [Authorize]
        [HttpPost]
        public ActionResult ReportCurrentAccount(DepositMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                string post1, post2 = string.Empty;

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                DataSet ReportResult;

                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();

                #region Date Format

                string DepositDateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string DepositDateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.DepositDateFrom))
                {
                    DepositDateFrom = Convert.ToDateTime(vm.DepositDateFrom).ToString("yyyy-MMM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.DepositDateTo))
                {
                    DepositDateTo = Convert.ToDateTime(vm.DepositDateTo).ToString("yyyy-MMM-dd");
                }

                #endregion

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

                if (vm.DepositDateFrom == null)
                {
                    DepositDateFrom = "";
                }
                if (vm.DepositDateTo == null)
                {
                    DepositDateTo = "";
                }

                MISReport _reportClass = new MISReport();

                reportDocument = _reportClass.VAT18(identity.Name, DepositDateFrom, DepositDateTo, post1, post2, vm.PreviewOnly);


                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");



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
        [HttpGet]
        public ActionResult PrintReconsciliation()
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
            ProductionMISViewModel vm = new ProductionMISViewModel();
            vm.FontSize = 8;
            return PartialView("_printReconsciliation", vm);
        }


        [Authorize]
        [HttpPost]
        public void ReportReconsciliation(ProductionMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                string[] Condition = new string[] { "one" };

                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                DataSet ReportResult;
                int BranchId = Convert.ToInt32(Session["BranchId"]);
                //int BranchId = vm.BranchId;

                #region Date Format

                string DepositDateFrom = "";
                string DepositDateTo = "";

                if (!string.IsNullOrWhiteSpace(vm.IssueDateFrom))
                {
                    DepositDateFrom = Convert.ToDateTime(vm.IssueDateFrom).ToString("yyyy-MMM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.IssueDateTo))
                {
                    DepositDateTo = Convert.ToDateTime(vm.IssueDateTo).ToString("yyyy-MMM-dd");
                }

                #endregion

                ProductRepo _repo = new ProductRepo(identity, Session);
                BranchProfileRepo BranchProfile = new BranchProfileRepo(identity, Session);

                ReportResult = _repo.GetReconsciliationData(DepositDateFrom, DepositDateTo);

                //DataTable dt = ReportResult.Tables[0];

                var branch = BranchProfile.SelectAll(BranchId.ToString(), null, null).FirstOrDefault(); ;

                string BranchLegalName = branch.BranchLegalName;


                Condition = new string[3];
                Condition[0] = BranchLegalName;
                Condition[1] = "Date From: " + Convert.ToDateTime(vm.IssueDateFrom).ToString("dd/MMM/yyyy") + " To " + Convert.ToDateTime(vm.IssueDateTo).ToString("dd/MMM/yyyy");
                Condition[2] = "Reconsciliation Statement";


                string[] ReportHeaders = new string[] { BranchLegalName, Condition[1], Condition[2] };

                //int TableHeadRow = 0;
                //TableHeadRow = ReportHeaders.Length + 2;

                //int RowCount = 0;
                //RowCount = dt.Rows.Count;

                //int ColumnCount = 0;
                //ColumnCount = dt.Columns.Count;

                string[] sheetNames = new[] { "Finish", "Raw_Pack" };


                ExcelPackage excel = new ExcelPackage();
                for (int j = 0; j < sheetNames.Length; j++)
                {
                    DataTable datatable = ReportResult.Tables[j];
                    int TableHeadRow = 0;
                    TableHeadRow = ReportHeaders.Length + 2;

                    int RowCount = 0;
                    RowCount = datatable.Rows.Count;

                    int ColumnCount = 0;
                    ColumnCount = datatable.Columns.Count;
                    ExcelWorksheet ws = excel.Workbook.Worksheets.Add(sheetNames[j]);
                    ws.Cells[TableHeadRow, 1].LoadFromDataTable(datatable, true);

                    #region Format

                    var format = new OfficeOpenXml.ExcelTextFormat();
                    format.Delimiter = '~';
                    format.TextQualifier = '"';
                    format.DataTypes = new[] { eDataTypes.String };



                    for (int i = 0; i < ReportHeaders.Length; i++)
                    {
                        ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                        ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                        ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

                    }

                    ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;

                    #endregion


                }
                //ExcelWorksheet ws = excel.Workbook.Worksheets.Add("Finish");
                ////ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);
                //ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                //#region Format

                //var format = new OfficeOpenXml.ExcelTextFormat();
                //format.Delimiter = '~';
                //format.TextQualifier = '"';
                //format.DataTypes = new[] { eDataTypes.String };



                //for (int i = 0; i < ReportHeaders.Length; i++)
                //{
                //    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                //    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                //    ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

                //}

                //ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;

                //#endregion


                string filename = "ReconsciliationReport" + DateTime.Now.ToString(" dd-MMM-yyyy");

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

            catch (Exception ex)
            {
                throw ex;
            }


        }

        [Authorize]
        [HttpGet]
        public ActionResult PrintBranchStockMovement()
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
            ProductionMISViewModel vm = new ProductionMISViewModel();
            vm.FontSize = 8;
            return PartialView("_printBranchStockMovement", vm);
        }

        [Authorize]
        [HttpPost]
        public void ReportBranchStockMovement(ProductionMISViewModel vm)
        {
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);


                ReportDSRepo repo = new ReportDSRepo(identity, Session);
                ReportClass objrpt = new ReportClass();
                var gr = new GenericReport<ReportClass>();
                DataSet ReportResult;
                string[] Condition = new string[] { "one" };

                int BranchId = Convert.ToInt32(Session["BranchId"]);

                #region Date Format

                string IssueDateFrom = DateTime.MinValue.ToString("yyyy-MM-dd");
                string IssueDateTo = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(vm.IssueDateFrom))
                {
                    IssueDateFrom = Convert.ToDateTime(vm.IssueDateFrom).ToString("yyyy-MMM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.IssueDateTo))
                {
                    IssueDateTo = Convert.ToDateTime(vm.IssueDateTo).ToString("yyyy-MMM-dd");
                }

                #endregion

                if (vm.ItemNo == null)
                {
                    vm.ItemNo = "";
                }

                TransferIssueRepo _repo = new TransferIssueRepo(identity, Session);
                BranchProfileRepo BranchProfile = new BranchProfileRepo(identity, Session);

                ReportResult = _repo.TransferMovement(vm.ItemNo, IssueDateFrom.ToString(), IssueDateTo.ToString(), BranchId, vm.Summary);
                var branch = BranchProfile.SelectAll(BranchId.ToString(), null, null).FirstOrDefault(); ;

                string BranchLegalName = branch.BranchLegalName;

                Condition = new string[3];
                Condition[0] = BranchLegalName;
                Condition[1] = "Date From: " + Convert.ToDateTime(vm.IssueDateFrom).ToString("dd/MMM/yyyy") + " To " + Convert.ToDateTime(vm.IssueDateTo).ToString("dd/MMM/yyyy");
                if (vm.Summary)
                {
                    Condition[2] = BranchLegalName + " Branch Stock Movement(Summery)";

                }
                else
                {
                    Condition[2] = BranchLegalName + " Branch Stock Movement(Details)";
                }

                DataTable dt = ReportResult.Tables[0];

                //string[] ReportHeaders = new string[] { BranchLegalName, " Date From: " + IssueDateFrom + "   To " + IssueDateTo, identity.Address1 };
                string[] ReportHeaders = new string[] { BranchLegalName, Condition[1], identity.Address1 };

                int TableHeadRow = 0;
                TableHeadRow = ReportHeaders.Length + 2;

                int RowCount = 0;
                RowCount = dt.Rows.Count;

                int ColumnCount = 0;
                ColumnCount = dt.Columns.Count;

                int GrandTotalRow = 0;
                GrandTotalRow = TableHeadRow + RowCount + 1;

                ExcelPackage excel = new ExcelPackage();

                //var workSheet = excel.Workbook.Worksheets.Add("StockMovement");

                //workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

                // using (ExcelPackage ExcelPackage = new ExcelPackage())
                //{

                //}

                ExcelWorksheet ws = excel.Workbook.Worksheets.Add("StockMovement");
                ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

                #region Format

                var format = new OfficeOpenXml.ExcelTextFormat();
                format.Delimiter = '~';
                format.TextQualifier = '"';
                format.DataTypes = new[] { eDataTypes.String };



                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                    ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

                }

                ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;

                #endregion


                string filename = "BranchStockMovement" + DateTime.Now.ToString("dd-MMM-yyyy");

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
                OrdinaryVATDesktop.Address2 = varCompanyProfileVM.Address2;
                OrdinaryVATDesktop.Address3 = varCompanyProfileVM.Address3;
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
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        //JBR 5292021
        [Authorize]
        [HttpGet]
        public ActionResult DownloadMIS_SaleReport(MISExcelVM vm)
        {
            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                SaleInvoiceRepo salRepoObj = new SaleInvoiceRepo(identity, Session);
                //vm.branchId = Convert.ToInt32(Session["BranchId"]);
                int BranchId = vm.branchId;

                if (vm.branchId == 0)
                {
                    BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (vm.SalesFromDate == null)
                {
                    //vm.SalesFromDate = "1753-Jan-01 00:00:00";
                    vm.SalesFromDate = "";
                }
                if (vm.SalesToDate == null)
                {
                    //vm.SalesToDate = "9998-Dec-31 00:00:00";
                    vm.SalesToDate = "";
                }

                if (vm.VatType == null || vm.VatType.ToLower() == "select")
                {
                    vm.VatType = "";
                }


                if (vm.SalesInvoiceNo == null)
                {
                    vm.SalesInvoiceNo = "";
                }


                if (string.IsNullOrWhiteSpace(vm.CustomerId) || vm.CustomerId == "undefined")
                {
                    vm.CustomerId = "";
                }
                if (string.IsNullOrWhiteSpace(vm.CustomerGroupID) || vm.CustomerGroupID == "undefined")
                {
                    vm.CustomerGroupID = "";
                }
                if (vm.ItemNo == null)
                {
                    vm.ItemNo = "";
                }
                if (string.IsNullOrWhiteSpace(vm.CategoryID) || vm.CategoryID == "undefined")
                {
                    vm.CategoryID = "";
                }
                if (vm.Post == true)
                {
                    vm.cmbPost = "Y";
                }
                else
                {
                    vm.cmbPost = "N";
                }
                if (vm.cmbType == null)
                {
                    vm.cmbType = "";
                }
                if (vm.vTransactionType == null)
                {
                    vm.vTransactionType = "";
                }

                #region Date Format

                //string DateFrom = "";
                //string DateTo = "";

                if (!string.IsNullOrWhiteSpace(vm.SalesFromDate))
                {
                    vm.SalesFromDate = Convert.ToDateTime(vm.SalesFromDate).ToString("yyyy-MM-dd HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(vm.SalesToDate))
                {
                    //DateTo = Convert.ToDateTime(vm.DateTo).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                    vm.SalesToDate = Convert.ToDateTime(vm.SalesToDate).ToString("yyyy-MM-dd 23:59:59");
                }

                if (vm.branchId == 0)
                {
                    vm.branchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (vm.branchId == -1)
                {
                    vm.branchId = 0;
                }

                #endregion
                //vm.FileName = "SaleInformation";
                MISExcelVM ExcelVar = salRepoObj.DownloadMIS_SaleReport(vm);


                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + ExcelVar.FileName + ".xlsx");
                    ExcelVar.varExcelPackage.SaveAs(memoryStream);
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

        //JBR 5292021
        [Authorize]
        [HttpGet]
        public ActionResult DownloadMIS_PurchaseReport(MISExcelVM vm)
        {
            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                PurchaseRepo purRepoObj = new PurchaseRepo(identity, Session);

                //vm.branchId = Convert.ToInt32(Session["BranchId"]);
                //vm.branchId = ParamVm.BranchId;

                if (vm.ReceiveDateFrom == null)
                {
                    //vm.ReceiveDateFrom = "1753-Jan-01 00:00:00";
                    vm.ReceiveDateFrom = "";
                }
                if (vm.ReceiveDateTo == null)
                {
                    //vm.ReceiveDateTo = "9998-Dec-31 00:00:00";
                    vm.ReceiveDateTo = "";
                }

                if (vm.PurchaseNo == null)
                {
                    vm.PurchaseNo = "";
                }
                if (vm.VatType == null)
                {
                    vm.VatType = "";
                }
                if (vm.VendorGroup == null)
                {
                    vm.VendorGroup = "";
                }
                if (vm.VendorId == null)
                {
                    vm.VendorId = "";
                }
                if (vm.VendorName == null)
                {
                    vm.VendorName = "";
                }
                if (vm.ProductType == null)
                {
                    vm.ProductType = "";
                }
                if (vm.ProductName == null)
                {
                    vm.ProductName = "";
                }
                if (vm.ProductGroupName == null)
                {
                    vm.ProductGroupName = "";
                }
                if (vm.ProductGroupId == null)
                {
                    vm.ProductGroupId = "";
                }
                if (vm.vTransactionType == null)
                {
                    vm.vTransactionType = "";
                }
                if (vm.Post)
                {
                    vm.cmbPost = "Y";
                }
                else
                {
                    vm.cmbPost = "N";
                }
                if (vm.branchId == 0)
                {
                    vm.branchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (vm.branchId == -1)
                {
                    vm.branchId = 0;
                }

                //vm.FileName = "SaleInformation";
                MISExcelVM ExcelVar = purRepoObj.DownloadMIS_PurchaseReport(vm);

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + ExcelVar.FileName + ".xlsx");
                    ExcelVar.varExcelPackage.SaveAs(memoryStream);
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

        [Authorize]
        [HttpGet]
        public ActionResult Traking(TrakingSaleVM vm)
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
        public ActionResult TrakingSaleExport(TrakingSaleVM vm)
        {
            //ProductRepo _repo = new ProductRepo(identity);
            SaleInvoiceRepo _repo = new SaleInvoiceRepo(identity, Session);

            string[] results = new string[5];
            DataTable dt = new DataTable();
            try
            {
                dt = _repo.GetSaleTrakingExcelData(vm);

                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                var vms = OrdinaryVATDesktop.DownloadExcel(dt, "SaleTraking", "SaleTrakingM");
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + vms.FileName + ".xlsx");
                    vms.varExcelPackage.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;

                return View("Traking", vm);
            }
            finally { }
            return RedirectToAction("Index");
            // return Redirect("/VMS/MISReport/Traking");

        }

        [Authorize]
        [HttpPost]
        public ActionResult TrakingPurchaseExport(TrakingSaleVM vm)
        {
            //ProductRepo _repo = new ProductRepo(identity);
            SaleInvoiceRepo _repo = new SaleInvoiceRepo(identity, Session);

            string[] results = new string[5];
            DataTable dt = new DataTable();
            try
            {
                dt = _repo.GetPurchaseTrakingExcelData(vm);

                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                var vms = OrdinaryVATDesktop.DownloadExcel(dt, "PurchseTraking", "PurchaseTrakingM");
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + vms.FileName + ".xlsx");
                    vms.varExcelPackage.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;

                return View("Traking", vm);
            }
            finally { }
            return RedirectToAction("Index");

        }

        [Authorize]
        [HttpPost]
        public ActionResult MISStockReport(TrakingSaleVM vm)
        {
            //ProductRepo _repo = new ProductRepo(identity);
            SaleInvoiceRepo _repo = new SaleInvoiceRepo(identity, Session);

            string[] results = new string[5];
            DataTable dt = new DataTable();
            try
            {
                dt = _repo.MISStockReport(vm);

                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                var vms = OrdinaryVATDesktop.DownloadExcel(dt, "stock report", "stock report");
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + vms.FileName + ".xlsx");
                    vms.varExcelPackage.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }

            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail~" + msg;

                return View("Traking", vm);
            }
            finally { }
            return RedirectToAction("Index");

        }
        [Authorize]
        [HttpGet]
        public ActionResult UBLReconsileReport()
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
        public ActionResult DataProcess(IntegrationParam vm)
        {
            try
            {
                MISReportRepo _repo = null;
                _repo = new MISReportRepo(identity, Session);

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
                String CompanyCode = Convert.ToString(Session["CompanyCode"]);
                vm.CompanyCode = CompanyCode;
                BranchProfileRepo _BranchProfileRepo = new BranchProfileRepo(identity, Session);

                vm.dtConnectionInfo = _BranchProfileRepo.SelectAl();
                vm.FromDate = vm.FromDate + " " + "00:00:00";
                vm.ToDate = vm.ToDate + " " + "23:59:59";
                rVM = _repo.DataProcess(vm);



                Session["result"] = rVM.Status + "~" + rVM.Message;
                return View("UBLReconsileReport", vm);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        [Authorize]
        public ActionResult ExportData(IntegrationParam model)
        {
            #region Access Control

            MISReportRepo _repo = null;
            _repo = new MISReportRepo(identity, Session);
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

                DataSet ds = new DataSet();

                DataTable dt = new DataTable();
                DataTable Branchdt = new DataTable();

                dt = _repo.GetTempData();
                DataTable dt1 = _repo.GetTempDataDetails();
                ds.Tables.Add(dt);
                ds.Tables.Add(dt1);

                var sheetNames = new[] { "SKU Wise", "Invoice Wize" };

                var vm = OrdinaryVATDesktop.DownloadExcelMultiple(ds, "Day End", sheetNames);

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


        #region ACI Reconsile Report

        [Authorize]
        [HttpGet]
        public ActionResult ACIReconsileReport()
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
        public ActionResult ExportACIReconsileData(IntegrationParam model)
        {
            #region Access Control

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

            #endregion

            ResultVM rVM = new ResultVM();

            try
            {

                DataSet ds = new DataSet();

                ds = _repo.ACIReconsileData(model.FromDate, model.ToDate);

                var sheetNames = new[] { "Product Wise", "Invoice Wise", "Unprocess" };

                var vm = OrdinaryVATDesktop.DownloadExcelMultiple(ds, "Sales Reconsile Data", sheetNames);

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


        #endregion


        [Authorize]
        [HttpGet]
        public ActionResult YearlySale()
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
            SaleMISViewModel vm = new SaleMISViewModel();
            return PartialView("_printYearlySale", vm);
        }
        [Authorize]
        [HttpGet]
        public ActionResult MonthlySale()
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
            SaleMISViewModel vm = new SaleMISViewModel();
            vm.FontSize = 8;
            return PartialView("_printMonthlySale", vm);
        }


        [Authorize]
        public ActionResult DownloadYearlySale(SaleMISViewModel paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            MISReportRepo _repo = new MISReportRepo(identity, Session);
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);
                MISReport _reportClass = new MISReport();
                ReportDSRepo repo = new ReportDSRepo(identity, Session);

                #region Comments

                //string PeriodName = Convert.ToDateTime(paramVM.DateFrom).ToString("MMMM-yyyy");

                //string[] vValues = { PeriodName };
                //string[] vFields = { "PeriodName" };
                //FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                //varFiscalYearVM = new FiscalYearRepo(identity, Session).SelectAll(0, vFields, vValues).FirstOrDefault();

                //string PeriodName1 = Convert.ToDateTime(paramVM.DateTo).ToString("MMMM-yyyy");
                //string[] vValue = { PeriodName1 };
                //string[] vField = { "PeriodName" };
                //FiscalYearVM FiscalYearVM = new FiscalYearVM();
                //FiscalYearVM = new FiscalYearRepo(identity, Session).SelectAll(0, vField, vValue).FirstOrDefault();

                //string DateFrom = Convert.ToDateTime(paramVM.DateFrom).ToString("yyyy-MM-") + "01";
                //string DateTo = Convert.ToDateTime(paramVM.DateTo).ToString("yyyy-MM-") + "01";
                //paramVM.DateFrom = DateFrom;
                //paramVM.DateTo = DateTo;

                #endregion

                if (paramVM.ReportType == "Preview")
                {

                    paramVM.reportName = "Sale Yearly";

                    OrdinaryVATDesktop.FontSize = paramVM.FontSize.ToString();

                    reportDocument = _reportClass.YearlySalesData(paramVM);

                    var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                    return new FileStreamResult(stream, "application/pdf");

                }
                else
                {

                    #region Excel Download

                    DataTable dtResult = _repo.GetYearlySaleData(paramVM);
                    ReportDSRepo report = new ReportDSRepo();
                    DataSet dsResult = report.ComapnyProfile("");
                    string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                    string fileDirectory = pathRoot + "//Excel Files";
                    if (!Directory.Exists(fileDirectory))
                    {
                        Directory.CreateDirectory(fileDirectory);
                    }

                    fileDirectory += "\\" + "Sale_Yearly" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";

                    ExcelPackage excel = new ExcelPackage();

                    ExcelWorksheet ws = excel.Workbook.Worksheets.Add("SaleYearly");

                    ws.Cells[1, 1, 1, dtResult.Columns.Count].Merge = true;
                    ws.Cells[1, 1, 1, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[1, 1, 1, dtResult.Columns.Count].Style.Font.Size = 14;

                    ws.Cells[2, 1, 2, dtResult.Columns.Count].Merge = true;
                    ws.Cells[2, 1, 2, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[2, 1, 2, dtResult.Columns.Count].Style.Font.Size = 14;

                    ws.Cells[3, 1, 3, dtResult.Columns.Count].Merge = true;
                    ws.Cells[3, 1, 3, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[3, 1, 3, dtResult.Columns.Count].Style.Font.Size = 14;

                    ws.Cells[4, 1, 4, dtResult.Columns.Count].Merge = true;
                    ws.Cells[4, 1, 4, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[4, 1, 4, dtResult.Columns.Count].Style.Font.Size = 14;

                    ws.Cells[6, 2, 6, 4].Merge = true;
                    ws.Cells[6, 2, 6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[6, 2, 6, 4].Style.Font.Size = 14;

                    ws.Cells[6, 5, 6, 7].Merge = true;
                    ws.Cells[6, 5, 6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[6, 5, 6, 7].Style.Font.Size = 14;

                    ws.Cells[1, 1].LoadFromText("Company Name: " + dsResult.Tables[0].Rows[0]["CompanyLegalName"]);
                    ws.Cells[2, 1].Value = "Address: " + dsResult.Tables[0].Rows[0]["Address1"];

                    if (string.IsNullOrEmpty(paramVM.ProductName))
                    {
                        ws.Cells[3, 1].LoadFromText("Product Name: All Product");

                    }
                    else
                    {
                        ws.Cells[3, 1].LoadFromText("Product Name: " + paramVM.ProductName);
                    }
                    ws.Cells[4, 1].LoadFromText("Sale Yearly");

                    ws.Cells[6, 2].LoadFromText("Year - " + (Convert.ToDecimal(paramVM.DateFrom) - 1).ToString() + "-" + paramVM.DateFrom);
                    ws.Cells[6, 5].LoadFromText("Year - " + (Convert.ToDecimal(paramVM.DateTo) - 1).ToString() + "-" + paramVM.DateTo);

                    ws.Cells["A7"].LoadFromDataTable(dtResult, true);

                    if (dtResult.Rows.Count > 0)
                    {
                        ws.Cells["A7:A" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["A7:A" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                        ws.Cells["B7:D" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["B7:D" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);

                        ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                    }

                    ws.Column(2).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(3).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(4).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(5).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(6).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(7).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(8).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(9).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(10).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                    string filename = "Sale_Yearly" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                        excel.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }

                    #endregion

                }


            }
            catch (Exception)
            {


            }
            finally
            {
                if (reportDocument != null)
                {
                    reportDocument.Close();
                    reportDocument.Dispose();
                }
            }

            return RedirectToAction("Index");

        }

        public ActionResult DownloadMonthlySale(SaleMISViewModel paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            MISReportRepo _repo = new MISReportRepo(identity, Session);


            try
            {

                #region Get period

                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();

                string[] vValues = { paramVM.DateFrom };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearRepo(identity, Session).SelectAll(0, vFields, vValues).FirstOrDefault();
                string FirstPriodID = "";
                string SecondPriodID = "";
                if (varFiscalYearVM != null)
                {
                    FirstPriodID = varFiscalYearVM.PeriodID;
                }

                string[] vValue = { paramVM.DateTo };
                string[] vField = { "PeriodName" };
                FiscalYearVM FiscalYearVM = new FiscalYearVM();
                FiscalYearVM = new FiscalYearRepo(identity, Session).SelectAll(0, vField, vValue).FirstOrDefault();
                if (FiscalYearVM != null)
                {
                    SecondPriodID = FiscalYearVM.PeriodID;
                }

                #endregion


                if (paramVM.ReportType == "Preview")
                {
                    paramVM.reportName = "Monthly Sale";

                    OrdinaryVATDesktop.FontSize = paramVM.FontSize.ToString();

                    reportDocument = _reportClass.MonthlySalesData(paramVM, varFiscalYearVM.PeriodName, FiscalYearVM.PeriodName, FirstPriodID, SecondPriodID);

                    var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                    return new FileStreamResult(stream, "application/pdf");

                }
                else
                {

                    #region Excel download

                    DataSet dtResult = _repo.GetMonthlySaleData(paramVM, FirstPriodID, SecondPriodID);

                    dt = dtResult.Tables[0];
                    dt1 = dtResult.Tables[1];
                    if (dt.Rows.Count < 1)
                    {
                        dt.Rows.Add(dt.NewRow());
                    }
                    if (dt1.Rows.Count < 1)
                    {
                        dt1.Rows.Add(dt1.NewRow());
                    }

                    ReportDSRepo report = new ReportDSRepo();
                    DataSet dsResult = report.ComapnyProfile("");
                    string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                    string fileDirectory = pathRoot + "//Excel Files";
                    if (!Directory.Exists(fileDirectory))
                    {
                        Directory.CreateDirectory(fileDirectory);
                    }

                    fileDirectory += "\\" + "Sale_Monthly" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";

                    ExcelPackage excel = new ExcelPackage();

                    ExcelWorksheet ws = excel.Workbook.Worksheets.Add("SaleMonthly");

                    ws.Cells[1, 1, 1, 8].Merge = true;
                    ws.Cells[1, 1, 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[1, 1, 1, 8].Style.Font.Size = 14;

                    ws.Cells[2, 1, 2, 8].Merge = true;
                    ws.Cells[2, 1, 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[2, 1, 2, 8].Style.Font.Size = 14;

                    ws.Cells[3, 1, 3, 8].Merge = true;
                    ws.Cells[3, 1, 3, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[3, 1, 3, 8].Style.Font.Size = 14;

                    ws.Cells[4, 1, 4, 8].Merge = true;
                    ws.Cells[4, 1, 4, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[4, 1, 4, 8].Style.Font.Size = 14;

                    ws.Cells[6, 1, 6, 3].Merge = true;
                    ws.Cells[6, 1, 6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[6, 1, 6, 3].Style.Font.Size = 14;

                    ws.Cells[6, 4, 6, 6].Merge = true;
                    ws.Cells[6, 4, 6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[6, 4, 6, 6].Style.Font.Size = 14;

                    ws.Cells[1, 1].LoadFromText("Company Name: " + dsResult.Tables[0].Rows[0]["CompanyLegalName"]);
                    ws.Cells[2, 1].Value = "Address: " + dsResult.Tables[0].Rows[0]["Address1"];

                    if (string.IsNullOrEmpty(paramVM.ProductName))
                    {
                        ws.Cells[3, 1].LoadFromText("Product Name: All Product");

                    }
                    else
                    {
                        ws.Cells[3, 1].LoadFromText("Product Name: " + paramVM.ProductName);
                    }

                    ws.Cells[4, 1].LoadFromText("Sale Monthly");

                    dt.Columns.Remove("Month");

                    //ws.Cells["A7:C" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    //ws.Cells["A7:C" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    ws.Cells["A7"].LoadFromDataTable(dt, true);

                    ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                    if (dt.Rows.Count > 0)
                    {

                        ws.Cells["A7:C" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["A7:C" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                        ////////ws.Cells["B7:C" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ////////ws.Cells["B7:C" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);

                    }
                    dt1.Columns.Remove("Month");

                    ws.Cells["D7"].LoadFromDataTable(dt1, true);

                    if (dt.Rows.Count > 0)
                    {

                        //////ws.Cells["D7:E" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        //////ws.Cells["D7:E" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                        ws.Cells["D7:F" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["D7:F" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                    }

                    if (varFiscalYearVM != null)
                    {
                        ws.Cells[6, 1].LoadFromText("'" + varFiscalYearVM.PeriodName.ToString());
                    }
                    if (FiscalYearVM != null)
                    {
                        ws.Cells[6, 4].LoadFromText("'" + FiscalYearVM.PeriodName.ToString());
                    }

                    ws.Column(1).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(2).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(3).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(4).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(5).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(6).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(7).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(8).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(9).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(10).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                    string filename = "Sale_Monthly" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                        excel.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }

                    #endregion

                }
            }
            catch (Exception)
            {


            }
            finally { }
            return RedirectToAction("Index");
        }


        [Authorize]
        [HttpGet]
        public ActionResult YearlyPurchase()
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
            PurchaseMISViewModel vm = new PurchaseMISViewModel();
            return PartialView("_printYearlyPurchase", vm);
        }
        [Authorize]
        [HttpGet]
        public ActionResult MonthlyPurchase()
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
            PurchaseMISViewModel vm = new PurchaseMISViewModel();
            return PartialView("_printMonthlyPurchase", vm);
        }


        [Authorize]
        public ActionResult DownloadYearlyPurchase(PurchaseMISViewModel paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            MISReportRepo _repo = new MISReportRepo(identity, Session);

            try
            {

                DataTable dtResult = _repo.GetYearlyPurchaseData(paramVM);

                if (paramVM.ReportType == "Preview")
                {
                    paramVM.reportName = "Yearly Purchase";

                    OrdinaryVATDesktop.FontSize = paramVM.FontSize.ToString();

                    reportDocument = _reportClass.YearlyPurchaseData(paramVM);

                    var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                    return new FileStreamResult(stream, "application/pdf");

                }
                else
                {
                    //string PeriodName = Convert.ToDateTime(paramVM.ReceiveDateFrom).ToString("MMMM-yyyy");
                    //string[] vValues = { PeriodName };
                    //string[] vFields = { "PeriodName" };
                    //FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                    //varFiscalYearVM = new FiscalYearRepo(identity, Session).SelectAll(0, vFields, vValues).FirstOrDefault();

                    //string PeriodName1 = Convert.ToDateTime(paramVM.ReceiveDateTo).ToString("MMMM-yyyy");
                    //string[] vValue = { PeriodName1 };
                    //string[] vField = { "PeriodName" };
                    //FiscalYearVM FiscalYearVM = new FiscalYearVM();
                    //FiscalYearVM = new FiscalYearRepo(identity, Session).SelectAll(0, vField, vValue).FirstOrDefault();
                    //string DateFrom = Convert.ToDateTime(paramVM.ReceiveDateFrom).ToString("yyyy-MM-") + "01";
                    //string DateTo = Convert.ToDateTime(paramVM.ReceiveDateTo).ToString("yyyy-MM-") + "01";
                    //paramVM.ReceiveDateFrom = DateFrom;
                    //paramVM.ReceiveDateTo = DateTo;
                    #region excle
                    ReportDSRepo report = new ReportDSRepo();
                    DataSet dsResult = report.ComapnyProfile("");
                    string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                    string fileDirectory = pathRoot + "//Excel Files";
                    if (!Directory.Exists(fileDirectory))
                    {
                        Directory.CreateDirectory(fileDirectory);
                    }

                    fileDirectory += "\\" + "Purchase_Yearly" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";

                    ExcelPackage excel = new ExcelPackage();

                    ExcelWorksheet ws = excel.Workbook.Worksheets.Add("SaleYearly");

                    ws.Cells[1, 1, 1, dtResult.Columns.Count].Merge = true;
                    ws.Cells[1, 1, 1, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[1, 1, 1, dtResult.Columns.Count].Style.Font.Size = 14;

                    ws.Cells[2, 1, 2, dtResult.Columns.Count].Merge = true;
                    ws.Cells[2, 1, 2, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[2, 1, 2, dtResult.Columns.Count].Style.Font.Size = 14;

                    ws.Cells[3, 1, 3, dtResult.Columns.Count].Merge = true;
                    ws.Cells[3, 1, 3, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[3, 1, 3, dtResult.Columns.Count].Style.Font.Size = 14;

                    ws.Cells[4, 1, 4, dtResult.Columns.Count].Merge = true;
                    ws.Cells[4, 1, 4, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[4, 1, 4, dtResult.Columns.Count].Style.Font.Size = 14;


                    ws.Cells[6, 2, 6, 4].Merge = true;
                    ws.Cells[6, 2, 6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[6, 2, 6, 4].Style.Font.Size = 14;

                    ws.Cells[6, 5, 6, 7].Merge = true;
                    ws.Cells[6, 5, 6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[6, 5, 6, 7].Style.Font.Size = 14;




                    ws.Cells[1, 1].LoadFromText("Company Name: " + dsResult.Tables[0].Rows[0]["CompanyLegalName"]);
                    ws.Cells[2, 1].Value = "Address: " + dsResult.Tables[0].Rows[0]["Address1"];

                    if (string.IsNullOrEmpty(paramVM.ProductName))
                    {
                        ws.Cells[3, 1].LoadFromText("Product Name: All Product");

                    }
                    else
                    {
                        ws.Cells[3, 1].LoadFromText("Product Name: " + paramVM.ProductName);
                    }
                    ws.Cells[4, 1].LoadFromText("Purchase Yearly");

                    ws.Cells[6, 2].LoadFromText("Year - " + (Convert.ToDecimal(paramVM.ReceiveDateFrom) - 1).ToString() + "-" + paramVM.ReceiveDateFrom);
                    ws.Cells[6, 5].LoadFromText("Year - " + (Convert.ToDecimal(paramVM.ReceiveDateTo) - 1).ToString() + "-" + paramVM.ReceiveDateTo);



                    ws.Cells["A7"].LoadFromDataTable(dtResult, true);

                    if (dtResult.Rows.Count > 0)
                    {
                        ws.Cells["A7:A" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["A7:A" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                        ws.Cells["B7:D" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["B7:D" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);

                        ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                    }





                    ws.Column(2).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(3).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(4).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(5).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(6).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(7).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(8).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(9).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(10).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";





                    string filename = "Purchase_Yearly" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

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
            }
                    #endregion
            catch (Exception)
            {


            }
            finally { }
            return RedirectToAction("Index");
        }

        public ActionResult DownloadMonthlyPurchase(PurchaseMISViewModel paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            MISReportRepo _repo = new MISReportRepo(identity, Session);

            try
            {
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();

                string[] vValues = { paramVM.ReceiveDateFrom };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearRepo(identity, Session).SelectAll(0, vFields, vValues).FirstOrDefault();
                string FirstPriodID = "";
                string SecondPriodID = "";
                if (varFiscalYearVM != null)
                {
                    FirstPriodID = varFiscalYearVM.PeriodID;
                }

                string[] vValue = { paramVM.ReceiveDateTo };
                string[] vField = { "PeriodName" };
                FiscalYearVM FiscalYearVM = new FiscalYearVM();
                FiscalYearVM = new FiscalYearRepo(identity, Session).SelectAll(0, vField, vValue).FirstOrDefault();
                if (FiscalYearVM != null)
                {
                    SecondPriodID = FiscalYearVM.PeriodID;
                }

                if (paramVM.ReportType == "Preview")
                {
                    paramVM.reportName = "Monthly Purchase";

                    OrdinaryVATDesktop.FontSize = paramVM.FontSize.ToString();

                    reportDocument = _reportClass.MonthlyPurchaseData(paramVM, varFiscalYearVM.PeriodName, FiscalYearVM.PeriodName, FirstPriodID, SecondPriodID);

                    var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                    return new FileStreamResult(stream, "application/pdf");

                }

                else
                {
                    DataSet dtResult = _repo.GetMonthlyPurchaseData(paramVM, FirstPriodID, SecondPriodID);

                    dt = dtResult.Tables[0];
                    dt1 = dtResult.Tables[1];
                    if (dt.Rows.Count < 1)
                    {
                        dt.Rows.Add(dt.NewRow());
                    }
                    if (dt1.Rows.Count < 1)
                    {
                        dt1.Rows.Add(dt1.NewRow());
                    }
                    ReportDSRepo report = new ReportDSRepo();
                    DataSet dsResult = report.ComapnyProfile("");
                    string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                    string fileDirectory = pathRoot + "//Excel Files";
                    if (!Directory.Exists(fileDirectory))
                    {
                        Directory.CreateDirectory(fileDirectory);
                    }

                    fileDirectory += "\\" + "Purchse_Monthly" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";

                    ExcelPackage excel = new ExcelPackage();

                    ExcelWorksheet ws = excel.Workbook.Worksheets.Add("SaleMonthly");

                    ws.Cells[1, 1, 1, 8].Merge = true;
                    ws.Cells[1, 1, 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[1, 1, 1, 8].Style.Font.Size = 14;

                    ws.Cells[2, 1, 2, 8].Merge = true;
                    ws.Cells[2, 1, 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[2, 1, 2, 8].Style.Font.Size = 14;

                    ws.Cells[3, 1, 3, 8].Merge = true;
                    ws.Cells[3, 1, 3, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[3, 1, 3, 8].Style.Font.Size = 14;

                    ws.Cells[4, 1, 4, 8].Merge = true;
                    ws.Cells[4, 1, 4, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[4, 1, 4, 8].Style.Font.Size = 14;


                    ws.Cells[6, 1, 6, 3].Merge = true;
                    ws.Cells[6, 1, 6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[6, 1, 6, 3].Style.Font.Size = 14;

                    ws.Cells[6, 4, 6, 6].Merge = true;
                    ws.Cells[6, 4, 6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[6, 4, 6, 6].Style.Font.Size = 14;




                    ws.Cells[1, 1].LoadFromText("Company Name: " + dsResult.Tables[0].Rows[0]["CompanyLegalName"]);
                    ws.Cells[2, 1].Value = "Address: " + dsResult.Tables[0].Rows[0]["Address1"];


                    if (string.IsNullOrEmpty(paramVM.ProductName))
                    {
                        ws.Cells[3, 1].LoadFromText("Product Name: All Product");

                    }
                    else
                    {
                        ws.Cells[3, 1].LoadFromText("Product Name: " + paramVM.ProductName);
                    }

                    ws.Cells[4, 1].LoadFromText("Purchse Monthly");




                    dt.Columns.Remove("Month");

                    ws.Cells["A7"].LoadFromDataTable(dt, true);




                    ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                    if (dt.Rows.Count > 0)
                    {

                        ws.Cells["A7:C" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["A7:C" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                        //ws.Cells["B7:C" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        //ws.Cells["B7:C" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);

                    }
                    dt1.Columns.Remove("Month");

                    ws.Cells["D7"].LoadFromDataTable(dt1, true);

                    if (dt1.Rows.Count > 0)
                    {

                        //ws.Cells["D7:E" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        //ws.Cells["D7:E" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                        ws.Cells["D7:F" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["D7:F" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                    }
                    if (varFiscalYearVM != null)
                    {
                        ws.Cells[6, 1].LoadFromText("'" + varFiscalYearVM.PeriodName.ToString());
                    }
                    if (FiscalYearVM != null)
                    {
                        ws.Cells[6, 4].LoadFromText("'" + FiscalYearVM.PeriodName.ToString());
                    }

                    ws.Column(1).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(2).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(3).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(4).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(5).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(6).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(7).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(8).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(9).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                    ws.Column(10).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";





                    string filename = "Purchse_Monthly" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

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
            }
            catch (Exception ex)
            {
                throw;

            }
            finally { }
            return RedirectToAction("Index");
        }


        [Authorize]
        [HttpGet]
        public ActionResult TransferIssue()
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
            TransferMISViewModel vm = new TransferMISViewModel();
            return PartialView("_printTransferissue", vm);
        }
        [Authorize]
        [HttpGet]
        public ActionResult TransferReceive()
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
            TransferMISViewModel vm = new TransferMISViewModel();
            return PartialView("_printTransferReceive", vm);
        }
        [Authorize]
        public ActionResult DownloadTransferIssue(TransferMISViewModel paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            MISReportRepo _repo = new MISReportRepo(identity, Session);

            try
            {
                if (!string.IsNullOrWhiteSpace(paramVM.DateFrom))
                {
                    paramVM.DateFrom = Convert.ToDateTime(paramVM.DateFrom).ToString("yyyy-MM-dd HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(paramVM.DateTo))
                {
                    //DateTo = Convert.ToDateTime(vm.DateTo).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                    paramVM.DateTo = Convert.ToDateTime(paramVM.DateTo).ToString("yyyy-MM-dd 23:59:59");
                }
                DataTable dtResult = _repo.GetTransferIssueData(paramVM);
                ReportDSRepo report = new ReportDSRepo();
                DataSet dsResult = report.ComapnyProfile("");
                string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                string fileDirectory = pathRoot + "//Excel Files";
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                dtResult.Columns.Remove("BranchId");

                fileDirectory += "\\" + "Transfer_Issue" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";

                ExcelPackage excel = new ExcelPackage();

                ExcelWorksheet ws = excel.Workbook.Worksheets.Add("Transfer_Issue");

                ws.Cells[1, 1, 1, 16].Merge = true;
                ws.Cells[1, 1, 1, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, 1, 1, 16].Style.Font.Size = 14;

                ws.Cells[2, 1, 2, 16].Merge = true;
                ws.Cells[2, 1, 2, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[2, 1, 2, 16].Style.Font.Size = 14;

                ws.Cells[3, 1, 3, 16].Merge = true;
                ws.Cells[3, 1, 3, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[3, 1, 3, 16].Style.Font.Size = 14;

                ws.Cells[4, 1, 4, 16].Merge = true;
                ws.Cells[4, 1, 4, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[4, 1, 4, 16].Style.Font.Size = 14;

                ws.Cells[5, 1, 5, 16].Merge = true;
                ws.Cells[5, 1, 5, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[5, 1, 5, 16].Style.Font.Size = 14;

                //ws.Cells[6, 2, 6, 4].Merge = true;
                //ws.Cells[6, 2, 6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //ws.Cells[6, 2, 6, 4].Style.Font.Size = 14;

                //ws.Cells[6, 5, 6, 7].Merge = true;
                //ws.Cells[6, 5, 6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //ws.Cells[6, 5, 6, 7].Style.Font.Size = 14;




                ws.Cells[1, 1].LoadFromText("Company Name: " + dsResult.Tables[0].Rows[0]["CompanyLegalName"]);
                ws.Cells[2, 1].Value = "Address: " + dsResult.Tables[0].Rows[0]["Address1"];

                ws.Cells[3, 1].LoadFromText("Transfer from Central to All Depo (Transfer Issue)");
                if (string.IsNullOrEmpty(paramVM.ProductName))
                {
                    ws.Cells[4, 1].LoadFromText("Product Name: All Product");
                }
                else
                {
                    ws.Cells[4, 1].LoadFromText("Product Name: " + paramVM.ProductName);
                }

                ws.Cells[5, 1].LoadFromText("From Date: " + ToDateString(paramVM.DateFrom, "dd-MMM-yyyy") + "      To Date: " + ToDateString(paramVM.DateTo, "dd-MMM-yyyy"));


                //ws.Cells[6, 2].LoadFromText("Year - " + (Convert.ToDecimal(varFiscalYearVM.CurrentYear) - 1).ToString() + "-" + varFiscalYearVM.CurrentYear);
                //ws.Cells[6, 5].LoadFromText("Year - " + (Convert.ToDecimal(FiscalYearVM.CurrentYear) - 1).ToString() + "-" + FiscalYearVM.CurrentYear);



                ws.Cells["A7"].LoadFromDataTable(dtResult, true);

                //if (dtResult.Rows.Count > 0)
                //{
                //    ws.Cells["A7:A" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    ws.Cells["A7:A" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                //    ws.Cells["B7:D" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    ws.Cells["B7:D" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);

                //    ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                //}





                //ws.Column(2).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(3).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(4).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(5).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(6).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(7).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(8).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(9).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(10).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";





                string filename = "Transfer_Issue" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

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

        [Authorize]
        public ActionResult DownloadDetailsTransferIssue(TransferMISViewModel paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            MISReportRepo _repo = new MISReportRepo(identity, Session);

            try
            {
                if (!string.IsNullOrWhiteSpace(paramVM.DateFrom))
                {
                    paramVM.DateFrom = Convert.ToDateTime(paramVM.DateFrom).ToString("yyyy-MM-dd HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(paramVM.DateTo))
                {
                    //DateTo = Convert.ToDateTime(vm.DateTo).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                    paramVM.DateTo = Convert.ToDateTime(paramVM.DateTo).ToString("yyyy-MM-dd 23:59:59");
                }
                DataTable dtResult = _repo.GetTransferIssueDetailsData(paramVM);
                ReportDSRepo report = new ReportDSRepo();
                DataSet dsResult = report.ComapnyProfile("");
                string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                string fileDirectory = pathRoot + "//Excel Files";
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                dtResult.Columns.Remove("BranchId");

                fileDirectory += "\\" + "Transfer_Issue" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";

                ExcelPackage excel = new ExcelPackage();

                ExcelWorksheet ws = excel.Workbook.Worksheets.Add("Transfer_Issue");

                ws.Cells[1, 1, 1, 16].Merge = true;
                ws.Cells[1, 1, 1, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, 1, 1, 16].Style.Font.Size = 14;

                ws.Cells[2, 1, 2, 16].Merge = true;
                ws.Cells[2, 1, 2, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[2, 1, 2, 16].Style.Font.Size = 14;

                ws.Cells[3, 1, 3, 16].Merge = true;
                ws.Cells[3, 1, 3, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[3, 1, 3, 16].Style.Font.Size = 14;

                ws.Cells[4, 1, 4, 16].Merge = true;
                ws.Cells[4, 1, 4, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[4, 1, 4, 16].Style.Font.Size = 14;

                ws.Cells[5, 1, 5, 16].Merge = true;
                ws.Cells[5, 1, 5, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[5, 1, 5, 16].Style.Font.Size = 14;

                //ws.Cells[6, 2, 6, 4].Merge = true;
                //ws.Cells[6, 2, 6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //ws.Cells[6, 2, 6, 4].Style.Font.Size = 14;

                //ws.Cells[6, 5, 6, 7].Merge = true;
                //ws.Cells[6, 5, 6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //ws.Cells[6, 5, 6, 7].Style.Font.Size = 14;




                ws.Cells[1, 1].LoadFromText("Company Name: " + dsResult.Tables[0].Rows[0]["CompanyLegalName"]);
                ws.Cells[2, 1].Value = "Address: " + dsResult.Tables[0].Rows[0]["Address1"];

                ws.Cells[3, 1].LoadFromText("Transfer from Central to All Depo (Transfer Issue)");
                if (string.IsNullOrEmpty(paramVM.ProductName))
                {
                    ws.Cells[4, 1].LoadFromText("Product Name: All Product");
                }
                else
                {
                    ws.Cells[4, 1].LoadFromText("Product Name: " + paramVM.ProductName);
                }

                ws.Cells[5, 1].LoadFromText("From Date: " + ToDateString(paramVM.DateFrom, "dd-MMM-yyyy") + "      To Date: " + ToDateString(paramVM.DateTo, "dd-MMM-yyyy"));


                //ws.Cells[6, 2].LoadFromText("Year - " + (Convert.ToDecimal(varFiscalYearVM.CurrentYear) - 1).ToString() + "-" + varFiscalYearVM.CurrentYear);
                //ws.Cells[6, 5].LoadFromText("Year - " + (Convert.ToDecimal(FiscalYearVM.CurrentYear) - 1).ToString() + "-" + FiscalYearVM.CurrentYear);



                ws.Cells["A7"].LoadFromDataTable(dtResult, true);

                //if (dtResult.Rows.Count > 0)
                //{
                //    ws.Cells["A7:A" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    ws.Cells["A7:A" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                //    ws.Cells["B7:D" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    ws.Cells["B7:D" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);

                //    ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                //}





                //ws.Column(2).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(3).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(4).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(5).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(6).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(7).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(8).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(9).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(10).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";





                string filename = "Transfer_Issue" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

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

        public ActionResult DownloadTransferReceive(TransferMISViewModel paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            MISReportRepo _repo = new MISReportRepo(identity, Session);

            try
            {
                if (!string.IsNullOrWhiteSpace(paramVM.DateFrom))
                {
                    paramVM.DateFrom = Convert.ToDateTime(paramVM.DateFrom).ToString("yyyy-MM-dd HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(paramVM.DateTo))
                {
                    //DateTo = Convert.ToDateTime(vm.DateTo).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                    paramVM.DateTo = Convert.ToDateTime(paramVM.DateTo).ToString("yyyy-MM-dd 23:59:59");
                }
                DataTable dtResult = _repo.GetTransferReceiveData(paramVM);
                ReportDSRepo report = new ReportDSRepo();
                DataSet dsResult = report.ComapnyProfile("");
                string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                string fileDirectory = pathRoot + "//Excel Files";
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                dtResult.Columns.Remove("BranchId");

                fileDirectory += "\\" + "Transfer_Receive" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";

                ExcelPackage excel = new ExcelPackage();

                ExcelWorksheet ws = excel.Workbook.Worksheets.Add("Transfer_Receive");

                ws.Cells[1, 1, 1, 16].Merge = true;
                ws.Cells[1, 1, 1, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, 1, 1, 16].Style.Font.Size = 14;

                ws.Cells[2, 1, 2, 16].Merge = true;
                ws.Cells[2, 1, 2, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[2, 1, 2, 16].Style.Font.Size = 14;

                ws.Cells[3, 1, 3, 16].Merge = true;
                ws.Cells[3, 1, 3, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[3, 1, 3, 16].Style.Font.Size = 14;

                ws.Cells[4, 1, 4, 16].Merge = true;
                ws.Cells[4, 1, 4, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[4, 1, 4, 16].Style.Font.Size = 14;

                ws.Cells[5, 1, 5, 16].Merge = true;
                ws.Cells[5, 1, 5, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[5, 1, 5, 16].Style.Font.Size = 14;

                //ws.Cells[6, 2, 6, 4].Merge = true;
                //ws.Cells[6, 2, 6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //ws.Cells[6, 2, 6, 4].Style.Font.Size = 14;

                //ws.Cells[6, 5, 6, 7].Merge = true;
                //ws.Cells[6, 5, 6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //ws.Cells[6, 5, 6, 7].Style.Font.Size = 14;




                ws.Cells[1, 1].LoadFromText("Company Name: " + dsResult.Tables[0].Rows[0]["CompanyLegalName"]);
                ws.Cells[2, 1].Value = "Address: " + dsResult.Tables[0].Rows[0]["Address1"];

                ws.Cells[3, 1].LoadFromText("Transfer from Central to All Depo (Transfer Receive)");

                if (string.IsNullOrEmpty(paramVM.ProductName))
                {
                    ws.Cells[4, 1].LoadFromText("Product Name: All Product");
                }
                else
                {
                    ws.Cells[4, 1].LoadFromText("Product Name: " + paramVM.ProductName);
                }


                ws.Cells[5, 1].LoadFromText("From Date: " + ToDateString(paramVM.DateFrom, "dd-MMM-yyyy") + "      To Date: " + ToDateString(paramVM.DateTo, "dd-MMM-yyyy"));


                //ws.Cells[6, 2].LoadFromText("Year - " + (Convert.ToDecimal(varFiscalYearVM.CurrentYear) - 1).ToString() + "-" + varFiscalYearVM.CurrentYear);
                //ws.Cells[6, 5].LoadFromText("Year - " + (Convert.ToDecimal(FiscalYearVM.CurrentYear) - 1).ToString() + "-" + FiscalYearVM.CurrentYear);



                ws.Cells["A7"].LoadFromDataTable(dtResult, true);

                //if (dtResult.Rows.Count > 0)
                //{
                //    ws.Cells["A7:A" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    ws.Cells["A7:A" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                //    ws.Cells["B7:D" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    ws.Cells["B7:D" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);

                //    ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                //}





                //ws.Column(2).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(3).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(4).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(5).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(6).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(7).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(8).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(9).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(10).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";





                string filename = "Transfer_Receive" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

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

        public ActionResult DownloadDetailsTransferReceive(TransferMISViewModel paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            MISReportRepo _repo = new MISReportRepo(identity, Session);

            try
            {
                if (!string.IsNullOrWhiteSpace(paramVM.DateFrom))
                {
                    paramVM.DateFrom = Convert.ToDateTime(paramVM.DateFrom).ToString("yyyy-MM-dd HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(paramVM.DateTo))
                {
                    //DateTo = Convert.ToDateTime(vm.DateTo).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                    paramVM.DateTo = Convert.ToDateTime(paramVM.DateTo).ToString("yyyy-MM-dd 23:59:59");
                }
                DataTable dtResult = _repo.GetTransferReceiveDetailsData(paramVM);
                ReportDSRepo report = new ReportDSRepo();
                DataSet dsResult = report.ComapnyProfile("");
                string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                string fileDirectory = pathRoot + "//Excel Files";
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                dtResult.Columns.Remove("BranchId");

                fileDirectory += "\\" + "Transfer_Receive" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";

                ExcelPackage excel = new ExcelPackage();

                ExcelWorksheet ws = excel.Workbook.Worksheets.Add("Transfer_Receive");

                ws.Cells[1, 1, 1, 16].Merge = true;
                ws.Cells[1, 1, 1, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, 1, 1, 16].Style.Font.Size = 14;

                ws.Cells[2, 1, 2, 16].Merge = true;
                ws.Cells[2, 1, 2, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[2, 1, 2, 16].Style.Font.Size = 14;

                ws.Cells[3, 1, 3, 16].Merge = true;
                ws.Cells[3, 1, 3, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[3, 1, 3, 16].Style.Font.Size = 14;

                ws.Cells[4, 1, 4, 16].Merge = true;
                ws.Cells[4, 1, 4, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[4, 1, 4, 16].Style.Font.Size = 14;

                ws.Cells[5, 1, 5, 16].Merge = true;
                ws.Cells[5, 1, 5, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[5, 1, 5, 16].Style.Font.Size = 14;

                //ws.Cells[6, 2, 6, 4].Merge = true;
                //ws.Cells[6, 2, 6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //ws.Cells[6, 2, 6, 4].Style.Font.Size = 14;

                //ws.Cells[6, 5, 6, 7].Merge = true;
                //ws.Cells[6, 5, 6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //ws.Cells[6, 5, 6, 7].Style.Font.Size = 14;




                ws.Cells[1, 1].LoadFromText("Company Name: " + dsResult.Tables[0].Rows[0]["CompanyLegalName"]);
                ws.Cells[2, 1].Value = "Address: " + dsResult.Tables[0].Rows[0]["Address1"];

                ws.Cells[3, 1].LoadFromText("Transfer from Central to All Depo (Transfer Receive)");

                if (string.IsNullOrEmpty(paramVM.ProductName))
                {
                    ws.Cells[4, 1].LoadFromText("Product Name: All Product");
                }
                else
                {
                    ws.Cells[4, 1].LoadFromText("Product Name: " + paramVM.ProductName);
                }


                ws.Cells[5, 1].LoadFromText("From Date: " + ToDateString(paramVM.DateFrom, "dd-MMM-yyyy") + "      To Date: " + ToDateString(paramVM.DateTo, "dd-MMM-yyyy"));


                //ws.Cells[6, 2].LoadFromText("Year - " + (Convert.ToDecimal(varFiscalYearVM.CurrentYear) - 1).ToString() + "-" + varFiscalYearVM.CurrentYear);
                //ws.Cells[6, 5].LoadFromText("Year - " + (Convert.ToDecimal(FiscalYearVM.CurrentYear) - 1).ToString() + "-" + FiscalYearVM.CurrentYear);



                ws.Cells["A7"].LoadFromDataTable(dtResult, true);

                //if (dtResult.Rows.Count > 0)
                //{
                //    ws.Cells["A7:A" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    ws.Cells["A7:A" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                //    ws.Cells["B7:D" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    ws.Cells["B7:D" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);

                //    ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    ws.Cells["E7:G" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                //}





                //ws.Column(2).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(3).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(4).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(5).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(6).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(7).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(8).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(9).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                //ws.Column(10).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";





                string filename = "Transfer_Receive" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

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

        [Authorize]
        public ActionResult ProductReportView(ProductReportViewModel vm)
        {
            try
            {
                if (vm.ProductName == null)
                {
                    vm.ProductName = "";
                }
                if (vm.ProductCode == null || vm.ProductCode == "0")
                {
                    vm.ProductCode = "";
                }
                if (vm.ProductGroup == null)
                {
                    vm.ProductGroup = "";
                }
                if (vm.ProductType == null)
                {
                    vm.ProductType = "";
                }

                ProductReport _reportClass = new ProductReport();
                bool PreviewOnly = true;
                reportDocument = _reportClass.ProductNew(vm.ItemNo, vm.ProductGroup, vm.ProductType, connVM, vm.ProductCode);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        [Authorize]
        public ActionResult BranchReportView()
        {
            try
            {
                List<string> BranchIdList = new List<string>();
                MISReport _reportClass = new MISReport();

                reportDocument = _reportClass.BranchListReport(BranchIdList, connVM);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception e)
            {
                throw e;
            }

        }


        [Authorize]
        //[HttpPost]
        public ActionResult MegnaCA16(ReportParamVM vm)
        {
            ReportDocument reportDocument = new ReportDocument();

            try
            {


                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);


                MISReport _reportClass = new MISReport();

                //if (string.IsNullOrWhiteSpace(vm.SalesInvoiceNo))
                //{
                //    vm.SalesInvoiceNo = vm.InvoiceNo;
                //}




                reportDocument = _reportClass.MegnaCA16Report(vm.Id, connVM);




                if (reportDocument == null)
                {
                    Session["result"] = "Fail" + "~" + "There is no data to preview";

                    return Redirect("/Vms/MISReport");
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


    }
}
