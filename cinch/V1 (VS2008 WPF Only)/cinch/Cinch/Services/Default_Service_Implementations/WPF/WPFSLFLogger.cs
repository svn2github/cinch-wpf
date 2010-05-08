using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Slf;

namespace Cinch
{
    /// <summary>
    /// A SimpleLoggerFacade based error logger.
    /// This is what Cinch uses by default, you can override
    /// this by supplying a new Cinch.ILogger based service
    /// </summary>
    public class WPFSLFLogger : Cinch.ILogger
    {
        #region Data
        private static Slf.ILogger logger = null;
        #endregion

        #region Ctor
        static WPFSLFLogger()
        {
            logger = LoggerService.GetLogger();
        }
        #endregion

        #region ILogger Members


        public void Error(Exception exception)
        {
            logger.Error(exception);
        }

        public void Error(object obj)
        {
            logger.Error(obj);
        }

        public void Error(string message)
        {
            logger.Error(message);
        }

        public void Error(Exception exception, string message)
        {
            logger.Error(exception, message);
        }

        public void Error(string format, params object[] args)
        {
            logger.Error(format, args);
        }

        public void Error(Exception exception, string format, params object[] args)
        {
            logger.Error(exception, format, args);
        }

        public void Error(IFormatProvider provider, string format, params object[] args)
        {
            logger.Error(provider, format, args);
        }

        public void Error(Exception exception, string format, IFormatProvider provider, params object[] args)
        {
            logger.Error(exception, format, provider, args);
        }


        

        #endregion
    }
}
