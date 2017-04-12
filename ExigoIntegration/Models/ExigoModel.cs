using ExigoIntegration.Data;
using ExigoIntegration.Exigo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ExigoIntegration.Models
{
    public static class ExigoModel
    {
        public static void LoadExigoOrders(DateTime? startDate)
        {
            ExigoApiSoapClient api = new ExigoApiSoapClient();
            var auth = new ApiAuthentication();
            auth.LoginName = ConfigurationManager.AppSettings["LoginName"].ToString();
            auth.Password = ConfigurationManager.AppSettings["Password"].ToString();
            auth.Company = ConfigurationManager.AppSettings["Company"].ToString();

            using (ExigoOrdersEntities DB = new ExigoOrdersEntities())
            {
                var order = DB.Orders.OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                if (order != null)
                {
                    startDate = order.CreatedDate;
                }
            }

            var req = new GetOrdersRequest();
            req.OrderDateStart = startDate ?? DateTime.Now.Date;
            req.OrderDateEnd = DateTime.Now;
            req.OrderStatus = OrderStatusType.Accepted;
            
            //Gets the initial batch
            var gorRes = api.GetOrders(auth, req);
            
            //this will now loop through, process the first 100 and keep going
            while (gorRes.Orders.Length > 0)
            {
                foreach (var exigoOrder in gorRes.Orders)
                {
                    using (ExigoOrdersEntities DB = new ExigoOrdersEntities())
                    {
                        Order order = new Order();
                        order.OrderNumber = exigoOrder.OrderID;
                        order.OrderNumberPrefix = "";
                        order.OrderNumberPostfix = "";
                        order.OrderDate = exigoOrder.OrderDate;
                        order.LastModified = exigoOrder.ModifiedDate;
                                                
                        order.StatusCode = (int)exigoOrder.OrderStatus;
                        order.CustomerID = exigoOrder.CustomerID;
                        order.Notes = exigoOrder.Notes;
                        order.ShippingFullName = exigoOrder.FirstName + " " + exigoOrder.LastName;
                        order.ShippingCompany = "";
                        order.ShippingStreet1 = exigoOrder.Address1;
                        order.ShippingStreet2 = exigoOrder.Address2;
                        order.ShippingStreet3 = exigoOrder.Address3;
                        order.ShippingCity = exigoOrder.City;
                        order.ShippingState = exigoOrder.State;
                        order.ShippingPostalCode = exigoOrder.Zip;
                        order.ShippingCountry = exigoOrder.Country;
                        order.ShippingPhone = exigoOrder.Phone;
                        order.ShippingEmail = exigoOrder.Email;
                        order.BillingFullName = exigoOrder.FirstName + " " + exigoOrder.LastName;
                        order.BillingCompany = "";
                        order.BillingStreet1 = exigoOrder.Address1;
                        order.BillingStreet2 = exigoOrder.Address2;
                        order.BillingStreet3 = exigoOrder.Address3;
                        order.BillingCity = exigoOrder.City;
                        order.BillingState = exigoOrder.State;
                        order.BillingPostalCode = exigoOrder.Zip;
                        order.BillingCountry = exigoOrder.Country;
                        order.BillingPhone = exigoOrder.Phone;
                        order.BillingEmail = exigoOrder.Email;

                        var payment = exigoOrder.Payments.ToList().FirstOrDefault();
                        if (payment != null)
                        {
                            order.PaymentMethod = payment.PaymentType.ToString();
                            order.BillingStreet1 = payment.BillingAddress1;
                            order.BillingStreet2 = payment.BillingAddress2;
                            order.BillingCity = payment.BillingCity;
                            order.BillingState = payment.BillingState;
                            order.BillingPostalCode = payment.BillingZip;
                            order.BillingCountry = payment.BillingCountry;
                            order.CreditCardType = payment.CreditCardType.ToString();
                            order.CreditCardOwner = "";
                            order.CreditCardNumber = payment.CreditCardNumberDisplay;
                            order.CreditCardExpires = "";
                        }
                        order.Total = exigoOrder.Total;
                        order.ShippingCost = exigoOrder.ShippingTotal;
                        order.IsOrderSent = false;
                        order.CreatedDate = DateTime.Now;
                        order.ModifiedDate = DateTime.Now;

                        foreach (var orderItem in exigoOrder.Details.ToList())
                        {
                            OrderItem item = new OrderItem();
                            item.ItemID = orderItem.ItemCode;
                            item.ProductID = orderItem.ItemCode;
                            item.Code = orderItem.ItemCode;
                            item.Name = orderItem.Description;
                            item.Quantity = orderItem.Quantity;
                            item.UnitPrice = orderItem.PriceEach;
                            item.Image = "";
                            item.Weight = orderItem.WeightEach;
                            order.OrderItems.Add(item);
                        }
                        DB.Orders.Add(order);
                        DB.SaveChanges();
                    }
                    
                }

                //This will now get the next batch of 100
                req.GreaterThanOrderID = gorRes.Orders[gorRes.Orders.Length - 1].OrderID;
                gorRes = api.GetOrders(auth, req);
            }
        }
    }
}