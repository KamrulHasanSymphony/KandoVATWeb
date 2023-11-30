using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using VATViewModel.DTOs;

namespace SymOrdinary
{
    public class Ordinary
    {
        #region GL
        public static string[] GLDocumentTypes = { "MTR", "MAR", "FIR", "MISC", "MAH", "ENG" };
        public static string[] GDICEmailForm(string FullName, string Code, string status, string url, string mailType)
        {
            string[] EmailForm = new string[2];
            //EmailBody[0] --EmailSubject/Header
            //EmailBody[1] --Email Body
            if (mailType.ToLower() == "bdereq")
            {
                EmailForm[0] = "BDE Requisition " + status;
                EmailForm[1] = "<div>"
                                      + "<h2>Dear " + FullName + "</h2>"
                                      + "<p>BDE Requisition : <b>" + Code + "</b>"
                                      + "<br />" + status
                                      + "<br /><br />" + "Please Go through this link: <a href='" + url + "'>" + Code + "</a>"
                                      + "</p>"
                                      + "</div>";
            }
            else if (mailType.ToLower() == "bdeexpense")
            {
                EmailForm[0] = "BDE Expense " + status;
                EmailForm[1] = "<div>"
                                      + "<h2>Dear " + FullName + "</h2>"
                                      + "<p>BDE Expense : <b>" + Code + "</b>"
                                      + "<br />" + status
                                      + "<br /><br />" + "Please Go through this link: <a href='" + url + "'>" + Code + "</a>"
                                      + "</p>"
                                      + "</div>";
            }
            else if (mailType.ToLower() == "pcreq")
            {
                EmailForm[0] = "Petty Cash Requisition " + status;
                EmailForm[1] = "<div>"
                                      + "<h2>Dear " + FullName + "</h2>"
                                      + "<p>Petty Cash Requisition : <b>" + Code + "</b>"
                                      + "<br />" + status
                                      + "<br /><br />" + "Please Go through this link: <a href='" + url + "'>" + Code + "</a>"
                                      + "</p>"
                                      + "</div>";
            }
            else if (mailType.ToLower() == "pc")
            {
                EmailForm[0] = "Petty Cash Expense " + status;
                EmailForm[1] = "<div>"
                                      + "<h2>Dear " + FullName + "</h2>"
                                      + "<p>Petty Cash Expense : <b>" + Code + "</b>"
                                      + "<br />" + status
                                      + "<br /><br />" + "Please Go through this link: <a href='" + url + "'>" + Code + "</a>"
                                      + "</p>"
                                      + "</div>";
            }


            return EmailForm;
        }
        #endregion

        public static DataTable DtEmptyRowReplace(DataTable dt)
        {
            int dtrows = dt.Rows.Count;
            int dtcols = dt.Columns.Count;
            for (int m = 0; m < dtrows; m++)
            {
                for (int n = 0; n < dtcols; n++)
                {
                    if (dt.Rows[m][n] != null && string.IsNullOrEmpty(dt.Rows[m][n].ToString()))
                    {
                        dt.Rows[m][n] = 0;
                    }
                }
            }
            return dt;
        }

        public static string AddSpacesToSentence(string text)
        {
            //Regex specialCharecters = new Regex(@"^[-_@.%\w\s()]*$");
            //bool IsSpecial = false;

            String PreString = text;
            StringBuilder SB = new StringBuilder();
            int past = 0, pre = 0;
            for (int i = 0; i < PreString.Length; i++)
            {
                var a = PreString.Substring(i, 1);
                Char b = Convert.ToChar(a);
                //Char nextChar = 'A';
                //if (i < PreString.Length - 1)
                //{
                //    nextChar = Convert.ToChar(PreString.Substring(i + 1, 1));
                //}

                //IsSpecial = false;
                //IsSpecial = specialCharecters.IsMatch(nextChar.ToString());

                if (Char.IsLetterOrDigit(b) == false)
                {
                    SB.Append(' ');
                    SB.Append(b);
                }
                else if (Char.IsUpper(b))
                {
                    if (pre != i - 1)
                    {
                        SB.Append(' ');
                    }
                    SB.Append(b);
                    pre = i;
                }
                else if (Char.IsNumber(b))
                {
                    if (past != i - 1)
                    {
                        SB.Append(' ');
                    }
                    SB.Append(b);

                    past = i;
                }
                //else if (Char.IsLetterOrDigit(b) == false && !IsSpecial)
                //{
                //    SB.Append(' ');
                //    SB.Append(b);
                //}
                else
                {
                    SB.Append(b);
                    pre = 0;
                    past = 0;
                }
            }
            return SB.ToString();
        }
        public static string UserName = "admin";
        public static int BranchId = 1;
        public static string CompanyLogoPath = "";
        public static string ReportHeaderLogo = "";
        public static string WorkStationIP = "192.168.15.100";
        public static string RemoveStringExpresion(string vstr)
        {
            string str = vstr;

            str = str.Replace("\n", String.Empty);
            str = str.Replace("\r", String.Empty);
            str = str.Replace("\t", String.Empty);
            str = str.Replace("'", String.Empty);
            var demo = str.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s));
            //str = str.Replace("  ", " ");
            str = string.Join(" ", demo);
            return str;
        }
        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            // Get the key from config file

            //string key = (string)settingsReader.GetValue("SecurityKey",
            //typeof(String));
            string key = "c_key";
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            //Get your key from config file to open the lock!
            //string key = (string)settingsReader.GetValue("SecurityKey",
            //                                             typeof(String));
            string key = "c_key";
            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        public static string[] Alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ", "DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", "DT", "DU", "DV", "DW", "DX", "DY", "DZ", "EA", "EB", "EC", "ED", "EE", "EF", "EG", "EH", "EI", "EJ", "EK", "EL", "EM", "EN", "EO", "EP", "EQ", "ER", "ES", "ET", "EU", "EV", "EW", "EX", "EY", "EZ", "FA", "FB", "FC", "FD", "FE", "FF", "FG", "FH", "FI", "FJ", "FK", "FL", "FM", "FN", "FO", "FP", "FQ", "FR", "FS", "FT", "FU", "FV", "FW", "FX", "FY", "FZ" };
        public static string[] MonthNames = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        public static string DateDifferenceYMD(string fDate, string tDate, bool day)
        {
            string returnValue = "";
            try
            {
                DateTime fromDate = DateTime.ParseExact(fDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                DateTime now = DateTime.ParseExact(tDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                fromDate = fromDate.Date;
                now = now.Date.AddDays(1);

                var days = now.Day - fromDate.Day;
                if (days < 0)
                {
                    var newNow = now.AddMonths(-1);
                    days += (int)(now - newNow).TotalDays;
                    now = newNow;
                }
                var months = now.Month - fromDate.Month;
                if (months < 0)
                {
                    months += 12;
                    now = now.AddYears(-1);
                }
                var years = now.Year - fromDate.Year;

                if (!day)
                {
                    days = 0;
                }
                if (years != 0)
                {
                    returnValue = years.ToString() + " years ";
                }
                if (months != 0)
                {
                    returnValue += months.ToString() + " months ";
                }
                if (days != 0)
                {
                    returnValue += days.ToString() + " days";
                }
            }
            catch (Exception)
            {

            }

            if (returnValue == "")
                returnValue = "Less than a month";

            return returnValue;
        }
        public static string TimeToString(string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return "";
            try
            {
                val = DateTime.Parse(val).ToString("HHmm");
            }
            catch (Exception)
            {
                val = "";
            }
            return val;
        }
        public static DateTime StringToDateAsDate(string val)
        {

            try
            {
                val = DateTime.ParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
            }
            catch (Exception)
            {
                val = "";
            }
            return Convert.ToDateTime(val);
        }
        public static DateTime StringToTimeAsTime(string val)
        {
            var returnVal = string.Empty;
            try
            {
                int hour = Convert.ToInt32(val.Substring(0, 2));
                string meridiem = "";
                meridiem = hour > 12 ? "PM" : "AM";
                if (hour > 12)
                {
                    hour = hour - 12;
                }
                returnVal = hour + ":" + val.Substring(2, 2) + " " + meridiem;
            }
            catch (Exception)
            {

                returnVal = "";
            }
            return Convert.ToDateTime(returnVal);
        }
        public static string DateTimeToDate(string val)
        {
            string senderVal = val;
            try
            {
                if (string.IsNullOrWhiteSpace(val)) return "";
                val = Convert.ToDateTime(val).ToString("dd-MMM-yyyy");
                
            }
            catch (Exception)
            {
                val = "";
            }

            return val;
        }


        public static string DateFormating(string val)
        {
            string senderVal = val;
            try
            {
                if (string.IsNullOrWhiteSpace(val)) return "";

                val = val.Replace(".", "").Replace("/", "").Replace("-", "");
                if (val.Length == 6)
                {
                    val = DateTime.ParseExact(val, "ddMMyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
                }
                else if (val.Length == 7)
                {
                    val = DateTime.ParseExact(val, "ddMMMyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
                }
                else if (val.Length == 8)
                {
                    val = DateTime.ParseExact(val, "ddMMyyyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
                }
                else if (val.Length == 9)
                {
                    val = DateTime.ParseExact(val, "ddMMMyyyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
                }
                else
                {
                    if (senderVal.ToLower() == "na")
                    {
                        val = senderVal;
                    }
                }
            }
            catch (Exception)
            {
                val = "";
            }

            return val;
        }


        public static string ShortFiscalPeriod(string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return "";
            try
            {
                var mon = val.Split('-')[0].Substring(0, 3);
                var year = val.Split('-')[1].Substring(2, 2);

                val = DateTime.Parse(val).ToString("yyyyMMdd");
            }
            catch (Exception)
            {
                val = "";
            }
            return val;
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
        public static bool IsNumeric(string valueToCheck)
        {
            try
            {
                decimal tt = 0;
                bool res = Decimal.TryParse(valueToCheck, out tt);
                //Convert.ToDecimal(valueToCheck);
                if (res)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                FileLogger.Log("Program", "IsNumeric", exMessage);
            }
            #endregion Catch
            return false;

        }

        public static decimal NumericFormat(decimal value, int decimalPlace)
        {
            decimal result = value;
            try
            {
                result = Convert.ToDecimal(String.Format("{0:n" + decimalPlace + "}", result));
                return result;
            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                FileLogger.Log("Program", "IsNumeric", exMessage);
            }
            #endregion Catch
            return result;

        }

        public static decimal NumericFormat(object value, int decimalPlace)
        {
            decimal result = 0;
            try
            {
                result = Convert.ToDecimal(value);
                result = Convert.ToDecimal(String.Format("{0:n" + decimalPlace + "}", result));
                return result;
            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                FileLogger.Log("Program", "IsNumeric", exMessage);
            }
            #endregion Catch
            return result;

        }
        public static string ParseDecimal(string numb)
        {

            String val = "";
            try
            {
                val = decimal.Parse(numb.ToString()).ToString("G29");

                if (Convert.ToDecimal(val) < 1000)
                {
                    val = Convert.ToDecimal(val).ToString("0.00000");
                }
                else
                {
                    val = Convert.ToDecimal(val).ToString("0,0.00000");
                }

            }
            catch { }
            return val;
        }
        public static bool IsString(string valueToCheck)
        {
            try
            {
                return true;
            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                FileLogger.Log("Program", "IsNumeric", exMessage);
            }
            #endregion Catch

            return false;

        }

        public static bool IsInteger(string valueToCheck)
        {
            try
            {
                Convert.ToInt32(valueToCheck);
                return true;
            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                FileLogger.Log("Program", "IsNumeric", exMessage);
            }
            #endregion Catch

            return false;

        }

        public static bool IsDate(string valueToCheck)
        {
            try
            {
                Convert.ToDateTime(valueToCheck);
                return true;
            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                FileLogger.Log("Program", "IsNumeric", exMessage);
            }
            #endregion Catch

            return false;

        }
        public static bool IsBool(string valueToCheck)
        {
            try
            {
                Convert.ToBoolean(valueToCheck);
                return true;
            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                FileLogger.Log("Program", "IsNumeric", exMessage);
            }
            #endregion Catch

            return false;

        }
        public static bool IsActive(string valueToCheck)
        {
            try
            {
                if (valueToCheck.Length == 1)
                {
                    if (valueToCheck.Trim() == "Y" || valueToCheck.Trim() == "N")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }

            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                FileLogger.Log("Program", "IsNumeric", exMessage);
            }
            #endregion Catch

            return false;

        }
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static bool IsTimeFormat(string valueToCheck)
        {
            try
            {
                Regex regex = new Regex(@"^\d+\.(?:[0-5]\d)$");

                if (regex.IsMatch(valueToCheck))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                FileLogger.Log("Program", "IsNumeric", exMessage);
            }
            #endregion Catch

            return false;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date">20161231</param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static List<MonthCalculation> MonthCalculation(string date, int number)
        {
            List<MonthCalculation> monthCalculations = new List<SymOrdinary.MonthCalculation>();
            MonthCalculation monthCalculation;
            DateTime Sdate = new DateTime(Convert.ToInt32(date.Substring(0, 4)), Convert.ToInt32(date.Substring(4, 2)), Convert.ToInt32(date.Substring(6, 2)));
            for (int i = 0; i < number; i++)
            {
                monthCalculation = new MonthCalculation();
                monthCalculation.StartDate = Sdate.AddMonths(i).ToString("dd-MMM-yyyy");
                monthCalculation.EndDate = Sdate.AddMonths(i + 1).AddDays(-1).ToString("dd-MMM-yyyy");
                monthCalculations.Add(monthCalculation);
            }
            return monthCalculations;

        }

        //public static List<T> DataTableToList<T>(DataTable dt)
        //{
        //    List<T> data = new List<T>();
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        T item = GetItem<T>(row);
        //        data.Add(item);
        //    }
        //    return data;
        //}
        //public static T GetItem<T>(DataRow dr)
        //{
        //    Type temp = typeof(T);
        //    T obj = Activator.CreateInstance<T>();

        //    foreach (DataColumn column in dr.Table.Columns)
        //    {
        //        foreach (PropertyInfo pro in temp.GetProperties())
        //        {
        //            if (pro.Name == column.ColumnName)
        //                pro.SetValue(obj, dr[column.ColumnName], null);
        //            else
        //                continue;
        //        }
        //    }
        //    return obj;
        //}  

        public static DataTable ListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public static DataSet UploadExcel(string FullPath, string fileName = null)
        {
            string connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=2;';", FullPath);
            //string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
            //                           "Data Source=" + FullPath + ";" + "Extended Properties=" + "\"" +
            //                           "Excel 12.0;HDR=YES;" + "\"";
            OleDbConnection theConnection = new OleDbConnection(connectionString);
            DataSet dt = new DataSet();
            try
            {
                theConnection.Open();

                OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [" + fileName + "$]", theConnection);
                dt = new DataSet();
                da.Fill(dt);


            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                theConnection.Close();

            }
            return dt;
        }
        public static bool WriteDataTableToExcel(System.Data.DataTable dataTable, string worksheetName, string saveAsLocation)
        {
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook excelworkBook;
            Microsoft.Office.Interop.Excel.Worksheet excelSheet;
            Microsoft.Office.Interop.Excel.Range excelCellrange;
            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = false;
                excel.DisplayAlerts = false;
                excelworkBook = excel.Workbooks.Add(Type.Missing);
                excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;
                excelSheet.Name = worksheetName;
                int j = 1;
                int rowcount = j;
                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[1000, 1]];
                excelCellrange.NumberFormat = "@";
                foreach (DataRow datarow in dataTable.Rows)
                {
                    rowcount += 1;
                    for (int i = 1; i <= dataTable.Columns.Count; i++)
                    {
                        if (rowcount == j + 1)
                        {
                            excelSheet.Cells[j, i] = dataTable.Columns[i - 1].ColumnName;
                        }
                        excelSheet.Cells[rowcount, i] = datarow[i - 1].ToString();
                    }
                }
                excelCellrange = excelSheet.Range[excelSheet.Cells[j, 1], excelSheet.Cells[rowcount, dataTable.Columns.Count]];
                excelCellrange.EntireColumn.AutoFit();
                Microsoft.Office.Interop.Excel.Borders border = excelCellrange.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;
                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[1, dataTable.Columns.Count]];
                excelCellrange.Font.Bold = true;
                excelworkBook.SaveAs(saveAsLocation);
                excelworkBook.Close();
                excel.Quit();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
            finally
            {

                excelSheet = null;
                excelCellrange = null;
                excelworkBook = null;
            }
        }

        public static bool WriteDataTableToExcel(System.Data.DataTable dataTable, string worksheetName, string saveAsLocation, string[] Condition, int ColorValue)
        {
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook excelworkBook;
            Microsoft.Office.Interop.Excel.Worksheet excelSheet;
            Microsoft.Office.Interop.Excel.Range excelCellrange;

            Microsoft.Office.Interop.Excel.Range c1 = null;
            Microsoft.Office.Interop.Excel.Range c2 = null;



            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();

                excel.Visible = false;
                excel.DisplayAlerts = false;

                excelworkBook = excel.Workbooks.Add(Type.Missing);

                excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;
                excelSheet.Name = worksheetName;

                int j = 8;
                //excelSheet.Cells[1, 1] = Condition[0];
                //excelSheet.Cells[2, 1] = Condition[1];

                for (int i = 0; i < Condition.Length; i++)
                {
                    j = i + 1;
                    excelSheet.Cells[j, 1] = Condition[i];
                    c1 = excelSheet.Cells[j, 1];
                    c2 = excelSheet.Cells[j, dataTable.Columns.Count + 1];

                    excelCellrange = excel.get_Range(c1, c2);
                    //range.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);
                    excelCellrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    excelCellrange.Merge(true);

                    //oRange = (Excel.Range)oSheet.get_Range(c1, c2);
                    //oRange.EntireColumn.AutoFit();

                }
                int rowcount = j + 2;
                int sl = 1;

                int color = 1;

                int row = 0;
                foreach (DataRow datarow in dataTable.Rows)
                {

                    if (ColorValue > 0)
                    {
                        if (ColorValue == 1)
                        {
                            if (color == 1)
                            {
                                c1 = excelSheet.Cells[row + 7, 1];
                                c2 = excelSheet.Cells[row + 7, 13];

                                excelCellrange = excel.get_Range(c1, c2);
                                //string tt1=  FormattingExcelCells(excelCellrange, "#F7CCCC", System.Drawing.Color.Black, false);
                            }
                        }
                        if (ColorValue == 2)
                        {
                            if (color == 1)
                            {
                                c1 = excelSheet.Cells[row + 7, 1];
                                c2 = excelSheet.Cells[row + 7, 13];
                                excelCellrange = excel.get_Range(c1, c2);
                                //FormattingExcelCells(excelCellrange, "#F7CCCC", System.Drawing.Color.Black, false);
                            }
                            else
                            {
                                c1 = excelSheet.Cells[row + 7, 1];
                                c2 = excelSheet.Cells[row + 7, 13];
                                excelCellrange = excel.get_Range(c1, c2);
                                //FormattingExcelCells(excelCellrange, "#C2ACF6", System.Drawing.Color.Black, false);
                            }
                        }
                    }



                    row++;
                    //Balance = Balance + Convert.ToDecimal(datarow["Tax Amount (TK)"]) - Convert.ToDecimal(datarow["Tax Paid-Amount (TK)"]);
                    //InvAmount = InvAmount + Convert.ToDecimal(datarow["Inv Amount (TK)"]);
                    //TaxPaid = TaxPaid + Convert.ToDecimal(datarow["Tax Paid-Amount (TK)"]);
                    //TaxAmount = TaxAmount + Convert.ToDecimal(datarow["Tax Amount (TK)"]);

                    //datarow["Balance"] = Balance;
                    //datarow["DATEINVC"] =Tools.StringToDate(datarow["TaxRate"].ToString());
                    //datarow["SL"] = sl++;

                    rowcount += 1;
                    for (int i = 1; i <= dataTable.Columns.Count; i++)
                    {
                        if (rowcount == j + 3)
                        {
                            excelSheet.Cells[j + 2, i] = dataTable.Columns[i - 1].ColumnName;
                        }

                        excelSheet.Cells[rowcount, i] = datarow[i - 1].ToString();
                        if (rowcount > j + 1)
                        {
                            if (i == dataTable.Columns.Count)
                            {
                                if (rowcount % 2 == 0)
                                {
                                    //excelCellrange = excelSheet.Range[excelSheet.Cells[rowcount, 1], excelSheet.Cells[rowcount, dataTable.Columns.Count]];
                                    //FormattingExcelCells(excelCellrange, "#FFFFFF", System.Drawing.Color.Black, false);
                                }

                            }
                        }

                    }

                }


                excelCellrange = excelSheet.Range[excelSheet.Cells[j + 2, 1], excelSheet.Cells[rowcount, dataTable.Columns.Count]];
                excelCellrange.EntireColumn.AutoFit();
                Microsoft.Office.Interop.Excel.Borders border = excelCellrange.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;


                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[j, dataTable.Columns.Count]];
                //FormattingExcelCells(excelCellrange, "#FFFFFF", System.Drawing.Color.White, true);

                //excelCellrange.Interior.Color = System.Drawing.ColorTranslator.ToOle(ColorTranslator.FromHtml("#FFFFFF"));
                //excelCellrange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

                excelCellrange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);

                //c1 = excelSheet.Cells[rowcount + 1, 8];
                //c2 = excelSheet.Cells[rowcount + 1, 13];

                //excelCellrange = excel.get_Range(c1, c2);
                //excelCellrange.Font.Bold = true;
                //excelCellrange.NumberFormat = "#,##0_);(#,##0)";

                //c1 = excelSheet.Cells[6, 1];
                //c2 = excelSheet.Cells[6, 13];

                //excelCellrange = excel.get_Range(c1, c2);
                //excelCellrange.Font.Bold = true;
                //excelCellrange.NumberFormat = "#,##0_);(#,##0)";

                //c1 = excelSheet.Cells[7, 7];
                //c2 = excelSheet.Cells[rowcount + 1, 13];

                //excelCellrange = excel.get_Range(c1, c2);
                //excelCellrange.NumberFormat = "#,##0_);(#,##0)";

                //c1 = excelSheet.Cells[j + 3, 9];
                //c2 = excelSheet.Cells[rowcount, dataTable.Columns.Count];

                //excelCellrange = excel.get_Range(c1, c2);
                //excelCellrange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);






                excelworkBook.SaveAs(saveAsLocation);
                excelworkBook.Close();
                excel.Quit();
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                excelSheet = null;
                excelCellrange = null;
                excelworkBook = null;
            }

        }
        private string FormattingExcelCells(Microsoft.Office.Interop.Excel.Range range, string HTMLcolorCode, System.Drawing.Color fontColor, bool IsFontbool)
        {
            string tt = "";
            range.Interior.Color = System.Drawing.ColorTranslator.ToOle(ColorTranslator.FromHtml(HTMLcolorCode));
            range.Font.Color = System.Drawing.ColorTranslator.ToOle(fontColor);
            if (IsFontbool == true)
            {
                range.Font.Bold = IsFontbool;
            }
            return tt;
        }
        public static void ExportExcel(DataTable dt)
        {
            //gridbind();
            //DataSet ds1=new DataSet();
            if (dt == null) return;

            string saveFileName = "";
            //SaveFileDialog saveDialog = new SaveFileDialog();
            //saveDialog.DefaultExt = "xls";
            //saveDialog.Filter = "Excel File|*.xls";
            //saveDialog.FileName = "Sheet1";
            //saveDialog.ShowDialog();
            //saveFileName = saveDialog.FileName;
            //if (saveFileName.IndexOf(":") < 0) return;

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            object missing = System.Reflection.Missing.Value;


            if (xlApp == null)
            {
                //MessageBox.Show("Create Excel object failed, maybe you dont install Excel ");
                return;
            }
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
            Microsoft.Office.Interop.Excel.Range range;

            string oldCaption = "This is title";// Title_label .Text.Trim ();
            long totalCount = dt.Rows.Count;
            long rowRead = 0;
            float percent = 0;

            worksheet.Cells[1, 1] = oldCaption;
            range = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1000, 1]];
            range.NumberFormat = "@";
            int j = 1;
            int rowcount = j;
            //DataTable dataTable = new DataTable();

            foreach (DataRow datarow in dt.Rows)
            {
                rowcount += 1;
                for (int i = 1; i <= dt.Columns.Count; i++)
                {
                    if (rowcount == j + 1)
                    {
                        worksheet.Cells[j, i] = dt.Columns[i - 1].ColumnName;
                    }
                    worksheet.Cells[rowcount, i] = datarow[i - 1].ToString();
                }
            }

            //     for(int i=0;i<ds1.Tables[0].columns.count;i++) worksheet.Cells[2,i+1]="ds1.Tables[0].Columns.ColumnName;" range.Interior.ColorIndex="15;" range.font.bold="true;" .visible="true;" r="0;r<ds1.Tables[0].Rows.Count;r++)" i="0;i<ds1.Tables[0].Columns.Count;i++)" worksheet.cells[r+3,i+1]="ds1.Tables[0].Rows[r];" percent="((float)(100*rowRead))/totalCount;" this.caption.visible="false;" this.caption.text=" Exporting Data [" range="(Excel.Range)worksheet.Cells[2,i+1];" range.borders[excel.xlbordersindex.xlinsidehorizontal].colorindex="Excel.XlColorIndex.xlColorIndexAutomatic;" range.borders[excel.xlbordersindex.xlinsidehorizontal].linestyle="Excel.XlLineStyle.xlContinuous;" range.borders[excel.xlbordersindex.xlinsidehorizontal].weight="Excel.XlBorderWeight.xlThin;">1)
            //   {
            //    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].ColorIndex
            //=Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic;
            //    }
            workbook.Close(missing, missing, missing);
            xlApp.Quit();
            //MessageBox.Show("saved");
        }
        //public static DataTable IExcelReader() { 

        //}
        public static void exp(DataTable dt1)
        {
            GridView dgv1 = new GridView();
            dgv1.DataSource = dt1;
            string data = null;
            DataTable dt = new DataTable();
            dt = dt1;
            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            int j = 1;
            int rowcount = j;
            int startRow = 7;
            foreach (DataRow datarow in dt.Rows)
            {
                rowcount += 1;
                for (int i = 1; i <= dt.Columns.Count; i++)
                {
                    if (rowcount == j + 1)
                    {
                        xlWorkSheet.Cells[j, i] = dt.Columns[i - 1].ColumnName;
                    }
                    xlWorkSheet.Cells[rowcount, i] = datarow[i - 1].ToString();
                }
                startRow++;
            }
            Microsoft.Office.Interop.Excel.Range Company = xlWorkSheet.get_Range("A1", "S1");
            Microsoft.Office.Interop.Excel.Range range = xlWorkSheet.get_Range("A1", "S" + rowcount);
            range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //range.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(153, 153, 153));
            //xlWorkSheet.get_Range("A7", "S7").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.AliceBlue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
        }
        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                throw;

            }
            finally
            {
                GC.Collect();
            }
        }

        
        public static string StringReplacing(string stringToReplace)
        {
            string newString = stringToReplace;
            if (stringToReplace.Contains("."))
            {
                newString = Regex.Replace(stringToReplace, @"^[^.]*.", "", RegexOptions.IgnorePatternWhitespace);
            }
            newString = newString.Replace(">", "From");
            newString = newString.Replace("<", "To");
            newString = newString.Replace("!", "");
            newString = newString.Replace("=", "");
            return newString;
        }
        public static bool ColumnExists(SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))    //reader.GetName(i) == columnName
                {
                    return true;
                }
            }
            return false;
        }
        public static DataTable DtColumnStringToDate(DataTable dt, string replaceColumn)
        {
            DataTable resultDt = new DataTable();
            resultDt = dt;
            resultDt.Columns.Add("Temp", typeof(string));

            for (int i = 0; i < resultDt.Rows.Count; i++)
            {
                resultDt.Rows[i]["Temp"] = StringToDate(resultDt.Rows[i][replaceColumn].ToString());
            }
            resultDt.Columns.Remove(replaceColumn);
            resultDt.Columns.Add(replaceColumn, typeof(string));

            for (int i = 0; i < resultDt.Rows.Count; i++)
            {
                resultDt.Rows[i][replaceColumn] = resultDt.Rows[i]["Temp"].ToString();
            }
            resultDt.Columns.Remove("Temp");
            return resultDt;
        }

        public static DataTable DtColumnNameChange(DataTable dt, string oldColumnName, string newColumnName)
        {
            DataTable resultDt = new DataTable();
            resultDt = dt;

            resultDt.Columns[oldColumnName].ColumnName = newColumnName;

            return resultDt;
        }

        public static DataTable DtSetColumnsOrder(DataTable table, string[] columnNames, int StartIndex = 0)
        {
            int columnIndex = StartIndex;
            foreach (var columnName in columnNames)
            {
                table.Columns[columnName].SetOrdinal(columnIndex);
                columnIndex++;
            }
            return table;
        }

        public static DataTable DtSelectedColumn(DataTable table, string[] columnNames, Type[] columnTypes)
        {

            DataTable resultDt = new DataTable();

            DataColumnCollection columns = table.Columns;
            for (int i = 0; i < columnNames.Length; i++)
            {
                if (columns.Contains(columnNames[i]))
                {
                    resultDt.Columns.Add(columnNames[i], columnTypes[i]);
                }
            }
            //foreach (var columnName in columnNames)
            //{
            //    if (columns.Contains(columnName))
            //    {
            //resultDt.Columns.Add(columnName, typeof(decimal));
            //    }
            //}
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow toInsert = resultDt.NewRow();
                resultDt.Rows.InsertAt(toInsert, i);

                foreach (var columnName in columnNames)
                {
                    resultDt.Rows[i][columnName] = table.Rows[i][columnName].ToString();
                }
            }

            return resultDt;
        }

        public static DataTable DtDeleteColumns(DataTable table, string[] columnNames)
        {
            DataColumnCollection columns = table.Columns;

            foreach (var columnName in columnNames)
            {
                if (columns.Contains(columnName))
                {
                    table.Columns.Remove(columnName);
                }
            }
            return table;
        }



        public static DataTable DtMultiColumnStringToDate(DataTable dt, string[] replaceColumns)
        {
            DataTable resultDt = new DataTable();
            resultDt = dt;
            int count = 0;
            foreach (string item in replaceColumns)
            {
                resultDt.Columns.Add("Temp", typeof(string));
                for (int i = 0; i < resultDt.Rows.Count; i++)
                {
                    resultDt.Rows[i]["Temp"] = StringToDate(resultDt.Rows[i][replaceColumns[count]].ToString());
                }
                resultDt.Columns.Remove(replaceColumns[count]);
                resultDt.Columns.Add(replaceColumns[count], typeof(string));

                for (int i = 0; i < resultDt.Rows.Count; i++)
                {
                    resultDt.Rows[i][replaceColumns[count]] = resultDt.Rows[i]["Temp"].ToString();
                }
                resultDt.Columns.Remove("Temp");
                count++;
            }

            return resultDt;
        }

        public static DataTable DtMultiColumnStringToTime(DataTable dt, string[] replaceColumns)
        {
            DataTable resultDt = new DataTable();
            resultDt = dt;
            int count = 0;
            foreach (string item in replaceColumns)
            {
                resultDt.Columns.Add("Temp", typeof(string));
                for (int i = 0; i < resultDt.Rows.Count; i++)
                {
                    resultDt.Rows[i]["Temp"] = StringToTime(resultDt.Rows[i][replaceColumns[count]].ToString());
                }
                resultDt.Columns.Remove(replaceColumns[count]);
                resultDt.Columns.Add(replaceColumns[count], typeof(string));

                for (int i = 0; i < resultDt.Rows.Count; i++)
                {
                    resultDt.Rows[i][replaceColumns[count]] = resultDt.Rows[i]["Temp"].ToString();
                }
                resultDt.Columns.Remove("Temp");
                count++;
            }

            return resultDt;
        }

        public static string SqlTextWihtCondNJoin(string sqlText, string[] parameters = null, int paramNo = 0)
        {
            //Paramerters: 
            //01//FieldName
            //02//FieldValue
            //03//FieldDataType
            //04//Joining .
            int i = 0;
            string dataType = "";
            bool flag = false;
            if (parameters != null)
            {
                foreach (string item in parameters)
                {
                    if (i >= parameters.Length
                        || string.IsNullOrWhiteSpace(parameters[i])
                        || string.IsNullOrWhiteSpace(parameters[i + 1])
                        || string.IsNullOrWhiteSpace(parameters[i + 2])
                        || string.IsNullOrWhiteSpace(parameters[i + 3])
                        )
                    {
                        continue;
                    }

                    dataType = parameters[i + 2].Replace("System.", "").ToLower();
                    switch (dataType)
                    {
                        case "int32": if (Convert.ToInt32(parameters[i + 1]) > 0) flag = true; break;
                        case "string": if (!string.IsNullOrWhiteSpace(parameters[i + 1])) flag = true; break;
                        case "boolean": if (Convert.ToBoolean(parameters[i + 1])) flag = true; break;
                        case "decimal": if (Convert.ToDecimal(parameters[i + 1]) > 0) flag = true; break;
                        default: break;
                    }

                    if (flag)
                    {
                        sqlText += (" AND " + parameters[i + 3] + parameters[i] + "=@" + parameters[i]);
                    }
                    flag = false;
                    i += paramNo;
                }
            }
            return sqlText;
        }
        public static string SqlTextWihtCondition(string sqlText, string[] parameters = null)
        {
            int i = 0;
            string dataType = "";
            bool flag = false;
            if (parameters != null)
            {
                foreach (string item in parameters)
                {
                    if (i >= parameters.Length
                        || string.IsNullOrWhiteSpace(parameters[i])
                        || string.IsNullOrWhiteSpace(parameters[i + 1])
                        || string.IsNullOrWhiteSpace(parameters[i + 2]))
                    {
                        continue;
                    }

                    dataType = parameters[i + 2].Replace("System.", "").ToLower();
                    switch (dataType)
                    {
                        case "int32": if (Convert.ToInt32(parameters[i + 1]) > 0) flag = true; break;
                        case "string": if (!string.IsNullOrWhiteSpace(parameters[i + 1])) flag = true; break;
                        case "boolean": if (Convert.ToBoolean(parameters[i + 1])) flag = true; break;
                        case "decimal": if (Convert.ToDecimal(parameters[i + 1]) > 0) flag = true; break;
                        default: break;
                    }

                    if (flag)
                    {
                        sqlText += (" AND " + parameters[i] + "=@" + parameters[i]);
                    }
                    flag = false;
                    i += 3;
                }
            }
            return sqlText;
        }

        #region May Omit R&D
        public static SqlCommand sqlCmd(SqlCommand objComm, string[] parameters = null)
        {
            int i = 0;
            if (parameters != null)
            {
                foreach (string item in parameters)
                {

                    if (i >= parameters.Length
                        || string.IsNullOrWhiteSpace(parameters[i])
                        || string.IsNullOrWhiteSpace(parameters[i + 1]))
                    {
                        continue;
                    }
                    objComm.Parameters.AddWithValue("@" + parameters[i], parameters[i + 1]);
                    i += 2;
                }
            }
            return objComm;
        }
        public static SqlCommand sqlCmdWithCondition(SqlCommand objComm, string[] parameters = null)
        {
            int i = 0;
            string dataType = "";
            bool flag = false;
            if (parameters != null)
            {
                foreach (string item in parameters)
                {

                    if (i >= parameters.Length
                        || string.IsNullOrWhiteSpace(parameters[i])
                        || string.IsNullOrWhiteSpace(parameters[i + 1])
                        || string.IsNullOrWhiteSpace(parameters[i + 2]))
                    {
                        continue;
                    }
                    dataType = parameters[i + 2].Replace("System.", "").ToLower();
                    switch (dataType)
                    {
                        case "int32": if (Convert.ToInt32(parameters[i + 1]) > 0) flag = true; break;
                        case "string": if (!string.IsNullOrWhiteSpace(parameters[i + 1])) flag = true; break;
                        case "bool": if (Convert.ToBoolean(parameters[i + 1])) flag = true; break;
                        case "decimal": if (Convert.ToDecimal(parameters[i + 1]) > 0) flag = true; break;
                        default: break;
                    }

                    if (flag)
                    {
                        objComm.Parameters.AddWithValue("@" + parameters[i], parameters[i + 1]);
                    }
                    i += 3;
                }
            }
            return objComm;
        }
        public static SqlDataAdapter sqlDA(SqlDataAdapter da, string[] parameters = null)
        {
            int i = 0;
            if (parameters != null)
            {
                foreach (string item in parameters)
                {
                    if (i >= parameters.Length
                        || string.IsNullOrWhiteSpace(parameters[i])
                        || string.IsNullOrWhiteSpace(parameters[i + 1]))
                    {
                        continue;
                    }
                    da.SelectCommand.Parameters.AddWithValue("@" + parameters[i], parameters[i + 1]);
                    i++;
                }
            }
            return da;
        }
        #endregion May Omit R&D

        // Summary:
        // sqlText, parameters[] {FieldName, FieldValue}
        public static object DataCommand(object dataCmd, string[] parameters = null)
        {
            int i = 0;
            if (parameters != null)
            {
                foreach (string item in parameters)
                {
                    if (i >= parameters.Length
                        || string.IsNullOrWhiteSpace(parameters[i])
                        || string.IsNullOrWhiteSpace(parameters[i + 1]))
                    {
                        continue;
                    }

                    if (dataCmd.GetType() == typeof(SqlCommand))
                    {
                        ((SqlCommand)dataCmd).Parameters.AddWithValue("@" + parameters[i], parameters[i + 1]);
                    }
                    else if (dataCmd.GetType() == typeof(SqlDataAdapter))
                    {
                        ((SqlDataAdapter)dataCmd).SelectCommand.Parameters.AddWithValue("@" + parameters[i], parameters[i + 1]);
                    }
                    i += 2;
                }
            }
            return dataCmd;
        }
        public static object DataCommandWithCondition(object dataCmd, string[] parameters = null)
        {
            int i = 0;
            string dataType = "";
            bool flag = false;
            if (parameters != null)
            {
                foreach (string item in parameters)
                {
                    if (i >= parameters.Length
                        || string.IsNullOrWhiteSpace(parameters[i])
                        || string.IsNullOrWhiteSpace(parameters[i + 1])
                        || string.IsNullOrWhiteSpace(parameters[i + 2]))
                    {
                        continue;
                    }
                    dataType = parameters[i + 2].Replace("System.", "").ToLower();
                    switch (dataType)
                    {
                        case "int32": if (Convert.ToInt32(parameters[i + 1]) > 0) flag = true; break;
                        case "string": if (!string.IsNullOrWhiteSpace(parameters[i + 1])) flag = true; break;
                        case "bool": if (Convert.ToBoolean(parameters[i + 1])) flag = true; break;
                        case "decimal": if (Convert.ToDecimal(parameters[i + 1]) > 0) flag = true; break;
                        default: break;
                    }

                    if (flag)
                    {
                        if (dataCmd.GetType() == typeof(SqlCommand))
                        {
                            ((SqlCommand)dataCmd).Parameters.AddWithValue("@" + parameters[i], parameters[i + 1]);
                        }
                        else if (dataCmd.GetType() == typeof(SqlDataAdapter))
                        {
                            ((SqlDataAdapter)dataCmd).SelectCommand.Parameters.AddWithValue("@" + parameters[i], parameters[i + 1]);
                        }
                    }
                    i += 3;
                }
            }
            return dataCmd;
        }
        public static SqlCommand SqlCmdSave(SqlCommand cmdSave, string[] parameters = null)
        {
            int i = 0;
            if (parameters != null)
            {
                foreach (string item in parameters)
                {
                    if (i >= parameters.Length || string.IsNullOrWhiteSpace(parameters[i]))
                    {
                        continue;
                    }
                    cmdSave.Parameters.AddWithValue("@" + parameters[i], parameters[i + 1] ?? Convert.DBNull);
                    i += 2;
                }
            }
            return cmdSave;
        }

        public static DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public static ExcelWorksheet HeaderFooter(ExcelWorksheet ws,
    string HeaderTextLeft = null, string HeaderTextCenter = null, string HeaderTextRight = null,
    string FooterTextLeft = null, string FooterTextCenter = null, string FooterTextRight = null)
        {
            if (!string.IsNullOrWhiteSpace(HeaderTextCenter))
            {
                ws.HeaderFooter.EvenHeader.CenteredText = HeaderTextCenter;
                ws.HeaderFooter.FirstHeader.CenteredText = HeaderTextCenter;
                ws.HeaderFooter.OddHeader.CenteredText = HeaderTextCenter;
            }
            if (!string.IsNullOrWhiteSpace(HeaderTextLeft))
            {
                ws.HeaderFooter.EvenHeader.LeftAlignedText = HeaderTextLeft;
                ws.HeaderFooter.FirstHeader.LeftAlignedText = HeaderTextLeft;
                ws.HeaderFooter.OddHeader.LeftAlignedText = HeaderTextLeft;
            }
            if (!string.IsNullOrWhiteSpace(HeaderTextRight))
            {
                ws.HeaderFooter.EvenHeader.RightAlignedText = HeaderTextRight;
                ws.HeaderFooter.FirstHeader.RightAlignedText = HeaderTextRight;
                ws.HeaderFooter.OddHeader.RightAlignedText = HeaderTextRight;
            }

            if (!string.IsNullOrWhiteSpace(FooterTextCenter))
            {
                ws.HeaderFooter.EvenFooter.CenteredText = FooterTextCenter;
                ws.HeaderFooter.FirstFooter.CenteredText = FooterTextCenter;
                ws.HeaderFooter.OddFooter.CenteredText = FooterTextCenter;
            }
            if (!string.IsNullOrWhiteSpace(FooterTextLeft))
            {
                ws.HeaderFooter.EvenFooter.LeftAlignedText = FooterTextLeft;
                ws.HeaderFooter.FirstFooter.LeftAlignedText = FooterTextLeft;
                ws.HeaderFooter.OddFooter.LeftAlignedText = FooterTextLeft;
            }
            if (!string.IsNullOrWhiteSpace(FooterTextRight))
            {
                ws.HeaderFooter.EvenFooter.RightAlignedText = FooterTextRight;
                ws.HeaderFooter.FirstFooter.RightAlignedText = FooterTextRight;
                ws.HeaderFooter.OddFooter.RightAlignedText = FooterTextRight;
            }

            return ws;
        }

        public static string DateToString(string val)
        {
            if (string.IsNullOrEmpty(val)) return "";
            try
            {
                val = DateTime.Parse(val).ToString("yyyyMMdd");
            }
            catch (Exception)
            {
                val = "";
            }
            return val;
        }
        public static string StringToDate(string val)
        {
            if (string.IsNullOrEmpty(val)) return "";
            try
            {
                val = DateTime.ParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
            }
            catch (Exception)
            {
                val = "";
            }
            return val;
        }
        public static string StringToTime(string val)
        {
            string returnVal = "";
            if (string.IsNullOrEmpty(val)) return returnVal;
            try
            {
                int hour = Convert.ToInt32(val.Substring(0, 2));
                string meridiem = "";
                meridiem = hour > 12 ? "PM" : "AM";
                if (hour > 12)
                {
                    hour = hour - 12;
                }
                returnVal = hour + ":" + val.Substring(2, 2) + " " + meridiem;
            }
            catch (Exception)
            {

                returnVal = "";
            }
            return returnVal;
        }
        public static string DateToDate(string val)
        {
            if (string.IsNullOrEmpty(val)) return "";
            try
            {
                val = Convert.ToDateTime(val).ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                val = "";
            }
            return val;
        }

        public static DataTable DtEmptyRowReplace(DataTable dt, string replaceValue = "0")
        {
            int dtrows = dt.Rows.Count;
            int dtcols = dt.Columns.Count;

            for (int m = 0; m < dtrows; m++)
            {
                for (int n = 0; n < dtcols; n++)
                {
                    var tt = dt.Rows[m][n].ToString();
                    if (string.IsNullOrEmpty(tt))
                    {

                    }
                    if (dt.Rows[m][n] != null && string.IsNullOrEmpty(dt.Rows[m][n].ToString().Trim()))
                    {
                        dt.Rows[m][n] = replaceValue;
                    }
                }
            }
            return dt;
        }

        public static DataTable DtSlColumnAdd(DataTable dt, string replaceValue = "0")
        {
            int dtrows = dt.Rows.Count;
            int dtcols = dt.Columns.Count;
            dt.Columns.Add("Sl", typeof(int));
            for (int m = 0; m < dtrows; m++)
            {
                dt.Rows[m]["Sl"] = m + 1;

            }
            return dt;
        }

        public static DataTable DtColumnAddNumeric(DataTable dt, string ColumnName, string DefaultValue = "0")
        {
            int dtrows = dt.Rows.Count;
            int dtcols = dt.Columns.Count;
            dt.Columns.Add(ColumnName, typeof(int));
            for (int m = 0; m < dtrows; m++)
            {
                dt.Rows[m][ColumnName] = DefaultValue;

            }
            return dt;
        }

        public static SysDBInfoVMTemp StaticValueReAssign(ShampanIdentity identity, HttpSessionStateBase session)
        {
            SysDBInfoVMTemp vm = new SysDBInfoVMTemp();

            try
            {
                vm.SysDatabaseName = identity.InitialCatalog;
                vm.SysUserName = SysDBInfoVM.SysUserName;
                vm.SysPassword = SysDBInfoVM.SysPassword;
                vm.SysdataSource = SysDBInfoVM.SysdataSource;

                vm.CompanyName = session[identity.CompanyId + "CompanyName"].ToString();
                vm.Address1 = session[identity.CompanyId + "Address1"].ToString();
                vm.TelephoneNo = session[identity.CompanyId + "TelephoneNo"].ToString();
                vm.FaxNo = session[identity.CompanyId + "FaxNo"].ToString();
                vm.VatRegistrationNo = session[identity.CompanyId + "VatRegistrationNo"].ToString();
                vm.Section = session[identity.CompanyId + "Section"] == null
                    ? ""
                    : session[identity.CompanyId + "Section"].ToString();
                vm.IsWCF = "N";
                vm.CurrentUser = Convert.ToString(session["LogInUserName"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return vm;
        }
    }
    

    public class MonthCalculation
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    //public class RazorPartialBridge
    //{
    //    public class WebFormShimController : Controller { }
    //    public static void RenderPartial(string partialName, object model)
    //    {
    //        //get a wrapper for the legacy WebForm context
    //        var httpCtx = new HttpContextWrapper(System.Web.HttpContext.Current);

    //        //create a mock route that points to the empty controller
    //        var rt = new RouteData();
    //        rt.Values.Add("controller", "WebFormShimController");

    //        //create a controller context for the route and http context
    //        var ctx = new ControllerContext(
    //            new RequestContext(httpCtx, rt), new WebFormShimController());

    //        //find the partial view using the viewengine
    //        var view = ViewEngines.Engines.FindPartialView(ctx, partialName).View;

    //        //create a view context and assign the model
    //        var vctx = new ViewContext(ctx, view,
    //            new ViewDataDictionary { Model = model },
    //            new TempDataDictionary(), httpCtx.Response.Output);

    //        //render the partial view
    //        view.Render(vctx, System.Web.HttpContext.Current.Response.Output);
    //    }
    //}


    #region ExcelTools

    /*
    Worksheet.Cells["A2:B2"].Style.Font.Name = "Segoe UI";
Worksheet.Cells["A2:B2"].Style.Font.Bold = true;
//Fill
Worksheet.Cells["A2:B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
Worksheet.Cells["A2:B2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Honeydew);
//Border Style
Worksheet.Cells["A2:B2"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
Worksheet.Cells["A2:B2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
//Align Center
Worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
Worksheet.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//Font Color
Worksheet.Cells["A3"].Style.Font.Color.SetColor(System.Drawing.Color.Green);
 
//Hide a row
Worksheet.Row(1).Hidden = true;
// Width of table columns
Worksheet.DefaultColWidth = 12;
Worksheet.Column(1).Width = 35;
 
//Number Fields Formating
Worksheet.Cells["C2:E5"].Style.Numberformat.Format = "#,##0.00";
//Formula
Worksheet.Cells["D5:E5"].Formula = "Sum(D2:D4)";
//comment
Worksheet.Cells["D5"].AddComment("This is comment", "Your Name Here");
Data Validations

//Validation
var validation = Worksheet.DataValidations.AddIntegerValidation("B3:B5");
validation.ErrorStyle = OfficeOpenXml.DataValidation.ExcelDataValidationWarningStyle.stop;
validation.PromptTitle = "Enter a integer value here";
validation.Prompt = "Value should be between 0 and 1000";
validation.ShowInputMessage = false;
validation.ErrorTitle = "An invalid value was entered";
validation.Error = "Value must be between 0 and 1000";
validation.ShowErrorMessage = true;
validation.Operator = OfficeOpenXml.DataValidation.ExcelDataValidationOperator.between;
validation.Formula.Value = 0;
validation.Formula2.Value = 1000;
Pie Chart

view sourceprint?
ExcelChart chart = Worksheet.Drawings.AddChart("Chart1", eChartType.Pie3D);
chart.Title.Text = "Trend Summary";
chart.SetPosition(2, 5, 2, 5);
chart.SetSize(640, 480);
chart.Series.Add("B3:B5", "A3:A5");
chart.Style = eChartStyle.Style2;
Password Protected Workbook

&nbsp; &nbsp; //Lock the workbook
    var workbook = package.Workbook;
    workbook.Protection.LockWindows = true;
    workbook.Protection.LockStructure = true;
    workbook.View.SetWindowSize(150, 525, 14500, 6000);
    workbook.View.ShowHorizontalScrollBar = false;
    workbook.View.ShowVerticalScrollBar = false;
    workbook.View.ShowSheetTabs = false;
    //Set a password for the workbookprotection
    workbook.Protection.SetPassword("Password");
     
     
      using (ExcelRange Rng = workSheet.Cells[rowCount + 1, 1, rowCount + 1, 15])
                {
                    Rng.Style.Font.Size = 12;
                    Rng.Style.Font.Bold = true;
                    //Rng.Style.Font.Italic = true;
                }
     
                #region Pie Chart

                //var myChart = workSheet.Drawings.AddChart("chart", eChartType.Pie);

                //// Define series for the chart
                //var series = myChart.Series.Add("C2: C4", "B2: B4");
                //myChart.Border.Fill.Color = System.Drawing.Color.Green;
                //myChart.Title.Text = "My Chart";
                //myChart.SetSize(400, 400);

                //// Add to 6th row and to the 6th column
                //myChart.SetPosition(100, 0, 6, 0);
                #endregion Pie Chart
    */
    #endregion ExcelTools

}
