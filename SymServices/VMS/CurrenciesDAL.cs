using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymOrdinary;
using SymphonySofttech.Utilities;
using SymViewModel.VMS;
using System.Reflection;


namespace SymServices.VMS
{
    public class CurrenciesDAL
    {

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        //==================DropDownAll=================
        public List<CurrencyVM> DropDownAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CurrencyVM> VMs = new List<CurrencyVM>();
            CurrencyVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT * FROM(
SELECT 
'B' Sl, CurrencyId
, CurrencyName
FROM Currencies
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' Id
, 'ALL Currency' CurrencyName  
FROM Currencies
)
AS a
order by Sl,CurrencyName
";



                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CurrencyVM();
                    vm.CurrencyId = dr["CurrencyId"].ToString(); ;
                    vm.CurrencyName = dr["CurrencyName"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }


        //==================DropDown=================
        public List<CurrencyVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CurrencyVM> VMs = new List<CurrencyVM>();
            CurrencyVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT
af.CurrencyId
,af.CurrencyCode
FROM Currencies af
WHERE  1=1 AND af.ActiveStatus = 'Y' order by af.CurrencyCode
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CurrencyVM();
                    vm.CurrencyId = dr["CurrencyId"].ToString();
                    vm.CurrencyCode = dr["CurrencyCode"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }

        //==================SelectAll=================
        public List<CurrencyVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CurrencyVM> VMs = new List<CurrencyVM>();
            CurrencyVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region sql statement
                #region SqlText

                sqlText = @"
SELECT
 CurrencyId
,CurrencyName
,CurrencyCode
,Country
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,CurrencyMajor
,CurrencyMinor
,CurrencySymbol

FROM Currencies  
WHERE  1=1 AND IsArchive = 0

";


                if (Id > 0)
                {
                    sqlText += @" and CurrencyId=@CurrencyId";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@CurrencyId", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CurrencyVM();
                    vm.CurrencyId = dr["CurrencyId"].ToString();
                    vm.CurrencyName = dr["CurrencyName"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn =dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.CurrencyCode = dr["CurrencyCode"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.CurrencyMajor = dr["CurrencyMajor"].ToString();
                    vm.CurrencyMinor = dr["CurrencyMinor"].ToString();
                    vm.CurrencySymbol = dr["CurrencySymbol"].ToString();

                    VMs.Add(vm);
                }
                dr.Close();

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return VMs;
        }

        //==================Insert Currency=================

        //public string[] InsertToCurrency(string CurrencyName, string CurrencyCode, string Country, string Comments, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string databaseName)
        public string[] InsertToCurrency(CurrencyVM vm)
        {

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[1] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            int nextId = 0;

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.CurrencyName))
                {
                    throw new ArgumentNullException("InsertToCurrencyInformation",
                                                    "Please enter Currency name.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("CurrencyConversion", "ConversionDate", "datetime", currConn);



                transaction = currConn.BeginTransaction("InsertToCurrencyInformation");

                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(vm.CurrencyCode))
                {


                    sqlText = "select count(CurrencyCode) from Currencies where  CurrencyCode=@CurrencyCode";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@CurrencyCode", vm.CurrencyCode);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("InsertToCurrencyInformation", "Currency information already exist");
                    }

                }

                #region Insert Currency Information

                sqlText = "select count(distinct CurrencyName) from Currencies where  CurrencyName=@CurrencyName";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@CurrencyName", vm.CurrencyName);
                int countName = (int)cmdNameExist.ExecuteScalar();
                if (countName > 0)
                {

                    throw new ArgumentNullException("InsertToCurrencyInformation",
                                                    "Requested Currency Name  is already exist");
                }


                sqlText = @" 
INSERT INTO Currencies(
CurrencyName
,CurrencyCode
,Country
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn

)  VALUES (
 @CurrencyName
,@CurrencyCode
,@Country
,@Comments
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn     
) ;SELECT SCOPE_IDENTITY();
";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@CurrencyName", vm.CurrencyName);
                cmdInsert.Parameters.AddWithValue("@CurrencyCode", vm.CurrencyCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@CurrencyMajor", vm.CurrencyMajor ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CurrencyMinor", vm.CurrencyMinor ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CurrencySymbol", vm.CurrencySymbol ?? Convert.DBNull);

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());

                nextId = transResult;

                #endregion Insert Currency Information


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();

                    }

                }

                #endregion Commit

                retResults[0] = "Success";
                retResults[1] = "Requested Currency Information successfully added";
                retResults[2] = "" + nextId;
            }
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
            }
            #endregion
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {

                        currConn.Close();

                    }
                }

            }

            return retResults;

        }


        //public string[] UpdateCurrency(int CurrencyId, string CurrencyName, string CurrencyCode, string Country, string Comments, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string databaseName)
        public string[] UpdateCurrency(CurrencyVM vm)
        {

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[1] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            int nextId = 0;
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.CurrencyName))
                {
                    throw new ArgumentNullException("InsertToCurrencyInformation",
                                                    "Please enter Currency name.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("CurrencyConversion", "ConversionDate", "datetime", currConn);

                transaction = currConn.BeginTransaction("InsertToCurrencyInformation");

                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(vm.CurrencyCode))
                {


                    sqlText = "select count(CurrencyCode) from Currencies where  CurrencyCode=@CurrencyCode";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@CurrencyCode", vm.CurrencyCode);
                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("InsertToCategoryInformation", "Could not find requested Currency information ");
                    }

                }

                #region Update Currency Information


                sqlText = "";


                sqlText = "UPDATE Currencies SET ";

                sqlText += " CurrencyName   =@CurrencyName";
                sqlText += ",CurrencyCode   =@CurrencyCode";
                sqlText += ",Country        =@Country";
                sqlText += ",Comments       =@Comments";
                sqlText += ",ActiveStatus   =@ActiveStatus";
                sqlText += ",LastModifiedBy =@LastModifiedBy";
                sqlText += ",LastModifiedOn =@LastModifiedOn";
                sqlText += ",CurrencyMajor  =@CurrencyMajor";
                sqlText += ",CurrencyMinor  =@CurrencyMinor";
                sqlText += ",CurrencySymbol =@CurrencySymbol";
                sqlText += " WHERE CurrencyId=@CurrencyId";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@CurrencyName", vm.CurrencyName);
                cmdInsert.Parameters.AddWithValue("@CurrencyCode", vm.CurrencyCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@CurrencyMajor", vm.CurrencyMajor ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CurrencyMinor", vm.CurrencyMinor ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CurrencySymbol", vm.CurrencySymbol ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CurrencyId", vm.CurrencyId ?? Convert.DBNull);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Update Currency Information


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Currency Information successfully Updated";
                        retResults[2] = vm.CurrencyId;

                    }

                }

                #endregion Commit


                // retResults[2] = "" + nextId;
            }
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                transaction.Rollback();

                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
            }
            #endregion
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {

                        currConn.Close();

                    }
                }

            }

            return retResults;
        }



        public string[] DeleteCurrencyInformation(int CurrencyId, string databaseName)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = CurrencyId.ToString();

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(CurrencyId.ToString()))
                {
                    throw new ArgumentNullException("InsertToCurrencyInformation",
                                "Could not find requested Currency Code.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("CurrencyConversion", "ConversionDate", "datetime", currConn);
                #endregion open connection and transaction

                sqlText = "select count(CurrencyId) from Currencies where CurrencyId=@CurrencyId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@CurrencyId", CurrencyId);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Currency Information.";
                    return retResults;
                }

                sqlText = "delete Currencies where CurrencyId=@CurrencyId";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@CurrencyId", CurrencyId);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Currency Information successfully deleted";
                }


            }
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {

                        currConn.Close();

                    }
                }

            }

            return retResults;
        }

        public string[] Delete(CurrencyVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteCurrency"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string retVal = "";
            #endregion
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {

                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update Currencies set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where CurrencyId=@CurrencyId";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@CurrencyId", Convert.ToInt32(ids[i]));
                        cmdUpdate.Parameters.AddWithValue("@ActiveStatus", "N");
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Currency Delete", vm.CurrencyId + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Currency Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }


        public DataTable SearchCurrencies(string CurrencyName, string CurrencyCode, string Country, string ActiveStatus)
        {
            string[] retResults = new string[2];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Currencies");

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("CurrencyConversion", "ConversionDate", "datetime", currConn);
                #endregion open connection and transaction

                sqlText = @"           
                            SELECT isnull(NULLIF(c.CurrencyId,''),0)CurrencyId, 
                            isnull(NULLIF(c.CurrencyName,''),'')CurrencyName,
                            isnull(NULLIF(c.CurrencyCode,''),'')CurrencyCode,
                            isnull(NULLIF(c.Country,''),'')Country,
                            isnull(NULLIF(c.Comments,''),'')Comments,
                            isnull(NULLIF(c.ActiveStatus,''),'')ActiveStatus
                            FROM Currencies c
                 
                            WHERE 
                                (CurrencyName  LIKE '%' +  @CurrencyName  + '%' OR @CurrencyName IS NULL) 
                            AND (CurrencyCode LIKE '%' + @CurrencyCode + '%' OR @CurrencyCode IS NULL)
                            AND (Country LIKE '%' + @Country + '%' OR @Country IS NULL)
                            AND (c.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                            order by CurrencyCode 
                            ";
                SqlCommand objCommProductCategory = new SqlCommand();
                objCommProductCategory.Connection = currConn;

                objCommProductCategory.CommandText = sqlText;
                objCommProductCategory.CommandType = CommandType.Text;


                if (!objCommProductCategory.Parameters.Contains("@CurrencyName"))
                { objCommProductCategory.Parameters.AddWithValue("@CurrencyName", CurrencyName); }
                else { objCommProductCategory.Parameters["@CurrencyName"].Value = CurrencyName; }
                if (!objCommProductCategory.Parameters.Contains("@CurrencyCode"))
                { objCommProductCategory.Parameters.AddWithValue("@CurrencyCode", CurrencyCode); }
                else { objCommProductCategory.Parameters["@CurrencyCode"].Value = CurrencyCode; }
                if (!objCommProductCategory.Parameters.Contains("@Country"))
                { objCommProductCategory.Parameters.AddWithValue("@Country", Country); }
                else { objCommProductCategory.Parameters["@Country"].Value = Country; }

                if (!objCommProductCategory.Parameters.Contains("@ActiveStatus"))
                { objCommProductCategory.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommProductCategory.Parameters["@ActiveStatus"].Value = ActiveStatus; }
                // Common Filed
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductCategory);

                dataAdapter.Fill(dataTable);
            }
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {

                        currConn.Close();

                    }
                }

            }

            return dataTable;

        }

        //------------------
        public DataTable SearchCurrency(string customer)
        {
            #region Objects & Variables

            SqlConnection currConn = null;

            string sqlText = "";

            DataTable dataTable = new DataTable();
            #endregion
            #region try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();


                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("CurrencyConversion", "ConversionDate", "datetime", currConn);
                #endregion open connection and transaction
                #region sql statement


                sqlText = @"

SELECT [SalesInvoiceNo]
	  ,isnull(sum([SubTotal]),0)SubTotal
      ,isnull(sum([CurrencyValue]),0)CurrencyValue
  FROM SalesInvoiceDetails where [SalesInvoiceNo] =@customer group by [SalesInvoiceNo]

";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                if (!objCommProductType.Parameters.Contains("@customer"))
                { objCommProductType.Parameters.AddWithValue("@customer", customer); }
                else { objCommProductType.Parameters["@customer"].Value = customer; }


                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
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
                if (currConn != null)
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
        //------------------
    }
}
