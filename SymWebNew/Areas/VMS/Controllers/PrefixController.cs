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
    public class PrefixController : Controller
    {
        //
        // GET: /VMS/Branch/

        // 
        ShampanIdentity identity = null;

        PrefixRepo _repo = null;

        public PrefixController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new PrefixRepo(identity);

            }
            catch
            {

            }
        }

        //PrefixRepo _repo = new PrefixRepo();
        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        [Authorize(Roles = "Admin")]
        public ActionResult Index(string group)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new PrefixRepo(identity, Session);

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
            List<CodeVM> vms = new List<CodeVM>();
            try
            {
                vms = _repo.SelectAll();
                if (group != null && group != "AllGroup")
                {
                    ViewBag.groupName = group;
                    var filteredList = vms.Where(m => m.CodeGroup == group).ToList();
                    return View(filteredList);
                }
                ViewBag.groupName = "AllGroup";
                return View(vms);
            }
            catch (Exception)
            {
                return View(vms);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(CodeVM vm)
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


                //Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_15", "edit").ToString();
                ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                //vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                //vm.LastModifiedBy = Identity.Name;
                //vm.LastModifiedOn = DateTime.Now.ToString();
                result = new PrefixRepo(identity, Session).PrefixDataUpdate(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
                //return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                // Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

                //return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
                //throw;
            }

        }

        [HttpPost]
        public JsonResult Update(CodeVM vm)
        {
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            result = new PrefixRepo(identity, Session).PrefixDataUpdate(vm);
            var retResult = result[0] + "~" + result[1];
            return Json(retResult, JsonRequestBehavior.AllowGet);
        }

    }
}
