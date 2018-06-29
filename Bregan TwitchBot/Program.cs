using System;
using System.Threading;
using Bregan_TwitchBot.Commands.Big_Ben;
using Bregan_TwitchBot.Connection;

namespace Bregan_TwitchBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //Start
            StartService.ServiceStart();
            Console.ReadLine();
        }
    }
}
