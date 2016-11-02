using System;
using Conreign.Client.Orleans;

namespace Conreign.Api.Configuration
{
    public class ConreignApiOptions
    {
        public ConreignApiOptions(IOrleansClientInitializer orleansClientInitializer)
        {
            if (orleansClientInitializer == null)
            {
                throw new ArgumentNullException(nameof(orleansClientInitializer));
            }
            OrleansClientInitializer = orleansClientInitializer;
            Path = string.Empty;
        }

        public string Path { get; set; }
        public IOrleansClientInitializer OrleansClientInitializer { get; }
    }
}