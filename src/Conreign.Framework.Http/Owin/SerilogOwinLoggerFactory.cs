using Microsoft.Owin.Logging;

namespace Conreign.Framework.Http.Owin
{
    public class SerilogOwinLoggerFactory : ILoggerFactory
    {
        public ILogger Create(string name)
        {
            return new SerilogOwinLogger(name);
        }
    }
}