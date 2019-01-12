using System;
using System.Threading.Tasks;
using BreganTwitchBot.Connection;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace BreganTwitchBot.Discord
{
    class DiscordConnection
    {
        public static DiscordSocketClient DiscordClient;

        public static async Task MainAsync()
        {

            DiscordClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = 2000
            });

            await DiscordClient.LoginAsync(TokenType.Bot, StartService.DiscordAPIKey);
            await DiscordClient.SetGameAsync("Botting");
            await DiscordClient.StartAsync();

            await Task.Delay(-1);
        }

        public static async Task SendMessage(ulong channelID, string message)
        {
            var channel = DiscordClient.GetChannel(channelID) as IMessageChannel;
            await channel.SendMessageAsync(message);
        }

        public static void SendSongMessage(ulong channelID, string message)
        {
            try
            {
                var channel = DiscordClient.GetChannel(channelID) as IMessageChannel;
                var sentMessage = channel.SendMessageAsync(message).Result;
                Log.Information($"[Discord Song Request Sent] {message}");
                sentMessage.AddReactionAsync(new Emoji("👍"));
                sentMessage.AddReactionAsync(new Emoji("👎"));
            }
            catch (Exception)
            {
                Log.Fatal("[Discord Message]: An error has occured when trying to send the song request. Is Discord down?");
            }
        }

    }
}