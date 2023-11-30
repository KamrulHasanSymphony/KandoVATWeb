using SymOrdinary;
using SymServices.Common;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SymServices.Sage
{
    public class CommissionBillDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        public List<CommissionBillVM> DropDownFromPCR(int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CommissionBillVM> VMs = new List<CommissionBillVM>();
            CommissionBillVM vm;
            #endregion
            try
            {
                string SAGEGLDB = _dbsqlConnection.SAGEGLDB;
                #region Get BranchCode
                string BranchCode = new GLBranchDAL().SelectAll(branchId).FirstOrDefault().Code;

                #endregion

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
distinct
 pcr.CommissionBillNo
 FROM GLPettyCashRequisitions pcr
 WHERE  1=1 
AND CommissionBillNo is not null
AND pcr.BranchId=@BranchId
AND pcr.IsApprovedL4 = 1
";
                sqlText += @" AND CommissionBillNo NOT IN (
SELECT ISNULL(CommissionBillNo, '0') CommissionBillNo FROM GLFinancialTransactions ft";
                sqlText += @" WHERE 1=1 AND ft.Post = 1 AND ft.IsRejected = 0 AND ft.BranchId=@BranchId
)

";
                if (branchId == 0)
                {
                    sqlText = sqlText.Replace("ft.BranchId=@BranchId", "1=1");
                    sqlText = sqlText.Replace("pcr.BranchId=@BranchId", "1=1");
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                if (branchId > 0)
                {
                    objComm.Parameters.AddWithValue("@BranchId", branchId);
                }


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CommissionBillVM();
                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
                    vm.Name = vm.CommissionBillNo;
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


        //==================DropDown=================
        public List<CommissionBillVM> DropDown(int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CommissionBillVM> VMs = new List<CommissionBillVM>();
            CommissionBillVM vm;
            #endregion
            try
            {
                string SAGEGLDB = _dbsqlConnection.SAGEGLDB;
                #region Get BranchCode
                string BranchCode = new GLBranchDAL().SelectAll(branchId).FirstOrDefault().Code;
                string CommissionBillNoList = "";
                List<GLPettyCashRequisitionVM> pcrVMs = new List<GLPettyCashRequisitionVM>();
                GLPettyCashRequisitionDAL _pcrDAL = new GLPettyCashRequisitionDAL();
                {
                    string[] cFields = { "pc.Post", "pc.IsRejected", "pc.BranchId" };
                    string[] cValues = { "1", "0", branchId.ToString() };

                    pcrVMs = _pcrDAL.SelectAll(0, cFields, cValues);
                    if (pcrVMs != null && pcrVMs.Count > 0)
                    {
                        foreach (GLPettyCashRequisitionVM item in pcrVMs)
                        {
                            CommissionBillNoList += "'" + item.CommissionBillNo + "',";
                        }

                        CommissionBillNoList = CommissionBillNoList.Remove(CommissionBillNoList.Length - 1, 1);

                    }
                }


                #endregion

                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionGDICCommissionBill();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT
distinct
 CommissionBillNo
,CONVERT(VARCHAR(8), ComDate, 112) ComDate
,BranchCode
,DeptCode

   FROM CommissionBill
WHERE  1=1
";
                if (!string.IsNullOrWhiteSpace(CommissionBillNoList))
                {
                    sqlText += " AND CommissionBillNo NOT IN (" + CommissionBillNoList + ")";
                }
                ////////////sqlText += " AND CommissionBillNo Not In( SELECT ISNULL(CommissionBillNo, '0') CommissionBillNo FROM " + SAGEGLDB + ".dbo.GLPettyCashRequisitions pcr";
                ////////////                sqlText += @" WHERE 1=1 AND pcr.Post = 1 AND pcr.IsRejected = 0)";

                if (!string.IsNullOrWhiteSpace(BranchCode))
                {
                    sqlText += @" AND BranchCode=@BranchCode";
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                if (!string.IsNullOrWhiteSpace(BranchCode))
                {
                    objComm.Parameters.AddWithValue("@BranchCode", BranchCode);
                }


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CommissionBillVM();
                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
                    vm.ComDate = Ordinary.StringToDate(dr["ComDate"].ToString());
                    vm.BranchCode = dr["BranchCode"].ToString();
                    vm.DeptCode = dr["DeptCode"].ToString();
                    vm.Name = vm.CommissionBillNo + "~" + vm.ComDate + "~" + vm.BranchCode + "~" + vm.DeptCode;
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
        public List<CommissionBillVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CommissionBillVM> VMs = new List<CommissionBillVM>();
            CommissionBillVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionGDICCommissionBill();
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
                sqlText = @"
SELECT
 CommissionBillNo
,CONVERT(VARCHAR(8), ComDate, 112) ComDate
,BranchCode
,DeptCode

from CommissionBill
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
                    vm = new CommissionBillVM();
                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
                    vm.ComDate = Ordinary.StringToDate(dr["ComDate"].ToString());
                    vm.BranchCode = dr["BranchCode"].ToString();
                    vm.DeptCode = dr["DeptCode"].ToString();

                    VMs.Add(vm);
                }
                dr.Close();
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

        //==================SelectAllDetail=================
        public List<CommissionBillDetailVM> SelectAllDetail(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CommissionBillDetailVM> VMs = new List<CommissionBillDetailVM>();
            CommissionBillDetailVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionGDICCommissionBill();
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
 cbd.CommissionBillNo 
,CONVERT(VARCHAR(8), cb.ComDate, 112) ComDate
,cbd.SLNo 
,cbd.DeptCode 
,cbd.Class 
,cbd.DocumentNo 
,cbd.MRNo 
,cbd.BranchCode 
,cbd.CustomerID 
,cbd.InsuredName 
,cbd.NetPremium
,cbd.RateOfCommission 
,cbd.CommissionAmount 
,cbd.TaxRate 
,cbd.TaxAmount 
,cbd.NetCommission 

FROM  CommissionBill_Details  cbd
LEFT OUTER JOIN CommissionBill cb ON cb.CommissionBillNo = cbd.CommissionBillNo

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
                    vm = new CommissionBillDetailVM();
                    vm.CommissionBillNo = dr["CommissionBillNo"].ToString();
                    vm.SLNo = Convert.ToInt32(dr["SLNo"].ToString());
                    vm.DeptCode = dr["DeptCode"].ToString();
                    vm.Class = dr["Class"].ToString();
                    vm.DocumentNo = dr["DocumentNo"].ToString();
                    vm.MRNo = dr["MRNo"].ToString();
                    vm.BranchCode = dr["BranchCode"].ToString();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.InsuredName = dr["InsuredName"].ToString();
                    vm.NetPremium = Convert.ToDecimal(dr["NetPremium"].ToString());
                    vm.RateOfCommission = Convert.ToDecimal(dr["RateOfCommission"].ToString());
                    vm.CommissionAmount = Convert.ToDecimal(dr["CommissionAmount"].ToString());
                    vm.TaxRate = Convert.ToDecimal(dr["TaxRate"].ToString());
                    vm.TaxAmount = Convert.ToDecimal(dr["TaxAmount"].ToString());
                    vm.NetCommission = Convert.ToDecimal(dr["NetCommission"].ToString());
                    vm.ComDate = Ordinary.StringToDate(dr["ComDate"].ToString());

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
    }
}
