using CrystalDecisions.Shared;
using SymOrdinary;
using SymphonySofttech.Reports;
using SymphonySofttech.Utilities;
using SymRepository.VMS;
using SymVATWebUI.Areas.VMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymVATWebUI.Filters;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace SymVATWebUI.Areas.VMS.Controllers
{
    [ShampanAuthorize]
    public class TransferIssueController : Controller
    {
        //
        // GET: /VMS/TransferIssue/
        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //TransferIssueRepo _repo = new TransferIssueRepo();
        ShampanIdentity identity = null;

        TransferIssueRepo _repo = null;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public TransferIssueController()
        {
            try
            {

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new TransferIssueRepo(identity);
            }
            catch
            {

            }

        }
        [Authorize(Roles = "Admin")]
        public ActionResult HomeIndex()
        {
            return View();
        }


        public ActionResult Index(TransferIssueVM paramVM)
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
                paramVM.TransactionType = "61Out";
            }

            paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());

            paramVM.IssueDateFrom = DateTime.Today.ToString("dd-MMM-yyyy"); //"1/1/1900";
            paramVM.IssueDateTo = DateTime.Today.ToString("dd-MMM-yyyy"); //"1/1/2050";

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

        public ActionResult _index(JQueryDataTableParamVM param, TransferIssueVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferIssueRepo(identity, Session);
            List<TransferIssueVM> getAllData = new List<TransferIssueVM>();
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
            string dtFrom = null;
            string dtTo = null;

            if (string.IsNullOrWhiteSpace(paramVM.SearchField))
            {
                dtFrom = DateTime.Now.ToString("yyyyMMdd");
                dtTo = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
                if (!string.IsNullOrWhiteSpace(paramVM.IssueDateFrom))
                {
                    dtFrom = paramVM.IssueDateFrom;
                }
                if (!string.IsNullOrWhiteSpace(paramVM.IssueDateTo))
                {
                    dtTo = paramVM.IssueDateTo + " 23:59:59";
                }
            }


            if (string.IsNullOrEmpty(paramVM.SearchField))
            {
                paramVM.SearchValue = "";
            }

            paramVM.SearchField = "ti." + paramVM.SearchField + " like";

            if (paramVM.BranchId == 0)
            {
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            }

            if (paramVM.BranchId == -1)
            {
                paramVM.BranchId = 0;
            }

            #endregion SeachParameters


            if (!identity.IsAdmin)
            {
                string[] conditionFields = { "ti.TransactionDateTime>=", "ti.TransactionDateTime<=", "ti.TransactionType", "ti.TransferIssueNo like", "ti.Post", "ti.BranchId", paramVM.SearchField };
                string[] conditionValues = { dtFrom, dtTo, paramVM.TransactionType, paramVM.TransferIssueNo, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SearchValue };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            else
            {
                string[] conditionFields = { "ti.TransactionDateTime>=", "ti.TransactionDateTime<=", "ti.TransactionType", "ti.TransferIssueNo like", "ti.Post", "ti.BranchId", paramVM.SearchField };
                string[] conditionValues = { dtFrom, dtTo, paramVM.TransactionType, paramVM.TransferIssueNo, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SearchValue };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            #endregion
            #region Search and Filter Data
            IEnumerable<TransferIssueVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Id
                //TransferIssueNo
                //TransactionDateTime
                //TotalAmount
                //TransferTo
                //SerialNo
                //Post

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.TransferIssueNo.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.TransactionDateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.TotalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.TransferBranchTo.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.SerialNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.ImportIDExcel.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<TransferIssueVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.TransferIssueNo :
                                                           sortColumnIndex == 2 && isSortable_2 ? Ordinary.DateToString(c.TransactionDateTime) :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.TotalAmount.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.TransferBranchTo.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.SerialNo.ToString() :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Post.ToString() :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.ImportIDExcel.ToString() :
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
                , c.TransferIssueNo
                , c.TransactionDateTime
                , c.TotalAmount.ToString()
                , c.TransferBranchTo.ToString()             
                , c.SerialNo.ToString()               
                , c.Post=="Y" ? "Posted" : "Not Posted"
                ,c.ImportIDExcel
                ,c.TransactionType
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

        [HttpGet]
        public ActionResult Create(string tType)
        {
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
            TransferIssueVM vm = new TransferIssueVM();
            if (tType == "61Out")
            {
                vm.ProductType = "Raw";
            }
            else
            {
                vm.ProductType = "Finish";
            }
            List<TransferIssueDetailVM> IssueDetailVMs = new List<TransferIssueDetailVM>();
            vm.Details = IssueDetailVMs;
            vm.Operation = "add";
            vm.VatName = "VAT 4.3";
            vm.ShiftId = 1;
            vm.TransactionType = tType;
            vm.TransactionDateTime = Session["SessionDate"].ToString();
            vm.FormNumeric = FormNumeric;
            return View(vm);
        }


        public JsonResult SelectProductDetails(string productCode, string IssueDate, string vatname, string tType)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var _repo = new ProductRepo(identity, Session);
            string[] conditionalFields = new string[] { "Pr.ItemNo" };
            //string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);

            CommonRepo commonRepo = new CommonRepo(identity, Session);
            string FormNumeric = commonRepo.settings("DecimalPlace", "FormNumeric");

            var product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
            if (product == null)
            {
                conditionalFields = new string[] { "Pr.ProductCode" };
                product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
            }

            #region businessLogic

            string UserId = identity.UserId;

            var issueDatetime = DateTime.Parse(IssueDate).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            DataTable priceData = _repo.AvgPriceNew(product.ItemNo, issueDatetime, null, null, true, true, true, true, UserId);
            decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
            decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

            product.Stock = quan;

            if (tType == "62Out")
            {
                var issueDate = DateTime.Parse(IssueDate).ToString("yyyy-MMM-dd");

                DataTable dt = _repo.GetBOMReferenceNo(product.ItemNo, vatname, issueDate);

                int BOMId = 0;
                decimal NBRPrice = 0;

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region BOMId and NBRPrice

                    DataRow dr = dt.Rows[0];
                    string tempBOMId = dr["BOMId"].ToString();
                    string tempNBRPrice = dr["NBRPrice"].ToString();
                    if (!string.IsNullOrWhiteSpace(tempBOMId))
                    {
                        BOMId = Convert.ToInt32(tempBOMId);
                    }

                    if (!string.IsNullOrWhiteSpace(tempNBRPrice))
                    {
                        NBRPrice = Convert.ToDecimal(tempNBRPrice);
                    }
                    #endregion

                }
                #region Value Assign

                product.BOMId = BOMId;
                product.CostPrice = NBRPrice;

                #endregion
            }
            else if (tType == "61Out")
            {
                DataTable dt = _repo.SelectBOMRaw(product.ItemNo, issueDatetime);
                if (quan > 0)
                {
                    product.CostPrice = (amount / quan);
                }
                else
                {
                    product.CostPrice = 0;
                }

                #region BOM

                int BOMId = 0;

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    string tempBOMId = dr["BOMId"].ToString();
                    if (!string.IsNullOrWhiteSpace(tempBOMId))
                    {
                        BOMId = Convert.ToInt32(tempBOMId);
                    }
                }

                //txtBOMId.Text = BOMId.ToString();
                product.BOMId = BOMId;

                #endregion
            }

            product.CostPrice = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(product.CostPrice.ToString(), FormNumeric));

            #endregion businessLogic

            return Json(product, JsonRequestBehavior.AllowGet);

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

                var html = "";

                if (uoms == null || uoms.Count == 0)
                {
                    string uomF = OrdinaryVATDesktop.StringReplacingForHTML(uomFrom);

                    html += "<option value='" + uomF + "'>" + uomF + "</option>";
                }
                else
                {
                    foreach (var item in uoms)
                    {
                        html += "<option value=" + item.UOMTo + ">" + item.UOMTo + "</option>";
                    }
                }

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

        public JsonResult GetConvFactor(string uomFrom, string UomTo)
        {
            var _repo = new UOMRepo(identity, Session);
            string[] conditionalFields = new string[] { "UOMFrom", "UOMTo" };
            string[] conditionalValues = new string[] { uomFrom, UomTo };
            var uom = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();
            var uomFactor = uom.Convertion;
            return Json(uomFactor, JsonRequestBehavior.AllowGet);
        }


        public ActionResult BlankItem(TransferIssueDetailVM vm)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {

            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }

            CommonRepo commonRepo = new CommonRepo(identity, Session);
            string FormNumeric = commonRepo.settings("DecimalPlace", "FormNumeric");

            vm.CostPrice = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(vm.CostPrice.ToString(), FormNumeric));
            vm.SubTotal = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(vm.SubTotal.ToString(), FormNumeric));
            vm.UOMQty = Convert.ToDecimal(OrdinaryVATDesktop.FormatingNumeric(vm.UOMQty.ToString(), FormNumeric));

            return PartialView("_detail", vm);
        }

        [HttpGet]
        public ActionResult Edit(string id, string TransactionType)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferIssueRepo(identity, Session);
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

            string[] conditionFields = new string[] { "TransactionType" };
            string[] conditionValues = new string[] { TransactionType };


            TransferIssueVM vm = new TransferIssueVM();
            vm = _repo.SelectAll(Convert.ToInt32(id), conditionFields, conditionValues).FirstOrDefault();

            if (vm == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<TransferIssueDetailVM> IssueDetailVMs = new List<TransferIssueDetailVM>();

            IssueDetailVMs = _repo.SelectDetail(vm.TransferIssueNo);

            vm.Details = IssueDetailVMs;
            vm.Operation = "update";
            if (vm.TransactionType == "61Out")
            {
                vm.ProductType = "Raw";
            }
            else
            {
                vm.ProductType = "Finish";
            }
            vm.VatName = "VAT 4.3";
            vm.FormNumeric = FormNumeric;

            return View("Create", vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(TransferIssueVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferIssueRepo(identity, Session);
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
            string[] result = new string[6];
            try
            {
                #region Setting default values
                if (string.IsNullOrWhiteSpace(vm.Comments))
                {
                    vm.Comments = "-";
                }
                if (string.IsNullOrWhiteSpace(vm.SerialNo))
                {
                    vm.SerialNo = "-";
                }
                if (string.IsNullOrWhiteSpace(vm.ReferenceNo))
                {
                    vm.ReferenceNo = "-";
                }
                #endregion
                #region SignatoryName & SignatoryDesig
                UserInformationRepo _UserInformationRepo = new UserInformationRepo(identity, Session);
                UserInformationVM varUserInformationVM = new UserInformationVM();
                varUserInformationVM = _UserInformationRepo.SelectAll(Convert.ToInt32(identity.UserId)).FirstOrDefault();


                vm.SignatoryName = varUserInformationVM.FullName;
                vm.SignatoryDesig = varUserInformationVM.Designation;
                #endregion

                //if (OrdinaryVATDesktop.IsACICompany(Convert.ToString(Session["CompanyCode"])))
                //{
                //    vm.TransactionDateTime = Convert.ToDateTime(vm.TransactionDateTime).ToString("yyyy-MM-dd") +
                //                             DateTime.Now.ToString(" HH:mm:ss");
                //}

                vm.TransactionDateTime = Convert.ToDateTime(vm.TransactionDateTime).ToString("yyyy-MM-dd") +
                                            DateTime.Now.ToString(" HH:mm:ss");

                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.CreatedBy = identity.Name;
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.Post = "N";
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                    #region Adding Line No
                    int i = 1;
                    foreach (var item in vm.Details)
                    {
                        item.IssueLineNo = i.ToString();
                        item.Comments = vm.Comments;
                        item.CreatedBy = vm.CreatedBy;
                        item.CreatedOn = vm.CreatedOn;
                        item.LastModifiedBy = vm.LastModifiedBy;
                        item.LastModifiedOn = vm.LastModifiedOn;
                        item.TransactionDateTime = vm.TransactionDateTime;
                        item.TransactionType = vm.TransactionType;
                        item.TransferIssueNo = vm.TransferIssueNo;
                        item.TransferTo = vm.TransferTo;
                        item.BranchId = Convert.ToInt32(Session["BranchId"]);
                        i++;
                    }
                    #endregion
                    result = _repo.Insert(vm, vm.Details);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[4], TransactionType = vm.TransactionType });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    if (vm.Post == "Y")
                    {
                        Session["result"] = "Fail" + "~" + "Transaction Was Posted";
                        return RedirectToAction("Edit", new { id = vm.Id });
                    }


                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                    #region Adding Line No
                    int i = 1;
                    foreach (var item in vm.Details)
                    {
                        item.IssueLineNo = i.ToString();
                        item.Comments = vm.Comments;
                        item.CreatedBy = vm.CreatedBy;
                        item.CreatedOn = vm.CreatedOn;
                        item.LastModifiedBy = vm.LastModifiedBy;
                        item.LastModifiedOn = vm.LastModifiedOn;
                        item.TransactionDateTime = vm.TransactionDateTime;
                        item.TransactionType = vm.TransactionType;
                        item.TransferIssueNo = vm.TransferIssueNo;
                        item.TransferTo = vm.TransferTo;
                        item.BranchId = Convert.ToInt32(Session["BranchId"]);

                        i++;
                    }
                    #endregion




                    result = _repo.Update(vm, vm.Details);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.TransactionType });
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
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                // Session["result"] = "Fail~Data not Successfully";
                return View("Create", vm);
            }
        }

        public ActionResult Navigate(string id, string btn, string ttype)
        {
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetIdForTtype("TransferIssues", "Id", id, btn, ttype);
            return RedirectToAction("Edit", new { id = targetId, TransactionType = ttype });
        }

        public ActionResult GetItemPopUp(string targetId, string ttype = "")
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

            if (ttype == "62In")
                ttype = "62Out";
            else if (ttype == "61In")
                ttype = "61Out";

            vm.TargetId = targetId;
            vm.TransactionType = ttype;
            return PartialView("_TIssuePopUp", vm);
        }
        public ActionResult GetIssueNoPopUp(PopUpViewModel vm)
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
            return PartialView("_transferIsuuePopUP", vm);
        }


        [HttpGet]
        public ActionResult GetFilteredItems(TransferVM vm)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferIssueRepo(identity, Session);
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


            Validation(vm);


            vm.Post = "Y";
            vm.BranchId = Convert.ToInt32(Session["BranchId"]);
            vm.VehicleNo = "";

            DataTable ReceiveResult = _repo.SearchTransfer(vm);  // Change 04
            List<TransferVM> list = new List<TransferVM>(); ;

            foreach (DataRow row in ReceiveResult.Rows)
            {
                TransferVM item = new TransferVM
                {
                    TransferFromNo = row["TransferFromNo"].ToString(),
                    TransactionDateTime = row["TransferDateTime"].ToString(),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"].ToString()),
                    SerialNo = row["SerialNo"].ToString(),
                    ReferenceNo = row["ReferenceNo"].ToString(),
                    TransferFromName = row["BranchName"].ToString(),
                    TransactionType = row["TransactionType"].ToString(),
                    Post = row["Post"].ToString()
                };

                list.Add(item);
            }

            return PartialView("_filteredTransfers", list);
        }

        private void Validation(TransferVM vm)
        {
            if (string.IsNullOrEmpty(vm.TransferFromNo))
            {
                vm.TransferFromNo = "";
            }

            if (string.IsNullOrEmpty(vm.DateTimeFrom))
            {
                vm.DateTimeFrom = "1/1/1900";
            }

            if (string.IsNullOrEmpty(vm.DateTimeTo))
            {
                vm.DateTimeTo = "1/1/2030";
            }

            if (string.IsNullOrEmpty(vm.TransferNo))
            {
                vm.TransferNo = "";
            }


            if (vm.SearchField == "TransferFromNo")
            {
                vm.TransferFromNo = vm.SearchValue;
            }

            if (vm.SearchField == "SerialNo")
            {
                vm.SerialNo = vm.SearchValue;
            }
            if (vm.SearchField == "ReferenceNo")
            {
                vm.ReferenceNo = vm.SearchValue;
            }
        }

        public ActionResult GetFilteredIssueNo(TransferVM vm)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferIssueRepo(identity, Session);
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
            //if (string.IsNullOrWhiteSpace(vm.TransactionType))
            //{
            //    vm.TransactionType = "61Out";
            //}
            string[] conditionalFields;
            string[] conditionalValues;

            conditionalFields = new string[] { "TransferIssueNo like", "Post", "TransactionType", "ti.BranchId" };
            conditionalValues = new string[] { vm.SearchValue, vm.Post, vm.TransactionType, vm.BranchId.ToString() };
            var list = _repo.SelectAll(0, conditionalFields, conditionalValues);
            return PartialView("_filteredIssueNo", list);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReportTransferIssueMis(ReportCommonVM vm)
        {
            try
            {

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                settingVM.SettingsDTUser = null;
                StaticValueReAssign(identity);

                connVM = Ordinary.StaticValueReAssign(identity, Session);

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new TransferIssueRepo(identity, Session);
                var reports = new MISReport();

                OrdinaryVATDesktop.FontSize = vm.FontSize.ToString();
                if (string.IsNullOrWhiteSpace(vm.IssueNo))
                {
                    vm.IssueNo = "";
                }

                string DateFrom = "";
                string DateTo = "";

                if (!string.IsNullOrWhiteSpace(vm.StartDate))
                {
                    DateFrom = Convert.ToDateTime(vm.StartDate).ToString("yyyy-MMM-dd");
                }

                if (!string.IsNullOrWhiteSpace(vm.EndDate))
                {
                    DateTo = Convert.ToDateTime(vm.EndDate).ToString("yyyy-MMM-dd");
                }

                vm.TransactionType = vm.TransactionType.Replace(".", "");
                var reportDocument = reports.TransferIssueOutReport(vm.IssueNo, DateFrom, DateTo, vm.TransactionType, vm.Branch, vm.TransferBranch, "0", connVM);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItems(string transferNo)
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
            List<TransferIssueDetailVM> vms = _repo.SelectDetail(transferNo);



            return PartialView("_detailMultiple", vms);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Getinformation(string transferNo)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferIssueRepo(identity, Session);
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

            conditionalFields = new string[] { "TransferIssueNo like" };
            conditionalValues = new string[] { transferNo };
            var list = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();


            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string TransferIssueNo)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferIssueRepo(identity, Session);
            TransferIssueVM vm = new TransferIssueVM();
            vm.TransferIssueNo = TransferIssueNo;
            #region SignatoryName & SignatoryDesig
            UserInformationRepo _UserInformationRepo = new UserInformationRepo(identity, Session);
            UserInformationVM varUserInformationVM = new UserInformationVM();
            varUserInformationVM = _UserInformationRepo.SelectAll(Convert.ToInt32(identity.UserId)).FirstOrDefault();


            vm.SignatoryName = varUserInformationVM.FullName;
            vm.SignatoryDesig = varUserInformationVM.Designation;
            #endregion

            string[] result = new string[6];
            result = _repo.Post(vm);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult MultiplePost(TransferIssueVM paramVM)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferIssueRepo(identity, Session);
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
            catch (Exception)
            {


            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult ExportExcel(TransferIssueVM paramVM)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferIssueRepo(identity, Session);
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


                if (paramVM.BranchId == 0)
                {
                    paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }
                if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
                {
                    paramVM.SelectTop = "All";
                }


                paramVM.CurrentUser = identity.UserId;

                if (paramVM.ExportAll)
                {
                    string[] conditionFields = new string[] { "ti.TransactionDateTime>=", "ti.TransactionDateTime<=", "ti.TransactionType", "ti.BranchId" };
                    string[] conditionValues = new string[] { dtFrom, dtTo, paramVM.TransactionType, paramVM.BranchId.ToString() };

                    // TransferIssueRepo repo = new TransferIssueRepo();

                    var list = _repo.SelectAll(0, conditionFields, conditionValues);

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

                // string[] ids = paramVM.IDs.ToArray();

                DataTable dt = _repo.GetExcelDataWeb(paramVM.IDs);

                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                var vm = OrdinaryVATDesktop.DownloadExcel(dt, "TransferIssue", "TransferIssueM");
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

            // return Json(rVM, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetProductQuantity(string tripNo)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferIssueRepo(identity, Session);
            CommonDAL commonDal = new CommonDAL();

            String saleFromProduction = commonDal.settingsDesktop("Sale", "SaleFromProduction");
            try
            {

                if (saleFromProduction.ToLower() == "y")
                {
                    ReceiveDAL _sDal = new ReceiveDAL();
                    DataTable dt = new DataTable();
                    dt = _sDal.SearchByReferenceNo(tripNo, "");

                    if (dt.Rows.Count <= 0)
                    {
                        return Json(new { productName = "", code = 0, quantity = 0, message = "Trip Not Found" }, JsonRequestBehavior.AllowGet);

                    }

                    string IsTripComplete = dt.Rows[0]["IsTripComplete"].ToString();

                    if (IsTripComplete.ToLower() == "y")
                    {
                        return Json(new { productName = "", code = 0, quantity = 0, message = "This Ref/Trip # Already Used." }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new
                    {
                        productName = dt.Rows[0]["ProductName"].ToString(),
                        code = dt.Rows[0]["ProductCode"].ToString(),
                        quantity = dt.Rows[0]["Quantity"].ToString()
                    }, JsonRequestBehavior.AllowGet);


                }

                return Json(new { productName = "", code = 0, quantity = 0 }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                return Json(new { productName = "", code = 0, quantity = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet, Authorize]
        public ActionResult SelectTransfer(TransferIssueVM paramVM)
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new TransferIssueRepo(identity, Session);
                TransferIssueVM vm = new TransferIssueVM();
                string[] cFields = { "ti.ReferenceNo", "ti.TransferIssueNo" };
                string[] cValues = { paramVM.ReferenceNo, paramVM.TransferIssueNo };


                vm = _repo.SelectAll(0, cFields, cValues).FirstOrDefault();

                Session["JSONTransfer"] = null;

                return RedirectToAction("Edit", new { id = vm.Id });
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
        }


        public ActionResult GetTransfer(TransferIssueVM paramVM)
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new TransferIssueRepo(identity, Session);
                TransferIssueVM vm = new TransferIssueVM();
                string[] cFields = { "ti.ReferenceNo", "ti.TransferIssueNo" };
                string[] cValues = { paramVM.ReferenceNo, paramVM.TransferIssueNo };


                vm = _repo.SelectAll(0, cFields, cValues, null, null).FirstOrDefault();


                return Json(vm, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
        }



        [Authorize(Roles = "Admin")]
        public JsonResult getVehicleDetails(string VehicleNo)
        {
            var repo = new VehicleRepo(identity, Session);
            var id = 0;
            string[] conditionalFields;
            string[] conditionalValues;

            try
            {
                conditionalFields = new string[] { "VehicleNo" };
                conditionalValues = new string[] { VehicleNo };

                //id = Convert.ToInt32(vehicleId);
            }
            catch (Exception)
            {
                throw;
            }
            //var vehicle = repo.SelectAll(id).FirstOrDefault();
            var vehicle = repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();
            return Json(vehicle, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportExcel(TransferIssueVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferIssueRepo(identity, Session);
            string[] result = new string[6];
            try
            {
                paramVM.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                paramVM.CreatedBy = identity.Name;
                paramVM.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                paramVM.LastModifiedBy = identity.Name;
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"]);
                paramVM.BranchCode = Session["BranchCode"].ToString();
                paramVM.CurrentUser = identity.UserId;
                result = _repo.ImportExcelFile(paramVM);
                Session["result"] = result[0] + "~" + result[1];
                //return View("Index", vm);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message.Replace("\r", "").Replace("\n", "");
                SymOrdinary.FileLogger.Log("Purchase", "ImportExcel", ex.Message + "\n" + ex.StackTrace + "\n");
                // FileLogger.Log("Sale", "SaveSale", ex.Message + "\n" + ex.StackTrace + "\n");
                //return View("Index", vm);

                return RedirectToAction("Index");
            }
        }

        private void StaticValueReAssign(ShampanIdentity identity)
        {
            try
            {
                #region Get Company Information
                CompanyProfileRepo _CompanyProfileRepo = new CompanyProfileRepo(identity);
                CompanyProfileVM varCompanyProfileVM = new CompanyProfileVM();
                string CompanyId = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, identity.CompanyId);
                varCompanyProfileVM = _CompanyProfileRepo.SelectAll(CompanyId).FirstOrDefault();
                OrdinaryVATDesktop.CompanyName = varCompanyProfileVM.CompanyLegalName;
                OrdinaryVATDesktop.Address1 = varCompanyProfileVM.Address1;
                OrdinaryVATDesktop.Address2 = varCompanyProfileVM.Address2;
                OrdinaryVATDesktop.Address3 = varCompanyProfileVM.Address3;
                OrdinaryVATDesktop.TelephoneNo = varCompanyProfileVM.TelephoneNo;
                OrdinaryVATDesktop.FaxNo = varCompanyProfileVM.FaxNo;
                OrdinaryVATDesktop.VatRegistrationNo = varCompanyProfileVM.VatRegistrationNo;
                OrdinaryVATDesktop.Section = varCompanyProfileVM.Section;
                #endregion

                #region Get Branch Information
                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity);
                BranchProfileVM varBranchProfileVM = new BranchProfileVM();
                varBranchProfileVM = branchProfileRepo.SelectAll(Convert.ToString(Session["BranchId"])).FirstOrDefault();
                OrdinaryVATDesktop.IsWCF = varBranchProfileVM.IsWCF;
                OrdinaryVATDesktop.BranchId = varBranchProfileVM.BranchID;

                #endregion
                OrdinaryVATDesktop.CurrentUser = Convert.ToString(Session["LogInUserName"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }



    }
}
