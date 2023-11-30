using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SymViewModel.VMS
{
    public class ImportVM
    {
        public string TableName { get; set; }

        public HttpPostedFileBase File { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
