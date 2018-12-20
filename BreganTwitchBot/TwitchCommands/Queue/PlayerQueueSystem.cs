using System;
using System.Collections.Generic;
using BreganTwitchBot.Connection;
using BreganTwitchBot.TwitchCommands.MessageLimiter;
using Serilog;

namespace BreganTwitchBot.TwitchCommands.Queue
{
    class PlayerQueueSystem
    {
        public static int QueueRemoveAmount;
        private static List<string> _playerQueue;

        //Create queue as soon as the bot starts
        public static void QueueCreate()
        {
            _playerQueue = new List<string>();
            QueueRemoveAmount = 3;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log.Information("[Player Queue] Player queue successfully created");
            Console.ResetColor();
        }
        //Check if user is in queue - don't want to add same person to queue twice
        public static bool QueueUserCheck(string user)
        {
            return _playerQueue.Contains(user); 
        }

        public static void QueueAdd(string user)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log.Information($"[Player Queue] {user} has joined the queue");
            Console.ResetColor();
            _playerQueue.Add(user);
        }

        public static void QueueRemove(string user)
        {
            Log.Information($"[Player Queue] {user} has left the queue");
            _playerQueue.Remove(user);
        }

        //Remove the set amount of people from the queue (default is 3 as set in QueueCreate)
        public static void QueueRemovePlayersAmount()
        {   
            //Wipe the whole list if the count of the queue is lower than the emote
            if (_playerQueue.Count <= QueueRemoveAmount) 
            {
                Log.Information("[Player Queue] The queue has been cleared");
                _playerQueue.Clear();
                return;
            }

            //More than the set amount then remove the first people in the list 
            for (int i = QueueRemoveAmount; i >= 0; i--)
            {
                _playerQueue.RemoveAt(i);
            }

            Log.Information($"[Player Queue] The queue has removed {QueueRemoveAmount} people");
        }

        public static void QueueClear()
        {
            _playerQueue.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log.Information("[Player Queue] The queue has been cleared completely");
            Console.ResetColor();
        }

        public static string NextGamePlayers()
        {

            //If the queue contains less than 4 players return the current amount
            if (_playerQueue.Count <= QueueRemoveAmount)
            {
                return string.Join(", ", _playerQueue);
            }

            //If over 4 players in the queue get the first 4

            var nextPlayers = new List<string>();

            for (int i = 0; i < QueueRemoveAmount; i++)
            {
                nextPlayers.Add(_playerQueue[i]);
            }
            return string.Join(", ", nextPlayers);
        }

        public static string CurrentQueue()
        {
            return string.Join(", ", _playerQueue); //Just show everyone in the queue and splitting them with a comma
        }

        public static void SetQueueRemoveAmount(string amount)
        {
            try
            {
                QueueRemoveAmount = int.Parse(amount);
                Console.ForegroundColor = ConsoleColor.Yellow;
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"The remove amount has been updated to {QueueRemoveAmount}");
                CommandLimiter.AddMessageCount();
                Log.Information($"[Player Queue] The queue remove amount has been set to {QueueRemoveAmount}");
                Console.ResetColor();
            }
            catch (FormatException)
            {
                Log.Warning("[Player Queue] A user attempted to break the bot! (Format Exeception)");
            }
            catch (OverflowException)
            {
                Log.Warning("[Player Queue] A user attempted to break the bot! (Overflow Exeception)");
            }


        }

        public static string GetQueuePosition(string username)
        {
            if (_playerQueue.Contains(username))
            {
                return $"Hey @{username} => Your queue position is {_playerQueue.IndexOf(username)}";
            }
            return "You aren't in the queue yet! Do !joinqueue";
        }

    }
}
