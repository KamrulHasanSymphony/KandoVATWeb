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
    public class AdjustmentHistoryController : Controller
    {

        //////ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        ////////AdjustmentHistoryRepo _repo = new AdjustmentHistoryRepo(identity);

        ShampanIdentity identity = null;
        AdjustmentHistoryRepo _repo = null;

        public AdjustmentHistoryController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                _repo = new AdjustmentHistoryRepo(identity);
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
        public ActionResult Index()
        {
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
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new AdjustmentHistoryRepo(identity, Session);

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
            //00     //AdjHistoryID 
            //01     //AdjHistoryNo
            //02     //HeadName 
            //03     //AdjInputAmount
            //04     //AdjInputPercent
            //05     //AdjAmount
            //06     //AdjType

            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<AdjustmentHistoryVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.AdjHistoryNo.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.HeadName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.AdjInputAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.AdjInputPercent.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.AdjAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.AdjType.ToLower().Contains(param.sSearch.ToLower())
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
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_4"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<AdjustmentHistoryVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.AdjHistoryNo :
                sortColumnIndex == 2 && isSortable_2 ? c.HeadName :
                sortColumnIndex == 3 && isSortable_3 ? c.AdjInputAmount.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.AdjInputPercent.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.AdjAmount.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.AdjType.ToString() :

                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                  c.AdjHistoryID
                , c.AdjHistoryNo
                , c.HeadName
                , c.AdjInputAmount.ToString()
                , c.AdjInputPercent.ToString()
                , c.AdjAmount.ToString()
                , c.AdjType

            };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult IndexCashPayable()
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new AdjustmentHistoryRepo(identity, Session);

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
            return View();
        }
        [Authorize(Roles = "Admin")]

        public ActionResult _indexCashPayable(JQueryDataTableParamModel param)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new AdjustmentHistoryRepo(identity, Session);

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
            //00     //AdjHistoryID 
            //01     //AdjHistoryNo
            //02     //HeadName 
            //03     //AdjInputAmount
            //04     //AdjInputPercent
            //05     //AdjAmount
            //06     //AdjType

            #region Search and Filter Data
            var getAllData = _repo.SelectAllCashPayable();
            IEnumerable<AdjustmentHistoryVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.AdjHistoryNo.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.HeadName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.AdjInputAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.AdjInputPercent.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.AdjAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.AdjType.ToLower().Contains(param.sSearch.ToLower())
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
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_4"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<AdjustmentHistoryVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.AdjHistoryNo :
                sortColumnIndex == 2 && isSortable_2 ? c.HeadName :
                sortColumnIndex == 3 && isSortable_3 ? c.AdjInputAmount.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.AdjInputPercent.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.AdjAmount.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.AdjType.ToString() :

                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                  c.AdjHistoryID
                , c.AdjHistoryNo
                , c.HeadName
                , c.AdjInputAmount.ToString()
                , c.AdjInputPercent.ToString()
                , c.AdjAmount.ToString()
                , c.AdjType

            };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            CommonRepo commonRepo = new CommonRepo(identity, Session);
            // string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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
            AdjustmentHistoryVM vm = new AdjustmentHistoryVM();
            vm.AdjDate = Session["SessionDate"].ToString();

            vm.Operation = "add";
            return View(vm);
        }

        public ActionResult CreateCashPayable()
        {
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
            CashPayableVM vm = new CashPayableVM();
            vm.Adjustment = new AdjustmentHistoryVM();
            vm.Deposit = new DepositMasterVM();
            vm.Operation = "add";
            vm.Adjustment.AdjType = "DevelopmentSurcharge";
            vm.Adjustment.AdjId = "1";
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(AdjustmentHistoryVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new AdjustmentHistoryRepo(identity, Session);
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
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    vm.CreatedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                    result = _repo.InsertAdjustmentHistory(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    vm.LastModifiedBy = identity.Name;
                    vm.AdjDate = Convert.ToDateTime(vm.AdjDate).ToString("yyyy-MM-dd hh:mm:ss");
                    result = _repo.UpdateAdjustmentHistory(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = result[2] });
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
                // FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("AdjustmentHistoryController", "CreateEdit", ex.ToString());
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEditCashPayable(CashPayableVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new AdjustmentHistoryRepo(identity, Session);

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
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.Adjustment.CreatedOn = DateTime.Now.ToString();
                    vm.Adjustment.CreatedBy = identity.Name;
                    vm.Adjustment.LastModifiedOn = DateTime.Now.ToString();
                    vm.Adjustment.LastModifiedBy = identity.Name;
                    vm.Deposit.BranchId = Convert.ToInt32(Session["BranchId"]);

                    vm.Deposit.CreatedOn = DateTime.Now.ToString();
                    vm.Deposit.CreatedBy = identity.Name;
                    vm.Deposit.LastModifiedOn = DateTime.Now.ToString();
                    vm.Deposit.LastModifiedBy = identity.Name;
                    vm.Deposit.TransactionType = vm.Adjustment.AdjType;

                    result = new DepositRepo(identity, Session).DepositInsert(vm.Deposit, null, vm.Adjustment);
                    string[] conditionFields = { "AdjHistoryNo" };
                    string[] conditionValues = { result[2] };
                    var AdjHistory = _repo.SelectAllCashPayable("0", conditionFields, conditionValues).SingleOrDefault();
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("EditCashPayable", new { id = AdjHistory.AdjHistoryID });
                    }
                    else
                    {
                        return View("CreateCashPayable", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.Adjustment.LastModifiedOn = DateTime.Now.ToString();
                    vm.Adjustment.LastModifiedBy = identity.Name;
                    vm.Deposit.LastModifiedOn = DateTime.Now.ToString();
                    vm.Deposit.LastModifiedBy = identity.Name;
                    vm.Deposit.TransactionType = vm.Adjustment.AdjType;

                    result = new DepositRepo(identity, Session).DepositUpdate(vm.Deposit, null, vm.Adjustment);
                    string[] conditionFields = { "AdjHistoryNo" };
                    string[] conditionValues = { result[2] };
                    var AdjHistory = _repo.SelectAllCashPayable("0", conditionFields, conditionValues).SingleOrDefault();
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("EditCashPayable", new { id = AdjHistory.AdjHistoryID });
                }
                else
                {
                    return View("CreateCashPayable", vm);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                //  FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("AdjustmentHistoryController", "CreateEditCashPayable", ex.ToString());
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new AdjustmentHistoryRepo(identity, Session);

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
            AdjustmentHistoryVM vm = new AdjustmentHistoryVM();
            vm = _repo.SelectAll(id).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }

        [HttpGet]
        public ActionResult EditCashPayable(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new AdjustmentHistoryRepo(identity, Session);

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
            AdjustmentHistoryVM adjustment = new AdjustmentHistoryVM();
            adjustment = _repo.SelectAllCashPayable(id).FirstOrDefault();

            DepositMasterVM deposit = new DepositMasterVM();
            string[] conditionFields = { "DepositId" };
            string[] conditionValues = { adjustment.AdjHistoryNo };
            deposit = new DepositRepo(identity, Session).SelectAll(0, conditionFields, conditionValues).SingleOrDefault();

            CashPayableVM vm = new CashPayableVM();
            vm.Adjustment = adjustment;
            vm.Deposit = deposit;

            vm.Operation = "update";
            return View("CreateCashPayable", vm);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new AdjustmentHistoryRepo(identity, Session);


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
            CurrencyVM vm = new CurrencyVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.LastModifiedBy = identity.Name;
            //   result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids, string AdjId, string AdjHistoryNo)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new AdjustmentHistoryRepo(identity, Session);

            UserInformationRepo _UserInformationRepo = new UserInformationRepo(identity, Session);
            UserInformationVM varUserInformationVM = new UserInformationVM();
            string[] a = ids.Split('~');
            var id = a[0];
            AdjustmentHistoryVM vm = new AdjustmentHistoryVM();
            // vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            vm.Post = "Y";
            vm.AdjId = AdjId;
            vm.AdjHistoryID = ids;
            vm.AdjHistoryNo = AdjHistoryNo;
            result = _repo.PostAdjHistory(vm);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }


    }
}
