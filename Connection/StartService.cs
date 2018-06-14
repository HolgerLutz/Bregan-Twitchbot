using System;
using System.Configuration;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Bregan_TwitchBot.Commands;
using Bregan_TwitchBot.Commands.Big_Ben;
using Bregan_TwitchBot.Commands.Message_Limiter;
using Bregan_TwitchBot.Commands.Queue;
using Bregan_TwitchBot.Logging;

namespace Bregan_TwitchBot.Connection
{
    internal class StartService
    {
        //Config variables
        public static string ChannelName;
        public static string BotName;
        public static string BotOAuth;
        public static string PubSubOAuth;
        public static string TwitchAPIOAuth;

        public static void ServiceStart()
        {
            var configCheck = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var botConfigured = configCheck.AppSettings.Settings["BotConfigured"].Value;

            if (botConfigured == "false") //First time has to be set up
            {
                FirstTimeConfig.FirstTimeStartup();
            }
            
            ConfigurationManager.RefreshSection("appSettings");
            var configReload = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //Load config to variables
            ChannelName = configReload.AppSettings.Settings["ChannelName"].Value;
            BotName = configReload.AppSettings.Settings["BotName"].Value;
            BotOAuth = configReload.AppSettings.Settings["ChannelOAuth"].Value;
            PubSubOAuth = configReload.AppSettings.Settings["PubSubOAuth"].Value;
            TwitchAPIOAuth = configReload.AppSettings.Settings["TwitchAPIOAuth"].Value;

            if (PubSubOAuth != "NotSet")
            {
                var pubsub = new PubSubConnection();
                pubsub.Connect();
            }

            //Threads for timers
            Thread t1 = new Thread(BigBenBong.Bong);
            Thread t2 = new Thread(CommandLimiter.ResetMessageLimit);
            t1.Start();
            t2.Start();

            //Start the bot
            TwitchBotConnection bot = new TwitchBotConnection();
            bot.Connect();

            //Connect to the API
            TwitchApiConnection twitchApi = new TwitchApiConnection();
            twitchApi.Connect();

            //Start everything else
            BotLogging.BotLoggingStart();
            PlayerQueueSystem.QueueCreate();
            TwitchBotGeneralMessages.TwitchMessageSetup();
            CommandListener.CommandListenerSetup();
            Task.Delay(1000).ContinueWith(t => CommandLimiter.SetMessageLimit());
        }
    }
}
