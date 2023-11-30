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
    public class GLFinancialTransactionDetailDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        #region Charts
        ////==================Single Month - All Heads Amount=================
        public List<GLFinancialTransactionDetailVM> SelectChart1(GLReportVM paramVM, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLFinancialTransactionDetailVM> VMs = new List<GLFinancialTransactionDetailVM>();
            GLFinancialTransactionDetailVM vm;
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
--declare @GLFiscalYearDetailId as varchar(20)
declare @PeriodStart as varchar(20)
declare @PeriodEnd as varchar(20)


--set @GLFiscalYearDetailId = 15

select @PeriodStart=PeriodStart,@PeriodEnd=PeriodEnd from GLFiscalYearDetails
where 1=1
and Id = @GLFiscalYearDetailId

select
distinct ftd.AccountId, acc.Name AccountName,   sum(ftd.TransactionAmount) TransactionAmount
from GLFinancialTransactionDetails ftd
LEFT OUTER JOIN GLAccounts acc ON ftd.AccountId = acc.Id
WHERE  1=1  
and ftd.TransactionDateTime>=@PeriodStart
and ftd.TransactionDateTime<=@PeriodEnd
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

                sqlText += " group by ftd.AccountId, acc.Name";

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
                objComm.Parameters.AddWithValue("@GLFiscalYearDetailId", paramVM.GLFiscalYearDetailId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLFinancialTransactionDetailVM();
                    vm.AccountId = Convert.ToInt32(dr["AccountId"]);
                    vm.TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]);
                    vm.AccountName = dr["AccountName"].ToString();

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

        ////==================Date Range(Multiple Month) - Single Head Amount=================
        public List<GLFinancialTransactionDetailVM> SelectChart2(GLReportVM paramVM, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLFinancialTransactionDetailVM> VMs = new List<GLFinancialTransactionDetailVM>();
            GLFinancialTransactionDetailVM vm;
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
--declare @AccountId as int
--declare @dtFrom as varchar(20)
--declare @dtTo as varchar(20)

--set @dtFrom = '20180201'
--set @dtTo = '20180501'
--set @AccountId = 1

declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @dtFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @dtTo between fd.PeriodStart and fd.PeriodEnd 

select fyd.Id GLFiscalYearDetailId, fyd.PeriodName, isnull(t.TransactionAmount,0)TransactionAmount from GLFiscalYearDetails fyd
left outer join (
select
distinct fyd.Id fydId, sum(ftd.TransactionAmount) TransactionAmount
from GLFinancialTransactionDetails ftd
LEFT OUTER JOIN GLFiscalYearDetails  fyd ON ftd.TransactionDateTime between PeriodStart and PeriodEnd
WHERE  1=1  
and ftd.TransactionDateTime>=@dtFrom
and ftd.TransactionDateTime<=@dtTo
and ftd.AccountId = @AccountId 

group by fyd.Id--, ftd.TransactionDateTime

) as t on t.fydId =  fyd.id
where  fyd.id between @PeriodIdFrom and @PeriodIdTo

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

                objComm.Parameters.AddWithValue("@dtFrom", Ordinary.DateToString(paramVM.DateFrom));
                objComm.Parameters.AddWithValue("@dtTo", Ordinary.DateToString(paramVM.DateTo));
                objComm.Parameters.AddWithValue("@AccountId", paramVM.AccountId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLFinancialTransactionDetailVM();
                    vm.GLFiscalYearDetailId = Convert.ToInt32(dr["GLFiscalYearDetailId"]);
                    vm.TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]);
                    vm.PeriodName = dr["PeriodName"].ToString();

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
        ////==================Date Range(Multiple Year) - Single Head Amount=================
        public List<GLFinancialTransactionDetailVM> SelectChart3(GLReportVM paramVM, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLFinancialTransactionDetailVM> VMs = new List<GLFinancialTransactionDetailVM>();
            GLFinancialTransactionDetailVM vm;
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
--declare @AccountId as int
--declare @dtFrom as varchar(20)
--declare @dtTo as varchar(20)

--set @dtFrom = '20180201'
--set @dtTo = '20180501'
--set @AccountId = 364

declare @YearFrom as varchar(20)
declare @YearTo as varchar(20)

select @YearFrom=Year   from GLFiscalYearDetails fd
where @dtFrom between fd.PeriodStart and fd.PeriodEnd 

select @YearTo=Year   from GLFiscalYearDetails fd
where @dtTo between fd.PeriodStart and fd.PeriodEnd 

select fy.Year,  isnull(t.TransactionAmount,0) TransactionAmount from GLFiscalYears fy
left outer join (
select
distinct fyd.Year Year, sum(ftd.TransactionAmount) TransactionAmount
from GLFinancialTransactionDetails ftd
LEFT OUTER JOIN GLFiscalYearDetails  fyd ON ftd.TransactionDateTime between fyd.PeriodStart and fyd.PeriodEnd
WHERE  1=1  
and ftd.TransactionDateTime>=@dtFrom
and ftd.TransactionDateTime<=@dtTo
and ftd.AccountId = @AccountId 

group by fyd.Year

) as t on t.Year =  fy.Year
where  fy.Year between @YearFrom and @YearTo

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

                objComm.Parameters.AddWithValue("@dtFrom", Ordinary.DateToString(paramVM.DateFrom));
                objComm.Parameters.AddWithValue("@dtTo", Ordinary.DateToString(paramVM.DateTo));
                objComm.Parameters.AddWithValue("@AccountId", paramVM.AccountId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLFinancialTransactionDetailVM();
                    vm.TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]);
                    vm.Year = dr["Year"].ToString();

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

        ////==================Multiple Branch - Total Amount=================
        public List<GLFinancialTransactionDetailVM> SelectChart4(GLReportVM paramVM, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLFinancialTransactionDetailVM> VMs = new List<GLFinancialTransactionDetailVM>();
            GLFinancialTransactionDetailVM vm;
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
--declare @dtFrom as varchar(20)
--declare @dtTo as varchar(20)

--set @dtFrom = '20180201'
--set @dtTo = '20180501'

select br.Name BranchName,  isnull(t.TransactionAmount,0) TransactionAmount from GLBranchs br
left outer join (

select
distinct ftd.BranchId, sum(ftd.TransactionAmount) TransactionAmount
from GLFinancialTransactionDetails ftd
WHERE  1=1  
and ftd.TransactionDateTime>=@dtFrom
and ftd.TransactionDateTime<=@dtTo

group by ftd.BranchId

) as t on t.BranchId =  br.Id


--select sum(TransactionAmount) from GLFinancialTransactionDetails

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

                objComm.Parameters.AddWithValue("@dtFrom", Ordinary.DateToString(paramVM.DateFrom));
                objComm.Parameters.AddWithValue("@dtTo", Ordinary.DateToString(paramVM.DateTo));

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLFinancialTransactionDetailVM();
                    vm.TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]);
                    vm.BranchName = dr["BranchName"].ToString();

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

        #endregion

        ////==================SelectAll=================
        public List<GLFinancialTransactionDetailVM> SelectAllFromPCR(GLFinancialTransactionVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLFinancialTransactionDetailVM> VMs = new List<GLFinancialTransactionDetailVM>();
            GLFinancialTransactionDetailVM vm;
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
SELECT DISTINCT CommissionBillNo,DocumentType,gla.Id AccountId,gla.Code AccountCode,gla.Name AccountName
,SUM(CommissionAmount)TransactionAmount,SUM(AITAmount)TaxAmount,SUM(PCAmount)PCAmount  
FROM(
SELECT   b.CommissionBillNo
,CASE 
WHEN b.DocumentType  IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='FIR') THEN 'FIR'
WHEN b.DocumentType  IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MTR') THEN 'MTR'
WHEN b.DocumentType  IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MISC') THEN 'MISC'
WHEN b.DocumentType  IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='ENG') THEN 'ENG'
WHEN b.DocumentType  IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MAR') THEN 'MAR'
WHEN b.DocumentType  IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MAH') THEN 'MAH'
 ELSE 'NA' END DocumentType,CommissionAmount,AITAmount,PCAmount
 FROM GLPettyCashRequisitionFormBs b 
WHERE 1=1  
AND b.CommissionBillNo = @CommissionBillNo
AND b.BranchId = @BranchId
AND b.IsRejected = 0
) AS a LEFT OUTER JOIN GLAccounts gla ON gla.BusinessClass=a.DocumentType
WHERE    gla.BranchId=@BranchId AND ISNULL(gla.IsBDE,0)=0
GROUP BY CommissionBillNo,DocumentType,gla.Id,gla.Code,gla.Name

";


                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                objComm.Parameters.AddWithValue("@BranchId", paramVM.BranchId);
                objComm.Parameters.AddWithValue("@CommissionBillNo", paramVM.CommissionBillNo);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLFinancialTransactionDetailVM();
                    vm.AccountId = Convert.ToInt32(dr["AccountId"]);
                    vm.TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]);
                    vm.TaxAmount = Convert.ToDecimal(dr["TaxAmount"]);
                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();

                    vm.AccountName = dr["AccountName"].ToString();
                    vm.AccountCode = dr["AccountCode"].ToString();

                    vm.AccountName = vm.AccountCode + " ( " + vm.AccountName + " )";

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

        //==================SelectByMasterId=================
        public List<GLFinancialTransactionDetailVM> SelectByMasterId(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLFinancialTransactionDetailVM> VMs = new List<GLFinancialTransactionDetailVM>();
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
                string[] conditionField = { "fd.GLFinancialTransactionId" };
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
        public List<GLFinancialTransactionDetailVM> SelectById(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLFinancialTransactionDetailVM> VMs = new List<GLFinancialTransactionDetailVM>();
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
        public List<GLFinancialTransactionDetailVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLFinancialTransactionDetailVM> VMs = new List<GLFinancialTransactionDetailVM>();
            GLFinancialTransactionDetailVM vm;
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
,fd.GLFinancialTransactionId
,fd.AccountId
,a.Name AccountName
,a.Code AccountCode
,fd.IsDebit
,fd.TransactionAmount
,ISNULL(fd.VATAmount       ,0)    VATAmount
,ISNULL(fd.TaxAmount       ,0)    TaxAmount

,fd.CommissionBillNo

,fd.TransactionDateTime
,fd.TransactionType
,fd.Post


,fd.Remarks

FROM  GLFinancialTransactionDetails fd 
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
                    vm = new GLFinancialTransactionDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.GLFinancialTransactionId = Convert.ToInt32(dr["GLFinancialTransactionId"]);
                    vm.AccountId = Convert.ToInt32(dr["AccountId"]);
                    vm.IsDebit = Convert.ToBoolean(dr["IsDebit"]);
                    vm.TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]);

                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"]);
                    vm.TaxAmount = Convert.ToDecimal(dr["TaxAmount"]);
                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();

                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.AccountName = dr["AccountName"].ToString();
                    vm.AccountCode = dr["AccountCode"].ToString();

                    vm.AccountName = vm.AccountCode + " ( " + vm.AccountName + " )";

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
        public string[] Insert(GLFinancialTransactionDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert GLFinancialTransactionDetail"; //Method Name
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

                ds = _glCeilingDAL.FindBalance(vm.TransactionDateTime, vm.AccountId.ToString(), currConn, transaction);
                dt = ds.Tables[0];

                DataTable dtCeiling = new DataTable();
                dtCeiling = ds.Tables[1];

                decimal amount = 0;
                if (dtCeiling.Rows.Count > 0)
                {
                    amount = Convert.ToDecimal(dtCeiling.Rows[0]["amount"]);
                }

                if (amount > 0)
                {
                    decimal balance = Convert.ToDecimal(dt.Rows[0]["Balance"]);
                    if (vm.TransactionAmount > balance)
                    {
                        retResults[1] = "Not Enough Balance!";
                        return retResults;
                    }
                }

                #endregion


                vm.Id = _cDal.NextId(" GLFinancialTransactionDetails", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  GLFinancialTransactionDetails(
Id
,BranchId
,GLFinancialTransactionId
,AccountId
,IsDebit
,TransactionAmount
,VATAmount
,TaxAmount
,CommissionBillNo
,TransactionDateTime
,TransactionType
,Post
,Remarks


) VALUES (
@Id
,@BranchId
,@GLFinancialTransactionId
,@AccountId
,@IsDebit
,@TransactionAmount
,@VATAmount
,@TaxAmount
,@CommissionBillNo
,@TransactionDateTime
,@TransactionType
,@Post
,@Remarks
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@GLFinancialTransactionId", vm.GLFinancialTransactionId);
                    cmdInsert.Parameters.AddWithValue("@AccountId", vm.AccountId);
                    cmdInsert.Parameters.AddWithValue("@IsDebit", true);
                    cmdInsert.Parameters.AddWithValue("@TransactionAmount", vm.TransactionAmount);
                    cmdInsert.Parameters.AddWithValue("@VATAmount", vm.VATAmount);
                    cmdInsert.Parameters.AddWithValue("@TaxAmount", vm.TaxAmount);
                    cmdInsert.Parameters.AddWithValue("@CommissionBillNo", vm.CommissionBillNo ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType);

                    cmdInsert.Parameters.AddWithValue("@Post", false);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);

                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update  GLFinancialTransactionDetails.", "");
                    }
                    #endregion SqlExecution

                }
                else
                {
                    retResults[1] = "This  GLFinancialTransactionDetail already used!";
                    throw new ArgumentNullException("Please Input  GLFinancialTransactionDetail Value", "");
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
        public string[] Insert(List<GLFinancialTransactionDetailVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert GLFinancialTransactionDetail"; //Method Name
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

                NextId = _cDal.NextId(" GLFinancialTransactionDetails", currConn, transaction);
                if (VMs != null && VMs.Count > 0)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  GLFinancialTransactionDetails(
Id
,BranchId
,GLFinancialTransactionId
,AccountId
,IsDebit
,TransactionAmount
,VATAmount
,TaxAmount
,CommissionBillNo
,TransactionDateTime
,TransactionType
,Post
,Remarks


) VALUES (
@Id
,@BranchId
,@GLFinancialTransactionId
,@AccountId
,@IsDebit
,@TransactionAmount
,@VATAmount
,@TaxAmount
,@CommissionBillNo
,@TransactionDateTime
,@TransactionType
,@Post
,@Remarks
) 
";

                    #endregion SqlText
                    foreach (GLFinancialTransactionDetailVM vm in VMs)
                    {
                        #region Balance Check

                        ds = _glCeilingDAL.FindBalance(vm.TransactionDateTime, vm.AccountId.ToString(), currConn, transaction);
                        dt = ds.Tables[0];

                        dtCeiling = ds.Tables[1];

                        decimal amount = 0;
                        if (dtCeiling.Rows.Count > 0)
                        {
                            amount = Convert.ToDecimal(dtCeiling.Rows[0]["amount"]);
                        }

                        if (amount > 0)
                        {
                            decimal balance = Convert.ToDecimal(dt.Rows[0]["Balance"]);
                            if (vm.TransactionAmount > balance)
                            {
                                retResults[1] = "Not Enough Balance!";
                                return retResults;
                            }
                        }
                        #endregion
                        #region SqlExecution
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@Id", NextId);
                        cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmdInsert.Parameters.AddWithValue("@GLFinancialTransactionId", vm.GLFinancialTransactionId);
                        cmdInsert.Parameters.AddWithValue("@AccountId", vm.AccountId);
                        cmdInsert.Parameters.AddWithValue("@IsDebit", true);
                        cmdInsert.Parameters.AddWithValue("@TransactionAmount", vm.TransactionAmount);
                        cmdInsert.Parameters.AddWithValue("@VATAmount", vm.VATAmount);
                        cmdInsert.Parameters.AddWithValue("@TaxAmount", vm.TaxAmount);
                        cmdInsert.Parameters.AddWithValue("@CommissionBillNo", vm.CommissionBillNo ?? Convert.DBNull);

                        cmdInsert.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                        cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType);

                        cmdInsert.Parameters.AddWithValue("@Post", false);
                        cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);

                        var exeRes = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update  GLFinancialTransactionDetails.", "");
                        }

                        NextId++;
                        #endregion SqlExecution
                    }
                }
                else
                {
                    retResults[1] = "This  GLFinancialTransactionDetail already used!";
                    throw new ArgumentNullException("Please Input  GLFinancialTransactionDetail Value", "");
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
        public DataTable Report(GLFinancialTransactionDetailVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                currConn = _dbsqlConnection.GetConnectionSageGL();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
select m.Code,m.TransactionDateTime
, (a.Code + ' ( '+ a.Name +' ) ') ExpenseHead
, d.TransactionAmount ExpenseAmount
, ISNULL(d.VATAmount       ,0)    VATAmount
, ISNULL(d.TaxAmount       ,0)    TaxAmount
, ISNULL(d.CommissionBillNo       ,'NA')    CommissionBillNo
, (am.Code + ' ( '+ am.Name +' ) ') CashOrBank
 ,case 
  when m.IsRejected =1 then 'Rejected' 
  when m.IsApprovedL4 =1 then 'Approval Completed' 
  when m.IsApprovedL3 =1 then 'Waiting for Approval Level 4' 
  when m.IsApprovedL2 =1 then 'Waiting for Approval Level 3' 
  when m.IsApprovedL1 =1 then 'Waiting for Approval Level 2' 
 when m.Post=1 then 'Waiting for Approval Level 1' 
 else 'Not Posted' end  Status
 ,br.Name BranchName
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.id=d.GLFinancialTransactionId  
left outer join GLAccounts a on a.id=d.AccountId  
left outer join GLAccounts am on am.id=m.AccountId  
left outer join glBranchs br on br.id=m.BranchId  
WHERE 1=1
";

                if (vm.Status == "Created")
                {
                    sqlText += @" and ( m.IsRejected <> 1 and  m.IsApprovedL4 <> 1 and m.Post<>1)";
                }
                else if (vm.Status == "Posted")
                {
                    sqlText += @" and ( m.IsRejected <> 1 and  m.IsApprovedL4 <> 1 and m.Post=1)";
                }
                else if (vm.Status == "Rejected")
                {
                    sqlText += @" and ( m.IsRejected = 1 and  m.IsApprovedL4 <> 1 and m.Post=1)";
                }
                else if (vm.Status == "Approval Completed")
                {
                    sqlText += @" and ( m.IsRejected <> 1 and  m.IsApprovedL4 = 1 and m.Post=1)";
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
                sqlText += @" ORDER BY m.TransactionDateTime, m.Code";

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
                //if (vm.Id > 0)

                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDateTime");
                dt.Columns["TransactionDateTime"].SetOrdinal(1);
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

        ////==================Report=================
        public DataTable VoucherReport(GLFinancialTransactionDetailVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                currConn = _dbsqlConnection.GetConnectionSageGL();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
--declare @GLFinancialTransactionId as varchar(10)
--set @GLFinancialTransactionId = 5

select acc.Name AccountName, b.Name BranchName,ft.Code,ft.TransactionDateTime, a.* 
,case when a.isDebit=1 then a.TransactionAmount else 0 end Dr
,case when a.isDebit=0 then a.TransactionAmount else 0 end Cr
 from(
select distinct GLFinancialTransactionId,AccountId,IsDebit, ISNULL(Remarks, 'NA')Remarks, sum(TransactionAmount)TransactionAmount  from GLFinancialTransactionDetails
where GLFinancialTransactionId=@GLFinancialTransactionId
group by GLFinancialTransactionId,AccountId,IsDebit,Remarks
union all
select distinct Id, AccountId,IsDebit,Remarks,sum(GrandTotal)TransactionAmount from GLFinancialTransactions
where id in (@GLFinancialTransactionId)
group by Id,AccountId,IsDebit,Remarks
union all
select distinct Id, VATAccountId,0 IsDebit,'' Remarks,sum(VATAmount)TransactionAmount from GLFinancialTransactions
where id in (@GLFinancialTransactionId)
group by Id,VATAccountId,IsDebit
union all

select distinct Id, AITAccountId,0 IsDebit,'' Remarks,sum(TaxAmount)TransactionAmount from GLFinancialTransactions
where id in (@GLFinancialTransactionId)
group by Id,AITAccountId,IsDebit
) a left outer join GLFinancialTransactions ft on ft.Id=a.GLFinancialTransactionId
 left outer join GLBranchs b on ft.BranchId=b.id
LEFT OUTER JOIN GLAccounts acc ON a.AccountId = acc.Id
where transactionAmount not in(0)
order by dr desc


";

                //if (vm.Id > 0)
                //{
                //    sqlText += @" and bde.Id=@Id";
                //}
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
                //if (vm.Id > 0)
                da.SelectCommand.Parameters.AddWithValue("@GLFinancialTransactionId", vm.GLFinancialTransactionId);

                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDateTime");
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
