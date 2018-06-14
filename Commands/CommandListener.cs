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
            if (CommandLimiter.MessagesSent >= CommandLimiter.MessageLimit)
            {
                Console.WriteLine("[Message Limiter] LIMIT HIT");
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

            if (e.ChatMessage.Message.StartsWith("!test"))
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "/mods");
                CommandLimiter.AddMessageCount();
            }
            //Queue Commands
            switch (e.ChatMessage.Message)
            {
                case "!joinqueue" when PlayerQueueSystem.QueueUserCheck(e.ChatMessage.Username) == false:
                    PlayerQueueSystem.QueueAdd(e.ChatMessage.Username);
                    CommandLimiter.AddMessageCount();
                    break;
                case "!leavequeue":
                    PlayerQueueSystem.QueueRemove(e.ChatMessage.Username);
                    CommandLimiter.AddMessageCount();
                    break;
                case "!queue":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"The current queue is {PlayerQueueSystem.CurrentQueue()}");
                    CommandLimiter.AddMessageCount();
                    break;
                case "!nextgame":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"The next players for the game are {PlayerQueueSystem.NextGamePlayers()}");
                    CommandLimiter.AddMessageCount();
                    break;
                case "!queuecommands":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "The commands are: !joinqueue, !leavequeue, !queue, !nextgame & the mod commands are !removegame & !clearqueue");
                    CommandLimiter.AddMessageCount();
                    break;

                //Mod Commands
                case "!removegame" when e.ChatMessage.IsModerator:
                    PlayerQueueSystem.QueueRemove3();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.ChatMessage.Username}: the current players have been removed");
                    CommandLimiter.AddMessageCount();
                    break;
                case "!clearqueue" when e.ChatMessage.IsBroadcaster:
                case "!clearqueue" when e.ChatMessage.IsModerator:
                    PlayerQueueSystem.QueueClear();
                    break;
            }           
        }
    }
}
