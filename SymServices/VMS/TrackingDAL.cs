using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using SymViewModel.VMS;

namespace SymServices.VMS
{
   public class TrackingDAL
    {
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        public DataTable FindTrackingItems(string fItemNo,string vatName,string effectDate)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("TrackingDetail");

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

                string BomId = string.Empty;
                DateTime BOMDate = DateTime.MinValue;
                string bomDate = "";
                #region Last BOMId

                sqlText = "  ";
                sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                sqlText += " where ";
                sqlText += " FinishItemNo   =@fItemNo";
                sqlText += " and vatname    =@vatName";
                sqlText += " and effectdate<=@effectDate";
                sqlText += " and post='Y' ";
                sqlText += " order by effectdate desc ";

                SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                //cmdBomId.Transaction = transaction;
                cmdBomId.Parameters.AddWithValue("@fItemNo", fItemNo);
                cmdBomId.Parameters.AddWithValue("@vatName", vatName);
                cmdBomId.Parameters.AddWithValue("@effectDate", effectDate);

                if (cmdBomId.ExecuteScalar() == null)
                {
                    throw new ArgumentNullException("Tracking Info", "No Data found for this item");
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
                sqlText += " where FinishItemNo=@fItemNo ";
                sqlText += " and vatname=@vatName ";
                sqlText += " and effectdate<=@effectDate";
                sqlText += " and post='Y' ";
                sqlText += " order by effectdate desc ";

                SqlCommand cmdBomEDate = new SqlCommand(sqlText, currConn);
                //cmdBomEDate.Transaction = transaction;
                cmdBomEDate.Parameters.AddWithValue("@fItemNo", fItemNo);
                cmdBomEDate.Parameters.AddWithValue("@vatName", vatName);
                cmdBomEDate.Parameters.AddWithValue("@effectDate", effectDate);
                if (cmdBomEDate.ExecuteScalar() == null)
                {
                    throw new ArgumentNullException("TrackingInfo", "No Data found for this item");
                    BOMDate = DateTime.MinValue;
                    bomDate = BOMDate.Date.ToString("yyyy/MM/dd 00:00:00");
                }
                else
                {
                    BOMDate = (DateTime)cmdBomEDate.ExecuteScalar();
                    bomDate = BOMDate.Date.ToString("yyyy/MM/dd 00:00:00");
                }

                #endregion Last BOMDate

                #region Find Raw Item From BOM  and update Stock

                sqlText = "";
                sqlText +=
                    " SELECT Distinct  b.RawItemNo,b.UseQuantity,b.WastageQuantity,b.UOMUQty,b.UOMWQty from BOMRaws b,Trackings t  ";
                sqlText += " where b.RawItemNo = t.ItemNo ";
                sqlText += " and b.FinishItemNo='" + fItemNo + "' ";
                sqlText += " and b.vatname='" + vatName + "' ";
                sqlText += " and b.effectdate='" + bomDate + "'";
                sqlText += " and b.post='Y' ";
                sqlText += "   and (rawitemtype='raw' or rawitemtype='finish') ";

                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                //cmdRIFB.Transaction = transaction;
                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                reportDataAdapt.Fill(dataTable);

                if (dataTable == null)
                {
                    throw new ArgumentNullException("TrackingInfo", "There is no data to Post");

                }
                else if (dataTable.Rows.Count <= 0)
                {
                    throw new ArgumentNullException("TrackingInfo","No Data found for the Item Code (" + fItemNo + ")");
                                                    
                }

                #endregion Find Raw Item From BOM and update Stock



                #endregion

            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
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

            return dataTable;
        }

        public DataTable SearchTrackingItems(string itemNo, string isIssue, string isReceive, string isSale, string finishItemNo,
            string receiveNo, string issueNo, string saleInvoiceNo)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

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
                #region Parameter
//                Declare @itemNo varchar(100) 
//Declare @FinishItemNo varchar(100) 
//Declare @saleInvoiceNo varchar(100) 
//Declare @isSale varchar(100) 
//Declare @isReceive varchar(100) 
//Declare @receiveNo varchar(100) 
//Declare @issueNo varchar(100) 
//Declare @isIssue varchar(100)

//SET @itemNo='36'
//SET @FinishItemNo =''
//SET @saleInvoiceNo=''
//SET @isSale =''
//SET @isReceive ='N'
//SET @receiveNo =''
//SET @issueNo =''
//SET @isIssue =''
                #endregion


                sqlText = "";
                sqlText += @"

SELECT pr.[ProductCode]
	  ,pr.[ProductName] 
      ,t.[ItemNo]
	  ,t.[TrackLineNo]
      ,t.[Heading1]
      ,t.[Heading2]
      ,t.[IsSale]
      ,t.[SaleInvoiceNo]
      ,t.[IsIssue]
      ,t.[IssueNo]
      ,t.[IsReceive]
      ,t.[ReceiveNo]
      ,t.[IsPurchase]
      ,t.[FinishItemNo]

  FROM Trackings t,Products pr

WHERE t.[ItemNo]=pr.[ItemNo] And t.IsPurchase='Y' And t.Post='Y'
           AND (t.ItemNo LIKE '%' +  @itemNo + '%' OR @itemNo IS NULL) 
		   AND ((IsIssue =@isIssue )  OR (IssueNo= @issueNo))
		   AND ((IsReceive =@isReceive )  OR (ReceiveNo= @receiveNo))
		   AND ((IsSale =@isSale )  OR (SaleInvoiceNo= @saleInvoiceNo))
		   AND (FinishItemNo LIKE '%' +  @FinishItemNo + '%' OR @FinishItemNo IS NULL)


  --       AND ((IsSale LIKE '%' +  @isIssue + '%' OR @isIssue IS NULL)  
				--OR (IssueNo LIKE '%' +  @issueNo + '%' OR @issueNo IS NULL))  
    --       AND ((IsReceive LIKE '%' +  @isReceive + '%' OR @isReceive IS NULL)  
				--OR (ReceiveNo LIKE '%' +  @receiveNo + '%' OR @receiveNo IS NULL)  )
		  
    --       AND ((IsSale LIKE '%' +  @isSale + '%' OR @isSale IS NULL)  
				--OR (SaleInvoiceNo LIKE '%' +  @saleInvoiceNo + '%' OR @saleInvoiceNo IS NULL) )                       

";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                if (itemNo == "" || string.IsNullOrEmpty(itemNo))
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@itemNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", itemNo); }
                    else { objCommProduct.Parameters["@itemNo"].Value = itemNo; }
                }

               
                if (string.IsNullOrEmpty(isIssue))
                {
                    if (!objCommProduct.Parameters.Contains("@isIssue"))
                    { objCommProduct.Parameters.AddWithValue("@isIssue", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@isIssue"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@isIssue"))
                    { objCommProduct.Parameters.AddWithValue("@isIssue", isIssue); }
                    else { objCommProduct.Parameters["@isIssue"].Value = isIssue; }
                }

                if (string.IsNullOrEmpty(issueNo))
                {
                    if (!objCommProduct.Parameters.Contains("@issueNo"))
                    { objCommProduct.Parameters.AddWithValue("@issueNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@issueNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@issueNo"))
                    { objCommProduct.Parameters.AddWithValue("@issueNo", issueNo); }
                    else { objCommProduct.Parameters["@issueNo"].Value = issueNo; }
                }

                if (string.IsNullOrEmpty(isReceive))
                {
                    if (!objCommProduct.Parameters.Contains("@isReceive"))
                    { objCommProduct.Parameters.AddWithValue("@isReceive", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@isReceive"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@isReceive"))
                    { objCommProduct.Parameters.AddWithValue("@isReceive", isReceive); }
                    else { objCommProduct.Parameters["@isReceive"].Value = isReceive; }
                }

                if (string.IsNullOrEmpty(receiveNo))
                {
                    if (!objCommProduct.Parameters.Contains("@receiveNo"))
                    { objCommProduct.Parameters.AddWithValue("@receiveNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@receiveNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@receiveNo"))
                    { objCommProduct.Parameters.AddWithValue("@receiveNo", receiveNo); }
                    else { objCommProduct.Parameters["@receiveNo"].Value = receiveNo; }
                }

                if (string.IsNullOrEmpty(isSale))
                {
                    if (!objCommProduct.Parameters.Contains("@isSale"))
                    { objCommProduct.Parameters.AddWithValue("@isSale", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@isSale"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@isSale"))
                    { objCommProduct.Parameters.AddWithValue("@isSale", isSale); }
                    else { objCommProduct.Parameters["@isSale"].Value = isSale; }
                }

                if (string.IsNullOrEmpty(saleInvoiceNo))
                {
                    if (!objCommProduct.Parameters.Contains("@saleInvoiceNo"))
                    { objCommProduct.Parameters.AddWithValue("@saleInvoiceNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@saleInvoiceNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@saleInvoiceNo"))
                    { objCommProduct.Parameters.AddWithValue("@saleInvoiceNo", saleInvoiceNo); }
                    else { objCommProduct.Parameters["@saleInvoiceNo"].Value = saleInvoiceNo; }
                }

                if (finishItemNo == "" || string.IsNullOrEmpty(finishItemNo))
                {
                    if (!objCommProduct.Parameters.Contains("@finishItemNo"))
                    { objCommProduct.Parameters.AddWithValue("@finishItemNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@finishItemNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@finishItemNo"))
                    { objCommProduct.Parameters.AddWithValue("@finishItemNo", finishItemNo); }
                    else { objCommProduct.Parameters["@finishItemNo"].Value = finishItemNo; }
                }



                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
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

        public DataTable SearchReceiveTrackItems(string itemNo, string isTransaction, string transactionId, string type)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

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
                #region Parameter

                #endregion

                sqlText = "";
                sqlText += @"

--Declare @itemNo varchar(100) 
--Declare @isTransaction varchar(100) 
--Declare @transactionId varchar(100) 

--SET @itemNo='19'
--SET @isTransaction='N'
--SET @transactionId =''

SELECT pr.[ProductCode]
	  ,pr.[ProductName] 
      ,t.[ItemNo]
	  ,t.[TrackLineNo]
      ,t.[Heading1]
      ,t.[Heading2]
      ,t.[IsSale]
      ,t.[SaleInvoiceNo]
      ,t.[IsIssue]
      ,t.[IssueNo]
      ,t.[IsReceive]
      ,t.[ReceiveNo]
      ,t.[IsPurchase]
      ,t.[FinishItemNo]

  FROM Trackings t,Products pr
  WHERE t.[ItemNo]=pr.[ItemNo] 
--And (t.IsPurchase='Y' OR t.Post='Y') 
And t.Post='Y' 
AND t.ItemNo=@itemNo

";

                if (type.ToLower()=="receive" )
                {
                    sqlText += " AND (ReturnPurchase = 'N' OR ReturnPurchase IS NULL)";
                    if (string.IsNullOrEmpty(transactionId))
                    {
                        sqlText += " AND (IsReceive LIKE '%' +  @isTransaction + '%' OR @isTransaction IS NULL)"; //NEW Receive
                    }
                    else
                    {
                        sqlText += " AND ((ReceiveNo = @transactionId )"; //Existing Receive
                        sqlText += " OR (IsReceive LIKE '%' +  @isTransaction + '%' OR @isTransaction IS NULL))";
                        
                    }
                }
                else if (type.ToLower() == "sale")
                {
                    sqlText += " AND t.ReceivePost='Y'";
                    sqlText += " AND (ReturnReceive = 'N' OR ReturnReceive IS NULL)";
                    if (string.IsNullOrEmpty(transactionId))
                    {
                        sqlText += @"   AND ((IsSale LIKE '%' +  @isTransaction + '%' OR @isTransaction IS NULL) or saleinvoiceno in(select PreviousSalesInvoiceNo  from salesInvoiceHeaders
where 1=1
and saleType='credit'))";  
                    }
                    else
                    {
                        sqlText += " AND ((SaleInvoiceNo = @transactionId )"; //Existing Receive
                        sqlText += " OR (IsSale LIKE '%' +  @isTransaction + '%' OR @isTransaction IS NULL))";
                    }
                }
                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                if (itemNo == "" || string.IsNullOrEmpty(itemNo))
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@itemNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", itemNo); }
                    else { objCommProduct.Parameters["@itemNo"].Value = itemNo; }
                }

                if (string.IsNullOrEmpty(isTransaction))
                {
                    if (!objCommProduct.Parameters.Contains("@isTransaction"))
                    { objCommProduct.Parameters.AddWithValue("@isTransaction", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@isTransaction"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@isTransaction"))
                    { objCommProduct.Parameters.AddWithValue("@isTransaction", isTransaction); }
                    else { objCommProduct.Parameters["@isTransaction"].Value = isTransaction; }
                }

                if (string.IsNullOrEmpty(transactionId))
                {
                    if (!objCommProduct.Parameters.Contains("@transactionId"))
                    { objCommProduct.Parameters.AddWithValue("@transactionId", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@transactionId"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@transactionId"))
                    { objCommProduct.Parameters.AddWithValue("@transactionId", transactionId); }
                    else { objCommProduct.Parameters["@transactionId"].Value = transactionId; }
                }

                

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
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



        public DataTable SearchExistingTrackingItems(string isReceive, string receiveNo, string isSale,string saleInvoiceNo)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

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
                #region Parameter
                //                Declare @itemNo varchar(100) 
                //Declare @FinishItemNo varchar(100) 
                //Declare @saleInvoiceNo varchar(100) 
                //Declare @isSale varchar(100) 
                //Declare @isReceive varchar(100) 
                //Declare @receiveNo varchar(100) 
                //Declare @issueNo varchar(100) 
                //Declare @isIssue varchar(100)

                //SET @itemNo='36'
                //SET @FinishItemNo =''
                //SET @saleInvoiceNo=''
                //SET @isSale =''
                //SET @isReceive ='N'
                //SET @receiveNo =''
                //SET @issueNo =''
                //SET @isIssue =''
                #endregion


                sqlText = "";
                sqlText += @"

SELECT pr.[ProductCode]
	  ,pr.[ProductName] 
      ,t.[ItemNo]
	  ,t.[TrackLineNo]
      ,t.[Heading1]
      ,t.[Heading2]
      ,t.[IsSale]
      ,t.[SaleInvoiceNo]
      ,t.[IsIssue]
      ,t.[IssueNo]
      ,t.[IsReceive]
      ,t.[ReceiveNo]
      ,t.[IsPurchase]
      ,t.[FinishItemNo]

  FROM Trackings t,Products pr

WHERE t.[ItemNo]=pr.[ItemNo] And t.Post='Y'
           AND ((IsReceive =@isReceive )  AND (ReceiveNo= @receiveNo))
		   OR ((IsSale =@isSale ) AND  (SaleInvoiceNo= @saleInvoiceNo))

		  ";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

               
                if (string.IsNullOrEmpty(isReceive))
                {
                    if (!objCommProduct.Parameters.Contains("@isReceive"))
                    { objCommProduct.Parameters.AddWithValue("@isReceive", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@isReceive"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@isReceive"))
                    { objCommProduct.Parameters.AddWithValue("@isReceive", isReceive); }
                    else { objCommProduct.Parameters["@isReceive"].Value = isReceive; }
                }

                if (string.IsNullOrEmpty(receiveNo))
                {
                    if (!objCommProduct.Parameters.Contains("@receiveNo"))
                    { objCommProduct.Parameters.AddWithValue("@receiveNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@receiveNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@receiveNo"))
                    { objCommProduct.Parameters.AddWithValue("@receiveNo", receiveNo); }
                    else { objCommProduct.Parameters["@receiveNo"].Value = receiveNo; }
                }

                if (string.IsNullOrEmpty(isSale))
                {
                    if (!objCommProduct.Parameters.Contains("@isSale"))
                    { objCommProduct.Parameters.AddWithValue("@isSale", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@isSale"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@isSale"))
                    { objCommProduct.Parameters.AddWithValue("@isSale", isSale); }
                    else { objCommProduct.Parameters["@isSale"].Value = isSale; }
                }

                if (string.IsNullOrEmpty(saleInvoiceNo))
                {
                    if (!objCommProduct.Parameters.Contains("@saleInvoiceNo"))
                    { objCommProduct.Parameters.AddWithValue("@saleInvoiceNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@saleInvoiceNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@saleInvoiceNo"))
                    { objCommProduct.Parameters.AddWithValue("@saleInvoiceNo", saleInvoiceNo); }
                    else { objCommProduct.Parameters["@saleInvoiceNo"].Value = saleInvoiceNo; }
                }

               
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
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

        public string TrackingUpdate(List<TrackingVM> Trackings,SqlTransaction transaction, SqlConnection currConn)
        {
            #region Initializ
            string retResult = "Fail";
                       
            SqlConnection vcurrConn = currConn;
            if (vcurrConn == null)
            {
                currConn = null;
                transaction = null;
            }
            int transResult = 0;
            string sqlText = "";
           
            #endregion Initializ
            #region Try
            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                    transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                }
                #endregion open connection and transaction
                #region Tracking

                if (Trackings.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate, MessageVM.PurchasemsgNoDataToUpdateImportDyties);
                }

                foreach (var tracking in Trackings.ToList())
                {

                    #region Find Heading1 Existence

                    sqlText = "";
                    sqlText += "Select COUNT(Heading1) from Trackings  ";
                    sqlText += " WHERE ItemNo=@trackingItemNo";
                    sqlText += " AND Heading1 =@trackingHeading1 ";

                    SqlCommand cmdFindHeading1 = new SqlCommand(sqlText, currConn);
                    cmdFindHeading1.Transaction = transaction;
                    cmdFindHeading1.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo);
                    cmdFindHeading1.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1);


                    decimal IDExist = (int)cmdFindHeading1.ExecuteScalar();
                    if (IDExist > 0)
                    {
                        //Update


                        if (tracking.transactionType == "Purchase_Return")
                        {
                            #region Update Return
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " ReturnType         =@trackingReturnType,";
                            sqlText += " ReturnPurchase     =@trackingReturnPurchase,";
                            sqlText += " ReturnPurchaseID   =@TrackingsReturnPurchaseID,";
                            sqlText += " ReturnPurDate      =@trackingReturnPurDate";
                            sqlText += " where ItemNo       =@trackingItemNo";
                            sqlText += " and Heading1       =@trackingHeading1";

                            SqlCommand cmdReturnUpdate = new SqlCommand(sqlText, currConn);
                            cmdReturnUpdate.Transaction = transaction;
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingReturnType", tracking.ReturnType ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingReturnPurchase", tracking.ReturnPurchase ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@TrackingsReturnPurchaseID", Trackings[0].ReturnPurchaseID ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingReturnPurDate", tracking.ReturnPurDate ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);

                            transResult = (int)cmdReturnUpdate.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion Update Return
                        }
                        else if (tracking.transactionType == "Receive_Return")
                        {
                            #region Update Return
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " ReturnType     =@trackingReturnType,";
                            sqlText += " ReturnReceive  =@trackingReturnReceive,";
                            sqlText += " ReturnReceiveID=@trackingReturnReceiveID";
                            sqlText += " where ItemNo   =@trackingItemNo";
                            sqlText += " and Heading1   =@trackingHeading1";

                            SqlCommand cmdReturnUpdate = new SqlCommand(sqlText, currConn);
                            cmdReturnUpdate.Transaction = transaction;
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingReturnType", tracking.ReturnType ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingReturnReceive", tracking.ReturnReceive ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingReturnReceiveID", tracking.ReturnReceiveID ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);

                            transResult = (int)cmdReturnUpdate.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion Update Return
                        }
                        else if (tracking.transactionType == "Sale_Return")
                        {
                            #region Update Return
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " ReturnType     =@trackingReturnType,";
                            sqlText += " ReturnSale     =@trackingReturnSale,";
                            sqlText += " ReturnSaleID   =@trackingReturnSaleID";
                            sqlText += " where ItemNo   =@trackingItemNo";
                            sqlText += " and Heading1   =@trackingHeading1";

                            SqlCommand cmdReturnUpdate = new SqlCommand(sqlText, currConn);
                            cmdReturnUpdate.Transaction = transaction;
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingReturnType", tracking.ReturnType ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingReturnSale", tracking.ReturnSale ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingReturnSaleID", tracking.ReturnSaleID ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);

                            transResult = (int)cmdReturnUpdate.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion Update Return
                        }
                        else if (tracking.transactionType == "Receive")
                        {
                            #region Update Return
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " IsReceive      =@trackingIsReceive,";
                            sqlText += " ReceiveNo      =@trackingReceiveNo,";
                            sqlText += " ReceiveDate    =@trackingReceiveDate,";
                            sqlText += " FinishItemNo   =@trackingFinishItemNo";
                            sqlText += " where ItemNo   =@trackingItemNo";
                            sqlText += " and Heading1   =@trackingHeading1";

                            SqlCommand cmdReturnUpdate  = new SqlCommand(sqlText, currConn);
                            cmdReturnUpdate.Transaction = transaction;
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingIsReceive", tracking.IsReceive ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingReceiveNo", tracking.ReceiveNo ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingReceiveDate", tracking.ReceiveDate);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingFinishItemNo", tracking.FinishItemNo ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);

                            transResult = (int)cmdReturnUpdate.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion Update Return
                        }
                        else if (tracking.transactionType == "Sale")
                        {
                            #region Update Return
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " IsSale         =@trackingIsSale,";
                            sqlText += " SaleInvoiceNo  =@trackingSaleInvoiceNo,";
                            sqlText += " FinishItemNo   =@trackingFinishItemNo";
                            sqlText += " where ItemNo   =@trackingItemNo";
                            sqlText += " and Heading1   =@trackingHeading1";

                            SqlCommand cmdReturnUpdate = new SqlCommand(sqlText, currConn);
                            cmdReturnUpdate.Transaction = transaction;
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingIsSale", tracking.IsSale ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingSaleInvoiceNo", tracking.SaleInvoiceNo ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingFinishItemNo", tracking.FinishItemNo ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo ?? Convert.DBNull);
                            cmdReturnUpdate.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);

                            transResult = (int)cmdReturnUpdate.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion Update Return
                        }
                        //else if (tracking.transactionType == "Sale_Return")
                        //{
                        //    #region Update Return
                        //    sqlText = "";
                        //    sqlText += " update Trackings set ";
                        //    sqlText += " ReturnType='" + tracking.ReturnType + "',";
                        //    sqlText += " ReturnSale= '" + tracking.ReturnSale + "',";
                        //    sqlText += " ReturnSaleID='" + tracking.ReturnSaleID + "'";

                        //    sqlText += " where ItemNo = '" + tracking.ItemNo + "'";
                        //    sqlText += " and Heading1 = '" + tracking.Heading1 + "'";

                        //    SqlCommand cmdReturnUpdate = new SqlCommand(sqlText, currConn);
                        //    cmdReturnUpdate.Transaction = transaction;
                        //    transResult = (int)cmdReturnUpdate.ExecuteNonQuery();

                        //    if (transResult <= 0)
                        //    {
                        //        throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate,
                        //                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                        //    }
                        //    #endregion Update Return
                        //}
                        else
                        {
                            #region Update
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " IsIssue        =@trackingIsIssue,";
                            sqlText += " IssueNo        =@trackingIssueNo,";
                            sqlText += " IsReceive      =@trackingIsReceive,";
                            sqlText += " ReceiveNo      =@trackingReceiveNo,";
                            sqlText += " ReceiveDate    =@trackingReceiveDate,";
                            sqlText += " IsSale         =@trackingIsSale,";
                            sqlText += " SaleInvoiceNo  =@trackingSaleInvoiceNo,";
                            sqlText += " FinishItemNo   =@trackingFinishItemNo";
                            sqlText += " where ItemNo   =@trackingItemNo";
                            sqlText += " and Heading1   =@trackingHeading1";


                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;
                            cmdInsDetail.Parameters.AddWithValue("@trackingIsIssue", tracking.IsIssue ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingIssueNo", tracking.IssueNo ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingIsReceive", tracking.IsReceive ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingReceiveNo", tracking.ReceiveNo ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingReceiveDate", tracking.ReceiveDate);
                            cmdInsDetail.Parameters.AddWithValue("@trackingIsSale", tracking.IsSale ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingSaleInvoiceNo", tracking.SaleInvoiceNo ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingFinishItemNo", tracking.FinishItemNo ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);

                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion
                        }

                       

                       
                    }


                    #endregion Find Heading1 Existence
                }

                #endregion Tracking
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

                retResult = "Success";
                
                #endregion SuccessResult
            }

            #endregion
            #region Catch and Finall

            catch (SqlException sqlex)
            {
                if (vcurrConn == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (vcurrConn == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            return retResult;
            #endregion Result
        }

        public string TrackingInsert(List<TrackingVM> Trackings, SqlTransaction transaction, SqlConnection currConn)
        {
            #region Initializ
            string retResult = "Fail";

            SqlConnection vcurrConn = currConn;
            if (vcurrConn == null)
            {
                currConn = null;
                transaction = null;
            }
            
            int transResult = 0;
            string sqlText = "";
            int IDExist = 0;

            #endregion Initializ
            #region Try
            try
            {
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                    transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                }
                #endregion open connection and transaction
                #region Tracking

                #region Tracking

                if (Trackings.Count() > 0)
                {
                    foreach (var tracking in Trackings.ToList())
                    {

                        #region Find Heading1 Existence

                        sqlText = "";
                        sqlText += "select COUNT(Heading1) from Trackings WHERE Heading1 =@trackingHeading1 ";
                        SqlCommand cmdFindHeading1 = new SqlCommand(sqlText, currConn);
                        cmdFindHeading1.Transaction = transaction;
                        cmdFindHeading1.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1);


                         object objIDExist = cmdFindHeading1.ExecuteScalar();
                        if (objIDExist != null)
                        {
                            IDExist = Convert.ToInt32(objIDExist);
                        }

                        if (IDExist > 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            "Requested Tracking Information ( " + tracking.Heading1 + " ) already added.");
                        }

                        #endregion Find Heading1 Existence

                        #region Find Heading2 Existence

                        sqlText = "";
                        sqlText += "select COUNT(Heading2) from Trackings WHERE Heading2 =@trackingHeading2";
                        SqlCommand cmdFindHeading2 = new SqlCommand(sqlText, currConn);
                        cmdFindHeading2.Transaction = transaction;
                        cmdFindHeading2.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2);

                        objIDExist = cmdFindHeading2.ExecuteScalar();
                        if (objIDExist != null)
                        {
                            IDExist = Convert.ToInt32(objIDExist);
                        }

                        if (IDExist > 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            "Requested Tracking Information ( " + tracking.Heading2 + " ) already added.");
                        }

                        #endregion Find Heading2 Existence


                        sqlText = "";
                        sqlText += " insert into Trackings";
                        sqlText += " (";

                        sqlText += " PurchaseInvoiceNo,";
                        sqlText += " ItemNo,";
                        sqlText += " TrackLineNo,";
                        sqlText += " Heading1,";
                        sqlText += " Heading2,";
                        sqlText += " Quantity,";
                        sqlText += " UnitPrice,";
                        
                        sqlText += " IsPurchase,";
                        sqlText += " IsIssue,";
                        sqlText += " IsReceive,";
                        sqlText += " IsSale,";
                        sqlText += " Post,";
                        sqlText += " ReceivePost,";
                        sqlText += " SalePost,";
                        sqlText += " IssuePost,";


                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn";

                        sqlText += " )";
                        sqlText += " values";
                        sqlText += " (";

                        sqlText += "'0',";
                        sqlText += "@TrackingsItemNo,";
                        sqlText += "@trackingTrackingLineNo,";
                        sqlText += "@trackingHeading1,";
                        sqlText += "@trackingHeading2,";
                        sqlText += "@trackingQuantity,";
                        sqlText += "@trackingUnitPrice,";
                        sqlText += "@trackingIsPurchase,";
                        sqlText += "@trackingIsIssue,";
                        sqlText += "@trackingIsReceive,";
                        sqlText += "@trackingIsSale,";
                        sqlText += "'Y',";
                        sqlText += "'N',";
                        sqlText += "'N',";
                        sqlText += "'N',";


                        sqlText += "'" + UserInfoVM.UserName + "',";
                        sqlText += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                        sqlText += "'" + UserInfoVM.UserName + "',";
                        sqlText += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";

                        sqlText += ")";


                        SqlCommand cmdInsertTrackings = new SqlCommand(sqlText, currConn);
                        cmdInsertTrackings.Transaction = transaction;
                        cmdInsertTrackings.Parameters.AddWithValue("@TrackingsItemNo", Trackings[0].ItemNo ?? Convert.DBNull);
                        cmdInsertTrackings.Parameters.AddWithValue("@trackingTrackingLineNo", tracking.TrackingLineNo ?? Convert.DBNull);
                        cmdInsertTrackings.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);
                        cmdInsertTrackings.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2 ?? Convert.DBNull);
                        cmdInsertTrackings.Parameters.AddWithValue("@trackingQuantity", tracking.Quantity);
                        cmdInsertTrackings.Parameters.AddWithValue("@trackingUnitPrice", tracking.UnitPrice);
                        cmdInsertTrackings.Parameters.AddWithValue("@trackingIsPurchase", tracking.IsPurchase);
                        cmdInsertTrackings.Parameters.AddWithValue("@trackingIsIssue", tracking.IsIssue ?? Convert.DBNull);
                        cmdInsertTrackings.Parameters.AddWithValue("@trackingIsReceive", tracking.IsReceive ?? Convert.DBNull);
                        cmdInsertTrackings.Parameters.AddWithValue("@trackingIsSale", tracking.IsSale ?? Convert.DBNull);

                        transResult = (int)cmdInsertTrackings.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.PurchasemsgSaveNotSuccessfully);
                        }

                    }
                }

                #endregion Tracking

                #endregion Tracking
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

                retResult = "Success";

                #endregion SuccessResult
            }

            #endregion
            #region Catch and Finall

            catch (SqlException sqlex)
            {
                if (vcurrConn == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (vcurrConn == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            return retResult;
            #endregion Result
        }

        public DataTable SearchTrackings(string itemNo) // use for product opening
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

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
                #region Parameter
                //                Declare @itemNo varchar(100) 
                //Declare @FinishItemNo varchar(100) 
                //Declare @saleInvoiceNo varchar(100) 
                //Declare @isSale varchar(100) 
                //Declare @isReceive varchar(100) 
                //Declare @receiveNo varchar(100) 
                //Declare @issueNo varchar(100) 
                //Declare @isIssue varchar(100)

                //SET @itemNo='36'
                //SET @FinishItemNo =''
                //SET @saleInvoiceNo=''
                //SET @isSale =''
                //SET @isReceive ='N'
                //SET @receiveNo =''
                //SET @issueNo =''
                //SET @isIssue =''
                #endregion


                sqlText = "";
                sqlText += @"
SELECT pr.[ProductCode]
	  ,pr.[ProductName] 
      ,t.[ItemNo]
	  ,t.[TrackLineNo]
      ,t.[Heading1]
      ,t.[Heading2]
      ,t.[IsSale]
      ,t.[SaleInvoiceNo]
      ,t.[IsIssue]
      ,t.[IssueNo]
      ,t.[IsReceive]
      ,t.[ReceiveNo]
      ,t.[IsPurchase]
      ,t.[FinishItemNo]
      ,t.[Quantity]
      ,ISNULL(t.[UnitPrice],0) UnitPrice

  FROM Trackings t,Products pr

WHERE t.[ItemNo]=pr.[ItemNo] And t.Post='Y' 
           AND (t.ItemNo LIKE '%' +  @itemNo + '%' OR @itemNo IS NULL) 
           AND (t.IsPurchase LIKE '%' +  @isPurchase + '%' OR @isPurchase IS NULL) 

";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                if (itemNo == "" || string.IsNullOrEmpty(itemNo))
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@itemNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", itemNo); }
                    else { objCommProduct.Parameters["@itemNo"].Value = itemNo; }
                }
                if (!objCommProduct.Parameters.Contains("@isPurchase"))
                { objCommProduct.Parameters.AddWithValue("@isPurchase", "N"); }
                else { objCommProduct.Parameters["@isPurchase"].Value = "N"; }
               

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
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

        public DataTable SearchTrackingForReturn(string transactionType, string itemNo,string transactionID)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

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
                sqlText += @"

SELECT t.[PurchaseInvoiceNo]
      ,pr.[ProductCode]
	  ,pr.[ProductName] 
      ,t.[ItemNo]
	  ,t.[Heading1]
      ,t.[Heading2]
      ,t.[Quantity]
      ,t.[IsPurchase]
      ,t.[Post]
	  ,t.ReturnPurchase
	  ,t.ReturnPurchaseID
	  ,t.ReturnReceive
	  ,t.ReturnReceiveID
	  ,t.ReturnSale
	  ,t.ReturnSaleID
	  ,t.ReturnType
,t.IsIssue
,t.IsReceive
,t.IsSale
,t.ReceiveNo
,t.IssueNo
,t.SaleInvoiceNo
      ,ISNULL(t.[UnitPrice],0) UnitPrice

FROM Trackings t,Products pr
where t.[ItemNo]=pr.[ItemNo]
and t.ItemNo=@itemNo

";

                if (transactionType == "Purchase")
                {
                    sqlText += "and t.PurchaseInvoiceNo = @transactionID ";
                    sqlText += "and t.ISReceive='N' and t.IsSale='N' ";
                }
                else if (transactionType == "Receive")
                {
                    sqlText += "and t.ReceiveNo = @transactionID ";
                    sqlText += "and t.IsSale='N' ";
                }
                else if (transactionType == "Sale")
                {
                    sqlText += "and t.SaleInvoiceNo = @transactionID ";
                    
                }

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;



                if (!objCommProduct.Parameters.Contains("@transactionID"))
                { objCommProduct.Parameters.AddWithValue("@transactionID", transactionID); }
                else { objCommProduct.Parameters["@transactionID"].Value = transactionID; }

                if (string.IsNullOrEmpty(itemNo))
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@itemNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", itemNo); }
                    else { objCommProduct.Parameters["@itemNo"].Value = itemNo; }
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
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

        public string TrackingDelete(List<string> Headings)
        {
            #region Initializ
            string retResult = "Fail";
            SqlConnection currConn = null;
            int transResult = 0;
            string sqlText = "";

            #endregion Initializ
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
                #region Statement
                if (Headings.Count > 0)
                {
                    for (int i = 0; i < Headings.Count; i++)
                    {
                        sqlText = "";
                        sqlText += " select count(Heading1) from Trackings Where Heading1 =@Headings ";
                        sqlText += " AND IsReceive ='N'";

                        SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                        cmdExist.Parameters.AddWithValue("@Headings", Headings[i].ToString());

                        int IDExist = (int)cmdExist.ExecuteScalar();

                        if (IDExist > 0)
                        {
                            sqlText = "";
                            sqlText += " Delete Trackings Where Heading1 =@Headings ";
                            sqlText += " AND IsReceive ='N'";

                            SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                            cmdDelete.Parameters.AddWithValue("@Headings", Headings[i].ToString());

                            transResult = (int)cmdDelete.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException("DeleteTracking", "Already used in production");
                            }
                            retResult = "Success";
                        }
                        
                    }
                }

                #endregion Statement
            }
            #endregion Try
            #region Catch and Finall

            catch (SqlException sqlex)
            {

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {

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
            #endregion Catch and Finall

            #region Result
            return retResult;
            #endregion Result


        }
       

    }
}
