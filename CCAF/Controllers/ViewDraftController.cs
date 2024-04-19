using CCAF.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CCAF.Controllers
{
    public class ViewDraftController : Controller
    {
        // GET: ViewDraft
        public ActionResult GetDataFormDraftedOrder()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetData()
        {
            DataTable _dt = new DataTable();
            string json = string.Empty;
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();



            _plist.Add("@CUSTOMERID"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _dt = CCAF.AppCode.Global.GetData_New("USP_GETDARFTEDORDERDETAILS", CommandType.StoredProcedure, _plist, _vlist);

            if (_dt != null && _dt.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(_dt, Formatting.Indented);
            }
            return Content(json);
        }
    }
}