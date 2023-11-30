using CrystalDecisions.CrystalReports.Engine;
using SymOrdinary;
using SymRepository.VMS;
using VATViewModel.DTOs;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading.Tasks;
using System.Net.Http;
using Excel.Log.Logger;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymVATWebUI.Filters;
using VATServer.Library;
using VATServer.Ordinary;
using SymphonySofttech.Utilities;
using System.Text;
using Elmah;


namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class MPLBankPaymentReceiveController : Controller
    {
        ShampanIdentity identity = null;

        MPLBankPaymentReceiveRepo _repo = null;

        public MPLBankPaymentReceiveController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new MPLBankPaymentReceiveRepo(identity);
            }
            catch
            {

            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Menu()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult HomeIndex()
        {
            return View();
        }

        [UserFilter]
        public ActionResult Index(MPLBankPaymentReceiveVM paramVM)
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

            paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());

            #region BranchList

            int userId = Convert.ToInt32(identity.UserId);
            var list = new SymRepository.VMS.BranchRepo(identity).UserDropDownBranchProfile(userId);

            var listBranch = new SymRepository.VMS.BranchRepo(identity).SelectAll();

            if (list.Count() == listBranch.Count())
            {
                list.Add(new BranchProfileVM() { BranchID = -1, BranchName = "All" });
            }

            paramVM.BranchList = list;

            #endregion

            return View(paramVM);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamVM param, MPLBankPaymentReceiveVM paramVM)
        {
            _repo = new MPLBankPaymentReceiveRepo(identity, Session);

            List<MPLBankPaymentReceiveVM> getAllData = new List<MPLBankPaymentReceiveVM>();

            #region Access Controll

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

            #endregion

            #region SeachParameters

            string dtFrom = null;
            string dtTo = null;
            paramVM.SelectTop = paramVM.SelectTop ==  null ? "100" : paramVM.SelectTop;
            if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
            {
                dtFrom = Convert.ToDateTime(paramVM.FromDate).ToString("yyyyMMdd");
            }
            if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
            {
                dtTo = Convert.ToDateTime(paramVM.ToDate).AddDays(1).ToString("yyyyMMdd");
            }

            if (paramVM.BranchId == 0)
            {
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            }

            if (paramVM.BranchId == -1)
            {
                paramVM.BranchId = 0;
            }

            #endregion SeachParameters

            #region Data Call

            string[] conditionFields;
            string[] conditionValues;

            conditionFields = new string[] { "BPR.InstrumentDate>=", "BPR.InstrumentDate<=", "BPR.BDBankId", "BPR.BranchId" };
            conditionValues = new string[] { dtFrom, dtTo, paramVM.BDBankId.ToString(), paramVM.BranchId.ToString() };

            getAllData = _repo.SelectAll(0, conditionFields, conditionValues, null, null, "Y",paramVM.SelectTop);


            #endregion

            #region Search and Filter Data
            IEnumerable<MPLBankPaymentReceiveVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
               
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.InstrumentDate.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.Amount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<MPLBankPaymentReceiveVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.InstrumentDate : sortColumnIndex == 2 && isSortable_2 ? c.Amount.ToString() :

                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id+"~"+ c.BDBankId+"~"+ c.BranchId
                , c.BDBankName
                , c.CustomerName           
                , c.ModeOfPayment           
                , c.InstrumentDate  
                , c.Amount.ToString()
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
            MPLBankPaymentReceiveVM vm = new MPLBankPaymentReceiveVM();
            CommonRepo commonRepo = new CommonRepo(identity, Session);
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            string FormNumeric = commonRepo.settings("DecimalPlace", "FormNumeric");
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    //
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }

            vm.Operation = "add";
            vm.InstrumentDate = Session["SessionDate"].ToString();

            return View(vm);
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult CreateEdit(MPLBankPaymentReceiveVM vm)
        {
            try
            {
                _repo = new MPLBankPaymentReceiveRepo(identity, Session);
                string[] result = new string[6];
                vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                if (!string.IsNullOrEmpty(vm.InstrumentDate))
                { vm.InstrumentDate = Convert.ToDateTime(vm.InstrumentDate).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"); }
               
                try
                {
                    string UserId = identity.UserId;

                    if (vm.Operation.ToLower() == "add")
                    {
                        result = _repo.InsertToMPLBankPaymentReceive(vm, null, null);

                        if (result[0] == "Success")
                        {
                            Session["result"] = result[0] + "~" + result[1];
                            return RedirectToAction("Edit", new { id = result[4]});
                        }
                        else
                        {
                            string msg = result[1].Split('\r').FirstOrDefault();

                            Session["result"] = result[0] + "~" + msg;

                            return View("Create", vm);
                        }
                    }
                    else if (vm.Operation.ToLower() == "update")
                    {

                        result = _repo.UpdateMPLBankPaymentReceive(vm, null, null);

                        if (result[0] == "Success")
                        {
                            Session["result"] = result[0] + "~" + result[1];
                            return RedirectToAction("Edit", new { id = vm.Id });
                        }
                        else
                        {
                            string msg = result[1].Split('\r').FirstOrDefault();
                            Session["result"] = result[0] + "~" + msg;

                            return View("Create", vm);
                        }
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
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    return RedirectToAction("Edit", new { id = vm.Id });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        [ShampanAuthorize]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            MPLBankPaymentReceiveVM vm = new MPLBankPaymentReceiveVM();

            try
            {
                _repo = new MPLBankPaymentReceiveRepo(identity, Session);
               
                vm = _repo.SelectAll(Convert.ToInt32(id), null, null, null, null, null).FirstOrDefault();

                if (vm == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                vm.Operation = "update";
                
                return View("Create", vm);
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index");
            }
        }
        

    }
}
