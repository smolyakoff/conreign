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
                Console.WriteLine("COMPLETED");
                Console.Out.Flush();
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR");
                Console.Error.WriteLine(ex.ToString());
                Console.Error.Flush();
                return 1;
            }
        }
    }
}
