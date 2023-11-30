using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class UOMConversionVM
    {
        public string UOMId { get; set; }
        public string UOMFrom { get; set; }
        public string UOMTo { get; set; }
        public decimal Convertion { get; set; }
        public string CTypes { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string ActiveStatus { get; set; }

        public string Operation { get; set; }

    }
}
