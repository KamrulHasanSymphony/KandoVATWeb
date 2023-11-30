using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
   public class UserLogsVM
    {
        public string DataBaseName { get; set; }
        public string LogID { get; set; }
        public string ComputerName { get; set; }
        public string ComputerLoginUserName { get; set; }
        public string ComputerIPAddress { get; set; }
        public string SoftwareUserId { get; set; }
        public string SoftwareUserName { get; set; }
        public string SessionDate { get; set; }
        public string LogInDateTime { get; set; }
        public string LogOutDateTime { get; set; }
        public string ReturnUrl { get; set; }
       
    }
}
