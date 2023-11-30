using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLFiscalYearDetailVM
    {
        public int Id { get; set; }
        public int GLFiscalYearId { get; set; }
        [Display(Name = "Period")]
        public string PeriodName { get; set; }
        [Display(Name = "Start")]
        public string PeriodStart { get; set; }
        [Display(Name = "End")]
        public string PeriodEnd { get; set; }
        [Display(Name = "Lock")]
        public bool PeriodLock { get; set; }
        public string Remarks { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

        public string PeriodSl { get; set; }
        [Display(Name = "Year")]
        public int Year { get; set; }

    }
}
