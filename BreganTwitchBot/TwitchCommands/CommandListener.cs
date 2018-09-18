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
            var messageLimter = new CommandLimiter();
            var databaseQuery = new DatabaseQueries();
            //Message limit checker
            if (messageLimter.CheckMessageLimit())
            {
                Console.WriteLine("[Message Limiter] Message Limit Hit");
                return;
            }

            //General pre-programmed commands
            switch (e.Command.CommandText.ToLower())
            {
                case "8ball":
                    var eightBall = new EightBall();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, eightBall.Ask8Ball());
                    messageLimter.AddMessageCount();
                    break;
                case "dadjoke":
                    var dadJoke = new DadJoke();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, dadJoke.DadJokeGenerate().Result);
                    messageLimter.AddMessageCount();
                    break;
                case "commands":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "You can find the commands at https://github.com/Bregann/Bregan-Twitchbot#bregan-twitchbot");
                    messageLimter.AddMessageCount();
                    break;
                case "pitchfork":
                    var randomUser = new RandomUsers();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username} just pitchforked -------E {randomUser.SelectRandomUser()}");
                    messageLimter.AddMessageCount();
                    break;
                case "shoutout" when e.Command.ChatMessage.IsModerator:
                case "shoutout" when e.Command.ChatMessage.IsBroadcaster:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Hey go check out {e.Command.ArgumentsAsString.Replace("@", "")} at twitch.tv/{e.Command.ArgumentsAsString.Replace("@", "").Trim()} for some great content!");
                    messageLimter.AddMessageCount();
                    break;

                //Giveaway commands
                case "startgiveaway" when e.Command.ChatMessage.IsModerator:
                case "startgiveaway" when e.Command.ChatMessage.IsBroadcaster:
                    giveaway.StartGiveaway();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "A new giveaway has started! Do !joingiveaway to join!");
                    break;
                case "joingiveaway":
                    giveaway.AddContestant(e.Command.ChatMessage.Username);
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username} => You have entered the giveaway. Good luck!");
                    messageLimter.AddMessageCount();
                    break;
                case "amountentered" when e.Command.ChatMessage.IsModerator:
                case "amountentered" when e.Command.ChatMessage.IsBroadcaster:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{giveaway.AmountOfContestantsEntered()}");
                    messageLimter.AddMessageCount();
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
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,$"The current queue is {queue.CurrentQueue()}");
                    messageLimter.AddMessageCount();
                    break;
                case "nextgame":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,$"The next players for the game are {queue.NextGamePlayers()}");
                    messageLimter.AddMessageCount();
                    break;
                case "queueposition":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, queue.GetQueuePosition(e.Command.ChatMessage.Username));
                    messageLimter.AddMessageCount();
                    break;
                case "removegame" when e.Command.ChatMessage.IsModerator:
                case "removegame" when e.Command.ChatMessage.IsBroadcaster:
                    queue.QueueRemovePlayersAmount();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username}: the current players have been removed");
                    messageLimter.AddMessageCount();
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
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName,$"@{e.Command.ChatMessage.Username} => You have {DatabaseQueries.GetUserPoints(e.Command.ChatMessage.Username).ToString()} points");
                        break;
                    }
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => {e.Command.ArgumentsAsString} has {DatabaseQueries.GetUserPoints(e.Command.ArgumentsAsString)} points");
                    break;
                case "hours":
                    if (e.Command.ChatMessage.Message.ToLower() == "!hours") //If the user is checking their own time
                    {
                        var time = DatabaseQueries.GetUserTime(e.Command.ChatMessage.Username);
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username} => You have {time.TotalMinutes} minutes (about {Math.Round(time.TotalMinutes / 60, 2)} hours) in the stream");
                        messageLimter.AddMessageCount();
                        break;
                    }
                    var otherUser = e.Command.ChatMessage.Message.Remove(0, 7).Replace('@', ' ').Trim().ToLower(); //if the user is checking someone elses time
                    var otherUserTime = DatabaseQueries.GetUserTime(otherUser);

                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,$"{e.Command.ChatMessage.Username} => {otherUser} has {otherUserTime.TotalMinutes} minutes (about {Math.Round(otherUserTime.TotalMinutes / 60, 2)} hours) in the stream");
                    messageLimter.AddMessageCount();
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
                        var userPointAmount = DatabaseQueries.GetUserPoints(e.Command.ChatMessage.Username);
                        if (userPointAmount < 100)
                        {
                            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "It's 100 points minimum!");
                            break;
                        }
                        var slotMachine = new SlotMachine();
                        slotMachine.SpinSlotMachine(e.Command.ChatMessage.Username, userPointAmount);
                        _spinCooldown = DateTime.Now;
                        break;
                    }

                    //Convert the points in the spin command to int
                    var pointsFromCommand = e.Command.ChatMessage.Message.ToLower().Replace("!spin", "").Trim();
                    var intConvert = long.TryParse(pointsFromCommand, out var pointsBeingGambled);

                    //Validate if user has enough points and is above the min limit

                    if (intConvert && pointsBeingGambled >= 100 && DatabaseQueries.HasEnoughPoints(e.Command.ChatMessage.Username, pointsBeingGambled))
                    {
                        var slotMachine = new SlotMachine();
                        slotMachine.SpinSlotMachine(e.Command.ChatMessage.Username, pointsBeingGambled);
                        _spinCooldown = DateTime.Now;
                        break;
                    }
                    else if(intConvert && pointsBeingGambled <= 100) //Minimum on the points to stop abuse
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "It's 100 points minimum!");
                        messageLimter.AddMessageCount();
                        break;
                    }
                    else if (DatabaseQueries.HasEnoughPoints(e.Command.ChatMessage.Username, pointsBeingGambled) == false) //Check if they don't have enough points
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "You don't have enough points! Check with !points how many you have");
                        messageLimter.AddMessageCount();
                        break;
                    }

                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "Hey try using !spin <points> or !spin all");
                    messageLimter.AddMessageCount();
                    break;

                case "55x2":
                    long.TryParse(e.Command.ChatMessage.Message.Remove(0,5).Trim(), out var gambledPointsResult);
                    //Validation that it can be gambled
                    if (DateTime.Now - TimeSpan.FromSeconds(5) <= _55x2Cooldown)
                    {
                        var sinceListSpin = DateTime.Now - _spinCooldown;
                        //var coolDownLeft = TimeSpan.FromSeconds(5) - sinceListSpin;
                        return;
                    }

                    if (gambledPointsResult == 0 || gambledPointsResult < 500)
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => It's 500 points minimum!");
                        break;
                    }

                    if (DatabaseQueries.HasEnoughPoints(e.Command.ChatMessage.Username, gambledPointsResult) == false)
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => You don't have enough points!");
                        messageLimter.AddMessageCount();
                        break;
                    }

                    var random = new Random().Next(1,101);
                    if (random > 55)
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username}=> You rolled a {random} and won! You have won {gambledPointsResult * 2} points");
                        DatabaseQueries.AddUserPoints(e.Command.ChatMessage.Username, gambledPointsResult * 2);
                        messageLimter.AddMessageCount();
                        _55x2Cooldown = DateTime.Now;
                        break;
                    }
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username}=> You rolled a {random} and lost :( You have lost {gambledPointsResult} points");
                    DatabaseQueries.RemoveUserPoints(e.Command.ChatMessage.Username, gambledPointsResult);
                    messageLimter.AddMessageCount();
                    _55x2Cooldown = DateTime.Now;
                    break;

                case "jackpot":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,$"The current jackpot is {DatabaseQueries.GetJackpotAmount()} points");
                    break;

                case "sr":
                    int srCooldown;

                    if (e.Command.ChatMessage.Username.ToLower() == "guinea")
                    {
                        srCooldown = 0;
                    }
                    else if (e.Command.ChatMessage.IsSubscriber || e.Command.ChatMessage.IsModerator )
                    {
                        srCooldown = 3;
                    }
                    else
                    {
                        srCooldown = 6;
                    }

                    if (e.Command.ChatMessage.Message.ToLower().Contains("youtube") || e.Command.ChatMessage.Message.ToLower().Contains("youtu.be"))
                    {
                        if (DatabaseQueries.HasEnoughPoints(e.Command.ChatMessage.Username, 3000) == false)
                        {
                            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username} => You need 3000 points for a song request!");
                            messageLimter.AddMessageCount();
                            break;
                        }

                        if (SongRequest.IsSongBlacklisted(e.Command.ArgumentsAsString))
                        {
                            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => The song you have requested is blacklisted");
                            messageLimter.AddMessageCount();
                            break;
                        }

                        if (SongRequest.CheckCooldown(e.Command.ChatMessage.Username, srCooldown))
                        {
                            SongRequest.SendSong(e.Command.ArgumentsAsString, e.Command.ChatMessage.Username);
                            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => Your song has been sent");
                            DatabaseQueries.UpdateLastSongRequest(e.Command.ChatMessage.Username);
                            messageLimter.AddMessageCount();
                            break;
                        }

                        if (SongRequest.CheckCooldown(e.Command.ChatMessage.Username, srCooldown) == false)
                        {
                            break;
                        }
                    }

                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username} => You need to put in a YouTube link for a song request!");
                    messageLimter.AddMessageCount();
                    break;

                case "blacklistsong" when e.Command.ChatMessage.IsModerator:
                    SongRequest.AddBlacklistedSong(e.Command.ArgumentsAsString);
                    break;

                case "addpoints" when e.Command.ChatMessage.Username == "guinea":
                case "addpoints" when e.Command.ChatMessage.IsBroadcaster:
                    if (e.Command.ChatMessage.Message.ToLower() == "!addpoints")
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => The usage of this command is !addpoints <username> <points>");
                        break;
                    }

                    var userandPointsSplit = e.Command.ChatMessage.Message.Split(' ');
                    var usernameForPoints = userandPointsSplit[1].ToLower();
                    long.TryParse(userandPointsSplit[2], out var pointsToAdd);

                    if (pointsToAdd == 0)
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => The usage of this command is !addpoints <username> <points>");
                    }

                    DatabaseQueries.AddUserPoints(usernameForPoints, pointsToAdd);
                    break;

            }
        }
    }
}
