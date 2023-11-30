using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SymViewModel.Sage
{
    public class NewTransactionDetailViewModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int NewTransactionId { get; set; }
        public int AccountId { get; set; }
        public decimal VATAmount { get; set; }
        public decimal TransactionAmount { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; } 

    }
}
