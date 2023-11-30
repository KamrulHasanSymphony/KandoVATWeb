using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymOrdinary;
using SymViewModel.VMS;
using System.Reflection;
using System.IO;
using Excel;

namespace SymServices.VMS
{
    public class ReceiveDAL
    {

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();

        #region Backup 18092013
        public string[] ReceiveInsert_18092013(ReceiveMasterVM Master, List<ReceiveDetailVM> Details)
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
            DateTime previousReceiveDate = DateTime.MinValue;
            DateTime receiveDate = DateTime.MinValue;
            DateTime BOMDate = DateTime.MinValue; //start
            string BomId = string.Empty;
            bool withoutBOM = false;
            bool issueAutoPost = false;
            string issueAutoPostValue = "N";
            bool NegStockAllow = false;

            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.ReceiveDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.ReceiveDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, "Please Check Invoice Data and Time");

                }




                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();


                transaction = currConn.BeginTransaction(MessageVM.receiveMsgMethodNameInsert);
                string vIssueAutoPost = string.Empty;
                vIssueAutoPost = commonDal.settings("IssueFromBOM", "IssueAutoPost");
                if (string.IsNullOrEmpty(vIssueAutoPost))
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Receive");
                }
                issueAutoPost = Convert.ToBoolean(vIssueAutoPost == "Y" ? true : false);
                if (issueAutoPost)
                    issueAutoPostValue = "Y";




                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.ReceiveDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.ReceiveDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(ReceiveNo) from ReceiveHeaders" +
                          " WHERE ReceiveNo=@MasterReceiveNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;

                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Purchase ID Create
                if (string.IsNullOrEmpty(Master.transactionType)) // start
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                #region Purchase ID Create For Other

                //CommonDAL commonDal = new CommonDAL();

                if (Master.transactionType == "Other" || Master.transactionType == "Tender")
                {
                    newID = commonDal.TransactionCode("Receive", "Other", "ReceiveHeaders", "ReceiveNo",
                                              "ReceiveDateTime", Master.ReceiveDateTime, currConn, transaction);
                }
                else if (Master.transactionType == "WIP")
                {
                    newID = commonDal.TransactionCode("Receive", "WIP", "ReceiveHeaders", "ReceiveNo",
                                              "ReceiveDateTime", Master.ReceiveDateTime, currConn, transaction);
                }
                else if (Master.transactionType == "TollFinishReceive")
                {
                    newID = commonDal.TransactionCode("TollFinishReceive", "TollFinishReceive", "ReceiveHeaders", "ReceiveNo",
                                              "ReceiveDateTime", Master.ReceiveDateTime, currConn, transaction);
                }
                else if (Master.transactionType == "ReceiveReturn")
                {
                    newID = commonDal.TransactionCode("Receive", "ReceiveReturn", "ReceiveHeaders", "ReceiveNo",
                                              "ReceiveDateTime", Master.ReceiveDateTime, currConn, transaction);
                    #region Find Receive Return Date

                    sqlText = "";
                    sqlText = sqlText + "select ReceiveDateTime from ReceiveHeaders" +
                              " WHERE ReceiveNo=@MasterReturnId";
                    SqlCommand cmdFindPDate = new SqlCommand(sqlText, currConn);
                    cmdFindPDate.Transaction = transaction;

                    cmdFindPDate.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);

                    previousReceiveDate = (DateTime)cmdFindPDate.ExecuteScalar();

                    if (previousReceiveDate == null)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgFindExistID);
                    }

                    #endregion  Find Receive Return Date
                }


                #endregion Purchase ID Create For Other


                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header


                sqlText = "";
                sqlText += " insert into ReceiveHeaders"; //Database Table name change
                sqlText += " (";

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

                sqlText += " values";
                sqlText += " (";
                sqlText += "@newID,";
                sqlText += "@MasterReceiveDateTime,";
                sqlText += "@MasterTotalAmount,";
                sqlText += "@MasterTotalVATAmount,";
                sqlText += "@MasterSerialNo,";
                sqlText += "@MasterComments,";
                sqlText += "@MasterCreatedBy,";
                sqlText += "@MasterCreatedOn,";
                sqlText += "@MasterLastModifiedBy,";
                sqlText += "@MasterLastModifiedOn,";
                sqlText += "@MastertransactionType,";
                sqlText += "@MasterReturnId,";
                sqlText += "@MasterPost ";
                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValueAndNullHandle("@newID", newID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalAmount", Master.TotalAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post ?? Convert.DBNull);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgSaveNotSuccessfully);
                }


                #endregion ID generated completed,Insert new Information in Header

                #region if Transection not Other Insert Issue /Receive

                #region Receive For BOM
                if (Master.FromBOM == "Y")
                {
                    string vNegStockAllow = string.Empty;
                    vNegStockAllow = commonDal.settings("Issue", "NegStockAllow");
                    NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);

                    if (string.IsNullOrEmpty(vNegStockAllow))
                    {
                        throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Receive");
                    }


                    if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn" || Master.transactionType == "WIP" || Master.transactionType == "Tender" || Master.transactionType == "TollFinishReceive")
                    {
                        #region Insert to Issue Header



                        sqlText = "";
                        sqlText += " insert into IssueHeaders(";
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
                        sqlText += " values(	";
                        //sqlText += "'" + Master.Id + "',";

                        sqlText += "@newID,";
                        sqlText += "@MasterReceiveDateTime,";
                        sqlText += " 0,0,";
                        sqlText += "@MasterSerialNo,";
                        sqlText += "@MasterComments,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@MasterReceiveNo,";
                        sqlText += "@MastertransactionType,";
                        sqlText += "@MasterReturnId,";
                        //sqlText += "'" + Master.Post";
                        sqlText += "@issueAutoPostValue";

                        //sqlText += "'" + Master.@Post + "'";
                        sqlText += ")	";

                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue.Transaction = transaction;
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@newID", newID);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue ?? Convert.DBNull);

                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();
                        if (transResult <= 0)
                        {

                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            MessageVM.receiveMsgUnableToSaveIssue);
                        }

                        #endregion Insert to Issue Header

                    }



                }

                #endregion Receive For BOM



                #endregion if Transection not Other Insert Issue /Receive

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo='" + newID + "' ";
                    sqlText += " AND ItemNo=@ItemItemNo ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable

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
                    sqlText += " transactionType,";
                    sqlText += " ReceiveReturnId,";
                    sqlText += " BOMId,";

                    sqlText += " UOMPrice,";
                    sqlText += " UOMQty,";
                    sqlText += " UOMn,";
                    sqlText += " UOMc,";
                    sqlText += " VATName,";

                    sqlText += " Post";
                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@newID,";
                    sqlText += "@ItemReceiveLineNo,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemQuantity,";
                    sqlText += "@ItemCostPrice,";
                    sqlText += "@ItemNBRPrice,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemVATRate,";
                    sqlText += "@ItemVATAmount,";
                    sqlText += "@ItemSubTotal,";
                    sqlText += "@ItemCommentsD,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@ItemSD,";
                    sqlText += "@ItemSDAmount,";
                    sqlText += "@MasterReceiveDateTime,";
                    sqlText += "@MastertransactionType,";
                    sqlText += "@MasterReturnId,";
                    sqlText += "@ItemBOMId,";
                    sqlText += "@ItemUOMPrice,";
                    sqlText += "@ItemUOMQty,";
                    sqlText += "@ItemUOMn,";
                    sqlText += "@ItemUOMc,";
                    sqlText += "@MasterPost";
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@newID", newID);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemCostPrice", Item.CostPrice);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemNBRPrice", Item.NBRPrice);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemVATRate", Item.VATRate);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemVATAmount", Item.VATAmount);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemSubTotal", Item.SubTotal);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn      ", Master.CreatedOn);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemSD", Item.SD);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemSDAmount", Item.SDAmount);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime ", Master.ReceiveDateTime);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@MastertransactionType ", Master.transactionType ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemBOMId", Item.BOMId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemUOMPrice", Item.UOMPrice);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemUOMQty", Item.UOMQty);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemUOMn", Item.UOMn);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemUOMc", Item.UOMc);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post ?? Convert.DBNull);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable  //done
                    //done
                    if (Master.transactionType == "ReceiveReturn")

                        receiveDate = previousReceiveDate.Date;
                    else
                        receiveDate = Convert.ToDateTime(Master.ReceiveDateTime);

                    #region Transaction is FromBOM
                    if (Master.FromBOM == "Y")
                    {
                        ProductDAL productDal = new ProductDAL();

                        BomId = string.Empty;
                        BOMDate = DateTime.MinValue;

                        #region Last BOMId

                        sqlText = "  ";
                        sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId from BOMs";
                        sqlText += " where ";
                        sqlText += " FinishItemNo   =@ItemItemNo ";
                        sqlText += " and vatname    =@ItemVatName ";
                        sqlText += " and effectdate<=@receiveDateDate";
                        sqlText += " and post='Y' ";
                        sqlText += " order by effectdate desc ";

                        SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                        cmdBomId.Transaction = transaction;

                        cmdBomId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                        cmdBomId.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                        cmdBomId.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);


                        if (cmdBomId.ExecuteScalar() == null)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            "No Price declaration found for this item");
                            BomId = "0";
                        }
                        else
                        {
                            BomId = (string)cmdBomId.ExecuteScalar();
                        }

                        #endregion Last BOMId

                        #region Last BOMDate

                        sqlText = "  ";
                        sqlText += " select top 1 CONVERT(varchar(20), isnull(EffectDate,'1900/01/01')) from BOMs";
                        sqlText += " where ";
                        sqlText += " FinishItemNo=@ItemItemNo ";
                        sqlText += " and vatname=@ItemVatName ";
                        sqlText += " and effectdate<=@receiveDateDate ";
                        sqlText += " and post='Y' ";
                        sqlText += " order by effectdate desc ";

                        SqlCommand cmdBomEDate = new SqlCommand(sqlText, currConn);
                        cmdBomEDate.Transaction = transaction;

                        cmdBomEDate.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                        cmdBomEDate.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                        cmdBomEDate.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);

                        if (cmdBomEDate.ExecuteScalar() == null)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            "No Price declaration found for this item");
                            BOMDate = DateTime.MinValue;
                        }
                        else
                        {
                            BOMDate = (DateTime)cmdBomEDate.ExecuteScalar();
                        }

                        #endregion Last BOMDate

                        if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn" ||
                            Master.transactionType == "WIP")
                        {
                            #region Find Raw Item From BOM  and update Stock

                            //sss
                            sqlText = "";
                            sqlText +=
                                " SELECT  b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty from BOMRaws b  ";
                            sqlText += " WHERE ";

                            sqlText += " FinishItemNo='" + Item.ItemNo + "' ";
                            sqlText += " and vatname='" + Item.VatName + "' ";
                            sqlText += " and effectdate='" + BOMDate.Date + "'";
                            sqlText += " and post='Y' ";
                            sqlText += "   and (rawitemtype='raw' or rawitemtype='pack') ";



                            DataTable dataTable = new DataTable("RIFB");
                            SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                            cmdRIFB.Transaction = transaction;
                            SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                            reportDataAdapt.Fill(dataTable);

                            if (dataTable == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgNoDataToPost);
                            }
                            else if (dataTable.Rows.Count <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                "There is no price declaration for this item.");
                            }
                            else
                            {
                                decimal vQuantity = 0;
                                decimal vWastage = 0;
                                decimal vStock = 0;
                                string rwUom = "";
                                decimal vConvertionRate = 0;
                                decimal AvgRate = 0;
                                foreach (DataRow BRItem in dataTable.Rows)
                                {
                                    #region Declare

                                    decimal v1Quantity = 0;
                                    string v1RawItemNo = "";
                                    decimal v1CostPrice = 0;
                                    string v1UOM = "";
                                    decimal v1SubTotal = 0;
                                    decimal v1Wastage = 0;
                                    DateTime v1BOMDate = DateTime.Now.Date;
                                    string v1FinishItemNo = "";

                                    decimal v1UOMQty = 0;
                                    decimal v1UOMPrice = 0;
                                    decimal v1UOMc = 0;
                                    string v1UOMn = "";
                                    string v1BOMId = "";
                                    decimal v1UOMWastage = 0;

                                    #endregion Declare

                                    #region Update Item Qty

                                    #region Find Quantity From Products


                                    //decimal AvgRate = productDal.AvgPrice(BRItem["RawItemNo"].ToString(),
                                    //                                      Master.ReceiveDateTime, currConn, transaction);
                                    DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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


                                    //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                    //    vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());

                                    vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                    vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                    rwUom = BRItem["Uom"].ToString();

                                    var rwMajorUom = BRItem["Uomn"].ToString();
                                    if (string.IsNullOrEmpty(rwUom))
                                    {
                                        throw new ArgumentNullException("ReceiveInsert",
                                                                        "Could not find UOM of raw item");
                                    }

                                    /*Processing UOM*/

                                    UOMDAL uomdal = new UOMDAL();
                                    vConvertionRate = uomdal.GetConvertionRate(rwMajorUom, rwUom, "Y", currConn, transaction); //uomc


                                    #region valueAssign

                                    v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                    v1Wastage = (vWastage) * Item.UOMQty;
                                    v1BOMId = BomId;
                                    v1RawItemNo = BRItem["RawItemNo"].ToString();
                                    v1UOM = BRItem["UOM"].ToString();
                                    v1CostPrice = AvgRate * vConvertionRate;
                                    v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                    v1UOMPrice = AvgRate;
                                    v1UOMn = BRItem["UOMn"].ToString();
                                    v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                    v1FinishItemNo = Item.ItemNo;
                                    v1UOMc = vConvertionRate;
                                    v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                    v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;

                                    #endregion valueAssign

                                    #region Stock

                                    if (NegStockAllow == false)
                                    {
                                        //var stock = productDal.StockInHand(BRItem["RawItemNo"].ToString(),
                                        //                                       Master.ReceiveDateTime+
                                        //                                       DateTime.Now.ToString(" HH:mm:ss"),
                                        //                                   currConn, transaction).ToString();
                                        var stock = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(),
                                                                                   Master.ReceiveDateTime +
                                                                                   DateTime.Now.ToString(" HH:mm:ss"),
                                                                 currConn, transaction, false).Rows[0]["Quantity"].ToString();

                                        vStock = Convert.ToDecimal(stock);


                                        if ((vStock - v1UOMQty) < 0)
                                        {
                                            string FinName = string.Empty;
                                            string FinCode = string.Empty;
                                            string RawName = string.Empty;
                                            string RawCode = string.Empty;
                                            DataTable finDt = new DataTable();
                                            finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                            foreach (DataRow FinItem in finDt.Rows)
                                            {
                                                FinName = FinItem["ProductName"].ToString();
                                                FinCode = FinItem["ProductCode"].ToString();
                                            }
                                            DataTable rawDt = new DataTable();
                                            rawDt =
                                                productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                            foreach (DataRow RawItem in rawDt.Rows)
                                            {
                                                RawName = RawItem["ProductName"].ToString();
                                                RawCode = RawItem["ProductCode"].ToString();
                                            }

                                            throw new ArgumentNullException("ReceiveInsert",
                                                                            "Stock not Available for Finish Item( Name: " +
                                                                            FinName + " & Code: " + FinCode +
                                                                            " ) \n and consumtion Material ( Name: " +
                                                                            RawName + " & Code: " + RawCode + " )");
                                        }
                                    }

                                    #endregion Stock


                                    #endregion Find Quantity From Products


                                    #region Find Quantity From Transaction


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
                                    sqlText += " SDAmount,"; //19
                                    sqlText += " Wastage,";
                                    sqlText += " BOMDate,";
                                    sqlText += " FinishItemNo,";
                                    sqlText += " transactionType,";
                                    sqlText += " IssueReturnId,";
                                    sqlText += " UOMQty,";
                                    sqlText += " UOMPrice,";
                                    sqlText += " UOMc,";
                                    sqlText += " UOMn,";
                                    sqlText += " UOMWastage,";
                                    sqlText += " BOMId,";

                                    sqlText += " Post";
                                    sqlText += " )";
                                    sqlText += " values( ";
                                    sqlText += "@newID,";
                                    sqlText += "@ItemReceiveLineNo,";
                                    sqlText += "@v1RawItemNo, ";
                                    sqlText += "@v1Quantity ,";
                                    sqlText += "@AvgRate,";
                                    sqlText += "@v1CostPrice,"; //6
                                    sqlText += "@v1UOM,";
                                    sqlText += " 0,0, "; //VATRate,VATAmount
                                    sqlText += "@v1SubTotal,";
                                    sqlText += "@ItemCommentsD,";
                                    sqlText += "@MasterCreatedBy,";
                                    sqlText += "@MasterCreatedOn,";
                                    sqlText += "@MasterLastModifiedBy,";
                                    sqlText += "@MasterLastModifiedOn,";
                                    sqlText += "@newID,";
                                    sqlText += "@MasterReceiveDateTime,";
                                    sqlText += " 0,	0,"; //SDSDAmount //19
                                    sqlText += "@v1Wastage,	";
                                    sqlText += "@v1BOMDate,	";
                                    sqlText += "@v1FinishItemNo,";
                                    sqlText += "@MastertransactionType,";
                                    sqlText += "@MasterReturnId,";
                                    sqlText += "@v1UOMQty,";
                                    sqlText += "@v1UOMPrice,";
                                    sqlText += "@v1UOMc,";
                                    sqlText += "@v1UOMn,";
                                    sqlText += "@v1UOMWastage,";
                                    sqlText += "@v1BOMId,";

                                    sqlText += "@issueAutoPostValue";
                                    sqlText += ")";
                                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                    cmdInsertIssue.Transaction = transaction;
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@newID", newID);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime ", Master.ReceiveDateTime);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@1UOMQty", v1UOMQty);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@1UOMPrice", v1UOMPrice);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@1UOMc", v1UOMc);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue ?? Convert.DBNull);

                                    transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

                                    #region Update Issue

                                    sqlText = "";
                                    sqlText += " update IssueHeaders set ";
                                    sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                    sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                    sqlText += " where (IssueHeaders.IssueNo=@newID )";

                                    SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                    cmdUpdateIssue.Transaction = transaction;
                                    cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@newID", newID);

                                    transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

                                    #endregion Update Issue

                                    #endregion Qty  check and Update

                                    #endregion Qty  check and Update
                                }
                            }

                            #endregion Find Raw Item From BOM and update Stock

                        }
                        else if (Master.transactionType == "Tender")
                        {
                            #region Declare

                            decimal v1Quantity = 0;
                            string v1RawItemNo = "";
                            decimal v1CostPrice = 0;
                            string v1UOM = "";
                            decimal v1SubTotal = 0;
                            decimal v1Wastage = 0;
                            DateTime v1BOMDate = DateTime.Now.Date;
                            string v1FinishItemNo = "";

                            decimal v1UOMQty = 0;
                            decimal v1UOMPrice = 0;
                            decimal v1UOMc = 0;
                            string v1UOMn = "";
                            string v1BOMId = "";
                            decimal v1UOMWastage = 0;

                            #endregion Declare

                            #region Find Raw Item From BOM  and update Stock

                            sqlText = "";


                            sqlText +=
                                " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty from BOMRaws b  ";
                            sqlText += " WHERE ";

                            sqlText += " FinishItemNo='" + Item.ItemNo + "' ";
                            sqlText += " and vatname='" + Item.VatName + "' ";
                            sqlText += " and effectdate='" + BOMDate.Date + "'";
                            sqlText += "  and post='Y' and (b.rawitemtype='raw' or b.rawitemtype='pack') ";


                            DataTable dataTable = new DataTable("RIFB");
                            SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                            cmdRIFB.Transaction = transaction;
                            SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                            reportDataAdapt.Fill(dataTable);

                            if (dataTable == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgNoDataToPost);
                            }
                            else if (dataTable.Rows.Count <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                "There is no price declaration for this item.");
                            }
                            else
                            {
                                decimal vQuantity = 0;
                                decimal vWastage = 0;
                                string rwUom = "";
                                decimal vConvertionRate = 0;
                                decimal AvgRate = 0;
                                foreach (DataRow BRItem in dataTable.Rows)
                                {
                                    #region Update Item Qty

                                    #region Find Quantity From Products

                                    //decimal AvgRate = productDal.AvgPrice(BRItem["RawItemNo"].ToString(),
                                    //                                      Master.ReceiveDateTime, currConn, transaction);

                                    DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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

                                    //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                    //vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());

                                    vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                    vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                    rwUom = BRItem["Uom"].ToString();
                                    var rwUomMajor = BRItem["Uomn"].ToString();
                                    if (string.IsNullOrEmpty(rwUom))
                                    {
                                        throw new ArgumentNullException("ReceiveInsert",
                                                                        "Could not find UOM of raw item");
                                    }

                                    /*Processing UOM*/

                                    UOMDAL uomdal = new UOMDAL();
                                    vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);


                                    #endregion Find Quantity From Products

                                    #region valueAssign

                                    v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                    v1Wastage = (vWastage) * Item.UOMQty;
                                    v1BOMId = Item.BOMId;
                                    v1RawItemNo = BRItem["RawItemNo"].ToString();
                                    v1UOM = BRItem["UOM"].ToString();
                                    v1CostPrice = AvgRate * vConvertionRate;
                                    v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                    v1UOMPrice = AvgRate;
                                    v1UOMn = BRItem["UOMn"].ToString();
                                    v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                    v1FinishItemNo = Item.ItemNo;
                                    v1UOMc = vConvertionRate;
                                    v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                    v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;

                                    #endregion valueAssign

                                    #region Stock

                                    if (NegStockAllow == false)
                                    {
                                        decimal vStock = 0;

                                        //var stock = productDal.StockInHand(v1RawItemNo,
                                        //                                      Master.ReceiveDateTime+
                                        //                                      DateTime.Now.ToString(" HH:mm:ss"),
                                        //                                  currConn, transaction).ToString();
                                        var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                                              Master.ReceiveDateTime +
                                                                              DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction, false).Rows[0]["Quantity"].ToString();




                                        vStock = Convert.ToDecimal(stock);


                                        if ((vStock - v1UOMQty) < 0)
                                        {
                                            string FinName = string.Empty;
                                            string FinCode = string.Empty;
                                            string RawName = string.Empty;
                                            string RawCode = string.Empty;
                                            DataTable finDt = new DataTable();
                                            finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                            foreach (DataRow FinItem in finDt.Rows)
                                            {
                                                FinName = FinItem["ProductName"].ToString();
                                                FinCode = FinItem["ProductCode"].ToString();
                                            }
                                            DataTable rawDt = new DataTable();
                                            rawDt =
                                                productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                            foreach (DataRow RawItem in rawDt.Rows)
                                            {
                                                RawName = RawItem["ProductName"].ToString();
                                                RawCode = RawItem["ProductCode"].ToString();
                                            }

                                            throw new ArgumentNullException("ReceiveInsert",
                                                                            "Stock not Available for Finish Item( Name: " +
                                                                            FinName + " & Code: " + FinCode +
                                                                            " ) \n and consumtion Material ( Name: " +
                                                                            RawName + " & Code: " + RawCode + " )");
                                        }
                                    }

                                    #endregion Stock



                                    #region Find Quantity From Transaction

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
                                    sqlText += " UOMWastage,";
                                    sqlText += " BOMId,";
                                    sqlText += " Post";
                                    sqlText += " )";
                                    sqlText += " values( ";
                                    sqlText += "@newID,";
                                    sqlText += "@ItemReceiveLineNo,";
                                    sqlText += "@v1RawItemNo, ";
                                    sqlText += "@v1Quantity ,";
                                    sqlText += "@AvgRate,";
                                    sqlText += "@v1CostPrice,";

                                    sqlText += "@v1UOM,";
                                    sqlText += " 0,0, ";
                                    sqlText += "@v1SubTotal,";
                                    sqlText += "@ItemCommentsD,";
                                    sqlText += "@MasterCreatedBy,";
                                    sqlText += "@MasterCreatedOn,";
                                    sqlText += "@MasterLastModifiedBy,";
                                    sqlText += "@MasterLastModifiedOn,";
                                    sqlText += "@newID,";
                                    sqlText += "@MasterReceiveDateTime,";
                                    sqlText += " 0,	0,";
                                    sqlText += "@v1Wastage,	";
                                    sqlText += "@v1BOMDate,	";
                                    sqlText += "@v1FinishItemNo,";
                                    sqlText += "@MastertransactionType,";
                                    sqlText += "@MasterReturnId,";
                                    sqlText += "@v1UOMQty,";
                                    sqlText += "@v1UOMPrice,";
                                    sqlText += "@v1UOMc,";
                                    sqlText += "@v1UOMn,";
                                    sqlText += "@v1UOMWastage,";
                                    sqlText += "@v1BOMId,";
                                    sqlText += "@issueAutoPostValue";
                                    sqlText += ")";
                                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                    cmdInsertIssue.Transaction = transaction;
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@newID", newID);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue ?? Convert.DBNull);

                                    transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

                                    #region Update Issue

                                    sqlText = "";
                                    sqlText += " update IssueHeaders set ";
                                    sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                    sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                    sqlText += " where (IssueHeaders.IssueNo= @newID)";

                                    SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                    cmdUpdateIssue.Transaction = transaction;
                                    cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@newID", newID);

                                    transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

                                    #endregion Update Issue

                                    #endregion Qty  check and Update

                                    #endregion Qty  check and Update
                                }
                            }

                            #endregion Find Raw Item From BOM and update Stock
                        }
                    }

                    #endregion Transaction is Trading

                    #region TollFinishReceive
                    if (Master.transactionType == "TollFinishReceive")
                    {

                        ProductDAL productDal = new ProductDAL();
                        string FinishItemIdFromOH = productDal.GetFinishItemIdFromOH(Item.ItemNo, "VAT 1 (Toll Issue)", Master.ReceiveDateTime,
                                                                           currConn, transaction).ToString();

                        BomId = string.Empty;  //BOMId
                        BOMDate = DateTime.MinValue;
                        #region Last BOMId
                        sqlText = "  ";
                        sqlText += " select top 1 CONVERT(varchar(10),isnull(BOMId,0)) from BOMs";
                        sqlText += " where ";
                        sqlText += " FinishItemNo=@FinishItemIdFromOH ";
                        sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                        sqlText += " and effectdate<=@receiveDateDate";
                        sqlText += " and post='Y' ";
                        sqlText += " order by effectdate desc ";

                        SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                        cmdBomId.Transaction = transaction;

                        cmdBomId.Parameters.AddWithValueAndNullHandle("@FinishItemIdFromOH", FinishItemIdFromOH);
                        cmdBomId.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);

                        if (cmdBomId.ExecuteScalar() == null)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            "No Price declaration found for this item");
                            BomId = "0";
                        }
                        else
                        {
                            BomId = (string)cmdBomId.ExecuteScalar();
                        }

                        #endregion Last BOMId

                        #region Last BOMDate
                        sqlText = "  ";
                        sqlText += " select top 1 CONVERT(varchar(20),isnull(EffectDate,'1900/01/01')) from BOMs";
                        sqlText += " where ";
                        sqlText += " FinishItemNo=@FinishItemIdFromOH  ";
                        sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                        sqlText += " and effectdate<=@receiveDateDate ";
                        sqlText += " and post='Y' ";
                        sqlText += " order by effectdate desc ";

                        SqlCommand cmdBomEDate = new SqlCommand(sqlText, currConn);
                        cmdBomEDate.Transaction = transaction;

                        cmdBomEDate.Parameters.AddWithValueAndNullHandle("@FinishItemIdFromOH", FinishItemIdFromOH);
                        cmdBomEDate.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);

                        if (cmdBomEDate.ExecuteScalar() == null)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            "No Price declaration found for this item");
                            BOMDate = DateTime.MinValue;
                        }
                        else
                        {
                            BOMDate = (DateTime)cmdBomEDate.ExecuteScalar();
                        }

                        #endregion Last BOMDate


                        #region Find Raw Item From BOM  and update Stock

                        sqlText = "";


                        sqlText +=
                            " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty  from BOMRaws b  ";
                        sqlText += " WHERE ";
                        sqlText += " FinishItemNo=@FinishItemIdFromOH" +
                                   "and post='Y' " +
                                   "and EffectDate=@BOMDateDate";
                        sqlText += " and Vatname='VAT 1 (Toll Issue)'";
                        sqlText += "   and (rawitemtype='raw' or rawitemtype='pack') ";

                        DataTable dataTable = new DataTable("RIFB");
                        SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                        cmdRIFB.Transaction = transaction;

                        cmdRIFB.Parameters.AddWithValueAndNullHandle("@FinishItemIdFromOH", FinishItemIdFromOH);
                        cmdRIFB.Parameters.AddWithValueAndNullHandle("@BOMDateDate", BOMDate.Date);

                        SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                        reportDataAdapt.Fill(dataTable);

                        if (dataTable == null)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                            MessageVM.receiveMsgNoDataToPost);
                        }
                        else if (dataTable.Rows.Count <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                            "There is no price declaration for this item.");
                        }
                        else
                        {
                            #region Declare
                            decimal v1Quantity = 0;
                            string v1RawItemNo = "";
                            decimal v1CostPrice = 0;
                            string v1UOM = "";
                            decimal v1SubTotal = 0;
                            decimal v1Wastage = 0;
                            DateTime v1BOMDate = DateTime.Now.Date;
                            string v1FinishItemNo = "";

                            decimal v1UOMQty = 0;
                            decimal v1UOMPrice = 0;
                            decimal v1UOMc = 0;
                            string v1UOMn = "";
                            string v1BOMId = "";
                            decimal v1UOMWastage = 0;
                            #endregion Declare

                            decimal vQuantity = 0;
                            decimal vWastage = 0;
                            decimal AvgRate = 0;

                            string rwUom = "";
                            decimal vConvertionRate = 0;

                            foreach (DataRow BRItem in dataTable.Rows)
                            {
                                #region Update Item Qty

                                #region Find Quantity From Products

                                //decimal AvgRate = productDal.AvgPrice(BRItem["RawItemNo"].ToString(),
                                //                                      Master.ReceiveDateTime, currConn, transaction);

                                DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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

                                //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                //vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());

                                vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                rwUom = BRItem["Uom"].ToString();
                                var rwUomMajor = BRItem["Uomn"].ToString();
                                if (string.IsNullOrEmpty(rwUom))
                                {
                                    throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                }

                                /*Processing UOM*/

                                UOMDAL uomdal = new UOMDAL();
                                vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);

                                #endregion Find Quantity From Products

                                #region valueAssign
                                v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                v1Wastage = (vWastage) * Item.UOMQty;
                                v1BOMId = BomId;
                                v1RawItemNo = BRItem["RawItemNo"].ToString();
                                v1UOM = BRItem["UOM"].ToString();
                                v1CostPrice = AvgRate * vConvertionRate;
                                v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                v1UOMPrice = AvgRate;
                                v1UOMn = BRItem["UOMn"].ToString();
                                v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                v1FinishItemNo = FinishItemIdFromOH;
                                v1UOMc = vConvertionRate;
                                v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                #endregion valueAssign

                                #region Stock
                                if (NegStockAllow == false)
                                {
                                    decimal vStock = 0;
                                    //var stock = productDal.StockInHand(v1RawItemNo,
                                    //    Master.ReceiveDateTime+ DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction).ToString();

                                    var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                                                  Master.ReceiveDateTime +
                                                                                  DateTime.Now.ToString(" HH:mm:ss"),
                                                                currConn, transaction, false).Rows[0]["Quantity"].ToString();
                                    vStock = Convert.ToDecimal(stock);


                                    if ((vStock - v1UOMQty) < 0)
                                    {
                                        string FinName = string.Empty;
                                        string FinCode = string.Empty;
                                        string RawName = string.Empty;
                                        string RawCode = string.Empty;
                                        DataTable finDt = new DataTable();
                                        finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                        foreach (DataRow FinItem in finDt.Rows)
                                        {
                                            FinName = FinItem["ProductName"].ToString();
                                            FinCode = FinItem["ProductCode"].ToString();
                                        }
                                        DataTable rawDt = new DataTable();
                                        rawDt = productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                        foreach (DataRow RawItem in rawDt.Rows)
                                        {
                                            RawName = RawItem["ProductName"].ToString();
                                            RawCode = RawItem["ProductCode"].ToString();
                                        }

                                        throw new ArgumentNullException("ReceiveInsert", "Stock not Available for Finish Item( Name: " + FinName + " & Code: " + FinCode + " ) \n and consumtion Material ( Name: " + RawName + " & Code: " + RawCode + " )");
                                    }
                                }
                                #endregion Stock

                                #region Find Quantity From Transaction

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
                                sqlText += " UOMWastage,";
                                sqlText += " BOMId,";
                                sqlText += " Post";
                                sqlText += " )";
                                sqlText += " values( ";
                                sqlText += "@newID,";
                                sqlText += "@ItemReceiveLineNo,";
                                sqlText += "@v1RawItemNo, ";
                                sqlText += "@v1Quantity ,";
                                sqlText += "@AvgRate,";
                                sqlText += "@v1CostPrice,";
                                sqlText += "@v1UOM,";
                                sqlText += " 0,0, ";
                                sqlText += "@v1SubTotal,";
                                sqlText += "@ItemCommentsD,";
                                sqlText += "@MasterCreatedBy,";
                                sqlText += "@MasterCreatedOn,";
                                sqlText += "@MasterLastModifiedBy,";
                                sqlText += "@MasterLastModifiedOn,";
                                sqlText += "@newID,";
                                sqlText += "@MasterReceiveDateTime,";
                                sqlText += " 0,	0,";
                                sqlText += "@v1Wastage,	";
                                sqlText += "@v1BOMDate,	";
                                sqlText += "@v1FinishItemNo,";
                                sqlText += "@MastertransactionType,";
                                sqlText += "@MasterReturnId,";
                                sqlText += "@v1UOMQty,";
                                sqlText += "@v1UOMPrice,";
                                sqlText += "@v1UOMc,";
                                sqlText += "@v1UOMn,";
                                sqlText += "@v1UOMWastage,";
                                sqlText += "@v1BOMId,";
                                sqlText += "@issueAutoPostValue";
                                sqlText += ")";
                                SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                cmdInsertIssue.Transaction = transaction;
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@newID", newID);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo ?? Convert.DBNull);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo ?? Convert.DBNull);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM ?? Convert.DBNull);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@newID", newID ?? Convert.DBNull);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo ?? Convert.DBNull);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn ?? Convert.DBNull);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId ?? Convert.DBNull);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue ?? Convert.DBNull);

                                transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    MessageVM.receiveMsgUnableToSaveIssue);
                                }

                                #region Update Issue

                                sqlText = "";
                                sqlText += " update IssueHeaders set ";
                                sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                sqlText += " where (IssueHeaders.IssueNo=@newID )";

                                SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                cmdUpdateIssue.Transaction = transaction;
                                cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@newID", newID);

                                transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    MessageVM.receiveMsgUnableToSaveIssue);
                                }

                                #endregion Update Issue

                                #endregion Qty  check and Update

                                #endregion Qty  check and Update
                            }
                        }

                        #endregion Find Raw Item From BOM and update Stock

                    }
                    #endregion TollFinishReceive


                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.ReceiveHeaders WHERE ReceiveNo='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgUnableCreatID);
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
                retResults[1] = MessageVM.receiveMsgSaveSuccessfully;
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
        public string[] ReceiveUpdate_18092013(ReceiveMasterVM Master, List<ReceiveDetailVM> Details)
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
            bool NegStockAllow = false;
            DateTime previousReceiveDate = DateTime.MinValue;
            DateTime receiveDate = DateTime.MinValue;
            DateTime BOMDate = DateTime.MinValue;  //start
            string BomId = string.Empty;

            string newID = "";
            string PostStatus = "";
            bool issueAutoPost = false;
            string issueAutoPostValue = "N";
            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.ReceiveDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.ReceiveDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, "Please Check Invoice Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                transaction = currConn.BeginTransaction(MessageVM.receiveMsgMethodNameInsert);
                #endregion open connection and transaction
                #region Master

                string vissueAutoPost, vNegStockAllow = string.Empty;

                vissueAutoPost = commonDal.settings("IssueFromBOM", "IssueAutoPost");
                vNegStockAllow = commonDal.settings("Issue", "NegStockAllow");
                if (
                    string.IsNullOrEmpty(vissueAutoPost)
                   || string.IsNullOrEmpty(vNegStockAllow)
                    )
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Receive");
                }
                issueAutoPost = Convert.ToBoolean(vissueAutoPost == "Y" ? true : false);
                NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);

                if (issueAutoPost)
                    issueAutoPostValue = "Y";

                #region Fiscal Year Check

                string transactionDate = Master.ReceiveDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.ReceiveDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(ReceiveNo) from ReceiveHeaders WHERE ReceiveNo=@MasterReceiveNo";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;

                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUnableFindExistID);
                }

                #endregion Find ID for Update


                if (Master.transactionType == "ReceiveReturn")
                {
                    #region Find Receive Return Date

                    sqlText = "";
                    sqlText = sqlText + "select ReceiveDateTime from ReceiveHeaders" +
                              " WHERE ReceiveNo=@MasterReturnId ";
                    SqlCommand cmdFindPDate = new SqlCommand(sqlText, currConn);
                    cmdFindPDate.Transaction = transaction;

                    cmdFindPDate.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);

                    previousReceiveDate = (DateTime)cmdFindPDate.ExecuteScalar();

                    if (previousReceiveDate == null)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                        MessageVM.receiveMsgFindExistID);
                    }

                    #endregion  Find Receive Return Date
                }

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update ReceiveHeaders set  ";
                sqlText += " ReceiveDateTime    = @MasterReceiveDateTime ,";
                sqlText += " TotalAmount        = @MasterTotalAmount ,";
                sqlText += " SerialNo           = @MasterSerialNo ,";
                sqlText += " Comments           = @MasterComments ,";
                sqlText += " LastModifiedBy     = @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn     = @MasterLastModifiedOn ,";
                sqlText += " transactionType    = @MastertransactionType ,";
                sqlText += " ReceiveReturnId    = @MasterReturnId ,";
                sqlText += " Post               = @MasterPost ";
                sqlText += " where  ReceiveNo   = @MasterReceiveNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUpdateNotSuccessfully);
                }
                #endregion update Header

                #region Transaction Not Other

                #region Transaction is FromBOM
                if (Master.FromBOM == "Y")
                {
                    if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn" || Master.transactionType == "WIP" || Master.transactionType == "TollFinishReceive" || Master.transactionType == "Tender")
                    {
                        #region update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " IssueDateTime  =@MasterReceiveDateTime,";
                        sqlText += " Comments       =@MasterComments ,";
                        sqlText += " SerialNo       =@MasterSerialNo ,";
                        sqlText += " LastModifiedBy =@MasterLastModifiedBy ,";
                        sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                        sqlText += " transactionType=@MastertransactionType ,";
                        sqlText += " IssueReturnId  =@MasterReturnId ,";
                        sqlText += " Post           =@issueAutoPostValue ";
                        sqlText += " where  IssueNo =@MasterReceiveNo ";


                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime ", Master.ReceiveDateTime);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments ?? Convert.DBNull);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue ?? Convert.DBNull);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);

                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                            MessageVM.receiveMsgUpdateNotSuccessfully);
                        }

                        #endregion update Issue

                    }

                }

                #endregion Transaction is TollReceive

                #endregion Transaction Not Other


                #endregion ID check completed,update Information in Header

                #endregion Master
                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    ProductDAL productDal = new ProductDAL();

                    #region Find Transaction Mode Insert or Update

                    IDExist = 0;
                    sqlText = "";
                    sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo=@MasterReceiveNo ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();
                    if (Master.transactionType == "ReceiveReturn")

                        receiveDate = previousReceiveDate.Date;
                    else
                        receiveDate = Convert.ToDateTime(Master.ReceiveDateTime);

                    if (IDExist <= 0)
                    {
                        #region Insert only DetailTable

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
                        sqlText += " transactionType,";
                        sqlText += " ReceiveReturnId,";
                        sqlText += " BOMId,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMQty,";
                        sqlText += " UOMn,";
                        sqlText += " UOMc,";
                        sqlText += " VATName,";

                        sqlText += " Post";
                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@MasterReceiveNo,";
                        sqlText += "@ItemReceiveLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@ItemCostPrice,";
                        sqlText += "@ItemNBRPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += "@ItemVATRate,";
                        sqlText += "@ItemVATAmount,";
                        sqlText += "@ItemSubTotal,";
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@ItemSD,";
                        sqlText += "@ItemSDAmount,";
                        sqlText += "@MasterReceiveDateTime,";
                        sqlText += "@MastertransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@ItemBOMId,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemVatName,";
                        sqlText += "@MasterPost";
                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCostPrice", Item.CostPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemNBRPrice", Item.NBRPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATAmount", Item.VATAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSD", Item.SD);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSDAmount", Item.SDAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBOMId", Item.BOMId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgSaveNotSuccessfully);
                        }
                        #endregion Insert only DetailTable
                        #region if Transection not Other Insert Issue /Receive


                        #region Purchase For TollReceive
                        #region From BOM

                        if (Master.FromBOM == "Y")
                        {
                            BomId = string.Empty;
                            BOMDate = DateTime.MinValue;

                            #region Last BOMId

                            sqlText = "  ";
                            sqlText += " select top 1 CONVERT(varchar(20),isnull(BOMId,0)) from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo   =@ItemItemNo";
                            sqlText += " and vatname    =@ItemVatName";
                            sqlText += " and effectdate<=@receiveDateDate";
                            sqlText += " and post='Y' ";
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                            cmdBomId.Transaction = transaction;

                            cmdBomId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomId.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdBomId.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);


                            if (cmdBomId.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BomId = "0";
                            }
                            else
                            {
                                BomId = (string)cmdBomId.ExecuteScalar();
                            }

                            #endregion Last BOMId

                            #region Last BOMDate

                            sqlText = "  ";
                            sqlText += " select top 1 CONVERT(varchar(20),isnull(EffectDate,'1900/01/01')) from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname=@ItemVatName ";
                            sqlText += " and effectdate<=@receiveDateDate";
                            sqlText += " and post='Y' ";
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomEDate = new SqlCommand(sqlText, currConn);
                            cmdBomEDate.Transaction = transaction;

                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);

                            if (cmdBomEDate.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BOMDate = DateTime.MinValue;
                            }
                            else
                            {
                                BOMDate = (DateTime)cmdBomEDate.ExecuteScalar();
                            }

                            #endregion Last BOMDate


                            if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn" || Master.transactionType == "WIP")
                            {



                                #region Find Raw Item From BOM  and update Stock


                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.Uom,UOMUQty,UOMWQty from BOMRaws b  ";
                                sqlText += " where ";
                                sqlText += " FinishItemNo='" + Item.ItemNo + "' ";
                                sqlText += " and vatname='" + Item.VatName + "' ";
                                sqlText += " and effectdate='" + BOMDate.Date + "'";
                                sqlText += " and post='Y' ";

                                sqlText += "   and (rawitemtype='raw' or rawitemtype='pack') ";

                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;
                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {
                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;
                                    string rwUom = "";
                                    decimal vConvertionRate = 0;
                                    decimal AvgRate = 0;
                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {
                                        #region Declare
                                        decimal v1Quantity = 0;
                                        string v1RawItemNo = "";
                                        decimal v1CostPrice = 0;
                                        string v1UOM = "";
                                        decimal v1SubTotal = 0;
                                        decimal v1Wastage = 0;
                                        DateTime v1BOMDate = DateTime.Now.Date;
                                        string v1FinishItemNo = "";

                                        decimal v1UOMQty = 0;
                                        decimal v1UOMPrice = 0;
                                        decimal v1UOMc = 0;
                                        string v1UOMn = "";
                                        string v1BOMId = "";
                                        decimal v1UOMWastage = 0;
                                        #endregion DECLARE

                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        //decimal AvgRate = productDal.AvgPrice(BRItem["RawItemNo"].ToString(),
                                        //Master.ReceiveDateTime, currConn, transaction);
                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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


                                        //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                        //vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());

                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }

                                        /*Processing UOM*/

                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);

                                        #endregion Find Quantity From Products

                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                        v1Wastage = (vWastage) * Item.UOMQty;
                                        v1BOMId = BomId;
                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1CostPrice = AvgRate * vConvertionRate;
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                        v1FinishItemNo = Item.ItemNo;
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                        #endregion valueAssign

                                        #region Stock
                                        if (NegStockAllow == false)
                                        {
                                            decimal vStock = 0;
                                            //var stock = productDal.StockInHand(v1RawItemNo,
                                            //    Master.ReceiveDateTime + DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction).ToString();

                                            var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                                                  Master.ReceiveDateTime +
                                                                                  DateTime.Now.ToString(" HH:mm:ss"),
                                                                currConn, transaction, false).Rows[0]["Quantity"].ToString();

                                            vStock = Convert.ToDecimal(stock);


                                            if ((vStock - v1UOMQty) < 0)
                                            {
                                                string FinName = string.Empty;
                                                string FinCode = string.Empty;
                                                string RawName = string.Empty;
                                                string RawCode = string.Empty;
                                                DataTable finDt = new DataTable();
                                                finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                                foreach (DataRow FinItem in finDt.Rows)
                                                {
                                                    FinName = FinItem["ProductName"].ToString();
                                                    FinCode = FinItem["ProductCode"].ToString();
                                                }
                                                DataTable rawDt = new DataTable();
                                                rawDt = productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                                foreach (DataRow RawItem in rawDt.Rows)
                                                {
                                                    RawName = RawItem["ProductName"].ToString();
                                                    RawCode = RawItem["ProductCode"].ToString();
                                                }

                                                throw new ArgumentNullException("ReceiveInsert", "Stock not Available for Finish Item( Name: " + FinName + " & Code: " + FinCode + " ) \n and consumtion Material ( Name: " + RawName + " & Code: " + RawCode + " )");
                                            }
                                        }
                                        #endregion Stock
                                        #region Find Quantity From Transaction

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
                                        sqlText += " UOMWastage,";
                                        sqlText += " BOMId,";
                                        sqlText += " Post";
                                        sqlText += " )";
                                        sqlText += " values( ";
                                        sqlText += "@MasterReceiveNo,";
                                        sqlText += "@ItemReceiveLineNo,";
                                        sqlText += "@v1RawItemNo, ";
                                        sqlText += "@v1Quantity ,";
                                        sqlText += "@AvgRate,";
                                        sqlText += "@v1CostPrice,";
                                        sqlText += "@v1UOM,";
                                        sqlText += " 0,0, ";
                                        sqlText += "@v1SubTotal,";
                                        sqlText += "@ItemCommentsD,";
                                        sqlText += "@MasterCreatedBy,";
                                        sqlText += "@MasterCreatedOn,";
                                        sqlText += "@MasterLastModifiedBy,";
                                        sqlText += "@MasterLastModifiedOn,";
                                        sqlText += "@newID,";
                                        sqlText += "@MasterReceiveDateTime,";
                                        sqlText += " 0,	0,";
                                        sqlText += "@v1Wastage,	";
                                        sqlText += "@v1BOMDate,	";
                                        sqlText += "@v1FinishItemNo,";
                                        sqlText += "@MastertransactionType,";
                                        sqlText += "@MasterReturnId,";
                                        sqlText += "@v1UOMQty,";
                                        sqlText += "@v1UOMPrice,";
                                        sqlText += "@v1UOMc,";
                                        sqlText += "@v1UOMn,";
                                        sqlText += "@v1UOMWastage,";
                                        sqlText += "@v1BOMId,";
                                        sqlText += "@issueAutoPostValue";
                                        sqlText += ")";
                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@newID", newID ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue ?? Convert.DBNull);

                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #region Update Issue

                                        sqlText = "";
                                        sqlText += " update IssueHeaders set ";
                                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                        sqlText += " where (IssueHeaders.IssueNo= @MasterReceiveNo )";

                                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                        cmdUpdateIssue.Transaction = transaction;
                                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #endregion Update Issue

                                        #endregion Qty  check and Update

                                        #endregion Qty  check and Update
                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock

                            }
                            else if (Master.transactionType == "Tender")
                            {

                                #region Find Raw Item From BOM  and update Stock

                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,UOMUQty,UOMWQty from BOMRaws b  ";
                                sqlText += " WHERE ";
                                sqlText += " FinishItemNo='" + Item.ItemNo + "' ";
                                sqlText += " and vatname='" + Item.VatName + "' ";
                                sqlText += " and effectdate='" + BOMDate.Date + "'";

                                sqlText += "   and post='Y'  and (b.rawitemtype='raw' or b.rawitemtype='pack') ";

                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;
                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {

                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;
                                    decimal AvgRate = 0;
                                    string rwUom = "";
                                    decimal vConvertionRate = 0;

                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {
                                        #region Declare
                                        decimal v1Quantity = 0;
                                        string v1RawItemNo = "";
                                        decimal v1CostPrice = 0;
                                        string v1UOM = "";
                                        decimal v1SubTotal = 0;
                                        decimal v1Wastage = 0;
                                        DateTime v1BOMDate = DateTime.Now.Date;
                                        string v1FinishItemNo = "";

                                        decimal v1UOMQty = 0;
                                        decimal v1UOMPrice = 0;
                                        decimal v1UOMc = 0;
                                        string v1UOMn = "";
                                        string v1BOMId = "";
                                        decimal v1UOMWastage = 0;
                                        #endregion DECLARE

                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        //decimal AvgRate = productDal.AvgPrice(BRItem["RawItemNo"].ToString(),
                                        //                                      Master.ReceiveDateTime, currConn, transaction);

                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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


                                        //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                        //vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());

                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }


                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);


                                        #endregion Find Quantity From Products
                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                        v1Wastage = (vWastage) * Item.UOMQty;
                                        v1BOMId = BomId;
                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1CostPrice = AvgRate * vConvertionRate;
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                        v1FinishItemNo = Item.ItemNo;
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                        #endregion valueAssign

                                        #region Stock
                                        if (NegStockAllow == false)
                                        {
                                            decimal vStock = 0;
                                            //var stock = productDal.StockInHand(v1RawItemNo,
                                            //    Master.ReceiveDateTime + DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction).ToString();
                                            var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                                                  Master.ReceiveDateTime +
                                                                                  DateTime.Now.ToString(" HH:mm:ss"),
                                                                currConn, transaction, false).Rows[0]["Quantity"].ToString();
                                            vStock = Convert.ToDecimal(stock);


                                            if ((vStock - v1UOMQty) < 0)
                                            {
                                                string FinName = string.Empty;
                                                string FinCode = string.Empty;
                                                string RawName = string.Empty;
                                                string RawCode = string.Empty;
                                                DataTable finDt = new DataTable();
                                                finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                                foreach (DataRow FinItem in finDt.Rows)
                                                {
                                                    FinName = FinItem["ProductName"].ToString();
                                                    FinCode = FinItem["ProductCode"].ToString();
                                                }
                                                DataTable rawDt = new DataTable();
                                                rawDt = productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                                foreach (DataRow RawItem in rawDt.Rows)
                                                {
                                                    RawName = RawItem["ProductName"].ToString();
                                                    RawCode = RawItem["ProductCode"].ToString();
                                                }

                                                throw new ArgumentNullException("ReceiveInsert", "Stock not Available for Finish Item( Name: " + FinName + " & Code: " + FinCode + " ) \n and consumtion Material ( Name: " + RawName + " & Code: " + RawCode + " )");
                                            }
                                        }
                                        #endregion Stock
                                        #region Find Quantity From Transaction

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
                                        sqlText += " UOMWastage,";
                                        sqlText += " BOMId,";
                                        sqlText += " Post";
                                        sqlText += " )";
                                        sqlText += " values( ";
                                        sqlText += "@MasterReceiveNo,";
                                        sqlText += "@ItemReceiveLineNo,";
                                        sqlText += "@v1RawItemNo, ";
                                        sqlText += "@v1Quantity ,";
                                        sqlText += "@AvgRate,";
                                        sqlText += "@v1CostPrice,";
                                        sqlText += "@v1UOM,";
                                        sqlText += " 0,0, ";
                                        sqlText += "@v1SubTotal,";
                                        sqlText += "@ItemCommentsD,";
                                        sqlText += "@MasterCreatedBy,";
                                        sqlText += "@MasterCreatedOn,";
                                        sqlText += "@MasterLastModifiedBy,";
                                        sqlText += "@MasterLastModifiedOn,";
                                        sqlText += "@newID,";
                                        sqlText += "@MasterReceiveDateTime,";
                                        sqlText += " 0,	0,";
                                        sqlText += "@v1Wastage,	";
                                        sqlText += "@v1BOMDate,	";
                                        sqlText += "@v1FinishItemNo,";
                                        sqlText += "@MastertransactionType,";
                                        sqlText += "@MasterReturnId,";
                                        sqlText += "@v1UOMQty,";
                                        sqlText += "@v1UOMPrice,";
                                        sqlText += "@v1UOMc,";
                                        sqlText += "@v1UOMn,";
                                        sqlText += "@v1UOMWastage,";
                                        sqlText += "@v1BOMId,";
                                        sqlText += "@issueAutoPostValue";
                                        sqlText += ")";
                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@newID", newID ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue ?? Convert.DBNull);

                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #region Update Issue

                                        sqlText = "";
                                        sqlText += " update IssueHeaders set ";
                                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                        sqlText += " where (IssueHeaders.IssueNo=@MasterReceiveNo )";

                                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                        cmdUpdateIssue.Transaction = transaction;
                                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #endregion Update Issue

                                        #endregion Qty  check and Update

                                        #endregion Qty  check and Update
                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock

                            }

                        }

                        #endregion From BOM
                        #region TollFinishReceive
                        if (Master.transactionType == "TollFinishReceive")
                        {
                            BomId = string.Empty;
                            BOMDate = DateTime.MinValue;

                            string FinishItemIdFromOH = productDal.GetFinishItemIdFromOH(Item.ItemNo, "VAT 1 (Toll Issue)", Master.ReceiveDateTime,
                                                                               currConn, transaction);


                            #region Last BOMId
                            sqlText = "  ";
                            sqlText += " select top 1 CONVERT(varchar(20),isnull(BOMId,0)) from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@FinishItemIdFromOH";
                            sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                            sqlText += " and effectdate<=@receiveDate.Date";
                            sqlText += " and post='Y' ";
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                            cmdBomId.Transaction = transaction;

                            cmdBomId.Parameters.AddWithValueAndNullHandle("@FinishItemIdFromOH", FinishItemIdFromOH);
                            cmdBomId.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);

                            if (cmdBomId.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BomId = "0";
                            }
                            else
                            {
                                BomId = (string)cmdBomId.ExecuteScalar();
                            }

                            #endregion Last BOMId

                            #region Last BOMDate
                            sqlText = "  ";
                            sqlText += " select top 1 CONVERT(varchar(20),isnull(EffectDate,'1900/01/01')) from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@FinishItemIdFromOH ";
                            sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                            sqlText += " and effectdate<=@receiveDateDate";
                            sqlText += " and post='Y' ";
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomEDate = new SqlCommand(sqlText, currConn);
                            cmdBomEDate.Transaction = transaction;

                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@FinishItemIdFromOH", FinishItemIdFromOH);
                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);

                            if (cmdBomEDate.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BOMDate = DateTime.MinValue;
                            }
                            else
                            {
                                BOMDate = (DateTime)cmdBomEDate.ExecuteScalar();
                            }

                            #endregion Last BOMDate


                            #region Find Raw Item From BOM  and update Stock

                            sqlText = "";


                            sqlText +=
                                " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty from BOMRaws b  ";
                            sqlText += " WHERE ";

                            sqlText += " FinishItemNo=@FinishItemIdFromOH ";
                            sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                            sqlText += " and effectdate=@BOMDateDate";
                            sqlText += " and post='Y' ";

                            sqlText += " and (b.rawitemtype='raw' or b.rawitemtype='pack') ";

                            DataTable dataTable = new DataTable("RIFB");
                            SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                            cmdRIFB.Transaction = transaction;

                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@FinishItemIdFromOH", FinishItemIdFromOH);
                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@BOMDateDate", BOMDate.Date);

                            SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                            reportDataAdapt.Fill(dataTable);

                            if (dataTable == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgNoDataToPost);
                            }
                            else if (dataTable.Rows.Count <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                "There is no price declaration for this item.");
                            }
                            else
                            {
                                decimal vQuantity = 0;
                                decimal vWastage = 0;
                                string rwUom = "";
                                decimal vConvertionRate = 0;
                                decimal AvgRate = 0;

                                foreach (DataRow BRItem in dataTable.Rows)
                                {

                                    #region Declare
                                    decimal v1Quantity = 0;
                                    string v1RawItemNo = "";
                                    decimal v1CostPrice = 0;
                                    string v1UOM = "";
                                    decimal v1SubTotal = 0;
                                    decimal v1Wastage = 0;
                                    DateTime v1BOMDate = DateTime.Now.Date;
                                    string v1FinishItemNo = "";

                                    decimal v1UOMQty = 0;
                                    decimal v1UOMPrice = 0;
                                    decimal v1UOMc = 0;
                                    string v1UOMn = "";
                                    string v1BOMId = "";
                                    decimal v1UOMWastage = 0;
                                    #endregion DECLARE
                                    #region Update Item Qty

                                    #region Find Quantity From Products

                                    //decimal AvgRate = productDal.AvgPrice(BRItem["RawItemNo"].ToString(),
                                    //                                      Master.ReceiveDateTime, currConn, transaction);

                                    DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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

                                    //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                    //vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());

                                    vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                    vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                    rwUom = BRItem["Uom"].ToString();
                                    var rwUomMajor = BRItem["Uomn"].ToString();
                                    if (string.IsNullOrEmpty(rwUom))
                                    {
                                        throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                    }

                                    /*Processing UOM*/

                                    UOMDAL uomdal = new UOMDAL();
                                    vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);


                                    #endregion Find Quantity From Products

                                    #region valueAssign
                                    v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                    v1Wastage = (vWastage) * Item.UOMQty;
                                    v1BOMId = BomId;
                                    v1RawItemNo = BRItem["RawItemNo"].ToString();
                                    v1UOM = BRItem["UOM"].ToString();
                                    v1CostPrice = AvgRate * vConvertionRate;
                                    v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                    v1UOMPrice = AvgRate;
                                    v1UOMn = BRItem["UOMn"].ToString();
                                    v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                    v1FinishItemNo = FinishItemIdFromOH;
                                    v1UOMc = vConvertionRate;
                                    v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                    v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                    #endregion valueAssign

                                    #region Stock
                                    if (NegStockAllow == false)
                                    {
                                        decimal vStock = 0;
                                        //var stock = productDal.StockInHand(v1RawItemNo,
                                        //    Master.ReceiveDateTime + DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction).ToString();
                                        var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                                                  Master.ReceiveDateTime +
                                                                                  DateTime.Now.ToString(" HH:mm:ss"),
                                                                currConn, transaction, false).Rows[0]["Quantity"].ToString();

                                        vStock = Convert.ToDecimal(stock);


                                        if ((vStock - v1UOMQty) < 0)
                                        {
                                            string FinName = string.Empty;
                                            string FinCode = string.Empty;
                                            string RawName = string.Empty;
                                            string RawCode = string.Empty;
                                            DataTable finDt = new DataTable();
                                            finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                            foreach (DataRow FinItem in finDt.Rows)
                                            {
                                                FinName = FinItem["ProductName"].ToString();
                                                FinCode = FinItem["ProductCode"].ToString();
                                            }
                                            DataTable rawDt = new DataTable();
                                            rawDt = productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                            foreach (DataRow RawItem in rawDt.Rows)
                                            {
                                                RawName = RawItem["ProductName"].ToString();
                                                RawCode = RawItem["ProductCode"].ToString();
                                            }

                                            throw new ArgumentNullException("ReceiveInsert", "Stock not Available for Finish Item( Name: " + FinName + " & Code: " + FinCode + " ) \n and consumtion Material ( Name: " + RawName + " & Code: " + RawCode + " )");
                                        }
                                    }
                                    #endregion Stock
                                    #region Find Quantity From Transaction

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
                                    sqlText += " UOMWastage,";
                                    sqlText += " BOMId,";
                                    sqlText += " Post";
                                    sqlText += " )";
                                    sqlText += " values( ";
                                    sqlText += "@MasterReceiveNo,";
                                    sqlText += "@ItemReceiveLineNo,";
                                    sqlText += "@v1RawItemNo, ";
                                    sqlText += "@v1Quantity ,";
                                    sqlText += "@AvgRate,";
                                    sqlText += "@v1CostPrice,";
                                    sqlText += "@v1UOM,";
                                    sqlText += " 0,0, ";
                                    sqlText += "@v1SubTotal,";
                                    sqlText += "@ItemCommentsD,";
                                    sqlText += "@MasterCreatedBy,";
                                    sqlText += "@MasterCreatedOn,";
                                    sqlText += "@MasterLastModifiedBy,";
                                    sqlText += "@MasterLastModifiedOn,";
                                    sqlText += "@newID,";
                                    sqlText += "@MasterReceiveDateTime,";
                                    sqlText += " 0,	0,";
                                    sqlText += "@v1Wastage,	";
                                    sqlText += "@v1BOMDate,	";
                                    sqlText += "@v1FinishItemNo,";
                                    sqlText += "@MastertransactionType,";
                                    sqlText += "@MasterReturnId,";
                                    sqlText += "@v1UOMQty,";
                                    sqlText += "@v1UOMPrice,";
                                    sqlText += "@v1UOMc,";
                                    sqlText += "@v1UOMn,";
                                    sqlText += "@v1UOMWastage,";
                                    sqlText += "@v1BOMId,";
                                    sqlText += "@issueAutoPostValue";
                                    sqlText += ")";
                                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                    cmdInsertIssue.Transaction = transaction;
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@newID", newID ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId ?? Convert.DBNull);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue ?? Convert.DBNull);

                                    transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

                                    #region Update Issue

                                    sqlText = "";
                                    sqlText += " update IssueHeaders set ";
                                    sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                    sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                    sqlText += " where (IssueHeaders.IssueNo=@MasterReceiveNo)";

                                    SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                    cmdUpdateIssue.Transaction = transaction;
                                    cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                    transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

                                    #endregion Update Issue

                                    #endregion Qty  check and Update

                                    #endregion Qty  check and Update
                                }
                            }

                            #endregion Find Raw Item From BOM and update Stock

                        }
                        #endregion TollFinishReceive

                        #endregion Purchase ID Create For IssueReturn

                        #endregion if Transection not Other Insert Issue /Receive
                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";


                        sqlText += " update ReceiveDetails set ";
                        sqlText += " ReceiveLineNo      =@ItemReceiveLineNo,";
                        sqlText += " Quantity           =@ItemQuantity,";
                        sqlText += " CostPrice          =@ItemCostPrice,";
                        sqlText += " NBRPrice           =@ItemNBRPrice,";
                        sqlText += " UOM                =@ItemUOM,";
                        sqlText += " VATRate            =@ItemVATRate,";
                        sqlText += " VATAmount          =@ItemVATAmount,";
                        sqlText += " SubTotal           =@ItemSubTotal,";
                        sqlText += " Comments           =@ItemCommentsD,";
                        sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                        sqlText += " SD                 =@ItemSD,";
                        sqlText += " SDAmount           =@ItemSDAmount,";
                        sqlText += " ReceiveDateTime    =@MasterReceiveDateTime,";
                        sqlText += " transactionType    =@MastertransactionType,";
                        sqlText += " ReceiveReturnId    =@MasterReturnId,";
                        sqlText += " BOMId              =@ItemBOMId,";
                        sqlText += " UOMPrice           =@ItemUOMPrice,";
                        sqlText += " UOMQty             =@ItemUOMQty,";
                        sqlText += " UOMn               =@ItemUOMn,";
                        sqlText += " UOMc               =@ItemUOMc,";
                        sqlText += " VATName            =@ItemVatName,";
                        sqlText += " Post               =@MasterPost";
                        sqlText += " where ReceiveNo    =@MasterReceiveNo ";
                        sqlText += " and 	ItemNo      =@ItemItemNo ";



                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCostPrice", Item.CostPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemNBRPrice", Item.NBRPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATAmount", Item.VATAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSD", Item.SD);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSDAmount", Item.SDAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBOMId", Item.BOMId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable
                        #region Update Issue and Receive if Transaction is not Other


                        #region Transaction is FromBOM
                        if (Master.FromBOM == "Y")
                        {
                            BomId = string.Empty;
                            BOMDate = DateTime.MinValue;

                            #region Last BOMId

                            sqlText = "  ";
                            sqlText += " select top 1 CONVERT(varchar(20),isnull(BOMId,0)) from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo   =@ItemItemNo";
                            sqlText += " and vatname    =@ItemVatName ";
                            sqlText += " and effectdate<=@receiveDateDate";
                            sqlText += " and post='Y' ";
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                            cmdBomId.Transaction = transaction;

                            cmdBomId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomId.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdBomId.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);

                            if (cmdBomId.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BomId = "0";
                            }
                            else
                            {
                                BomId = (string)cmdBomId.ExecuteScalar();
                            }

                            #endregion Last BOMId

                            #region Last BOMDate

                            sqlText = "  ";
                            sqlText += " select top 1 CONVERT(varchar(20),isnull(EffectDate,'1900/01/01')) from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname=@ItemVatName ";
                            sqlText += " and effectdate<=@receiveDateDate";
                            sqlText += " and post='Y' ";
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomEDate = new SqlCommand(sqlText, currConn);
                            cmdBomEDate.Transaction = transaction;

                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);

                            if (cmdBomEDate.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BOMDate = DateTime.MinValue;
                            }
                            else
                            {
                                BOMDate = (DateTime)cmdBomEDate.ExecuteScalar();
                            }

                            #endregion Last BOMDate


                            if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn" || Master.transactionType == "WIP")
                            {
                                #region Update to Issue



                                #region Find Raw Item From BOM  and update Stock
                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty from BOMRaws b  ";
                                sqlText += " WHERE ";
                                sqlText += " FinishItemNo='" + Item.ItemNo + "' ";
                                sqlText += " and vatname='" + Item.VatName + "' ";
                                sqlText += " and effectdate='" + BOMDate.Date + "'";
                                sqlText += " and post='Y' ";

                                sqlText += "   and (rawitemtype='raw' or rawitemtype='pack') ";

                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;
                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {
                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;
                                    decimal vConvertionRate = 0;
                                    decimal AvgRate = 0;


                                    #region Declare
                                    decimal v1Quantity = 0;
                                    string v1RawItemNo = "";
                                    decimal v1CostPrice = 0;
                                    string v1UOM = "";
                                    decimal v1SubTotal = 0;
                                    decimal v1Wastage = 0;
                                    DateTime v1BOMDate = DateTime.Now.Date;
                                    string v1FinishItemNo = "";

                                    decimal v1UOMQty = 0;
                                    decimal v1UOMPrice = 0;
                                    decimal v1UOMc = 0;
                                    string v1UOMn = "";
                                    string v1BOMId = "";
                                    decimal v1UOMWastage = 0;
                                    string rwUom = "";

                                    #endregion DECLARE

                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {
                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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


                                        #endregion Find Quantity From Products

                                        //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                        //vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());

                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());


                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }

                                        /*Processing UOM*/

                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);

                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                        v1Wastage = (vWastage) * Item.UOMQty;
                                        v1BOMId = BomId;
                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1CostPrice = AvgRate * vConvertionRate;
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                        v1FinishItemNo = Item.ItemNo;
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                        #endregion valueAssign

                                        #region Qty  check and Update

                                        if (NegStockAllow == false)
                                        {
                                            #region Find Quantity From Products

                                            //decimal BRItemoldStock =
                                            //productDal.StockInHand(BRItem["RawItemNo"].ToString(),
                                            //                       Master.ReceiveDateTime, currConn,
                                            //                       transaction);
                                            decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(),
                                                                                  Master.ReceiveDateTime,
                                                                currConn, transaction, false).Rows[0]["Quantity"].ToString());



                                            #endregion Find Quantity From Products

                                            #region Find Quantity From Transaction

                                            sqlText = "";
                                            sqlText +=
                                                "select isnull(UOMQty ,isnull(Quantity,0)) from IssueDetails ";
                                            sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                                       "and IssueNo=@MasterReceiveNo";
                                            SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                            cmdBRItemTranQty.Transaction = transaction;

                                            cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                            decimal BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                            #endregion Find Quantity From Transaction



                                            if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                            {
                                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                                MessageVM.
                                                                                    receiveMsgStockNotAvailablePost);
                                            }
                                        }




                                        #endregion Qty  check and Update




                                        sqlText = "";
                                        sqlText += " update IssueDetails set";
                                        sqlText += " IssueLineNo        =@ItemReceiveLineNo,";
                                        sqlText += " Comments           =@ItemCommentsD,";
                                        sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                                        sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                                        sqlText += " IssueDateTime      =@MasterReceiveDateTime,";
                                        sqlText += " Post               =@issueAutoPostValue,";
                                        sqlText += " uom                =@v1UOM,";
                                        sqlText += " Quantity           =@v1Quantity,";
                                        sqlText += " Wastage            =@v1Wastage,";
                                        sqlText += " BOMDate            =@v1BOMDate,";
                                        sqlText += " CostPrice          =@v1CostPrice,";
                                        sqlText += " NBRPrice           =@AvgRate,";
                                        sqlText += " transactionType    =@MastertransactionType,";
                                        sqlText += " IssueReturnId      =@MasterReturnId,";
                                        sqlText += " UOMQty             =@v1UOMQty,";
                                        sqlText += " UOMPrice           =@v1UOMPrice,";
                                        sqlText += " UOMc               =@v1UOMc,";
                                        sqlText += " UOMn               =@v1UOMn,";
                                        sqlText += " UOMWastage         =@v1UOMWastage,";
                                        sqlText += " BOMId              =@v1BOMId,";
                                        sqlText += " SubTotal           =@v1SubTotal";
                                        sqlText += " WHERE FinishItemNo =@v1FinishItemNo " +
                                                    " AND ItemNo        =@v1RawItemNo ";
                                        sqlText += " and  IssueNo       =@MasterReceiveNo";


                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);

                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                            MessageVM.receiveMsgUnableToUpdateIssue);
                                        }

                                        #endregion Qty  check and Update

                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock

                                /////////

                                #endregion Update to Issue

                                #region Update Issue Header

                                sqlText = "";
                                sqlText += " update IssueHeaders set ";
                                sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                sqlText += " where (IssueHeaders.IssueNo= @MasterReceiveNo)";

                                SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                cmdUpdateIssue.Transaction = transaction;
                                cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);


                                transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                    MessageVM.receiveMsgUnableToUpdateIssue);
                                }

                                #endregion Update Issue Header

                            }
                            else if (Master.transactionType == "Tender")
                            {
                                #region Update to Issue


                                #region Find Raw Item From BOM  and update Stock

                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,UOMUQty,UOMWQty from BOMRaws b  ";
                                sqlText += " WHERE ";
                                sqlText += " FinishItemNo='" + Item.ItemNo + "' ";
                                sqlText += " and vatname='" + Item.VatName + "' ";
                                sqlText += " and effectdate='" + BOMDate.Date + "'";

                                sqlText += "  and post='Y'   and (b.rawitemtype='raw' or b.rawitemtype='pack') ";
                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;
                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {

                                    #region Declare
                                    decimal v1Quantity = 0;
                                    string v1RawItemNo = "";
                                    decimal v1CostPrice = 0;
                                    string v1UOM = "";
                                    decimal v1SubTotal = 0;
                                    decimal v1Wastage = 0;
                                    DateTime v1BOMDate = DateTime.Now.Date;
                                    string v1FinishItemNo = "";

                                    decimal v1UOMQty = 0;
                                    decimal v1UOMPrice = 0;
                                    decimal v1UOMc = 0;
                                    string v1UOMn = "";
                                    string v1BOMId = "";
                                    decimal v1UOMWastage = 0;
                                    #endregion DECLARE

                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;

                                    string rwUom = "";
                                    decimal vConvertionRate = 0;
                                    decimal AvgRate = 0;


                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {
                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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


                                        #endregion Find Quantity From Products

                                        //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                        //vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());
                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }

                                        /*Processing UOM*/

                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);


                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                        v1Wastage = (vWastage) * Item.UOMQty;
                                        v1BOMId = Item.BOMId;
                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1CostPrice = AvgRate * vConvertionRate;
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                        v1FinishItemNo = Item.ItemNo;
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                        #endregion valueAssign
                                        #region Stock Check
                                        if (NegStockAllow == false)
                                        {
                                            #region Find Quantity From Products

                                            //decimal BRItemoldStock =
                                            //    productDal.StockInHand(v1RawItemNo,
                                            //                           Master.ReceiveDateTime, currConn,
                                            //                           transaction);
                                            decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(v1RawItemNo,
                                                                                 Master.ReceiveDateTime,
                                                               currConn, transaction, false).Rows[0]["Quantity"].ToString());




                                            #endregion Find Quantity From Products

                                            #region Find Quantity From Transaction

                                            sqlText = "";
                                            sqlText +=
                                                "select isnull(UOMQty,isnull(Quantity ,0)) from IssueDetails ";
                                            sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                                       "and IssueNo=@MasterReceiveNo";
                                            SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                            cmdBRItemTranQty.Transaction = transaction;

                                            cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                            decimal BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                            #endregion Find Quantity From Transaction



                                            if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                            {
                                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                                MessageVM.
                                                                                    receiveMsgStockNotAvailablePost);
                                            }
                                        }
                                        #endregion Stock Check



                                        sqlText = "";
                                        sqlText += " update IssueDetails set";
                                        sqlText += " IssueLineNo        =@ItemReceiveLineNo,";
                                        sqlText += " Comments           =@ItemCommentsD,";
                                        sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                                        sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                                        sqlText += " IssueDateTime      =@MasterReceiveDateTime,";
                                        sqlText += " Post               =@issueAutoPostValue,";
                                        sqlText += " uom                =@v1UOM,";
                                        sqlText += " Quantity           =@v1Quantity,";
                                        sqlText += " Wastage            =@v1Wastage,";
                                        sqlText += " BOMDate            =@v1BOMDate,";
                                        sqlText += " CostPrice          =@v1CostPrice,";
                                        sqlText += " NBRPrice           =@AvgRate,";
                                        sqlText += " transactionType    =@MastertransactionType,";
                                        sqlText += " IssueReturnId      =@MasterReturnId,";
                                        sqlText += " UOMQty             =@v1UOMQty,";
                                        sqlText += " UOMPrice           =@v1UOMPrice,";
                                        sqlText += " UOMc               =@v1UOMc,";
                                        sqlText += " UOMn               =@v1UOMn,";
                                        sqlText += " UOMWastage         =@v1UOMWastage,";
                                        sqlText += " BOMId              =@v1BOMId,";
                                        sqlText += " SubTotal           =@v1SubTotal";
                                        sqlText += " WHERE FinishItemNo =@v1FinishItemNo" +
                                                        " AND ItemNo    =@v1RawItemNo ";
                                        sqlText += " and  IssueNo       =@MasterReceiveNo";


                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo ?? Convert.DBNull);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);

                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                            MessageVM.receiveMsgUnableToUpdateIssue);
                                        }

                                        #endregion Qty  check and Update

                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock

                                /////////

                                #endregion Update to Issue

                                #region Update Issue Header

                                sqlText = "";
                                sqlText += " update IssueHeaders set ";
                                sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                sqlText += " where (IssueHeaders.IssueNo=@MasterReceiveNo )";

                                SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                cmdUpdateIssue.Transaction = transaction;
                                cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                    MessageVM.receiveMsgUnableToUpdateIssue);
                                }

                                #endregion Update Issue Header

                            }

                        }

                        #endregion Transaction is Trading

                        #region Transaction is TollFinishReceive

                        if (Master.transactionType == "TollFinishReceive")
                        {
                            string FinishItemIdFromOH = productDal.GetFinishItemIdFromOH(Item.ItemNo, "VAT 1 (Toll Issue)", Master.ReceiveDateTime,
                                                                           currConn, transaction);

                            BomId = string.Empty;
                            BOMDate = DateTime.MinValue;

                            #region Last BOMId
                            sqlText = "  ";
                            sqlText += " select top 1 CONVERT(varchar(20),isnull(BOMId,0)) from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@FinishItemIdFromOH ";
                            sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                            sqlText += " and effectdate<=@receiveDateDate";
                            sqlText += " and post='Y' ";
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                            cmdBomId.Transaction = transaction;

                            cmdBomId.Parameters.AddWithValueAndNullHandle("@FinishItemIdFromOH", FinishItemIdFromOH);
                            cmdBomId.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);

                            if (cmdBomId.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BomId = "0";
                            }
                            else
                            {
                                BomId = (string)cmdBomId.ExecuteScalar();
                            }

                            #endregion Last BOMId

                            #region Last BOMDate
                            sqlText = "  ";
                            sqlText += " select top 1 CONVERT(varchar(20),isnull(EffectDate,'1900/01/01')) from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@FinishItemIdFromOH ";
                            sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                            sqlText += " and effectdate<=@receiveDateDate ";
                            sqlText += " and post='Y' ";
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomEDate = new SqlCommand(sqlText, currConn);
                            cmdBomEDate.Transaction = transaction;

                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@FinishItemIdFromOH", FinishItemIdFromOH);
                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);

                            if (cmdBomEDate.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BOMDate = DateTime.MinValue;
                            }
                            else
                            {
                                BOMDate = (DateTime)cmdBomEDate.ExecuteScalar();
                            }

                            #endregion Last BOMDate

                            #region Update to Issue



                            #region Find Raw Item From BOM  and update Stock



                            sqlText = "";
                            sqlText +=
                                " SELECT   b.RawItemNo,b.UseQuantity,b.UOMUQty,b.UOMWQty,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM from BOMRaws b  ";
                            sqlText += " WHERE ";

                            sqlText += " FinishItemNo='" + Item.ItemNo + "' ";
                            sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                            sqlText += " and effectdate='" + BOMDate.Date + "'";
                            sqlText += " and post='Y' ";

                            sqlText += "   and (rawitemtype='raw' or rawitemtype='pack') ";


                            DataTable dataTable = new DataTable("RIFB");
                            SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                            cmdRIFB.Transaction = transaction;
                            SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                            reportDataAdapt.Fill(dataTable);

                            if (dataTable == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgNoDataToPost);
                            }
                            else if (dataTable.Rows.Count <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgNoDataToPost);
                            }
                            else
                            {
                                decimal vQuantity = 0;
                                decimal vWastage = 0;

                                string rwUom = "";
                                decimal vConvertionRate = 0;
                                decimal AvgRate = 0;


                                foreach (DataRow BRItem in dataTable.Rows)
                                {
                                    #region Declare
                                    decimal v1Quantity = 0;
                                    string v1RawItemNo = "";
                                    decimal v1CostPrice = 0;
                                    string v1UOM = "";
                                    decimal v1SubTotal = 0;
                                    decimal v1Wastage = 0;
                                    DateTime v1BOMDate = DateTime.Now.Date;
                                    string v1FinishItemNo = "";

                                    decimal v1UOMQty = 0;
                                    decimal v1UOMPrice = 0;
                                    decimal v1UOMc = 0;
                                    string v1UOMn = "";
                                    string v1BOMId = "";
                                    decimal v1UOMWastage = 0;
                                    #endregion DECLARE

                                    #region Update Item Qty

                                    #region Find Quantity From Products

                                    DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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


                                    #endregion Find Quantity From Products

                                    vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                    vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());
                                    rwUom = BRItem["Uom"].ToString();
                                    var rwUomMajor = BRItem["Uomn"].ToString();
                                    if (string.IsNullOrEmpty(rwUom))
                                    {
                                        throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                    }

                                    /*Processing UOM*/

                                    UOMDAL uomdal = new UOMDAL();
                                    vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);

                                    #region valueAssign
                                    v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                    v1Wastage = (vWastage) * Item.UOMQty;
                                    v1BOMId = Item.BOMId;
                                    v1RawItemNo = BRItem["RawItemNo"].ToString();
                                    v1UOM = BRItem["UOM"].ToString();
                                    v1CostPrice = AvgRate * vConvertionRate;
                                    v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                    v1UOMPrice = AvgRate;
                                    v1UOMn = BRItem["UOMn"].ToString();
                                    v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                    v1FinishItemNo = Item.ItemNo;
                                    v1UOMc = vConvertionRate;
                                    v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                    v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                    #endregion valueAssign
                                    #region Stock Check
                                    if (NegStockAllow == false)
                                    {
                                        #region Find Quantity From Products

                                        //decimal BRItemoldStock =
                                        //    productDal.StockInHand(v1RawItemNo,
                                        //                           Master.ReceiveDateTime, currConn,
                                        //                           transaction);
                                        decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(v1RawItemNo,
                                                                             Master.ReceiveDateTime,
                                                           currConn, transaction, false).Rows[0]["Quantity"].ToString());



                                        #endregion Find Quantity From Products

                                        #region Find Quantity From Transaction

                                        sqlText = "";
                                        sqlText +=
                                            "select isnull(UOMQty,isnull(Quantity ,0)) from IssueDetails ";
                                        sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                                   "and IssueNo= @MasterReceiveNo";
                                        SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                        cmdBRItemTranQty.Transaction = transaction;

                                        cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                        decimal BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                        #endregion Find Quantity From Transaction



                                        if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                            MessageVM.
                                                                                receiveMsgStockNotAvailablePost);
                                        }
                                    }
                                    #endregion Stock Check



                                    sqlText = "";
                                    sqlText += " update IssueDetails set";
                                    sqlText += " IssueLineNo        =@ItemReceiveLineNo,";
                                    sqlText += " Comments           =@ItemCommentsD,";
                                    sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                                    sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                                    sqlText += " IssueDateTime      =@MasterReceiveDateTime,";
                                    sqlText += " Post               =@issueAutoPostValue,";
                                    sqlText += " uom                =@v1UOM,";
                                    sqlText += " Quantity           =@v1Quantity,";
                                    sqlText += " Wastage            =@v1Wastage,";
                                    sqlText += " BOMDate            =@v1BOMDate,";
                                    sqlText += " CostPrice          =@v1CostPrice,";
                                    sqlText += " NBRPrice           =@AvgRate,";
                                    sqlText += " transactionType    =@MasterTransactionType,";
                                    sqlText += " IssueReturnId      =@MasterReturnId,";
                                    sqlText += " UOMQty             =@v1UOMQty,";
                                    sqlText += " UOMPrice           =@v1UOMPrice,";
                                    sqlText += " UOMc               =@v1UOMc,";
                                    sqlText += " UOMn               =@v1UOMn,";
                                    sqlText += " UOMWastage         =@v1UOMWastage,";
                                    sqlText += " BOMId              =@v1BOMId,";
                                    sqlText += " SubTotal           =@v1SubTotal";
                                    sqlText += " WHERE FinishItemNo =@v1FinishItemNo AND ItemNo=@v1RawItemNo ";
                                    sqlText += " and  IssueNo       =@MasterReceiveNo";

                                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                    cmdInsertIssue.Transaction = transaction;

                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);


                                    transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                        MessageVM.receiveMsgUnableToUpdateIssue);
                                    }

                                    #endregion Qty  check and Update

                                }
                            }

                            #endregion Find Raw Item From BOM and update Stock


                            #endregion Update to Issue

                            #region Update Issue Header

                            sqlText = "";
                            sqlText += " update IssueHeaders set ";
                            sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                            sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                            sqlText += " where (IssueHeaders.IssueNo= @MasterReceiveNo )";

                            SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                            cmdUpdateIssue.Transaction = transaction;

                            cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                            transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                MessageVM.receiveMsgUnableToUpdateIssue);
                            }

                            #endregion Update Issue Header

                        }



                        #endregion Transaction is Trading



                        #endregion Update Issue and Receive if Transaction is not Other
                    }

                    #endregion Find Transaction Mode Insert or Update
                }


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from ReceiveHeaders WHERE ReceiveNo=@MasterReceiveNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;

                cmdIPS.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUnableCreatID);
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
                retResults[1] = MessageVM.receiveMsgUpdateSuccessfully;
                retResults[2] = Master.ReceiveNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {

                throw sqlex;
                transaction.Rollback();
            }
            catch (Exception ex)
            {

                throw ex;
                transaction.Rollback();
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
        public string[] ReceivePost_18092013(ReceiveMasterVM Master, List<ReceiveDetailVM> Details)
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
            bool issueAutoPost = false;
            string issueAutoPostValue = "N";
            DateTime receiveDate = DateTime.MinValue;
            DateTime BOMDate = DateTime.MinValue; //start
            string BomId = string.Empty;

            #endregion Initializ

            #region Try
            try
            {
                string vNegStockAllow, vIssueAutoPost = string.Empty;
                CommonDAL commonDal = new CommonDAL();
                vNegStockAllow = commonDal.settings("Issue", "NegStockAllow");
                vIssueAutoPost = commonDal.settings("IssueFromBOM", "IssueAutoPost");
                if (string.IsNullOrEmpty(vNegStockAllow)
                    || string.IsNullOrEmpty(vIssueAutoPost)
                    )
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Sale");
                }
                bool NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);
                issueAutoPost = Convert.ToBoolean(vIssueAutoPost == "Y" ? true : false);
                if (issueAutoPost)
                    issueAutoPostValue = "Y";
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.ReceiveDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.ReceiveDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgCheckDatePost);

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }



                transaction = currConn.BeginTransaction(MessageVM.receiveMsgMethodNamePost);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.ReceiveDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.ReceiveDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(ReceiveNo) from ReceiveHeaders WHERE ReceiveNo=@MasterReceiveNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;

                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);


                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update ReceiveHeaders set  ";
                sqlText += " LastModifiedBy= @MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn= @MasterLastModifiedOn ,";
                sqlText += " Post= @MasterPost ";
                sqlText += " where  ReceiveNo=@MasterReceiveNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);


                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgPostNotSuccessfully);
                }
                #endregion update Header
                #region Transaction is FromBOM
                if (Master.FromBOM == "Y")
                {
                    if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn" || Master.transactionType == "WIP")
                    {
                        #region update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " LastModifiedBy= @MasterLastModifiedBy ,";
                        sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                        sqlText += " Post= '" + issueAutoPostValue + "' ";
                        sqlText += " where  IssueNo=@MasterReceiveNo";


                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;

                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                            MessageVM.receiveMsgUnableToIssuePost);
                        }

                        #endregion update Issue
                    }

                }
                #endregion Transaction is FromBOM

                #region Transaction is FromBOM

                if (Master.transactionType == "TollFinishReceive")
                {
                    #region update Issue

                    sqlText = "";
                    sqlText += " update IssueHeaders set ";
                    sqlText += " LastModifiedBy=@MasterLastModifiedBy ,";
                    sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                    sqlText += " Post= '" + issueAutoPostValue + "' ";
                    sqlText += " where  IssueNo=@MasterReceiveNo";


                    SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                    cmdUpdateIssue.Transaction = transaction;

                    cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                    transResult = (int)cmdUpdateIssue.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                        MessageVM.receiveMsgUnableToIssuePost);
                    }

                    #endregion update Issue
                }


                #endregion Transaction is FromBOM

                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail
                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                        MessageVM.receiveMsgNoDataToPost);
                }
                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo=@MasterReceiveNo";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {

                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                            MessageVM.receiveMsgNoDataToPost);
                    }
                    else
                    {
                        #region Update only DetailTable
                        sqlText = "";
                        sqlText += " update ReceiveDetails set ";
                        sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                        sqlText += " Post=@MasterPost";
                        sqlText += " where ReceiveNo=@MasterReceiveNo ";
                        sqlText += " and 	ItemNo='@ItemItemNo ";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable
                        #region Update Issue and Receive if Transaction is not Other

                        //if (Master.transactionType == "ReceiveReturn")

                        //    receiveDate = previousReceiveDate.Date;
                        //else
                        receiveDate = Convert.ToDateTime(Master.ReceiveDateTime);

                        #region Transaction is FromBOM
                        if (Master.FromBOM == "Y")
                        {
                            BomId = string.Empty;
                            BOMDate = DateTime.MinValue;

                            #region Last BOMId

                            sqlText = "  ";
                            sqlText += " select top 1 CONVERT(varchar(20),isnull(BOMId,0)) from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname=@ItemVatName ";
                            sqlText += " and effectdate<=@receiveDateDate";
                            sqlText += " and post='Y' ";
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                            cmdBomId.Transaction = transaction;
                            cmdBomId.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                            cmdBomId.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdBomId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                            if (cmdBomId.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BomId = "0";
                            }
                            else
                            {
                                BomId = (string)cmdBomId.ExecuteScalar();
                            }

                            #endregion Last BOMId

                            #region Last BOMDate

                            sqlText = "  ";
                            sqlText += " select top 1 CONVERT(varchar(20),isnull(EffectDate,'1900/01/01')) from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname=@ItemVatName";
                            sqlText += " and effectdate<=@receiveDateDate";
                            sqlText += " and post='Y' ";
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomEDate = new SqlCommand(sqlText, currConn);
                            cmdBomEDate.Transaction = transaction;

                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                            if (cmdBomEDate.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BOMDate = DateTime.MinValue;
                            }
                            else
                            {
                                BOMDate = (DateTime)cmdBomEDate.ExecuteScalar();
                            }

                            #endregion Last BOMDate

                            if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn" || Master.transactionType == "WIP")
                            {
                                #region Update to Issue

                                sqlText = "";
                                sqlText += " update IssueDetails set";
                                sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                                sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                                sqlText += " Post=@issueAutoPostValue";
                                sqlText += " WHERE  IssueNo =@MasterReceiveNo";
                                sqlText += "  and IssueDetails.FinishItemNo =@ItemItemNo";
                                SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                cmdInsertIssue.Transaction = transaction;
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                                transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgUnableToIssuePost);
                                }

                                #endregion Update to Issue

                                #region Find Raw Item From BOM  and update Stock

                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity from BOMRaws b  ";
                                sqlText += " WHERE ";

                                sqlText += " FinishItemNo   =@ItemItemNo";
                                sqlText += " and vatname    =@ItemVatName ";
                                sqlText += " and effectdate =@BOMDateDate";
                                sqlText += " and post='Y' ";

                                sqlText += "   and (rawitemtype='raw' or rawitemtype='pack') ";

                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;

                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@BOMDateDate", BOMDate.Date);


                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {
                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {
                                        #region Update Item Qty
                                        #region Qty  check and Update

                                        if (NegStockAllow == false)
                                        {
                                            #region Find Quantity From Products

                                            ProductDAL productDal = new ProductDAL();
                                            //decimal BRItemoldStock =
                                            //    productDal.StockInHand(BRItem["RawItemNo"].ToString(),
                                            //                           Master.ReceiveDateTime, currConn,
                                            //                           transaction);

                                            decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(),
                                                                                 Master.ReceiveDateTime,
                                                               currConn, transaction, true).Rows[0]["Quantity"].ToString());

                                            #endregion Find Quantity From Products

                                            #region Find Quantity From Transaction

                                            sqlText = "";
                                            sqlText +=
                                                "select isnull(isnull(Quantity ,0)+isnull(Wastage ,0),0) from IssueDetails ";
                                            sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                                       "and IssueNo=@MasterReceiveNo";
                                            SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                            cmdBRItemTranQty.Transaction = transaction;

                                            cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                            decimal BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                            #endregion Find Quantity From Transaction


                                            if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                            {
                                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                                MessageVM.
                                                                                    receiveMsgStockNotAvailablePost);
                                            }
                                        }



                                        #endregion Qty  check and Update

                                        #endregion Qty  check and Update
                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock
                            }


                        }

                        #endregion Transaction is FromBOM

                        #region Transaction is FromBOM

                        if (Master.transactionType == "TollFinishReceive")
                        {
                            ProductDAL productDal1 = new ProductDAL();
                            string FinishItemIdFromOH = productDal1.GetFinishItemIdFromOH(Item.ItemNo, "VAT 1 (Toll Issue)", Master.ReceiveDateTime,
                                                                           currConn, transaction);

                            BomId = string.Empty;  //BOMId
                            BOMDate = DateTime.MinValue;
                            #region Last BOMId
                            sqlText = "  ";
                            sqlText += " select top 1 CONVERT(varchar(20),isnull(BOMId,0)) from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@FinishItemIdFromOH ";
                            sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                            sqlText += " and effectdate<=@receiveDateDate";
                            sqlText += " and post='Y' ";
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                            cmdBomId.Transaction = transaction;

                            cmdBomId.Parameters.AddWithValueAndNullHandle("@FinishItemIdFromOH", FinishItemIdFromOH);
                            cmdBomId.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);

                            if (cmdBomId.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BomId = "0";
                            }
                            else
                            {
                                BomId = (string)cmdBomId.ExecuteScalar();
                            }

                            #endregion Last BOMId

                            #region Last BOMDate
                            sqlText = "  ";
                            sqlText += " select top 1 isnull(EffectDate,'1900/01/01') from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@FinishItemIdFromOH";
                            sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                            sqlText += " and effectdate<=@receiveDateDate";
                            sqlText += " and post='Y' ";
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomEDate = new SqlCommand(sqlText, currConn);
                            cmdBomEDate.Transaction = transaction;

                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@FinishItemIdFromOH", FinishItemIdFromOH);
                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);

                            if (cmdBomEDate.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BOMDate = DateTime.MinValue;
                            }
                            else
                            {
                                BOMDate = (DateTime)cmdBomEDate.ExecuteScalar();
                            }

                            #endregion Last BOMDate

                            #region Update to Issue

                            sqlText = "";
                            sqlText += " update IssueDetails set";
                            sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                            sqlText += " Post='" + issueAutoPostValue + "'";
                            sqlText += " WHERE  IssueNo =@MasterReceiveNo";
                            sqlText += "  and IssueDetails.FinishItemNo =@FinishItemIdFromOH";
                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@FinishItemIdFromOH", FinishItemIdFromOH);

                            transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgUnableToIssuePost);
                            }

                            #endregion Update to Issue

                            #region Find Raw Item From BOM  and update Stock


                            sqlText = "";
                            sqlText +=
                                " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity from BOMRaws b  ";
                            sqlText += " WHERE ";

                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and b.Vatname='VAT 1 (Toll Issue)'";
                            sqlText += " and effectdate=@BOMDateDate";
                            sqlText += " and post='Y' ";
                            sqlText += "   and (rawitemtype='raw' or rawitemtype='pack') ";

                            DataTable dataTable = new DataTable("RIFB");
                            SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                            cmdRIFB.Transaction = transaction;
                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@BOMDateDate", BOMDate.Date);

                            SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                            reportDataAdapt.Fill(dataTable);

                            if (dataTable == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgNoDataToPost);
                            }
                            else if (dataTable.Rows.Count <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgNoDataToPost);
                            }
                            else
                            {
                                foreach (DataRow BRItem in dataTable.Rows)
                                {
                                    #region Update Item Qty

                                    #region Find Quantity From Products

                                    ProductDAL productDal = new ProductDAL();
                                    //decimal BRItemoldStock = productDal.StockInHand(Item.ItemNo,
                                    //                                                Master.ReceiveDateTime, currConn,
                                    //                                                transaction);
                                    decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(Item.ItemNo,
                                                                           Master.ReceiveDateTime,
                                                         currConn, transaction, true).Rows[0]["Quantity"].ToString());



                                    #endregion Find Quantity From Products

                                    #region Find Quantity From Transaction

                                    sqlText = "";
                                    sqlText +=
                                        "select isnull(isnull(Quantity ,0)+isnull(Wastage ,0),0) from IssueDetails ";
                                    sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                               "and IssueNo=@MasterReceiveNo";
                                    SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                    cmdBRItemTranQty.Transaction = transaction;

                                    cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                    decimal BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                    #endregion Find Quantity From Transaction

                                    #region Qty  check and Update

                                    if (NegStockAllow == false)
                                    {
                                        if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                            MessageVM.
                                                                                receiveMsgStockNotAvailablePost);
                                        }
                                    }


                                    #endregion Qty  check and Update

                                    #endregion Qty  check and Update
                                }
                            }

                            #endregion Find Raw Item From BOM and update Stock
                        }




                        #endregion Transaction is FromBOM

                        #endregion Update Issue and Receive if Transaction is not Other
                    }

                    #endregion Find Transaction Mode Insert or Update
                }


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from ReceiveHeaders WHERE ReceiveNo=@MasterReceiveNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;

                cmdIPS.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgPostNotSelect);
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
                retResults[1] = MessageVM.receiveMsgSuccessfullyPost;
                retResults[2] = Master.ReceiveNo;
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

        #endregion Backup 18092013
        public string[] ReceiveInsert(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

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
            DateTime previousReceiveDate = DateTime.MinValue;
            DateTime VDate = DateTime.MinValue;
            string receiveDate = "";
            DateTime BOMDate = DateTime.MinValue; //start
            string BomId = string.Empty;
            bool withoutBOM = false;
            bool issueAutoPost = false;
            string issueAutoPostValue = "N";
            bool NegStockAllow = false;

            int nextId = 0;
            #endregion Initializ

            #region Try
            try
            {


                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.ReceiveDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.ReceiveDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, "Please Check Invoice Data and Time");

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
                    transaction = currConn.BeginTransaction(MessageVM.receiveMsgMethodNameInsert);
                }

                CommonDAL commonDal = new CommonDAL();
                int IssuePlaceQty = Convert.ToInt32(commonDal.settings("Issue", "Quantity"));
                int IssuePlaceAmt = Convert.ToInt32(commonDal.settings("Issue", "Amount"));
                int ReceivePlaceAmt = Convert.ToInt32(commonDal.settings("Receive", "Amount"));


                string vIssueAutoPost = string.Empty;
                vIssueAutoPost = commonDal.settings("IssueFromBOM", "IssueAutoPost");
                if (string.IsNullOrEmpty(vIssueAutoPost))
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Receive");
                }
                issueAutoPost = Convert.ToBoolean(vIssueAutoPost == "Y" ? true : false);
                if (issueAutoPost)
                    issueAutoPostValue = "Y";




                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionYearCheck = Convert.ToDateTime(Master.ReceiveDateTime).ToString("yyyy-MM-dd");
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
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
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
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK
                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(ReceiveNo) from ReceiveHeaders" +
                          " WHERE ReceiveNo=@MasterReceiveNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;

                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Purchase ID Create
                if (string.IsNullOrEmpty(Master.transactionType)) // start
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                #region Purchase ID Create For Other

                //CommonDAL commonDal = new CommonDAL();

                if (Master.transactionType == "Other" || Master.transactionType == "Tender")
                {
                    newID = commonDal.TransactionCode("Receive", "Other", "ReceiveHeaders", "ReceiveNo",
                                              "ReceiveDateTime", Master.ReceiveDateTime, currConn, transaction);
                }
                else if (Master.transactionType == "WIP")
                {
                    newID = commonDal.TransactionCode("Receive", "WIP", "ReceiveHeaders", "ReceiveNo",
                                              "ReceiveDateTime", Master.ReceiveDateTime, currConn, transaction);
                }
                else if (Master.transactionType == "TollFinishReceive")
                {
                    newID = commonDal.TransactionCode("TollFinishReceive", "TollFinishReceive", "ReceiveHeaders", "ReceiveNo",
                                              "ReceiveDateTime", Master.ReceiveDateTime, currConn, transaction);
                }
                else if (Master.transactionType == "PackageProduction")
                {
                    newID = commonDal.TransactionCode("Receive", "Package", "ReceiveHeaders", "ReceiveNo",
                                              "ReceiveDateTime", Master.ReceiveDateTime, currConn, transaction);
                }
                else if (Master.transactionType == "ReceiveReturn")
                {
                    newID = commonDal.TransactionCode("Receive", "ReceiveReturn", "ReceiveHeaders", "ReceiveNo",
                                              "ReceiveDateTime", Master.ReceiveDateTime, currConn, transaction);
                    #region Find Receive Return Date

                    sqlText = "";
                    sqlText = sqlText + "select ReceiveDateTime from ReceiveHeaders" +
                              " WHERE ReceiveNo=@MasterReturnId";
                    SqlCommand cmdFindPDate = new SqlCommand(sqlText, currConn);
                    cmdFindPDate.Transaction = transaction;
                    cmdFindPDate.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);

                    if (cmdFindPDate.ExecuteScalar() == null)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgFindExistID);
                    }
                    else
                    {
                        previousReceiveDate = (DateTime)cmdFindPDate.ExecuteScalar();
                    }


                    #endregion  Find Receive Return Date
                }


                #endregion Purchase ID Create For Other


                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header
                var Id = _cDal.NextId("ReceiveHeaders", null, null).ToString();

                sqlText = "";
                sqlText += " insert into ReceiveHeaders"; //Database Table name change
                sqlText += " (";
                //////sqlText += " Id,";
                sqlText += " ReceiveNo";
                sqlText += " ,ReceiveDateTime";
                sqlText += " ,WithToll";
                sqlText += " ,TotalAmount";
                sqlText += " ,TotalVATAmount";
                sqlText += " ,SerialNo";
                sqlText += " ,Comments";
                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,LastModifiedBy";
                sqlText += " ,LastModifiedOn";
                sqlText += " ,transactionType";
                sqlText += " ,ReceiveReturnId";
                sqlText += " ,ImportIDExcel";
                sqlText += " ,ReferenceNo";
                sqlText += " ,CustomerID";
                sqlText += " ,Post";
                sqlText += " ,ShiftId";
                
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                //////sqlText += "@Id";
                sqlText += "@newID";
                sqlText += ",@MasterReceiveDateTime";
                sqlText += ",@MasterWithToll";
                sqlText += ",@MasterTotalAmount";
                sqlText += ",@MasterTotalVATAmount";
                sqlText += ",@MasterSerialNo";
                sqlText += ",@MasterComments";
                sqlText += ",@MasterCreatedBy";
                sqlText += ",@MasterCreatedOn";
                sqlText += ",@MasterLastModifiedBy";
                sqlText += ",@MasterLastModifiedOn";
                sqlText += ",@MasterTransactionType";
                sqlText += ",@MasterReturnId";
                sqlText += ",@MasterImportId";
                sqlText += ",@MasterReferenceNo";
                sqlText += ",@MasterCustomerID";
                sqlText += ",@MasterPost";
                sqlText += ",@ShiftId";
                sqlText += ") SELECT SCOPE_IDENTITY()";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                ////////cmdInsert.Parameters.AddWithValueAndNullHandle("@Id", Id);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@newID", newID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterWithToll", Master.WithToll);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalAmount", Master.TotalAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterImportId", Master.ImportId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReferenceNo", Master.ReferenceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ShiftId", Master.ShiftId);
                

                var exec = cmdInsert.ExecuteScalar();

                transResult = Convert.ToInt32(exec);
                Master.Id = transResult.ToString();

                ////// transResult = (int)cmdInsert.ExecuteNonQuery();
                //////Master.Id = Id.ToString();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgSaveNotSuccessfully);
                }


                #endregion ID generated completed,Insert new Information in Header

                #region if Transection not Other Insert Issue /Receive

                #region Receive For BOM
                if (Master.FromBOM == "Y")
                {
                    string vNegStockAllow = string.Empty;
                    vNegStockAllow = commonDal.settings("Issue", "NegStockAllow");
                    NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);

                    if (string.IsNullOrEmpty(vNegStockAllow))
                    {
                        throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Receive");
                    }


                    if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn"
                        || Master.transactionType == "WIP" || Master.transactionType == "Tender"
                        || Master.transactionType == "TollFinishReceive")
                    {
                        #region Insert to Issue Header

                        sqlText = "";
                        sqlText += " insert into IssueHeaders(";
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
                        sqlText += " values(	";
                        //sqlText += "'" + Master.Id + "',";

                        sqlText += "'" + newID + "',";

                        sqlText += "@MasterReceiveDateTime,";
                        sqlText += " 0,0,";
                        sqlText += "@MasterSerialNo,";
                        sqlText += "@MasterComments,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@MasterReceiveNo,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "'" + issueAutoPostValue + "'";

                        //sqlText += "'" + Master.@Post + "'";
                        sqlText += ")	";

                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue.Transaction = transaction;

                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);

                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();
                        if (transResult <= 0)
                        {

                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            MessageVM.receiveMsgUnableToSaveIssue);
                        }

                        #endregion Insert to Issue Header

                    }
                    else if (Master.transactionType == "PackageProduction")// insert into sale header
                    {
                        #region Insert to Issue Header

                        sqlText = "";
                        sqlText += " insert into IssueHeaders(";
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
                        sqlText += " values(	";

                        sqlText += "'" + newID + "',";

                        sqlText += "@MasterReceiveDateTime,";
                        sqlText += " 0,0,";
                        sqlText += "@MasterSerialNo,";
                        sqlText += "@MasterComments,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@MasterReceiveNo,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@issueAutoPostValue";

                        sqlText += ")	";

                        SqlCommand cmdInsertIssue9 = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue9.Transaction = transaction;
                        cmdInsertIssue9.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdInsertIssue9.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo);
                        cmdInsertIssue9.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                        cmdInsertIssue9.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                        cmdInsertIssue9.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsertIssue9.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsertIssue9.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsertIssue9.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                        cmdInsertIssue9.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                        cmdInsertIssue9.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);

                        transResult = (int)cmdInsertIssue9.ExecuteNonQuery();
                        if (transResult <= 0)
                        {

                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            MessageVM.receiveMsgUnableToSaveIssue);
                        }

                        #endregion Insert to Issue Header
                        #region Insert to SalesInvoiceHeaders Header

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

                        sqlText += " Post";
                        sqlText += " )";

                        sqlText += " values";
                        sqlText += " (";
                        sqlText += "'" + newID + "',";
                        sqlText += "'0',";
                        sqlText += "'NA',";
                        sqlText += "'NA',";
                        sqlText += "'NA',";
                        sqlText += "'NA',";
                        sqlText += "@MasterReceiveDateTime,";
                        sqlText += "@MasterTotalAmount,";
                        sqlText += "@MasterTotalVATAmount,";
                        sqlText += "@MasterSerialNo,";
                        sqlText += "@MasterComments,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "'New',";
                        sqlText += "@MasterReturnId,";
                        sqlText += "'N',";//'" + Master.Trading + "',";
                        sqlText += "'N',";//'" + Master.IsPrint + "',";
                        sqlText += "'0',";//'" + Master.TenderId + "',";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReceiveDateTime,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "'260',";//'" + Master.CurrencyID + "',";
                        sqlText += "'0',";//'" + Master.CurrencyRateFromBDT + "',";
                        sqlText += "@MasterPost";
                        sqlText += ")";

                        SqlCommand cmdInsertIssue10 = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue10.Transaction = transaction;

                        cmdInsertIssue10.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdInsertIssue10.Parameters.AddWithValueAndNullHandle("@MasterTotalAmount", Master.TotalAmount);
                        cmdInsertIssue10.Parameters.AddWithValueAndNullHandle("@MasterTotalVATAmount", Master.TotalVATAmount);
                        cmdInsertIssue10.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo);
                        cmdInsertIssue10.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                        cmdInsertIssue10.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                        cmdInsertIssue10.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsertIssue10.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsertIssue10.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsertIssue10.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                        cmdInsertIssue10.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                        cmdInsertIssue10.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdInsertIssue10.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);

                        transResult = (int)cmdInsertIssue10.ExecuteNonQuery();
                        if (transResult <= 0)
                        {

                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            MessageVM.receiveMsgUnableToSaveIssue);
                        }

                        #endregion Insert to SalesInvoiceHeaders Header
                    }




                }//  if (Master.FromBOM == "Y")

                #endregion Receive For BOM



                #endregion if Transection not Other Insert Issue /Receive

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table
                var lineNo = 1;
                foreach (var Item in Details.ToList())
                {
                    Item.ReceiveLineNo = lineNo.ToString();
                    lineNo++;
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo='" + newID + "' ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                        MessageVM.receiveMsgFindExistID);
                    }

                    #endregion Find Transaction Exist
                    #region USD calculate

                    string[] usdResults = GetUSDCurrency(Item.SubTotal);
                    #endregion USD calculate
                    #region Insert only DetailTable

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
                    sqlText += " transactionType,";
                    sqlText += " ReceiveReturnId,";
                    sqlText += " BOMId,";

                    sqlText += " UOMPrice,";
                    sqlText += " UOMQty,";
                    sqlText += " UOMn,";
                    sqlText += " UOMc,";
                    sqlText += " VATName,";
                    sqlText += " CurrencyValue,";
                    sqlText += " DollerValue,";
                    if (Master.transactionType == "ReceiveReturn")
                    {
                        sqlText += " ReturnTransactionType,";
                    }

                    sqlText += " Post";
                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "'" + newID + "',";
                    sqlText += "@ItemReceiveLineNo,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemQuantity,";
                    sqlText += "@ItemCostPrice,";
                    sqlText += "@ItemNBRPrice,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemVATRate,";
                    sqlText += "@ItemVATAmount,";
                    sqlText += "@ItemSubTotal,";
                    sqlText += "@ItemCommentsD,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn,";
                    sqlText += "@ItemSD,";
                    sqlText += "@ItemSDAmount,";
                    sqlText += "@MasterReceiveDateTime,";
                    sqlText += "@MasterTransactionType,";
                    sqlText += "@MasterReturnId,";
                    sqlText += "@ItemBOMId,";
                    sqlText += "@ItemUOMPrice,";
                    sqlText += "@ItemUOMQty,";
                    sqlText += "@ItemUOMn,";
                    sqlText += "@ItemUOMc,";
                    sqlText += "@ItemVatName,";
                    sqlText += "'" + usdResults[0] + "',";//CurrencyValue
                    sqlText += "'" + usdResults[1] + "',";//DollerValue
                    if (Master.transactionType == "ReceiveReturn")
                    {
                        sqlText += "'" + Item.ReturnTransactionType + "',";
                    }

                    sqlText += "'" + Master.Post + "'";
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;

                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCostPrice", Item.CostPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemNBRPrice", Item.NBRPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATRate", Item.VATRate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATAmount", Item.VATAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSubTotal", Item.SubTotal);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSD", Item.SD);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSDAmount", Item.SDAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBOMId", Item.BOMId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMPrice", Item.UOMPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMQty", Item.UOMQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMn", Item.UOMn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMc", Item.UOMc);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                        MessageVM.receiveMsgSaveNotSuccessfully);
                    }

                    #endregion Insert only DetailTable  //done

                    //done
                    if (Master.transactionType == "ReceiveReturn")

                        //receiveDate = previousReceiveDate.Date;
                        receiveDate = previousReceiveDate.Date.ToString("yyyy/MM/dd");
                    else
                    {
                        VDate = Convert.ToDateTime(Master.ReceiveDateTime);
                        receiveDate = VDate.Date.ToString("yyyy/MM/dd");
                    }
                    #region TollReceive
                    if (Master.transactionType == "ReceiveReturn" && Item.ReturnTransactionType == "TollReceive")
                    {
                        ProductDAL productDal = new ProductDAL();
                        DataTable TollItemInfo = productDal.SearchRawItemNo(Master.ReturnId);

                        string TollItem;
                        decimal TollUnitCost = 0;

                        TollItem = TollItemInfo.Rows[0]["ItemNo"].ToString();
                        TollUnitCost = Convert.ToDecimal(TollItemInfo.Rows[0]["CostPrice"].ToString());

                        #region Insert to Issue 16 out

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

                        sqlText += " UOMQty,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMc,";
                        sqlText += " UOMn,";
                        sqlText += " UOMWastage,";


                        sqlText += " Post";

                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "'" + newID + "',";
                        sqlText += "'10',";
                        sqlText += "'" + TollItem + "',";
                        sqlText += "@ItemQuantity,";
                        sqlText += " 0,"; // NBR
                        //sqlText += " (select isnull(TollCharge,0)  from products where itemno=" + Item.ItemNo + "),";
                        sqlText += "'" + TollUnitCost + "',";

                        sqlText += "@ItemUOM,";
                        sqlText += " 0,	";
                        sqlText += " 0,";
                        sqlText += "@TollUnitCostItemUOMQty,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "'" + newID + "',";
                        sqlText += "@MasterReceiveDateTime,";
                        sqlText += " 0,";
                        sqlText += " 0,";
                        sqlText += " 0,	";
                        sqlText += " 0,	";
                        sqlText += " 0,";
                        sqlText += "'TollReceiveReturn',";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "'0',";

                        sqlText += "@MasterPost";

                        sqlText += ")	";
                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                        cmdInsertIssue.Transaction = transaction;

                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@TollUnitCostItemUOMQty", TollUnitCost * Item.UOMQty);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMQty", Item.UOMQty);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMc", Item.UOMc);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMn", Item.UOMn);
                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);

                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.PurchasemsgUnableToSaveIssue);
                        }

                        #endregion Insert to Issue




                    }
                    #endregion TollReceive

                    #region Transaction is FromBOM

                    if (Master.FromBOM == "Y")
                    {

                        ProductDAL productDal = new ProductDAL();

                        BomId = string.Empty;
                        BOMDate = DateTime.MinValue;
                        string bomDate = "";
                        #region Last BOMId

                        sqlText = "  ";
                        sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId from BOMs";
                        sqlText += " where ";
                        sqlText += " FinishItemNo   =@ItemItemNo";
                        sqlText += " and vatname    =@ItemVatName";
                        sqlText += " and effectdate<=@receiveDate";
                        sqlText += " and post='Y' ";
                        if (Master.CustomerID != "0")
                        {
                            sqlText += " and CustomerID=@MasterCustomerID";
                        }
                        sqlText += " order by effectdate desc ";

                        SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                        cmdBomId.Transaction = transaction;

                        cmdBomId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                        cmdBomId.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                        cmdBomId.Parameters.AddWithValueAndNullHandle("@receiveDate", receiveDate);
                        cmdBomId.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                        if (cmdBomId.ExecuteScalar() == null)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            "No Price declaration found for this item");
                            BomId = "0";
                        }
                        else
                        {
                            BomId = (string)cmdBomId.ExecuteScalar();
                        }

                        #endregion Last BOMId

                        #region Last BOMDate

                        sqlText = "  ";
                        sqlText += " select top 1 isnull(EffectDate,'1900/01/01') from BOMs";
                        sqlText += " where FinishItemNo =@ItemItemNo";
                        sqlText += " and vatname        =@ItemVatName ";
                        sqlText += " and effectdate<='" + receiveDate + "'";
                        sqlText += " and post='Y' ";
                        if (Master.CustomerID != "0")
                        {
                            sqlText += " and CustomerID=@MasterCustomerID  ";
                        }
                        sqlText += " order by effectdate desc ";

                        SqlCommand cmdBomEDate = new SqlCommand(sqlText, currConn);
                        cmdBomEDate.Transaction = transaction;

                        cmdBomEDate.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                        cmdBomEDate.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                        cmdBomEDate.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                        if (cmdBomEDate.ExecuteScalar() == null)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                            "No Price declaration found for this item");
                            BOMDate = DateTime.MinValue;
                            bomDate = BOMDate.Date.ToString("yyyy/MM/dd 00:00:00");
                        }
                        else
                        {
                            BOMDate = (DateTime)cmdBomEDate.ExecuteScalar();
                            bomDate = BOMDate.Date.ToString("yyyy/MM/dd 00:00:00");
                        }

                        #endregion Last BOMDate

                        if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn" ||
                            Master.transactionType == "WIP")
                        {
                            #region Find Raw Item From BOM  and update Stock

                            //sss
                            sqlText = "";
                            sqlText +=
                                " SELECT  b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ," +
                                "b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty,b.TransactionType from BOMRaws b  ";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname=@ItemVatName ";
                            sqlText += " and effectdate=@bomDate";
                            sqlText += " and post='Y' ";
                            sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID";
                            }

                            //sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='finish') ";
                            sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='wip' or rawitemtype='finish' or rawitemtype='export') ";



                            DataTable dataTable = new DataTable("RIFB");
                            SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                            cmdRIFB.Transaction = transaction;

                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@bomDate", bomDate);
                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                            reportDataAdapt.Fill(dataTable);

                            if (dataTable == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgNoDataToPost);
                            }
                            else if (dataTable.Rows.Count <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                "There is no Item for Auto Consumption for the Item Name (" + Item.ItemName + ") and Item Code (" + Item.ItemCode + ") in price declaration.");
                            }
                            else
                            {
                                decimal vQuantity = 0;
                                decimal vWastage = 0;
                                decimal vStock = 0;
                                string rwUom = "";
                                decimal vConvertionRate = 0;
                                decimal AvgRate = 0;

                                foreach (DataRow BRItem in dataTable.Rows)
                                {
                                    #region Declare

                                    decimal v1Quantity = 0;
                                    string v1RawItemNo = "";
                                    decimal v1CostPrice = 0;
                                    string v1UOM = "";
                                    decimal v1SubTotal = 0;
                                    decimal v1Wastage = 0;
                                    DateTime v1BOMDate = DateTime.Now.Date;
                                    string v1FinishItemNo = "";

                                    decimal v1UOMQty = 0;
                                    decimal v1UOMPrice = 0;
                                    decimal v1UOMc = 0;
                                    string v1UOMn = "";
                                    string v1BOMId = "";
                                    decimal v1UOMWastage = 0;
                                    string vTransactionType = "";

                                    #endregion Declare

                                    #region Update Item Qty

                                    #region Find Quantity From Products

                                    DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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


                                    vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                    vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                    #region Issue Settings
                                    //CommonDAL commDal = new CommonDAL();
                                    //int IssuePlaceQty = Convert.ToInt32(commDal.settings("Issue", "Quantity"));
                                    //int IssuePlaceAmt = Convert.ToInt32(commDal.settings("Issue", "Amount"));
                                    AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                    vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                    vWastage = FormatingNumeric(vWastage, IssuePlaceQty);
                                    #endregion Issue Settings

                                    rwUom = BRItem["Uom"].ToString();

                                    var rwMajorUom = BRItem["Uomn"].ToString();
                                    if (string.IsNullOrEmpty(rwUom))
                                    {
                                        throw new ArgumentNullException("ReceiveInsert",
                                                                        "Could not find UOM of raw item");
                                    }

                                    /*Processing UOM*/

                                    UOMDAL uomdal = new UOMDAL();
                                    vConvertionRate = uomdal.GetConvertionRate(rwMajorUom, rwUom, "Y", currConn, transaction); //uomc


                                    #region valueAssign

                                    v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                    v1Wastage = (vWastage) * Item.UOMQty;
                                    v1BOMId = BomId;
                                    v1RawItemNo = BRItem["RawItemNo"].ToString();
                                    v1UOM = BRItem["UOM"].ToString();
                                    v1CostPrice = AvgRate * vConvertionRate;
                                    v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                    v1UOMPrice = AvgRate;
                                    v1UOMn = BRItem["UOMn"].ToString();
                                    v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                    v1FinishItemNo = Item.ItemNo;
                                    v1UOMc = vConvertionRate;
                                    v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                    v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                    vTransactionType = BRItem["TransactionType"].ToString();
                                    #endregion valueAssign

                                    #region Stock

                                    if (NegStockAllow == false)
                                    {
                                        //var stock = productDal.StockInHand(BRItem["RawItemNo"].ToString(),
                                        //                                       Master.ReceiveDateTime,
                                        //                                   currConn, transaction).ToString();

                                        var stock = Convert.ToDecimal(productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(),
                                                                               Master.ReceiveDateTime,
                                                             currConn, transaction, false).Rows[0]["Quantity"].ToString());

                                        vStock = Convert.ToDecimal(stock);


                                        if ((vStock - v1UOMQty) < 0)
                                        {
                                            string FinName = string.Empty;
                                            string FinCode = string.Empty;
                                            string RawName = string.Empty;
                                            string RawCode = string.Empty;
                                            DataTable finDt = new DataTable();
                                            finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                            foreach (DataRow FinItem in finDt.Rows)
                                            {
                                                FinName = FinItem["ProductName"].ToString();
                                                FinCode = FinItem["ProductCode"].ToString();
                                            }
                                            DataTable rawDt = new DataTable();
                                            rawDt =
                                                productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                            foreach (DataRow RawItem in rawDt.Rows)
                                            {
                                                RawName = RawItem["ProductName"].ToString();
                                                RawCode = RawItem["ProductCode"].ToString();
                                            }

                                            throw new ArgumentNullException("ReceiveInsert",
                                                                            "Stock not Available for Finish Item( Name: " +
                                                                            FinName + " & Code: " + FinCode +
                                                                            " ) \n and consumtion Material ( Name: " +
                                                                            RawName + " & Code: " + RawCode + " )");
                                        }
                                    }

                                    #endregion Stock

                                    #endregion Find Quantity From Products


                                    #region Find Quantity From Transaction


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
                                    sqlText += " SDAmount,"; //19
                                    sqlText += " Wastage,";
                                    sqlText += " BOMDate,";
                                    sqlText += " FinishItemNo,";
                                    sqlText += " transactionType,";
                                    sqlText += " IssueReturnId,";
                                    sqlText += " UOMQty,";
                                    sqlText += " UOMPrice,";
                                    sqlText += " UOMc,";
                                    sqlText += " UOMn,";
                                    sqlText += " UOMWastage,";
                                    sqlText += " BOMId,";

                                    sqlText += " Post";
                                    sqlText += " )";
                                    sqlText += " values( ";
                                    sqlText += "'" + newID + "',";
                                    sqlText += "@ItemReceiveLineNo,";
                                    sqlText += "@v1RawItemNo, ";
                                    sqlText += "@v1QuantityIssuePlaceQty ,";
                                    sqlText += "@AvgRate,";
                                    sqlText += "@v1CostPriceIssuePlaceAmt,";
                                    sqlText += "@v1UOM,";
                                    sqlText += " 0,0, ";
                                    sqlText += "@v1SubTotalIssuePlaceAmt,";
                                    sqlText += "@ItemCommentsD,";
                                    sqlText += "@MasterCreatedBy,";
                                    sqlText += "@MasterCreatedOn,";
                                    sqlText += "@MasterLastModifiedBy,";
                                    sqlText += "@MasterLastModifiedOn,";
                                    sqlText += "'" + newID + "',";
                                    sqlText += "@MasterReceiveDateTime,";
                                    sqlText += " 0,	0,";
                                    sqlText += "@v1Wastage,	";
                                    sqlText += "@v1BOMDate,	";
                                    sqlText += "@v1FinishItemNo,";
                                    if (vTransactionType.Trim() == "TollReceiveRaw")
                                    {
                                        sqlText += " @vTransactionTypeTrim,";
                                    }
                                    else
                                    {
                                        if (Master.WithToll == "Y")
                                        {
                                            sqlText += "@MasterTransactionType" + "Toll" + "',";

                                        }
                                        else
                                        {
                                            sqlText += "@MasterTransactionType,";
                                        }
                                    }

                                    sqlText += "@MasterReturnId,";
                                    sqlText += "@v1UOMQtyIssuePlaceQty,";
                                    sqlText += "@v1UOMPriceIssuePlaceAmt,";
                                    sqlText += "@v1UOMc,";
                                    sqlText += "@v1UOMn,";
                                    sqlText += "@v1UOMWastage,";
                                    sqlText += "@v1BOMId,";

                                    sqlText += "'" + issueAutoPostValue + "'";
                                    sqlText += ")";
                                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                    cmdInsertIssue.Transaction = transaction;

                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1QuantityIssuePlaceQty", FormatingNumeric(v1Quantity, IssuePlaceQty));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPriceIssuePlaceAmt", FormatingNumeric(v1CostPrice, IssuePlaceAmt));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotalIssuePlaceAmt", FormatingNumeric(v1SubTotal, IssuePlaceAmt));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", Convert.ToDateTime(v1BOMDate).ToString("MM/dd/yyyy"));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQtyIssuePlaceQty", FormatingNumeric(v1UOMQty, IssuePlaceQty));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPriceIssuePlaceAmt", FormatingNumeric(v1UOMPrice, IssuePlaceAmt));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);

                                    if (vTransactionType.Trim() == "TollReceiveRaw")
                                    {
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@vTransactionTypeTrim", vTransactionType.Trim());
                                    }
                                    else
                                    {
                                        if (Master.WithToll == "Y")
                                        {
                                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                        }
                                        else
                                        {
                                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                        }
                                    }

                                    transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }


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
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

                                    #endregion Update Issue

                                    #endregion Qty  check and Update

                                    #endregion Qty  check and Update
                                }
                            }

                            #endregion Find Raw Item From BOM and update Stock

                        }
                        else if (Master.transactionType == "Tender")
                        {
                            #region Declare

                            decimal v1Quantity = 0;
                            string v1RawItemNo = "";
                            decimal v1CostPrice = 0;
                            string v1UOM = "";
                            decimal v1SubTotal = 0;
                            decimal v1Wastage = 0;
                            DateTime v1BOMDate = DateTime.Now.Date;
                            string v1FinishItemNo = "";

                            decimal v1UOMQty = 0;
                            decimal v1UOMPrice = 0;
                            decimal v1UOMc = 0;
                            string v1UOMn = "";
                            string v1BOMId = "";
                            decimal v1UOMWastage = 0;

                            #endregion Declare

                            #region Find Raw Item From BOM  and update Stock
                            #region Insert into Issue

                            sqlText = "";


                            sqlText +=
                                " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ," +
                                "b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty from BOMRaws b  ";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname=@ItemVatName ";
                            sqlText += " and effectdate=@bomDate";
                            sqlText += " and post='Y' ";
                            sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID ";
                            }

                            sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='finish') ";


                            DataTable dataTable = new DataTable("RIFB");
                            SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                            cmdRIFB.Transaction = transaction;

                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@bomDate", bomDate);
                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                            reportDataAdapt.Fill(dataTable);

                            if (dataTable == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgNoDataToPost);
                            }
                            else if (dataTable.Rows.Count <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                "There is no price declaration for this item.");
                            }
                            else
                            {
                                decimal vQuantity = 0;
                                decimal vWastage = 0;
                                string rwUom = "";
                                decimal vConvertionRate = 0;
                                decimal AvgRate = 0;

                                foreach (DataRow BRItem in dataTable.Rows)
                                {
                                    #region Update Item Qty

                                    #region Find Quantity From Products

                                    DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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

                                    vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                    vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                    #region Issue Settings
                                    //CommonDAL commDal = new CommonDAL();
                                    //int IssuePlaceQty = Convert.ToInt32(commDal.settings("Issue", "Quantity"));
                                    //int IssuePlaceAmt = Convert.ToInt32(commDal.settings("Issue", "Amount"));
                                    AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                    vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                    vWastage = FormatingNumeric(vWastage, IssuePlaceQty);
                                    #endregion Issue Settings



                                    rwUom = BRItem["Uom"].ToString();
                                    var rwUomMajor = BRItem["Uomn"].ToString();
                                    if (string.IsNullOrEmpty(rwUom))
                                    {
                                        throw new ArgumentNullException("ReceiveInsert",
                                                                        "Could not find UOM of raw item");
                                    }

                                    /*Processing UOM*/

                                    UOMDAL uomdal = new UOMDAL();
                                    vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);


                                    #endregion Find Quantity From Products

                                    #region valueAssign

                                    v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                    v1Wastage = (vWastage) * Item.UOMQty;
                                    v1BOMId = Item.BOMId;
                                    v1RawItemNo = BRItem["RawItemNo"].ToString();
                                    v1UOM = BRItem["UOM"].ToString();
                                    v1CostPrice = AvgRate * vConvertionRate;
                                    v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                    v1UOMPrice = AvgRate;
                                    v1UOMn = BRItem["UOMn"].ToString();
                                    v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                    v1FinishItemNo = Item.ItemNo;
                                    v1UOMc = vConvertionRate;
                                    v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                    v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;

                                    #endregion valueAssign

                                    #region Stock

                                    if (NegStockAllow == false)
                                    {
                                        decimal vStock = 0;

                                        var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                                              Master.ReceiveDateTime,
                                                            currConn, transaction, false).Rows[0]["Quantity"].ToString();

                                        vStock = Convert.ToDecimal(stock);


                                        if ((vStock - v1UOMQty) < 0)
                                        {
                                            string FinName = string.Empty;
                                            string FinCode = string.Empty;
                                            string RawName = string.Empty;
                                            string RawCode = string.Empty;
                                            DataTable finDt = new DataTable();
                                            finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                            foreach (DataRow FinItem in finDt.Rows)
                                            {
                                                FinName = FinItem["ProductName"].ToString();
                                                FinCode = FinItem["ProductCode"].ToString();
                                            }
                                            DataTable rawDt = new DataTable();
                                            rawDt =
                                                productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                            foreach (DataRow RawItem in rawDt.Rows)
                                            {
                                                RawName = RawItem["ProductName"].ToString();
                                                RawCode = RawItem["ProductCode"].ToString();
                                            }

                                            throw new ArgumentNullException("ReceiveInsert",
                                                                            "Stock not Available for Finish Item( Name: " +
                                                                            FinName + " & Code: " + FinCode +
                                                                            " ) \n and consumtion Material ( Name: " +
                                                                            RawName + " & Code: " + RawCode + " )");
                                        }
                                    }

                                    #endregion Stock



                                    #region Find Quantity From Transaction

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
                                    sqlText += " UOMWastage,";
                                    sqlText += " BOMId,";
                                    sqlText += " Post";
                                    sqlText += " )";
                                    sqlText += " values( ";
                                    sqlText += "'" + newID + "',";
                                    sqlText += "@ItemReceiveLineNo,";
                                    sqlText += "@v1RawItemNo, ";
                                    sqlText += "@v1QuantityIssuePlaceQty ,";
                                    sqlText += "@AvgRate,";
                                    sqlText += "@v1CostPriceIssuePlaceAmt,";

                                    sqlText += "@v1UOM,";
                                    sqlText += " 0,0, ";
                                    sqlText += "@v1SubTotalIssuePlaceAmt,";
                                    sqlText += "@ItemCommentsD,";
                                    sqlText += "@MasterCreatedBy,";
                                    sqlText += "@MasterCreatedOn,";
                                    sqlText += "@MasterLastModifiedBy,";
                                    sqlText += "@MasterLastModifiedOn,";
                                    sqlText += "'" + newID + "',";
                                    sqlText += "@MasterReceiveDateTime,";
                                    sqlText += " 0,	0,";
                                    sqlText += "@v1Wastage,	";
                                    sqlText += "@v1BOMDate,	";
                                    sqlText += "@v1FinishItemNo,";
                                    sqlText += "@MasterTransactionType,";
                                    sqlText += "@MasterReturnId,";
                                    sqlText += "@Fv1UOMQtyIssuePlaceQty,";
                                    sqlText += "@v1UOMPriceIssuePlaceAmt,";
                                    sqlText += "@v1UOMc,";
                                    sqlText += "@v1UOMn,";
                                    sqlText += "@v1UOMWastage,";
                                    sqlText += "@v1BOMId,";
                                    sqlText += "'" + issueAutoPostValue + "'";
                                    sqlText += ")";
                                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                    cmdInsertIssue.Transaction = transaction;

                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1QuantityIssuePlaceQty", FormatingNumeric(v1Quantity, IssuePlaceQty));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPriceIssuePlaceAmt", FormatingNumeric(v1CostPrice, IssuePlaceAmt));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotalIssuePlaceAmt", FormatingNumeric(v1SubTotal, IssuePlaceAmt));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQtyIssuePlaceQty", FormatingNumeric(v1UOMQty, IssuePlaceQty));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPriceIssuePlaceAmt", FormatingNumeric(v1UOMPrice, IssuePlaceAmt));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);

                                    transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

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
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

                                    #endregion Update Issue

                                    #endregion Qty  check and Update

                                    #endregion Qty  check and Update
                                }
                            }
                            #endregion Insert into Issue

                            #endregion Find Raw Item From BOM and update Stock
                        }


                    #endregion Transaction is Trading

                        #region TollFinishReceive

                        else if (Master.transactionType == "TollFinishReceive")
                        {
                            #region TollFinishReceive


                            BomId = string.Empty; //BOMId
                            BOMDate = DateTime.MinValue;

                            #region Last BOMId

                            sqlText = "  ";
                            sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                            sqlText += " and effectdate<=@receiveDate";
                            sqlText += " and post='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID ";
                            }
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomId1 = new SqlCommand(sqlText, currConn);
                            cmdBomId1.Transaction = transaction;

                            cmdBomId1.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomId1.Parameters.AddWithValueAndNullHandle("@receiveDat", receiveDate);
                            cmdBomId1.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            if (cmdBomId1.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BomId = "0";
                            }
                            else
                            {
                                BomId = (string)cmdBomId1.ExecuteScalar();
                            }

                            #endregion Last BOMId

                            #region Last BOMDate

                            sqlText = "  ";
                            sqlText += " select top 1 isnull(EffectDate,'1900/01/01') from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo";
                            sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                            sqlText += " and effectdate<=@receiveDate";
                            sqlText += " and post='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID ";
                            }
                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomEDate1 = new SqlCommand(sqlText, currConn);
                            cmdBomEDate1.Transaction = transaction;

                            cmdBomEDate1.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomEDate1.Parameters.AddWithValueAndNullHandle("@receiveDat", receiveDate);
                            cmdBomEDate1.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            if (cmdBomEDate1.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BOMDate = DateTime.MinValue;
                                bomDate = DateTime.MinValue.ToString("yyyy/MM/dd 00:00:00");
                            }
                            else
                            {
                                BOMDate = (DateTime)cmdBomEDate1.ExecuteScalar();
                                bomDate = BOMDate.Date.ToString("yyyy/MM/dd 00:00:00");
                            }

                            #endregion Last BOMDate


                            #region Find Raw Item From BOM  and update Stock

                            sqlText = "";


                            sqlText +=
                                " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty  from BOMRaws b  ";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                            sqlText += " and effectdate=@bomDate";
                            sqlText += " and post='Y' ";
                            sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";

                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID ";
                            }

                            sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='finish') ";

                            DataTable dataTable = new DataTable("RIFB");
                            SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                            cmdRIFB.Transaction = transaction;

                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@bomDate", bomDate);
                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                            reportDataAdapt.Fill(dataTable);

                            if (dataTable == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgNoDataToPost);
                            }
                            else if (dataTable.Rows.Count <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                "There is no price declaration for this item.");
                            }
                            else
                            {
                                #region Declare

                                decimal v1Quantity = 0;
                                string v1RawItemNo = "";
                                decimal v1CostPrice = 0;
                                string v1UOM = "";
                                decimal v1SubTotal = 0;
                                decimal v1Wastage = 0;
                                DateTime v1BOMDate = DateTime.Now.Date;
                                string v1FinishItemNo = "";

                                decimal v1UOMQty = 0;
                                decimal v1UOMPrice = 0;
                                decimal v1UOMc = 0;
                                string v1UOMn = "";
                                string v1BOMId = "";
                                decimal v1UOMWastage = 0;

                                #endregion Declare

                                decimal vQuantity = 0;
                                decimal vWastage = 0;

                                string rwUom = "";
                                decimal vConvertionRate = 0;
                                decimal AvgRate = 0;


                                foreach (DataRow BRItem in dataTable.Rows)
                                {
                                    #region Update Item Qty

                                    #region Find Quantity From Products

                                    DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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

                                    vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                    vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                    #region Issue Settings
                                    //CommonDAL commDal = new CommonDAL();
                                    //int IssuePlaceQty = Convert.ToInt32(commDal.settings("Issue", "Quantity"));
                                    //int IssuePlaceAmt = Convert.ToInt32(commDal.settings("Issue", "Amount"));
                                    AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                    vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                    vWastage = FormatingNumeric(vWastage, IssuePlaceQty);
                                    #endregion Issue Settings

                                    rwUom = BRItem["Uom"].ToString();
                                    var rwUomMajor = BRItem["Uomn"].ToString();
                                    if (string.IsNullOrEmpty(rwUom))
                                    {
                                        throw new ArgumentNullException("ReceiveInsert",
                                                                        "Could not find UOM of raw item");
                                    }

                                    /*Processing UOM*/

                                    UOMDAL uomdal = new UOMDAL();
                                    vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);

                                    #endregion Find Quantity From Products

                                    #region valueAssign

                                    v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                    v1Wastage = (vWastage) * Item.UOMQty;
                                    v1BOMId = BomId;
                                    v1RawItemNo = BRItem["RawItemNo"].ToString();
                                    v1UOM = BRItem["UOM"].ToString();
                                    v1CostPrice = AvgRate * vConvertionRate;
                                    v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                    v1UOMPrice = AvgRate;
                                    v1UOMn = BRItem["UOMn"].ToString();
                                    v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                    v1FinishItemNo = Item.ItemNo;
                                    v1UOMc = vConvertionRate;
                                    v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                    v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;

                                    #endregion valueAssign

                                    #region Stock

                                    if (NegStockAllow == false)
                                    {
                                        decimal vStock = 0;
                                        //var stock = productDal.StockInHand(v1RawItemNo,
                                        //                                       Master.ReceiveDateTime+
                                        //                                       DateTime.Now.ToString(" HH:mm:ss"),
                                        //                                   currConn, transaction).ToString();
                                        var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                                              Master.ReceiveDateTime,
                                                            currConn, transaction, false).Rows[0]["Quantity"].ToString();


                                        vStock = Convert.ToDecimal(stock);


                                        if ((vStock - v1UOMQty) < 0)
                                        {
                                            string FinName = string.Empty;
                                            string FinCode = string.Empty;
                                            string RawName = string.Empty;
                                            string RawCode = string.Empty;
                                            DataTable finDt = new DataTable();
                                            finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                            foreach (DataRow FinItem in finDt.Rows)
                                            {
                                                FinName = FinItem["ProductName"].ToString();
                                                FinCode = FinItem["ProductCode"].ToString();
                                            }
                                            DataTable rawDt = new DataTable();
                                            rawDt =
                                                productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                            foreach (DataRow RawItem in rawDt.Rows)
                                            {
                                                RawName = RawItem["ProductName"].ToString();
                                                RawCode = RawItem["ProductCode"].ToString();
                                            }

                                            throw new ArgumentNullException("ReceiveInsert",
                                                                            "Stock not Available for Finish Item( Name: " +
                                                                            FinName + " & Code: " + FinCode +
                                                                            " ) \n and consumtion Material ( Name: " +
                                                                            RawName + " & Code: " + RawCode + " )");
                                        }
                                    }

                                    #endregion Stock

                                    #region Find Quantity From Transaction

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
                                    sqlText += " UOMWastage,";
                                    sqlText += " BOMId,";
                                    sqlText += " Post";
                                    sqlText += " )";
                                    sqlText += " values( ";
                                    sqlText += "'" + newID + "',";
                                    sqlText += "@ItemReceiveLineNo,";
                                    sqlText += "@v1RawItemNo, ";
                                    sqlText += "@v1QuantityIssuePlaceQty ,";
                                    sqlText += "@AvgRate,";
                                    sqlText += "@v1CostPriceIssuePlaceAmt,";
                                    sqlText += "@v1UOM,";
                                    sqlText += " 0,0, ";
                                    sqlText += "@v1SubTotalIssuePlaceAmt,";
                                    sqlText += "@ItemCommentsD,";
                                    sqlText += "@MasterCreatedBy,";
                                    sqlText += "@MasterCreatedOn,";
                                    sqlText += "@MasterLastModifiedBy,";
                                    sqlText += "@MasterLastModifiedOn,";
                                    sqlText += "'" + newID + "',";
                                    sqlText += "@MasterReceiveDateTime,";
                                    sqlText += " 0,	0,";
                                    sqlText += "@v1Wastage,	";
                                    sqlText += "@v1BOMDate,	";
                                    sqlText += "@v1FinishItemNo,";
                                    sqlText += "@MasterTransactionType,";
                                    sqlText += "@MasterReturnId,";
                                    sqlText += "@v1UOMQtyIssuePlaceQty,";
                                    sqlText += "@v1UOMPriceIssuePlaceAmt,";
                                    sqlText += "@v1UOMc,";
                                    sqlText += "@v1UOMn,";
                                    sqlText += "@v1UOMWastage,";
                                    sqlText += "@v1BOMId,";
                                    sqlText += "'" + issueAutoPostValue + "'";
                                    sqlText += ")";
                                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                    cmdInsertIssue.Transaction = transaction;

                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1QuantityIssuePlaceQty", FormatingNumeric(v1Quantity, IssuePlaceQty));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPriceIssuePlaceAmt", FormatingNumeric(v1CostPrice, IssuePlaceAmt));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotalIssuePlaceAmt", FormatingNumeric(v1SubTotal, IssuePlaceAmt));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQtyIssuePlaceQty", FormatingNumeric(v1UOMQty, IssuePlaceQty));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPriceIssuePlaceAmt", FormatingNumeric(v1UOMPrice, IssuePlaceAmt));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);


                                    transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

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
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

                                    #endregion Update Issue

                                    #endregion Qty  check and Update

                                    #endregion Qty  check and Update
                                }
                            }

                            #endregion Find Raw Item From BOM and update Stock

                            #endregion TollFinishReceive

                        }


                        #endregion TollFinishReceive

                        #region PackageProduction

                        else if (Master.transactionType == "PackageProduction")
                        {
                            #region TollFinishReceive

                            BomId = string.Empty; //BOMId
                            BOMDate = DateTime.MinValue;

                            #region Last BOMId

                            sqlText = "  ";
                            sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname='VAT 1 (Package)' ";
                            sqlText += " and effectdate<=@receiveDate ";
                            sqlText += " and post='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID ";
                            }

                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomId1 = new SqlCommand(sqlText, currConn);
                            cmdBomId1.Transaction = transaction;

                            cmdBomId1.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomId1.Parameters.AddWithValueAndNullHandle("@receiveDate", receiveDate);
                            cmdBomId1.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            if (cmdBomId1.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BomId = "0";
                            }
                            else
                            {
                                BomId = (string)cmdBomId1.ExecuteScalar();
                            }

                            #endregion Last BOMId

                            #region Last BOMDate

                            sqlText = "  ";
                            sqlText += " select top 1 isnull(EffectDate,'1900/01/01') from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo  ";
                            sqlText += " and vatname='VAT 1 (Package)' ";
                            sqlText += " and effectdate<=@receiveDate";
                            sqlText += " and post='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID ";
                            }

                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomEDate1 = new SqlCommand(sqlText, currConn);
                            cmdBomEDate1.Transaction = transaction;

                            cmdBomEDate1.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomEDate1.Parameters.AddWithValueAndNullHandle("@receiveDate", receiveDate);
                            cmdBomEDate1.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            if (cmdBomEDate1.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BOMDate = DateTime.MinValue;
                                bomDate = BOMDate.Date.ToString("yyyy/MM/dd 00:00:00");
                            }
                            else
                            {
                                BOMDate = (DateTime)cmdBomEDate1.ExecuteScalar();
                                bomDate = BOMDate.Date.ToString("yyyy/MM/dd 00:00:00");
                            }

                            #endregion Last BOMDate

                            #region Find Raw Item From BOM  and update Stock
                            #region Finish

                            sqlText = "";
                            sqlText +=
                                " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty  from BOMRaws b  ";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname='VAT 1 (Package)' ";
                            //sqlText += " and effectdate='" + BOMDate + "'";
                            sqlText += " and effectdate=@bomDate ";
                            sqlText += " and post='Y' ";
                            sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID ";
                            }

                            sqlText += "   and (rawitemtype='finish') ";

                            DataTable dataTable = new DataTable("RIFB");
                            SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                            cmdRIFB.Transaction = transaction;

                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@bomDate", bomDate);
                            cmdRIFB.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                            reportDataAdapt.Fill(dataTable);

                            if (dataTable == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgNoDataToPost);
                            }
                            else if (dataTable.Rows.Count <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                "There is no price declaration for this item.");
                            }
                            else
                            {
                                #region Declare

                                decimal v1Quantity = 0;
                                string v1RawItemNo = "";
                                decimal v1CostPrice = 0;
                                string v1UOM = "";
                                decimal v1SubTotal = 0;
                                decimal v1UOMQty = 0;
                                decimal v1UOMPrice = 0;
                                decimal v1UOMc = 0;
                                string v1UOMn = "";
                                #endregion Declare

                                decimal vQuantity = 0;
                                decimal vWastage = 0;

                                string rwUom = "";
                                decimal vConvertionRate = 0;
                                decimal AvgRate = 0;

                                foreach (DataRow BRItem in dataTable.Rows)
                                {
                                    #region Update Item Qty

                                    #region Find Quantity From Products

                                    DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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

                                    decimal nbrPriceFromBom = productDal.GetLastNBRPriceFromBOM(BRItem["RawItemNo"].ToString(),
                                                                  "VAT 1", Master.ReceiveDateTime,
                                                                  null, null);

                                    vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                    vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());



                                    rwUom = BRItem["Uom"].ToString();
                                    var rwUomMajor = BRItem["Uomn"].ToString();
                                    if (string.IsNullOrEmpty(rwUom))
                                    {
                                        throw new ArgumentNullException("ReceiveInsert",
                                                                        "Could not find UOM of raw item");
                                    }

                                    /*Processing UOM*/

                                    UOMDAL uomdal = new UOMDAL();
                                    vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);

                                    #endregion Find Quantity From Products

                                    #region valueAssign

                                    v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                    v1RawItemNo = BRItem["RawItemNo"].ToString();
                                    v1UOM = BRItem["UOM"].ToString();
                                    v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                    v1UOMPrice = AvgRate;
                                    v1UOMn = BRItem["UOMn"].ToString();
                                    v1UOMc = vConvertionRate;
                                    v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;

                                    #endregion valueAssign

                                    #region Stock

                                    if (NegStockAllow == false)
                                    {
                                        decimal vStock = 0;
                                        var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                         Master.ReceiveDateTime,
                                       currConn, transaction, false).Rows[0]["Quantity"].ToString();

                                        vStock = Convert.ToDecimal(stock);


                                        if ((vStock - v1UOMQty) < 0)
                                        {
                                            string FinName = string.Empty;
                                            string FinCode = string.Empty;
                                            string RawName = string.Empty;
                                            string RawCode = string.Empty;
                                            DataTable finDt = new DataTable();
                                            finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                            foreach (DataRow FinItem in finDt.Rows)
                                            {
                                                FinName = FinItem["ProductName"].ToString();
                                                FinCode = FinItem["ProductCode"].ToString();
                                            }
                                            DataTable rawDt = new DataTable();
                                            rawDt =
                                                productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                            foreach (DataRow RawItem in rawDt.Rows)
                                            {
                                                RawName = RawItem["ProductName"].ToString();
                                                RawCode = RawItem["ProductCode"].ToString();
                                            }

                                            throw new ArgumentNullException("ReceiveInsert",
                                                                            "Stock not Available for Finish Item( Name: " +
                                                                            FinName + " & Code: " + FinCode +
                                                                            " ) \n and consumtion Material ( Name: " +
                                                                            RawName + " & Code: " + RawCode + " )");
                                        }
                                    }

                                    #endregion Stock

                                    #region Find Quantity From Transaction

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
                                    sqlText += " FinishItemNo,";
                                    sqlText += " UOMPrice";
                                    sqlText += " )";
                                    sqlText += " values(	";
                                    sqlText += "'" + newID + "',";//sqlText += "'" + newID + "',";
                                    sqlText += "@ItemReceiveLineNo, ";//sqlText += " '" + Item.InvoiceLineNo + "', ";
                                    sqlText += "@v1RawItemNo, ";//sqlText += " '" + Item.ItemNo + " ',";
                                    sqlText += "@v1Quantity, ";//sqlText += " '" + Item.Quantity + " ',";
                                    sqlText += " 0, ";//sqlText += " '" + Item.PromotionalQuantity + " ',";
                                    sqlText += "@nbrPriceFromBom, ";//sqlText += " '" + Item.SalesPrice + " ',";
                                    sqlText += "@nbrPriceFromBom, ";//sqlText += " '" + Item.NBRPrice + " ',";
                                    sqlText += "@AvgRate, ";//sqlText += " '" + AvgRate + " ',";
                                    sqlText += "@v1UOM, ";//sqlText += " '" + Item.UOM + " ',";
                                    sqlText += " '0', ";//sqlText += " '" + Item.VATRate + " ',";
                                    sqlText += " '0', ";//sqlText += " '" + Item.VATAmount + " ',";
                                    sqlText += "@v1SubTotal, ";//sqlText += " '" + Item.SubTotal + " ',";
                                    sqlText += "@ItemCommentsD, ";//sqlText += " '" + Item.CommentsD + " ',";
                                    sqlText += "@MasterCreatedBy, ";//sqlText += " '" + Master.CreatedBy + " ',";
                                    sqlText += "@MasterCreatedOn, ";//sqlText += " '" + Master.CreatedOn + " ',";
                                    sqlText += "@MasterLastModifiedBy, ";//sqlText += " '" + Master.LastModifiedBy + " ',";
                                    sqlText += "@MasterLastModifiedOn, ";//sqlText += " '" + Master.LastModifiedOn + " ',";
                                    sqlText += " '0', ";//sqlText += " '" + Item.SD + " ',";
                                    sqlText += " '0', ";//sqlText += " '" + Item.SDAmount + " ',";
                                    sqlText += " 'New', ";//sqlText += " '" + Item.SaleTypeD + " ',";
                                    sqlText += "@MasterReturnId, ";//sqlText += " '" + Item.PreviousSalesInvoiceNoD + " ',";
                                    sqlText += " 'N', ";//sqlText += " '" + Item.TradingD + " ',";
                                    sqlText += " 'N', ";//sqlText += " '" + Item.NonStockD + " ',";
                                    sqlText += " '0', ";//sqlText += " '" + Item.TradingMarkUp + " ',";
                                    sqlText += "@MasterReceiveDateTime, ";//sqlText += " '" + Master.InvoiceDateTime + " ',";
                                    sqlText += " 'VAT', ";//sqlText += " '" + Item.Type + " ',";
                                    sqlText += "@MasterTransactionType, ";//sqlText += " '" + Master.transactionType + " ',";
                                    sqlText += "@MasterReturnId, ";//sqlText += " '" + Master.ReturnId + " ',";
                                    sqlText += "@MasterPost, ";//sqlText += " '" + Master.Post + " ',";
                                    sqlText += "@v1UOMQty, ";//sqlText += " '" + Item.UOMQty + " ',";
                                    sqlText += "@v1UOMn, ";//sqlText += " '" + Item.UOMn + " ',";
                                    sqlText += "@v1UOMc, ";//sqlText += " '" + Item.UOMc + " ',";
                                    sqlText += " '0', ";//sqlText += "'" + Item.DiscountAmount + "',";
                                    sqlText += "@nbrPriceFromBom, ";//sqlText += "'" + Item.DiscountedNBRPrice + "',";
                                    sqlText += " '0',";//sqlText += "'" + Item.DollerValue + "',";
                                    sqlText += "@nbrPriceFromBom, ";
                                    sqlText += "@ItemItemNo, ";
                                    sqlText += "@v1UOMPrice ";//sqlText += " '" + Item.UOMPrice + "' ";
                                    sqlText += ")	";
                                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                    cmdInsertIssue.Transaction = transaction;

                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@nbrPriceFromBom", nbrPriceFromBom);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@nbrPriceFromBom", nbrPriceFromBom);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@nbrPriceFromBom", nbrPriceFromBom);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@nbrPriceFromBom", nbrPriceFromBom);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);

                                    transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

                                    #region Update Issue

                                    sqlText = "";
                                    sqlText += " update SalesInvoiceHeaders set ";
                                    sqlText += " TotalAmount= (select sum(Quantity*NBRPrice) from SalesInvoiceDetails";
                                    sqlText += "  where SalesInvoiceDetails.SalesInvoiceNo =SalesInvoiceHeaders.SalesInvoiceNo)";
                                    sqlText += " where (SalesInvoiceHeaders.SalesInvoiceNo= '" + newID + "')";

                                    SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                    cmdUpdateIssue.Transaction = transaction;
                                    transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

                                    #endregion Update Issue

                                    #endregion Qty  check and Update

                                    #endregion Qty  check and Update
                                }
                            }
                            #endregion Finish

                            #region Insert into Issue

                            sqlText = "";


                            sqlText = "";
                            sqlText +=
                                " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty  from BOMRaws b  ";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname='VAT 1 (Package)' ";
                            sqlText += " and effectdate=@bomDate";
                            sqlText += " and post='Y' ";
                            sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID ";
                            }

                            sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='Trading' ) ";


                            DataTable dataTableR = new DataTable("RIFB");
                            SqlCommand cmdRIF = new SqlCommand(sqlText, currConn);
                            cmdRIF.Transaction = transaction;

                            cmdRIF.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdRIF.Parameters.AddWithValueAndNullHandle("@bomDate", bomDate);
                            cmdRIF.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            SqlDataAdapter reportDataAdapt1 = new SqlDataAdapter(cmdRIF);
                            reportDataAdapt1.Fill(dataTableR);

                            if (dataTableR == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgNoDataToPost);
                            }
                            else if (dataTableR.Rows.Count <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                "There is no price declaration for this item.");
                            }
                            else
                            {
                                decimal vQuantity = 0;
                                decimal vWastage = 0;
                                string rwUom = "";
                                decimal vConvertionRate = 0;
                                decimal AvgRate = 0;

                                foreach (DataRow BRItem in dataTableR.Rows)
                                {
                                    #region Update Item Qty

                                    #region Find Quantity From Products

                                    DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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


                                    vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                    vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                    #region Issue Settings
                                    //CommonDAL commDal = new CommonDAL();
                                    //int IssuePlaceQty = Convert.ToInt32(commDal.settings("Issue", "Quantity"));
                                    //int IssuePlaceAmt = Convert.ToInt32(commDal.settings("Issue", "Amount"));
                                    AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                    vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                    vWastage = FormatingNumeric(vWastage, IssuePlaceQty);
                                    #endregion Issue Settings

                                    rwUom = BRItem["Uom"].ToString();
                                    var rwUomMajor = BRItem["Uomn"].ToString();
                                    if (string.IsNullOrEmpty(rwUom))
                                    {
                                        throw new ArgumentNullException("ReceiveInsert",
                                                                        "Could not find UOM of raw item");
                                    }

                                    /*Processing UOM*/

                                    UOMDAL uomdal = new UOMDAL();
                                    vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);


                                    #endregion Find Quantity From Products

                                    #region valueAssign
                                    #region Declare

                                    decimal v1Quantity = 0;
                                    string v1RawItemNo = "";
                                    decimal v1CostPrice = 0;
                                    string v1UOM = "";
                                    decimal v1SubTotal = 0;
                                    decimal v1Wastage = 0;
                                    DateTime v1BOMDate = DateTime.Now.Date;
                                    string v1FinishItemNo = "";

                                    decimal v1UOMQty = 0;
                                    decimal v1UOMPrice = 0;
                                    decimal v1UOMc = 0;
                                    string v1UOMn = "";
                                    string v1BOMId = "";
                                    decimal v1UOMWastage = 0;

                                    #endregion Declare

                                    v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                    v1Wastage = (vWastage) * Item.UOMQty;
                                    v1BOMId = Item.BOMId;
                                    v1RawItemNo = BRItem["RawItemNo"].ToString();
                                    v1UOM = BRItem["UOM"].ToString();
                                    v1CostPrice = AvgRate * vConvertionRate;
                                    v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                    v1UOMPrice = AvgRate;
                                    v1UOMn = BRItem["UOMn"].ToString();
                                    v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                    v1FinishItemNo = Item.ItemNo;
                                    v1UOMc = vConvertionRate;
                                    v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                    v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;

                                    #endregion valueAssign

                                    #region Stock

                                    if (NegStockAllow == false)
                                    {
                                        decimal vStock = 0;

                                        var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                                              Master.ReceiveDateTime,
                                                            currConn, transaction, false).Rows[0]["Quantity"].ToString();

                                        vStock = Convert.ToDecimal(stock);


                                        if ((vStock - v1UOMQty) < 0)
                                        {
                                            string FinName = string.Empty;
                                            string FinCode = string.Empty;
                                            string RawName = string.Empty;
                                            string RawCode = string.Empty;
                                            DataTable finDt = new DataTable();
                                            finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                            foreach (DataRow FinItem in finDt.Rows)
                                            {
                                                FinName = FinItem["ProductName"].ToString();
                                                FinCode = FinItem["ProductCode"].ToString();
                                            }
                                            DataTable rawDt = new DataTable();
                                            rawDt =
                                                productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                            foreach (DataRow RawItem in rawDt.Rows)
                                            {
                                                RawName = RawItem["ProductName"].ToString();
                                                RawCode = RawItem["ProductCode"].ToString();
                                            }

                                            throw new ArgumentNullException("ReceiveInsert",
                                                                            "Stock not Available for Finish Item( Name: " +
                                                                            FinName + " & Code: " + FinCode +
                                                                            " ) \n and consumtion Material ( Name: " +
                                                                            RawName + " & Code: " + RawCode + " )");
                                        }
                                    }

                                    #endregion Stock
                                    #region Find Quantity From Transaction

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
                                    sqlText += " UOMWastage,";
                                    sqlText += " BOMId,";
                                    sqlText += " Post";
                                    sqlText += " )";
                                    sqlText += " values( ";
                                    sqlText += "'" + newID + "',";
                                    sqlText += "@ItemReceiveLineNo,";
                                    sqlText += "@v1RawItemNo, ";
                                    sqlText += "@v1QuantityIssuePlaceQty ,";
                                    sqlText += "@AvgRate,";
                                    sqlText += "@v1CostPriceIssuePlaceAmt,";
                                    sqlText += "@v1UOM,";
                                    sqlText += " 0,0, ";
                                    sqlText += "@v1SubTotalIssuePlaceAmt,";
                                    sqlText += "@ItemCommentsD,";
                                    sqlText += "@MasterCreatedBy,";
                                    sqlText += "@MasterCreatedOn,";
                                    sqlText += "@MasterLastModifiedBy,";
                                    sqlText += "@MasterLastModifiedOn,";
                                    sqlText += "@newID,";
                                    sqlText += "@MasterReceiveDateTime,";
                                    sqlText += " 0,	0,";
                                    sqlText += "@v1Wastage,	";
                                    sqlText += "@v1BOMDate,	";
                                    sqlText += "@v1FinishItemNo,";
                                    sqlText += "@MasterTransactionType,";
                                    sqlText += "@MasterReturnId,";
                                    sqlText += "@v1UOMQtyIssuePlaceQty,";
                                    sqlText += "@v1UOMPriceIssuePlaceAmt,";
                                    sqlText += "@v1UOMc,";
                                    sqlText += "@v1UOMn,";
                                    sqlText += "@v1UOMWastage,";
                                    sqlText += "@v1BOMId,";
                                    sqlText += "'" + issueAutoPostValue + "'";
                                    sqlText += ")";
                                    SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                    cmdInsertIssue.Transaction = transaction;

                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1QuantityIssuePlaceQty", FormatingNumeric(v1Quantity, IssuePlaceQty));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPriceIssuePlaceAmt", FormatingNumeric(v1CostPrice, IssuePlaceAmt));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotalIssuePlaceAmt", FormatingNumeric(v1SubTotal, IssuePlaceAmt));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@newID", newID);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQtyIssuePlaceQty", FormatingNumeric(v1UOMQty, IssuePlaceQty));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPriceIssuePlaceAmt", FormatingNumeric(v1UOMPrice, IssuePlaceAmt));
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                    cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);

                                    transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

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
                                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                        MessageVM.receiveMsgUnableToSaveIssue);
                                    }

                                    #endregion Update Issue

                                    #endregion Qty  check and Update

                                    #endregion Qty  check and Update
                                }
                            }
                            #endregion Insert into Issue

                            #endregion Find Raw Item From BOM and update Stock

                            #endregion PackageProduction

                        }


                        #endregion TollFinishReceive
                    }//if (Master.FromBOM == "Y")

                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

                #region Tracking
                if (Trackings != null && Trackings.Count > 0)
                {
                    for (int i = 0; i < Trackings.Count; i++)
                    {
                        if (Master.transactionType == "ReceiveReturn")
                        {
                            if (Trackings[i].ReturnReceive == "Y")
                            {
                                Trackings[i].ReturnReceiveID = newID;
                                Trackings[i].ReturnType = Master.transactionType;
                            }

                        }
                        else if (Trackings[i].IsReceive == "Y")
                        {
                            Trackings[i].ReceiveNo = newID;
                            Trackings[i].ReceiveDate = Master.ReceiveDateTime;
                            //Trackings[i].Post = Master.Post;
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
                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.ReceiveHeaders WHERE ReceiveNo='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = cmdIPS.ExecuteScalar().ToString();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgUnableCreatID);
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
                retResults[1] = MessageVM.receiveMsgSaveSuccessfully;
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
        public string[] ReceiveUpdate(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings)
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
            bool NegStockAllow = false;
            DateTime previousReceiveDate = DateTime.MinValue;
            DateTime receiveDate = DateTime.MinValue;
            DateTime BOMDate = DateTime.MinValue;  //start
            string BomId = string.Empty;

            string newID = "";
            string PostStatus = "";
            bool issueAutoPost = false;
            string issueAutoPostValue = "N";
            int nextId = 0;
            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.ReceiveDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.ReceiveDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, "Please Check Invoice Data and Time");

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

                transaction = currConn.BeginTransaction(MessageVM.receiveMsgMethodNameInsert);
                #endregion open connection and transaction
                #region Master

                string vissueAutoPost, vNegStockAllow = string.Empty;

                vissueAutoPost = commonDal.settings("IssueFromBOM", "IssueAutoPost");
                vNegStockAllow = commonDal.settings("Issue", "NegStockAllow");
                if (
                    string.IsNullOrEmpty(vissueAutoPost)
                   || string.IsNullOrEmpty(vNegStockAllow)
                    )
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Receive");
                }
                issueAutoPost = Convert.ToBoolean(vissueAutoPost == "Y" ? true : false);
                NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);

                if (issueAutoPost)
                    issueAutoPostValue = "Y";

                #region Fiscal Year Check

                string transactionDate = Master.ReceiveDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.ReceiveDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(ReceiveNo) from ReceiveHeaders WHERE ReceiveNo=@MasterReceiveNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;

                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);


                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUnableFindExistID);
                }

                #endregion Find ID for Update


                if (Master.transactionType == "ReceiveReturn")
                {
                    #region Find Receive Return Date

                    sqlText = "";
                    sqlText = sqlText + "select ReceiveDateTime from ReceiveHeaders" +
                              " WHERE ReceiveNo=@MasterReturnId ";
                    SqlCommand cmdFindPDate = new SqlCommand(sqlText, currConn);
                    cmdFindPDate.Transaction = transaction;
                    cmdFindPDate.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);

                    previousReceiveDate = (DateTime)cmdFindPDate.ExecuteScalar();

                    if (previousReceiveDate == null)
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                        MessageVM.receiveMsgFindExistID);
                    }

                    #endregion  Find Receive Return Date
                }

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update ReceiveHeaders set  ";
                sqlText += " ReceiveDateTime=@MasterReceiveDateTime ";
                sqlText += " ,WithToll=@MasterWithToll";
                sqlText += " ,TotalAmount=@MasterTotalAmount";
                sqlText += " ,SerialNo=@MasterSerialNo";
                sqlText += " ,Comments=@MasterComments";
                sqlText += " ,LastModifiedBy=@MasterLastModifiedBy";
                sqlText += " ,LastModifiedOn=@MasterLastModifiedOn";
                sqlText += " ,transactionType=@MasterTransactionType";
                sqlText += " ,ReceiveReturnId=@MasterReturnId";
                sqlText += " ,ReferenceNo=@MasterReferenceNo";
                sqlText += " ,CustomerID=@MasterCustomerID";
                sqlText += " ,Post=@MasterPost";
                sqlText += " ,ShiftId=@ShiftId";
                sqlText += " where ReceiveNo=@MasterReceiveNo ";

                
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterWithToll", Master.WithToll);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterReferenceNo", Master.ReferenceNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ShiftId", Master.ShiftId);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUpdateNotSuccessfully);
                }
                #endregion update Header

                #region Transaction Not Other

                #region Transaction is FromBOM
                if (Master.FromBOM == "Y")
                {
                    if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn"
                        || Master.transactionType == "WIP" || Master.transactionType == "TollFinishReceive"
                        || Master.transactionType == "Tender")
                    {
                        #region update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " IssueDateTime  =@MasterReceiveDateTime,";
                        sqlText += " Comments       =@MasterComments ,";
                        sqlText += " SerialNo       =@MasterSerialNo ,";
                        sqlText += " LastModifiedBy =@MasterLastModifiedBy ,";
                        sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                        sqlText += " transactionType=@MasterTransactionType ,";
                        sqlText += " IssueReturnId  =@MasterReturnId ,";
                        sqlText += " Post           =@issueAutoPostValue ";
                        sqlText += " where  IssueNo =@MasterReceiveNo ";


                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;

                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                            MessageVM.receiveMsgUpdateNotSuccessfully);
                        }

                        #endregion update Issue

                    }

                    if (Master.transactionType == "PackageProduction")
                    {
                        #region update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " IssueDateTime  =@MasterReceiveDateTime,";
                        sqlText += " Comments       =@MasterComments ,";
                        sqlText += " SerialNo       =@MasterSerialNo ,";
                        sqlText += " LastModifiedBy =@MasterLastModifiedBy ,";
                        sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                        sqlText += " transactionType=@MasterTransactionType ,";
                        sqlText += " IssueReturnId  =@MasterReturnId ,";
                        sqlText += " Post           =@issueAutoPostValue ";
                        sqlText += " where  IssueNo =@MasterReceiveNo ";


                        SqlCommand cmdUpdateIssue1 = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue1.Transaction = transaction;

                        cmdUpdateIssue1.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdUpdateIssue1.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                        cmdUpdateIssue1.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo);
                        cmdUpdateIssue1.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdUpdateIssue1.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdUpdateIssue1.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                        cmdUpdateIssue1.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                        cmdUpdateIssue1.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue);
                        cmdUpdateIssue1.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                        transResult = (int)cmdUpdateIssue1.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                            MessageVM.receiveMsgUpdateNotSuccessfully);
                        }

                        #endregion update Issue
                        #region update Sale

                        sqlText = "";

                        sqlText += " update SalesInvoiceHeaders set  ";
                        sqlText += " CustomerID= '0' ,";
                        sqlText += " DeliveryAddress1= 'NA' ,";
                        sqlText += " DeliveryAddress2= 'NA' ,";
                        sqlText += " DeliveryAddress3= 'NA' ,";
                        sqlText += " VehicleID= '0' ,";
                        sqlText += " InvoiceDateTime        = @MasterReceiveDateTime ,";
                        sqlText += " SerialNo               = @MasterSerialNo ,";
                        sqlText += " Comments               = @MasterComments ,";
                        sqlText += " DeliveryDate           = @MasterReceiveDateTime ,";
                        sqlText += " LastModifiedBy         = @MasterLastModifiedBy ,";
                        sqlText += " LastModifiedOn         = @MasterLastModifiedOn ,";
                        sqlText += " SaleType= 'New' ,";
                        sqlText += " PreviousSalesInvoiceNo = @MasterReturnId ,";
                        sqlText += " Trading= 'N' ,";
                        sqlText += " IsPrint= 'N' ,";
                        sqlText += " TenderId= 'N' ,";
                        sqlText += " TransactionType        = @MasterTransactionType ,";
                        sqlText += " SaleReturnId           = @MasterReturnId ,";
                        sqlText += " CurrencyID= '260' ,";
                        sqlText += " CurrencyRateFromBDT= '1' ,";
                        sqlText += " Post                   = @MasterPost ";
                        sqlText += " where  SalesInvoiceNo  = @MasterReceiveNo ";


                        SqlCommand cmdUpdateSale = new SqlCommand(sqlText, currConn);
                        cmdUpdateSale.Transaction = transaction;

                        cmdUpdateSale.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdUpdateSale.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo);
                        cmdUpdateSale.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                        cmdUpdateSale.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdUpdateSale.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdUpdateSale.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdUpdateSale.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                        cmdUpdateSale.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                        cmdUpdateSale.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                        cmdUpdateSale.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdUpdateSale.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                        transResult = (int)cmdUpdateSale.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                            MessageVM.receiveMsgUpdateNotSuccessfully);
                        }

                        #endregion update Sale

                    }


                }

                #endregion Transaction is TollReceive

                #endregion Transaction Not Other


                #endregion ID check completed,update Information in Header

                #endregion Master
                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table
                var lineNo = 1;
                foreach (var Item in Details.ToList())
                {
                    Item.ReceiveLineNo = lineNo.ToString();
                    lineNo++;
                    ProductDAL productDal = new ProductDAL();

                    #region Find Transaction Mode Insert or Update

                    IDExist = 0;
                    sqlText = "";
                    sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo=@MasterReceiveNo ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);


                    IDExist = (int)cmdFindId.ExecuteScalar();

                    string receiveFormatDate = "";
                    if (Master.transactionType == "ReceiveReturn")
                    {
                        receiveDate = previousReceiveDate.Date;
                        receiveFormatDate = receiveDate.ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        receiveDate = Convert.ToDateTime(Master.ReceiveDateTime).Date;
                        receiveFormatDate = receiveDate.ToString("yyyy/MM/dd");
                    }

                    #region USD calculate

                    string[] usdResults = GetUSDCurrency(Item.SubTotal);
                    #endregion USD calculate

                    if (IDExist <= 0)
                    {
                        #region Insert only DetailTable

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
                        sqlText += " transactionType,";
                        sqlText += " ReceiveReturnId,";
                        sqlText += " BOMId,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMQty,";
                        sqlText += " UOMn,";
                        sqlText += " UOMc,";
                        sqlText += " VATName,";
                        sqlText += " CurrencyValue,";
                        sqlText += " DollerValue,";

                        sqlText += " Post";
                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@MasterReceiveNo,";
                        sqlText += "@ItemReceiveLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += "@ItemCostPrice,";
                        sqlText += "@ItemNBRPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += "@ItemVATRate,";
                        sqlText += "@ItemVATAmount,";
                        sqlText += "@ItemSubTotal,";
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@ItemSD,";
                        sqlText += "@ItemSDAmount,";
                        sqlText += "@MasterReceiveDateTime,";
                        sqlText += "@MasterTransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@ItemBOMId,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemVatName,";
                        sqlText += "'" + usdResults[0] + "',";//CurrenyValue
                        sqlText += "'" + usdResults[1] + "',";//DollerValue


                        sqlText += "@MasterPost";
                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCostPrice", Item.CostPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemNBRPrice", Item.NBRPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATAmount", Item.VATAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSD", Item.SD);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSDAmount", Item.SDAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBOMId", Item.BOMId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMn", Item.UOMn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgSaveNotSuccessfully);
                        }
                        #endregion Insert only DetailTable
                        #region if Transection not Other Insert Issue /Receive

                        #region TollReceive
                        if (Master.transactionType == "ReceiveReturn" && Item.ReturnTransactionType == "TollReceive")
                        {
                            //ProductDAL productDal = new ProductDAL();
                            DataTable TollItemInfo = productDal.SearchRawItemNo(Master.ReturnId);

                            string TollItem;
                            decimal TollUnitCost = 0;

                            TollItem = TollItemInfo.Rows[0]["ItemNo"].ToString();
                            TollUnitCost = Convert.ToDecimal(TollItemInfo.Rows[0]["CostPrice"].ToString());

                            #region Insert to Issue Header

                            //sqlText = "";
                            //sqlText += " insert into IssueHeaders(";
                            ////sqlText += " IssueNo,";

                            //sqlText += " IssueNo,";
                            //sqlText += " IssueDateTime,";
                            //sqlText += " TotalVATAmount,";
                            //sqlText += " TotalAmount,";
                            //sqlText += " SerialNo,";
                            //sqlText += " Comments,";
                            //sqlText += " CreatedBy,";
                            //sqlText += " CreatedOn,";
                            //sqlText += " LastModifiedBy,";
                            //sqlText += " LastModifiedOn,";
                            //sqlText += " ReceiveNo,";
                            //sqlText += " transactionType,";
                            //sqlText += " IssueReturnId,";
                            //sqlText += " Post";

                            ////sqlText += " Post";
                            //sqlText += " )";
                            //sqlText += " values(	";
                            ////sqlText += "'" + Master.Id + "',";

                            //sqlText += "'" + newID + "',";
                            //sqlText += "'" + Master.ReceiveDateTime + "',";
                            //sqlText += " 0 ,";
                            //sqlText += "0,";
                            //sqlText += "'" + Item.ReceiveLineNo + "',";
                            //sqlText += "'" + Master.Comments + "',";
                            //sqlText += "'" + Master.CreatedBy + "',";
                            //sqlText += "'" + Master.CreatedOn + "',";
                            //sqlText += "'" + Master.LastModifiedBy + "',";
                            //sqlText += "'" + Master.LastModifiedOn + "',";
                            //sqlText += "'" + newID + "',";
                            //sqlText += "'TollReceiveReturn',";
                            //sqlText += "'" + Master.ReturnId + "',";
                            //sqlText += "'" + Master.Post + "'";

                            ////sqlText += "'" + Master.@Post + "'";
                            //sqlText += ")	";

                            //SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            //cmdInsertIssue.Transaction = transaction;
                            //transResult = (int)cmdInsertIssue.ExecuteNonQuery();
                            //if (transResult <= 0)
                            //{

                            //    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                            //                                    MessageVM.PurchasemsgUnableToSaveIssue);
                            //}

                            #endregion Insert to Issue Header

                            #region Insert to Issue 16 out

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

                            sqlText += " UOMQty,";
                            sqlText += " UOMPrice,";
                            sqlText += " UOMc,";
                            sqlText += " UOMn,";
                            sqlText += " UOMWastage,";


                            sqlText += " Post";

                            sqlText += " )";
                            sqlText += " values(	";
                            sqlText += "'" + newID + "',";
                            sqlText += "'00',";
                            sqlText += "@TollItem,";
                            sqlText += "@ItemQuantity,";
                            sqlText += " 0,"; // NBR
                            sqlText += "@TollUnitCost,";
                            sqlText += "@ItemUOM,";
                            sqlText += " 0,	";
                            sqlText += " 0,";
                            sqlText += "@TollUnitCostItemUOMQty,"; // sub total
                            sqlText += "@MasterCreatedBy,";
                            sqlText += "@MasterCreatedOn,";
                            sqlText += "@MasterLastModifiedBy,";
                            sqlText += "@MasterLastModifiedOn,";
                            sqlText += "'" + newID + "',";
                            sqlText += "@MasterReceiveDateTime,";
                            sqlText += " 0,";
                            sqlText += " 0,";
                            sqlText += " 0,	";
                            sqlText += " 0,	";
                            sqlText += " 0,";
                            sqlText += "'TollReceiveReturn',";
                            sqlText += "@MasterReturnId,";
                            sqlText += "@ItemUOMQty,";
                            sqlText += "@ItemUOMPrice,";
                            sqlText += "@ItemUOMc,";
                            sqlText += "@ItemUOMn,";
                            sqlText += "'0',";
                            sqlText += "@MasterPost";

                            //sqlText += "'" + Master.@Post + "'";
                            sqlText += ")	";
                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@TollItem", TollItem);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@TollUnitCost", TollUnitCost);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@TollUnitCostItemUOMQty", TollUnitCost * Item.UOMQty);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMQty", Item.UOMQty);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMPrice", Item.UOMPrice);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMc", Item.UOMc);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMn", Item.UOMn);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);


                            transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.PurchasemsgUnableToSaveIssue);
                            }

                            #endregion Insert to Issue


                        }
                        #endregion TollReceive


                        #region Purchase For TollReceive
                        #region From BOM

                        if (Master.FromBOM == "Y")
                        {
                            BomId = string.Empty;
                            BOMDate = DateTime.MinValue;

                            #region Last BOMId

                            sqlText = "  ";
                            sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo   =@ItemItemNo ";
                            sqlText += " and vatname    =@ItemVatName ";
                            sqlText += " and effectdate<=@receiveDateDate";
                            sqlText += " and post='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID ";
                            }

                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomId2 = new SqlCommand(sqlText, currConn);
                            cmdBomId2.Transaction = transaction;

                            cmdBomId2.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomId2.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdBomId2.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                            cmdBomId2.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            if (cmdBomId2.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BomId = "0";
                            }
                            else
                            {
                                BomId = (string)cmdBomId2.ExecuteScalar();
                            }

                            #endregion Last BOMId

                            #region Last BOMDate

                            sqlText = "  ";
                            sqlText += " select top 1 isnull(EffectDate,'1900/01/01') from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname=@ItemVatName ";
                            sqlText += " and effectdate<@receiveDate";
                            sqlText += " and post='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID ";
                            }

                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomEDate2 = new SqlCommand(sqlText, currConn);
                            cmdBomEDate2.Transaction = transaction;

                            cmdBomEDate2.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomEDate2.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdBomEDate2.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                            cmdBomEDate2.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            if (cmdBomEDate2.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BOMDate = DateTime.MinValue;
                            }
                            else
                            {
                                BOMDate = (DateTime)cmdBomEDate2.ExecuteScalar();
                            }

                            #endregion Last BOMDate


                            if (Master.transactionType == "Other"
                                || Master.transactionType == "ReceiveReturn"
                                || Master.transactionType == "WIP")
                            {
                                #region Find Raw Item From BOM  and update Stock
                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.Uom,UOMUQty,UOMWQty,b.TransactionType from BOMRaws b  ";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and vatname=@ItemVatName ";
                                sqlText += " and effectdate=@BOMDate";
                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";

                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID  ";
                                }

                                sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='finsih' or rawitemtype='wip') ";

                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;

                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@BOMDate", BOMDate);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {
                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;
                                    string rwUom = "";
                                    decimal vConvertionRate = 0;
                                    decimal AvgRate = 0;

                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {
                                        #region Declare
                                        decimal v1Quantity = 0;
                                        string v1RawItemNo = "";
                                        decimal v1CostPrice = 0;
                                        string v1UOM = "";
                                        decimal v1SubTotal = 0;
                                        decimal v1Wastage = 0;
                                        DateTime v1BOMDate = DateTime.Now.Date;
                                        string v1FinishItemNo = "";

                                        decimal v1UOMQty = 0;
                                        decimal v1UOMPrice = 0;
                                        decimal v1UOMc = 0;
                                        string v1UOMn = "";
                                        string v1BOMId = "";
                                        decimal v1UOMWastage = 0;
                                        string vTransactionType = "";
                                        #endregion DECLARE

                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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


                                        //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                        //vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());

                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                        vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                        vWastage = FormatingNumeric(vWastage, IssuePlaceQty);


                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }

                                        /*Processing UOM*/

                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);

                                        #endregion Find Quantity From Products

                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                        v1Wastage = (vWastage) * Item.UOMQty;
                                        v1BOMId = BomId;
                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1CostPrice = AvgRate * vConvertionRate;
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                        v1FinishItemNo = Item.ItemNo;
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                        vTransactionType = BRItem["TransactionTtpe"].ToString();
                                        #endregion valueAssign

                                        #region Stock
                                        if (NegStockAllow == false)
                                        {
                                            decimal vStock = 0;
                                            //var stock = productDal.StockInHand(v1RawItemNo,
                                            //   Master.ReceiveDateTime + DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction).ToString();
                                            var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                                             Master.ReceiveDateTime +
                                                                             DateTime.Now.ToString(" HH:mm:ss"),
                                                           currConn, transaction, false).Rows[0]["Quantity"].ToString();

                                            vStock = Convert.ToDecimal(stock);


                                            if ((vStock - v1UOMQty) < 0)
                                            {
                                                string FinName = string.Empty;
                                                string FinCode = string.Empty;
                                                string RawName = string.Empty;
                                                string RawCode = string.Empty;
                                                DataTable finDt = new DataTable();
                                                finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                                foreach (DataRow FinItem in finDt.Rows)
                                                {
                                                    FinName = FinItem["ProductName"].ToString();
                                                    FinCode = FinItem["ProductCode"].ToString();
                                                }
                                                DataTable rawDt = new DataTable();
                                                rawDt = productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                                foreach (DataRow RawItem in rawDt.Rows)
                                                {
                                                    RawName = RawItem["ProductName"].ToString();
                                                    RawCode = RawItem["ProductCode"].ToString();
                                                }

                                                throw new ArgumentNullException("ReceiveInsert", "Stock not Available for Finish Item( Name: " + FinName + " & Code: " + FinCode + " ) \n and consumtion Material ( Name: " + RawName + " & Code: " + RawCode + " )");
                                            }
                                        }
                                        #endregion Stock
                                        #region Find Quantity From Transaction

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
                                        sqlText += " UOMWastage,";
                                        sqlText += " BOMId,";
                                        sqlText += " Post";
                                        sqlText += " )";
                                        sqlText += " values( ";
                                        sqlText += "@MasterReceiveNo,";
                                        sqlText += "@ItemReceiveLineNo,";
                                        sqlText += "@v1RawItemNo, ";
                                        sqlText += "@v1QuantityIssuePlaceQty,";
                                        sqlText += "@AvgRateIssuePlaceAmt,";
                                        sqlText += "@v1CostPrice,";
                                        sqlText += "@v1UOM,";
                                        sqlText += " 0,0, ";
                                        sqlText += "@v1SubTotalIssuePlaceAmt,";
                                        sqlText += "@ItemCommentsD,";
                                        sqlText += "@MasterCreatedBy,";
                                        sqlText += "@MasterCreatedOn,";
                                        sqlText += "@MasterLastModifiedBy,";
                                        sqlText += "@MasterLastModifiedOn,";
                                        sqlText += "@newID,";
                                        sqlText += "@MasterReceiveDateTime,";
                                        sqlText += " 0,	0,";
                                        sqlText += "@v1Wastage,	";
                                        sqlText += "@v1BOMDate,	";
                                        sqlText += "@v1FinishItemNo,";
                                        if (vTransactionType.Trim() == "TollReceiveRaw")
                                        {
                                            sqlText += "@vTransactionTypeTrim,";
                                        }
                                        else
                                        {
                                            if (Master.WithToll == "Y")
                                            {
                                                sqlText += "@MasterTransactionType" + "Toll" + "',";
                                            }
                                            else
                                            {
                                                sqlText += "@MasterTransactionType,";
                                            }
                                        }
                                        sqlText += "@MasterReturnId,";
                                        sqlText += "@FormatingNumeric(v1UOMQty, IssuePlaceQty),";
                                        sqlText += "@FormatingNumeric(v1UOMPrice, IssuePlaceAmt),";
                                        sqlText += "@v1UOMc,";
                                        sqlText += "@v1UOMn,";
                                        sqlText += "@v1UOMWastage,";
                                        sqlText += "@v1BOMId,";

                                        sqlText += "'" + issueAutoPostValue + "'";
                                        sqlText += ")";
                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;

                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1QuantityIssuePlaceQty", FormatingNumeric(v1Quantity, IssuePlaceQty));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRateIssuePlaceAmt", FormatingNumeric(AvgRate, IssuePlaceAmt));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotalIssuePlaceAmt", FormatingNumeric(v1SubTotal, IssuePlaceAmt));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@newID", newID);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQtyIssuePlaceQty", FormatingNumeric(v1UOMQty, IssuePlaceQty));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPriceIssuePlaceAmt", FormatingNumeric(v1UOMPrice, IssuePlaceAmt));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);

                                        if (vTransactionType.Trim() == "TollReceiveRaw")
                                        {
                                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@vTransactionTypeTrim", vTransactionType.Trim());
                                        }
                                        else
                                        {
                                            if (Master.WithToll == "Y")
                                            {
                                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                            }
                                            else
                                            {
                                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                            }
                                        }
                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #region Update Issue

                                        sqlText = "";
                                        sqlText += " update IssueHeaders set ";
                                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                        sqlText += " where (IssueHeaders.IssueNo= @MasterReceiveNo)";

                                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                        cmdUpdateIssue.Transaction = transaction;

                                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);


                                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #endregion Update Issue

                                        #endregion Qty  check and Update

                                        #endregion Qty  check and Update
                                    }
                                }

                                #endregion Insert Issue

                            }
                            else if (Master.transactionType == "Tender")
                            {

                                #region Find Raw Item From BOM  and update Stock

                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,UOMUQty,UOMWQty from BOMRaws b  ";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and vatname=@ItemVatName ";
                                sqlText += " and effectdate=@BOMDate ";
                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";

                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID ";
                                }
                                sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='finsih') ";

                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;

                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@BOMDate", BOMDate);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {

                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;

                                    string rwUom = "";
                                    decimal vConvertionRate = 0;
                                    decimal AvgRate = 0;

                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {
                                        #region Declare
                                        decimal v1Quantity = 0;
                                        string v1RawItemNo = "";
                                        decimal v1CostPrice = 0;
                                        string v1UOM = "";
                                        decimal v1SubTotal = 0;
                                        decimal v1Wastage = 0;
                                        DateTime v1BOMDate = DateTime.Now.Date;
                                        string v1FinishItemNo = "";

                                        decimal v1UOMQty = 0;
                                        decimal v1UOMPrice = 0;
                                        decimal v1UOMc = 0;
                                        string v1UOMn = "";
                                        string v1BOMId = "";
                                        decimal v1UOMWastage = 0;
                                        #endregion DECLARE

                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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

                                        //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                        //vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());

                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                        vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                        vWastage = FormatingNumeric(vWastage, IssuePlaceQty);

                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }


                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);


                                        #endregion Find Quantity From Products
                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                        v1Wastage = (vWastage) * Item.UOMQty;
                                        v1BOMId = BomId;
                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1CostPrice = AvgRate * vConvertionRate;
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                        v1FinishItemNo = Item.ItemNo;
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                        #endregion valueAssign

                                        #region Stock
                                        if (NegStockAllow == false)
                                        {
                                            decimal vStock = 0;
                                            //var stock = productDal.StockInHand(v1RawItemNo,
                                            //    Master.ReceiveDateTime + DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction).ToString();
                                            var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                                             Master.ReceiveDateTime +
                                                                             DateTime.Now.ToString(" HH:mm:ss"),
                                                           currConn, transaction, false).Rows[0]["Quantity"].ToString();
                                            vStock = Convert.ToDecimal(stock);


                                            if ((vStock - v1UOMQty) < 0)
                                            {
                                                string FinName = string.Empty;
                                                string FinCode = string.Empty;
                                                string RawName = string.Empty;
                                                string RawCode = string.Empty;
                                                DataTable finDt = new DataTable();
                                                finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                                foreach (DataRow FinItem in finDt.Rows)
                                                {
                                                    FinName = FinItem["ProductName"].ToString();
                                                    FinCode = FinItem["ProductCode"].ToString();
                                                }
                                                DataTable rawDt = new DataTable();
                                                rawDt = productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                                foreach (DataRow RawItem in rawDt.Rows)
                                                {
                                                    RawName = RawItem["ProductName"].ToString();
                                                    RawCode = RawItem["ProductCode"].ToString();
                                                }

                                                throw new ArgumentNullException("ReceiveInsert", "Stock not Available for Finish Item( Name: " + FinName + " & Code: " + FinCode + " ) \n and consumtion Material ( Name: " + RawName + " & Code: " + RawCode + " )");
                                            }
                                        }
                                        #endregion Stock
                                        #region Find Quantity From Transaction

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
                                        sqlText += " UOMWastage,";
                                        sqlText += " BOMId,";
                                        sqlText += " Post";
                                        sqlText += " )";
                                        sqlText += " values( ";
                                        sqlText += "@MasterReceiveNo,";
                                        sqlText += "@ItemReceiveLineNo,";
                                        sqlText += "@v1RawItemNo, ";
                                        sqlText += "@v1QuantityIssuePlaceQty ,";
                                        sqlText += "@AvgRateIssuePlaceAmt,";
                                        sqlText += "@v1CostPrice,";
                                        sqlText += "@v1UOM,";

                                        sqlText += " 0,0, ";

                                        sqlText += "@v1SubTotalIssuePlaceAmt,";
                                        sqlText += "@ItemCommentsD,";
                                        sqlText += "@MasterCreatedBy,";
                                        sqlText += "@MasterCreatedOn,";
                                        sqlText += "@MasterLastModifiedBy,";
                                        sqlText += "@MasterLastModifiedOn,";
                                        sqlText += "'" + newID + "',";
                                        sqlText += "@MasterReceiveDateTime,";

                                        sqlText += " 0,	0,";

                                        sqlText += "@v1Wastage,	";
                                        sqlText += "@v1BOMDate,	";
                                        sqlText += "@v1FinishItemNo,";
                                        sqlText += "@MasterTransactionType,";
                                        sqlText += "@MasterReturnId,";
                                        sqlText += "@v1UOMQtyIssuePlaceQty,";
                                        sqlText += "@v1UOMPriceIssuePlaceAmt,";
                                        sqlText += "@v1UOMc,";
                                        sqlText += "@v1UOMn,";
                                        sqlText += "@v1UOMWastage,";
                                        sqlText += "@v1BOMId,";

                                        sqlText += "'" + issueAutoPostValue + "'";
                                        sqlText += ")";
                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;

                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1QuantityIssuePlaceQty", FormatingNumeric(v1Quantity, IssuePlaceQty));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRateIssuePlaceAmt", FormatingNumeric(AvgRate, IssuePlaceAmt));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotalIssuePlaceAmt", FormatingNumeric(v1SubTotal, IssuePlaceAmt));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQtyIssuePlaceQty", FormatingNumeric(v1UOMQty, IssuePlaceQty));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPriceIssuePlaceAmt", FormatingNumeric(v1UOMPrice, IssuePlaceAmt));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);


                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #region Update Issue

                                        sqlText = "";
                                        sqlText += " update IssueHeaders set ";
                                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                        sqlText += " where (IssueHeaders.IssueNo= @MasterReceiveNo)";

                                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                        cmdUpdateIssue.Transaction = transaction;

                                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #endregion Update Issue

                                        #endregion Qty  check and Update

                                        #endregion Qty  check and Update
                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock

                            }


                            else if (Master.transactionType == "TollFinishReceive")
                            {
                                #region TollFinishReceive
                                BomId = string.Empty;
                                BOMDate = DateTime.MinValue;

                                #region Last BOMId
                                sqlText = "  ";
                                sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                                sqlText += " and effectdate<=@receiveDateDate";
                                sqlText += " and post='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID ";
                                }

                                sqlText += " order by effectdate desc ";

                                SqlCommand cmdBomId3 = new SqlCommand(sqlText, currConn);
                                cmdBomId3.Transaction = transaction;

                                cmdBomId3.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdBomId3.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                                cmdBomId3.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);


                                if (cmdBomId3.ExecuteScalar() == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    "No Price declaration found for this item");
                                    BomId = "0";
                                }
                                else
                                {
                                    BomId = (string)cmdBomId3.ExecuteScalar();
                                }

                                #endregion Last BOMId

                                #region Last BOMDate
                                sqlText = "  ";
                                sqlText += " select top 1 isnull(EffectDate,'1900/01/01') from BOMs";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                                sqlText += " and effectdate<=@receiveDateDate";
                                sqlText += " and post='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID  ";
                                }

                                sqlText += " order by effectdate desc ";

                                SqlCommand cmdBomEDate3 = new SqlCommand(sqlText, currConn);
                                cmdBomEDate3.Transaction = transaction;

                                cmdBomEDate3.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdBomEDate3.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                                cmdBomEDate3.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                if (cmdBomEDate3.ExecuteScalar() == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    "No Price declaration found for this item");
                                    BOMDate = DateTime.MinValue;
                                }
                                else
                                {
                                    BOMDate = (DateTime)cmdBomEDate3.ExecuteScalar();
                                }

                                #endregion Last BOMDate


                                #region Find Raw Item From BOM  and update Stock

                                sqlText = "";


                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty from BOMRaws b  ";
                                sqlText += " where ";
                                sqlText += " FinishItemNo='" + Item.ItemNo + "' ";
                                sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                                sqlText += " and effectdate='" + BOMDate + "'";
                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";

                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID='" + Master.CustomerID + "' ";
                                }
                                sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='finsih') ";

                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;
                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    "There is no price declaration for this item.");
                                }
                                else
                                {
                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;
                                    string rwUom = "";
                                    decimal vConvertionRate = 0;
                                    decimal AvgRate = 0;


                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {

                                        #region Declare
                                        decimal v1Quantity = 0;
                                        string v1RawItemNo = "";
                                        decimal v1CostPrice = 0;
                                        string v1UOM = "";
                                        decimal v1SubTotal = 0;
                                        decimal v1Wastage = 0;
                                        DateTime v1BOMDate = DateTime.Now.Date;
                                        string v1FinishItemNo = "";

                                        decimal v1UOMQty = 0;
                                        decimal v1UOMPrice = 0;
                                        decimal v1UOMc = 0;
                                        string v1UOMn = "";
                                        string v1BOMId = "";
                                        decimal v1UOMWastage = 0;
                                        #endregion DECLARE
                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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


                                        //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                        //vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());

                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());
                                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                        vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                        vWastage = FormatingNumeric(vWastage, IssuePlaceQty);


                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }

                                        /*Processing UOM*/

                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);


                                        #endregion Find Quantity From Products

                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                        v1Wastage = (vWastage) * Item.UOMQty;
                                        v1BOMId = BomId;
                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1CostPrice = AvgRate * vConvertionRate;
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                        v1FinishItemNo = Item.ItemNo;
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                        #endregion valueAssign

                                        #region Stock
                                        if (NegStockAllow == false)
                                        {
                                            decimal vStock = 0;
                                            //var stock = productDal.StockInHand(v1RawItemNo,
                                            //   Master.ReceiveDateTime + DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction).ToString();
                                            var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                                             Master.ReceiveDateTime +
                                                                             DateTime.Now.ToString(" HH:mm:ss"),
                                                           currConn, transaction, false).Rows[0]["Quantity"].ToString();

                                            vStock = Convert.ToDecimal(stock);


                                            if ((vStock - v1UOMQty) < 0)
                                            {
                                                string FinName = string.Empty;
                                                string FinCode = string.Empty;
                                                string RawName = string.Empty;
                                                string RawCode = string.Empty;
                                                DataTable finDt = new DataTable();
                                                finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                                foreach (DataRow FinItem in finDt.Rows)
                                                {
                                                    FinName = FinItem["ProductName"].ToString();
                                                    FinCode = FinItem["ProductCode"].ToString();
                                                }
                                                DataTable rawDt = new DataTable();
                                                rawDt = productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                                foreach (DataRow RawItem in rawDt.Rows)
                                                {
                                                    RawName = RawItem["ProductName"].ToString();
                                                    RawCode = RawItem["ProductCode"].ToString();
                                                }

                                                throw new ArgumentNullException("ReceiveInsert", "Stock not Available for Finish Item( Name: " + FinName + " & Code: " + FinCode + " ) \n and consumtion Material ( Name: " + RawName + " & Code: " + RawCode + " )");
                                            }
                                        }
                                        #endregion Stock
                                        #region Find Quantity From Transaction

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
                                        sqlText += " UOMWastage,";
                                        sqlText += " BOMId,";
                                        sqlText += " Post";
                                        sqlText += " )";
                                        sqlText += " values( ";
                                        sqlText += "@MasterReceiveNo,";
                                        sqlText += "@ItemReceiveLineNo,";
                                        sqlText += "@v1RawItemNo, ";
                                        sqlText += "@v1QuantityIssuePlaceQty ,";
                                        sqlText += "@AvgRateIssuePlaceAmt,";
                                        sqlText += "@v1CostPriceIssuePlaceAmt,";

                                        sqlText += "@v1UOM,";
                                        sqlText += " 0,0, ";
                                        sqlText += "@Fv1SubTotalIssuePlaceAmt,";
                                        sqlText += "@ItemCommentsD,";
                                        sqlText += "@MasterCreatedBy,";
                                        sqlText += "@MasterCreatedOn,";
                                        sqlText += "@MasterLastModifiedBy,";
                                        sqlText += "@MasterLastModifiedOn,";
                                        sqlText += "'" + newID + "',";
                                        sqlText += "@MasterReceiveDateTime,";
                                        sqlText += " 0,	0,";
                                        sqlText += "@v1Wastage,	";
                                        sqlText += "@v1BOMDate,	";
                                        sqlText += "@v1FinishItemNo,";
                                        sqlText += "@MasterTransactionType,";
                                        sqlText += "@MasterReturnId,";
                                        sqlText += "@v1UOMQtyIssuePlaceQty,";
                                        sqlText += "@Fv1UOMPriceIssuePlaceAmt,";
                                        sqlText += "@v1UOMc,";
                                        sqlText += "@v1UOMn,";
                                        sqlText += "@v1UOMWastage,";
                                        sqlText += "@v1BOMId,";

                                        sqlText += "'" + issueAutoPostValue + "'";
                                        sqlText += ")";
                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;

                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1QuantityIssuePlaceQty", FormatingNumeric(v1Quantity, IssuePlaceQty));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRateIssuePlaceAmt", FormatingNumeric(AvgRate, IssuePlaceAmt));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPriceIssuePlaceAmt", FormatingNumeric(v1CostPrice, IssuePlaceAmt));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotalIssuePlaceAmt", FormatingNumeric(v1SubTotal, IssuePlaceAmt));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQtyIssuePlaceQty", FormatingNumeric(v1UOMQty, IssuePlaceQty));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPriceIssuePlaceAmt", FormatingNumeric(v1UOMPrice, IssuePlaceAmt));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);


                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #region Update Issue

                                        sqlText = "";
                                        sqlText += " update IssueHeaders set ";
                                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                        sqlText += " where (IssueHeaders.IssueNo= @MasterReceiveNo)";

                                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                        cmdUpdateIssue.Transaction = transaction;

                                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #endregion Update Issue

                                        #endregion Qty  check and Update

                                        #endregion Qty  check and Update
                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock
                                #endregion TollFinishReceive
                            }
                            else if (Master.transactionType == "PackageProduction")
                            {
                                #region PackageProduction
                                BomId = string.Empty;
                                BOMDate = DateTime.MinValue;

                                #region Last BOMId
                                sqlText = "  ";
                                sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and vatname='VAT 1 (Package)' ";
                                sqlText += " and effectdate<=@receiveDateDate";
                                sqlText += " and post='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID ";
                                }

                                sqlText += " order by effectdate desc ";

                                SqlCommand cmdBomId3 = new SqlCommand(sqlText, currConn);
                                cmdBomId3.Transaction = transaction;

                                cmdBomId3.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdBomId3.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                                cmdBomId3.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                if (cmdBomId3.ExecuteScalar() == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    "No Price declaration found for this item");
                                    BomId = "0";
                                }
                                else
                                {
                                    BomId = (string)cmdBomId3.ExecuteScalar();
                                }

                                #endregion Last BOMId

                                #region Last BOMDate
                                sqlText = "  ";
                                sqlText += " select top 1 isnull(EffectDate,'1900/01/01') from BOMs";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo";
                                sqlText += " and vatname='VAT 1' ";
                                sqlText += " and effectdate<=@receiveDateDate";
                                sqlText += " and post='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID ";
                                }

                                sqlText += " order by effectdate desc ";

                                SqlCommand cmdBomEDate3 = new SqlCommand(sqlText, currConn);
                                cmdBomEDate3.Transaction = transaction;

                                cmdBomEDate3.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdBomEDate3.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                                cmdBomEDate3.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                if (cmdBomEDate3.ExecuteScalar() == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    "No Price declaration found for this item");
                                    BOMDate = DateTime.MinValue;
                                }
                                else
                                {
                                    BOMDate = (DateTime)cmdBomEDate3.ExecuteScalar();
                                }

                                #endregion Last BOMDate

                                #region Find Raw Item From BOM  and update Stock
                                sqlText = "";


                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty from BOMRaws b  ";
                                sqlText += " where ";
                                sqlText += " FinishItemNo='" + Item.ItemNo + "' ";
                                sqlText += " and vatname='VAT 1' ";
                                sqlText += " and effectdate='" + BOMDate + "'";
                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";

                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID='" + Master.CustomerID + "' ";
                                }
                                sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='trading') ";

                                DataTable dataTabler = new DataTable("RIFB");
                                SqlCommand cmdRIFB1 = new SqlCommand(sqlText, currConn);
                                cmdRIFB1.Transaction = transaction;
                                SqlDataAdapter reportDataAdapt1 = new SqlDataAdapter(cmdRIFB1);
                                reportDataAdapt1.Fill(dataTabler);

                                if (dataTabler == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTabler.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {
                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;
                                    string rwUom = "";
                                    decimal vConvertionRate = 0;
                                    decimal AvgRate = 0;

                                    foreach (DataRow BRItem in dataTabler.Rows)
                                    {
                                        #region Declare
                                        decimal v1Quantity = 0;
                                        string v1RawItemNo = "";
                                        decimal v1CostPrice = 0;
                                        string v1UOM = "";
                                        decimal v1SubTotal = 0;
                                        decimal v1Wastage = 0;
                                        DateTime v1BOMDate = DateTime.Now.Date;
                                        string v1FinishItemNo = "";

                                        decimal v1UOMQty = 0;
                                        decimal v1UOMPrice = 0;
                                        decimal v1UOMc = 0;
                                        string v1UOMn = "";
                                        string v1BOMId = "";
                                        decimal v1UOMWastage = 0;
                                        #endregion DECLARE

                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);


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


                                        //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                        //vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());

                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                        vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                        vWastage = FormatingNumeric(vWastage, IssuePlaceQty);

                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }

                                        /*Processing UOM*/

                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);

                                        #endregion Find Quantity From Products

                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                        v1Wastage = (vWastage) * Item.UOMQty;
                                        v1BOMId = BomId;
                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1CostPrice = AvgRate * vConvertionRate;
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                        v1FinishItemNo = Item.ItemNo;
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                        #endregion valueAssign

                                        #region Stock
                                        if (NegStockAllow == false)
                                        {
                                            decimal vStock = 0;
                                            //var stock = productDal.StockInHand(v1RawItemNo,
                                            //   Master.ReceiveDateTime + DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction).ToString();
                                            var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                                             Master.ReceiveDateTime +
                                                                             DateTime.Now.ToString(" HH:mm:ss"),
                                                           currConn, transaction, false).Rows[0]["Quantity"].ToString();

                                            vStock = Convert.ToDecimal(stock);


                                            if ((vStock - v1UOMQty) < 0)
                                            {
                                                string FinName = string.Empty;
                                                string FinCode = string.Empty;
                                                string RawName = string.Empty;
                                                string RawCode = string.Empty;
                                                DataTable finDt = new DataTable();
                                                finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                                foreach (DataRow FinItem in finDt.Rows)
                                                {
                                                    FinName = FinItem["ProductName"].ToString();
                                                    FinCode = FinItem["ProductCode"].ToString();
                                                }
                                                DataTable rawDt = new DataTable();
                                                rawDt = productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                                foreach (DataRow RawItem in rawDt.Rows)
                                                {
                                                    RawName = RawItem["ProductName"].ToString();
                                                    RawCode = RawItem["ProductCode"].ToString();
                                                }

                                                throw new ArgumentNullException("ReceiveInsert", "Stock not Available for Finish Item( Name: " + FinName + " & Code: " + FinCode + " ) \n and consumtion Material ( Name: " + RawName + " & Code: " + RawCode + " )");
                                            }
                                        }
                                        #endregion Stock
                                        #region Find Quantity From Transaction

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
                                        sqlText += " UOMWastage,";
                                        sqlText += " BOMId,";
                                        sqlText += " Post";
                                        sqlText += " )";
                                        sqlText += " values( ";
                                        sqlText += "@MasterReceiveNo,";
                                        sqlText += "@ItemReceiveLineNo,";
                                        sqlText += "@v1RawItemNo, ";
                                        sqlText += "@v1Quantity ,";
                                        sqlText += "@AvgRate,";
                                        sqlText += "@v1CostPrice,";
                                        sqlText += "@v1UOM,";

                                        sqlText += " 0,0, ";

                                        sqlText += "@v1SubTotal,";
                                        sqlText += "@ItemCommentsD,";
                                        sqlText += "@MasterCreatedBy,";
                                        sqlText += "@MasterCreatedOn,";
                                        sqlText += "@MasterLastModifiedBy,";
                                        sqlText += "@MasterLastModifiedOn,";
                                        sqlText += "'" + newID + "',";
                                        sqlText += "@MasterReceiveDateTime,";

                                        sqlText += " 0,	0,";

                                        sqlText += "@v1Wastage,	";
                                        sqlText += "@v1BOMDate,	";
                                        sqlText += "@v1FinishItemNo,";
                                        sqlText += "@MasterTransactionType,";
                                        sqlText += "@MasterReturnId,";
                                        sqlText += "@v1UOMQty,";
                                        sqlText += "@v1UOMPrice,";
                                        sqlText += "@v1UOMc,";
                                        sqlText += "@v1UOMn,";
                                        sqlText += "@v1UOMWastage,";
                                        sqlText += "@v1BOMId,";

                                        sqlText += "'" + issueAutoPostValue + "'";
                                        sqlText += ")";
                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;

                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);

                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #region Update Issue

                                        sqlText = "";
                                        sqlText += " update IssueHeaders set ";
                                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                        sqlText += " where (IssueHeaders.IssueNo= @MasterReceiveNo)";

                                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                        cmdUpdateIssue.Transaction = transaction;

                                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #endregion Update Issue

                                        #endregion Qty  check and Update

                                        #endregion Qty  check and Update
                                    }
                                }

                                #endregion Insert Issue


                                #region Find Raw Item From BOM  and update Stock

                                sqlText = "";


                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty from BOMRaws b  ";
                                sqlText += " where ";
                                sqlText += " FinishItemNo='" + Item.ItemNo + "' ";
                                sqlText += " and vatname='VAT 1' ";
                                sqlText += " and effectdate='" + BOMDate + "'";
                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID='" + Master.CustomerID + "' ";
                                }


                                sqlText += "   and (rawitemtype='finsih') ";

                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;
                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    "There is no price declaration for this item.");
                                }
                                else
                                {
                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;
                                    string rwUom = "";
                                    decimal vConvertionRate = 0;
                                    decimal AvgRate = 0;

                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {

                                        #region Declare
                                        decimal v1Quantity = 0;
                                        string v1RawItemNo = "";
                                        decimal v1CostPrice = 0;
                                        string v1UOM = "";
                                        decimal v1SubTotal = 0;
                                        decimal v1Wastage = 0;
                                        DateTime v1BOMDate = DateTime.Now.Date;
                                        string v1FinishItemNo = "";

                                        decimal v1UOMQty = 0;
                                        decimal v1UOMPrice = 0;
                                        decimal v1UOMc = 0;
                                        string v1UOMn = "";
                                        string v1BOMId = "";
                                        decimal v1UOMWastage = 0;
                                        #endregion DECLARE
                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);
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

                                        decimal nbrPriceFromBom = productDal.GetLastNBRPriceFromBOM(BRItem["RawItemNo"].ToString(),
                                                                 "VAT 1", Master.ReceiveDateTime,
                                                                 null, null);


                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                        vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                        vWastage = FormatingNumeric(vWastage, IssuePlaceQty);

                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }

                                        /*Processing UOM*/

                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);


                                        #endregion Find Quantity From Products

                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;

                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        #endregion valueAssign

                                        #region Stock
                                        if (NegStockAllow == false)
                                        {
                                            decimal vStock = 0;
                                            var stock = productDal.AvgPriceNew(v1RawItemNo,
                                                                             Master.ReceiveDateTime +
                                                                             DateTime.Now.ToString(" HH:mm:ss"),
                                                           currConn, transaction, false).Rows[0]["Quantity"].ToString();

                                            vStock = Convert.ToDecimal(stock);


                                            if ((vStock - v1UOMQty) < 0)
                                            {
                                                string FinName = string.Empty;
                                                string FinCode = string.Empty;
                                                string RawName = string.Empty;
                                                string RawCode = string.Empty;
                                                DataTable finDt = new DataTable();
                                                finDt = productDal.GetProductCodeAndNameByItemNo(Item.ItemNo);
                                                foreach (DataRow FinItem in finDt.Rows)
                                                {
                                                    FinName = FinItem["ProductName"].ToString();
                                                    FinCode = FinItem["ProductCode"].ToString();
                                                }
                                                DataTable rawDt = new DataTable();
                                                rawDt = productDal.GetProductCodeAndNameByItemNo(BRItem["RawItemNo"].ToString());
                                                foreach (DataRow RawItem in rawDt.Rows)
                                                {
                                                    RawName = RawItem["ProductName"].ToString();
                                                    RawCode = RawItem["ProductCode"].ToString();
                                                }

                                                throw new ArgumentNullException("ReceiveInsert", "Stock not Available for Finish Item( Name: " + FinName + " & Code: " + FinCode + " ) \n and consumtion Material ( Name: " + RawName + " & Code: " + RawCode + " )");
                                            }
                                        }
                                        #endregion Stock
                                        #region Find Quantity From Transaction

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
                                        sqlText += " FinishItemNo,";
                                        sqlText += " UOMPrice";
                                        sqlText += " )";
                                        sqlText += " values(	";
                                        sqlText += "@MasterReceiveNo,";//sqlText += "'" + newID + "',";
                                        sqlText += "@ItemReceiveLineNo, ";//sqlText += " '" + Item.InvoiceLineNo + "', ";
                                        sqlText += "@v1RawItemNo, ";//sqlText += " '" + Item.ItemNo + " ',";
                                        sqlText += "@v1Quantity, ";//sqlText += " '" + Item.Quantity + " ',";
                                        sqlText += " 0, ";//sqlText += " '" + Item.PromotionalQuantity + " ',";
                                        sqlText += "@nbrPriceFromBom, ";//sqlText += " '" + Item.SalesPrice + " ',";
                                        sqlText += "@nbrPriceFromBom, ";//sqlText += " '" + Item.NBRPrice + " ',";
                                        sqlText += "@AvgRate, ";//sqlText += " '" + AvgRate + " ',";
                                        sqlText += "@v1UOM, ";//sqlText += " '" + Item.UOM + " ',";
                                        sqlText += " '0', ";//sqlText += " '" + Item.VATRate + " ',";
                                        sqlText += " '0', ";//sqlText += " '" + Item.VATAmount + " ',";
                                        sqlText += "@v1SubTotal, ";//sqlText += " '" + Item.SubTotal + " ',";
                                        sqlText += "@ItemCommentsD, ";//sqlText += " '" + Item.CommentsD + " ',";
                                        sqlText += "@MasterCreatedBy, ";//sqlText += " '" + Master.CreatedBy + " ',";
                                        sqlText += "@MasterCreatedOn, ";//sqlText += " '" + Master.CreatedOn + " ',";
                                        sqlText += "@MasterLastModifiedBy, ";//sqlText += " '" + Master.LastModifiedBy + " ',";
                                        sqlText += "@MasterLastModifiedOn, ";//sqlText += " '" + Master.LastModifiedOn + " ',";
                                        sqlText += " '0', ";//sqlText += " '" + Item.SD + " ',";
                                        sqlText += " '0', ";//sqlText += " '" + Item.SDAmount + " ',";
                                        sqlText += " 'New', ";//sqlText += " '" + Item.SaleTypeD + " ',";
                                        sqlText += "@MasterReturnId, ";//sqlText += " '" + Item.PreviousSalesInvoiceNoD + " ',";
                                        sqlText += " 'N', ";//sqlText += " '" + Item.TradingD + " ',";
                                        sqlText += " 'N', ";//sqlText += " '" + Item.NonStockD + " ',";
                                        sqlText += " '0', ";//sqlText += " '" + Item.TradingMarkUp + " ',";
                                        sqlText += "@MasterReceiveDateTime, ";//sqlText += " '" + Master.InvoiceDateTime + " ',";
                                        sqlText += " 'VAT', ";//sqlText += " '" + Item.Type + " ',";
                                        sqlText += "@MasterTransactionType, ";//sqlText += " '" + Master.transactionType + " ',";
                                        sqlText += "@MasterReturnId, ";//sqlText += " '" + Master.ReturnId + " ',";
                                        sqlText += "@MasterPost, ";//sqlText += " '" + Master.Post + " ',";
                                        sqlText += "@v1UOMQty, ";//sqlText += " '" + Item.UOMQty + " ',";
                                        sqlText += "@v1UOMn, ";//sqlText += " '" + Item.UOMn + " ',";
                                        sqlText += "@v1UOMc, ";//sqlText += " '" + Item.UOMc + " ',";
                                        sqlText += " '0', ";//sqlText += "'" + Item.DiscountAmount + "',";
                                        sqlText += "@nbrPriceFromBom, ";//sqlText += "'" + Item.DiscountedNBRPrice + "',";
                                        sqlText += " '0',";//sqlText += "'" + Item.DollerValue + "',";
                                        sqlText += "@nbrPriceFromBom, ";
                                        sqlText += "@ItemItemNo, ";
                                        sqlText += "@v1UOMPrice, ";//sqlText += " '" + Item.UOMPrice + "' ";
                                        sqlText += ")	";
                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;

                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@nbrPriceFromBom", nbrPriceFromBom);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@nbrPriceFromBom", nbrPriceFromBom);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@nbrPriceFromBom", nbrPriceFromBom);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@nbrPriceFromBom", nbrPriceFromBom);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);

                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #region Update Issue


                                        sqlText = "";
                                        sqlText += " update SalesInvoiceHeaders set ";
                                        sqlText += " TotalAmount= (select sum(Quantity*NBRPrice) from SalesInvoiceDetails";
                                        sqlText += "  where SalesInvoiceDetails.SalesInvoiceNo =SalesInvoiceHeaders.SalesInvoiceNo)";
                                        sqlText += " where (SalesInvoiceHeaders.SalesInvoiceNo= @MasterReceiveNo )";



                                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                        cmdUpdateIssue.Transaction = transaction;

                                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                            MessageVM.receiveMsgUnableToSaveIssue);
                                        }

                                        #endregion Update Issue

                                        #endregion Qty  check and Update

                                        #endregion Qty  check and Update
                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock


                                #endregion PackageProduction
                            }

                        }//  if (Master.FromBOM == "Y")

                        #endregion From BOM


                        #endregion Purchase ID Create For IssueReturn

                        #endregion if Transection not Other Insert Issue /Receive
                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";


                        sqlText += " update ReceiveDetails set ";
                        sqlText += " ReceiveLineNo  =@ItemReceiveLineNo,";
                        sqlText += " Quantity       =@ItemQuantity,";
                        sqlText += " CostPrice      =@ItemCostPrice,";
                        sqlText += " NBRPrice       =@ItemNBRPrice,";
                        sqlText += " UOM            =@ItemUOM,";
                        sqlText += " VATRate        =@ItemVATRate,";
                        sqlText += " VATAmount      =@ItemVATAmount,";
                        sqlText += " SubTotal       =@ItemSubTotal,";
                        sqlText += " Comments       =@ItemCommentsD,";
                        sqlText += " LastModifiedBy =@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                        sqlText += " SD             =@ItemSD,";
                        sqlText += " SDAmount       =@ItemSDAmount,";
                        sqlText += " ReceiveDateTime=@MasterReceiveDateTime,";
                        sqlText += " transactionType=@MasterTransactionType,";
                        sqlText += " ReceiveReturnId=@MasterReturnId,";
                        sqlText += " BOMId          =@ItemBOMId,";
                        sqlText += " UOMPrice       =@ItemUOMPrice,";
                        sqlText += " UOMQty         =@ItemUOMQty,";
                        sqlText += " UOMn           =@ItemUOMn,";
                        sqlText += " UOMc           =@ItemUOMc,";
                        sqlText += " VATName        =@ItemVatName,";
                        sqlText += " CurrencyValue  ='" + usdResults[0] + "',";
                        sqlText += " DollerValue    ='" + usdResults[1] + "',";
                        sqlText += " Post           =@MasterPost";
                        sqlText += " where ReceiveNo=@MasterReceiveNo ";
                        sqlText += " and 	ItemNo  =@ItemItemNo ";



                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCostPrice", Item.CostPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemNBRPrice", Item.NBRPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVATAmount", Item.VATAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSD", Item.SD);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSDAmount", Item.SDAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBOMId", Item.BOMId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMn", Item.UOMn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable
                        #region Update Issue and Receive if Transaction is not Other

                        #region TollReceive
                        if (Master.transactionType == "ReceiveReturn" && Item.ReturnTransactionType == "TollReceive")
                        {

                            DataTable TollItemInfo = productDal.SearchRawItemNo(Master.ReturnId);

                            string TollItem;
                            decimal TollUnitCost = 0;

                            TollItem = TollItemInfo.Rows[0]["ItemNo"].ToString();
                            TollUnitCost = Convert.ToDecimal(TollItemInfo.Rows[0]["CostPrice"].ToString());
                            #region CheckID
                            int IDRawExist = 0;
                            sqlText = "";
                            sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterReceiveNo ";
                            sqlText += " ItemNo='" + TollItem + "' ";

                            SqlCommand cmdFindRawId = new SqlCommand(sqlText, currConn);
                            cmdFindRawId.Transaction = transaction;

                            cmdFindRawId.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                            IDRawExist = (int)cmdFindRawId.ExecuteScalar();

                            #endregion CheckID

                            if (IDRawExist <= 0)
                            {
                                #region Insert to Issue 16 out

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

                                sqlText += " UOMQty,";
                                sqlText += " UOMPrice,";
                                sqlText += " UOMc,";
                                sqlText += " UOMn,";
                                sqlText += " UOMWastage,";


                                sqlText += " Post";

                                sqlText += " )";
                                sqlText += " values(	";
                                sqlText += "'" + newID + "',";
                                sqlText += "'00',";
                                sqlText += "@TollItem,";
                                sqlText += "@ItemQuantity,";
                                sqlText += " 0,"; // NBR
                                sqlText += "@TollUnitCost,";
                                sqlText += "@ItemUOM,";
                                sqlText += " 0,	";
                                sqlText += " 0,";
                                sqlText += "@TollUnitCostItemUOMQty ,"; // sub total
                                sqlText += "@MasterCreatedBy,";
                                sqlText += "@MasterCreatedOn,";
                                sqlText += "@MasterLastModifiedBy,";
                                sqlText += "@MasterLastModifiedOn,";
                                sqlText += "'" + newID + "',";
                                sqlText += "@MasterReceiveDateTime,";
                                sqlText += " 0,";
                                sqlText += " 0,";
                                sqlText += " 0,	";
                                sqlText += " 0,	";
                                sqlText += " 0,";
                                sqlText += "'TollReceiveReturn',";
                                sqlText += "@MasterReturnId,";
                                sqlText += "@ItemUOMQty,";
                                sqlText += "@ItemUOMPrice,";
                                sqlText += "@ItemUOMc,";
                                sqlText += "@ItemUOMn,";
                                sqlText += "'0',";

                                sqlText += "@MasterPost";

                                //sqlText += "'" + Master.@Post + "'";
                                sqlText += ")	";
                                SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                cmdInsertIssue.Transaction = transaction;

                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@TollItem", TollItem);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemQuantity", Item.Quantity);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@TollUnitCost", TollUnitCost);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@TollUnitCostItemUOMQty", TollUnitCost * Item.UOMQty);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMQty", Item.UOMQty);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMPrice", Item.UOMPrice);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMc", Item.UOMc);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMn", Item.UOMn);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);


                                transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                    MessageVM.PurchasemsgUnableToSaveIssue);
                                }

                                #endregion Insert to Issue
                            }
                            else
                            {
                                #region Update to Issue 16 out


                                sqlText = "";
                                sqlText += " update IssueDetails set";
                                sqlText += " IssueLineNo='00',";
                                sqlText += " Comments       =@ItemCommentsD,";
                                sqlText += " LastModifiedBy =@MasterLastModifiedBy,";
                                sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                                sqlText += " IssueDateTime  =@MasterReceiveDateTime,";
                                sqlText += " Post           =@MasterPost,";
                                sqlText += " uom            =@ItemUOM,";
                                sqlText += " Quantity       =@ItemQuantityIssuePlaceQty,";
                                sqlText += " CostPrice      =@TollUnitCostIssuePlaceAmt,";
                                sqlText += " NBRPrice       = " + 0 + ",";
                                sqlText += " transactionType='TollReceiveReturn',";
                                sqlText += " IssueReturnId  =@MasterReturnId,";
                                sqlText += " UOMQty         =@ItemUOMQty,";
                                sqlText += " UOMPrice       =@TollUnitCost,";
                                sqlText += " UOMc           =@ItemUOMc,";
                                sqlText += " UOMn           =@ItemUOMn,";
                                sqlText += " UOMWastage     ='" + 0 + "',";
                                sqlText += " BOMId          =@ItemBOMId,";
                                sqlText += " SubTotal       =@TollUnitCostItemQuantityIssuePlaceAmt";
                                sqlText += " WHERE ItemNo   =@TollItem ";
                                sqlText += " and  IssueNo   =@MasterReceiveNo";



                                SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                cmdInsertIssue.Transaction = transaction;

                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemQuantityIssuePlaceQty", FormatingNumeric(Item.Quantity, IssuePlaceQty));
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@TollUnitCostIssuePlaceAmt", FormatingNumeric(TollUnitCost, IssuePlaceAmt));
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMQty", Item.UOMQty);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@TollUnitCost", TollUnitCost);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMc", Item.UOMc);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemUOMn", Item.UOMn);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemBOMId", Item.BOMId);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@TollUnitCostItemQuantityIssuePlaceAmt", FormatingNumeric((TollUnitCost * Item.Quantity), IssuePlaceAmt));
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@TollItem", TollItem);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                    MessageVM.PurchasemsgUnableToSaveIssue);
                                }

                                #endregion Update to Issue
                            }






                        }
                        #endregion TollReceive


                        #region Transaction is FromBOM
                        if (Master.FromBOM == "Y")
                        {
                            BomId = string.Empty;
                            BOMDate = DateTime.MinValue;
                            string bomFormatDate = "";

                            #region Last BOMId

                            sqlText = "  ";
                            sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo";
                            sqlText += " and vatname=@ItemVatName ";
                            //sqlText += " and effectdate<='" + receiveDate.Date + "'";
                            sqlText += " and effectdate<=@receiveFormatDate";

                            sqlText += " and post='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID ";
                            }

                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomId4 = new SqlCommand(sqlText, currConn);
                            cmdBomId4.Transaction = transaction;

                            cmdBomId4.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomId4.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdBomId4.Parameters.AddWithValueAndNullHandle("@receiveFormatDate", receiveFormatDate);
                            cmdBomId4.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);


                            if (cmdBomId4.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BomId = "0";
                            }
                            else
                            {
                                BomId = (string)cmdBomId4.ExecuteScalar();

                            }

                            #endregion Last BOMId

                            #region Last BOMDate

                            sqlText = "  ";
                            sqlText += " select top 1 isnull(EffectDate,'1900/01/01') from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname=@ItemVatName ";
                            //sqlText += " and effectdate<='" + receiveDate.Date + "'";
                            sqlText += " and effectdate<=@receiveFormatDate";
                            sqlText += " and post='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID  ";
                            }

                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomEDate4 = new SqlCommand(sqlText, currConn);
                            cmdBomEDate4.Transaction = transaction;

                            cmdBomId4.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomId4.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdBomId4.Parameters.AddWithValueAndNullHandle("@receiveFormatDate", receiveFormatDate);
                            cmdBomId4.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            if (cmdBomEDate4.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BOMDate = DateTime.MinValue;
                                bomFormatDate = BOMDate.Date.ToString("yyyy/MM/dd 00:00:00");
                            }
                            else
                            {
                                BOMDate = (DateTime)cmdBomEDate4.ExecuteScalar();
                                //bomFormatDate = BOMDate.Date.ToString("yyyy-MM-dd 00:00:00");
                                bomFormatDate = BOMDate.Date.ToString("yyyy/MM/dd 00:00:00");

                            }

                            #endregion Last BOMDate


                            if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn"
                                || Master.transactionType == "WIP")
                            {
                                #region Update to Issue



                                #region Find Raw Item From BOM  and update Stock
                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,b.UOMUQty,b.UOMWQty,b.TransactionType from BOMRaws b  ";
                                sqlText += " WHERE ";
                                sqlText += " FinishItemNo='" + Item.ItemNo + "' ";
                                sqlText += " and vatname='" + Item.VatName + "' ";
                                //sqlText += " and effectdate='" + BOMDate.Date + "'";
                                sqlText += " and effectdate='" + bomFormatDate + "'";
                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID='" + Master.CustomerID + "' ";
                                }


                                sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='wip' or rawitemtype='finish') ";

                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;
                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {
                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;
                                    decimal vConvertionRate = 0;
                                    decimal AvgRate = 0;

                                    #region Declare
                                    decimal v1Quantity = 0;
                                    string v1RawItemNo = "";
                                    decimal v1CostPrice = 0;
                                    string v1UOM = "";
                                    decimal v1SubTotal = 0;
                                    decimal v1Wastage = 0;
                                    DateTime v1BOMDate = DateTime.Now.Date;
                                    string v1FinishItemNo = "";

                                    decimal v1UOMQty = 0;
                                    decimal v1UOMPrice = 0;
                                    decimal v1UOMc = 0;
                                    string v1UOMn = "";
                                    string v1BOMId = "";
                                    decimal v1UOMWastage = 0;
                                    string rwUom = "";
                                    string vTransactionType = "";

                                    #endregion DECLARE

                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {
                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);
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


                                        #endregion Find Quantity From Products

                                        //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                        //vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());

                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                        vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                        vWastage = FormatingNumeric(vWastage, IssuePlaceQty);

                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }

                                        /*Processing UOM*/

                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);

                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                        v1Wastage = (vWastage) * Item.UOMQty;
                                        v1BOMId = BomId;
                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1CostPrice = AvgRate * vConvertionRate;
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                        v1FinishItemNo = Item.ItemNo;
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                        vTransactionType = BRItem["TransactionType"].ToString();
                                        #endregion valueAssign

                                        #region Qty  check and Update

                                        if (NegStockAllow == false)
                                        {
                                            #region Find Quantity From Products

                                            //decimal BRItemoldStock =
                                            //    productDal.StockInHand(BRItem["RawItemNo"].ToString(),
                                            //                           Master.ReceiveDateTime, currConn,
                                            //                           transaction);
                                            decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(),
                                                                             Master.ReceiveDateTime,
                                                           currConn, transaction, false).Rows[0]["Quantity"].ToString());


                                            #endregion Find Quantity From Products

                                            #region Find Quantity From Transaction

                                            sqlText = "";
                                            sqlText +=
                                                "select isnull(UOMQty ,isnull(Quantity,0)) from IssueDetails ";
                                            sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                                       "and IssueNo=@MasterReceiveNo";
                                            SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                            cmdBRItemTranQty.Transaction = transaction;

                                            cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                            decimal BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                            #endregion Find Quantity From Transaction



                                            if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                            {
                                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                                MessageVM.
                                                                                    receiveMsgStockNotAvailablePost);
                                            }
                                        }

                                        #endregion Qty  check and Update

                                        sqlText = "";
                                        sqlText += " update IssueDetails set";
                                        sqlText += " IssueLineNo    =@ItemReceiveLineNo,";
                                        sqlText += " Comments       =@ItemCommentsD,";
                                        sqlText += " LastModifiedBy =@MasterLastModifiedBy,";
                                        sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                                        sqlText += " IssueDateTime  =@MasterReceiveDateTime,";
                                        sqlText += " Post           =@issueAutoPostValue,";
                                        sqlText += " uom            =@v1UOM,";
                                        sqlText += " Quantity       =@FormatingNumeric(v1Quantity, IssuePlaceQty),";
                                        sqlText += " Wastage        =@v1Wastage,";
                                        sqlText += " BOMDate        =@v1BOMDate,";
                                        sqlText += " CostPrice      =@FormatingNumeric(v1CostPrice, IssuePlaceAmt),";
                                        sqlText += " NBRPrice       =@AvgRate,";
                                        if (vTransactionType.Trim() == "TollReceiveRaw")
                                        {
                                            sqlText += " transactionType=@vTransactionTypeTrim,";
                                        }
                                        else
                                        {
                                            if (Master.WithToll == "Y")
                                            {
                                                sqlText += " transactionType=@MasterTransactionType" + "Toll" + "',";
                                            }
                                            else
                                            {
                                                sqlText += " transactionType=@MasterTransactionType,";
                                            }

                                        }

                                        sqlText += " IssueReturnId      =@MasterReturnId,";
                                        sqlText += " UOMQty             =@v1UOMQty,";
                                        sqlText += " UOMPrice           =@v1UOMPrice,";
                                        sqlText += " UOMc               =@v1UOMc,";
                                        sqlText += " UOMn               =@v1UOMn,";
                                        sqlText += " UOMWastage         =@v1UOMWastage,";
                                        sqlText += " BOMId              =@v1BOMId,";
                                        sqlText += " SubTotal           =@v1SubTotalIssuePlaceAmt";
                                        sqlText += " WHERE FinishItemNo =@v1FinishItemNo AND ItemNo=@v1RawItemNo ";
                                        sqlText += " and  IssueNo=@MasterReceiveNo";


                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;

                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1QuantityIssuePlaceQty", FormatingNumeric(v1Quantity, IssuePlaceQty));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPriceIssuePlaceAmt", FormatingNumeric(v1CostPrice, IssuePlaceAmt));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotalIssuePlaceAmt", FormatingNumeric(v1SubTotal, IssuePlaceAmt));
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                        if (vTransactionType.Trim() == "TollReceiveRaw")
                                        {
                                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@vTransactionTypeTrim", vTransactionType.Trim());
                                        }
                                        else
                                        {
                                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                        }

                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                            MessageVM.receiveMsgUnableToUpdateIssue);
                                        }

                                        #endregion Qty  check and Update

                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock

                                /////////

                                #endregion Update to Issue

                                #region Update Issue Header

                                sqlText = "";
                                sqlText += " update IssueHeaders set ";
                                sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                sqlText += " where (IssueHeaders.IssueNo=@MasterReceiveNo)";

                                SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                cmdUpdateIssue.Transaction = transaction;
                                cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                    MessageVM.receiveMsgUnableToUpdateIssue);
                                }

                                #endregion Update Issue Header

                            }
                            else if (Master.transactionType == "Tender")
                            {
                                #region Update to Issue


                                #region Find Raw Item From BOM  and update Stock

                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM,UOMUQty,UOMWQty from BOMRaws b  ";
                                sqlText += " WHERE ";
                                sqlText += " FinishItemNo='" + Item.ItemNo + "' ";
                                sqlText += " and vatname='" + Item.VatName + "' ";
                                sqlText += " and effectdate='" + BOMDate.Date + "'";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";

                                sqlText += "  and post='Y'   and (b.rawitemtype='raw' or b.rawitemtype='pack') ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID='" + Master.CustomerID + "' ";
                                }

                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;
                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {

                                    #region Declare
                                    decimal v1Quantity = 0;
                                    string v1RawItemNo = "";
                                    decimal v1CostPrice = 0;
                                    string v1UOM = "";
                                    decimal v1SubTotal = 0;
                                    decimal v1Wastage = 0;
                                    DateTime v1BOMDate = DateTime.Now.Date;
                                    string v1FinishItemNo = "";

                                    decimal v1UOMQty = 0;
                                    decimal v1UOMPrice = 0;
                                    decimal v1UOMc = 0;
                                    string v1UOMn = "";
                                    string v1BOMId = "";
                                    decimal v1UOMWastage = 0;
                                    #endregion DECLARE

                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;

                                    string rwUom = "";
                                    decimal vConvertionRate = 0;
                                    decimal AvgRate = 0;


                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {
                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);
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


                                        #endregion Find Quantity From Products

                                        //vQuantity = Convert.ToDecimal(BRItem["UOMUQty"].ToString());
                                        //vWastage = Convert.ToDecimal(BRItem["UOMWQty"].ToString());
                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                        vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                        vWastage = FormatingNumeric(vWastage, IssuePlaceQty);

                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }

                                        /*Processing UOM*/

                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);


                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                        v1Wastage = (vWastage) * Item.UOMQty;
                                        v1BOMId = Item.BOMId;
                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1CostPrice = AvgRate * vConvertionRate;
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                        v1FinishItemNo = Item.ItemNo;
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                        #endregion valueAssign
                                        #region Stock Check
                                        if (NegStockAllow == false)
                                        {
                                            #region Find Quantity From Products

                                            //decimal BRItemoldStock =
                                            //    productDal.StockInHand(v1RawItemNo,
                                            //                           Master.ReceiveDateTime, currConn,
                                            //                           transaction);

                                            decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(),
                                                                             Master.ReceiveDateTime,
                                                           currConn, transaction, false).Rows[0]["Quantity"].ToString());

                                            #endregion Find Quantity From Products

                                            #region Find Quantity From Transaction

                                            sqlText = "";
                                            sqlText +=
                                                "select isnull(UOMQty,isnull(Quantity ,0)) from IssueDetails ";
                                            sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                                       "and IssueNo=@MasterReceiveNo";
                                            SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                            cmdBRItemTranQty.Transaction = transaction;

                                            cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                            decimal BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                            #endregion Find Quantity From Transaction



                                            if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                            {
                                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                                MessageVM.
                                                                                    receiveMsgStockNotAvailablePost);
                                            }
                                        }
                                        #endregion Stock Check



                                        sqlText = "";
                                        sqlText += " update IssueDetails set";
                                        sqlText += " IssueLineNo    =@ItemReceiveLineNo,";
                                        sqlText += " Comments       =@ItemCommentsD,";
                                        sqlText += " LastModifiedBy =@MasterLastModifiedBy,";
                                        sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                                        sqlText += " IssueDateTime  =@MasterReceiveDateTime,";
                                        sqlText += " Post           =@issueAutoPostValue,";
                                        sqlText += " uom            =@v1UOM,";
                                        sqlText += " Quantity       =@v1Quantity,";
                                        sqlText += " Wastage        =@v1Wastage,";
                                        sqlText += " BOMDate        =@v1BOMDate,";
                                        sqlText += " CostPrice      =@v1CostPrice,";
                                        sqlText += " NBRPrice       =@AvgRate,";
                                        sqlText += " transactionType=@MasterTransactionType,";
                                        sqlText += " IssueReturnId  =@MasterReturnId,";
                                        sqlText += " UOMQty         =@v1UOMQty,";
                                        sqlText += " UOMPrice       =@v1UOMPrice,";
                                        sqlText += " UOMc           =@v1UOMc,";
                                        sqlText += " UOMn           =@v1UOMn,";
                                        sqlText += " UOMWastage     =@v1UOMWastage,";
                                        sqlText += " BOMId          =@v1BOMId,";
                                        sqlText += " SubTotal       =@v1SubTotal";
                                        sqlText += " WHERE FinishItemNo=@v1FinishItemNo  AND ItemNo=@v1RawItemNo ";
                                        sqlText += " and  IssueNo=@MasterReceiveNo";


                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;

                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                            MessageVM.receiveMsgUnableToUpdateIssue);
                                        }

                                        #endregion Qty  check and Update

                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock

                                /////////

                                #endregion Update to Issue

                                #region Update Issue Header

                                sqlText = "";
                                sqlText += " update IssueHeaders set ";
                                sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                sqlText += " where (IssueHeaders.IssueNo= @MasterReceiveNo)";

                                SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                cmdUpdateIssue.Transaction = transaction;

                                cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                    MessageVM.receiveMsgUnableToUpdateIssue);
                                }

                                #endregion Update Issue Header

                            }
                            #region Transaction is TollFinishReceive

                            else if (Master.transactionType == "TollFinishReceive")
                            {
                                BomId = string.Empty;
                                BOMDate = DateTime.MinValue;

                                #region Last BOMId
                                sqlText = "  ";
                                sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                                sqlText += " and effectdate<=@receiveDateDate ";
                                sqlText += " and post='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID ";
                                }

                                sqlText += " order by effectdate desc ";

                                SqlCommand cmdBomId5 = new SqlCommand(sqlText, currConn);
                                cmdBomId5.Transaction = transaction;

                                cmdBomId5.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdBomId5.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                                cmdBomId5.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                if (cmdBomId5.ExecuteScalar() == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    "No Price declaration found for this item");
                                    BomId = "0";
                                }
                                else
                                {
                                    BomId = (string)cmdBomId5.ExecuteScalar();
                                }

                                #endregion Last BOMId

                                #region Last BOMDate
                                sqlText = "  ";
                                sqlText += " select top 1 isnull(EffectDate,'1900/01/01') from BOMs";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                                sqlText += " and effectdate<=@receiveDateDate";
                                sqlText += " and post='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID  ";
                                }

                                sqlText += " order by effectdate desc ";

                                SqlCommand cmdBomEDate5 = new SqlCommand(sqlText, currConn);
                                cmdBomEDate5.Transaction = transaction;

                                cmdBomEDate5.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdBomEDate5.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                                cmdBomEDate5.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                if (cmdBomEDate5.ExecuteScalar() == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    "No Price declaration found for this item");
                                    BOMDate = DateTime.MinValue;
                                }
                                else
                                {
                                    BOMDate = (DateTime)cmdBomEDate5.ExecuteScalar();
                                }

                                #endregion Last BOMDate

                                #region Update to Issue



                                #region Find Raw Item From BOM  and update Stock



                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,b.UOMUQty,b.UOMWQty,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM from BOMRaws b  ";
                                sqlText += " where ";
                                sqlText += " FinishItemNo='" + Item.ItemNo + "' ";
                                sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                                sqlText += " and effectdate='" + BOMDate + "'";
                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID='" + Master.CustomerID + "' ";
                                }


                                sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='finsih') ";


                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;
                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {
                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;

                                    string rwUom = "";
                                    decimal vConvertionRate = 0;
                                    decimal AvgRate = 0;


                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {
                                        #region Declare
                                        decimal v1Quantity = 0;
                                        string v1RawItemNo = "";
                                        decimal v1CostPrice = 0;
                                        string v1UOM = "";
                                        decimal v1SubTotal = 0;
                                        decimal v1Wastage = 0;
                                        DateTime v1BOMDate = DateTime.Now.Date;
                                        string v1FinishItemNo = "";

                                        decimal v1UOMQty = 0;
                                        decimal v1UOMPrice = 0;
                                        decimal v1UOMc = 0;
                                        string v1UOMn = "";
                                        string v1BOMId = "";
                                        decimal v1UOMWastage = 0;
                                        #endregion DECLARE

                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);
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


                                        #endregion Find Quantity From Products

                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());
                                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                        vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                        vWastage = FormatingNumeric(vWastage, IssuePlaceQty);

                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }

                                        /*Processing UOM*/

                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);

                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                        v1Wastage = (vWastage) * Item.UOMQty;
                                        v1BOMId = Item.BOMId;
                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1CostPrice = AvgRate * vConvertionRate;
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                        v1FinishItemNo = Item.ItemNo;
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                        #endregion valueAssign
                                        #region Stock Check
                                        if (NegStockAllow == false)
                                        {
                                            #region Find Quantity From Products


                                            decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(v1RawItemNo,
                                                                             Master.ReceiveDateTime,
                                                           currConn, transaction, false).Rows[0]["Quantity"].ToString());

                                            #endregion Find Quantity From Products

                                            #region Find Quantity From Transaction

                                            sqlText = "";
                                            sqlText +=
                                                "select isnull(UOMQty,isnull(Quantity ,0)) from IssueDetails ";
                                            sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                                       "and IssueNo=@MasterReceiveNo";
                                            SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                            cmdBRItemTranQty.Transaction = transaction;

                                            cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                            decimal BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                            #endregion Find Quantity From Transaction



                                            if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                            {
                                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                                MessageVM.
                                                                                    receiveMsgStockNotAvailablePost);
                                            }
                                        }
                                        #endregion Stock Check



                                        sqlText = "";
                                        sqlText += " update IssueDetails set";
                                        sqlText += " IssueLineNo    =@ItemReceiveLineNo,";
                                        sqlText += " Comments       =@ItemCommentsD,";
                                        sqlText += " LastModifiedBy =@MasterLastModifiedBy,";
                                        sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                                        sqlText += " IssueDateTime  =@MasterReceiveDateTime,";
                                        sqlText += " Post           ='" + issueAutoPostValue + "',";
                                        sqlText += " uom            =@v1UOM,";
                                        sqlText += " Quantity       =@v1Quantity,";
                                        sqlText += " Wastage        =@v1Wastage,";
                                        sqlText += " BOMDate        =@v1BOMDate,";
                                        sqlText += " CostPrice      =@v1CostPrice,";
                                        sqlText += " NBRPrice       =@AvgRate,";
                                        sqlText += " transactionType=@MasterTransactionType,";
                                        sqlText += " IssueReturnId  =@MasterReturnId,";
                                        sqlText += " UOMQty         =@v1UOMQty,";
                                        sqlText += " UOMPrice       =@v1UOMPrice,";
                                        sqlText += " UOMc           =@v1UOMc,";
                                        sqlText += " UOMn           =@v1UOMn,";
                                        sqlText += " UOMWastage     =@v1UOMWastage,";
                                        sqlText += " BOMId          =@v1BOMId,";
                                        sqlText += " SubTotal       =@v1SubTotal";
                                        sqlText += " WHERE FinishItemNo=@v1FinishItemNo AND ItemNo=@v1RawItemNo ";
                                        sqlText += " and  IssueNo=@MasterReceiveNo";

                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;

                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                            MessageVM.receiveMsgUnableToUpdateIssue);
                                        }

                                        #endregion Qty  check and Update

                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock


                                #endregion Update to Issue

                                #region Update Issue Header

                                sqlText = "";
                                sqlText += " update IssueHeaders set ";
                                sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                sqlText += " where (IssueHeaders.IssueNo=@MasterReceiveNo)";

                                SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                                cmdUpdateIssue.Transaction = transaction;

                                cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                    MessageVM.receiveMsgUnableToUpdateIssue);
                                }

                                #endregion Update Issue Header

                            }
                            else if (Master.transactionType == "PackageProduction")
                            {
                                BomId = string.Empty;
                                BOMDate = DateTime.MinValue;

                                #region Last BOMId
                                sqlText = "  ";
                                sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and vatname='VAT 1 (Package)' ";
                                sqlText += " and effectdate<=@receiveDateDate";
                                sqlText += " and post='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID ";
                                }

                                sqlText += " order by effectdate desc ";

                                SqlCommand cmdBomId5 = new SqlCommand(sqlText, currConn);
                                cmdBomId5.Transaction = transaction;

                                cmdBomId5.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdBomId5.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                                cmdBomId5.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                if (cmdBomId5.ExecuteScalar() == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    "No Price declaration found for this item");
                                    BomId = "0";
                                }
                                else
                                {
                                    BomId = (string)cmdBomId5.ExecuteScalar();
                                }

                                #endregion Last BOMId

                                #region Last BOMDate
                                sqlText = "  ";
                                sqlText += " select top 1 isnull(EffectDate,'1900/01/01') from BOMs";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and vatname='VAT 1 (Package)' ";
                                sqlText += " and effectdate<=@receiveDateDate";
                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID ";
                                }

                                sqlText += " order by effectdate desc ";
                                SqlCommand cmdBomEDate5 = new SqlCommand(sqlText, currConn);
                                cmdBomEDate5.Transaction = transaction;

                                cmdBomEDate5.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdBomEDate5.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                                cmdBomEDate5.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                if (cmdBomEDate5.ExecuteScalar() == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    "No Price declaration found for this item");
                                    BOMDate = DateTime.MinValue;
                                }
                                else
                                {
                                    BOMDate = (DateTime)cmdBomEDate5.ExecuteScalar();
                                }

                                #endregion Last BOMDate
                                #region Sale

                                #region Find Raw Item From BOM  and update Stock



                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,b.UOMUQty,b.UOMWQty,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM from BOMRaws b  ";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo  ";
                                sqlText += " and vatname='VAT 1 (Package)' ";
                                sqlText += " and effectdate=@BOMDate";
                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID  ";
                                }

                                sqlText += "   and (rawitemtype='finish' ) ";


                                DataTable dataTable33 = new DataTable("RIFB");
                                SqlCommand cmdRIFB33 = new SqlCommand(sqlText, currConn);
                                cmdRIFB33.Transaction = transaction;

                                cmdRIFB33.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdRIFB33.Parameters.AddWithValueAndNullHandle("@BOMDate", BOMDate);
                                cmdRIFB33.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                SqlDataAdapter reportDataAdapt33 = new SqlDataAdapter(cmdRIFB33);
                                reportDataAdapt33.Fill(dataTable33);

                                if (dataTable33 == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable33.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {
                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;

                                    string rwUom = "";
                                    decimal vConvertionRate = 0;
                                    decimal AvgRate = 0;


                                    foreach (DataRow BRItem in dataTable33.Rows)
                                    {
                                        #region Declare
                                        decimal v1Quantity = 0;
                                        string v1RawItemNo = "";
                                        string v1UOM = "";
                                        decimal v1SubTotal = 0;

                                        decimal v1UOMQty = 0;
                                        decimal v1UOMPrice = 0;
                                        decimal v1UOMc = 0;
                                        string v1UOMn = "";
                                        #endregion DECLARE

                                        #region Update Item Qty

                                        #region Find Quantity From Products


                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);
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

                                        decimal nbrPriceFromBom = 0;
                                        nbrPriceFromBom = productDal.GetLastNBRPriceFromBOM(BRItem["RawItemNo"].ToString(),
                                                                 "VAT 1", Master.ReceiveDateTime,
                                                                 null, null);
                                        if (nbrPriceFromBom == 0)
                                        {
                                            nbrPriceFromBom = productDal.GetLastNBRPriceFromBOM(BRItem["RawItemNo"].ToString(),
                                                                 "VAT 1 Ka (Tarrif)", Master.ReceiveDateTime,
                                                                 null, null);
                                            if (nbrPriceFromBom == 0)
                                            {
                                                nbrPriceFromBom = productDal.GetLastNBRPriceFromBOM(BRItem["RawItemNo"].ToString(),
                                                                     "VAT 1 Kha (Trading)", Master.ReceiveDateTime,
                                                                     null, null);

                                            }

                                        }




                                        #endregion Find Quantity From Products

                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                        vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                        vWastage = FormatingNumeric(vWastage, IssuePlaceQty);

                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }

                                        /*Processing UOM*/

                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);

                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        #endregion valueAssign
                                        #region Stock Check
                                        if (NegStockAllow == false)
                                        {
                                            #region Find Quantity From Products

                                            decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(v1RawItemNo,
                                                                            Master.ReceiveDateTime,
                                                          currConn, transaction, false).Rows[0]["Quantity"].ToString());

                                            #endregion Find Quantity From Products

                                            #region Find Quantity From Transaction

                                            sqlText = "";
                                            sqlText +=
                                                "select isnull(UOMQty,isnull(Quantity ,0)) from SalesInvoiceDetails ";
                                            sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                                       "and SalesInvoiceNo=@MasterReceiveNo";
                                            SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                            cmdBRItemTranQty.Transaction = transaction;

                                            cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                            decimal BRItemTranQty = 0;
                                            BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                            #endregion Find Quantity From Transaction



                                            if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                            {
                                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                                MessageVM.
                                                                                    receiveMsgStockNotAvailablePost);
                                            }
                                        }
                                        #endregion Stock Check

                                        sqlText = "";

                                        sqlText += " update SalesInvoiceDetails set ";

                                        sqlText += " InvoiceLineNo          =@ItemReceiveLineNo ,";
                                        sqlText += " Quantity               =@v1Quantity ,";
                                        sqlText += " PromotionalQuantity    ='0' ,";
                                        sqlText += " SalesPrice             =@nbrPriceFromBom ,";
                                        sqlText += " NBRPrice               =@nbrPriceFromBom ,";
                                        sqlText += " AVGPrice               =@AvgRate ,";
                                        sqlText += " UOM                    =@v1UOM ,";
                                        sqlText += " VATRate                ='0' ,";
                                        sqlText += " VATAmount              ='0' ,";
                                        sqlText += " SubTotal               =@v1SubTotal ,";
                                        sqlText += " Comments               =@ItemCommentsD ,";
                                        sqlText += " LastModifiedBy         =@MasterLastModifiedBy ,";
                                        sqlText += " LastModifiedOn         =@MasterLastModifiedOn ,";
                                        sqlText += " SD                     ='0' ,";
                                        sqlText += " SDAmount               ='0' ,";
                                        sqlText += " SaleType               ='New' ,";
                                        sqlText += " PreviousSalesInvoiceNo =@MasterReturnId ,";
                                        sqlText += " Trading                ='N' ,";
                                        sqlText += " NonStock               ='N' ,";
                                        sqlText += " TradingMarkUp          ='0' ,";
                                        sqlText += " InvoiceDateTime        =@MasterReceiveDateTime ,";
                                        sqlText += " UOMQty                 =@v1UOMQty ,";
                                        sqlText += " UOMn                   =@v1UOMn ,";
                                        sqlText += " UOMc                   =@v1UOMc ,";
                                        sqlText += " UOMPrice               =@v1UOMPrice ,";
                                        sqlText += " Type                   ='VAT' ,";
                                        sqlText += " TransactionType        =@MasterTransactionType ,";
                                        sqlText += " SaleReturnId           =@MasterReturnId ,";
                                        sqlText += " DiscountAmount         = '0',";
                                        sqlText += " DiscountedNBRPrice     =@nbrPriceFromBom,";
                                        sqlText += " DollerValue            = '0',";
                                        sqlText += " CurrencyValue          =@nbrPriceFromBom,";
                                        sqlText += " FinishItemNo           =@ItemItemNo,";
                                        sqlText += " Post                   =@MasterPost";
                                        sqlText += " where  SalesInvoiceNo  =@MasterReceiveNo ";
                                        sqlText += " and ItemNo             =@v1RawItemNo";
                                        sqlText += " and FinishItemNo       =@ItemItemNo";


                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;

                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@nbrPriceFromBom", nbrPriceFromBom);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@nbrPriceFromBom", nbrPriceFromBom);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@nbrPriceFromBom", nbrPriceFromBom);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@nbrPriceFromBom", nbrPriceFromBom);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                            MessageVM.receiveMsgUnableToUpdateIssue);
                                        }

                                        #endregion Qty  check and Update

                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock

                                #region Update Sale Header

                                sqlText = "";
                                sqlText += " update SalesInvoiceHeaders set ";
                                sqlText += " TotalAmount= (select sum(Quantity*NBRPrice) from SalesInvoiceDetails";
                                sqlText += "  where SalesInvoiceDetails.SalesInvoiceNo =SalesInvoiceHeaders.SalesInvoiceNo)";
                                sqlText += " where (SalesInvoiceHeaders.SalesInvoiceNo=@MasterReceiveNo)";

                                SqlCommand cmdUpdateSale1 = new SqlCommand(sqlText, currConn);
                                cmdUpdateSale1.Transaction = transaction;

                                cmdUpdateSale1.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);


                                transResult = (int)cmdUpdateSale1.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                    MessageVM.receiveMsgUnableToUpdateIssue);
                                }

                                #endregion Update Sale Header
                                #endregion sale
                                #region issue
                                #region Update to Issue



                                #region Find Raw Item From BOM  and update Stock



                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,b.UOMUQty,b.UOMWQty,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity,b.UOM from BOMRaws b  ";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and vatname='VAT 1 (Package)' ";
                                sqlText += " and effectdate=@BOMDate";
                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID  ";
                                }


                                sqlText += "   and (rawitemtype='raw' or rawitemtype='pack' or rawitemtype='trading') ";


                                DataTable dataTableS = new DataTable("RIFB");
                                SqlCommand cmdRIFBs = new SqlCommand(sqlText, currConn);
                                cmdRIFBs.Transaction = transaction;

                                cmdRIFBs.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdRIFBs.Parameters.AddWithValueAndNullHandle("@BOMDate", BOMDate);
                                cmdRIFBs.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFBs);
                                reportDataAdapt.Fill(dataTableS);

                                if (dataTableS == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTableS.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {
                                    decimal vQuantity = 0;
                                    decimal vWastage = 0;

                                    string rwUom = "";
                                    decimal vConvertionRate = 0;
                                    decimal AvgRate = 0;


                                    foreach (DataRow BRItem in dataTableS.Rows)
                                    {
                                        #region Declare
                                        decimal v1Quantity = 0;
                                        string v1RawItemNo = "";
                                        decimal v1CostPrice = 0;
                                        string v1UOM = "";
                                        decimal v1SubTotal = 0;
                                        decimal v1Wastage = 0;
                                        DateTime v1BOMDate = DateTime.Now.Date;
                                        string v1FinishItemNo = "";

                                        decimal v1UOMQty = 0;
                                        decimal v1UOMPrice = 0;
                                        decimal v1UOMc = 0;
                                        string v1UOMn = "";
                                        string v1BOMId = "";
                                        decimal v1UOMWastage = 0;
                                        #endregion DECLARE

                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        DataTable priceData = productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(), Master.ReceiveDateTime, currConn, transaction, false);
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


                                        #endregion Find Quantity From Products

                                        vQuantity = Convert.ToDecimal(BRItem["UseQuantity"].ToString());
                                        vWastage = Convert.ToDecimal(BRItem["WastageQuantity"].ToString());

                                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                                        vQuantity = FormatingNumeric(vQuantity, IssuePlaceQty);
                                        vWastage = FormatingNumeric(vWastage, IssuePlaceQty);

                                        rwUom = BRItem["Uom"].ToString();
                                        var rwUomMajor = BRItem["Uomn"].ToString();
                                        if (string.IsNullOrEmpty(rwUom))
                                        {
                                            throw new ArgumentNullException("ReceiveInsert", "Could not find UOM of raw item");
                                        }

                                        /*Processing UOM*/

                                        UOMDAL uomdal = new UOMDAL();
                                        vConvertionRate = uomdal.GetConvertionRate(rwUomMajor, rwUom, "Y", currConn, transaction);

                                        #region valueAssign
                                        v1Quantity = (vQuantity + vWastage) * Item.UOMQty;
                                        v1Wastage = (vWastage) * Item.UOMQty;
                                        v1BOMId = Item.BOMId;
                                        v1RawItemNo = BRItem["RawItemNo"].ToString();
                                        v1UOM = BRItem["UOM"].ToString();
                                        v1CostPrice = AvgRate * vConvertionRate;
                                        v1SubTotal = (vQuantity + vWastage) * Item.UOMQty * (AvgRate * vConvertionRate);

                                        v1UOMPrice = AvgRate;
                                        v1UOMn = BRItem["UOMn"].ToString();
                                        v1BOMDate = Convert.ToDateTime(BRItem["EffectDate"].ToString());
                                        v1FinishItemNo = Item.ItemNo;
                                        v1UOMc = vConvertionRate;
                                        v1UOMQty = (vQuantity + vWastage) * Item.UOMQty * vConvertionRate;
                                        v1UOMWastage = (vWastage) * Item.UOMQty * vConvertionRate;
                                        #endregion valueAssign
                                        #region Stock Check
                                        if (NegStockAllow == false)
                                        {
                                            #region Find Quantity From Products


                                            decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(v1RawItemNo,
                                                                             Master.ReceiveDateTime,
                                                           currConn, transaction, false).Rows[0]["Quantity"].ToString());

                                            #endregion Find Quantity From Products

                                            #region Find Quantity From Transaction

                                            sqlText = "";
                                            sqlText +=
                                                "select isnull(UOMQty,isnull(Quantity ,0)) from IssueDetails ";
                                            sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                                       "and IssueNo=@MasterReceiveNo";
                                            SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                            cmdBRItemTranQty.Transaction = transaction;

                                            cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                            decimal BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                            #endregion Find Quantity From Transaction



                                            if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                            {
                                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                                MessageVM.
                                                                                    receiveMsgStockNotAvailablePost);
                                            }
                                        }
                                        #endregion Stock Check



                                        sqlText = "";
                                        sqlText += " update IssueDetails set";
                                        sqlText += " IssueLineNo        =@ItemReceiveLineNo,";
                                        sqlText += " Comments           =@ItemCommentsD,";
                                        sqlText += " LastModifiedBy     =@MasterLastModifiedBy,";
                                        sqlText += " LastModifiedOn     =@MasterLastModifiedOn,";
                                        sqlText += " IssueDateTime      =@MasterReceiveDateTime,";
                                        sqlText += " Post               =@issueAutoPostValue,";
                                        sqlText += " uom                =@v1UOM,";
                                        sqlText += " Quantity           =@v1Quantity,";
                                        sqlText += " Wastage            =@v1Wastage,";
                                        sqlText += " BOMDate            =@v1BOMDate,";
                                        sqlText += " CostPrice          =@v1CostPrice,";
                                        sqlText += " NBRPrice           =@AvgRate,";
                                        sqlText += " transactionType    =@MasterTransactionType,";
                                        sqlText += " IssueReturnId      =@MasterReturnId,";
                                        sqlText += " UOMQty             =@v1UOMQty,";
                                        sqlText += " UOMPrice           =@v1UOMPrice,";
                                        sqlText += " UOMc               =@v1UOMc,";
                                        sqlText += " UOMn               =@v1UOMn,";
                                        sqlText += " UOMWastage         =@v1UOMWastage,";
                                        sqlText += " BOMId              =@v1BOMId,";
                                        sqlText += " SubTotal           =@v1SubTotal";
                                        sqlText += " WHERE FinishItemNo =@v1FinishItemNo  AND ItemNo=@v1RawItemNo ";
                                        sqlText += " and  IssueNo=@MasterReceiveNo";

                                        SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                        cmdInsertIssue.Transaction = transaction;

                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemReceiveLineNo", Item.ReceiveLineNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemCommentsD", Item.CommentsD);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@issueAutoPostValue", issueAutoPostValue);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOM", v1UOM);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Quantity", v1Quantity);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1Wastage", v1Wastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMDate", v1BOMDate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1CostPrice", v1CostPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@AvgRate", AvgRate);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterTransactionType", Master.transactionType);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMQty", v1UOMQty);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMPrice", v1UOMPrice);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMc", v1UOMc);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMn", v1UOMn);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1UOMWastage", v1UOMWastage);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1BOMId", v1BOMId);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1SubTotal", v1SubTotal);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1FinishItemNo", v1FinishItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@v1RawItemNo", v1RawItemNo);
                                        cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                        transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                        if (transResult <= 0)
                                        {
                                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                            MessageVM.receiveMsgUnableToUpdateIssue);
                                        }

                                        #endregion Qty  check and Update

                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock


                                #endregion Update to Issue

                                #region Update Issue Header

                                sqlText = "";
                                sqlText += " update IssueHeaders set ";
                                sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                sqlText += " where (IssueHeaders.IssueNo= @MasterReceiveNo)";

                                SqlCommand cmdUpdateSale = new SqlCommand(sqlText, currConn);
                                cmdUpdateSale.Transaction = transaction;

                                cmdUpdateSale.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                transResult = (int)cmdUpdateSale.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                    MessageVM.receiveMsgUnableToUpdateIssue);
                                }

                                #endregion Update Issue Header
                                #endregion Issue
                            }



                            #endregion Transaction is Trading
                        }//if (Master.FromBOM == "Y")

                        #endregion Transaction is FromBOM

                        #endregion Update Issue and Receive if Transaction is not Other
                    }

                    #endregion Find Transaction Mode Insert or Update
                }//foreach (var Item in Details.ToList())

                #region Remove row
                sqlText = "";
                sqlText += " SELECT  distinct ItemNo";
                sqlText += " from ReceiveDetails WHERE ReceiveNo=@MasterReceiveNo";

                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIF = new SqlCommand(sqlText, currConn);
                cmdRIF.Transaction = transaction;
                cmdRIF.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                SqlDataAdapter dta = new SqlDataAdapter(cmdRIF);
                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var p = pItem["ItemNo"].ToString();

                    //var tt= Details.Find(x => x.ItemNo == p);
                    var tt = Details.Count(x => x.ItemNo.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM ReceiveDetails ";
                        sqlText += " WHERE ReceiveNo=@MasterReceiveNo ";
                        sqlText += " AND ItemNo='" + p + "'";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                        if (Master.FromBOM == "Y")
                        {
                            if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn"
                               || Master.transactionType == "WIP" || Master.transactionType == "Tender" || Master.transactionType == "TollFinishReceive"
                               || Master.transactionType == "PackageProduction")
                            {
                                sqlText = "";
                                sqlText += " delete FROM IssueDetails ";
                                sqlText += " WHERE IssueNo=@MasterReceiveNo ";
                                sqlText += " AND FinishItemNo='" + p + "'";
                                SqlCommand cmdInsDetail13 = new SqlCommand(sqlText, currConn);
                                cmdInsDetail13.Transaction = transaction;

                                cmdInsDetail13.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                transResult = (int)cmdInsDetail13.ExecuteNonQuery();
                            }
                            if (Master.transactionType == "PackageProduction")
                            {

                                sqlText = "";
                                sqlText += " delete FROM SalesInvoiceDetails ";
                                sqlText += " WHERE SalesInvoiceNo=@MasterReceiveNo ";
                                sqlText += " AND FinishItemNo='" + p + "'";
                                SqlCommand cmdInsDetail2 = new SqlCommand(sqlText, currConn);
                                cmdInsDetail2.Transaction = transaction;

                                cmdInsDetail2.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                transResult = (int)cmdInsDetail2.ExecuteNonQuery();
                            }
                        }

                    }

                }

                #endregion Remove row




                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)
                #region Tracking
                if (Trackings != null && Trackings.Count > 0)
                {

                    for (int i = 0; i < Trackings.Count; i++)
                    {
                        if (Master.transactionType == "ReceiveReturn")
                        {
                            if (Trackings[i].ReturnReceive == "Y")
                            {
                                Trackings[i].ReturnReceiveID = Master.ReceiveNo;
                                Trackings[i].ReturnType = Master.transactionType;
                            }

                        }
                        else if (Trackings[i].IsReceive == "Y")
                        {
                            Trackings[i].ReceiveNo = Master.ReceiveNo;
                            Trackings[i].ReceiveDate = Master.ReceiveDateTime;
                            //Trackings[i].Post = Master.Post;
                        }
                        else
                        {
                            Trackings[i].ReceiveNo = "";

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

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from ReceiveHeaders WHERE ReceiveNo=@MasterReceiveNo ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;

                cmdIPS.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate, MessageVM.receiveMsgUnableCreatID);
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
                retResults[1] = MessageVM.receiveMsgUpdateSuccessfully;
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
        public string[] ReceivePost(ReceiveMasterVM Master, List<ReceiveDetailVM> Details, List<TrackingVM> Trackings)
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
            bool issueAutoPost = false;
            string issueAutoPostValue = "N";
            DateTime receiveDate = DateTime.MinValue;
            string receiveDateFormat = "";
            DateTime BOMDate = DateTime.MinValue; //start
            string BomId = string.Empty;

            #endregion Initializ

            #region Try
            try
            {
                string vNegStockAllow, vIssueAutoPost = string.Empty;
                CommonDAL commonDal = new CommonDAL();
                vNegStockAllow = commonDal.settings("Issue", "NegStockAllow");
                vIssueAutoPost = commonDal.settings("IssueFromBOM", "IssueAutoPost");
                if (string.IsNullOrEmpty(vNegStockAllow)
                    || string.IsNullOrEmpty(vIssueAutoPost)
                    )
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Sale");
                }
                bool NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);
                issueAutoPost = Convert.ToBoolean(vIssueAutoPost == "Y" ? true : false);
                if (issueAutoPost)
                    issueAutoPostValue = "Y";

                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.ReceiveDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.ReceiveDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                    MessageVM.receiveMsgCheckDatePost);

                }



                #endregion Validation for Header

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }



                transaction = currConn.BeginTransaction(MessageVM.receiveMsgMethodNamePost);

                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionDate = Master.ReceiveDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.ReceiveDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(ReceiveNo) from ReceiveHeaders WHERE ReceiveNo=@MasterReceiveNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;

                cmdFindIdUpd.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                    MessageVM.receiveMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header

                sqlText = "";

                sqlText += " update ReceiveHeaders set  ";
                sqlText += " LastModifiedBy=@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn=@MasterLastModifiedOn ,";
                sqlText += " Post=@MasterPost ";
                sqlText += " where  ReceiveNo=@MasterReceiveNo";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                    MessageVM.receiveMsgPostNotSuccessfully);
                }

                #endregion update Header

                #region Transaction is FromBOM

                if (Master.FromBOM == "Y")
                {
                    if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn"
                        || Master.transactionType == "WIP" || Master.transactionType == "TollFinishReceive"
                        || Master.transactionType == "Tender")
                    {
                        #region update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " LastModifiedBy=@MasterLastModifiedBy ,";
                        sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                        sqlText += " Post= '" + issueAutoPostValue + "' ";
                        sqlText += " where  IssueNo=@MasterReceiveNo";


                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;

                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        //cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                        #endregion update Issue
                    }
                    if (Master.transactionType == "PackageProduction")
                    {
                        #region update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " LastModifiedBy=@MasterLastModifiedBy ,";
                        sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                        sqlText += " Post= '" + issueAutoPostValue + "' ";
                        sqlText += " where  IssueNo=@MasterReceiveNo ";


                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssue.Transaction = transaction;

                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        //cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();


                        #endregion update Issue

                        #region update Issue

                        sqlText = "";
                        sqlText += " update SalesInvoiceHeaders set ";
                        sqlText += " LastModifiedBy=@MasterLastModifiedBy  ,";
                        sqlText += " LastModifiedOn=@MasterLastModifiedOn ,";
                        sqlText += " Post= '" + issueAutoPostValue + "' ";
                        sqlText += " where  SalesInvoiceNo= @MasterReceiveNo";


                        SqlCommand cmdUpdateIssueS = new SqlCommand(sqlText, currConn);
                        cmdUpdateIssueS.Transaction = transaction;

                        cmdUpdateIssueS.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdUpdateIssueS.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        //cmdUpdateIssue.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdUpdateIssueS.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                        transResult = (int)cmdUpdateIssueS.ExecuteNonQuery();


                        #endregion update Issue
                    }
                }
                #endregion Transaction is FromBOM



                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail
                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                        MessageVM.receiveMsgNoDataToPost);
                }
                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(ReceiveNo) from ReceiveDetails WHERE ReceiveNo=@MasterReceiveNo ";
                    sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {

                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                            MessageVM.receiveMsgNoDataToPost);
                    }
                    else
                    {
                        #region Update only DetailTable
                        sqlText = "";
                        sqlText += " update ReceiveDetails set ";
                        sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                        sqlText += " Post=@MasterPost";
                        sqlText += " where ReceiveNo=@MasterReceiveNo ";
                        sqlText += " and 	ItemNo=@ItemItemNo ";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable
                        #region Update Issue and Receive if Transaction is not Other

                        //if (Master.transactionType == "ReceiveReturn")

                        //    receiveDate = previousReceiveDate.Date;
                        //else
                        receiveDate = Convert.ToDateTime(Master.ReceiveDateTime).Date;
                        receiveDateFormat = receiveDate.ToString("yyyy/MM/dd");
                        #region TollReceive
                        if (Master.transactionType == "ReceiveReturn" && Item.ReturnTransactionType == "TollReceive")
                        {
                            ProductDAL productDal = new ProductDAL();
                            DataTable TollItemInfo = productDal.SearchRawItemNo(Master.ReturnId);

                            string TollItem;
                            decimal TollUnitCost = 0;

                            TollItem = TollItemInfo.Rows[0]["ItemNo"].ToString();
                            TollUnitCost = Convert.ToDecimal(TollItemInfo.Rows[0]["CostPrice"].ToString());

                            #region Update to Issue

                            sqlText = "";
                            sqlText += " update IssueDetails set";
                            sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                            sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                            sqlText += " Post='" + issueAutoPostValue + "'";
                            sqlText += " WHERE  IssueNo =@MasterReceiveNo";
                            sqlText += "  and IssueDetails.ItemNo =@TollItem ";
                            SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                            cmdInsertIssue.Transaction = transaction;

                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                            //cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                            cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@TollItem", TollItem);

                            transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                MessageVM.receiveMsgUnableToIssuePost);
                            }

                            #endregion Update to Issue

                        }

                        #endregion TollReceive


                        #region Transaction is FromBOM
                        if (Master.FromBOM == "Y")
                        {
                            BomId = string.Empty;
                            BOMDate = DateTime.MinValue;
                            string bomDateFormat = "";

                            #region Last BOMId

                            sqlText = "  ";
                            sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname=@ItemVatName ";
                            sqlText += " and effectdate<=@receiveDateFormat";
                            sqlText += " and post='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID ";
                            }

                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                            cmdBomId.Transaction = transaction;

                            cmdBomId.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomId.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdBomId.Parameters.AddWithValueAndNullHandle("@receiveDateFormat", receiveDateFormat);
                            cmdBomId.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            if (cmdBomId.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BomId = "0";
                            }
                            else
                            {
                                BomId = (string)cmdBomId.ExecuteScalar();
                            }

                            #endregion Last BOMId

                            #region Last BOMDate

                            sqlText = "  ";
                            sqlText += " select top 1 isnull(EffectDate,'1900/01/01') from BOMs";
                            sqlText += " where ";
                            sqlText += " FinishItemNo=@ItemItemNo ";
                            sqlText += " and vatname=@ItemVatName ";
                            sqlText += " and effectdate<=@receiveDateFormat ";
                            sqlText += " and post='Y' ";
                            if (Master.CustomerID != "0")
                            {
                                sqlText += " and CustomerID=@MasterCustomerID ";
                            }

                            sqlText += " order by effectdate desc ";

                            SqlCommand cmdBomEDate = new SqlCommand(sqlText, currConn);
                            cmdBomEDate.Transaction = transaction;

                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@receiveDateFormat", receiveDateFormat);
                            cmdBomEDate.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                            if (cmdBomEDate.ExecuteScalar() == null)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                "No Price declaration found for this item");
                                BOMDate = DateTime.MinValue;
                                bomDateFormat = BOMDate.Date.ToString("yyyy-MM-dd 00:00:00");

                            }
                            else
                            {
                                BOMDate = (DateTime)cmdBomEDate.ExecuteScalar();
                                bomDateFormat = BOMDate.Date.ToString("yyyy/MM/dd 00:00:00");

                            }

                            #endregion Last BOMDate

                            if (Master.transactionType == "Other" || Master.transactionType == "ReceiveReturn"
                                || Master.transactionType == "WIP" || Master.transactionType == "Tender")
                            {
                                #region Update to Issue

                                sqlText = "";
                                sqlText += " update IssueDetails set";
                                sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                                sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                                sqlText += " Post='" + issueAutoPostValue + "'";
                                sqlText += " WHERE  IssueNo =@MasterReceiveNo";
                                sqlText += "  and IssueDetails.FinishItemNo = @ItemItemNo";
                                SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                cmdInsertIssue.Transaction = transaction;

                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                                transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgUnableToIssuePost);
                                }

                                #endregion Update to Issue

                                #region Find Raw Item From BOM  and update Stock

                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity from BOMRaws b  ";
                                sqlText += " WHERE ";

                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and vatname=@ItemVatName ";
                                sqlText += " and effectdate=@bomDateFormat";

                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";

                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID ";
                                }
                                sqlText += "   and (rawitemtype='raw' or rawitemtype='pack') ";

                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;

                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemVatName", Item.VatName);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@bomDateFormat", bomDateFormat);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {
                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {
                                        #region Update Item Qty
                                        #region Qty  check and Update

                                        if (NegStockAllow == false)
                                        {
                                            #region Find Quantity From Products

                                            ProductDAL productDal = new ProductDAL();
                                            //decimal BRItemoldStock =
                                            //    productDal.StockInHand(BRItem["RawItemNo"].ToString(),
                                            //                           Master.ReceiveDateTime, currConn,
                                            //                           transaction);

                                            decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(),
                                                                            Master.ReceiveDateTime,
                                                          currConn, transaction, true).Rows[0]["Quantity"].ToString());

                                            #endregion Find Quantity From Products

                                            #region Find Quantity From Transaction

                                            sqlText = "";
                                            sqlText +=
                                                "select isnull(isnull(Quantity ,0)+isnull(Wastage ,0),0) from IssueDetails ";
                                            sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                                       "and IssueNo= @MasterReceiveNo";
                                            SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                            cmdBRItemTranQty.Transaction = transaction;

                                            cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                            decimal BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                            #endregion Find Quantity From Transaction


                                            if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                            {
                                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                                MessageVM.
                                                                                    receiveMsgStockNotAvailablePost);
                                            }
                                        }



                                        #endregion Qty  check and Update

                                        #endregion Qty  check and Update
                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock
                            }
                            #region  TollFinishReceive

                            else if (Master.transactionType == "TollFinishReceive")
                            {
                                ProductDAL productDal1 = new ProductDAL();
                                BomId = string.Empty;  //BOMId
                                BOMDate = DateTime.MinValue;
                                #region Last BOMId
                                sqlText = "  ";
                                sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                                sqlText += " and effectdate<=@receiveDateDate";
                                sqlText += " and post='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID ";
                                }

                                sqlText += " order by effectdate desc ";

                                SqlCommand cmdBomId1 = new SqlCommand(sqlText, currConn);
                                cmdBomId1.Transaction = transaction;

                                cmdBomId1.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdBomId1.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                                cmdBomId1.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                if (cmdBomId1.ExecuteScalar() == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    "No Price declaration found for this item");
                                    BomId = "0";
                                }
                                else
                                {
                                    BomId = (string)cmdBomId1.ExecuteScalar();
                                }

                                #endregion Last BOMId

                                #region Last BOMDate
                                sqlText = "  ";
                                sqlText += " select top 1 isnull(EffectDate,'1900/01/01') from BOMs";
                                sqlText += " where ";
                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and vatname='VAT 1 (Toll Issue)' ";
                                sqlText += " and effectdate<=@receiveDateDate";
                                sqlText += " and post='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID ";
                                }

                                sqlText += " order by effectdate desc ";

                                SqlCommand cmdBomEDate1 = new SqlCommand(sqlText, currConn);
                                cmdBomEDate1.Transaction = transaction;

                                cmdBomEDate1.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdBomEDate1.Parameters.AddWithValueAndNullHandle("@receiveDateDate", receiveDate.Date);
                                cmdBomEDate1.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                if (cmdBomEDate1.ExecuteScalar() == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert,
                                                                    "No Price declaration found for this item");
                                    BOMDate = DateTime.MinValue;
                                }
                                else
                                {
                                    BOMDate = (DateTime)cmdBomEDate1.ExecuteScalar();
                                }

                                #endregion Last BOMDate

                                #region Update to Issue

                                sqlText = "";
                                sqlText += " update IssueDetails set";
                                sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                                sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                                sqlText += " Post='" + issueAutoPostValue + "'";
                                sqlText += " WHERE  IssueNo =@MasterReceiveNo";
                                sqlText += "  and IssueDetails.FinishItemNo = @ItemItemNo";
                                SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                cmdInsertIssue.Transaction = transaction;

                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);

                                transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgUnableToIssuePost);
                                }

                                #endregion Update to Issue

                                #region Find Raw Item From BOM  and update Stock


                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity from BOMRaws b  ";
                                sqlText += " WHERE ";

                                sqlText += " FinishItemNo=@ItemItemNo ";
                                sqlText += " and b.Vatname='VAT 1 (Toll Issue)'";
                                sqlText += " and effectdate=@BOMDateDate";
                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";
                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID ";
                                }
                                sqlText += "   and (rawitemtype='raw' or rawitemtype='pack') ";

                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;

                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@BOMDateDate", BOMDate.Date);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {
                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {
                                        #region Update Item Qty

                                        #region Find Quantity From Products

                                        ProductDAL productDal = new ProductDAL();
                                        //decimal BRItemoldStock = productDal.StockInHand(Item.ItemNo,
                                        //                                                Master.ReceiveDateTime, currConn,
                                        //                                                transaction);

                                        decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(Item.ItemNo,
                                                                            Master.ReceiveDateTime,
                                                          currConn, transaction, true).Rows[0]["Quantity"].ToString());

                                        #endregion Find Quantity From Products

                                        #region Find Quantity From Transaction

                                        sqlText = "";
                                        sqlText +=
                                            "select isnull(isnull(Quantity ,0)+isnull(Wastage ,0),0) from IssueDetails ";
                                        sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                                   "and IssueNo=@MasterReceiveNo";
                                        SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                        cmdBRItemTranQty.Transaction = transaction;

                                        cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                        decimal BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                        #endregion Find Quantity From Transaction

                                        #region Qty  check and Update

                                        if (NegStockAllow == false)
                                        {
                                            if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                            {
                                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                                MessageVM.
                                                                                    receiveMsgStockNotAvailablePost);
                                            }
                                        }


                                        #endregion Qty  check and Update

                                        #endregion Qty  check and Update
                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock
                            }
                            #endregion TollFinishReceive

                            else if (Master.transactionType == "PackageProduction")
                            {
                                #region Update to Issue

                                sqlText = "";
                                sqlText += " update IssueDetails set";
                                sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                                sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                                sqlText += " Post='" + issueAutoPostValue + "'";
                                sqlText += " WHERE  IssueNo =@MasterReceiveNo";
                                SqlCommand cmdInsertIssue21 = new SqlCommand(sqlText, currConn);
                                cmdInsertIssue21.Transaction = transaction;

                                cmdInsertIssue21.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                cmdInsertIssue21.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                cmdInsertIssue21.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                transResult = (int)cmdInsertIssue21.ExecuteNonQuery();



                                #endregion Update to Issue

                                #region Find Raw Item From BOM  and update Stock

                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity from BOMRaws b  ";
                                sqlText += " WHERE ";

                                sqlText += " FinishItemNo=@ItemItemNo  ";
                                sqlText += " and vatname='VAT 1 (Package)' ";
                                sqlText += " and effectdate=@BOMDateDate";
                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";

                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID  ";
                                }

                                sqlText += "   and (rawitemtype='raw' or rawitemtype='pack'  or rawitemtype='Trading') ";

                                DataTable dataTable21 = new DataTable("RIFB");
                                SqlCommand cmdRIFB21 = new SqlCommand(sqlText, currConn);
                                cmdRIFB21.Transaction = transaction;

                                cmdRIFB21.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdRIFB21.Parameters.AddWithValueAndNullHandle("@BOMDateDate", BOMDate.Date);
                                cmdRIFB21.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                SqlDataAdapter reportDataAdapt21 = new SqlDataAdapter(cmdRIFB21);
                                reportDataAdapt21.Fill(dataTable21);

                                if (dataTable21 == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable21.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {
                                    foreach (DataRow BRItem in dataTable21.Rows)
                                    {

                                        #region Qty  check and Update

                                        if (NegStockAllow == false)
                                        {
                                            #region Find Quantity From Products

                                            ProductDAL productDal = new ProductDAL();


                                            decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(),
                                                                            Master.ReceiveDateTime,
                                                          currConn, transaction, true).Rows[0]["Quantity"].ToString());

                                            #endregion Find Quantity From Products

                                            #region Find Quantity From Transaction

                                            sqlText = "";
                                            sqlText +=
                                                "select isnull(isnull(Quantity ,0)+isnull(Wastage ,0),0) from IssueDetails ";
                                            sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                                       "and IssueNo= @MasterReceiveNo ";
                                            SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                            cmdBRItemTranQty.Transaction = transaction;

                                            cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                            decimal BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                            #endregion Find Quantity From Transaction


                                            if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                            {
                                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                                MessageVM.
                                                                                    receiveMsgStockNotAvailablePost);
                                            }
                                        }



                                        #endregion Qty  check and Update

                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock

                                #region Update to Issue

                                sqlText = "";
                                sqlText += " update SalesInvoiceDetails set";
                                sqlText += " LastModifiedBy=@MasterLastModifiedBy,";
                                sqlText += " LastModifiedOn=@MasterLastModifiedOn,";
                                sqlText += " Post='" + issueAutoPostValue + "'";
                                sqlText += " WHERE  SalesInvoiceNo =@MasterReceiveNo";
                                SqlCommand cmdInsertIssue = new SqlCommand(sqlText, currConn);
                                cmdInsertIssue.Transaction = transaction;

                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                cmdInsertIssue.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                transResult = (int)cmdInsertIssue.ExecuteNonQuery();

                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgUnableToIssuePost);
                                }

                                #endregion Update to Issue

                                #region Find Raw Item From BOM  and update Stock

                                sqlText = "";
                                sqlText +=
                                    " SELECT   b.RawItemNo,b.UseQuantity,convert (date,b.EffectDate,120)EffectDate,b.UOMn ,b.WastageQuantity from BOMRaws b  ";
                                sqlText += " WHERE ";

                                sqlText += " FinishItemNo=@ItemItemNo  ";
                                sqlText += " and vatname='VAT 1 (Package)' ";
                                sqlText += " and effectdate=@BOMDateDate ";
                                sqlText += " and post='Y' ";
                                sqlText += " and isnull(IssueOnProduction,'Y')='Y' ";

                                if (Master.CustomerID != "0")
                                {
                                    sqlText += " and CustomerID=@MasterCustomerID ";
                                }

                                sqlText += "   and (rawitemtype='Finish') ";

                                DataTable dataTable = new DataTable("RIFB");
                                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                                cmdRIFB.Transaction = transaction;

                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@BOMDateDate", BOMDate.Date);
                                cmdRIFB.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                                reportDataAdapt.Fill(dataTable);

                                if (dataTable == null)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else if (dataTable.Rows.Count <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                    MessageVM.receiveMsgNoDataToPost);
                                }
                                else
                                {
                                    foreach (DataRow BRItem in dataTable.Rows)
                                    {
                                        #region Update Item Qty
                                        #region Qty  check and Update

                                        if (NegStockAllow == false)
                                        {
                                            #region Find Quantity From Products

                                            ProductDAL productDal = new ProductDAL();


                                            decimal BRItemoldStock = Convert.ToDecimal(productDal.AvgPriceNew(BRItem["RawItemNo"].ToString(),
                                                                            Master.ReceiveDateTime,
                                                          currConn, transaction, true).Rows[0]["Quantity"].ToString());

                                            #endregion Find Quantity From Products

                                            #region Find Quantity From Transaction

                                            sqlText = "";
                                            sqlText +=
                                                "select isnull(Quantity ,0) from SalesInvoiceDetails ";
                                            sqlText += " WHERE ItemNo='" + BRItem["RawItemNo"].ToString() + "' " +
                                                       "and SalesInvoiceNo= @MasterReceiveNo";
                                            SqlCommand cmdBRItemTranQty = new SqlCommand(sqlText, currConn);
                                            cmdBRItemTranQty.Transaction = transaction;

                                            cmdBRItemTranQty.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                                            decimal BRItemTranQty = (decimal)cmdBRItemTranQty.ExecuteScalar();

                                            #endregion Find Quantity From Transaction


                                            if (BRItemTranQty > (BRItemoldStock + BRItemTranQty))
                                            {
                                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                                                MessageVM.
                                                                                    receiveMsgStockNotAvailablePost);
                                            }
                                        }



                                        #endregion Qty  check and Update

                                        #endregion Qty  check and Update
                                    }
                                }

                                #endregion Find Raw Item From BOM and update Stock
                            }

                        }

                        #endregion Transaction is FromBOM





                        #endregion Update Issue and Receive if Transaction is not Other
                    }

                    #endregion Find Transaction Mode Insert or Update
                }


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)

                #region Tracking
                if (Trackings != null && Trackings.Count > 0)
                {
                    for (int i = 0; i < Trackings.Count; i++)
                    {
                        if (Trackings[i].ReceiveNo == Master.ReceiveNo)
                        {
                            sqlText = "";

                            sqlText += " update Trackings set  ";
                            sqlText += " LastModifiedBy= @MasterLastModifiedBy, ";
                            sqlText += " LastModifiedOn= @MasterLastModifiedOn, ";
                            sqlText += " ReceivePost=@MasterPost ";
                            sqlText += " where  ReceiveNo= @MasterReceiveNo ";
                            sqlText += " and  Heading1= @TrackingsHeading1 ";

                            SqlCommand cmdUpdateTracking = new SqlCommand(sqlText, currConn);
                            cmdUpdateTracking.Transaction = transaction;

                            cmdUpdateTracking.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdUpdateTracking.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                            cmdUpdateTracking.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                            cmdUpdateTracking.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                            cmdUpdateTracking.Parameters.AddWithValueAndNullHandle("@TrackingsHeading1", Trackings[i].Heading1);

                            transResult = (int)cmdUpdateTracking.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }

                        }
                        else
                        {
                            if (Trackings[i].transactionType == "Receive_Return")
                            {
                                if (Trackings[i].ReturnReceive == "Y")
                                {
                                    sqlText = "";

                                    sqlText += " update Trackings set  ";
                                    sqlText += " LastModifiedBy=@MasterLastModifiedBy, ";
                                    sqlText += " LastModifiedOn=@MasterLastModifiedOn, ";
                                    sqlText += " ReturnReceiveDate=@MasterReceiveDateTime ";

                                    sqlText += " where Heading1=@TrackingsHeading1 ";


                                    SqlCommand cmdUpdateTracking = new SqlCommand(sqlText, currConn);
                                    cmdUpdateTracking.Transaction = transaction;

                                    cmdUpdateTracking.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                                    cmdUpdateTracking.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                                    cmdUpdateTracking.Parameters.AddWithValueAndNullHandle("@MasterReceiveDateTime", Master.ReceiveDateTime);
                                    cmdUpdateTracking.Parameters.AddWithValueAndNullHandle("@TrackingsHeading1", Trackings[i].Heading1);

                                    transResult = (int)cmdUpdateTracking.ExecuteNonQuery();
                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                                    }
                                }
                            }
                        }

                    }
                }


                #endregion
                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from ReceiveHeaders WHERE ReceiveNo=@MasterReceiveNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;

                cmdIPS.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost, MessageVM.receiveMsgPostNotSelect);
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
                retResults[1] = MessageVM.receiveMsgSuccessfullyPost;
                retResults[2] = Master.Id;
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
        public DataTable SearchReceiveHeaderDTNew(string ReceiveNo, string ReceiveDateFrom,
            string ReceiveDateTo, string SerialNo, string transactionType, string Post, string CustomerID = "0")
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("ReceiveHeader");

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

                sqlText += @"
                            SELECT
                            ReceiveNo, 
                            convert (varchar,ReceiveHeaders.ReceiveDateTime,120)ReceiveDateTime,
                            isnull(ReceiveHeaders.TotalAmount,0)TotalAmount,
                            isnull(ReceiveHeaders.TotalVATAmount,0)TotalVATAmount,
                            isnull(ReceiveHeaders.SerialNo,'N/A')SerialNo,
                            isnull(ReceiveHeaders.WithToll,'N')WithToll,
                            isnull(ReceiveHeaders.ReferenceNo,'-')ReferenceNo,
                            isnull(ReceiveHeaders.Comments,'N/A')Comments,Post,transactionType,
                            isnull(NULLIF(ReceiveHeaders.ReceiveReturnId,''),'NA')ReturnId,
                            isnull(ReceiveHeaders.ImportIDExcel,'') ImportIDExcel
                           ,isnull(ReceiveHeaders.CustomerID,0)CustomerID
                            ,isnull(c.CustomerName,'NA')CustomerName
                            FROM dbo.ReceiveHeaders
                           left outer join Customers c on ReceiveHeaders.CustomerID =c.CustomerID                 
                            
                            WHERE 
                                (ReceiveHeaders.ReceiveNo LIKE '%' +  @ReceiveNo + '%' OR @ReceiveNo IS NULL) 
                            AND (ReceiveHeaders.ReceiveDateTime>= @ReceiveDateFrom OR @ReceiveDateFrom IS NULL)
                            AND (ReceiveHeaders.ReceiveDateTime<dateadd(d,1, @ReceiveDateTo) OR @ReceiveDateTo IS NULL)
                            AND (ReceiveHeaders.SerialNo LIKE '%' + @SerialNo + '%' OR @SerialNo IS NULL)
                            
                            AND (ReceiveHeaders.Post LIKE '%' + @Post + '%' OR @Post IS NULL)

                            ";

                if (transactionType == "All")
                //{sqlText += " AND (PurchaseInvoiceHeaders.TransactionType LIKE '%' +@TransactionType + '%' OR @TransactionType IS NULL) ";}
                { sqlText += " AND (ReceiveHeaders.transactionType not in ('ReceiveReturn') ) "; }
                else
                { sqlText += " AND (ReceiveHeaders.transactionType='" + transactionType + "') "; }

                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(ReceiveHeaders.CustomerId,0)='" + CustomerID + "' ";
                }

                sqlText += " order by ReceiveHeaders.ReceiveDateTime desc";

                #endregion

                #region SQL Command

                SqlCommand objCommReceiveHeader = new SqlCommand();
                objCommReceiveHeader.Connection = currConn;

                objCommReceiveHeader.CommandText = sqlText;
                objCommReceiveHeader.CommandType = CommandType.Text;

                #endregion

                #region Parameter
                if (!objCommReceiveHeader.Parameters.Contains("@Post"))
                { objCommReceiveHeader.Parameters.AddWithValueAndNullHandle("@Post", Post); }
                else { objCommReceiveHeader.Parameters["@Post"].Value = Post; }
                if (!objCommReceiveHeader.Parameters.Contains("@ReceiveNo"))
                { objCommReceiveHeader.Parameters.AddWithValueAndNullHandle("@ReceiveNo", ReceiveNo); }
                else { objCommReceiveHeader.Parameters["@ReceiveNo"].Value = ReceiveNo; }
                if (ReceiveDateFrom == "")
                {
                    if (!objCommReceiveHeader.Parameters.Contains("@ReceiveDateFrom"))
                    { objCommReceiveHeader.Parameters.AddWithValueAndNullHandle("@ReceiveDateFrom", System.DBNull.Value); }
                    else { objCommReceiveHeader.Parameters["@ReceiveDateFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommReceiveHeader.Parameters.Contains("@ReceiveDateFrom"))
                    { objCommReceiveHeader.Parameters.AddWithValueAndNullHandle("@ReceiveDateFrom", ReceiveDateFrom); }
                    else { objCommReceiveHeader.Parameters["@ReceiveDateFrom"].Value = ReceiveDateFrom; }
                }
                if (ReceiveDateTo == "")
                {
                    if (!objCommReceiveHeader.Parameters.Contains("@ReceiveDateTo"))
                    { objCommReceiveHeader.Parameters.AddWithValueAndNullHandle("@ReceiveDateTo", System.DBNull.Value); }
                    else { objCommReceiveHeader.Parameters["@ReceiveDateTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommReceiveHeader.Parameters.Contains("@ReceiveDateTo"))
                    { objCommReceiveHeader.Parameters.AddWithValueAndNullHandle("@ReceiveDateTo", ReceiveDateTo); }
                    else { objCommReceiveHeader.Parameters["@ReceiveDateTo"].Value = ReceiveDateTo; }
                }

                if (!objCommReceiveHeader.Parameters.Contains("@SerialNo"))
                { objCommReceiveHeader.Parameters.AddWithValueAndNullHandle("@SerialNo", SerialNo); }
                else { objCommReceiveHeader.Parameters["@SerialNo"].Value = SerialNo; }
                if (!objCommReceiveHeader.Parameters.Contains("@transactionType"))
                { objCommReceiveHeader.Parameters.AddWithValueAndNullHandle("@transactionType", transactionType); }
                else { objCommReceiveHeader.Parameters["@transactionType"].Value = transactionType; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommReceiveHeader);
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
        public DataTable SearchReceiveDetailNew(string ReceiveNo, string databaseName)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("SearchReceiveDetail");

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
                            ReceiveDetails.ReceiveNo,
                            ReceiveDetails.ReceiveLineNo,
                            ReceiveDetails.ItemNo,
                            isnull(ReceiveDetails.Quantity,0)Quantity,
                            isnull(ReceiveDetails.CostPrice,0)CostPrice ,
                            isnull(ReceiveDetails.NBRPrice,0)NBRPrice,
                            isnull(ReceiveDetails.UOM,'N/A')UOM ,
                            isnull(ReceiveDetails.VATRate,0)VATRate ,
                            isnull(ReceiveDetails.VATAmount,0)VATAmount,
                            isnull(ReceiveDetails.SubTotal,0)SubTotal ,
                            isnull(ReceiveDetails.Comments,'N/A')Comments ,
                            isnull(Products.ProductName,'N/A')ProductName,
                            isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
                            isnull(ReceiveDetails.SD,0)SD ,
                            isnull(nullif(ReceiveDetails.BOMId,''),0)BOMId ,
isnull(nullif(ReceiveDetails.VATName,''),'NA')VATName ,
                            isnull(ReceiveDetails.SDAmount,0)SDAmount ,
                            isnull(Products.ProductCode,'N/A')ProductCode,
							isnull(ReceiveDetails.UOMQty,isnull(ReceiveDetails.Quantity,0))	UOMQty,
							isnull(ReceiveDetails.UOMPrice,isnull(ReceiveDetails.CostPrice,0))	UOMPrice,
							isnull(ReceiveDetails.UOMc,1)		UOMc,
							isnull(ReceiveDetails.UOMn,isnull(ReceiveDetails.UOM,'N/A'))		UOMn,
ReturnTransactionType,TransactionType
                            FROM         ReceiveDetails LEFT OUTER JOIn
                            Products ON ReceiveDetails.ItemNo = Products.ItemNo             
                            WHERE 
                                (ReceiveDetails.ReceiveNo = @ReceiveNo )
                            ";

                #endregion

                #region SQL Command

                SqlCommand objCommReceiveDetail = new SqlCommand();
                objCommReceiveDetail.Connection = currConn;

                objCommReceiveDetail.CommandText = sqlText;
                objCommReceiveDetail.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommReceiveDetail.Parameters.Contains("@ReceiveNo"))
                { objCommReceiveDetail.Parameters.AddWithValueAndNullHandle("@ReceiveNo", ReceiveNo); }
                else { objCommReceiveDetail.Parameters["@ReceiveNo"].Value = ReceiveNo; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommReceiveDetail);
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

        public string[] ImportData(DataTable dtReceiveM, DataTable dtReceiveD)
        {
            #region variable

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            ReceiveMasterVM receiveMasterVM;
            List<ReceiveDetailVM> receiveDetailVMs = new List<ReceiveDetailVM>();

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

                #region Settings Value
                CommonDAL commondal = new CommonDAL();
                string vPriceDeclaration = commondal.settings("Receive", "PriceDeclarationForImport");
                bool isPriceDeclaration = vPriceDeclaration == "Y" ? true : false;
                #endregion
                #region RowCount

                int MRowCount = 0;
                int MRow = dtReceiveM.Rows.Count;



                for (int i = 0; i < dtReceiveM.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtReceiveM.Rows[i]["ID"].ToString()))
                    {
                        MRowCount++;
                    }

                }
                if (MRow != MRowCount)
                {
                    throw new ArgumentNullException("you have select " + MRow.ToString() +
                                                    " data for import, but you have " + MRowCount + " id.");
                }

                #endregion RowCount

                #region ID in Master or Detail table

                for (int i = 0; i < MRowCount; i++)
                {
                    string importID = dtReceiveM.Rows[i]["ID"].ToString();

                    if (!string.IsNullOrEmpty(importID))
                    {
                        DataRow[] DetailRaws = dtReceiveD.Select("ID='" + importID + "'");
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
                    string id = dtReceiveM.Rows[i]["ID"].ToString();
                    DataRow[] tt = dtReceiveM.Select("ID='" + id + "'");
                    if (tt.Length > 1)
                    {
                        throw new ArgumentNullException("you have duplicate master id " + id + " in import file.");
                    }

                }

                #endregion Double ID in Master

                #region Find Column in details
                /// CP has no NBR_Price
                bool isColNBRPrice = false;
                for (int i = 0; i < dtReceiveD.Columns.Count; i++)
                {
                    if (dtReceiveD.Columns[i].ColumnName.ToString() == "NBR_Price")
                    {
                        isColNBRPrice = true;
                    }

                }
                #endregion

                #region Process model

                CommonImport cImport = new CommonImport();

                #region checking from database is exist the information(NULL Check)

                #region Master

                for (int j = 0; j < MRowCount; j++)
                {
                    #region Checking Date is null or different formate

                    bool IsReceiveDate;
                    IsReceiveDate = cImport.CheckDate(dtReceiveM.Rows[j]["Receive_DateTime"].ToString().Trim());
                    if (IsReceiveDate != true)
                    {
                        throw new ArgumentNullException(
                            "Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Receive_Date field.");
                    }

                    #endregion Checking Date is null or different formate

                    #region Checking Y/N value

                    bool post;
                    post = cImport.CheckYN(dtReceiveM.Rows[j]["Post"].ToString().Trim());
                    if (post != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Post field.");
                    }

                    #endregion Checking Y/N value

                    #region Check Return receive id

                    string ReturnId = string.Empty;
                    ReturnId = cImport.CheckReceiveReturnID(dtReceiveM.Rows[j]["Return_Id"].ToString().Trim(), currConn, transaction);


                    #endregion Check Return receive id
                }

                #endregion Master

                #region Details

                #region Row count for details table

                int DRowCount = 0;
                for (int i = 0; i < dtReceiveD.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtReceiveD.Rows[i]["ID"].ToString()))
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
                    if (string.IsNullOrEmpty(dtReceiveD.Rows[i]["Item_Code"].ToString().Trim()))
                    {
                        throw new ArgumentNullException("Please insert value in 'Item_Code' field.");
                    }
                    ItemNo = cImport.FindItemId(dtReceiveD.Rows[i]["Item_Name"].ToString().Trim()
                                                , dtReceiveD.Rows[i]["Item_Code"].ToString().Trim(), currConn, transaction);

                    #endregion FindItemId

                    #region FindUOMn

                    UOMn = cImport.FindUOMn(ItemNo, currConn, transaction);

                    #endregion FindUOMn

                    #region FindUOMn
                    if (DatabaseInfoVM.DatabaseName.ToString() != "Sanofi_DB")
                    {
                        cImport.FindUOMc(UOMn, dtReceiveD.Rows[i]["UOM"].ToString().Trim(), currConn, transaction);
                    }
                    #endregion FindUOMn

                    #region VATName

                    cImport.FindVatName(dtReceiveD.Rows[i]["VAT_Name"].ToString().Trim());

                    #endregion VATName

                    #region FindLastNBRPrice

                    DataRow[] vmaster; //= new DataRow[];//
                    string nbrPrice = string.Empty;

                    var transactionDate = "";

                    vmaster = dtReceiveM.Select("ID='" + dtReceiveD.Rows[i]["ID"].ToString().Trim() + "'");

                    foreach (DataRow row in vmaster)
                    {
                        var tt =
                            Convert.ToDateTime(row["Receive_DateTime"].ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss");
                        transactionDate = tt;

                    }
                    if (isPriceDeclaration == true)
                    {
                        nbrPrice = cImport.FindLastNBRPrice(ItemNo, dtReceiveD.Rows[i]["VAT_Name"].ToString().Trim(),
                                        transactionDate, currConn, transaction);
                    }


                    #endregion FindLastNBRPrice

                    #region Numeric value check

                    bool IsQuantity = cImport.CheckNumericBool(dtReceiveD.Rows[i]["Quantity"].ToString().Trim());
                    if (IsQuantity != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Quantity field.");
                    }

                    if (isColNBRPrice == true)
                    {


                        bool IsNbrPrice = false;
                        if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                        {
                            IsNbrPrice = cImport.CheckNumericBool(dtReceiveD.Rows[i]["Total_Price"].ToString().Trim());
                        }
                        else
                        {
                            IsNbrPrice = cImport.CheckNumericBool(dtReceiveD.Rows[i]["NBR_Price"].ToString().Trim());
                        }
                        if (IsNbrPrice != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in NBR_Price/Total_Price field.");
                        }
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

                for (int i = 0; i < MRowCount; i++)
                {
                    #region Master Receive

                    string importID = dtReceiveM.Rows[i]["ID"].ToString().Trim();
                    string receiveDateTime = dtReceiveM.Rows[i]["Receive_DateTime"].ToString().Trim();

                    #region CheckNull

                    string serialNo = cImport.ChecKNullValue(dtReceiveM.Rows[i]["Reference_No"].ToString().Trim());
                    string comments = cImport.ChecKNullValue(dtReceiveM.Rows[i]["Comments"].ToString().Trim());

                    #endregion CheckNull

                    string post = dtReceiveM.Rows[i]["Post"].ToString().Trim();

                    #region Check Return receive id

                    string ReturnId = cImport.CheckReceiveReturnID(dtReceiveM.Rows[i]["Return_Id"].ToString().Trim(),
                                                                   currConn, transaction);

                    #endregion Check Return receive id
                    string createdBy = dtReceiveM.Rows[i]["Created_By"].ToString().Trim();
                    string lastModifiedBy = dtReceiveM.Rows[i]["LastModified_By"].ToString().Trim();
                    string transactionType = dtReceiveM.Rows[i]["Transection_Type"].ToString().Trim();
                    string fromBOM = dtReceiveM.Rows[i]["From_BOM"].ToString().Trim();
                    string totalVATAmount = dtReceiveM.Rows[i]["Total_VAT_Amount"].ToString().Trim();
                    string totalAmount = dtReceiveM.Rows[i]["Total_Amount"].ToString().Trim();

                    receiveMasterVM = new ReceiveMasterVM();
                    receiveMasterVM.ReceiveDateTime = Convert.ToDateTime(receiveDateTime).ToString("yyyy-MM-dd") +
                                                      DateTime.Now.ToString(" HH:mm:ss");
                    //receiveMasterVM.VatName = "NA";
                    //receiveMasterVM.VatName = vatName;
                    receiveMasterVM.Post = post;
                    receiveMasterVM.ReturnId = ReturnId;
                    receiveMasterVM.SerialNo = serialNo.Replace(" ", "");
                    receiveMasterVM.Comments = comments;
                    receiveMasterVM.CreatedBy = createdBy;
                    receiveMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    receiveMasterVM.LastModifiedBy = lastModifiedBy;
                    receiveMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    receiveMasterVM.transactionType = transactionType;
                    receiveMasterVM.FromBOM = fromBOM;
                    receiveMasterVM.TotalVATAmount = Convert.ToDecimal(totalVATAmount);
                    receiveMasterVM.TotalAmount = Convert.ToDecimal(totalAmount);
                    receiveMasterVM.ImportId = importID;


                    DataRow[] RDRaws; //= new DataRow[];//

                    #region fitemno

                    if (!string.IsNullOrEmpty(importID))
                    {
                        RDRaws =
                            dtReceiveD.Select("ID='" + importID + "'");
                    }
                    else
                    {
                        RDRaws = null;
                    }

                    #endregion fitemno

                    #endregion Master Receive

                    #region Details Receive

                    int counter = 1;
                    receiveDetailVMs = new List<ReceiveDetailVM>();


                    DataTable dtDistinctItem = RDRaws.CopyToDataTable().DefaultView.ToTable(true, "Item_Code");

                    DataTable dtReceiveDetail = RDRaws.CopyToDataTable();
                    foreach (DataRow item in dtDistinctItem.Rows)
                    {
                        string itemCode = item["Item_Code"].ToString().Trim();
                        string itemName = "";
                        string itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);
                        //string quantity = item["Quantity"].ToString().Trim();
                        decimal quantity = 0;
                        string uOM = "";
                        string uOMn = "";
                        string uOMc = "";
                        string vATName = "";
                        decimal nbrPrice = 0;
                        decimal LastNBRPrice = 0;
                        decimal totalPrice = 0;
                        DataTable dtRepeatedItems = dtReceiveDetail.Select("item_No =" + itemNo).CopyToDataTable();
                        //DataTable dtRepeatedItems = dtReceiveDetail.Select("[Item_Code] ='" + itemCode + "'").CopyToDataTable();
                        foreach (DataRow row in dtRepeatedItems.Rows)
                        {
                            //itemName = row["Item_Name"].ToString().Trim();
                            vATName = row["VAT_Name"].ToString().Trim();
                            quantity = quantity + Convert.ToDecimal(row["Quantity"].ToString().Trim());

                            if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                            {
                                uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                                uOM = uOMn;
                                uOMc = "1";


                                totalPrice = totalPrice + Convert.ToDecimal(row["Total_Price"].ToString().Trim());
                                nbrPrice = totalPrice / quantity;
                                LastNBRPrice = nbrPrice;
                                receiveMasterVM.FromBOM = "N";

                            }
                            else
                            {
                                uOM = row["UOM"].ToString().Trim();
                                uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                                uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction);

                                string lastPrice = cImport.FindLastNBRPrice(itemNo, vATName, Convert.ToDateTime(receiveDateTime).ToString("yyyy-MM-dd"), currConn, transaction);
                                LastNBRPrice = Convert.ToDecimal(lastPrice);

                                if (isColNBRPrice == true)
                                {
                                    nbrPrice = nbrPrice + Convert.ToDecimal(row["NBR_Price"].ToString().Trim());

                                }
                                if (isPriceDeclaration == false)
                                {
                                    LastNBRPrice = nbrPrice;
                                    receiveMasterVM.FromBOM = "N";
                                }
                            }
                        }

                        ReceiveDetailVM receiveDetailVM = new ReceiveDetailVM();
                        receiveDetailVM.ReceiveLineNo = counter.ToString();
                        receiveDetailVM.ItemNo = itemNo.ToString();
                        receiveDetailVM.Quantity = Convert.ToDecimal(quantity);
                        receiveDetailVM.UOM = uOM;
                        receiveDetailVM.UOMn = uOMn.ToString();
                        receiveDetailVM.UOMc = Convert.ToDecimal(uOMc);
                        receiveDetailVM.VATRate = Convert.ToDecimal(0);
                        receiveDetailVM.VATAmount = Convert.ToDecimal(0);
                        receiveDetailVM.SD = Convert.ToDecimal(0);
                        receiveDetailVM.SDAmount = Convert.ToDecimal(0);
                        receiveDetailVM.BOMId = "0";
                        receiveDetailVM.CommentsD = "NA";
                        receiveDetailVM.VatName = vATName;
                        receiveDetailVM.CostPrice = Convert.ToDecimal(LastNBRPrice) * Convert.ToDecimal(uOMc);
                        receiveDetailVM.NBRPrice = Convert.ToDecimal(LastNBRPrice) * Convert.ToDecimal(uOMc);
                        receiveDetailVM.SubTotal = Convert.ToDecimal(LastNBRPrice) * Convert.ToDecimal(uOMc) *
                                                   Convert.ToDecimal(quantity);
                        receiveDetailVM.UOMPrice = Convert.ToDecimal(LastNBRPrice);
                        receiveDetailVM.UOMQty = Convert.ToDecimal(uOMc) *
                                                 Convert.ToDecimal(quantity);
                        receiveDetailVMs.Add(receiveDetailVM);

                        counter++;

                    }
                    #endregion Details Receive

                    #region OLD
                    //foreach (DataRow row in RDRaws)
                    //{
                    //    string itemCode = row["Item_Code"].ToString().Trim();
                    //    string itemName = row["Item_Name"].ToString().Trim();
                    //    string itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);
                    //    string quantity = row["Quantity"].ToString().Trim();
                    //    string uOM = "";
                    //    string uOMn = "";
                    //    string uOMc = "";
                    //    string vATName = row["VAT_Name"].ToString().Trim();
                    //    string nbrPrice = "0";
                    //    string LastNBRPrice = "0";
                    //    if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                    //    {
                    //        uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                    //        uOM = uOMn;
                    //        uOMc = "1";


                    //        string totalPrice = row["Total_Price"].ToString().Trim();
                    //        nbrPrice = (Convert.ToDecimal(totalPrice) / Convert.ToDecimal(quantity)).ToString();
                    //        LastNBRPrice = nbrPrice;
                    //        receiveMasterVM.FromBOM = "N";
                    //    }
                    //    else
                    //    {
                    //        uOM = row["UOM"].ToString().Trim();
                    //        uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                    //        uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction);

                    //        nbrPrice = row["NBR_Price"].ToString().Trim();
                    //        LastNBRPrice = cImport.FindLastNBRPrice(itemNo, vATName, Convert.ToDateTime(receiveDateTime).ToString("yyyy-MM-dd"), currConn, transaction);

                    //        if (isPriceDeclaration == false && nbrPrice != "0")
                    //        {
                    //            LastNBRPrice = nbrPrice;
                    //            receiveMasterVM.FromBOM = "N";
                    //        }
                    //    }


                    //    ReceiveDetailVM receiveDetailVM = new ReceiveDetailVM();
                    //    receiveDetailVM.ReceiveLineNo = counter.ToString();
                    //    receiveDetailVM.ItemNo = itemNo.ToString();
                    //    receiveDetailVM.Quantity = Convert.ToDecimal(quantity);
                    //    receiveDetailVM.UOM = uOM;
                    //    receiveDetailVM.UOMn = uOMn.ToString();
                    //    receiveDetailVM.UOMc = Convert.ToDecimal(uOMc);
                    //    receiveDetailVM.VATRate = Convert.ToDecimal(0);
                    //    receiveDetailVM.VATAmount = Convert.ToDecimal(0);
                    //    receiveDetailVM.SD = Convert.ToDecimal(0);
                    //    receiveDetailVM.SDAmount = Convert.ToDecimal(0);
                    //    receiveDetailVM.BOMId = "0";
                    //    receiveDetailVM.CommentsD = "NA";
                    //    receiveDetailVM.VatName = vATName;
                    //    receiveDetailVM.CostPrice = Convert.ToDecimal(LastNBRPrice) * Convert.ToDecimal(uOMc);
                    //    receiveDetailVM.NBRPrice = Convert.ToDecimal(LastNBRPrice) * Convert.ToDecimal(uOMc);
                    //    receiveDetailVM.SubTotal = Convert.ToDecimal(LastNBRPrice) * Convert.ToDecimal(uOMc) *
                    //                               Convert.ToDecimal(quantity);
                    //    receiveDetailVM.UOMPrice = Convert.ToDecimal(LastNBRPrice);
                    //    receiveDetailVM.UOMQty = Convert.ToDecimal(uOMc) *
                    //                             Convert.ToDecimal(quantity);
                    //    receiveDetailVMs.Add(receiveDetailVM);

                    //    counter++;

                    //} // detail
                    #endregion OLD

                    string[] sqlResults = ReceiveInsert(receiveMasterVM, receiveDetailVMs, null, transaction, currConn);
                    retResults[0] = sqlResults[0];
                }

                if (retResults[0] == "Success")
                {
                    transaction.Commit();
                    #region SuccessResult

                    retResults[0] = "Success";
                    retResults[1] = MessageVM.PurchasemsgSaveSuccessfully;
                    retResults[2] = "" + "1";
                    retResults[3] = "" + "N";
                    #endregion SuccessResult
                }



                #endregion Process model
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
        public string[] GetUSDCurrency(decimal subTotal)
        {
            string[] retResults = new string[2];
            retResults[0] = "0";
            retResults[1] = "0";
            try
            {
                CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();
                DataTable CurrencyConversionResult = currencyConversionDal.SearchCurrencyConversionNew("USD", string.Empty, "BDT", string.Empty,
                      "Y", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                if (CurrencyConversionResult != null)
                {

                    decimal currencyRate = Convert.ToDecimal(CurrencyConversionResult.Rows[0]["CurrencyRate"].ToString());

                    decimal bDTValue = subTotal;
                    decimal dollerValue = subTotal / currencyRate;
                    retResults[0] = bDTValue.ToString();
                    retResults[1] = dollerValue.ToString();
                }
            }
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



            return retResults;
        }

        public decimal ReturnReceiveQty(string receiveReturnId, string itemNo)
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

                sqlText = "select Sum(isnull(ReceiveDetails.Quantity,0)) from ReceiveDetails ";
                sqlText += "where ItemNo = '" + itemNo + "' and ReceiveReturnId = '" + receiveReturnId + "'";
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

        public string[] ImportData_Sanofi(DataTable dtReceiveM, DataTable dtReceiveD)
        {
            #region variable

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            ReceiveMasterVM receiveMasterVM;
            List<ReceiveDetailVM> receiveDetailVMs = new List<ReceiveDetailVM>();

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

                #region Settings Value
                CommonDAL commondal = new CommonDAL();
                string vPriceDeclaration = commondal.settings("Receive", "PriceDeclarationForImport");
                bool isPriceDeclaration = vPriceDeclaration == "Y" ? true : false;
                #endregion
                #region RowCount

                int MRowCount = 0;
                int MRow = dtReceiveM.Rows.Count;



                for (int i = 0; i < dtReceiveM.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtReceiveM.Rows[i]["ID"].ToString()))
                    {
                        MRowCount++;
                    }

                }
                if (MRow != MRowCount)
                {
                    throw new ArgumentNullException("you have select " + MRow.ToString() +
                                                    " data for import, but you have " + MRowCount + " id.");
                }

                #endregion RowCount

                #region ID in Master or Detail table

                for (int i = 0; i < MRowCount; i++)
                {
                    string importID = dtReceiveM.Rows[i]["ID"].ToString();

                    if (!string.IsNullOrEmpty(importID))
                    {
                        DataRow[] DetailRaws = dtReceiveD.Select("ID='" + importID + "'");
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
                    string id = dtReceiveM.Rows[i]["ID"].ToString();
                    DataRow[] tt = dtReceiveM.Select("ID='" + id + "'");
                    if (tt.Length > 1)
                    {
                        throw new ArgumentNullException("you have duplicate master id " + id + " in import file.");
                    }

                }

                #endregion Double ID in Master


                #region Process model

                CommonImport cImport = new CommonImport();

                #region checking from database is exist the information(NULL Check)

                #region Master

                for (int j = 0; j < MRowCount; j++)
                {
                    #region Checking Date is null or different formate

                    bool IsReceiveDate;
                    IsReceiveDate = cImport.CheckDate(dtReceiveM.Rows[j]["Receive_DateTime"].ToString().Trim());
                    if (IsReceiveDate != true)
                    {
                        throw new ArgumentNullException(
                            "Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Receive_Date field.");
                    }

                    #endregion Checking Date is null or different formate

                    #region Checking Y/N value

                    bool post;
                    post = cImport.CheckYN(dtReceiveM.Rows[j]["Post"].ToString().Trim());
                    if (post != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Post field.");
                    }

                    #endregion Checking Y/N value

                    #region Check Return receive id

                    string ReturnId = string.Empty;
                    ReturnId = cImport.CheckReceiveReturnID(dtReceiveM.Rows[j]["Return_Id"].ToString().Trim(), currConn, transaction);


                    #endregion Check Return receive id
                }

                #endregion Master

                #region Details

                #region Row count for details table

                int DRowCount = 0;
                for (int i = 0; i < dtReceiveD.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtReceiveD.Rows[i]["ID"].ToString()))
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

                    ItemNo = cImport.FindItemId(dtReceiveD.Rows[i]["Item_Name"].ToString().Trim()
                                                , dtReceiveD.Rows[i]["Item_Code"].ToString().Trim(), currConn, transaction);

                    #endregion FindItemId

                    #region FindUOMn
                    UOMn = cImport.FindUOMn(ItemNo, currConn, transaction);
                    #endregion FindUOMn

                    #region VATName

                    cImport.FindVatName(dtReceiveD.Rows[i]["VAT_Name"].ToString().Trim());

                    #endregion VATName

                    #region FindLastNBRPrice
                    //DataRow[] vmaster; //= new DataRow[];//
                    //string nbrPrice = string.Empty;

                    //var transactionDate = "";

                    //vmaster = dtReceiveM.Select("ID='" + dtReceiveD.Rows[i]["ID"].ToString().Trim() + "'");

                    //foreach (DataRow row in vmaster)
                    //{
                    //    var tt =
                    //        Convert.ToDateTime(row["Receive_DateTime"].ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss");
                    //    transactionDate = tt;

                    //}
                    //if (isPriceDeclaration == true)
                    //{
                    //    nbrPrice = cImport.FindLastNBRPrice(ItemNo, dtReceiveD.Rows[i]["VAT_Name"].ToString().Trim(),
                    //                    transactionDate, currConn, transaction);
                    //}


                    #endregion FindLastNBRPrice

                    #region Numeric value check

                    bool IsQuantity = cImport.CheckNumericBool(dtReceiveD.Rows[i]["Quantity"].ToString().Trim());
                    if (IsQuantity != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Quantity field.");
                    }

                    bool IsNbrPrice = false;
                    IsNbrPrice = cImport.CheckNumericBool(dtReceiveD.Rows[i]["Total_Price"].ToString().Trim());
                    if (IsNbrPrice != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in NBR_Price/Total_Price field.");
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

                for (int i = 0; i < MRowCount; i++)
                {
                    #region Master Receive

                    string importID = dtReceiveM.Rows[i]["ID"].ToString().Trim();
                    string receiveDateTime = dtReceiveM.Rows[i]["Receive_DateTime"].ToString().Trim();

                    #region CheckNull

                    string serialNo = cImport.ChecKNullValue(dtReceiveM.Rows[i]["Reference_No"].ToString().Trim());
                    string comments = cImport.ChecKNullValue(dtReceiveM.Rows[i]["Comments"].ToString().Trim());

                    #endregion CheckNull

                    string post = dtReceiveM.Rows[i]["Post"].ToString().Trim();

                    #region Check Return receive id

                    string ReturnId = cImport.CheckReceiveReturnID(dtReceiveM.Rows[i]["Return_Id"].ToString().Trim(),
                                                                   currConn, transaction);

                    #endregion Check Return receive id
                    string createdBy = dtReceiveM.Rows[i]["Created_By"].ToString().Trim();
                    string lastModifiedBy = dtReceiveM.Rows[i]["LastModified_By"].ToString().Trim();
                    string transactionType = dtReceiveM.Rows[i]["Transection_Type"].ToString().Trim();
                    string fromBOM = dtReceiveM.Rows[i]["From_BOM"].ToString().Trim();
                    string totalVATAmount = dtReceiveM.Rows[i]["Total_VAT_Amount"].ToString().Trim();
                    string totalAmount = dtReceiveM.Rows[i]["Total_Amount"].ToString().Trim();

                    receiveMasterVM = new ReceiveMasterVM();
                    receiveMasterVM.ReceiveDateTime = Convert.ToDateTime(receiveDateTime).ToString("yyyy-MM-dd") +
                                                      DateTime.Now.ToString(" HH:mm:ss");
                    receiveMasterVM.Post = post;
                    receiveMasterVM.ReturnId = ReturnId;
                    receiveMasterVM.SerialNo = serialNo.Replace(" ", "");
                    receiveMasterVM.Comments = comments;
                    receiveMasterVM.CreatedBy = createdBy;
                    receiveMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    receiveMasterVM.LastModifiedBy = lastModifiedBy;
                    receiveMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    receiveMasterVM.transactionType = transactionType;
                    receiveMasterVM.FromBOM = fromBOM;
                    receiveMasterVM.TotalVATAmount = Convert.ToDecimal(totalVATAmount);
                    receiveMasterVM.TotalAmount = Convert.ToDecimal(totalAmount);
                    receiveMasterVM.ImportId = importID;
                    receiveMasterVM.FromBOM = "N";

                    DataRow[] RDRaws; //= new DataRow[];//

                    #region fitemno

                    if (!string.IsNullOrEmpty(importID))
                    {
                        RDRaws =
                            dtReceiveD.Select("ID='" + importID + "'");
                    }
                    else
                    {
                        RDRaws = null;
                    }

                    #endregion fitemno

                    #endregion Master Receive

                    #region Details Receive

                    int counter = 1;
                    receiveDetailVMs = new List<ReceiveDetailVM>();
                    DataTable dtDistinctItem = RDRaws.CopyToDataTable().DefaultView.ToTable(true, "Item_Code");

                    DataTable dtReceiveDetail = RDRaws.CopyToDataTable();
                    foreach (DataRow item in dtDistinctItem.Rows)
                    {
                        string itemCode = item["Item_Code"].ToString().Trim();

                        DataTable dtRepeatedItems = dtReceiveDetail.Select("[Item_Code] ='" + item["Item_Code"].ToString() + "'").CopyToDataTable();

                        decimal nbrPrice = 0;
                        decimal LastNBRPrice = 0;
                        decimal quantity = 0;
                        decimal totalPrice = 0;

                        string vATName = "";
                        string itemNo = "";
                        foreach (DataRow row in dtRepeatedItems.Rows)
                        {
                            string itemName = row["Item_Name"].ToString().Trim();
                            itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);

                            quantity = quantity + Convert.ToDecimal(row["Quantity"].ToString().Trim());
                            totalPrice = totalPrice + Convert.ToDecimal(row["Total_Price"].ToString().Trim());
                            nbrPrice = (totalPrice) / (quantity);
                            LastNBRPrice = nbrPrice;

                            vATName = row["VAT_Name"].ToString().Trim();
                        }
                        string uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                        string uOM = uOMn;
                        string uOMc = "1";


                        ReceiveDetailVM receiveDetailVM = new ReceiveDetailVM();
                        receiveDetailVM.ReceiveLineNo = counter.ToString();
                        receiveDetailVM.ItemNo = itemNo.ToString();
                        receiveDetailVM.Quantity = Convert.ToDecimal(quantity);
                        receiveDetailVM.UOM = uOM;
                        receiveDetailVM.UOMn = uOMn.ToString();
                        receiveDetailVM.UOMc = Convert.ToDecimal(uOMc);
                        receiveDetailVM.VATRate = Convert.ToDecimal(0);
                        receiveDetailVM.VATAmount = Convert.ToDecimal(0);
                        receiveDetailVM.SD = Convert.ToDecimal(0);
                        receiveDetailVM.SDAmount = Convert.ToDecimal(0);
                        receiveDetailVM.BOMId = "0";
                        receiveDetailVM.CommentsD = "NA";
                        receiveDetailVM.VatName = vATName;
                        receiveDetailVM.CostPrice = Convert.ToDecimal(LastNBRPrice) * Convert.ToDecimal(uOMc);
                        receiveDetailVM.NBRPrice = Convert.ToDecimal(LastNBRPrice) * Convert.ToDecimal(uOMc);
                        receiveDetailVM.SubTotal = Convert.ToDecimal(LastNBRPrice) * Convert.ToDecimal(uOMc) *
                                                   Convert.ToDecimal(quantity);
                        receiveDetailVM.UOMPrice = Convert.ToDecimal(LastNBRPrice);
                        receiveDetailVM.UOMQty = Convert.ToDecimal(uOMc) *
                                                 Convert.ToDecimal(quantity);
                        receiveDetailVMs.Add(receiveDetailVM);

                        counter++;

                    }
                    #endregion Details Receive

                    string[] sqlResults = ReceiveInsert(receiveMasterVM, receiveDetailVMs, null, transaction, currConn);
                    retResults[0] = sqlResults[0];
                }

                if (retResults[0] == "Success")
                {
                    transaction.Commit();
                    #region SuccessResult

                    retResults[0] = "Success";
                    retResults[1] = MessageVM.PurchasemsgSaveSuccessfully;
                    retResults[2] = "" + "1";
                    retResults[3] = "" + "N";
                    #endregion SuccessResult
                }



                #endregion Process model
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

        //==================SelectAll=================
        public List<ReceiveMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ReceiveMasterVM likeVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ReceiveMasterVM> VMs = new List<ReceiveMasterVM>();
            ReceiveMasterVM vm;
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
 rh.Id
,rh.ReceiveNo
,rh.ReceiveDateTime
,isnull(rh.TotalAmount,0) TotalAmount
,isnull(rh.TotalVATAmount,0) TotalVATAmount
,rh.SerialNo
,rh.Comments
,rh.CreatedBy
,rh.CreatedOn
,rh.LastModifiedBy
,rh.LastModifiedOn
,rh.TransactionType
,rh.Post
,rh.ReceiveReturnId
,rh.ImportIDExcel
,rh.ReferenceNo
,rh.CustomerID
,rh.WithToll
,c.CustomerName
,ISNULL(rh.ShiftId,0) ShiftId


FROM ReceiveHeaders rh left outer join Customers c
on rh.CustomerID=c.CustomerID
WHERE  1=1
";


                if (Id > 0)
                {
                    sqlText += @" and rh.Id=@Id";
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
                    if (!string.IsNullOrWhiteSpace(likeVM.ReceiveNo))
                    {
                        sqlText += " AND rh.ReceiveNo like @ReceiveNo";
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
                    if (!string.IsNullOrWhiteSpace(likeVM.ReceiveNo))
                    {
                        objComm.Parameters.AddWithValue("@ReceiveNo", "%" + likeVM.ReceiveNo + "%");
                    }
                }
                if (Id > 0)
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ReceiveMasterVM();
                    vm.Id = dr["Id"].ToString();
                    vm.ReceiveNo = dr["ReceiveNo"].ToString();
                    vm.ReceiveDateTime = Ordinary.DateTimeToDate(dr["ReceiveDateTime"].ToString());
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());
                    vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"].ToString());
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.transactionType = dr["TransactionType"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.ReferenceNo = dr["ReferenceNo"].ToString();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();
                    vm.WithToll = dr["WithToll"].ToString();
                    vm.ShiftId = dr["ShiftId"].ToString();
                    
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
        public List<ReceiveDetailVM> SelectReceiveDetail(string ReceiveNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ReceiveDetailVM> VMs = new List<ReceiveDetailVM>();
            ReceiveDetailVM vm;
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
 rd.ReceiveNo
,rd.ReceiveLineNo
,rd.ItemNo
,isnull(rd.Quantity ,0) Quantity
,isnull(rd.CostPrice ,0) CostPrice
,isnull(rd.NBRPrice ,0) NBRPrice
,rd.UOM
,isnull(rd.VATRate ,0) VATRate
,isnull(rd.VATAmount ,0) VATAmount
,isnull(rd.SubTotal ,0) SubTotal
,rd.Comments
,rd.CreatedBy
,rd.CreatedOn
,rd.LastModifiedBy
,rd.LastModifiedOn
,isnull(rd.SD ,0) SD
,isnull(rd.SDAmount ,0) SDAmount
,rd.TransactionType
,rd.ReceiveDateTime
,rd.Post
,rd.VATName
,rd.ReceiveReturnId
,isnull(rd.DiscountAmount ,0) DiscountAmount
,isnull(rd.DiscountedNBRPrice ,0) DiscountedNBRPrice
,rd.BOMId
,rd.BOMId1
,isnull(rd.UOMQty ,0) UOMQty
,isnull(rd.UOMPrice ,0) UOMPrice
,isnull(rd.UOMc ,0) UOMc
,rd.UOMn
,isnull(rd.CurrencyValue ,0) CurrencyValue
,isnull(rd.DollerValue ,0) DollerValue
,rd.ReturnTransactionType
,p.ProductCode
,p.ProductName

FROM ReceiveDetails  rd left outer join Products p on rd.ItemNo=p.ItemNo
WHERE  1=1

";

                if (ReceiveNo != null)
                {
                    sqlText += "AND rd.ReceiveNo=@ReceiveNo";
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

                if (ReceiveNo != null)
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@ReceiveNo", ReceiveNo);
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
                        objComm.Parameters.AddWithValueAndNullHandle("@" + cField, conditionValues[j]);
                    }
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                //ProductDAL ProductdDal;
                while (dr.Read())
                {
                    vm = new ReceiveDetailVM();
                    vm.ReceiveNo = dr["ReceiveNo"].ToString();
                    vm.ReceiveLineNo = dr["ReceiveLineNo"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                    vm.Post = dr["Post"].ToString() == "Y" ? true : false;
                    vm.VatName = dr["VATName"].ToString();
                    vm.BOMId = dr["BOMId"].ToString();
                    vm.UOMQty = Convert.ToDecimal(dr["UOMQty"].ToString());
                    vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.ReturnTransactionType = dr["ReturnTransactionType"].ToString();
                    vm.ItemName = dr["ProductName"].ToString();
                    vm.ItemCode = dr["ProductCode"].ToString();
                    

                    //var product = new ProductDAL().SelectAll(vm.ItemNo).FirstOrDefault();
                    //vm.ItemName = product.ProductName;
                    //vm.ItemCode = product.ProductCode;

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


        public string[] ImportExcelFile(ReceiveMasterVM paramVM)
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

                DataTable dtReceiveM = new DataTable();
                dtReceiveM = ds.Tables["ReceiveM"];

                DataTable dtReceiveD = new DataTable();
                dtReceiveD = ds.Tables["ReceiveD"];


                dtReceiveM.Columns.Add("Transection_Type");
                dtReceiveM.Columns.Add("Created_By");
                dtReceiveM.Columns.Add("LastModified_By");
                dtReceiveM.Columns.Add("From_BOM");
                dtReceiveM.Columns.Add("Total_VAT_Amount");
                dtReceiveM.Columns.Add("Total_Amount");

                CommonImport cImport = new CommonImport();
                foreach (DataRow row in dtReceiveM.Rows)
                {
                    row["Transection_Type"] = paramVM.transactionType;
                    row["Created_By"] = paramVM.CreatedBy;
                    row["LastModified_By"] = paramVM.LastModifiedBy;
                    row["From_BOM"] = "N";////hardcoding no here by robin
                    row["Total_VAT_Amount"] = "0";
                    row["Total_Amount"] = "0";////hardcoding no here by robin
                }

                dtReceiveD.Columns.Add("item_No");
                foreach (DataRow row in dtReceiveD.Rows)
                {
                    string itemNo = cImport.FindItemId("", row["Item_Code"].ToString().Trim(), null, null).ToString();
                    row["item_No"] = itemNo.Trim();


                }

                #region Data Insert
                retResults = ImportData(dtReceiveM, dtReceiveD);
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
