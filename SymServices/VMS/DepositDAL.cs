
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
    public class DepositDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();

        #endregion

        public DataTable SearchDepositNew(string DepositId, string TreasuryNo, string DepositDateFrom, string DepositDateTo,
            string DepositType, string ChequeNo, string ChequeDateFrom, string ChequeDateTo,
            string BankName, string BranchName, string AccountNumber, string TransactionType, string Post)
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Deposit");

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
                                Deposits.DepositId,
                                isnull(Deposits.TreasuryNo,'N/A')TreasuryNo,
                                convert (varchar,Deposits.DepositDateTime,120)DepositDateTime,
                                isnull(Deposits.DepositType,'N/A')DepositType,
                                isnull(Deposits.DepositAmount,0)DepositAmount ,
                                isnull(Deposits.ChequeNo,'N/A')ChequeNo,
                                isnull(Deposits.ChequeBank,'N/A')ChequeBank,isnull(Deposits.ChequeBankBranch,'N/A')ChequeBankBranch,
                                convert (varchar,Deposits.ChequeDate,120)ChequeDate,                                
                                Deposits.BankID,                                
                                isnull(BankInformations.BankName,'N/A')BankName ,                                
                                isnull(BankInformations.BranchName,'N/A')BranchName,                                
                                isnull(BankInformations.AccountNumber,'N/A')AccountNumber,                                
                                isnull(Deposits.DepositPerson,'N/A')DepositPerson ,                                
                                isnull(Deposits.DepositPersonDesignation,'N/A')DepositPersonDesignation ,                                
                                isnull(Deposits.Comments,'N/A')Comments,
                                Deposits.Post,
                                isnull(Deposits.ReverseDepositId,'')ReverseDepositId,
                                ISNULL(NULLIF(Deposits.TransactionType,''),'Treasury')TransactionType
                                FROM Deposits LEFT OUTER JOIN
                                BankInformations ON Deposits.BankID = BankInformations.BankID
   
                                WHERE 
                                (DepositId  LIKE '%' +  @DepositId + '%' OR @DepositId IS NULL) 
                                AND (TreasuryNo LIKE '%' + @TreasuryNo + '%' OR @TreasuryNo IS NULL)
                                AND (DepositDateTime >= @DepositDateFrom OR @DepositDateFrom IS NULL)
                                AND (DepositDateTime <dateadd(d,1, @DepositDateTo) OR @DepositDateTo IS NULL)
                                AND (DepositType LIKE '%' + @DepositType + '%' OR @DepositType IS NULL)
                                AND (ChequeNo LIKE '%' + @ChequeNo + '%' OR @ChequeNo IS NULL)
                                AND (ChequeDate >= @ChequeDateFrom OR @ChequeDateFrom IS NULL)
                                AND (ChequeDate <dateadd(d,1, @ChequeDateTo) OR @ChequeDateTo IS NULL)

                                AND (BankName LIKE '%' + @BankName + '%' OR @BankName IS NULL)
                                AND (BranchName	 LIKE '%' + @BranchName	 + '%' OR @BranchName	 IS NULL)
                                AND (AccountNumber LIKE '%' + @AccountNumber + '%' OR @AccountNumber IS NULL)
                                AND (Post LIKE '%' + @Post + '%' OR @Post IS NULL) ";
                //--AND (Deposits.TransactionType LIKE '%' + @TransactionType + '%' OR @TransactionType IS NULL)
                //--AND (Deposits.TransactionType LIKE '%' + @TransactionType OR @TransactionType IS NULL)

                //--order by DepositDateTime desc
                if (TransactionType == "Treasury")
                {
                    sqlText += "AND (Deposits.TransactionType IN ('Treasury','Treasury-Opening'))";
                }
                else if (TransactionType == "VDS" || TransactionType == "SaleVDS")
                {
                    sqlText += "AND (Deposits.TransactionType IN ('VDS','SaleVDS'))";
                }
                else
                {
                    sqlText += "   AND (Deposits.TransactionType= @TransactionType )";
                }
                sqlText += " order by DepositDateTime desc";

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



        public string[] DepositInsert(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory, SqlTransaction transaction, SqlConnection currConn)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            var adjustmentId = "";
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            SqlConnection vcurrConn = currConn;
            if (vcurrConn == null)
            {
                currConn = null;
                transaction = null;
            }
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

                if (vcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction(MessageVM.depMsgMethodNameInsert);
                }


                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionDate = Master.DepositDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DepositDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where @transactionYearCheck between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@transactionYearCheck", transactionYearCheck);

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
                sqlText = sqlText + "select COUNT(DepositId) from Deposits WHERE DepositId=@MasterId";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);

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
                    newID = commonDal.TransactionCode("Deposit", "Treasury", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, currConn, transaction);
                }

                else if (Master.TransactionType == "VDS")
                {
                    newID = commonDal.TransactionCode("Deposit", "VDS", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, currConn, transaction);
                }
                else if (Master.TransactionType == "SaleVDS")
                {
                    newID = commonDal.TransactionCode("Deposit", "VDS", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, currConn, transaction);
                }
                else if (Master.TransactionType == "AdjCashPayble")
                {
                    newID = commonDal.TransactionCode("Deposit", "AdjCashPayble", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, currConn, transaction);
                }
                else if (Master.TransactionType == "Treasury-Credit" || Master.TransactionType == "Treasury-Opening-Credit")
                {
                    newID = commonDal.TransactionCode("Deposit", "Treasury-Credit", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, currConn, transaction);
                }

                else if (Master.TransactionType == "VDS-Credit")
                {
                    newID = commonDal.TransactionCode("Deposit", "VDS-Credit", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, currConn, transaction);
                }
                else if (Master.TransactionType == "AdjCashPayble-Credit")
                {
                    newID = commonDal.TransactionCode("Deposit", "AdjCashPayble-Credit", "Deposits", "DepositId",
                                             "DepositDateTime", Master.DepositDate, currConn, transaction);
                }


                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header


                sqlText = "";
                sqlText += " insert into Deposits(";
                //sqlText += " Id,	";
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
                //sqlText += "@Id,";
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

                var Id = _cDal.NextId("Deposits", null, null).ToString();

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                //cmdInsert.Parameters.AddWithValue("@Id", Id);
                cmdInsert.Parameters.AddWithValue("@newID", newID);
                cmdInsert.Parameters.AddWithValue("@MasterTreasuryNo", Master.TreasuryNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDepositDate", Master.DepositDate ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDepositType", Master.DepositType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDepositAmount", Master.DepositAmount);
                cmdInsert.Parameters.AddWithValue("@MasterChequeNo", Master.ChequeNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterChequeBank", Master.ChequeBank ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterChequeBankBranch", Master.ChequeBankBranch ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterChequeDate", Master.ChequeDate ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterBankId", Master.BankId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTreasuryCopy", Master.TreasuryCopy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDepositPerson", Master.DepositPerson ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDepositPersonDesignation", Master.DepositPersonDesignation ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterReturnID", Master.ReturnID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterPost", Master.Post ??"N");

                transResult = (int)cmdInsert.ExecuteNonQuery();
                Master.Id = Id;
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.dpMsgSaveNotSuccessfully);
                }

                #endregion ID generated completed,Insert new Information in Header

                if (Master.TransactionType == "AdjCashPayble" || Master.TransactionType == "AdjCashPayble-Credit")
                {
                    #region new id generation
                    sqlText = "select isnull(max(cast(AdjHistoryID as int)),0)+1 FROM  AdjustmentHistorys";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    int nextId = (int)cmdNextId.ExecuteScalar();
                    if (nextId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.AdjmsgMethodNameInsert, "Unable to create new Adjustment No");
                    }
                    #endregion product type new id generation


                    #region Insert

                    sqlText = "";



                    sqlText += "insert into AdjustmentHistorys";
                    sqlText += "(";

                    sqlText += " AdjHistoryID,";
                    sqlText += " AdjId,";
                    sqlText += " AdjDate,";
                    sqlText += " AdjAmount,";
                    sqlText += " AdjVATRate,";
                    sqlText += " AdjVATAmount,";
                    sqlText += " AdjSD,";
                    sqlText += " AdjSDAmount,";
                    sqlText += " AdjOtherAmount,";
                    sqlText += " AdjType,";
                    sqlText += " AdjDescription,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " AdjInputAmount,";
                    sqlText += " AdjInputPercent,";
                    sqlText += " AdjReferance,";
                    sqlText += " Post,";
                    sqlText += " ReverseAdjHistoryNo,";
                    sqlText += " AdjHistoryNo";

                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += "@nextId ,";
                    sqlText += "@adjHistoryAdjId,";
                    sqlText += "@adjHistoryAdjDate,";
                    sqlText += "@adjHistoryAdjAmount ,";
                    sqlText += "@adjHistoryAdjVATRate,";
                    sqlText += "@adjHistoryAdjVATAmount,";
                    sqlText += "@adjHistoryAdjSD,";
                    sqlText += "@adjHistoryAdjSDAmount,";
                    sqlText += "@adjHistoryAdjOtherAmount,";
                    sqlText += "@adjHistoryAdjType,";
                    sqlText += "@adjHistoryAdjDescription,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@adjHistoryAdjInputAmount,";
                    sqlText += "@adjHistoryAdjInputPercent,";
                    sqlText += "@adjHistoryAdjReferance,";
                    sqlText += "@MasterPost,";
                    sqlText += "@MasterReturnID,";
                    sqlText += "@newID";


                    sqlText += ")";

                    SqlCommand cmdInsert1 = new SqlCommand(sqlText, currConn);
                    cmdInsert1.Transaction = transaction;

                    cmdInsert1.Parameters.AddWithValue("@nextId", nextId);
                    cmdInsert1.Parameters.AddWithValue("@adjHistoryAdjId", adjHistory.AdjId);
                    cmdInsert1.Parameters.AddWithValue("@adjHistoryAdjDate", adjHistory.AdjDate);
                    cmdInsert1.Parameters.AddWithValue("@adjHistoryAdjAmount", adjHistory.AdjAmount);
                    cmdInsert1.Parameters.AddWithValue("@adjHistoryAdjVATRate", adjHistory.AdjVATRate);
                    cmdInsert1.Parameters.AddWithValue("@adjHistoryAdjVATAmount", adjHistory.AdjVATAmount);
                    cmdInsert1.Parameters.AddWithValue("@adjHistoryAdjSD", adjHistory.AdjSD);
                    cmdInsert1.Parameters.AddWithValue("@adjHistoryAdjSDAmount", adjHistory.AdjSDAmount);
                    cmdInsert1.Parameters.AddWithValue("@adjHistoryAdjOtherAmount", adjHistory.AdjOtherAmount);
                    cmdInsert1.Parameters.AddWithValue("@adjHistoryAdjType", adjHistory.AdjType);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjDescription", adjHistory.AdjDescription);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsert1.Parameters.AddWithValue("@adjHistoryAdjInputAmount", adjHistory.AdjInputAmount);
                    cmdInsert1.Parameters.AddWithValue("@adjHistoryAdjInputPercent", adjHistory.AdjInputPercent);
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjReferance", adjHistory.AdjReferance);
                    cmdInsert1.Parameters.AddWithValue("@MasterPost", Master.Post??"N");
                    cmdInsert1.Parameters.AddWithValueAndNullHandle("@MasterReturnID", Master.ReturnID);
                    cmdInsert1.Parameters.AddWithValue("@newID", newID);

                   transResult=(int)cmdInsert1.ExecuteNonQuery();
                   adjustmentId = nextId.ToString();
                    #endregion Insert
                }

                else if (Master.TransactionType == "VDS"
                    || Master.TransactionType == "VDS-Credit"
                    || Master.TransactionType == "SaleVDS"

                    )
                {
                    #region Validation for Detail

                    if (vds == null || vds.Count() == 0)
                    {
                        retResults[1] = "Add details!";
                        throw new ArgumentNullException(retResults[1], "");
                    }


                    #endregion Validation for Detail

                    #region Insert Detail Table

                    foreach (var Item in vds.ToList())
                    {
                        #region Find Transaction Exist

                        if (Item.PurchaseNumber.Trim().ToUpper() != "NA")
                        {
                            sqlText = "";
                            sqlText += "select COUNT(VDSId) from VDS WHERE DepositNumber=@newID ";
                            sqlText += " AND PurchaseNumber=@ItemPurchaseNumber ";
                            SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                            cmdFindId.Transaction = transaction;
                            cmdFindId.Parameters.AddWithValue("@newID", newID);
                            cmdFindId.Parameters.AddWithValue("@ItemPurchaseNumber", Item.PurchaseNumber);

                            IDExist = (int)cmdFindId.ExecuteScalar();

                            if (IDExist > 0)
                            {
                                throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                                MessageVM.dpMsgFindExistIDP);
                            }
                        }

                        #endregion Find Transaction Exist

                        if (string.IsNullOrEmpty(Item.DepositNumber))
                        {
                            Item.DepositNumber = newID;

                        }
                        #region Insert only DetailTable

                        sqlText = "";
                        sqlText += " insert into VDS(";

                        sqlText += " VDSId";
                        sqlText += " ,VendorId";
                        sqlText += " ,BillAmount";
                        sqlText += " ,BillDate";
                        sqlText += " ,BillDeductAmount";
                        sqlText += " ,DepositNumber";
                        sqlText += " ,PurchaseNumber";
                        sqlText += " ,DepositDate";
                        sqlText += " ,Remarks";
                        sqlText += " ,IssueDate";
                        sqlText += " ,CreatedBy";
                        sqlText += " ,CreatedOn";
                        sqlText += " ,LastModifiedBy";
                        sqlText += " ,LastModifiedOn";
                        sqlText += " ,VDSPercent";
                        sqlText += " ,IsPercent";
                        sqlText += " ,IsPurchase";
                        sqlText += " ,ReverseVDSId";
                        sqlText += " )";

                        sqlText += " values(	";

                        sqlText += "@newID, ";
                        sqlText += "@ItemVendorId ,";
                        sqlText += "@ItemBillAmount ,";
                        sqlText += "@ItemBillDate ,";
                        sqlText += "@ItemBillDeductedAmount ,";
                        sqlText += "@ItemDepositNumber ,";
                        sqlText += "@ItemPurchaseNumber ,";
                        sqlText += "@MasterDepositDate ,";
                        sqlText += "@ItemRemarks ,";
                        sqlText += "@ItemIssueDate ,";
                        sqlText += "@MasterCreatedBy ,";
                        sqlText += "@MasterCreatedOn ,";
                        sqlText += "@MasterLastModifiedBy ,";
                        sqlText += "@MasterLastModifiedOn ,";
                        sqlText += "@ItemVDSPercent ,";
                        sqlText += "@ItemIsPercent ,";
                        sqlText += "@ItemIsPurchase ,";
                        sqlText += "@MasterReturnID ";

                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@newID", newID);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVendorId", Item.VendorId);
                        cmdInsDetail.Parameters.AddWithValue("@ItemBillAmount", Item.BillAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemBillDate", Item.BillDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemBillDeductedAmount", Item.BillDeductedAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemDepositNumber", Item.DepositNumber ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseNumber", Item.PurchaseNumber);
                        cmdInsDetail.Parameters.AddWithValue("@MasterDepositDate", Master.DepositDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemRemarks", Item.Remarks ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemIssueDate", Item.IssueDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVDSPercent", Item.VDSPercent);
                        cmdInsDetail.Parameters.AddWithValue("@ItemIsPercent", Item.IsPercent ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemIsPurchase", Item.IsPurchase ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterReturnID", Master.ReturnID ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert,
                                                            MessageVM.dpMsgSaveNotSuccessfully);
                        }

                        #endregion Insert only DetailTable


                    }


                    #endregion Insert Detail Table

                }

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.Deposits WHERE DepositId=@newID";
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

                if (vcurrConn == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                        }
                    }
                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.dpMsgSaveSuccessfully;
                retResults[2] = "" + Master.Id;
                if (Master.TransactionType == "AdjCashPayble")
                {
                    retResults[2] = "" + adjustmentId;
                }
                retResults[3] = "" + PostStatus;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex

                if (vcurrConn == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (vcurrConn == null)
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

            #endregion Catch and Finall

            #region Result

            return retResults;

            #endregion Result
        }
        public string[] DepositUpdate(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory)
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
                sqlText = sqlText + "select COUNT(DepositId) from Deposits WHERE DepositId=@MasterId ";
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

                sqlText += " update Deposits set  ";
                sqlText += " TreasuryNo                 =@MasterTreasuryNo,";
                sqlText += " DepositDateTime            =@MasterDepositDate,";
                sqlText += " DepositType                =@MasterDepositType,";
                sqlText += " DepositAmount              =@MasterDepositAmount,";
                sqlText += " ChequeNo                   =@MasterChequeNo,";
                sqlText += " ChequeBank                 =@MasterChequeBank,";
                sqlText += " ChequeBankBranch           =@MasterChequeBankBranch,";
                sqlText += " ChequeDate                 =@MasterChequeDate,";
                sqlText += " BankID                     =@MasterBankId,";
                sqlText += " TreasuryCopy               =@MasterTreasuryCopy,";
                sqlText += " DepositPerson              =@MasterDepositPerson,";
                sqlText += " DepositPersonDesignation   =@MasterDepositPersonDesignation,";
                sqlText += " Comments                   =@MasterComments,";
                sqlText += " LastModifiedBy             =@MasterLastModifiedBy,";
                sqlText += " LastModifiedOn             =@MasterLastModifiedOn,";
                sqlText += " TransactionType            =@MasterTransactionType,";
                sqlText += " ReverseDepositId           =@MasterReturnID,";
                sqlText += " Post                       =@MasterPost";
                sqlText += " where DepositId            =@MasterId";

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
                cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post ?? "N");
                cmdUpdate.Parameters.AddWithValue("@MasterId", Master.DepositId ?? Convert.DBNull);


                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header

                #region AdjCashPayble
                if (Master.TransactionType == "AdjCashPayble" || Master.TransactionType == "AdjCashPayble-Credit")
                {
                    #region Find ID for Update

                    int countId = 0;
                    #region product type existence checking

                    sqlText = "select count(AdjHistoryNo) from AdjustmentHistorys where  AdjHistoryNo =@MasterId";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@MasterId", Master.DepositId);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                        "Could not find requested Adjustment id.");
                    }
                    #endregion
                    #region same name product type existence checking
                    countId = 0;
                    sqlText = "select count(AdjHistoryNo) from AdjustmentHistorys where  AdjId=@adjHistoryAdjId ";
                    sqlText += " and AdjDate=@adjHistoryAdjDate ";
                    sqlText += " and AdjType=@adjHistoryAdjType ";
                    sqlText += " and not AdjHistoryNo =@MasterId ";
                    SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                    cmdNameExist.Transaction = transaction;
                    cmdNameExist.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjId", adjHistory.AdjId);
                    cmdNameExist.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjDate", adjHistory.AdjDate);
                    cmdNameExist.Parameters.AddWithValueAndNullHandle("@adjHistoryAdjType", adjHistory.AdjType);
                    cmdNameExist.Parameters.AddWithValueAndNullHandle("@MasterId", Master.DepositId);

                    countId = (int)cmdNameExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                        "Same Adjustment name already exist in same date");
                    }
                    #endregion
                    #endregion Find ID for Update

                    #region sql statement

                    sqlText = "";
                    sqlText += "UPDATE AdjustmentHistorys SET";
                    sqlText += " AdjId              =@adjHistoryAdjId,";
                    sqlText += " AdjDate            =@adjHistoryAdjDate,";
                    sqlText += " AdjAmount          =@adjHistoryAdjAmount,";
                    sqlText += " AdjVATRate         =@adjHistoryAdjVATRate,";
                    sqlText += " AdjVATAmount       =@adjHistoryAdjVATAmount,";
                    sqlText += " AdjSD              =@adjHistoryAdjSD,";
                    sqlText += " AdjSDAmount        =@adjHistoryAdjSDAmount,";
                    sqlText += " AdjOtherAmount     =@adjHistoryAdjOtherAmount,";
                    sqlText += " AdjType            =@adjHistoryAdjType,";
                    sqlText += " AdjDescription     =@adjHistoryAdjDescription,";
                    sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                    sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                    sqlText += " AdjInputAmount     =@adjHistoryAdjInputAmount,";
                    sqlText += " AdjInputPercent    =@adjHistoryAdjInputPercent,";
                    sqlText += " AdjReferance       =@adjHistoryAdjReferance,";
                    sqlText += " ReverseAdjHistoryNo=@MasterReturnID,";
                    sqlText += " Post               =@MasterPost";
                    sqlText += " WHERE AdjHistoryNo =@MasterId";

                    SqlCommand cmdUpdateACP = new SqlCommand(sqlText, currConn);
                    cmdUpdateACP.Transaction = transaction;

                    cmdUpdateACP.Parameters.AddWithValue("@adjHistoryAdjId", adjHistory.AdjId ?? Convert.DBNull);
                    cmdUpdateACP.Parameters.AddWithValue("@adjHistoryAdjDate", adjHistory.AdjDate);
                    cmdUpdateACP.Parameters.AddWithValue("@adjHistoryAdjAmount", adjHistory.AdjAmount);
                    cmdUpdateACP.Parameters.AddWithValue("@adjHistoryAdjVATRate", adjHistory.AdjVATRate);
                    cmdUpdateACP.Parameters.AddWithValue("@adjHistoryAdjVATAmount", adjHistory.AdjVATAmount);
                    cmdUpdateACP.Parameters.AddWithValue("@adjHistoryAdjSD", adjHistory.AdjSD);
                    cmdUpdateACP.Parameters.AddWithValue("@adjHistoryAdjSDAmount", adjHistory.AdjSDAmount);
                    cmdUpdateACP.Parameters.AddWithValue("@adjHistoryAdjOtherAmount", adjHistory.AdjOtherAmount);
                    cmdUpdateACP.Parameters.AddWithValue("@adjHistoryAdjType", adjHistory.AdjType ?? Convert.DBNull);
                    cmdUpdateACP.Parameters.AddWithValue("@adjHistoryAdjDescription", adjHistory.AdjDescription ?? Convert.DBNull);
                    cmdUpdateACP.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdUpdateACP.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdUpdateACP.Parameters.AddWithValue("@adjHistoryAdjInputAmount", adjHistory.AdjInputAmount);
                    cmdUpdateACP.Parameters.AddWithValue("@adjHistoryAdjInputPercent", adjHistory.AdjInputPercent);
                    cmdUpdateACP.Parameters.AddWithValue("@adjHistoryAdjReferance", adjHistory.AdjReferance);
                    cmdUpdateACP.Parameters.AddWithValue("@MasterReturnID", Master.ReturnID ?? Convert.DBNull);
                    cmdUpdateACP.Parameters.AddWithValue("@MasterPost", Master.Post ?? "N");
                    cmdUpdateACP.Parameters.AddWithValue("@MasterId", Master.DepositId ?? Convert.DBNull);

                    transResult = (int)cmdUpdateACP.ExecuteNonQuery();

                    #endregion
                }
                #endregion AdjCashPayble

                #endregion ID check completed,update Information in Header
                #region VDS
                if (Master.TransactionType == "VDS" || Master.TransactionType == "VDS-Credit")
                {
                    if (vds.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                    }

                    foreach (var Item in vds.ToList())
                    {
                        #region Find Transaction Exist

                        sqlText = "";
                        sqlText += "select COUNT(VDSId) from VDS WHERE VDSId=@MasterId ";
                        sqlText += " AND PurchaseNumber=@ItemPurchaseNumber ";
                        sqlText += " AND VendorId=@ItemVendorId ";
                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;
                        cmdFindId.Parameters.AddWithValue("@MasterId", Master.DepositId);
                        cmdFindId.Parameters.AddWithValue("@ItemPurchaseNumber", Item.PurchaseNumber);
                        cmdFindId.Parameters.AddWithValue("@ItemVendorId", Item.VendorId);

                        IDExist = (int)cmdFindId.ExecuteScalar();

                        if (IDExist <= 0)
                        {
                            #region Insert only DetailTable

                            sqlText = "";
                            sqlText += " insert into VDS(";

                            sqlText += " VDSId";
                            sqlText += " ,VendorId";
                            sqlText += " ,BillAmount";
                            sqlText += " ,BillDate";
                            sqlText += " ,BillDeductAmount";
                            sqlText += " ,DepositNumber";
                            sqlText += " ,PurchaseNumber";
                            sqlText += " ,DepositDate";
                            sqlText += " ,Remarks";
                            sqlText += " ,IssueDate";
                            sqlText += " ,CreatedBy";
                            sqlText += " ,CreatedOn";
                            sqlText += " ,LastModifiedBy";
                            sqlText += " ,LastModifiedOn";
                            sqlText += " ,VDSPercent";
                            sqlText += " ,IsPercent";
                            sqlText += " ,IsPurchase";
                            sqlText += " ,ReverseVDSId";
                            sqlText += " )";

                            sqlText += " values(	";

                            sqlText += "@MasterId,";
                            sqlText += "@ItemVendorId,";
                            sqlText += "@ItemBillAmount,";
                            sqlText += "@ItemBillDate,";
                            sqlText += "@ItemBillDeductedAmount,";
                            sqlText += "@ItemDepositNumber,";
                            sqlText += "@ItemPurchaseNumber,";
                            sqlText += "@MasterDepositDate,";
                            sqlText += "@ItemRemarks,";
                            sqlText += "@ItemIssueDate,";
                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy,";
                            sqlText += "@MasterLastModifiedOn,";
                            sqlText += "@ItemVDSPercent,";
                            sqlText += "@ItemIsPercent,";
                            sqlText += "@ItemIsPurchase,";
                            sqlText += "@MasterReturnID";

                            sqlText += ")	";


                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;
                            cmdInsDetail.Parameters.AddWithValue("@MasterId", Master.DepositId);
                            cmdInsDetail.Parameters.AddWithValue("@ItemVendorId", Item.VendorId);
                            cmdInsDetail.Parameters.AddWithValue("@ItemBillAmount", Item.BillAmount);
                            cmdInsDetail.Parameters.AddWithValue("@ItemBillDate", Item.BillDate);
                            cmdInsDetail.Parameters.AddWithValue("@ItemBillDeductedAmount", Item.BillDeductedAmount);
                            cmdInsDetail.Parameters.AddWithValue("@ItemDepositNumber", Item.DepositNumber);
                            cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseNumber", Item.PurchaseNumber);
                            cmdInsDetail.Parameters.AddWithValue("@MasterDepositDate", Master.DepositDate);
                            cmdInsDetail.Parameters.AddWithValue("@ItemRemarks", Item.Remarks);
                            cmdInsDetail.Parameters.AddWithValue("@ItemIssueDate", Item.IssueDate);
                            cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                            cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                            cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsDetail.Parameters.AddWithValue("@ItemVDSPercent", Item.VDSPercent);
                            cmdInsDetail.Parameters.AddWithValue("@ItemIsPercent", Item.IsPercent);
                            cmdInsDetail.Parameters.AddWithValue("@ItemIsPurchase", Item.IsPurchase);
                            cmdInsDetail.Parameters.AddWithValue("@MasterReturnID", Master.ReturnID);

                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate,
                                                                MessageVM.dpMsgUpdateNotSuccessfully);
                            }

                            #endregion Insert only DetailTable
                        }
                        else
                        {


                            sqlText = "";
                            sqlText += " update VDS set ";

                            sqlText += " BillAmount         =@ItemBillAmount ,";
                            sqlText += " BillDate           =@ItemBillDate ,";
                            sqlText += " BillDeductAmount   =@ItemBillDeductedAmount,";
                            sqlText += " DepositNumber      =@MasterId,";
                            sqlText += " PurchaseNumber     =@ItemPurchaseNumber,";
                            sqlText += " DepositDate        =@MasterDepositDate,";
                            sqlText += " Remarks            =@ItemRemarks,";
                            sqlText += " IssueDate          =@ItemIssueDate,";
                            sqlText += " LastModifiedBy     =@MasterLastModifiedBy ,";
                            sqlText += " LastModifiedOn     =@MasterLastModifiedOn ,";
                            sqlText += " VDSPercent         =@ItemVDSPercent, ";
                            sqlText += " ReverseVDSId       =@MasterReturnID, ";
                            sqlText += " IsPercent          =@ItemIsPercent,";
                            sqlText += " IsPurchase         =@ItemIsPurchase";
                            sqlText += " where  VDSId       =@MasterId ";
                            sqlText += " and PurchaseNumber =@ItemPurchaseNumber ";
                            sqlText += " and VendorId       =@ItemVendorId ";

                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;

                            cmdInsDetail.Parameters.AddWithValue("@ItemBillAmount", Item.BillAmount);
                            cmdInsDetail.Parameters.AddWithValue("@ItemBillDate", Item.BillDate);
                            cmdInsDetail.Parameters.AddWithValue("@ItemBillDeductedAmount", Item.BillDeductedAmount);
                            cmdInsDetail.Parameters.AddWithValue("@MasterId", Master.DepositId ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseNumber", Item.PurchaseNumber ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@MasterDepositDate", Master.DepositDate);
                            cmdInsDetail.Parameters.AddWithValue("@ItemRemarks", Item.Remarks ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@ItemIssueDate", Item.IssueDate);
                            cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsDetail.Parameters.AddWithValue("@ItemVDSPercent", Item.VDSPercent);
                            cmdInsDetail.Parameters.AddWithValue("@MasterReturnID", Master.ReturnID ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@ItemIsPercent", Item.IsPercent);
                            cmdInsDetail.Parameters.AddWithValue("@ItemIsPurchase", Item.IsPurchase);
                            //cmdInsDetail.Parameters.AddWithValue("@MasterId", Master.DepositId ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@ItemVendorId", Item.VendorId ?? Convert.DBNull);

                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate,
                                                                MessageVM.dpMsgUpdateNotSuccessfully);
                            }
                        }

                        #endregion Find Transaction Exist
                    }

                    #region Remove row
                    sqlText = "";
                    sqlText += " SELECT  distinct VendorId";
                    sqlText += " from VDS WHERE VDSId=@MasterId";

                    DataTable dt = new DataTable("Previous");
                    SqlCommand cmdPrevious = new SqlCommand(sqlText, currConn);
                    cmdPrevious.Transaction = transaction;
                    cmdPrevious.Parameters.AddWithValue("@MasterId", Master.DepositId);

                    SqlDataAdapter dta = new SqlDataAdapter(cmdPrevious);
                    dta.Fill(dt);
                    foreach (DataRow pVId in dt.Rows)
                    {
                        var p = pVId["VendorId"].ToString();

                        //var tt = Details.Find(x => x.ItemNo == p);
                        var tt = vds.Count(x => x.VendorId.Trim() == p.Trim());
                        if (tt == 0)
                        {
                            sqlText = "";
                            sqlText += " delete FROM VDS ";
                            sqlText += " WHERE VDSId=@MasterId";
                            sqlText += " AND VendorId=@p";

                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;
                            cmdInsDetail.Parameters.AddWithValue("@MasterId", Master.DepositId);
                            cmdInsDetail.Parameters.AddWithValue("@p", p);


                            transResult = (int)cmdInsDetail.ExecuteNonQuery();
                        }

                    }
                    #endregion Remove row
                }
                #endregion VDS

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from Deposits WHERE DepositId=@MasterId";
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

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex

                transaction.Rollback();
                return retResults;
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
        public string[] DepositPost(DepositMasterVM Master, List<VDSMasterVM> vds, AdjustmentHistoryVM adjHistory)
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
                sqlText = sqlText + "select COUNT(DepositId) from Deposits WHERE DepositId=@MasterId";
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

                sqlText += " update Deposits set  ";
                sqlText += " LastModifiedBy= @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn= @MasterLastModifiedOn ,";
                sqlText += " Post= @MasterPost ";
                sqlText += " where DepositId= @MasterId ";

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

                #region AdjCashPayble
                if (Master.TransactionType == "AdjCashPayble" || Master.TransactionType == "AdjCashPayble-Credit")
                {
                    #region Find ID for Update

                    countId = 0;
                    #region product type existence checking

                    sqlText = "select count(AdjHistoryNo) from AdjustmentHistorys where  AdjHistoryNo =@MasterId";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@MasterId", Master.DepositId);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                        "Could not find requested Adjustment id.");
                    }
                    #endregion
                    #region same name product type existence checking
                    countId = 0;
                    sqlText = "select count(AdjHistoryID) from AdjustmentHistorys where  AdjId=@adjHistoryAdjId ";
                    sqlText += " and AdjDate=@adjHistoryAdjDate";
                    sqlText += " and AdjType=@adjHistoryAdjType ";
                    sqlText += " and not AdjHistoryNo = @MasterId ";
                    SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                    cmdNameExist.Transaction = transaction;
                    cmdNameExist.Parameters.AddWithValue("@MasterId", Master.DepositId);
                    cmdNameExist.Parameters.AddWithValue("@adjHistoryAdjId", adjHistory.AdjId);
                    cmdNameExist.Parameters.AddWithValue("@adjHistoryAdjDate", adjHistory.AdjDate);
                    cmdNameExist.Parameters.AddWithValue("@adjHistoryAdjType", adjHistory.AdjType);

                    countId = (int)cmdNameExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                        "Same Adjustment name already exist in same date");
                    }
                    #endregion
                    #endregion Find ID for Update

                    #region sql statement

                    sqlText = "";
                    sqlText += "UPDATE AdjustmentHistorys SET";
                    sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                    sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                    sqlText += " Post=@MasterPost";

                    sqlText += " WHERE AdjHistoryNo =@MasterId";

                    SqlCommand cmdUpdateACP = new SqlCommand(sqlText, currConn);
                    cmdUpdateACP.Transaction = transaction;
                    cmdUpdateACP.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdUpdateACP.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdUpdateACP.Parameters.AddWithValue("@MasterPost", Master.Post);
                    cmdUpdateACP.Parameters.AddWithValue("@MasterId", Master.DepositId);


                    transResult = (int)cmdUpdateACP.ExecuteNonQuery();

                    #endregion
                }
                #endregion AdjCashPayble

                #region VDS
                if (Master.TransactionType == "VDS" || Master.TransactionType == "VDS-Credit")
                {
                    if (vds.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                    }

                    foreach (var Item in vds.ToList())
                    {
                        #region Find Transaction Exist

                        sqlText = "";
                        sqlText += "select COUNT(VDSId) from VDS WHERE VDSId=@MasterId";
                        sqlText += " AND PurchaseNumber=@ItemPurchaseNumber";
                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;
                        cmdFindId.Parameters.AddWithValue("@MasterId", Master.DepositId);
                        cmdFindId.Parameters.AddWithValue("@ItemPurchaseNumber", Item.PurchaseNumber);


                        IDExist = (int)cmdFindId.ExecuteScalar();

                        if (IDExist <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.depMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);

                        }
                        else
                        {


                            sqlText = "";
                            sqlText += " update VDS set ";

                            sqlText += " LastModifiedBy=@MasterLastModifiedBy ,";
                            sqlText += " LastModifiedOn=@MasterLastModifiedOn ";
                            sqlText += " where  VDSId=@MasterId ";
                            sqlText += " and PurchaseNumber=@ItemPurchaseNumber ";

                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;
                            cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsDetail.Parameters.AddWithValue("@MasterId", Master.DepositId);
                            cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseNumber", Item.PurchaseNumber);


                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.depMsgMethodNameUpdate,
                                                                MessageVM.dpMsgUpdateNotSuccessfully);
                            }
                        }

                        #endregion Find Transaction Exist
                    }
                }
                #endregion VDS

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from Deposits WHERE DepositId=@MasterId";
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
        public DataTable SearchVDSDT(string DepositNumber)
        {


            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("VDS");
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
                sqlText = @"SELECT VDS.VDSId, VDS.VendorId, Vendors.VendorName,
VDS.BillAmount,
VDS.VDSPercent,
VDS.BillDeductAmount VDSAmount,
convert (varchar,BillDate,120)PurchaseDate,
convert (varchar,IssueDate,120)IssueDate,
 VDS.Remarks,
VDS.DepositNumber,
VDS.IsPercent,
isnull(VDS.IsPurchase,'Y')IsPurchase,
--, VDS.DepositDate, 
 VDS.PurchaseNumber
--, VendorGroups.VendorGroupName
FROM VDS LEFT OUTER JOIN
Vendors ON VDS.VendorId = Vendors.VendorID LEFT OUTER JOIN
VendorGroups ON Vendors.VendorGroupID = VendorGroups.VendorGroupID

WHERE 	 (VDS.VDSId = @DepositNumber )";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;

                if (!objCommProductType.Parameters.Contains("@DepositNumber"))
                { objCommProductType.Parameters.AddWithValue("@DepositNumber", DepositNumber); }
                else { objCommProductType.Parameters["@DepositNumber"].Value = DepositNumber; }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
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

        public string[] ImportData(DataTable dtDeposit, DataTable dtVDS)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            DepositMasterVM depositMaster;
            List<VDSMasterVM> vdsMasterVMs = new List<VDSMasterVM>();
            AdjustmentHistoryVM adjustmentHistory = new AdjustmentHistoryVM();



            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable
            #region try
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    currConn.Open();
                }

                #region RowCount
                int MRowCount = 0;
                int MRow = dtDeposit.Rows.Count;
                for (int i = 0; i < dtDeposit.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtDeposit.Rows[i]["ID"].ToString()))
                    {
                        MRowCount++;
                    }

                }
                if (MRow != MRowCount)
                {
                    throw new ArgumentNullException("you have select " + MRow.ToString() + " data for import, but you have " + MRowCount + " id.");
                }
                #endregion RowCount
                #region ID in Master or Detail table

                for (int i = 0; i < MRowCount; i++)
                {
                    string importID = dtDeposit.Rows[i]["ID"].ToString();
                    if (!string.IsNullOrEmpty(importID))
                    {
                        DataRow[] DetailRaws = dtVDS.Select("ID='" + importID + "'");
                        if (DetailRaws.Length == 0)
                        {
                            throw new ArgumentNullException("The ID " + importID + " is not available in detail table");
                        }

                    }

                }

                #endregion
                #region Double ID in Master

                for (int i = 0; i < MRowCount; i++)
                {
                    string id = dtDeposit.Rows[i]["ID"].ToString();
                    DataRow[] tt = dtDeposit.Select("ID='" + id + "'");
                    if (tt.Length > 1)
                    {
                        throw new ArgumentNullException("you have duplicate master id " + id + " in import file.");
                    }

                }

                #endregion Double ID in Master

                CommonImport cImport = new CommonImport();

                #region checking from database is exist the information(NULL Check)
                #region Deposit


                for (int rows = 0; rows < MRowCount; rows++)
                {
                    #region Check Date

                    bool IsDepositDate, IsChequeDate;
                    IsDepositDate = cImport.CheckDate(dtDeposit.Rows[rows]["Deposit_Date"].ToString().Trim());
                    if (IsDepositDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Deposit_Date field.");
                    }
                    if (!string.IsNullOrEmpty(dtDeposit.Rows[rows]["Cheque_Date"].ToString().Trim()))
                    {
                        IsChequeDate = cImport.CheckDate(dtDeposit.Rows[rows]["Cheque_Date"].ToString().Trim());
                        if (IsChequeDate != true)
                        {
                            throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Cheque_Date field.");
                        }
                    }
                    #endregion Check Date
                    #region Deposit type check

                    bool depositType;
                    depositType = cImport.CheckDepositType(dtDeposit.Rows[rows]["Deposit_Type"].ToString().Trim());
                    if (depositType != true)
                    {
                        throw new ArgumentNullException("Please insert Cash/Cheque in Deposit_Type field.");
                    }

                    #endregion Deposit type check
                    #region Yes no check

                    bool post;
                    post = cImport.CheckYN(dtDeposit.Rows[rows]["Post"].ToString().Trim());
                    if (post != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Post field.");
                    }
                    #endregion Yes no check
                    #region Null value check
                    if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Bank_Name"].ToString().Trim()) || dtDeposit.Rows[rows]["Bank_Name"].ToString().Trim() == "-")
                    {
                        throw new ArgumentNullException("Please insert valid value in Bank_Name field.");
                    }
                    if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Branch_Name"].ToString().Trim()) || dtDeposit.Rows[rows]["Branch_Name"].ToString().Trim() == "-")
                    {
                        throw new ArgumentNullException("Please insert valid value in Branch_Name field.");
                    }
                    if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Account_No"].ToString().Trim()) || dtDeposit.Rows[rows]["Account_No"].ToString().Trim() == "-")
                    {
                        throw new ArgumentNullException("Please insert valid value in Account_No field.");
                    }
                    #endregion Null value check
                    #region Bank Check
                    string bankName = dtDeposit.Rows[rows]["Bank_Name"].ToString().Trim();
                    string branchName = dtDeposit.Rows[rows]["Branch_Name"].ToString().Trim();
                    string accNo = dtDeposit.Rows[rows]["Account_No"].ToString().Trim();

                    string bankId = cImport.CheckBankID(bankName, branchName, accNo, currConn, transaction);
                    if (string.IsNullOrEmpty(bankId))
                    {
                        throw new ArgumentNullException("FindBankId", "Bank '(" + bankName + ")' not in database");
                    }
                    #endregion Bank Check

                    #region Cheque Information check
                    if (dtDeposit.Rows[rows]["Deposit_Type"].ToString().Trim().ToUpper() == "CHEQUE")
                    {
                        if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Cheque_No"].ToString().Trim()) || dtDeposit.Rows[rows]["Cheque_No"].ToString().Trim() == "-")
                        {
                            throw new ArgumentNullException("Please insert valid value in Cheque_No field.");
                        }
                        if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Cheque_Date"].ToString().Trim()) || dtDeposit.Rows[rows]["Cheque_Date"].ToString().Trim() == "-")
                        {
                            throw new ArgumentNullException("Please insert valid value in Cheque_Date field.");
                        }
                        if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Cheque_Bank"].ToString().Trim()) || dtDeposit.Rows[rows]["Cheque_Bank"].ToString().Trim() == "-")
                        {
                            throw new ArgumentNullException("Please insert valid value in Cheque_Bank field.");
                        }
                        if (string.IsNullOrEmpty(dtDeposit.Rows[rows]["Cheque_Bank_Branch"].ToString().Trim()) || dtDeposit.Rows[rows]["Cheque_Bank_Branch"].ToString().Trim() == "-")
                        {
                            throw new ArgumentNullException("Please insert valid value in Cheque_Bank_Branch field.");
                        }
                    }
                    #endregion Cheque Information check
                }
                #endregion Deposit

                #region VDS
                #region Row count for vds table
                int DRowCount = 0;
                for (int i = 0; i < dtVDS.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtVDS.Rows[i]["ID"].ToString()))
                    {
                        DRowCount++;
                    }

                }

                #endregion Row count for vds table

                for (int i = 0; i < DRowCount; i++)
                {
                    #region FindVendorId
                    cImport.FindVendorId(dtVDS.Rows[i]["Vendor_Name"].ToString().Trim(),
                                           dtVDS.Rows[i]["Vendor_Code"].ToString().Trim(), currConn, transaction);
                    #endregion FindVendorId
                    #region Check Date

                    bool IsBillDate, IsIssueDate;
                    // BillDate=PurchaseDate
                    IsBillDate = cImport.CheckDate(dtVDS.Rows[i]["Bill_Date"].ToString().Trim());
                    if (IsBillDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Bill_Date field.");
                    }
                    IsIssueDate = cImport.CheckDate(dtVDS.Rows[i]["Issue_Date"].ToString().Trim());
                    if (IsIssueDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Issue_Date field.");
                    }

                    #endregion Check Date
                    #region Numeric value check
                    bool IsBillAmt, IsVDSAmt;
                    IsBillAmt = cImport.CheckNumericBool(dtVDS.Rows[i]["Bill_Amount"].ToString().Trim());
                    if (IsBillAmt != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Bill_Amount field.");
                    }
                    IsVDSAmt = cImport.CheckNumericBool(dtVDS.Rows[i]["VDS_Amount"].ToString().Trim());
                    if (IsVDSAmt != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in VDS_Amount field.");
                    }
                    #endregion Numeric value check
                }
                #endregion VDS

                #endregion checking from database is exist the information(NULL Check)

                #region Connection
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                    currConn.Open();
                    transaction = currConn.BeginTransaction("Import Data.");
                }
                #endregion

                for (int i = 0; i < MRowCount; i++)
                {
                    string importID = dtDeposit.Rows[i]["ID"].ToString().Trim();
                    string depositType = dtDeposit.Rows[i]["Deposit_Type"].ToString().Trim();
                    string depositDate = dtDeposit.Rows[i]["Deposit_Date"].ToString().Trim();
                    string post = dtDeposit.Rows[i]["Post"].ToString().Trim();
                    string treasuryNo = cImport.ChecKNullValue(dtDeposit.Rows[i]["Treasury_No"].ToString().Trim());
                    string chequeNo = cImport.ChecKNullValue(dtDeposit.Rows[i]["Cheque_No"].ToString().Trim());
                    string chequeDate = dtDeposit.Rows[i]["Cheque_Date"].ToString().Trim();
                    string chequeBank = cImport.ChecKNullValue(dtDeposit.Rows[i]["Cheque_Bank"].ToString().Trim());
                    string chqBBrunch = cImport.ChecKNullValue(dtDeposit.Rows[i]["Cheque_Bank_Branch"].ToString().Trim());
                    string bankName = cImport.ChecKNullValue(dtDeposit.Rows[i]["Bank_Name"].ToString().Trim());
                    string branchName = cImport.ChecKNullValue(dtDeposit.Rows[i]["Branch_Name"].ToString().Trim());
                    string accNo = dtDeposit.Rows[i]["Account_No"].ToString().Trim();
                    #region Bank Check
                    string bankId = cImport.CheckBankID(bankName, branchName, accNo, currConn, transaction);
                    #endregion Bank Check
                    string comment = cImport.ChecKNullValue(dtDeposit.Rows[i]["Comments"].ToString().Trim());
                    string dPerson = cImport.ChecKNullValue(dtDeposit.Rows[i]["Deposit_Person"].ToString().Trim());
                    string personDesg = cImport.ChecKNullValue(dtDeposit.Rows[i]["Person_Designation"].ToString().Trim());
                    string createdBy = dtDeposit.Rows[i]["Created_By"].ToString().Trim();
                    string lastModifiedBy = dtDeposit.Rows[i]["LastModified_By"].ToString().Trim();
                    string transactionType = dtDeposit.Rows[i]["Transection_Type"].ToString().Trim();

                    #region Deposit
                    depositMaster = new DepositMasterVM();
                    //depositMaster.DepositId = NextID.ToString();
                    depositMaster.TreasuryNo = treasuryNo;
                    depositMaster.DepositDate = Convert.ToDateTime(depositDate).ToString("yyyy-MM-dd HH:mm:ss");// dtpDepositDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    depositMaster.DepositType = depositType;
                    //depositMaster.DepositAmount = Convert.ToDecimal(vdsAmt);
                    depositMaster.ChequeNo = chequeNo;
                    depositMaster.ChequeBank = chequeBank;
                    depositMaster.ChequeBankBranch = chqBBrunch;
                    //depositMaster.ChequeDate = dtpChequeDate.Value.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");//dtpChequeDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    if (string.IsNullOrEmpty(chequeDate))
                    {
                        chequeDate = depositDate;
                    }
                    depositMaster.ChequeDate = Convert.ToDateTime(chequeDate).ToString("yyyy-MM-dd HH:mm:ss");
                    depositMaster.BankId = bankId;
                    depositMaster.TreasuryCopy = "-";
                    depositMaster.DepositPerson = dPerson;
                    depositMaster.DepositPersonDesignation = personDesg;
                    depositMaster.Comments = comment;
                    depositMaster.CreatedBy = createdBy;
                    depositMaster.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    depositMaster.LastModifiedBy = createdBy;
                    depositMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    depositMaster.TransactionType = transactionType;
                    //depositMaster.Info2 = "Info2";
                    //depositMaster.Info3 = "Info3";
                    //depositMaster.Info4 = "Info4";
                    //depositMaster.Info5 = "Info5";
                    depositMaster.Post = post;

                    #endregion Deposit

                    #region VDS

                    DataRow[] VDSRaws; //= new DataRow[];//

                    #region MAtch

                    if (!string.IsNullOrEmpty(importID))
                    {
                        VDSRaws = dtVDS.Select("ID='" + importID + "'");
                    }
                    else
                    {
                        VDSRaws = null;
                    }

                    #endregion MAtch

                    int dCounter = 1;
                    decimal depositAmt = 0;
                    vdsMasterVMs = new List<VDSMasterVM>();
                    foreach (var row in VDSRaws)
                    {

                        //}
                        //for (int vdsRow = 0; vdsRow < dgvVDS.RowCount; vdsRow++)
                        //{
                        string vendorName = row["Vendor_Name"].ToString().Trim();
                        string vendorCode = row["Vendor_Code"].ToString().Trim();
                        #region FindVendorId
                        string vendorId = cImport.FindVendorId(vendorName, vendorCode, currConn, transaction);
                        #endregion FindVendorId
                        string billAmt = row["Bill_Amount"].ToString().Trim();
                        string billDate = row["Bill_Date"].ToString().Trim();
                        string vdsAmt = row["VDS_Amount"].ToString().Trim();
                        string issueDate = row["Issue_Date"].ToString().Trim();
                        string purchaseNo = row["Purchase_No"].ToString().Trim();
                        string remarks = cImport.ChecKNullValue(row["Remarks"].ToString().Trim());

                        VDSMasterVM vdsDetail = new VDSMasterVM();

                        //vdsDetail.Id = NextID.ToString();
                        vdsDetail.VendorId = vendorId;
                        vdsDetail.BillAmount = Convert.ToDecimal(billAmt);
                        vdsDetail.BillDate = Convert.ToDateTime(billDate).ToString("yyyy-MM-dd HH:mm:ss");
                        vdsDetail.BillDeductedAmount = Convert.ToDecimal(vdsAmt);
                        vdsDetail.Remarks = remarks;
                        vdsDetail.IssueDate = Convert.ToDateTime(issueDate).ToString("yyyy-MM-dd HH:mm:ss");
                        if (string.IsNullOrEmpty(purchaseNo))
                        {
                            purchaseNo = "NA";
                        }
                        vdsDetail.PurchaseNumber = purchaseNo;
                        decimal VDSPercentRate = (Convert.ToDecimal(vdsAmt) * 100) / Convert.ToDecimal(billAmt);
                        vdsDetail.VDSPercent = VDSPercentRate;
                        vdsDetail.IsPercent = "N";
                        vdsMasterVMs.Add(vdsDetail);

                        dCounter++;
                        depositAmt = depositAmt + Convert.ToDecimal(vdsAmt);
                    }
                    #endregion
                    depositMaster.DepositAmount = depositAmt;

                    string[] sqlResults = DepositInsert(depositMaster, vdsMasterVMs, adjustmentHistory, transaction, currConn);
                    retResults[0] = sqlResults[0];
                }

                if (retResults[0] == "Success")
                {
                    transaction.Commit();
                    #region SuccessResult

                    retResults[0] = "Success";
                    retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                    retResults[2] = "" + "1";
                    retResults[3] = "" + "N";
                    #endregion SuccessResult
                }
            }

            #endregion try
            #region catch & final
            catch (SqlException sqlex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (ArgumentNullException aeg)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + aeg.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion catch & final

            return retResults;
        }

        public decimal ReverseAmount(string reverseDepositId)
        {
            #region Initializ

            decimal retResults = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region Return Amount

                sqlText = "  ";

                sqlText = " Select Sum(isnull(Deposits.DepositAmount,0)) from Deposits ";
                sqlText += " where ReverseDepositId =@reverseDepositId ";
                sqlText += " and Post = 'Y'";
                sqlText += " group by ReverseDepositId";


                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@reverseDepositId", reverseDepositId);

                if (cmd.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmd.ExecuteScalar();
                }

                #endregion Return Amount

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

        //==================SelectAll=================
        public List<DepositMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<DepositMasterVM> VMs = new List<DepositMasterVM>();
            DepositMasterVM vm;
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


FROM Deposits d
LEFT OUTER JOIN BankInformations b ON d.BankID = b.BankID
WHERE  1=1
";


                if (Id > 0)
                {
                    sqlText += @" and Id=@Id";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        if (conditionFields[i].ToLower().Contains("like"))
                        {
                            sqlText += " AND " + conditionFields[i] + " '%'+ " + " @" + cField.Replace("like", "").Trim() + " +'%'";
                        }
                        else if (conditionFields[i].Contains(">") || conditionFields[i].Contains("<"))
                        {
                            sqlText += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else
                        {
                            sqlText += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        if (conditionFields[j].ToLower().Contains("like"))
                        {
                            objComm.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
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
                    vm = new DepositMasterVM();
                    vm.Id = dr["Id"].ToString();
                    vm.DepositId = dr["DepositId"].ToString();
                    vm.TreasuryNo = dr["TreasuryNo"].ToString();
                    vm.DepositDate = Ordinary.DateTimeToDate(dr["DepositDateTime"].ToString());
                    vm.DepositType = dr["DepositType"].ToString();
                    vm.DepositAmount = Convert.ToDecimal(dr["DepositAmount"].ToString());
                    vm.ChequeNo = dr["ChequeNo"].ToString();
                    vm.ChequeBank = dr["ChequeBank"].ToString();
                    vm.ChequeBankBranch = dr["ChequeBankBranch"].ToString();
                    vm.ChequeDate = Ordinary.DateTimeToDate(dr["ChequeDate"].ToString());
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

    }
}
