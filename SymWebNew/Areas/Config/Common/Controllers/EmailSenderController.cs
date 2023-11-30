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
    public class EmailSenderController : Controller
    {
        //
        // GET: /Common/EmailSender/

        public ActionResult Index()
        {
            return View();
        }
         [Authorize]
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Column Search
            var IdFilter                = Convert.ToString(Request["sSearch_0"]);
            var SenderEmailFilter         = Convert.ToString(Request["sSearch_1"]);
            var RecepentEmailFilter          = Convert.ToString(Request["sSearch_2"]);
            var EmailSubjectFilter      = Convert.ToString(Request["sSearch_3"]);
            var RecipientNameFilter            = Convert.ToString(Request["sSearch_4"]);
            var PresentDistrictIdFilter     = Convert.ToString(Request["sSearch_5"]);
            var JobCategoryIdFilter            = Convert.ToString(Request["sSearch_6"]);
            var WebsiteFilter            = Convert.ToString(Request["sSearch_7"]);

        //00  //Id
        //01  //SenderEmail
        //02  //RecepentEmail
        //03  //EmailSubject
        //04  //RecipientName
        //05  //PresentDistrictId
        //06  //JobCategoryId
        //07  //Website

            #endregion Column Search

            EmailSenderRepo _repo = new EmailSenderRepo();
            var getAllData = _repo.SelectAll();
            IEnumerable<EmailSenderVM> filteredData;
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
                           isSearchable1 &&  c.SenderEmail        .ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable2 &&  c.RecepentEmail          .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable3 &&  c.EmailSubject         .ToString().ToLower().Contains(param.sSearch.ToLower()) 
                         ||isSearchable5 &&  c.RecipientName           .ToString().ToLower().Contains(param.sSearch.ToLower()) 
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
                  SenderEmailFilter        != "" 
               || RecepentEmailFilter      != "" 
               || EmailSubjectFilter       != "" 
               || RecipientNameFilter      != ""  
               || PresentDistrictIdFilter  != "" 
               || JobCategoryIdFilter      != "" 
               || WebsiteFilter           != ""  
                )
            {
                filteredData = filteredData
                                .Where(c =>
                                    (SenderEmailFilter == "" ||     c.SenderEmail                 .ToLower().Contains(SenderEmailFilter.ToLower()))
                                && (RecepentEmailFilter == "" ||    c.RecepentEmail              .ToString().ToLower().Contains(RecepentEmailFilter.ToLower()))
                                && (EmailSubjectFilter == "" ||     c.EmailSubject              .ToString().ToLower().Contains(EmailSubjectFilter.ToLower()))
                                && (RecipientNameFilter == "" ||    c.RecipientName          .ToString().ToLower().Contains(RecipientNameFilter.ToLower()))
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
            Func<EmailSenderVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ?   c.SenderEmail                          :
                sortColumnIndex == 2 && isSortable_2 ?   c.RecepentEmail                        :
                sortColumnIndex == 3 && isSortable_3 ?   c.EmailSubject                         :
                sortColumnIndex == 4 && isSortable_4 ?   c.RecipientName                        :
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
                , c.SenderEmail                 
                , c.RecepentEmail               
                , c.EmailSubject                
                , c.RecipientName               
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
