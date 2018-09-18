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

namespace BreganTwitchBot.TwitchCommands.SongRequests
{
    class SongRequest
    {
        private static List<string> _blacklistedSongList;
        private static string _folderPath;
        private static string _blacklistedSongsFilePath;

        public static void SongRequestSetup()
        {
            _folderPath = Directory.GetCurrentDirectory();
            _blacklistedSongsFilePath = Path.Combine(_folderPath, "Config/blacklistedsongs.txt");

            if (!File.Exists(_blacklistedSongsFilePath)) //If the path doesn't exist then create it & create file
            {
                File.Create(_blacklistedSongsFilePath).Dispose();
                Console.WriteLine($"[Song Blacklist] {DateTime.Now}: File successfully created");
            }

            try
            {
                _blacklistedSongList = new List<string>(File.ReadAllLines(_blacklistedSongsFilePath).ToList());
                Console.WriteLine($"[Song Blacklist] {DateTime.Now}: blacklisted songs successfully loaded");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"[Song Blacklist] {DateTime.Now}: File not found... Creating new file");
                Directory.CreateDirectory(Path.Combine(_folderPath, "Config"));
                File.Create(_blacklistedSongsFilePath).Dispose();
                Console.WriteLine($"[Song Blacklist] {DateTime.Now}: File successfully created");
            }

            DiscordConnection.DiscordClient.ReactionAdded += DiscordClient_ReactionAdded;

        }

        public static void AddBlacklistedSong(string song)
        {
            if (_blacklistedSongList.Contains(song))
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "That song is already on the blacklist");
                messageLimter.AddMessageCount();
                return;
            }

            _blacklistedSongList.Add(song);
            File.AppendAllText(_blacklistedSongsFilePath, song + Environment.NewLine);
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "song successfully blacklisted");
            Console.WriteLine($"[Song Blacklist] {DateTime.Now}: {song} has been blacklisted");
        }

        public static bool IsSongBlacklisted(string song)
        {
            return _blacklistedSongList.Contains(song);
        }

        public static void SendSong(string song, string username)
        {
            DatabaseQueries.RemoveUserPoints(username, 3000);
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
                AddBlacklistedSong(messageContents[0]);
                message.GetOrDownloadAsync().Result.DeleteAsync();
            }

            return Task.CompletedTask;
        }

        public static bool CheckCooldown(string username, int srCooldown)
        {
            if (DateTime.Now - TimeSpan.FromMinutes(srCooldown) <= DatabaseQueries.GetLastSongRequest(username))
            {
                var sinceListSr = DateTime.Now - DatabaseQueries.GetLastSongRequest(username);
                var coolDownLeft = TimeSpan.FromMinutes(srCooldown) - sinceListSr;
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{username} => You are on cooldown. You can next request a song in {coolDownLeft.Minutes} minutes {coolDownLeft.Seconds}");
                messageLimter.AddMessageCount();
                return false;
            }
            return true;
        }
    }
}
