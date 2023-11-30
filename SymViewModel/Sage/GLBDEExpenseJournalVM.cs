using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLBDEExpenseJournalVM
    {

        public int Id { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Branch")]
        public string BranchName { get; set; }
        public string Code { get; set; }
        [Display(Name = "Transaction Date")]
        public string TransactionDateTime { get; set; }
        public bool Post { get; set; }

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

        public string PostedBy { get; set; }
        public string PostedAt { get; set; }
        public string PostedFrom { get; set; }

        public string TransactionType { get; set; }

        [Display(Name = "Date From")]
        public string TransactionDateTimeFrom { get; set; }

        [Display(Name = "Date To")]
        public string TransactionDateTimeTo { get; set; }

        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal Dr { get; set; }
        public decimal Cr { get; set; }

        public int HeadId { get; set; }

        public string HeadName { get; set; }
        public List<GLBDEExpenseJournalVM> glBDEExpenseJournalVMs { get; set; }
    }
}
