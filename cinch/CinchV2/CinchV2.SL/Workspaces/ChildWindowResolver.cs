using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Reflection;
using MEFedMVVM.ViewModelLocator;

namespace Cinch
{
    public static class ChildWindowResolver
    {


        public static void ResolveChildWindowLookups(IEnumerable<Assembly> assembliesToExamine)
        {
            try
            {
                IChildWindowService childWindowService =
                    ViewModelRepository.Instance.Resolver.Container.GetExport<IChildWindowService>().Value;

                foreach (Assembly ass in assembliesToExamine)
                {
                    foreach (Type type in ass.GetTypes())
                    {
                        foreach (var attrib in type.GetCustomAttributes(typeof(PopupNameToViewLookupKeyMetadataAttribute), true))
                        {
                            PopupNameToViewLookupKeyMetadataAttribute viewMetadataAtt = (PopupNameToViewLookupKeyMetadataAttribute)attrib;
                            childWindowService.Register(viewMetadataAtt.PopupName, viewMetadataAtt.ViewLookupKey);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ChildWindowResolver is unable to ResolveChildWindowLookups based on current parameters", ex);
            }
        }

    }
}
