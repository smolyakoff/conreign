using System;

namespace Conreign.LoadTest.Supervisor
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                LoadTestSupervisorRunner.Run(args).Wait();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }
}