using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOrdinary;
using SymphonySofttech.Utilities;
using SymViewModel.VMS;

namespace SymServices.VMS
{
   public class CompanyInformationDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        #endregion
        public List<SymphonyVATSysCompanyInformationVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SymphonyVATSysCompanyInformationVM> VMs = new List<SymphonyVATSysCompanyInformationVM>();
            SymphonyVATSysCompanyInformationVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionSymphonyVATSys();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
                SELECT
                CompanyID Id
                ,CompanyName Name
                FROM CompanyInformations  
                WHERE  1=1 AND ActiveStatus = 'mXSJfsAdbf0='
                order by Serial
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SymphonyVATSysCompanyInformationVM();
                    vm.Id = dr["Id"].ToString();

                    vm.Name =Converter.DESDecrypt(PassPhrase, EnKey,dr["Name"].ToString());
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

        public List<SymphonyVATSysCompanyInformationVM> SelectAll(string CompanyID = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SymphonyVATSysCompanyInformationVM> VMs = new List<SymphonyVATSysCompanyInformationVM>();
            SymphonyVATSysCompanyInformationVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionSymphonyVATSys();
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
 CompanySl
,CompanyID
,CompanyName
,DatabaseName
,ActiveStatus
,Serial

FROM CompanyInformations  
WHERE  1=1 

";
                if (CompanyID != null)
                {
                    sqlText += @" and CompanyID=@CompanyID";
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

                sqlText += @" order by Serial";

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

                if (CompanyID != null)
                {
                    objComm.Parameters.AddWithValue("@CompanyID", CompanyID);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SymphonyVATSysCompanyInformationVM();

vm.CompanySl           =Convert.ToInt32( dr["CompanySl"] );
vm.CompanyID           = dr["CompanyID"].ToString();
vm.CompanyName         = dr["CompanyName"].ToString();
vm.DatabaseName = Converter.DESDecrypt(PassPhrase, EnKey, dr["DatabaseName"].ToString());

vm.ActiveStatus        = dr["ActiveStatus"].ToString();
vm.Serial = Convert.ToInt32( dr["Serial"] );
                     

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


    }
}
