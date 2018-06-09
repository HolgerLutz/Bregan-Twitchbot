using System;
using System.Configuration;
using System.Threading;

namespace Bregan_TwitchBot.Connection
{
    internal class FirstTimeConfig
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
                Console.WriteLine(
                    "To thank bit donations, another Twitch Access Token is needed from the channel broadcaster account");
                Console.WriteLine(
                    "To generate the token, go to https://twitchtokengenerator.com and select Custom Scope Token. Enable bits:read and generate the token");
                Console.WriteLine("If you don't want bits thanked out, type skip");

                var pubSubOAuth = Console.ReadLine();

                if (pubSubOAuth == "skip")
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

            Console.WriteLine("Thank you! The Bot has now been setup. To edit this at any time simply type reconfig in the console window");
            config.AppSettings.Settings["BotConfigured"].Value = "true";
            config.Save();
            Thread.Sleep(5000);
            Console.Clear();
        }
    }
}
