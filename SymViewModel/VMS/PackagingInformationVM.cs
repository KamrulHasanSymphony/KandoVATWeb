using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class PackagingInformationVM
    {
        public string PackagingID { get; set; }
        public string PackagingNature { get; set; }
        public string PackagingCapacity { get; set; }
        public string UOM { get; set; }
        public string Description { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }

    }
}
