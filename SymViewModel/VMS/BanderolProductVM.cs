using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class BanderolProductVM
    {
        public string BandProductId { get; set; }
        public string ItemNo { get; set; }
        public string BanderolId { get; set; }
        public string PackagingId { get; set; }
        public decimal BUsedQty { get; set; }
        public bool ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public decimal WastageQty { get; set; }
        public decimal OpeningQty { get; set; }
        public string OpeningDate { get; set; }
    }

   

}
