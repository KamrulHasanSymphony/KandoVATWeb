using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class CustomerGroupVM
    {
        public string CustomerGroupID { get; set; }
        public string CustomerGroupName { get; set; }
        public string CustomerGroupDescription { get; set; }
        public string GroupType { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Info1 { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public string Info4 { get; set; }
        public string Info5 { get; set; }
        public string Operation { get; set; }

    }
}
