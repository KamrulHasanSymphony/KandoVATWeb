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
    public class SDDepositDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
         CommonDAL _cDal = new  CommonDAL();

        #endregion

        
        #region New Methods

        //==================Search SDDeposit=================

        public DataTable SearchSDDepositNew
            (string DepositId,
            string TreasuryNo,
            string DepositDateFrom, 
            string DepositDateTo,
            string DepositType,
            string ChequeNo,
            string ChequeDateFrom,
            string ChequeDateTo,
            string BankName,
            string BranchName,
            string AccountNumber,
            string TransactionType,
            string Post)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SDDeposits");

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
                                SDDeposits.DepositId,
                                isnull(SDDeposits.TreasuryNo,'N/A')TreasuryNo,
                                convert (varchar,SDDeposits.DepositDateTime,120)DepositDateTime,
                                isnull(SDDeposits.DepositType,'N/A')DepositType,
                                isnull(SDDeposits.DepositAmount,0)DepositAmount ,
                                isnull(SDDeposits.ChequeNo,'N/A')ChequeNo,
                                isnull(SDDeposits.ChequeBank,'N/A')ChequeBank,isnull(SDDeposits.ChequeBankBranch,'N/A')ChequeBankBranch, 
                                convert (varchar,SDDeposits.ChequeDate,120)ChequeDate,                                
                                SDDeposits.BankID,                                
                                isnull(BankInformations.BankName,'N/A')BankName ,                                
                                isnull(BankInformations.BranchName,'N/A')BranchName,                                
                                isnull(BankInformations.AccountNumber,'N/A')AccountNumber,                                
                                isnull(SDDeposits.DepositPerson,'N/A')DepositPerson ,                                
                                isnull(SDDeposits.DepositPersonDesignation,'N/A')DepositPersonDesignation ,                                
                                isnull(SDDeposits.Comments,'N/A')Comments,
                                isnull(SDDeposits.ReverseDepositId,'')ReverseDepositId,
                                SDDeposits.Post,
                                ISNULL(NULLIF(SDDeposits.TransactionType,''),'Treasury')TransactionType


                                FROM SDDeposits LEFT OUTER JOIN
                                BankInformations ON SDDeposits.BankID = BankInformations.BankID
   
                                WHERE 
                                (DepositId  LIKE '%' +  @DepositId + '%' OR @DepositId IS NULL) 
                                AND (TreasuryNo LIKE '%' + @TreasuryNo + '%' OR @TreasuryNo IS NULL)
                                AND (DepositDateTime >= @DepositDateFrom OR @DepositDateFrom IS NULL)
                                AND (DepositDateTime <= @DepositDateTo  OR @DepositDateTo IS NULL)
                                AND (DepositType LIKE '%' + @DepositType + '%' OR @DepositType IS NULL)
                                AND (ChequeNo LIKE '%' + @ChequeNo + '%' OR @ChequeNo IS NULL)
                                AND (ChequeDate >= @ChequeDateFrom OR @ChequeDateFrom IS NULL)
                                AND (ChequeDate <= @ChequeDateTo  OR @ChequeDateTo IS NULL)
                                AND (BankName LIKE '%' + @BankName + '%' OR @BankName IS NULL)
                                AND (BranchName	 LIKE '%' + @BranchName	 + '%' OR @BranchName	 IS NULL)
                                AND (AccountNumber LIKE '%' + @AccountNumber + '%' OR @AccountNumber IS NULL)
                                --AND (SDDeposits.TransactionType LIKE '%' + @TransactionType + '%' OR @TransactionType IS NULL)
                                AND (SDDeposits.TransactionType LIKE '%' + @TransactionType OR @TransactionType IS NULL)
                                AND (Post LIKE '%' + @Post + '%' OR @Post IS NULL)
                                order by DepositDateTime desc";

                SqlCommand objCommDeposit = new SqlCommand();
                objCommDeposit.Connection = currConn;
                objCommDeposit.CommandText = sqlText;
                objCommDeposit.CommandType = CommandType.Text;

                #region param

                if (!objCommDeposit.Parameters.Contains("@DepositId"))
                { objCommDeposit.Parameters.AddWithValue("@DepositId", DepositId); }
                else { objCommDeposit.Parameters["@DepositId"].Value = DepositId; }
                if (!objCommDeposit.Parameters.Contains("@Post"))
                { objCommDeposit.Parameters.AddWithValue("@Post", Post); }
                else { objCommDeposit.Parameters["@Post"].Value = Post; }
                if (!objCommDeposit.Parameters.Contains("@TreasuryNo"))
                { objCommDeposit.Parameters.AddWithValue("@TreasuryNo", TreasuryNo); }
                else { objCommDeposit.Parameters["@TreasuryNo"].Value = TreasuryNo; }
                if (DepositDateFrom == "")
                {
                    if (!objCommDeposit.Parameters.Contains("@DepositDateFrom"))
                    { objCommDeposit.Parameters.AddWithValue("@DepositDateFrom", System.DBNull.Value); }
                    else { objCommDeposit.Parameters["@DepositDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommDeposit.Parameters.Contains("@DepositDateFrom"))
                    { objCommDeposit.Parameters.AddWithValue("@DepositDateFrom", DepositDateFrom); }
                    else { objCommDeposit.Parameters["@DepositDateFrom"].Value = DepositDateFrom; }
                }
                if (DepositDateTo == "")
                {
                    if (!objCommDeposit.Parameters.Contains("@DepositDateTo"))
                    { objCommDeposit.Parameters.AddWithValue("@DepositDateTo", System.DBNull.Value); }
                    else { objCommDeposit.Parameters["@DepositDateTo"].Value = System.DBNull.Value; }
                }
                else
                {

                    if (!objCommDeposit.Parameters.Contains("@DepositDateTo"))
                    { objCommDeposit.Parameters.AddWithValue("@DepositDateTo", DepositDateTo); }
                    else { objCommDeposit.Parameters["@DepositDateTo"].Value = DepositDateTo; }
                }

                if (!objCommDeposit.Parameters.Contains("@DepositType"))
                { objCommDeposit.Parameters.AddWithValue("@DepositType", DepositType); }
                else { objCommDeposit.Parameters["@DepositType"].Value = DepositType; }

                if (!objCommDeposit.Parameters.Contains("@ChequeNo"))
                { objCommDeposit.Parameters.AddWithValue("@ChequeNo", ChequeNo); }
                else { objCommDeposit.Parameters["@ChequeNo"].Value = ChequeNo; }

                if (ChequeDateFrom == "")
                {
                    if (!objCommDeposit.Parameters.Contains("@ChequeDateFrom"))
                    { objCommDeposit.Parameters.AddWithValue("@ChequeDateFrom", System.DBNull.Value); }
                    else { objCommDeposit.Parameters["@ChequeDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {

                    if (!objCommDeposit.Parameters.Contains("@ChequeDateFrom"))
                    { objCommDeposit.Parameters.AddWithValue("@ChequeDateFrom", ChequeDateFrom); }
                    else { objCommDeposit.Parameters["@ChequeDateFrom"].Value = ChequeDateFrom; }
                }
                if (ChequeDateTo == "")
                {
                    if (!objCommDeposit.Parameters.Contains("@ChequeDateTo"))
                    { objCommDeposit.Parameters.AddWithValue("@ChequeDateTo", System.DBNull.Value); }
                    else { objCommDeposit.Parameters["@ChequeDateTo"].Value = System.DBNull.Value; }
                }
                else
                {

                    if (!objCommDeposit.Parameters.Contains("@ChequeDateTo"))
                    { objCommDeposit.Parameters.AddWithValue("@ChequeDateTo", ChequeDateTo); }
                    else { objCommDeposit.Parameters["@ChequeDateTo"].Value = ChequeDateTo; }
                }

                if (!objCommDeposit.Parameters.Contains("@BankName"))
                { objCommDeposit.Parameters.AddWithValue("@BankName", BankName); }
                else { objCommDeposit.Parameters["@BankName"].Value = BranchName; }
                if (!objCommDeposit.Parameters.Contains("@BranchName"))
                { objCommDeposit.Parameters.AddWithValue("@BranchName", BranchName); }
                else { objCommDeposit.Parameters["@BranchName"].Value = BranchName; }
                if (!objCommDeposit.Parameters.Contains("@AccountNumber"))
                { objCommDeposit.Parameters.AddWithValue("@AccountNumber", AccountNumber); }
                else { objCommDeposit.Parameters["@AccountNumber"].Value = AccountNumber; }

                if (!objCommDeposit.Parameters.Contains("@TransactionType"))
                { objCommDeposit.Parameters.AddWithValue("@TransactionType", TransactionType); }
                else { objCommDeposit.Parameters["@TransactionType"].Value = TransactionType; }

                #endregion
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommDeposit);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
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
        public string[] DepositInsert(SDDepositVM Master)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            string newID = "";
            string PostStatus = "";
            int IDExist = 0;

            #endregion Initializ

            #region Try

            try
            {
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.depMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.DepositDate) < DateTime.MinValue || Convert.ToDateTime(Master.DepositDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                    MessageVM.msgFiscalYearNotExist);
                }


                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.depMsgMethodNameInsert);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DepositDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DepositDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK
                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(DepositId) from SDDeposits WHERE DepositId=@newID ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValue("@newID", newID);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.dpMsgFindExistID);
                }

                #endregion Find Transaction Exist
                #region Purchase ID Create

                if (string.IsNullOrEmpty(Master.TransactionType))
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                    MessageVM.msgTransactionNotDeclared);
                }

                CommonDAL commonDal = new CommonDAL();
                if (Master.TransactionType == "Treasury" || Master.TransactionType == "Treasury-Opening")
                {
                    newID = commonDal.TransactionCode("SDDeposit", "Treasury", "SDDeposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, currConn, transaction);
                }
                else if (Master.TransactionType == "Treasury-Credit" || Master.TransactionType == "Treasury-Opening-Credit")
                {
                    newID = commonDal.TransactionCode("SDDeposit", "Treasury-Credit", "SDDeposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, currConn, transaction);
                }


                #endregion Purchase ID Create Not Complete
                #region ID generated completed,Insert new Information in Header


                sqlText = "";
                sqlText += " insert into SDDeposits(";
                sqlText += " Id,	";
                sqlText += " DepositId,	";
                sqlText += " TreasuryNo,";
                sqlText += " DepositDateTime,";
                sqlText += " DepositType,";
                sqlText += " DepositAmount,";
                sqlText += " ChequeNo,";
                sqlText += " ChequeBank,";
                sqlText += " ChequeBankBranch,";
                sqlText += " ChequeDate,";
                sqlText += " BankID,";
                sqlText += " TreasuryCopy,";
                sqlText += " DepositPerson,";
                sqlText += " DepositPersonDesignation,";
                sqlText += " Comments,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " TransactionType,";
                sqlText += " ReverseDepositId,";
                sqlText += " Post";
                sqlText += " )";



                sqlText += " values";
                sqlText += " (";
                sqlText += "@Id,";
                sqlText += "@newID,";
                sqlText += "@MasterTreasuryNo,";
                sqlText += "@MasterDepositDate,";
                sqlText += "@MasterDepositType,";
                sqlText += "@MasterDepositAmount,";
                sqlText += "@MasterChequeNo,";
                sqlText += "@MasterChequeBank,";
                sqlText += "@MasterChequeBankBranch,";
                sqlText += "@MasterChequeDate,";
                sqlText += "@MasterBankId,";
                sqlText += "@MasterTreasuryCopy,";
                sqlText += "@MasterDepositPerson,";
                sqlText += "@MasterDepositPersonDesignation,";
                sqlText += "@MasterComments,";
                sqlText += "@MasterCreatedBy,";
                sqlText += "@MasterCreatedOn,";
                sqlText += "@MasterLastModifiedBy,";
                sqlText += "@MasterLastModifiedOn,";
                sqlText += "@MasterTransactionType,";
                sqlText += "@MasterReturnID,";
                sqlText += "@MasterPost";
                sqlText += ")";

                var Id = _cDal.NextId("SDDeposits", null, null).ToString();

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@Id", Id);
                cmdInsert.Parameters.AddWithValue("@newID", newID);
                cmdInsert.Parameters.AddWithValue("@MasterTreasuryNo", Master.TreasuryNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDepositDate", Master.DepositDate);
                cmdInsert.Parameters.AddWithValue("@MasterDepositType", Master.DepositType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDepositAmount", Master.DepositAmount);
                cmdInsert.Parameters.AddWithValue("@MasterChequeNo", Master.ChequeNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterChequeBank", Master.ChequeBank ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterChequeBankBranch", Master.ChequeBankBranch ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterChequeDate", Master.ChequeDate);
                cmdInsert.Parameters.AddWithValue("@MasterBankId", Master.BankId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTreasuryCopy", Master.TreasuryCopy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDepositPerson", Master.DepositPerson ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDepositPersonDesignation", Master.DepositPersonDesignation ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterReturnID", Master.ReturnID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.dpMsgSaveNotSuccessfully);
                }

                #endregion ID generated completed,Insert new Information in Header



                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.SDDeposits WHERE DepositId=@newID ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@newID", newID);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgUnableCreatID);
                }


                #endregion Prefetch
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
                retResults[1] = MessageVM.dpMsgSaveSuccessfully;
                retResults[2] = "" + Id;
                retResults[3] = "" + PostStatus;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (SqlException sqlex)
            {

                transaction.Rollback();
                throw sqlex;
            }
            catch (Exception ex)
            {

                transaction.Rollback();
                throw ex;
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

        public string[] DepositUpdate(SDDepositVM Master)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.DepositDate) < DateTime.MinValue || Convert.ToDateTime(Master.DepositDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate,
                        "Please Check Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.depMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DepositDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DepositDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK
                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(DepositId) from SDDeposits WHERE DepositId=@MasterId ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterId", Master.DepositId);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update SDDeposits set  ";
                sqlText += " TreasuryNo                 =@MasterTreasuryNo ,";
                sqlText += " DepositDateTime            =@MasterDepositDate ,";
                sqlText += " DepositType                =@MasterDepositType ,";
                sqlText += " DepositAmount              =@MasterDepositAmount ,";
                sqlText += " ChequeNo                   =@MasterChequeNo ,";
                sqlText += " ChequeBank                 =@MasterChequeBank ,";
                sqlText += " ChequeBankBranch           =@MasterChequeBankBranch ,";
                sqlText += " ChequeDate                 =@MasterChequeDate ,";
                sqlText += " BankID                     =@MasterBankId ,";
                sqlText += " TreasuryCopy               =@MasterTreasuryCopy ,";
                sqlText += " DepositPerson              =@MasterDepositPerson ,";
                sqlText += " DepositPersonDesignation   =@MasterDepositPersonDesignation ,";
                sqlText += " Comments                   =@MasterComments ,";
                sqlText += " LastModifiedBy             =@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn             =@MasterLastModifiedOn ,";
                sqlText += " TransactionType            =@MasterTransactionType ,";
                sqlText += " ReverseDepositId           =@MasterReturnID ,";
                sqlText += " Post                       =@MasterPost ";
                sqlText += " where DepositId            =@MasterId ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@MasterTreasuryNo", Master.TreasuryNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterDepositDate", Master.DepositDate);
                cmdUpdate.Parameters.AddWithValue("@MasterDepositType", Master.DepositType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterDepositAmount", Master.DepositAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterChequeNo", Master.ChequeNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterChequeBank", Master.ChequeBank ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterChequeBankBranch", Master.ChequeBankBranch ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterChequeDate", Master.ChequeDate);
                cmdUpdate.Parameters.AddWithValue("@MasterBankId", Master.BankId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterTreasuryCopy", Master.TreasuryCopy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterDepositPerson", Master.DepositPerson ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterDepositPersonDesignation", Master.DepositPersonDesignation ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterReturnID", Master.ReturnID ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterId", Master.DepositId ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header



                #endregion ID check completed,update Information in Header

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from SDDeposits WHERE DepositId=@MasterId ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterId", Master.DepositId);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgUnableCreatID);
                }


                #endregion Prefetch
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
                retResults[1] = MessageVM.dpMsgUpdateSuccessfully;
                retResults[2] = Master.Id;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {

                transaction.Rollback();
                throw sqlex;
            }
            catch (Exception ex)
            {

                transaction.Rollback();
                throw ex;
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

        public string[] DepositPost(SDDepositVM Master)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

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
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.DepositDate) < DateTime.MinValue || Convert.ToDateTime(Master.DepositDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate,
                        "Please Check Data and Time");

                }
                
                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.depMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DepositDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DepositDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK
                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(DepositId) from SDDeposits WHERE DepositId=MasterId ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterId", Master.DepositId);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update SDDeposits set  ";
                sqlText += " LastModifiedBy =@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn =@MasterLastModifiedOn ,";
                sqlText += " Post           =@MasterPost ";
                sqlText += " where DepositId=@MasterId ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post);
                cmdUpdate.Parameters.AddWithValue("@MasterId", Master.DepositId);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header



                #endregion ID check completed,update Information in Header
              

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from SDDeposits WHERE DepositId=@MasterId";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterId", Master.DepositId);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.dpMsgUnableCreatID);
                }


                #endregion Prefetch
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
                retResults[1] = MessageVM.dpMsgPostSuccessfully;
                retResults[2] = Master.DepositId;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {

                transaction.Rollback();
                throw sqlex;
            }
            catch (Exception ex)
            {

                transaction.Rollback();
                throw ex;
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

        //==================SelectAll=================
        public List<SDDepositVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SDDepositVM> VMs = new List<SDDepositVM>();
            SDDepositVM vm;
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
 d.Id
,d.DepositId
,d.TreasuryNo
,d.DepositDateTime
,d.DepositType
,d.DepositAmount
,d.ChequeNo
,d.ChequeBank
,d.ChequeBankBranch
,d.ChequeDate
,d.BankID
,d.TreasuryCopy
,d.DepositPerson
,d.DepositPersonDesignation
,d.Comments
,d.CreatedBy
,d.CreatedOn
,d.LastModifiedBy
,d.LastModifiedOn
,d.TransactionType
,d.Post
,d.ReverseDepositId

,b.BankName
,b.BranchName
,b.AccountNumber


FROM SDDeposits d
LEFT OUTER JOIN BankInformations b ON d.BankID = b.BankID
WHERE  1=1
";


                if (Id > 0)
                {
                    sqlText += @" and d.Id=@Id";
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
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SDDepositVM();
                    vm.Id = dr["Id"].ToString();
                    vm.DepositId = dr["DepositId"].ToString();
                    vm.TreasuryNo = dr["TreasuryNo"].ToString();
                    vm.DepositDate = dr["DepositDateTime"].ToString();
                    vm.DepositType = dr["DepositType"].ToString();
                    vm.DepositAmount = Convert.ToDecimal(dr["DepositAmount"].ToString());
                    vm.ChequeNo = dr["ChequeNo"].ToString();
                    vm.ChequeBank = dr["ChequeBank"].ToString();
                    vm.ChequeBankBranch = dr["ChequeBankBranch"].ToString();
                    vm.ChequeDate = dr["ChequeDate"].ToString();
                    vm.BankId = dr["BankID"].ToString();
                    vm.TreasuryCopy = dr["TreasuryCopy"].ToString();
                    vm.DepositPerson = dr["DepositPerson"].ToString();
                    vm.DepositPersonDesignation = dr["DepositPersonDesignation"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Post = dr["Post"].ToString();


                    vm.BankName = dr["BankName"].ToString();
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.AccountNumber = dr["AccountNumber"].ToString();
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
                throw new ArgumentNullException("", "SQL:" + sqlText  + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + ex.Message.ToString());
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


    }
}
