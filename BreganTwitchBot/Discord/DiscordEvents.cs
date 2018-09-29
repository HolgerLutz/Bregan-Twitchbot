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

            var timer = new Timer(20000);
            timer.Start();
            timer.Elapsed += Timer_Elapsed;
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
