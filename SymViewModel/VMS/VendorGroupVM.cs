using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class VendorGroupVM
    {
        public string VendorGroupID { get; set; }
        //[Display(Name = "Group Name")]
        public string VendorGroupName { get; set; }
        //[Display(Name = "Description")]
        public string VendorGroupDescription { get; set; }
        //[Display(Name = "Type")]
        public string GroupType { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        //[Display(Name = "Information 3")]
        public string Info3 { get; set; }
        //[Display(Name = "Information 4")]
        public string Info4 { get; set; }
        //[Display(Name = "Information 5")]
        public string Info5 { get; set; }
        //[Display(Name = "Information 2")]
        public string Info2 { get; set; }
        public string Operation { get; set; }

    }
}
