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


namespace SymServices.VMS
{
    public class BanderolProductsDAL
    {

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();


        public string[] InsertToBanderolProducts(BanderolProductVM vm)
        {

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[1] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.BandProductId))
                {
                    throw new ArgumentNullException("InsertToBanderolProducts","Please enter Product name.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("InsertToBanderolProducts");

                #endregion open connection and transaction
                #region check in product table
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText = "select count(distinct ItemNo) from Products where ItemNo=@ItemNo";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("InsertToBanderolProducts", "Product information does not exist in Products list.");
                    }

                }
                #endregion

                #region Insert Product Information

                sqlText = "select count(distinct ItemNo) from BanderolProducts where  ItemNo=@ItemNo";
                SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                cmdNameExist.Transaction = transaction;
                cmdNameExist.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                int countName = (int)cmdNameExist.ExecuteScalar();
                if (countName > 0)
                {
                    throw new ArgumentNullException("InsertToBanderolProducts", "Requested Product is already exist");
                }

                sqlText = "select isnull(max(cast(BandProductId as int)),0)+1 FROM  BanderolProducts";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                int nextId = (int)cmdNextId.ExecuteScalar();

                if (nextId <= 0)
                {
                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Banderol ProductId";
                    throw new ArgumentNullException("InsertToBanderolProducts","Unable to create new Banderol Product information Id");
                }
                vm.BandProductId = nextId.ToString();

                sqlText = "";
                #region sqlText
                sqlText += @" 
INSERT INTO Currencies(
 BandProductId
,ItemNo
,BanderolId
,PackagingId
,BUsedQty
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,WastageQty
,OpeningQty
,OpeningDate

) VALUES (
 @BandProductId
,@ItemNo
,@BanderolId
,@PackagingId
,@BUsedQty
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@WastageQty
,@OpeningQty 
,@OpeningDate 
) 
";
                #endregion sqlText
                #region oldquery
                //sqlText += "insert into BanderolProducts";
                //sqlText += "(";
                //sqlText += "BandProductId,";
                //sqlText += "ItemNo,";
                //sqlText += "BanderolId,";
                //sqlText += "PackagingId,";
                //sqlText += "BUsedQty,";
                //sqlText += "ActiveStatus,";
                //sqlText += "WastageQty,";
                //sqlText += "OpeningQty,";
                //sqlText += "OpeningDate,";
                //sqlText += "CreatedBy,";
                //sqlText += "CreatedOn,";
                //sqlText += "LastModifiedBy,";
                //sqlText += "LastModifiedOn";
                //sqlText += ")";
                //sqlText += " values(";
                //sqlText += "'" + nextId + "',";
                //sqlText += "'" + ItemNo + "',";
                //sqlText += "'" + BanderolID + "',";
                //sqlText += "'" + PackagingId + "',";
                //sqlText += "'" + BUsedQty + "',";
                //sqlText += "'" + ActiveStatus + "',";
                //sqlText += "'" + WastageQty + "',";
                //sqlText += "'" + OpeningQty + "',";
                //sqlText += "'" + OpeningDate + "',";
                //sqlText += "'" + CreatedBy + "',";
                //sqlText += "'" + CreatedOn + "',";
                //sqlText += "'" + LastModifiedBy + "',";
                //sqlText += "'" + LastModifiedOn + "'";
                //sqlText += ")";
                #endregion oldquery

                #region sqlexecution
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@BandProductId", vm.BandProductId);
                cmdInsert.Parameters.AddWithValue("@ItemNo", vm.ItemNo??Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BanderolId", vm.BanderolId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@PackagingId", vm.PackagingId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BUsedQty", vm.BUsedQty);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus?"Y":"N");
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@WastageQty", vm.WastageQty);
                cmdInsert.Parameters.AddWithValue("@OpeningQty", vm.OpeningQty);
                cmdInsert.Parameters.AddWithValue("@OpeningDate", Ordinary.DateToDate(vm.OpeningDate));
                #endregion sqlexecution

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Insert Currency Information


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();

                    }

                }

                #endregion Commit

                retResults[0] = "Success";
                retResults[1] = "Requested Banderol Product Information successfully added";
                retResults[2] = "" + nextId;
            }
            #region Catch
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

            return retResults;

        }

        //public string[] UpdateBanderolProduct(string BandProductId, string ItemNo, string BanderolID, string PackagingId, decimal BUsedQty, string ActiveStatus,
        //    string LastModifiedBy, string LastModifiedOn, string WastageQty, decimal OpeningQty, string OpeningDate)
        public string[] UpdateBanderolProduct(BanderolProductVM vm)
        {

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[1] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.BandProductId))
                {
                    throw new ArgumentNullException("UpdateBanderolProduct","Please enter Product name.");
                }


                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("UpdateBanderolProduct");
              
                #endregion open connection and transaction

                if (!string.IsNullOrEmpty(vm.BandProductId))
                {
                    sqlText = "select count(BandProductId) from BanderolProducts where  BandProductId=@BandProductId";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@BandProductId", vm.BandProductId);
                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("UpdateBanderolProduct", "Could not find requested product information ");
                    }

                }

                #region check in product table
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText = "select count(distinct ItemNo) from Products where ItemNo=@ItemNo";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("InsertToBanderolProducts", "Product information does not exist in Products list.");
                    }

                }
                #endregion

                #region Update Banderol Product Information

                sqlText = "";
                sqlText += "UPDATE BanderolProducts SET ";
                sqlText += " ItemNo             =@ItemNo";
                sqlText += " ,BanderolId        =@BanderolId";
                sqlText += " ,PackagingId       =@PackagingId";
                sqlText += " ,BUsedQty          =@BUsedQty";
                sqlText += " ,ActiveStatus      =@ActiveStatus";
                sqlText += " ,LastModifiedBy    =@LastModifiedBy";
                sqlText += " ,LastModifiedOn    =@LastModifiedOn";
                sqlText += " ,WastageQty        =@WastageQty";
                sqlText += " ,OpeningQty        =@OpeningQty";
                sqlText += " ,OpeningDate       =@OpeningDate";
                sqlText += " where BandProductId=@BandProductId";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                cmdInsert.Parameters.AddWithValue("@BanderolId", vm.BanderolId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@PackagingId", vm.PackagingId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@BUsedQty", vm.BUsedQty);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus ? "Y" : "N");
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@WastageQty", vm.WastageQty);
                cmdInsert.Parameters.AddWithValue("@OpeningQty", vm.OpeningQty);
                cmdInsert.Parameters.AddWithValue("@OpeningDate", vm.OpeningDate);
                cmdInsert.Parameters.AddWithValue("@BandProductId", vm.BandProductId);

                //cmdInsert.Parameters.AddWithValue("@BandProductId", vm.BandProductId);
                //cmdInsert.Parameters.AddWithValue("@ItemNo", vm.ItemNo ?? Convert.DBNull);
                //cmdInsert.Parameters.AddWithValue("@BanderolId", vm.BanderolId ?? Convert.DBNull);
                //cmdInsert.Parameters.AddWithValue("@PackagingId", vm.PackagingId ?? Convert.DBNull);
                //cmdInsert.Parameters.AddWithValue("@BUsedQty", vm.BUsedQty);
                //cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus ? "Y" : "N");
                //cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                //cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                //cmdInsert.Parameters.AddWithValue("@WastageQty", vm.WastageQty);
                //cmdInsert.Parameters.AddWithValue("@OpeningQty", vm.OpeningQty);
                //cmdInsert.Parameters.AddWithValue("@OpeningDate", Ordinary.StringToDate(vm.OpeningDate));

                transResult = (int)cmdInsert.ExecuteNonQuery();

                #endregion Update Banderol Product Information


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Banderol Product Information successfully Updated";

                    }

                }

                #endregion Commit
            }
            #region Catch
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

            return retResults;
        }

        public string[] DeleteBanderolProduct(string BandProductId)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = BandProductId;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(BandProductId.ToString()))
                {
                    throw new ArgumentNullException("DeleteBanderolProduct",
                                "Could not find requested Banderol Product.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction

                sqlText = "select count(BandProductId) from BanderolProducts where BandProductId=@BandProductId";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Parameters.AddWithValue("@BandProductId", BandProductId);

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Banderol Product Information.";
                    return retResults;
                }

                sqlText = "delete BanderolProducts where BandProductId=@BandProductId";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                cmdDelete.Parameters.AddWithValue("@BandProductId", BandProductId);

                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Banderol Product Information successfully deleted";
                }


            }
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

            return retResults;
        }

        public DataTable SearchBanderolProducts(string ProductName, string ProductCode, string BanderolId, string BanderolName, string PackagingId, string PackagingNature, string ActiveStatus)
        {
            string[] retResults = new string[2];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("BanderolProducts");
            
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                sqlText =
           @" SELECT isnull(NULLIF(bp.BandProductId,''),'')BandProductId,
                            isnull(NULLIF(bp.ItemNo,''),0)ItemNo, 
                            isnull(NULLIF(p.ProductCode,''),'')ProductCode,
                            isnull(NULLIF(p.ProductName,''),'')ProductName,
                            isnull(NULLIF(bp.BanderolId,''),'')BanderolId,
                            isnull(NULLIF(b.BanderolName,''),'')BanderolName,
                            isnull(NULLIF(b.BanderolSize,''),'')BanderolSize,
                            isnull(NULLIF(b.UOM,''),'')BanderolUom,
                            isnull(NULLIF(bp.PackagingId,''),'')PackagingId,
                            isnull(NULLIF(pii.PackagingNature,''),'')PackagingName,
                            isnull(NULLIF(pii.PackagingCapacity,''),'')PackagingSize,
                            isnull(NULLIF(pii.UOM,''),'')PackagingUom,
                            isnull(NULLIF(bp.BUsedQty,0),0)BUsedQty,
                            isnull(NULLIF(bp.WastageQty,0),0)WastageQty,
                            isnull(NULLIF(bp.ActiveStatus,''),'')ActiveStatus,
                            isnull(NULLIF(bp.OpeningQty,0),0)OpeningQty,
                            convert (varchar,isnull (bp.OpeningDate,GETDATE()),120)OpeningDate
                            FROM BanderolProducts bp Left Outer Join Products p
							on bp.ItemNo=p.ItemNo Left outer Join Banderols b
							on bp.BanderolId=b.BanderolID Left outer join PackagingInformations pii
							on bp.PackagingId=pii.PackagingId 
                 
                            WHERE 
                                (p.ProductCode  LIKE '%' +  @ProductCode  + '%' OR @ProductCode IS NULL) 
                            AND (p.ProductName LIKE '%' + @ProductName + '%' OR @ProductName IS NULL)
                            AND (bp.BanderolId LIKE '%' + @BanderolId + '%' OR @BanderolId IS NULL)
                            AND (b.BanderolName LIKE '%' + @BanderolName + '%' OR @BanderolName IS NULL)
							AND (bp.PackagingId  LIKE '%' +  @PackagingId  + '%' OR @PackagingId IS NULL) 
                            AND (pii.PackagingNature LIKE '%' + @PackagingNature + '%' OR @PackagingNature IS NULL)
                            AND (bp.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)       
                            order by
                                bp.BandProductId";
                #region sqlText
//                sqlText = @"
//select 
//bp.BandProductId,
//bp.ItemNo,
//bp.BanderolId,
//bp.PackagingId,
//ISNULL(bp.BUsedQty,0) BUsedQty,  
//ISNULL(bp.BUsedQty,0) WastageQty,  
//ISNULL(bp.BUsedQty,0) OpeningQty,  
//bp.OpeningDate,
//p.ProductName, 
//p.ProductCode, 
//b.BanderolName,
//b.BanderolSize,b.UOM,
//pc.PackagingId, pc.PackagingNature from BanderolProducts bp 
//left outer join Products p on bp.BandProductId=p.Id
//left outer join Banderols b on bp.BanderolId=b.BanderolID
//left outer join PackagingInformations pc on bp.PackagingId=pc.PackagingID
//where 1=1;
//";
//                if (Id > 0)
//                {
//                    sqlText += @" and bp.BandProductId=@BandProductId";
//                }
                #endregion sqlText
                SqlCommand objCommBanderolProduct = new SqlCommand(sqlText, currConn);
                
                #region oldConditions
                objCommBanderolProduct.Connection = currConn;

                objCommBanderolProduct.CommandText = sqlText;
                objCommBanderolProduct.CommandType = CommandType.Text;


                if (!objCommBanderolProduct.Parameters.Contains("@ProductName"))
                { objCommBanderolProduct.Parameters.AddWithValue("@ProductName", ProductName); }
                else { objCommBanderolProduct.Parameters["@ProductName"].Value = ProductName; }
                if (!objCommBanderolProduct.Parameters.Contains("@ProductCode"))
                { objCommBanderolProduct.Parameters.AddWithValue("@ProductCode", ProductCode); }
                else { objCommBanderolProduct.Parameters["@ProductCode"].Value = ProductCode; }
                if (!objCommBanderolProduct.Parameters.Contains("@BanderolId"))
                { objCommBanderolProduct.Parameters.AddWithValue("@BanderolId", BanderolId); }
                else { objCommBanderolProduct.Parameters["@BanderolId"].Value = BanderolId; }

                if (BanderolName == "")
                {
                    if (!objCommBanderolProduct.Parameters.Contains("@BanderolName"))
                    { objCommBanderolProduct.Parameters.AddWithValue("@BanderolName", System.DBNull.Value); }
                    else { objCommBanderolProduct.Parameters["@BanderolName"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommBanderolProduct.Parameters.Contains("@BanderolName"))
                    { objCommBanderolProduct.Parameters.AddWithValue("@BanderolName", BanderolName); }
                    else { objCommBanderolProduct.Parameters["@BanderolName"].Value = BanderolName; }
                }
                if (!objCommBanderolProduct.Parameters.Contains("@PackagingId"))
                { objCommBanderolProduct.Parameters.AddWithValue("@PackagingId", PackagingId); }
                else { objCommBanderolProduct.Parameters["@PackagingId"].Value = PackagingId; }

                if (PackagingNature == "")
                {
                    if (!objCommBanderolProduct.Parameters.Contains("@PackagingNature"))
                    { objCommBanderolProduct.Parameters.AddWithValue("@PackagingNature", System.DBNull.Value); }
                    else { objCommBanderolProduct.Parameters["@PackagingNature"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommBanderolProduct.Parameters.Contains("@PackagingNature"))
                    { objCommBanderolProduct.Parameters.AddWithValue("@PackagingNature", PackagingNature); }
                    else { objCommBanderolProduct.Parameters["@PackagingNature"].Value = PackagingNature; }
                }



                if (!objCommBanderolProduct.Parameters.Contains("@ActiveStatus"))
                { objCommBanderolProduct.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommBanderolProduct.Parameters["@ActiveStatus"].Value = ActiveStatus; }
                // Common Filed
                #endregion oldconditons
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommBanderolProduct);

                dataAdapter.Fill(dataTable);
            }
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

            return dataTable;

        }

        public DataTable SearchBanderol(string BandProductId)
        {
            string[] retResults = new string[2];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Banderol");
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                sqlText = "";
                sqlText += " Select b.BanderolId,b.BanderolName,b.BanderolSize,b.UOM,bp.BandProductId ";
                sqlText += " from BanderolProducts bp left outer join Banderols b on bp.BanderolID=b.BanderolID ";
                sqlText += " where bp.BandProductId =@BandProductId";

                SqlCommand cmdBande = new SqlCommand(sqlText, currConn);
                cmdBande.Parameters.AddWithValue("@BandProductId", BandProductId);
                SqlDataAdapter adptBande = new SqlDataAdapter(cmdBande);
                adptBande.Fill(dataTable);


            }
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
            return dataTable;
        }
    }
}
