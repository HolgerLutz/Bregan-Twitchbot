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
            //Run in new thread as it loops
            Thread thread = new Thread(BigBenBong.Bong);
            thread.Start();

            //Start bot
            TwitchBotConnection bot = new TwitchBotConnection();
            bot.Connect();
            
            Console.ReadLine();
        }
    }
}
