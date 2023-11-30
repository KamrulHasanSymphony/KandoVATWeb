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
using System.Configuration;
using SymVATWebUI.Filters;
using VATServer.Ordinary;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class IssueHeaderController : Controller
    {
        //
        // GET: /Vms/FinancialTransaction/

        IssueHeaderRepo _repo = null;
        ShampanIdentity identity = null;

        public IssueHeaderController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new IssueHeaderRepo(identity);
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

        #region Index and _index
        [Authorize(Roles = "Admin")]
        public ActionResult Index(IssueMasterVM paramVM, string transactionType)
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


            if (string.IsNullOrWhiteSpace(paramVM.transactionType))
            {
                paramVM.transactionType = "Other";
            }
            //Session["dtFrom"] = dtFrom;
            //Session["dtTo"] = dtTo;

            //IssueMasterVM vm = new IssueMasterVM();
            //vm.transactionType = tType;
            paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());

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
        public ActionResult _index(JQueryDataTableParamVM param, IssueMasterVM paramVM)
        {
            _repo = new IssueHeaderRepo(identity, Session);

            List<IssueMasterVM> getAllData = new List<IssueMasterVM>();
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
            string dtTo = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            //if (!string.IsNullOrWhiteSpace(Session["Branch"] as string))
            //{
            //    searchedBranchId = Session["Branch"].ToString();
            //}
            if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTimeFrom))
            {
                dtFrom = Convert.ToDateTime(paramVM.IssueDateTimeFrom).ToString("yyyyMMdd");
            }
            if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTimeTo))
            {

                dtTo = Convert.ToDateTime(paramVM.IssueDateTimeTo).AddDays(1).ToString("yyyyMMdd");
            }

            //if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTimeFrom))
            //{
            //    dtFrom = paramVM.IssueDateTimeFrom;
            //}
            //if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTimeTo))
            //{
            //    dtTo = paramVM.IssueDateTimeTo;
            //}


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
            if (!string.IsNullOrWhiteSpace(paramVM.IssueNo) || !string.IsNullOrWhiteSpace(paramVM.ImportId))
            {
                dtFrom = "";
                dtTo = "";
            }

            #endregion SeachParameters


            if (!identity.IsAdmin)
            {
                string[] conditionFields = { "IssueDateTime>=", "IssueDateTime<=", "TransactionType", "IssueNo like", "Post", "BranchId", "SelectTop", "ImportIDExcel like" };
                string[] conditionValues = { dtFrom, dtTo, paramVM.transactionType, paramVM.IssueNo, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SelectTop, paramVM.ImportId };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            else
            {
                string[] conditionFields = { "IssueDateTime>=", "IssueDateTime<=", "TransactionType", "IssueNo like", "Post", "BranchId", "SelectTop", "ImportIDExcel like" };
                string[] conditionValues = { dtFrom, dtTo, paramVM.transactionType, paramVM.IssueNo, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SelectTop, paramVM.ImportId };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            #endregion
            #region Search and Filter Data
            IEnumerable<IssueMasterVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Id
                //IssueNo
                //IssueDateTime
                //TotalVATAmount
                //TotalAmount
                //SerialNo
                //Post

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.IssueNo.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.IssueDateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.TotalVATAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.TotalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.SerialNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<IssueMasterVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.IssueNo :
                                                           sortColumnIndex == 2 && isSortable_2 ? Ordinary.DateToString(c.IssueDateTime) :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.TotalVATAmount.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.TotalAmount.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.SerialNo.ToString() :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Post.ToString() :
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
                , c.IssueNo
                , c.IssueDateTime
                , c.TotalVATAmount.ToString()
                , c.TotalAmount.ToString()             
                , c.SerialNo.ToString()               
                , c.Post=="Y" ? "Posted" : "Not Posted"
                ,c.ImportId              
                ,c.transactionType              
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


        #endregion

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(IssueDetailVM vm)
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
            vm.FinishItemNo = "N/A";
            vm.FinishItemName = "N/A";
            //vm.WQty = vm.Wastage * vm.UOMc;

            ProductRepo PRepo = new ProductRepo(identity, Session);
            string BOMId = null;
            var dtbom = PRepo.SelectBOMRaw(vm.ItemNo, DateTime.Parse(vm.IssueDateTime).ToString("yyyy-MMM-dd"));

            if (dtbom != null && dtbom.Rows.Count > 0)
            {
                BOMId = dtbom.Rows[0]["BOMId"].ToString();
            }
            if (string.IsNullOrEmpty(BOMId))
            {
                vm.BOMId = 0;
            }
            else
            {
                vm.BOMId = Convert.ToInt32(BOMId);
            }

            return PartialView("_detail", vm);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create(string tType)
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
            IssueMasterVM vm = new IssueMasterVM();

            List<IssueDetailVM> IssueDetailVMs = new List<IssueDetailVM>();
            vm.Details = IssueDetailVMs;
            vm.Operation = "add";
            vm.transactionType = tType;
            vm.IssueDateTime = Session["SessionDate"].ToString();
            vm.ProductType = "Raw";
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(IssueMasterVM vm)
        {
            _repo = new IssueHeaderRepo(identity, Session);
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
                    vm.Post = "N";
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                    result = _repo.IssueInsert(vm);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[4], TransactionType = vm.transactionType });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);


                    var check = vm.Id;
                    result = _repo.IssueUpdate(vm, identity.UserId);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.transactionType });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
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
        public ActionResult Edit(string id, string TransactionType)
        {
            _repo = new IssueHeaderRepo(identity, Session);
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

            IssueMasterVM vm = new IssueMasterVM();

            string[] conditionFields = new string[] { "TransactionType" };
            string[] conditionValues = new string[] { TransactionType };

            vm = _repo.SelectAll(Convert.ToInt32(id), conditionFields, conditionValues).FirstOrDefault();

            if (vm == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<IssueDetailVM> IssueDetailVMs = new List<IssueDetailVM>();

            IssueDetailVMs = _repo.SelectIssueDetail(vm.IssueNo);

            vm.Details = IssueDetailVMs;
            vm.Operation = "update";
            vm.ProductType = "Raw";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Posted(string id)
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
            IssueMasterVM vm = new IssueMasterVM();
            //vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            List<IssueDetailVM> IssueDetailVMs = new List<IssueDetailVM>();

            //IssueDetailVMs = _detailRepo.SelectByMasterId(Convert.ToInt32(id));

            vm.Details = IssueDetailVMs;

            vm.Operation = "posted";
            return View("Posted", vm);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult VoucherReportView(string id)
        {
            try
            {
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                IssueDetailVM vm = new IssueDetailVM();
                //IssueDetailRepo _detailRepo = new IssueDetailRepo();

                string[] ConditionFields = { };
                string[] ConditionValues = { };
                //dt = _detailRepo.VoucherReport(vm, ConditionFields, ConditionValues);

                ds.Tables.Add(dt);
                ds.Tables[0].TableName = "Issue";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Vms\IssueVoucher.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);

                //string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\" + Ordinary.ReportHeaderLogo;
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'My company'";
                //doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            _repo = new IssueHeaderRepo(identity, Session);
            string[] a = ids.Split('~');
            var id = a[0];
            IssueMasterVM vm = new IssueMasterVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            List<IssueDetailVM> IssueDetailVMs = new List<IssueDetailVM>();
            IssueDetailVMs = _repo.SelectIssueDetail(vm.IssueNo);
            vm.Details = IssueDetailVMs;
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);
            result = _repo.IssuePost(vm, identity.UserId);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult MultiplePost(IssueMasterVM paramVM)
        {
            #region Access Control
            _repo = new IssueHeaderRepo(identity, Session);
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
                OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);
                result = _repo.MultiplePost(ids);

                rVM.Status = result[0];
                rVM.Message = result[1];


            }
            catch (Exception ex)
            {

                result[0] = "Fail";
                result[1] = ex.Message;

                rVM.Status = result[0];
                rVM.Message = result[1];

                return Json(rVM, JsonRequestBehavior.AllowGet);
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult ExportExcel(IssueMasterVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new IssueHeaderRepo(identity, Session);
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
                    string[] conditionFields = new string[] { "IssueDateTime>=", "IssueDateTime<=", "TransactionType", "SelectTop" };
                    string[] conditionValues = new string[] { dtFrom, dtTo, paramVM.transactionType, paramVM.SelectTop };

                    IssueHeaderRepo repo = new IssueHeaderRepo(identity, Session);

                    var list = repo.SelectAll(0, conditionFields, conditionValues);

                    paramVM.IDs = list.Select(x => x.Id).ToList();

                }

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

                //string[] ids = paramVM.IDs.ToArray();

                DataTable dt = _repo.GetExcelDataWeb(paramVM.IDs);
                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                var vm = OrdinaryVATDesktop.DownloadExcel(dt, "Issue", "IssueM");
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + vm.FileName + ".xlsx");
                    vm.varExcelPackage.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                //rVM.Status = result[0];
                //rVM.Message = result[1];


            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;
            }

            finally { }
            return RedirectToAction("Index");
            //return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }

        public JsonResult SelectProductDetails(string productCode, string IssueDate)
        {
            var _repo = new ProductRepo(identity, Session);
            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };

            var product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();

            ////////var code = product.ProductCode;
            ////////var uom = product.UOM;
            ////////var hscode = product.HSCodeNo;
            ////////var costPrice = "";
            ////////var stock = "";
            ////////var name = product.ProductName;
            ////////var itemNo = product.ItemNo;
            if (product == null)
            {
                product = _repo.SelectAll(productCode).FirstOrDefault();
            }

            string UserId = identity.UserId;

            #region businessLogic
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);
            var issueDatetime = DateTime.Parse(IssueDate).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            DataTable priceData = _repo.AvgPriceNew(product.ItemNo, issueDatetime, null, null, true, true, true, true, UserId);
            decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
            decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

            if (quan > 0)
            {
                product.CostPrice = (amount / quan);
            }
            else
            {
                product.CostPrice = 0;
            }
            #endregion businessLogic
            product.Stock = quan;

            #region BOM Set

            //DataTable dt = new DataTable();

            //dt = _repo.SelectBOMRaw(product.ProductCode, Convert.ToDateTime(issueDatetime).ToString("yyyy-MMM-dd"));

            //int BOMId = 0;



            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    DataRow dr = dt.Rows[0];
            //    string tempBOMId = dr["BOMId"].ToString();
            //    if (!string.IsNullOrWhiteSpace(tempBOMId))
            //    {
            //        BOMId = Convert.ToInt32(tempBOMId);
            //    }
            //}

            //txtBOMId.Text = BOMId.ToString();
            #endregion

            return Json(product, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductDetails(string productCode, string IssueDate)
        {
            var _repo = new ProductRepo(identity, Session);

            var product = _repo.SelectAll(productCode).FirstOrDefault();


            //if (product == null)
            //{
            //    product = _repo.SelectAll(productCode).FirstOrDefault();
            //}

            string UserId = identity.UserId;

            #region businessLogic
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);
            var issueDatetime = DateTime.Parse(IssueDate).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            DataTable priceData = _repo.AvgPriceNew(product.ItemNo, issueDatetime, null, null, true, true, true, true, UserId);
            decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
            decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

            if (quan > 0)
            {
                product.CostPrice = (amount / quan);
            }
            else
            {
                product.CostPrice = 0;
            }
            #endregion businessLogic
            product.Stock = quan;

            #region BOM Set

            //DataTable dt = new DataTable();

            //dt = _repo.SelectBOMRaw(product.ProductCode, Convert.ToDateTime(issueDatetime).ToString("yyyy-MMM-dd"));

            //int BOMId = 0;



            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    DataRow dr = dt.Rows[0];
            //    string tempBOMId = dr["BOMId"].ToString();
            //    if (!string.IsNullOrWhiteSpace(tempBOMId))
            //    {
            //        BOMId = Convert.ToInt32(tempBOMId);
            //    }
            //}

            //txtBOMId.Text = BOMId.ToString();
            #endregion

            return Json(product, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductDetailsSearch(string productCode, string IssueDate)
        {
            var _repo = new ProductRepo(identity, Session);

            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };

            //var product = _repo.SelectAll(productCode).FirstOrDefault();
            var product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();


            //if (product == null)
            //{
            //    product = _repo.SelectAll(productCode).FirstOrDefault();
            //}

            string UserId = identity.UserId;

            #region businessLogic
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);
            var issueDatetime = DateTime.Parse(IssueDate).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            DataTable priceData = _repo.AvgPriceNew(product.ItemNo, issueDatetime, null, null, true, true, true, true, UserId);
            decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
            decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

            if (quan > 0)
            {
                product.CostPrice = (amount / quan);
            }
            else
            {
                product.CostPrice = 0;
            }
            #endregion businessLogic
            product.Stock = quan;

            #region BOM Set

            //DataTable dt = new DataTable();

            //dt = _repo.SelectBOMRaw(product.ProductCode, Convert.ToDateTime(issueDatetime).ToString("yyyy-MMM-dd"));

            //int BOMId = 0;



            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    DataRow dr = dt.Rows[0];
            //    string tempBOMId = dr["BOMId"].ToString();
            //    if (!string.IsNullOrWhiteSpace(tempBOMId))
            //    {
            //        BOMId = Convert.ToInt32(tempBOMId);
            //    }
            //}

            //txtBOMId.Text = BOMId.ToString();
            #endregion

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

                #region old code

                ////var html = "";

                ////string uomF = OrdinaryVATDesktop.StringReplacingForHTML(uomFrom);
                ////html += "<option value='" + uomF + "'>" + uomF + "</option>";

                ////if (uoms != null || uoms.Count != 0)
                ////{
                ////    foreach (var item in uoms)
                ////    {
                ////        html += "<option value='" + item.UOMTo + "'>" + item.UOMTo + "</option>";

                ////    }
                ////}

                #endregion old code

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

        public JsonResult GetConvFactor(string uomFrom, string UomTo)
        {
            var _repo = new UOMRepo(identity, Session);
            string[] conditionalFields = new string[] { "UOMFrom", "UOMTo" };
            string[] conditionalValues = new string[] { uomFrom, UomTo };
            var uom = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();
            var uomFactor = uom.Convertion;
            return Json(uomFactor, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Navigate(string id, string btn, string ttype)
        {
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetIdForTtype("IssueHeaders", "Id", id, btn, ttype);
            return RedirectToAction("Edit", new { id = targetId, TransactionType = ttype });
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetIssuePopUp(string targetId)
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
            return PartialView("_IssueHeader", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredIssue(IssueMasterVM vm)
        {

            _repo = new IssueHeaderRepo(identity, Session);
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
            string[] conditionalFields = new string[] { "IssueDateTime>", "IssueDateTime<", "Post" };
            string[] conditionalValues = new string[] { vm.IssueDateTimeFrom, vm.IssueDateTimeTo, vm.Post };
            var list = _repo.SelectAll(0, conditionalFields, conditionalValues, null, null, vm);

            return PartialView("_filteredIssues", list);
        }

        [Authorize]
        public ActionResult ImportExcel(IssueMasterVM vm)
        {
            _repo = new IssueHeaderRepo(identity, Session);
            string[] result = new string[6];
            try
            {
                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                //////FileLogger.Log("IssueHeaderController", "ImportExcel", "Start Import Excel File");

                result = _repo.ImportExcelFile(vm);

                //////FileLogger.Log("IssueHeaderController", "ImportExcel", "End Import Excel File result1 : " + result[1] + " result4" + result[4]);
                string represult = result[1];
                represult = represult.Replace("\r\n", "");

                ////Session["result"] = result[0] + "~" + result[1] + " " + (result.Length >= 5 ? result[4] : ""); ;
                Session["result"] = result[0] + "~" + represult;
                //Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index", new { vm.transactionType });
            }
            catch (Exception e)
            {
                Session["result"] = result[0] + "~" + result[1] + "\n" + e.Message;

                FileLogger.Log("IssueHeaderController", "ImportExcel", e.ToString());

                return View("Index", vm);
            }
        }
    }
}
