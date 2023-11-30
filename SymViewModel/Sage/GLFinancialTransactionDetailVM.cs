using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SymViewModel.Sage
{
    public class GLFinancialTransactionDetailVM
    {
        public int Id { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public int GLFinancialTransactionId { get; set; }
        public int AccountId { get; set; }
        public bool IsDebit { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal TransactionAmount { get; set; }
        public string TransactionDateTime { get; set; }
        public string TransactionType { get; set; }
        public bool Post { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }


        public List<GLFinancialTransactionFileVM> glFinancialTransactionFileVMs { get; set; }


        public string AccountName { get; set; }

        public int MyProperty { get; set; }

        //IEnumerable<HttpPostedFileBase> myFile { get; set; }

        [Display(Name = "Date From")]
        public string DateFrom { get; set; }

        [Display(Name = "Date To")]
        public string DateTo { get; set; }
        public string Code { get; set; }


        [Display(Name = "Period Name")]
        public string PeriodName { get; set; }
        public int GLFiscalYearDetailId { get; set; }


        public string Year { get; set; }

        [Display(Name = "Branch")]
        public string BranchName { get; set; }

        public string AccountCode { get; set; }
        public string Status { get; set; }

        public decimal VATAmount { get; set; }
        public decimal TaxAmount { get; set; }


        public string CommissionBillNo { get; set; }
    }
}
