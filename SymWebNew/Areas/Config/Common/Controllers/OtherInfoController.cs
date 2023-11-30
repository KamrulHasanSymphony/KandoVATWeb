using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymVATWebUI.Areas.Common.Controllers
{
    public class OtherInfoController : Controller
    {
        //
        // GET: /Common/OtherInfo/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        OtherInfoRepo _repo = new OtherInfoRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string infoType = "")
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            OtherInfoVM vm = new OtherInfoVM();
            vm.InfoType = infoType;
            if (infoType == "Other1")
            {
                vm.LabelInfoType = new AppSettingsReader().GetValue("LabelOther1", typeof(string)).ToString();
            }
            else if (infoType == "Other2")
            {
                vm.LabelInfoType = new AppSettingsReader().GetValue("LabelOther2", typeof(string)).ToString();
            }
            else if (infoType == "Other3")
            {
                vm.LabelInfoType = new AppSettingsReader().GetValue("LabelOther3", typeof(string)).ToString();
            }

            return View(vm);
        }
        public ActionResult _index(JQueryDataTableParamModel param, string infoType = "")
        {
            //00     //Id 
            //01     //Code      
            //02     //Name       
            //03     //Remarks
            #region Search and Filter Data
            string[] conditionField = { "InfoType" };
            string[] conditionValue = { infoType };

            var getAllData = _repo.SelectAll("", conditionField, conditionValue);
            IEnumerable<OtherInfoVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Remarks.ToString().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<OtherInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.Name :
                sortColumnIndex == 3 && isSortable_3 ? c.Remarks :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                Convert.ToString(c.Id)
                , c.Code 
                , c.Name 
                , c.Remarks 
     
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
        public ActionResult Create(string infoType = "")
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            OtherInfoVM vm = new OtherInfoVM();
            vm.Operation = "add";
            vm.InfoType = infoType;
            return PartialView(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(OtherInfoVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                vm.BranchId = Convert.ToInt32(identity.BranchId);
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Index", new { infoType = vm.InfoType });

                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Index", new { infoType = vm.InfoType });
                }
                else
                {
                    return RedirectToAction("Index", new { infoType = vm.InfoType });
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index", new { infoType = vm.InfoType });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            OtherInfoVM vm = new OtherInfoVM();
            vm = _repo.SelectAll(id).FirstOrDefault();
            vm.Operation = "update";
            return PartialView("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            OtherInfoVM vm = new OtherInfoVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

    }
}
