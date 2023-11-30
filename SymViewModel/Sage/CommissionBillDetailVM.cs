using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class CommissionBillDetailVM
    {

        public string CommissionBillNo { get; set; }
        public int SLNo { get; set; }
        public string DeptCode { get; set; }
        public string Class { get; set; }
        public string DocumentNo { get; set; }
        public string MRNo { get; set; }
        public string BranchCode { get; set; }
        public string CustomerID { get; set; }
        public string InsuredName { get; set; }
        public decimal NetPremium { get; set; }
        public decimal RateOfCommission { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetCommission { get; set; }


        public string ComDate { get; set; }


        //CommissionBillNo 
        //SLNo 
        //DeptCode 
        //Class 
        //DocumentNo 
        //MRNo 
        //BranchCode 
        //CustomerID 
        //InsuredName 
        //NetPremium
        //RateOfCommission 
        //CommissionAmount 
        //TaxRate 
        //TaxAmount 
        //NetCommission 


    }
}
