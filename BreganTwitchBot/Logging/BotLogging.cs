using System;
using BreganTwitchBot.Connection;

namespace BreganTwitchBot.Logging
{
    class BotLogging
    {
        public static void BotLoggingStart()
        {
            TwitchBotConnection.Client.OnMessageReceived += MessageRecieved;
            TwitchBotConnection.Client.OnUserJoined += UserJoinedStream;
            TwitchBotConnection.Client.OnUserLeft += UserLeftStream;
            TwitchBotConnection.Client.OnNewSubscriber += NewSub;
            TwitchBotConnection.Client.OnUserBanned += UserBanned;
            TwitchBotConnection.Client.OnUserTimedout += UserTimedOut;
            //TwitchBotConnection.Client.OnLog += Client_OnLog;
        }

        private static void UserTimedOut(object sender, TwitchLib.Client.Events.OnUserTimedoutArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"[User Timed out] {DateTime.Now}: User banned: {e.UserTimeout.Username} Duration: {e.UserTimeout.TimeoutDuration}");
            Console.ResetColor();
        }

        private static void UserBanned(object sender, TwitchLib.Client.Events.OnUserBannedArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"[User Banned] {DateTime.Now}: User banned: {e.UserBan.Username}");
            Console.ResetColor();
        }

       // private static void Client_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
       // {
       //     Console.WriteLine(e.Data);
       // }


        private static void NewSub(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[New Subscriber] {DateTime.Now}: {e.Subscriber.DisplayName} has just subbed!");
            Console.ResetColor();
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
            Console.WriteLine($"[Chat Message Recieved] {DateTime.Now}: {e.ChatMessage.DisplayName}: {e.ChatMessage.Message}");
        }

    }
}
