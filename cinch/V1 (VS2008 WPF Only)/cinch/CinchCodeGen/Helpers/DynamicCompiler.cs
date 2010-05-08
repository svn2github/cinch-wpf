using System.Reflection;
using System.CodeDom.Compiler;
using System.Linq;
using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.Text;


using Cinch;
using System.Windows;
using System.IO;

namespace CinchCodeGen
{
    /// <summary>
    /// Provides a method to attempt to compile the the 
    /// generated code string.
    /// Which will verify that the generated code string 
    /// is ok to be output as a generated file from this 
    /// Cinch code generator
    /// </summary>
    public static class DynamicCompiler
    {
        #region Public Methods
        /// <summary>
        /// Validates the code string that will be generated using 
        /// the compiler services available in the 
        /// <c>System.CodeDom.Compiler</c> namespace
        /// </summary>
        /// <param name="code">The code to compile as a string</param>
        /// <returns>Boolean if the code string parameter could be 
        /// compiled using the CSharpCodeProvider</returns>
        public static Boolean ComplileCodeBlock(String code)
        {
            try
            {
                var provider = new CSharpCodeProvider(
                        new Dictionary<String, String>() { { "CompilerVersion", "v3.5" } });

                CompilerParameters parameters = new CompilerParameters();

                // Start by adding any referenced assemblies
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add(typeof(ViewModelBase).Assembly.Location);
                parameters.ReferencedAssemblies.Add(typeof(System.Linq.Enumerable).Assembly.Location);
                parameters.ReferencedAssemblies.Add(typeof(System.Windows.Data.CollectionViewSource).Assembly.Location);
                parameters.ReferencedAssemblies.Add(typeof(System.Collections.Specialized.INotifyCollectionChanged).Assembly.Location);
                parameters.ReferencedAssemblies.Add(typeof(System.Collections.Generic.List<>).Assembly.Location);
                //add in any referenced assembly locations
                foreach (FileInfo refAssFile in ((App)App.Current).ReferencedAssemblies.ToList())
                    parameters.ReferencedAssemblies.Add(refAssFile.FullName);

                // Load the resulting assembly into memory
                parameters.GenerateInMemory = true;
                // Now compile the whole thing 
                //Must create a fully functional assembly as the code string
                CompilerResults compiledCode =
                    provider.CompileAssemblyFromSource(parameters, code);

                if (compiledCode.Errors.HasErrors)
                {
                    String errorMsg = String.Empty;
                    errorMsg = compiledCode.Errors.Count.ToString() +
                               " \n Dynamically generated code threw an error. \n Errors:";

                    for (int x = 0; x < compiledCode.Errors.Count; x++)
                    {
                        errorMsg = errorMsg + "\r\nLine: " +
                                   compiledCode.Errors[x].Line.ToString() + " - " +
                                   compiledCode.Errors[x].ErrorText;
                    }

                    throw new Exception(errorMsg);
                }
                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}