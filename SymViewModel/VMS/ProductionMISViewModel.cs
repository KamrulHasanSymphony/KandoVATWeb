using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class ProductionMISViewModel
    {
        public string IssueNo { get; set; }
        public string IssueDateFrom { get; set; }
        public string IssueDateTo { get; set; }
        public string ProductName { get; set; }
        public string ItemNo { get; set; }
        public string ProductGroup { get; set; }
        public string ProductType { get; set; }
        public string ReportType { get; set; }
        public bool Post { get; set; }
        public bool Wastage { get; set; }
        public bool PreviewOnly { get; set; }
        public string shift { get; set; }
    }
}
