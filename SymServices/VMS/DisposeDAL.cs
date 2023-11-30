using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using SymViewModel.VMS;

namespace SymServices.VMS
{
    public class DisposeDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        public string[] DisposeInsert(DisposeMasterVM Master, List<DisposeDetailVM> Details)
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

            string n = "";
            string Prefetch = "";
            int Len = 0;
            int SetupLen = 0;
            int CurrentID = 0;
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
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.DisposeDate) < DateTime.MinValue ||Convert.ToDateTime( Master.DisposeDate)> DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgCheckDate);

                }


                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.disposeMsgMethodNameInsert);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DisposeDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DisposeDate).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(DisposeNumber) from DisposeHeaders " +
                          " where DisposeNumber=@MasterDisposeNumber ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValue("@MasterDisposeNumber", Master.DisposeNumber);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Purchase ID Create
                if (string.IsNullOrEmpty(Master.TransactionType))
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                CommonDAL commonDal = new CommonDAL();


                if (Master.TransactionType == "VAT26")
                {
                    newID = commonDal.TransactionCode("Dispose", "Raw", "DisposeHeaders", "DisposeNumber",
                                              "DisposeDate", Master.DisposeDate, currConn, transaction);
                }
                else if (Master.TransactionType == "VAT27")
                {
                    newID = commonDal.TransactionCode("Dispose", "Finish", "DisposeHeaders", "DisposeNumber",
                                             "DisposeDate", Master.DisposeDate, currConn, transaction);
                }



                #endregion Purchase ID Create Not Complete
                #region ID generated completed,Insert new Information in Header

                sqlText = "";
                sqlText += " insert into DisposeHeaders";
                sqlText += " (";

sqlText += " DisposeNumber,";
sqlText += " DisposeDate,";
sqlText += " RefNumber,";
sqlText += " VATAmount,";
sqlText += " Remarks,";
sqlText += " CreatedBy,";
sqlText += " CreatedOn,";
sqlText += " LastModifiedBy,";
sqlText += " LastModifiedOn,";
sqlText += " TransactionType,";
sqlText += " Post,";
sqlText += " FromStock,"; 
sqlText += " ImportVATAmount,";
sqlText += " TotalPrice,"; 
sqlText += " TotalPriceImport,";
sqlText += " AppVATAmount,"; 
sqlText += " AppTotalPrice,";  
sqlText += " AppDate,";
sqlText += " AppRefNumber,";
sqlText += " AppRemarks,";
sqlText += " AppVATAmountImport,";
                sqlText += " AppTotalPriceImport";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@newID,";
                sqlText += "@MasterDisposeDate,";
                sqlText += "@MasterRefNumber,";
                sqlText += "@MasterVATAmount,";
                sqlText += "@MasterRemarks,";
                sqlText += "@MasterCreatedBy,";
                sqlText += "@MasterCreatedOn,";
                sqlText += "@MasterLastModifiedBy,";
                sqlText += "@MasterLastModifiedOn,";
                sqlText += "@MasterTransactionType,";
                sqlText += "@MasterPost,";
                sqlText += "@MasterFromStock,"; 
                sqlText += "@MasterImportVATAmount,"; 
                sqlText += "@MasterTotalPrice,"; 
                sqlText += "@MasterTotalPriceImport,";
                sqlText += "@MasterAppVATAmount,"; 
                sqlText += "@MasterAppTotalPrice,";
                sqlText += "@MasterAppDate,";
                sqlText += "@MasterAppRefNumber,"; 
                sqlText += "@MasterAppRemarks,";
                sqlText += "@MasterAppVATAmountImport,";
                sqlText += "@MasterAppTotalPriceImport";

                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@newID", newID);
                cmdInsert.Parameters.AddWithValue("@MasterDisposeDate", Master.DisposeDate);
                cmdInsert.Parameters.AddWithValue("@MasterRefNumber", Master.RefNumber);
                cmdInsert.Parameters.AddWithValue("@MasterVATAmount", Master.VATAmount);
                cmdInsert.Parameters.AddWithValue("@MasterRemarks", Master.Remarks ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterFromStock", Master.FromStock);
                cmdInsert.Parameters.AddWithValue("@MasterImportVATAmount", Master.ImportVATAmount);
                cmdInsert.Parameters.AddWithValue("@MasterTotalPrice", Master.TotalPrice);
                cmdInsert.Parameters.AddWithValue("@MasterTotalPriceImport", Master.TotalPriceImport);
                cmdInsert.Parameters.AddWithValue("@MasterAppVATAmount", Master.AppVATAmount);
                cmdInsert.Parameters.AddWithValue("@MasterAppTotalPrice", Master.AppTotalPrice);
                cmdInsert.Parameters.AddWithValue("@MasterAppDate", Master.AppDate);
                cmdInsert.Parameters.AddWithValue("@MasterAppRefNumber", Master.AppRefNumber);
                cmdInsert.Parameters.AddWithValue("@MasterAppRemarks", Master.AppRemarks ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterAppVATAmountImport", Master.AppVATAmountImport);
                cmdInsert.Parameters.AddWithValue("@MasterAppTotalPriceImport", Master.AppTotalPriceImport);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgSaveNotSuccessfully);
                }
                
                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.PurchasemsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(DisposeNumber) from DisposeDetails" +
                               " WHERE DisposeNumber=@newID ";
                    sqlText += " AND ItemNo=@ItemItemNo ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@newID", newID);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable

                    #region USD calculate
                    ReceiveDAL reciveDal=new ReceiveDAL();
                    string[] usdResults =reciveDal.GetUSDCurrency(Item.RealPrice);
                    #endregion USD calculate


                    sqlText = "";
                    sqlText += " insert into DisposeDetails(";
sqlText += " DisposeNumber,";
sqlText += " LineNumber,";
sqlText += " ItemNo,";
sqlText += " Quantity,";
sqlText += " RealPrice,";
sqlText += " VATAmount,";
sqlText += " SaleNumber,";
sqlText += " PurchaseNumber,";
sqlText += " PresentPrice,";
sqlText += " CreatedBy,";
sqlText += " CreatedOn,";
sqlText += " LastModifiedBy,";
sqlText += " LastModifiedOn,";
sqlText += " Post,";
sqlText += " UOM,";
sqlText += " Remarks,";
sqlText += " DisposeDate,";
sqlText += " QuantityImport,";
sqlText += " TransactionType,";
sqlText += " FromStock,";
sqlText += " VATRate,";
sqlText += "DollarPrice";

                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@newID,";
                    sqlText += "@ItemLineNumber,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemQuantity,";
                    sqlText += "@ItemRealPrice,";
                    sqlText += "@ItemVATAmountD,";
                    sqlText += "@ItemSaleNumber,";
                    sqlText += "@ItemPurchaseNumber,";
                    sqlText += "@ItemPresentPrice,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@MasterPost,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemRemarksD,";
                    sqlText += "@MasterDisposeDate,";
                    sqlText += "@ItemQuantityImport,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@MasterFromStock,";
                    sqlText += "@ItemVATRate,";
                    sqlText += "@usdResults1";
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValue("@newID", newID ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemLineNumber", Item.LineNumber ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                    cmdInsDetail.Parameters.AddWithValue("@ItemRealPrice", Item.RealPrice);
                    cmdInsDetail.Parameters.AddWithValue("@ItemVATAmountD", Item.VATAmountD);
                    cmdInsDetail.Parameters.AddWithValue("@ItemSaleNumber", Item.SaleNumber ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseNumber", Item.PurchaseNumber ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPresentPrice", Item.PresentPrice);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemRemarksD", Item.RemarksD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterDisposeDate", Master.DisposeDate);
                    cmdInsDetail.Parameters.AddWithValue("@ItemQuantityImport", Item.QuantityImport ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterFromStock", Master.FromStock ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                    cmdInsDetail.Parameters.AddWithValue("@usdResults1", usdResults[1]);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable


                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.DisposeHeaders " +
                          " where DisposeNumber=@newID ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@newID", newID);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgUnableCreatID);
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
                retResults[1] = MessageVM.disposeMsgSaveSuccessfully;
                retResults[2] = "" + newID;
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
        public string[] DisposeUpdate(DisposeMasterVM Master, List<DisposeDetailVM> Details)
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
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.DisposeDate) < DateTime.MinValue ||Convert.ToDateTime( Master.DisposeDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgCheckDate);

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.disposeMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DisposeDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DisposeDate).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(DisposeNumber) from DisposeHeaders WHERE DisposeNumber=@MasterDisposeNumber ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterDisposeNumber", Master.DisposeNumber);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgUnableFindExistID);
                }

                #endregion Find ID for Update



                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update DisposeHeaders set  ";

                sqlText += " DisposeDate        =@MasterDisposeDate ,";
                sqlText += " RefNumber          =@MasterRefNumber ,";
                sqlText += " VATAmount          =@MasterVATAmount ,";
                sqlText += " Remarks            =@MasterRemarks ,";
                sqlText += " LastModifiedBy     =@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn     =@MasterLastModifiedOn ,";
                sqlText += " TransactionType    =@MasterTransactionType ,";
                sqlText += " Post               =@MasterPost ,";
                sqlText += " FromStock          =@MasterFromStock ,";
                sqlText += " ImportVATAmount    =@MasterImportVATAmount ,";
                sqlText += " TotalPrice         =@MasterTotalPrice ,";
                sqlText += " TotalPriceImport   =@MasterTotalPriceImport ,";
                sqlText += " AppVATAmount       =@MasterAppVATAmount ,";
                sqlText += " AppRefNumber       =@MasterAppRefNumber ,";
                sqlText += " AppTotalPrice      =@MasterAppTotalPrice ,";
                sqlText += " AppDate            =@MasterAppDate ,";
                sqlText += " AppRemarks         =@MasterAppRemarks ,";
                sqlText += " AppTotalPriceImport=@MasterAppTotalPriceImport ,";
                sqlText += " AppVATAmountImport =@MasterAppVATAmountImport ";
                sqlText += " where DisposeNumber=@MasterDisposeNumber ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@MasterDisposeDate", Master.DisposeDate);
                cmdUpdate.Parameters.AddWithValue("@MasterRefNumber", Master.RefNumber ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterVATAmount", Master.VATAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterRemarks", Master.Remarks ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterFromStock", Master.FromStock);
                cmdUpdate.Parameters.AddWithValue("@MasterImportVATAmount", Master.ImportVATAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalPrice", Master.TotalPrice);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalPriceImport", Master.TotalPriceImport);
                cmdUpdate.Parameters.AddWithValue("@MasterAppVATAmount", Master.AppVATAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterAppRefNumber", Master.AppRefNumber ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterAppTotalPrice", Master.AppTotalPrice);
                cmdUpdate.Parameters.AddWithValue("@MasterAppDate", Master.AppDate);
                cmdUpdate.Parameters.AddWithValue("@MasterAppRemarks", Master.AppRemarks ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterAppTotalPriceImport", Master.AppTotalPriceImport);
                cmdUpdate.Parameters.AddWithValue("@MasterAppVATAmountImport", Master.AppVATAmountImport);
                cmdUpdate.Parameters.AddWithValue("@MasterDisposeNumber", Master.DisposeNumber ?? Convert.DBNull);


                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgUpdateNotSuccessfully);
                }
                #endregion update Header

                #region Transaction Not Other


                #endregion Transaction Not Other


                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += " select COUNT(DisposeNumber) from DisposeDetails WHERE DisposeNumber=@MasterDisposeNumber ";
                    sqlText += " AND ItemNo=@ItemItemNo ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@MasterDisposeNumber", Master.DisposeNumber);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    #region USD calculate
                    ReceiveDAL receiveDal=new ReceiveDAL();
                    string[] usdResults = receiveDal.GetUSDCurrency(Item.RealPrice);
                    #endregion USD calculate


                    if (IDExist <= 0)
                    {
                        #region Insert only DetailTable

                        sqlText = "";
                        sqlText += " insert into DisposeDetails(";
                        sqlText += " DisposeNumber,";
                        sqlText += " LineNumber,";
                        sqlText += " ItemNo,";
                        sqlText += " Quantity,";
                        sqlText += " RealPrice,";
                        sqlText += " VATAmount,";
                        sqlText += " SaleNumber,";
                        sqlText += " PurchaseNumber,";
                        sqlText += " PresentPrice,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " Post,";
                        sqlText += " UOM,";
                        sqlText += " Remarks,";
                        sqlText += " DisposeDate,";
                        sqlText += " QuantityImport,";
                        sqlText += " TransactionType,";
                        sqlText += " FromStock,";
                        sqlText += " VATRate,";
                        sqlText += " DollarPrice";

                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@MasterDisposeNumber,";
                        sqlText += "@ItemLineNumber,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@ItemRealPrice,";
                        sqlText += "@ItemVATAmountD,";
                        sqlText += "@ItemSaleNumber,";
                        sqlText += "@ItemPurchaseNumber,";
                        sqlText += "@ItemPresentPrice,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@MasterPost,";
                        sqlText += "@ItemUOM,";
                        sqlText += "@ItemRemarksD,";
                        sqlText += "@MasterDisposeDate,";
                        sqlText += "@ItemQuantityImport,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterFromStock,";
                        sqlText += "@ItemVATRate,";
                        sqlText += "@usdResults1";
                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterDisposeNumber", Master.DisposeNumber ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemLineNumber", Item.LineNumber ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemRealPrice", Item.RealPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATAmountD", Item.VATAmountD);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSaleNumber", Item.SaleNumber ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseNumber", Item.PurchaseNumber ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemPresentPrice", Item.PresentPrice);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemRemarksD", Item.RemarksD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterDisposeDate", Master.DisposeDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantityImport", Item.QuantityImport ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterFromStock", Master.FromStock ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValue("@usdResults1", usdResults[1] ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.disposeMsgMethodNameInsert, MessageVM.disposeMsgSaveNotSuccessfully);
                        }
                        #endregion Insert only DetailTable
                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";

                        sqlText += " update DisposeDetails set ";
                        sqlText += " LineNumber     =@ItemLineNumber,";
                        sqlText += " Quantity       =@ItemQuantity,";
                        sqlText += " RealPrice      =@ItemRealPrice,";
                        sqlText += " VATAmount      =@ItemVATAmountD,";
                        sqlText += " SaleNumber     =@ItemSaleNumber,";
                        sqlText += " PurchaseNumber =@ItemPurchaseNumber,";
                        sqlText += " PresentPrice   =@ItemPresentPrice,";
                        sqlText += " LastModifiedBy =@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                        sqlText += " DisposeDate    =@MasterDisposeDate,";
                        sqlText += " Post           =@MasterPost,";
                        sqlText += " UOM            =@ItemUOM,";
                        sqlText += " VATRate        =@ItemVATRate,";
                        sqlText += " Remarks        =@ItemRemarksD,";
                        sqlText += " TransactionType=@MasterTransactionType,";
                        sqlText += " FromStock      =@MasterFromStock,";
                        sqlText += " QuantityImport =@ItemQuantityImport,";
                        sqlText += "DollarPrice     =@usdResults1";
                    sqlText += " where DisposeNumber=@MasterDisposeNumber";
                        sqlText += " and ItemNo     =@ItemItemNo";



                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@ItemLineNumber", Item.LineNumber ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemRealPrice", Item.RealPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATAmountD", Item.VATAmountD);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSaleNumber", Item.SaleNumber);
                        cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseNumber", Item.PurchaseNumber ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemPresentPrice", Item.PresentPrice);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@MasterDisposeDate", Master.DisposeDate);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemRemarksD", Item.RemarksD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterFromStock", Master.FromStock ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantityImport", Item.QuantityImport ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@usdResults1", usdResults[1] ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterDisposeNumber", Master.DisposeNumber ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);


                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable
                    }

                    #endregion Find Transaction Mode Insert or Update
                }


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from DisposeHeaders WHERE DisposeNumber=@MasterDisposeNumber ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterDisposeNumber", Master.DisposeNumber);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNameUpdate, MessageVM.disposeMsgUnableCreatID);
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
                retResults[1] = MessageVM.disposeMsgUpdateSuccessfully;
                retResults[2] = Master.DisposeNumber;
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
        public string[] DisposePost(DisposeMasterVM Master, List<DisposeDetailVM> Details)
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
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.DisposeDate) < DateTime.MinValue || Convert.ToDateTime(Master.DisposeDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgCheckDatePost);

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.disposeMsgMethodNamePost);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.DisposeDate;
                string transactionYearCheck = Convert.ToDateTime(Master.DisposeDate).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(DisposeNumber) from DisposeHeaders WHERE DisposeNumber=@MasterDisposeNumber ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterDisposeNumber", Master.DisposeNumber);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update



                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update DisposeHeaders set  ";

                sqlText += " LastModifiedBy=@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn=@MasterLastModifiedOn ,";
                sqlText += " Post=@MasterPost ";
                sqlText += " where DisposeNumber=@MasterDisposeNumber ";

                
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterDisposeNumber", Master.DisposeNumber ?? Convert.DBNull);


                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgUpdateNotSuccessfully);
                }
                #endregion update Header

                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgNoDataToPost);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += " select COUNT(DisposeNumber) from DisposeDetails WHERE DisposeNumber=@MasterDisposeNumber ";
                    sqlText += " AND ItemNo=@ItemItemNo ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@MasterDisposeNumber", Master.DisposeNumber);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgNoDataToPost);
                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";

                        sqlText += " update DisposeDetails set ";
                        sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                        sqlText += " Post=@MasterPost";
                        sqlText += " where DisposeNumber=@MasterDisposeNumber";

                        sqlText += " and ItemNo =@ItemItemNo";



                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterDisposeNumber", Master.DisposeNumber ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);


                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgPostNotSuccessfully);
                        }
                        #endregion Update only DetailTable
                    }

                    #endregion Find Transaction Mode Insert or Update
                }


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from DisposeHeaders WHERE DisposeNumber=@MasterDisposeNumber";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterDisposeNumber", Master.DisposeNumber);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.disposeMsgMethodNamePost, MessageVM.disposeMsgUnableCreatID);
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
                retResults[1] = MessageVM.disposeMsgSuccessfullyPost;
                retResults[2] = Master.DisposeNumber;
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
        
        
        
        #region // == Search == //
        public DataTable SearchDisposeHeaderDTNew(string DisposeNumber, string DisposeDateFrom, string DisposeDateTo, string transactionType, string Post, string databasename)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("DisposeSearch");

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
                            DisposeNumber, convert (varchar,DisposeDate,120)DisposeDate,
                            isnull(RefNumber,'NA')RefNumber,
                            isnull(Remarks,'NA')Remarks,
                            isnull(TransactionType,'NA')TransactionType,
                            isnull(Post,'N')Post,
                            isnull(AppTotalPrice,'0')AppTotalPrice,
                            isnull(AppVATAmount,'0')AppVATAmount,
                            isnull(convert (varchar,AppDate,120),DisposeDate)AppDate,
                            isnull(AppRefNumber,'NA')AppRefNumber,
                            isnull(AppRemarks,'NA')AppRemarks,
                            isnull(AppVATAmountImport,'0')AppVATAmountImport,
                            isnull(AppTotalPriceImport,'0')AppTotalPriceImport

                            FROM         dbo.DisposeHeaders
                            WHERE 
                            (DisposeNumber  LIKE '%' +  @DisposeNumber   + '%' OR @DisposeNumber IS NULL) 
                            AND (DisposeDate>= @DisposeDateFrom OR @DisposeDateFrom IS NULL)
                            AND (DisposeDate <dateadd(d,1, @DisposeDateTo) OR @DisposeDateTo IS NULL)
                            AND (Post LIKE '%' + @Post + '%' OR @Post IS NULL)
                            AND (TransactionType=@TransactionType)";
                #endregion

                #region SQL Command

                SqlCommand objCommDisposeHeader = new SqlCommand();
                objCommDisposeHeader.Connection = currConn;

                objCommDisposeHeader.CommandText = sqlText;
                objCommDisposeHeader.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommDisposeHeader.Parameters.Contains("@Post"))
                { objCommDisposeHeader.Parameters.AddWithValue("@Post", Post); }
                else { objCommDisposeHeader.Parameters["@Post"].Value = Post; }

                if (!objCommDisposeHeader.Parameters.Contains("@DisposeNumber"))
                { objCommDisposeHeader.Parameters.AddWithValue("@DisposeNumber", DisposeNumber); }
                else { objCommDisposeHeader.Parameters["@DisposeNumber"].Value = DisposeNumber; }
                if (DisposeDateFrom == "")
                {
                    if (!objCommDisposeHeader.Parameters.Contains("@DisposeDateFrom"))
                    { objCommDisposeHeader.Parameters.AddWithValue("@DisposeDateFrom", System.DBNull.Value); }
                    else { objCommDisposeHeader.Parameters["@DisposeDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommDisposeHeader.Parameters.Contains("@DisposeDateFrom"))
                    { objCommDisposeHeader.Parameters.AddWithValue("@DisposeDateFrom", DisposeDateFrom); }
                    else { objCommDisposeHeader.Parameters["@DisposeDateFrom"].Value = DisposeDateFrom; }
                }
                if (DisposeDateTo == "")
                {
                    if (!objCommDisposeHeader.Parameters.Contains("@DisposeDateTo"))
                    { objCommDisposeHeader.Parameters.AddWithValue("@DisposeDateTo", System.DBNull.Value); }
                    else { objCommDisposeHeader.Parameters["@DisposeDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommDisposeHeader.Parameters.Contains("@DisposeDateTo"))
                    { objCommDisposeHeader.Parameters.AddWithValue("@DisposeDateTo", DisposeDateTo); }
                    else { objCommDisposeHeader.Parameters["@DisposeDateTo"].Value = DisposeDateTo; }
                }


                if (!objCommDisposeHeader.Parameters.Contains("@transactionType"))
                { objCommDisposeHeader.Parameters.AddWithValue("@transactionType", transactionType); }
                else { objCommDisposeHeader.Parameters["@transactionType"].Value = transactionType; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommDisposeHeader);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
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

            #endregion

            return dataTable;
        }
        public DataTable SearchDisposeDetailDTNew(string DisposeNumber)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("DisposeSearch");

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
                            select 
LineNumber,
DD.ItemNo ItemNo,
p.ProductName ItemName,
p.ProductCode PCode,
dd.UOM UOM,
isnull(Quantity,0)Quantity,
isnull(QuantityImport,Quantity)QuantityImport,
isnull(RealPrice,0)RealPrice,
isnull(VATAmount,0)VATAmt,
SaleNumber SaleNumber,
PurchaseNumber PurchaseNumber,
isnull(PresentPrice,0)PresentPrice,
isnull(Post,'N')Post,
remarks Comments,
dd.VATRate,0 as Stock 
from DisposeDetails  DD left outer join
products P on DD.ItemNo= p.itemno
     
WHERE 
(DisposeNumber = @DisposeNumber ) ";
                #endregion

                #region SQL Command

                SqlCommand objCommDisposeHeader = new SqlCommand();
                objCommDisposeHeader.Connection = currConn;

                objCommDisposeHeader.CommandText = sqlText;
                objCommDisposeHeader.CommandType = CommandType.Text;
                
                #endregion

                #region Parameter

                if (!objCommDisposeHeader.Parameters.Contains("@DisposeNumber"))
                { objCommDisposeHeader.Parameters.AddWithValue("@DisposeNumber", DisposeNumber); }
                else { objCommDisposeHeader.Parameters["@DisposeNumber"].Value = DisposeNumber; }
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommDisposeHeader);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
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

            #endregion

            return dataTable;
        }
        
        #endregion
       
    }
}
