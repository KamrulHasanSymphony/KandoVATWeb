using SymOrdinary;
using SymRepository.VMS;
using SymVATWebUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using VATViewModel.DTOs;

namespace SymVATWebUI.Areas.VMS.Controllers
{
    [ShampanAuthorize]
    public class CustomsHouseController : Controller
    {
        //
        // GET: /VMS/CustomsHouse/

        ShampanIdentity identity = null;

        CustomsHouseRepo _repo = null;

        public CustomsHouseController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new CustomsHouseRepo(identity);

            }
            catch 
            {
               
            }
        }
        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //CustomsHouseRepo _repo = new CustomsHouseRepo();


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

            _repo = new CustomsHouseRepo(identity, Session);

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

            #region Search and Filter Data
            var getAllData = _repo.SelectAllLst();
            IEnumerable<CustomsHouseVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);


                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.CustomsHouseName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.CustomsHouseAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.ActiveStatus.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<CustomsHouseVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.CustomsHouseName :
                sortColumnIndex == 3 && isSortable_3 ? c.CustomsHouseAddress.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.ActiveStatus.ToString() :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                c.ID
                , c.Code
                , c.CustomsHouseName
                , c.CustomsHouseAddress
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
            CustomsHouseVM vm = new CustomsHouseVM();
            vm.Operation = "add";
            vm.ActiveStatus = "Y";
            return View(vm);
        }



        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(CustomsHouseVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new CustomsHouseRepo(identity, Session);

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
                    result = _repo.InsertToCustomsHouse(vm);
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
                    result = _repo.UpdateToCustomsHouse(vm);
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
                // FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("CustomsHouseController", "CreateEdit", ex.ToString());
                return View("Create", vm);
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new CustomsHouseRepo(identity, Session);

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
            CustomsHouseVM vm = new CustomsHouseVM();
            vm = _repo.SelectAllLst(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new CustomsHouseRepo(identity, Session);

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
            CustomsHouseVM vm = new CustomsHouseVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.LastModifiedBy = identity.Name;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetCustomHousePopUp(CustomsHouseVM vm)
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
            return PartialView("_CustomHouse", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredCustomHouse(CustomsHouseVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CustomsHouseRepo(identity, Session);

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
            string[] conditionalFields;
            string[] conditionalValues;

            conditionalFields = new string[] { };
            conditionalValues = new string[] { };
            //if (vm.SearchField != null)
            //{
            //    conditionalFields = new string[] { "v." + vm.SearchField + " like", "v.VendorGroupID", "v.ActiveStatus", "v.StartDateTime>=", "v.StartDateTime<=" };
            //    conditionalValues = new string[] { vm.SearchValue, vm.VendorGroupID, vm.IsActive, vm.StartDateFrom, vm.StartDateTo };
            //}
            //else
            //{
            //    conditionalFields = new string[] { "v.VendorGroupID", "v.ActiveStatus", "v.StartDateTime>=", "v.StartDateTime<=" };
            //    conditionalValues = new string[] { vm.VendorGroupID, vm.IsActive, vm.StartDateFrom, vm.StartDateTo };
            //}
            var list = _repo.SelectAllLst(0, conditionalFields, conditionalValues);

            return PartialView("_filteredCustomHouse", list);
        }


    }
}
