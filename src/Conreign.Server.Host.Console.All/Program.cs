using System;
using System.Diagnostics;
using System.Threading;
using SiloRunner = Conreign.Server.Host.Console.Api.Runner;
using ApiRunner = Conreign.Server.Host.Console.Silo.Runner;

namespace Conreign.Server.Host.Console.All
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var exitEvent = new ManualResetEvent(false);
            System.Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };
            try
            {
                using (SiloRunner.Run(args))
                using (ApiRunner.Run(args))
                {
                    exitEvent.WaitOne();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                if (Debugger.IsAttached)
                {
                    System.Console.WriteLine("Press enter to exit...");
                    System.Console.ReadLine();
                }
                Environment.Exit(1);
            }

        }
    }
}