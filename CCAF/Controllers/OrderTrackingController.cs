using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using CCAF.Models;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Data.OleDb;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using CCAF.App_Code;
using CCAF.AXWO;

namespace CCAF.Controllers
{
    public class OrderTrackingController : Controller
    {
        // GET: OrderTracking
        public ActionResult GetOrderTrackingdOrder(string objData)
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetOrderTrackingdData(string Fromdate, string Todate)
        {
            DataTable dtParmList = new DataTable();
            string msg = "";
            try
            {
                if (string.IsNullOrWhiteSpace(Fromdate))
                    msg = "From date is missing";
                if (string.IsNullOrWhiteSpace(Todate))
                    msg += " To date is missing";
            }
            catch (Exception ex)
            {
                return Json("Wrong Parameter Construction for objData, " + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrWhiteSpace(msg))
            {
                return Json(msg, JsonRequestBehavior.AllowGet);
            }

            bool blFetchData = true;
            List<OrderTrackingDetails> _tracking = new List<OrderTrackingDetails>();
            OrderTrackingDetails _TmpDataList = new OrderTrackingDetails();

            CCAF.AppCode.Global obj = new AppCode.Global();
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            //_plist.Add("@ACCOUNTNUM"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@FROMDATE"); _vlist.Add(Fromdate);
            _plist.Add("@TODATE"); _vlist.Add(Todate);

            DataTable dtusertyp = new DataTable();
            dtusertyp = CCAF.AppCode.Global.GetData_New("USP_GETORDERTRACKING", CommandType.StoredProcedure, _plist, _vlist);

            _tracking = CCAF.BussinessLayer.BussinessLayer.ConvertToList<OrderTrackingDetails>(dtusertyp);
            if (_tracking.Count > 0 && _tracking != null)
            {
                List<Reassign> _reassign = new List<Reassign>();
                string AccountNum = string.Empty;
                string WebOrderNo = string.Empty;

                for (int i = 0; i < _tracking.Count; i++)
                {
                    blFetchData = true;
                    //_TmpDataList = _tracking.Where(f => (f.AccountNum == _tracking[i].AccountNum.ToString()) && (f => f.reassignCount > 0)).FirstOrDefault();
                    _TmpDataList = _tracking.Where(f => f.AccountNum == _tracking[i].AccountNum.ToString()).ToList().Where(x => x.reassignCount > 0).FirstOrDefault();
                    //_TmpDataList = _tracking.Where(f => f.AccountNum == _tracking[i].AccountNum.ToString() && f.reassignCount > 0).FirstOrDefault();
                    if (_TmpDataList != null)
                    {
                        blFetchData = false;
                        _tracking[i]._reassignlist = _TmpDataList._reassignlist;
                    }

                    if (blFetchData == true)
                    {
                        AccountNum = _tracking[i].AccountNum.ToString();
                        WebOrderNo = _tracking[i].WebOrderNo.ToString();
                        List<string> _plistassign = new List<string>();
                        List<string> _vlistassign = new List<string>();
                        _plistassign.Add("@DATAAREAID"); _vlistassign.Add(Session["DATAAREAID"].ToString());
                        _plistassign.Add("@CUSTOMERID"); _vlistassign.Add(AccountNum);
                        //_plistassign.Add("@WebOrderNo"); _vlistassign.Add(WebOrderNo);

                        DataTable dtreassign = new DataTable();
                        dtreassign = CCAF.AppCode.Global.GetData_New("USP_GETUSERASSIGNLIST", CommandType.StoredProcedure, _plistassign, _vlistassign);
                        _reassign = CCAF.BussinessLayer.BussinessLayer.ConvertToList<Reassign>(dtreassign);
                        _tracking[i]._reassignlist = _reassign;
                    }

                    /*
                    if (AccountNum != _tracking[i].AccountNum.ToString())
                    {
                      AccountNum = _tracking[i].AccountNum.ToString();
                      List<string> _plistassign = new List<string>();
                      List<string> _vlistassign = new List<string>();
                      _plistassign.Add("@DATAAREAID"); _vlistassign.Add(Session["DATAAREAID"].ToString());
                      _plistassign.Add("@CUSTOMERID"); _vlistassign.Add(AccountNum);
                      DataTable dtreassign = new DataTable();
                      dtreassign = CCAF.AppCode.Global.GetData_New("USP_GETUSERASSIGNLIST", CommandType.StoredProcedure, _plistassign, _vlistassign);
                      _reassign = CCAF.BussinessLayer.BussinessLayer.ConvertToList<Reassign>(dtreassign);
                      _tracking[i]._reassignlist = _reassign;
                    }
                    else
                    {
                      _tracking[i]._reassignlist = _reassign;
                    }
                    */
                }

            }
            ViewData["Orderlist"] = _tracking;

            string DataResult = JsonConvert.SerializeObject(_tracking);
            //string DataResult = obj.c //obj.ConvertDataTableTojSonString(dtusertyp);
            return Json(DataResult, JsonRequestBehavior.AllowGet);

            //return View();
        }


        public JsonResult OrderTrackingdData()
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _plist.Add("@ACCOUNTNUM"); _vlist.Add(Session["USERCODE"].ToString());
            DataTable dtusertyp = new DataTable();
            try
            {

                dtusertyp = CCAF.AppCode.Global.GetData_New("USP_GETORDERTRACKING", CommandType.StoredProcedure, _plist, _vlist);

                string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
                return Json(DataResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error!" + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
            string json = string.Empty;
            if (dtusertyp != null && dtusertyp.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(dtusertyp, Formatting.Indented);
            }
            if (string.IsNullOrEmpty(json))
                return Json("No Data Found !", JsonRequestBehavior.AllowGet);
        }

        public JsonResult PendingSyncOrder()
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            string SyncStatus = CCAF.AppCode.Global.PendingSyncOrderStatus();
            return Json(SyncStatus, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ReassignUser(string weborderid, string reassignuser, string AccountNum)
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@ORDERNO"); _vlist.Add(weborderid);
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _plist.Add("@CUSTOMERID"); _vlist.Add(AccountNum);//_vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@REASSIGNID"); _vlist.Add(reassignuser);
            DataTable _dt = new DataTable();
            _dt = CCAF.AppCode.Global.GetData_New("USP_UPREASSIGNUSER", CommandType.StoredProcedure, _plist, _vlist);

            if (_dt != null && _dt.Rows.Count > 0)
            {
                if (_dt.Rows[0]["STATUSRESULT"].ToString() == "SUCCESS")
                {
                    return Json("Reassigned user updated successfully", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Failed to update reassigned user", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Failed to update reassigned user", JsonRequestBehavior.AllowGet);
            }



        }
    }
}