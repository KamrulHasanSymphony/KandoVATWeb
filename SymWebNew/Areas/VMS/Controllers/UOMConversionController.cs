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
    public class UOMConversionController : Controller
    {
        //
        // GET: /vms/Branch/
        ShampanIdentity identity = null;

        UOMRepo _repo = null;

        public UOMConversionController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new UOMRepo(identity);

            }
            catch
            {

            }
        }

        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //UOMRepo _repo = new UOMRepo();

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
            _repo = new UOMRepo(identity, Session);

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
            //00     //UOMId
            //01     //UOMFrom
            //02     //UOMTo
            //03     //Convertion
            //04     //CTypes


            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<UOMConversionVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.UOMFrom.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.UOMTo.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Convertion.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.CTypes.ToLower().Contains(param.sSearch.ToLower())
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
            Func<UOMConversionVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.UOMFrom :
                sortColumnIndex == 2 && isSortable_2 ? c.UOMTo :
                sortColumnIndex == 3 && isSortable_3 ? c.Convertion.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.CTypes :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                  c.UOMId
                , c.UOMFrom
                , c.UOMTo
                , c.Convertion.ToString()
                , c.CTypes
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
            UOMConversionVM vm = new UOMConversionVM();
            vm.Operation = "add";
            vm.ActiveStatus = "Y";
            return PartialView(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(UOMConversionVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new UOMRepo(identity, Session);

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
                    vm.CreatedOn = DateTime.Now.ToString();
                    vm.CreatedBy = identity.Name;
                    //vm.ActiveStatus = true;
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    result = _repo.InsertToUOM(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    vm.LastModifiedBy = identity.Name;
                    result = _repo.UpdateUOM(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                //  Session["result"] = "Fail~Data Not Succeessfully!";
                //  FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("UOMConversionController", "CreateEdit", ex.ToString());
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new UOMRepo(identity, Session);

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
            UOMConversionVM vm = new UOMConversionVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return PartialView("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new UOMRepo(identity, Session);

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
            UOMConversionVM vm = new UOMConversionVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.LastModifiedBy = identity.Name;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
