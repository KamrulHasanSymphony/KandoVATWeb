using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLPettyCashRequisitionVM
    {

        public int Id { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public string Code { get; set; }
        [Display(Name = "Transaction Date")]
        public string TransactionDateTime { get; set; }
        [Display(Name = "Commission Bill No.")]
        public string CommissionBillNo { get; set; }


        [Display(Name = "Reference 1")]
        public string ReferenceNo1 { get; set; }
        [Display(Name = "Reference 2")]
        public string ReferenceNo2 { get; set; }
        [Display(Name = "Reference 3")]
        public string ReferenceNo3 { get; set; }
        public bool Post { get; set; }


        [Display(Name = "Opening Balance")]public decimal OpeningBalance { get; set; }
        [Display(Name = "Total Expense")]public decimal TotalExpense { get; set; }
        [Display(Name = "Total Agency Commission")]public decimal TotalAgencyCommission { get; set; }
        [Display(Name = "Total Requisition")]public decimal TotalRequisition { get; set; }
        [Display(Name = "Fund Receive")]public decimal FundReceive { get; set; }
        [Display(Name = "Next Opening")]public decimal NextOpening { get; set; }

        [Display(Name = "Requisition Expense")]public decimal RequisitionExpense { get; set; }
        [Display(Name = "Requisition BankCharge")]public decimal RequisitionBankCharge { get; set; }
        [Display(Name = "Requisition Contingency")]public decimal RequisitionContingency { get; set; }
        [Display(Name = "Requisition Agency Commission")]public decimal RequisitionAgencyCommission { get; set; }



        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }

         #region Maker-Checker-Audit
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Archive")]
        public bool IsArchive { get; set; }
        [Display(Name = "Make By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Make At")]
        public string CreatedAt { get; set; }
        [Display(Name = "Maker From")]
        public string CreatedFrom { get; set; }
        [Display(Name = "Update By")]
        public string LastUpdateBy { get; set; }
        [Display(Name = "Update At")]
        public string LastUpdateAt { get; set; }
        [Display(Name = "Update From")]
        public string LastUpdateFrom { get; set; }
        [Display(Name = "Post By")]
        public string PostedBy { get; set; }
        [Display(Name = "Post At")]
        public string PostedAt { get; set; }
        [Display(Name = "Post From")]
        public string PostedFrom { get; set; }
        [Display(Name = "Approved L1")]
        public bool IsApprovedL1 { get; set; }
        [Display(Name = "Appr. By L1")]
        public string ApprovedByL1 { get; set; }
        [Display(Name = "Appr. At L1")]
        public string ApprovedDateL1 { get; set; }
        [Display(Name = "L1 Comments")]
        public string CommentsL1 { get; set; }
        [Display(Name = "Approved L2")]
        public bool IsApprovedL2 { get; set; }
        [Display(Name = "Appr. By L2")]
        public string ApprovedByL2 { get; set; }
        [Display(Name = "Appr. At L1")]
        public string ApprovedDateL2 { get; set; }
        [Display(Name = "L2 Comments")]
        public string CommentsL2 { get; set; }
        [Display(Name = "Approved L3")]
        public bool IsApprovedL3 { get; set; }
        [Display(Name = "Appr. By L3")]
        public string ApprovedByL3 { get; set; }
        [Display(Name = "Appr. At L3")]
        public string ApprovedDateL3 { get; set; }
        [Display(Name = "L3 Comments")]
        public string CommentsL3 { get; set; }
        [Display(Name = "Approved L4")]
        public bool IsApprovedL4 { get; set; }
        [Display(Name = "Appr. By L4")]
        public string ApprovedByL4 { get; set; }
        [Display(Name = "Appr. At L4")]
        public string ApprovedDateL4 { get; set; }
        [Display(Name = "L4 Comments")]
        public string CommentsL4 { get; set; }
        [Display(Name = "Audited")]
        public bool IsAudited { get; set; }
        [Display(Name = "Audited By")]
        public string AuditedBy { get; set; }
        [Display(Name = "Audited At")]
        public string AuditedDate { get; set; }
        [Display(Name = "Audite Comments")]
        public string AuditedComments { get; set; }
        [Display(Name = "Rejected")]
        public bool IsRejected { get; set; }
        [Display(Name = "Rejected By")]
        public string RejectedBy { get; set; }
        [Display(Name = "Rejecte At")]
        public string RejectedDate { get; set; }
        [Display(Name = "Rejecte Comments")]
        public string RejectedComments { get; set; }
         #endregion

        public string Name { get; set; }

        public string ByName { get; set; }
        public string NameDate { get; set; }
        public string NameComments { get; set; }

        public List<GLPettyCashRequisitionFormAVM> glPettyCashRequisitionFormAVMs { get; set; }
        public List<GLPettyCashRequisitionFormAVM> glPettyCashRequisitionFormBankChargeVMs { get; set; }
        public List<GLPettyCashRequisitionFormAVM> glPettyCashRequisitionFormExpectedFundVMs { get; set; }
        //public List<GLPettyCashRequisitionFormBankChargeVM> glPettyCashRequisitionFormBankChargeVMs { get; set; }
        //public List<GLPettyCashRequisitionFormExpectedFundVM> glPettyCashRequisitionFormExpectedFundVMs { get; set; }


        public List<GLPettyCashRequisitionFormBVM> glPettyCashRequisitionFormBVMs { get; set; }
        public List<GLPettyCashRequisitionFileVM> glPettyCashRequisitionFileVMs { get; set; }

        public string Operation { get; set; }

        [Display(Name = "Status")]
        public string MyStatus { get; set; }

        public string Status { get; set; }

        [Display(Name = "Branch")]
        public string BranchName { get; set; }

        public string TransactionType { get; set; }
        [Display(Name = "Date From")]
        public string TransactionDateTimeFrom { get; set; }

        [Display(Name = "Date To")]
        public string TransactionDateTimeTo { get; set; }

        [Display(Name = "Files")]
        public string FilesToBeUploaded { get; set; }

        public bool MadeJournal { get; set; }
    }
}
