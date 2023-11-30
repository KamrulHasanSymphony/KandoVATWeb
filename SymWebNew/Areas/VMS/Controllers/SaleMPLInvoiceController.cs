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
    public class SaleMPLInvoiceController : Controller
    {
        ShampanIdentity identity = null;

        SaleMPLInvoiceRepo _repo = null;

        public SaleMPLInvoiceController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new SaleMPLInvoiceRepo(identity);
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
        public ActionResult Index(SalesInvoiceMPLHeaderVM paramVM)
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
        public ActionResult _index(JQueryDataTableParamVM param, SalesInvoiceMPLHeaderVM paramVM)
        {
            _repo = new SaleMPLInvoiceRepo(identity, Session);

            List<SalesInvoiceMPLHeaderVM> getAllData = new List<SalesInvoiceMPLHeaderVM>();

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

                if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTime))
                {
                    dtFrom = Convert.ToDateTime(paramVM.InvoiceDateTime).ToString("yyyyMMdd");
                }
                if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTime))
                {
                    dtTo = Convert.ToDateTime(paramVM.InvoiceDateTime).AddDays(1).ToString("yyyyMMdd");
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

            conditionFields = new string[] { "sih.InvoiceDateTime>=", "sih.InvoiceDateTime<=", "sih.SaleType", "sih.CustomerID", "sih.IsPrint", "sih.Post", "sih.BranchId", "sih.ReportType" };
            conditionValues = new string[] { dtFrom, dtTo, paramVM.SaleType, paramVM.CustomerID, paramVM.IsPrint, paramVM.Post, paramVM.BranchId.ToString(), paramVM.ReportType };

            getAllData = _repo.SelectAll(0, conditionFields, conditionValues, null, null, null, paramVM.TransactionType, "Y");


            #endregion

            #region Search and Filter Data
            IEnumerable<SalesInvoiceMPLHeaderVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
               
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                var isSearchable9 = Convert.ToBoolean(Request["bSearchable_9"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.SalesInvoiceNo.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.DeliveryAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.InvoiceDateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.TotalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.TotalVATAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<SalesInvoiceMPLHeaderVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.SalesInvoiceNo :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.DeliveryAddress.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.InvoiceDateTime.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.TotalAmount.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.TotalVATAmount.ToString() :
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
                  c.Id+"~"+ c.Post+"~"+ c.BranchId
                , c.SalesInvoiceNo
                , c.DeliveryAddress.ToString()
                , c.InvoiceDateTime.ToString()             
                , c.TotalAmount.ToString()               
                , c.TotalVATAmount.ToString()               
                , c.Post=="Y" ? "Posted" : "Not Posted"
                , c.TransactionType
                , c.ReportType

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

            SalesInvoiceMPLHeaderVM vm = new SalesInvoiceMPLHeaderVM();
            List<SalesInvoiceMPLDetailVM> saleDetailVMs = new List<SalesInvoiceMPLDetailVM>();
            vm.SalesInvoiceMPLDetailVMs = saleDetailVMs;

            List<SalesInvoiceMPLCRInfoVM> crDetailVMs = new List<SalesInvoiceMPLCRInfoVM>();
            vm.SalesInvoiceMPLCRInfoVMs = crDetailVMs;
            List<SalesInvoiceMPLBankPaymentVM> bankDetailVMs = new List<SalesInvoiceMPLBankPaymentVM>();
            vm.SalesInvoiceMPLBankPaymentVMs = bankDetailVMs;
            vm.Operation = "add";
            vm.TransactionType = "Other";
            vm.InvoiceDateTime = Session["SessionDate"].ToString();
            vm.ConversionDate = Session["SessionDate"].ToString();
            vm.ReportType = tType;
            vm.CustomerOrderDate = Session["SessionDate"].ToString();
            vm.ProductType = "Trading";
            vm.ProductCategoryId = 6;
            vm.ProductGroup = "Trading";
            vm.FormNumeric = FormNumeric;

            return View(vm);
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult CreateEdit(SalesInvoiceMPLHeaderVM vm)
        {
            try
            {
                _repo = new SaleMPLInvoiceRepo(identity, Session);
                string[] result = new string[6];

                vm.InvoiceDateTime = Convert.ToDateTime(vm.InvoiceDateTime).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                vm.DeliveryDate = Convert.ToDateTime(vm.DeliveryDate).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");

                try
                {
                    string UserId = identity.UserId;

                    if (vm.Operation.ToLower() == "add")
                    {
                        vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                        vm.CreatedBy = identity.Name;
                        vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                        result = _repo.SalesMPLInsert(vm,null, null);

                        if (result[0] == "Success")
                        {
                            Session["result"] = result[0] + "~" + result[1];
                            return RedirectToAction("Edit", new { id = result[4], tType = vm.ReportType });
                        }
                        else
                        {
                            string msg = result[1].Split('\r').FirstOrDefault();

                            Session["result"] = result[0] + "~" + msg;
                            List<SalesInvoiceMPLDetailVM> saleDetailVMs = new List<SalesInvoiceMPLDetailVM>();
                            List<SalesInvoiceMPLBankPaymentVM> bankDetailVMs = new List<SalesInvoiceMPLBankPaymentVM>();
                            List<SalesInvoiceMPLCRInfoVM> crDetailVMs = new List<SalesInvoiceMPLCRInfoVM>();
                            if (vm.SalesInvoiceMPLDetailVMs == null)
                            {
                                vm.SalesInvoiceMPLDetailVMs = saleDetailVMs;
                            }
                            if (vm.SalesInvoiceMPLBankPaymentVMs == null)
                            {
                                vm.SalesInvoiceMPLBankPaymentVMs = bankDetailVMs;
                            }
                            if (vm.SalesInvoiceMPLCRInfoVMs == null)
                            {
                                vm.SalesInvoiceMPLCRInfoVMs = crDetailVMs;
                            }
                            return View("Create", vm);
                        }
                    }
                    else if (vm.Operation.ToLower() == "update")
                    {
                        vm.LastModifiedBy = identity.Name;
                        vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                        vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                        result = _repo.SalesMPLUpdate(vm, null, null);

                        if (result[0] == "Success")
                        {
                            Session["result"] = result[0] + "~" + result[1];
                            return RedirectToAction("Edit", new { id = vm.Id, tType = vm.ReportType });
                        }
                        else
                        {
                            string msg = result[1].Split('\r').FirstOrDefault();
                            Session["result"] = result[0] + "~" + msg;

                            List<SalesInvoiceMPLDetailVM> saleDetailVMs = new List<SalesInvoiceMPLDetailVM>();
                            List<SalesInvoiceMPLBankPaymentVM> bankDetailVMs = new List<SalesInvoiceMPLBankPaymentVM>();
                            List<SalesInvoiceMPLCRInfoVM> crDetailVMs = new List<SalesInvoiceMPLCRInfoVM>();
                            if (vm.SalesInvoiceMPLDetailVMs == null)
                            {
                                vm.SalesInvoiceMPLDetailVMs = saleDetailVMs;
                            }
                            if (vm.SalesInvoiceMPLBankPaymentVMs == null)
                            {
                                vm.SalesInvoiceMPLBankPaymentVMs = bankDetailVMs;
                            }
                            if (vm.SalesInvoiceMPLCRInfoVMs == null)
                            {
                                vm.SalesInvoiceMPLCRInfoVMs = crDetailVMs;
                            }
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
        public ActionResult Edit(string id, string tType)
        {
            SalesInvoiceMPLHeaderVM vm = new SalesInvoiceMPLHeaderVM();

            try
            {
                _repo = new SaleMPLInvoiceRepo(identity, Session);
                CommonRepo commonRepo = new CommonRepo(identity, Session);
                string FormNumeric = commonRepo.settings("DecimalPlace", "FormNumeric");

                if (tType == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                vm = _repo.SelectAll(Convert.ToInt32(id), null, null, null, null, null, "Other").FirstOrDefault();

                if (vm == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                List<SalesInvoiceMPLDetailVM> SaleDetailVMs = new List<SalesInvoiceMPLDetailVM>();
                SaleDetailVMs = _repo.SearchSaleMPLDetailList(id);
                foreach (SalesInvoiceMPLDetailVM vmD in SaleDetailVMs)
                {
                    vmD.TotalAmount = vmD.LineTotal;
                    vmD.TotalSaleVolume = Convert.ToDecimal( vmD.Quantity);
                }
                vm.SalesInvoiceMPLDetailVMs = SaleDetailVMs;

                List<SalesInvoiceMPLBankPaymentVM> bankDetailVMs = new List<SalesInvoiceMPLBankPaymentVM>();
                
                bankDetailVMs = _repo.SearchSaleMPLBankPaymentList(id);
                
                foreach (SalesInvoiceMPLBankPaymentVM vmD in bankDetailVMs)
                {
                    vmD.InstrumentDate = OrdinaryVATDesktop.DateTimeToDate(vmD.InstrumentDate);
                }
               
                vm.SalesInvoiceMPLBankPaymentVMs = bankDetailVMs;

                List<SalesInvoiceMPLCRInfoVM> crDetailVMs = new List<SalesInvoiceMPLCRInfoVM>();
                SalesInvoiceMPLCRInfoVM crDetailVM = new SalesInvoiceMPLCRInfoVM();
                
                    crDetailVMs = _repo.SearchSaleMPLCRInfoList(id);
                    var CRInfoList = crDetailVMs;

                    crDetailVMs = crDetailVMs.Where(item => item.SalesInvoiceRefId != 0).ToList();


                    foreach (SalesInvoiceMPLCRInfoVM vmD in crDetailVMs)
                    {
                        vmD.CRDate = OrdinaryVATDesktop.DateTimeToDate(vmD.CRDate);
                    }
                    vm.SalesInvoiceMPLCRInfoVMs = crDetailVMs;

                    //if (CRInfoList.Count > 0)
                    //{
                    //    vm.NewCRCode = CRInfoList.LastOrDefault().CRCode;
                    //    vm.TotalNewCRAmnt = CRInfoList.LastOrDefault().Amount;
                    //}
                    crDetailVM = _repo.SearchSaleMPLCRInfoListById(id).FirstOrDefault();

                    if (crDetailVM !=null)
                    {
                        vm.NewCRCode = crDetailVM.CRCode;
                        vm.TotalNewCRAmnt = crDetailVM.Amount;
                    }

                vm.Operation = "update";
                vm.ProductType = "Trading";
                vm.ProductCategoryId = 6;
                vm.ProductGroup = "Trading";

                vm.FormNumeric = FormNumeric;
                
                vm.ProductGroup = "Trading";
                return View("Create", vm);
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index");
            }
        }

        #region Post

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            _repo = new SaleMPLInvoiceRepo(identity, Session);
            UserInformationRepo _UserInformationRepo = new UserInformationRepo(identity, Session);
            UserInformationVM varUserInformationVM = new UserInformationVM();
            string[] a = ids.Split('~');
            var id = a[0];
            SalesInvoiceMPLHeaderVM vm = new SalesInvoiceMPLHeaderVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
           

           

            //varUserInformationVM = _UserInformationRepo.SelectAll(Convert.ToInt32(identity.UserId)).FirstOrDefault();
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);

            //vm.SignatoryName = varUserInformationVM.FullName;
            //vm.SignatoryDesig = varUserInformationVM.Designation;
            //vm.SignatoryName = identity.FullName.Split('~')[0];
            //vm.SignatoryDesig = identity.FullName.Split('~')[1];
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";
            //result = _repo.SalesPost(vm, vm.Details, new List<TrackingVM>());
            result = _repo.SalesPost(vm);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }


        //[Authorize]
        //public ActionResult MultipleSalePost(SaleMasterVM paramVM)
        //{
        //    #region Access Control
        //    _repo = new SaleMPLInvoiceRepo(identity, Session);
        //    string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
        //    if (project.ToLower() == "vms")
        //    {
        //        if (!identity.IsAdmin)
        //        {
        //        }
        //    }
        //    else
        //    {
        //        Session["rollPermission"] = "deny";
        //        return Redirect("/Vms/Home");
        //    }

        //    #endregion

        //    ResultVM rVM = new ResultVM();

        //    try
        //    {
        //        OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);

        //        paramVM.CurrentUser = identity.UserId;

        //        if (paramVM != null && paramVM.IDs != null && paramVM.IDs.Count > 0)
        //        {
        //            paramVM.IDs = paramVM.IDs.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

        //            if (paramVM.IDs == null || paramVM.IDs.Count == 0)
        //            {
        //                rVM.Message = "No Data to Post";
        //                return Json(rVM, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else
        //        {
        //            rVM.Message = "No Data to Post";
        //            return Json(rVM, JsonRequestBehavior.AllowGet);
        //        }

        //        rVM = _repo.Multiple_SalePost(paramVM);

        //    }
        //    catch (Exception)
        //    {


        //    }

        //    finally { }

        //    return Json(rVM, JsonRequestBehavior.AllowGet);
        //}

        #endregion


        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(SalesInvoiceMPLDetailVM vm)
        {
            #region Calculations



            vm.UOMn =vm.UOMn;
            vm.InputQuantity = vm.Quantity;
            vm.NBRPrice = vm.UnitPriceWithVAT * 100 / (100 + vm.VATRate);
            vm.UOMQty = vm.UOMc * vm.Quantity;
            vm.Quantity = vm.UOMQty;
            vm.UOMPrice = vm.UOMQty * vm.NBRPrice;
            vm.SubTotal = vm.UOMPrice;
            vm.VATAmount = Convert.ToDecimal(vm.SubTotal * vm.VATRate/100);
            vm.TotalAmount = Convert.ToDecimal(vm.SubTotal + vm.VATAmount);
            vm.LineTotal = Convert.ToDecimal(vm.UOMQty * vm.UnitPriceWithVAT);
            int decimalPlaces = 2; // You can change this as needed

            decimal roundedNBRPrice = Math.Round(vm.NBRPrice, decimalPlaces);
            decimal SubTotal = Math.Round(vm.SubTotal, decimalPlaces);
            decimal VATAmount = Math.Round(vm.VATAmount, decimalPlaces);
            decimal LineTotal = Math.Round(vm.LineTotal, decimalPlaces);
            vm.NBRPrice = roundedNBRPrice;
            vm.SubTotal = SubTotal;
            vm.VATAmount = VATAmount;
            vm.LineTotal = LineTotal;
            #endregion

            return PartialView("_detail", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BankPayment(SalesInvoiceMPLBankPaymentVM vm)
        {
            BankInformationRepo _repo = new BankInformationRepo(identity);
            if(vm.BankId>0)
            {
                BankInformationVM bankvm = _repo.SelectAll(Convert.ToInt32(vm.BankId)).FirstOrDefault();
                vm.BankName = bankvm.BankName;
                vm.BankCode = bankvm.BankCode;
            }

           return PartialView("_bankPayment", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult CreditInfo(SalesInvoiceMPLCRInfoVM vm)
        {
            return PartialView("_creditInfo", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult SelectProductDetails(string productCode, string IssueDate, string vatName, string TransactionType)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var _repo = new ProductRepo(identity, Session);
            CommonRepo commonRepo = new CommonRepo(identity, Session);
            string FixBOMReferenceName = commonRepo.settings("Sale", "FixBOMReferenceName");
            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };

            var product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();

            if (product == null)
            {
                conditionalFields = new string[] { "Pr.ItemNo" };
                conditionalValues = new string[] { productCode };
                product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
            }
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);
            Decimal salesPrice = 0;
            Decimal stock = 0;
            Decimal amount = 0;
            Decimal quantity = 0;
            //var name = product.ProductName;
            #region businessLogic

            string UserId = identity.UserId;

            var issueDatetime = DateTime.Parse(IssueDate).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            if (vatName == "VAT 1 (Wastage)")
            {
                stock = 0;
            }
            else
            {
                DataTable priceData = _repo.AvgPriceNew(product.ItemNo, issueDatetime, null, null, true, true, true, false, UserId);
                stock = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
            }
            if (!string.IsNullOrEmpty(FixBOMReferenceName) && FixBOMReferenceName != "-")
            {
                //Cavinkare BOMReference 
                salesPrice = _repo.CavinkareGetLastNBRPriceFromBOM(product.ItemNo, vatName, issueDatetime, FixBOMReferenceName, null, null);
            }
            else
            {
                salesPrice = _repo.GetLastNBRPriceFromBOM(product.ItemNo, vatName, issueDatetime, null, null);
            }
            salesPrice = Convert.ToDecimal(ParseDecimalObject(salesPrice));
            if (TransactionType == "TollIssue")
            {
                DataTable priceData = _repo.AvgPriceNew(product.ItemNo, issueDatetime, null, null, true, true, true, false, UserId);
                quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                stock = quantity;
                if (quantity > 0)
                {
                    salesPrice = (amount / quantity);
                }

            }
            #endregion businessLogic
            product.Stock = stock;
            product.SalesPrice = salesPrice;
            return Json(product, JsonRequestBehavior.AllowGet);
        }
        
        [Authorize]
        [HttpGet]
        public ActionResult GetCRInfoPopUp(int CustomerId, int SalesInvoiceMPLHeaderId)
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
            SalesInvoiceMPLCRInfoVM vm = new SalesInvoiceMPLCRInfoVM();
          
            return PartialView("_crInfo", vm);
        }
        
        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredCRInfo(SalesInvoiceMPLCRInfoVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new SaleMPLInvoiceRepo(identity, Session);
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



            List<SalesInvoiceMPLCRInfoVM> list = _repo.SelectAll(vm);
            SalesInvoiceMPLCRInfoVM CRInfo = new SalesInvoiceMPLCRInfoVM();
            if (list.Count>0)
            {
                CRInfo = list.FirstOrDefault();
            }
            //return PartialView("_filteredCRInfo", list);
            return Json(CRInfo, JsonRequestBehavior.AllowGet);

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
        public ActionResult LoadTankInfo(string ItemNo, int MasterId, int TankId)
        {
            try
            {
                TankMPLsVM vm = new TankMPLsVM();
                vm.ItemNo = ItemNo.Trim();
                vm.SalesInvoiceDetailsRefId = MasterId;
                vm.TankId = TankId;
                return PartialView("_loadTankInfo", vm);
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult UpdateSalesDetailsTankInfo(SalesInvoiceMPLDetailVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new SaleMPLInvoiceRepo(identity, Session);

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
                result = _repo.UpdateSalesDetailsTankInfo(vm,null,null);
                Session["result"] = result[0] + "~" + result[1];
                return Json(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                return Json(ex.Message.ToString());
            }
        }




    }
}
