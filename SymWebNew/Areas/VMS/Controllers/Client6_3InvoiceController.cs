using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using SymOrdinary;
using SymRepository.VMS;
using SymVATWebUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using VATViewModel.DTOs;

namespace SymVATWebUI.Areas.VMS.Controllers
{
    [ShampanAuthorize]
    public class Client6_3InvoiceController : Controller
    {
        ShampanIdentity identity = null;
        Client6_3InvoiceRepo _repo = null;

        public Client6_3InvoiceController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new Client6_3InvoiceRepo(identity);
            }
            catch
            {

            }
        }

        //
        // GET: /VMS/Client6_3Invoice/
        [Authorize(Roles = "Admin")]
        public ActionResult Index(Client6_3VM VM)
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
            VM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());

            return View(VM);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param, Client6_3VM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new Client6_3InvoiceRepo(identity, Session);

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
            //00     //TollID 
            //01     //Toll No
            //02     //Customer
            //03     //Address
            //04     //	Toll Date
            if (paramVM.BranchId == 0)
            {
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            }


            if (string.IsNullOrWhiteSpace(paramVM.InvoiceNo))
            {
                if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeFrom))
                {
                    paramVM.InvoiceDateTimeFrom = null;
                }
                if (!string.IsNullOrWhiteSpace(paramVM.InvoiceDateTimeFrom))
                {
                    paramVM.InvoiceDateTimeFrom = null;
                }
            }
            #region Search and Filter Data

            string[] conditionFields;
            string[] conditionValues;
            conditionFields = new string[] { "cln.InvoiceNo", "cln.InvoiceDateTime>=", "cln.InvoiceDateTime<", "cln.TransactionType", "cln.Post", "cln.BranchId" };
            conditionValues = new string[] { paramVM.InvoiceNo, paramVM.InvoiceDateTimeFrom, paramVM.InvoiceDateTimeTo, paramVM.TransactionType, paramVM.Post, paramVM.BranchId.ToString() };
            var getAllData = _repo.SelectAll(conditionFields, conditionValues);
            IEnumerable<Client6_3VM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.InvoiceNo.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.VendorName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Address.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.InvoiceDateTime.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.Post.ToLower().Contains(param.sSearch.ToLower())
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
            Func<Client6_3VM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.InvoiceNo :
                sortColumnIndex == 2 && isSortable_2 ? c.VendorName :
                sortColumnIndex == 3 && isSortable_3 ? c.Address :
                sortColumnIndex == 4 && isSortable_4 ? c.InvoiceDateTime :
                sortColumnIndex == 5 && isSortable_5 ? c.Post :
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
                , c.InvoiceNo
                , c.VendorName
                , c.Address
                , c.InvoiceDateTime= Convert.ToDateTime(c.InvoiceDateTime).ToString("yyyy-MMM-dd")
                , c.Post=="Y" ? "Posted" : "Not Posted"
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
        public ActionResult Create(string TransactionType)
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
            Client6_3VM vm = new Client6_3VM();
            List<Client6_3DetailVM> Client6_3DetailVMs = new List<Client6_3DetailVM>();
            vm.Details = Client6_3DetailVMs;
            vm.Operation = "add";
            vm.TransactionType = TransactionType;
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(Client6_3VM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new Client6_3InvoiceRepo(identity, Session);
            ResultVM rVM = new ResultVM();

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
            string[] result = new string[6];
            int i = 1;
            foreach (Client6_3DetailVM vmD in vm.Details)
            {
                vmD.InvoiceLineNo = i;
                i++;
            }

            vm.BranchId = Convert.ToInt32(Session["BranchId"]);
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    vm.CreatedBy = identity.Name;
                    vm.Post = "N";


                    //vm.ActiveStatus = true;
                    rVM = _repo.InsertToClient6_3Invoice(vm);
                    Session["result"] = rVM.Status + "~" + rVM.Message;
                    if (rVM.Status.ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { invNumber = rVM.InvoiceNo });
                    }
                    else
                    {
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    vm.LastModifiedBy = identity.Name;

                    vm.Post = "N";

                    rVM = _repo.Update(vm);
                    Session["result"] = rVM.Status + "~" + rVM.Message;

                    return RedirectToAction("Edit", new { invNumber = rVM.InvoiceNo });
                }
                else
                {
                    return View("Create", vm);
                }
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                //  Session["result"] = "Fail~Data Not Succeessfully!";
                // FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                FileLogger.Log("Client6_3InvoiceController", "CreateEdit", e.ToString());
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string invNumber)
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
            Client6_3VM vm = new Client6_3VM();


            string[] cFields = { "cln.InvoiceNo" };
            string[] cValues = { invNumber };
            //Convert.ToInt32(invNumber)
            vm = _repo.SelectAll(cFields, cValues, null, null).FirstOrDefault();

            vm.Details = _repo.SelectDetail(new[] { "clnd.InvoiceNo" }, new[] { invNumber });
            vm.InvoiceDateTime = Convert.ToDateTime(vm.InvoiceDateTime).ToString("yyyy-MMM-dd");
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            try
            {
                _repo = new Client6_3InvoiceRepo(identity, Session);
                ResultVM rVM = new ResultVM();
                ParameterVM vm = new ParameterVM();

                UserInformationRepo _UserInformationRepo = new UserInformationRepo(identity, Session);
                UserInformationVM varUserInformationVM = new UserInformationVM();
                varUserInformationVM = _UserInformationRepo.SelectAll(Convert.ToInt32(identity.UserId)).FirstOrDefault();

                vm.SignatoryName = varUserInformationVM.FullName;
                vm.SignatoryDesig = varUserInformationVM.Designation;

                vm.InvoiceNo = ids;
                rVM = _repo.Post(vm);
                return Json(rVM.Status + "~" + rVM.Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                return RedirectToAction("Edit", new { invNumber = ids });

            }
        }

        public ActionResult GetItemPopUp()
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
            PopUpViewModel vm = new PopUpViewModel();
            vm.TransactionType = "ClientFGReceiveWOBOM";
            return PartialView("_purchasePopUp", vm);
        }

        [HttpPost]
        public ActionResult BlankItem(string[] ids)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

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
            PurchaseRepo _repo = new PurchaseRepo(identity, Session);

            List<Client6_3DetailVM> vms = new List<Client6_3DetailVM>();
            for (int i = 0; i < ids.Length; i++)
            {
                List<PurchaseDetailVM> PurchaseDetailVm = new List<PurchaseDetailVM>();
                Client6_3DetailVM Client6_3DetailVm = new Client6_3DetailVM();

                PurchaseDetailVm = _repo.SelectPurchaseDetail(ids[i]);
                foreach (PurchaseDetailVM Item in PurchaseDetailVm.ToList())
                {
                    ProductRepo _Productrepo = new ProductRepo(identity, Session);

                    var Productvm = _Productrepo.SelectAll("0", new[] { "Pr.ItemNo" }, new[] { Item.ItemNo }).FirstOrDefault();
                    Client6_3DetailVm.ReceiveNo = Item.PurchaseInvoiceNo;
                    Client6_3DetailVm.ProductName = Item.ProductName;
                    Client6_3DetailVm.ItemNo = Item.ItemNo;
                    Client6_3DetailVm.Quantity = Item.Quantity;
                    Client6_3DetailVm.UOM = Item.UOM;
                    Client6_3DetailVm.UnitPrice = Productvm.TollCharge;
                    Client6_3DetailVm.SDRate = Productvm.SD;
                    Client6_3DetailVm.VATRate = Productvm.VATRate;
                    Client6_3DetailVm.Subtotal = Client6_3DetailVm.UnitPrice * Client6_3DetailVm.Quantity;
                    Client6_3DetailVm.SDAmount = Client6_3DetailVm.Subtotal * Client6_3DetailVm.SDRate / 100;
                    Client6_3DetailVm.VATAmount = (Client6_3DetailVm.Subtotal + Client6_3DetailVm.SDAmount) * Client6_3DetailVm.VATRate / 100;
                    Client6_3DetailVm.LineTotalAmount = Client6_3DetailVm.Subtotal + Client6_3DetailVm.SDAmount + Client6_3DetailVm.VATAmount;

                    vms.Add(Client6_3DetailVm);
                }
            }




            return PartialView("_detail", vms);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItemRow(Client6_3DetailVM vm)
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
            #region Calculations

            vm.VATAmount = ((vm.Subtotal + vm.SDAmount) * vm.VATRate) / 100;
            vm.LineTotalAmount = vm.Subtotal + vm.VATAmount + vm.SDAmount;
            #endregion
            List<Client6_3DetailVM> vms = new List<Client6_3DetailVM>();
            vms.Add(vm);
            return PartialView("_detail", vms);
        }


        [HttpGet]
        public ActionResult GetFilteredItemsX(Client6_3VM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new Client6_3InvoiceRepo(identity, Session);
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            string transactionType = "ClientFGReceiveWOBOM";

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


            string[] cFields = {"pih.PurchaseInvoiceNo like", 
                    "v.VendorName  like", 
                    "pih.ReceiveDate>", 
                    "pih.ReceiveDate<", 
                    "pih.TransactionType", 
                    "pih.post like",
                    "pih.BranchId",
                    "SelectTop",
                    "pih.IsClients6_3Complete isnull"
                                     };


            string[] cValues = { vm.InvoiceNo, vm.VendorName, vm.InvoiceDateTime, transactionType, vm.InvoiceDateTime, "", "N" };




            PurchaseRepo _repoSale = new PurchaseRepo(identity, Session);
            var list = _repoSale.SelectAll(0, cFields, cValues, null, null, null);

            return PartialView("_filteredTollInvoice", list);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetFilteredItems(PurchaseMasterVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            PurchaseRepo _repo = new PurchaseRepo(identity, Session);
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
            string[] conditionalFields;
            string[] conditionalValues;

            string vendor = null;
            if (vm.VendorID != "" && vm.VendorID != "0" && vm.VendorID != null)
            {
                vendor = vm.VendorID.ToString();
            }


            string IsVDSCompleted = "";

            if (vm.TransactionType == "VDS")
            {
                IsVDSCompleted = "N";
            }

            if (vm.SearchField != null)
            {
                if (vm.SearchField == "VendorName")
                {
                    vm.SearchField = "v.VendorName like";
                }
                else
                {
                    vm.SearchField = "pih." + vm.SearchField + " like";
                }
                conditionalFields = new string[] { "pih.InvoiceDateTime>", "pih.InvoiceDateTime<", "pih.TransactionType", "pih.Post", "pih.VendorID", "v.VendorGroupID", "pih.WithVDS", "pih.IsVDSCompleted isnull", "pih.IsClients6_3Complete isnull", vm.SearchField };
                conditionalValues = new string[] { vm.InvoiceDateTimeFrom, vm.InvoiceDateTimeTo, vm.TransactionType, "Y", vendor, vm.VendorGroup, vm.WithVDS, IsVDSCompleted, "N", vm.SearchValue };
            }
            else
            {
                conditionalFields = new string[] { "pih.InvoiceDateTime>", "pih.InvoiceDateTime<", "pih.TransactionType", "pih.Post", "pih.VendorID", "v.VendorGroupID", "pih.WithVDS", "pih.IsVDSCompleted isnull", "pih.IsClients6_3Complete isnull", };
                conditionalValues = new string[] { vm.InvoiceDateTimeFrom, vm.InvoiceDateTimeTo, vm.TransactionType, "Y", vendor, vm.VendorGroup, vm.WithVDS, IsVDSCompleted, "N" };
            }

            var list = _repo.SelectAll(0, conditionalFields, conditionalValues);

            return PartialView("_filteredPurchases", list);
        }


        public ActionResult Navigate(string id, string btn)
        {
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetId("Client6_3s", "Id", id, btn);

            string[] cFields = { "cln.Id" };
            string[] cValues = { targetId };
            Client6_3InvoiceRepo repo = new Client6_3InvoiceRepo(identity, Session);

            var vm = repo.SelectAll(cFields, cValues, null, null).FirstOrDefault();
            return RedirectToAction("Edit", new { invNumber = vm.InvoiceNo });
        }

    }
}
