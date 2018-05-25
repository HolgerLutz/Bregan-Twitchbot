using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Models.v5.Channels;
using TwitchLib.Api.Models.v5.Subscriptions;
using TwitchLib.Api.Models.v5.Users;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.PubSub;

namespace Twitch_Bot
{
    internal class TwitchBotConnection
    {
        public static TwitchClient client;
        public const string channelConnectName = "blocksssssss";

        //TEST ACCOUNT DETAILS: readonly ConnectionCredentials credentials = new ConnectionCredentials("dadbot7373", "oauth:iwsasga1qzxvbhcs5q5jllk3v1pzhy");
        readonly ConnectionCredentials credentials = new ConnectionCredentials("blocksssssssbot", "oauth:p2lf4nhslc9a96vd3bj9aa11xnka9q");
        internal void Connect()
        {
            Console.WriteLine("Attempting to connect to twitch chat");
            client = new TwitchClient();
            client.Initialize(credentials, channelConnectName);
            client.Connect();

            var twitchApi = new TwitchApiConnection();
            twitchApi.Connect();

            //var pubsub = new PubSubConnection();
            //pubsub.Connect();

            BotLogging.BotLoggingStart();
            PlayerQueueSystem.QueueCreate();

            TwitchBotGeneralMessages.TwitchMessageSetup();
            PlayerQueueCommands.QueueSystemCommandSetup();
        }

    }

    internal class PubSubConnection
    {
        public static TwitchPubSub pubSubClient;
        internal void Connect()
        {
            pubSubClient = new TwitchPubSub();
            pubSubClient.Connect();
            pubSubClient.OnPubSubServiceConnected += PubSubConnected;
            pubSubClient.OnListenResponse += PubSubClientOnListenResponse;



            void PubSubConnected(object sender, EventArgs e)
            {
                Console.WriteLine("Connected?");
                pubSubClient.ListenToBitsEvents(TwitchApiConnection.GetChannelId());
                pubSubClient.SendTopics("k4bmphnnrihhvhpb9ux82obxv2vthy"); //NEEDS AUTH CODE OF STREAMER
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
        public static TwitchAPI apiClient;

        internal void Connect()
        {
            apiClient = new TwitchAPI();
            apiClient.Settings.ClientId = "zupx1ka4amj0nbyactajcdcup08hkq";

        }

        public static string GetChannelId()
        {
            var userList = apiClient.Users.v5.GetUserByNameAsync(TwitchBotConnection.channelConnectName).Result.Matches;
            return userList[0].Id;
        }
    }

}


