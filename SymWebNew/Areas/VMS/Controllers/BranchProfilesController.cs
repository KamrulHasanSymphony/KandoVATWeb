using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VATViewModel.DTOs;
using SymVATWebUI.Areas.VMS.Models;
using SymOrdinary;
using System.Threading;
using SymRepository.VMS;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.VMS.Controllers
{
    [ShampanAuthorize]
    public class BranchProfilesController : Controller
    {
        ShampanIdentity identity = null;

        BranchRepo _repo = null;

        public BranchProfilesController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new BranchRepo(identity);

            }
            catch
            {

            }
        }
        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //BranchRepo _repo = new BranchRepo();

        //SelectAll
        // GET: /VMS/BranchProfiles/

        public ActionResult Index()
        {

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BranchRepo(identity, Session);


            var getAllData = _repo.SelectAll();
            IEnumerable<BranchProfileVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                //var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.BranchCode.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.BranchName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.BIN.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Address.ToLower().Contains(param.sSearch.ToLower())
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
            //var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<BranchProfileVM, string> orderingFunction = (c =>
                sortColumnIndex == 0 && isSortable_1 ? c.BranchCode :
                sortColumnIndex == 1 && isSortable_2 ? c.BranchName :
                sortColumnIndex == 2 && isSortable_3 ? c.BIN :
                sortColumnIndex == 3 && isSortable_4 ? c.Address :

                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { c.BranchID.ToString(),
                  c.BranchCode                
                , c.BranchName
                , c.BIN
                , c.Address
               
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

        public ActionResult Create()
        {
            //string project = new System.Configuration.AppSettingsReader().GetValue("BranchName", typeof(string)).ToString();

            //if (project.ToLower() == "vms")
            //{
            //    if (!identity.IsAdmin)
            //    {
            //        Session["rollPermission"] = "deny";
            //        return Redirect("/vms/Home");
            //    }
            //}
            //else
            //{
            //    Session["rollPermission"] = "deny";
            //    return Redirect("/vms/Home");
            //}
            BranchProfileVM vm = new BranchProfileVM();
            vm.Operation = "add";
            vm.ActiveStatus = "Y";
            return View(vm);



            // return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(BranchProfileVM VM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BranchRepo(identity, Session);

            //string project = new System.Configuration.AppSettingsReader().GetValue("BranchName", typeof(string)).ToString();
            //if (project.ToLower() == "VMs")
            //{
            //    if (!identity.IsAdmin)
            //    {
            //        Session["rollPermission"] = "deny";
            //        return Redirect("/VMs/Home");
            //    }
            //}
            //else
            //{
            //    Session["rollPermission"] = "deny";
            //    return Redirect("/VMs/Home");
            //}
            string[] result = new string[6];
            try
            {
                if (VM.Operation.ToLower() == "add")
                {
                    VM.CreatedOn = DateTime.Now.ToString();
                    VM.CreatedBy = identity.Name;
                    //VM.ActiveStatus = "Y";
                    //VM.LastModifiedOn = DateTime.Now.ToString();
                    result = _repo.InsertBranch(VM);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("Create", VM);
                    }
                }
                else if (VM.Operation.ToLower() == "update")
                {
                    VM.LastModifiedOn = DateTime.Now.ToString();
                    VM.LastModifiedBy = identity.Name;
                    result = _repo.UpdateBranchInformation(VM);
                    Session["result"] = result[0] + "~" + result[1];
                    ////return RedirectToAction("Edit", new { id = result[2] });

                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("Create", VM);
                    }

                }
                else
                {
                    return View("Create", VM);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                // FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("BranchProfilesController", "CreateEdit", ex.ToString());
                return View("Create", VM);
            }

        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BranchRepo(identity, Session);

            BranchProfileVM VM = new BranchProfileVM();

            VM = _repo.SelectAll(id).FirstOrDefault();
            VM.Operation = "update";
            return View("Create", VM);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BranchRepo(identity, Session);

            BranchProfileVM VM = new BranchProfileVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            VM.LastModifiedOn = DateTime.Now.ToString();
            VM.LastModifiedBy = identity.Name;
            result = _repo.DeleteBranch(VM, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "Admin")]
        public JsonResult getBranchDetails(string BranchName)
        {
            var repo = new BranchRepo(identity, Session);
            var id = 0;
            string[] conditionalFields;
            string[] conditionalValues;

            try
            {
                conditionalFields = new string[] { "BranchName" };
                conditionalValues = new string[] { BranchName };
            }
            catch (Exception)
            {
                throw;
            }
            var customer = repo.SelectAll(null, conditionalFields, conditionalValues).FirstOrDefault();
            return Json(customer, JsonRequestBehavior.AllowGet);
        }

    }
}
