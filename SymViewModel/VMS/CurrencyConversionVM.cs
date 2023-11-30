using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class CurrencyConversionVM
    {
        public string CurrencyConversionId { get; set; }
        public string CurrencyCodeFrom { get; set; }
        public string CurrencyCodeTo { get; set; }
        public string CurrencyNameFrom { get; set; }
        public string CurrencyNameTo { get; set; }
        public decimal CurrencyRate { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string ConvertionDate { get; set; }

        public string Operation { get; set; }

    }
}
