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
    public class MPLBankDepositSlipController : Controller
    {
        ShampanIdentity identity = null;

        MPLBankDepositSlipRepo _repo = null;

        public MPLBankDepositSlipController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new MPLBankDepositSlipRepo(identity);
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
        public ActionResult Index(MPLBankDepositSlipHeaderVM paramVM)
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
            if (string.IsNullOrWhiteSpace(paramVM.TransactionType))
            {
                paramVM.TransactionType = "Other";
            }

            ViewBag.TransactionType = paramVM.TransactionType;

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
        public ActionResult _index(JQueryDataTableParamVM param, MPLBankDepositSlipHeaderVM paramVM)
        {
            _repo = new MPLBankDepositSlipRepo(identity, Session);

            List<MPLBankDepositSlipHeaderVM> getAllData = new List<MPLBankDepositSlipHeaderVM>();

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

            conditionFields = new string[] { "DSH.TransactionDateTime>=", "DSH.TransactionDateTime<=", "DSH.SelfBankId", "DSH.BranchId" };
            conditionValues = new string[] { dtFrom, dtTo, paramVM.SelfBankId.ToString(), paramVM.BranchId.ToString() };

            getAllData = _repo.SelectAll(0, conditionFields, conditionValues, null, null,paramVM.TransactionType, "Y",paramVM.SelectTop);


            #endregion

            #region Search and Filter Data
            IEnumerable<MPLBankDepositSlipHeaderVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
               
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.TransactionDateTime.ToLower().Contains(param.sSearch.ToLower())
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
            Func<MPLBankDepositSlipHeaderVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.TransactionDateTime : sortColumnIndex == 2 && isSortable_2 ? c.Amount.ToString() :

                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id+"~"+ c.Code+"~"+ c.BranchId
                , c.Code
                , c.SelfBankName            
                , c.TransactionDateTime.ToString()               
                , c.TotalAmount.ToString()
                , c.TransactionType

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
        public ActionResult Create(string tType)
        {
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

            MPLBankDepositSlipHeaderVM vm = new MPLBankDepositSlipHeaderVM();
            List<MPLBankDepositSlipDetailVM> detailVMs = new List<MPLBankDepositSlipDetailVM>();
            vm.MPLBankDepositSlipDetailVMs = detailVMs;

            vm.Operation = "add";
            vm.TransactionType = "Other";
            vm.TransactionDateTime = Session["SessionDate"].ToString();

            return View(vm);
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult CreateEdit(MPLBankDepositSlipHeaderVM vm)
        {
            try
            {
                _repo = new MPLBankDepositSlipRepo(identity, Session);
                string[] result = new string[6];
                vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                if (!string.IsNullOrEmpty(vm.TransactionDateTime))
                { vm.TransactionDateTime = Convert.ToDateTime(vm.TransactionDateTime).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"); }
               
                try
                {
                    string UserId = identity.UserId;

                    if (vm.Operation.ToLower() == "add")
                    {
                        vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                        vm.CreatedBy = identity.Name;

                        result = _repo.InsertToMPLBankDepositSlip(vm, null, null);

                        if (result[0] == "Success")
                        {
                            Session["result"] = result[0] + "~" + result[1];
                            return RedirectToAction("Edit", new { id = result[4], TransactionType = vm.TransactionType });
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
                        vm.LastModifiedBy = identity.Name;
                        vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");

                        result = _repo.UpdateMPLBankDepositSlip(vm, null, null);

                        if (result[0] == "Success")
                        {
                            Session["result"] = result[0] + "~" + result[1];
                            return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.TransactionType });
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
                    return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.TransactionType });
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
        public ActionResult Edit(string id, string TransactionType)
        {
            MPLBankDepositSlipHeaderVM vm = new MPLBankDepositSlipHeaderVM();

            try
            {
                _repo = new MPLBankDepositSlipRepo(identity, Session);
                if (TransactionType == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                vm = _repo.SelectAll(Convert.ToInt32(id), null, null, null, null, null, TransactionType).FirstOrDefault();

                if (vm == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                List<MPLBankDepositSlipDetailVM> detailVMs = new List<MPLBankDepositSlipDetailVM>();
                detailVMs = _repo.SearchMPLBankDepositSlipDetailList(id);
                vm.MPLBankDepositSlipDetailVMs = detailVMs;

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
        
        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(MPLBankDepositSlipDetailVM vm)
        {
            return PartialView("_detail", vm);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetMPLBankPaymentPopUp(MPLBankDepositSlipHeaderVM paramVM)
        {
            try
            {
                string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

                string dtFrom = null;
                string dtTo = null;
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    dtFrom = Convert.ToDateTime(paramVM.FromDate).ToString("yyyy-MM-dd");
                }
                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    dtTo = Convert.ToDateTime(paramVM.ToDate).AddDays(1).ToString("yyyy-MM-dd");
                }

                string[] conditionFields;
                string[] conditionValues;

                //conditionFields = new string[] { "bp.InstrumentDate>=", "bp.InstrumentDate<=", "bb.BankName", "sh.SalesInvoiceNo", "bp.InstrumentNo", "bp.ModeOfPayment", "bp.BranchId" };
                conditionFields = new string[] { "d.InstrumentDate>=", "d.InstrumentDate<=", "bb.BankName", "bb.BankCode", "c.CustomerName", "sh.SalesInvoiceNo", "d.InstrumentNo", "d.ModeOfPayment", "d.BranchId" };
                conditionValues = new string[] { dtFrom, dtTo, paramVM.SelfBankName, paramVM.BankCode, paramVM.CustomerName, paramVM.SalesInvoiceNo, paramVM.InstrumentNo, paramVM.ModeOfPayment, paramVM.BranchId.ToString() };

                MPLBankDepositSlipHeaderVM vm = new MPLBankDepositSlipHeaderVM();
                List<MPLBankDepositSlipDetailVM> detailVMs = new List<MPLBankDepositSlipDetailVM>();
                detailVMs = _repo.GetMPLBankDepositSlipDetailList(paramVM.BranchId.ToString(), conditionFields, conditionValues);
                vm.MPLBankDepositSlipDetailVMs = detailVMs;

                return PartialView("_depositSlipDetail", vm);
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult SearchMPLBankPaymentPopUp(MPLBankDepositSlipHeaderVM paramVM)
        {
            try
            {
                string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

                string dtFrom = null;
                string dtTo = null;
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    dtFrom = Convert.ToDateTime(paramVM.FromDate).ToString("yyyy-MM-dd");
                }
                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    dtTo = Convert.ToDateTime(paramVM.ToDate).AddDays(1).ToString("yyyy-MM-dd");
                }

                string[] conditionFields;
                string[] conditionValues;

                if (paramVM.SearchField != null)
                {
                    conditionFields = new string[] { paramVM.SearchField + " like", "bp.InstrumentDate>=", "bp.InstrumentDate<=", "bp.BranchId", "bb.BankName", "bb.BankCode", "c.CustomerName", "", "", "" };
                    conditionValues = new string[] { paramVM.SearchValue, dtFrom, dtTo, paramVM.BranchId.ToString(), paramVM.BankCode, paramVM.CustomerName, "", "", "" };
                }
                else
                {
                    //conditionFields = new string[] { "bp.InstrumentDate>=", "bp.InstrumentDate<=", "bb.BankName", "sh.SalesInvoiceNo", "bp.InstrumentNo", "bp.ModeOfPayment", "bp.BranchId" };
                    conditionFields = new string[] { "d.InstrumentDate>=", "d.InstrumentDate<=", "bb.BankName", "bb.BankCode", "c.CustomerName", "d.SalesInvoiceNo", "d.InstrumentNo", "d.ModeOfPayment", "d.BranchId" };
                    conditionValues = new string[] { dtFrom, dtTo, paramVM.SelfBankName, paramVM.BankCode, paramVM.CustomerName, paramVM.SalesInvoiceNo, paramVM.InstrumentNo, paramVM.ModeOfPayment, paramVM.BranchId.ToString() };
                }

                List<MPLBankDepositSlipDetailVM> detailVMs = new List<MPLBankDepositSlipDetailVM>();
                detailVMs = _repo.GetMPLBankDepositSlipDetailList(paramVM.BranchId.ToString(), conditionFields, conditionValues);

                return PartialView("_filteredDepositSlipDetail", detailVMs);
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
