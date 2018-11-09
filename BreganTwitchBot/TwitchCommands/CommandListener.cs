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
using BreganTwitchBot.TwitchCommands.Supermods;
using BreganTwitchBot.TwitchCommands.WordBlacklister;
using BreganTwitchBot.TwitchCommands._8Ball;
using Serilog;

namespace BreganTwitchBot.TwitchCommands
{
    class CommandListener
    {
        private static DateTime _gambleCooldown;
        private static bool _srToggle;
        public static void CommandListenerSetup()
        {
            TwitchBotConnection.Client.OnChatCommandReceived += Commands;
            _srToggle = false;
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
            var supermod = new Supermod();
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
                    message = $"@{e.Command.ChatMessage.Username} => You can find the commands at https://github.com/Bregann/Bregan-Twitchbot#bregan-twitchbot";
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

                    message = $"@{e.Command.ChatMessage.Username} => {e.Command.ArgumentsAsString.Replace("@", "")} has {databaseQuery.GetUserPoints(e.Command.ArgumentsAsString.ToLower().Replace("@", "")):N0} points";
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
                    
                    var otherUserTime = databaseQuery.GetUserTime(e.Command.ArgumentsAsString.Replace("@","").ToLower());
                    message = $"{e.Command.ChatMessage.Username} => {e.Command.ArgumentsAsString.Replace("@", "")} has {otherUserTime.TotalMinutes} minutes (about {Math.Round(otherUserTime.TotalMinutes / 60, 2)} hours) in the stream";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                //Leaderboards for points/hours
                case "pointslb":
                    var points = databaseQuery.GetTopPoints();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, points);
                    Log.Information($"[Twitch Message Sent] {points}");
                    messageLimiter.AddMessageCount();
                    break;

                case "hourslb":
                    var hours = databaseQuery.GetTopHours();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, hours);
                    Log.Information($"[Twitch Message Sent] {hours}");
                    messageLimiter.AddMessageCount();
                    break;

                //Gamble
                case "gamble":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,"Use !spin <points> or !spin all if you want to risk them all!");
                    break;

                case "spin":
                    //Cooldown Check
                    if (DateTime.Now - TimeSpan.FromSeconds(10) <= _gambleCooldown)
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
                        _gambleCooldown = DateTime.Now;
                        break;
                    }

                    //Convert the points in the spin command to int
                    var intConvert = long.TryParse(e.Command.ArgumentsAsString, out var pointsBeingGambled);

                    //Validate if user has enough points and is above the min limit

                    if (intConvert && pointsBeingGambled >= 100 && databaseQuery.HasEnoughPoints(e.Command.ChatMessage.Username, pointsBeingGambled))
                    {
                        var slotMachine = new SlotMachine();
                        slotMachine.SpinSlotMachine(e.Command.ChatMessage.Username, pointsBeingGambled);
                        _gambleCooldown = DateTime.Now;
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
                    if (DateTime.Now - TimeSpan.FromSeconds(10) <= _gambleCooldown)
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
                        _gambleCooldown = DateTime.Now;
                        break;
                    }

                    message = $"@{e.Command.ChatMessage.Username}=> You rolled a {random} and lost :( You have lost {gambledPointsResult:N0} points";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    databaseQuery.RemoveUserPoints(e.Command.ChatMessage.Username, gambledPointsResult);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    _gambleCooldown = DateTime.Now;
                    break;

                case "jackpot":
                    message = $"The current jackpot is {databaseQuery.GetJackpotAmount():N0} points";
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                    Log.Information($"[Twitch Message Sent] {message}");
                    messageLimiter.AddMessageCount();
                    break;

                //Song requests
                case "sr":
                case "songrequest":
                    int srCooldown;
                    int srCost;

                    //Are song requests disabled?
                    if (_srToggle)
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => Song requests are disabled!");
                        Log.Information($"[Twitch Message Sent] @{e.Command.ChatMessage.Username} => Song requests are disabled!");
                        break;
                    }

                    //Find out how much the SR is. Supermods get free song requests as they are trusted
                    if (supermod.CheckSupermod(e.Command.ChatMessage.Username))
                    {
                        songRequest.SendSong(e.Command.ArgumentsAsString, e.Command.ChatMessage.Username, 0);
                        message = $"@{e.Command.ChatMessage.Username} => Your song has been sent";
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                        databaseQuery.UpdateLastSongRequest(e.Command.ChatMessage.Username);
                        Log.Information($"[Twitch Message Sent] {message}");
                        break;
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
                    //Check if the user has enough points
                    if (databaseQuery.HasEnoughPoints(e.Command.ChatMessage.Username, srCost) == false)
                    {
                        message = $"{e.Command.ChatMessage.Username} => You need {srCost:N0} points for a song request!";
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                        Log.Information($"[Twitch Message Sent] {message}");
                        messageLimiter.AddMessageCount();
                        break;
                    }


                    if (e.Command.ChatMessage.Message.ToLower().Contains("youtube") || e.Command.ChatMessage.Message.ToLower().Contains("youtu.be"))
                    {

                        if (songRequest.IsSongBlacklisted(e.Command.ArgumentsAsString))
                        {
                            message = $"@{e.Command.ChatMessage.Username} => The song you have requested is blacklisted";
                            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, message);
                            Log.Information($"[Twitch Message Sent] {message}");
                            messageLimiter.AddMessageCount();
                            break;
                        }

                        //If they are on cooldown the message will be sent in the CheckCooldown method
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

                case "togglesr" when supermod.CheckSupermod(e.Command.ChatMessage.Username):
                case "togglesr" when e.Command.ChatMessage.IsBroadcaster:
                case "srtoggle" when supermod.CheckSupermod(e.Command.ChatMessage.Username):
                case "srtoggle" when e.Command.ChatMessage.IsBroadcaster:
                    _srToggle = !_srToggle;

                    if (_srToggle)
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => Song requests have been disabled");
                        Log.Information($"[Twitch Message Sent] {e.Command.ChatMessage.Username} => Song requests have been disabled");
                        messageLimiter.AddMessageCount();
                    }
                    else
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => Song requests have been enabled");
                        Log.Information($"[Twitch Message Sent] {e.Command.ChatMessage.Username} => Song requests have been enabled");
                        messageLimiter.AddMessageCount();
                    }

                    break;
                case "blacklistsong" when e.Command.ChatMessage.IsModerator:
                    songRequest.AddBlacklistedSong(e.Command.ArgumentsAsString);
                    break;

                case "addpoints" when supermod.CheckSupermod(e.Command.ChatMessage.Username):
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

                //Supermods
                case "addsupermod" when supermod.CheckSupermod(e.Command.ChatMessage.Username):
                case "addsupermod" when e.Command.ChatMessage.Username == e.Command.ChatMessage.BotUsername:
                    supermod.AddSupermod(e.Command.ArgumentsAsList[0].ToLower(), e.Command.ChatMessage.Username);
                    break;
                case "removesupermod" when e.Command.ChatMessage.IsBroadcaster:
                case "removesupermod" when e.Command.ChatMessage.Username == e.Command.ChatMessage.BotUsername:
                    supermod.RemoveSupermod(e.Command.ArgumentsAsList[0].ToLower(), e.Command.ChatMessage.Username);
                    break;
            }


        }
    }
}
