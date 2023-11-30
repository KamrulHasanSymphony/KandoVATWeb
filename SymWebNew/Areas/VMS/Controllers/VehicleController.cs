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
using VATServer.Ordinary;
using System.IO;
using System.Data;

namespace SymVATWebUI.Areas.vms.Controllers
{
    [ShampanAuthorize]
    public class VehicleController : Controller
    {
        //
        // GET: /vms/Branch/
        ShampanIdentity identity = null;

        VehicleRepo _repo = null;

        public VehicleController()
        {
            try
            {

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new VehicleRepo(identity);

            }
            catch
            {

            }
        }

        // ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        // VehicleRepo _repo = new VehicleRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index(VehicleVM paramVM)
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
            return View(paramVM);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param, VehicleVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new VehicleRepo(identity, Session);

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
            //00     //VehicleID 
            //01     //VehicleCode
            //02     //VehicleType
            //03     //VehicleNo
            //04     //DriverName

            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "100";
            }
            #region Search and Filter Data
            string[] cFields = { "VehicleCode like", "VehicleNo like", "VehicleType like", "ActiveStatus", "SelectTop" };
            string[] cValues = { paramVM.Code, paramVM.VehicleNo, paramVM.VehicleType, paramVM.IsActive, paramVM.SelectTop };
            var getAllData = _repo.SelectAll(0, cFields, cValues);
            IEnumerable<VehicleVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);


                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.VehicleType.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.VehicleNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.DriverName.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<VehicleVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.VehicleType :
                sortColumnIndex == 3 && isSortable_3 ? c.VehicleNo.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.DriverName.ToString() :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                  c.VehicleID 
                , c.Code
                , c.VehicleType
                , c.VehicleNo
                , c.DriverName
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
            VehicleVM vm = new VehicleVM();
            vm.Operation = "add";
            vm.ActiveStatus = "Y";
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(VehicleVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new VehicleRepo(identity, Session);

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
                    result = _repo.InsertToVehicle(vm);
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
                    result = _repo.UpdateToVehicle(vm);
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
                FileLogger.Log("VehicleController", "CreateEdit", ex.ToString());
                return View("Create", vm);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new VehicleRepo(identity, Session);

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
            VehicleVM vm = new VehicleVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }


        [Authorize]
        public ActionResult ExportExcell(VehicleVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new VehicleRepo(identity, Session);

            ResultVM rVM = new ResultVM();

            List<VehicleVM> getAllData = new List<VehicleVM>();
            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "All";
            }
            try
            {

                if (paramVM.ExportAll)
                {
                    string[] conditionFields = new string[] { "SelectTop" };
                    string[] conditionValues = new string[] { paramVM.SelectTop };

                    List<VehicleVM> Vehicles = _repo.SelectAll(0, conditionFields, conditionValues);

                    paramVM.VehicleIDs = Vehicles.Select(x => x.Code).ToList(); ;

                }


                DataTable dt = _repo.GetExcelData(paramVM.VehicleIDs);
                //var address = _repo.GetExcelAddress(paramVM.VehicleIDs);

                var dataSet = new DataSet();
                dataSet.Tables.Add(dt);

                var sheetNames = new[] { "Vehicle" };

                //if (address.Rows.Count > 0)
                //{
                //    dataSet.Tables.Add(address);
                //    sheetNames = new[] { "Vehicle", "VehicleAddress" };
                //}

                var vm = OrdinaryVATDesktop.DownloadExcelMultiple(dataSet, "Vehicle", sheetNames);
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + vm.FileName + ".xlsx");
                    vm.varExcelPackage.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }
            catch (Exception)
            {


            }
            finally { }
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new VehicleRepo(identity, Session);

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
            VehicleVM vm = new VehicleVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.LastModifiedBy = identity.Name;
            //  result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        public ActionResult Navigate(string id, string btn)
        {
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetId("Vehicles", "VehicleID", id, btn);
            return RedirectToAction("Edit", new { id = targetId });
        }
    }
}
