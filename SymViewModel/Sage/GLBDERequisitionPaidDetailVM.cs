using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Sage
{
    public class GLBDERequisitionPaidDetailVM
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int GLBDERequisitionPaidId { get; set; }
        public int GLBDERequisitionId { get; set; }
        public int GLBDERequisitionDetailId { get; set; }

        public string TransactionType { get; set; }
    }
}
