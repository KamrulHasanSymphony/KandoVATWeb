using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VATViewModel.DTOs;

namespace SymVATWebUI.Areas.VMS.Models
{
    public class Vat16ViewModel
    {
        public string ItemNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int FontSize { get; set; }
        public string InEnglish { get; set; }
        public bool PreviewOnly { get; set; }
        public string VatName { get; set; }
        public string SaleType { get; set; }
        public string PrinterName { get; set; }
        public string FiscalYear { get; set; }
        public string BOMId { get; set; }
        public int Branch { get; set; }
        public int BranchId { get; set; }
        public string pageNo { get; set; }
        public string copyNo { get; set; }
        public string SalesInvoiceNo { get; set; }
        public string DepositId { get; set; }
        public string TransactionTypes { get; set; }
        public string Post { get; set; }
        public bool IsMonthly { get; set; }
        public bool BranchWise { get; set; }

        public int Id { get; set; }

        public List<string> IDs { get; set; }

        public string Json { get; set; }
        public string BillNo { get; set; }
        public List<BranchProfileVM> BranchList { get; set; }

    }
}