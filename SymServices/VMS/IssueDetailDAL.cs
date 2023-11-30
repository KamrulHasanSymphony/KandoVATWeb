using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.VMS;
 
namespace SymServices.VMS
{
    public class IssueDetailDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
      CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================SelectByMasterId=================
        public List<IssueDetailViewModel> SelectByMasterId(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<IssueDetailViewModel> VMs = new List<IssueDetailViewModel>();
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
                string[] conditionField = { "MasterId" };
                string[] conditionValue = { Id.ToString() };
                VMs = SelectAll(conditionField, conditionValue, currConn, transaction);
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
        //==================SelectById=================
        public List<IssueDetailViewModel> SelectById(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<IssueDetailViewModel> VMs = new List<IssueDetailViewModel>();
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
                string[] conditionField = { "Id" };
                string[] conditionValue = { Id.ToString() };
                VMs = SelectAll(conditionField, conditionValue, currConn, transaction);
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
        public List<IssueDetailViewModel> SelectAll(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<IssueDetailViewModel> VMs = new List<IssueDetailViewModel>();
            IssueDetailViewModel vm;
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
 Id
,MasterId
,IssueNo
,IssueLineNo
,ItemNo
,ISNULL(Quantity,0) Quantity  
,ISNULL(NBRPrice,0) NBRPrice  
,ISNULL(CostPrice,0) CostPrice  
,UOM
,ISNULL(VATRate,0) VATRate  
,ISNULL(VATAmount,0) VATAmount  
,ISNULL(SubTotal,0) SubTotal  
,Comments
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,ReceiveNo
,IssueDateTime
,ISNULL(SD,0) SD  
,ISNULL(SDAmount,0) SDAmount  
,ISNULL(Wastage,0) Wastage  
,BOMDate
,FinishItemNo
,Post
,TransactionType
,IssueReturnId
,ISNULL(DiscountAmount,0) DiscountAmount  
,ISNULL(DiscountedNBRPrice,0) DiscountedNBRPrice  
,UOMQty
,ISNULL(UOMPrice,0) UOMPrice  
,ISNULL(UOMc,0) UOMc  
,UOMn
,BOMId
,ISNULL(UOMWastage,0) UOMWastage  
,IsProcess

FROM IssueDetails  
WHERE  1=1

";

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
                        objComm.Parameters.AddWithValue("@" + cField,conditionValues[j]);
                    }
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                ProductDAL ProductdDal;
                while (dr.Read())
                {
                    vm = new IssueDetailViewModel();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.MasterId = Convert.ToInt32(dr["MasterId"].ToString());
                    vm.IssueNo = dr["IssueNo"].ToString();
                    vm.IssueLineNo = Convert.ToInt32(dr["IssueLineNo"].ToString());
                    vm.ItemNo = Convert.ToInt32(dr["ItemNo"].ToString());
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = DateTime.Parse(dr["CreatedOn"].ToString());
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = DateTime.Parse(dr["LastModifiedOn"].ToString());
                    vm.ReceiveNo = dr["ReceiveNo"].ToString();
                    vm.IssueDateTime = Ordinary.DateTimeToDate(dr["IssueDateTime"].ToString());
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                    vm.Wastage = Convert.ToDecimal(dr["Wastage"].ToString());
                    vm.BOMDate = DateTime.Parse(dr["BOMDate"].ToString());
                    vm.FinishItemNo = dr["FinishItemNo"].ToString();
                    vm.Post = dr["Post"].ToString() == "Y" ? true : false;
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.IssueReturnId = dr["IssueReturnId"].ToString();
                    vm.DiscountAmount = Convert.ToDecimal(dr["DiscountAmount"].ToString());
                    vm.DiscountedNBRPrice = Convert.ToDecimal(dr["DiscountedNBRPrice"].ToString());
                    vm.UOMQty = Convert.ToDecimal(dr["UOMQty"].ToString());
                    vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.BOMId = dr["BOMId"].ToString();
                    vm.UOMWastage = Convert.ToDecimal(dr["UOMWastage"].ToString());
                    vm.IsProcess = dr["IsProcess"].ToString() == "Y" ? true : false;

                    ProductdDal = new ProductDAL();
                    var product = ProductdDal.SelectAll(vm.ItemNo.ToString()).FirstOrDefault();
                    vm.ItemName = product.ProductName;

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
        public string[] Insert(IssueDetailViewModel vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert IssueDetail"; //Method Name
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
                #region Save


                vm.Id = Convert.ToInt32(_cDal.NextIdWithColumn("IssueDetails", "Id", currConn, transaction).ToString());
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  IssueDetails(
 Id
,MasterId
,IssueNo
,IssueLineNo
,ItemNo
,Quantity
,NBRPrice
,CostPrice
,UOM
,VATRate
,VATAmount
,SubTotal
,Comments
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,ReceiveNo
,IssueDateTime
,SD
,SDAmount
,Wastage
,BOMDate
,FinishItemNo
,Post
,TransactionType
,IssueReturnId
,DiscountAmount
,DiscountedNBRPrice
,UOMQty
,UOMPrice
,UOMc
,UOMn
,BOMId
,UOMWastage
,IsProcess

) VALUES (
 @Id
,@MasterId
,@IssueNo
,@IssueLineNo
,@ItemNo
,@Quantity
,@NBRPrice
,@CostPrice
,@UOM
,@VATRate
,@VATAmount
,@SubTotal
,@Comments
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@ReceiveNo
,@IssueDateTime
,@SD
,@SDAmount
,@Wastage
,@BOMDate
,@FinishItemNo
,@Post
,@TransactionType
,@IssueReturnId
,@DiscountAmount
,@DiscountedNBRPrice
,@UOMQty
,@UOMPrice
,@UOMc
,@UOMn
,@BOMId
,@UOMWastage
,@IsProcess
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@MasterId", vm.MasterId);
                    cmdInsert.Parameters.AddWithValue("@IssueNo", vm.IssueNo);
                    cmdInsert.Parameters.AddWithValue("@IssueLineNo", vm.IssueLineNo);
                    cmdInsert.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                    cmdInsert.Parameters.AddWithValue("@Quantity", vm.Quantity);
                    cmdInsert.Parameters.AddWithValue("@NBRPrice", vm.NBRPrice);
                    cmdInsert.Parameters.AddWithValue("@CostPrice", vm.CostPrice);
                    cmdInsert.Parameters.AddWithValue("@UOM", vm.UOM ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@VATRate", vm.VATRate);
                    cmdInsert.Parameters.AddWithValue("@VATAmount", vm.VATAmount);
                    cmdInsert.Parameters.AddWithValue("@SubTotal", vm.SubTotal);
                    cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                    cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                    cmdInsert.Parameters.AddWithValue("@ReceiveNo", vm.ReceiveNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IssueDateTime", Ordinary.DateToString(vm.IssueDateTime));
                    cmdInsert.Parameters.AddWithValue("@SD", vm.SD);
                    cmdInsert.Parameters.AddWithValue("@SDAmount", vm.SDAmount);
                    cmdInsert.Parameters.AddWithValue("@Wastage", vm.Wastage);
                    cmdInsert.Parameters.AddWithValue("@BOMDate", Ordinary.DateToString(vm.IssueDateTime));
                    cmdInsert.Parameters.AddWithValue("@FinishItemNo", vm.FinishItemNo);
                    cmdInsert.Parameters.AddWithValue("@Post", false);
                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IssueReturnId", vm.IssueReturnId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@DiscountAmount", vm.DiscountAmount);
                    cmdInsert.Parameters.AddWithValue("@DiscountedNBRPrice", vm.DiscountedNBRPrice);
                    cmdInsert.Parameters.AddWithValue("@UOMQty", vm.UOMQty);
                    cmdInsert.Parameters.AddWithValue("@UOMPrice", vm.UOMPrice);
                    cmdInsert.Parameters.AddWithValue("@UOMc", vm.UOMc);
                    cmdInsert.Parameters.AddWithValue("@UOMn", vm.UOMn ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@BOMId", vm.BOMId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@UOMWastage", vm.UOMWastage);
                    cmdInsert.Parameters.AddWithValue("@IsProcess", true);

                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update  IssueDetails.", "");
                    }
                    #endregion SqlExecution

                }
                else
                {
                    retResults[1] = "This  IssueDetail already used!";
                    throw new ArgumentNullException("Please Input  IssueDetail Value", "");
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
                retResults[2] = vm.Id.ToString();
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
        //==================Insert =================
        public string[] Insert(List<IssueDetailViewModel> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert IssueDetail"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int NextId = 0;
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
                #region Save

                NextId = _cDal.NextId(" IssueDetails", currConn, transaction);
                if (VMs != null && VMs.Count > 0)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  IssueDetails(
 MasterId
,IssueNo
,IssueLineNo
,ItemNo
,Quantity
,NBRPrice
,CostPrice
,UOM
,VATRate
,VATAmount
,SubTotal
,Comments
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,ReceiveNo
,IssueDateTime
,SD
,SDAmount
,Wastage
,BOMDate
,FinishItemNo
,Post
,TransactionType
,IssueReturnId
,DiscountAmount
,DiscountedNBRPrice
,UOMQty
,UOMPrice
,UOMc
,UOMn
,BOMId
,UOMWastage
,IsProcess

) VALUES (
 @MasterId
,@IssueNo
,@IssueLineNo
,@ItemNo
,@Quantity
,@NBRPrice
,@CostPrice
,@UOM
,@VATRate
,@VATAmount
,@SubTotal
,@Comments
,@CreatedBy
,@CreatedOn
,@LastModifiedBy
,@LastModifiedOn
,@ReceiveNo
,@IssueDateTime
,@SD
,@SDAmount
,@Wastage
,@BOMDate
,@FinishItemNo
,@Post
,@TransactionType
,@IssueReturnId
,@DiscountAmount
,@DiscountedNBRPrice
,@UOMQty
,@UOMPrice
,@UOMc
,@UOMn
,@BOMId
,@UOMWastage
,@IsProcess
) 
";

                    #endregion SqlText
                    foreach (IssueDetailViewModel vm in VMs)
                    {
                        
                        #region SqlExecution
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@Id", NextId);
                        cmdInsert.Parameters.AddWithValue("@MasterId", vm.MasterId);
                        cmdInsert.Parameters.AddWithValue("@IssueNo", vm.IssueNo);
                        cmdInsert.Parameters.AddWithValue("@IssueLineNo", vm.IssueLineNo);
                        cmdInsert.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                        cmdInsert.Parameters.AddWithValue("@Quantity", vm.Quantity);
                        cmdInsert.Parameters.AddWithValue("@NBRPrice", vm.NBRPrice);
                        cmdInsert.Parameters.AddWithValue("@CostPrice", vm.CostPrice);
                        cmdInsert.Parameters.AddWithValue("@UOM", vm.UOM ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@VATRate", vm.VATRate);
                        cmdInsert.Parameters.AddWithValue("@VATAmount", vm.VATAmount);
                        cmdInsert.Parameters.AddWithValue("@SubTotal", vm.SubTotal);
                        cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                        cmdInsert.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                        cmdInsert.Parameters.AddWithValue("@ReceiveNo", vm.ReceiveNo ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IssueDateTime", vm.IssueDateTime);
                        cmdInsert.Parameters.AddWithValue("@SD", vm.SD);
                        cmdInsert.Parameters.AddWithValue("@SDAmount", vm.SDAmount);
                        cmdInsert.Parameters.AddWithValue("@Wastage", vm.Wastage);
                        cmdInsert.Parameters.AddWithValue("@BOMDate", vm.BOMDate);
                        cmdInsert.Parameters.AddWithValue("@FinishItemNo", vm.FinishItemNo);
                        cmdInsert.Parameters.AddWithValue("@Post", false);
                        cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IssueReturnId", vm.IssueReturnId ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@DiscountAmount", vm.DiscountAmount);
                        cmdInsert.Parameters.AddWithValue("@DiscountedNBRPrice", vm.DiscountedNBRPrice);
                        cmdInsert.Parameters.AddWithValue("@UOMQty", vm.UOMQty);
                        cmdInsert.Parameters.AddWithValue("@UOMPrice", vm.UOMPrice);
                        cmdInsert.Parameters.AddWithValue("@UOMc", vm.UOMc);
                        cmdInsert.Parameters.AddWithValue("@UOMn", vm.UOMn ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@BOMId", vm.BOMId ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@UOMWastage", vm.UOMWastage);
                        cmdInsert.Parameters.AddWithValue("@IsProcess", true);

                        var exeRes = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update  IssueDetails.", "");
                        }

                        NextId++;
                        #endregion SqlExecution
                    }
                }
                else
                {
                    retResults[1] = "This  IssueDetail already used!";
                    throw new ArgumentNullException("Please Input  IssueDetail Value", "");
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
                retResults[2] = VMs.FirstOrDefault().Id.ToString();
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
        //==================Report View=============
        public DataTable VoucherReport(IssueDetailViewModel vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
select b.IssueDateTime DateTime, b.IssueNo IssuNo, b.TotalAmount TotalAmount,
a.Quantity Quantity, a.CostPrice CostPrice,a.VATAmount TotalVat, a.SubTotal SubTotal
 from IssueDetails a left outer join 
IssueHeaders b on a.MasterId=b.Id where a.MasterId=@MasterId;
";

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

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

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
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                da.SelectCommand.Parameters.AddWithValue("@MasterId", vm.MasterId);

                da.Fill(dt);
                //dt = Ordinary.DtColumnStringToDate(dt, "DateTime");
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
            return dt;
        }

        #endregion
    }
}
