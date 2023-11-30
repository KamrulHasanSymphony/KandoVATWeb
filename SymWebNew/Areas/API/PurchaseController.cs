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
using System.Xml.Linq;
using System.Web;
using System.Text;
using System.Net.Http.Headers;

namespace SymVATWebUI.Areas.API
{
    public class PurchaseController : ApiController
    {
        private AuthHelper _authHelper = new AuthHelper();

        [HttpPost]
        public string SavePurchase(SaleAPIVM vm)
        {

            var table = new System.Data.DataTable();
            var resultSet = new DataSet();

            table.Columns.Add("Result");
            table.Columns.Add("Message");
            table.Rows.Add("fail", "Purchase Import Fail");

            resultSet.Tables.Add(table);
            try
            {
                _authHelper.Authenticate(vm);

                PurchaseRepo api = new PurchaseRepo();

                vm.Xml = vm.Xml.Replace("&", "and");

                var result = api.SavePurchase(vm.Xml);

                CommonRepo settingRepo = new CommonRepo();
                string flag = settingRepo.settings("Purchase", "LogXML");
                DataSet ds = OrdinaryVATDesktop.GetDataSetFromXML(result);

                if (flag == "Y")
                {
                    FileLogger.LogRegularSale("Sale", "SavePurchase", vm.Xml, "Pur-" + ds.Tables[0].Rows[0]["FileName"].ToString());
                }


                ds.Tables[0].Columns.Remove("FileName");

                result = OrdinaryVATDesktop.GetXMLFromDataSet(ds);


                return result;
            }
            catch (Exception e)
            {
                FileLogger.Log("Purchase", "SavePurchase", e.Message + "\n" + e.StackTrace + "\n" + vm.Xml + "\n" + vm.Bin + "\n" + vm.Password + "\n" + vm.UserName);
                table.Rows[0]["Message"] = e.Message;
                return OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);
            }
        }

        

    }
}
