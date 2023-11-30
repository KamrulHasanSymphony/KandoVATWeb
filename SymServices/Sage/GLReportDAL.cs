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
    public class GLReportDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        // Petty Cash Statement
        ////==================R7 PettyCashStatement=================

        public DataSet Report6(string DateFrom, string DateTo, int BranchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
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
--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--set @DateFrom='20180201'
--set @DateTo='20181130'
--set @BranchId=5

declare @FundReceive as decimal(18,2)

declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

------select @FundReceive=sum(isnull(d.TransactionAmount,0)+isnull(d.VATAmount,0)+isnull(d.TaxAmount,0)) 
------ from GLFinancialTransactionDetails d
------left outer join GLFinancialTransactions m on m.id=d.GLFinancialTransactionId
------where d.BranchId=@BranchId and d.TransactionDateTime<@DateFrom

select  @FundReceive=@FundReceive+sum(isnull(FundReceive,0))   from GLPettyCashRequisitions d
where d.BranchId=@BranchId and d.TransactionDateTime between @DateFrom AND @DateTo

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 

create table #Temp(TransactionDateTime  varchar(200),AccountCode  varchar(200),AccountName  varchar(200), Amount Decimal(18,2))
create table #Temp1(TransactionDateTime  varchar(200),BranchCode  varchar(200),BranchName  varchar(200), Amount Decimal(18,2))
insert into #Temp

select d.TransactionDateTime--distinct CONVERT(date, d.TransactionDateTime, 106) 

,a.Code AccCode,a.Name AccName,sum(isnull(d.TransactionAmount,0))TransactionAmount from GLFinancialTransactionDetails d
left outer join GLAccounts a on d.AccountId=a.Id
where TransactionDateTime between @DateFrom and @DateTo
and d.BranchId=@BranchId
group by d.TransactionDateTime,a.Code ,a.Name 
 
select * from #temp
DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.TransactionDateTime) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT AccountCode,AccountName, ' + @cols + ' from 
            (
                select AccountCode,AccountName
                    , isnull(Amount,0)amount
                    , TransactionDateTime
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(Amount)
                for TransactionDateTime in (' + @cols + ')
            ) p 
			'

execute(@query)
insert into #Temp1
select TransactionDateTime,b.Code BranchCode,b.Name BranchName,FundAmount from GLFundReceivedPettyCashRequisitions d
left outer join GLBranchs b on b.id=d.branchid
where TransactionDateTime between   @DateFrom and @DateTo 
and d.BranchId=@BranchId
 and IsReceived=1
 
SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.TransactionDateTime) 
            FROM #Temp1 c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT BranchCode,BranchName, ' + @cols + ' from 
            (
                select BranchCode,BranchName
                    , isnull(Amount,0)amount
                    , TransactionDateTime
                from #Temp1
				
           ) x
		   
            pivot 
            (
                 sum(Amount)
                for TransactionDateTime in (' + @cols + ')
            ) p 
			'


execute(@query)

 
 
";

                sqlText = sqlText + " select 'OpeningBalance'OpeningBalance,'OpeningBalance'Particular, @FundReceive  '" + Ordinary.DateToString(DateFrom) + "'";
                sqlText = sqlText + @" 

drop table #temp
drop table #temp1";
                if (BranchId <= 0)
                    sqlText = sqlText.Replace("d.BranchId=@BranchId", "1=1");

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(DateTo));
                if (BranchId > 0)
                    da.SelectCommand.Parameters.AddWithValue("@BranchId", BranchId);

                da.Fill(ds);
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
            return ds;
        }
        ////==================R7 BusinessDevelopmentExpenseStatement=================
        public DataSet Report7(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
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
--R7--Business Development Expense Statement

--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--set @DateFrom='20180201'
--set @DateTo='20181130'
--set @BranchId=5


declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 

select  MRNo, MRDate, DocumentNo 
,case when DocumentType IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='FIR') then NetPremium else 0 end  FIR
,case when DocumentType IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MAR') then NetPremium else 0 end  MAR
,case when DocumentType IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MAH') then NetPremium else 0 end  MAH
,case when DocumentType IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MTR') then NetPremium else 0 end  MTR
,case when DocumentType IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MISC') then NetPremium else 0 end MISC
,case when DocumentType IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='ENG') then NetPremium else 0 end  ENG
,BDE
,AIT
,case when DocumentType IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='FIR') then PaidAmount else 0 end  BDE_FIR
,case when DocumentType IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MAR') then PaidAmount else 0 end  BDE_MAR
,case when DocumentType IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MAH') then PaidAmount else 0 end  BDE_MAH
,case when DocumentType IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MTR') then PaidAmount else 0 end  BDE_MTR
,case when DocumentType IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='MISC') then PaidAmount else 0 end BDE_MISC
,case when DocumentType IN(select distinct SubClassName from GLEnumBusinessClasses where ClassName='ENG') then PaidAmount else 0 end  BDE_ENG
 from GLBDERequisitionFormAs
where PaymentDate   between @DateFrom and @DateTo

select sum(Opening)Opening
,sum(ReceiveFromHO)ReceiveFromHO
,sum(PCRecovery)PCRecovery
,sum(BDEAmount)BDEAmount
,sum(Salary)Salary
,sum(BankCharge)BankCharge
 from (
select distinct 0 Id,'Opening' PeriodName
,isnull(sum(fund.FundAmount),0)  +isnull(sum(bdee.PCRecovery),0)
-( isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)) Opening
,0 ReceiveFromHO
,0 PCRecovery
,0 BDEAmount
,0 Salary
,0 BankCharge

 from GLFiscalYearDetails fd
 left outer join(
 select fd.Id FYId, (m.FundAmount)FundAmount from GLFundReceivedBDERequisitions as m
left outer join GLFiscalYearDetails fd on m.ReceivedAt between fd.PeriodStart and fd.PeriodEnd
where m.IsReceived=1  
and m.ReceivedAt<@DateFrom
and m.BranchId=@BranchId
 ) as fund  on fund.FYId=fd.id
left outer join (select fd.Id FYId, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
left outer  join (
select fd.Id FYId, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
) as ace on  ace.FYId=fd.id
left outer  join (
select fd.Id FYId, (a.PaidAmount)BDEAmount  
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on a.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and a.IsRejected=0  and a.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
 )bdea on  bdea.FYId=fd.id

left outer  join(
select fd.Id FYId, (b.PaidAmount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
 )bdeb on  bdeb.FYId=fd.id
left outer  join (
select fd.Id FYId, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and c.IsRejected=0  and  c.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
 )bdec on  bdec.FYId=fd.id
 left outer  join(
 select fd.Id FYId, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and d.IsRejected=0  and d.IsFundReceived=1
and m.TransactionDateTime<@DateFrom

and m.BranchId=@BranchId
)bded on  bded.FYId=fd.id

left outer  join (
select fd.Id FYId, (e.PaidAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on e.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and e.IsRejected=0  and  e.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
)bdee on  bdee.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo
group by fd.Id,fd.PeriodName

union all

select distinct fd.Id,fd.PeriodName
, 0 Opening ,isnull(sum(bded.ContingencyFund),0) + isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)  ReceiveFromHO
,isnull(sum(bdee.PCRecovery),0)PCRecovery
,  isnull(sum(bdea.BDEAmount),0) BDEAmount,
isnull(sum(bdeb.Salary),0)Salary,
isnull(sum(bdec.BankCharge),0)  BankCharge
 
from GLFiscalYearDetails fd
left outer join (select fd.Id FYId, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
left outer  join (
select fd.Id FYId, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.BranchId=@BranchId
) as ace on  ace.FYId=fd.id
left outer  join (
select fd.Id FYId, (a.PaidAmount)BDEAmount  
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on a.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and a.IsRejected=0  and  a.IsFundReceived=1
and m.BranchId=@BranchId
 )bdea on  bdea.FYId=fd.id

left outer  join(
select fd.Id FYId, (b.PaidAmount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.BranchId=@BranchId
 )bdeb on  bdeb.FYId=fd.id
left outer  join (
select fd.Id FYId, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and c.IsRejected=0  and  c.IsFundReceived=1
and m.BranchId=@BranchId
 )bdec on  bdec.FYId=fd.id
 left outer  join(
 select fd.Id FYId, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and d.IsRejected=0  and  d.IsFundReceived=1
and m.BranchId=@BranchId
)bded on  bded.FYId=fd.id

left outer  join (
select fd.Id FYId, (e.PaidAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on e.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and e.IsRejected=0  and  e.IsFundReceived=1
and m.BranchId=@BranchId
)bdee on  bdee.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo
group by fd.Id,fd.PeriodName
) as a


 
 
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));

                da.Fill(ds);
                //dt.Columns.Remove("PeriodSl");
                //dt.Columns.Add("Month", typeof(string));
                //dt.Columns["Month"].SetOrdinal(0);
                //for (int i = 0; i < 12; i++)
                //{
                //    dt.Rows[i]["Month"] = Ordinary.MonthNames[i];
                //}

                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return ds;
        }
        ////==================R8 YearComparisonBranchExpenseStatement=================
        public DataTable Report8(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R8--Year-Wise Comparison of Branch Expense Statement


--declare @GLFiscalYearIdTo as varchar(20)
--declare @BranchId as varchar(20)
--set @GLFiscalYearIdTo=2
--set @BranchId=5

declare @GLFiscalYearIdFrom as varchar(20)

select @GLFiscalYearIdFrom=id from GLFiscalYears where id<@GLFiscalYearIdTo
select @GLFiscalYearIdFrom=isnull(@GLFiscalYearIdFrom,0)

create table #Temp(YearName  varchar(200),PeriodSl  varchar(200), Amount Decimal(18,2))

insert into #Temp
select distinct fd.Year, SUBSTRING(fd.PeriodSl,1,2)
,isnull(sum(be.BranchExpense),0)  TotalBDEExpense
  from GLFiscalYearDetails fd
left outer join (select  d.TransactionDateTime, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 ) as be on be.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
 
where   fd.GLFiscalYearId between @GLFiscalYearIdFrom and @GLFiscalYearIdTo  
group by fd.year,SUBSTRING(fd.PeriodSl,1,2)
order by  SUBSTRING(fd.PeriodSl,1,2) asc ,YEAR desc

--select * from #temp
DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.YearName) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT PeriodSl, ' + @cols + ' from 
            (
                select PeriodSl
                    , isnull(Amount,0)amount
                    , YearName
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(Amount)
                for YearName in (' + @cols + ')
            ) p 
			'

execute(@query)
drop table #temp
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@GLFiscalYearIdTo", vm.GLFiscalYearId);

                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Add("Month", typeof(string));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int index = Array.IndexOf(Ordinary.Alphabet, dt.Rows[i]["PeriodSl"]);
                        if (index == -1)
                        {
                            dt.Rows[i]["Month"] = dt.Rows[i]["PeriodSl"];
                        }
                        else
                        {
                            dt.Rows[i]["Month"] = Ordinary.MonthNames[index];
                        }
                    }
                    dt.Columns["Month"].SetOrdinal(0);
                    dt.Columns.Remove("PeriodSl");
                }


                //dt.Columns[1].SetOrdinal(2);

                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================R9 PettyCashExpenseStatement=================
        public DataTable Report9(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R9--PettyCash Expense Statement

--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--set @DateFrom='20180101'
--set @DateTo='20181130'
--set @BranchId=5

declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 
 
create table #Temp(PeriodSl  varchar(200),AccountHead  varchar(200), Amount Decimal(18,2))
insert into #Temp
select distinct fd.PeriodSl,be.AccountHead
, isnull(sum(be.BranchExpense),0)   Payment
 
 from GLFiscalYearDetails fd
 
left outer join (select fd.Id FYId,ac.Name AccountHead, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on d.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer join GLAccounts ac on d.AccountId= ac.id
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
 
where fd.id between @PeriodIdFrom and @PeriodIdTo
and be.AccountHead is not null
group by  fd.PeriodSl,be.AccountHead
order by fd.PeriodSl  asc  
--select * from #Temp

DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.PeriodSl) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT AccountHead, ' + @cols + ' from 
            (
                select AccountHead
                    , isnull(Amount,0)amount
                    , PeriodSl
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(Amount)
                for PeriodSl in (' + @cols + ')
            ) p 
			'

execute(@query)
drop table #temp
 
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));

                da.Fill(dt);
                //dt.Columns.Remove("PeriodSl");
                //dt.Columns.Add("Month", typeof(string));
                //dt.Columns["Month"].SetOrdinal(0);
                //for (int i = 0; i < 12; i++)
                //{
                //    dt.Rows[i]["Month"] = Ordinary.MonthNames[i];
                //}

                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================R10 PettyCashReceiptPaymentStatement=================
        public DataTable Report10(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R10--PettyCash Receipt and Payment Statement

--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--set @DateFrom='20180201'
--set @DateTo='20181130'
--set @BranchId=5

declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 

select * from (
select distinct 0 Id,'Opening' PeriodSl
,isnull(sum(fund.FundAmount),0)
-( isnull(sum(be.BranchExpense),0) +isnull(sum(ace.AgencyComExpense),0)) Opening
,0 ReceiveFromHO
,0 Payment
 
 from GLFiscalYearDetails fd
 left outer join(
 select fd.Id FYId, (m.FundAmount)FundAmount from GLFundReceivedPettyCashRequisitions as m
left outer join GLFiscalYearDetails fd on m.ReceivedAt between fd.PeriodStart and fd.PeriodEnd
where m.IsReceived=1  
and m.ReceivedAt<@DateFrom
and m.BranchId=@BranchId
 ) as fund  on fund.FYId=fd.id
left outer join (select fd.Id FYId, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
left outer  join (
select fd.Id FYId, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
) as ace on  ace.FYId=fd.id

where fd.id between @PeriodIdFrom and @PeriodIdTo
group by fd.Id,fd.PeriodSl

union all

select distinct fd.Id,fd.PeriodName
, 0 Opening ,isnull(sum(fund.FundAmount),0)    ReceiveFromHO
, isnull(sum(be.BranchExpense),0)+isnull(sum(ace.AgencyComExpense),0)  Payment
 
  from GLFiscalYearDetails fd
   left outer join(
 select fd.Id FYId, (m.FundAmount)FundAmount from GLFundReceivedPettyCashRequisitions as m
left outer join GLFiscalYearDetails fd on m.ReceivedAt between fd.PeriodStart and fd.PeriodEnd
where m.IsReceived=1  
and m.ReceivedAt  between fd.PeriodStart and fd.PeriodEnd
and m.BranchId=@BranchId
 ) as fund  on fund.FYId=fd.id

left outer join (select fd.Id FYId, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
left outer  join (
select fd.Id FYId, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.BranchId=@BranchId
) as ace on  ace.FYId=fd.id
 
where fd.id between @PeriodIdFrom and @PeriodIdTo
group by fd.Id,fd.PeriodName
) as a
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));

                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Remove("Id");
                    dt.Columns.Add("Balance", typeof(decimal));
                }
                decimal Opening = 0;
                decimal ReceiveFromHO = 0;
                decimal Payment = 0;
                decimal Balance = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //O = 0;
                    ReceiveFromHO = 0;
                    Payment = 0;
                    //B = 0;

                    if (i == 0)
                    {
                        Opening = Convert.ToDecimal(dt.Rows[i]["Opening"]);
                    }
                    else
                    {
                        dt.Rows[i]["Opening"] = Opening;
                    }
                    ReceiveFromHO = Convert.ToDecimal(dt.Rows[i]["ReceiveFromHO"]);
                    Payment = Convert.ToDecimal(dt.Rows[i]["Payment"]);
                    Balance = Opening + ReceiveFromHO - Payment;
                    Opening = Balance;

                    dt.Rows[i]["Balance"] = Balance;
                }



                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================R11 AgentCommissionStatement=================
        public DataTable Report11(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R11--Agent Commission Statement

--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--set @DateFrom='20180101'
--set @DateTo='20181130'
--set @BranchId=5
 
declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 
create table #Temp(PeriodSl  varchar(200),DocumentType  varchar(200), BDEAmount Decimal(18,2))
 insert into #Temp
 select * from
 (
select distinct   fd.PeriodSl, bdeb.DocumentType,
 isnull(sum(bdeb.PaidAmount),0) BDEAmount
  from GLFiscalYearDetails fd
left outer  join(
select fd.Id FYId,  
 DocumentType,PaidAmount   from GLBDERequisitionFormAs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.BranchId=@BranchId
 
 )bdeb on  bdeb.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo  
group by  fd.PeriodSl , bdeb.DocumentType

  union all
  select distinct   fd.PeriodSl, bdeb.DocumentType,
 -isnull(sum(bdeb.PaidAmount),0) BDEAmount
  from GLFiscalYearDetails fd
left outer  join(
select fd.Id FYId,  
 DocumentType,PaidAmount   from GLBDERequisitionFormEs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.BranchId=@BranchId
 
 )bdeb on  bdeb.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo  
group by  fd.PeriodSl , bdeb.DocumentType


 ) as a


--select * from #Temp

DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.DocumentType) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT PeriodSl, ' + @cols + ' from 
            (
                select PeriodSl
                    , isnull(BDEAmount,0)BDEAmount
                    , DocumentType
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(BDEAmount)
                for DocumentType in (' + @cols + ')
            ) p 
			'

execute(@query)

drop table #Temp
 
 
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));

                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Add("Month", typeof(string));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int index = Array.IndexOf(Ordinary.Alphabet, dt.Rows[i]["PeriodSl"]);
                        if (index == -1)
                        {
                            dt.Rows[i]["Month"] = dt.Rows[i]["PeriodSl"];
                        }
                        else
                        {
                            dt.Rows[i]["Month"] = Ordinary.MonthNames[index];
                        }
                    }
                    dt.Columns["Month"].SetOrdinal(0);
                    dt.Columns.Remove("PeriodSl");
                }


                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================R12 BusinessDevelopmentExpenseStatement=================
        public DataTable Report12(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R12--Business Development Expense Statement

--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--set @DateFrom='20180101'
--set @DateTo='20181130'
--set @BranchId=5
 
declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 
create table #Temp(PeriodSl  varchar(200),DocumentType  varchar(200), BDEAmount Decimal(18,2))
 insert into #Temp
 select * from
 (
select distinct   fd.Periodsl, bdeb.DocumentType,
 isnull(sum(bdeb.BDEAmount),0) BDEAmount
  from GLFiscalYearDetails fd
left outer  join(
select fd.Id FYId,  
 DocumentType,PaidAmount BDEAmount  from GLBDERequisitionFormAs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.BranchId=@BranchId
 
 )bdeb on  bdeb.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo  
group by  fd.Periodsl , bdeb.DocumentType

  union all
  select distinct   fd.PeriodSl, bdeb.DocumentType,
 -isnull(sum(bdeb.BDEAmount),0) BDEAmount
  from GLFiscalYearDetails fd
left outer  join(
select fd.Id FYId,  
 DocumentType,PaidAmount BDEAmount  from GLBDERequisitionFormEs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.BranchId=@BranchId
 
 )bdeb on  bdeb.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo  
group by  fd.PeriodSl , bdeb.DocumentType

union all
select distinct   fd.PeriodSl, bdeb.DocumentType,
 isnull(sum(bdeb.BDEAmount),0) BDEAmount
  from GLFiscalYearDetails fd
left outer  join(
select fd.Id FYId,  
 'EmployeeSalary'DocumentType,PaidAmount BDEAmount  from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.BranchId=@BranchId
 
 )bdeb on  bdeb.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo  
group by  fd.PeriodSl , bdeb.DocumentType
union all
select distinct   fd.Periodsl, bdeb.DocumentType,
 isnull(sum(bdeb.BDEAmount),0) BDEAmount
  from GLFiscalYearDetails fd
left outer  join(
select fd.Id FYId,  
 'BankCharge'DocumentType,Amount BDEAmount  from GLBDERequisitionFormCs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.BranchId=@BranchId
 
 )bdeb on  bdeb.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo  
group by  fd.Periodsl , bdeb.DocumentType

 ) as a
 



--select * from #Temp

DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.DocumentType) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT PeriodSl, ' + @cols + ' from 
            (
                select PeriodSl
                    , isnull(BDEAmount,0)BDEAmount
                    , DocumentType
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(BDEAmount)
                for DocumentType in (' + @cols + ')
            ) p 
			'

execute(@query)

drop table #Temp
 
 
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));

                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Add("Month", typeof(string));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int index = Array.IndexOf(Ordinary.Alphabet, dt.Rows[i]["PeriodSl"]);
                        if (index == -1)
                        {
                            dt.Rows[i]["Month"] = dt.Rows[i]["PeriodSl"];
                        }
                        else
                        {
                            dt.Rows[i]["Month"] = Ordinary.MonthNames[index];
                        }
                    }
                    dt.Columns["Month"].SetOrdinal(0);
                    dt.Columns.Remove("PeriodSl");
                }

                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================R13 BusinessDevelopmentEmployeeSalaryStatement=================
        public DataTable Report13(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R13-BusinessDevelopmentEmployeeSalaryStatement

--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--declare @EmployeeId as varchar(20)
--set @DateFrom='20180101'
--set @DateTo='20181130'
--set @BranchId=5
--set @EmployeeId=1
declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 
create table #Temp(PeriodName  varchar(200),EmployeeName  varchar(200), Salary Decimal(18,2))
 insert into #Temp
select distinct  fd.PeriodName, bdeb.Name EmployeeName,
 isnull(sum(bdeb.Salary),0) Salary
  from GLFiscalYearDetails fd
left outer  join(
select fd.Id FYId, em.Name, (b.PaidAmount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
left outer join GLEmployees em on em.id=b.EmployeeId
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.BranchId=@BranchId
and em.Id=@EmployeeId
 )bdeb on  bdeb.FYId=fd.id
 
where fd.id between @PeriodIdFrom and @PeriodIdTo and bdeb.Name is not null
group by  fd.PeriodName ,bdeb.Name


--select * from #Temp

DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.PeriodName) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT EmployeeName, ' + @cols + ' from 
            (
                select EmployeeName
                    , isnull(Salary,0)Salary
                    , PeriodName
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(Salary)
                for PeriodName in (' + @cols + ')
            ) p 
			'

execute(@query)

drop table #Temp
 
 
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));

                da.Fill(dt);
                //dt.Columns.Remove("PeriodSl");
                //dt.Columns.Add("Month", typeof(string));
                //dt.Columns["Month"].SetOrdinal(0);
                //for (int i = 0; i < 12; i++)
                //{
                //    dt.Rows[i]["Month"] = Ordinary.MonthNames[i];
                //}

                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================R14 BusinessDevelopmentExpenseReceiptPaymentStatement=================
        public DataTable Report14(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R14--PettyCash Expense Statement
--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--set @DateFrom='20180201'
--set @DateTo='20191130'
--set @BranchId=5


declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 

select * from (
select distinct 0 Id,'Opening' PeriodName
,isnull(sum(fund.FundAmount),0)  +isnull(sum(bdee.PCRecovery),0)
-( isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)) Opening
,0 ReceiveFromHO
,0 PCRecovery
,0 Payment
 
 from GLFiscalYearDetails fd
 left outer join(
 select fd.Id FYId, (m.FundAmount)FundAmount from GLFundReceivedBDERequisitions as m
left outer join GLFiscalYearDetails fd on m.ReceivedAt between fd.PeriodStart and fd.PeriodEnd
where m.IsReceived=1  
and m.ReceivedAt<@DateFrom
and m.BranchId=@BranchId
 ) as fund  on fund.FYId=fd.id
left outer join (select fd.Id FYId, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
left outer  join (
select fd.Id FYId, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
) as ace on  ace.FYId=fd.id
left outer  join (
select fd.Id FYId, (a.PaidAmount)BDEAmount  
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on a.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and a.IsRejected=0  and a.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
 )bdea on  bdea.FYId=fd.id

left outer  join(
select fd.Id FYId, (b.PaidAmount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
 )bdeb on  bdeb.FYId=fd.id
left outer  join (
select fd.Id FYId, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and c.IsRejected=0  and  c.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
 )bdec on  bdec.FYId=fd.id
 left outer  join(
 select fd.Id FYId, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and d.IsRejected=0  and d.IsFundReceived=1
and m.TransactionDateTime<@DateFrom

and m.BranchId=@BranchId
)bded on  bded.FYId=fd.id

left outer  join (
select fd.Id FYId, (e.PaidAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on e.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and e.IsRejected=0  and  e.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
)bdee on  bdee.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo
group by fd.Id,fd.PeriodName

union all

select distinct fd.Id,fd.Periodname
, 0 Opening ,isnull(sum(bded.ContingencyFund),0) + isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)  ReceiveFromHO
,isnull(sum(bdee.PCRecovery),0)PCRecovery
,  isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)  Payment
 
from GLFiscalYearDetails fd
left outer join (select fd.Id FYId, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
left outer  join (
select fd.Id FYId, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.BranchId=@BranchId
) as ace on  ace.FYId=fd.id
left outer  join (
select fd.Id FYId, (a.PaidAmount)BDEAmount  
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on a.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and a.IsRejected=0  and  a.IsFundReceived=1
and m.BranchId=@BranchId
 )bdea on  bdea.FYId=fd.id

left outer  join(
select fd.Id FYId, (b.PaidAmount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.BranchId=@BranchId
 )bdeb on  bdeb.FYId=fd.id
left outer  join (
select fd.Id FYId, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and c.IsRejected=0  and  c.IsFundReceived=1
and m.BranchId=@BranchId
 )bdec on  bdec.FYId=fd.id
 left outer  join(
 select fd.Id FYId, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and d.IsRejected=0  and  d.IsFundReceived=1
and m.BranchId=@BranchId
)bded on  bded.FYId=fd.id

left outer  join (
select fd.Id FYId, (e.PaidAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on e.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and e.IsRejected=0  and  e.IsFundReceived=1
and m.BranchId=@BranchId
)bdee on  bdee.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo
group by fd.Id,fd.Periodname
) as a  order by id
 
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));

                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Remove("Id");
                    dt.Columns.Add("Balance", typeof(decimal));
                }
                decimal Opening = 0;
                decimal ReceiveFromHO = 0;
                decimal Payment = 0;
                decimal PCRecovery = 0;
                decimal Balance = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //O = 0;
                    ReceiveFromHO = 0;
                    PCRecovery = 0;
                    Payment = 0;
                    //B = 0;

                    if (i == 0)
                    {
                        Opening = Convert.ToDecimal(dt.Rows[i]["Opening"]);
                    }
                    else
                    {
                        dt.Rows[i]["Opening"] = Opening;
                    }
                    ReceiveFromHO = Convert.ToDecimal(dt.Rows[i]["ReceiveFromHO"]);
                    PCRecovery = Convert.ToDecimal(dt.Rows[i]["PCRecovery"]);
                    Payment = Convert.ToDecimal(dt.Rows[i]["Payment"]);
                    Balance = Opening + ReceiveFromHO + PCRecovery - Payment;
                    Opening = Balance;

                    dt.Rows[i]["Balance"] = Balance;
                }

                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ///==================R15 YearTotalExpenseStatement=================
        public DataTable Report15(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R15--YearTotalExpenseStatement

--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--set @DateFrom='20180201'
--set @DateTo='20181130'
--set @BranchId=5

declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 
 
select *
  from (
select distinct 0 Id,'Opening' PeriodName
,isnull(sum(fund.FundAmount),0)  
-( isnull(sum(be.BranchExpense),0) +isnull(sum(ace.AgencyComExpense),0) ) Opening
,0 ReceiveFundAmount
,0 BranchExpense
,0 AgencyComExpense
 
 
 from GLFiscalYearDetails fd

 left outer join(
 select fd.Id FYId, (m.FundAmount)FundAmount from GLFundReceivedPettyCashRequisitions as m
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsReceived=1  
and m.ReceivedAt<@DateFrom
and m.BranchId=@BranchId
 ) as fund  on fund.FYId=fd.id
left outer join (select fd.Id FYId, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
left outer  join (
select fd.Id FYId, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.TransactionDateTime<@DateFrom
and m.BranchId=@BranchId
) as ace on  ace.FYId=fd.id
 
where fd.id between @PeriodIdFrom and @PeriodIdTo
group by fd.Id,fd.PeriodName

union all

select distinct fd.Id,fd.PeriodName
, 0 Opening  
,isnull(sum(fund.ReceiveFundAmount),0)ReceiveFundAmount
,isnull(sum(be.BranchExpense),0)BranchExpense
,isnull(sum(ace.AgencyComExpense),0)AgencyComExpense
 
from GLFiscalYearDetails fd
left outer join(
 select fd.Id FYId, (m.FundAmount)ReceiveFundAmount from GLFundReceivedPettyCashRequisitions as m
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsReceived=1  
and m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
and m.BranchId=@BranchId
 ) as fund  on fund.FYId=fd.id

left outer join (select fd.Id FYId, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
left outer  join (
select fd.Id FYId, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and b.IsRejected=0  and  b.IsFundReceived=1
and m.BranchId=@BranchId
) as ace on  ace.FYId=fd.id
 
where fd.id between @PeriodIdFrom and @PeriodIdTo
group by fd.Id,fd.PeriodName
) as a order by id



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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));

                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Remove("Id");
                    dt.Columns.Remove("Opening");
                }

                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================R16 YearTotalExpenseStatement2=================
        public DataTable Report16(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R16--YearTotalExpenseStatement2

--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--set @DateFrom='20170201'
--set @DateTo='20191230'
--set @BranchId=5

declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 

select distinct fd.Year
,isnull(sum(be.BranchExpense),0)BranchExpense
,isnull(sum(ace.AgencyComExpense),0)AgencyComExpense
,isnull(sum(bdea.BDEAmount),0)BDEAmount
,isnull(sum(bdeb.Salary),0)BDEEmployeeSalary
,isnull(sum(bdec.BankCharge),0)BDEBankCharge
,isnull(sum(bdee.PCRecovery),0)PCRecovery
,isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)-isnull(sum(bdee.PCRecovery),0) TotalBDEExpense
,isnull(sum(be.BranchExpense),0)+isnull(sum(ace.AgencyComExpense),0)+isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)-isnull(sum(bdee.PCRecovery),0) TotalExpense
  from GLFiscalYearDetails fd
left outer join (select fd.Id FYId, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
left outer  join (
select fd.Id FYId, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and B.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
) as ace on  ace.FYId=fd.id
left outer  join (
select fd.Id FYId, (a.PaidAmount)BDEAmount  
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on a.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and A.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdea on  bdea.FYId=fd.id

left outer  join(
select fd.Id FYId, (b.PaidAmount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and B.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdeb on  bdeb.FYId=fd.id
left outer  join (
select fd.Id FYId, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and C.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdec on  bdec.FYId=fd.id
 left outer  join(
 select fd.Id FYId, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1  and m.IsRejected=0 and d.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
)bded on  bded.FYId=fd.id

left outer  join (
select fd.Id FYId, (e.PaidAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on e.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1  and m.IsRejected=0 and E.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
)bdee on  bdee.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo
group by fd.year
order by year
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));

                da.Fill(dt);
                //dt.Columns.Remove("PeriodSl");
                //dt.Columns.Add("Month", typeof(string));
                //dt.Columns["Month"].SetOrdinal(0);
                //for (int i = 0; i < 12; i++)
                //{
                //    dt.Rows[i]["Month"] = Ordinary.MonthNames[i];
                //}

                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================R17 YearComparisonBranchExpenseStatement2=================
        public DataTable Report17(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R17--Year-Wise Comparison of Branch Expense Statement2

--
--declare @GLFiscalYearIdTo as varchar(20)
--declare @BranchId as varchar(20)
--set @GLFiscalYearIdTo=2
--set @BranchId=5

select @GLFiscalYearIdFrom=id from GLFiscalYears where id<@GLFiscalYearIdTo
select @GLFiscalYearIdFrom=isnull(@GLFiscalYearIdFrom,0)

create table #Temp(YearName  varchar(200),PeriodSl  varchar(200), Amount Decimal(18,2))

insert into #Temp
select distinct fd.Year, SUBSTRING(fd.PeriodSl,1,2)
,isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)-isnull(sum(bdee.PCRecovery),0) TotalBDEExpense

  from GLFiscalYearDetails fd
 
left outer join (select  d.TransactionDateTime, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 ) as be on be.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select b.TransactionDateTime, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
) as ace on  ace.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select  a.PaymentDate, (a.PaidAmount)BDEAmount  
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0  
and m.BranchId=@BranchId
 )bdea on  bdea.PaymentDate between fd.PeriodStart and fd.PeriodEnd

left outer  join(
select b.PaymentDate, (b.PaidAmount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 )bdeb on  bdeb.PaymentDate between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select c.TransactionDateTime, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId

 )bdec on  bdec.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

 left outer  join(
 select d.TransactionDateTime, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
)bded on  bded.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

left outer  join (
select e.PaymentDate, (e.PaidAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
)bdee on  bdee.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where   fd.GLFiscalYearId between @GLFiscalYearIdFrom and @GLFiscalYearIdTo  
group by fd.year,SUBSTRING(fd.PeriodSl,1,2)
order by  SUBSTRING(fd.PeriodSl,1,2) asc ,YEAR desc

--select * from #temp
DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.YearName) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT PeriodSl, ' + @cols + ' from 
            (
                select PeriodSl
                    , isnull(Amount,0)amount
                    , YearName
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(Amount)
                for YearName in (' + @cols + ')
            ) p 
			'

execute(@query)
drop table #temp
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@GLFiscalYearIdTo", vm.GLFiscalYearId);

                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Add("Month", typeof(string));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int index = Array.IndexOf(Ordinary.Alphabet, dt.Rows[i]["PeriodSl"]);
                        if (index == -1)
                        {
                            dt.Rows[i]["Month"] = dt.Rows[i]["PeriodSl"];
                        }
                        else
                        {
                            dt.Rows[i]["Month"] = Ordinary.MonthNames[index];
                        }
                    }
                    dt.Columns["Month"].SetOrdinal(0);
                    dt.Columns.Remove("PeriodSl");
                }
                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================R18 YearComparisonAgentCommissionExpenseStatement=================
        public DataTable Report18(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R18--YearComparisonAgentCommissionExpenseStatement

--
--declare @GLFiscalYearIdTo as varchar(20)
--declare @BranchId as varchar(20)
--set @GLFiscalYearIdTo=2
--set @BranchId=5
declare @GLFiscalYearIdFrom as varchar(20)

select @GLFiscalYearIdFrom=id from GLFiscalYears where id<@GLFiscalYearIdTo
select @GLFiscalYearIdFrom=isnull(@GLFiscalYearIdFrom,0)

create table #Temp(YearName  varchar(200),PeriodSl  varchar(200), Amount Decimal(18,2))

insert into #Temp
select distinct fd.Year, SUBSTRING(fd.PeriodSl,1,2)
, isnull(sum(ace.AgencyComExpense),0)+isnull(sum(bdea.BDEAmount),0)   -isnull(sum(bdee.PCRecovery),0) TotalExpense

  from GLFiscalYearDetails fd
 
left outer join (select  d.TransactionDateTime, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 ) as be on be.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select b.TransactionDateTime, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
) as ace on  ace.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select  a.PaymentDate, (a.PaidAmount)BDEAmount  
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 )bdea on  bdea.PaymentDate between fd.PeriodStart and fd.PeriodEnd

left outer  join(
select b.PaymentDate, (b.PaidAmount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 )bdeb on  bdeb.PaymentDate between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select c.TransactionDateTime, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId

 )bdec on  bdec.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

 left outer  join(
 select d.TransactionDateTime, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
)bded on  bded.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

left outer  join (
select e.PaymentDate, (e.PaidAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
)bdee on  bdee.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where   fd.GLFiscalYearId between @GLFiscalYearIdFrom and @GLFiscalYearIdTo  
group by fd.year,SUBSTRING(fd.PeriodSl,1,2)
order by  SUBSTRING(fd.PeriodSl,1,2) asc ,YEAR desc

--select * from #temp
DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.YearName) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT PeriodSl, ' + @cols + ' from 
            (
                select PeriodSl
                    , isnull(Amount,0)amount
                    , YearName
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(Amount)
                for YearName in (' + @cols + ')
            ) p 
			'

execute(@query)
drop table #temp
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@GLFiscalYearIdTo", vm.GLFiscalYearId);

                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Add("Month", typeof(string));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int index = Array.IndexOf(Ordinary.Alphabet, dt.Rows[i]["PeriodSl"]);
                        if (index == -1)
                        {
                            dt.Rows[i]["Month"] = dt.Rows[i]["PeriodSl"];
                        }
                        else
                        {
                            dt.Rows[i]["Month"] = Ordinary.MonthNames[index];
                        }
                    }
                    dt.Columns["Month"].SetOrdinal(0);
                    dt.Columns.Remove("PeriodSl");
                }
                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================R19 YearComparisonBusinessDevelopmentExpenseStatement=================
        public DataTable Report19(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R8--Year-Wise Comparison of BusinessDevelopment Expense Statement

--declare @GLFiscalYearIdTo as varchar(20)
--declare @BranchId as varchar(20)
--set @GLFiscalYearIdTo=4
--set @BranchId=5

declare @GLFiscalYearIdFrom as varchar(20)

select @GLFiscalYearIdFrom=id from GLFiscalYears where id<@GLFiscalYearIdTo
select @GLFiscalYearIdFrom=isnull(@GLFiscalYearIdFrom,0)

create table #Temp(YearName  varchar(200),PeriodSl  varchar(200), Amount Decimal(18,2))

insert into #Temp
select distinct fd.Year, SUBSTRING(fd.PeriodSl,1,2)
, isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)-isnull(sum(bdee.PCRecovery),0) TotalExpense

  from GLFiscalYearDetails fd
 
left outer join (select  d.TransactionDateTime, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
where m.IsApprovedL4=1 and m.IsRejected=0

and m.BranchId=@BranchId
 ) as be on be.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select b.TransactionDateTime, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
) as ace on  ace.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select  a.PaymentDate, (a.PaidAmount)BDEAmount  
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 )bdea on  bdea.PaymentDate between fd.PeriodStart and fd.PeriodEnd

left outer  join(
select b.PaymentDate, (b.PaidAmount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 )bdeb on  bdeb.PaymentDate between fd.PeriodStart and fd.PeriodEnd
left outer  join (
select c.TransactionDateTime, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId

 )bdec on  bdec.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

 left outer  join(
 select d.TransactionDateTime, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
)bded on  bded.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd

left outer  join (
select e.PaymentDate, (e.PaidAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
)bdee on  bdee.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where   fd.GLFiscalYearId between @GLFiscalYearIdFrom and @GLFiscalYearIdTo  
group by fd.year,SUBSTRING(fd.PeriodSl,1,2)
order by  SUBSTRING(fd.PeriodSl,1,2) asc ,YEAR desc

--select * from #temp
DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.YearName) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT PeriodSl, ' + @cols + ' from 
            (
                select PeriodSl
                    , isnull(Amount,0)amount
                    , YearName
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(Amount)
                for YearName in (' + @cols + ')
            ) p 
			'
execute(@query)
drop table #temp
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@GLFiscalYearIdTo", vm.GLFiscalYearId);

                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Add("Month", typeof(string));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int index = Array.IndexOf(Ordinary.Alphabet, dt.Rows[i]["PeriodSl"]);
                        if (index == -1)
                        {
                            dt.Rows[i]["Month"] = dt.Rows[i]["PeriodSl"];
                        }
                        else
                        {
                            dt.Rows[i]["Month"] = Ordinary.MonthNames[index];
                        }
                    }
                    dt.Columns["Month"].SetOrdinal(0);
                    dt.Columns.Remove("PeriodSl");
                }
                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================R20 YearBranchExpenseStatement=================
        public DataTable Report20(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R20--YearBranchExpenseStatement

--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--set @DateFrom='20170201'
--set @DateTo='20191230'
--set @BranchId=5

declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 

create table #Temp(YearName  varchar(200),PeriodSl  varchar(200), Amount Decimal(18,2))

insert into #Temp
select distinct fd.Year, SUBSTRING(fd.PeriodSl,1,2)
,isnull(sum(be.BranchExpense),0)+isnull(sum(ace.AgencyComExpense),0)+isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)-isnull(sum(bdee.PCRecovery),0) TotalExpense

  from GLFiscalYearDetails fd
 left outer join (select fd.Id FYId, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
left outer  join (
select fd.Id FYId, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and B.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
) as ace on  ace.FYId=fd.id
left outer  join (
select fd.Id FYId, (a.PaidAmount)BDEAmount  
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on a.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and A.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdea on  bdea.FYId=fd.id

left outer  join(
select fd.Id FYId, (b.PaidAmount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and B.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdeb on  bdeb.FYId=fd.id
left outer  join (
select fd.Id FYId, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and C.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdec on  bdec.FYId=fd.id
 left outer  join(
 select fd.Id FYId, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1  and m.IsRejected=0 and d.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
)bded on  bded.FYId=fd.id

left outer  join (
select fd.Id FYId, (e.PaidAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on e.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1  and m.IsRejected=0 and E.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
)bdee on  bdee.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo
group by fd.year,SUBSTRING(fd.PeriodSl,1,2)
order by  SUBSTRING(fd.PeriodSl,1,2) asc ,YEAR desc

--select * from #temp
DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.YearName) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT PeriodSl, ' + @cols + ' from 
            (
                select PeriodSl
                    , isnull(Amount,0)amount
                    , YearName
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(Amount)
                for YearName in (' + @cols + ')
            ) p 
			'

execute(@query)
drop table #temp
 
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));

                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Add("Month", typeof(string));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int index = Array.IndexOf(Ordinary.Alphabet, dt.Rows[i]["PeriodSl"]);
                        if (index == -1)
                        {
                            dt.Rows[i]["Month"] = dt.Rows[i]["PeriodSl"];
                        }
                        else
                        {
                            dt.Rows[i]["Month"] = Ordinary.MonthNames[index];
                        }
                    }
                    dt.Columns["Month"].SetOrdinal(0);
                    dt.Columns.Remove("PeriodSl");
                }
                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================R21 YearAgentCommissionExpenseStatement=================
        public DataTable Report21(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R21--YearAgentCommissionExpenseStatement

--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--set @DateFrom='20170201'
--set @DateTo='20191230'
--set @BranchId=5

declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 
create table #Temp(YearName  varchar(200),PeriodSl  varchar(200), Amount Decimal(18,2))

insert into #Temp
select distinct fd.Year, SUBSTRING(fd.PeriodSl,1,2)
,isnull(sum(ace.AgencyComExpense),0)+isnull(sum(bdea.BDEAmount),0) AgencyComExpense
  from GLFiscalYearDetails fd
left outer join (select fd.Id FYId, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
left outer  join (
select fd.Id FYId, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and B.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
) as ace on  ace.FYId=fd.id
left outer  join (
select fd.Id FYId, (a.PaidAmount)BDEAmount  
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on a.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and A.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdea on  bdea.FYId=fd.id

left outer  join(
select fd.Id FYId, (b.PaidAmount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and B.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdeb on  bdeb.FYId=fd.id
left outer  join (
select fd.Id FYId, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and C.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdec on  bdec.FYId=fd.id
 left outer  join(
 select fd.Id FYId, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1  and m.IsRejected=0 and d.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
)bded on  bded.FYId=fd.id

left outer  join (
select fd.Id FYId, (e.PaidAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on e.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1  and m.IsRejected=0 and E.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
)bdee on  bdee.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo
group by fd.year,SUBSTRING(fd.PeriodSl,1,2)
order by  SUBSTRING(fd.PeriodSl,1,2) 

--select * from #temp
DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.YearName) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT PeriodSl, ' + @cols + ' from 
            (
                select PeriodSl
                    , isnull(Amount,0)amount
                    , YearName
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(Amount)
                for YearName in (' + @cols + ')
            ) p 
			'

execute(@query)
drop table #temp
 
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));

                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Add("Month", typeof(string));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int index = Array.IndexOf(Ordinary.Alphabet, dt.Rows[i]["PeriodSl"]);
                        if (index == -1)
                        {
                            dt.Rows[i]["Month"] = dt.Rows[i]["PeriodSl"];
                        }
                        else
                        {
                            dt.Rows[i]["Month"] = Ordinary.MonthNames[index];
                        }
                    }
                    dt.Columns["Month"].SetOrdinal(0);
                    dt.Columns.Remove("PeriodSl");
                }
                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================R22 YearBusinessDevelopmentExpenseStatement=================
        public DataTable Report22(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R22--Year-Wise BusinessDevelopment Expense Statement

--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--set @DateFrom='20170201'
--set @DateTo='20191230'
--set @BranchId=5

declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 

create table #Temp(YearName  varchar(200),PeriodSl  varchar(200), Amount Decimal(18,2))

insert into #Temp
select distinct fd.Year, SUBSTRING(fd.PeriodSl,1,2)
,isnull(sum(bdea.BDEAmount),0)+isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)-isnull(sum(bdee.PCRecovery),0) TotalBDEExpense
  from GLFiscalYearDetails fd
left outer join (select fd.Id FYId, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
left outer  join (
select fd.Id FYId, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and B.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
) as ace on  ace.FYId=fd.id
left outer  join (
select fd.Id FYId, (a.PaidAmount)BDEAmount  
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on a.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and A.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdea on  bdea.FYId=fd.id

left outer  join(
select fd.Id FYId, (b.PaidAmount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and B.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdeb on  bdeb.FYId=fd.id
left outer  join (
select fd.Id FYId, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and C.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdec on  bdec.FYId=fd.id
 left outer  join(
 select fd.Id FYId, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1  and m.IsRejected=0 and d.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
)bded on  bded.FYId=fd.id

left outer  join (
select fd.Id FYId, (e.PaidAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on e.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1  and m.IsRejected=0 and E.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
)bdee on  bdee.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo
group by fd.year,SUBSTRING(fd.PeriodSl,1,2)
order by  SUBSTRING(fd.PeriodSl,1,2) 

--select * from #temp
DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX);

SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.YearName) 
            FROM #Temp c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')

set @query = 'SELECT PeriodSl, ' + @cols + ' from 
            (
                select PeriodSl
                    , isnull(Amount,0)amount
                    , YearName
                from #Temp
				
           ) x
		   
            pivot 
            (
                 sum(Amount)
                for YearName in (' + @cols + ')
            ) p 
			'

execute(@query)
drop table #temp
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));


                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Add("Month", typeof(string));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int index = Array.IndexOf(Ordinary.Alphabet, dt.Rows[i]["PeriodSl"]);
                        if (index == -1)
                        {
                            dt.Rows[i]["Month"] = dt.Rows[i]["PeriodSl"];
                        }
                        else
                        {
                            dt.Rows[i]["Month"] = Ordinary.MonthNames[index];
                        }
                    }
                    dt.Columns["Month"].SetOrdinal(0);
                    dt.Columns.Remove("PeriodSl");
                }
                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }
        ////==================23 MonthTotalExpenseStatement=================
        public DataTable Report23(GLReportVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
--R23--MonthTotalExpenseStatement

--declare @DateFrom as varchar(20)
--declare @DateTo as varchar(20)
--declare @BranchId as varchar(20)
--set @DateFrom='20170201'
--set @DateTo='20191230'
--set @BranchId=5

declare @PeriodIdFrom as varchar(20)
declare @PeriodIdTo as varchar(20)

select @PeriodIdFrom=id   from GLFiscalYearDetails fd
where @DateFrom between fd.PeriodStart and fd.PeriodEnd 

select @PeriodIdTo=id   from GLFiscalYearDetails fd
where @DateTo between fd.PeriodStart and fd.PeriodEnd 

select distinct fd.Id,fd.PeriodName
,isnull(sum(be.BranchExpense),0)BranchExpense
,isnull(sum(ace.AgencyComExpense),0)AgencyComExpense
,isnull(sum(bdea.BDEAmount),0)BDEAmount
,isnull(sum(bdeb.Salary),0)BDEEmployeeSalary
,isnull(sum(bdec.BankCharge),0)BDEBankCharge
,isnull(sum(bdee.PCRecovery),0)PCRecovery
,isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)-isnull(sum(bdee.PCRecovery),0) TotalBDEExpense
,isnull(sum(be.BranchExpense),0)+isnull(sum(ace.AgencyComExpense),0)+isnull(sum(bdea.BDEAmount),0) +isnull(sum(bdeb.Salary),0)+isnull(sum(bdec.BankCharge),0)-isnull(sum(bdee.PCRecovery),0) TotalExpense
 
  from GLFiscalYearDetails fd
left outer join (select fd.Id FYId, (d.TransactionAmount)BranchExpense
 from GLFinancialTransactionDetails d
left outer join GLFinancialTransactions m on m.Id=d.GLFinancialTransactionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0
and m.BranchId=@BranchId
 ) as be on be.FYId=fd.id
left outer  join (
select fd.Id FYId, (b.PCAmount)AgencyComExpense 
from GLPettyCashRequisitionFormBs b
left outer join GLPettyCashRequisitions m on m.Id=b.GLPettyCashRequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and B.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
) as ace on  ace.FYId=fd.id
left outer  join (
select fd.Id FYId, (a.PaidAmount)BDEAmount  
from GLBDERequisitionFormAs a
left outer join GLBDERequisitions m on m.Id=a.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on a.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and A.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdea on  bdea.FYId=fd.id

left outer  join(
select fd.Id FYId, (b.PaidAmount)Salary 
from GLBDERequisitionFormBs b
left outer join GLBDERequisitions m on m.Id=b.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on b.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and B.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdeb on  bdeb.FYId=fd.id
left outer  join (
select fd.Id FYId, (c.Amount)BankCharge 
from GLBDERequisitionFormCs c
left outer join GLBDERequisitions m on m.Id=c.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1 and m.IsRejected=0 and C.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
 )bdec on  bdec.FYId=fd.id
 left outer  join(
 select fd.Id FYId, (d.Amount)ContingencyFund
from GLBDERequisitionFormDs d
left outer join GLBDERequisitions m on m.Id=d.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on m.TransactionDateTime between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1  and m.IsRejected=0 and d.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
)bded on  bded.FYId=fd.id

left outer  join (
select fd.Id FYId, (e.PaidAmount)PCRecovery
from GLBDERequisitionFormEs e
left outer join GLBDERequisitions m on m.Id=e.GLBDERequisitionId
left outer join GLFiscalYearDetails fd on e.PaymentDate between fd.PeriodStart and fd.PeriodEnd
where m.IsApprovedL4=1  and m.IsRejected=0 and E.IsRejected=0  and m.IsFundReceived=1
and m.BranchId=@BranchId
)bdee on  bdee.FYId=fd.id
where fd.id between @PeriodIdFrom and @PeriodIdTo
group by fd.Id,fd.PeriodName
order by fd.Id
 
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
                da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));

                da.Fill(dt);

                //dt.Columns.Add("Month", typeof(string));
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    int index = Array.IndexOf(Ordinary.Alphabet, dt.Rows[i]["PeriodName"]);
                //    dt.Rows[i]["Month"] = Ordinary.MonthNames[index];
                //}
                //dt.Columns["Month"].SetOrdinal(0);
                //dt.Columns.Remove("PeriodName");
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Remove("Id");
                }
                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

    }
}
