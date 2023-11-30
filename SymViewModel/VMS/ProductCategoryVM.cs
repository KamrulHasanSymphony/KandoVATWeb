using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class ProductCategoryVM
    {

        public string CategoryID { get; set; }
        //[Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        //[Display(Name = "Product Type")]
        public string IsRaw { get; set; }
        //[Display(Name = "HS Code")]
        public string HSCodeNo { get; set; }
        //[Display(Name = "Vat Rate")]
        public decimal VATRate { get; set; }
        //[Display(Name = "Propergating Rate")]
        public string PropergatingRate { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public decimal SD { get; set; }
        public string Trading { get; set; }
        //[Display(Name = "Non-stock")]
        public string NonStock { get; set; }
        //[Display(Name = "Information 4")]
        public string Info4 { get; set; }
        //[Display(Name = "Information 5")]
        public string Info5 { get; set; }
        public string Operation { get; set; }
         
    }
}
