using System;
using Serilog;
using TwitchLib.Api;
using TwitchLib.Api.Core.Exceptions;
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
            Log.Information("Attempting to connect to twitch chat");
            Client = new TwitchClient();
            Client.Initialize(_credentials, StartService.ChannelName);
            Client.Connect();

            Client.OnConnected += OnConnectedToChannel;
            Client.OnDisconnected += OnDisconnectedToChannel;
        }

        private static void OnDisconnectedToChannel(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
        {
            Log.Information("[Bot Connection] Bot disconnected from channel");
        }

        private static void OnConnectedToChannel(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            Client.SendMessage(StartService.ChannelName,"Successfully connected!");
            Log.Information("[Bot Connection] Bot successfully connected");
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
            PubSubClient.ListenToFollows(StartService.TwitchChannelID);
            PubSubClient.Connect();
            
            void PubSubConnected(object sender, EventArgs e)
            {
                Log.Information("[PubSub] Connected");
                PubSubClient.SendTopics(StartService.PubSubOAuth);
            }

            void PubSubClientOnListenResponse(object sender, TwitchLib.PubSub.Events.OnListenResponseArgs e)
            {
                if (e.Successful)
                {
                    Log.Information($"[PubSub] Successfully verified listening to topic: {e.Topic}");
                }
                else
                {
                    Log.Fatal($"[PubSub] Failed to listen! Error: {e.Response.Error}");
                }
                    
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
            catch (BadGatewayException) //These are the two main errors that occur when the Twitch API goes down
            {
                Log.Fatal("[Twitch API Connection] BadGatewayException Error connecting to the Twitch API");
            }
            catch (InternalServerErrorException)
            {
                Log.Fatal("[Twitch API Connection] InternalServerErrorException Error connecting to the Twitch API");
            }

        }
    }

}


