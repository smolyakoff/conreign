using Microsoft.Owin.Logging;

namespace Conreign.Api.Framework.Owin
{
    public class SerilogOwinLoggerFactory : ILoggerFactory
    {
        public ILogger Create(string name)
        {
            return new SerilogOwinLogger(name);
        }
    }
}