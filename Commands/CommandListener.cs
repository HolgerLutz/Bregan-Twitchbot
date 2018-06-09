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

            //8Ball
            if (e.ChatMessage.Message.StartsWith("!8ball"))
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, EightBall.Ask8Ball());
            }
            //Dadjoke
            if (e.ChatMessage.Message.StartsWith("!dadjoke"))
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, DadJoke.DadJoke.DadJokeGenerate().Result);
            }

            //Queue Commands
            switch (e.ChatMessage.Message)
            {
                case "!joinqueue" when PlayerQueueSystem.QueueUserCheck(e.ChatMessage.Username) == false:
                    PlayerQueueSystem.QueueAdd(e.ChatMessage.Username);
                    break;
                case "!leavequeue":
                    PlayerQueueSystem.QueueRemove(e.ChatMessage.Username);
                    break;
                case "!queue":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"The current queue is {PlayerQueueSystem.CurrentQueue()}");
                    break;
                case "!nextgame":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"The next players for the game are {PlayerQueueSystem.NextGamePlayers()}");
                    break;
                case "!queuecommands":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "The commands are: !joinqueue, !leavequeue, !queue, !nextgame & the mod commands are !removegame & !clearqueue");
                    break;

                //Mod Commands
                case "!removegame" when e.ChatMessage.IsModerator:
                    PlayerQueueSystem.QueueRemove3();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.ChatMessage.Username}: the current players have been removed");
                    break;
                case "!clearqueue" when e.ChatMessage.IsBroadcaster:
                case "!clearqueue" when e.ChatMessage.IsModerator:
                    PlayerQueueSystem.QueueClear();
                    break;

            }
        }
    }
}
