using SymOrdinary;
using SymRepository.VMS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using VATViewModel.DTOs;

namespace SymVATWebUI.Areas.VMS.Controllers
{
    public class AuditController : Controller
    {
        //
        // GET: /VMS/Audit/

        ShampanIdentity identity = null;
        AuditRepo _repo = null;

        public AuditController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                _repo = new AuditRepo(identity);
            }
            catch
            {

            }

        }

        public ActionResult Index()
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
            return View();
        }

        public ActionResult _index(JQueryDataTableParamModel param)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new AuditRepo(identity, Session);

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

            #region Search and Filter Data

            var getAllData = _repo.SelectAll();

            IEnumerable<AuditVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Id.ToString().Contains(param.sSearch.ToLower())
                    || isSearchable1 && c.FiscalYear.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.FileName.ToLower().Contains(param.sSearch.ToLower())
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

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<AuditVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Id.ToString() :
                sortColumnIndex == 1 && isSortable_1 ? c.FiscalYear :
                sortColumnIndex == 2 && isSortable_2 ? c.FileName :
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
                , c.FiscalYear
                , c.FileName
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
        public ActionResult Create()
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

            AuditVM vm = new AuditVM();
            //vm.Operation = "add";

            return View(vm);

        }

        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            _repo = new AuditRepo(identity, Session);
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
            AuditVM vm = new AuditVM();
            vm = _repo.SelectAll(id).FirstOrDefault();

            var path = Server.MapPath("~/Files/Audits");
            path = Path.Combine(path, vm.FilePath); // Use Path.Combine for better path handling

            using (var stream = new FileStream(path, FileMode.Open))
            {
                var memory = new MemoryStream();
                stream.CopyTo(memory);
                memory.Position = 0;
                return File(memory, "application/pdf", Path.GetFileName(path));
            }

        }

        public ActionResult ImportAudit(AuditVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new AuditRepo(identity, Session);
            string[] result = new string[6];
            try
            {
                vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.CreatedBy = identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                vm.LastModifiedBy = identity.Name;
                vm.ServerPath = Server.MapPath("~/Files/Audits");

                result = _repo.ImportFile(vm);
                Session["result"] = result[0] + "~" + result[1];
                //return View("Index", vm);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message.Replace("\r", "").Replace("\n", "");

                return RedirectToAction("Index");
            }
        }

    }
}
