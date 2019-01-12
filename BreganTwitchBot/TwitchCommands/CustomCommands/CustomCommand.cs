using System;
using System.Collections.Generic;
using System.Linq;
using BreganTwitchBot.Database;
using Serilog;

namespace BreganTwitchBot.TwitchCommands.CustomCommands
{
    class CustomCommand
    {
        private static Dictionary<string, Tuple<string, DateTime, long>> _commandsDictionary;
        private static DatabaseQueries _databaseQueries;
        private static string[] _blacklistedCommandNames;

        public static void CustomCommandsSetup()
        {
            _databaseQueries = new DatabaseQueries();
            _commandsDictionary = new Dictionary<string, Tuple<string, DateTime, long>>();
            _commandsDictionary = _databaseQueries.LoadCommands();
            Log.Information($"[Twitch Commands] {_commandsDictionary.Count} commands successfully loaded from the database");
            _blacklistedCommandNames = new []{ "!8ball", "!dadjoke", "!commands", "!pitchfork", "!shoutout", "!so", "!startgiveaway", "!joingiveaway", "!amountentered", "!setgiveawaytime", "!reroll", "!joinqueue", "!leavequeue", "!queue", "!nextgame", "!queueposition", "!removegame", "!clearqueue", "!setremoveamount", "!addbadword", "!removebadword", "!points", "!hours", "!hrs", "!pointslb", "!gamble", "!spin", "!slots", "!flip", "!jackpot", "!sr", "!songrequest", "!togglesr", "!srtoggle", "!blacklistsong", "!addpoints", "!addsupermod", "!removesupermod", "!addcmd", "!cmdadd", "!editcmd", "!cmdedit", "!delcmd", "!cmddel, !howlong"};
        }

        public string GetCommand(string commandName, string username)
        {
            if (_blacklistedCommandNames.Contains(commandName))
            {
                return null;
            }

            //Check if the command exists and if it not on cooldown
            if (!_commandsDictionary.ContainsKey(commandName) || DateTime.Now - TimeSpan.FromSeconds(5) < _commandsDictionary[commandName].Item2)
            {
                return null;
            }

            UpdateCommandUsage(commandName);

            string command = _commandsDictionary[commandName].Item1;

            //Counters
            if (_commandsDictionary[commandName].Item1.Contains("[count]"))
            {
                command = command.Replace("[count]", _commandsDictionary[commandName].Item3.ToString());
            }

            //Usernames
            if (_commandsDictionary[commandName].Item1.IndexOf("[user]", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                command = command.Replace("[user]", username);
            }

            //Single quotes 
            if (_commandsDictionary[commandName].Item1.Contains("''"))
            {
                command = command.Replace("''", "'");
            }
            return command;

        }

        public void UpdateCommandUsage(string commandName)
        {
            var commandText = _commandsDictionary[commandName].Item1;
            var commandTimesUsed = _commandsDictionary[commandName].Item3 + 1;

            _commandsDictionary.Remove(commandName);
            _commandsDictionary.Add(commandName, new Tuple<string, DateTime, long>(commandText, DateTime.Now, commandTimesUsed));
            _databaseQueries.UpdateDatabaseCommandUsage(commandName, commandTimesUsed);
        }

        public bool AddNewCommand(string commandName, string commandText)
        {
            if (_commandsDictionary.ContainsKey(commandName) || _blacklistedCommandNames.Contains(commandName))
            {
                return false;
            }

            if (commandText.Contains("'"))
            {
                var updatedCommandText = commandText.Replace("'", "''");
                _commandsDictionary.Add(commandName, new Tuple<string, DateTime, long>(updatedCommandText, DateTime.Now - TimeSpan.FromSeconds(5), 0));
                _databaseQueries.AddNewCommandDatabase(commandName, updatedCommandText, DateTime.Now - TimeSpan.FromSeconds(5));
                return true;
            }
            _commandsDictionary.Add(commandName, new Tuple<string, DateTime, long>(commandText, DateTime.Now - TimeSpan.FromSeconds(5), 0));
            _databaseQueries.AddNewCommandDatabase(commandName, commandText, DateTime.Now - TimeSpan.FromSeconds(5));
            return true;
        }

        public bool RemoveCommand(string commandName)
        {
            if (!_commandsDictionary.ContainsKey(commandName))
            {
                return false;
            }

            _commandsDictionary.Remove(commandName);
            _databaseQueries.DeleteCommandDatabase(commandName);
            return true;
        }

        public bool EditCommand(string commandName, string commandText)
        {
            if (!_commandsDictionary.ContainsKey(commandName))
            {
                return false;
            }

            var timesUsed = _commandsDictionary[commandName].Item3;
            _commandsDictionary.Remove(commandName);

            if (commandText.Contains("'"))
            {
                var updatedCommandText = commandText.Replace("'", "''");
                _commandsDictionary.Add(commandName, new Tuple<string, DateTime, long>(commandText, DateTime.Now, timesUsed));
                _databaseQueries.EditCommandDatabase(commandName, updatedCommandText);
                return true;
            }

            _commandsDictionary.Add(commandName, new Tuple<string, DateTime, long>(commandText, DateTime.Now, timesUsed));
            _databaseQueries.EditCommandDatabase(commandName, commandText);
            return true;
        }
    }
}
