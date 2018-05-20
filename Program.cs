using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            TwitchBotConnection bot = new TwitchBotConnection();
            bot.Connect();
            Console.ReadLine();
        }
    }
}
