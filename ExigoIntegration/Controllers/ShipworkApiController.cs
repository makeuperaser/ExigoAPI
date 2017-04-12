using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExigoIntegration.Exigo;
using System.Configuration;
using ExigoIntegration.Models;
using System.Security;
using ExigoIntegration.Data;

namespace ExigoIntegration.Controllers
{
    public class ShipworkApiController : Controller
    {
        // GET: ShipworkApi            
        public ActionResult Index(string action, string username, string password, string maxcount, string order, int? status, string tracking, string carrier, string shippingcost, DateTime? shippingdate, DateTime? processeddate, bool? voided, DateTime? voideddate, string voideduser, string serviceused, decimal? totalcharges, decimal? totalweight, bool? returnshipment)
        {
            if (!(username == ConfigurationManager.AppSettings["LoginUsername"].ToString() && password == ConfigurationManager.AppSettings["LoginPassword"].ToString()))
            {
                return null;
            }
            if (this.Request.QueryString["Action"]?.ToString().ToLower() == "getmodule" || action.ToLower() == "getmodule")
            {
                ExigoModel.LoadExigoOrders(null);
                string XML = "<?xml version=\"1.0\"?>" +
                                "<ShipWorks schemaVersion=\"1.1.0\" moduleVersion=\"3.10.0\">" +
                                    "<Module>" +
                                        "<Platform>" + ConfigurationManager.AppSettings["Platform"].ToString() + "</Platform>" +
                                        "<Developer>" + ConfigurationManager.AppSettings["Developer"].ToString() + "</Developer>" +
                                        "<Capabilities>" +
                                            "<DownloadStrategy>ByModifiedTime</DownloadStrategy>" +
                                            "<OnlineCustomerID dataType=\"numeric\" supported=\"true\"/>" +
                                            "<OnlineStatus dataType=\"numeric\" supported=\"true\" supportsComments=\"true\"/>" +
                                            "<OnlineShipmentUpdate supported=\"false\"/>" +
                                        "</Capabilities>" +
                                    "</Module>" +
                                "</ShipWorks>";

                var x = new System.Xml.XmlDocument();
                x.LoadXml(XML);
                return new XmlActionResult(x);
            }
            else if (this.Request.QueryString["action"]?.ToString().ToLower() == "getstore" || action.ToLower() == "getstore")
            {
                string XML = "<?xml version=\"1.0\"?>" +
                                "<ShipWorks schemaVersion=\"1.1.0\" moduleVersion=\"3.10.0\">" +
                                    "<Store>" +
                                        "<Name>" + ConfigurationManager.AppSettings["StoreName"].ToString() + "</Name>" +
                                        "<CompanyOrOwner>" + ConfigurationManager.AppSettings["CompanyOrOwner"].ToString() + "</CompanyOrOwner>" +
                                        "<Email>" + ConfigurationManager.AppSettings["Email"].ToString() + "</Email>" +
                                        "<City>" + ConfigurationManager.AppSettings["City"].ToString() + "</City>" +
                                        "<State>" + ConfigurationManager.AppSettings["State"].ToString() + "</State>" +
                                        "<PostalCode>" + ConfigurationManager.AppSettings["PostalCode"].ToString() + "</PostalCode>" +
                                        "<Country>United State</Country>" +
                                        "<Website>www.makeuperaser.com</Website>" +
                                    "</Store>" +
                                "</ShipWorks>";

                var x = new System.Xml.XmlDocument();
                x.LoadXml(XML);
                return new XmlActionResult(x);
            }
            else if (this.Request.QueryString["action"]?.ToString().ToLower() == "getstatuscodes" || action.ToLower() == "getstatuscodes")
            {



                string XML = "<?xml version=\"1.0\"?>" +
                                "<ShipWorks schemaVersion=\"1.1.0\" moduleVersion=\"3.10.0\">" +
                                    "<StatusCodes>" +
                                        "<StatusCode>" +
                                            "<Code>1</Code>" +
                                            "<Name>Pending</Name>" +
                                        "</StatusCode>" +
                                        "<StatusCode>" +
                                            "<Code>2</Code>" +
                                            "<Name>Processing</Name>" +
                                        "</StatusCode>" +
                                        "<StatusCode>" +
                                            "<Code>3</Code>" +
                                            "<Name>Delivered</Name>" +
                                        "</StatusCode>" +
                                        "<StatusCode>" +
                                            "<Code>4</Code>" +
                                            "<Name>PayPal [Transactions]</Name>" +
                                        "</StatusCode>" +
                                    "</StatusCodes>" +
                                "</ShipWorks>";

                var x = new System.Xml.XmlDocument();
                x.LoadXml(XML);
                return new XmlActionResult(x);
            }
            else if (this.Request.QueryString["action"]?.ToString().ToLower() == "getcount" || action.ToLower() == "getcount")
            {
                string XML = "";
                using (Data.ExigoOrdersEntities DB = new Data.ExigoOrdersEntities())
                {
                    var count = DB.Orders.Where(y => y.IsOrderSent == false).Count();

                    XML = "<?xml version=\"1.0\"?>" +
                            "<ShipWorks schemaVersion=\"1.1.0\" moduleVersion=\"3.10.0\">" +
                                "<Parameters>" +
                                    "<Start>" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</Start>" +
                                    "<OrderCount>" + count + "</OrderCount>" +
                                "</Parameters>" +
                            "</ShipWorks>";
                }
                var x = new System.Xml.XmlDocument();
                x.LoadXml(XML);
                return new XmlActionResult(x);
            }
            else if (this.Request.QueryString["action"]?.ToString().ToLower() == "getorders" || action.ToLower() == "getorders")
            {
                List<Data.Order> orders = new List<Data.Order>();
                string XML = "";
                using (Data.ExigoOrdersEntities DB = new Data.ExigoOrdersEntities())
                {
                    orders = DB.Orders.Where(y => y.IsOrderSent == false).ToList();
                    orders = orders.Take(Convert.ToInt32(maxcount)).ToList();
                    XML = "<?xml version=\"1.0\"?>" +
                                    "<ShipWorks schemaVersion=\"1.1.0\" moduleVersion=\"3.10.0\">" +
                                        "<Parameters>" +
                                            "<StartGMT>" + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "</StartGMT>" +
                                            "<StartLocal>" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</StartLocal>" +
                                            "<End>" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</End>" +
                                            "<MaxCount>" + maxcount + "</MaxCount>" +
                                        "</Parameters>" +
                                        "<Orders>";
                    foreach (var Order in orders)
                    {
                        XML += "<Order>" +
                                "<OrderNumber>" + Order.OrderNumber + "</OrderNumber>" +
                                "<OrderNumberPrefix>" + "Exigo" + "</OrderNumberPrefix>" +
                                "<OrderNumberPostfix>" + "API" + "</OrderNumberPostfix>" +
                                "<OrderDate>" + (Order.OrderDate != null ? Order.OrderDate.Value.ToString("yyyy-MM-ddTHH:mm:ss") : "") + "</OrderDate>" +
                                "<LastModified>" + (Order.LastModified != null ? Order.LastModified.Value.ToString("yyyy-MM-ddTHH:mm:ss") : "") + "</LastModified>" +
                                "<ShippingMethod>Flat Rate (Best Way)</ShippingMethod>" +
                                "<StatusCode>" + 1 + "</StatusCode>" +
                                "<CustomerID>" + Order.CustomerID + "</CustomerID>" +
                                "<Notes>" + Order.Notes + "</Notes>" +
                                "<ShippingAddress>" +
                                    "<FullName>" + Order.ShippingFullName + "</FullName>" +
                                    "<Company>" + Order.ShippingCompany + "</Company>" +
                                    "<Street1>" + Order.ShippingStreet1 + "</Street1>" +
                                    (string.IsNullOrEmpty(Order.ShippingStreet2) ? "" : "<Street2>" + Order.ShippingStreet2 + "</Street2>") +
                                    (string.IsNullOrEmpty(Order.ShippingStreet3) ? "" : "<Street3>" + Order.ShippingStreet3 + "</Street3>") +
                                    "<City>" + Order.ShippingCity + "</City>" +
                                    "<State>" + Order.ShippingState + "</State>" +
                                    "<PostalCode>" + Order.ShippingPostalCode + "</PostalCode>" +
                                    "<Country>" + Order.ShippingCountry + "</Country>" +
                                    "<Phone>" + Order.ShippingPhone + "</Phone>" +
                                    "<Email>" + Order.ShippingEmail + "</Email>" +
                                "</ShippingAddress>" +
                                "<BillingAddress>" +
                                    "<FullName>" + Order.BillingFullName + "</FullName>" +
                                    "<Company>" + Order.BillingCompany + "</Company>" +
                                    "<Street1>" + Order.BillingStreet1 + "</Street1>" +
                                    (string.IsNullOrEmpty(Order.BillingStreet2) ? "" : "<Street2>" + Order.BillingStreet2 + "</Street2>") +
                                    (string.IsNullOrEmpty(Order.BillingStreet3) ? "" : "<Street3>" + Order.BillingStreet3 + "</Street3>") +
                                    "<City>" + Order.BillingCity + "</City>" +
                                    "<State>" + Order.BillingState + "</State>" +
                                    "<PostalCode>" + Order.BillingPostalCode + "</PostalCode>" +
                                    "<Country>" + Order.BillingCountry + "</Country>" +
                                    "<Phone>" + Order.BillingPhone + "</Phone>" +
                                    "<Email>" + Order.BillingEmail + "</Email>" +
                                "</BillingAddress>" +
                                "<Payment>" +
                                    "<Method>" + Order.PaymentMethod + "</Method>";
                        if (Order.PaymentMethod == PaymentType.CreditCard.ToString() && false)
                        {
                            XML += "<CreditCard>" +
                                "<Type>" + Order.CreditCardType + "</Type>" +
                                "<Owner>" + Order.CreditCardOwner + "</Owner>" +
                                "<Number>" + Order.CreditCardNumber + "</Number>" +
                                "<Expiries>" + Order.CreditCardExpires + "</Expiries>" +
                            "</CreditCard>";
                        }
                        XML += "</Payment>" +
                        "<Items>";
                        foreach (var orderItem in Order.OrderItems)
                        {
                            XML += "<Item>" +
                                    "<ItemID>" + orderItem.OrderItemID + "</ItemID>" +
                                    "<Code>" + orderItem.Code + "</Code>" +
                                    "<Name>" + orderItem.Name + "</Name>" +
                                    "<Quantity>" + orderItem.Quantity + "</Quantity>" +
                                    "<UnitPrice>" + orderItem.UnitPrice + "</UnitPrice>" +
                                    "<Weight>" + orderItem.Weight + "</Weight>" +
                                "</Item>";
                        }
                        XML += "</Items>" +
                                "<Totals>" +
                                    "<Total impact=\"none\" class=\"subtotal\" name=\"Sub-Total\" id=\"1\">" + Order.OrderItems.Sum(y => y.UnitPrice * y.Quantity) + "</Total>" +
                                    "<Total impact=\"add\" class=\"shipping\" name=\"Shipping Charges\" id=\"2\">" + Order.ShippingCost + "</Total>" +
                                    "<Total impact=\"none\" class=\"total\" name=\"Total\" id=\"3\">" + Order.Total + "</Total>" +
                                "</Totals>";
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["Debug"]))
                        {
                            XML += "<Debug>" +
                                        "<LastModifiedLocal>" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</LastModifiedLocal>" +
                                    "</Debug>";
                        }
                        XML += "</Order>";
                    }
                    XML += "</Orders>" +
                        "</ShipWorks>";

                    XML = XML.Replace("&", "&amp;").Replace("\'", "&apos;");

                    orders.ForEach(y => y.IsOrderSent = true);
                    DB.SaveChanges();
                }
                var x = new System.Xml.XmlDocument();
                x.LoadXml(XML);
                return new XmlActionResult(x);
            }
            else if (this.Request.QueryString["action"]?.ToString().ToLower() == "updatestatus" || action.ToLower() == "updatestatus")
            {
                using (ExigoOrdersEntities DB = new ExigoOrdersEntities())
                {
                    var OrderID = Convert.ToInt32(order.Replace("API", "").Replace("Exigo", ""));

                    var Order = DB.Orders.Where(x => x.OrderNumber == OrderID).FirstOrDefault();
                    if (Order != null)
                    {
                        Order.StatusCode = status;
                        DB.SaveChanges();

                        string XML = "<?xml version=\"1.0\"?>" +
                            "<ShipWorks schemaVersion=\"1.1.0\" moduleVersion=\"3.10.0\">" +
                                "<Parameters>" +
                                    "<Start>" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</Start>" +
                                "</Parameters>" +
                            "</ShipWorks>";

                        var x = new System.Xml.XmlDocument();
                        x.LoadXml(XML);
                        return new XmlActionResult(x);
                    }
                }
            }
            else if (this.Request.QueryString["action"]?.ToString().ToLower() == "updateshipment" || action.ToLower() == "updateshipment")
            {
                using (ExigoOrdersEntities DB = new ExigoOrdersEntities())
                {
                    var OrderID = Convert.ToInt32(order.Replace("API", "").Replace("Exigo", ""));

                    var Order = DB.Orders.Where(x => x.OrderNumber == OrderID).FirstOrDefault();
                    if (Order != null)
                    {
                        Order.Tracking = tracking;
                        Order.Carrier = carrier;
                        Order.ShippingDate = shippingdate;
                        Order.ProcessedDate = processeddate;
                        Order.Voided = voided;
                        Order.VoidedDate = voideddate;
                        Order.VoidedUser = voideduser;
                        Order.ServiceUsed = serviceused;
                        Order.TotalCharges = totalcharges;
                        Order.TotalWeight = totalweight;
                        Order.ReturnShipment = returnshipment;
                        DB.SaveChanges();

                        ExigoApiSoapClient client = new ExigoApiSoapClient();
                        var auth = new ApiAuthentication();
                        auth.LoginName = ConfigurationManager.AppSettings["LoginName"].ToString();
                        auth.Password = ConfigurationManager.AppSettings["Password"].ToString();
                        auth.Company = ConfigurationManager.AppSettings["Company"].ToString();

                        /// Update order to Exigo
                        var req = new UpdateOrderRequest();
                        req.OrderID = OrderID;                        
                        req.TrackingNumber1 = tracking;
                        var res = client.UpdateOrder(auth, req);

                        if (!string.IsNullOrEmpty(tracking))
                        {
                            ChangeOrderStatusRequest orderStatusReq = new ChangeOrderStatusRequest();
                            orderStatusReq.OrderID = OrderID;
                            orderStatusReq.OrderStatus = OrderStatusType.Shipped;
                            //Submit the request to the server
                            ChangeOrderStatusResponse orderStatusRes = client.ChangeOrderStatus(auth, orderStatusReq);
                            if (orderStatusRes.Result.Status == ResultStatus.Success) { }
                        }
                        string XML = "<?xml version=\"1.0\"?>" +
                            "<ShipWorks schemaVersion=\"1.1.0\" moduleVersion=\"3.10.0\">" +
                                "<Parameters>" +
                                    "<Start>" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</Start>" +
                                    "<Result>" + res.Result.Status.ToString() + "</Result>" +
                                "</Parameters>" +
                            "</ShipWorks>";

                        var x = new System.Xml.XmlDocument();
                        x.LoadXml(XML);
                        return new XmlActionResult(x);
                    }
                }
            }
            return null;
        }
    }
}