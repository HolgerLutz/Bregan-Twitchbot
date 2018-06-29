using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Bregan_TwitchBot.Connection;
using Newtonsoft.Json;
using TwitchLib.Api.Enums;
using TwitchLib.Client.Extensions;

namespace Bregan_TwitchBot.Commands.Message_Limiter
{
    internal class CommandLimiter
    {
        public static int MessageLimit;
        public static int MessagesSent;
        public static void SetMessageLimit()
        {
            var userList = TwitchApiConnection.ApiClient.Undocumented.GetChattersAsync("blocksssssss").Result;


            foreach (var test in userList)
            {
                if (test.Username == StartService.BotName && test.UserType == UserType.Moderator)
                {
                    MessageLimit = 95;
                    Console.WriteLine(MessageLimit);
                    return;
                }
                MessageLimit = 15;
            }
        }

        public static void ResetMessageLimit()
        {
            var limitTimer = new Timer(30000);
            limitTimer.Elapsed += ResetMessagesSent;
            limitTimer.Start();
        }

        private static void ResetMessagesSent(object sender, ElapsedEventArgs e)
        {
            MessagesSent = 0;
            Console.WriteLine($"[Rate Limiter] {DateTime.Now}: Messages sent set to 0");
        }

        public static void AddMessageCount()
        {
            MessagesSent += 1;
        }
    }
}
