using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Text;
using CCAF.AXWO;
using System.IO;
using CCAF.Models;

namespace CCAF.AppCode
{
    public class Global
    {
        public SqlConnection conn = null;
        public SqlCommand cmd = null;
        public static SqlTransaction trans;
        public DataTable dt = null;

        public static Decimal[] GetAxPriceList(string CustomerCode, string ProductCode, int Qty, string PriceGroup)
        {
            decimal[] AxPriceList = new decimal[3];
            AxPriceList[0] = 0;
            AxPriceList[1] = 0;
            AxPriceList[2] = 0;
            //return AxPriceList;//Line is added for Devlopement Only, Please Comment this When Making Live
            try
            {
                WebPortalServiceClient proxy = new WebPortalServiceClient();
                CallContext cc = new CallContext();
                cc.Company = HttpContext.Current.Session["DATAAREAID"].ToString();
                cc.Language = "en-us";
                proxy.ClientCredentials.Windows.ClientCredential.UserName = Convert.ToString(HttpContext.Current.Session["APIUser"]);
                proxy.ClientCredentials.Windows.ClientCredential.Password = Convert.ToString(HttpContext.Current.Session["APIPassword"]);
                //proxy.ClientCredentials.UserName.UserName = Convert.ToString(HttpContext.Current.Session["APIUser"]);
                //proxy.ClientCredentials.UserName.Password = Convert.ToString(HttpContext.Current.Session["APIPassword"]);
                decimal[] values;
                values = proxy.getPriceDisc(cc, CustomerCode, ProductCode, Qty, PriceGroup);
                AxPriceList[0] = values[0];
                AxPriceList[1] = values[1];
                AxPriceList[2] = values[2];
                //WriteLog("Value1:" + Convert.ToString(values[0]) + ";Value2:" + Convert.ToString(values[1]) + ";DataareaId:" + values[2].ToString(), "\\LogFiles\\ErrorLog\\Error" + DateTime.Now.ToString("ddMMyyyy") + ".log");

            }
            catch (Exception ex)
            {
                //WriteLog("User:" + Convert.ToString(HttpContext.Current.Session["APIUser"]) + ";Password:" + Convert.ToString(HttpContext.Current.Session["APIPassword"]) + ";DataareaId:" + HttpContext.Current.Session["DATAAREAID"].ToString(), "\\LogFiles\\ErrorLog\\Error" + DateTime.Now.ToString("ddMMyyyy") + ".log");

                WriteLog(ex.Message.ToString(), "\\LogFiles\\ErrorLog\\Error" + DateTime.Now.ToString("ddMMyyyy") + ".log");
            }
            return AxPriceList;
        }

        public static decimal GetAxCustomerCreditBalance(string CustomerCode)
        {
            decimal values = 0;
            // return values;//Line is added for Devlopement Only, Please Comment this When Making Live
            try
            {
                WebPortalServiceClient proxy = new WebPortalServiceClient();
                CallContext cc = new CallContext();
                cc.Company = HttpContext.Current.Session["DATAAREAID"].ToString();
                cc.Language = "en-us";

                proxy.ClientCredentials.Windows.ClientCredential.UserName = Convert.ToString(HttpContext.Current.Session["APIUser"]);
                proxy.ClientCredentials.Windows.ClientCredential.Password = Convert.ToString(HttpContext.Current.Session["APIPassword"]);
                values = proxy.getCustomerBalance(cc, CustomerCode);

                //values = proxy.getPriceDisc(cc, CustomerCode, ProductCode, Qty, PriceGroup);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message.ToString(), "\\LogFiles\\ErrorLog\\" + DateTime.Now.ToString("ddMMyyyy"));
            }
            return values;
        }


        public static decimal GetItemOutOfStock(string CustomerCode, string ItemId)
        {
            //string[] values  = { };
            decimal values = 0;
            List<GetItemOutOfStockModel> itemList = new List<GetItemOutOfStockModel>();
            try
            {

                WebPortalServiceClient proxy = new WebPortalServiceClient();

                CallContext cc = new CallContext();
                cc.Company = HttpContext.Current.Session["DATAAREAID"].ToString();
                cc.Language = "en-us";

                proxy.ClientCredentials.Windows.ClientCredential.UserName = Convert.ToString(HttpContext.Current.Session["APIUser"]);
                proxy.ClientCredentials.Windows.ClientCredential.Password = Convert.ToString(HttpContext.Current.Session["APIPassword"]);
                values = proxy.getItemOutOfStock(cc, CustomerCode, ItemId);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message.ToString(), "\\LogFiles\\ErrorLog\\" + DateTime.Now.ToString("ddMMyyyy"));
            }
            return values;
        }

        public static string PendingSyncOrderStatus()
        {

            // return values;//Line is added for Devlopement Only, Please Comment this When Making Live
            try
            {
                //DataTable _dt = GetFilledDataTable("select WebOrderNo,DataAreaId from ax.ORDERHEADER where OrderStatus=2 and ISNULL(ISSYNC,0)=0", "OrderList");
                DataTable _dt = GetFilledDataTable("select WebOrderNo,DataAreaId from ax.ORDERHEADER where OrderStatus=2 and ISNULL(ISSYNC,0)=0", "ax.ORDERHEADER");
                string finalStatus = "";
                int Counter = 0;
                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in _dt.Rows)
                    {
                        Counter += 1;
                        string _SyncStatus = SendSaleOrder(dr["WebOrderNo"].ToString());

                        finalStatus += Counter.ToString() + "-" + _SyncStatus;
                        if (_SyncStatus.ToString().ToUpper() == "SUCCESS")
                        {
                            ExecQuery("exec USP_UPORDERSTATUS '" + dr["WebOrderNo"].ToString() + "'");
                        }
                    }
                    //return (finalStatus); 
                }
                else
                {
                    return ("No Pending Order to Sync");
                }
                return (Counter.ToString() + "Order Sync to AX");
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message.ToString(), "\\LogFiles\\ErrorLog\\" + DateTime.Now.ToString("ddMMyyyy"));
            }
            return "";
        }

        public static DataTable GetFilledDataTable(string strQuery, string tblName)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    using (SqlDataAdapter dap = new SqlDataAdapter(strQuery, con))
                    {
                        DataTable dtObj = new DataTable(tblName);
                        dap.Fill(dtObj);
                        return dtObj;
                    }
                }
            }
            catch { return null; }
        }
        public static bool ExecQuery(string strQuery)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    using (SqlCommand sc = new SqlCommand(strQuery, con))
                    {
                        sc.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch { return false; }
        }
        public static string SendSaleOrder(string OrderNo)
        {
            // return "";//Line is added for Devlopement Only, Please Comment this When Making Live
            try
            {

                WebPortalServiceClient proxy = new WebPortalServiceClient();

                CallContext cc = new CallContext();

                cc.Company = HttpContext.Current.Session["DATAAREAID"].ToString();

                cc.Language = "en-us";

                proxy.ClientCredentials.Windows.ClientCredential.UserName = Convert.ToString(HttpContext.Current.Session["APIUser"]);

                proxy.ClientCredentials.Windows.ClientCredential.Password = Convert.ToString(HttpContext.Current.Session["APIPassword"]);

                List<string> _plist = new List<string>();
                List<string> _vlist = new List<string>();

                DataTable _dt = new DataTable();
                _plist.Add("@ORDERNO"); _vlist.Add(OrderNo);
                _plist.Add("@DATAAREAID"); _vlist.Add(HttpContext.Current.Session["DATAAREAID"].ToString());
                _dt = GetData_New("USP_GETSYNCORDERDETAILS", CommandType.StoredProcedure, _plist, _vlist);

                if (_dt.Rows.Count > 0)

                {

                    ACXWebPortalOrderLineCC[] WOLAR = new ACXWebPortalOrderLineCC[_dt.Rows.Count];

                    ACXWebPortalOrderLineCC WOL = new ACXWebPortalOrderLineCC();

                    for (int i = 0; i < _dt.Rows.Count; i++)

                    {

                        WOL = new ACXWebPortalOrderLineCC();

                        WOL.Customercode = _dt.Rows[i]["AccountNum"].ToString();

                        WOL.deliverydate = Convert.ToDateTime(_dt.Rows[i]["DeliveryDate"]);

                        WOL.DeliveryMessage = _dt.Rows[i]["Remark"].ToString();

                        WOL.Itemcode = _dt.Rows[i]["ItemId"].ToString();

                        WOL.LineNo = Convert.ToDecimal(_dt.Rows[i]["LineNumber"]);

                        WOL.orderdate = Convert.ToDateTime(_dt.Rows[i]["OrderDate"]);

                        WOL.orderno = _dt.Rows[i]["WebOrderNo"].ToString();

                        WOL.Pricegroup = _dt.Rows[i]["PriceGroupId"].ToString();

                        WOL.Qty = Convert.ToDecimal(_dt.Rows[i]["ConfirmQty"]);
                        WOL.DeliveryMode = Convert.ToString(_dt.Rows[i]["ModeOfDelivery"]);

                        WOL.Ordertype = Convert.ToString(_dt.Rows[i]["OrderType"]);
                        WOL.Requestor = _dt.Rows[i]["REQUESTERCODE"].ToString();
                        WOLAR[i] = WOL;

                    }

                    string result = proxy.sendOrder(cc, WOLAR);

                    return result;

                }

                else

                {

                    return "";

                }

            }

            catch (Exception ex)

            {



                WriteLog(ex.Message.ToString(), "\\LogFiles\\ErrorLog\\Error" + DateTime.Now.ToString("ddMMyyyy") + ".log");

                return ex.Message.ToString();

            }

        }

        public static void WriteLog(string Log, string FileName)

        {

            try

            {

                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles"))

                { Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles"); }

                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles\\ErrorLog"))

                { Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles\\ErrorLog"); }

                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles\\Data"))

                { Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles\\Data"); }

                using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + FileName, true))

                {

                    sw.WriteLine(Log);

                }

            }

            catch { }

        }

        public static void ExportDataTable(DataTable dtExport, string FileName, string ExportType, string HeaderText)
        {
            try
            {
                //Create a dummy GridView
                GridView GridView1 = new GridView();
                GridView1.AllowPaging = false;
                GridView1.DataSource = dtExport;
                GridView1.DataBind();
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + FileName);
                HttpContext.Current.Response.Charset = "";
                if (ExportType.ToUpper() == "WORD")
                {
                    HttpContext.Current.Response.ContentType = "application/vnd.ms-word ";
                }
                else if (ExportType.ToUpper() == "PDF")
                {
                    HttpContext.Current.Response.ContentType = "application/pdf";
                }
                else
                {
                    HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                }

                System.IO.StringWriter sw = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw);
                if (ExportType.ToUpper() == "EXCEL")
                {
                    for (int i = 0; i < GridView1.Rows.Count; i++)
                    {
                        //Apply text style to each Row
                        GridView1.Rows[i].Attributes.Add("class", "textmode");
                    }
                }
                GridView1.RenderControl(hw);
                //style to format numbers to string
                if (ExportType.ToUpper() == "EXCEL")
                {
                    string style = @"<style> .textmode { mso-number-format:\@; } </style>";
                    HttpContext.Current.Response.Write(style);
                }
                HttpContext.Current.Response.Write("<table><tr><td colspan=\"" + dtExport.Columns.Count + "\"><center> <b>" + HeaderText + " </b></center></td></tr></table>");
                HttpContext.Current.Response.Output.Write(sw.ToString());
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
            catch
            { }
        }

        public static string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["CCAF"].ToString();
        }

        public static SqlConnection GetConnection()
        {
            SqlConnection conn1 = new SqlConnection(GetConnectionString());
            conn1.Open();
            return conn1;
        }

        public void CloseSqlConnection()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public String ConvertDataTableTojSonString(DataTable dataTable)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer =
                   new System.Web.Script.Serialization.JavaScriptSerializer();

            serializer.MaxJsonLength = 2147483647;// Int32.MaxValue;
            List<Dictionary<String, Object>> tableRows = new List<Dictionary<String, Object>>();

            Dictionary<String, Object> row;

            foreach (DataRow dr in dataTable.Rows)
            {
                row = new Dictionary<String, Object>();
                foreach (DataColumn col in dataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                tableRows.Add(row);
            }
            return serializer.Serialize(tableRows);
        }

        public static DataTable GetData(string query)
        {
            SqlConnection _sqlconnection = GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = _sqlconnection;
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _sqlconnection.Close();
                _sqlconnection.Dispose();
                SqlConnection.ClearPool(_sqlconnection);
            }
        }

        public static string GenerateHTMLtoDT(DataTable dtTemp, string tableid)
        {
            #region begin 
            //   ================= Create HTML ==========
            string result = "";
            try
            {
                result += " <thead> <tr>";
                foreach (DataColumn dc in dtTemp.Columns)
                {
                    result += "<th>" + dc.ColumnName + "</th>";
                }

                result += "</tr> </thead> ";
                int i;
                foreach (DataRow dr in dtTemp.Rows)
                {
                    result += "<tbody id='" + tableid + "'> <tr>";
                    for (i = 0; i < dtTemp.Columns.Count; i++)
                    {
                        result += "<td>" + dr[i].ToString() + "</td>";
                    }
                    result += "</tr></tbody>";
                }
            }
            catch (Exception ex)
            {

            }
            return result;
            #endregion
        }

        public static string GenerateHTMLtoDTNew(DataTable dtTemp, string tableid)
        {
            #region begin 
            //   ================= Create HTML ==========
            StringBuilder result = new StringBuilder();
            try
            {
                //string cdnJquery = "<script src='https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js' type='text/javascript'></script>";
                //string cdnCSS = "<script src='http://cdn.datatables.net/1.10.19/css/jquery.dataTables.min.css'></script>";
                //string cdnJS = "<script src='http://cdn.datatables.net/1.10.19/js/jquery.dataTables.min.js'></script>";
                //string cdnCSS = "<link rel='stylesheet' type='text/css' href='../DataTables/datatables.min.css'/>";
                //string cdnJS = "<script type='text/javascript' src='../DataTables/datatables.min.js'></script>";
                //result.AppendLine(cdnCSS + cdnJS);
                result.AppendLine("<table id='tbl' class='table table - bordered product_master'>");
                result.AppendLine("<thead> <tr>");
                foreach (DataColumn dc in dtTemp.Columns)
                {
                    result.AppendLine("<th>" + dc.ColumnName + "</th>");
                }

                result.AppendLine("</tr></thead><tbody id='" + tableid + "'>");
                int i;
                foreach (DataRow dr in dtTemp.Rows)
                {
                    result.AppendLine("<tr>");
                    for (i = 0; i < dtTemp.Columns.Count; i++)
                    {
                        result.AppendLine("<td>" + dr[i].ToString() + "</td>");
                    }
                    result.AppendLine("</tr>");
                }
                result.AppendLine("</tbody></table>");
                //string callScript = "<script>$(document).ready(function(){$('#tbl').DataTable();});</script>";
                //string callScript = "<script>$(document).ready(function(){var buttonCommon = { exportOptions: {  format: {  body: function ( data, row, column, node ) {  return column === 5 ? data.replace( /[$,]/g, '' ) :  data;"+
                //    "}}}};$('#tbl').DataTable({dom: 'Bfrtip',buttons: [$.extend( true, {}, buttonCommon, {extend: 'copyHtml5'} ),$.extend( true, {}, buttonCommon, {extend: 'excelHtml5'})]});});</script>";
                string callScript = @"<script>$(document).ready(function(){var buttonCommon = { exportOptions: {  format: {  body: function ( data, row, column, node ) {  return column === 5 ? data.replace( /[$,]/g, '' ) :  data;
                                        }}}};$('#tbl').DataTable({ 'processing': true,dom: 'Blpfrtip',buttons: [$.extend( true, {}, buttonCommon, {extend: 'copyHtml5',text: '<u>C</u>opy',key: {key: 'c',altKey: true},className: 'btn btn-block common_btn'} )
                                        ,$.extend(true, { }, buttonCommon, { extend: 'excelHtml5',text: 'E<u>x</u>port to Excel',key: { key: 'x',altKey: true},className: 'btn btn-block common_btn'})]});
                                                });</script>";
                //result.AppendLine(callScript);
            }
            catch (Exception ex)
            {

            }
            return result.ToString();
            #endregion
        }

        public static DataTable GetData_New(string query, CommandType type, List<string> list, List<string> item)
        {
            SqlConnection _sqlconnection = Global.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = _sqlconnection;
                cmd.CommandTimeout = 100;
                cmd.CommandType = type;
                if (type == CommandType.StoredProcedure)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        cmd.Parameters.AddWithValue(list[i].ToString(), item[i].ToString());
                    }
                }
                cmd.CommandText = query;
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _sqlconnection.Close();
                _sqlconnection.Dispose();
                SqlConnection.ClearPool(_sqlconnection);
            }
        }

        public static DataTable GetData_New(string query, CommandType type, List<string> list, List<object> item)
        {
            SqlConnection _sqlconnection = Global.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = _sqlconnection;
                cmd.CommandTimeout = 100;
                cmd.CommandType = type;
                if (type == CommandType.StoredProcedure)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        cmd.Parameters.AddWithValue(list[i].ToString(), item[i]);
                    }
                }
                cmd.CommandText = query;
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _sqlconnection.Close();
                _sqlconnection.Dispose();
                SqlConnection.ClearPool(_sqlconnection);
            }
        }

        public int ExecuteCommand(string query)
        {
            conn = GetConnection();
            try
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                trans = conn.BeginTransaction();
                cmd.Transaction = trans;
                cmd.CommandTimeout = 100;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                int rowAffected = cmd.ExecuteNonQuery();
                if (rowAffected > 0)
                {
                    trans.Commit();
                }

                return rowAffected;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                SqlConnection.ClearPool(conn);
            }
        }

        public static int ExecuteSPCommand(string query, CommandType type, List<string> list, List<string> item)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection();
            try
            {
                conn = GetConnection();
                cmd.Connection = conn;
                trans = conn.BeginTransaction();
                cmd.Transaction = trans;
                cmd.CommandTimeout = 100;
                cmd.CommandType = type;
                if (type == CommandType.StoredProcedure)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        cmd.Parameters.AddWithValue(list[i].ToString(), item[i].ToString());
                    }
                }
                cmd.CommandText = query;
                int rowAffected = cmd.ExecuteNonQuery();
                if (rowAffected > 0)
                {
                    trans.Commit();
                }

                return rowAffected;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                SqlConnection.ClearPool(conn);
            }
        }

        internal void GetData()
        {
            throw new NotImplementedException();
        }

        public string GetScalarValueOld(string query)
        {
            GetConnection();
            string value = string.Empty;
            try
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 100;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                object obj = cmd.ExecuteScalar();
                value = obj.ToString();
                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                SqlConnection.ClearPool(conn);
            }
        }

        public string GetScalarValue(string query)
        {
            GetConnection();
            string value = null;
            try
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 100;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                object obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    value = obj.ToString();
                }
                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                SqlConnection.ClearPool(conn);
            }
        }

        public static decimal ParseDecimal(string value)
        {
            try
            {
                if (value.Trim().Length == 0 || value == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToDecimal(value);
                }
            }
            catch { return 0; }
        }

        internal void BindToDropDown(RadioButtonList chkCustomerName, string sqlstr, string p1, string p2)
        {
            throw new NotImplementedException();
        }

        public DataTable GetDataTableFeed(string query, CommandType type, List<string> list, List<string> item)
        {
            GetConnection();
            try
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 100;
                cmd.CommandType = type;
                if (type == CommandType.StoredProcedure)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        cmd.Parameters.AddWithValue(list[i].ToString(), item[i].ToString());
                    }
                }
                cmd.CommandText = query;
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                SqlConnection.ClearPool(conn);
            }
        }

        public static DataTable GetDataTable(string connectionString, CommandType cmdType, string cmdText, SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            DataTable dtLocal = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                cmd.CommandTimeout = 0;
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = cmd;
                //cmd.CommandTimeout.Equals(36000);

                sqlDA.Fill(dtLocal);
                cmd.Parameters.Clear();
            }
            return dtLocal;
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        public void DeleteLine(string query)
        {
            GetConnection();
            string value = null;
            try
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 100;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                object obj = cmd.ExecuteNonQuery();
                if (obj != null)
                {
                    value = obj.ToString();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                SqlConnection.ClearPool(conn);
            }
        }

        public static decimal ConvertToDecimal(object value)
        {
            decimal retValue;
            try
            {
                if (value != null && value.ToString().Trim().Length > 0)
                {
                    retValue = Convert.ToDecimal(value);
                }
                else
                {
                    retValue = 0;
                }
            }
            catch
            {
                retValue = 0;
            }
            return retValue;
        }

        public bool IsDate(string inputDate)
        {
            bool isDate = true;
            try
            {
                DateTime dt = DateTime.Parse(inputDate);
            }
            catch (Exception)
            {
                isDate = false;
            }
            return isDate;
        }

    }
}