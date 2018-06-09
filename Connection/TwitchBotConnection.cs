using System;
using Bregan_TwitchBot.Commands;
using Bregan_TwitchBot.Commands.Queue;
using Bregan_TwitchBot.Logging;
using Bregan_TwitchBot.Connection;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.PubSub;

namespace Bregan_TwitchBot.Connection
{
    internal class TwitchBotConnection
    {
        public static TwitchClient Client;
        private readonly ConnectionCredentials _credentials = new ConnectionCredentials(StartService.BotName, StartService.BotOAuth);

        internal void Connect()
        {
            Console.WriteLine("Attempting to connect to twitch chat");
            Client = new TwitchClient();
            Client.Initialize(_credentials, StartService.ChannelName);
            Client.Connect();
        }

    }

    internal class PubSubConnection
    {
        public static TwitchPubSub PubSubClient;
        internal void Connect()
        {
            PubSubClient = new TwitchPubSub();
            PubSubClient.Connect();
            PubSubClient.OnPubSubServiceConnected += PubSubConnected;
            PubSubClient.OnListenResponse += PubSubClientOnListenResponse;

            void PubSubConnected(object sender, EventArgs e)
            {
                Console.WriteLine("[PubSub] Connected");
                PubSubClient.ListenToBitsEvents(TwitchApiConnection.GetChannelId());
                PubSubClient.SendTopics(StartService.PubSubOAuth); //NEEDS AUTH CODE OF STREAMER
            }

            void PubSubClientOnListenResponse(object sender, TwitchLib.PubSub.Events.OnListenResponseArgs e)
            {
                if (e.Successful)
                    Console.WriteLine($"Successfully verified listening to topic: {e.Topic}");
                else
                    Console.WriteLine($"Failed to listen! Error: {e.Response.Error}");
            }
        }
    }

    internal class TwitchApiConnection
    {
        public static TwitchAPI ApiClient;

        internal void Connect()
        {
            ApiClient = new TwitchAPI();
            ApiClient.Settings.ClientId = StartService.TwitchAPIOAuth;

        }

        public static string GetChannelId()
        {
            var userList = ApiClient.Users.v5.GetUserByNameAsync(StartService.ChannelName).Result.Matches;
            return userList[0].Id;
        }
    }

}


