using System;
using BreganTwitchBot.Connection;
using BreganTwitchBot.Database;
using BreganTwitchBot.TwitchCommands.MessageLimiter;
using Serilog;
using TwitchLib.Client.Enums;
using TwitchLib.PubSub.Events;

namespace BreganTwitchBot.TwitchCommands
{
    class TwitchBotGeneralMessages
    {
        private static DatabaseQueries _databaseQuery;

        public static void TwitchMessageSetup()
        {
            _databaseQuery = new DatabaseQueries();

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
            switch (e.GiftedSubscription.MsgParamSubPlan)
            {
                case SubscriptionPlan.NotSet:
                case SubscriptionPlan.Tier1:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Thank you {e.GiftedSubscription.DisplayName} for gifting {e.GiftedSubscription.MsgParamRecipientUserName} a sub PogChamp <3");
                    _databaseQuery.AddUserPoints(e.GiftedSubscription.DisplayName.ToLower(), 15000);
                    _databaseQuery.AddUserPoints(e.GiftedSubscription.MsgParamRecipientUserName.ToLower(), 4000);
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Tier2:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Thank you {e.GiftedSubscription.DisplayName} for gifting {e.GiftedSubscription.MsgParamRecipientDisplayName} a tier 2 sub PogChamp PogChamp <3 <3");
                    _databaseQuery.AddUserPoints(e.GiftedSubscription.DisplayName.ToLower(), 30000);
                    _databaseQuery.AddUserPoints(e.GiftedSubscription.MsgParamRecipientUserName.ToLower(), 4000);
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Tier3:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Thank you {e.GiftedSubscription.DisplayName} for gifting {e.GiftedSubscription.MsgParamRecipientDisplayName} a tier 3 sub PogChamp PogChamp PogChamp <3 <3 <3");
                    _databaseQuery.AddUserPoints(e.GiftedSubscription.DisplayName.ToLower(), 45000);
                    _databaseQuery.AddUserPoints(e.GiftedSubscription.MsgParamRecipientUserName.ToLower(), 4000);
                    CommandLimiter.AddMessageCount();
                    break;
            }
            
        }
        //Resubscriptions
        private static void Resub(object sender, TwitchLib.Client.Events.OnReSubscriberArgs e)
        {
            switch (e.ReSubscriber.SubscriptionPlan)
            {
                case SubscriptionPlan.Tier1:
                case SubscriptionPlan.NotSet:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome back {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months} months! PogChamp <3");
                    _databaseQuery.AddUserPoints(e.ReSubscriber.DisplayName.ToLower(), 10000 + e.ReSubscriber.Months * 2000);
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Tier2:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome back {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months} months with a tier 2 subscription! PogChamp PogChamp <3 <3");
                    _databaseQuery.AddUserPoints(e.ReSubscriber.DisplayName.ToLower(), 20000 + e.ReSubscriber.Months * 2000);
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Tier3:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome back {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months} months with a tier 3 sub! PogChamp PogChamp PogChamp <3 <3 <3");
                    _databaseQuery.AddUserPoints(e.ReSubscriber.DisplayName.ToLower(), 30000 + e.ReSubscriber.Months * 2000);
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Prime:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome back {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months} months with a Twitch Prime sub! PogChamp <3");
                    _databaseQuery.AddUserPoints(e.ReSubscriber.DisplayName.ToLower(), 10000 + e.ReSubscriber.Months * 2000);
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
                    _databaseQuery.AddUserPoints(e.Subscriber.DisplayName.ToLower(), 10000);
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Tier2:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome {e.Subscriber.DisplayName} to the {StartService.ChannelName} squad with a tier 2 sub! PogChamp PogChamp");
                    _databaseQuery.AddUserPoints(e.Subscriber.DisplayName.ToLower(), 20000);
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Tier3:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome {e.Subscriber.DisplayName} to the {StartService.ChannelName} squad with a tier 3 sub! PogChamp PogChamp PogChamp <3 <3 <3");
                    _databaseQuery.AddUserPoints(e.Subscriber.DisplayName.ToLower(), 30000);
                    CommandLimiter.AddMessageCount();
                    break;
                case SubscriptionPlan.Prime:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Welcome {e.Subscriber.DisplayName} to the {StartService.ChannelName} squad with a Twitch Prime sub! PogChamp");
                    _databaseQuery.AddUserPoints(e.Subscriber.DisplayName.ToLower(), 10000);
                    CommandLimiter.AddMessageCount();
                    break;
            }
        }
        //Connection
        private static void BotConnectedToChannel(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "Successfully connected");
            Log.Information("[Twitch Connection] Bot Successfully connected");
        }

      
        private static void BitsDonated(object sender, OnBitsReceivedArgs e)
        {
            Log.Information($"[PubSub] Just received {e.BitsUsed} bits from {e.Username}. That brings their total to {e.TotalBitsUsed} bits!");
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Username} has donated {e.BitsUsed} bits with a grand total of {e.TotalBitsUsed} donated PogChamp You have gained {e.BitsUsed * 30} points");
            _databaseQuery.AddUserPoints(e.Username.ToLower(), e.BitsUsed * 30);
            CommandLimiter.AddMessageCount();
        }
    }
}
