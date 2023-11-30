using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class CurrencyVM
    {
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public string Country { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string CurrencyMajor { get; set; }
        public string CurrencyMinor { get; set; }
        public string CurrencySymbol { get; set; }
        public string Operation { get; set; }
    }
}
