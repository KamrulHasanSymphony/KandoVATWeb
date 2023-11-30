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
using System.IO;
using Excel;
namespace SymServices.VMS
{
    public class ImportDAL
    {
        
        private string[] sqlResults;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        #region New Methods
        public string[] ImportProduct(List<ProductVM> products,List<TrackingVM> trackings)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            TrackingDAL trackingDal = new TrackingDAL();
            string trackMsg = "";
            #endregion Initializ

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("ImportProduct");

                #endregion open connection and transaction
                #region Insert

                foreach (var Item in products.ToList())
                {

                    if (string.IsNullOrEmpty(Item.CategoryName))
                    {
                        throw new ArgumentNullException("ImportProduct",
                                                        "Group Name not exist");
                    }

                    sqlText = "select count(CategoryName) from ProductCategories where  CategoryName=@ItemCategoryName ";
                    SqlCommand cmdCatExist = new SqlCommand(sqlText, currConn);
                    cmdCatExist.Transaction = transaction;
                    cmdCatExist.Parameters.AddWithValue("@ItemCategoryName", Item.CategoryName);

                    countId = (int)cmdCatExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("ImportProduct",
                                                        "Group Name not exist in Database (" + Item.CategoryName + ").");
                    }

                    sqlText = "select distinct CategoryID from ProductCategories where  CategoryName=@ItemCategoryName";
                    SqlCommand cmdCId = new SqlCommand(sqlText, currConn);
                    cmdCId.Transaction = transaction;
                    cmdCId.Parameters.AddWithValue("@ItemCategoryName", Item.CategoryName);

                    string CID = cmdCId.ExecuteScalar().ToString();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("ImportProduct",
                                                        "Group Name not exist in Database (" + Item.CategoryName +
                                                        ").");
                    }

                    
                    #region Product Code Exist or not
                    if (!string.IsNullOrEmpty(Item.ProductCode))
                    {
                        sqlText = "select count(ProductCode) from Products where  ProductCode=@ItemProductCode";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@ItemProductCode", Item.ProductCode);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            //throw new ArgumentNullException("ImportProduct",
                            //                                "Product Code('" + Item.ProductCode + "') already exist.");
                            goto UpdatePro;
                        }
                    }
                    #endregion Product Code Exist

                    #region Product Name and Category Id not exist,Insert new Product
                    #region ProductID
                    sqlText = "select isnull(max(cast(ItemNo as int)),0)+1 FROM  Products WHERE SUBSTRING(ItemNo,1,3)<>'ovh'";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    int nextId = (int)cmdNextId.ExecuteScalar();
                    if (nextId <= 0)
                    {

                        throw new ArgumentNullException("ImportProduct",
                                                        "Unable to create new Product No");
                    }
                    #endregion ProductID
                                     
                    sqlText = "";
                    sqlText += "insert into Products";
                    sqlText += "(";
                    sqlText += "ItemNo,";
                    sqlText += "ProductCode,";
                    sqlText += "ProductName,";
                    sqlText += "ProductDescription,";
                    sqlText += "CategoryID,";
                    sqlText += "UOM,";
                    sqlText += "CostPrice,";
                    sqlText += "SalesPrice,";
                    sqlText += "NBRPrice,";
                    sqlText += "ReceivePrice,";
                    sqlText += "IssuePrice,";
                    sqlText += "TenderPrice,";
                    sqlText += "ExportPrice,";
                    sqlText += "InternalIssuePrice,";
                    sqlText += "TollIssuePrice,";
                    sqlText += "TollCharge,";
                    sqlText += "OpeningBalance,";
                    sqlText += "SerialNo,";
                    sqlText += "HSCodeNo,";
                    sqlText += "VATRate,";
                    sqlText += "Comments,";
                    sqlText += "ActiveStatus,";
                    sqlText += "CreatedBy,";
                    sqlText += "CreatedOn,";
                    sqlText += "LastModifiedBy,";
                    sqlText += "LastModifiedOn,";
                    sqlText += "SD,";
                    sqlText += "PacketPrice,";
                    sqlText += "Trading,";
                    sqlText += "TradingMarkUp,";
                    sqlText += "NonStock,";
                    sqlText += "Quantityinhand,";
                    sqlText += "OpeningTotalCost,";
                    sqlText += "OpeningDate";

                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += "@nextId,";
                    sqlText += "@ItemProductCode,";
                    sqlText += "@ItemProductName,";
                    sqlText += "@ItemProductDescription,";
                    sqlText += "@CID,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemCostPrice,";//CostPrice
                    sqlText += "@ItemCostPrice,";//SalePrice
                    sqlText += "@ItemCostPrice,";//NBRPrice
                    sqlText += "@ItemNBRPrice,";//ReceivePrice
                    sqlText += "@ItemNBRPrice,";//IssuePrice
                    sqlText += " " + 0 + ",";//TenderPrice
                    sqlText += " " + 0 + ",";//ExportPrice
                    sqlText += " " + 0 + ",";//InternalIssuePrice
                    sqlText += " " + 0 + ",";//TollIssueprice
                    sqlText += "@ItemTollCharge,";//TollCharge
                    sqlText += "@ItemOpeningBalance,";//OpeningBalance
                    sqlText += "@ItemSerialNo,";
                    sqlText += "@ItemHSCodeNo,";
                    sqlText += "@ItemVATRate,";
                    sqlText += "@ItemComments,";
                    sqlText += "@ItemActiveStatus,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@ItemLastModifiedBy,";
                    sqlText += "@ItemLastModifiedOn,";
                    sqlText += "@ItemSD,";
                    sqlText += "@ItemPacketprice,";
                    sqlText += "@ItemTrading,";
                    sqlText += "@ItemTradingMarkUp,";
                    sqlText += "@ItemNonStock,";
                    sqlText += " " + 0 + ",";//QuantityInHand
                    sqlText += "@ItemOpeningTotalCost,";

                    sqlText += "@ItemOpeningDate";//OpeningDate
                    sqlText += ")";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@nextId",nextId);
                    cmdInsert.Parameters.AddWithValue("@ItemProductCode", Item.ProductCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemProductName", Item.ProductName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemProductDescription", Item.ProductDescription ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CID", CID ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCostPrice",Item.CostPrice);
                    //cmdInsert.Parameters.AddWithValue("@ItemCostPrice",Item.CostPrice);
                    //cmdInsert.Parameters.AddWithValue("@ItemCostPrice",Item.CostPrice);
                    cmdInsert.Parameters.AddWithValue("@ItemNBRPrice",Item.NBRPrice);
                    //cmdInsert.Parameters.AddWithValue("@ItemNBRPrice",Item.NBRPrice);
                    cmdInsert.Parameters.AddWithValue("@ItemTollCharge",Item.TollCharge);
                    cmdInsert.Parameters.AddWithValue("@ItemOpeningBalance",Item.OpeningBalance);
                    cmdInsert.Parameters.AddWithValue("@ItemSerialNo", Item.SerialNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemHSCodeNo", Item.HSCodeNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemVATRate",Item.VATRate);
                    cmdInsert.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemActiveStatus", Item.ActiveStatus);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedOn",Item.CreatedOn);
                    cmdInsert.Parameters.AddWithValue("@ItemLastModifiedBy", Item.LastModifiedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemLastModifiedOn", Item.LastModifiedOn);
                    cmdInsert.Parameters.AddWithValue("@ItemSD",Item.SD);
                    cmdInsert.Parameters.AddWithValue("@ItemPacketprice",Item.Packetprice);
                    cmdInsert.Parameters.AddWithValue("@ItemTrading", Item.Trading ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemTradingMarkUp",Item.TradingMarkUp);
                    cmdInsert.Parameters.AddWithValue("@ItemNonStock", Item.NonStock ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemOpeningTotalCost",Item.OpeningTotalCost);
                    cmdInsert.Parameters.AddWithValue("@ItemOpeningDate", Ordinary.DateToDate(Item.OpeningDate));

                    transResult = (int)cmdInsert.ExecuteNonQuery();
                    if (transResult <= 0 || cmdInsert == null)
                    {

                        throw new ArgumentNullException("ImportProduct",
                                                        "Unable to Insert Product('" + Item.ProductName + "')");
                    }
                   
                    #endregion Product Name and Category Id not exist,Insert new Product

                    #region Update product
                UpdatePro:
                    nextId = Convert.ToInt32(Item.ItemNo);
                //    sqlText = "";
                //sqlText = "update Products set";
                //sqlText += " ProductName='" + Item.ProductName + "',";
                //sqlText += " ProductDescription='" + Item.ProductDescription + "',";
                //sqlText += " CategoryID='" + CID + "',";
                //sqlText += " UOM='" + Item.UOM + "',";
                //sqlText += " CostPrice=" + Item.CostPrice + ",";
                //sqlText += " OpeningBalance=" + Item.OpeningBalance + ",";
                //sqlText += " OpeningDate='" + Item.OpeningDate + "',";
                //sqlText += " SerialNo='" + Item.SerialNo + "',";
                //sqlText += " HSCodeNo='" + Item.HSCodeNo + "',";
                //sqlText += " VATRate=" + Item.VATRate + ",";
                //sqlText += " Comments='" + Item.Comments + "',";
                //sqlText += " ActiveStatus='" + Item.ActiveStatus + "',";
                //sqlText += " LastModifiedBy='" + Item.LastModifiedBy + "',";
                //sqlText += " LastModifiedOn='" + Item.LastModifiedOn + "',";
                //sqlText += " SD=" + Item.SD + ",";
                //sqlText += " PacketPrice=" + Item.Packetprice + ",";
                //sqlText += " NBRPrice=" + Item.NBRPrice + ",";
                //sqlText += " receiveprice=" +Item.NBRPrice + ",";
                //sqlText += " Trading='" + Item.Trading + "',";
                //sqlText += " TradingMarkUp=" + Item.TradingMarkUp + ",";
                //sqlText += " NonStock='" + Item.NonStock + "',";
                //sqlText += " TollCharge=" + Item.TollCharge + ",";
                //sqlText += " OpeningTotalCost=" + Item.OpeningTotalCost + "";
                //sqlText += " where ProductCode='" + Item.ProductCode + "'";

                //SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                //cmdUpdate.Transaction = transaction;
                //transResult = (int)cmdUpdate.ExecuteNonQuery();
                    #endregion  Update product

                    #region Trackings
                    if (trackings.Count>0)
                    {
                        var tracks = from x in trackings.ToList()
                                     where x.ProductName == Item.ProductName || x.ProductCode == Item.ProductCode
                                     select x;
                        if (tracks != null && tracks.Any())
                        {
                            List<TrackingVM> trackinfos = tracks.ToList();
                            foreach (var trackItem in trackinfos)
                            {
                                trackItem.ItemNo = nextId.ToString();
                            }
                            trackMsg = trackingDal.TrackingInsert(trackinfos, transaction, currConn);
                            if (trackMsg == "Fail")
                            {
                                throw new ArgumentNullException("ImportProduct", "Tracking Information not added for Item (" + Item.ProductCode + " ) .");
                            }
                        }
                        
                    }
                    #endregion Trackings
                }
                #endregion Insert


                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Products Information successfully Added";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add Products";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected erro to add Products ";
                }

                #endregion Commit
            }

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
        public string[] ImportProductOld(List<ProductVM> products)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
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
                transaction = currConn.BeginTransaction("ImportProduct");

                #endregion open connection and transaction
                #region Insert

                foreach (var Item in products.ToList())
                {

                    if (string.IsNullOrEmpty(Item.CategoryName))
                    {
                        throw new ArgumentNullException("ImportProduct",
                                                        "Group Name not exist");
                    }

                    sqlText = "select count(CategoryName) from ProductCategories where  CategoryName='@ItemCategoryName ";
                    SqlCommand cmdCatExist = new SqlCommand(sqlText, currConn);
                    cmdCatExist.Transaction = transaction;
                    cmdCatExist.Parameters.AddWithValue("@ItemCategoryName", Item.CategoryName);

                    countId = (int)cmdCatExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("ImportProduct",
                                                        "Group Name not exist in Database (" + Item.CategoryName + ").");
                    }

                    sqlText = "select distinct CategoryID from ProductCategories where  CategoryName=@ItemCategoryName ";
                    SqlCommand cmdCId = new SqlCommand(sqlText, currConn);
                    cmdCId.Transaction = transaction;
                    cmdCId.Parameters.AddWithValue("@ItemCategoryName", Item.CategoryName);

                    string CID = (string)cmdCId.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("ImportProduct",
                                                        "Group Name not exist in Database (" + Item.CategoryName +
                                                        ").");
                    }

                    #region Product Exist or not
                    /*Checking existance of provided bank Id information*/
                    //if (!string.IsNullOrEmpty(Item.ProductName))
                    //{


                    //    sqlText = "select count(ItemNo) from Products where  ProductName='" + Item.ProductName +
                    //              "' and CategoryID='" + CID + "'";
                    //    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    //    cmdIdExist.Transaction = transaction;
                    //    countId = (int)cmdIdExist.ExecuteScalar();
                    //    if (countId > 0)
                    //    {
                    //        throw new ArgumentNullException("ImportProduct",
                    //                                        "Same product('" + Item.ProductName + "' ) under same category('"+Item.CategoryName +"') already exist");
                    //    }
                    //}
                    #endregion ProductExist
                    #region Product Code Exist or not
                    if (!string.IsNullOrEmpty(Item.ProductCode))
                    {


                        sqlText = "select count(ProductCode) from Products where  ProductCode=@ItemProductCode";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@ItemProductCode", Item.ProductCode);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("ImportProduct",
                                                            "Product Code('" + Item.ProductCode + "') already exist.");
                        }
                    }
                    #endregion Product Code Exist

                    #region Product Name and Category Id not exist,Insert new Product
                    #region ProductID
                    sqlText = "select isnull(max(cast(ItemNo as int)),0)+1 FROM  Products WHERE SUBSTRING(ItemNo,1,3)<>'ovh'";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    int nextId = (int)cmdNextId.ExecuteScalar();
                    if (nextId <= 0)
                    {

                        throw new ArgumentNullException("ImportProduct",
                                                        "Unable to create new Product No");
                    }
                    #endregion ProductID

                    sqlText = "";
                    sqlText += "insert into Products";
                    sqlText += "(";
                    sqlText += "ItemNo,";
                    sqlText += "ProductCode,";
                    sqlText += "ProductName,";
                    sqlText += "ProductDescription,";
                    sqlText += "CategoryID,";
                    sqlText += "UOM,";
                    sqlText += "CostPrice,";
                    sqlText += "SalesPrice,";
                    sqlText += "NBRPrice,";
                    sqlText += "ReceivePrice,";
                    sqlText += "IssuePrice,";
                    sqlText += "TenderPrice,";
                    sqlText += "ExportPrice,";
                    sqlText += "InternalIssuePrice,";
                    sqlText += "TollIssuePrice,";
                    sqlText += "TollCharge,";
                    sqlText += "OpeningBalance,";
                    sqlText += "SerialNo,";
                    sqlText += "HSCodeNo,";
                    sqlText += "VATRate,";
                    sqlText += "Comments,";
                    sqlText += "ActiveStatus,";
                    sqlText += "CreatedBy,";
                    sqlText += "CreatedOn,";
                    sqlText += "LastModifiedBy,";
                    sqlText += "LastModifiedOn,";
                    sqlText += "SD,";
                    sqlText += "PacketPrice,";
                    sqlText += "Trading,";
                    sqlText += "TradingMarkUp,";
                    sqlText += "NonStock,";
                    sqlText += "Quantityinhand,";
                    sqlText += "OpeningTotalCost,";
                    sqlText += "OpeningDate";

                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += "@nextId,";
                    sqlText += "@ItemProductCode,";
                    sqlText += "@ItemProductName,";
                    sqlText += "@ItemProductDescription,";
                    sqlText += "@CID,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemCostPrice,";//CostPrice
                    sqlText += "@ItemCostPrice,";//SalePrice
                    sqlText += "@ItemCostPrice,";//NBRPrice
                    sqlText += "@ItemNBRPrice,";//ReceivePrice
                    sqlText += "@ItemNBRPrice,";//IssuePrice
                    sqlText += " " + 0 + ",";//TenderPrice
                    sqlText += " " + 0 + ",";//ExportPrice
                    sqlText += " " + 0 + ",";//InternalIssuePrice
                    sqlText += " " + 0 + ",";//TollIssueprice
                    sqlText += "@ItemTollCharge,";//TollCharge
                    sqlText += "@ItemOpeningBalance,";//OpeningBalance
                    sqlText += "@ItemSerialNo,";
                    sqlText += "@ItemHSCodeNo,";
                    sqlText += "@ItemVATRate,";
                    sqlText += "@ItemComments,";
                    sqlText += "@ItemActiveStatus,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@ItemSD,";
                    sqlText += "@ItemPacketprice,";
                    sqlText += "@ItemTrading,";
                    sqlText += "@ItemTradingMarkUp,";
                    sqlText += "@ItemNonStock,";
                    sqlText += " " + 0 + ",";//QuantityInHand
                    sqlText += "@ItemOpeningTotalCost,";

                    sqlText += "@Item.OpeningDate";//OpeningDate
                    sqlText += ")";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                    cmdInsert.Parameters.AddWithValue("@ItemProductCode", Item.ProductCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemProductName", Item.ProductName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemProductDescription", Item.ProductDescription ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CID", CID ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCostPrice", Item.CostPrice);
                    cmdInsert.Parameters.AddWithValue("@ItemNBRPrice", Item.NBRPrice);
                    cmdInsert.Parameters.AddWithValue("@ItemTollCharge", Item.TollCharge);
                    cmdInsert.Parameters.AddWithValue("@ItemOpeningBalance", Item.OpeningBalance);
                    cmdInsert.Parameters.AddWithValue("@ItemSerialNo", Item.SerialNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemHSCodeNo", Item.HSCodeNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                    cmdInsert.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemActiveStatus", Item.ActiveStatus);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedOn", Item.CreatedOn);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemSD", Item.SD);
                    cmdInsert.Parameters.AddWithValue("@ItemPacketprice", Item.Packetprice);
                    cmdInsert.Parameters.AddWithValue("@ItemTrading", Item.Trading ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemTradingMarkUp", Item.TradingMarkUp);
                    cmdInsert.Parameters.AddWithValue("@ItemNonStock", Item.NonStock ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemOpeningTotalCost", Item.OpeningTotalCost);
                    cmdInsert.Parameters.AddWithValue("@ItemOpeningDate", Ordinary.DateToDate(Item.OpeningDate));

                    transResult = (int)cmdInsert.ExecuteNonQuery();
                    if (transResult <= 0 || cmdInsert == null)
                    {

                        throw new ArgumentNullException("ImportProduct",
                                                        "Unable to Insert Product('" + Item.ProductName + "')");
                    }
                    #endregion Product Name and Category Id not exist,Insert new Product
                }
                #endregion Insert

                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Products Information successfully Added";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add Products";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected erro to add Products ";
                }

                #endregion Commit
            }

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
        public string[] ImportCustomer(List<CustomerVM> customers)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string customerCode;

            #endregion Initializ
            try
            {
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Customer") == "Y" ? true : false);
                #endregion settingsValue

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("ImportProduct");

                #endregion open connection and transaction

                foreach (var Item in customers.ToList())
                {
                    customerCode = Item.CustomerCode;
                    if (string.IsNullOrEmpty(Item.CustomerGroup))
                    {
                        throw new ArgumentNullException("ImportProduct",
                                                        "Group Name not exist");
                    }

                    sqlText = "select count(CustomerGroupName) from CustomerGroups where  CustomerGroupName=@ItemCustomerGroup ";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@ItemCustomerGroup", Item.CustomerGroup);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("ImportProduct",
                                                        "Group Name not exist in Database (" + Item.CustomerGroup + ").");
                    }

                    sqlText = "select distinct CustomerGroupID from CustomerGroups where  CustomerGroupName=@ItemCustomerGroup ";
                    SqlCommand cmdCId = new SqlCommand(sqlText, currConn);
                    cmdCId.Transaction = transaction;
                    cmdCId.Parameters.AddWithValue("@ItemCustomerGroup", Item.CustomerGroup);

                    string CID = cmdCId.ExecuteScalar().ToString();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("ImportProduct",
                                                        "Group Name not exist in Database (" + Item.CustomerGroup +").");
                    }
                    #region Customer  name existence checking

                    //select @Present = count(CustomerID) from Customers where CustomerID = @CustomerID;
                    //sqlText = "select count(CustomerID) from Customers where  CustomerName='" + Item.CustomerName + "'" +
                    //          " and CustomerGroupID='" + CID + "'";
                    //SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                    //cmdNameExist.Transaction = transaction;
                    //countId = (int)cmdNameExist.ExecuteScalar();
                    //if (countId > 0)
                    //{
                    //    throw new ArgumentNullException("InsertToCustomer", "Same customer  name('" + Item.CustomerName + "') already exist under same Group('"+Item.CustomerGroup +"')");
                    //}
                    
                    #endregion Customer Group name existence checking
                    #region Customer  new id generation
                    sqlText = "select isnull(max(cast(CustomerID as int)),0)+1 FROM  Customers";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    int nextId = (int)cmdNextId.ExecuteScalar();
                    if (nextId <= 0)
                    {
                        throw new ArgumentNullException("InsertToCustomer",
                                                        "Unable to create new Customer No");
                    }
                    #region Code
                    if (Auto == false)
                    {
                        if (string.IsNullOrEmpty(customerCode))
                        {
                            throw new ArgumentNullException("InsertToCustomer", "Code generation is Manual, but Code not Issued");
                        }
                        else
                        {
                            sqlText = "select count(CustomerID) from Customers where  CustomerCode=@customerCode ";
                            SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                            cmdCodeExist.Transaction = transaction;
                            cmdCodeExist.Parameters.AddWithValue("@customerCode", customerCode);

                            countId = (int)cmdCodeExist.ExecuteScalar();
                            if (countId > 0)
                            {
                           goto UpdateCust;

                                //throw new ArgumentNullException("InsertToCustomer", "Same customer  Code('" + customerCode + "') already exist");
                            }
                        }
                    }
                    else
                    {
                        customerCode = nextId.ToString();
                    }
                    #endregion Code

                    #endregion Customer  new id generation
                    #region Inser new customer
                    sqlText = "";
                    sqlText += "insert into Customers";
                    sqlText += "(";
                    sqlText += "CustomerID,";
                    sqlText += "CustomerName,";
                    sqlText += "CustomerGroupID,";
                    sqlText += "Address1,";
                    sqlText += "Address2,";
                    sqlText += "Address3,";
                    sqlText += "City,";
                    sqlText += "TelephoneNo,";
                    sqlText += "FaxNo,";
                    sqlText += "Email,";
                    sqlText += "StartDateTime,";
                    sqlText += "ContactPerson,";
                    sqlText += "ContactPersonDesignation,";
                    sqlText += "ContactPersonTelephone,";
                    sqlText += "ContactPersonEmail,";
                    sqlText += "TINNo,";
                    sqlText += "VATRegistrationNo,";
                    sqlText += "Comments,";
                    sqlText += "ActiveStatus,";
                    sqlText += "CreatedBy,";
                    sqlText += "CreatedOn,";
                    sqlText += "LastModifiedBy,";
                    sqlText += "LastModifiedOn,";
                    sqlText += "Country,CustomerCode";
                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += "@nextId,";
                    sqlText += "@ItemCustomerName,";
                    sqlText += "@CID,";
                    sqlText += "@ItemAddress1,";
                    sqlText += "@ItemAddress2,";
                    sqlText += "@ItemAddress3,";
                    sqlText += "@ItemCity,";
                    sqlText += "@ItemTelephoneNo,";
                    sqlText += "@ItemFaxNo,";
                    sqlText += "@ItemEmail,";
                    sqlText += "@ItemStartDateTime,";
                    sqlText += "@ItemContactPerson,";
                    sqlText += "@ItemContactPersonDesignation,";
                    sqlText += "@ItemContactPersonTelephone,";
                    sqlText += "@ItemContactPersonEmail,";
                    sqlText += "@ItemTINNo,";
                    sqlText += "@ItemVATRegistrationNo,";
                    sqlText += "@ItemComments,";
                    sqlText += "@ItemActiveStatus,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@ItemLastModifiedBy,";
                    sqlText += "@ItemLastModifiedOn,";
                    sqlText += "@ItemCountry,";
                    sqlText += "@customerCode";
                    sqlText += ")";


                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                    cmdInsert.Parameters.AddWithValue("@ItemCustomerName", Item.CustomerName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CID", CID ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemAddress1", Item.Address1 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemAddress2", Item.Address2 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemAddress3", Item.Address3 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCity", Item.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemTelephoneNo", Item.TelephoneNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemFaxNo", Item.FaxNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemEmail", Item.Email ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemStartDateTime", Ordinary.DateToDate(Item.StartDateTime));
                    cmdInsert.Parameters.AddWithValue("@ItemContactPerson", Item.ContactPerson ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemContactPersonDesignation", Item.ContactPersonDesignation ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemContactPersonTelephone", Item.ContactPersonTelephone ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemContactPersonEmail", Item.ContactPersonEmail ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemTINNo", Item.TINNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemVATRegistrationNo", Item.VATRegistrationNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemActiveStatus", Item.ActiveStatus);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedOn", Item.CreatedOn);
                    cmdInsert.Parameters.AddWithValue("@ItemLastModifiedBy", Item.LastModifiedBy);
                    cmdInsert.Parameters.AddWithValue("@ItemLastModifiedOn", Item.LastModifiedOn);
                    cmdInsert.Parameters.AddWithValue("@ItemCountry", Item.Country ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@customerCode", customerCode ?? Convert.DBNull);

                    transResult = (int)cmdInsert.ExecuteNonQuery();
                    if (transResult <= 0 || cmdInsert == null)
                    {

                        throw new ArgumentNullException("ImportProduct",
                                                        "Unable to Insert Product('" + Item.CustomerName + "')");
                    }
                   
                    #endregion Inser new customer
                UpdateCust:
                    string tt = "";

                }
                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested customers Information successfully Added";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add customer";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected erro to add customer ";
                }

                #endregion Commit
            }

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
        public string[] ImportVendor(List<VendorVM> vendors)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string vendorCode;


            #endregion Initializ
            try
            {
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Vendor") == "Y" ? true : false);
                #endregion settingsValue
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("ImportProduct");

                #endregion open connection and transaction

                foreach (var Item in vendors.ToList())
                {

                    vendorCode = Item.VendorCode;
                    sqlResults = new string[2];
                    if (string.IsNullOrEmpty(Item.VendorGroup))
                    {
                        throw new ArgumentNullException("ImportVendor",
                                                        "Group Name not exist");
                    }

                    sqlText = "select count(VendorGroupName) from VendorGroups where  VendorGroupName=@ItemVendorGroup ";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@ItemVendorGroup", Item.VendorGroup);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("ImportVendor",
                                                        "Group Name not exist in Database (" + Item.VendorGroupID + ").");
                    }

                    sqlText = "select distinct VendorGroupID from VendorGroups where  VendorGroupName=@ItemVendorGroup ";
                    SqlCommand cmdCId = new SqlCommand(sqlText, currConn);
                    cmdCId.Transaction = transaction;
                    cmdCId.Parameters.AddWithValue("@ItemVendorGroup", Item.VendorGroup);

                    string CID = cmdCId.ExecuteScalar().ToString();
                    if (countId <= 0)
                    {
                        throw new ArgumentNullException("ImportVendor",
                                                        "Group Name not exist in Database (" + Item.VendorGroupID +
                                                        ").");
                    }
                    #region Insert Vendor Information

                    //sqlText = "select count(distinct VendorName) from Vendors where  VendorName='" + Item.VendorName + "'" +
                    //          "and VendorGroupID ='" + CID + "'";
                    //SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                    //cmdNameExist.Transaction = transaction;
                    //int countName = (int)cmdNameExist.ExecuteScalar();
                    //if (countName > 0)
                    //{

                    //    throw new ArgumentNullException("InsertToVendorInformation",
                    //                                    "Requested Vendor Name('" + Item.VendorName + "') is already exist under same Group('"+Item.VendorGroup +"')");
                    //}

                    sqlText = "select isnull(max(cast(VendorID as int)),0)+1 FROM  Vendors";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    int nextId = (int)cmdNextId.ExecuteScalar();
                    if (nextId <= 0)
                    {
                        throw new ArgumentNullException("InsertToVendorInformation",
                                                        "Unable to create new Vendor information Id");
                    }
                    #region Code
                    if (Auto == false)
                    {
                        if (string.IsNullOrEmpty(vendorCode))
                        {
                            throw new ArgumentNullException("InsertToVendorInformation", "Code generation is Manual, but Code not Issued");
                        }
                        else
                        {
                            sqlText = "select count(VendorID) from Vendors where  VendorCode=@vendorCode ";
                            SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                            cmdCodeExist.Transaction = transaction;
                            cmdCId.Parameters.AddWithValue("@vendorCode", vendorCode);

                            countId = (int)cmdCodeExist.ExecuteScalar();
                            if (countId > 0)
                            {
                                throw new ArgumentNullException("InsertToVendorInformation", "Same customer  Code('" + vendorCode + "') already exist");
                            }
                        }
                    }
                    else
                    {
                        vendorCode = nextId.ToString();

                    }
                    #endregion Code

                    sqlText = "";
                    sqlText += "insert into Vendors";
                    sqlText += "(";
                    sqlText += "VendorID,";
                    sqlText += "VendorName,";
                    sqlText += "VendorGroupID,";
                    sqlText += "Address1,";
                    sqlText += "Address2,";
                    sqlText += "Address3,";
                    sqlText += "City,";
                    sqlText += "TelephoneNo,";
                    sqlText += "FaxNo,";
                    sqlText += "Email,";
                    sqlText += "StartDateTime,";
                    sqlText += "ContactPerson,";
                    sqlText += "ContactPersonDesignation,";
                    sqlText += "ContactPersonTelephone,";
                    sqlText += "ContactPersonEmail,";
                    sqlText += "VATRegistrationNo,";
                    sqlText += "TINNo,";
                    sqlText += "Comments,";
                    sqlText += "ActiveStatus,";
                    sqlText += "CreatedBy,";
                    sqlText += "CreatedOn,";
                    sqlText += "LastModifiedBy,";
                    sqlText += "LastModifiedOn,";
                    sqlText += "Country,";
                    sqlText += "VendorCode";
                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += "@nextId,";
                    sqlText += "@ItemVendorName,";
                    sqlText += "@CID,";
                    sqlText += "@ItemAddress1,";
                    sqlText += "@ItemAddress2,";
                    sqlText += "@ItemAddress3,";
                    sqlText += "@ItemCity,";
                    sqlText += "@ItemTelephoneNo,";
                    sqlText += "@ItemFaxNo,";
                    sqlText += "@ItemEmail,";
                    sqlText += "@ItemStartDateTime,";
                    sqlText += "@ItemContactPerson,";
                    sqlText += "@ItemContactPersonDesignation,";
                    sqlText += "@ItemContactPersonTelephone,";
                    sqlText += "@ItemContactPersonEmail,";
                    sqlText += "@ItemVATRegistrationNo,";
                    sqlText += "@ItemTINNo,";
                    sqlText += "@ItemComments,";
                    sqlText += "@ItemActiveStatus,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@ItemCountry,";
                    sqlText += "@vendorCode";

                    sqlText += ")";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                    cmdInsert.Parameters.AddWithValue("@ItemVendorName", Item.VendorName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CID", CID ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemAddress1", Item.Address1 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemAddress2", Item.Address2 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemAddress3", Item.Address3 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCity", Item.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemTelephoneNo", Item.TelephoneNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemFaxNo", Item.FaxNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemEmail", Item.Email ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemStartDateTime", Item.StartDateTime);
                    cmdInsert.Parameters.AddWithValue("@ItemContactPerson", Item.ContactPerson ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemContactPersonDesignation", Item.ContactPersonDesignation ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemContactPersonTelephone", Item.ContactPersonTelephone ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemContactPersonEmail", Item.ContactPersonEmail ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemVATRegistrationNo", Item.VATRegistrationNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemTINNo", Item.TINNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemActiveStatus", Item.ActiveStatus);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedOn", Item.CreatedOn);
                    cmdInsert.Parameters.AddWithValue("@ItemCountry", Item.Country ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@vendorCode", vendorCode ?? Convert.DBNull);

                    transResult = (int)cmdInsert.ExecuteNonQuery();
                    if (transResult <= 0 || cmdInsert == null)
                    {

                        throw new ArgumentNullException("ImportProduct",
                                                        "Unable to Insert Product('" + Item.VendorName + "')");
                    }

                    #endregion Insert

                }
                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested customer Information successfully Added";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add customer";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected erro to add customer ";
                }

                #endregion COMMIT

            }

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
        public string[] ImportVehicle(List<VehicleVM> vehicles)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string vehicleCode;


            #endregion Initializ
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("ImportVehicle");

                #endregion open connection and transaction

                foreach (var Item in vehicles.ToList())
                {
                    
                    sqlResults = new string[2];
                    if (string.IsNullOrEmpty(Item.VehicleNo))
                    {
                        throw new ArgumentNullException("ImportVehicle",
                                                        "Vehicle Name not exist");
                    }

                    sqlText = "select count(VehicleNo) from Vehicles where  VehicleNo=@ItemVehicleNo ";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@ItemVehicleNo", Item.VehicleNo);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("ImportVehicle",
                                                        "Vehicle No not exist in Database (" + Item.VehicleNo + ").");
                    }
                    #region Vehicle new id generation

                    //select @VehicleID= isnull(max(cast(VehicleID as int)),0)+1 FROM  Vehicles;
                    sqlText = "select isnull(max(cast(VehicleID as int)),0)+1 FROM  Vehicles";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    int nextId = (int)cmdNextId.ExecuteScalar();
                    if (nextId <= 0)
                    {

                        throw new ArgumentNullException("InsertToVehicle", "Unable to create new vehicle");
                    }

                    #endregion Vehicle new id generation

                    vehicleCode = nextId.ToString();

                    #region Insert new vehicle

                    sqlText = "";
                    sqlText += "insert into Vehicles";
                    sqlText += "(";
                    sqlText += "VehicleID,";
                    sqlText += "VehicleType,";
                    sqlText += "VehicleNo,";
                    sqlText += "Description,";
                    sqlText += "Comments,";
                    sqlText += "ActiveStatus,";
                    sqlText += "CreatedBy,";
                    sqlText += "CreatedOn,";
                    sqlText += "LastModifiedBy,";
                    sqlText += "LastModifiedOn,";
                    sqlText += "VehicleCode";
                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += "@nextId,";
                    sqlText += "@ItemVehicleType,";
                    sqlText += "@ItemVehicleNo,";
                    sqlText += "@ItemDescription,";
                    sqlText += "@ItemComments,";
                    sqlText += "@ItemActiveStatus,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@vehicleCode";
                    sqlText += ")";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                    cmdInsert.Parameters.AddWithValue("@ItemVehicleType", Item.VehicleType ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemVehicleNo", Item.VehicleNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemDescription", Item.Description ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemActiveStatus", Item.ActiveStatus);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedOn", Item.CreatedOn);
                    cmdInsert.Parameters.AddWithValue("@vehicleCode", vehicleCode ?? Convert.DBNull);

                    transResult = (int)cmdInsert.ExecuteNonQuery();
                    if (transResult <= 0 || cmdInsert == null)
                    {

                        throw new ArgumentNullException("ImportVehicle",
                                                        "Unable to Insert Vehicle('" + Item.VehicleNo + "')");
                    }
                    
                    
                }
#endregion
                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Vehicle Information successfully Added";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add Vehicle";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected erro to add Vehicle ";
                }

                #endregion COMMIT

            }

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
        public string[] ImportBank(List<BankVM> banks)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string bankCode;

            #endregion Initializ
            try
            {
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                bool Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Bank") == "Y" ? true : false);
                #endregion settingsValue
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("ImportProduct");

                #endregion open connection and transaction

                foreach (var Item in banks.ToList())
                {

                    bankCode = Item.BankCode;
                    sqlResults = new string[2];
                    if (string.IsNullOrEmpty(Item.BankName))
                    {
                        throw new ArgumentNullException("ImportBank",
                                                        "Bank not exist");
                    }
                    #region Insert Bank Information

                    //sqlText = "select count(distinct BankName) from BankInformations where  BankName='" +Item.BankName +
                    //          "' and " + "AccountNumber='" + Item.AccountNumber + "'";
                    //SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                    //cmdNameExist.Transaction = transaction;
                    //int countName = (int)cmdNameExist.ExecuteScalar();
                    //if (countName > 0)
                    //{

                    //    throw new ArgumentNullException("InsertToBankInformation",
                    //                                    "Requested Bank Name('" + Item.BankName + "') and Account number('" + Item.AccountNumber + "') is already exist");
                    //}

                    sqlText = "select isnull(max(cast(BankID as int)),0)+1 FROM  BankInformations";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    int nextId = (int)cmdNextId.ExecuteScalar();
                    if (nextId <= 0)
                    {

                        throw new ArgumentNullException("InsertToBankInformation",
                                                        "Unable to create new Bank information Id");
                    }
                    #region Code
                    if (Auto == false)
                    {
                        if (string.IsNullOrEmpty(bankCode))
                        {
                            throw new ArgumentNullException("InsertToBankInformation", "Code generation is Manual, but Code not Issued");
                        }
                        else
                        {
                            sqlText = "select count(BankID) from BankInformations where  BankCode=@bankCode";
                            SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                            cmdCodeExist.Transaction = transaction;
                            cmdCodeExist.Parameters.AddWithValue("@bankCode", bankCode);

                            countId = (int)cmdCodeExist.ExecuteScalar();
                            if (countId > 0)
                            {
                                throw new ArgumentNullException("InsertToCustomer", "Same Bank  Code('" + bankCode + "') already exist");
                            }
                        }
                    }
                    else
                    {
                        bankCode = nextId.ToString();

                    }
                    #endregion Code

                    sqlText = "";
                    sqlText += "insert into BankInformations";
                    sqlText += "(";
                    sqlText += "BankID,";
                    sqlText += "BankName,";
                    sqlText += "BranchName,";
                    sqlText += "AccountNumber,";
                    sqlText += "Address1,";
                    sqlText += "Address2,";
                    sqlText += "Address3,";
                    sqlText += "City,";
                    sqlText += "TelephoneNo,";
                    sqlText += "FaxNo,";
                    sqlText += "Email,";
                    sqlText += "ContactPerson,";
                    sqlText += "ContactPersonDesignation,";
                    sqlText += "ContactPersonTelephone,";
                    sqlText += "ContactPersonEmail,";
                    sqlText += "Comments,";
                    sqlText += "ActiveStatus,";
                    sqlText += "CreatedBy,";
                    sqlText += "CreatedOn,";
                    sqlText += "LastModifiedBy,";
                    sqlText += "LastModifiedOn,";
                    sqlText += "BankCode";
                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += "@nextId,";
                    sqlText += "@ItemBankName,";
                    sqlText += "@ItemBranchName,";
                    sqlText += "@ItemAccountNumber,";
                    sqlText += "@ItemAddress1,";
                    sqlText += "@ItemAddress2,";
                    sqlText += "@ItemAddress3,";
                    sqlText += "@ItemCity,";
                    sqlText += "@ItemTelephoneNo,";
                    sqlText += "@ItemFaxNo,";
                    sqlText += "@ItemEmail,";
                    sqlText += "@ItemContactPerson,";
                    sqlText += "@ItemContactPersonDesignation,";
                    sqlText += "@ItemContactPersonTelephone,";
                    sqlText += "@ItemContactPersonEmail,";
                    sqlText += "@ItemComments,";
                    sqlText += "@ItemActiveStatus,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@bankCode";
                    sqlText += ")";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                    cmdInsert.Parameters.AddWithValue("@ItemBankName", Item.BankName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemBranchName", Item.BranchName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemAccountNumber", Item.AccountNumber ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemAddress1", Item.Address1 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemAddress2", Item.Address2 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemAddress3", Item.Address3 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCity", Item.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemTelephoneNo", Item.TelephoneNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemFaxNo", Item.FaxNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemEmail", Item.Email ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemContactPerson", Item.ContactPerson ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemContactPersonDesignation", Item.ContactPersonDesignation ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemContactPersonTelephone", Item.ContactPersonTelephone ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemContactPersonEmail", Item.ContactPersonEmail ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemActiveStatus", Item.ActiveStatus ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedOn", Item.CreatedOn);
                    cmdInsert.Parameters.AddWithValue("@bankCode", bankCode ?? Convert.DBNull);

                    transResult = (int)cmdInsert.ExecuteNonQuery();

                    if (transResult <= 0 || cmdInsert == null)
                    {

                        throw new ArgumentNullException("ImportProduct",
                                                        "Unable to Insert BankName('" + Item.BankName + "')");
                    }
                    #endregion Insert

                }
                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Bank Information successfully Added";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add customer";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected erro to add customer ";
                }

                #endregion COMMIT
            }

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

        public string[] ImportCosting(List<CostingVM> costings)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            
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
                transaction = currConn.BeginTransaction("ImportCosting");

                #endregion open connection and transaction
                CommonImport cImport = new CommonImport();
                int countId = 0;
                #region Code

                sqlText = "select count(ItemNo) from Costing ";
                SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                cmdCodeExist.Transaction = transaction;
                countId = (int)cmdCodeExist.ExecuteScalar();
                

                #endregion Code
                foreach (var Item in costings.ToList())
                {
                    sqlResults = new string[2];

                    #region FindItemId

                    string ItemNo = cImport.FindItemId(Item.ProductName,Item.ProductCode, null, null);
                    #endregion FindItemId

                    #region Insert Costing Information
                    countId++;
                    

                    sqlText = "";
                    sqlText += " insert into Costing ";
                    sqlText += "(";
                    sqlText += " Id,";
                    sqlText += " ItemNo,";
                    sqlText += " BENumber,";
                    sqlText += " RefNo,";
                    sqlText += " InputDate,";
                    sqlText += " CostPrice,";
                    sqlText += " Quantity,";
                    sqlText += " UnitCost,";
                    sqlText += " AV,";
                    sqlText += " CD,";
                    sqlText += " RD,";
                    sqlText += " TVB,";
                    sqlText += " SDAmount,";
                    sqlText += " VATAmount,";
                    sqlText += " TVA,";
                    sqlText += " ATV,";
                    sqlText += " Other,";
                    sqlText += "CreatedBy,";
                    sqlText += "CreatedOn";

                    sqlText += ")";
                    sqlText += " values(";
                    sqlText += "@countId,";
                    sqlText += "@ItemNo,";
                    sqlText += "@ItemBENumber,";
                    sqlText += "@ItemRefNo,";
                    sqlText += "@ItemInputDate,";
                    sqlText += "@ItemCostPrice,";
                    sqlText += "@ItemQuantity,";
                    sqlText += "@ItemUnitCost,";
                    sqlText += "@ItemAV,";
                    sqlText += "@ItemCD,";
                    sqlText += "@ItemRD,";
                    sqlText += "@ItemTVB,";
                    sqlText += "@ItemSDAmount,";
                    sqlText += "@ItemVATAmount,";
                    sqlText += "@ItemTVA,";
                    sqlText += "@ItemATV,";
                    sqlText += "@ItemOther,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn";
                    sqlText += ")";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@countId", countId);
                    cmdInsert.Parameters.AddWithValue("@ItemNo", ItemNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemBENumber", Item.BENumber ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemRefNo", Item.RefNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemInputDate", Ordinary.DateToDate(Item.InputDate));
                    cmdInsert.Parameters.AddWithValue("@ItemCostPrice", Item.CostPrice);
                    cmdInsert.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                    cmdInsert.Parameters.AddWithValue("@ItemUnitCost", Item.UnitCost);
                    cmdInsert.Parameters.AddWithValue("@ItemAV", Item.AV);
                    cmdInsert.Parameters.AddWithValue("@ItemCD", Item.CD);
                    cmdInsert.Parameters.AddWithValue("@ItemRD", Item.RD);
                    cmdInsert.Parameters.AddWithValue("@ItemTVB", Item.TVB);
                    cmdInsert.Parameters.AddWithValue("@ItemSDAmount", Item.SDAmount);
                    cmdInsert.Parameters.AddWithValue("@ItemVATAmount", Item.VATAmount);
                    cmdInsert.Parameters.AddWithValue("@ItemTVA", Item.TVA);
                    cmdInsert.Parameters.AddWithValue("@ItemATV", Item.ATV);
                    cmdInsert.Parameters.AddWithValue("@ItemOther", Item.Other);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedOn", Item.CreatedOn);

                    transResult = (int)cmdInsert.ExecuteNonQuery();

                    if (transResult <= 0 || cmdInsert == null)
                    {

                        throw new ArgumentNullException("ImportCosting",
                                                        "Unable to Insert Costing");
                    }
                    #endregion Insert

                   
                }
                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Costing Information successfully Added";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to insert Costing Price";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected erro to insert Costing Price ";
                }

                #endregion COMMIT
            }

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
        public string[] ImportUOM(List<UOMVM> uoms)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
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
                transaction = currConn.BeginTransaction("ImportUOM");

                #endregion open connection and transaction

                foreach (var Item in uoms.ToList())
                {

                    sqlResults = new string[2];
                    if (string.IsNullOrEmpty(Item.UOMCode))
                    {
                        throw new ArgumentNullException("ImportUOM","UOM Name not exist");
                    }
                    sqlText = "select count(UOMCode) from UOMName where  UOMCode=@ItemUOMCode ";
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValue("@ItemUOMCode", Item.UOMCode);

                    countId = (int)cmdIdExist.ExecuteScalar();
                    if (countId > 0)
                    {
                        throw new ArgumentNullException("InsertToUOMName", "UOM Name (" + Item.UOMName + ") is already exist");
                    }

                    sqlText = "select count(distinct UOMName) from UOMName where  UOMName=@ItemUOMName";
                    SqlCommand cmdNameExist = new SqlCommand(sqlText, currConn);
                    cmdNameExist.Transaction = transaction;
                    cmdNameExist.Parameters.AddWithValue("@ItemUOMName", Item.UOMName);

                    int countName = (int)cmdNameExist.ExecuteScalar();
                    if (countName > 0)
                    {

                        throw new ArgumentNullException("InsertToUOMName", "UOM Name (" + Item.UOMName + ") is already exist");
                    }
                  

                    #region Insert new vehicle
                    sqlText = "";
                    sqlText += "insert into UOMName";
                    sqlText += "(";
                    sqlText += "UOMName,";
                    sqlText += "UOMCode,";
                    sqlText += "Comments,";
                    sqlText += "ActiveStatus,";
                    sqlText += "CreatedBy,";
                    sqlText += "CreatedOn,";
                    sqlText += "LastModifiedBy,";
                    sqlText += "LastModifiedOn";
                    sqlText += ")";

                    sqlText += " values(";
                   
                    sqlText += "@ItemUOMName,";
                    sqlText += "@ItemUOMCode,";
                    sqlText += "@ItemComments,";
                    sqlText += "@ItemActiveStatus,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@ItemLastModifiedBy,";
                    sqlText += "@ItemLastModifiedOn";
                    sqlText += ")";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.Parameters.AddWithValue("@ItemUOMName", Item.UOMName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemUOMCode", Item.UOMCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemComments", Item.Comments ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemActiveStatus", Item.ActiveStatus);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemCreatedOn", Item.CreatedOn);
                    cmdInsert.Parameters.AddWithValue("@ItemLastModifiedBy", Item.LastModifiedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ItemLastModifiedOn", Item.LastModifiedOn);

                    transResult = (int)cmdInsert.ExecuteNonQuery();

                    if (transResult <= 0 || cmdInsert == null)
                    {

                        throw new ArgumentNullException("ImportUOM","Unable to Insert UOM('" + Item.UOMName + "')");
                    }


                }
                    #endregion
                #region Commit


                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested UOM Information successfully Added";

                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add UOM";
                    }

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected erro to add UOM ";
                }

                #endregion COMMIT

            }

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

        public string[] ImportExcelFile(ImportVM paramVM)
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
                DataTable dt = new DataTable();
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

                DataTable dtMaster = new DataTable();
                dtMaster = ds.Tables[paramVM.TableName];

                #region Data Insert
                if (paramVM.TableName == "Customer")
                {
                    var customers = DataTableToCustomer(dtMaster, paramVM);
                    retResults = ImportCustomer(customers);
                }
                if (paramVM.TableName == "Product")
                {
                    var products = DataTableToProduct(dtMaster, paramVM);
                    List<TrackingVM> trackings = new List<TrackingVM>();
                    retResults = ImportProduct(products, trackings);
                }
                if (paramVM.TableName == "Vendor")
                {
                    var vendors = DataTableToVendor(dtMaster, paramVM);
                    retResults = ImportVendor(vendors);
                }
                if (paramVM.TableName == "Vehicle")
                {
                    var vehicles = DataTableToVehicle(dtMaster, paramVM);
                    retResults = ImportVehicle(vehicles);
                }
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }
                #endregion

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
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

        private List<CustomerVM> DataTableToCustomer(DataTable dt, ImportVM paramVM)
        {
            List<CustomerVM> vms=new List<CustomerVM>();
            CustomerVM vm;
            foreach (DataRow row in dt.Rows)
            {
                vm = new CustomerVM();
                vm.CustomerName = row["CustomerName"].ToString();
                vm.CustomerCode = row["Code"].ToString();
                vm.CustomerGroup = row["CustomerGroup"].ToString();
                vm.Address1 = row["Address1"].ToString();
                vm.Address2 = row["Address2"].ToString();
                vm.Address3 = row["Address3"].ToString();
                vm.City = row["City"].ToString();
                vm.TelephoneNo = row["TelephoneNo"].ToString();
                vm.FaxNo = row["FaxNo"].ToString();
                vm.Email = row["Email"].ToString();
                vm.StartDateTime = Convert.ToDateTime(row["StartDateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                vm.ContactPerson = row["ContactPerson"].ToString();
                vm.ContactPersonDesignation = row["ContactPersonDesignation"].ToString();
                vm.ContactPersonTelephone = row["ContactPersonTelephone"].ToString();
                vm.ContactPersonEmail = row["ContactPersonEmail"].ToString();
                vm.TINNo = row["TIN"].ToString();
                vm.VATRegistrationNo = row["VATRegistrationNo"].ToString();
                vm.Comments = row["Comments"].ToString();
                vm.ActiveStatus = row["ActiveStatus"].ToString();
                vm.CreatedBy = paramVM.CreatedBy;
                vm.CreatedOn = paramVM.CreatedOn;
                vm.LastModifiedBy = paramVM.LastModifiedBy;
                vm.LastModifiedOn = paramVM.LastModifiedOn;
                vm.Country = row["Country"].ToString();
                vms.Add(vm);
            }
            return vms;
        }
        private List<ProductVM> DataTableToProduct(DataTable dt, ImportVM paramVM)
        {
            List<ProductVM> vms = new List<ProductVM>();
            ProductVM vm;
            foreach (DataRow row in dt.Rows)
            {
                vm = new ProductVM();
                vm.ProductName = row["ProductName"].ToString();
                vm.ProductDescription = row["Description"].ToString();
                vm.CategoryName = row["Group"].ToString();
                vm.UOM = row["UOM"].ToString();

                string tprice = row["TotalPrice"].ToString();
                string opbalance = row["OpeningQuantity"].ToString();

                if (string.IsNullOrEmpty(tprice))
                { tprice = "0"; }
                if (string.IsNullOrEmpty(opbalance))
                { opbalance = "0"; }

                if (Convert.ToDecimal(opbalance) <= 0 || Convert.ToDecimal(tprice) <= 0)
                {
                    vm.CostPrice = 0;
                }
                else
                {
                    vm.CostPrice = Convert.ToDecimal(tprice) / Convert.ToDecimal(opbalance);//Convert.ToDecimal(row["TotalPrice"].ToString());
                }

                vm.OpeningBalance = Convert.ToDecimal(opbalance);
                vm.OpeningTotalCost = Convert.ToDecimal(tprice);// Convert.ToDecimal(row["OpeningQuantity"].ToString());
                vm.SerialNo = row["RefNo"].ToString();
                vm.HSCodeNo = row["HSCode"].ToString();
                vm.VATRate = Convert.ToDecimal(row["VATRate"].ToString());
                vm.Comments = row["Comments"].ToString();
                vm.ActiveStatus = row["ActiveStatus"].ToString();
                vm.SD = Convert.ToDecimal(row["SDRate"].ToString());
                vm.Packetprice = Convert.ToDecimal(row["PacketPrice"].ToString());
                vm.Trading = row["Trading"].ToString();
                vm.TradingMarkUp = Convert.ToDecimal(row["TradingMarkUp"].ToString());
                vm.NonStock = row["NonStock"].ToString();
                vm.OpeningDate = Convert.ToDateTime(row["OpeningDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                vm.CreatedBy = paramVM.CreatedBy;
                vm.CreatedOn = paramVM.CreatedOn;
                vm.LastModifiedBy = paramVM.LastModifiedBy;
                vm.LastModifiedOn = paramVM.LastModifiedOn;
                vm.ProductCode = row["Code"].ToString();
                vm.TollCharge = Convert.ToDecimal(row["TollCharge"].ToString());
                vms.Add(vm);
            }
            return vms;
        }
        private List<VendorVM> DataTableToVendor(DataTable dt, ImportVM paramVM)
        {
            List<VendorVM> vms = new List<VendorVM>();
            VendorVM vm;
            foreach (DataRow row in dt.Rows)
            {
                vm = new VendorVM();
                vm.VendorCode = row["Code"].ToString();
                vm.VendorName = row["VendorName"].ToString();
                vm.VendorGroup = row["VendorGroup"].ToString();
                vm.Address1 = row["Address1"].ToString();
                vm.Address2 = row["Address2"].ToString();
                vm.Address3 = row["Address3"].ToString();
                vm.City = row["City"].ToString();
                vm.TelephoneNo = row["TelephoneNo"].ToString();
                vm.FaxNo = row["FaxNo"].ToString();
                vm.Email = row["Email"].ToString();
                vm.StartDateTime = Convert.ToDateTime(row["StartDateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                vm.ContactPerson = row["ContactPerson"].ToString();
                vm.ContactPersonDesignation = row["ContactPersonDesignation"].ToString();
                vm.ContactPersonTelephone = row["ContactPersonTelephone"].ToString();
                vm.ContactPersonEmail = row["ContactPersonEmail"].ToString();
                vm.VATRegistrationNo = row["VATRegistrationNo"].ToString();
                vm.TINNo = row["TIN"].ToString();
                vm.Comments = row["Comments"].ToString();
                vm.ActiveStatus = row["ActiveStatus"].ToString();
                vm.CreatedBy =paramVM.CreatedBy;
                vm.CreatedOn = paramVM.CreatedOn;
                vm.LastModifiedBy = paramVM.LastModifiedBy;
                vm.LastModifiedOn = paramVM.LastModifiedOn;
                vm.Country = row["Country"].ToString();
                vms.Add(vm);
            }
            return vms;
        }
        private List<VehicleVM> DataTableToVehicle(DataTable dt, ImportVM paramVM)
        {
            List<VehicleVM> vms = new List<VehicleVM>();
            VehicleVM vm;
            foreach (DataRow row in dt.Rows)
            {
                vm = new VehicleVM();
                vm.VehicleType = row["VehicleType"].ToString();
                vm.Code = row["Code"].ToString();
                vm.VehicleType = row["VehicleType"].ToString();
                vm.VehicleNo = row["VehicleNo"].ToString();
                vm.Description = row["Description"].ToString();
                vm.Comments = row["Comments"].ToString();
                vm.ActiveStatus = row["ActiveStatus"].ToString();
                vm.CreatedBy = paramVM.CreatedBy;
                vm.CreatedOn = paramVM.CreatedOn;
                vm.LastModifiedBy = paramVM.LastModifiedBy;
                vm.LastModifiedOn = paramVM.LastModifiedOn;
                vms.Add(vm);
            }
            return vms;
        }
        #endregion New Methods

        #region New Methods Backup
        //public string[] ImportProduct1(List<ProductVM> products)
        //{
        //    #region Initializ

        //    string[] retResults = new string[3];
        //    retResults[0] = "Fail";
        //    retResults[1] = "Fail";

        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;
        //    int transResult = 0;
        //    int countId = 0;
        //    string sqlText = "";

        //    #endregion Initializ
        //    try
        //    {
        //        #region open connection and transaction

        //        currConn = _dbsqlConnection.GetConnection();

        //        #endregion open connection and transaction

        //        foreach (var Item in products.ToList())
        //        {
        //            if (currConn.State != ConnectionState.Open)
        //            {
        //                currConn.Open();
        //            }
        //            transaction = currConn.BeginTransaction("InsertToProduct");

        //            sqlResults = new string[2];
        //            if (string.IsNullOrEmpty(Item.CategoryName))
        //            {
        //                throw new ArgumentNullException("ImportProduct",
        //                                                "Group Name not exist");
        //            }

        //            sqlText = "select count(CategoryName) from ProductCategories where  CategoryName='" +
        //                      Item.CategoryName + "'";
        //            SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
        //            cmdIdExist.Transaction = transaction;
        //            countId = (int)cmdIdExist.ExecuteScalar();
        //            if (countId <= 0)
        //            {
        //                throw new ArgumentNullException("ImportProduct",
        //                                                "Group Name not exist in Database (" + Item.CategoryName + ").");
        //            }

        //            sqlText = "select distinct CategoryID from ProductCategories where  CategoryName='" +
        //                      Item.CategoryName + "'";
        //            SqlCommand cmdCId = new SqlCommand(sqlText, currConn);
        //            cmdCId.Transaction = transaction;
        //            string CID = (string)cmdCId.ExecuteScalar();
        //            if (countId <= 0)
        //            {
        //                throw new ArgumentNullException("ImportProduct",
        //                                                "Group Name not exist in Database (" + Item.CategoryName +
        //                                                ").");
        //            }
        //            if (currConn != null)
        //            {
        //                if (currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();

        //                }
        //            }
        //            ProductDAL productDal = new ProductDAL();
        //            sqlResults = productDal.InsertToProduct(Item.ItemNo, Item.ProductName, Item.ProductDescription, CID,
        //                                                    Item.UOM, Item.CostPrice, Item.SalesPrice, Item.NBRPrice,
        //                                                    Item.OpeningBalance, Item.SerialNo,
        //                                                    Item.HSCodeNo, Item.VATRate, Item.Comments,
        //                                                    Item.ActiveStatus, Item.CreatedBy, Item.CreatedOn,
        //                                                    Item.CreatedBy, Item.CreatedOn, Item.SD,
        //                                                    Item.Packetprice, Item.Trading, Item.TradingMarkUp,
        //                                                    Item.NonStock, Item.OpeningDate, Item.ProductCode,
        //                                                    Item.TollCharge);

        //            if (sqlResults.Length > 0)
        //            {
        //                retResults[0] = sqlResults[0];
        //                retResults[1] = sqlResults[1];

        //                if (string.IsNullOrEmpty(retResults[0]))
        //                {
        //                    return sqlResults;
        //                }
        //                else if (retResults[0] == "Fail")
        //                {
        //                    return sqlResults;
        //                }
        //            }
        //        }
        //    }

        //    #region Catch and Finall
        //    catch (SqlException sqlex)
        //    {
        //        throw sqlex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    finally
        //    {
        //        if (currConn != null)
        //        {
        //            if (currConn.State == ConnectionState.Open)
        //            {

        //                currConn.Close();

        //            }
        //        }

        //    }

        //    #endregion Catch and Finall
        //    #region Result
        //    return retResults;
        //    #endregion Result

        //}
        //public string[] ImportCustomer1(List<CustomerVM> customers)
        //{
        //    #region Initializ

        //    string[] retResults = new string[3];
        //    retResults[0] = "Fail";
        //    retResults[1] = "Fail";

        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;
        //    int transResult = 0;
        //    int countId = 0;
        //    string sqlText = "";

        //    #endregion Initializ
        //    try
        //    {
        //        #region open connection and transaction

        //        currConn = _dbsqlConnection.GetConnection();

        //        #endregion open connection and transaction

        //        foreach (var Item in customers.ToList())
        //        {
        //            if (currConn.State != ConnectionState.Open)
        //            {
        //                currConn.Open();
        //            }
        //            transaction = currConn.BeginTransaction("ImportCustomer");

        //            sqlResults = new string[2];
        //            if (string.IsNullOrEmpty(Item.CustomerGroup))
        //            {
        //                throw new ArgumentNullException("ImportProduct",
        //                                                "Group Name not exist");
        //            }

        //            sqlText = "select count(CustomerGroupName) from CustomerGroups where  CustomerGroupName='" +
        //                      Item.CustomerGroup + "'";
        //            SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
        //            cmdIdExist.Transaction = transaction;
        //            countId = (int)cmdIdExist.ExecuteScalar();
        //            if (countId <= 0)
        //            {
        //                throw new ArgumentNullException("ImportProduct",
        //                                                "Group Name not exist in Database (" + Item.CustomerGroup + ").");
        //            }

        //            sqlText = "select distinct CustomerGroupID from CustomerGroups where  CustomerGroupName='" +
        //                      Item.CustomerGroup + "'";
        //            SqlCommand cmdCId = new SqlCommand(sqlText, currConn);
        //            cmdCId.Transaction = transaction;
        //            string CID = (string)cmdCId.ExecuteScalar();
        //            if (countId <= 0)
        //            {
        //                throw new ArgumentNullException("ImportProduct",
        //                                                "Group Name not exist in Database (" + Item.CustomerGroup +
        //                                                ").");
        //            }
        //            if (currConn != null)
        //            {
        //                if (currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();

        //                }
        //            }

        //            CustomerDAL customerDal = new CustomerDAL();
        //            sqlResults = customerDal.InsertToCustomerNew(Item.CustomerID, Item.CustomerName, CID, Item.Address1, Item.Address2, Item.Address3, Item.City, Item.TelephoneNo, Item.FaxNo, Item.Email, Item.StartDateTime, Item.ContactPerson
        //                , Item.ContactPersonDesignation, Item.ContactPersonTelephone, Item.ContactPersonEmail, Item.TINNo, Item.VATRegistrationNo, Item.Comments, Item.ActiveStatus, Item.CreatedBy, Item.CreatedOn, Item.CreatedBy, Item.CreatedOn
        //                , Item.Country);

        //            if (sqlResults.Length > 0)
        //            {
        //                retResults[0] = sqlResults[0];
        //                retResults[1] = sqlResults[1];

        //                if (string.IsNullOrEmpty(retResults[0]))
        //                {
        //                    return sqlResults;
        //                }
        //                else if (retResults[0] == "Fail")
        //                {
        //                    return sqlResults;
        //                }
        //            }
        //        }
        //    }

        //    #region Catch and Finall
        //    catch (SqlException sqlex)
        //    {
        //        throw sqlex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    finally
        //    {
        //        if (currConn != null)
        //        {
        //            if (currConn.State == ConnectionState.Open)
        //            {

        //                currConn.Close();

        //            }
        //        }

        //    }

        //    #endregion Catch and Finall
        //    #region Result
        //    return retResults;
        //    #endregion Result

        //}
        //public string[] ImportVendor1(List<VendorVM> vendors)
        //{
        //    #region Initializ

        //    string[] retResults = new string[3];
        //    retResults[0] = "Fail";
        //    retResults[1] = "Fail";

        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;
        //    int transResult = 0;
        //    int countId = 0;
        //    string sqlText = "";

        //    #endregion Initializ
        //    try
        //    {
        //        #region open connection and transaction

        //        currConn = _dbsqlConnection.GetConnection();

        //        #endregion open connection and transaction

        //        foreach (var Item in vendors.ToList())
        //        {
        //            if (currConn.State != ConnectionState.Open)
        //            {
        //                currConn.Open();
        //            }
        //            transaction = currConn.BeginTransaction("ImportVendor");

        //            sqlResults = new string[2];
        //            if (string.IsNullOrEmpty(Item.VendorGroup))
        //            {
        //                throw new ArgumentNullException("ImportVendor",
        //                                                "Group Name not exist");
        //            }

        //            sqlText = "select count(VendorGroupName) from VendorGroups where  VendorGroupName='" +
        //                      Item.VendorGroup + "'";
        //            SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
        //            cmdIdExist.Transaction = transaction;
        //            countId = (int)cmdIdExist.ExecuteScalar();
        //            if (countId <= 0)
        //            {
        //                throw new ArgumentNullException("ImportVendor",
        //                                                "Group Name not exist in Database (" + Item.VendorGroup + ").");
        //            }

        //            sqlText = "select distinct VendorGroupID from VendorGroups where  VendorGroupName='" +
        //                      Item.VendorGroup + "'";
        //            SqlCommand cmdCId = new SqlCommand(sqlText, currConn);
        //            cmdCId.Transaction = transaction;
        //            string CID = (string)cmdCId.ExecuteScalar();
        //            if (countId <= 0)
        //            {
        //                throw new ArgumentNullException("ImportVendor",
        //                                                "Group Name not exist in Database (" + Item.VendorGroup +
        //                                                ").");
        //            }
        //            if (currConn != null)
        //            {
        //                if (currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();

        //                }
        //            }

        //            VendorDAL vendorDal = new VendorDAL();
        //            sqlResults = vendorDal.InsertToVendorNewSQL(Item.VendorID, Item.VendorName, CID, Item.Address1, Item.Address2, Item.Address3, Item.City, Item.TelephoneNo, Item.FaxNo, Item.Email, Item.StartDateTime, Item.ContactPerson, Item.ContactPersonDesignation, Item.ContactPersonTelephone
        //                , Item.ContactPersonEmail, Item.VATRegistrationNo, Item.TINNo, Item.Comments, Item.ActiveStatus, Item.CreatedBy, Item.CreatedOn, Item.CreatedBy, Item.CreatedOn, Item.Country);

        //            if (sqlResults.Length > 0)
        //            {
        //                retResults[0] = sqlResults[0];
        //                retResults[1] = sqlResults[1];

        //                if (string.IsNullOrEmpty(retResults[0]))
        //                {
        //                    return sqlResults;
        //                }
        //                else if (retResults[0] == "Fail")
        //                {
        //                    return sqlResults;
        //                }
        //            }
        //        }
        //    }

        //    #region Catch and Finall
        //    catch (SqlException sqlex)
        //    {
        //        throw sqlex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    finally
        //    {
        //        if (currConn != null)
        //        {
        //            if (currConn.State == ConnectionState.Open)
        //            {

        //                currConn.Close();

        //            }
        //        }

        //    }

        //    #endregion Catch and Finall
        //    #region Result
        //    return retResults;
        //    #endregion Result

        //}
        //public string[] ImportVehicle1(List<VehicleVM> vehicles)
        //{
        //    #region Initializ

        //    string[] retResults = new string[3];
        //    retResults[0] = "Fail";
        //    retResults[1] = "Fail";

        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;
        //    int transResult = 0;
        //    int countId = 0;
        //    string sqlText = "";

        //    #endregion Initializ
        //    try
        //    {
        //        #region open connection and transaction

        //        currConn = _dbsqlConnection.GetConnection();

        //        #endregion open connection and transaction

        //        foreach (var Item in vehicles.ToList())
        //        {
        //            if (currConn.State != ConnectionState.Open)
        //            {
        //                currConn.Open();
        //            }
        //            transaction = currConn.BeginTransaction("ImportVehicle");

        //            sqlResults = new string[2];
        //            if (string.IsNullOrEmpty(Item.VehicleNo))
        //            {
        //                throw new ArgumentNullException("ImportVehicle",
        //                                                "Vehicle Name not exist");
        //            }

        //            sqlText = "select count(VehicleNo) from Vehicles where  VehicleNo='" +
        //                      Item.VehicleNo + "'";
        //            SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
        //            cmdIdExist.Transaction = transaction;
        //            countId = (int)cmdIdExist.ExecuteScalar();
        //            if (countId > 0)
        //            {
        //                throw new ArgumentNullException("ImportVehicle",
        //                                                "Vehicle No not exist in Database (" + Item.VehicleNo + ").");
        //            }


        //            if (currConn != null)
        //            {
        //                if (currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();

        //                }
        //            }

        //            VehicleDAL vehicleDal = new VehicleDAL();
        //            sqlResults = vehicleDal.InsertToVehicle(Item.VehicleID, Item.VehicleType, Item.VehicleNo, Item.Description, Item.Comments, Item.ActiveStatus, Item.CreatedBy, Item.CreatedOn, Item.CreatedBy, Item.CreatedOn);

        //            if (sqlResults.Length > 0)
        //            {
        //                retResults[0] = sqlResults[0];
        //                retResults[1] = sqlResults[1];

        //                if (string.IsNullOrEmpty(retResults[0]))
        //                {
        //                    return sqlResults;
        //                }
        //                else if (retResults[0] == "Fail")
        //                {
        //                    return sqlResults;
        //                }
        //            }
        //        }
        //    }

        //    #region Catch and Finall
        //    catch (SqlException sqlex)
        //    {
        //        throw sqlex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    finally
        //    {
        //        if (currConn != null)
        //        {
        //            if (currConn.State == ConnectionState.Open)
        //            {

        //                currConn.Close();

        //            }
        //        }

        //    }

        //    #endregion Catch and Finall
        //    #region Result
        //    return retResults;
        //    #endregion Result

        //}
        //public string[] ImportBank1(List<BankVM> banks)
        //{
        //    #region Initializ

        //    string[] retResults = new string[3];
        //    retResults[0] = "Fail";
        //    retResults[1] = "Fail";

        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;
        //    int transResult = 0;
        //    int countId = 0;
        //    string sqlText = "";

        //    #endregion Initializ
        //    try
        //    {
        //        #region open connection and transaction

        //        currConn = _dbsqlConnection.GetConnection();

        //        #endregion open connection and transaction

        //        foreach (var Item in banks.ToList())
        //        {
        //            if (currConn.State != ConnectionState.Open)
        //            {
        //                currConn.Open();
        //            }
        //            transaction = currConn.BeginTransaction("ImportBank");

        //            sqlResults = new string[2];
        //            if (string.IsNullOrEmpty(Item.BankName))
        //            {
        //                throw new ArgumentNullException("ImportBank",
        //                                                "Bank not exist");
        //            }

        //            sqlText = "select count(BankID) from BankInformations where  BankName='" +
        //                      Item.BankName + "'";
        //            SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
        //            cmdIdExist.Transaction = transaction;
        //            countId = (int)cmdIdExist.ExecuteScalar();
        //            if (countId > 0)
        //            {
        //                throw new ArgumentNullException("ImportBank",
        //                                                "Bank not exist in Database (" + Item.BankName + ").");
        //            }


        //            if (currConn != null)
        //            {
        //                if (currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();

        //                }
        //            }

        //            BankInformationDAL bankInformationDal = new BankInformationDAL();
        //            sqlResults = bankInformationDal.InsertToBankInformation(Item.BankID, Item.BankName, Item.BranchName, Item.AccountNumber, Item.Address1, Item.Address2, Item.Address3, Item.City, Item.TelephoneNo, Item.FaxNo, Item.Email
        //                , Item.ContactPerson, Item.ContactPersonDesignation, Item.ContactPersonTelephone, Item.ContactPersonEmail, Item.Comments, Item.ActiveStatus, Item.CreatedBy, Item.CreatedOn, Item.CreatedBy, Item.CreatedOn);

        //            if (sqlResults.Length > 0)
        //            {
        //                retResults[0] = sqlResults[0];
        //                retResults[1] = sqlResults[1];

        //                if (string.IsNullOrEmpty(retResults[0]))
        //                {
        //                    return sqlResults;
        //                }
        //                else if (retResults[0] == "Fail")
        //                {
        //                    return sqlResults;
        //                }
        //            }
        //        }
        //    }

        //    #region Catch and Finall
        //    catch (SqlException sqlex)
        //    {
        //        throw sqlex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    finally
        //    {
        //        if (currConn != null)
        //        {
        //            if (currConn.State == ConnectionState.Open)
        //            {

        //                currConn.Close();

        //            }
        //        }

        //    }

        //    #endregion Catch and Finall
        //    #region Result
        //    return retResults;
        //    #endregion Result

        //}
        #endregion New Backup

      
    }

}
