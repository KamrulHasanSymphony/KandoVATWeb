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
    [Authorize]
    public class SettingsController : Controller
    {
        //
        // GET: /Common/Settings/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        SettingRepo _repo = new SettingRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            List<SettingsVM> vms = new List<SettingsVM>();
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_15", "index").ToString();
                vms = _repo.SettingsAll();

                return View(vms);
            }
            catch (Exception)
            {
                return View(vms);
            }
        }

        public ActionResult Edit(SettingsVM vm)
        {
            string[] result = new string[6];
            try
            {


                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_15", "edit").ToString();
                ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = Identity.Name;
                vm.LastUpdateFrom = Identity.WorkStationIP;
                result = new SettingRepo().settingsDataUpdate(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
                //return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

                //return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
                //throw;
            }

        }

    }
}
