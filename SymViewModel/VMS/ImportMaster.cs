using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class ProductVM
    {
        public string Id { get; set; }
        public string ItemNo { get; set; }
        //[Display(Name = "Name")]
        public string ProductName { get; set; }
        //[Display(Name = "Desciption")]
        public string ProductDescription { get; set; }
        public string CategoryName { get; set; }
        //[Display(Name = "Group")]
        public string CategoryID { get; set; }
        public string UOM { get; set; }
        //[Display(Name = "Cost Price")]
        public decimal CostPrice { get; set; }
        //[Display(Name = "Sales Price")]
        public decimal SalesPrice { get; set; }
        //[Display(Name = "NBR Price")]
        public decimal NBRPrice { get; set; }
        public decimal OpeningBalance { get; set; }
        //[Display(Name = "Serial No")]
        public string SerialNo { get; set; }
        //[Display(Name = "HS Code No")]
        public string HSCodeNo { get; set; }
        //[Display(Name = "Information")]
        public decimal VATRate { get; set; }
        public string Comments { get; set; }
        //[Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public decimal SD { get; set; }
        public decimal Packetprice { get; set; }
        public string Trading { get; set; }
        //[Display(Name = "Trading MarkUp")]
        public decimal TradingMarkUp { get; set; }
        public decimal Stock { get; set; }
        //[Display(Name = "Non-stock")]
        public string NonStock { get; set; }
        //[Display(Name = "Opening Date")]
        public string OpeningDate { get; set; }
        //[Display(Name = "Product Code")]
        public string ProductCode { get; set; }
        //[Display(Name = "Toll Charge")]
        public decimal TollCharge { get; set; }
        //[Display(Name = "Opening Total Cost")]
        public decimal OpeningTotalCost { get; set; }
        //[Display(Name = "Rebate Percent")]
        public decimal RebatePercent { get; set; }
        public string Banderol { get; set; }
        public string TollProduct { get; set; }
        //[Display(Name = "Product Type")]
        public string ProductType { get; set; }
        public string Type { get; set; }
        public string Operation { get; set; }
        public string TargetId { get; set; }
        public string isActive { get; set; }
        //[Display(Name = "VAT Rate 2")]
        public decimal VATRate2 { get; set; }
        public decimal VDSRate { get; set; }
        public decimal TVBRate { get; set; }
        public decimal CDRate { get; set; }
        public decimal RDRate { get; set; }
        public decimal AITRate { get; set; }
        public decimal TVARate { get; set; }
        public decimal ATVRate { get; set; }
        public decimal CnFRate { get; set; }
        public string[] retResult { get; set; }
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
    }

    

    public class BankVM
    {
        public string BankID { get; set; }
        public string BankCode { get; set; }
        
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string AccountNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactPersonTelephone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }

    }

    //public class VehicleVM
    //{

    //    public string Code { get; set; }
    //    public string VehicleID { get; set; }
    //    public string VehicleType { get; set; }
    //    public string VehicleNo { get; set; }
    //    public string Description { get; set; }
    //    public string Comments { get; set; }
    //    public string ActiveStatus { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string CreatedOn { get; set; }

    //}

    public class ProductTypeVM
    {
        public string TypeID { get; set; }
        public string ProductType { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string Description { get; set; }
    }

    public class CostingVM
    {
        public string ItemNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string InputDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal AV { get; set; }
        public decimal CD { get; set; }
        public decimal RD { get; set; }
        public decimal TVB { get; set; }
        public decimal SDAmount { get; set; }
        public decimal VATAmount { get; set; }
        public decimal TVA { get; set; }
        public decimal ATV { get; set; }
        public decimal Other { get; set; }
        public decimal CostPrice { get; set; }//TotalPrice
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string BENumber { get; set; }
        public string RefNo { get; set; }
    }

    public class UOMVM
    {
        public string UOMID { get; set; }
        public string UOMName { get; set; }
        public string UOMCode { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string Operation { get; set; }
    }


}

