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
using System.IO;
using Excel;

// TransactionType no probs
namespace SymServices.VMS
{
    public class SaleDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();

        #endregion

        static string[] columnName = new string[] { "Sales Invoice No", "Serial No", "Vehicle No"};
        public IEnumerable<object> GetSalesColumn()
        {
            IEnumerable<object> enumList = from e in columnName
                                           select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
            return enumList;
        }
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

                sqlText = @"SELECT isnull(AlReadyPrint,0)AlReadyPrint FROM SalesInvoiceHeaders WHERE SalesInvoiceNo = @InvoiceNo";

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

                cmdUpdate.Parameters.AddWithValue("@InvoiceNo", InvoiceNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@PrintCopy", PrintCopy);

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

        public string[] SalesInsert(SaleMasterVM Master, List<SaleDetailVM> Details, List<SaleExportVM> ExportDetails, List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn)
        {
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
            string vehicleId = "0";


            int nextId = 0;
            #endregion Initializ

            #region Try
            try
            {
                CommonDAL commonDal = new CommonDAL();
                int IssuePlaceQty = Convert.ToInt32(commonDal.settings("Issue", "Quantity"));
                int IssuePlaceAmt = Convert.ToInt32(commonDal.settings("Issue", "Amount"));
                int RPlaceQty = Convert.ToInt32(commonDal.settings("Receive", "Quantity"));
                int RPlaceAmt = Convert.ToInt32(commonDal.settings("Receive", "Amount"));
                var tt = commonDal.settings("Sale", "TotalPrice");
                bool IsTotalPrice = Convert.ToBoolean(commonDal.settings("Sale", "TotalPrice").ToString().ToLower() == "y" ? true : false);

                #region Validation for Header
                SetDefaultValue(Master);


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
                sqlText = sqlText + "select COUNT(SalesInvoiceNo) from SalesInvoiceHeaders WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;

                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

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

                else if (Master.TransactionType == "Credit")
                {
                    newID = commonDal.TransactionCode("Sale", "Credit", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }


                else if (
                    Master.TransactionType == "Export"
                    || Master.TransactionType == "ExportTrading"
                    || Master.TransactionType == "ExportServiceNS"
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
                else if (Master.TransactionType == "Wastage")
                {
                    newID = commonDal.TransactionCode("Sale", "Other", "SalesInvoiceHeaders", "SalesInvoiceNo",
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
                    )
                {
                    vehicleId = "0";
                }
                else
                {
                    string vehicleID = "0";
                    sqlText = "";
                    sqlText = sqlText + "select VehicleID from Vehicles WHERE VehicleID=@VehicleID ";

                    SqlCommand cmdExistVehicleId = new SqlCommand(sqlText, currConn);
                    cmdExistVehicleId.Transaction = transaction;

                    cmdExistVehicleId.Parameters.AddWithValueAndNullHandle("@VehicleID", Master.VehicleID);////

                    string vehicleIDExist = cmdExistVehicleId.ExecuteScalar().ToString();
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
                        sqlText += "values(@vehicleID,";
                        sqlText += "@MasterVehicleType,";
                        sqlText += "@MasterVehicleNo,";
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

                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy ,";
                        sqlText += "@MasterLastModifiedOn)";

                        SqlCommand cmdExistVehicleIns = new SqlCommand(sqlText, currConn);
                        cmdExistVehicleIns.Transaction = transaction;

                        cmdExistVehicleIns.Parameters.AddWithValue("@vehicleID", vehicleID);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleType", Master.VehicleType ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);

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
                var Id = _cDal.NextId("SalesInvoiceHeaders", null, null).ToString();

                sqlText = "";
                sqlText += " insert into SalesInvoiceHeaders";
                sqlText += " (";
                //sqlText += " Id,";
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

                sqlText += " LCBank,";
                sqlText += " LCDate,";
                sqlText += " Post,";
                sqlText += " CompInvoiceNo";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                //sqlText += "@Id,";
                sqlText += "@newID,";
                sqlText += "@MasterCustomerID,";
                sqlText += "@MasterDeliveryAddress1,";
                sqlText += "@MasterDeliveryAddress2,";
                sqlText += "@MasterDeliveryAddress3,";
                sqlText += "@vehicleId,";
                sqlText += "@MasterInvoiceDateTime,";
                sqlText += "@MasterTotalAmount,";
                sqlText += "@MasterTotalVATAmount,";
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
                sqlText += "@MasterCurrencyRateFromBDT,";
                sqlText += "@MasterImportID,";
                sqlText += "@MasterLCBank, ";
                sqlText += "@MasterLCDate, ";
                sqlText += "@MasterPost, ";
                sqlText += "@MasterCompInvoiceNo";
                sqlText += ") SELECT SCOPE_IDENTITY()";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                //cmdInsert.Parameters.AddWithValue("@Id", Id);
                cmdInsert.Parameters.AddWithValue("@newID", newID);
                cmdInsert.Parameters.AddWithValue("@MasterCustomerID", Master.CustomerID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDeliveryAddress1", Master.DeliveryAddress1 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDeliveryAddress2", Master.DeliveryAddress2 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDeliveryAddress3", Master.DeliveryAddress3 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@vehicleId", vehicleId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTotalAmount", commonDal.decimal259(Master.TotalAmount));
                cmdInsert.Parameters.AddWithValue("@MasterTotalVATAmount", commonDal.decimal259(Master.TotalVATAmount));
                cmdInsert.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterSaleType", Master.SaleType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterPreviousSalesInvoiceNo", Master.PreviousSalesInvoiceNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTrading", Master.Trading ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterIsPrint", Master.IsPrint ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTenderId", Master.TenderId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDeliveryDate", Master.DeliveryDate ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCurrencyID", Master.CurrencyID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCurrencyRateFromBDT", commonDal.decimal259(Master.CurrencyRateFromBDT));
                cmdInsert.Parameters.AddWithValue("@MasterImportID", Master.ImportID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLCBank", Master.LCBank ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLCDate", Master.LCDate ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCompInvoiceNo", Master.CompInvoiceNo ?? Convert.DBNull);


                var exec = cmdInsert.ExecuteScalar();
                transResult = Convert.ToInt32(exec);
                Master.Id = transResult.ToString();
                ////////transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                //////Master.Id = Id.ToString();
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
                    sqlText += ") SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                    cmdInsertIssue.Transaction = transaction;

                    cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    //////cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                    transResult = Convert.ToInt32(cmdInsertIssue.ExecuteScalar());
                    //////Master.Id = transResult.ToString();
                    //transResult = (int)cmdInsertIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {

                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                    }

                    #endregion Insert to Issue Header

                    #region Insert to Receive

                    sqlText = "";
                    sqlText += " insert into ReceiveHeaders(";
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
                    sqlText += "'" + newID + "',";
                    sqlText += "'" + Master.InvoiceDateTime + "',";
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

                    cmdInsertReceive.Parameters.AddWithValue("@newID", newID);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
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
                    sqlText += ")	SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                    cmdInsertIssue.Transaction = transaction;

                    cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    //////cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                    transResult = Convert.ToInt32(cmdInsertIssue.ExecuteScalar());
                    //////Master.Id = transResult.ToString();
                    if (transResult <= 0)
                    {

                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                    }

                    #endregion Insert to Issue Header

                }
                #endregion TollIssue
                #region Sale for Wastage
                if (Master.TransactionType == "Wastage")
                {
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

                    cmdInsertReceive.Parameters.AddWithValue("@newID", newID);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
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
                #endregion Sale for Wastage


                #endregion if Transection not Other Insert Issue /Receive

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details == null || Details.Count() < 0)
                {
                    retResults[1] = "Details has no Data";
                    throw new ArgumentNullException(retResults[1], "");
                }


                #endregion Validation for Detail
                int lno = 1;
                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find AVG Rate
                    ProductDAL productDal = new ProductDAL();
                    //decimal AvgRate = productDal.AvgPrice(Item.ItemNo, Master.InvoiceDateTime, currConn, transaction);
                    decimal AvgRate = 0;
                    DataTable priceData = null;
                    if (Item.ReturnTransactionType == "InternalIssue" || Item.ReturnTransactionType == "Trading" || Item.ReturnTransactionType == "Service")
                    {
                        priceData = productDal.AvgPriceForInternalSales(Item.ItemNo, Master.InvoiceDateTime, currConn, transaction, false);
                    }
                    else
                    {
                        priceData = productDal.AvgPriceNew(Item.ItemNo, Master.InvoiceDateTime, currConn, transaction, false);
                    }

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

                    //AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
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

                    #region Transaction is Wastage
                    if (Master.TransactionType == "Wastage")
                    {
                        #region Insert to Receive

                        decimal subTotalRecive = Item.NBRPrice * Item.Quantity;
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
                        sqlText += " )";

                        sqlText += " values(	";
                        sqlText += "@newID,";
                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@CostPrice,";
                        sqlText += "@ItemNBRPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,";
                        if (IsTotalPrice)
                        {
                            sqlText += "@ItemSubTotal,";
                        }
                        else
                        {
                            sqlText += "@subTotalRecive,";
                        }
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += " 0,0,";
                        sqlText += "@MasterInvoiceDateTime,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@ItemDiscountAmount,";
                        sqlText += "@ItemDiscountedNBRPrice,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@temUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";
                        sqlText += ")	";
                        SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                        cmdInsertReceive.Transaction = transaction;

                        cmdInsertReceive.Parameters.AddWithValue("@newID", newID);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemInvoiceLineNo", lno);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", commonDal.decimal259(Item.Quantity));
                        cmdInsertReceive.Parameters.AddWithValue("@CostPrice", commonDal.decimal259(Item.NBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPrice", commonDal.decimal259(Item.NBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", commonDal.decimal259(Item.UOMQty));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMPrice)", commonDal.decimal259(Item.UOMPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMc", commonDal.decimal259(Item.UOMc));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                        if (IsTotalPrice)
                        {
                            cmdInsertReceive.Parameters.AddWithValue("@ItemSubTotal", commonDal.decimal259(Item.SubTotal));
                        }
                        else
                        {
                            cmdInsertReceive.Parameters.AddWithValue("@subTotalRecive", commonDal.decimal259(subTotalRecive));
                        }

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
                        sqlText += " where ReceiveHeaders.ReceiveNo='" + newID + "' ";


                        SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                        cmdUpdateReceive.Transaction = transaction;
                        int UpdateReceive = (int)cmdUpdateReceive.ExecuteNonQuery();

                        if (UpdateReceive <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveReceive);
                        }
                        #endregion Update Receive
                    }

                    #endregion Transaction is Wastage
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

                        decimal subTotatlIssue = AvgRate * Item.Quantity;
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

                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@newID,";
                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += " 0,";
                        sqlText += "@AvgRate,	";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,	";
                        if (IsTotalPrice)
                        {
                            sqlText += "@ItemSubTotal,";
                        }
                        else
                        {
                            sqlText += "@subTotatlIssue,";
                        }
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
                        sqlText += "@ItemDiscountAmount,";
                        sqlText += "@ItemDiscountedNBRPrice,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";

                        sqlText += ")	";
                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue.Transaction = transaction;

                        cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", commonDal.decimal259(Item.Quantity));
                        cmdInsertIssue.Parameters.AddWithValue("@AvgRate", commonDal.decimal259(AvgRate));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemSubTotal", commonDal.decimal259(Item.SubTotal));
                        cmdInsertIssue.Parameters.AddWithValue("@subTotatlIssue", commonDal.decimal259(subTotatlIssue));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", commonDal.decimal259(Item.UOMQty));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMPrice", commonDal.decimal259(Item.UOMPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", commonDal.decimal259(Item.UOMc));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

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
                        sqlText += " where (IssueHeaders.IssueNo= '" + newID + "')";

                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;
                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                        }
                        #endregion Update Issue

                        #region Insert to Receive

                        decimal subTotalRecive = Item.NBRPrice * Item.Quantity;
                        sqlText = "";
                        sqlText += " insert into ReceiveDetails(";
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

                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@newID,";
                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@CostPrice,";
                        sqlText += "@ItemNBRPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,";
                        if (IsTotalPrice)
                        {
                            sqlText += "@ItemSubTotal,";
                        }
                        else
                        {
                            sqlText += "@subTotalRecive,";
                        }
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += " 0,0,";
                        sqlText += "@MasterInvoiceDateTime,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@ItemDiscountAmount,";
                        sqlText += "@ItemDiscountedNBRPrice,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";

                        sqlText += ")	";
                        SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                        cmdInsertReceive.Transaction = transaction;

                        cmdInsertReceive.Parameters.AddWithValue("@newID", newID);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", commonDal.decimal259(Item.Quantity));
                        cmdInsertReceive.Parameters.AddWithValue("@CostPrice", commonDal.decimal259(Item.NBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPrice", commonDal.decimal259(Item.NBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemSubTotal", commonDal.decimal259(Item.SubTotal));
                        cmdInsertReceive.Parameters.AddWithValue("@subTotalRecive", commonDal.decimal259(subTotalRecive));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", commonDal.decimal259(Item.UOMQty));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMPrice", commonDal.decimal259(Item.UOMPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMc", commonDal.decimal259(Item.UOMc));
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
                        sqlText += " where ReceiveHeaders.ReceiveNo='" + newID + "' ";


                        SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                        cmdUpdateReceive.Transaction = transaction;
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
                        #region Issue Settings
                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                        decimal subTotA = AvgRate * Item.Quantity;
                        subTotA = FormatingNumeric(subTotA, IssuePlaceAmt);

                        #endregion Issue Settings

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
                        sqlText += "@newID,";
                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += " 0,";
                        sqlText += "@AvgRate,";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,	";
                        if (IsTotalPrice)
                        {
                            sqlText += "@ItemSubTotal,";
                        }
                        else
                        {
                            sqlText += "@subTotA,";
                        }
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
                        sqlText += "@ItemDiscountAmount,";
                        sqlText += "@ItemDiscountedNBRPrice,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";

                        sqlText += ")	";
                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue.Transaction = transaction;

                        cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", commonDal.decimal259(Item.Quantity));
                        cmdInsertIssue.Parameters.AddWithValue("@AvgRate", commonDal.decimal259(AvgRate));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemSubTotal", commonDal.decimal259(Item.SubTotal));
                        cmdInsertIssue.Parameters.AddWithValue("@subTotA", subTotA);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", commonDal.decimal259(Item.UOMQty));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMPrice", commonDal.decimal259(Item.UOMPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", commonDal.decimal259(Item.UOMc));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

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
                        sqlText += " where (IssueHeaders.IssueNo= '" + newID + "')";

                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;
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

                        #region Issue Settings
                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                        decimal subTotA = AvgRate * Item.Quantity;
                        subTotA = FormatingNumeric(subTotA, IssuePlaceAmt);

                        #endregion Issue Settings

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

                        sqlText += " )";
                        sqlText += " values(	";

                        sqlText += "@newID,";
                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += " 0,";
                        sqlText += "@AvgRate,	";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,	";
                        if (IsTotalPrice)
                        {
                            sqlText += "@ItemSubTotal,";
                        }
                        else
                        {
                            sqlText += "@subTotA,";
                        }
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
                        sqlText += "@ItemDiscountAmount,";
                        sqlText += "@ItemDiscountedNBRPrice,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@AvgRate,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";


                        sqlText += ")	";
                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue.Transaction = transaction;

                        cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsertIssue.Parameters.AddWithValue("@AvgRate", AvgRate);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemSubTotal", commonDal.decimal259(Item.SubTotal));
                        cmdInsertIssue.Parameters.AddWithValue("@subTotA", subTotA);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        cmdInsertIssue.Parameters.AddWithValue("@AvgRate", AvgRate);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

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
                        sqlText += " where (IssueHeaders.IssueNo= '" + newID + "')";

                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;
                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                        }
                        #endregion Update Issue

                        #region Insert to Receive

                        #region Issue Settings


                        NBRPrice = FormatingNumeric(NBRPrice, RPlaceQty);


                        #endregion Issue Settings

                        decimal subTot = Item.NBRPrice * Item.Quantity;
                        subTot = FormatingNumeric(subTot, RPlaceAmt);

                        //var t = commonDal.decimal259(NBRPrice + "*" + Item.Quantity);
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
                        sqlText += "@newID,";
                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@CostPrice,";
                        sqlText += "@ItemNBRPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,";
                        if (IsTotalPrice)
                        {
                            sqlText += "@ItemSubTotal,";
                        }
                        else
                        {
                            sqlText += "@subTot,";
                        }

                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += " 0,0,";
                        sqlText += "@MasterInvoiceDateTime,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@ItemDiscountAmount),";
                        sqlText += "@ItemDiscountedNBRPrice),";
                        sqlText += "@ItemUOMQty),";
                        sqlText += "@ItemUOMPrice),";
                        sqlText += "@ItemUOMc),";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";

                        sqlText += ")	";
                        SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                        cmdInsertReceive.Transaction = transaction;

                        cmdInsertReceive.Parameters.AddWithValue("@newID", newID);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", commonDal.decimal259(Item.Quantity));
                        cmdInsertReceive.Parameters.AddWithValue("@CostPrice", commonDal.decimal259(Item.NBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPrice", commonDal.decimal259(Item.NBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemSubTotal", commonDal.decimal259(Item.SubTotal));
                        cmdInsertReceive.Parameters.AddWithValue("@subTot", subTot);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", commonDal.decimal259(Item.UOMQty));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMPrice", commonDal.decimal259(Item.UOMPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMc", commonDal.decimal259(Item.UOMc));
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
                        sqlText += " where ReceiveHeaders.ReceiveNo='" + newID + "' ";


                        SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                        cmdUpdateReceive.Transaction = transaction;
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
                    AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);

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
                    sqlText += " ValueOnly,";
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
                    sqlText += " VATName,";
                    sqlText += " UOMPrice,";
                    sqlText += "CConversionDate,";
                    sqlText += "Weight";
                    if (Master.TransactionType == "Credit" || Master.TransactionType == "Debit")
                    {
                        sqlText += ", ReturnTransactionType ";
                    }
                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@newID,";
                    sqlText += "@ItemInvoiceLineNo, ";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemQuantity,";
                    sqlText += "@ItemPromotionalQuantity,";
                    sqlText += "@ItemSalesPrice,";
                    sqlText += "@ItemNBRPrice,";
                    sqlText += "@AvgRate,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemVATRate,";
                    sqlText += "@ItemVATAmount,";
                    sqlText += "@ItemValueOnly,";
                    sqlText += "@ItemSubTotal,";
                    sqlText += "@ItemCommentsD,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@ItemSD,";
                    sqlText += "@ItemSDAmount,";
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
                    sqlText += "@ItemUOMQty,";
                    sqlText += "@ItemUOMn,";
                    sqlText += "@ItemUOMc,";
                    sqlText += "@ItemDiscountAmount,";
                    sqlText += "@ItemDiscountedNBRPrice,";
                    sqlText += "@ItemDollerValue,";
                    sqlText += "@ItemCurrencyValue,";
                    sqlText += "@ItemVatName,";
                    sqlText += "@ItemUOMPrice, ";
                    sqlText += "@ItemCConversionDate, ";
                    sqlText += "@ItemWeight ";

                    if (Master.TransactionType == "Credit" || Master.TransactionType == "Debit")
                    {
                        sqlText += ",@ItemReturnTransactionType";
                    }
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsDetail.Parameters.AddWithValue("@newID", newID);
                    cmdInsDetail.Parameters.AddWithValue("@ItemInvoiceLineNo", lno);
                    cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", commonDal.decimal259(Item.Quantity));
                    cmdInsDetail.Parameters.AddWithValue("@ItemPromotionalQuantity", commonDal.decimal259(Item.PromotionalQuantity));
                    cmdInsDetail.Parameters.AddWithValue("@ItemSalesPrice", commonDal.decimal259(Item.SalesPrice));
                    cmdInsDetail.Parameters.AddWithValue("@ItemNBRPrice", commonDal.decimal259(Item.NBRPrice));
                    cmdInsDetail.Parameters.AddWithValue("@AvgRate", commonDal.decimal259(AvgRate));
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", commonDal.decimal259(Item.VATRate));
                    cmdInsDetail.Parameters.AddWithValue("@ItemVATAmount", commonDal.decimal259(Item.VATAmount));
                    cmdInsDetail.Parameters.AddWithValue("@ItemValueOnly", Item.ValueOnly ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", commonDal.decimal259(Item.SubTotal));
                    cmdInsDetail.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemSD", commonDal.decimal259(Item.SD));
                    cmdInsDetail.Parameters.AddWithValue("@ItemSDAmount", commonDal.decimal259(Item.SDAmount));
                    cmdInsDetail.Parameters.AddWithValue("@ItemSaleTypeD", Item.SaleTypeD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPreviousSalesInvoiceNoD", Item.PreviousSalesInvoiceNoD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemTradingD", Item.TradingD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemNonStockD", Item.NonStockD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemTradingMarkUp", Item.TradingMarkUp);
                    cmdInsDetail.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemType", Item.Type ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", commonDal.decimal259(Item.UOMQty));
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", commonDal.decimal259(Item.UOMc));
                    cmdInsDetail.Parameters.AddWithValue("@ItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                    cmdInsDetail.Parameters.AddWithValue("@ItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                    cmdInsDetail.Parameters.AddWithValue("@ItemDollerValue", commonDal.decimal259(Item.DollerValue));
                    cmdInsDetail.Parameters.AddWithValue("@ItemCurrencyValue", commonDal.decimal259(Item.CurrencyValue));
                    cmdInsDetail.Parameters.AddWithValue("@ItemVatName", Item.VatName ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", commonDal.decimal259(Item.UOMPrice));
                    cmdInsDetail.Parameters.AddWithValue("@ItemCConversionDate", Item.CConversionDate ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemWeight", Item.Weight ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemReturnTransactionType", Item.ReturnTransactionType ?? Convert.DBNull);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable
                    lno++;
                }



                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)


                #region Insert into Export
                if (Master.TransactionType == "Export"
                    || Master.TransactionType == "ExportTender"
                    || Master.TransactionType == "ExportTrading"
                    || Master.TransactionType == "ExportServiceNS"
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
                    sqlText += "delete from SalesInvoiceHeadersExport where SalesInvoiceNo='" + newID + "'";


                    SqlCommand cmdFindId1 = new SqlCommand(sqlText, currConn);
                    cmdFindId1.Transaction = transaction;
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

                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@newID", newID);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportSaleLineNo ", ItemExport.SaleLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportRefNo", ItemExport.RefNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportDescription", ItemExport.Description ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportQuantityE", ItemExport.QuantityE);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportGrossWeight", ItemExport.GrossWeight);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportNetWeight", ItemExport.NetWeight);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportNumberFrom ", ItemExport.NumberFrom);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportNumberTo", ItemExport.NumberTo);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportCommentsE", ItemExport.CommentsE ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy ", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn ", Master.LastModifiedOn ?? Convert.DBNull);

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

                #region Tracking
                if (Trackings != null && Trackings.Count > 0)
                {
                    for (int i = 0; i < Trackings.Count; i++)
                    {
                        if (Master.TransactionType == "Credit" || Master.TransactionType == "Debit")
                        {
                            if (Trackings[i].ReturnSale == "Y")
                            {
                                Trackings[i].ReturnSaleID = newID;
                                Trackings[i].ReturnType = Master.TransactionType;
                            }

                        }
                        else if (Trackings[i].IsSale == "Y")
                        {
                            Trackings[i].SaleInvoiceNo = newID;
                        }

                    }


                    string trackingUpdate = string.Empty;
                    TrackingDAL trackingDal = new TrackingDAL();
                    trackingUpdate = trackingDal.TrackingUpdate(Trackings, transaction, currConn);

                    if (trackingUpdate == "Fail")
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, "Tracking Information not added.");
                    }
                }
                #endregion


                #region TrackingWithSale
                bool TrackingWithSale = Convert.ToBoolean(commonDal.settingValue("Purchase", "TrackingWithSale") == "Y" ? true : false);
                bool TrackingWithSaleFIFO = Convert.ToBoolean(commonDal.settingValue("Purchase", "TrackingWithSaleFIFO") == "Y" ? true : false);
                int NumberOfItems = Convert.ToInt32(commonDal.settingValue("Sale", "NumberOfItems"));
                if (TrackingWithSale)
                {


                    foreach (var Item in Details.ToList())
                    {
                        sqlText += "@ItemInvoiceLineNo, ";
                        DataTable tracDt = new DataTable();
                        sqlText = "";
                        sqlText += @" select top " + Convert.ToInt32(Item.Quantity) + " * from PurchaseSaleTrackings";
                        sqlText += @" where IsSold=0";
                        sqlText += @" and ItemNo=@ItemItemNo";
                        if (TrackingWithSaleFIFO)
                        {
                            sqlText += @" order by id asc ";
                        }
                        else
                        {
                            sqlText += @" order by id desc ";
                        }
                        SqlCommand cmdRIFB1 = new SqlCommand(sqlText, currConn);
                        cmdRIFB1.Transaction = transaction;

                        cmdRIFB1.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo);
                        cmdRIFB1.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                        SqlDataAdapter reportDataAdapt1 = new SqlDataAdapter(cmdRIFB1);
                        reportDataAdapt1.Fill(tracDt);
                        if (Item.Quantity > tracDt.Rows.Count)
                        {
                            throw new ArgumentNullException("Stock not available", "Stock not available");
                        }
                        foreach (DataRow itemTrac in tracDt.Rows)
                        {
                            sqlText = "";
                            sqlText += " update PurchaseSaleTrackings set  ";
                            sqlText += " IsSold= '1' ,";
                            sqlText += " SalesInvoiceNo= @newID ,";
                            sqlText += " SaleInvoiceDateTime=@MasterInvoiceDateTime ";
                            sqlText += " where  Id= '" + itemTrac["Id"] + "' ";

                            SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                            cmdUpdate.Transaction = transaction;

                            cmdUpdate.Parameters.AddWithValue("@newID", newID);
                            cmdUpdate.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime);

                            transResult = (int)cmdUpdate.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                            }
                        }
                    }
                    ReportDSDAL rds = new ReportDSDAL();
                    DataSet ds = new DataSet();
                    ds = rds.VAT11ReportCommercialImporterNew(newID, "N", "N", currConn, transaction);
                    if (ds.Tables[0].Rows.Count > NumberOfItems)
                    {
                        throw new ArgumentNullException("Number of Items in a Invoice exist", "Number of Items in a Invoice exist");
                    }

                }
                #endregion TrackingWithSale
                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from SalesInvoiceHeaders WHERE SalesInvoiceNo='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
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
                retResults[2] = "" + Master.Id;
                retResults[3] = "" + PostStatus;
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

                if (vcurrConn == null) { transaction.Rollback(); }

                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
            }
            #endregion
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

        public string[] SalesUpdate(SaleMasterVM Master, List<SaleDetailVM> Details, List<SaleExportVM> ExportDetails, List<TrackingVM> Trackings)
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



            int nextId = 0;
            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                SetDefaultValue(Master);

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

                string transactionDate = Master.InvoiceDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.InvoiceDateTime).ToString("yyyy-MM-dd");
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
                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(SalesInvoiceNo) from SalesInvoiceHeaders WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;

                cmdFindIdUpd.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableFindExistID);
                }

                #endregion Find ID for Update
                #region Not Needed in Web Application
                if (false)
                {
                    #region VehicleId
                    if (Master.TransactionType == "Service"
                        || Master.TransactionType == "ServiceNS"
                        || Master.TransactionType == "ExportService"
                        || Master.TransactionType == "ExportServiceNS"
                        )
                    {
                        vehicleId = "0";
                    }
                    else
                    {


                        sqlText = "";
                        sqlText = sqlText + "select COUNT(VehicleNo) from Vehicles WHERE VehicleID=@VehicleID ";
                        SqlCommand cmdExistVehicleId = new SqlCommand(sqlText, currConn);
                        cmdExistVehicleId.Transaction = transaction;
                        if (Master.VehicleNo == null)
                        {

                        }

                        cmdExistVehicleId.Parameters.AddWithValueAndNullHandle("@VehicleID", Master.VehicleID);

                        IDExist = (int)cmdExistVehicleId.ExecuteScalar();


                        if (IDExist <= 0)
                        {

                            sqlText = "";
                            sqlText +=
                                " INSERT INTO Vehicles (VehicleID,	VehicleType,	VehicleNo,	Description,	Comments,	ActiveStatus,CreatedBy,	CreatedOn,	LastModifiedBy,	LastModifiedOn)";
                            sqlText += " select MAX(isnull(VehicleID,0)+1) ,";
                            sqlText += " @MasterVehicleType ,";
                            sqlText += " @MasterVehicleNo,";
                            sqlText += " 'NA',";
                            sqlText += " 'NA',	";
                            if (Master.vehicleSaveInDB == true)
                            {
                                sqlText += " 'Y',";

                            }
                            else
                            {
                                sqlText += " 'N',";
                            }
                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy ,";
                            sqlText += "@MasterLastModifiedOn";
                            sqlText += " from Vehicles;";

                            SqlCommand cmdExistVehicleIns = new SqlCommand(sqlText, currConn);
                            cmdExistVehicleIns.Transaction = transaction;

                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleType", Master.VehicleType ?? Convert.DBNull);
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo ?? Convert.DBNull);
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);

                            transResult = (int)cmdExistVehicleIns.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                                MessageVM.saleMsgUnableCreatID);
                            }
                        }
                        else
                        {
                            sqlText = "";
                            sqlText += " Update Vehicles SET VehicleType= @MasterVehicleType";
                            sqlText += " where VehicleNo=@MasterVehicleNo";

                            SqlCommand cmdUpdateVehicleIns = new SqlCommand(sqlText, currConn);
                            cmdUpdateVehicleIns.Transaction = transaction;

                            cmdUpdateVehicleIns.Parameters.AddWithValue("@MasterVehicleType", Master.VehicleType ?? Convert.DBNull);
                            cmdUpdateVehicleIns.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo ?? Convert.DBNull);

                            transResult = (int)cmdUpdateVehicleIns.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                            }
                        }




                        //////sqlText = "";
                        //////sqlText += "select	VehicleID FROM Vehicles where VehicleNo=@MasterVehicleNo; ";

                        //////SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                        //////cmdPrefetch.Transaction = transaction;

                        //cmdPrefetch.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo ?? Convert.DBNull);

                        //var temp=cmdPrefetch.ExecuteScalar();

                        //////vehicleId = Master.VehicleNo;

                        //////if (string.IsNullOrEmpty(vehicleId))
                        //////{
                        //////    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableCreatID);
                        //////}


                    }


                    #endregion VehicleId
                }

                #endregion

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update SalesInvoiceHeaders set  ";
                sqlText += " CustomerID             =@MasterCustomerID ,";
                sqlText += " DeliveryAddress1       =@MasterDeliveryAddress1 ,";
                sqlText += " DeliveryAddress2       =@MasterDeliveryAddress2 ,";
                sqlText += " DeliveryAddress3       =@MasterDeliveryAddress3 ,";
                sqlText += " VehicleID              =@vehiclId ,";
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
                sqlText += " ImportIDExcel          =@MasterImportID ,";
                sqlText += " LCDate                 =@MasterLCDate ,";
                sqlText += " LCBank                 =@MasterLCBank ,";
                sqlText += " LCNumber               =@MasterLCNumber ,";
                sqlText += " Post                   =@MasterPost ";
                sqlText += " where  SalesInvoiceNo  =@MasterSalesInvoiceNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@MasterCustomerID", Master.CustomerID);
                cmdUpdate.Parameters.AddWithValue("@MasterDeliveryAddress1", Master.DeliveryAddress1 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterDeliveryAddress2", Master.DeliveryAddress2 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterDeliveryAddress3", Master.DeliveryAddress3 ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@vehiclId", Master.VehicleID ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterDeliveryDate", Master.DeliveryDate ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterSaleType", Master.SaleType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterPreviousSalesInvoiceNo", Master.PreviousSalesInvoiceNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterTrading", Master.Trading ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterIsPrint", Master.IsPrint ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterTenderId", Master.TenderId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterCurrencyID", Master.CurrencyID ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterCurrencyRateFromBDT", Master.CurrencyRateFromBDT);
                cmdUpdate.Parameters.AddWithValue("@MasterImportID", Master.ImportID ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLCDate", Master.LCDate ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLCBank", Master.LCBank ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLCNumber", Master.LCNumber ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                }
                #endregion update Header

                #region Transaction Not Other

                #region Transaction is Wastage
                if (Master.TransactionType == "Wastage")
                {
                    #region update Receive
                    sqlText = "";

                    sqlText += " update ReceiveHeaders set";
                    sqlText += " ReceiveDateTime    = @MasterInvoiceDateTime ,";
                    sqlText += " Comments           = @MasterComments ,";
                    sqlText += " LastModifiedBy     = @MasterLastModifiedBy ,";
                    sqlText += " LastModifiedOn     = @MasterLastModifiedOn ,";
                    sqlText += " transactionType    = @MasterTransactionType ,";
                    sqlText += " ReceiveReturnId    = @MasterReturnId ,";
                    sqlText += " Post               = @MasterPost ";
                    sqlText += " where  ReceiveNo   = @MasterSalesInvoiceNo";

                    SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                    cmdUpdateReceive.Transaction = transaction;

                    cmdUpdateReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);

                    transResult = (int)cmdUpdateReceive.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, "Requested Sales entry not update in Receive data");
                    }
                    #endregion update Receive
                }
                #endregion Transaction is Wastage
                #region Transaction is TollReceive

                if (Master.TransactionType == "InternalIssue"
                    || Master.TransactionType == "Service"
                    || Master.TransactionType == "ExportService"
                    || Master.TransactionType == "Trading"
                    || Master.TransactionType == "ExportTradingTender"
                    || Master.TransactionType == "TradingTender"
                    || Master.TransactionType == "ExportTrading"
                    )
                {
                    #region update Issue

                    sqlText = "";

                    sqlText += " update IssueHeaders set ";
                    sqlText += " IssueDateTime  = @MasterInvoiceDateTime ,";
                    sqlText += " Comments       = @MasterComments  ,";
                    sqlText += " LastModifiedBy = @MasterLastModifiedBy  ,";
                    sqlText += " LastModifiedOn = @MasterLastModifiedOn ,";
                    sqlText += " transactionType= @MasterTransactionType  ,";
                    sqlText += " IssueReturnId  = @MasterReturnId  ,";
                    sqlText += " Post           = @MasterPost  ";
                    sqlText += " where  IssueNo = @MasterSalesInvoiceNo  ";


                    SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                    cmdUpdateIssue.Transaction = transaction;

                    cmdUpdateIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);

                    transResult = (int)cmdUpdateIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate,
                                                        MessageVM.saleMsgUpdateNotSuccessfully);
                    }

                    #endregion update Issue

                    #region update Receive
                    sqlText = "";

                    sqlText += " update ReceiveHeaders set";
                    sqlText += " ReceiveDateTime    = @MasterInvoiceDateTime ,";
                    sqlText += " Comments           = @MasterComments ,";
                    sqlText += " LastModifiedBy     = @MasterLastModifiedBy ,";
                    sqlText += " LastModifiedOn     = @MasterLastModifiedOn ,";
                    sqlText += " transactionType    = @MasterTransactionType ,";
                    sqlText += " ReceiveReturnId    = @MasterReturnId ,";
                    sqlText += " Post               = @MasterPost ";
                    sqlText += " where  ReceiveNo   = @MasterSalesInvoiceNo";

                    SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                    cmdUpdateReceive.Transaction = transaction;

                    cmdUpdateReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);

                    transResult = (int)cmdUpdateReceive.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, "Requested Sales entry not update in Receive data");
                    }
                    #endregion update Receive

                }
                #endregion Transaction is TollReceive

                #region Transaction is TollReceive

                else if (Master.TransactionType == "TollIssue")
                {
                    #region update Issue

                    sqlText = "";

                    sqlText += " update IssueHeaders set ";
                    sqlText += " IssueDateTime  = @MasterInvoiceDateTime,";
                    sqlText += " Comments       = @MasterComments ,";
                    sqlText += " LastModifiedBy = @MasterLastModifiedBy ,";
                    sqlText += " LastModifiedOn = @MasterLastModifiedOn,";
                    sqlText += " transactionType= @MasterTransactionType ,";
                    sqlText += " IssueReturnId  = @MasterReturnId ,";
                    sqlText += " Post           = @MasterPost ";
                    sqlText += " where  IssueNo = @MasterSalesInvoiceNo ";


                    SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                    cmdUpdateIssue.Transaction = transaction;

                    cmdUpdateIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);

                    transResult = (int)cmdUpdateIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate,
                                                        MessageVM.saleMsgUpdateNotSuccessfully);
                    }

                    #endregion update Issue

                }
                #endregion Transaction is TollReceive

                #endregion Transaction Not Other


                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table
                int lno = 1;
                foreach (var Item in Details.ToList())
                {
                    #region Find AVG Rate
                    ProductDAL productDal = new ProductDAL();
                    //decimal AvgRate = productDal.AvgPrice(Item.ItemNo, Master.InvoiceDateTime, currConn, transaction);
                    decimal AvgRate = 0;
                    DataTable priceData = null;
                    if (Item.ReturnTransactionType == "InternalIssue" || Item.ReturnTransactionType == "Trading" || Item.ReturnTransactionType == "Service")
                    {
                        priceData = productDal.AvgPriceForInternalSales(Item.ItemNo, Master.InvoiceDateTime, currConn, transaction, false);
                    }
                    else
                    {
                        priceData = productDal.AvgPriceNew(Item.ItemNo, Master.InvoiceDateTime, currConn, transaction, false);
                    }

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
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(SalesInvoiceNo) from SalesInvoiceDetails WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        // Insert

                        #region Insert Issue and Receive if Transaction is not Other
                        #region Transaction is Wastage
                        if (Master.TransactionType == "Wastage")
                        {
                            #region Insert to Receive

                            sqlText = "";
                            sqlText += " insert into ReceiveDetails(";
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
                            sqlText += " VATName,";
                            sqlText += " Post";
                            sqlText += " )";

                            sqlText += " values(	";
                            sqlText += "@MasterSalesInvoiceNo,";
                            sqlText += "@ItemInvoiceLineNo,";
                            sqlText += "@ItemItemNo,";
                            sqlText += "@ItemQuantity,";
                            sqlText += "@CostPrice,";
                            sqlText += "@ItemNBRPrice,";
                            sqlText += "@ItemUOM,";
                            sqlText += " 0,	0,";
                            sqlText += "@ItemNBRPriceItemQuantity ,";
                            sqlText += "@ItemCommentsD,";
                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy,";
                            sqlText += "@MasterLastModifiedOn,";
                            sqlText += " 0,0,";
                            sqlText += "@MasterInvoiceDateTime,";
                            sqlText += "@MasterTransactionType,";
                            sqlText += "@MasterReturnId,";
                            sqlText += "@ItemDiscountAmount,";
                            sqlText += "@ItemDiscountedNBRPrice,";
                            sqlText += "@ItemUOMQty,";
                            sqlText += "@ItemUOMPrice,";
                            sqlText += "@ItemUOMc,";
                            sqlText += "@ItemUOMn,";
                            sqlText += "@ItemVatName,";
                            sqlText += "@MasterPost";
                            sqlText += ")	";
                            SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                            cmdInsertReceive.Transaction = transaction;

                            cmdInsertReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertReceive.Parameters.AddWithValue("@CostPrice", Item.NBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPrice", Item.NBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPriceItemQuantity", Item.NBRPrice * Item.Quantity);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemVatName", Item.VatName ?? Convert.DBNull);
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
                            sqlText += " where ReceiveHeaders.ReceiveNo=@MasterSalesInvoiceNo ";


                            SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                            cmdUpdateReceive.Transaction = transaction;

                            cmdUpdateReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                            transResult = (int)cmdUpdateReceive.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveReceive);
                            }
                            #endregion Update Receive
                        }

                        #endregion Transaction is Wastage
                        #region Transaction is Trading

                        if (
                            Master.TransactionType == "Trading"
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
                            sqlText += "@MasterSalesInvoiceNo,";
                            sqlText += "@ItemInvoiceLineNo,";
                            sqlText += "@ItemItemNo,";
                            sqlText += "@ItemQuantity,";
                            sqlText += " 0,";
                            sqlText += "@AvgRate,	";
                            sqlText += "@ItemUOM,";
                            sqlText += " 0,	0,	";
                            sqlText += "@AvgRateItemQuantity,";
                            sqlText += "@ItemCommentsD,";
                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy,";
                            sqlText += "@MasterLastModifiedOn,";
                            sqlText += "@MasterSalesInvoiceNo,";
                            sqlText += "@MasterInvoiceDateTime,";
                            sqlText += " 0,	0,";
                            sqlText += " 0,	0,	0,";
                            sqlText += "@MasterTransactionType,";
                            sqlText += "@MasterReturnId,";
                            sqlText += "@ItemDiscountAmount,";
                            sqlText += "@ItemDiscountedNBRPrice,";
                            sqlText += "@ItemUOMQty,";
                            sqlText += "@ItemUOMPrice,";
                            sqlText += "@ItemUOMc,";
                            sqlText += "@ItemUOMn,";
                            sqlText += "@MasterPost";

                            sqlText += ")	";
                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertIssue.Parameters.AddWithValue("@AvgRate", AvgRate);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@AvgRateItemQuantity", AvgRate * Item.Quantity);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            //////cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

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
                            sqlText += " where (IssueHeaders.IssueNo= @MasterSalesInvoiceNo )";

                            SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                            cmdUpdateIssue.Transaction = transaction;

                            cmdUpdateIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                            transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                            }
                            #endregion Update Issue

                            #region Insert to Receive

                            sqlText = "";
                            sqlText += " insert into ReceiveDetails(";
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
                            sqlText += " VATName,";
                            sqlText += " Post";

                            sqlText += " )";
                            sqlText += " values(	";
                            sqlText += "@MasterSalesInvoiceNo,";
                            sqlText += "@ItemInvoiceLineNo,";
                            sqlText += "@ItemItemNo,";
                            sqlText += "@ItemQuantity,";
                            sqlText += "@CostPrice,";
                            sqlText += "@ItemNBRPrice,";
                            sqlText += "@ItemUOM,";
                            sqlText += " 0,	0,";
                            sqlText += "@ItemNBRPriceItemQuantity,";
                            sqlText += "@ItemCommentsD,";
                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy,";
                            sqlText += "@MasterLastModifiedOn,";
                            sqlText += " 0,0,";
                            sqlText += "@MasterInvoiceDateTime,";
                            sqlText += "@MasterTransactionType,";
                            sqlText += "@MasterReturnId,";
                            sqlText += "@ItemDiscountAmount,";
                            sqlText += "@ItemDiscountedNBRPrice,";
                            sqlText += "@ItemUOMQty,";
                            sqlText += "@ItemUOMPrice,";
                            sqlText += "@ItemUOMc,";
                            sqlText += "@ItemUOMn,";
                            sqlText += "@ItemVatName,";
                            sqlText += "@MasterPost";

                            sqlText += ")	";
                            SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                            cmdInsertReceive.Transaction = transaction;

                            cmdInsertReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertReceive.Parameters.AddWithValue("@CostPrice", Item.NBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPrice", Item.NBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPriceItemQuantity", Item.NBRPrice * Item.Quantity);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemVatName", Item.VatName ?? Convert.DBNull);
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
                            sqlText += " where ReceiveHeaders.ReceiveNo=@MasterSalesInvoiceNo ";


                            SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                            cmdUpdateReceive.Transaction = transaction;

                            cmdUpdateReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                            transResult = (int)cmdUpdateReceive.ExecuteNonQuery();

                            if (transResult <= 0)
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
                            sqlText += "@MasterSalesInvoiceNo,";
                            sqlText += "@ItemInvoiceLineNo,";
                            sqlText += "@ItemItemNo,";
                            sqlText += "@ItemQuantity,";
                            sqlText += " 0,";
                            sqlText += "@AvgRate,	";
                            sqlText += "@ItemUOM,";
                            sqlText += " 0,	0,	";
                            sqlText += "@AvgRateItemQuantity,";
                            sqlText += "@ItemCommentsD,";
                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy,";
                            sqlText += "@MasterLastModifiedOn,";
                            sqlText += "@MasterSalesInvoiceNo,";
                            sqlText += "@MasterInvoiceDateTime,";
                            sqlText += " 0,	0,";
                            sqlText += " 0,	0,	0,";
                            sqlText += "@MasterTransactionType,";
                            sqlText += "@MasterReturnId,";
                            sqlText += "@ItemDiscountAmount,";
                            sqlText += "@ItemDiscountedNBRPrice,";
                            sqlText += "@ItemUOMQty,";
                            sqlText += "@ItemUOMPrice,";
                            sqlText += "@ItemUOMc,";
                            sqlText += "@ItemUOMn,";
                            sqlText += "@MasterPost";

                            sqlText += ")	";
                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertIssue.Parameters.AddWithValue("@AvgRate", AvgRate);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM);
                            cmdInsertIssue.Parameters.AddWithValue("@AvgRateItemQuantity", AvgRate * Item.Quantity);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            ////cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

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
                            sqlText += " where (IssueHeaders.IssueNo= @MasterSalesInvoiceNo )";

                            SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                            cmdUpdateIssue.Transaction = transaction;

                            cmdUpdateIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

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


                            decimal NBRPrice = productDal.GetLastNBRPriceFromBOM(Item.ItemNo, "VAT 1 (Internal Issue)", Master.InvoiceDateTime, currConn, transaction);


                            #region Issue Settings


                            AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                            decimal subTotA = AvgRate * Item.Quantity;
                            subTotA = FormatingNumeric(subTotA, IssuePlaceAmt);

                            #endregion Issue Settings


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
                            sqlText += "@MasterSalesInvoiceNo,";
                            sqlText += "@ItemInvoiceLineNo,";
                            sqlText += "@ItemItemNo,";
                            sqlText += "@ItemQuantity,";
                            sqlText += " 0,";
                            sqlText += "@AvgRate,	";
                            sqlText += "@ItemUOM,";
                            sqlText += " 0,	0,	";
                            sqlText += "@subTotA,";
                            sqlText += "@ItemCommentsD,";
                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy,";
                            sqlText += "@MasterLastModifiedOn,";
                            sqlText += "@MasterSalesInvoiceNo,";
                            sqlText += "@MasterInvoiceDateTime,";
                            sqlText += " 0,	0,";
                            sqlText += " 0,	0,	0,";
                            sqlText += "@MasterTransactionType,";
                            sqlText += "@MasterReturnId,";
                            sqlText += "@ItemDiscountAmount,";
                            sqlText += "@ItemDiscountedNBRPrice,";
                            sqlText += "@ItemUOMQty,";
                            sqlText += "@AvgRate,";
                            sqlText += "@ItemUOMc,";
                            sqlText += "@ItemUOMn,";
                            sqlText += "@MasterPost";

                            sqlText += ")	";
                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertIssue.Parameters.AddWithValue("@AvgRate", AvgRate);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@subTotA", subTotA);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            //////cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertIssue.Parameters.AddWithValue("@AvgRate", AvgRate);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

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
                            sqlText += " where (IssueHeaders.IssueNo= @MasterSalesInvoiceNo)";

                            SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                            cmdUpdateIssue.Transaction = transaction;

                            cmdUpdateIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                            transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                            }
                            #endregion Update Issue

                            subTotA = Item.NBRPrice * Item.Quantity;

                            NBRPrice = FormatingNumeric(NBRPrice, ReceivePlaceAmt);
                            subTotA = FormatingNumeric(subTotA, ReceivePlaceAmt);

                            #region Insert to Receive

                            sqlText = "";
                            sqlText += " insert into ReceiveDetails(";
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
                            sqlText += " VATName,";
                            sqlText += " Post";

                            //sqlText += " Post";
                            sqlText += " )";
                            sqlText += " values(	";
                            sqlText += "@MasterSalesInvoiceNo,";
                            sqlText += "@ItemInvoiceLineNo,";
                            sqlText += "@ItemItemNo,";
                            sqlText += "@ItemQuantity,";
                            sqlText += "@CostPrice,";
                            sqlText += "@ItemNBRPrice,";
                            sqlText += "@ItemUOM,";
                            sqlText += " 0,	0,";
                            sqlText += "@subTotA,";
                            sqlText += "@ItemCommentsD,";
                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy,";
                            sqlText += "@MasterLastModifiedOn,";
                            sqlText += " 0,0,";
                            sqlText += "@MasterInvoiceDateTime,";
                            sqlText += "@MasterTransactionType,";
                            sqlText += "@MasterReturnId,";
                            sqlText += "@ItemDiscountAmount,";
                            sqlText += "@ItemDiscountedNBRPrice,";
                            sqlText += "@ItemUOMQty,";
                            sqlText += "@ItemUOMPrice,";
                            sqlText += "@ItemUOMc,";
                            sqlText += "@ItemUOMn,";
                            sqlText += "@ItemVatName,";
                            sqlText += "@MasterPost";

                            sqlText += ")	";
                            SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                            cmdInsertReceive.Transaction = transaction;

                            cmdInsertReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertReceive.Parameters.AddWithValue("@CostPrice", Item.NBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPrice", Item.NBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@subTotA", subTotA);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemVatName", Item.VatName ?? Convert.DBNull);
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
                            sqlText += " where ReceiveHeaders.ReceiveNo=@MasterSalesInvoiceNo ";


                            SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                            cmdUpdateReceive.Transaction = transaction;

                            cmdUpdateReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

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
                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);

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
                        sqlText += " ValueOnly,";
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
                        sqlText += " VATName,";
                        sqlText += " UOMPrice,";
                        sqlText += " CConversionDate,";
                        sqlText += " Weight";
                        if (Master.TransactionType == "Credit" || Master.TransactionType == "Debit")
                        {
                            sqlText += ", ReturnTransactionType ";
                        }
                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@MasterSalesInvoiceNo,";
                        sqlText += "@ItemInvoiceLineNo, ";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@ItemPromotionalQuantity,";
                        sqlText += "@ItemSalesPrice,";
                        sqlText += "@ItemNBRPrice,";
                        sqlText += "@AvgRate,";
                        sqlText += "@ItemUOM,";
                        sqlText += "@ItemVATRate,";
                        sqlText += "@ItemVATAmount,";
                        sqlText += "@ItemValueOnly,";
                        sqlText += "@ItemSubTotal,";
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@ItemSD,";
                        sqlText += "@ItemSDAmount,";
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
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemDiscountAmount,";
                        sqlText += "@ItemDiscountedNBRPrice,";
                        sqlText += "@ItemDollerValue,";
                        sqlText += "@ItemCurrencyValue,";
                        sqlText += "@ItemVatName,";
                        sqlText += "@ItemUOMPrice, ";
                        sqlText += "@ItemCConversionDate ,";
                        sqlText += "@ItemWeight ";
                        if (Master.TransactionType == "Credit" || Master.TransactionType == "Debit")
                        {
                            sqlText += ",@ItemReturnTransactionType";
                        }
                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemPromotionalQuantity", Item.PromotionalQuantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSalesPrice", Item.SalesPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemNBRPrice", Item.NBRPrice);
                        cmdInsDetail.Parameters.AddWithValue("@AvgRate", AvgRate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATAmount", Item.VATAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemValueOnly", Item.ValueOnly ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSD", Item.SD);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSDAmount", Item.SDAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSaleTypeD", Item.SaleTypeD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemPreviousSalesInvoiceNoD ", Item.PreviousSalesInvoiceNoD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemTradingD", Item.TradingD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemNonStockD", Item.NonStockD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemTradingMarkUp", Item.TradingMarkUp);
                        cmdInsDetail.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemType", Item.Type ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemDollerValue", Item.DollerValue);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCurrencyValue", Item.CurrencyValue);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVatName", Item.VatName ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCConversionDate", Item.CConversionDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemWeight", Item.Weight ?? Convert.DBNull);

                        if (Master.TransactionType == "Credit" || Master.TransactionType == "Debit")
                        {
                            cmdInsDetail.Parameters.AddWithValue("@ItemReturnTransactionType", Item.ReturnTransactionType);
                        }

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
                        #region Transaction is Wastage
                        if (Master.TransactionType == "Wastage")
                        {
                            #region Update to Receive
                            sqlText = "";
                            sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo=@MasterSalesInvoiceNo";
                            sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                            SqlCommand cmdFindIdReceive = new SqlCommand(sqlText, currConn);
                            cmdFindIdReceive.Transaction = transaction;

                            cmdFindIdReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                            IDExist = (int)cmdFindIdReceive.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                            }
                            sqlText = "";
                            sqlText += " update ReceiveDetails set ";
                            sqlText += " ReceiveLineNo      =@ItemInvoiceLineNo,";
                            ////sqlText += " ItemNo             =@ItemItemNo,";
                            sqlText += " Quantity           =@ItemQuantity,";
                            sqlText += " CostPrice          =@CostPrice,";
                            sqlText += " NBRPrice           =@ItemNBRPrice,";
                            sqlText += " UOM                =@ItemUOM,";
                            sqlText += " SubTotal           =@ItemNBRPriceItemQuantity ,";
                            sqlText += " Comments           =@ItemCommentsD,";
                            sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                            sqlText += " ReceiveDateTime    =@MasterInvoiceDateTime,";
                            sqlText += " TransactionType    =@MasterTransactionType,";
                            sqlText += " ReceiveReturnId    =@MasterReturnId,";
                            sqlText += " DiscountAmount     =@ItemDiscountAmount,";
                            sqlText += " DiscountedNBRPrice =@ItemDiscountedNBRPrice,";
                            sqlText += " UOMQty             =@ItemUOMQty,";
                            sqlText += " UOMPrice           =@ItemUOMPrice,";
                            sqlText += " UOMc               =@ItemUOMc,";
                            sqlText += " UOMn               =@ItemUOMn,";
                            sqlText += " VATName            =@ItemVatName,";
                            sqlText += " Post               =@MasterPost";
                            sqlText += " where  ReceiveNo   =@MasterSalesInvoiceNo ";
                            sqlText += " and ItemNo         =@ItemItemNo";
                            //sqlText += "'" + Master.@Post + "'";
                            SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                            cmdInsertReceive.Transaction = transaction;

                            cmdInsertReceive.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                            ////cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertReceive.Parameters.AddWithValue("@CostPrice", Item.NBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPrice", Item.NBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPriceItemQuantity", Item.NBRPrice * Item.Quantity);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemVatName", Item.VatName ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                            transResult = (int)cmdInsertReceive.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableToUpdateReceive);
                            }
                            #endregion Update to Receive
                            #region Update Receive Header

                            sqlText = "";
                            sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                            sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                            sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                            sqlText += " where ReceiveHeaders.ReceiveNo=@MasterSalesInvoiceNo ";

                            SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                            cmdUpdateReceive.Transaction = transaction;

                            cmdUpdateReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                            transResult = (int)cmdUpdateReceive.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableToUpdateReceive);
                            }
                            #endregion Update Receive
                        }
                        #endregion Transaction is Wastage

                        #region Transaction is Trading

                        if (
                            Master.TransactionType == "Trading"
                            || Master.TransactionType == "ExportTradingTender"
                            || Master.TransactionType == "TradingTender"
                            || Master.TransactionType == "ExportTrading"

                            || Master.TransactionType == "Service"
                            || Master.TransactionType == "ExportService"
                            )
                        {
                            #region Update to Issue
                            sqlText = "";
                            sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterSalesInvoiceNo ";
                            sqlText += " AND ItemNo=@ItemItemNo";
                            SqlCommand cmdFindIdIssue = new SqlCommand(sqlText, currConn);
                            cmdFindIdIssue.Transaction = transaction;

                            cmdFindIdIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdFindIdIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            IDExist = (int)cmdFindIdIssue.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                            }

                            sqlText = "";
                            sqlText += " update IssueDetails set";
                            sqlText += " IssueLineNo        =@ItemInvoiceLineNo,";
                            sqlText += " Quantity           =@ItemQuantity,";
                            sqlText += " uom                =@ItemUOM,";
                            sqlText += " CostPrice          =@AvgRate,";
                            sqlText += " SubTotal           =@AvgRateItemQuantity,";
                            sqlText += " Comments           =@ItemCommentsD,";
                            sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                            sqlText += " IssueDateTime      =@MasterInvoiceDateTime,";
                            sqlText += " TransactionType    =@MasterTransactionType,";
                            sqlText += " IssueReturnId      =@MasterReturnId,";
                            sqlText += " DiscountAmount     =@ItemDiscountAmount,";
                            sqlText += " DiscountedNBRPrice =@ItemDiscountedNBRPrice,";
                            sqlText += " UOMQty             =@ItemUOMQty,";
                            sqlText += " UOMPrice           =@ItemUOMPrice,";
                            sqlText += " UOMc               =@ItemUOMc,";
                            sqlText += " UOMn               =@ItemUOMn,";


                            sqlText += " UOMWastage= '0',";

                            sqlText += " Post=@MasterPost";
                            sqlText += " where  IssueNo =@MasterSalesInvoiceNo and ItemNo = @ItemItemNo ";

                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@AvgRate", AvgRate);
                            cmdInsertIssue.Parameters.AddWithValue("@AvgRateItemQuantity", AvgRate * Item.Quantity);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                            transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableToUpdateIssue);
                            }
                            #endregion Update to Issue
                            #region Update Issue Header

                            sqlText = "";
                            sqlText += " update IssueHeaders set ";
                            sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                            sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                            sqlText += " where (IssueHeaders.IssueNo= @MasterSalesInvoiceNo)";

                            SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                            cmdUpdateIssue.Transaction = transaction;

                            cmdUpdateIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                            transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableToUpdateIssue);
                            }
                            #endregion Update Issue Header

                            #region Update to Receive
                            sqlText = "";
                            sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo=@MasterSalesInvoiceNo ";
                            sqlText += " AND ItemNo=@ItemItemNo";
                            SqlCommand cmdFindIdReceive = new SqlCommand(sqlText, currConn);
                            cmdFindIdReceive.Transaction = transaction;

                            cmdFindIdReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdFindIdReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            IDExist = (int)cmdFindIdReceive.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                            }
                            sqlText = "";
                            sqlText += " update ReceiveDetails set ";
                            sqlText += " ReceiveLineNo      = @ItemInvoiceLineNo,";
                            ////sqlText += " ItemNo             = @ItemItemNo,";
                            sqlText += " Quantity           = @ItemQuantity,";
                            sqlText += " CostPrice          = @CostPrice,";
                            sqlText += " NBRPrice           = @ItemNBRPrice,";
                            sqlText += " UOM                = @ItemUOM,";
                            sqlText += " SubTotal           = @ItemNBRPriceItemQuantity ,";
                            sqlText += " Comments           = @ItemCommentsD,";
                            sqlText += " LastModifiedBy     = @MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn     = @MasterLastModifiedOn,";
                            sqlText += " ReceiveDateTime    = @MasterInvoiceDateTime,";
                            sqlText += " TransactionType    = @MasterTransactionType,";
                            sqlText += " ReceiveReturnId    = @MasterReturnId,";
                            sqlText += " DiscountAmount     = @ItemDiscountAmount,";
                            sqlText += " DiscountedNBRPrice = @ItemDiscountedNBRPrice,";
                            sqlText += " UOMQty             = @ItemUOMQty,";
                            sqlText += " UOMPrice           = @ItemUOMPrice,";
                            sqlText += " UOMc               = @ItemUOMc,";
                            sqlText += " UOMn               = @ItemUOMn,";
                            sqlText += " VATName            = @ItemVatName,";
                            sqlText += " Post               = @MasterPost";
                            sqlText += " where  ReceiveNo   = @MasterSalesInvoiceNo ";
                            sqlText += " and ItemNo         = @ItemItemNo";
                            SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                            cmdInsertReceive.Transaction = transaction;

                            cmdInsertReceive.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                            ////cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertReceive.Parameters.AddWithValue("@CostPrice", Item.NBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPrice", Item.NBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPriceItemQuantity", Item.NBRPrice * Item.Quantity);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemVatName", Item.VatName ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                            transResult = (int)cmdInsertReceive.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableToUpdateReceive);
                            }
                            #endregion Update to Receive
                            #region Update Receive Header

                            sqlText = "";
                            sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                            sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                            sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                            sqlText += " where ReceiveHeaders.ReceiveNo=@MasterSalesInvoiceNo ";

                            SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                            cmdUpdateReceive.Transaction = transaction;

                            cmdUpdateReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                            transResult = (int)cmdUpdateReceive.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableToUpdateReceive);
                            }
                            #endregion Update Receive

                        }
                        #endregion Transaction is Trading

                        #region Transaction is TollIssue

                        else if (Master.TransactionType == "TollIssue")
                        {
                            #region Update to Issue
                            sqlText = "";
                            sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterSalesInvoiceNo ";
                            sqlText += " AND ItemNo=@ItemItemNo ";
                            SqlCommand cmdFindIdIssue = new SqlCommand(sqlText, currConn);
                            cmdFindIdIssue.Transaction = transaction;

                            cmdFindIdIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdFindIdIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            IDExist = (int)cmdFindIdIssue.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                            }
                            AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                            decimal subTot = AvgRate * Item.Quantity;
                            subTot = FormatingNumeric(subTot, IssuePlaceAmt);

                            sqlText = "";
                            sqlText += " update IssueDetails set";
                            sqlText += " IssueLineNo        =@ItemInvoiceLineNo,";
                            sqlText += " Quantity           =@ItemQuantity,";
                            sqlText += " uom                =@ItemUOM,";
                            sqlText += " UOMQty             =@ItemUOMQty,";
                            sqlText += " UOMPrice           =@ItemUOMPrice,";
                            sqlText += " UOMc               =@ItemUOMc,";
                            sqlText += " UOMn               =@ItemUOMn,";
                            sqlText += " CostPrice          =@AvgRate,";
                            sqlText += " SubTotal           =@subTot,";
                            sqlText += " Comments           =@ItemCommentsD,";
                            sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                            sqlText += " IssueDateTime      =@MasterInvoiceDateTime,";
                            sqlText += " TransactionType    =@MasterTransactionType,";
                            sqlText += " IssueReturnId      =@MasterReturnId,";
                            sqlText += " DiscountAmount     =@ItemDiscountAmount,";
                            sqlText += " DiscountedNBRPrice =@ItemDiscountedNBRPrice,";
                            sqlText += " UOMWastage= '0',";
                            sqlText += " Post               =@MasterPost";
                            sqlText += " where  IssueNo     =@MasterSalesInvoiceNo and ItemNo = @ItemItemNo";

                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@AvgRate", AvgRate);
                            cmdInsertIssue.Parameters.AddWithValue("@subTot", subTot);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                            transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableToUpdateIssue);
                            }
                            #endregion Update to Issue
                            #region Update Issue Header

                            sqlText = "";
                            sqlText += " update IssueHeaders set ";
                            sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                            sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                            sqlText += " where (IssueHeaders.IssueNo=@MasterSalesInvoiceNo)";

                            SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                            cmdUpdateIssue.Transaction = transaction;

                            cmdUpdateIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                            int UpdateIssue = (int)cmdUpdateIssue.ExecuteNonQuery();

                            if (UpdateIssue <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableToUpdateIssue);
                            }
                            #endregion Update Issue Header

                        }
                        #endregion Transaction is TollIssue

                        #region Transaction is InternalIssue

                        else if (Master.TransactionType == "InternalIssue")
                        {
                            decimal InternalIssuePrice = productDal.GetLastNBRPriceFromBOM(Item.ItemNo, "VAT 1 (Internal Issue)", Master.InvoiceDateTime, currConn, transaction);

                            AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                            decimal subTot = AvgRate * Item.Quantity;
                            subTot = FormatingNumeric(subTot, IssuePlaceAmt);

                            #region Update to Issue
                            sqlText = "";
                            sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterSalesInvoiceNo";
                            sqlText += " AND ItemNo=@ItemItemNo";
                            SqlCommand cmdFindIdIssue = new SqlCommand(sqlText, currConn);
                            cmdFindIdIssue.Transaction = transaction;

                            cmdFindIdIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdFindIdIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            IDExist = (int)cmdFindIdIssue.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                            }

                            decimal tot = FormatingNumeric(AvgRate * Item.Quantity, IssuePlaceAmt);
                            sqlText = "";
                            sqlText += " update IssueDetails set";
                            sqlText += " IssueLineNo        =@ItemInvoiceLineNo,";
                            sqlText += " Quantity           =@ItemQuantity,";
                            sqlText += " uom                =@ItemUOM,";
                            sqlText += " UOMQty             =@ItemUOMQty,";
                            sqlText += " UOMPrice           =@AvgRate,";
                            sqlText += " UOMc               =@ItemUOMc,";
                            sqlText += " UOMn               =@ItemUOMn,";
                            sqlText += " CostPrice          =@AvgRate,";
                            sqlText += " SubTotal           =@subTot,";
                            sqlText += " Comments           =@ItemCommentsD,";
                            sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                            sqlText += " IssueDateTime      =@MasterInvoiceDateTime,";
                            sqlText += " TransactionType    =@MasterTransactionType,";
                            sqlText += " IssueReturnId      =@MasterReturnId,";
                            sqlText += " DiscountAmount     =@ItemDiscountAmount,";
                            sqlText += " DiscountedNBRPrice =@ItemDiscountedNBRPrice,";
                            sqlText += " Post               =@MasterPost";
                            sqlText += " where  IssueNo     =@MasterSalesInvoiceNo and ItemNo =@ItemItemNo";

                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertIssue.Parameters.AddWithValue("@AvgRte", AvgRate);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@AvgRate", AvgRate);
                            cmdInsertIssue.Parameters.AddWithValue("@subTot", subTot);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                            transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableToUpdateIssue);
                            }
                            #endregion Update to Issue
                            #region Update Issue Header

                            sqlText = "";
                            sqlText += " update IssueHeaders set ";
                            sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                            sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                            sqlText += " where (IssueHeaders.IssueNo=@MasterSalesInvoiceNo )";

                            SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                            cmdUpdateIssue.Transaction = transaction;

                            cmdUpdateIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                            transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableToUpdateIssue);
                            }
                            #endregion Update Issue Header

                            #region Update to Receive
                            sqlText = "";
                            sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo=@MasterSalesInvoiceNo ";
                            sqlText += " AND ItemNo=@ItemItemNo";
                            SqlCommand cmdFindIdReceive = new SqlCommand(sqlText, currConn);
                            cmdFindIdReceive.Transaction = transaction;

                            cmdUpdateIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdUpdateIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            IDExist = (int)cmdFindIdReceive.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                            }
                            sqlText = "";
                            sqlText += " update ReceiveDetails set ";
                            sqlText += " ReceiveLineNo      = @ItemInvoiceLineNo,";
                            ////sqlText += " ItemNo             = @ItemItemNo,";
                            sqlText += " Quantity           = @ItemQuantity,";
                            sqlText += " CostPrice          = @InternalIssuePrice,";
                            sqlText += " NBRPrice           = @InternalIssuePrice,";
                            sqlText += " UOM                = @ItemUOM,";
                            sqlText += " SubTotal           = @InternalIssuePriceItemQuantity ,";
                            sqlText += " Comments           = @ItemCommentsD,";
                            sqlText += " LastModifiedBy     = @MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn     = @MasterLastModifiedOn,";
                            sqlText += " ReceiveDateTime    = @MasterInvoiceDateTime,";
                            sqlText += " TransactionType    = @MasterTransactionType,";
                            sqlText += " ReceiveReturnId    = @MasterReturnId,";
                            sqlText += " DiscountAmount     = @ItemDiscountAmount,";
                            sqlText += " DiscountedNBRPrice = @ItemDiscountedNBRPrice,";
                            sqlText += " UOMQty             = @ItemUOMQty,";
                            sqlText += " UOMPrice           = @ItemUOMPrice,";
                            sqlText += " UOMc               = @ItemUOMc,";
                            sqlText += " UOMn               = @ItemUOMn,";
                            sqlText += " VATName            = @ItemVatName,";
                            sqlText += " Post               = @MasterPost";
                            sqlText += " where  ReceiveNo   = @MasterSalesInvoiceNo ";
                            sqlText += " and ItemNo         = @ItemItemNo";

                            SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                            cmdInsertReceive.Transaction = transaction;

                            cmdInsertReceive.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                            ////cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                            cmdInsertReceive.Parameters.AddWithValue("@InternalIssuePrice", InternalIssuePrice);
                            cmdInsertReceive.Parameters.AddWithValue("@InternalIssuePrice", InternalIssuePrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@InternalIssuePriceItemQuantity", InternalIssuePrice * Item.Quantity);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemVatName", Item.VatName ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                            transResult = (int)cmdInsertReceive.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableToUpdateReceive);
                            }
                            #endregion Update to Receive
                            #region Update Receive Header

                            sqlText = "";
                            sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                            sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                            sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                            sqlText += " where ReceiveHeaders.ReceiveNo=@MasterSalesInvoiceNo ";

                            SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                            cmdUpdateReceive.Transaction = transaction;

                            cmdUpdateReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                            transResult = (int)cmdUpdateReceive.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableToUpdateReceive);
                            }
                            #endregion Update Receive

                        }
                        #endregion Transaction is InternalIssue

                        #region Update only DetailTable

                        sqlText = "";

                        sqlText += " update SalesInvoiceDetails set ";

                        sqlText += " InvoiceLineNo          =@ItemInvoiceLineNo ,";
                        sqlText += " Quantity               =@ItemQuantity ,";
                        sqlText += " PromotionalQuantity    =@ItemPromotionalQuantity ,";
                        sqlText += " SalesPrice             =@ItemSalesPrice ,";
                        sqlText += " NBRPrice               =@ItemNBRPrice ,";
                        sqlText += " AVGPrice               =@AvgRate ,";
                        sqlText += " UOM                    =@ItemUOM ,";
                        sqlText += " VATRate                =@ItemVATRate ,";
                        sqlText += " VATAmount              =@ItemVATAmount ,";
                        sqlText += " ValueOnly              =@ItemValueOnly ,";
                        sqlText += " SubTotal               =@ItemSubTotal ,";
                        sqlText += " Comments               =@ItemCommentsD ,";
                        sqlText += " LastModifiedBy         =@MasterLastModifiedBy ,";
                        sqlText += " LastModifiedOn         =@MasterLastModifiedOn ,";
                        sqlText += " SD                     =@ItemSD ,";
                        sqlText += " SDAmount               =@ItemSDAmount ,";
                        sqlText += " SaleType               =@ItemSaleTypeD ,";
                        sqlText += " PreviousSalesInvoiceNo =@MasterPreviousSalesInvoiceNo ,";
                        sqlText += " Trading                =@ItemTradingD ,";
                        sqlText += " NonStock               =@ItemNonStockD ,";
                        sqlText += " TradingMarkUp          =@ItemTradingMarkUp ,";
                        sqlText += " InvoiceDateTime        =@MasterInvoiceDateTime ,";
                        sqlText += " UOMQty                 =@ItemUOMQty ,";
                        sqlText += " UOMn                   =@ItemUOMn ,";
                        sqlText += " UOMc                   =@ItemUOMc ,";
                        sqlText += " UOMPrice               =@ItemUOMPrice ,";
                        sqlText += " Type                   =@ItemType ,";
                        sqlText += " TransactionType        =@MasterTransactionType ,";
                        sqlText += " SaleReturnId           =@MasterReturnId ,";
                        sqlText += " DiscountAmount         =@ItemDiscountAmount,";
                        sqlText += " DiscountedNBRPrice     =@ItemDiscountedNBRPrice,";
                        sqlText += " DollerValue            =@ItemDollerValue,";
                        sqlText += " CurrencyValue          =@ItemCurrencyValue,";
                        sqlText += " VATName                =@ItemVatName,";
                        sqlText += " Post                   =@MasterPost,";
                        sqlText += " CConversionDate        =@ItemCConversionDate,";
                        sqlText += " Weight                 =@ItemWeight";

                        if (Master.TransactionType == "Credit" || Master.TransactionType == "Debit")
                        {
                            sqlText += ", ReturnTransactionType =@ItemReturnTransactionType";
                        }

                        sqlText += " where  SalesInvoiceNo  =@MasterSalesInvoiceNo";
                        sqlText += " and ItemNo             =@ItemItemNo";



                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemPromotionalQuantity", Item.PromotionalQuantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSalesPrice", Item.SalesPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemNBRPrice", Item.NBRPrice);
                        cmdInsDetail.Parameters.AddWithValue("@AvgRate", AvgRate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATAmount", Item.VATAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemValueOnly", Item.ValueOnly ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSD", Item.SD);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSDAmount", Item.SDAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSaleTypeD", Item.SaleTypeD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPreviousSalesInvoiceNo", Master.PreviousSalesInvoiceNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemTradingD", Item.TradingD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemNonStockD", Item.NonStockD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemTradingMarkUp", Item.TradingMarkUp);
                        cmdInsDetail.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemType", Item.Type ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemDiscountAmount", Item.DiscountAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemDiscountedNBRPrice", Item.DiscountedNBRPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemDollerValue", Item.DollerValue);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCurrencyValue", Item.CurrencyValue);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVatName", Item.VatName ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCConversionDate", Item.CConversionDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemWeight", Item.Weight ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                        if (Master.TransactionType == "Credit" || Master.TransactionType == "Debit")
                        {
                            cmdInsDetail.Parameters.AddWithValue("@ItemReturnTransactionType", Item.ReturnTransactionType);
                        }


                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable



                        #endregion Update Issue and Receive if Transaction is not Other
                    }

                    #endregion Find Transaction Mode Insert or Update

                    lno++;
                }// foreach (var Item in Details.ToList())
                #region Remove row
                sqlText = "";
                sqlText += " SELECT  distinct ItemNo";
                sqlText += " from SalesInvoiceDetails WHERE SalesInvoiceNo=@MasterSalesInvoiceNo";

                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                cmdRIFB.Transaction = transaction;

                cmdRIFB.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB);
                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var p = pItem["ItemNo"].ToString();

                    //var tt = Details.Find(x => x.ItemNo == p);
                    var tt = Details.Count(x => x.ItemNo.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM SalesInvoiceDetails ";
                        sqlText += " WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                        sqlText += " AND ItemNo='" + p + "'";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                    }

                }
                #endregion Remove row


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)

                #region Insert into Export
                if (
                    Master.TransactionType == "Export"
                   || Master.TransactionType == "ExportTender"
                   || Master.TransactionType == "ExportTrading"
                   || Master.TransactionType == "ExportServiceNS"
                   || Master.TransactionType == "ExportService"
                   || Master.TransactionType == "ExportPackage"
                   || Master.TransactionType == "ExportTradingTender"
                    )
                {
                    #region Validation for Export

                    if (ExportDetails.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                    }

                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "delete SalesInvoiceHeadersExport where SalesInvoiceNo=@MasterSalesInvoiceNo";
                    SqlCommand cmdFindId1 = new SqlCommand(sqlText, currConn);
                    cmdFindId1.Transaction = transaction;

                    cmdFindId1.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

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
                        sqlText += "@MasterSalesInvoiceNo,";
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
                        sqlText += "@MasterLastModifiedOn ";

                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportSaleLineNo", ItemExport.SaleLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportRefNo", ItemExport.RefNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportDescription", ItemExport.Description ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportQuantityE", ItemExport.QuantityE ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportGrossWeight", ItemExport.GrossWeight ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportNetWeight", ItemExport.NetWeight ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportNumberFrom", ItemExport.NumberFrom ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportNumberTo", ItemExport.NumberTo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportCommentsE", ItemExport.CommentsE ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);

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
                #region Tracking
                if (Trackings != null && Trackings.Count > 0)
                {
                    for (int i = 0; i < Trackings.Count; i++)
                    {
                        if (Master.TransactionType == "Credit" || Master.TransactionType == "Debit")
                        {
                            if (Trackings[i].ReturnSale == "Y")
                            {
                                Trackings[i].ReturnSaleID = Master.SalesInvoiceNo;
                                Trackings[i].ReturnType = Master.TransactionType;
                            }

                        }
                        if (Trackings[i].IsSale == "Y")
                        {
                            Trackings[i].SaleInvoiceNo = Master.SalesInvoiceNo;
                        }
                        else
                        {
                            Trackings[i].SaleInvoiceNo = "";
                        }

                    }
                    string trackingUpdate = string.Empty;
                    TrackingDAL trackingDal = new TrackingDAL();
                    trackingUpdate = trackingDal.TrackingUpdate(Trackings, transaction, currConn);

                    if (trackingUpdate == "Fail")
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, "Tracking Information not added.");
                    }
                }
                #endregion
                #region TrackingWithSale
                bool TrackingWithSale = Convert.ToBoolean(commonDal.settingValue("Purchase", "TrackingWithSale") == "Y" ? true : false);
                bool TrackingWithSaleFIFO = Convert.ToBoolean(commonDal.settingValue("Purchase", "TrackingWithSaleFIFO") == "Y" ? true : false);
                int NumberOfItems = Convert.ToInt32(commonDal.settingValue("Sale", "NumberOfItems"));
                if (TrackingWithSale)
                {
                    sqlText = "";
                    sqlText += " update PurchaseSaleTrackings set  ";
                    sqlText += " IsSold= '0' ,";
                    sqlText += " SalesInvoiceNo= '0' ,";
                    sqlText += " SaleInvoiceDateTime= '01/01/1900' ";
                    sqlText += " where  SalesInvoiceNo=@MasterSalesInvoiceNo ";

                    SqlCommand cmdUpdate1 = new SqlCommand(sqlText, currConn);
                    cmdUpdate1.Transaction = transaction;

                    cmdUpdate1.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                    //transResult = (int)cmdUpdate1.ExecuteNonQuery();
                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                    //}


                    foreach (var Item in Details.ToList())
                    {
                        sqlText += " '" + Item.InvoiceLineNo + "', ";
                        DataTable tracDt = new DataTable();
                        sqlText = "";
                        sqlText += @" select top " + Convert.ToInt32(Item.Quantity) + " * from PurchaseSaleTrackings";
                        sqlText += @" where IsSold=0";
                        sqlText += @" and ItemNo=@ItemItemNo ";
                        if (TrackingWithSaleFIFO)
                        {
                            sqlText += @" order by id asc ";
                        }
                        else
                        {
                            sqlText += @" order by id desc ";
                        }
                        SqlCommand cmdRIFB1 = new SqlCommand(sqlText, currConn);
                        cmdRIFB1.Transaction = transaction;

                        cmdRIFB1.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                        SqlDataAdapter reportDataAdapt1 = new SqlDataAdapter(cmdRIFB1);
                        reportDataAdapt1.Fill(tracDt);
                        if (Item.Quantity > tracDt.Rows.Count)
                        {
                            throw new ArgumentNullException("Stock not available", "Stock not available");
                        }
                        foreach (DataRow itemTrac in tracDt.Rows)
                        {
                            sqlText = "";
                            sqlText += " update PurchaseSaleTrackings set  ";
                            sqlText += " IsSold= '1' ,";
                            sqlText += " SalesInvoiceNo= @MasterSalesInvoiceNo  ,";
                            sqlText += " SaleInvoiceDateTime=@MasterInvoiceDateTime ";
                            sqlText += " where  Id= '" + itemTrac["Id"] + "' ";

                            SqlCommand cmdUpdate3 = new SqlCommand(sqlText, currConn);
                            cmdUpdate3.Transaction = transaction;

                            cmdUpdate3.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdUpdate3.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime);

                            transResult = (int)cmdUpdate3.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                            }
                        }
                    }
                    ReportDSDAL rds = new ReportDSDAL();
                    DataSet ds = new DataSet();
                    ds = rds.VAT11ReportCommercialImporterNew(Master.SalesInvoiceNo, "N", "N", currConn, transaction);
                    if (ds.Tables[0].Rows.Count > NumberOfItems)
                    {
                        throw new ArgumentNullException("Number of Items in a Invoice exist", "Number of Items in a Invoice exist");
                    }
                }
                #endregion TrackingWithSale

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from SalesInvoiceHeaders WHERE SalesInvoiceNo=@MasterSalesInvoiceNo";
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
                retResults[2] = Master.Id;
                retResults[3] = PostStatus;
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
       
        private void SetDefaultValue(SaleMasterVM vm)
        {
            //if (string.IsNullOrWhiteSpace(vm.hsdescription))
            //{
            //    vm.ProductDescription = "-";
            //}
            if (string.IsNullOrWhiteSpace(vm.SerialNo))
            {
                vm.SerialNo = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.Comments))
            {
                vm.Comments = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.DeliveryAddress1))
            {
                vm.DeliveryAddress1 = "-";
            }
            if(string.IsNullOrWhiteSpace(vm.DeliveryAddress2))
            {
                vm.DeliveryAddress2 = "-";
            }
            if(string.IsNullOrWhiteSpace(vm.DeliveryAddress3))
            {
                vm.DeliveryAddress3 = "-";
            }
        }
        
        
        
        public string[] SalesPost(SaleMasterVM Master, List<SaleDetailVM> Details, List<TrackingVM> Trackings)
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
                sqlText += " LastModifiedBy= @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn= @MasterLastModifiedOn ,";
                sqlText += " Post= @MasterPost ";
                sqlText += " where  SalesInvoiceNo=@MasterSalesInvoiceNo ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                }
                #endregion update Header

                #region Transaction Not Other
                #region Transaction is Wastage
                if (Master.TransactionType == "Wastage")
                {
                    #region update Receive
                    sqlText = "";


                    sqlText += " update ReceiveHeaders set";
                    sqlText += " LastModifiedBy=@MasterLastModifiedBy ,";
                    sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                    sqlText += " Post=@MasterPost ";
                    sqlText += " where  ReceiveNo = @MasterSalesInvoiceNo";

                    SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                    cmdUpdateReceive.Transaction = transaction;

                    cmdUpdateReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);

                    transResult = (int)cmdUpdateReceive.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                    }
                    #endregion update Receive
                }
                #endregion Transaction is Wastage
                #region Transaction is TollReceive

                if (Master.TransactionType == "InternalIssue"
                    || Master.TransactionType == "Service"
                    || Master.TransactionType == "ExportService"
                    || Master.TransactionType == "Trading"
                    || Master.TransactionType == "ExportTradingTender"
                    || Master.TransactionType == "TradingTender"
                    || Master.TransactionType == "ExportTrading"
                    )
                {
                    #region update Issue

                    sqlText = "";

                    sqlText += " update IssueHeaders set ";
                    sqlText += " LastModifiedBy= @MasterLastModifiedBy ,";
                    sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                    sqlText += " Post= @MasterPost ";
                    sqlText += " where  IssueNo= @MasterSalesInvoiceNo ";


                    SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                    cmdUpdateIssue.Transaction = transaction;

                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);

                    transResult = (int)cmdUpdateIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost,
                                                        MessageVM.saleMsgPostNotSuccessfully);
                    }

                    #endregion update Issue

                    #region update Receive
                    sqlText = "";


                    sqlText += " update ReceiveHeaders set";
                    sqlText += " LastModifiedBy= @MasterLastModifiedBy ,";
                    sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                    sqlText += " Post= @MasterPost ";
                    sqlText += " where  ReceiveNo = @MasterSalesInvoiceNo";

                    SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, currConn);
                    cmdUpdateReceive.Transaction = transaction;

                    cmdUpdateReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                    cmdUpdateReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);

                    transResult = (int)cmdUpdateReceive.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                    }
                    #endregion update Receive

                }
                #endregion Transaction is TollReceive
                #region Transaction is TollIssue

                else if (Master.TransactionType == "TollIssue")
                {
                    #region update Issue

                    sqlText = "";

                    sqlText += " update IssueHeaders set ";
                    sqlText += " LastModifiedBy= @MasterLastModifiedBy ,";
                    sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                    sqlText += " Post= @MasterPost";
                    sqlText += " where  IssueNo= @MasterSalesInvoiceNo ";


                    SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                    cmdUpdateIssue.Transaction = transaction;

                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                    cmdUpdateIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);

                    transResult = (int)cmdUpdateIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost,
                                                        MessageVM.saleMsgPostNotSuccessfully);
                    }

                    #endregion update Issue


                }
                #endregion Transaction is TollIssue


                #endregion Transaction Not Other


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
                    sqlText += "select COUNT(SalesInvoiceNo) from SalesInvoiceDetails WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                    sqlText += " AND ItemNo=@ItemItemNo ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgNoDataToPost);
                    }
                    #endregion Find Transaction Mode Insert or Update
                    #region Update only DetailTable

                    else
                    {
                        #region Transaction is Wastage
                        if (Master.TransactionType == "Wastage")
                        {
                            #region Update to Receive
                            sqlText = "";
                            sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo=@MasterSalesInvoiceNo ";
                            sqlText += " AND ItemNo=@ItemItemNo ";
                            SqlCommand cmdFindIdReceive = new SqlCommand(sqlText, currConn);
                            cmdFindIdReceive.Transaction = transaction;

                            cmdFindIdReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdFindIdReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            IDExist = (int)cmdFindIdReceive.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                            }
                            sqlText = "";
                            sqlText += " update ReceiveDetails set ";
                            sqlText += " LastModifiedBy= @MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                            sqlText += " Post= @MasterPost";
                            sqlText += " where  ReceiveNo = @MasterSalesInvoiceNo ";
                            sqlText += " and ItemNo = @ItemItemNo ";
                            SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                            cmdInsertReceive.Transaction = transaction;

                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                            transResult = (int)cmdInsertReceive.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgUnableToPostReceive);
                            }


                            #endregion Update to Receive
                        }
                        #endregion Transaction is Wastage

                        #region Transaction is Trading

                        if (
                            Master.TransactionType == "Trading"
                            || Master.TransactionType == "ExportTradingTender"
                            || Master.TransactionType == "TradingTender"
                            || Master.TransactionType == "ExportTrading"
                            || Master.TransactionType == "Service"
                            || Master.TransactionType == "ExportService"
                            )
                        {
                            #region Update to Issue
                            sqlText = "";
                            sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterSalesInvoiceNo ";
                            sqlText += " AND ItemNo=@ItemItemNo";
                            SqlCommand cmdFindIdIssue = new SqlCommand(sqlText, currConn);
                            cmdFindIdIssue.Transaction = transaction;

                            cmdFindIdIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdFindIdIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            IDExist = (int)cmdFindIdIssue.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                            }

                            sqlText = "";
                            sqlText += " update IssueDetails set";
                            sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                            sqlText += " Post=@MasterPost";
                            sqlText += " where  IssueNo =@MasterSalesInvoiceNo and ItemNo = @ItemItemNo ";

                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                            transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgUnableToPostIssue);
                            }


                            #region Update Item Qty -
                            #region Find Quantity From Products


                            ProductDAL productDal = new ProductDAL();
                            //decimal oldStockI = productDal.StockInHand(Item.ItemNo, Master.InvoiceDateTime, currConn, transaction);
                            decimal oldStockI = Convert.ToDecimal(productDal.AvgPriceNew(
                                                                        Item.ItemNo, Master.InvoiceDateTime,
                                                                        currConn, transaction, true).Rows[0]["Quantity"].ToString());


                            #endregion Find Quantity From Products

                            #region Find Quantity From Transaction

                            sqlText = "";
                            sqlText += "select isnull(Quantity ,0) from IssueDetails ";
                            sqlText += " WHERE ItemNo=@ItemItemNo and IssueNo=@MasterSalesInvoiceNo";
                            SqlCommand cmdTranQtyI = new SqlCommand(sqlText, currConn);
                            cmdTranQtyI.Transaction = transaction;

                            cmdTranQtyI.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdTranQtyI.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            decimal TranQtyI = (decimal)cmdTranQtyI.ExecuteScalar();

                            #endregion Find Quantity From Transaction

                            #region Qty  check and Update
                            if (NegStockAllow == false)
                            {
                                if (TranQtyI > (oldStockI + TranQtyI))
                                {
                                    throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost,
                                                                    MessageVM.saleMsgStockNotAvailablePost);
                                }
                            }

                            #endregion Qty  check and Update


                            #endregion Qty  check and UPDATE -

                            #endregion Update to Issue
                            #region Update to Receive
                            sqlText = "";
                            sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo=@MasterSalesInvoiceNo ";
                            sqlText += " AND ItemNo=@ItemItemNo ";
                            SqlCommand cmdFindIdReceive = new SqlCommand(sqlText, currConn);
                            cmdFindIdReceive.Transaction = transaction;

                            cmdFindIdReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdFindIdReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            IDExist = (int)cmdFindIdReceive.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                            }
                            sqlText = "";
                            sqlText += " update ReceiveDetails set ";
                            sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                            sqlText += " Post= @MasterPos";
                            sqlText += " where  ReceiveNo = @MasterSalesInvoiceNo ";
                            sqlText += " and ItemNo = @ItemItemNo ";
                            SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                            cmdInsertReceive.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                            transResult = (int)cmdInsertReceive.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgUnableToPostReceive);
                            }


                            #endregion Update to Receive
                        }
                        #endregion Transaction is Trading

                        #region Transaction is Tender

                        if (
                            Master.TransactionType == "Tender"
                            || Master.TransactionType == "ExportTradingTender"
                            || Master.TransactionType == "ExportTender"

                            )
                        {
                            #region Update to Issue
                            sqlText = "";
                            sqlText += "select COUNT(tenderid) from TenderDetails WHERE tenderid=@MasterTenderId ";
                            sqlText += " AND ItemNo=@ItemItemNo";
                            SqlCommand cmdFindIdIssue = new SqlCommand(sqlText, currConn);
                            cmdFindIdIssue.Transaction = transaction;

                            cmdFindIdIssue.Parameters.AddWithValue("@MasterTenderId", Master.TenderId);
                            cmdFindIdIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            IDExist = (int)cmdFindIdIssue.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                            }

                            #region Update Item Qty from Tender -
                            #region Find Quantity From Products

                            sqlText = "";
                            sqlText += " select isnull(ISNULL(TenderQty,0)-ISNULL(SaleQty,0),0) from TenderDetails ";
                            sqlText += " WHERE ItemNo=@ItemItemNo and tenderid=@MasterTenderId";
                            SqlCommand cmdOldStockT = new SqlCommand(sqlText, currConn);
                            cmdOldStockT.Transaction = transaction;

                            cmdOldStockT.Parameters.AddWithValue("@MasterTenderId", Master.TenderId);
                            cmdOldStockT.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            decimal oldStockT = (decimal)cmdOldStockT.ExecuteScalar();

                            #endregion Find Quantity From Products

                            #region Find Quantity From Transaction

                            sqlText = "";
                            sqlText += "select isnull(Quantity ,0) from SalesInvoiceDetails ";
                            sqlText += " WHERE ItemNo=@ItemItemNo  and SalesInvoiceNo=@MasterSalesInvoiceNo";
                            SqlCommand cmdTranQtyT = new SqlCommand(sqlText, currConn);
                            cmdTranQtyT.Transaction = transaction;

                            cmdTranQtyT.Parameters.AddWithValue("@MasterTenderId", Master.TenderId);
                            cmdTranQtyT.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            decimal TranQtyT = (decimal)cmdTranQtyT.ExecuteScalar();

                            #endregion Find Quantity From Transaction

                            #region Qty  check and Update

                            if (TranQtyT > oldStockT)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost,
                                                                MessageVM.saleMsgStockNotAvailablePost);
                            }
                            else
                            {
                                sqlText = "";
                                sqlText += " update TenderDetails set ";
                                sqlText += " SaleQty=SaleQty-@TranQtyT";
                                sqlText += " where  ItemNo =@ItemItemNo ";
                                sqlText += " and  TenderId =@MasterTenderId  ";

                                SqlCommand cmdUpdStock = new SqlCommand(sqlText, currConn);
                                cmdUpdStock.Transaction = transaction;

                                cmdUpdStock.Parameters.AddWithValue("@TranQtyT", TranQtyT);
                                cmdUpdStock.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                                cmdUpdStock.Parameters.AddWithValue("@MasterTenderId", Master.TenderId ?? Convert.DBNull);

                                transResult = (int)cmdUpdStock.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost,
                                                                    MessageVM.saleMsgPostNotSuccessfully);
                                }
                            }

                            #endregion Qty  check and Update


                            #endregion Qty  check and UPDATE -
                            #endregion Update to Issue

                        }
                        #endregion Transaction is Tender

                        #region Transaction is TollIssue

                        else if (Master.TransactionType == "TollIssue")
                        {
                            #region Update to Issue
                            sqlText = "";
                            sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterSalesInvoiceNo";
                            sqlText += " AND ItemNo=@ItemItemNo";
                            SqlCommand cmdFindIdIssue = new SqlCommand(sqlText, currConn);
                            cmdFindIdIssue.Transaction = transaction;

                            cmdFindIdIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdFindIdIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            IDExist = (int)cmdFindIdIssue.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                            }

                            sqlText = "";
                            sqlText += " update IssueDetails set";
                            sqlText += " IssueLineNo=@ItemInvoiceLineNo,";
                            sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                            sqlText += " Post=@MasterPost";
                            sqlText += " where  IssueNo =@MasterSalesInvoiceNo and ItemNo = @ItemItemNo";

                            //sqlText += "'" + Master.@Post + "'";
                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                            transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgUnableToPostIssue);
                            }

                            #region Update Item Qty -
                            #region Find Quantity From Products
                            ProductDAL productDal = new ProductDAL();
                            //decimal oldStockI = productDal.StockInHand(Item.ItemNo, Master.InvoiceDateTime, currConn, transaction);
                            decimal oldStockI = Convert.ToDecimal(productDal.AvgPriceNew(
                                                                       Item.ItemNo, Master.InvoiceDateTime,
                                                                       currConn, transaction, true).Rows[0]["Quantity"].ToString());


                            #endregion Find Quantity From Products

                            #region Find Quantity From Transaction

                            sqlText = "";
                            sqlText += "select isnull(Quantity ,0) from IssueDetails ";
                            sqlText += " WHERE ItemNo=@ItemItemNo  and IssueNo=@MasterSalesInvoiceNo";
                            SqlCommand cmdTranQtyI = new SqlCommand(sqlText, currConn);
                            cmdTranQtyI.Transaction = transaction;

                            cmdTranQtyI.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdTranQtyI.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            decimal TranQtyI = (decimal)cmdTranQtyI.ExecuteScalar();

                            #endregion Find Quantity From Transaction

                            #region Qty  check and Update
                            if (NegStockAllow == false)
                            {
                                if (TranQtyI > (oldStockI + TranQtyI))
                                {
                                    throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost,
                                                                    MessageVM.saleMsgStockNotAvailablePost);
                                }
                            }


                            #endregion Qty  check and Update


                            #endregion Qty  check and UPDATE -
                            #endregion Update to Issue
                        }
                        #endregion Transaction is TollIssue

                        #region Transaction is InternalIssue

                        else if (Master.TransactionType == "InternalIssue")
                        {

                            #region Update to Issue
                            sqlText = "";
                            sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterSalesInvoiceNo  ";
                            sqlText += " AND ItemNo=@ItemItemNo ";
                            SqlCommand cmdFindIdIssue = new SqlCommand(sqlText, currConn);
                            cmdFindIdIssue.Transaction = transaction;

                            cmdFindIdIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdFindIdIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            IDExist = (int)cmdFindIdIssue.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                            }

                            sqlText = "";
                            sqlText += " update IssueDetails set";
                            sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                            sqlText += " Post=@MasterPost";
                            sqlText += " where  IssueNo =@MasterSalesInvoiceNo and ItemNo =@ItemItemNo";

                            //sqlText += "'" + Master.@Post + "'";
                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                            transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgUnableToUpdateIssue);
                            }
                            #region Update Item Qty -
                            #region Find Quantity From Products
                            ProductDAL productDal = new ProductDAL();
                            //decimal oldStockI = productDal.StockInHand(Item.ItemNo, Master.InvoiceDateTime, currConn, transaction);
                            decimal oldStockI = Convert.ToDecimal(productDal.AvgPriceNew(
                                                                       Item.ItemNo, Master.InvoiceDateTime,
                                                                       currConn, transaction, true).Rows[0]["Quantity"].ToString());


                            #endregion Find Quantity From Products

                            #region Find Quantity From Transaction

                            sqlText = "";
                            sqlText += "select isnull(Quantity ,0) from IssueDetails ";
                            sqlText += " WHERE ItemNo=@ItemItemNo and IssueNo=@MasterSalesInvoiceNo ";
                            SqlCommand cmdTranQtyI = new SqlCommand(sqlText, currConn);
                            cmdTranQtyI.Transaction = transaction;

                            cmdTranQtyI.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdTranQtyI.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            decimal TranQtyI = (decimal)cmdTranQtyI.ExecuteScalar();

                            #endregion Find Quantity From Transaction

                            #region Qty  check and Update
                            if (NegStockAllow == false)
                            {
                                if (TranQtyI > (oldStockI + TranQtyI))
                                {
                                    throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost,
                                                                    MessageVM.saleMsgStockNotAvailablePost);
                                }
                            }


                            #endregion Qty  check and Update


                            #endregion Qty  check and UPDATE -
                            #endregion Update to Issue
                            #region Update to Receive
                            sqlText = "";
                            sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo=@MasterSalesInvoiceNo  ";
                            sqlText += " AND ItemNo=@ItemItemNo ";
                            SqlCommand cmdFindIdReceive = new SqlCommand(sqlText, currConn);
                            cmdFindIdReceive.Transaction = transaction;

                            cmdFindIdReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdFindIdReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            IDExist = (int)cmdFindIdReceive.ExecuteScalar();
                            if (IDExist <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                            }
                            sqlText = "";
                            sqlText += " update ReceiveDetails set ";
                            sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                            sqlText += " Post=@MasterPost";
                            sqlText += " where  ReceiveNo =@MasterSalesInvoiceNo ";
                            sqlText += " and ItemNo = @ItemItemNo";
                            SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                            cmdInsertReceive.Transaction = transaction;

                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterPost", Master.Post);
                            cmdInsertReceive.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                            cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            transResult = (int)cmdInsertReceive.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgUnableToPostReceive);
                            }
                            #endregion Update to Receive

                        }
                        #endregion Transaction is InternalIssue
                        //Update

                        #region Update only DetailTable

                        sqlText = "";
                        sqlText += " update SalesInvoiceDetails set ";
                        sqlText += " LastModifiedBy=@MasterLastModifiedBy ,";
                        sqlText += " LastModifiedOn=@MasterLastModifiedOn ,";
                        sqlText += " Post=@MasterPost";
                        sqlText += " where  SalesInvoiceNo =@MasterSalesInvoiceNo ";
                        sqlText += " and ItemNo = @ItemItemNo";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                        }
                        else
                        {
                            if (Master.TransactionType != "ServiceNS" && Master.TransactionType != "ExportServiceNS" && Master.TransactionType != "Wastage")
                            {
                                if (Master.SaleType != "Credit")
                                {

                                    #region Check Stock
                                    #region Find Quantity From Query
                                    ProductDAL productDal = new ProductDAL();
                                    //decimal oldStock = productDal.StockInHand(Item.ItemNo, Master.InvoiceDateTime, currConn, transaction);
                                    decimal oldStock = Convert.ToDecimal(productDal.AvgPriceNew(
                                                                       Item.ItemNo, Master.InvoiceDateTime,
                                                                       currConn, transaction, true).Rows[0]["Quantity"].ToString());
                                    #endregion Find Quantity From Query
                                    #region Find Quantity From Transaction

                                    sqlText = "";
                                    sqlText += "select isnull(Quantity ,0) from SalesInvoiceDetails ";
                                    sqlText += " WHERE ItemNo=@ItemItemNo and SalesInvoiceNo= @MasterSalesInvoiceNo ";
                                    SqlCommand cmdTranQty = new SqlCommand(sqlText, currConn);
                                    cmdTranQty.Transaction = transaction;

                                    cmdTranQty.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                                    cmdTranQty.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                                    decimal TranQty = (decimal)cmdTranQty.ExecuteScalar();

                                    #endregion Find Quantity From Transaction
                                    #region Qty  check
                                    if (NegStockAllow == false)
                                    {
                                        if (TranQty > (oldStock + TranQty))
                                        {
                                            DataTable dtProduct = new DataTable();
                                            dtProduct = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                            throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, dtProduct.Rows[0]["ProductName"].ToString()
                                                + " (" + dtProduct.Rows[0]["ProductCode"].ToString() + ") " +
                                                                            MessageVM.saleMsgStockNotAvailablePost);
                                        }
                                    }


                                    #endregion Qty  check
                                    #endregion Check Stock

                                }
                            }



                        }
                        #endregion Qty  check and Update

                    }
                    #endregion Update only DetailTable

                }


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)

                #region Tracking
                if (Trackings != null && Trackings.Count > 0)
                {
                    for (int i = 0; i < Trackings.Count; i++)
                    {
                        if (Trackings[i].SaleInvoiceNo == Master.SalesInvoiceNo)
                        {
                            sqlText = "";

                            sqlText += " update Trackings set  ";
                            sqlText += " LastModifiedBy =@MasterLastModifiedBy, ";
                            sqlText += " LastModifiedOn =@MasterLastModifiedOn, ";
                            sqlText += " SalePost =@MasterPost ";
                            sqlText += " where  SaleInvoiceNo =@MasterSalesInvoiceNo ";
                            sqlText += " and  Heading1 = @TrackingsHeading1  ";

                            SqlCommand cmdUpdateTracking = new SqlCommand(sqlText, currConn);
                            cmdUpdateTracking.Transaction = transaction;

                            cmdUpdateTracking.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                            cmdUpdateTracking.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                            cmdUpdateTracking.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                            cmdUpdateTracking.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                            cmdUpdateTracking.Parameters.AddWithValue("@TrackingsHeading1", Trackings[i].Heading1 ?? Convert.DBNull);

                            transResult = (int)cmdUpdateTracking.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                            }

                        }

                    }
                }


                #endregion



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

        public DataTable SearchSalesHeaderDTNew(string SalesInvoiceNo, string CustomerName,
                   string CustomerGroupName, string VehicleType, string VehicleNo, string SerialNo, string InvoiceDateFrom,
                   string InvoiceDateTo, string SaleType, string Trading, string IsPrint, string transactionType, string Post)
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
                sqlText += "    isnull(Customers.VATRegistrationNo,'0')  VATRegistrationNo,";
                sqlText += " isnull(SalesInvoiceHeaders.LCNumber,'N/A')LCNumber,";
                sqlText += " isnull(SalesInvoiceHeaders.LCDate,'1900/01/01')LCDate,";
                sqlText += " isnull(SalesInvoiceHeaders.LCBank,'N/A')LCBank,";

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
                sqlText += " isnull(SalesInvoiceHeaders.DeliveryChallanNo,'N')DeliveryChallan,";
                sqlText += " isnull(SalesInvoiceHeaders.IsGatePass,'N')IsGatePass,";
                sqlText += " isnull(SalesInvoiceHeaders.Post,'N')Post,";
                sqlText += " isnull(SalesInvoiceHeaders.ImportIDExcel,'')ImportID";
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
                    if (transactionType == "ExportServiceNS")
                    {
                        sqlText += " AND (SalesInvoiceHeaders.transactionType in('ExportServiceNS','ExportServiceNSCredit')) ";

                    }
                    else
                    {
                        sqlText += " AND (SalesInvoiceHeaders.transactionType in('" + transactionType + "')) ";
                    }
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
            int transResult = 0;
            int countId = 0;
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

                sqlText = @"
                            SELECT    
                            SalesInvoiceDetails.SalesInvoiceNo,
                            SalesInvoiceDetails.InvoiceLineNo, 
                            SalesInvoiceDetails.ItemNo,
                            isnull(SalesInvoiceDetails.Quantity,0)Quantity, 
                            
                            isnull(SalesInvoiceDetails.PromotionalQuantity,0)PromotionalQuantity, 
                            isnull(isnull(SalesInvoiceDetails.Quantity,0)-isnull(SalesInvoiceDetails.PromotionalQuantity,0),0)SaleQuantity, 
                                                        
                            isnull(SalesInvoiceDetails.SalesPrice,0)SalesPrice,
                            isnull(SalesInvoiceDetails.NBRPrice,0)NBRPrice,
                            isnull(SalesInvoiceDetails.UOM,'N/A')UOM,
                            isnull(SalesInvoiceDetails.VATRate,0)VATRate ,
                            isnull(SalesInvoiceDetails.VATAmount,0)VATAmount ,
                            isnull(nullif(SalesInvoiceDetails.ValueOnly,''),'N')ValueOnly,
                            isnull(SalesInvoiceDetails.SubTotal,0)SubTotal,
                            isnull(SalesInvoiceDetails.Comments,'N/A')Comments,
                            isnull(Products.ProductName,'N/A')ProductName,
                            isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
                            isnull(SalesInvoiceDetails.SD,0)SD,
                            isnull(SalesInvoiceDetails.SDAmount,0)SDAmount,
                            isnull(SaleType,'VAT'),
                            isnull(SalesInvoiceDetails.PreviousSalesInvoiceNo,SalesInvoiceDetails.SalesInvoiceNo)PreviousSalesInvoiceNo,
                            isnull(SalesInvoiceDetails.Trading,'N')Trading,
                            isnull(SalesInvoiceDetails.NonStock,'N')NonStock,
                            isnull(SalesInvoiceDetails.tradingMarkup,0)tradingMarkup,
                            isnull(SalesInvoiceDetails.Type,'Type')Type,
                            isnull(Products.ProductCode,'N/A')ProductCode,
                            isnull(SalesInvoiceDetails.UOMQty,0)UOMQty,
                            isnull(SalesInvoiceDetails.UOMn,SalesInvoiceDetails.UOM)UOMn,
                            isnull(SalesInvoiceDetails.UOMc,0)UOMc,
                            isnull(SalesInvoiceDetails.UOMPrice,0)UOMPrice,
                            isnull(nullif(SalesInvoiceDetails.VATName,''),'NA')VATName ,
                            isnull(SalesInvoiceDetails.DiscountAmount,0)DiscountAmount,
                            isnull(SalesInvoiceDetails.DiscountedNBRPrice,0)DiscountedNBRPrice,
                            isnull(SalesInvoiceDetails.CurrencyValue,SalesInvoiceDetails.SubTotal)CurrencyValue,
                            isnull(SalesInvoiceDetails.DollerValue,0)DollerValue,
                            isnull(SalesInvoiceDetails.ReturnTransactionType,'')ReturnTransactionType,
                            isnull(SalesInvoiceDetails.Weight,'')Weight,

                            convert (varchar,isnull(SalesInvoiceDetails.CConversionDate,'01/01/1900'),120) CConversionDate
                            

                            FROM  SalesInvoiceDetails LEFT OUTER JOIN
                             Products ON SalesInvoiceDetails.ItemNo = Products.ItemNo               
                            WHERE 
                            (SalesInvoiceDetails.SalesInvoiceNo = @SalesInvoiceNo) 
                            order by InvoiceLineNo
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
                        DataTable dt = sale.AvgPriceNew(vIssueItem, vIssueDate, null, null, false);
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
                sqlText += " and ItemNo =@itemNo";
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
                sqlText += " SubTotal=@avgPrice* UOMQty ,";
                sqlText += " NBRPrice=@avgPrice* UOMc ,";
                sqlText += " CostPrice@avgPrice* UOMc ,";
                sqlText += " IsProcess='Y'";

                sqlText += " where  IssueNo =@invoiceNo ";
                sqlText += " and ItemNo = @itemNo";

                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                cmdInsDetail.Transaction = transaction;

                cmdInsDetail.Parameters.AddWithValue("@avgPrice", avgPrice);
                cmdInsDetail.Parameters.AddWithValue("@invoiceNo", invoiceNo ?? Convert.DBNull);
                cmdInsDetail.Parameters.AddWithValue("@itemNo", itemNo ?? Convert.DBNull);

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

                else if (Master.TransactionType == "Credit")
                {
                    newID = commonDal.TransactionCode("Sale", "Credit", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", Master.InvoiceDateTime, currConn, transaction);
                }


                else if (
                    Master.TransactionType == "Export"
                    || Master.TransactionType == "ExportTrading"
                    || Master.TransactionType == "ExportServiceNS"
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
                    )
                {
                    vehicleId = "0";
                }
                else
                {
                    string vehicleID = "0";
                    sqlText = "";
                    sqlText = sqlText + "select VehicleID from Vehicles WHERE VehicleID=@VehicleID ";

                    SqlCommand cmdExistVehicleId = new SqlCommand(sqlText, currConn);
                    cmdExistVehicleId.Transaction = transaction;

                    cmdExistVehicleId.Parameters.AddWithValue("@VehicleID", Master.VehicleID);

                    string vehicleIDExist = cmdExistVehicleId.ExecuteScalar().ToString();
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
                        sqlText += @"values(
                                     @vehicleID,";
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

                        sqlText += " @MasterCreatedBy,";
                        sqlText += " @MasterCreatedOn,";
                        sqlText += " @MasterLastModifiedBy ,";
                        sqlText += " @MasterLastModifiedOn)";

                        SqlCommand cmdExistVehicleIns = new SqlCommand(sqlText, currConn);
                        cmdExistVehicleIns.Transaction = transaction;

                        cmdExistVehicleIns.Parameters.AddWithValue("@vehicleID", vehicleID ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleType", Master.VehicleType ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);

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
                sqlText += " LCDate,";
                sqlText += " LCBank,";

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
                sqlText += "@MasterTotalAmount,";
                sqlText += "@MasterTotalVATAmount,";
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
                sqlText += "@MasterCurrencyRateFromBDT,";
                sqlText += "@MasterLCDate,";
                sqlText += "@MasterLCBank,";
                sqlText += "@MasterImportID,";
                sqlText += "@MasterPost";
                sqlText += ")";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@newID", newID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCustomerID", Master.CustomerID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDeliveryAddress1", Master.DeliveryAddress1 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDeliveryAddress2", Master.DeliveryAddress2 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDeliveryAddress3", Master.DeliveryAddress3 ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@vehicleId", vehicleId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTotalAmount", commonDal.decimal259(Master.TotalAmount) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTotalVATAmount", commonDal.decimal259(Master.TotalVATAmount) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterSaleType", Master.SaleType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterPreviousSalesInvoiceNo", Master.PreviousSalesInvoiceNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTrading", Master.Trading ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterIsPrint", Master.IsPrint ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTenderId", Master.TenderId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterDeliveryDate", Master.DeliveryDate ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCurrencyID", Master.CurrencyID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCurrencyRateFromBDT", commonDal.decimal259(Master.CurrencyRateFromBDT) ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLCDate", Master.LCDate ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterLCBank", Master.LCBank ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterImportID", Master.ImportID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

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

                    cmdInsertIssue.Parameters.AddWithValue("@newID", newID ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
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

                    #region Insert to Receive

                    sqlText = "";
                    sqlText += " insert into ReceiveHeaders(";
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
                    cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
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
                    cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
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
                    DataTable priceData = productDal.AvgPriceNew(Item.ItemNo, Master.InvoiceDateTime, currConn, transaction, false);
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
                        sqlText += "@newID,";
                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += " 0,";
                        sqlText += "@AvgRate,	";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,	";
                        sqlText += "@AvgRateItemQuantity,";
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
                        sqlText += "@ItemDiscountAmount,";
                        sqlText += "@ItemDiscountedNBRPrice,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";

                        sqlText += ")	";
                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue.Transaction = transaction;

                        cmdInsertIssue.Parameters.AddWithValue("@newID", newID ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", commonDal.decimal259(Item.Quantity));
                        cmdInsertIssue.Parameters.AddWithValue("@AvgRate", commonDal.decimal259(AvgRate));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@AvgRateItemQuantity ", commonDal.decimal259(AvgRate * Item.Quantity));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", commonDal.decimal259(Item.UOMQty));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMPrice", commonDal.decimal259(Item.UOMPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", commonDal.decimal259(Item.UOMc));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn  ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

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
                        sqlText += "@newID,";
                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@CostPrice,";
                        sqlText += "@ItemNBRPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,";
                        sqlText += "@ItemNBRPrice*ItemQuantity,";
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += " 0,0,";
                        sqlText += "@MasterInvoiceDateTime,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@ItemDiscountAmount,";
                        sqlText += "@ItemDiscountedNBRPrice,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";

                        //sqlText += "'" + Master.@Post + "'";
                        sqlText += ")	";
                        SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                        cmdInsertReceive.Transaction = transaction;

                        cmdInsertReceive.Parameters.AddWithValue("@newID", newID);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", commonDal.decimal259(Item.Quantity));
                        cmdInsertReceive.Parameters.AddWithValue("@CostPrice", commonDal.decimal259(Item.NBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPrice", commonDal.decimal259(Item.NBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemNBRPriceItemQuantity", commonDal.decimal259(Item.NBRPrice * Item.Quantity));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", commonDal.decimal259(Item.UOMQty));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMPrice", commonDal.decimal259(Item.UOMPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMc", commonDal.decimal259(Item.UOMc));
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
                        sqlText += " where ReceiveHeaders.ReceiveNo=@newID ";


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
                        sqlText += "@newID,";
                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += " 0,";
                        sqlText += "@AvgRate,";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,	";
                        sqlText += "@AvgRateItemQuantity,";
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
                        sqlText += "@ItemDiscountAmount,";
                        sqlText += "@ItemDiscountedNBRPrice,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";

                        sqlText += ")	";
                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue.Transaction = transaction;

                        cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", commonDal.decimal259(Item.Quantity));
                        cmdInsertIssue.Parameters.AddWithValue("@AvgRate", commonDal.decimal259(AvgRate));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@AvgRateItemQuantity", commonDal.decimal259(AvgRate * Item.Quantity));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", commonDal.decimal259(Item.UOMQty));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMPrice", commonDal.decimal259(Item.UOMPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", commonDal.decimal259(Item.UOMc));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

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
                        sqlText += " where (IssueHeaders.IssueNo= @newID)";

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
                        sqlText += "@newID,";
                        sqlText += "@ItemInvoiceLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity),";
                        sqlText += " 0,";
                        sqlText += "@AvgRate,	";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,	";
                        sqlText += "@AvgRateItemQuantity,";
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
                        sqlText += "@ItemDiscountAmount,";
                        sqlText += "@ItemDiscountedNBRPrice,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";

                        sqlText += ")	";
                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue.Transaction = transaction;

                        cmdInsertIssue.Parameters.AddWithValue("@newID", newID);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemQuantity", commonDal.decimal259(Item.Quantity));
                        cmdInsertIssue.Parameters.AddWithValue("@AvgRate", commonDal.decimal259(AvgRate));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@AvgRateItemQuantity", commonDal.decimal259(AvgRate * Item.Quantity));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMQty", commonDal.decimal259(Item.UOMQty));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMPrice", commonDal.decimal259(Item.UOMPrice));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMc", commonDal.decimal259(Item.UOMc));
                        cmdInsertIssue.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

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
                        sqlText += " where (IssueHeaders.IssueNo= @newID)";

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
                        sqlText += "@ItemQuantity,";
                        sqlText += "@NBRPrice,";
                        sqlText += "@NBRPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	0,";
                        sqlText += "@NBRPriceItemQuantity,";
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += " 0,0,";
                        sqlText += "@MasterInvoiceDateTime,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@ItemDiscountAmount,";
                        sqlText += "@ItemDiscountedNBRPrice,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";

                        sqlText += ")	";
                        SqlCommand cmdInsertReceive = new SqlCommand(sqlText, currConn);
                        cmdInsertReceive.Transaction = transaction;

                        cmdInsertReceive.Parameters.AddWithValue("@newID", newID);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemQuantity", commonDal.decimal259(Item.Quantity));
                        cmdInsertReceive.Parameters.AddWithValue("@NBRPrice", commonDal.decimal259(NBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@NBRPriceItemQuantity", commonDal.decimal259(NBRPrice * Item.Quantity));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMQty", commonDal.decimal259(Item.UOMQty));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMPrice", commonDal.decimal259(Item.UOMPrice));
                        cmdInsertReceive.Parameters.AddWithValue("@ItemUOMc", commonDal.decimal259(Item.UOMc));
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
                        sqlText += " where ReceiveHeaders.ReceiveNo=@newID  ";


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
                    sqlText += " ValueOnly,";
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
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemQuantity,";
                    sqlText += "@ItemPromotionalQuantity,";
                    sqlText += "@ItemSalesPrice,";
                    sqlText += "@ItemNBRPrice,";
                    sqlText += "@AvgRate,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemVATRate),";
                    sqlText += "@ItemVATAmount),";
                    sqlText += "@ItemValueOnly),";
                    sqlText += "@ItemSubTotal),";
                    sqlText += "@ItemCommentsD,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@ItemSD,";
                    sqlText += "@ItemSDAmount,";
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
                    sqlText += "@ItemUOMQty,";
                    sqlText += "@ItemUOMn,";
                    sqlText += "@ItemUOMc,";
                    sqlText += "@ItemDiscountAmount,";
                    sqlText += "@ItemDiscountedNBRPrice,";
                    sqlText += "@ItemDollerValue,";
                    sqlText += "@ItemCurrencyValue,";
                    sqlText += "@ItemUOMPrice ";
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsDetail.Parameters.AddWithValue("@newID", newID);
                    cmdInsDetail.Parameters.AddWithValue("@ItemInvoiceLineNo", Item.InvoiceLineNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", commonDal.decimal259(Item.Quantity));
                    cmdInsDetail.Parameters.AddWithValue("@ItemPromotionalQuantity", commonDal.decimal259(Item.PromotionalQuantity));
                    cmdInsDetail.Parameters.AddWithValue("@ItemSalesPrice", commonDal.decimal259(Item.SalesPrice));
                    cmdInsDetail.Parameters.AddWithValue("@ItemNBRPrice", commonDal.decimal259(Item.NBRPrice));
                    cmdInsDetail.Parameters.AddWithValue("@AvgRate", commonDal.decimal259(AvgRate));
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", commonDal.decimal259(Item.VATRate));
                    cmdInsDetail.Parameters.AddWithValue("@ItemVATAmount", commonDal.decimal259(Item.VATAmount));
                    cmdInsDetail.Parameters.AddWithValue("@ItemValueOnly", commonDal.decimal259(Item.ValueOnly));
                    cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", commonDal.decimal259(Item.SubTotal));
                    cmdInsDetail.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemSD", commonDal.decimal259(Item.SD));
                    cmdInsDetail.Parameters.AddWithValue("@ItemSDAmount", commonDal.decimal259(Item.SDAmount));
                    cmdInsDetail.Parameters.AddWithValue("@ItemSaleTypeD", Item.SaleTypeD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemPreviousSalesInvoiceNoD", Item.PreviousSalesInvoiceNoD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemTradingD", Item.TradingD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemNonStockD", Item.NonStockD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemTradingMarkUp", Item.TradingMarkUp);
                    cmdInsDetail.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemType", Item.Type ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterTransactionType", Master.TransactionType ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", commonDal.decimal259(Item.UOMQty));
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", commonDal.decimal259(Item.UOMc));
                    cmdInsDetail.Parameters.AddWithValue("@ItemDiscountAmount", commonDal.decimal259(Item.DiscountAmount));
                    cmdInsDetail.Parameters.AddWithValue("@ItemDiscountedNBRPrice", commonDal.decimal259(Item.DiscountedNBRPrice));
                    cmdInsDetail.Parameters.AddWithValue("@ItemDollerValue", commonDal.decimal259(Item.DollerValue));
                    cmdInsDetail.Parameters.AddWithValue("@ItemCurrencyValue", commonDal.decimal259(Item.CurrencyValue));
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", commonDal.decimal259(Item.UOMPrice));


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
                        sqlText += " @newID,";
                        sqlText += " @ItemExportSaleLineNo ,";
                        sqlText += " @ItemExportRefNo ,";
                        sqlText += " @ItemExportDescription ,";
                        sqlText += " @ItemExportQuantityE ,";
                        sqlText += " @ItemExportGrossWeight ,";
                        sqlText += " @ItemExportNetWeight ,";
                        sqlText += " @ItemExportNumberFrom ,";
                        sqlText += " @ItemExportNumberTo ,";
                        sqlText += " @ItemExportCommentsE ,";
                        sqlText += " @MasterCreatedBy ,";
                        sqlText += " @MasterCreatedOn ,";
                        sqlText += " @MasterLastModifiedBy ,";
                        sqlText += " @MasterLastModifiedOn";

                        //sqlText += "'" + Master.@Post + "'";
                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@newID", newID);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportSaleLineNo", ItemExport.SaleLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportRefNo", ItemExport.RefNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportDescription", ItemExport.Description ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportQuantityE", ItemExport.QuantityE ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportGrossWeight", ItemExport.GrossWeight ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportNetWeight", ItemExport.NetWeight ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportNumberFrom", ItemExport.NumberFrom ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportNumberTo", ItemExport.NumberTo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemExportCommentsE", ItemExport.CommentsE ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn ?? Convert.DBNull);


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
        public string[] ImportData(DataTable dtSaleM, DataTable dtSaleD, DataTable dtSaleE)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SaleMasterVM saleMaster = new SaleMasterVM();
            List<SaleDetailVM> saleDetails = new List<SaleDetailVM>();
            List<SaleExportVM> saleExport = new List<SaleExportVM>();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string VinvoiceNo = "";
            string VitemCode = "";
            #endregion variable

            #region try
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    currConn.Open();
                    transaction = currConn.BeginTransaction("Checking Das ta");
                }
                CommonImport cImport = new CommonImport();
                CommonDAL commonDal = new CommonDAL();
                ProductDAL productDal = new ProductDAL();

                #region RowCount
                int MRowCount = 0;
                int MRow = dtSaleM.Rows.Count;
                DataTable varDt = new DataTable();
                varDt.TableName = "NewSaleM";
                varDt = dtSaleM.Copy();
                var DatabaseName = commonDal.settings("DatabaseName", "DatabaseName");
                var vTotalPrice = commonDal.settings("Sale", "TotalPrice");
                var saleExistContinue = commonDal.settings("Import", "SaleExistContinue");

                for (int i = 0; i < MRow; i++)
                {
                    string importID = varDt.Rows[i]["ID"].ToString().Trim();
                    var exist = cImport.CheckSaleImportIdExist(importID, currConn, transaction);
                    if (exist.ToLower() == "exist")
                    {
                        if (DatabaseName.ToLower() == "cpb")
                        {
                            if (saleExistContinue.ToLower() == "n")
                            {
                                string msg = "Import Id " + importID.ToString() + "Already Exist in Database";
                                throw new ArgumentNullException(msg);
                            }
                            else
                            {
                                DataRow[] drm = null;//a datarow array
                                DataRow[] drd = null;//a datarow array
                                DataRow[] dre = null;//a datarow array
                                if (dtSaleM != null && dtSaleM.Rows.Count > 0)
                                {
                                    drm = dtSaleM.Select("ID =" + importID); //get the rows with matching condition in arrray
                                    foreach (DataRow row in drm)
                                    {
                                        dtSaleM.Rows.Remove(row);
                                    }
                                }
                                if (dtSaleD != null && dtSaleD.Rows.Count > 0)
                                {
                                    drd = dtSaleD.Select("ID =" + importID); //get the rows with matching condition in arrray
                                    foreach (DataRow row in drd)
                                    {
                                        dtSaleD.Rows.Remove(row);
                                    }
                                }
                                if (dtSaleE != null && dtSaleE.Rows.Count > 0)
                                {
                                    dre = dtSaleE.Select("ID =" + importID); //get the rows with matching condition in arrray
                                    foreach (DataRow row in dre)
                                    {
                                        dtSaleE.Rows.Remove(row);
                                    }
                                }

                                //loop throw the array and deete those rows from datatable

                                //loop throw the array and deete those rows from datatable
                            }
                        }
                    }
                    else
                    {
                        var tt = importID;
                    }

                }
                if (dtSaleM.Rows.Count <= 0)
                {
                    retResults[0] = "Information";
                    retResults[1] = "You do Not Have Data to Import";
                    throw new ArgumentNullException(retResults[1]);

                }
                for (int i = 0; i < dtSaleM.Rows.Count; i++)
                {
                    string importID = dtSaleM.Rows[i]["ID"].ToString();
                    if (!string.IsNullOrEmpty(importID))
                    {
                        MRowCount++;
                    }

                }

                #endregion RowCount

                #region ID in Master or Detail table

                //// in details check
                for (int i = 0; i < MRowCount; i++)
                {
                    string importID = dtSaleM.Rows[i]["ID"].ToString();

                    if (!string.IsNullOrEmpty(importID))
                    {
                        DataRow[] DetailRaws = dtSaleD.Select("ID='" + importID + "'");
                        if (DetailRaws.Length == 0)
                        {
                            throw new ArgumentNullException("The ID " + importID + " is not available in detail table");
                        }

                    }

                }
                //// in master check
                for (int i = 0; i < dtSaleD.Rows.Count; i++)
                {
                    string importID = dtSaleD.Rows[i]["ID"].ToString();

                    if (!string.IsNullOrEmpty(importID))
                    {
                        DataRow[] DetailRaws = dtSaleM.Select("ID='" + importID + "'");
                        if (DetailRaws.Length == 0)
                        {
                            throw new ArgumentNullException("The ID " + importID + " is not available in master table");
                        }

                    }

                }

                #endregion

                #region Double ID in Master

                for (int i = 0; i < MRowCount; i++)
                {
                    string id = dtSaleM.Rows[i]["ID"].ToString();
                    DataRow[] tt = dtSaleM.Select("ID='" + id + "'");
                    if (tt.Length > 1)
                    {
                        throw new ArgumentNullException("you have duplicate master id " + id + " in import file.");
                    }

                }

                #endregion Double ID in Master


                #region Read from settings
                string vPriceDeclaration = commonDal.settings("Sale", "PriceDeclarationForImport");
                bool IsPriceDeclaration = Convert.ToBoolean(vPriceDeclaration == "Y" ? true : false);
                string vNegStockAllow = commonDal.settings("Sale", "NegStockAllow");
                bool isNegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);
                #endregion

                string conversionDate = DateTime.Now.ToString("yyyy-MM-dd");

                #region Find Master Column for Sanofi

                bool IsCompInvoiceNo = false;
                for (int i = 0; i < dtSaleM.Columns.Count; i++)
                {
                    if (dtSaleM.Columns[i].ColumnName.ToString() == "Comp_Invoice_No")
                    {
                        IsCompInvoiceNo = true;
                    }
                }
                ////Find details column for CP
                bool IsColWeight = false;
                for (int i = 0; i < dtSaleD.Columns.Count; i++)
                {
                    if (dtSaleD.Columns[i].ColumnName.ToString() == "Weight")
                    {
                        IsColWeight = true;
                    }
                }

                #endregion
                #region checking from database is exist the information(NULL Check)
                #region Master
                //string CurrencyId = string.Empty;
                //string USDCurrencyId = string.Empty;

                for (int i = 0; i < MRowCount; i++)
                {

                    //var ImportId = dtSaleM.Rows[i]["ID"].ToString().Trim();
                    VinvoiceNo = dtSaleM.Rows[i]["ID"].ToString().Trim();
                    //CurrencyId = string.Empty;
                    //USDCurrencyId = string.Empty;
                    #region Master
                    #region FindCustomerId
                    //cImport.FindCustomerId(dtSaleM.Rows[i]["Customer_Name"].ToString().Trim(),dtSaleM.Rows[i]["Customer_Code"].ToString().Trim(), currConn, transaction);
                    #endregion FindCustomerId

                    #region FindCurrencyId
                    //CurrencyId = cImport.FindCurrencyId(dtSaleM.Rows[i]["Currency_Code"].ToString().Trim(), currConn, transaction);
                    //USDCurrencyId = cImport.FindCurrencyId("USD", currConn, transaction);
                    //cImport.FindCurrencyRateFromBDT(CurrencyId, currConn, transaction);
                    //cImport.FindCurrencyRateBDTtoUSD(USDCurrencyId,conversionDate, currConn, transaction);

                    #endregion FindCurrencyId

                    #region FindTenderId
                    //cImport.FindTenderId(dtSaleM.Rows[i]["Tender_Id"].ToString().Trim(), currConn, transaction);
                    #endregion FindTenderId

                    #region Checking Date is null or different formate
                    bool IsInvoiceDate;
                    IsInvoiceDate = cImport.CheckDate(dtSaleM.Rows[i]["Invoice_Date_Time"].ToString().Trim());
                    if (IsInvoiceDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Invoice_Date_Time field.");
                    }
                    bool IsDeliveryDate;
                    IsDeliveryDate = cImport.CheckDate(dtSaleM.Rows[i]["Delivery_Date_Time"].ToString().Trim());
                    if (IsDeliveryDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Delivery_Date_Time field.");
                    }
                    #endregion Checking Date is null or different formate

                    #region Checking Y/N value
                    bool post;
                    bool isPrint;
                    post = cImport.CheckYN(dtSaleM.Rows[i]["Post"].ToString().Trim());
                    if (post != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Post field.");
                    }
                    isPrint = cImport.CheckYN(dtSaleM.Rows[i]["Is_Print"].ToString().Trim());
                    if (isPrint != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Is_Print field.");
                    }
                    #endregion Checking Y/N value

                    #region Check previous invoice id
                    //string PreInvoiceId = string.Empty;
                    //string TenderId = string.Empty;

                    //PreInvoiceId = cImport.CheckPreInvoiceID(dtSaleM.Rows[i]["Previous_Invoice_No"].ToString().Trim(), currConn, transaction);
                    //TenderId = cImport.CheckTenderID(dtSaleM.Rows[i]["Tender_Id"].ToString().Trim(), currConn, transaction);
                    #endregion Check previous invoice id

                    #region Check LC Number
                    if (dtSaleM.Rows[i]["Transection_Type"].ToString().Trim() == "Export")
                    {

                        string LCNumber = string.Empty;
                        DataRow[] ExportRaws = dtSaleE.Select("ID='" + dtSaleM.Rows[i]["ID"].ToString().Trim() + "'");
                        if (ExportRaws.Length > 0)
                        {
                            LCNumber = dtSaleM.Rows[i]["LC_Number"].ToString().Trim();
                            if (string.IsNullOrEmpty(LCNumber) || LCNumber == "0")
                            {
                                throw new ArgumentNullException("Please insert value in LC_Number field.");
                            }
                        }
                    }
                    #endregion  Check LC Number

                    #endregion Master

                }
                #endregion Master

                #region Details

                #region Row count for details table
                int DRowCount = 0;
                for (int i = 0; i < dtSaleD.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtSaleD.Rows[i]["ID"].ToString()))
                    {
                        DRowCount++;
                    }

                }
                #endregion Row count for details table

                for (int i = 0; i < DRowCount; i++)
                {
                    VitemCode = string.Empty;
                    string UOMn = string.Empty;
                    string UOMc = string.Empty;
                    bool IsQuantity, IsNbrPrice, IsTrading, IsSDRate, IsVatRate, IsDiscount, IsPromoQuantity;


                    #region FindItemId
                    if (string.IsNullOrEmpty(dtSaleD.Rows[i]["Item_Code"].ToString().Trim()))
                    {
                        throw new ArgumentNullException("Please insert value in 'Item_Code' field.");
                    }
                    VitemCode = cImport.FindItemId(dtSaleD.Rows[i]["Item_Name"].ToString().Trim()
                                 , dtSaleD.Rows[i]["Item_Code"].ToString().Trim(), currConn, transaction);

                    #endregion FindItemId

                    #region FindUOMn
                    UOMn = cImport.FindUOMn(VitemCode, currConn, transaction);
                    #endregion FindUOMn
                    #region FindUOMc
                    if (DatabaseInfoVM.DatabaseName.ToString() != "Sanofi_DB")
                    {
                        UOMc = cImport.FindUOMc(UOMn, dtSaleD.Rows[i]["UOM"].ToString().Trim(), currConn, transaction);
                    }
                    #endregion FindUOMc

                    #region FindLastNBRPrice

                    DataRow[] vmaster; //= new DataRow[];//

                    string nbrPrice = string.Empty;
                    var transactionDate = "";
                    vmaster = dtSaleM.Select("ID='" + dtSaleD.Rows[i]["ID"].ToString().Trim() + "'");
                    foreach (DataRow row in vmaster)
                    {
                        var tt = Convert.ToDateTime(row["Invoice_Date_Time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss").Trim();
                        transactionDate = tt;
                    }
                    if (IsPriceDeclaration == true)
                    {
                        nbrPrice = cImport.FindLastNBRPrice(VitemCode, dtSaleD.Rows[i]["VAT_Name"].ToString().Trim(),
                            transactionDate, null, null);
                        if (Convert.ToDecimal(nbrPrice) == 0)
                        {

                            if (vmaster[0]["Transection_Type"].ToString() != "ExportService"
                                && vmaster[0]["Transection_Type"].ToString() != "ExportServiceNS"
                                && vmaster[0]["Transection_Type"].ToString() != "Service"
                                && vmaster[0]["Transection_Type"].ToString() != "ServiceNS"
                               )
                            {
                                throw new ArgumentNullException("Price declaration of item('" +
                                                dtSaleD.Rows[i]["Item_Name"].ToString().Trim() + "') not find in database");
                            }
                        }
                    }

                    #endregion FindLastNBRPrice
                    #region VATName
                    cImport.FindVatName(dtSaleD.Rows[i]["VAT_Name"].ToString().Trim());
                    #endregion VATName
                    #region Numeric value check

                    if (!string.IsNullOrEmpty(dtSaleD.Rows[i]["Quantity"].ToString().Trim())
                        && !string.IsNullOrEmpty(dtSaleD.Rows[i]["Promotional_Quantity"].ToString().Trim()))
                    {
                        if (Convert.ToDecimal(dtSaleD.Rows[i]["Quantity"].ToString().Trim()) == 0
                            && Convert.ToDecimal(dtSaleD.Rows[i]["Promotional_Quantity"].ToString().Trim()) == 0)
                        {
                            throw new ArgumentNullException("Please insert quantity value in ID: " + dtSaleD.Rows[i]["ID"].ToString().Trim() + ", '" +
                                      dtSaleD.Rows[i]["Item_Name"].ToString().Trim() + "'('" + dtSaleD.Rows[i]["Item_Code"].ToString().Trim() + "').");
                        }
                    }


                    IsQuantity = cImport.CheckNumericBool(dtSaleD.Rows[i]["Quantity"].ToString().Trim());
                    if (IsQuantity != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Quantity field.");
                    }
                    if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                    {
                        IsNbrPrice = cImport.CheckNumericBool(dtSaleD.Rows[i]["Total_Price"].ToString().Trim());
                    }
                    else
                    {
                        IsNbrPrice = cImport.CheckNumericBool(dtSaleD.Rows[i]["NBR_Price"].ToString().Trim());
                    }
                    if (IsNbrPrice != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in NBR_Price field.");
                    }
                    if (!string.IsNullOrEmpty(dtSaleD.Rows[i]["VAT_Rate"].ToString().Trim()))
                    {
                        IsVatRate = cImport.CheckNumericBool(dtSaleD.Rows[i]["VAT_Rate"].ToString().Trim());
                        if (IsVatRate != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in VAT_Rate field.");
                        }
                    }

                    IsSDRate = cImport.CheckNumericBool(dtSaleD.Rows[i]["SD_Rate"].ToString().Trim());
                    if (IsSDRate != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in SD_Rate field.");
                    }
                    IsTrading = cImport.CheckNumericBool(dtSaleD.Rows[i]["Trading_MarkUp"].ToString().Trim());
                    if (IsTrading != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Trading_MarkUp field.");
                    }
                    IsDiscount = cImport.CheckNumericBool(dtSaleD.Rows[i]["Discount_Amount"].ToString().Trim());
                    if (IsDiscount != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Discount_Amount field.");
                    }
                    IsPromoQuantity = cImport.CheckNumericBool(dtSaleD.Rows[i]["Promotional_Quantity"].ToString().Trim());
                    if (IsPromoQuantity != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Promotional_Quantity field.");
                    }
                    #endregion Numeric value check

                    #region Checking Y/N value
                    bool NonStock;
                    NonStock = cImport.CheckYN(dtSaleD.Rows[i]["Non_Stock"].ToString().Trim());
                    if (NonStock != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Non_Stock field.");
                    }
                    #endregion Checking Y/N value


                    #region Check Stock

                    string quantityInHand = productDal.AvgPriceNew(VitemCode, transactionDate, null, null, false).Rows[0]["Quantity"].ToString();

                    string tenderStock = "0";//"0,0.0000");

                    decimal minValue = 0;
                    if (Convert.ToDecimal(quantityInHand) < Convert.ToDecimal(tenderStock))
                    {
                        minValue = Convert.ToDecimal(quantityInHand);
                    }
                    else
                    {
                        minValue = Convert.ToDecimal(tenderStock);

                    }
                    if (vmaster[0]["Transection_Type"].ToString() == "Tender" || vmaster[0]["Transection_Type"].ToString() == "TradingTender")
                    {
                        if (Convert.ToDecimal(quantityInHand) * Convert.ToDecimal(UOMc) > Convert.ToDecimal(minValue))
                        {
                            throw new ArgumentNullException("Stock Not available for " + VitemCode);
                        }
                    }
                    else if (vmaster[0]["Transection_Type"].ToString() != "Credit"
                                && vmaster[0]["Transection_Type"].ToString() != "VAT11GaGa"
                                && vmaster[0]["Transection_Type"].ToString() != "ServiceNS"
                                && vmaster[0]["Transection_Type"].ToString() != "ExportServiceNS")
                    {
                        if (isNegStockAllow == false)
                        {
                            if (Convert.ToDecimal(dtSaleD.Rows[i]["Quantity"].ToString().Trim()) * Convert.ToDecimal(UOMc) > Convert.ToDecimal(quantityInHand))
                            {
                                throw new ArgumentNullException("Stock Not available for " + VitemCode);
                            }
                        }
                    }


                    #endregion Check Stock


                }


                #endregion Details
                #region Export
                if (dtSaleM.Rows.Count > 0 && (dtSaleM.Rows[0]["Transection_Type"].ToString() == "Export"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportTender"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportTrading"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportServiceNS"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportService"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportTradingTender"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportPackage"))
                {

                    #region Row count for export details table
                    int ERowCount = 0;
                    for (int i = 0; i < dtSaleE.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(dtSaleE.Rows[i]["ID"].ToString()))
                        {
                            ERowCount++;
                        }

                    }
                    #endregion Row count for export details table

                    for (int e = 0; e < ERowCount; e++)
                    {
                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["Description"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in Description field.");
                        }

                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["Quantity"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in Quantity field.");
                        }

                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["GrossWeight"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in GrossWeight field.");
                        }

                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["NetWeight"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in NetWeight field.");
                        }

                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["NumberFrom"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in NumberFrom field.");
                        }
                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["NumberTo"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in NumberTo field.");
                        }

                    }
                }

                #endregion Export

                #endregion checking from database is exist the information(NULL Check)


                if (currConn.State == ConnectionState.Open)
                {
                    transaction.Commit();
                    currConn.Close();
                    currConn.Open();
                    transaction = currConn.BeginTransaction("Import Data");
                }


                for (int i = 0; i < MRowCount; i++)
                {
                    #region Process model
                    #region Master Sale

                    VinvoiceNo = dtSaleM.Rows[i]["ID"].ToString().Trim();
                    string customerName = dtSaleM.Rows[i]["Customer_Name"].ToString().Trim();
                    string customerCode = dtSaleM.Rows[i]["Customer_Code"].ToString().Trim();

                    #region FindCustomerId

                    string customerId = cImport.FindCustomerId(customerName, customerCode, currConn, transaction);

                    #endregion FindCustomerId

                    string deliveryAddress = dtSaleM.Rows[i]["Delivery_Address"].ToString().Trim();
                    string vehicleNo = dtSaleM.Rows[i]["Vehicle_No"].ToString().Trim();
                    var invoiceDateTime = Convert.ToDateTime(dtSaleM.Rows[i]["Invoice_Date_Time"].ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss");
                    var deliveryDateTime =
                        Convert.ToDateTime(dtSaleM.Rows[i]["Delivery_Date_Time"].ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss");
                    #region CheckNull
                    string referenceNo = cImport.ChecKNullValue(dtSaleM.Rows[i]["Reference_No"].ToString().Trim());
                    string comments = cImport.ChecKNullValue(dtSaleM.Rows[i]["Comments"].ToString().Trim());
                    #endregion CheckNull
                    string saleType = dtSaleM.Rows[i]["Sale_Type"].ToString().Trim();
                    #region Check previous invoice no.
                    string previousInvoiceNo = cImport.CheckPrePurchaseNo(dtSaleM.Rows[i]["Previous_Invoice_No"].ToString().Trim(), currConn, transaction);
                    #endregion Check previous invoice no.
                    string isPrint = dtSaleM.Rows[i]["Is_Print"].ToString().Trim();
                    #region Check Tender id
                    string tenderId = cImport.CheckPrePurchaseNo(dtSaleM.Rows[i]["Tender_Id"].ToString().Trim(), currConn, transaction);
                    #endregion Check Tender id
                    string post = dtSaleM.Rows[i]["Post"].ToString().Trim();

                    #region CheckNull
                    string lCNumber = cImport.ChecKNullValue(dtSaleM.Rows[i]["LC_Number"].ToString().Trim());
                    #endregion CheckNull
                    string currencyCode = dtSaleM.Rows[i]["Currency_Code"].ToString().Trim();
                    string createdBy = dtSaleM.Rows[i]["Created_By"].ToString().Trim();
                    string lastModifiedBy = dtSaleM.Rows[i]["LastModified_By"].ToString().Trim();
                    string transactionType = dtSaleM.Rows[i]["Transection_Type"].ToString().Trim();
                    string compInvoiceNo = null;
                    if (IsCompInvoiceNo == true)
                    {
                        compInvoiceNo = dtSaleM.Rows[i]["Comp_Invoice_No"].ToString().Trim();
                    }



                    #region Master

                    saleMaster = new SaleMasterVM();
                    saleMaster.CustomerID = customerId;
                    saleMaster.DeliveryAddress1 = deliveryAddress;
                    saleMaster.VehicleNo = vehicleNo;
                    #region FindVehicleId

                    string vehicleId = cImport.FindVehicleId(vehicleNo, currConn, transaction);

                    #endregion FindCustomerId
                    saleMaster.VehicleID = vehicleId;
                    saleMaster.InvoiceDateTime =
                       Convert.ToDateTime(invoiceDateTime).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    saleMaster.SerialNo = referenceNo;
                    saleMaster.Comments = comments;
                    saleMaster.CreatedBy = createdBy;
                    saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    saleMaster.LastModifiedBy = lastModifiedBy;
                    saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    saleMaster.SaleType = saleType;
                    saleMaster.PreviousSalesInvoiceNo = previousInvoiceNo;
                    saleMaster.Trading = "N";
                    saleMaster.IsPrint = isPrint;
                    saleMaster.TenderId = tenderId;
                    saleMaster.TransactionType = transactionType;
                    saleMaster.DeliveryDate =
                       Convert.ToDateTime(deliveryDateTime).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    saleMaster.Post = post; //Post
                    var currencyid = cImport.FindCurrencyId(currencyCode, currConn, transaction);
                    saleMaster.CurrencyID = currencyid; //Post
                    saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(cImport.FindCurrencyRateFromBDT(currencyid, currConn, transaction));
                    saleMaster.ReturnId = cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction), invoiceDateTime, currConn, transaction);
                    // return ID is used for doller rate
                    saleMaster.LCNumber = lCNumber;
                    saleMaster.ImportID = VinvoiceNo;

                    saleMaster.TotalAmount = Convert.ToDecimal("0.00");
                    saleMaster.TotalVATAmount = Convert.ToDecimal("0.00");
                    saleMaster.CompInvoiceNo = compInvoiceNo;

                    #endregion Master


                    #endregion Master Sale

                    #region Match

                    DataRow[] DetailsRaws; //= new DataRow[];//
                    if (!string.IsNullOrEmpty(VinvoiceNo))
                    {
                        DetailsRaws = dtSaleD.Select("ID='" + VinvoiceNo + "'");
                    }
                    else
                    {
                        DetailsRaws = null;
                    }



                    #endregion Match

                    #region Details Sale

                    //int totalCounter = 1;
                    int lineCounter = 1;
                    decimal totalAmount = 0;
                    decimal totalVatAmount = 0;


                    saleDetails = new List<SaleDetailVM>();
                    #region Juwel 15/10/2015

                    //DataTable dtDistinctItem = DetailsRaws.CopyToDataTable().DefaultView.ToTable(true, "Item_Code", "Item_Name", "Non_Stock", "Type", "VAT_Name");
                    DataTable dtDistinctItem = DetailsRaws.CopyToDataTable().DefaultView.ToTable(true, "Item_Code", "Non_Stock", "Type", "VAT_Name");
                    DataTable dtSalesDetail = DetailsRaws.CopyToDataTable();



                    string nBRPrice = "", uOM = "", uOMn = "", uOMc = "";

                    foreach (DataRow item in dtDistinctItem.Rows)
                    {
                        decimal Total_Price = 0;
                        decimal NBR_Price = 0, totalQuantity = 0, totalPrice = 0, vatRate = 0, sdRate = 0, tradingMarkup = 0, discountAmount = 0, promotionalQuantity = 0;



                        decimal LastNBRPrice = 0;
                        string saleDID = VinvoiceNo;//row["ID"].ToString().Trim();
                        VitemCode = item["Item_Code"].ToString().Trim();
                        //string itemName = item["Item_Name"].ToString().Trim();
                        string itemNo = cImport.FindItemId("", VitemCode, currConn, transaction);
                        string vATName = item["VAT_Name"].ToString().Trim();
                        string nonStock = item["Non_Stock"].ToString().Trim();
                        string type = item["Type"].ToString().Trim();
                        string weight = "";

                        DataTable dtRepeatedItems = dtSalesDetail.Select("[Item_Code] ='" + item["Item_Code"].ToString() + "'").CopyToDataTable();

                        foreach (DataRow row in dtRepeatedItems.Rows)
                        {
                            Total_Price = Convert.ToDecimal(row["Total_Price"].ToString().Trim());
                            totalQuantity = totalQuantity + Convert.ToDecimal(row["Quantity"].ToString().Trim());
                            //totalPrice = totalPrice + Convert.ToDecimal(row["Total_Price"].ToString().Trim());/////Juwel
                            if (type.ToLower() != "non-vat system")
                            {
                                vatRate = vatRate + Convert.ToDecimal(row["VAT_Rate"].ToString().Trim());
                                sdRate = sdRate + Convert.ToDecimal(row["SD_Rate"].ToString().Trim());
                                tradingMarkup = tradingMarkup + Convert.ToDecimal(row["Trading_MarkUp"].ToString().Trim());
                            }
                            //else
                            //{
                            //    vatRate =vatRate+ 0;
                            //    sdRate =sdRate+ 0;
                            //    tradingMarkup = tradingMarkup + 0;
                            //}

                            discountAmount = discountAmount + Convert.ToDecimal(row["Discount_Amount"].ToString().Trim());
                            promotionalQuantity = promotionalQuantity + Convert.ToDecimal(row["Promotional_Quantity"].ToString().Trim());
                            if (IsColWeight == true)
                            {
                                weight = row["Weight"].ToString().Trim();
                            }

                            if (DatabaseInfoVM.DatabaseName.ToString() != "Sanofi_DB")
                            {
                                NBR_Price = NBR_Price + Convert.ToDecimal(row["NBR_Price"].ToString().Trim());
                                uOM = row["UOM"].ToString().Trim();
                                uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                                uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction);
                            }
                            else
                            {
                                totalPrice = totalPrice + Convert.ToDecimal(row["Total_Price"].ToString().Trim());
                            }

                        }

                        #region For Sanofi
                        if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                        {
                            // string totalPrice = row["Total_Price"].ToString().Trim();
                            nBRPrice = (Convert.ToDecimal(totalPrice) / Convert.ToDecimal(totalQuantity)).ToString();
                            LastNBRPrice = Convert.ToDecimal(nBRPrice);


                            uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                            uOM = uOMn;
                            uOMc = "1";
                        }
                        else
                        {
                            nBRPrice = NBR_Price.ToString();// row["NBR_Price"].ToString().Trim();

                            if (transactionType == "ExportService"
                                 && transactionType == "ExportServiceNS"
                               && transactionType == "Service"
                               && transactionType == "ServiceNS")
                            {
                                if (nBRPrice == "0")
                                {
                                    LastNBRPrice = cImport.FindLastNBRPriceFromBOM(itemNo, vATName,
                                                                            invoiceDateTime, currConn, transaction);
                                }
                                else
                                {
                                    LastNBRPrice = Convert.ToDecimal(nBRPrice);
                                }
                            }
                            else
                            {
                                LastNBRPrice = cImport.FindLastNBRPriceFromBOM(itemNo, vATName,
                                                                            invoiceDateTime, currConn, transaction);
                                if (IsPriceDeclaration == false)
                                {
                                    LastNBRPrice = Convert.ToDecimal(nBRPrice);
                                }
                            }
                        }
                        #endregion

                        string vATRate = Convert.ToString(vatRate / dtRepeatedItems.Rows.Count);
                        if (string.IsNullOrEmpty(vATRate))
                        {
                            vATRate = "0";
                        }
                        string sDRate = Convert.ToString(sdRate / dtRepeatedItems.Rows.Count);
                        string tradingMarkUp = tradingMarkup.ToString();
                        if (type.ToLower() == "vat" || type.ToLower() == "vat system")
                        {
                            type = "VAT";

                        }
                        else
                        {
                            type = "Non VAT";
                            vATName = " ";
                        }

                        SaleDetailVM detail = new SaleDetailVM();
                        detail.InvoiceLineNo = lineCounter.ToString();
                        detail.ItemNo = itemNo;
                        decimal vQuantity = Convert.ToDecimal(Convert.ToDecimal(totalQuantity) + Convert.ToDecimal(promotionalQuantity));
                        detail.Quantity = vQuantity;
                        detail.PromotionalQuantity = Convert.ToDecimal(promotionalQuantity);
                        detail.VATRate = Convert.ToDecimal(vATRate);
                        detail.SD = Convert.ToDecimal(sDRate);
                        detail.CommentsD = "NA";
                        detail.SaleTypeD = saleType;
                        detail.PreviousSalesInvoiceNoD = previousInvoiceNo;
                        detail.TradingD = "N";
                        detail.NonStockD = nonStock;
                        detail.Type = type;
                        detail.CConversionDate = invoiceDateTime;
                        detail.VatName = vATName;
                        detail.Weight = weight;
                        detail.TradingMarkUp = Convert.ToDecimal(tradingMarkUp);
                        if (vTotalPrice.ToLower() == "y")
                        {
                            detail.SubTotal = Total_Price;

                            detail.DiscountAmount = Convert.ToDecimal(discountAmount);
                            LastNBRPrice = Total_Price / vQuantity;
                            decimal discountedNBRPrice = Convert.ToDecimal(Convert.ToDecimal(LastNBRPrice)
                                                                           * Convert.ToDecimal(uOMc))
                                                         - Convert.ToDecimal(discountAmount);

                            detail.DiscountedNBRPrice = Convert.ToDecimal(discountedNBRPrice);
                            decimal nbrPrice = Convert.ToDecimal(discountedNBRPrice) - Convert.ToDecimal(discountAmount);
                            detail.UOMn = uOMn;

                            detail.SalesPrice = nbrPrice;
                            detail.NBRPrice = nbrPrice;
                            decimal subTotal = Total_Price;// vQuantity* nbrPrice;
                            detail.SubTotal = Convert.ToDecimal(subTotal);
                            decimal vATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                            detail.VATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                            detail.SDAmount = subTotal * Convert.ToDecimal(sDRate) / 100;
                            detail.UOMQty = Convert.ToDecimal(vQuantity) * Convert.ToDecimal(uOMc);
                            detail.UOMc = Convert.ToDecimal(uOMc);
                            detail.UOMPrice = Convert.ToDecimal(LastNBRPrice);
                            detail.DollerValue = Convert.ToDecimal(subTotal) /
                                                 Convert.ToDecimal(
                                                     cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction), invoiceDateTime, currConn, transaction));
                            detail.CurrencyValue = Convert.ToDecimal(subTotal);


                            #region Total for Master
                            totalAmount = totalAmount + subTotal + vATAmount;
                            totalVatAmount = totalVatAmount + vATAmount;

                            saleMaster.TotalAmount = totalAmount;
                            saleMaster.TotalVATAmount = totalVatAmount;
                            #endregion Total for Master
                        }
                        else
                        {
                            detail.DiscountAmount = Convert.ToDecimal(discountAmount);
                            decimal discountedNBRPrice = Convert.ToDecimal(Convert.ToDecimal(LastNBRPrice)
                                                                           * Convert.ToDecimal(uOMc))
                                                         - Convert.ToDecimal(discountAmount);
                            detail.DiscountedNBRPrice = Convert.ToDecimal(discountedNBRPrice);
                            decimal nbrPrice = Convert.ToDecimal(discountedNBRPrice) - Convert.ToDecimal(discountAmount);
                            detail.UOMn = uOMn;

                            detail.SalesPrice = nbrPrice;
                            detail.NBRPrice = nbrPrice;
                            decimal subTotal = vQuantity * nbrPrice;
                            detail.SubTotal = Convert.ToDecimal(subTotal);
                            decimal vATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                            detail.VATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                            detail.SDAmount = subTotal * Convert.ToDecimal(sDRate) / 100;
                            detail.UOMQty = Convert.ToDecimal(vQuantity) * Convert.ToDecimal(uOMc);
                            detail.UOMc = Convert.ToDecimal(uOMc);
                            detail.UOMPrice = Convert.ToDecimal(LastNBRPrice);
                            detail.DollerValue = Convert.ToDecimal(subTotal) /
                                                 Convert.ToDecimal(
                                                     cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction), invoiceDateTime, currConn, transaction));
                            detail.CurrencyValue = Convert.ToDecimal(subTotal);
                            #region Total for Master
                            totalAmount = totalAmount + subTotal + vATAmount;
                            totalVatAmount = totalVatAmount + vATAmount;

                            saleMaster.TotalAmount = totalAmount;
                            saleMaster.TotalVATAmount = totalVatAmount;
                            #endregion Total for Master
                        }


                        saleDetails.Add(detail);


                        lineCounter++;
                    }
                    #endregion
                    #region previous code on 15/10/2015
                    //foreach (DataRow row in DetailsRaws)
                    //{
                    //    #region number of items setting for a challan
                    //    //if (lineCounter == 1)
                    //    //{
                    //    //    #region Master

                    //    //    saleMaster = new SaleMasterVM();
                    //    //    saleMaster.CustomerID = customerId;
                    //    //    saleMaster.DeliveryAddress1 = deliveryAddress;
                    //    //    saleMaster.VehicleNo = vehicleNo;
                    //    //    saleMaster.InvoiceDateTime =
                    //    //       Convert.ToDateTime(invoiceDateTime).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    //    //    saleMaster.SerialNo = referenceNo;
                    //    //    saleMaster.Comments = comments;
                    //    //    saleMaster.CreatedBy = createdBy;
                    //    //    saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //    //    saleMaster.LastModifiedBy = lastModifiedBy;
                    //    //    saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //    //    saleMaster.SaleType = saleType;
                    //    //    saleMaster.PreviousSalesInvoiceNo = previousInvoiceNo;
                    //    //    saleMaster.Trading = "N";
                    //    //    saleMaster.IsPrint = isPrint;
                    //    //    saleMaster.TenderId = tenderId;
                    //    //    saleMaster.TransactionType = transactionType;
                    //    //    saleMaster.DeliveryDate =
                    //    //       Convert.ToDateTime(deliveryDateTime).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    //    //    saleMaster.Post = post; //Post
                    //    //    var currencyid = cImport.FindCurrencyId(currencyCode, currConn, transaction);
                    //    //    saleMaster.CurrencyID = currencyid; //Post
                    //    //    saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(cImport.FindCurrencyRateFromBDT(currencyid, currConn, transaction));
                    //    //    saleMaster.ReturnId = cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction), currConn, transaction);
                    //    //    // return ID is used for doller rate
                    //    //    saleMaster.LCNumber = lCNumber;
                    //    //    saleMaster.ImportID = importId;

                    //    //    saleMaster.TotalAmount = Convert.ToDecimal("0.00");
                    //    //    saleMaster.TotalVATAmount = Convert.ToDecimal("0.00");

                    //    //    #endregion Master
                    //    //    saleDetails = new List<SaleDetailVM>();

                    //    //}
                    //    #endregion number of items setting for a challan
                    //    decimal LastNBRPrice = 0;
                    //    string saleDID = row["ID"].ToString().Trim();
                    //    string itemCode = row["Item_Code"].ToString().Trim();
                    //    string itemName = row["Item_Name"].ToString().Trim();
                    //    string itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);
                    //    string quantity = row["Quantity"].ToString().Trim();

                    //    string vATName = row["VAT_Name"].ToString().Trim();

                    //    string nBRPrice = "";

                    //    string uOM = "";
                    //    string uOMn = "";
                    //    string uOMc = "";

                    //    #region For Sanofi
                    //    if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                    //    {
                    //        string totalPrice = row["Total_Price"].ToString().Trim();
                    //        nBRPrice =( Convert.ToDecimal(totalPrice) / Convert.ToDecimal(quantity)).ToString();
                    //        LastNBRPrice = Convert.ToDecimal(nBRPrice);


                    //        uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                    //        uOM = uOMn;
                    //        uOMc = "1";
                    //    }
                    //    else
                    //    {
                    //        nBRPrice = row["NBR_Price"].ToString().Trim();

                    //        if (transactionType == "ExportService"
                    //             && transactionType == "ExportServiceNS"
                    //           && transactionType == "Service"
                    //           && transactionType == "ServiceNS")
                    //        {
                    //            if (nBRPrice == "0")
                    //            {
                    //                LastNBRPrice = cImport.FindLastNBRPriceFromBOM(itemNo, vATName,
                    //                                                        invoiceDateTime, currConn, transaction);
                    //            }
                    //            else
                    //            {
                    //                LastNBRPrice = Convert.ToDecimal(nBRPrice);
                    //            }
                    //        }
                    //        else
                    //        {
                    //            LastNBRPrice = cImport.FindLastNBRPriceFromBOM(itemNo, vATName,
                    //                                                        invoiceDateTime, currConn, transaction);
                    //            if (IsPriceDeclaration == false)
                    //            {
                    //                LastNBRPrice = Convert.ToDecimal(nBRPrice);
                    //            }
                    //        }
                    //        uOM = row["UOM"].ToString().Trim();
                    //        uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                    //        uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction);
                    //    }
                    //    #endregion



                    //    string vATRate = row["VAT_Rate"].ToString().Trim();
                    //    if (string.IsNullOrEmpty(vATRate))
                    //    {
                    //        vATRate = "0";
                    //    }
                    //    string sDRate = row["SD_Rate"].ToString().Trim();
                    //    string nonStock = row["Non_Stock"].ToString().Trim();
                    //    string tradingMarkUp = row["Trading_MarkUp"].ToString().Trim();
                    //    string type = row["Type"].ToString().Trim();
                    //    if (type.ToLower() =="vat" || type.ToLower()=="vat system")
                    //    {
                    //        type = "VAT";

                    //    }
                    //    else
                    //    {
                    //        type = "Non VAT";
                    //        vATName = " ";
                    //    }
                    //    string discountAmount = row["Discount_Amount"].ToString().Trim();
                    //    string promotionalQuantity = row["Promotional_Quantity"].ToString().Trim();

                    //    SaleDetailVM detail = new SaleDetailVM();

                    //    detail.InvoiceLineNo = lineCounter.ToString();
                    //    detail.ItemNo = itemNo;
                    //    decimal vQuantity =
                    //        Convert.ToDecimal(Convert.ToDecimal(quantity) + Convert.ToDecimal(promotionalQuantity));
                    //    detail.Quantity = vQuantity;
                    //    detail.PromotionalQuantity = Convert.ToDecimal(promotionalQuantity);
                    //    detail.UOM = uOM;
                    //    detail.VATRate = Convert.ToDecimal(vATRate);
                    //    detail.SD = Convert.ToDecimal(sDRate);
                    //    detail.CommentsD = "NA";
                    //    detail.SaleTypeD = saleType;
                    //    detail.PreviousSalesInvoiceNoD = previousInvoiceNo;
                    //    detail.TradingD = "N";
                    //    detail.NonStockD = nonStock;
                    //    detail.Type = type;
                    //    detail.TradingMarkUp = Convert.ToDecimal(tradingMarkUp);
                    //    detail.DiscountAmount = Convert.ToDecimal(discountAmount);

                    //    decimal discountedNBRPrice = Convert.ToDecimal(Convert.ToDecimal(LastNBRPrice)
                    //                                                   * Convert.ToDecimal(uOMc))
                    //                                 - Convert.ToDecimal(discountAmount);

                    //    detail.DiscountedNBRPrice = Convert.ToDecimal(discountedNBRPrice);
                    //    decimal nbrPrice = Convert.ToDecimal(discountedNBRPrice) - Convert.ToDecimal(discountAmount);

                    //    detail.SalesPrice = nbrPrice;
                    //    detail.NBRPrice = nbrPrice;
                    //    decimal subTotal = vQuantity * nbrPrice;
                    //    detail.SubTotal = Convert.ToDecimal(subTotal);
                    //    decimal vATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                    //    detail.VATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                    //    detail.SDAmount = subTotal * Convert.ToDecimal(sDRate) / 100;
                    //    detail.UOMQty = Convert.ToDecimal(vQuantity) * Convert.ToDecimal(uOMc);
                    //    detail.UOMn = uOMn;
                    //    detail.UOMc = Convert.ToDecimal(uOMc);
                    //    detail.UOMPrice = Convert.ToDecimal(LastNBRPrice);

                    //    ////detail.DollerValue = Convert.ToDecimal(nbrPrice) *
                    //    ////                     Convert.ToDecimal(
                    //    ////                         cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction), invoiceDateTime, currConn, transaction));
                    //    ////detail.CurrencyValue = Convert.ToDecimal(nbrPrice);

                    //    //// subtoal value is BDT value
                    //    detail.DollerValue = Convert.ToDecimal(subTotal)/
                    //                         Convert.ToDecimal(
                    //                             cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction), invoiceDateTime, currConn, transaction));
                    //    detail.CurrencyValue = Convert.ToDecimal(subTotal);
                    //    detail.CConversionDate = invoiceDateTime;
                    //    detail.VatName = vATName;
                    //    saleDetails.Add(detail);

                    //    #region Total for Master
                    //    totalAmount = totalAmount + subTotal + vATAmount;
                    //    totalVatAmount = totalVatAmount + vATAmount;

                    //    saleMaster.TotalAmount = totalAmount;
                    //    saleMaster.TotalVATAmount = totalVatAmount;
                    //    #endregion Total for Master

                    //    #region Generate items per challan as settings


                    //    //if (lineCounter == Convert.ToInt32(15) || totalCounter == DetailsRaws.Length)
                    //    //{
                    //    //    saleMaster.TotalAmount = totalAmount;
                    //    //    saleMaster.TotalVATAmount = totalVatAmount;

                    //    //    //// Insert into database
                    //    //    var sqlResults = SalesInsert(saleMaster, saleDetails,saleExport, transaction, currConn);
                    //    //    retResults[0] = sqlResults[0];

                    //    //    lineCounter = 0;
                    //    //    totalAmount = 0;
                    //    //    totalVatAmount = 0;

                    //    //}

                    //    //lineCounter++;
                    //    //totalCounter++;

                    //    #endregion Generate items per challan as settings


                    //    lineCounter++;

                    //} // details
                    #endregion
                    #endregion Details Sale

                    #region Details Export

                    int eCounter = 1;

                    if (transactionType == "Export"
                    || transactionType == "ExportTender"
                    || transactionType == "ExportTrading"
                    || transactionType == "ExportServiceNS"
                    || transactionType == "ExportService"
                    || transactionType == "ExportTradingTender"
                    || transactionType == "ExportPackage")
                    {
                        DataRow[] ExportRaws; //= new DataRow[];//
                        if (!string.IsNullOrEmpty(VinvoiceNo))
                        {
                            ExportRaws = dtSaleE.Select("ID='" + VinvoiceNo + "'");
                        }
                        else
                        {

                            ExportRaws = null;
                            if (ExportRaws == null)
                            {
                                throw new ArgumentNullException("For Export sale must filup the SaleE file");
                            }

                        }

                        saleExport = new List<SaleExportVM>();
                        foreach (DataRow row in ExportRaws)
                        {
                            string saleEID = row["ID"].ToString().Trim();
                            string description = row["Description"].ToString().Trim();
                            string quantityE = row["Quantity"].ToString().Trim();
                            string grossWeight = row["GrossWeight"].ToString().Trim();

                            string netWeight = row["NetWeight"].ToString().Trim();
                            string numberFrom = row["NumberFrom"].ToString().Trim();
                            string numberTo = row["NumberTo"].ToString().Trim();
                            //string portFrom = row["PortFrom"].ToString().Trim();

                            //string portTo = row["PortTo"].ToString().Trim();


                            SaleExportVM expDetail = new SaleExportVM();
                            expDetail.SaleLineNo = eCounter.ToString();
                            expDetail.Description = description.ToString();
                            expDetail.QuantityE = quantityE.ToString();
                            expDetail.GrossWeight = grossWeight.ToString();
                            expDetail.NetWeight = netWeight.ToString();
                            expDetail.NumberFrom = numberFrom.ToString();
                            expDetail.NumberTo = numberTo.ToString();
                            expDetail.CommentsE = "NA";
                            expDetail.RefNo = "NA";

                            saleExport.Add(expDetail);



                            eCounter++;

                        } // details


                    }
                    #endregion Details Export

                    #endregion Process model

                    var sqlResults = SalesInsert(saleMaster, saleDetails, saleExport, null, transaction, currConn);
                    retResults[0] = sqlResults[0];
                }// master
                if (retResults[0] == "Success")
                {
                    transaction.Commit();
                    #region SuccessResult

                    retResults[0] = "Success";
                    retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                    retResults[2] = "" + "1";
                    retResults[3] = "" + "N";
                    #endregion SuccessResult
                }
                //SAVE_DOWORK_SUCCESS = true;
            }
            #endregion try
            #region catch & final
            //catch (SqlException sqlex)
            //{
            //    if (transaction != null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            //catch (ArgumentNullException aeg)
            //{
            //    if (transaction != null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + aeg.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            //    //throw ex;
            //}
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString() + FieldDelimeter + "Invoice:" + VinvoiceNo + FieldDelimeter + "Item:" + VitemCode);//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
                sqlText += "where ItemNo = @itemNo and SaleReturnId =@saleReturnId ";
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
                sqlText += " and s.SalesInvoiceNo=@salesInvoiceNo ";
                sqlText += " group by c.CurrencyMajor,c.CurrencyMinor,c.CurrencySymbol ";

                DataTable dt = new DataTable("CurrencyData");
                SqlCommand cmdCurrency = new SqlCommand(sqlText, currConn);
                cmdCurrency.Transaction = transaction;

                cmdCurrency.Parameters.AddWithValue("@salesInvoiceNo", salesInvoiceNo);

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

        public string GetCategoryName(string itemNo)
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
                sqlText += " where ItemNo = @itemNo";

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


        public void SetDeliveryChallanNo(string saleInvoiceNo, string challanDate)
        {
            #region Initializ

            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
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
                    transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameInsert);
                }

                #endregion open connection and transaction

                #region SetDCNo
                CommonDAL commonDal = new CommonDAL();
                string newID = commonDal.TransactionCode("Sale", "Delivery", "SalesInvoiceHeaders", "DeliveryChallanNo",
                                                    "DeliveryDate", challanDate, currConn, transaction);

                #endregion
                #region Update into table

                sqlText = "  ";
                sqlText = " Update SalesInvoiceHeaders Set DeliveryChallanNo= '";
                sqlText += "@newID where SalesInvoiceNo = @saleInvoiceNo ";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;

                cmd.Parameters.AddWithValue("@newID", newID);
                cmd.Parameters.AddWithValue("@saleInvoiceNo", saleInvoiceNo);

                transResult = (int)cmd.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Update into table

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
        }

        public void SetGatePass(string saleInvoiceNo)
        {
            #region Initializ

            string sqlText = "";
            SqlConnection currConn = null;

            int transResult = 0;
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

                #region Update into table

                sqlText = "  ";
                sqlText = " Update SalesInvoiceHeaders Set IsGatePass= 'Y'";
                sqlText += " where SalesInvoiceNo = @saleInvoiceNo ";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                cmd.Parameters.AddWithValue("@saleInvoiceNo", saleInvoiceNo);

                transResult = (int)cmd.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }


                #endregion Update into table

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
        }

        //==================SelectAll=================
        public List<SaleMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SaleMasterVM likeVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SaleMasterVM> VMs = new List<SaleMasterVM>();
            SaleMasterVM vm;
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
 sih.Id
,sih.SalesInvoiceNo
,sih.CustomerID
,sih.DeliveryAddress1
,sih.DeliveryAddress2
,sih.DeliveryAddress3
,sih.VehicleID
,sih.InvoiceDateTime
,sih.DeliveryDate
,isnull(sih.TotalAmount,0) TotalAmount 
,isnull(sih.TotalVATAmount,0) TotalVATAmount 
,sih.SerialNo
,sih.Comments
,sih.CreatedBy
,sih.CreatedOn
,sih.LastModifiedBy
,sih.LastModifiedOn
,sih.SaleType
,sih.PreviousSalesInvoiceNo
,isnull(sih.Trading,0) Trading 
,sih.IsPrint
,sih.TenderId
,sih.TransactionType
,sih.Post
,sih.LCNumber
,sih.CurrencyID
,isnull(sih.CurrencyRateFromBDT,0) CurrencyRateFromBDT 
,sih.SaleReturnId
,sih.IsVDS
,sih.GetVDSCertificate
,sih.VDSCertificateDate
,sih.ImportIDExcel
,sih.AlReadyPrint
,sih.DeliveryChallanNo
,sih.IsGatePass
,sih.CompInvoiceNo
,sih.LCBank
,sih.LCDate
,sih.ValueOnly
,c.CustomerName
,v.VehicleType
,v.VehicleNo
,cr.CurrencyCode
,cg.CustomerGroupName

FROM SalesInvoiceHeaders sih 
left outer join Customers c on sih.CustomerID=c.CustomerID 
left outer join Vehicles v on sih.VehicleID = v.VehicleID
left outer join Currencies cr on sih.CurrencyID=cr.CurrencyId
left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID
WHERE  1=1
";


                if (Id > 0)
                {
                    sqlText += @" and sih.Id=@Id";
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
                if (likeVM != null)
                {
                    if (!string.IsNullOrWhiteSpace(likeVM.SalesInvoiceNo))
                    {
                        sqlText += " AND sih.SalesInvoiceNo like @SalesInvoiceNo";
                    }
                    if (!string.IsNullOrWhiteSpace(likeVM.SerialNo))
                    {
                        sqlText += " AND sih.SerialNo like @SerialNo";
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
                if (likeVM != null)
                {
                    if (!string.IsNullOrWhiteSpace(likeVM.SalesInvoiceNo))
                    {
                        objComm.Parameters.AddWithValue("@SalesInvoiceNo", "%" + likeVM.SalesInvoiceNo + "%");
                    }
                    if (!string.IsNullOrWhiteSpace(likeVM.SerialNo))
                    {
                        objComm.Parameters.AddWithValue("@SerialNo", "%" + likeVM.SerialNo + "%");
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
                    vm = new SaleMasterVM();
                    vm.Id = dr["Id"].ToString();
                    vm.SalesInvoiceNo = dr["SalesInvoiceNo"].ToString();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();
                    vm.DeliveryAddress1 = dr["DeliveryAddress1"].ToString();
                    vm.DeliveryAddress2 = dr["DeliveryAddress2"].ToString();
                    vm.DeliveryAddress3 = dr["DeliveryAddress3"].ToString();
                    vm.InvoiceDateTime = Ordinary.DateTimeToDate(dr["InvoiceDateTime"].ToString());
                    vm.DeliveryDate = Ordinary.DateTimeToDate(dr["DeliveryDate"].ToString());
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());
                    vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"].ToString());
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.SaleType = dr["SaleType"].ToString();
                    vm.PreviousSalesInvoiceNo = dr["PreviousSalesInvoiceNo"].ToString();
                    vm.Trading = dr["Trading"].ToString();
                    vm.IsPrint = dr["IsPrint"].ToString();
                    vm.TenderId = dr["TenderId"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.LCNumber = dr["LCNumber"].ToString();
                    vm.CurrencyID = dr["CurrencyID"].ToString();
                    vm.CurrencyRateFromBDT = Convert.ToDecimal(dr["CurrencyRateFromBDT"].ToString());
                    vm.CompInvoiceNo = dr["CompInvoiceNo"].ToString();
                    vm.LCBank = dr["LCBank"].ToString();
                    vm.LCDate = dr["LCDate"].ToString();
                    vm.VehicleID = dr["VehicleID"].ToString();
                    vm.VehicleType = dr["VehicleType"].ToString();
                    vm.VehicleNo = dr["VehicleNo"].ToString();
                    
                    vm.CustomerGroup = dr["CustomerGroupName"].ToString();
                    vm.CurrencyCode = dr["CurrencyCode"].ToString();

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
        public List<SaleDetailVM> SelectSaleDetail(string saleInvoiceNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SaleDetailVM> VMs = new List<SaleDetailVM>();
            SaleDetailVM vm;
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
 sd.SalesInvoiceNo
,sd.InvoiceLineNo
,sd.ItemNo
,ISNULL(sd.Quantity,0) Quantity
,ISNULL(sd.SalesPrice,0) SalesPrice  
,ISNULL(sd.NBRPrice,0) NBRPrice  
,ISNULL(sd.AVGPrice,0) AVGPrice  
,sd.UOM
,sd.VATRate
,sd.VATAmount
,ISNULL(sd.SubTotal,0) SubTotal  
,sd.Comments
,sd.CreatedBy
,sd.CreatedOn
,sd.LastModifiedBy
,sd.LastModifiedOn
,ISNULL(sd.SD,0) SD  
,ISNULL(sd.SDAmount,0) SDAmount  
,sd.SaleType
,sd.PreviousSalesInvoiceNo
,sd.Trading
,sd.InvoiceDateTime
,sd.NonStock
,sd.TradingMarkUp
,sd.Type
,sd.BENumber
,sd.Post
,sd.UOMQty
,ISNULL(sd.UOMPrice,0) UOMPrice  
,ISNULL(sd.UOMc,0) UOMc  
,sd.UOMn
,ISNULL(sd.DollerValue,0) DollerValue  
,ISNULL(sd.CurrencyValue,0) CurrencyValue  
,sd.TransactionType
,sd.VATName
,sd.SaleReturnId
,ISNULL(sd.DiscountAmount,0) DiscountAmount  
,ISNULL(sd.DiscountedNBRPrice,0) DiscountedNBRPrice  
,ISNULL(sd.PromotionalQuantity,0) PromotionalQuantity  
,sd.FinishItemNo
,ISNULL(sd.CConversionDate,'19900101') CConversionDate
,sd.ReturnTransactionType
,ISNULL(sd.Weight,0) Weight  
,sd.ValueOnly
,p.ProductName
,p.ProductCode
,sd.TotalValue
,sd.WareHouseRent
,sd.WareHouseVAT
,sd.ATVRate
,sd.ATVablePrice
,sd.ATVAmount
,sd.IsCommercialImporter
FROM SalesInvoiceDetails sd
LEFT OUTER JOIN Products p ON p.ItemNo = sd.ItemNo
WHERE  1=1

";

                if (saleInvoiceNo != null)
                {
                    sqlText += "AND sd.SalesInvoiceNo=@SalesInvoiceNo";
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

                if (saleInvoiceNo != null)
                {
                    objComm.Parameters.AddWithValue("@SalesInvoiceNo", saleInvoiceNo);
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
                    vm = new SaleDetailVM();
                    vm.InvoiceLineNo = dr["InvoiceLineNo"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.SalesPrice = Convert.ToDecimal(dr["SalesPrice"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                    vm.TradingD = dr["Trading"].ToString();
                    vm.NonStockD = dr["NonStock"].ToString();
                    vm.SaleTypeD = dr["SaleType"].ToString();
                    vm.CommentsD = dr["Comments"].ToString();
                    vm.TradingMarkUp = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                    vm.Type = dr["Type"].ToString();
                    vm.Post = dr["Post"].ToString() == "Y" ? true : false;
                    vm.UOMQty = Convert.ToDecimal(dr["UOMQty"].ToString());
                    vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.DollerValue = Convert.ToDecimal(dr["DollerValue"].ToString());
                    vm.CurrencyValue = Convert.ToDecimal(dr["CurrencyValue"].ToString());
                    vm.DiscountAmount = Convert.ToDecimal(dr["DiscountAmount"].ToString());
                    vm.DiscountedNBRPrice = Convert.ToDecimal(dr["DiscountedNBRPrice"].ToString());
                    vm.PromotionalQuantity = Convert.ToDecimal(dr["PromotionalQuantity"].ToString());
                    vm.CConversionDate = Ordinary.DateTimeToDate(dr["CConversionDate"].ToString());
                    vm.ReturnTransactionType = dr["ReturnTransactionType"].ToString();
                    vm.Weight = dr["Weight"].ToString();
                    vm.ValueOnly = dr["ValueOnly"].ToString() == "" ? "N" : dr["ValueOnly"].ToString();
                    vm.VatName = dr["VATName"].ToString();
                    vm.Total = vm.SubTotal + vm.VATAmount;
                    //vm.BDTValue = vm.Total;
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    //new fields
                    vm.TotalValue = Convert.ToDecimal(dr["TotalValue"].ToString() == "" ? "0" : dr["TotalValue"].ToString());
                    vm.WareHouseRent = Convert.ToDecimal(dr["WareHouseRent"].ToString() == "" ? "0" : dr["WareHouseRent"].ToString());
                    vm.WareHouseVAT = Convert.ToDecimal(dr["WareHouseVAT"].ToString() == "" ? "0" : dr["WareHouseVAT"].ToString());
                    vm.ATVRate = Convert.ToDecimal(dr["ATVRate"].ToString() == "" ? "0" : dr["ATVRate"].ToString());
                    vm.ATVablePrice = Convert.ToDecimal(dr["ATVablePrice"].ToString() == "" ? "0" : dr["ATVablePrice"].ToString());
                    vm.ATVAmount = Convert.ToDecimal(dr["ATVAmount"].ToString() == "" ? "0" : dr["ATVAmount"].ToString());
                    vm.IsCommercialImporter = dr["IsCommercialImporter"].ToString() == "" ? "N" : dr["IsCommercialImporter"].ToString();

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

        public string[] ImportExcelFile(SaleMasterVM paramVM)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ImportExcelFile"; //Method Name
            #endregion

            #region try
            try
            {
                DataSet ds = new DataSet();
                //DataTable dt = new DataTable();
                #region Excel Reader

                string FileName = paramVM.File.FileName;
                string Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                System.IO.File.Delete(Fullpath);
                if (paramVM.File != null && paramVM.File.ContentLength > 0)
                {
                    paramVM.File.SaveAs(Fullpath);
                }


                FileStream stream = File.Open(Fullpath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();


                //dt = ds.Tables[0];
                reader.Close();
                System.IO.File.Delete(Fullpath);
                #endregion

                DataTable dtSaleM = new DataTable();
                dtSaleM = ds.Tables["SaleM"];

                DataTable dtSaleD = new DataTable();
                dtSaleD = ds.Tables["SaleD"];

                DataTable dtSaleE = new DataTable();
                dtSaleE = ds.Tables["SaleE"];


                dtSaleM.Columns.Add("Transection_Type");
                dtSaleM.Columns.Add("Created_By");
                dtSaleM.Columns.Add("LastModified_By");
                foreach (DataRow row in dtSaleM.Rows)
                {
                    row["Transection_Type"] = paramVM.TransactionType;
                    row["Created_By"] = paramVM.CreatedBy;
                    row["LastModified_By"] = paramVM.LastModifiedBy;

                }
                dtSaleD.Columns.Add("TotalValue");
                dtSaleD.Columns.Add("WareHouseRent");
                dtSaleD.Columns.Add("WareHouseVAT");
                dtSaleD.Columns.Add("ATVRate");
                dtSaleD.Columns.Add("ATVablePrice");
                dtSaleD.Columns.Add("ATVAmount");
                dtSaleD.Columns.Add("IsCommercialImporter");

                for (int i = 0; i < dtSaleD.Rows.Count; i++)
                {
                    dtSaleD.Rows[i]["TotalValue"] = "0";
                    dtSaleD.Rows[i]["WareHouseRent"] = "0";
                    dtSaleD.Rows[i]["WareHouseVAT"] = "0";
                    dtSaleD.Rows[i]["ATVRate"] = "0";
                    dtSaleD.Rows[i]["ATVablePrice"] = "0";
                    dtSaleD.Rows[i]["ATVAmount"] = "0";
                    dtSaleD.Rows[i]["IsCommercialImporter"] = "N";

                    if (false)
                    {
                        //dtSaleD.Rows[i]["NBR_Price"] =   CommercialImporterCalculation(dtSaleD.Rows[i]["Total_Price"].ToString(), dtSaleD.Rows[i]["VAT_Rate"].ToString(), dtSaleD.Rows[i]["Quantity"].ToString()).ToString();
                        // dtSaleD.Rows[i]["TotalValue"] = cTotalValue;
                        // dtSaleD.Rows[i]["WareHouseRent"] = cWareHouseRent;
                        // dtSaleD.Rows[i]["WareHouseVAT"] = cWareHouseVAT;
                        // dtSaleD.Rows[i]["ATVRate"] = cATVRate;
                        // dtSaleD.Rows[i]["ATVablePrice"] = cATVablePrice;
                        // dtSaleD.Rows[i]["ATVAmount"] = cATVAmount;
                        // dtSaleD.Rows[i]["IsCommercialImporter"] = (CommercialImporter == true ? "Y" : "N").ToString();
                        // dtSaleD.Rows[i]["NBR_Price"] = cVATablePrice.ToString();
                    }
                }

                #region Data Insert
                retResults = ImportData(dtSaleM, dtSaleD, dtSaleE);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }
                #endregion

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                //retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
                return retResults;
            }
            finally
            {
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

    }
}