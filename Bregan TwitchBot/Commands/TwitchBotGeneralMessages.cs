using System;
using System.Collections.Generic;
using System.Linq;
using Bregan_TwitchBot.Commands.Message_Limiter;
using Bregan_TwitchBot.Connection;
using TwitchLib.PubSub.Events;

namespace Bregan_TwitchBot.Commands
{
    internal class TwitchBotGeneralMessages
    {
        public static void TwitchMessageSetup()
        {
            TwitchBotConnection.Client.OnConnected += BotConnectedToChannel;
            TwitchBotConnection.Client.OnNewSubscriber += NewSub;
            TwitchBotConnection.Client.OnReSubscriber += Resub;
            TwitchBotConnection.Client.OnGiftedSubscription += GiftSub;
            if (StartService.PubSubOAuth != "NotSet")
            {
                PubSubConnection.PubSubClient.OnBitsReceived += BitsDonated;
            }
            
        }

        //Gifted subs
        private static void GiftSub(object sender, TwitchLib.Client.Events.OnGiftedSubscriptionArgs e)
        {
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Thank you {e.GiftedSubscription.DisplayName} for gifting {e.GiftedSubscription.MsgParamRecipientUserName} a sub PogChamp <3 <3 <3");
        }
        //Resubscriptions
        private static void Resub(object sender, TwitchLib.Client.Events.OnReSubscriberArgs e)
        {
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome back {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months} months with the message {e.ReSubscriber.ResubMessage} <3 <3 <3 <3");
        }
        //New subscription
        private static void NewSub(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
        {
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome {e.Subscriber.DisplayName} to the {StartService.ChannelName} squad!");
        }
        //Connection
        private static void BotConnectedToChannel(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "Successfully connected");
            Console.WriteLine("[Twitch Connection] Bot Sucessfully connected");
        }

      
        private static void BitsDonated(object sender, OnBitsReceivedArgs e)
        {
            Console.WriteLine($"Just received {e.BitsUsed} bits from {e.Username}. That brings their total to {e.TotalBitsUsed} bits!");
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Username} has donated + {e.BitsUsed} with a grand total of {e.TotalBitsUsed} donated PogChamp");
        }

    }
}
