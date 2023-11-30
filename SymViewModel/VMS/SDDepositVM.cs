using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class SDDepositVM
    {

        public string Id { get; set; }
        public string DepositId { get; set; }
        //[Display(Name = "Treasury No")]
        public string TreasuryNo { get; set; }
        //[Display(Name = "Deposit Date")]
        public string DepositDate { get; set; }
        //[Display(Name = "Deposit Type")]
        public string DepositType { get; set; }
        //[Display(Name = "Deposit Amount")]
        public decimal DepositAmount { get; set; }
        //[Display(Name = "Checque No")]
        public string ChequeNo { get; set; }
        //[Display(Name = "Cheque Bank")]
        public string ChequeBank { get; set; }
        public string ChequeBankBranch { get; set; }
        public string ChequeDate { get; set; }
        public string BankId { get; set; }
        public string TreasuryCopy { get; set; }
        public string DepositPerson { get; set; }
        public string DepositPersonDesignation { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string TransactionType { get; set; }
        public string Post { get; set; }
        public string ReturnID { get; set; }
        public string Operation { get; set; }
        //[Display(Name = "Deposit Date From")]
        public string IssueDateTimeFrom { get; set; }
        //[Display(Name = "Deposit Date To")]
        public string IssueDateTimeTo { get; set; }
        //[Display(Name = "Cheque Date From")]
        public string CheckDateFrom { get; set; }
        //[Display(Name = "Cheque Date To")]
        public string CheckDateTo { get; set; }
        //[Display(Name = "Bank Name")]
        public string BankName { get; set; }
        public string BranchName { get; set; }
        //[Display(Name = "Account No.")]
        public string AccountNumber { get; set; }

    }
        
 }


