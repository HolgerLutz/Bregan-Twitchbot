using System;
using BreganTwitchBot.Connection;

namespace BreganTwitchBot.TwitchCommands.BigBen
{
    class BigBenBong
    {
        private static bool _hasBongedThisHour;
        public static void Bong()
        {
            _hasBongedThisHour = false;
            var timer = new System.Timers.Timer(15000); //Check every 15 secs
            timer.Start();
            timer.Elapsed += TimeCheck;
            

        }

        private static void TimeCheck(object sender, System.Timers.ElapsedEventArgs e)
        {
            //If minute is not 0 then don't proceed
            if (DateTime.Now.Minute != 0)
            {
                _hasBongedThisHour = false;
                return;
            }
            //If the time is 0 and already bonged then no bong again
            if (DateTime.Now.Minute == 0 && _hasBongedThisHour)
            {
                return;
            }
            switch (DateTime.Now.Hour) //As this uses a 24h clock both am and pm have to be accounted for
            {
                case 1:
                case 13:
                {
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName, ":clock1: BONG");
                    break;
                }
                case 2:
                case 14:
                {
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        ":clock2: BONG BONG");
                    break;
                }
                case 3:
                case 15:
                {
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        ":clock3: BONG BONG BONG");
                    break;
                }
                case 4:
                case 16:
                {
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        ":clock4: BONG BONG BONG BONG");
                    break;
                }
                case 5:
                case 17:
                {
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        ":clock5: BONG BONG BONG BONG BONG");
                    break;
                }
                case 6:
                case 18:
                {
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        ":clock6: BONG BONG BONG BONG BONG BONG");
                    break;
                }

                case 7:
                case 19:
                {
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        ":clock7: BONG BONG BONG BONG BONG BONG BONG");
                        
                        break;
                }

                case 8:
                case 20:
                {
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        ":clock8: BONG BONG BONG BONG BONG BONG BONG BONG");
                    break;
                }

                case 9:
                case 21:
                {
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        ":clock9: BONG BONG BONG BONG BONG BONG BONG BONG BONG");
                    break;
                }
                case 10:
                case 22:
                {
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        ":clock10: BONG BONG BONG BONG BONG BONG BONG BONG BONG BONG");
                    break;
                }

                case 11:
                case 23:
                {
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        ":clock11: BONG BONG BONG BONG BONG BONG BONG BONG BONG BONG BONG");
                    break;
                }
                case 12:
                case 24:
                {
                    TwitchBotConnection.Client.SendMessage(StartService.ChannelName,
                        ":clock12: BONG BONG BONG BONG BONG BONG BONG BONG BONG BONG BONG BONG");
                    break;
                }
            }

            _hasBongedThisHour = true;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("[Bonger] Sucessfully bonged");
            Console.ResetColor();
        }
    }
}
