using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SymViewModel.Common
{
    public class CmpanyListVM
    {
        [Display(Name = "Company Sl")]
        public string CompanySl { get; set; }
        [Display(Name = "Company ID")]
        public string CompanyID { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Database Name")]
        public string DatabaseName { get; set; }
        [Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }
        public string Serial { get; set; }
    }

    public class settingVM
    {
        public static DataTable SettingsDT;
    }
}
