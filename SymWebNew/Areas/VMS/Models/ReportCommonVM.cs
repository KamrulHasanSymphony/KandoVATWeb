using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymVATWebUI.Areas.VMS.Models
{
    public class ReportCommonVM
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int Branch { get; set; }
        public int Id { get; set; }

        public int TransferBranch { get; set; }

        public bool PreviewOnly { get; set; }

        public string PrinterName { get; set; }
        public string IssueNo { get; set; }
        public string TransactionType { get; set; }
    

        public int FontSize { get; set; }

        public string Json { get; set; }
        public string Post { get; set; }

    }
}