using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLPettyCashRequisitionFormBVM
    {
        public int Id { get; set; }
        public int GLPettyCashRequisitionId { get; set; }
        [Display(Name = "Date")]
        public string TransactionDateTime { get; set; }
        [Display(Name = "Commission Bill No.")]
        public string CommissionBillNo { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "MR No")]
        public string MRNo { get; set; }
        [Display(Name = "MR Date")]
        public string MRDate { get; set; }
        [Display(Name = "Document No")]
        public string DocumentNo { get; set; }
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        [Display(Name = "Net Premium")]
        public decimal NetPremium { get; set; }
       
        [Display(Name = "Com. Rate")]public decimal CommissionRate { get; set; }
        [Display(Name = "Com. Amount")]public decimal CommissionAmount { get; set; }
        [Display(Name = "AIT Rate")]public decimal AITRate { get; set; }
        [Display(Name = "AIT Amount")]public decimal AITAmount { get; set; }


        [Display(Name = "PC Amount")]
        public decimal PCAmount { get; set; }
        public bool Post { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        public bool IsRejected { get; set; }
        public string RejectedBy { get; set; }
        public string RejectedDate { get; set; }
        [Display(Name = "Disagree Comments")]
        public string RejectedComments { get; set; }


        [Display(Name = "Customer")]
        public string CustomerName { get; set; }
        [Display(Name = "Document Type")]
        public string DocumentType { get; set; }
        [Display(Name = "Date From")]
        public string DateFrom { get; set; }

        [Display(Name = "Date To")]
        public string DateTo { get; set; }
        public string Code { get; set; }

        public int MasterId { get; set; }
        [Display(Name = "Date From")]
        public string MRDateFrom { get; set; }

        [Display(Name = "Date To")]
        public string MRDateTo { get; set; }
        public string Status { get; set; }

         [Display(Name = "PC (%)")]
        public decimal PC { get; set; }


        public string CustomerCode { get; set; }
        public string BranchCode { get; set; }


        [Display(Name = "Make By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Make At")]
        public string CreatedAt { get; set; }
        [Display(Name = "Maker From")]
        public string CreatedFrom { get; set; }
        
    }
}
