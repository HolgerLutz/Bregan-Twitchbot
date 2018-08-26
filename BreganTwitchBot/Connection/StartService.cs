﻿using System;
using System.Configuration;
using System.Threading;
using BreganTwitchBot.Database;
using BreganTwitchBot.Discord;
using BreganTwitchBot.Logging;
using BreganTwitchBot.TwitchCommands;
using BreganTwitchBot.TwitchCommands.BigBen;
using BreganTwitchBot.TwitchCommands.Giveaway;
using BreganTwitchBot.TwitchCommands.MessageLimiter;
using BreganTwitchBot.TwitchCommands.Queue;
using BreganTwitchBot.TwitchCommands.RandomUser;
using BreganTwitchBot.TwitchCommands.WordBlacklister;
using TwitchLib.Api.Exceptions;

namespace BreganTwitchBot.Connection
{
    class StartService
    {
        //Config variables
        public static string ChannelName;
        public static string BotName;
        public static string BotOAuth;
        public static string PubSubOAuth;
        public static string TwitchAPIOAuth;
        public static string TwitchChannelID;
        public static string DiscordAPIKey;
        public static ulong DiscordUsernameID;
        public static ulong DiscordEventChannelID;
        public static ulong DiscordAnnouncementChannelID;

        public static void ServiceStart()
        {
            var configCheck = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configCheck.AppSettings.Settings["BotConfigured"].Value == "false") //First time has to be set up
            {
                FirstTimeConfig.FirstTimeStartup();
            }
            
            ConfigurationManager.RefreshSection("appSettings");
            var configReload = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //Both channels have to be ulong to work with Discord
            ulong.TryParse(configReload.AppSettings.Settings["DiscordEventChannelID"].Value, out var discordEventID);
            ulong.TryParse(configReload.AppSettings.Settings["DiscordAnnouncementChannelID"].Value, out var discordAnnouncementID);
            ulong.TryParse(configReload.AppSettings.Settings["DiscordNameID"].Value, out var discordUserID);
            //Load config to variables
            ChannelName = configReload.AppSettings.Settings["ChannelName"].Value;
            BotName = configReload.AppSettings.Settings["BotName"].Value;
            BotOAuth = configReload.AppSettings.Settings["ChannelOAuth"].Value;
            PubSubOAuth = configReload.AppSettings.Settings["PubSubOAuth"].Value;
            TwitchAPIOAuth = configReload.AppSettings.Settings["TwitchAPIOAuth"].Value;
            DiscordAPIKey = configReload.AppSettings.Settings["DiscordAPIKey"].Value;
            DiscordEventChannelID = discordEventID;
            DiscordAnnouncementChannelID = discordAnnouncementID;
            DiscordUsernameID = discordUserID;

            if (PubSubOAuth != "NotSet")
            {
                var pubSub = new PubSubConnection();
                pubSub.Connect();
            }

            
            //Start the bot
            TwitchBotConnection bot = new TwitchBotConnection();
            bot.Connect();

            //DB
            DatabaseSetup.StartupDatabase();

            //Start Discord if it has been enabled
            if (DiscordAPIKey != "NotSet")
            {
                Thread discordThread = new Thread(DiscordConnection.MainAsync().GetAwaiter().GetResult);
                discordThread.Start();
                DiscordEvents.StartDiscordAlerts();
                DiscordCommands.StartDiscordCommands();
            }

            //Connect to the API
            try
            {
                TwitchApiConnection twitchApi = new TwitchApiConnection();
                twitchApi.Connect();

                var getUserID = TwitchApiConnection.ApiClient.Users.v5.GetUserByNameAsync(ChannelName).Result.Matches;
                TwitchChannelID = getUserID[0].Id;
            }
            catch (BadGatewayException)
            {
                Console.WriteLine("[Startup] BadGatewayException while connecting to the the Twitch API");
                throw;
            }
            catch (InternalServerErrorException)
            {
                Console.WriteLine("[Startup] InternalServerErrorException while connecting to the the Twitch API");
                throw;
            }

            //Start everything
            BotLogging.BotLoggingStart(); //Logging
            CommandListener.CommandListenerSetup(); //Commands
            
            BigBenBong.Bong(); //Big Ben
            RandomUser.StartGetChattersTimer(); //Get the chatters for random user commands
            WordBlackList.StartBlacklist(); //Word blacklister
            TimeTracker.UserTimeTracker(); //Time tracker
            PlayerQueueSystem.QueueCreate(); //Queue
            TwitchBotGeneralMessages.TwitchMessageSetup(); //Sub/bit messages
            CommandLimiter.SetMessageLimit(); //Set Message Limit
            CommandLimiter.ResetMessageLimit(); //Start message resetter

            //Giveaway
            Giveaways.IsGiveawayOn = false;
            Giveaways.TimerAmount = 60000;
        }
    }
}
