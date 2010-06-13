using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;



namespace Cinch
{
    public static class CinchBootStrapper
    {
        public static void Initialise(IEnumerable<Assembly> assembliesToExamine)
        {
            try
            {
                //now pass the same Assemblies to the ViewResolver so it can
                //resolve the childWindows
                ChildWindowResolver.ResolveChildWindowLookups(assembliesToExamine);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Bootstrapper.Initialise() failed", ex);
            }
        }
    }
}
