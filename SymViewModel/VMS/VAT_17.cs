using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class VAT_17VM
    {
        public decimal Column1 { get; set; }  // Serial No   
        public DateTime Column2 { get; set; }  // Date
        public decimal Column3 { get; set; }  // Opening Quantity
        public decimal Column4 { get; set; }  // Opening Price
        public decimal Column5 { get; set; }  // Production Quantity
        public decimal Column6 { get; set; }  // Production Price
        public string Column7 { get; set; }  // Customer Name
        public string Column8 { get; set; }  // Customer VAT Reg No
        public string Column9 { get; set; }  // Customer Address
        public string Column10 { get; set; } // Sale Invoice No
        public DateTime Column11 { get; set; } // Sale Invoice Date and Time
        public string Column11string { get; set; } // Sale Invoice Date and Time
        public string Column12 { get; set; } // Sale Product Name
        public decimal Column13 { get; set; } // Sale Product Quantity
        public decimal Column14 { get; set; } // Sale Product Sale Price(NBR Price with out VAT and SD amount)
        public decimal Column15 { get; set; } // SD Amount
        public decimal Column16 { get; set; } // VAT Amount
        public decimal Column17 { get; set; } // Closing Quantity
        public decimal Column18 { get; set; } // Closing Amount
        public string Column19 { get; set; } // Remarks
        public string temp1 { get; set; } // Remarks
        public string temp2 { get; set; } // Remarks
        public string temp3 { get; set; } // Remarks

    }
}
