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
    public class SettingMasterController : Controller
    {
        //
        // GET: /VMS/SettingMaster/

        ShampanIdentity identity = null;

        SettingMasterRepo repo = null;
        SettingRepo _repo = null;

        public SettingMasterController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                repo = new SettingMasterRepo(identity);

            }
            catch
            {

            }
        }

        //public ActionResult Index()
        //{
        //    return View();
        //}

        [Authorize(Roles = "Admin")]
        public ActionResult Index(string group)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new SettingRepo(identity, Session);

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

            repo.SettingChangeMaster();

            List<SettingsVM> vms = new List<SettingsVM>();
            try
            {
                vms = repo.SelectAllList();
                if (group != null && group != "AllGroup")
                {
                    ViewBag.groupName = group;
                    var filteredList = vms.Where(m => m.SettingGroup == group).ToList();
                    return View(filteredList);
                }
                ViewBag.groupName = "AllGroup";

                vms = new List<SettingsVM>();
                return View(vms);
            }
            catch (Exception)
            {
                return View(vms);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(SettingsVM vm)
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
                List<SettingsVM> vms = new List<SettingsVM>();
                vm.LastModifiedBy = Identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString();
                vm.ActiveStatus = "Y";
                vms.Add(vm);
                result = new SettingMasterRepo(identity, Session).SettingsUpdatelistMaster(vms);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public JsonResult Update(SettingsVM vm)
        {
            List<SettingsVM> vms = new List<SettingsVM>();

            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.ActiveStatus = "Y";
            vms.Add(vm);
            result = new SettingMasterRepo(identity, Session).SettingsUpdatelistMaster(vms);
            var retResult = result[0] + "~" + result[1];
            return Json(retResult, JsonRequestBehavior.AllowGet);
        }




        public ActionResult DBUpdate()
        {
            ResultVM rVM = new ResultVM();
            rVM = new SettingMasterRepo(identity, Session).DBUpdate("");

            Session["result"] = rVM.Status + "~" + rVM.Message;
            return Redirect("/VMS/Home");
        }

    }
}
