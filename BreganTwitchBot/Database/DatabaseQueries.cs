using System;
using System.Globalization;
using Discord;
using Microsoft.Data.Sqlite;

namespace BreganTwitchBot.Database
{
    class DatabaseQueries
    {
        public void ExecuteQuery(string query)
        {
            var sqlCommand = new SqliteCommand(query, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
        }

        public long GetUserPoints(string username)
        {
            var sqlQuery = $"SELECT points FROM users WHERE username='{username}'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();
            reader.Read();

            if (reader["points"] == DBNull.Value)
            {
                return 0;
            }

            return (long) reader["points"];
        }

        public TimeSpan GetUserTime(string username)
        {
            var sqlQuery = $"SELECT minutesInStream FROM users WHERE username='{username}'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();
            reader.Read();

            if (reader["minutesInStream"] == DBNull.Value) //Incase the user is new and has not been registered yet
            {
                return TimeSpan.FromMinutes(0);
            }

            var minutes = (long) reader["minutesInStream"];
            return TimeSpan.FromMinutes(minutes);
        }

        public long GetJackpotAmount()
        {
            var sqlQuery = "SELECT jackpotAmount FROM slotMachine";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();
            reader.Read();
            return (long) reader["jackpotAmount"];
        }

        public bool HasEnoughPoints(string username, long points)
        {
            var sqlQuery = $"SELECT points FROM users WHERE username='{username}'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();
            reader.Read();

            if (reader["points"] == DBNull.Value)
            {
                return false;
            }

            return (long) reader["points"] >= points;
        }

        public void RemoveUserPoints(string username, long pointsToRemove)
        {
            ExecuteQuery($"UPDATE users SET points = points - {pointsToRemove} WHERE username='{username}'");
        }

        public void AddUserPoints(string username, long pointsToAdd)
        {
            ExecuteQuery($"UPDATE users SET points = points + {pointsToAdd} WHERE username='{username}'");
        }

        public void UpdateLastSongRequest(string username)
        {
            ExecuteQuery($"UPDATE users SET lastSongRequest = datetime('now', 'localtime') WHERE username='{username}'");
        }

        public DateTime GetLastSongRequest(string username)
        {
            var sqlQuery = $"SELECT lastSongRequest FROM users WHERE username='{username}'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();
            reader.Read();
            var time = reader["lastSongRequest"];

            if (time == DBNull.Value)
            {
                return DateTime.Now.AddMinutes(-10);
            }

            return Convert.ToDateTime(time);
        }
    }
}
