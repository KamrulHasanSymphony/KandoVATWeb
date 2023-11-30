using System.Collections.Generic;
using System.Linq;

namespace SymVATWebUI.Areas.VMS.Models
{
    public class ChartData
    {
        public ChartData()
        {
            labels = Enumerable.Range(1, 31).ToList();
            datasets = new List<ChartLine>();
        }

        public List<int> labels { get; set; }

        public List<ChartLine> datasets { get; set; }
    }


    public class ChartLine
    {
        public ChartLine()
        {
            fill = false;
            linetension = 0;
        }

        public string label { get; set; }

        public int linetension { get; set; }

        public bool fill { get; set; }

        public string borderColor { get; set; }

        public List<decimal> data { get; set; }
    }
}