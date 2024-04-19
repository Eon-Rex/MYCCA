using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace CCAF.Models
{
    public class OrderViewModel
    {
        public OHeader Header { get; set; }

        public List<OLine> Line { get; set; }

        public OAddLine AddLine { get; set; }
    }

    public class OHeader
    {
        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Delivery Type")]
        public string DeliveryType { get; set; }

        [Display(Name = "Order Type")]
        public string OrderType { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [Display(Name = "Reason of Order")]
        public string ReasonofOrder { get; set; }

        [Display(Name = "Delivery Date")]
        public string DeliveryDate { get; set; }

        [Display(Name = "Contact")]
        public string Contact { get; set; }

        [Display(Name = "Credit Balance")]
        public string CreditBalance { get; set; }

    }

    public class OLine
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Required]
        public int SrNo { get; set; }

        //[Required]
        public string ProductCode { get; set; }

        //[Required]
        public string ProductName { get; set; }

        //[Required]
        public decimal Qty { get; set; }

        //[Required]
        public string UOM { get; set; }

        //[Required]
        public decimal UnitPriceVEP { get; set; }

        public decimal UnitPriceVIP { get; set; }

        //[Required]
        public string PriceGP { get; set; }

        //[Required]
        public decimal TaxAmount { get; set; }

        //[Required]
        public decimal ExtendedPriceVIP { get; set; }

        public decimal AdjustedPrice { get; set; }

        public decimal Discount { get; set; }

        public decimal VAT { get; set; }

        //[Required]
        public decimal Amount { get; set; }
    }

    public class OAddLine
    {
        //[Required]
        public string ProductCode { get; set; }

        //[Required]
        public string ProductName { get; set; }

        //[Required]
        public int Qty { get; set; }

        //[Required]
        public string UOM { get; set; }
    }

    public class DateParm
    {
        [Display(Name = "From Date")]
        [Required]
        public string FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required]
        public string ToDate { get; set; }
    }

    public class OrderHistoryViewModel
    {
        public DateParm _DateParm { get; set; }
        public DataTable _LineData { get; set; }
    }

    public class MyCartViewModel
    {
        public HeaderMyCart _CartHeader { get; set; }
        public List<OLine> _CartLine { get; set; }
    }

    public class HeaderMyCart
    {
        [Display(Name = "Order No")]
        public string OrderNo { get; set; }

        [Display(Name = "Order Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Order Type")]
        public string OrderType { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [Display(Name = "Reason of Order")]
        public string ReasonofOrder { get; set; }

        [Display(Name = "Delivery Type")]
        public string DeliveryType { get; set; }

        [Display(Name = "Delivery Date")]
        public string DeliveryDate { get; set; }

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
    }
}


