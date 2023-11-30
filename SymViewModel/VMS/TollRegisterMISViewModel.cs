using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class TollRegisterMISViewModel
    {
        public string ProductName { get; set; }
        public string ItemNo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public bool Preview { get; set; }
    }
}
