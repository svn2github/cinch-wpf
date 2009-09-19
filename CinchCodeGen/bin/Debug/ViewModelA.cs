using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections.Specialized;
using Cinch;

namespace ViewModels
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
	public partial class ViewModelA
	{

		#region Ctor
		public ViewModelA()
		{

			#region Create Auto Generated Property Callbacks
			//Create callbacks for auto generated properties in auto generated partial class part
			//Which allows this part to know when a property in the generated part changes
			Action counterCallback = new Action(CounterChanged);
			autoPartPropertyCallBacks.Add(counterChangeArgs.PropertyName,counterCallback);

			#endregion
		}
		#endregion

		#region Auto Generated Property Changed CallBacks
		//Callbacks which are called whenever an auto generated property in auto generated partial class part changes
		//Which allows this part to know when a property in the generated part changes
		private void CounterChanged()
		{
		      //You can insert code here that needs to run when the Counter property changes
		}

		#endregion
	}
}
