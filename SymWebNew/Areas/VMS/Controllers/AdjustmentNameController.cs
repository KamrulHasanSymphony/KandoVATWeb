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
    public class AdjustmentNameController : Controller
    {

        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //AdjustmentRepo _repo = new AdjustmentRepo();

        ShampanIdentity identity = null;
        AdjustmentRepo _repo = null;

        public AdjustmentNameController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                _repo = new AdjustmentRepo(identity);
            }
            catch
            {

            }

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

            _repo = new AdjustmentRepo(identity, Session);

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
            //00     //AdjId 
            //01     //AdjName
            //02     //Comments
            //03     //ActiveStatus

            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<AdjustmentVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.AdjName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Comments.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.ActiveStatus.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<AdjustmentVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.AdjName :
                sortColumnIndex == 2 && isSortable_2 ? c.Comments :
                sortColumnIndex == 3 && isSortable_3 ? c.ActiveStatus.ToString() :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                  c.AdjId
                , c.AdjName
                , c.Comments
                , c.ActiveStatus.ToString()
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
        public ActionResult Create()
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
            AdjustmentVM vm = new AdjustmentVM();
            vm.Operation = "add";
            vm.ActiveStatus = "Y";
            return PartialView(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(AdjustmentVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new AdjustmentRepo(identity, Session);
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
                    vm.ActiveStatus = "Y";
                    vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    result = _repo.InsertAdjustmentName(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return PartialView("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    vm.LastModifiedBy = identity.Name;
                    result = _repo.UpdateAdjustmentName(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    return PartialView("Create", vm);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                //FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("AdjustmentNameController", "CreateEdit", ex.ToString());
                //return PartialView("Create", vm);
                return View("Index", vm);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new AdjustmentRepo(identity, Session);
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
            AdjustmentVM vm = new AdjustmentVM();
            vm = _repo.SelectAll(id).FirstOrDefault();
            vm.Operation = "update";
            return PartialView("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new AdjustmentRepo(identity, Session);
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
            AdjustmentVM vm = new AdjustmentVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.LastModifiedBy = identity.Name;
            //result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
