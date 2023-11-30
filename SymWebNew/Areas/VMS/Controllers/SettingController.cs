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
    public class SettingController : Controller
    {
        //
        // GET: /VMS/Branch/

        // 
         ShampanIdentity identity = null;

         SettingRepo _repo = null;

        public SettingController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new SettingRepo(identity);

            }
            catch 
            {
                
            }
        }
       // SettingRepo _repo = new SettingRepo();
       // ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
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

            _repo.SettingChange();

            List<SettingsVM> vms = new List<SettingsVM>();
            try
            {
                vms = _repo.SelectAll();
                if (group != null && group!= "AllGroup")
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
                vm.LastModifiedBy = Identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString();
                result = new SettingRepo(identity, Session).settingsDataUpdate(vm);
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
            string[] result = new string[6];            
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            result = new SettingRepo(identity, Session).settingsDataUpdate(vm);
            var retResult = result[0] + "~" + result[1];
            return Json(retResult, JsonRequestBehavior.AllowGet);
        }




        public ActionResult DBUpdate()
        {
            ResultVM rVM = new ResultVM();
            rVM = new SettingRepo(identity, Session).DBUpdate("");

            Session["result"] = rVM.Status + "~" + rVM.Message;
            return Redirect("/VMS/Home");
        } 





    }
}
