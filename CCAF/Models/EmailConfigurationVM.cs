using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CCAF.Models
{

    public partial class EmailConfigurationVM
    {
        public string ACTIVITYTYPE { get; set; }
        [Display(Name = "Activity For :")]
        public string DESCRIPTION { get; set; }
        [Display(Name = "Email To :")]
        public string EMAILTO { get; set; }
        [Display(Name = "Email Cc :")]
        public string EMAILCC { get; set; }
        [Display(Name = "Email Subject :")]
        public string EMAILSUBJECT { get; set; }
        [Display(Name = "Email Title :")]
        public string EMAILTITLE { get; set; }
    }
}