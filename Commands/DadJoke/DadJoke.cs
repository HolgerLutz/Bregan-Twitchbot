using System.Threading.Tasks;
using ICanHazDadJoke.NET;

namespace Bregan_TwitchBot.Commands.DadJoke
{
    internal class DadJoke
    {
        public static async Task<string> DadJokeGenerate()
        {
            //Make dad bot work and return a joke generated
            var libraryName = "ICanHazDadJoke.NET Readme";
            var contactUri = "https://github.com/mattleibow/ICanHazDadJoke.NET";
            var dadClient = new DadJokeClient(libraryName, contactUri);
            var dadJoke = await dadClient.GetRandomJokeStringAsync();
            return dadJoke;
        }
    }
}
