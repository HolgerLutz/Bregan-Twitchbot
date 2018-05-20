using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.PubSub.Events;

namespace Twitch_Bot
{
    internal class TwitchBotGeneralMessages
    {
        public static void TwitchMessageSetup()
        {
            TwitchBotConnection.client.OnConnected += BotConnectedToChannel;
            TwitchBotConnection.client.OnNewSubscriber += NewSub;
            TwitchBotConnection.client.OnReSubscriber += Resub;
            TwitchBotConnection.client.OnGiftedSubscription += GiftSub;
        }

        private static void GiftSub(object sender, TwitchLib.Client.Events.OnGiftedSubscriptionArgs e)
        {
            TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName, "PogChamp thank you " + e.GiftedSubscription.DisplayName + " for gifting " + e.GiftedSubscription.MsgParamRecipientUserName + " a sub PogChamp <3 <3 <3");
        }

        private static void Resub(object sender, TwitchLib.Client.Events.OnReSubscriberArgs e)
        {
            TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName, "Welcome back " + e.ReSubscriber.DisplayName + " for " + e.ReSubscriber.Months + " with the message " + e.ReSubscriber.ResubMessage + " <3 <3 <3 <3");
        }

        private static void NewSub(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
        {
            TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName, "Welcome " + e.Subscriber.DisplayName + " to the " + TwitchBotConnection.channelConnectName + " squad!");
        }

        private static void BotConnectedToChannel(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName, "Successfully connected");
        }


    }
}
