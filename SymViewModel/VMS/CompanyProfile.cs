using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class CompanyProfileVM
    {
        //
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLegalName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactPersonTelephone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string TINNo { get; set; }
        public string VatRegistrationNo { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string StartDateTime { get; set; }
        public string FYearStart { get; set; }
        public string FYearEnd { get; set; }
        public string Tom { get; set; }       //encrypted companyName
        public string Jary { get; set; }      //encrypted CompanyLegalName
        public string Miki { get; set; }      //encrypted VatRegistrationNo
        public string Mouse { get; set; }     //encrypted ProcessorId
        public string Operation { get; set; }
    }
}
