using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BreganTwitchBot.Connection;
using BreganTwitchBot.Database;
using BreganTwitchBot.Discord;
using BreganTwitchBot.TwitchCommands.MessageLimiter;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace BreganTwitchBot.TwitchCommands.SongRequests
{
    class SongRequest
    {
        private static List<string> _blacklistedSongList;
        private static CommandLimiter _commandLimiter;
        private static DatabaseQueries _databaseQuery;

        public static void SongRequestSetup()
        {
            _databaseQuery = new DatabaseQueries();
            _commandLimiter = new CommandLimiter();
            _blacklistedSongList = new List<string>(_databaseQuery.LoadBlacklistedSongs());
            Log.Information("[Song Blacklist] Songs successfully loaded");
            DiscordConnection.DiscordClient.ReactionAdded += DiscordClient_ReactionAdded;
        }

        public void AddBlacklistedSong(string song)
        {
            var youtubeId = GetYoutubeId(song);

            if (_blacklistedSongList.Contains(youtubeId))
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "That song is already on the blacklist");
                _commandLimiter.AddMessageCount();
                return;
            }

            _blacklistedSongList.Add(youtubeId);
            _databaseQuery.AddBlacklistedItem(youtubeId, "song");
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "song successfully blacklisted");
            Log.Information($"[Song Blacklist] {song} has been blacklisted");
        }

        public bool IsSongBlacklisted(string song)
        {
            var youtubeId = GetYoutubeId(song);
            return _blacklistedSongList.Contains(youtubeId);
        }

        public void SendSong(string song, string username, int points)
        {
            _databaseQuery.RemoveUserPoints(username, points);
            DiscordConnection.SendSongMessage(StartService.DiscordEventChannelID, $"{song} has been sent in by {username}");
        }

        private static Task DiscordClient_ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            if (messageChannel.Id == StartService.DiscordEventChannelID && reaction.UserId == StartService.DiscordUsernameID && reaction.Emote.Name == "👍")
            {
                message.GetOrDownloadAsync().Result.DeleteAsync();
            }
            else if (messageChannel.Id == StartService.DiscordEventChannelID && reaction.UserId == StartService.DiscordUsernameID && reaction.Emote.Name == "👎")
            {
                var messageContents = message.GetOrDownloadAsync().Result.Content.Split(' ');
                var blacklistSong = new SongRequest();
                blacklistSong.AddBlacklistedSong(messageContents[0]);
                message.GetOrDownloadAsync().Result.DeleteAsync();
            }

            return Task.CompletedTask;
        }

        public bool CheckCooldown(string username, int srCooldown)
        {
            if (DateTime.Now - TimeSpan.FromMinutes(srCooldown) <= _databaseQuery.GetLastSongRequest(username))
            {
                var sinceListSr = DateTime.Now - _databaseQuery.GetLastSongRequest(username);
                var coolDownLeft = TimeSpan.FromMinutes(srCooldown) - sinceListSr;
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{username} => You are on cooldown. You can next request a song in {coolDownLeft.Minutes} minutes {coolDownLeft.Seconds} seconds");
                _commandLimiter.AddMessageCount();
                return false;
            }
            return true;
        }

        public string GetYoutubeId(string youtubeId)
        {
            var uri = new Uri(youtubeId);
            var query = HttpUtility.ParseQueryString(uri.Query);

            string videoId;

            if (query.AllKeys.Contains("v"))
            {
                //Youtube video IDs always start with v= so need to query just that section
                videoId = query["v"];
                return videoId;
            }
            //Links like you.be do not have v in the name so just return the last segment
            videoId = uri.Segments.Last(); 
            return videoId;
        }
    }
}
    