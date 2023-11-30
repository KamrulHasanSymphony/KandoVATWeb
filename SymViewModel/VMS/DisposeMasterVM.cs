using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class DisposeMasterVM
    {
        ////master properties
        //string DisposeNumber = DisposeHeaderFields[0];
        //DateTime DisposeDate = Convert.ToDateTime(DisposeHeaderFields[1]);
        //string RefNumber = DisposeHeaderFields[2];
        //decimal VATAmount = Convert.ToDecimal(DisposeHeaderFields[3]);
        //string Remarks = DisposeHeaderFields[4];
        //string CreatedBy = DisposeHeaderFields[5];
        //DateTime CreatedOn = Convert.ToDateTime(DisposeHeaderFields[6]);
        //string LastModifiedBy = DisposeHeaderFields[7];
        //DateTime LastModifiedOn = Convert.ToDateTime(DisposeHeaderFields[8]);
        //string TransactionType = DisposeHeaderFields[9];
        //string Post = DisposeHeaderFields[10];
        //string FromStock = DisposeHeaderFields[11];
        //decimal ImportVATAmount = Convert.ToDecimal(DisposeHeaderFields[12]);
        //decimal TotalPrice = Convert.ToDecimal(DisposeHeaderFields[13]);
        //decimal TotalPriceImport = Convert.ToDecimal(DisposeHeaderFields[14]);
        //decimal AppVATAmount = Convert.ToDecimal(DisposeHeaderFields[15]);
        //decimal AppTotalPrice = Convert.ToDecimal(DisposeHeaderFields[16]);
        //DateTime AppDate = Convert.ToDateTime(DisposeHeaderFields[17]);
        //string AppRefNumber = DisposeHeaderFields[18];
        //string AppRemarks = DisposeHeaderFields[19];
        //decimal AppVATAmountImport = Convert.ToDecimal(DisposeHeaderFields[20]);
        //decimal AppTotalPriceImport = Convert.ToDecimal(DisposeHeaderFields[21]);
        ////master properties

        //master properties
        public string DisposeNumber { get; set; }
        public string DisposeDate { get; set; }
        public string RefNumber { get; set; }
        public decimal VATAmount { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string TransactionType { get; set; }
        public string Post { get; set; }
        public string FromStock { get; set; }
        public decimal ImportVATAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalPriceImport { get; set; }
        public decimal AppVATAmount { get; set; }
        public decimal AppTotalPrice { get; set; }
        public string AppDate { get; set; }
        public string AppRefNumber { get; set; }
        public string AppRemarks { get; set; }
        public decimal AppVATAmountImport { get; set; }
        public decimal AppTotalPriceImport { get; set; }
        //master properties

    }

    public class DisposeDetailVM
    {
        ////details properties
        //string DisposeNumberD = DisposeHeaderResult1[1];
        //string LineNumber = DisposeDetailFields[1];
        //string ItemNo = DisposeDetailFields[2];
        //decimal Quantity = Convert.ToDecimal(DisposeDetailFields[3]);
        //decimal RealPrice = Convert.ToDecimal(DisposeDetailFields[4]);
        //decimal VATAmountD = Convert.ToDecimal(DisposeDetailFields[5]);
        //string SaleNumber = DisposeDetailFields[6];
        //string PurchaseNumber = DisposeDetailFields[7];
        //decimal PresentPrice = Convert.ToDecimal(DisposeDetailFields[8]);
        //string CreatedByD = DisposeDetailFields[9];
        //DateTime CreatedOnD = Convert.ToDateTime(DisposeDetailFields[10]);
        //string LastModifiedByD = DisposeDetailFields[11];
        //DateTime LastModifiedOnD = Convert.ToDateTime(DisposeDetailFields[12]);
        //DateTime DisposeDateD = Convert.ToDateTime(DisposeDetailFields[13]);
        //string UOM = DisposeDetailFields[14];
        //string RemarksD = DisposeDetailFields[15];
        //string QuantityImport = DisposeDetailFields[16];
        //decimal VATRate = Convert.ToDecimal(DisposeDetailFields[17]);
        ////details properties

        //details properties
        public string DisposeNumberD { get; set; }
        public string LineNumber { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal RealPrice { get; set; }
        public decimal VATAmountD { get; set; }
        public string SaleNumber { get; set; }
        public string PurchaseNumber { get; set; }
        public decimal PresentPrice { get; set; }
        public string CreatedByD { get; set; }
        public string CreatedOnD { get; set; }
        public string LastModifiedByD { get; set; }
        public string LastModifiedOnD { get; set; }
        public string DisposeDateD { get; set; }
        public string UOM { get; set; }
        public string RemarksD { get; set; }
        public string QuantityImport { get; set; }
        public decimal VATRate { get; set; }
        //details properties

    }
}
