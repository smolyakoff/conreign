using System.Collections.Generic;

namespace Conreign.Framework.Http.ErrorHandling
{
    public class ErrorFactorySettings
    {
        public ErrorFactorySettings()
        {
            SerializeStackTrace = true;
            SerializeExceptionErrorMessage = true;
            StatusCodeMappers = new List<IHttpErrorStatusCodeMapper> {new StaticTableHttpErrorStatusCodeMapper()};
        }

        public bool SerializeStackTrace { get; set; }

        public bool SerializeExceptionErrorMessage { get; set; }

        public List<IHttpErrorStatusCodeMapper> StatusCodeMappers { get; set; }
    }
}