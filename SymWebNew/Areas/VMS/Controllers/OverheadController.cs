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

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class OverheadController : Controller
    {
        ShampanIdentity identity = null;

        ProductRepo _repo = null;

        public OverheadController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new ProductRepo(identity);

            }
            catch
            {

            }
        }
        // ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //  ProductRepo _repo = new ProductRepo();
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
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ProductRepo(identity, Session);

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
            //00     //Id 
            //01     //ProductName
            //02     //CostPrice
            //03     //SalesPrice  
            //04     //VATRate
            //05     //OpeningDate
            //06    //RebatePercent


            #region Search and Filter Data
            //string[] conditionFields = { "RebatePercent>" };
            //string[] conditionValues = { "1" };
            var getAllData = _repo.SelectAllOverhead("0");
            //var getAllData = _repo.SelectAll();
            IEnumerable<ProductVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);


                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.ProductName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.CostPrice.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.SalesPrice.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.VATRate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.OpeningDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.RebatePercent.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.ProductCode.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);


            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ProductVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.ProductName :
                sortColumnIndex == 2 && isSortable_2 ? c.CostPrice.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.SalesPrice.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.VATRate.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.OpeningDate.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.RebatePercent.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.ProductCode.ToString() :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                c.ItemNo.ToString()
                , c.ProductCode
                , c.ProductName
                , c.CostPrice.ToString()
                , c.SalesPrice.ToString()
                , c.VATRate.ToString()+"%"
                , c.OpeningDate.ToString()
                , c.HSCodeNo
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
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
            }
            ProductVM vm = new ProductVM();
            vm.Operation = "add";
            vm.ActiveStatus = "Y";
            vm.IsFixedVATRebate = "N";
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(ProductVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ProductRepo(identity, Session);

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
            string[] result = new string[6];
            try
            {
                vm.OpeningDate = DateTime.Now.ToString("dd-MMM-yyyy");

                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedOn = DateTime.Now.ToString();
                    vm.CreatedBy = identity.Name;
                    vm.ActiveStatus = "Y";
                    vm.IsConfirmed = "Y";
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    vm.UOM = "-";
                    result = _repo.InsertToProduct(vm, new List<TrackingVM>(), vm.Type);
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
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    vm.LastModifiedBy = identity.Name;
                    result = _repo.UpdateProduct(vm, new List<TrackingVM>(), vm.Type);
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
                // Session["result"] = "Fail~Data Not Succeessfully!";
                //  FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("OverheadController", "CreateEdit", ex.ToString());
                return View("Create", vm);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ProductRepo(identity, Session);

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
            ProductVM vm = new ProductVM();
            vm = _repo.SelectAll(id).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ProductRepo(identity, Session);

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
            ProductVM vm = new ProductVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.LastModifiedBy = identity.Name;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        public ActionResult Navigate(string id, string btn)
        {
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetId("Products", "Id", id, btn);
            return RedirectToAction("Edit", new { id = targetId });
        }
    }
}
