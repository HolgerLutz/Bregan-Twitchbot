using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BreganTwitchBot.Connection;
using BreganTwitchBot.TwitchCommands.MessageLimiter;
using Serilog;
using TwitchLib.Client.Extensions;

namespace BreganTwitchBot.TwitchCommands.WordBlacklister
{
    class WordBlackList
    {
        private static List<string> _blacklistedWords;
        private static string _folderPath;
        private static string _badWordFilePath;
        private static CommandLimiter _commandLimiter;

        public static void StartBlacklist() //TODO: Throw exceptions if the files are missing but the config file exists
        {
            _folderPath = Directory.GetCurrentDirectory();
            _badWordFilePath = Path.Combine(_folderPath, "Config/blacklistedwords.txt"); //Some of the words may produce false bans in everyday chat
            _commandLimiter = new CommandLimiter();

            if (!Directory.Exists(Path.Combine(_folderPath, "Config"))) //If the path doesn't exist then create it & create file
            {
                Directory.CreateDirectory(Path.Combine(_folderPath, "Config"));
                File.Create(_badWordFilePath).Dispose();
                Log.Information("[Word Blacklist] File successfully created");
                TwitchBotConnection.Client.OnMessageReceived += MessageReceived;
                return;
            }
            try
            {
                _blacklistedWords = new List<string>(File.ReadAllLines(_badWordFilePath).ToList());
                Log.Information("[Word Blacklist] Words and names successfully loaded");
                TwitchBotConnection.Client.OnMessageReceived += MessageReceived;
            }
            catch (FileNotFoundException)
            {
                Log.Warning("[Word Blacklist] File not found... Creating new file");
                Directory.CreateDirectory(Path.Combine(_folderPath, "Config"));
                File.Create(_badWordFilePath).Dispose();
                Log.Information("[Word Blacklist] File successfully created");
            }
            PubSubConnection.PubSubClient.OnFollow += OnFollow;
        }

        private static void OnFollow(object sender, TwitchLib.PubSub.Events.OnFollowArgs e)
        {
            Log.Information($"[New Twitch Follow] {e.Username} just followed!");
            foreach (var badWord in _blacklistedWords)
            {
                if (e.DisplayName.Replace("_", "").ToLower().Contains(badWord))
                {
                    TwitchBotConnection.Client.BanUser(StartService.ChannelName, e.Username, "offensive name");
                    Log.Information($"[Word Blacklist] {e.DisplayName} has been banned for having an offensive name");
                    return;
                }
            }
        }

        //Bad words
        public void AddBadWord(string word)
        {
            if (_blacklistedWords.Contains(word))
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName,"That word is already on the blacklist");
                _commandLimiter.AddMessageCount();
                return;
            }
            _blacklistedWords.Add(word);
            File.AppendAllText(_badWordFilePath, word + Environment.NewLine);
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName,"Word successfully blacklisted");
            Log.Information($"[Word Blacklist] {word} has been blacklisted");
        }

        public void RemoveBadWord(string word)
        {
            if (!_blacklistedWords.Contains(word))
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "That word is not blacklisted");
                _commandLimiter.AddMessageCount();
                return;
            }
            _blacklistedWords.Remove(word);
            File.WriteAllText(_badWordFilePath, String.Empty); //Empty the file to re-write all the words minus the removed

            foreach (var badword in _blacklistedWords)
            {
                File.AppendAllText(_badWordFilePath, badword + Environment.NewLine); //Add each word back in except from the removed one
            }
            Log.Information($"[Word Blacklist] {word} has been removed from the blacklist");
        }

        private static void MessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            var message = Regex.Replace(e.ChatMessage.Message, @"[^\w\d]|,|_| |","").ToLower();

            if (_blacklistedWords.Capacity == 0)
            {
                return;
            }

            foreach (var badWord in _blacklistedWords) 
            {
                if (message.Contains(badWord) || e.ChatMessage.Username.Replace("_","").ToLower().Contains(badWord))
                {
                    TwitchBotConnection.Client.BanUser(StartService.ChannelName, e.ChatMessage.Username, "offensive word");
                    Log.Warning($"[Word Blacklist] BAD WORD DETECTED {badWord} was sent by {e.ChatMessage.Username}");
                    return;
                }
            }
        }
    }
}
