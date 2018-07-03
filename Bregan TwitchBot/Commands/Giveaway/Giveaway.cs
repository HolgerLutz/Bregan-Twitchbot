using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Bregan_TwitchBot.Connection;

namespace Bregan_TwitchBot.Commands.Giveaway
{
    internal class
        Giveaway //TODO: Re-roll option, see time left and see the timer amount that is set
    {
        private static List<string> _contestents;
        private static int _timerAmount;
        public static bool IsGiveawayOn;
        private static Timer _giveawayTimer;

        public static void StartGiveaway()
        {
            _contestents = new List<string>();
            IsGiveawayOn = true;
            _giveawayTimer = new Timer {Interval = _timerAmount};
            _giveawayTimer.Start();
            _giveawayTimer.Elapsed += WinnerWinner;
        }

        private static void WinnerWinner(object sender, ElapsedEventArgs e)
        {
            var random = new Random();
            TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                $"Congratulations @{_contestents[random.Next(0, _contestents.Count)]}! You have won the giveaway PogChamp");
            IsGiveawayOn = false;
            _giveawayTimer.Dispose();
            Console.WriteLine("[Giveaway] User successfully won");
        }

        public static void SetTimerAmount(int time)
        {
            try //Make sure the number entered is not bigger than max int
            {
                _timerAmount = time * 100;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void AddContestant(string username)
        {
            if (IsGiveawayOn && !_contestents.Contains(username))
            {
                _contestents.Add(username);
                Console.WriteLine($"[Giveaway] {DateTime.Now}: {username} has entered the giveaway");
            }
        }

        public static string AmountOfContestantsEntered()
        {
            return IsGiveawayOn ? _contestents.Count.ToString() : "There is no giveaway running currently";
        }


    }
}
