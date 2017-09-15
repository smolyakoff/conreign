﻿using System;
using System.Net;
using System.Threading;

namespace Conreign.Server.Host.Console.Api
{
    internal class Program
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
            catch
            {
                Environment.Exit(-1);
            }
        }
    }
}
