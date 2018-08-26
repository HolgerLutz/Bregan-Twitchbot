using System;
using System.Collections.Generic;
using BreganTwitchBot.Connection;
using BreganTwitchBot.Database;
using BreganTwitchBot.TwitchCommands.MessageLimiter;

namespace BreganTwitchBot.TwitchCommands.Gambling
{
    public class SlotMachine
    {
        public void SpinSlotMachine(string username, long pointsGambled)
        {
            DatabaseQueries.RemoveUserPoints(username, pointsGambled);

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
                DatabaseQueries.ExecuteQuery("UPDATE slotMachine SET tier1Wins = tier1Wins + 1");
                DatabaseQueries.ExecuteQuery($"UPDATE users SET points = points + {pointsGambled * 12} WHERE username = '{username}'");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{username} => You have spun {emoteList[0]} | {emoteList[1]} | {emoteList[2]}. You have won {pointsGambled * 12} points!");
                CommandLimiter.AddMessageCount();
            }
            else if (emoteList[0] == "4Head" && emoteList[1] == "4Head" && emoteList[2] == "4Head")
            {
                DatabaseQueries.ExecuteQuery("UPDATE slotMachine SET tier2Wins = tier2Wins + 1");
                DatabaseQueries.ExecuteQuery($"UPDATE users SET points = points + {pointsGambled * 22} WHERE username = '{username}'");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{username} => You have spun {emoteList[0]} | {emoteList[1]} | {emoteList[2]}. You have won {pointsGambled * 22} points!");
                CommandLimiter.AddMessageCount();
            }
            else if (emoteList[0] == "LUL" && emoteList[1] == "LUL" && emoteList[2] == "LUL")
            {
                DatabaseQueries.ExecuteQuery("UPDATE slotMachine SET tier3Wins = tier3Wins + 1");
                DatabaseQueries.ExecuteQuery($"UPDATE users SET points = points + {pointsGambled * 50} WHERE username = '{username}'");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{username} => You have spun {emoteList[0]} | {emoteList[1]} | {emoteList[2]}. You have won {pointsGambled * 50} points!");
                CommandLimiter.AddMessageCount();
            }
            else if (emoteList[0] == "TriHard" && emoteList[1] == "TriHard" && emoteList[2] == "TriHard")
            {
                var jackpotAmount = DatabaseQueries.GetJackpotAmount();
                DatabaseQueries.ExecuteQuery("UPDATE slotMachine SET jackpotWins = jackpotWins + 1");
                DatabaseQueries.ExecuteQuery($"UPDATE users SET points = points + {jackpotAmount} WHERE username = '{username}'");
                DatabaseQueries.ExecuteQuery("UPDATE slotMachine SET jackpotAmount = 0");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{username} => You have spun {emoteList[0]} | {emoteList[1]} | {emoteList[2]}. DING DING DING JACKPOT!!! You have won {jackpotAmount} points!");
                CommandLimiter.AddMessageCount();
            }
            else
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{username} => You have spun {emoteList[0]} | {emoteList[1]} | {emoteList[2]}. No win :(");
                CommandLimiter.AddMessageCount();
                DatabaseQueries.ExecuteQuery($"UPDATE slotMachine SET jackpotAmount = jackpotAmount + {pointsGambled}");
            }

            DatabaseQueries.ExecuteQuery("UPDATE slotMachine SET totalSpins = totalSpins + 1");
        }
    }
}
