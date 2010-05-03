using System;


namespace Cinch
{


    /// <summary>
    /// This interface defines a interface that will allow 
    /// a ViewModel to output a Logmessage to whatever format 
    /// the consumer specifies when they provide a ILogger based
    /// service. Note Cinch supplies a SLF default ILogger based
    /// service, but a new one can be injected using any IOC mechanism
    /// you like
    /// </summary>
    public interface ILogger
    {
        void Error(Exception exception);
        void Error(object obj);
        void Error(string message);
        void Error(Exception exception, string message);
        void Error(string format, params object[] args);
        void Error(Exception exception, string format, params object[] args);
        void Error(IFormatProvider provider, string format, params object[] args);
        void Error(Exception exception, string format, IFormatProvider provider, params object[] args);
    }
}
