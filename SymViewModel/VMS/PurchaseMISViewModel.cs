using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class PurchaseMISViewModel
    {
        public string PurchaseNo { get; set; }
        public string VendorGroup { get; set; }
        public string VendorName { get; set; }
        public string ProductGroup { get; set; }
        public string ProductType { get; set; }
        public string  ProductName { get; set; }
        public string ItemNo { get; set; }
        public string ReceiveDateFrom { get; set; }
        public string ReceiveDateTo { get; set; }
        public bool Post { get; set; }
        public bool Duty { get; set; }
        public bool PreviewOnly { get; set; }
        public string ReportType { get; set; }
        public string LC { get; set; }
    }
}
