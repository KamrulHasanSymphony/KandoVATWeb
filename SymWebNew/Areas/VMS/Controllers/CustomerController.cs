using SymOrdinary;
using SymRepository.VMS;
using SymVATWebUI.Areas.VMS.Models;
using VATViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymphonySofttech.Reports.Report;
using VATServer.License;
using VATServer.Ordinary;
using System.IO;
using Newtonsoft.Json;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class CustomerController : Controller
    {

        ShampanIdentity identity = null;

        CustomerRepo _repo = null;

        public CustomerController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new CustomerRepo(identity);
            }
            catch
            {

            }

        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index(CustomerVM paramVM)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
            }

            string customerSync = new CommonRepo(identity, Session).settings("Customer", "CustomerSync");
            paramVM.CustomerSync = customerSync == "Y" && !string.IsNullOrEmpty(customerSync);
            return View(paramVM);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param, CustomerVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CustomerRepo(identity, Session);

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
            }
            //00     //Id 
            //01     //CustomerName
            //02     //Address1
            //03     //TelephoneNo  
            //04     //Email
            //05    //VDSPercent

            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "100";
            }

            #region Search and Filter Data
            string[] cFields = { "CustomerCode like", "CustomerName like", "c.CustomerGroupID", "City like", "ActiveStatus", "StartDateTime>", "StartDateTime<", "ContactPerson", "TINNo", "VATRegistrationNo", "SelectTop" };
            string[] cValues = { paramVM.CustomerCode, paramVM.CustomerName, paramVM.CustomerGroupID, paramVM.City, paramVM.IsActive, paramVM.StartDateFrom, paramVM.StartDateTo, paramVM.ContactPerson, paramVM.TINNo, paramVM.VATRegistrationNo, paramVM.SelectTop };
            var getAllData = _repo.SelectAll("0", cFields, cValues);
            IEnumerable<CustomerVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);



                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.CustomerName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Address1.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.TelephoneNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Email.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.ContactPersonEmail.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.VDSPercent.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.CustomerCode.ToString().ToLower().Contains(param.sSearch.ToLower())

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
            Func<CustomerVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.CustomerName :
                sortColumnIndex == 2 && isSortable_2 ? c.Address1.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.TelephoneNo.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.Email.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.VDSPercent.ToString() :
                sortColumnIndex == 6 && isSortable_5 ? c.CustomerCode.ToString() :

                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                c.CustomerID
                , c.CustomerCode
                , c.CustomerName
                , c.Address1.ToString()
                , c.TelephoneNo.ToString()
                , c.Email.ToString()
                , c.VDSPercent.ToString()
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
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
            }
            CustomerVM vm = new CustomerVM();
            vm.Operation = "add";
            vm.ActiveStatus = "Y";
            vm.IsInstitution = "N";

            CommonRepo commonrepo = new CommonRepo(identity, Session);

            string code = commonrepo.settings("CompanyCode", "Code");

            vm.CompanyCode = code;

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(CustomerVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CustomerRepo(identity, Session);

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
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
                    result = _repo.InsertToCustomerNew(vm);
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
                    result = _repo.UpdateToCustomerNew(vm);
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
                //  Session["result"] = "Fail~Data Not Succeessfully!";
                // FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("CustomerController", "CreateEdit", ex.ToString());
                return View("Create", vm);
            }
        }

        public ActionResult Navigate(string id, string btn)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetId("Customers", "CustomerID", id, btn);
            return RedirectToAction("Edit", new { id = targetId });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CustomerRepo(identity, Session);

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
            }
            CustomerVM vm = new CustomerVM();
            vm = _repo.SelectAll(id).FirstOrDefault();
            vm.StartDateTime = Ordinary.DateTimeToDate(vm.StartDateTime);
            vm.Operation = "update";

            DataTable CustomerAddressResult = new DataTable();

            CustomerAddressResult = _repo.SearchCustomerAddress(vm.CustomerID);

            vm.Details = new List<CustomerAddressVM>();

            foreach (DataRow row in CustomerAddressResult.Rows)
            {
                CustomerAddressVM Customer = new CustomerAddressVM();

                Customer.CustomerID = row["CustomerID"].ToString();
                Customer.Id = Convert.ToInt32(row["Id"].ToString());
                Customer.CustomerAddress = row["CustomerAddress"].ToString();

                vm.Details.Add(Customer);
            }
            #region Discount

            DataTable CustomerDiscount = new DataTable();

            CustomerDiscount = _repo.SearchCustomerDiscount(vm.CustomerID);

            vm.Discount = new List<CustomerDiscountVM>();

            foreach (DataRow row in CustomerDiscount.Rows)
            {
                CustomerDiscountVM Customer = new CustomerDiscountVM();

                Customer.Id = Convert.ToInt32(row["Id"].ToString());
                Customer.CustomerID = row["CustomerID"].ToString();

                Customer.MinValue = Convert.ToDecimal(row["MinValue"].ToString());
                Customer.MaxValue = Convert.ToDecimal(row["MaxValue"].ToString());
                Customer.Rate = Convert.ToDecimal(row["Rate"].ToString());
                Customer.Comments = row["Comments"].ToString();

                vm.Discount.Add(Customer);
            }

            #endregion

            CommonRepo commonrepo = new CommonRepo(identity, Session);

            string code = commonrepo.settings("CompanyCode", "Code");

            vm.CompanyCode = code;

            if (code.ToLower() == "TelNet".ToLower())
            {
                CustomerItemRepo _repoItem = new CustomerItemRepo();
                CustomerBillProcessVM BillProcessVM = new CustomerBillProcessVM();
                BillProcessVM = _repoItem.SelectAllCustomerBillProcessList(vm.CustomerID).FirstOrDefault();

                vm.CustomerBillProcess = BillProcessVM;

            }

            return View("Create", vm);
        }

        [HttpPost]
        public ActionResult ImportExcel(CustomerVM customerVM)
        {
            string[] result = new string[6];
            try
            {
                _repo = new CustomerRepo(identity, Session);

                IntegrationParam vm = new IntegrationParam();

                vm.File = customerVM.File;
                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                vm.BranchId = Convert.ToString(Session["BranchId"]);
                vm.BranchCode = Session["BranchCode"].ToString();

                result = _repo.ImportExcelIntegrationFile(vm);
                Session["result"] = result[0] + "~" + result[1];
                return Json(JsonConvert.SerializeObject(new { message = "Saved Successfully", action = result[0] }),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Session["result"] = result[0] + "~" + result[1];

                Elmah.ErrorSignal.FromCurrentContext().Raise(e);

                return Json(JsonConvert.SerializeObject(new { message = e.Message, action = "Fail" }),
                    JsonRequestBehavior.AllowGet);

            }
        }



        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CustomerRepo(identity, Session);
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
            }
            CustomerVM vm = new CustomerVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.LastModifiedBy = identity.Name;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewTable()
        {
            return View();
        }

        public ActionResult _dataSource()
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CustomerRepo(identity, Session);
            #region authorization
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
            }
            #endregion authorization
            var getAllData = _repo.SelectAll();

            var result = from c in getAllData
                         select new[] { 
                  c.CustomerID
                , c.CustomerName
                , c.Address1.ToString()
                , c.TelephoneNo.ToString()
                , c.Email.ToString()
                , c.VDSPercent.ToString()
                , c.CustomerName
                , c.CustomerName
                , c.CustomerName
                , c.CustomerName
            };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Print()
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
            return PartialView("_printCustomer");
        }

        [Authorize]
        public ActionResult ReportView(string cId, string cgId)
        {
            try
            {
                if (cId == null)
                {
                    cId = "";
                }
                if (cgId == null)
                {
                    cgId = "";
                }
                var ReportResult = new DataSet();
                ReportDSRepo reportDsdal = new ReportDSRepo(identity, Session);
                ReportResult = reportDsdal.CustomerNew(cId, cgId);
                if (ReportResult.Tables.Count <= 0)
                {
                    //some codes here
                }
                ReportResult.Tables[0].TableName = "DsCustomer";
                RptCustomerListing objrpt = new RptCustomerListing();
                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + identity.Name + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Customer Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + identity.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + identity.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + identity.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + identity.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + identity.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + identity.FaxNo + "'";

                var gr = new GenericReport<RptCustomerListing>();
                var rpt = gr.RenderReportAsPDF(objrpt);
                objrpt.Close();
                //if (Convert.ToDecimal(LicenseExpite.ExpiteDate(identity.InitialCatalog)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                //    reports.ShowDialog();

                return rpt;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ActionResult GetItemGroupId(string ItemId)
        {
            var vm = new CustomerRepo(identity, Session).SelectAll(ItemId).FirstOrDefault();
            string tinNo = vm.TINNo;
            string vatNo = vm.VATRegistrationNo;
            var data = vm.CustomerGroupID + "~" + tinNo + "~" + vatNo;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DropdownByGroup(string groupId)
        {
            string[] conditionFields = { "CustomerGroupID" };
            string[] conditionValues = { groupId };
            return Json(new SelectList(new SymRepository.VMS.CustomerRepo(identity, Session).SelectAll(null, conditionFields, conditionValues), "CustomerID", "CustomerName"), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetCustomerPopUp(string CustomerName)
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
            CustomerVM vm = new CustomerVM();
            vm.CustomerName = CustomerName;
            return PartialView("_customers", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetCustomerAddressPopUp(string CustomerName, string CustomerId)
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
            CustomerAddressVM vm = new CustomerAddressVM();
            vm.CustomerName = CustomerName;
            vm.CustomerID = CustomerId;

            return PartialView("_customersAddressSearch", vm);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredCustomers(CustomerVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CustomerRepo(identity, Session);
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
            var activeSatus = vm.ActiveStatus == "true" ? "Y" : "N";
            string[] conditionalFields;
            string[] conditionalValues;
            if (vm.SearchField != null)
            {
                conditionalFields = new string[] { vm.SearchField + " like", "c.CustomerGroupID", "c.ActiveStatus", "StartDateTime>=", "StartDateTime<=" };
                conditionalValues = new string[] { vm.SearchValue, vm.CustomerGroupID, activeSatus, vm.StartDateFrom, vm.StartDateTo };
            }
            else
            {
                conditionalFields = new string[] { "c.CustomerGroupID", "c.ActiveStatus", "StartDateTime>=", "StartDateTime<=" };
                conditionalValues = new string[] { vm.CustomerGroupID, activeSatus, vm.StartDateFrom, vm.StartDateTo };
            }
            var list = _repo.SelectAll("0", conditionalFields, conditionalValues);

            return PartialView("_filteredCustomers", list);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredCustomersAddress(CustomerVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CustomerRepo(identity, Session);
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
            DataTable CustomerAddressResult = new DataTable();

            CustomerAddressResult = _repo.SearchCustomerAddress(vm.CustomerID);

            List<CustomerAddressVM> VMS = new List<CustomerAddressVM>();

            foreach (DataRow row in CustomerAddressResult.Rows)
            {
                CustomerAddressVM Customer = new CustomerAddressVM();

                Customer.CustomerID = row["CustomerID"].ToString();
                Customer.Id = Convert.ToInt32(row["Id"].ToString());
                Customer.CustomerAddress = row["CustomerAddress"].ToString();

                Customer.CustomerCode = row["CustomerCode"].ToString();
                Customer.CustomerName = row["CustomerName"].ToString();
                Customer.CustomerGroupName = row["CustomerGroupName"].ToString();
                Customer.CustomerVATRegNo = row["CustomerVATRegNo"].ToString();

                VMS.Add(Customer);
            }

            return PartialView("_filteredCustomersAddress", VMS);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(CustomerAddressVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CustomerRepo(identity, Session);

            string[] sqlResults = new string[4];

            if (vm.Operation == "add")
            {
                sqlResults = _repo.InsertToCustomerAddress(vm);
                vm.Id = Convert.ToInt32(sqlResults[2]);
            }
            else if (vm.Operation == "update")
            {
                sqlResults = _repo.UpdateToCustomerAddress(vm);
            }

            //sqlResults = _repo.InsertToCustomerAddress(vm);

            if (sqlResults[0] == "Fail")
            {
                Session["result"] = sqlResults[0] + "~" + sqlResults[1];

                throw new Exception("Fail");

            }


            return PartialView("_customerAddress", vm);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAddress(string id, string CustomerId)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CustomerRepo(identity, Session);
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
            }
            CustomerVM vm = new CustomerVM();
            string[] result = new string[6];
            result = _repo.DeleteCustomerAddress("", id);

            Session["result"] = result[0] + "~" + result[1];
            if (result[0].ToLower() == "success")
            {
                return RedirectToAction("Edit", new { id = CustomerId });
            }
            else
            {
                return View("Create", vm);
            }



            //return Json(result[1], JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult BlankItems(CustomerDiscountVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CustomerRepo(identity, Session);
            string[] sqlResults = new string[4];

            if (string.IsNullOrWhiteSpace(vm.Comments))
            {
                vm.Comments = "NA";
            }

            if (vm.Operation == "add")
            {
                vm.CreatedOn = DateTime.Now.ToString();
                vm.CreatedBy = identity.Name;

                sqlResults = _repo.InsertToCustomerDiscountNew(vm);
                vm.Id = Convert.ToInt32(sqlResults[2]);
            }
            else if (vm.Operation == "update")
            {
                vm.LastModifiedOn = DateTime.Now.ToString();
                vm.LastModifiedBy = identity.Name;

                sqlResults = _repo.UpdateToCustomerDiscountNew(vm);
            }

            if (sqlResults[0] == "Fail")
            {
                Session["result"] = sqlResults[0] + "~" + sqlResults[1];

                throw new Exception("Fail");

            }

            return PartialView("_customerDiscount", vm);


        }


        [Authorize(Roles = "Admin")]
        public ActionResult DeleteDiscount(string id, string CustomerId)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CustomerRepo(identity, Session);

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
            }
            CustomerVM vm = new CustomerVM();
            string[] result = new string[6];
            result = _repo.DeleteCustomerDiscount("", id);

            Session["result"] = result[0] + "~" + result[1];
            if (result[0].ToLower() == "success")
            {
                return RedirectToAction("Edit", new { id = CustomerId });
            }
            else
            {
                return View("Create", vm);
            }



            //return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult SyncCustomer()
        {
            try
            {
                DataTable customerDt = new DataTable();
                string[] results = new string[4];
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                ImportRepo importrepo = new ImportRepo(identity, Session);
                CommonRepo commonrepo = new CommonRepo(identity, Session);
                IntegrationRepo integrationrepo = new IntegrationRepo(identity, Session);
                results[0] = "fail";
                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                var BranchInfoDT = branchProfileRepo.SelectAl(Convert.ToString(Session["BranchId"]));
                string code = commonrepo.settings("CompanyCode", "Code");
                settingVM.BranchInfoDT = BranchInfoDT;

                if (OrdinaryVATDesktop.IsACICompany(Convert.ToString(Session["CompanyCode"])))
                {
                    customerDt = importrepo.GetCustomerACIDbData(settingVM.BranchInfoDT);
                }
                else if (OrdinaryVATDesktop.IsUnileverCompany(Convert.ToString(Session["CompanyCode"])))
                {
                    customerDt = importrepo.GetCustomerUnileverDbData(settingVM.BranchInfoDT);
                }

                else if (code.ToLower() == "eon" || code.ToLower() == "eahpl" || code.ToLower() == "eail" || code.ToLower() == "eeufl" || code.ToLower() == "exfl")
                {
                    customerDt = integrationrepo.GetCustomerEONAPIData(settingVM.BranchInfoDT);
                }

                List<CustomerVM> customers = new List<CustomerVM>();

                int rowsCount = customerDt.Rows.Count;
                List<string> ids = new List<string>();

                string defaultGroup = commonrepo.settings("AutoSave", "DefaultCustomerGroup");

                for (int i = 0; i < rowsCount; i++)
                {
                    CustomerVM customer = new CustomerVM();

                    customer.CustomerName =
                        Ordinary.RemoveStringExpresion(customerDt.Rows[i]["CustomerName"].ToString());
                    customer.CustomerCode =
                        Ordinary.RemoveStringExpresion(customerDt.Rows[i]["CustomerCode"].ToString());
                    customer.CustomerGroup = customerDt.Rows[i]["CustomerGroup"].ToString();
                    customer.Address1 = customerDt.Rows[i]["Address"].ToString();

                    if (customer.CustomerGroup == "-" || string.IsNullOrWhiteSpace(customer.CustomerGroup))
                    {
                        customer.CustomerGroup = defaultGroup;
                    }


                    if (OrdinaryVATDesktop.IsUnileverCompany(Convert.ToString(Session["CompanyCode"])))
                    {
                        customer.FaxNo = customerDt.Rows[i]["FaxNo"].ToString(); ;
                        customer.Email = customerDt.Rows[i]["Email"].ToString(); ;
                        customer.TINNo = customerDt.Rows[i]["TINNo"].ToString(); ;
                        customer.ContactPerson = OrdinaryVATDesktop.RemoveStringExpresion(customerDt.Rows[i]["ContactPerson"].ToString());
                        customer.ContactPersonTelephone = customerDt.Rows[i]["ContactPersonTelephone"].ToString();
                        customer.CustomerBanglaName = customerDt.Rows[i]["CustomerBanglaName"].ToString();
                        customer.Address3 = customerDt.Rows[i]["BanglaAddress"].ToString();
                        customer.BranchId = 1;
                    }
                    else
                    {
                        customer.FaxNo = "-";
                        customer.Email = "-";
                        customer.TINNo = "-";
                        customer.ContactPerson = "-";
                        customer.ContactPersonTelephone = "-";
                        customer.BranchId = OrdinaryVATDesktop.BranchId;
                    }

                    customer.City = "-";
                    customer.TelephoneNo = "-";
                    //customer.FaxNo = "-";
                    //customer.Email = "-";
                    customer.StartDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    //customer.ContactPerson = "-";
                    customer.ContactPersonDesignation = "-";
                    //customer.ContactPersonTelephone = "-";
                    customer.ContactPersonEmail = "-";
                    ;
                    //customer.TINNo = "-";
                    ;
                    customer.VATRegistrationNo = customerDt.Rows[i]["BIN_No"].ToString();
                    customer.Comments = "-";
                    customer.ActiveStatus = "Y";
                    customer.CreatedBy = OrdinaryVATDesktop.CurrentUser; // need to change
                    customer.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    customer.Country = "-";
                    customer.IsVDSWithHolder = "N";
                    customer.IsInstitution = "N";
                    customers.Add(customer);

                    ids.Add(customerDt.Rows[i]["SL"].ToString());
                }



                results = importrepo.ImportCustomer(customers);

                if (results[0].ToLower() == "success")
                {
                    if (OrdinaryVATDesktop.IsACICompany(Convert.ToString(Session["CompanyCode"])))
                    {
                        results = importrepo.UpdateACIMaster(ids, settingVM.BranchInfoDT, "Customers");
                    }
                    else if (OrdinaryVATDesktop.IsUnileverCompany(Convert.ToString(Session["CompanyCode"])))
                    {
                        results = importrepo.UpdateUnileverMaster(ids, settingVM.BranchInfoDT, "Customers");
                    }

                }
                if (results[0].ToLower() == "success")
                {
                    Session["result"] = "Success~Successfully Synchronized";
                    return Redirect("/Vms/Customer/Index");
                }
                else
                {
                    Session["result"] = "Fail~Nothing to syncronize";
                    return Redirect("/Vms/Customer/Index");
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                // Session["result"] = "Fail~Fail";
                return Redirect("/Vms/Customer/Index");
            }
        }



        [Authorize]
        public ActionResult ExportExcell(CustomerVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CustomerRepo(identity, Session);

            ResultVM rVM = new ResultVM();

            List<CustomerVM> getAllData = new List<CustomerVM>();
            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "All";
            }
            try
            {

                if (paramVM.ExportAll)
                {
                    string[] conditionFields = new string[] {  "SelectTop" };
                    string[] conditionValues = new string[] {paramVM.SelectTop };

                    List<CustomerVM> customers = _repo.SelectAll("0", conditionFields, conditionValues);

                    paramVM.CustomerIDs = customers.Select(x => x.CustomerCode).ToList();

                }


                DataTable dt = _repo.GetExcelData(paramVM.CustomerIDs);
                var address = _repo.GetExcelAddress(paramVM.CustomerIDs);

                var dataSet = new DataSet();
                dataSet.Tables.Add(dt);

                var sheetNames = new[] { "Customer" };

                if (address.Rows.Count > 0)
                {
                    dataSet.Tables.Add(address);
                    sheetNames = new[] { "Customer", "CustomerAddress" };
                }

                var vm = OrdinaryVATDesktop.DownloadExcelMultiple(dataSet, "Customers", sheetNames);
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

        public ActionResult AddCustomerBill(CustomerBillProcessVM vm)
        {
            ResultVM rVM = new ResultVM();
            CustomerItemRepo _repo = new CustomerItemRepo();

            try
            {
                vm.Jan = vm.JanChecked == true ? "Y" : "N";
                vm.Feb = vm.FebChecked == true ? "Y" : "N";
                vm.Mar = vm.MarChecked == true ? "Y" : "N";
                vm.Apr = vm.AprChecked == true ? "Y" : "N";
                vm.May = vm.MayChecked == true ? "Y" : "N";
                vm.Jun = vm.JunChecked == true ? "Y" : "N";
                vm.Jul = vm.JulChecked == true ? "Y" : "N";
                vm.Aug = vm.AugChecked == true ? "Y" : "N";
                vm.Sep = vm.SepChecked == true ? "Y" : "N";
                vm.Oct = vm.OctChecked == true ? "Y" : "N";
                vm.Nov = vm.NovChecked == true ? "Y" : "N";
                vm.Dec = vm.DecChecked == true ? "Y" : "N";

                vm.CreatedOn = DateTime.Now.ToString();
                vm.CreatedBy = identity.Name;

                vm.LastModifiedOn = DateTime.Now.ToString();
                vm.LastModifiedBy = identity.Name;

                rVM = _repo.CustomerBillProcess(vm);

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault();

            }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }


    }
}
