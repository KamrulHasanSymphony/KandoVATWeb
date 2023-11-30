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
    public class IssueController : ApiController
    {
        private AuthHelper _authHelper = new AuthHelper();

        [HttpPost]
        public string SaveIssue(SaleAPIVM vm)
        {

            DataTable table = new System.Data.DataTable();
            DataSet resultSet = new DataSet();

            table.Columns.Add("Result");
            table.Columns.Add("Message");
            table.Rows.Add("fail", "Issue Import Fail");

            resultSet.Tables.Add(table);
            try 
            {

                _authHelper.Authenticate(vm);

                IssueHeaderRepo api = new IssueHeaderRepo();

                var result = api.SaveIssue(vm.Xml);

                return result;
            }
            catch (Exception e)
            {
                FileLogger.Log("Issue", "SaveIssue",
                    e.Message + "\n" + e.StackTrace + "\n" + vm.Xml + "\n" + vm.Bin + "\n" + vm.Password + "\n" +
                    vm.UserName);

                table.Rows[0]["Message"] = e.Message;

                return OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);
            }
        }
    }
}
