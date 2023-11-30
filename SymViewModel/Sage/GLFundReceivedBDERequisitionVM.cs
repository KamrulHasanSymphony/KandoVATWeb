using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLFundReceivedBDERequisitionVM
    {

        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ReferenceId { get; set; }
        public decimal FundAmount { get; set; }
        public string FinalApprovedDate { get; set; }
        public bool IsReceived { get; set; }
        [Display(Name = "Received Date")]
        public string TransactionDateTime { get; set; }


        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
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

        [Display(Name = "Received By")]
        public string ReceivedBy { get; set; }
        [Display(Name = "Received At")]
        public string ReceivedAt { get; set; }
        [Display(Name = "Received From")]
        public string ReceivedFrom { get; set; }

        public string Operation { get; set; }

        public string ReferenceCode { get; set; }
        public string ReferenceDate { get; set; }
        public string TransactionDateTimeTo { get; set; }
        public string TransactionDateTimeFrom { get; set; }

        public string BranchName { get; set; }
    }
}









