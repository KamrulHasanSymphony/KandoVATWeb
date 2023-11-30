using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLCeilingVM
    {

        public int Id { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Year")]
        public int GLFiscalYearId { get; set; }
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
        public bool Post { get; set; }

        public List<GLCeilingDetailVM> glCeilingDetailVMs { get; set; }





        [Display(Name = "Year")]
        public string FiscalYear { get; set; }

        public string YearStart { get; set; }

        public string YearEnd { get; set; }

        public string BranchName { get; set; }

        public decimal TotalCeiling { get; set; }

        public string Operation { get; set; }
    }
}
