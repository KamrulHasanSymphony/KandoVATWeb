﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLAccountVM
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        [Display(Name = "Account Nature")]public string AccountNature { get; set; }
        [Display(Name = "Business Nature")]public string BusinessNature { get; set; }
        [Display(Name = "ACCT ID")]public string ACCTID { get; set; }
        [Display(Name = "ACCT FMTTD")]public string ACCTFMTTD { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        [Display(Name = "Is BDE")]
        public bool IsBDE { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

        [Display(Name = "Branch")]public string BranchName { get; set; }

        public string Operation { get; set; }

        [Display(Name = "Branch")]public string BranchCode { get; set; }

        [Display(Name = "Business Class")]public string BusinessClass { get; set; }

    }
}
