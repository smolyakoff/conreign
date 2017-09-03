using System.IO;
using System.Text;
using Serilog;
using Serilog.Events;

namespace Conreign.Client.SignalR.Logging
{
    internal class SerilogTextWriter : TextWriter
    {
        private readonly ILogger _logger;
        private readonly string _prefix;

        public SerilogTextWriter(string prefix = "", ILogger logger = null)
        {
            _prefix = prefix;
            _logger = logger ?? Log.Logger;
            Encoding = Encoding.UTF8;
        }

        public override Encoding Encoding { get; }

        public override void WriteLine(string value)
        {
            if (_logger.IsEnabled(LogEventLevel.Verbose))
            {
                _logger.Verbose($"{_prefix}{value}");
            }
        }
    }
}