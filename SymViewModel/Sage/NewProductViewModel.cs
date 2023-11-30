using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class NewProductViewModel
    {
        public string ItemNo{ get; set; }
        [Display(Name = "Product Code")]
        public string ProductCode{ get; set; }
        [Display(Name = "Product Name")]
        public string ProductName{ get; set; }
        [Display(Name = "Product description")]
        public string ProductDescription{ get; set; }
        [Display(Name = "Category ID")]
        public string CategoryID { get; set; }
        public string UOM { get; set; }
        [Display(Name = "Cost Price")]
        public decimal CostPrice { get; set; }
        [Display(Name = "Sales Price")]
        public decimal SalesPrice { get; set; }
        [Display(Name = "NBR Price")]
        public decimal NBRPrice { get; set; }
        [Display(Name = "Receive Price")]
        public decimal ReceivePrice { get; set; }
        [Display(Name = "Issue Price")]
        public decimal IssuePrice { get; set; }
        [Display(Name = "Tender Price")]
        public decimal TenderPrice { get; set; }
        [Display(Name = "Export Price")]
        public decimal ExportPrice { get; set; }
        [Display(Name = "Internal Issue Price")]
        public decimal InternalIssuePrice { get; set; }
        [Display(Name = "Toll Issue Price")]
        public decimal TollIssuePrice { get; set; }
        [Display(Name = "Toll Charge")]
        public decimal TollCharge { get; set; }
        [Display(Name = "Opening Balance")]
        public decimal OpeningBalance { get; set; }
        [Display(Name = "Serial No")]
        public string SerialNo { get; set; }
        [Display(Name = "HS Code No")]
        public string HSCodeNo { get; set; }
        [Display(Name = "VAT Rate")]
        public decimal VATRate { get; set; }
        [Display(Name = "Comments")]
        public string Comments { get; set; }
        [Display(Name = "SD")]
        public decimal SD { get; set; }
        [Display(Name = "Packet Price")]
        public decimal PacketPrice { get; set; }
        public bool Trading { get; set; }
        [Display(Name = "Trading MarkUp")]
        public string TradingMarkUp { get; set; }
        public bool NonStock { get; set; }
        [Display(Name = "Quantity")]
        public decimal QuantityInHand { get; set; }
        [Display(Name = "Opening Date")]
        public DateTime? OpeningDate { get; set; }
        [Display(Name = "Rebate Percent")]
        public decimal RebatePercent { get; set; }
        [Display(Name = "TVB Rate")]
        public decimal TVBRate { get; set; }
        [Display(Name = "CnF Rate")]
        public decimal CnFRate { get; set; }
        [Display(Name = "Insurance Rate")]
        public decimal InsuranceRate { get; set; }
        [Display(Name = "CD Rate")]
        public decimal CDRate { get; set; }
        [Display(Name = "RD Rate")]
        public decimal RDRate { get; set; }
        [Display(Name = "AIT Rate")]
        public decimal AITRate { get; set; }
        [Display(Name = "TVA Rate")]
        public decimal TVARate { get; set; }
        [Display(Name = "ATV Rate")]
        public decimal ATVRate { get; set; }
        public bool ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        [Display(Name = "Opening Total Cost")]
        public decimal OpeningTotalCost { get; set; }
        public bool Banderol { get; set; }
        public bool IsVATRate { get; set; }
        public bool IsSDRate { get; set; }
        public string Operation { get; set; }

    }
}
