//using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.VMS;
using VATViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class FiscalYearController : Controller
    {
        //
        // GET: /VMS/Branch/

        // 
        ShampanIdentity identity = null;

        FiscalYearRepo _repo = null;

        public FiscalYearController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new FiscalYearRepo(identity);

            }
            catch
            {

            }
        }
        // FiscalYearRepo _repo = new FiscalYearRepo();
        // ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string year)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new FiscalYearRepo(identity, Session);

            ViewBag.Operation = "Update";
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
            List<FiscalYearVM> vms = new List<FiscalYearVM>();
            if (year == null)
            {
                return View(vms);
            }
            try
            {
                vms = _repo.SelectAll();
                if (year != null && year != "All")
                {
                    var filteredList = vms.Where(m => m.CurrentYear == year).ToList();
                    ViewBag.groupName = year;
                    return View(filteredList);
                }
                ViewBag.groupName = "All";
                return View(vms);
            }
            catch (Exception)
            {
                return View(vms);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(List<FiscalYearVM> VMs)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new FiscalYearRepo(identity, Session);

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
                ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                var modifiedBy = Identity.Name;

                List<FiscalYearVM> fVMS = new List<FiscalYearVM>();

                foreach (var item in VMs)
                {
                    FiscalYearVM fVM = new FiscalYearVM();
                    fVM.CurrentYear = item.CurrentYear;
                    fVM.FiscalYearName = item.FiscalYearName;
                    fVM.PeriodEnd = item.PeriodEnd;
                    fVM.PeriodID = item.PeriodID;
                    fVM.PeriodLock = item.PeriodLock;
                    fVM.PeriodName = item.PeriodName;
                    fVM.GLLock = item.PeriodLock;
                    fVM.PeriodStart = item.PeriodStart;
                    fVM.VATReturnPost = item.VATReturnPost;
                    fVM.CreatedBy = item.CreatedBy;
                    fVM.CreatedOn = item.CreatedOn;
                    fVM.LastModifiedBy = item.LastModifiedBy;
                    fVM.LastModifiedOn = item.LastModifiedOn;

                    fVMS.Add(fVM);


                }

                result = new FiscalYearRepo(identity, Session).FiscalYearUpdate(fVMS, modifiedBy);
                //////result = new FiscalYearRepo(identity, Session).FiscalYearUpdate(VMs, modifiedBy);
                Session["result"] = result[0] + "~" + result[1];
                var year = VMs[0].CurrentYear;
                List<FiscalYearVM> vms = _repo.SelectAll();
                if (year != "All")
                {
                    var filteredList = vms.Where(m => m.CurrentYear == year).ToList();
                    ViewBag.groupName = year;
                    return View("Index", filteredList);
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }

        }

        [Authorize(Roles = "Admin")]
        public ActionResult CreateFiscalYear(List<FiscalYearVM> VMs)
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
            string[] result = new string[6];
            try
            {
                ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                result = new FiscalYearRepo(identity, Session).FiscalYearInsert(VMs);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                //Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }

        }




        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new FiscalYearRepo(identity, Session);

            ViewBag.Operation = "Add";
            var unlocked = _repo.LockChek();
            if (unlocked > 0)
            {
                Session["result"] = "Fail" + "~" + "Lock all previous years first";
                return RedirectToAction("Index");
            }
            string startDate = "";
            var yearList = _repo.SelectAll();
            if (yearList.Count > 0)
            {
                var maxDate = _repo.MaxDate();
                startDate = DateTime.Parse(maxDate).AddDays(1).ToString("yyyy-MM-dd");
                List<FiscalYearVM> VMs = DesignFiscalYear(startDate);
                return View("Index", VMs);
            }
            return RedirectToAction("Index");
        }

        private List<FiscalYearVM> DesignFiscalYear(string startDate)
        {
            var VMs = new List<FiscalYearVM>();

            var date = Ordinary.DateToString(startDate);

            DateTime start_date = new DateTime(Convert.ToInt32(date.Substring(0, 4)), Convert.ToInt32(date.Substring(4, 2)), Convert.ToInt32(date.Substring(6, 2)));
            var currentYear = start_date.AddYears(1).ToString("yyyy");

            DateTime yearEnd = (Convert.ToDateTime(Convert.ToDateTime(startDate).ToString("dd/MMM/yyyy")).AddYears(+1)).AddDays(-1);
            DateTime yearStart = Convert.ToDateTime(Convert.ToDateTime(startDate).ToString("dd/MMM/yyyy"));

            string FiscalYearName = yearStart.ToString("dd/MMM/yyyy") + " To " + yearEnd.ToString("dd/MMM/yyyy");

            FiscalYearVM vm;
            for (int i = 0; i < 12; i++)
            {
                vm = new FiscalYearVM();
                // vm.PeriodName = start_date.AddMonths(i).ToString("MMM-yy"); // start_date.AddMonths(i).ToString("MMMM") + "-" + vm.Year;

                ////vm.FiscalYearName = vm.PeriodStart + "To" + vm.PeriodEnd;
                vm.FiscalYearName = FiscalYearName;

                vm.PeriodName = start_date.AddMonths(i).ToString("MMMM-yyyy"); // start_date.AddMonths(i).ToString("MMMM") + "-" + vm.Year;
                vm.PeriodStart = start_date.AddMonths(i).ToString("dd-MMM-yyyy");
                vm.PeriodEnd = start_date.AddMonths(i + 1).AddDays(-1).ToString("dd-MMM-yyyy");
                ////vm.PeriodID = (i + 1).ToString("D2") + currentYear;
                vm.PeriodID = Convert.ToDateTime(vm.PeriodStart).ToString("MMyyyy");
                vm.PeriodLock = "N";
                vm.GLLock = "N";
                vm.CurrentYear = currentYear;
                vm.CreatedBy = identity.Name;
                vm.CreatedOn = DateTime.Now.ToString();
                VMs.Add(vm);
            }
            //vm.glFiscalYearDetailVMs = fvms;
            //vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //vm.CreatedBy = identity.Name;
            //vm.CreatedFrom = identity.WorkStationIP;
            //vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //vm.LastUpdateBy = identity.Name;
            //vm.LastUpdateFrom = identity.WorkStationIP;
            //vm.BranchId = Convert.ToInt32(identity.BranchId);
            return VMs;
        }

    }
}
