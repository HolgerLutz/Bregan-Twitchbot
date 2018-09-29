using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
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
        private static string _folderPath;
        private static string _blacklistedSongsFilePath;
        private static CommandLimiter _commandLimiter;
        private static DatabaseQueries _databaseQuery;

        public static void SongRequestSetup()
        {
            _folderPath = Directory.GetCurrentDirectory();
            _blacklistedSongsFilePath = Path.Combine(_folderPath, "Config/blacklistedsongs.txt");
            _commandLimiter = new CommandLimiter();
            _databaseQuery = new DatabaseQueries();

            if (!File.Exists(_blacklistedSongsFilePath)) //If the path doesn't exist then create it & create file
            {
                File.Create(_blacklistedSongsFilePath).Dispose();
                Log.Information("[Song Blacklist] File successfully created");
            }

            try
            {
                _blacklistedSongList = new List<string>(File.ReadAllLines(_blacklistedSongsFilePath).ToList());
                Log.Information("[Song Blacklist] Blacklisted songs successfully loaded");
            }
            catch (FileNotFoundException)
            {
                Log.Warning("[Song Blacklist] File not found... Creating new file");
                Directory.CreateDirectory(Path.Combine(_folderPath, "Config"));
                File.Create(_blacklistedSongsFilePath).Dispose();
                Log.Information("[Song Blacklist] File successfully created");
            }

            DiscordConnection.DiscordClient.ReactionAdded += DiscordClient_ReactionAdded;

        }

        public void AddBlacklistedSong(string song)
        {
            if (_blacklistedSongList.Contains(song))
            {
                var message = "That song is already on the blacklist";
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                Log.Information($"[Twitch Message Sent] {message}");
                _commandLimiter.AddMessageCount();
                return;
            }

            _blacklistedSongList.Add(song);
            File.AppendAllText(_blacklistedSongsFilePath, song + Environment.NewLine);
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "song successfully blacklisted");
            Log.Information($"[Song Blacklist] {song} has been blacklisted");
        }

        public bool IsSongBlacklisted(string song)
        {
            return _blacklistedSongList.Contains(song);
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
                var message = $"@{username} => You are on cooldown. You can next request a song in {coolDownLeft.Minutes} minutes {coolDownLeft.Seconds} seconds";
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                Log.Information($"[Twitch Message Sent] {message}");
                _commandLimiter.AddMessageCount();
                return false;
            }
            return true;
        }
    }
}
    