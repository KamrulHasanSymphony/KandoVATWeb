using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class TransferRawMasterVM
    {
        public string Id { get; set; }
        public string TransferId { get; set; }
        public string TransferDateTime { get; set; }
        public string TransFromItemNo { get; set; }
        public string CategoryId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal TransferedQty { get; set; }
        public decimal TransferedAmt { get; set; }
        public string UOM { get; set; }
        public string TransactionType { get; set; }
        public string Post { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Operation { get; set; }
        public List<TransferRawDetailVM> Details { get; set; }
        public string RawCode { get; set; }
        public string RawName { get; set; }
        public string IssueDateTimeFrom { get; set; }
        public string IssueDateTimeTo { get; set; }
        
    }
    public class TransferRawDetailVM
    {
        public string TransferIdD { get; set; }
        public string TransFromItemNo { get; set; }
        public string TransLineNo { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public string UOM { get; set; }
        public decimal SubTotal { get; set; }
        public decimal UOMQty { get; set; }
        public decimal UOMPrice { get; set; }
        public decimal UOMc { get; set; }
        public string UOMn { get; set; }
        public string groupId { get; set; }
    }
}
