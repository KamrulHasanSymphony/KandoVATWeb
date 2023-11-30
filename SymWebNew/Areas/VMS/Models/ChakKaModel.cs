namespace SymVATWebUI.Areas.VMS.Models
{
    public class ChakKaModel
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int Branch { get; set; }

        public int TransferBranch { get; set; }

        public bool PreviewOnly { get; set; }

        public string PrinterName { get; set; }


        public int FontSize { get; set; }
    }
}