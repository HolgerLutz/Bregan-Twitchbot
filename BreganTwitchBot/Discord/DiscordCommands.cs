using System.Threading.Tasks;
using BreganTwitchBot.Database;
using Discord.WebSocket;

namespace BreganTwitchBot.Discord
{
    class DiscordCommands
    {
        public static void StartDiscordCommands()
        {
            DiscordConnection.DiscordClient.MessageReceived += MessageRecieved;
        }

        private static async Task MessageRecieved(SocketMessage message)
        {
            if (message.Content.StartsWith("!getall"))
            {
                await TimeTracker.GetAll();
            }
        }
    }
}
