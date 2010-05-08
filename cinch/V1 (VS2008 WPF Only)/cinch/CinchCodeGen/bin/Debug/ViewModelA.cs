using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections.Specialized;
using Cinch;

namespace viewmodels
{
	/// <summary>
	///You may edit this code by hand, but there is DataWrapper code
	///and some boiler plate code provided here, to help you on your way.
	///A lot of which is actually quite useful, and a lot of thought has been
	///put into, what code to place in which file parts, and this custom part
	///does have some excellent starting code, so use it as you wish.
	///
	///But please note : One area that will need to be examined closely if you decide to introduce
	///New DataWrapper<T> properties in this part, is the IsValid override
	///Which will need to include the dataWrappers something like:
	///<pre>
	///       return base.IsValid &&
	///          DataWrapperHelper.AllValid(cachedListOfDataWrappers);
	///</pre>
	/// </summary>
	public partial class sameVM
	{
		#region Data
		private IEnumerable<DataWrapperBase> cachedListOfDataWrappers;
		private ViewMode currentViewMode = ViewMode.AddMode;
		#endregion

		#region Ctor
		public sameVM()
		{
			#region Create DataWrappers
			PersonA = new Cinch.DataWrapper<Decimal>(this,personAChangeArgs);
			PersonB = new Cinch.DataWrapper<Int32>(this,personBChangeArgs);
			//fetch list of all DataWrappers, so they can be used again later without the
			//need for reflection
			cachedListOfDataWrappers =
			    DataWrapperHelper.GetWrapperProperties<sameVM>(this);
			#endregion

			#region Create Auto Generated Property Callbacks
			//Create callbacks for auto generated properties in auto generated partial class part
			//Which allows this part to know when a property in the generated part changes
			Action personACallback = new Action(PersonAChanged);
			autoPartPropertyCallBacks.Add(personAChangeArgs.PropertyName,personACallback);

			Action personBCallback = new Action(PersonBChanged);
			autoPartPropertyCallBacks.Add(personBChangeArgs.PropertyName,personBCallback);

			#endregion
		}
		#endregion

		#region Auto Generated Property Changed CallBacks
		//Callbacks which are called whenever an auto generated property in auto generated partial class part changes
		//Which allows this part to know when a property in the generated part changes
		private void PersonAChanged()
		{
		      //You can insert code here that needs to run when the PersonA property changes
		}

		private void PersonBChanged()
		{
		      //You can insert code here that needs to run when the PersonB property changes
		}

		#endregion
		/// <summary>
		/// The current ViewMode, when changed will loop
		/// through all nested DataWrapper objects and change
		/// their state also
		/// </summary>
		static PropertyChangedEventArgs currentViewModeChangeArgs =
		    ObservableHelper.CreateArgs<sameVM>(x => x.CurrentViewMode);

		public ViewMode CurrentViewMode
		{
		    get { return currentViewMode; }
		    set
		    {
		        currentViewMode = value;
		        //Now change all the cachedListOfDataWrappers
		        //Which sets all the Cinch.DataWrapper<T>s to the correct IsEditable
		        //state based on the new ViewMode applied to the ViewModel
		        //we can use the Cinch.DataWrapperHelper class for this
		        DataWrapperHelper.SetMode(
		            cachedListOfDataWrappers,
		            currentViewMode);

		        NotifyPropertyChanged(currentViewModeChangeArgs);
		    }
		}
	}
}
