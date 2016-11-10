using System;

namespace Conreign.LoadTest
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                LoadTestRunner.Run(args).Wait();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }
}
