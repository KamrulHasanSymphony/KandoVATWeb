using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using SymOrdinary;
using SymRepository.VMS;
using System.Threading;
using SymVATWebUI.Filters;


namespace SymVATWebUI.Areas.VMS.Controllers
{
    [ShampanAuthorize]
    public class EXPController : Controller
    {


        ShampanIdentity identity = null;

        EXPRepo _repo = null;

        public EXPController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new EXPRepo(identity);

            }
            catch
            {

            }
        }
        //
        // GET: /VMS/TDS/

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param, SalesInvoiceExpVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new EXPRepo(identity, Session);


            #region Search and Filter Data
            var getAllData = _repo.SelectAll(0);
            IEnumerable<SalesInvoiceExpVM> filteredData;
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
                       isSearchable1 && c.LCNumber.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.LCBank.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.LCDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.EXPNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.EXPDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.PINo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.PIDate.ToLower().Contains(param.sSearch.ToLower())
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
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SalesInvoiceExpVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.LCNumber :
                sortColumnIndex == 2 && isSortable_2 ? c.LCBank :
                sortColumnIndex == 3 && isSortable_3 ? c.LCDate :
                sortColumnIndex == 4 && isSortable_4 ? c.EXPNo.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.EXPDate.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.PINo.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.PIDate :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                  c.ID.ToString()
                , c.LCNumber.ToString()
                , c.LCBank.ToString()
                , c.LCDate.ToString()
                , c.EXPNo.ToString()
                , c.EXPDate.ToString()
                , c.PINo.ToString()
                , c.PIDate.ToString()
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

            SalesInvoiceExpVM vm = new SalesInvoiceExpVM();
            vm.Operation = "add";
            //vm.ActiveStatus = "Y";
            return View(vm);



            //return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(SalesInvoiceExpVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new EXPRepo(identity, Session);

            string[] result = new string[6];

            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedOn = DateTime.Now.ToString();
                    vm.CreatedBy = identity.Name;

                    result = _repo.InsertToTDS(vm);

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
                    result = _repo.UpdateToTDSsNew(vm);

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
                //  FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("EXPController", "CreateEdit", ex.ToString());
                return View("Create", vm);
            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new EXPRepo(identity, Session);
            SalesInvoiceExpVM VM = new SalesInvoiceExpVM();

            VM = _repo.SelectAll(id).FirstOrDefault();
            VM.Operation = "update";
            return View("Create", VM);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new EXPRepo(identity, Session);

            SalesInvoiceExpVM vm = new SalesInvoiceExpVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.LastModifiedBy = identity.Name;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

    }
}
