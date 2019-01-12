 using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Timers;
using BreganTwitchBot.Connection;
using Discord;
using Discord.WebSocket;
using TwitchLib.Api.Core.Exceptions;

namespace BreganTwitchBot.Discord
{
    class DiscordEvents
    {
        public static void StartDiscordAlerts()
        {
            DiscordConnection.DiscordClient.Log += Log;
            DiscordConnection.DiscordClient.UserJoined += UserJoined;
            DiscordConnection.DiscordClient.UserLeft += UserLeft;
            DiscordConnection.DiscordClient.MessageDeleted += MessageDeleted;
            DiscordConnection.DiscordClient.UserIsTyping += DiscordClient_UserIsTyping;
            DiscordConnection.DiscordClient.MessageReceived += MessageReceived;
            var timer = new Timer(20000);
            timer.Start();
            timer.Elapsed += Timer_Elapsed;
        }

        private static Task MessageReceived(SocketMessage message)
        {
            Serilog.Log.Information($"[Discord Message Received] Username: {message.Author} Message: {message.Content} Channel: {message.Channel.Name}");
            return Task.CompletedTask;
        }

        private static Task DiscordClient_UserIsTyping(SocketUser user, ISocketMessageChannel messageChannel)
        {
            Serilog.Log.Information($"[Discord Typing] {user.Username} is typing a message in {messageChannel.Name}");
            return Task.CompletedTask;
        }

        private static Task MessageDeleted(Cacheable<IMessage, ulong> deletedMessage, ISocketMessageChannel arg2)
        {
            if (!deletedMessage.HasValue || deletedMessage.Value.Author.Id == DiscordConnection.DiscordClient.CurrentUser.Id)
            {
                return Task.CompletedTask;
            }
            DiscordConnection.SendMessage(StartService.DiscordEventChannelID, $"Message Deleted: {deletedMessage.Value.Content} \nSent By: {deletedMessage.Value.Author} \nIn Channel: {deletedMessage.Value.Channel.Name} \nDeleted at: {DateTime.Now}");
            Serilog.Log.Information($"[Discord Message Deleted] Message Deleted: {deletedMessage.Value.Content} Sent By: {deletedMessage.Value.Author.Username} In Channel: {deletedMessage.Value.Channel.Name} Deleted at: {DateTime.Now}");
            return Task.CompletedTask;
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var isStreamOn = TwitchApiConnection.ApiClient.V5.Streams.BroadcasterOnlineAsync(StartService.TwitchChannelID).Result;
                if (isStreamOn && StartService.StreamAnnounced == "false")
                {
                    DiscordConnection.SendMessage(StartService.DiscordAnnouncementChannelID, $"hey @everyone! {StartService.ChannelName} has gone live! Tune in at https://www.twitch.tv/{StartService.ChannelName} !");
                    StartService.StreamAnnounced = "true";
                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["StreamAnnounced"].Value = "true";
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                else if (isStreamOn == false && StartService.StreamAnnounced == "true")
                {
                    StartService.StreamAnnounced = "false";
                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["StreamAnnounced"].Value = "false";
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }

            catch (BadGatewayException)
            {
                Serilog.Log.Fatal("[Discord Stream Announcement] A BadGatewayException error has occured while checking if the stream is live.");
                Console.ResetColor();
            }
            catch (InternalServerErrorException)
            {
                Serilog.Log.Fatal("[Discord Stream Announcement] A InternalServerErrorException error has occured while checking if the stream is live.");
                Console.ResetColor();
            }
        }

        private static async Task UserJoined(SocketGuildUser user)
        {
            await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, $"User joined: {user.Username}");
            Serilog.Log.Information($"[Discord] User joined: {user.Username}");
        }

        private static async Task UserLeft(SocketGuildUser user)
        {
            await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, $"User left: {user.Username}");
            Serilog.Log.Information($"[Discord] User left: {user.Username}");
        }

        private static Task Log(LogMessage log)
        {
            Serilog.Log.Information($"[Discord Event Log] {log.ToString()}");
            return Task.CompletedTask;
        }
    }
}
