using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class IssueDetailViewModel
    {
        public int Id { get; set; }
        public int MasterId { get; set; }
        public string IssueNo { get; set; }
        public int IssueLineNo { get; set; }
        public int ItemNo { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public decimal NBRPrice { get; set; }
        public decimal CostPrice { get; set; }
        public string UOM { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SubTotal { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string ReceiveNo { get; set; }
        public string IssueDateTime { get; set; }
        public decimal SD { get; set; }
        public decimal SDAmount { get; set; }
        public decimal Wastage { get; set; }
        public DateTime BOMDate { get; set; }
        public string FinishItemNo { get; set; }
        public bool Post { get; set; }
        public string TransactionType { get; set; }
        public string IssueReturnId { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountedNBRPrice { get; set; }
        public decimal UOMQty { get; set; }
        public decimal UOMPrice { get; set; }
        public decimal UOMc { get; set; }
        public string UOMn { get; set; }
        public string BOMId { get; set; }
        public decimal UOMWastage { get; set; }
        public bool IsProcess { get; set; }
        public string Operation { get; set; }
        
    }
}
