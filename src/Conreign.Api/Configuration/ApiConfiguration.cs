using System.Web.Http;

namespace Conreign.Api.Configuration
{
    public static class ApiConfiguration
    {
        public static HttpConfiguration CreateHttpConfiguration()
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            return config;
        }
    }
}
