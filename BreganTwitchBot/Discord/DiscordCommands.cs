using System.Threading.Tasks;
using BreganTwitchBot.Database;
using Discord.WebSocket;

namespace BreganTwitchBot.Discord
{
    class DiscordCommands
    {
        public static void StartDiscordCommands()
        {
            DiscordConnection.DiscordClient.MessageReceived += MessageReceived;
        }

        private static async Task MessageReceived(SocketMessage message)
        {
            if (message.Content.StartsWith("!getall"))
            {
                var time = new TimeTracker();
                await time.GetAll();
            }
        }
    }
}
