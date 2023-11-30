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
    public class UOMNameDAL
    {


        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();


        //==================DropDownAll=================
        public List<UOMNameVM> DropDownAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<UOMNameVM> VMs = new List<UOMNameVM>();
            UOMNameVM vm;
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
'B' Sl, UOMId
, UOMCode
FROM UOMName
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' UOMId
, 'ALL UOM' UOMCode  
FROM UOMName
)
AS a
order by Sl,UOMCode
";



                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new UOMNameVM();
                    vm.UOMID = dr["UOMId"].ToString();
                    vm.UOMCode = dr["UOMCode"].ToString();
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
        public List<UOMNameVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<UOMNameVM> VMs = new List<UOMNameVM>();
            UOMNameVM vm;
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
af.UOMCode
FROM UOMName af
WHERE  1=1 AND af.ActiveStatus = 'Y'
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new UOMNameVM();
                    vm.UOMCode = dr["UOMCode"].ToString();
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
        public List<UOMNameVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<UOMNameVM> VMs = new List<UOMNameVM>();
            UOMNameVM vm;
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
 UOMId
,UOMName
,UOMCode
,Comments
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn

FROM UOMName  
WHERE  1=1 AND IsArchive = 0

";


                if (Id > 0)
                {
                    sqlText += @" and UOMId=@UOMId";
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
                    objComm.Parameters.AddWithValue("@UOMId", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new UOMNameVM();
                    vm.UOMID = dr["UOMId"].ToString();
                    vm.UOMName = dr["UOMName"].ToString();
                    vm.UOMCode = dr["UOMCode"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();

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

        //public string[] InsertToUOMName(string UOMName, string UOMCode, string Comments, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string databaseName)
        public string[] InsertToUOMName(UOMNameVM vm)
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

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.UOMName))
                {
                    throw new ArgumentNullException("InsertToUOMName",
                                                    "Please enter UOM name.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToUOMName");

               if (!string.IsNullOrEmpty(vm.UOMCode))
                {


                    sqlText = "select count(UOMCode) from UOMName where  UOMCode=@UOMCode";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@UOMCode", vm.UOMCode);
                    
                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("InsertToUOMName", "UOM Name already exist");
                    }

                }
                #endregion
                #region Insert UOM Name

               sqlText = "select count(distinct UOMName) from UOMName where  UOMName=@UOMName";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@UOMName", vm.UOMName);

                int countName = (int)cmdNameExist.ExecuteScalar();
                if (countName > 0)
                {

                    throw new ArgumentNullException("InsertToUOMName",
                                                    "Requested UOM Name  is already exist");
                }

                sqlText = "select count(distinct UOMCode) from UOMName where  UOMCode=@UOMCode";
                SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                cmdCodeExist.Transaction = transaction;
                cmdCodeExist.Parameters.AddWithValue("@UOMCode", vm.UOMCode);

                int countCode = (int)cmdCodeExist.ExecuteScalar();
                if (countCode > 0)
                {

                    throw new ArgumentNullException("InsertToUOMName",
                                                    "Requested UOM Code  is already exist");
                }


                

                sqlText = "";
                sqlText += "insert into UOMName";
                sqlText += "(";
                sqlText += "UOMName,";
                sqlText += "UOMCode,";
               sqlText += "Comments,";
               sqlText += "ActiveStatus,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@UOMName,";
                sqlText += "@UOMCode,";
                sqlText += "@Comments,";
                sqlText += "@ActiveStatus,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn";
                sqlText += ")";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@UOMName", vm.UOMName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@UOMCode", vm.UOMCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Insert Currency Information
                #region Return ID

                sqlText = "select distinct UOMId from UOMName where  UOMCode=@UOMCode ";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                cmdNextId.Parameters.AddWithValue("@UOMCode", vm.UOMCode);

                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new UOM Id";
                    throw new ArgumentNullException("InsertToUOMName",
                                                    "Unable to create new UOM Id");
                }
                #endregion Return ID
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
                retResults[1] = "Requested UOM Name successfully added";
                retResults[2] = "" + nextId;

            }
            #region Catch
            catch (Exception ex)
            {
                int nextId = 0;

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


        //public string[] UpdateUOMName(int UOMId, string UOMName, string UOMCode, string Comments, string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string databaseName)
        public string[] UpdateUOMName(UOMNameVM vm)
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

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.UOMName))
                {
                    throw new ArgumentNullException("InsertToCurrencyInformation",
                                                    "Please enter Currency name.");
                }
                else if (string.IsNullOrEmpty(vm.UOMCode))
                {
                    throw new ArgumentNullException("InsertToCurrencyInformation",
                                                    "Please enter Currency Code.");
                }

                else if (string.IsNullOrEmpty(vm.UOMID.ToString()))
                {
                    throw new ArgumentNullException("InsertToCurrencyInformation",
                                                   "Please enter Currency .");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToUOMName");

                #endregion open connection and transaction


                if (!string.IsNullOrEmpty(vm.UOMCode))
                {


                    sqlText = "select count(UOMCode) from UOMName where  UOMCode=@UOMCode and UOMId <>@UOMId ";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@UOMCode", vm.UOMCode);
                    cmdIdExist.Parameters.AddWithValue("@UOMId", vm.UOMID);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("InsertToUOMName", "requested UOM Code already exist");
                    }

                }

                if (!string.IsNullOrEmpty(vm.UOMName))
                {


                    sqlText = "select count(UOMName) from UOMName where  UOMName=@UOMName and UOMId <>@UOMId ";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@UOMName", vm.UOMName);
                    cmdIdExist.Parameters.AddWithValue("@UOMId", vm.UOMID);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("InsertToUOMName", "requested UOM Name already exist");
                    }

                }

                #region Update UOM Name



                sqlText = "";
                sqlText += "UPDATE UOMName SET ";
                sqlText += " UOMName        =@UOMName,";
                sqlText += " UOMCode        =@UOMCode,";
                sqlText += " Comments       =@Comments,";
                sqlText += " ActiveStatus   =@ActiveStatus,";
                sqlText += " LastModifiedBy =@LastModifiedBy,";
                sqlText += " LastModifiedOn =@LastModifiedOn";
                sqlText += " where UOMId    =@UOMId";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@UOMName", vm.UOMName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@UOMCode", vm.UOMCode ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@UOMId", vm.UOMID);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Update UOM Name


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested UOM Name successfully Updated";

                    }

                }

                #endregion Commit

            }
            #region Catch
            catch (Exception ex)
            {
                int nextId = 0;

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

        public string[] DeleteUOMName(int UOMId, string databaseName)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = UOMId.ToString();

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(UOMId.ToString()))
                {
                    throw new ArgumentNullException("InsertToUOMName",
                                "Could not find requested UOM Code.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = "select count(UOMId) from UOMName where UOMId=@UOMId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@UOMId", UOMId);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested UOM Name.";
                    return retResults;
                }

                sqlText = "delete UOMName where UOMId=@UOMId";

                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@UOMId", UOMId);

                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested UOM Name successfully deleted";
                }


            }
            #region Catch
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

        public string[] Delete(UOMNameVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteUOM"; //Method Name
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
                        sqlText = "update UOMName set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where UOMId=@UOMId";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@UOMId", Convert.ToInt32(ids[i]));
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
                        throw new ArgumentNullException("UOM Delete", vm.UOMID + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("UOM Information Delete", "Could not found any item.");
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


        public DataTable SearchUOMName(string UOMName, string UOMCode, string ActiveStatus)
        {
            string[] retResults = new string[2];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("UOMName");

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = @"           
                            SELECT isnull(NULLIF(c.UOMId,''),0)UOMId, 
                            isnull(NULLIF(c.UOMName,''),'')UOMName,
                            isnull(NULLIF(c.UOMCode,''),'')UOMCode,
                            isnull(NULLIF(c.Comments,''),'')Comments,
                            isnull(NULLIF(c.ActiveStatus,''),'')ActiveStatus
                            FROM UOMName c
                 
                            WHERE 
                                (UOMName  LIKE '%' +  @UOMName  + '%' OR @UOMName IS NULL) 
                            AND (UOMCode LIKE '%' + @UOMCode + '%' OR @UOMCode IS NULL)
                            AND (c.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                            order by UOMCode 
                            ";
                SqlCommand objCommProductCategory = new SqlCommand();
                objCommProductCategory.Connection = currConn;

                objCommProductCategory.CommandText = sqlText;
                objCommProductCategory.CommandType = CommandType.Text;


                if (!objCommProductCategory.Parameters.Contains("@UOMName"))
                { objCommProductCategory.Parameters.AddWithValue("@UOMName", UOMName); }
                else { objCommProductCategory.Parameters["@UOMName"].Value = UOMName; }
                if (!objCommProductCategory.Parameters.Contains("@UOMCode"))
                { objCommProductCategory.Parameters.AddWithValue("@UOMCode", UOMCode); }
                else { objCommProductCategory.Parameters["@UOMCode"].Value = UOMCode; }
                if (!objCommProductCategory.Parameters.Contains("@ActiveStatus"))
                { objCommProductCategory.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommProductCategory.Parameters["@ActiveStatus"].Value = ActiveStatus; }
                // Common Filed
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductCategory);

                dataAdapter.Fill(dataTable);
            }
            #region Catch
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
