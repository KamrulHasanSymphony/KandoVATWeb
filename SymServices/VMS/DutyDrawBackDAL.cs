using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SymViewModel.VMS;
using System.Data.SqlClient;
using System.Data;

namespace SymServices.VMS
{
   public class DutyDrawBackDAL
    {
       private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
       private const string FieldDelimeter = DBConstant.FieldDelimeter;
      
       public string[] DutyDrawBacknsert(DDBHeaderVM Master, List<DDBDetailsVM> Details)
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
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
              }
              else if (Convert.ToDateTime(Master.DDBackDate) < DateTime.MinValue || Convert.ToDateTime(Master.DDBackDate) > DateTime.MaxValue)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "Please Check Issue Data and Time");

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

              string transactionDate = Master.DDBackDate;
              string transactionYearCheck = Convert.ToDateTime(Master.DDBackDate).ToString("yyyy-MM-dd");
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
              sqlText = sqlText + "select COUNT(DDBackNo) from DutyDrawBackHeader WHERE DDBackNo=@MasterDDBackNo";
              SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
              cmdExistTran.Transaction = transaction;
              cmdExistTran.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);

              IDExist = (int)cmdExistTran.ExecuteScalar();

              if (IDExist > 0)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
              }

              #endregion Find Transaction Exist


              #region Purchase ID Create For Other

              CommonDAL commonDal = new CommonDAL();

              newID = commonDal.TransactionCode("DDB", "DDB", "DutyDrawBackHeader", "DDBackNo",
                                           "DDBackDate", Master.DDBackDate, currConn, transaction);


              #endregion Purchase ID Create For Other



              #endregion Purchase ID Create Not Complete
              #region ID generated completed,Insert new Information in Header


              sqlText = "";
              sqlText += " insert into DutyDrawBackHeader(";
              sqlText += " DDBackNo,";
              sqlText += " DDBackDate,";
              sqlText += " SalesInvoiceNo,";
              sqlText += " SalesDate,";
              sqlText += " CustormerID,";
              sqlText += " CurrencyId,";
              sqlText += " ExpCurrency,";
              sqlText += " BDTCurrency,";
              sqlText += " FgItemNo,";
              sqlText += " TotalClaimCD,";
              sqlText += " TotalClaimRD,";
              sqlText += " TotalClaimSD,";
              sqlText += " TotalDDBack,";
              sqlText += " TotalClaimVAT,";
              sqlText += " TotalClaimCnFAmount,";
              sqlText += " TotalClaimInsuranceAmount,";
              sqlText += " TotalClaimTVBAmount,";
              sqlText += " TotalClaimTVAAmount,";
              sqlText += " TotalClaimATVAmount,";
              sqlText += " TotalClaimOthersAmount,";
              sqlText += " Comments,";
              sqlText += " CreatedBy,";
              sqlText += " CreatedOn,";
              sqlText += " LastModifiedBy,";
              sqlText += " LastModifiedOn,";
              sqlText += " Post";
              sqlText += " )";

              sqlText += " values";
              sqlText += " (";
              sqlText += "@newID,";
              sqlText += "@MasterDDBackDate,";
              sqlText += "@MasterSalesInvoiceNo,";
              sqlText += "@MasterSalesDate,";
              sqlText += "@MasterCustormerID,";
              sqlText += "@MasterCurrencyId,";
              sqlText += "@MasterExpCurrency,";
              sqlText += "@MasterBDTCurrency,";
              sqlText += "@MasterFgItemNo,";
              sqlText += "@MasterTotalClaimCD,";
              sqlText += "@MasterTotalClaimRD,";
              sqlText += "@MasterTotalClaimSD,";
              sqlText += "@MasterTotalDDBack,";
              sqlText += "@MasterTotalClaimSD,";
              sqlText += "@MasterTotalClaimVAT,";
              sqlText += "@MasterTotalClaimCnFAmount,";
              sqlText += "@MasterTotalClaimInsuranceAmount,";
              sqlText += "@MasterTotalClaimTVBAmount,";
              sqlText += "@MasterTotalClaimTVAAmount,";
              sqlText += "@MasterTotalClaimOthersAmount,";
              sqlText += "@MasterComments,";
              sqlText += "@MasterCreatedBy,";
              sqlText += "@MasterCreatedOn,";
              sqlText += "@MasterLastModifiedBy,";
              sqlText += "@MasterLastModifiedOn,";
              sqlText += "@MasterPost";
              sqlText += ")";


              SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
              cmdInsert.Transaction = transaction;
              cmdInsert.Parameters.AddWithValue("@newID", newID);
              cmdInsert.Parameters.AddWithValue("@MasterDDBackDate", Master.DDBackDate);
              cmdInsert.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
              cmdInsert.Parameters.AddWithValue("@MasterSalesDate", Master.SalesDate);
              cmdInsert.Parameters.AddWithValue("@MasterCustormerID", Master.CustormerID ?? Convert.DBNull);
              cmdInsert.Parameters.AddWithValue("@MasterCurrencyId", Master.CurrencyId);
              cmdInsert.Parameters.AddWithValue("@MasterExpCurrency", Master.ExpCurrency);
              cmdInsert.Parameters.AddWithValue("@MasterBDTCurrency", Master.BDTCurrency);
              cmdInsert.Parameters.AddWithValue("@MasterFgItemNo", Master.FgItemNo ?? Convert.DBNull);
              cmdInsert.Parameters.AddWithValue("@MasterTotalClaimCD", Master.TotalClaimCD);
              cmdInsert.Parameters.AddWithValue("@MasterTotalClaimRD", Master.TotalClaimRD);
              cmdInsert.Parameters.AddWithValue("@MasterTotalClaimSD", Master.TotalClaimSD);
              cmdInsert.Parameters.AddWithValue("@MasterTotalDDBack", Master.TotalDDBack);
              cmdInsert.Parameters.AddWithValue("@MasterTotalClaimSD", Master.TotalClaimSD);
              cmdInsert.Parameters.AddWithValue("@MasterTotalClaimVAT", Master.TotalClaimVAT);
              cmdInsert.Parameters.AddWithValue("@MasterTotalClaimCnFAmount", Master.TotalClaimCnFAmount);
              cmdInsert.Parameters.AddWithValue("@MasterTotalClaimInsuranceAmount", Master.TotalClaimInsuranceAmount);
              cmdInsert.Parameters.AddWithValue("@MasterTotalClaimTVBAmount", Master.TotalClaimTVBAmount);
              cmdInsert.Parameters.AddWithValue("@MasterTotalClaimTVAAmount", Master.TotalClaimTVAAmount);
              cmdInsert.Parameters.AddWithValue("@MasterTotalClaimOthersAmount", Master.TotalClaimOthersAmount);
              cmdInsert.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
              cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
              cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
              cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
              cmdInsert.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
              cmdInsert.Parameters.AddWithValue("@MasterPost", Master.Post);

              transResult = (int)cmdInsert.ExecuteNonQuery();
              if (transResult <= 0)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
              }


              #endregion ID generated completed,Insert new Information in Header
              #region if Transection not Other Insert Issue /Receive



              #endregion if Transection not Other Insert Issue /Receive

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
                  sqlText += "select COUNT(DDBackNo) from DutyDrawBackDetails WHERE DDBackNo=@newID ";
                  sqlText += " AND ItemNo=@ItemItemNo ";
                  sqlText += " AND FgItemNo=@ItemFgItemNo ";
                  SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                  cmdFindId.Transaction = transaction;
                  cmdFindId.Parameters.AddWithValue("@newID", newID);
                  cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                  cmdFindId.Parameters.AddWithValue("@ItemFgItemNo", Item.FgItemNo);

                  IDExist = (int)cmdFindId.ExecuteScalar();

                  if (IDExist > 0)
                  {
                      throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                  }

                  #endregion Find Transaction Exist

                  #region Insert only DetailTable

                  sqlText = "";
                  sqlText += " insert into DutyDrawBackDetails(";
                  sqlText += " DDBackNo,";
                  sqlText += " DDBackDate,";
                  sqlText += " DDLineNo,";
                  sqlText += " PurchaseInvoiceNo,";
                  sqlText += " PurchaseDate,";
                  sqlText += " FgItemNo,";
                  sqlText += " FGQty,";
                  sqlText += " ItemNo,";
                  sqlText += " BillOfEntry,";
                  sqlText += " PurchaseUom,";
                  sqlText += " PurchaseQuantity,";
                  sqlText += " UnitPrice,";
                  sqlText += " AV,";
                  sqlText += " CD,";
                  sqlText += " RD,";
                  sqlText += " SD,";
                  sqlText += " VAT,";
                  sqlText += " CnF,";
                  sqlText += " Insurance,";
                  sqlText += " TVB,";
                  sqlText += " TVA,";
                  sqlText += " ATV,";
                  sqlText += " Others,";
                  sqlText += " UseQuantity,";
                  sqlText += " ClaimCD,";
                  sqlText += " ClaimRD,";
                  sqlText += " ClaimSD,";
                  sqlText += " ClaimVAT,";
                  sqlText += " ClaimCnF,";
                  sqlText += " ClaimInsurance,";
                  sqlText += " ClaimTVB,";
                  sqlText += " ClaimTVA,";
                  sqlText += " ClaimATV,";
                  sqlText += " ClaimOthers,";
                  sqlText += " SubTotalDDB,";
                  sqlText += " UOMc,";
                  sqlText += " UOMn,";
                  sqlText += " UOMCD,";
                  sqlText += " UOMRD,";
                  sqlText += " UOMSD,";
                  sqlText += " UOMVAT,";
                  sqlText += " UOMCnF,";
                  sqlText += " UOMInsurance,";
                  sqlText += " UOMTVB,";
                  sqlText += " UOMTVA,";
                  sqlText += " UOMATV,";
                  sqlText += " UOMOthers,";
                  sqlText += " UOMSubTotalDDB,";
                  sqlText += " Post,";
                  sqlText += " CreatedBy,";
                  sqlText += " CreatedOn,";
                  sqlText += " LastModifiedBy,";
                  sqlText += " LastModifiedOn,";
                  sqlText += " PurchasetransactionType,";
                  sqlText += " SalesInvoiceNo";
                  sqlText += " )";

                  sqlText += " values(	";
                  //sqlText += "'" + Master.Id + "',";

                  sqlText += "@newID,";
                  sqlText += "@ItemDDBackDate,";
                  sqlText += "@ItemDDLineNo,";
                  sqlText += "@ItemPurchaseInvoiceNo,";
                  sqlText += "@ItemPurchaseDate,";
                  sqlText += "@ItemFgItemNo,";
                  sqlText += "@ItemFGQty,";
                  sqlText += "@ItemItemNo,";
                  sqlText += "@ItemBillOfEntry,";
                  sqlText += "@ItemPurchaseUom,";
                  sqlText += "@ItemPurchaseQuantity,";
                  sqlText += "@ItemUnitPrice,";
                  sqlText += "@ItemAV,";
                  sqlText += "@ItemCD,";
                  sqlText += "@ItemRD,";
                  sqlText += "@ItemSD,";
                  sqlText += "@ItemVAT,";
                  sqlText += "@ItemCnF,";
                  sqlText += "@ItemInsurance,";
                  sqlText += "@ItemTVB,";
                  sqlText += "@ItemTVA,";
                  sqlText += "@ItemATV,";
                  sqlText += "@ItemOthers,";
                  sqlText += "@ItemUseQuantity,";
                  sqlText += "@ItemClaimCD,";
                  sqlText += "@ItemClaimRD,";
                  sqlText += "@ItemClaimSD,";
                  sqlText += "@ItemClaimVAT,";
                  sqlText += "@ItemClaimCnF,";
                  sqlText += "@ItemClaimInsurance,";
                  sqlText += "@ItemClaimTVB,";
                  sqlText += "@ItemClaimTVA,";
                  sqlText += "@ItemClaimATV,";
                  sqlText += "@ItemClaimOthers,";
                  sqlText += "@ItemSubTotalDDB,";
                  sqlText += "@ItemUOMc,";
                  sqlText += "@ItemUOMn,";
                  sqlText += "@ItemUOMCD,";
                  sqlText += "@ItemUOMRD,";
                  sqlText += "@ItemUOMSD,";
                  sqlText += "@ItemUOMVAT,";
                  sqlText += "@ItemUOMCnF,";
                  sqlText += "@ItemUOMInsurance,";
                  sqlText += "@ItemUOMTVB,";
                  sqlText += "@ItemUOMTVA,";
                  sqlText += "@ItemUOMATV,";
                  sqlText += "@ItemUOMOthers,";
                  sqlText += "@ItemUOMSubTotalDDB,";
                  sqlText += "@ItemPost,";
                  sqlText += "@ItemCreatedBy,";
                  sqlText += "@ItemCreatedOn,";
                  sqlText += "@ItemLastModifiedBy,";
                  sqlText += "@ItemLastModifiedOn,";
                  sqlText += "@ItemPTransType,";
                  sqlText += "@ItemSalesInvoiceNo";
                  sqlText += ")	";


                  SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                  cmdInsDetail.Transaction = transaction;
                  cmdInsDetail.Parameters.AddWithValue("@newID", newID);
                  cmdInsDetail.Parameters.AddWithValue("@ItemDDBackDate", Item.DDBackDate);
                  cmdInsDetail.Parameters.AddWithValue("@ItemDDLineNo", Item.DDLineNo);
                  cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseInvoiceNo", Item.PurchaseInvoiceNo ?? Convert.DBNull);
                  cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseDate", Item.PurchaseDate);
                  cmdInsDetail.Parameters.AddWithValue("@ItemFgItemNo", Item.FgItemNo ?? Convert.DBNull);
                  cmdInsDetail.Parameters.AddWithValue("@ItemFGQty", Item.FGQty);
                  cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                  cmdInsDetail.Parameters.AddWithValue("@ItemBillOfEntry", Item.BillOfEntry ?? Convert.DBNull);
                  cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseUom", Item.PurchaseUom ?? Convert.DBNull);
                  cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseQuantity", Item.PurchaseQuantity);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUnitPrice", Item.UnitPrice);
                  cmdInsDetail.Parameters.AddWithValue("@ItemAV", Item.AV);
                  cmdInsDetail.Parameters.AddWithValue("@ItemCD", Item.CD);
                  cmdInsDetail.Parameters.AddWithValue("@ItemRD", Item.RD);
                  cmdInsDetail.Parameters.AddWithValue("@ItemSD", Item.SD);
                  cmdInsDetail.Parameters.AddWithValue("@ItemVAT", Item.VAT);
                  cmdInsDetail.Parameters.AddWithValue("@ItemCnF", Item.CnF);
                  cmdInsDetail.Parameters.AddWithValue("@ItemInsurance", Item.Insurance);
                  cmdInsDetail.Parameters.AddWithValue("@ItemTVB", Item.TVB);
                  cmdInsDetail.Parameters.AddWithValue("@ItemTVA", Item.TVA);
                  cmdInsDetail.Parameters.AddWithValue("@ItemATV", Item.ATV);
                  cmdInsDetail.Parameters.AddWithValue("@ItemOthers", Item.Others);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUseQuantity", Item.UseQuantity);
                  cmdInsDetail.Parameters.AddWithValue("@ItemClaimCD", Item.ClaimCD);
                  cmdInsDetail.Parameters.AddWithValue("@ItemClaimRD", Item.ClaimRD);
                  cmdInsDetail.Parameters.AddWithValue("@ItemClaimSD", Item.ClaimSD);
                  cmdInsDetail.Parameters.AddWithValue("@ItemClaimVAT", Item.ClaimVAT);
                  cmdInsDetail.Parameters.AddWithValue("@ItemClaimCnF", Item.ClaimCnF);
                  cmdInsDetail.Parameters.AddWithValue("@ItemClaimInsurance", Item.ClaimInsurance);
                  cmdInsDetail.Parameters.AddWithValue("@ItemClaimTVB", Item.ClaimTVB);
                  cmdInsDetail.Parameters.AddWithValue("@ItemClaimTVA", Item.ClaimTVA);
                  cmdInsDetail.Parameters.AddWithValue("@ItemClaimATV", Item.ClaimATV);
                  cmdInsDetail.Parameters.AddWithValue("@ItemClaimOthers", Item.ClaimOthers);
                  cmdInsDetail.Parameters.AddWithValue("@ItemSubTotalDDB", Item.SubTotalDDB);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUOMCD", Item.UOMCD);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUOMRD", Item.UOMRD);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUOMSD", Item.UOMSD);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUOMVAT", Item.UOMVAT);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUOMCnF", Item.UOMCnF);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUOMInsurance", Item.UOMInsurance);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUOMTVB", Item.UOMTVB);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUOMTVA", Item.UOMTVA);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUOMATV", Item.UOMATV);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUOMOthers", Item.UOMOthers);
                  cmdInsDetail.Parameters.AddWithValue("@ItemUOMSubTotalDDB", Item.UOMSubTotalDDB);
                  cmdInsDetail.Parameters.AddWithValue("@ItemPost", Item.Post ?? Convert.DBNull);
                  cmdInsDetail.Parameters.AddWithValue("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                  cmdInsDetail.Parameters.AddWithValue("@ItemCreatedOn", Item.CreatedOn);
                  cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedBy", Item.LastModifiedBy ?? Convert.DBNull);
                  cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedOn", Item.LastModifiedOn);
                  cmdInsDetail.Parameters.AddWithValue("@ItemPTransType", Item.PTransType ?? Convert.DBNull);
                  cmdInsDetail.Parameters.AddWithValue("@ItemSalesInvoiceNo", Item.SalesInvoiceNo ?? Convert.DBNull);

                  transResult = (int)cmdInsDetail.ExecuteNonQuery();

                  if (transResult <= 0)
                  {
                      throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                  }
                  #endregion Insert only DetailTable


                  #region Insert Issue and Receive if Transaction is not Other







                  #endregion Insert Issue and Receive if Transaction is not Other
              }


              #endregion Insert Detail Table
              #endregion Insert into Details(Insert complete in Header)

              #region return Current ID and Post Status

              sqlText = "";
              sqlText = sqlText + "select distinct  Post from dbo.DutyDrawBackHeader WHERE DDBackNo=@newID";
              SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
              cmdIPS.Transaction = transaction;
              cmdIPS.Parameters.AddWithValue("@newID", newID);

              PostStatus = (string)cmdIPS.ExecuteScalar();
              if (string.IsNullOrEmpty(PostStatus))
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgUnableCreatID);
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
              retResults[1] = MessageVM.issueMsgSaveSuccessfully;
              retResults[2] = "" + newID;
              retResults[3] = "" + PostStatus;
              #endregion SuccessResult

          }
         // #endregion Try

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
       public string[] DutyDrawBackUpdate(DDBHeaderVM Master, List<DDBDetailsVM> Details)
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

          #endregion Initializ

          #region Try
          try
          {
              #region Validation for Header

              if (Master == null)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
              }
              else if (Convert.ToDateTime(Master.DDBackDate) < DateTime.MinValue || Convert.ToDateTime(Master.DDBackDate) > DateTime.MaxValue)
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
              transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);

              #endregion open connection and transaction
              #region Fiscal Year Check

              string transactionDate = Master.DDBackDate;
              string transactionYearCheck = Convert.ToDateTime(Master.DDBackDate).ToString("yyyy-MM-dd");
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
              sqlText = sqlText + "select COUNT(DDBackNo) from DutyDrawBackHeader WHERE DDBackNo=@MasterDDBackNo ";
              SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
              cmdFindIdUpd.Transaction = transaction;
              cmdFindIdUpd.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);

              int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

              if (IDExist <= 0)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
              }

              #endregion Find ID for Update



              #region ID check completed,update Information in Header

              #region update Header
              sqlText = "";
              sqlText += " update DutyDrawBackHeader set  ";
              sqlText += " DDBackDate               =@MasterDDBackDate,";
              sqlText += " SalesInvoiceNo           =@MasterSalesInvoiceNo,";
              sqlText += " SalesDate                =@MasterSalesDate,";
              sqlText += " CustormerID              =@MasterCustormerID,";
              sqlText += " CurrencyId               =@MasterCurrencyId,";
              sqlText += " ExpCurrency              =@MasterExpCurrency,";
              sqlText += " BDTCurrency              =@MasterBDTCurrency,";
              sqlText += " FgItemNo                 =@MasterFgItemNo,";
              sqlText += " TotalClaimCD             =@MasterTotalClaimCD,";
              sqlText += " TotalClaimRD             =@MasterTotalClaimRD,";
              sqlText += " TotalClaimSD             =@MasterTotalClaimSD,";
              sqlText += " TotalDDBack              =@MasterTotalDDBack,";
              sqlText += " TotalClaimVAT            =@MasterTotalClaimVAT,";
              sqlText += " TotalClaimCnFAmount      =@MasterTotalClaimCnFAmount,";
              sqlText += " TotalClaimInsuranceAmount=@MasterTotalClaimInsuranceAmount,";
              sqlText += " TotalClaimTVBAmount      =@MasterTotalClaimTVBAmount,";
              sqlText += " TotalClaimTVAAmount      =@MasterTotalClaimTVAAmount,";
              sqlText += " TotalClaimATVAmount      =@MasterTotalClaimATVAmount,";
              sqlText += " TotalClaimOthersAmount   =@MasterTotalClaimOthersAmount,";
              sqlText += " Comments                 =@MasterComments, ";
              sqlText += " LastModifiedBy           =@MasterLastModifiedBy ,";
              sqlText += " LastModifiedOn           =@MasterLastModifiedOn, ";
              sqlText += " Post                     =@MasterPost ";
              sqlText += " where  DDBackNo          =@MasterDDBackNo ";


              SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
              cmdUpdate.Transaction = transaction;

              cmdUpdate.Parameters.AddWithValue("@MasterDDBackDate", Master.DDBackDate);
              cmdUpdate.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo ?? Convert.DBNull);
              cmdUpdate.Parameters.AddWithValue("@MasterSalesDate", Master.SalesDate);
              cmdUpdate.Parameters.AddWithValue("@MasterCustormerID", Master.CustormerID ?? Convert.DBNull);
              cmdUpdate.Parameters.AddWithValue("@MasterCurrencyId", Master.CurrencyId);
              cmdUpdate.Parameters.AddWithValue("@MasterExpCurrency", Master.ExpCurrency);
              cmdUpdate.Parameters.AddWithValue("@MasterBDTCurrency", Master.BDTCurrency);
              cmdUpdate.Parameters.AddWithValue("@MasterFgItemNo", Master.FgItemNo ?? Convert.DBNull);
              cmdUpdate.Parameters.AddWithValue("@MasterTotalClaimCD", Master.TotalClaimCD);
              cmdUpdate.Parameters.AddWithValue("@MasterTotalClaimRD", Master.TotalClaimRD);
              cmdUpdate.Parameters.AddWithValue("@MasterTotalClaimSD", Master.TotalClaimSD);
              cmdUpdate.Parameters.AddWithValue("@MasterTotalDDBack", Master.TotalDDBack);
              cmdUpdate.Parameters.AddWithValue("@MasterTotalClaimVAT", Master.TotalClaimVAT);
              cmdUpdate.Parameters.AddWithValue("@MasterTotalClaimCnFAmount", Master.TotalClaimCnFAmount);
              cmdUpdate.Parameters.AddWithValue("@MasterTotalClaimInsuranceAmount", Master.TotalClaimInsuranceAmount);
              cmdUpdate.Parameters.AddWithValue("@MasterTotalClaimTVBAmount", Master.TotalClaimTVBAmount);
              cmdUpdate.Parameters.AddWithValue("@MasterTotalClaimTVAAmount", Master.TotalClaimTVAAmount);
              cmdUpdate.Parameters.AddWithValue("@MasterTotalClaimATVAmount", Master.TotalClaimATVAmount);
              cmdUpdate.Parameters.AddWithValue("@MasterTotalClaimOthersAmount", Master.TotalClaimOthersAmount);
              cmdUpdate.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
              cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
              cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
              cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
              cmdUpdate.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo ?? Convert.DBNull);

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
                  sqlText += "select COUNT(DDBackNo) from DutyDrawBackDetails WHERE DDBackNo=@MasterDDBackNo ";
                  sqlText += " AND ItemNo=@ItemItemNo ";
                  sqlText += " AND FgItemNo=@ItemFgItemNo";
                  SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                  cmdFindId.Transaction = transaction;
                  cmdFindIdUpd.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);
                  cmdFindIdUpd.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                  cmdFindIdUpd.Parameters.AddWithValue("@ItemFgItemNo", Item.FgItemNo);

                  IDExist = (int)cmdFindId.ExecuteScalar();

                  if (IDExist <= 0)
                  {
                      // Insert
                      #region Insert only DetailTable

                      sqlText = "";
                      sqlText += " insert into DutyDrawBackDetails(";
                      sqlText += " DDBackNo,";
                      sqlText += " DDBackDate,";
                      sqlText += " DDLineNo,";
                      sqlText += " PurchaseInvoiceNo,";
                      sqlText += " PurchaseDate,";
                      sqlText += " FgItemNo,";
                      sqlText += " ItemNo,";
                      sqlText += " BillOfEntry,";
                      sqlText += " PurchaseUom,";
                      sqlText += " PurchaseQuantity,";
                      sqlText += " UnitPrice,";
                      sqlText += " AV,";
                      sqlText += " CD,";
                      sqlText += " RD,";
                      sqlText += " SD,";
                      sqlText += " VAT,";
                      sqlText += " CnF,";
                      sqlText += " Insurance,";
                      sqlText += " TVB,";
                      sqlText += " TVA,";
                      sqlText += " ATV,";
                      sqlText += " Others,";
                      sqlText += " UseQuantity,";
                      sqlText += " ClaimCD,";
                      sqlText += " ClaimRD,";
                      sqlText += " ClaimSD,";
                      sqlText += " ClaimVAT,";
                      sqlText += " ClaimCnF,";
                      sqlText += " ClaimInsurance,";
                      sqlText += " ClaimTVB,";
                      sqlText += " ClaimTVA,";
                      sqlText += " ClaimATV,";
                      sqlText += " ClaimOthers,";
                      sqlText += " SubTotalDDB,";
                      sqlText += " UOMc,";
                      sqlText += " UOMn,";
                      sqlText += " UOMCD,";
                      sqlText += " UOMRD,";
                      sqlText += " UOMSD,";
                      sqlText += " UOMVAT,";
                      sqlText += " UOMCnF,";
                      sqlText += " UOMInsurance,";
                      sqlText += " UOMTVB,";
                      sqlText += " UOMTVA,";
                      sqlText += " UOMATV,";
                      sqlText += " UOMOthers,";
                      sqlText += " UOMSubTotalDDB,";
                      sqlText += " Post,";
                      sqlText += " CreatedBy,";
                      sqlText += " CreatedOn,";
                      sqlText += " LastModifiedBy,";
                      sqlText += " SalesInvoiceNo,";
                      sqlText += " FGQty,";
                      sqlText += " PurchasetransactionType,";
                      sqlText += " LastModifiedOn";
                      sqlText += " )";

                      sqlText += " values(	";
                      //sqlText += "'" + Master.Id + "',";

                      sqlText += "@MasterDDBackNo,";
                      sqlText += "@ItemDDBackDate,";
                      sqlText += "@ItemDDLineNo,";
                      sqlText += "@ItemPurchaseInvoiceNo,";
                      sqlText += "@ItemPurchaseDate,";
                      sqlText += "@ItemFgItemNo,";
                      sqlText += "@ItemItemNo,";
                      sqlText += "@ItemBillOfEntry,";
                      sqlText += "@ItemPurchaseUom,";
                      sqlText += "@ItemPurchaseQuantity,";
                      sqlText += "@ItemUnitPrice,";
                      sqlText += "@ItemAV,";
                      sqlText += "@ItemCD,";
                      sqlText += "@ItemRD,";
                      sqlText += "@ItemSD,";
                      sqlText += "@ItemVAT,";
                      sqlText += "@ItemCnF,";
                      sqlText += "@ItemInsurance,";
                      sqlText += "@ItemTVB,";
                      sqlText += "@ItemTVA,";
                      sqlText += "@ItemATV,";
                      sqlText += "@ItemOthers,";
                      sqlText += "@ItemUseQuantity,";
                      sqlText += "@ItemClaimCD,";
                      sqlText += "@ItemClaimRD,";
                      sqlText += "@ItemClaimSD,";
                      sqlText += "@ItemClaimVAT,";
                      sqlText += "@ItemClaimCnF,";
                      sqlText += "@ItemClaimInsurance,";
                      sqlText += "@ItemClaimTVB,";
                      sqlText += "@ItemClaimTVA,";
                      sqlText += "@ItemClaimATV,";
                      sqlText += "@ItemClaimOthers,";
                      sqlText += "@ItemSubTotalDDB,";
                      sqlText += "@ItemUOMc,";
                      sqlText += "@ItemUOMn,";
                      sqlText += "@ItemUOMCD,";
                      sqlText += "@ItemUOMRD,";
                      sqlText += "@ItemUOMSD,";
                      sqlText += "@ItemUOMVAT,";
                      sqlText += "@ItemUOMCnF,";
                      sqlText += "@ItemUOMInsurance,";
                      sqlText += "@ItemUOMTVB,";
                      sqlText += "@ItemUOMTVA,";
                      sqlText += "@ItemUOMATV,";
                      sqlText += "@ItemUOMOthers,";
                      sqlText += "@ItemUOMSubTotalDDB,";
                      sqlText += "@ItemPost,";
                      sqlText += "@ItemCreatedBy,";
                      sqlText += "@ItemCreatedOn,";
                      sqlText += "@ItemLastModifiedBy,";
                      sqlText += "@ItemSalesInvoiceNo,";
                      sqlText += "@ItemFGQty,";
                      sqlText += "@ItemPTransType,";

                      sqlText += "@ItemLastModifiedOn ";
                      sqlText += ")	";




                      SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                      cmdInsDetail.Transaction = transaction;
                      cmdInsDetail.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemDDBackDate", Item.DDBackDate);
                      cmdInsDetail.Parameters.AddWithValue("@ItemDDLineNo", Item.DDLineNo);
                      cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseInvoiceNo", Item.PurchaseInvoiceNo ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseDate", Item.PurchaseDate);
                      cmdInsDetail.Parameters.AddWithValue("@ItemFgItemNo", Item.FgItemNo ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemBillOfEntry", Item.BillOfEntry ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseUom", Item.PurchaseUom ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseQuantity", Item.PurchaseQuantity);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUnitPrice", Item.UnitPrice);
                      cmdInsDetail.Parameters.AddWithValue("@ItemAV", Item.AV);
                      cmdInsDetail.Parameters.AddWithValue("@ItemCD", Item.CD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemRD", Item.RD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemSD", Item.SD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemVAT", Item.VAT);
                      cmdInsDetail.Parameters.AddWithValue("@ItemCnF", Item.CnF);
                      cmdInsDetail.Parameters.AddWithValue("@ItemInsurance", Item.Insurance);
                      cmdInsDetail.Parameters.AddWithValue("@ItemTVB", Item.TVB);
                      cmdInsDetail.Parameters.AddWithValue("@ItemTVA", Item.TVA);
                      cmdInsDetail.Parameters.AddWithValue("@ItemATV", Item.ATV);
                      cmdInsDetail.Parameters.AddWithValue("@ItemOthers", Item.Others);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUseQuantity", Item.UseQuantity);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimCD", Item.ClaimCD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimRD", Item.ClaimRD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimSD", Item.ClaimSD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimVAT", Item.ClaimVAT);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimCnF", Item.ClaimCnF);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimInsurance", Item.ClaimInsurance);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimTVB", Item.ClaimTVB);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimTVA", Item.ClaimTVA);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimATV", Item.ClaimATV);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimOthers", Item.ClaimOthers);
                      cmdInsDetail.Parameters.AddWithValue("@ItemSubTotalDDB", Item.SubTotalDDB);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMCD", Item.UOMCD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMRD", Item.UOMRD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMSD", Item.UOMSD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMVAT", Item.UOMVAT);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMCnF", Item.UOMCnF);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMInsurance", Item.UOMInsurance);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMTVB", Item.UOMTVB);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMTVA", Item.UOMTVA);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMATV", Item.UOMATV);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMOthers", Item.UOMOthers);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMSubTotalDDB", Item.UOMSubTotalDDB);
                      cmdInsDetail.Parameters.AddWithValue("@ItemPost", Item.Post ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemCreatedOn", Item.CreatedOn);
                      cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedBy", Item.LastModifiedBy ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemSalesInvoiceNo", Item.SalesInvoiceNo ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemFGQty", Item.FGQty);
                      cmdInsDetail.Parameters.AddWithValue("@ItemPTransType", Item.PTransType ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedOn", Item.LastModifiedOn);

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
                      sqlText += " update DutyDrawBackDetails set ";

                      sqlText += " DDBackDate       =@ItemDDBackDate,";
                      sqlText += " DDLineNo         =@ItemDDLineNo,";
                      sqlText += " PurchaseInvoiceNo=@ItemPurchaseInvoiceNo,";
                      sqlText += " PurchaseDate     =@ItemPurchaseDate,";
                      sqlText += " FgItemNo         =@ItemFgItemNo,";
                      sqlText += " ItemNo           =@ItemItemNo,";
                      sqlText += " BillOfEntry      =@ItemBillOfEntry,";
                      sqlText += " PurchaseUom      =@ItemPurchaseUom,";
                      sqlText += " PurchaseQuantity =@ItemPurchaseQuantity,";
                      sqlText += " UnitPrice        =@ItemUnitPrice,";
                      sqlText += " AV               =@ItemAV,";
                      sqlText += " CD               =@ItemCD,";
                      sqlText += " RD               =@ItemRD,";
                      sqlText += " SD               =@ItemSD,";
                      sqlText += " VAT              =@ItemVAT,";
                      sqlText += " CnF              =@ItemCnF,";
                      sqlText += " Insurance        =@ItemInsurance,";
                      sqlText += " TVB              =@ItemTVB,";
                      sqlText += " TVA              =@ItemTVA,";
                      sqlText += " ATV              =@ItemATV,";
                      sqlText += " Others           =@ItemOthers,";
                      sqlText += " UseQuantity      =@ItemUseQuantity,";
                      sqlText += " ClaimCD          =@ItemClaimCD,";
                      sqlText += " ClaimRD          =@ItemClaimRD,";
                      sqlText += " ClaimSD          =@ItemClaimSD,";
                      sqlText += " ClaimVAT         =@ItemClaimVAT,";
                      sqlText += " ClaimCnF         =@ItemClaimCnF,";
                      sqlText += " ClaimInsurance   =@ItemClaimInsurance,";
                      sqlText += " ClaimTVB         =@ItemClaimTVB,";
                      sqlText += " ClaimTVA         =@ItemClaimTVA,";
                      sqlText += " ClaimATV         =@ItemClaimATV,";
                      sqlText += " ClaimOth ers     =@ItemClaimOthers,";
                      sqlText += " SubTotalDDB      =@ItemSubTotalDDB,";
                      sqlText += " UOMc             =@ItemUOMc,";
                      sqlText += " UOMn             =@ItemUOMn,";
                      sqlText += " UOMCD            =@ItemUOMCD,";
                      sqlText += " UOMRD            =@ItemUOMRD,";
                      sqlText += " UOMSD            =@ItemUOMSD,";
                      sqlText += " UOMVAT           =@ItemUOMVAT,";
                      sqlText += " UOMCnF           =@ItemUOMCnF,";
                      sqlText += " UOMInsurance     =@ItemUOMInsurance,";
                      sqlText += " UOMTVB           =@ItemUOMTVB,";
                      sqlText += " UOMTVA           =@ItemUOMTVA,";
                      sqlText += " UOMATV           =@ItemUOMATV,";
                      sqlText += " UOMOthers        =@ItemUOMOthers,";
                      sqlText += " UOMSubTotalDDB   =@ItemUOMSubTotalDDB,";
                      sqlText += " Post             =@ItemPost,";
                      sqlText += " LastModifiedBy   =@ItemLastModifiedBy,";
                      sqlText += " LastModifiedOn   =@ItemLastModifiedOn,";
                      sqlText += " SalesInvoiceNo   =@ItemSalesInvoiceNo,";
                      sqlText += " FGQty            =@ItemFGQty";
                      sqlText += " where  DDBackNo  =@MasterDDBackNo ";
                      sqlText += " and ItemNo       =@ItemItemNo";
                      sqlText += " AND FgItemNo     =@ItemFgItemNo";


                      SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                      cmdInsDetail.Transaction = transaction;

                      cmdInsDetail.Parameters.AddWithValue("@ItemDDBackDate",Item.DDBackDate);
                      cmdInsDetail.Parameters.AddWithValue("@ItemDDLineNo",Item.DDLineNo);
                      cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseInvoiceNo", Item.PurchaseInvoiceNo ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseDate",Item.PurchaseDate);
                      cmdInsDetail.Parameters.AddWithValue("@ItemFgItemNo", Item.FgItemNo ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemBillOfEntry", Item.BillOfEntry ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseUom", Item.PurchaseUom ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemPurchaseQuantity",Item.PurchaseQuantity);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUnitPrice",Item.UnitPrice);
                      cmdInsDetail.Parameters.AddWithValue("@ItemAV",Item.AV);
                      cmdInsDetail.Parameters.AddWithValue("@ItemCD",Item.CD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemRD",Item.RD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemSD",Item.SD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemVAT",Item.VAT);
                      cmdInsDetail.Parameters.AddWithValue("@ItemCnF",Item.CnF);
                      cmdInsDetail.Parameters.AddWithValue("@ItemInsurance",Item.Insurance);
                      cmdInsDetail.Parameters.AddWithValue("@ItemTVB",Item.TVB);
                      cmdInsDetail.Parameters.AddWithValue("@ItemTVA",Item.TVA);
                      cmdInsDetail.Parameters.AddWithValue("@ItemATV",Item.ATV);
                      cmdInsDetail.Parameters.AddWithValue("@ItemOthers",Item.Others);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUseQuantity",Item.UseQuantity);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimCD",Item.ClaimCD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimRD",Item.ClaimRD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimSD",Item.ClaimSD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimVAT",Item.ClaimVAT);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimCnF",Item.ClaimCnF);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimInsurance",Item.ClaimInsurance);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimTVB",Item.ClaimTVB);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimTVA",Item.ClaimTVA);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimATV",Item.ClaimATV);
                      cmdInsDetail.Parameters.AddWithValue("@ItemClaimOthers",Item.ClaimOthers);
                      cmdInsDetail.Parameters.AddWithValue("@ItemSubTotalDDB",Item.SubTotalDDB);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMc",Item.UOMc);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMCD",Item.UOMCD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMRD",Item.UOMRD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMSD",Item.UOMSD);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMVAT",Item.UOMVAT);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMCnF",Item.UOMCnF);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMInsurance",Item.UOMInsurance);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMTVB",Item.UOMTVB);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMTVA",Item.UOMTVA);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMATV",Item.UOMATV);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMOthers",Item.UOMOthers);
                      cmdInsDetail.Parameters.AddWithValue("@ItemUOMSubTotalDDB",Item.UOMSubTotalDDB);
                      cmdInsDetail.Parameters.AddWithValue("@ItemPost", Item.Post ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedBy", Item.LastModifiedBy ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedOn",Item.LastModifiedOn);
                      cmdInsDetail.Parameters.AddWithValue("@ItemSalesInvoiceNo", Item.SalesInvoiceNo ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemFGQty",Item.FGQty);
                      cmdInsDetail.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemFgItemNo", Item.FgItemNo ?? Convert.DBNull);

                      transResult = (int)cmdInsDetail.ExecuteNonQuery();

                      if (transResult <= 0)
                      {
                          throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                      }
                      #endregion Update only DetailTable
                      
                  }

                  #endregion Find Transaction Mode Insert or Update
              }


              #endregion Update Detail Table

              #endregion  Update into Details(Update complete in Header)


              #region return Current ID and Post Status

              sqlText = "";
              sqlText = sqlText + "select distinct Post from DutyDrawBackHeader WHERE DDBackNo=@MasterDDBackNo ";
              SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
              cmdIPS.Transaction = transaction;
              cmdIPS.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);

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
              retResults[2] = Master.DDBackNo;
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
       public string[] DutyDrawBackPost(DDBHeaderVM Master, List<DDBDetailsVM> Details)
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

          #endregion Initializ

          #region Try
          try
          {
              #region Validation for Header

              if (Master == null)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
              }
              else if (Convert.ToDateTime(Master.DDBackDate) < DateTime.MinValue || Convert.ToDateTime(Master.DDBackDate) > DateTime.MaxValue)
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
              transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);

              #endregion open connection and transaction
              #region Fiscal Year Check

              string transactionDate = Master.DDBackDate;
              string transactionYearCheck = Convert.ToDateTime(Master.DDBackDate).ToString("yyyy-MM-dd");
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
              sqlText = sqlText + "select COUNT(DDBackNo) from DutyDrawBackHeader WHERE DDBackNo=@MasterDDBackNo ";
              SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
              cmdFindIdUpd.Transaction = transaction;
              cmdFindIdUpd.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);

              int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

              if (IDExist <= 0)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
              }

              #endregion Find ID for Update



              #region ID check completed,update Information in Header

              #region update Header
              sqlText = "";
              sqlText += " update DutyDrawBackHeader set  ";
              sqlText += " LastModifiedBy= @MasterLastModifiedBy,";
              sqlText += " LastModifiedOn= @MasterLastModifiedOn, ";
              sqlText += " Post          = @MasterPost ";
              sqlText += " where DDBackNo= @MasterDDBackNo ";


              SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
              cmdUpdate.Transaction = transaction;
              cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy ", Master.LastModifiedBy);
              cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn ", Master.LastModifiedOn);
              cmdUpdate.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);
              cmdUpdate.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo ?? Convert.DBNull);

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
                  sqlText += "select COUNT(DDBackNo) from DutyDrawBackDetails WHERE DDBackNo@MasterDDBackNo ";
                  sqlText += " AND ItemNo=@ItemItemNo";
                  sqlText += " AND FgItemNo=@ItemFgItemNo ";

                  SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                  cmdFindId.Transaction = transaction;
                  cmdFindId.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);
                  cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                  cmdFindId.Parameters.AddWithValue("@ItemFgItemNo", Item.FgItemNo);

                  IDExist = (int)cmdFindId.ExecuteScalar();

                  if (IDExist <= 0)
                  {
                     
                          throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                     
                  }
                  else
                  {
                      //Update

                      #region Update only DetailTable

                      sqlText = "";
                      sqlText += " update DutyDrawBackDetails set ";
                      sqlText += " Post             =@ItemPost,";
                      sqlText += " LastModifiedBy   =@ItemLastModifiedBy,";
                      sqlText += " LastModifiedOn   =@ItemLastModifiedOn";
                      sqlText += " where  DDBackNo  =@MasterDDBackNo ";
                      sqlText += " and ItemNo       =@ItemItemNo";
                      sqlText += " AND FgItemNo     =@ItemFgItemNo";




                      SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                      cmdInsDetail.Transaction = transaction;

                      cmdInsDetail.Parameters.AddWithValue("@ItemPost", Item.Post ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedBy", Item.LastModifiedBy ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemLastModifiedOn", Item.LastModifiedOn);
                      cmdInsDetail.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                      cmdInsDetail.Parameters.AddWithValue("@ItemFgItemNo", Item.FgItemNo ?? Convert.DBNull);

                      transResult = (int)cmdInsDetail.ExecuteNonQuery();

                      if (transResult <= 0)
                      {
                          throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                      }
                      #endregion Update only DetailTable
                      #region Update Issue and Receive if Transaction is not Other







                      #endregion Update Issue and Receive if Transaction is not Other
                  }

                  #endregion Find Transaction Mode Insert or Update
              }


              #endregion Update Detail Table

              #endregion  Update into Details(Update complete in Header)
              #region return Current ID and Post Status

              sqlText = "";
              sqlText = sqlText + "select distinct Post from DutyDrawBackHeader WHERE DDBackNo=@MasterDDBackNo";
              SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
              cmdIPS.Transaction = transaction;
              cmdIPS.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);


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
              retResults[1] = MessageVM.issueMsgSuccessfullyPost;
              retResults[2] = Master.DDBackNo;
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
      public DataTable SearchDDBackHeader(string DDBackNo, string DDBackFromDate, string DDBackToDate, string DDBackSaleFromDate, string DDBackSaleToDate,string SalesInvoicNo,string CustomerName,string FinishGood,string Post)
      {
          #region Variables

          SqlConnection currConn = null;
          int transResult = 0;
          int countId = 0;
          string sqlText = "";
          DataTable dataTable = new DataTable("SearchDDBackHeade");

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

              sqlText = string.Format(@"SELECT 
       d.DDBackNo
      ,convert (varchar,d.DDBackDate,120)DDBackDate
      ,d.SalesInvoiceNo
      ,convert (varchar,d.SalesDate,120)SalesDate,
isnull(d.CustormerID,0)CustormerID,
isnull(cm.CustomerName,0)CustomerName,
isnull(d.CurrencyId,0)CurrencyId,
isnull(c.CurrencyCode,0)CurrencyCode,
isnull(d.ExpCurrency,0)ExpCurrency,
isnull(d.BDTCurrency,0)BDTCurrency,
isnull(d.FgItemNo,0)FgItemNo,
isnull(ps.ProductName,0)ProductName,
isnull(d.TotalClaimCD,0)TotalClaimCD,
isnull(d.TotalClaimRD,0)TotalClaimRD,
isnull(d.TotalClaimSD,0)TotalClaimSD,
isnull(d.TotalDDBack,0)TotalDDBack,
isnull(d.TotalClaimVAT,0)TotalClaimVAT,
isnull(d.TotalClaimCnFAmount,0)TotalClaimCnFAmount,
isnull(d.TotalClaimInsuranceAmount,0)TotalClaimInsuranceAmount,
isnull(d.TotalClaimTVBAmount,0)TotalClaimTVBAmount,
isnull(d.TotalClaimTVAAmount,0)TotalClaimTVAAmount,
isnull(d.TotalClaimATVAmount,0)TotalClaimATVAmount,
isnull(d.TotalClaimOthersAmount,0)TotalClaimOthersAmount,
isnull(d.Comments,'N/A')Comments
      ,d.CreatedBy
      ,d.CreatedOn
      ,d.LastModifiedBy
      ,d.LastModifiedOn
      ,d.Post
  FROM [dbo].[DutyDrawBackHeader] d LEFT OUTER JOIN Currencies c on c.CurrencyID=d.CurrencyId
left outer join Customers cm on cm.CustomerID=d.CustormerID
left outer join Products ps on ps.ItemNo=d.FgItemNo

 WHERE
                            (d.DDBackNo  LIKE '%' +  @DDBackNo   + '%' OR @DDBackNo IS NULL)
                            AND (d.DDBackDate>= @DDBackFromDate OR @DDBackFromDate IS NULL)
                            AND (d.DDBackDate <dateadd(d,1, @DDBackToDate) OR @DDBackToDate IS NULL)
                            AND (d.SalesDate>= @DDBackSaleFromDate OR @DDBackSaleFromDate IS NULL)
                            AND (d.SalesDate <dateadd(d,1, @DDBackSaleToDate) OR @DDBackSaleToDate IS NULL)
                            AND (d.SalesInvoiceNo  LIKE '%' +  @SalesInvoicNo   + '%' OR @SalesInvoicNo IS NULL) 
                            AND (d.CustormerID  LIKE '%' +  @CustomerName   + '%' OR @CustomerName IS NULL)
                            AND (d.FgItemNo  LIKE '%' +  @FinishGood   + '%' OR @FinishGood IS NULL)
                            AND (d.Post  LIKE '%' +  @Post   + '%' OR @Post IS NULL)

                            ");



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

              if (!objCommIssueHeader.Parameters.Contains("@DDBackNo"))
              { objCommIssueHeader.Parameters.AddWithValue("@DDBackNo", DDBackNo); }
              else { objCommIssueHeader.Parameters["@DDBackNo"].Value = DDBackNo; }

              if (DDBackFromDate == "")
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackFromDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackFromDate", System.DBNull.Value); }
                  else { objCommIssueHeader.Parameters["@DDBackFromDate"].Value = System.DBNull.Value; }
              }
              else
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackFromDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackFromDate", DDBackFromDate); }
                  else { objCommIssueHeader.Parameters["@DDBackFromDate"].Value = DDBackFromDate; }
              }
              if (DDBackToDate == "")
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackToDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackToDate", System.DBNull.Value); }
                  else { objCommIssueHeader.Parameters["@DDBackToDate"].Value = System.DBNull.Value; }
              }
              else
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackToDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackToDate", DDBackToDate); }
                  else { objCommIssueHeader.Parameters["@DDBackToDate"].Value = DDBackToDate; }
              }

              if (DDBackSaleFromDate == "")
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackSaleFromDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackSaleFromDate", System.DBNull.Value); }
                  else { objCommIssueHeader.Parameters["@DDBackSaleFromDate"].Value = System.DBNull.Value; }
              }
              else
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackSaleFromDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackSaleFromDate", DDBackFromDate); }
                  else { objCommIssueHeader.Parameters["@DDBackSaleFromDate"].Value = DDBackFromDate; }
              }
              if (DDBackSaleToDate == "")
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackSaleToDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackSaleToDate", System.DBNull.Value); }
                  else { objCommIssueHeader.Parameters["@DDBackSaleToDate"].Value = System.DBNull.Value; }
              }
              else
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackSaleToDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackSaleToDate", DDBackToDate); }
                  else { objCommIssueHeader.Parameters["@DDBackSaleToDate"].Value = DDBackToDate; }
              }

              if (!objCommIssueHeader.Parameters.Contains("@SalesInvoicNo"))
              { objCommIssueHeader.Parameters.AddWithValue("@SalesInvoicNo", SalesInvoicNo); }
              else { objCommIssueHeader.Parameters["@SalesInvoicNo"].Value = SalesInvoicNo; }

              // Common Filed
              if (!objCommIssueHeader.Parameters.Contains("@CustomerName"))
              { objCommIssueHeader.Parameters.AddWithValue("@CustomerName", CustomerName); }
              else { objCommIssueHeader.Parameters["@CustomerName"].Value = CustomerName; }


              // Common Filed
              if (!objCommIssueHeader.Parameters.Contains("@FinishGood"))
              { objCommIssueHeader.Parameters.AddWithValue("@FinishGood", FinishGood); }
              else { objCommIssueHeader.Parameters["@FinishGood"].Value = FinishGood; }
              

              #endregion Parameter

              SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommIssueHeader);
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

      public DataTable SearchddBackDetails(string DDBackNo, string oldSaleID)
      {
          #region Variables

          SqlConnection currConn = null;
          int transResult = 0;
          int countId = 0;
          string sqlText = "";
          DataTable dataTable = new DataTable("ddBackDetails");

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
SELECT d.[DDBackNo]
      ,convert (varchar,d.[DDBackDate],120)[DDBackDate]
      ,d.[DDLineNo]
      ,d.[PurchaseInvoiceNo]
      ,convert (varchar,d.[PurchaseDate],120)[PurchaseDate]      
      ,d.[ItemNo]
      ,pR.[ProductCode] pitemcode
	  ,pR.[ProductName] pitemname
	  ,d.[FgItemNo]
	  ,pF.[ProductCode] fitemcode
	  ,pF.[ProductName] fitemname
      ,d.[BillOfEntry]
      ,d.[PurchaseUom]
      ,d.[PurchaseQuantity]
      ,d.[UnitPrice]
      ,d.[AV]
      ,d.[CD]
      ,d.[RD]
      ,d.[SD]
      ,d.[VAT]
      ,d.[CnF]
      ,d.[Insurance]
      ,d.[TVB]
      ,d.[TVA]
      ,d.[ATV]
      ,d.[Others]
      ,d.[UseQuantity]
      ,d.[ClaimCD]
      ,d.[ClaimRD]
      ,d.[ClaimSD]
      ,d.[ClaimVAT]
      ,d.[ClaimCnF]
      ,d.[ClaimInsurance]
      ,d.[ClaimTVB]
      ,d.[ClaimTVA]
      ,d.[ClaimATV]
      ,d.[ClaimOthers]
      ,d.[SubTotalDDB]
      ,d.[UOMc]
      ,d.[UOMn]
      ,d.[UOMCD]
      ,d.[UOMRD]
      ,d.[UOMSD]
      ,d.[UOMVAT]
      ,d.[UOMCnF]
      ,d.[UOMInsurance]
      ,d.[UOMTVB]
      ,d.[UOMTVA]
      ,d.[UOMATV]
      ,d.[UOMOthers]
      ,d.[UOMSubTotalDDB]
      ,d.[Post]
      ,d.[CreatedBy]
      ,d.[CreatedOn]
      ,d.[LastModifiedBy]
      ,d.[LastModifiedOn]
,isnull(nullif(d.SalesInvoiceNo,''),'-')SalesInvoiceNo
,isnull(nullif(d.PurchasetransactionType,''),'-')PurchasetransactionType
,isnull(d.FGQty,0)FGQty

  FROM [dbo].[DutyDrawBackDetails] d

left outer join 
                        Products pR on pR.ItemNo=d.ItemNo left outer 
					 join Products pF on pF.ItemNo=d.FgItemNo

					 WHERE 
                         DDBackNo = @DDBackNo


";

              #endregion

              #region SQL Command

              SqlCommand objCommIssueDetail = new SqlCommand();
              objCommIssueDetail.Connection = currConn;

              objCommIssueDetail.CommandText = sqlText;
              objCommIssueDetail.CommandType = CommandType.Text;

              #endregion

              #region Parameter

              if (!objCommIssueDetail.Parameters.Contains("@DDBackNo"))
              { objCommIssueDetail.Parameters.AddWithValue("@DDBackNo", DDBackNo); }
              else { objCommIssueDetail.Parameters["@DDBackNo"].Value = DDBackNo; }

              #endregion Parameter

              SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommIssueDetail);
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

      public DataTable Purchase_DDBQty(string PurchaseInvoiceNo, string PurItemNo, SqlConnection currConn, SqlTransaction transaction)
      {
          #region Variables

          int transResult = 0;
          int countId = 0;
          string sqlText = "";
          DataTable dataTable = new DataTable("ddBackDetails");

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


sqlText = "  ";
sqlText += " DECLARE @PurchaseQty DECIMAL(25,9); ";
sqlText += " DECLARE @TotalDDBQty DECIMAL(25,9); ";
sqlText += " SELECT @PurchaseQty=isnull(uomQty,0)  FROM PurchaseInvoiceDetails  ";
sqlText += " WHERE  ";
sqlText += " PurchaseInvoiceNo='"+PurchaseInvoiceNo +"' ";
sqlText += " and ItemNo='" + PurItemNo + "' ";
sqlText += " SELECT @TotalDDBQty=isnull(sum(isnull(UseQuantity,0)* isnull(uomc,1)),0) ";
sqlText += " FROM DutyDrawBackDetails   ";
sqlText += " WHERE  ";
sqlText += " PurchaseInvoiceNo='" + PurchaseInvoiceNo + "' ";
sqlText += " and ItemNo='"+ PurItemNo +"' ";
sqlText += " SELECT @PurchaseQty PurchaseQty,@TotalDDBQty TotalDDBQty ";

              #endregion

              #region SQL Command

              SqlCommand objCommIssueDetail = new SqlCommand();
              objCommIssueDetail.Connection = currConn;

              objCommIssueDetail.CommandText = sqlText;
              objCommIssueDetail.CommandType = CommandType.Text;

              #endregion

             

              SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommIssueDetail);
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
      
    }
}
