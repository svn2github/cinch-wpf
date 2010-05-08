using System;
using System.ComponentModel;
using System.Diagnostics;

using Cinch;

using MVVM.DataAccess;
using System.Linq.Expressions;

namespace MVVM.Models
{
    /// <summary>
    /// Respresents a UI Product Model
    /// This is never entered from the UI, and is Static
    /// data within the database, and is assumed to be
    /// part of the initial database structure
    /// That is why this class simply implements
    /// INotifyPropertyChanged. Really it doesnt even
    /// need to do that as these items will never change
    /// but for consistency decided to make this a notifying
    /// object
    /// </summary>
    public class ProductModel : INotifyPropertyChanged
    {
        #region Data
		private Int32 productId;
		private String productName;
		private Double productPrice;
        #endregion

        #region Ctor
        public ProductModel()
        {

        }
        #endregion

        #region Public Properties

        /// <summary>
        /// ProductId
        /// </summary>
        static PropertyChangedEventArgs productIdChangeArgs =
            ObservableHelper.CreateArgs<ProductModel>(x => x.ProductId);

        public Int32 ProductId
        {
            get { return productId; }
            set
            {
                productId = value;
                NotifyPropertyChanged(productIdChangeArgs);
            }
        }

        /// <summary>
        /// ProductName
        /// </summary>
        static PropertyChangedEventArgs productNameChangeArgs =
            ObservableHelper.CreateArgs<ProductModel>(x => x.ProductName);

        public String ProductName
        {
            get { return productName; }
            set
            {
                productName = value;
                NotifyPropertyChanged(productNameChangeArgs);
            }
        }

        /// <summary>
        /// ProductPrice
        /// </summary>
        static PropertyChangedEventArgs productPriceChangeArgs =
            ObservableHelper.CreateArgs<ProductModel>(x => x.ProductPrice);

        public Double ProductPrice
        {
            get { return productPrice; }
            set
            {
                productPrice = value;
                NotifyPropertyChanged(productPriceChangeArgs);
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Allows Service layer objects to be translated into
        /// UI objects
        /// </summary>
        /// <param name="cust">Service layer object</param>
        /// <returns>UI layer object</returns>
        public static ProductModel ProductToProductModel(Product product)
        {
            ProductModel productModel = new ProductModel();
            productModel.ProductId = product.ProductId;
            productModel.ProductName = product.ProductName;
            productModel.ProductPrice = product.ProductPrice;
            return productModel;

        }
        #endregion

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify using pre-made PropertyChangedEventArgs
        /// </summary>
        /// <param name="args"></param>
        protected void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Notify using String property name
        /// </summary>
        protected void NotifyPropertyChanged(String propertyName)
        {
            this.VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
