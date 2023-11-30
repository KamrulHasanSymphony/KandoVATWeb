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
    public class TransferIssueMPLController : Controller
    {
        ShampanIdentity identity = null;

        TransferIssueMPLRepo _repo = null;

        public TransferIssueMPLController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new TransferIssueMPLRepo(identity);
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
        public ActionResult Index(TransferMPLIssueVM paramVM)
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
        public ActionResult _index(JQueryDataTableParamVM param, TransferMPLIssueVM paramVM)
        {
            _repo = new TransferIssueMPLRepo(identity, Session);

            List<TransferMPLIssueVM> getAllData = new List<TransferMPLIssueVM>();

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

            conditionFields = new string[] { "TI.TransferDateTime>=", "TI.TransferDateTime<=", "TI.Post", "TI.BranchId" };
            conditionValues = new string[] { dtFrom, dtTo, paramVM.Post, paramVM.BranchId.ToString()};

            getAllData = _repo.SelectAll(0, conditionFields, conditionValues, null, null,paramVM.TransactionType, "Y",paramVM.SelectTop);


            #endregion

            #region Search and Filter Data
            IEnumerable<TransferMPLIssueVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
               
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.TransferIssueNo.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.TransferDateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.TotalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.TotalVATAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
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

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<TransferMPLIssueVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.TransferIssueNo :                                                                        sortColumnIndex == 2 && isSortable_2 ? c.TransferDateTime.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.TotalAmount.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.TotalVATAmount.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Post.ToString() :

                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id+"~"+ c.Post+"~"+ c.BranchId
                , c.TransferIssueNo
                , c.TransferDateTime.ToString()             
                , c.TotalAmount.ToString()               
                , c.TotalVATAmount.ToString()     
                , c.TransferToBranch.ToString()     
                , c.Post=="Y" ? "Posted" : "Not Posted"
                , c.IsTransfer=="Y" ? "Transfered" : "Not Transfered"
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

            TransferMPLIssueVM vm = new TransferMPLIssueVM();
            List<TransferMPLIssueDetailVM> detailVMs = new List<TransferMPLIssueDetailVM>();
            vm.TransferMPLIssueDetailVMs = detailVMs;

            vm.Operation = "add";
            vm.TransactionType = tType;
            vm.TransferDateTime = Session["SessionDate"].ToString();

            return View(vm);
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult CreateEdit(TransferMPLIssueVM vm)
        {
            try
            {
                _repo = new TransferIssueMPLRepo(identity, Session);
                string[] result = new string[6];

                if (!string.IsNullOrEmpty(vm.TransferDateTime))
                { vm.TransferDateTime = Convert.ToDateTime(vm.TransferDateTime).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"); }
                if (!string.IsNullOrEmpty(vm.ReceiveDateTime))
                { vm.ReceiveDateTime = Convert.ToDateTime(vm.ReceiveDateTime).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"); }

                try
                {
                    string UserId = identity.UserId;

                    if (vm.Operation.ToLower() == "add")
                    {
                        vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                        vm.CreatedBy = identity.Name;
                        vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                        vm.BranchFromRef = vm.BranchId.ToString();

                        result = _repo.TransIssueMPLInsert(vm, null, null);

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
                        vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                        result = _repo.TransIssueMPLUpdate(vm, null, null);

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
            TransferMPLIssueVM vm = new TransferMPLIssueVM();

            try
            {
                _repo = new TransferIssueMPLRepo(identity, Session);
                if (TransactionType == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                vm = _repo.SelectAll(Convert.ToInt32(id), null, null, null, null, null, TransactionType).FirstOrDefault();

                if (vm == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                List<TransferMPLIssueDetailVM> detailVMs = new List<TransferMPLIssueDetailVM>();
                detailVMs = _repo.SearchTransIssueMPLDetailList(id);
                vm.TransferMPLIssueDetailVMs = detailVMs;

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


        [HttpPost]
        [Authorize]
        public ActionResult TransferMPLIssuePost(TransferMPLIssueVM vm)
        {
            try
            {
                if (vm.IDs == null)
                {
                    return Json("Already Posted!", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    _repo = new TransferIssueMPLRepo(identity, Session);
                    string[] result = new string[6];

                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.LastModifiedBy = identity.Name;

                    result = _repo.TransferMPLIssuePost(vm, null, null);
                    return Json(result[1], JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(TransferMPLIssueDetailVM vm)
        {
            #region Calculations

            //vm.SubTotal = vm.NBRPrice * vm.Quantity;
            //vm.SDAmount = (vm.SubTotal * vm.SDAmount) / 100;

            //vm.VATAmount = ((vm.SubTotal + vm.SDAmount) * vm.VATRate) / 100;
            //vm.TotalAmount = vm.SubTotal + vm.VATAmount + vm.SDAmount;

            //vm.UOMQty = vm.UOMc * vm.Quantity;
            //vm.UOMPrice = vm.UOMc * vm.NBRPrice;

            //vm.CurrencyValue = vm.CurrencyRateFromBDT * vm.SubTotal;
            //vm.DollerValue = vm.CurrencyValue / vm.CurrencyRateFromBDT;
            //vm.TransactionType = "Other";
            //vm.SaleType = "New";
            #endregion

            return PartialView("_detail", vm);
        }


    }
}
