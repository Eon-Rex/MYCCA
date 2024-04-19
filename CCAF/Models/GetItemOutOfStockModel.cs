using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCAF.Models
{
    public class GetItemOutOfStockModel
    {        
        public string ItemId { get; set; }
        public decimal Qty { get; set; }
    }
}