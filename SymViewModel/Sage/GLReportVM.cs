using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLReportVM
    {
        [Display(Name = "Branch")]
        public string BranchName { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [Display(Name = "Date From")]
        public string DateFrom { get; set; }

        [Display(Name = "Date To")]
        public string DateTo { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Year")]
        public int GLFiscalYearId { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeId { get; set; }

        [Display(Name = "Account")]
        public int AccountId { get; set; }

        public int GLFiscalYearDetailId { get; set; }
        public string GLDocumentType { get; set; }

        
    }
}
