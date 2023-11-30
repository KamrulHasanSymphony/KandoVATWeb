using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class CmpanyListVM
    {
        public string CompanySl { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string DatabaseName { get; set; }
        public string ActiveStatus { get; set; }
        public string Serial { get; set; }
    }

    public class settingVM
    {
        public static DataTable SettingsDT;
    }

    public class DatabaseInfoVM
    {
        public static string DatabaseName = "PCCL_DB";// { get; set; }
        public static string dbUserName { get; set; }
        public static string dbPassword { get; set; }
        public static string dataSource { get; set; }
    }

    public class SuperAdminInfoVM
    {
        public static string dbUserName { get; set; }
        public static string dbPassword { get; set; }
        public static string dataSource { get; set; }
    }

    public class SysDBInfoVM
    {
        public static string SysDatabaseName = "SymphonyVATSys";
        public static string SysUserName="sa";// { get; set; }
        public static string SysPassword="S123456_";// { get; set; }
        public static string SysdataSource = ".";// { get; set; }

    }

    public class UserInfoVM
    {
        public static string UserName { get; set; }
        public static string Password { get; set; }
        public static string UserId = "10"; //{ get; set; }
    }

    public class BureauInfoVM
    {
        public static bool IsBureau { get; set; }
        public static string SessionDate { get; set; }
    }
    public class VAT7MasterVM
    {
        public VAT7VM Master { get; set; }
        public List<VAT7VM> Details { get; set; }
        public string Operation { get; set; }
        public string Post { get; set; }
    }
    public class VAT7VM
    {
        public string Id { get; set; }
        public string VAT7No { get; set; }
        public string Vat7DateTime { get; set; }
        public string FinishItemNo { get; set; }
        public string FinishItemCode { get; set; }
        public string FinishItemName { get; set; }
        public string FinishUOM { get; set; }
        public string Vat7LineNo { get; set; }

        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string ItemUOM { get; set; }
        public decimal Quantity { get; set; }
        public decimal UOMQty { get; set; }
        public decimal UOMc { get; set; }
        public string UOMn { get; set; }

        public string Post { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Operation { get; set; }

    }




}
