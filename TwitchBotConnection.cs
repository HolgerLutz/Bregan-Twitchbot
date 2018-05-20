using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Models.v5.Channels;
using TwitchLib.Api.Models.v5.Subscriptions;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace Twitch_Bot
{
    internal class TwitchBotConnection
    {
        public static TwitchClient client;
        public static TwitchAPI apiClient;
        public const string channelConnectName = "blocksssssss";

        //TEST ACCOUNT DETAILS: readonly ConnectionCredentials credentials = new ConnectionCredentials("dadbot7373", "oauth:iwsasga1qzxvbhcs5q5jllk3v1pzhy");
        readonly ConnectionCredentials credentials = new ConnectionCredentials("blocksssssss", "oauth:1zpurobffxjmpu5l9h81cxtat0bjyf");
        internal void Connect()
        {
            Console.WriteLine("Attempting to connect to twitch chat");
            client = new TwitchClient();
            client.Initialize(credentials, channelConnectName);
            client.Connect();

            apiClient = new TwitchAPI();
            apiClient.Settings.ClientId = "krn1npp2luabpkdalei2nenf8nf8n4vep";
            TwitchBotGeneralMessages.TwitchMessageSetup();
            BotLogging.BotLoggingStart();
        }

    }
}
