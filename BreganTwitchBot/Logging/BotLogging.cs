using System;
using BreganTwitchBot.Connection;
using Serilog;

namespace BreganTwitchBot.Logging
{
    class BotLogging
    {
 
        public static void BotLoggingStart()
        {
            TwitchBotConnection.Client.OnMessageReceived += MessageReceived;
            TwitchBotConnection.Client.OnUserJoined += UserJoinedStream;
            TwitchBotConnection.Client.OnUserLeft += UserLeftStream;
            TwitchBotConnection.Client.OnNewSubscriber += NewSub;
            TwitchBotConnection.Client.OnUserBanned += UserBanned;
            TwitchBotConnection.Client.OnUserTimedout += UserTimedOut;
            Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.RollingFile("Logs/log-{Date}.log").CreateLogger();

        }

        private static void UserTimedOut(object sender, TwitchLib.Client.Events.OnUserTimedoutArgs e)
        {
            Log.Information($"[User Timed out in Stream] User banned: {e.UserTimeout.Username} Duration: {e.UserTimeout.TimeoutDuration} Reason: {e.UserTimeout.TimeoutReason}");
        }

        private static void UserBanned(object sender, TwitchLib.Client.Events.OnUserBannedArgs e)
        {
            Log.Information($"[User Banned in Stream] User banned: {e.UserBan.Username}");
        }

        private static void NewSub(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
        {
            Log.Information($"[New Twitch Subscriber] {e.Subscriber.DisplayName} has just subbed!");
        }

        private static void UserJoinedStream(object sender, TwitchLib.Client.Events.OnUserJoinedArgs e)
        {
            Log.Information($"[User Joined Stream] {e.Username}");
        }
        private static void UserLeftStream(object sender, TwitchLib.Client.Events.OnUserLeftArgs e)
        {
            Log.Information($"[User Left Stream] {e.Username}");
        }

        private static void MessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            Log.Information($"[Chat Message Received in Stream] {e.ChatMessage.DisplayName}: {e.ChatMessage.Message}");
        }
    }
}
