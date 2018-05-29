using System;

namespace Twitch_Bot
{
    internal class CommandListener
    {
        public static void CommandListenerSetup()
        {
            TwitchBotConnection.client.OnMessageReceived += Commands;
        }

        private static void Commands(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.StartsWith("!8ball"))
            {
                TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName, EightBall.Ask8Ball());
            }
        }
    }
}
