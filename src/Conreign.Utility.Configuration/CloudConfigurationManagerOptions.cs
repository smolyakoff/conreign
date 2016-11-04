using System.Collections.Generic;

namespace Conreign.Utility.Configuration
{
    public class CloudConfigurationManagerOptions
    {
        public CloudConfigurationManagerOptions()
        {
            SettingKeys = new HashSet<string>();
        }

        public HashSet<string> SettingKeys { get; }
    }
}