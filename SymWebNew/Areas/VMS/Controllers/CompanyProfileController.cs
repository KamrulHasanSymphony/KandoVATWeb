//using JQueryDataTables.Models;
using SymOrdinary;
using SymphonySofttech.Reports.Report;
//using SymRepository.Common;
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
using SymphonySofttech.Utilities;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class CompanyProfileController : Controller
    {
        //
        // GET: /VMS/Branch/
        ShampanIdentity identity = null;

        CompanyProfileRepo _repo = null;

        public CompanyProfileController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new CompanyProfileRepo(identity);

            }
            catch
            {

            }
        }


        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        // CompanyProfileRepo _repo = new CompanyProfileRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
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
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CompanyProfileRepo(identity, Session);

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
            //00     //CompanyID 
            //01     //CompanyName
            //02     //Address1
            //03     //ZipCode  
            //04     //Email
            //05    //TelephoneNo


            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<CompanyProfileVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);


                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.CompanyName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Address1.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.ZipCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Email.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.ContactPersonEmail.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<CompanyProfileVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.CompanyName :
                sortColumnIndex == 2 && isSortable_2 ? c.Address1 :
                sortColumnIndex == 3 && isSortable_3 ? c.ZipCode :
                sortColumnIndex == 4 && isSortable_4 ? c.Email :
                sortColumnIndex == 5 && isSortable_5 ? c.TelephoneNo :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                c.CompanyID
                , c.CompanyName
                , c.Address1
                , c.ZipCode
                , c.Email
                , c.TelephoneNo
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
        [HttpPost]
        public ActionResult CreateEdit(CompanyProfileVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CompanyProfileRepo(identity, Session);

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
                    vm.LastModifiedOn = DateTime.Now.ToString(); ;
                    //result = _repo.Insert(vm);
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
                    result = _repo.UpdateCompanyProfileNew(vm);
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
                //  FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("CompanyProfileController", "CreateEdit", ex.ToString());
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEditN(CompanyProfileVM vmN)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CompanyProfileRepo(identity, Session);

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
            CompanyProfileVM vm = new CompanyProfileVM();
            vm = _repo.SelectAll().FirstOrDefault();
            vm.Operation = vmN.Operation;
            vm.IsSymphonyUser = vmN.IsSymphonyUser;
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new CompanyProfileRepo(identity, Session);

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
            CompanyProfileVM vm = new CompanyProfileVM();
            vm = _repo.SelectAll(id).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }


        [Authorize]
        public ActionResult ReportView()
        {
            try
            {
                var company = new CompanyProfileRepo(identity, Session).SelectAll().FirstOrDefault();
                var ReportResult = new DataSet();
                ReportDSRepo reportDsdal = new ReportDSRepo(identity, Session);
                if (company.CompanyID == null)
                {
                    company.CompanyID = "";
                }
                ReportResult = reportDsdal.ComapnyProfile(company.CompanyID);
                if (ReportResult.Tables.Count <= 0)
                {
                    //some codes here
                }
                ReportResult.Tables[0].TableName = "DsCompanyProfile";
                RptComapnyProfile objrpt = new RptComapnyProfile();
                objrpt.SetDataSource(ReportResult);
                objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + identity.UserId + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Company Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + identity.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'Program.Address1'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'Program.Address2'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'Program.Address3'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'TelephoneNo'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'FaxNo'";

                var gr = new GenericReport<RptComapnyProfile>();
                var rpt = gr.RenderReportAsPDF(objrpt);
                objrpt.Close();
                return rpt;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult Symphony()
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
            CompanyProfileVM vm = new CompanyProfileVM();


            return PartialView("_PopUpSymphony", vm);
        }

        public ActionResult SuperAdmistator()
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
            LoginVM vm = new LoginVM();


            return PartialView("_SuperAdministator", vm);
        }

        public JsonResult GetSuperAdmistator(string UserName, string Password)
        {
            string SuperLoginPWD;
            var Administrator = "";
            var _repo = new CommonRepo(identity, Session);

            var dt = _repo.SuperAdministrator();
            if (UserName.ToUpper() == "ADMINISTRATOR")
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["user"].ToString() == "zTvrNxNvP08=")
                    {
                        SuperLoginPWD = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, Convert.ToString(row["pwd"]));
                        if (UserName.ToUpper().ToUpper() == "ADMINISTRATOR" &
                                Password.ToUpper() == SuperLoginPWD.ToUpper())
                        {
                            return Json(Administrator = "Yes", JsonRequestBehavior.AllowGet);

                        }
                    }
                }
                return Json(Administrator == "No", JsonRequestBehavior.AllowGet);

            }
            List<LoginVM> CompanyProfileList = new List<LoginVM>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                LoginVM CompanyProfile = new LoginVM();
                CompanyProfile.UserName = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, Convert.ToString(dt.Rows[i]["user"]));
                CompanyProfile.UserPassword = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, Convert.ToString(dt.Rows[i]["pwd"]));
                CompanyProfileList.Add(CompanyProfile);
            }
            return Json(CompanyProfileList, JsonRequestBehavior.AllowGet);
        }


    }
}
