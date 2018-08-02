using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Bregan_TwitchBot.Connection;
using TwitchLib.Api;
using TwitchLib.Api.Exceptions;

namespace Bregan_TwitchBot.Database
{
    internal class TimeTracker
    {
        private static string _sqlquery;

        public static void UserTimeTracker()
        {
            var timer = new Timer(60000);
            timer.Start();
            timer.Elapsed += OnMinute;
        }

        private static void OnMinute(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (TwitchApiConnection.ApiClient.Streams.v5.BroadcasterOnlineAsync(StartService.TwitchChannelID).Result)
                {
                    var userList = TwitchApiConnection.ApiClient.Undocumented.GetChattersAsync(StartService.ChannelName).Result;
                    foreach (var user in userList)
                    {
                        _sqlquery = $"INSERT OR IGNORE INTO users (username, minutesInStream, points) VALUES ('{user.Username}',0,0)";
                        DatabaseQueries.ExecuteQuery(_sqlquery);
                        _sqlquery = $"UPDATE users SET minutesInStream = minutesInStream +1, points = points + 10 WHERE username='{user.Username}'";
                        DatabaseQueries.ExecuteQuery(_sqlquery);
                        Console.WriteLine($"[Database] {DateTime.Now}: User {user.Username} updated");
                    }
                }
            }
            catch (BadGatewayException exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
