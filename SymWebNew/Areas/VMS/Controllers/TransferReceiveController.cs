using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SymRepository.VMS;
using System.Threading;
using VATViewModel.DTOs;
using SymVATWebUI.Areas.VMS.Models;
using SymphonySofttech.Reports;
using VATServer.Ordinary;
using CrystalDecisions.Shared;
using VATServer.Library;
using System.Configuration;
using System.Data;
using SymVATWebUI.Filters;
using System.IO;

namespace SymVATWebUI.Areas.VMS.Controllers
{
    [ShampanAuthorize]
    public class TransferReceiveController : Controller
    {

        ShampanIdentity identity = null;

        TransferReceiveRepo _repo = null;

        public TransferReceiveController()
        {
            try
            {

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new TransferReceiveRepo(identity);
            }
            catch 
            {
                
            }

        }
        public ActionResult Index(TransferReceiveVM paramVM)
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

            if (string.IsNullOrWhiteSpace(paramVM.TransactionType))
            {
                paramVM.TransactionType = "61In";
            }

            paramVM.ReceiveDateFrom = DateTime.Today.ToString("dd-MMM-yyyy"); //"1/1/1900";
            paramVM.ReceiveDateTo = DateTime.Today.ToString("dd-MMM-yyyy"); //"1/1/2050";

            #region BranchList

            ////int userId = Convert.ToInt32(identity.UserId);
            ////var list = new SymRepository.VMS.BranchRepo(identity).UserDropDownBranchProfile(userId);

            ////var listBranch = new SymRepository.VMS.BranchRepo(identity).SelectAll();

            ////if (list.Count() == listBranch.Count())
            ////{
            ////    list.Add(new BranchProfileVM() { BranchID = -1, BranchName = "All" });
            ////}

            ////paramVM.BranchList = list;

            #endregion

            return View(paramVM);
        }

        public ActionResult _index(JQueryDataTableParamVM param, TransferReceiveVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferReceiveRepo(identity, Session);
            List<TransferReceiveVM> getAllData = new List<TransferReceiveVM>();
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
            #region SeachParameters
            //string searchedBranchId = "0";
            string dtFrom = DateTime.Now.ToString("yyyyMMdd");
            string dtTo = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            //if (!string.IsNullOrWhiteSpace(Session["Branch"] as string))
            //{
            //    searchedBranchId = Session["Branch"].ToString();
            //}
            if (!string.IsNullOrWhiteSpace(paramVM.ReceiveDateFrom))
            {
                dtFrom = paramVM.ReceiveDateFrom;
            }
            if (!string.IsNullOrWhiteSpace(paramVM.ReceiveDateTo))
            {
                dtTo = paramVM.ReceiveDateTo;
            }
            paramVM.BranchId = Convert.ToInt32(Session["BranchId"]);

            if (string.IsNullOrEmpty(paramVM.SearchField))
            {
                paramVM.SearchValue = "";
            }

            paramVM.SearchField = "tr." + paramVM.SearchField + " like";

            #endregion SeachParameters


            if (!identity.IsAdmin)
            {
                string[] conditionFields = { "TransactionDateTime>=", "TransactionDateTime<=", "TransactionType", "TransferReceiveNo like", "Post", "tr.BranchId", paramVM.SearchField };
                string[] conditionValues = { dtFrom, dtTo, paramVM.TransactionType, paramVM.TransferReceiveNo, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SearchValue };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            else
            {
                string[] conditionFields = { "TransactionDateTime>=", "TransactionDateTime<=", "TransactionType", "TransferReceiveNo like", "Post", "tr.BranchId", paramVM.SearchField };
                string[] conditionValues = { dtFrom, dtTo, paramVM.TransactionType, paramVM.TransferReceiveNo, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SearchValue };
                getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            }
            #endregion
            #region Search and Filter Data
            IEnumerable<TransferReceiveVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Id
                //TransferReceiveNo
                //TransactionDateTime
                //TransferNo
                //TotalAmount
                //SerialNo
                //Post

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.TransferReceiveNo.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.TransactionDateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.TransferFromNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.TotalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.SerialNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<TransferReceiveVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.TransferReceiveNo :
                                                           sortColumnIndex == 2 && isSortable_2 ? Ordinary.DateToString(c.TransactionDateTime) :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.TransferFromNo.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.TotalAmount.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.SerialNo.ToString() :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Post.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id+"~"+ c.Post
                , c.TransferReceiveNo
                , c.TransactionDateTime
                , c.TransferFromNo.ToString()
                , c.TotalAmount.ToString()             
                , c.SerialNo.ToString()               
                , c.Post=="Y" ? "Posted" : "Not Posted"
                ,c.TransactionType
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
            TransferReceiveVM vm = new TransferReceiveVM();
            List<TransferReceiveDetailVM> IssueDetailVMs = new List<TransferReceiveDetailVM>();
            vm.Details = IssueDetailVMs;
            vm.Operation = "add";
            vm.TransactionType = tType;
            vm.TransactionDateTime = Session["SessionDate"].ToString();

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(TransferReceiveVM vm)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferReceiveRepo(identity, Session);
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




                #region Setting default values
                if (string.IsNullOrWhiteSpace(vm.Comments))
                {
                    vm.Comments = "-";
                }
                if (string.IsNullOrWhiteSpace(vm.SerialNo))
                {
                    vm.SerialNo = "-";
                }
                if (string.IsNullOrWhiteSpace(vm.ReferenceNo))
                {
                    vm.ReferenceNo = "-";
                }
                #endregion
                if (vm.Operation.ToLower() == "add")
                {
                    if (!string.IsNullOrWhiteSpace(vm.TransferFromNo))
                    {
                        TransferReceiveVM receiveInDb = new TransferReceiveDAL().SelectAllList(0, new[] { "tr.TransferFromNo" }, new[] { vm.TransferFromNo }).FirstOrDefault();

                        if (receiveInDb != null && !string.IsNullOrWhiteSpace(receiveInDb.Id))
                        {
                            Session["result"] = "Fail~This Transfer From No Already Exist! Cannot Add!";
                            return View("Create", vm);

                        }
                    }


                    vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.CreatedBy = identity.Name;
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.Post = "N";
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                    #region Adding Line No
                    int i = 1;
                    foreach (var item in vm.Details)
                    {
                        item.ReceiveLineNo = i.ToString();
                        item.Comments = vm.Comments;
                        item.CreatedBy = vm.CreatedBy;
                        item.CreatedOn = vm.CreatedOn;
                        item.LastModifiedBy = vm.LastModifiedBy;
                        item.LastModifiedOn = vm.LastModifiedOn;
                        item.TransactionDateTime = vm.TransactionDateTime;
                        item.TransactionType = vm.TransactionType;
                        item.TransferReceiveNo = vm.TransferReceiveNo;
                        item.TransferFrom = vm.TransferFrom;
                        item.BranchId = vm.BranchId;
                        i++;
                    }
                    #endregion

                    result = _repo.Insert(vm, vm.Details, null, null);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];

                        var Master = new TransferIssueVM();

                        //TransferIssueDAL transferIssueDAl = new TransferIssueDAL();




                        //Master = transferIssueDAl.SelectAllList(0, cFields, cVals, null, null, null).FirstOrDefault();
                        //Master.Post = "Y";
                        //Master.IsTransfer = "Y";
                        //List<TransferIssueDetailVM> Details = transferIssueDAl.SelectDetail(Master.TransferIssueNo);
                        //transferIssueDAl.Post(Master, Details);

                        return RedirectToAction("Edit", new { id = result[4], TransactionType = vm.TransactionType });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");

                    #region Adding Line No
                    int i = 1;
                    foreach (var item in vm.Details)
                    {
                        item.ReceiveLineNo = i.ToString();
                        item.Comments = vm.Comments;
                        item.CreatedBy = vm.CreatedBy;
                        item.CreatedOn = vm.CreatedOn;
                        item.LastModifiedBy = vm.LastModifiedBy;
                        item.LastModifiedOn = vm.LastModifiedOn;
                        item.TransactionDateTime = vm.TransactionDateTime;
                        item.TransactionType = vm.TransactionType;
                        item.TransferReceiveNo = vm.TransferReceiveNo;
                        item.TransferFrom = vm.TransferFrom;
                        i++;
                    }
                    #endregion

                    result = _repo.Update(vm, vm.Details);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.TransactionType });
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
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;
                //  Session["result"] = "Fail~Data not Successfully";
                return View("Create", vm);
            }
        }

        [HttpGet]
        public ActionResult Edit(string id, string TransactionType)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferReceiveRepo(identity, Session);
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

            if (TransactionType == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string[] conditionFields = new string[] { "TransactionType" };
            string[] conditionValues = new string[] { TransactionType };

            TransferReceiveVM vm = new TransferReceiveVM();
            vm = _repo.SelectAll(Convert.ToInt32(id), conditionFields, conditionValues).FirstOrDefault();

            if (vm == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<TransferReceiveDetailVM> ReceiveDetailVMs = new List<TransferReceiveDetailVM>();

            ReceiveDetailVMs = _repo.SelectDetail(vm.TransferReceiveNo);

            vm.Details = ReceiveDetailVMs;
            vm.Operation = "update";
            return View("Create", vm);
        }

        public ActionResult Navigate(string id, string btn, string ttype)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var _repo = new SymRepository.VMS.CommonRepo(identity, Session);
            var targetId = _repo.GetTargetIdForTtype("TransferReceives", "Id", id, btn, ttype);
            return RedirectToAction("Edit", new { id = targetId, TransactionType = ttype });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string TransferReceiveNo)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferReceiveRepo(identity, Session);
            TransferReceiveVM vm = new TransferReceiveVM();
            vm.TransferReceiveNo = TransferReceiveNo;
            string[] result = new string[6];
            result = _repo.Post(vm);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult MultiplePost(TransferReceiveVM paramVM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferReceiveRepo(identity, Session);

            #region Access Control

            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            #endregion

            ResultVM rVM = new ResultVM();
            string[] result = new string[6];


            try
            {

                paramVM.CurrentUser = identity.UserId;

                if (paramVM != null && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    paramVM.IDs = paramVM.IDs.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

                    if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                    {
                        rVM.Message = "No Data to Post";
                        return Json(rVM, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    rVM.Message = "No Data to Post";
                    return Json(rVM, JsonRequestBehavior.AllowGet);
                }

                string[] ids = paramVM.IDs.ToArray();

                result = _repo.MultiplePost(ids);

                rVM.Status = result[0];
                rVM.Message = result[1];


            }
            catch (Exception)
            {


            }

            finally { }

            return Json(rVM, JsonRequestBehavior.AllowGet);
        }


        ////[Authorize]
        public ActionResult ExportExcel(TransferReceiveVM paramVM)
        {

            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferReceiveRepo(identity, Session);
            #region Access Control

            string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

            #endregion

            ResultVM rVM = new ResultVM();
            string[] result = new string[6];


            try
            {

                string dtFrom = null;
                string dtTo = null;

                if (paramVM.ExportAll)
                {
                    dtFrom = Convert.ToDateTime(paramVM.ReceiveDateFrom).ToString("yyyy-MM-dd 00:00:00.000");
                    dtTo = Convert.ToDateTime(paramVM.ReceiveDateTo).ToString("yyyy-MM-dd 23:59:59.000");

                }
                else
                {
                    dtFrom = "2019-07-01 00:00:00";
                    dtTo = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                }
                
                if (paramVM.BranchId == 0)
                {
                    paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                }
                if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
                {
                    paramVM.SelectTop = "All";
                }

                paramVM.CurrentUser = identity.UserId;

                if (paramVM.ExportAll)
                {
                    string[] conditionFields = new string[] { "tr.TransactionDateTime>=", "tr.TransactionDateTime<=", "tr.TransactionType", "tr.BranchId" };
                    string[] conditionValues = new string[] { dtFrom, dtTo, paramVM.TransactionType, paramVM.BranchId.ToString() };

                    // TransferIssueRepo repo = new TransferIssueRepo();

                    var list = _repo.SelectAll(0, conditionFields, conditionValues);

                    paramVM.IDs = list.Select(x => x.TransferReceiveNo).ToList();

                }

                paramVM.CurrentUser = identity.UserId;

                if (paramVM != null && paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    paramVM.IDs = paramVM.IDs.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

                    if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                    {
                        rVM.Message = "No Data to Export";
                        return Json(rVM, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    rVM.Message = "No Data to Export";
                    return Json(rVM, JsonRequestBehavior.AllowGet);
                }

                // string[] ids = paramVM.IDs.ToArray();

                DataTable dt = _repo.GetExcelDataWeb(paramVM.IDs);

                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                var vm = OrdinaryVATDesktop.DownloadExcel(dt, "TransferReceive", "TransferReceiveM");
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + vm.FileName + ".xlsx");
                    vm.varExcelPackage.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                //rVM.Status = result[0];
                //rVM.Message = result[1];


            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                Session["result"] = "Fail" + "~" + msg;
                
            }

            finally { }
            return RedirectToAction("Index");

            // return Json(rVM, JsonRequestBehavior.AllowGet);
        }





        public ActionResult GetReceiveNoPopUp(PopUpViewModel vm)
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
            return PartialView("_transferReceivePopUP", vm);

        }
        public ActionResult GetFilteredReceiveNo(TransferVM vm)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new TransferReceiveRepo(identity, Session);
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
            //if (string.IsNullOrWhiteSpace(vm.TransactionType))
            //{
            //    vm.TransactionType = "61Out";
            //}
            string[] conditionalFields;
            string[] conditionalValues;

            conditionalFields = new string[] { "TransferReceiveNo like", "Post", "TransactionType" };
            conditionalValues = new string[] { vm.SearchValue, vm.Post, vm.TransactionType };
            var list = _repo.SelectAll(0, conditionalFields, conditionalValues);
            return PartialView("_filteredReceiveNo", list);
        }


        public ActionResult ReportTransferReceiveMis(ReportCommonVM vm)
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                SysDBInfoVM.SysDatabaseName = identity.InitialCatalog;
                DatabaseInfoVM.DatabaseName = identity.InitialCatalog;
                var reports = new MISReport();

                OrdinaryVATDesktop.FontSize = "9";
                if (string.IsNullOrWhiteSpace(vm.IssueNo))
                {
                    vm.IssueNo = "";
                }
                vm.TransactionType = vm.TransactionType.Replace(".", "");
                var reportDocument = reports.TransferReceiveInReport(vm.IssueNo, vm.StartDate, vm.EndDate, vm.TransactionType, vm.Branch, vm.TransferBranch);

                var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

                return new FileStreamResult(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ActionResult MultipleSave(TransferVM vm)
        {
            ResultVM rVM = new ResultVM();
            try
            {

                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new TransferReceiveRepo(identity, Session);
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

                string[] IdArray = vm.IDs.Select(i => i.ToString()).ToArray();

                int BranchId = Convert.ToInt32(Session["BranchId"]);
                string CurrentUser = Convert.ToString(Session["LogInUserName"]);

                string[] results = _repo.MultipleSave(IdArray, vm.TransactionType, BranchId, vm.ReceiveDate, CurrentUser);

                rVM.Status = results[0];
                rVM.Message = results[1];
                return Json(rVM, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();

                rVM.Status = "Fail";
                rVM.Message = msg;
                return Json(rVM, JsonRequestBehavior.AllowGet);

            }

        }


    }
}
