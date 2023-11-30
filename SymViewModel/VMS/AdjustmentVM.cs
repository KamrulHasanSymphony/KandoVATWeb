using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class AdjustmentVM
    {
        public string AdjId { get; set; }
        public string AdjName { get; set; }
        public string Comments { get; set; }
        public bool ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string Operation { get; set; }

    }

   

}
