using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class ProductTypeViewModel
    {

        public string TypeID { get; set; }
        [Display(Name = "Product Type")]
        public string ProductType { get; set; }
        public string Comments { get; set; }
        public string Description { get; set; }
        public bool ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        [Display(Name = "Information 2")]
        public string Info2 { get; set; }
        [Display(Name = "Information 3")]
        public string Info3 { get; set; }
        [Display(Name = "Information 4")]
        public string Info4 { get; set; }
        [Display(Name = "Information 5")]
        public string Info5 { get; set; }
        [Display(Name = "Type ID1")]
        public string TypeID1 { get; set; }
        [Display(Name = "Type ID12")]
        public string TypeID12 { get; set; }
        [Display(Name = "Type ID123")]
        public string TypeID123 { get; set; }
        [Display(Name = "BOM Id")]
        public string BOMId { get; set; }
        public string Operation { get; set; }
    }
}
