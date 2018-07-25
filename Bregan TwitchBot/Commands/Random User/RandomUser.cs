using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bregan_TwitchBot.Connection;
using TwitchLib.Api.Models.ThirdParty.UsernameChange;
using Timer = System.Timers.Timer;

namespace Bregan_TwitchBot.Commands.Random_User
{
    internal class RandomUser
    {
        private static List<string> _userList;

        public static void StartGetChattersTimer()
        {
            _userList = new List<string>();

            var userList = TwitchApiConnection.ApiClient.Undocumented.GetChattersAsync(StartService.ChannelName).Result; 
            foreach (var user in userList) //On startup the list needs to be populated
            {
                _userList.Add(user.Username);
            }

            var timer = new Timer(120000);
            timer.Start();
            timer.Elapsed += Timer_Elapsed;
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _userList.Clear();
            var userList = TwitchApiConnection.ApiClient.Undocumented.GetChattersAsync(StartService.ChannelName).Result;
            foreach (var user in userList)
            {
                _userList.Add(user.Username);
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
