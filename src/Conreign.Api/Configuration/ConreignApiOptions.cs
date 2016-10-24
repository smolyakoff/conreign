namespace Conreign.Api.Configuration
{
    public class ConreignApiOptions
    {
        public ConreignApiOptions()
        {
            Path = string.Empty;
            OrleansClientConfigFilePath = "OrleansClientConfiguration.xml";
        }

        public string Path { get; set; }
        public string OrleansClientConfigFilePath { get; set; }
    }
}