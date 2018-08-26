using System;
using BreganTwitchBot.Connection;
using BreganTwitchBot.Database;
using BreganTwitchBot.TwitchCommands.DadJokes;
using BreganTwitchBot.TwitchCommands.Gambling;
using BreganTwitchBot.TwitchCommands.Giveaway;
using BreganTwitchBot.TwitchCommands.MessageLimiter;
using BreganTwitchBot.TwitchCommands.Queue;
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
            //Message limit checker
            if (CommandLimiter.MessagesSent >= CommandLimiter.MessageLimit)
            {
                Console.WriteLine("[Message Limiter] Message Limit Hit");
                return;
            }

            //General pre-programmed commands
            switch (e.Command.CommandText.ToLower())
            {
                case "8ball":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, EightBall.Ask8Ball());
                    CommandLimiter.AddMessageCount();
                    break;
                case "dadjoke":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, DadJoke.DadJokeGenerate().Result);
                    CommandLimiter.AddMessageCount();
                    break;
                case "commands":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "You can find the commands at https://github.com/Bregann/Bregan-Twitchbot#bregan-twitchbot");
                    CommandLimiter.AddMessageCount();
                    break;
                case "pitchfork":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        $"{e.Command.ChatMessage.Username} just pitchforked -------E {RandomUser.RandomUser.SelectRandomUser()}");
                    CommandLimiter.AddMessageCount();
                    break;
                case "shoutout" when e.Command.ChatMessage.IsModerator:
                case "shoutout" when e.Command.ChatMessage.IsBroadcaster:
                    var userToShoutout = e.Command.ChatMessage.Message.Replace("!shoutout @", ""); //Remove the @ if it is there
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"Hey go check out {userToShoutout.Replace("!shoutout","")} at twitch.tv/{userToShoutout.Replace("!shoutout", "").Trim()} for some great content!");
                    CommandLimiter.AddMessageCount();
                    break;

                //Giveaway commands
                case "startgiveaway" when e.Command.ChatMessage.IsModerator:
                case "startgiveaway" when e.Command.ChatMessage.IsBroadcaster:
                    Giveaways.StartGiveaway();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "A new giveaway has started! Do !joingiveaway to join!");
                    return;
                case "joingiveaway":
                    Giveaways.AddContestant(e.Command.ChatMessage.Username);
                    return;
                case "amountentered" when e.Command.ChatMessage.IsModerator:
                case "amountentered" when e.Command.ChatMessage.IsBroadcaster:
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{Giveaways.AmountOfContestantsEntered()}");
                    CommandLimiter.AddMessageCount();
                    break;
                case "setgiveawaytime" when e.Command.ChatMessage.IsModerator:
                case "setgiveawaytime" when e.Command.ChatMessage.IsBroadcaster:
                    Giveaways.SetTimerAmount(e.Command.ChatMessage.Message, e.Command.ChatMessage.Username);
                    break;
                case "reroll" when e.Command.ChatMessage.IsModerator:
                case "reroll" when e.Command.ChatMessage.IsBroadcaster:
                    Giveaways.ReRoll();
                    break;

                //Queue Commands
                case "joinqueue" when PlayerQueueSystem.QueueUserCheck(e.Command.ChatMessage.Username) == false:
                    PlayerQueueSystem.QueueAdd(e.Command.ChatMessage.Username);
                    break;
                case "leavequeue":
                    PlayerQueueSystem.QueueRemove(e.Command.ChatMessage.Username);
                    break;
                case "queue":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        $"The current queue is {PlayerQueueSystem.CurrentQueue()}");
                    CommandLimiter.AddMessageCount();
                    break;
                case "nextgame":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        $"The next players for the game are {PlayerQueueSystem.NextGamePlayers()}");
                    CommandLimiter.AddMessageCount();
                    break;
                case "queueposition":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        PlayerQueueSystem.GetQueuePosition(e.Command.ChatMessage.Username));
                    CommandLimiter.AddMessageCount();
                    break;
                case "removegame" when e.Command.ChatMessage.IsModerator:
                case "removegame" when e.Command.ChatMessage.IsBroadcaster:
                    PlayerQueueSystem.QueueRemove3();
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, 
                        $"{e.Command.ChatMessage.Username}: the current players have been removed");
                    CommandLimiter.AddMessageCount();
                    break;
                case "clearqueue" when e.Command.ChatMessage.IsModerator:
                case "clearqueue" when e.Command.ChatMessage.IsBroadcaster:
                    PlayerQueueSystem.QueueClear();
                    break;
                case "setremoveamount" when e.Command.ChatMessage.IsModerator:
                case "setremoveamount" when e.Command.ChatMessage.IsBroadcaster:
                    PlayerQueueSystem.SetQueueRemoveAmount(e.Command.ChatMessage.Message);
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, 
                        $"The remove amount has been updated to {PlayerQueueSystem.QueueRemoveAmount}");
                    CommandLimiter.AddMessageCount();
                    break;
                //Bad word filter
                case "addbadword" when e.Command.ChatMessage.IsModerator:
                    WordBlackList.AddBadWord(e.Command.ChatMessage.Message);
                    break;
                case "removebadword" when e.Command.ChatMessage.IsModerator:
                    WordBlackList.RemoveBadWord(e.Command.ChatMessage.Message);
                    break;

                    //Points/time
                case "points":
                    if (e.Command.ChatMessage.Message.ToLower() == "!points")
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName,$"@{e.Command.ChatMessage.Username} => You have {DatabaseQueries.GetUserPoints(e.Command.ChatMessage.Username).ToString()} points");
                        break;
                    }
                    var otherPointsUser = e.Command.ChatMessage.Message.Remove(0, 8).Trim().ToLower(); //if the user is checking someone elses points
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username} => {otherPointsUser} has {DatabaseQueries.GetUserPoints(otherPointsUser)} points");
                    break;
                case "hours":
                    if (e.Command.ChatMessage.Message.ToLower() == "!hours") //If the user is checking their own time
                    {
                        var time = DatabaseQueries.GetUserTime(e.Command.ChatMessage.Username);
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                            $"{e.Command.ChatMessage.Username} => You have {time.TotalMinutes} minutes (about {Math.Round(time.TotalMinutes / 60, 2)} hours) in the stream");
                        CommandLimiter.AddMessageCount();
                        break;
                    }
                    var otherUser = e.Command.ChatMessage.Message.Remove(0, 7).Trim().ToLower(); //if the user is checking someone elses time
                    var otherUserTime = DatabaseQueries.GetUserTime(otherUser);

                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        $"{e.Command.ChatMessage.Username} => {otherUser} has {otherUserTime.TotalMinutes} minutes (about {Math.Round(otherUserTime.TotalMinutes / 60, 2)} hours) in the stream");
                    CommandLimiter.AddMessageCount();
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
                        CommandLimiter.AddMessageCount();
                        break;
                    }
                    else if (DatabaseQueries.HasEnoughPoints(e.Command.ChatMessage.Username, pointsBeingGambled) == false) //Check if they don't have enough points
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "You don't have enough points! Check with !points how many you have");
                        CommandLimiter.AddMessageCount();
                        break;
                    }

                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, "Hey try using !spin <points> or !spin all");
                    CommandLimiter.AddMessageCount();
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
                        CommandLimiter.AddMessageCount();
                        break;
                    }

                    var random = new Random().Next(1,101);
                    if (random > 55)
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username}=> You rolled a {random} and won! You have won {gambledPointsResult * 2} points");
                        DatabaseQueries.AddUserPoints(e.Command.ChatMessage.Username, gambledPointsResult * 2);
                        CommandLimiter.AddMessageCount();
                        _55x2Cooldown = DateTime.Now;
                        break;
                    }
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"@{e.Command.ChatMessage.Username}=> You rolled a {random} and lost :( You have lost {gambledPointsResult} points");
                    DatabaseQueries.RemoveUserPoints(e.Command.ChatMessage.Username, gambledPointsResult);
                    CommandLimiter.AddMessageCount();
                    _55x2Cooldown = DateTime.Now;
                    break;

                case "jackpot":
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,$"The current jackpot is {DatabaseQueries.GetJackpotAmount()} points");
                    break;

                case "sr":
                case "songrequest":
                    if (!e.Command.ChatMessage.Message.ToLower().Contains("youtube"))
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username} => You need to put in a YouTube link for a song request!");
                        CommandLimiter.AddMessageCount();
                        break;
                    }

                    if (DatabaseQueries.HasEnoughPoints(e.Command.ChatMessage.Username, 3000) == false)
                    {
                        TwitchBotConnection.Client.SendMessage(StartService.ChannelName, $"{e.Command.ChatMessage.Username} => You need 3000 points for a song request!");
                        CommandLimiter.AddMessageCount();
                        break;
                    }
                    SongRequest.SendSong(e.Command.ChatMessage.Message.Remove(0,4), e.Command.ChatMessage.Username);
                    break;
            }
        }
    }
}
