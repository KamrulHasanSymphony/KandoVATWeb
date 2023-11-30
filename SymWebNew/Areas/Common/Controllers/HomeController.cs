using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymVATWebUI.Areas.Common.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //
        // GET: /Common/Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}
