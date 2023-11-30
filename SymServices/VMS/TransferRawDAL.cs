using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SymViewModel.VMS;
using System.Data;
using SymOrdinary;

namespace SymServices.VMS
{
    public class TransferRawDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion

        public string[] TransferRawInsert(TransferRawMasterVM Master, List<TransferRawDetailVM> Details)
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
            int MasterId;


            #endregion Initializ
            #region Try
            try
            {
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.TransferDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransferDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "Please Check Transfer Data and Time");

                }


                #endregion Validation for Header
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);
                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.TransferDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.TransferDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(TransferId) from TransferRawHeaders WHERE TransferId=@MasterTransferId ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterTransferId", Master.TransferId);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                }

                #endregion Find Transaction Exist
                #region VAT7 ID Create
                CommonDAL commonDal = new CommonDAL();
                newID = commonDal.TransactionCode("TransferRaw", "TransferRaw", "TransferRawHeaders", "TransferId", "TransferDateTime", Master.TransferDateTime, currConn, transaction);
                #endregion VAT7 ID Create
                #region ID generated completed,Insert new Information in Header

                sqlText = "";
                sqlText += " insert into TransferRawHeaders(";
                sqlText += " TransferId ,";
                sqlText += " TransferDateTime ,";
                sqlText += " TransFromItemNo ,";
                sqlText += " CostPrice ,";
                sqlText += " Quantity ,";
                sqlText += " TransferedQty ,";
                sqlText += " TransferedAmt ,";
                sqlText += " UOM ,";
                sqlText += " TransactionType ,";
                sqlText += " CreatedBy ,";
                sqlText += " CreatedOn ,";
                sqlText += " LastModifiedBy ,";
                sqlText += " LastModifiedOn ,";

                sqlText += " Post ";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@newID,";
                sqlText += "@MasterTransferDateTime,";
                sqlText += "@MasterTransFromItemNo,";
                sqlText += "@MasterCostPrice,";
                sqlText += "@MasterQuantity,";
                sqlText += "@MasterTransferedQty,";
                sqlText += "@MasterTransferedAmt,";
                sqlText += "@MasterUOM,";
                sqlText += "@MasterTransactionType,";
                sqlText += "@MasterCreatedBy,";
                sqlText += "@MasterCreatedOn,";
                sqlText += "@MasterLastModifiedBy,";
                sqlText += "@MasterLastModifiedOn,";
                sqlText += "@MasterPost";
                sqlText += "); SELECT SCOPE_IDENTITY()";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@newID", newID);
                cmdInsert.Parameters.AddWithValue("@MasterTransferDateTime", Master.TransferDateTime);
                cmdInsert.Parameters.AddWithValue("@MasterTransFromItemNo", Master.TransFromItemNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCostPrice", Master.CostPrice);
                cmdInsert.Parameters.AddWithValue("@MasterQuantity", Master.Quantity);
                cmdInsert.Parameters.AddWithValue("@MasterTransferedQty", Master.TransferedQty);
                cmdInsert.Parameters.AddWithValue("@MasterTransferedAmt", Master.TransferedAmt);
                cmdInsert.Parameters.AddWithValue("@MasterUOM", Master.UOM ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@MasterPost", Master.Post ?? "N");

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                MasterId = Convert.ToInt32(transResult);
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                }


                #endregion ID generated completed,Insert new Information in Header
                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist
                    sqlText = "";
                    sqlText += "select COUNT(TransferId) from TransferRawDetails WHERE TransferId=@newID ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@newID", newID);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into TransferRawDetails(";
                    sqlText += " TransferId ,";
                    sqlText += " TransLineNo ,";
                    sqlText += " ItemNo ,";
                    sqlText += " Quantity ,";
                    sqlText += " CostPrice ,";
                    sqlText += " UOM ,";
                    sqlText += " SubTotal ,";
                    sqlText += " UOMQty ,";
                    sqlText += " UOMPrice ,";
                    sqlText += " UOMc ,";
                    sqlText += " UOMn,";

                    sqlText += " TransactionType,";
                    sqlText += " TransferDateTime ,";
                    sqlText += " TransFromItemNo ,";
                    sqlText += " CreatedBy ,";
                    sqlText += " CreatedOn ,";
                    sqlText += " LastModifiedBy ,";
                    sqlText += " LastModifiedOn ,";

                    sqlText += " Post ";
                    sqlText += " )";

                    sqlText += " values";
                    sqlText += " (";
                    sqlText += "@newID,";
                    sqlText += "@ItemTransLineNo,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemQuantity,";
                    sqlText += "@ItemCostPrice,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemSubTotal,";
                    sqlText += "@ItemUOMQty,";
                    sqlText += "@ItemUOMPrice,";
                    sqlText += "@ItemUOMc,";
                    sqlText += "@ItemUOMn,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@MasterTransferDateTime,";
                    sqlText += "@MasterTransFromItemNo,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@MasterPost";
                    sqlText += ")";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValue("@newID", newID);
                    cmdInsDetail.Parameters.AddWithValue("@ItemTransLineNo", Item.TransLineNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                    cmdInsDetail.Parameters.AddWithValue("@ItemCostPrice", Item.CostPrice);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterTransferDateTime", Master.TransferDateTime);
                    cmdInsDetail.Parameters.AddWithValue("@MasterTransFromItemNo", Master.TransFromItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy ", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ??"N");

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable


                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)
                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.TransferRawHeaders WHERE TransferId =@newID";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@newID", newID);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgUnableCreatID);
                }


                #endregion eturn Current ID and Post Status
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
                retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                retResults[2] = "" + MasterId;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult
            }
            #endregion Try
            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
        public string[] TransferRawUpdate(TransferRawMasterVM Master, List<TransferRawDetailVM> Details)
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
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.TransferDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransferDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, "Please Check Invoice Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #region Add BOMId
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("IssueDetails", "BOMId", "varchar(20)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMQty", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMPrice", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMc", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMn", "varchar(50)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMWastage", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                #endregion Add BOMId
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.TransferDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.TransferDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(TransferId) from TransferRawHeaders WHERE TransferId=@MasterTransferId ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterTransferId", Master.TransferId);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
                }

                #endregion Find ID for Update



                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update TransferRawHeaders set  ";

                sqlText += " TransferDateTime   =@MasterTransferDateTime ,";
                sqlText += " TransferedAmt      =@MasterTransferedAmt ,";
                sqlText += " TransferedQty      =@MasterTransferedQty ,";
                sqlText += " LastModifiedBy     =@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn     =@MasterLastModifiedOn ,";
                sqlText += " transactionType    =@MasterTransactionType ,";
                sqlText += " Post               =@MasterPost ";
                sqlText += " where  TransferId  =@MasterTransferId ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@MasterTransferDateTime", Master.TransferDateTime);
                cmdUpdate.Parameters.AddWithValue("@MasterTransferedAmt", Master.TransferedAmt);
                cmdUpdate.Parameters.AddWithValue("@MasterTransferedQty", Master.TransferedQty);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterTransactionType ", Master.TransactionType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post ??"N");
                cmdUpdate.Parameters.AddWithValue("@MasterTransferId", Master.TransferId ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header



                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(TransferId) from TransferRawDetails WHERE TransferId=@MasterTransferId ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@MasterTransferId", Master.TransferId);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        // Insert
                        #region Insert only DetailTable

                        sqlText = "";
                        sqlText += " insert into TransferRawDetails(";
                        sqlText += " TransferId ,";
                        sqlText += " TransLineNo ,";
                        sqlText += " ItemNo ,";
                        sqlText += " Quantity ,";
                        sqlText += " CostPrice ,";
                        sqlText += " UOM ,";
                        sqlText += " SubTotal ,";
                        sqlText += " UOMQty ,";
                        sqlText += " UOMPrice ,";
                        sqlText += " UOMc ,";
                        sqlText += " UOMn,";

                        sqlText += " TransactionType,";
                        sqlText += " TransferDateTime ,";
                        sqlText += " TransFromItemNo ,";
                        sqlText += " CreatedBy ,";
                        sqlText += " CreatedOn ,";
                        sqlText += " LastModifiedBy ,";
                        sqlText += " LastModifiedOn ,";

                        sqlText += " Post ";
                        sqlText += " )";

                        sqlText += " values";
                        sqlText += " (";
                        sqlText += "@MasterTransferId,";
                        sqlText += "@ItemTransLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@ItemCostPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += "@ItemSubTotal,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterTransferDateTime,";
                        sqlText += "@ItemTransFromItemNo,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@MasterPost";
                        sqlText += ")";



                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransferId", Master.TransferId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemTransLineNo", Item.TransLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCostPrice", Item.CostPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransferDateTime", Master.TransferDateTime);
                        cmdInsDetail.Parameters.AddWithValue("@ItemTransFromItemNo", Item.TransFromItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ??"N");

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        }
                        #endregion Insert only DetailTable
                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";
                        sqlText += " update TransferRawDetails set ";


                        sqlText += " TransferId      =@ItemTransferIdD,";
                        sqlText += " Quantity        =@ItemQuantity,";
                        sqlText += " CostPrice       =@ItemCostPrice,";
                        sqlText += " UOM             =@ItemUOM,";
                        sqlText += " SubTotal        =@ItemSubTotal,";
                        sqlText += " UOMQty          =@ItemUOMQty,";
                        sqlText += " UOMPrice        =@ItemUOMPrice,";
                        sqlText += " UOMc            =@ItemUOMc,";
                        sqlText += " UOMn            =@ItemUOMn,";
                        sqlText += " LastModifiedBy  =@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn  =@MasterLastModifiedOn,";
                        sqlText += " TransferDateTime=@MasterTransferDateTime,";
                        sqlText += " transactionType =@MasterTransactionType,";
                        sqlText += " Post            =@MasterPost";
                    sqlText += " where  TransferId   =@MasterTransferId ";
                        sqlText += " and ItemNo      =@ItemItemNo";
                        if (!string.IsNullOrEmpty(Item.TransFromItemNo))
                        {
                            if (Item.TransFromItemNo != "N/A" && Item.TransFromItemNo != "0")
                            {
                                sqlText += " and TransFromItemNo=@Item.TransFromItemNo ";
                            }
                        }


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@ItemTransferIdD", Master.TransferId);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCostPrice", Item.CostPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransferDateTime", Master.TransferDateTime);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ??"N");
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransferId", Master.TransferId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemTransFromItemNo", Item.TransFromItemNo ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable

                    }

                    #endregion Find Transaction Mode Insert or Update
                }//foreach (var Item in Details.ToList())

                #region Remove row
                sqlText = "";
                sqlText += " SELECT  distinct ItemNo";
                sqlText += " from TransferRawDetails WHERE TransferId='" + Master.TransferId + "'";

                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                cmdRIFB.Transaction = transaction;
                SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB);
                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var p = pItem["ItemNo"].ToString();

                    //var tt= Details.Find(x => x.ItemNo == p);
                    var tt = Details.Count(x => x.ItemNo.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM TransferRawDetails ";
                        sqlText += " WHERE TransferId=@MasterTransferId ";
                        sqlText += " AND ItemNo=@p ";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransferId", Master.TransferId);
                        cmdInsDetail.Parameters.AddWithValue("@p", p);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                    }

                }
                #endregion Remove row


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from TransferRawHeaders WHERE TransferId=@MasterTransferId";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterTransferId", Master.TransferId);
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableCreatID);
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
                retResults[1] = MessageVM.issueMsgUpdateSuccessfully;
                retResults[2] = Master.Id;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string[] TransferRawPost(TransferRawMasterVM Master, List<TransferRawDetailVM> Details)
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
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.TransferDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransferDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgCheckDatePost);

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNamePost);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.TransferDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.TransferDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(TransferId) from TransferRawHeaders WHERE TransferId=@MasterTransferId ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterTransferId", Master.TransferId);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update TransferRawHeaders set  ";
                sqlText += " LastModifiedBy     =@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn     =@MasterLastModifiedOn ,";
                sqlText += " Post               =@MasterPost ";
                sqlText += " where  TransferId  =@MasterTransferId ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterTransferId", Master.TransferId ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSuccessfully);
                }
                #endregion update Header



                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(TransferId) from TransferRawDetails WHERE TransferId=@MasterTransferId ";
                    sqlText += " AND ItemNo=@ItemItemNo ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@MasterTransferId", Master.TransferId);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                    }
                    else
                    {
                        #region Update only DetailTable

                        sqlText = "";
                        sqlText += " update TransferRawDetails set ";
                        sqlText += " Post=@MasterPost";
                        sqlText += " where  TransferId =@MasterTransferId ";
                        sqlText += " and ItemNo=@ItemItemNo";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransferId", Master.TransferId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);


                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSuccessfully);
                        }
                        #endregion Update only DetailTable
                    }
                    #endregion Find Transaction Mode Insert or Update
                }

                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from TransferRawHeaders WHERE TransferId=@MasterTransferId ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterTransferId", Master.TransferId);
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSelect);
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
                retResults[1] = MessageVM.issueMsgSuccessfullyPost;
                retResults[2] = Master.TransferId;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public DataTable SearchTransferRawHeaderDTNew(string TransferId, string TransferRawDateFrom,
            string TransferRawDateTo,string Post)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("TransferRawSearchHeader");

            #endregion

            #region Try
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

                sqlText = " ";
                sqlText = @" SELECT  
                            TransferId,
                            convert (varchar,TransferDateTime,120)TransferDateTime,
                            isnull(TransferedQty,0)TransferedQty,
                            isnull(TransferedAmt,0)TransferedAmt ,
                            isnull(trh.CostPrice,0)CostPrice,
                            isnull(trh.Quantity,0)Quantity,
							isnull(trh.UOM,'N/A')UOM,
							trh.Post,trh.TransactionType,
                            isnull(trh.TransFromItemNo,'N/A')TransFromItemNo,
							isnull(fp.ProductCode,'N/A')TransFromCode,
							isnull(fp.ProductName,'N/A')TransFromName

							FROM         dbo.TransferRawHeaders trh LEFT OUTER JOIN
                            Products fp on trh.TransFromItemNo=fp.ItemNo
							
							WHERE

                            (TransferId  LIKE '%' +  @TransferId   + '%' OR @TransferId IS NULL) 
                            AND (TransferDateTime>= @TransferDateFrom OR @TransferDateFrom IS NULL)
                            AND (TransferDateTime <dateadd(d,1, @TransferDateTo) OR @TransferDateTo IS NULL)
                            and (Post  LIKE '%' +  @Post   + '%' OR @Post IS NULL) 
                            ";
                
                #endregion

                #region SQL Command

                SqlCommand objCommIssueHeader = new SqlCommand();
                objCommIssueHeader.Connection = currConn;

                objCommIssueHeader.CommandText = sqlText;
                objCommIssueHeader.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommIssueHeader.Parameters.Contains("@Post"))
                { objCommIssueHeader.Parameters.AddWithValue("@Post", Post); }
                else { objCommIssueHeader.Parameters["@Post"].Value = Post; }

                if (!objCommIssueHeader.Parameters.Contains("@TransferId"))
                { objCommIssueHeader.Parameters.AddWithValue("@TransferId", TransferId); }
                else { objCommIssueHeader.Parameters["@TransferId"].Value = TransferId; }

                if (TransferRawDateFrom == "")
                {
                    if (!objCommIssueHeader.Parameters.Contains("@TransferDateFrom"))
                    { objCommIssueHeader.Parameters.AddWithValue("@TransferDateFrom", System.DBNull.Value); }
                    else { objCommIssueHeader.Parameters["@TransferDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommIssueHeader.Parameters.Contains("@TransferDateFrom"))
                    { objCommIssueHeader.Parameters.AddWithValue("@TransferDateFrom", TransferRawDateFrom); }
                    else { objCommIssueHeader.Parameters["@TransferDateFrom"].Value = TransferRawDateFrom; }
                }
                if (TransferRawDateTo == "")
                {
                    if (!objCommIssueHeader.Parameters.Contains("@TransferDateTo"))
                    { objCommIssueHeader.Parameters.AddWithValue("@TransferDateTo", System.DBNull.Value); }
                    else { objCommIssueHeader.Parameters["@TransferDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommIssueHeader.Parameters.Contains("@TransferDateTo"))
                    { objCommIssueHeader.Parameters.AddWithValue("@TransferDateTo", TransferRawDateTo); }
                    else { objCommIssueHeader.Parameters["@TransferDateTo"].Value = TransferRawDateTo; }
                }

               
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommIssueHeader);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return dataTable;
        }
        public DataTable SearchTransferRawDetailDTNew(string TransferId)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("IssueSearchDetail");

            #endregion

            #region Try
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
                          							SELECT  
trd.TransferId, 
trd.TransLineNo,
trd.ItemNo, 
isnull(trd.Quantity,0)Quantity ,
isnull(trd.CostPrice,0)CostPrice,
isnull(trd.UOM,'N/A')UOM ,
isnull(trd.SubTotal,0)SubTotal,
isnull(trd.UOMQty,isnull(trd.Quantity,0))UOMQty,
isnull(trd.UOMn,trd.UOM)UOMn,
isnull(trd.UOMc,1)UOMc,
isnull(trd.UOMPrice,isnull(trd.CostPrice,0))UOMPrice,
isnull(Products.ProductName,'N/A')ProductName,
isnull(Products.ProductCode,'N/A')ProductCode,

isnull(trd.TransFromItemNo,'0')TransFromItemNo,
isnull(fp.ProductCode,'N/A')TransFromCode,
isnull(fp.ProductName,'N/A')TransFromName

                            FROM         dbo.TransferRawDetails trd  left outer join
                            Products on trd.ItemNo=Products.ItemNo LEFT OUTER JOIN
                            Products fp on trd.TransFromItemNo=fp.ItemNo 
                            
                               
                            WHERE 
                            (TransferId = @TransferId ) 
                            ";

                #endregion

                #region SQL Command

                SqlCommand objCommIssueDetail = new SqlCommand();
                objCommIssueDetail.Connection = currConn;

                objCommIssueDetail.CommandText = sqlText;
                objCommIssueDetail.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommIssueDetail.Parameters.Contains("@TransferId"))
                { objCommIssueDetail.Parameters.AddWithValue("@TransferId", TransferId); }
                else { objCommIssueDetail.Parameters["@TransferId"].Value = TransferId; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommIssueDetail);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return dataTable;
        }

        //==================SelectAll=================
        public List<TransferRawMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<TransferRawMasterVM> VMs = new List<TransferRawMasterVM>();
            TransferRawMasterVM vm;
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
 tr.Id
,tr.TransferId
,tr.TransferDateTime
,tr.TransFromItemNo
,tr.UOM
,tr.CostPrice
,isnull(tr.Quantity,0) Quantity
,isnull(tr.TransferedQty,0) TransferedQty
,isnull(tr.TransferedAmt,0) TransferedAmt
,tr.TransactionType
,tr.Post
,tr.CreatedBy
,tr.CreatedOn
,tr.LastModifiedBy
,tr.LastModifiedOn
,p.ProductCode
,p.ProductName
,p.CategoryID

FROM TransferRawHeaders tr
left outer join Products p on tr.TransFromItemNo=p.ItemNo
WHERE  1=1

";
                if (Id > 0)
                {
                    sqlText += @" and tr.Id=@Id";
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
                    vm = new TransferRawMasterVM();
                    vm.Id = dr["Id"].ToString();
                    vm.TransferId = dr["TransferId"].ToString();
                    vm.TransferDateTime = dr["TransferDateTime"].ToString();
                    vm.TransFromItemNo = dr["TransFromItemNo"].ToString();
                    vm.UOM = dr["UOM"].ToString();
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.TransferedQty = Convert.ToDecimal(dr["TransferedQty"].ToString());
                    vm.TransferedAmt = Convert.ToDecimal(dr["TransferedAmt"].ToString());
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.RawCode = dr["ProductCode"].ToString();
                    vm.RawName = dr["ProductName"].ToString();
                    vm.CategoryId = dr["CategoryID"].ToString();

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

        //==================SelectAll=================
        public List<TransferRawDetailVM> SelectTransferDetail(string transferId, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<TransferRawDetailVM> VMs = new List<TransferRawDetailVM>();
            TransferRawDetailVM vm;
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
 tr.TransferId
,tr.TransLineNo
,tr.ItemNo
,isnull(tr.Quantity,0) Quantity
,isnull(tr.CostPrice,0) CostPrice
,tr.UOM
,isnull(tr.SubTotal,0) SubTotal
,isnull(tr.UOMQty,0) UOMQty
,isnull(tr.UOMPrice,0) UOMPrice
,isnull(tr.UOMc,0) UOMc
,tr.UOMn
,tr.TransactionType
,tr.Post
,tr.TransFromItemNo
,tr.TransferDateTime
,tr.CreatedBy
,tr.CreatedOn
,tr.LastModifiedBy
,tr.LastModifiedOn
,p.ProductName
,p.ProductCode
,p.CategoryID

FROM TransferRawDetails tr
left outer join Products p on tr.ItemNo=p.ItemNo
WHERE  1=1

";

                if (transferId != null)
                {
                    sqlText += "AND tr.TransferId=@TransferId";
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

                if (transferId != null)
                {
                    objComm.Parameters.AddWithValue("@TransferId", transferId);
                }
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
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new TransferRawDetailVM();
                    vm.TransferIdD = dr["TransferId"].ToString();
                    vm.TransLineNo = dr["TransLineNo"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                    vm.UOMQty = Convert.ToDecimal(dr["UOMQty"].ToString());
                    vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.TransFromItemNo = dr["TransFromItemNo"].ToString();
                    vm.ItemName = dr["ProductName"].ToString();
                    vm.ItemCode = dr["ProductCode"].ToString();
                    vm.groupId = dr["CategoryID"].ToString();

                    //vm.TransferDateTime = dr["TransferDateTime"].ToString();
                    //vm.CreatedBy = dr["CreatedBy"].ToString();
                    //vm.CreatedOn = dr["CreatedOn"].ToString();
                    //vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    //vm.LastModifiedOn = dr["LastModifiedOn"].ToString();

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
