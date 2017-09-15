using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Conreign.Server.Silo;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;

namespace Conreign.Server.Host.Console.Silo
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var exitEvent = new ManualResetEvent(false);
            System.Console.CancelKeyPress += (sender, eventArgs) => {
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
            catch (Exception)
            {
                Environment.Exit(-1);
            }
        }
    }
}
