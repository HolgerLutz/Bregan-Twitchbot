using System;
using TwitchLib.Api;
using TwitchLib.Api.Exceptions;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.PubSub;

namespace BreganTwitchBot.Connection
{
    class TwitchBotConnection
    {
        public static TwitchClient Client;
        private readonly ConnectionCredentials _credentials = new ConnectionCredentials(StartService.BotName, StartService.BotOAuth);

        internal void Connect()
        {
            Console.WriteLine("Attempting to connect to twitch chat");
            Client = new TwitchClient();
            Client.Initialize(_credentials, StartService.ChannelName);
            Client.Connect();

            Client.OnConnected += OnConnectedToChannel;
        }

        private static void OnConnectedToChannel(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            Client.SendMessage(StartService.ChannelName,"Successfully connected!");
            Console.WriteLine($"[Bot Connection] {DateTime.Now}: Bot successfully connected");
        }
    }

    class PubSubConnection
    {
        public static TwitchPubSub PubSubClient;

        internal void Connect()
        {
            PubSubClient = new TwitchPubSub();
            PubSubClient.OnPubSubServiceConnected += PubSubConnected;
            PubSubClient.OnListenResponse += PubSubClientOnListenResponse;
            PubSubClient.ListenToBitsEvents(StartService.TwitchChannelID);
            PubSubClient.Connect();

            void PubSubConnected(object sender, EventArgs e)
            {
                Console.WriteLine($"[PubSub] {DateTime.Now}: Connected");
                PubSubClient.SendTopics(StartService.PubSubOAuth);
            }

            void PubSubClientOnListenResponse(object sender, TwitchLib.PubSub.Events.OnListenResponseArgs e)
            {
                if (e.Successful)
                    Console.WriteLine($"[PubSub] {DateTime.Now}: Successfully verified listening to topic: {e.Topic}");
                else
                    Console.WriteLine($"[PubSub] {DateTime.Now}: Failed to listen! Error: {e.Response.Error}");
            }
        }
    }

    class TwitchApiConnection
    {
        public static TwitchAPI ApiClient;

        internal void Connect()
        {
            try
            {
                ApiClient = new TwitchAPI();
                ApiClient.Settings.ClientId = StartService.TwitchAPIOAuth;
            }
            catch (BadGatewayException)
            {
                Console.WriteLine("[Twitch API Connection] BadGatewayException Error connecting to the Twitch API");
                throw;
            }
            catch (InternalServerErrorException)
            {
                Console.WriteLine("[Twitch API Connection] InternalServerErrorException Error connecting to the Twitch API");
                throw;
            }

        }
    }

}


