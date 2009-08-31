using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Linq.Dynamic;

namespace MVVM.DataAccess
{
    /// <summary>
    /// Provides CRUD operation to the MVVM_DEMO SQL database
    /// using LINQ to SQL
    /// </summary>
    public static class DataService
    {

        #region SELECT
        /// <summary>
        /// Returns a list of all Products
        /// </summary>
        /// <returns></returns>
        public static List<MVVM.DataAccess.Product> FetchAllProducts()
        {
            MVVM_DemoDataContext context = new MVVM_DemoDataContext();
            return context.Products.ToList();
        }

        /// <summary>
        /// Returns a list of all Orders based on the Customers Id
        /// </summary>
        /// <returns></returns>
        public static List<MVVM.DataAccess.Order> FetchAllOrders(Int32 customerId)
        {
            MVVM_DemoDataContext context = new MVVM_DemoDataContext();
            return context.Orders.Where(o => o.CustomerId == customerId).ToList();
        }

        /// <summary>
        /// Returns a list of all Customers
        /// </summary>
        /// <returns></returns>
        public static List<MVVM.DataAccess.Customer> FetchAllCustomers()
        {
            MVVM_DemoDataContext context = new MVVM_DemoDataContext();
            return context.Customers.ToList();
        }


        /// <summary>
        /// Returns a list of all Customers that match a filter
        /// this uses the DynamicLINQ API
        /// </summary>
        public static List<MVVM.DataAccess.Customer> FindMatchingCustomers(String filter)
        {
            MVVM_DemoDataContext context = new MVVM_DemoDataContext();
            return context.Customers.Where(filter, null).ToList(); ;
        }
        #endregion

        #region DELETE

        /// <summary>
        /// Deletes a Customer based on a CustomerId
        /// </summary>
        public static Boolean DeleteCustomer(Int32 CustomerId)
        {
            MVVM_DemoDataContext context = new MVVM_DemoDataContext();
            var custToDelete = 
                context.Customers.Where(c => c.CustomerId == CustomerId).SingleOrDefault();

            context.Customers.DeleteOnSubmit(custToDelete);
            context.SubmitChanges();
            return true;
        }

        /// <summary>
        /// Deletes a Order based on a OrderId
        /// </summary>
        public static Boolean DeleteOrder(Int32 OrderId)
        {
            MVVM_DemoDataContext context = new MVVM_DemoDataContext();
            var orderToDelete =
                context.Orders.Where(o => o.OrderId == OrderId).SingleOrDefault();

            context.Orders.DeleteOnSubmit(orderToDelete);
            context.SubmitChanges();
            return true;
        }

        #endregion

        #region INSERT

        /// <summary>
        /// Adds a new Customer
        /// </summary>
        public static Int32 AddCustomer(MVVM.DataAccess.Customer newCustomer)
        {
            MVVM_DemoDataContext context = new MVVM_DemoDataContext();
            context.Customers.InsertOnSubmit(newCustomer);
            context.SubmitChanges();
            return newCustomer.CustomerId;
        }

        /// <summary>
        /// Adds a new Order
        /// </summary>
        public static Int32 AddOrder(MVVM.DataAccess.Order newOrder)
        {
            MVVM_DemoDataContext context = new MVVM_DemoDataContext();
            context.Orders.InsertOnSubmit(newOrder);
            context.SubmitChanges();
            return newOrder.OrderId;
        }

        #endregion

        #region UPDATE

        /// <summary>
        /// Updates an existing Customer
        /// </summary>
        public static Boolean UpdateCustomer(MVVM.DataAccess.Customer newCustomer)
        {
            MVVM_DemoDataContext context = new MVVM_DemoDataContext();
            var customerToUpdate =
                context.Customers.Where(c => c.CustomerId == newCustomer.CustomerId)
                    .SingleOrDefault();

            //update the values
            customerToUpdate.FirstName = newCustomer.FirstName;
            customerToUpdate.LastName = newCustomer.LastName;
            customerToUpdate.Email = newCustomer.Email;
            customerToUpdate.HomePhoneNumber = newCustomer.HomePhoneNumber;
            customerToUpdate.MobilePhoneNumber = newCustomer.MobilePhoneNumber;
            customerToUpdate.Address1 = newCustomer.Address1;
            customerToUpdate.Address2 = newCustomer.Address2;
            customerToUpdate.Address3 = newCustomer.Address3;

            context.SubmitChanges();
            return true;
        }

        /// <summary>
        /// Updates an existing Order
        /// </summary>
        public static Boolean UpdateOrder(MVVM.DataAccess.Order newOrder)
        {
            MVVM_DemoDataContext context = new MVVM_DemoDataContext();
            var orderToUpdate =
                context.Orders.Where(o => o.OrderId == newOrder.OrderId)
                    .SingleOrDefault();

            //update the values
            orderToUpdate.CustomerId = newOrder.CustomerId;
            orderToUpdate.Quantity = newOrder.Quantity;
            orderToUpdate.ProductId = newOrder.ProductId;
            orderToUpdate.DeliveryDate = newOrder.DeliveryDate;

            context.SubmitChanges();
            return true;
        }

        #endregion

    }
}
