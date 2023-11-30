using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymVATWebUI.Areas.Common.Controllers
{
    public class FiscalYearController : Controller
    {
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_13", "index").ToString();
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var yearFilter = Convert.ToString(Request["sSearch_1"]);
            var yearStartFilter = Convert.ToString(Request["sSearch_2"]);
            var yearEndFilter = Convert.ToString(Request["sSearch_3"]);

            //Year
            //YearStart
            //YearEnd 



            var fromID = 0;
            var toID = 0;
            if (idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }
            #endregion Column Search

            #region Search and Filter Data


            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var getAllData = new FiscalYearRepo().SelectAll(Convert.ToInt32(identity.BranchId));
            IEnumerable<FiscalYearVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Year.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.YearStart.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.YearEnd.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable3 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }


            #endregion Search and Filter Data

            #region Column Filtering
            if (yearFilter != "" || yearStartFilter == "" || yearEndFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (yearFilter == "" || c.Year.ToString().ToLower().Contains(yearFilter.ToLower()))
                                            &&
                                            (yearStartFilter == "" || c.YearStart.ToLower().Contains(yearStartFilter.ToLower()))
                                            &&
                                            (yearEndFilter == "" || c.YearEnd.Contains(yearEndFilter.ToLower()))

                                        );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<FiscalYearVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Year.ToString() :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.YearStart :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.YearEnd :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Remarks :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                Convert.ToString(c.Id)
                , c.Year.ToString() //+ "~" + Convert.ToString(c.Id)
                , c.YearStart
                , c.YearEnd 
                //, c.Remarks 
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
        public ActionResult Create()
        {

            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_13", "add").ToString();

            FiscalYearRepo fiscalYearRepo = new FiscalYearRepo();
            List<FiscalYearVM> fiscalYearLists = new List<FiscalYearVM>();
            fiscalYearLists = fiscalYearRepo.SelectAll(Convert.ToInt32(identity.BranchId));
            string yearStartDate = "";
            if (fiscalYearLists.Count > 0)
            {
                yearStartDate = DateTime.Parse(fiscalYearLists.LastOrDefault().YearEnd).AddDays(1).ToString("dd-MMM-yyyy");
                ViewBag.YearStart = "disabled";
            }
            else
            {
                CompanyRepo compRepo = new CompanyRepo();
                CompanyVM company = compRepo.SelectById(Convert.ToInt32(identity.CompanyId));

                DateTime newDate = Convert.ToDateTime(Ordinary.StringToDate(company.YearStart));
                yearStartDate = newDate.ToString("dd-MMM-yyyy");
                ViewBag.YearStart = "";
            }

            FiscalYearVM vm = new FiscalYearVM();
            List<FiscalYearDetailVM> dvms = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM dvm;
            for (int i = 1; i < 13; i++)
            {
                dvm = new FiscalYearDetailVM();

                dvms.Add(dvm);
            }
            vm.FiscalYearDetailVM = dvms;
            vm.YearStart = yearStartDate;
            vm = DesignFiscalYear(vm);
            return View(vm);
        }
        [HttpPost]
        public ActionResult Create(FiscalYearVM vm)
        {
            string[] result = new string[6];
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            FiscalYearRepo fiscalYearRepo = new FiscalYearRepo();
            //FiscalYearVM fiscalYearList = fiscalYearRepo.SelectAll(Convert.ToInt32(identity.BranchId)).LastOrDefault();
            //if (fiscalYearList!=null)
            //{
            //    vm.YearStart = DateTime.Parse(fiscalYearList.YearEnd).AddDays(1).ToString("dd-MMM-yyyy");
            //}
            //else
            //{
            //    //var values = Ordinary.DateToString(vm.YearStart);
            //    vm.YearStart = Ordinary.DateToString(vm.YearStart);// new DateTime(Convert.ToInt32(values.Substring(0, 4)), Convert.ToInt32(values.Substring(4, 2)), 01).ToString("dd-MMM-yyyy");
            //}

            FiscalYearVM newVM = DesignFiscalYear(vm);
            try
            {

                result = new FiscalYearRepo().FiscalYearInsert(newVM);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        private FiscalYearVM DesignFiscalYear(FiscalYearVM vm)
        {
            var date = Ordinary.DateToString(vm.YearStart);
            DateTime start_date = new DateTime(Convert.ToInt32(date.Substring(0, 4)), Convert.ToInt32(date.Substring(4, 2)), Convert.ToInt32(date.Substring(6, 2)));
            vm.YearEnd = start_date.AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");
            vm.Year = Convert.ToInt32(start_date.AddYears(1).AddDays(-1).ToString("yyyy"));

            List<FiscalYearDetailVM> fvms = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM fvm = new FiscalYearDetailVM();
            for (int i = 0; i < 12; i++)
            {
                fvm = new FiscalYearDetailVM();
                fvm.PeriodName = start_date.AddMonths(i).ToString("MMM-yy"); // start_date.AddMonths(i).ToString("MMMM") + "-" + vm.Year;
                fvm.PeriodStart = start_date.AddMonths(i).ToString("dd-MMM-yyyy");
                fvm.PeriodEnd = start_date.AddMonths(i + 1).AddDays(-1).ToString("dd-MMM-yyyy");
                fvms.Add(fvm);
            }
            //foreach (var item in vm.FiscalYearDetailVM)
            //{
            //    item.PeriodName = start_date.AddMonths(i).ToString("MMMM-yyyy"); // start_date.AddMonths(i).ToString("MMMM") + "-" + vm.Year;
            //    item.PeriodStart = start_date.AddMonths(i).ToString("dd-MMM-yyyy");
            //    item.PeriodEnd = start_date.AddMonths(i + 1).AddDays(-1).ToString("dd-MMM-yyyy");
            //    i++;
            //}
            vm.FiscalYearDetailVM = fvms;
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
            return vm;
        }
        [HttpGet]
        public ActionResult Edit(string Id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_13", "edit").ToString();
            return View(new FiscalYearRepo().SelectById(Id));
        }
        [HttpPost]
        public ActionResult Edit(FiscalYearVM vm)
        {
            string[] result = new string[6];
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                vm.BranchId = Convert.ToInt32(identity.BranchId);
                result = new FiscalYearRepo().FiscalYearUpdate(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
        public JsonResult Year(string term)
        {
            return Json(new FiscalYearRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }
    }
}
