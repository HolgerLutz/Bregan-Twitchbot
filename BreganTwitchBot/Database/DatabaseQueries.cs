using System;
using Microsoft.Data.Sqlite;

namespace BreganTwitchBot.Database
{
    class DatabaseQueries
    {
        public static void ExecuteQuery(string query)
        {
            var sqlCommand = new SqliteCommand(query, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
        }

        public static long GetUserPoints(string username)
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

        public static TimeSpan GetUserTime(string username)
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

        public static long GetJackpotAmount()
        {
            var sqlQuery = "SELECT jackpotAmount FROM slotMachine";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();
            reader.Read();
            return (long) reader["jackpotAmount"];
        }

        public static bool HasEnoughPoints(string username, long points)
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

        public static void RemoveUserPoints(string username, long pointsToRemove)
        {
            ExecuteQuery($"UPDATE users SET points = points - {pointsToRemove} WHERE username='{username}'");
        }

        public static void AddUserPoints(string username, long pointsToAdd)
        {
            ExecuteQuery($"UPDATE users SET points = points + {pointsToAdd} WHERE username='{username}'");
        }
    }
}
