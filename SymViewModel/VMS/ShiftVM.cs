using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.VMS
{
    public class ShiftVM
    {

        public int Id { get; set; }
        public string ShiftName { get; set; }
        public string ShiftStart { get; set; }
        public string ShiftEnd { get; set; }
        public string Remarks { get; set; }
        public int Sl { get; set; }
        public string NextDay { get; set; }


        public string Name { get; set; }
    }
}
