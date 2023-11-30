using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using SymServices.VMS;

namespace SymServices.Common
{
    public class CommonImport
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion
        public string PostCheck(string Post)
        {
            string result = "";
            if (string.IsNullOrEmpty(Post))
            {
                throw new ArgumentNullException("PostCheck", "Please input Y/N for Post Information");
            }
            if (Post!="Y")
            {
                if (Post != "N")
                {
                    throw new ArgumentNullException("PostCheck", "Please input Y/N for Post Information");
                }
                else
                {
                    result = Post;
                }
            }
            else
            {
                result = Post;
            }
            return result;
        }

        public string YesNoCheck(string YesNo)
        {
            string result = "";
            if (string.IsNullOrEmpty(YesNo))
            {
                throw new ArgumentNullException("YesNoCheck", "Please input Y/N for Post Information");
            }
            if (YesNo != "Y")
            {
                if (YesNo != "N")
                {
                    throw new ArgumentNullException("YesNoCheck", "Please input Y/N for Post Information");
                }
                else
                {
                    result = YesNo;
                }
            }
            else
            {
                result = YesNo;
            }
            return result;
        }

        public void CheckNumeric(string input)
        {
            try
            {
                if (input != string.Empty)
                {

                    if (Convert.ToDecimal(input) < 0)
                    {
                        //throw new ArgumentNullException("CheckNumeric", "Please input number Information");

                    }
                }


            }
            #region Catch
            catch (Exception ex)
            {
                //FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);

            }
            #endregion Catch
        }

        public string NonStockCheck(string NonStock)
        {
            string result = "";
            if (string.IsNullOrEmpty(NonStock))
            {
                throw new ArgumentNullException("NonStockCheck", "Please input Y/N for NonStock Information");
            }
            if (NonStock != "Y")
            {
                if (NonStock != "N")
                {
                    throw new ArgumentNullException("NonStockCheck", "Please input Y/N for NonStock Information");
                }
                else
                {
                    result = NonStock;
                }
            }
            else
            {
                result = NonStock;
            }
            return result;
        }

        
        #region bool

        public bool CheckYN(string Post)
        {
            bool result = true;

            if (string.IsNullOrEmpty(Post))
            {
                result = false;
            }
            else
            {
                if (Post.ToUpper() == "Y")
                {
                    result = true;
                }
                else if (Post.ToUpper() == "N")
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        public bool CheckNumericBool(string input)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    Convert.ToDecimal(input);
                    result = true;
                }
                else
                {
                    result = false;
                }

                return result;
            }
            #region Catch
            catch (Exception ex)
            {
                //FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);

                return result;

            }
            #endregion Catch
        }

        public bool CheckDate(string input)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    //////Correct format "MM/dd/yyyy","MM/dd/yy","dd/MMM/yy"
                    Convert.ToDateTime(input);
                    result = true;
                   
                }
               

                return result;
            }
            #region Catch
            catch (Exception ex)
            {
                return result;

            }
            #endregion Catch
        }

        public string ChecKNullValue(string input)
        {
            string result = "-";

            if (string.IsNullOrEmpty(input))
            {
                result = "-";

            }
            else
            {
                result = input;
            }

            return result;
        }

        #endregion bool


        #region Burear 
        
        public string CheckCellValue(string cellValue)
        {
            string results = "Y";
            if (string.IsNullOrEmpty(cellValue))
            {
                results = "N";
                return results;
            }
            else
            {
                return results;
            }

        }

        public string CheckNumericValue(string input)
        {
            string result = "N";
            string amtValue = input;
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    if (input.StartsWith("$"))
                    {
                        amtValue = input.Substring(1).ToString();
                    }
                    
                    Convert.ToDecimal(amtValue);
                    result = "Y";
                }
                
                return result;
            }
            #region Catch
            catch (Exception ex)
            {
                //FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);

                return result;

            }
            #endregion Catch
        }


        #endregion
    }
}
