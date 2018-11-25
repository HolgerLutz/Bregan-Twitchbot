using System;
using System.Collections.Generic;
using System.Text;
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

        public void OnMinuteUserPoints(string query, SqliteTransaction transaction)
        {
            var sqlCommand = new SqliteCommand(query, DatabaseSetup.SqlConnection, transaction);
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

        public string GetTopPoints()
        {
            var userList = new List<string>();
            var pointsList = new List<long>();
            var sqlQuery = "SELECT username, points FROM users ORDER BY points DESC limit 5";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                userList.Add(reader["username"].ToString());
                pointsList.Add((long)reader["points"]);
            }

            var usersAndPointsSb = new StringBuilder();

            for (int i = 0; i < userList.Count; i++)
            {
                var username = $"#{i +1} - {userList[i]} - {pointsList[i]:N0} | ";
                usersAndPointsSb.Append(username);
            }

            var usersAndPoints = usersAndPointsSb.ToString();
            return usersAndPoints;
        }

        public string GetTopHours()
        {
            var userList = new List<string>();
            var hoursList = new List<long>();
            var sqlQuery = "SELECT username, minutesInStream FROM users ORDER BY minutesInStream DESC limit 5";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                userList.Add(reader["username"].ToString());
                hoursList.Add((long)reader["minutesInStream"]);
            }

            var usersAndHoursSb = new StringBuilder();

            for (int i = 0; i < userList.Count; i++)
            {
                var username = $"#{i + 1} - {userList[i]} - {Math.Round((double)hoursList[i] / 60, 2)} hours | ";
                usersAndHoursSb.Append(username);
            }

            var usersAndHours = usersAndHoursSb.ToString();
            return usersAndHours;
        }

        public List<string> GetSuperMods()
        {
            var userList = new List<string>();
            var sqlQuery = "SELECT username FROM users WHERE isSuperMod=1";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                userList.Add(reader["username"].ToString());
            }
            return userList;
        }

        public void AddSuperMod(string username)
        {
            var sqlQuery = $"UPDATE users SET isSuperMod=1 WHERE username='{username}'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
        }

        public void RemoveSuperMod(string username)
        {
            var sqlQuery = $"UPDATE users SET isSuperMod=0 WHERE username='{username}'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
        }

        public List<string> LoadBlockedBots()
        {
            var userList = new List<string>();
            var sqlQuery = "SELECT word FROM blacklist WHERE type='bot'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);

            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                userList.Add(reader["word"].ToString());
            }

            return userList;
        }

        public List<string> LoadBlacklistedSongs()
        {
            var userList = new List<string>();
            var sqlQuery = "SELECT word FROM blacklist WHERE type='song'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);

            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                userList.Add(reader["word"].ToString());
            }

            return userList;
        }

        public List<string> LoadBlacklistedWords()
        {
            var userList = new List<string>();
            var sqlQuery = "SELECT word FROM blacklist WHERE type='word'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);

            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                userList.Add(reader["word"].ToString());
            }

            return userList;
        }

        public void AddBlacklistedItem(string item, string itemType)
        {
            var sqlQuery = $"INSERT INTO blacklist (word, type) VALUES ('{item}','{itemType}')";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);

            sqlCommand.ExecuteNonQuery();
        }

        public void RemoveBlacklistedItem(string item)
        {
            var sqlQuery = $"DELETE FROM blacklist WHERE word='{item}'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);

            sqlCommand.ExecuteNonQuery();
        }

        public Dictionary<string, Tuple<string, DateTime, long>> LoadCommands()
        {
            var commands = new Dictionary<string, Tuple<string, DateTime, long>>();
            var sqlQuery = "SELECT * FROM commands";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);

            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                commands.Add(reader["commandName"].ToString(), new Tuple<string, DateTime, long>(reader["commandText"].ToString(), Convert.ToDateTime(reader["lastUsed"]), Convert.ToInt64(reader["timesUsed"])));
            }
            return commands;
        }

        public void UpdateDatabaseCommandUsage(string commandName, long commandTimesUsed)
        {
            var sqlQuery = $"UPDATE commands SET lastUsed='{DateTime.Now}', timesUsed={commandTimesUsed} WHERE commandName='{commandName}'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
        }

        public void AddNewCommandDatabase(string commandName, string commandText, DateTime commandLastUsed)
        {
            var sqlQuery = $"INSERT INTO commands (commandName, commandText, lastUsed, timesUsed) VALUES ('{commandName}','{commandText}','{commandLastUsed}',0)";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
        }

        public void DeleteCommandDatabase(string commandName)
        {
            var sqlQuery = $"DELETE FROM commands WHERE commandName='{commandName}'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
        }

        public void EditCommandDatabase(string commandName, string commandText)
        {
            var sqlQuery = $"UPDATE commands SET commandText='{DateTime.Now}' WHERE commandName='{commandName}'";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
        }

        public List<string> GetUsersBasedOnTime(long minMinutes, long maxMinutes)
        {
            var sqlQuery = $"SELECT * FROM users WHERE minutesInStream BETWEEN {minMinutes} AND {maxMinutes}";
            var sqlCommand = new SqliteCommand(sqlQuery, DatabaseSetup.SqlConnection);
            sqlCommand.ExecuteNonQuery();
            var reader = sqlCommand.ExecuteReader();

            //Add all the users in list
            var userList = new List<string>();
            while (reader.Read())
            {
                userList.Add(Convert.ToString(reader["username"]));
            }

            userList.Sort();

            return userList;
        }
    }
}
