using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymServices.VMS;
namespace SymServices.Common
{
    public class DBUpdateDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        #region DB Migrate
        public string[] DBTableAdd(string TableName, string FieldName, string DataType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (objects)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (columns)");
                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (data type)");
                }
                #endregion Validation
                sqlText += " IF  NOT EXISTS (SELECT * FROM sys.objects ";
                sqlText += " WHERE object_id = OBJECT_ID(N'" + TableName + "') AND type in (N'U'))";
                sqlText += " BEGIN";
                sqlText += " CREATE TABLE " + TableName + "( " + FieldName + " " + DataType + " null) ";
                sqlText += " END";
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
               if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            return retResults;
        }
        public string[] DBTableFieldAdd(string TableName, string FieldName, string DataType, bool NullType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (objects)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (columns)");
                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (data type)");
                }
                #endregion Validation
                sqlText = "";
                sqlText += " if not exists(select * from sys.columns ";
                sqlText += " where Name = N'" + FieldName + "' and Object_ID = Object_ID(N'" + TableName + "'))   ";
                sqlText += " begin";
                if (NullType == true)
                {
                    sqlText += " ALTER TABLE " + TableName + " ADD " + FieldName + " " + DataType + " NULL DEFAULT 0 ;";
                }
                else
                {
                    sqlText += " ALTER TABLE " + TableName + " ADD " + FieldName + " " + DataType + " NOT NULL DEFAULT 0 ;";
                }
                sqlText += " END";
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
               if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            return retResults;
        }
        public string[] DBTableFieldAlter(string TableName, string FieldName, string DataType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (objects)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (columns)");
                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (data type)");
                }
                #endregion Validation
                sqlText = "";
                sqlText += " ALTER TABLE " + TableName + " ALTER COLUMN " + FieldName + "   " + DataType + "";
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
               if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            return retResults;
        }
        public string[] DBTableFieldRemove(string TableName, string FieldName, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (objects)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (columns)");
                }
                #endregion Validation
                sqlText = "";
                sqlText += " if exists(select * from sys.columns ";
                sqlText += " where Name = N'" + FieldName + "' and Object_ID = Object_ID(N'" + TableName + "'))   ";
                sqlText += " begin";
                sqlText += " ALTER TABLE " + TableName + " DROP COLUMN " + FieldName ;
                sqlText += " END";
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
               if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
            return retResults;
        }
        #endregion
    }
}
