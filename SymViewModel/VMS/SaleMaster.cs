using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SymViewModel.VMS
{
    public class SaleMasterVM
    {
        public string SalesInvoiceNo { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }
        public string DeliveryAddress3 { get; set; }
        public string VehicleType { get; set; }
        public string InvoiceDateTime { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalVATAmount { get; set; }
        public string SerialNo { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string SaleType { get; set; }
        public string PreviousSalesInvoiceNo { get; set; }
        public string Trading { get; set; }
        public string IsPrint { get; set; }
        public string TenderId { get; set; }
        public string TransactionType { get; set; }
        public string DeliveryDate { get; set; }
        public string VehicleNo { get; set; }
        public bool vehicleSaveInDB { get; set; }
        public string Post { get; set; }
        public string ReturnId { get; set; }
        public string CurrencyID { get; set; }
        public decimal CurrencyRateFromBDT { get; set; }
        public string ImportID { get; set; }
        public string LCNumber { get; set; }
        public string CompInvoiceNo { get; set; }

        public string Operation { get; set; }
        public string InvoiceDateTimeFrom { get; set; }
        public string InvoiceDateTimeTo { get; set; }
        public string Id { get; set; }

        public string LCBank { get; set; }
        public string LCDate { get; set; }

        public string VehicleID { get; set; }

        public List<SaleDetailVM> Details { get; set; }
        public string CustomerGroup { get; set; }
        public string CurrencyCode { get; set; }
        public Decimal DollerRate { get; set; }


        public string ConversionDate { get; set; }


         public string ProductGroup  { get; set; }
         public string ProductType  { get; set; }
         public int ProductCategoryId  { get; set; }

        public string Type { get; set; }
        public string VatName { get; set; }

        public HttpPostedFileBase File { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }

    }
    public class SaleDetailVM
    {
        public string InvoiceLineNo { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal PromotionalQuantity { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal NBRPrice { get; set; }
        public string UOM { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SubTotal { get; set; }
        public string CommentsD { get; set; }
        public decimal SD { get; set; }
        public decimal SDAmount { get; set; }
        public string SaleTypeD { get; set; }
        public string PreviousSalesInvoiceNoD { get; set; }
        public string TradingD { get; set; }
        public string NonStockD { get; set; }
        public decimal TradingMarkUp { get; set; }
        public string Type { get; set; }
        public decimal UOMQty { get; set; }
        public string UOMn { get; set; }
        public decimal UOMc { get; set; }
        public decimal UOMPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountedNBRPrice { get; set; }
        public decimal DollerValue { get; set; }
        public decimal CurrencyValue { get; set; }
        public string VatName { get; set; }
        public string CConversionDate { get; set; }
        public string ReturnTransactionType { get; set; }
        //CP use
        public string Weight { get; set; }
        public string ValueOnly { get; set; }
        public bool Post { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        //public decimal PQuantiy { get; set; }
        public decimal Total { get; set; }
        public decimal BDTValue { get; set; }
        //new fields added on 5/5/2019
        public decimal TotalValue { get; set; }
        public decimal WareHouseRent { get; set; }
        public decimal WareHouseVAT { get; set; }
        public decimal ATVRate { get; set; }
        public decimal ATVablePrice { get; set; }
        public decimal ATVAmount { get; set; }
        public string IsCommercialImporter { get; set; }

    }

    public class SaleExportVM
    {
        public string SaleLineNo { get; set; }
        public string RefNo { get; set; }
        
        public string Description { get; set; }
        public string QuantityE { get; set; }
        public string GrossWeight { get; set; }
        public string NetWeight { get; set; }
        public string NumberFrom { get; set; }
        public string NumberTo { get; set; }
        public string CommentsE { get; set; }


    }

    public class BureauSaleDetailVM
    {
        public string InvoiceLineNo { get; set; }
        public string InvoiceName { get; set; }
        public string InvoiceDateTime { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalesPrice { get; set; }
        public string ItemNo { get; set; }
        public decimal SD { get; set; }
        public decimal SDAmount { get; set; }
        public string UOM { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SubTotal { get; set; }
       
        public string Type { get; set; }
        public string PreviousSalesInvoiceNo { get; set; }
        public string ChallanDateTime { get; set; }
        public decimal DollerValue { get; set; }
        public decimal CurrencyValue { get; set; }
        public string InvoiceCurrency { get; set; }
        public string CConversionDate { get; set; }
        public string ReturnTransactionType { get; set; }
        public string BureauType { get; set; }
        public string BureauId { get; set; }
        




    }
}
