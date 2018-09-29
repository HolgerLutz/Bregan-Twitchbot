using System;
using System.Collections.Generic;
using System.Timers;
using BreganTwitchBot.Connection;
using BreganTwitchBot.TwitchCommands.MessageLimiter;
using Serilog;

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
            var commandLimiter = new CommandLimiter();
            //Don't want to be starting multiple giveaways at once - this is by default set to false in StartService.cs
            if (IsGiveawayOn)
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "There is already a giveaway running!");
                commandLimiter.AddMessageCount();
                return;
            }
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "A new giveaway has started! Do !joingiveaway to join!");
            _contestents = new List<string>();
            IsGiveawayOn = true;
            _giveawayTimer = new Timer {Interval = TimerAmount};
            _giveawayTimer.Start();
            _giveawayTimer.AutoReset = false;
            _giveawayTimer.Elapsed += TimerWinner;
        }

        private static void TimerWinner(object sender, ElapsedEventArgs e)
        {
            //First winner 
            var giveawayWinner = new Giveaways();
            giveawayWinner.WinnerWinner();
            Log.Information("[Giveaway] User successfully won");
            _giveawayTimer.Dispose();
        }

        public void SetTimerAmount(string timeCommand, string username)
        {
            var commandLimiter = new CommandLimiter();
            try //Make sure the number entered is not bigger than max int
            {
                var time = int.Parse(timeCommand);
                TimerAmount = time * 1000; //Times by 1000 to convert to milliseconds
                Console.ForegroundColor = ConsoleColor.Yellow;
                Log.Information("[Giveaway] The giveaway time has been updated to {time}");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName,$"@{username} => The giveaway time has been updated to {time} seconds");
                commandLimiter.AddMessageCount();
                Console.ResetColor();
            }
            catch (OverflowException exception)
            {
                Console.WriteLine(exception);
                Console.ForegroundColor = ConsoleColor.Red;
                Log.Warning("[Giveaway] User attempted to break the bot (OverflowException)");
                Console.ResetColor();
            }
            catch (FormatException exception)
            {
                Console.WriteLine(exception);
                Console.ForegroundColor = ConsoleColor.Red;
                Log.Warning("[Giveaway] User attempted to break the bot (FormatException)");
                Console.ResetColor();
            }
        }

        public void AddContestant(string username)
        {
            if (IsGiveawayOn && !_contestents.Contains(username))
            {
                _contestents.Add(username);
                Log.Information($"[Giveaway] {username} has entered the giveaway");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{username} => You have entered the giveaway. Good luck!");
                return;
            }

            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "There is no giveaway running currently");
        }

        public string AmountOfContestantsEntered()
        {
            return IsGiveawayOn ? $"There are {_contestents.Count.ToString()} people in the giveaway" : "There is no giveaway running currently";
        }

        private void WinnerWinner()
        {
            var random = new Random();

            if (_contestents.Count == 0)
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "No one entered the giveaway :(");
                _giveawayTimer.Dispose();
                IsGiveawayOn = false;
                return;
            }
            var commandLimiter = new CommandLimiter();
            var winner = _contestents[random.Next(0, _contestents.Count)];
            var message = $"Congratulations @{winner}! You have won the giveaway PogChamp";
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
            Log.Information($"[Twitch Message Sent] {message}");
            _contestents.Remove(winner);
            commandLimiter.AddMessageCount();
            IsGiveawayOn = false;
        }

        public void ReRoll()
        {
            var commandLimiter = new CommandLimiter();
            if (_contestents.Count > 0)
            {
                WinnerWinner();
                return;
            }

            var message = "There is no giveaway running!";
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
            Log.Information($"[Twitch Message Sent] {message}");
            commandLimiter.AddMessageCount();
        }
    }
}
