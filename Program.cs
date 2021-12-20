using System;
using System.Threading.Tasks;
using System.IO;
using Discord.WebSocket;
using Discord;

namespace DiscordCSBot
{
    class Program
    {
        private DiscordSocketClient _client;

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            string token = String.Empty;
            try
            {
                token = File.ReadAllText("APIKey.txt");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading the file \"APIKey.txt\"");
                Console.WriteLine(e.Message);
            }

            _client = new DiscordSocketClient(
                new DiscordSocketConfig { MessageCacheSize = 100 });
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.MessageReceived += _client_MessageReceived;

            await Task.Delay(-1);
        }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            string content = arg.Content.ToLower();
            if ((content.Contains("chuck") || content.Contains("norris")) &&
                (arg.Author.Id != _client.CurrentUser.Id))
            {
                var joke = await DataSource.RandomJoke();
                await arg.Channel.SendMessageAsync("Did anyone say something about Chuck Norris?\n" +
                    $"{joke}");
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
