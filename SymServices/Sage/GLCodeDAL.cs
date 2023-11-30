using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SymOrdinary;
using System.Linq;
namespace SymServices.Sage
{
   public class GLCodeDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion

//Ref: GDI/KRL/01/2018/ACC-PCR/0001  -Petty Cash Requesition
//Ref: GDI/KRL/01/2018/ACC-PCR/0001  -Petty Cash Requesition(Summery/Top Shit)
//Ref: GDI/KRL/01/2018/ACC-BDE/0001  -BDE Requesition
//Ref: GDI/KRL/01/2018/ACC-BDE/0001  -BDE Requesition(Summery/Top Shit)
//Ref: GDI/KRL/01/2018/ACC-EXP/0001  -Petty Cash EXP
        //Ref: GDI/KRL/01/2018/ACC-VCH/0001  -Petty Cash Voucher 
        //Ref: GDI/KRL/01/2018/ACC-PRV/0001  -Petty Cash Requesition Voucher VCH 

        public string NextCodeAcc(string tableName, int BranchId, DateTime TransDate, string TransactionType, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            string NextCode = "";
            string BranchCode = "";
            int LastCode =0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
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
                GLBranchDAL bdal = new GLBranchDAL();
                BranchCode = bdal.SelectAll(BranchId, null, null, currConn, transaction).FirstOrDefault().Code;
                #region Save
                sqlText = "";
                sqlText += @" select isnull(substring(code,CHARINDEX('/',code,17)+1,10),0) LastCode";
                sqlText += @" from " + tableName + "";
                sqlText += @" where 1=1 ";
                sqlText += @" and  SUBSTRING(Code,12,4)='" + TransDate.ToString("yyyy") + "' ";
                sqlText += @" and  BranchId='" + BranchId + "' ";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    LastCode =Convert.ToInt32( dr["LastCode"]);
                }
                dr.Close();
                
                    LastCode =LastCode+1;

                    //        GDI/KRL/01/2018/ACC-PCR/00001
                    NextCode = "GDI/" + BranchCode + "/" + TransDate.ToString("MM") + "/" + TransDate.ToString("yyyy") + "/ACC-" + TransactionType + "/" + LastCode.ToString().PadLeft(5, '0');
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
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return NextCode;
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
            return NextCode;
            #endregion
        }

    }
}
