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

namespace SymServices.VMS
{
    public class UserInformationDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        #endregion

        #region New Methods

        //==================SelectAll=================
        public List<UserInformationVM> SelectForLogin(LoginVM Logvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<UserInformationVM> VMs = new List<UserInformationVM>();
            UserInformationVM vm;
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

                    currConn = _dbsqlConnection.GetConnection(Logvm.DatabaseName);
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
    SELECT top 1
    *

    FROM UserInformations 
    WHERE  1=1    and ActiveStatus='Y' and   UserName=@UserName   and UserPassword=@UserPassword

";
                
 
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                objComm.Parameters.AddWithValue("@UserName", Logvm.UserName);
                Logvm.UserPassword = Converter.DESEncrypt(PassPhrase, EnKey, Logvm.UserPassword);

                objComm.Parameters.AddWithValue("@UserPassword", Logvm.UserPassword);
               
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new UserInformationVM();
                    vm.UserID = dr["UserID"].ToString();
                    vm.UserName = dr["UserName"].ToString();
                    vm.UserPassword = dr["UserPassword"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString() == "Y" ? true : false;
                    vm.IsAdmin = dr["IsAdmin"].ToString() == "Y" ? true : false;

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

        public List<UserInformationVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<UserInformationVM> VMs = new List<UserInformationVM>();
            UserInformationVM vm;
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
 ui.UserID
,ui.UserName
,ui.UserPassword
,ui.ActiveStatus
,ui.LastLoginDateTime
,ui.CreatedBy
,ui.CreatedOn
,ui.LastModifiedBy
,ui.LastModifiedOn
,ui.GroupID
,ug.GroupName

FROM UserInformations ui left outer join UserGroups ug on ui.GroupID=ug.GroupID
WHERE  1=1 

";
                if (Id > 0)
                {
                    sqlText += @" and UserID=@UserID";
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
                    objComm.Parameters.AddWithValue("@UserID", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new UserInformationVM();
                    vm.UserID = dr["UserID"].ToString();
                    vm.UserName = dr["UserName"].ToString();
                    vm.UserPassword = dr["UserPassword"].ToString();
                    vm.ActiveStatus = dr["ActiveStatus"].ToString() == "Y" ? true : false;
                    vm.LastLoginDateTime = dr["LastLoginDateTime"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.GroupID = dr["GroupID"].ToString();
                    vm.GroupName = dr["GroupName"].ToString();

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

        public string[] InsertToUserInformationNew(UserInformationVM vm)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion
            
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.UserName))
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Please enter user name.");
                }
                if (string.IsNullOrEmpty(vm.UserPassword))
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Please enter user password.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToUserInformationNew");

                #endregion open connection and transaction

                #region UserName existence checking

                //select @Present = count(distinct UserName) from UserInformations where  UserName=@UserName;
                sqlText = "select count(distinct UserName) from UserInformations where  UserName =@UserName ";
                SqlCommand userNameExist = new SqlCommand(sqlText, currConn);
                userNameExist.Transaction = transaction;
                userNameExist.Parameters.AddWithValue("@UserName", vm.UserName);

                countId = (int)userNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Same user name already exist.");
                }

                #endregion UserName existence checking

                #region User new id generation

                //select @UserID= isnull(max(cast(UserID as int)),0)+1 FROM  UserInformations;
                sqlText = "select isnull(max(cast(UserID as int)),0)+1 FROM  UserInformations";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("Insert To User Information New", "Unable to create new user");
                }

                #endregion User new id generation

                #region Insert new user

                sqlText = "";
                sqlText += "insert into UserInformations";
                sqlText += "(";
                sqlText += "UserID,";
                sqlText += "UserName,";
                sqlText += "UserPassword,";
                sqlText += "ActiveStatus,";
                sqlText += "LastLoginDateTime,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@nextId,";
                sqlText += "@UserName,";
                sqlText += "@UserPassword,";
                sqlText += "@ActiveStatus,";
                sqlText += "@LastLoginDateTime,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn";
                sqlText += ")";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                cmdInsert.Parameters.AddWithValue("@UserName", vm.UserName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@UserPassword", vm.UserPassword ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus ?"Y":"N");
                cmdInsert.Parameters.AddWithValue("@LastLoginDateTime", vm.LastLoginDateTime ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Information successfully Added.";
                        retResults[2] = "" + nextId;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to add user";
                        retResults[2] = "";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to add user ";
                    retResults[2] = "";
                }

                #endregion Commit

                #endregion Insert new user

            }
            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw sqlex;
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

            #endregion

            return retResults;
        }

        public string InsertUserLogin(string LogID, string ComputerName, string ComputerLoginUserName,
            string ComputerIPAddress, string SoftwareUserId, string SessionDate, string LogInDateTime,
            string LogOutDateTime)
        {
            #region Variables

            string retResults = string.Empty;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                var tt = currConn.Database;
                CommonDAL commonDal = new CommonDAL();
                #region UserLog

                //commonDal.TableAdd("UserAuditLogs", "LogID", "varchar(50)", currConn); //tablename,fieldName, datatype

                commonDal.TableFieldAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerLoginUserName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerIPAddress", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SoftwareUserId", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SessionDate", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogInDateTime", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogOutDateTime", "datetime", currConn, transaction);

                #endregion UserLog

                transaction = currConn.BeginTransaction("InsertToUserInformationNew");

                #endregion open connection and transaction
                sqlText = "";
                sqlText += "  select ISNULL( COUNT (DISTINCT logid),0)logid FROM   UserAuditLogs ";
                sqlText += " WHERE ComputerName         =@ComputerName";
                sqlText += " and ComputerLoginUserName  =@ComputerLoginUserName";
                sqlText += " and ComputerIPAddress      =@ComputerIPAddress";
                sqlText += " and SoftwareUserId         =@SoftwareUserId";
                sqlText += " and SessionDate            =@SessionDate";
                sqlText += " and LogInDateTime          =@LogInDateTime";
                sqlText += " and LogOutDateTime         =@LogOutDateTime";

                SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                cmdFindId.Transaction = transaction;
                cmdFindId.Parameters.AddWithValue("@ComputerName", ComputerName);
                cmdFindId.Parameters.AddWithValue("@ComputerLoginUserName", ComputerLoginUserName);
                cmdFindId.Parameters.AddWithValue("@ComputerIPAddress", ComputerIPAddress);
                cmdFindId.Parameters.AddWithValue("@SoftwareUserId", SoftwareUserId);
                cmdFindId.Parameters.AddWithValue("@SessionDate", SessionDate);
                cmdFindId.Parameters.AddWithValue("@LogInDateTime", LogInDateTime);
                cmdFindId.Parameters.AddWithValue("@LogOutDateTime", LogOutDateTime);

                int IDExist = (int)cmdFindId.ExecuteScalar();

                if (IDExist > 0)//update
                {
                    #region Update

                    sqlText = "";
                    sqlText += " UPDATE UserAuditLogs";
                    sqlText += " SET LogOutDateTime = @LogOutDateTime";
                    sqlText += " WHERE LogID=@LogID";


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@LogOutDateTime", LogOutDateTime);
                    cmdInsert.Parameters.AddWithValue("@LogID", LogID);

                    transResult = (int)cmdInsert.ExecuteNonQuery();

                    #region Commit

                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();

                            retResults = "" + LogID;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults = "0";

                        }

                    }
                    else
                    {
                        retResults = "0";
                    }

                    #endregion Commit

                    #endregion Insert new user
                }
                else // insert 
                {



                    #region User new id generation


                    sqlText = "select isnull(max(cast(LogID as int)),0)+1 FROM  UserAuditLogs";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    int vLogID = (int)cmdNextId.ExecuteScalar();
                    if (vLogID <= 0)
                    {
                        throw new ArgumentNullException("Insert To User Information New", "Unable to create new user");
                    }

                    #endregion User new id generation

                    #region Insert new user

                    sqlText = "";
                    sqlText += " INSERT INTO UserAuditLogs";
                    sqlText += " (	LogID,";
                    sqlText += " 	ComputerName,";
                    sqlText += " 	ComputerLoginUserName,";
                    sqlText += " 	ComputerIPAddress,";
                    sqlText += " 	SoftwareUserId,";
                    sqlText += " 	SessionDate,";
                    sqlText += " 	LogInDateTime,";
                    sqlText += " 	LogOutDateTime";
                    sqlText += " )";
                    sqlText += "   VALUES";
                    sqlText += " (";
                    sqlText += "@vLogID,";
                    sqlText += "@ComputerName,";
                    sqlText += "@ComputerLoginUserName,";
                    sqlText += "@ComputerIPAddress,";
                    sqlText += "@SoftwareUserId,";
                    sqlText += "@SessionDate,";
                    sqlText += "@LogInDateTime,";
                    sqlText += "@LogOutDateTime";
                    sqlText += " )";


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@vLogID", vLogID);
                    cmdInsert.Parameters.AddWithValue("@ComputerName", ComputerName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ComputerLoginUserName", ComputerLoginUserName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ComputerIPAddress", ComputerIPAddress ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@SoftwareUserId", SoftwareUserId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@SessionDate", SessionDate ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@LogInDateTime", LogInDateTime ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@LogOutDateTime", LogOutDateTime ?? Convert.DBNull);

                    transResult = (int)cmdInsert.ExecuteNonQuery();
                    #endregion Insert new user
                    #region Commit

                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();

                            retResults = "" + vLogID;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults = "0";

                        }

                    }
                    else
                    {
                        retResults = "0";
                    }

                    #endregion Commit
                }
                
            }
            #region catch finally

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
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

            #endregion

            return retResults;
        } 
        public string InsertUserLogin(List<UserLogsVM> Details, string LogOutDateTime)
        {
            #region Variables

            string retResults = string.Empty;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                var vDatabaseName = currConn.Database;
                CommonDAL commonDal = new CommonDAL();
                #region UserLog

                commonDal.TableAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction); //tablename,fieldName, datatype

                commonDal.TableFieldAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerLoginUserName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerIPAddress", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SoftwareUserId", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SessionDate", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogInDateTime", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogOutDateTime", "datetime", currConn, transaction);

                #endregion UserLog

                transaction = currConn.BeginTransaction("InsertToUserInformationNew");

                #endregion open connection and transaction

                foreach (var Item in Details.ToList())
                {
                    #region IfSameDatabase
                    if (Item.DataBaseName == vDatabaseName)
                    {

                        sqlText = "";
                        sqlText += "  select ISNULL( COUNT (DISTINCT logid),0)logid FROM   UserAuditLogs ";
                        sqlText += " WHERE ComputerName         =@ItemComputerName";
                        sqlText += " and ComputerLoginUserName  =@ItemComputerLoginUserName";
                        sqlText += " and ComputerIPAddress      =@ItemComputerIPAddress";
                        sqlText += " and SoftwareUserId         =@ItemSoftwareUserId";
                        sqlText += " and SessionDate            =@ItemSessionDate";
                        sqlText += " and LogInDateTime          =@ItemLogInDateTime";
                        //sqlText += " and LogOutDateTime='" + Item.LogOutDateTime + "'";

                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;
                        cmdFindId.Parameters.AddWithValue("@ItemComputerName", Item.ComputerName);
                        cmdFindId.Parameters.AddWithValue("@ItemComputerLoginUserName", Item.ComputerLoginUserName);
                        cmdFindId.Parameters.AddWithValue("@ItemComputerIPAddress", Item.ComputerIPAddress);
                        cmdFindId.Parameters.AddWithValue("@ItemSoftwareUserId", Item.SoftwareUserId);
                        cmdFindId.Parameters.AddWithValue("@ItemSessionDate", Item.SessionDate);
                        cmdFindId.Parameters.AddWithValue("@ItemLogInDateTime", Item.LogInDateTime);

                        int IDExist = (int)cmdFindId.ExecuteScalar();

                        if (IDExist <= 0)//update
                        {
                            #region User new id generation


                            sqlText = "select isnull(max(cast(LogID as int)),0)+1 FROM  UserAuditLogs";
                            SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                            cmdNextId.Transaction = transaction;
                            int vLogID = (int)cmdNextId.ExecuteScalar();
                            if (vLogID <= 0)
                            {
                                throw new ArgumentNullException("Insert To User Information New", "Unable to create new user");
                            }

                            #endregion User new id generation

                            #region Insert new user

                            sqlText = "";
                            sqlText += " INSERT INTO UserAuditLogs";
                            sqlText += " (	LogID,";
                            sqlText += " 	ComputerName,";
                            sqlText += " 	ComputerLoginUserName,";
                            sqlText += " 	ComputerIPAddress,";
                            sqlText += " 	SoftwareUserId,";
                            sqlText += " 	SessionDate,";
                            sqlText += " 	LogInDateTime";
                            sqlText += " )";
                            sqlText += "   VALUES";
                            sqlText += " (";
                            sqlText += "@vLogID,";
                            sqlText += "@Item.ComputerName,";
                            sqlText += "@Item.ComputerLoginUserName,";
                            sqlText += "@Item.ComputerIPAddress,";
                            sqlText += "@Item.SoftwareUserId,";
                            sqlText += "@Item.SessionDate,";
                            sqlText += "@Item.LogInDateTime";
                            sqlText += " )";


                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                            cmdInsert.Transaction = transaction;
                            cmdInsert.Parameters.AddWithValue("@vLogID", vLogID);
                            cmdInsert.Parameters.AddWithValue("@Item.ComputerName", Item.ComputerName ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Item.ComputerLoginUserName ", Item.ComputerLoginUserName ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Item.ComputerIPAddress", Item.ComputerIPAddress ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Item.SoftwareUserId", Item.SoftwareUserId ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Item.SessionDate", Item.SessionDate ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Item.LogInDateTime", Item.LogInDateTime ?? Convert.DBNull);

                            transResult = (int)cmdInsert.ExecuteNonQuery();
                            #endregion Insert new user
                            #region Commit

                            if (transaction != null)
                            {
                                if (transResult > 0)
                                {
                                    transaction.Commit();

                                    retResults = "" + vLogID;

                                }
                                else
                                {
                                    transaction.Rollback();
                                    retResults = "0";

                                }

                            }
                            else
                            {
                                retResults = "0";
                            }

                            #endregion Commit
                        }

                    }
                    #endregion IfSameDatabase
                    #region IfNotSameDatabase
                    else{

                        sqlText = "";
                        sqlText += "  select ISNULL( COUNT (DISTINCT logid),0)logid FROM   UserAuditLogs ";
                        sqlText += " WHERE ComputerName         =@ItemComputerName";
                        sqlText += " and ComputerLoginUserName  =@ItemComputerLoginUserName";
                        sqlText += " and ComputerIPAddress      =@ItemComputerIPAddress";
                        sqlText += " and SoftwareUserId         =@ItemSoftwareUserId";
                        sqlText += " and SessionDate            =@ItemSessionDate";
                        sqlText += " and LogInDateTime          =@ItemLogInDateTime";
                        //sqlText += " and LogOutDateTime='" + Item.LogOutDateTime + "'";

                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;
                        cmdFindId.Parameters.AddWithValue("@ItemComputerName", Item.ComputerName);
                        cmdFindId.Parameters.AddWithValue("@ItemComputerLoginUserName", Item.ComputerLoginUserName);
                        cmdFindId.Parameters.AddWithValue("@ItemComputerIPAddress", Item.ComputerIPAddress);
                        cmdFindId.Parameters.AddWithValue("@ItemSoftwareUserId", Item.SoftwareUserId);
                        cmdFindId.Parameters.AddWithValue("@ItemSessionDate", Item.SessionDate);
                        cmdFindId.Parameters.AddWithValue("@ItemLogInDateTime", Item.LogInDateTime);

                        int IDExist = (int)cmdFindId.ExecuteScalar();

                        if (IDExist > 0) //update
                        {
                            #region Update

                            sqlText = "";
                            sqlText += " UPDATE UserAuditLogs";
                            sqlText += " SET LogOutDateTime         =@ItemLogOutDateTime";
                            sqlText += " WHERE ComputerName         =@ItemComputerName";
                            sqlText += " and ComputerLoginUserName  =@ItemComputerLoginUserName";
                            sqlText += " and ComputerIPAddress      =@ItemComputerIPAddress";
                            sqlText += " and SoftwareUserId         =@ItemSoftwareUserId";
                            sqlText += " and SessionDate            =@ItemSessionDate";
                            sqlText += " and LogInDateTime          =@ItemLogInDateTime";


                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                            cmdInsert.Transaction = transaction;
                            cmdInsert.Parameters.AddWithValue("@ItemLogOutDateTime", Item.LogOutDateTime);
                            cmdInsert.Parameters.AddWithValue("@ItemComputerName", Item.ComputerName ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ItemComputerLoginUserName", Item.ComputerLoginUserName ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ItemComputerIPAddress", Item.ComputerIPAddress ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ItemSoftwareUserId", Item.SoftwareUserId ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ItemSessionDate", Item.SessionDate ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ItemLogInDateTime", Item.LogInDateTime ?? Convert.DBNull);

                            transResult = (int)cmdInsert.ExecuteNonQuery();

                            #region Commit

                            if (transaction != null)
                            {
                                if (transResult > 0)
                                {
                                    transaction.Commit();

                                    retResults = "" + Item.LogID;

                                }
                                else
                                {
                                    transaction.Rollback();
                                    retResults = "0";

                                }

                            }
                            else
                            {
                                retResults = "0";
                            }

                            #endregion Commit

                            #endregion Insert new user
                        }

                    }
                    #endregion IfSameDatabase

                }
                
            }
            #region catch finally

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
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

            #endregion

            return retResults;
        }
        public string InsertUserLogOut(List<UserLogsVM> Details, string LogOutDateTime)
        {
            #region Variables

            string retResults = string.Empty;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                var vDatabaseName = currConn.Database;
                CommonDAL commonDal = new CommonDAL();
                #region UserLog

                commonDal.TableAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction); //tablename,fieldName, datatype

                commonDal.TableFieldAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerLoginUserName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerIPAddress", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SoftwareUserId", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SessionDate", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogInDateTime", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogOutDateTime", "datetime", currConn, transaction);

                #endregion UserLog
               
                transaction = currConn.BeginTransaction("InsertToUserInformationNew");

                #endregion open connection and transaction

                foreach (var Item in Details.ToList())
                {
                    if (Item.DataBaseName == vDatabaseName)
                    {

                        sqlText = "";
                        sqlText += "  select ISNULL( COUNT (DISTINCT logid),0)logid FROM   UserAuditLogs ";
                        sqlText += " WHERE ComputerName         =@ItemComputerName";
                        sqlText += " and ComputerLoginUserName  =@ItemComputerLoginUserName";
                        sqlText += " and ComputerIPAddress      =@ItemComputerIPAddress";
                        sqlText += " and SoftwareUserId         =@ItemSoftwareUserId";
                        sqlText += " and SessionDate            =@ItemSessionDate";
                        sqlText += " and LogInDateTime          =@ItemLogInDateTime";
                        //sqlText += " and LogOutDateTime='" + Item.LogOutDateTime + "'";

                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;
                        cmdFindId.Parameters.AddWithValue("@ItemComputerName", Item.ComputerName);
                        cmdFindId.Parameters.AddWithValue("@ItemComputerLoginUserName", Item.ComputerLoginUserName);
                        cmdFindId.Parameters.AddWithValue("@ItemComputerIPAddress", Item.ComputerIPAddress);
                        cmdFindId.Parameters.AddWithValue("@ItemSoftwareUserId", Item.SoftwareUserId);
                        cmdFindId.Parameters.AddWithValue("@ItemSessionDate", Item.SessionDate);
                        cmdFindId.Parameters.AddWithValue("@ItemLogInDateTime", Item.LogInDateTime);

                        int IDExist = (int) cmdFindId.ExecuteScalar();

                        if (IDExist > 0) //update
                        {
                            #region Update

                            sqlText = "";
                            sqlText += " UPDATE UserAuditLogs";
                            sqlText += " SET LogOutDateTime         =@LogOutDateTime";
                            sqlText += " WHERE ComputerName         =@ItemComputerName";
                            sqlText += " and ComputerLoginUserName  =@ItemComputerLoginUserName";
                            sqlText += " and ComputerIPAddress      =@ItemComputerIPAddress";
                            sqlText += " and SoftwareUserId         =@ItemSoftwareUserId";
                            sqlText += " and SessionDate            =@ItemSessionDate";
                            sqlText += " and LogInDateTime          =@ItemLogInDateTime";


                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                            cmdInsert.Transaction = transaction;
                            cmdInsert.Parameters.AddWithValue("@LogOutDateTime", LogOutDateTime);
                            cmdInsert.Parameters.AddWithValue("@ItemComputerName", Item.ComputerName ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ItemComputerLoginUserName", Item.ComputerLoginUserName ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ItemComputerIPAddress", Item.ComputerIPAddress ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ItemSoftwareUserId", Item.SoftwareUserId ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ItemSessionDate", Item.SessionDate ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ItemLogInDateTime", Item.LogInDateTime ?? Convert.DBNull);

                            transResult = (int) cmdInsert.ExecuteNonQuery();

                            #region Commit

                            if (transaction != null)
                            {
                                if (transResult > 0)
                                {
                                    transaction.Commit();

                                    retResults = "" + Item.LogID;

                                }
                                else
                                {
                                    transaction.Rollback();
                                    retResults = "0";

                                }

                            }
                            else
                            {
                                retResults = "0";
                            }

                            #endregion Commit

                            #endregion Insert new user
                        }
                    }
                }

               

            } 
            #region catch finally

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
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

            #endregion

            return retResults;
        }

        public string InsertUserLogOut(string LogID, string LogOutDateTime)
        {
            #region Variables

            string retResults = string.Empty;
           
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {
                

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToUserInformationNew");

                #endregion open connection and transaction
                
                #region Update

                sqlText = "";
                sqlText += " UPDATE UserAuditLogs";
                sqlText += " SET LogOutDateTime = @LogOutDateTime";
                sqlText += " WHERE LogID=@LogID";
                

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@LogOutDateTime", LogOutDateTime);
                cmdInsert.Parameters.AddWithValue("@LogID", LogID);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        
                        retResults = "" +LogID;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults = "0" ;

                    }

                }
                else
                {
                    retResults = "0";
                }

                #endregion Commit

                #endregion Insert new user

            }
            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
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

            #endregion

            return retResults;
        }
        public DataTable SearchUserLog(string ComputerLoginUserName, string SoftwareUserName, string ComputerName, string StartDate, string EndDate)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("SearchtUserLog");

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

                sqlText = @"SELECT 
LogID,
SoftwareUserId ,
ui.UserName SoftwareUserName,
convert (DATETIME,SessionDate,101)SessionDate,
convert (DATETIME,LogInDateTime,101)LogInDateTime,
convert (DATETIME,isnull(LogOutDateTime,'1900/01/01'),101)LogOutDateTime,
ComputerName,
ComputerLoginUserName,
ComputerIPAddress
FROM UserAuditLogs ul 
LEFT OUTER JOIN UserInformations ui  ON ul.SoftwareUserId=ui.UserID 
                            WHERE 
                            (ComputerLoginUserName LIKE '%' + @ComputerLoginUserName	 + '%' OR @ComputerLoginUserName	 IS NULL) 
                            AND (ui.UserName LIKE '%' + @SoftwareUserName + '%' OR @SoftwareUserName IS NULL)
                            AND (ComputerName LIKE '%' + @ComputerName + '%' OR @ComputerName IS NULL)
                            AND (LogInDateTime>= @StartDate OR @StartDate IS NULL)
                            AND (LogInDateTime <dateadd(d,1, @EndDate) OR @EndDate IS NULL)
                            order by username";

                SqlCommand objCommUser = new SqlCommand();
                objCommUser.Connection = currConn;
                objCommUser.CommandText = sqlText;
                objCommUser.CommandType = CommandType.Text;

                if (!objCommUser.Parameters.Contains("@ComputerLoginUserName"))
                { objCommUser.Parameters.AddWithValue("@ComputerLoginUserName", ComputerLoginUserName); }
                else { objCommUser.Parameters["@ComputerLoginUserName"].Value = ComputerLoginUserName; }
                
                if (!objCommUser.Parameters.Contains("@SoftwareUserName"))
                { objCommUser.Parameters.AddWithValue("@SoftwareUserName", SoftwareUserName); }
                else { objCommUser.Parameters["@SoftwareUserName"].Value = SoftwareUserName; }

                if (!objCommUser.Parameters.Contains("@ComputerName"))
                { objCommUser.Parameters.AddWithValue("@ComputerName", ComputerName); }
                else { objCommUser.Parameters["@ComputerName"].Value = ComputerName; }

                if (!objCommUser.Parameters.Contains("@StartDate"))
                { objCommUser.Parameters.AddWithValue("@StartDate", StartDate); }
                else { objCommUser.Parameters["@StartDate"].Value = StartDate; }

                if (!objCommUser.Parameters.Contains("@EndDate"))
                { objCommUser.Parameters.AddWithValue("@EndDate", EndDate); }
                else { objCommUser.Parameters["@EndDate"].Value = EndDate; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommUser);
                dataAdapter.Fill(dataTable);

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



        public string[] UpdateToUserInformationNew(string UserID, string UserName, string UserPassword, string ActiveStatus, string LastModifiedBy, string LastModifiedOn, string databaseName)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(UserID))
                {
                    throw new ArgumentNullException("UpdateToUserInformationNew", "Please enter user id.");
                }
                if (string.IsNullOrEmpty(UserName))
                {
                    throw new ArgumentNullException("UpdateToUserInformationNew", "Please enter user name.");
                }
                //if (string.IsNullOrEmpty(UserPassword))
                //{
                //    throw new ArgumentNullException("UpdateToUserInformationNew", "Please enter user password.");
                //}

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("Update To User Information New");

                #endregion open connection and transaction

                #region UserID existence checking by id

                //select @Present = count(UserID) from UserInformations where  UserID=@UserID;
                sqlText = "select count(UserID) from UserInformations where  UserID =@UserID";
                SqlCommand userIDExist = new SqlCommand(sqlText, currConn);
                userIDExist.Transaction = transaction;
                userIDExist.Parameters.AddWithValue("@UserID", UserID);

                countId = (int)userIDExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToUserInformationNew", "Could not find requested user id.");
                }

                #endregion UserID existence checking by id

                #region UserName existence checking by id and requied field

                sqlText = "select count(UserName) from UserInformations ";
                sqlText += " where  UserID=@UserID";
                sqlText += " and UserName =@UserName";
                SqlCommand userNameExist = new SqlCommand(sqlText, currConn);
                userNameExist.Transaction = transaction;
                userNameExist.Parameters.AddWithValue("@UserID", UserID);
                userNameExist.Parameters.AddWithValue("@UserName", UserName);

                countId = (int)userNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException("UpdateToUserInformationNew", "Same user name already exist.");
                }

                #endregion UserName existence checking by id and requied field

                #region Update user

                sqlText = "";
                sqlText = "update UserInformations set";
                sqlText += " UserName='" + UserName + "',";
                sqlText += " ActiveStatus  =@ActiveStatus ,";
                sqlText += " LastModifiedBy=@LastModifiedBy,";
                sqlText += " LastModifiedOn=@LastModifiedOn";
                sqlText += " where UserID  =@UserID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", ActiveStatus ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@UserID", UserID ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Information Successfully Update.";
                        retResults[2] = "" + UserID;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update user.";
                        retResults[2] = "" + UserID;
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update user group";
                    retResults[2] = "" + UserID;
                }

                #endregion Commit

                #endregion Update user

            }
            #region catch

            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
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

            #endregion

            return retResults;
        }

        public string[] UpdateUserPasswordNew(string UserName, string UserPassword, string LastModifiedBy, string LastModifiedOn, string databaseName)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(UserName))
                {
                    throw new ArgumentNullException("UpdateUserPasswordNew", "Please enter user name.");
                }
                if (string.IsNullOrEmpty(UserPassword))
                {
                    throw new ArgumentNullException("UpdateUserPasswordNew", "Please enter user password.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateToUserInformationNew");

                #endregion open connection and transaction

                #region UserName existence checking by id

                //select @Present = count(UserID) from UserInformations where  UserID=@UserID;
                sqlText = "select count(UserName) from UserInformations where  UserName = @UserName";
                SqlCommand userIDExist = new SqlCommand(sqlText, currConn);
                userIDExist.Transaction = transaction;
                userIDExist.Parameters.AddWithValue("@UserName", UserName);
                countId = (int)userIDExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateUserPasswordNew", "Could not find requested user name.");
                }

                #endregion UserName existence checking by id

                #region Update user

                sqlText = "";
                sqlText = "update UserInformations set";
                sqlText += " UserPassword  =@UserPassword,";
                sqlText += " LastModifiedBy=@LastModifiedBy,";
                sqlText += " LastModifiedOn=@LastModifiedOn";

                sqlText += " where UserName='" + UserName + "'";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@UserPassword  ", UserPassword ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", LastModifiedOn);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Password Information Successfully Update.";
                        retResults[2] = "" + UserName;

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update user.";
                        retResults[2] = "" + UserName;
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update user group";
                    retResults[2] = "" + UserName;
                }

                #endregion Commit

                #endregion Update user

            }
            #region catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
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

            #endregion

            return retResults;

        }

        //==================Search User=================
        /// <summary>
        /// Search User with separate SQL
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="UserName"></param>
        /// <param name="ActiveStatus"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public DataTable SearchUserDataTable(string UserID, string UserName, string ActiveStatus, string databaseName)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("User Search");

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

                sqlText = @"SELECT UserID, UserName, ActiveStatus,UserPassword
                            FROM UserInformations
                            WHERE 
                            (UserID LIKE '%' + @UserID	 + '%' OR @UserID	 IS NULL) 
                            AND (UserName LIKE '%' + @UserName + '%' OR @UserName IS NULL)
                            AND (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                            order by username";

                SqlCommand objCommUser = new SqlCommand();
                objCommUser.Connection = currConn;
                objCommUser.CommandText = sqlText;
                objCommUser.CommandType = CommandType.Text;

                if (!objCommUser.Parameters.Contains("@UserID"))
                { objCommUser.Parameters.AddWithValue("@UserID", UserID); }
                else { objCommUser.Parameters["@UserID"].Value = UserID; }
                if (!objCommUser.Parameters.Contains("@UserName"))
                { objCommUser.Parameters.AddWithValue("@UserName", UserName); }
                else { objCommUser.Parameters["@UserName"].Value = UserName; }
                if (!objCommUser.Parameters.Contains("@ActiveStatus"))
                { objCommUser.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommUser.Parameters["@ActiveStatus"].Value = ActiveStatus; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommUser);
                dataAdapter.Fill(dataTable);

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

        //==================Search User Has=================
        /// <summary>
        /// Search User Has with separate SQL
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public DataTable SearchUserHasNew(string UserName, string databaseName)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("SearchUserHas");

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

                sqlText = @"SELECT UserID, UserName, ActiveStatus,UserPassword
                            FROM         UserInformations                
                            WHERE (UserName = @UserName )
                            order by username";

                SqlCommand objCommBankInformation = new SqlCommand();
                objCommBankInformation.Connection = currConn;
                objCommBankInformation.CommandText = sqlText;
                objCommBankInformation.CommandType = CommandType.Text;

                if (!objCommBankInformation.Parameters.Contains("@UserName"))
                { objCommBankInformation.Parameters.AddWithValue("@UserName", UserName); }
                else { objCommBankInformation.Parameters["@UserName"].Value = UserName; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommBankInformation);
                dataAdapter.Fill(dataTable);

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

        #endregion

        #region Methods

        public string[] InsertToUserRoll(List<UserRollVM> userRollVMs, string databaseName)
        {

            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string userId = "";


            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToUserRoll");

                #endregion open connection and transaction

                #region id existence checking

                if (userRollVMs.Any())
                {
                    foreach (var item in userRollVMs)
                    {
                        if (!string.IsNullOrEmpty(item.UserID))
                        {
                            userId = item.UserID;
                        }
                        break;
                    }

                }

                sqlText = "select  count(FormID) from UserRolls where  UserID =@userId";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                cmdIdExist.Parameters.AddWithValue("@UserName", userId);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId > 0)
                {
                    sqlText = "delete from UserRolls where  UserID =@userId";
                    SqlCommand cmdIdExist1 = new SqlCommand(sqlText, currConn);
                    cmdIdExist1.Transaction = transaction;
                    cmdIdExist1.Parameters.AddWithValue("@UserName", userId);

                    cmdIdExist1.ExecuteScalar();
                }

                #endregion

                if (userRollVMs.Any())
                {
                    int j = 0;
                    foreach (var item in userRollVMs)
                    {
                        Debug.WriteLine(j.ToString());
                        j++;
                        #region Update Settings
                        sqlText ="";
                        //sqlText += "declare @Present numeric";
                        //sqlText +=" select @Present = count(FormID) from UserRolls ";
                        //sqlText += " where  UserID = '" + item.UserID + "' and FormID='" + item.FormID + "'; ";
                        //                sqlText +=" if(@Present <=0 ) ";
                        //                sqlText +=" BEGIN ";
                                        sqlText +=" insert into UserRolls( ";
                                        sqlText +=" UserID, ";
                                        sqlText +=" FormID, ";
                                        sqlText +=" Access, ";
                                        sqlText +=" CreatedBy, ";
                                        sqlText +=" CreatedOn, ";
                                        sqlText +=" LastModifiedBy, ";
                                        sqlText += " LastModifiedOn, ";
                                        sqlText += " AddAccess,EditAccess, ";
                                        sqlText +=" LineID,FormName,PostAccess) ";
                                        sqlText +=" values( ";
                                        sqlText += "@itemUserID, ";
                                        sqlText += "@itemFormID, ";
                                        sqlText += "@itemAccess, ";
                                        sqlText += "@itemCreatedBy, ";
                                        sqlText += "@itemCreatedOn, ";
                                        sqlText += "@itemLastModifiedBy, ";
                                        sqlText += "@itemLastModifiedOn, ";
                                        sqlText += "@itemAddAccess,";
                                        sqlText += "@itemEditAccess,";
                                        sqlText += "@itemLineID,";
                                        sqlText += "@itemFormName,";
                                        sqlText += "@itemPostAccess); ";
                                        //sqlText +=" END ";
                                        //sqlText +=" else ";
                                        //sqlText +=" BEGIN ";
                                        //sqlText +=" update UserRolls ";
                                        //sqlText +=" set  ";
                                        //sqlText += " Access='" + item.Access + "', ";
                                        //sqlText += " LineID='" + item.LineID + "', ";
                                        //sqlText += " FormName='" + item.FormName + "', ";
                                        //sqlText += " LastModifiedBy='" + item.LastModifiedBy + "', ";
                                        //sqlText += " LastModifiedOn='" + item.LastModifiedOn + "', ";
                                        //sqlText += " PostAccess='" + item.PostAccess + "' ";
                                        //sqlText += " where userid='" + item.UserID + "' and FormID='" + item.FormID + "'; ";
                                        //sqlText +=" END";

                        

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@itemUserID", item.UserID);
                        cmdInsDetail.Parameters.AddWithValue("@itemFormID", item.FormID ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@itemAccess", item.Access ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@itemCreatedBy", item.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@itemCreatedOn", item.CreatedOn);
                        cmdInsDetail.Parameters.AddWithValue("@itemLastModifiedBy", item.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@itemLastModifiedOn", item.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@itemAddAccess", item.AddAccess ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@itemEditAccess", item.EditAccess ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@itemLineID", item.LineID ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@itemFormName", item.FormName ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@itemPostAccess", item.PostAccess ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.PurchasemsgSaveNotSuccessfully);
                        }
                      
                        #endregion Update Settings
                    }

                }
                else
                {
                    throw new ArgumentNullException("InsertToUserRoll", "Could not found any item.");
                }


                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Roll Information Successfully Updated.";
                    }

                }

                #endregion Commit
                

            }

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public string SearchUserRoll(string UserID)
        {
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            //string userRoll = string.Empty;
            string decryptedData = string.Empty;
            string encriptedData = string.Empty;
            

            try
            {
                #region open connection

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection
                #region MyRegion

                CommonDAL commonDal = new CommonDAL();
                int insertCol = 0;
                SqlTransaction transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);
                insertCol = commonDal.TableFieldAdd("UserRolls", "AddAccess", "varchar(1)", currConn, transaction);
                insertCol = commonDal.TableFieldAdd("UserRolls", "EditAccess", "varchar(1)", currConn, transaction);

                if (insertCol < 0)
                {
                    transaction.Commit();
                }

                #endregion
                
                sqlText = @"
                SELECT LineID,UserID,FormID,isnull(Access,'N')Access,isnull(PostAccess,'N')PostAccess,isnull(AddAccess,'N')AddAccess,isnull(EditAccess,'N')EditAccess 
                FROM dbo.UserRolls

                WHERE (UserID  = @UserID ) 
                order by UserID,LineID
                ";

                SqlCommand objCommBankInformation = new SqlCommand();
                objCommBankInformation.Connection = currConn;
                objCommBankInformation.CommandText = sqlText;
                objCommBankInformation.CommandType = CommandType.Text;


                //objCommUser.CommandText = sqlText;
                //objCommUser.CommandType = CommandType.Text;


                if (!objCommBankInformation.Parameters.Contains("@UserID"))
                { objCommBankInformation.Parameters.AddWithValue("@UserID", UserID); }
                else { objCommBankInformation.Parameters["@UserID"].Value = UserID; }

                SqlDataReader reader = objCommBankInformation.ExecuteReader();
                while (reader.Read())
                {
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        decryptedData = decryptedData + FieldDelimeter + reader[j].ToString();
                    }
                    decryptedData = decryptedData + LineDelimeter;
                }
                reader.Close();

                encriptedData = Converter.DESEncrypt(PassPhrase, EnKey, decryptedData);
                //return decryptedData;
                return encriptedData;

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

          
            return encriptedData;
        }
      
        public DataTable SearchUserHas(string UserName)
        {
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable("UserHas");


            try
            {
                #region open connection

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection


                sqlText = @"SELECT UserID, UserName, ActiveStatus,UserPassword,GETDATE() ServerDate
                            FROM         UserInformations                
                            WHERE (UserName = @UserName )
                            order by username";

                SqlCommand objCommUI = new SqlCommand();
                objCommUI.Connection = currConn;
                objCommUI.CommandText = sqlText;
                objCommUI.CommandType = CommandType.Text;


                if (!objCommUI.Parameters.Contains("@UserName"))
                { objCommUI.Parameters.AddWithValue("@UserName", UserName); }
                else { objCommUI.Parameters["@UserName"].Value = UserName; }

                //SqlDataReader reader = objCommBankInformation.ExecuteReader();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommUI);
                dataAdapter.Fill(dt);




            }
            #region catch and final
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
            #endregion catch and final

            return dt;
        }
    
        #endregion
    }
}
