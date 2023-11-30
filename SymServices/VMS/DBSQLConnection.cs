using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using SymOrdinary;
using SymViewModel.VMS;

namespace SymServices.VMS
{
    public class DBSQLConnection
    {
        public string DataSource = new AppSettingsReader().GetValue("DataSource", typeof(string)).ToString();
        public string UserID = new AppSettingsReader().GetValue("UserID", typeof(string)).ToString();
        public string Password = new AppSettingsReader().GetValue("Password", typeof(string)).ToString();


        public SqlConnection GetConnection(string InitialCatalog = "")
        {
            string ConnectionString = "";
            if (string.IsNullOrWhiteSpace(InitialCatalog))
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                ConnectionString = "Data Source=" + DataSource + ";" +
                                    "Initial Catalog=" + identity.InitialCatalog + ";" +

                                    "user id=" + UserID + ";" +
                                    "password=" + Password + ";" +
                                    " connect Timeout=6000;MultipleActiveResultSets=True; pooling=no;";
            }
            else
            {
                ConnectionString = "Data Source=" + DataSource + ";" +

                                    "Initial Catalog=" + InitialCatalog + ";" +
                                    "user id=" + UserID + ";" +
                                    "password=" + Password + ";" +
                                    "connect Timeout=60;";

            }

            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }

        public SqlConnection GetConnectionEmailDb()
        {
            string ConnectionString = new AppSettingsReader().GetValue("dbConnectionStringsEmailDb", typeof(string)).ToString();
            SqlConnection conn = new SqlConnection(ConnectionString);
            return conn;
        }

        public SqlConnection GetConnectionSymphonyVATSys()
        {
            string ConnectionString = "Data Source=" + DataSource + ";" +
                                      "Initial Catalog=SymphonyVATSys;" +
                                      "user id=" + UserID + ";" +
                                      "password=" + Password + ";" +
                                      "connect Timeout=60;";
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }
        public SqlConnection GetConnectionMaster()
        {

            string ConnectionString = "Data Source=" + DataSource + ";" +
                            "Initial Catalog=master;" +
                            "user id=" + UserID + ";" +
                            "password=" + Password + ";" +
                            "connect Timeout=60;";
            SqlConnection conn = new SqlConnection(ConnectionString);


            return conn;
        }
        public string ServerDateTime()
        {
            string result = "19800101";
            SqlConnection currConn = null;
            string sqlText = "";
            try
            {
                #region open connection and transaction

                currConn = GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = @"use master";
                sqlText += @" SELECT CONVERT(VARCHAR(8), SYSDATETIME(), 112)";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                result = cmdIdExist.ExecuteScalar().ToString();

            }
            #region Catch

            catch (Exception ex)
            {
            }
            #endregion
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
            return result;

        }


    }
}
