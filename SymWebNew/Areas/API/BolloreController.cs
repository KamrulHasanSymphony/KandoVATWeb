using Newtonsoft.Json;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using SymOrdinary;
using SymRepository.VMS;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml;
using VATViewModel.DTOs;

namespace SymVATWebUI.Areas.API
{
    public class BolloreController : ApiController
    {

        private AuthHelper _authHelper = new AuthHelper();

        [HttpPost]
        public HttpResponseMessage SaveSaleBollore(SaleAPIVM_Bollore vm, [FromUri]string BIN)
        {
            FileLogger.Log("BolloreAPI", "SaveSaleBollore", "Data Received. BIN Number : " + BIN + " ");

            SaleAPIVM svm = new SaleAPIVM();
            ResultVM rVM = new ResultVM();

            var table = new DataTable("API");
            var resultSet = new DataSet();

            table.Columns.Add("Result");
            table.Columns.Add("Message");
            table.Rows.Add("fail", "Sale Import Fail");
            resultSet.Tables.Add(table);

            HttpContext httpContext = HttpContext.Current;

            string authHeader = httpContext.Request.Headers["Authorization"];

            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                int seperatorIndex = usernamePassword.IndexOf(':');

                svm.UserName = usernamePassword.Substring(0, seperatorIndex);
                svm.Password = usernamePassword.Substring(seperatorIndex + 1);
            }
            else
            {
                //Handle what happens if that isn't the case
                throw new Exception("The authorization header is either empty or isn't Basic.");
            }

            try
            {

                #region Decompress XML

                string compressedString = vm.Args;
                byte[] compressedData = Convert.FromBase64String(compressedString);
                byte[] decompressedData;

                using (MemoryStream ms = new MemoryStream(compressedData))
                using (GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress))
                using (MemoryStream decompressedMs = new MemoryStream())
                {
                    gzip.CopyTo(decompressedMs);
                    decompressedData = decompressedMs.ToArray();
                }

                string xmlData = Encoding.UTF8.GetString(decompressedData);

                #endregion

                svm.Bin = BIN;
                ////svm.Bin = "8888888381-88886";
                ////svm.UserName = "admin";
                ////svm.Password = "admin";

                _authHelper.Authenticate(svm);

                SaleInvoiceRepo api = new SaleInvoiceRepo();

                rVM = api.ImportSaleBollore(xmlData);

                #region XML Compress

                string DataCompressed;
                string Newxml = rVM.XML;

                DataCompressed = CompressedXmlData(Newxml);

                #region Comments

                //////////// Convert the XML data to a byte array
                //////////byte[] xmlBytes = Encoding.UTF8.GetBytes(Newxml);

                //////////// Create a memory stream to store the compressed data
                //////////using (MemoryStream outputStream = new MemoryStream())
                //////////{
                //////////    // Create a GZipStream to compress the data
                //////////    using (GZipStream gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                //////////    {
                //////////        // Write the XML bytes to the GZipStream
                //////////        gzipStream.Write(xmlBytes, 0, xmlBytes.Length);
                //////////    }

                //////////    // Get the compressed bytes from the memory stream
                //////////    byte[] compressedBytes = outputStream.ToArray();

                //////////    // Convert the compressed bytes to a Base64-encoded string
                //////////    DataCompressed = Convert.ToBase64String(compressedBytes);

                //////////}

                #endregion

                #endregion

                var jsonObject = new Dictionary<string, object>();
                jsonObject["Command"] = "TIMIResponse";
                jsonObject["Context"] = vm.Context;
                jsonObject["Args"] = DataCompressed;
                jsonObject["Company"] = vm.Company;
                jsonObject["Batch"] = vm.Batch;
                var jsonString = JsonConvert.SerializeObject(jsonObject);

                #region FileName and Log

                CommonRepo settingRepo = new CommonRepo();
                string flag = settingRepo.settings("Sale", "LogXML");

                if (flag == "Y")
                {
                    FileLogger.LogRegularSale("Sale", "SaveSaleBollore", xmlData, "Sales" + rVM.FileName);
                }

                table.Rows[0]["Result"] = rVM.Status;
                table.Rows[0]["Message"] = rVM.Message;

                #endregion

                ////var response = new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(resultSet)), StatusCode = HttpStatusCode.Created };
                var response = new HttpResponseMessage() { Content = new StringContent(jsonString), StatusCode = HttpStatusCode.Created };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/Json");

                return response;
                //return result;
            }
            catch (Exception e)
            {

                table.Rows[0]["Message"] = e.Message;

                string xml = SalesResponse_Bollore("Fail", e.Message);

                string CompressedData = CompressedXmlData(xml);

                var jsonObject = new Dictionary<string, object>();
                jsonObject["Command"] = "TIMIResponse";
                jsonObject["Context"] = vm.Context;
                jsonObject["Args"] = CompressedData;
                jsonObject["Company"] = vm.Company;
                jsonObject["Batch"] = vm.Batch;
                var jsonString = JsonConvert.SerializeObject(jsonObject);

                var response = new HttpResponseMessage()
                {
                    Content = new StringContent(jsonString),
                    StatusCode = HttpStatusCode.Created
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/Json");
                return response;

            }

            finally
            {
                SendSalesResponse(rVM.XML2, vm, rVM.FileName);
            }
        }

        public string CompressedXmlData(string xml)
        {
            #region XML Compress

            string DataCompressed;
            string Newxml = xml;

            // Convert the XML data to a byte array
            byte[] xmlBytes = Encoding.UTF8.GetBytes(Newxml);

            // Create a memory stream to store the compressed data
            using (MemoryStream outputStream = new MemoryStream())
            {
                // Create a GZipStream to compress the data
                using (GZipStream gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    // Write the XML bytes to the GZipStream
                    gzipStream.Write(xmlBytes, 0, xmlBytes.Length);
                }

                // Get the compressed bytes from the memory stream
                byte[] compressedBytes = outputStream.ToArray();

                // Convert the compressed bytes to a Base64-encoded string
                DataCompressed = Convert.ToBase64String(compressedBytes);

            }

            return DataCompressed;

            #endregion
        }

        private string SalesResponse_Bollore(string Status, String Message)
        {
            // Create an XmlDocument object
            XmlDocument xmlDocument = new XmlDocument();

            // Create the XML declaration
            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);

            // Create the root element
            XmlElement rootElement = xmlDocument.CreateElement("TIMIStatus");
            xmlDocument.AppendChild(rootElement);

            XmlElement ValueElement = xmlDocument.CreateElement("Value");
            rootElement.AppendChild(ValueElement);

            XmlElement StatusElement = xmlDocument.CreateElement("TIMIStatus");
            string status = "";
            if (Status.ToLower() == "success")
            {
                status = "OK";
            }
            else
            {
                status = "KO";
            }
            StatusElement.InnerText = status;
            ValueElement.AppendChild(StatusElement);

            XmlElement MessageElement = xmlDocument.CreateElement("TIMIMessage");
            MessageElement.InnerText = Message;
            ValueElement.AppendChild(MessageElement);

            string xmlString = xmlDocument.OuterXml;

            return xmlString;

        }

        public string SendSalesResponse(string Xml, SaleAPIVM_Bollore vm, string FileName)
        {

            try
            {
                string url = "https://esb-uat.bollore-logistics.com:32905/rest/blx/one/timi/api/response";

                #region  XML Compress

                string DataCompressed;
                string Newxml = Xml;

                DataCompressed = CompressedXmlData(Newxml);

                #endregion

                var jsonObject = new Dictionary<string, object>();
                jsonObject["Command"] = "TIMIResponse";
                jsonObject["Context"] = vm.Context;
                jsonObject["Args"] = DataCompressed;
                jsonObject["Company"] = vm.Company;
                jsonObject["Batch"] = vm.Batch;
                var jsonString = JsonConvert.SerializeObject(jsonObject);

                string responseMessage = PostData(url, jsonString);

                try
                {
                    ErrorLogVM evm = new ErrorLogVM();

                    evm.ImportId = FileName;
                    evm.FileName = FileName;
                    evm.ErrorMassage = responseMessage;
                    evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    evm.SourceName = "SalesResponseAPI";
                    evm.ActionName = "SendSalesResponse";
                    evm.TransactionName = "Sales";

                    CommonRepo _repo = new CommonRepo();

                    string[] result = _repo.ErrorLogs(evm);

                }
                catch (Exception e)
                {

                }

                return responseMessage;

            }
            catch (Exception e)
            {
                try
                {
                    ErrorLogVM evm = new ErrorLogVM();

                    evm.ImportId = FileName;
                    evm.FileName = FileName;
                    evm.ErrorMassage = e.Message;
                    evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    evm.SourceName = "SalesResponseAPIException";
                    evm.ActionName = "SendSalesResponse";
                    evm.TransactionName = "Sales";

                    CommonRepo _repo = new CommonRepo();

                    string[] result = _repo.ErrorLogs(evm);

                }
                catch (Exception ex)
                {

                }

                return e.Message;
            }
        }

        public string PostData(string url, string payLoad)
        {
            try
            {
                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                //request.Headers.Add("Authorization", "Bearer " + auth.Access_token);
                byte[] byteArray = Encoding.UTF8.GetBytes(payLoad);
                request.ContentLength = byteArray.Length;
                request.ContentType = "text/plain";
                NetworkCredential creds = GetCredentials();
                request.Credentials = creds;

                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                WebResponse response = request.GetResponse();
                datastream = response.GetResponseStream();

                StreamReader reader = new StreamReader(datastream);
                string responseMessage = reader.ReadToEnd();

                reader.Close();

                return responseMessage;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private NetworkCredential GetCredentials()
        {
            return new NetworkCredential("ext_symphony", "uexRb#ZmR9hA");
        }



    }
}
