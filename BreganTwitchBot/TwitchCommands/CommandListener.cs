using System;
using BreganTwitchBot.Connection;
using BreganTwitchBot.Database;
using BreganTwitchBot.TwitchCommands.DadJokes;
using BreganTwitchBot.TwitchCommands.Gambling;
using BreganTwitchBot.TwitchCommands.Giveaway;
using BreganTwitchBot.TwitchCommands.MessageLimiter;
using BreganTwitchBot.TwitchCommands.Queue;
using BreganTwitchBot.TwitchCommands.RandomUser;
using BreganTwitchBot.TwitchCommands.SongRequests;
using BreganTwitchBot.TwitchCommands.WordBlacklister;
using BreganTwitchBot.TwitchCommands._8Ball;
using Serilog;

namespace BreganTwitchBot.TwitchCommands
{
    class CommandListener
    {
        private static DateTime _spinCooldown;
        private static DateTime _55x2Cooldown;

        public static void CommandListenerSetup()
        {
            TwitchBotConnection.Client.OnChatCommandReceived += Commands;
        }

        private static void Commands(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
        {
            //Giveaways & the player queue have multiple commands. To prevent errors it is defined before the switch statement
            var giveaway = new Giveaways();
            var queue = new PlayerQueueSystem();
            var wordBlacklist = new WordBlackList();
            var messageLimiter = new CommandLimiter();
            var databaseQuery = new DatabaseQueries();
            var songRequest = new SongRequest();
            //Message limit checker
            if (messageLimiter.CheckMessageLimit())
            {
                Log.Warning("[Message Limiter] Message Limit Hit");
                return;
            }

            string message;
            //General pre-programmed commands
            switch (e.Command.CommandText.ToLower())
            {
                case "8ball":
                    var eightBall = new EightBall();
                    message = eightBall.Ask8Ball();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "dadjoke":
                    var dadJoke = new DadJoke();
                    message = dadJoke.DadJokeGenerate().Result;
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "commands":
                    message = $"@{e.Command.ChatMessage.Username}You can find the commands at https://github.com/Bregann/Bregan-Twitchbot#bregan-twitchbot";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "pitchfork":
                    var randomUser = new RandomUsers();
                    message = $"{e.Command.ChatMessage.Username} just pitchforked -------E {randomUser.SelectRandomUser()}";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "shoutout" when e.Command.ChatMessage.IsModerator:
                case "shoutout" when e.Command.ChatMessage.IsBroadcaster:
                case "so" when e.Command.ChatMessage.IsModerator:
                case "so" when e.Command.ChatMessage.IsBroadcaster:
                    message = $"Hey go check out {e.Command.ArgumentsAsString.Replace("@", "")} at twitch.tv/{e.Command.ArgumentsAsString.Replace("@", "").Trim()} for some great content!";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                //Giveaway commands
                case "startgiveaway" when e.Command.ChatMessage.IsModerator:
                case "startgiveaway" when e.Command.ChatMessage.IsBroadcaster:
                    giveaway.StartGiveaway();
                    break;

                case "joingiveaway":
                    giveaway.AddContestant(e.Command.ChatMessage.Username);
                    break;

                case "amountentered" when e.Command.ChatMessage.IsModerator:
                case "amountentered" when e.Command.ChatMessage.IsBroadcaster:
                    message = $"{giveaway.AmountOfContestantsEntered()}";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "setgiveawaytime" when e.Command.ChatMessage.IsModerator:
                case "setgiveawaytime" when e.Command.ChatMessage.IsBroadcaster:
                    giveaway.SetTimerAmount(e.Command.ArgumentsAsString, e.Command.ChatMessage.Username);
                    break;

                case "reroll" when e.Command.ChatMessage.IsModerator:
                case "reroll" when e.Command.ChatMessage.IsBroadcaster:
                    giveaway.ReRoll();
                    break;

                //Queue Commands
                case "joinqueue" when queue.QueueUserCheck(e.Command.ChatMessage.Username) == false:
                    queue.QueueAdd(e.Command.ChatMessage.Username);
                    break;

                case "leavequeue":
                    queue.QueueRemove(e.Command.ChatMessage.Username);
                    break;

                case "queue":
                    message = $"The current queue is {queue.CurrentQueue()}";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "nextgame":
                    message = $"The next players for the game are {queue.NextGamePlayers()}";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "queueposition":
                    message = queue.GetQueuePosition(e.Command.ChatMessage.Username);
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "removegame" when e.Command.ChatMessage.IsModerator:
                case "removegame" when e.Command.ChatMessage.IsBroadcaster:
                    queue.QueueRemovePlayersAmount();
                    message = $"{e.Command.ChatMessage.Username}: the current players have been removed";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "clearqueue" when e.Command.ChatMessage.IsModerator:
                case "clearqueue" when e.Command.ChatMessage.IsBroadcaster:
                    queue.QueueClear();
                    break;

                case "setremoveamount" when e.Command.ChatMessage.IsModerator:
                case "setremoveamount" when e.Command.ChatMessage.IsBroadcaster:
                    queue.SetQueueRemoveAmount(e.Command.ArgumentsAsString);
                    break;

                //Bad word filter
                case "addbadword" when e.Command.ChatMessage.IsModerator:
                    wordBlacklist.AddBadWord(e.Command.ArgumentsAsString);
                    break;

                case "removebadword" when e.Command.ChatMessage.IsModerator:
                    wordBlacklist.RemoveBadWord(e.Command.ArgumentsAsString);
                    break;

                    //Points/time
                case "points":
                    if (e.Command.ChatMessage.Message.ToLower() == "!points")
                    {
                        message = $"@{e.Command.ChatMessage.Username} => You have {databaseQuery.GetUserPoints(e.Command.ChatMessage.Username):N0} points";
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                        Log.Information($"[Twitch Message Sent] {message}");
                        break;
                    }

                    message = $"@{e.Command.ChatMessage.Username} => {e.Command.ArgumentsAsString} has {databaseQuery.GetUserPoints(e.Command.ArgumentsAsString.ToLower()):N0} points";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "hours":
                case "hrs":
                    if (e.Command.ChatMessage.Message.ToLower() == "!hours" || e.Command.ChatMessage.Message.ToLower() == "!hrs") //If the user is checking their own time
                    {
                        var time = databaseQuery.GetUserTime(e.Command.ChatMessage.Username);
                        message = $"{e.Command.ChatMessage.Username} => You have {time.TotalMinutes} minutes (about {Math.Round(time.TotalMinutes / 60, 2)} hours) in the stream";
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                        Log.Information($"[Twitch Message Sent] {message}");
                        messageLimiter.AddMessageCount();
                        break;
                    }
                    
                    var otherUserTime = databaseQuery.GetUserTime(e.Command.ArgumentsAsString.Replace('@', ' ').Trim().ToLower());
                    message = $"{e.Command.ChatMessage.Username} => {e.Command.ArgumentsAsString} has {otherUserTime.TotalMinutes} minutes (about {Math.Round(otherUserTime.TotalMinutes / 60, 2)} hours) in the stream";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "pointslb":

                    break;

                //Gamble
                case "gamble":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,"Use !spin <points> or !spin all if you want to risk them all!");
                    break;
                case "spin":
                    //Cooldown Check
                    if (DateTime.Now - TimeSpan.FromSeconds(5) <= _spinCooldown)
                    {
                        //var sinceListSpin = DateTime.Now - _spinCooldown;
                        //var coolDownLeft = TimeSpan.FromSeconds(5) - sinceListSpin;
                        return;
                   }

                    if (e.Command.ChatMessage.Message.ToLower() == "!spin all")
                    {
                        var userPointAmount = databaseQuery.GetUserPoints(e.Command.ChatMessage.Username);
                        if (userPointAmount < 100)
                        {
                            message = "It's 100 points minimum!";
                            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                            Log.Information($"[Twitch Message Sent] {message}");
                            messageLimiter.AddMessageCount();
                            break;
                        }
                        var slotMachine = new SlotMachine();
                        slotMachine.SpinSlotMachine(e.Command.ChatMessage.Username, userPointAmount);
                        _spinCooldown = DateTime.Now;
                        break;
                    }

                    //Convert the points in the spin command to int
                    var intConvert = long.TryParse(e.Command.ArgumentsAsString, out var pointsBeingGambled);

                    //Validate if user has enough points and is above the min limit

                    if (intConvert && pointsBeingGambled >= 100 && databaseQuery.HasEnoughPoints(e.Command.ChatMessage.Username, pointsBeingGambled))
                    {
                        var slotMachine = new SlotMachine();
                        slotMachine.SpinSlotMachine(e.Command.ChatMessage.Username, pointsBeingGambled);
                        _spinCooldown = DateTime.Now;
                        break;
                    }
                    else if(intConvert && pointsBeingGambled <= 100) //Minimum on the points to stop abuse
                    {
                        message = "It's 100 points minimum!";
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                        Log.Information($"[Twitch Message Sent] {message}");
                        messageLimiter.AddMessageCount();
                        break;
                    }
                    else if (databaseQuery.HasEnoughPoints(e.Command.ChatMessage.Username, pointsBeingGambled) == false) //Check if they don't have enough points
                    {
                        message = "You don't have enough points! Check with !points how many you have";
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                        Log.Information($"[Twitch Message Sent] {message}");
                        messageLimiter.AddMessageCount();
                        break;
                    }

                    message = "Hey try using !spin <points> or !spin all";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "55x2":
                    long.TryParse(e.Command.ArgumentsAsString, out var gambledPointsResult);

                    //Validation that it can be gambled
                    if (DateTime.Now - TimeSpan.FromSeconds(5) <= _55x2Cooldown)
                    {
                        return;
                    }

                    if (gambledPointsResult == 0 || gambledPointsResult < 500)
                    {
                        message = $"@{e.Command.ChatMessage.Username} => It's 500 points minimum!";
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                        Log.Information($"[Twitch Message Sent] {message}");
                        messageLimiter.AddMessageCount();
                        break;
                    }

                    if (databaseQuery.HasEnoughPoints(e.Command.ChatMessage.Username, gambledPointsResult) == false)
                    {
                        message = $"@{e.Command.ChatMessage.Username} => You don't have enough points!";
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                        Log.Information($"[Twitch Message Sent] {message}");
                        messageLimiter.AddMessageCount();
                        break;
                    }

                    var random = new Random().Next(1,101);
                    if (random > 55)
                    {
                        var pointsWon = (long) Math.Round(gambledPointsResult * 1.5);
                        message = $"@{e.Command.ChatMessage.Username}=> You rolled a {random} and won! You have won {pointsWon:N0} points";
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                        databaseQuery.AddUserPoints(e.Command.ChatMessage.Username, pointsWon);
                        Log.Information($"[Twitch Message Sent] {message}");
                        messageLimiter.AddMessageCount();
                        _55x2Cooldown = DateTime.Now;
                        break;
                    }

                    message = $"@{e.Command.ChatMessage.Username}=> You rolled a {random} and lost :( You have lost {gambledPointsResult:N0} points";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    databaseQuery.RemoveUserPoints(e.Command.ChatMessage.Username, gambledPointsResult);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    _55x2Cooldown = DateTime.Now;
                    break;

                case "jackpot":
                    message = $"The current jackpot is {databaseQuery.GetJackpotAmount():N0} points";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "sr":
                case "songrequest":
                    int srCooldown;
                    int srCost;

                    if (e.Command.ChatMessage.Username.ToLower() == "guinea")
                    {
                        srCooldown = 0;
                        srCost = 0;
                    }
                    else if (e.Command.ChatMessage.IsSubscriber || e.Command.ChatMessage.IsModerator )
                    {
                        srCooldown = 3;
                        srCost = 2000;
                    }
                    else
                    {
                        srCooldown = 6;
                        srCost = 3000;
                    }

                    if (e.Command.ChatMessage.Message.ToLower().Contains("youtube") || e.Command.ChatMessage.Message.ToLower().Contains("youtu.be"))
                    {
                        if (databaseQuery.HasEnoughPoints(e.Command.ChatMessage.Username, srCost) == false)
                        {
                            message = $"{e.Command.ChatMessage.Username} => You need {srCost:N0} points for a song request!";
                            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                            Log.Information($"[Twitch Message Sent] {message}");
                            messageLimiter.AddMessageCount();
                            break;
                        }

                        if (songRequest.IsSongBlacklisted(e.Command.ArgumentsAsString))
                        {
                            message = $"@{e.Command.ChatMessage.Username} => The song you have requested is blacklisted";
                            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                            Log.Information($"[Twitch Message Sent] {message}");
                            messageLimiter.AddMessageCount();
                            break;
                        }

                        if (songRequest.CheckCooldown(e.Command.ChatMessage.Username, srCooldown) == false)
                        {
                            break;
                        }

                        if (songRequest.CheckCooldown(e.Command.ChatMessage.Username, srCooldown))
                        {
                            songRequest.SendSong(e.Command.ArgumentsAsString, e.Command.ChatMessage.Username, srCost);
                            message = $"@{e.Command.ChatMessage.Username} => Your song has been sent";
                            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                            databaseQuery.UpdateLastSongRequest(e.Command.ChatMessage.Username);
                            Log.Information($"[Twitch Message Sent] {message}");
                            messageLimiter.AddMessageCount();
                            break;
                        }
                    }

                    message = $"{e.Command.ChatMessage.Username} => You need to put in a YouTube link for a song request!";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                case "blacklistsong" when e.Command.ChatMessage.IsModerator:
                    songRequest.AddBlacklistedSong(e.Command.ArgumentsAsString);
                    break;

                case "addpoints" when e.Command.ChatMessage.Username == "guinea":
                case "addpoints" when e.Command.ChatMessage.IsBroadcaster:
                    if (e.Command.ChatMessage.Message.ToLower() == "!addpoints")
                    {
                        message = $"@{e.Command.ChatMessage.Username} => The usage of this command is !addpoints <username> <points>";
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                        Log.Information($"[Twitch Message Sent] {message}");
                        messageLimiter.AddMessageCount();
                        break;
                    }

                    var userandPointsSplit = e.Command.ArgumentsAsList;
                    var usernameForPoints = userandPointsSplit[0].ToLower();
                    long.TryParse(userandPointsSplit[1], out var pointsToAdd);

                    if (pointsToAdd == 0)
                    {
                        message = $"@{e.Command.ChatMessage.Username} => The usage of this command is !addpoints <username> <points>";
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                        Log.Information($"[Twitch Message Sent] {message}");
                        messageLimiter.AddMessageCount();
                    }

                    databaseQuery.AddUserPoints(usernameForPoints, pointsToAdd);
                    break;
            }

        }
    }
}
