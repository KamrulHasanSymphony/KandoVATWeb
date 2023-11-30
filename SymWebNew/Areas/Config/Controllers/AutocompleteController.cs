//using SymRepository.Common;
using SymRepository.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SymRepository.VMS;
using SymOrdinary;
using System.Threading;

namespace SymVATWebUI.Areas.Config.Controllers
{
    public class AutocompleteController : Controller
    {
        //

        // GET: /Enum/DropDown/
       
        public ActionResult Index()
        {
            return View();
        }
        

         
        //public JsonResult Project(string term)
        //{
        //    return Json(new ProjectRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        //}



        public JsonResult AutocompleteVehicle(string term)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new VehicleRepo(identity).Autocomplete(term), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutocompleteCustomer(string term)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new CustomerRepo(identity).Autocomplete(term), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutocompleteBranch(string term)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            return Json(new BranchRepo(identity).Autocomplete(term), JsonRequestBehavior.AllowGet);
        }





         
    }
}
