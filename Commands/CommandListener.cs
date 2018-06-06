

using Bregan_TwitchBot.Commands._8Ball;

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
            if (e.ChatMessage.Message.StartsWith("!8ball"))
            {
                TwitchBotConnection.Client.SendMessage(TwitchBotConnection.ChannelConnectName, EightBall.Ask8Ball());
            }

            if (e.ChatMessage.Message.StartsWith("!dadjoke"))
            {
                TwitchBotConnection.Client.SendMessage(TwitchBotConnection.ChannelConnectName, DadJoke.DadJoke.DadJokeGenerate().Result);
            }
        }
    }
}
