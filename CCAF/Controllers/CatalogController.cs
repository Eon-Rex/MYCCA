using CCAF.AppCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CCAF.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;

namespace CCAF.Controllers
{
    public class CatalogController : Controller
    {
        CCAF.AppCode.Global obj = new AppCode.Global();
        public ActionResult Catalog()
        {
            return View("_Catalog");
        }

        public ActionResult Mycart()
        {
            return View("_MyCart");
        }

        public ActionResult NewCatalog()
        {
            return View("_NewCatalog");
        }

        #region Manage Catalog

        public ActionResult ManageCatalog()
        {
            return View("_ManageCatalog");
        }

        public JsonResult BindProductGrp()
        {
            try
            {
                CCAF.AppCode.Global obj = new AppCode.Global();
                List<string> _plist = new List<string>();
                List<string> _vlist = new List<string>();
                DataTable dtusertyp = new DataTable();
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                dtusertyp = CCAF.AppCode.Global.GetData_New("USP_ACXGETCUSTOMERGRP", CommandType.StoredProcedure, _plist, _vlist);

                string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
                return Json(DataResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult BindProduct(string PrdctGrp)
        {
            try
            {
                CCAF.AppCode.Global obj = new AppCode.Global();
                List<string> _plist = new List<string>();
                List<string> _vlist = new List<string>();
                _plist.Add("@GROUP"); _vlist.Add(PrdctGrp);
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                DataTable dtusertyp = new DataTable();
                dtusertyp = CCAF.AppCode.Global.GetData_New("USP_ACXGETPRODUCT", CommandType.StoredProcedure, _plist, _vlist);

                string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
                return Json(DataResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult UploadCatalog(string Image, string ProductGrp, string Itemid)
        {
            try
            {
                CCAF.AppCode.Global obj = new AppCode.Global();
                List<string> _plist = new List<string>();
                List<string> _vlist = new List<string>();
                _plist.Add("@IMG"); _vlist.Add(Image);
                _plist.Add("@PRODUCTGRP"); _vlist.Add(ProductGrp);
                _plist.Add("@ITEMID"); _vlist.Add(Itemid);
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                DataTable dtusertyp = new DataTable();
                dtusertyp = CCAF.AppCode.Global.GetData_New("USP_ACXUploadCatalog", CommandType.StoredProcedure, _plist, _vlist);
                return null;
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ViewCatalog(string ProductGrp, string Itemid)
        {
            try
            {
                CCAF.AppCode.Global obj = new AppCode.Global();
                List<string> _plist = new List<string>();
                List<string> _vlist = new List<string>();

                _plist.Add("@PRODUCTGRP"); _vlist.Add(ProductGrp);
                _plist.Add("@ITEMID"); _vlist.Add(Itemid);
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());

                DataTable dtusertyp = new DataTable();
                dtusertyp = CCAF.AppCode.Global.GetData_New("USP_ACXViewCatalog", CommandType.StoredProcedure, _plist, _vlist);
                string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
                return Json(DataResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        public JsonResult ViewAllCatalog()
        {
            try
            {
                CCAF.AppCode.Global obj = new AppCode.Global();
                List<string> _plist = new List<string>();
                List<string> _vlist = new List<string>();

                DataTable dtusertyp = new DataTable();
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                _plist.Add("@CUSTOMERID"); _vlist.Add(Session["USERCODE"].ToString());
                dtusertyp = CCAF.AppCode.Global.GetData_New("USP_ACXViewAllCatalog", CommandType.StoredProcedure, _plist, _vlist);
                string DataResult = obj.ConvertDataTableTojSonString(dtusertyp);
                return Json(DataResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult SAVEITEMCATALOGTOCART(string ProductID, string Qty)
        {
            string CustomerID = Session["USERCODE"].ToString();
            DataTable _dt = new DataTable();
            string json = string.Empty;
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            if (!string.IsNullOrWhiteSpace(ProductID) && !string.IsNullOrWhiteSpace(CustomerID) && !string.IsNullOrWhiteSpace(Qty))
            {
                _plist.Add("@ITEMID"); _vlist.Add(ProductID);
                _plist.Add("@CUSTOMERID"); _vlist.Add(CustomerID);
                _plist.Add("@QTY"); _vlist.Add(Qty);
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                if (Qty != "0")
                {
                    decimal[] PriceArrary = CCAF.AppCode.Global.GetAxPriceList((CustomerID), (ProductID), Convert.ToInt32(Qty), "");
                    _plist.Add("@PRICE"); _vlist.Add(Convert.ToString(PriceArrary[0]));
                    _plist.Add("@DISCOUNT"); _vlist.Add(Convert.ToString(PriceArrary[1]));
                    _plist.Add("@TAX"); _vlist.Add(Convert.ToString(PriceArrary[2]));
                }
                else
                {
                    _plist.Add("@PRICE"); _vlist.Add("0");
                    _plist.Add("@DISCOUNT"); _vlist.Add("0");
                    _plist.Add("@TAX"); _vlist.Add("0");
                }
                _plist.Add("@PRICEGROUP"); _vlist.Add("");
                try
                {
                    _dt = CCAF.AppCode.Global.GetData_New("USP_ACXSAVEITEMCATALOGTOCART", CommandType.StoredProcedure, _plist, _vlist);
                }
                catch (Exception ex)
                {
                    json = ex.Message.ToString();
                }

            }
            if (_dt != null && _dt.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(_dt, Newtonsoft.Json.Formatting.Indented);
            }
            return Content(json);
        }

        #region Catalog

        //[HttpGet]
        //public HttpResponseMessage SAVEITEMCATALOGTOINDENT(string itemid, string qty, string sitecode)
        //{
        //    try
        //    {
        //        // WildcraftAPI.AppCode.Global obj = new Global();
        //        Global obj = new Global();
        //        List<string> ilist = new List<string>();
        //        List<string> item = new List<string>();

        //        //  LISTOBJ.Add("@code");
        //        //  LISTVAL.Add(code);

        //        string query = "USP_SAVEITEMCATALOGTOINDENT";
        //        ilist.Add("@ITEMID"); item.Add(itemid);
        //        ilist.Add("@QTY"); item.Add(qty);
        //        ilist.Add("@SITECODE"); item.Add(sitecode);

        //        DataTable dt = obj.GetData_New(query, CommandType.StoredProcedure, ilist, item);
        //        if (dt != null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, dt.Rows[0][0].ToString());
        //        }
        //        else
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error !!!!!!!!!!");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message.ToString());
        //    }

        //}

        #endregion

        #region MyCart

        [HttpGet]
        public ActionResult MyCart(string Redirect)
        {
            string val = Redirect;
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            DataTable _dt = new DataTable();
            //For Developement Only, Please Remove before Making Live
            ViewBag.ReturnURL = Redirect == "NewCatalog" ? "NewCatalog" : "Catalog";

            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _plist.Add("@CUSTOMERID"); _vlist.Add(Session["USERCODE"].ToString());
            _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETDELIVERYMODEMASTER", CommandType.StoredProcedure, _plist, _vlist);
            ViewBag.DeliveryTypeDropdown = CCAF.BussinessLayer.BussinessLayer.ToSelectList(_dt, "CODE", "DESCRIPTION", Session["DILVMODE"].ToString());
            //_plist.Add("@CUSTOMERID"); _vlist.Add(Session["USERCODE"].ToString());


            List<OLine> lines = new List<OLine>();
            List<HeaderMyCart> header = new List<HeaderMyCart>();

            _dt.Clear();
            _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETMYCARTHEADER", CommandType.StoredProcedure, _plist, _vlist);
            if (_dt != null && _dt.Rows.Count > 0)
                header = CCAF.BussinessLayer.BussinessLayer.ConvertToList<HeaderMyCart>(_dt);

            _dt.Clear();
            _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETMYCARTLINE", CommandType.StoredProcedure, _plist, _vlist);
            if (_dt != null && _dt.Rows.Count > 0)
                lines = CCAF.BussinessLayer.BussinessLayer.ConvertToList<OLine>(_dt);

            _dt.Clear();
            _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETPRICEGROUP", CommandType.StoredProcedure, _plist, _vlist);  // 

            if (_dt != null && _dt.Rows.Count > 0)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (DataRow row in _dt.Rows)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = row["PriceGroupName"].ToString(),
                        Value = row["PriceGroup"].ToString()
                    });
                }
                ViewBag.PriceGroupDropDown = list;
            }
            ViewBag.PriceGroupVisible = Session["WOCUSTOMERTYPE"].ToString() == "0" ? "" : "none";

            _dt.Clear(); _plist.Clear(); _vlist.Clear();

            _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETORDERTYPEMASTER", CommandType.StoredProcedure, _plist, _vlist);
            ViewBag.OrderTypeDropdown = CCAF.BussinessLayer.BussinessLayer.ToSelectList(_dt, "CODE", "DESCRIPTION", Session["ORDERTYPE"].ToString());


            MyCartViewModel mycartvm = new MyCartViewModel();
            if (header != null && header.Count > 0)
                mycartvm._CartHeader = header[0];
            if (lines != null && lines.Count > 0)
                mycartvm._CartLine = lines;
            return View("_MyCart", mycartvm);
        }

        //Old Update Cart order Status Changed to Post MyCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> MyCart(MyCartViewModel mycartvm)
        {
            string STATUSCHECK = "0";
            string STATUSSET = "1";
            DataTable _dt = new DataTable();
            DataTable _dtParmHeader = new DataTable();
            List<HeaderMyCart> OrderHeaderParm = new List<HeaderMyCart>();
            OrderHeaderParm.Add(mycartvm._CartHeader);

            string json = string.Empty;
            List<string> _plist = new List<string>();
            List<object> _vlist = new List<object>();
            _dtParmHeader = CCAF.BussinessLayer.BussinessLayer.ToDataTable(OrderHeaderParm);
            string weborderno = _dtParmHeader.Rows[0]["ORDERNO"] == null ? "" : _dtParmHeader.Rows[0]["ORDERNO"].ToString();
            decimal CreditBal = CCAF.AppCode.Global.GetAxCustomerCreditBalance((Session["USERCODE"].ToString()));
            string _message = string.Empty;
            if (!string.IsNullOrWhiteSpace(weborderno) && weborderno != "")
            {
                _plist.Add("@AXCREDITBALANCE"); _vlist.Add(CreditBal);
                _plist.Add("@WEBORDERNO"); _vlist.Add(weborderno);
                _plist.Add("@STATUSCHECK"); _vlist.Add(STATUSCHECK);
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
                _plist.Add("@STATUSSET"); _vlist.Add(STATUSSET);
                _plist.Add("@CARTORDERDATAHEADER"); _vlist.Add(_dtParmHeader);
                try
                {
                    _dt = CCAF.AppCode.Global.GetData_New("USP_ACXUPDATECARTORDERSTATUS", CommandType.StoredProcedure, _plist, _vlist);
                }
                catch (Exception ex)
                {
                    _message += "Message : " + ex.Message.ToString();
                }
            }
            if (_dt != null && _dt.Rows.Count > 0)
            {
                _message += _dt.Rows[0]["DISPLAYMESSAGE"].ToString();
                json = JsonConvert.SerializeObject(_dt, Newtonsoft.Json.Formatting.Indented);
                if (_dt.Rows[0]["status"].ToString().ToUpper() == "success".ToUpper())
                {
                    _message = "Order Number : " + _dt.Rows[0]["orderno"].ToString() + " Saved Successfully!";
                    if (_dt.Rows[0]["EMAILSTATUS"].ToString() == "2")
                    {
                        ////Sending Order to AX
                        try
                        {
                            string _SyncStatus = AppCode.Global.SendSaleOrder(_dt.Rows[0]["orderno"].ToString());
                            if (_SyncStatus.ToString().ToUpper() == "SUCCESS")
                            {
                                _plist.Clear(); _vlist.Clear();
                                _plist.Add("@ORDERNO"); _vlist.Add(_dt.Rows[0]["orderno"].ToString());
                                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                                CCAF.AppCode.Global.GetData_New("USP_UPORDERSTATUS", CommandType.StoredProcedure, _plist, _vlist);
                                _message += " Sync Successfully.";
                            }
                            else
                            {
                                _message += _SyncStatus.ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            _message += "Sync: " + ex.Message.ToString();
                        }
                    }

                    try
                    {
                        ////Sending Email
                        BussinessLayer.Email mailobj = new BussinessLayer.Email();
                        if (_dt.Rows[0]["EMAILSTATUS"].ToString() == "2")
                        {
                            await mailobj.SendApproveOrderEmail(_dt.Rows[0]["orderno"].ToString(), Session["USERCODE"].ToString(), _dt.Rows[0]["EMAILSTATUS"].ToString());
                        }
                        else if (_dt.Rows[0]["EMAILSTATUS"].ToString() == "1")
                        {
                            await mailobj.SendSaveOrderEmail(_dt.Rows[0]["orderno"].ToString(), Session["USERCODE"].ToString(), _dt.Rows[0]["EMAILSTATUS"].ToString());
                        }
                    }
                    catch (Exception exmail)
                    {
                        _message += "Message : " + exmail.Message.ToString();
                    }
                }
            }
            TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('" + _message + "');});</script>";
            return RedirectToAction("MyCart");
            //return Content(json);
        }

        [HttpGet]
        public ActionResult GetAddLineItemOnPriceGroupChange(string ProductID, string CustomerID, string Qty, string Pricegroup)
        {
            DataTable _dt = new DataTable();
            string json = string.Empty;
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            if (!string.IsNullOrWhiteSpace(ProductID) && !string.IsNullOrWhiteSpace(CustomerID) && !string.IsNullOrWhiteSpace(Qty))
            {
                _plist.Add("@ITEMID"); _vlist.Add(ProductID);
                _plist.Add("@CUSTOMERID"); _vlist.Add(Session["USERCODE"].ToString());
                _plist.Add("@QTY"); _vlist.Add(Qty);
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                decimal[] PriceArrary = CCAF.AppCode.Global.GetAxPriceList((Session["USERCODE"].ToString()), (ProductID), Convert.ToInt32(Qty), Pricegroup);
                _plist.Add("@PRICE"); _vlist.Add(Convert.ToString(PriceArrary[0]));
                _plist.Add("@DISCOUNT"); _vlist.Add(Convert.ToString(PriceArrary[1]));
                _plist.Add("@TAX"); _vlist.Add(Convert.ToString(PriceArrary[2]));
                _plist.Add("@PRICEGROUP"); _vlist.Add(Pricegroup);
                _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETCARTLINEONPRICEGROUPCHANGE", CommandType.StoredProcedure, _plist, _vlist);
            }
            if (_dt != null && _dt.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(_dt, Formatting.Indented);
            }
            return Content(json);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MyCart1(MyCartViewModel mycart)
        {
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            DataTable _dt = new DataTable();

            _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETDELIVERYMODEMASTER", CommandType.StoredProcedure, _plist, _vlist);
            ViewBag.DeliveryTypeDropdown = CCAF.BussinessLayer.BussinessLayer.ToSelectList(_dt, "CODE", "DESCRIPTION", Session["DILVMODE"].ToString());

            _plist.Add("@CUSTOMERID"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETPRICEGROUP", CommandType.StoredProcedure, _plist, _vlist);  // 

            ViewBag.PriceGroupDropDown = CCAF.BussinessLayer.BussinessLayer.ToSelectList(_dt, "PriceGroup", "PriceGroupName", Session["PRICEGROUP"].ToString());
            return View("_MyCart", mycart);
        }

        #endregion

    }
}