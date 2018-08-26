using System;
using BreganTwitchBot.Connection;
using BreganTwitchBot.TwitchCommands.MessageLimiter;
using TwitchLib.Client.Enums;
using TwitchLib.PubSub.Events;

namespace BreganTwitchBot.TwitchCommands
{
    class TwitchBotGeneralMessages
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
            switch (e.ReSubscriber.SubscriptionPlan)
            {
                case SubscriptionPlan.Tier1:
                case SubscriptionPlan.NotSet:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome back {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months} months! PogChamp <3");
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Tier2:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome back {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months} months with a tier 2 subscription! PogChamp PogChamp <3 <3");
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Tier3:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome back {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months} months with a tier 3 sub! PogChamp PogChamp PogChamp <3 <3 <3");
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Prime:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome back {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months} months with a Twitch Prime sub! PogChamp <3");
                    CommandLimiter.AddMessageCount();
                    break;
            }
        }
        //New subscription
        private static void NewSub(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
        {
            switch (e.Subscriber.SubscriptionPlan)
            {
                case SubscriptionPlan.Tier1:
                case SubscriptionPlan.NotSet:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome {e.Subscriber.DisplayName} to the {StartService.ChannelName} squad! PogChamp");
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Tier2:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome {e.Subscriber.DisplayName} to the {StartService.ChannelName} squad with a tier 2 sub! PogChamp PogChamp");
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Tier3:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome {e.Subscriber.DisplayName} to the {StartService.ChannelName} squad with a tier 3 sub! PogChamp PogChamp PogChamp <3 <3 <3");
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Prime:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome {e.Subscriber.DisplayName} to the {StartService.ChannelName} squad with a Twitch Prime sub! PogChamp");
                    CommandLimiter.AddMessageCount();
                    break;
            }
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
