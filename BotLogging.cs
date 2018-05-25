using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Extensions;

namespace Twitch_Bot
{
    internal class BotLogging
    {
        public static void BotLoggingStart()
        {
            TwitchBotConnection.client.OnMessageReceived += MessageRecieved;
            TwitchBotConnection.client.OnUserJoined += UserJoinedStream;
            TwitchBotConnection.client.OnUserLeft += UserLeftStream;
            TwitchBotConnection.client.OnNewSubscriber += NewSub;
            //TwitchBotConnection.client.OnLog += Client_OnLog;
        }

        //private static void Client_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
        //{
        //    Console.WriteLine(e.Data);
        //}


        private static void NewSub(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[New Subscriber] {DateTime.Now}: {e.Subscriber.DisplayName} has just subbed!");
        }

        private static void UserJoinedStream(object sender, TwitchLib.Client.Events.OnUserJoinedArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[User Joined] {DateTime.Now}: {e.Username}");
            Console.ResetColor();
        }
        private static void UserLeftStream(object sender, TwitchLib.Client.Events.OnUserLeftArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[User Left] {DateTime.Now}: {e.Username}");
            Console.ResetColor();
        }

        private static void MessageRecieved(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            Console.WriteLine($"[Chat Message Recieved] {DateTime.Now}: {e.ChatMessage.DisplayName} : {e.ChatMessage.Message}");
        }


    }
}
