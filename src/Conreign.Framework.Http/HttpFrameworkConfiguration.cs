using Conreign.Framework.Http.Core;
using Conreign.Framework.Http.ErrorHandling;
using Newtonsoft.Json;

namespace Conreign.Framework.Http
{
    public class HttpFrameworkConfiguration
    {
        public HttpFrameworkConfiguration()
        {
            SerializerSettings = new JsonSerializerSettings();
            ErrorFactorySettings = new ErrorFactorySettings();
            EventHubSettings = new EventHubSettings();
        }

        public JsonSerializerSettings SerializerSettings { get; set; }

        public ErrorFactorySettings ErrorFactorySettings { get; set; }

        public EventHubSettings EventHubSettings { get; set; }
    }
}