using SymOrdinary;
using SymRepository.VMS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace SymVATWebUI.Areas.API
{
    public class PurchaseBritishCouncilController : ApiController
    {

        private AuthHelper _authHelper = new AuthHelper();


        [HttpPost]
        public HttpResponseMessage BCPurchase([FromBody]XElement xml, [FromUri]string BIN)
        {
            string fileName = "Purchase-" + DateTime.Now.ToString("yyyyMMdd HHmmss");

            FileLogger.WriteToFileTempForBC(xml.ToString(), fileName, "BritishCouncilController", "CreateSale");

            FileLogger.Log("BritishCouncilPurchaseAPI", "CreatePurchaseBritishCouncil", "Data Received. BIN Number : " + BIN + "  " + xml.ToString());

            var table = new DataTable();
            var resultSet = new DataSet();

            table.Columns.Add("Result");
            table.Columns.Add("Message");
            table.Rows.Add("fail", "Purchase Import Fail");
            resultSet.Tables.Add(table);

            SaleAPIVM vm = new SaleAPIVM
            {
                Xml = xml.ToString(),
                Bin = BIN
            };

            try
            {

                HttpContext httpContext = HttpContext.Current;

                string authHeader = httpContext.Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic"))
                {
                    string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    var fromBase64String = Convert.FromBase64String(encodedUsernamePassword);
                    string usernamePassword = encoding.GetString(fromBase64String);

                    int seperatorIndex = usernamePassword.IndexOf(':');

                    vm.UserName = usernamePassword.Substring(0, seperatorIndex);
                    vm.Password = usernamePassword.Substring(seperatorIndex + 1);
                }
                else
                {
                    //Handle what happens if that isn't the case
                    throw new Exception("The authorization header is either empty or isn't Basic.");
                }

                if (string.IsNullOrWhiteSpace(vm.Bin))
                {
                    throw new ArgumentNullException("Bin number can not be empty");
                }
                if (string.IsNullOrWhiteSpace(vm.UserName) || string.IsNullOrWhiteSpace(vm.Password))
                {
                    throw new ArgumentNullException("UserName or Password can not be empty");
                }
                if (string.IsNullOrWhiteSpace(vm.Xml))
                {
                    throw new ArgumentNullException("Xml can not be empty");
                }

                _authHelper.Authenticate(vm);

                PurchaseRepo api = new PurchaseRepo();

                vm.Xml = vm.Xml.Replace("&", "and");

                ResultVM rVM = api.SaveBCPurchase(vm.Xml);

                #region Remove FileName and Log
                CommonRepo settingRepo = new CommonRepo();
                string flag = settingRepo.settings("Purchase", "LogXML");
                //DataSet ds = OrdinaryVATDesktop.GetDataSetFromXML(result);

                if (flag == "Y")
                {
                    FileLogger.LogRegularSale("BritishCouncil", "CreatePurchase", vm.Xml, rVM.FileName);
                }

                table.Rows[0]["Result"] = rVM.Status;
                table.Rows[0]["Message"] = rVM.Message;

                #endregion

                var response = new HttpResponseMessage() { Content = new StringContent(rVM.XML), StatusCode = HttpStatusCode.Created };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

                return response;
                //return result;
            }
            catch (Exception e)
            {

                FileLogger.Log("PurchaseBritishCouncilController", "BCPurchase",
                    e.Message + "\n" + e.StackTrace + "\n" + vm.Xml + "\n" + vm.Bin + "\n" + vm.Password + "\n" +
                    vm.UserName + "\n" + HttpContext.Current.Request.Headers["Authorization"]);

                table.Rows[0]["Message"] = e.Message;

                //return  OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);
                var response = new HttpResponseMessage() { Content = new StringContent(OrdinaryVATDesktop.GetXMLFromDataSet(resultSet)), StatusCode = HttpStatusCode.Created };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                return response;

            }
        }



    }
}
