using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.VMS;


namespace SymServices.VMS
{
    public class DBUpdateDAL
    {
        #region Global Variables
        private DataTable dataTable;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        List<DBUpdateViewModel> viewModels=new List<DBUpdateViewModel>();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================Table Name=================
        public List<DBUpdateViewModel> TableName()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            string tableName = "";
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
                sqlText = @"select name from sys.tables";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                DBUpdateViewModel vm;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new DBUpdateViewModel();
                    vm.tableName = dr["name"].ToString();
                    viewModels.Add(vm);
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
            return viewModels;
        }

        //==================SelectAll=================
        public DataTable SelectAll(string tableName, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            #endregion

            #region New open connection and transaction
            if (VcurrConn != null)
            {
                currConn = VcurrConn;
            }
            if (Vtransaction != null)
            {
                transaction = Vtransaction;
            }
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
            #endregion New open connection and transaction

            sqlText = "select * from " + tableName + "";

            SqlCommand cmd = new SqlCommand(sqlText, currConn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dataTable);
            return dataTable;
        }

        //==================Update =================
        public string[] Update(string query, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "CurrencyUpdate"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Update"); }
                #endregion open connection and transaction

                    #region Update Settings
                    #region SqlText
                    sqlText = "";
                    sqlText += query;

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update", "");
                    }
                    #endregion SqlExecution

                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Currency Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                #endregion SuccessResult
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

    }
}
#endregion