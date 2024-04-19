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
    public class GenerateOrderController : Controller
    {
        // GET: GenerateOrder
        public ActionResult GenerateOrder(string WebOrderNo ="" , string AccountNum = "")
        {
            Session["WebOrderNo"] = WebOrderNo;
            //Session["AccountNum"] = AccountNum;
            Session["_Draftdt"] = "";
            List<string> _plistdraft = new List<string>();
            List<string> _vlistdraft = new List<string>();
            List <string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            DataTable _dt = new DataTable();
            DataTable _Draftdt = new DataTable();

            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETITEMLIST", CommandType.StoredProcedure, _plist, _vlist);

            ViewBag.ItemListDropdown = CCAF.BussinessLayer.BussinessLayer.ToSelectList(_dt, "ITEMID", "NAME");

            _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
            _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETDELIVERYMODEMASTER", CommandType.StoredProcedure, _plist, _vlist);
            ViewBag.DeliveryTypeDropdown = CCAF.BussinessLayer.BussinessLayer.ToSelectList(_dt, "CODE", "DESCRIPTION", Session["DILVMODE"].ToString());            

            _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETLINKEDCUSTOMER", CommandType.StoredProcedure, _plist, _vlist);
            ViewBag.CustomerDropdown = CCAF.BussinessLayer.BussinessLayer.ToSelectList(_dt, "CustomerCode", "CustomerName", Session["USERCODE"].ToString());


            _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETORDERTYPEMASTER", CommandType.StoredProcedure, _plist, _vlist);
            ViewBag.OrderTypeDropdown = CCAF.BussinessLayer.BussinessLayer.ToSelectList(_dt, "CODE", "DESCRIPTION", Session["ORDERTYPE"].ToString());


            _plistdraft.Add("@WebOrderNo"); _vlistdraft.Add(WebOrderNo);
            _plistdraft.Add("@CUSTOMERID"); _vlistdraft.Add(AccountNum);
            _plistdraft.Add("@DATAAREAID"); _vlistdraft.Add(Session["DATAAREAID"].ToString());
            _Draftdt = CCAF.AppCode.Global.GetData_New("USP_ACXGETDRAFTLINE", CommandType.StoredProcedure, _plistdraft, _vlistdraft);
            Session["_Draftdt"] = _Draftdt;

            //ViewBag.OrderTypeDropdown = new SelectList(new List<SelectListItem> { new SelectListItem() { Text="General",Value="General" },
            //    new SelectListItem() { Text = "FOC", Value = "FOC" } }, "Value", "Text", "General");

            //OrderViewModel PostedData = new OrderViewModel();
            //OLine line = new OLine();
            //line.ProductCode = "2112";
            //line.ProductName = "PServ Name";
            //line.Qty = 5;
            //line.UOM = "CASE";
            //line.UnitPrice = 10;
            //line.PriceGP = "TestServerPG";
            //line.TaxAmount = 30;
            //line.ExtendedPrice = 30;
            //line.Amount = 300;
            //List<OLine> lineList = new List<OLine>();
            //lineList.Add(line);
            //PostedData.Line = lineList;
            return View("_GenerateOrder");
        }

        [HttpGet]
        public ActionResult DraftOrder()
        {
         
            DataTable _dt = new DataTable();
            string json = string.Empty;

            _dt = ((DataTable)Session["_Draftdt"]);
            
            if (_dt != null && _dt.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(_dt, Formatting.Indented);
            }
            return Content(json);
        }

        public List<OLine> ConvertOrderLineList( DataTable Odata)
        {
            List<OLine> _OLine = new List<OLine>();
            if(Odata.Rows.Count>0)
            {              
                foreach(DataRow items in Odata.Rows)
                {
                    OLine _lstline = new OLine();
                    _lstline.ProductCode = items["ProductCode"].ToString();
                    _lstline.ProductName = items["ProductName"].ToString();
                    _lstline.Qty = Convert.ToDecimal(items["Qty"].ToString());
                    _lstline.UOM = items["UOM"].ToString();
                    _lstline.UnitPriceVEP = Convert.ToDecimal(items["UnitPriceVEP"].ToString());
                    _lstline.UnitPriceVIP = Convert.ToDecimal(items["UnitPriceVIP"].ToString());
                    _lstline.TaxAmount = Convert.ToDecimal(items["TaxAmount"].ToString());
                    _lstline.ExtendedPriceVIP = Convert.ToDecimal(items["ExtendedPriceVIP"].ToString());
                    _lstline.PriceGP = items["PriceGP"].ToString();
                    
                    _OLine.Add(_lstline);
                }

                return _OLine;
            }
            return _OLine;
        }

        [HttpGet]
        public ActionResult GetCustomerDetailsbyID(string CustomerID)
        {
            DataTable _dt = new DataTable();
            string json = string.Empty;
            if (!string.IsNullOrWhiteSpace(CustomerID))
            {
                List<string> _plist = new List<string>();
                List<string> _vlist = new List<string>();
                _plist.Add("@CUSTOMERID"); _vlist.Add(CustomerID);
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
                decimal CreditBal = CCAF.AppCode.Global.GetAxCustomerCreditBalance((CustomerID));
                _plist.Add("@CRBALANCE"); _vlist.Add(Convert.ToString(CreditBal));
                _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETCUSTOMERDETAILSBYID", CommandType.StoredProcedure, _plist, _vlist);
            }
            if (_dt != null && _dt.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(_dt, Formatting.Indented);
            }
            return Content(json);
        }

        [HttpGet]
        public ActionResult GetItemDetailByID(string ProductID)
        {
            DataTable _dt = new DataTable();
            string json = string.Empty;
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            if (!string.IsNullOrWhiteSpace(ProductID))
            {
                _plist.Add("@ITEMID"); _vlist.Add(ProductID);
                _plist.Add("@DATAAREID"); _vlist.Add(Session["DATAAREAID"].ToString());
                _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETITEMLISTDETAILBYID", CommandType.StoredProcedure, _plist, _vlist);
            }
            if (_dt != null && _dt.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(_dt, Formatting.Indented);
            }
            return Content(json);
        }

        //[HttpGet]
        //public ActionResult GetOrderTypeFoc(string Ordertype)
        //{
        //    DataTable _dt = new DataTable();
        //    string json = string.Empty;
        //    List<string> _plist = new List<string>();
        //    List<string> _vlist = new List<string>();
        //    if (!string.IsNullOrWhiteSpace(Ordertype))
        //    {
        //        _plist.Add("@ORDERTYPE"); _vlist.Add(Ordertype);
        //        _plist.Add("@DATAAREID"); _vlist.Add(Session["DATAAREAID"].ToString());
        //        _dt = CCAF.AppCode.Global.GetData_New("USP_ACXCHECKORDERTYPEFOC", CommandType.StoredProcedure, _plist, _vlist);
        //    }
        //    if (_dt != null && _dt.Rows.Count > 0)
        //    {
        //        json = JsonConvert.SerializeObject(_dt, Formatting.Indented);
        //    }
        //    return Content(json);
        //}

        
        [HttpGet]
        public ActionResult GetAddLineItem(string ProductID,string CustomerID, string Qty)
        {
            DataTable _dt = new DataTable();
            string json = string.Empty;
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            if (!string.IsNullOrWhiteSpace(ProductID) && !string.IsNullOrWhiteSpace(CustomerID) && !string.IsNullOrWhiteSpace(Qty))
            {
                _plist.Add("@ITEMID"); _vlist.Add(ProductID);
                _plist.Add("@CUSTOMERID"); _vlist.Add(CustomerID);
                _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
                _plist.Add("@QTY"); _vlist.Add(Qty);
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                decimal[] PriceArrary = CCAF.AppCode.Global.GetAxPriceList((CustomerID), (ProductID), Convert.ToInt32(Qty), "");
                _plist.Add("@PRICE"); _vlist.Add(Convert.ToString(PriceArrary[0]));
                _plist.Add("@DISCOUNT"); _vlist.Add(Convert.ToString(PriceArrary[1]));
                _plist.Add("@TAX"); _vlist.Add(Convert.ToString(PriceArrary[2]));
                _plist.Add("@PRICEGROUP"); _vlist.Add("");
                _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETADDLINEFORORDER", CommandType.StoredProcedure, _plist, _vlist);
            }
            if (_dt != null && _dt.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(_dt, Formatting.Indented);
            }
            return Content(json);
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
                _plist.Add("@CUSTOMERID"); _vlist.Add(CustomerID);
                _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
                _plist.Add("@QTY"); _vlist.Add(Qty);
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                decimal[] PriceArrary = CCAF.AppCode.Global.GetAxPriceList((CustomerID), (ProductID), Convert.ToInt32(Qty), Pricegroup);
                _plist.Add("@PRICE"); _vlist.Add(Convert.ToString(PriceArrary[0]));
                _plist.Add("@DISCOUNT"); _vlist.Add(Convert.ToString(PriceArrary[1]));
                _plist.Add("@TAX"); _vlist.Add(Convert.ToString(PriceArrary[2]));
                _plist.Add("@PRICEGROUP"); _vlist.Add(Pricegroup);
                _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETLINEONPRICEGROUPCHANGE", CommandType.StoredProcedure, _plist, _vlist);
            }
            if (_dt != null && _dt.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(_dt, Formatting.Indented);
            }
            return Content(json);
        }


        [HttpGet]
        public ActionResult GetDataForPriceGroup(string CustomerID)
        {
            DataTable _dt = new DataTable();
            string json = string.Empty;
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(CustomerID))
            {
                _plist.Add("@CUSTOMERID"); _vlist.Add(CustomerID);
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETPRICEGROUP", CommandType.StoredProcedure, _plist, _vlist);
            }
            if (_dt != null && _dt.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(_dt, Formatting.Indented);
            }
            return Content(json);
        }

        [HttpGet]
        public decimal GetItemStockList(string CustomerID, string ProductId)
        {
            DataTable _dt = new DataTable();
            decimal json = 0;
            //List<GetItemOutOfStockModel> itemList = new List<GetItemOutOfStockModel>();
            if (!string.IsNullOrWhiteSpace(CustomerID) && !string.IsNullOrWhiteSpace(ProductId))
            {
                json = CCAF.AppCode.Global.GetItemOutOfStock(CustomerID, ProductId);
            }
            return json;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> GenerateOrder(OrderViewModel PostedData)
        {
            string _message = string.Empty;
            try
            {
                List<OLine> OrderlineParm = PostedData.Line;
                OHeader Header = PostedData.Header;
                List<string> _plist = new List<string>();
                List<object> _vlist = new List<object>();
                DataTable _dt = new DataTable();
                List<OHeader> OrderHeaderParm = new List<OHeader>();
                OrderHeaderParm.Add(Header);
                if (OrderlineParm == null)
                {
                    _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETDELIVERYMODEMASTER", CommandType.StoredProcedure, _plist, _vlist);
                    ViewBag.DeliveryTypeDropdown = CCAF.BussinessLayer.BussinessLayer.ToSelectList(_dt, "CODE", "DESCRIPTION", Session["DILVMODE"].ToString());

                    _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                    _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETITEMLIST", CommandType.StoredProcedure, _plist, _vlist);

                    ViewBag.ItemListDropdown = CCAF.BussinessLayer.BussinessLayer.ToSelectList(_dt, "ITEMID", "NAME");

                    _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());

                    _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETLINKEDCUSTOMER", CommandType.StoredProcedure, _plist, _vlist);
                    ViewBag.CustomerDropdown = CCAF.BussinessLayer.BussinessLayer.ToSelectList(_dt, "CustomerCode", "CustomerName", Session["USERCODE"].ToString());


                    _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETORDERTYPEMASTER", CommandType.StoredProcedure, _plist, _vlist);
                    ViewBag.OrderTypeDropdown = CCAF.BussinessLayer.BussinessLayer.ToSelectList(_dt, "CODE", "DESCRIPTION", Session["ORDERTYPE"].ToString());

                    _message = "Please Add Line Item First";
                    ViewBag.div = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('" + _message + "');});</script>";
                    return View("_GenerateOrder", PostedData);
                }
                decimal CreditBal = CCAF.AppCode.Global.GetAxCustomerCreditBalance(Header.CustomerName);                

                _plist.Add("@AXCREDITBALANCE"); _vlist.Add(CreditBal);
                _plist.Add("@WEBORDERNO"); _vlist.Add(Session["WebOrderNo"].ToString());
                _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
                _plist.Add("@ORDERDATAHEADER"); _vlist.Add(CCAF.BussinessLayer.BussinessLayer.ToDataTable(OrderHeaderParm));
                _plist.Add("@ORDERDATALINE"); _vlist.Add(BussinessLayer.BussinessLayer.ToDataTable(OrderlineParm));
                _dt = CCAF.AppCode.Global.GetData_New("USP_ACXPOSTORDERTODB", CommandType.StoredProcedure, _plist, _vlist);
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    if (_dt.Rows[0]["STATUSRESULT"].ToString() == "Success")
                    {
                        ////Sending Order to AX
                        _message = "Order Number : " + _dt.Rows[0]["ORDERNUMBER"].ToString() + " Saved Successfully!";
                        try
                        {
                            string _SyncStatus = AppCode.Global.SendSaleOrder(_dt.Rows[0]["ORDERNUMBER"].ToString());
                            if (_SyncStatus.ToString().ToUpper() == "SUCCESS")
                            {
                                _plist.Clear(); _vlist.Clear();
                                _plist.Add("@ORDERNO"); _vlist.Add(_dt.Rows[0]["ORDERNUMBER"].ToString());
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
                            _message += ex.Message.ToString();
                        }
                        TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('" + _message + "');});</script>";
                        try
                        {
                            ////Sending Email
                            BussinessLayer.Email mailobj = new BussinessLayer.Email();
                            if (_dt.Rows[0]["EMAILSTATUS"].ToString() == "2")
                            {
                                await mailobj.SendApproveOrderEmail(_dt.Rows[0]["ORDERNUMBER"].ToString(), Header.CustomerName, _dt.Rows[0]["EMAILSTATUS"].ToString());
                            }
                            else if (_dt.Rows[0]["EMAILSTATUS"].ToString() == "1")
                            {
                                await mailobj.SendSaveOrderEmail(_dt.Rows[0]["ORDERNUMBER"].ToString(), Header.CustomerName, _dt.Rows[0]["EMAILSTATUS"].ToString());
                            }
                        }
                        catch(Exception exmail)
                        {
                            _message += exmail.Message.ToString();
                        }
                        return RedirectToAction("GenerateOrder");
                    }

                    if (_dt.Rows[0]["STATUSRESULT"].ToString() == "Draft")
                    {
                        ////Sending Order to AX
                        _message = "Extended Amount VIP Total Has been exceed Credit Balance Order Number : " + _dt.Rows[0]["ORDERNUMBER"].ToString() + " Saved in Draft!";
                        TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('" + _message + "');});</script>";

                        //try
                        //{
                        //    ////Sending Email
                        //    BussinessLayer.Email mailobj = new BussinessLayer.Email();
                        //    if (_dt.Rows[0]["EMAILSTATUS"].ToString() == "2")
                        //    {
                        //        await mailobj.SendApproveOrderEmail(_dt.Rows[0]["ORDERNUMBER"].ToString(), Header.CustomerName, _dt.Rows[0]["EMAILSTATUS"].ToString());
                        //    }
                        //    else if (_dt.Rows[0]["EMAILSTATUS"].ToString() == "1")
                        //    {
                        //        await mailobj.SendSaveOrderEmail(_dt.Rows[0]["ORDERNUMBER"].ToString(), Header.CustomerName, _dt.Rows[0]["EMAILSTATUS"].ToString());
                        //    }
                        //}
                        //catch (Exception exmail)
                        //{
                        //    _message += exmail.Message.ToString();
                        //}
                        return RedirectToAction("GenerateOrder");
                    }
                }
                else
                { 
                    TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('" + _message + "');});</script>";
                    return RedirectToAction("GenerateOrder");
                }
            }
            catch (Exception ex)
            {
                _message = ex.Message.ToString();
                TempData["msg"] = "<script type='text/javascript'>$(document).ready(function() { ShowMessage('" + _message + "');});</script>";
                return RedirectToAction("GenerateOrder");
            }

            return Content("");
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetData(string FunctionName, string objData,[Optional] int specific)
        {
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            DataTable dtParmList = new DataTable();
            try
            {
                if(objData.Length>0)
                dtParmList = (DataTable)JsonConvert.DeserializeObject("[" + objData + "]", (typeof(DataTable)));
            }
            catch (Exception ex)
            {
                return Content("Wrong Parameter Construction for objData, "+ex.Message.ToString());
            }
           try
            {
                if (dtParmList.Rows.Count == 1)
                {
                    DataRow dr = dtParmList.Rows[0];
                    foreach (DataColumn dc in dtParmList.Columns)
                    {
                        _plist.Add("@" + dc.ColumnName.ToString().ToUpper());
                        _vlist.Add(dtParmList.Rows[0][dc.ColumnName].ToString());
                    }
                }
                if (specific == 1)
                {
                    _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                    _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
                }
                else if (specific == 2)
                {
                    _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                    _plist.Add("@CUSTOMERID"); _vlist.Add(Session["USERCODE"].ToString());
                }
                else
                {
                    return Content("Unknown Parameter for Specific");
                }
            }
            catch(Exception ex)
            {
                return Content("Blank Property Name Not Allowed, "+ex.Message.ToString());
            }

            DataTable _dtResult = new DataTable();
            try
            {
                _dtResult = CCAF.AppCode.Global.GetData_New("USP_ACX" + FunctionName, CommandType.StoredProcedure, _plist, _vlist);
            }
            catch(Exception ex)
            {
                // return Content(ex.Message.ToString());
                return Json("Error!" + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }

            string json = string.Empty;
            if (_dtResult != null && _dtResult.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(_dtResult, Formatting.Indented);
            }
            if (string.IsNullOrEmpty(json))
            return Json("No Data Found !", JsonRequestBehavior.AllowGet);
            return Content(json);
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetDataToDasboard(string FunctionName, string objData, [Optional] int specific)
        {
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            DataTable dtParmList = new DataTable();
            try
            {
                if (objData.Length > 0)
                    dtParmList = (DataTable)JsonConvert.DeserializeObject("[" + objData + "]", (typeof(DataTable)));
            }
            catch (Exception ex)
            {
                return Content("Wrong Parameter Construction for objData, " + ex.Message.ToString());
            }
            try
            {
                if (dtParmList.Rows.Count == 1)
                {
                    DataRow dr = dtParmList.Rows[0];
                    foreach (DataColumn dc in dtParmList.Columns)
                    {
                        _plist.Add("@" + dc.ColumnName.ToString().ToUpper());
                        _vlist.Add(dtParmList.Rows[0][dc.ColumnName].ToString());
                    }
                }
                if (specific == 1)
                {
                    _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                    _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
                }
                else if (specific == 2)
                {
                    _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
                    _plist.Add("@CUSTOMERID"); _vlist.Add(Session["USERCODE"].ToString());
                }
            }
            catch (Exception ex)
            {
                return Content("Blank Property Name Not Allowed, " + ex.Message.ToString());
            }

            DataTable _dtResult = new DataTable();
            try
            {
                _dtResult = CCAF.AppCode.Global.GetData_New("USP_ACX" + FunctionName, CommandType.StoredProcedure, _plist, _vlist);
            }
            catch (Exception ex)
            {
                // return Content(ex.Message.ToString());
                return Json("Error!" + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }

            string json = string.Empty;
            if (_dtResult != null && _dtResult.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(_dtResult, Formatting.Indented);
            }
            // if (string.IsNullOrEmpty(json))
             // return Json("No Data Found !", JsonRequestBehavior.AllowGet);
            return Content(json);
        }

        public async Task SendApproveOrderEmail(string weborder, string customerid, string status)
        {
            //string parmObj= @"{"mailtoid":"sachin.bharadwaj @acxiomconsulting.com","mailsubject":"API Mail","mailbody":" < b > test </ b > Email Body from URL Parm."}"
            string introline = string.Empty;
            if (status == "2")
            {
                introline = "Your order has been processed. The following are the confirmed order details:";
            }
            else
            {
                introline = "Your order has been cancelled. The following are the rejected order details:";
            }
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@CUSTOMERID"); _vlist.Add(customerid);
            _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _plist.Add("@WEBORDERNO"); _vlist.Add(weborder);
            DataTable _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETEMAILBODYFORAPPROVAL", CommandType.StoredProcedure, _plist, _vlist);
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/ApproveOrderTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", _dt.Rows[0]["CUSTOMERNAME"].ToString());
            body = body.Replace("{OrderNo}", _dt.Rows[0]["WEBORDERNO"].ToString());
            body = body.Replace("{OrderDate}", _dt.Rows[0]["ORDERDATE"].ToString());
            body = body.Replace("{DeliveryDate}", _dt.Rows[0]["DELIVERYDATE"].ToString());
            body = body.Replace("{OrderAmount}", _dt.Rows[0]["TOTALAMOUNT"].ToString());
            body = body.Replace("{IntroLine}", _dt.Rows[0]["INTROLINE"].ToString());
            string _LineTable = "<tbody>";

            foreach (DataRow dr in _dt.Rows)
            {
                _LineTable += "<tr><td style='width: 56px;'>" + dr["SRNO"].ToString();
                _LineTable += "</td><td style='width: 278px;'>" + dr["PRODUCTNAME"].ToString();
                _LineTable += "</td><td style='width: 77px;'>" + dr["CONFIRMQTY"].ToString();
                _LineTable += "</td><td style='width: 109px;'>" + dr["EXTPRICEVIP"].ToString() + "</td></tr>";
            }
            body = body.Replace("{LineTable}", _LineTable);

            EmailParmObject emailParm = new EmailParmObject();
            emailParm.mailtoid = _dt.Rows[0]["TOEMAILID"].ToString();
            emailParm.mailccid = _dt.Rows[0]["CCEMAILID"].ToString();
            emailParm.mailsubject = _dt.Rows[0]["MAILSUBJECT"].ToString();
            emailParm.mailbody = body;

            string JSONparmObject = JsonConvert.SerializeObject(emailParm);

            BussinessLayer.Email emailobj = new BussinessLayer.Email();

            await emailobj.SendEmail((int)_dt.Rows[0]["EVENTID"], JSONparmObject);

        }

        public async Task SendSaveOrderEmail(string weborder, string customerid, string status)
        {
            //string parmObj= @"{"mailtoid":"sachin.bharadwaj @acxiomconsulting.com","mailsubject":"API Mail","mailbody":" < b > test </ b > Email Body from URL Parm."}"
            string introline = string.Empty;
            if (status == "1")
            {
                introline = "Thank you for your order. The following are your order details:";
            }
            else
            {
                return;
            }
            List<string> _plist = new List<string>();
            List<string> _vlist = new List<string>();
            _plist.Add("@CUSTOMERID"); _vlist.Add(customerid);
            _plist.Add("@USERCODE"); _vlist.Add(Session["USERCODE"].ToString());
            _plist.Add("@DATAAREAID"); _vlist.Add(Session["DATAAREAID"].ToString());
            _plist.Add("@WEBORDERNO"); _vlist.Add(weborder);
            DataTable _dt = CCAF.AppCode.Global.GetData_New("USP_ACXGETEMAILBODYFORORDERSAVE", CommandType.StoredProcedure, _plist, _vlist);
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/SaveOrderTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", _dt.Rows[0]["CUSTOMERNAME"].ToString());
            body = body.Replace("{OrderNo}", _dt.Rows[0]["WEBORDERNO"].ToString());
            body = body.Replace("{OrderDate}", _dt.Rows[0]["ORDERDATE"].ToString());
            body = body.Replace("{DeliveryDate}", _dt.Rows[0]["DELIVERYDATE"].ToString());
            body = body.Replace("{OrderAmount}", _dt.Rows[0]["TOTALAMOUNT"].ToString());
            body = body.Replace("{IntroLine}", introline);
            string _LineTable = "<tbody>";

            foreach (DataRow dr in _dt.Rows)
            {
                _LineTable += "<tr><td style='width: 56px;'>" + dr["SRNO"].ToString();
                _LineTable += "</td><td style='width: 278px;'>" + dr["PRODUCTNAME"].ToString();
                _LineTable += "</td><td style='width: 77px;'>" + dr["CONFIRMQTY"].ToString();
                _LineTable += "</td><td style='width: 109px;'>" + dr["EXTPRICEVIP"].ToString() + "</td></tr>";
            }
            body = body.Replace("{LineTable}", _LineTable);
            body = body.Replace("{CCAREPNAME}", _dt.Rows[0]["CCAREPNAME"].ToString());
            body = body.Replace("{CCAEMAIL}", _dt.Rows[0]["CCAREPEMAILID"].ToString());
            body = body.Replace("{CCACONTACT}", _dt.Rows[0]["CCAREPCONTACT"].ToString());

            EmailParmObject emailParm = new EmailParmObject();
            emailParm.mailtoid = _dt.Rows[0]["TOEMAILID"].ToString();
            emailParm.mailccid = _dt.Rows[0]["CCEMAILID"].ToString();
            emailParm.mailsubject = _dt.Rows[0]["MAILSUBJECT"].ToString();
            emailParm.mailbody = body;

            string JSONparmObject = JsonConvert.SerializeObject(emailParm);

            BussinessLayer.Email emailobj = new BussinessLayer.Email();

            await emailobj.SendEmail((int)_dt.Rows[0]["EVENTID"], JSONparmObject);

        }
    }
}