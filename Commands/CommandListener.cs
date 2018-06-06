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
                TwitchBotConnection.Client.SendMessage(TwitchBotConnection.ChannelConnectName, EightBall.Ask8Ball());
            }
            //Dadjoke
            if (e.ChatMessage.Message.StartsWith("!dadjoke"))
            {
                TwitchBotConnection.Client.SendMessage(TwitchBotConnection.ChannelConnectName, DadJoke.DadJoke.DadJokeGenerate().Result);
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
                    TwitchBotConnection.Client.SendMessage(TwitchBotConnection.ChannelConnectName, $"The current queue is {PlayerQueueSystem.CurrentQueue()}");
                    break;
                case "!nextgame":
                    TwitchBotConnection.Client.SendMessage(TwitchBotConnection.ChannelConnectName, $"The next players for the game are {PlayerQueueSystem.NextGamePlayers()}");
                    break;
            }
            //Remove 3 users (last game)
            if (e.ChatMessage.Message == "!remove3" && e.ChatMessage.IsModerator)
            {
                PlayerQueueSystem.QueueRemove3();
                TwitchBotConnection.Client.SendMessage(TwitchBotConnection.ChannelConnectName, $"{e.ChatMessage.Username}: the current players have been removed");
            }
            //Remove all users
            else if (e.ChatMessage.Message == "!clearqueue" && e.ChatMessage.IsBroadcaster || e.ChatMessage.Message == "!clearqueue" && e.ChatMessage.Username == "guinea")
            {
                PlayerQueueSystem.QueueClear();
            }
            //See the commands for the queue system
            else if (e.ChatMessage.Message == "!queuecommands")
            {
                TwitchBotConnection.Client.SendMessage(TwitchBotConnection.ChannelConnectName, "The commands are: !joinqueue, !leavequeue, !queue, !nextgame & the mod commands are !remove4 & !clearqueue (blocksssssss only)");
            }
        }
    }
}
