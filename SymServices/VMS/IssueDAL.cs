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
using SymViewModel.VMS;
using System.Reflection;
using System.IO;
using Excel;

namespace SymServices.VMS
{
    public class IssueDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();
        #endregion

        private void SetDefaultValue(IssueMasterVM vm)
        {
            if(string.IsNullOrWhiteSpace(vm.Comments))
            {
                vm.Comments = "-";
            }
            if(string.IsNullOrWhiteSpace(vm.SerialNo))
            {
                vm.SerialNo = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ReceiveNo))
            {
                vm.ReceiveNo = "-";
            }
        }
        public string[] IssueInsert(IssueMasterVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            #region Check user from settings
            //SettingDAL settingDal=new SettingDAL();
            //   bool isAllowUser = settingDal.CheckUserAccess();
            //   if (!isAllowUser)
            //   {
            //       throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.msgAccessPermision);
            //   }
            #endregion
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
            int nextId = 0;

            #endregion Initializ

            #region Try
            try
            {
                SetDefaultValue(Master);
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.IssueDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.IssueDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "Please Check Issue Data and Time");

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

                    transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);
                }


                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.IssueDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.IssueDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(IssueNo) from IssueHeaders WHERE IssueNo=@MasterIssueNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;

                cmdExistTran.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo ?? Convert.DBNull);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                }

                #endregion Find Transaction Exist
                #region Purchase ID Create
                if (string.IsNullOrEmpty(Master.transactionType)) //start
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                #region Purchase ID Create For Other

                CommonDAL commonDal = new CommonDAL();

                if (Master.transactionType == "Other")
                {
                    newID = commonDal.TransactionCode("Issue", "Other", "IssueHeaders", "IssueNo",
                                              "IssueDateTime", Master.IssueDateTime, currConn, transaction);


                }
                if (Master.transactionType == "IssueReturn")
                {
                    newID = commonDal.TransactionCode("Issue", "IssueReturn", "IssueHeaders", "IssueNo",
                                              "IssueDateTime", Master.IssueDateTime, currConn, transaction);


                }
                #endregion Purchase ID Create For Other



                #endregion Purchase ID Create Not Complete
                #region ID generated completed,Insert new Information in Header


                sqlText = "";
                sqlText += " insert into IssueHeaders(";
                //////sqlText += " Id,";
                sqlText += " IssueNo,";
                sqlText += " IssueDateTime,";
                sqlText += " TotalVATAmount,";
                sqlText += " TotalAmount,";
                sqlText += " SerialNo,";
                sqlText += " Comments,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " ReceiveNo,";
                sqlText += " transactionType,";
                sqlText += " IssueReturnId,";
                sqlText += " ImportIDExcel,";

                sqlText += " Post";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@newID,";
                sqlText += "@MasterIssueDateTime,";
                sqlText += "@MasterTotalVATAmount,";
                sqlText += "@MasterTotalAmount,";
                sqlText += "@MasterSerialNo,";
                sqlText += "@MasterComments,";
                sqlText += "@MasterCreatedBy,";
                sqlText += "@MasterCreatedOn,";
                sqlText += "@MasterReceiveNo,";
                sqlText += "@MastertransactionType,";
                sqlText += "@MasterReturnId,";
                sqlText += "@MasterImportId,";
                sqlText += "@MasterPost";
                sqlText += ") SELECT SCOPE_IDENTITY() ";

                var Id = _cDal.NextId("IssueHeaders", currConn, transaction).ToString();

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                //////cmdInsert.Parameters.AddWithValue("@Id", Id);
                cmdInsert.Parameters.AddWithValue("@newID", newID);
                cmdInsert.Parameters.AddWithValue("@MasterIssueDateTime", Master.IssueDateTime);
                cmdInsert.Parameters.AddWithValue("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdInsert.Parameters.AddWithValue("@MasterTotalAmount", Master.TotalAmount);
                cmdInsert.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterImportId", Master.ImportId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterPost", "N");


                var exec = cmdInsert.ExecuteScalar();

                transResult = Convert.ToInt32(exec);
                Master.Id = transResult.ToString();

                //transResult = (int)cmdInsert.ExecuteNonQuery();
                //Master.Id = Id.ToString();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                }


                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Master.Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table
                var lineNo = 1;
                foreach (var Item in Master.Details)
                {
                    Item.IssueLineNo = lineNo.ToString();
                    lineNo++;
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo='" + newID + "' ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into IssueDetails(";
                    //sqlText += " IssueNo,";
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
                    sqlText += " ReceiveNo,";
                    sqlText += " IssueDateTime,";
                    sqlText += " SD,";
                    sqlText += " SDAmount,";
                    sqlText += " Wastage,";
                    sqlText += " BOMDate,";
                    sqlText += " FinishItemNo,";
                    sqlText += " transactionType,";
                    sqlText += " IssueReturnId,";
                    sqlText += " UOMQty,";
                    sqlText += " UOMPrice,";
                    sqlText += " UOMc,";
                    sqlText += " UOMn,";
                    sqlText += " Post";
                    sqlText += " )";

                    sqlText += " values(	";
                    //sqlText += "'" + Master.Id + "',";

                    sqlText += "@newID,";
                    sqlText += "@ItemIssueLineNo,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemQuantity,";
                    sqlText += " 0,";
                    sqlText += "@ItemCostPrice,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemVATRate,";
                    sqlText += "@ItemVATAmount,";
                    sqlText += "@ItemSubTotal,";
                    sqlText += "@ItemCommentsD,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@ItemReceiveNoD,";
                    sqlText += "@ItemIssueDateTimeD,";
                    sqlText += " 0,	";
                    sqlText += " 0,";
                    sqlText += "@ItemWastage,";
                    sqlText += "@ItemBOMDate,";
                    sqlText += "@ItemFinishItemNo,";
                    sqlText += "@MastertransactionType,";
                    sqlText += "@MasterReturnId,";
                    sqlText += "@ItemUOMQty,";
                    sqlText += "@ItemUOMPrice,";
                    sqlText += "@ItemUOMc,";
                    sqlText += "@ItemUOMn,";
                    sqlText += "@MasterPost";
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValue("@newID", newID);
                    cmdInsDetail.Parameters.AddWithValue("@ItemIssueLineNo", Item.IssueLineNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                    cmdInsDetail.Parameters.AddWithValue("@ItemCostPrice", Item.CostPrice);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                    cmdInsDetail.Parameters.AddWithValue("@ItemVATAmount", Item.VATAmount);
                    cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                    cmdInsDetail.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValue("@ItemReceiveNoD", Item.ReceiveNoD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemIssueDateTimeD", Ordinary.DateToDate(Item.IssueDateTimeD));
                    cmdInsDetail.Parameters.AddWithValue("@ItemWastage", Item.Wastage);
                    cmdInsDetail.Parameters.AddWithValue("@ItemBOMDate", Item.BOMDate ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemFinishItemNo", Item.FinishItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn);
                    cmdInsDetail.Parameters.AddWithValue("@MasterPost", "N");

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
                sqlText = sqlText + "select distinct  Post from dbo.IssueHeaders WHERE IssueNo='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgUnableCreatID);
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
                retResults[1] = MessageVM.issueMsgSaveSuccessfully;
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

        public string[] IssueUpdate(IssueMasterVM Master)
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
            //DateTime MinDate = DateTime.MinValue;
            //DateTime MaxDate = DateTime.MaxValue;
            string PostStatus = "";
                int nextId = 0;

            #endregion Initializ

            #region Try
            try
            {
                SetDefaultValue(Master);

                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.IssueDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.IssueDateTime) > DateTime.MaxValue)
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

                string transactionDate = Master.IssueDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.IssueDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(IssueNo) from IssueHeaders WHERE IssueNo=@MasterIssueNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;

                cmdFindIdUpd.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
                }

                #endregion Find ID for Update



                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update IssueHeaders set  ";

                sqlText += " IssueDateTime  =@MasterIssueDateTime ,";
                sqlText += " TotalVATAmount =@MasterTotalVATAmount ,";
                sqlText += " TotalAmount    =@MasterTotalAmount ,";
                sqlText += " SerialNo       =@MasterSerialNo ,";
                sqlText += " Comments       =@MasterComments ,";
                sqlText += " LastModifiedBy =@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn =@MasterLastModifiedOn ,";
                sqlText += " ReceiveNo      =@MasterReceiveNo ,";
                sqlText += " transactionType=@MastertransactionType ,";
                sqlText += " IssueReturnId  =@MasterReturnId ";
                sqlText += " where  IssueNo =@MasterIssueNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@MasterIssueDateTime", Master.IssueDateTime);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header



                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Master.Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table
                var lineNo = 1;
                foreach (var Item in Master.Details)
                {
                    #region Find Transaction Mode Insert or Update
                    Item.IssueLineNo = lineNo.ToString();
                    lineNo++;
                    sqlText = "";
                    sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterIssueNo ";
                    sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        // Insert
                        #region Insert only DetailTable

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
                        sqlText += " transactionType,";
                        sqlText += " IssueReturnId,";
                        sqlText += " UOMQty,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMc,";
                        sqlText += " UOMn,";
                        sqlText += " Post";
                        sqlText += " )";
                        sqlText += " values(	";
                        //sqlText += "'" + Master.Id + "',";

                        sqlText += "@MasterIssueNo,";
                        sqlText += "@ItemIssueLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += " 0,	";
                        sqlText += "@ItemCostPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += "@ItemVATRate,";
                        sqlText += "@ItemVATAmount,";
                        sqlText += "@ItemSubTotal,";
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@ItemReceiveNoD,";
                        sqlText += "@ItemIssueDateTimeD,";
                        sqlText += " 0,";
                        sqlText += " 0,";
                        sqlText += "@ItemWastage,";
                        sqlText += "@ItemBOMDate,";
                        sqlText += "@ItemFinishItemNo,";
                        sqlText += "@MastertransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";
                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemIssueLineNo", Item.IssueLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCostPrice", Item.CostPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATAmount", Item.VATAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", DateTime.Now.ToString());
                        cmdInsDetail.Parameters.AddWithValue("@ItemReceiveNoD", Item.ReceiveNoD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemIssueDateTimeD", Ordinary.DateToDate(Item.IssueDateTimeD));
                        cmdInsDetail.Parameters.AddWithValue("@ItemWastage", Item.Wastage);
                        cmdInsDetail.Parameters.AddWithValue("@ItemBOMDate", Item.BOMDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemFinishItemNo", Item.FinishItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        }
                        #endregion Insert only DetailTable
                        #region Insert Issue and Receive if Transaction is not Other

                        #endregion Insert Issue and Receive if Transaction is not Other
                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";
                        sqlText += " update IssueDetails set ";

                        sqlText += " IssueLineNo    =@ItemIssueLineNo,";
                        sqlText += " Quantity       =@ItemQuantity,";
                        sqlText += " CostPrice      =@ItemCostPrice,";
                        sqlText += " UOM            =@ItemUOM,";
                        sqlText += " SubTotal       =@ItemSubTotal,";
                        sqlText += " Comments       =@ItemComments,";
                        sqlText += " LastModifiedBy =@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                        sqlText += " ReceiveNo      =@ItemReceiveNo,";
                        sqlText += " IssueDateTime  =@ItemIssueDateTime,";
                        sqlText += " Wastage        =@ItemWastage,";
                        sqlText += " BOMDate        =@ItemBOMDate,";
                        sqlText += " transactionType=@MastertransactionType,";
                        sqlText += " IssueReturnId  =@MasterReturnId,";
                        sqlText += " UOMQty         =@ItemUOMQty,";
                        sqlText += " UOMPrice       =@ItemUOMPrice,";
                        sqlText += " UOMc           =@ItemUOMc,";
                        sqlText += " UOMn           =@ItemUOMn";
                        sqlText += " where  IssueNo =@MasterIssueNo ";
                        sqlText += " and ItemNo     =@ItemItemNo";
                        if (!string.IsNullOrEmpty(Item.FinishItemNo))
                        {
                            if (Item.FinishItemNo != "N/A" && Item.FinishItemNo != "0")
                            {
                                sqlText += " and FinishItemNo=@ItemFinishItemNo";
                            }
                        }


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@ItemIssueLineNo", Item.IssueLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCostPrice", Item.CostPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValue("@ItemComments", Item.CommentsD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@ItemReceiveNo", Item.ReceiveNoD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemIssueDateTime", Item.IssueDateTimeD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemWastage", Item.Wastage);
                        cmdInsDetail.Parameters.AddWithValue("@ItemBOMDate", Item.BOMDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemFinishItemNo", Item.FinishItemNo ?? Convert.DBNull);

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
                sqlText += " from IssueDetails WHERE IssueNo='" + Master.IssueNo + "'";

                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                cmdRIFB.Transaction = transaction;
                SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB);
                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var p = pItem["ItemNo"].ToString();

                    //var tt= Details.Find(x => x.ItemNo == p);
                    var tt = Master.Details.Count(x => x.ItemNo.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM IssueDetails ";
                        sqlText += " WHERE IssueNo=@MasterIssueNo ";
                        sqlText += " AND ItemNo=@p";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                        cmdInsDetail.Parameters.AddWithValue("@p", p);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                    }

                }
                #endregion Remove row


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from IssueHeaders WHERE IssueNo=@MasterIssueNo ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;

                cmdIPS.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

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

        public string[] IssuePost(IssueMasterVM Master)
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
                string vNegStockAllow = string.Empty;
                CommonDAL commonDal = new CommonDAL();
                vNegStockAllow = commonDal.settings("Sale", "NegStockAllow");
                if (string.IsNullOrEmpty(vNegStockAllow))
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Sale");
                }
                bool NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.IssueDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.IssueDateTime) > DateTime.MaxValue)
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
                #region Add BOMId

                //commonDal.TableFieldAdd("IssueDetails", "BOMId", "varchar(20)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMQty", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMPrice", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMc", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMn", "varchar(50)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMWastage", "decimal(25, 9)", currConn); //tablename,fieldName, datatype


                #endregion Add BOMId
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNamePost);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.IssueDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.IssueDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(IssueNo) from IssueHeaders WHERE IssueNo=@MasterIssueNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update IssueHeaders set  ";
                sqlText += " LastModifiedBy=@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn=@MasterLastModifiedOn ,";
                sqlText += " Post=@MasterPost ";
                sqlText += " where  IssueNo=@MasterIssueNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterPost", "Y");
                cmdUpdate.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSuccessfully);
                }
                #endregion update Header



                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Master.Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Master.Details)
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterIssueNo ";
                    sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                    }
                    else
                    {
                        #region Update only DetailTable

                        sqlText = "";
                        sqlText += " update IssueDetails set ";
                        sqlText += " Post=@MasterPost";
                        sqlText += " where  IssueNo =@MasterIssueNo ";
                        sqlText += " and ItemNo=@ItemItemNo";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", "Y");
                        cmdInsDetail.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSuccessfully);
                        }
                        #endregion Update only DetailTable

                        #region Update Item Qty
                        else
                        {
                            #region Find Quantity From Products
                            ProductDAL productDal = new ProductDAL();
                            //decimal oldStock = productDal.StockInHand(Item.ItemNo, Master.IssueDateTime, currConn, transaction);
                            decimal oldStock = Convert.ToDecimal(productDal.AvgPriceNew(Item.ItemNo, Master.IssueDateTime,
                                                              currConn, transaction, true).Rows[0]["Quantity"].ToString());


                            #endregion Find Quantity From Products

                            #region Find Quantity From Transaction

                            sqlText = "";
                            sqlText += "select isnull(Quantity ,0) from IssueDetails ";
                            sqlText += " WHERE ItemNo='" + Item.ItemNo + "' and IssueNo= @MasterIssueNo ";
                            SqlCommand cmdTranQty = new SqlCommand(sqlText, currConn);
                            cmdTranQty.Transaction = transaction;

                            cmdTranQty.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

                            decimal TranQty = (decimal)cmdTranQty.ExecuteScalar();

                            #endregion Find Quantity From Transaction

                            #region Qty  check and Update
                            if (NegStockAllow == false)
                            {
                                if (TranQty > (oldStock + TranQty))
                                {
                                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost,
                                                                    MessageVM.issueMsgStockNotAvailablePost);
                                }
                            }


                            #endregion Qty  check and Update
                        }

                        #endregion Qty  check and Update
                    }
                    #endregion Find Transaction Mode Insert or Update
                }

                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from IssueHeaders WHERE IssueNo=@MasterIssueNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;

                cmdIPS.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

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
                retResults[2] = Master.IssueNo;
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

        public DataTable SearchIssueHeaderDTNew(string IssueNo, string IssueDateFrom,
        string IssueDateTo, string SerialNo, string ReceiveNo, string transactionType, string Post)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("IssueSearchHeader");

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
                #region Add BOMId
                CommonDAL commonDal = new CommonDAL();

                //commonDal.TableFieldAdd("IssueDetails", "BOMId", "varchar(20)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMQty", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMPrice", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMc", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMn", "varchar(50)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMWastage", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                #endregion Add BOMId
                #endregion open connection and transaction

                #region SQL Statement

                sqlText = " ";
                sqlText = @" SELECT  
                            IssueNo,
                            convert (varchar,IssueDateTime,120)IssueDateTime,
                            isnull(TotalVATAmount,0)TotalVATAmount,
                            isnull(TotalAmount,0)TotalAmount ,
                            isnull(SerialNo,'N/A')SerialNo ,
                            isnull(Comments,'N/A')Comments,Post,TransactionType,IssueReturnId
                            FROM         dbo.IssueHeaders

                            WHERE

                            (IssueNo  LIKE '%' +  @IssueNo   + '%' OR @IssueNo IS NULL) 
                            AND (IssueDateTime>= @IssueDateFrom OR @IssueDateFrom IS NULL)
                            AND (IssueDateTime <dateadd(d,1, @IssueDateTo) OR @IssueDateTo IS NULL)
                            and (ReceiveNo  LIKE '%' +  @ReceiveNo   + '%' OR @ReceiveNo IS NULL) 
                            and (Post  LIKE '%' +  @Post   + '%' OR @Post IS NULL) 
                            ";
                if (transactionType == "IssueReturn")
                {
                    sqlText += " and (transactionType in('IssueReturn','ReceiveReturn'))";
                }
                else if (transactionType == "All")
                {
                    sqlText += " and (transactionType not in('IssueReturn','ReceiveReturn'))";
                }
                else

                { sqlText += " AND (transactionType='" + transactionType + "') "; }


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

                if (!objCommIssueHeader.Parameters.Contains("@IssueNo"))
                { objCommIssueHeader.Parameters.AddWithValue("@IssueNo", IssueNo); }
                else { objCommIssueHeader.Parameters["@IssueNo"].Value = IssueNo; }

                if (IssueDateFrom == "")
                {
                    if (!objCommIssueHeader.Parameters.Contains("@IssueDateFrom"))
                    { objCommIssueHeader.Parameters.AddWithValue("@IssueDateFrom", System.DBNull.Value); }
                    else { objCommIssueHeader.Parameters["@IssueDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommIssueHeader.Parameters.Contains("@IssueDateFrom"))
                    { objCommIssueHeader.Parameters.AddWithValue("@IssueDateFrom", IssueDateFrom); }
                    else { objCommIssueHeader.Parameters["@IssueDateFrom"].Value = IssueDateFrom; }
                }
                if (IssueDateTo == "")
                {
                    if (!objCommIssueHeader.Parameters.Contains("@IssueDateTo"))
                    { objCommIssueHeader.Parameters.AddWithValue("@IssueDateTo", System.DBNull.Value); }
                    else { objCommIssueHeader.Parameters["@IssueDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommIssueHeader.Parameters.Contains("@IssueDateTo"))
                    { objCommIssueHeader.Parameters.AddWithValue("@IssueDateTo", IssueDateTo); }
                    else { objCommIssueHeader.Parameters["@IssueDateTo"].Value = IssueDateTo; }
                }

                if (!objCommIssueHeader.Parameters.Contains("@SerialNo"))
                { objCommIssueHeader.Parameters.AddWithValue("@SerialNo", SerialNo); }
                else { objCommIssueHeader.Parameters["@SerialNo"].Value = SerialNo; }


                // Common Filed
                if (!objCommIssueHeader.Parameters.Contains("@ReceiveNo"))
                { objCommIssueHeader.Parameters.AddWithValue("@ReceiveNo", ReceiveNo); }
                else { objCommIssueHeader.Parameters["@ReceiveNo"].Value = ReceiveNo; }
                if (!objCommIssueHeader.Parameters.Contains("@transactionType"))
                { objCommIssueHeader.Parameters.AddWithValue("@transactionType", transactionType); }
                else { objCommIssueHeader.Parameters["@transactionType"].Value = transactionType; }



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

        public DataTable SearchIssueDetailDTNew(string IssueNo, string databaseName)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
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
IssueDetails.IssueNo, 
IssueDetails.IssueLineNo,
IssueDetails.ItemNo, 
isnull(IssueDetails.Quantity,0)Quantity ,
isnull(IssueDetails.CostPrice,0)CostPrice,
isnull(IssueDetails.NBRPrice,0)NBRPrice,
isnull(IssueDetails.UOM,'N/A')UOM ,
isnull(IssueDetails.VATRate,0)VATRate,
isnull(IssueDetails.VATAmount,0)VATAmount,
isnull(IssueDetails.SubTotal,0)SubTotal,
isnull(IssueDetails.Comments,'N/A')Comments,
isnull(Products.ProductName,'N/A')ProductName,
isnull(isnull(Products.OpeningBalance,0)+
isnull(Products.QuantityInHand,0),0) as Stock,
isnull(IssueDetails.SD,0)SD,
isnull(IssueDetails.SDAmount,0)SDAmount,
isnull(Products.ProductCode,'N/A')ProductCode,
isnull(IssueDetails.UOMQty,isnull(IssueDetails.Quantity,0))UOMQty,
isnull(IssueDetails.UOMn,IssueDetails.UOM)UOMn,
isnull(IssueDetails.UOMc,1)UOMc,
isnull(IssueDetails.UOMPrice,isnull(IssueDetails.CostPrice,0))UOMPrice,
isnull(IssueDetails.UOMWastage,isnull(IssueDetails.Wastage,0))UOMWastage,
isnull(IssueDetails.BOMId,0)	BOMId,
isnull(IssueDetails.FinishItemNo,'0')FinishItemNo,
isnull(fp.ProductCode,'N/A')FinishProductCode,
isnull(fp.ProductName,'N/A')FinishProductName

                            FROM         dbo.IssueDetails  left outer join
                            Products on IssueDetails.ItemNo=Products.ItemNo LEFT OUTER JOIN
                            Products fp on IssueDetails.FinishItemNo=fp.ItemNo 
                            
                               
                            WHERE 
                            (IssueNo = @IssueNo ) 
                            ";

                #endregion

                #region SQL Command

                SqlCommand objCommIssueDetail = new SqlCommand();
                objCommIssueDetail.Connection = currConn;

                objCommIssueDetail.CommandText = sqlText;
                objCommIssueDetail.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommIssueDetail.Parameters.Contains("@IssueNo"))
                { objCommIssueDetail.Parameters.AddWithValue("@IssueNo", IssueNo); }
                else { objCommIssueDetail.Parameters["@IssueNo"].Value = IssueNo; }

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
        public List<IssueMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,IssueMasterVM likeVM=null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<IssueMasterVM> VMs = new List<IssueMasterVM>();
            IssueMasterVM vm;
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
 Id
,IssueNo
,IssueDateTime
,ISNULL(TotalVATAmount,0) TotalVATAmount 
,ISNULL(TotalAmount,0) TotalAmount 
,SerialNo
,Comments
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,ReceiveNo
,TransactionType
,IssueReturnId
,Post
,ImportIDExcel

FROM IssueHeaders  
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
                if (likeVM != null)
                {
                    if (!string.IsNullOrWhiteSpace(likeVM.IssueNo))
                    {
                        sqlText += " AND IssueNo like @IssueNo";
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
                    if (!string.IsNullOrWhiteSpace(likeVM.IssueNo))
                    {
                        objComm.Parameters.AddWithValue("@IssueNo", "%"+likeVM.IssueNo+"%");
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
                    vm = new IssueMasterVM();
                    vm.Id = dr["Id"].ToString();
                    vm.IssueNo = dr["IssueNo"].ToString();
                    vm.IssueDateTime = Ordinary.DateTimeToDate(dr["IssueDateTime"].ToString());
                    vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"]);
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.ReceiveNo = dr["ReceiveNo"].ToString();
                    vm.transactionType = dr["TransactionType"].ToString();
                    vm.Post = dr["Post"].ToString();

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
        public string[] ImportData(DataTable dtIssueM, DataTable dtIssueD)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            IssueMasterVM issueMasterVM;
            List<IssueDetailVM> issueDetailVMs = new List<IssueDetailVM>();

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
                int MRow = dtIssueM.Rows.Count;
                for (int i = 0; i < dtIssueM.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtIssueM.Rows[i]["ID"].ToString()))
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
                    string importID = dtIssueM.Rows[i]["ID"].ToString();
                    if (!string.IsNullOrEmpty(importID))
                    {
                        DataRow[] DetailRaws = dtIssueD.Select("ID='" + importID + "'");
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
                    string id = dtIssueM.Rows[i]["ID"].ToString();
                    DataRow[] tt = dtIssueM.Select("ID='" + id + "'");
                    if (tt.Length > 1)
                    {
                        throw new ArgumentNullException("you have duplicate master id " + id + " in import file.");
                    }

                }

                #endregion Double ID in Master


                CommonImport cImport = new CommonImport();

                #region checking from database is exist the information(NULL Check)

                #region Master

                for (int j = 0; j < MRowCount; j++)
                {
                    #region Checking Date is null or different formate

                    bool IsIssueDate;
                    IsIssueDate = cImport.CheckDate(dtIssueM.Rows[j]["Issue_DateTime"].ToString().Trim());
                    if (IsIssueDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Issue_Date field.");
                    }
                    #endregion Checking Date is null or different formate

                    #region Checking Y/N value
                    bool post;
                    post = cImport.CheckYN(dtIssueM.Rows[j]["Post"].ToString().Trim());
                    if (post != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Post field.");
                    }
                    #endregion Checking Y/N value

                    #region Check Return issue id
                    string ReturnId = string.Empty;
                    ReturnId = cImport.CheckIssueReturnID(dtIssueM.Rows[j]["Return_Id"].ToString().Trim(), currConn, transaction);
                    #endregion Check Return issue id
                }

                #endregion Master

                #region Details

                #region Row count for details table
                int DRowCount = 0;
                for (int i = 0; i < dtIssueD.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtIssueD.Rows[i]["ID"].ToString()))
                    {
                        DRowCount++;
                    }

                }

                #endregion Row count for details table

                for (int i = 0; i < DRowCount; i++)
                {
                    string ItemNo = string.Empty;
                    string UOMn = string.Empty;

                    #region FindItemId
                    if (string.IsNullOrEmpty(dtIssueD.Rows[i]["Item_Code"].ToString().Trim()))
                    {
                        throw new ArgumentNullException("Please insert value in 'Item_Code' field.");
                    }
                    ItemNo = cImport.FindItemId(dtIssueD.Rows[i]["Item_Name"].ToString().Trim()
                                                , dtIssueD.Rows[i]["Item_Code"].ToString().Trim(), currConn, transaction);

                    #endregion FindItemId

                    #region FindUOMn

                    UOMn = cImport.FindUOMn(ItemNo, currConn, transaction);

                    #endregion FindUOMn

                    #region FindUOMn
                    if (DatabaseInfoVM.DatabaseName.ToString() != "Sanofi_DB")
                    {
                        cImport.FindUOMc(UOMn, dtIssueD.Rows[i]["UOM"].ToString().Trim(), currConn, transaction);
                    }
                    #endregion FindUOMn

                    #region Numeric value check
                    bool IsQuantity = cImport.CheckNumericBool(dtIssueD.Rows[i]["Quantity"].ToString().Trim());
                    if (IsQuantity != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Quantity field.");
                    }
                    #endregion Numeric value check
                }

                #endregion Details


                #endregion checking from database is exist the information(NULL Check)

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                    currConn.Open();
                    transaction = currConn.BeginTransaction("Import Data.");
                }
                decimal TotalAmount;
                for (int j = 0; j < MRowCount; j++)
                {
                    TotalAmount = 0;

                    #region Master Issue

                    string importID = dtIssueM.Rows[j]["ID"].ToString().Trim();
                    DateTime issueDateTime = Convert.ToDateTime(dtIssueM.Rows[j]["Issue_DateTime"].ToString().Trim());
                    #region CheckNull
                    string serialNo = cImport.ChecKNullValue(dtIssueM.Rows[j]["Reference_No"].ToString().Trim());
                    string comments = cImport.ChecKNullValue(dtIssueM.Rows[j]["Comments"].ToString().Trim());
                    #endregion CheckNull

                    #region Check Return issue id
                    string issueReturnId = cImport.CheckIssueReturnID(dtIssueM.Rows[j]["Return_Id"].ToString().Trim(), currConn, transaction);
                    #endregion Check Return receive id
                    string post = dtIssueM.Rows[j]["Post"].ToString().Trim();
                    string createdBy = dtIssueM.Rows[j]["Created_By"].ToString().Trim();
                    string lastModifiedBy = dtIssueM.Rows[j]["LastModified_By"].ToString().Trim();
                    string transactionType = dtIssueM.Rows[j]["Transection_Type"].ToString().Trim();

                    issueMasterVM = new IssueMasterVM();

                    issueMasterVM.IssueDateTime =
                        issueDateTime.ToString("yyyy-MM-dd") +
                                           DateTime.Now.ToString(" HH:mm:ss");
                    issueMasterVM.TotalVATAmount = 0;
                    issueMasterVM.TotalAmount = Convert.ToDecimal(0);
                    issueMasterVM.SerialNo = serialNo.Replace(" ", "");
                    issueMasterVM.Comments = comments;
                    issueMasterVM.CreatedBy = createdBy;
                    issueMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    issueMasterVM.LastModifiedBy = lastModifiedBy;
                    issueMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    issueMasterVM.ReturnId = issueReturnId;
                    issueMasterVM.transactionType = transactionType;
                    issueMasterVM.Post = post;
                    issueMasterVM.ImportId = importID;
                    DataRow[] DetailRaws; //= new DataRow[];//

                    #region MAtch

                    if (!string.IsNullOrEmpty(importID))
                    {
                        DetailRaws = dtIssueD.Select("ID='" + importID + "'");
                    }
                    else
                    {
                        DetailRaws = null;
                    }

                    #endregion MAtch

                    #endregion Master Issue

                    #region Details Issue

                    int counter = 1;
                    issueDetailVMs = new List<IssueDetailVM>();
                    // Juwel 13/10/2015
                    DataTable dtDistinctItem = DetailRaws.CopyToDataTable().DefaultView.ToTable(true, "Item_Code", "Item_Name");

                    DataTable dtIssueDetail = DetailRaws.CopyToDataTable();

                    foreach (DataRow item in dtDistinctItem.Rows)
                    {
                        DataTable dtRepeatedItems = dtIssueDetail.Select("[Item_Code] ='" + item["Item_Code"].ToString() + "'").CopyToDataTable();

                        string itemCode = item["Item_Code"].ToString().Trim();
                        string itemName = item["Item_Name"].ToString().Trim();
                        string itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);
                        decimal quantity = 0;
                        decimal avgPrice;
                        CommonDAL cmnDal = new CommonDAL();
                        DataTable priceData = cImport.FindAvgPriceImport(itemNo, issueDateTime.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction);
                        decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                        decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                        if (quan > 0)
                        {
                            avgPrice = cmnDal.FormatingDecimal((amount / quan).ToString());
                        }
                        else
                        {
                            avgPrice = 0;
                        }

                        string uOM = "";
                        string uOMn = "";
                        string uOMc = "";


                        IssueDetailVM detail = new IssueDetailVM();

                        foreach (DataRow row in dtRepeatedItems.Rows)
                        {
                            if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                            {
                                uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                                uOM = uOMn;
                                uOMc = "1";
                            }
                            else
                            {
                                uOM = row["UOM"].ToString().Trim();
                                uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                                uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction);
                            }
                            quantity = quantity + Convert.ToDecimal(row["Quantity"].ToString().Trim());
                        }

                        detail.ItemNo = itemNo;
                        detail.IssueLineNo = counter.ToString();
                        detail.Quantity = Convert.ToDecimal(quantity);
                        detail.NBRPrice = 0;
                        detail.VATRate = 0;
                        detail.VATAmount = 0;
                        detail.UOM = uOM;
                        detail.SD = 0;
                        detail.SDAmount = 0;
                        detail.CommentsD = "NA";
                        detail.IssueDateTimeD =
                            issueDateTime.ToString("yyyy-MM-dd") +
                                               DateTime.Now.ToString(" HH:mm:ss");
                        detail.BOMDate = "1900-01-01";
                        detail.FinishItemNo = "0";
                        detail.UOMn = uOMn;
                        detail.UOMc = Convert.ToDecimal(uOMc);
                        detail.Wastage = 0;

                        detail.CostPrice = Convert.ToDecimal(avgPrice);

                        detail.SubTotal = Convert.ToDecimal(Convert.ToDecimal(avgPrice) * Convert.ToDecimal(quantity));
                        TotalAmount = TotalAmount + Convert.ToDecimal(Convert.ToDecimal(avgPrice) * Convert.ToDecimal(quantity));
                        detail.UOMQty = Convert.ToDecimal(Convert.ToDecimal(quantity) / Convert.ToDecimal(uOMc));
                        detail.UOMPrice = Convert.ToDecimal(Convert.ToDecimal(avgPrice) / Convert.ToDecimal(uOMc));

                        issueDetailVMs.Add(detail);
                        counter++;
                    }
                    #region Previous code by ruba apu 13/10/2015
                    //foreach (DataRow row in DetailRaws)
                    //{
                    //    //string itemCode = row["Item_Code"].ToString().Trim();
                    //    //string itemName = row["Item_Name"].ToString().Trim();

                    //    //string itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);

                    //    //string quantity = row["Quantity"].ToString().Trim();
                    //    //string uOM ="";
                    //    //string uOMn="";
                    //    //string uOMc = "";
                    //    //if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                    //    //{
                    //    //    uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                    //    //    uOM = uOMn;
                    //    //    uOMc = "1";
                    //    //}
                    //    //else
                    //    //{
                    //    //    uOM = row["UOM"].ToString().Trim();
                    //    //    uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                    //    //    uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction);
                    //    //}

                    //    //IssueDetailVM detail = new IssueDetailVM();
                    //    detail.ItemNo = itemNo;
                    //    detail.Quantity = Convert.ToDecimal(quantity);
                    //    detail.NBRPrice = 0;
                    //    detail.VATRate = 0;
                    //    detail.VATAmount = 0;
                    //    detail.UOM = uOM;
                    //    detail.SD = 0;
                    //    detail.SDAmount = 0;
                    //    detail.CommentsD = "NA";
                    //    detail.IssueDateTimeD =
                    //        issueDateTime.ToString("yyyy-MM-dd") +
                    //                           DateTime.Now.ToString(" HH:mm:ss");
                    //    detail.BOMDate = "1900-01-01";
                    //    detail.FinishItemNo = "0";
                    //    detail.UOMn = uOMn;
                    //    detail.UOMc = Convert.ToDecimal(uOMc);
                    //    detail.Wastage = 0;

                    //    CommonDAL cmnDal = new CommonDAL();
                    //    decimal avgPrice;

                    //        DataTable priceData = cImport.FindAvgPriceImport(itemNo, issueDateTime.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction);
                    //        decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                    //        decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                    //        if (quan > 0)
                    //        {
                    //            avgPrice = cmnDal.FormatingDecimal((amount/quan).ToString());
                    //        }
                    //        else
                    //        {
                    //            avgPrice = 0;
                    //        }

                    //    //detail.CostPrice = cmnDal.FormatingDecimal(avgPrice);
                    //    detail.CostPrice = Convert.ToDecimal(avgPrice);

                    //    detail.SubTotal = Convert.ToDecimal(Convert.ToDecimal(avgPrice) * Convert.ToDecimal(quantity));
                    //    detail.UOMQty = Convert.ToDecimal(Convert.ToDecimal(quantity) / Convert.ToDecimal(uOMc));
                    //    detail.UOMPrice = Convert.ToDecimal(Convert.ToDecimal(avgPrice) / Convert.ToDecimal(uOMc));

                    //    issueDetailVMs.Add(detail);
                    //    counter++;
                    //} // detail
                    #endregion previous code

                    #endregion Details Issue
                    issueMasterVM.TotalAmount = Convert.ToDecimal(TotalAmount);
                    issueMasterVM.Details = issueDetailVMs;//added by Robin
                    string[] sqlResults = IssueInsert(issueMasterVM, transaction, currConn);
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

        public decimal ReturnIssueQty(string issueReturnId, string itemNo)
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

                sqlText = "select Sum(isnull(IssueDetails.Quantity,0)) from IssueDetails ";
                sqlText += "where ItemNo = '" + itemNo + "' and IssueReturnId = '" + issueReturnId + "'";
                sqlText += "group by ItemNo";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
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

        //==================SelectAll=================
        public List<IssueDetailVM> SelectIssueDetail(string issueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<IssueDetailVM> VMs = new List<IssueDetailVM>();
            IssueDetailVM vm;
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
 iss.IssueNo
,iss.IssueLineNo
,iss.ItemNo
,ISNULL(iss.Quantity,0) Quantity  
,ISNULL(iss.NBRPrice,0) NBRPrice  
,ISNULL(iss.CostPrice,0) CostPrice  
,iss.UOM
,ISNULL(iss.VATRate,0) VATRate  
,ISNULL(iss.VATAmount,0) VATAmount  
,ISNULL(iss.SubTotal,0) SubTotal  
,iss.Comments
,iss.CreatedBy
,iss.CreatedOn
,iss.LastModifiedBy
,iss.LastModifiedOn
,iss.ReceiveNo
,iss.IssueDateTime
,ISNULL(iss.SD,0) SD  
,ISNULL(iss.SDAmount,0) SDAmount  
,ISNULL(iss.Wastage,0) Wastage  
,iss.BOMDate
,iss.FinishItemNo
,iss.Post
,iss.TransactionType
,iss.IssueReturnId
,ISNULL(iss.DiscountAmount,0) DiscountAmount  
,ISNULL(iss.DiscountedNBRPrice,0) DiscountedNBRPrice  
,iss.UOMQty
,ISNULL(iss.UOMPrice,0) UOMPrice  
,ISNULL(iss.UOMc,0) UOMc  
,iss.UOMn
,iss.BOMId
,ISNULL(iss.UOMWastage,0) UOMWastage  
,iss.IsProcess
,p.ProductCode
,p.ProductName
FROM IssueDetails iss left outer join Products p on iss.ItemNo=p.ItemNo
WHERE  1=1

";
                if (issueNo != null)
                {
                    sqlText += "AND iss.IssueNo=@issueNo";
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

                if (issueNo != null)
                {
                    objComm.Parameters.AddWithValue("@issueNo", issueNo);
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
                ProductDAL ProductdDal;
                while (dr.Read())
                {
                    vm = new IssueDetailVM();
                    vm.IssueNo = dr["IssueNo"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                    vm.Wastage = Convert.ToDecimal(dr["Wastage"].ToString());
                    vm.FinishItemNo = dr["FinishItemNo"].ToString();
                    vm.Post = dr["Post"].ToString() == "Y" ? true : false;
                    vm.UOMQty = Convert.ToDecimal(dr["UOMQty"].ToString());
                    vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();

                    vm.ItemName = dr["ProductName"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();

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

        public string[] ImportExcelFile(IssueMasterVM paramVM)
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

                DataTable dtIssueM = new DataTable();
                dtIssueM = ds.Tables["IssueM"];

                DataTable dtIssueD = new DataTable();
                dtIssueD = ds.Tables["IssueD"];


                dtIssueM.Columns.Add("Transection_Type");
                dtIssueM.Columns.Add("Created_By");
                dtIssueM.Columns.Add("LastModified_By");
                foreach (DataRow row in dtIssueM.Rows)
                {
                    row["Transection_Type"] = paramVM.transactionType;
                    row["Created_By"] = paramVM.CreatedBy;
                    row["LastModified_By"] = paramVM.CreatedBy;

                }


                //dt = ds.Tables[0].Select("empCode <>''").CopyToDataTable();

                #region Data Insert
                //PurchaseDAL puchaseDal = new PurchaseDAL();
                retResults = ImportData(dtIssueM, dtIssueD);
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

