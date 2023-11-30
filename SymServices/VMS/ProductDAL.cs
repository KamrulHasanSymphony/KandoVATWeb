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

namespace SymServices.VMS
{
    public class ProductDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();
        #endregion
        //==================DropDownAll=================
        public List<ProductVM> DropDownAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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
                sqlText = @"
SELECT * FROM(
SELECT 
'B' Sl, ItemNo
, CategoryName
FROM Products
WHERE ActiveStatus='Y'
UNION 
SELECT 
'A' Sl, '-1' Id
, 'ALL Product' CategoryName  
FROM Products
)
AS a
order by Sl,CategoryName
";



                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                }
                dr.Close();
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }

        static string[] columnName = new string[] { "Product Name", "Product Code", "Serial No", "HS Code No"};
        public IEnumerable<object> GetProductColumn()
        {
            IEnumerable<object> enumList = from e in columnName
                                           select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
            return enumList;
        }

        //==================DropDown=================
        public List<ProductVM> DropDown(int CategoryID = 0, string IsRaw = "")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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
                sqlText = @"
SELECT
p.ItemNo
,p.ProductCode
,p.ProductName
,p.CategoryID
,pc.IsRaw
   FROM Products p
LEFT OUTER JOIN ProductCategories pc ON p.CategoryID = pc.CategoryID
WHERE  1=1 AND p.ActiveStatus = 'Y'
";
                if (CategoryID > 0)
                {
                    sqlText += @" AND p.CategoryID=@CategoryID";
                }
                if (!string.IsNullOrWhiteSpace(IsRaw))
                {
                    sqlText += @" AND pc.IsRaw=@IsRaw";
                }
                sqlText += @" order by p.ProductName";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                if (CategoryID > 0)
                {
                    objComm.Parameters.AddWithValue("@CategoryID", CategoryID);
                }
                if (!string.IsNullOrWhiteSpace(IsRaw))
                {
                    objComm.Parameters.AddWithValue("@IsRaw", IsRaw);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductVM();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductName = vm.ProductName + "(" + vm.ProductCode + ")";
                    VMs.Add(vm);
                }
                dr.Close();
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }

        //==================product by type
        public List<ProductVM> GetProductByType(string type)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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
                sqlText = @"
select * from Products pr 
left outer join ProductCategories pc
on pr.CategoryID=pc.CategoryID
where pc.IsRaw=@type and pr.ActiveStatus = 'Y'
ORDER BY pr.ProductName;
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValue("@type", type);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductVM();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductName = vm.ProductName + "(" + vm.ProductCode + ")";
                    VMs.Add(vm);
                }
                dr.Close();
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }

        public List<ProductVM> DropDownByCategory(string catId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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
                sqlText = @"
SELECT
af.ItemNo
,af.ProductCode
,af.ProductName
   FROM Products af
WHERE  1=1 AND af.ActiveStatus = 'Y' AND af.CategoryID=@catId
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValue("@catId", catId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProductVM();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductName = vm.ProductName + "(" + vm.ProductCode + ")";
                    VMs.Add(vm);
                }
                dr.Close();
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return VMs;
        }

        private void SetDefaultValue(ProductVM vm)
        {
            //if (string.IsNullOrWhiteSpace(vm.hsdescription))
            //{
            //    vm.ProductDescription = "-";
            //}
            if (string.IsNullOrWhiteSpace(vm.SerialNo))
            {
                vm.SerialNo = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ProductDescription))
            {
                vm.ProductDescription = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.Comments))
            {
                vm.Comments = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.HSCodeNo))
            {
                vm.HSCodeNo = "NA";
            }
        }
        //public string[] InsertToProduct(string ItemNo, string ProductName, string ProductDescription, string CategoryID,
        //    string UOM, decimal CostPrice, decimal SalesPrice, decimal NBRPrice, decimal OpeningBalance, string SerialNo,
        //    string HSCodeNo, decimal VATRate, string Comments, string ActiveStatus, string CreatedBy, string CreatedOn,
        //    string LastModifiedBy, string LastModifiedOn, decimal SD, decimal Packetprice, string Trading, decimal TradingMarkUp,
        //    string NonStock, string OpeningDate, string ProductCode, decimal TollCharge, string RebatePercent, string ItemType, decimal OpeningTotalCost, string Banderol, string TollProduct
        //    ,List<TrackingVM>Trackings)
        public string[] InsertToProduct(ProductVM vm, List<TrackingVM> Trackings, string ItemType)
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
            int countId = 0;
            string sqlText = "";
            string productCode = vm.ProductCode;
            string itemType = ItemType;
            bool Auto = false;
            string nextId = "0";
            #endregion Initializ

            #region Try
            try
            {
                #region Validation
                SetDefaultValue(vm);
                //////////if (string.IsNullOrEmpty(vm.ProductName))
                //////////{
                //////////    throw new ArgumentNullException("InsertToItem",
                //////////                                    "Please enter product name.");
                //////////}
                ////////////if (string.IsNullOrEmpty(vm.CategoryName))
                ////////////{
                ////////////    throw new ArgumentNullException("InsertToItem",
                ////////////                                    "Please enter product category.");
                ////////////}
                //////////if (string.IsNullOrEmpty(vm.UOM))
                //////////{
                //////////    throw new ArgumentNullException("InsertToItem",
                //////////                                    "Please enter Product UOM.");
                //////////}

                #endregion Validation
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                if (itemType == "Overhead")
                {
                    Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "OverHead") == "Y" ? true : false);

                }
                else
                {
                    Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Item") == "Y" ? true : false);

                }
                #endregion settingsValue
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                //commonDal.TableFieldAdd("Products", "OpeningTotalCost", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                transaction = currConn.BeginTransaction("InsertToItem");


                #endregion open connection and transaction

                #region check in product table
                //if (!string.IsNullOrEmpty(ProductName))
                //{
                //    sqlText = "select count(distinct ProductName) from Products where ProductName='" + ProductName + "'";
                //    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                //    cmdIdExist.Transaction = transaction;
                //    countId = (int)cmdIdExist.ExecuteScalar();
                //    if (countId > 0)
                //    {
                //        //throw new ArgumentNullException("InsertToProducts", "Product already exist.Do you want to save?");
                //        retResults[0] = "Exist";
                //        return retResults;
                //    }

                //}
                #endregion
                #region Product Name and Category Id not exist,Insert new Product
                #region ProductID

                if (itemType == "Overhead")
                {
                    //                    sqlText = @"select 'ovh'+ltrim(rtrim(str(isnull(max(substring(ItemNo,4,len(ItemNo))),0)+1))) from Products 
                    //                               LEFT outer JOIN ProductCategories pc ON Products.CategoryID=pc.CategoryID
                    //                                WHERE pc.IsRaw='Overhead'";
                    sqlText = "select 'ovh'+ltrim(rtrim(str(isnull(max(cast(substring(ItemNo,4,len(ItemNo))AS INT) ),0)+1))) from Products WHERE SUBSTRING(ItemNo,1,3)='ovh'";

                }
                else
                {
                    sqlText = "select isnull(max(cast(ItemNo as int)),0)+1 FROM  Products WHERE SUBSTRING(ItemNo,1,3)<>'ovh'";

                }

                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                object objNextId = cmdNextId.ExecuteScalar();
                if (objNextId == null)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Overhead information Id";
                    throw new ArgumentNullException("InsertToOverheadInformation",
                                                    "Unable to create new Overhead information Id");
                }

                nextId = objNextId.ToString();
                if (string.IsNullOrEmpty(nextId))
                {


                    //THROGUH EXCEPTION BECAUSE WE COULD NOT FIND EXPECTED NEW ID
                    retResults[0] = "Fail";
                    retResults[1] = "Unable to create new Overhead information Id";
                    throw new ArgumentNullException("InsertToOverheadInformation",
                                                    "Unable to create new Overhead information Id");
                }
                #endregion ProductID

                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrEmpty(productCode))
                    {
                        throw new ArgumentNullException("InsertToItem", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(ItemNo) from Products where  ProductCode='" + productCode + "'";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("InsertToItem", "Same Code('" + productCode + "') already exist");
                        }
                    }
                }
                else
                {
                    productCode = nextId;
                }
                #endregion Code
                var Id = _cDal.NextId("Products", null, null).ToString();

                sqlText = "";
                sqlText += @" SET IDENTITY_INSERT Products ON ";

                sqlText += " INSERT into Products";
                sqlText += "(";
                sqlText += "Id,";
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
                sqlText += "RebatePercent,";
                sqlText += "OpeningTotalCost,";
                sqlText += "OpeningDate,";
                sqlText += "Banderol,";

                sqlText += "CDRate,";
                sqlText += "RDRate,";
                sqlText += "TVARate,";
                sqlText += "ATVRate,";
                sqlText += "VATRate2,";
                sqlText += "VDSRate,";

                sqlText += "TollProduct";


                sqlText += ")";
                sqlText += " values(";
                sqlText += "@Id,";
                sqlText += "@nextId,";
                sqlText += "@productCode,";
                sqlText += "@ProductName,";
                sqlText += "@ProductDescription,";
                sqlText += "@CategoryID,";
                sqlText += "@UOM,";
                sqlText += "@CostPrice,";//CostPrice
                sqlText += "@CostPrice,";//SalePrice
                sqlText += "@CostPrice,";//NBRPrice
                sqlText += "@NBRPrice,";//ReceivePrice
                sqlText += "@NBRPrice,";//IssuePrice
                sqlText += "" + 0 + ",";//TenderPrice
                sqlText += "" + 0 + ",";//ExportPrice
                sqlText += "" + 0 + ",";//InternalIssuePrice
                sqlText += "" + 0 + ",";//TollIssueprice
                sqlText += "@TollCharge,";//TollCharge
                sqlText += "@OpeningBalance,";//OpeningBalance
                sqlText += "@SerialNo,";
                sqlText += "@HSCodeNo,";
                sqlText += "@VATRate,";
                sqlText += "@Comments,";
                sqlText += "@ActiveStatus,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn,";
                sqlText += "@SD,";
                sqlText += "@Packetprice,";
                sqlText += "@Trading,";
                sqlText += "@TradingMarkUp,";
                sqlText += "@NonStock,";
                sqlText += "" + 0 + ",";//QuantityInHand
                sqlText += "@RebatePercent,";//QuantityInHand
                sqlText += "@OpeningTotalCost,";//QuantityInHand
                sqlText += "@OpeningDate,";//OpeningDate
                sqlText += "@Banderol,";//Banderol

                sqlText += "@CDRate,";
                sqlText += "@RDRate,";
                sqlText += "@TVARate,";
                sqlText += "@ATVRate,";
                sqlText += "@VATRate2,";
                sqlText += "@VDSRate,";

                sqlText += "@TollProduct";//TollProduct
                sqlText += ")";
                sqlText += @" SET IDENTITY_INSERT Products OFF ";




                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValue("@Id", Id);
                cmdInsert.Parameters.AddWithValue("@nextId", nextId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@productCode", productCode);
                cmdInsert.Parameters.AddWithValue("@ProductName", vm.ProductName ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ProductDescription", vm.ProductDescription ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CategoryID", vm.CategoryID ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@UOM", vm.UOM ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CostPrice", vm.CostPrice);
                cmdInsert.Parameters.AddWithValue("@NBRPrice", vm.NBRPrice);
                cmdInsert.Parameters.AddWithValue("@TollCharge", vm.TollCharge);
                cmdInsert.Parameters.AddWithValue("@OpeningBalance", vm.OpeningBalance);
                cmdInsert.Parameters.AddWithValue("@SerialNo", vm.SerialNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@HSCodeNo", vm.HSCodeNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@VATRate", vm.VATRate);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdInsert.Parameters.AddWithValue("@SD", vm.SD);
                cmdInsert.Parameters.AddWithValue("@Packetprice", vm.Packetprice);
                cmdInsert.Parameters.AddWithValue("@Trading", vm.Trading != null ? "Y" : "N");
                cmdInsert.Parameters.AddWithValue("@TradingMarkUp", vm.TradingMarkUp);
                cmdInsert.Parameters.AddWithValue("@NonStock", vm.NonStock != null ? "Y" : "N");
                cmdInsert.Parameters.AddWithValue("@RebatePercent", vm.RebatePercent);
                cmdInsert.Parameters.AddWithValue("@OpeningTotalCost", vm.OpeningTotalCost);
                cmdInsert.Parameters.AddWithValue("@OpeningDate", vm.OpeningDate ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@Banderol", vm.Banderol);

                cmdInsert.Parameters.AddWithValue("@CDRate", vm.CDRate);
                cmdInsert.Parameters.AddWithValue("@RDRate", vm.RDRate);
                cmdInsert.Parameters.AddWithValue("@TVARate", vm.TVARate);
                cmdInsert.Parameters.AddWithValue("@ATVRate", vm.ATVRate);
                cmdInsert.Parameters.AddWithValue("@VATRate2", vm.VATRate2);
                cmdInsert.Parameters.AddWithValue("@VDSRate", vm.VDSRate);

                cmdInsert.Parameters.AddWithValue("@TollProduct", vm.TollProduct);


                transResult = cmdInsert.ExecuteNonQuery();

                #endregion Product Name and Category Id not exist,Insert new Product

                #region Trackings
                if (Trackings.Count() > 0)
                {
                    Trackings[0].ItemNo = nextId;

                    string trackingUpdate = string.Empty;
                    TrackingDAL trackingDal = new TrackingDAL();
                    trackingUpdate = trackingDal.TrackingInsert(Trackings, transaction, currConn);

                    if (trackingUpdate == "Fail")
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, "Tracking Information not added.");
                    }
                }
                #endregion
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
                retResults[1] = "Requested Item Information successfully Added";
                retResults[2] = "" + Id;
                retResults[3] = "" + productCode;
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
        //public string[] UpdateProduct(string ItemNo, string ProductName, string ProductDescription,
        //    string CategoryID, string UOM, decimal CostPrice, decimal SalesPrice, decimal NBRPrice,
        //    decimal OpeningBalance, string SerialNo, string HSCodeNo, decimal VATRate, string Comments, 
        //    string ActiveStatus, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn,
        //    decimal SD, decimal Packetprice, string Trading, decimal TradingMarkUp, string NonStock, string OpeningDate,
        //    string ProductCode, decimal TollCharge, string RebatePercent, string ItemType, decimal OpeningTotalCost, string Banderol, string TollProduct, List<TrackingVM> Trackings)
        public string[] UpdateProduct(ProductVM vm, List<TrackingVM> Trackings, string ItemType)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = vm.Id;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string productCode = vm.ProductCode;
            string itemType = ItemType;
            bool Auto = false;
            int nextId = 0;
            #endregion Initializ

            #region Try

            try
            {
                #region Validation
                SetDefaultValue(vm);


                //////if (string.IsNullOrEmpty(vm.ItemNo))
                //////{
                //////    throw new ArgumentNullException("UpdateItem",
                //////                "Please enter product no.");
                //////}
                //////if (string.IsNullOrEmpty(vm.ProductName))
                //////{
                //////    throw new ArgumentNullException("UpdateItem",
                //////                "Please enter product name.");
                //////}
                //////if (string.IsNullOrEmpty(vm.CategoryID))
                //////{
                //////    throw new ArgumentNullException("UpdateItem",
                //////                "Please enter product category.");
                //////}
                //////if (string.IsNullOrEmpty(vm.UOM))
                //////{
                //////    throw new ArgumentNullException("UpdateItem",
                //////                "Please enter Product UOM.");
                //////}

                #endregion Validation
                #region settingsValue
                CommonDAL commonDal = new CommonDAL();
                if (itemType == "Overhead")
                {
                    Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "OverHead") == "Y" ? true : false);


                }
                else
                {
                    Auto = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Item") == "Y" ? true : false);

                }
                #endregion settingsValue

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                //commonDal.TableFieldAdd("Products", "OpeningTotalCost", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                transaction = currConn.BeginTransaction("UpdateItem");


                #endregion open connection and transaction

                /*Checking existance of provided bank Id information*/

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where  ItemNo =@ItemNo";
                SqlCommand cmdCIdExist = new SqlCommand(sqlText, currConn);
                cmdCIdExist.Transaction = transaction;
                cmdCIdExist.Parameters.AddWithValue("@ItemNo", vm.ItemNo);

                countId = (int)cmdCIdExist.ExecuteScalar();
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateItem",
                                                    "Unable to find requested product no");
                }
                #endregion ProductExist
                #region Product Exist or not
                /*Checking existance of provided bank Id information*/
                //if (!string.IsNullOrEmpty(ProductName))
                //{


                //    sqlText = "select count(ItemNo) from Products where  ProductName='" + ProductName +
                //              "' and CategoryID='" + CategoryID + "' and ItemNo <>'" + ItemNo + "'";
                //    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                //    cmdIdExist.Transaction = transaction;
                //    countId = (int)cmdIdExist.ExecuteScalar();
                //    if (countId > 0)
                //    {
                //        throw new ArgumentNullException("UpdateItem",
                //                                        "Same Item('" + ProductName + "' ) under same category already exist");
                //    }
                //}

                #endregion ProductExist
                #region Code
                if (Auto == false)
                {
                    if (string.IsNullOrEmpty(productCode))
                    {
                        throw new ArgumentNullException("UpdateItem", "Code generation is Manual, but Code not Issued");
                    }
                    else
                    {
                        sqlText = "select count(ItemNo) from Products where  ProductCode=@productCode and ItemNo <>@ItemNo ";
                        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                        cmdCodeExist.Transaction = transaction;
                        cmdCodeExist.Parameters.AddWithValue("@productCode", productCode);
                        cmdCodeExist.Parameters.AddWithValue("@ItemNo", vm.ItemNo);

                        countId = (int)cmdCodeExist.ExecuteScalar();
                        if (countId > 0)
                        {
                            throw new ArgumentNullException("UpdateItem", "Same Code('" + productCode + "') already exist");
                        }
                    }
                }
                else
                {
                    productCode = vm.ItemNo;
                }

                #endregion Code

                #region Update

                sqlText = "";
                sqlText = "update Products set";
                //sqlText += "(";
                sqlText += " ProductName        =@ProductName,";
                sqlText += " ProductDescription =@ProductDescription,";
                sqlText += " CategoryID         =@CategoryID,";
                sqlText += " UOM                =@UOM,";
                sqlText += " CostPrice          =@CostPrice,";
                sqlText += " OpeningBalance     =@OpeningBalance,";
                sqlText += " OpeningDate        =@OpeningDate,";
                sqlText += " SerialNo           =@SerialNo,";
                sqlText += " HSCodeNo           =@HSCodeNo,";
                sqlText += " VATRate            =@VATRate,";
                sqlText += " Comments           =@Comments,";
                sqlText += " ActiveStatus       =@ActiveStatus,";
                sqlText += " LastModifiedBy     =@LastModifiedBy,";
                sqlText += " LastModifiedOn     =@LastModifiedOn,";
                sqlText += " SD                 =@SD,";
                sqlText += " PacketPrice        =@Packetprice,";
                sqlText += " NBRPrice           =@NBRPrice,";
                sqlText += " receiveprice       =@NBRPrice,";
                sqlText += " Trading            =@Trading,";
                sqlText += " TradingMarkUp      =@TradingMarkUp,";
                sqlText += " NonStock           =@NonStock,";
                sqlText += " TollCharge         =@TollCharge,";
                sqlText += " RebatePercent      =@RebatePercent,";
                sqlText += " OpeningTotalCost   =@OpeningTotalCost,";
                sqlText += " ProductCode        =@productCode,";
                sqlText += " TollProduct        =@TollProduct,";

                sqlText += "CDRate      =@CDRate,";
                sqlText += "RDRate      =@RDRate,";
                sqlText += "TVARate     =@TVARate,";
                sqlText += "ATVRate     =@ATVRate,";
                sqlText += "VATRate2    =@VATRate2,";
                sqlText += "VDSRate     =@VDSRate,";

                sqlText += " Banderol           =@Banderol";
                sqlText += " where ItemNo       =@ItemNo";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@ProductName", vm.ProductName);
                cmdUpdate.Parameters.AddWithValue("@ProductDescription", vm.ProductDescription ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@CategoryID", vm.CategoryID);
                cmdUpdate.Parameters.AddWithValue("@UOM", vm.UOM ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@CostPrice", vm.CostPrice);
                cmdUpdate.Parameters.AddWithValue("@OpeningBalance", vm.OpeningBalance);
                cmdUpdate.Parameters.AddWithValue("@OpeningDate", vm.OpeningDate ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@SerialNo", vm.SerialNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@HSCodeNo", vm.HSCodeNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@VATRate", vm.VATRate);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@SD", vm.SD);
                cmdUpdate.Parameters.AddWithValue("@Packetprice", vm.Packetprice);
                cmdUpdate.Parameters.AddWithValue("@NBRPrice", vm.NBRPrice);
                //cmdUpdate.Parameters.AddWithValue("@NBRPrice", vm.NBRPrice);
                cmdUpdate.Parameters.AddWithValue("@Trading", vm.Trading ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TradingMarkUp", vm.TradingMarkUp);
                cmdUpdate.Parameters.AddWithValue("@NonStock", vm.NonStock ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TollCharge", vm.TollCharge);
                cmdUpdate.Parameters.AddWithValue("@RebatePercent", vm.RebatePercent);
                cmdUpdate.Parameters.AddWithValue("@OpeningTotalCost", vm.OpeningTotalCost);
                cmdUpdate.Parameters.AddWithValue("@productCode", productCode ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@TollProduct", vm.TollProduct);
                cmdUpdate.Parameters.AddWithValue("@Banderol", vm.Banderol);

                cmdUpdate.Parameters.AddWithValue("@CDRate", vm.CDRate);
                cmdUpdate.Parameters.AddWithValue("@RDRate", vm.RDRate);
                cmdUpdate.Parameters.AddWithValue("@TVARate", vm.TVARate);
                cmdUpdate.Parameters.AddWithValue("@ATVRate", vm.ATVRate);
                cmdUpdate.Parameters.AddWithValue("@VATRate2", vm.VATRate2);
                cmdUpdate.Parameters.AddWithValue("@VDSRate", vm.VDSRate);

                cmdUpdate.Parameters.AddWithValue("@ItemNo", vm.ItemNo ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();


                #endregion Update

                #region Tracking

                if (Trackings.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate, MessageVM.PurchasemsgNoDataToUpdateImportDyties);
                }

                foreach (var tracking in Trackings.ToList())
                {

                    #region Find Heading1 Existence

                    sqlText = "";
                    sqlText += "select COUNT(Heading1) from Trackings WHERE Heading1 = '" + tracking.Heading1 + "'";
                    sqlText += " AND ItemNo='" + tracking.ItemNo + "'";

                    SqlCommand cmdFindHeading1 = new SqlCommand(sqlText, currConn);
                    cmdFindHeading1.Transaction = transaction;
                    decimal IDExist = (int)cmdFindHeading1.ExecuteScalar();
                    if (IDExist <= 0)
                    {
                        #region Check Heading2

                        sqlText = "";
                        sqlText += "select COUNT(Heading2) from Trackings WHERE Heading2 = '" + tracking.Heading2 + "'";
                        sqlText += " AND ItemNo='" + tracking.ItemNo + "'";

                        SqlCommand cmdFindHeading2 = new SqlCommand(sqlText, currConn);
                        cmdFindHeading2.Transaction = transaction;
                        decimal IDExist2 = (int)cmdFindHeading2.ExecuteScalar();
                        #endregion
                        if (IDExist2 <= 0)
                        {
                            // Insert
                            #region Insert
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

                            sqlText += " '0',";
                            sqlText += "@trackingItemNo,";
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


                            sqlText += "@LastModifiedBy,";
                            sqlText += "@LastModifiedOn,";
                            sqlText += "@LastModifiedBy,";
                            sqlText += "@LastModifiedOn";

                            sqlText += ")";


                            SqlCommand cmdInsertTrackings = new SqlCommand(sqlText, currConn);
                            cmdInsertTrackings.Transaction = transaction;
                            cmdInsertTrackings.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                            cmdInsertTrackings.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);

                            cmdInsertTrackings.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo ?? Convert.DBNull);
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


                            #endregion
                        }
                        else
                        {
                            //Update
                            #region Update
                            sqlText = "";
                            sqlText += " update Trackings set ";
                            sqlText += " TrackLineNo=@trackingTrackingLineNo,";
                            sqlText += " Heading1   =@trackingHeading1,";
                            sqlText += " Heading2   =@trackingHeading2,";
                            sqlText += " Quantity   =@trackingQuantity,";
                            sqlText += " UnitPrice  =@trackingUnitPrice,";

                            sqlText += " Post= 'Y',";

                            sqlText += " LastModifiedBy = @LastModifiedBy,";
                            sqlText += " LastModifiedOn = @LastModifiedOn";

                            sqlText += " where ItemNo =@trackingItemNo ";
                            sqlText += " and Heading2 =@trackingHeading2 ";

                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;

                            cmdInsDetail.Parameters.AddWithValue("@trackingTrackingLineNo ", tracking.TrackingLineNo);
                            cmdInsDetail.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2 ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingQuantity", tracking.Quantity);
                            cmdInsDetail.Parameters.AddWithValue("@trackingUnitPrice", tracking.UnitPrice);
                            cmdInsDetail.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2);
                            cmdInsDetail.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                            cmdInsDetail.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);

                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                                MessageVM.PurchasemsgUpdateNotSuccessfully);
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        //Update
                        #region Update
                        sqlText = "";
                        sqlText += " update Trackings set ";
                        sqlText += " TrackLineNo= @trackingTrackingLineNo,";
                        sqlText += " Heading1   = @trackingHeading1,";
                        sqlText += " Heading2   = @trackingHeading2,";
                        sqlText += " Quantity   = @trackingQuantity,";
                        sqlText += " UnitPrice  = @trackingUnitPrice,";
                        sqlText += " Post       = 'Y',";

                        sqlText += " LastModifiedBy = @LastModifiedBy,";
                        sqlText += " LastModifiedOn = @LastModifiedOn";


                        sqlText += " where ItemNo =@trackingItemNo ";
                        sqlText += " and Heading1 =@trackingHeading1 ";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@trackingTrackingLineNo ", tracking.TrackingLineNo);
                        cmdInsDetail.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@trackingHeading2", tracking.Heading2 ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@trackingQuantity", tracking.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@trackingUnitPrice", tracking.UnitPrice);
                        cmdInsDetail.Parameters.AddWithValue("@trackingItemNo", tracking.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@trackingHeading1", tracking.Heading1 ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                            MessageVM.PurchasemsgUpdateNotSuccessfully);
                        }
                        #endregion
                    }


                    #endregion Find Heading1 Existence
                }

                #endregion Tracking

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
                retResults[1] = "Requested information successfully updated";
                retResults[2] = vm.Id;
                retResults[3] = productCode;
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

        public string[] Delete(ProductVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteVehicle"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string retVal = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {

                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update Products set";
                        sqlText += " ActiveStatus=@ActiveStatus";
                        sqlText += " ,LastModifiedBy=@LastModifiedBy";
                        sqlText += " ,LastModifiedOn=@LastModifiedOn";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@ActiveStatus", "N");
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Vehicle Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Vehicle Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }


        public string[] DeleteProduct(string ItemNo)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = ItemNo;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(ItemNo))
                {
                    throw new ArgumentNullException("DeleteItem",
                                "Could not find requested Product No.");
                }
                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("Products", "OpeningTotalCost", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo='" + ItemNo + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Could not find requested Prouct information.";
                    return retResults;
                }

                #endregion ProductExist

                #region Tracking
                sqlText = "";
                sqlText += " delete FROM Trackings ";
                sqlText += " WHERE ItemNo='" + ItemNo + "' ";
                sqlText += " AND IsPurchase='N' AND IsReceive='N'";
                SqlCommand cmdInsTracking = new SqlCommand(sqlText, currConn);
                transResult = (int)cmdInsTracking.ExecuteNonQuery();
                #endregion

                #region Delete

                sqlText = "delete Products where ItemNo='" + ItemNo + "'";
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn);
                countId = (int)cmdDelete.ExecuteNonQuery();
                if (countId > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested Product Information successfully deleted";
                }

                #endregion


            }

            #endregion

            #region Catch and Finall

            catch (SqlException sqlex)
            {

                retResults[0] = "Fail";
                retResults[1] = "Database related error. See the log for details";
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {

                retResults[0] = "Fail";
                retResults[1] = "Unexpected error. See the log for details";
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

        public List<ProductVM> SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, ProductVM likeVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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
 Pr.Id
,Pr.ItemNo
,Pr.ProductCode
,Pr.ProductName
,Pr.ProductDescription
,Pr.CategoryID
,Pr.UOM
,isnull(Pr.CostPrice,0) CostPrice
,isnull(Pr.SalesPrice,0) SalesPrice
,isnull(Pr.NBRPrice,0) NBRPrice
,isnull(Pr.ReceivePrice,0) ReceivePrice
,isnull(Pr.IssuePrice,0) IssuePrice
,isnull(Pr.TenderPrice,0) TenderPrice
,isnull(Pr.ExportPrice,0) ExportPrice
,isnull(Pr.InternalIssuePrice,0) InternalIssuePrice
,isnull(Pr.TollIssuePrice,0) TollIssuePrice
,isnull(Pr.TollCharge,0) TollCharge
,isnull(isnull(Pr.OpeningBalance,0)+isnull(Pr.QuantityInHand,0),0) Stock
,Pr.OpeningBalance
,Pr.SerialNo
,Pr.HSCodeNo
,isnull(Pr.VATRate,0) VATRate
,Pr.Comments
,isnull(Pr.SD,0) SD
,isnull(Pr.PacketPrice,0) PacketPrice
,Pr.Trading
,isnull(Pr.TradingMarkUp,0) TradingMarkUp
,Pr.NonStock
,isnull(Pr.QuantityInHand,0) QuantityInHand
,Pr.OpeningDate
,isnull(Pr.RebatePercent,0) RebatePercent
,isnull(Pr.TVBRate,0) TVBRate
,isnull(Pr.CnFRate,0) CnFRate
,isnull(Pr.InsuranceRate,0) InsuranceRate
,isnull(Pr.CDRate,0) CDRate
,isnull(Pr.RDRate,0) RDRate
,isnull(Pr.AITRate,0) AITRate
,isnull(Pr.TVARate,0) TVARate
,isnull(Pr.ATVRate,0) ATVRate
,Pr.ActiveStatus
,Pr.CreatedBy
,Pr.CreatedOn
,Pr.LastModifiedBy
,Pr.LastModifiedOn
,isnull(Pr.OpeningTotalCost,0) OpeningTotalCost

,isnull(Pr.CDRate,0) CDRate
,isnull(Pr.RDRate,0) RDRate
,isnull(Pr.TVARate,0) TVARate
,isnull(Pr.ATVRate,0) ATVRate
,isnull(Pr.VATRate2,0) VATRate2
,isnull(Pr.VDSRate,0) VDSRate

,Pr.Banderol
,Pr.TollProduct
,Pc.CategoryName
,Pc.IsRaw

FROM Products Pr left outer join ProductCategories Pc on Pr.CategoryID=Pc.CategoryID
WHERE  1=1 AND Pr.IsArchive = 0
";


                if (Id != "0")
                {
                    sqlText += @" and Pr.Id=@Id";
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
                    if (!string.IsNullOrWhiteSpace(likeVM.ProductName))
                    {
                        sqlText += " AND Pr.ProductName like @ProductName ";
                    }
                    if (!string.IsNullOrWhiteSpace(likeVM.ProductCode))
                    {
                        sqlText += " AND Pr.ProductCode like @ProductCode ";
                    }
                    if (!string.IsNullOrWhiteSpace(likeVM.HSCodeNo))
                    {
                        sqlText += " AND Pr.HSCodeNo like @HSCodeNo ";
                    }
                    if (!string.IsNullOrWhiteSpace(likeVM.SerialNo))
                    {
                        sqlText += " AND Pr.SerialNo like @SerialNo ";
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
                    if (!string.IsNullOrWhiteSpace(likeVM.ProductName))
                    {
                        objComm.Parameters.AddWithValue("@ProductName", "%" + likeVM.ProductName + "%");
                    }
                    if (!string.IsNullOrWhiteSpace(likeVM.ProductCode))
                    {
                        objComm.Parameters.AddWithValue("@ProductCode", "%" + likeVM.ProductCode + "%");
                    }
                    if (!string.IsNullOrWhiteSpace(likeVM.HSCodeNo))
                    {
                        objComm.Parameters.AddWithValue("@HSCodeNo", "%" + likeVM.HSCodeNo + "%");
                    }
                    if (!string.IsNullOrWhiteSpace(likeVM.SerialNo))
                    {
                        objComm.Parameters.AddWithValue("@SerialNo", "%" + likeVM.SerialNo + "%");
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
                    vm = new ProductVM();
                    vm.Id = dr["Id"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductDescription = dr["ProductDescription"].ToString();
                    vm.CategoryID = dr["CategoryID"].ToString();
                    vm.CategoryName = dr["CategoryName"].ToString();
                    vm.UOM = dr["UOM"].ToString();
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.SalesPrice = Convert.ToDecimal(dr["SalesPrice"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.TollCharge = Convert.ToDecimal(dr["TollCharge"].ToString());
                    vm.OpeningBalance = Convert.ToDecimal(dr["OpeningBalance"].ToString());
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.HSCodeNo = dr["HSCodeNo"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.Comments = dr["Comments"].ToString();
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.Trading = dr["Trading"].ToString();
                    vm.TradingMarkUp = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                    vm.NonStock = dr["NonStock"].ToString();
                    vm.OpeningDate = dr["OpeningDate"].ToString();
                    vm.RebatePercent = Convert.ToDecimal(dr["RebatePercent"].ToString());
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.OpeningTotalCost = Convert.ToDecimal(dr["OpeningTotalCost"].ToString());
                    vm.Banderol = dr["Banderol"].ToString();

                    vm.CDRate = Convert.ToDecimal(dr["CDRate"].ToString());
                    vm.RDRate = Convert.ToDecimal(dr["RDRate"].ToString());
                    vm.TVARate = Convert.ToDecimal(dr["TVARate"].ToString());
                    vm.ATVRate = Convert.ToDecimal(dr["ATVRate"].ToString());
                    vm.VATRate2 = Convert.ToDecimal(dr["VATRate2"].ToString());
                    vm.VDSRate = Convert.ToDecimal(dr["VDSRate"].ToString());
                    vm.CnFRate = Convert.ToDecimal(dr["CnFRate"].ToString());

                    vm.TollProduct = dr["TollProduct"].ToString();
                    vm.Stock = Convert.ToDecimal(dr["Stock"].ToString());
                    vm.ProductType = dr["IsRaw"].ToString();

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

        public List<ProductVM> SelectAllOverhead(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProductVM> VMs = new List<ProductVM>();
            ProductVM vm;
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
 Pr.Id
,Pr.ItemNo
,Pr.ProductCode
,Pr.ProductName
,Pr.ProductDescription
,Pr.CategoryID
,Pr.UOM
,isnull(Pr.CostPrice,0) CostPrice
,isnull(Pr.SalesPrice,0) SalesPrice
,isnull(Pr.NBRPrice,0) NBRPrice
,isnull(Pr.ReceivePrice,0) ReceivePrice
,isnull(Pr.IssuePrice,0) IssuePrice
,isnull(Pr.TenderPrice,0) TenderPrice
,isnull(Pr.ExportPrice,0) ExportPrice
,isnull(Pr.InternalIssuePrice,0) InternalIssuePrice
,isnull(Pr.TollIssuePrice,0) TollIssuePrice
,isnull(Pr.TollCharge,0) TollCharge
,Pr.OpeningBalance
,Pr.SerialNo
,Pr.HSCodeNo
,isnull(Pr.VATRate,0) VATRate
,Pr.Comments
,isnull(Pr.SD,0) SD
,isnull(Pr.PacketPrice,0) PacketPrice
,Pr.Trading
,isnull(Pr.TradingMarkUp,0) TradingMarkUp
,Pr.NonStock
,isnull(Pr.QuantityInHand,0) QuantityInHand
,Pr.OpeningDate
,isnull(Pr.RebatePercent,0) RebatePercent
,isnull(Pr.TVBRate,0) TVBRate
,isnull(Pr.CnFRate,0) CnFRate
,isnull(Pr.InsuranceRate,0) InsuranceRate
,isnull(Pr.CDRate,0) CDRate
,isnull(Pr.RDRate,0) RDRate
,isnull(Pr.AITRate,0) AITRate
,isnull(Pr.TVARate,0) TVARate
,isnull(Pr.ATVRate,0) ATVRate
,Pr.ActiveStatus
,Pr.CreatedBy
,Pr.CreatedOn
,Pr.LastModifiedBy
,Pr.LastModifiedOn
,isnull(Pr.OpeningTotalCost,0) OpeningTotalCost
,Pr.Banderol
,Pr.TollProduct
,Pc.CategoryName

FROM Products Pr left outer join ProductCategories Pc on Pr.CategoryID=Pc.CategoryID
WHERE  1=1 AND Pr.IsArchive = 0 and pc.IsRaw='Overhead'
";


                if (Id != "0")
                {
                    sqlText += @" and Pr.Id=@Id";
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
                        sqlText += " AND Pr." + conditionFields[i] + "=@" + cField;
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
                    vm = new ProductVM();
                    vm.Id = dr["Id"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductDescription = dr["ProductDescription"].ToString();
                    vm.CategoryID = dr["CategoryID"].ToString();
                    vm.CategoryName = dr["CategoryName"].ToString();
                    vm.UOM = dr["UOM"].ToString();
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.SalesPrice = Convert.ToDecimal(dr["SalesPrice"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.TollCharge = Convert.ToDecimal(dr["TollCharge"].ToString());
                    vm.OpeningBalance = Convert.ToDecimal(dr["OpeningBalance"].ToString());
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.HSCodeNo = dr["HSCodeNo"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.Comments = dr["Comments"].ToString();
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.Trading = dr["Trading"].ToString();
                    vm.TradingMarkUp = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                    vm.NonStock = dr["NonStock"].ToString();
                    vm.OpeningDate = dr["OpeningDate"].ToString();
                    vm.RebatePercent = Convert.ToDecimal(dr["RebatePercent"].ToString());
                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.OpeningTotalCost = Convert.ToDecimal(dr["OpeningTotalCost"].ToString());
                    vm.Banderol = dr["Banderol"].ToString();
                    vm.TollProduct = dr["TollProduct"].ToString();

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


        public DataTable SearchProductDT(string ItemNo, string ProductName, string CategoryID, string CategoryName, string UOM, string IsRaw, string SerialNo, string HSCodeNo, string ActiveStatus, string Trading, string NonStock, string ProductCode, string databaseName, string customerId = "0")
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Product");

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

                sqlText = @"SELECT
                                    Products.ItemNo,
                                    isnull(Products.ProductName,'N/A')ProductName,
                                    isnull(Products.ProductDescription,'N/A')ProductDescription,
                                    Products.CategoryID, 
                                    isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
                                    isnull(Products.UOM,'N/A')UOM,
                                    isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
                                    isnull(Products.CostPrice,0)CostPrice,
                                    isnull(Products.SalesPrice,0)SalesPrice,
                                    isnull(Products.NBRPrice,0)NBRPrice,
                                    isnull(ProductCategories.IsRaw,'N')IsRaw,
                                    isnull(Products.SerialNo,'N/A')SerialNo ,
                                    isnull(Products.HSCodeNo,'N/A')HSCodeNo,
                                    isnull(Products.VATRate,0)VATRate,
                                    isnull(Products.ActiveStatus,'N')ActiveStatus,
                                    isnull(Products.OpeningBalance,0)OpeningBalance,
                                    isnull(Products.Comments,'N/A')Comments,
                                    'N/A' HSDescription, 
                                    isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
                                    isnull(Products.SD,0)SD, 
                                    isnull(Products.Packetprice,0)Packetprice, Products.Trading, 
                                    Products.TradingMarkUp,Products.NonStock,
                                    isnull(Products.QuantityInHand,0)QuantityInHand,
                                    convert(varchar, Products.OpeningDate,120)OpeningDate,
                                    isnull(Products.ReceivePrice,0)ReceivePrice,
                                    isnull(Products.IssuePrice,0)IssuePrice,
                                    isnull(Products.ProductCode,'N/A')ProductCode,
                                    isnull(Products.RebatePercent,0)RebatePercent,
                                    isnull(Products.TollCharge,0)TollCharge,
                                    isnull(Products.Banderol,'N')Banderol,
                                    isnull(Products.TollProduct,'N')TollProduct

                                    FROM Products LEFT OUTER JOIN
                                    ProductCategories ON Products.CategoryID = ProductCategories.CategoryID 
                      
                                    WHERE (Products.ItemNo LIKE '%' + @ItemNo + '%'  OR @ItemNo IS NULL) 
                                    AND (Products.ProductName LIKE '%' + @ProductName + '%' OR  @ProductName IS NULL)
                                    AND (Products.CategoryID LIKE '%' + @CategoryID + '%' OR @CategoryID IS NULL) 
                                    AND (ProductCategories.CategoryName LIKE '%' + @CategoryName + '%' OR  @CategoryName IS NULL)
                                    AND (ProductCategories.IsRaw LIKE '%' + @IsRaw + '%' OR @IsRaw IS NULL)  
                                    AND (Products.SerialNo LIKE '%' + @SerialNo + '%' OR @SerialNo IS NULL)
                                    AND (Products.HSCodeNo LIKE '%' + @HSCodeNo + '%' OR @HSCodeNo IS NULL)
                                    AND (Products.ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                                    AND (Products.ProductCode LIKE '%' + @ProductCode + '%' OR @ProductCode IS NULL)
                                    order by Products.ProductCode";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                if (!objCommProduct.Parameters.Contains("@ItemNo"))
                { objCommProduct.Parameters.AddWithValue("@ItemNo", ItemNo); }
                else { objCommProduct.Parameters["@ItemNo"].Value = ItemNo; }
                if (!objCommProduct.Parameters.Contains("@ProductName"))
                { objCommProduct.Parameters.AddWithValue("@ProductName", ProductName); }
                else { objCommProduct.Parameters["@ProductName"].Value = ProductName; }
                if (!objCommProduct.Parameters.Contains("@CategoryID"))
                { objCommProduct.Parameters.AddWithValue("@CategoryID", CategoryID); }
                else { objCommProduct.Parameters["@CategoryID"].Value = CategoryID; }
                if (!objCommProduct.Parameters.Contains("@CategoryName"))
                { objCommProduct.Parameters.AddWithValue("@CategoryName", CategoryName); }
                else { objCommProduct.Parameters["@CategoryName"].Value = CategoryName; }
                if (!objCommProduct.Parameters.Contains("@UOM"))
                { objCommProduct.Parameters.AddWithValue("@UOM", UOM); }
                else { objCommProduct.Parameters["@UOM"].Value = UOM; }
                if (!objCommProduct.Parameters.Contains("@IsRaw"))
                { objCommProduct.Parameters.AddWithValue("@IsRaw", IsRaw); }
                else { objCommProduct.Parameters["@IsRaw"].Value = IsRaw; }
                if (!objCommProduct.Parameters.Contains("@SerialNo"))
                { objCommProduct.Parameters.AddWithValue("@SerialNo", SerialNo); }
                else { objCommProduct.Parameters["@SerialNo"].Value = SerialNo; }
                if (!objCommProduct.Parameters.Contains("@HSCodeNo"))
                { objCommProduct.Parameters.AddWithValue("@HSCodeNo", HSCodeNo); }
                else { objCommProduct.Parameters["@HSCodeNo"].Value = HSCodeNo; }
                if (!objCommProduct.Parameters.Contains("@ActiveStatus"))
                { objCommProduct.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommProduct.Parameters["@ActiveStatus"].Value = ActiveStatus; }
                if (!objCommProduct.Parameters.Contains("@Trading"))
                { objCommProduct.Parameters.AddWithValue("@Trading", Trading); }
                else { objCommProduct.Parameters["@Trading"].Value = Trading; }
                if (!objCommProduct.Parameters.Contains("@NonStock"))
                { objCommProduct.Parameters.AddWithValue("@NonStock", NonStock); }
                else { objCommProduct.Parameters["@NonStock"].Value = NonStock; }
                if (ProductCode == "")
                {
                    if (!objCommProduct.Parameters.Contains("@ProductCode"))
                    { objCommProduct.Parameters.AddWithValue("@ProductCode", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@ProductCode"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@ProductCode"))
                    { objCommProduct.Parameters.AddWithValue("@ProductCode", ProductCode); }
                    else { objCommProduct.Parameters["@ProductCode"].Value = ProductCode; }
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

        public DataTable SearchProductMiniDS(string ItemNo, string CategoryID, string IsRaw, string CategoryName,
            string ActiveStatus, string Trading, string NonStock, string ProductCode)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("DT");

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


SELECT   
Products.ItemNo,
isnull(Products.ProductName,'N/A')ProductName,
isnull(Products.ProductCode,'N/A')ProductCode,
Products.CategoryID, 
isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
isnull(Products.UOM,'N/A')UOM,
isnull(Products.HSCodeNo,'N/A')HSCodeNo,
isnull(ProductCategories.IsRaw,'N/A')IsRaw,
isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
isnull(Products.CostPrice,0)CostPrice,
isnull(Products.OpeningBalance,0)OpeningBalance, 
isnull(Products.SalesPrice,0)SalesPrice,
isnull(Products.NBRPrice,0)NBRPrice,
isnull( Products.ReceivePrice,0)ReceivePrice,
isnull( Products.IssuePrice,0)IssuePrice,
isnull(Products.Packetprice,0)Packetprice, 
isnull(Products.TenderPrice,0)TenderPrice, 
isnull(Products.ExportPrice,0)ExportPrice, 
isnull(Products.InternalIssuePrice,0)InternalIssuePrice, 
isnull(Products.TollIssuePrice,0)TollIssuePrice, 
isnull(Products.TollCharge,0)TollCharge, 
isnull(Products.VATRate,0)VATRate,
isnull(Products.SD,0)SD,
isnull(Products.TradingMarkUp,0)TradingMarkUp,
isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
isnull(Products.QuantityInHand,0)QuantityInHand,
isnull(Products.Trading,'N')Trading, 
isnull(Products.NonStock,'N')NonStock,
isnull(Products.RebatePercent,0)RebatePercent

FROM Products LEFT OUTER JOIN
ProductCategories ON Products.CategoryID = ProductCategories.CategoryID ";


                sqlText += "  WHERE (Products.ItemNo LIKE '%' +'" + ItemNo + "'+ '%'  OR Products.ItemNo IS NULL) AND";
                sqlText += " (Products.CategoryID  LIKE '%'  +'" + CategoryID + "'+  '%' OR Products.CategoryID IS NULL) ";
                sqlText += " AND (ProductCategories.IsRaw LIKE '%'+'" + IsRaw + "'+  '%' OR ProductCategories.IsRaw IS NULL)  ";
                sqlText += " AND (ProductCategories.CategoryName LIKE '%'+'" + CategoryName + "'+  '%' OR ProductCategories.CategoryName IS NULL)  ";
                sqlText += " AND (Products.ActiveStatus LIKE '%'+'" + ActiveStatus + "'+  '%' OR Products.ActiveStatus IS NULL)";
                sqlText += " AND (Products.Trading LIKE '%' +'" + Trading + "'+  '%' OR Products.Trading IS NULL)";
                sqlText += " AND (Products.NonStock LIKE '%' +'" + NonStock + "'+  '%' OR Products.NonStock IS NULL)";
                sqlText += " AND (Products.ProductCode LIKE '%'+'" + ProductCode + "'+  '%' OR Products.ProductCode IS NULL)";
                sqlText += " order by Products.ItemNo ";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

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

        public DataTable SearchProductFinder(string ProductName, string ProductCode, string IsRaw, string CustomerId)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("DT");

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


SELECT   
Products.ItemNo,
isnull(Products.ProductName,'N/A')ProductName,
isnull(Products.ProductCode,'N/A')ProductCode,
Products.CategoryID, 
isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
isnull(Products.UOM,'N/A')UOM,
isnull(Products.HSCodeNo,'N/A')HSCodeNo,
isnull(ProductCategories.IsRaw,'N/A')IsRaw,
isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
isnull(Products.CostPrice,0)CostPrice,
isnull(Products.OpeningBalance,0)OpeningBalance, 
isnull(Products.SalesPrice,0)SalesPrice,
isnull(Products.NBRPrice,0)NBRPrice,
isnull( Products.ReceivePrice,0)ReceivePrice,
isnull( Products.IssuePrice,0)IssuePrice,
isnull(Products.Packetprice,0)Packetprice, 
isnull(Products.TenderPrice,0)TenderPrice, 
isnull(Products.ExportPrice,0)ExportPrice, 
isnull(Products.InternalIssuePrice,0)InternalIssuePrice, 
isnull(Products.TollIssuePrice,0)TollIssuePrice, 
isnull(Products.TollCharge,0)TollCharge, 
isnull(Products.VATRate,0)VATRate,
isnull(Products.SD,0)SD,
isnull(Products.TradingMarkUp,0)TradingMarkUp,
isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
isnull(Products.QuantityInHand,0)QuantityInHand,
isnull(Products.Trading,'N')Trading, 
isnull(Products.NonStock,'N')NonStock,
isnull(Products.RebatePercent,0)RebatePercent

FROM Products LEFT OUTER JOIN
ProductCategories ON Products.CategoryID = ProductCategories.CategoryID ";
                sqlText += "  WHERE (1=1) ";
                sqlText += " AND (Products.ActiveStatus ='Y')";
                if (!string.IsNullOrEmpty(IsRaw))
                {
                    sqlText += " AND (ProductCategories.IsRaw = '" + IsRaw + "')  ";
                }
                //sqlText += " AND (ProductCategories.IsRaw LIKE '%'+'" + IsRaw + "'+  '%' OR ProductCategories.IsRaw IS NULL)";
                sqlText += " AND (Products.ProductCode LIKE '%'+'" + ProductCode + "'+  '%' OR Products.ProductCode IS NULL)";
                sqlText += " AND (Products.ProductName LIKE '%'+'" + ProductName + "'+  '%' OR Products.ProductName IS NULL)";
                if (!string.IsNullOrEmpty(CustomerId) && CustomerId != "0")
                {
                    sqlText += "and Products.ItemNo in(select distinct FinishItemNo from BOMs where CustomerID ='" + CustomerId + "')   ";
                }

                sqlText += " order by Products.ItemNo ";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

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

        public decimal GetLastNBRPriceFromBOM(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0")
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

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

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo='" + itemNo + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price

                sqlText = "  ";
                sqlText += " select top 1 isnull(nbrprice,0) from boms";
                sqlText += " where ";
                sqlText += " FinishItemNo='" + itemNo + "' ";
                sqlText += " and vatname='" + VatName + "' ";
                sqlText += " and effectdate<='" + effectDate + "'";
                sqlText += " and post='Y'";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                }
                if (retResults == 0)
                {
                    sqlText = "  ";
                    sqlText += " select isnull(NBRPrice,0) from products";
                    sqlText += " where ";
                    sqlText += " itemno='" + itemNo + "' ";

                    SqlCommand cmdGetLastNBRPriceFromProducts = new SqlCommand(sqlText, currConn);
                    cmdGetLastNBRPriceFromProducts.Transaction = transaction;
                    if (cmdGetLastNBRPriceFromProducts.ExecuteScalar() == null)
                    {
                        retResults = 0;
                    }
                    else
                    {
                        retResults = (decimal)cmdGetLastNBRPriceFromProducts.ExecuteScalar();
                    }
                }


                #endregion Last Price

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

        public decimal GetLastNBRPriceFromBOM(string itemNo, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0")
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

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

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo='" + itemNo + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price

                sqlText = "  ";
                sqlText += " select top 1 isnull(nbrprice,0) from boms";
                sqlText += " where ";
                sqlText += " FinishItemNo='" + itemNo + "' ";
                //sqlText += " and vatname='" + VatName + "' ";
                sqlText += " and effectdate<='" + effectDate + "'";
                sqlText += " and post='Y'";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                }
                if (retResults == 0)
                {
                    sqlText = "  ";
                    sqlText += " select isnull(NBRPrice,0) from products";
                    sqlText += " where ";
                    sqlText += " itemno='" + itemNo + "' ";

                    SqlCommand cmdGetLastNBRPriceFromProducts = new SqlCommand(sqlText, currConn);
                    cmdGetLastNBRPriceFromProducts.Transaction = transaction;
                    if (cmdGetLastNBRPriceFromProducts.ExecuteScalar() == null)
                    {
                        retResults = 0;
                    }
                    else
                    {
                        retResults = (decimal)cmdGetLastNBRPriceFromProducts.ExecuteScalar();
                    }
                }


                #endregion Last Price

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

        public decimal GetLastUseQuantityFromBOM(string FinishItemNo, string RawItemNo, string VatName, DateTime effectDate, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(FinishItemNo))
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (effectDate < DateTime.MinValue || effectDate > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

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



                #region Last UseQuantity

                sqlText = "  ";
                sqlText += " SELECT TOP 1 ISNULL(isnull(UOMUQty,0)+isnull(UOMWQty,0),0)TotalQty FROM BOMRaws ";
                sqlText += " where ";
                sqlText += " FinishItemNo='" + FinishItemNo + "' ";
                sqlText += " and RawItemNo='" + RawItemNo + "' ";
                sqlText += " and vatname='VAT 1 Ga (Export)' ";
                //sqlText += " and effectdate<='" + effectDate + "'";
                sqlText += " and post='Y'";
                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                }
                if (retResults == 0)
                {
                    sqlText = "  ";
                    sqlText += " SELECT TOP 1 ISNULL(isnull(UOMUQty,0)+isnull(UOMWQty,0),0)TotalQty FROM BOMRaws ";
                    sqlText += " where ";
                    sqlText += " FinishItemNo='" + FinishItemNo + "' ";
                    sqlText += " and RawItemNo='" + RawItemNo + "' ";
                    sqlText += " and vatname='VAT 1' ";
                    //sqlText += " and effectdate<='" + effectDate + "'";
                    sqlText += " and post='Y'";
                    sqlText += " order by effectdate desc ";


                    SqlCommand cmdGetLastNBRPriceFromBOM1 = new SqlCommand(sqlText, currConn);
                    cmdGetLastNBRPriceFromBOM1.Transaction = transaction;
                    if (cmdGetLastNBRPriceFromBOM1.ExecuteScalar() == null)
                    {
                        retResults = 0;
                    }
                    else
                    {
                        retResults = (decimal)cmdGetLastNBRPriceFromBOM1.ExecuteScalar();
                    }
                }



                #endregion Last UseQuantity

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

        public string GetBomIdFromBOM(string FinishItemNo, string RawItemNo, string VatName, DateTime effectDate, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "0";
            int countId = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(FinishItemNo))
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");
                }
                else if (effectDate < DateTime.MinValue || effectDate > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastNBRPriceFromBOM", "There is No data to find Price");

                }
                #endregion Validation

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



                #region Last UseQuantity

                sqlText = "  ";
                sqlText += " SELECT TOP 1 BOMID FROM BOMRaws ";
                sqlText += " where ";
                sqlText += " FinishItemNo='" + FinishItemNo + "' ";
                sqlText += " and RawItemNo='" + RawItemNo + "' ";
                sqlText += " and vatname='VAT 1 Ga (Export)' ";
                //sqlText += " and effectdate<='" + effectDate + "'";
                sqlText += " and post='Y'";
                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = "0";
                }
                else
                {
                    retResults = cmdGetLastNBRPriceFromBOM.ExecuteScalar().ToString();
                }
                if (retResults == "0")
                {
                    sqlText = "  ";
                    sqlText += " SELECT TOP 1 BOMID FROM BOMRaws ";

                    sqlText += " where ";
                    sqlText += " FinishItemNo='" + FinishItemNo + "' ";
                    sqlText += " and RawItemNo='" + RawItemNo + "' ";
                    sqlText += " and vatname='VAT 1' ";
                    //sqlText += " and effectdate<='" + effectDate + "'";
                    sqlText += " and post='Y'";
                    sqlText += " order by effectdate desc ";


                    SqlCommand cmdGetLastNBRPriceFromBOM1 = new SqlCommand(sqlText, currConn);
                    cmdGetLastNBRPriceFromBOM1.Transaction = transaction;
                    if (cmdGetLastNBRPriceFromBOM1.ExecuteScalar() == null)
                    {
                        retResults = "0";
                    }
                    else
                    {
                        retResults = cmdGetLastNBRPriceFromBOM1.ExecuteScalar().ToString();
                    }
                }



                #endregion Last UseQuantity

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

        public decimal GetLastVatableFromBOM(string itemNo, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("GetLastVatableFromBOM", "There is No data to find Price");
                }

                #endregion Validation

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

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo='" + itemNo + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastVatableFromBOM", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price
                //kkk
                sqlText = "  ";
                sqlText += " select top 1 isnull(UOMPrice,0) from BOMRaws";
                sqlText += " where ";
                sqlText += " rawItemNo='" + itemNo + "' ";
                sqlText += " and post='Y' ";
                sqlText += " order by effectdate desc ";

                SqlCommand cmdGetLastVatableFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastVatableFromBOM.Transaction = transaction;
                if (cmdGetLastVatableFromBOM.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdGetLastVatableFromBOM.ExecuteScalar();
                }



                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall
            catch (Exception ex)
            {

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

        public decimal GetLastTollChargeFBOMOH(string HeadName, string VatName, string effectDate,
            SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            decimal retResults = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(HeadName))
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");

                }
                #endregion Validation

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

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo='" + HeadName + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");
                }

                #endregion ProductExist

                //st
                sqlText = "  ";
                sqlText += " select top 1 isnull(RebateAmount,0) from BOMCompanyOverhead";
                sqlText += " where ";
                sqlText += " HeadId='" + HeadName + "' ";
                sqlText += " and vatname='" + VatName + "' ";
                sqlText += " and effectdate<='" + effectDate + "'";
                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM1 = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM1.Transaction = transaction;
                if (cmdGetLastNBRPriceFromBOM1.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdGetLastNBRPriceFromBOM1.ExecuteScalar();
                }
                if (retResults == 0)
                {
                    #region Last Price

                    //st
                    sqlText = "  ";
                    sqlText += " select top 1 isnull(AdditionalCost,0) from BOMCompanyOverhead";
                    sqlText += " where ";
                    sqlText += " HeadId='" + HeadName + "' ";
                    sqlText += " and vatname='" + VatName + "' ";
                    sqlText += " and effectdate<='" + effectDate + "'";
                    sqlText += " order by effectdate desc ";


                    SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                    cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                    if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                    {
                        retResults = 0;
                    }
                    else
                    {
                        retResults = (decimal)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                    }
                    if (retResults == 0)
                    {
                        sqlText = "  ";
                        sqlText += " select isnull(TollCharge,0) from products";
                        sqlText += " where ";
                        sqlText += " ProductName='" + HeadName + "' ";

                        SqlCommand cmdGetLastNBRPriceFromProducts = new SqlCommand(sqlText, currConn);
                        cmdGetLastNBRPriceFromProducts.Transaction = transaction;
                        if (cmdGetLastNBRPriceFromProducts.ExecuteScalar() == null)
                        {
                            retResults = 0;
                        }
                        else
                        {
                            retResults = (decimal)cmdGetLastNBRPriceFromProducts.ExecuteScalar();
                        }
                    }
                }


                    #endregion Last Price

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

        public string GetFinishItemIdFromOH(string ItemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(ItemNo))
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");

                }
                #endregion Validation

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

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo='" + ItemNo + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price
                //1
                sqlText = "  ";
                sqlText += " select top 1 FinishItemNo from BOMCompanyOverhead";
                sqlText += " where ";
                sqlText += "  vatname='" + VatName + "' ";
                sqlText += " and effectdate<='" + effectDate + "'";
                sqlText += " and HeadId='" + ItemNo + "'";

                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = "0";
                }
                else
                {
                    retResults = (string)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                }
                if (string.IsNullOrEmpty(retResults))
                {
                    sqlText = "  ";
                    sqlText += " select isnull(TollCharge,0) from products";
                    sqlText += " where ";
                    sqlText += " ItemNo='" + ItemNo + "' ";

                    SqlCommand cmdGetLastNBRPriceFromProducts = new SqlCommand(sqlText, currConn);
                    cmdGetLastNBRPriceFromProducts.Transaction = transaction;
                    if (cmdGetLastNBRPriceFromProducts.ExecuteScalar() == null)
                    {
                        retResults = "0";
                    }
                    else
                    {
                        retResults = (string)cmdGetLastNBRPriceFromProducts.ExecuteScalar();
                    }
                }


                #endregion Last Price

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

        public string GetBOMIdFromOH(string OverHeadItemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(OverHeadItemNo))
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue || Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");

                }
                #endregion Validation

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

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo='" + OverHeadItemNo + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("GetLastTollChargeFromBOMOH", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last Price
                //1
                sqlText = "  ";
                sqlText += " select top 1 BOMId from BOMCompanyOverhead";
                sqlText += " where ";
                sqlText += "  vatname='" + VatName + "' ";
                sqlText += " and effectdate<='" + effectDate + "'";
                sqlText += " and HeadID='" + OverHeadItemNo + "'";

                sqlText += " order by effectdate desc ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;

                var exec = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (exec == null)
                {
                    retResults = "0";
                }
                else
                {
                    retResults = cmdGetLastNBRPriceFromBOM.ExecuteScalar().ToString();
                }



                #endregion Last Price

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

        public decimal GetLastLIFOPrice(string itemNo, DateTime receiveDate)
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

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("GetLastLIFOPrice", "There is No data in purchase");
                }
                //else if (receiveDate < DateTime.MinValue || receiveDate > DateTime.MaxValue)
                //{
                //    throw new ArgumentNullException("GetLastLIFOPrice", "There is No data to find Price");

                //}
                #endregion Validation

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




                #region Stock

                sqlText = "  ";



                sqlText += " DECLARE @qty DECIMAL(25,9);";
                sqlText += " select top 1 ";
                sqlText += " @qty =isnull(UOMQty,0)";
                sqlText += " from PurchaseInvoiceDetails where  ItemNo='" + itemNo + "'  and Post='Y'  ";
                sqlText += " and ReceiveDate<='" + receiveDate + "' order by ReceiveDate desc ";
                sqlText += " IF(@qty=0)";
                sqlText += " BEGIN";
                sqlText += " SELECT 0;";
                sqlText += " END";
                sqlText += " ELSE";
                sqlText += " BEGIN";
                sqlText += " select top 1 ";
                sqlText += " isnull(isnull(SubTotal,0) +isnull(CnFAmount,0) +";
                sqlText += " isnull(InsuranceAmount,0)  +";
                sqlText += " isnull(CDAmount,0)  +";
                sqlText += " isnull(SDAmount,0)  +";
                sqlText += " isnull(OthersAmount,0)  +";
                sqlText += " isnull(RDAmount,0) ,0)  / isnull(UOMQty,0)";
                sqlText += " from PurchaseInvoiceDetails where  ItemNo='" + itemNo + "'  and Post='Y'  ";
                sqlText += " and ReceiveDate<='" + receiveDate + "' order by ReceiveDate desc ";
                sqlText += " END";



                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                    //object objDel = cmdDelete.ExecuteScalar();
                }

                #endregion Stock

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

        public DataTable GetLIFOPurchaseInformation(string itemNo, string receiveDate, string PurchaseInvoiceNo = "")
        {
            #region Initializ

            DataTable retResults = new DataTable();
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("GetLIFOPurchaseInformation", "There is No data in purchase");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }


                #endregion open connection and transaction

                CommonDAL _cDal = new CommonDAL();
                bool ImportCostingIncludeATV = false;
                ImportCostingIncludeATV = Convert.ToBoolean(_cDal.settings("Purchase", "ImportCostingIncludeATV") == "Y" ? true : false);

                #region Stock

                sqlText = "  ";

                sqlText +=
                    @"  
                DECLARE @costPrice AS DECIMAL(25,9);
                DECLARE @effectDate AS datetime;
                DECLARE @receiveDate AS datetime;

                SELECT TOP 1 @costPrice=  isnull(CostPrice,0),@effectDate=InputDate  FROM  Costing 
                ";
                sqlText += "  where  ItemNo='" + itemNo + "' and InputDate<='" + receiveDate + "'  ORDER BY InputDate DESC ";

                sqlText += @"
                SELECT TOP 1 @receiveDate=receiveDate FROM  PurchaseInvoiceDetails";
                sqlText += " where  ItemNo='" + itemNo + "' and Post='Y' and ReceiveDate<='" + receiveDate + "' ";
                if (!string.IsNullOrEmpty(PurchaseInvoiceNo))
                {
                    sqlText += " and PurchaseInvoiceNo='" + PurchaseInvoiceNo + "'";

                }
                sqlText += " ORDER BY receiveDate DESC";

                sqlText += "\n if @receiveDate!='' ";
                sqlText += "\n Begin";

                sqlText += @"  

                select top 1 PurchaseInvoiceNo,isnull(NBRPrice,0)CostPrice, isnull(UOMQty,0) PurchaseQuantity,";
                if (ImportCostingIncludeATV)
                {
                    sqlText += @"      CASE WHEN TransactionType='Import' OR TransactionType='InputServiceImport' OR TransactionType='ServiceImport' 
                OR TransactionType='ServiceNSImport' OR TransactionType='TradingImport' THEN
                isnull((isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(SDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0) 	
                ";
                }
                else
                {
                    sqlText += @"      CASE
                WHEN   TransactionType='InputServiceImport' OR TransactionType='ServiceImport' 
                OR TransactionType='ServiceNSImport' OR TransactionType='TradingImport' THEN
                isnull((isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(SDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(OthersAmount,0)),0)),0) 	
                
                WHEN TransactionType='Import'   THEN
                isnull((isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(SDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(OthersAmount,0)),0)),0) 	
              
";
                }


                sqlText += @"      ELSE 
                isnull(SubTotal,0)
                END AS  PurchaseCostPrice

                from PurchaseInvoiceDetails 

                ";

                sqlText += "  where  ItemNo='" + itemNo + "'  and Post='Y'  ";
                sqlText += " and ReceiveDate<='" + receiveDate + "'  ";
                if (!string.IsNullOrEmpty(PurchaseInvoiceNo))
                {
                    sqlText += " and PurchaseInvoiceNo='" + PurchaseInvoiceNo + "'";

                }
                sqlText += " ORDER BY receiveDate DESC";
                sqlText += @" EnD 
                ELSE
                BEGIN 
                select top 1 Id PurchaseInvoiceNo, isnull((costPrice-VATAmount),0) PurchaseCostPrice,quantity PurchaseQuantity,costPrice FROM  Costing ";
                sqlText += " where  ItemNo ='" + itemNo + "' ORDER BY InputDate DESC ";

                sqlText += "END";



                DataTable dataTable = new DataTable("UsableItems");
                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                SqlDataAdapter dataAdapt = new SqlDataAdapter(cmdGetLastNBRPriceFromBOM);
                dataAdapt.Fill(dataTable);

                if (dataTable == null)
                {
                    throw new ArgumentNullException("GetLastLIFOPriceNInvNo", "No row found ");
                }
                retResults = dataTable;
                return retResults;


                #endregion Stock

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

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();

                }

            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public DataTable AvgPriceNew(string itemNo, string tranDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, bool isPost)
        {
            #region Initializ

            string sqlText = "";
            DataTable retResults = new DataTable();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }
                else if (Convert.ToDateTime(tranDate) < DateTime.MinValue || Convert.ToDateTime(tranDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");

                }
                #endregion Validation

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
                ProductDAL productDal = new ProductDAL();

                var AvgPriceVAT16 = productDal.AvgPriceVAT16(itemNo, tranDate, currConn, transaction);
                var AvgPriceVAT17 = productDal.AvgPriceVAT17(itemNo, tranDate, currConn, transaction);
                decimal Quantity = Convert.ToDecimal(AvgPriceVAT16.Rows[0]["Quantity"]);
                decimal Amount = Convert.ToDecimal(AvgPriceVAT16.Rows[0]["Amount"]);

                Quantity = Quantity + Convert.ToDecimal(AvgPriceVAT17.Rows[0]["Quantity"]);
                Amount = Amount + Convert.ToDecimal(AvgPriceVAT17.Rows[0]["Amount"]);
                retResults.Columns.Add("Quantity");
                retResults.Columns.Add("Amount");
                retResults.Rows.Add(new object[] { Quantity, Amount });

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
                //if (Vtransaction == null && transaction != null)
                //{
                //    transaction.Commit();
                //}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public DataTable AvgPriceNewBackup(string itemNo, string tranDate, SqlConnection currConn, SqlTransaction transaction, bool isPost)
        {
            #region Initializ

            string sqlText = "";
            DataTable retResults = new DataTable();

            #endregion
            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }
                else if (Convert.ToDateTime(tranDate) < DateTime.MinValue || Convert.ToDateTime(tranDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");

                }
                #endregion Validation

                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                    transaction = currConn.BeginTransaction("AvgPprice");
                }

                #endregion open connection and transaction
                ProductDAL productDal = new ProductDAL();

                var AvgPriceVAT16 = productDal.AvgPriceVAT16(itemNo, tranDate, currConn, transaction);
                var AvgPriceVAT17 = productDal.AvgPriceVAT17(itemNo, tranDate, currConn, transaction);
                decimal Quantity = Convert.ToDecimal(AvgPriceVAT16.Rows[0]["Quantity"]);
                decimal Amount = Convert.ToDecimal(AvgPriceVAT16.Rows[0]["Amount"]);

                Quantity = Quantity + Convert.ToDecimal(AvgPriceVAT17.Rows[0]["Quantity"]);
                Amount = Amount + Convert.ToDecimal(AvgPriceVAT17.Rows[0]["Amount"]);
                retResults.Columns.Add("Quantity");
                retResults.Columns.Add("Amount");
                retResults.Rows.Add(new object[] { Quantity, Amount });

                CommonDAL _cDal = new CommonDAL();
                bool ImportCostingIncludeATV = false;
                ImportCostingIncludeATV = Convert.ToBoolean(_cDal.settings("Purchase", "ImportCostingIncludeATV") == "Y" ? true : false);

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo='" + itemNo + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }

                #endregion ProductExist

                #region AvgPrice

                sqlText = "  ";
                sqlText += "  SELECT SUM(isnull(SubTotal,0)) Amount,SUM(isnull(stock,0))Quantity" +
                           " FROM(   ";
                sqlText += "  (SELECT  isnull(OpeningBalance,0) Stock, isnull(p.OpeningTotalCost,0) SubTotal  FROM Products p  WHERE p.ItemNo = '" + itemNo + "')";

                sqlText += "  UNION ALL   ";
                sqlText += "  (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal   ";
                sqlText +=
                    "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('other','Service','ServiceNS','InputService','Trading','TollReceive','TollReceive-WIP','TollReceiveRaw','PurchaseCN')";
                if (!isPost)
                {
                    sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                if (ImportCostingIncludeATV)
                {
                    sqlText += @"   UNION ALL  ( 
                SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity, 
                isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0)SubTotal  
                FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('Import','InputServiceImport','ServiceImport','ServiceNSImport','TradingImport')  
                ";
                    if (!isPost)
                    {
                        sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
                    }
                    sqlText += " AND ItemNo = '" + itemNo + "') ";
                }
                else
                {
                    sqlText += @"    UNION ALL  ( 
               SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity, 
                isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0)SubTotal  
                FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in( 'InputServiceImport','ServiceImport','ServiceNSImport','TradingImport')  
                ";
                    if (!isPost)
                    {
                        sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
                    }
                    sqlText += " AND ItemNo = '" + itemNo + "') ";

                    sqlText += @"    UNION ALL  (  
                SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity, 
                isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+  isnull(OthersAmount,0)),0)),0)SubTotal  
                FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('Import' )  
                ";
                    if (!isPost)
                    {
                        sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
                    }
                    sqlText += " AND ItemNo = '" + itemNo + "') ";
                }




                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal   ";
                sqlText +=
                    "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('PurchaseReturn','PurchaseDN')";
                if (!isPost)
                {
                    sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal  " +
                           "  FROM IssueDetails WHERE Post='Y' ";
                sqlText +=
                    "  and TransactionType IN('PackageProduction','Other','TollFinishReceive','Tender','WIP','TollReceive','InputService','InputServiceImport','Trading','TradingTender','ExportTrading','ExportTradingTender','Service','ExportService','InternalIssue','TollIssue')";
                if (!isPost)
                {
                    sqlText += "  AND IssueDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal    " +
                           "FROM IssueDetails WHERE Post='Y' and TransactionType IN('IssueReturn','ReceiveReturn')";
                if (!isPost)
                {
                    sqlText += "  AND IssueDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL  ";
                //sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal   " +
                //           " FROM ReceiveDetails WHERE Post='Y' and TransactionType<>'ReceiveReturn' ";

                sqlText += " (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal " +
                                " FROM ReceiveDetails WHERE Post='Y' and TransactionType not in('ReceiveReturn','InternalIssue','Trading') "; //business is different for InternalIssue and Trading.
                if (!isPost)
                {
                    sqlText += "  AND ReceiveDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL ";
                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal    FROM ReceiveDetails WHERE Post='Y'";
                sqlText += " and TransactionType='ReceiveReturn' ";
                if (!isPost)
                {
                    sqlText += "  AND ReceiveDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL ";
                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleNewQuantity,-" +
                           "isnull(sum( SubTotal),0)SubTotal " +
                         "  FROM SalesInvoiceDetails ";
                //sqlText += "  WHERE Post='Y' " +
                //           " AND TransactionType in('Other','PackageSale','PackageProduction','Service','ServiceNS','Trading','TradingTender','Tender','Debit','TollFinishIssue','ServiceStock','InternalIssue')";

                sqlText += "  WHERE Post='Y' " +
                           " AND TransactionType in('Other','PackageSale','PackageProduction','Service','ServiceNS','TradingTender','Tender','Debit','TollFinishIssue','ServiceStock')";

                if (!isPost)
                {
                    sqlText += "  AND InvoiceDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleExpQuantity,-isnull(sum( CurrencyValue),0)SubTotal " +
                            "   FROM SalesInvoiceDetails ";
                sqlText += "  WHERE Post='Y' " +
                           "AND TransactionType in('Export','ExportService','ExportTrading','ExportTradingTender','ExportPackage','ExportTender') ";
                if (!isPost)
                {
                    sqlText += "  AND InvoiceDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT isnull(sum(  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleCreditQuantity,isnull(sum( SubTotal),0)SubTotal     FROM SalesInvoiceDetails ";
                sqlText += "  WHERE Post='Y'  AND TransactionType in( 'Credit') ";
                if (!isPost)
                {
                    sqlText += "  AND InvoiceDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                ////   New bussiness for transfer raw
                ////Transferred from item will be decrease stock
                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal     FROM TransferRawDetails  ";
                sqlText += "  WHERE Post='Y' ";
                if (!isPost)
                {
                    sqlText += "  AND TransferDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND TransFromItemNo = '" + itemNo + "') ";

                //----Transferred to item will be increase stock
                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal     FROM TransferRawDetails  ";
                sqlText += "  WHERE Post='Y' ";
                if (!isPost)
                {
                    sqlText += "  AND TransferDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";



                sqlText += "  UNION ALL  ";
                sqlText += "  (select -isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)Qty,";
                sqlText += "  isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)*isnull(sum(isnull(RealPrice,0)),0)";
                sqlText += "  from DisposeDetails  LEFT OUTER JOIN ";
                sqlText += "  DisposeHeaders sih ON DisposeDetails.DisposeNumber=sih.DisposeNumber  where ItemNo='" + itemNo + "' ";
                sqlText += "  AND (DisposeDetails.Post ='Y')    ";
                if (!isPost)
                {
                    sqlText += "  AND DisposeDetails.DisposeDate<='" + tranDate + "' ";
                }
                sqlText += "  and sih.FromStock in ('Y'))  ";
                sqlText += "  ) AS a";


                DataTable dataTable = new DataTable("UsableItems");
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                SqlDataAdapter dataAdapt = new SqlDataAdapter(cmd);
                dataAdapt.Fill(dataTable);

                if (dataTable == null)
                {
                    throw new ArgumentNullException("GetLIFOPurchaseInformation", "No row found ");
                }
                retResults = dataTable;


                #endregion Stock

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

        public DataTable AvgPriceVAT17(string ItemNo, string StartDate, SqlConnection currConn, SqlTransaction transaction)
        {
            //Delete all #VAT_17_0 information. It is not necessary for calculation. 
            #region Variables

            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataSet = new DataTable("ReportVAT17");


            #endregion

            #region Try

            try
            {
                #region vat19 value

                string vExportInBDT = "";
                CommonDAL commonDal = new CommonDAL();
                vExportInBDT = commonDal.settings("VAT19", "ExportInBDT");

                #endregion vat19 value



                string IsExport = "No";

                if (vExportInBDT == "N")
                {
                    sqlText = "Select CASE WHEN pc.IsRaw = 'Export' THEN 'Yes' ELSE 'No' END AS IsExport ";
                    sqlText += "from ProductCategories pc join Products p on pc.CategoryID = p.CategoryID ";
                    sqlText += "where p.ItemNo = '" + ItemNo + "'";

                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                    object objItemNo = cmd.ExecuteScalar();
                    if (objItemNo == null)
                        IsExport = "No";
                    else
                        IsExport = objItemNo.ToString();
                }


                var top = "";
                sqlText = " ";


                #region SQL

                sqlText += @"
                
--DECLARE @StartDate DATETIME;
--DECLARE @EndDate DATETIME;
--DECLARE @post1 VARCHAR(2);
--DECLARE @post2 VARCHAR(2);
--DECLARE @ItemNo VARCHAR(20);

--DECLARE @IsExport VARCHAR(20);
--SET @IsExport ='No';

--SET @Itemno='24';
--SET @post1='Y';
--SET @post2='N';
--SET @StartDate='2014-04-01';
--SET @EndDate= '2014-04-27';

             
declare @Present DECIMAL(25, 9);
DECLARE @OpeningDate DATETIME;


CREATE TABLE #VAT_17(
SerialNo  varchar (2) NULL,	 ItemNo   varchar (200) NULL,
 StartDateTime   datetime  NULL,	 StartingQuantity   decimal (25, 9) NULL,
 StartingAmount   decimal (25, 9) NULL,	 CustomerID   varchar (200) NULL,
 SD   decimal (25, 9) NULL,	 VATRate   decimal (25, 9) NULL,	 Quantity   decimal (25, 9) NULL,
 UnitCost   decimal (25, 9) NULL,	 TransID   varchar (200) NULL,	 TransType   varchar (200) NULL,Remarks VARCHAR(200),CreatedDateTime   datetime  NULL)

CREATE TABLE #VATTemp_17(SerialNo  varchar (2) NULL,	 Dailydate   datetime  NULL,	 TransID   varchar (200) NULL,
 TransType   varchar (200) NULL,	 ItemNo   varchar (200) NULL,	 UnitCost   decimal (25, 9) NULL,
 Quantity   decimal (25, 9) NULL,	 VATRate   decimal (25, 9) NULL,	 SD   decimal (25, 9) NULL,Remarks VARCHAR(200),CreatedDateTime   datetime  NULL) 
 
 

------end Disposee--------

select @OpeningDate = p.OpeningDate from Products p
WHERE ItemNo=@ItemNo

IF(@OpeningDate<@StartDate)
set @OpeningDate=@StartDate


insert into #VATTemp_17(SerialNo,Dailydate,TransID,VATRate,SD,remarks,TransType,ItemNo,Quantity,UnitCost)
SELECT distinct 'A' SerialNo,@OpeningDate Dailydate,'0' TransID,0 VATRate,0 SD,'Opening' remarks,'Opening' TransType,a.ItemNo,
 SUM(a.Quantity)Quantity,sum(a.Amount)UnitCost
	FROM (
		 
(SELECT @itemNo ItemNo,isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) Quantity,
CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS Amount
 FROM ReceiveDetails WHERE Post='Y'  AND ReceiveDateTime< @StartDate   
  and TransactionType not IN('ReceiveReturn') AND ItemNo = @itemNo ) 
UNION ALL
(SELECT @itemNo ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,
-CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS SubTotal
FROM ReceiveDetails WHERE Post='Y'  AND ReceiveDateTime< @StartDate   
 and TransactionType IN('ReceiveReturn') AND ItemNo = @itemNo ) 
UNION ALL 
(SELECT  @itemNo ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleNewQuantity,
-CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS SubTotal
FROM SalesInvoiceDetails   WHERE Post='Y' AND InvoiceDateTime< @StartDate     
AND TransactionType in('Other','PackageSale','PackageProduction','Service','ServiceNS','Trading','TradingTender','Tender','Debit','TollFinishIssue','ServiceStock','InternalIssue') AND ItemNo = @itemNo )  
UNION ALL  
(SELECT  @itemNo ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleExpQuantity,
-CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(CurrencyValue,0)),0) END AS SubTotal
FROM SalesInvoiceDetails   WHERE Post='Y' AND InvoiceDateTime< @StartDate      
AND TransactionType in('Export','ExportService','ExportTrading','ExportTradingTender','ExportPackage','ExportTender') AND ItemNo = @itemNo )  
UNION ALL
(SELECT @itemNo ItemNo,isnull(sum(  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleCreditQuantity,
CASE WHEN @IsExport='Yes' THEN isnull(sum(isnull(DollerValue,0)),0) ELSE isnull(sum(isnull(SubTotal,0)),0) END AS SubTotal
FROM SalesInvoiceDetails   WHERE Post='Y' AND InvoiceDateTime< @StartDate    
 AND TransactionType in( 'Credit') AND ItemNo = @itemNo )
   

) AS a GROUP BY a.ItemNo

insert into #VAT_17(SerialNo,ItemNo,StartDateTime,StartingQuantity,StartingAmount,
CustomerID,Quantity,UnitCost,TransID,TransType,VATRate,SD,remarks,CreatedDateTime)
select SerialNo,ItemNo,Dailydate,0,0,0,Quantity,UnitCost,TransID,TransType,VATRate,SD,remarks,CreatedDateTime
from #VATTemp_17
order by dailydate,SerialNo;

update #VAT_17 set 
CustomerID=SalesInvoiceHeaders.CustomerID
from SalesInvoiceHeaders
where SalesInvoiceHeaders.SalesInvoiceNo=#VAT_17.TransID 
and #VAT_17.TransType='Sale'
AND (SalesInvoiceHeaders.Post =@post1 or SalesInvoiceHeaders.Post= @post2)

select  
  #VAT_17.Quantity, #vat_17.UnitCost Amount
from #VAT_17  

DROP TABLE #VAT_17
DROP TABLE #VATTemp_17

                ";

                #endregion SQL



                top = "A";




                #region SQL Command

                SqlCommand objCommVAT17 = new SqlCommand();

                objCommVAT17.Connection = currConn;
                objCommVAT17.Transaction = transaction;

                objCommVAT17.CommandText = sqlText;
                objCommVAT17.CommandType = CommandType.Text;

                #endregion

                #region Parameter


                if (!objCommVAT17.Parameters.Contains("@IsExport"))
                {
                    objCommVAT17.Parameters.AddWithValue("@IsExport", IsExport);
                }
                else
                {
                    objCommVAT17.Parameters["@IsExport"].Value = IsExport;
                }

                if (!objCommVAT17.Parameters.Contains("@ItemNo"))
                {
                    objCommVAT17.Parameters.AddWithValue("@ItemNo", ItemNo);
                }
                else
                {
                    objCommVAT17.Parameters["@ItemNo"].Value = ItemNo;
                }

                if (!objCommVAT17.Parameters.Contains("@StartDate"))
                {
                    objCommVAT17.Parameters.AddWithValue("@StartDate", StartDate);
                    objCommVAT17.Parameters.AddWithValue("@EndDate", StartDate);
                }
                else
                {
                    objCommVAT17.Parameters["@StartDate"].Value = StartDate;
                    objCommVAT17.Parameters["@EndDate"].Value = StartDate;
                }
                objCommVAT17.Parameters.AddWithValue("@post1", "Y");
                objCommVAT17.Parameters.AddWithValue("@post2", "Y");




                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT17);
                dataAdapter.Fill(dataSet);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                //if (currConn.State == ConnectionState.Open)
                //{
                //    currConn.Close();
                //}

            }

            #endregion

            return dataSet;
        }

        public DataTable AvgPriceVAT16(string ItemNo, string StartDate, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Variables

            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataSet = new DataTable("ReportVAT16");

            #endregion

            #region Try

            try
            {

                string top;

                CommonDAL _cDal = new CommonDAL();
                bool ImportCostingIncludeATV = false;
                ImportCostingIncludeATV = Convert.ToBoolean(_cDal.settings("Purchase", "ImportCostingIncludeATV") == "Y" ? true : false);

                #region SQL Statement

                sqlText = "";

                #region Backup

                sqlText += @"
                          
	--DECLARE @StartDate DATETIME;
	--DECLARE @EndDate DATETIME;
	--DECLARE @post1 VARCHAR(200);
	--DECLARE @post2 VARCHAR(200);
	--DECLARE @ItemNo VARCHAR(200);
    
	--SET @Itemno='46';
	--SET @post1='Y';
	--SET @post2='N';
	--SET @StartDate='2014-04-01';
	--SET @EndDate='2014-04-27';

declare @Present DECIMAL(25, 9);
DECLARE @OpeningDate DATETIME;

CREATE TABLE #VAT_16(	SerialNo [varchar] (2) NULL,
[ItemNo] [varchar](200) NULL,	[StartDateTime] [datetime] NULL,
[StartingQuantity] [decimal](25, 9) NULL,	[StartingAmount] [decimal](25, 9) NULL,
[VendorID] [varchar](200) NULL,	[SD] [decimal](25, 9) NULL,	[VATRate] [decimal](25, 9) NULL,
[Quantity] [decimal](25, 9) NULL,	[UnitCost] [decimal](25, 9) NULL,	[TransID] [varchar](200) NULL,
[TransType] [varchar](200) NULL,[BENumber] [varchar](200) NULL,[InvoiceDateTime] [datetime] NULL,[Remarks] [varchar](200) NULL,[CreateDateTime] [datetime] NULL)

CREATE TABLE #VATTemp_16([SerialNo] [varchar] (2) NULL,[Dailydate] [datetime] NULL,[InvoiceDateTime] [datetime] NULL,
[TransID] [varchar](200) NULL,	[TransType] [varchar](200) NULL,[BENumber] [varchar](200) NULL,
[ItemNo] [varchar](200) NULL,	[UnitCost] [decimal](25, 9) NULL,
[Quantity] [decimal](25, 9) NULL,	[VATRate] [decimal](25, 9) NULL,	[SD] [decimal](25, 9) NULL,[Remarks] [varchar](200) NULL,[CreateDateTime] [datetime] NULL) 

---- start purchase---
 
 

select @OpeningDate = p.OpeningDate from Products p
WHERE ItemNo=@ItemNo

IF(@OpeningDate<@StartDate)
set @OpeningDate=@StartDate

insert into #VATTemp_16(SerialNo,Dailydate,TransID,VATRate,SD,Remarks,TransType,ItemNo,Quantity,UnitCost,InvoiceDateTime,BENumber)
 		    
SELECT distinct 'A' SerialNo,@OpeningDate Dailydate,'0' TransID,0 VATRate,0 SD,'Opening' remarks,'Opening' TransType,a.ItemNo, SUM(a.Quantity)Quantity,sum(a.Amount)UnitCost,@OpeningDate InvoiceDateTime,'-' BENumber
	FROM (
				SELECT @itemNo ItemNo, isnull(OpeningBalance,0) Quantity, isnull(p.OpeningTotalCost,0) Amount  
FROM Products p  WHERE p.ItemNo = @itemNo 
UNION ALL (
		SELECT @itemNo ItemNo, isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal 
FROM ReceiveDetails WHERE Post='Y' 
and TransactionType in('WIP') 
AND ReceiveDateTime < @StartDate      AND ItemNo = @itemNo
 )   
UNION ALL (
		SELECT @itemNo ItemNo, isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal 
FROM PurchaseInvoiceDetails WHERE Post='Y' 
and TransactionType in('other','Service','ServiceNS','InputService','Trading', 'TollReceive-WIP','PurchaseCN') 

AND ReceiveDate < @StartDate      AND ItemNo = @itemNo
 )  	 UNION ALL (
	SELECT @itemNo ItemNo, isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,
	isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0)SubTotal 
FROM PurchaseInvoiceDetails WHERE Post='Y' 
and TransactionType in('Import','InputServiceImport','ServiceImport','ServiceNSImport','TradingImport') 
AND ReceiveDate < @StartDate      AND ItemNo = @itemNo
 ) 	 UNION ALL 
(	SELECT  @itemNo ItemNo,-isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,
-isnull(sum(isnull(SubTotal,0)),0)SubTotal     FROM PurchaseInvoiceDetails WHERE Post='Y' 
and TransactionType in('PurchaseReturn','PurchaseDN')  AND ReceiveDate< @StartDate     AND ItemNo = @itemNo ) 
 
 --Transfer to Raw
 UNION ALL (
	SELECT @itemNo ItemNo,isnull(sum(UOMQty),0) TransferQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal
FROM TransferRawDetails WHERE Post='Y'   AND TransferDateTime< @StartDate  
   AND ItemNo = @itemNo  AND (UOMQty>0)   
 ) 

UNION ALL 
(SELECT @itemNo ItemNo,-isnull(sum(UOMQty),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)
FROM IssueDetails WHERE Post='Y'   AND IssueDateTime< @StartDate  
   and TransactionType IN('other','Receive','TollReceive','TollIssue')  AND ItemNo = @itemNo  AND (UOMQty>0))   
UNION ALL 
(SELECT @itemNo ItemNo,isnull(sum(UOMQty),0) IssueQuantity,isnull(sum(isnull(SubTotal,0)),0)
FROM SalesInvoiceDetails WHERE Post='Y'   AND InvoiceDateTime< @StartDate  
   and TransactionType IN('TollIssue ')  AND ItemNo = @itemNo  AND (UOMQty>0))   

UNION ALL 
(select @itemNo ItemNo,-isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)Qty, 
isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)*isnull(sum(isnull(RealPrice,0)),0)  
from DisposeDetails  LEFT OUTER JOIN   DisposeHeaders sih ON DisposeDetails.DisposeNumber=sih.DisposeNumber 
 where ItemNo=@itemNo   
AND DisposeDetails.DisposeDate< @StartDate      AND (DisposeDetails.Post ='Y')      and sih.FromStock in ('Y'))    

	
) AS a GROUP BY a.ItemNo


insert into #VAT_16(SerialNo,ItemNo,StartDateTime,InvoiceDateTime,StartingQuantity,StartingAmount,
VendorID,Quantity,UnitCost,TransID,TransType,VATRate,SD,BENumber,Remarks,CreateDateTime)
select SerialNo,@ItemNo,Dailydate,InvoiceDateTime,0,0,0,
Quantity,UnitCost,TransID,TransType,VATRate,SD,BENumber,Remarks,CreateDateTime
from #VATTemp_16
order by dailydate,SerialNo

update #VAT_16 set 
VendorID=PurchaseInvoiceHeaders.VendorID
from PurchaseInvoiceHeaders
where PurchaseInvoiceHeaders.PurchaseInvoiceNo=#VAT_16.TransID
and #VAT_16.TransType='Purchase'

update #VAT_16 set 
StartingQuantity=0,
StartingAmount=0
from PurchaseInvoiceHeaders
where PurchaseInvoiceHeaders.PurchaseInvoiceNo=#VAT_16.TransID 
and PurchaseInvoiceHeaders.TransactionType IN('ServiceNS')
AND (PurchaseInvoiceHeaders.Post =@post1 or PurchaseInvoiceHeaders.Post= @post2)
and #VAT_16.TransType='Purchase'

select  
 #VAT_16.Quantity ,#VAT_16.UnitCost  Amount
from #VAT_16  
  

DROP TABLE #VAT_16
DROP TABLE #VATTemp_16


                
                ";

                #endregion



                top = "Go";

                #endregion

                #region SQL Command

                SqlCommand objCommVAT16 = new SqlCommand();
                objCommVAT16.Connection = currConn;
                objCommVAT16.Transaction = transaction;

                objCommVAT16.CommandText = sqlText;
                objCommVAT16.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                //objCommVAT16.CommandText = sqlText;
                //objCommVAT16.CommandType = CommandType.Text;

                #region Parameter

                //objCommVAT16.CommandText = sqlText;
                //objCommVAT16.CommandType = CommandType.Text;

                if (!objCommVAT16.Parameters.Contains("@ItemNo"))
                {
                    objCommVAT16.Parameters.AddWithValue("@ItemNo", ItemNo);
                }
                else
                {
                    objCommVAT16.Parameters["@ItemNo"].Value = ItemNo;
                }


                if (StartDate == "")
                {
                    if (!objCommVAT16.Parameters.Contains("@StartDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@StartDate", System.DBNull.Value);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@StartDate"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommVAT16.Parameters.Contains("@StartDate"))
                    {
                        objCommVAT16.Parameters.AddWithValue("@StartDate", StartDate);
                        objCommVAT16.Parameters.AddWithValue("@EndDate", StartDate);
                    }
                    else
                    {
                        objCommVAT16.Parameters["@StartDate"].Value = StartDate;
                        objCommVAT16.Parameters["@EndDate"].Value = StartDate;
                    }

                } // Common Filed

                #endregion Parameter

                objCommVAT16.Parameters.AddWithValue("@post1", "Y");
                objCommVAT16.Parameters.AddWithValue("@post2", "Y");



                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT16);
                dataAdapter.Fill(dataSet);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
            }


            #endregion

            return dataSet;
        }

        public DataTable AvgPriceForInternalSales(string itemNo, string tranDate, SqlConnection currConn, SqlTransaction transaction, bool isPost)
        {
            // from Internal Sales,Trading, Service-Stock
            #region Initializ

            string sqlText = "";
            DataTable retResults = new DataTable();

            #endregion
            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }
                else if (Convert.ToDateTime(tranDate) < DateTime.MinValue || Convert.ToDateTime(tranDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");

                }
                #endregion Validation

                #region open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                    transaction = currConn.BeginTransaction("AvgPprice");
                }

                #endregion open connection and transaction
                CommonDAL _cDal = new CommonDAL();
                bool ImportCostingIncludeATV = false;
                ImportCostingIncludeATV = Convert.ToBoolean(_cDal.settings("Purchase", "ImportCostingIncludeATV") == "Y" ? true : false);


                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo='" + itemNo + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;

                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }

                #endregion ProductExist

                #region AvgPrice

                sqlText = "  ";
                sqlText += "  SELECT SUM(isnull(SubTotal,0)) Amount,SUM(isnull(stock,0))Quantity" +
                           " FROM(   ";
                sqlText += "  (SELECT  isnull(OpeningBalance,0) Stock, isnull(p.OpeningTotalCost,0) SubTotal  FROM Products p  WHERE p.ItemNo = '" + itemNo + "')";

                sqlText += "  UNION ALL   ";
                sqlText += "  (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal   ";
                sqlText +=
                    "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('other','Service','ServiceNS','InputService','Trading','TollReceive','TollReceive-WIP','TollReceiveRaw','PurchaseCN')";
                if (!isPost)
                {
                    sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                if (ImportCostingIncludeATV)
                {
                    sqlText += "  UNION ALL   ";
                    sqlText += "  (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity," +
                               "isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0)SubTotal   ";
                    sqlText +=
                        "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('Import','InputServiceImport','ServiceImport','ServiceNSImport','TradingImport') ";

                }
                else
                {
                    sqlText += @"  
                    UNION ALL  
                    (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity, 
                    isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(ATVAmount,0)+isnull(OthersAmount,0)),0)),0)SubTotal 
                    FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('InputServiceImport','ServiceImport','ServiceNSImport','TradingImport') 
                    ";
                    sqlText += @"  
                    UNION ALL  
                    (SELECT  isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity, 
                    isnull(sum(isnull((isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)+ isnull(OthersAmount,0)),0)),0)SubTotal 
                    FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('Import') 
                    ";
                }

                if (!isPost)
                {
                    sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0)PurchaseQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal   ";
                sqlText +=
                    "  FROM PurchaseInvoiceDetails WHERE Post='Y' and TransactionType in('PurchaseReturn','PurchaseDN')";
                if (!isPost)
                {
                    sqlText += "  AND ReceiveDate<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal  " +
                           "  FROM IssueDetails WHERE Post='Y' ";
                sqlText +=
                    "  and TransactionType IN('PackageProduction','Other','TollFinishReceive','Tender','WIP','TollReceive','InputService','InputServiceImport','Trading','TradingTender','ExportTrading','ExportTradingTender','Service','ExportService','InternalIssue','TollIssue')";
                if (!isPost)
                {
                    sqlText += "  AND IssueDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) IssueQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal    " +
                           "FROM IssueDetails WHERE Post='Y' and TransactionType IN('IssueReturn','ReceiveReturn')";
                if (!isPost)
                {
                    sqlText += "  AND IssueDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL  ";
                //sqlText += "  (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal   " +
                //           " FROM ReceiveDetails WHERE Post='Y' and TransactionType<>'ReceiveReturn' ";

                sqlText += " (SELECT isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,isnull(sum(isnull(SubTotal,0)),0)SubTotal " +
                                " FROM ReceiveDetails WHERE Post='Y' and TransactionType not in('ReceiveReturn','InternalIssue','Trading') "; //business is different for InternalIssue and Trading.
                if (!isPost)
                {
                    sqlText += "  AND ReceiveDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL ";
                sqlText += "  (SELECT -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) ReceiveQuantity,-isnull(sum(isnull(SubTotal,0)),0)SubTotal    FROM ReceiveDetails WHERE Post='Y'";
                sqlText += " and TransactionType='ReceiveReturn' ";
                if (!isPost)
                {
                    sqlText += "  AND ReceiveDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL ";
                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleNewQuantity,-" +
                           "isnull(sum( SubTotal),0)SubTotal " +
                         "  FROM SalesInvoiceDetails ";
                //sqlText += "  WHERE Post='Y' " +
                //           " AND TransactionType in('Other','PackageSale','PackageProduction','Service','ServiceNS','Trading','TradingTender','Tender','Debit','TollFinishIssue','ServiceStock','InternalIssue')";

                sqlText += "  WHERE Post='Y' " +
                           " AND TransactionType in('Other','PackageSale','PackageProduction','Service','ServiceNS','TradingTender','Tender','Debit','TollFinishIssue','ServiceStock')";

                if (!isPost)
                {
                    sqlText += "  AND InvoiceDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT  -isnull(sum(isnull(UOMQty,isnull(Quantity,0))),0) SaleExpQuantity,-isnull(sum( CurrencyValue),0)SubTotal " +
                            "   FROM SalesInvoiceDetails ";
                sqlText += "  WHERE Post='Y' " +
                           "AND TransactionType in('Export','ExportService','ExportTrading','ExportTradingTender','ExportPackage','ExportTender') ";
                if (!isPost)
                {
                    sqlText += "  AND InvoiceDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (SELECT isnull(sum(  case when isnull(ValueOnly,'N')='Y' then 0 else  UOMQty end ),0) SaleCreditQuantity,isnull(sum( SubTotal),0)SubTotal     FROM SalesInvoiceDetails ";
                sqlText += "  WHERE Post='Y'  AND TransactionType in( 'Credit') ";
                if (!isPost)
                {
                    sqlText += "  AND InvoiceDateTime<='" + tranDate + "' ";
                }
                sqlText += " AND ItemNo = '" + itemNo + "') ";

                sqlText += "  UNION ALL  ";
                sqlText += "  (select -isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)Qty,";
                sqlText += "  isnull(sum(isnull(Quantity,0)+isnull(QuantityImport,0)),0)*isnull(sum(isnull(RealPrice,0)),0)";
                sqlText += "  from DisposeDetails  LEFT OUTER JOIN ";
                sqlText += "  DisposeHeaders sih ON DisposeDetails.DisposeNumber=sih.DisposeNumber  where ItemNo='" + itemNo + "' ";
                sqlText += "  AND (DisposeDetails.Post ='Y')    ";
                if (!isPost)
                {
                    sqlText += "  AND DisposeDetails.DisposeDate<='" + tranDate + "' ";
                }
                sqlText += "  and sih.FromStock in ('Y'))  ";
                sqlText += "  ) AS a";


                DataTable dataTable = new DataTable("UsableItems");
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                SqlDataAdapter dataAdapt = new SqlDataAdapter(cmd);
                dataAdapt.Fill(dataTable);

                if (dataTable == null)
                {
                    throw new ArgumentNullException("GetLIFOPurchaseInformation", "No row found ");
                }
                retResults = dataTable;


                #endregion Stock

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

        public decimal PurchasePrice(string itemNo, String PurchaseNo, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }

                #endregion Validation

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

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo='" + itemNo + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("IssuePrice", "There is No data to find Issue Price");
                }

                #endregion ProductExist

                #region AvgPrice

                sqlText = "  ";
                sqlText += "  SELECT ";
                sqlText += "  isnull(AssessableValue,0)+ isnull(CDAmount,0)+isnull(OthersAmount,0)" +
                           "+ isnull(RDAmount,0)+ isnull(TVBAmount,0)";
                sqlText += "   FROM PurchaseInvoiceDetails id WHERE id.PurchaseInvoiceNo='" + PurchaseNo + "'";
                sqlText += "  and itemno='" + itemNo + "' ";


                SqlCommand cmdIssuePrice = new SqlCommand(sqlText, currConn);
                cmdIssuePrice.Transaction = transaction;
                if (cmdIssuePrice.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmdIssuePrice.ExecuteScalar();
                    //object objDel = cmdDelete.ExecuteScalar();

                }

                #endregion Stock

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

        public string GetProductUOMc(string uomFrom, string uomTo)
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
                #region Validation
                if (string.IsNullOrEmpty(uomFrom))
                {
                    return retResults = string.Empty;

                    //throw new ArgumentNullException("GetProductUOMc", "Invalid UOM From");
                }
                if (string.IsNullOrEmpty(uomTo))
                {

                    return retResults = string.Empty;
                    //throw new ArgumentNullException("GetProductUOMc", "Invalid UOM To");
                }

                #endregion Validation



                #region Find UOM From

                sqlText = " ";
                sqlText = " SELECT top 1 u.UOMId ";
                sqlText += " from UOMs u";
                sqlText += " where ";
                sqlText += " u.UOMFrom='" + uomFrom + "' ";

                SqlCommand cmdUOMFrom = new SqlCommand(sqlText, currConn);
                object objUOMFrom = cmdUOMFrom.ExecuteScalar();
                if (objUOMFrom == null)
                    throw new ArgumentNullException("GetProductNo", "No UOM  '" + uomFrom + "' from found in conversion");

                #endregion Find UOM From

                #region Find UOM to

                sqlText = " ";
                sqlText = " SELECT top 1 u.UOMId ";
                sqlText += " from UOMs u";
                sqlText += " where ";
                sqlText += " u.UOMTo='" + uomTo + "' ";

                SqlCommand cmdUOMTo = new SqlCommand(sqlText, currConn);
                object objUOMTo = cmdUOMTo.ExecuteScalar();
                if (objUOMTo == null)
                    throw new ArgumentNullException("GetProductNo", "No UOM  '" + uomTo + "' to found in conversion");

                #endregion Find UOM to

                #region Stock

                sqlText = "  ";

                sqlText = " SELECT top 1 u.Convertion FROM UOMs u ";
                sqlText += " where ";
                sqlText += " u.UOMFrom='" + uomFrom + "' ";
                sqlText += " and u.UOMTo='" + uomTo + "' ";

                //select prd.ItemNo, prd.productCode,prd.ProductName,prt.ProductType from Products prd
                //inner join ProductTypes prt on prt.TypeId=prd.CategoryId
                //where producttype='finish' 


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (objItemNo == null)
                    throw new ArgumentNullException("GetProductNo", "No conversion found from ='" + uomFrom + "'" +
                                                                    " to '" + uomTo + "'");

                retResults = objItemNo.ToString();
                return retResults;



                #endregion Stock

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
        public string GetProductUOMn(string ProductName, string ProductGroup)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(ProductName))
                {
                    throw new ArgumentNullException("GetProductNo", "Invalid product name");
                }
                if (string.IsNullOrEmpty(ProductGroup))
                {
                    throw new ArgumentNullException("GetProductNo", "Invalid product group");
                }

                #endregion Validation

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

                #region Stock

                sqlText = "  ";

                sqlText = " SELECT p.UOM ";
                sqlText += " FROM Products p LEFT OUTER JOIN ProductCategories pc ON p.CategoryID=pc.CategoryID";
                sqlText += " where ";
                sqlText += " ProductName='" + ProductName + "' ";
                sqlText += " and Israw='" + ProductGroup + "' ";

                //select prd.ItemNo, prd.productCode,prd.ProductName,prt.ProductType from Products prd
                //inner join ProductTypes prt on prt.TypeId=prd.CategoryId
                //where producttype='finish' 


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (objItemNo == null)
                    retResults = string.Empty;
                //    throw new ArgumentNullException("GetProductNo", "No product found with product Code ='" + ProductCode + "' and group ='" + ProductGroup + "'");
                else
                    retResults = objItemNo.ToString();
                return retResults;



                #endregion Stock

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
        public string GetProductCodeUOMn(string ProductCode, string ProductGroup)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation


                #endregion Validation

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

                #region Stock

                sqlText = "  ";

                sqlText = " SELECT p.UOM ";
                sqlText += " FROM Products p LEFT OUTER JOIN ProductCategories pc ON p.CategoryID=pc.CategoryID";
                sqlText += " where ";
                sqlText += " ProductCode='" + ProductCode + "' ";
                sqlText += " and Israw='" + ProductGroup + "' ";

                //select prd.ItemNo, prd.productCode,prd.ProductName,prt.ProductType from Products prd
                //inner join ProductTypes prt on prt.TypeId=prd.CategoryId
                //where producttype='finish' 


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                //if (objItemNo == null)
                //    throw new ArgumentNullException("GetProductNo", "No product found with product name ='" + ProductName + "' and group ='" + ProductGroup + "'");
                if (objItemNo == null)
                    retResults = string.Empty;
                //    throw new ArgumentNullException("GetProductNo", "No product found with product Code ='" + ProductCode + "' and group ='" + ProductGroup + "'");
                else
                    retResults = objItemNo.ToString();
                return retResults;



                #endregion Stock

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
        public string GetProductNoByGroup(string ProductName, string ProductGroup)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(ProductName))
                //{
                //    throw new ArgumentNullException("GetProductNo", "Invalid product name");
                //}
                //if (string.IsNullOrEmpty(ProductGroup))
                //{
                //    throw new ArgumentNullException("GetProductNo", "Invalid product group");
                //}

                #endregion Validation

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


                #region Stock

                sqlText = "  ";

                sqlText = " select prd.ItemNo from Products prd ";
                sqlText += " inner join ProductCategories cat on cat.CategoryId=prd.CategoryId ";
                sqlText += " where ";
                sqlText += " ProductName='" + ProductName + "' ";
                sqlText += " and Israw='" + ProductGroup + "' ";

                //select prd.ItemNo, prd.productCode,prd.ProductName,prt.ProductType from Products prd
                //inner join ProductTypes prt on prt.TypeId=prd.CategoryId
                //where producttype='finish' 


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                //if (objItemNo == null)
                //    throw new ArgumentNullException("GetProductNo", "No product found with product name ='" + ProductName + "' and group ='" + ProductGroup+"'");
                if (objItemNo == null)
                    retResults = string.Empty;
                //    throw new ArgumentNullException("GetProductNo", "No product found with product Code ='" + ProductCode + "' and group ='" + ProductGroup + "'");
                else
                    retResults = objItemNo.ToString();
                return retResults;



                #endregion Stock

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

        public string GetProductIdByName(string ProductName)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation


                #endregion Validation

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


                #region Stock

                sqlText = "  ";

                sqlText = " select ItemNo from Products ";
                sqlText += " where 1=1 ";
                sqlText += " and ProductName='" + ProductName + "' ";

                //select prd.ItemNo, prd.productCode,prd.ProductName,prt.ProductType from Products prd
                //inner join ProductTypes prt on prt.TypeId=prd.CategoryId
                //where producttype='finish' 


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                //if (objItemNo == null)
                //    throw new ArgumentNullException("GetProductNo", "No product found with product name ='" + ProductName + "' and group ='" + ProductGroup+"'");
                if (objItemNo == null)
                    retResults = string.Empty;
                //    throw new ArgumentNullException("GetProductNo", "No product found with product Code ='" + ProductCode + "' and group ='" + ProductGroup + "'");
                else
                    retResults = objItemNo.ToString();
                return retResults;



                #endregion Stock

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

        public string GetProductNoByGroup_Code(string ProductCode, string ProductGroup)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(ProductCode))
                //{
                //    throw new ArgumentNullException("GetProductNo", "Invalid product Code");
                //}
                //if (string.IsNullOrEmpty(ProductGroup))
                //{
                //    throw new ArgumentNullException("GetProductNo", "Invalid product group");
                //}

                #endregion Validation

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


                #region Stock

                sqlText = "  ";

                sqlText = " select prd.ItemNo from Products prd ";
                sqlText += " inner join ProductCategories cat on cat.CategoryId=prd.CategoryId ";
                sqlText += " where ";
                sqlText += " ProductCode='" + ProductCode + "' ";
                sqlText += " and Israw='" + ProductGroup + "' ";

                //select prd.ItemNo, prd.productCode,prd.ProductName,prt.ProductType from Products prd
                //inner join ProductTypes prt on prt.TypeId=prd.CategoryId
                //where producttype='finish' 


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (objItemNo == null)
                    retResults = string.Empty;
                //    throw new ArgumentNullException("GetProductNo", "No product found with product Code ='" + ProductCode + "' and group ='" + ProductGroup + "'");
                else
                    retResults = objItemNo.ToString();
                return retResults;



                #endregion Stock

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

        public DataTable GetProductCodeAndNameByItemNo(string ItemNo)
        {
            #region Initializ

            DataTable retResults = new DataTable();
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(ItemNo))
                {
                    throw new ArgumentNullException("GetProductCodeAndNameByItemNo", "Invalid product name");
                }


                #endregion Validation

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


                #region Stock

                sqlText = "  ";

                sqlText = " select ProductCode,ProductName from Products";
                sqlText += " where ";
                sqlText += " ItemNo='" + ItemNo + "' ";

                DataTable dataTable = new DataTable("RIFB");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                //cmdRIFB.Transaction = transaction;
                SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdRIFB);
                reportDataAdapt.Fill(dataTable);

                if (dataTable == null)
                {
                    throw new ArgumentNullException("GetProductCodeAndNameByItemNo", "No product found ");
                }


                retResults = dataTable;
                return retResults;



                #endregion Stock

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
        public string GetProductTypeByItemNo(string ProductNo, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
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
                        transaction = currConn.BeginTransaction("Product Type");
                    }
                }

                #endregion open connection and transaction
                #region ProductTYpe

                sqlText = "  ";
                sqlText += " SELECT DISTINCT IsRaw";
                sqlText += " FROM Products p LEFT OUTER JOIN";
                sqlText += " productCategories pc ON p.CategoryID=pc.CategoryID ";

                sqlText += "  where p.ItemNo='" + ProductNo + "' ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (objItemNo == null)
                    retResults = string.Empty;
                else
                    retResults = objItemNo.ToString();
                return retResults;



                #endregion ProductTYpe

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
        public DataTable SearchProductMiniDSDispose(string purchaseNumber)
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("DisposeItem");
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
                            select PD.itemno,
isnull(Products1.ProductName,'N/A')ProductName,
isnull(Products1.ProductCode,'N/A')ProductCode,
Products1.CategoryID, 
isnull(Products1.CategoryName,'N/A')CategoryName ,
isnull(PD.uom,'N/A')UOM,
isnull(Products1.HSCodeNo,'N/A')HSCodeNo,
isnull(Products1.IsRaw,'N/A')IsRaw,
isnull(Products1.CostPrice,0)CostPrice,
isnull(Products1.OpeningBalance,0)OpeningBalance, 
isnull(Products1.SalesPrice,0)SalesPrice,
isnull(Products1.NBRPrice,0)NBRPrice,
isnull( Products1.ReceivePrice,0)ReceivePrice,
isnull( PD.costprice,0)IssuePrice,
isnull(Products1.Packetprice,0)Packetprice, 
isnull(Products1.TenderPrice,0)TenderPrice, 
isnull(Products1.ExportPrice,0)ExportPrice, 
isnull(Products1.InternalIssuePrice,0)InternalIssuePrice, 
isnull(Products1.TollIssuePrice,0)TollIssuePrice, 
isnull(Products1.TollCharge,0)TollCharge, 
isnull(PD.vatrate,0)VATRate,
isnull(Products1.SD,0)SD,
isnull(Products1.TradingMarkUp,0)TradingMarkUp,
isnull(PD.UOMQty,PD.quantity)as PurchaseQty,
isnull(Products1.Stock,0) as Stock,

isnull(Products1.QuantityInHand,0)QuantityInHand,
isnull(Products1.Trading,'N')Trading, 
isnull(Products1.NonStock,'N')NonStock
from PurchaseInvoiceDetails PD left outer join
(SELECT   
Products.ItemNo,
isnull(Products.ProductName,'N/A')ProductName,
isnull(Products.ProductCode,'N/A')ProductCode,
Products.CategoryID, 
isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
isnull(Products.HSCodeNo,'N/A')HSCodeNo,
isnull(ProductCategories.IsRaw,'N/A')IsRaw,
isnull(Products.CostPrice,0)CostPrice,
isnull(Products.OpeningBalance,0)OpeningBalance, 
isnull(Products.SalesPrice,0)SalesPrice,
isnull(Products.NBRPrice,0)NBRPrice,
isnull( Products.ReceivePrice,0)ReceivePrice,
isnull(Products.Packetprice,0)Packetprice, 
isnull(Products.TenderPrice,0)TenderPrice, 
isnull(Products.ExportPrice,0)ExportPrice, 
isnull(Products.InternalIssuePrice,0)InternalIssuePrice, 
isnull(Products.TollIssuePrice,0)TollIssuePrice, 
isnull(Products.TollCharge,0)TollCharge, 
isnull(Products.vatrate,0)VATRate,
isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,

isnull(Products.SD,0)SD,
isnull(Products.TradingMarkUp,0)TradingMarkUp,
isnull(Products.QuantityInHand,0)QuantityInHand,
isnull(Products.Trading,'N')Trading, 
isnull(Products.NonStock,'N')NonStock
FROM         Products LEFT OUTER JOIN
ProductCategories ON Products.CategoryID = ProductCategories.CategoryID 
) Products1 on pd.itemno=Products1.itemno

 WHERE 
    (pd.PurchaseInvoiceNo = @purchaseNumber) ";

                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;

                if (!objCommProductType.Parameters.Contains("@purchaseNumber"))
                { objCommProductType.Parameters.AddWithValue("@purchaseNumber", purchaseNumber); }
                else { objCommProductType.Parameters["@purchaseNumber"].Value = purchaseNumber; }

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

        public DataTable SearchProductbySaleInvoice(string SaleInvoiceNo)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
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

SELECT s.[SalesInvoiceNo]      
      ,s.[ItemNo]
      ,s.[Quantity]   
      ,s.[UOM]   
      ,s.[SubTotal]
      ,p.[ProductCode]
      ,p.[ProductName]      
  FROM SalesInvoiceDetails s,
Products p
Where s.SalesInvoiceNo =@SaleInvoiceNo
and s.[ItemNo]=p.[ItemNo]

";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;



                if (!objCommProduct.Parameters.Contains("@SaleInvoiceNo"))
                { objCommProduct.Parameters.AddWithValue("@SaleInvoiceNo", SaleInvoiceNo); }
                else { objCommProduct.Parameters["@SaleInvoiceNo"].Value = SaleInvoiceNo; }


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

        public DataTable SearchOverheadForBOMNew(string ActiveStatus)
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("CompanyOverheads");

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

                sqlText = @"select Productname Headname,RebatePercent RebatePercent,ProductCode OHCode,ItemNo HeadID   
from  Products p LEFT OUTER JOIN
ProductCategories pc ON p.CategoryID=pc.CategoryID 
 WHERE (pc.IsRaw='Overhead') 
and p.ActiveStatus=@ActiveStatus 
                          order by Productname";

                SqlCommand objCommOverhead = new SqlCommand();
                objCommOverhead.Connection = currConn;
                objCommOverhead.CommandText = sqlText;
                objCommOverhead.CommandType = CommandType.Text;

                #region param

                if (!objCommOverhead.Parameters.Contains("@ActiveStatus"))
                { objCommOverhead.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommOverhead.Parameters["@ActiveStatus"].Value = ActiveStatus; }

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommOverhead);
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
        public DataTable SearchChassis()
        {
            #region Objects & Variables

            //string Description = "";
            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("Chassis");
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
                        select Heading1 from Trackings
 where issale='Y'
                            order by Heading1
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dataTable);

                // Common Filed
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
        public DataTable SearchEngine()
        {
            #region Objects & Variables

            //string Description = "";
            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("Engine");
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
                        select Heading2 from Trackings
 where issale='Y'
                            order by Heading2
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dataTable);

                // Common Filed
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

        public string[] productType = new string[]
                                                {
                                                    "Overhead",//0
                                                    "Raw",//1
                                                    "Pack",//2
                                                    "Finish",//3
                                                    "Service",//4
                                                    "Trading",//5
                                                    "WIP",//6
                                                    "Export"//7
                                                };

        public IList<string> ProductTypeList
        {
            get
            {
                return productType.ToList<string>();

            }
        }


        public DataTable SearchBanderolProducts()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("DT");

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


SELECT   
Products.ItemNo,
isnull(Products.ProductName,'N/A')ProductName,
isnull(Products.ProductCode,'N/A')ProductCode,
Products.CategoryID, 
isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
isnull(Products.UOM,'N/A')UOM,
isnull(Products.HSCodeNo,'N/A')HSCodeNo,
isnull(ProductCategories.IsRaw,'N/A')IsRaw,
isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
isnull(Products.CostPrice,0)CostPrice,
isnull(Products.OpeningBalance,0)OpeningBalance, 
isnull(Products.SalesPrice,0)SalesPrice,
isnull(Products.NBRPrice,0)NBRPrice,
isnull( Products.ReceivePrice,0)ReceivePrice,
isnull( Products.IssuePrice,0)IssuePrice,
isnull(Products.Packetprice,0)Packetprice, 
isnull(Products.TenderPrice,0)TenderPrice, 
isnull(Products.ExportPrice,0)ExportPrice, 
isnull(Products.InternalIssuePrice,0)InternalIssuePrice, 
isnull(Products.TollIssuePrice,0)TollIssuePrice, 
isnull(Products.TollCharge,0)TollCharge, 
isnull(Products.VATRate,0)VATRate,
isnull(Products.SD,0)SD,
isnull(Products.TradingMarkUp,0)TradingMarkUp,
isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
isnull(Products.QuantityInHand,0)QuantityInHand,
isnull(Products.Trading,'N')Trading, 
isnull(Products.NonStock,'N')NonStock,
isnull(Products.RebatePercent,0)RebatePercent

FROM Products LEFT OUTER JOIN
ProductCategories ON Products.CategoryID = ProductCategories.CategoryID 
Where Products.Banderol='Y'  ";


                //sqlText += "  WHERE (Products.ItemNo LIKE '%' +'" + ItemNo + "'+ '%'  OR Products.ItemNo IS NULL) AND";
                //sqlText += " (Products.CategoryID  LIKE '%'  +'" + CategoryID + "'+  '%' OR Products.CategoryID IS NULL) ";
                //sqlText += " AND (ProductCategories.IsRaw LIKE '%'+'" + IsRaw + "'+  '%' OR ProductCategories.IsRaw IS NULL)  ";
                //sqlText += " AND (ProductCategories.CategoryName LIKE '%'+'" + CategoryName + "'+  '%' OR ProductCategories.CategoryName IS NULL)  ";
                //sqlText += " AND (Products.ActiveStatus LIKE '%'+'" + ActiveStatus + "'+  '%' OR Products.ActiveStatus IS NULL)";
                //sqlText += " AND (Products.Trading LIKE '%' +'" + Trading + "'+  '%' OR Products.Trading IS NULL)";
                //sqlText += " AND (Products.NonStock LIKE '%' +'" + NonStock + "'+  '%' OR Products.NonStock IS NULL)";
                //sqlText += " AND (Products.ProductCode LIKE '%'+'" + ProductCode + "'+  '%' OR Products.ProductCode IS NULL)";
                sqlText += " order by Products.ItemNo ";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

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

        public string GetExistingProductName(string ProductName)
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
                #region Stock

                sqlText = "  ";
                sqlText += " SELECT DISTINCT ProductName";
                sqlText += " FROM Products";
                sqlText += "  where ProductName='" + ProductName + "' ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (objItemNo == null)
                    retResults = string.Empty;
                else
                    retResults = objItemNo.ToString();

                #endregion Stock

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

        public string TrackingStockCheck(string ItemNo)
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
                #region Stock

                sqlText = "  ";
                sqlText += @" select COUNT(id)Stock from PurchaseSaleTrackings
                                where IsSold=0";
                sqlText += "  and  ItemNo='" + ItemNo + "' ";


                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                object objItemNo = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                if (objItemNo == null)
                    retResults = string.Empty;
                else
                    retResults = objItemNo.ToString();

                #endregion Stock

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


        public DataTable SearchRawItemNo(string IssueNo)
        {
            // for TollReceive
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("CompanyOverheads");

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
                sqlText = "Select ItemNo,CostPrice from IssueDetails ";
                sqlText += " where IssueNo='" + IssueNo + "'";

                SqlCommand objCommOverhead = new SqlCommand(sqlText, currConn);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommOverhead);
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

        public DataTable SearchByItemNo(string ItemNo)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Product");

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

                sqlText = @"SELECT
                                    Products.ItemNo,
                                    isnull(Products.ProductName,'N/A')ProductName,
                                    isnull(Products.ProductDescription,'N/A')ProductDescription,
                                    Products.CategoryID, 
                                    isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
                                    isnull(Products.UOM,'N/A')UOM,
                                    isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
                                    isnull(Products.CostPrice,0)CostPrice,
                                    isnull(Products.SalesPrice,0)SalesPrice,
                                    isnull(Products.NBRPrice,0)NBRPrice,
                                    isnull(ProductCategories.IsRaw,'N')IsRaw,
                                    isnull(Products.SerialNo,'N/A')SerialNo ,
                                    isnull(Products.HSCodeNo,'N/A')HSCodeNo,
                                    isnull(Products.VATRate,0)VATRate,
                                    isnull(Products.ActiveStatus,'N')ActiveStatus,
                                    isnull(Products.OpeningBalance,0)OpeningBalance,
                                    isnull(Products.Comments,'N/A')Comments,
                                    'N/A' HSDescription, 
                                    isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
                                    isnull(Products.SD,0)SD, 
                                    isnull(Products.Packetprice,0)Packetprice, Products.Trading, 
                                    Products.TradingMarkUp,Products.NonStock,
                                    isnull(Products.QuantityInHand,0)QuantityInHand,
                                    convert(varchar, Products.OpeningDate,120)OpeningDate,
                                    isnull(Products.ReceivePrice,0)ReceivePrice,
                                    isnull(Products.IssuePrice,0)IssuePrice,
                                    isnull(Products.ProductCode,'N/A')ProductCode,
                                    isnull(Products.RebatePercent,0)RebatePercent,
                                    isnull(Products.TollCharge,0)TollCharge,
                                    isnull(Products.Banderol,'N')Banderol,
                                    isnull(Products.TollProduct,'N')TollProduct

                                    FROM Products LEFT OUTER JOIN
                                    ProductCategories ON Products.CategoryID = ProductCategories.CategoryID 
                      
                                    WHERE (Products.ItemNo = @ItemNo)  ";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                if (!objCommProduct.Parameters.Contains("@ItemNo"))
                { objCommProduct.Parameters.AddWithValue("@ItemNo", ItemNo); }
                else { objCommProduct.Parameters["@ItemNo"].Value = ItemNo; }


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


        public string GetTransactionType(string itemNo, string effectDate)
        {
            #region Initializ

            string retResults = "";
            string sqlText = "";
            SqlConnection currConn = null;
            DataTable dt = new DataTable();

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
                #region Stock

                sqlText = "  ";
                sqlText += " SELECT DISTINCT TransactionType,ReceiveDate";
                sqlText += " FROM PurchaseInvoiceDetails";
                sqlText += "  where  ItemNo='" + itemNo + "'  and Post='Y'  ";
                sqlText += " and ReceiveDate<='" + effectDate + "' order by ReceiveDate desc ";



                SqlCommand cmbTransactionType = new SqlCommand(sqlText, currConn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmbTransactionType);
                dataAdapter.Fill(dt);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        retResults = dt.Rows[0][0].ToString();
                    }
                }


                #endregion Stock

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

        public ProductVM GetProductWithCostPrice(string productCode, string purchaseNo, string effectDate)
        {
            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };
            var product = SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();
            decimal NewCostPrice = 0;
            var productGroup = new ProductCategoryDAL().SelectAll(Convert.ToInt32(product.CategoryID)).FirstOrDefault();
            string transType = GetTransactionType(product.ItemNo, effectDate);

            if (productGroup.IsRaw == "Raw")
            {
                if (transType == "TollReceiveRaw")
                {
                    NewCostPrice = 0;
                }
            }
            if (productGroup.IsRaw == "Raw" || productGroup.IsRaw == "Pack" ||
                                productGroup.IsRaw == "Trading")
            {
                DataTable dt = GetLIFOPurchaseInformation(product.ItemNo, effectDate, purchaseNo);

                if (dt.Rows.Count > 0)
                {
                    var PurchaseCostPrice = Convert.ToDecimal(dt.Rows[0]["PurchaseCostPrice"].ToString());
                    var PurchaseQuantity = Convert.ToDecimal(dt.Rows[0]["PurchaseQuantity"].ToString());
                    NewCostPrice = 0;
                    var PinvoiceNo = dt.Rows[0]["PurchaseInvoiceNo"].ToString();
                    if (PurchaseQuantity != 0)
                    {
                        NewCostPrice = PurchaseCostPrice / PurchaseQuantity;
                    }

                }
                else
                {
                    string[] retResult = { "Fail", "This Item has no price declaration" };
                    product.retResult = retResult;
                    //MessageBox.Show("This Item Name('" + txtRProductName.Text.Trim() + "'), Code('" + txtRProductCode.Text.Trim() + "') has no purchase price. ", this.Text);

                }
                if (transType == "TollReceiveRaw")
                {
                    NewCostPrice = 0;
                }
            }
            else
            {
                var nbrPrice = GetLastNBRPriceFromBOM(product.ItemNo, "VAT 1", effectDate, null, null);
                if (nbrPrice == 0)
                {
                    nbrPrice = GetLastNBRPriceFromBOM(product.ItemNo, "VAT 1 Ka (Tarrif)", effectDate, null, null);
                    NewCostPrice = nbrPrice;
                }
                else
                {
                    NewCostPrice = nbrPrice;
                }
                if (nbrPrice == 0)
                {
                    string[] retResult = { "Fail", "This Item has no price declaration" };
                    product.retResult = retResult;
                    //MessageBox.Show(
                    //    "This Item Name('" + txtRProductName.Text.Trim() + "'), Code('" + txtRProductCode.Text.Trim() + "') has no price declaration. ", this.Text);

                }
            }
            product.CostPrice = NewCostPrice;
            return product;
        }

        public DataTable ProductDTByItemNo(string ItemNo, string ProductName = "")
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Product");

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

                sqlText = @"SELECT  top 1  Products.ItemNo,
                                    isnull(Products.ProductName,'N/A')ProductName,
                                    isnull(Products.ProductDescription,'N/A')ProductDescription,
                                    Products.CategoryID, 
                                    isnull(Products.UOM,'N/A')UOM,
                                    isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
                                    isnull(Products.CostPrice,0)CostPrice,
                                    isnull(Products.SalesPrice,0)SalesPrice,
                                    isnull(Products.NBRPrice,0)NBRPrice,
                                    isnull(Products.SerialNo,'N/A')SerialNo ,
                                    isnull(Products.HSCodeNo,'N/A')HSCodeNo,
                                    isnull(Products.VDSRate,0)VDSRate,
                                    isnull(Products.VATRate,0)VATRate,
                                    isnull(Products.ActiveStatus,'N')ActiveStatus,
                                    isnull(Products.OpeningBalance,0)OpeningBalance,
                                    isnull(Products.Comments,'N/A')Comments,
                                    'N/A' HSDescription, 
                                    isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
                                    isnull(Products.SD,0)SD, 
                                    isnull(Products.VDSRate,0)VDSRate, 
                                    isnull(Products.Packetprice,0)Packetprice, Products.Trading, 
                                    Products.TradingMarkUp,Products.NonStock,
                                    isnull(Products.QuantityInHand,0)QuantityInHand,
                                    convert(varchar, Products.OpeningDate,120)OpeningDate,
                                    isnull(Products.ReceivePrice,0)ReceivePrice,
                                    isnull(Products.IssuePrice,0)IssuePrice,
                                    isnull(Products.ProductCode,'N/A')ProductCode,
                                    isnull(Products.RebatePercent,0)RebatePercent,
                                    isnull(Products.TollCharge,0)TollCharge,
                                    isnull(Products.Banderol,'N')Banderol,

                                    isnull(Products.CDRate,0)CDRate,
                                    isnull(Products.RDRate,0)RDRate,
                                    isnull(Products.TVARate,0)TVARate,
                                    isnull(Products.ATVRate,0)ATVRate,
                                    isnull(Products.VATRate2,0)VATRate2,   isnull(Products.TollProduct,'N')TollProduct   FROM Products  where 1=1 ";
                if (string.IsNullOrEmpty(ProductName))
                {
                    sqlText += @"   and ItemNo= @ItemNo  ";
                }
                else
                {
                    sqlText += @"   and ProductName= @ProductName  ";
                }

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;
                if (string.IsNullOrEmpty(ProductName))
                {
                    if (!objCommProduct.Parameters.Contains("@ItemNo"))
                    { objCommProduct.Parameters.AddWithValue("@ItemNo", ItemNo); }
                    else { objCommProduct.Parameters["@ItemNo"].Value = ItemNo; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@ProductName"))
                    { objCommProduct.Parameters.AddWithValue("@ProductName", ProductName); }
                    else { objCommProduct.Parameters["@ProductName"].Value = ProductName; }
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
    }
}
