using System;
using System.Linq;
using System.Reflection;

namespace Conreign.Utility.Configuration
{
    public class CloudConfigurationManagerOptionsBuilder
    {
        public CloudConfigurationManagerOptionsBuilder()
        {
            Options = new CloudConfigurationManagerOptions();
        }

        public CloudConfigurationManagerOptions Options { get; }

        public CloudConfigurationManagerOptionsBuilder UseKeys(params string[] keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (keys.Any(string.IsNullOrEmpty))
            {
                throw new ArgumentException("Configuration keys should not be null.", nameof(keys));
            }
            foreach (var key in keys)
            {
                Options.SettingKeys.Add(key);
            }
            return this;
        }

        public CloudConfigurationManagerOptionsBuilder UseKeysFrom<T>()
        {
            var names = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(x => x.Name)
                .ToArray();
            return UseKeys(names);
        }
    }
}