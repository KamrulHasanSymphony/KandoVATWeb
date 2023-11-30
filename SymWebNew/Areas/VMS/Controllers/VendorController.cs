//using JQueryDataTables.Models;
using SymOrdinary;
using SymphonySofttech.Reports.Report;
//using SymRepository.Common;
using SymRepository.VMS;
using SymVATWebUI.Areas.VMS.Models;
//using SymViewModel.Common;
using VATViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using VATServer.Ordinary;
using System.IO;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class VendorController : Controller
    {
        //
        // GET: /VMS/Branch/

        // 
        ShampanIdentity identity = null;

        VendorRepo _repo = null;

        public VendorController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new VendorRepo(identity);

            }
            catch
            {

            }
        }
        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //VendorRepo _repo = new VendorRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index(VendorVM paramVM)
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
            return View(paramVM);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param, VendorVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new VendorRepo(identity, Session);

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
            //01     //VendorName
            //02     //Address1
            //03     //TelephoneNo  
            //04     //Email
            //05    //VDSPercent
            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "100";
            }

            #region Search and Filter Data
            string[] cFields;
            string[] cValues;
            if (string.IsNullOrWhiteSpace(paramVM.SearchField))
            {
                cFields = new string[] { "v.VendorGroupID", "v.StartDateTime>", "v.StartDateTime<", "v.ActiveStatus", "SelectTop" };
                cValues = new string[] { paramVM.VendorGroupID, paramVM.StartDateFrom, paramVM.StartDateTo, paramVM.IsActive, paramVM.SelectTop };
            }
            else
            {
                cFields = new string[] { paramVM.SearchField + " like", "VendorGroupID", "StartDateTime>", "StartDateTime<", "ActiveStatus", "SelectTop" };
                cValues = new string[] { paramVM.SearchValue, paramVM.VendorGroupID, paramVM.StartDateFrom, paramVM.StartDateTo, paramVM.IsActive, paramVM.SelectTop };
            }
            var getAllData = _repo.SelectAll(0, cFields, cValues);
            IEnumerable<VendorVM> filteredData;
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
                       isSearchable1 && c.VendorName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Address1.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.TelephoneNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Email.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.ContactPersonEmail.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.VDSPercent.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.VendorCode.ToString().ToLower().Contains(param.sSearch.ToLower())

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
            Func<VendorVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.VendorName :
                sortColumnIndex == 2 && isSortable_2 ? c.Address1.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.TelephoneNo.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.Email.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.VDSPercent.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.VendorCode.ToString() :

                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                c.VendorID
                , c.VendorCode
                , c.VendorName
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
            VendorVM vm = new VendorVM();
            vm.Operation = "add";
            vm.ActiveStatus = "Y";
            vm.IsTurnover = "N";
            vm.IsRegister = "N";
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(VendorVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new VendorRepo(identity, Session);

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


                    if (string.IsNullOrWhiteSpace(vm.StartDateTime))
                    {
                        var StartDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                        vm.StartDateTime = StartDateTime.ToString();
                    }

                    if (string.IsNullOrWhiteSpace(vm.VATRegistrationNo))
                    {
                        vm.VATRegistrationNo = "-";
                    }



                    if (string.IsNullOrWhiteSpace(vm.TINNo))
                    {
                        vm.TINNo = "-";
                    }
                    if (vm.VDSPercent == 0 || vm.VDSPercent == null)
                    {
                        vm.VDSPercent = 0;
                    }
                    if (string.IsNullOrWhiteSpace(vm.BusinessType))
                    {
                        vm.BusinessType = "-";
                    }
                    if (string.IsNullOrWhiteSpace(vm.BusinessCode))
                    {
                        vm.BusinessCode = "-";
                    }

                    if (string.IsNullOrWhiteSpace(vm.Address1))
                    {
                        vm.Address1 = "-";
                    }
                    vm.Address2 = "-";
                    vm.Address3 = "-";



                    if (string.IsNullOrWhiteSpace(vm.City))
                    {
                        vm.City = "-";
                    }
                    if (string.IsNullOrWhiteSpace(vm.Country))
                    {
                        vm.Country = "-";
                    }
                    if (string.IsNullOrWhiteSpace(vm.Email))
                    {
                        vm.Email = "-";
                    }
                    if (string.IsNullOrWhiteSpace(vm.FaxNo))
                    {
                        vm.FaxNo = "-";
                    }
                    if (string.IsNullOrWhiteSpace(vm.TelephoneNo))
                    {
                        vm.TelephoneNo = "-";
                    }
                    if (string.IsNullOrWhiteSpace(vm.Comments))
                    {
                        vm.Comments = "-";
                    }

                    if (string.IsNullOrWhiteSpace(vm.ShortName))
                    {
                        vm.ShortName = "-";
                    }



                    vm.CreatedOn = DateTime.Now.ToString();
                    vm.CreatedBy = identity.Name;
                    //vm.ActiveStatus = true;
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    result = _repo.InsertToVendorNewSQL(vm);
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
                    result = _repo.UpdateVendorNewSQL(vm);
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
                FileLogger.Log("VendorController", "CreateEdit", ex.ToString());
                return View("Create", vm);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new VendorRepo(identity, Session);

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
            VendorVM vm = new VendorVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new VendorRepo(identity, Session);

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
            VendorVM vm = new VendorVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.LastModifiedBy = identity.Name;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        public ActionResult Navigate(string id, string btn)
        {


            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetId("Vendors", "VendorID", id, btn);
            return RedirectToAction("Edit", new { id = targetId });
        }

        [Authorize]
        public ActionResult PrintVendor()
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
            return PartialView("_printVendor");
        }

        [Authorize]
        public ActionResult ReportView(string VendorId, string VendorGroupId)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new VendorRepo(identity, Session);
            try
            {
                if (VendorId == null)
                {
                    VendorId = "";
                }
                if (VendorGroupId == null)
                {
                    VendorGroupId = "";
                }
                var ReportResult = new DataSet();
                ReportDSRepo reportDsdal = new ReportDSRepo(identity, Session);
                ReportResult = reportDsdal.VendorReportNew(VendorId, VendorGroupId);
                if (ReportResult.Tables.Count <= 0)
                {
                    //some codes here
                }
                ReportResult.Tables[0].TableName = "DsVendor";
                RptVendorListing objrpt = new RptVendorListing();
                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + identity.FullName + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Vendor Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + identity.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Program.Address1'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'Program.Address2'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'Program.Address3'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                var gr = new GenericReport<RptVendorListing>();
                var rpt = gr.RenderReportAsPDF(objrpt);
                objrpt.Close();
                return rpt;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public JsonResult DropdownByGroup(string groupId)
        {
            string[] conditionFields = { "VendorGroupId" };
            string[] conditionValues = { groupId };
            return Json(new SelectList(new SymRepository.VMS.VendorRepo(identity, Session).SelectAll(0, conditionFields, conditionValues), "VendorID", "VendorName"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVendorGroupId(string vendorId)
        {
            var vm = new VendorRepo(identity, Session).SelectAll(Convert.ToInt32(vendorId)).FirstOrDefault();
            return Json(vm.VendorGroupID, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetVendorPopUp(string VendorName)
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
            VendorVM vm = new VendorVM();
            vm.VendorName = VendorName;
            return PartialView("_vendors", vm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredVendors(VendorVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new VendorRepo(identity, Session);

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
            if (vm.SearchField != null)
            {
                conditionalFields = new string[] { "v." + vm.SearchField + " like", "v.VendorGroupID", "v.ActiveStatus", "v.StartDateTime>=", "v.StartDateTime<=" };
                conditionalValues = new string[] { vm.SearchValue, vm.VendorGroupID, vm.IsActive, vm.StartDateFrom, vm.StartDateTo };
            }
            else
            {
                conditionalFields = new string[] { "v.VendorGroupID", "v.ActiveStatus", "v.StartDateTime>=", "v.StartDateTime<=" };
                conditionalValues = new string[] { vm.VendorGroupID, vm.IsActive, vm.StartDateFrom, vm.StartDateTo };
            }
            var list = _repo.SelectAll(0, conditionalFields, conditionalValues);

            return PartialView("_filteredVendors", list);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult SyncVendor()
        {
            try
            {
                string[] results = new string[4];

                ImportRepo importrepo = new ImportRepo(identity, Session);
                CommonRepo commonrepo = new CommonRepo(identity, Session);
                results[0] = "fail";
                BranchProfileRepo branchProfileRepo = new BranchProfileRepo(identity, Session);
                var BranchInfoDT = branchProfileRepo.SelectAl(Convert.ToString(Session["BranchId"]));
                string code = commonrepo.settings("CompanyCode", "Code");
                settingVM.BranchInfoDT = BranchInfoDT;
                DataTable vendorsDt = importrepo.GetVendorACIDbData(settingVM.BranchInfoDT);

                List<VendorVM> vendors = new List<VendorVM>();


                int rowsCount = vendorsDt.Rows.Count;
                List<string> ids = new List<string>();

                string defaultGroup = commonrepo.settings("AutoSave", "DefaultVendorGroup");

                //if(defaultGroup == "-")
                //{
                //    MessageBox.Show("Default Vendor Group Not Found");
                //}

                for (int i = 0; i < rowsCount; i++)
                {
                    VendorVM vendor = new VendorVM();
                    vendor.VendorCode = Ordinary.RemoveStringExpresion(vendorsDt.Rows[i]["VendorCode"].ToString());
                    vendor.VendorName = Ordinary.RemoveStringExpresion(vendorsDt.Rows[i]["VendorName"].ToString());

                    vendor.VendorGroup = vendorsDt.Rows[i]["VendorGroup"].ToString();

                    if (vendor.VendorGroup == "-")
                    {
                        if (defaultGroup == "-")
                        {
                            throw new Exception("Default Vendor Group Not Found.\nPlease set Default Vendor Group in Setting .");
                        }
                        vendor.VendorGroup = defaultGroup;
                    }

                    vendor.Address1 = vendorsDt.Rows[i]["Address"].ToString();
                    vendor.Address2 = "-";
                    vendor.Address3 = "-";

                    vendor.City = "-";
                    vendor.TelephoneNo = vendorsDt.Rows[i]["TelephoneNo"].ToString();
                    vendor.FaxNo = "-";
                    vendor.Email = "-";

                    vendor.StartDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    vendor.ContactPerson = "-";
                    vendor.ContactPersonDesignation = "-";
                    vendor.ContactPersonTelephone = "-";
                    vendor.ContactPersonEmail = "-";
                    vendor.VATRegistrationNo = vendorsDt.Rows[i]["BIN_No"].ToString();
                    vendor.TINNo = "-";
                    vendor.Comments = "-";
                    vendor.ActiveStatus = "Y";
                    vendor.CreatedBy = OrdinaryVATDesktop.CurrentUser;
                    vendor.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    vendor.Country = "-";
                    vendor.BranchId = OrdinaryVATDesktop.BranchId;
                    vendors.Add(vendor);

                    ids.Add(vendorsDt.Rows[i]["SL"].ToString());
                }


                results = importrepo.ImportVendor(vendors);

                if (results[0].ToLower() == "success")
                {
                    results = importrepo.UpdateACIMaster(ids, settingVM.BranchInfoDT);

                }
                if (results[0].ToLower() == "success")
                {
                    Session["result"] = "Success~Successfully Synchronized";
                    return Redirect("/Vms/Vendor/Index");
                }
                else
                {
                    Session["result"] = "Fail~Nothing to syncronize";
                    return Redirect("/Vms/Vendor/Index");
                }
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail~Fail";
                return Redirect("/Vms/Vendor/Index");
            }
        }

        [Authorize]
        public ActionResult ExportExcell(VendorVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new VendorRepo(identity, Session);

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

                    List<VendorVM> vendors = _repo.SelectAll(0, conditionFields, conditionValues);
                    paramVM.VendorIDs = vendors.Select(x => x.VendorCode).ToList();
                }

                DataTable dt = _repo.GetExcelData(paramVM.VendorIDs);
                var dataSet = new DataSet();
                dataSet.Tables.Add(dt);
                var sheetNames = new[] { "Vendor" };

                var vm = OrdinaryVATDesktop.DownloadExcelMultiple(dataSet, "Vendors", sheetNames);
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
            catch (Exception e)
            {


            }
            finally { }
            return RedirectToAction("Index");
        }
    }
}
