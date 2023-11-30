using SymRepository.Common;
using SymRepository.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymVATWebUI.Areas.Common.Controllers
{
    [Authorize]
    public class CommonController : Controller
    {
        //
        // GET: /Common/Common/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult EnumAlreadyExist(string tableName, string fieldName, string value)
        {
            return Json(new CommonRepo().AlreadyExist("Enum" + tableName, fieldName, value), JsonRequestBehavior.AllowGet);
        }
    }
}
