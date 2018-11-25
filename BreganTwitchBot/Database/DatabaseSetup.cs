using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Serilog;

namespace BreganTwitchBot.Database
{
    class DatabaseSetup
    {
        public static SqliteConnection SqlConnection;

        public static void StartupDatabase()
        {
            var databaseQuery = new DatabaseQueries();

            if (File.Exists("TwitchBotDatabase.sqlite"))
            {
                Log.Information("[Database] Database exists. Creating tables if needed");

                SqlConnection = new SqliteConnection("Filename=TwitchBotDatabase.sqlite;");
                SqlConnection.Open();

                databaseQuery.ExecuteQuery("CREATE TABLE IF NOT EXISTS users (username TEXT UNIQUE, minutesInStream BIGINT, points BIGINT, lastSongRequest TEXT, isSuperMod INT)");
                databaseQuery.ExecuteQuery("CREATE TABLE IF NOT EXISTS slotMachine (tier1Wins BIGINT, tier2Wins BIGINT, tier3Wins BIGINT, jackpotWins BIGINT, totalSpins BIGINT, jackpotAmount BIGINT)");
                databaseQuery.ExecuteQuery("CREATE TABLE IF NOT EXISTS blacklist (word TEXT, type TEXT)");
                databaseQuery.ExecuteQuery("CREATE TABLE IF NOT EXISTS commands (commandName TEXT, commandText TEXT, lastUsed TEXT, timesUsed BIGINT, TEXT userLevel)");
                var sqlCommand = new SqliteCommand("SELECT Count(*) FROM slotMachine", SqlConnection);

                var count = (long) sqlCommand.ExecuteScalar();

                if (count == 0)
                {
                    databaseQuery.ExecuteQuery("INSERT INTO slotMachine(tier1Wins,tier2Wins,tier3Wins,jackpotWins,totalSpins,jackpotAmount) VALUES (0,0,0,0,0,0)");
                }
                Log.Information("[Database] Database loaded");
                return;

            }
            SqlConnection = new SqliteConnection("Filename=TwitchBotDatabase.sqlite");
            SqlConnection.Open();
            databaseQuery.ExecuteQuery("CREATE TABLE users (username TEXT UNIQUE, minutesInStream bigint, points bigint, lastSongRequest text, isSuperMod int)");
            databaseQuery.ExecuteQuery("CREATE TABLE slotMachine (tier1Wins bigint, tier2Wins bigint, tier3Wins bigint, jackpotWins bigint, totalSpins bigint, jackpotAmount bigint)");
            databaseQuery.ExecuteQuery("INSERT INTO slotMachine(tier1Wins,tier2Wins,tier3Wins,jackpotWins,totalSpins,jackpotAmount) VALUES (0,0,0,0,0,0)");
            databaseQuery.ExecuteQuery("CREATE TABLE blacklist (word TEXT, type TEXT)");
            databaseQuery.ExecuteQuery("CREATE TABLE commands (commandName TEXT, commandText TEXT, lastUsed TEXT, timesUsed BIGINT, userLevel TEXT)");
            Log.Information("[Database] Database created");
        }
    }
}
