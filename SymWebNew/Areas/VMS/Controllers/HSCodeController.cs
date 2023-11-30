using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VATViewModel.DTOs;
using SymRepository.VMS;
using SymOrdinary;
using System.Threading;
using System.Data;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.VMS.Controllers
{
    [ShampanAuthorize]
    public class HSCodeController : Controller
    {
        ShampanIdentity identity = null;

        HSCodeRepo _repo = null;

        public HSCodeController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new HSCodeRepo(identity);

            }
            catch
            {

            }
        }
        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //HSCodeRepo _repo = new HSCodeRepo();

        //
        // GET: /VMS/HSCode/

        public ActionResult Index(HSCodeVM paramVM)
        {
            return View(paramVM);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param, HSCodeVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new HSCodeRepo(identity, Session);

            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "100";
            }
            #region Search and Filter Data
            string[] cFields;
            string[] cValues;

            cFields = new string[] { "SelectTop" };
            cValues = new string[] { paramVM.SelectTop };

            if (!string.IsNullOrWhiteSpace(paramVM.SearchField))
            {
                //////cFields.ToList().Add("Pr." + paramVM.SearchField + " like");
                //////cValues.ToList().Add(paramVM.SearchValue);

                var tempFields = cFields.ToList();
                tempFields.Add(paramVM.SearchField + " like");
                cFields = tempFields.ToArray();

                var tempValues = cValues.ToList();
                tempValues.Add(paramVM.SearchValue);
                cValues = tempValues.ToArray();

            }

            if (!string.IsNullOrWhiteSpace(paramVM.FiscalYear))
            {
                var tempFields = cFields.ToList();
                tempFields.Add("FiscalYear");
                cFields = tempFields.ToArray();

                var tempValues = cValues.ToList();
                tempValues.Add(paramVM.FiscalYear);
                cValues = tempValues.ToArray();

            }
            else
            {
                string PeriodName = DateTime.Now.AddMonths(-1).ToString("MMMM-yyyy");
                FiscalYearRepo fiscalYearRepo = new FiscalYearRepo(identity, Session);

                var Fiscalresult = fiscalYearRepo.SelectAll(0, new[] { "PeriodName" }, new[] { PeriodName }).FirstOrDefault();
                var tempFields = cFields.ToList();
                tempFields.Add("FiscalYear");
                cFields = tempFields.ToArray();

                var tempValues = cValues.ToList();
                tempValues.Add(Fiscalresult.CurrentYear);
                cValues = tempValues.ToArray();
            }
            var getAllData = _repo.SelectAll(0, cFields, cValues);
            IEnumerable<HSCodeVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.HSCode.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.SD.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.VAT.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.OtherVAT.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.IsFixedVAT.ToString().ToLower().Contains(param.sSearch.ToLower())

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
            Func<HSCodeVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.HSCode :
                sortColumnIndex == 2 && isSortable_2 ? c.SD.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.VAT.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.OtherVAT.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.IsFixedVAT.ToString() :

                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id.ToString()
                , c.HSCode.ToString()
                , c.SD.ToString()
                , c.VAT.ToString()
                , c.OtherVAT.ToString()
                , c.IsFixedVAT.ToString()
                
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


        public ActionResult Create()
        {

            HSCodeVM vm = new HSCodeVM();

            vm.IsFixedOtherSD = "N";
            vm.IsFixedOtherVAT = "N";
            vm.IsVDS = "N";
            vm.Operation = "add";
            vm.FiscalYear = DateTime.Now.ToString("yyyy");
            return View(vm);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(HSCodeVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new HSCodeRepo(identity, Session);


            string[] result = new string[6];

            vm.IsFixedSD = vm.IsFixedSDChecked ? "Y" : "N";
            vm.IsFixedCD = vm.IsFixedCDChecked ? "Y" : "N";
            vm.IsFixedRD = vm.IsFixedRDChecked ? "Y" : "N";
            vm.IsFixedAIT = vm.IsFixedAITChecked ? "Y" : "N";
            vm.IsFixedVAT = vm.IsFixedVAT1Checked ? "Y" : "N";
            vm.IsFixedAT = vm.IsFixedATChecked ? "Y" : "N";


            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedOn = DateTime.Now.ToString();
                    vm.CreatedBy = identity.Name;


                    result = _repo.InsertToHSCode(vm);

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
                    vm.IsFixedSD = vm.IsFixedSDChecked ? "Y" : "N";
                    result = _repo.UpdateToHSCode(vm);

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
                FileLogger.Log("HSCodeController", "CreateEdit", ex.ToString());
                return View("Create", vm);
            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new HSCodeRepo(identity, Session);

            HSCodeVM vm = new HSCodeVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.LastModifiedBy = identity.Name;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new HSCodeRepo(identity, Session);

            HSCodeVM VM = new HSCodeVM();

            int ids = Convert.ToInt32(id);

            VM = _repo.SelectAll(ids).FirstOrDefault();
            VM.Operation = "update";
            VM.IsFixedSDChecked = VM.IsFixedSD == "Y";
            VM.IsFixedCDChecked = VM.IsFixedCD == "Y";
            VM.IsFixedRDChecked = VM.IsFixedRD == "Y";
            VM.IsFixedAITChecked = VM.IsFixedAIT == "Y";
            VM.IsFixedVAT1Checked = VM.IsFixedVAT == "Y";
            VM.IsFixedATChecked = VM.IsFixedAT == "Y";

            return View("Create", VM);
        }
        [Authorize]
        [HttpGet]
        public ActionResult GetHSCodeNoPopUp()
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

            HSCodeVM vm = new HSCodeVM();

            return PartialView("_hscodeSearch", vm);

        }


        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredHsCode(HSCodeVM vm)
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
            //////DataTable HScodeResult = new DataTable();

            //  HScodeResult = _repo.SearchCustomerAddress(vm.CustomerID);
            if (string.IsNullOrWhiteSpace(vm.SelectTop))
            {
                vm.SelectTop = "100";
            }
            List<HSCodeVM> VMS = new List<HSCodeVM>();
            #region Search and Filter Data
            string[] cFields;
            string[] cValues;

            cFields = new string[] { "SelectTop" };
            cValues = new string[] { vm.SelectTop };

            if (!string.IsNullOrWhiteSpace(vm.SearchField))
            {
                //////cFields.ToList().Add("Pr." + paramVM.SearchField + " like");
                //////cValues.ToList().Add(paramVM.SearchValue);

                var tempFields = cFields.ToList();
                tempFields.Add(vm.SearchField + " like");
                cFields = tempFields.ToArray();

                var tempValues = cValues.ToList();
                tempValues.Add(vm.SearchValue);
                cValues = tempValues.ToArray();

            }

            if (!string.IsNullOrWhiteSpace(vm.FiscalYear))
            {
                var tempFields = cFields.ToList();
                tempFields.Add("FiscalYear");
                cFields = tempFields.ToArray();

                var tempValues = cValues.ToList();
                tempValues.Add(vm.FiscalYear);
                cValues = tempValues.ToArray();

            }
            else
            {
                string PeriodName = DateTime.Now.AddMonths(-1).ToString("MMMM-yyyy");
                FiscalYearRepo fiscalYearRepo = new FiscalYearRepo(identity, Session);

                var Fiscalresult = fiscalYearRepo.SelectAll(0, new[] { "PeriodName" }, new[] { PeriodName }).FirstOrDefault();
                var tempFields = cFields.ToList();
                tempFields.Add("FiscalYear");
                cFields = tempFields.ToArray();

                var tempValues = cValues.ToList();
                tempValues.Add(Fiscalresult.CurrentYear);
                cValues = tempValues.ToArray();
            }
            VMS = _repo.SelectAll(0, cFields, cValues);
            #endregion
            //VMS = _repo.SelectAll();


            //////foreach (DataRow row in HScodeResult.Rows)
            //////{
            //////    HSCodeVM hscode = new HSCodeVM();

            //////    hscode.CustomerID = row["CustomerID"].ToString();
            //////    Customer.Id = Convert.ToInt32(row["Id"].ToString());
            //////    Customer.CustomerAddress = row["CustomerAddress"].ToString();

            //////    Customer.CustomerCode = row["CustomerCode"].ToString();
            //////    Customer.CustomerName = row["CustomerName"].ToString();
            //////    Customer.CustomerGroupName = row["CustomerGroupName"].ToString();
            //////    Customer.CustomerVATRegNo = row["CustomerVATRegNo"].ToString();

            //////    VMS.Add(hscode);
            //////}

            return PartialView("_filteredHscode", VMS);
        }


    }
}
