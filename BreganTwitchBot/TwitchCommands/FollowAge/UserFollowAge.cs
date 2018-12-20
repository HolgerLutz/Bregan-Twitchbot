using System;
using BreganTwitchBot.Connection;
using Humanizer;
using Humanizer.Localisation;
using Serilog;
using TwitchLib.Api.Core.Exceptions;

namespace BreganTwitchBot.TwitchCommands.FollowAge
{
    class UserFollowAge
    {
        public static string GetFollowAge(string username)
        {
            try
            {
                var getUserID = TwitchApiConnection.ApiClient.V5.Users.GetUserByNameAsync(username).Result.Matches;
                var checkFollow = TwitchApiConnection.ApiClient.Helix.Users.GetUsersFollowsAsync(fromId: getUserID[0].Id, toId: StartService.TwitchChannelID).Result;

                if (checkFollow.Follows.Length == 0) //If the response is empty then they do not follow the channel
                {
                    return $"@{username}=> You don't follow {StartService.ChannelName}! Why don't you drop a follow? ;)";
                }

                var followTime = checkFollow.Follows[0].FollowedAt;
                var followDifference = DateTime.Now - followTime;

                return $"@{username} => You have been following {StartService.ChannelName} for {followDifference.Humanize(maxUnit: TimeUnit.Year, minUnit: TimeUnit.Second, precision: 7)}";
            }
            catch (Exception exception)
            {
                Log.Fatal(exception.Message);
                return "An error has occured. Please try again";
            }
        }

        public static string GetOtherUserFollowAge(string usernameRequester, string username)
        {
            try
            {
                var getUserID = TwitchApiConnection.ApiClient.V5.Users.GetUserByNameAsync(username).Result.Matches;

                if (getUserID.Length == 0)
                {
                    return $"@{usernameRequester} => That username does not exist!";
                }

                var checkFollow = TwitchApiConnection.ApiClient.Helix.Users.GetUsersFollowsAsync(fromId: getUserID[0].Id, toId: StartService.TwitchChannelID).Result;

                if (checkFollow.Follows.Length == 0) //If the response is empty then they do not follow the channel
                {
                    return $"@{usernameRequester}=> {username} does not follow {StartService.ChannelName}!";
                }

                var followTime = checkFollow.Follows[0].FollowedAt;
                var followDifference = DateTime.Now - followTime;
                return $"@{usernameRequester} => {username} has been following {StartService.ChannelName} for {followDifference.Humanize(maxUnit: TimeUnit.Year, minUnit: TimeUnit.Second, precision: 7)}";
            }

            catch (AggregateException exception)
            {
                Log.Warning(exception.Message);
                return $"@{usernameRequester} => That username does not exist!";
            }

            catch (Exception exception)
            {
                Log.Fatal(exception.Message);
                return "An error has occured. Please try again";
            }

        }


    }
}
