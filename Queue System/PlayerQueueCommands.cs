using System;
using System.Threading;

namespace Twitch_Bot
{
    internal class PlayerQueueCommands
    {
        public static void QueueSystemCommandSetup()
        {
            TwitchBotConnection.client.OnMessageReceived += MessageRecieved;
        }

        private static void MessageRecieved(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            switch (e.ChatMessage.Message)
            {
                case "!joinqueue" when PlayerQueueSystem.QueueUserCheck(e.ChatMessage.Username) == false:
                    PlayerQueueSystem.QueueAdd(e.ChatMessage.Username);
                    break;
                case "!leavequeue":
                    PlayerQueueSystem.QueueRemove(e.ChatMessage.Username);
                    break;
                case "!queue":
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName, $"The current queue is {PlayerQueueSystem.CurrentQueue()}");
                    break;
                case "!nextgame":
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName, $"The next players for the game are {PlayerQueueSystem.NextGamePlayers()}"); 
                    break;
            }

            if (e.ChatMessage.Message == "!remove3" && e.ChatMessage.IsModerator)
            {
                PlayerQueueSystem.QueueRemove3();
                TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName, $"{e.ChatMessage.Username}: the current players have been removed");
            }
            
            else if (e.ChatMessage.Message == "!clearqueue" && e.ChatMessage.IsBroadcaster || e.ChatMessage.Message == "!clearqueue" && e.ChatMessage.Username == "guinea")
            {
                PlayerQueueSystem.QueueClear();
            }
            else if (e.ChatMessage.Message == "!queuecommands")
            {
                TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName, "The commands are: !joinqueue, !leavequeue, !queue, !nextgame & the mod commands are !remove4 & !clearqueue (blocksssssss only)");
            }
        }
    }
}
