using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using SymOrdinary;
using SymphonySofttech.Utilities;
using SymRepository.VMS;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VMSAPI;
using System.Threading;
using Newtonsoft.Json;
using OfficeOpenXml.Packaging.Ionic.Zlib;

namespace SymVATWebUI.Areas.API
{
    public class SaleController : ApiController
    {

        private AuthHelper _authHelper = new AuthHelper();

        [HttpPost]
        public string SaveSale(SaleAPIVM vm)
        {

            var table = new DataTable();
            var resultSet = new DataSet();

            table.Columns.Add("Result");
            table.Columns.Add("Message");
            table.Rows.Add("fail", "Sale Import Fail");
            resultSet.Tables.Add(table);
            try
            {
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

                string result = api.ImportSale(vm.Xml);

                #region Remove FileName and Log
                CommonRepo settingRepo = new CommonRepo();
                string flag = settingRepo.settings("Sale", "LogXML");
                DataSet ds = OrdinaryVATDesktop.GetDataSetFromXML(result);

                if (flag == "Y")
                {
                    FileLogger.LogRegularSale("Sale", "SaveSale", vm.Xml, ds.Tables[0].Rows[0]["FileName"].ToString());
                }

                ds.Tables[0].Columns.Remove("FileName");

                result = OrdinaryVATDesktop.GetXMLFromDataSet(ds);

                #endregion

                //return new HttpResponseMessage() { Content = new StringContent(result),StatusCode = HttpStatusCode.Created};
                return result;
            }
            catch (Exception e)
            {

                FileLogger.Log("Sale", "SaveSale", e.Message + "\n" + e.StackTrace + "\n" + vm.Xml + "\n" + vm.Bin + "\n" + vm.Password + "\n" + vm.UserName);

                table.Rows[0]["Message"] = e.Message;

                return OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);

                //return new HttpResponseMessage()
                //{
                //    Content = new StringContent(OrdinaryVATDesktop.GetXMLFromDataSet(resultSet)),
                //    StatusCode = HttpStatusCode.InternalServerError
                //};

            }
        }


        [HttpPatch]
        public HttpResponseMessage CreateSale(SaleAPIVM vm)
        {

            var table = new DataTable();
            var resultSet = new DataSet();

            table.Columns.Add("Result");
            table.Columns.Add("Message");
            table.Rows.Add("fail", "Sale Import Fail");
            resultSet.Tables.Add(table);
            try
            {
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

                string result = api.ImportSale(vm.Xml);

                #region Remove FileName and Log
                CommonRepo settingRepo = new CommonRepo();
                string flag = settingRepo.settings("Sale", "LogXML");
                DataSet ds = OrdinaryVATDesktop.GetDataSetFromXML(result);

                if (flag == "Y")
                {
                    FileLogger.LogRegularSale("Sale", "SaveSale", vm.Xml, ds.Tables[0].Rows[0]["FileName"].ToString());
                }

                ds.Tables[0].Columns.Remove("FileName");

                result = OrdinaryVATDesktop.GetXMLFromDataSet(ds);

                #endregion

                var response = new HttpResponseMessage() { Content = new StringContent(result), StatusCode = HttpStatusCode.Created };
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

        [HttpPut]
        public HttpResponseMessage SaveSaleBollore(SaleAPIVM_Bollore vm)
        {

            var table = new DataTable("API");
            var resultSet = new DataSet();

            table.Columns.Add("Result");
            table.Columns.Add("Message");
            table.Rows.Add("fail", "Sale Import Fail");
            resultSet.Tables.Add(table);
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
                
                SaleAPIVM svm = new SaleAPIVM();
                svm.Bin = "8888888381-88886";
                svm.UserName = "admin";
                svm.Password = "admin";

                _authHelper.Authenticate(svm);

                SaleInvoiceRepo api = new SaleInvoiceRepo();

                ResultVM rVM = api.ImportSaleBollore(xmlData);

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
                var response = new HttpResponseMessage() { Content = new StringContent(rVM.XML), StatusCode = HttpStatusCode.Created };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/Json");

                return response;
                //return result;
            }
            catch (Exception e)
            {

                table.Rows[0]["Message"] = e.Message;

                var response = new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(resultSet)), StatusCode = HttpStatusCode.Created };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/Json");
                return response;

            }
        }


        #region OldAuth

        private void Authenticate(SaleAPIVM vm)
        {
            SymphonyVATSysCompanyInformationRepo _sysComRepo = new SymphonyVATSysCompanyInformationRepo();
            CommonRepo _commonRepo = new CommonRepo();
            ;
            UserInformationRepo _userRepo = new UserInformationRepo();

            var flag = _commonRepo.SuperInformationFileExist(AppDomain.CurrentDomain.BaseDirectory);


            var enBin = Converter.DESEncrypt(DBConstant.PassPhrase, DBConstant.EnKey, vm.Bin.Trim());

            var sysInfo = _sysComRepo.SelectAll(null, new[] { "Bin" }, new[] { enBin }).FirstOrDefault();


            var dbName =
                sysInfo.DatabaseName; //Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, sysInfo.DatabaseName);

            List<UserInformationVM> vms = _userRepo.SelectForLogin(new LoginVM()
            {
                DatabaseName = dbName,
                UserName = vm.UserName,
                UserPassword = vm.Password
            });

            if (!vms.Any())
            {
                throw new Exception("Wrong UserName/Password");
            }

            _commonRepo.LoginSuccess(dbName);

            //test = dbName + " " + vm.UserName + " " + AppDomain.CurrentDomain.BaseDirectory + " " + flag;
        }

        #endregion
    }
}
