using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace SymViewModel.VMS
{
    public class PurchaseMasterVM
    {
        public string Id { get; set; }
        //[Display(Name = "Purchase No")]
        public string PurchaseInvoiceNo { get; set; }
        public string VendorID { get; set; }
        //[Display(Name = "Vendor Group")]
        public string VendorGroup { get; set; }
        //[Display(Name = "Invoice Date")]
        public string InvoiceDate { get; set; }
        //[Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        //[Display(Name = "Total VAT Amount")]
        public decimal TotalVATAmount { get; set; }
        //[Display(Name = "Ref No")]
        public string SerialNo { get; set; }
        //[Display(Name = "LC No")]
        public string LCNumber { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        //[Display(Name = "BE No")]
        public string BENumber { get; set; }
        //[Display(Name = "Product Type")]
        public string ProductType { get; set; }//IsRaw
        public string TransactionType { get; set; }
        public string ReceiveDate { get; set; }
        public string Post { get; set; }
        //[Display(Name = "With VDS")]
        public string WithVDS { get; set; }
        //[Display(Name = "Return Id")]
        public string ReturnId { get; set; }
        //[Display(Name = "Import Id")]
        public string ImportID { get; set; }
        //[Display(Name = "LC Date")]
        public string LCDate { get; set; }
        //[Display(Name = "Landed Cost")]
        public decimal LandedCost { get; set; }
        //[Display(Name = "Custom House")]
        public string CustomHouse { get; set; }

        public List<PurchaseDetailVM> Details { get; set; }
        public List<PurchaseDutiesVM> Duties { get; set; }
        public string Operation { get; set; }
        //[Display(Name = "Invoice Date From")]
        public string InvoiceDateTimeFrom { get; set; }
        //[Display(Name = "Invoice Date To")]
        public string InvoiceDateTimeTo { get; set; }
        //[Display(Name = "Vendor Name")]
        public string VendorName { get; set; }
        public bool IsImport { get; set; }


        public string Type { get; set; }

        public int ProductCategoryId { get; set; }

        public HttpPostedFileBase File { get; set; }
        //[Display(Name = "Search Field")]
        public string SearchField { get; set; }
        //[Display(Name = "Search Value")]
        public string SearchValue { get; set; }
    }

    public class PurchaseDutiesVM
    {
        public string PIDutyID { get; set; }
        public string PurchaseInvoiceNo { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal DutyCompleteQuantity { get; set; }
        public decimal DutyCompleteQuantityPercent { get; set; }
        public decimal CnFInp { get; set; }
        public decimal CnFRate { get; set; }
        public decimal CnFAmount { get; set; }
        public decimal InsuranceInp { get; set; }
        public decimal InsuranceRate { get; set; }
        public decimal InsuranceAmount { get; set; }
        public decimal AssessableInp { get; set; }
        public decimal AssessableValue { get; set; }
        public decimal CDInp { get; set; }
        public decimal CDRate { get; set; }
        public decimal CDAmount { get; set; }
        public decimal RDInp { get; set; }
        public decimal RDRate { get; set; }
        public decimal RDAmount { get; set; }
        public decimal TVBInp { get; set; }
        public decimal TVBRate { get; set; }
        public decimal TVBAmount { get; set; }
        public decimal SDInp { get; set; }
        public decimal SD { get; set; }
        public decimal SDAmount { get; set; }
        public decimal VATInp { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal TVAInp { get; set; }
        public decimal TVARate { get; set; }
        public decimal TVAAmount { get; set; }
        public decimal ATVInp { get; set; }
        public decimal ATVRate { get; set; }
        public decimal ATVAmount { get; set; }
        public decimal OthersInp { get; set; }
        public decimal OthersRate { get; set; }
        public decimal OthersAmount { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineCost { get; set; }
        public string Remarks { get; set; }

        public void SetCost()
        {
            LineCost = CnFAmount +
                       InsuranceAmount +
                       AssessableValue +
                       CDAmount +
                       RDAmount +
                       TVBAmount +
                       SDAmount +
                       VATAmount +
                       TVAAmount +
                       ATVAmount +
                       OthersAmount;
            if (LineCost > 0)
            {
                UnitCost = LineCost / Quantity;
            }
        }


    }
    public class PurchaseDetailVM
    {
        public string PurchaseInvoiceNo { get; set; }
        public string LineNo { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal NBRPrice { get; set; }
        public string UOM { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SubTotal { get; set; }
        public string Comments { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime CreatedOn { get; set; }
        //public string LastModifiedBy { get; set; }
        //public DateTime LastModifiedOn { get; set; }
        public decimal SD { get; set; }
        public decimal SDAmount { get; set; }
        public string Type { get; set; }
        public string ProductType { get; set; }
        public string BENumber { get; set; }
        public decimal UOMQty { get; set; }
        public string UOMn { get; set; }
        public decimal UOMc { get; set; }
        public decimal UOMPrice { get; set; }


        public decimal RebateRate { get; set; }
        public decimal RebateAmount { get; set; }
        public decimal CnFAmount { get; set; }
        public decimal InsuranceAmount { get; set; }
        public decimal AssessableValue { get; set; }
        public decimal CDAmount { get; set; }
        public decimal RDAmount { get; set; }
        public decimal TVBAmount { get; set; }
        public decimal TVAAmount { get; set; }
        public decimal ATVAmount { get; set; }
        public decimal OthersAmount { get; set; }
        public string ReturnTransactionType { get; set; }

        public decimal Total { get; set; }
        public string ProductName { get; set; }
        public string InvoiceDateTime { get; set; }
        public string ReceiveDate { get; set; }
        public string ProductCode { get; set; }
        public bool Post { get; set; }

        public string[] retResults { get; set; }

    }

    public class TrackingVM
    {
        public string ItemNo { get; set; }
        public string TrackingLineNo { get; set; }
        public string Heading1 { get; set; }
        public string Heading2 { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string FinishItemNo { get; set; }

        public string IsPurchase { get; set; }
        public string IsIssue { get; set; }
        public string IsReceive { get; set; }
        public string IsSale { get; set; }

        public string PurchaseInvoiceNo { get; set; }
        public string IssueNo { get; set; }
        public string ReceiveNo { get; set; }
        public string SaleInvoiceNo { get; set; }
        public string ReceiveDate { get; set; }

        public string ReturnPurchase { get; set; }
        public string ReturnReceive { get; set; }
        public string ReturnSale { get; set; }
        public string ReturnPurchaseID { get; set; }
        public string ReturnReceiveID { get; set; }
        public string ReturnSaleID { get; set; }
        public string transactionType { get; set; }
        public string ReturnType { get; set; }
        public string Post { get; set; }

        public string ReturnReceiveDate { get; set; }
        public string ReturnPurDate { get; set; }

        public string ProductCode { get; set; }
        public string ProductName { get; set; }
    }
}
