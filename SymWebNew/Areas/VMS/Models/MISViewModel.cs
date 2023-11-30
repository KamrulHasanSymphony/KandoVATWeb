using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SymVATWebUI.Areas.VMS.Models
{
    public class MISViewModel
    {
        public string PurchaseId { get; set; }
        public string PurchaseNo { get; set; }
        public string ReceiveDateFrom { get; set; }
        public string ReceiveDateTo { get; set; }
        public string VendorGroup { get; set; }
        public string VendorName { get; set; }
        public string ProductGroup { get; set; }
        public string ProductName { get; set; }
        public string Post { get; set; }
        public bool Duty { get; set; }
        public bool PreviewOnly { get; set; }
        public string ReportType { get; set; }

    }
}