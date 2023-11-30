using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLBDERequisitionFormBVM
    {

        public int Id { get; set; }
        public int GLBDERequisitionId { get; set; }
        [Display(Name = "Transaction Date")]
        public string TransactionDateTime { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public bool Post { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }


        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        public bool IsRejected { get; set; }
        public string RejectedBy { get; set; }
        public string RejectedDate { get; set; }
        [Display(Name = "Disagree Comments")]
        public string RejectedComments { get; set; }
        [Display(Name = "Date From")]
        public string DateFrom { get; set; }

        [Display(Name = "Date To")]
        public string DateTo { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }

        [Display(Name = "Paid")]public bool IsPaid { get; set; }
        [Display(Name = "Paid Amount")]public decimal PaidAmount { get; set; }
        [Display(Name = "Paid To")]public string PaidTo { get; set; }
        [Display(Name = "Payment Date")]public string PaymentDate { get; set; }
    }
}
