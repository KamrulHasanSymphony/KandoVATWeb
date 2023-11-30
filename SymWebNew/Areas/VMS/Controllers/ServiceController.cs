using CrystalDecisions.CrystalReports.Engine;
//using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.VMS;
//using SymRepository.Common;
using VATViewModel.DTOs;
//using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading.Tasks;
using System.Net.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymVATWebUI.Filters;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class ServiceController : Controller
    {
        ShampanIdentity identity = null;

        BOMRepo _repo = null;

        public ServiceController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new BOMRepo(identity);

            }
            catch
            {

            }
        }
        //
        // GET: /Vms/FinancialTransaction/

        //BOMRepo _repo = new BOMRepo();

        //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        #region Index and _index
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
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


            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamVM param)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BOMRepo(identity, Session);

            List<BOMNBRVM> getAllData = new List<BOMNBRVM>();
            #region Access Controll
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
            string[] conditionalFields = new string[] { "bm.VATName" };
            string[] conditionalValues = new string[] { "Form Ka(Service)" };
            getAllData = _repo.SelectAll(null, conditionalFields, conditionalValues);
            #endregion
            #region Search and Filter Data
            IEnumerable<BOMNBRVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //BomId
                //FinishItemName
                //UOM
                //EffectDate
                //PNBRPrice
                //WholeSalePrice

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.FinishItemName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.UOM.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.EffectDate.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.PNBRPrice.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.WholeSalePrice.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<BOMNBRVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.FinishItemName :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.UOM :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.EffectDate :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.PNBRPrice.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.WholeSalePrice.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.BOMId+"~"+ c.Post
                , c.FinishItemName
                , c.UOM
                , c.EffectDate
                , c.PNBRPrice.ToString()             
                , c.WholeSalePrice.ToString()               
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


        #endregion

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(BOMNBRVM vm)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {

            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }
            return PartialView("_detail", vm);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create(string tType)
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

            List<BOMNBRVM> nbrVMs = new List<BOMNBRVM>();
            NBRMaster vm = new NBRMaster();
            vm.NbrVMs = nbrVMs;
            vm.Operation = "add";
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(NBRMaster vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BOMRepo(identity, Session);

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
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    foreach (var item in vm.NbrVMs)
                    {
                        item.CreatedBy = identity.Name;
                        item.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                        item.LastModifiedBy = identity.Name;
                        item.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                        //item.BOMId = null;
                        item.ActiveStatus = "Y";
                        item.Post = "N";
                        item.TotalQuantity = 1;
                        item.VATName = "Form Ka(Service)";// "Ka(Service)";

                        item.MarkupValue = item.OtherAmount;


                    }

                    result = _repo.ServiceInsert(vm.NbrVMs);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    foreach (var item in vm.NbrVMs)
                    {
                        item.LastModifiedBy = identity.Name;
                        item.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                        item.ActiveStatus = "Y";
                        item.Post = "N";
                        item.TotalQuantity = 1;
                        //item.VATName = "Ka(Service)";
                        item.VATName = "Form Ka(Service)";

                        item.MarkupValue = item.OtherAmount;

                    }
                    result = _repo.ServiceUpdate(vm.NbrVMs);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return View("Create", vm);
                    }
                }
                else
                {
                    return View("Create", vm);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;

                // Session["result"] = "Fail~Data not Successfully";
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BOMRepo(identity, Session);

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
            NBRMaster vm = new NBRMaster();

            List<BOMNBRVM> service = new List<BOMNBRVM>();

            service = _repo.SelectAll(id);

            vm.NbrVMs = service;
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new BOMRepo(identity, Session);

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
            var id = ids.Split('~')[0];
            List<BOMNBRVM> NbrVMs = _repo.SelectAll(id);
            string[] result = new string[6];
            result = _repo.ServiceDelete(NbrVMs);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            _repo = new BOMRepo(identity, Session);
            string[] a = ids.Split('~');
            var id = a[0];
            var vm = _repo.SelectAll(id);
            string[] result = new string[6];
            result = _repo.ServicePost(vm);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }

        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }

        public JsonResult SelectProductDetails(string productCode, string IssueDate)
        {
            var _repo = new ProductRepo(identity, Session);
            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };

            var product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();

            var code = product.ProductCode;
            var uom = product.UOM;
            var hscode = product.HSCodeNo;
            var costPrice = "";
            var stock = "";
            var name = product.ProductName;
            var itemNo = product.ItemNo;
            var sd = product.SD.ToString();
            var vat = product.VATRate.ToString();

            #region businessLogic
            string UserId = identity.UserId;

            if (IssueDate == "" || IssueDate == null)
            {
                IssueDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            var issueDatetime = DateTime.Parse(IssueDate).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            DataTable priceData = _repo.AvgPriceNew(code, issueDatetime, null, null, false, true, true, true, UserId);
            decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
            decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

            if (quan > 0)
            {
                costPrice = (amount / quan).ToString();
            }
            else
            {
                costPrice = "0";
            }
            #endregion businessLogic
            stock = quan.ToString();
            string result = code + "~" + uom + "~" + hscode + "~" + costPrice + "~" + stock + "~" + name + "~" + itemNo + "~" + sd + "~" + vat;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUomOption(string uomFrom)
        {
            var _repo = new UOMRepo(identity, Session);
            string[] conditionalFields = new string[] { "UOMFrom" };
            string[] conditionalValues = new string[] { uomFrom };
            var uoms = _repo.SelectAll(0, conditionalFields, conditionalValues);
            var html = "";
            foreach (var item in uoms)
            {
                html += "<option value=" + item.UOMTo + ">" + item.UOMTo + "</option>";
            }
            return Json(html, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetConvFactor(string uomFrom, string UomTo)
        {
            var _repo = new UOMRepo(identity, Session);
            string[] conditionalFields = new string[] { "UOMFrom", "UOMTo" };
            string[] conditionalValues = new string[] { uomFrom, UomTo };
            var uom = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();
            var uomFactor = uom.Convertion;
            return Json(uomFactor, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Navigate(string id, string btn)
        {
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetId("IssueHeaders", "Id", id, btn);
            return RedirectToAction("Edit", new { id = targetId });
        }

    }
}
