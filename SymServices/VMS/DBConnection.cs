using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using System.Text;
//using System.Web.Services;
using System.Data;
using System.Diagnostics;
using SymphonySofttech.Utilities;  
using System.Xml;
using SymViewModel.VMS;


/// <summary>
/// Summary description for DBConnection
/// </summary>  
public static class DBConnection
{

  

    private static string encryptedData123;// = "VAT";
   
    public static string ConnectionString = string.Empty;// = "Data Source=" + DBS + ";user id=" + ID + ";password=" + PWD + "";

    //private static string ConnectionString = "Data Source=.\\SQLEXPRESS;user id=sa;password=sssa";
    
    private static SqlConnection DefaultConnection = null;

    public static  string DBUserInformation()
    {
        try
        {
            ////System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            ////doc.Load(System.Web.HttpContext.Current.Server.MapPath("~/XML/DBInformation.xml"));
            ////DataSet ds = new DataSet();
            ////ds.ReadXml(System.Web.HttpContext.Current.Server.MapPath("~/XML/DBInformation.xml"));
            ////ID = Converter.DESDecrypt(PassPhrase, EnKey, ds.Tables[0].Rows[0][0].ToString());
            ////PWD = Converter.DESDecrypt(PassPhrase, EnKey, ds.Tables[0].Rows[0][1].ToString());
            ////DBS = Converter.DESDecrypt(PassPhrase, EnKey, ds.Tables[0].Rows[0][2].ToString());

            //ConnectionString = "Data Source=.;user id=sa;password=sssa";
            //ConnectionString = "Data Source=DULAL;user id=sa;password=sssa";
            //ConnectionString = "Data Source=ma-pc\\SQLEXPRESS;user id=sa;password=Sa123456_";
            //string cnString = ConfigurationManager.ConnectionStrings["VATDBConn"].ConnectionString;
            //ConnectionString = "Data Source=localhost;Initial Catalog=VATSample_DB;user id=sa;password=sssa ;connect Timeout=600;";
          //  ConnectionString = "Data Source=192.168.15.1\\SQLEXPRESS;Initial Catalog=VATSample_DB;user id=sa;password=Sa123456_ ;connect Timeout=600;";
           // string conn = System.Configuration.ConfigurationSettings.AppSettings["VATDB"];
            string conn =Convert.ToString( System.Configuration.ConfigurationManager.AppSettings["VATDB"]);

            //conn= conn.Replace("\\\\", "\\");
            ConnectionString = conn;
            //string conn2 = System.Configuration.ConfigurationManager.ConnectionStrings[0].ConnectionString;
            
            //ConnectionString = "Data Source=" + DBS + ";user id=" + ID + ";password=" + PWD + ";connect Timeout=600" ;
  
        //            ConnectionString = "user id=" + ID + ";password=" + PWD + "";  
         //   string str1 = "";
        }

        catch (SqlException ex)
        {
            Trace.Write(ex.Message);
        }
        return ConnectionString;
    }

 public static SqlConnection GetConnection(String DatabaseName)
    {
        try
        {
                ConnectionString = string.Empty;
                DBUserInformation();
                DefaultConnection = new SqlConnection(ConnectionString);
                if (DefaultConnection.State == ConnectionState.Open)
                {
                    DefaultConnection.Close();
                }
                if (DefaultConnection.State != ConnectionState.Open)
                {
                    DefaultConnection.Open();
                }
            //DefaultConnection.ChangeDatabase(DatabaseName);

        }
        catch (SqlException ex)
        {
            Trace.Write(ex.Message);
        }
        catch (Exception ex)
        {
            Trace.Write(ex.Message);
        }
        return DefaultConnection;

     }

 public static SqlConnection GetConnectionforSymphonyVATSys(String DatabaseName)
 {
     try
     {
         //ConnectionString = string.Empty;
         //DBUserInformation();
         string ConnectionString =
             "Data Source=192.168.15.1\\SQLEXPRESS;Initial Catalog=VATSample_DB;user id=sa;password=Sa123456_ ;connect Timeout=600;";
         DefaultConnection = new SqlConnection(ConnectionString);

         if (DefaultConnection.State == ConnectionState.Open)
         {
             DefaultConnection.Close();
         }
         if (DefaultConnection.State != ConnectionState.Open)
         {
             DefaultConnection.Open();
         }
         DefaultConnection.ChangeDatabase(DatabaseName);

     }
     catch (SqlException ex)
     {
         Trace.Write(ex.Message);
     }
     catch (Exception ex)
     {
         Trace.Write(ex.Message);
     }
     return DefaultConnection;

 }

 public static void FirstCloseConnection()
 {
     try
     {

         if (DefaultConnection.State == ConnectionState.Open)
         {
             DefaultConnection.Close();
         }
     }
     catch (Exception)
     {
         DefaultConnection.Close();
     }
     finally
     {
         DefaultConnection.Close();

     }
 }
 public static void CloseConnection()
 {
     try
     {
         if (DefaultConnection != null)
         {
             //if (DefaultConnection.State == ConnectionState.Open)
             //{
                 DefaultConnection.Close();
             //}
             DefaultConnection.Dispose();
         }
     }
     catch (Exception)
     {
         DefaultConnection.Close();
         DefaultConnection.Dispose();
     }
     finally
     {
         DefaultConnection.Close();
         DefaultConnection.Dispose();

     }
 }
    public static SqlConnection MasterGetConnection()
    {
        ConnectionString = string.Empty;
        try
        {
            DBUserInformation();
            DefaultConnection = new SqlConnection(ConnectionString);
            DefaultConnection.Open();
            DefaultConnection.ChangeDatabase("master");

        }
        catch (SqlException ex)
        {
            Trace.Write(ex.Message);
        }
        return DefaultConnection;

    }
   

    public static SqlConnection SingleGetConnection()
    {
        ConnectionString = string.Empty;
        DBUserInformation();
        if (DefaultConnection == null)
        {
            DefaultConnection = new SqlConnection(ConnectionString);

        }
        if (DefaultConnection.State != ConnectionState.Open)
        {
            DefaultConnection.Open();
        }

        DefaultConnection.ChangeDatabase(encryptedData123);
        return DefaultConnection;

    }


  

        
}