using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class DDBHeaderVM
    {

        public string DDBackNo { get; set; }
        public string DDBackDate { get; set; }
        public string SalesInvoiceNo { get; set; }
        public string SalesDate { get; set; }
        public string CustormerID { get; set; }
        public int CurrencyId { get; set; }
        public decimal ExpCurrency { get; set; }
        public decimal BDTCurrency { get; set; }
        public string FgItemNo { get; set; }
        public decimal TotalClaimCD { get; set; }
        public decimal TotalClaimRD { get; set; }
        public decimal TotalClaimSD { get; set; }
        public decimal TotalDDBack { get; set; }
        public decimal TotalClaimVAT { get; set; }
        public decimal TotalClaimCnFAmount { get; set; }
        public decimal TotalClaimInsuranceAmount { get; set; }
        public decimal TotalClaimTVBAmount { get; set; }
        public decimal TotalClaimTVAAmount { get; set; }
        public decimal TotalClaimATVAmount { get; set; }
        public decimal TotalClaimOthersAmount { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Post { get; set; }



    }

    public class DDBDetailsVM
    {

        public string DDBackNo { get; set; }
        public string DDBackDate { get; set; }
        public int DDLineNo { get; set; }
        public string PurchaseInvoiceNo { get; set; }
        public string PurchaseDate { get; set; }
        public string FgItemNo { get; set; }
        public string ItemNo { get; set; }
        public string BillOfEntry { get; set; }
        public string PurchaseUom { get; set; }
        public decimal PurchaseQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal AV { get; set; }
        public decimal CD { get; set; }
        public decimal RD { get; set; }
        public decimal SD { get; set; }
        public decimal VAT { get; set; }
        public decimal CnF { get; set; }
        public decimal Insurance { get; set; }
        public decimal TVB { get; set; }
        public decimal TVA { get; set; }
        public decimal ATV { get; set; }
        public decimal Others { get; set; }
        public decimal UseQuantity { get; set; }
        public decimal ClaimCD { get; set; }
        public decimal ClaimRD { get; set; }
        public decimal ClaimSD { get; set; }
        public decimal ClaimVAT { get; set; }
        public decimal ClaimCnF { get; set; }
        public decimal ClaimInsurance { get; set; }
        public decimal ClaimTVB { get; set; }
        public decimal ClaimTVA { get; set; }
        public decimal ClaimATV { get; set; }
        public decimal ClaimOthers { get; set; }
        public decimal SubTotalDDB { get; set; }
        public decimal UOMc { get; set; }
        public string UOMn { get; set; }
        public decimal UOMCD { get; set; }
        public decimal UOMRD { get; set; }
        public decimal UOMSD { get; set; }
        public decimal UOMVAT { get; set; }
        public decimal UOMCnF { get; set; }
        public decimal UOMInsurance { get; set; }
        public decimal UOMTVB { get; set; }
        public decimal UOMTVA { get; set; }
        public decimal UOMATV { get; set; }
        public decimal UOMOthers { get; set; }
        public decimal UOMSubTotalDDB { get; set; }
        public string Post { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string SalesInvoiceNo { get; set; }
        public decimal FGQty { get; set; }
        public string PTransType { get; set; }

        

      



    }
}
