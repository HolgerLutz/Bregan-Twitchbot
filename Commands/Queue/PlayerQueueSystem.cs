using System;
using System.Collections.Generic;

namespace Bregan_TwitchBot.Commands.Queue
{
    internal class PlayerQueueSystem
    {
        private static List<string> _playerQueue;
        //TODO: Move Console logging to BotLogging.cs? 

        //Create queue as soon as the bot starts
        public static void QueueCreate()
        {
            _playerQueue = new List<string>();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Player Queue] Player queue sucessfully created");
            Console.ResetColor();
        }
        //Check if user is in queue - don't want to add same person to queue twice
        public static bool QueueUserCheck(string user)
        {
            return _playerQueue.Contains(user); 
        }
        //Add user to the end of the player queue
        public static void QueueAdd(string user)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Player Queue]{DateTime.Now}: {user} has joined the queue");
            Console.ResetColor();
            _playerQueue.Add(user);
        }
        //Users can leave the queue if they want
        public static void QueueRemove(string user)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Player Queue]{DateTime.Now}: {user} has left the queue");
            Console.ResetColor();
            _playerQueue.Remove(user);
        }
        //Remove the first 3 people of the queue TODO: have an option in config to change how many people?
        public static void QueueRemove3()
        {
            if (_playerQueue.Count <= 3) //If theres only 3 or less then the whole queue can be wiped clean
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Player Queue]{DateTime.Now}: The queue has been cleared");
                Console.ResetColor();
                _playerQueue.Clear();
                return;
            }

            for (int i = 2; i >= 0; i--) //If there are more than 3 then loop through the first 3 people
            {
                _playerQueue.RemoveAt(i);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Player Queue]{DateTime.Now}: The queue has removed 4 people");
            Console.ResetColor();
        }

        public static void QueueClear() //Wipes queue completely ignoring amount of people
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
            return String.Join(", ", _playerQueue); //Just show everyone in the queue and splitting them with a comma
        }

        //TODO: add their queue position? 

    }
}
