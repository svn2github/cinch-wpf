using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinch
{
    public interface IViewStatusAwareWindowInjectionAware
    {
        void InitialiseViewAwareWindowService(IViewAwareStatusWindow viewAwareStatusServiceWindow);
    }
}
