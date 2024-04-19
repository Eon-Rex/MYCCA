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
using CCAF.Helper;
using System.Runtime.InteropServices;

namespace CCAF.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult MyProfile()
        {
            DataTable dt = CCAF.DataAccessLayer.DataAccessLayer.GetMyProfile(Session["USERCODE"].ToString());
            dt.AcceptChanges();
            return View("_MyProfile", dt);
        }

        #region ManageCatalog

        public ActionResult ManageCatalog()
        {
            return View("_ManageCatalog");
        }

 
        #endregion

        #region ManageNotification

        [HttpGet]
        public ActionResult ManageNotification()
        {
            return View("_ManageNotification");
        }

        public JsonResult BindUserTyp()
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@ADMINCODE"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            DataTable dtusertyp = new DataTable();
            dtusertyp = CCAF.AppCode.Global.GetData_New("USP_ACXGetCustomerType", CommandType.StoredProcedure, _plist, _vlist);

            string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
            return Json(DataResult, JsonRequestBehavior.AllowGet);
        }
        public string SaveNotification(string Image, string TextMsg, string Usertype, string Detail)
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            try
            {
                List<string> _plist = new List<string>();
                List<string> _vlist = new List<string>();
                _plist.Add("@USERTYP"); _vlist.Add(Usertype);
                _plist.Add("@IMG"); _vlist.Add(Image);
                _plist.Add("@TXT"); _vlist.Add(TextMsg);
                _plist.Add("@DETAIL"); _vlist.Add(Detail);
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                _plist.Add("@ADMINCODE"); _vlist.Add(Session["USERCODE"].ToString());
                CCAF.AppCode.Global.GetData_New("USP_ACXInsertNotification", CommandType.StoredProcedure, _plist, _vlist);
                return null;
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
        public string ViewRecord(string Usertype)
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            JavaScriptSerializer a = new JavaScriptSerializer();
            DataSet ds = new DataSet();
            SqlConnection conn = new SqlConnection(CCAF.AppCode.Global.GetConnectionString());
            SqlCommand sqlComm = new SqlCommand("USP_ACXShowNotification",conn);
            sqlComm.Parameters.AddWithValue("@USERTYP", Usertype);
            sqlComm.Parameters.AddWithValue("@DATAAREAID", Session["DATAAREAID"].ToString());
            sqlComm.Parameters.AddWithValue("@ADMINCODE", Session["USERCODE"].ToString());
            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = sqlComm;
            da.Fill(ds);
            DataTable _dtObj = ds.Tables[0];
            DataTable _dtObj1 = ds.Tables[1];
            DataTable _dtObj2 = ds.Tables[2];
            DataTable _dtObj3 = ds.Tables[3];
            DataTable _dtObj4 = ds.Tables[4];
            DataTable _dtObj5 = ds.Tables[5];
            DataTable _dtObj6 = ds.Tables[6];
            string Data1 = obj.ConvertDataTableTojSonString(_dtObj);
            string Data2 = obj.ConvertDataTableTojSonString(_dtObj1);
            string Data3 = obj.ConvertDataTableTojSonString(_dtObj2);
            string Data4 = obj.ConvertDataTableTojSonString(_dtObj3);
            string Data5 = obj.ConvertDataTableTojSonString(_dtObj4);
            string Data6 = obj.ConvertDataTableTojSonString(_dtObj5);
            string Data7 = obj.ConvertDataTableTojSonString(_dtObj6);
            return a.Serialize(new { Txt = Data1, Img1 = Data2, Img2 = Data3, Img3 = Data4, Img4 = Data5, Img5 = Data6, Img6 = Data7 });
        }

        #endregion

        #region UserManagement

        [HttpGet]
        public ActionResult UserManagement()
        {
            return View("UserManagement");
        }

        public ActionResult GetUserManagement()
        {
            DataTable dt = new DataTable();
            CCAF.AppCode.Global obj = new AppCode.Global();
            try
            {
                List<string> _plist = new List<string>();
                List<string> _vlist = new List<string>();
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                _plist.Add("@ADMINCODE"); _vlist.Add(Session["USERCODE"].ToString());
                dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETUSERMANAGEMENT", CommandType.StoredProcedure, _plist, _vlist);
                dt.AcceptChanges();

            }
            catch
            {

            }
            string DataResult = obj.ConvertDataTableTojSonString(dt);

            #region  Web Application Security Assessment [V001] - Sensitive Data Exposure
            //*
            //Auther : Abhishek Dheeman
            //Date :  09-Jun-2022
            //Ref : Web Application Security Assessment [V001] - Sensitive Data Exposure
            //
            //
            //*//
            string CryptoDataResult = new ClsCrypto("coke").Encrypt(DataResult);
            #endregion

            return Json(CryptoDataResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlockUser(string id, string code, string userid)
        {
            if (Request.IsAjaxRequest())
            {
                List<string> _plist = new List<string>();
                List<string> _vlist = new List<string>();
                _plist.Add("@USERCODE"); _vlist.Add(userid);
                _plist.Add("@TYPE"); _vlist.Add(id == "BLOCK" ? "1" : "2");
                DataTable dt = CCAF.AppCode.Global.GetData_New("USP_ACXUPDATEUSERSTATUS", CommandType.StoredProcedure, _plist, _vlist);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Content("SUCCESS");
                }
                return Content("FAILURE");
            }
            if (id == "NA")
            {
                TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('Not Applicable');});</script>";
            }
            else
            {
                List<string> _plist = new List<string>();
                List<string> _vlist = new List<string>();
                _plist.Add("@USERCODE"); _vlist.Add(userid);
                _plist.Add("@TYPE"); _vlist.Add(id == "BLOCK" ? "1" : "2");
                DataTable dt = CCAF.AppCode.Global.GetData_New("USP_ACXUPDATEUSERSTATUS", CommandType.StoredProcedure, _plist, _vlist);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string _message = userid + " User Has been ";
                    _message += id == "BLOCK" ? "Blocked" : "UnBlocked.";
                    TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('" + _message + "');});</script>";
                }
            }
            return RedirectToAction("UserManagement", "Home");
        }
        [HttpGet]
        public ActionResult Create(string code, string userid, string UserName,
            string address, string contact, string Email, string Usertype, string RoleCode, string AuthorizationMode)
        {
            if (code == "EXISTING")
            {
                TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('The User Already Exists.');});</script>";
            }
            else if (code == "CREATE")
            {
                try
                {
                    string _uniquepassword = CCAF.BussinessLayer.BussinessLayer.GetUniqueKey(6);
                    List<string> _plist = new List<string>();
                    List<string> _vlist = new List<string>();
                    _plist.Add("@USERCODE"); _vlist.Add(userid);
                    _plist.Add("@USERNAME"); _vlist.Add(UserName);
                    _plist.Add("@USERTYPE"); _vlist.Add(Usertype);
                    _plist.Add("@EMAILID"); _vlist.Add(string.IsNullOrEmpty(Email) ? "" : Email);
                    _plist.Add("@ROLECODE"); _vlist.Add(RoleCode);
                    _plist.Add("@CONTACTNO"); _vlist.Add(string.IsNullOrEmpty(contact) ? "" : contact);
                    _plist.Add("@AUTHORIZATIONMODE"); _vlist.Add(AuthorizationMode);
                    _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                    _plist.Add("@ADMINCODE"); _vlist.Add(Session["USERCODE"].ToString());
                    _plist.Add("@PASSWORD"); _vlist.Add(_uniquepassword);
                    _plist.Add("@PASSWORDENC"); _vlist.Add(BussinessLayer.CustomSecurityLayer.HashPassword(_uniquepassword));
                    CCAF.AppCode.Global.GetData_New("USP_ACXINSUSERMASTER", CommandType.StoredProcedure, _plist, _vlist);
                    string _message = "User Code:" + userid + " User has been Created.";
                    return Content("Success");
                    //TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('" + _message + "');});</script>";
                }
                catch (Exception ex)
                {
                    return Content(ex.Message.ToString());
                    //TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('" + ex.Message.ToString() + "');});</script>";
                }
            }
            return Content("EXISTING USER");
            //return RedirectToAction("UserManagement", "Home");
        }

        public ActionResult ChangeAuthMode(string code, string userid, string AuthorizationMode)
        {
            if (code == "CREATE")
            {
                return Content("Failure");
            }
            else if (code == "EXISTING")
            {
                try
                {
                    string _uniquepassword = CCAF.BussinessLayer.BussinessLayer.GetUniqueKey(6);
                    string _message = string.Empty;
                    List<string> _plist = new List<string>();
                    List<string> _vlist = new List<string>();
                    _plist.Add("@ADMINCODE"); _vlist.Add(Session["USERCODE"].ToString());
                    _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                    _plist.Add("@AUTHORIZATIONMODE"); _vlist.Add(AuthorizationMode);
                    _plist.Add("@CUSTOMERID"); _vlist.Add(userid);
                    _plist.Add("@PASSWORD"); _vlist.Add(_uniquepassword);
                    _plist.Add("@PASSWORDENC"); _vlist.Add(CCAF.BussinessLayer.CustomSecurityLayer.HashPassword(_uniquepassword));
                    DataTable dt = CCAF.AppCode.Global.GetData_New("USP_ACXCHANGEUSERAUTHENTICATIONMODE", CommandType.StoredProcedure, _plist, _vlist);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "Success")
                        {
                            return Content("Success");
                        }
                    }
                    return Content("Failure");
                }
                catch (Exception ex)
                {
                    TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('" + ex.Message.ToString() + "');});</script>";
                    return Content(ex.Message.ToString());
                }
            }
            return Content("Failure");
        }

        //[HttpGet]
        public ActionResult BlockUserTest()
        {
            ManageUserViewModel mjn = new ManageUserViewModel();
            TryUpdateModel(mjn);
            if (Request.IsAjaxRequest())
            {
                TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('Not Applicable');});</script>";
                return RedirectToAction("UserManagement", "Home");
                //return Content("Successfull");
            }
            TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('Not Applicable');});</script>";
            return RedirectToAction("UserManagement", "Home");
        }

        #endregion

        #region Order Hierarchy

        [HttpGet]
        public ActionResult OrderHierarchy()
        {
            DataTable dt = new DataTable();
            return View("OrderHierarchy", dt);
        }

        public JsonResult BindCustomerGroup()
        {
            try
            {
                CCAF.AppCode.Global obj = new AppCode.Global();
                List<string> _plist = new List<string>();
                List<string> _vlist = new List<string>();
                _plist.Add("@ADMINCODE"); _vlist.Add(Session["USERCODE"].ToString());
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                DataTable dtcusttyp = new DataTable();
                dtcusttyp = CCAF.AppCode.Global.GetData_New("USP_ACXGetCustomerType", CommandType.StoredProcedure, _plist, _vlist);

                string DataResult = obj.ConvertDataTableTojSonString(dtcusttyp);
                return Json(DataResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ViewOrderHierarchy(string CustGroup)
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@TYPE"); _vlist.Add(CustGroup);
            _plist.Add("@ADMINCODE"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            DataTable dtusertyp = new DataTable();
            dtusertyp = CCAF.AppCode.Global.GetData_New("USP_VIEW_ORDERHIERARCHY", CommandType.StoredProcedure, _plist, _vlist);

            string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
            var jsonResult = Json(DataResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            //return Json(DataResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult OrderHierarchy(HttpPostedFileBase FileUpload, string dropdown)
        {

            DataTable temp = new DataTable("NEWTABLE");
            try
            {
                var supportedTypes = new[] { "xlsx" };
                var fileExt = System.IO.Path.GetExtension(FileUpload.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {

                    TempData["msg"] = "<script type='text/javascript' >$(document).ready(function() { ShowMessage('File Extension Is InValid - Only Upload EXCEL File');});</script>";
                }
                else if (FileUpload.ContentLength > (500000))
                {

                    TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() {ShowMessage('File size Should Be UpTo 500 KB');});</script>";

                }
                else
                {
                    if (Session["USERCODE"] == null)
                    {
                        return RedirectToAction("Login", "Authentication");
                    }
                    else
                    {
                        if (FileUpload != null)
                        {
                            string _msg = string.Empty;
                            try
                            {
                                string fileName = System.IO.Path.GetFileName(FileUpload.FileName);
                                FileUpload.SaveAs(Server.MapPath("~/Upload/" + fileName));
                                string excelPath = Server.MapPath("~/Upload/") + Path.GetFileName(FileUpload.FileName);
                                string conString = string.Empty;
                                string extension = Path.GetExtension(FileUpload.FileName);
                                DataTable dtExcelData = new DataTable();
                                dtExcelData = CCAF.App_Code.ExcelUpload.ImportExcelXLS(Server.MapPath("~/Upload/" + fileName), true);
                                dtExcelData.TableName = "OrderHierarchy";
                                CCAF.AppCode.Global obj = new AppCode.Global();
                                DataSet lds = new DataSet();
                                lds.Tables.Add(dtExcelData);
                                string ls_xml = lds.GetXml();
                                List<string> _plist = new List<string>();
                                List<string> _vlist = new List<string>();
                                _plist.Add("@XMLDATA"); _vlist.Add(ls_xml);
                                _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
                                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                                _plist.Add("@CUSTOMERGROUP"); _vlist.Add(dropdown);
                                temp = CCAF.AppCode.Global.GetData_New("USP_INS_XMLORDERHIERARCHY", CommandType.StoredProcedure, _plist, _vlist);
                            }
                            catch (Exception ex)
                            {
                                _msg = ex.Message.ToString();
                            }
                            //ViewBag.div = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('" + _msg + "');});</script>";
                            return View("OrderHierarchy", temp);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "" + ex.Message.ToString() + " ";
            }
            return RedirectToAction("OrderHierarchy", "Home");
        }

        //public ActionResult UploadExcel(HttpPostedFileBase FileUpload, string dropdown)
        //{
        //    try
        //    {
        //        if (FileUpload != null)
        //        {
        //            if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        //            {

        //                DataTable temp = new DataTable("NEWTABLE");
        //                string filename = FileUpload.FileName + Session["USERCODE"].ToString() + DateTime.Now.ToString("yyyyMMdd_hhmmss");

        //                string targetpath = Server.MapPath("~/Upload/");
        //                FileUpload.SaveAs(targetpath + filename);
        //                string pathToExcelFile = targetpath + filename;
        //                var connectionString = "";
        //                if (filename.EndsWith(".xls"))
        //                {
        //                    connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
        //                }
        //                if (filename.EndsWith(".xlsx"))
        //                {
        //                    connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
        //                }

        //                var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
        //                var lds = new DataSet();
        //                adapter.Fill(lds, "OrderHierarchy");

        //                DataTable dtable = lds.Tables["OrderHierarchy"];
        //                string ls_xml = lds.GetXml();

        //                CCAF.AppCode.Global obj = new AppCode.Global();
        //                List<string> _plist = new List<string>();
        //                List<string> _vlist = new List<string>();
        //                _plist.Add("@XMLDATA"); _vlist.Add(ls_xml);
        //                _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
        //                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
        //                _plist.Add("@CUSTOMERGROUP"); _vlist.Add(dropdown);

        //                temp = CCAF.AppCode.Global.GetData_New("USP_INS_XMLORDERHIERARCHY", CommandType.StoredProcedure, _plist, _vlist);

        //                using (XLWorkbook wb = new XLWorkbook())
        //                {
        //                    temp.TableName = "OrderHierarchy";
        //                    wb.Worksheets.Add(temp);
        //                    wb.Protect();           //Protect addition of extra sheet in downloaded excel

        //                    using (MemoryStream stream = new MemoryStream())
        //                    {
        //                        wb.SaveAs(stream);
        //                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OrderHierarchy.xlsx");
        //                    }
        //                }

        //            }
        //        }

        //    }
        //    catch (Exception Ex)
        //    {
        //        TempData["msg"] = Json(Ex.Message.ToString(), JsonRequestBehavior.AllowGet);
        //        return View("OrderHierarchy");
        //    }
        //    //TempData["msg"] = "<script>alert('Excel Inserted succesfully!');</script>";

        //    return RedirectToAction("OrderHierarchy", "Home");
        //}

        #endregion

        #region ManageHierarchy

        [HttpGet]
        public ActionResult ManageHierarchy()
        {
            DataTable dt = new DataTable();
            return View("_ManageHierarchy", dt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ManageHierarchy(HttpPostedFileBase FileUpload, string dropdown)
        {
            try
            {
                var supportedTypes = new[] { "xlsx" };
                var fileExt = System.IO.Path.GetExtension(FileUpload.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('File Extension Is InValid - Only Upload EXCEL File');});</script>";
                }
                else if (FileUpload.ContentLength > (500000))
                {

                    TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() {ShowMessage('File size Should Be UpTo 500 KB');});</script>";

                }
                else
                {
                    if (Session["USERCODE"] == null)
                    {
                        return RedirectToAction("Login", "Authentication");
                    }
                    else
                    {
                        if (FileUpload != null)
                        {
                            string _msg = string.Empty;
                            DataTable temp = new DataTable("NEWTABLE");
                            try
                            {
                                string fileName = System.IO.Path.GetFileName(FileUpload.FileName);
                                FileUpload.SaveAs(Server.MapPath("~/Upload/" + fileName));
                                string excelPath = Server.MapPath("~/Upload/") + Path.GetFileName(FileUpload.FileName);
                                string conString = string.Empty;
                                string extension = Path.GetExtension(FileUpload.FileName);
                                DataTable dtExcelData = new DataTable();
                                dtExcelData = CCAF.App_Code.ExcelUpload.ImportExcelXLS(Server.MapPath("~/Upload/" + fileName), true);
                                dtExcelData.TableName = "MANAGEHIERARCHY";
                                CCAF.AppCode.Global obj = new AppCode.Global();
                                DataSet lds = new DataSet();
                                lds.Tables.Add(dtExcelData);
                                string ls_xml = lds.GetXml();
                                List<string> _plist = new List<string>();
                                List<string> _vlist = new List<string>();
                                _plist.Add("@XMLDATA"); _vlist.Add(ls_xml);
                                _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
                                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                                _plist.Add("@CUSTOMERGROUP"); _vlist.Add("");
                                temp = CCAF.AppCode.Global.GetData_New("USP_INS_XMLMANAGEHIERARCHY", CommandType.StoredProcedure, _plist, _vlist);
                            }
                            catch (Exception ex)
                            {
                                _msg = ex.Message.ToString();
                                ViewBag.div = "ShowMessage('" + _msg + "');";
                            }
                            return View("_ManageHierarchy", temp);

                        }

                    }
                }

            }
            catch (Exception ex)
            {

                TempData["msg"] = "ShowMessage('" + ex.Message.ToString() + "');";
                return RedirectToAction("ManageHierarchy", "Home");
            }
            return RedirectToAction("ManageHierarchy", "Home");
        }

        public JsonResult ViewManageHierarchy(string CustGroup)
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@TYPE"); _vlist.Add(CustGroup);
            _plist.Add("@ADMINCODE"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());

            DataTable dtusertyp = new DataTable();
            dtusertyp = CCAF.AppCode.Global.GetData_New("USP_VIEW_MANAGEHIERARCHY", CommandType.StoredProcedure, _plist, _vlist);

            string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
            var jsonResult = Json(DataResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            //return Json(DataResult, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult UploadExcelMH(HttpPostedFileBase FileUpload, string dropdown)
        //{
        //    try
        //    {

        //        if (FileUpload != null)
        //        {

        //            if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        //            {

        //                DataTable temp = new DataTable("NewTable");
        //                string filename = FileUpload.FileName;
        //                string targetpath = Server.MapPath("~/Upload/");
        //                FileUpload.SaveAs(targetpath + filename);
        //                string pathToExcelFile = targetpath + filename;
        //                var connectionString = "";
        //                if (filename.EndsWith(".xls"))
        //                {
        //                    connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
        //                }
        //                if (filename.EndsWith(".xlsx"))
        //                {
        //                    connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
        //                }

        //                var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
        //                var lds = new DataSet();
        //                adapter.Fill(lds, "MANAGEHIERARCHY");

        //                DataTable dtable = lds.Tables["MANAGEHIERARCHY"];
        //                string ls_xml = lds.GetXml();

        //                CCAF.AppCode.Global obj = new AppCode.Global();
        //                List<string> _plist = new List<string>();
        //                List<string> _vlist = new List<string>();
        //                _plist.Add("@XMLDATA"); _vlist.Add(ls_xml);
        //                _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
        //                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
        //                _plist.Add("@CUSTOMERGROUP"); _vlist.Add(dropdown);

        //                temp = CCAF.AppCode.Global.GetData_New("USP_INS_XMLMANAGEHIERARCHY", CommandType.StoredProcedure, _plist, _vlist);
        //                using (XLWorkbook wb = new XLWorkbook())
        //                {
        //                    temp.TableName = "ManageHierarchy";
        //                    wb.Worksheets.Add(temp);
        //                    wb.Protect();           //Protect addition of extra sheet in downloaded excel

        //                    using (MemoryStream stream = new MemoryStream())
        //                    {
        //                        wb.SaveAs(stream);
        //                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ManageHierarchy.xlsx");
        //                    }
        //                }
        //            }
        //        }
        //        // TempData["msg"] = "<script>alert('Excel Inserted succesfully!');</script>";
        //        // return View("_ManageHierarchy");
        //    }
        //    catch (Exception Ex)
        //    {
        //        TempData["msg"] = Json(Ex.Message.ToString(), JsonRequestBehavior.AllowGet);
        //        return View("_ManageHierarchy");
        //    }
        //    return RedirectToAction("ManageHierarchy", "Home");
        //}

        #endregion

        #region ApproveOrder

        [HttpGet]
        public ActionResult ApproveOrder()
        {
            DataTable dt = CCAF.DataAccessLayer.DataAccessLayer.GetOrderHistory(Session["USERCODE"].ToString(), Session["DATAAREAID"].ToString());
            dt.AcceptChanges();
            OrderHistoryViewModel ohvm = new OrderHistoryViewModel();
            ohvm._LineData = dt;
            return View("_ApproveOrder", ohvm);
        }
        public async Task<ActionResult> UpdateOrderStatus(string statusc, string statuss, string Webordern, string AccountNum, string DeliveryDate)
        {
            string _message = string.Empty;
            CCAF.AppCode.Global obj = new AppCode.Global();
            try
            {
                List<string> _plist = new List<string>();
                List<string> _vlist = new List<string>();
                _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
                _plist.Add("@CUSTOMERID"); _vlist.Add(AccountNum);
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                _plist.Add("@WEBORDERNO"); _vlist.Add(Webordern);
                _plist.Add("@STATUSCHECK"); _vlist.Add(statusc);
                _plist.Add("@STATUSSET"); _vlist.Add(statuss);
                _plist.Add("@DELIVERYDATE"); _vlist.Add(DeliveryDate);
                DataTable dtusertyp = new DataTable();
                try
                {
                    dtusertyp = CCAF.AppCode.Global.GetData_New("USP_ACXUPDATEORDERSTATUS", CommandType.StoredProcedure, _plist, _vlist);
                }
                catch(Exception ex)
                {
                    return Content(ex.Message.ToString());
                }
                string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);

                BussinessLayer.Email mailobject = new BussinessLayer.Email();

                try
                {
                    if (dtusertyp.Rows[0]["EMAILSTATUS"].ToString() != "0")
                    {
                        await mailobject.SendApproveOrderEmail(Webordern, AccountNum, dtusertyp.Rows[0]["EMAILSTATUS"].ToString());
                    }
                }
                catch (Exception exm)
                {
                    _message += "Mail Error: " + exm.Message.ToString();
                }
                try
                {
                    string _SyncStatus = AppCode.Global.SendSaleOrder(Webordern);
                    if (_SyncStatus.ToString().ToUpper() == "SUCCESS")
                    {
                        _plist.Clear(); _vlist.Clear(); DataTable _dt = new DataTable();
                        _plist.Add("@ORDERNO"); _vlist.Add(Webordern);
                        _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                        _dt = CCAF.AppCode.Global.GetData_New("USP_UPORDERSTATUS", CommandType.StoredProcedure, _plist, _vlist);
                        _message += " Sync Successfully. " + Webordern + " : " + DateTime.Now.ToString();
                    }
                    else
                    {
                        _message += "AX Sync Error: " + _SyncStatus.ToString();
                    }
                }
                catch (Exception ex)
                {
                    _message += ex.Message.ToString();
                }
                CCAF.AppCode.Global.WriteLog(_message, "\\LogFiles\\ErrorLog\\" + DateTime.Now.ToString("ddMMyyyy"));

                return Json(DataResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult PopupDataApproveOrder(string OrdNo, string Usercode)
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@CUSTOMERID"); _vlist.Add(Usercode);
            _plist.Add("@ORDERNO"); _vlist.Add(OrdNo);
            DataTable dtusertyp = new DataTable();
            dtusertyp = CCAF.AppCode.Global.GetData_New("USP_GETORDERLINEDETAILS", CommandType.StoredProcedure, _plist, _vlist);

            string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
            return Json(DataResult, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Dashboard

        [HttpGet]
        public ActionResult Dashboard()
        {
            return View("_Dashboard");
        }

        public JsonResult DashboardTab()
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _plist.Add("@ACCOUNTNUM"); _vlist.Add(Session["USERCODE"].ToString());
            DataTable dtusertyp = new DataTable();
            dtusertyp = CCAF.AppCode.Global.GetData_New("USP_DASHBOARD", CommandType.StoredProcedure, _plist, _vlist);

            string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
            return Json(DataResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DashboardCrBal()
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@CUSTOMERID"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            decimal CreditBal = CCAF.AppCode.Global.GetAxCustomerCreditBalance((Session["USERCODE"].ToString()));
            _plist.Add("@CRBALANCE"); _vlist.Add(Convert.ToString(CreditBal));
            DataTable dtusertyp = new DataTable();
            dtusertyp = CCAF.AppCode.Global.GetData_New("USP_ACXGETCUSTOMERDETAILSBYID", CommandType.StoredProcedure, _plist, _vlist);//USP_ACXGETCREDITBALANCEBYCUSTID

            string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
            return Json(DataResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DashboardTextNotification()
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();

            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());

            DataTable dtusertyp = new DataTable();
            dtusertyp = CCAF.AppCode.Global.GetData_New("USP_ACXDashboardTextNotification", CommandType.StoredProcedure, _plist, _vlist);

            string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
            return Json(DataResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DashboardImages()
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@CUSTOMERID"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());

            DataTable dtusertyp = new DataTable();
            dtusertyp = CCAF.AppCode.Global.GetData_New("", CommandType.StoredProcedure, _plist, _vlist);

            string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
            return Json(DataResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetDataToDasboard(string FunctionName, string objData, [Optional] int specific)
        {
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();

            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@USERTYP"); _vlist.Add(Session["WOCUSTOMERTYPE"].ToString());

            DataTable _dtResult = new DataTable();
            try
            {
                _dtResult = CCAF.AppCode.Global.GetData_New("USP_ACXDashboardImgNotification", CommandType.StoredProcedure, _plist, _vlist);
            }
            catch (Exception ex)
            {
                // return Content(ex.Message.ToString());
                //------------AYUSH 8-11-2022---------//
                //return Json("Error!" + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
                return Json("Error!", JsonRequestBehavior.AllowGet);
            }

            string json = string.Empty;
            if (_dtResult != null && _dtResult.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(_dtResult, Formatting.Indented);
            }
            return Content(json);
        }

        #endregion

        #region OrderHistory

        [HttpGet]
        public ActionResult OrderHistory()
        {
            DataTable dt = CCAF.DataAccessLayer.DataAccessLayer.GetOrderHistory(Session["USERCODE"].ToString(), Session["DATAAREAID"].ToString());
            dt.AcceptChanges();
            OrderHistoryViewModel ohvm = new OrderHistoryViewModel();
            ohvm._LineData = dt;
            return View("_OrderHistory", ohvm);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult OrderHistory(OrderHistoryViewModel ohvm)
        {
            TryUpdateModel(ohvm);
            DateTime _fromdate = Convert.ToDateTime(ohvm._DateParm.FromDate);
            DateTime _todate = Convert.ToDateTime(ohvm._DateParm.ToDate);

            DataTable dt = CCAF.DataAccessLayer.DataAccessLayer.GetOrderHistory(usercode: Session["USERCODE"].ToString(),
                dataareaid: Session["DATAAREAID"].ToString(), FromDate: _fromdate.ToShortDateString(), ToDate: _todate.ToShortDateString());
            dt.AcceptChanges();

            ohvm._LineData = dt;
            return View("_OrderHistory", ohvm);
        }
        public JsonResult PopupData(string OrdNo)
        {
            CCAF.AppCode.Global obj = new AppCode.Global();
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@CUSTOMERID"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@ORDERNO"); _vlist.Add(OrdNo);
            DataTable dtusertyp = new DataTable();
            dtusertyp = CCAF.AppCode.Global.GetData_New("USP_GETORDERLINEDETAILS", CommandType.StoredProcedure, _plist, _vlist);

            string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
            return Json(DataResult, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region EmailConfiguration

        [HttpGet]
        public ActionResult EmailConfiguration()
        {
            EmailConfigurationVM ECViewModel = new EmailConfigurationVM();
            ECViewModel.DESCRIPTION = "Order Confirmation";
            ECViewModel.ACTIVITYTYPE = "1";

            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@ACTIVITYTYPE"); _vlist.Add("1");
            DataTable dt = new DataTable();
            dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETEMAILCONFIGURATION", CommandType.StoredProcedure, _plist, _vlist);
            
            if(dt != null && dt.Rows.Count > 0)
            {
                ECViewModel.EMAILCC = dt.Rows[0]["EMAILCC"] == null ? "" : dt.Rows[0]["EMAILCC"].ToString();
                ECViewModel.EMAILTO = dt.Rows[0]["EMAILTO"] == null ? "" : dt.Rows[0]["EMAILTO"].ToString();
                ECViewModel.EMAILSUBJECT = dt.Rows[0]["EMAILSUBJECT"] == null ? "" : dt.Rows[0]["EMAILSUBJECT"].ToString();
                ECViewModel.EMAILTITLE = dt.Rows[0]["EMAILTITLE"] == null ? "" : dt.Rows[0]["EMAILTITLE"].ToString();
            }

            return View("EmailConfiguration", ECViewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EmailConfiguration(EmailConfigurationVM ECViewModel)
        {
            //EmailConfigurationVM ECViewModel = new EmailConfigurationVM();
            ECViewModel.DESCRIPTION = "Order Confirmation";
            ECViewModel.ACTIVITYTYPE = "1";

            string _message = string.Empty;
            List <string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@ACTIVITYTYPE"); _vlist.Add("1");
            _plist.Add("@DESCRIPTION"); _vlist.Add(ECViewModel.DESCRIPTION);
            _plist.Add("@EMAILTO"); _vlist.Add(ECViewModel.EMAILTO == null ? "" : ECViewModel.EMAILTO);
            _plist.Add("@EMAILCC"); _vlist.Add(ECViewModel.EMAILCC == null ? "" : ECViewModel.EMAILCC);
            _plist.Add("@EMAILSUBJECT"); _vlist.Add(ECViewModel.EMAILSUBJECT == null ? "" : ECViewModel.EMAILSUBJECT);
            _plist.Add("@EMAILTITLE"); _vlist.Add(ECViewModel.EMAILTITLE == null ? "" : ECViewModel.EMAILTITLE);
            DataTable dt = new DataTable();
            try
            {
                dt = CCAF.AppCode.Global.GetData_New("USP_ACXSETEMAILCONFIGURATION", CommandType.StoredProcedure, _plist, _vlist);
                _message = "Email Configuration Updated Successfully!";
            }
            catch (Exception ex)
            {
                _message = ex.Message.ToString();
            }

            ECViewModel.EMAILCC = dt.Rows[0]["EMAILCC"] == null ? "" : dt.Rows[0]["EMAILCC"].ToString();
            ECViewModel.EMAILTO = dt.Rows[0]["EMAILTO"] == null ? "" : dt.Rows[0]["EMAILTO"].ToString();
            ECViewModel.EMAILSUBJECT = dt.Rows[0]["EMAILSUBJECT"] == null ? "" : dt.Rows[0]["EMAILSUBJECT"].ToString();
            ECViewModel.EMAILTITLE = dt.Rows[0]["EMAILTITLE"] == null ? "" : dt.Rows[0]["EMAILTITLE"].ToString();

            ViewBag.div = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('" + _message + "');});</script>";
            return View("EmailConfiguration", ECViewModel);
        }

        #endregion

        //[HttpGet]
        //[Authorize]
        //public ActionResult GetData(string FunctionName, string objData)
        //{
        //    List<string> _plist = new List<string>();
        //    List<string> _vlist = new List<string>();
        //    DataTable dtParmList = new DataTable();
        //    try
        //    {
        //        dtParmList = (DataTable)JsonConvert.DeserializeObject("[" + objData + "]", (typeof(DataTable)));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content("Wrong Parameter Construction for objData, " + ex.Message.ToString());
        //    }
        //    try
        //    {
        //        if (dtParmList.Rows.Count == 1)
        //        {
        //            DataRow dr = dtParmList.Rows[0];
        //            foreach (DataColumn dc in dtParmList.Columns)
        //            {
        //                _plist.Add("@" + dc.ColumnName.ToString().ToUpper());
        //                _vlist.Add(dtParmList.Rows[0][dc.ColumnName].ToString());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content("Blank Property Name Not Allowed, " + ex.Message.ToString());
        //    }

        //    DataTable _dtResult = new DataTable();
        //    try
        //    {
        //        _dtResult = CCAF.AppCode.Global.GetData_New("USP_ACX" + FunctionName, CommandType.StoredProcedure, _plist, _vlist);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(ex.Message.ToString());
        //    }

        //    string json = string.Empty;
        //    if (_dtResult != null && _dtResult.Rows.Count > 0)
        //    {
        //        json = JsonConvert.SerializeObject(_dtResult, Formatting.Indented);
        //    }
        //    return Content(json);
        //}

        //public ActionResult GenerateOrder()
        //{
        //    return View("_GenerateOrder");
        //}
        //public ActionResult MyCart()
        //{
        //    return View("_MyCart");
        //}
        //public ActionResult Catalog()
        //{
        //    return View("_Catalog");
        //}
    }
}