using System.IO;
using System.Text;
using Serilog;

namespace Conreign.Client.SignalR
{
    public class SerilogTextWriter : TextWriter
    {
        private readonly ILogger _logger;

        public SerilogTextWriter(ILogger logger)
        {
            _logger = logger;
        }

        public override Encoding Encoding { get; }


        public override void WriteLine(string value)
        {
            _logger.Verbose(value);
        }
    }
}