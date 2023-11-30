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
using SymOrdinary;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace SymVATWebUI.Areas.API
{
    public class BritishCouncilController : ApiController
    {
        private AuthHelper _authHelper = new AuthHelper();

        [HttpPost]
        public HttpResponseMessage CreateSale([FromBody]XElement xml, [FromUri]string BIN)
        {
            string fileName = "Sales-" + DateTime.Now.ToString("yyyyMMdd HHmmss");

            FileLogger.WriteToFileTempForBC(xml.ToString(), fileName, "BritishCouncilController", "CreateSale");

            FileLogger.Log("BritishCouncilSalesAPI", "CreateSaleBritishCouncil", "Data Received. BIN Number : " + BIN + "  " + xml.ToString());

            var table = new DataTable();
            var resultSet = new DataSet();

            table.Columns.Add("Result");
            table.Columns.Add("Message");
            table.Rows.Add("fail", "Sale Import Fail");
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
                    string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

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

                SaleInvoiceRepo api = new SaleInvoiceRepo();

                vm.Xml = vm.Xml.Replace("&", "and");

                ResultVM rVM = api.ImportSaleBritishCouncil(vm.Xml);

                #region Remove FileName and Log

                CommonRepo settingRepo = new CommonRepo();
                string flag = settingRepo.settings("Sale", "LogXML");
                //DataSet ds = OrdinaryVATDesktop.GetDataSetFromXML(result);

                if (flag == "Y")
                {
                    FileLogger.LogRegularSale("BritishCouncil", "CreateSale", vm.Xml, rVM.FileName);
                }

                table.Rows[0]["Result"] = rVM.Status;
                table.Rows[0]["Message"] = rVM.Message;

                ////ds.Tables[0].Columns.Remove("FileName");
                ////result = OrdinaryVATDesktop.GetXMLFromDataSet(ds);

                #endregion

                var response = new HttpResponseMessage() { Content = new StringContent(rVM.XML), StatusCode = HttpStatusCode.Created };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

                return response;
                //return result;
            }
            catch (Exception e)
            {

                FileLogger.Log("Sale", "SaveSale", e.Message + "\n" + e.StackTrace + "\n" + vm.Xml + "\n" + vm.Bin + "\n" + vm.Password + "\n" + vm.UserName);

                table.Rows[0]["Message"] = e.Message;

                //return  OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);
                var response = new HttpResponseMessage() { Content = new StringContent(OrdinaryVATDesktop.GetXMLFromDataSet(resultSet)), StatusCode = HttpStatusCode.Created };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                return response;

            }
        }


    }
}
