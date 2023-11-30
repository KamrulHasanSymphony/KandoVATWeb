using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class DemandMasterVM
    {
        public string DemandNo { get; set; }
        public string DemandDateTime { get; set; }
        public string FiscalYear { get; set; }
        public string MonthFrom { get; set; }
        public string MonthTo { get; set; }
        public decimal TotalQty { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string TransactionType { get; set; }
        public string Post { get; set; }
        public string DemandReceiveID { get; set; }
        public string VehicleNo { get; set; }
        public bool VehicleSaveInDB { get; set; }
        public string VehicleType { get; set; }
        public string DriverName { get; set; }
        public string ReceiveDate { get; set; }
        public string RefDate { get; set; }
        public string RefNo { get; set; }
        






        public bool vehicleSaveInDB { get; set; }
    }

    public class DemandDetailVM
    {
        public string DemandNo { get; set; }
        public string DemandLineNo { get; set; }
        public string BandProductId { get; set; }
        public string ItemNo { get; set; }
        public decimal Quantity { get; set; }
        public string UOM { get; set; }
        public decimal DemandQty { get; set; }
        public decimal NBRPrice { get; set; }
        public string Comments { get; set; }
        public string Post { get; set; }
        public string TransactionDate { get; set; }
        
    }
}
