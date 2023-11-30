using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLUserVM
    {
        public int Id { get; set; }
        public string LogId { get; set; }
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        
        public string Designation { get; set; }

        public string Email { get; set; }
        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Auditor")]
        public bool IsAuditor { get; set; }
        [Display(Name = "Expense")]
        public bool IsExpense { get; set; }
        [Display(Name = "Expense Requisition")]
        public bool IsExpenseRequisition { get; set; }
        [Display(Name = "BDE Requisition")]
        public bool IsBDERequisition { get; set; }
        [Display(Name = "Expense Approve")]
        public bool HaveExpenseApproval { get; set; }
        [Display(Name = "Expense Req. Approve")]
        public bool HaveExpenseRequisitionApproval { get; set; }
        [Display(Name = "BDE Req. Approve")]
        public bool HaveBDERequisitionApproval { get; set; }
        [Display(Name = "App. Level1")]
        public bool HaveApprovalLevel1 { get; set; }
        [Display(Name = "App. Level2")]
        public bool HaveApprovalLevel2 { get; set; }
        [Display(Name = "App. Level3")]
        public bool HaveApprovalLevel3 { get; set; }
        [Display(Name = "App. Level4")]
        public bool HaveApprovalLevel4 { get; set; }

        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }


        public string Operation { get; set; }

        public List<GLUserBranchVM> glUserBranchVMs { get; set; }


        public string OldPassword { get; set; }
        public int LoginBranchId { get; set; }

        //[Display(Name = "First Name")]public string FirstName { get; set; }
        //[Display(Name = "Middle Name")]public string MiddleName { get; set; }
        //[Display(Name = "Last Name")]public string LastName { get; set; }



        [Display(Name = "PF No")]public string PFNo { get; set; }
        [Display(Name = "Mobile")]public string Mobile { get; set; }
        [Display(Name = "Photo")]public string PhotoFileName { get; set; }
        [Display(Name = "Signature")]public string SignatureFileName { get; set; }


        public string ReturnUrl { get; set; }
        public int BranchId { get; set; }

    }
}
