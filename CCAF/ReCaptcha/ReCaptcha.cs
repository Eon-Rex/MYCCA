using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ReCaptcha
{
    public static class SiteSettings
    {
        public static string GoogleRecaptchaSecretKey = ConfigurationManager.AppSettings["GoogleRecaptchaSecretKey"].ToString();
        public static string GoogleRecaptchaSiteKey = ConfigurationManager.AppSettings["GoogleRecaptchaSiteKey"].ToString();
    }
}