using System;
using System.IO;
using BreganTwitchBot.Connection;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.RollingFile;

namespace BreganTwitchBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //Start
            StartService.ServiceStart();
            Console.Read();
        }

    }
}
