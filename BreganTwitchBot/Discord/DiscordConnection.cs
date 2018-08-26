using System.Threading.Tasks;
using BreganTwitchBot.Connection;
using Discord;
using Discord.WebSocket;

namespace BreganTwitchBot.Discord
{
    class DiscordConnection
    {
        public static DiscordSocketClient DiscordClient;

        public static async Task MainAsync() 
        {
            DiscordClient = new DiscordSocketClient();

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
            var channel = DiscordClient.GetChannel(channelID) as IMessageChannel;
            channel.SendMessageAsync(message).Result.AddReactionAsync(new Emoji("💩"));
        }

    }
}