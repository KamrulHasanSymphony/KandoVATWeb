using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class ChartVM
    {
        public string BranchName { get; set; }
        public string Year { get; set; }
        public string PeriodName { get; set; }
        public string AccountName { get; set; }
        public decimal TransactionAmount { get; set; }

        public string DocumentType { get; set; }

    }
}
