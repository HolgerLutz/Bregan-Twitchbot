using System;
using Bregan_TwitchBot.Commands.Message_Limiter;
using Bregan_TwitchBot.Commands.Queue;
using Bregan_TwitchBot.Commands.Random_User;
using Bregan_TwitchBot.Commands._8Ball;
using Bregan_TwitchBot.Connection;


namespace Bregan_TwitchBot.Commands
{
    internal class CommandListener
    {
        public static void CommandListenerSetup()
        {
            TwitchBotConnection.Client.OnChatCommandReceived += Commands;
        }

        private static void Commands(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
        {
            //Message limit checker
            if (CommandLimiter.MessagesSent >= CommandLimiter.MessageLimit)
            {
                Console.WriteLine("[Message Limiter] Message Limit Hit");
                return;
            }

            //General pre-programmed commands
            switch (e.Command.CommandText)
            {
                case "8ball":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, EightBall.Ask8Ball());
                    CommandLimiter.AddMessageCount();
                    break;
                case "dadjoke":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        DadJoke.DadJoke.DadJokeGenerate().Result);
                    CommandLimiter.AddMessageCount();
                    break;
                case "commands":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        "You can find the commands at https://github.com/Bregann/Bregan-Twitchbot#bregan-twitchbot");
                    CommandLimiter.AddMessageCount();
                    break;
                case "pitchfork":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        $"{e.Command.ChatMessage.Username} just pitchforked -------E {RandomUser.SelectRandomUser()}");
                    CommandLimiter.AddMessageCount();
                    break;
                case "shoutout" when e.Command.ChatMessage.IsModerator:
                case "shoutout" when e.Command.ChatMessage.IsBroadcaster:
                    var userToShoutout = e.Command.ChatMessage.Message.Replace("!shoutout @", ""); //Remove the @ if it is there
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Hey go check out {userToShoutout.Replace("!shoutout","")} at twitch.tv/{userToShoutout.Replace("!shoutout", "").Trim()} for some great content!");
                    CommandLimiter.AddMessageCount();
                    break;
            }

            //Queue commands
            switch (e.Command.CommandText)
            {
                case "joinqueue" when PlayerQueueSystem.QueueUserCheck(e.Command.ChatMessage.Username) == false:
                    PlayerQueueSystem.QueueAdd(e.Command.ChatMessage.Username);
                    break;
                case "leavequeue":
                    PlayerQueueSystem.QueueRemove(e.Command.ChatMessage.Username);
                    break;
                case "queue":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        $"The current queue is {PlayerQueueSystem.CurrentQueue()}");
                    CommandLimiter.AddMessageCount();
                    break;
                case "nextgame":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        $"The next players for the game are {PlayerQueueSystem.NextGamePlayers()}");
                    CommandLimiter.AddMessageCount();
                    break;
                case "queueposition":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        PlayerQueueSystem.GetQueuePosition(e.Command.ChatMessage.Username));
                    CommandLimiter.AddMessageCount();
                    break;
                case "removegame" when e.Command.ChatMessage.IsModerator:
                case "removegame" when e.Command.ChatMessage.IsBroadcaster:
                    PlayerQueueSystem.QueueRemove3();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        $"{e.Command.ChatMessage.Username}: the current players have been removed");
                    CommandLimiter.AddMessageCount();
                    break;
                case "clearqueue" when e.Command.ChatMessage.IsModerator:
                case "clearqueue" when e.Command.ChatMessage.IsBroadcaster:
                    PlayerQueueSystem.QueueClear();
                    break;
                case "setremoveamount" when e.Command.ChatMessage.IsModerator:
                case "setremoveamount" when e.Command.ChatMessage.IsBroadcaster:
                    PlayerQueueSystem.SetQueueRemoveAmount(e.Command.ChatMessage.Message);
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        $"The remove amount has been updated to {PlayerQueueSystem.QueueRemoveAmount}");
                    CommandLimiter.AddMessageCount();
                    break;
            }

        }
    }
}
