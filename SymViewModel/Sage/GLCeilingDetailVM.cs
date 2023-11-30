using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLCeilingDetailVM
    {

        public int Id { get; set; }
        public int GLCeilingId { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Account")]
        public int AccountId { get; set; }
        [Display(Name = "Month")]
        public int GLFiscalYearDetailId { get; set; }
        public string PeriodStart { get; set; }
        public string PeriodEnd { get; set; }
        public decimal Amount { get; set; }
        public bool Post { get; set; }

        public decimal AmountP1 { get; set; }
        public decimal AmountP2 { get; set; }
        public decimal AmountP3 { get; set; }
        public decimal AmountP4 { get; set; }
        public decimal AmountP5 { get; set; }
        public decimal AmountP6 { get; set; }
        public decimal AmountP7 { get; set; }
        public decimal AmountP8 { get; set; }
        public decimal AmountP9 { get; set; }
        public decimal AmountP10 { get; set; }
        public decimal AmountP11 { get; set; }
        public decimal AmountP12 { get; set; }

        public string AccountName { get; set; }

        public string PeriodSl { get; set; }

        [Display(Name = "Year")]
        public int GLFiscalYearId { get; set; }
        public string Remarks { get; set; }


        public string Operation { get; set; }

        public decimal LineTotal { get; set; }

        public string AccountCode { get; set; }
    }
}
