using System;
using BreganTwitchBot.Connection;
using BreganTwitchBot.Database;
using BreganTwitchBot.TwitchCommands.CustomCommands;
using BreganTwitchBot.TwitchCommands.DadJokes;
using BreganTwitchBot.TwitchCommands.FollowAge;
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
        private static DateTime _howLongCooldown;
        private static bool _srToggle;
        private static Giveaways _giveaway;
        private static PlayerQueueSystem _queue;
        private static WordBlackList _wordBlacklist;
        private static CommandLimiter _messageLimiter;
        private static DatabaseQueries _databaseQuery;
        private static SongRequest _songRequest;
        private static Supermod _supermod;
        private static CustomCommand _customCommand;
        private static UserFollowAge _followAge;

        public static void CommandListenerSetup()
        {
            TwitchBotConnection.Client.OnChatCommandReceived += Commands;
            TwitchBotConnection.Client.OnMessageReceived += ChatMessage;
            _srToggle = false;
            _giveaway = new Giveaways();
            _queue = new PlayerQueueSystem();
            _wordBlacklist = new WordBlackList();
            _messageLimiter = new CommandLimiter();
            _databaseQuery = new DatabaseQueries();
            _songRequest = new SongRequest();
            _supermod = new Supermod();
            _customCommand = new CustomCommand();
            _followAge = new UserFollowAge();
        }

        private static void ChatMessage(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            //Chat messages for commands that may not start with "!"
            if (_messageLimiter.CheckMessageLimit())
            {
                Log.Warning("[Message Limiter] Message Limit Hit");
                return;
            }

            var message = e.ChatMessage.Message.Split(' ');
            var command = _customCommand.GetCommand(message[0].ToLower(), e.ChatMessage.Username);

            if (command == null)
            {
                return;
            }

            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, command);
            _messageLimiter.AddMessageCount();
        }

        private static void Commands(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
        {
            //Giveaways & the player queue have multiple commands. To prevent errors it is defined before the switch statement

            //Message limit checker
            if (_messageLimiter.CheckMessageLimit())
            {
                Log.Warning("[Message Limiter] Message Limit Hit");
                return;
            }

            //Custom commands
            var command = _customCommand.GetCommand(e.Command.CommandIdentifier + e.Command.CommandText, e.Command.ChatMessage.Username);

            if (command != null)
            {
                TwitchBotConnection.Client.SendMessage(StartService.ChannelName, command);
                _messageLimiter.AddMessageCount();
                return;
            }

            //General pre-programmed commands
            switch (e.Command.CommandText.ToLower())
            {
                case "8ball":
                    var eightBall = new EightBall();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, eightBall.Ask8Ball());
                    _messageLimiter.AddMessageCount();
                    break;

                case "dadjoke":
                    var dadJoke = new DadJoke();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, dadJoke.DadJokeGenerate().Result);
                    _messageLimiter.AddMessageCount();
                    break;

                case "commands":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => You can find the commands at https://github.com/Bregann/Bregan-Twitchbot#bregan-twitchbot");
                    _messageLimiter.AddMessageCount();
                    break;

                case "pitchfork":
                    var randomUser = new RandomUsers();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username} just pitchforked -------E {randomUser.SelectRandomUser()}");
                    _messageLimiter.AddMessageCount();
                    break;

                case "shoutout" when e.Command.ChatMessage.IsModerator:
                case "shoutout" when e.Command.ChatMessage.IsBroadcaster:
                case "so" when e.Command.ChatMessage.IsModerator:
                case "so" when e.Command.ChatMessage.IsBroadcaster:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Hey go check out {e.Command.ArgumentsAsString.Replace("@", "")} at twitch.tv/{e.Command.ArgumentsAsString.Replace("@", "").Trim()} for some great content!");
                    _messageLimiter.AddMessageCount();
                    break;

                case "howlong":

                    if (DateTime.Now - TimeSpan.FromSeconds(2) <= _howLongCooldown)
                    {
                        return;
                    }

                    if (e.Command.ChatMessage.Message.ToLower() == "!howlong")
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, _followAge.GetFollowAge(e.Command.ChatMessage.Username));
                        _howLongCooldown = DateTime.Now;
                        _messageLimiter.AddMessageCount();
                        break;
                    }

                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, _followAge.GetOtherUserFollowAge(e.Command.ChatMessage.Username, e.Command.ArgumentsAsList[0]));
                    _howLongCooldown = DateTime.Now;
                    _messageLimiter.AddMessageCount();
                    break;
                //Giveaway commands
                case "startgiveaway" when e.Command.ChatMessage.IsModerator:
                case "startgiveaway" when e.Command.ChatMessage.IsBroadcaster:
                    _giveaway.StartGiveaway();
                    break;

                case "joingiveaway":
                    _giveaway.AddContestant(e.Command.ChatMessage.Username);
                    break;

                case "amountentered" when e.Command.ChatMessage.IsModerator:
                case "amountentered" when e.Command.ChatMessage.IsBroadcaster:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{_giveaway.AmountOfContestantsEntered()}");
                    _messageLimiter.AddMessageCount();
                    break;

                case "setgiveawaytime" when e.Command.ChatMessage.IsModerator:
                case "setgiveawaytime" when e.Command.ChatMessage.IsBroadcaster:
                    _giveaway.SetTimerAmount(e.Command.ArgumentsAsString, e.Command.ChatMessage.Username);
                    break;

                case "reroll" when e.Command.ChatMessage.IsModerator:
                case "reroll" when e.Command.ChatMessage.IsBroadcaster:
                    _giveaway.ReRoll();
                    break;

                //Queue Commands
                case "joinqueue" when _queue.QueueUserCheck(e.Command.ChatMessage.Username) == false:
                    _queue.QueueAdd(e.Command.ChatMessage.Username);
                    break;

                case "leavequeue":
                    _queue.QueueRemove(e.Command.ChatMessage.Username);
                    break;

                case "queue":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"The current queue is {_queue.CurrentQueue()}");
                    _messageLimiter.AddMessageCount();
                    break;

                case "nextgame":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"The next players for the game are {_queue.NextGamePlayers()}");
                    _messageLimiter.AddMessageCount();
                    break;

                case "queueposition":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, _queue.GetQueuePosition(e.Command.ChatMessage.Username));
                    _messageLimiter.AddMessageCount();
                    break;

                case "removegame" when e.Command.ChatMessage.IsModerator:
                case "removegame" when e.Command.ChatMessage.IsBroadcaster:
                    _queue.QueueRemovePlayersAmount();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username}: the current players have been removed");
                    _messageLimiter.AddMessageCount();
                    break;

                case "clearqueue" when e.Command.ChatMessage.IsModerator:
                case "clearqueue" when e.Command.ChatMessage.IsBroadcaster:
                    _queue.QueueClear();
                    break;

                case "setremoveamount" when e.Command.ChatMessage.IsModerator:
                case "setremoveamount" when e.Command.ChatMessage.IsBroadcaster:
                    _queue.SetQueueRemoveAmount(e.Command.ArgumentsAsString);
                    break;

                //Bad word filter
                case "addbadword" when e.Command.ChatMessage.IsModerator:
                    _wordBlacklist.AddBadWord(e.Command.ArgumentsAsString);
                    break;

                case "removebadword" when e.Command.ChatMessage.IsModerator:
                    _wordBlacklist.RemoveBadWord(e.Command.ArgumentsAsString);
                    break;

                    //Points/time
                case "points":
                    if (e.Command.ChatMessage.Message.ToLower() == "!points")
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => You have {_databaseQuery.GetUserPoints(e.Command.ChatMessage.Username):N0} points");
                        break;
                    }

                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => {e.Command.ArgumentsAsString.Replace("@", "")} has {_databaseQuery.GetUserPoints(e.Command.ArgumentsAsString.ToLower().Replace("@", "")):N0} points");
                    _messageLimiter.AddMessageCount();
                    break;

                case "hours":
                case "hrs":
                    if (e.Command.ChatMessage.Message.ToLower() == "!hours" || e.Command.ChatMessage.Message.ToLower() == "!hrs") //If the user is checking their own time
                    {
                        var time = _databaseQuery.GetUserTime(e.Command.ChatMessage.Username);
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username} => You have {time.TotalMinutes} minutes (about {Math.Round(time.TotalMinutes / 60, 2)} hours) in the stream");
                        _messageLimiter.AddMessageCount();
                        break;
                    }
                    
                    var otherUserTime = _databaseQuery.GetUserTime(e.Command.ArgumentsAsString.Replace("@","").ToLower());
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username} => {e.Command.ArgumentsAsString.Replace("@", "")} has {otherUserTime.TotalMinutes} minutes (about {Math.Round(otherUserTime.TotalMinutes / 60, 2)} hours) in the stream");
                    _messageLimiter.AddMessageCount();
                    break;

                //Leaderboards for points/hours
                case "pointslb":
                    var points = _databaseQuery.GetTopPoints();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, points);
                    _messageLimiter.AddMessageCount();
                    break;

                case "hourslb":
                    var hours = _databaseQuery.GetTopHours();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, hours);
                    _messageLimiter.AddMessageCount();
                    break;

                //Gamble
                case "gamble":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,"Use !spin <points> or !spin all if you want to risk them all!");
                    break;

                case "spin":
                case "slots":
                    //Cooldown Check
                    if (DateTime.Now - TimeSpan.FromSeconds(10) <= _gambleCooldown)
                    {
                        //var sinceListSpin = DateTime.Now - _spinCooldown;
                        //var coolDownLeft = TimeSpan.FromSeconds(5) - sinceListSpin;
                        return;
                   }

                    if (e.Command.ChatMessage.Message.ToLower() == "!spin all")
                    {
                        var userPointAmount = _databaseQuery.GetUserPoints(e.Command.ChatMessage.Username);
                        if (userPointAmount < 100)
                        {
                            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "It's 100 points minimum!");
                            _messageLimiter.AddMessageCount();
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

                    if (intConvert && pointsBeingGambled >= 100 && _databaseQuery.HasEnoughPoints(e.Command.ChatMessage.Username, pointsBeingGambled))
                    {
                        var slotMachine = new SlotMachine();
                        slotMachine.SpinSlotMachine(e.Command.ChatMessage.Username, pointsBeingGambled);
                        _gambleCooldown = DateTime.Now;
                        break;
                    }
                    else if(intConvert && pointsBeingGambled <= 100) //Minimum on the points to stop abuse
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "It's 100 points minimum!");
                        _messageLimiter.AddMessageCount();
                        break;
                    }
                    else if (_databaseQuery.HasEnoughPoints(e.Command.ChatMessage.Username, pointsBeingGambled) == false) //Check if they don't have enough points
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "You don't have enough points! Check with !points how many you have");
                        _messageLimiter.AddMessageCount();
                        break;
                    }

                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "Hey try using !spin <points> or !spin all");
                    _messageLimiter.AddMessageCount();
                    break;

                case "flip":
                    long.TryParse(e.Command.ArgumentsAsString, out var gambledPointsResult);

                    //Validation that it can be gambled
                    if (DateTime.Now - TimeSpan.FromSeconds(10) <= _gambleCooldown)
                    {
                        return;
                    }

                    if (gambledPointsResult == 0 || gambledPointsResult < 500)
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => It's 500 points minimum!");
                        _messageLimiter.AddMessageCount();
                        break;
                    }

                    if (_databaseQuery.HasEnoughPoints(e.Command.ChatMessage.Username, gambledPointsResult) == false)
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => You don't have enough points!");
                        _messageLimiter.AddMessageCount();
                        break;
                    }

                    var random = new Random().Next(1,101);
                    if (random > 60)
                    {
                        var pointsWon = (long) Math.Round(gambledPointsResult * 1.5);
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username}=> You flipped heads and won! You have won {pointsWon:N0} points");
                        _databaseQuery.AddUserPoints(e.Command.ChatMessage.Username, pointsWon);
                        _messageLimiter.AddMessageCount();
                        _gambleCooldown = DateTime.Now;
                        break;
                    }

                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username}=> You flipped tails and lost :( You have lost {gambledPointsResult:N0} points");
                    _databaseQuery.RemoveUserPoints(e.Command.ChatMessage.Username, gambledPointsResult);
                    _messageLimiter.AddMessageCount();
                    _gambleCooldown = DateTime.Now;
                    break;

                case "jackpot":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"The current jackpot is {_databaseQuery.GetJackpotAmount():N0} points");
                    _messageLimiter.AddMessageCount();
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
                        break;
                    }

                    //Find out how much the SR is. Supermods get free song requests as they are trusted
                    if (_supermod.CheckSupermod(e.Command.ChatMessage.Username))
                    {
                        _songRequest.SendSong(e.Command.ArgumentsAsString, e.Command.ChatMessage.Username, 0);
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => Your song has been sent");
                        _databaseQuery.UpdateLastSongRequest(e.Command.ChatMessage.Username);
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
                    if (_databaseQuery.HasEnoughPoints(e.Command.ChatMessage.Username, srCost) == false)
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username} => You need {srCost:N0} points for a song request!");
                        _messageLimiter.AddMessageCount();
                        break;
                    }


                    if (e.Command.ChatMessage.Message.ToLower().Contains("youtube") || e.Command.ChatMessage.Message.ToLower().Contains("youtu.be"))
                    {

                        if (_songRequest.IsSongBlacklisted(e.Command.ArgumentsAsString))
                        {
                            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => The song you have requested is blacklisted");
                            _messageLimiter.AddMessageCount();
                            break;
                        }

                        //If they are on cooldown the message will be sent in the CheckCooldown method
                        if (_songRequest.CheckCooldown(e.Command.ChatMessage.Username, srCooldown) == false)
                        {
                            break;
                        }

                        if (_songRequest.CheckCooldown(e.Command.ChatMessage.Username, srCooldown))
                        {
                            _songRequest.SendSong(e.Command.ArgumentsAsString, e.Command.ChatMessage.Username, srCost);
                            TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => Your song has been sent");
                            _databaseQuery.UpdateLastSongRequest(e.Command.ChatMessage.Username);
                            _messageLimiter.AddMessageCount();
                            break;
                        }
                    }

                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username} => You need to put in a YouTube link for a song request!");
                    _messageLimiter.AddMessageCount();
                    break;

                case "togglesr" when _supermod.CheckSupermod(e.Command.ChatMessage.Username):
                case "togglesr" when e.Command.ChatMessage.IsBroadcaster:
                case "srtoggle" when _supermod.CheckSupermod(e.Command.ChatMessage.Username):
                case "srtoggle" when e.Command.ChatMessage.IsBroadcaster:

                    _srToggle = !_srToggle;

                    if (_srToggle)
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => Song requests have been disabled");
                        _messageLimiter.AddMessageCount();
                    }
                    else
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => Song requests have been enabled");
                        _messageLimiter.AddMessageCount();
                    }

                    break;
                case "blacklistsong" when e.Command.ChatMessage.IsModerator:
                    _songRequest.AddBlacklistedSong(e.Command.ArgumentsAsString);
                    break;

                case "addpoints" when _supermod.CheckSupermod(e.Command.ChatMessage.Username):
                case "addpoints" when e.Command.ChatMessage.IsBroadcaster:
                    if (e.Command.ChatMessage.Message.ToLower() == "!addpoints")
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => The usage of this command is !addpoints <username> <points>");
                        _messageLimiter.AddMessageCount();
                        break;
                    }

                    var userandPointsSplit = e.Command.ArgumentsAsList;
                    var usernameForPoints = userandPointsSplit[0].ToLower();
                    long.TryParse(userandPointsSplit[1], out var pointsToAdd);

                    if (pointsToAdd == 0)
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => The usage of this command is !addpoints <username> <points>");
                        _messageLimiter.AddMessageCount();
                    }

                    _databaseQuery.AddUserPoints(usernameForPoints, pointsToAdd);
                    break;

                //Supermods
                case "addsupermod" when _supermod.CheckSupermod(e.Command.ChatMessage.Username):
                case "addsupermod" when e.Command.ChatMessage.Username == e.Command.ChatMessage.BotUsername:
                    _supermod.AddSupermod(e.Command.ArgumentsAsList[0].ToLower(), e.Command.ChatMessage.Username);
                    break;
                case "removesupermod" when e.Command.ChatMessage.IsBroadcaster:
                case "removesupermod" when e.Command.ChatMessage.Username == e.Command.ChatMessage.BotUsername:
                    _supermod.RemoveSupermod(e.Command.ArgumentsAsList[0].ToLower(), e.Command.ChatMessage.Username);
                    break;

                //Custom commands
                case "addcmd" when e.Command.ChatMessage.IsModerator:
                case "addcmd" when e.Command.ChatMessage.IsBroadcaster:
                case "cmdadd" when e.Command.ChatMessage.IsModerator:
                case "addcmd" when e.Command.ChatMessage.IsBroadcaster:

                    //Explain how it works
                    if (e.Command.ChatMessage.Message == "!addcmd" || e.Command.ChatMessage.Message == "!cmdadd")
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => The usage for this command is !addcmd <command name> <command text>");
                        break;
                    }

                    //Prevent people from adding the add command command as a command
                    if (e.Command.ChatMessage.Message.StartsWith("!addcmd !addcmd") || e.Command.ChatMessage.Message.StartsWith("!cmdadd !cmdadd"))
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => You cannot add the add command as a command!");
                        _messageLimiter.AddMessageCount();
                        break;
                    }

                    var commandName = e.Command.ArgumentsAsList;
                    var commandText = e.Command.ArgumentsAsString.Remove(0, commandName[0].Length);

                    if (commandText == "")
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => You need to add command text!");
                        _messageLimiter.AddMessageCount();
                        break;
                    }

                    if (_customCommand.AddNewCommand(commandName[0].ToLower(), commandText))
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => {commandName[0].ToLower()} has been successfully added!");
                        _messageLimiter.AddMessageCount();
                        break;
                    }

                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $@"{e.Command.ChatMessage.Username} => That command already exists!");
                    _messageLimiter.AddMessageCount();
                    break;

                case "delcmd" when e.Command.ChatMessage.IsBroadcaster:
                case "delcmd" when e.Command.ChatMessage.IsModerator:
                case "cmddel" when e.Command.ChatMessage.IsBroadcaster:
                case "cmddel" when e.Command.ChatMessage.IsModerator:

                    //Explain how it works
                    if (e.Command.ChatMessage.Message == "!delcmd" || e.Command.ChatMessage.Message == "!cmddel")
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => The usage for this command is !delcmd <command name>");
                        _messageLimiter.AddMessageCount();
                        break;
                    }

                    var commandNameToRemove = e.Command.ArgumentsAsList;

                    if (!_customCommand.RemoveCommand(commandNameToRemove[0]))
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => That command does not exist!");
                        break;
                    }

                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => The command {commandNameToRemove[0]} has been removed!");
                    break;

                case "editcmd" when e.Command.ChatMessage.IsBroadcaster:
                case "editcmd" when e.Command.ChatMessage.IsModerator:
                case "cmdedit" when e.Command.ChatMessage.IsBroadcaster:
                case "cmdedit" when e.Command.ChatMessage.IsModerator:
                    if (e.Command.ChatMessage.Message == "!editcmd" || e.Command.ChatMessage.Message == "!cmdedit")
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => The usage for this command is !editcmd <command name> <command text>");
                        _messageLimiter.AddMessageCount();
                        break;
                    }

                    var commandNameToEdit = e.Command.ArgumentsAsList;
                    var commandTextToEdit = e.Command.ArgumentsAsString.Replace(commandNameToEdit[0], "").Trim();

                    if (commandTextToEdit == "")
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => You need to supply some command text!");
                        _messageLimiter.AddMessageCount();
                        break;
                    }

                    if (!_customCommand.EditCommand(commandNameToEdit[0], commandTextToEdit))
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => That command does not exist!");
                        _messageLimiter.AddMessageCount();
                        break;
                    }

                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => The command {commandNameToEdit[0]} has been edited!");
                    _messageLimiter.AddMessageCount();
                    break;
            }
        }
    }
}
