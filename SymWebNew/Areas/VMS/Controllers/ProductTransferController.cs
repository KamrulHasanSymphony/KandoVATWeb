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

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class ProductTransferController : Controller
    {
        //
        // GET: /Vms/FinancialTransaction/

        ProductTransferRepo _repo1 = null;
        IssueHeaderRepo _repo = null;
        ShampanIdentity identity = null;


        public ProductTransferController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo1 = new ProductTransferRepo(identity);
           
                //identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new IssueHeaderRepo(identity);
                //_repo1 = new ProductTransferRepo(identity);
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
        public ActionResult Index(ProductTransfersVM paramVM)
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

            //paramVM.TransactionType = tType;
            if (string.IsNullOrWhiteSpace(paramVM.TransactionType))
            {
                paramVM.TransactionType = "RawCTC";
            }


            return View(paramVM);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamVM param, ProductTransfersVM paramVM)
        {
            _repo1 = new ProductTransferRepo(identity, Session);

            List<ProductTransfersVM> getAllData = new List<ProductTransfersVM>();
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
            
            if (!string.IsNullOrWhiteSpace(paramVM.TransferDateFrom))
            {
                dtFrom = paramVM.TransferDateFrom;
            }
            if (!string.IsNullOrWhiteSpace(paramVM.TransferDateTo))
            {
                dtTo = paramVM.TransferDateTo;
            }

            #endregion SeachParameters


            if (!identity.IsAdmin)
            {
                string[] conditionFields = { "TransferDate>=", "TransferDate<=","TransactionType","TransferCode like", "Post", "BranchId" };
                string[] conditionValues = { dtFrom, dtTo,paramVM.TransactionType,paramVM.TransferCode, paramVM.Post, paramVM.BranchId.ToString() };
                getAllData = _repo1.SelectAll(0, conditionFields, conditionValues);
            }
            else
            {
                string[] conditionFields = { "TransferDate>=", "TransferDate<=", "TransactionType", "TransferCode like", "Post", "BranchId" };
                string[] conditionValues = { dtFrom, dtTo, paramVM.TransactionType, paramVM.TransferCode, paramVM.Post, paramVM.BranchId.ToString() };
                getAllData = _repo1.SelectAll(0, conditionFields, conditionValues);
            }
            #endregion
            #region Search and Filter Data
            IEnumerable<ProductTransfersVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Id
                //TransferCode
                //TransferDate
                //Post
                //IsWastage
            

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
             

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.TransferCode.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.TransferDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.IsWastage.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ProductTransfersVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.TransferCode :
                                                           sortColumnIndex == 2 && isSortable_2 ? Ordinary.DateToString(c.TransferDate) :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Post.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.IsWastage.ToString() :
                                                   
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
                , c.TransferCode
                , c.TransferDate
                , c.Post=="Y" ? "Posted" : "Not Posted"
                , c.IsWastage
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
        public ActionResult BlankItem(ProductTransfersDetailVM vm)
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
            ProductTransfersVM vm = new ProductTransfersVM();

            List<ProductTransfersDetailVM> IssueDetailVMs = new List<ProductTransfersDetailVM>();
            vm.Details = IssueDetailVMs;
            vm.Operation = "add";
            vm.TransactionType = tType;
            //vm.IsWastage = tType;

            string type = tType;
            if (type == "WastageCTC")
            {
                vm.IsWastage = "Wastage";
            }
            if (type == "RawCTC")
            {
                vm.IsWastage = "Raw";
            }
            if (type == "FinishCTC")
            {
                vm.IsWastage = "Finish";
            }

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(ProductTransfersVM vm)
        {
            _repo1 = new ProductTransferRepo(identity, Session);
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

            vm.IsWastage = vm.IsWastage == "Wastage" ? "Y" : "R";

            if (vm.TransactionType == "WastageCTC")
            {
                vm.IsWastage = "Y";
            }
            else if (vm.TransactionType == "RawCTC")
            {
                vm.IsWastage = "R";
            }
            else
            {
                vm.IsWastage = "F";
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
                    vm.ActiveStatus = "Y";
                    foreach (ProductTransfersDetailVM vmD in vm.Details)
                    {
                        vmD.BranchId = vm.BranchId;
                        vmD.IsWastage = vm.IsWastage;
                        vmD.TransferDate = vm.TransferDate;
                    }
                    result = _repo1.Insert(vm,null,null,identity.UserId);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[4]});
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
                    foreach (ProductTransfersDetailVM vmD in vm.Details)
                    {
                        vmD.BranchId = vm.BranchId;
                        vmD.IsWastage = vm.IsWastage;
                        vmD.TransferDate = vm.TransferDate;
                    }
                    result = _repo1.Update(vm,identity.UserId);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = vm.Id });
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
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
              //  Session["result"] = "Fail~Data not Successfully";
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            _repo1 = new ProductTransferRepo(identity, Session);
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

           

            ProductTransfersVM vm = new ProductTransfersVM();

            
            vm = _repo1.SelectAll(Convert.ToInt32(id),null,null).FirstOrDefault();

            if (vm == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<ProductTransfersDetailVM> TransfersDetailVMs = new List<ProductTransfersDetailVM>();

            TransfersDetailVMs = _repo1.SearchTransferDetail(vm.Id.ToString());

            vm.Details = TransfersDetailVMs;
            vm.Operation = "update";
            //vm.GetTransactionType();
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

      

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            _repo1 = new ProductTransferRepo(identity, Session);
            string[] a = ids.Split('~');
            var id = a[0];
            ProductTransfersVM vm = new ProductTransfersVM();
            vm = _repo1.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";
            vm.IsWastage = vm.IsWastage == "Wastage" ? "Y" : "R";
            vm.Id = vm.Id;
            vm.TransferCode = vm.TransferCode;

            result = _repo1.Post(vm);
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


       



        public JsonResult GetConvFactor(string uomFrom, string UomTo)
        {
            var uomFactor ="";
            var _repo = new UOMRepo(identity, Session);
            string[] conditionalFields = new string[] { "UOMFrom", "UOMTo" };
            string[] conditionalValues = new string[] { uomFrom, UomTo };
            var uom = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();
            if(uom!=null)
            {
              uomFactor = uom.Convertion.ToString();

            }
            return Json(uomFactor, JsonRequestBehavior.AllowGet);
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
            string[] conditionalFields = new string[] { "IssueDateTime>", "IssueDateTime<" ,"Post"};
            string[] conditionalValues = new string[] { vm.IssueDateTimeFrom, vm.IssueDateTimeTo, vm.Post};
            var list = _repo.SelectAll(0, conditionalFields, conditionalValues,null,null,vm);

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

                result = _repo.ImportExcelFile(vm);
                Session["result"] = result[0] + "~" + result[1];
                return View("Index", vm);
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
             //  Session["result"] = result[0] + "~" + result[1];
               // FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("ProductTransferController", "ImportExcel", ex.ToString());
                return View("Index", vm);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetProductPopUp(PopUpViewModel vm)
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
            vm.ProductCode = null;
            vm.TargetId = null;
            return PartialView("_products", vm);
        }
    }
}
