using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Conreign.Server.Host.Console.Silo
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var exitEvent = new ManualResetEvent(false);
            System.Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };
            ServicePointManager.DefaultConnectionLimit = 10000;
            try
            {
                using (Runner.Run(args))
                {
                    exitEvent.WaitOne();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                if (Debugger.IsAttached)
                {
                    System.Console.WriteLine("Press enter to exit...");
                    System.Console.ReadLine();
                }
                Environment.Exit(-1);
            }
        }
    }
}