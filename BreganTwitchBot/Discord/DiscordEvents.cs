using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Timers;
using BreganTwitchBot.Connection;
using Discord;
using Discord.WebSocket;
using TwitchLib.Api.Exceptions;

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
                var isStreamOn = TwitchApiConnection.ApiClient.Streams.v5.BroadcasterOnlineAsync(StartService.TwitchChannelID).Result;
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Discord Stream Announcement] A BadGatewayException error has occured while checking if the stream is live.");
                Console.ResetColor();
                throw;
            }
            catch (InternalServerErrorException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Discord Stream Announcement] A InternalServerErrorException error has occured while checking if the stream is live.");
                Console.ResetColor();
                throw;
            }
        }

        private static async Task UserJoined(SocketGuildUser user)
        {
            await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, $"User joined: {user.Username}");
        }

        private static async Task UserLeft(SocketGuildUser user)
        {
            await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, $"User left: {user.Username}");
        }

        private static Task Log(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
    }
}
