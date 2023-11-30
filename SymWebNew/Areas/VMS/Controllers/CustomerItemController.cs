using SymOrdinary;
using SymRepository.VMS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.VMS.Controllers
{
    [ShampanAuthorize]
    public class CustomerItemController : Controller
    {
        //
        // GET: /VMS/CustomerItem/

        CustomerItemRepo _repo = null;
        ShampanIdentity identity = null;

        public CustomerItemController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new CustomerItemRepo(identity);
            }
            catch
            {

            }
        }

        #region Index and _index

        public ActionResult Index(CustomerItemVM paramVM, string transactionType)
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
        public ActionResult _index(JQueryDataTableParamVM param, CustomerItemVM paramVM)
        {
            _repo = new CustomerItemRepo(identity, Session);

            List<CustomerItemVM> getAllData = new List<CustomerItemVM>();
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

            #region Data Search

            #region SeachParameters

            ////if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            ////{
            ////    paramVM.SelectTop = "All";
            ////}

            #endregion SeachParameters

            string[] conditionFields = { "cus.CustomerID" };
            string[] conditionValues = { paramVM.CustomerID };

            getAllData = _repo.SelectCustomerList(0, conditionFields, conditionValues);

            #region Comments

            ////if (paramVM.BranchId != 0)
            ////{
            ////    //////paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            ////    string[] conditionFields = { "cih.InvoiceNo like", "cih.Post", "cih.BranchId" };
            ////    string[] conditionValues = { paramVM.InvoiceNo, paramVM.Post, paramVM.BranchId.ToString()};
            ////    getAllData = _repo.SelectAllList(0, conditionFields, conditionValues);

            ////}
            ////else
            ////{
            ////    string[] conditionFields = { "cih.InvoiceNo like", "cih.Post"};
            ////    string[] conditionValues = { paramVM.InvoiceNo, paramVM.Post };
            ////    getAllData = _repo.SelectAllList(0, conditionFields, conditionValues);

            ////}

            #endregion

            #endregion

            #region Filter Data

            IEnumerable<CustomerItemVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.TotalItem.ToString().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.CustomerCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.CustomerName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Attention.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Notes.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.TotalValue.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<CustomerItemVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.TotalItem.ToString() :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.CustomerCode.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.CustomerName.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Attention.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Notes.ToString() :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.TotalValue.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.CustomerID.ToString()
                , c.CustomerCode
                , c.CustomerName.ToString()
                , c.TotalItem.ToString()
                , c.TotalValue.ToString()
                , c.Attention.ToString()             
                , c.Notes.ToString()    
                
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

        #region Create and Update

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

            CustomerItemVM vm = new CustomerItemVM();

            List<CustomerItemDetailsVM> DetailVMs = new List<CustomerItemDetailsVM>();
            vm.Details = DetailVMs;
            vm.Operation = "add";
            vm.TransactionType = tType;
            vm.ProductType = "Raw";

            return View(vm);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(CustomerItemVM vm)
        {
            _repo = new CustomerItemRepo(identity, Session);
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
                    result = _repo.CustomerItemInsert(vm);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = vm.CustomerID, TransactionType = vm.TransactionType });
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
                    result = _repo.CustomerItemUpdate(vm);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = vm.CustomerID, TransactionType = vm.TransactionType });
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
            List<CustomerItemDetailsVM> DetailVMs = new List<CustomerItemDetailsVM>();

            _repo = new CustomerItemRepo(identity, Session);
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

            CustomerItemVM vm = new CustomerItemVM();

            ////string[] conditionFields = new string[] { "TransactionType" };
            ////string[] conditionValues = new string[] { TransactionType };

            vm = _repo.SelectAllList(Convert.ToInt32(id)).FirstOrDefault();

            if (vm == null)
            {
                string[] conditionFields = { "cus.CustomerID" };
                string[] conditionValues = { id };

                vm = _repo.SelectCustomerList(0, conditionFields, conditionValues).FirstOrDefault();

                vm.TransactionType = TransactionType;
                vm.Details = DetailVMs;

                vm.Operation = "add";

                vm.ProductType = "Raw";
                return View("Create", vm);

                ////return RedirectToAction("Index", "Home");
            }


            DetailVMs = _repo.SelectAllDetail(vm.CustomerID);

            vm.Details = DetailVMs;
            vm.Operation = "update";

            vm.ProductType = "Raw";
            return View("Create", vm);
        }


        #endregion

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(CustomerItemDetailsVM vm)
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
            ////vm.FinishItemNo = "N/A";
            ////vm.FinishItemName = "N/A";

            ProductRepo PRepo = new ProductRepo(identity, Session);
            //////string BOMId = null;
            //////var dtbom = PRepo.SelectBOMRaw(vm.ItemNo, DateTime.Parse(vm.IssueDateTime).ToString("yyyy-MMM-dd"));

            //////if (dtbom != null && dtbom.Rows.Count > 0)
            //////{
            //////    BOMId = dtbom.Rows[0]["BOMId"].ToString();
            //////}
            //////if (string.IsNullOrEmpty(BOMId))
            //////{
            //////    vm.BOMId = 0;
            //////}
            //////else
            //////{
            //////    vm.BOMId = Convert.ToInt32(BOMId);
            //////}

            return PartialView("_detail", vm);
        }

        public JsonResult SelectProductDetails(string productCode, string IssueDate)
        {
            var _repo = new ProductRepo(identity, Session);
            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };

            var product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();

            if (product == null)
            {
                product = _repo.SelectAll(productCode).FirstOrDefault();
            }

            string UserId = identity.UserId;

            return Json(product, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            _repo = new CustomerItemRepo(identity, Session);
            string[] a = ids.Split('~');
            var id = a[0];
            CustomerItemVM vm = new CustomerItemVM();
            vm = _repo.SelectAllList(Convert.ToInt32(id)).FirstOrDefault();
            List<CustomerItemDetailsVM> DetailVMs = new List<CustomerItemDetailsVM>();
            DetailVMs = _repo.SelectAllDetail(vm.InvoiceNo);
            vm.Details = DetailVMs;
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";
            result = _repo.CustomerItemPost(vm, identity.UserId);

            return Json(result[1], JsonRequestBehavior.AllowGet);

        }


        #region BillProcessIndex and _GetCustomerItem

        public ActionResult BillProcess(CustomerItemVM paramVM)
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

            if (string.IsNullOrWhiteSpace(paramVM.PeriodName))
            {
                paramVM.PeriodName = DateTime.Now.ToString("MMMM-yyyy");
            }

            if (string.IsNullOrWhiteSpace(paramVM.ProcessDate))
            {
                paramVM.ProcessDate = DateTime.Now.ToString("dd-MMM-yyyy");
            }

            return View(paramVM);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _GetCustomerItem(JQueryDataTableParamVM param, CustomerItemVM paramVM)
        {
            _repo = new CustomerItemRepo(identity, Session);

            List<CustomerItemVM> getAllData = new List<CustomerItemVM>();

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

            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "All";
            }

            string Month = Convert.ToDateTime(paramVM.ProcessDate).ToString("MMM");
            string processMonth = "cbp." + Month;

            #endregion SeachParameters

            if (paramVM.BranchId != 0)
            {
                //////paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                string[] conditionFields = { "cih.TransactionType", "cih.InvoiceNo like", "cih.Post", "cih.BranchId", processMonth };
                string[] conditionValues = { paramVM.TransactionType, paramVM.InvoiceNo, paramVM.Post, paramVM.BranchId.ToString(), "Y" };
                getAllData = _repo.SelectAllCustomerList(0, conditionFields, conditionValues);

            }
            else
            {
                string[] conditionFields = { "cih.TransactionType", "cih.InvoiceNo like", "cih.Post", processMonth };
                string[] conditionValues = { paramVM.TransactionType, paramVM.InvoiceNo, paramVM.Post, "Y" };
                getAllData = _repo.SelectAllCustomerList(0, conditionFields, conditionValues);

            }

            #endregion

            #region Search and Filter Data

            IEnumerable<CustomerItemVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.TotalValue.ToString().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.CustomerCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.CustomerName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Attention.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Notes.ToString().ToLower().Contains(param.sSearch.ToLower())
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

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<CustomerItemVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.TotalValue.ToString() :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.CustomerCode.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.CustomerName.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Attention.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Notes.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.CustomerID.ToString()
                , c.CustomerCode
                , c.CustomerName.ToString()
                , c.TotalValue.ToString()
                , c.Attention.ToString()             
                , c.Notes.ToString()               
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

        #endregion

        [Authorize]
        public ActionResult ProcessBill(CustomerItemVM MasterVM)
        {
            #region Access Control

            _repo = new CustomerItemRepo(identity, Session);
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
            IntegrationParam paramVM = new IntegrationParam();
            try
            {


                if (MasterVM == null && MasterVM.IDs == null && MasterVM.IDs.Count <= 0)
                {

                    rVM.Message = "No Data to Process";
                    return Json(rVM, JsonRequestBehavior.AllowGet);

                    //////MasterVM.IDs = MasterVM.IDs.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

                    //////if (MasterVM.IDs == null || MasterVM.IDs.Count == 0)
                    //////{
                    //////    rVM.Message = "No Data to Process";
                    //////    return Json(rVM, JsonRequestBehavior.AllowGet);
                    //////}
                }
                //////else
                //////{
                //////    rVM.Message = "No Data to Process";
                //////    return Json(rVM, JsonRequestBehavior.AllowGet);
                //////}

                paramVM.BranchCode = Session["BranchCode"].ToString();
                paramVM.BranchId = Session["BranchId"].ToString();
                paramVM.CurrentUserName = identity.Name;
                paramVM.CurrentUser = identity.UserId;
                paramVM.TransactionType = MasterVM.TransactionType;

                rVM = _repo.BillProcess(MasterVM, paramVM);

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;

                return Json(rVM, JsonRequestBehavior.AllowGet);

            }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult ExportExcell(CustomerItemVM paramVM)
        {
            #region Access Control
            _repo = new CustomerItemRepo(identity, Session);

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
                        Session["result"] = "Fail" + "~" + "No Data to Export";

                        ////rVM.Message = "No Data to Export";
                        ////return Json(rVM, JsonRequestBehavior.AllowGet);
                        return RedirectToAction("Index");

                    }
                }
                else
                {
                    Session["result"] = "Fail" + "~" + "No Data to Export";

                    ////rVM.Message = "No Data to Export";
                    ////return Json(rVM, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("Index");

                }

                DataTable dt = _repo.GetExcelDataWeb(paramVM.IDs);

                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                //  OrdinaryVATDesktop.SaveExcel(dt, "Sale", "SaleM");
                var vm = OrdinaryVATDesktop.DownloadExcel(dt, "CustomerProductMapping", "CustomerProducts");

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + vm.FileName + ".xlsx");
                    vm.varExcelPackage.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                ////rVM.Status = "Success";
                ////rVM.Message = "Your requested information successfully Exported";
            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
            }

            finally { }

            return RedirectToAction("Index");

            ////return Json(rVM, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult ExportExcell_BillProcess(CustomerItemVM paramVM)
        {
            #region Access Control
            _repo = new CustomerItemRepo(identity, Session);

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
                        Session["result"] = "Fail" + "~" + "No Data to Export";

                        ////rVM.Message = "No Data to Export";
                        ////return Json(rVM, JsonRequestBehavior.AllowGet);
                        return RedirectToAction("BillProcess");

                    }
                }
                else
                {
                    Session["result"] = "Fail" + "~" + "No Data to Export";

                    ////rVM.Message = "No Data to Export";
                    ////return Json(rVM, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("BillProcess");

                }

                DataTable dt = _repo.SelectAllCustomer_Export(paramVM.IDs);

                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                //  OrdinaryVATDesktop.SaveExcel(dt, "Sale", "SaleM");
                var vm = OrdinaryVATDesktop.DownloadExcel(dt, "BillProcess", "BillProcess");

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + vm.FileName + ".xlsx");
                    vm.varExcelPackage.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                ////rVM.Status = "Success";
                ////rVM.Message = "Your requested information successfully Exported";
            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
            }

            finally { }

            return RedirectToAction("BillProcess");

            ////return Json(rVM, JsonRequestBehavior.AllowGet);
        }


    }
}
