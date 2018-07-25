using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Bregan_TwitchBot.Connection;
using Bregan_TwitchBot.Commands.Message_Limiter;

namespace Bregan_TwitchBot.Commands.Giveaway
{
    internal class Giveaways //TODO: see time left and see the timer amount that is set, set sub only, set follow only
    {
        private static List<string> _contestents;
        public static int TimerAmount;
        public static bool IsGiveawayOn;
        private static Timer _giveawayTimer;

        public static void StartGiveaway()
        {
            if (IsGiveawayOn)
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "There is already a giveaway running!");
                CommandLimiter.AddMessageCount();
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

        public static void SetTimerAmount(string timeCommand, string username)
        {
            

            try //Make sure the number entered is not bigger than max int
            {
                var trimAmount = timeCommand.Remove(0, 16); //Remove the command the space so its only left with the number
                var time = int.Parse(trimAmount);
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
                CommandLimiter.AddMessageCount();
                Console.ResetColor();
            }
            catch (FormatException exception)
            {
                Console.WriteLine(exception);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Giveaway] {DateTime.Now}: User attempted to break the bot (FormatException)");
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "Good attempt at breaking the bot (FormatException)");
                CommandLimiter.AddMessageCount();
                Console.ResetColor();
            }
        }

        public static void AddContestant(string username)
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

        public static string AmountOfContestantsEntered()
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
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Congratulations @{_contestents[random.Next(0, _contestents.Count)]}! You have won the giveaway PogChamp");
            CommandLimiter.AddMessageCount();
            IsGiveawayOn = false;

        }

        public static void ReRoll()
        {
            if (_contestents.Count > 0)
            {
                WinnerWinner();
                return;
            }
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName,"There is no giveaway running!");
            CommandLimiter.AddMessageCount();
        }
    }
}
