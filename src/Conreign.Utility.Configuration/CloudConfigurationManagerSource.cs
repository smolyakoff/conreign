using System;
using Microsoft.Extensions.Configuration;

namespace Conreign.Utility.Configuration
{
    public class CloudConfigurationManagerSource : IConfigurationSource
    {
        private readonly Action<CloudConfigurationManagerOptionsBuilder> _configure;

        public CloudConfigurationManagerSource(Action<CloudConfigurationManagerOptionsBuilder> configure)
        {
            _configure = configure;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CloudConfigurationManagerConfigurationProvider(_configure);
        }
    }
}