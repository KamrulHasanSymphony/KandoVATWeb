using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class UserRollVM
    {
        public string UserID { get; set; }
        public string FormID { get; set; }
        public string Access { get; set; }
        public string LastLoginDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string LineID { get; set; }
        public string FormName { get; set; }
        public string PostAccess { get; set; }
        public string AddAccess { get; set; }
        public string EditAccess { get; set; }
    }

    public class UserInformationVM
    {
        public string UserID { get; set; }
        public string UserName { get; set; } 
        public string UserPassword { get; set; }
        public bool ActiveStatus { get; set; }
        public string LastLoginDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string databaseName { get; set; }
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public string Operation { get; set; }


        public string FullName { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string VerificationCode { get; set; }
        public string IsVerified { get; set; }
        public string LastPasswordChangeDate { get; set; }
        public bool IsAdmin { get; set; }

    }

    public class LoginVM
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string CompanyID { get; set; }
        public string DatabaseName { get; set; }
        public string SessionDate { get; set; }
    }
}
