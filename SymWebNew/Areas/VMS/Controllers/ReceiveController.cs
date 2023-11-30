using CrystalDecisions.CrystalReports.Engine;
//using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.VMS;
//using SymRepository.Common;
using VATViewModel.DTOs;
//using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading.Tasks;
using System.Net.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Configuration;
using SymVATWebUI.Filters;
using VATServer.Ordinary;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class ReceiveController : Controller
    {

        ReceiveRepo _repo = null;
        ShampanIdentity identity = null;

        public ReceiveController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new ReceiveRepo(identity);
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
        public ActionResult Index(ReceiveMasterVM paramVM)
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


            if (string.IsNullOrWhiteSpace(paramVM.transactionType))
            {
                paramVM.transactionType = "Other";
            }

            //Session["dtFrom"] = dtFrom;
            //Session["dtTo"] = dtTo;

            //ReceiveMasterVM vm = new ReceiveMasterVM();
            //vm.TransactionType = tType;
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
        public ActionResult _index(JQueryDataTableParamVM param, ReceiveMasterVM paramVM)
        {
            List<ReceiveMasterVM> getAllData = new List<ReceiveMasterVM>();
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
            #region SeachParameters
            ///string searchedBranchId = "0";
            string dtFrom = "";
            string dtTo = "";
            //if (!string.IsNullOrWhiteSpace(Session["Branch"] as string))
            //{
            //    searchedBranchId = Session["Branch"].ToString();
            //}
            if (string.IsNullOrWhiteSpace(paramVM.SearchField))
            {
                dtFrom = DateTime.Now.ToString("yyyyMMdd");
                dtTo = DateTime.Now.AddDays(1).ToString("yyyyMMdd");

                if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTimeFrom))
                {
                    dtFrom = paramVM.IssueDateTimeFrom;
                }
                if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTimeTo))
                {
                    dtTo = paramVM.IssueDateTimeTo;
                }
            }
            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "All";
            }


            if (paramVM.BranchId == 0)
            {
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            }

            if (paramVM.BranchId == -1)
            {
                paramVM.BranchId = 0;
            }

            #region Unuse

            //string BranchId = "";
            //if (searchedBranchId == "-1")
            //{
            //    BranchId = "";
            //}
            //else if (BranchId != searchedBranchId && searchedBranchId != "0")
            //{
            //    BranchId = searchedBranchId;
            //}
            //else
            //{
            //    BranchId = identity.BranchId.ToString();
            //}

            #endregion

            #endregion SeachParameters

            _repo = new ReceiveRepo(identity, Session);
            if (!identity.IsAdmin)
            {
                if (string.IsNullOrWhiteSpace(paramVM.SearchField))
                {
                    ////////string[] conditionFields = { "rh.ReceiveDateTime>=", "rh.ReceiveDateTime<=", "rh.TransactionType", "rh.ReceiveNo like", "rh.CustomerID", "rh.Post", "SelectTop" };
                    ////////string[] conditionValues = { dtFrom, dtTo, paramVM.transactionType, paramVM.ReceiveNo, paramVM.CustomerID, paramVM.Post, paramVM.SelectTop };

                    string[] conditionFields = { "rh.ReceiveDateTime>=", "rh.ReceiveDateTime<=", "rh.TransactionType", "rh.CustomerID", "rh.Post", "rh.BranchId", "SelectTop" };
                    string[] conditionValues = { dtFrom, dtTo, paramVM.transactionType, paramVM.CustomerID, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SelectTop };

                    getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
                }
                else
                {

                    string[] conditionFields = { "rh.ReceiveDateTime>=", "rh.ReceiveDateTime<=", "rh.TransactionType", paramVM.SearchField, "rh.CustomerID", "rh.Post", "rh.BranchId", "SelectTop" };
                    string[] conditionValues = { dtFrom, dtTo, paramVM.transactionType, paramVM.SearchValue, paramVM.CustomerID, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SelectTop };

                    getAllData = _repo.SelectAll(0, conditionFields, conditionValues);

                }
            }
            else
            {
                ////string[] conditionFields = { "rh.ReceiveDateTime>=", "rh.ReceiveDateTime<=", "rh.TransactionType", "rh.ReceiveNo like", "rh.CustomerID", "rh.Post", "SelectTop" };
                ////string[] conditionValues = { dtFrom, dtTo, paramVM.transactionType, paramVM.ReceiveNo, paramVM.CustomerID, paramVM.Post, paramVM.SelectTop };

                if (string.IsNullOrWhiteSpace(paramVM.SearchField))
                {

                    string[] conditionFields = { "rh.ReceiveDateTime>=", "rh.ReceiveDateTime<=", "rh.TransactionType", "rh.CustomerID", "rh.Post", "rh.BranchId", "SelectTop" };
                    string[] conditionValues = { dtFrom, dtTo, paramVM.transactionType, paramVM.CustomerID, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SelectTop };

                    getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
                }
                else
                {


                    if (paramVM.SearchField == "ImportID")
                    {
                        paramVM.SearchField = "rh.ImportIDExcel like";
                    }
                    else
                    {
                        paramVM.SearchField = "rh." + paramVM.SearchField + " like";
                    }

                    //paramVM.SearchField = "rh." + paramVM.SearchField + " like";

                    string[] conditionFields = { "rh.ReceiveDateTime>=", "rh.ReceiveDateTime<=", "rh.TransactionType", paramVM.SearchField, "rh.CustomerID", "rh.Post", "rh.BranchId", "SelectTop" };
                    string[] conditionValues = { dtFrom, dtTo, paramVM.transactionType, paramVM.SearchValue, paramVM.CustomerID, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SelectTop };

                    getAllData = _repo.SelectAll(0, conditionFields, conditionValues);

                }
            }
            #endregion
            #region Search and Filter Data
            IEnumerable<ReceiveMasterVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Id
                //ReceiveNo
                //ReceiveDateTime
                //TotalAmount
                //TransactionType
                //SerialNo
                //Post

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.ReceiveNo.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.ReceiveDateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.TotalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.transactionType.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.SerialNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ReceiveMasterVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.ReceiveNo :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.ReceiveDateTime.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.TotalAmount.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.transactionType :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.SerialNo.ToString() :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Post.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id+"~"+ c.Post
                , c.ReceiveNo
                , c.ReceiveDateTime.ToString()
                , c.TotalAmount.ToString()
                , c.transactionType            
                , c.SerialNo.ToString()               
                , c.Post=="Y" ? "Posted" : "Not Posted"
                ,c.ImportId
                ,c.transactionType
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
        public ActionResult BlankItem(ReceiveDetailVM vm)
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
            return PartialView("_detail", vm);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create(string tType)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            CommonRepo cRepo = new CommonRepo(identity, Session);
            string FormNumeric = cRepo.settings("DecimalPlace", "FormNumeric");
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
            ReceiveMasterVM vm = new ReceiveMasterVM();

            List<ReceiveDetailVM> ReceiveDetailVMs = new List<ReceiveDetailVM>();
            vm.Details = ReceiveDetailVMs;
            vm.Operation = "add";
            vm.transactionType = tType;
            vm.ShiftId = "1";

            //CommonRepo cRepo = new CommonRepo(identity, Session);

            string tracking = cRepo.settingValue("TrackingTrace", "Tracking");
            vm.TrackingTrace = tracking == "Y";
            vm.FormNumeric = FormNumeric;
            vm.ReceiveDateTime = Session["SessionDate"].ToString();
            #region FormMaker
            FormMaker(vm);
            #endregion


            return View(vm);
        }

        private void FormMaker(ReceiveMasterVM vm)
        {
            vm.ProductType = "Finish";
            vm.VatName = "VAT 4.3";
            CommonRepo cRepo = new CommonRepo(identity, Session);
            string vIssueFromBOM = "";
            vIssueFromBOM = cRepo.settingValue("IssueFromBOM", "IssueFromBOM");

            switch (vm.transactionType)
            {
                case "Other": break;
                case "WIP": { vm.ProductType = "WIP"; } break;
                case "ReceiveReturn": { } break;
                case "PackageProduction": { vm.VatName = "VAT 1 (Package)"; } break;
                case "TollFinishReceive":
                    {
                        vm.ProductType = "Overhead";
                        vm.VatName = "VAT 1 (Toll Receive)";
                        vIssueFromBOM = cRepo.settingValue("IssueFromBOM", "TollFGReceive");
                    }
                    break;
                default: break;
            }
            vm.IssueFromBOM = vIssueFromBOM;

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(ReceiveMasterVM vm)
        {
            _repo = new ReceiveRepo(identity, Session);
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
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedBy = identity.Name;
                    vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                    vm.FromBOM = vm.IssueFromBOM;
                    vm.ReceiveDateTime = Convert.ToDateTime(vm.ReceiveDateTime).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    //////vm.TransactionType = "Other";
                    vm.Post = "N";
                    List<TrackingVM> Trackings = new List<TrackingVM>();

                    result = _repo.ReceiveInsert(vm, vm.Details, Trackings, null, null, identity.UserId);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[4], TransactionType = vm.transactionType });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                    vm.FromBOM = vm.IssueFromBOM;

                    List<TrackingVM> Trackings = new List<TrackingVM>();
                    result = _repo.ReceiveUpdate(vm, vm.Details, Trackings, identity.UserId);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.transactionType });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
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
                //////Session["result"] = "Fail~Data not Successfully";
                string[] Message = ex.Message.Split('\r');
                string msg = Message[0].ToString();

                FileLogger.Log("Purchase", "SavePurchase", ex.ToString());

                Session["result"] = "Fail~" + msg;
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        public JsonResult SelectProductDetails(string productCode, string IssueDate, string vatName)
        {
            try
            {
                //FileLogger.Log("Purchase", "SavePurchase", e.ToString());

                var _repo = new ProductRepo(identity, Session);
                CommonRepo commonRepo = new CommonRepo(identity, Session);

                string FormNumeric = commonRepo.settings("DecimalPlace", "FormNumeric");

                //string[] conditionalFields = new string[] { "Pr.ProductCode" };
                //string[] conditionalValues = new string[] { productCode };

                //////DataTable dt = _repo.SearchProductDT("", productName, "", "", "", "", "", "", "", "", "", "", "", "");
                //var product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
                var product = _repo.SelectAll(productCode).FirstOrDefault();

                //////var code = product.ProductCode;
                //////var itemNo = product.ItemNo;
                //////var uom = product.UOM;
                //////var hscode = product.HSCodeNo;
                //////var vatrate = product.VATRate.ToString();
                var issueDatetime = DateTime.Parse(IssueDate).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                //var sd = product.SD.ToString();
                //var name = product.ProductName;

                //var costPrice = product.NBRPrice.ToString();
                //var stock = "0";

                //var lastDeclaredPrice = _repo.GetLastNBRPriceFromBOM(product.ItemNo, vatName, issueDatetime, null, null).ToString();
                //var stockInHand = _repo.AvgPriceNew(product.ProductCode, issueDatetime, null, null, false).Rows[0]["Quantity"].ToString();

                //if (lastDeclaredPrice != null)
                //{
                //    product.CostPrice = Convert.ToDecimal(lastDeclaredPrice);
                //}

                OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);

                string UserId = identity.UserId;

                //var stockInHand = 0;//_repo.AvgPriceNew(productCode, issueDatetime, null, null, true, true, true, false, UserId).Rows[0]["Quantity"].ToString();
                var stockInHand = _repo.AvgPriceNew(productCode, issueDatetime, null, null, true, true, true, false, UserId).Rows[0]["Quantity"].ToString();


                //JBR
                if (stockInHand != null)
                {
                    product.Stock = Convert.ToDecimal(stockInHand);
                }


                if (product == null)
                {
                    return Json(product, JsonRequestBehavior.AllowGet);
                }



                DataTable dt = _repo.GetBOMReferenceNo(product.ItemNo, vatName, issueDatetime);
                int BOMId = 0;
                decimal NBRPrice = 0;
                DataRow dr = dt.Rows[0];
                string tempBOMId = dr["BOMId"].ToString();
                string tempNBRPrice = dr["NBRPrice"].ToString();
                if (!string.IsNullOrWhiteSpace(tempBOMId))
                {
                    BOMId = Convert.ToInt32(tempBOMId);
                    product.BOMId = BOMId;
                }

                if (!string.IsNullOrWhiteSpace(tempNBRPrice))
                {
                    NBRPrice = Convert.ToDecimal(tempNBRPrice);
                    //product.NBRPrice = Convert.ToDecimal(NBRPrice.ToString("0.0000"));
                    product.NBRPrice = Convert.ToDecimal(ParseDecimalObject(NBRPrice));


                }


                //string result = code + "~" + uom + "~" + hscode + "~" + costPrice + "~" + stock + "~" + name + "~" + vatrate + "~" + sd + "~" + itemNo;

                return Json(product, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                FileLogger.Log("Purchase", "SavePurchase", e.ToString());

                throw e;
            }
        }

        public JsonResult GetUomOption(string uomFrom)
        {
            try
            {


                var _repo = new UOMRepo(identity, Session);
                string[] conditionalFields = new string[] { "UOMFrom" };
                string[] conditionalValues = new string[] { uomFrom };
                var uoms = _repo.SelectAll(0, conditionalFields, conditionalValues);

                #region Old Code

                ////var html = "";
                ////if (uoms == null || uoms.Count == 0)
                ////{
                ////    string uomF = OrdinaryVATDesktop.StringReplacingForHTML(uomFrom);
                ////    ////uomF = uomF.Replace("'", "&#39;");

                ////    html += "<option value='" + uomF + "'>" + uomF + "</option>";
                ////    ////html += "<option value='" + uomFrom + "'>" + uomFrom + "</option>";

                ////}
                ////else
                ////{
                ////    foreach (var item in uoms)
                ////    {
                ////        html += "<option value='" + item.UOMTo + "'>" + item.UOMTo + "</option>";

                ////        ////html += "<option value='" + item.UOMTo + "'>" + item.UOMTo + "</option>";
                ////        //////html += "<option value=" + item.UOMTo + ">" + item.UOMTo + "</option>";
                ////    }
                ////}

                #endregion Old Code

                return Json(uoms, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                List<UOMConversionVM> VMs = new List<UOMConversionVM>();
                UOMConversionVM vm = new UOMConversionVM();
                vm.UOMFrom = uomFrom.ToString();
                vm.UOMTo = uomFrom.ToString();
                vm.Convertion = 1;
                VMs.Add(vm);

                return Json(VMs, JsonRequestBehavior.AllowGet);

            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id, string TransactionType)
        {
            _repo = new ReceiveRepo(identity, Session);
            CommonRepo commonRepo = new CommonRepo(identity, Session);
            string FormNumeric = commonRepo.settings("DecimalPlace", "FormNumeric");
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

            if (TransactionType == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ReceiveMasterVM vm = new ReceiveMasterVM();

            string[] conditionFields = new string[] { "rh.TransactionType" };
            string[] conditionValues = new string[] { TransactionType };

            vm = _repo.SelectAll(Convert.ToInt32(id), conditionFields, conditionValues).FirstOrDefault();

            if (vm == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<ReceiveDetailVM> ReceiveDetailVMs = new List<ReceiveDetailVM>();

            ReceiveDetailVMs = _repo.SelectReceiveDetail(vm.ReceiveNo);

            vm.Details = ReceiveDetailVMs;
            vm.Operation = "update";

            #region FormMaker
            FormMaker(vm);
            #endregion
            string tracking = commonRepo.settingValue("TrackingTrace", "Tracking");
            vm.TrackingTrace = tracking == "Y";
            vm.FormNumeric = FormNumeric;
            return View("Create", vm);
        }

        public ActionResult Navigate(string id, string btn, string ttype)
        {
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetIdForTtype("ReceiveHeaders", "Id", id, btn, ttype);
            return RedirectToAction("Edit", new { id = targetId, TransactionType = ttype });
        }
        [Authorize]
        [HttpGet]
        public ActionResult GetReceivePopUp(string targetId)
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
            PopUpViewModel vm = new PopUpViewModel();
            vm.TargetId = targetId;
            return PartialView("_Receive", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredReceive(ReceiveMasterVM vm)
        {
            _repo = new ReceiveRepo(identity, Session);
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
            string[] conditionalFields = new string[] { "rh.ReceiveDateTime>", "rh.ReceiveDateTime<", "rh.Post", "rh.CustomerID" };
            string[] conditionalValues = new string[] { vm.IssueDateTimeFrom, vm.IssueDateTimeTo, vm.Post, vm.CustomerID };
            var list = _repo.SelectAll(0, conditionalFields, conditionalValues, null, null, vm);

            return PartialView("_filteredReceive", list);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            try
            {

                _repo = new ReceiveRepo(identity, Session);
                string[] a = ids.Split('~');
                var id = a[0];
                ReceiveMasterVM vm = new ReceiveMasterVM();
                vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
                List<ReceiveDetailVM> ReceiveDetailVMS = new List<ReceiveDetailVM>();
                List<TrackingVM> TrackingVM = new List<TrackingVM>();
                ReceiveDetailVMS = _repo.SelectReceiveDetail(vm.ReceiveNo);
                TrackingVM = _repo.SelectTrakingsDetail(ReceiveDetailVMS, vm.ReceiveNo, null);
                vm.Details = ReceiveDetailVMS;
                vm.Trackings = TrackingVM;
                string[] result = new string[6];
                vm.LastModifiedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString();
                vm.Post = "Y";
                ////result = _repo.ReceivePost(vm, vm.Details, new List<TrackingVM>(), identity.UserId);
                result = _repo.ReceivePost(vm, vm.Details, vm.Trackings, identity.UserId);
                return Json(result[1], JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return Json("Fail~" + message, JsonRequestBehavior.AllowGet);

            }
        }

        [Authorize]
        public ActionResult MultiplePost(ReceiveMasterVM paramVM)
        {
            _repo = new ReceiveRepo(identity, Session);
            #region Access Control

            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            ResultVM rVM = new ResultVM();
            string[] result = new string[6];


            try
            {

                paramVM.CurrentUser = identity.UserId;

                if (paramVM != null && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    paramVM.IDs = paramVM.IDs.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

                    if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                    {
                        rVM.Message = "No Data to Post";
                        return Json(rVM, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    rVM.Message = "No Data to Post";
                    return Json(rVM, JsonRequestBehavior.AllowGet);
                }

                string[] ids = paramVM.IDs.ToArray();

                result = _repo.MultiplePost(ids);

                rVM.Status = result[0];
                rVM.Message = result[1];


            }
            catch (Exception ex)
            {

                rVM.Status = "Fail";
                rVM.Message = ex.Message;
            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult ExportExcel(ReceiveMasterVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new ReceiveRepo(identity, Session);
            #region Access Control

            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            ResultVM rVM = new ResultVM();
            string[] result = new string[6];


            try
            {
                string dtFrom = null;
                string dtTo = null;
                dtFrom = "2019-07-01 00:00:00";
                dtTo = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");

                if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTimeFrom))
                {
                    dtFrom = Convert.ToDateTime(paramVM.IssueDateTimeFrom).ToString("yyyyMMdd");
                }
                if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTimeTo))
                {

                    dtTo = Convert.ToDateTime(paramVM.IssueDateTimeTo).AddDays(1).ToString("yyyyMMdd");
                }


                if (paramVM.BranchId == 0)
                {
                    paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }

                if (paramVM.BranchId == -1)
                {
                    paramVM.BranchId = 0;
                }

                if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
                {
                    paramVM.SelectTop = "All";
                }


                paramVM.CurrentUser = identity.UserId;

                if (paramVM.ExportAll)
                {
                    string[] conditionFields = new string[] { "rh.ReceiveDateTime>=", "rh.ReceiveDateTime<=", "rh.BranchId", "rh.TransactionType", "SelectTop" };
                    string[] conditionValues = new string[] { dtFrom, dtTo, paramVM.BranchId.ToString(), paramVM.transactionType.ToString(), paramVM.SelectTop };

                    ReceiveRepo repo = new ReceiveRepo(identity, Session);

                    var list = repo.SelectAll(0, conditionFields, conditionValues);

                    paramVM.IDs = list.Select(x => x.Id).ToList();

                }



                paramVM.CurrentUser = identity.UserId;

                if (paramVM != null && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    paramVM.IDs = paramVM.IDs.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

                    if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                    {
                        rVM.Message = "No Data to Export";
                        return Json(rVM, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    rVM.Message = "No Data to Export";
                    return Json(rVM, JsonRequestBehavior.AllowGet);
                }

                //string[] ids = paramVM.IDs.ToArray();

                DataTable Dt = _repo.GetExcelDataWeb(paramVM.IDs);
                if (Dt.Rows.Count == 0)
                {
                    Dt.Rows.Add(Dt.NewRow());
                }
                // OrdinaryVATDesktop.SaveExcel(Dt, "Receive", "ReceiveM");
                var vm = OrdinaryVATDesktop.DownloadExcel(Dt, "Receive", "ReceiveM");
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + vm.FileName + ".xlsx");
                    vm.varExcelPackage.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                //rVM.Status = result[0];
                //rVM.Message = result[1];


            }
            catch (Exception)
            {


            }

            finally { }

            return RedirectToAction("Index");

            //// return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ImportExcel(ReceiveMasterVM vm)
        {
            _repo = new ReceiveRepo(identity, Session);
            string[] result = new string[6];
            try
            {
                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                result = _repo.ImportExcelFile(vm);
                Session["result"] = result[0] + "~" + result[1].ToLower() + " " + (result.Length >= 5 ? result[4] : "");

                return RedirectToAction("Index", new { vm.transactionType });
            }
            catch (Exception ex)
            {
                Session["result"] = result[0] + "~" + result[1] + "\n" + ex.Message;
                return View("Index", vm);
            }
        }

        public JsonResult GetConvFactor(string uomFrom, string UomTo)
        {
            decimal uomFactor = 0;

            try
            {
                var _repo = new UOMRepo(identity, Session);
                string[] conditionalFields = new string[] { "UOMFrom", "UOMTo" };
                string[] conditionalValues = new string[] { uomFrom, UomTo };
                var uom = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();

                if (uom != null)
                {
                    uomFactor = uom.Convertion;
                }

            }
            catch (Exception ex)
            {
                uomFactor = 0;

                FileLogger.Log("ReceiveController", "GetConvFactor", ex.ToString());

            }

            return Json(uomFactor, JsonRequestBehavior.AllowGet);

        }

        public string ParseDecimalObject(object numb)
        {

            String val = "0";
            string a = "str123";
            string numbField = numb.ToString();



            try
            {


                if (string.IsNullOrWhiteSpace(numbField.ToString()))
                {
                    numbField = "0";
                }
                else
                {
                    numbField = numbField.ToString().Replace(",", "");
                }
                CommonRepo commonRepo = new CommonRepo(identity, Session);

                string FormNumeric = commonRepo.settings("DecimalPlace", "FormNumeric");

                string Pre = "";
                Pre = Pre.PadRight(Convert.ToInt32(FormNumeric), '#');
                val = decimal.Parse(numbField.ToString(), System.Globalization.NumberStyles.Float).ToString("#,###0." + Pre);

            }
            catch { }
            return val;
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetTrakingsNo(string ProductCode, string cDate, string ReceiveNo, string tType)
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
            TrackingVM vm = new TrackingVM();
            List<TrackingVM> vms = new List<TrackingVM>();

            var repo = new ProductRepo(identity, Session);
            var _repo = new SaleInvoiceRepo(identity, Session);

            if (tType.ToLower() == "other")
            {
                tType = "Receive";

            }

            ////string[] conditionalFields = new string[] { "Pr.ProductCode" };
            ////string[] conditionalValues = new string[] { ProductCode };
            ////var product = repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();

            DataTable TrackingRawsDt = _repo.FindTrackingItems(ProductCode, "VAT 4.3", cDate);

            for (int i = 0; i < TrackingRawsDt.Rows.Count; i++)
            {
                string rawItemNo = TrackingRawsDt.Rows[i]["RawItemNo"].ToString();
                vms = _repo.SelectTrakingsDetail(rawItemNo, "N", ReceiveNo, tType);
            }

            //if(vms.Count<=0 || vms= null )
            //{
            //}


            return PartialView("_detailTrakings", vms);
        }

        [Authorize]
        [HttpPost]
        public ActionResult TrakingsUpdate(TrackingVM vm)
        {
            #region Access Control
            _repo = new ReceiveRepo(identity, Session);
            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            ResultVM rVM = new ResultVM();
            List<TrackingVM> vms = new List<TrackingVM>();

            try
            {
                string RInvoice = "";
                decimal qty = 0;
                decimal engQty = 0;
                string itmno = "";

                foreach (var item in vm.IDs)
                {
                    TrackingVM Trakingsvm = new TrackingVM();
                    if (item.transactionType.ToLower() == "other")
                    {
                        Trakingsvm.transactionType = "Receive";

                    }

                    Trakingsvm.ItemNo = item.ItemNo;
                    Trakingsvm.Heading1 = item.Heading1;
                    Trakingsvm.IsReceive = item.IsReceive;
                    Trakingsvm.ReceiveNo = item.ReceiveNo;
                    Trakingsvm.FinishItemNo = item.FinishItemNo;
                    Trakingsvm.ReceiveDate = item.ReceiveDate;

                    RInvoice = Trakingsvm.ReceiveNo;
                    itmno = Trakingsvm.FinishItemNo;

                    engQty += 1;

                    vms.Add(Trakingsvm);


                }

                var ReceiveDetailsvm = new ReceiveDetailVM();
                string[] conditionalFields = new string[] { "rd.ReceiveNo", "rd.ItemNo" };
                string[] conditionalValues = new string[] { RInvoice, itmno };
                ReceiveDetailsvm = _repo.SelectReceiveDetail(null, conditionalFields, conditionalValues).FirstOrDefault();
                qty = ReceiveDetailsvm.Quantity;

                if (engQty != qty)
                {

                    rVM.Message = "Fail" + "~" + "Please select " + qty + " iteams for finish products";

                    return Json(rVM, JsonRequestBehavior.AllowGet);

                }

                //vm.BranchCode = Session["BranchCode"].ToString();
                string result = _repo.TrackingUpdate(vms, null, null, null);
                rVM.Message = result;

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;
            }

            return Json(rVM, JsonRequestBehavior.AllowGet);

        }

    }
}
