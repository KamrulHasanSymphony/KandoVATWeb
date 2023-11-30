using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VATViewModel.DTOs;
using SymRepository.VMS;
using SymOrdinary;
using System.Threading;


namespace SymVATWebUI.Areas.VMS.Controllers
{
    public class ShiftController : Controller

    {
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        ShiftRepo _repo = new ShiftRepo();

        //
        // GET: /VMS/HSCode/

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param, ShiftVM paramVM)
        {

            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<ShiftVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Sl.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.ShiftName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.ShiftStart.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.ShiftEnd.ToString().ToLower().Contains(param.sSearch.ToLower())

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


            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ShiftVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Sl :
                sortColumnIndex == 2 && isSortable_2 ? c.ShiftName.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.ShiftStart.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.ShiftEnd.ToString() :

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
                , c.Sl.ToString()
                , c.ShiftName.ToString()
                , c.ShiftStart.ToString()
                , c.ShiftEnd.ToString()
                
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

            ShiftVM vm = new ShiftVM();

            vm.Operation = "add";
            return View(vm);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(ShiftVM vm)
        {
            

            string[] result = new string[6];       

            try
            {
                if (vm.Operation.ToLower() == "add")
                {



                    result = _repo.InsertToShiftNew(vm);

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
                    result = _repo.UpdateToShiftNew(vm);

                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = result[2] });
                }
                else
                {
                    return View("Create", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("Create", vm);
            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            string[] ID=ids.Split('~');
            string[] result = new string[6];
            string Id =ID[0];
            result = _repo.Delete(Id);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            ShiftVM VM = new ShiftVM();

            int ids = Convert.ToInt32(id);

            VM = _repo.SelectAll(ids).FirstOrDefault();
            VM.Operation = "update";
           

            return View("Create", VM);
        }




    }
}
