using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
   public class FiscalYearVM
    {
       public string FiscalYearName { get; set; }
       public string CurrentYear { get; set; }
       public string PeriodID { get; set; }
       public string PeriodName { get; set; }
       public string PeriodStart { get; set; }
       public string PeriodEnd { get; set; }
       public string PeriodLock { get; set; }
       public string GLLock { get; set; }
       public string CreatedBy { get; set; }
       public string CreatedOn { get; set; }
       public string LastModifiedBy { get; set; }
       public string LastModifiedOn { get; set; }
        
    }
}
