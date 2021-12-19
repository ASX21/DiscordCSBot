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

            _client.MessageUpdated += _client_MessageUpdated;

            await Task.Delay(-1);
        }

        private async Task _client_MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            var message = await arg1.DownloadAsync();
            Console.WriteLine($"{message} => {arg2} in {arg3.Name}");
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
