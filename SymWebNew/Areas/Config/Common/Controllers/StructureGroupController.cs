using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymVATWebUI.Areas.Common.Controllers
{
    [Authorize]
    public class StructureGroupController : Controller
    {
        //
        // GET: /Common/StructureGroup/
        StructureGroupRepo repo = new StructureGroupRepo();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _index(JQueryDataTableParamVM param)
        {

            var getAllData = repo.SelectAll();
            IEnumerable<StructureGroupVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Remarks.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<StructureGroupVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.LeaveStructureId :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.SalaryStructureId :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.EmployeeGroupId :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { Convert.ToString(c.Id), c.Name + "~" + Convert.ToString(c.Id), c.LeaveStructure, c.SalaryStructureId, c.EmployeeGroupId };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            return PartialView();
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(StructureGroupVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = Identity.Name;
            vm.CreatedFrom = Identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(Identity.BranchId);
            try
            {
                
                result = repo.Insert(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            return PartialView(repo.SelectById(id));
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(StructureGroupVM vm, string btn)
        {
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = Identity.Name;
            vm.LastUpdateFrom = Identity.WorkStationIP;
            try
            {
                
                result = repo.Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Updated";

                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult Delete(string ids)
        {
            StructureGroupVM vm = new StructureGroupVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }

}
