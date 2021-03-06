﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Windows;
using Cinch;
using System.Reflection;


namespace CinchCodeGen
{



    /// <summary>
    /// Provides public methods to allow the current InMemoryViewModel to be serialized to and from
    /// an XML file on disk
    /// </summary>
    public static class ViewModelPersistence
    {
        #region Data
        private static IMessageBoxService MessageBoxService =
                    ViewModelBase.ServiceProvider.Resolve<IMessageBoxService>();
        #endregion

        #region Public Methods
        /// <summary>
        /// Serializes a PesistentVM to disk, in a XML file format
        /// </summary>
        /// <param name="fileName">The file name of the ViewModel to save</param>
        /// <param name="vmToPersist">The actual serializable ViewModel</param>
        /// <returns>True if the save operation succeeds</returns>
        public static Boolean PersistViewModel(String fileName, PesistentVM vmToPersist)
        {
            try
            {
                FileInfo file = new FileInfo(fileName);
                if (!file.Extension.Equals(".xml"))
                    throw new NotSupportedException(
                        String.Format("The file name {0} you picked is not supported\r\n\r\nOnly .xml files are valid",
                        file.Name));

                if (vmToPersist == null)
                    throw new NotSupportedException("The ViewModel is null");

                
                //write the file to disk
                XmlSerializer serializer = new XmlSerializer(typeof(PesistentVM));

                using (TextWriter writer = new StreamWriter(file.FullName))
                {
                    serializer.Serialize(writer, vmToPersist);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Strips all extension off a file name
        /// </summary>
        /// <param name="wholeFileName">The entire file name</param>
        /// <returns>A stripped filename string</returns>
        public static String StripFileName(String wholeFileName)
        {
            String justFilePart=wholeFileName;
            do
            {
                justFilePart =justFilePart.LastIndexOf(".") > 0 ?
                justFilePart.Substring(0, justFilePart.LastIndexOf("."))
                : justFilePart;

            }while (justFilePart.LastIndexOf(".") > 0);

            return justFilePart;
        }


        /// <summary>
        /// Creates the ViewModel code in C# format
        /// </summary>
        /// <param name="fileName">The file name of the ViewModel to save</param>
        /// <param name="vmToPersist">The actual serializable ViewModel</param>
        /// <returns>True if the save operation succeeds</returns>
        public static Boolean CreateViewModelCode(String fileName, PesistentVM vmToPersist)
        {
            try
            {
                if (vmToPersist == null)
                    throw new NotSupportedException("The ViewModel is null");

                
                FileInfo file = new FileInfo(fileName);
                String allCode = CreateAllCodeParts(vmToPersist);
                Tuple<Boolean,String> compilationSucceeded = CompileGeneratedCode(allCode);

                
                //check for .g.cs
                String justFilePart = StripFileName(fileName);

                //Write the Auto generated file part
                String autoGeneratedFileName = String.Format("{0}.g.cs", justFilePart);
                String autoGeneratedFilePath = Path.Combine(file.Directory.FullName, autoGeneratedFileName);
                String autoGenPartContents = CreateAutoGeneratedPart(vmToPersist, true);
  
                //No need to carry on if user says don't generate 1st part
                if (!SaveGeneratedCode(compilationSucceeded.First,compilationSucceeded.Second,
                    autoGeneratedFileName, autoGenPartContents))
                    return false;


                //Write the Custom file part
                String customFileName = String.Format("{0}.cs", justFilePart);
                String customFilePath = Path.Combine(file.Directory.FullName, customFileName);
                String customPartContents = CreateCustomPart(vmToPersist, true);
                return SaveGeneratedCode(compilationSucceeded.First, compilationSucceeded.Second, 
                    customFileName, customPartContents);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// DeSerializes an XML file into a PesistentVM 
        /// (if the xml is of the correct formatting)
        /// </summary>
        /// <param name="fileName">The file name of the ViewModel to open</param>
        /// <returns>The XML read PesistentVM, or null if it can't be read</returns>
        public static PesistentVM HydratePersistedViewModel(String fileName)
        {
            try
            {
                FileInfo file = new FileInfo(fileName);
                if (!file.Extension.Equals(".xml"))
                    throw new NotSupportedException(
                        String.Format("The file name {0} you picked is not supported\r\n\r\nOnly .xml files are valid",
                        file.Name));

                //read the file from disk
                XmlSerializer serializer = new XmlSerializer(typeof(PesistentVM));
                serializer.UnknownNode += Serializer_UnknownNode;
                serializer.UnknownAttribute += Serializer_UnknownAttribute;
                PesistentVM vmToHydrate = null;

                using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
                {
                    vmToHydrate = (PesistentVM)serializer.Deserialize(fs);
                }
                return vmToHydrate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Compiles and generates the code, and returns a tuple of success/failure and the
        /// generated filename
        /// </summary>
        /// <param name="code">The code to compile</param>
        private static Tuple<Boolean, String> CompileGeneratedCode(String code)
        {
            bool succeeded = false;

            try
            {
                succeeded = DynamicCompiler.ComplileCodeBlock(code);
                return TupleHelper.New(true, String.Empty);
            }
            catch (Exception ex)
            {
                return TupleHelper.New(false, ex.Message);
            }
        }

        /// <summary>
        /// If the compilationSucceeded is false, the user is asks if they would 
        /// still like to generate the code anyway, and if they say yes the code
        /// is generated. If the compilationSucceeded is true, the code is simply
        /// generated. In either case this method returns true if the operation was
        /// successful.
        /// </summary>
        private static Boolean SaveGeneratedCode(Boolean compilationSucceeded,
            String compilationErrorMsg, String fileName, String code)
        {
            if (!compilationSucceeded)
            {
                if (MessageBoxService.ShowYesNo(
                    "There was an issue with the code file\r\n" +
                    compilationErrorMsg + "\r\n\r\n" +
                    "But this could be due to a missing import required by one of the properties\r\n" +
                    "Generate anyway?", CustomDialogIcons.Question) == CustomDialogResults.Yes)
                {
                    using (TextWriter writer = new StreamWriter(fileName))
                        writer.Write(code);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                using (TextWriter writer = new StreamWriter(fileName))
                    writer.Write(code);
                return true;
            }
        }

        /// <summary>
        /// Creates and returns a single generated code String which has both parts
        /// of the partial class, wcich can then be sent to the compilation phase
        /// as a single file. The single code string is generated based on the vmToPersist
        /// parameter
        /// </summary>
        private static String CreateAllCodeParts(PesistentVM vmToPersist)
        {
            //The order is important, do not change the order here
            //otherwise it will not work
            String autoPart = CreateAutoGeneratedPart(vmToPersist, false);
            String customPart = CreateCustomPart(vmToPersist, false);

            StringBuilder sb = new StringBuilder(1000);
            sb.Append(autoPart);
            sb.Append(customPart);
            sb.AppendLine("}");

            return sb.ToString();

        }


        /// <summary>
        /// Creates and returns an auto generated part String, based on the vmToPersist
        /// parameter. The returned string will form tne auto generated part (.g.cs)
        /// </summary>
        private static String CreateAutoGeneratedPart(PesistentVM vmToPersist, Boolean shouldEmit)
        {
            StringBuilder sb = new StringBuilder(1000);

            //Write out namespaces
            #region Namespaces
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.ComponentModel;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine("using System.Windows.Data;");
            sb.AppendLine("using System.Collections.Specialized;");

            //write out referenced assemblies Using statements
            List<String> refAssUsingStments =
                ReferencedAssembliesHelper.GetReferencedAssembliesUsingStatements();
            if (refAssUsingStments.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("//Referenced assemblies");
                foreach (String usingStmnt in refAssUsingStments)
                {
                    sb.AppendLine(usingStmnt);
                }
                sb.AppendLine();
            }
            sb.AppendLine("using Cinch;");
            sb.AppendLine();
            #endregion

            #region Class code
            sb.AppendLine(String.Format("namespace {0}", vmToPersist.VMNamespace));
            sb.AppendLine("{");

            sb.AppendLine("\t/// <summary>");
            sb.AppendLine("\t///NOTE : This class was auto generated by a tool");
            sb.AppendLine("\t///Edit this code at your peril!!!!!!");
            sb.AppendLine("\t/// </summary>");
            
            sb.AppendLine(String.Format("\tpublic partial class {0} : Cinch.{1}",
                vmToPersist.VMName, vmToPersist.InheritenceVMType.ToString()));
            sb.AppendLine("\t{");

            //write Data
            #region Data
            sb.AppendLine("\t\t#region Data");
            foreach (var prop in vmToPersist.VMProperties)
                sb.AppendLine(prop.FieldDeclaration);
            sb.AppendLine("\t\t//callbacks to allow auto generated part to notify custom part, when property changes");
            sb.AppendLine("\t\tprivate Dictionary<String, Action> autoPartPropertyCallBacks = new Dictionary<String, Action>();");
            sb.AppendLine("\t\t#endregion");
            sb.AppendLine("\r\n");
            #endregion

            //write get/set declarations
            #region Props
            sb.AppendLine("\t\t#region Public Properties");
            foreach (var prop in vmToPersist.VMProperties)
                sb.AppendLine(prop.PropGetSet);
            sb.AppendLine("\t\t#endregion");
            #endregion

            #region EditableValidatingObject overrides

            var propsWithWrappers = 
                vmToPersist.VMProperties.Where(x => x.UseDataWrapper == true);


            //EditableValidatingObject overrides
            if (vmToPersist.VMType == ViewModelType.ValidatingAndEditable
                && propsWithWrappers.Count() > 0)
            {
                sb.AppendLine("\t\t#region EditableValidatingObject overrides");

                sb.AppendLine("\t\t/// <summary>");
                sb.AppendLine("\t\t/// Override hook which allows us to also put any child");
                sb.AppendLine("\t\t/// EditableValidatingObject objects into the BeginEdit state");
                sb.AppendLine("\t\t/// </summary>");
                sb.AppendLine("\t\tprotected override void OnBeginEdit()");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t    base.OnBeginEdit();");
                sb.AppendLine("\t\t    //Now walk the list of properties in the ViewModel");
                sb.AppendLine("\t\t    //and call BeginEdit() on all Cinch.DataWrapper<T>s.");
                sb.AppendLine("\t\t    //we can use the Cinch.DataWrapperHelper class for this");
                sb.AppendLine("\t\t    DataWrapperHelper.SetBeginEdit(cachedListOfDataWrappers);");
                sb.AppendLine("\t\t}");
                sb.AppendLine();
                sb.AppendLine("\t\t/// <summary>");
                sb.AppendLine("\t\t/// Override hook which allows us to also put any child");
                sb.AppendLine("\t\t/// EditableValidatingObject objects into the EndEdit state");
                sb.AppendLine("\t\t/// </summary>");
                sb.AppendLine("\t\tprotected override void OnEndEdit()");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t    base.OnEndEdit();");
                sb.AppendLine("\t\t    //Now walk the list of properties in the ViewModel");
                sb.AppendLine("\t\t    //and call CancelEdit() on all Cinch.DataWrapper<T>s.");
                sb.AppendLine("\t\t    //we can use the Cinch.DataWrapperHelper class for this");
                sb.AppendLine("\t\t    DataWrapperHelper.SetEndEdit(cachedListOfDataWrappers);");
                sb.AppendLine("\t\t}");
                sb.AppendLine();
                sb.AppendLine("\t\t/// <summary>");
                sb.AppendLine("\t\t/// Override hook which allows us to also put any child ");
                sb.AppendLine("\t\t/// EditableValidatingObject objects into the CancelEdit state");
                sb.AppendLine("\t\t/// </summary>");
                sb.AppendLine("\t\tprotected override void OnCancelEdit()");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t    base.OnCancelEdit();");
                sb.AppendLine("\t\t    //Now walk the list of properties in the ViewModel");
                sb.AppendLine("\t\t    //and call CancelEdit() on all Cinch.DataWrapper<T>s.");
                sb.AppendLine("\t\t    //we can use the Cinch.DataWrapperHelper class for this");
                sb.AppendLine("\t\t    DataWrapperHelper.SetCancelEdit(cachedListOfDataWrappers);");
                sb.AppendLine("\t\t}");
                sb.AppendLine("\t\t#endregion");
            }
            #endregion


            #endregion

            sb.AppendLine("\t}");

            if (shouldEmit)
                sb.AppendLine("}");

            return sb.ToString();
        }


        /// <summary>
        /// Creates and returns an custom generated part String, based on the vmToPersist
        /// parameter. The returned string will form tne auto generated part (.cs)
        /// </summary>
        private static String CreateCustomPart(PesistentVM vmToPersist, Boolean shouldEmit)
        {
            StringBuilder sb = new StringBuilder(1000);

            //write out namespaces
            #region Namespaces
            if (shouldEmit)
            {
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using System.ComponentModel;");
                sb.AppendLine("using System.Collections.ObjectModel;");
                sb.AppendLine("using System.Windows.Data;");
                sb.AppendLine("using System.Collections.Specialized;");

                //write out referenced assemblies Using statements
                List<String> refAssUsingStments =
                    ReferencedAssembliesHelper.GetReferencedAssembliesUsingStatements();
                if (refAssUsingStments.Count > 0)
                {
                    sb.AppendLine("//Referenced assemblies");
                    foreach (String usingStmnt in refAssUsingStments)
                    {
                        sb.AppendLine(usingStmnt);
                    }
                    sb.AppendLine();
                }
                sb.AppendLine("using Cinch;");
                sb.AppendLine();

                sb.AppendLine(String.Format("namespace {0}", vmToPersist.VMNamespace));
                sb.AppendLine("{");
            }
            #endregion

            //Write out data
            #region Data
            sb.AppendLine("\t/// <summary>");
            sb.AppendLine("\t///You may edit this code by hand, but there is DataWrapper code");
            sb.AppendLine("\t///and some boiler plate code provided here, to help you on your way.");
            sb.AppendLine("\t///A lot of which is actually quite useful, and a lot of thought has been");
            sb.AppendLine("\t///put into, what code to place in which file parts, and this custom part");
            sb.AppendLine("\t///does have some excellent starting code, so use it as you wish.");
            sb.AppendLine("\t///");
            sb.AppendLine("\t///But please note : One area that will need to be examined closely if you decide to introduce");
            sb.AppendLine("\t///New DataWrapper<T> properties in this part, is the IsValid override");
            sb.AppendLine("\t///Which will need to include the dataWrappers something like:");
            sb.AppendLine("\t///<pre>");
            sb.AppendLine("\t///       return base.IsValid &&");
            sb.AppendLine("\t///          DataWrapperHelper.AllValid(cachedListOfDataWrappers);");
            sb.AppendLine("\t///</pre>");
            sb.AppendLine("\t/// </summary>");


            sb.AppendLine(String.Format("\tpublic partial class {0}", vmToPersist.VMName));
            sb.AppendLine("\t{");


            //write fields
            var propsWithWrappers = vmToPersist.VMProperties.Where(x => x.UseDataWrapper == true);
            if (propsWithWrappers.Count() > 0)
            {
                sb.AppendLine("\t\t#region Data");
                sb.AppendLine("\t\tprivate IEnumerable<DataWrapperBase> cachedListOfDataWrappers;");
                sb.AppendLine("\t\tprivate ViewMode currentViewMode = ViewMode.AddMode;");
                sb.AppendLine("\t\t//Example rule declaration : YOU WILL NEED TO DO THIS BIT");
                sb.AppendLine("\t\t//private static SimpleRule quantityRule;");
                sb.AppendLine("\t\t#endregion");
            }
            sb.AppendLine();
            #endregion

            //Ctor
            #region Ctor
            sb.AppendLine("\t\t#region Ctor");
            sb.AppendLine(String.Format("\t\tpublic {0}()", vmToPersist.VMName));
            sb.AppendLine("\t\t{");


            //write out auto generated part callbacks
            sb.AppendLine();
            sb.AppendLine("\t\t\t#region Create Auto Generated Property Callbacks");
            sb.AppendLine("\t\t\t//Create callbacks for auto generated properties in auto generated partial class part");
            sb.AppendLine("\t\t\t//Which allows this part to know when a property in the generated part changes");
            foreach (var prop in vmToPersist.VMProperties)
            {
                sb.AppendLine(prop.CustomPartCtorCallBack);
            }
            sb.AppendLine("\t\t\t#endregion");



            //write out wrappers if they are needed
            if (propsWithWrappers.Count() > 0)
            {
                sb.AppendLine("\t\t\t#region Create DataWrappers");
                foreach (var propWithWrapper in propsWithWrappers)
                    sb.AppendLine(propWithWrapper.ConstructorDeclaration);

                sb.AppendLine("\t\t\t//fetch list of all DataWrappers, so they can be used again later without the");
                sb.AppendLine("\t\t\t//need for reflection");
                sb.AppendLine("\t\t\tcachedListOfDataWrappers =");
                sb.AppendLine(String.Format(
                    "\t\t\t    DataWrapperHelper.GetWrapperProperties<{0}>(this);", vmToPersist.VMName));
                sb.AppendLine("\t\t\t#endregion");


            }


           


            //write out example validation, if this ViewModel type supports them
            if (vmToPersist.VMType != ViewModelType.Standard)
            {
                sb.AppendLine();
                sb.AppendLine("\t\t\t//    #region TODO : You WILL need to create YOUR OWN validation rules");
                sb.AppendLine("\t\t\t//    //Here is an example of how to create a validation rule");
                sb.AppendLine("\t\t\t//    //you can use this as a guide to create your own validation rules");
                sb.AppendLine("\t\t\t//    quantity.AddRule(quantityRule);");
                sb.AppendLine("\t\t\t//    #endregion");
            }

            sb.AppendLine("\t\t}");



            //write out static Constructor
            sb.AppendLine();
            sb.AppendLine(String.Format("\t\tstatic {0}()", vmToPersist.VMName));
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\t//quantityRule = new SimpleRule(\"DataValue\", \"Quantity can not be < 0\",");
            sb.AppendLine("\t\t\t//          (Object domainObject)=>");
            sb.AppendLine("\t\t\t//          {");
            sb.AppendLine("\t\t\t//              DataWrapper<Int32> obj = (DataWrapper<Int32>)domainObject;");
            sb.AppendLine("\t\t\t//              return obj.DataValue <= 0;");
            sb.AppendLine("\t\t\t//          });");
            sb.AppendLine("\t\t}");

            sb.AppendLine("\t\t#endregion");
            #endregion

            //Write out actual auto generated property changed callback stubs
            #region Property Changed Stubs
            sb.AppendLine();
            sb.AppendLine("\t\t#region Auto Generated Property Changed CallBacks");
            sb.AppendLine("\t\t//Callbacks which are called whenever an auto generated property in auto generated partial class part changes");
            sb.AppendLine("\t\t//Which allows this part to know when a property in the generated part changes");
            foreach (var prop in vmToPersist.VMProperties)
            {
                sb.AppendLine(prop.CustomPartActualCallBack);
            }

            sb.AppendLine("\t\t#endregion");
            #endregion

            //region ViewMode property
            #region ViewMode property (which allows all DataWrappers to be put into a specific ViewMode state)
            if (propsWithWrappers.Count() > 0)
            {
                sb.AppendLine("\t\t/// <summary>");
                sb.AppendLine("\t\t/// The current ViewMode, when changed will loop");
                sb.AppendLine("\t\t/// through all nested DataWrapper objects and change");
                sb.AppendLine("\t\t/// their state also");
                sb.AppendLine("\t\t/// </summary>");
                sb.AppendLine("\t\tstatic PropertyChangedEventArgs currentViewModeChangeArgs =");
                sb.AppendLine(String.Format(
                    "\t\t    ObservableHelper.CreateArgs<{0}>(x => x.CurrentViewMode);", 
                    vmToPersist.VMName));
                sb.AppendLine();
                sb.AppendLine("\t\tpublic ViewMode CurrentViewMode");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t    get { return currentViewMode; }");
                sb.AppendLine("\t\t    set");
                sb.AppendLine("\t\t    {");
                sb.AppendLine("\t\t        currentViewMode = value;");
                sb.AppendLine("\t\t        //Now change all the cachedListOfDataWrappers");
                sb.AppendLine("\t\t        //Which sets all the Cinch.DataWrapper<T>s to the correct IsEditable");
                sb.AppendLine("\t\t        //state based on the new ViewMode applied to the ViewModel");
                sb.AppendLine("\t\t        //we can use the Cinch.DataWrapperHelper class for this");
                sb.AppendLine("\t\t        DataWrapperHelper.SetMode(");
                sb.AppendLine("\t\t            cachedListOfDataWrappers,");
                sb.AppendLine("\t\t            currentViewMode);");
                sb.AppendLine();
                sb.AppendLine("\t\t        NotifyPropertyChanged(currentViewModeChangeArgs);");
                sb.AppendLine("\t\t    }");
                sb.AppendLine("\t\t}");
            }
            #endregion


            //Overrides (IsValid)
            #region Overrides
            if (vmToPersist.VMType != ViewModelType.Standard)
            {
                sb.AppendLine();
                sb.AppendLine("\t\t#region Overrides");
                sb.AppendLine("\t\t/// <summary>");
                sb.AppendLine("\t\t/// Override hook which allows us to also put any child ");
                sb.AppendLine("\t\t/// EditableValidatingObject objects IsValid state into");
                sb.AppendLine("\t\t/// a combined IsValid state for the whole ViewModel,");
                sb.AppendLine("\t\t/// should you need to do so");
                sb.AppendLine("\t\t/// </summary>");
                sb.AppendLine("\t\tpublic override bool IsValid");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t    get");
                sb.AppendLine("\t\t    {");
                if (propsWithWrappers.Count() > 0)
                {
                    sb.AppendLine("\t\t       //return base.IsValid and use DataWrapperHelper, as you are");
                    sb.AppendLine("\t\t       //using DataWrappers");
                    sb.AppendLine("\t\t       return base.IsValid &&");
                    sb.AppendLine("\t\t          DataWrapperHelper.AllValid(cachedListOfDataWrappers);");
                }
                else
                {
                    sb.AppendLine("\t\t       //simply return base.IsValid as you are not");
                    sb.AppendLine("\t\t       //using DataWrappers");
                    sb.AppendLine("\t\t       return base.IsValid;");
                }
                sb.AppendLine("\t\t    }");
                sb.AppendLine("\t\t}");
                sb.AppendLine("\t\t#endregion");
            }
            #endregion

            sb.AppendLine("\t}");
            if (shouldEmit)
                sb.AppendLine("}");

            return sb.ToString();
        }


        /// <summary>
        /// Fault handling for reading an unknown node from the specified Xml file
        /// </summary>
        private static void Serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            MessageBox.Show(String.Format("Unknown Node: {0} \t {1}", e.Name, e.Text), 
                "Error reading file",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Fault handling for reading an unknown attribute from the specified Xml file
        /// </summary>
        private static void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            MessageBox.Show(String.Format("Unknown attribute: {0} ='{1}'", e.Attr.Name, e.Attr.Value),
                "Error reading file",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion
    }


    /// <summary>
    /// Represents a light weight persistable ViewModel
    /// </summary>
    public class PesistentVM
    {
        #region Ctor
        public PesistentVM()
        {
            VMProperties = new List<PesistentVMSingleProperty>();
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// ViewModel type
        /// </summary>
        public String InheritenceVMType 
        {
            get
            {
                switch (VMType)
                {
                    case ViewModelType.Standard:
                        return "ViewModelBase";
                    case ViewModelType.Validating:
                        return "ValidatingViewModelBase"; 
                    case ViewModelType.ValidatingAndEditable:
                        return "EditableValidatingViewModelBase"; 
                    default:
                        return "ViewModelBase";
                }
            }
        }

        /// <summary>
        /// ViewModel type
        /// </summary>
        public ViewModelType VMType { get; set; }

        /// <summary>
        /// Viewmodel name
        /// </summary>
        public String VMName { get; set; }

        /// <summary>
        /// ViewModel namespace
        /// </summary>
        public String VMNamespace { get; set; }

        /// <summary>
        /// Nested properties
        /// </summary>
        public List<PesistentVMSingleProperty> VMProperties { get; set; }
        #endregion
    }

    /// <summary>
    /// Represents a light weight persistable ViewModel property
    /// </summary>
    public class PesistentVMSingleProperty
    {
        #region Private Properties
        /// <summary>
        /// Gets lower cased property name
        /// </summary>
        private String LowerPropName
        {
            get
            {
                return this.PropName.Substring(0, 1).ToLower() +
                    this.PropName.Substring(1);
            }
        }

        /// <summary>
        /// Gets upper cased property name
        /// </summary>
        private String UpperPropName
        {
            get
            {
                return this.PropName.Substring(0, 1).ToUpper() +
                    this.PropName.Substring(1);
            }
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Property name
        /// </summary>
        public String PropName { get; set; }

        /// <summary>
        /// Property type, Double, Int32 etc etc
        /// </summary>
        public String PropertyType { get; set; }

        /// <summary>
        /// Use a DataWrapper
        /// </summary>
        public Boolean UseDataWrapper { get; set; }

        /// <summary>
        /// Parent ViewModel name
        /// </summary>
        public String ParentViewModelName { get; set; }

        /// <summary>
        /// Generated constructor declaration string
        /// </summary>
        public String ConstructorDeclaration
        {
            get
            {   
                #region Example text generated
                //HomePhoneNumber = new DataWrapper<String>(this, homePhoneNumberChangeArgs);
                #endregion

                return
                UseDataWrapper ?
                String.Format("\t\t\t{0} = new Cinch.DataWrapper<{1}>(this,{2}ChangeArgs, {2}Callback);",
                    UpperPropName, PropertyType, LowerPropName) :
                String.Empty;
            }
        }


        /// <summary>
        /// Generated field declaration string
        /// </summary>
        public String FieldDeclaration
        {
            get
            {
                #region Example text generated
                //private Cinch.DataWrapper<String> homePhoneNumber;
                //private String homePhoneNumber;
                #endregion

                return
                UseDataWrapper ?
                String.Format("\t\tprivate Cinch.DataWrapper<{0}> {1};", PropertyType, LowerPropName) :
                String.Format("\t\tprivate {0} {1};", PropertyType, LowerPropName);
            }
        }


        /// <summary>
        /// Provides the Custom part Ctor Action callback strings
        /// </summary>
        public String CustomPartCtorCallBack
        {
            #region Example Text Generated
            //Action firstNameCallBack = new Action(FistNameChanged);
            //autoPartPropertyCallBacks.Add(firstNameChangeArgs.PropertyName,firstNameCallBack);
            #endregion

            get
            {
                StringBuilder sb = new StringBuilder(200);
                sb.AppendLine(String.Format("\t\t\tAction {0}Callback = new Action({1}Changed);", LowerPropName, UpperPropName));
                sb.AppendLine(String.Format("\t\t\tautoPartPropertyCallBacks.Add({0}ChangeArgs.PropertyName,{0}Callback);", LowerPropName));
                return sb.ToString();
            }
        }


        /// <summary>
        /// Provides the Custom part Ctor Action callback strings
        /// </summary>
        public String CustomPartActualCallBack
        {
            #region Example Text Generated
            //private void FistNameChanged()
            //{
            //      //You can insert code here that needs to run when the FistName property changes
            //}
            #endregion

            get
            {
                StringBuilder sb = new StringBuilder(200);
                sb.AppendLine(String.Format("\t\tprivate void {0}Changed()", UpperPropName));
                sb.AppendLine("\t\t{");
                sb.AppendLine(String.Format("\t\t      //You can insert code here that needs to run when the {0} property changes", UpperPropName));
                sb.AppendLine("\t\t}");
                return sb.ToString();
            }
        }


        /// <summary>
        /// Property Get/Set string
        /// </summary>
        public String PropGetSet
        {
            get
            {
                 #region Example text generated
                ///// <summary>
                ///// FirstName
                ///// </summary>
                //static PropertyChangedEventArgs firstNameChangeArgs =
                //  ObservableHelper.CreateArgs<CustomerModel>(x => x.FirstName);

                //public Cinch.DataWrapper<String> FirstName
                //{
                //    get { return firstName; }
                //    set
                //    {
                //        firstName = value;
                //        NotifyPropertyChanged(firstNameChangeArgs);
                //    }
                //}
                 #endregion

                StringBuilder sb = new StringBuilder(1000);
                sb.AppendLine(String.Format("\t\t#region {0}", UpperPropName));
                sb.AppendLine(String.Format("\t\t/// <summary>"));
                sb.AppendLine(String.Format("\t\t/// {0}", UpperPropName));
                sb.AppendLine(String.Format("\t\t/// </summary>"));
                sb.AppendLine(String.Format("\t\tstatic PropertyChangedEventArgs {0}ChangeArgs =",
                    LowerPropName));
                sb.AppendLine(String.Format("\t\t\tObservableHelper.CreateArgs<{0}>(x => x.{1});",
                     ParentViewModelName, UpperPropName));
                sb.AppendLine("\r\n");
                sb.AppendLine(String.Format("\t\tpublic {0} {1}",
                    UseDataWrapper ? String.Format("Cinch.DataWrapper<{0}>", PropertyType) : PropertyType,
                    UpperPropName));
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t\tget { return " + LowerPropName +"; }");
                sb.AppendLine(UseDataWrapper ? "\t\t\tprivate set" : "\t\t\tset");
                sb.AppendLine("\t\t\t{");
                sb.AppendLine(String.Format("\t\t\t\t{0} = value;",LowerPropName));
                sb.AppendLine(String.Format("\t\t\t\tNotifyPropertyChanged({0}ChangeArgs);", LowerPropName));
                sb.AppendLine("\t\t\t\t//Use callback to provide non auto generated part of partial");
                sb.AppendLine("\t\t\t\t//class with notification, when an auto generated property value changes");
                sb.AppendLine("\t\t\t\tAction callback = null;");
                sb.AppendLine("\t\t\t\tif (autoPartPropertyCallBacks.TryGetValue(");
                sb.AppendLine(String.Format("\t\t\t\t    {0}ChangeArgs.PropertyName, out callback))", LowerPropName));
                sb.AppendLine("\t\t\t\t{");
                sb.AppendLine("\t\t\t\t    callback();");
                sb.AppendLine("\t\t\t\t}");
                sb.AppendLine("\t\t\t}");
                sb.AppendLine("\t\t}");
                sb.AppendLine("\t\t#endregion");
                return sb.ToString();
            }
        }



        #endregion
    }
 
}
