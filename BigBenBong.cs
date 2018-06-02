using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Twitch_Bot
{
    internal class BigBenBong
    {
        private static bool _hasBongedThisHour;
        public static void Bong()
        {
            _hasBongedThisHour = false;
            var timer = new System.Timers.Timer(15000);
            timer.Start();
            timer.Elapsed += TimeCheck;
            

        }

        private static void TimeCheck(object sender, System.Timers.ElapsedEventArgs e)
        {
            //If minute is not 0 then don't proceed
            if (DateTime.Now.Minute != 59)
            {
                _hasBongedThisHour = false;
                return;
            }
            //If the time is 0 and already bonged then no bong again
            if (DateTime.Now.Minute == 59 && _hasBongedThisHour)
            {
                return;
            }
            switch (DateTime.Now.Hour)
            {
                case 1:
                case 13:
                {
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName, ":clock1: BONG");
                    break;
                }
                case 2:
                case 14:
                {
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName,
                        ":clock2: BONG BONG");
                    break;
                }
                case 3:
                case 15:
                {
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName,
                        ":clock3: BONG BONG BONG");
                    break;
                }
                case 4:
                case 16:
                {
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName,
                        ":clock4: BONG BONG BONG BONG");
                    break;
                }
                case 5:
                case 17:
                {
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName,
                        ":clock5: BONG BONG BONG BONG BONG");
                    break;
                }
                case 6:
                case 18:
                {
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName,
                        ":clock6: BONG BONG BONG BONG BONG BONG");
                    break;
                }

                case 7:
                case 19:
                {
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName,
                        ":clock7: BONG BONG BONG BONG BONG BONG BONG");
                        
                        break;
                }

                case 8:
                case 20:
                {
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName,
                        ":clock8: BONG BONG BONG BONG BONG BONG BONG BONG");
                    break;
                }

                case 9:
                case 21:
                {
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName,
                        ":clock9: BONG BONG BONG BONG BONG BONG BONG BONG BONG");
                    break;
                }
                case 10:
                case 22:
                {
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName,
                        ":clock10: BONG BONG BONG BONG BONG BONG BONG BONG BONG BONG");
                    break;
                }

                case 11:
                case 23:
                {
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName,
                        ":clock11: BONG BONG BONG BONG BONG BONG BONG BONG BONG BONG BONG");
                    break;
                }
                case 12:
                case 24:
                {
                    TwitchBotConnection.client.SendMessage(TwitchBotConnection.channelConnectName,
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
