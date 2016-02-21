using MediatR;
using Newtonsoft.Json;

namespace Conreign.Api.Framework.Owin
{
    public class FrameworkOptions
    {
        public FrameworkOptions()
        {
            Debug = true;
            SerializerSettings = new JsonSerializerSettings();
        }

        public bool Debug { get; set; }

        public JsonSerializerSettings SerializerSettings { get; set; }

        public SingleInstanceFactory ObjectFactory { get; set; }
    }
}