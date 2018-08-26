using System;
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
        private static bool _streamAnnounced;

        public static void StartDiscordAlerts()
        {
            DiscordConnection.DiscordClient.Log += Log;
            DiscordConnection.DiscordClient.UserJoined += UserJoined;
            DiscordConnection.DiscordClient.UserLeft += UserLeft;

            var timer = new Timer(20000);
            timer.Start();
            timer.Elapsed += Timer_Elapsed;
            _streamAnnounced = false;
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var streamAnnounced = TwitchApiConnection.ApiClient.Streams.v5.BroadcasterOnlineAsync(StartService.TwitchChannelID).Result;
                if (streamAnnounced && _streamAnnounced == false)
                {
                    DiscordConnection.SendMessage(StartService.DiscordAnnouncementChannelID, $"hey @everyone! {StartService.ChannelName} has gone live! Tune in at https://www.twitch.tv/{StartService.ChannelName} !");
                    _streamAnnounced = true;
                }
                else if (streamAnnounced == false && _streamAnnounced)
                {
                    _streamAnnounced = false;
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
