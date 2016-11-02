namespace Conreign.LoadTest
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            LoadTestRunner.Run(args).Wait();
        }
    }
}
