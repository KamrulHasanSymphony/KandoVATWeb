using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLBDERequisitionPaidVM
    {

        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Code { get; set; }
        public string TransactionDateTime { get; set; }
        public string ReferenceNo1 { get; set; }
        public string ReferenceNo2 { get; set; }
        public string ReferenceNo3 { get; set; }
        public bool Post { get; set; }
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



        public string Operation { get; set; }


        public string Status { get; set; }
        [Display(Name = "Status")]
        public string MyStatus { get; set; }

        [Display(Name = "Branch")]
        public string BranchName { get; set; }


        [Display(Name = "Date From")]
        public string TransactionDateTimeFrom { get; set; }

        [Display(Name = "Date To")]
        public string TransactionDateTimeTo { get; set; }

        public int GLFiscalYearId { get; set; }




        public string TransactionType { get; set; }

    }
}
