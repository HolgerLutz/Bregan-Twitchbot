using System;
using System.Collections.Generic;
using BreganTwitchBot.Connection;
using TwitchLib.Api.Exceptions;
using Timer = System.Timers.Timer;

namespace BreganTwitchBot.TwitchCommands.RandomUser
{
    class RandomUser
    {
        private static List<string> _userList;

        public static void StartGetChattersTimer()
        {
            _userList = new List<string>();

            try
            {
                var userList = TwitchApiConnection.ApiClient.Undocumented.GetChattersAsync(StartService.ChannelName).Result; 
                foreach (var user in userList) //On startup the list needs to be populated
                {
                    _userList.Add(user.Username);
                }
            }
            catch (BadGatewayException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("it broke");
                throw;
            }
            catch (InternalServerErrorException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("it broke");
                throw;
            }
            var timer = new Timer(20000);
            timer.Start();
            timer.Elapsed += Timer_Elapsed;
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _userList.Clear();
            try
            {
                var userList = TwitchApiConnection.ApiClient.Undocumented.GetChattersAsync(StartService.ChannelName).Result;
                foreach (var user in userList)
                {
                    _userList.Add(user.Username);
                }
            }
            catch (BadGatewayException errorException)
            {
                Console.WriteLine(errorException);
                Console.WriteLine("it broke");
                throw;
            }
            catch (InternalServerErrorException errorException)
            {
                Console.WriteLine(errorException);
                Console.WriteLine("it broke");
                throw;
            }
        }

        public static string SelectRandomUser()
        {
            var random = new Random();
            var number = random.Next(0, _userList.Count);
            return _userList[number];
        }
    }
}
