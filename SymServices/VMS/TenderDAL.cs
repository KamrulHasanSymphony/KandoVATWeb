using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using SymViewModel.VMS;
using SymOrdinary;

namespace SymServices.VMS
{
    public class TenderDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        #endregion

        #region New Methods

        //==================Search Tender=================


        #endregion

        #region Old Methods

        public string[] TenderInsert(TenderMasterVM Master, List<TenderDetailVM> Details, SqlTransaction transaction, SqlConnection currConn)
        {

            #region Initializ
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            int Present = 0;
            int newID = 0;
            string PostStatus = "";

            int IDExist = 0;

            SqlConnection vcurrConn = currConn;
            if (vcurrConn == null)
            {
                currConn = null;
                transaction = null;
            }


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.PurchasemsgNoDataToSave);
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

                    transaction = currConn.BeginTransaction(MessageVM.tpMsgMethodNameInsert);
                }

                //currConn = _dbsqlConnection.GetConnection();
                //if (currConn.State != ConnectionState.Open)
                //{
                //    currConn.Open();
                //}

                //transaction = currConn.BeginTransaction(MessageVM.tpMsgMethodNameInsert);


                #endregion open connection and transaction

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(TenderId) from TenderHeaders WHERE TenderId=@MasterTenderId ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterTenderId", Master.TenderId);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Reference Number Check
                sqlText = "";
                sqlText += " select count(distinct RefNo) from TenderHeaders where  RefNo=@MasterRefNo ";
                sqlText += " and CustomerId=@MasterCustomerId ";
                SqlCommand cmdIDR = new SqlCommand(sqlText, currConn);
                cmdIDR.Transaction = transaction;
                cmdIDR.Parameters.AddWithValueAndNullHandle("@MasterRefNo", Master.RefNo);
                cmdIDR.Parameters.AddWithValueAndNullHandle("@MasterCustomerId", Master.CustomerId);

                Present = (int)cmdIDR.ExecuteScalar();

                if (Present > 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgFindRef);
                }
                #endregion Reference Number Check

                #region ID Create
                sqlText = "";
                sqlText = "select isnull(max(cast(TenderId as int)),0)+1 FROM  TenderHeaders";

                //sqlText = sqlText + "select max(isnull(cast(TenderId as int),0))+1 FROM  TenderHeaders";
                SqlCommand cmdIDc = new SqlCommand(sqlText, currConn);
                cmdIDc.Transaction = transaction;
                newID = (int)cmdIDc.ExecuteScalar();

                if (newID <= 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgIDNotCreate);
                }
                #endregion ID Create
                #region ID generated completed,Insert new Information in Header

                sqlText = "";
                sqlText += " insert into TenderHeaders";
                sqlText += " (";
                sqlText += " TenderId,";
                sqlText += " RefNo,";
                sqlText += " CustomerId,";
                sqlText += " TenderDate,";
                sqlText += " DeleveryDate,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " Comments,";
                sqlText += " CustomerGroupID";

                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@newID,";
                sqlText += "@MasterRefNo,";
                sqlText += "@MasterCustomerId,";
                sqlText += "@MasterTenderDate,";
                sqlText += "@MasterDeleveryDate,";
                sqlText += "@MasterCreatedBy,";
                sqlText += "@MasterCreatedOn,";
                sqlText += "@MasterLastModifiedBy,";
                sqlText += "@MasterLastModifiedOn,";
                sqlText += "@MasterComments,";
                sqlText += "@MasterCustomerGrpID";
                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@newID", newID);
                cmdInsert.Parameters.AddWithValue("@MasterRefNo", Master.RefNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCustomerId", Master.CustomerId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTenderDate", Master.TenderDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDeleveryDate", Master.DeleveryDate);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCustomerGrpID ", Master.CustomerGrpID ?? Convert.DBNull);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgSaveNotSuccessfully);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail



                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(TenderId) from TenderDetails WHERE TenderId=@newID and ItemNo =@ItemItemNo ";

                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@newID", newID);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into TenderDetails(";

                    sqlText += " TenderId,";
                    sqlText += " ItemNo,";
                    sqlText += " TenderQty,";
                    sqlText += " TenderPrice,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " BOMId,";
                    sqlText += " LastModifiedOn";

                    sqlText += " )";
                    sqlText += " values(	";
                    //sqlText += "'" + Master.Id + "',";
                    sqlText += "@newID,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemTenderQty,";
                    sqlText += "@ItemTenderPrice,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@ItemBOMId,";
                    sqlText += "@MasterLastModifiedOn";

                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValue("@newID", newID);
                    cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemTenderQty", Item.TenderQty);
                    cmdInsDetail.Parameters.AddWithValue("@ItemTenderPrice", Item.TenderPrice);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemBOMId", Item.BOMId ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgSaveNotSuccessfully);
                    }

                    #endregion Insert only DetailTable

                    #region Update product Tender Price

                    sqlText = "";
                    sqlText += "update Products set TenderPrice=@ItemTenderPrice  where itemNo=@ItemItemNo ";
                    SqlCommand cmdUpdTenderPrice = new SqlCommand(sqlText, currConn);
                    cmdUpdTenderPrice.Transaction = transaction;
                    cmdUpdTenderPrice.Parameters.AddWithValue("@ItemTenderPrice", Item.TenderPrice);
                    cmdUpdTenderPrice.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    transResult = (int)cmdUpdTenderPrice.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgSaveNotSuccessfully);
                    }

                    #endregion Update product Tender Price
                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

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
                retResults[1] = MessageVM.tpMsgSaveSuccessfully;
                retResults[2] = "" + newID;
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
        
        public string[] TenderUpdate(TenderMasterVM Master, List<TenderDetailVM> Details)
        {
            #region Initializ
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

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
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameUpdate, MessageVM.PurchasemsgNoDataToUpdate);
                }

                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.tpMsgMethodNameInsert);


                #endregion open connection and transaction
                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(TenderId) from TenderHeaders WHERE TenderId=@MasterTenderId ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterTenderId", Master.TenderId);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameUpdate, MessageVM.tpMsgUnableFindExistID);
                }

                #endregion Find ID for Update



                #region ID check completed,update Information in Header

                sqlText = "";

                sqlText += " update TenderHeaders set  ";

                sqlText += " RefNo              =@MasterRefNo ,";
                sqlText += " CustomerId         =@MasterCustomerId ,";
                sqlText += " TenderDate         =@MasterTenderDate ,";
                sqlText += " DeleveryDate       =@MasterDeleveryDate ,";
                sqlText += " LastModifiedBy     =@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn     =@MasterLastModifiedOn ,";
                sqlText += " CustomerGroupID    =@MasterCustomerGrpID ,";
                sqlText += " Comments           =@MasterComments ";
                sqlText += " where  TenderId    =@MasterTenderId ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@MasterRefNo", Master.RefNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterCustomerId", Master.CustomerId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterTenderDate", Master.TenderDate);
                cmdUpdate.Parameters.AddWithValue("@MasterDeleveryDate", Master.DeleveryDate);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterCustomerGrpID", Master.CustomerGrpID ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterTenderId", Master.TenderId ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameUpdate, MessageVM.PurchasemsgUpdateNotSuccessfully);
                }

                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameUpdate, MessageVM.PurchasemsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(TenderId) from TenderDetails WHERE TenderId=@MasterTenderId ";
                    sqlText += " AND ItemNo=@ItemItemNo ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@MasterTenderId", Master.TenderId);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        // Insert
                        #region ID generated completed,Insert new Information in Header

                        sqlText = "";
                        sqlText += " insert into TenderDetails(";

                        sqlText += " TenderId,";
                        sqlText += " ItemNo,";
                        sqlText += " TenderQty,";
                        sqlText += " TenderPrice,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn";

                        sqlText += " )";
                        sqlText += " values(	";
                        //sqlText += "'" + Master.Id + "',";
                        sqlText += "@MasterTenderId,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemTenderQty,";
                        sqlText += "@ItemTenderPrice,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn";

                        sqlText += ")	";


                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.Parameters.AddWithValue("@MasterTenderId", Master.TenderId);
                        cmdInsert.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@ItemTenderQty", Item.TenderQty);
                        cmdInsert.Parameters.AddWithValue("@ItemTenderPrice", Item.TenderPrice);
                        cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn);
                        cmdInsert.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);

                        transResult = (int)cmdInsert.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgUpdateNotSuccessfully);
                        }

                        #endregion ID generated completed,Insert new Information in Header

                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";

                        sqlText += " update TenderDetails set ";
                        sqlText += " TenderQty          = @ItemTenderQty,";
                        sqlText += " TenderPrice        = @ItemTenderPrice,";
                        sqlText += " LastModifiedBy     = @MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn     = @MasterLastModifiedOn";
                        sqlText += " where  TenderId    = @MasterTenderId";
                        sqlText += " and ItemNo         = @ItemItemNo";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@ItemTenderQty", Item.TenderQty);
                        cmdInsDetail.Parameters.AddWithValue("@ItemTenderPrice", Item.TenderPrice);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@MasterTenderId", Master.TenderId);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.tpMsgMethodNameUpdate, MessageVM.tpMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable

                    }

                    #endregion Find Transaction Mode Insert or Update
                    #region Update product Tender Price

                    sqlText = "";
                    sqlText += "update Products set TenderPrice=@ItemTenderPrice where itemNo=@ItemItemNo ";
                    SqlCommand cmdUpdTenderPrice = new SqlCommand(sqlText, currConn);
                    cmdUpdTenderPrice.Transaction = transaction;
                    cmdUpdTenderPrice.Parameters.AddWithValue("@ItemTenderPrice", Item.TenderPrice);
                    cmdUpdTenderPrice.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    transResult = (int)cmdUpdTenderPrice.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgUpdateNotSuccessfully);
                    }

                    #endregion Update product Tender Price
                }

                #region Remove row
                sqlText = "";
                sqlText += " SELECT  distinct ItemNo";
                sqlText += " from TenderDetails WHERE TenderId='" + Master.TenderId + "'";

                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIF = new SqlCommand(sqlText, currConn);
                cmdRIF.Transaction = transaction;
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
                        sqlText += " delete FROM TenderDetails ";
                        sqlText += " WHERE TenderId=@MasterTenderId ";
                        sqlText += " AND ItemNo=@p";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterTenderId", Master.TenderId);
                        cmdInsDetail.Parameters.AddWithValue("@p", p);


                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    }

                }

                #endregion Remove row

                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


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
                retResults[1] = MessageVM.tpMsgUpdateSuccessfully;
                retResults[2] = Master.TenderId;
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

        public DataTable SearchTenderHeader(string TenderId, string RefNo, string CustomerName)
        {
            #region Objects & Variables

            //string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Search Tender Header");
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
//                sqlText = @"SELECT     T.TenderId, T.RefNo, T.CustomerId, C.CustomerName, C.Address1, 
//C.Address2, C.Address3, convert (varchar,T.TenderDate,120)TenderDate, convert (varchar,T.DeleveryDate,120)DeleveryDate, T.Comments,cg.CustomerGroupName
//FROM         dbo.TenderHeaders AS T LEFT OUTER JOIN
//dbo.Customers AS C ON T.CustomerId = C.CustomerID LEFT OUTER JOIN
//customergroups AS cg ON c.CustomerGroupID=cg.CustomerGroupID
//
//WHERE 
//(t.TenderId  LIKE '%' +  @TenderId   + '%' OR @TenderId IS NULL) 
//and (RefNo  LIKE '%' +  @RefNo   + '%' OR @RefNo IS NULL) 
//and (C.CustomerName  LIKE '%' +  @CustomerName   + '%' OR @CustomerName IS NULL)";

                sqlText = @"
SELECT     T.TenderId, T.RefNo,T.CustomerId, C.CustomerName, T.CustomerGroupID,   
convert (varchar,T.TenderDate,120)TenderDate, convert (varchar,T.DeleveryDate,120)DeleveryDate, T.Comments,cg.CustomerGroupName
FROM         dbo.TenderHeaders AS T LEFT OUTER JOIN
dbo.Customers AS C ON T.CustomerId = C.CustomerID LEFT OUTER JOIN
customergroups AS cg ON c.CustomerGroupID=cg.CustomerGroupID

WHERE 
T.CustomerId <> '0' AND
(t.TenderId  LIKE '%' +  @TenderId   + '%' OR @TenderId IS NULL) 
and (RefNo  LIKE '%' +  @RefNo   + '%' OR @RefNo IS NULL) 
and (C.CustomerName  LIKE '%' +  @CustomerName   + '%' OR @CustomerName IS NULL)";


                SqlCommand objCommTenderHeader = new SqlCommand();
                objCommTenderHeader.Connection = currConn;
                objCommTenderHeader.CommandText = sqlText;
                objCommTenderHeader.CommandType = CommandType.Text;
                #region Parameter

                if (!objCommTenderHeader.Parameters.Contains("@TenderId"))
                { objCommTenderHeader.Parameters.AddWithValue("@TenderId", TenderId); }
                else { objCommTenderHeader.Parameters["@TenderId"].Value = TenderId; }
                if (RefNo == "")
                {
                    if (!objCommTenderHeader.Parameters.Contains("@RefNo"))
                    { objCommTenderHeader.Parameters.AddWithValue("@RefNo", System.DBNull.Value); }
                    else { objCommTenderHeader.Parameters["@RefNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommTenderHeader.Parameters.Contains("@RefNo"))
                    { objCommTenderHeader.Parameters.AddWithValue("@RefNo", RefNo); }
                    else { objCommTenderHeader.Parameters["@RefNo"].Value = RefNo; }
                }
                if (CustomerName == "")
                {
                    if (!objCommTenderHeader.Parameters.Contains("@CustomerName"))
                    { objCommTenderHeader.Parameters.AddWithValue("@CustomerName", System.DBNull.Value); }
                    else { objCommTenderHeader.Parameters["@CustomerName"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommTenderHeader.Parameters.Contains("@CustomerName"))
                    { objCommTenderHeader.Parameters.AddWithValue("@CustomerName", CustomerName); }
                    else { objCommTenderHeader.Parameters["@CustomerName"].Value = CustomerName; }
                }


                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTenderHeader);
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

        public DataTable SearchTenderHeaderByCustomerGrp(string TenderId, string RefNo, string CustomerGrpID)
        {
            #region Objects & Variables

            //string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Search Tender Header");
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
//                sqlText = @"SELECT     T.TenderId, T.RefNo, T.CustomerGroupID, ' ' CustomerName, ' ' Address1, 
//' ' Address2, ' ' Address3, convert (varchar,T.TenderDate,120)TenderDate, convert (varchar,T.DeleveryDate,120)DeleveryDate, T.Comments,cg.CustomerGroupName
//FROM         dbo.TenderHeaders AS T LEFT OUTER JOIN
//--dbo.Customers AS C ON T.CustomerId = C.CustomerID LEFT OUTER JOIN
//customergroups AS cg ON T.CustomerGroupID=cg.CustomerGroupID
//
//WHERE 
//(t.TenderId  LIKE '%' +  @TenderId   + '%' OR @TenderId IS NULL) 
//and (RefNo  LIKE '%' +  @RefNo   + '%' OR @RefNo IS NULL) 
//and (T.CustomerGroupID  LIKE '%' +  @CustomerGrpID   + '%' OR @CustomerGrpID IS NULL)
//";

                sqlText = @"
SELECT     T.TenderId, T.RefNo,' ' CustomerId, ' ' CustomerName, T.CustomerGroupID,   
convert (varchar,T.TenderDate,120)TenderDate, convert (varchar,T.DeleveryDate,120)DeleveryDate, T.Comments,cg.CustomerGroupName
FROM         dbo.TenderHeaders AS T LEFT OUTER JOIN
customergroups AS cg ON T.CustomerGroupID=cg.CustomerGroupID

WHERE 
T.CustomerGroupID <> '0' AND
(t.TenderId  LIKE '%' +  @TenderId   + '%' OR @TenderId IS NULL) 
and (RefNo  LIKE '%' +  @RefNo   + '%' OR @RefNo IS NULL) 
and (T.CustomerGroupID  LIKE '%' +  @CustomerGrpID   + '%' OR @CustomerGrpID IS NULL)


";
                SqlCommand objCommTenderHeader = new SqlCommand();
                objCommTenderHeader.Connection = currConn;
                objCommTenderHeader.CommandText = sqlText;
                objCommTenderHeader.CommandType = CommandType.Text;
                #region Parameter

                if (!objCommTenderHeader.Parameters.Contains("@TenderId"))
                { objCommTenderHeader.Parameters.AddWithValue("@TenderId", TenderId); }
                else { objCommTenderHeader.Parameters["@TenderId"].Value = TenderId; }

                if (RefNo == "")
                {
                    if (!objCommTenderHeader.Parameters.Contains("@RefNo"))
                    { objCommTenderHeader.Parameters.AddWithValue("@RefNo", System.DBNull.Value); }
                    else { objCommTenderHeader.Parameters["@RefNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommTenderHeader.Parameters.Contains("@RefNo"))
                    { objCommTenderHeader.Parameters.AddWithValue("@RefNo", RefNo); }
                    else { objCommTenderHeader.Parameters["@RefNo"].Value = RefNo; }
                }
                if (CustomerGrpID == "")
                {
                    if (!objCommTenderHeader.Parameters.Contains("@CustomerGrpID"))
                    { objCommTenderHeader.Parameters.AddWithValue("@CustomerGrpID", System.DBNull.Value); }
                    else { objCommTenderHeader.Parameters["@CustomerGrpID"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommTenderHeader.Parameters.Contains("@CustomerGrpID"))
                    { objCommTenderHeader.Parameters.AddWithValue("@CustomerGrpID", CustomerGrpID); }
                    else { objCommTenderHeader.Parameters["@CustomerGrpID"].Value = CustomerGrpID; }
                }


                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTenderHeader);
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

        public DataTable SearchTenderDetail(string TenderId, string transactionDate)//start
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Tender");
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


                sqlText = "";
                sqlText +=
                    " SELECT td.TenderId,td.ItemNo ItemNo,isnull(isnull(td.TenderQty,0)-isnull(td.SaleQty,0),0) Stock,";
                sqlText += " isnull(td.TenderPrice,0) NBRPrice, isnull(td.Post,'N')Post, p.ProductCode ProductCode,p.ProductName ProductName";
                sqlText += " ,p.UOM,isnull(td.BOMId,0)BOMId,p.CategoryID,isnull(td.TenderQty,0)TenderQty,";
                sqlText += " pc.CategoryName CategoryName,p.HSCodeNo HSCodeNo,pc.IsRaw IsRaw,p.VATRate,";
                sqlText += " p.SD,p.NonStock,p.Trading,isnull(p.TradingMarkUp,0)TradingMarkUp";
                sqlText += " FROM TenderDetails td LEFT OUTER JOIN  ";
                sqlText += "  TenderHeaders th ON td.TenderId=th.TenderId  LEFT OUTER JOIN ";
                sqlText += "  products p ON td.ItemNo=p.ItemNo ";
                sqlText += "  left outer join ProductCategories pc ON p.CategoryID=pc.CategoryID ";
                sqlText += " WHERE td.tenderid='" + TenderId + "'";
                if (!string.IsNullOrEmpty(transactionDate))
                {
                    sqlText += " and th.TenderDate <='" + transactionDate + "'";
                }
                //sqlText += " ";
                SqlCommand objCommTenderDetail = new SqlCommand();
                objCommTenderDetail.Connection = currConn;
                objCommTenderDetail.CommandText = sqlText;
                objCommTenderDetail.CommandType = CommandType.Text;
                #region Parameter
                //if (!objCommTenderDetail.Parameters.Contains("@TenderId"))
                //{ objCommTenderDetail.Parameters.AddWithValue("@TenderId", TenderId); }
                //else { objCommTenderDetail.Parameters["@TenderId"].Value = TenderId; }
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTenderDetail);
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

        public DataTable SearchTenderDetailSale(string TenderId, string transactiondate)
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Tender");
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

                sqlText = " ";
                sqlText += @"SELECT     TD.TenderId, P.ItemNo, ISNULL(P.ProductName, 'N/A') AS ProductName, ISNULL(P.ProductCode, 'N/A') AS ProductCode, P.CategoryID, ISNULL(PC.CategoryName, 'N/A')
AS CategoryName, ISNULL(P.UOM, 'N/A') AS UOM, ISNULL(P.HSCodeNo, 'N/A') AS HSCodeNo, ISNULL(PC.IsRaw, 'N/A') AS IsRaw, ISNULL(P.CostPrice, 0) 
AS CostPrice, ISNULL(P.OpeningBalance, 0) AS OpeningBalance, ISNULL(P.SalesPrice, 0) AS SalesPrice, isnull(td.TenderPrice,0) NBRPrice, 
ISNULL(P.ReceivePrice, 0) AS ReceivePrice, ISNULL(P.IssuePrice, 0) AS IssuePrice,
ISNULL(P.PacketPrice, 0) AS PacketPrice, ISNULL(td.TenderPrice, 0)AS TenderPrice, ISNULL(P.ExportPrice, 0) AS ExportPrice,
ISNULL(P.InternalIssuePrice, 0) AS InternalIssuePrice, ISNULL(P.TollIssuePrice, 0) AS TollIssuePrice, 
ISNULL(P.TollCharge, 0) AS TollCharge, ISNULL(P.VATRate, 0) AS VATRate, ISNULL(P.SD, 0) AS SD, ISNULL(P.TradingMarkUp, 0) AS TradingMarkUp, 
isnull(ISNULL(td.TenderQty,0)-ISNULL(td.SaleQty,0),0) AS TenderStock,
isnull(isnull(p.OpeningBalance,0)+isnull(p.QuantityInHand,0),0) as Stock,
ISNULL(P.QuantityInHand, 0) AS QuantityInHand, ISNULL(P.Trading, 'N') 
AS Trading, ISNULL(P.NonStock, 'N') AS NonStock,
ISNULL(td.BOMId, '0') AS BOMId,
ISNULL(td.TenderQty,0)TenderQty

FROM         dbo.TenderDetails AS TD  LEFT OUTER JOIN 
 TenderHeaders th ON td.TenderId=th.TenderId 
INNER JOIN
dbo.Products AS P ON TD.ItemNo = P.ItemNo INNER JOIN
dbo.ProductCategories AS PC ON P.CategoryID = PC.CategoryID
WHERE 
(td.TenderId = @TenderId or  @TenderId is null)";
                if (!string.IsNullOrEmpty(transactiondate))
                {
                    sqlText += " and th.TenderDate <='" + transactiondate + "'";
                }



                //sqlText += " ";
                SqlCommand objCommTenderDetail = new SqlCommand();
                objCommTenderDetail.Connection = currConn;
                objCommTenderDetail.CommandText = sqlText;
                objCommTenderDetail.CommandType = CommandType.Text;
                #region Parameter
                if (!objCommTenderDetail.Parameters.Contains("@TenderId"))
                { objCommTenderDetail.Parameters.AddWithValue("@TenderId", TenderId); }
                else { objCommTenderDetail.Parameters["@TenderId"].Value = TenderId; }
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTenderDetail);
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

        public DataTable SearchTenderDetailSaleNew(string TenderId, string transactiondate)
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Tender");
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

                sqlText = " ";
                sqlText += @"SELECT     '0' SalesInvoiceNo,P.ItemNo,ISNULL(td.TenderQty,0)Quantity,'0' PromotionalQuantity,ISNULL(td.TenderQty,0)SaleQuantity,'0' SalePrice,
     ISNULL(td.TenderPrice, 0)AS NBRPrice,ISNULL(P.UOM, 'N/A') AS UOM,ISNULL(P.VATRate, 0) AS VATRate,'0'VATAmount,
    '0'SubTotal,'N\A'Comments, ISNULL(P.ProductName, 'N/A') AS ProductName,isnull(isnull(p.OpeningBalance,0)+isnull(p.QuantityInHand,0),0) as Stock, 
    ISNULL(P.SD, 0) AS SD,'0'SDAmount,'New',
    ISNULL(P.QuantityInHand, 0) AS QuantityInHand, ISNULL(P.Trading, 'N') AS Trading, ISNULL(P.NonStock, 'N') AS NonStock,
    ISNULL(P.TradingMarkUp, 0) AS TradingMarkUp,'VAT' Type,ISNULL(P.ProductCode, 'N/A') AS ProductCode,
    ISNULL(td.TenderQty,0)UOMQty,ISNULL(P.UOM, 'N/A') AS UOMn,'1' UOMc,isnull(td.TenderPrice,0) UOMPrice,'0' DiscountAmount,
    isnull(td.TenderPrice,0) DiscountedNBRPrice,isnull(td.TenderPrice,0) CurrencyValue,'0' DollerValue, td.BOMId AS BOMID
    

    FROM         dbo.TenderDetails AS TD  LEFT OUTER JOIN 
    TenderHeaders th ON td.TenderId=th.TenderId 
    INNER JOIN
    dbo.Products AS P ON TD.ItemNo = P.ItemNo INNER JOIN
    dbo.ProductCategories AS PC ON P.CategoryID = PC.CategoryID
    WHERE 
    (td.TenderId = @TenderId or  @TenderId is null)";
                if (!string.IsNullOrEmpty(transactiondate))
                {
                    sqlText += " and th.TenderDate <='" + transactiondate + "'";
                }



                //sqlText += " ";
                SqlCommand objCommTenderDetail = new SqlCommand();
                objCommTenderDetail.Connection = currConn;
                objCommTenderDetail.CommandText = sqlText;
                objCommTenderDetail.CommandType = CommandType.Text;
                #region Parameter
                if (!objCommTenderDetail.Parameters.Contains("@TenderId"))
                { objCommTenderDetail.Parameters.AddWithValue("@TenderId", TenderId); }
                else { objCommTenderDetail.Parameters["@TenderId"].Value = TenderId; }
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTenderDetail);
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

        #endregion

        public string[] ImportData(DataTable dtTenderM, DataTable dtTenderD)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            TenderMasterVM tenderMasterVM;
            List<TenderDetailVM> tenderDetailVMs = new List<TenderDetailVM>();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable
            #region  Try
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    currConn.Open();
                }


                #region RowCount
                int MRowCount = 0;
                int MRow = dtTenderM.Rows.Count;
                for (int i = 0; i < dtTenderM.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtTenderM.Rows[i]["ID"].ToString()))
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
                    string importID = dtTenderM.Rows[i]["ID"].ToString();
                    if (!string.IsNullOrEmpty(importID))
                    {
                        DataRow[] DetailRaws = dtTenderD.Select("ID='" + importID + "'");
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
                    string id = dtTenderM.Rows[i]["ID"].ToString();
                    DataRow[] tt = dtTenderM.Select("ID='" + id + "'");
                    if (tt.Length > 1)
                    {
                        throw new ArgumentNullException("you have duplicate master id " + id + " in import file.");
                    }

                }

                #endregion Double ID in Master

                CommonDAL commonDal = new CommonDAL();
                string vCustGrp = commonDal.settings("ImportTender", "CustomerGroup");
                //if (string.IsNullOrEmpty(vCustGrp))
                //{
                //    return;
                //}

                bool ImportByCustGrp = Convert.ToBoolean(vCustGrp == "Y" ? true : false);



                CommonImport cImport = new CommonImport();

                #region checking from database is exist the information(NULL Check)

                #region Master

                for (int j = 0; j < MRowCount; j++)
                {
                    #region Checking Date is null or different formate

                    #region FindCustomerId

                    if (ImportByCustGrp == false)
                    {
                        cImport.FindCustomerId(dtTenderM.Rows[j]["Customer_Name"].ToString().Trim(),
                                          dtTenderM.Rows[j]["Customer_Code"].ToString().Trim(), currConn, transaction);
                    }
                    else
                    {
                        cImport.FindCustGroupID(dtTenderM.Rows[j]["Customer_Grp_Name"].ToString().Trim(),
                                                                  dtTenderM.Rows[j]["Customer_Grp_ID"].ToString().Trim(), currConn, transaction);
                    }
                   

                   

                    #endregion FindCustomerId


                    bool IsTenderDate;
                    IsTenderDate = cImport.CheckDate(dtTenderM.Rows[j]["Tender_Date"].ToString().Trim());
                    if (IsTenderDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Issue_Date field.");
                    }

                    bool IsDeleveryDate;
                    IsDeleveryDate = cImport.CheckDate(dtTenderM.Rows[j]["Delevery_Date"].ToString().Trim());
                    if (IsDeleveryDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Issue_Date field.");
                    }

                    #endregion Checking Date is null or different formate

                    #region Checking Y/N value
                    //bool post;
                    //post = cImport.CheckYN(dtTenderM.Rows[j]["Post"].ToString().Trim());
                    //if (post != true)
                    //{
                    //    throw new ArgumentNullException("Please insert Y/N in Post field.");
                    //}
                    #endregion Checking Y/N value

                    #region Check Ref id
                    string RefId = string.Empty;
                    RefId = dtTenderM.Rows[j]["Ref_No"].ToString().Trim();
                    if (string.IsNullOrEmpty(RefId))
                    {
                        throw new ArgumentNullException("Please insert value in RefId field.");
                    }
                    #endregion Check Ref id
                }

                #endregion Master

                #region Details

                #region Row count for details table
                int DRowCount = 0;
                for (int i = 0; i < dtTenderD.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtTenderD.Rows[i]["ID"].ToString()))
                    {
                        DRowCount++;
                    }

                }

                #endregion Row count for details table

                for (int i = 0; i < DRowCount; i++)
                {
                    string ItemNo = string.Empty;
                    //string UOMn = string.Empty;

                    #region FindItemId

                    ItemNo = cImport.FindItemId(dtTenderD.Rows[i]["Product_Name"].ToString().Trim()
                                                , dtTenderD.Rows[i]["Product_Code"].ToString().Trim(), currConn, transaction);

                    #endregion FindItemId

                    #region FindUOMn

                    //UOMn = cImport.FindUOMn(ItemNo, currConn, transaction);

                    //#endregion FindUOMn

                    //#region FindUOMn

                    //cImport.FindUOMc(UOMn, dtIssueD.Rows[i]["UOM"].ToString().Trim(), currConn, transaction);

                    #endregion FindUOMn

                    #region Numeric value check
                    //bool IsQuantity = cImport.CheckNumericBool(dtTenderD.Rows[i]["Quantity"].ToString().Trim());
                    //if (IsQuantity != true)
                    //{
                    //    throw new ArgumentNullException("Please insert decimal value in Quantity field.");
                    //}

                    bool IsTenderPrice = cImport.CheckNumericBool(dtTenderD.Rows[i]["Price"].ToString().Trim());
                    if (IsTenderPrice != true)
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

                for (int j = 0; j < MRowCount; j++)
                {


                    #region Master Issue

                    string importID = dtTenderM.Rows[j]["ID"].ToString().Trim();
                    #region FindCustomerId
                    string custGrpID = "0";
                    string customerId = "0";

                    if (ImportByCustGrp == false)
                    {
                        string customerName = dtTenderM.Rows[j]["Customer_Name"].ToString().Trim();
                        string customerCode = dtTenderM.Rows[j]["Customer_Code"].ToString().Trim();
                        customerId = cImport.FindCustomerId(customerName, customerCode, currConn, transaction);
                        
                    }
                    else
                    {
                        string custGrpName = dtTenderM.Rows[j]["Customer_Grp_Name"].ToString().Trim();
                        string custGrpCode = dtTenderM.Rows[j]["Customer_Grp_ID"].ToString().Trim();
                        custGrpID = cImport.FindCustGroupID(custGrpName, custGrpCode, currConn, transaction);
                    }

                    
                    #endregion FindCustomerId


                    DateTime tenderDate = Convert.ToDateTime(dtTenderM.Rows[j]["Tender_Date"].ToString().Trim());
                    DateTime DeleveryDate = Convert.ToDateTime(dtTenderM.Rows[j]["Delevery_Date"].ToString().Trim());
                    #region CheckNull
                    string comments = cImport.ChecKNullValue(dtTenderM.Rows[j]["Comments"].ToString().Trim());
                    #endregion CheckNull
                    string RefId = dtTenderM.Rows[j]["Ref_No"].ToString().Trim();
                    //string post = dtTenderM.Rows[j]["Post"].ToString().Trim();
                    string createdBy = dtTenderM.Rows[j]["Created_By"].ToString().Trim();
                    //string lastModifiedBy = dtTenderM.Rows[j]["LastModified_By"].ToString().Trim();

                    tenderMasterVM = new TenderMasterVM();

                    tenderMasterVM.TenderDate = tenderDate.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    tenderMasterVM.DeleveryDate = DeleveryDate.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    tenderMasterVM.RefNo = RefId;
                    tenderMasterVM.CustomerId = customerId;
                    tenderMasterVM.CustomerGrpID = custGrpID;
                    tenderMasterVM.Comments = comments;
                    tenderMasterVM.CreatedBy = createdBy;
                    tenderMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    tenderMasterVM.LastModifiedBy = createdBy;
                    tenderMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    tenderMasterVM.ImportID = importID;
                    DataRow[] DetailRaws; //= new DataRow[];//

                    #region MAtch

                    if (!string.IsNullOrEmpty(importID))
                    {
                        DetailRaws = dtTenderD.Select("ID='" + importID + "'");
                    }
                    else
                    {
                        DetailRaws = null;
                    }

                    #endregion MAtch

                    #endregion Master Issue

                    #region Details Issue

                    int counter = 1;
                    tenderDetailVMs = new List<TenderDetailVM>();


                    foreach (DataRow row in DetailRaws)
                    {

                        string itemCode = row["Product_Code"].ToString().Trim();
                        string itemName = row["Product_Name"].ToString().Trim();

                        string itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);

                        string quantity = row["Quantity"].ToString().Trim();
                        string price = row["Price"].ToString().Trim();
                        TenderDetailVM detail = new TenderDetailVM();
                        detail.ItemNo = itemNo;
                        detail.TenderQty = Convert.ToDecimal(quantity);
                        detail.TenderPrice = Convert.ToDecimal(price);

                        tenderDetailVMs.Add(detail);
                        counter++;
                    } // detail


                    #endregion Details Issue


                    string[] sqlResults = TenderInsert(tenderMasterVM, tenderDetailVMs, transaction, currConn);
                    retResults[0] = sqlResults[0];
                }

                if (retResults[0] == "Success")
                {
                    transaction.Commit();
                    #region SuccessResult

                    retResults[0] = "Success";
                    retResults[1] = MessageVM.tpMsgSaveSuccessfully;
                    retResults[2] = "" + "1";
                    retResults[3] = "" + "N";
                    #endregion SuccessResult
                }

            }

            #endregion  Try
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

        public List<TenderMasterVM> SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<TenderMasterVM> VMs = new List<TenderMasterVM>();
            TenderMasterVM vm;
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
 th.TenderId
,th.RefNo
,th.CustomerId
,th.TenderDate
,th.DeleveryDate
,th.Comments
,th.CreatedBy
,th.CreatedOn
,th.LastModifiedBy
,th.LastModifiedOn
,th.Post
,th.CustomerGroupID
,c.CustomerName
,cg.CustomerGroupName

FROM TenderHeaders th left outer join Customers c on th.CustomerId=c.CustomerId
left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID
WHERE  1=1 
";


                if (Id != "0")
                {
                    sqlText += @" and th.TenderId=@Id";
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

                if (Id != "0")
                {
                    objComm.Parameters.AddWithValue("@Id", Id.ToString());
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new TenderMasterVM();
                    vm.TenderId = dr["TenderId"].ToString();
                    vm.RefNo = dr["RefNo"].ToString();
                    vm.CustomerId = dr["CustomerId"].ToString();
                    vm.TenderDate = Ordinary.DateTimeToDate(dr["TenderDate"].ToString());
                    vm.DeleveryDate = Ordinary.DateTimeToDate(dr["DeleveryDate"].ToString());
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.CustomerGrpID = dr["CustomerGroupID"].ToString();
                    vm.GroupName = dr["CustomerGroupName"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();

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

        public List<TenderDetailVM> SelectAllDetails(string tenderId = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<TenderDetailVM> VMs = new List<TenderDetailVM>();
            TenderDetailVM vm;
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
 td.TenderId
,td.ItemNo
,isnull(td.TenderQty,0) TenderQty
,isnull(td.SaleQty,0) SaleQty
,isnull(td.TenderPrice,0) TenderPrice
,td.CreatedBy
,td.CreatedOn
,td.LastModifiedBy
,td.LastModifiedOn
,td.Post
,td.BOMId
,p.ProductName
,p.ProductCode
,p.UOM

FROM TenderDetails td left outer join Products p on td.ItemNo=p.ItemNo
WHERE  1=1 
";


                if (tenderId != "0")
                {
                    sqlText += @" and td.TenderId=@tenderId";
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

                if (tenderId != "0")
                {
                    objComm.Parameters.AddWithValue("@tenderId", tenderId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new TenderDetailVM();
                    vm.TenderIdD = dr["TenderId"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.TenderQty = Convert.ToDecimal(dr["TenderQty"].ToString());
                    //vm.SaleQty =  Convert.ToDecimal(dr["SaleQty"].ToString());
                    vm.TenderPrice = Convert.ToDecimal(dr["TenderPrice"].ToString());
                    vm.BOMId = dr["BOMId"].ToString();
                    vm.PCode = dr["ProductCode"].ToString();
                    vm.ItemName = dr["ProductName"].ToString();
                    vm.UOM = dr["UOM"].ToString();
                    vm.SubTotal = vm.TenderPrice * vm.TenderQty;

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
