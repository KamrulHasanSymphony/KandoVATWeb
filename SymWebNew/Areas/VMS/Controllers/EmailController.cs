using Excel;
using JQueryDataTables.Models;
using SymRepository.Common;
using SymRepository.Vms;
//using SymViewModel.Sage;
using SymViewModel.Vms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    public class EmailController : Controller
    {
        //
        // GET: /Sage/Email/
        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        EmailSendRepo _repo = new EmailSendRepo();

        public ActionResult MailList()
        {
            return View();
        }

        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //01     //FromAddress
            //02     //ToAddress
            //03     //Subject
            //04     //IsSend
            //05     //ScheduleDate


            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<EmailSendViewModel> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);


                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.FromAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.ToAddress.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Subject.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.IsSend.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.ScheduleDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    
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
            Func<EmailSendViewModel, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.FromAddress :
                sortColumnIndex == 2 && isSortable_2 ? c.ToAddress :
                sortColumnIndex == 3 && isSortable_3 ? c.Subject.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.IsSend.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.ScheduleDate.ToString() :
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
                , c.FromAddress
                , c.ToAddress
                , c.Subject
                , c.IsSend?"Y":"N"
                , c.ScheduleDate
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


        public ActionResult Index()
        {
            var mailvm = new MailViewModel();
            var mailDetailVm = new List<MailDetailViewModel>();
            mailvm.Vms = mailDetailVm;
            return View(mailvm);
        }

        public ActionResult BlankItem(MailDetailViewModel vm)
        {
            return PartialView("_mailTo", vm);
        }

        private string ConnectionString(string FileName, string Header)
        {
            OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder();
            if (Path.GetExtension(FileName).ToUpper() == ".XLS")
            {
                Builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                Builder.Add("Extended Properties", string.Format("Excel 8.0;IMEX=1;HDR={0};", Header));
            }
            else
            {
                Builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                Builder.Add("Extended Properties", string.Format("Excel 12.0;IMEX=1;HDR={0};", Header));
            }

            Builder.DataSource = FileName;

            return Builder.ConnectionString;
        }

        [HttpPost]
        public ActionResult MailListUpload(MailViewModel Mvm, HttpPostedFileBase file, HttpPostedFileBase mailList)
        {
            //uploading file
            string saveFilePath = "";
            if (mailList != null && mailList.ContentLength > 0)
            {
                string path = Path.GetFileName(mailList.FileName);

                saveFilePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/Sage/HtmlFiles"), path);

                mailList.SaveAs(saveFilePath);
            }
            //readingData
            #region old system
            //DataSet ds = new DataSet();
            //System.Data.DataTable dt = new System.Data.DataTable();
            //FileStream stream = System.IO.File.Open(saveFilePath, FileMode.Open, FileAccess.Read);
            //IExcelDataReader reader = null;
            //if (mailList.FileName.EndsWith(".xls"))
            //{
            //    reader = ExcelReaderFactory.CreateBinaryReader(stream);
            //}
            //else if (mailList.FileName.EndsWith(".xlsx"))
            //{
            //    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            //}
            //reader.IsFirstRowAsColumnNames = true;
            //ds = reader.AsDataSet();
            //dt = ds.Tables[0];
            //reader.Close();

            //MailDetailViewModel vm;
            //var Vms = new List<MailDetailViewModel>();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    vm = new MailDetailViewModel();
            //    vm.MailTo = dr["Email"].ToString();
            //    Vms.Add(vm);
            //}
            #endregion old system

            #region new system 1
            var Vms = new List<MailDetailViewModel>();
            MailDetailViewModel vm;
            var dt = new DataTable();
            //var query = "SELECT F2 As B FROM [Sheet1$]";
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = ConnectionString(saveFilePath, "No") })
            {
                OleDbCommand cmd = new OleDbCommand();
                OleDbDataAdapter oda = new OleDbDataAdapter();
                cmd.Connection = cn;

                cn.Open();
                DataTable dtExcelSchema;
                dtExcelSchema = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                cn.Close();

                cn.Open();
                cmd.CommandText = "SELECT * From [" + SheetName + "]";
                oda.SelectCommand = cmd;
                DataSet ds = new DataSet();
                oda.Fill(ds);
                cn.Close();
                dt = ds.Tables[0];
            }
            foreach (DataRow dr in dt.Rows)
            {
                vm = new MailDetailViewModel();
                vm.MailTo = dr["F1"].ToString();
                Vms.Add(vm);
            }

            #endregion new system 1


            //Deleting file
            if (System.IO.File.Exists(saveFilePath))
            {
                System.IO.File.Delete(saveFilePath);
            }
            if (Mvm.Vms == null)
            {
                Mvm.Vms = new List<MailDetailViewModel>();
            }
            foreach (var item in Vms)
            {
                Mvm.Vms.Add(item);
            }
            Mvm.file = file;
            return View("Index", Mvm);
        }

        [HttpPost]
        public ActionResult SendMail(MailViewModel vm)
        {
            string[] result = new string[6];
            //var repo = new EmailSendRepo();
            result = _repo.Insert(vm);
            Session["result"] = result[0] + "~" + result[1];
            return RedirectToAction("Index");

        }
    }
}
