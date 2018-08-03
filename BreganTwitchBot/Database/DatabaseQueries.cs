using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Text;

namespace Bregan_TwitchBot.Database
{
    internal class DatabaseQueries
    {
        public static void ExecuteQuery(string query)
        {
            var sqlCommand = new SqliteCommand(query, DatabaseSetup.sqlConnection);
            sqlCommand.ExecuteNonQuery();
        }

        public static long GetUserPoints(string username)
        {
            var sqlQuery = $"SELECT points FROM users WHERE username='{username}'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.sqlConnection);
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
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.sqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();
            reader.Read();

            if (reader["minutesInStream"] == DBNull.Value) //Incase the user is new and has not been registered yet
            {
                return TimeSpan.FromMinutes(0);
            }
            var minutes = (long)reader["minutesInStream"];
            return TimeSpan.FromMinutes(minutes);
        }
    }
}
