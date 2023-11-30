using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOrdinary
{
   public class DBSQLConnection
    {
 

        public string DataSource = new AppSettingsReader().GetValue("DataSource", typeof(string)).ToString();
        public string UserID = new AppSettingsReader().GetValue("UserID", typeof(string)).ToString();
        public string Password = new AppSettingsReader().GetValue("Password", typeof(string)).ToString();

     
        public SqlConnection GetConnection(string InitialCatalog="")
        {
            string ConnectionString = "";
            if (string.IsNullOrWhiteSpace(InitialCatalog))
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                ConnectionString = "Data Source=" + DataSource + ";" +
                                    "Initial Catalog=" + identity.UserId + ";" +

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
    }
}
