using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class DepositMISViewModel
    {
        public string DepositNo { get; set; }
        public string DepositDateFrom { get; set; }
        public string DepositDateTo { get; set; }
        public bool Post { get; set; }
        public string ReportType { get; set; }
        public string BankName { get; set; }
        public string BankId { get; set; }
    }
}
