using System;
using Bregan_TwitchBot.Commands.Message_Limiter;
using Bregan_TwitchBot.Commands.Queue;
using Bregan_TwitchBot.Commands._8Ball;
using Bregan_TwitchBot.Connection;

namespace Bregan_TwitchBot.Commands
{
    internal class CommandListener
    {
        public static void CommandListenerSetup()
        {
            TwitchBotConnection.Client.OnMessageReceived += Commands;
        }

        private static void Commands(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            //Message limit checker
            if (CommandLimiter.MessagesSent >= CommandLimiter.MessageLimit)
            {
                Console.WriteLine("[Message Limiter] Message Limit Hit");
                return;
            }
            //8Ball
            if (e.ChatMessage.Message.StartsWith("!8ball"))
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, EightBall.Ask8Ball());
                CommandLimiter.AddMessageCount();
            }
            //Dadjoke
            if (e.ChatMessage.Message.StartsWith("!dadjoke"))
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, DadJoke.DadJoke.DadJokeGenerate().Result);
                CommandLimiter.AddMessageCount();
            }

            //Queue Commands
            if (e.ChatMessage.Message == "!joinqueue" && PlayerQueueSystem.QueueUserCheck(e.ChatMessage.Username) == false)
            {
                PlayerQueueSystem.QueueAdd(e.ChatMessage.Username);
                CommandLimiter.AddMessageCount();
            }
            else if (e.ChatMessage.Message == "!leavequeue")
            {
                PlayerQueueSystem.QueueRemove(e.ChatMessage.Username);
                CommandLimiter.AddMessageCount();
            }
            else if (e.ChatMessage.Message == "!queue")
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"The current queue is {PlayerQueueSystem.CurrentQueue()}");
                CommandLimiter.AddMessageCount();
            }
            else if (e.ChatMessage.Message == "!nextgame")
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"The next players for the game are {PlayerQueueSystem.NextGamePlayers()}");
                CommandLimiter.AddMessageCount();
            }
            //Mod Commands
            else if ((e.ChatMessage.Message == "!removegame" && e.ChatMessage.IsModerator) ||
                     (e.ChatMessage.Message == "!removegame" && e.ChatMessage.IsBroadcaster))
            {
                PlayerQueueSystem.QueueRemove3();
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.ChatMessage.Username}: the current players have been removed");
                CommandLimiter.AddMessageCount();
            }
            else if ((e.ChatMessage.Message == "!clearqueue" && e.ChatMessage.IsBroadcaster) ||
                     (e.ChatMessage.Message == "!clearqueue" && e.ChatMessage.IsModerator))
            {
                PlayerQueueSystem.QueueClear();
            }
            else if ((e.ChatMessage.Message == "!setremoveamount" && e.ChatMessage.IsModerator) ||
                     (e.ChatMessage.Message == "!setremoveamount" && e.ChatMessage.IsBroadcaster))
            {
                PlayerQueueSystem.SetQueueRemoveAmount(e.ChatMessage.Message);
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName,$"The remove amount has been updated to {PlayerQueueSystem.QueueRemoveAmount}");
                CommandLimiter.AddMessageCount();
            }
            else if (e.ChatMessage.Message.StartsWith("!setremoveamount") && e.ChatMessage.IsModerator ||
                e.ChatMessage.Message.StartsWith("!setremoveamount") && e.ChatMessage.IsBroadcaster)
            {
                PlayerQueueSystem.SetQueueRemoveAmount(e.ChatMessage.Message);
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"The remove amount has been updated to {PlayerQueueSystem.QueueRemoveAmount}");
                CommandLimiter.AddMessageCount();
            }
            else if (e.ChatMessage.Message.StartsWith("!queueposition"))
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, PlayerQueueSystem.GetQueuePosition(e.ChatMessage.Username));
                CommandLimiter.AddMessageCount();
            }
        }
    }
}
