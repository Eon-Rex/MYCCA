using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCAF.Models
{
    public class OrderTrackingDetails
    {
        public string OrderDate{get;set;}
        public string WebOrderNo { get; set; }
        public string  AccountNum { get; set; }
        public decimal ORDQTY { get; set; }
        public decimal CONFQTY { get; set; }
        public int OrderStatus { get; set; }

        public string SubmittedBY { get; set; }
        public string ASSIGNTO { get; set; }
      
        public string Submitted { get; set; }
        public string Pending { get; set; }

        public string Approved { get; set; }
        public string Cancelled { get; set; }

        public string Dispatched { get; set; }

        public string Invoiced { get; set; }

        public List<Reassign> _reassignlist { get; set; } = new List<Reassign>();

    public DateParm _DateParm { get; set; }

    public int reassignCount 
    {
      get { return _reassignlist.Count(); }
    }
    //public
  }

    public class Reassign
    {
        public string ApproverCode { get; set; }
        
    }


  //public class DateParm
  //{
  //  [Display(Name = "From Date")]
  //  [Required]
  //  public string FromDate { get; set; }

  //  [Display(Name = "To Date")]
  //  [Required]
  //  public string ToDate { get; set; }
  //}

}