using System;
using System.Threading.Tasks;
using BreganTwitchBot.Connection;
using BreganTwitchBot.Database;
using BreganTwitchBot.Discord;
using Discord;
using Discord.WebSocket;

namespace BreganTwitchBot.TwitchCommands.SongRequests
{
    class SongRequest
    {
        public static void SendSong(string song, string username)
        {
            DatabaseQueries.RemoveUserPoints(username, 3000);
            DiscordConnection.SendSongMessage(StartService.DiscordEventChannelID, $"{song} has been sent in by {username}");
            DiscordConnection.DiscordClient.ReactionAdded += DiscordClient_ReactionAdded;

        }

        private static Task DiscordClient_ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            if (messageChannel.Id == StartService.DiscordEventChannelID && reaction.UserId == 219623957957967872)
            {
                message.GetOrDownloadAsync().Result.DeleteAsync();
            }
            return Task.CompletedTask;
        }
    }
}
