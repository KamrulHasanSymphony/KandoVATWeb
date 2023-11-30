//using JQueryDataTables.Models;
using SymOrdinary;
//using SymRepository.Common;
using SymRepository.VMS;
//using SymViewModel.Common;
using VATViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.vms.Controllers
{
    [ShampanAuthorize]
    public class SDDepositController : Controller
    {
        ShampanIdentity identity = null;

        SDDepositRepo _repo = null;

        public SDDepositController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new SDDepositRepo(identity);

            }
            catch
            {

            }
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Index(SDDepositVM paramVM)
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

            //Session["dtFrom"] = dtFrom;
            //Session["dtTo"] = dtTo;
            //Session["tType"] = tType;

            //SDDepositVM vm = new SDDepositVM();
            //vm.TransactionType = tType;

            return View(paramVM);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param, SDDepositVM paramVM)
        {
            _repo = new SDDepositRepo(identity, Session);
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



            #region Search and Filter Data
            //string searchedBranchId = "0";
            string dtFrom = DateTime.Now.ToString("yyyyMMdd");
            string dtTo = DateTime.Now.ToString("yyyyMMdd");
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
                dtFrom = paramVM.IssueDateTimeFrom;
            }
            if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTimeTo))
            {
                dtTo = paramVM.IssueDateTimeTo;
            }
            #endregion Search and Filter Data
            List<SDDepositVM> getAllData = new List<SDDepositVM>();
            if (!identity.IsAdmin)
            {
                string[] conditionFields = { "d.DepositId like", "d.TreasuryNo like", "d.DepositDateTime>=", "d.DepositDateTime<=", "d.DepositType", "d.ChequeNo like", "d.ChequeDate>=", "d.ChequeDate<=", "b.BankName like", "b.AccountNumber like", "d.Post", "d.TransactionType" };
                string[] conditionValues = { paramVM.DepositId, paramVM.TreasuryNo, dtFrom, dtTo, paramVM.DepositType, paramVM.ChequeNo, paramVM.CheckDateFrom, paramVM.CheckDateTo, paramVM.BankName, paramVM.AccountNumber, paramVM.Post, paramVM.TransactionType };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);

                //string[] conditionFields = { "DepositDateTime>", "DepositDateTime<", "TransactionType" };
                //string[] conditionValues = { Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo), tType };
                //getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            else
            {
                string[] conditionFields = { "d.DepositId like", "d.TreasuryNo like", "d.DepositDateTime>=", "d.DepositDateTime<=", "d.DepositType", "d.ChequeNo like", "d.ChequeDate>=", "d.ChequeDate<=", "b.BankName like", "b.AccountNumber like", "d.Post", "d.TransactionType" };
                string[] conditionValues = { paramVM.DepositId, paramVM.TreasuryNo, dtFrom, dtTo, paramVM.DepositType, paramVM.ChequeNo, paramVM.CheckDateFrom, paramVM.CheckDateTo, paramVM.BankName, paramVM.AccountNumber, paramVM.Post, paramVM.TransactionType };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            //var getAllData = _repo.SelectAll();
            IEnumerable<SDDepositVM> filteredData;
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
            Func<SDDepositVM, string> orderingFunction = (c =>
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
            if (string.IsNullOrWhiteSpace(tType))
            {
                tType = "Treasury";
            }
            SDDepositVM vm = new SDDepositVM();
            vm.TransactionType = tType;
            vm.Operation = "add";
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(SDDepositVM vm)
        {
            _repo = new SDDepositRepo(identity, Session);

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
                    vm.CreatedOn = DateTime.Now.ToString();
                    vm.CreatedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    vm.Post = "N";
                    vm.DepositId = "0";
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                    result = _repo.DepositInsert(vm);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[4] });
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
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    result = _repo.DepositUpdate(vm);
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
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                //  Session["result"] = "Fail~Data not Successfully";
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            _repo = new SDDepositRepo(identity, Session);

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
            SDDepositVM vm = new SDDepositVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult GetBankInformation(string bankId)
        {
            var _repo = new BankInformationRepo(identity, Session);

            var bank = _repo.SelectAll(Convert.ToInt32(bankId)).FirstOrDefault();
            var branchName = bank.BranchName;
            var accountNo = bank.AccountNumber;

            string result = branchName + "~" + accountNo;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            _repo = new SDDepositRepo(identity, Session);

            string[] a = ids.Split('~');
            var id = a[0];
            SDDepositVM vm = new SDDepositVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";
            result = _repo.DepositPost(vm);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

    }
}
