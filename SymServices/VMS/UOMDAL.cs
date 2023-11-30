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
    public class UOMDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        public List<UOMConversionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<UOMConversionVM> VMs = new List<UOMConversionVM>();
            UOMConversionVM vm;
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
,UOMFrom
,UOMTo
,Convertion
,CTypes
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,ActiveStatus

FROM UOMs
WHERE  1=1 AND IsArchive =0
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
                    vm = new UOMConversionVM();
                    vm.UOMId = dr["UOMId"].ToString();
                    vm.UOMFrom = dr["UOMFrom"].ToString();
                    vm.UOMTo = dr["UOMTo"].ToString();
                    vm.Convertion = Convert.ToDecimal(dr["Convertion"]);
                    vm.CTypes = dr["CTypes"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();

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

        public string[] Delete(UOMConversionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                        sqlText = "update UOMs set";
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
                        throw new ArgumentNullException("Currency Delete", vm.UOMId + " could not Delete.");
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


        #region Insert
        //public string[] InsertToUOMNew(string UOMId, string UOMFrom, string UOMTo, string Conversion, string CTypes,
        //     string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string ActiveStatus, string databaseName)
        public string[] InsertToUOMNew(UOMConversionVM vm)
        {
            #region Objects & Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

                int nextId = 0;
            #endregion

            #region try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.UOMFrom))
                {
                    throw new ArgumentNullException("Insert To UOM", "Please enter a UOM Conversion From Text.");
                }
                if (string.IsNullOrEmpty(vm.UOMTo))
                {
                    throw new ArgumentNullException("Insert To UOM", "Please enter a UOM Conversion To Text.");
                }
                //if (string.IsNullOrEmpty(vm.Conversion))
                //{
                //    throw new ArgumentNullException("Insert To UOM", "Please enter a UOM Conversion Ratio Text.");
                //}
                //if (string.IsNullOrEmpty(CTypes))
                //{
                //    throw new ArgumentNullException("Insert To UOM", "Please enter a UOM Conversion Type.");
                //}

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UOMTransaction");

                #endregion open connection and transaction

                #region id existence checking

                if (!string.IsNullOrEmpty(vm.UOMId))
                {
                    sqlText = "select count(UOMId) from UOMs where  UOMId =@UOMId ";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@UOMId", vm.UOMId);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("Insert To UOM", "Information Information already exist");
                    }
                }

                #endregion

                #region name existence checking

                sqlText = "";
                sqlText += "SELECT COUNT(DISTINCT UOMId) FROM UOMs WHERE UOMFrom =";
                sqlText += "@UOMFrom AND UOMTo =";
                sqlText += "@UOMTo ";

                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@UOMFrom", vm.UOMFrom);
                cmdNameExist.Parameters.AddWithValue("@UOMTo", vm.UOMTo);

                int countName = (int)cmdNameExist.ExecuteScalar();
                if (countName > 0)
                {
                    throw new ArgumentNullException("Insert To UOM Information",
                                                    "Requested UOM Information already exists");
                }
                #endregion

                #region new id generation
                sqlText = "select isnull(max(cast(UOMId as int)),0)+1 FROM  UOMs";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                nextId = Convert.ToInt32(cmdNextId.ExecuteScalar());
                if (nextId <= 0)
                {
                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Overhead information Id";
                    throw new ArgumentNullException("Insert To UOM Information",
                                                    "Unable to create new UOM information Id");
                }
                #endregion

                #region Insert new row to table

                #region sql statement
                sqlText = "";
                sqlText += "insert into UOMs";
                sqlText += "(";
                sqlText += "UOMId,";
                sqlText += "UOMFrom,";
                sqlText += "UOMTo,";
                sqlText += "Convertion,";
                sqlText += "CTypes,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn,";
                sqlText += "ActiveStatus";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@nextId,";
                sqlText += "@UOMFrom,";
                sqlText += "@UOMTo,";
                sqlText += "@Conversion,";
                sqlText += "@CTypes,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn,";
                sqlText += "@ActiveStatus";
                sqlText += ")";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                cmdInsert.Parameters.AddWithValue("@UOMFrom", vm.UOMFrom);
                cmdInsert.Parameters.AddWithValue("@UOMTo", vm.UOMTo);
                cmdInsert.Parameters.AddWithValue("@Conversion", vm.Convertion);
                cmdInsert.Parameters.AddWithValue("@CTypes", vm.CTypes ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                #endregion

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested UOM Information successfully Added";
                        retResults[2] = "" + nextId;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to add UOM";
                        retResults[2] = "";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to add UOM";
                    retResults[2] = "";
                }
                #endregion Commit

                #endregion
            }
            #endregion try

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

            #region Finally

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

            return retResults;
        }

       
        #endregion

        #region Update
        //public string[] UpdateUOM(string UOMId, string UOMFrom, string UOMTo, string Conversion, string CTypes,
        //     string LastModifiedBy, string LastModifiedOn, string ActiveStatus, string databaseName)
        public string[] UpdateUOM(UOMConversionVM vm)
        {
            #region Objects & Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.UOMId;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

                int nextId = 0;
            #endregion

            #region try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.UOMId))
                {
                    throw new ArgumentNullException("Update UOM Information", "Invalid UOM Id.");
                }

                if (string.IsNullOrEmpty(vm.UOMFrom))
                {
                    throw new ArgumentNullException("Update UOM Information", "Please enter a UOM Conversion From Text.");
                }
                if (string.IsNullOrEmpty(vm.UOMTo))
                {
                    throw new ArgumentNullException("Update UOM Information", "Please enter a UOM Conversion To Text.");
                }
                //if (string.IsNullOrEmpty(vm.Conversion))
                //{
                //    throw new ArgumentNullException("Update UOM Information", "Please enter a UOM Conversion Ratio Text.");
                //}

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UOMTransaction");

                #endregion open connection and transaction

                #region id existence checking

                if (!string.IsNullOrEmpty(vm.UOMId))
                {
                    sqlText = "select count(UOMId) from UOMs where  UOMId =@UOMId ";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@UOMId", vm.UOMId);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("Update UOM Information", "UOM Information already exist");
                    }
                }

                #endregion

                #region name existence checking

                sqlText = "";
                sqlText += "SELECT COUNT(DISTINCT UOMId) FROM UOMs WHERE UOMFrom = @UOMFrom AND UOMTo =@UOMTo ";
                sqlText += " and not UOMId =@UOMId";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@UOMFrom", vm.UOMFrom);
                cmdNameExist.Parameters.AddWithValue("@UOMTo", vm.UOMTo);
                cmdNameExist.Parameters.AddWithValue("@UOMId", vm.UOMId);

                int countName = (int)cmdNameExist.ExecuteScalar();
                if (countName > 0)
                {
                    throw new ArgumentNullException("Update UOM Information",
                                                    "Requested UOM Information already exists");
                }
                #endregion

                #region new id generation
                //sqlText = "select isnull(max(cast(UOMId as int)),0)+1 FROM  UOMs";
                //SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                //cmdNextId.Transaction = transaction;
                //int nextId = (int)cmdNextId.ExecuteScalar();
                //if (nextId <= 0)
                //{
                //    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                //    retResults[0] = "Fail";
                //    retResults[1] = "Unable to create new Overhead information Id";
                //    throw new ArgumentNullException("Insert To UOM Information",
                //                                    "Unable to create new UOM information Id");
                //}
                #endregion

                #region Update new row to table

                #region sql statement
                sqlText = "";
                sqlText = "update UOMs set";
                sqlText += " UOMFrom        =@UOMFrom,";
                sqlText += " UOMTo          =@UOMTo,";
                sqlText += " Convertion     =@Conversion,";
                sqlText += " CTypes         =@CTypes,";
                sqlText += " LastModifiedBy =@LastModifiedBy,";
                sqlText += " LastModifiedOn =@LastModifiedOn,";
                sqlText += " ActiveStatus   =@ActiveStatus";
                sqlText += " where UOMId    =@UOMId";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@UOMFrom", vm.UOMFrom);
                cmdInsert.Parameters.AddWithValue("@UOMTo", vm.UOMTo);
                cmdInsert.Parameters.AddWithValue("@Conversion", vm.Convertion);
                cmdInsert.Parameters.AddWithValue("@CTypes", vm.CTypes ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@UOMId", vm.UOMId ?? Convert.DBNull);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                #endregion

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested UOM Information Updated successfully !";
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update UOM";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Update UOM";
                    retResults[2] = "";
                }
                #endregion Commit

                #endregion
            }
            #endregion try

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

            #region Finally

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

            return retResults;
        }

        #endregion

        #region Delete

        public string[] DeleteUOMNew(string UOMId, string databaseName)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = UOMId;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(UOMId))
                {
                    throw new ArgumentNullException("Delete UOM Information",
                                "Could not find requested UOM Id.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQL Statements
                sqlText = "select count(UOMId) from UOMs where UOMId=@UOMId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@UOMId", UOMId);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested UOM Information.";
                    return retResults;
                }

                sqlText = "delete UOMs where UOMId=@UOMId ";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@UOMId", UOMId);

                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested UOM Information successfully deleted";
                }
                #endregion
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
        #endregion

        #region Search

        //commented for error resolve

        public DataTable SearchUOMNew2(string UOMId, string UOMFrom, string UOMTo, string ActiveStatus, string databaseName)
        {

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dt = new DataTable("UOM");
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQL Statement

                sqlText = @"
                select DISTINCT UOMId, UOMFrom, UOMTo, Convertion, CTypes, ActiveStatus
                FROM UOMs	
                WHERE 	(UOMId  LIKE '%' +  @UOMId + '%' OR @UOMId IS NULL) 
                and (UOMFrom LIKE '%' + @UOMFrom + '%' OR @UOMFrom IS NULL)	
                and (UOMTo LIKE '%' + @UOMTo + '%' OR @UOMTo IS NULL)
                and (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
	            order by UOMFrom   

                SELECT DISTINCT UOMName,u.UOMCode FROM UOMName u ORDER BY u.UOMCode

";
                SqlCommand objCommPUOM = new SqlCommand();
                objCommPUOM.Connection = currConn;
                objCommPUOM.CommandText = sqlText;
                objCommPUOM.CommandType = CommandType.Text;

                if (!objCommPUOM.Parameters.Contains("@UOMId"))
                {
                    objCommPUOM.Parameters.AddWithValue("@UOMId", UOMId);
                }
                else
                {
                    objCommPUOM.Parameters["@UOMId"].Value = UOMId;
                }
                if (!objCommPUOM.Parameters.Contains("@UOMFrom"))
                {
                    objCommPUOM.Parameters.AddWithValue("@UOMFrom", UOMFrom);
                }
                else
                {
                    objCommPUOM.Parameters["@UOMFrom"].Value = UOMFrom;
                }
                if (!objCommPUOM.Parameters.Contains("@ActiveStatus"))
                {
                    objCommPUOM.Parameters.AddWithValue("@ActiveStatus", ActiveStatus);
                }
                else
                {
                    objCommPUOM.Parameters["@ActiveStatus"].Value = ActiveStatus;
                }
                if (!objCommPUOM.Parameters.Contains("@UOMTo"))
                {
                    objCommPUOM.Parameters.AddWithValue("@UOMTo", UOMTo);
                }
                else
                {
                    objCommPUOM.Parameters["@UOMTo"].Value = UOMTo;
                }
                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommPUOM);
                dataAdapter.Fill(dt);
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
            return dt;
        }
        public DataTable SearchUOM(string UOMId, string UOMFrom, string UOMTo, string ActiveStatus, string databaseName)
        {

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("UOM");
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = @"select DISTINCT UOMId,
                            UOMFrom,
                            UOMTo,
                            Convertion,
                            CTypes,ActiveStatus
                            FROM UOMs	
                            WHERE 	(UOMId  LIKE '%' +  @UOMId + '%' OR @UOMId IS NULL) 
                            and (UOMFrom LIKE '%' + @UOMFrom + '%' OR @UOMFrom IS NULL)	
                            and (UOMTo LIKE '%' + @UOMTo + '%' OR @UOMTo IS NULL)
                            and (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                            order by UOMFrom";

                SqlCommand objCommPUOM = new SqlCommand();
                objCommPUOM.Connection = currConn;
                objCommPUOM.CommandText = sqlText;
                objCommPUOM.CommandType = CommandType.Text;


                if (!objCommPUOM.Parameters.Contains("@UOMId"))
                {
                    objCommPUOM.Parameters.AddWithValue("@UOMId", UOMId);
                }
                else
                {
                    objCommPUOM.Parameters["@UOMId"].Value = UOMId;
                }
                if (!objCommPUOM.Parameters.Contains("@UOMFrom"))
                {
                    objCommPUOM.Parameters.AddWithValue("@UOMFrom", UOMFrom);
                }
                else
                {
                    objCommPUOM.Parameters["@UOMFrom"].Value = UOMFrom;
                }

                if (!objCommPUOM.Parameters.Contains("@ActiveStatus"))
                {
                    objCommPUOM.Parameters.AddWithValue("@ActiveStatus", ActiveStatus);
                }
                else
                {
                    objCommPUOM.Parameters["@ActiveStatus"].Value = ActiveStatus;
                }
                if (!objCommPUOM.Parameters.Contains("@UOMTo"))
                {
                    objCommPUOM.Parameters.AddWithValue("@UOMTo", UOMTo);
                }
                else
                {
                    objCommPUOM.Parameters["@UOMTo"].Value = UOMTo;
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommPUOM);

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
        public DataTable SearchUOMCodeOnly(string ActiveStatus)
        {

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("UOM");
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = @" SELECT DISTINCT UOMCode FROM UOMName
                            WHERE  (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                            order by UOMCode";

                SqlCommand objCommPUOM = new SqlCommand();
                objCommPUOM.Connection = currConn;
                objCommPUOM.CommandText = sqlText;
                objCommPUOM.CommandType = CommandType.Text;


               
                if (!objCommPUOM.Parameters.Contains("@ActiveStatus"))
                {
                    objCommPUOM.Parameters.AddWithValue("@ActiveStatus", ActiveStatus);
                }
                else
                {
                    objCommPUOM.Parameters["@ActiveStatus"].Value = ActiveStatus;
                }
              

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommPUOM);

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

        public decimal GetConvertionRate(string UOMFrom, string UOMTo, string ActiveStatus, SqlConnection currConn, SqlTransaction transaction)
        {

            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            decimal convertionValue = 0;
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                    transaction = currConn.BeginTransaction("AvgPprice");
                }
                if (UOMFrom == UOMTo)
                {
                    convertionValue = 1;
                }
                else
                {

                    sqlText = @"select DISTINCT Convertion FROM UOMs";
                    sqlText += " WHERE UOMFrom =@UOMFrom";
                    sqlText += "and UOMTo =@UOMTo";
                    sqlText += "and ActiveStatus @ActiveStatus";

                    SqlCommand objCommPUOM = new SqlCommand(sqlText, currConn,transaction);
                    //objCommPUOM.Transaction = transaction;
                    //IDExist = (int)cmdExistTran.ExecuteScalar();
                    objCommPUOM.Parameters.AddWithValue("@UOMFrom", UOMFrom);
                    objCommPUOM.Parameters.AddWithValue("@UOMTo", UOMTo);
                    objCommPUOM.Parameters.AddWithValue("@ActiveStatus", ActiveStatus);

                    object objConvertion = objCommPUOM.ExecuteScalar();

                    if (objConvertion != null)
                    {
                        convertionValue = Convert.ToDecimal(objConvertion);
                    }
                    else
                    {
                        throw new ArgumentNullException("GetConvertionRate",
                                                        string.Format(
                                                            "Unable to find Convertion Rate from {0} to {1}.", UOMFrom,
                                                            UOMTo));
                    }

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

            return convertionValue;
        }
        #endregion
      
    }
}
