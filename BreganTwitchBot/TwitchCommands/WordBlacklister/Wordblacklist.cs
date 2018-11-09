using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BreganTwitchBot.Connection;
using BreganTwitchBot.Database;
using BreganTwitchBot.TwitchCommands.MessageLimiter;
using Serilog;
using TwitchLib.Client.Extensions;

namespace BreganTwitchBot.TwitchCommands.WordBlacklister
{
    class WordBlackList
    {
        private static List<string> _blacklistedWords;
        private static DatabaseQueries _databaseQuery;
        private static CommandLimiter _commandLimiter;

        public static void StartBlacklist() //TODO: Throw exceptions if the files are missing but the config file exists
        {
            _commandLimiter = new CommandLimiter();
            _databaseQuery = new DatabaseQueries();

            _blacklistedWords = new List<string>(_databaseQuery.LoadBlacklistedWords());
            TwitchBotConnection.Client.OnMessageReceived += MessageReceived;
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
            _databaseQuery.AddBlacklistedItem(word, "word");
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
            _databaseQuery.RemoveBlacklistedItem(word);
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
