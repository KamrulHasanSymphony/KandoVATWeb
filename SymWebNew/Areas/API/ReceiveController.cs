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
    public class ReceiveController : ApiController
    {
        private AuthHelper _authHelper = new AuthHelper();

        [HttpPost]
        public string SaveReceive(SaleAPIVM vm)
        {

            var table = new System.Data.DataTable();
            var resultSet = new DataSet();

            table.Columns.Add("Result");
            table.Columns.Add("Message");
            table.Rows.Add("fail", "Receive Import Fail");

            resultSet.Tables.Add(table);
            try
            {
                FileLogger.Log("Receive", "SaveReceive",
                    vm.Xml + "\n" + vm.Bin + "\n" + vm.Password + "\n" +
                    vm.UserName);

                _authHelper.Authenticate(vm);

                ReceiveRepo api = new ReceiveRepo();

                var result = api.SaveReceive(vm.Xml);

                return result;
            }
            catch (Exception e)
            {
                FileLogger.Log("Receive", "SaveReceive",
                    e.Message + "\n" + e.StackTrace + "\n" + vm.Xml + "\n" + vm.Bin + "\n" + vm.Password + "\n" +
                    vm.UserName);

                table.Rows[0]["Message"] = e.Message;

                return OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);
            }
        }
    }
}
