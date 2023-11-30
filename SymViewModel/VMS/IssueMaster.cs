using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace SymViewModel.VMS
{
    public class IssueMasterVM
    {
        public string Id { get; set; }
        [Display(Name = "Issue No")]
        public string IssueNo { get; set; }
        public string IssueDateTime { get; set; }
        public decimal TotalVATAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string SerialNo { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string ReceiveNo { get; set; }
        public string transactionType { get; set; }
        public string Post { get; set; }
        public string ImportId { get; set; }
        public string ReturnId { get; set; }
        public string Operation { get; set; }
        [Display(Name = "Issue Date From")]
        public string IssueDateTimeFrom { get; set; }
        [Display(Name = "Issue Date To")]
        public string IssueDateTimeTo { get; set; }
        public List<IssueDetailVM> Details { get; set; }
        public string TargetId { get; set; }

        public string ProductType { get; set; }

        public int ProductCategoryId { get; set; }
        public HttpPostedFile File { get; set; }
    }

    public class IssueDetailVM
    {

        public string IssueNo { get; set; }
        public string IssueLineNo { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal NBRPrice { get; set; }
        public decimal CostPrice { get; set; }
        public string UOM { get; set; }
        public decimal VATRate { get; set; }
        public decimal VATAmount { get; set; }
        public decimal SubTotal { get; set; }
        public string CommentsD { get; set; }
        public string ReceiveNoD { get; set; }
        public string IssueDateTimeD { get; set; }
        public decimal SD { get; set; }
        public decimal SDAmount { get; set; }
        public decimal Wastage { get; set; }
        public decimal WQty { get; set; }
        public string BOMDate { get; set; }
        public string FinishItemNo { get; set; }
        public string FinishItemName { get; set; }
        public decimal UOMQty { get; set; }
        public string UOMn { get; set; }
        public decimal UOMc { get; set; }
        public decimal UOMPrice { get; set; }
        public string BomId { get; set; }
        public string Operation { get; set; }
        public string ItemName { get; set; }
        public string ProductCode { get; set; }
        public bool Post { get; set; }
    }
}
