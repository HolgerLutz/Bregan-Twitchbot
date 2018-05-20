using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Bot
{
    internal class BotLogging
    {
        public static void LoggingSetup()
        {
            TwitchBotConnection.client.OnMessageReceived += MessageRecieved;
            TwitchBotConnection.client.OnUserJoined += UserJoinedStream;
            TwitchBotConnection.client.OnNewSubscriber += NewSub;
        }

        private static void NewSub(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(DateTime.Now + ": " + e.Subscriber.DisplayName + " has just subbed!");
        }

        private static void UserJoinedStream(object sender, TwitchLib.Client.Events.OnUserJoinedArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(DateTime.Now + ": " + e.Username);
            Console.ResetColor();
        }

        private static void MessageRecieved(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            Console.WriteLine(e.ChatMessage);
        }


    }
}
