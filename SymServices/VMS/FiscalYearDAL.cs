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
    public class FiscalYearDAL
    {
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        public DataTable SearchYear()
        {

            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("ProductType");
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
                            SELECT CurrentYear FROM 
(
SELECT DISTINCT CurrentYear FROM  FiscalYear 
union 
select max(CurrentYear)+1 FROM FiscalYear 
UNION 
                                 
SELECT DATEPART(yyyy, FYearEnd) FROM CompanyProfiles
) AS a
WHERE CurrentYear IS NOT null ORDER BY CurrentYear";

                SqlCommand objCommYear = new SqlCommand();
                objCommYear.Connection = currConn;
                objCommYear.CommandText = sqlText;
                objCommYear.CommandType = CommandType.Text;


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommYear);
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
        public string[] FiscalYearInsert(List<FiscalYearVM> Details)
        {
            #region Initializ
            string[] retResults = new string[2];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            
            int IDExist = 0;
                int nextId = 0;


            #endregion Initializ

            #region Try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.FYMsgMethodNameInsert);

                #endregion open connection and transaction

                if (!Details.Any()) throw new ArgumentNullException("FiscalYearInsert", "Sorry,No item found to add.");

                #region Find Transaction Exist

                //sqlText = "";
                //sqlText = sqlText + "select COUNT(PeriodID) from FiscalYear" +
                //          " WHERE  PeriodID=@DetailsPeriodID ";
                //SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                //cmdExistTran.Transaction = transaction;
                //cmdExistTran.Parameters.AddWithValue("@DetailsPeriodID", Details[0].PeriodID);

                //IDExist = (int)cmdExistTran.ExecuteScalar();

                //if (IDExist > 0)
                //{
                //    throw new ArgumentNullException("FiscalYearInsert", "Fiscal Year aleady exist.");
                //}

                #endregion Find Transaction Exist

                #region Find Previous Year Lock

                //sqlText = "";
                //sqlText = sqlText + "select COUNT(periodlock) from FiscalYear" +
                //          " WHERE currentyear='" + Details[0].CurrentYear + "'-1  and periodlock='Y'";
                //SqlCommand cmdPreYearLock = new SqlCommand(sqlText, currConn);
                //cmdPreYearLock.Transaction = transaction;
                //IDExist = (int)cmdPreYearLock.ExecuteScalar();

                //if (IDExist < 12)
                //{
                //    throw new ArgumentNullException(MessageVM.FYMsgMethodNameInsert, MessageVM.FYMsgPreviouseYearNotLock);
                //}

                #endregion Find Previous Year Lock

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.FYMsgMethodNameInsert, MessageVM.FYMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {

                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into FiscalYear(";
                    sqlText += " FiscalYearName,";
                    sqlText += " CurrentYear,";
                    sqlText += " PeriodID,";
                    sqlText += " PeriodName,";
                    sqlText += " PeriodStart,";
                    sqlText += " PeriodEnd,";
                    sqlText += " PeriodLock,";
                    sqlText += " GLLock,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn";

                    sqlText += " )";
                    sqlText += " values(	";

                    sqlText += "@ItemFiscalYearName,";
                    sqlText += "@ItemCurrentYear,";
                    sqlText += "@ItemPeriodID,";
                    sqlText += "@ItemPeriodName,";
                    sqlText += "@ItemPeriodStart,";
                    sqlText += "@ItemPeriodEnd,";
                    sqlText += "@ItemPeriodLock,";
                    sqlText += "@ItemGLLock,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@ItemLastModifiedBy,";
                    sqlText += "@ItemLastModifiedOn";

                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValue("@ItemFiscalYearName", Item.FiscalYearName ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemCurrentYear", Item.CurrentYear ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodID", Item.PeriodID ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodName", Item.PeriodName ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodStart", Item.PeriodStart ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodEnd", Item.PeriodEnd ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodLock", Item.PeriodLock);
                    cmdInsDetail.Parameters.AddWithValue("@ItemGLLock", Item.GLLock);
                    cmdInsDetail.Parameters.AddWithValue("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemCreatedOn", Item.CreatedOn ?? Convert.DBNull);  
                    cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedBy", Item.LastModifiedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedOn", Item.LastModifiedOn ?? Convert.DBNull);  

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.FYMsgMethodNameInsert, MessageVM.FYMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable
                }
                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.FYMsgSaveSuccessfully;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }
        public string[] FiscalYearUpdate(List<FiscalYearVM> Details,string modifiedBy)
        {
            #region Initializ
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
                int nextId = 0;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            int Present = 0;
            string MLock = "";
            string YLock = "";
            DateTime MinDate = DateTime.MinValue;
            DateTime MaxDate = DateTime.MaxValue;

            int count = 0;
            string n = "";
            string Prefetch = "";
            int Len = 0;
            int SetupLen = 0;
            int CurrentID = 0;
            string newID = "";
            int IssueInsertSuccessfully = 0;
            int ReceiveInsertSuccessfully = 0;
            string PostStatus = "";

            int SetupIDUpdate = 0;
            int InsertDetail = 0;


            #endregion Initializ

            #region Try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.FYMsgMethodNameUpdate);

                #endregion open connection and transaction



                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.FYMsgMethodNameUpdate, MessageVM.FYMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText = sqlText + "select COUNT(PeriodID) from FiscalYear" +
                              " WHERE  PeriodID=@ItemPeriodID";
                    SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                    cmdExistTran.Transaction = transaction;
                    cmdExistTran.Parameters.AddWithValue("@ItemPeriodID", Item.PeriodID);

                    int IDExist = (int)cmdExistTran.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.FYMsgMethodNameInsert, MessageVM.FYMsgNotExist);
                    }

                    #endregion Find Transaction Exist

                    #region Update only DetailTable

                    sqlText = "";


                    sqlText += " update FiscalYear set ";

                    sqlText += " PeriodLock     =@ItemPeriodLock,";
                    sqlText += " GLLock         =@ItemGLLock,";
                    sqlText += " LastModifiedBy =@ItemLastModifiedBy,";
                    sqlText += " LastModifiedOn =@ItemLastModifiedOn";
                    sqlText += " where  PeriodID=@ItemPeriodID";

                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodLock", Item.PeriodLock);
                    cmdInsDetail.Parameters.AddWithValue("@ItemGLLock", Item.GLLock);
                    cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedBy", modifiedBy??Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedOn", DateTime.Now);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPeriodID", Item.PeriodID);


                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.FYMsgMethodNameUpdate, MessageVM.FYMsgUpdateNotSuccessfully);
                    }
                    #endregion Update only DetailTable


                }


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.FYMsgUpdateSuccessfully;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public DataTable LoadYear(string CurrentYear)
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Year");
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
                            SELECT    
FiscalYearName,
CurrentYear,
PeriodID,
PeriodName,
convert(varchar, PeriodStart,120)PeriodStart,
convert(varchar, PeriodEnd,120)PeriodEnd, 
PeriodLock,
isnull(GLLock,'N')GLLock 

FROM         FiscalYear
WHERE 	(CurrentYear  =  @CurrentYear ) 

ORDER BY PeriodStart";

                SqlCommand objCommYear = new SqlCommand();
                objCommYear.Connection = currConn;
                objCommYear.CommandText = sqlText;
                objCommYear.CommandType = CommandType.Text;

                if (!objCommYear.Parameters.Contains("@CurrentYear"))
                {
                    objCommYear.Parameters.AddWithValue("@CurrentYear", CurrentYear);
                }
                else
                {
                    objCommYear.Parameters["@CurrentYear"].Value = CurrentYear;
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommYear);
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

        //==================SelectAll=================
        public List<FiscalYearVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<FiscalYearVM> VMs = new List<FiscalYearVM>();
            FiscalYearVM vm;
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
 FiscalYearName
,CurrentYear
,PeriodID
,PeriodName
,PeriodStart
,PeriodEnd
,PeriodLock
,GLLock
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn

FROM FiscalYear  
WHERE  1=1

";
                if (Id > 0)
                {
                    sqlText += @" and PeriodID=@PeriodID";
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
                    objComm.Parameters.AddWithValue("@PeriodID", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearVM();
                    vm.FiscalYearName = dr["FiscalYearName"].ToString();
                    vm.CurrentYear = dr["CurrentYear"].ToString();
                    vm.PeriodID = dr["PeriodID"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.PeriodStart = Ordinary.DateTimeToDate(dr["PeriodStart"].ToString());
                    vm.PeriodEnd = Ordinary.DateTimeToDate(dr["PeriodEnd"].ToString());
                    vm.PeriodLock = dr["PeriodLock"].ToString();
                    vm.GLLock = dr["GLLock"].ToString();
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

        //==================DropDownAll==========
        public List<FiscalYearVM> DropDownAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<FiscalYearVM> VMs = new List<FiscalYearVM>();
            FiscalYearVM vm;
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
//                sqlText = @"
//select distinct CurrentYear from FiscalYear union select 'All' CurrentYear from FiscalYear
//WHERE  1=1
//";
                sqlText = @" select distinct CurrentYear from FiscalYear WHERE  1=1";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new FiscalYearVM();
                    vm.CurrentYear = dr["CurrentYear"].ToString(); ;
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

        //=================LockCheck============
        public int LockChek() 
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            int Unlocked = 0;
            #endregion

            #region open connection and transaction
            currConn = _dbsqlConnection.GetConnection();
            if (currConn.State != ConnectionState.Open)
            {
                currConn.Open();
            }
            #endregion open connection and transaction


            sqlText += "select COUNT(PeriodID) from FiscalYear where PeriodLock='N'";

            SqlCommand objComm = new SqlCommand(sqlText, currConn);
            Unlocked = (int)objComm.ExecuteScalar();
            return Unlocked;
        }

        public string MaxDate() 
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            string maxDate ="";
            #endregion

            #region open connection and transaction
            currConn = _dbsqlConnection.GetConnection();
            if (currConn.State != ConnectionState.Open)
            {
                currConn.Open();
            }
            #endregion open connection and transaction


            sqlText += "select MaxDate=max(PeriodEnd) from FiscalYear";

            SqlCommand objComm = new SqlCommand(sqlText, currConn);
            SqlDataReader dr;
            dr = objComm.ExecuteReader();
             while (dr.Read())
                {
                    maxDate = dr["MaxDate"].ToString();
                }
            
            return maxDate;
        }

        public FiscalYearPeriodVM StartEndPeriod(string year)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            FiscalYearPeriodVM vm=new FiscalYearPeriodVM();
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
                sqlText = @"select min(PeriodStart) PeriodStart,max(PeriodEnd) PeriodEnd from FiscalYear where CurrentYear=@CurrentYear";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValue("@CurrentYear", year);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();
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
            return vm;
        }
    }
}
