using SymOrdinary;
using SymServices.Common;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymServices.VMS;

namespace SymServices.Enum
{
    public class EnumColumnListDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion
        public List<EnumColumnListVM> DropDown(string tableName = "")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EnumColumnListVM> VMs = new List<EnumColumnListVM>();
            EnumColumnListVM vm;
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
Id,
Name
   FROM EnumColumnList
WHERE IsArchive=0 and IsActive=1
";
                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    sqlText += " AND TableName = @TableName";
                }
                sqlText += "  ORDER BY Name";
                SqlCommand _objComm = new SqlCommand(sqlText,currConn);
                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    _objComm.Parameters.AddWithValue("@TableName", tableName);
                }

                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EnumColumnListVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
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
    }
}
