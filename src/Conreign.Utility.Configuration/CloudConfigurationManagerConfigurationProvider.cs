using System;
using Microsoft.Azure;
using Microsoft.Extensions.Configuration;

namespace Conreign.Utility.Configuration
{
    public class CloudConfigurationManagerConfigurationProvider : ConfigurationProvider
    {
        private readonly Action<CloudConfigurationManagerOptionsBuilder> _configure;
        private CloudConfigurationManagerOptions _options;

        public CloudConfigurationManagerConfigurationProvider(Action<CloudConfigurationManagerOptionsBuilder> configure)
        {
            _configure = configure ?? (c => { });
        }

        public override void Load()
        {
            var builder = new CloudConfigurationManagerOptionsBuilder();
            _configure(builder);
            _options = builder.Options;
            foreach (var key in _options.SettingKeys)
            {
                var value = CloudConfigurationManager.GetSetting(key);
                if (value == null)
                {
                    continue;
                }
                Data[key] = value;
            }
            base.Load();
        }
    }
}