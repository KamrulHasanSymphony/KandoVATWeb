using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class VehicleVM
    {
        public string VehicleID { get; set; }
        //[Display(Name = "Code")]
        public string Code { get; set; }
        //[Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }
        //[Display(Name = "Vehicle No")]
        public string VehicleNo { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        //[Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Info1 { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public string Info4 { get; set; }
        public string Info5 { get; set; }
        public string DriverName { get; set; }
        public string Operation { get; set; }
        public string IsActive { get; set; }
    }
}
