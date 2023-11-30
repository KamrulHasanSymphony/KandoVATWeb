using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Vms;
using SymRepository.Common;
using SymViewModel.Vms;
using SymViewModel.Common;
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

namespace SymWebUI.Areas.Vms.Controllers
{
    public class ReceiveHeaderController : Controller
    {
        //
        // GET: /Vms/FinancialTransaction/

        IssueHeaderRepo _repo = new IssueHeaderRepo();
        IssueDetailRepo _detailRepo = new IssueDetailRepo();


        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        [Authorize(Roles = "Admin")]
        public ActionResult Menu()
        {
            return View();
        }
        #region Index
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string tType, string dtFrom = "", string dtTo = "")
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    if (!identity.IsExpense)
                    {
                        Session["rollPermission"] = "deny";
                        return Redirect("/Vms/Home");
                    }
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }


            if (string.IsNullOrWhiteSpace(tType))
            {
                tType = "";
            }

            Session["dtFrom"] = dtFrom;
            Session["dtTo"] = dtTo;

            IssueHeaderViewModel vm = new IssueHeaderViewModel();
            vm.TransactionType = tType;

            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamVM param, string tType)
        {
            List<IssueHeaderViewModel> getAllData = new List<IssueHeaderViewModel>();
            #region Access Controll
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    if (!identity.IsExpense)
                    {
                        Session["rollPermission"] = "deny";
                        return Redirect("/Vms/Home");
                    }
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }
            #region SeachParameters
            string searchedBranchId = "0";
            string dtFrom = DateTime.Now.ToString("yyyyMMdd");
            string dtTo = DateTime.Now.ToString("yyyyMMdd");
            if (!string.IsNullOrWhiteSpace(Session["Branch"] as string))
            {
                searchedBranchId = Session["Branch"].ToString();
            }
            if (!string.IsNullOrWhiteSpace(Session["dtFrom"] as string))
            {
                dtFrom = Convert.ToDateTime(Session["dtFrom"]).ToString("yyyyMMdd");
            }
            if (!string.IsNullOrWhiteSpace(Session["dtTo"] as string))
            {
                dtTo = Convert.ToDateTime(Session["dtTo"]).ToString("yyyyMMdd");
            }
            string BranchId = "";
            if (searchedBranchId == "-1")
            {
                BranchId = "";
            }
            else if (BranchId != searchedBranchId && searchedBranchId != "0")
            {
                BranchId = searchedBranchId;
            }
            else
            {
                BranchId = identity.BranchId.ToString();
            }

            #endregion SeachParameters


            if (!identity.IsAdmin)
            {
                string[] conditionFields = {"IssueDateTime>", "IssueDateTime<","TransactionType" };
                string[] conditionValues = { Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo), tType };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            else
            {
                string[] conditionFields = { "IssueDateTime>", "IssueDateTime<", "TransactionType" };
                string[] conditionValues = { Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo), tType };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            #endregion
            #region Search and Filter Data
            IEnumerable<IssueHeaderViewModel> filteredData;
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
            Func<IssueHeaderViewModel, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.IssueNo :
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
                  c.Id.ToString()
                , c.IssueNo
                , c.IssueDateTime
                , c.TotalVATAmount.ToString()
                , c.TotalAmount.ToString()             
                , c.SerialNo.ToString()               
                , c.Post ? "Posted" : "Not Posted"
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
        public ActionResult PostedIndex(string dtFrom = "", string dtTo = "")
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    if (!identity.HaveApproval && !identity.HaveExpenseApproval)
                    {
                        Session["rollPermission"] = "deny";
                        return Redirect("/Vms/Home");
                    }
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }
            Session["dtFrom"] = dtFrom;
            Session["dtTo"] = dtTo;
            IssueHeaderViewModel vm = new IssueHeaderViewModel();
            return View(vm);
        }
        [Authorize(Roles = "Admin")]

        public ActionResult _postedIndex(JQueryDataTableParamVM param)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    if (!identity.HaveApproval && !identity.HaveExpenseApproval)
                    {
                        Session["rollPermission"] = "deny";
                        return Redirect("/Vms/Home");
                    }
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }
            List<IssueHeaderViewModel> getAllData = new List<IssueHeaderViewModel>();


            #region SeachParameters
            string searchedBranchId = "0";
            string dtFrom = DateTime.Now.ToString("yyyyMMdd");
            string dtTo = DateTime.Now.ToString("yyyyMMdd");

            if (!string.IsNullOrWhiteSpace(Session["dtFrom"] as string))
            {
                dtFrom = Convert.ToDateTime(Session["dtFrom"]).ToString("yyyyMMdd");
            }
            if (!string.IsNullOrWhiteSpace(Session["dtTo"] as string))
            {
                dtTo = Convert.ToDateTime(Session["dtTo"]).ToString("yyyyMMdd");
            }
            string BranchId = "";
            if (searchedBranchId == "-1")
            {
                BranchId = "";
            }
            else if (BranchId != searchedBranchId && searchedBranchId != "0")
            {
                BranchId = searchedBranchId;
            }
            else
            {
                BranchId = identity.BranchId.ToString();
            }

            #endregion SeachParameters

            #region Search and Filter Data

            string[] conditionFields = { "IssueDateTime>", "IssueDateTime<"};
            string[] conditionValues = { Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo)};

            getAllData = _repo.SelectAllPosted(0, conditionFields, conditionValues);
            IEnumerable<IssueHeaderViewModel> filteredData;
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
            Func<IssueHeaderViewModel, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.IssueNo :
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
                  c.Id.ToString()
                , c.IssueNo
                , c.IssueDateTime.ToString()
                , c.TotalVATAmount.ToString()
                , c.TotalAmount.ToString()           
                , c.SerialNo
                , c.Post?"Posted":"Not Posted"
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
        public ActionResult BlankItem(IssueDetailViewModel vm)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    if (!identity.IsExpense)
                    {
                        Session["rollPermission"] = "deny";
                        return Redirect("/Vms/Home");
                    }
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }
            ProductRepo repo = new ProductRepo();
            var product = repo.SelectAll(Convert.ToInt32(vm.ItemNo)).FirstOrDefault();
            vm.ItemName = product.ProductName;
            //vm.ItemNo = Convert.ToInt32(vm.ItemNo);
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
                    if (!identity.IsExpense)
                    {
                        Session["rollPermission"] = "deny";
                        return Redirect("/Vms/Home");
                    }
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }
            IssueHeaderViewModel vm = new IssueHeaderViewModel();

            List<IssueDetailViewModel> IssueDetailViewModels = new List<IssueDetailViewModel>();
            vm.issueDetailVMs = IssueDetailViewModels;
            vm.Operation = "add";
            //vm.TransactionType = "Other";
            vm.TransactionType = tType;
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(IssueHeaderViewModel vm, List<HttpPostedFileBase> fileUpload)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    if (!identity.IsExpense)
                    {
                        Session["rollPermission"] = "deny";
                        return Redirect("/Vms/Home");
                    }
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
                    vm.CreatedOn = DateTime.Now;
                    vm.CreatedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now;
                    result = _repo.Insert(vm);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        vm.Id = 0;
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now;
                    result = _repo.Update(vm);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[2] });
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
            catch (Exception)
            {
                Session["result"] = "Fail~Data not Successfully";
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    if (!identity.IsExpense)
                    {
                        Session["rollPermission"] = "deny";
                        return Redirect("/Vms/Home");
                    }
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }
            IssueHeaderViewModel vm = new IssueHeaderViewModel();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

            List<IssueDetailViewModel> IssueDetailViewModels = new List<IssueDetailViewModel>();

            IssueDetailViewModels = _detailRepo.SelectByMasterId(Convert.ToInt32(id));

            vm.issueDetailVMs = IssueDetailViewModels;
            vm.Operation = "update";
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
                    if (!identity.HaveApproval && !identity.HaveExpenseApproval)
                    {
                        Session["rollPermission"] = "deny";
                        return Redirect("/Vms/Home");
                    }
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }
            IssueHeaderViewModel vm = new IssueHeaderViewModel();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            List<IssueDetailViewModel> IssueDetailViewModels = new List<IssueDetailViewModel>();

            IssueDetailViewModels = _detailRepo.SelectByMasterId(Convert.ToInt32(id));

            vm.issueDetailVMs = IssueDetailViewModels;

            vm.Operation = "posted";
            return View("Posted", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    if (!identity.IsExpense)
                    {
                        Session["rollPermission"] = "deny";
                        return Redirect("/Vms/Home");
                    }
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }
            string[] a = ids.Split('~');
            string[] result = new string[6];
            IssueHeaderViewModel vm = new IssueHeaderViewModel();
            result = _repo.Post(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }


    }
}
