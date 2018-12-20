using System.Collections.Generic;
using BreganTwitchBot.Connection;
using BreganTwitchBot.Database;
using BreganTwitchBot.TwitchCommands.MessageLimiter;
using Serilog;

namespace BreganTwitchBot.TwitchCommands.Supermods
{
    public class Supermod
    {
        private static List<string> _supermodList;

        public static void SupermodSetup()
        {
            var databaseQuery = new DatabaseQueries();
            _supermodList = new List<string>();
            _supermodList = databaseQuery.GetSuperMods();
            Log.Information("[Supermods] Supermods loaded");
        }

        public static void AddSupermod(string username, string commandSender)
        {
            if (_supermodList.Contains(username))
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{commandSender} => {username} is already a supermod!");
                CommandLimiter.AddMessageCount();
                return;
            }

            var databaseQuery = new DatabaseQueries();
            databaseQuery.AddSuperMod(username);
            _supermodList.Add(username);
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{commandSender} => {username} has been added as a supermod!");
            Log.Information($"[Supermods] {username} has been added as a super mod by {commandSender}");
        }

        public static void RemoveSupermod(string username, string commandSender)
        {
            if (_supermodList.Contains(username))
            {
                var databaseQuery = new DatabaseQueries();
                databaseQuery.RemoveSuperMod(username);
                _supermodList.Remove(username);
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{commandSender} => {username} has been removed as a supermod!");
                Log.Information($"[Supermods] {username} has been removed as a super mod by {commandSender}");
                CommandLimiter.AddMessageCount();
                return;
            }

            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{commandSender} => {username} is not a supermod!");
            CommandLimiter.AddMessageCount();
        }

        public static bool CheckSupermod(string username)
        {
            return _supermodList.Contains(username);
        }
    }
}