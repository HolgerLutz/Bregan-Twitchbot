using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using BreganTwitchBot.Connection;
using BreganTwitchBot.Database;
using BreganTwitchBot.Discord;
using Microsoft.Data.Sqlite;
using Serilog;

namespace BreganTwitchBot.TwitchCommands.Points
{
    class TimeTracker
    {
        private static List<string> _blockedBotsUserList;

        public static void UserTimeTracker()
        {
            var timer = new Timer(60000);
            timer.Start();
            timer.Elapsed += OnMinute;

            //For the leader boards all bots that are in the stream will need to be removed from gaining points
            //This is a custom list that can have bots added directly from the chat
            var databaseQuery = new DatabaseQueries();
            _blockedBotsUserList = new List<string>(databaseQuery.LoadBlockedBots());
            Log.Information("[Blocked Bots] Bots successfully loaded");
        }

        private static void OnMinute(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!TwitchApiConnection.ApiClient.V5.Streams.BroadcasterOnlineAsync(StartService.TwitchChannelID).Result)
                {
                    return;
                }

                var connection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
                var databaseQuery = new DatabaseQueries();
                connection.Open();
                var transaction = connection.BeginTransaction();

                var userList = TwitchApiConnection.ApiClient.Undocumented.GetChattersAsync(StartService.ChannelName).Result;

                foreach (var user in userList)
                {
                    if (_blockedBotsUserList.Contains(user.Username))
                    {
                        continue;
                    }
                    databaseQuery.OnMinuteUserPoints($"INSERT OR IGNORE INTO users (username, minutesInStream, points) VALUES ('{user.Username}',0,0)", transaction, connection);
                    databaseQuery.OnMinuteUserPoints($"UPDATE users SET minutesInStream = minutesInStream +1, points = points + 20 WHERE username='{user.Username}'", transaction, connection);
                    Log.Information($"[Database] User {user.Username} updated");
                }
                transaction.Commit();
                transaction.Dispose();
                connection.Close();
            }
            catch (Exception exception)
            {
                Log.Fatal($"[Time Tracker] {exception.Message}");
            }
        }

        public async Task GetUsersTime(long minMinutes, long maxMinutes, string rankName)
        {
            var databaseQueries = new DatabaseQueries();
            var userList = databaseQueries.GetUsersBasedOnTime(minMinutes, maxMinutes);

            var users = $"**{rankName}:**" + Environment.NewLine;

            foreach (var user in userList)
            {
                users += user + Environment.NewLine;
                if (users.Length > 1900
                ) //Check if it is over 1900 as 2000 char is the limit to a discord message. 200 less to be safe
                {
                    await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, users);
                    users = ""; //Reset the string so none of users are left
                }
            }

            await DiscordConnection.SendMessage(StartService.DiscordEventChannelID, users); //Sends the users from the loop
        }

        public async Task GetAll()
        {
            await GetUsersTime(60,1499, "Melvins");
            await GetUsersTime(1500,5999,"WOT Crew");
            await GetUsersTime(6000,14999, "BLOCKS Crew");
            await GetUsersTime(15000, 29999, "The Name of Legends");
            await GetUsersTime(30000, long.MaxValue, "King of The Stream");
        }
    }
}
