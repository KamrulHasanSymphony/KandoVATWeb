using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class CompanyOverheadVM
    {
        public string HeadID { get; set; }
        public string HeadName { get; set; }
        public decimal HeadAmount { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public bool ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string OHCode { get; set; }
        public decimal VATRate { get; set; }
        public decimal RebatePercent { get; set; }

    }
}
