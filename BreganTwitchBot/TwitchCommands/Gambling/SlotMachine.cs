using System;
using System.Collections.Generic;
using BreganTwitchBot.Connection;
using BreganTwitchBot.Database;
using BreganTwitchBot.TwitchCommands.MessageLimiter;
using Serilog;

namespace BreganTwitchBot.TwitchCommands.Gambling
{
    public class SlotMachine
    {
        public void SpinSlotMachine(string username, long pointsGambled)
        {
            var databaseQuery = new DatabaseQueries();
            var messageLimiter = new CommandLimiter();
            databaseQuery.RemoveUserPoints(username, pointsGambled);

            var random = new Random();
            var emoteList = new List<string>();

            for (var i = 0; i < 3; i++)
            {
                var number = random.Next(1, 11);

                if (number <= 4) //1,2,3,4
                {
                    emoteList.Add("Kappa");
                }
                else if (number > 4 && 7 >= number) //5,6,7
                {
                    emoteList.Add("4Head");
                }
                else if (number > 7 && 9 >= number) //8,9
                {
                    emoteList.Add("LUL");
                }
                else if (number == 10)
                {
                    emoteList.Add("TriHard");
                }
            }

            if (emoteList[0] == "Kappa" && emoteList[1] == "Kappa" && emoteList[2] == "Kappa")
            {
                databaseQuery.ExecuteQuery("UPDATE slotMachine SET tier1Wins = tier1Wins + 1");
                databaseQuery.ExecuteQuery($"UPDATE users SET points = points + {pointsGambled * 12} WHERE username = '{username}'");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{username} => You have spun {emoteList[0]} | {emoteList[1]} | {emoteList[2]}. You have won {pointsGambled * 12:N0} points!");
                Log.Information($"[Slot Machine] {username} got a Kappa win!");
                messageLimiter.AddMessageCount();
            }
            else if (emoteList[0] == "4Head" && emoteList[1] == "4Head" && emoteList[2] == "4Head")
            {
                databaseQuery.ExecuteQuery("UPDATE slotMachine SET tier2Wins = tier2Wins + 1");
                databaseQuery.ExecuteQuery($"UPDATE users SET points = points + {pointsGambled * 22} WHERE username = '{username}'");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{username} => You have spun {emoteList[0]} | {emoteList[1]} | {emoteList[2]}. You have won {pointsGambled * 22:N0} points!");
                Log.Information($"[Slot Machine] {username} got a 4Head win!");
                messageLimiter.AddMessageCount();
            }
            else if (emoteList[0] == "LUL" && emoteList[1] == "LUL" && emoteList[2] == "LUL")
            {
                databaseQuery.ExecuteQuery("UPDATE slotMachine SET tier3Wins = tier3Wins + 1");
                databaseQuery.ExecuteQuery($"UPDATE users SET points = points + {pointsGambled * 50} WHERE username = '{username}'");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{username} => You have spun {emoteList[0]} | {emoteList[1]} | {emoteList[2]}. You have won {pointsGambled * 50:N0} points!");
                Log.Information($"[Slot Machine] {username} got a LUL win!");
                messageLimiter.AddMessageCount();
            }
            else if (emoteList[0] == "TriHard" && emoteList[1] == "TriHard" && emoteList[2] == "TriHard")
            {
                var jackpotAmount = databaseQuery.GetJackpotAmount();
                databaseQuery.ExecuteQuery("UPDATE slotMachine SET jackpotWins = jackpotWins + 1");
                databaseQuery.ExecuteQuery($"UPDATE users SET points = points + {jackpotAmount} WHERE username = '{username}'");
                databaseQuery.ExecuteQuery("UPDATE slotMachine SET jackpotAmount = 0");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{username} => You have spun {emoteList[0]} | {emoteList[1]} | {emoteList[2]}. DING DING DING JACKPOT!!! You have won {jackpotAmount:N0} points!");
                Log.Information($"[Slot Machine] {username} won the jackpot!");
                messageLimiter.AddMessageCount();
            }
            else
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{username} => You have spun {emoteList[0]} | {emoteList[1]} | {emoteList[2]}. No win :(");
                messageLimiter.AddMessageCount();
                databaseQuery.ExecuteQuery($"UPDATE slotMachine SET jackpotAmount = jackpotAmount + {pointsGambled}");
            }
            databaseQuery.ExecuteQuery("UPDATE slotMachine SET totalSpins = totalSpins + 1");
        }
    }
}
