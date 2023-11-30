using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class VendorVM
    {
        public string VendorID { get; set; }
        //[Display(Name = "Code")]
        public string VendorCode { get; set; }
        //[Display(Name = "Name")]
        public string VendorName { get; set; }
        public string VendorGroup { get; set; }
        //[Display(Name = "Group")]
        public string VendorGroupID { get; set; }
        //[Display(Name = "Address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        //[Display(Name = "Telephone No")]
        public string TelephoneNo { get; set; }
        //[Display(Name = "Fax No")]
        public string FaxNo { get; set; }
        public string Email { get; set; }
        //[Display(Name = "Start Date")]
        public string StartDateTime { get; set; }
        //[Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        //[Display(Name = "CP Designation")]
        public string ContactPersonDesignation { get; set; }
        //[Display(Name = "CP Telephone")]
        public string ContactPersonTelephone { get; set; }
        //[Display(Name = "CP Email")]
        public string ContactPersonEmail { get; set; }
        //[Display(Name = "VAT Registration No")]
        public string VATRegistrationNo { get; set; }
        //[Display(Name = "TIN No")]
        public string TINNo { get; set; }
        public string Comments { get; set; }
        //[Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }
        
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Country { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public string Info4 { get; set; }
        public string Info5 { get; set; }
        public decimal VDSPercent { get; set; }
        //[Display(Name = "Business Type")]
        public string BusinessType { get; set; }
        //[Display(Name = "Business Code")]
        public string BusinessCode { get; set; }
        public string Operation { get; set; }
        //[Display(Name = "Start Date From")]
        public string StartDateFrom { get; set; }
        //[Display(Name = "Start Date To")]
        public string StartDateTo { get; set; }
        public string IsActive { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
    }
}
