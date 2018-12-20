using System;
using System.Collections.Generic;
using BreganTwitchBot.Connection;
using Serilog;
using TwitchLib.Api.Core.Exceptions;
using Timer = System.Timers.Timer;

namespace BreganTwitchBot.TwitchCommands.RandomUser
{
    class RandomUsers
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
            catch (BadGatewayException)
            {
                Log.Fatal("[Random User] BadGatewayException when attempting to receive the user list");
            }
            catch (InternalServerErrorException)
            {
                Log.Fatal("[Random User] InternalServerErrorException when attempting to receive the user list");
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
            catch (BadGatewayException)
            {
                Log.Fatal("[Random User] BadGatewayException when attempting to receive the user list");
            }
            catch (InternalServerErrorException)
            {
                Log.Fatal("[Random User] InternalServerErrorException when attempting to receive the user list");
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
