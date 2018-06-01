using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.PubSub;

namespace Twitch_Bot
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
