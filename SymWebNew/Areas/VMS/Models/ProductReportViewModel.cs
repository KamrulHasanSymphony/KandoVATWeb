using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SymVATWebUI.Areas.VMS.Models
{
    public class ProductReportViewModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductGroup { get; set; }
        public string ProductType { get; set; }
        public string ItemNo { get; set; }
    }
}