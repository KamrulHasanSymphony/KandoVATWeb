using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class AdjustmentHistoryVM
    {
        public string AdjHistoryID { get; set; }
        public string AdjHistoryNo { get; set; }
        public string AdjId { get; set; }
        public string AdjDate { get; set; }
        public decimal AdjInputAmount { get; set; }
        public decimal AdjInputPercent { get; set; }
        public decimal AdjAmount { get; set; }
        public decimal AdjVATRate { get; set; }
        public decimal AdjVATAmount { get; set; }
        public decimal AdjSD { get; set; }
        public decimal AdjSDAmount { get; set; }
        public decimal AdjOtherAmount { get; set; }
        public string AdjType { get; set; }
        public string AdjDescription { get; set; }
        public string AdjReferance { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string HeadName { get; set; }
        public string Post { get; set; }
        public string Operation { get; set; }
    }

    public class CashPayableVM
    {
        public AdjustmentHistoryVM Adjustment { get; set; }

        public DepositMasterVM Deposit { get; set; }

        public string Operation { get; set; }
    }
}
