using System;
using Microsoft.Extensions.Configuration;

namespace Conreign.Utility.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddCloudConfiguration(
            this IConfigurationBuilder builder,
            Action<CloudConfigurationManagerOptionsBuilder> configure = null)
        {
            return builder.Add(new CloudConfigurationManagerSource(configure));
        }
    }
}