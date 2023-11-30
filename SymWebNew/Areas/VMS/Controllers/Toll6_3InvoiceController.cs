using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using SymOrdinary;
using SymRepository.VMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using VATViewModel.DTOs;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.VMS.Controllers
{
    [ShampanAuthorize]
    public class Toll6_3InvoiceController : Controller
    {
        ShampanIdentity identity = null;
        Toll6_3InvoiceRepo _repo = null;

        public Toll6_3InvoiceController()
        {
            try
            {

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new Toll6_3InvoiceRepo(identity);
            }
            catch
            {

            }
        }

        //
        // GET: /VMS/Toll6_3Invoice/
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string TransactionType)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/vms/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/vms/Home");
            }
            Toll6_3InvoiceVM tollVM = new Toll6_3InvoiceVM();
            tollVM.TransactionType = TransactionType;

            return View(tollVM);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new Toll6_3InvoiceRepo(identity, Session);

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/vms/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/vms/Home");
            }
            //00     //TollID 
            //01     //Toll No
            //02     //Customer
            //03     //Address
            //04     //	Toll Date

            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<Toll6_3InvoiceVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.TollNo.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.CustomerID.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Address.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.TollDateTime.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Post.ToLower().Contains(param.sSearch.ToLower())
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
            Func<Toll6_3InvoiceVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.TollNo :
                sortColumnIndex == 2 && isSortable_2 ? c.CustomerName :
                sortColumnIndex == 3 && isSortable_3 ? c.Address :
                sortColumnIndex == 4 && isSortable_4 ? c.TollDateTime :
                sortColumnIndex == 5 && isSortable_5 ? c.Post :
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
                , c.TollNo
                , c.CustomerName
                , c.Address
                ,Convert.ToDateTime(c.TollDateTime).ToString("yyyy-MM-dd")
                , c.Post
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
        public ActionResult Create(string TransactionType)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/vms/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/vms/Home");
            }
            Toll6_3InvoiceVM vm = new Toll6_3InvoiceVM();
            vm.Operation = "add";
            vm.ActiveStatus = "Y";
            vm.TransactionType = TransactionType;
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(Toll6_3InvoiceVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new Toll6_3InvoiceRepo(identity, Session);

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/vms/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/vms/Home");
            }
            string[] result = new string[6];

            vm.BranchId = Convert.ToInt32(Session["BranchId"]);

            vm.CustomerID = vm.CustomerID.Trim();

            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedOn = DateTime.Now.ToString();
                    vm.CreatedBy = identity.Name;
                    //vm.ActiveStatus = true;
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    result = _repo.InsertToToll6_3Invoice(vm, vm.Details);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { invNumber = result[2] });
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

                    foreach (var data in vm.Details)
                    {
                        data.TollNo = vm.TollNo;
                    }

                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1].Replace("\n", "").Replace("\r", "");
                    return RedirectToAction("Edit", new { invNumber = result[2] });
                }
                else
                {
                    return View("Create", vm);
                }
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                //  Session["result"] = "Fail~Data Not Succeessfully!";
                // FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("Toll6_3InvoiceController", "CreateEdit", e.ToString());
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string invNumber)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/vms/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/vms/Home");
            }
            Toll6_3InvoiceVM vm = new Toll6_3InvoiceVM();


            string[] cFields = { "ti.TollNo" };
            string[] cValues = { invNumber };
            //Convert.ToInt32(invNumber)
            vm = _repo.SelectAll(0, cFields, cValues, null, null).FirstOrDefault();

            vm.Details = _repo.SelectDetail(invNumber);

            vm.Operation = "update";
            return View("Create", vm);
        }


        public ActionResult GetItemPopUp()
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
            PopUpViewModel vm = new PopUpViewModel();

            return PartialView("_Toll6_3InvoiceSearchPopup", vm);
        }

        [HttpPost]
        public ActionResult BlankItem(string[] ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

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

            //string[] ids = JsonConvert.DeserializeObject<string[]>(json);

            SaleInvoiceRepo _rep = new SaleInvoiceRepo(identity, Session);

            List<SaleMasterVM> vms = _rep.SelectAll(0, null, null, null, null, null, null, "Y", ids);

            List<Toll6_3InvoiceDetailVM> Toll6_3VMS = new List<Toll6_3InvoiceDetailVM>();

            int i = 1;

            foreach (SaleMasterVM vmD in vms)
            {

                Toll6_3InvoiceDetailVM toll6_3details = new Toll6_3InvoiceDetailVM();

                #region Adding Line No
                //vmD.SaleInvoiceNumber = i.ToString();

                toll6_3details.TollLineNo = i.ToString();
                toll6_3details.SalesInvoiceNo = vmD.SalesInvoiceNo;
                toll6_3details.InvoiceDateTime = vmD.InvoiceDateTime;

                Toll6_3VMS.Add(toll6_3details);
                //vmD.SalesInvoiceNo
                i++;

                #endregion
            }

            return PartialView("_detail", Toll6_3VMS);
        }


        [HttpGet]
        public ActionResult GetFilteredItems(Toll6_3InvoiceVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new Toll6_3InvoiceRepo(identity, Session);
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


            string dtFrom = null;
            string dtTo = null;

            dtFrom = Convert.ToDateTime(vm.TollDateFrom).ToString("yyyyMMdd");
            dtTo = Convert.ToDateTime(vm.TollDateTo).AddDays(1).ToString("yyyyMMdd");
            vm.Post = "Y";


            string[] cFields = { "sih.SalesInvoiceNo like",
                                      "c.CustomerName like",                                      
                                      "sih.SerialNo like",
                                      "sih.InvoiceDateTime>=",
                                      "sih.InvoiceDateTime<=",                                     
                                      "sih.IsPrint like",
                                      "sih.post like",
                                      "sih.EXPFormNo like",                                      
                                     };


            string[] cValues = { vm.TollNo, vm.CustomerName, vm.RefNo, dtFrom, dtTo, vm.IsPrint, vm.Post, vm.EXPFormNo };



            string transactionType = "TollFinishIssue";
            string Is6_3TollCompleted = "N";

            SaleInvoiceRepo _repoSale = new SaleInvoiceRepo(identity, Session);
            var list = _repoSale.SelectAll(0, cFields, cValues, null, null, null, transactionType, "Y", null, Is6_3TollCompleted);

            return PartialView("_filteredTollInvoice", list);
        }



        //[Authorize(Roles = "Admin")]
        //public ActionResult Delete(string ids)
        //{
        //    identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //    _repo = new CurrencyRepo(identity);

        //    string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
        //    if (project.ToLower() == "vms")
        //    {
        //        if (!identity.IsAdmin)
        //        {
        //            Session["rollPermission"] = "deny";
        //            return Redirect("/vms/Home");
        //        }
        //    }
        //    else
        //    {
        //        Session["rollPermission"] = "deny";
        //        return Redirect("/vms/Home");
        //    }
        //    CurrencyVM vm = new CurrencyVM();
        //    string[] a = ids.Split('~');
        //    string[] result = new string[6];
        //    vm.LastModifiedOn = DateTime.Now.ToString();
        //    vm.LastModifiedBy = identity.Name;
        //    result = _repo.Delete(vm, a);
        //    return Json(result[1], JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Navigate(string id, string btn)
        {
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetId("Toll6_3Invoice", "Id", id, btn);
            return RedirectToAction("Edit", new { id = targetId });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new Toll6_3InvoiceRepo(identity, Session);
            string[] a = ids.Split('~');
            var id = a[0];
            ResultVM rVM = new ResultVM();
            Toll6_3InvoiceVM vm = new Toll6_3InvoiceVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";

            result = _repo.Toll6_3InvoicePost(vm);

            if (result[0].ToLower() == "success")
            {
                _repo.UpdateTollCompleted("Y", vm.TollNo);
            }

            //result[1] = rVM.Message;

            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

    }
}
