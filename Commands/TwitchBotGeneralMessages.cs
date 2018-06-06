using System;
using Bregan_TwitchBot.Connection;

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
            TwitchBotConnection.Client.OnBeingHosted += Client_OnBeingHosted;
            //PubSubConnection.pubSubClient.OnBitsReceived += BitsDonated; Needs channel auth owner
        }

        private static void Client_OnBeingHosted(object sender, TwitchLib.Client.Events.OnBeingHostedArgs e)
        {
            Console.WriteLine("TEST");
        }

        //Gifted subs
        private static void GiftSub(object sender, TwitchLib.Client.Events.OnGiftedSubscriptionArgs e)
        {
            TwitchBotConnection.Client.SendMessage(TwitchBotConnection.ChannelConnectName, $"Thank you {e.GiftedSubscription.DisplayName} for gifting {e.GiftedSubscription.MsgParamRecipientUserName} a sub PogChamp <3 <3 <3 blocksWOT");
        }
        //Resubscriptions
        private static void Resub(object sender, TwitchLib.Client.Events.OnReSubscriberArgs e)
        {
            TwitchBotConnection.Client.SendMessage(TwitchBotConnection.ChannelConnectName, $"Welcome back {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months} months with the message {e.ReSubscriber.ResubMessage} <3 <3 <3 <3 blocksWOT");
        }
        //New subscription
        private static void NewSub(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
        {
            TwitchBotConnection.Client.SendMessage(TwitchBotConnection.ChannelConnectName, $"Welcome {e.Subscriber.DisplayName} to the {TwitchBotConnection.ChannelConnectName} squad! blocksWOT");
        }
        //Connection
        private static void BotConnectedToChannel(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            TwitchBotConnection.Client.SendMessage(TwitchBotConnection.ChannelConnectName, "Successfully connected blocksWOT");
            Console.WriteLine("[Twitch Connection] Bot Sucessfully connected");
        }

        
        //Bits donations --------------- NEEDS auth from channel owner
//        private static void BitsDonated(object sender, OnBitsReceivedArgs e)
//        {
//            Console.WriteLine($"Just received {e.BitsUsed} bits from {e.Username}. That brings their total to {e.TotalBitsUsed} bits!");
//            TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName, $"hoooo {e.Username} has donated + {e.BitsUsed} with a grand total of {e.TotalBitsUsed} donated PogChamp");
//        }

    }
}
