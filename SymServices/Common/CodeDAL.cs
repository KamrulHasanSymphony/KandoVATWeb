
using SymOrdinary;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SymServices.VMS;

namespace SymServices.Common
{
    public class CodeDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
       
        #endregion

        #region Methods

        //==================Search Codes=================
        public DataSet SearchCodes()
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            //DataTable dataTable = new DataTable("Search Codes");
            DataSet dataSet = new DataSet("Search Codes");

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
CodeId,
CodeGroup,
CodeName,
prefix,
Lenth,
ActiveStatus
                                      FROM Codes
                                      ORDER BY CodeGroup

SELECT DISTINCT CodeGroup FROM Codes order by CodeGroup

";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dataSet);

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

            return dataSet;
        }

        //==================Update Codes=================
        public string[] CodeUpdate(List<CodeVM> codeVMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
         
            bool iSTransSuccess = false;

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

                if (codeVMs.Any())
                {
                    foreach (var item in codeVMs)
                    {
                        #region Update Settings

                        sqlText = "";
                        sqlText = "update Codes set";
                        sqlText += " prefix='" + item.prefix + "',";
                        sqlText += " Lenth='" + item.Lenth + "'";

                        sqlText += " where CodeName='" + item.CodeName + "'" + " and CodeGroup='" + item.CodeGroup + "'";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
			            var exeRes = cmdUpdate.ExecuteNonQuery();
			            transResult = Convert.ToInt32(exeRes);

                        #region Commit

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("CodeUpdate", item.CodeName + " could not updated.");
                        }

                        #endregion Commit

                        #endregion Update Settings
                    }

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("CodeUpdate", "Could not found any item.");
                }


                if (iSTransSuccess == true)
                {
                   
                       if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                    
                    retResults[0] = "Success";
                    retResults[1] = "Requested Code Information Successfully Updated.";
                    retResults[2] = "";

                }
                else
                {
                    retResults[0] = "Fail";//Success or Fail
                    if (Vtransaction == null) { transaction.Rollback(); }
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update code.";
                    retResults[2] = "";
                }

            }
            #region catch

            catch (SqlException sqlex)
            {
                retResults[0] = "Fail";//Success or Fail
                if (Vtransaction == null) { transaction.Rollback(); }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                if (Vtransaction == null) { transaction.Rollback(); }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                //throw ex;
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

        public string CodeDataInsert(string CodeGroup, string CodeName, string prefix, string Lenth,
            SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(CodeGroup))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(CodeName))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(prefix))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(Lenth))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }

                #endregion Validation

                

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
				   

                #endregion open connection and transaction

                #region ProductExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT CodeId)CodeId FROM Codes ";
                sqlText += " WHERE CodeGroup='" + CodeGroup + "' AND CodeName='" + CodeName + "' AND prefix='" + prefix + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("CodesDataInsert", "Please Input Codes Value");
                }
                #endregion ProductExist
                #region Last Price

                int foundId = (int)objfoundId;
                if (foundId <= 0)
                {
                    sqlText = "  ";
                    sqlText += " INSERT INTO Codes(	CodeGroup,	CodeName,prefix,Lenth,ActiveStatus,CreatedBy,CreatedOn,LastModifiedBy,LastModifiedOn)";
                    sqlText += " VALUES(";
                    sqlText += " '" + CodeGroup + "',";
                    sqlText += " '" + CodeName + "',";
                    sqlText += " '" + prefix + "',";
                    sqlText += " '" + Lenth + "',";
                    sqlText += " 'Y',";
                    sqlText += " 'admin',";
                    sqlText += " '1900-01-01',";
                    sqlText += " 'admin',";
                    sqlText += " '1900-01-01'";
                    sqlText += " )";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("CodeDataInsert", "Please Input Code Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("CodeDataInsert", "Please Input Code Value");
                    }



                }


                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall


            catch (SqlException sqlex)
            {
                
                if (Vtransaction == null) { transaction.Rollback(); }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                //throw sqlex;
            }
            catch (Exception ex)
            {

                if (Vtransaction == null) { transaction.Rollback(); }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                //throw ex;
            }

            finally
            {
                if (currConn == null)
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
  
        #endregion
            

    }
}
