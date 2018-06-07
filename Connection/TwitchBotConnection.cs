using System;
using Bregan_TwitchBot.Commands;
using Bregan_TwitchBot.Commands.Queue;
using Bregan_TwitchBot.Logging;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.PubSub;

namespace Bregan_TwitchBot.Connection
{
    internal class TwitchBotConnection
    {
        public static TwitchClient Client;
        public const string ChannelConnectName = "blocksssssss";

        //TEST ACCOUNT DETAILS: readonly ConnectionCredentials credentials = new ConnectionCredentials("dadbot7373", "oauth:iwsasga1qzxvbhcs5q5jllk3v1pzhy");
        private readonly ConnectionCredentials _credentials = new ConnectionCredentials("blocksssssssbot", "oauth:p2lf4nhslc9a96vd3bj9aa11xnka9q");
        internal void Connect()
        {
            Console.WriteLine("Attempting to connect to twitch chat");
            Client = new TwitchClient();
            Client.Initialize(_credentials, ChannelConnectName);
            Client.Connect();

            //Connect PubSub and API

            //var pubsub = new PubSubConnection();
            var twitchApi = new TwitchApiConnection();
            twitchApi.Connect();
            //pubsub.Connect();

            //Logging

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
                Console.WriteLine("Connected?");
                PubSubClient.ListenToBitsEvents(TwitchApiConnection.GetChannelId());
                PubSubClient.SendTopics(""); //NEEDS AUTH CODE OF STREAMER
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
            ApiClient.Settings.ClientId = "zupx1ka4amj0nbyactajcdcup08hkq";

        }

        public static string GetChannelId()
        {
            var userList = ApiClient.Users.v5.GetUserByNameAsync(TwitchBotConnection.ChannelConnectName).Result.Matches;
            return userList[0].Id;
        }
    }

}


