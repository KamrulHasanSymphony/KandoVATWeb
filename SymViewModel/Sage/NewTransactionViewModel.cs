using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class NewTransactionViewModel
    {
        public int Id{ get; set; }
        public int BranchId{ get; set; }
        public int AccountId{ get; set; }
        public string Code{ get; set; }
        public string TransactionDateTime{ get; set; }
        public decimal VATAmount{ get; set; }
        public decimal SubTotal { get; set; }
        public bool IsActive{ get; set; }
        public string Operation { get; set; }
        public string Name { get; set; }

        [Display(Name = "Account")]
        public string AccountName { get; set; }

        [Display(Name = "Branch")]
        public string BranchName { get; set; }
        [Display(Name = "Date From")]
        public string TransactionDateTimeFrom { get; set; }

        [Display(Name = "Date To")]
        public string TransactionDateTimeTo { get; set; }

        public List<NewTransactionDetailViewModel> NewTransactionDetailVMs { get; set; }
    }
}
