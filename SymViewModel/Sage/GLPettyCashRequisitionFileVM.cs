using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SymViewModel.Sage
{
    public class GLPettyCashRequisitionFileVM
    {
        public int Id { get; set; }
        public int GLPettyCashRequisitionId { get; set; }
        public string FileName { get; set; }
        public string FileOrginalName { get; set; }

        public bool Post { get; set; }

        public string Remarks { get; set; }
    }
}
