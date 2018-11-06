using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using BreganTwitchBot.Connection;
using BreganTwitchBot.Discord;
using Microsoft.Data.Sqlite;
using Serilog;
using TwitchLib.Api.Core.Exceptions;

namespace BreganTwitchBot.Database
{
    class TimeTracker
    {
        private static List<string> _blockedBotsUserList;
        private static string _folderPath;
        private static string _blockedBotsUserFilePath;

        public static void UserTimeTracker()
        {
            var timer = new Timer(60000);
            timer.Start();
            timer.Elapsed += OnMinute;

            //For the leader boards all bots that are in the stream will need to be removed from gaining points
            //This is a custom list that can have bots added directly from the chat

            _folderPath = Directory.GetCurrentDirectory();
            _blockedBotsUserFilePath = Path.Combine(_folderPath, "Config/blockedbots.txt");

            if (!File.Exists(_blockedBotsUserFilePath))
            {
                File.Create(_blockedBotsUserFilePath).Dispose();
                Log.Information("[Time Tracker] Blocked bot list created");
            }

            try
            {
                _blockedBotsUserList = new List<string>(File.ReadAllLines(_blockedBotsUserFilePath).ToList());
                Log.Information("[Time Tracker] Blocked bots successfully loaded");
            }
            catch (FileNotFoundException)
            {
                Log.Warning("[Time Tracker] File not found... Creating new file");
                File.Create(_blockedBotsUserFilePath).Dispose();
                Log.Information("[Time Tracker] File successfully created");
            }
        }

        private static void OnMinute(object sender, ElapsedEventArgs e)
        {
            var databaseQuery = new DatabaseQueries();
            try
            {
                if (TwitchApiConnection.ApiClient.V5.Streams.BroadcasterOnlineAsync(StartService.TwitchChannelID).Result)
                {
                    var userList = TwitchApiConnection.ApiClient.Undocumented.GetChattersAsync(StartService.ChannelName).Result;

                    foreach (var user in userList)
                    {
                        if (!_blockedBotsUserList.Contains(user.Username))
                        {
                            databaseQuery.ExecuteQuery($"INSERT OR IGNORE INTO users (username, minutesInStream, points) VALUES ('{user.Username}',0,0)");
                            databaseQuery.ExecuteQuery($"UPDATE users SET minutesInStream = minutesInStream +1, points = points + 10 WHERE username='{user.Username}'");
                            Log.Information($"[Database] User {user.Username} updated");
                        }
                    }
                }
            }
            catch (BadGatewayException exception)
            {
                Console.WriteLine(exception);
            }
            catch (InternalServerErrorException exception)
            {
                Console.WriteLine(exception);
            }
        }

        public async Task GetAndSendFirstRankUsers()
        {
            //RANK 1 is between 1hour -> 24 hours 59 mins
            var sqlQuery = "SELECT * FROM users WHERE minutesInStream BETWEEN 60 AND 1499";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();

            //Add all the users in list
            var userList = new List<string>();
            while (await reader.ReadAsync())
            {
                userList.Add(Convert.ToString(reader["username"]));
            }
            userList.Sort();
            //Create the varible to add all users to
            var users = "**Melvins:**" + Environment.NewLine;

            foreach (var user in userList)
            {
                users += user + Environment.NewLine;
                if (users.Length > 1800) //Check if it is over 1800 as 2000 char is the limit to a discord message. 200 less to be safe
                {
                    await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, users);
                    users = ""; //Reset the string so none of users are left
                }
            }
            await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, users); //Sends the users from the loop
        }

        public async Task GetAndSendSecondRankUsers()
        {
            //RANK 2 is between 25 hours -> 99 hours 59 mins
            var sqlQuery = "SELECT * FROM users WHERE minutesInStream BETWEEN 1500 AND 5999";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();

            //Add all the users in list
            var userList = new List<string>();
            while (await reader.ReadAsync())
            {
                userList.Add(Convert.ToString(reader["username"]));
            }
            userList.Sort();
            //Create the variable to add all users to
            var users = "**WOT Crew:**" + Environment.NewLine;

            foreach (var user in userList)
            {
                users += user + Environment.NewLine;
                if (users.Length > 1800) //Check if it is over 1800 as 2000 char is the limit to a discord message. 200 less to be safe
                {
                    await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, users);
                    users = ""; //Reset the string so none of users are left
                }
            }
            await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, users); //Sends the users from the loop
        }

        public async Task GetAndSendThirdRankUsers()
        {
            //RANK 3 is between 100 hours -> 249 hours 59 minutes
            var sqlQuery = "SELECT * FROM users WHERE minutesInStream BETWEEN 6000 AND 14999";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();

            //Add all the users in list
            var userList = new List<string>();
            while (await reader.ReadAsync())
            {
                userList.Add(Convert.ToString(reader["username"]));
            }
            userList.Sort();
            //Create the variable to add all users to
            var users = "**BLOCKS Crew:**" + Environment.NewLine;

            foreach (var user in userList)
            {
                users += user + Environment.NewLine;
                if (users.Length > 1800) //Check if it is over 1800 as 2000 char is the limit to a discord message. 200 less to be safe
                {
                    await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, users);
                    users = ""; //Reset the string so none of users are left
                }
            }
            await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, users); //Sends the users from the loop
        }

        public async Task GetAndSendFourthRankUsers()
        {
            //RANK 2 is between 250 hours -> 499 hours 59 minutes
            var sqlQuery = "SELECT * FROM users WHERE minutesInStream BETWEEN 15000 AND 29999";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();

            //Add all the users in list
            var userList = new List<string>();
            while (await reader.ReadAsync())
            {
                userList.Add(Convert.ToString(reader["username"]));
            }
            userList.Sort();
            //Create the variable to add all users to
            var users = "**The name of legends:**" + Environment.NewLine;

            foreach (var user in userList)
            {
                users += user + Environment.NewLine;
                if (users.Length > 1800) //Check if it is over 1800 as 2000 char is the limit to a discord message. 200 less to be safe
                {
                    await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, users);
                    users = ""; //Reset the string so none of users are left
                }
            }
            await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, users); //Sends the users from the loop
        }


        public async Task GetAndSendFifthRankUsers()
        {
            //RANK 2 is between 250 hours -> 499 hours 59 minutes
            var sqlQuery = "SELECT * FROM users WHERE minutesInStream > 30000";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();

            //Add all the users in list
            var userList = new List<string>();
            while (await reader.ReadAsync())
            {
                userList.Add(Convert.ToString(reader["username"]));
            }
            userList.Sort();
            //Create the variable to add all users to
            var users = "**King of the stream:**" + Environment.NewLine;

            foreach (var user in userList)
            {
                users += user + Environment.NewLine;
                if (users.Length > 1800) //Check if it is over 1800 as 2000 char is the limit to a discord message. 200 less to be safe
                {
                    await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, users);
                    users = ""; //Reset the string so none of users are left
                }
            }
            await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, users); //Sends the users from the loop
        }


        public async Task GetAll()
        {
            await GetAndSendFirstRankUsers();
            await GetAndSendSecondRankUsers();
            await GetAndSendThirdRankUsers();
            await GetAndSendFourthRankUsers();
            await GetAndSendFifthRankUsers();
        }
    }
}
