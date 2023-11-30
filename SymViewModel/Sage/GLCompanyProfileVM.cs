using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLCompanyProfileVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Legal Name")]
        public string LegalName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        [Display(Name = "Telephone No")]
        public string TelephoneNo { get; set; }
        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }
        [Display(Name = "Fax No")]
        public string FaxNo { get; set; }
        public string Email { get; set; }
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        [Display(Name = "Contact Person Designation")]
        public string ContactPersonDesignation { get; set; }
        [Display(Name = "Contact Person Telephone")]
        public string ContactPersonTelephone { get; set; }
        [Display(Name = "Contact Person Email")]
        public string ContactPersonEmail { get; set; }
        public string TIN { get; set; }
        public string BIN { get; set; }

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
        public string VATNo { get; set; }

        public string Operation { get; set; }
        [Display(Name = "Year")]
        public string Year { get; set; }
        [Display(Name = "Year Start")]
        public string YearStart { get; set; }

    }
}
