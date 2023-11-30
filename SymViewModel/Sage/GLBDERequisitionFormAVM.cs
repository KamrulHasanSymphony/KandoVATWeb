using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLBDERequisitionFormAVM
    {
        public int Id { get; set; }
        public int GLBDERequisitionId { get; set; }
        public string TransactionDateTime { get; set; }
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
        [Display(Name = "BDE (%)")]
        public decimal BDE { get; set; }
        [Display(Name = "BDE Amount (Exc. AIT)")]
        public decimal BDEAmountWithoutAIT { get; set; }
        [Display(Name = "AIT (%)")]
        public decimal AIT { get; set; }
        [Display(Name = "AIT Amount")]
        public decimal AITAmount { get; set; }
        [Display(Name = "BDE Amount")]
        public decimal BDEAmount { get; set; }
        public bool Post { get; set; }
        public bool IsRejected { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }


        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        [Display(Name = "Rej. By")]
        public string RejectedBy { get; set; }

        [Display(Name = "Rej. Date")]
        public string RejectedDate { get; set; }

        [Display(Name = "Disagree Comments")]
        public string RejectedComments { get; set; }

        [Display(Name = "Document Type")]
        public string DocumentType { get; set; }
        [Display(Name = "Date From")]
        public string DateFrom { get; set; }

        [Display(Name = "Date To")]
        public string DateTo { get; set; }
        public string Code { get; set; }

        public int GLFiscalYearDetailId { get; set; }

        public decimal TransactionAmount { get; set; }

        public string PeriodName { get; set; }
        public string Status { get; set; }


        [Display(Name = "Paid")]public bool IsPaid { get; set; }
        [Display(Name = "Paid Amount")]public decimal PaidAmount { get; set; }
        [Display(Name = "Paid To")]public string PaidTo { get; set; }
        [Display(Name = "Payment Date")]public string PaymentDate { get; set; }


    }
}
