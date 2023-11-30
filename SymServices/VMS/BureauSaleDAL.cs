using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using SymViewModel.VMS;

// TransactionType no probs
namespace SymServices.VMS
{
    public class BureauSaleDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DataTable dtCredit = null;

        #endregion


        public DataTable SearchSaleExportDTNew(string SalesInvoiceNo, string databaseName)
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("SearchExport");

            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("SalesInvoiceDetails", "DiscountAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("SalesInvoiceDetails", "DiscountedNBRPrice", "decimal(25, 9)", currConn);

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT SaleLineNo,
                                Description,
                                Quantity,
                                GrossWeight, NetWeight,
                                NumberFrom, NumberTo,
                                Comments
                                FROM SalesInvoiceHeadersExport
                                WHERE (SalesInvoiceNo = @SalesInvoiceNo)";

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;
                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                if (!objCommSaleDetail.Parameters.Contains("@SalesInvoiceNo"))
                { objCommSaleDetail.Parameters.AddWithValue("@SalesInvoiceNo", SalesInvoiceNo); }
                else { objCommSaleDetail.Parameters["@SalesInvoiceNo"].Value = SalesInvoiceNo; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
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
        public string[] UpdatePrintNew(string InvoiceNo, int PrintCopy)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "0";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(InvoiceNo))
                {
                    throw new ArgumentNullException("UpdatePrintNew", "Please select one invoice no.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                transaction = currConn.BeginTransaction("UpdatePrintNew");

                #endregion open connection and transaction

                sqlText = @"SELECT isnull(AlReadyPrint,0)AlReadyPrint FROM SalesInvoiceHeaders WHERE SalesInvoiceNo =@InvoiceNo";

                DataTable dataTable = new DataTable("SalesInvoiceHeaders");
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                cmdIdExist.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);

                transResult = (int)cmdIdExist.ExecuteScalar();

                if (transResult <= 0)
                {
                    //PrintCopy = 0;
                }
                else
                {
                    PrintCopy = transResult + PrintCopy;
                }

                #region Update Print

                sqlText = @"update SalesInvoiceHeaders set IsPrint='Y',AlReadyPrint=@PrintCopy where SalesInvoiceNo=@InvoiceNo";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);

                cmdUpdate.CommandText = sqlText;
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@PrintCopy", PrintCopy);
                cmdUpdate.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Sales Invoice Print Successfully Update.";
                        retResults[2] = "" + InvoiceNo;
                        retResults[3] = "" + PrintCopy.ToString();


                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update vehicles.";
                        retResults[2] = "" + InvoiceNo;
                        retResults[3] = "" + PrintCopy.ToString();

                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update print.";
                    retResults[2] = "" + InvoiceNo;
                    retResults[3] = "" + PrintCopy.ToString();

                }

                #endregion Commit

                #endregion Update Print

            }
            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
        public string[] SalesInsert(SaleMasterVM Master, List<BureauSaleDetailVM> Details, SqlTransaction transaction, SqlConnection currConn)
            
        {
            var tt = "";
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

           
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
                CommonDAL commonDal = new CommonDAL();
                
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.InvoiceDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.InvoiceDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, "Please Check Invoice Data and Time");

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

                    transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameInsert);
                }


                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.InvoiceDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.InvoiceDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(SalesInvoiceNo) from SalesInvoiceHeaders WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgFindExistID);
                }

                #endregion Find Transaction Exist



                #region Sale ID Create
                if (string.IsNullOrEmpty(Master.TransactionType)) // start
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }

                if (Master.TransactionType == "Other")
                {
                    newID = commonDal.TransactionCode("Sale", "Other", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                              "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                else if (Master.TransactionType == "Credit")
                {
                    newID = commonDal.TransactionCode("Sale", "Credit", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                else if (Master.TransactionType == "ServiceNS")
                {
                    newID = commonDal.TransactionCode("Sale", "ServiceNS", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                else if (Master.TransactionType == "ExportServiceNS")
                {
                    newID = commonDal.TransactionCode("Sale", "ExportServiceNS", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                else if (Master.TransactionType == "ExportServiceNSCredit")
                {
                    newID = commonDal.TransactionCode("Sale", "ExportServiceNSCredit", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                if (string.IsNullOrEmpty(newID) || newID == "")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                            "ID Prefetch not set please update Prefetch first");
                }



                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header

                var vTotalAmount = commonDal.decimal259(Master.TotalAmount);

                sqlText = "";
                sqlText += " insert into SalesInvoiceHeaders";
                sqlText += " (";
                sqlText += " SalesInvoiceNo,";
                sqlText += " CustomerID,";
                sqlText += " InvoiceDateTime,";
                sqlText += " TotalAmount,";
                sqlText += " TotalVATAmount,";
                sqlText += " SerialNo,";
                sqlText += " Comments,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " SaleType,";
                sqlText += " PreviousSalesInvoiceNo,";
                sqlText += " Trading,";
                sqlText += " IsPrint,";
                sqlText += " TenderId,";
                sqlText += " TransactionType,";
                sqlText += " DeliveryDate,";
                sqlText += " SaleReturnId,";

                sqlText += " CurrencyID,";
                sqlText += " CurrencyRateFromBDT,";
                sqlText += " ImportIDExcel,";

                sqlText += " Post";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@newID,";
                sqlText += "@MasterCustomerID,";
                sqlText += "@MasterInvoiceDateTime,";
                sqlText += "@commonDaldecimalMasterTotalAmount,";
                sqlText += "@commonDaldecimalMasterTotalVATAmount,";
                sqlText += "@MasterSerialNo,";
                sqlText += "@MasterComments,";
                sqlText += "@MasterCreatedBy,";
                sqlText += "@MasterCreatedOn,";
                sqlText += "@MasterLastModifiedBy,";
                sqlText += "@MasterLastModifiedOn,";
                sqlText += "@MasterSaleType,";
                sqlText += "@MasterPreviousSalesInvoiceNo,";
                sqlText += "@MasterTrading,";
                sqlText += "@MasterIsPrint,";
                sqlText += "@MasterTenderId,";
                sqlText += "@MasterTransactionType,";
                sqlText += "@MasterDeliveryDate,";
                sqlText += "@MasterReturnId,";
                sqlText += "@MasterCurrencyID,";
                sqlText += "@commonDaldecimalMasterCurrencyRateFromBDT,";
                sqlText += "@MasterImportID,";
                sqlText += "@MasterPost";
                sqlText += ")";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@newID", newID);
                cmdInsert.Parameters.AddWithValue("@MasterCustomerID", Master.CustomerID);
                cmdInsert.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime);
                cmdInsert.Parameters.AddWithValue("@commonDaldecimalMasterTotalAmount", commonDal.decimal259(Master.TotalAmount));
                cmdInsert.Parameters.AddWithValue("@commonDaldecimaMaster.TotalVATAmount", commonDal.decimal259(Master.TotalVATAmount));
                cmdInsert.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo);
                cmdInsert.Parameters.AddWithValue("@MasterComments", Master.Comments);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@MasterSaleType", Master.SaleType);
                cmdInsert.Parameters.AddWithValue("@MasterPreviousSalesInvoiceNo", Master.PreviousSalesInvoiceNo);
                cmdInsert.Parameters.AddWithValue("@MasterTrading", Master.Trading);
                cmdInsert.Parameters.AddWithValue("@MasterIsPrint", Master.IsPrint);
                cmdInsert.Parameters.AddWithValue("@MasterTenderId", Master.TenderId);
                cmdInsert.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValue("@MasterDeliveryDate", Master.DeliveryDate);
                cmdInsert.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId);
                cmdInsert.Parameters.AddWithValue("@MasterCurrencyID", Master.CurrencyID);
                cmdInsert.Parameters.AddWithValue("@commonDaldecimalMasterCurrencyRateFromBDT", commonDal.decimal259(Master.CurrencyRateFromBDT));
                cmdInsert.Parameters.AddWithValue("@MasterImportID", Master.ImportID);
                cmdInsert.Parameters.AddWithValue("@MasterPost", Master.Post);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }


                #endregion ID generated completed,Insert new Information in Header
               
                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(SalesInvoiceNo) from BureauSalesInvoiceDetails WHERE SalesInvoiceNo=@newID ";
                    sqlText += " AND InvoiceName=@ItemInvoiceName";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@newID", newID);
                    cmdFindId.Parameters.AddWithValue("@ItemInvoiceName", Item.InvoiceName);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable
                   
                    sqlText = "";
                    sqlText += " insert into BureauSalesInvoiceDetails(";
                    sqlText += " SalesInvoiceNo,";
                    sqlText += " InvoiceLineNo,";
                    sqlText += " InvoiceName,";
                    sqlText += " InvoiceDateTime,";
                    sqlText += " Quantity,";
                    sqlText += " SalesPrice,";
                    sqlText += " VATRate,";
                    sqlText += " VATAmount,";
                    sqlText += " SubTotal,";
                    sqlText += " CustomerId,";

                    sqlText += " ItemNo,";
                    sqlText += " SD,";
                    sqlText += " SDAmount,";
                    sqlText += " UOM,";
                    sqlText += " Type,";
                    
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " ChallanDateTime,";
                    sqlText += " TransactionType,";
                    sqlText += " BureauType,";
                    sqlText += " BureauId,";
                    sqlText += " Post,";
                    sqlText += " DollerValue,";
                    sqlText += " CurrencyValue,";
                    sqlText += " InvoiceCurrency,";
                    sqlText += " CConversionDate";
                    if (Master.TransactionType == "Credit")
                    {
                        sqlText += ", ReturnTransactionType ";
                        sqlText += ", PreviousSalesInvoiceNo";
                    }
                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@newID,";
                                
                    sqlText += "@ItemInvoiceLineNo, ";
                    sqlText += "@ItemInvoiceName,";
                    sqlText += "@ItemInvoiceDateTime,";
                    sqlText += "@ItemQuantity,";
                    sqlText += "@ItemSalesPrice,";
                    sqlText += "@ItemVATRate,";
                    sqlText += "@ItemVATAmount,";
                    sqlText += "@ItemSubTotal,";
                    sqlText += "@MasterCustomerID,";
                                
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemSD,";
                    sqlText += "@ItemSDAmount,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemType,";
                                
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@MasterInvoiceDateTime,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@ItemBureauType,";
                    sqlText += "@ItemBureauId,";
                                
                    sqlText += "@MasterPost,";
                    sqlText += "@ItemDollerValue,";
                    sqlText += "@ItemCurrencyValue,";
                    sqlText += "@ItemInvoiceCurrency, ";
                    sqlText += "@ItemCConversionDate ";

                    if (Master.TransactionType == "Credit" )
                    {
                        sqlText += ",@ItemReturnTransactionType";
                        sqlText += ",@ItemPreviousSalesInvoiceNo";
                    }
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsert.Parameters.AddWithValue("@newID", newID);
                    cmdInsert.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemInvoiceName", Item.InvoiceName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemInvoiceDateTime", Item.InvoiceDateTime);
                    cmdInsert.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                    cmdInsert.Parameters.AddWithValue("@ItemSalesPrice", Item.SalesPrice);
                    cmdInsert.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                    cmdInsert.Parameters.AddWithValue("@ItemVATAmount", Item.VATAmount);
                    cmdInsert.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                    cmdInsert.Parameters.AddWithValue("@MasterCustomerID", Master.CustomerID);
                    cmdInsert.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemSD", Item.SD);
                    cmdInsert.Parameters.AddWithValue("@ItemSDAmount", Item.SDAmount);
                    cmdInsert.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemType", Item.Type ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsert.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime);
                    cmdInsert.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemBureauType", Item.BureauType ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemBureauId", Item.BureauId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemDollerValue", Item.DollerValue);
                    cmdInsert.Parameters.AddWithValue("@ItemCurrencyValue", Item.CurrencyValue);
                    cmdInsert.Parameters.AddWithValue("@ItemInvoiceCurrency", Item.InvoiceCurrency);
                    cmdInsert.Parameters.AddWithValue("@ItemCConversionDate", Item.CConversionDate);
                    cmdInsert.Parameters.AddWithValue("@ItemReturnTransactionType", Item.ReturnTransactionType ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemPreviousSalesInvoiceNo", Item.PreviousSalesInvoiceNo ?? Convert.DBNull);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable
                }



                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from SalesInvoiceHeaders WHERE SalesInvoiceNo=@newID ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@newID", newID);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableCreatID);
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
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                if (vcurrConn == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (ArgumentNullException aeg)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + aeg.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + aeg.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            catch (Exception ex)
            {
                if (vcurrConn == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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
        public string[] SalesUpdate(SaleMasterVM Master, List<BureauSaleDetailVM> Details)
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

            string vehicleId = "0";

            

            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.InvoiceDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.InvoiceDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, "Please Check Invoice Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                int IssuePlaceQty = Convert.ToInt32(commonDal.settings("Issue", "Quantity"));
                int IssuePlaceAmt = Convert.ToInt32(commonDal.settings("Issue", "Amount"));
                int ReceivePlaceAmt = Convert.ToInt32(commonDal.settings("Receive", "Amount"));
              
                transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                //string transactionDate = Master.InvoiceDateTime;
                //string transactionYearCheck = Convert.ToDateTime(Master.InvoiceDateTime).ToString("yyyy-MM-dd");
                //if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                //{

                //    #region YearLock
                //    sqlText = "";

                //    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                //                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                //    DataTable dataTable = new DataTable("ProductDataT");
                //    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                //    cmdIdExist.Transaction = transaction;
                //    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                //    reportDataAdapt.Fill(dataTable);

                //    if (dataTable == null)
                //    {
                //        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                //    }

                //    else if (dataTable.Rows.Count <= 0)
                //    {
                //        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                //    }
                //    else
                //    {
                //        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                //        {
                //            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                //        }
                //        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                //        {
                //            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                //        }
                //    }
                //    #endregion YearLock
                //    #region YearNotExist
                //    sqlText = "";
                //    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                //    DataTable dtYearNotExist = new DataTable("ProductDataT");

                //    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                //    cmdYearNotExist.Transaction = transaction;
                //    //countId = (int)cmdIdExist.ExecuteScalar();

                //    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                //    YearNotExistDataAdapt.Fill(dtYearNotExist);

                //    if (dtYearNotExist == null)
                //    {
                //        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                //    }

                //    else if (dtYearNotExist.Rows.Count < 0)
                //    {
                //        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                //    }
                //    else
                //    {
                //        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                //            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                //        {
                //            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                //        }
                //    }
                //    #endregion YearNotExist

                //}


                #endregion Fiscal Year CHECK
                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(SalesInvoiceNo) from SalesInvoiceHeaders WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasteSalesInvoiceNo", Master.SalesInvoiceNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update SalesInvoiceHeaders set  ";
                sqlText += " CustomerID             =@MasterCustomerID ,";
                sqlText += " DeliveryAddress1       =@MasterDeliveryAddress1 ,";
                sqlText += " DeliveryAddress2       =@MasterDeliveryAddress2 ,";
                sqlText += " DeliveryAddress3       =@MasterDeliveryAddress3 ,";
                sqlText += " InvoiceDateTime        =@MasterInvoiceDateTime ,";
                sqlText += " TotalAmount            =@MasterTotalAmount ,";
                sqlText += " TotalVATAmount         =@MasterTotalVATAmount ,";
                sqlText += " SerialNo               =@MasterSerialNo ,";
                sqlText += " Comments               =@MasterComments ,";
                sqlText += " DeliveryDate           =@MasterDeliveryDate ,";
                sqlText += " LastModifiedBy         =@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn         =@MasterLastModifiedOn ,";
                sqlText += " SaleType               =@MasterSaleType ,";
                sqlText += " PreviousSalesInvoiceNo =@MasterPreviousSalesInvoiceNo ,";
                sqlText += " Trading                =@MasterTrading ,";
                sqlText += " IsPrint                =@MasterIsPrint ,";
                sqlText += " TenderId               =@MasterTenderId ,";
                sqlText += " TransactionType        =@MasterTransactionType ,";
                sqlText += " SaleReturnId           =@MasterReturnId ,";
                sqlText += " CurrencyID             =@MasterCurrencyID ,";
                sqlText += " CurrencyRateFromBDT    =@MasterCurrencyRateFromBDT ,";
                sqlText += " Post=@MasterPost ";
                sqlText += " where  SalesInvoiceNo  =@MasterSalesInvoiceNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@MasterCustomerID", Master.CustomerID);
                cmdUpdate.Parameters.AddWithValue("@MasterDeliveryAddress1", Master.DeliveryAddress1 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterDeliveryAddress2", Master.DeliveryAddress2 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterDeliveryAddress3", Master.DeliveryAddress3 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterDeliveryDate", Master.DeliveryDate);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterSaleType", Master.SaleType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterPreviousSalesInvoiceNo", Master.PreviousSalesInvoiceNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterTrading", Master.Trading ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterIsPrint", Master.IsPrint ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterTenderId", Master.TenderId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterCurrencyID", Master.CurrencyID ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterCurrencyRateFromBDT", Master.CurrencyRateFromBDT);
                cmdUpdate.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                }
                #endregion update Header

                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(SalesInvoiceNo) from BureauSalesInvoiceDetails WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                    sqlText += " AND InvoiceName=@ItemInvoiceName ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@MasteSalesInvoiceNo", Master.SalesInvoiceNo);
                    cmdFindId.Parameters.AddWithValue("@ItemInvoiceName", Item.InvoiceName);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        // Insert

                        #region Insert only DetailTable
                       
                        sqlText = "";
                        sqlText += " insert into BureauSalesInvoiceDetails(";
                        sqlText += " SalesInvoiceNo,";
                        sqlText += " InvoiceLineNo,";
                        sqlText += " InvoiceName,";
                        sqlText += " InvoiceDateTime,";
                        sqlText += " Quantity,";
                        sqlText += " SalesPrice,";
                        sqlText += " VATRate,";
                        sqlText += " VATAmount,";
                        sqlText += " SubTotal,";

                        sqlText += " ItemNo,";
                        sqlText += " SD,";
                        sqlText += " SDAmount,";
                        sqlText += " UOM,";
                        sqlText += " Type,";

                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " ChallanDateTime,";
                        sqlText += " TransactionType,";
                        sqlText += " BureauType,";

                        sqlText += " Post,";
                        sqlText += " DollerValue,";
                        sqlText += " CurrencyValue,";
                        sqlText += " InvoiceCurrency,";
                        sqlText += " CConversionDate";
                        if (Master.TransactionType == "Credit")
                        {
                            sqlText += ", ReturnTransactionType ";
                            sqlText += ", PreviousSalesInvoiceNo";
                        }
                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@MasterSalesInvoiceNo ,";
                                    
                        sqlText += "@ItemInvoiceLineNo , ";
                        sqlText += "@ItemInvoiceName,";
                        sqlText += "@ItemInvoiceDateTime,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@ItemSalesPrice,";
                        sqlText += "@ItemVATRate,";
                        sqlText += "@ItemVATAmount,";
                        sqlText += "@ItemSubTotal,";
                                    
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemSD,";
                        sqlText += "@ItemSDAmount,";
                        sqlText += "@ItemUOM,";
                        sqlText += "@ItemType,";
                                    
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@MasterInvoiceDateTime,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@ItemBureauType,";
                                    
                        sqlText += "@MasterPost,";
                        sqlText += "@ItemDollerValue,";
                        sqlText += "@ItemCurrencyValue,";
                        sqlText += "@ItemInvoiceCurrency,";
                        sqlText += "@ItemCConversionDate ";

                        if (Master.TransactionType == "Credit" )
                        {
                            sqlText += ",@ItemReturnTransactionType";
                            sqlText += ",@ItemPreviousSalesInvoiceNo";
                        }
                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemInvoiceName", Item.InvoiceName ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemInvoiceDateTime", Item.InvoiceDateTime);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSalesPrice", Item.SalesPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATAmount", Item.VATAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSD", Item.SD);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSDAmount", Item.SDAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemType", Item.Type ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemBureauType", Item.BureauType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemDollerValue", Item.DollerValue);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCurrencyValue", Item.CurrencyValue);
                        cmdInsDetail.Parameters.AddWithValue("@ItemInvoiceCurrency", Item.InvoiceCurrency);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCConversionDate", Item.CConversionDate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemReturnTransactionType", Item.ReturnTransactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemPreviousSalesInvoiceNo", Item.PreviousSalesInvoiceNo ?? Convert.DBNull);


                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                        }
                        #endregion Insert only DetailTable
                    }
                    else
                    {
                        //Update


                        #region Update Issue and Receive if Transaction is not Other

                        #region Update only DetailTable

                        sqlText = "";

                        sqlText += " update BureauSalesInvoiceDetails set ";

                        sqlText += " InvoiceLineNo  =@ItemInvoiceLineNo ,";
                        sqlText += " InvoiceDateTime=@ItemInvoiceDateTime ,";
                        sqlText += " ItemNo         =@ItemItemNo ,";
                        sqlText += " Quantity       =@ItemQuantity ,";
                        sqlText += " SalesPrice     =@ItemSalesPrice ,";
                        sqlText += " UOM            =@ItemUOM ,";
                        sqlText += " VATRate        =@ItemVATRate ,";
                        sqlText += " VATAmount      =@ItemVATAmount ,";
                        sqlText += " SubTotal       =@ItemSubTotal ,";
                        sqlText += " LastModifiedBy =@MasterLastModifiedBy ,";
                        sqlText += " LastModifiedOn =@MasterLastModifiedOn ,";
                        sqlText += " SD             =@ItemSD ,";
                        sqlText += " SDAmount       =@ItemSDAmount ,";
                        sqlText += " ChallanDateTime=@MasterInvoiceDateTime ,";
                        sqlText += " Type           =@ItemType ,";
                        sqlText += " TransactionType=@MasterTransactionType ,";
                        sqlText += " DollerValue    =@ItemDollerValue,";
                        sqlText += " CurrencyValue  =@ItemCurrencyValue,";
                        sqlText += " Post           =@MasterPost,";
                        sqlText += " CConversionDate=@ItemCConversionDate";

                        if (Master.TransactionType == "Credit")
                        {
                            sqlText += ", ReturnTransactionType =@ItemReturnTransactionType";
                            sqlText += ", PreviousSalesInvoiceNo=@MasterPreviousSalesInvoiceNo";

                        }

                        sqlText += " where  SalesInvoiceNo  =@MasterSalesInvoiceNo ";
                        sqlText += " and InvoiceName        =@ItemInvoiceName";



                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemInvoiceDateTime", Item.InvoiceDateTime);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSalesPrice", Item.SalesPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATAmount", Item.VATAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSD", Item.SD);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSDAmount", Item.SDAmount);
                        cmdInsDetail.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime);
                        cmdInsDetail.Parameters.AddWithValue("@ItemType", Item.Type ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemDollerValue", Item.DollerValue);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCurrencyValue", Item.CurrencyValue);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCConversionDate", Item.CConversionDate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemReturnTransactionType", Item.ReturnTransactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPreviousSalesInvoiceNo", Master.PreviousSalesInvoiceNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemInvoiceName", Item.InvoiceName ?? Convert.DBNull);


                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable



                        #endregion Update Issue and Receive if Transaction is not Other
                    }

                    #endregion Find Transaction Mode Insert or Update
                }// foreach (var Item in Details.ToList())
                #region Remove row
                sqlText = "";
                sqlText += " SELECT  distinct InvoiceName";
                sqlText += " from BureauSalesInvoiceDetails WHERE SalesInvoiceNo='" + Master.SalesInvoiceNo + "'";

                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                cmdRIFB.Transaction = transaction;
                SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB);
                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var p = pItem["InvoiceName"].ToString();

                    //var tt = Details.Find(x => x.ItemNo == p);
                    var tt = Details.Count(x => x.InvoiceName.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM BureauSalesInvoiceDetails ";
                        sqlText += " WHERE SalesInvoiceNo=@MasterSalesInvoiceNo";
                        sqlText += " AND InvoiceName=@p";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                        cmdInsDetail.Parameters.AddWithValue("@p", p);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                    }

                }
                #endregion Remove row


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)

               #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from SalesInvoiceHeaders WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableCreatID);
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
                retResults[1] = MessageVM.saleMsgUpdateSuccessfully;
                retResults[2] = Master.SalesInvoiceNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

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
        public string[] SalesPost(SaleMasterVM Master, List<BureauSaleDetailVM> Details)
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
                CommonDAL commonDal = new CommonDAL();

                string vNegStockAllow = string.Empty;
                vNegStockAllow = commonDal.settings("Sale", "NegStockAllow");
                if (string.IsNullOrEmpty(vNegStockAllow))
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Sale");
                }
                bool NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);

                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.InvoiceDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.InvoiceDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgCheckDatePost);

                }

                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNamePost);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.InvoiceDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.InvoiceDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(SalesInvoiceNo) from SalesInvoiceHeaders WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update


                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";
                sqlText += " update SalesInvoiceHeaders set  ";
                sqlText += " LastModifiedBy= @MasterLastModifiedBy,";
                sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                sqlText += " Post=@MasterPost ";
                sqlText += " where  SalesInvoiceNo=@MasterSalesInvoiceNo ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post);
                cmdUpdate.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);


                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                }
                #endregion update Header

               
                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)  /////
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgNoDataToPost);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(SalesInvoiceNo) from BureauSalesInvoiceDetails WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                    sqlText += " AND InvoiceName=@ItemInvoiceName ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                    cmdFindId.Parameters.AddWithValue("@ItemInvoiceName", Item.InvoiceName);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgNoDataToPost);
                    }
                    #endregion Find Transaction Mode Insert or Update
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";
                        sqlText += " update BureauSalesInvoiceDetails set ";
                        sqlText += " LastModifiedBy= @MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                        sqlText += " Post=@MasterPost ";
                        sqlText += " where  SalesInvoiceNo=@MasterSalesInvoiceNo ";
                        sqlText += " and InvoiceName =@ItemInvoiceName ";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post);
                        cmdInsDetail.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                        cmdInsDetail.Parameters.AddWithValue("@ItemInvoiceName", Item.InvoiceName);


                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                        }
                        #endregion Update only DetailTable
                    }
                    

                }


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from SalesInvoiceHeaders WHERE SalesInvoiceNo=@MasterSalesInvoiceNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgUnableCreatID);
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
                retResults[1] = MessageVM.saleMsgSuccessfullyPost;
                retResults[2] = Master.SalesInvoiceNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

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

        public DataTable SearchSalesHeaderDTNew(string SalesInvoiceNo, string CustomerName,string CustomerGroupName, string VehicleType, string VehicleNo, string SerialNo, string InvoiceDateFrom,string InvoiceDateTo, string SaleType, string Trading, string IsPrint, string transactionType, string Post)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchSalesHeader");

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
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("SalesInvoiceDetails", "DiscountAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("SalesInvoiceDetails", "DiscountedNBRPrice", "decimal(25, 9)", currConn);
                #endregion open connection and transaction

                #region SQL Statement



                sqlText = "";
                //sqlText += " insert into IssueHeaders(";
                sqlText += " SELECT ";
                sqlText += " SalesInvoiceHeaders.SalesInvoiceNo, ";
                sqlText += " SalesInvoiceHeaders.CustomerID,";
                sqlText += " isnull(Customers.CustomerName,'N/A')CustomerName,";
                sqlText += " isnull(CustomerGroups.CustomerGroupName,'N/A')CustomerGroupName,";
                sqlText += " isnull(SalesInvoiceHeaders.DeliveryAddress1,'N/A')DeliveryAddress1,";
                sqlText += " isnull(SalesInvoiceHeaders.DeliveryAddress2,'N/A')DeliveryAddress2,";
                sqlText += " isnull(SalesInvoiceHeaders.DeliveryAddress3,'N/A')DeliveryAddress3,";
                sqlText += " isnull(SalesInvoiceHeaders.LCNumber,'N/A')LCNumber,";

                sqlText += " SalesInvoiceHeaders.VehicleID,";
                sqlText += " isnull(Vehicles.VehicleType,'N/A')VehicleType,";
                sqlText += " isnull(Vehicles.VehicleNo,'N/A')VehicleNo,";
                sqlText += " isnull(SalesInvoiceHeaders.TotalAmount,0)TotalAmount,";
                sqlText += " isnull(SalesInvoiceHeaders.TotalVATAmount,0)TotalVATAmount,";
                sqlText += " isnull(SalesInvoiceHeaders.SerialNo,'N/A')SerialNo,";
                sqlText += " convert (varchar,SalesInvoiceHeaders.InvoiceDateTime,120)InvoiceDate,";
                sqlText += " convert (varchar,SalesInvoiceHeaders.DeliveryDate,120)DeliveryDate,";
                sqlText += " isnull(SalesInvoiceHeaders.Comments ,'N/A')Comments,";
                sqlText += " SaleType,isnull(PreviousSalesInvoiceNo,SalesInvoiceHeaders.SalesInvoiceNo) PID,";
                sqlText += " Trading,IsPrint,";
                sqlText += " SalesInvoiceHeaders.TenderId,";
                sqlText += " isnull(SalesInvoiceHeaders.transactionType,'NA')transactionType,";
                sqlText += " isnull(SalesInvoiceHeaders.CurrencyID,'260')CurrencyID,";
                sqlText += " isnull(cur.CurrencyCode,'BDT')CurrencyCode,";
                sqlText += " isnull(SalesInvoiceHeaders.CurrencyRateFromBDT,1)CurrencyRateFromBDT,";
                sqlText += " isnull(SalesInvoiceHeaders.AlReadyPrint,0)AlReadyPrint,";
                sqlText += " isnull(SalesInvoiceHeaders.SaleReturnId,0)SaleReturnId,";
                sqlText += " isnull(SalesInvoiceHeaders.Post,'N')Post";
                sqlText += " FROM  SalesInvoiceHeaders LEFT OUTER JOIN";
                sqlText += " Customers ON  SalesInvoiceHeaders.CustomerID =  Customers.CustomerID LEFT OUTER JOIN";
                sqlText += " CustomerGroups ON  Customers.CustomerGroupID =  CustomerGroups.CustomerGroupID LEFT OUTER JOIN";
                sqlText += " Vehicles ON  SalesInvoiceHeaders.VehicleID =  Vehicles.VehicleID  LEFT OUTER JOIN ";
                sqlText += " Currencies cur ON isnull(SalesInvoiceHeaders.CurrencyID,'260')=cur.CurrencyId";
                sqlText += " WHERE ";
                sqlText += " (SalesInvoiceHeaders.SalesInvoiceNo LIKE '%' +'" + SalesInvoiceNo + "' + '%' OR SalesInvoiceHeaders.SalesInvoiceNo IS NULL)  ";
                //sqlText += " AND (SalesInvoiceHeaders.CustomerID LIKE '%' + '" + CustomerID + "' + '%' OR '" + CustomerID + "' IS NULL) ";
                sqlText += " AND (Customers.CustomerName LIKE '%' + '" + CustomerName + "' + '%' OR Customers.CustomerName IS NULL)  ";
                //sqlText += " AND (Customers.CustomerGroupID LIKE '%' +'" + CustomerGroupID + "' + '%' OR '" + CustomerGroupID + "' IS NULL) ";
                sqlText += " AND (CustomerGroups.CustomerGroupName LIKE '%' + '" + CustomerGroupName + "' + '%' OR customerGroups.CustomerGroupName IS NULL)  ";
                //sqlText += " AND (SalesInvoiceHeaders.VehicleID LIKE '%' + '" + VehicleID + "' + '%' OR '" + VehicleID + "' IS NULL)  ";
                //sqlText += " AND (Vehicles.VehicleType LIKE '%' + '" + VehicleType + "' + '%' OR '" + VehicleType + "' IS NULL)  ";
                sqlText += " AND (Vehicles.VehicleNo LIKE '%' + '" + VehicleNo + "' + '%' OR Vehicles.VehicleNo IS NULL)  ";
                sqlText += " AND (SalesInvoiceHeaders.SerialNo LIKE '%' + '" + SerialNo + "' + '%' OR SalesInvoiceHeaders.SerialNo IS NULL) ";
                sqlText += " AND (SalesInvoiceHeaders.InvoiceDateTime>= '" + InvoiceDateFrom + "' ) ";
                sqlText += " AND (SalesInvoiceHeaders.InvoiceDateTime<dateadd(d,1, '" + InvoiceDateTo + "') ) ";
                sqlText += " AND (SalesInvoiceHeaders.SaleType LIKE '%' + '" + SaleType + "' + '%' OR SalesInvoiceHeaders.SaleType IS NULL) ";
                sqlText += " AND (SalesInvoiceHeaders.Trading LIKE '%' + '" + Trading + "' + '%' OR SalesInvoiceHeaders.Trading IS NULL) ";
                sqlText += " AND (SalesInvoiceHeaders.IsPrint LIKE '%' + '" + IsPrint + "' + '%' OR SalesInvoiceHeaders.IsPrint IS NULL) ";
                if (transactionType != "All")
                {
                    sqlText += " AND (SalesInvoiceHeaders.transactionType in('" + transactionType + "')) ";
                }
                sqlText += " AND (SalesInvoiceHeaders.Post LIKE '%' + '" + Post + "' + '%' OR SalesInvoiceHeaders.Post IS NULL) ";
                sqlText += " order by InvoiceDateTime desc";

                #endregion

                #region SQL Command

                SqlCommand objCommSalesHeader = new SqlCommand();
                objCommSalesHeader.Connection = currConn;

                objCommSalesHeader.CommandText = sqlText;
                objCommSalesHeader.CommandType = CommandType.Text;

                #endregion



                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSalesHeader);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally

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

            #endregion

            return dataTable;

        }


        public DataTable SearchSaleDetailDTNew(string SalesInvoiceNo)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchSalesDetail");

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

                sqlText = "";
                sqlText = @"
                            SELECT    
                            BureauSalesInvoiceDetails.SalesInvoiceNo,
                            BureauSalesInvoiceDetails.InvoiceLineNo, 
                            BureauSalesInvoiceDetails.ItemNo,

                            isnull(BureauSalesInvoiceDetails.InvoiceName,' ')InvoiceName,
                            convert (varchar,isnull(BureauSalesInvoiceDetails.InvoiceDateTime,'01/01/1900'),120) InvoiceDateTime,

                            isnull(BureauSalesInvoiceDetails.Quantity,0)Quantity, 
                            isnull(BureauSalesInvoiceDetails.SalesPrice,0)SalesPrice,
                            isnull(BureauSalesInvoiceDetails.VATRate,0)VATRate ,
                            isnull(BureauSalesInvoiceDetails.VATAmount,0)VATAmount ,
                            isnull(BureauSalesInvoiceDetails.SubTotal,0)SubTotal,
isnull(BureauSalesInvoiceDetails.UOM,'N/A')UOM,
                            isnull(BureauSalesInvoiceDetails.SD,0)SD,
                            isnull(BureauSalesInvoiceDetails.SDAmount,0)SDAmount,
                           
                            isnull(Products.ProductName,'N/A')ProductName,
                            isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
                            isnull(BureauSalesInvoiceDetails.PreviousSalesInvoiceNo,'')PreviousSalesInvoiceNo,
                            convert (varchar,isnull(BureauSalesInvoiceDetails.ChallanDateTime,'01/01/1900'),120) ChallanDateTime,
                            isnull(Products.ProductCode,'N/A')ProductCode,
                            
                            isnull(BureauSalesInvoiceDetails.CurrencyValue,BureauSalesInvoiceDetails.SubTotal)CurrencyValue,
                            isnull(BureauSalesInvoiceDetails.DollerValue,0)DollerValue,
                            isnull(BureauSalesInvoiceDetails.ReturnTransactionType,'')ReturnTransactionType,
                            isnull(BureauSalesInvoiceDetails.InvoiceCurrency,'')InvoiceCurrency,
                            isnull(BureauSalesInvoiceDetails.Type,'VAT')Type,

                            convert (varchar,isnull(BureauSalesInvoiceDetails.CConversionDate,'01/01/1900'),120) CConversionDate

                            FROM  BureauSalesInvoiceDetails LEFT OUTER JOIN
                             Products ON BureauSalesInvoiceDetails.ItemNo = Products.ItemNo               
                            WHERE 
                            (BureauSalesInvoiceDetails.SalesInvoiceNo = @SalesInvoiceNo) 
                            order by BureauSalesInvoiceDetails.InvoiceLineNo asc                            
                            ";
                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;

                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion.

                #region Parameter
                if (!objCommSaleDetail.Parameters.Contains("@SalesInvoiceNo"))
                { objCommSaleDetail.Parameters.AddWithValue("@SalesInvoiceNo", SalesInvoiceNo); }
                else { objCommSaleDetail.Parameters["@SalesInvoiceNo"].Value = SalesInvoiceNo; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally

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

            #endregion

            return dataTable;

        }

        public int LoadIssueItems()
        {
            #region Variables
            int transResult = 0;

            string sqlText = "";
            DataTable dtIssue = new DataTable("IssueDetails");
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try
            try
            {
                currConn = _dbsqlConnection.GetConnection();

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #region SQL Statement

                sqlText = @"
                            SELECT    
                            IssueNo,
                            ItemNo,
                            IssueDateTime
                            FROM  IssueDetails  
                            where isnull(IsProcess,'N') <>'Y'
                            
                            order by IssueDateTime    
 
                           
                            ";
                #endregion

                #region SQL Command

                SqlCommand objsql = new SqlCommand();
                objsql.Transaction = transaction;

                objsql.Connection = currConn;
                objsql.CommandText = sqlText;
                objsql.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter1 = new SqlDataAdapter(objsql);
                dataAdapter1.Fill(dtIssue);

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
                #endregion
                if (dtIssue == null)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                    MessageVM.receiveMsgNoDataToPost);
                }

                else
                {
                    ProductDAL sale = new ProductDAL();
                    string vIssueItem = "";
                    string vIssueNo = "";
                    string vIssueDate;
                    foreach (DataRow IssueItem in dtIssue.Rows)
                    {
                        vIssueDate = Convert.ToDateTime(IssueItem["IssueDateTime"]).ToString("yyyy-MM-dd");
                        vIssueItem = IssueItem["ItemNo"].ToString();
                        vIssueNo = IssueItem["IssueNo"].ToString();
                        DataTable dt = sale.AvgPriceNew(vIssueItem, vIssueDate, null, null,false);
                        decimal Quantity = Convert.ToDecimal(dt.Rows[0]["Quantity"]);
                        decimal Amount = Convert.ToDecimal(dt.Rows[0]["Amount"]);
                        decimal AvgRate = 0;
                        if (Quantity == 0)
                        {
                            AvgRate = 0;
                        }
                        else
                        {
                            AvgRate = Amount / Quantity;
                        }
                        if (AvgRate <= 0)
                        {
                            AvgRate = 0;
                        }
                        UpdatIssueAvgPrice(vIssueNo, vIssueItem, AvgRate);

                    }
                }
            }
            #endregion

            #region Catch & Finally

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

            return transResult;

        }

        private int UpdatIssueAvgPrice(string invoiceNo, string itemNo, decimal avgPrice)
        {
            #region Variables


            int transResult = 0;
            string sqlText = "";
            decimal retResults = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try
            try
            {
                currConn = _dbsqlConnection.GetConnection();

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #region Last UseQuantity

                sqlText = "  ";
                sqlText += " SELECT SubTotal FROM IssueDetails ";
                sqlText += " where  IssueNo =@invoiceNo ";
                sqlText += " and ItemNo =@itemNo ";
                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@invoiceNo", invoiceNo);
                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@itemNo", itemNo);

                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {

                }
                else
                {
                    retResults = (decimal)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                }
                #endregion Last UseQuantity
                CommonDAL commonDal = new CommonDAL();
                var tt = retResults;
                var tt1 = avgPrice;
                var tt2 = commonDal.decimal259(avgPrice);


                var tt4 = "";
                #region Update only DetailTable

                sqlText = "";

                sqlText += " update IssueDetails set ";
                sqlText += " UOMPrice=@avgPrice ,";
                sqlText += " SubTotal=@avgPrice * UOMQty ,";
                sqlText += " NBRPrice=@avgPrice * UOMc ,";
                sqlText += " CostPrice=@avgPrice * UOMc ,";
                sqlText += " IsProcess='Y'";

                sqlText += " where  IssueNo =@invoiceNo ";
                sqlText += " and ItemNo =@ itemNo";

                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                cmdInsDetail.Transaction = transaction;
                cmdInsDetail.Parameters.AddWithValue("@avgPrice", avgPrice);
                cmdInsDetail.Parameters.AddWithValue("@invoiceNo", invoiceNo);
                cmdInsDetail.Parameters.AddWithValue("@itemNo", itemNo);


                transResult = (int)cmdInsDetail.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("Update Avg Price", "Update Avg Price error");

                }
                #endregion Update only DetailTable
            }


            #endregion

            #region Catch & Finally

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
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            return transResult;

            #endregion

        }
        
        public string[] SalesInsertImport(SaleMasterVM Master, List<SaleDetailVM> Details, List<SaleExportVM> ExportDetails
           , SqlTransaction transaction, SqlConnection currConn)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection vcurrConn = currConn;
            //SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            string newID = "";
            string PostStatus = "";

            int IDExist = 0;
            string vehicleId = "0";


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.InvoiceDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.InvoiceDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, "Please Check Invoice Data and Time");

                }

                #endregion Validation for Header
                CommonDAL commonDal = new CommonDAL();
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameInsert);

                }
                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.InvoiceDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.InvoiceDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(SalesInvoiceNo) from SalesInvoiceHeaders WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgFindExistID);
                }

                #endregion Find Transaction Exist



                #region Sale ID Create
                if (string.IsNullOrEmpty(Master.TransactionType)) // start
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }

                if (Master.TransactionType == "Other")
                {
                    newID = commonDal.TransactionCode("Sale", "Other", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                              "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                else if (Master.TransactionType == "Trading")
                {
                    newID = commonDal.TransactionCode("Sale", "Trading", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                else if (Master.TransactionType == "VAT11GaGa")
                {
                    newID = commonDal.TransactionCode("VAT11GaGa", "VAT11GaGa", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                else if (Master.TransactionType == "Debit")
                {
                    newID = commonDal.TransactionCode("Sale", "Debit", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                       
                else if (Master.TransactionType == "ExportServiceNS")
                {
                    newID = commonDal.TransactionCode("Sale", "ExportServiceNS", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                else if (Master.TransactionType == "ExportServiceNSCredit")
                {
                    newID = commonDal.TransactionCode("Sale", "ExportServiceNSCredit", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                else if (Master.TransactionType == "Credit")
                {
                    newID = commonDal.TransactionCode("Sale", "Credit", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }


                else if (
                    Master.TransactionType == "Export"
                    || Master.TransactionType == "ExportTrading"
                    || Master.TransactionType == "ExportServiceNS"
                    || Master.TransactionType == "ExportServiceNSCredit"
                    || Master.TransactionType == "ExportService"
                    || Master.TransactionType == "ExportTradingTender"
                    || Master.TransactionType == "ExportTender"
                    || Master.TransactionType == "ExportPackage"
                    )
                {
                    newID = commonDal.TransactionCode("Sale", "Export", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }


                else if (Master.TransactionType == "InternalIssue")
                {
                    newID = commonDal.TransactionCode("InternalIssue", "InternalIssue", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }


                else if (Master.TransactionType == "Service")
                {
                    newID = commonDal.TransactionCode("Sale", "Service", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                else if (Master.TransactionType == "ServiceNS")
                {
                    newID = commonDal.TransactionCode("Sale", "ServiceNS", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                else if (Master.TransactionType == "Tender"
                    || Master.TransactionType == "TradingTender"
                    )
                {
                    newID = commonDal.TransactionCode("Sale", "Tender", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }


                else if (Master.TransactionType == "TollIssue")
                {
                    newID = commonDal.TransactionCode("TollIssue", "TollIssue", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                else if (Master.TransactionType == "TollFinishIssue")
                {
                    newID = commonDal.TransactionCode("TollFinishIssue", "TollFinishIssue", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }

                else if (Master.TransactionType == "PackageSale")
                {
                    newID = commonDal.TransactionCode("Sale", "Package", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }
                if (string.IsNullOrEmpty(newID) || newID == "")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                            "ID Prefetch not set please update Prefetch first");
                }



                #endregion Purchase ID Create Not Complete

                #region VehicleId
                if (Master.TransactionType == "Service"
                    || Master.TransactionType == "ServiceNS"
                    || Master.TransactionType == "ExportService"
                    || Master.TransactionType == "ExportServiceNS"
                    || Master.TransactionType == "ExportServiceNSCredit"
                    )
                {
                    vehicleId = "0";
                }
                else
                {
                    string vehicleID = "0";
                    sqlText = "";
                    sqlText = sqlText + "select VehicleID from Vehicles WHERE VehicleNo=@MasterVehicleNo ";

                    SqlCommand cmdExistVehicleId = new SqlCommand(sqlText, currConn);
                    cmdExistVehicleId.Transaction = transaction;
                    cmdExistTran.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo);

                    string vehicleIDExist = (string)cmdExistVehicleId.ExecuteScalar();
                    vehicleId = vehicleIDExist;

                    if (Convert.ToDecimal(vehicleId) <= 0 || string.IsNullOrEmpty(vehicleId))
                    {
                        sqlText = "";
                        sqlText = sqlText + "select isnull(max(cast(VehicleID as int)),0)+1 FROM  Vehicles ";
                        SqlCommand cmdVehicleId = new SqlCommand(sqlText, currConn);
                        cmdVehicleId.Transaction = transaction;
                        int NewvehicleID = (int)cmdVehicleId.ExecuteScalar();
                        vehicleID = NewvehicleID.ToString();

                        sqlText = "";
                        sqlText +=
                            " INSERT INTO Vehicles (VehicleID,	VehicleType,	VehicleNo,	Description,	Comments,	ActiveStatus,CreatedBy,	CreatedOn,	LastModifiedBy,	LastModifiedOn)";
                        sqlText += "values('@vehicleID,";
                        sqlText += " @MasterVehicleType,";
                        sqlText += " @MasterVehicleNo,";
                        sqlText += " 'NA',";
                        sqlText += " 'NA',";
                        if (Master.vehicleSaveInDB == true)
                        {
                            sqlText += " 'Y',";

                        }
                        else
                        {
                            sqlText += " 'N',";
                        }

                        sqlText += "@MasterCreatedBy";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@ MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn )";
                        //sqlText += " from Vehicles;";

                        SqlCommand cmdExistVehicleIns = new SqlCommand(sqlText, currConn);
                        cmdExistVehicleIns.Transaction = transaction;
                        cmdExistVehicleIns.Parameters.AddWithValue("@vehicleID", vehicleID ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleType", Master.VehicleType ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);



                        transResult = (int)cmdExistVehicleIns.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                            MessageVM.saleMsgUnableCreatID);
                        }
                        vehicleId = vehicleID.ToString();
                        if (string.IsNullOrEmpty(vehicleId))
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableCreatID);
                        }
                    }

                }


                #endregion VehicleId
                #region ID generated completed,Insert new Information in Header

                var vTotalAmount = commonDal.decimal259(Master.TotalAmount);

                sqlText = "";
                sqlText += " insert into SalesInvoiceHeaders";
                sqlText += " (";
                sqlText += " SalesInvoiceNo,";
                sqlText += " CustomerID,";
                sqlText += " DeliveryAddress1,";
                sqlText += " DeliveryAddress2,";
                sqlText += " DeliveryAddress3,";
                sqlText += " VehicleID,";
                sqlText += " InvoiceDateTime,";
                sqlText += " TotalAmount,";
                sqlText += " TotalVATAmount,";
                sqlText += " SerialNo,";
                sqlText += " Comments,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " SaleType,";
                sqlText += " PreviousSalesInvoiceNo,";
                sqlText += " Trading,";
                sqlText += " IsPrint,";
                sqlText += " TenderId,";
                sqlText += " TransactionType,";
                sqlText += " DeliveryDate,";
                sqlText += " SaleReturnId,";

                sqlText += " CurrencyID,";
                sqlText += " CurrencyRateFromBDT,";
                sqlText += " ImportIDExcel,";

                sqlText += " Post";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@newID,";
                sqlText += "@MasterCustomerID,";
                sqlText += "@MasterDeliveryAddress1,";
                sqlText += "@MasterDeliveryAddress2,";
                sqlText += "@MasterDeliveryAddress3,";
                sqlText += "@vehicleId,";
                sqlText += "@MasterInvoiceDateTime,";
                sqlText += "@commonDaldecimalMasterTotalAmount,";
                sqlText += "@commonDaldecimalMasterTotalVATAmount,";
                sqlText += "@MasterSerialNo,";
                sqlText += "@MasterComments,";
                sqlText += "@MasterCreatedBy,";
                sqlText += "@MasterCreatedOn,";
                sqlText += "@MasterLastModifiedBy,";
                sqlText += "@MasterLastModifiedOn,";
                sqlText += "@MasterSaleType,";
                sqlText += "@MasterPreviousSalesInvoiceNo,";
                sqlText += "@MasterTrading,";
                sqlText += "@MasterIsPrint,";
                sqlText += "@MasterTenderId,";
                sqlText += "@MasterTransactionType,";
                sqlText += "@MasterDeliveryDate,";
                sqlText += "@MasterReturnId,";
                sqlText += "@MasterCurrencyID,";
                sqlText += "@commonDaldecimalMasterCurrencyRateFromBDT,";
                sqlText += "@MasterImportID,";
                sqlText += "@MasterPost";
                sqlText += ")";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@newID", newID);
                cmdInsert.Parameters.AddWithValue("@MasterCustomerID", Master.CustomerID);
                cmdInsert.Parameters.AddWithValue("@MasterDeliveryAddress1", Master.DeliveryAddress1);
                cmdInsert.Parameters.AddWithValue("@MasterDeliveryAddress2", Master.DeliveryAddress2);
                cmdInsert.Parameters.AddWithValue("@MasterDeliveryAddress3", Master.DeliveryAddress3);
                cmdInsert.Parameters.AddWithValue("@vehicleId", vehicleId);
                cmdInsert.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime);
                cmdInsert.Parameters.AddWithValue("@commonDaldecimalMasterTotalAmount", commonDal.decimal259(Master.TotalAmount));
                cmdInsert.Parameters.AddWithValue("@commonDaldecimalMasterTotalVATAmount", commonDal.decimal259(Master.TotalVATAmount));
                cmdInsert.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo);
                cmdInsert.Parameters.AddWithValue("@MasterComments", Master.Comments);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@MasterSaleType", Master.SaleType);
                cmdInsert.Parameters.AddWithValue("@MasterPreviousSalesInvoiceNo", Master.PreviousSalesInvoiceNo);
                cmdInsert.Parameters.AddWithValue("@MasterTrading", Master.Trading);
                cmdInsert.Parameters.AddWithValue("@MasterIsPrint", Master.IsPrint);
                cmdInsert.Parameters.AddWithValue("@MasterTenderId", Master.TenderId);
                cmdInsert.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValue("@MasterDeliveryDate", Master.DeliveryDate);
                cmdInsert.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId);
                cmdInsert.Parameters.AddWithValue("@MasterCurrencyID", Master.CurrencyID);
                cmdInsert.Parameters.AddWithValue("@commonDaldecimalMasterCurrencyRateFromBDT", commonDal.decimal259(Master.CurrencyRateFromBDT));
                cmdInsert.Parameters.AddWithValue("@MasterImportID", Master.ImportID);
                cmdInsert.Parameters.AddWithValue("@MasterPost", Master.Post);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }


                #endregion ID generated completed,Insert new Information in Header
                #region if Transection not Other Insert Issue /Receive


                #region Sale For InternalIssue or TollIssue or Trading

                if (Master.TransactionType == "Service"
                    || Master.TransactionType == "ExportService"
                    || Master.TransactionType == "InternalIssue"
                    || Master.TransactionType == "TradingTender"
                    || Master.TransactionType == "ExportTrading"
                    || Master.TransactionType == "ExportTradingTender"
                    || Master.TransactionType == "Trading")
                {
                    #region Insert to Issue Header

                    sqlText = "";
                    sqlText += " insert into IssueHeaders(";
                    //sqlText += " IssueNo,";

                    sqlText += " IssueNo,";
                    sqlText += " IssueDateTime,";
                    sqlText += " TotalVATAmount,";
                    sqlText += " TotalAmount,";
                    sqlText += " SerialNo,";
                    sqlText += " Comments,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " ReceiveNo,";
                    sqlText += " transactionType,";
                    sqlText += " IssueReturnId,";
                    sqlText += " Post";
                    sqlText += " )";
                    sqlText += " values(";
                    //sqlText += "'" + Master.Id + "',";

                    sqlText += "@newID,";
                    sqlText += "@MasterInvoiceDateTime,";
                    sqlText += " 0,";
                    sqlText += "0,";
                    sqlText += "@MasterSalesInvoiceNo,";
                    sqlText += "@MasterComments,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@MasterSalesInvoiceNo,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@MasterReturnId,";
                    sqlText += "@MasterPost";
                    sqlText += ")	";

                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                    cmdInsertIssue.Transaction = transaction;
                    cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterComments", Master.Comments);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post);

                    transResult = (int)cmdInsertIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {

                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                    }

                    #endregion Insert to Issue Header

                    #region Insert to Receive

                    sqlText = "";
                    sqlText += " insert into ReceiveHeaders(";
                    //sqlText += " IssueNo,";
                    sqlText += " ReceiveNo,";
                    sqlText += " ReceiveDateTime,";
                    sqlText += " TotalAmount,";
                    sqlText += " TotalVATAmount,";
                    sqlText += " SerialNo,";
                    sqlText += " Comments,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " transactionType,";
                    sqlText += " ReceiveReturnId,";
                    sqlText += " Post";
                    sqlText += " )";
                    sqlText += " values(";
                    sqlText += "@newID,";
                    sqlText += "@MasterInvoiceDateTime,";
                    sqlText += " 0,";
                    sqlText += "0,";
                    sqlText += "@newID,";
                    sqlText += "@MasterComments,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@MasterReturnId,";
                    sqlText += "@MasterPost";
                    sqlText += ")	";

                    SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                    cmdInsertReceive.Transaction = transaction;
                    cmdInsertReceive.Parameters.AddWithValue("@newID", newID ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime);
                    cmdInsertReceive.Parameters.AddWithValue("@newID", newID ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                    transResult = (int)cmdInsertReceive.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveReceive);
                    }

                    #endregion Insert to Receive Header

                }


                #endregion Purchase ID Create For IssueReturn
                #region TollIssue
                else if (Master.TransactionType == "TollIssue")
                {
                    #region Insert to Issue Header

                    sqlText = "";
                    sqlText += " insert into IssueHeaders(";
                    //sqlText += " IssueNo,";

                    sqlText += " IssueNo,";             
                    sqlText += " IssueDateTime,";
                    sqlText += " TotalVATAmount,";
                    sqlText += " TotalAmount,";
                    sqlText += " SerialNo,";
                    sqlText += " Comments,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " ReceiveNo,";
                    sqlText += " transactionType,";
                    sqlText += " IssueReturnId,";
                    sqlText += " Post";
                    sqlText += " )";
                    sqlText += " values(";
                    //sqlText += "'" + Master.Id + "',";
                     
                    sqlText += "@newID,";                     
                    sqlText += "@MasterInvoiceDateTime,";
                    sqlText += " 0,";
                    sqlText += "0,";
                    sqlText += "@MasterSalesInvoiceNo,";
                    sqlText += "@MasterComments,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@MasterSalesInvoiceNo,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@MasterReturnId,";
                    sqlText += "@MasterPost";
                    sqlText += ")	";

                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                    cmdInsertIssue.Transaction = transaction;
                    cmdInsertIssue.Parameters.AddWithValue("@newID", newID ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime ", Master.InvoiceDateTime);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                    transResult = (int)cmdInsertIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {

                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                    }

                    #endregion Insert to Issue Header

                }
                #endregion TollIssue


                #endregion if Transection not Other Insert Issue /Receive

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find AVG Rate
                    ProductDAL productDal = new ProductDAL();
                    //decimal AvgRate = productDal.AvgPrice(Item.ItemNo, Master.InvoiceDateTime, currConn, transaction);
                    decimal AvgRate = 0;
                    DataTable priceData = productDal.AvgPriceNew(Item.ItemNo, Master.InvoiceDateTime, currConn, transaction,false);
                    decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                    decimal quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                    if (quantity > 0)
                    {
                        AvgRate = amount / quantity;
                    }
                    else
                    {
                        AvgRate = 0;
                    }


                    #endregion Find AVG Rate


                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(SalesInvoiceNo) from SalesInvoiceDetails WHERE SalesInvoiceNo=@newID ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@newID", newID);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert Issue and Receive if Transaction is not Other

                    #region Transaction is Service


                    #endregion Transaction is Service
                    #region Transaction is Trading

                    if (Master.TransactionType == "Trading"
                        || Master.TransactionType == "ExportTradingTender"
                        || Master.TransactionType == "TradingTender"
                        || Master.TransactionType == "ExportTrading"
                        || Master.TransactionType == "Service"
                        || Master.TransactionType == "ExportService"
                        )
                    {
                        #region Insert to Issue

                        sqlText = "";
                        sqlText += " insert into IssueDetails(";
                        sqlText += " IssueNo,";
                        sqlText += " IssueLineNo,";
                        sqlText += " ItemNo,";
                        sqlText += " Quantity,";
                        sqlText += " NBRPrice,";
                        sqlText += " CostPrice,";
                        sqlText += " UOM,";
                        sqlText += " VATRate,";
                        sqlText += " VATAmount,";
                        sqlText += " SubTotal,";
                        sqlText += " Comments,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " ReceiveNo,";
                        sqlText += " IssueDateTime,";
                        sqlText += " SD,";
                        sqlText += " SDAmount,";
                        sqlText += " Wastage,";
                        sqlText += " BOMDate,";
                        sqlText += " FinishItemNo,";
                        sqlText += " TransactionType,";
                        sqlText += " IssueReturnId,";
                        sqlText += " DiscountAmount,";
                        sqlText += " DiscountedNBRPrice,";

                        sqlText += " UOMQty,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMc,";
                        sqlText += " UOMn,";

                        sqlText += " Post";

                        //sqlText += " Post";
                        sqlText += " )";
                        sqlText += " values(	";
                        //sqlText += "'" + Item.Id + "',";
                        sqlText += "@newID,";

                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@commonDaldecimalItemQuantity,";
                        sqlText += " 0,";
                        sqlText += "@commonDal.decimalAvgRate,	";
                        sqlText += "@ItemUOM ,";
                        sqlText += " 0,	0,	";
                        sqlText += "@commonDaldecimalAvgRate * @commonDaldecimalItemQuantity ,";
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@newID,";
                        sqlText += "@MasterInvoiceDateTime,";
                        sqlText += " 0,	0,";
                        sqlText += " 0,	0,	0,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@commonDaldecimalItemDiscountAmount,";
                        sqlText += "@commonDaldecimalItemDiscountedNBRPrice,";
                                    
                        sqlText += "@commonDaldecimalItemUOMQty,";
                        sqlText += "@commonDaldecimalItemUOMPrice,";
                        sqlText += "@commonDaldecimalItemUOMc,";
                        sqlText += "@commonDaldecimalItemUOMn,";

                        sqlText += "@MasterPost";

                        //sqlText += "'" + Master.@Post + "'";
                        sqlText += ")	";
                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue.Transaction = transaction;
                        cmdInsertIssue.Parameters.AddWithValue("@newID",newID);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo",Item.InvoiceLineNo);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo",Item.ItemNo);
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItem.Quantity",commonDal.decimal259(Item.Quantity));
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalAvgRate",commonDal.decimal259(AvgRate));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOM",Item.UOM );
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalAvgRate",commonDal.decimal259(AvgRate));
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItemQuantity",commonDal.decimal259(Item.Quantity) );
                        cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD",Item.CommentsD);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy",Master.CreatedBy);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn",Master.CreatedOn);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy",Master.LastModifiedBy);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn",Master.LastModifiedOn);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime",Master.InvoiceDateTime);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType",Master.TransactionType);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId",Master.ReturnId);
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItemDiscountAmount",commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItemDiscountedNBRPrice",commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItemUOMQty",commonDal.decimal259(Item.UOMQty));
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItemUOMPrice",commonDal.decimal259(Item.UOMPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItemUOMc",commonDal.decimal259(Item.UOMc));
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItemUOMn",commonDal.decimal259(Item.UOMn));
                        cmdInsertIssue.Parameters.AddWithValue("@MasterPost",Master.Post);

                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                        }
                        #endregion Insert to Issue
                        #region Update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                        sqlText += " where (IssueHeaders.IssueNo=@newID)";

                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;
                        cmdUpdateIssue.Parameters.AddWithValue("@newID", newID);

                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                        }
                        #endregion Update Issue

                        #region Insert to Receive

                        sqlText = "";
                        sqlText += " insert into ReceiveDetails(";
                        //sqlText += " IssueNo,";
                        sqlText += " ReceiveNo,";
                        sqlText += " ReceiveLineNo,";
                        sqlText += " ItemNo,";
                        sqlText += " Quantity,";
                        sqlText += " CostPrice,";
                        sqlText += " NBRPrice,";
                        sqlText += " UOM,";
                        sqlText += " VATRate,";
                        sqlText += " VATAmount,";
                        sqlText += " SubTotal,";
                        sqlText += " Comments,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " SD,";
                        sqlText += " SDAmount,";
                        sqlText += " ReceiveDateTime,";
                        sqlText += " TransactionType,";
                        sqlText += " ReceiveReturnId,";
                        sqlText += " DiscountAmount,";
                        sqlText += " DiscountedNBRPrice,";

                        sqlText += " UOMQty,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMc,";
                        sqlText += " UOMn,";
                        sqlText += " Post";

                        //sqlText += " Post";
                        sqlText += " )";
                        sqlText += " values(	";
                        //sqlText += "'" + Item.Id + "',";
                        sqlText += "@newID,";
                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@commonDaldecimalItemQuantity,";
                        sqlText += "@commonDaldecimalItemNBRPrice,";
                        sqlText += "@commonDaldecimalItemNBRPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,";
                        sqlText += "@SubTotal,";
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += " 0,0,";
                        sqlText += "@MasterInvoiceDateTime,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@commonDaldecimalItemDiscountAmount,";
                        sqlText += "@commonDaldecimalItemDiscountedNBRPrice,";
                        sqlText += "@commonDaldecimalItemUOMQty,";
                        sqlText += "@commonDaldecimalItemUOMPrice,";
                        sqlText += "@commonDaldecimalItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";

                        //sqlText += "'" + Master.@Post + "'";
                        sqlText += ")	";
                        SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                        cmdInsertReceive.Transaction = transaction;
                        cmdInsertReceive.Parameters.AddWithValue("@newID", newID ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemQuantity",commonDal.decimal259(Item.Quantity));
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemNBRPrice",commonDal.decimal259(Item.NBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemNBRPrice",commonDal.decimal259(Item.NBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@SubTotal",commonDal.decimal259(Item.NBRPrice + "*" + Item.Quantity));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn",Master.CreatedOn);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn",Master.LastModifiedOn);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime",Master.InvoiceDateTime);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId",Master.ReturnId);
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemDiscountAmount",commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemDiscountedNBRPrice",commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemUOMQty",commonDal.decimal259(Item.UOMQty));
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemUOMPrice",commonDal.decimal259(Item.UOMPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemUOMc",commonDal.decimal259(Item.UOMc));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                        transResult = (int)cmdInsertReceive.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveReceive);
                        }
                        #endregion Insert to Receive
                        #region Update Receive

                        sqlText = "";


                        sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                        sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                        sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                        sqlText += " where ReceiveHeaders.ReceiveNo=@newID";


                        SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                        cmdUpdateReceive.Transaction = transaction;
                        cmdUpdateReceive.Parameters.AddWithValue("@newID", newID);

                        int UpdateReceive = (int)cmdUpdateReceive.ExecuteNonQuery();

                        if (UpdateReceive <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveReceive);
                        }
                        #endregion Update Receive


                    }
                    #endregion Transaction is Trading

                    #region Transaction is TollIssue

                    if (Master.TransactionType == "TollIssue")
                    {
                        #region Insert to Issue

                        sqlText = "";
                        sqlText += " insert into IssueDetails(";
                        sqlText += " IssueNo,";
                        sqlText += " IssueLineNo,";
                        sqlText += " ItemNo,";
                        sqlText += " Quantity,";
                        sqlText += " NBRPrice,";
                        sqlText += " CostPrice,";
                        sqlText += " UOM,";
                        sqlText += " VATRate,";
                        sqlText += " VATAmount,";
                        sqlText += " SubTotal,";
                        sqlText += " Comments,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " ReceiveNo,";
                        sqlText += " IssueDateTime,";
                        sqlText += " SD,";
                        sqlText += " SDAmount,";
                        sqlText += " Wastage,";
                        sqlText += " BOMDate,";
                        sqlText += " FinishItemNo,";
                        sqlText += " TransactionType,";
                        sqlText += " IssueReturnId,";
                        sqlText += " DiscountAmount,";
                        sqlText += " DiscountedNBRPrice,";
                        sqlText += " UOMQty,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMc,";
                        sqlText += " UOMn,";
                        sqlText += " Post";

                        //sqlText += " Post";
                        sqlText += " )";
                        sqlText += " values(	";
                        //sqlText += "'" + Item.Id + "',";
                        sqlText += "@newID,";

                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@commonDaldecimal259(Item.Quantity),";
                        sqlText += " 0,";
                        sqlText += " @commonDaldecimalAvgRate,";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,	";
                        sqlText += "@SubTotal,";
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@newID,";
                        sqlText += "@MasterInvoiceDateTime,";
                        sqlText += " 0,	0,";
                        sqlText += " 0,	0,	0,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@commonDaldecimalItemDiscountAmount),";
                        sqlText += "@commonDaldecimalItemDiscountedNBRPrice),";
                        sqlText += "@commonDaldecimalItemUOMQty),";
                        sqlText += "@commonDaldecimalItemUOMPrice),";
                        sqlText += "@commonDaldecimalItemUOMc),";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";

                        //sqlText += "'" + Master.@Post + "'";
                        sqlText += ")	";
                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue.Transaction = transaction;
                        cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItemQuantity", commonDal.decimal259(Item.Quantity));
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalAvgRate", commonDal.decimal259(AvgRate));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM);
                        cmdInsertIssue.Parameters.AddWithValue("@SubTotal", commonDal.decimal259(AvgRate) + "*" + commonDal.decimal259(Item.Quantity));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId);
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItemDiscountAmount)", commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItemDiscountedNBRPrice)", commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItemUOMQty)", commonDal.decimal259(Item.UOMQty));
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItemUOMPrice)", commonDal.decimal259(Item.UOMPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@commonDaldecimalItemUOMc)", commonDal.decimal259(Item.UOMc));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post);

                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                        }
                        #endregion Insert to Issue
                        #region Update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                        sqlText += " where (IssueHeaders.IssueNo=@newID )";

                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;
                        cmdUpdateIssue.Parameters.AddWithValue("@newID", newID);

                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                        }
                        #endregion Update Issue


                    }
                    #endregion Transaction is TollIssue

                    #region Transaction is InternalIssue

                    if (Master.TransactionType == "InternalIssue")
                    {
                        //ProductDAL productDal = new ProductDAL();

                        decimal NBRPrice = productDal.GetLastNBRPriceFromBOM(Item.ItemNo, "VAT 1 (Internal Issue)", Master.InvoiceDateTime, currConn, transaction);
                        #region Insert to Issue

                        sqlText = "";
                        sqlText += " insert into IssueDetails(";
                        sqlText += " IssueNo,";
                        sqlText += " IssueLineNo,";
                        sqlText += " ItemNo,";
                        sqlText += " Quantity,";
                        sqlText += " NBRPrice,";
                        sqlText += " CostPrice,";
                        sqlText += " UOM,";
                        sqlText += " VATRate,";
                        sqlText += " VATAmount,";
                        sqlText += " SubTotal,";
                        sqlText += " Comments,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " ReceiveNo,";
                        sqlText += " IssueDateTime,";
                        sqlText += " SD,";
                        sqlText += " SDAmount,";
                        sqlText += " Wastage,";
                        sqlText += " BOMDate,";
                        sqlText += " FinishItemNo,";
                        sqlText += " TransactionType,";
                        sqlText += " IssueReturnId,";
                        sqlText += " DiscountAmount,";
                        sqlText += " DiscountedNBRPrice,";
                        sqlText += " UOMQty,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMc,";
                        sqlText += " UOMn,";
                        sqlText += " Post";

                        //sqlText += " Post";
                        sqlText += " )";
                        sqlText += " values(	";
                        //sqlText += "'" + Item.Id + "',";
                        sqlText += "@newID,";
                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@commonDaldecimalItemQuantity,";
                        sqlText += " 0,";
                        sqlText += "@commonDaldecimalAvgRate,	";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,	";
                        sqlText += "@commonDal.decimalAvgRate * @commonDaldecimalItemQuantity,";
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@newID,";
                        sqlText += "@MasterInvoiceDateTime,";
                        sqlText += " 0,	0,";
                        sqlText += " 0,	0,	0,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@commonDaldecimalItemDiscountAmount,";
                        sqlText += "@commonDaldecimalItemDiscountedNBRPrice,";
                                    
                        sqlText += "@commonDaldecimalItemUOMQty),";
                        sqlText += "@commonDaldecimalItemUOMPrice),";
                        sqlText += "@commonDaldecimalItemUOMc),";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";

                        //sqlText += "'" + Master.@Post + "'";
                        sqlText += ")	";
                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue.Transaction = transaction;
                        cmdInsert.Parameters.AddWithValue("@newID", newID);
                        cmdInsert.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@commonDaldecimalItemQuantity", commonDal.decimal259(Item.Quantity));
                        cmdInsert.Parameters.AddWithValue("@commonDaldecimalAvgRate", commonDal.decimal259(AvgRate));
                        cmdInsert.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@commonDaldecimalAvgRate", commonDal.decimal259(AvgRate));
                        cmdInsert.Parameters.AddWithValue("@commonDaldecimalItemQuantity", commonDal.decimal259(Item.Quantity));
                        cmdInsert.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD);
                        cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsert.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsert.Parameters.AddWithValue("@newID", newID ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime);
                        cmdInsert.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@commonDaldecimalItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                        cmdInsert.Parameters.AddWithValue("@commonDaldecimalItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsert.Parameters.AddWithValue("@commonDaldecimalItemUOMQty", commonDal.decimal259(Item.UOMQty));
                        cmdInsert.Parameters.AddWithValue("@commonDaldecimalItemUOMPrice", commonDal.decimal259(Item.UOMPrice));
                        cmdInsert.Parameters.AddWithValue("@commonDaldecimalItemUOMc", commonDal.decimal259(Item.UOMc));
                        cmdInsert.Parameters.AddWithValue("@ItemUOMn", Item.UOMn);
                        cmdInsert.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                        }
                        #endregion Insert to Issue
                        #region Update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                        sqlText += " where (IssueHeaders.IssueNo=@newID)";

                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;
                        cmdUpdateIssue.Parameters.AddWithValue("@newID", newID);

                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                        }
                        #endregion Update Issue

                        #region Insert to Receive

                        sqlText = "";
                        sqlText += " insert into ReceiveDetails(";
                        //sqlText += " IssueNo,";
                        sqlText += " ReceiveNo,";
                        sqlText += " ReceiveLineNo,";
                        sqlText += " ItemNo,";
                        sqlText += " Quantity,";
                        sqlText += " CostPrice,";
                        sqlText += " NBRPrice,";
                        sqlText += " UOM,";
                        sqlText += " VATRate,";
                        sqlText += " VATAmount,";
                        sqlText += " SubTotal,";
                        sqlText += " Comments,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " SD,";
                        sqlText += " SDAmount,";
                        sqlText += " ReceiveDateTime,";
                        sqlText += " TransactionType,";
                        sqlText += " ReceiveReturnId,";
                        sqlText += " DiscountAmount,";
                        sqlText += " DiscountedNBRPrice,";
                        sqlText += " UOMQty,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMc,";
                        sqlText += " UOMn,";
                        sqlText += " Post";

                        //sqlText += " Post";
                        sqlText += " )";
                        sqlText += " values(	";
                        //sqlText += "'" + Item.Id + "',";
                        sqlText += "@newID,";
                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@commonDaldecimalItemQuantity,";
                        sqlText += "@commonDaldecimalNBRPrice,";
                        sqlText += "@commonDaldecimalNBRPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,";
                        sqlText += "@SubTotal,";
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += " 0,0,";
                        sqlText += "@MasterInvoiceDateTime,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@commonDaldecimalItemDiscountAmount,";
                        sqlText += "@commonDaldecimalItemDiscountedNBRPrice,";
                        sqlText += "@commonDaldecimalItemUOMQty,";
                        sqlText += "@commonDaldecimalItemUOMPrice,";
                        sqlText += "@commonDaldecimalItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";

                        //sqlText += "'" + Master.@Post + "'";
                        sqlText += ")	";
                        SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                        cmdInsertReceive.Transaction = transaction;
                        cmdInsertReceive.Parameters.AddWithValue("@newID",newID);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemInvoiceLineNo",Item.InvoiceLineNo);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo",Item.ItemNo);
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemQuantity)",commonDal.decimal259(Item.Quantity));
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalNBRPrice",commonDal.decimal259(NBRPrice));
                        //cmdInsertReceive.Parameters.AddWithValue("@commonDal.decimal259(NBRPrice)",commonDal.decimal259(NBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOM",Item.UOM);
                        cmdInsertReceive.Parameters.AddWithValue("@SubTotal",commonDal.decimal259(NBRPrice + "*" + Item.Quantity));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemCommentsD",Item.CommentsD);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy",Master.CreatedBy);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn",Master.CreatedOn);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy",Master.LastModifiedBy);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn",Master.LastModifiedOn);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime",Master.InvoiceDateTime);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType",Master.TransactionType);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId",Master.ReturnId);
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemDiscountAmount",commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemDiscountedNBRPrice",commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemUOMQty",commonDal.decimal259(Item.UOMQty));
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemUOMPrice",commonDal.decimal259(Item.UOMPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@commonDaldecimalItemUOMc",commonDal.decimal259(Item.UOMc));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMn",Item.UOMn);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterPost",Master.Post);

                        transResult = (int)cmdInsertReceive.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveReceive);
                        }
                        #endregion Insert to Receive
                        #region Update Receive

                        sqlText = "";


                        sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                        sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                        sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                        sqlText += " where ReceiveHeaders.ReceiveNo=@newID ";


                        SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                        cmdUpdateReceive.Transaction = transaction;
                        cmdUpdateReceive.Parameters.AddWithValue("@newID", newID);

                        transResult = (int)cmdUpdateReceive.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveReceive);
                        }
                        #endregion Update Receive



                    }
                    #endregion Transaction is InternalIssue

                    #endregion Insert Issue and Receive if Transaction is not Other
                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into SalesInvoiceDetails(";
                    sqlText += " SalesInvoiceNo,";
                    sqlText += " InvoiceLineNo,";
                    sqlText += " ItemNo,";
                    sqlText += " Quantity,";
                    sqlText += " PromotionalQuantity,";
                    sqlText += " SalesPrice,";
                    sqlText += " NBRPrice,";
                    sqlText += " AVGPrice,";
                    sqlText += " UOM,";
                    sqlText += " VATRate,";
                    sqlText += " VATAmount,";
                    sqlText += " SubTotal,";
                    sqlText += " Comments,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " SD,";
                    sqlText += " SDAmount,";
                    sqlText += " SaleType,";
                    sqlText += " PreviousSalesInvoiceNo,";
                    sqlText += " Trading,";
                    sqlText += " NonStock,";
                    sqlText += " TradingMarkUp,";
                    sqlText += " InvoiceDateTime,";
                    sqlText += " Type,";
                    sqlText += " TransactionType,";
                    sqlText += " SaleReturnId,";
                    sqlText += " Post,";
                    sqlText += " UOMQty,";
                    sqlText += " UOMn,";
                    sqlText += " UOMc,";
                    sqlText += " DiscountAmount,";
                    sqlText += " DiscountedNBRPrice,";
                    sqlText += " DollerValue,";
                    sqlText += " CurrencyValue,";
                    sqlText += " UOMPrice";
                    sqlText += " )";
                    sqlText += " values(	";

                    sqlText += "@newID,";
                    sqlText += "@ItemInvoiceLineNo, ";
                    sqlText += "@Item14ItemNo,";
                    sqlText += "@commonDaldecimalItemQuantity,";
                    sqlText += "@commonDaldecimalItemPromotionalQuantity,";
                    sqlText += "@commonDaldecimalItemSalesPrice,";
                    sqlText += "@commonDaldecimalItemNBRPrice,";
                    sqlText += "@commonDaldecimalAvgRate,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@commonDaldecimalItemVATRate,";
                    sqlText += "@commonDaldecimalItemVATAmount,";
                    sqlText += "@commonDaldecimalItemSubTotal,";
                    sqlText += "@ItemCommentsD,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@commonDaldecimalItemSD,";
                    sqlText += "@commonDaldecimalItemSDAmount,";
                    sqlText += "@ItemSaleTypeD,";
                    sqlText += "@ItemPreviousSalesInvoiceNoD,";
                    sqlText += "@ItemTradingD,";
                    sqlText += "@ItemNonStockD,";
                    sqlText += "@ItemTradingMarkUp,";
                    sqlText += "@MasterInvoiceDateTime,";
                    sqlText += "@ItemType,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@MasterReturnId,";
                    sqlText += "@MasterPost,";
                    sqlText += "@commonDaldecimalItemUOMQty,";
                    sqlText += "@ItemUOMn,";
                    sqlText += "@commonDaldecimalItemUOMc),";
                    sqlText += "@commonDaldecimalItemDiscountAmount),";
                    sqlText += "@commonDaldecimalItemDiscountedNBRPrice),";
                    sqlText += "@commonDaldecimalItemDollerValue),";
                    sqlText += "@commonDaldecimalItemCurrencyValue),";
                    sqlText += "@commonDaldecimalItemUOMPrice) ";
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsDetail.Parameters.AddWithValue("@newID",newID);
                    cmdInsDetail.Parameters.AddWithValue("@ItemInvoiceLineNo",Item.InvoiceLineNo?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemItemNo",Item.ItemNo?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemQuantity",commonDal.decimal259(Item.Quantity));
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemPromotionalQuantity",commonDal.decimal259(Item.PromotionalQuantity));
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemSalesPrice",commonDal.decimal259(Item.SalesPrice));
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemNBRPrice",commonDal.decimal259(Item.NBRPrice));
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalAvgRate",commonDal.decimal259(AvgRate));
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOM",Item.UOM?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemVATRate",commonDal.decimal259(Item.VATRate));
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemVATAmount",commonDal.decimal259(Item.VATAmount));
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemSubTotal)",commonDal.decimal259(Item.SubTotal));
                    cmdInsDetail.Parameters.AddWithValue("@ItemCommentsD",Item.CommentsD);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy",Master.CreatedBy?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn",Master.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy",Master.LastModifiedBy?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn",Master.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemSD)",commonDal.decimal259(Item.SD));
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemSDAmount)",commonDal.decimal259(Item.SDAmount));
                    cmdInsDetail.Parameters.AddWithValue("@ItemSaleTypeD",Item.SaleTypeD?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPreviousSalesInvoiceNoD",Item.PreviousSalesInvoiceNoD?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemTradingD",Item.TradingD?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemNonStockD",Item.NonStockD?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemTradingMarkUp",Item.TradingMarkUp);
                    cmdInsDetail.Parameters.AddWithValue("@MasterInvoiceDateTime",Master.InvoiceDateTime);
                    cmdInsDetail.Parameters.AddWithValue("@ItemType",Item.Type?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType",Master.TransactionType?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterReturnId",Master.ReturnId?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterPost",Master.Post?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimal259(Item.UOMQty)",commonDal.decimal259(Item.UOMQty));
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMn",Item.UOMn);
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemUOMc",commonDal.decimal259(Item.UOMc));
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemDiscountAmount",commonDal.decimal259(Item.DiscountAmount));
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemDiscountedNBRPrice",commonDal.decimal259(Item.DiscountedNBRPrice));
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemDollerValue",commonDal.decimal259(Item.DollerValue));
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemCurrencyValue",commonDal.decimal259(Item.CurrencyValue));
                    cmdInsDetail.Parameters.AddWithValue("@commonDaldecimalItemUOMPrice",commonDal.decimal259(Item.UOMPrice));

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable
                }



                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)


                #region Insert into Export
                if (Master.TransactionType == "Export"
                    || Master.TransactionType == "ExportTender"
                    || Master.TransactionType == "ExportTrading"
                    || Master.TransactionType == "ExportServiceNS"
                    || Master.TransactionType == "ExportServiceNSCredit"
           
               || Master.TransactionType == "ExportService"
                    || Master.TransactionType == "ExportTradingTender"
                    || Master.TransactionType == "ExportPackage"

                    )
                {
                    #region Validation for Export

                    if (ExportDetails.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                    }

                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "delete from SalesInvoiceHeadersExport where SalesInvoiceNo=@newID";


                    SqlCommand cmdFindId1 = new SqlCommand(sqlText, currConn);
                    cmdFindId1.Transaction = transaction;
                    cmdFindId1.Parameters.AddWithValue("@newID", newID);

                    cmdFindId1.ExecuteNonQuery();

                    #endregion Find Transaction Exist

                    #endregion Validation for Export

                    foreach (var ItemExport in ExportDetails.ToList())
                    {
                        #region Insert only DetailTable

                        sqlText = "";
                        sqlText += " insert into SalesInvoiceHeadersExport(";
                        sqlText += " SalesInvoiceNo,";
                        sqlText += " SaleLineNo,";
                        sqlText += " RefNo,";
                        sqlText += " Description,";
                        sqlText += " Quantity,";
                        sqlText += " GrossWeight,";
                        sqlText += " NetWeight,";
                        sqlText += " NumberFrom,";
                        sqlText += " NumberTo,";
                        sqlText += " Comments,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn";
                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@newID,";
                        sqlText += "@ItemExportSaleLineNo ,";
                        sqlText += "@ItemExportRefNo ,";
                        sqlText += "@ItemExportDescription ,";
                        sqlText += "@ItemExportQuantityE ,";
                        sqlText += "@ItemExportGrossWeight ,";
                        sqlText += "@ItemExportNetWeight ,";
                        sqlText += "@ItemExportNumberFrom ,";
                        sqlText += "@ItemExportNumberTo ,";
                        sqlText += "@ItemExportCommentsE ,";
                        sqlText += "@MasterCreatedBy ,";
                        sqlText += "@MasterCreatedOn ,";
                        sqlText += "@MasterLastModifiedBy ,";
                        sqlText += "@MasterLastModifiedOn";

                        //sqlText += "'" + Master.@Post + "'";
                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportSaleLineNo",ItemExport.SaleLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportRefNo",ItemExport.RefNo?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportDescription",ItemExport.Description?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportQuantityE",ItemExport.QuantityE?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportGrossWeight",ItemExport.GrossWeight?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportNetWeight",ItemExport.NetWeight?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportNumberFrom",ItemExport.NumberFrom ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportNumberTo",ItemExport.NumberTo?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportCommentsE",ItemExport.CommentsE?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy",Master.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn",Master.CreatedOn );
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy",Master.LastModifiedBy?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn",Master.LastModifiedOn?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                            MessageVM.saleMsgSaveNotSuccessfully);
                        }

                        #endregion Insert only DetailTable
                    }
                }

                #endregion Insert into Export


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from SalesInvoiceHeaders WHERE SalesInvoiceNo@newID ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@newID", newID);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableCreatID);
                }


                #endregion Prefetch
                #region Commit
                if (vcurrConn == null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

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
                if (vcurrConn == null)
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

        public string[] ImportInspectionData(DataTable dtSaleM, string noOfSale)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SaleMasterVM saleMaster = new SaleMasterVM();
            List<BureauSaleDetailVM> saleDetails = new List<BureauSaleDetailVM>();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion variable

            #region try
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    currConn.Open();
                    transaction = currConn.BeginTransaction("Checking Data");
                }


                #region RowCount
                int MRowCount = 0;
                int MRow = dtSaleM.Rows.Count;
                for (int i = 0; i < dtSaleM.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtSaleM.Rows[i]["CLIENT"].ToString()))
                    {
                        MRowCount++;
                    }

                }
                if (MRow != MRowCount)
                {
                    string msg = "you have select " + MRow.ToString() + " data for import, but you have " + MRowCount + " id.";
                    throw new ArgumentNullException(msg);
                }
                #endregion RowCount
                CommonDAL cmnDal = new CommonDAL();
                string cCode = cmnDal.settingValue("CompanyCode", "Code");
                CommonImport cImport = new CommonImport();
                // for currency conversion
                string challanDate = BureauInfoVM.SessionDate;
                #region checking from database is exist the information(NULL Check)
               
                string CurrencyId = string.Empty;
                string USDCurrencyId = string.Empty;

                for (int i = 0; i < MRowCount; i++)
                {
                    CurrencyId = string.Empty;
                    USDCurrencyId = string.Empty;
                    #region Master
                    #region FindCustomerId
                    cImport.FindCustomerId(dtSaleM.Rows[i]["CLIENT"].ToString().Trim(),
                                           dtSaleM.Rows[i]["ADDRESS CODE"].ToString().Trim(), currConn, transaction);
                    #endregion FindCustomerId

                    #region FindCurrencyId
                    CurrencyId = cImport.FindCurrencyId("BDT", currConn, transaction);
                    USDCurrencyId = cImport.FindCurrencyId("USD", currConn, transaction);
                    //cImport.FindCurrencyRateBDTtoUSD(USDCurrencyId, currConn, transaction);

                    #endregion FindCurrencyId

                    #region Checking Date is null or different formate
                    bool IsInvoiceDate;
                    //if (!string.IsNullOrEmpty(dtSaleM.Rows[i]["INVOICE DATE"].ToString()))
                    //{
                    //    IsInvoiceDate = cImport.CheckBureauDate(dtSaleM.Rows[i]["INVOICE DATE"].ToString().Trim());
                    //    if (IsInvoiceDate != true)
                    //    {
                    //        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Invoice_Date_Time field.");
                    //    }
                    //}
                    if (!string.IsNullOrEmpty(dtSaleM.Rows[i]["MONTH"].ToString()))
                    {
                        IsInvoiceDate = cImport.CheckNumericBool(dtSaleM.Rows[i]["MONTH"].ToString().Trim());
                        if (IsInvoiceDate != true)
                        {
                            throw new ArgumentNullException("Please insert correct month no. in 'MONTH' field.");
                        }
                    }
                    

                    #endregion Checking Date is null or different formate

                    #region Check invoice id
                    string InvoiceNo = string.Empty;

                    InvoiceNo = cImport.CheckCellValue(dtSaleM.Rows[i]["INVOICE NO"].ToString().Trim());
                    if (InvoiceNo=="N")
                    {
                        throw new ArgumentNullException("Please insert invoice no. in 'Invoice No.'"); 
                    }
                    #endregion Check invoice id

                    #region Check Numeric Value

                    bool IsAmount, IsVat;

                    //IsAmount = cImport.CheckNumericValue(dtSaleM.Rows[i]["INVOICE AMOUNT"].ToString().Trim());
                    //if (IsAmount == "N")
                    //{
                    //    throw new ArgumentNullException("Please insert decimal value in 'INVOICE AMOUNT' field.");
                    //}

                    IsAmount = cImport.CheckNumericBool(dtSaleM.Rows[i]["BDT"].ToString().Trim());
                    if (IsAmount != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in 'BDT' field.");
                    }
                    IsVat = cImport.CheckNumericBool(dtSaleM.Rows[i]["VAT"].ToString().Trim());
                    if (IsVat != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in 'VAT' field.");
                    }
                    #endregion Check Numeric Value
                    #endregion Master

                }
                

                #endregion checking from database is exist the information(NULL Check)
                string creditWithoutTran = "";
                #region  Database Connection

                if (currConn.State == ConnectionState.Open)
                {
                    transaction.Commit();
                    currConn.Close();
                    currConn.Open();
                    transaction = currConn.BeginTransaction("Import Data");
                }
                #endregion Database Connection
                #region New

                #region Find Company Code

               
                #endregion Find Company Code
                #region NewT

                DataRow[] dtCreditRow = dtSaleM.Select("[BDT] < " + 0);
                if (dtCreditRow.Length > 0)
                {
                    dtCredit = dtCreditRow.CopyToDataTable();
                }

                dtSaleM = dtSaleM.Select("[BDT] > " + 0).CopyToDataTable();
                dtSaleM.DefaultView.Sort = "BU";
                dtSaleM = dtSaleM.DefaultView.ToTable();

                DataTable dtBuIds = dtSaleM.DefaultView.ToTable(true, "BU");
                
                #endregion NewT

                int creditCount = 0;
                int countID = 0;
                #region Check in Database
                string InvoiceDate = "";
                string InvoiceToDate = "";
                if (!string.IsNullOrEmpty(dtSaleM.Rows[0]["Month"].ToString().Trim()))
                {
                    var lastDay = DateTime.DaysInMonth(DateTime.Now.Year, Convert.ToInt32(dtSaleM.Rows[0]["Month"].ToString().Trim()));
                    DateTime sessionDate = Convert.ToDateTime(BureauInfoVM.SessionDate);
                    InvoiceDate = sessionDate.Year.ToString() + "-" + dtSaleM.Rows[0]["Month"].ToString().Trim() + "-" + lastDay;
                    InvoiceDate = Convert.ToDateTime(InvoiceDate).ToString("yyyy-MM-dd");
                    
                    InvoiceToDate = Convert.ToDateTime(InvoiceDate).AddDays(1).ToString("yyyy-MM-dd");

                }
                else
                {
                    throw new ArgumentNullException("Please insert month.'");
                }

                //string sqlText = "";
                //sqlText += "SELECT COUNT( SalesInvoiceNo) from SalesInvoiceHeaders where  InvoiceDateTime >= '" + InvoiceDate + "' and InvoiceDateTime <= '"
                //                                                                           + InvoiceToDate + "'";

                //DataTable dataTable = new DataTable("BureauSales");
                //SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                //cmdIdExist.Transaction = transaction;

                //transResult = (int)cmdIdExist.ExecuteScalar();

                //if (transResult > 0)
                //{
                //    throw new ArgumentNullException("Import Sales", MessageVM.saleMsgFindExistID);
                //}
                challanDate = InvoiceDate;
                #endregion


                foreach (DataRow buItem in dtBuIds.Rows)
                {
                    DataTable dtIndivisualBuIds = null;
                    string bureauID = "";
                    
                    if (cCode == "246")
                    {
                        if (buItem["BU"].ToString() == "246105")
                        {
                            if (countID == 0)
                            {
                                bureauID = "'246105','246113'";
                                dtIndivisualBuIds = dtSaleM.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                                countID = 1;
                            }
                        }
                        else if (buItem["BU"].ToString() == "246107")
                        {
                            bureauID = "'246107'";
                            dtIndivisualBuIds = dtSaleM.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                        }
                        else if (buItem["BU"].ToString() == "246108")
                        {
                            bureauID = "'246108'";
                            dtIndivisualBuIds = dtSaleM.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                        }
                        else
                        {
                            if (buItem["BU"].ToString() == "246113")
                            {
                                if (countID == 0)
                                {
                                    bureauID = "'246113'";
                                    dtIndivisualBuIds = dtSaleM.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                                    countID = 1;
                                }
                                else
                                {
                                    goto FinalOuter;
                                }
                            }
                            else
                            {
                                goto FinalOuter;
                            }
                                
                        }
                    }
                    else if (cCode == "549")
                    {
                        if (buItem["BU"].ToString() == "549105")
                        {
                                bureauID = "'549105','549113'";
                                dtIndivisualBuIds = dtSaleM.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                                countID = 1;
                        }
                        else if (buItem["BU"].ToString() == "549107")
                        {
                            bureauID = "'549107'";
                            //dtIndivisualBuIds = dtSaleM.Select("[BU] ='" + buItem["BU"].ToString() + "'").CopyToDataTable();
                            dtIndivisualBuIds = dtSaleM.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                        }
                        else if (buItem["BU"].ToString() == "549108")
                        {
                            bureauID = "'549108'";
                            dtIndivisualBuIds = dtSaleM.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                        }
                        else
                        {
                            if (buItem["BU"].ToString() == "549113")
                            {
                                if (countID == 0)
                                {
                                    bureauID = "'549113'";
                                    dtIndivisualBuIds = dtSaleM.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                                    countID = 1;
                                }
                                else
                                {
                                    goto FinalOuter;
                                }
                            }
                            else
                            {
                                goto FinalOuter;
                            }
                        }
                    }

                   
                    DataTable dtAllInvoices = dtIndivisualBuIds;  /// All values repetaed invoice and customer
                    dtIndivisualBuIds.DefaultView.Sort = "CLIENT ASC";
                    dtIndivisualBuIds = dtIndivisualBuIds.DefaultView.ToTable(true);
                    DataTable dtCustomers = dtIndivisualBuIds.DefaultView.ToTable(true, "CLIENT");
                    
                    #region Retrive Customer informations

                    foreach (DataRow custRows in dtCustomers.Rows)
                    {
                        DataTable dtCustomerInfos = null;
                        if (dtIndivisualBuIds.Select("[CLIENT] ='" + custRows["CLIENT"].ToString() + "'").Length > 0)
                        {
                            dtCustomerInfos = dtIndivisualBuIds.Select("[CLIENT] ='" + custRows["CLIENT"].ToString() + "' and "
                                             + "[BU] in (" + bureauID + ")").CopyToDataTable();


                        }
                        else
                        {
                            if (creditCount == 0)
                            {
                                creditWithoutTran = dtCredit.Select("[CLIENT] ='" + custRows["CLIENT"].ToString() + "'")[0]["CLIENT"].ToString();
                            }
                            else
                            {
                                creditWithoutTran = creditWithoutTran + ";" + dtCredit.Select("[CLIENT] ='" + custRows["CLIENT"].ToString() + "'")[0]["CLIENT"].ToString();
                            }
                            creditCount++;
                            goto Outer;
                        }
                        //// Retrive Customer total distinct invoice no.
                        DataTable dtDistinctInvoices = dtCustomerInfos.DefaultView.ToTable(true, "INVOICE NO");

                        
                        #region Master Sale Common info
                        DataRow dr = dtCustomerInfos.Rows[0];
                        string customerName = dr["CLIENT"].ToString().Trim();
                        string customerCode = dr["ADDRESS CODE"].ToString().Trim();
                        #region FindCustomerId
                        string customerId = cImport.FindCustomerId(customerName, customerCode, currConn, transaction);
                        #endregion FindCustomerId
                        #region FindItemNo
                        string itemNo;
                        string sd = "0";
                        string uom = "NO";
                        try
                        {
                            string productName = dr["Product NAME"].ToString().Trim();
                            string productCode = dr["Product Code"].ToString().Trim();
                            itemNo = cImport.FindItemId(productName, productCode, currConn, transaction);
                        }
                        catch (Exception)
                        {
                            DataTable productInfo = cImport.GetProductInfo(currConn, transaction);
                            itemNo = productInfo.Rows[0]["ItemNo"].ToString();
                            sd = productInfo.Rows[0]["SD"].ToString();
                            uom = productInfo.Rows[0]["UOM"].ToString();


                        }


                        #endregion FindItemNo
                        string isPrint = "N";
                        string currencyCode = "USD";
                        var currencyid = cImport.FindCurrencyId(currencyCode, currConn, transaction);
                        string createdBy = dr["Created_By"].ToString().Trim();
                        string lastModifiedBy = dr["LastModified_By"].ToString().Trim();
                        string transactionType = dr["Transection_Type"].ToString().Trim();
                        string IsExport = dr["IsExport"].ToString().Trim();
                        string post = "N";
                        try
                        {
                            post = dr["Post"].ToString().Trim();
                        }
                        catch (Exception)
                        {
                            post = "N";
                        }
                        string buID = buItem["BU"].ToString();
                        string bureauType = SearchType(buID);
                        #endregion Master Sale Common info

                        #region Detail Sale
                        int totalCounter = 1;
                        int lineCounter = 1;
                        decimal totalAmount = 0;
                        decimal totalVatAmount = 0;

                        //saleDetails = new List<BureauSaleDetailVM>();
                        foreach (DataRow detailRow in dtDistinctInvoices.Rows)
                        {
                            if (lineCounter == 1)
                            {
                                #region Set Master Value

                                saleMaster = new SaleMasterVM();
                                saleMaster.CustomerID = customerId;
                                saleMaster.InvoiceDateTime = challanDate;
                                saleMaster.DeliveryDate = challanDate;
                                saleMaster.SaleType = "New";
                               
                                saleMaster.CreatedBy = createdBy;
                                saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                saleMaster.LastModifiedBy = lastModifiedBy;
                                saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                saleMaster.IsPrint = isPrint;
                                saleMaster.TransactionType = transactionType;
                                saleMaster.Post = post;
                                //var currencyid = cImport.FindCurrencyId(currencyCode, currConn, transaction);
                                saleMaster.CurrencyID = USDCurrencyId;
                                saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(cImport.FindCurrencyRateFromBDTForBureau(currencyid, challanDate, currConn, transaction));
                                saleMaster.SerialNo = buID;
                                saleMaster.Comments = bureauType;
                                #endregion Set Master Value
                                saleDetails = new List<BureauSaleDetailVM>();
                            }

                            //// Retrive repated invoices
                            //DataTable dtRepeatedInvoices = dtCustomerInfos.Select("[INVOICE NO] ='" + detailRow["INVOICE NO"].ToString() + "'").CopyToDataTable();
                            DataTable dtRepeatedInvoices = dtAllInvoices.Select("[INVOICE NO] ='" + detailRow["INVOICE NO"].ToString() + "'").CopyToDataTable();


                            #region Repetat Value
                            decimal salesPrice = 0;
                            decimal vATAmount = 0;
                            decimal qty = 0;
                            string invoiceName = "";
                            //var invoiceDateTime = "";
                            #endregion
                            foreach (DataRow row in dtRepeatedInvoices.Rows)
                            {
                                salesPrice = salesPrice + Convert.ToDecimal(row["BDT"].ToString().Trim());
                                vATAmount = vATAmount + Convert.ToDecimal(row["VAT"].ToString().Trim());
                                qty = 1;
                                invoiceName = row["INVOICE NO"].ToString().Trim();
                            }

                            string sqlText = "";
                            sqlText += "SELECT COUNT( SalesInvoiceNo) from BureauSalesInvoiceDetails where InvoiceName =@invoiceName  and   InvoiceDateTime >= @InvoiceDate  and InvoiceDateTime <= @InvoiceToDate ";

                            DataTable dataTable = new DataTable("BureauSales");
                            SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                            cmdIdExist.Transaction = transaction;
                            cmdIdExist.Parameters.AddWithValue("@invoiceName", invoiceName);
                            cmdIdExist.Parameters.AddWithValue("@InvoiceDate", InvoiceDate);
                            cmdIdExist.Parameters.AddWithValue("@InvoiceToDate", InvoiceToDate);

                            transResult = (int)cmdIdExist.ExecuteScalar();

                            if (transResult > 0)
                            {
                                throw new ArgumentNullException("Import Sales",invoiceName+" "+ MessageVM.saleMsgFindExistID);
                            }

                            #region Detailes
                            BureauSaleDetailVM detail = new BureauSaleDetailVM();

                            detail.InvoiceLineNo = lineCounter.ToString();
                            detail.InvoiceName = invoiceName;
                            //detail.InvoiceDateTime = invoiceDateTime;
                            detail.InvoiceDateTime = InvoiceDate;
                            detail.Quantity = qty;
                            detail.ItemNo = itemNo;
                            detail.SD = Convert.ToDecimal(sd);
                            detail.SDAmount = 0;
                            detail.UOM = uom;
                            detail.Type = "VAT";
                            if (IsExport=="Y")
                            {
                                detail.Type = "Export";

                            }

                            decimal dollerValue = Convert.ToDecimal(salesPrice) /
                                Convert.ToDecimal(cImport.FindCurrencyRateBDTtoUSD((cImport.FindCurrencyId("USD", currConn, transaction)), challanDate, currConn, transaction));

                            decimal currencyValue = Convert.ToDecimal(salesPrice);
                            decimal vatRate = (Convert.ToDecimal(vATAmount) * 100) / salesPrice;
                            decimal vatAmount = Convert.ToDecimal(vATAmount);
                            detail.VATRate = vatRate;
                            detail.SalesPrice = currencyValue;
                            detail.SubTotal = currencyValue;
                            detail.VATAmount = vatAmount;
                            detail.DollerValue = dollerValue;
                            detail.CurrencyValue = currencyValue;
                            detail.InvoiceCurrency = "USD";
                            detail.CConversionDate = challanDate;
                            detail.BureauType = bureauType;
                            detail.BureauId = buID;
                            
                            saleDetails.Add(detail);
                            #endregion Detailes

                            #region Total for Master
                            totalAmount = totalAmount + currencyValue + vatAmount;
                            totalVatAmount = totalVatAmount + vatAmount;
                            #endregion Total for Master

                            #region Generate items per challan as settings


                            if (lineCounter == Convert.ToInt32(noOfSale) || totalCounter == dtDistinctInvoices.Rows.Count)
                            {
                                saleMaster.TotalAmount = totalAmount;
                                saleMaster.TotalVATAmount = totalVatAmount;
                               
                                //// Insert into database
                                try
                                {

                               
                                var sqlResults = SalesInsert(saleMaster, saleDetails, transaction, currConn);
                               
                                    retResults[0] = sqlResults[0];
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                                lineCounter = 0;
                                totalAmount = 0;
                                totalVatAmount = 0;

                            }

                            lineCounter++;
                            totalCounter++;

                            #endregion Generate items per challan as settings
                        }
                    Outer:
                        continue;
                        #endregion Detail Sale

                    } // end customer loop
                    #endregion Retrive Customer informations
                FinalOuter:
                    continue;
                }
           

                
                //////  Insert Credit information
                if (dtCreditRow.Length > 0)
                {
                    var creditResult = InsertCreditInfo(dtCredit, currConn, transaction);
                    retResults[0] = creditResult[0];
                }


                #endregion New

                if (retResults[0] == "Success")
                {
                    transaction.Commit();
                    #region SuccessResult

                    retResults[0] = "Success";
                    retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                    retResults[2] = "" + "1";
                    retResults[3] = "" + "N";
                    #endregion SuccessResult

                    if (dtCreditRow.Length > 0)
                    {
                        string result = UpdateCreditNo(challanDate);
                    }
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
               
                //throw sqlex;
            }
            catch (ArgumentNullException aeg)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + aeg.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

        public string[] InsertCreditInfo(DataTable dtCredit, SqlConnection currConn, SqlTransaction transaction)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SaleMasterVM saleMaster = new SaleMasterVM();
            List<BureauSaleDetailVM> saleDetails = new List<BureauSaleDetailVM>();

            string challanDate = BureauInfoVM.SessionDate;
            #endregion variable

            try
            {
                CommonDAL cmnDal = new CommonDAL();
                string cCode = cmnDal.settingValue("CompanyCode", "Code");
                CommonImport cImport = new CommonImport();
                string USDCurrencyId = cImport.FindCurrencyId("USD", currConn, transaction);
                //// Retrive distinct BU Id
                dtCredit.DefaultView.Sort = "BU";
                dtCredit = dtCredit.DefaultView.ToTable();

                DataTable dtBuIds = dtCredit.DefaultView.ToTable(true, "BU");
                int countID = 0;

                #region Check in Database
                string InvoiceDate = "";
                //string InvoiceToDate = "";
                if (!string.IsNullOrEmpty(dtCredit.Rows[0]["Month"].ToString().Trim()))
                {
                    var lastDay = DateTime.DaysInMonth(DateTime.Now.Year, Convert.ToInt32(dtCredit.Rows[0]["Month"].ToString().Trim()));
                    DateTime sessionDate = Convert.ToDateTime(BureauInfoVM.SessionDate);
                    InvoiceDate = sessionDate.Year.ToString() + "-" + dtCredit.Rows[0]["Month"].ToString().Trim() + "-" + lastDay;
                    InvoiceDate = Convert.ToDateTime(InvoiceDate).ToString("yyyy-MM-dd");
                }
                else
                {
                    throw new ArgumentNullException("Please insert month.'");
                }

                challanDate = InvoiceDate;
                #endregion

                foreach (DataRow buItem in dtBuIds.Rows)
                {
                    #region Retrive Customers
                    DataTable dtIndivisualBuIds = null;
                    string bureauID = "";
                    
                    if (cCode=="246")
                    {
                        if (buItem["BU"].ToString() == "246105" )
                        {
                            if (countID == 0)
                            {
                                bureauID = "'246105','246113'";
                                dtIndivisualBuIds = dtCredit.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                                countID = 1;
                            }
                        }
                        else if (buItem["BU"].ToString() == "246107")
                        {
                            bureauID = "'246107'";
                            dtIndivisualBuIds = dtCredit.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                        }
                        else if (buItem["BU"].ToString() == "246108")
                        {
                            bureauID = "'246108'";
                            dtIndivisualBuIds = dtCredit.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                        }
                        else
                        {
                            if (buItem["BU"].ToString() == "246113")
                            {
                                if (countID == 0)
                                {
                                    bureauID = "'246113'";
                                    dtIndivisualBuIds = dtCredit.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                                    countID = 1;
                                }
                                else
                                {
                                    goto FinalOuter;
                                }
                            }
                            else
                            {
                                goto FinalOuter;
                            }

                                
                        }
                    }
                    else if (cCode=="549")
                    {
                        if (buItem["BU"].ToString() == "549105" )
                        {
                            if (countID == 0)
                            {
                                bureauID = "'549105','549113'";
                                dtIndivisualBuIds = dtCredit.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                                countID = 1;
                            }
                               
                        }
                        else if (buItem["BU"].ToString() == "549107")
                        {
                            bureauID = "'549107'";
                            dtIndivisualBuIds = dtCredit.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                        }
                        else if (buItem["BU"].ToString() == "549108")
                        {
                            bureauID = "'549108'";
                            dtIndivisualBuIds = dtCredit.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                        }
                        else
                        {
                            if (buItem["BU"].ToString() == "549113")
                            {
                                if (countID == 0)
                                {
                                    bureauID = "'549113'";
                                    dtIndivisualBuIds = dtCredit.Select("[BU] in (" + bureauID + ")").CopyToDataTable();
                                    countID = 1;
                                }
                                else
                                {
                                    goto FinalOuter;
                                }
                            }
                            else
                            {
                                goto FinalOuter;
                            }
                        }
                    }

                   

                    DataTable dtAllInvoices = dtIndivisualBuIds;  /// All values repetaed invoice and customer
                    dtIndivisualBuIds.DefaultView.Sort = "CLIENT ASC";
                    dtIndivisualBuIds = dtIndivisualBuIds.DefaultView.ToTable(true);
                    DataTable dtCustomers = dtIndivisualBuIds.DefaultView.ToTable(true, "CLIENT");
                    

                    foreach (DataRow custRows in dtCustomers.Rows)
                    {
                        DataTable dtCustomerInfos = dtIndivisualBuIds.Select("[CLIENT] ='" + custRows["CLIENT"].ToString() + "' and "
                                             + "[BU] in (" + bureauID + ")").CopyToDataTable();


                        DataTable dtInvoiceNos = dtCustomerInfos.DefaultView.ToTable(true, "INVOICE NO");

                        #region Master Sale
                        DataRow dr = dtCustomerInfos.Rows[0];
                        string customerName = dr["CLIENT"].ToString().Trim();
                        string customerCode = dr["ADDRESS CODE"].ToString().Trim();
                        #region FindCustomerId
                        string customerId = cImport.FindCustomerId(customerName, customerCode, currConn, transaction);
                        #endregion FindCustomerId
                        #region FindItemNo
                        string itemNo;
                        string sd = "0";
                        string uom = "NO";

                        try
                        {
                            string productName = dr["Product NAME"].ToString().Trim();
                            string productCode = dr["Product Code"].ToString().Trim();
                            itemNo = cImport.FindItemId(productName, productCode, currConn, transaction);
                        }
                        catch (Exception)
                        {
                            DataTable productInfo = cImport.GetProductInfo(currConn, transaction);
                            itemNo = productInfo.Rows[0]["ItemNo"].ToString();
                            sd = productInfo.Rows[0]["SD"].ToString();
                            uom = productInfo.Rows[0]["UOM"].ToString();
                        }

                        #endregion FindItemNo
                        string isPrint = "N";
                        string currencyCode = "USD";
                        string createdBy = dr["Created_By"].ToString().Trim();
                        string lastModifiedBy = dr["LastModified_By"].ToString().Trim();


                        string transactionType = dr["Transection_Type"].ToString().Trim();
                        string IsExport = dr["IsExport"].ToString().Trim();
                        if (IsExport == "Y")
                        {
                            transactionType = "ExportServiceNSCredit";
                        }
                        else
                        {
                            transactionType = "Credit";
                        }
                        string post = "N";
                        try
                        {
                            post = dr["Post"].ToString().Trim();
                        }
                        catch (Exception)
                        {
                            post = "N";
                        }
                        string buId = buItem["BU"].ToString();
                        string bureauType = SearchType(buId);
                        #region Master

                        saleMaster = new SaleMasterVM();
                        saleMaster.CustomerID = customerId;
                        saleMaster.InvoiceDateTime = challanDate;
                        saleMaster.DeliveryDate = challanDate;
                        saleMaster.SaleType = "Credit";
                        saleMaster.CreatedBy = createdBy;
                        saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        saleMaster.LastModifiedBy = lastModifiedBy;
                        saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        saleMaster.IsPrint = isPrint;
                        saleMaster.TransactionType = transactionType;
                        saleMaster.Post = post; //Post
                        var currencyid = cImport.FindCurrencyId(currencyCode, currConn, transaction);
                        saleMaster.CurrencyID = USDCurrencyId; //Post
                        saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(cImport.FindCurrencyRateFromBDTForBureau(currencyid, challanDate, currConn, transaction));
                        saleMaster.SerialNo = buId;
                        saleMaster.Comments = bureauType;
                        #endregion Master


                        #endregion Master Sale

                        int dCounter = 1;
                        decimal totalAmount = 0;
                        decimal totalVatAmount = 0;
                        //var invoiceDateTime = "";

                        saleDetails = new List<BureauSaleDetailVM>();
                        foreach (DataRow detailRow in dtInvoiceNos.Rows)
                        {
                            //DataTable dtInvoices = dtCustomerInfos.Select("[INVOICE NO] ='" + detailRow["INVOICE NO"].ToString() + "'").CopyToDataTable();
                            DataTable dtInvoices = dtAllInvoices.Select("[INVOICE NO] ='" + detailRow["INVOICE NO"].ToString() + "'").CopyToDataTable();

                            
                            #region Repetat Value
                            decimal salesPrice = 0;
                            decimal vATAmount = 0;
                            decimal qty = 0;
                            string invoiceName = "";
                            
                            #endregion
                            foreach (DataRow row in dtInvoices.Rows)
                            {
                                salesPrice = salesPrice + (-Convert.ToDecimal(row["BDT"].ToString().Trim()));
                                vATAmount = vATAmount + (-Convert.ToDecimal(row["VAT"].ToString().Trim()));
                                qty = 1;
                                invoiceName = row["INVOICE NO"].ToString().Trim();
                                
                            }
                            #region Detail Sale



                            #region Detail
                            string previousSalesInvoiceNo = "";

                            BureauSaleDetailVM detail = new BureauSaleDetailVM();

                            detail.InvoiceLineNo = dCounter.ToString();
                            detail.InvoiceName = invoiceName;
                            detail.InvoiceDateTime = InvoiceDate;
                            detail.Quantity = qty;
                            detail.ItemNo = itemNo;
                            detail.SD = Convert.ToDecimal(sd);
                            detail.SDAmount = 0;
                            detail.UOM = uom;
                            detail.Type = "VAT";
                            if (IsExport == "Y")
                            {
                                detail.Type = "Export";
                            }

                            decimal dollerValue = Convert.ToDecimal(salesPrice) /
                                Convert.ToDecimal(cImport.FindCurrencyRateBDTtoUSD((cImport.FindCurrencyId("USD", currConn, transaction)), challanDate, currConn, transaction));

                            decimal currencyValue = Convert.ToDecimal(salesPrice);
                            decimal vatRate = (Convert.ToDecimal(vATAmount) * 100) / salesPrice;
                            decimal vatAmount = Convert.ToDecimal(vATAmount);

                            detail.VATRate = vatRate;
                            detail.SalesPrice = currencyValue;
                            detail.SubTotal = currencyValue;
                            detail.VATAmount = vatAmount;
                            detail.DollerValue = dollerValue;
                            detail.CurrencyValue = currencyValue;
                            detail.InvoiceCurrency = "USD";
                            detail.ReturnTransactionType = "ServiceNS";
                            detail.PreviousSalesInvoiceNo = previousSalesInvoiceNo;
                            detail.CConversionDate = challanDate;
                            detail.BureauType = bureauType;
                            detail.BureauId = buId;
                           
                            saleDetails.Add(detail);

                            dCounter++;
                            #region Total for Master
                            totalAmount = totalAmount + currencyValue + vatAmount;
                            totalVatAmount = totalVatAmount + vatAmount;
                            #endregion Total for Master
                            #endregion Detail

                        }
                        saleMaster.TotalAmount = totalAmount;
                        saleMaster.TotalVATAmount = totalVatAmount;
                        
                            #endregion Detail Sale

                        var sqlResults = SalesInsert(saleMaster, saleDetails, transaction, currConn);
                        retResults[0] = sqlResults[0];
                    }
                    #endregion Retrive Customers
                FinalOuter:
                    continue;
                }
               
            }
            #region catch & final
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (ArgumentNullException aeg)
            {
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + aeg.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
            }
            #endregion catch & final
            return retResults;
        }
        #region Old
        //public string[] ImportTestingData(DataTable dtSaleM)
        //{
        //    #region variable
        //    string[] retResults = new string[4];
        //    retResults[0] = "Fail";
        //    retResults[1] = "Fail";
        //    retResults[2] = "";
        //    retResults[3] = "";

        //    SaleMasterVM saleMaster = new SaleMasterVM();
        //    List<BureauSaleDetailVM> saleDetails = new List<BureauSaleDetailVM>();
        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;

        //    #endregion variable

        //    #region try
        //    try
        //    {
        //        if (currConn == null)
        //        {
        //            currConn = _dbsqlConnection.GetConnection();
        //            currConn.Open();
        //            transaction = currConn.BeginTransaction("Checking Data");
        //        }


        //        #region RowCount
        //        int MRowCount = 0;
        //        int MRow = dtSaleM.Rows.Count;
        //        for (int i = 0; i < dtSaleM.Rows.Count; i++)
        //        {
        //            if (!string.IsNullOrEmpty(dtSaleM.Rows[i]["inv_recipient"].ToString()))
        //            {
        //                MRowCount++;
        //            }

        //        }
        //        if (MRow != MRowCount)
        //        {
        //            string msg = "you have select " + MRow.ToString() + " data for import, but you have " + MRowCount + " id.";
        //            throw new ArgumentNullException(msg);
        //        }
        //        #endregion RowCount

        //        CommonImport cImport = new CommonImport();
        //        // for currency conversion
        //        string challanDate = BureauInfoVM.SessionDate;
        //        #region checking from database is exist the information(NULL Check)
        //        #region Master
        //        string CurrencyId = string.Empty;
        //        string USDCurrencyId = string.Empty;

        //        for (int i = 0; i < MRowCount; i++)
        //        {
        //            CurrencyId = string.Empty;
        //            USDCurrencyId = string.Empty;
        //            #region Master
        //            #region FindCustomerId
        //            cImport.FindCustomerId(dtSaleM.Rows[i]["inv_recipient"].ToString().Trim(), currConn, transaction);
        //            #endregion FindCustomerId

        //            #region FindCurrencyId
        //            CurrencyId = cImport.FindCurrencyId("BDT", currConn, transaction);
        //            USDCurrencyId = cImport.FindCurrencyId(dtSaleM.Rows[i]["inv_currency"].ToString().Trim(), currConn, transaction);
        //            cImport.FindCurrencyRateBDTtoUSD(USDCurrencyId,challanDate, currConn, transaction);

        //            #endregion FindCurrencyId

        //            #region Checking Date is null or different formate
        //            bool IsInvoiceDate;
        //            IsInvoiceDate = cImport.CheckBureauDate(dtSaleM.Rows[i]["inv_date"].ToString().Trim());
        //            if (IsInvoiceDate != true)
        //            {
        //                throw new ArgumentNullException("Please insert correct date format 'MM/DD/YYYY' such as 31/Jan/13 in Invoice_Date_Time field.");
        //            }
                   
        //            #endregion Checking Date is null or different formate

        //            #region Check invoice id
        //            string PreInvoiceId = string.Empty;

        //            PreInvoiceId = cImport.CheckCellValue(dtSaleM.Rows[i]["inv_no"].ToString().Trim());
        //            #endregion Check previous invoice id

        //            #region Check Numeric Value

        //            string IsAmount, IsVat;

        //            IsAmount = cImport.CheckNumericValue(dtSaleM.Rows[i]["amount"].ToString().Trim());
        //            if (IsAmount == "N")
        //            {
        //                throw new ArgumentNullException("Please insert decimal value in 'INVOICE AMOUNT' field.");
        //            }

        //            IsVat = cImport.CheckNumericValue(dtSaleM.Rows[i]["vat_amount"].ToString().Trim());
        //            if (IsVat == "N")
        //            {
        //                throw new ArgumentNullException("Please insert decimal value in 'VAT' field.");
        //            }
        //            #endregion
        //            #endregion Master

        //        }
        //        #endregion Master

        //        #endregion checking from database is exist the information(NULL Check)


        //        if (currConn.State == ConnectionState.Open)
        //        {
        //            transaction.Commit();
        //            currConn.Close();
        //            currConn.Open();
        //            transaction = currConn.BeginTransaction("Import Data");
        //        }

        //        string runningCustomer;
        //        int customerRowCount = -1;
        //        for (int i = 0; i < MRowCount; i++)
        //        {
        //            runningCustomer = dtSaleM.Rows[i]["inv_recipient"].ToString().Trim();

        //            #region Master Sale
        //            string importId = i.ToString();
        //            string customerName = runningCustomer;
        //            #region FindCustomerId
        //            string customerId = cImport.FindCustomerId(customerName, "", currConn, transaction);
        //            #endregion FindCustomerId
        //            string isPrint = "N";
        //            string currencyCode = dtSaleM.Rows[i]["inv_currency"].ToString().Trim();
        //            string createdBy = dtSaleM.Rows[i]["Created_By"].ToString().Trim();
        //            string lastModifiedBy = dtSaleM.Rows[i]["LastModified_By"].ToString().Trim();
        //            string transactionType = dtSaleM.Rows[i]["Transection_Type"].ToString().Trim();

        //            #region FindItemNo
        //            string itemNo;
        //            decimal sd = 0;
        //            try
        //            {
        //                string productName = dtSaleM.Rows[i]["Product NAME"].ToString().Trim();
        //                string productCode = dtSaleM.Rows[i]["Product Code"].ToString().Trim();
        //                itemNo = cImport.FindItemId(productName, productCode, currConn, transaction);
        //            }
        //            catch (Exception)
        //            {
        //                DataTable productInfo = cImport.GetProductInfo(currConn, transaction);
        //                itemNo = productInfo.Rows[0]["ItemNo"].ToString();
        //                sd = Convert.ToDecimal(productInfo.Rows[0]["SD"].ToString());
        //            }


        //            #endregion FindItemNo

        //            #region Master

        //            saleMaster = new SaleMasterVM();
        //            saleMaster.CustomerID = customerId;
        //            saleMaster.InvoiceDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //            saleMaster.SaleType = "New";
        //            saleMaster.CreatedBy = createdBy;
        //            saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //            saleMaster.LastModifiedBy = lastModifiedBy;
        //            saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //            saleMaster.IsPrint = isPrint;
        //            saleMaster.TransactionType = transactionType;
        //            saleMaster.Post = "Y"; //Post
        //            var currencyid = cImport.FindCurrencyId(currencyCode, currConn, transaction);
        //            //saleMaster.CurrencyID = currencyid; //Post
        //            saleMaster.CurrencyID = CurrencyId;
        //            saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(cImport.FindCurrencyRateFromBDT(currencyid, currConn, transaction));
        //            saleMaster.ImportID = importId;

                    

        //            #endregion Master


        //            #endregion Master Sale

        //            #region Detail Sale
        //            if (!string.IsNullOrEmpty(runningCustomer))
        //            {
        //                int dCounter = 1;
        //                decimal totalAmount = 0;
        //                decimal totalVatAmount = 0;
        //                saleDetails = new List<BureauSaleDetailVM>();
        //                DataRow[] DetailsRow = dtSaleM.Select("[inv_recipient] = '" + runningCustomer + "'");
        //                foreach (DataRow row in DetailsRow)
        //                {
        //                    string invoiceName = row["inv_no"].ToString().Trim();
        //                    var invoiceDateTime = Convert.ToDateTime(row["inv_date"].ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss");

        //                    string salesPrice = row["amount"].ToString().Trim();
        //                    string vATAmount = row["vat_amount"].ToString().Trim();

        //                    #region Detail              
        //                    BureauSaleDetailVM detail = new BureauSaleDetailVM();

        //                    detail.InvoiceLineNo = dCounter.ToString();
        //                    detail.InvoiceName = invoiceName;
        //                    detail.InvoiceDateTime = invoiceDateTime;
        //                    detail.Quantity = 1;
        //                    detail.ItemNo = itemNo;
        //                    detail.SD = 0;
        //                    detail.SDAmount = 0;
        //                    detail.UOM = "Packet";
        //                    detail.Type = "VAT";
        //                    decimal dollerValue = Convert.ToDecimal(salesPrice);
        //                    decimal currencyValue = Convert.ToDecimal(dollerValue) *
        //                                         Convert.ToDecimal(
        //                                             cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction),challanDate, currConn, transaction));
        //                    decimal vatRate = (Convert.ToDecimal(vATAmount) * 100) / dollerValue;
        //                    decimal vatAmount = Convert.ToDecimal(vATAmount) *
        //                                         Convert.ToDecimal(
        //                                             cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction),challanDate, currConn, transaction));
        //                    detail.VATRate = vatRate;
        //                    detail.SalesPrice = currencyValue;
        //                    detail.SubTotal = Convert.ToDecimal(currencyValue);
        //                    detail.VATAmount = vatAmount;
        //                    detail.DollerValue = dollerValue;
        //                    detail.CurrencyValue = currencyValue;
        //                    //var currencyid = cImport.FindCurrencyId(currencyCode, currConn, transaction);
        //                    //detail.InvoiceCurrency = currencyid; //Post
        //                    detail.InvoiceCurrency = currencyCode;
        //                    saleDetails.Add(detail);
        //                    #endregion Detail

        //                    #region Total for Master
        //                    totalAmount = totalAmount + currencyValue + vatAmount;
        //                    totalVatAmount = totalVatAmount + vatAmount;
        //                    #endregion Total for Master
        //                    dCounter++;
        //                }
        //                saleMaster.TotalAmount = totalAmount;
        //                saleMaster.TotalVATAmount = totalVatAmount;

        //                customerRowCount = customerRowCount + DetailsRow.Length;
        //                i = customerRowCount;
        //            }
        //            #endregion Detail Sale
                   
        //            var sqlResults = SalesInsert(saleMaster, saleDetails, transaction, currConn);
        //            retResults[0] = sqlResults[0];
        //        }
              
        //        if (retResults[0] == "Success")
        //        {
        //            transaction.Commit();
        //            #region SuccessResult

        //            retResults[0] = "Success";
        //            retResults[1] = MessageVM.saleMsgSaveSuccessfully;
        //            retResults[2] = "" + "1";
        //            retResults[3] = "" + "N";
        //            #endregion SuccessResult
        //        }
        //        //SAVE_DOWORK_SUCCESS = true;
        //    }
        //    #endregion try
        //    #region catch & final
        //    catch (SqlException sqlex)
        //    {
        //        if (transaction != null)
        //        {
        //            transaction.Rollback();
        //        }
        //        throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
        //        //throw sqlex;
        //    }
        //    catch (ArgumentNullException aeg)
        //    {
        //        if (transaction != null)
        //        {
        //            transaction.Rollback();
        //        }
        //        throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + aeg.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
        //        //throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null)
        //        {
        //            transaction.Rollback();
        //        }
        //        throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
        //        //throw ex;
        //    }
        //    finally
        //    {
        //        if (currConn.State == ConnectionState.Open)
        //        {
        //            currConn.Close();
        //        }
        //    }
        //    #endregion catch & final
        //    return retResults;
        //}
        #endregion
        

        public decimal ReturnSaleQty(string saleReturnId, string itemNo)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
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

                #region Return Qty

                sqlText = "  ";

                sqlText = "select Sum(isnull(SalesInvoiceDetails.Quantity,0)) from SalesInvoiceDetails ";
                sqlText += "where ItemNo =@itemNo  and SaleReturnId =@saleReturnId ";
                sqlText += "group by ItemNo";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@itemNo", itemNo);
                cmd.Parameters.AddWithValue("@saleReturnId", saleReturnId);


                if (cmd.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmd.ExecuteScalar();
                }

                #endregion Return Qty

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

        public string[] CurrencyInfo(string salesInvoiceNo)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "";//currency major
            retResults[2] = "";//currency minor
            retResults[3] = "";//currency symbol


            SqlConnection currConn = null;
            SqlTransaction transaction = null;
           
            string sqlText = "";

            #endregion Initializ

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.spMsgMethodNameUpdate);

                #endregion open connection and transaction

                #region currency

                sqlText = "";
                sqlText +=
                    " select c.CurrencyMajor,c.CurrencyMinor,c.CurrencySymbol from Currencies c,SalesInvoiceHeaders s ";
                sqlText += " where c.CurrencyId = s.CurrencyID ";
                sqlText += " and s.SalesInvoiceNo='" + salesInvoiceNo + "' ";
                sqlText += " group by c.CurrencyMajor,c.CurrencyMinor,c.CurrencySymbol ";

                DataTable dt = new DataTable("CurrencyData");
                SqlCommand cmdCurrency = new SqlCommand(sqlText, currConn);
                cmdCurrency.Transaction = transaction;

                SqlDataAdapter adpt = new SqlDataAdapter(cmdCurrency);
                adpt.Fill(dt);

                if (dt != null)
                {
                    retResults[0] = "Success";
                    retResults[1] = dt.Rows[0]["CurrencyMajor"].ToString(); //currency major
                    retResults[2] = dt.Rows[0]["CurrencyMinor"].ToString(); //currency minor
                    retResults[3] = dt.Rows[0]["CurrencySymbol"].ToString(); //currency symbol

                }

                #endregion currency

                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit
            }
            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
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

        public string  GetCategoryName(string itemNo)
        {
            #region Initializ

            string retResults = "";
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

                #region Return CategoryName

                sqlText = "  ";
               
                sqlText = " select pc.CategoryName from Products p inner join ProductCategories pc on p.CategoryID = pc.CategoryID ";
                sqlText += " where ItemNo =@itemNo ";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@itemNo", itemNo);

                if (cmd.ExecuteScalar() == null)
                {
                    retResults = "";
                }
                else
                {
                    retResults = (string)cmd.ExecuteScalar();
                }

                #endregion Return CategoryName

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
                if (currConn != null)
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

        public decimal FormatingNumeric(decimal value, int DecPlace)
        {
            object outPutValue = 0;
            string decPointLen = "";
            try
            {

                for (int i = 0; i < DecPlace; i++)
                {
                    decPointLen = decPointLen + "0";
                }
                if (value < 1000)
                {
                    var a = "0." + decPointLen + "";
                    outPutValue = value.ToString(a);
                }
                else
                {
                    var a = "0,0." + decPointLen + "";
                    outPutValue = value.ToString(a);

                }


            }
            #region Catch
            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}

            //catch (SoapHeaderException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }

            //    FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (SoapException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            catch (Exception ex)
            {
                //string exMessage = ex.Message;
                //if (ex.InnerException != null)
                //{
                //    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                //                ex.StackTrace;

                //}
                //MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            }
            #endregion Catch

            return Convert.ToDecimal(outPutValue);
        }

        public DataTable GetProductInfo()
        {
            // for TollReceive
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("ProductInfo");

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

                sqlText = "";
                sqlText = @" Select Top 1 ItemNo,ProductCode,ProductName,Products.SD from Products,ProductCategories 
                                where Products.CategoryID = ProductCategories.CategoryID 
                                and IsRaw='Service' and CategoryName='Service Non Stock'
                                order by Products.CreatedOn desc";

                SqlCommand objCommOverhead = new SqlCommand(sqlText, currConn);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommOverhead);
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

        public string UpdateCreditNo(string challanDate)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            int transaction;
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

                #region Return CategoryName

                sqlText = "  ";

                sqlText = @"

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'TempCredit') AND type in (N'U'))

BEGIN

Drop Table TempCredit

END

Create Table TempCredit(SalesInvoiceNo varchar(20),CustomerId varchar(20),InvoiceName varchar(120),ChallanDateTime DateTime)
Insert into TempCredit(CustomerId,InvoiceName,ChallanDateTime)
Select CustomerId,InvoiceName,ChallanDateTime from BureauSalesInvoiceDetails where TransactionType='Credit' 
and challanDateTime = @challanDateTime

Declare @SalesInvoiceNo varchar(20),@CustomerId varchar(20),@InvoiceName varchar(120),@ChallanDate DateTime
Declare @cursorInsert CURSOR
set @cursorInsert = Cursor FOR
Select SalesInvoiceNo,CustomerId,InvoiceName,ChallanDateTime from BureauSalesInvoiceDetails
where TransactionType='ServiceNS' and InvoiceName in (Select InvoiceName from TempCredit)
and ChallanDateTime in (Select ChallanDateTime from TempCredit)
OPEN @cursorInsert
FETCH NEXT FROM @cursorInsert
into @SalesInvoiceNo, @CustomerId,@InvoiceName,@ChallanDate
WHILE @@FETCH_STATUS = 0
BEGIN
UPDATE BureauSalesInvoiceDetails SET PreviousSalesInvoiceNo=@SalesInvoiceNo
where InvoiceName=@InvoiceName and CustomerId=@CustomerId and TransactionType='Credit' 
and ChallanDateTime = @challanDate

UPDATE SalesInvoiceHeaders SET PreviousSalesInvoiceNo=@SalesInvoiceNo
where CustomerId=@CustomerId and TransactionType='Credit' 
and InvoiceDateTime = @challanDate
FETCH NEXT FROM @cursorInsert
INTo  @SalesInvoiceNo, @CustomerId,@InvoiceName,@ChallanDate
END
CLOSE @cursorInsert
DEALLOCATE @cursorInsert

Drop Table TempCredit
                        ";


                SqlCommand cmd = new SqlCommand();
                cmd.Connection = currConn;
                cmd.CommandText = sqlText;
                cmd.CommandType = CommandType.Text;

                if (!cmd.Parameters.Contains("@challanDateTime"))
                {
                    cmd.Parameters.AddWithValue("@challanDateTime", challanDate);
                }
                else
                {
                    cmd.Parameters["@challanDateTime"].Value = challanDate;
                }
                transaction = (int)cmd.ExecuteNonQuery();
                if (transaction > 0)
                {
                    retResults = "Success";
                }
                else
                {
                    retResults = "Fail";
                }

                #endregion Return CategoryName

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
                if (currConn != null)
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

        public string SearchType(string buId)
        {
            string type = "";
            if (buId == "246107" || buId == "549107")
            {
                type = "Inspection";
            }
            else if (buId == "246108"|| buId == "549108")
            {
                type = "Social Audit";
            }
            else if (buId == "246105" || buId == "246113" || buId == "549105" || buId == "549113")
            {
                type = "Testing";
            }
            else
                type = "";

            return type;
        }
       

    }
}
    
   




