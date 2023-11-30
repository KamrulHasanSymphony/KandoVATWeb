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
    public class VDSDAL
    {
       
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();


        // public static string SearchVDS(string encryptedData, string encryptedData123)

        //==================SelectAll=================
        public List<VDSMasterVM> SelectVDSDetail(string VDSId, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<VDSMasterVM> VMs = new List<VDSMasterVM>();
            VDSMasterVM vm;
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
 vd.VDSId
,vd.VendorId
,ISNULL(vd.BillAmount,0) BillAmount
,vd.BillDate
,ISNULL(vd.BillDeductAmount,0) BillDeductAmount
,ISNULL(vd.DepositNumber,0) DepositNumber
,vd.DepositDate
,vd.Remarks
,vd.IssueDate
,vd.PurchaseNumber
,vd.CreatedBy
,vd.CreatedOn
,vd.LastModifiedBy
,vd.LastModifiedOn
,ISNULL(vd.VDSPercent,0) VDSPercent
,vd.IsPurchase
,vd.IsPercent
,vd.ReverseVDSId
,vn.VendorName

FROM VDS  vd left outer join Vendors vn
on vd.VendorId=vn.VendorId
WHERE  1=1

";

                if (VDSId != null)
                {
                    sqlText += "AND vd.VDSId=@VDSId";
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

                if (VDSId != null)
                {
                    objComm.Parameters.AddWithValue("@VDSId", VDSId);
                }
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
                    vm = new VDSMasterVM();
                    vm.Id = dr["VDSId"].ToString();
                    vm.VendorId = dr["VendorId"].ToString();
                    vm.VendorName = dr["VendorName"].ToString();
                    vm.BillAmount = Convert.ToDecimal(dr["BillAmount"].ToString());
                    vm.BillDate = Ordinary.DateTimeToDate(dr["BillDate"].ToString());
                    vm.BillDeductedAmount = Convert.ToDecimal(dr["BillDeductAmount"].ToString());
                    vm.DepositNumber = dr["DepositNumber"].ToString();
                    vm.DepositDate = Ordinary.DateTimeToDate(dr["DepositDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IssueDate = Ordinary.DateTimeToDate(dr["IssueDate"].ToString());
                    vm.PurchaseNumber = dr["PurchaseNumber"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.VDSPercent = Convert.ToDecimal(dr["VDSPercent"].ToString());
                    vm.IsPurchase = dr["IsPurchase"].ToString();
                    vm.IsPercent = dr["IsPercent"].ToString();

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
                throw new ArgumentNullException("", "SQL:" + sqlText  + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + ex.Message.ToString());
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
    }
}
