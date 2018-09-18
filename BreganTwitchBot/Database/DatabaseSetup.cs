using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace BreganTwitchBot.Database
{
    class DatabaseSetup
    {
        public static SqliteConnection SqlConnection;

        public static void StartupDatabase()
        {
            if (File.Exists("TwitchBotDatabase.sqlite"))
            {
                Console.WriteLine($"[Database] {DateTime.Now}: Database exists, skipping");
                SqlConnection = new SqliteConnection("Filename=TwitchBotDatabase.sqlite;");
                SqlConnection.Open();
                DatabaseQueries.ExecuteQuery("CREATE TABLE IF NOT EXISTS users (username VARCHAR(50) UNIQUE, minutesInStream bigint, points bigint, lastSongRequest text)");
                DatabaseQueries.ExecuteQuery("CREATE TABLE IF NOT EXISTS slotMachine (tier1Wins bigint, tier2Wins bigint, tier3Wins bigint, jackpotWins bigint, totalSpins bigint, jackpotAmount bigint)");

                var sqlCommand = new SqliteCommand("SELECT Count(*) FROM slotMachine", SqlConnection);
                var count = (long) sqlCommand.ExecuteScalar();

                if (count == 0)
                {
                    DatabaseQueries.ExecuteQuery("INSERT INTO slotMachine(tier1Wins,tier2Wins,tier3Wins,jackpotWins,totalSpins,jackpotAmount) VALUES (0,0,0,0,0,0)");
                }
                return;

            }
            SqlConnection = new SqliteConnection("Filename=TwitchBotDatabase.sqlite");
            SqlConnection.Open();
            DatabaseQueries.ExecuteQuery("CREATE TABLE users (username VARCHAR(50) UNIQUE, minutesInStream bigint, points bigint, lastSongRequest text)");
            DatabaseQueries.ExecuteQuery("CREATE TABLE slotMachine (tier1Wins bigint, tier2Wins bigint, tier3Wins bigint, jackpotWins bigint, totalSpins bigint, jackpotAmount bigint)");
            DatabaseQueries.ExecuteQuery("INSERT INTO slotMachine(tier1Wins,tier2Wins,tier3Wins,jackpotWins,totalSpins,jackpotAmount) VALUES (0,0,0,0,0,0)");
            Console.WriteLine($"[Database] {DateTime.Now}: Database created");
        }
    }
}
