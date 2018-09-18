using System;
using System.Collections.Generic;
using System.Timers;
using BreganTwitchBot.Connection;
using BreganTwitchBot.TwitchCommands.MessageLimiter;

namespace BreganTwitchBot.TwitchCommands.Giveaway
{
    class Giveaways //TODO: see time left and see the timer amount that is set, set sub only, set follow only
    {
        private static List<string> _contestents;
        public static int TimerAmount;
        public static bool IsGiveawayOn;
        private static Timer _giveawayTimer;

        public void StartGiveaway()
        {
            //Don't want to be starting multiple giveaways at once - this is by default set to false in StartService.cs
            if (IsGiveawayOn)
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "There is already a giveaway running!");
                messageLimter.AddMessageCount();
                return;
            }

            _contestents = new List<string>();
            IsGiveawayOn = true;
            _giveawayTimer = new Timer {Interval = TimerAmount};
            _giveawayTimer.Start();
            _giveawayTimer.Elapsed += TimerWinner;
        }

        private static void TimerWinner(object sender, ElapsedEventArgs e)
        {
            //First winner 
            WinnerWinner();
            _giveawayTimer.Dispose();
            Console.WriteLine("[Giveaway] User successfully won");
            
        }

        public void SetTimerAmount(string timeCommand, string username)
        {
            try //Make sure the number entered is not bigger than max int
            {
                var time = int.Parse(timeCommand);
                TimerAmount = time * 1000; //Times by 1000 to convert to milliseconds
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Giveaway] {DateTime.Now}: The giveaway time has been updated to {time}");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName,$"@{username} => The giveaway time has been updated to {time} seconds");
                Console.ResetColor();
            }
            catch (OverflowException exception)
            {
                Console.WriteLine(exception);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Giveaway] {DateTime.Now}: User attempted to break the bot (OverflowException)");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "Good attempt at breaking the bot (OverflowException)");
                messageLimter.AddMessageCount();
                Console.ResetColor();
            }
            catch (FormatException exception)
            {
                Console.WriteLine(exception);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Giveaway] {DateTime.Now}: User attempted to break the bot (FormatException)");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "Good attempt at breaking the bot (FormatException)");
                messageLimter.AddMessageCount();
                Console.ResetColor();
            }
        }

        public void AddContestant(string username)
        {
            if (IsGiveawayOn && !_contestents.Contains(username))
            {
                _contestents.Add(username);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Giveaway] {DateTime.Now}: {username} has entered the giveaway");
                Console.ResetColor();
                return;
            }
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "There is no giveaway running currently");
        }

        public string AmountOfContestantsEntered()
        {
            return IsGiveawayOn ? $"There are {_contestents.Count.ToString()} people in the giveaway" : "There is no giveaway running currently";
        }

        private static void WinnerWinner()
        {
            var random = new Random();

            if (_contestents.Count == 0)
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "No one entered the giveaway :(");
                IsGiveawayOn = false;
                return;
            }

            var winner = _contestents[random.Next(0, _contestents.Count)];
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Congratulations @{winner}! You have won the giveaway PogChamp");
            _contestents.Remove(winner);
            messageLimter.AddMessageCount();
            IsGiveawayOn = false;
        }

        public void ReRoll()
        {
            if (_contestents.Count > 0)
            {
                WinnerWinner();
                return;
            }
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName,"There is no giveaway running!");
            messageLimter.AddMessageCount();
        }
    }
}
