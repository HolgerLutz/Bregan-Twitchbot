using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Text;

namespace Bregan_TwitchBot.Database
{
    internal class DatabaseSetup
    {
        public static SqliteConnection sqlConnection;
        public static SqliteCommand sqlCommand;

        public static void StartupDatabase()
        {
            if (File.Exists("TwitchBotDatabase.sqlite"))
            {
                Console.WriteLine($"[Database] {DateTime.Now}: Database exists, skipping");
                sqlConnection = new SqliteConnection("Filename=TwitchBotDatabase.sqlite;");
                sqlConnection.Open();
                return;
            }

            sqlConnection = new SqliteConnection("Filename=TwitchBotDatabase.sqlite");
            sqlConnection.Open();

            var tableCommand = "CREATE TABLE users (username VARCHAR(50) UNIQUE, minutesInStream bigint, points bigint)";
            sqlCommand = new SqliteCommand(tableCommand, sqlConnection);

            sqlCommand.ExecuteNonQuery();
        }
    }
}
