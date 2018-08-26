using System;
using System.Timers;
using BreganTwitchBot.Connection;
using TwitchLib.Api.Enums;
using TwitchLib.Api.Exceptions;

namespace BreganTwitchBot.TwitchCommands.MessageLimiter
{
    class CommandLimiter
    {
        public static int MessageLimit;
        public static int MessagesSent;
        public static void SetMessageLimit()
        {
            try
            {
                var userList = TwitchApiConnection.ApiClient.Undocumented.GetChattersAsync(StartService.ChannelName).Result;
                foreach (var username in userList)
                {
                    if (username.Username == StartService.BotName && username.UserType == UserType.Moderator)
                    {
                        MessageLimit = 95;
                        return;
                    }

                    MessageLimit = 15;
                }
            }
            catch (BadGatewayException)
            {
                Console.WriteLine("[Command Limiter] BadGatewayException error has occured while attempting to check bot rank");
                throw;
            }
            catch (InternalServerErrorException)
            {
                Console.WriteLine("[Command Limiter] InternalServerErrorException error has occured while attempting to check bot rank");
                throw;
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
        }

        public static void AddMessageCount()
        {
            MessagesSent += 1;
        }
    }
}
