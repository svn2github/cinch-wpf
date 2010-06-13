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
    public static class PopupResolver
    {


        public static void ResolvePopupLookups(IEnumerable<Assembly> assembliesToExamine)
        {
            try
            {
                IUIVisualizerService uiVisualizerService  = 
                    ViewModelRepository.Instance.Resolver.Container.GetExport<IUIVisualizerService>().Value;

                foreach (Assembly ass in assembliesToExamine)
                {
                    foreach (Type type in ass.GetTypes())
                    {
                        foreach (var attrib in type.GetCustomAttributes(typeof(PopupNameToViewLookupKeyMetadataAttribute), true))
                        {
                            PopupNameToViewLookupKeyMetadataAttribute viewMetadataAtt = (PopupNameToViewLookupKeyMetadataAttribute)attrib;
                            uiVisualizerService.Register(viewMetadataAtt.PopupName, viewMetadataAtt.ViewLookupKey);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("PopupResolver is unable to ResolvePopupLookups based on current parameters", ex);
            }
        }
       
    }
}
