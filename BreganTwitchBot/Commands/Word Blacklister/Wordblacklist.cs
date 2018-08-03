using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using Bregan_TwitchBot.Commands.Message_Limiter;
using Bregan_TwitchBot.Connection;
using TwitchLib.Client.Extensions;

namespace Bregan_TwitchBot.Commands.Word_Blacklister
{
    internal class WordBlackList
    {
        private static List<string> _blacklistedWords;
        private static string _folderPath;
        private static string _badWordFilePath;
        private static TwitchLib.Api.Services.FollowerService _followerService;

        public static void StartBlacklist() //TODO: Throw exceptions if the files are missing but the config file exists
        {
            _folderPath = Directory.GetCurrentDirectory();
            _badWordFilePath = Path.Combine(_folderPath, "Config/blacklistedwords.txt"); //Some of the words may produce false bans in everyday chat
            if (!Directory.Exists(Path.Combine(_folderPath, "Config"))) //If the path doesn't exist then create it & create file
            {
                Directory.CreateDirectory(Path.Combine(_folderPath, "Config"));
                File.Create(_badWordFilePath).Dispose();
                Console.WriteLine($"[Word Blacklist] {DateTime.Now}: File successfully created");
                TwitchBotConnection.Client.OnMessageReceived += MessageRecieved;
                return;
            }

            try
            {
                _blacklistedWords = new List<string>(File.ReadAllLines(_badWordFilePath).ToList());
                Console.WriteLine($"[Word Blacklist] {DateTime.Now}: Words and names successfully loaded");
                TwitchBotConnection.Client.OnMessageReceived += MessageRecieved;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"[Word Blacklist] {DateTime.Now}: File not found... Creating new file");
                Directory.CreateDirectory(Path.Combine(_folderPath, "Config"));
                File.Create(_badWordFilePath).Dispose();
                Console.WriteLine($"[Word Blacklist] {DateTime.Now}: File successfully created");
            }

            _followerService = new TwitchLib.Api.Services.FollowerService(TwitchApiConnection.ApiClient, 10);
            _followerService.SetChannelByChannelId(StartService.TwitchChannelID);
            _followerService.OnServiceStarted += FollowerServiceStarted;
            _followerService.OnNewFollowersDetected += NewFollowers;
            _followerService.StartService();
        }

        //Bad words
        public static void AddBadWord(string word)
        {
            var badWord = word.Remove(0, 12).Replace(" ", "").ToLower();

            if (_blacklistedWords.Contains(badWord))
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName,"That word is already on the blacklist");
                CommandLimiter.AddMessageCount();
                return;
            }
            _blacklistedWords.Add(badWord);
            File.AppendAllText(_badWordFilePath, badWord + Environment.NewLine);
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName,"Word successfully blacklisted");
            Console.WriteLine($"[Word Blacklist] {DateTime.Now}: {badWord} has been blacklisted");
        }

        public static void RemoveBadWord(string word)
        {
            var badWord = word.Remove(0, 15).Replace(" ", "").ToLower();
            Console.WriteLine(badWord);
            if (!_blacklistedWords.Contains(badWord))
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "That word is not blacklisted");
                CommandLimiter.AddMessageCount();
                return;
            }
            _blacklistedWords.Remove(badWord);
            File.WriteAllText(_badWordFilePath, String.Empty); //Empty the file to re-write all the words minus the removed

            foreach (var badword in _blacklistedWords)
            {
                File.AppendAllText(_badWordFilePath, badword + Environment.NewLine); //Add each word back in except from the removed one
            }
            Console.WriteLine($"[Word Blacklist] {DateTime.Now}: {badWord} has been removed from the blacklist");
        }

        private static void MessageRecieved(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            var message = Regex.Replace(e.ChatMessage.Message, @"[^\w\d]|,|_| |","").ToLower();

            foreach (var badword in _blacklistedWords)
            {
                if (message.Contains(badword) || e.ChatMessage.Username.Replace("_","").ToLower().Contains(badword))
                {
                    TwitchBotConnection.Client.BanUser(e.ChatMessage.Username);
                    Console.WriteLine($"[Bad Words] BAD WORD DETECTED {badword} was sent by {e.ChatMessage.Username}");
                    return;
                }
            }
        }
        private static void FollowerServiceStarted(object sender, TwitchLib.Api.Services.Events.FollowerService.OnServiceStartedArgs e)
        {
            Console.WriteLine($"[Follower Service] {DateTime.Now} Follower service started");
        }

        private static void NewFollowers(object sender, TwitchLib.Api.Services.Events.FollowerService.OnNewFollowersDetectedArgs e)
        {
            foreach (var newFollower in e.NewFollowers.ToArray())
            {
                Console.WriteLine($"new follower: {newFollower.User.DisplayName}");

                foreach (var badword in _blacklistedWords)
                {
                    if (newFollower.User.DisplayName.Replace("_", "").ToLower().Contains(badword))
                    {
                        TwitchBotConnection.Client.BanUser(newFollower.User.DisplayName);
                    }
                }
            }
        }
    }
}
