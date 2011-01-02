using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinch
{
    public interface ICinchDisposable
    {
        // Summary:
        //     Performs Cinch related resource cleaning, such as unhooking
        //     Mediator message registration, etc etc
        void Dispose();
    }
}
