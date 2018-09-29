using System;
using System.Configuration;

namespace BreganTwitchBot.Connection
{
    class FirstTimeConfig
    {
        public static void FirstTimeStartup()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Console.WriteLine("Welcome to the Bregan TwitchBot! As this is the first time you will have to do some configuration");

            //Channel name
            while (true)
            {
                Console.WriteLine("Please enter the channel name to connect to");
                var channelName = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(channelName))
                {
                    config.AppSettings.Settings["ChannelName"].Value = channelName;
                    Console.Clear();
                    break;
                }
                Console.WriteLine("please enter a value");
            }

            //Bot Name
            while (true)
            {
                Console.WriteLine("Please enter the Twitch username of the bot you are going to use");
                var botUsername = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(botUsername))
                {
                    config.AppSettings.Settings["BotName"].Value = botUsername;
                    Console.Clear();
                    break;

                }
                Console.WriteLine("Please enter a value");
            }
            //Bot OAuth
            while (true)
            {
                Console.WriteLine("Please generate the Twitch Access Token with the Twitch bot account. This can be done from https://twitchtokengenerator.com");
                var botOAuth = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(botOAuth) && botOAuth.Length > 6)
                {
                    config.AppSettings.Settings["ChannelOAuth"].Value = botOAuth;
                    Console.Clear();
                    break;
                }
                Console.WriteLine("Please enter a value");
            }
            //PubSub OAuth
            while (true)
            {
                Console.WriteLine("To thank bit donations, another Twitch Access Token is needed from the channel broadcaster account");
                Console.WriteLine("To generate the token, go to https://twitchtokengenerator.com and select Custom Scope Token. Enable bits:read and generate the token");
                Console.WriteLine("If you don't want bits thanked out, press enter");

                var pubSubOAuth = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(pubSubOAuth))
                {
                    Console.Clear();
                    break;
                }

                if (!string.IsNullOrWhiteSpace(pubSubOAuth) && pubSubOAuth.Length > 6)
                {
                    config.AppSettings.Settings["PubSubOAuth"].Value = pubSubOAuth;
                    Console.Clear();
                    break;
                }
                Console.WriteLine(@"Please enter the Twitch Token or ""skip"" to not enable it");
            }

            //Twitch API OAuth
            while (true)
            {
                Console.WriteLine("The final bit needed is a Twitch API Token. This can be created at https://dev.twitch.tv/dashboard/apps/create");
                var twitchAPIOAuth = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(twitchAPIOAuth) && twitchAPIOAuth.Length > 6)
                {
                    config.AppSettings.Settings["TwitchAPIOAuth"].Value = twitchAPIOAuth;
                    Console.Clear();
                    break;
                }
                Console.WriteLine("Please enter a value");
            }
            config.AppSettings.Settings["BotConfigured"].Value = "true";
            Console.Clear();

            //Discord
            while (true)
            {
                Console.WriteLine("Next is discord integration:");
                Console.WriteLine("Please go to https://discordapp.com/developers/applications/ and create a new API key and enter it");
                var discordAPIkey = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(discordAPIkey))
                {
                    config.AppSettings.Settings["DiscordAPIKey"].Value = discordAPIkey;
                    Console.Clear();
                }

                //Discord event channel
                Console.WriteLine("Go to Discord and enable Developer mode under appearance. Create a channel for bot events and copy the channel ID");
                var discordEventChannelID = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(discordEventChannelID))
                {
                    config.AppSettings.Settings["DiscordEventChannelID"].Value = discordEventChannelID;
                    Console.Clear();
                }

                while (true)
                {
                    Console.WriteLine("Please copy your Discord User ID and enter it. This is for song requests");
                    var discordNameID = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(discordNameID))
                    {
                        config.AppSettings.Settings["DiscordNameID"].Value = discordNameID;
                        break;
                    }

                    Console.WriteLine("Please enter a valid ID");
                }

                //Discord announcement channel
                Console.WriteLine("Please enter the channel ID for stream announcements");
                var discordAnnouncementChannelID = Console.ReadLine();

                config.AppSettings.Settings["DiscordAnnouncementChannelID"].Value = discordAnnouncementChannelID;
                Console.Clear();
            }
        }

    }
}
