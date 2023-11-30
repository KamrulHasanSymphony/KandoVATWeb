using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Sage;

namespace SymServices.Sage
{
    public class NewTransactionDetailDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods

        ////==================SelectAll=================
//        public List<NewTransactionDetailViewModel> SelectAllFromPCR(NewTransactionViewModel paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
//        {
//            #region Variables
//            SqlConnection currConn = null;
//            SqlTransaction transaction = null;
//            string sqlText = "";
//            List<NewTransactionDetailViewModel> VMs = new List<NewTransactionDetailViewModel>();
//            NewTransactionDetailViewModel vm;
//            #endregion
//            try
//            {
//                #region open connection and transaction
//                #region New open connection and transaction
//                if (VcurrConn != null)
//                {
//                    currConn = VcurrConn;
//                }
//                if (Vtransaction != null)
//                {
//                    transaction = Vtransaction;
//                }
//                #endregion New open connection and transaction
//                if (currConn == null)
//                {
//                    currConn = _dbsqlConnection.GetConnectionSageGL();
//                    if (currConn.State != ConnectionState.Open)
//                    {
//                        currConn.Open();
//                    }
//                }
//                if (transaction == null)
//                {
//                    transaction = currConn.BeginTransaction("");
//                }
//                #endregion open connection and transaction
//                #region sql statement
//                #region SqlText

//                sqlText = @"
//SELECT DISTINCT CommissionBillNo,DocumentType,gla.Id AccountId,gla.Code AccountCode,gla.Name AccountName
//,SUM(CommissionAmount)TransactionAmount,SUM(AITAmount)TaxAmount,SUM(PCAmount)PCAmount  
//FROM(
//SELECT   b.CommissionBillNo
//,CASE 
//WHEN b.DocumentType  IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='FIR') THEN 'FIR'
//WHEN b.DocumentType  IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MTR') THEN 'MTR'
//WHEN b.DocumentType  IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MISC') THEN 'MISC'
//WHEN b.DocumentType  IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='ENG') THEN 'ENG'
//WHEN b.DocumentType  IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MAR') THEN 'MAR'
//WHEN b.DocumentType  IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MAH') THEN 'MAH'
// ELSE 'NA' END DocumentType,CommissionAmount,AITAmount,PCAmount
// FROM GLPettyCashRequisitionFormBs b 
//WHERE 1=1  
//AND b.CommissionBillNo = @CommissionBillNo
//AND b.BranchId = @BranchId
//AND b.IsRejected = 0
//) AS a LEFT OUTER JOIN GLAccounts gla ON gla.BusinessClass=a.DocumentType
//WHERE    gla.BranchId=@BranchId AND ISNULL(gla.IsBDE,0)=0
//GROUP BY CommissionBillNo,DocumentType,gla.Id,gla.Code,gla.Name
//
//";


//                #endregion SqlText
//                #region SqlExecution

//                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

//                objComm.Parameters.AddWithValue("@BranchId", paramVM.BranchId);
//                objComm.Parameters.AddWithValue("@CommissionBillNo", paramVM.CommissionBillNo);

//                SqlDataReader dr;
//                dr = objComm.ExecuteReader();
//                while (dr.Read())
//                {
//                    vm = new NewTransactionDetailViewModel();
//                    vm.AccountId = Convert.ToInt32(dr["AccountId"]);
//                    vm.TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]);
//                    vm.TaxAmount = Convert.ToDecimal(dr["TaxAmount"]);
//                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();

//                    vm.AccountName = dr["AccountName"].ToString();
//                    vm.AccountCode = dr["AccountCode"].ToString();

//                    vm.AccountName = vm.AccountCode + " ( " + vm.AccountName + " )";

//                    VMs.Add(vm);
//                }
//                dr.Close();

//                #endregion SqlExecution

//                if (Vtransaction == null && transaction != null)
//                {
//                    transaction.Commit();
//                }
//                #endregion
//            }
//            #region catch
//            catch (SqlException sqlex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
//            }
//            catch (Exception ex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
//            }
//            #endregion
//            #region finally
//            finally
//            {
//                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
//                {
//                    currConn.Close();
//                }
//            }
//            #endregion
//            return VMs;
//        }

        //==================SelectByMasterId=================

        public List<NewTransactionDetailViewModel> SelectByMasterId(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<NewTransactionDetailViewModel> VMs = new List<NewTransactionDetailViewModel>();
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
                string[] conditionField = { "fd.NewTransactionId" };
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
        public List<NewTransactionDetailViewModel> SelectById(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<NewTransactionDetailViewModel> VMs = new List<NewTransactionDetailViewModel>();
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
                string[] conditionField = { "fd.Id" };
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

        ////==================SelectAll=================
        public List<NewTransactionDetailViewModel> SelectAll(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<NewTransactionDetailViewModel> VMs = new List<NewTransactionDetailViewModel>();
            NewTransactionDetailViewModel vm;
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
fd.Id
,fd.BranchId
,fd.NewTransactionId
,fd.AccountId
,a.Name AccountName
,fd.TransactionAmount
,ISNULL(fd.VATAmount,0)VATAmount
,fd.Remarks
FROM  NewTransactionDetails fd 
left outer join GLAccounts a on fd.AccountId=a.id 
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
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new NewTransactionDetailViewModel();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.AccountId = Convert.ToInt32(dr["AccountId"]);
                    vm.TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]);
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"]);
                    vm.Remarks = dr["Remarks"].ToString();
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
        public string[] Insert(NewTransactionDetailViewModel vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert NewTransactionDetail"; //Method Name
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
                #region Balance Check
                DataTable dt = new DataTable();
                GLCeilingDAL _glCeilingDAL = new GLCeilingDAL();
                DataSet ds = new DataSet();


                //DataTable dtCeiling = new DataTable();
                //dtCeiling = ds.Tables[1];

                //decimal amount = 0;
                //if (dtCeiling.Rows.Count > 0)
                //{
                //    amount = Convert.ToDecimal(dtCeiling.Rows[0]["amount"]);
                //}

                //if (amount > 0)
                //{
                //    decimal balance = Convert.ToDecimal(dt.Rows[0]["Balance"]);
                //    if (vm.TransactionAmount > balance)
                //    {
                //        retResults[1] = "Not Enough Balance!";
                //        return retResults;
                //    }
                //}

                #endregion


                vm.Id = _cDal.NextId(" NewTransactionDetails", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  NewTransactionDetails(
Id
,BranchId
,NewTransactionId
,AccountId
,TransactionAmount
,VATAmount
,Remarks

) VALUES (
@Id
,@BranchId
,@NewTransactionId
,@AccountId
,@TransactionAmount
,@VATAmount
,@Remarks
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@NewTransactionId", vm.NewTransactionId);
                    cmdInsert.Parameters.AddWithValue("@AccountId", vm.AccountId);
                    cmdInsert.Parameters.AddWithValue("@TransactionAmount", vm.TransactionAmount);
                    cmdInsert.Parameters.AddWithValue("@VATAmount", vm.VATAmount);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);

                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update  NewTransactionDetails.", "");
                    }
                    #endregion SqlExecution

                }
                else
                {
                    retResults[1] = "This  NewTransactionDetail already used!";
                    throw new ArgumentNullException("Please Input  NewTransactionDetail Value", "");
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
        public string[] Insert(List<NewTransactionDetailViewModel> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert NewTransactionDetail"; //Method Name
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

                DataTable dt = new DataTable();
                GLCeilingDAL _glCeilingDAL = new GLCeilingDAL();
                DataSet ds = new DataSet();
                DataTable dtCeiling = new DataTable();

                NextId = _cDal.NextId(" NewTransactionDetails", currConn, transaction);
                if (VMs != null && VMs.Count > 0)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  NewTransactionDetails(
Id
,BranchId
,NewTransactionId
,AccountId
,TransactionAmount
,VATAmount
,Remarks

) VALUES (
@Id
,@BranchId
,@NewTransactionId
,@AccountId
,@TransactionAmount
,@VATAmount
,@Remarks
) 
";

                    #endregion SqlText
                    foreach (NewTransactionDetailViewModel vm in VMs)
                    {
                        #region SqlExecution
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@Id", NextId);
                        cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmdInsert.Parameters.AddWithValue("@NewTransactionId", vm.NewTransactionId);
                        cmdInsert.Parameters.AddWithValue("@AccountId", vm.AccountId);
                        cmdInsert.Parameters.AddWithValue("@TransactionAmount", vm.TransactionAmount);
                        cmdInsert.Parameters.AddWithValue("@VATAmount", vm.VATAmount);
                        cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                        var exeRes = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update  NewTransactionDetails.", "");
                        }

                        NextId++;
                        #endregion SqlExecution
                    }
                }
                else
                {
                    retResults[1] = "This  NewTransactionDetail already used!";
                    throw new ArgumentNullException("Please Input  NewTransactionDetail Value", "");
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

        ////==================Report=================

        ////==================Report=================
//        public DataTable VoucherReport(NewTransactionDetailViewModel vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
//        {
//            #region Variables
//            SqlConnection currConn = null;
//            SqlTransaction transaction = null;
//            string sqlText = "";
//            DataTable dt = new DataTable();
//            #endregion
//            try
//            {
//                #region open connection and transaction
//                currConn = _dbsqlConnection.GetConnectionSageGL();
//                if (currConn.State != ConnectionState.Open)
//                {
//                    currConn.Open();
//                }
//                #endregion open connection and transaction
//                #region sql statement
//                sqlText = @"
//--declare @GLFinancialTransactionId as varchar(10)
//--set @GLFinancialTransactionId = 5
//
//select acc.Name AccountName, b.Name BranchName,ft.Code,ft.TransactionDateTime, a.* 
//,case when a.isDebit=1 then a.TransactionAmount else 0 end Dr
//,case when a.isDebit=0 then a.TransactionAmount else 0 end Cr
// from(
//select distinct GLFinancialTransactionId,AccountId,IsDebit, ISNULL(Remarks, 'NA')Remarks, sum(TransactionAmount)TransactionAmount  from NewTransactionDetails
//where GLFinancialTransactionId=@GLFinancialTransactionId
//group by GLFinancialTransactionId,AccountId,IsDebit,Remarks
//union all
//select distinct Id, AccountId,IsDebit,Remarks,sum(GrandTotal)TransactionAmount from GLFinancialTransactions
//where id in (@GLFinancialTransactionId)
//group by Id,AccountId,IsDebit,Remarks
//union all
//select distinct Id, VATAccountId,0 IsDebit,'' Remarks,sum(VATAmount)TransactionAmount from GLFinancialTransactions
//where id in (@GLFinancialTransactionId)
//group by Id,VATAccountId,IsDebit
//union all
//
//select distinct Id, AITAccountId,0 IsDebit,'' Remarks,sum(TaxAmount)TransactionAmount from GLFinancialTransactions
//where id in (@GLFinancialTransactionId)
//group by Id,AITAccountId,IsDebit
//) a left outer join GLFinancialTransactions ft on ft.Id=a.GLFinancialTransactionId
// left outer join GLBranchs b on ft.BranchId=b.id
//LEFT OUTER JOIN GLAccounts acc ON a.AccountId = acc.Id
//where transactionAmount not in(0)
//order by dr desc
//
//
//";

//                //if (vm.Id > 0)
//                //{
//                //    sqlText += @" and bde.Id=@Id";
//                //}
//                string cField = "";
//                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
//                {
//                    for (int i = 0; i < conditionFields.Length; i++)
//                    {
//                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
//                        {
//                            continue;
//                        }
//                        cField = conditionFields[i].ToString();
//                        cField = Ordinary.StringReplacing(cField);
//                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
//                    }
//                }

//                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
//                da.SelectCommand.Transaction = transaction;

//                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
//                {
//                    for (int j = 0; j < conditionFields.Length; j++)
//                    {
//                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
//                        {
//                            continue;
//                        }
//                        cField = conditionFields[j].ToString();
//                        cField = Ordinary.StringReplacing(cField);
//                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
//                    }
//                }
//                //if (vm.Id > 0)
//                da.SelectCommand.Parameters.AddWithValue("@GLFinancialTransactionId", vm.GLFinancialTransactionId);

//                da.Fill(dt);
//                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDateTime");
//                if (Vtransaction == null && transaction != null)
//                {
//                    transaction.Commit();
//                }
//                #endregion
//            }
//            #region catch
//            catch (SqlException sqlex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
//            }
//            catch (Exception ex)
//            {
//                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
//            }
//            #endregion
//            #region finally
//            finally
//            {
//                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
//                {
//                    currConn.Close();
//                }
//            }
//            #endregion
//            return dt;
//        }


        #endregion
    }
}
