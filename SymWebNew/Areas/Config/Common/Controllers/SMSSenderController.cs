using SymViewModel.Common;
using SymRepository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JQueryDataTables.Models;
using SymOrdinary;


namespace SymVATWebUI.Areas.Common.Controllers
{
    public class SMSSenderController : Controller
    {
        //
        // GET: /Common/SMSSender/

        public ActionResult Index()
        {
            return View();
        }
            [Authorize]
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Column Search
            var IdFilter                = Convert.ToString(Request["sSearch_0"]);
            var SenderMobileNoFilter         = Convert.ToString(Request["sSearch_1"]);
            var RecepentMobileNoFilter          = Convert.ToString(Request["sSearch_2"]);
            var SMSBodyFilter      = Convert.ToString(Request["sSearch_3"]);
            var FullNameFilter            = Convert.ToString(Request["sSearch_4"]);
            var PresentDistrictIdFilter     = Convert.ToString(Request["sSearch_5"]);
            var JobCategoryIdFilter            = Convert.ToString(Request["sSearch_6"]);
            var WebsiteFilter            = Convert.ToString(Request["sSearch_7"]);

        //00  //Id
        //01  //SenderMobileNo
        //02  //RecepentMobileNo
        //03  //SMSBody
        //04  //FullName 
        //05  //PresentDistrictId
        //06  //JobCategoryId
        //07  //Website

            #endregion Column Search

            SMSSenderRepo _repo = new SMSSenderRepo();
            var getAllData = _repo.SelectAll();
            IEnumerable<SMSSenderVM> filteredData;
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

                filteredData = getAllData
                   .Where(c =>
                           isSearchable1 &&  c.SenderMobileNo      .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable2 &&  c.RecepentMobileNo          .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable3 &&  c.SMSBody              .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable5 &&  c.FullName            .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable4 &&  c.PresentDistrictId          .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable6 &&  c.JobCategoryId              .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable7 &&  c.Website          .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            if (
                  SenderMobileNoFilter        != "" 
               || RecepentMobileNoFilter      != "" 
               || SMSBodyFilter       != "" 
               || FullNameFilter      != ""  
               || PresentDistrictIdFilter  != "" 
               || JobCategoryIdFilter      != "" 
               || WebsiteFilter           != ""  
                )
            {
                filteredData = filteredData
                                .Where(c =>
                                    (SenderMobileNoFilter == "" ||      c.SenderMobileNo               .ToLower().Contains(SenderMobileNoFilter.ToLower()))
                                && (RecepentMobileNoFilter == "" ||     c.RecepentMobileNo              .ToString().ToLower().Contains(RecepentMobileNoFilter.ToLower()))
                                && (SMSBodyFilter == "" ||              c.SMSBody             .ToString().ToLower().Contains(SMSBodyFilter.ToLower()))
                                && (FullNameFilter == "" ||             c.FullName           .ToString().ToLower().Contains(FullNameFilter.ToLower()))
                                && (PresentDistrictIdFilter == "" ||c.PresentDistrictId      .ToString().ToLower().Contains(PresentDistrictIdFilter.ToLower()))
                                && (JobCategoryIdFilter == "" ||    c.JobCategoryId             .ToString().ToLower().Contains(JobCategoryIdFilter.ToLower()))
                                && (WebsiteFilter == "" ||          c.Website                   .ToLower().Contains(JobCategoryIdFilter.ToLower()))
                                );
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SMSSenderVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ?   c.SenderMobileNo                        :
                sortColumnIndex == 2 && isSortable_2 ?   c.RecepentMobileNo                        :
                sortColumnIndex == 3 && isSortable_3 ?   c.SMSBody                        :
                sortColumnIndex == 4 && isSortable_4 ?   c.FullName                         :
                sortColumnIndex == 5 && isSortable_5 ?   c.PresentDistrictId.ToString()         :
                sortColumnIndex == 6 && isSortable_6 ?   c.JobCategoryId.ToString()             :
                sortColumnIndex == 7 && isSortable_7 ?   c.Website                              :
            "");                                     
                                                     
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] 
            { 
                  c.Id.ToString()
                , c.SenderMobileNo               
                , c.RecepentMobileNo               
                , c.SMSBody               
                , c.FullName                
                , c.PresentDistrictId.ToString()
                , c.JobCategoryId.ToString()    
                , c.Website                     
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

    }
}
