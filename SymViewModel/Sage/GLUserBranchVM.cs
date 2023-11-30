using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLUserBranchVM
    {
        public int Id { get; set; }
        public int GLUserId { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string BranchAddress { get; set; }
        public bool IsBranchChecked { get; set; }
        public string Name { get; set; }

    }
}
