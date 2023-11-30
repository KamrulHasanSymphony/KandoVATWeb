using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class VAT_16VM
    {

        public decimal Column1 { get; set; }  // Serial No   
        public DateTime Column2 { get; set; }  // Date
        public decimal Column3 { get; set; }  // Opening Quantity
        public decimal Column4 { get; set; }  // Opening Price
        public string Column5 { get; set; }  // Invoice/BE No
        public DateTime Column6 { get; set; }  // Invoice/BE Date
        public string Column6String { get; set; }  // Invoice/BE Date
        public string Column7 { get; set; }  // Vendor Name
        public string Column8 { get; set; }  // Vendor Address
        public string Column9 { get; set; }  // Vendor VAT Reg No
        public string Column10 { get; set; } // Product Information
        public decimal Column11 { get; set; } // Purchase Quantity
        public decimal Column12 { get; set; } // Price(Sub total) without VAT and SD
        public decimal Column13 { get; set; } // SD Amount
        public decimal Column14 { get; set; } // VAT Amount
        public decimal Column15 { get; set; } // Issue Quantity for Production
        public decimal Column16 { get; set; } // Issue Price for Production
        public decimal Column17 { get; set; } // Closing Quantity
        public decimal Column18 { get; set; } // Closing Amount
        public string Column19 { get; set; } // Remarks
        public string temp1 { get; set; } // Remarks
        public string temp2 { get; set; } // Remarks
        public string temp3 { get; set; } // Remarks

    }
}
