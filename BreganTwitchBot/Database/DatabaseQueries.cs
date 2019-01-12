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
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();
            var sqlCommand = new SqliteCommand(query, sqlConnection);
            sqlCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
            sqlConnection.Close();
        }

        public void OnMinuteUserPoints(string query, SqliteTransaction transaction, SqliteConnection connection)
        {
            var sqlCommand = new SqliteCommand(query, connection, transaction);
            sqlCommand.ExecuteNonQuery();
        }

        public long GetUserPoints(string username)
        {
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var sqlQuery = $"SELECT points FROM users WHERE username='{username}'";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);
            sqlCommand.ExecuteNonQuery();

            var reader = sqlCommand.ExecuteReader();
            reader.Read();

            if (reader["points"] == DBNull.Value)
            {
                sqlCommand.Dispose();
                reader.Dispose();
                sqlConnection.Dispose();
                return 0;
            }
            var points = (long) reader["points"];

            sqlCommand.Dispose();
            reader.Dispose();
            sqlConnection.Dispose();
            return points;
        }

        public TimeSpan GetUserTime(string username)
        {
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var sqlQuery = $"SELECT minutesInStream FROM users WHERE username='{username}'";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);
            sqlCommand.ExecuteNonQuery();

            var reader = sqlCommand.ExecuteReader();
            reader.Read();

            if (reader["minutesInStream"] == DBNull.Value) //In case the user is new and has not been registered yet
            {
                sqlCommand.Dispose();
                reader.Dispose();
                sqlConnection.Dispose();
                return TimeSpan.FromMinutes(0);
            }

            var minutes = (long) reader["minutesInStream"];

            sqlCommand.Dispose();
            reader.Dispose();
            sqlConnection.Dispose();

            return TimeSpan.FromMinutes(minutes);
        }

        public long GetJackpotAmount()
        {
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var sqlQuery = "SELECT jackpotAmount FROM slotMachine";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);
            sqlCommand.ExecuteNonQuery();

            var reader = sqlCommand.ExecuteReader();
            reader.Read();

            var result = (long) reader["jackpotAmount"];

            reader.Dispose();
            sqlConnection.Dispose();
            sqlCommand.Dispose();
            return result;

        }

        public bool HasEnoughPoints(string username, long points)
        {
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var sqlQuery = $"SELECT points FROM users WHERE username='{username}'";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);
            sqlCommand.ExecuteNonQuery();

            var reader = sqlCommand.ExecuteReader();
            reader.Read();

            if (reader["points"] == DBNull.Value)
            {
                reader.Dispose();
                sqlConnection.Dispose();
                sqlCommand.Dispose();
                return false;
            }

            var pointsCheck = (long) reader["points"] >= points;
            reader.Dispose();
            sqlConnection.Dispose();
            sqlCommand.Dispose();

            return pointsCheck;
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
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var sqlQuery = $"SELECT lastSongRequest FROM users WHERE username='{username}'";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);
            sqlCommand.ExecuteNonQuery();

            var reader = sqlCommand.ExecuteReader();
            reader.Read();
            var time = reader["lastSongRequest"];

            if (time == DBNull.Value)
            {
                reader.Dispose();
                sqlConnection.Dispose();
                sqlCommand.Dispose();
                return DateTime.Now.AddMinutes(-10);
            }

            reader.Dispose();
            sqlConnection.Dispose();
            sqlCommand.Dispose();
            return Convert.ToDateTime(time);
        }

        public string GetTopPoints()
        {
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var userList = new List<string>();
            var pointsList = new List<long>();

            var sqlQuery = "SELECT username, points FROM users ORDER BY points DESC limit 5";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);
            sqlCommand.ExecuteNonQuery();

            var reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                userList.Add(reader["username"].ToString());
                pointsList.Add((long)reader["points"]);
            }

            reader.Dispose();
            sqlConnection.Dispose();
            sqlCommand.Dispose();

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
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var userList = new List<string>();
            var hoursList = new List<long>();

            var sqlQuery = "SELECT username, minutesInStream FROM users ORDER BY minutesInStream DESC limit 5";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);
            sqlCommand.ExecuteNonQuery();

            var reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                userList.Add(reader["username"].ToString());
                hoursList.Add((long)reader["minutesInStream"]);
            }

            reader.Dispose();
            sqlConnection.Dispose();
            sqlCommand.Dispose();

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
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var userList = new List<string>();

            var sqlQuery = "SELECT username FROM users WHERE isSuperMod=1";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);
            sqlCommand.ExecuteNonQuery();

            var reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                userList.Add(reader["username"].ToString());
            }

            reader.Dispose();
            sqlConnection.Dispose();
            sqlCommand.Dispose();
            return userList;
        }

        public void AddSuperMod(string username)
        {
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var sqlQuery = $"UPDATE users SET isSuperMod=1 WHERE username='{username}'";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);
            sqlCommand.ExecuteNonQuery();

            sqlCommand.Dispose();
            sqlConnection.Dispose();
        }

        public void RemoveSuperMod(string username)
        {
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var sqlQuery = $"UPDATE users SET isSuperMod=0 WHERE username='{username}'";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);
            sqlCommand.ExecuteNonQuery();

            sqlCommand.Dispose();
            sqlConnection.Dispose();
        }

        public List<string> LoadBlockedBots()
        {
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var userList = new List<string>();
            var sqlQuery = "SELECT word FROM blacklist WHERE type='bot'";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);

            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                userList.Add(reader["word"].ToString());
            }

            reader.Dispose();
            sqlConnection.Dispose();
            sqlCommand.Dispose();
            return userList;
        }

        public List<string> LoadBlacklistedSongs()
        {
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var userList = new List<string>();
            var sqlQuery = "SELECT word FROM blacklist WHERE type='song'";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);

            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                userList.Add(reader["word"].ToString());
            }

            reader.Dispose();
            sqlConnection.Dispose();
            sqlCommand.Dispose();
            return userList;
        }

        public List<string> LoadBlacklistedWords()
        {
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var userList = new List<string>();
            var sqlQuery = "SELECT word FROM blacklist WHERE type='word'";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);

            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                userList.Add(reader["word"].ToString());
            }

            reader.Dispose();
            sqlConnection.Dispose();
            sqlCommand.Dispose();
            return userList;
        }

        public void AddBlacklistedItem(string item, string itemType)
        {
            ExecuteQuery($"INSERT INTO blacklist (word, type) VALUES ('{item}','{itemType}')");
        }

        public void RemoveBlacklistedItem(string item)
        {
            ExecuteQuery($"DELETE FROM blacklist WHERE word='{item}'");

        }

        public Dictionary<string, Tuple<string, DateTime, long>> LoadCommands()
        {
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var commands = new Dictionary<string, Tuple<string, DateTime, long>>();

            var sqlQuery = "SELECT * FROM commands";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);

            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                commands.Add(reader["commandName"].ToString(), new Tuple<string, DateTime, long>(reader["commandText"].ToString(), Convert.ToDateTime(reader["lastUsed"]), Convert.ToInt64(reader["timesUsed"])));
            }

            reader.Dispose();
            sqlConnection.Dispose();
            sqlCommand.Dispose();
            return commands;
        }

        public void UpdateDatabaseCommandUsage(string commandName, long commandTimesUsed)
        {
            ExecuteQuery($"UPDATE commands SET lastUsed='{DateTime.Now}', timesUsed={commandTimesUsed} WHERE commandName='{commandName}'");
        }

        public void AddNewCommandDatabase(string commandName, string commandText, DateTime commandLastUsed)
        {
            ExecuteQuery($"INSERT INTO commands (commandName, commandText, lastUsed, timesUsed) VALUES ('{commandName}','{commandText}','{commandLastUsed}',0)");
        }

        public void DeleteCommandDatabase(string commandName)
        {
            ExecuteQuery($"DELETE FROM commands WHERE commandName='{commandName}'");
        }

        public void EditCommandDatabase(string commandName, string commandText)
        {
            ExecuteQuery($"UPDATE commands SET commandText='{commandText}' WHERE commandName='{commandName}'");
        }

        public List<string> GetUsersBasedOnTime(long minMinutes, long maxMinutes)
        {
            var sqlConnection = new SqliteConnection(DatabaseSetup.SqlConnectionString);
            sqlConnection.Open();

            var sqlQuery = $"SELECT * FROM users WHERE minutesInStream BETWEEN {minMinutes} AND {maxMinutes}";
            var sqlCommand = new SqliteCommand(sqlQuery, sqlConnection);
            sqlCommand.ExecuteNonQuery();

            var reader = sqlCommand.ExecuteReader();

            //Add all the users in list
            var userList = new List<string>();
            while (reader.Read())
            {
                userList.Add(Convert.ToString(reader["username"]));
            }

            userList.Sort();

            reader.Dispose();
            sqlConnection.Dispose();
            sqlCommand.Dispose();
            return userList;
        }
    }
}
