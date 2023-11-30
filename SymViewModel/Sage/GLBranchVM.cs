using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLBranchVM
    {
        public int Id { get; set; }
        [StringLength(3, ErrorMessage = "Code cannot be longer than 3 characters.")]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        [Display(Name = "Zip Code")]public string ZipCode { get; set; }
        [Display(Name = "Telephone No.")]public string TelephoneNo { get; set; }
        [Display(Name = "Mobile No.")]public string MobileNo { get; set; }
        [Display(Name = "Fax No.")]public string FaxNo { get; set; }
        public string Email { get; set; }
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        [Display(Name = "CP Designation")]
        public int ContactPersonDesignation { get; set; }
        [Display(Name = "CP Telephone")]
        public string ContactPersonTelephone { get; set; }
        [Display(Name = "CP Email")]
        public string ContactPersonEmail { get; set; }

        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
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


        public string Operation { get; set; }

        [Display(Name = "Head Office")]
        public bool IsHO { get; set; }

        [Display(Name = "Debit Account")]
        public int DebitAccountId { get; set; }
        [Display(Name = "Credit Account")]
        public int CreditAccountId { get; set; }


        
        
    }
}
