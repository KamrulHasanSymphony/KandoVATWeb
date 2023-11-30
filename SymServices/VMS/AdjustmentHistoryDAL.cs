using System;
using System.Data;
using System.Data.SqlClient;
using SymViewModel.VMS;
using System.Collections.Generic;
using SymOrdinary;


namespace SymServices.VMS
{
   public class AdjustmentHistoryDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Method
        

        //=========================NewSQLMethod====================================
//        public string[] InsertAdjustmentHistory(string AdjHistoryID, string AdjId, string AdjDate, decimal AdjAmount, decimal AdjVATRate, decimal AdjVATAmount, decimal AdjSD,
//decimal AdjSDAmount, decimal AdjOtherAmount, string AdjType, string AdjDescription, string CreatedBy,
//            string CreatedOn, string LastModifiedBy, string LastModifiedOn
//            , decimal AdjInputAmount, decimal AdjInputPercent, string AdjReferance, string Post, string AdjHistoryNo)
        public string[] InsertAdjustmentHistory(AdjustmentHistoryVM vm)
        {

            #region Objects & Variables

            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            string newID = "";
            string PostStatus = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.AdjId))
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameInsert,
                                                    "Please enter Adjustment name.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToAdjName");

                #endregion open connection and transaction

                #region name existence checking

                sqlText = "select count(AdjHistoryID) from AdjustmentHistorys where  AdjId=@AdjId" + 
                    " and AdjDate=@AdjDate " +
                     " and AdjType=@AdjType ";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@AdjId", vm.AdjId);
                cmdNameExist.Parameters.AddWithValue("@AdjDate", vm.AdjDate);
                cmdNameExist.Parameters.AddWithValue("@AdjType", vm.AdjType);

                countId = (int)cmdNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameInsert,
                                                    "Same Adjustment name already exist in same date");
                }
                #endregion product type name existence checking

                #region new id generation
                sqlText = "select isnull(max(cast(AdjHistoryID as int)),0)+1 FROM  AdjustmentHistorys";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();
                if (nextId <= 0)
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameInsert, "Unable to create new Adjustment No");
                }
                #endregion product type new id generation
                CommonDAL commonDal = new CommonDAL();


                newID = commonDal.TransactionCode("Adjustment", "Both", "AdjustmentHistorys", "AdjHistoryNo",
                                              "AdjDate", vm.AdjDate, currConn, transaction);


               
                #region Insert new row to table
                sqlText = "";



                sqlText += "insert into AdjustmentHistorys";
                sqlText += "(";

sqlText += " AdjHistoryID,";
sqlText += " AdjId,";
sqlText += " AdjDate,";
sqlText += " AdjAmount,";
sqlText += " AdjVATRate,";
sqlText += " AdjVATAmount,";
sqlText += " AdjSD,";
sqlText += " AdjSDAmount,";
sqlText += " AdjOtherAmount,";
sqlText += " AdjType,";
sqlText += " AdjDescription,";
sqlText += " CreatedBy,";
sqlText += " CreatedOn,";
sqlText += " LastModifiedBy,";
sqlText += " LastModifiedOn,";
sqlText += " AdjInputAmount,";
sqlText += " AdjInputPercent,";
sqlText += " AdjReferance,";
sqlText += " Post,";
sqlText += " AdjHistoryNo";

                sqlText += ")";
                sqlText += " values(";
                sqlText += "@nextId,";
                sqlText += "@AdjId,";
                sqlText += "@AdjDate,";
                sqlText += "@AdjAmount,";
                sqlText += "@AdjVATRate,";
                sqlText += "@AdjVATAmount,";
                sqlText += "@AdjSD,";
                sqlText += "@AdjSDAmount,";
                sqlText += "@AdjOtherAmount,";
                sqlText += "@AdjType,";
                sqlText += "@AdjDescription,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn,";
                sqlText += "@AdjInputAmount,";
                sqlText += "@AdjInputPercent,";
                sqlText += "@AdjReferance,";
                sqlText += "@Post,";
                sqlText += "@newID";


                sqlText += ")";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@nextId",nextId);
                cmdInsert.Parameters.AddWithValue("@AdjId", vm.AdjId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@AdjDate", vm.AdjDate);
                cmdInsert.Parameters.AddWithValue("@AdjAmount", vm.AdjAmount);
                cmdInsert.Parameters.AddWithValue("@AdjVATRate", vm.AdjVATRate);
                cmdInsert.Parameters.AddWithValue("@AdjVATAmount", vm.AdjVATAmount);
                cmdInsert.Parameters.AddWithValue("@AdjSD", vm.AdjSD);
                cmdInsert.Parameters.AddWithValue("@AdjSDAmount", vm.AdjSDAmount);
                cmdInsert.Parameters.AddWithValue("@AdjOtherAmount", vm.AdjOtherAmount);
                cmdInsert.Parameters.AddWithValue("@AdjType", vm.AdjType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@AdjDescription ", vm.AdjDescription ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy ", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn ", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@AdjInputAmount ", vm.AdjInputAmount);
                cmdInsert.Parameters.AddWithValue("@AdjInputPercent", vm.AdjInputPercent);
                cmdInsert.Parameters.AddWithValue("@AdjReferance", vm.AdjReferance ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Post", "N");
                cmdInsert.Parameters.AddWithValue("@newID", newID);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from AdjustmentHistorys WHERE AdjHistoryNo='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
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
                        retResults[0] = "Success";
                        retResults[1] = "Requested Adjustment Information successfully Added";
                        retResults[2] = nextId.ToString();
                        retResults[3] = newID;
                        retResults[4] = PostStatus;
                        

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to add Adjustment";
                        retResults[2] = "";
                        retResults[3] = "";
                        retResults[4] = "";


                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to add Adjustment";
                    retResults[2] = "";
                    retResults[3] = "";
                    retResults[4] = "";

                }

                #endregion Commit


                #endregion Inser new product type
            }
            #endregion try

            #region catch

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

            #endregion catch

            #region Finally

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

//        public string[] UpdateAdjustmentHistory(string AdjHistoryID, string AdjId, string AdjDate, decimal AdjAmount, decimal AdjVATRate, decimal AdjVATAmount, decimal AdjSD,
//decimal AdjSDAmount, decimal AdjOtherAmount, string AdjType, string AdjDescription, string CreatedBy,
//            string CreatedOn, string LastModifiedBy, string LastModifiedOn
//            , decimal AdjInputAmount, decimal AdjInputPercent, string AdjReferance, string Post, string AdjHistoryNo)
        public string[] UpdateAdjustmentHistory(AdjustmentHistoryVM vm)
        {
            #region Objects & Variables

            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.AdjHistoryID;
            retResults[3] = vm.AdjHistoryNo;
            retResults[4] = vm.Post;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string PostStatus = "";

            #endregion

            #region try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.AdjHistoryID))
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                    "Invalid Adjustment id.");
                }
                if (string.IsNullOrEmpty(vm.AdjId))
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                    "Please enter Adjustment name.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.AdjmsgMethodNameUpdate);

                #endregion open connection and transaction

                #region product type existence checking

                sqlText = "select count(AdjHistoryID) from AdjustmentHistorys where  AdjHistoryID =@AdjHistoryID";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                cmdIdExist.Parameters.AddWithValue("@AdjHistoryID", vm.AdjHistoryID);

                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                    "Could not find requested Adjustment id.");
                }
                #endregion

                #region same name product type existence checking

                sqlText = "select count(AdjHistoryID) from AdjustmentHistorys where  AdjId=@AdjId ";
                          sqlText += " and AdjDate=@AdjDate ";
                sqlText += " and AdjType=@AdjType ";
                sqlText += " and not AdjHistoryID = @AdjHistoryID ";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@AdjId", vm.AdjId);
                cmdNameExist.Parameters.AddWithValue("@AdjDate", vm.AdjDate);
                cmdNameExist.Parameters.AddWithValue("@AdjType", vm.AdjType);
                cmdNameExist.Parameters.AddWithValue("@AdjHistoryID", vm.AdjHistoryID);

                countId = (int)cmdNameExist.ExecuteScalar();
                if (countId > 0)
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                    "Same Adjustment name already exist in same date");
                }
                #endregion



                #region update existing row to table

                #region sql statement

                sqlText = "";
                sqlText += "UPDATE AdjustmentHistorys SET";
                sqlText += " AdjId              =@AdjId,";
                sqlText += " AdjDate            =@AdjDate,";
                sqlText += " AdjAmount          =@AdjAmount,";
                sqlText += " AdjVATRate         =@AdjVATRate,";
                sqlText += " AdjVATAmount       =@AdjVATAmount,";
                sqlText += " AdjSD              =@AdjSD,";
                sqlText += " AdjSDAmount        =@AdjSDAmount,";
                sqlText += " AdjOtherAmount     =@AdjOtherAmount,";
                sqlText += " AdjType            =@AdjType,";
                sqlText += " AdjDescription     =@AdjDescription,";
                sqlText += " LastModifiedBy     =@LastModifiedBy,";
                sqlText += " LastModifiedOn     =@LastModifiedOn,";
                sqlText += " AdjInputAmount     =@AdjInputAmount,";
                sqlText += " AdjInputPercent    =@AdjInputPercent,";
                sqlText += " AdjReferance       =@AdjReferance";
                sqlText +=" WHERE AdjHistoryID = @AdjHistoryID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@AdjId", vm.AdjId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@AdjDate", vm.AdjDate);
                cmdUpdate.Parameters.AddWithValue("@AdjAmount", vm.AdjAmount);
                cmdUpdate.Parameters.AddWithValue("@AdjVATRate", vm.AdjVATRate);
                cmdUpdate.Parameters.AddWithValue("@AdjVATAmount", vm.AdjVATAmount);
                cmdUpdate.Parameters.AddWithValue("@AdjSD", vm.AdjSD);
                cmdUpdate.Parameters.AddWithValue("@AdjSDAmount", vm.AdjSDAmount);
                cmdUpdate.Parameters.AddWithValue("@AdjOtherAmount", vm.AdjOtherAmount);
                cmdUpdate.Parameters.AddWithValue("@AdjType", vm.AdjType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@AdjDescription", vm.AdjDescription ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@AdjInputAmount", vm.AdjInputAmount);
                cmdUpdate.Parameters.AddWithValue("@AdjInputPercent", vm.AdjInputPercent);
                cmdUpdate.Parameters.AddWithValue("@AdjReferance", vm.AdjReferance ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@AdjHistoryID", vm.AdjHistoryID ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion
                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from AdjustmentHistorys WHERE AdjHistoryNo=@AdjHistoryNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@AdjHistoryNo", vm.AdjHistoryNo);

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
                        retResults[0] = "Success";
                        retResults[1] = "Requested Adjustment Information successfully Updated";
                        retResults[4] = PostStatus;


                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Adjustment";
                        retResults[4] = PostStatus;


                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Update Adjustment";

                }

                #endregion Commit

                #endregion
            }
            #endregion try

            #region catch

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

            #endregion catch

            #region Finally

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


//        public string[] PostAdjustmentHistory(string AdjHistoryID, string AdjId, string AdjDate, decimal AdjAmount, decimal AdjVATRate, decimal AdjVATAmount, decimal AdjSD,
//decimal AdjSDAmount, decimal AdjOtherAmount, string AdjType, string AdjDescription, string CreatedBy,
//     string CreatedOn, string LastModifiedBy, string LastModifiedOn
//     , decimal AdjInputAmount, decimal AdjInputPercent, string AdjReferance, string Post, string AdjHistoryNo)
        public string[] PostAdjustmentHistory(AdjustmentHistoryVM vm)
        {
            #region Objects & Variables

            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.AdjHistoryID;
            retResults[3] = vm.AdjHistoryNo;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string PostStatus = "";

            #endregion

            #region try
            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.AdjHistoryID))
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                    "Invalid Adjustment id.");
                }
                if (string.IsNullOrEmpty(vm.AdjId))
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                    "Please enter Adjustment name.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.AdjmsgMethodNameUpdate);

                #endregion open connection and transaction

                #region product type existence checking

                sqlText = "select count(AdjHistoryID) from AdjustmentHistorys where  AdjHistoryID = @AdjHistoryID";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                cmdIdExist.Parameters.AddWithValue("AdjHistoryID", vm.AdjHistoryID);
                countId = (int)cmdIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException(MessageVM.AdjmsgMethodNameUpdate,
                                                    "Could not find requested Adjustment id.");
                }
                #endregion




                #region update existing row to table

                #region sql statement

                sqlText = "";
                sqlText += "UPDATE AdjustmentHistorys SET";

                sqlText += " LastModifiedBy     =@LastModifiedBy,";
                sqlText += " LastModifiedOn     =@LastModifiedOn,";
                sqlText += " Post               =@Post";
                sqlText += " WHERE AdjHistoryID =@AdjHistoryID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@Post", vm.Post ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@AdjHistoryID", vm.AdjHistoryID ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                #endregion
                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from AdjustmentHistorys WHERE AdjHistoryNo=@AdjHistoryNo ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@AdjHistoryNo", vm.AdjHistoryNo);


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
                        retResults[0] = "Success";
                        retResults[1] = "Requested Adjustment Information successfully Posted";
                        retResults[4] = PostStatus;


                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Posted Adjustment";

                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Posted Adjustment";

                }

                #endregion Commit

                #endregion
            }
            #endregion try

            #region catch

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

            #endregion catch

            #region Finally

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

        public DataTable SearchAdjustmentHistory(string AdjHistoryNo, string AdjReferance, string AdjType, string Post, string AdjFromDate, string AdjToDate)
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("AdjustmentHistory");
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
                sqlText = @"
SELECT ah.AdjHistoryID,
ah.AdjId,
an.AdjName,
convert (varchar,ah.AdjDate,120)AdjDate,
isnull(ah.AdjAmount,0)AdjAmount,
isnull(ah.AdjVATRate,0)AdjVATRate,
isnull(ah.AdjVATAmount,0)AdjVATAmount,
isnull(ah.AdjSD,0)AdjSD, 
isnull(ah.AdjSDAmount,0)AdjSDAmount,
isnull(ah.AdjOtherAmount,0)AdjOtherAmount,
isnull(ah.AdjType,'NA')AdjType,
isnull(NULLIF(ah.AdjDescription,''),'NA')AdjDescription,
isnull(ah.AdjHistoryNo,'NA')AdjHistoryNo,
isnull(ah.AdjInputAmount,0)AdjInputAmount,
isnull(ah.AdjInputPercent,0)AdjInputPercent,
isnull(NULLIF(ah.AdjReferance,''),'NA')AdjReferance,
ah.Post

FROM AdjustmentHistorys ah
LEFT OUTER JOIN AdjustmentName an
ON ah.AdjId=an.AdjId
WHERE 	(ah.AdjType LIKE '%' + @AdjType + '%' OR @AdjType IS NULL)	
and (ah.AdjHistoryNo LIKE '%' + @AdjHistoryNo + '%' OR @AdjHistoryNo IS NULL)	
and (ah.AdjReferance LIKE '%' + @AdjReferance + '%' OR @AdjReferance IS NULL)	
and (ah.Post LIKE '%' + @Post + '%' OR @Post IS NULL)	
AND (AdjDate >= @AdjFromDate OR @AdjFromDate IS NULL)
                                AND (AdjDate <dateadd(d,1, @AdjToDate) OR @AdjToDate IS NULL)
and ah.AdjType not in ('Cash Payable')

                           ";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;

                if (AdjFromDate == "")
                {
                    if (!objCommProductType.Parameters.Contains("@AdjFromDate"))
                    { objCommProductType.Parameters.AddWithValue("@AdjFromDate", System.DBNull.Value); }
                    else { objCommProductType.Parameters["@AdjFromDate"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProductType.Parameters.Contains("@AdjFromDate"))
                    { objCommProductType.Parameters.AddWithValue("@AdjFromDate", AdjFromDate); }
                    else { objCommProductType.Parameters["@AdjFromDate"].Value = AdjFromDate; } 
                }
                if (AdjToDate=="")
                {
                    if (!objCommProductType.Parameters.Contains("@AdjToDate"))
                    { objCommProductType.Parameters.AddWithValue("@AdjToDate", System.DBNull.Value); }
                    else { objCommProductType.Parameters["@AdjToDate"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProductType.Parameters.Contains("@AdjToDate"))
                    { objCommProductType.Parameters.AddWithValue("@AdjToDate", AdjToDate); }
                    else { objCommProductType.Parameters["@AdjToDate"].Value = AdjToDate; }     
                }
                if (!objCommProductType.Parameters.Contains("@AdjType"))
                { objCommProductType.Parameters.AddWithValue("@AdjType", AdjType); }
                else { objCommProductType.Parameters["@AdjType"].Value = AdjType; }

                if (!objCommProductType.Parameters.Contains("@AdjHistoryNo"))
                { objCommProductType.Parameters.AddWithValue("@AdjHistoryNo", AdjHistoryNo); }
                else { objCommProductType.Parameters["@AdjHistoryNo"].Value = AdjHistoryNo; }
                if (!objCommProductType.Parameters.Contains("@AdjReferance"))
                { objCommProductType.Parameters.AddWithValue("@AdjReferance", AdjReferance); }
                else { objCommProductType.Parameters["@AdjReferance"].Value = AdjReferance; }

                if (!objCommProductType.Parameters.Contains("@Post"))
                { objCommProductType.Parameters.AddWithValue("@Post", Post); }
                else { objCommProductType.Parameters["@Post"].Value = Post; }
                
               


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
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
        public DataTable SearchAdjustmentHistoryForDeposit(string AdjHistoryNo)
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("AdjustmentHistory");
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
                sqlText = @"
SELECT ah.AdjHistoryID,
ah.AdjId,
an.AdjName,
ah.AdjDate,
isnull(ah.AdjAmount,0)AdjAmount,
isnull(ah.AdjVATRate,0)AdjVATRate,
isnull(ah.AdjVATAmount,0)AdjVATAmount,
isnull(ah.AdjSD,0)AdjSD, 
isnull(ah.AdjSDAmount,0)AdjSDAmount,
isnull(ah.AdjOtherAmount,0)AdjOtherAmount,
isnull(ah.AdjType,'NA')AdjType,
isnull(NULLIF(ah.AdjDescription,''),'NA')AdjDescription,
isnull(ah.AdjHistoryNo,'NA')AdjHistoryNo,
isnull(ah.AdjInputAmount,0)AdjInputAmount,
isnull(ah.AdjInputPercent,0)AdjInputPercent,
isnull(NULLIF(ah.AdjReferance,''),'NA')AdjReferance,
ah.Post

FROM AdjustmentHistorys ah
LEFT OUTER JOIN AdjustmentName an
ON ah.AdjId=an.AdjId
WHERE 	(ah.AdjHistoryNo= '" + AdjHistoryNo +"')	";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;

               

                if (!objCommProductType.Parameters.Contains("@AdjHistoryNo"))
                { objCommProductType.Parameters.AddWithValue("@AdjHistoryNo", AdjHistoryNo); }
                else { objCommProductType.Parameters["@AdjHistoryNo"].Value = AdjHistoryNo; }
               




                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
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

        public List<AdjustmentHistoryVM> SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<AdjustmentHistoryVM> VMs = new List<AdjustmentHistoryVM>();
            AdjustmentHistoryVM vm;
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
  adjh.AdjHistoryID
,adjh.AdjHistoryNo
,adjh.AdjId
,adjh.AdjDate
,isnull(adjh.AdjInputAmount,0) AdjInputAmount
,isnull(adjh.AdjInputPercent,0) AdjInputPercent
,isnull(adjh.AdjAmount,0) AdjAmount
,isnull(adjh.AdjVATRate,0) AdjVATRate
,isnull(adjh.AdjVATAmount,0) AdjVATAmount
,isnull(adjh.AdjSD,0) AdjSD
,isnull(adjh.AdjSDAmount,0) AdjSDAmount
,isnull(adjh.AdjOtherAmount,0) AdjOtherAmount
,adjh.AdjType
,adjh.AdjDescription
,adjh.AdjReferance
,adjh.Post
,adjh.CreatedBy
,adjh.CreatedOn
,adjh.LastModifiedBy
,adjh.LastModifiedOn
,adjh.ReverseAdjHistoryNo
,adjn.AdjName

FROM AdjustmentHistorys adjh
left outer join AdjustmentName adjn on adjh.AdjId=adjn.AdjId
WHERE  1=1 and adjh.AdjType<>'Cash Payable' 
";


                if (Id != "0")
                {
                    sqlText += @" and adjh.AdjHistoryID=@AdjHistoryID";
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
                    objComm.Parameters.AddWithValue("@AdjHistoryID", Id.ToString());
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AdjustmentHistoryVM();
                    vm.AdjHistoryID = dr["AdjHistoryID"].ToString();
                    vm.AdjHistoryNo = dr["AdjHistoryNo"].ToString();
                    vm.AdjId = dr["AdjId"].ToString();
                    vm.AdjDate = dr["AdjDate"].ToString();
                    vm.AdjInputAmount = Convert.ToDecimal(dr["AdjInputAmount"].ToString());
                    vm.AdjInputPercent = Convert.ToDecimal(dr["AdjInputPercent"].ToString());
                    vm.AdjAmount = Convert.ToDecimal(dr["AdjAmount"].ToString());
                    vm.AdjVATRate = Convert.ToDecimal(dr["AdjVATRate"].ToString());
                    vm.AdjVATAmount = Convert.ToDecimal(dr["AdjVATAmount"].ToString());
                    vm.AdjSD = Convert.ToDecimal(dr["AdjSD"].ToString());
                    vm.AdjSDAmount = Convert.ToDecimal(dr["AdjSDAmount"].ToString());
                    vm.AdjOtherAmount = Convert.ToDecimal(dr["AdjOtherAmount"].ToString());
                    vm.AdjType = dr["AdjType"].ToString();
                    vm.AdjDescription = dr["AdjDescription"].ToString();
                    vm.AdjReferance = dr["AdjReferance"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.HeadName = dr["AdjName"].ToString();

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

        public List<AdjustmentHistoryVM> SelectAllCashPayable(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<AdjustmentHistoryVM> VMs = new List<AdjustmentHistoryVM>();
            AdjustmentHistoryVM vm;
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
  adjh.AdjHistoryID
,adjh.AdjHistoryNo
,adjh.AdjId
,adjh.AdjDate
,isnull(adjh.AdjInputAmount,0) AdjInputAmount
,isnull(adjh.AdjInputPercent,0) AdjInputPercent
,isnull(adjh.AdjAmount,0) AdjAmount
,isnull(adjh.AdjVATRate,0) AdjVATRate
,isnull(adjh.AdjVATAmount,0) AdjVATAmount
,isnull(adjh.AdjSD,0) AdjSD
,isnull(adjh.AdjSDAmount,0) AdjSDAmount
,isnull(adjh.AdjOtherAmount,0) AdjOtherAmount
,adjh.AdjType
,adjh.AdjDescription
,adjh.AdjReferance
,adjh.Post
,adjh.CreatedBy
,adjh.CreatedOn
,adjh.LastModifiedBy
,adjh.LastModifiedOn
,adjh.ReverseAdjHistoryNo
,adjn.AdjName

FROM AdjustmentHistorys adjh
left outer join AdjustmentName adjn on adjh.AdjId=adjn.AdjId
WHERE  1=1 and adjh.AdjType='Cash Payable' 
";


                if (Id != "0")
                {
                    sqlText += @" and adjh.AdjHistoryID=@AdjHistoryID";
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
                    objComm.Parameters.AddWithValue("@AdjHistoryID", Id.ToString());
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AdjustmentHistoryVM();
                    vm.AdjHistoryID = dr["AdjHistoryID"].ToString();
                    vm.AdjHistoryNo = dr["AdjHistoryNo"].ToString();
                    vm.AdjId = dr["AdjId"].ToString();
                    vm.AdjDate = dr["AdjDate"].ToString();
                    vm.AdjInputAmount = Convert.ToDecimal(dr["AdjInputAmount"].ToString());
                    vm.AdjInputPercent = Convert.ToDecimal(dr["AdjInputPercent"].ToString());
                    vm.AdjAmount = Convert.ToDecimal(dr["AdjAmount"].ToString());
                    vm.AdjVATRate = Convert.ToDecimal(dr["AdjVATRate"].ToString());
                    vm.AdjVATAmount = Convert.ToDecimal(dr["AdjVATAmount"].ToString());
                    vm.AdjSD = Convert.ToDecimal(dr["AdjSD"].ToString());
                    vm.AdjSDAmount = Convert.ToDecimal(dr["AdjSDAmount"].ToString());
                    vm.AdjOtherAmount = Convert.ToDecimal(dr["AdjOtherAmount"].ToString());
                    vm.AdjType = dr["AdjType"].ToString();
                    vm.AdjDescription = dr["AdjDescription"].ToString();
                    vm.AdjReferance = dr["AdjReferance"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.HeadName = dr["AdjName"].ToString();

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
        #endregion Method
    }
}
