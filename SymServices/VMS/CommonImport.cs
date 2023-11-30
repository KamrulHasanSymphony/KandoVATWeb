using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;

namespace SymServices.VMS
{
    public class CommonImport
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion
        public string FindCustomerId(string customerName, string customerCode, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }
                

                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(customerName) && string.IsNullOrEmpty(customerCode))
                {
                    throw new ArgumentNullException("FindCustomerId", "Invalid Customer Information");
                }
               

                #endregion Validation
                #region Find 

                sqlText = " ";
                sqlText = " SELECT top 1 CustomerID ";
                sqlText += " from Customers";
                sqlText += " where ";
                sqlText += " CustomerName=@customerName  ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);

                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValue("@customerName", customerName);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    sqlText = " ";
                    sqlText = " SELECT top 1 CustomerID ";
                    sqlText += " from Customers";
                    sqlText += " where ";
                    sqlText += " CustomerCode=@customerCode ";

                    SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                    cmd2.Transaction = transaction;
                    cmd2.Parameters.AddWithValue("@customerCode", customerCode);

                    object obj2 = cmd2.ExecuteScalar();
                    if (obj2 == null)
                    {
                        throw new ArgumentNullException("FindCustomerId", "Customer '(" + customerName + ")' not in database");

                    }
                    else
                    {
                        retResults = obj2.ToString();
                        
                    }

                }
                else
                {
                    retResults = obj1.ToString();
                    
                }


               

                #endregion Find 


            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }
        public string FindVendorId(string vendorName, string vendorCode, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }

                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(vendorName) && string.IsNullOrEmpty(vendorCode))
                {
                    throw new ArgumentNullException("FindVendorId", "Invalid Vendor Information");
                }


                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 VendorID ";
                sqlText += " from Vendors";
                sqlText += " where ";
                sqlText += " VendorName=@vendorName ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValue("@vendorName", vendorName);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    sqlText = " ";
                    sqlText = " SELECT top 1 VendorID ";
                    sqlText += " from Vendors";
                    sqlText += " where ";
                    sqlText += " VendorCode=@vendorCode ";

                    SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                    cmd2.Transaction = transaction;
                    cmd2.Parameters.AddWithValue("@vendorCode", vendorCode);

                    object obj2 = cmd2.ExecuteScalar();
                    if (obj2 == null)
                    {
                        throw new ArgumentNullException("FindVendorId", "Vendor '(" + vendorName + ")' not in database");

                    }
                    else
                    {
                        retResults = obj2.ToString();

                    }

                }
                else
                {
                    retResults = obj1.ToString();

                }




                #endregion Find 

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }
        public string FindVehicleId(string vehicleNo,SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }

                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(vehicleNo))
                {
                    throw new ArgumentNullException("FindVehicleId", "Invalid Vehicle Information");
                }


                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 VehicleID ";
                sqlText += " from Vehicles";
                sqlText += " where ";
                sqlText += " VehicleNo=@VehicleNo ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValue("@VehicleNo", vehicleNo);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("FindVendorId", "Vehicle '(" + vehicleNo + ")' not in database");

                }
                else
                {
                    retResults = obj1.ToString();
                }
                #endregion Find

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public string FindItemId(string productName, string productCode, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }

                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(productName) && string.IsNullOrEmpty(productCode))
                {
                    throw new ArgumentNullException("FindItemId", "Invalid Item Information");
                }


                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 ItemNo ";
                sqlText += " from Products";
                sqlText += " where ";
                sqlText += " ProductCode=@productCode  ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValue("@productCode", productCode);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    sqlText = " ";
                    sqlText = " SELECT top 1 ItemNo ";
                    sqlText += " from Products";
                    sqlText += " where ";
                    sqlText += " ProductName=@productName ";

                    SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                    cmd2.Transaction = transaction;
                    cmd2.Parameters.AddWithValue("@productName", productName);

                    object obj2 = cmd2.ExecuteScalar();
                    if (obj2 == null)
                    {
                        throw new ArgumentNullException("FindItemId", "Item '(" + productName + ")' not in database");

                    }
                    else
                    {
                        retResults = obj2.ToString();

                    }

                }
                else
                {
                    retResults = obj1.ToString();

                }




                #endregion Find 
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }
        public string FindUOMn(string ItemNo, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion Initializ

 #region Try

 try
 {
     #region open connection and transaction

     if (vcurrConn == null)
     {
         if (currConn == null)
         {
             currConn = _dbsqlConnection.GetConnection();
             if (currConn.State != ConnectionState.Open)
             {
                 currConn.Open();
                 transaction = currConn.BeginTransaction("Import Data");
             }
         }
     }

     #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(ItemNo))
                {
                    throw new ArgumentNullException("FindUOMn", "Invalid Item Information");
                }


                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 uom ";
                sqlText += " from Products";
                sqlText += " where ";
                sqlText += " ItemNo=@ItemNo ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValue("@ItemNo", ItemNo);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("FindUOMn", "Item '(" + ItemNo + ")' not in database");

                    }
                    else
                    {
                        retResults = obj1.ToString();

                    }

                




                #endregion Find 

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }
        public string FindLastNBRPrice(string ItemNo, string VATName, string RequestDate, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(ItemNo))
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid Item Information");
                }
                else if (Convert.ToDateTime(RequestDate) == DateTime.MinValue || Convert.ToDateTime(RequestDate) == DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid Request Date ");
                }
                if (string.IsNullOrEmpty(VATName))
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid VATName ");
                }

                #endregion Validation
                #region FindLastNBRPrice
                ProductDAL productDal = new ProductDAL();
                retResults = productDal.GetLastNBRPriceFromBOM(ItemNo, VATName, RequestDate,
                                            currConn, transaction).ToString();


                #endregion FindLastNBRPrice


            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }
        public string FindAvgPrice(string ItemNo, string RequestDate)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            #endregion

            #region Try

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(ItemNo))
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid Item Information");
                }
                else if (Convert.ToDateTime(RequestDate) == DateTime.MinValue || Convert.ToDateTime(RequestDate) == DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid Request Date ");
                }

                #endregion Validation
                #region FindAvgPrice
                ProductDAL productDal = new ProductDAL();

                //retResults = productDal.AvgPrice(ItemNo, RequestDate, null, null).ToString();
                //retResults = productDal.AvgPriceNew(ItemNo, RequestDate, null, null).ToString();

                DataTable priceData = productDal.AvgPriceNew(ItemNo, RequestDate, null, null,false);

                decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                decimal quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                if (quantity > 0)
                {
                    retResults = (amount / quantity).ToString();
                }
                else
                {
                    retResults = "0";
                }



                #endregion FindAvgPrice
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }
        public string FindUOMc(string uomFrom, string uomTo, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }


                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(uomFrom))
                {
                    throw new ArgumentNullException("FindUOMc", "Invalid UOM From");
                }
                if (string.IsNullOrEmpty(uomTo))
                {
                    throw new ArgumentNullException("FindUOMc", "Invalid UOM To");
                }

                #endregion Validation
                #region Find UOM From

                sqlText = " ";
                sqlText = " SELECT top 1 u.UOMId ";
                sqlText += " from UOMs u";
                sqlText += " where ";
                sqlText += " u.UOMFrom=@uomFrom ";

                SqlCommand cmdUOMFrom = new SqlCommand(sqlText, currConn);
                cmdUOMFrom.Transaction = transaction;
                cmdUOMFrom.Parameters.AddWithValue("@uomFrom", uomFrom);

                object objUOMFrom = cmdUOMFrom.ExecuteScalar();
                if (objUOMFrom == null)
                {
                    retResults = "1";
                    return retResults;
                }

                    //throw new ArgumentNullException("GetProductNo", "No UOM  '" + uomFrom + "' from found in conversion");

                #endregion Find UOM From

                #region Find UOM to

                sqlText = " ";
                sqlText = " SELECT top 1 u.UOMId ";
                sqlText += " from UOMs u";
                sqlText += " where ";
                sqlText += " u.UOMTo=@uomTo ";

                SqlCommand cmdUOMTo = new SqlCommand(sqlText, currConn);
                cmdUOMTo.Transaction = transaction;
                cmdUOMTo.Parameters.AddWithValue("@uomTo", uomTo);

                object objUOMTo = cmdUOMTo.ExecuteScalar();
                if (objUOMTo == null)
                    throw new ArgumentNullException("GetProductNo", "No UOM  '" + uomTo + "' to found in conversion");

                #endregion Find UOM to

                #region UOMc

                sqlText = "  ";

                sqlText = " SELECT top 1 u.Convertion FROM UOMs u ";
                sqlText += " where ";
                sqlText += " u.UOMFrom=@uomFrom ";
                sqlText += " and u.UOMTo=@uomTo ";

                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@uomFrom", uomFrom);
                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@uomTo", uomTo);

                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (objItemNo == null)
                    throw new ArgumentNullException("GetProductNo", "No conversion found from ='" + uomFrom + "'" +
                                                                    " to '" + uomTo + "'");

                retResults = objItemNo.ToString();
                #endregion UOMc

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results
            return retResults;

            

            #endregion

        }

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

        public string SaleDetailTypeCheck(string Type)
        {
            string result = "";
            if (string.IsNullOrEmpty(Type))
            {
                throw new ArgumentNullException("SaleTypeCheck", "Please input VAT/Non VAT for Post Information");
            }
            if (Type != "VAT")
            {
                if (Type != "Non VAT")
                {
                    throw new ArgumentNullException("SaleTypeCheck", "Please input VAT/Non VAT for Post Information");
                }
                else
                {
                    result = Type;
                }
            }
            else
            {
                result = Type;
            }
            return result;
        }

        public string SaleMasterTypeCheck(string Type)
        {
            string result = "";
            if (string.IsNullOrEmpty(Type))
            {
                throw new ArgumentNullException("SaleMasterTypeCheck", "Please input New/Debit/Credit VAT for Post Information");
            }
            if (Type != "New")
            {
                if (Type != "Debit")
                {
                    if (Type != "Credit")
                    {
                        throw new ArgumentNullException("SaleMasterTypeCheck",
                                                        "Please input New/Debit/Credit VAT for Post Information");

                    }
                    else
                    {
                        result = Type;
                    }
                }
                else
                {
                    result = Type;
                }
            }
            else
            {
                result = Type;
            }
            return result;
        }
        public string FindVatName(string vatName)
        {
            #region Initializ

            string retResults = "";
           
            #endregion

           
                #region Validation
                if (string.IsNullOrEmpty(vatName))
                {
                    throw new ArgumentNullException("FindVatName", "Invalid Vat Name.");
                }
                
                #endregion Validation
                #region Find Vat Name 
                VATName vname = new VATName();

                for (int i = 0; i < vname.VATNameList.Count; i++)
                {
                    if (vatName == vname.VATNameList[i])
                        {
                            retResults = vatName;
                            break;
                        }
                }
                    
                if (retResults != vatName)
                {
                    throw new ArgumentNullException("FindVatName", "VAT Name  '" + vatName + "' to found in database");

                }

                #endregion Find Vat Name

                #region Results
                return retResults;
            #endregion

        }

        public string FindCurrencyId(string currencyCode, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                if (vcurrConn == null)
                {

                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }

                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(currencyCode))
                {
                    throw new ArgumentNullException("FindCurrencyId", "Invalid Currency Information");
                }


                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyId ";
                sqlText += " from Currencies";
                sqlText += " where ";
                sqlText += " CurrencyCode=@currencyCode ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValue("@currencyCode", currencyCode);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {

                    throw new ArgumentNullException("FindCurrencyId", "Currency '(" + currencyCode + ")' not in database");

                   

                }
                else
                {
                    retResults = obj1.ToString();

                }




                #endregion Find
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public string FindTenderId(string TenderId, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ
            string retResults = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try
            try
            {
                #region open connection and transaction

                if (vcurrConn==null)
                {

                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }

                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(TenderId))
                {
                    //throw new ArgumentNullException("FindTenderId", "Invalid Tender Information");
                    return sqlText;
                }

                #endregion Validation
                #region Find

                if (TenderId == "0")
                {
                    retResults = TenderId;
                }
                else
                {
                    sqlText = " ";
                    sqlText = " SELECT top 1 TenderId ";
                    sqlText += " from TenderHeaders";
                    sqlText += " where ";
                    sqlText += " TenderId=@TenderId ";

                    SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                    cmd1.Transaction = transaction;
                    cmd1.Parameters.AddWithValue("@TenderId", TenderId);

                    object obj1 = cmd1.ExecuteScalar();
                    if (obj1 == null)
                    {
                        throw new ArgumentNullException("FindTenderId", "Tender '(" + TenderId + ")' not in database");
                    }
                    else
                    {
                        retResults = obj1.ToString();
                    }
                }

                #endregion Find
            }
            #endregion try
            #region Catch and Finall
            catch (SqlException sqlex)
            {
                //throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                throw new ArgumentNullException("", sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                //throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                throw new ArgumentNullException("", sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion

            #region Results
            return retResults;
            #endregion

        }

        public string FindCurrencyRateFromBDT(string currencyId, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string bdtId = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }

                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(currencyId))
                {
                    throw new ArgumentNullException("FindCurrencyRateFromBDT", "Invalid Currency Information");
                }


                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyId ";
                sqlText += " from Currencies";
                sqlText += " where ";
                sqlText += " CurrencyCode='BDT'";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("FindCurrencyRateFromBDT", "Currency 'BDT' not in database");

                }
                else
                {
                    bdtId = obj1.ToString();

                }

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyRate ";
                sqlText += " from CurrencyConversion";
                sqlText += " where ";
                sqlText += " CurrencyCodeFrom=@currencyId ";
                sqlText += " and  CurrencyCodeTo=@bdtId ";


                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                cmd2.Parameters.AddWithValue("@currencyId", currencyId);
                cmd2.Parameters.AddWithValue("@bdtId", bdtId);

                object obj2 = cmd2.ExecuteScalar();
                if (obj2 == null)
                {
                    throw new ArgumentNullException("FindCurrencyRateFromBDT", "Currency '(" + currencyId + ")' not in database");

                }
                else
                {
                    retResults = obj2.ToString();

                }
                #endregion Find
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public string FindCurrencyRateBDTtoUSD(string currencyId, string convertionDate, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string bdtId = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }

                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(currencyId))
                {
                    throw new ArgumentNullException("FindCurrencyRateFromUSD", "Invalid Currency Information");
                }


                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyId ";
                sqlText += " from Currencies";
                sqlText += " where ";
                sqlText += " CurrencyCode='BDT'";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("FindCurrencyRateFromUSD", "Currency 'BDT' not in database");
                }
                else
                {
                    bdtId = obj1.ToString();
                }

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyRate ";
                sqlText += " from CurrencyConversion";
                sqlText += " where ";
                sqlText += " CurrencyCodeFrom=@currencyId ";
                sqlText += " and  CurrencyCodeTo=@bdtId ";
                sqlText += " and  ConversionDate<=@convertionDate ";
                sqlText += " order by conversionDate desc "; 

                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                cmd2.Parameters.AddWithValue("@currencyId", currencyId);
                cmd2.Parameters.AddWithValue("@bdtId", bdtId);
                cmd2.Parameters.AddWithValue("@convertionDate", convertionDate);

                object obj2 = cmd2.ExecuteScalar();
                if (obj2 == null)
                {
                    throw new ArgumentNullException("FindCurrencyId", "Currency '(" + currencyId + ")' not in database");
                }
                else
                {
                    retResults = obj2.ToString();
                }

                #endregion Find
            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

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
                //FileLogger.Log("Program", "FormatTextBox", ex.Message + Environment.NewLine + ex.StackTrace);

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

        public string CheckPrePurchaseNo(string PurchaseID, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string results = "0";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                if (string.IsNullOrEmpty(PurchaseID))
                {
                    return results;
                }

                if (PurchaseID == "0")
                {
                    return results;
                }
              

                #region open connection and transaction

                if (vcurrConn == null)
                {

                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                        }
                    }
                }

                #endregion open connection and transaction
                #region Validation
               


                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 PurchaseInvoiceNo";
                sqlText += " from PurchaseInvoiceHeaders";
                sqlText += " where ";
                sqlText += " PurchaseInvoiceNo=@PurchaseID ";
               
                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValue("@PurchaseID", PurchaseID);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("Previous Purchase ID", "Previous purchase no. '(" + PurchaseID + ")' is not in database");

                }
                else
                {
                    results = PurchaseID;
                }

                #endregion Find

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return results;

            #endregion

        }

        public string CheckReceiveReturnID(string ReturnID, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string results = "0";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                if (string.IsNullOrEmpty(ReturnID))
                {
                    return results;
                }

                if (ReturnID == "0")
                {
                    return results;
                }


                #region open connection and transaction

                if (vcurrConn == null)
                {

                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                        }
                    }
                }

                #endregion open connection and transaction
                #region Validation



                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 ReceiveId";
                sqlText += " from ReceiveHeaders";
                sqlText += " where ";
                sqlText += " ReceivId=@ReturnID ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Parameters.AddWithValue("@ReturnID", ReturnID);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("Return ID", "Previous receive no. '(" + ReturnID + ")' is not in database");

                }
                else
                {
                    results = ReturnID;
                }

                #endregion Find

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return results;

            #endregion

        }

        public string CheckIssueReturnID(string ReturnID, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string results = "0";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                if (string.IsNullOrEmpty(ReturnID))
                {
                    return results;
                }

                if (ReturnID == "0")
                {
                    return results;
                }


                #region open connection and transaction


                if (vcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                        transaction = currConn.BeginTransaction("Import Data");
                    }
                }

                #endregion open connection and transaction
                #region Validation



                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 IssueNo";
                sqlText += " from IssueHeaders";
                sqlText += " where ";
                sqlText += " IssueNo=@ReturnID ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValue("@ReturnID", ReturnID);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("Return ID", "Previous issue no. '(" + ReturnID + ")' is not in database");

                }
                else
                {
                    results = ReturnID;
                }

                #endregion Find

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return results;

            #endregion

        }

        public string CheckPreInvoiceID(string InvoiceID, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string results = "0";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                if (string.IsNullOrEmpty(InvoiceID))
                {
                    return results;
                }

                if (InvoiceID == "0")
                {
                    return results;
                }


                #region open connection and transaction
                if (vcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction
                #region Validation



                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 SalesInvoiceNo";
                sqlText += " from SalesInvoiceHeaders";
                sqlText += " where ";
                sqlText += " SalesInvoiceNo=@InvoiceID ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValue("@InvoiceID", InvoiceID);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("Previous Invoice ID", "Previous invoice no. '(" + InvoiceID + ")' is not in database");

                }
                else
                {
                    results = InvoiceID;
                }

                #endregion Find

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return results;

            #endregion

        }

        public string CheckTenderID(string TenderID, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string results = "0";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                if (string.IsNullOrEmpty(TenderID))
                {
                    return results;
                }

                if (TenderID == "0")
                {
                    return results;
                }


                #region open connection and transaction
                if (vcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                        transaction = currConn.BeginTransaction("Import Data");
                    }
                }

                #endregion open connection and transaction
                #region Validation



                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 TenderId";
                sqlText += " from TenderHeaders";
                sqlText += " where ";
                sqlText += " TenderId='" + TenderID + "' ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("Tender ID", "Tender id '(" + TenderID + ")' is not in database");

                }
                else
                {
                    results = TenderID;
                }

                #endregion Find

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return results;

            #endregion

        }

        public DataTable FindAvgPriceImport(string itemNo, string tranDate, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string sqlText = "";
            DataTable retResults = new DataTable();
            SqlConnection vcurrConn = currConn;
           
            #endregion
            #region Try

            try
            {
                

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid Item Information");
                }
                else if (Convert.ToDateTime(tranDate) == DateTime.MinValue || Convert.ToDateTime(tranDate) == DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindAvgPrice", "Invalid Request Date ");
                }
                //if (string.IsNullOrEmpty(itemNo))
                //{
                //    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                //}
                //else if (Convert.ToDateTime(tranDate) < DateTime.MinValue || Convert.ToDateTime(tranDate) > DateTime.MaxValue)
                //{
                //    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");

                //}
                #endregion Validation

                #region open connection and transaction
                if (vcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                    transaction = currConn.BeginTransaction("AvgPprice");
                }

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@itemNo", itemNo);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }

                #endregion ProductExist

                #region AvgPrice

                sqlText = "  ";
                sqlText += "  SELECT SUM(isnull(SubTotal,0)) Amount,SUM(isnull(stock,0))Quantity" +
                           " FROM(   ";
                sqlText += "  (SELECT  isnull(OpeningBalance,0) Stock, isnull(p.OpeningTotalCost,0) SubTotal  FROM Products p  WHERE p.ItemNo = '" + itemNo + "')";

                sqlText += "  UNION ALL   ";
                sqlText += "  (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal   ";
                sqlText += "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('other','Service','ServiceNS','InputService','Trading','TollReceive','TollReceive-WIP','TollReceiveRaw','PurchaseCN')" +
                           "  AND ReceiveDate<='" + tranDate + "' " +
                           "AND ItemNo = '" + itemNo + "') ";


                sqlText += "  UNION ALL   ";
                sqlText += "  (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity," +
                           "isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0)SubTotal   ";
                sqlText += "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('Import','InputServiceImport','ServiceImport','ServiceNSImport','TradingImport') " +
                           "  AND ReceiveDate<='" + tranDate + "' " +
                           "AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal   ";
                sqlText += "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('PurchaseReturn','PurchaseDN')  AND ReceiveDate<='" + tranDate + "' " +
                           "AND ItemNo = '" + itemNo + "') ";
                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal  " +
                           "  FROM IssueDetails WHERE Post='Y' ";
                sqlText += "  AND IssueDateTime<='" + tranDate + "' and TransactionType IN('Other','TollFinishReceive','Tender','WIP','TollReceive','InputService','InputServiceImport','Trading','TradingTender','ExportTrading','ExportTradingTender','Service','ExportService','InternalIssue','TollIssue')" +
                           "  AND ItemNo = '" + itemNo + "')  ";
                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal    " +
                           "FROM IssueDetails WHERE Post='Y' ";
                sqlText += "  AND IssueDateTime<='" + tranDate + "' and TransactionType IN('IssueReturn','ReceiveReturn')  AND ItemNo = '" + itemNo + "')  ";
                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal   " +
                           " FROM ReceiveDetails WHERE Post='Y'";
                sqlText += "  AND ReceiveDateTime<='" + tranDate + "'and TransactionType<>'ReceiveReturn' AND ItemNo = '" + itemNo + "')";
                sqlText += "  UNION ALL ";
                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal    FROM ReceiveDetails WHERE Post='Y'";
                sqlText += "  AND ReceiveDateTime<='" + tranDate + "'and TransactionType='ReceiveReturn' AND ItemNo = '" + itemNo + "')";
                sqlText += "  UNION ALL ";
                sqlText += "  (SELECT  -isnull(sum(  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleNewQuantity,-" +
                           "isnull(sum( SubTotal),0)SubTotal " +
                "  FROM SalesInvoiceDetails ";
                sqlText += "  WHERE Post='Y' AND InvoiceDateTime<='" + tranDate + "'  " +
                           "AND TransactionType in('Other','PackageSale','PackageProduction','Service','ServiceNS','Trading','TradingTender','Tender','Debit','Credit','TollFinishIssue','ServiceStock','InternalIssue')" +
                           " AND ItemNo = '" + itemNo + "')";
                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleExpQuantity,-isnull(sum( CurrencyValue),0)SubTotal " +
                "   FROM SalesInvoiceDetails ";
                sqlText += "  WHERE Post='Y' AND InvoiceDateTime<='" + tranDate + "'  " +
                           "AND TransactionType in('Export','ExportService','ExportTrading','ExportTradingTender','ExportPackage','ExportTender') AND ItemNo = '" + itemNo + "')";
                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT isnull(sum(  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleCreditQuantity,isnull(sum( SubTotal),0)SubTotal     FROM SalesInvoiceDetails ";
                sqlText += "  WHERE Post='Y' AND InvoiceDateTime<='" + tranDate + "'  AND TransactionType in( 'Credit') AND ItemNo = '" + itemNo + "') ";
                sqlText += "  UNION ALL  ";
                sqlText += "  (select -isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)Qty,";
                sqlText += "  isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)*isnull(sum(isnull(RealPrice,0)),0)";
                sqlText += "  from DisposeDetails  LEFT OUTER JOIN ";
                sqlText += "  DisposeHeaders sih ON DisposeDetails.DisposeNumber=sih.DisposeNumber  where ItemNo='" + itemNo + "' ";
                sqlText += "  AND DisposeDetails.DisposeDate<='" + tranDate + "'  AND (DisposeDetails.Post ='Y')    ";
                sqlText += "  and sih.FromStock in ('Y'))  ";
                sqlText += "  ) AS a";


                DataTable dataTable = new DataTable("UsableItems");
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                SqlDataAdapter dataAdapt = new SqlDataAdapter(cmd);
                dataAdapt.Fill(dataTable);

                if (dataTable == null)
                {
                    throw new ArgumentNullException("GetLIFOPurchaseInformation", "No row found ");
                }
                retResults = dataTable;


                #endregion Stock

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (vcurrConn == null)
                {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public decimal FindLastNBRPriceFromBOM(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            CommonDAL cmDal = new CommonDAL();

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("FindLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

                #region open connection and transaction
                if (vcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                        transaction = currConn.BeginTransaction("Import Data");
                    }
                }

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@itemNo", itemNo);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("FindLastNBRPriceFromBOM", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price

                sqlText = "  ";
                sqlText += " select top 1 isnull(nbrprice,0) from boms";
                sqlText += " where ";
                sqlText += " FinishItemNo=@itemNo ";
                sqlText += " and vatname=@VatName ";
                sqlText += " and effectdate<=@effectDate";
                sqlText += " and post='Y'";
                sqlText += " order by effectdate desc ";


                SqlCommand cmdFindLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRPriceFromBOM.Transaction = transaction;
                cmdFindLastNBRPriceFromBOM.Parameters.AddWithValue("@itemNo", itemNo);
                cmdFindLastNBRPriceFromBOM.Parameters.AddWithValue("@VatName", VatName);
                cmdFindLastNBRPriceFromBOM.Parameters.AddWithValue("@effectDate", effectDate);

                if (cmdFindLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdFindLastNBRPriceFromBOM.ExecuteScalar();
                }
                if (retResults == 0)
                {
                    sqlText = "  ";
                    sqlText += " select isnull(NBRPrice,0) from products";
                    sqlText += " where ";
                    sqlText += " itemno=@itemNo ";

                    SqlCommand cmdFindLastNBRPriceFromProducts = new SqlCommand(sqlText, currConn);
                    cmdFindLastNBRPriceFromProducts.Transaction = transaction;
                    cmdFindLastNBRPriceFromProducts.Parameters.AddWithValue("@itemNo", itemNo);

                    if (cmdFindLastNBRPriceFromProducts.ExecuteScalar() == null)
                    {
                        retResults = 0;
                    }
                    else
                    {
                        retResults = (decimal)cmdFindLastNBRPriceFromProducts.ExecuteScalar();
                        retResults = cmDal.FormatingDecimal(retResults.ToString());

                    }
                }


                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public string FindCustGroupID(string custGrpName, string custGrpID, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }


                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(custGrpName) && string.IsNullOrEmpty(custGrpID))
                {
                    throw new ArgumentNullException("FindCustomerId", "Invalid Customer Information");
                }


                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 CustomerGroupID ";
                sqlText += " from CustomerGroups";
                sqlText += " where ";
                sqlText += " CustomerGroupName=@custGrpName ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);

                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValue("@custGrpName", custGrpName);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    sqlText = " ";
                    sqlText = " SELECT top 1 CustomerGroupID ";
                    sqlText += " from CustomerGroups";
                    sqlText += " where ";
                    sqlText += " CustomerGroupID=@custGrpID ";

                    SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                    cmd2.Transaction = transaction;
                    cmd2.Parameters.AddWithValue("@custGrpID", custGrpID);

                    object obj2 = cmd2.ExecuteScalar();
                    if (obj2 == null)
                    {
                        throw new ArgumentNullException("FindCustomerId", "Customer Group '(" + custGrpName + ")' not in database");

                    }
                    else
                    {
                        retResults = obj2.ToString();

                    }

                }
                else
                {
                    retResults = obj1.ToString();

                }




                #endregion Find


            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public string CheckSaleImportIdExist(string ImportId, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string results = "0";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                if (string.IsNullOrEmpty(ImportId))
                {
                    return results;
                }

                if (ImportId == "0")
                {
                    return results;
                }

                #region open connection and transaction
                if (vcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction
                #region Validation

                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 SalesInvoiceNo";
                sqlText += " from SalesInvoiceHeaders";
                sqlText += " where ";
                sqlText += " ImportIDExcel=@ImportId ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValue("@ImportId", ImportId);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 != null)
                {
                    results = "Exist";
                }
                else
                {
                    results = "NotExist";
                }

                #endregion Find

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return results;

            #endregion

        }

        #region Burear 
        
        public string FindCustomerId(string customerName, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }


                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(customerName))
                {
                    throw new ArgumentNullException("FindCustomerId", "Invalid Customer Information");
                }


                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 CustomerID ";
                sqlText += " from Customers";
                sqlText += " where ";
                sqlText += " CustomerName=@customerName ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);

                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValue("@customerName", customerName);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("FindCustomerId", "Customer '(" + customerName + ")' not in database");
                }
                else
                {
                    retResults = obj1.ToString();

                }

                #endregion Find


            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public bool CheckBureauDate(string input)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    Convert.ToDateTime(input);
                    //Convert.ToDateTime("24/9/2013");
                    result = true;

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

        public DataTable GetProductInfo(SqlConnection currConn,SqlTransaction transaction)
        {
            // for TollReceive
            #region Variables

            SqlConnection vcurrConn = currConn;
            string sqlText = "";

            DataTable dataTable = new DataTable("ProductInfo");

            #endregion

            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                        }
                        transaction = currConn.BeginTransaction("Products");                              
                    }
                }


                #endregion open connection and transaction

                #region sql statement

                sqlText = "";
                sqlText = @" Select Top 1 ItemNo,ProductCode,ProductName,Products.SD,Products.UOM from Products,ProductCategories 
                                where Products.CategoryID = ProductCategories.CategoryID 
                                and IsRaw='Service' and CategoryName='Service Non Stock'
                                order by Products.CreatedOn desc";

                 SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                SqlDataAdapter dataAdapt = new SqlDataAdapter(cmd);
                dataAdapt.Fill(dataTable);

                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            #endregion
            #region finally

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            return dataTable;
        }
        

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

        public string FindCurrencyRateFromBDTForBureau(string currencyId,string convertionDate, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string bdtId = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }

                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(currencyId))
                {
                    throw new ArgumentNullException("FindCurrencyRateFromBDT", "Invalid Currency Information");
                }


                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyId ";
                sqlText += " from Currencies";
                sqlText += " where ";
                sqlText += " CurrencyCode='BDT'";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Transaction = transaction;
                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("FindCurrencyRateFromBDT", "Currency 'BDT' not in database");

                }
                else
                {
                    bdtId = obj1.ToString();

                }

                sqlText = " ";
                sqlText = " SELECT top 1 CurrencyRate ";
                sqlText += " from CurrencyConversion";
                sqlText += " where ";
                sqlText += " CurrencyCodeFrom=@currencyId  ";
                sqlText += " and  CurrencyCodeTo=@bdtId ";
                sqlText += " and  ConversionDate<=@convertionDate ";
                sqlText += " order by conversionDate desc "; 

                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                cmd2.Parameters.AddWithValue("@currencyId", currencyId);
                cmd2.Parameters.AddWithValue("@bdtId", bdtId);
                cmd2.Parameters.AddWithValue("@convertionDate", convertionDate);

                object obj2 = cmd2.ExecuteScalar();
                if (obj2 == null)
                {
                    throw new ArgumentNullException("FindCurrencyRateFromBDT", "Currency '(" + currencyId + ")' not in database");

                }
                else
                {
                    retResults = obj2.ToString();

                }








                #endregion Find
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public string FindSaleInvoiceNo(string customerId,string invoiceNo, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection vcurrConn = currConn;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }


                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(customerId))
                {
                    throw new ArgumentNullException("FindCustomerId", "Invalid Customer Information");
                }


                #endregion Validation
                #region Find

                sqlText = " ";
                sqlText = " Select SalesInvoiceNo";
                sqlText += " from BureauSalesInvoiceDetails";
                sqlText += " where ";
                sqlText += " CustomerId=@customerId ";
                sqlText += " and InvoiceName=@invoiceNo ";

                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);

                cmd1.Transaction = transaction;
                cmd1.Parameters.AddWithValue("@customerId", customerId);
                cmd1.Parameters.AddWithValue("@invoiceNo", invoiceNo);

                object obj1 = cmd1.ExecuteScalar();
                if (obj1 == null)
                {
                    throw new ArgumentNullException("Sale information not in database");
                }
                else
                {
                    retResults = obj1.ToString();

                }

                #endregion Find


            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public bool CheckDepositType(string DepositType)
        {
            bool result = true;

            if (string.IsNullOrEmpty(DepositType))
            {
                result = false;
            }
            else
            {
                if (DepositType.ToUpper() == "CASH")
                {
                    result = true;
                }
                else if (DepositType.ToUpper() == "CHEQUE")
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

        public string CheckBankID(string BankName, string BranchName, string AccountNumber, SqlConnection currConn, SqlTransaction transaction)
        {

            SqlConnection vcurrConn = currConn;
            string sqlText = "";
            string result = "";


            DataTable dataTable = new DataTable("Bank");

            try
            {
                #region open connection and transaction

                if (vcurrConn == null)
                {
                    if (currConn == null)
                    {
                        currConn = _dbsqlConnection.GetConnection();
                        if (currConn.State != ConnectionState.Open)
                        {
                            currConn.Open();
                            transaction = currConn.BeginTransaction("Import Data");
                        }
                    }
                }

                #endregion open connection and transaction

                sqlText = @"SELECT 
                            BankID,
                            isnull(BankName,'N/A')BankName,
                            isnull(BranchName,'N/A')BranchName,
                            isnull(AccountNumber,'N/A')AccountNumber
                            FROM dbo.BankInformations
                            WHERE (BankName LIKE '%' + @BankName + '%' OR @BankName IS NULL)
                            AND (BranchName LIKE '%' + @BranchName	 + '%' OR @BranchName	 IS NULL) 
                            AND (AccountNumber LIKE '%' + @AccountNumber	 + '%' OR @AccountNumber	 IS NULL)
                            order by BankName"
                            ;

                SqlCommand objCommBankInformation = new SqlCommand();
                objCommBankInformation.Connection = currConn;
                objCommBankInformation.Transaction = transaction;
                objCommBankInformation.CommandText = sqlText;
                objCommBankInformation.CommandType = CommandType.Text;

                if (!objCommBankInformation.Parameters.Contains("@BankName"))
                { objCommBankInformation.Parameters.AddWithValue("@BankName", BankName); }
                else { objCommBankInformation.Parameters["@BankName"].Value = BankName; }
                if (!objCommBankInformation.Parameters.Contains("@BranchName"))
                { objCommBankInformation.Parameters.AddWithValue("@BranchName", BranchName); }
                else { objCommBankInformation.Parameters["@BranchName"].Value = BranchName; }
                if (!objCommBankInformation.Parameters.Contains("@AccountNumber"))
                { objCommBankInformation.Parameters.AddWithValue("@AccountNumber", AccountNumber); }
                else { objCommBankInformation.Parameters["@AccountNumber"].Value = AccountNumber; }
                

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommBankInformation);

                dataAdapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    result = dataTable.Rows[0]["BankID"].ToString();
                }
            }
            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            finally
            {
                if (vcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            return result;
        }

        #endregion
    }
}
