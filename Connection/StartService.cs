using System.Threading;
using Bregan_TwitchBot.Commands;
using Bregan_TwitchBot.Commands.Big_Ben;
using Bregan_TwitchBot.Commands.Queue;
using Bregan_TwitchBot.Logging;

namespace Bregan_TwitchBot.Connection
{
    internal class StartService
    {
        public static void ServiceStart()
        {
            //Run in new thread as it loops
            Thread t1 = new Thread(BigBenBong.Bong);
            t1.Start();
            //Start the bot
            TwitchBotConnection bot = new TwitchBotConnection();
            bot.Connect();
            //Start everything else
            BotLogging.BotLoggingStart();
            PlayerQueueSystem.QueueCreate();
            TwitchBotGeneralMessages.TwitchMessageSetup();
            CommandListener.CommandListenerSetup();
        }
    }
}
