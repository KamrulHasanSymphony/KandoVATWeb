using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SymViewModel.VMS;
using System.Data.SqlClient;
using System.Data;
using SymOrdinary;

namespace SymServices.VMS
{
    public class VAT7DAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion

        public string[] Vat7Insert(VAT7VM Master, List<VAT7VM> Details)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string VAT7Id="";

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
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.Vat7DateTime) < DateTime.MinValue || Convert.ToDateTime(Master.Vat7DateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "Please Check VAT7 Data and Time");

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

                string transactionDate = Master.Vat7DateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.Vat7DateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(VAT7No) from VAT7 WHERE VAT7No=@MasterVAT7No ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValue("@MasterVAT7No", Master.VAT7No);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                }

                #endregion Find Transaction Exist
                #region VAT7 ID Create
                CommonDAL commonDal = new CommonDAL();
                newID = commonDal.TransactionCode("VAT7", "VAT7", "VAT7", "VAT7No", "Vat7Date", Master.Vat7DateTime, currConn, transaction);
                #endregion VAT7 ID Create
                #region ID generated completed,Insert new Information in Header


                //sqlText = "";
                //sqlText += " insert into VAT7(";
                //sqlText += " VAT7No ,";
                //sqlText += " Vat7Date ,";
                //sqlText += " FinishItemNo ,";
                //sqlText += " FinishUOM ,";
                //sqlText += " Vat7LineNo ,";
                //sqlText += " ItemNo ,";
                //sqlText += " UOM ,";
                //sqlText += " Quantity ,";
                //sqlText += " UOMQty ,";
                //sqlText += " UOMc ,";
                //sqlText += " UOMn,";
                //sqlText += " CreatedBy ,";
                //sqlText += " CreatedOn ,";
                //sqlText += " LastModifiedBy ,";
                //sqlText += " LastModifiedOn ,";

                //sqlText += " Post ";
                //sqlText += " )";

                //sqlText += " values";
                //sqlText += " (";
                //sqlText += "'" + newID + "',";
                //sqlText += " '" + Master.Vat7DateTime + "',";
                //sqlText += " '" + Master.FinishItemNo + "',";
                //sqlText += " '" + Master.FinishUOM + "',";

                //sqlText += " '" + Master.Vat7LineNo + "',";
                //sqlText += " '" + Master.ItemNo + "',";
                //sqlText += " '" + Master.ItemUOM + "',";
                //sqlText += " '" + Master.Quantity + "',";
                //sqlText += " '" + Master.UOMQty + "',";
                //sqlText += " '" + Master.UOMc + "',";
                //sqlText += " '" + Master.UOMn + "',";

                //sqlText += " '" + Master.CreatedBy + "',";
                //sqlText += " '" + Master.CreatedOn + "',";
                //sqlText += " '" + Master.LastModifiedBy + "',";
                //sqlText += " '" + Master.LastModifiedOn + "',";
                //sqlText += "'" + Master.Post + "'";
                //sqlText += ")";


                //SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                //cmdInsert.Transaction = transaction;
                //transResult = (int)cmdInsert.ExecuteNonQuery();
                //if (transResult <= 0)
                //{
                //    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                //}


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
                    sqlText += "select COUNT(VAT7No) from VAT7 WHERE VAT7No=@ewID ";
                    sqlText += " AND FinishItemNo=@ItemFinishItemNo ";
                    sqlText += " AND ItemNo=@ItemItemNo ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@ewID", newID);
                    cmdFindId.Parameters.AddWithValue("@ItemFinishItemNo", Item.FinishItemNo);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);


                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into VAT7(";
                    sqlText += " VAT7No ,";
                    sqlText += " Vat7Date ,";
                    sqlText += " FinishItemNo ,";
                    sqlText += " FinishUOM ,";
                    sqlText += " Vat7LineNo ,";
                    sqlText += " ItemNo ,";
                    sqlText += " UOM ,";
                    sqlText += " Quantity ,";
                    sqlText += " UOMQty ,";
                    sqlText += " UOMc ,";
                    sqlText += " UOMn,";
                    sqlText += " CreatedBy ,";
                    sqlText += " CreatedOn ,";
                    sqlText += " LastModifiedBy ,";
                    sqlText += " LastModifiedOn ,";

                    sqlText += " Post ";
                    sqlText += " )";

                    sqlText += " values";
                    sqlText += " (";
                    sqlText += "@newID,";
                    sqlText += "@MasterVat7DateTime,";
                    sqlText += "@ItemFinishItemNo,";
                    sqlText += "@ItemFinishUOM,";
                    sqlText += "@ItemVat7LineNo,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemItemUOM,";
                    sqlText += "@ItemQuantity,";
                    sqlText += "@ItemUOMQty,";
                    sqlText += "@ItemUOMc,";
                    sqlText += "@ItemUOMn,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@MasterPost";
                    sqlText += "); SELECT SCOPE_IDENTITY()";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValue("@newID", newID);
                    cmdInsDetail.Parameters.AddWithValue("@MasterVat7DateTime", Master.Vat7DateTime);
                    cmdInsDetail.Parameters.AddWithValue("@ItemFinishItemNo", Item.FinishItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemFinishUOM", Item.FinishUOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemVat7LineNo", Item.Vat7LineNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemItemUOM", Item.ItemUOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post);

                    transResult = Convert.ToInt32(cmdInsDetail.ExecuteScalar());
                    VAT7Id = transResult.ToString();
                    #endregion Insert only DetailTable


                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)
                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.VAT7 WHERE VAT7No=@newID ";
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
                retResults[2] = "" + VAT7Id;
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
        public string[] Vat7update(VAT7VM Master, List<VAT7VM> Details)
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
                else if (Convert.ToDateTime(Master.Vat7DateTime) < DateTime.MinValue || Convert.ToDateTime(Master.Vat7DateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, "Please Check VAT7 Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.Vat7DateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.Vat7DateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(VAT7No) from VAT7 WHERE VAT7No=@MasterVAT7No  ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterVAT7No", Master.VAT7No);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
                }

                #endregion Find ID for Update

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
                    sqlText += "select COUNT(VAT7No) from VAT7 WHERE VAT7No=@MasterVAT7No ";
                    sqlText += " AND FinishItemNo=@ItemFinishItemNo ";
                    sqlText += " AND ItemNo=@ItemItemNo ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@MasterVAT7No", Master.VAT7No);
                    cmdFindId.Parameters.AddWithValue("@ItemFinishItemNo", Item.FinishItemNo);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        // Insert
                        #region Insert only DetailTable
                        sqlText = "";
                        sqlText += " insert into VAT7(";
                        sqlText += " VAT7No ,";
                        sqlText += " Vat7Date ,";
                        sqlText += " FinishItemNo ,";
                        sqlText += " FinishUOM ,";
                        sqlText += " Vat7LineNo ,";
                        sqlText += " ItemNo ,";
                        sqlText += " UOM ,";
                        sqlText += " Quantity ,";
                        sqlText += " UOMQty ,";
                        sqlText += " UOMc ,";
                        sqlText += " UOMn,";
                        sqlText += " CreatedBy ,";
                        sqlText += " CreatedOn ,";
                        sqlText += " LastModifiedBy ,";
                        sqlText += " LastModifiedOn ,";

                        sqlText += " Post ";
                        sqlText += " )";

                        sqlText += " values";
                        sqlText += " (";
                        sqlText += "@MasterVAT7No,";
                        sqlText += "@MasterVat7DateTime,";
                        sqlText += "@ItemFinishItemNo,";
                        sqlText += "@ItemFinishUOM,";
                        sqlText += "@ItemVat7LineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemItemUOM,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@MasterPost";
                        sqlText += ")";



                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterVAT7No", Master.VAT7No ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterVat7DateTime", Master.Vat7DateTime);
                        cmdInsDetail.Parameters.AddWithValue("@ItemFinishItemNo", Item.FinishItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemFinishUOM", Item.FinishUOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVat7LineNo", Item.Vat7LineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemUOM", Item.ItemUOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
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
                        sqlText += " update VAT7 set ";

                        sqlText += " Vat7LineNo         =@ItemVat7LineNo,";
                        sqlText += " Quantity           =@ItemQuantity,";
                        sqlText += " UOM                =@ItemItemUOM,";
                        sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                        sqlText += " Vat7Date           =@MasterVat7DateTime,";
                        sqlText += " UOMQty             =@ItemUOMQty,";
                        sqlText += " UOMc               =@ItemUOMc,";
                        sqlText += " UOMn               =@ItemUOMn,";
                        sqlText += " Post               =@MasterPost";
                        sqlText += " where  VAT7No      =@MasterVAT7No ";
                        sqlText += " AND FinishItemNo   =@ItemFinishItemNo";
                        sqlText += " and ItemNo         =@ItemItemNo";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@ItemVat7LineNo", Item.Vat7LineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemUOM", Item.ItemUOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@MasterVat7DateTime", Master.Vat7DateTime);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterVAT7No", Master.VAT7No ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemFinishItemNo", Item.FinishItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

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
                sqlText += " SELECT  distinct ItemNo,FinishItemNo";
                sqlText += " from VAT7 WHERE VAT7No='" + Master.VAT7No + "'";

                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                cmdRIFB.Transaction = transaction;
                SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB);
                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var itemNo = pItem["ItemNo"].ToString();
                    var fItemNo = pItem["FinishItemNo"].ToString();

                    //var tt= Details.Find(x => x.ItemNo == p);
                    //var tt = Details.Count(x => x.ItemNo.Trim() == itemNo.Trim());
                    int itemCount = Details.Where(x => x.ItemNo == itemNo && x.FinishItemNo == fItemNo).Count();

                    if (itemCount == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM VAT7 ";
                        sqlText += " WHERE VAT7No       =@MasterVAT7No ";
                        sqlText += " AND FinishItemNo   =@fItemNo";
                        sqlText += " AND ItemNo         =@itemNo";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterVAT7No", Master.VAT7No ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@fItemNo", fItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@itemNo", itemNo ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                    }

                }
                #endregion Remove row


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)
                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from VAT7 WHERE VAT7No=@MasterVAT7No ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterVAT7No", Master.VAT7No);

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
        public string[] VAT7Post(VAT7VM Master, List<VAT7VM> Details)
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
                else if (Convert.ToDateTime(Master.Vat7DateTime) < DateTime.MinValue || Convert.ToDateTime(Master.Vat7DateTime) > DateTime.MaxValue)
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

                string transactionDate = Master.Vat7DateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.Vat7DateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(VAT7No) from VAT7 WHERE VAT7No=@MasterVAT7No ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterVAT7No", Master.VAT7No);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update
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
                    sqlText += "select COUNT(VAT7No) from VAT7 WHERE VAT7No=@MasterVAT7No ";
                    sqlText += " AND FinishItemNo=@ItemFinishItemNo";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@MasterVAT7No", Master.VAT7No);
                    cmdFindId.Parameters.AddWithValue("@ItemFinishItemNo", Item.FinishItemNo);
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
                        sqlText += " update VAT7 set ";
                        sqlText += " Post               =@MasterPost";
                        sqlText += " where  VAT7No      =@MasterVAT7No ";
                        sqlText += " AND FinishItemNo   =@ItemFinishItemNo";
                        sqlText += " and ItemNo         =@ItemItemNo";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterVAT7No", Master.VAT7No ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemFinishItemNo", Item.FinishItemNo ?? Convert.DBNull);
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
                sqlText = sqlText + "select distinct Post from VAT7 WHERE VAT7No=@MasterVAT7No ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterVAT7No", Master.VAT7No);
 
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
                retResults[2] = Master.VAT7No;
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
        public DataTable SearchVat7HeaderDT(string Vat7No, string VAT7DateFrom,string VAT7DateTo,string Post)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("Vat7Search");

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
                sqlText = @" SELECT Distinct 
                                    VAT7No,
                                    convert (varchar,Vat7Date,120)Vat7Date,
                                    Post
                                    FROM  VAT7
                                    WHERE
                                        (VAT7No  LIKE '%' +  @VAT7No   + '%' OR @VAT7No IS NULL) 
                                        AND (Vat7Date>= @VAT7DateFrom OR @VAT7DateFrom IS NULL)
                                        AND (Vat7Date <dateadd(d,1, @VAT7DateTo) OR @VAT7DateTo IS NULL)
                                       --and (ReceiveNo  LIKE '%' +  @ReceiveNo   + '%' OR @ReceiveNo IS NULL) 
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

                if (!objCommIssueHeader.Parameters.Contains("@Vat7No"))
                { objCommIssueHeader.Parameters.AddWithValue("@Vat7No", Vat7No); }
                else { objCommIssueHeader.Parameters["@Vat7No"].Value = Vat7No; }

                if (VAT7DateFrom == "")
                {
                    if (!objCommIssueHeader.Parameters.Contains("@VAT7DateFrom"))
                    { objCommIssueHeader.Parameters.AddWithValue("@VAT7DateFrom", System.DBNull.Value); }
                    else { objCommIssueHeader.Parameters["@VAT7DateFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommIssueHeader.Parameters.Contains("@VAT7DateFrom"))
                    { objCommIssueHeader.Parameters.AddWithValue("@VAT7DateFrom", VAT7DateFrom); }
                    else { objCommIssueHeader.Parameters["@VAT7DateFrom"].Value = VAT7DateFrom; }
                }
                if (VAT7DateTo == "")
                {
                    if (!objCommIssueHeader.Parameters.Contains("@VAT7DateTo"))
                    { objCommIssueHeader.Parameters.AddWithValue("@VAT7DateTo", System.DBNull.Value); }
                    else { objCommIssueHeader.Parameters["@VAT7DateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommIssueHeader.Parameters.Contains("@VAT7DateTo"))
                    { objCommIssueHeader.Parameters.AddWithValue("@VAT7DateTo", VAT7DateTo); }
                    else { objCommIssueHeader.Parameters["@VAT7DateTo"].Value = VAT7DateTo; }
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

        public DataTable SearchVat7DetailDT(string Vat7No)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("VAT7SearchDetail");

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
       VAT7.FinishItemNo
      ,isnull(fp.ProductCode,'N/A') FinishProductCode
      ,isnull(fp.ProductName,'N/A') FinishProductName
      ,isnull(VAT7.FinishUOM,'N/A') FinishUOM
      ,isnull(VAT7.Vat7LineNo,0) Vat7LineNo
      ,isnull(VAT7.ItemNo,'N/A') ItemNo
	  ,isnull(Products.ProductCode,'N/A') ProductCode
      ,isnull(Products.ProductName,'N/A') ProductName
      ,isnull(VAT7.UOM,'N/A') UOM
      ,isnull(VAT7.Quantity,0) Quantity
      ,isnull(VAT7.UOMQty,0) UOMQty
      ,isnull(VAT7.UOMc,0) UOMc
      ,isnull(VAT7.UOMn,'N/A') UOMn
	  
	   FROM VAT7 left outer join Products on VAT7.ItemNo=Products.ItemNo 
                 LEFT OUTER JOIN Products fp on VAT7.FinishItemNo=fp.ItemNo 

       WHERE (Vat7No = @Vat7No ) 
                            ";

                #endregion

                #region SQL Command

                SqlCommand objCommIssueDetail = new SqlCommand();
                objCommIssueDetail.Connection = currConn;

                objCommIssueDetail.CommandText = sqlText;
                objCommIssueDetail.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommIssueDetail.Parameters.Contains("@Vat7No"))
                { objCommIssueDetail.Parameters.AddWithValue("@Vat7No", Vat7No); }
                else { objCommIssueDetail.Parameters["@Vat7No"].Value = Vat7No; }

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

        public List<VAT7VM> SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<VAT7VM> VMs = new List<VAT7VM>();
            VAT7VM vm;
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
 v.VAT7No
,v.Id
,v.Vat7Date
,v.FinishItemNo
,v.FinishUOM
,v.Post
,v.CreatedBy
,v.CreatedOn
,v.LastModifiedBy
,v.LastModifiedOn
,p.ProductName
,p.ProductCode
,v.ItemNo
,v.UOM
,v.UOMn
,isnull(v.Quantity,0) Quantity
,v.UOMc
,v.Post
,pr.ProductName RawProductName
,pr.ProductCode RawProductCode

FROM VAT7 v left outer join Products P on v.FinishItemNo=P.ItemNo
left outer join Products Pr on v.ItemNo=Pr.ItemNo
WHERE  1=1 

";
                if (Id != null)
                {
                    sqlText += @" and v.Id=@Id";
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

                if (Id != null)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new VAT7VM();
                    vm.Id = dr["Id"].ToString();
                    vm.VAT7No = dr["VAT7No"].ToString();
                    vm.Vat7DateTime = dr["Vat7Date"].ToString();
                    vm.FinishItemNo = dr["FinishItemNo"].ToString();
                    vm.FinishUOM = dr["FinishUOM"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.FinishItemName = dr["ProductName"].ToString();
                    vm.FinishItemCode = dr["ProductCode"].ToString();
                    vm.ItemName = dr["RawProductName"].ToString();
                    vm.ItemCode = dr["RawProductCode"].ToString();
                    vm.ItemUOM = dr["UOM"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
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
