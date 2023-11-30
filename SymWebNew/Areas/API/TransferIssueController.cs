using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SymOrdinary;
using SymRepository.VMS;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VMSAPI;

namespace SymVATWebUI.Areas.API
{
    public class TransferIssueController : ApiController
    {
        private AuthHelper _authHelper = new AuthHelper();

        [HttpPost]
        public string SaveTransferIssue(SaleAPIVM vm)
        {
            FileLogger.Log("TransferIssueController", "SaveTransferIssue", "Start of SaveTransferIssue" + vm.Xml
                   + "\n" + vm.Bin + "\n" + vm.Password + "\n" + vm.UserName);

            var table = new System.Data.DataTable();
            var resultSet = new DataSet();

            table.Columns.Add("Result");
            table.Columns.Add("Message");
            table.Rows.Add("fail", "Transfer Issue Import Fail");

            resultSet.Tables.Add(table);
            try
            {
                _authHelper.Authenticate(vm);

                TransferRepo api = new TransferRepo();

                string fileName = api.GetFileName(vm.Xml);

                CommonRepo settingRepo = new CommonRepo();
                string flag = settingRepo.settings("TransferIssue", "LogXML");
                DataSet ds = OrdinaryVATDesktop.GetDataSetFromXML(fileName);

                string result = api.SaveTransferIssue(vm.Xml);

                if (flag == "Y")
                {
                    FileLogger.LogRegularSale("Transfer", "SaveTransferIssue", vm.Xml, ds.Tables[0].Rows[0]["FileName"].ToString());
                }

                return result;
            }
            catch (Exception e)
            {
                //FileLogger.Log("TransferIssueController", "SaveTransferIssue",e.Message + "\n" + e.StackTrace + "\n" + vm.Xml 
                //    + "\n" + vm.Bin + "\n" + vm.Password + "\n" + vm.UserName);
                table.Rows[0]["Message"] = e.Message;
                return OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);
            }
        }
    }
}
