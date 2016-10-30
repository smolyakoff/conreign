using Conreign.Core.AI.LoadTest;

namespace Conreign.LoadTest
{
    public class LoadTestOptions
    {
        public LoadTestOptions()
        {
            ConnectionUri = "http://localhost";
            BotOptions = new LoadTestBotOptions();
        }

        public string ConnectionUri { get; set; }
        public LoadTestBotOptions BotOptions { get; set; }
    }
}