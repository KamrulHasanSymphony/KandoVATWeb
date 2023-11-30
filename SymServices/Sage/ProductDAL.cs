using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Sage;


namespace SymServices.Sage
{
    public class ProductDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods

        //==================DropDownAll=================
        public List<NewProductViewModel> DropDownAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<NewProductViewModel> VMs = new List<NewProductViewModel>();
            NewProductViewModel vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionSageGL();
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


        //==================DropDown=================
        public List<NewProductViewModel> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<NewProductViewModel> VMs = new List<NewProductViewModel>();
            NewProductViewModel vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionSageGL();
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
WHERE  1=1 AND af.ActiveStatus = 'Y'
";



                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new NewProductViewModel();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = vm.ProductName + " ( " + vm.ProductName + " )";
                    vm.ProductCode = vm.ProductCode + "~" + vm.ItemNo;
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

        //==================SelectAll=================
        public List<NewProductViewModel> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<NewProductViewModel> VMs = new List<NewProductViewModel>();
            NewProductViewModel vm;
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
                    currConn = _dbsqlConnection.GetConnectionSageGL();
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
ItemNo
,ProductCode
,ProductName
,ProductDescription
,CategoryID
,ItemNo
,UOM
,CostPrice
,SalesPrice
,NBRPrice
,ReceivePrice
,IssuePrice
,TenderPrice
,ExportPrice
,InternalIssuePrice
,TollIssuePrice
,TollCharge
,OpeningBalance
,SerialNo
,HSCodeNo
,VATRate
,Comments
,SD
,PacketPrice
,Trading
,TradingMarkUp
,NonStock
,QuantityInHand
,OpeningDate
,RebatePercent
,TVBRate
,CnFRate
,InsuranceRate
,CDRate
,RDRate
,AITRate
,TVARate
,ATVRate
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,OpeningTotalCost
,Banderol
,IsVATRate
,IsSDRate

FROM Products  
WHERE  1=1 AND ActiveStatus = 'Y'

";


                if (Id > 0)
                {
                    sqlText += @" and ItemNo=@ItemNo";
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

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@ItemNo", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new NewProductViewModel();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductDescription = dr["ProductDescription"].ToString();
                    vm.CategoryID = dr["CategoryID"].ToString();
                    vm.UOM = dr["UOM"].ToString();
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"]);
                    vm.SalesPrice = Convert.ToDecimal(dr["SalesPrice"]);
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"]);
                    vm.ReceivePrice = Convert.ToDecimal(dr["ReceivePrice"]);
                    vm.IssuePrice = Convert.ToDecimal(dr["IssuePrice"]);
                    vm.TenderPrice = Convert.ToDecimal(dr["TenderPrice"]);
                    vm.ExportPrice = Convert.ToDecimal(dr["ExportPrice"]);
                    vm.InternalIssuePrice = Convert.ToDecimal(dr["InternalIssuePrice"]);
                    vm.TollIssuePrice = Convert.ToDecimal(dr["TollIssuePrice"]);
                    vm.TollCharge = Convert.ToDecimal(dr["TollCharge"]);
                    vm.OpeningBalance = Convert.ToDecimal(dr["OpeningBalance"]);
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.HSCodeNo = dr["HSCodeNo"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"]);
                    vm.Comments = dr["Comments"].ToString();
                    vm.SD = Convert.ToDecimal(dr["SD"]);
                    vm.PacketPrice = Convert.ToDecimal(dr["PacketPrice"]);
                    vm.Trading = dr["Trading"].ToString() == "Y" ? true : false;
                    vm.TradingMarkUp = dr["TradingMarkUp"].ToString();
                    vm.NonStock= dr["NonStock"].ToString() == "Y" ? true : false;
                    vm.QuantityInHand = Convert.ToDecimal(dr["QuantityInHand"]);
                    vm.OpeningDate= DateTime.Parse(dr["OpeningDate"].ToString());
                    vm.RebatePercent= Convert.ToDecimal(dr["RebatePercent"]);
                    vm.TVBRate= Convert.ToDecimal(dr["TVBRate"]);
                    vm.CnFRate= Convert.ToDecimal(dr["CnFRate"]);
                    vm.InsuranceRate= Convert.ToDecimal(dr["InsuranceRate"]);
                    vm.CDRate= Convert.ToDecimal(dr["CDRate"]);
                    vm.RDRate= Convert.ToDecimal(dr["RDRate"]);
                    vm.AITRate= Convert.ToDecimal(dr["AITRate"]);
                    vm.TVARate= Convert.ToDecimal(dr["TVARate"]);
                    vm.ATVRate= Convert.ToDecimal(dr["ATVRate"]);
                    vm.ActiveStatus = dr["ActiveStatus"].ToString() == "Y" ? true : false;
                    vm.CreatedBy= dr["CreatedBy"].ToString();
                    vm.CreatedOn= DateTime.Parse(dr["CreatedOn"].ToString());
                    vm.LastModifiedBy= dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn= DateTime.Parse(dr["LastModifiedOn"].ToString());
                    vm.OpeningTotalCost= Convert.ToDecimal(dr["OpeningTotalCost"]);
                    vm.Banderol= dr["Banderol"].ToString() == "Y" ? true : false;
                    vm.IsVATRate = dr["IsVATRate"].ToString() == "Y" ? true : false;
                    vm.IsSDRate = dr["IsSDRate"].ToString() == "Y" ? true : false;

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
        //==================Insert =================
        public string[] Insert(NewProductViewModel vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertProduct"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion
            #region Try
            try
            {
                #region Validation
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
                    currConn = _dbsqlConnection.GetConnectionSageGL();
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
                #region Save
                vm.ItemNo = _cDal.NextIdWithProduct("Products", currConn, transaction).ToString();
                //vm.ItemNo = "14";

                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO Products(
ItemNo
,ProductCode
,ProductName
,ProductDescription
,CategoryID
,UOM
,CostPrice
,SalesPrice
,NBRPrice
,ReceivePrice
,IssuePrice
,TenderPrice
,ExportPrice
,InternalIssuePrice
,TollIssuePrice
,TollCharge
,OpeningBalance
,SerialNo
,HSCodeNo
,VATRate
,Comments
,SD
,PacketPrice
,Trading
,TradingMarkUp
,NonStock
,QuantityInHand
,OpeningDate
,RebatePercent
,TVBRate
,CnFRate
,InsuranceRate
,CDRate
,RDRate
,AITRate
,TVARate
,ATVRate
,ActiveStatus
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,OpeningTotalCost
,Banderol
,IsVATRate
,IsSDRate
) VALUES (
 @ItemNo
,@ProductCode
,@ProductName
,@ProductDescription
,@CategoryID
,@UOM
,@CostPrice
,@SalesPrice
,@NBRPrice
,@ReceivePrice
,@IssuePrice
,@TenderPrice
,@ExportPrice
,@InternalIssuePrice
,@TollIssuePrice
,@TollCharge
,@OpeningBalance
,@SerialNo
,@HSCodeNo
,@VATRate
,@Comments
,@SD
,@PacketPrice
,@Trading
,@TradingMarkUp
,@NonStock
,@QuantityInHand
,@OpeningDate
,@RebatePercent
,@TVBRate
,@CnFRate
,@InsuranceRate
,@CDRate
,@RDRate
,@AITRate
,@TVARate
,@ATVRate
,@ActiveStatus
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@OpeningTotalCost
,@Banderol
,@IsVATRate
,@IsSDRate
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                    cmdInsert.Parameters.AddWithValue("@ItemNo",vm.ItemNo);
                    cmdInsert.Parameters.AddWithValue("@ProductCode", vm.ProductCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ProductName", vm.ProductName);
                    cmdInsert.Parameters.AddWithValue("@ProductDescription", vm.ProductDescription ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CategoryID",vm.CategoryID);
                    cmdInsert.Parameters.AddWithValue("@UOM", vm.UOM ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CostPrice",vm.CostPrice);
                    cmdInsert.Parameters.AddWithValue("@SalesPrice",vm.SalesPrice);
                    cmdInsert.Parameters.AddWithValue("@NBRPrice",vm.NBRPrice);
                    cmdInsert.Parameters.AddWithValue("@ReceivePrice",vm.ReceivePrice);
                    cmdInsert.Parameters.AddWithValue("@IssuePrice",vm.IssuePrice);
                    cmdInsert.Parameters.AddWithValue("@TenderPrice",vm.TenderPrice);
                    cmdInsert.Parameters.AddWithValue("@ExportPrice",vm.ExportPrice);
                    cmdInsert.Parameters.AddWithValue("@InternalIssuePrice",vm.InternalIssuePrice);
                    cmdInsert.Parameters.AddWithValue("@TollIssuePrice",vm.TollIssuePrice);
                    cmdInsert.Parameters.AddWithValue("@TollCharge",vm.TollCharge);
                    cmdInsert.Parameters.AddWithValue("@OpeningBalance",vm.OpeningBalance);
                    cmdInsert.Parameters.AddWithValue("@SerialNo", vm.SerialNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@HSCodeNo", vm.HSCodeNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@VATRate",vm.VATRate);
                    cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@SD",vm.SD);
                    cmdInsert.Parameters.AddWithValue("@PacketPrice",vm.PacketPrice);
                    cmdInsert.Parameters.AddWithValue("@Trading", vm.Trading?"Y":"N");
                    cmdInsert.Parameters.AddWithValue("@TradingMarkUp", vm.TradingMarkUp ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@NonStock", vm.NonStock? "Y" : "N");
                    cmdInsert.Parameters.AddWithValue("@QuantityInHand",vm.QuantityInHand);
                    cmdInsert.Parameters.AddWithValue("@OpeningDate",vm.OpeningDate);
                    cmdInsert.Parameters.AddWithValue("@RebatePercent",vm.RebatePercent);
                    cmdInsert.Parameters.AddWithValue("@TVBRate",vm.TVBRate);
                    cmdInsert.Parameters.AddWithValue("@CnFRate",vm.CnFRate);
                    cmdInsert.Parameters.AddWithValue("@InsuranceRate",vm.InsuranceRate);
                    cmdInsert.Parameters.AddWithValue("@CDRate",vm.CDRate);
                    cmdInsert.Parameters.AddWithValue("@RDRate",vm.RDRate);
                    cmdInsert.Parameters.AddWithValue("@AITRate",vm.AITRate);
                    cmdInsert.Parameters.AddWithValue("@TVARate",vm.TVARate);
                    cmdInsert.Parameters.AddWithValue("@ATVRate",vm.ATVRate);
                    cmdInsert.Parameters.AddWithValue("@ActiveStatus", vm.ActiveStatus? "Y" : "N");
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CreatedOn",vm.CreatedOn);
                    cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@LastModifiedOn",vm.LastModifiedOn);
                    cmdInsert.Parameters.AddWithValue("@OpeningTotalCost",vm.OpeningTotalCost);
                    cmdInsert.Parameters.AddWithValue("@Banderol", vm.Banderol? "Y" : "N");
                    cmdInsert.Parameters.AddWithValue("@IsVATRate", vm.IsVATRate? "Y" : "N");
                    cmdInsert.Parameters.AddWithValue("@IsSDRate", vm.IsSDRate? "Y" : "N");                

                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Product.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This Product already used!";
                    throw new ArgumentNullException("Please Input Product Value", "");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.ItemNo.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
            #region Results
            return retResults;
            #endregion
        }
        //==================Update =================
        public string[] Update(NewProductViewModel vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ProductUpdate"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                    currConn = _dbsqlConnection.GetConnectionSageGL();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("Update"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE Products SET ";

                    sqlText+="ProductCode =@ProductCode";
                    sqlText += ", ProductName=@ProductName";
                    sqlText += ", ProductDescription=@ProductDescription ";
                    sqlText += ", CategoryID=@CategoryID";
                    sqlText += ", UOM=@UOM";
                    sqlText += ", CostPrice=@CostPrice";
                    sqlText += ", SalesPrice=@SalesPrice";
                    sqlText += ", NBRPrice=@NBRPrice";
                    sqlText += ", ReceivePrice=@ReceivePrice";
                    sqlText += ", IssuePrice=@IssuePrice";
                    sqlText += ", TenderPrice=@TenderPrice";
                    sqlText += ", ExportPrice=@ExportPrice";
                    sqlText += ", InternalIssuePrice =@InternalIssuePrice ";
                    sqlText += ", TollIssuePrice=@TollIssuePrice";
                    sqlText += ", TollCharge=@TollCharge";
                    sqlText += ", OpeningBalance=@OpeningBalance";
                    sqlText += ", SerialNo=@SerialNo";
                    sqlText += ", HSCodeNo=@HSCodeNo";
                    sqlText += ", VATRate=@VATRate";
                    sqlText += ", Comments=@Comments";
                    sqlText += ", SD=@SD";
                    sqlText += ", PacketPrice=@PacketPrice";
                    sqlText += ", Trading=@Trading";
                    sqlText += ", TradingMarkUp=@TradingMarkUp";
                    sqlText += ", NonStock=@NonStock";
                    sqlText += ", QuantityInHand=@QuantityInHand";
                    sqlText += ", OpeningDate=@OpeningDate";
                    sqlText += ", RebatePercent=@RebatePercent";
                    sqlText += ", TVBRate=@TVBRate";
                    sqlText += ", CnFRate=@CnFRate";
                    sqlText += ", InsuranceRate=@InsuranceRate";
                    sqlText += ", CDRate=@CDRate";
                    sqlText += ", RDRate=@RDRate";
                    sqlText += ", AITRate=@AITRate";
                    sqlText += ", TVARate=@TVARate";
                    sqlText += ", ATVRate=@ATVRate";
                    sqlText += ", LastModifiedBy=@LastModifiedBy";
                    sqlText += ", LastModifiedOn=@LastModifiedOn";
                    sqlText += ", OpeningTotalCost=@OpeningTotalCost";
                    sqlText += ", Banderol=@Banderol";
                    sqlText += ", IsVATRate=@IsVATRate";
                    sqlText += ", IsSDRate=@IsSDRate";
                    sqlText += " WHERE ItemNo=@ItemNo ";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                    cmdUpdate.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    cmdUpdate.Parameters.AddWithValue("@ProductCode", vm.ProductCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ProductName", vm.ProductName);
                    cmdUpdate.Parameters.AddWithValue("@ProductDescription", vm.ProductDescription ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@CategoryID", vm.CategoryID);
                    cmdUpdate.Parameters.AddWithValue("@UOM", vm.UOM ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@CostPrice", vm.CostPrice);
                    cmdUpdate.Parameters.AddWithValue("@SalesPrice", vm.SalesPrice);
                    cmdUpdate.Parameters.AddWithValue("@NBRPrice", vm.NBRPrice);
                    cmdUpdate.Parameters.AddWithValue("@ReceivePrice", vm.ReceivePrice);
                    cmdUpdate.Parameters.AddWithValue("@IssuePrice", vm.IssuePrice);
                    cmdUpdate.Parameters.AddWithValue("@TenderPrice", vm.TenderPrice);
                    cmdUpdate.Parameters.AddWithValue("@ExportPrice", vm.ExportPrice);
                    cmdUpdate.Parameters.AddWithValue("@InternalIssuePrice", vm.InternalIssuePrice);
                    cmdUpdate.Parameters.AddWithValue("@TollIssuePrice", vm.TollIssuePrice);
                    cmdUpdate.Parameters.AddWithValue("@TollCharge", vm.TollCharge);
                    cmdUpdate.Parameters.AddWithValue("@OpeningBalance", vm.OpeningBalance);
                    cmdUpdate.Parameters.AddWithValue("@SerialNo", vm.SerialNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@HSCodeNo", vm.HSCodeNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@VATRate", vm.VATRate);
                    cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@SD", vm.SD);
                    cmdUpdate.Parameters.AddWithValue("@PacketPrice", vm.PacketPrice);
                    cmdUpdate.Parameters.AddWithValue("@Trading", vm.Trading ? "Y" : "N");
                    cmdUpdate.Parameters.AddWithValue("@TradingMarkUp", vm.TradingMarkUp ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@NonStock", vm.NonStock ? "Y" : "N");
                    cmdUpdate.Parameters.AddWithValue("@QuantityInHand", vm.QuantityInHand);
                    cmdUpdate.Parameters.AddWithValue("@OpeningDate", vm.OpeningDate);
                    cmdUpdate.Parameters.AddWithValue("@RebatePercent", vm.RebatePercent);
                    cmdUpdate.Parameters.AddWithValue("@TVBRate", vm.TVBRate);
                    cmdUpdate.Parameters.AddWithValue("@CnFRate", vm.CnFRate);
                    cmdUpdate.Parameters.AddWithValue("@InsuranceRate", vm.InsuranceRate);
                    cmdUpdate.Parameters.AddWithValue("@CDRate", vm.CDRate);
                    cmdUpdate.Parameters.AddWithValue("@RDRate", vm.RDRate);
                    cmdUpdate.Parameters.AddWithValue("@AITRate", vm.AITRate);
                    cmdUpdate.Parameters.AddWithValue("@TVARate", vm.TVARate);
                    cmdUpdate.Parameters.AddWithValue("@ATVRate", vm.ATVRate);
                    cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                    cmdUpdate.Parameters.AddWithValue("@OpeningTotalCost", vm.OpeningTotalCost);
                    cmdUpdate.Parameters.AddWithValue("@Banderol", vm.Banderol ? "Y" : "N");
                    cmdUpdate.Parameters.AddWithValue("@IsVATRate", vm.IsVATRate ? "Y" : "N");
                    cmdUpdate.Parameters.AddWithValue("@IsSDRate", vm.IsSDRate ? "Y" : "N");   

 
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Product.", "");
                    }
                    #endregion SqlExecution

                    retResults[2] = vm.ItemNo.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Product Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Product Update", "Could not found any item.");
                }
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.ItemNo.ToString();
                #endregion SuccessResult
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
        ////==================Delete =================
        public string[] Delete(NewProductViewModel vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteProduct"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionSageGL();
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
                        sqlText += " where ItemNo=@ItemNo";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@ItemNo", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@ActiveStatus", "N");
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);
                        cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Product Delete", vm.ItemNo + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Product Information Delete", "Could not found any item.");
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

        #endregion
    }
}
