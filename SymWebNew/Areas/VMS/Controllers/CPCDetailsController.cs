//using JQueryDataTables.Models;
using SymOrdinary;
using SymphonySofttech.Reports.Report;
using SymRepository.VMS;
using SymVATWebUI.Areas.VMS.Models;
using VATViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymVATWebUI.Filters;


namespace SymVATWebUI.Areas.VMS.Controllers
{
    [ShampanAuthorize]
    public class CPCDetailsController : Controller
    {
        //
        // GET: /VMS/CPCDetails/

        ShampanIdentity identity = null;

        CPCDetailsRepo _repo = null;

        public CPCDetailsController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new CPCDetailsRepo(identity);
            }
            catch
            {

            }

        }
        public ActionResult HomeIndex()
        {
            return View();
        }

        [UserFilter]
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
            _repo = new CPCDetailsRepo(identity, Session);
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
            //01     //Name
            //02     //IsRaw  
            //03     //HSCodeNo
            //04     //VATRate
            //05     //PropergatingRate


            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<CPCDetailsVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                //var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                //var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);


                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Type.ToString().ToLower().Contains(param.sSearch.ToLower())
                    //|| isSearchable4 && c.VATRate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    //|| isSearchable5 && c.PropergatingRate.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            //var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            //var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<CPCDetailsVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.Name :
                sortColumnIndex == 3 && isSortable_3 ? c.Type.ToString() :
                    //sortColumnIndex == 4 && isSortable_4 ? c.VATRate.ToString() :
                    //sortColumnIndex == 5 && isSortable_5 ? c.PropergatingRate.ToString() :
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
                , c.Code.ToString()
                , c.Name
                , c.Type
                //, c.VATRate.ToString()
                //, c.PropergatingRate.ToString()
                //, c.ActiveStatus
                //, c.CreatedBy
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
            CPCDetailsVM vm = new CPCDetailsVM();
            vm.Operation = "add";
            // vm.ActiveStatus = "Y";
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(CPCDetailsVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CPCDetailsRepo(identity, Session);
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
                    //vm.Code = txtCode.Text.Trim();
                    //vm.Name = txtName.Text.Trim();
                    //vm.Type = cmbType.Text.Trim();

                    vm.CreatedBy = identity.Name;
                    vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    //vm.ActiveStatus = true;
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    result = _repo.InsertToCPCDetails(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        string msg = result[1].Split('\r').FirstOrDefault();
                        Session["result"] = "Fail~" + msg;
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    //vm.Id = Convert.ToInt32(NextID);
                    //vm.Code = txtCode.Text.Trim();
                    //vm.Name = txtName.Text.Trim();
                    //vm.Type = cmbType.Text.Trim();
                    //vm.LastModifiedBy = Program.CurrentUser;
                    //vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    vm.LastModifiedOn = DateTime.Now.ToString();
                    vm.LastModifiedBy = identity.Name;
                    result = _repo.UpdateCPCDetails(vm);
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
                FileLogger.Log("ProductCategoryController", "CreateEdit", ex.ToString());
                return View("Create", vm);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CPCDetailsRepo(identity, Session);
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
            CPCDetailsVM vm = new CPCDetailsVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CPCDetailsRepo(identity, Session);

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
            CPCDetailsVM vm = new CPCDetailsVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.LastModifiedBy = identity.Name;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
