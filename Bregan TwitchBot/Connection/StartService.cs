using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Bregan_TwitchBot.Commands;
using Bregan_TwitchBot.Commands.Big_Ben;
using Bregan_TwitchBot.Commands.Giveaway;
using Bregan_TwitchBot.Commands.Message_Limiter;
using Bregan_TwitchBot.Commands.Queue;
using Bregan_TwitchBot.Commands.Random_User;
using Bregan_TwitchBot.Commands.Word_Blacklister;
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
        public static string TwitchChannelID;
        public static void ServiceStart()
        {
            var configCheck = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configCheck.AppSettings.Settings["BotConfigured"].Value == "false") //First time has to be set up
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

            //Start the bot
            TwitchBotConnection bot = new TwitchBotConnection();
            bot.Connect();

            //Connect to the API
            TwitchApiConnection twitchApi = new TwitchApiConnection();
            twitchApi.Connect();

            var getUserID = TwitchApiConnection.ApiClient.Users.v5.GetUserByNameAsync(ChannelName).Result.Matches;
            TwitchChannelID = getUserID[0].Id;

            //Start Threads
            Thread t1 = new Thread(CommandListener.CommandListenerSetup); //Commands
            Thread t2 = new Thread(BotLogging.BotLoggingStart); //Logging
            Thread t3 = new Thread(WordBlackList.StartBlacklist); //Start word blacklist
            t1.Start();
            t2.Start();
            t3.Start();

            //Start everything
            BigBenBong.Bong(); //Big Ben
            RandomUser.StartGetChattersTimer(); //Get the chatters for random user commands
            PlayerQueueSystem.QueueCreate(); //Queue
            TwitchBotGeneralMessages.TwitchMessageSetup(); //Sub/bit messages
            CommandLimiter.SetMessageLimit(); //Set Message Limit
            CommandLimiter.ResetMessageLimit(); //Start message resetter
            //Giveaway
            Giveaways.IsGiveawayOn = false;
            Giveaways.TimerAmount = 40000;
        }
    }
}
