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
    public class PublicApplicantController : Controller
    {
        //
        // GET: /Common/PublicApplicant/
         SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
           PublicApplicantRepo _PArepo = new PublicApplicantRepo();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _index(JQueryDataTableParamVM param)
        {
             #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var JopTitleFilter = Convert.ToString(Request["sSearch_1"]);
            var DesignationNameFilter = Convert.ToString(Request["sSearch_2"]);
            var ApplicantNameFilter = Convert.ToString(Request["sSearch_3"]);
            var ApplicantEmailFilter = Convert.ToString(Request["sSearch_3"]);
            var ExprianceFilter = Convert.ToString(Request["sSearch_3"]);
            var CVIdentificationNoFilter = Convert.ToString(Request["sSearch_3"]);
            #endregion Column Search

            var getAllData = _PArepo.SelectAll();
            IEnumerable<PublicApplicantVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Id.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.JobTitle.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.DesignationName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.ApplicantName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.ApplicantEmail.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Expriance.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable7 && c.CVIdentificationNo.ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredData = getAllData;
            }
            //#region Column Filtering
            if (JopTitleFilter != "" || DesignationNameFilter != "" || ApplicantNameFilter != "" || ApplicantEmailFilter != "" || ExprianceFilter != "" || CVIdentificationNoFilter != "")   
            {
                filteredData = filteredData
                                .Where(c =>
                                           (JopTitleFilter == "" || c.JobTitle.ToLower().Contains(JopTitleFilter.ToLower()))
                                            && (DesignationNameFilter == "" || c.DesignationName.ToString().ToLower().Contains(DesignationNameFilter.ToLower()))
                                            && (ApplicantNameFilter == "" || c.ApplicantName.ToString().ToLower().Contains(ApplicantNameFilter.ToLower()))
                                            && (ApplicantEmailFilter == "" || c.ApplicantEmail.ToString().ToLower().Contains(ApplicantEmailFilter.ToLower()))
                                            && (ExprianceFilter == "" || c.Expriance.ToString().ToLower().Contains(ExprianceFilter.ToLower()))
                                            && (CVIdentificationNoFilter == "" || c.CVIdentificationNo.ToString().ToLower().Contains(CVIdentificationNoFilter.ToLower()))
                                           );
            }

            //#endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PublicApplicantVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Id :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.JobTitle :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.DesignationName :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.ApplicantName :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.ApplicantEmail :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Expriance :
                                                           sortColumnIndex == 7 && isSortable_7 ? c.CVIdentificationNo :
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
                             , c.JobTitle
                             , c.DesignationName
                             , c.ApplicantName
                             , c.ApplicantEmail
                             , c.Expriance
                             , c.CVIdentificationNo
                       
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
        public ActionResult Edit(string id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_17", "edit").ToString();
            return PartialView(_PArepo.SelectById(id));
        }

        [HttpPost]
        public ActionResult Edit(PublicApplicantVM vm, string btn)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {
                
                result = _PArepo.Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Updated";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }

    }
}
