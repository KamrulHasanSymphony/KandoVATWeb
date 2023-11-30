using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class BanderolVM
    {
        public string BanderolID { get; set; }
        public string BanderolName { get; set; }
        public string BanderolSize { get; set; }
        public string UOM { get; set; }
        public decimal OpeningQty { get; set; }
        public string OpeningDate { get; set; }
        public string Description { get; set; }
        public bool ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }
}
