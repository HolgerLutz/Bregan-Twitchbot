using System;
using System.Collections.Generic;

namespace Bregan_TwitchBot.Commands.Queue
{
    internal class PlayerQueueSystem
    {
        private static List<string> _playerQueue;
        //TODO: Move Console logging to BotLogging.cs? 
        public static void QueueCreate()
        {
            _playerQueue = new List<string>();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Player Queue] Player queue sucessfully created");
            Console.ResetColor();
        }

        public static bool QueueUserCheck(string user)
        {
            return _playerQueue.Contains(user); 
        }

        public static void QueueAdd(string user)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Player Queue]{DateTime.Now}: {user} has joined the queue");
            Console.ResetColor();
            _playerQueue.Add(user);
        }

        public static void QueueRemove(string user)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Player Queue]{DateTime.Now}: {user} has left the queue");
            Console.ResetColor();
            _playerQueue.Remove(user);
        }

        public static void QueueRemove3()
        {
            if (_playerQueue.Count <= 3)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Player Queue]{DateTime.Now}: The queue has been cleared");
                Console.ResetColor();
                _playerQueue.Clear();
                return;
            }

            for (int i = 2; i >= 0; i--)
            {
                _playerQueue.RemoveAt(i);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Player Queue]{DateTime.Now}: The queue has removed 4 people");
            Console.ResetColor();
        }

        public static void QueueClear()
        {
            _playerQueue.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Player Queue] {DateTime.Now}: The queue has been cleared completly");
            Console.ResetColor();
        }

        public static string NextGamePlayers()
        {

            //If the queue contains less than 4 players return the current amount
            if (_playerQueue.Count <= 3)
            {
                return String.Join(", ", _playerQueue);
            }

            //If over 4 players in the queue get the first 4

            var nextPlayers = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                nextPlayers.Add(_playerQueue[i]);
            }

            return String.Join(", ", nextPlayers);
        }

        public static string CurrentQueue()
        {
            return String.Join(", ", _playerQueue);
        }

    }
}
